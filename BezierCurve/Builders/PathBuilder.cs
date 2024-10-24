using System.Numerics;

public abstract class PathBuilder
{
    public abstract void MoveTo(Vector2 p0);
    public abstract void LineTo(Vector2 p1);
    public abstract void QuadTo(Vector2 k1, Vector2 p2);
    public abstract void CubicTo(Vector2 k1, Vector2 k2, Vector2 p3);
}
