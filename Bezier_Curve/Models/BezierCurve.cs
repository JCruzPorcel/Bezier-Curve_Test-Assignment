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
            float x = 3 * (1 - t) * (1 - t) * (controlPoints[1].X - controlPoints[0].X) +
                      6 * (1 - t) * t * (controlPoints[2].X - controlPoints[1].X) +
                      3 * t * t * (controlPoints[3].X - controlPoints[2].X);

            float y = 3 * (1 - t) * (1 - t) * (controlPoints[1].Y - controlPoints[0].Y) +
                      6 * (1 - t) * t * (controlPoints[2].Y - controlPoints[1].Y) +
                      3 * t * t * (controlPoints[3].Y - controlPoints[2].Y);

            // Verificamos la magnitud del vector tangente
            float magnitude = (float)Math.Sqrt(x * x + y * y);
            if (Math.Abs(magnitude) < 1e-6)
            {
                throw new InvalidOperationException("Tangente es cero; verifica los puntos de control.");
            }

            // Normalizamos el vector tangente
            x /= magnitude;
            y /= magnitude;

            return new Point(x, y);
        }

        public Point EvaluateNormal(float t)
        {
            Point tangent = EvaluateTangent(t);
            // La normal es perpendicular a la tangente
            return new Point(-tangent.Y, tangent.X);
        }

        public float Length()
        {
            float length = 0;
            const int segments = 500; // Aumenta el número de segmentos para mayor precisión
            Point previousPoint = Evaluate(0.0f);

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
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
            var left = new List<Point> { controlPoints[0] };
            var right = new List<Point> { controlPoints[^1] };

            var temp = controlPoints.ToList();

            while (temp.Count > 1)
            {
                var newTemp = new List<Point>();
                for (int i = 0; i < temp.Count - 1; i++)
                {
                    float x = (1 - t) * temp[i].X + t * temp[i + 1].X;
                    float y = (1 - t) * temp[i].Y + t * temp[i + 1].Y;
                    newTemp.Add(new Point(x, y));
                }

                left.Add(newTemp[0]);
                right.Insert(0, newTemp[^1]);
                temp = newTemp;
            }

            return (new BezierCurve(left.ToArray()), new BezierCurve(right.ToArray()));
        }

        public Point ClosestPoint(Point target)
        {
            Point closestPoint = Evaluate(0.0f);
            float minDistance = Distance(target, closestPoint);

            const int segments = 500; // Aumenta para mejor precisión
            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float)segments;
                Point currentPoint = Evaluate(t);
                float currentDistance = Distance(target, currentPoint);

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestPoint = currentPoint;
                }
            }

            return closestPoint;
        }

        public float DistanceTo(Point target)
        {
            Point closestPoint = ClosestPoint(target);
            return Distance(target, closestPoint);
        }
    }
}
