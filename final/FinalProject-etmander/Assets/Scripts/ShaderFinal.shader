

/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak

    Shader for Final Project

 */




Shader "ShaderFinal" {

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
                float3 wNormal : TEXCOORD0; //normal
                float3 lDir : TEXCOORD1; //light 1 pos
                float3 rDir : TEXCOORD2; //light 2 pos
                float3 cDir : TEXCOORD3; //camera
            };
            
            //light position given by CPU
            float3 _Light;
            float3 _RLight;
            //camera pos
            float3 _Cam;
            //light color
            float4 _Light_Color;
            float4 _R_Light_Color;
            //spline color given by CPU
            fixed4 _Color;
            //light material coefficients
            float _aI; //ambient
            float _dI; //diffuse
            float _sI; //specular

            float _lI; //light 1 intensity
            float _rI; //light 2 intensity

            //bool for whether we render in frag or vert shader
            float _VorF;

            // ---------------------------------------------------------
            // the Vertex Shaders outputs positions on the Spline Curve:
            v2f vert (appdata v) {
            
                // the output to this shader is:
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                //[ Ambientlight*Ambientmaterial + Diffuselight*Diffusematerial*max(0,L·Nvert) + Speclight*Specmaterial*|max(0,B·Nvert)|shininess]
               
                // get the direction vector of v to Light
                float3 L = _Light - v.vertex;
                float3 RL = (_RLight) - v.vertex;
                //get direction vector of v to viewport
                float3 V = -(mul(UNITY_MATRIX_MV, v.vertex));
                V = normalize(V);
                L = normalize(L);
                RL = normalize(RL);

                //get unit vector normal for v, taking into account coord difference
                float3 N = normalize(mul(v.normal, unity_WorldToObject));

                //lambert (diffuse)
                float Lam = max(0, dot(N, L));
                float RLam = max(0, dot(N, RL));

                //ambient
                float3 Amb = _Color;

                //spec
                float3 B = reflect(-L, N); 
                float3 RB = reflect(-RL, N);//(C + L)/normalize(C + L); //reflection vector of lightsource and normal
                float refl = max(0, dot(B, V)); //spec calculation
                float Rrefl = max(0, dot(RB, V)); //spec calculation
                float3 spec = pow(refl, .4) * _Color; 
                float3 Rspec = pow(Rrefl, .4) * _Color; 

                //if no direct light, set spec to 0
                if(Lam <= 0){
                    spec = 0;
                }

                if(RLam <= 0){
                    Rspec = 0;
                }

                //pass vertex vectors to fragment shader
                o.wNormal = N; //normal
                o.lDir = L; //direction to light
                o.rDir = RL;
                o.cDir = V; //direction to viewer

                
                float3 base = _aI * Amb + _dI * (_Light_Color * (Lam * _Color)) + _sI * (_Light_Color * spec); //lighting equation
                float3 red = _aI * Amb + _dI * (_R_Light_Color * (RLam * _Color)) + _sI * (_R_Light_Color * Rspec);

                if(_VorF == 0){ //determine if we're passing computed or non computed color to fragment shader, dependent on renerer type
                    o.vColor = _Color; //use fragment
                }
                else{
                    o.vColor = (_lI *base) + (_rI * red); //use vertex, combine lightings
                }
                
                return o;
            } // end of vert shader


            // -------------------------------------------------
            // the Fragment Shader

            fixed3 frag (v2f i) : SV_Target {

                //lambert (diffuse)
                float Lam = max(0, dot(i.wNormal, i.lDir));
                float RLam = max(0, dot(i.wNormal, i.rDir));

                //ambient
                float3 amb = i.vColor;

                //specularity
                float3 B = reflect(-i.lDir, i.wNormal);
                float3 RB = reflect(-i.rDir, i.wNormal);

                float refl = max(0, dot(B, i.cDir));
                float Rrefl = max(0, dot(RB, i.cDir));

                float3 spec = pow(refl, .4) * i.vColor;
                float3 Rspec = pow(Rrefl, .4) * i.vColor;

                //if no direct light, set spec to 0
                if(Lam <= 0){
                    spec = 0;
                }

                if(RLam <= 0){
                    Rspec = 0;
                }

                //lighting equation
                float3 base = _aI * amb + _dI * (_Light_Color * (Lam * i.vColor)) + _sI * (_Light_Color * spec);
                float3 red = _aI * amb + _dI * (_R_Light_Color * (RLam * i.vColor)) + _sI * (_R_Light_Color * Rspec);

                if(_VorF == 0){
                    //weights of .2, .5, .7 respectively
                    return (_lI *base) + (_rI * red); //use frag, combine lightings
                }
                else{
                    return i.vColor; //use vert
                }
                
            } // end of frag shader

            ENDCG
        }
    }
}