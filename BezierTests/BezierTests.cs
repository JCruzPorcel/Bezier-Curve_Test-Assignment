using NUnit.Framework;
using System.Numerics;

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

        NUnit.Framework.Assert.AreEqual(new Vector2(0, 0), left.StartPoint);
        NUnit.Framework.Assert.AreEqual(new Vector2(0.5f, 0.5f), left.EndPoint);
        NUnit.Framework.Assert.AreEqual(new Vector2(0.5f, 0.5f), right.StartPoint);
        NUnit.Framework.Assert.AreEqual(new Vector2(1, 1), right.EndPoint);
    }

    [Test]
    public void TestArcLengthParameter()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1));
        float arcLength = bezier.ArcLengthParameter(0.5f);
        NUnit.Framework.Assert.AreEqual(0.7071f, arcLength, 0.01f);
    }

    [Test]
    public void TestTimeParameter()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1));
        float arcLength = bezier.Length;
        float t = bezier.TimeParameter(arcLength);
        NUnit.Framework.Assert.AreEqual(1.0f, t, 0.01f);
    }

    [Test]
    public void TestClip()
    {
        var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 2), new Vector2(2, 0));
        var clipped = bezier.Clip(0.25f, 0.75f);

        NUnit.Framework.Assert.AreEqual(3, clipped.Points.Count, "Clipped segment must have 3 points.");
        NUnit.Framework.Assert.AreEqual(new Vector2(0.5f, 1.0f), clipped.StartPoint);
        NUnit.Framework.Assert.AreEqual(new Vector2(1.5f, 1.0f), clipped.EndPoint);
    }
}
