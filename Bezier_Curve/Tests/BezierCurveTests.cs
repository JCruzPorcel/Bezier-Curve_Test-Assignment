using Bezier_Curve.Models;

namespace Bezier_Curve.Tests
{
    public class BezierCurveTests
    {
        public static void RunTests() // Cambiar a static
        {
            TestEvaluate();
            // Agrega más pruebas aquí si es necesario
        }

        public static void TestEvaluate() // Cambiar a static
        {
            var p0 = new Point(0, 0);
            var p1 = new Point(1, 1);
            var p2 = new Point(2, 1);
            var p3 = new Point(3, 0);

            var controlPoints = new List<Point> { p0, p1, p2, p3 }.ToArray();
            var curve = new BezierCurve(controlPoints);

            Point result = curve.Evaluate(0.5f);
            Point expected = new Point(1.5f, 0.75f);

            AssertEqual(result, expected);
        }

        private static void AssertEqual(Point actual, Point expected) // Cambiar a static
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
    }
}
