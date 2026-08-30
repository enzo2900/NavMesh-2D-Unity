using UnityEngine;

public class HalfEdge
{
    public Vertex v;

    public HalfEdge twin;

    public HalfEdge next;
    public HalfEdge prev;
    public Face incidentFace;

    public int tag;


    public float magnitude() {
        float x = twin.v.x - v.x;
        float y = twin.v.y - v.y;
        
        return Mathf.Sqrt(x * x  + y * y);
    }
    public float[] normalized() {
        
        float magnitude = this.magnitude();
        float x = twin.v.x - v.x;
        float y = twin.v.y - v.y;
        return new float[] {x / magnitude,y / magnitude};
    }

    public override bool Equals(object obj)
    {
        if(obj is HalfEdge edge) {
            return v.Equals(edge.v) && next.v.Equals(edge.twin.v);
        }
        return false;
    }
    
}
