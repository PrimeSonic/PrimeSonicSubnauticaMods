namespace CustomCraftSMLTests
{
    using System.Collections.Generic;
    using EasyMarkup;
    using NUnit.Framework;

    [TestFixture]
    public class EasyMarkupTests
    {
        private class TestStringProperty : EmProperty<string>
        {
            public TestStringProperty(string key) : base(key)
            {
            }

            public TestStringProperty(string key, string value) : base(key, value)
            {
            }
        }

        private class TestSimpleStringList : EmPropertyList<string>
        {
            public TestSimpleStringList(string key) : base(key)
            {
            }

            public TestSimpleStringList(string key, ICollection<string> values) : base(key, values)
            {
            }
        }

        private class TestIntProperty : EmProperty<int>
        {
            public TestIntProperty(string key) : base(key)
            {
            }

            public TestIntProperty(string key, int value) : base(key, value)
            {
            }
        }

        private class TestSimpleIntList : EmPropertyList<int>
        {
            public TestSimpleIntList(string key) : base(key)
            {
            }

            public TestSimpleIntList(string key, ICollection<int> values) : base(key, values)
            {
            }
        }

        private class TestSimpleCollection : EmPropertyCollection
        {
            public static List<EmProperty> DefaultComplexStructure => new List<EmProperty>
            {
                new EmProperty<string>("Nstring"),
                new EmProperty<int>("Nint"),
            };

            public static string DefaultKey = "NestedComplexList";


            public TestSimpleCollection() : this(DefaultKey, DefaultComplexStructure)
            {
            }

            public TestSimpleCollection(string key, ICollection<EmProperty> definitions) : base(key, definitions)
            {
            }

            internal override EmProperty Copy()
            {
                return new TestSimpleCollection(this.Key, this.CopyDefinitions);
            }
        }

        [Test]
        public void EmProperty_ToString_GetExpected()
        {
            const string key = "TestKey";
            var testProp = new TestIntProperty(key, 1);
            string expectedValue = $"{key}:{1};";

            Assert.AreEqual(1, testProp.Value);
            Assert.AreEqual(expectedValue, testProp.ToString());
        }

        [TestCase("TestKey:\"Test Value\";", "Test Value")]
        [TestCase("TestKey : \"Test,Value\"; ", "Test,Value")]
        [TestCase(" TestKey : \"Test;Value\" ;", "Test;Value")]
        [TestCase(" TestKey: \"Test:Value\";", "Test:Value")]
        [TestCase(" TestKey: \"Test:  Value\";", "Test:  Value")]
        [TestCase(@"TestKey : Test\,Value; ", "Test,Value")]
        [TestCase(@" TestKey : Test\;Value ;", "Test;Value")]
        [TestCase(@" TestKey: Test\:Value;", "Test:Value")]
        [TestCase(@" TestKey: Test\#Value;", "Test#Value")]
        public void EmPropertyString_FromString_GoodString_GetExpected(string goodString, string expectedValue)
        {
            var testProp = new TestStringProperty("TestKey");
            testProp.FromString(goodString);

            Assert.AreEqual(expectedValue, testProp.Value);
        }

        [TestCase("TestKey:1;", 1)]
        [TestCase("TestKey : 1; ", 1)]
        [TestCase(" TestKey : 1 ;", 1)]
        [TestCase("TestKey:\"1\";", 1)]
        [TestCase("TestKey : \"1\"; ", 1)]
        [TestCase(" TestKey : \"1\" ;", 1)]
        [TestCase("TestKey:\r\n1\r\n;", 1)]
        public void EmPropertyInt_FromString_GoodString_GetExpected(string goodString, int expectedValue)
        {
            var testProp = new TestIntProperty("TestKey");
            testProp.FromString(goodString);

            Assert.AreEqual(expectedValue, testProp.Value);
        }

        [TestCase("TestAKey:1;", "TestAKey")]
        [TestCase("TestBKey : 1; ", "TestBKey")]
        [TestCase(" TestCKey : 1 ;", "TestCKey")]
        [TestCase("TestDKey:\r\n1\r\n;", "TestDKey")]
        public void EmProperty_FromString_MismatchedKey_Throws(string serialValue, string badKey)
        {
            var testProp = new TestIntProperty("TestKey");
            EmException emEx = Assert.Throws<EmException>(() => testProp.FromString(serialValue, true));

            Assert.IsNotNull(emEx.CurrentBuffer);
            Assert.IsFalse(emEx.CurrentBuffer.IsEmpty);
            Assert.AreEqual(badKey, emEx.CurrentBuffer.ToString());
        }

        [TestCase("TestAKey:1;", 1)]
        [TestCase("TestBKey : 1; ", 1)]
        [TestCase(" TestCKey : 1 ;", 1)]
        [TestCase("TestDKey:\r\n1\r\n;", 1)]
        public void EmProperty_FromString_MismatchedKey_Ignored_GetExpected(string goodString, int expectedValue)
        {
            var testProp = new TestIntProperty("TestKey");
            testProp.FromString(goodString, false);

            Assert.AreEqual(expectedValue, testProp.Value);
        }

        [Test]
        public void EmProperty_ToAndFromString_DataPreserved()
        {
            const string key = "TestKey";
            var orig = new TestIntProperty(key, 10);
            var deserialized = new TestIntProperty(key);

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
            var values = new List<int>(5)
            { 1, 2, 3, 4, 5, };

            var testProp = new TestSimpleIntList(key, values);
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
        [TestCase("TestKey:\r\n    1,2,3,\r\n    4,5;")]
        public void EmPropertyIntList_FromString_GoodString_GetExpected(string goodString)
        {
            var values = new List<int> { 1, 2, 3, 4, 5 };

            var testProp = new TestSimpleIntList("TestKey");
            testProp.FromString(goodString);

            Assert.AreEqual(values.Count, testProp.Count);
            for (int i = 0; i < testProp.Count; i++)
            {
                Assert.AreEqual(values[i], testProp[i]);
            }
        }

        [TestCase("TestKey:1\\:,2\\,,3\\;,\"4 \",\" 5\";")]
        [TestCase("TestKey : \"1:\", \"2,\", \"3;\", \"4 \", \" 5\";")]
        public void EmPropertyStringList_FromString_GoodString_GetExpected(string goodString)
        {
            var values = new List<string> { "1:", "2,", "3;", "4 ", " 5" };

            var testProp = new TestSimpleStringList("TestKey");
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
            var orig = new TestSimpleIntList(key, values);
            var deserialized = new TestSimpleIntList(key);

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

        [TestCase("Val", "Val")]
        [TestCase("Val:", @"Val\:")]
        [TestCase("Val,", @"Val\,")]
        [TestCase("Val;", @"Val\;")]
        [TestCase("Val#", @"Val\#")]
        public void EmPropertyCollection_NoNestedComplex_ToString_GetExpected(string stValue, string escapedValue)
        {
            var properties = new List<EmProperty>()
            {
                new EmProperty<string>("String", stValue),
                new EmProperty<int>("Int", 12),
                new EmPropertyList<float>("FloatList", new List<float>{ 1.0f, 2.1f, 3.2f }),
            };

            var testProp = new TestSimpleCollection("TestKey", properties);

            string expectedValue = "TestKey:" +
                                    "(" +
                                    $"String:{escapedValue};" +
                                        "Int:12;" +
                                        "FloatList:1,2.1,3.2;" +
                                    ");";

            Assert.AreEqual(expectedValue, testProp.ToString());
        }

        [TestCase("Val", "Val")]
        [TestCase("Val:", @"Val\:")]
        [TestCase("Val,", @"Val\,")]
        [TestCase("Val;", @"Val\;")]
        [TestCase("Val#", @"Val\#")]
        [TestCase("Val:01", "\"Val:01\"")]
        public void EmPropertyCollection_FromString_GoodString_GetExpected(string stValue, string escapedValue)
        {
            string testValue = "TestKey:" +
                                "(" +
                                   $"String:{escapedValue};" +
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

            Assert.AreEqual(stValue, ((EmProperty<string>)testProp["String"]).Value);
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

            const string text = "Val : has ; special";
            const int number = 12;

            var list = new List<float> { 1.0f, 2.1f, 3.2f };

            var propertiesOrig = new List<EmProperty>
            {
                new EmProperty<string>("String", text),
                new EmProperty<int>("Int", number),
                new EmPropertyList<float>("FloatList", list),
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

            Assert.AreEqual(text, ((EmProperty<string>)deserialized["String"]).Value);
            Assert.AreEqual(number, ((EmProperty<int>)deserialized["Int"]).Value);

            Assert.AreEqual(list.Count, ((EmPropertyList<float>)deserialized["FloatList"]).Count);
            Assert.AreEqual(list[0], ((EmPropertyList<float>)deserialized["FloatList"])[0]);
            Assert.AreEqual(list[1], ((EmPropertyList<float>)deserialized["FloatList"])[1]);
            Assert.AreEqual(list[2], ((EmPropertyList<float>)deserialized["FloatList"])[2]);

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


            var compList = new EmPropertyCollectionList<TestSimpleCollection>("NestedComplexList");

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

            string actualValue = testProp.PrettyPrint();

            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void EmProperty_SuperSimple_PrettyPrint_GetExpected()
        {
            var emString = new EmProperty<string>("String", "Value");

            string actualValue = emString.PrettyPrint();

            Assert.AreEqual("String: Value;\r\n", actualValue);
        }
    }
}