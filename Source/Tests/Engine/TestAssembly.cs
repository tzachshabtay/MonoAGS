using NUnit.Framework;
using AGS.Engine.Desktop;

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

