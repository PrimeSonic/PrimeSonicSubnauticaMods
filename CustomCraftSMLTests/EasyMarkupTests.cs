namespace CustomCraftSMLTests
{
    using System.Collections.Generic;
    using Common.EasyMarkup;
    using NUnit.Framework;
    using AssertionException = UnityEngine.Assertions.AssertionException;

    [TestFixture]
    public class EasyMarkupTests
    {

        private class TestProperty : EmProperty<int>
        {
            public TestProperty(string key) : base(key)
            {
            }

            public TestProperty(string key, int value) : base(key, value)
            {
            }
        }

        private class TestSimpleList : EmPropertyList<int>
        {
            public TestSimpleList(string key) : base(key)
            {
            }

            public TestSimpleList(string key, ICollection<int> values) : base(key, values)
            {
            }
        }

        private class TestSimpleCollection : EmPropertyCollection
        {
            public TestSimpleCollection(string key, ICollection<EmProperty> definitions) : base(key, definitions)
            {
            }

            internal override EmProperty Copy() => new TestSimpleCollection(Key, CopyDefinitions);
        }

        [Test]
        public void EmProperty_ToString_GetExpected()
        {
            const string key = "TestKey";
            var testProp = new TestProperty(key, 1);
            string expectedValue = $"{key}:{1};";

            Assert.AreEqual(1, testProp.Value);
            Assert.AreEqual(expectedValue, testProp.ToString());
        }

        [TestCase("TestKey:1;", 1)]
        [TestCase("TestKey : 1; ", 1)]
        [TestCase(" TestKey : 1 ;", 1)]
        [TestCase("TestKey:\r\n1\r\n;", 1)]
        public void EmProperty_FromString_GoodString_GetExpected(string goodString, int expectedValue)
        {
            var testProp = new TestProperty("TestKey");
            testProp.FromString(goodString);

            Assert.AreEqual(expectedValue, testProp.Value);
        }

        [TestCase("TestAKey:1;")]
        [TestCase("TestBKey : 1; ")]
        [TestCase(" TestCKey : 1 ;")]
        [TestCase("TestDKey:\r\n1\r\n;")]
        public void EmProperty_FromString_MismatchedKey_Throws(string serialValue)
        {
            var testProp = new TestProperty("TestKey");
            Assert.Throws<AssertionException>(() =>
            {
                testProp.FromString(serialValue, true);
            });
        }

        [TestCase("TestAKey:1;", 1)]
        [TestCase("TestBKey : 1; ", 1)]
        [TestCase(" TestCKey : 1 ;", 1)]
        [TestCase("TestDKey:\r\n1\r\n;", 1)]
        public void EmProperty_FromString_MismatchedKey_Ignored_GetExpected(string goodString, int expectedValue)
        {
            var testProp = new TestProperty("TestKey");
            testProp.FromString(goodString, false);

            Assert.AreEqual(expectedValue, testProp.Value);
        }

        [Test]
        public void EmProperty_ToAndFromString_DataPreserved()
        {
            const string key = "TestKey";
            var orig = new TestProperty(key, 10);
            var deserialized = new TestProperty(key);

            string originalSerialized = orig.ToString();
            deserialized.FromString(originalSerialized);

            string serialized = deserialized.ToString();

            Assert.AreEqual(orig.Value, deserialized.Value);
            Assert.AreEqual(originalSerialized, serialized);
        }

        [Test]
        public void EmPropertyList_ToString_GetExpected()
        {
            const string key = "TestKey";
            List<int> values = new List<int>(5)
            { 1, 2, 3, 4, 5, };

            var testProp = new TestSimpleList(key, values);
            string expectedValue = $"{key}:1,2,3,4,5;";

            Assert.AreEqual(values.Count, testProp.Count);
            for (int i = 0; i < testProp.Count; i++)
            {
                Assert.AreEqual(values[i], testProp[i]);
            }

            Assert.AreEqual(expectedValue, testProp.ToString());
        }

        [TestCase("TestKey:1,2,3,4,5;")]
        [TestCase("TestKey : 1, 2, 3, 4, 5; ")]
        [TestCase(" TestKey : 1 , 2 , 3 , 4 , 5 ;")]
        [TestCase("TestKey:\r\n1,\r\n2,\r\n3,\r\n4,\r\n5;")]
        public void EmPropertyList_FromString_GoodString_GetExpected(string goodString)
        {
            var values = new List<int> { 1, 2, 3, 4, 5 };

            var testProp = new TestSimpleList("TestKey");
            testProp.FromString(goodString);

            Assert.AreEqual(values.Count, testProp.Count);
            for (int i = 0; i < testProp.Count; i++)
            {
                Assert.AreEqual(values[i], testProp[i]);
            }
        }

        [Test]
        public void EmPropertyList_ToAndFromString_DataPreserved()
        {
            var values = new List<int> { 1, 2, 3, 4, 5 };

            const string key = "TestKey";
            var orig = new TestSimpleList(key, values);
            var deserialized = new TestSimpleList(key);

            string originalSerialized = orig.ToString();
            deserialized.FromString(originalSerialized);

            string serialized = deserialized.ToString();


            Assert.AreEqual(values.Count, orig.Count);
            for (int i = 0; i < deserialized.Count; i++)
            {
                Assert.AreEqual(values[i], orig[i]);
            }

            Assert.AreEqual(originalSerialized, serialized);
        }

        [Test]
        public void EmPropertyCollection_NoNestedComplex_ToString_GetExpected()
        {
            var properties = new List<EmProperty>()
            {
                new EmProperty<string>("String", "Val"),
                new EmProperty<int>("Int", 12),
                new EmPropertyList<float>("FloatList", new List<float>{ 1.0f, 2.1f, 3.2f }),
            };

            var testProp = new TestSimpleCollection("TestKey", properties);

            const string expectedValue = "TestKey:" +
                                         "(" +
                                             "String:Val;" +
                                             "Int:12;" +
                                             "FloatList:1,2.1,3.2;" +
                                         ");";

            Assert.AreEqual(expectedValue, testProp.ToString());
        }

        public void EmPropertyCollection_FromString_GoodString_GetExpected()
        {
            const string testValue = "TestKey:" +
                                     "(" +
                                        "String:Val;" +
                                        "Int:12;" +
                                        "FloatList:1,2.1,3.2;" +
                                     ");";

            var properties = new List<EmProperty>()
            {
                new EmProperty<string>("String"),
                new EmProperty<int>("Int"),
                new EmPropertyList<float>("FloatList"),
            };

            var testProp = new TestSimpleCollection("TestKey", properties);
            testProp.FromString(testValue);

            Assert.AreEqual("Val", ((EmProperty<string>)testProp["String"]).Value);
            Assert.AreEqual(12, ((EmProperty<int>)testProp["Int"]).Value);

            Assert.AreEqual(3, ((EmPropertyList<float>)testProp["FloatList"]).Count);
            Assert.AreEqual(1.0f, ((EmPropertyList<float>)testProp["FloatList"])[0]);
            Assert.AreEqual(2.1f, ((EmPropertyList<float>)testProp["FloatList"])[1]);
            Assert.AreEqual(3.2f, ((EmPropertyList<float>)testProp["FloatList"])[2]);
        }

        [Test]
        public void EmPropertyCollection_WithNestedCollection_ToString_GetExpected()
        {
            var nestedComplex = new TestSimpleCollection("Nested", new List<EmProperty>
            {
                new EmProperty<string>("Nstring", "Nval"),
                new EmProperty<int>("Nint", 10),
            });

            var properties = new List<EmProperty>
            {
                new EmProperty<string>("String", "Val"),
                new EmProperty<int>("Int", 12),
                new EmPropertyList<float>("FloatList", new List<float>{ 1.0f, 2.1f, 3.2f }),
                nestedComplex
            };


            var testProp = new TestSimpleCollection("TestKey", properties);

            const string expectedValue = "TestKey:" +
                                         "(" +
                                             "String:Val;" +
                                             "Int:12;" +
                                             "FloatList:1,2.1,3.2;" +
                                             "Nested:" +
                                                 "(" +
                                                 "Nstring:Nval;" +
                                                 "Nint:10;" +
                                                 ");" +
                                         ");";

            Assert.AreEqual(expectedValue, testProp.ToString());
        }

        [Test]
        public void EmPropertyCollection_WithNestedCollection_FromString_GetExpected()
        {
            const string testValue = "TestKey:" +
                                         "(" +
                                             "String:Val;" +
                                             "Int:12;" +
                                             "FloatList:1,2.1,3.2;" +
                                             "Nested:" +
                                                 "(" +
                                                     "Nstring:Nval;" +
                                                     "Nint:10;" +
                                                 ");" +
                                         ");";

            var nestedComplex = new TestSimpleCollection("Nested", new List<EmProperty>
            {
                new EmProperty<int>("Nint"),
                new EmProperty<string>("Nstring"),
            });

            var properties = new List<EmProperty>
            {
                new EmProperty<int>("Int"),
                new EmProperty<string>("String"),
                new EmPropertyList<float>("FloatList"),
                nestedComplex
            };

            var testProp = new TestSimpleCollection("TestKey", properties);
            testProp.FromString(testValue);

            Assert.AreEqual("Val", ((EmProperty<string>)testProp["String"]).Value);
            Assert.AreEqual(12, ((EmProperty<int>)testProp["Int"]).Value);

            Assert.AreEqual(3, ((EmPropertyList<float>)testProp["FloatList"]).Count);
            Assert.AreEqual(1.0f, ((EmPropertyList<float>)testProp["FloatList"])[0]);
            Assert.AreEqual(2.1f, ((EmPropertyList<float>)testProp["FloatList"])[1]);
            Assert.AreEqual(3.2f, ((EmPropertyList<float>)testProp["FloatList"])[2]);

            Assert.AreEqual("Nval", ((EmProperty<string>)((EmPropertyCollection)testProp["Nested"])["Nstring"]).Value);
            Assert.AreEqual(10, ((EmProperty<int>)((EmPropertyCollection)testProp["Nested"])["Nint"]).Value);
        }

        [Test]
        public void EmPropertyCollection_WithNestedCollection_ToAndFromString_DataPreserved()
        {
            var nestedComplexOrig = new TestSimpleCollection("Nested", new List<EmProperty>
            {
                new EmProperty<string>("Nstring", "Nval"),
                new EmProperty<int>("Nint", 10),
            });

            var propertiesOrig = new List<EmProperty>
            {
                new EmProperty<string>("String", "Val"),
                new EmProperty<int>("Int", 12),
                new EmPropertyList<float>("FloatList", new List<float>{ 1.0f, 2.1f, 3.2f }),
                nestedComplexOrig
            };

            var orig = new TestSimpleCollection("TestKey", propertiesOrig);

            var nestedComplex = new TestSimpleCollection("Nested", new List<EmProperty>
            {
                new EmProperty<string>("Nstring"),
                new EmProperty<int>("Nint"),
            });

            var properties = new List<EmProperty>
            {
                new EmProperty<string>("String"),
                new EmProperty<int>("Int"),
                new EmPropertyList<float>("FloatList"),
                nestedComplex
            };

            var deserialized = new TestSimpleCollection("TestKey", properties);

            string originalSerialized = orig.ToString();
            deserialized.FromString(originalSerialized);

            string serialized = deserialized.ToString();

            Assert.AreEqual(originalSerialized, serialized);

            Assert.AreEqual("Val", ((EmProperty<string>)deserialized["String"]).Value);
            Assert.AreEqual(12, ((EmProperty<int>)deserialized["Int"]).Value);

            Assert.AreEqual(3, ((EmPropertyList<float>)deserialized["FloatList"]).Count);
            Assert.AreEqual(1.0f, ((EmPropertyList<float>)deserialized["FloatList"])[0]);
            Assert.AreEqual(2.1f, ((EmPropertyList<float>)deserialized["FloatList"])[1]);
            Assert.AreEqual(3.2f, ((EmPropertyList<float>)deserialized["FloatList"])[2]);

            Assert.AreEqual("Nval", ((EmProperty<string>)((EmPropertyCollection)deserialized["Nested"])["Nstring"]).Value);
            Assert.AreEqual(10, ((EmProperty<int>)((EmPropertyCollection)deserialized["Nested"])["Nint"]).Value);
        }

        [Test]
        public void EmPropertyCollectionList_WithMultiEntries_FromString_GetExpected()
        {
            const string testValue = "NestedComplexList:" +
                                        "(" +
                                            "Nstring:Val1;" +
                                            "Nint:2;" +
                                        ")," +
                                        "(" +
                                            "Nstring:Val3;," +
                                            "Nint:4;" +
                                        ");";

            var complexStructure = new List<EmProperty>
            {
                new EmProperty<string>("Nstring"),
                new EmProperty<int>("Nint"),
            };

            var twoKeyCollection = new TestSimpleCollection("NestedComplexList", complexStructure);

            var compList = new EmPropertyCollectionList<TestSimpleCollection>("NestedComplexList", twoKeyCollection);

            compList.FromString(testValue);

            Assert.AreEqual(2, compList.Count);

            Assert.AreEqual("Val1", ((EmProperty<string>)compList[0]["Nstring"]).Value);
            Assert.AreEqual(2, ((EmProperty<int>)compList[0]["Nint"]).Value);

            Assert.AreEqual("Val3", ((EmProperty<string>)compList[1]["Nstring"]).Value);
            Assert.AreEqual(4, ((EmProperty<int>)compList[1]["Nint"]).Value);
        }

        [Test]
        public void EmPropertyCollection_WithNestedCollection_PrettyPrint_GetExpected()
        {
            const string testValue = "TestKey: " +
                                     "(" +
                                         "String: Val;" +
                                         "Int: 12;" +
                                         "FloatList: 1,2.1,3.2;" +
                                         "Nested:" +
                                         "(" +
                                             "Nstring: Nval;" +
                                             "Nint: 10;" +
                                         ");" +
                                     ");";

            const string expectedValue = "TestKey: \r\n" +
                                         "(\r\n" +
                                         "    String: Val;\r\n" +
                                         "    Int: 12;\r\n" +
                                         "    FloatList: 1,2.1,3.2;\r\n" +
                                         "    Nested: \r\n" +
                                         "    (\r\n" +
                                         "        Nstring: Nval;\r\n" +
                                         "        Nint: 10;\r\n" +
                                         "    );\r\n" +
                                         ");\r\n";

            var nestedComplex = new TestSimpleCollection("Nested", new List<EmProperty>
            {
                new EmProperty<string>("Nstring"),
                new EmProperty<int>("Nint"),
            });

            var properties = new List<EmProperty>
            {
                new EmProperty<string>("String"),
                new EmProperty<int>("Int"),
                new EmPropertyList<float>("FloatList"),
                nestedComplex
            };

            var testProp = new TestSimpleCollection("TestKey", properties);
            testProp.FromString(testValue);

            var actualValue = testProp.PrettyPrint();

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void EmProperty_SuperSImple_PrettyPrint_GetExpected()
        {
            var emString = new EmProperty<string>("String", "Value");

            var actualValue = emString.PrettyPrint();

            Assert.AreEqual("String: Value;\r\n", actualValue);
        }
    }
}