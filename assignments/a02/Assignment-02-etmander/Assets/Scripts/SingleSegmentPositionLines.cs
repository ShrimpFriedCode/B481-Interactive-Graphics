/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    This script will position the start and end points for two line renderers.
    Positioning is done by using GameObject Transforms.
    Used to show closest point on line segment.
    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */

using UnityEngine;

namespace A01 {

    public class SingleSegmentPositionLines : MonoBehaviour {

        // fields to connect to Unity objects:
        [SerializeField] private Transform subjectLineStartTransform, subjectLineEndTransform, subjectPointTransform;
        [SerializeField] private LineRenderer subjectLineRenderer, connectingLineRenderer;

        // Update() is called once per frame:
        private void Update() {

            // set positions for subject line vertices:
            subjectLineRenderer.SetPosition(0, subjectLineStartTransform.position);
            subjectLineRenderer.SetPosition(1, subjectLineEndTransform.position);

            // if debug is necessary, uncomment these lines:
            // Debug.Log("subjectLineStartTransform.position = " + subjectLineStartTransform.position);
             //Debug.Log("subjectLineEndTransform.position = " + subjectLineEndTransform.position);
             //Debug.Log("subjectLineRenderer.GetPosition(0) = " + subjectLineRenderer.GetPosition(0));
            // Debug.Log("subjectLineRenderer.GetPosition(1) = " + subjectLineRenderer.GetPosition(1));

            // set positions for connecting line vertices:
            
            Vector2 lClosestPoint = LineUtility.ClosestPointOnSegment(
                subjectLineStartTransform.position,
                subjectLineEndTransform.position,
                subjectPointTransform.position);

            // Vector2 lClosestPoint = Vector2.one; // temporarily!
            connectingLineRenderer.SetPosition(0, subjectPointTransform.position);
            connectingLineRenderer.SetPosition(1, lClosestPoint);
            
        } // end of Update()

    } // end of class SingleSegmentPositionLines

} // end of namespace A01
