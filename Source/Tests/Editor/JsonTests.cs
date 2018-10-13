using System;
using AGS.API;
using AGS.Editor;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class JsonTests
    {
        [Test]
        public void ColorJsonTest() => test(new FourCorners<Color>(default, Colors.AliceBlue, Colors.Black.WithAlpha(100), Color.FromArgb(100, 100, 100, 100)));

        [Test]
        public void PositionJsonTest() => test(new FourCorners<Position>(default, Position.Empty, new Position(200f, 100f, 350f), new Position(-105.4f, -120f, -120f)));

        [Test]
        public void PointFJsonTest() => test(new FourCorners<PointF>(default, PointF.Empty, new PointF(200f, 100f), new PointF(-105.4f, -120f)));

        [Test]
        public void PointJsonTest() => test(new FourCorners<Point>(default, Point.Empty, new Point(200, 100), new Point(-105, -120)));

        [Test]
        public void SizeFJsonTest() => test(new FourCorners<SizeF>(default, SizeF.Empty, new SizeF(200f, 100f), new SizeF(-105.4f, -120f)));

        [Test]
        public void SizeJsonTest() => test(new FourCorners<Size>(default, Size.Empty, new Size(200, 100), new Size(-105, -120)));

        private void test<TValue>(TValue expected)
        {
            var json = JsonConvert.SerializeObject(expected, Formatting.Indented, AGSProject.JsonSettings);
            var actual = JsonConvert.DeserializeObject<TValue>(json, AGSProject.JsonSettings);
            Assert.AreEqual(expected, actual);
        }
    }
}