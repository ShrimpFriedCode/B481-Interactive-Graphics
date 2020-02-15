/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    This script will position the start and end points for two line renderers.
    Positioning is done by using GameObject Transforms.
    Used to show closest point on line segment.
    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */

using UnityEngine;

namespace A01 {

    public class ManySegmentPositionLines : MonoBehaviour {

        // fields to connect to Unity objects:
        [SerializeField] private Transform v1, v2, v3, v4, v5, subjectPointTransform;

        [SerializeField] private LineRenderer subLine1, subLine2, subLine3, subLine4, subLine5, connectingLineRenderer;

        // Update() is called once per frame:
        private void Update() {

            // set positions for subject line vertices:
            subLine1.SetPosition(0, v1.position);
            subLine1.SetPosition(1, v2.position);

            subLine2.SetPosition(0, v2.position);
            subLine2.SetPosition(1, v3.position);

            subLine3.SetPosition(0, v3.position);
            subLine3.SetPosition(1, v4.position);

            subLine4.SetPosition(0, v4.position);
            subLine4.SetPosition(1, v5.position);

            subLine5.SetPosition(0, v5.position);
            subLine5.SetPosition(1, v1.position);

            // if debug is necessary, uncomment these lines:
            // Debug.Log("subjectLineStartTransform.position = " + subjectLineStartTransform.position);
             //Debug.Log("subjectLineEndTransform.position = " + subjectLineEndTransform.position);
             //Debug.Log("subjectLineRenderer.GetPosition(0) = " + subjectLineRenderer.GetPosition(0));
            // Debug.Log("subjectLineRenderer.GetPosition(1) = " + subjectLineRenderer.GetPosition(1));

            // set positions for connecting line vertices:
            Transform[] arr = new Transform[] {v1, v2, v3, v4, v5};
            Vector2 lClosestPoint = LineUtility.ClosestPointOnPolygon(arr, subjectPointTransform.position);

            // Vector2 lClosestPoint = Vector2.one; // temporarily!
            connectingLineRenderer.SetPosition(0, subjectPointTransform.position);
            connectingLineRenderer.SetPosition(1, lClosestPoint);
            
        } // end of Update()
        
    } // end of class SingleSegmentPositionLines

} // end of namespace A01
