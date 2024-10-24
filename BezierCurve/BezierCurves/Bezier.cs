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

    // Properties
    public int Degree => Points.Count - 1;
    public Vector2 StartPoint => Points.First();
    public Vector2 EndPoint => Points.Last();
    public List<Vector2> Knots => Points.Skip(1).Take(Points.Count - 2).ToList();
    public Vector2 StartTangent => GetTangentAt(0);
    public Vector2 EndTangent => GetTangentAt(1);
    public float Length => EstimateLength(200);
    public (Vector2 Min, Vector2 Max) BoundingBox => GetAxisAlignedBoundingBox();
    public List<Vector2> ApproximatingPolygon => GetApproximatingPolygon(100);

    // Public Methods
    public Vector2 PointAt(float t) => DeCasteljau(Points, t);
    public Vector2 GetTangentAt(float t) => ComputeTangent(t);
    public Vector2 NormalAt(float t) => GetNormalAt(t);
    public (Bezier left, Bezier right) Split(float t) => SplitAt(t);
    public Bezier Clip(float t0, float t1) => ClipBezier(t0, t1);
    public Vector2 ClosestPoint(Vector2 point) => FindClosestPoint(point);
    public float DistanceTo(Vector2 point) => Vector2.Distance(point, ClosestPoint(point));

    public void ApplyTransformation(Matrix4x4 transformationMatrix)
    {
        for (int i = 0; i < Points.Count; i++)
        {
            Points[i] = Vector2.Transform(Points[i], transformationMatrix);
        }
    }

    public float ArcLengthParameter(float t) => EstimateArcLengthParameter(t, 100);
    public float TimeParameter(float arcLength) => FindTimeByArcLength(arcLength, 100);

    // New Methods
    public Vector2 PointAtArcLength(float arcLength) => PointAt(TimeParameter(arcLength));
    public Vector2 TangentAtArcLength(float arcLength) => GetTangentAt(TimeParameter(arcLength));
    public Vector2 NormalAtArcLength(float arcLength) => NormalAt(TimeParameter(arcLength));

    // Private Methods
    private Vector2 DeCasteljau(List<Vector2> points, float t)
    {
        while (points.Count > 1)
            points = points.Zip(points.Skip(1), (a, b) => Vector2.Lerp(a, b, t)).ToList();
        return points.First();
    }

    private Vector2 ComputeTangent(float t)
    {
        if (Points.Count == 2)
            return (Points[1] - Points[0]).Normalized();
        else if (Points.Count == 3)
            return Vector2.Lerp(Points[1] - Points[0], Points[2] - Points[1], t).Normalized();
        else
            return Vector2.Lerp(
                Vector2.Lerp(Points[1] - Points[0], Points[2] - Points[1], t),
                Points[3] - Points[2], t
            ).Normalized();
    }

    private Vector2 GetNormalAt(float t)
    {
        Vector2 tangent = GetTangentAt(t);
        return new Vector2(-tangent.Y, tangent.X).Normalized();
    }

    private (Bezier left, Bezier right) SplitAt(float t)
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

    private Bezier ClipBezier(float t0, float t1)
    {
        if (t0 < 0 || t1 > 1 || t0 >= t1)
            throw new ArgumentOutOfRangeException("Invalid parameters for clipping.");

        var (leftPart, rightPart) = Split(t0);
        var (clipped, _) = rightPart.Split((t1 - t0) / (1 - t0));
        return clipped;
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
        float totalLength = Length;
        float segmentLength = 0;
        Vector2 prev = PointAt(0);

        for (int i = 1; i <= steps; i++)
        {
            float currentT = i / (float)steps * t;
            Vector2 current = PointAt(currentT);
            segmentLength += Vector2.Distance(prev, current);
            prev = current;
        }
        return segmentLength / totalLength;
    }

    private float FindTimeByArcLength(float arcLength, int steps)
    {
        float targetLength = arcLength;
        float lengthSoFar = 0;
        Vector2 prev = PointAt(0);

        for (int i = 1; i <= steps; i++)
        {
            float t = i / (float)steps;
            Vector2 current = PointAt(t);
            lengthSoFar += Vector2.Distance(prev, current);

            if (lengthSoFar >= targetLength)
                return t;

            prev = current;
        }
        return 1;
    }

    private (Vector2 Min, Vector2 Max) GetAxisAlignedBoundingBox()
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (var point in Points)
        {
            minX = MathF.Min(minX, point.X);
            minY = MathF.Min(minY, point.Y);
            maxX = MathF.Max(maxX, point.X);
            maxY = MathF.Max(maxY, point.Y);
        }

        for (int i = 0; i <= 100; i++)
        {
            Vector2 currentPoint = PointAt(i / 100f);
            minX = MathF.Min(minX, currentPoint.X);
            minY = MathF.Min(minY, currentPoint.Y);
            maxX = MathF.Max(maxX, currentPoint.X);
            maxY = MathF.Max(maxY, currentPoint.Y);
        }

        return (new Vector2(minX, minY), new Vector2(maxX, maxY));
    }

    private List<Vector2> GetApproximatingPolygon(int steps)
    {
        var polygon = new List<Vector2>();
        for (int i = 0; i <= steps; i++)
        {
            polygon.Add(PointAt(i / (float)steps));
        }
        return polygon;
    }

    private Vector2 FindClosestPoint(Vector2 point)
    {
        float minDistance = float.MaxValue;
        Vector2 closestPoint = PointAt(0);

        for (int i = 0; i <= 100; i++)
        {
            Vector2 currentPoint = PointAt(i / 100f);
            float distance = Vector2.Distance(point, currentPoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = currentPoint;
            }
        }
        return closestPoint;
    }
}
