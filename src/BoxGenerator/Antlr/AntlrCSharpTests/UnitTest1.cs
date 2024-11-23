using Antlr4.Runtime;
using AntlrCSharp;

namespace AntlrCSharpTests;

[TestClass]
public class UnitTest1
{
    [TestClass]
    public class ParserTest
    {
        private SpeakParser Setup(string text)
        {
            AntlrInputStream inputStream = new AntlrInputStream(text);
            SpeakLexer speakLexer = new SpeakLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(speakLexer);
            SpeakParser speakParser = new SpeakParser(commonTokenStream);

            return speakParser;
        }

        [TestMethod]
        public void TestChat()
        {
            SpeakParser parser = Setup("john says \"hello\" \n michael says \"world\" \n");

            SpeakParser.ChatContext context = parser.chat();
            BasicSpeakVisitor visitor = new BasicSpeakVisitor();
            visitor.Visit(context);

            Assert.AreEqual(2, visitor.Lines.Count);
        }

        [TestMethod]
        public void TestLine()
        {
            SpeakParser parser = Setup("john says \"hello\" \n");

            SpeakParser.LineContext context = parser.line();
            BasicSpeakVisitor visitor = new BasicSpeakVisitor();
            SpeakLine line = (SpeakLine)visitor.VisitLine(context);

            Assert.AreEqual("john", line.Person);
            Assert.AreEqual("hello", line.Text);
        }

        [TestMethod]
        public void TestWrongLine()
        {
            SpeakParser parser = Setup("john sayan \"hello\" \n");

            var context = parser.line();

            Assert.IsInstanceOfType(context, typeof(SpeakParser.LineContext));
            Assert.AreEqual("john", context.name().GetText());
            Assert.IsNull(context.SAYS());
            Assert.AreEqual("johnsayan\"hello\"\n", context.GetText());
        }
    }
}