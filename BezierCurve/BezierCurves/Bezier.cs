using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

public static class Vector2Extensions
{
    public static Vector2 Normalized(this Vector2 vector)
    {
        float length = vector.Length();
        return length > 0 ? vector / length : Vector2.Zero;
    }
}

public class Bezier
{
    public List<Vector2> Points { get; }

    public Bezier(params Vector2[] points)
    {
        if (points.Length < 2 || points.Length > 4)
            throw new ArgumentException("Bezier curve must have 2 to 4 points.");

        Points = new List<Vector2>(points);
    }

    public int Degree => Points.Count - 1;
    public Vector2 StartPoint => Points.First();
    public Vector2 EndPoint => Points.Last();
    public IEnumerable<Vector2> Knots => Points.Skip(1).SkipLast(1);
    public float Length => EstimateLength(200);
    public Vector2 StartTangent => GetTangentAt(0);
    public Vector2 EndTangent => GetTangentAt(1);

    public Vector2 PointAt(float t) => DeCasteljau(Points, t);

    public float ArcLengthParameter(float t) => EstimateArcLengthParameter(t, 200);

    public float TimeParameter(float arcLength)
    {
        float totalLength = EstimateLength(200); // Calculate total length of the curve
        if (totalLength == 0) return 0; // Avoid division by zero
        return FindTimeForArcLength(arcLength, 200); // Scale arc length
    }

    public (Bezier left, Bezier right) Split(float t)
    {
        var leftPoints = new List<Vector2> { Points[0] };
        var rightPoints = new List<Vector2> { Points[^1] };
        var temp = Points.ToList();

        while (temp.Count > 1)
        {
            var next = new List<Vector2>();
            for (int i = 0; i < temp.Count - 1; i++)
            {
                var interpolated = Vector2.Lerp(temp[i], temp[i + 1], t);
                next.Add(interpolated);
            }
            leftPoints.Add(next.First());
            rightPoints.Insert(0, next.Last());
            temp = next;
        }

        return (new Bezier(leftPoints.ToArray()), new Bezier(rightPoints.ToArray()));
    }

    public Bezier Clip(float t0, float t1)
    {
        if (t0 < 0 || t1 > 1 || t0 >= t1)
            throw new ArgumentOutOfRangeException("Invalid parameters for clipping.");

        var (leftPart, rightPart) = Split(t0);
        float t1Adjusted = (t1 - t0) / (1 - t0); // Adjust for the range

        var (clipped, _) = rightPart.Split(t1Adjusted);

        if (clipped.Points.Count < 2)
            throw new InvalidOperationException("Invalid clip resulting in fewer than 2 points.");

        return clipped;
    }

    public float EstimateArcLengthParameter(float t, int steps)
    {
        float length = 0;
        Vector2 prev = PointAt(0);
        for (int i = 1; i <= steps; i++)
        {
            float u = i / (float)steps;
            Vector2 current = PointAt(u);
            length += Vector2.Distance(prev, current);
            prev = current;
        }
        return length; // Return total length
    }

    public float FindTimeForArcLength(float targetLength, int steps)
    {
        float totalLength = 0;
        Vector2 prev = PointAt(0);
        List<float> lengths = new List<float>();

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 current = PointAt(t);
            float segmentLength = Vector2.Distance(prev, current);
            totalLength += segmentLength;
            lengths.Add(totalLength);
            prev = current;
        }

        if (totalLength < targetLength) return 1; // If it does not reach the desired length

        float low = 0, high = 1;
        while (high - low > 0.00001f) // Desired precision
        {
            float mid = (low + high) / 2;
            float midLength = EstimateArcLengthParameter(mid, steps);
            if (midLength < targetLength)
                low = mid; // Search in the upper half
            else
                high = mid; // Search in the lower half
        }
        return high;
    }

    private Vector2 DeCasteljau(List<Vector2> points, float t)
    {
        while (points.Count > 1)
            points = points.Zip(points.Skip(1), (a, b) => Vector2.Lerp(a, b, t)).ToList();
        return points.First();
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
        return length; // Return total length without normalization
    }

    private Vector2 GetTangentAt(float t)
    {
        if (Points.Count == 2) // Linear
        {
            return (Points[1] - Points[0]).Normalized();
        }
        else if (Points.Count == 3) // Quadratic
        {
            return Vector2.Lerp(Points[1] - Points[0], Points[2] - Points[1], t).Normalized();
        }
        else // Cubic
        {
            return Vector2.Lerp(Vector2.Lerp(Points[1] - Points[0], Points[2] - Points[1], t),
                                Points[3] - Points[2], t).Normalized();
        }
    }

    public IEnumerable<Vector2> ApproximationPolygon(int segments)
    {
        for (int i = 0; i <= segments; i++)
        {
            yield return PointAt(i / (float)segments);
        }
    }

    public RectangleF GetBoundingBox()
    {
        var minX = Points.Min(p => p.X);
        var maxX = Points.Max(p => p.X);
        var minY = Points.Min(p => p.Y);
        var maxY = Points.Max(p => p.Y);

        return new RectangleF(minX, minY, maxX - minX, maxY - minY);
    }

    public Vector2 ClosestPoint(Vector2 point)
    {
        float closestDist = float.MaxValue;
        Vector2 closestPoint = Vector2.Zero;

        // Use approximation of segments
        int segments = 100;
        for (int i = 0; i < segments; i++)
        {
            Vector2 p = PointAt(i / (float)segments);
            float dist = Vector2.Distance(point, p);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPoint = p;
            }
        }

        return closestPoint;
    }

    public float DistanceToPoint(Vector2 point)
    {
        Vector2 closestPoint = ClosestPoint(point);
        return Vector2.Distance(point, closestPoint);
    }

    public Vector2 NormalAt(float t)
    {
        Vector2 tangent = GetTangentAt(t);
        return new Vector2(-tangent.Y, tangent.X); // Perpendicular to the tangent
    }

    public Vector2 PointAtArcLength(float arcLength)
    {
        return PointAt(TimeParameter(arcLength)); // Use the existing TimeParameter method
    }

    public Vector2 TangentAtArcLength(float arcLength)
    {
        float timeParam = TimeParameter(arcLength);
        return GetTangentAt(timeParam);
    }

    public void ApplyTransformation(Matrix4x4 transformationMatrix)
    {
        for (int i = 0; i < Points.Count; i++)
        {
            Points[i] = Vector2.Transform(Points[i], transformationMatrix);
        }
    }
}
