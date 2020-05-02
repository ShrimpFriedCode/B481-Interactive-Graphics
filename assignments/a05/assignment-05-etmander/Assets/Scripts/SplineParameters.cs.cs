/*  CSCI-B481 / Spring 2020 / Mitja Hmeljak
    This script should:
    provide the correct parameters in the spline matrices,
    as from the Hermite Form.
    
    Defined as in textbook Chapter 11.5 in the 7th edition Textbook 
                         / Chapter 10.5 in the 6th edition Textbook.
                         
    However, keep in mind that Unity Matrix4x4 are "column major",
    as detailed in assignment instructions.
    Original demo code by CSCI-B481 alumnus Rajin Shankar, IU Computer Science.
 */

using UnityEngine;


namespace A04 {

    public static class SplineParameters    {
    
        public enum SplineType { Bezier, CatmullRom, Bspline }

        public static Matrix4x4 GetMatrix(SplineType type) {
        
            switch (type) {
                // TODO: generate Bezier spline matrix,
                //   with constants as per Textbook Chapter 11.6.1 :
                case SplineType.Bezier:
                    return new Matrix4x4( // COLUMN MAJOR!
                        new Vector4(-1, 3, -3, 1), // TODO
                        new Vector4(3, -6, 3, 0), // TODO
                        new Vector4(-3, 3, 0, 0), // TODO
                        new Vector4(1, 0, 0, 0) // TODO
                    );
                // TODO: generate Catmull-Rom spline matrix,
                //   with constants as per Textbook Chapter 11.8.5 :
                case SplineType.CatmullRom:
                    return new Matrix4x4( // COLUMN MAJOR!
                        new Vector4(-1/2f, 3/2f, -3/2f, 1/2f), // TODO
                        new Vector4(1, -5/2f, 2, -1/2f), // TODO
                        new Vector4(-1/2f, 0, 1/2f, 0), // TODO
                        new Vector4(0, 1, 0, 0) // TODO
                    );
                // TODO: generate B-spline matrix,
                //   with constants as per Textbook Chapter 11.7.1 :
                case SplineType.Bspline:
                    return new Matrix4x4( // COLUMN MAJOR!
                        new Vector4(-1/6f, 3/6f, -3/6f, 1/6f), // TODO
                        new Vector4(3/6f, -1, 3/6f, 0), // TODO
                        new Vector4(-3/6f, 0, 3/6f, 0), // TODO
                        new Vector4(1/6f, 4/6f, 1/6f, 0) // TODO
                    );
                default:
                    return Matrix4x4.identity;
            }
        } // end of GetMatrix()

        // this could be useful for multi-segment spline curves:
        public static bool UsesConnectedEndpoints(SplineType type) {
            switch (type) {
                case SplineType.Bezier: return true;
                case SplineType.CatmullRom: return false;
                case SplineType.Bspline: return false;
                default: return false;
            }
        } // end of UsesConnectedEndpoints()
        
    } // end of class SplineParameters
    
} // end of namespace A04
