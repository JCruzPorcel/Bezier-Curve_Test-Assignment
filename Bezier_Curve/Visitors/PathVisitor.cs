using Bezier_Curve.Interfaces;
using Bezier_Curve.Models;

namespace Bezier_Curve.Visitors
{
    public class PathVisitor
    {
        private readonly IPathBuilder builder;

        public PathVisitor(IPathBuilder builder)
        {
            this.builder = builder;
        }

        public void Visit(BezierCurve curve)
        {
            Point start = curve.Start;
            Point end = curve.End;

            builder.MoveTo(start);
            if (curve.Degree == 1)
            {
                builder.LineTo(end);
            }
            else if (curve.Degree == 2)
            {
                builder.QuadTo(curve.Knots[0], end);
            }
            else if (curve.Degree == 3)
            {
                builder.CubicTo(curve.Knots[0], curve.Knots[1], end);
            }
        }
    }
}
