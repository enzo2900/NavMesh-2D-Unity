using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Utility;
using UnityEngine;

public class CDT 
{
     public static SubdivisionBuilder compute(List<Point> points)
     {
         
        var triangles = BoywerWatson.compute(points);


        var builder = SubdivisionBuilder.builder();
        foreach(BoywerWatson.TriangleGraph triangleGraph in triangles) {
            List<Vertex> triangle = new List<Vertex>();
            var v=triangleGraph.i.v1;
            triangle.Add(new Vertex(v.getX(),v.getY()));
            v=triangleGraph.j.v1;
            triangle.Add(new Vertex(v.getX(),v.getY()));
            v=triangleGraph.k.v1;
            triangle.Add(new Vertex(v.getX(),v.getY()));
            v=triangleGraph.i.v1;
            triangle.Add(new Vertex(v.getX(),v.getY()));
            var f = builder.buildPolygon(triangle);
        }
        return builder;

    }

    public static bool onSegments(MapPoint p, MapPoint q, MapPoint r) {
        return Math.Min(p.getX(),r.getX()) < q.getX() &&  q.getX() < Math.Max(p.getX(),r.getX())
                && Math.Min(p.getY(),r.getY()) < q.getY() &&  q.getY() < Math.Max(p.getY(),r.getY());
    }

    public static bool xOverlap(MapPoint p, MapPoint q, MapPoint r, MapPoint t) {
        double rt = t.getX() - r.getX();
        double rtY = t.getY() - r.getY();
        double xParY = rt/rtY;

        double xp = xParY * (p.getY()-t.getY()) +t.getY();
        double xq = xParY * (q.getY() -t.getY()) + t.getY();
        return xp < p.getX() && xq > q.getX() || xp > p.getX() && xp < q.getX() || xq > p.getX() && xq < q.getX();
    }

    public static bool yOverlap(MapPoint p, MapPoint q, MapPoint r, MapPoint t) {
        double rt = t.getX() - r.getX();
        double rtY = t.getY() - r.getY();
        double yParX = rtY/rt;

        double yP = yParX * (p.getX()-t.getX()) +t.getX();
        double yQ = yParX * (q.getX() -t.getX()) + t.getX();
        return yP < p.getY() && yQ > q.getY() || yP > p.getY() && yP < q.getY() || yQ > p.getY() && yQ < q.getY();
    }

    public static bool intersects(MapPoint p, MapPoint q, MapPoint r, MapPoint t) {
        var o1 = GeometryUtility.toTheLeftOf(p,q,r);
        var o2 = GeometryUtility.toTheLeftOf(p,q,t);

        if(o1 * o2 < 0) {


            return xOverlap(p,q,r,t) && yOverlap(p,q,r,t);
        }

        if(o1== 0  && onSegments(p,r,q)) {
            return true;
        }

        if(o2 == 0 && onSegments(p,t,q)) {
            return true;
        }

        return false;
    }

    public static void applyConstraints(List<ConstraintPolygon> polygons,SubdivisionBuilder subdivision) {
        foreach(ConstraintPolygon polygon in polygons) {
            var segments = polygon.segments;
            foreach(Segment<Vertex> constraint in segments) {
                if(subdivision.edgeExists(constraint.start,constraint.end)) {
                    Debug.Log("constraint segment " + constraint.start.x +" " +constraint.start.y + " " + constraint.end.x +" " + constraint.end.y+ " Exists. No contraint applied");
                    continue;
                }
                Queue<Face> facesToCheck = new Queue<Face>();
                var list = subdivision.vertices[constraint.start];
                foreach(HalfEdge hf in list) {
                    if(hf.incidentFace != null && !facesToCheck.Contains(hf.incidentFace)) {
                        facesToCheck.Enqueue(hf.incidentFace);
                    }
                }
                List<HalfEdge> edgesToCheck = new List<HalfEdge>();
                while (facesToCheck.Count != 0) {
                    Face f = facesToCheck.Dequeue();
                    var listE = subdivision.faces[f];
                    if(listE == null) {
                        Debug.LogError("Mauvaises faces construites.");
                        continue;
                    }
                    HalfEdge edgeI = listE[0];
                    HalfEdge edgeJ = listE[2];
                    HalfEdge edgeK = listE[4];
                    
                    if(intersects(constraint.start,constraint.end,edgeI.v,edgeI.twin.v)) {
                        splitEdgeIfNecessary(subdivision, constraint, edgeI, f, facesToCheck, edgesToCheck);
                        continue;
                    }
                    if(intersects(constraint.start,constraint.end,edgeJ.v,edgeJ.twin.v)) {
                        splitEdgeIfNecessary(subdivision, constraint,edgeJ, f, facesToCheck, edgesToCheck);
                        continue;
                    }
                    if(intersects(constraint.start,constraint.end,edgeK.v,edgeK.twin.v)) {
                        splitEdgeIfNecessary(subdivision, constraint, edgeK, f, facesToCheck, edgesToCheck);
                    }
                }
            }
        }


    }

    public static  void removePolygons(SubdivisionBuilder subdivision, List<ConstraintPolygon> polygons) {
       
        foreach(ConstraintPolygon polygon in polygons) {
            HashSet<Vertex> constrainedVertices = new HashSet<Vertex>();
            List<Segment<Vertex>> segments = polygon.segments;
            foreach(Segment<Vertex> segment in segments) {
                if(!constrainedVertices.Contains(segment.start)) {
                    constrainedVertices.Add(segment.start);
                }
                if(!constrainedVertices.Contains(segment.end)) {
                    constrainedVertices.Add(segment.end);
                }
            }
            
            for(int i = 0 ; i < segments.Count ; i ++) {
                var segment = segments[i];
                
                HalfEdge edge = subdivision.getEdge(segment.start,segment.end);
                if (edge == null)
                {
                    continue;
                }
                // If do not contain then the edge does not lead inside the polygon
                if ( !constrainedVertices.Contains(edge.next.v) || !constrainedVertices.Contains(edge.prev.v))
                {
                    edge = edge.twin;
                    if(!constrainedVertices.Contains(edge.next.v) || !constrainedVertices.Contains(edge.prev.v))
                    {
                        throw new ArgumentException(
                            "La topologie est incorrecte. Le polygon ne peut pas être enlevé. ");
                    }
                }
               
                
                Face f = edge.incidentFace;
                List<HalfEdge> list = null;
                if ( f != null && subdivision.faces.ContainsKey(f))
                {
                     list = subdivision.faces[f];
                }
                else
                {
                    continue;
                }
                
                Face finalF = f;
                bool allToTheLeft = list.All(edge1 => edge1.incidentFace != finalF || GeometryUtility.toTheLeftOf(edge.v,edge.twin.v,edge1.next.v) >= 0);
                // FIXME naive remove

                if(!allToTheLeft) {
                    f = edge.twin.incidentFace;
                    list = subdivision.faces[f];
                }

                foreach(HalfEdge edge1 in list) {
                    if(f == edge1.incidentFace) {
                        edge1.incidentFace = null;
                    }

                }

                subdivision.faces.Remove(f);
            }
        }
    }

    
    private static void splitEdgeIfNecessary(SubdivisionBuilder subdivision, Segment<Vertex> segment,
        HalfEdge edgeIntersecting, Face f, Queue<Face> facesToCheck, List<HalfEdge> edgesToCheck) {
        Face newF = edgeIntersecting.twin.incidentFace;
        HalfEdge fisrt = edgeIntersecting.next;
        HalfEdge second = edgeIntersecting.twin.next;
        HalfEdge split1 = edgeIntersecting.next.next;
        HalfEdge split2 = edgeIntersecting.twin.next.next;
        var fMerged = subdivision.mergeFaces(f,newF, edgeIntersecting, edgeIntersecting.twin);
        if(newF == null) {
            return;
        }
        if(isQuadrilateralConvex(subdivision.faces[fMerged])) {
            var edges = subdivision.splitFace(fMerged,split1,split2);
            if(intersects(segment.start, segment.end,edges.left().v,edges.right().v)) {
                facesToCheck.Enqueue(fMerged);
            } else {
                edgesToCheck.Add(edges.left());
            }
            //facesToCheck.add(edges.right().incidentFace);
        } else {
            subdivision.splitFace(fMerged,fisrt,second);
        }
    }

    public static bool isQuadrilateralConvex(List<HalfEdge> edges) {
        if(edges.Count != 4) return false;
        for(int i = 0 ; i < edges.Count-1; i++) {
            Vertex v = edges[i].v;
            Vertex v2 = edges[i+1].v;
            Vertex toCheck = edges[i+1].twin.v;
            if(GeometryUtility.toTheLeftOf(v,v2,toCheck) < 0) {
                return false;
            }
        }
        return true;
    }

    public class ConstraintPolygon {
        public List<Segment<Vertex>> segments;
        public ConstraintPolygon() {
            segments = new List<Segment<Vertex>>();
        }
    }

}
