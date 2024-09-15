using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace ConsoleApp;

partial class Program
{
    public static Parser<char, string> Identifier =>
        Letter.Then(Token(c => char.IsLetterOrDigit(c) || c == '_').Labelled("letter or digit").ManyString(), (first, rest) => first + rest);

    public static Parser<char, string> BoxType =>
        Char('\'').Then(Identifier).Before(Char('\''));

    public static Parser<char, string> FieldType =>
        OneOf(
            Try(String("unsigned int(64)")),
            Try(String("template int(32)[9]")), 
            Try(String("unsigned int(32)")),
            Try(String("const unsigned int(32)[2]")),
            Try(String("template int(32)")), 
            Try(String("int(16)")), 
            Try(String("const bit(16)")), 
            Try(String("bit(32)[6]")))
        .Labelled("field type");

    public static Parser<char, string> FieldName =>
        Identifier.Labelled("field name");

    public static Parser<char, string> BoxName =>
        Identifier.Labelled("box name");

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

    public static Parser<char, (string Type, string Name, Maybe<string> Value, Maybe<Unit> NoValue, Maybe<string> Comment)> Field =>
        Map((type, name, value, novalue, comment) => (type, name, value, novalue, comment),
            FieldType.Before(SkipWhitespaces),
            FieldName.Before(SkipWhitespaces),
            Try(Char('=').Then(SkipWhitespaces).Then(Any.Until(Char(';')), (first, rest) => string.Concat(rest)).Before(SkipWhitespaces)).Optional(),
            Try(Char(';').Then(SkipWhitespaces)).Optional(),
            Try(LineComment(String("//"))).Optional()
        );

    public static Parser<char, IEnumerable<(string Type, string Name, Maybe<string> Value, Maybe<Unit> NoValue, Maybe<string> Comment)>> Fields =>
        Field.SeparatedAndOptionallyTerminated(SkipWhitespaces);

    public static Parser<char, string> Parameter =>
        SkipWhitespaces.Then(LetterOrDigit.ManyString());

    public static Parser<char, IEnumerable<string>> Parameters =>
        Parameter.SeparatedAndOptionallyTerminated(Char(','));

    public static Parser<char, (string BoxName, string BoxType, IEnumerable<(string Type, string Name, Maybe<string> Value, Maybe<Unit> NoValue, Maybe<string> Comment)> Fields)> Box =>
        Map((boxName, boxType, fields) => (boxName, boxType, fields),
            String("aligned(8)").Then(SkipWhitespaces).Then(String("class")).Then(SkipWhitespaces).Then(Identifier).Before(SkipWhitespaces).Before(String("extends")).Before(SkipWhitespaces),
            BoxName.Then(SkipWhitespaces).Then(Char('(')).Then(BoxType).Before(Char(',')).Before(Parameters).Before(Char(')')).Before(SkipWhitespaces),
            Char('{').Then(SkipWhitespaces).Then(Fields).Before(Char('}'))
        );

    static void Main(string[] args)
    {
        HelloFrom("Generated Code");

		string code =
//@"aligned(8) class MovieHeaderBox extends FullBox('mvhd', version, 0) {
//	if (version==1) {
//		unsigned int(64)	creation_time;
//		unsigned int(64)	modification_time;
//		unsigned int(32)	timescale;
//		unsigned int(64)	duration;
//	} else { // version==0
//		unsigned int(32)	creation_time;
//		unsigned int(32)	modification_time;
//		unsigned int(32)	timescale;
//		unsigned int(32)	duration;
//	}
//	template int(32)	rate = 0x00010000;	// typically 1.0
//	template int(16)	volume = 0x0100;	// typically, full volume
//	const bit(16)	reserved = 0;
//	const unsigned int(32)[2]	reserved = 0;
//	template int(32)[9]	matrix =
//		{ 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };
//		// Unity matrix
//	bit(32)[6]	pre_defined = 0;
//	unsigned int(32)	next_track_ID;
//}";
@"aligned(8) class MovieHeaderBox extends FullBox('mvhd', version, 0) {
		unsigned int(64)	creation_time;
		unsigned int(64)	modification_time;
		unsigned int(32)	timescale; 
		unsigned int(64)	duration;
        template int(32)	rate;	// typically 1.0
        const unsigned int(32)[2]	reserved;
        template int(32)[9]	matrix =
		     { 0x00010000,0,0,0,0x00010000,0,0,0,0x40000000 };
		     // Unity matrix
}";




        var result = Box.ParseOrThrow(code);

        Console.WriteLine($"Box Type: {result.BoxType}");
        foreach (var field in result.Fields)
        {
            Console.WriteLine($"Field: {field.Type} {field.Name}");
        }





    }

    static partial void HelloFrom(string name);
}