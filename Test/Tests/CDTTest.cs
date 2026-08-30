using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Utility;

namespace Test.Tests
{
    public class CDTTest
    {
        [Test]
        public void compute2()
        {
            List<Point> points = new List<Point>();

            // Carré
            points.Add(new Point(0, 0));
            points.Add(new Point(100, 0));
            points.Add(new Point(100, 100));
            points.Add(new Point(0, 100));

            // Point central
            points.Add(new Point(10, 10));
            points.Add(new Point(15, 10));
            points.Add(new Point(15, 15));
            points.Add(new Point(10, 15));
            var builder = CDT.compute(points);

            foreach (Face f in
            builder.faces.Keys) {
                Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
            }


            List<Segment<Vertex>> contrainst = new List<Segment<Vertex>>();
            contrainst.Add(new Segment<Vertex>(new Vertex(10, 10), new Vertex(15, 10)));
            contrainst.Add(new Segment<Vertex>(new Vertex(15, 10), new Vertex(15, 15)));
            contrainst.Add(new Segment<Vertex>(new Vertex(15, 15), new Vertex(10, 15)));
            contrainst.Add(new Segment<Vertex>(new Vertex(10, 15), new Vertex(10, 10)));
            CDT.ConstraintPolygon constraintPolygon = new CDT.ConstraintPolygon();
            constraintPolygon.segments = contrainst;
            
            CDT.applyConstraints(new List<CDT.ConstraintPolygon>{constraintPolygon}, builder);
            CDT.removePolygons(builder, new List<CDT.ConstraintPolygon>{constraintPolygon});

            foreach (Face f in
            builder.faces.Keys) {
                Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
            }
        }
    

        [Test]
        public void computeNearPolygon()  {
            List<Point> points = new List<Point>();

            points.Add(new Point(17,15));
            points.Add(new Point(0, 0));
            points.Add(new Point(100, 0));
            points.Add(new Point(10,15));
            points.Add(new Point(10, 10));
            points.Add(new Point(25, 15));
            points.Add(new Point(100, 100));
            points.Add(new Point(25, 10));
            points.Add(new Point(0, 100));

            points.Add(new Point(20, 10));
            points.Add(new Point(20, 15));

            points.Add(new Point(17,10));
            var builder = CDT.compute(points);

            foreach(Face f in builder.faces.Keys) {
                Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
            }

            List<Segment<Vertex>> contrainst = new List<Segment<Vertex>>();
            contrainst.Add(new Segment<Vertex>(new Vertex(10,10),new Vertex(17,10)));
            contrainst.Add(new Segment<Vertex>(new Vertex(17,10),new Vertex(17,15)));
            contrainst.Add(new Segment<Vertex>(new Vertex(17,15),new Vertex(10,15)));
            contrainst.Add(new Segment<Vertex>(new Vertex(10,15),new Vertex(10,10)));

            CDT.ConstraintPolygon constraintPolygon = new CDT.ConstraintPolygon();
            constraintPolygon.segments = contrainst;

            List<Segment<Vertex>> contrainst2 = new List<Segment<Vertex>>();
            contrainst2.Add(new Segment<Vertex>(new Vertex(20,10),new Vertex(25,10)));
            contrainst2.Add(new Segment<Vertex>(new Vertex(25,10),new Vertex(25,15)));
            contrainst2.Add(new Segment<Vertex>(new Vertex(25,15),new Vertex(20,15)));
            contrainst2.Add(new Segment<Vertex>(new Vertex(20,15),new Vertex(20,10)));

            CDT.ConstraintPolygon constraintPolygon2 = new CDT.ConstraintPolygon();
            constraintPolygon2.segments = contrainst2;

            CDT.applyConstraints(new List<CDT.ConstraintPolygon>{constraintPolygon,constraintPolygon2},builder);

            CDT.removePolygons(builder,new List<CDT.ConstraintPolygon>{constraintPolygon,constraintPolygon2});

            foreach(Face f in builder.faces.Keys) {
                Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
            }
        }
        
        [Test]
        public void intersectCDT()
        {
            List<Point> points = new List<Point>();

            points.Add(new Point(0, 0));
            points.Add(new Point(100, 0));
            points.Add(new Point(100, 100));
            points.Add(new Point(0, 100));

            // Point central
            points.Add(new Point(10, 10));
            points.Add(new Point(17, 10));
            points.Add(new Point(17, 15));
            points.Add(new Point(10, 15));
            points.Add(new Point(13, 12));
            points.Add(new Point(13, 8));

            var builder = CDT.compute(points);

            foreach(Face f in builder.faces.Keys) {
                Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
            }

            List<Segment<Vertex>> contrainst = new List<Segment<Vertex>>();
            contrainst.Add(new Segment<Vertex>(new Vertex(10,10),new Vertex(17,10)));
            contrainst.Add(new Segment<Vertex>(new Vertex(17,10),new Vertex(17,15)));
            contrainst.Add(new Segment<Vertex>(new Vertex(17,15),new Vertex(10,15)));
            contrainst.Add(new Segment<Vertex>(new Vertex(10,15),new Vertex(10,10)));

            CDT.ConstraintPolygon constraintPolygon = new CDT.ConstraintPolygon();
            constraintPolygon.segments = contrainst;
            CDT.applyConstraints(new List<CDT.ConstraintPolygon>{constraintPolygon},builder);
            CDT.removePolygons(builder, new List<CDT.ConstraintPolygon>{constraintPolygon});

            foreach(Face f in builder.faces.Keys) {
                if (builder.faces.ContainsKey(f))
                {
                    Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
                    
                }
                
            }
        }
    

    }
}