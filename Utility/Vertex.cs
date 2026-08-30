using System;
using UnityEngine;
using Utility;

public class Vertex : MapPoint
{
    public float x,y;

    public HalfEdge incidentEdge;
    public Vertex(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public Vertex copy() {
        return new Vertex(x,y);
    }

    public override int GetHashCode()
    {
        
        return HashCode.Combine(x, y);;
    }

    public override bool Equals(object obj)
    {
        if(obj is Vertex v) {
            return v.x == x && v.y == y;
        }
        return false;
    }

    public override string ToString()
    {
        return "Vertex " +
               +x + "," +
               +y;
    }

    public float getX()
    {
        return x;
    }

    public float getY()
    {
        return y;
    }
}
