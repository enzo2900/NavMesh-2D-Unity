using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

public class SubdivisionBuilder 
{
     public Dictionary<Vertex, List<HalfEdge>> vertices ;
    public Dictionary<Face, List<HalfEdge>> faces;
    List<HalfEdge> cycles;

    private bool edgeToInsertIsWrongDirection;

    private bool insertingAPolygonCCW;
    SubdivisionBuilder() {
        vertices = new Dictionary<Vertex, List<HalfEdge>>();
        faces = new Dictionary<Face, List<HalfEdge>>();
        cycles = new List<HalfEdge>();
        edgeToInsertIsWrongDirection = false;
    }


    public static SubdivisionBuilder builder() {
        return new SubdivisionBuilder();
    }


    /**
     * Merge two adjacents faces from the edge to remove.
     * @param f
     * @param f2
     * @param toRemove
     * @param twin
     * @return the new face merged
     */
    public Face mergeFaces(Face f, Face f2, HalfEdge toRemove, HalfEdge twin)
    {
        vertices[toRemove.v].Remove(toRemove);
        vertices[twin.v].Remove(twin);

        HalfEdge e = toRemove.prev;
        HalfEdge e2 = twin.next;
        HalfEdge e3 = toRemove.next;
        HalfEdge e4 = twin.prev;

        HalfEdge eNext = twin.next;
        HalfEdge e2Prev = toRemove.prev;
        HalfEdge e3Prev = twin.prev;
        HalfEdge e4Next = toRemove.next;

        e.next = eNext;
        e2.prev = e2Prev;
        e3.prev = e3Prev;
        e4.next = e4Next;

        var list = getConnectedEdges(e);
        faces.Remove(f);
        faces.Add(f,list);
        faces.Remove(f2);

        return f;
    }

    public Couple<HalfEdge, HalfEdge> splitFace(Face f, HalfEdge e1, HalfEdge e2)
    {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();

        edge.v = e2.v;
        edgeTwin.v = e1.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;

        HalfEdge eNext = e1;
        HalfEdge ePrev = e2.prev;
        HalfEdge eTnext = e2;
        HalfEdge eTprev = e1.prev;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.prev = eTprev;
        edgeTwin.next = eTnext;

        eNext.prev = edge;
        ePrev.next = edge;
        eTnext.prev = edgeTwin;
        eTprev.next = edgeTwin;

        List<HalfEdge> edges = getConnectedEdges(edge);
        faces.Remove(f);
        faces.Add(f, edges);
        edges.ForEach(edge1 => edge1.incidentFace = f);
        Face f2 = new Face();
        List<HalfEdge> edges2 = getConnectedEdges(edgeTwin);
        //faces.Remove(f2);
        faces.Add(f2, edges2);
        edges2.ForEach(edge1 => edge1.incidentFace = f2);
        vertices[edge.v].Add(edge);
        vertices[edgeTwin.v].Add(edgeTwin);

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);

    }

    public void swapEdgeFace(HalfEdge toSwap, Face f, Face f2) {
        HalfEdge opposite = toSwap.next.next;
        HalfEdge opposite2 = toSwap.twin.next.next;

        HalfEdge eNext = opposite;
        HalfEdge ePrev = opposite2.prev;
        HalfEdge eTNext = opposite2;
        HalfEdge eTPrev = opposite.prev;

        toSwap.next = eNext;
        toSwap.prev = ePrev;
        HalfEdge twin = toSwap.twin;

        twin.prev = eTPrev;
        twin.next = eTNext;

        eNext.prev = toSwap;
        ePrev.next = toSwap;
        eTPrev.next = twin;
        eTNext.prev = twin;

        List<HalfEdge> edges = getConnectedEdges(toSwap);
        faces.Add(f,edges);
        List<HalfEdge> edges2 = getConnectedEdges(toSwap.twin);
        faces.Add(f2,edges2);
    }

    public List<HalfEdge> getConnectedEdges(HalfEdge edge) {
        List<HalfEdge> halfEdgesFound = new List<HalfEdge>();
        HalfEdge first = edge;
        halfEdgesFound.Add(first);

        HalfEdge next = edge.next;
        while (next != first) {
            halfEdgesFound.Add(next);
            next = next.next;

        }
        //halfEdgesFound.add(first);
        return halfEdgesFound;
    }

    public List<HalfEdge> get(Vertex v) {
        return vertices[v];
    }
    public List<HalfEdge> getAdjacents(HalfEdge edge) {
        return vertices[edge.v];
        //return EdgeUtility.getAdjacentsEdges(edge);
    }
    public Couple<HalfEdge,HalfEdge> connectEdges(HalfEdge e1, HalfEdge e2) {
        var listV2 = getAdjacents(e2);
        if(listV2.Count == 1) {
            if(GeometryUtility.toTheLeftOf(e1.v,e1.twin.v,e2.v) < 0 && !insertingAPolygonCCW) {
                edgeToInsertIsWrongDirection = true;
                return EdgeUtility.connectEdgesCW(e1,e2);
            }
            return EdgeUtility.connectEdges(e1,e2);
        } else {

            var newEdge = EdgeUtility.createEdgeFromVertices(e1.twin.v,e2.v);
            if(GeometryUtility.toTheLeftOf(e1.v,e1.twin.v,e2.v) < 0 && !insertingAPolygonCCW) {
                // Wrong direction
                edgeToInsertIsWrongDirection = true;
                var lf2 = EdgeUtility.getLeftAndRightOf(newEdge.right(),listV2);
                HalfEdge leftConnection2 = lf2.left();
                HalfEdge rightConnection2 = lf2.right();
                return EdgeUtility.connectEdgeToEdgesCW(e1,rightConnection2,leftConnection2);
            }
            var lf = EdgeUtility.getLeftAndRightOf(newEdge.right(),listV2);
            HalfEdge leftConnection = GeometryUtility.toTheLeftOf(e2.v,e2.twin.v,e1.twin.v) > 0 ? lf.left().twin : lf.right().twin;
            HalfEdge rightConnection = GeometryUtility.toTheLeftOf(e2.v,e2.twin.v,e1.twin.v) > 0 ? lf.right() : lf.left();
            leftConnection = lf.left().twin;
            rightConnection = lf.right();
            return EdgeUtility.connectEdgeToEdges2(e1,rightConnection,leftConnection);
            //return EdgeUtility.connectEdgeToEdges(e1,rightConnection,leftConnection);
        }
    }

    /**
     *
     * @param edges A list with more than 1 elements
     * @param edges2 A list with more than 1 elements
     * @return an CCW edge and the twin CW edge
     */
    public Couple<HalfEdge,HalfEdge> connect(List<HalfEdge> edges,List<HalfEdge> edges2) {
        //throw new RuntimeException("Not implemented");
        var edge = edges[0];
        var v1 = edges[0].v;
        var v2 = edges2[0].v;
        Couple<HalfEdge, HalfEdge> edge1 = null;
        Couple<HalfEdge, HalfEdge> edge2 = null;
        Couple<HalfEdge, HalfEdge> lf1 = null;
        Couple<HalfEdge, HalfEdge> lf2 = null;
        //TODO detect if the edge is in wrong order
        if(GeometryUtility.toTheLeftOf(edge.twin.v,edge.v,v2) > 0 &&!insertingAPolygonCCW) {
            edgeToInsertIsWrongDirection = true;
             edge1 = EdgeUtility.createEdgeFromVertices(v1,v2);
            edge2 = EdgeUtility.createEdgeFromVertices(v2,v1);
            lf1 = EdgeUtility.getLeftAndRightOf(edge1.left(),edges);
            lf2 = EdgeUtility.getLeftAndRightOf(edge2.left(),edges2);
            return EdgeUtility.connectEdgesToEdgesCW(lf1.left().twin,lf1.right(),
                    lf2.left().twin,lf2.right());
        }
        edge1 = EdgeUtility.createEdgeFromVertices(v1,v2);
         edge2 = EdgeUtility.createEdgeFromVertices(v2,v1);
        lf1 = EdgeUtility.getLeftAndRightOf(edge1.left(),edges);
        lf2 = EdgeUtility.getLeftAndRightOf(edge2.left(),edges2);
        return EdgeUtility.connectEdgesToEdges(lf1.left().twin,lf1.right(),lf2.left().twin,lf2.right());
    }

    public static bool goToTheWrongDirection(Vertex v, Vertex v2) {
        double x = v2.x - v.x;
        double y = v2.y - v.y;
        return x< 0 || y < 0;
    }
    public bool exists(Vertex v) {
        return vertices.ContainsKey(v);
    }

    public Couple<HalfEdge,HalfEdge> connect(Vertex v, Vertex v2) {
        /*if(exists(v)) {
            connect(vertices.get(v),vertices.get(v2));
        }*/
        
        
        if (!vertices.ContainsKey(v2))
        {
            var listV1 = vertices[v];
            if(listV1.Count ==1) {
                edgeToInsertIsWrongDirection = true;
                return EdgeUtility.connectVertexCW(listV1[0].twin,v);
            } else {
                var newEdge = EdgeUtility.createEdgeFromVertices(v,v2);
                edgeToInsertIsWrongDirection = true;
                var lf = EdgeUtility.getLeftAndRightOf(newEdge.left(),listV1);
                HalfEdge BeforeConnection = GeometryUtility.toTheLeftOf(v,lf.right().twin.v,v2) > 0 ? lf.left().twin : lf.right().twin;
                HalfEdge AfterConnection = GeometryUtility.toTheLeftOf(v,lf.right().twin.v,v2) > 0 ? lf.right() : lf.left();
                return EdgeUtility.connectVertexToBothEdgeCW(BeforeConnection,AfterConnection,v2);
            }


        }
        var listV2 = vertices[v2];
        if(listV2.Count == 1) {
            var newEdge = EdgeUtility.createEdgeFromVertices(v2,v);
            var edge = listV2[listV2.Count-1].twin;
            if(GeometryUtility.toTheLeftOf(edge.v,edge.twin.v,v) > 0 || insertingAPolygonCCW) {
                return EdgeUtility.connectVertex(listV2[0].twin,v);
            } else {
                edgeToInsertIsWrongDirection = true;
                return EdgeUtility.connectVertexCW(listV2[0].twin,v);
            }

        } else {
            var t0 = listV2[0].twin.prev;
            var newEdge = EdgeUtility.createEdgeFromVertices(v2,v);

            if(GeometryUtility.toTheLeftOf(t0.v,t0.twin.v,v) <= 0 && !insertingAPolygonCCW) {
                edgeToInsertIsWrongDirection = true;
                var lf2 = EdgeUtility.getLeftAndRightOf(newEdge.left(),listV2);
                HalfEdge BeforeConnection2 = GeometryUtility.toTheLeftOf(v2,lf2.right().twin.v,v) > 0 ? lf2.left().twin : lf2.right().twin;
                HalfEdge AfterConnection2 = GeometryUtility.toTheLeftOf(v2,lf2.right().twin.v,v) > 0 ? lf2.right() : lf2.left();
                return EdgeUtility.connectVertexToBothEdgeCW(BeforeConnection2,AfterConnection2,v);
            }
            var lf = EdgeUtility.getLeftAndRightOf(newEdge.left(),listV2);

            HalfEdge BeforeConnection = GeometryUtility.toTheLeftOf(v2,lf.right().twin.v,v) > 0 ? lf.left().twin : lf.right().twin;
            HalfEdge AfterConnection = GeometryUtility.toTheLeftOf(v2,lf.right().twin.v,v) > 0 ? lf.right() : lf.left();
            BeforeConnection = lf.left().twin;
            AfterConnection = lf.right();
            return EdgeUtility.connectVertexToBothEdge2(BeforeConnection,AfterConnection,v);
            //return EdgeUtility.connectVertexToBothEdge(BeforeConnection,AfterConnection,v);
        }

    }

    public SubdivisionBuilder buildVertex(Vertex v,Vertex v2) {
        edgeToInsertIsWrongDirection = false;
        var couple = build(v,v2);
        var listV1 = vertices.GetValueOrDefault(v,new List<HalfEdge>());
        var listV2 = vertices.GetValueOrDefault(v2,new List<HalfEdge>());
        if(edgeToInsertIsWrongDirection) {
            listV2.Add(couple.left());
            listV1.Add(couple.right());
            vertices.Add(v,listV1);
            vertices.Add(v2,listV2);
        } else {
            listV1.Add(couple.left());
            vertices.Add(v,listV1);
            listV2.Add(couple.right());
            vertices.Add(couple.right().v,listV2);
        }

        return this;
    }

    public Couple<HalfEdge,HalfEdge> build(Vertex v,Vertex v2) {
        if(!exists(v) && !exists(v2)) {
            //if(goToTheWrongDirection(v,v2)) {
              //  edgeToInsertIsWrongDirection = true;
               // return EdgeUtility.createEdgeFromVertices(v2,v);
            //}
            return EdgeUtility.createEdgeFromVertices(v,v2);

        } else if ((exists(v2)))
        {
            var listV2 = vertices[v2];
            
            if(!vertices.ContainsKey(v)) {
                return connect(v2,v);
            }
            var listV1 = vertices[v];
            if(listV2.Count== 1 && listV1.Count == 1) {

                return connectEdges( listV1[0].twin,listV2[0]);


            } else if(listV2.Count> 1 && listV1.Count > 1) {
                return connect(listV1,listV2);

            } else if(listV2.Count > 1) {
                return connectEdges(listV1[0].twin,listV2[0]);
            }else {
                HalfEdge e1 = listV1[0];
                HalfEdge e2 = listV2[0];
                var newEdge = EdgeUtility.createEdgeFromVertices(v,v2);

                    // Wrong direction
                edgeToInsertIsWrongDirection = true;
                var lf = EdgeUtility.getLeftAndRightOf(newEdge.left(),listV1);
                HalfEdge leftConnection = lf.left().twin;
                HalfEdge rightConnection = lf.right();
                return EdgeUtility.connectEdgeToEs(e2,rightConnection,leftConnection);
            }
        } else {
            return connect(v2,v);
        }
    }

    public Couple<HalfEdge,HalfEdge> buildV(Vertex v, Vertex v2) {
        edgeToInsertIsWrongDirection = false;
        var couple = build(v,v2);

        var listV1 = vertices.GetValueOrDefault(couple.left().v,new List<HalfEdge>());
        var listV2 = vertices.GetValueOrDefault(couple.right().v,new List<HalfEdge>());
        if(edgeToInsertIsWrongDirection) {
            listV1.Add(couple.left());
            listV2.Add(couple.right());
            if (!vertices.ContainsKey(couple.Left.v))
            {
                vertices.Add(couple.left().v,listV1);
            }
            if (!vertices.ContainsKey(couple.Right.v))
            {
                vertices.Add(couple.right().v,listV2);    
            }
        } else {
            listV1.Add(couple.left());
            if (!vertices.ContainsKey(couple.Left.v))
            {
                vertices.Add(couple.left().v,listV1);
            }
            
            listV2.Add(couple.right());
            if (!vertices.ContainsKey(couple.Right.v))
            {
                vertices.Add(couple.right().v,listV2);    
            }
            
        }

        return couple;
    }

    public bool edgeExists(Vertex v, Vertex v2)
    { 
        List<HalfEdge> list = null;
        if (vertices.ContainsKey(v))
        {
            list = vertices[v];
        }

        List<HalfEdge> list2 = null;
        if (vertices.ContainsKey(v2))
        {
            list2 = vertices[v2];
        }
        if(list == null && list2 == null) return false;

        if(list != null)
        {
            return list.Any(edge => edge.twin.v.Equals(v2));

        }
        
        return list2.Any(edge => edge.twin.v.Equals(v)  );
    }

    public HalfEdge getEdge(Vertex v, Vertex v2) {
        var list = vertices[v];
        var newList = list.Where(edge => edge.twin.v.Equals(v2)).ToList();
        if (newList.Count == 0)
        {
            return null;
        }
        return newList[0];
    }
    /**
     *
     * @param vertices
     * @return return the face of the polygon
     */
    public Face buildPolygon(List<Vertex> verticesList)
    {
        List<HalfEdge> halfEdges = new List<HalfEdge>();
        List<Vertex> vertices;
        if(isCWPolygon(verticesList))
        {
            verticesList.Reverse();
            vertices = verticesList;
            //vertices = verticesList.Reverse(0,this.vertices.Count-1);
        }else {
            vertices = verticesList;
        }
        insertingAPolygonCCW = true;
        Face f = new Face();
        for(int i = 0 ; i < vertices.Count-1 ; i ++) {
            Vertex v1 = vertices[i];
            Vertex v2 = vertices[i+1];
            if(edgeExists(v1,v2)) {

                var edge = getEdge(v1,v2);
                if(edge.incidentFace != null) {
                    edge = edge.twin;
                }

                //HalfEdge TNext = halfEdges.get(i-1);
                //edge.next = TNext;
                halfEdges.Add(edge);
                halfEdges.Add(edge.twin);
                edge.incidentFace = f;
                continue;
            }
            var couple = buildV(vertices[i],vertices[i+1]);
            halfEdges.Add(couple.left());
            halfEdges.Add(couple.right());
            couple.left().incidentFace = f;
        }
        f.outerComponent = halfEdges[0];
        faces.Add(f,halfEdges);
        insertingAPolygonCCW  =false;
        return f;
    }

    public Triangle toTriangle(Face f) {
        var list = faces[f];
        if(list.Count == 6) {
            Triangle t = new Triangle(list[0].v,list[2].v,list[4].v);
            return t;
        }
        return null;
    }

    public List<Triangle> getTriangles() {
        List<Triangle> triangles = new List<Triangle>();
        foreach (Face f in faces.Keys)
        {
            var list = faces[f];
            if(list.Count == 6) {
                //Triangle t = new Triangle(list.get(0).v,list.get(2).v,list.get(4).v);
                
                triangles.Add(toTriangle(f));
            }
        }
        
        return triangles;
    }

    public static bool isCWPolygon(List<Vertex> vertices) {
        double sum = 0;
        for(int i = 0 ; i < vertices.Count-1; i++) {
            Vertex v1 = vertices[i];
            Vertex v2 = vertices[i+1];
            sum += (v1.x * v2.y) - (v2.x * v1.y);
        }
        return sum < 0;
    }
    
    public static bool inEdgeBounded(List<HalfEdge> edges) {
        HalfEdge first =edges.First();
        if(first.incidentFace == null)
        {
            throw new ArgumentException(
                "The algorithm is so bad it cant get a correct in edge on the first try in inedgeBounded");
        }
        HalfEdge current = first.next;
        int counter = 0;
        int maxCounter = edges.Count;
        while (current != first) {
            counter++;
            if(maxCounter <= counter)
            {
                throw new Exception("OuterEdges are not bouded");
            }
            if(current.incidentFace == null) {
                throw new Exception("Not an in edge face");
            }
            if(!edges.Contains(current)) {
                throw new Exception("inEdge is not in the polygon. The polygon has not a correct in bound.");
            }
            current = current.next;
        }
        return true;
    }
    
    public static bool outerEdgeBounded(List<HalfEdge> edges) {
        HalfEdge first = edges.Where(e=>  e.incidentFace == null).First();

        if(first.incidentFace != null) {
            throw new Exception("The algorithm is so bad it cant get a correct outer edge on the first try in outerEdgeBounded");
        }
        HalfEdge current = first.next;
        int counter = 0;
        int maxCounter = edges.Count;
        while (current != first) {
            counter++;
            if(maxCounter <= counter) {
                throw new Exception("OuterEdges are not bouded");
            }
            if(current.incidentFace != null) {
                throw new Exception("Not an outer edge face");
            }
            if(!edges.Contains(current)) {
                throw new Exception("OuterEdge is not in the polygon. The polygon has not a correct outer bound.");
            }
            current = current.next;
        }
        return true;


    }



}
