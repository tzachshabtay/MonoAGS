using NUnit.Framework;
using AGS.Engine;

namespace Tests.Engine.Misc
{
    [TestFixture]
    public class CustomPropertiesTests
    {
        private Mocks _mocks;
        private const string VAR_NAME = "SomeVar";

        [SetUp]
        public void Init()
        {
            _mocks = Mocks.Init();
        }

        [TearDown]
        public void Teardown()
        {
            _mocks.Dispose();
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(1)]
        [TestCase(999)]
        [TestCase(5678)]
        public void GetInt_Test(int defaultInt)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllInts().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultInt, obj.GetInt(VAR_NAME, defaultInt));
                Assert.IsTrue(obj.AllInts().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultInt, obj.GetInt(VAR_NAME));
            }
        }

        [TestCase(0,false)]
        [TestCase(-1,false)]
        [TestCase(1,false)]
        [TestCase(999,false)]
        [TestCase(5678,false)]
        [TestCase(0,true)]
        [TestCase(-1,true)]
        [TestCase(1,true)]
        [TestCase(999,true)]
        [TestCase(5678,true)]
        public void SetInt_GetInt_Test(int intToSet, bool getIntFirst)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllInts().ContainsKey(VAR_NAME));
                if (getIntFirst) Assert.AreEqual(999, obj.GetInt(VAR_NAME, 999));
                obj.SetInt(VAR_NAME, intToSet);
                Assert.IsTrue(obj.AllInts().ContainsKey(VAR_NAME));
                Assert.AreEqual(intToSet, obj.GetInt(VAR_NAME));
                Assert.AreEqual(intToSet, obj.GetInt(VAR_NAME, 999));
            }
        }

        [TestCase(0f)]
        [TestCase(-1f)]
        [TestCase(1f)]
        [TestCase(999f)]
        [TestCase(5678f)]
        public void GetFloat_Test(float defaultFloat)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllFloats().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultFloat, obj.GetFloat(VAR_NAME, defaultFloat));
                Assert.IsTrue(obj.AllFloats().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultFloat, obj.GetFloat(VAR_NAME));
            }
        }

        [TestCase(0f, false)]
        [TestCase(-1f, false)]
        [TestCase(1f, false)]
        [TestCase(999f, false)]
        [TestCase(5678f, false)]
        [TestCase(0f, true)]
        [TestCase(-1f, true)]
        [TestCase(1f, true)]
        [TestCase(999f, true)]
        [TestCase(5678f, true)]
        public void SetFloat_GetFloat_Test(float floatToSet, bool getFloatFirst)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllFloats().ContainsKey(VAR_NAME));
                if (getFloatFirst) Assert.AreEqual(999f, obj.GetFloat(VAR_NAME, 999));
                obj.SetFloat(VAR_NAME, floatToSet);
                Assert.IsTrue(obj.AllFloats().ContainsKey(VAR_NAME));
                Assert.AreEqual(floatToSet, obj.GetFloat(VAR_NAME));
                Assert.AreEqual(floatToSet, obj.GetFloat(VAR_NAME, 999f));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GetBool_Test(bool defaultBool)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllBooleans().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultBool, obj.GetBool(VAR_NAME, defaultBool));
                Assert.IsTrue(obj.AllBooleans().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultBool, obj.GetBool(VAR_NAME));
            }
        }

        [TestCase(false, false)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void SetBool_GetBool_Test(bool boolToSet, bool getBoolFirst)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllBooleans().ContainsKey(VAR_NAME));
                if (getBoolFirst) Assert.AreEqual(false, obj.GetBool(VAR_NAME, false));
                obj.SetBool(VAR_NAME, boolToSet);
                Assert.IsTrue(obj.AllBooleans().ContainsKey(VAR_NAME));
                Assert.AreEqual(boolToSet, obj.GetBool(VAR_NAME));
                Assert.AreEqual(boolToSet, obj.GetBool(VAR_NAME, false));
            }
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("abc")]
        [TestCase("SomeVar")]
        [TestCase("234")]
        [TestCase(" a %$# bert#@%$RDGSDF 45ijleitjgwer$")]
        public void GetString_Test(string defaultString)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllStrings().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultString, obj.GetString(VAR_NAME, defaultString));
                Assert.IsTrue(obj.AllStrings().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultString, obj.GetString(VAR_NAME));
            }
        }

        [TestCase(null, false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("abc", false)]
        [TestCase("SomeVar", false)]
        [TestCase("234", false)]
        [TestCase(" a %$# bert#@%$RDGSDF 45ijleitjgwer$", false)]
        [TestCase(null, true)]
        [TestCase("", true)]
        [TestCase(" ", true)]
        [TestCase("abc", true)]
        [TestCase("SomeVar", true)]
        [TestCase("234", true)]
        [TestCase(" a %$# bert#@%$RDGSDF 45ijleitjgwer$", true)]
        public void SetString_GetString_Test(string stringToSet, bool getStringFirst)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.AllStrings().ContainsKey(VAR_NAME));
                if (getStringFirst) Assert.AreEqual("", obj.GetString(VAR_NAME, ""));
                obj.SetString(VAR_NAME, stringToSet);
                Assert.IsTrue(obj.AllStrings().ContainsKey(VAR_NAME));
                Assert.AreEqual(stringToSet, obj.GetString(VAR_NAME));
                Assert.AreEqual(stringToSet, obj.GetString(VAR_NAME, ""));
            }
        }
    }
}
