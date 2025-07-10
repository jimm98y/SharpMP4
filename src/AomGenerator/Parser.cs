using Pidgin;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace AomGenerator
{
    public class Parser
    {
        public static Parser<char, string> Identifier => LetterOrDigit.Then(Token(c => char.IsLetterOrDigit(c) || c == '_').ManyString(), (first, rest) => first + rest);

        public static Parser<char, string> BlockComment<T, U>(Parser<char, T> blockCommentStart, Parser<char, U> blockCommentEnd)
        {
            if (blockCommentStart == null)
                throw new ArgumentNullException(nameof(blockCommentStart));
            if (blockCommentEnd == null)
                throw new ArgumentNullException(nameof(blockCommentEnd));
            return blockCommentStart.Then(Any.Until(blockCommentEnd), (first, rest) => string.Concat(rest));
        }

        public static Parser<char, AomCode> Comment =>
            Map((comment) => new AomComment(comment),
                BlockComment(String("/*"), String("*/"))
            ).Select(x => (AomCode)x);

        public static Parser<char, string> AnyCharE => AnyCharExcept('(', ')').AtLeastOnceString();
        public static Parser<char, string> Expr => OneOf(Rec(() => Parentheses), AnyCharE);
        public static Parser<char, string> Parentheses => Char('(').Then(Rec(() => Expr).Until(Char(')'))).Select(x => $"({string.Concat(x)})");
        public static Parser<char, IEnumerable<AomCode>> SingleBlock => (Try(BlockDoWhile).Or(Try(BlockIfThenElse).Or(Try(BlockForWhile).Or(Try(BreakStatement).Or(Try(ReturnStatement).Or(Field)))))).Repeat(1);

        public static Parser<char, string> FieldType => OneOf(
            Try(String("f(1)")),
            Try(String("f(2)")),
            Try(String("f(3)")),
            Try(String("f(4)")),
            Try(String("f(5)")),
            Try(String("f(6)")),
            Try(String("f(8)")),
            Try(String("f(9)")),
            Try(String("f(12)")),
            Try(String("f(16)")),
            Try(String("f(32)")),
            Try(String("f(n)")),
            Try(String("f(N)")),
            Try(String("f(b2)")),
            Try(String("f(bitsToRead)")),
            Try(String("f(idLen)")),
            Try(String("f(leadingZeros)")),
            Try(String("f(time_offset_length)")),
            Try(String("f(tileBits)")),
            Try(String("f(OrderHintBits)")),
            Try(String("f(SUPERRES_DENOM_BITS)")),
            Try(String("f(TileRowsLog2+TileColsLog2)")),
            Try(String("f(w - 1)")),
            Try(String("L(1)")),
            Try(String("L(2)")),
            Try(String("L(3)")),
            Try(String("L(b2)")),
            Try(String("L(paletteBits)")),
            Try(String("L(w - 1)")),
            Try(String("L(n)")),
            Try(String("L(delta_q_rem_bits)")),
            Try(String("L(BitDepth)")),
            Try(String("L(cdef_bits)")),
            Try(String("L(SGRPROJ_PARAMS_BITS)")),
            Try(String("le(TileSizeBytes)")),
            Try(String("S()")),
            Try(String("su(1+6)")),
            Try(String("su(1+bitsToRead)")),
            Try(String("ns(maxWidth)")),
            Try(String("ns(maxHeight)")),
            Try(String("ns(numSyms - mk)")),
            Try(String("NS(PaletteSizeUV)")),
            Try(String("NS(numSyms - mk)")),
            Try(String("uvlc()")),
            Try(String("leb128()"))
            );

        public static Parser<char, string> FieldValue =>
            Map((operation, value) => $"{operation} {value}",
                SkipWhitespaces.Then(OneOf(String("="), String("+="), String("-="))),
                SkipWhitespaces.Then(Any.Until(EndOfLine)).Select(x => string.Concat(x))
                );


        public static Parser<char, string> AnyCharA => AnyCharExcept('[', ']').AtLeastOnceString();
        public static Parser<char, string> ExprA => OneOf(Rec(() => FieldArray), AnyCharA);
        public static Parser<char, string> FieldArray => Char('[').Then(Rec(() => ExprA).Until(Char(']'))).Select(x => $"[{string.Concat(x)}]");
        public static Parser<char, string> FieldArrays => FieldArray.SeparatedAndTerminated(SkipWhitespaces).Select(x => string.Concat(x));

        public static Parser<char, AomCode> Field =>
             Map((className, classParameter, classArray, increment, value, comment, type) => new AomField(className, classParameter, classArray, increment, value, comment, type),
                SkipWhitespaces.Then(Identifier),
                SkipWhitespaces.Then(Try(Parentheses).Optional()),
                SkipWhitespaces.Then(Try(FieldArrays).Optional()),
                OneOf(Try(String("++")), Try(String("--"))).Optional(),
                SkipWhitespaces.Then(Try(FieldValue).Optional()),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(FieldType).Optional())
             ).Select(x => (AomCode)x);

        public static Parser<char, AomCode> ReturnStatement =>
             Map((classParameter) => new AomReturn(classParameter),
                SkipWhitespaces.Then(String("return")).Then(Try(Any.Until(Try(EndOfLine).IgnoreResult())).Optional())
             ).Select(x => (AomCode)x);

        public static Parser<char, AomCode> BreakStatement => SkipWhitespaces.Then(String("break").ThenReturn((AomCode)new AomBreak()));

        public static Parser<char, AomCode> BlockIf =>
            Map((type, condition, comment, content) => new AomBlock(type, condition, comment, content),
                Try(String("if").Before(SkipWhitespaces).Before(Lookahead(Parentheses))).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (AomCode)x);
        public static Parser<char, AomCode> BlockElseIf =>
            Map((type, condition, comment, content) => new AomBlock(type, condition, comment, content),
                Try(String("else if").Before(SkipWhitespaces).Before(Lookahead(Parentheses))),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (AomCode)x);
        public static Parser<char, AomCode> BlockElse =>
            Map((type, condition, comment, content) => new AomBlock(type, condition, comment, content),
                Try(String("else")).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (AomCode)x);
        public static Parser<char, AomCode> BlockIfThenElse =>
            Map((blockIf, blockElseIf, blockElse) => new AomBlockIfThenElse(blockIf, blockElseIf, blockElse),
                SkipWhitespaces.Then(Try(BlockIf)),
                SkipWhitespaces.Then(Try(BlockElseIf).SeparatedAndOptionallyTerminated(SkipWhitespaces)).Optional(),
                SkipWhitespaces.Then(Try(BlockElse)).Optional()
            ).Select(x => (AomCode)x);

        public static Parser<char, AomCode> BlockForWhile =>
            Map((type, condition, comment, content) => new AomBlock(type, condition, comment, content),
                OneOf(
                    Try(String("for").Before(SkipWhitespaces).Before(Lookahead(Parentheses))),
                    Try(String("while").Before(SkipWhitespaces).Before(Lookahead(Parentheses)))
                    ).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (AomCode)x);

        public static Parser<char, AomCode> BlockDoWhile =>
            Map((type, comment, content, condition) => new AomBlock(type, condition, comment, content),
                String("do").Before(SkipWhitespaces).Before(Lookahead(SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))).Before(SkipWhitespaces.Before(String("while"))))).Before(SkipWhitespaces),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))),
                Try(SkipWhitespaces.Then(String("while")).Then(SkipWhitespaces.Then(Parentheses))).Optional()
            ).Select(x => (AomCode)x);

        public static Parser<char, AomCode> CodeBlock => Try(BlockIfThenElse).Or(Try(BlockDoWhile).Or(Try(BlockForWhile).Or(Try(BreakStatement).Or(Try(ReturnStatement).Or(Try(Field).Or(Comment))))));

        public static Parser<char, IEnumerable<AomCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));


        public static Parser<char, AomClass> AomClass =>
            Map((className, classParameter, fields, endOffset) => new AomClass(className, classParameter, fields, endOffset),
                SkipWhitespaces.Then(Identifier),
                SkipWhitespaces.Then(Parentheses.Optional()).Before(SkipWhitespaces),
                Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}')),
                CurrentOffset
            );

        public static Parser<char, IEnumerable<AomClass>> AomClasses => SkipWhitespaces.Then(AomClass.SeparatedAndOptionallyTerminated(SkipWhitespaces));
    }

    [SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
    public interface AomCode
    { }

    public class AomComment : AomCode
    {
        public AomComment()
        { }

        public AomComment(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; }
    }

    public class AomField : AomCode
    {
        public AomField()
        { }

        public AomField(string name, Maybe<string> parameter, Maybe<string> array, Maybe<string> increment, Maybe<string> value, Maybe<string> comment, Maybe<string> type)
        {
            Name = ClassType = name;
            Increment = increment.GetValueOrDefault();
            Parameter = parameter.GetValueOrDefault();
            FieldArray = array.GetValueOrDefault();
            Value = value.GetValueOrDefault();
            Comment = comment.GetValueOrDefault();
            Type = type.GetValueOrDefault();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string ClassType { get; set; }
        public string Value { get; set; }
        public string Parameter { get; set; }
        public string Increment { get; set; }
        public string FieldArray { get; set; }
        public string Comment { get; set; }

        public AomBlock Parent { get; internal set; }
        public bool MakeList { get; internal set; }
    }

    public class AomReturn : AomCode
    {
        public AomReturn()
        { }

        public AomReturn(Maybe<IEnumerable<char>> parameter)
        {
            Parameter = string.Concat(parameter.GetValueOrDefault());
        }

        public string Parameter { get; set; }

        public AomBlock Parent { get; internal set; }
        public bool MakeList { get; internal set; }
    }

    public class AomBreak : AomCode
    {
        public AomBreak()
        { }

        public AomBlock Parent { get; internal set; }
        public bool MakeList { get; internal set; }
    }

    public class AomClass : AomCode
    {
        public AomClass(string className, Maybe<string> classParameter, IEnumerable<AomCode> fields, int endOffset)
        {
            ClassName = className;
            ClassParameter = classParameter.GetValueOrDefault();
            Fields = fields;
            EndOffset = endOffset;
        }

        public string ClassName { get; }
        public string ClassParameter { get; set; }
        public IEnumerable<AomCode> Fields { get; }
        public int EndOffset { get; }

        public List<AomField> FlattenedFields { get; internal set; }
        public List<AomField> AddedFields { get; internal set; } = new List<AomField>();
        public List<AomField> RequiresDefinition { get; internal set; } = new List<AomField>();
        public string Syntax { get; set; }
    }

    public class AomBlock : AomCode
    {
        public string Type { get; }
        public string Condition { get; }
        public string Comment { get; }
        public IEnumerable<AomCode> Content { get; }
        public AomBlock Parent { get; internal set; }
        public List<AomField> RequiresAllocation { get; internal set; } = new List<AomField>();

        public AomBlock(string type, Maybe<string> condition, Maybe<string> comment, IEnumerable<AomCode> content)
        {
            this.Type = type;
            this.Condition = condition.GetValueOrDefault();
            this.Comment = comment.GetValueOrDefault();
            this.Content = content;
        }
    }

    public class AomBlockIfThenElse : AomCode
    {
        public AomBlockIfThenElse(AomCode blockIf, Maybe<IEnumerable<AomCode>> blockElseIf, Maybe<AomCode> blockElse)
        {
            BlockIf = blockIf;
            BlockElseIf = blockElseIf.GetValueOrDefault();
            if (BlockElseIf == null)
                BlockElseIf = new List<AomCode>();
            BlockElse = blockElse.GetValueOrDefault();
        }

        public AomCode BlockIf { get; }
        public AomCode BlockElse { get; }
        public IEnumerable<AomCode> BlockElseIf { get; set; }
        public AomBlock Parent { get; internal set; }
    }
}
