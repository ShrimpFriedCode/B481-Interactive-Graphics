/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak

    Per object handler for shader, passes object specific properties to shader

 */

using UnityEngine;

namespace AF {

    public class GPUCompute : MonoBehaviour {

        public enum RendererType { vertex, fragment }

        // specify the name of the Vertex Shader to be used:
        private const string shaderName = "ShaderFinal";

        //light positions
        [SerializeField] private GameObject light;
        [SerializeField] private GameObject RedLight;
        //camera pos
        [SerializeField] private GameObject camera;

        //light intensity and obj color
        [Header("Object Properties")]
        [SerializeField] private Color color;
        [Range(0f, 1f)] [SerializeField] private float AmbientIntensity, DiffuseIntensity, SpecularIntensity;


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

            meshRenderer.sortingLayerName = "Default";
            meshRenderer.sortingOrder = 1;
            
        } // end of Awake()

        // ---------------------------------------------------------
        private void Update() {

            //object properties
            material.SetColor("_Color", color); //color of obj
            material.SetColor("_Light_Color", light.GetComponent<SpriteRenderer>().color); //light color
            material.SetColor("_R_Light_Color", RedLight.GetComponent<SpriteRenderer>().color); //second light color
            material.SetVector("_Light", light.transform.position); //light position
            material.SetVector("_RLight", RedLight.transform.position); //second light position
            material.SetVector("_Cam", GameObject.Find("MainScene").GetComponent<ControlScript>().activeCamera.transform.position); //active camera

            //object lighing weights (component intensity)
            material.SetFloat("_aI", AmbientIntensity);
            material.SetFloat("_dI", DiffuseIntensity);
            material.SetFloat("_sI", SpecularIntensity);

            //light intensity
            material.SetFloat("_lI", GameObject.Find("MainScene").GetComponent<ControlScript>().lightIntensity);
            material.SetFloat("_rI", GameObject.Find("MainScene").GetComponent<ControlScript>().RedLightIntensity);

            //determine if vertex or fragment shader is computing
            if(GameObject.Find("MainScene").GetComponent<ControlScript>().renderType == ControlScript.RendererType.vertex){
                material.SetFloat("_VorF", 1);
            }
            else{
                material.SetFloat("_VorF", 0);
            }

        } // end of Update()

    } // end of SplineSegmentGPUCompute

} // end of AF