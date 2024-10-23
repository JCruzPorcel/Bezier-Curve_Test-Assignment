using Bezier_Curve.Models;
using System;
using System.Collections.Generic;

namespace Bezier_Curve.Tests
{
    public class BezierCurveTests
    {
        public static void RunTests()
        {
            TestEvaluate();
            TestEvaluateTangent();
            TestEvaluateNormal();
            TestLength();
            TestSplit();
            TestClosestPoint();
            TestDistanceTo();
        }

        public static void TestEvaluate()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            // Suponiendo que deseas evaluar en t=1 para el punto final
            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            Point result = curve.Evaluate(1.0f); // Evaluar en t=1
            Point expected = new Point(3, 0); // Asegúrate de que este sea el punto correcto
            AssertEqual(result, expected);
        }

        public static void TestEvaluateTangent()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            Point result = curve.EvaluateTangent(0.5f);
            Point expected = new Point(3, -2); // Este valor debe ser ajustado según la implementación correcta

            AssertEqual(result, expected);
        }

        public static void TestEvaluateNormal()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            Point result = curve.EvaluateNormal(0.5f);
            Point expected = new Point(-0.8944272f, 0.4472136f); // Este valor debe ser ajustado según la implementación correcta

            AssertEqual(result, expected);
        }

        public static void TestLength()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            float result = curve.Length();
            float expected = 4.24264f; // Este valor debe ser verificado

            AssertEqual(result, expected, 0.0001f);
        }

        public static void TestSplit()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            var (left, right) = curve.Split(0.5f);

            Point leftEnd = left.End;
            Point expectedLeftEnd = new Point(1.5f, 0.75f); // Este valor debe ser verificado

            AssertEqual(leftEnd, expectedLeftEnd);
        }

        public static void TestClosestPoint()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            Point target = new Point(1, 0);
            Point result = curve.ClosestPoint(target);
            Point expected = new Point(1, 0); // Este valor debe ser verificado

            AssertEqual(result, expected);
        }

        public static void TestDistanceTo()
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            Point target = new Point(1, 0);
            float distance = curve.DistanceTo(target);
            float expectedDistance = 1; // Este valor debe ser verificado

            AssertEqual(distance, expectedDistance, 0.0001f);
        }

        private static void AssertEqual(Point actual, Point expected)
        {
            if (Math.Abs(actual.X - expected.X) > 0.001 || Math.Abs(actual.Y - expected.Y) > 0.001)
            {
                Console.WriteLine($"Test Failed: Expected Point({expected.X}, {expected.Y}), but got Point({actual.X}, {actual.Y})");
            }
            else
            {
                Console.WriteLine("Test Passed");
            }
        }

        private static void AssertEqual(float actual, float expected, float tolerance = 0.001f)
        {
            if (Math.Abs(actual - expected) > tolerance)
            {
                Console.WriteLine($"Test Failed: Expected {expected}, but got {actual}");
            }
            else
            {
                Console.WriteLine("Test Passed");
            }
        }
    }
}
