using System.Collections.Generic;
using Utility;

namespace Core
{
    public class NavMesh
    {
        public Dictionary<Vertex, List<HalfEdge>> vertices ;
        public Dictionary<Face, List<HalfEdge>> faces;


        public NavMesh(Dictionary<Vertex, List<HalfEdge>> vertices, Dictionary<Face, List<HalfEdge>> faces)
        {
            this.vertices = vertices;
            this.faces = faces;
        }

        public Face getFaceFromPoint(Point p)
        {
            foreach (var keyValuePair in faces)
            {
                if (pointInside(keyValuePair.Key, p))
                {
                    return keyValuePair.Key;
                }
            }

            return null;
        }

        public bool pointInside(Face f,Point p)
        {
            foreach (var halfEdge in faces[f])
            {
                if (halfEdge.incidentFace == f)
                {
                    if (GeometryUtility.toTheLeftOf(halfEdge.v, halfEdge.next.v, p) <0)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
    }
}