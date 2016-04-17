using System;
using NUnit.Framework;
using AGS.Engine;
using AGS.API;
using AGS.Engine.Desktop;
using System.Threading.Tasks;

namespace Tests
{
	[SetUpFixture]
	public class TestAssembly
	{
		[SetUp]
		public void Init()
		{
			AGSEngineDesktop.Init();
		}

		[TearDown]
		public void TearDown()
		{
		}
	}
}

