using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SpeakParser;

namespace AntlrCSharp
{
    public class BasicSpeakVisitor : SpeakBaseVisitor<object>
    {
        public List<SpeakLine> Lines = new List<SpeakLine>();

        public override object VisitLine(SpeakParser.LineContext context)
        {
            NameContext name = context.name();
            OpinionContext opinion = context.opinion();

            SpeakLine line = new SpeakLine() { Person = name.GetText(), Text = opinion.GetText().Trim('"') };
            Lines.Add(line);

            return line;
        }
    }
}
