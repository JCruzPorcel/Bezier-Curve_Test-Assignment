using System;
using System.Collections.Generic;

namespace Bezier_Curve.Models
{
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
                float coeff = BinomialCoefficient(n, i) * (float)Math.Pow(1 - t, n - i) * (float)Math.Pow(t, i);
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

        // Método adicional para evaluar el vector tangente en un parámetro dado
        public Point EvaluateTangent(float t)
        {
            // Evaluar la derivada de la curva en el punto t
            int n = Degree;
            Point result = new Point(0, 0);

            for (int i = 0; i < n; i++)
            {
                float coeff = n * BinomialCoefficient(n - 1, i) * (float)Math.Pow(1 - t, n - 1 - i) * (float)Math.Pow(t, i);
                result.X += coeff * (controlPoints[i + 1].X - controlPoints[i].X);
                result.Y += coeff * (controlPoints[i + 1].Y - controlPoints[i].Y);
            }

            return result;
        }

        // Métodos adicionales y lógica para los otros requerimientos se pueden añadir aquí
    }
}
