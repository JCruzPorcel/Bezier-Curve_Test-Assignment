using NUnit.Framework;
using System.Numerics;

namespace BezierCurveTests
{
    [TestFixture]
    public class BezierTests
    {
        [Test]
        public void TestClip_DifferentPoints()
        {
            var bezier = new Bezier(
                new Vector2(1, 1),
                new Vector2(4, 8),
                new Vector2(7, 3),
                new Vector2(10, 1)
            );

            var clippedBezier = bezier.Clip(0.2f, 0.8f);

            Console.WriteLine($"Clipped Points: {string.Join(", ", clippedBezier.Points)}");

            NUnit.Framework.Assert.AreEqual(4, clippedBezier.Points.Count, "Clipped points count mismatch.");
        }

        [Test]
        public void TestNormalAt_WithDifferentCurve()
        {
            var bezier = new Bezier(new Vector2(2, 1), new Vector2(5, 5));

            var normal = bezier.NormalAt(0.5f);
            Console.WriteLine($"Normal: {normal}");

            NUnit.Framework.Assert.AreEqual(1, normal.Length(), 0.001, "Normal is not normalized.");
        }

        [Test]
        public void TestApplyTransformation_WithTranslation()
        {
            var bezier = new Bezier(new Vector2(2, 2), new Vector2(5, 5));
            var matrix = Matrix4x4.CreateTranslation(2, -1, 0);

            bezier.ApplyTransformation(matrix);

            Console.WriteLine($"Transformed Points: {string.Join(", ", bezier.Points)}");
            NUnit.Framework.Assert.AreEqual(new Vector2(4, 1), bezier.Points[0], "Point not transformed correctly.");
        }

        [Test]
        public void TestLength_LongCurve()
        {
            var bezier = new Bezier(
                new Vector2(1, 1),
                new Vector2(8, 16),  // Ajuste para longitud mayor
                new Vector2(15, 1)
            );

            var length = bezier.Length;
            Console.WriteLine($"Length: {length}");

            NUnit.Framework.Assert.Greater(length, 18, "Length should be greater than 18.");
        }

        [Test]
        public void TestBoundingBox_LargeRange()
        {
            var bezier = new Bezier(
                new Vector2(-10, -10),  // Inicio en (-10, -10)
                new Vector2(0, 15),     // Punto intermedio en (0, 15) para alcanzar el máximo Y
                new Vector2(12, -10)    // Final en (12, -10)
            );

            var (min, max) = bezier.BoundingBox;
            Console.WriteLine($"BoundingBox: Min={min}, Max={max}");

            NUnit.Framework.Assert.AreEqual(new Vector2(-10, -10), min, "Incorrect bounding box min.");
            NUnit.Framework.Assert.AreEqual(new Vector2(12, 15), max, "Incorrect bounding box max.");
        }

        [Test]
        public void TestClosestPoint_ToCurve()
        {
            var bezier = new Bezier(
                new Vector2(1, 1),
                new Vector2(3, 4),
                new Vector2(6, 2)
            );

            var point = new Vector2(4, 3);
            var closestPoint = bezier.ClosestPoint(point);

            Console.WriteLine($"Closest Point: {closestPoint}");

            NUnit.Framework.Assert.IsTrue(Vector2.Distance(point, closestPoint) < 1.0f,
                "Closest point calculation incorrect.");
        }

        [Test]
        public void TestDistanceTo_FarPoint()
        {
            var bezier = new Bezier(
                new Vector2(-1, -1),
                new Vector2(2, 5),
                new Vector2(5, -2)
            );

            var point = new Vector2(7, 7);
            var distance = bezier.DistanceTo(point);

            Console.WriteLine($"Distance to Point: {distance}");

            NUnit.Framework.Assert.Greater(distance, 3, "Distance should be greater than 3.");
            NUnit.Framework.Assert.LessOrEqual(distance, 8, "Distance is unexpectedly large.");
        }

        [Test]
        public void TestArcLengthParameter_AtMiddle()
        {
            var bezier = new Bezier(
                new Vector2(1, 1),
                new Vector2(4, 6),
                new Vector2(7, 1)
            );

            var arcLengthParameter = bezier.ArcLengthParameter(0.5f);

            Console.WriteLine($"Arc Length Parameter at t=0.5: {arcLengthParameter}");

            NUnit.Framework.Assert.Greater(arcLengthParameter, 0.2f, "Arc length parameter should be greater than 0.2.");
            NUnit.Framework.Assert.LessOrEqual(arcLengthParameter, 0.8f, "Arc length parameter should not exceed 0.8.");
        }

        [Test]
        public void TestTimeParameter_ForArcLength()
        {
            var bezier = new Bezier(
                new Vector2(0, 0),
                new Vector2(4, 5),
                new Vector2(8, 0)
            );

            float arcLength = bezier.Length / 3;
            var timeParameter = bezier.TimeParameter(arcLength);

            Console.WriteLine($"Time Parameter for one-third length: {timeParameter}");

            NUnit.Framework.Assert.GreaterOrEqual(timeParameter, 0, "Time parameter should not be negative.");
            NUnit.Framework.Assert.LessOrEqual(timeParameter, 1, "Time parameter should not exceed 1.");
        }
    }
}
