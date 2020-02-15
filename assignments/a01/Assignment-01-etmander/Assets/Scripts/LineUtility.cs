/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    This script provides a library of "utility" methods,
    that may be useful to solve Assignment 01.
    
    However, you may have to complete the parts marked as TODO ...
    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */

using UnityEngine;
using System; //Math

namespace A01 {

    public static class LineUtility {
    
        // DirectionNormal() --- returns the normal to a given direction vector:
        public static Vector2 DirectionNormal(Vector2 direction) {
              Vector2 normal = direction.normalized;
              return normal;
        } // end of DirectionNormal() 

        // LineSegmentNormal() --- returns the normal to a line segment:
        public static Vector2 LineSegmentNormal(Vector2 start, Vector2 end) {
              Vector2 direction =  new Vector2((end.x - start.x),(end.y - start.y));
              Vector2 normal = DirectionNormal(direction);
              return normal;
        } // end of LineSegmentNormal()

        // ClosestPointOnLine() --- returns the closest point on a line to a given query point:
        public static Vector2 ClosestPointOnLine(Vector2 pointOnLine, Vector2 direction, Vector2 point) {
              Vector2 localX = point-pointOnLine;
              // localX == x1 (new origin)
              //P = localX + (dot(normalized v,localx))v
              return pointOnLine + Vector2.Dot(direction, localX) * direction;
              
            
            //  you may find useful the 2D Point-Line Geometry expressions shown at Lecture 07:
            //  https://www.cs.indiana.edu/classes/b481/2020/lectures/lecture-07A-b481-2020-affine-transformations-2D.html
            
            
        } // end of ClosestPointOnLine()

        // ClosestPointOnSegment() --- returns the closest point (on a line segment)
        //                             to a given subject point:
        public static Vector2 ClosestPointOnSegment(Vector2 start, Vector2 end, Vector2 point) {

            // get a rounded (2 sig-fig) decimal of the length of the segment
            decimal length = Decimal.Round((decimal)Vector2.Distance(start, end), 2); 

            //get calculated point on line (be sure to normalize distance vector)
            Vector2 pol = ClosestPointOnLine(start, LineSegmentNormal(start, end), point);

            //point on line's distance to seg start
            decimal sx = (decimal)Vector2.Distance(start, pol);
            //point on line's distance to seg end
            decimal ex = (decimal)Vector2.Distance(pol, end);
            //point on line's distance to start + distance to end
            decimal comb = Decimal.Round((decimal)sx+ex, 2);

            //Debug prints:

            //Debug.Log("sx: " + sx);
            //Debug.Log("ex: " + ex);
            //Debug.Log("comb: " + comb);
            //Debug.Log("length: " + length);

              //if our point is on the line segment, the distance to start and end should match
              //    the length of the line segment. Otherwise, it's off.
              if (comb != length) {
                  if(sx < ex){
                        //we're closer to seg start, so that's our snap point
                        return start;
                  }
                  else{
                        //we're closer to seg end, and that's where we snap
                        return end;
                  }
              } else { //else we're on the line, so calculated point is valid
                  return pol;
              }
        } // end of ClosestPointOnSegment()

        // ClosestPointOnPolygon() --- returns the closest point (on a polygon)
        //                             to a given subject point.
        //  Note:
        //      the polygon is given as array of transforms
        //      with vertex[n-1] connecting back to vertex[0]
        //
        public static Vector2 ClosestPointOnPolygon(Transform[] polygonVertices, Vector2 point) {
        
            Vector2 result = Vector2.zero;
            float minSqrDistance = float.PositiveInfinity;
            
            for (int i = 0; i < polygonVertices.Length; i++) {
                int j = (i + 1) % polygonVertices.Length;
                Vector2 side = polygonVertices[j].position - polygonVertices[i].position;
                float sideLength = side.magnitude;
                Vector2 sideDirection = side / sideLength;

            //get a point on the current polygon line segment
            Vector2 pol = ClosestPointOnLine(polygonVertices[i].position, LineSegmentNormal(polygonVertices[i].position, polygonVertices[j].position), point);

            //similar to the single line segment, we must determine if our calculated point is on the segment,
            // if not, it needs to snap to a vertex.

            //point distance to segment start
            decimal sx = (decimal)Vector2.Distance(polygonVertices[i].position, pol);
            //point distance to segment end
            decimal ex = (decimal)Vector2.Distance(pol, polygonVertices[j].position);
            //distance between seg start -> point -> seg end
            decimal comb = Decimal.Round((decimal)sx+ex, 2);

            //init
            Vector2 pointOnPolygon;

            //if distance between seg start -> point -> seg end == segLength, we're on the line. otherwise, snap to vertex
            if (comb != Decimal.Round((decimal)sideLength, 2)) {
                  if(sx < ex){
                        //we're closer to seg start
                        pointOnPolygon = polygonVertices[i].position;
                  }
                  else{
                        //we're closer to seg end
                        pointOnPolygon = polygonVertices[j].position;
                  }
              } else {
                  pointOnPolygon = pol;
              }

            //  the following code works, as long as you computed pointOnPolygon correctly.
            //  It will be useful to understand what the following lines do:
                Vector2 delta = point - pointOnPolygon;
                float sqrDistance = delta.sqrMagnitude;

                if (sqrDistance < minSqrDistance) {
                    result = pointOnPolygon;
                    minSqrDistance = sqrDistance;
                }
            }
            return result;
        } // end of ClosestPointOnPolygon()

    } // end of static class LineUtility

} // end of namespace A01