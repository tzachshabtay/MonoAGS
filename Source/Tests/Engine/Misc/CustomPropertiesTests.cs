using NUnit.Framework;

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
                Assert.IsTrue(!obj.Properties.Ints.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultInt, obj.Properties.Ints.GetValue(VAR_NAME, defaultInt));
                Assert.IsTrue(obj.Properties.Ints.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultInt, obj.Properties.Ints.GetValue(VAR_NAME));
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
                Assert.IsTrue(!obj.Properties.Ints.AllProperties().ContainsKey(VAR_NAME));
                if (getIntFirst) Assert.AreEqual(999, obj.Properties.Ints.GetValue(VAR_NAME, 999));
                obj.Properties.Ints.SetValue(VAR_NAME, intToSet);
                Assert.IsTrue(obj.Properties.Ints.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(intToSet, obj.Properties.Ints.GetValue(VAR_NAME));
                Assert.AreEqual(intToSet, obj.Properties.Ints.GetValue(VAR_NAME, 999));
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
                Assert.IsTrue(!obj.Properties.Floats.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultFloat, obj.Properties.Floats.GetValue(VAR_NAME, defaultFloat));
                Assert.IsTrue(obj.Properties.Floats.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultFloat, obj.Properties.Floats.GetValue(VAR_NAME));
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
                Assert.IsTrue(!obj.Properties.Floats.AllProperties().ContainsKey(VAR_NAME));
                if (getFloatFirst) Assert.AreEqual(999f, obj.Properties.Floats.GetValue(VAR_NAME, 999));
                obj.Properties.Floats.SetValue(VAR_NAME, floatToSet);
                Assert.IsTrue(obj.Properties.Floats.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(floatToSet, obj.Properties.Floats.GetValue(VAR_NAME));
                Assert.AreEqual(floatToSet, obj.Properties.Floats.GetValue(VAR_NAME, 999f));
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void GetBool_Test(bool defaultBool)
        {
            foreach (var obj in ObjectTests.GetImplementors(_mocks, _mocks.GameState()))
            {
                Assert.IsTrue(!obj.Properties.Bools.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultBool, obj.Properties.Bools.GetValue(VAR_NAME, defaultBool));
                Assert.IsTrue(obj.Properties.Bools.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultBool, obj.Properties.Bools.GetValue(VAR_NAME));
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
                Assert.IsTrue(!obj.Properties.Bools.AllProperties().ContainsKey(VAR_NAME));
                if (getBoolFirst) Assert.AreEqual(false, obj.Properties.Bools.GetValue(VAR_NAME, false));
                obj.Properties.Bools.SetValue(VAR_NAME, boolToSet);
                Assert.IsTrue(obj.Properties.Bools.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(boolToSet, obj.Properties.Bools.GetValue(VAR_NAME));
                Assert.AreEqual(boolToSet, obj.Properties.Bools.GetValue(VAR_NAME, false));
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
                Assert.IsTrue(!obj.Properties.Strings.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultString, obj.Properties.Strings.GetValue(VAR_NAME, defaultString));
                Assert.IsTrue(obj.Properties.Strings.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(defaultString, obj.Properties.Strings.GetValue(VAR_NAME));
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
                Assert.IsTrue(!obj.Properties.Strings.AllProperties().ContainsKey(VAR_NAME));
                if (getStringFirst) Assert.AreEqual("", obj.Properties.Strings.GetValue(VAR_NAME, ""));
                obj.Properties.Strings.SetValue(VAR_NAME, stringToSet);
                Assert.IsTrue(obj.Properties.Strings.AllProperties().ContainsKey(VAR_NAME));
                Assert.AreEqual(stringToSet, obj.Properties.Strings.GetValue(VAR_NAME));
                Assert.AreEqual(stringToSet, obj.Properties.Strings.GetValue(VAR_NAME, ""));
            }
        }
    }
}
