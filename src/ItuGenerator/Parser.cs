using Pidgin;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace ItuGenerator
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

        public static Parser<char, ItuCode> Comment =>
            Map((comment) => new ItuComment(comment),
                BlockComment(String("/*"), String("*/"))
            ).Select(x => (ItuCode)x);

        public static Parser<char, string> AnyCharE => AnyCharExcept('(', ')').AtLeastOnceString();
        public static Parser<char, string> Expr => OneOf(Rec(() => Parentheses), AnyCharE);
        public static Parser<char, string> Parentheses => Char('(').Then(Rec(() => Expr).Until(Char(')'))).Select(x => $"({string.Concat(x)})");
        public static Parser<char, IEnumerable<ItuCode>> SingleBlock => (Try(BlockDoWhile).Or(Try(BlockIfThenElse).Or(Try(BlockForWhile).Or(Field)))).Repeat(1);

        public static Parser<char, string> FieldCategory => OneOf(
            Try(String("All")), 
            Try(String("2 | 3 | 4")),
            Try(String("0 | 1")),
            Try(String("0 | 5")),
            Try(String("2 | 5")),
            Try(String("3 | 4")),
            Try(String("10")),
            Try(String("11")),
            Try(String("0")), 
            Try(String("1")), 
            Try(String("2")), 
            Try(String("3")), 
            Try(String("4")), 
            Try(String("5")),
            Try(String("6")),
            Try(String("7")),
            Try(String("8")),
            Try(String("9"))
            );
        public static Parser<char, string> FieldType => OneOf(
            Try(String("u(1) | ae(v)")),
            Try(String("u(3) | ae(v)")),
            Try(String("ue(v) | ae(v)")),
            Try(String("me(v) | ae(v)")),
            Try(String("se(v) | ae(v)")),
            Try(String("te(v) | ae(v)")),
            Try(String("f(1)")), 
            Try(String("u(1)")), 
            Try(String("u(2)")), 
            Try(String("u(3)")), 
            Try(String("u(4)")), 
            Try(String("u(5)")), 
            Try(String("u(6)")), 
            Try(String("u(7)")), 
            Try(String("u(8)")), 
            Try(String("u(10)")), 
            Try(String("u(20)")), 
            Try(String("u(16)")), 
            Try(String("u(24)")), 
            Try(String("u(32)")), 
            Try(String("i(32)")), // ?
            Try(String("i(v)")), // ?
            Try(String("u(33)")), 
            Try(String("u(34)")), 
            Try(String("u(35)")), 
            Try(String("u(43)")), 
            Try(String("u(128)")), 
            Try(String("b(8)")),
            Try(String("f(8)")),
            Try(String("f(16)")),
            Try(String("ue(v)")),
            Try(String("se(v)")),
            Try(String("st(v)")),
            Try(String("u(v)")),
            Try(String("ae(v)")),
            Try(String("ce(v)"))
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

        public static Parser<char, ItuCode> Field =>
             Map((className, classParameter, classArray, increment, value, comment, category, type) => new ItuField(className, classParameter, classArray, increment, value, comment, category, type),
                SkipWhitespaces.Then(Identifier),
                SkipWhitespaces.Then(Try(Parentheses).Optional()), 
                SkipWhitespaces.Then(Try(FieldArrays).Optional()),
                OneOf(Try(String("++")), Try(String("--"))).Optional(),
                SkipWhitespaces.Then(Try(FieldValue).Optional()),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(FieldCategory).Optional()),
                SkipWhitespaces.Then(Try(FieldType).Optional())
             ).Select(x => (ItuCode)x);

        public static Parser<char, ItuCode> BlockIf =>
            Map((type, condition, comment, content) => new ItuBlock(type, condition, comment, content),
                Try(String("if").Before(SkipWhitespaces).Before(Lookahead(Parentheses))).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (ItuCode)x);
        public static Parser<char, ItuCode> BlockElseIf =>
            Map((type, condition, comment, content) => new ItuBlock(type, condition, comment, content),
                Try(String("else if").Before(SkipWhitespaces).Before(Lookahead(Parentheses))),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (ItuCode)x);
        public static Parser<char, ItuCode> BlockElse =>
            Map((type, condition, comment, content) => new ItuBlock(type, condition, comment, content),
                Try(String("else")).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (ItuCode)x);
        public static Parser<char, ItuCode> BlockIfThenElse =>
            Map((blockIf, blockElseIf, blockElse) => new ItuBlockIfThenElse(blockIf, blockElseIf, blockElse),
                SkipWhitespaces.Then(Try(BlockIf)),
                SkipWhitespaces.Then(Try(BlockElseIf).SeparatedAndOptionallyTerminated(SkipWhitespaces)).Optional(),
                SkipWhitespaces.Then(Try(BlockElse)).Optional()
            ).Select(x => (ItuCode)x);



        public static Parser<char, ItuCode> BlockForWhile =>
            Map((type, condition, comment, content) => new ItuBlock(type, condition, comment, content),
                OneOf(
                    Try(String("for").Before(SkipWhitespaces).Before(Lookahead(Parentheses))),
                    Try(String("while").Before(SkipWhitespaces).Before(Lookahead(Parentheses)))
                    ).Before(SkipWhitespaces),
                Try(Parentheses).Optional(),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
            ).Select(x => (ItuCode)x);

        public static Parser<char, ItuCode> BlockDoWhile =>
            Map((type, comment, content, condition) => new ItuBlock(type, condition, comment, content),
                String("do").Before(SkipWhitespaces).Before(Lookahead(SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))).Before(SkipWhitespaces.Before(String("while"))))).Before(SkipWhitespaces),
                SkipWhitespaces.Then(Try(BlockComment(String("/*"), String("*/"))).Optional()),
                SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))),
                Try(SkipWhitespaces.Then(String("while")).Then(SkipWhitespaces.Then(Parentheses))).Optional()
            ).Select(x => (ItuCode)x);

        public static Parser<char, ItuCode> CodeBlock => Try(BlockIfThenElse).Or(Try(BlockDoWhile).Or(Try(BlockForWhile).Or(Try(Field).Or(Comment))));

        public static Parser<char, IEnumerable<ItuCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));


        public static Parser<char, ItuClass> ItuClass =>
            Map((className, classParameter, fields, endOffset) => new ItuClass(className, classParameter, fields, endOffset),
                SkipWhitespaces.Then(Identifier),
                SkipWhitespaces.Then(Parentheses.Optional()).Before(SkipWhitespaces),
                Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}')),
                CurrentOffset
            );

        public static Parser<char, IEnumerable<ItuClass>> ItuClasses => SkipWhitespaces.Then(ItuClass.SeparatedAndOptionallyTerminated(SkipWhitespaces));
    }

    [SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
    public interface ItuCode
    { }

    public class ItuComment : ItuCode
    {
        public ItuComment()
        { }

        public ItuComment(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; }
    }

    public class ItuField : ItuCode
    {
        public ItuField()
        { }


        public ItuField(string name, Maybe<string> parameter, Maybe<string> array, Maybe<string> increment, Maybe<string> value, Maybe<string> comment, Maybe<string> category, Maybe<string> type)
        {
            Name = name;
            Increment = increment.GetValueOrDefault();
            Parameter = parameter.GetValueOrDefault();
            FieldArray = array.GetValueOrDefault();
            Value = value.GetValueOrDefault();
            Comment = comment.GetValueOrDefault();
            Category = category.GetValueOrDefault();
            Type = type.GetValueOrDefault();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Parameter { get; set; }
        public string Increment { get; set; }
        public string FieldArray { get; set; }
        public string Comment { get; set; }
        public string Category { get; set; }

        public ItuBlock Parent { get; internal set; }
    }

    public class ItuClass : ItuCode
    {
        public ItuClass(string className, Maybe<string> classParameter, IEnumerable<ItuCode> fields, int endOffset)
        {
            ClassName = className;
            ClassParameter = classParameter.GetValueOrDefault();
            Fields = fields;
            EndOffset = endOffset;
        }

        public string ClassName { get; }
        public string ClassParameter { get; }
        public IEnumerable<ItuCode> Fields { get; }
        public int EndOffset { get; }

        public List<ItuField> FlattenedFields { get; internal set; }
        public List<ItuField> AddedFields { get; internal set; } = new List<ItuField>();
        public List<ItuField> RequiresDefinition { get; internal set; } = new List<ItuField>();
        public string Syntax { get; set; }
    }

    public class ItuBlock : ItuCode
    {
        public string Type { get; }
        public string Condition { get; }
        public string Comment { get; }
        public IEnumerable<ItuCode> Content { get; }
        public ItuBlock Parent { get; internal set; }
        public List<ItuField> RequiresAllocation { get; internal set; } = new List<ItuField>();

        public ItuBlock(string type, Maybe<string> condition, Maybe<string> comment, IEnumerable<ItuCode> content)
        {
            this.Type = type;
            this.Condition = condition.GetValueOrDefault();
            this.Comment = comment.GetValueOrDefault();
            this.Content = content;
        }
    }

    public class ItuBlockIfThenElse : ItuCode
    {
        public ItuBlockIfThenElse(ItuCode blockIf, Maybe<IEnumerable<ItuCode>> blockElseIf, Maybe<ItuCode> blockElse)
        {
            BlockIf = blockIf;
            BlockElseIf = blockElseIf.GetValueOrDefault();
            if (BlockElseIf == null)
                BlockElseIf = new List<ItuCode>();
            BlockElse = blockElse.GetValueOrDefault();
        }

        public ItuCode BlockIf { get; }
        public ItuCode BlockElse { get; }
        public IEnumerable<ItuCode> BlockElseIf { get; set; } 
        public ItuBlock Parent { get; internal set; }
    }
}
