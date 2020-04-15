/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    Utility Methods for A03
    Ethan Anderson (etmander)
 */

using UnityEngine;
using System; //Math
using System.Collections;
using System.Collections.Generic;

namespace A03 {

    public static class LineUtility {

        //calculate points for Bezier Spline
        public static void Bezier(Transform v1, Transform v2, Transform v3, Transform v4, LineRenderer spline, int resolution){
            spline.positionCount = resolution;//resolution of spline curve
            float time = 0f; //t value for interpolation

            var u = new Vector4(Mathf.Pow(time, 3), Mathf.Pow(time, 2), Mathf.Pow(time, 1), 1); //initilization of u vector

            //bezier matrix
            var Mbez = Matrix4x4.zero; //init
            Mbez.SetColumn(0, new Vector4(-1, 3, -3, 1));
            Mbez.SetColumn(1, new Vector4(3, -6, 3, 0));
            Mbez.SetColumn(2, new Vector4(-3, 3, 0, 0));
            Mbez.SetColumn(3, new Vector4(1, 0, 0, 0));

            //4x4 matrix containing z,y,z,w of control points
            var positionM = Matrix4x4.zero; //init
            var p1 = v1.position; //for readability, point1 = position of control point 1 
            var p2 = v2.position;
            var p3 = v3.position;
            var p4 = v4.position;
            positionM.SetColumn(0, new Vector4(p1.x, p1.y, p1.z, 1));
            positionM.SetColumn(1, new Vector4(p2.x, p2.y, p2.z, 1));
            positionM.SetColumn(2, new Vector4(p3.x, p3.y, p3.z, 1));
            positionM.SetColumn(3, new Vector4(p4.x, p4.y, p4.z, 1));

            //iterate through resolution of curve
            for (int i = 0; i < resolution; i++){
                //calculate u vector using new time
                u = new Vector4(Mathf.Pow(time, 3), Mathf.Pow(time, 2), Mathf.Pow(time, 1), 1);
                //position vector from calculation
                var pu = positionM * Mbez * u;
                //set position of current spline segment
                spline.SetPosition(i, new Vector3(pu.x, pu.y, pu.z));

                //increase time incrementally approaching 1
                time += (float)1 / (float)resolution;
            }
            
        }

        //calculate points for Catmull-rom spline. Exactly the same as Bezier equation, different spline matrix
        public static void Catmull(Transform v1, Transform v2, Transform v3, Transform v4, LineRenderer spline, int resolution){
            spline.positionCount = resolution;
            float time = 0f;

            var u = new Vector4(Mathf.Pow(time, 3), Mathf.Pow(time, 2), Mathf.Pow(time, 1), 1);

            //Catmull-rom spline matrix
            var Mcr = Matrix4x4.zero;
            Mcr.SetColumn(0, new Vector4(-1/2f, 3/2f, -3/2f, 1/2f));
            Mcr.SetColumn(1, new Vector4(1, -5/2f, 2, -1/2f));
            Mcr.SetColumn(2, new Vector4(-1/2f, 0, 1/2f, 0));
            Mcr.SetColumn(3, new Vector4(0, 1, 0, 0));

            var positionM = Matrix4x4.zero;
            var p1 = v1.position;
            var p2 = v2.position;
            var p3 = v3.position;
            var p4 = v4.position;
            positionM.SetColumn(0, new Vector4(p1.x, p1.y, p1.z, 1));
            positionM.SetColumn(1, new Vector4(p2.x, p2.y, p2.z, 1));
            positionM.SetColumn(2, new Vector4(p3.x, p3.y, p3.z, 1));
            positionM.SetColumn(3, new Vector4(p4.x, p4.y, p4.z, 1));

            for (int i = 0; i < resolution; i++){

                u = new Vector4(Mathf.Pow(time, 3), Mathf.Pow(time, 2), Mathf.Pow(time, 1), 1);

                var pu = positionM * Mcr * u;
                
                spline.SetPosition(i, new Vector3(pu.x, pu.y, pu.z));
                
                time += 1 / (float)resolution;
            }
            
            
        }

        //calculate points for B-spline. Exactly the same as Bezier equation, different spline matrix
        public static void Bspline(Transform v1, Transform v2, Transform v3, Transform v4, LineRenderer spline, int resolution){
            spline.positionCount = resolution;
            float time = 0f;

            var u = new Vector4(Mathf.Pow(time, 3), Mathf.Pow(time, 2), Mathf.Pow(time, 1), 1);

            //B-spline matrix
            var Mb = Matrix4x4.zero;
            Mb.SetColumn(0, new Vector4(-1/6f, 3/6f, -3/6f, 1/6f));
            Mb.SetColumn(1, new Vector4(3/6f, -1, 3/6f, 0));
            Mb.SetColumn(2, new Vector4(-3/6f, 0, 3/6f, 0));
            Mb.SetColumn(3, new Vector4(1/6f, 4/6f, 1/6f, 0));

            var positionM = Matrix4x4.zero;
            var p1 = v1.position;
            var p2 = v2.position;
            var p3 = v3.position;
            var p4 = v4.position;
            positionM.SetColumn(0, new Vector4(p1.x, p1.y, p1.z, 1));
            positionM.SetColumn(1, new Vector4(p2.x, p2.y, p2.z, 1));
            positionM.SetColumn(2, new Vector4(p3.x, p3.y, p3.z, 1));
            positionM.SetColumn(3, new Vector4(p4.x, p4.y, p4.z, 1));

            for (int i = 0; i < resolution; i++){
                u = new Vector4(Mathf.Pow(time, 3), Mathf.Pow(time, 2), Mathf.Pow(time, 1), 1);

                var pu = positionM * Mb * u;
               
                spline.SetPosition(i, new Vector3(pu.x, pu.y, pu.z));
                
                time += 1 / (float)resolution;
            }
            
        }

    } // end of static class LineUtility

} // end of namespace A03