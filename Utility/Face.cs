using System.Collections.Generic;
using UnityEngine;

public class Face 
{
    public HalfEdge outerComponent;

    public List<HalfEdge> innerComponents;

    public static short idFace;
    public short id;

    public Face() {
        innerComponents = new List<HalfEdge>();
        idFace++;
        id = idFace;
    }

    public override string ToString()
    {
        return "Face " + id;
    }

    public override bool Equals(object obj)
    {
        if(obj is Face f) {
            if (f.outerComponent == null)
            {
                return false;
            }
            return f.outerComponent.Equals(outerComponent);
        }
        return false;
    }
}
