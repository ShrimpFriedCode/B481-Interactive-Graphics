/*CSCI-B481 / Spring 2020 / Mitja Hmeljak

    Scene control script for final.
    sets scene wide lighting variables to be called from per object shader compute scripts.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF {

public class ControlScript : MonoBehaviour
{

    //enum states
    public enum LightState { on, off }
    public enum CameraView { Doom, Orbit }
    public enum RendererType { vertex, fragment }

    //references to in scene objects
    [Header("Scene Objects")]
    [SerializeField] private Transform object1;
    [SerializeField] private Transform object2;
    //view center
    [SerializeField] private Transform center;

    [Header("Scene Rendereing")]
    //renderer
    [SerializeField] public RendererType renderType;

    //properties for light 1
    [Header("White Light")]
    [SerializeField] private GameObject light1;
    [SerializeField] private LightState Power1;
    [SerializeField] public float lightIntensity;
    //properties for light 2
    [Header("Red Light")]
    [SerializeField] private GameObject light2;
    [SerializeField] private LightState Power2;
    [SerializeField] public float RedLightIntensity;
    //camera control settings
    [Header("Camera Control")]
    [SerializeField] private GameObject cameraOne;
    [SerializeField] private GameObject cameraTwo;
    [SerializeField] private CameraView ViewMode;
    //active camera for reference from GPUcompute
    [System.NonSerialized] public GameObject activeCamera;
    //animation vars
    private Vector3 rl, saw1, saw2, dest;
    private float rotationL;

    // Start is called before the first frame update
    void Awake()
    {
        //set active camera by default
        activeCamera = cameraOne;
        cameraOne.GetComponent<Camera>().enabled = true;
        cameraTwo.GetComponent<Camera>().enabled = false;

        //init animation positions
        rl = center.transform.position;
        saw1 = new Vector3(7.5f, 3.3f, -3.1f);
        saw2 = new Vector3(-1f, 3.3f, -3.1f);
        dest = saw1;
    }

    // Update is called once per frame
    void Update()
    {
        //set appropriate view/camera mode
        if(ViewMode == CameraView.Doom){
            activeCamera = cameraOne;
            cameraOne.GetComponent<Camera>().enabled = true;
            cameraTwo.GetComponent<Camera>().enabled = false;
        }
        else{
            activeCamera = cameraTwo;
            cameraOne.GetComponent<Camera>().enabled = false;
            cameraTwo.GetComponent<Camera>().enabled = true;
        }

        //set light state
        if(Power1 == LightState.off){
            light1.GetComponent<SpriteRenderer>().color = Color.black;
        }
        else{
            light1.GetComponent<SpriteRenderer>().color = Color.white;
        }

        if(Power2 == LightState.off){
            light2.GetComponent<SpriteRenderer>().color = Color.black;
        }
        else{
            light2.GetComponent<SpriteRenderer>().color = Color.red;
        }


        object1.Rotate (0,10*Time.deltaTime,0); //rotate center object

        //rotate light 2 about a point
        rotationL += .5f * Time.deltaTime;
        light2.transform.position = rl + (new Vector3(Mathf.Sin(rotationL), 0, Mathf.Cos(rotationL)) * 2f);

        //move light 1 in sawing motion
        light1.transform.position = Vector3.MoveTowards(light1.transform.position, dest, Time.fixedDeltaTime * .5f);
        if(light1.transform.position == saw1){
            dest = saw2;
        }
        else if(light1.transform.position == saw2){
            dest = saw1;
        }

    }
}
}