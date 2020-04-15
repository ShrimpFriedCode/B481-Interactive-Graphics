/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    A03 main
    Ethan Anderson (etmander)
 */

using UnityEngine;

namespace A03 {

    public class ManySegmentPositionLines : MonoBehaviour {

        // fields to connect to Unity objects:
        [SerializeField] private Transform v1, v2, v3, v4;

        [SerializeField] private LineRenderer subLine1, subLine2, subLine3, subLine4, splineLine;

        public int splineType = 0; //default bezier spline

        private int resolution = 100; //line position count

        // Update() is called once per frame:
        private void Update() {

            // set positions for control points:
            subLine1.SetPosition(0, v1.position);
            subLine1.SetPosition(1, v2.position);

            subLine2.SetPosition(0, v2.position);
            subLine2.SetPosition(1, v3.position);

            subLine3.SetPosition(0, v3.position);
            subLine3.SetPosition(1, v4.position);

            //Spline Calulation

            //draw the currently selected spline type
            if(splineType == 0){
                LineUtility.Bezier(v1, v2, v3, v4, splineLine, resolution);
            }
            else if(splineType == 1){
                LineUtility.Catmull(v1, v2, v3, v4, splineLine, resolution);
            }
            else if(splineType == 2){
                LineUtility.Bspline(v1, v2, v3, v4, splineLine, resolution);
            }
            
        } // end of Update()

        //event to run onClick of UI button
        public void ChangeSplineType(int type){
            splineType = type;
        }
        
    } // end of class SingleSegmentPositionLines

} // end of namespace A03
