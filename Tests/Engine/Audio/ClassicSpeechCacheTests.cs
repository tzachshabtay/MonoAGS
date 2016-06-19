using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ClassicSpeechCacheTests
    {
        [TestCase("&1 Hello!", "Hello!")]
        [TestCase("&2Hello!", "Hello!")]
        [TestCase("&1 H", "H")]
        [TestCase("&2h", "h")]
        [TestCase("hello" , "hello")]
        [TestCase("hello&goodbye", "hello&goodbye")]
        public async Task GetSpeechLineAsync_ReturnsCorrectText_Test(string text, string expectedText)
        {
            Mock<IAudioFactory> factoryMock = new Mock<IAudioFactory>();
            AGSClassicSpeechCache cache = new AGSClassicSpeechCache(factoryMock.Object);

            var line = await cache.GetSpeechLineAsync("Character", text);

            Assert.AreEqual(expectedText, line.Text);
        }
    }
}

