using Pidgin;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace BoxGenerator
{
    public class Parser
    {
        public static Parser<char, string> Identifier => LetterOrDigit.Then(Token(c => char.IsLetterOrDigit(c) || c == '_').ManyString(), (first, rest) => first + rest);
        public static Parser<char, string> IdentifierWithSpace => Token(c => char.IsLetterOrDigit(c) || c == '_' || c == ' ' || c == '©' || c == '@' || c == '-').ManyString();
        public static Parser<char, char> LetterOrDigitOrUnderscore => Token(c => char.IsLetterOrDigit(c) || c == '_');
        public static Parser<char, char> LetterOrDigitOrUnderscoreOrDot => Token(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
        public static Parser<char, string> Parentheses => Char('(').Then(Rec(() => Expr).Until(Char(')'))).Select(x => $"({string.Concat(x)})");
        public static Parser<char, string> Expr => OneOf(Rec(() => Parentheses), AnyCharE);
        public static Parser<char, string> AnyCharE => AnyCharExcept('(', ')').AtLeastOnceString();

        public static Parser<char, string> BoxName => Identifier;
        public static Parser<char, string> FieldName => Identifier;

        public static Parser<char, string> BoxType => Char('\'').Then(IdentifierWithSpace).Before(Char('\''));
        public static Parser<char, string> OldBoxType => Char('\'').Then(Char('!')).Then(IdentifierWithSpace).Before(Char('\'')).Before(Char(',')).Before(SkipWhitespaces);
        public static Parser<char, string> ClassType => Parentheses;

        public static Parser<char, string> Parameter => SkipWhitespaces.Then(LetterOrDigitOrUnderscore.ManyString());
        public static Parser<char, string> ParameterValue => Char('=').Then(SkipWhitespaces).Then(LetterOrDigit.ManyString());
        public static Parser<char, (string Name, Maybe<string> Value)> ParameterFull => Map((name, value) => (name, value), Parameter.Before(SkipWhitespaces), Try(ParameterValue).Optional());
        public static Parser<char, IEnumerable<(string Name, Maybe<string> Value)>> Parameters => ParameterFull.SeparatedAndOptionallyTerminated(Char(','));

        public static Parser<char, string> TagValue => LetterOrDigitOrUnderscoreOrDot.ManyString();
        public static Parser<char, string> DescriptorTag => SkipWhitespaces.Then(Try(String("ProfileLevelIndicationIndexDescrTag")).Or(String("tag=").Then(TagValue)));

        public static Parser<char, string> FieldArray => Char('[').Then(Any.Until(Char(']'))).Select(x => $"[{string.Concat(x)}]");
        public static Parser<char, IEnumerable<PseudoCode>> SingleBlock => Field.Repeat(1);
        public static Parser<char, PseudoCode> CodeBlock => Try(SwitchBlock).Or(Try(Block).Or(Try(RepeatingBlock).Or(Try(Field).Or(Comment))));
        public static Parser<char, IEnumerable<PseudoCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));

        public static Parser<char, PseudoType> FieldType =>
            Map((aligned, template, cons, sign, type, param) => new PseudoType(aligned, template, cons, sign, type, param),
                Try(String("aligned").Before(SkipWhitespaces)).Optional(),
                Try(String("template").Before(SkipWhitespaces)).Optional(),
                Try(String("const").Before(SkipWhitespaces)).Optional(),
                Try(OneOf(Try(String("signed")), Try(String("unsigned"))).Before(Try(Char('_')).Optional()).Before(SkipWhitespaces)).Optional(),
                Identifier.Before(SkipWhitespaces),
                Try(Parentheses).Optional()
            );

        public static Parser<char, PseudoCode> Field =>
            Map((type, array, name, value, comment) => new PseudoField(type, array, name, value, comment),
                Try(FieldTypeWorkaround.Select(x => new PseudoType() { Type = x })).Or(FieldType).Before(SkipWhitespaces),
                Try(FieldArray.Before(SkipWhitespaces)).Optional(),
                Try(FieldName.Before(SkipWhitespaces)).Optional(),
                Try(Any.Until(Char(';')).Before(SkipWhitespaces)).Or(Try(Any.Until(Char('\n')).Before(SkipWhitespaces))).Optional(),
                Try(LineComment(String("//"))).Optional()
            ).Select(x => (PseudoCode)x);

        public static Parser<char, PseudoCode> Block =>
            Map((type, condition, comment, content) => new PseudoBlock(type, condition, comment, content),
                OneOf(
                    Try(String("else if")),
                    Try(String("if")),
                    Try(String("else")),
                    Try(String("do ")),
                    Try(String("for")),
                    Try(String("while"))
                    ).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (PseudoCode)x);


        public static Parser<char, PseudoCode> CaseBlock => Map((cs, op) => new PseudoCase(cs, op), String("case").Before(SkipWhitespaces), AnyCharExcept(':').AtLeastOnceString().Before(SkipWhitespaces).Before(Char(':')).Before(SkipWhitespaces)).Select(x => (PseudoCode)x);
        public static Parser<char, PseudoCode> DefaultBlock => Map((cs) => new PseudoCase(cs), String("default").Before(SkipWhitespaces).Before(Char(':')).Before(SkipWhitespaces).Select(x => $"{x}:")).Select(x => (PseudoCode)x);
        public static Parser<char, PseudoCode> BreakBlock => Map((cs) => new PseudoCase(cs), String("break").Before(SkipWhitespaces).Before(Char(';')).Before(SkipWhitespaces).Select(x => $"{x}:")).Select(x => (PseudoCode)x);

        public static Parser<char, PseudoCode> SwitchBlock =>
            Map((type, condition, comment, content) => new PseudoBlock(type, condition, comment, content),
                String("switch"),
                SkipWhitespaces.Then(Try(Parentheses).Optional()),
                SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => SwitchCaseBlocks).Between(Char('{'), Char('}'))))
            ).Select(x => (PseudoCode)x);

        public static Parser<char, PseudoCode> SwitchCaseBlock => Try(CaseBlock).Or(Try(DefaultBlock).Or(Try(BreakBlock).Or(Try(Block).Or(Try(Field).Or(Comment)))));

        public static Parser<char, IEnumerable<PseudoCode>> SwitchCaseBlocks => SkipWhitespaces.Then(SwitchCaseBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));

        public static Parser<char, PseudoCode> RepeatingBlock =>
            Map((content, array) => new PseudoRepeatingBlock(content, array),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))),
                SkipWhitespaces.Then(Char('[')).Then(Any.AtLeastOnceUntil(Char(']')))
            ).Select(x => (PseudoCode)x);

        public static Parser<char, string> MultiBoxType =>
            Map((box, otherBox) => $"'{box}' or '{otherBox}'",
                BoxType.Before(SkipWhitespaces),
                String("or").Then(SkipWhitespaces).Then(BoxType)
            );

        public static Parser<char, PseudoExtendedClass> ExtendedClass =>
            Map((extendedBoxName, oldBoxType, boxType, parameters, descriptorTag) => new PseudoExtendedClass(extendedBoxName, oldBoxType, boxType, parameters, descriptorTag),
                SkipWhitespaces.Then(Try(String("extends")).Optional()).Then(SkipWhitespaces).Then(Try(BoxName).Optional()),
                SkipWhitespaces.Then(Try(Char('(')).Optional()).Then(Try(OldBoxType).Optional()),
                SkipWhitespaces.Then(Try(MultiBoxType).Or(Try(BoxType)).Optional()),
                SkipWhitespaces.Then(Try(Char(',')).Optional()).Then(Try(Parameters).Optional()).Before(Try(Char(')')).Optional()).Before(SkipWhitespaces).Optional(),
                SkipWhitespaces.Then(Try(Char(':').Then(SkipWhitespaces).Then(String("bit(8)")).Then(SkipWhitespaces).Then(DescriptorTag).Before(SkipWhitespaces))).Optional()
            );

        public static Parser<char, PseudoClass> Box =>
            Map((comment, alignment, boxName, classType, extended, fields, endComment, endOffset) => new PseudoClass(comment, alignment, boxName, classType, extended, fields, endComment, endOffset),
                SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
                SkipWhitespaces.Then(Try(String("abstract")).Optional()).Before(SkipWhitespaces).Before(Try(String("aligned(8)")).Optional()).Before(SkipWhitespaces).Before(Try(String("expandable(228-1)")).Optional()),
                SkipWhitespaces.Then(String("class")).Then(SkipWhitespaces).Then(Identifier),
                SkipWhitespaces.Then(Try(ClassType).Optional()),
                SkipWhitespaces.Then(Try(ExtendedClass).Optional()),
                Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}')),
                SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
                CurrentOffset
            );
        public static Parser<char, IEnumerable<PseudoClass>> Boxes => SkipWhitespaces.Then(Box.SeparatedAndOptionallyTerminated(SkipWhitespaces));

        public static Parser<char, PseudoCode> Comment =>
            Map((comment) => new PseudoComment(comment),
                Try(LineComment(String("//"))).Or(BlockComment(String("/*"), String("*/")))
            ).Select(x => (PseudoCode)x);

        public static Parser<char, string> LineComment<T>(Parser<char, T> lineCommentStart)
        {
            if (lineCommentStart == null)
                throw new ArgumentNullException(nameof(lineCommentStart));
            Parser<char, Unit> eol = Try(EndOfLine).IgnoreResult();
            return lineCommentStart.Then(Any.Until(End.Or(eol)), (first, rest) => string.Concat(rest));
        }

        public static Parser<char, string> BlockComment<T, U>(Parser<char, T> blockCommentStart, Parser<char, U> blockCommentEnd)
        {
            if (blockCommentStart == null)
                throw new ArgumentNullException(nameof(blockCommentStart));
            if (blockCommentEnd == null)
                throw new ArgumentNullException(nameof(blockCommentEnd));
            return blockCommentStart.Then(Any.Until(blockCommentEnd), (first, rest) => string.Concat(rest));
        }

        public static HashSet<string> Workarounds = new HashSet<string>()
        {
            "int downmix_instructions_count = 1",
            "int i, j",
            "int i,j",
            "int i",
            "j=1",
            "j++",
            "int size = 4",
            "sizeOfInstance = sizeOfInstance<<7 | sizeByte",
            "int sizeOfInstance = 0",
            "size += 5",
            "subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8",
            "totalPatternLength = 0",
            "samplerate = samplerate >> 16",
            "sbrPresentFlag = -1",
            "psPresentFlag = -1",
            "extensionAudioObjectType = 0",
            "extensionAudioObjectType = 5",
            "sbrPresentFlag = 1",
            "psPresentFlag = 1",
            "audioObjectType = 32 + audioObjectTypeExt",
            "return audioObjectType",
            "len = eldExtLen",
            "len += eldExtLenAddAdd",
            "len += eldExtLenAdd",
            "numSbrHeader = 1",
            "numSbrHeader = 2",
            "numSbrHeader = 3",
            "numSbrHeader = 4",
            "numSbrHeader = 0"
        };

        public static Parser<char, string> FieldTypeWorkaround => OneOf(Workarounds.Select(x => Try(String(x))));
    }

    public enum ParsedBoxType
    {
        Unknown,
        Box,
        Entry,
        Descriptor,
        Class,
        Number,
        String,
        Iso639,
        ByteAlignment
    }

    [SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
    public interface PseudoCode
    { }

    public class PseudoType : PseudoCode
    {
        public string Aligned { get; set; }
        public string Template { get; set; }
        public string Cons { get; set; }
        public string Sign { get; set; }
        public string Type { get; set; }
        public string Param { get; set; }

        public PseudoType()
        { }

        public PseudoType(Maybe<string> aligned, Maybe<string> template, Maybe<string> cons, Maybe<string> sign, string type, Maybe<string> param)
        {
            this.Aligned = aligned.GetValueOrDefault();
            this.Template = template.GetValueOrDefault();
            this.Cons = cons.GetValueOrDefault();
            this.Sign = sign.GetValueOrDefault();
            this.Type = type;
            this.Param = param.GetValueOrDefault();
        }

        public override string ToString()
        {
            var fields = new string[] { Aligned, Template, Cons, Sign, Type, Param };
            return string.Join(" ", fields.Where(x => !string.IsNullOrEmpty(x))).Replace($"{Type} {Param}", $"{Type}{Param}");
        }
    }

    public class PseudoClass : PseudoCode
    {
        public ParsedBoxType ParsedBoxType { get; set; }
        public string Syntax { get; set; }
        public bool IsContainer { get; set; }
        public string CtorContent { get; set; } // TODO: remove C#

        public string Comment { get; set; }
        public string Abstract { get; set; }
        public string BoxName { get; set; }
        public string ClassType { get; set; }
        public PseudoExtendedClass Extended { get; set; }
        public string EndComment { get; set; }
        public long CurrentOffset { get; set; }
        public IList<PseudoCode> Fields { get; set; }
        public List<PseudoField> FlattenedFields { get; internal set; }

        public PseudoClass()
        { }

        public PseudoClass(
            Maybe<string> comment,
            Maybe<string> abstrct,
            string boxName,
            Maybe<string> classType,
            Maybe<PseudoExtendedClass> extended,
            IEnumerable<PseudoCode> fields,
            Maybe<string> endComment,
            long currentOffset)
        {
            Comment = comment.GetValueOrDefault();
            Abstract = abstrct.GetValueOrDefault();
            BoxName = boxName;
            ClassType = classType.GetValueOrDefault();
            Extended = extended.GetValueOrDefault();
            Fields = fields.ToList();
            EndComment = endComment.GetValueOrDefault();
            CurrentOffset = currentOffset;
        }

    }

    public class PseudoExtendedClass : PseudoCode
    {
        public string OldType { get; set; }
        public string BoxType { get; set; }
        public string UserType { get; set; }
        public IList<(string Name, string Value)> Parameters { get; set; }
        public string BoxName { get; set; }
        public string DescriptorTag { get; set; }

        public PseudoExtendedClass()
        { }

        public PseudoExtendedClass(
            Maybe<string> boxName,
            Maybe<string> oldType,
            Maybe<string> boxType,
            Maybe<Maybe<IEnumerable<(string Name, Maybe<string> Value)>>> parameters,
            Maybe<string> descriptorTag)
        {
            OldType = oldType.GetValueOrDefault();
            BoxType = boxType.GetValueOrDefault();
            var p = parameters.GetValueOrDefault();
            if (p.HasValue)
            {
                var pp = p.GetValueOrDefault();
                Parameters = pp.Where(x => !(string.IsNullOrEmpty(x.Name) && string.IsNullOrEmpty(x.Value.GetValueOrDefault()))).Select(x => (x.Name, x.Value.GetValueOrDefault())).ToList();
                if (Parameters.Count == 0)
                    Parameters = null;
            }
            BoxName = boxName.GetValueOrDefault();
            DescriptorTag = descriptorTag.GetValueOrDefault();
        }
    }

    public class PseudoField : PseudoCode
    {
        public PseudoField()
        {  }

        public PseudoField(PseudoType type, Maybe<string> array, Maybe<string> name, Maybe<IEnumerable<char>> value, Maybe<string> comment)
        {
            Type = type;
            FieldArray = array.GetValueOrDefault();
            Name = name.GetValueOrDefault();
            Value = value.HasValue ? string.IsNullOrEmpty(string.Concat(value.GetValueOrDefault())) ? null : string.Concat(value.GetValueOrDefault()) : null;
            Comment = comment.GetValueOrDefault();
        }

        public PseudoType Type { get; set; }
        public string FieldArray { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
        public PseudoBlock Parent { get; set; }
    }


    public class PseudoComment : PseudoCode
    {
        public PseudoComment()
        { }

        public PseudoComment(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; }
    }

    public class PseudoCase : PseudoCode
    {
        public PseudoCase()
        { }

        public PseudoCase(string cs, string op = "")
        {
            Cs = cs;
            Op = op;
        }

        public string Cs { get; }
        public string Op { get; }
    }

    public class PseudoBlock : PseudoCode
    {
        public PseudoBlock()
        {  }

        public PseudoBlock(string type, Maybe<string> condition, Maybe<string> comment, IEnumerable<PseudoCode> content)
        {
            Type = type;
            Condition = condition.GetValueOrDefault();
            Content = content;
            Comment = comment.GetValueOrDefault();
        }

        public string Type { get; }
        public string Condition { get; }
        public string Comment { get; }
        public IEnumerable<PseudoCode> Content { get; }
        public PseudoBlock Parent { get; set; }
        public List<PseudoField> RequiresAllocation { get; set; } = new List<PseudoField>();
    }

    public class PseudoRepeatingBlock : PseudoCode
    {
        public PseudoRepeatingBlock()
        { }

        public PseudoRepeatingBlock(IEnumerable<PseudoCode> content, IEnumerable<char> array)
        {
            Content = content;
            Array = string.Concat(array);
        }

        public string Array { get; }
        public IEnumerable<PseudoCode> Content { get; }
    }
}
