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
    public Maybe<string> ClassType { get; }
    public IEnumerable<PseudoCode> Fields { get; }
    public Maybe<string> Alignment { get; }
    public Maybe<PseudoExtendedClass> Extended { get; }

    public PseudoClass(
        Maybe<string> alignment, 
        string boxName, 
        Maybe<string> classType,
        Maybe<PseudoExtendedClass> extended,
        IEnumerable<PseudoCode> fields)
    {
        BoxName = boxName;
        ClassType = classType;
        Fields = fields;
        Alignment = alignment;
        Extended = extended;
    }

}

public class PseudoExtendedClass : PseudoCode
{
    public Maybe<string> OldBoxType { get; }
    public Maybe<string> BoxType { get; }
    public Maybe<Maybe<IEnumerable<(string Name, Maybe<string> Value)>>> Parameters { get; }
    public Maybe<string> ExtendedBoxName { get; }

    public PseudoExtendedClass(
        Maybe<string> oldBoxType,
        Maybe<string> boxType,
        Maybe<string> extendedBoxName,
        Maybe<Maybe<IEnumerable<(string Name, Maybe<string> Value)>>> parameters)
    {
        OldBoxType = oldBoxType;
        BoxType = boxType;
        Parameters = parameters;
        ExtendedBoxName = extendedBoxName;
    }

}

public class PseudoField : PseudoCode
{
    public PseudoField(string type, Maybe<string> name, string value, Maybe<string> comment)
    {
        Type = type;
        Name = name;
        Value = value;
        Comment = comment;
    }

    public string Type { get; }
    public Maybe<string> Name { get; }
    public string Value { get; }
    public Maybe<string> Comment { get; }
}


public class PseudoComment : PseudoCode
{
    public PseudoComment(string comment)
    {
        Comment = comment;
    }

    public string Comment { get; }
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
    public PseudoBlock(string type, Maybe<string> condition, Maybe<string> comment, IEnumerable<PseudoCode> content)
    {
        Type = type;
        Condition = condition;
        Content = content;
        Comment = comment;
    }

    public string Type { get; }
    public Maybe<string> Condition { get; }
    public Maybe<string> Comment { get; }
    public IEnumerable<PseudoCode> Content { get; }
}

public class PseudoRepeatingBlock : PseudoCode
{
    public PseudoRepeatingBlock(IEnumerable<PseudoCode> content, IEnumerable<char> array)
    {
        Content = content;
        Array = string.Concat(array);
    }

    public string Array { get; }
    public IEnumerable<PseudoCode> Content { get; }
}

partial class Program
{
    public static Parser<char, string> Identifier =>
        LetterOrDigit.Then(Token(c => char.IsLetterOrDigit(c) || c == '_').Labelled("letter or digit or _").ManyString(), (first, rest) => first + rest);

    public static Parser<char, string> IdentifierWithSpace =>
        LetterOrDigit.Then(Token(c => char.IsLetterOrDigit(c) || c == '_' || c == ' ').Labelled("letter or digit or _ or space").ManyString(), (first, rest) => first + rest);

    public static Parser<char, string> BoxType =>
        Char('\'').Then(IdentifierWithSpace).Before(Char('\''));

    public static Parser<char, string> OldBoxType =>
        Char('\'').Then(Char('!')).Then(IdentifierWithSpace).Before(Char('\'')).Before(Char(',')).Before(SkipWhitespaces);

    public static Parser<char, string> BoxName =>
        Identifier.Labelled("box name");

    public static Parser<char, char> LetterOrDigitOrUnderscore { get; } = Token(c => char.IsLetterOrDigit(c) || c == '_').Labelled("letter or digit or underscore");
    public static Parser<char, string> Parameter =>
        SkipWhitespaces.Then(LetterOrDigitOrUnderscore.ManyString());

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
            Try(String("unsigned int(48)")),
            Try(String("template int(32)[9]")),
            Try(String("unsigned int(32)")),
            Try(String("unsigned int(24)")),
            Try(String("unsigned int(29)")),
            Try(String("unsigned int(26)")),
            Try(String("unsigned int(16)")),
            Try(String("unsigned int(15)")),
            Try(String("unsigned int(12)")),
            Try(String("unsigned int(10)")),
            Try(String("unsigned int(15)")),
            Try(String("unsigned int(8)[length]")),
            Try(String("unsigned int(8)")),
            Try(String("unsigned int(7)")),
            Try(String("unsigned int(5)[3]")),
            Try(String("unsigned int(5)")),
            Try(String("unsigned int(4)")),
            Try(String("unsigned int(3)")),
            Try(String("unsigned int(2)")),
            Try(String("unsigned int(1)")),
            Try(String("unsigned int (32)")),
            Try(String("unsigned int(f(pattern_size_code))")),
            Try(String("unsigned int(f(index_size_code))")),
            Try(String("unsigned int(f(count_size_code))")),
            Try(String("unsigned int(base_offset_size*8)")),
            Try(String("unsigned int(offset_size*8)")),
            Try(String("unsigned int(length_size*8)")),
            Try(String("unsigned int(index_size*8)")),
            Try(String("unsigned int(field_size)")),
            Try(String("unsigned int((length_size_of_traf_num+1) * 8)")),
            Try(String("unsigned int((length_size_of_trun_num+1) * 8)")),
            Try(String("unsigned int((length_size_of_sample_num+1) * 8)")),
            Try(String("unsigned int(8*size-64)")),
            Try(String("unsigned int(8)[16]")),
            Try(String("const unsigned int(32)[2]")),
            Try(String("const unsigned int(32)[3]")),
            Try(String("const unsigned int(32)")),
            Try(String("const unsigned int(16)")),
            Try(String("const unsigned int(26)")),
            Try(String("template int(32)")),
            Try(String("template int(16)")),
            Try(String("template unsigned int(30)")),
            Try(String("template unsigned int(32)")),
            Try(String("template unsigned int(16)[3]")),
            Try(String("template unsigned int(16)")),
            Try(String("template unsigned int(8)")),
            Try(String("int(16)")),
            Try(String("int(32)")),
            Try(String("const bit(16)")),
            Try(String("const bit(1)")),
            Try(String("bit(1)")),
            Try(String("bit(2)")),
            Try(String("bit(3)")),
            Try(String("bit(4)")),
            Try(String("bit(5)")),
            Try(String("bit(6)")),
            Try(String("bit(7)")),
            Try(String("bit(8)")),
            Try(String("bit(16)")),
            Try(String("bit(31)")),
            Try(String("bit(8 ceil(size / 8) \u2013 size)")),
            Try(String("bit(8* ps_nalu_length)")),
            Try(String("utf8string")),
            Try(String("utfstring")),
            Try(String("utf8list")),
            Try(String("boxstring")),
            Try(String("string")),
            Try(String("bit(32)[6]")),
            Try(String("uint(32)")),
            Try(String("uint(16)")),
            Try(String("uint(64)")),
            Try(String("uint(8)")),
            Try(String("unsigned int(6)")),
            Try(String("signed int(32)")),
            Try(String("signed int (16)")),
            Try(String("signed int(16)")),
            Try(String("signed int (8)")),
            Try(String("signed int(64)")),
            Try(String("signed   int(32)")),
            Try(String("signed   int(8)")),
            Try(String("int(4)")),
            Try(String("Box[]")),
            Try(String("Box")),
            Try(String("SchemeTypeBox")),
            Try(String("SchemeInformationBox")),
            Try(String("ItemPropertyContainerBox")),
            Try(String("ItemPropertyAssociationBox")),
            Try(String("char")),
            Try(String("int")),
            Try(String("loudness")),
            Try(String("ICC_profile")),
            Try(String("OriginalFormatBox(fmt)")),
            Try(String("DataEntryBaseBox(entry_type, entry_flags)")),
            Try(String("ItemInfoEntry[ entry_count ]")),
            Try(String("TypeCombinationBox")),
            Try(String("FilePartitionBox")),
            Try(String("FECReservoirBox")),
            Try(String("FileReservoirBox")),
            Try(String("PartitionEntry")),
            Try(String("FDSessionGroupBox")),
            Try(String("GroupIdToNameBox")),
            Try(String("base64string")),
            Try(String("ProtectionSchemeInfoBox")),
            Try(String("SingleItemTypeReferenceBox")),
            Try(String("SingleItemTypeReferenceBoxLarge")),
            Try(String("HandlerBox(handler_type)")),
            Try(String("PrimaryItemBox")),
            Try(String("DataInformationBox")),
            Try(String("ItemLocationBox")),
            Try(String("ItemProtectionBox")),
            Try(String("ItemInfoBox")),
            Try(String("IPMPControlBox")),
            Try(String("ItemReferenceBox")),
            Try(String("ItemDataBox")),
            Try(String("TrackReferenceTypeBox []")),
            Try(String("MetadataKeyBox[]")),
            Try(String("TierInfoBox")),
            Try(String("MultiviewRelationAttributeBox")),
            Try(String("TierBitRateBox")),
            Try(String("BufferingBox")),
            Try(String("MultiviewSceneInfoBox")),
            Try(String("MVDDecoderConfigurationRecord")),
            Try(String("MVDDepthResolutionBox")),
            Try(String("MVCDecoderConfigurationRecord()")),
            Try(String("AVCDecoderConfigurationRecord()")),
            Try(String("HEVCDecoderConfigurationRecord()")),
            Try(String("LHEVCDecoderConfigurationRecord()")),
            Try(String("SVCDecoderConfigurationRecord()")),
            Try(String("HEVCTileTierLevelConfigurationRecord()")),
            Try(String("EVCDecoderConfigurationRecord()")),
            Try(String("VvcDecoderConfigurationRecord()")),
            Try(String("EVCSliceComponentTrackConfigurationRecord()")),
            Try(String("SampleGroupDescriptionEntry (grouping_type)")),
            Try(String("Descriptor")),
            Try(String("WebVTTConfigurationBox")),
            Try(String("WebVTTSourceLabelBox")),
            Try(String("AVCConfigurationBox")),
            Try(String("OperatingPointsRecord")),
            Try(String("VvcSubpicIDEntry")),
            Try(String("VvcSubpicOrderEntry")),
            Try(String("URIInitBox")),
            Try(String("URIbox")),
            Try(String("CleanApertureBox")),
            Try(String("PixelAspectRatioBox")),
            Try(String("DownMixInstructions()")),
            Try(String("DRCCoefficientsBasic()")),
            Try(String("DRCInstructionsBasic()")),
            Try(String("DRCCoefficientsUniDRC()")),
            Try(String("DRCInstructionsUniDRC()")),
            Try(String("HEVCConfigurationBox")),
            Try(String("LHEVCConfigurationBox")),
            Try(String("AVCConfigurationBox")),
            Try(String("SVCConfigurationBox")),
            Try(String("ScalabilityInformationSEIBox")),
            Try(String("SVCPriorityAssignmentBox")),
            Try(String("ViewScalabilityInformationSEIBox")),
            Try(String("ViewIdentifierBox")),
            Try(String("MVCConfigurationBox")),
            Try(String("MVCViewPriorityAssignmentBox")),
            Try(String("IntrinsicCameraParametersBox")),
            Try(String("ExtrinsicCameraParametersBox")),
            Try(String("MVCDConfigurationBox")),
            Try(String("MVDScalabilityInformationSEIBox")),
            Try(String("A3DConfigurationBox")),
            Try(String("VvcOperatingPointsRecord")),
            Try(String("VVCSubpicIDRewritingInfomationStruct()")),
            Try(String("size += 5")), // WORKAROUND
            Try(String("j=1")), // WORKAROUND
            Try(String("j++")), // WORKAROUND
            Try(String("totalPatternLength = 0")) // WORKAROUND
            )
        .Labelled("field type");

    public static Parser<char, string> FieldName =>
        Identifier.Labelled("field name");

    public static Parser<char, PseudoCode> Comment =>
    Map((comment) => new PseudoComment(comment),
        Try(LineComment(String("//"))).Or(SkipBlockComment(String("/*"), String("*/")))
    ).Select(x => (PseudoCode)x);


    public static Parser<char, PseudoCode> Field =>
        Map((type, name, value, comment) => new PseudoField(type, name, string.Concat(value), comment),
            FieldType.Before(SkipWhitespaces),
            Try(FieldName.Before(SkipWhitespaces)).Optional(),
            Try(Any.Until(Char(';')).Before(SkipWhitespaces)).Or(Try(Any.Until(Char('\n')).Before(SkipWhitespaces))).Optional(),
            Try(LineComment(String("//"))).Optional()
        ).Select(x => (PseudoCode)x);

    public static Parser<char, string> MethodName =>
        Identifier.Labelled("field name");

    public static Parser<char, PseudoCode> Method =>
        Map((name, value, comment) => new PseudoMethod(name, string.Concat(value), comment),
            MethodName.Before(SkipWhitespaces).Before(Char('(')),
            Any.Until(Char(')')).Before(Char(';')).Before(SkipWhitespaces),
            Try(LineComment(String("//"))).Or(Try(SkipBlockComment(String("/*"), String("*/")))).Optional()
        ).Select(x => (PseudoCode)x);

    public static Parser<char, PseudoCode> Block =>
        Map((type, condition, comment, content) => new PseudoBlock(type, condition, comment, content),
            OneOf(
                Try(String("else if")), 
                Try(String("if")), 
                Try(String("else")),
                Try(String("do")),
                Try(String("for"))
                ).Before(SkipWhitespaces),
            Try(Parentheses).Optional(),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(SkipBlockComment(String("/*"), String("*/")))).Optional()),
            SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock))))
        ).Select(x => (PseudoCode)x);

    public static Parser<char, PseudoCode> RepeatingBlock =>
        Map((content, array) => new PseudoRepeatingBlock(content, array),
            SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))),
            SkipWhitespaces.Then(Char('[')).Then(Any.AtLeastOnceUntil(Char(']')))
        ).Select(x => (PseudoCode)x);


    public static Parser<char, IEnumerable<PseudoCode>> SingleBlock => Try(Method).Or(Field).Repeat(1);
    public static Parser<char, PseudoCode> CodeBlock => Try(Block).Or(Try(RepeatingBlock).Or(Try(Method).Or(Try(Field).Or(Comment))));
    public static Parser<char, IEnumerable<PseudoCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));


    public static Parser<char, string> ClassType =>
        OneOf(
            Try(String("()")),
            Try(String("(bit(24) flags)")),
            Try(String("(fmt)")),
            Try(String("(codingname)")),
            Try(String("(handler_type)")),
            Try(String("(referenceType)")),
            Try(String("(unsigned int(32) reference_type)")),
            Try(String("(grouping_type, version, flags)")),
            Try(String("('snut')")),
            Try(String("('resv')")),
            Try(String("('msrc')")),
            Try(String("('cstg')")),
            Try(String("('alte')")),
            Try(String("(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)"))
            ).Labelled("class type");

    public static Parser<char, PseudoExtendedClass> ExtendedClass => Map((oldBoxType, boxType, extendedBoxName, parameters) => new PseudoExtendedClass(oldBoxType, boxType, extendedBoxName, parameters),
            SkipWhitespaces.Then(Try(String("extends")).Optional()).Then(SkipWhitespaces).Then(Try(BoxName).Optional()),
            SkipWhitespaces.Then(Char('(')).Then(Try(OldBoxType).Optional()),
            SkipWhitespaces.Then(Try(String("'avc1' or 'avc3'")).Or(Try(BoxType)).Optional()),
            SkipWhitespaces.Then(Try(Char(',')).Optional()).Then(Try(Parameters).Optional()).Before(Char(')')).Before(SkipWhitespaces).Optional()
        );

    public static Parser<char, PseudoClass> Box =>
        Map((alignment, boxName, classType, extended, fields) => new PseudoClass(alignment, boxName, classType, extended, fields),
            Try(String("aligned(8)")).Optional(),
            SkipWhitespaces.Then(String("class")).Then(SkipWhitespaces).Then(Identifier),
            SkipWhitespaces.Then(Try(ClassType).Optional()),
            SkipWhitespaces.Then(Try(ExtendedClass).Optional()),
            Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}'))
        );


    static void Main(string[] args)
    {
        Box.ParseOrThrow(
            "aligned(8) class TrackGroupTypeBox('cstg') extends FullBox('cstg', version = 0, flags = 0)\n" +
            "{\n" +
            "\tunsigned int(32) track_group_id;\n" +
            "\t// the remaining data may be specified \n" +
            "\t//  for a particular track_group_type\n" +
            "}"
            );

        string[] files = {
            "14496-12-boxes.json",
            "14496-15-boxes.json",
            "14496-30-boxes.json",
            "23008-12-boxes.json",
            "14496-12-codecs.json",
            "14496-12-entity-groups.json",
            "14496-12-item-properties.json",
            "14496-12-item-references.json",
            "14496-12-sample-groups.json",
            "14496-12-track-groups.json",
            "14496-12-track-references.json",
            "14496-15-codecs.json",
            "14496-15-entity-groups.json",
            "14496-15-sample-groups.json",
            "14496-15-track-groups.json",
            "14496-15-track-references.json",
            "23008-12-entity-groups.json",
            "23008-12-item-properties.json",
            "23008-12-item-references.json",
            "23008-12-sample-groups.json",
        };
        int success = 0;
        int fail = 0;

        foreach (var file in files)
        {
            using (var json = File.OpenRead(file))
            using (JsonDocument document = JsonDocument.Parse(json, new JsonDocumentOptions()))
            {

                foreach (JsonElement element in document.RootElement.GetProperty("entries").EnumerateArray())
                {
                    string sample = element.GetProperty("syntax").GetString()!;

                    if (string.IsNullOrEmpty(sample))
                    {
                        Console.WriteLine($"Succeeded parsing: {element.GetProperty("fourcc").GetString()} (empty)");
                        success++;
                        continue;
                    }

                    try
                    {
                        var result = Box.ParseOrThrow(sample);
                        Console.WriteLine($"Succeeded parsing: {element.GetProperty("fourcc").GetString()}");
                        success++;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine($"Failed to parse: {element.GetProperty("fourcc").GetString()}");
                        fail++;
                    }
                }
            }
        }

        Console.WriteLine($"Succeessful: {success}, Failed: {fail}, Total: {success + fail}");
    }

    static partial void HelloFrom(string name);
}