using Pidgin;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using static Pidgin.Parser<char>;
using static Pidgin.Parser;

namespace ConsoleApp;

[SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
public abstract class PseudoCode
{
}

public class PseudoClass : PseudoCode
{
    public string BoxName { get; }
    public string BoxType { get; }
    public IEnumerable<PseudoCode> Fields { get; }
    public Maybe<IEnumerable<(string Name, Maybe<string> Value)>> Parameters { get; }

    public PseudoClass(string boxName, string boxType, Maybe<IEnumerable<(string Name, Maybe<string> Value)>> parameters, IEnumerable<PseudoCode> fields)
    {
        BoxName = boxName;
        BoxType = boxType;
        Parameters = parameters;
        Fields = fields;
    }

}

public class PseudoField : PseudoCode
{
    public PseudoField(string type, string name, string value, Maybe<string> comment)
    {
        Type = type;
        Name = name;
        Value = value;
        Comment = comment;
    }

    public string Type { get; }
    public string Name { get; }
    public string Value { get; }
    public Maybe<string> Comment { get; }
}

public class PseudoMethod : PseudoCode
{
    public PseudoMethod(string name, string value, Maybe<string> comment)
    {
        Name = name;
        Value = value;
        Comment = comment;
    }

    public string Name { get; }
    public string Value { get; }
    public Maybe<string> Comment { get; }
}

public class PseudoBlock : PseudoCode
{
    public PseudoBlock(string type, Maybe<string> condition, IEnumerable<PseudoCode> content)
    {
        Type = type;
        Condition = condition;
        Content = content;
    }

    public string Type { get; }
    public Maybe<string> Condition { get; }
    public IEnumerable<PseudoCode> Content { get; }
}

partial class Program
{
    public static Parser<char, string> Identifier =>
        Letter.Then(Token(c => char.IsLetterOrDigit(c) || c == '_').Labelled("letter or digit or _").ManyString(), (first, rest) => first + rest);

    public static Parser<char, string> IdentifierWithSpace =>
        Letter.Then(Token(c => char.IsLetterOrDigit(c) || c == '_' || c == ' ').Labelled("letter or digit or _ or space").ManyString(), (first, rest) => first + rest);

    public static Parser<char, string> BoxType =>
        Char('\'').Then(IdentifierWithSpace).Before(Char('\''));

    public static Parser<char, string> OldBoxType =>
        Char('\'').Then(Char('!')).Then(IdentifierWithSpace).Before(Char('\'')).Before(Char(',')).Before(SkipWhitespaces);

    public static Parser<char, string> BoxName =>
        Identifier.Labelled("box name");

    public static Parser<char, string> Parameter =>
        SkipWhitespaces.Then(LetterOrDigit.ManyString());

    public static Parser<char, string> ParameterValue =>
        Char('=').Then(SkipWhitespaces).Then(LetterOrDigit.ManyString());

    public static Parser<char, (string Name, Maybe<string> Value)> ParameterFull =>
        Map((name, value) => (name, value),
            Parameter.Before(SkipWhitespaces),
            Try(ParameterValue).Optional()
        );

    public static Parser<char, IEnumerable<(string Name, Maybe<string> Value)>> Parameters =>
        ParameterFull.SeparatedAndOptionallyTerminated(Char(','));

    private static Parser<char, string> Parentheses =>
        Char('(').Then(Rec(() => Expr).Until(Char(')'))).Select(x => $"({string.Concat(x)})");

    private static Parser<char, string> Expr =>
        OneOf(
            Rec(() => Parentheses),
            AnyCharE
            );

    private static Parser<char, string> AnyCharE =>
            AnyCharExcept('(', ')').AtLeastOnceString();

    public static Parser<char, string> LineComment<T>(Parser<char, T> lineCommentStart)
    {
        if (lineCommentStart == null)
        {
            throw new ArgumentNullException(nameof(lineCommentStart));
        }

        var eol = Try(EndOfLine).IgnoreResult();
        return lineCommentStart
            .Then(Any.Until(End.Or(eol)), (first, rest) => string.Concat(rest))
            .Labelled("line comment");
    }

    public static Parser<char, string> SkipBlockComment<T, U>(Parser<char, T> blockCommentStart, Parser<char, U> blockCommentEnd)
    {
        if (blockCommentStart == null)
        {
            throw new ArgumentNullException(nameof(blockCommentStart));
        }

        if (blockCommentEnd == null)
        {
            throw new ArgumentNullException(nameof(blockCommentEnd));
        }

        return blockCommentStart
            .Then(Any.SkipUntil(blockCommentEnd), (first, rest) => string.Concat(rest))
            .Labelled("block comment");
    }

    public static Parser<char, string> FieldType =>
        OneOf(
            Try(String("unsigned int(64)")),
            Try(String("template int(32)[9]")),
            Try(String("unsigned int(32)")),
            Try(String("unsigned int(16)")),
            Try(String("unsigned int(8)[length]")),
            Try(String("unsigned int(8)")),
            Try(String("unsigned int(5)[3]")),
            Try(String("unsigned int(4)")),
            Try(String("unsigned int(3)")),
            Try(String("unsigned int(2)")),
            Try(String("unsigned int(1)")),
            Try(String("const unsigned int(32)[2]")),
            Try(String("template int(32)")),
            Try(String("template int(16)")),
            Try(String("template unsigned int(30)")),
            Try(String("int(16)")),
            Try(String("int(32)")),
            Try(String("const bit(16)")),
            Try(String("const bit(1)")),
            Try(String("bit(2)")),
            Try(String("bit(7)")),
            Try(String("utf8string")),
            Try(String("utfstring")),
            Try(String("bit(32)[6]")),
            Try(String("uint(32)")),
            Try(String("uint(64)")),
            Try(String("uint(8)")),
            Try(String("unsigned int(6)")),
            Try(String("signed int(32)")),
            Try(String("signed int (16)")),
            Try(String("signed int (8)")),
            Try(String("signed int(64)")),
            Try(String("int(4)")),
            Try(String("Box[]")),
            Try(String("Box")),
            Try(String("SchemeTypeBox")),
            Try(String("SchemeInformationBox")),
            Try(String("ItemPropertyContainerBox")),
            Try(String("ItemPropertyAssociationBox")),
            Try(String("char")),
            Try(String("int")),
            Try(String("loudness"))
            )
        .Labelled("field type");

    public static Parser<char, string> FieldName =>
        Identifier.Labelled("field name");

    public static Parser<char, PseudoCode> Field =>
        Map((type, name, value, comment) => new PseudoField(type, name, string.Concat(value), comment),
            FieldType.Before(SkipWhitespaces),
            FieldName.Before(SkipWhitespaces),
            Any.Until(Char(';')).Before(SkipWhitespaces),
            Try(LineComment(String("//"))).Optional()
        ).Select(x => (PseudoCode)x);

    public static Parser<char, string> MethodName =>
        Identifier.Labelled("field name");

    public static Parser<char, PseudoCode> Method =>
        Map((name, value, comment) => new PseudoMethod(name, string.Concat(value), comment),
            MethodName.Before(Char('(')),
            Any.Until(Char(')')).Before(Char(';')).Before(SkipWhitespaces),
            Try(LineComment(String("//"))).Or(Try(SkipBlockComment(String("/*"), String("*/")))).Optional()
        ).Select(x => (PseudoCode)x);

    public static Parser<char, PseudoCode> Block =>
        Map((type, condition, content) => new PseudoBlock(type, condition, content),
            OneOf(Try(String("else if")), Try(String("if")), Try(String("else")), Try(String("for"))),
            Try(SkipWhitespaces.Then(Parentheses)).Optional(),
            SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
        ).Select(x => (PseudoCode)x);


    public static Parser<char, IEnumerable<PseudoCode>> SingleBlock => Try(Method).Or(Field).Repeat(1);

    public static Parser<char, PseudoCode> CodeBlock => Try(LineComment(String("//")).Then(SkipWhitespaces)).Optional().Then(Try(Block).Or(Try(Method).Or(Field)));
    public static Parser<char, IEnumerable<PseudoCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));

    public static Parser<char, PseudoClass> Box =>
        Map((boxName, boxType, parameters, fields) => new PseudoClass(boxName, boxType, parameters, fields),
            Try(String("aligned(8)")).Optional().Then(SkipWhitespaces).Then(String("class")).Then(SkipWhitespaces).Then(Identifier).Before(SkipWhitespaces)
                .Before(Try(String("()")).Or(Try(String("(bit(24) flags)"))).Optional())
                .Before(SkipWhitespaces)
                .Before(String("extends"))
                .Before(SkipWhitespaces),
            BoxName.Then(SkipWhitespaces).Then(Char('(')).Then(Try(OldBoxType).Optional()).Then(BoxType),
            Try(Char(',').Then(Parameters)).Optional().Before(Char(')')).Before(SkipWhitespaces),
            Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}'))
        );


    static void Main(string[] args)
    {
        HelloFrom("Generated Code");
        using (var json = File.OpenRead("boxes.json"))
        using (JsonDocument document = JsonDocument.Parse(json, new JsonDocumentOptions()))
        {
            int success = 0;
            int fail = 0;

            foreach (JsonElement element in document.RootElement.GetProperty("entries").EnumerateArray())
            {
                string sample = element.GetProperty("syntax").GetString()!;

                try
                {
                    var result = Box.ParseOrThrow(sample);
                    Console.WriteLine($"Succeeded parsing: {element.GetProperty("fourcc").GetString()}");
                    success++;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Failed to parse: {element.GetProperty("fourcc").GetString()}");
                    fail++;
                }
            }

            Console.WriteLine($"Succeessful: {success}, Failed: {fail}, Total: {success + fail}"); 
        }
    }

    static partial void HelloFrom(string name);
}

