using System.Collections.Generic;
using NUnit.Framework;

public class TestSubBuilder
{


    [Test]
    public void createPolygon() {
        var builder = SubdivisionBuilder.builder();
        List<Vertex> p1 = new List<Vertex>();
        p1.Add(new Vertex(0,0));
        p1.Add(new Vertex(1,0));
        p1.Add(new Vertex(1,1));
        p1.Add(new Vertex(0,0));
        Face f=builder.buildPolygon(p1);
        Assert.AreEqual(6,builder.faces[f].Count);
        Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
        Assert.True(SubdivisionBuilder.outerEdgeBounded(builder.faces[f]));
        builder = SubdivisionBuilder.builder();
        p1.Clear();
        p1.Add(new Vertex(0,0));
        p1.Add(new Vertex(-1,0));
        p1.Add(new Vertex(-1,1));
        p1.Add(new Vertex(0,0));
        f=builder.buildPolygon(p1);
        Assert.AreEqual(6,builder.faces[f].Count);
        Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f]));
        Assert.True(SubdivisionBuilder.outerEdgeBounded(builder.faces[f]));

        List<Vertex> p2 = new List<Vertex>();
        p2.Add(new Vertex(-1,0));
        p2.Add(new Vertex(-2,1));
        p2.Add(new Vertex(-1,1));
        p2.Add(new Vertex(-1,0));

        var f2 = builder.buildPolygon(p2);
        Assert.AreEqual(6,builder.faces[f2].Count);
        Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f2]));
//        assertTrue(Subdivision.outerEdgeBounded(builder.faces.get(f)));

        p1.Clear();
        p1.Add(new Vertex(0,0));
        p1.Add(new Vertex(1,0));
        p1.Add(new Vertex(1,1));
        p1.Add(new Vertex(0,0));
        var f3=builder.buildPolygon(p1);
        Assert.AreEqual(6,builder.faces[f3].Count);
        Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f3]));
        Assert.AreEqual(3,builder.faces.Count);

        p1.Clear();
        p1.Add(new Vertex(0,2));
        p1.Add(new Vertex(1,2));
        p1.Add(new Vertex(1,3));
        p1.Add(new Vertex(0,2));
        var f4=builder.buildPolygon(p1);
        Assert.AreEqual(6,builder.faces[f4].Count);
        Assert.True(SubdivisionBuilder.inEdgeBounded(builder.faces[f4]));

        p1.Clear();
       /* p1.Add(new Vertex(0,0));
        p1.Add(new Vertex(0,2));
        p1.Add(new Vertex(1,1));
        p1.Add(new Vertex(0,0));
        var f5 = builder.buildPolygon(p1);
        Assertions.assertEquals(6,builder.faces.get(f3).size());
        assertTrue(Subdivision.inEdgeBounded(builder.faces.get(f4)));
        builder.buildVertex(new Vertex(0,0),new Vertex(0,2));
        builder.buildVertex(new Vertex(0,2),new Vertex(1,1));
        var triangles = builder.getTriangles();
        assertEquals(5,triangles.size());*/
    }

    [Test]
    public void isCWPolygon() {
        List<Vertex> polygon = new List<Vertex>();
        polygon.Add(new Vertex(0,0));
        polygon.Add(new Vertex(0,2));
        polygon.Add(new Vertex(1,1));
        polygon.Add(new Vertex(0,0));
        Assert.True(SubdivisionBuilder.isCWPolygon(polygon));

        polygon.Clear();
        polygon.Add(new Vertex(0,0));
        polygon.Add(new Vertex(-1,1));
        polygon.Add(new Vertex(3,3));
        polygon.Add(new Vertex(1,1));
        polygon.Add(new Vertex(0,0));
        Assert.True(SubdivisionBuilder.isCWPolygon(polygon));

        polygon.Clear();

        polygon.Add(new Vertex(0,0));
        polygon.Add(new Vertex(3,1));
        polygon.Add(new Vertex(1,2));
        polygon.Add(new Vertex(0,0));

        Assert.False(SubdivisionBuilder.isCWPolygon(polygon));
        polygon.Reverse();
        var reversedPolygon = polygon;
        Assert.True(SubdivisionBuilder.isCWPolygon(reversedPolygon));
    }

    
}
