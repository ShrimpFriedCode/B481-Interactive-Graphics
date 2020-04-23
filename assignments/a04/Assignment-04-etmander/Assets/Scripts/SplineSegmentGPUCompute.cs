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

        // specify the name of the Vertex Shader to be used:
        private const string shaderName = "SplineVertexShader";
        
        // control points for a single Spline Curve segment:
        [SerializeField] private Transform control0, control1, control2, control3;
        // choice of Spline Curve type:
        [SerializeField] private SplineParameters.SplineType splineType;
        // only one line renderer: the control polyline:
        [SerializeField] private LineRenderer controlPolyLine;
        
        // what color should the Spline Curve be?
        [SerializeField] private Color splineColor = new Color(255f / 255f, 255f / 255f, 0f / 255f);

        // how wide should the Spline Curve be?
        [SerializeField] private float splineWidth = 0.1f;

        // how many vertices on the spline curve?
        //   (the more vertices you set, the smoother the curve will be)
        [Range(8, 512)] [SerializeField] private int verticesOnCurve = 64;


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
            
        } // end of Awake()

        // ---------------------------------------------------------
        private void Update() {
            controlPolyLine.positionCount = 4;
            Matrix4x4 splineMatrix = SplineParameters.GetMatrix(splineType);
            
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


            // to draw the enclosing polyLine, set control line points:
            //
            controlPolyLine.SetPosition(0, control0.position);
            controlPolyLine.SetPosition(1, control1.position);
            controlPolyLine.SetPosition(2, control2.position);
            controlPolyLine.SetPosition(3, control3.position);

        } // end of Update()

    } // end of SplineSegmentGPUCompute

} // end of A04