using System.Numerics;

public class Bezier
{
    public List<Vector2> Points { get; }

    // Constructor para aceptar puntos de control para cualquier grado (lineal, cuadrático o cúbico)
    public Bezier(params Vector2[] points)
    {
        if (points.Length < 2 || points.Length > 4)
            throw new ArgumentException("Bezier curve must have 2 to 4 points.");

        Points = new List<Vector2>(points);
    }

    // Propiedades
    public int Degree => Points.Count - 1; // El grado es el número de puntos menos 1
    public Vector2 StartPoint => Points.First();
    public Vector2 EndPoint => Points.Last();
    public IEnumerable<Vector2> Knots => Points.Skip(1).SkipLast(1);

    public Vector2 StartTangent => TangentAt(0); // Tangente en t=0
    public Vector2 EndTangent => TangentAt(1);   // Tangente en t=1

    public List<Vector2> ApproximatingPolygon => Points.ToList();

    public float Length => EstimateLength(100); // Aproximar la longitud con 100 pasos.

    public (Vector2 Min, Vector2 Max) BoundingBox => CalculateBoundingBox();

    // Métodos
    public Vector2 PointAt(float t) => DeCasteljau(Points, t);

    public (Vector2 Tangent, Vector2 Normal) TangentAndNormalAt(float t)
    {
        Vector2 tangent = TangentAt(t);
        Vector2 normal = new Vector2(-tangent.Y, tangent.X); // Vector perpendicular
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

    public (Bezier left, Bezier right) Split(float t)
    {
        var leftPoints = new List<Vector2> { Points[0] };
        var rightPoints = new List<Vector2> { Points[^1] };
        var temp = Points.ToList();

        while (temp.Count > 1)
        {
            var next = new List<Vector2>();
            for (int i = 0; i < temp.Count - 1; i++)
                next.Add(Vector2.Lerp(temp[i], temp[i + 1], t));

            leftPoints.Add(next.First());
            rightPoints.Insert(0, next.Last());
            temp = next;
        }

        return (new Bezier(leftPoints.ToArray()), new Bezier(rightPoints.ToArray()));
    }

    public Bezier Clip(float t0, float t1)
    {
        var (left, _) = Split(t0);
        var (clipped, _) = left.Split((t1 - t0) / (1 - t0));

        if (clipped.Points.Count < 2)
            throw new InvalidOperationException("Invalid clip resulting in fewer than 2 points.");

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

    // Método para obtener la longitud de arco en un parámetro de tiempo
    public float EstimateArcLengthParameter(float t, int steps)
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

    // Método para encontrar el parámetro de tiempo dado una longitud de arco
    public float FindTimeForArcLength(float targetLength, int steps)
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

    // Método para calcular la Caja de Alineación de Ejes
    private (Vector2 Min, Vector2 Max) CalculateBoundingBox()
    {
        float minX = Points.Min(p => p.X);
        float minY = Points.Min(p => p.Y);
        float maxX = Points.Max(p => p.X);
        float maxY = Points.Max(p => p.Y);
        return (new Vector2(minX, minY), new Vector2(maxX, maxY));
    }

    // Método para aproximar la longitud de la bezier
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

    // Método para evaluar el punto usando el algoritmo de De Casteljau
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

    // Método Visitor para PathBuilder
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
}
