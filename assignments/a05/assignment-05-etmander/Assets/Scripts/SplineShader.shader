/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak

    The Vertex Shader component in this Unity Shader should:

    use t1, t2 values to compute the position of each 
       vertex on the Spline Curve:
    t1 is for the "base curve",
    t2 is for the "offset curve".

    to calculate (on the GPU) the vertices on a single Spline Curve segment,
    to be displayed as a Mesh, using a Mesh Renderer.

    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */




Shader "SplineVertexShader" {

    Properties { }
    
    SubShader {
    
        cull off
        
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        
        Pass  {
        
            CGPROGRAM
            
            // we'll provide both Vertex and Fragment shaders:
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            // "varying" struct of variables,
            //     passed from vertex shader
            //     to fragment shader:
            struct v2f {
                float4 vertex : SV_POSITION;
                // the color output from the vertex shader:
                fixed3 vColor : COLOR0;
                //pass from vert
                float3 lColor : COLOR1;
                float3 wNormal : TEXCOORD0;
                float3 lDir : TEXCOORD1;
                float3 cDir : TEXCOORD2;
            };
            
            // here we receive (from application, CPU side) the spline Hermite form matrix:
            float4x4 _SplineMatrix;
            
            // here we receive (from application, CPU side) the four control points:
            float3 _Control0, _Control1, _Control2, _Control3;
            
            // here we receive (from application, CPU side) how many steps there should be on curve:
            float _Step;

            //light position given by CPU
            float3 _Light;
            //camera pos
            float3 _Cam;
            //light color
            float4 _Light_Color;
            //spline color given by CPU
            fixed4 _Color;

            //bool for whether we render in frag or vert shader
            float _VorF;
            
            

            // ---------------------------------------------------------
            // the main spline calculation happens here, i.e.
            //   the multiplication of the Hermite form (ch. 11.5.1 / 10.5.1 in textbook)
            //   as per Assignment 02 instructions:
            //   p(u) = uT * M * p
            //   
            //   Note: here we name the parameter t instead of u (that's in the textbook)
            //   Note2: matrices are defined in "column major" !
            //
            float4 HermiteMult(float4x4 controlP, float4x4 splineM, float4 tParamsVector) {
                float4 vertexOnCurve = mul(mul(controlP, splineM), tParamsVector); 
                return vertexOnCurve;
            } // end of HermiteMult()



            // ---------------------------------------------------------
            // compute the Normal to the curve, at vertex provided by parameter t:
            // 
            float2 GetNormalToCurve(float t, float dt, float4x4 controlMatrix, float4x4 splineMatrix) {
                //
                // compute normal to segment (p1, p2) on curve
                //        where p1 is the vertex on curve at parameter t
                //        where p2 is the vertex on curve at parameter t2 = t + dt
                //        n = ( -(y2 - y1) , (x2 - x1) ) / length of N
                // ADDITIONAL NOTE:
                //      The resulting normal should be normalized before returning it!
                //      And the returned value should be a float2 (not a float4!)

                float4 p1 = HermiteMult(controlMatrix, splineMatrix, float4(t*t*t, t*t, t, 1));
                float tt = t + dt;
                float4 p2 = HermiteMult(controlMatrix, splineMatrix, float4(tt*tt*tt, tt*tt, tt, 1));

                float ny = p2.y-p1.y;
                float nx = p2.x-p1.x;

                float2 normal = float2(-ny, nx);

                //float length = sqrt(pow((p2.x-p1.x), 2) + pow((p2.y-p1.y), 2));

                float2 Nnormal = normalize(normal);
                return Nnormal;
            }  // end of GetNormalToCurve() 
            


            // ---------------------------------------------------------
            // the Vertex Shaders outputs positions on the Spline Curve:
            v2f vert (appdata v) {
            
                // the output to this shader is:
                v2f o;
                
                // in the vertex shader,
                //  we receive t in the x parameter,
                //  and the offset in the y parameter:
                float t = v.vertex.x;
                float offset = v.vertex.y;
                float4 tRow = float4(t*t*t, t*t, t, 1);

                //
                // compute matrix of four Control Points for spline:
                //
                float4x4 controlMatrix = float4x4(
                    _Control0.x, _Control1.x, _Control2.x, _Control3.x,
                    _Control0.y, _Control1.y, _Control2.y, _Control3.y,
                    0, 0, 0, 0,
                    1, 1, 1, 1
                );

                // base spline:
                float4 worldPosition = HermiteMult(controlMatrix, _SplineMatrix, tRow);

                // normal to obtain point on offset curve:
                float2 normal = GetNormalToCurve(t, _Step, controlMatrix, _SplineMatrix);

                // the following will do nothing when offset is 0,
                //         i.e. vertex on "base curve",
                // but it will move the vertex in the direction of the normal
                //         for the vertex on "offset curve":
                worldPosition.xy += normal * offset;

                worldPosition.z = 0;
                worldPosition.w = 1;

                o.vertex = mul(UNITY_MATRIX_VP, worldPosition);
                ///////////////////////////////////////////////////////

                // get the direction vector of v to Light
                float3 L = _Light - v.vertex;
                //get direction vector of v to viewport
                float3 V = -(mul(UNITY_MATRIX_MV, v.vertex));
                V = normalize(V);
                L = normalize(L);
                
                //get unit vector normal for v, taking into account coord difference
                float3 N = normalize(mul(v.normal, unity_WorldToObject));

                //lambert (diffuse)
                float Lam = max(0, dot(N, L));

                //ambient
                float3 Amb = mul(_Color, _Light_Color);

                //spec
                float3 B = reflect(-L, N);  //reflection vector of lightsource and normal
                float refl = max(0, dot(B, V)); //spec calculation
                float3 spec = mul(pow(refl, .7), _Color); //.7 shininess

                //if no direct light, set spec to 0
                if(Lam < 0){
                    spec = 0;
                }

                //[ Ambientlight*Ambientmaterial + Diffuselight*Diffusematerial*max(0,L·Nvert) + Speclight*Specmaterial*|max(0,B·Nvert)|shininess]
            
                float3 color = _Color; //default color

                //pass vertex vectors to fragment shader
                o.wNormal = N; //normal
                o.lDir = L; //direction to light
                o.lColor = _Light_Color; //light color
                o.cDir = V; //direction to viewer

                if(_VorF == 0){ //determine if we're passing computed or non computed color to fragment shader, dependent on renerer type
                    o.vColor = (color.x, color.y, color.z); //use fragment
                }
                else{
                    o.vColor = (.1 * Amb + .5 * mul(Lam, _Color) + .7 * spec); //use vertex, weights/coefficents of .1, .5, .7 for each property respectively
                }
                
                return o;
            } // end of vert shader


            // -------------------------------------------------
            // the Fragment Shader

            fixed3 frag (v2f i) : SV_Target {

                //lambert diffuse
                float3 lam = .5 * max(0, dot(i.wNormal, i.lDir));
                //ambient
                float3 amb = .2 * mul(i.vColor, i.lColor);
                //specularity
                float3 B = reflect(-i.lDir, i.wNormal);
                float refl = max(0, dot(B, i.cDir));
                float3 spec = .7 * mul(pow(refl, .7), i.vColor);

                if(max(0, dot(i.wNormal, i.lDir)) < 0){
                    spec = 0;
                }

                if(_VorF == 0){
                    //weights of .2, .5, .7 respectively
                    return (amb + i.vColor * lam + spec); //use frag
                }
                else{
                    return i.vColor; //use vert
                }
                
            } // end of frag shader

            ENDCG
        }
    }
}