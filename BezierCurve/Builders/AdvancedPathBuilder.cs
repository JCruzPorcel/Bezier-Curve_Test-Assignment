using System.Numerics;

public class AdvancedPathBuilder : PathBuilder
{
    public override void MoveTo(Vector2 p0) => Console.WriteLine($"MoveTo {p0}");
    public override void LineTo(Vector2 p1) => Console.WriteLine($"LineTo {p1}");
    public override void QuadTo(Vector2 k1, Vector2 p2) => Console.WriteLine($"QuadTo {k1}, {p2}");
    public override void CubicTo(Vector2 k1, Vector2 k2, Vector2 p3) => Console.WriteLine($"CubicTo {k1}, {k2}, {p3}");
}
