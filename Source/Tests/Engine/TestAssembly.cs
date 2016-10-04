using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using AGS.Engine.Desktop;
using System.Threading.Tasks;
using Moq;
using System.IO;

namespace Tests
{
	[SetUpFixture]
	public class TestAssembly
	{
		[SetUp]
		public void Init()
		{
			AGSEngineDesktop.Init();

            Mock<IBitmapLoader> bitmapLoader = new Mock<IBitmapLoader>();
            bitmapLoader.Setup(loader => loader.Load(It.IsAny<int>(), It.IsAny<int>())).Returns(new Mock<IBitmap>().Object);
            Hooks.BitmapLoader = bitmapLoader.Object;
		}

		[TearDown]
		public void TearDown()
		{
		}
	}
}

