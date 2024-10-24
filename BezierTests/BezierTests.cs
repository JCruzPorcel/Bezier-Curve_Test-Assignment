using NUnit.Framework;
using System.Numerics;

namespace BezierCurveTests
{
    [TestFixture]
    public class BezierTests
    {
        [Test]
        public void TestBezierCreation_ValidPoints()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1));
            NUnit.Framework.Assert.AreEqual(1, bezier.Degree);
            NUnit.Framework.Assert.AreEqual(new Vector2(0, 0), bezier.StartPoint);
            NUnit.Framework.Assert.AreEqual(new Vector2(1, 1), bezier.EndPoint);
        }

        [Test]
        public void TestBezierCreation_InvalidPoints()
        {
            NUnit.Framework.Assert.Throws<ArgumentException>(() => new Bezier(new Vector2(0, 0))); // Solo un punto
            NUnit.Framework.Assert.Throws<ArgumentException>(() => new Bezier(
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(2, 2),
                new Vector2(3, 3),
                new Vector2(4, 4))); // Demasiados puntos
        }

        [Test]
        public void TestPointAt()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            var pointAtHalf = bezier.PointAt(0.5f);
            NUnit.Framework.Assert.AreEqual(new Vector2(1, 0.5f), pointAtHalf); // Debe estar en el punto medio
        }

        [Test]
        public void TestTangentAt()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            var tangentAtHalf = bezier.StartTangent; // Evaluar la tangente en t=0
            NUnit.Framework.Assert.AreEqual(new Vector2(1, 1).Normalized(), tangentAtHalf); // El vector tangente en el inicio
        }

        [Test]
        public void TestNormalAt()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            var normalAtHalf = bezier.NormalAt(0.5f);
            NUnit.Framework.Assert.AreEqual(new Vector2(2, 2).Normalized(), normalAtHalf); // El vector normal en el punto medio
        }

        [Test]
        public void TestLength()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            float length = bezier.Length;
            NUnit.Framework.Assert.IsTrue(length > 0); // Debe ser positivo
        }

        [Test]
        public void TestClip()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            var clipped = bezier.Clip(0.25f, 0.75f);
            NUnit.Framework.Assert.AreEqual(2, clipped.Points.Count); // Debería haber dos puntos en la parte recortada
        }

        [Test]
        public void TestDistanceToPoint()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            float distance = bezier.DistanceToPoint(new Vector2(1, 0));
            NUnit.Framework.Assert.IsTrue(distance >= 0); // La distancia debe ser no negativa
        }

        [Test]
        public void TestApplyTransformation()
        {
            var bezier = new Bezier(new Vector2(0, 0), new Vector2(1, 1), new Vector2(2, 0));
            var transformationMatrix = Matrix4x4.CreateTranslation(1, 1, 0); // Trasladar
            bezier.ApplyTransformation(transformationMatrix);

            NUnit.Framework.Assert.AreEqual(new Vector2(1, 1), bezier.StartPoint);
            NUnit.Framework.Assert.AreEqual(new Vector2(2, 2), bezier.EndPoint);
        }
    }
}
