using System.Collections.Generic;
using NUnit.Framework;
using Utility;
namespace Test.Tests
{
    public class TestBowyerWatson
    {
        [Test]
        public void compute() {
            List<Point> points = new List<Point>();
            points.Add(new Point(0,0));
            points.Add(new Point(10,0));
            points.Add(new Point(5,10));
            BoywerWatson.compute(points);
        }
        [Test]
        public void computeComplex() {
            List<Point> points = new List<Point>();

            // Enveloppe convexe
            points.Add(new Point(0, 0));
            points.Add(new Point(20, 0));
            points.Add(new Point(25, 10));
            points.Add(new Point(15, 25));
            points.Add(new Point(0, 20));

            // Points intérieurs
            points.Add(new Point(8, 6));
            points.Add(new Point(12, 8));
            points.Add(new Point(10, 15));
            points.Add(new Point(6, 12));

            // Points presque cocycliques (tests numériques)
            points.Add(new Point(13, 13));
            points.Add(new Point(14, 12));
            points.Add(new Point(12, 14));
            
            
            var result = BoywerWatson.compute(points);
            //result.showGraph();
            //new CountDownLatch(10).await();
        }


    }
}