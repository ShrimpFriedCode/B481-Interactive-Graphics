/*CSCI-B481 / Spring 2020 / Mitja Hmeljak

    Camera orbit over set "center"

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    //view center
    [SerializeField] private Transform center;

    //rotation angle for circular animation
    private float rotationCam;
    private Vector3 cam;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(center.position.x, 0, center.position.z);
        cam = center.position;
    }

    // Update is called once per frame
    void Update()
    {
        //center view and orbit
        transform.LookAt(center);
        rotationCam += .3f * Time.deltaTime;
        transform.position = cam + (new Vector3(Mathf.Sin(rotationCam), 1, Mathf.Cos(rotationCam)) * 2f);
    }
}
