namespace Bezier_Curve.Models
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

    public class BezierCurve
    {
        private Point[] controlPoints;

        public BezierCurve(Point[] points)
        {
            if (points.Length < 2 || points.Length > 4)
                throw new ArgumentException("Bezier curve must have 2 to 4 control points.");
            controlPoints = points;
        }

        public int Degree => controlPoints.Length - 1;

        public Point Start => controlPoints[0];

        public Point End => controlPoints[^1];

        public Point[] Knots => controlPoints.Length > 2 ? controlPoints[1..^1] : Array.Empty<Point>();

        public Point Evaluate(float t)
        {
            int n = Degree;
            Point result = new Point(0, 0);

            for (int i = 0; i <= n; i++)
            {
                float coeff = BinomialCoefficient(n, i) * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, n - i);
                result.X += coeff * controlPoints[i].X;
                result.Y += coeff * controlPoints[i].Y;
            }

            return result;
        }

        private static int BinomialCoefficient(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0 || k == n) return 1;
            return Factorial(n) / (Factorial(k) * Factorial(n - k));
        }

        private static int Factorial(int n)
        {
            return n == 0 ? 1 : n * Factorial(n - 1);
        }

        public Point EvaluateTangent(float t)
        {
            int n = Degree;
            Point result = new Point(0, 0);

            for (int i = 0; i < n; i++)
            {
                float coeff = n * BinomialCoefficient(n - 1, i) * (float)Math.Pow(t, i) * (float)Math.Pow(1 - t, n - 1 - i);
                // Calculamos la diferencia entre el siguiente y el actual punto de control
                Point delta = new Point(controlPoints[i + 1].X - controlPoints[i].X, controlPoints[i + 1].Y - controlPoints[i].Y);
                result.X += coeff * delta.X;
                result.Y += coeff * delta.Y;
            }

            // Normalizamos el vector tangente
            float length = (float)Math.Sqrt(result.X * result.X + result.Y * result.Y);
            if (length > 0)
            {
                result.X /= length;
                result.Y /= length;
            }

            return result;
        }

        public Point EvaluateNormal(float t)
        {
            Point tangent = EvaluateTangent(t);
            float length = (float)Math.Sqrt(tangent.X * tangent.X + tangent.Y * tangent.Y);

            // Normalización del vector tangente
            if (length == 0) return new Point(0, 0);

            return new Point(-tangent.Y / length, tangent.X / length); // Perpendicular
        }

        public float Length(int steps = 100)
        {
            float length = 0;
            Point previousPoint = Evaluate(0);

            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / steps;
                Point currentPoint = Evaluate(t);
                length += Distance(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            return length;
        }

        private float Distance(Point p1, Point p2)
        {
            return (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
        }

        public (BezierCurve, BezierCurve) Split(float t)
        {
            var leftPoints = new Point[controlPoints.Length];
            var rightPoints = new Point[controlPoints.Length];

            // Calcular los puntos en la nueva curva izquierda y derecha
            for (int i = 0; i < controlPoints.Length; i++)
            {
                leftPoints[i] = Evaluate(i == 0 ? 0 : t);
                rightPoints[i] = Evaluate(t);
            }

            return (new BezierCurve(leftPoints), new BezierCurve(rightPoints));
        }

        public Point ClosestPoint(Point target)
        {
            Point closest = Evaluate(0);
            float minDistance = Distance(closest, target);

            for (int i = 1; i <= 100; i++)
            {
                float t = (float)i / 100;
                Point pointOnCurve = Evaluate(t);
                float distance = Distance(pointOnCurve, target);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = pointOnCurve;
                }
            }

            return closest;
        }

        public float DistanceTo(Point target)
        {
            Point closest = ClosestPoint(target);
            return Distance(closest, target);
        }
    }
}
