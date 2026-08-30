using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Object = System.Object;

public class BoywerWatson
{
     public static List<TriangleGraph> compute(List<Point> points) {
        //var builder = Graph2DTopologyBuilder.builder();
        float yMax = float.MinValue;
        float yMin = float.MaxValue;
        float xMax = float.MinValue;
        float xMin = float.MaxValue;
        foreach(Point p in points) {
            if(p.x >  xMax) {
                xMax = p.x;
            }
            if(p.x < xMin) {
                xMin = p.x;
            }
            if(p.y > yMax) {
                yMax = p.y;
            }
            if(p.y < yMin) {
                yMin = p.y;
            }
        }
        if(xMin == 0) {
            xMin = -10;
        }
        if(yMin == 0) {
            yMin = -10;
        }
        xMax *=10;
        xMin =Math.Abs(xMin)*-10;
        yMax *=10;
        yMin = Math.Abs(yMin)*-10;
        /*builder.addEdge(new Vertex(0,yMax),new Vertex(xMin,yMin))
                .addEdge(new Vertex(xMin,yMin),new Vertex(xMax,yMin))
                .addEdge(new Vertex(xMax,yMin),new Vertex(0,yMax));*/
        var triangleGraphs = new List<TriangleGraph>();
        triangleGraphs.Add(new TriangleGraph(new Vertex(0,yMax),new Vertex(xMin,yMin),new Vertex(xMax,yMin)));

        foreach(Point point in points) {
            List<TriangleGraph> badTriangles = new List<TriangleGraph>();
            //var triangles =getTriangles(builder);
            foreach(TriangleGraph triangle in triangleGraphs) {
                if (GeometryUtility.isInsideCircleD(triangle.k.v1,triangle.j.v1,triangle.i.v1,point)) {
                    badTriangles.Add(triangle);
                }
            }
            List<Edge> edgesList = new List<Edge>();
            foreach(TriangleGraph badTriangle in badTriangles) {

                Edge[] edges = new Edge[]{badTriangle.i,badTriangle.j,badTriangle.k};
                foreach(Edge edge in edges) {
                    bool shared = triangleListContainsEdge(badTriangle, edge, badTriangles);
                    if(!shared) {
                        edgesList.Add(edge);
                    }
                }
            }
            foreach(TriangleGraph triangle in badTriangles) {
                triangleGraphs.Remove(triangle);

            }

            Vertex newV = new Vertex(point.x,point.y);
            foreach(Edge edge in edgesList) {
                // Create a triangle
                TriangleGraph triangleGraph = new TriangleGraph(edge.v1,newV,edge.v2);
                //triangleGraph.i = new Edge(edge.v1(),newV);
                //triangleGraph.j = new Edge(newV,edge.v2());
                //triangleGraph.k = new Edge(edge.v2(),edge.v1());
                triangleGraphs.Add(triangleGraph);
                //addOneTriangleInside(builder,edge,newV);

            }
            //builder.showGraph();
           /* try {
                //new CountDownLatch(3).await(3, TimeUnit.SECONDS);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }*/

        }
        //var triangles = getTriangles(builder);
        foreach(TriangleGraph t in new List<TriangleGraph>(triangleGraphs)) {
            List<Vertex> vertices = new List<Vertex>();
            vertices.Add(new Vertex(t.i.v1.getX(),t.i.v1.getY()));
            vertices.Add(new Vertex(t.j.v1.getX(),t.j.v1.getY()));
            vertices.Add(new Vertex(t.k.v1.getX(),t.k.v1.getY()));
            float finalYMin = yMin;
            float finalYMax = yMax;
            float finalXMax = xMax;
            float finalXMin = xMin;
            if(vertices.Any(v => v.Equals(new Vertex(0, finalYMax))
                    || v.Equals(new Vertex(finalXMax, finalYMin))
                    || v.Equals(new Vertex(finalXMin, finalYMin)))) {
                //removeTriangle(builder,t);
                triangleGraphs.Remove(t);
            }
        }
        //builder.removeVertex(new Vertex(0,yMax));
        //builder.removeVertex(new Vertex(xMax,yMin));
        //builder.removeVertex(new Vertex(xMin,yMin));
        /*builder.showGraph();
        try {
            new CountDownLatch(1).await(2,TimeUnit.SECONDS);
        } catch (InterruptedException e) {
            throw new RuntimeException(e);
        }*/
        return triangleGraphs;
    }
     
    public static bool triangleListContainsEdge(TriangleGraph badTriangle, Edge edge, List<TriangleGraph> badTriangles) {
        bool shared = false;
        foreach(TriangleGraph t in badTriangles) {
            if (t != badTriangle && (t.i.Equals(edge) || t.j.Equals(edge) || t.k.Equals(edge))) {
                // t contains edge ?
                shared = true;
                break;
            }
        }
        return shared;
    }

     public class TriangleGraph
     {
         public Edge i;
         public Edge j;
         public Edge k;

         public TriangleGraph(MapPoint p1, MapPoint p2, MapPoint p3)
         {
             this.i = new Edge(p1, p2);
             this.j = new Edge(p2, p3);
             this.k = new Edge(p3, p1);
         }
     }
     
     public struct Edge {

         public MapPoint v1, v2;
         public Edge(MapPoint v1, MapPoint v2)
         {
             this.v1 = v1;
             this.v2 = v2;
         }

         public override bool Equals(object obj)
         {
             if (!(obj is Edge edge)) return false;
             return Object.Equals(v1,edge.v1) && Object.Equals(v2,edge.v2)
                    || Object.Equals(v1, edge.v2) && Object.Equals(v2, edge.v1);
         }
         
         public bool nonOrientedEquals(Edge edge) {
             return Object.Equals(v1,edge.v1) && Object.Equals(v2,edge.v2)
                    || Object.Equals(v1, edge.v2) && Object.Equals(v2, edge.v1);
         }

         public override int GetHashCode()
         {
             return HashCode.Combine(v1, v2);
         }
     }


}
