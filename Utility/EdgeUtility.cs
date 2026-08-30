using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Utility;

public class EdgeUtility 
{
    public static Couple<HalfEdge,HalfEdge> connectEdges(HalfEdge e, HalfEdge e2) {

        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = e.twin.v;
        edgeTwin.v = e2.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;
        HalfEdge eNext = e2;
        HalfEdge ePrev = e;

        HalfEdge e2Next = e.twin;
        HalfEdge e2Prev = e2.twin;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = e2Next;
        edgeTwin.prev = e2Prev;

        e2Prev.next = edgeTwin;
        ePrev.next = edge;
        e2Next.prev = edgeTwin;
        eNext.prev = edge;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }
    
    public static Couple<HalfEdge,HalfEdge> connectEdgesCW(HalfEdge e, HalfEdge e2) {

        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = e2.v;
        edgeTwin.v = e.twin.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;
        HalfEdge eNext = e.twin;
        HalfEdge ePrev = e2.twin;

        HalfEdge e2Next = e2;
        HalfEdge e2Prev = e;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = e2Next;
        edgeTwin.prev = e2Prev;

        e2Prev.next = edgeTwin;
        ePrev.next = edge;
        e2Next.prev = edgeTwin;
        eNext.prev = edge;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }
    
    public static Couple<HalfEdge,HalfEdge> createEdgeFromVertices(MapPoint a, MapPoint b) {
        HalfEdge previous = new HalfEdge();
        HalfEdge twinPrevious = new HalfEdge();

        Vertex vPrevious = new Vertex(a.getX(),a.getY());
        Vertex vPTwin = new Vertex(b.getX(),b.getY());
        previous.v = vPrevious;
        twinPrevious.v = vPTwin;
        previous.twin = twinPrevious;
        twinPrevious.twin = previous;

        previous.next = twinPrevious;
        previous.prev = twinPrevious;
        twinPrevious.next = previous;
        twinPrevious.prev = previous;
        vPrevious.incidentEdge = previous;
        vPTwin.incidentEdge = twinPrevious;

        return new Couple<HalfEdge,HalfEdge>(previous,twinPrevious);
    }
    
     public static Couple<HalfEdge,HalfEdge> connectEdgeToEdgesCW(HalfEdge e, HalfEdge eBase,HalfEdge eBPrev) {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = eBase.v;
        edgeTwin.v = e.twin.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;

        HalfEdge eNext = e.twin;
        HalfEdge ePrev = eBPrev.twin;

        HalfEdge eTNext = eBase;
        HalfEdge eTPrev = e;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = eTNext;
        edgeTwin.prev = eTPrev;

        eTPrev.next = edgeTwin;
        ePrev.next = edge;
        eTNext.prev = edgeTwin;
        eNext.prev = edge;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }
    public static Couple<HalfEdge,HalfEdge> connectEdgeToEdges2(HalfEdge e, HalfEdge eRight,HalfEdge eLeft) {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = e.twin.v;
        edgeTwin.v = eRight.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;
        HalfEdge eNext = eRight;
        HalfEdge ePrev = e;

        HalfEdge eTNext = e.twin;
        HalfEdge eTPrev = eLeft;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.prev = eTPrev;
        edgeTwin.next = eTNext;

        eRight.prev = edge;
        eLeft.next = edgeTwin;
        eTPrev.next = edgeTwin;
        eNext.prev = edge;
        ePrev.next = edge;
        eTNext.prev = edgeTwin;
        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);


    }

    /**
     * Return a new edges from 4 edges
     * @param eLeft The left edge
     * @param eRight the right edge
     * @param e2Left the left edge of the connection end
     * @param e2Right
     * @return
     */
    public static Couple<HalfEdge,HalfEdge> connectEdgesToEdges(HalfEdge eLeft,HalfEdge eRight, HalfEdge e2Left, HalfEdge e2Right) {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = eRight.v;
        edgeTwin.v = e2Right.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;

        HalfEdge eNext = e2Right;
        HalfEdge ePrev = eLeft;
        HalfEdge eTNext = eRight;
        HalfEdge eTPrev = e2Left;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = eTNext;
        edgeTwin.prev = eTPrev;

        eNext.prev = edge;
        ePrev.next = edge;
        eTNext.prev = edgeTwin;
        eTPrev.next = edgeTwin;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }

    public static Couple<HalfEdge,HalfEdge> connectEdgesToEdgesCW(HalfEdge eLeft,HalfEdge eRight, HalfEdge e2Left, HalfEdge e2Right) {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = e2Right.v;
        edgeTwin.v = eRight.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;

        HalfEdge eNext = eRight;
        HalfEdge ePrev = e2Left;
        HalfEdge eTNext = e2Right;
        HalfEdge eTPrev = eLeft;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = eTNext;
        edgeTwin.prev = eTPrev;

        eNext.prev = edge;
        ePrev.next = edge;
        eTNext.prev = edgeTwin;
        eTPrev.next = edgeTwin;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }

    public static Couple<HalfEdge,HalfEdge> connectEdgeToEdges(HalfEdge e, HalfEdge eBase,HalfEdge eBPrev) {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = e.twin.v;
        edgeTwin.v = eBase.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;

        if(GeometryUtility.toTheLeftOf(eBPrev.v,eBPrev.twin.v,e.v) > 0) {

        }
        HalfEdge eNext = eBPrev.twin;
        HalfEdge ePrev = e;

        HalfEdge eTNext = e.twin;
        HalfEdge eTPrev = eBase.twin;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = eTNext;
        edgeTwin.prev = eTPrev;

        eTPrev.next = edgeTwin;
        ePrev.next = edge;
        eTNext.prev = edgeTwin;
        eNext.prev = edge;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }

    /**
     * Connect the edge to the
     * @param e
     * @param v2
     */
    public static Couple<HalfEdge,HalfEdge> connectEdgeToVertex(HalfEdge e, Vertex v2) {
        bool isCycle = isInCycle(e);

        // point d’attache : twin si cycle, sinon e lui-même
        HalfEdge anchor = isCycle ? e.twin : e;

        HalfEdge e2 = new HalfEdge();
        HalfEdge e2Twin = new HalfEdge();

        // Origines
        e2.v = anchor.v;
        e2Twin.v = v2;

        // Twins
        e2.twin = e2Twin;
        e2Twin.twin = e2;

        e2Twin.incidentFace = null;

        // Si l'arête fait partie d'un cycle → insertion dans la boucle
        if (isCycle) {
            HalfEdge p = anchor.prev;
            p.next = e2;
            e2.prev = p;
            e2.next = anchor;
            anchor.prev = e2;
        }
        // Sinon → demi-arête isolée
        else {
            e2.next = e2Twin;
            e2.prev = anchor;
            anchor.next = e2;
            e2Twin.next = anchor.twin;
            e2Twin.prev = e2;
            anchor.twin.prev = e2Twin;
        }

        return new Couple<HalfEdge,HalfEdge>(e2,e2Twin);
    }
    
    public static Couple<HalfEdge,HalfEdge> connectEdgeToEs(HalfEdge e, HalfEdge eBase,HalfEdge eBPrev) {
        HalfEdge edge = new HalfEdge();
        HalfEdge edgeTwin = new HalfEdge();
        edge.v = eBase.v;
        edgeTwin.v = e.v;
        edge.twin = edgeTwin;
        edgeTwin.twin = edge;

        HalfEdge eNext = e;
        HalfEdge ePrev = eBPrev;

        HalfEdge eTNext = eBase;
        HalfEdge eTPrev = e.twin;

        edge.next = eNext;
        edge.prev = ePrev;
        edgeTwin.next = eTNext;
        edgeTwin.prev = eTPrev;

        eTPrev.next = edgeTwin;
        ePrev.next = edge;
        eTNext.prev = edgeTwin;
        eNext.prev = edge;

        return new Couple<HalfEdge,HalfEdge>(edge,edgeTwin);
    }
    
    public static List<HalfEdge> getAdjacentsEdges(HalfEdge edge) {
        List<HalfEdge> edges = new List<HalfEdge>();
        List<HalfEdge> visited = new List<HalfEdge>();
        HalfEdge first =edge;

        visited.Add(first);
        visited.Add(first.twin);
        edges.Add(first);
        HalfEdge current = first.twin.next;
        int compteur = 0;
        while (!current.Equals( first)) {
            if(!visited.Contains(current) && ! visited.Contains(current.twin)) {
                visited.Add(current);
                visited.Add(current.twin);

                edges.Add(current);
            }


            current = current.twin.next;
            compteur++;
            if (compteur > 1000)
            {
                Debug.LogError("Loop");
                throw new Exception("Loop");
            }
        }
        return edges;
    }

    
    
    public static Couple<HalfEdge,HalfEdge> connectVertex(HalfEdge e, Vertex v) {
        HalfEdge e1 = new HalfEdge();
        HalfEdge eTwin = new HalfEdge();
        e1.v = e.twin.v;
        v.incidentEdge = e1;
        eTwin.v = v;
        eTwin.twin = e1;
        e1.twin = eTwin;
        connectEdges(e,e1,eTwin);
        return new Couple<HalfEdge,HalfEdge>(e1,eTwin);

    }
    public static void connectEdges(HalfEdge e, HalfEdge hfConnect, HalfEdge hfConnectTwin) {
        hfConnect.twin = hfConnectTwin;
        hfConnectTwin.twin = hfConnect;
        HalfEdge eTwinNext= e.twin;
        HalfEdge eTwinPrev = hfConnect;
        HalfEdge eNext = hfConnectTwin;
        HalfEdge ePrev = e;

        hfConnect.prev = ePrev;
        hfConnect.next = eNext;
        hfConnectTwin.next = eTwinNext;
        hfConnectTwin.prev = eTwinPrev;

        e.next = hfConnect;
        e.twin.prev = hfConnectTwin;
    }

    
    public static bool isInCycle(HalfEdge e) {
        List<HalfEdge> visited = new List<HalfEdge>();
        HalfEdge current = e;
        Queue<HalfEdge> file = new Queue<HalfEdge>();
        Dictionary<HalfEdge, HalfEdge> parents = new Dictionary<HalfEdge, HalfEdge>();
        file.Enqueue(e);
        parents.Add(e,e);
        visited.Add(e);
        visited.Add(e.twin);
        int counter = 0;
        while (file.Count != 0) {
            HalfEdge head = file.Dequeue();
            HalfEdge eNext = head.next;
            counter++;
            if (counter > 1000)
            {
                Debug.LogError("Loop");
                throw new Exception("Loop");
            }
            if(eNext == null) {
                return false;
            }
            if(!visited.Contains(eNext)) {
                visited.Add(eNext);
                visited.Add(eNext.twin);
                file.Enqueue(eNext);
                parents.Add(eNext,head);
            } else {
                if(!eNext.v.Equals(head.v) && eNext != head.prev && eNext.v.Equals(e.v)) {
                    return true;
                }
            }
        }
        return false;

    }

    public static Couple<HalfEdge,HalfEdge> connectVertexCW(HalfEdge e, Vertex v) {
        HalfEdge e1 = new HalfEdge();
        HalfEdge eTwin = new HalfEdge();
        e1.v = v;
        v.incidentEdge = e1;
        eTwin.v = e.twin.v;
        eTwin.twin = e1;
        e1.twin = eTwin;
        HalfEdge eTwinNext= e1;
        HalfEdge eTwinPrev = e;
        HalfEdge eNext = e.twin;
        HalfEdge ePrev = eTwin;

        e1.prev = ePrev;
        e1.next = eNext;
        eTwin.next = eTwinNext;
        eTwin.prev = eTwinPrev;

        e.next = eTwin;
        e.twin.prev = e1;
        return new Couple<HalfEdge,HalfEdge>(e1,eTwin);

    }
    
    /**
     * Get the left and right of edge that are closest to toCompare (right and left)
     * The edge compared are adjacents to edge.origin.
     * @param edge
     * @param toCompare
     * @return
     */
    /*public static Couple<HalfEdge,HalfEdge> getLeftAndRightOf(HalfEdge edge, HalfEdge toCompare) {
        var list = getAdjacentsEdges(edge);
        return getLeftAndRightOf(toCompare,list);

    }
    public static Couple<HalfEdge,HalfEdge> getLeftAndRightOfNoTwin(HalfEdge edge, HalfEdge toCompare) {
        var list = getAdjacentsEdgesSameDir(edge);
        return getLeftAndRightOf(toCompare,list);
    }*/
    public static Couple<HalfEdge,HalfEdge> getLeftAndRightOf( HalfEdge toCompare,List<HalfEdge> list) {
        List<double> angles = new List<double>();

        PriorityQ<HalfEdge> queue = new PriorityQ<HalfEdge>();
        
        //PriorityQueue<Couple<double,HalfEdge>> queue = new PriorityQueue<>((o1,o2) -> -Double.compare(o1.left(),o2.left()));
        float[] toCompareNormalized = toCompare.normalized();
        foreach(HalfEdge edge1 in list) {
            if(edge1 == toCompare)continue;
            float[] normalized = edge1.normalized();
            float cross = GeometryUtility.toTheLeftOf(new Vertex(0,0),new Vertex(toCompareNormalized[0],toCompareNormalized[1])
                    ,new Vertex(normalized[0],normalized[1]));
            float dot = GeometryUtility.dotProduct(new Vertex(0,0),new Vertex(toCompareNormalized[0],toCompareNormalized[1])
                    ,new Vertex(normalized[0],normalized[1]));
            //double dotProduct = GeometryUtility.dotProduct(toCompare.v,toCompare.twin.v,edge1.twin.v);
            //dotProduct = dotProduct > 0 ? dotProduct >= 1 ? dotProduct+1 *dotProduct+1 : 2 : 1;
            queue.Add((Mathf.Sign(cross) * (1+dot)),edge1);
            //queue.offer(new Couple<>(GeometryUtility.toTheLeftOf(toCompare.v,toCompare.twin.v,edge1.twin.v) * dotProduct,edge1));

        }
        
        HalfEdge left = queue.Dequeue();
        int iterationsCount = queue.Size() -1;
        for (int i = 0 ; i < iterationsCount; i++) {
            queue.Dequeue();
        }
        HalfEdge right = null;
        if(queue.IsEmpty()) {
            right = left.twin;
        } else {
             right = queue.Dequeue();
        }


        return new Couple<HalfEdge,HalfEdge>(left,right);

    }


    public static Couple<HalfEdge,HalfEdge> connectVertexToBothEdge2(HalfEdge e, HalfEdge e2, Vertex v) {
        HalfEdge e1 = new HalfEdge();
        HalfEdge eTwin = new HalfEdge();
        e1.v = e2.v;

        eTwin.v = v;
        eTwin.twin = e1;
        e1.twin = eTwin;
        v.incidentEdge = e1;

        HalfEdge eNext = eTwin;
        HalfEdge ePrev = e;
        HalfEdge eTNext = e2;
        HalfEdge eTPrev = e1;

        e1.next = eNext;
        e1.prev = ePrev;
        eTwin.next = eTNext;
        eTwin.prev = eTPrev;

        e.next = e1;
        e2.prev = eTwin;
        return new Couple<HalfEdge,HalfEdge>(e1,eTwin);
    }
    
    public static Couple<HalfEdge,HalfEdge> connectVertexToBothEdgeCW(HalfEdge e, HalfEdge e2, Vertex v) {
        HalfEdge e1 = new HalfEdge();
        HalfEdge eTwin = new HalfEdge();
        e1.v = v;

        eTwin.v = e2.v;
        eTwin.twin = e1;
        e1.twin = eTwin;
        v.incidentEdge = e1;


        // Connect with the twins
        HalfEdge eNext = e2;
        HalfEdge ePrev = eTwin;
        HalfEdge eTNext = e1;
        HalfEdge eTPrev = e;
        e1.next = eNext;
        e1.prev = ePrev;
        eTwin.next = eTNext;
        eTwin.prev = eTPrev;

        e.next = eTwin;
        e2.prev = e1;

        return new Couple<HalfEdge,HalfEdge>(e1,eTwin);
    }




}
