using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public class Bezier
{
    public List<Vector2> Points { get; }

    // Constructor público: acepta de 2 a 4 puntos
    public Bezier(params Vector2[] points)
    {
        if (points.Length < 2 || points.Length > 4)
            throw new ArgumentException("La curva Bezier debe tener entre 2 y 4 puntos.");

        Points = new List<Vector2>(points);
    }

    // Constructor privado: permite curvas degeneradas con 1 punto (para uso interno)
    private Bezier(Vector2 point)
    {
        Points = new List<Vector2> { point };
    }

    // Propiedades
    public int Degree => Points.Count - 1;
    public Vector2 StartPoint => Points.First();
    public Vector2 EndPoint => Points.Last();
    public IEnumerable<Vector2> Knots => Points.Skip(1).SkipLast(1);
    public Vector2 StartTangent => TangentAt(0);
    public Vector2 EndTangent => TangentAt(1);
    public List<Vector2> ApproximatingPolygon => Points.ToList();
    public float Length => EstimateLength(100);
    public (Vector2 Min, Vector2 Max) BoundingBox => CalculateBoundingBox();

    // Métodos públicos
    public Vector2 PointAt(float t) => DeCasteljau(Points, t);

    public (Vector2 Tangent, Vector2 Normal) TangentAndNormalAt(float t)
    {
        Vector2 tangent = TangentAt(t);
        Vector2 normal = new Vector2(-tangent.Y, tangent.X);
        return (Vector2.Normalize(tangent), Vector2.Normalize(normal));
    }

    public float ArcLengthParameter(float t) => EstimateArcLengthParameter(t, 100);

    public float TimeParameter(float arcLength) => FindTimeForArcLength(arcLength, 100);

    public Vector2 PointAtArcLength(float arcLength) => PointAt(TimeParameter(arcLength));

    public (Vector2 Tangent, Vector2 Normal) TangentAndNormalAtArcLength(float arcLength)
    {
        float t = TimeParameter(arcLength);
        return TangentAndNormalAt(t);
    }

    public Bezier ApplyLinearTransformation(Matrix3x2 transform)
    {
        var transformedPoints = Points.Select(p => Vector2.Transform(p, transform)).ToArray();
        return new Bezier(transformedPoints);
    }

    public (Bezier, Bezier) Split(float t)
    {
        var left = new List<Vector2>();
        var right = new List<Vector2>(Points);

        while (right.Count > 1)
        {
            left.Add(right.First());
            right = right.Zip(right.Skip(1), (a, b) => Vector2.Lerp(a, b, t)).ToList();
        }
        left.Add(right.First());

        return (
            left.Count > 1 ? new Bezier(left.ToArray()) : new Bezier(left.First()),
            right.Count > 1 ? new Bezier(right.ToArray()) : new Bezier(right.First())
        );
    }

    public Bezier Clip(float t0, float t1)
    {
        var (left, _) = Split(t0);
        var (clipped, _) = left.Split(t1 / (1 - t0));
        return clipped;
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        float closestT = Enumerable.Range(0, 100)
            .Select(i => i / 99f)
            .OrderBy(t => Vector2.Distance(PointAt(t), point))
            .First();
        return PointAt(closestT);
    }

    public float DistanceTo(Vector2 point) => Vector2.Distance(ClosestPoint(point), point);

    public void Accept(PathBuilder builder)
    {
        builder.MoveTo(StartPoint);
        switch (Degree)
        {
            case 1:
                builder.LineTo(EndPoint);
                break;
            case 2:
                builder.QuadTo(Points[1], EndPoint);
                break;
            case 3:
                builder.CubicTo(Points[1], Points[2], EndPoint);
                break;
        }
    }

    // Métodos auxiliares
    private Vector2 DeCasteljau(List<Vector2> points, float t)
    {
        while (points.Count > 1)
            points = points.Zip(points.Skip(1), (a, b) => Vector2.Lerp(a, b, t)).ToList();
        return points.First();
    }

    private Vector2 TangentAt(float t)
    {
        List<Vector2> derivatives = Points.Zip(Points.Skip(1), (a, b) => b - a).ToList();
        return DeCasteljau(derivatives, t) * Degree;
    }

    private (Vector2 Min, Vector2 Max) CalculateBoundingBox()
    {
        float minX = Points.Min(p => p.X);
        float minY = Points.Min(p => p.Y);
        float maxX = Points.Max(p => p.X);
        float maxY = Points.Max(p => p.Y);
        return (new Vector2(minX, minY), new Vector2(maxX, maxY));
    }

    private float EstimateLength(int steps)
    {
        float length = 0;
        Vector2 prev = PointAt(0);
        for (int i = 1; i <= steps; i++)
        {
            Vector2 current = PointAt(i / (float)steps);
            length += Vector2.Distance(prev, current);
            prev = current;
        }
        return length;
    }

    private float EstimateArcLengthParameter(float t, int steps)
    {
        float length = 0;
        Vector2 prev = PointAt(0);
        for (int i = 1; i <= steps; i++)
        {
            float u = i / (float)steps;
            Vector2 current = PointAt(u);
            length += Vector2.Distance(prev, current);
            if (u >= t) break;
            prev = current;
        }
        return length;
    }

    private float FindTimeForArcLength(float targetLength, int steps)
    {
        float length = 0;
        Vector2 prev = PointAt(0);
        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 current = PointAt(t);
            length += Vector2.Distance(prev, current);
            if (length >= targetLength) return t;
            prev = current;
        }
        return 1;
    }
}
