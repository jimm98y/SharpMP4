using Pidgin;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using static Pidgin.Parser<char>;
using static Pidgin.Parser;

namespace ConsoleApp;

[SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
public abstract class PseudoCode
{
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

    public static Parser<char, string> BoxName =>
        Identifier.Labelled("box name");

    public static Parser<char, string> Parameter =>
        SkipWhitespaces.Then(LetterOrDigit.ManyString());

    public static Parser<char, IEnumerable<string>> Parameters =>
        Parameter.SeparatedAndOptionallyTerminated(Char(','));

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

    public static Parser<char, string> FieldType =>
        OneOf(
            Try(String("unsigned int(64)")),
            Try(String("template int(32)[9]")),
            Try(String("unsigned int(32)")),
            Try(String("unsigned int(16)")),
            Try(String("const unsigned int(32)[2]")),
            Try(String("template int(32)")),
            Try(String("template int(16)")),
            Try(String("int(16)")),
            Try(String("const bit(16)")),
            Try(String("utf8string")),
            Try(String("bit(32)[6]")))
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
            Try(LineComment(String("//"))).Optional()
        ).Select(x => (PseudoCode)x);

    public static Parser<char, PseudoCode> Block =>
        Map((type, condition, content) => new PseudoBlock(type, condition, content),
            OneOf(Try(String("else if")), Try(String("if")), Try(String("else"))).Before(SkipWhitespaces),
            Try(Parentheses).Optional(),
            SkipWhitespaces.Then(Rec(() => CodeBlocks).Between(Char('{'), Char('}')))
        ).Select(x => (PseudoCode)x);


    public static Parser<char, PseudoCode> CodeBlock => Try(LineComment(String("//")).Then(SkipWhitespaces)).Optional().Then(Block.Or(Field).Or(Method));
    public static Parser<char, IEnumerable<PseudoCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));

    public static Parser<char, (string BoxName, string BoxType, IEnumerable<PseudoCode> Fields)> Box =>
        Map((boxName, boxType, fields) => (boxName, boxType, fields),
            String("aligned(8)").Then(SkipWhitespaces).Then(String("class")).Then(SkipWhitespaces).Then(Identifier).Before(SkipWhitespaces).Before(String("extends")).Before(SkipWhitespaces),
            BoxName.Then(SkipWhitespaces).Then(Char('(')).Then(BoxType).Before(Char(',')).Before(Parameters).Before(Char(')')).Before(SkipWhitespaces),
            Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}'))
        );


    static void Main(string[] args)
    {
        HelloFrom("Generated Code");

        string code =
/*@"aligned(8) class MovieHeaderBox extends FullBox('mvhd', version, 0) {
	if (version==1) {
		unsigned int(64)	creation_time;
		unsigned int(64)	modification_time;
		unsigned int(32)	timescale;
		unsigned int(64)	duration;
	} else { // version==0
		unsigned int(32)	creation_time;
		unsigned int(32)	modification_time;
		unsigned int(32)	timescale;
		unsigned int(32)	duration;
	}
	template int(32)	rate = 0x00010000;	// typically 1.0
	template int(16)	volume = 0x0100;	// typically, full volume
	const bit(16)	reserved = 0;
	const unsigned int(32)[2]	reserved = 0;
	template int(32)[9]	matrix =
		{ 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };
		// Unity matrix
	bit(32)[6]	pre_defined = 0;
	unsigned int(32)	next_track_ID;
}";*/
@"aligned(8) class ItemInfoEntry
		extends FullBox('infe', version, flags) {
	if ((version == 0) || (version == 1)) {
		unsigned int(16) item_ID;
		unsigned int(16) item_protection_index;
		utf8string item_name;
		utf8string content_type;
		utf8string content_encoding; //optional
	}
	if (version == 1) {
		unsigned int(32) extension_type; //optional
		ItemInfoExtension(extension_type); //optional
	}
	if (version >= 2) {
		if (version == 2) {
			unsigned int(16) item_ID;
		} else if (version == 3) {
			unsigned int(32) item_ID;
		}
		unsigned int(16) item_protection_index;
		unsigned int(32) item_type;
		utf8string item_name;
		if (item_type=='mime') {
			utf8string content_type;
			utf8string content_encoding; //optional
		} else if (item_type == 'uri ') {
			utf8string item_uri_type;
		}
	}
}";


        var result = Box.ParseOrThrow(code);

        Console.WriteLine($"Box Type: {result.BoxType}");
        //foreach (var field in result.Fields)
        //{
        //    Console.WriteLine($"Field: {field.Type} {field.Name}");
        //}





    }

    static partial void HelloFrom(string name);
}

