using Bezier_Curve.Models;

namespace Bezier_Curve.Interfaces
{
    public interface IPathBuilder
    {
        void MoveTo(Point p0);
        void LineTo(Point p1);
        void QuadTo(Point k1, Point p2);
        void CubicTo(Point k1, Point k2, Point p3);
    }
}
