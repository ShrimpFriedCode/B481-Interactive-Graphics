/*CSCI-B481 / Spring 2020 / Mitja Hmeljak

    Camera "doom" style control

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDoomControl : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    //forward
    if(Input.GetKey(KeyCode.W)){
        //get appropriate vector for forward motion
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert =  Input.GetAxisRaw("Vertical");
        Vector3 dir = (horiz * transform.right + vert * transform.forward).normalized;
        transform.position += dir * 10 * Time.deltaTime;
     }
    if(Input.GetKey(KeyCode.S)){ //backwards
        //get appropriate vector for backwards motion
        float horiz = Input.GetAxisRaw("Horizontal");
        float vert =  Input.GetAxisRaw("Vertical");
        Vector3 dir = (horiz * transform.right + vert * transform.forward).normalized;
        transform.position -= -dir * 10 * Time.deltaTime;
     }
    if(Input.GetKey(KeyCode.A)){ //roatate view left/right
         transform.Rotate (0,-(70*Time.deltaTime),0);
     }
    if(Input.GetKey(KeyCode.D)){
         transform.Rotate (0,(70*Time.deltaTime),0);
     }
        
    }
}
