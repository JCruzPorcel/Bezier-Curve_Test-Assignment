using System.Numerics;
using NUnit.Framework;

[TestFixture]
public class BezierTests
{
    [Test]
    public void TestLinearBezier()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1));
        NUnit.Framework.Assert.AreEqual(1, bezier.Degree);
        NUnit.Framework.Assert.AreEqual(new Vector2(0, 0), bezier.StartPoint);
        NUnit.Framework.Assert.AreEqual(new Vector2(1, 1), bezier.EndPoint);
    }

    [Test]
    public void TestQuadraticBezier()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 2), new Vector2(2, 0));
        NUnit.Framework.Assert.AreEqual(2, bezier.Degree);
    }

    [Test]
    public void TestCubicBezier()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 2), new Vector2(2, 2), new Vector2(3, 0));
        NUnit.Framework.Assert.AreEqual(3, bezier.Degree);
    }

    [Test]
    public void TestPointAt()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1));
        NUnit.Framework.Assert.AreEqual(new Vector2(0.5f, 0.5f), bezier.PointAt(0.5f));
    }

    [Test]
    public void TestSplitBezier()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1));
        var (left, right) = bezier.Split(0.5f);
        NUnit.Framework.Assert.AreEqual(new Vector2(0.5f, 0.5f), left.EndPoint);
        NUnit.Framework.Assert.AreEqual(new Vector2(0.5f, 0.5f), right.StartPoint);
    }
}
