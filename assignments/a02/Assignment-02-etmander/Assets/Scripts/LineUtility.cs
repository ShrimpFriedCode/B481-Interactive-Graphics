/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    This script provides a library of "utility" methods,
    that may be useful to solve Assignment 01.
    
    However, you may have to complete the parts marked as TODO ...
    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */

using UnityEngine;
using System; //Math
using System.Collections;
using System.Collections.Generic;

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

        //cyrus-beck() -- returns the two points of a clipped line from a hexagon
        //
        public static Vector2[] CyrusBeck(Transform subLine0, Transform subLine1, Transform[] polygonVertices) {
            //init
            Vector2 lS = Vector2.zero;
            Vector2 lE = Vector2.zero;
            Vector2[] ret = new Vector2[] {lS, lE};

            //calculations

            //array of each normal edge
            Vector2[] normals = new Vector2[6];
            for(int i = 0; i < 6; i++){
                //get all normals facing away from center
                //n = (-(x2.y - x1.y), (x2.x - x1.x))
                normals[i] = new Vector2( -(polygonVertices[((i + 1) % 6)].position.y - polygonVertices[i].position.y),(polygonVertices[((i + 1) % 6)].position.x - polygonVertices[i].position.x));
            }

            //(xi - x1), where xi is a point along each respective edge of hexagon, and x1 is start of line to be clipped
            Vector2[] edgeMinSubL = new Vector2[6];
            for(int i = 0; i < 6; i++){
                edgeMinSubL[i] = new Vector2((polygonVertices[i].position.x - subLine0.position.x),(polygonVertices[i].position.y - subLine0.position.y));
            }

            //calculate t values

            //lists for tS and tE, considering we don't know sizes
            List<float> tS = new List<float>();
            List<float> tE = new List<float>();
            for(int i = 0; i < 6; i++){

                //t = [n ⋅ (xi - x1)] / [n ⋅ (x2 - x1)]
                float t = ((float)(Vector2.Dot(normals[i], edgeMinSubL[i])))/((float)(Vector2.Dot(normals[i], (subLine1.position - subLine0.position))));

                if(Vector2.Dot(normals[i], (subLine1.position - subLine0.position)) > 0){
                    //normals greater than 0 -- ni ⋅ (x2 - x1) > 0
                    tS.Add(t);
                } else {
                    //less than 0 -- nj ⋅ (x2 - x1) < 0
                    tE.Add(t);
                }
            }

            //add 0, 1 to Tstart and Tend, then sort so we can find max/min
            tS.Add((float)0);
            tE.Add((float)1);
            tS.Sort();
            tE.Sort();

            //tS = max, tE = min
            float Tstart = tS[tS.Count-1];
            float Tend = tE[0];

            //if tend < tstart, line outside window
            if(Tend < Tstart){
                //return zero vectors
                return ret;
            }

            //if 0 <= tstart <= tend <= 1, draw
            //use parametric equation of line to interpolate point of clipping along our line to be clipped
            ret[0] = new Vector2(((float)subLine0.position.x + (float)(subLine1.position.x - subLine0.position.x) * (float)Tstart),((float)subLine0.position.y + (float)(subLine1.position.y - subLine0.position.y) * (float)Tstart));
            ret[1] = new Vector2(((float)subLine0.position.x + (float)(subLine1.position.x - subLine0.position.x) * (float)Tend),((float)subLine0.position.y + (float)(subLine1.position.y - subLine0.position.y) * (float)Tend));

            return ret;
        }
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