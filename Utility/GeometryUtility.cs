using UnityEngine;
using Utility;

public class GeometryUtility
{
    public static float toTheLeftOf(MapPoint A, MapPoint B, MapPoint C) {
        //double[] abN = normalized(A,B);
        //double[] acN = normalized(A,C);
        //return (abN[0] * acN[1] - abN[1] * acN[0]);
        return (B.getX() - A.getX()) * (C.getY() - A.getY()) - (B.getY() - A.getY()) * (C.getX() - A.getX());
    }

    public static float dotProduct(MapPoint A, MapPoint B, MapPoint C) {
        return (B.getX() -A.getX()) * (C.getX() - A.getX()) + (B.getY() - A.getY()) * (C.getY() - A.getY());
    }

    public static float magnitude(MapPoint A, MapPoint B) {

        float x = B.getX() - A.getX();
        float y = B.getY() - A.getY();
        return Mathf.Sqrt(x * x  + y * y);
    }
    public static float[] normalized(MapPoint A, MapPoint B)
    {
        float magnitude = GeometryUtility.magnitude(A,B);
        float x = B.getX() - A.getX();
        float y = B.getY() - A.getY();
        return new float[] {x / magnitude,y / magnitude};
    }


    public static bool isInsideCircleD(MapPoint A, MapPoint B, MapPoint C , MapPoint D) {
        double orientation = (B.getX() - A.getX()) * (C.getY() - A.getY()) -
                             (B.getY() - A.getY()) * (C.getX()- A.getX());
        double ax = A.getX() - D.getX();
        double ay = A.getY() - D.getY();
        double bx = B.getX() - D.getX();
        double by = B.getY() - D.getY();
        double cx = C.getX() - D.getX();
        double cy = C.getY() - D.getY();

        double det = (ax*ax + ay*ay)*(bx*cy - cx*by)
                     - (bx*bx + by*by)*(ax*cy - cx*ay)
                     + (cx*cx + cy*cy)*(ax*by - bx*ay);
        // double det = inCircle( A,  B, C, D);
        if (orientation < 0) det = -det;

        return det >= 0;
    }

}
