namespace CustomCraftSMLTests
{
    using EasyMarkup;
    using NUnit.Framework;

    [TestFixture]
    public class StringBufferTests
    {
        [TestCase("123", "1", "A", "A23")]
        [TestCase("123", "2", "B", "1B3")]
        [TestCase("123", "3", "C", "12C")]
        [TestCase("123123123", "2", "B", "1B31B31B3")]
        [TestCase("1234", "23", "B", "1B4")]
        [TestCase("1234", "23", "ABC", "1ABC4")]
        [TestCase("1234123$1234", "234", "X", "1X123$1X")]
        [TestCase("123", "0", "A", "123")]
        [TestCase("123", "123", "ABC", "ABC")]
        public void Replace_String_GetExpectedString(string original, string search, string replace, string expected)
        {
            var buffer = new StringBuffer(original);

            buffer.Replace(search, replace);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("123", '1', 'A', "A23")]
        [TestCase("123", '2', 'B', "1B3")]
        [TestCase("123", '3', 'C', "12C")]
        [TestCase("123123123", '2', 'X', "1X31X31X3")]
        [TestCase("123", '0', 'A', "123")]
        [TestCase("111", '1', 'A', "AAA")]
        public void Replace_Char_GetExpectedString(string original, char search, char replace, string expected)
        {
            var buffer = new StringBuffer(original);

            buffer.Replace(search, replace);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("123", "ABC", "ABC123")]
        [TestCase("123", "", "123")]
        public void PushToStart_String_GetExpectedString(string original, string pushed, string expected)
        {
            var buffer = new StringBuffer(original);

            buffer.PushToStart(pushed);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("123", "ABC", "123ABC")]
        [TestCase("123", "", "123")]
        public void PushToEnd_String_GetExpectedString(string original, string pushed, string expected)
        {
            var buffer = new StringBuffer(original);

            buffer.PushToEnd(pushed);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("123", "ABC", "ABC123")]
        [TestCase("123", "", "123")]
        public void TransferToStart_String_GetExpectedString(string original, string toTransfer, string expected)
        {
            var buffer = new StringBuffer(original);
            var xfer = new StringBuffer(toTransfer);

            buffer.TransferToStart(xfer);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("123", "ABC", "123ABC")]
        [TestCase("123", "", "123")]
        public void TransferToEnd_String_GetExpectedString(string original, string toTransfer, string expected)
        {
            var buffer = new StringBuffer(original);
            var xfer = new StringBuffer(toTransfer);

            buffer.TransferToEnd(xfer);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("ABC123ABC", "ABC", "123ABC")]
        [TestCase("AAAA123ABC", "A", "123ABC")]
        [TestCase("A1A123ABCA", "A", "1A123ABCA")]
        [TestCase("ABBCCC123ABC", "ABC", "123ABC")]
        public void PopAllFromStartIfEquals_Chars_GetExpectedString(string original, string ToPop, string expected)
        {
            var buffer = new StringBuffer(original);

            char[] toRemove = ToPop.ToCharArray();

            buffer.PopAllFromStartIfEquals(toRemove);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("ABC123ABC", "ABC", "ABC123")]
        [TestCase("ABC123AAAAA", "A", "ABC123")]
        [TestCase("ABCA123A1A", "A", "ABCA123A1")]
        [TestCase("ABC123ABBCCC", "ABC", "ABC123")]
        public void PopAllFromEndIfEquals_Chars_GetExpectedString(string original, string ToPop, string expected)
        {
            var buffer = new StringBuffer(original);

            char[] toRemove = ToPop.ToCharArray();

            buffer.PopAllFromEndIfEquals(toRemove);

            string actual = buffer.ToString();

            Assert.AreEqual(expected, actual);
        }

        [TestCase("123", "1", true)]
        [TestCase("123", "12", true)]
        [TestCase("123", "123", true)]
        [TestCase("123", "1234", false)]
        [TestCase("1245", "123", false)]
        public void StartsWith_String_GetExpected(string original, string search, bool expected)
        {
            var buffer = new StringBuffer(original);

            bool actual = buffer.StartsWith(search);

            Assert.AreEqual(expected, actual);
        }

        [TestCase("321", "1", true)]
        [TestCase("321", "21", true)]
        [TestCase("123", "123", true)]
        [TestCase("123", "1234", false)]
        [TestCase("5423", "123", false)]
        public void EndsWith_String_GetExpected(string original, string search, bool expected)
        {
            var buffer = new StringBuffer(original);

            bool actual = buffer.EndsWith(search);

            Assert.AreEqual(expected, actual);
        }

    }
}
