using NUnit.Framework;
using System.Numerics;

// Summary:
// To install the NUnit framework, open the Developer PowerShell for Visual Studio
// and run the following command:
// Install-Package NUnit -Version 3.13.3
//
// Make sure to add the dependency to the BezierTests project. You can do this by right-clicking on 
// the project in Solution Explorer, selecting "Manage NuGet Packages", searching for "NUnit", 
// and installing version 3.13.3.
//
// The following dependencies are also used in the project:
// <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
// <PackageReference Include="MSTest.TestAdapter" Version="3.6.1" />
// <PackageReference Include="MSTest.TestFramework" Version="3.6.1" />
//
// Set the BezierCurve project as the startup project to run your tests effectively.


namespace BezierCurveTests
{
    [TestFixture]
    public class BezierTests
    {
        [Test]
        public void TestClip_DifferentPoints()
        {
            var bezier = new Bezier(
                new Vector2(2, 2),
                new Vector2(5, 10),
                new Vector2(8, 4),
                new Vector2(12, 2)
            );

            var clippedBezier = bezier.Clip(0.3f, 0.7f);

            Console.WriteLine($"Clipped Points: {string.Join(", ", clippedBezier.Points)}");

            NUnit.Framework.Assert.AreEqual(4, clippedBezier.Points.Count, "Clipped points count mismatch.");
        }

        [Test]
        public void TestNormalAt_WithDifferentCurve()
        {
            var bezier = new Bezier(new Vector2(3, 2), new Vector2(6, 6));

            var normal = bezier.NormalAt(0.25f);
            Console.WriteLine($"Normal: {normal}");

            NUnit.Framework.Assert.AreEqual(1, normal.Length(), 0.001, "Normal is not normalized.");
        }

        [Test]
        public void TestApplyTransformation_WithTranslation()
        {
            var bezier = new Bezier(new Vector2(3, 3), new Vector2(6, 6));
            var matrix = Matrix4x4.CreateTranslation(-2, 2, 0);

            bezier.ApplyTransformation(matrix);

            Console.WriteLine($"Transformed Points: {string.Join(", ", bezier.Points)}");
            NUnit.Framework.Assert.AreEqual(new Vector2(1, 5), bezier.Points[0], "Point not transformed correctly.");
        }

        [Test]
        public void TestLength_LongCurve()
        {
            var bezier = new Bezier(
                new Vector2(1, 1),
                new Vector2(12, 25),
                new Vector2(30, 1)
            );

            var length = bezier.Length;
            Console.WriteLine($"Length: {length}");

            NUnit.Framework.Assert.Greater(length, 35, "Length should be greater than 35.");
        }

        [Test]
        public void TestBoundingBox_LargeRange()
        {
            var bezier = new Bezier(
                new Vector2(-15, -15),
                new Vector2(-5, 20),
                new Vector2(15, -15)
            );

            var (min, max) = bezier.BoundingBox;
            Console.WriteLine($"BoundingBox: Min={min}, Max={max}");

            NUnit.Framework.Assert.AreEqual(new Vector2(-15, -15), min, "Incorrect bounding box min.");
            NUnit.Framework.Assert.AreEqual(new Vector2(15, 20), max, "Incorrect bounding box max.");
        }

        [Test]
        public void TestClosestPoint_ToCurve()
        {
            var bezier = new Bezier(
                new Vector2(2, 2),
                new Vector2(4, 6),
                new Vector2(8, 3)
            );

            var point = new Vector2(5, 4);
            var closestPoint = bezier.ClosestPoint(point);

            Console.WriteLine($"Closest Point: {closestPoint}");

            NUnit.Framework.Assert.IsTrue(Vector2.Distance(point, closestPoint) < 1.0f,
                "Closest point calculation incorrect.");
        }

        [Test]
        public void TestDistanceTo_FarPoint()
        {
            var bezier = new Bezier(
                new Vector2(-5, -5),
                new Vector2(4, 8),
                new Vector2(8, -4)
            );

            var point = new Vector2(15, 15);
            var distance = bezier.DistanceTo(point);

            Console.WriteLine($"Distance to Point: {distance}");

            NUnit.Framework.Assert.Greater(distance, 8, "Distance should be greater than 8.");
            NUnit.Framework.Assert.LessOrEqual(distance, 18, "Distance is unexpectedly large.");
        }

        [Test]
        public void TestArcLengthParameter_AtMiddle()
        {
            var bezier = new Bezier(
                new Vector2(2, 2),
                new Vector2(5, 8),
                new Vector2(10, 2)
            );

            var arcLengthParameter = bezier.ArcLengthParameter(0.5f);

            Console.WriteLine($"Arc Length Parameter at t=0.5: {arcLengthParameter}");

            NUnit.Framework.Assert.Greater(arcLengthParameter, 0.3f, "Arc length parameter should be greater than 0.3.");
            NUnit.Framework.Assert.LessOrEqual(arcLengthParameter, 0.9f, "Arc length parameter should not exceed 0.9.");
        }

        [Test]
        public void TestTimeParameter_ForArcLength()
        {
            var bezier = new Bezier(
                new Vector2(1, 1),
                new Vector2(5, 6),
                new Vector2(9, 1)
            );

            float arcLength = bezier.Length / 4;
            var timeParameter = bezier.TimeParameter(arcLength);

            Console.WriteLine($"Time Parameter for one-fourth length: {timeParameter}");

            NUnit.Framework.Assert.GreaterOrEqual(timeParameter, 0, "Time parameter should not be negative.");
            NUnit.Framework.Assert.LessOrEqual(timeParameter, 1, "Time parameter should not exceed 1.");
        }

        [Test]
        public void TestPointAt_MidParameter()
        {
            var bezier = new Bezier(
                new Vector2(0, 0),
                new Vector2(5, 10),
                new Vector2(10, 0)
            );

            var midPoint = bezier.PointAt(0.5f);

            Console.WriteLine($"Mid Point at t=0.5: {midPoint}");

            NUnit.Framework.Assert.AreEqual(new Vector2(5, 5), midPoint, "Mid point calculation is incorrect.");
        }
    }
}
