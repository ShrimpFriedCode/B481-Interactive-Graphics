/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    This script needs to:
    prepare a meshRenderer and connect it to a Material.
    The Material will be implemented in a Vertex Shader,
    to calculate (on the GPU) the vertices on a single Spline Curve segment,
    to be displayed as a Mesh, using a Mesh Renderer.
    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */

using UnityEngine;

namespace A04 {

    public class SplineSegmentGPUCompute : MonoBehaviour {

        public enum RendererType { vertex, fragment }

        // specify the name of the Vertex Shader to be used:
        private const string shaderName = "SplineVertexShader";
        
        // control points for a single Spline Curve segment:
        [SerializeField] private Transform control0, control1, control2, control3;
        //view center
        [SerializeField] private Transform center;
        // choice of Spline Curve type:
        [SerializeField] private SplineParameters.SplineType splineType;
        //renderer
        [SerializeField] private RendererType renderer;
        // only one line renderer: the control polyline:
        [SerializeField] private LineRenderer controlPolyLine;
        //light source pos and color
        [SerializeField] private GameObject light;
        //camera pos
        [SerializeField] private GameObject camera;
        
        // what color should the Spline Curve be?
        [SerializeField] private Color splineColor = new Color(0f, 0f, 0f);

        // how wide should the Spline Curve be?
        [SerializeField] private float splineWidth = 0.1f;

        // how many vertices on the spline curve?
        //   (the more vertices you set, the smoother the curve will be)
        [Range(8, 512)] [SerializeField] private int verticesOnCurve = 64;

        //animation vectors
        public Vector3 cp0, cp1, cp2, cp3, cam, ls, le, lDest;

        //rotation angle for circular animation
        public float rotationCp1, rotationCp3, rotationCam;


        // the Spline Curve will be rendered by a MeshRenderer,
        //   (and the vertices for the Mesh Renderer
        //   will be computed in our Vertex Shader)
        private MeshRenderer meshRenderer;
        
        // The Mesh Filter is meant to take a mesh from assets
        //    and pass it to the Mesh Renderer for rendering on the screen.
        // However, we create the mesh in this script,
        //    before the Mesh Filter passes it to the Mesh Renderer:
        private MeshFilter meshFilter;
        
        // the Vertex Shader will be considered a "Material" for rendering purposes:
        private Material material;
        
        // the Mesh to be rendered:
        private Mesh mesh;

        public void SetType(SplineParameters.SplineType type) {
            splineType = type;
        }
        
        public void UseBezier() => SetType(SplineParameters.SplineType.Bezier);
        
        public void UseCatmullRom() => SetType(SplineParameters.SplineType.CatmullRom);
        
        public void UseB() => SetType(SplineParameters.SplineType.Bspline);

        // ---------------------------------------------------------
        // set up the renderer, the first time this object is instantiated in the scene:
        private void Awake() {
        
            // obtain Mesh Renderer and Mesh Filter components from Unity scene:
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();

            // find the vertex shader that will compute Spline curve vertices:
            material = new Material(Shader.Find(shaderName));
            
            // connect our MeshRenderer to the Vertex Shader:
            meshRenderer.material = material;

            // instantiate required vertices and triangles for the Mesh:
            Vector3[] vertices = new Vector3[verticesOnCurve * 2];
            int[] triangles = new int[verticesOnCurve * 6 - 6];
            
            for (int i = 0; i < verticesOnCurve; i++) {
            
                
                // parameter for vertices on "base spline curve":
                float t1 = (float)i / (float)(verticesOnCurve - 1);

                // parameter for vertices on "offset spline curve":
                float t2 = (float)i / (float)(verticesOnCurve - 1);

                // the "trick" is to pass the parameters t1 and t2
                //   (for Spline Curve computation in the Vertex Shader)
                // as .x components in the vertices.
                
                // we also use the .y components to pass another value
                //   used to compute the "offset spline curve" vertices (see below)
                
                // the Vertex Shader will receive the t1, t2 parameters
                // and use t1, t2 values to compute the position of each 
                // vertex on the Spline Curve.
                
                // vertices on "base spline curve":
                vertices[2 * i].x = t1;
                vertices[2 * i].y = 0;

                // vertices on "offset spline curve":
                vertices[2 * i + 1].x = t2;
                vertices[2 * i + 1].y = splineWidth;

                if (i < verticesOnCurve - 1) {
                
                    // triangle with last side on "base spline curve"
                    // i.e. vertex 2 to vertex 0:
                    triangles[6 * i] = 2 * i;
                    triangles[6 * i + 1] = 2 * i + 1;
                    triangles[6 * i + 2] = 2 * i + 2;

                    // triangle with one side on "offset spline curve"
                    // i.e. vertex 1 to vertex to vertex 3:
                    triangles[6 * i + 3] = 2 * i + 1;
                    triangles[6 * i + 4] = 2 * i + 3;
                    triangles[6 * i + 5] = 2 * i + 2;
                }
            }
            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;
            meshRenderer.sortingLayerName = "Default";
            meshRenderer.sortingOrder = 1;

            //set animation vectors
            cp0 = new Vector3(2, -2, 0); 
            cp1 = control1.position;
            cp2 = new Vector3(2, 1, 0);
            cp3 = control3.position;
            cam = center.position;
            ls = new Vector3(4, 1, .13f);
            le = new Vector3(0, 1, .13f);
            lDest = ls;
            
        } // end of Awake()

        // ---------------------------------------------------------
        private void Update() {

            controlPolyLine.positionCount = 4;
            Matrix4x4 splineMatrix = SplineParameters.GetMatrix(splineType);

            //rotate camera

            //center view and orbit
            camera.transform.LookAt(center);
            rotationCam += .5f * Time.deltaTime;
            camera.transform.position = cam + (new Vector3(Mathf.Sin(rotationCam), 0, Mathf.Cos(rotationCam)) * 6f);


            //animate
            
            //bounce between tow points
            control0.position = Vector3.MoveTowards(control0.position, cp0, Time.fixedDeltaTime * .1f);
            if(control0.position == cp0){
                cp0 = new Vector3(cp0.x * -1, cp0.y, cp0.z); 
            }

            //rotate around a centeral axis with a defined radius
            rotationCp1 += 1.5f * Time.deltaTime;
            control1.position = cp1 + (new Vector3(Mathf.Sin(rotationCp1), Mathf.Cos(rotationCp1), 0) * 1f);
            
            control2.position = Vector3.MoveTowards(control2.position, cp2, Time.fixedDeltaTime * .3f);
            if(control2.position == cp2){
                cp2 = new Vector3(cp2.x, cp2.y * -1, cp2.z); 
            }

            rotationCp3 += 1f * Time.deltaTime;
            control3.position = cp3 + (new Vector3(Mathf.Sin(rotationCp3), Mathf.Cos(rotationCp3), 0) * .5f);

            //bounce light source between two points
            light.transform.position = Vector3.MoveTowards(light.transform.position, lDest, Time.fixedDeltaTime * .1f);
            if(light.transform.position == le){
                lDest = ls;
            }
            else if(light.transform.position == ls){
                lDest = le;
            }
            
            // pass all necessary variables to the Vertex Shader:
            //
            // spline matrix in Hermite form:
            material.SetMatrix("_SplineMatrix", splineMatrix);
            // control points for spline curve rendering:
            material.SetVector("_Control0", control0.position);
            material.SetVector("_Control1", control1.position);
            material.SetVector("_Control2", control2.position);
            material.SetVector("_Control3", control3.position);
            // step between subsequent t parameter values for curve:
            float step = (float)1.0 / (float)(verticesOnCurve - 1);
            material.SetFloat("_Step", step);
            
            material.SetColor("_Color", splineColor);

            //pass light vars; camera pos, light color, light pos
            material.SetColor("_Light_Color", light.GetComponent<SpriteRenderer>().color);
            material.SetVector("_Light", light.transform.position);
            material.SetVector("_Cam", camera.transform.position);

            //determine if vertex or fragment shader is computing
            if(renderer == RendererType.vertex){
                material.SetFloat("_VorF", 1);
            }
            else{
                material.SetFloat("_VorF", 0);
            }

            // to draw the enclosing polyLine, set control line points:
            //

            controlPolyLine.SetPosition(0, control0.position);
            controlPolyLine.SetPosition(1, control1.position);
            controlPolyLine.SetPosition(2, control2.position);
            controlPolyLine.SetPosition(3, control3.position);

        } // end of Update()

    } // end of SplineSegmentGPUCompute

} // end of A04