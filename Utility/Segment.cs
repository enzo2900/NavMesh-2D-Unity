using UnityEngine;
using Utility;

public class Segment<T> where T : MapPoint
{
    public  T start;

    public  T end;

    public Segment(T p, T q) {
        this.start = p;
        this.end = q;
    }

}
