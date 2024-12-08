using Pidgin;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
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
    public string FourCC { get; set; }
    public string BoxName { get; set; }
    public string ClassType { get; }
    public string Comment { get; }
    public string EndComment { get; }
    public IList<PseudoCode> Fields { get; }
    public string Alignment { get; }
    public PseudoExtendedClass Extended { get; }

    public PseudoClass(
        Maybe<string> comment,
        Maybe<string> alignment,
        string boxName, 
        Maybe<string> classType,
        Maybe<PseudoExtendedClass> extended,
        IEnumerable<PseudoCode> fields,
        Maybe<string> endComment)
    {
        Comment = comment.GetValueOrDefault();
        BoxName = boxName;
        ClassType = classType.GetValueOrDefault();
        Fields = fields.ToList();
        Alignment = alignment.GetValueOrDefault();
        Extended = extended.GetValueOrDefault();
        EndComment = endComment.GetValueOrDefault();
    }

}

public class PseudoExtendedClass : PseudoCode
{
    public string OldType { get; }
    public string BoxType { get; }
    public IList<(string Name, string Value)> Parameters { get; }
    public string BoxName { get; }

    public PseudoExtendedClass(
        Maybe<string> boxName,
        Maybe<string> oldType,
        Maybe<string> boxType,
        Maybe<Maybe<IEnumerable<(string Name, Maybe<string> Value)>>> parameters)
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
    }
}

public class PseudoField : PseudoCode
{
    public PseudoField(string type, Maybe<string> name, Maybe<IEnumerable<char>> value, Maybe<string> comment)
    {
        Type = type;
        Name = name.GetValueOrDefault();
        Value = value.HasValue ? string.IsNullOrEmpty(string.Concat(value.GetValueOrDefault())) ? null : string.Concat(value.GetValueOrDefault()) : null;
        Comment = comment.GetValueOrDefault();
    }

    public string Type { get; }
    public string Name { get; }
    public string Value { get; }
    public string Comment { get; }
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
        Comment = comment.GetValueOrDefault();
    }

    public string Name { get; }
    public string Value { get; }
    public string Comment { get; }
}

public class PseudoBlock : PseudoCode
{
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

    public static Parser<char, string> BlockComment<T, U>(Parser<char, T> blockCommentStart, Parser<char, U> blockCommentEnd)
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
            .Then(Any.Until(blockCommentEnd), (first, rest) => string.Concat(rest))
            .Labelled("block comment");
    }

    public static Parser<char, string> FieldType =>
        OneOf(
            Try(String("int i, j")), // WORKAROUND
            Try(String("int i,j")), // WORKAROUND
            Try(String("int i")), // WORKAROUND
            Try(String("unsigned int(64)")),
            Try(String("unsigned int(48)")),
            Try(String("template int(32)[9]")),
            Try(String("unsigned int(32)[3]")),
            Try(String("unsigned int(32)")),
            Try(String("unsigned_int(32)")),
            Try(String("unsigned int(24)")),
            Try(String("unsigned int(29)")),
            Try(String("unsigned int(26)")),
            Try(String("unsigned int(16)")),
            Try(String("unsigned_int(16)")),
            Try(String("unsigned int(15)")),
            Try(String("unsigned int(12)")),
            Try(String("unsigned int(10)")),
            Try(String("unsigned int(8)[length]")),
            Try(String("unsigned int(8)[32]")),
            Try(String("unsigned int(8)[16]")),
            Try(String("unsigned int(9)")),
            Try(String("unsigned int(8)")),
            Try(String("unsigned int(7)")),
            Try(String("unsigned int(6)")),
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
            Try(String("unsigned int(subgroupIdLen)")),
            Try(String("const unsigned int(32)[2]")),
            Try(String("const unsigned int(32)[3]")),
            Try(String("const unsigned int(32)")),
            Try(String("const unsigned int(16)[3]")),
            Try(String("const unsigned int(16)")),
            Try(String("const unsigned int(26)")),
            Try(String("template int(32)")),
            Try(String("template int(16)")),
            Try(String("template unsigned int(30)")),
            Try(String("template unsigned int(32)")),
            Try(String("template unsigned int(16)[3]")),
            Try(String("template unsigned int(16)")),
            Try(String("template unsigned int(8)")),
            Try(String("int(64)")),
            Try(String("int(32)")),
            Try(String("int(16)")),
            Try(String("int(8)")),
            Try(String("int(4)")),
            Try(String("int")),
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
            Try(String("bit(24)")),
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
            Try(String("uint(8)[32]")),
            Try(String("uint(8)")),
            Try(String("signed int(32)")),
            Try(String("signed int (16)")),
            Try(String("signed int(16)")),
            Try(String("signed int (8)")),
            Try(String("signed int(64)")),
            Try(String("signed   int(32)")),
            Try(String("signed   int(8)")),
            Try(String("Box()[]")),
            Try(String("Box[]")),
            Try(String("Box")),
            Try(String("SchemeTypeBox")),
            Try(String("SchemeInformationBox")),
            Try(String("ItemPropertyContainerBox")),
            Try(String("ItemPropertyAssociationBox")),
            Try(String("char")),
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
            Try(String("SingleItemTypeReferenceBoxLarge")),
            Try(String("SingleItemTypeReferenceBox")),
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
            Try(String("OperatingPointsRecord")),
            Try(String("VvcSubpicIDEntry")),
            Try(String("VvcSubpicOrderEntry")),
            Try(String("URIInitBox")),
            Try(String("URIBox")),
            Try(String("URIbox")),
            Try(String("CleanApertureBox")),
            Try(String("PixelAspectRatioBox")),
            Try(String("DownMixInstructions()")),
            Try(String("DRCCoefficientsBasic() []")),
            Try(String("DRCInstructionsBasic() []")),
            Try(String("DRCCoefficientsUniDRC() []")),
            Try(String("DRCInstructionsUniDRC() []")),
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
            Try(String("MPEG4ExtensionDescriptorsBox ()")),
            Try(String("MPEG4ExtensionDescriptorsBox()")),
            Try(String("MPEG4ExtensionDescriptorsBox")),
            Try(String("bit(8*dci_nal_unit_length)")),
            Try(String("DependencyInfo")),
            Try(String("VvcPTLRecord(0)")),
            Try(String("EVCSliceComponentTrackConfigurationBox")),
            Try(String("SVCMetadataSampleConfigBox")),
            Try(String("SVCPriorityLayerInfoBox")),
            Try(String("EVCConfigurationBox")),
            Try(String("VvcNALUConfigBox")),
            Try(String("VvcConfigurationBox")),
            Try(String("HEVCTileConfigurationBox")),
            Try(String("MetadataKeyTableBox()")),
            Try(String("BitRateBox ()")),
            Try(String("size += 5")), // WORKAROUND
            Try(String("j=1")), // WORKAROUND
            Try(String("j++")), // WORKAROUND
            Try(String("subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8")), // WORKAROUND
            Try(String("totalPatternLength = 0")) // WORKAROUND
            )
        .Labelled("field type");

    public static Parser<char, string> FieldName =>
        Identifier.Labelled("field name");

    public static Parser<char, PseudoCode> Comment =>
    Map((comment) => new PseudoComment(comment),
        Try(LineComment(String("//"))).Or(BlockComment(String("/*"), String("*/")))
    ).Select(x => (PseudoCode)x);


    public static Parser<char, PseudoCode> Field =>
        Map((type, name, value, comment) => new PseudoField(type, name, value, comment),
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
            Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()
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
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
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
            Try(String("('vvcb', version, flags)")),
            Try(String("(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)"))
            ).Labelled("class type");

    public static Parser<char, PseudoExtendedClass> ExtendedClass => Map((extendedBoxName, oldBoxType, boxType, parameters) => new PseudoExtendedClass(extendedBoxName, oldBoxType, boxType, parameters),
            SkipWhitespaces.Then(Try(String("extends")).Optional()).Then(SkipWhitespaces).Then(Try(BoxName).Optional()),
            SkipWhitespaces.Then(Char('(')).Then(Try(OldBoxType).Optional()),
            SkipWhitespaces.Then(
                    Try(String("'avc2' or 'avc4'")).Or(
                    Try(String("'svc1' or 'svc2'"))).Or(
                    Try(String("'vvc1' or 'vvi1'"))).Or(
                    Try(String("'evs1' or 'evs2'"))).Or(
                    Try(String("'avc1' or 'avc3'"))).Or(
                    Try(BoxType)).Optional()),
            SkipWhitespaces.Then(Try(Char(',')).Optional()).Then(Try(Parameters).Optional()).Before(Char(')')).Before(SkipWhitespaces).Optional()
        );

    public static Parser<char, PseudoClass> Box =>
        Map((comment, alignment, boxName, classType, extended, fields, endComment) => new PseudoClass(comment, alignment, boxName, classType, extended, fields, endComment),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
            Try(String("aligned(8)")).Optional(),
            SkipWhitespaces.Then(String("class")).Then(SkipWhitespaces).Then(Identifier),
            SkipWhitespaces.Then(Try(ClassType).Optional()),
            SkipWhitespaces.Then(Try(ExtendedClass).Optional()),
            Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}')),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional())
        );

    public static Parser<char, IEnumerable<PseudoClass>> Boxes => SkipWhitespaces.Then(Box.SeparatedAndOptionallyTerminated(SkipWhitespaces));


    static void Main(string[] args)
    {
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

        Dictionary<string, PseudoClass> ret = new Dictionary<string, PseudoClass>();
        List<PseudoClass> dupliates = new List<PseudoClass>();

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
                        var result = Boxes.ParseOrThrow(sample);
                        foreach (var item in result)
                        {
                            item.FourCC = element.GetProperty("fourcc").GetString();
                            success++;
                            if (!ret.TryAdd(item.BoxName, item))
                            {
                                dupliates.Add(item);

                                int index = 1;
                                while (!ret.TryAdd($"{item.BoxName}{index}", item))
                                {
                                    index++;
                                }

                                // override the name
                                item.BoxName = $"{item.BoxName}{index}";
                            }
                        }

                        Console.WriteLine($"Succeeded parsing: {element.GetProperty("fourcc").GetString()}");

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

        Console.WriteLine($"Successful: {success}, Failed: {fail}, Total: {success + fail}");
        string js = Newtonsoft.Json.JsonConvert.SerializeObject(ret.Values.ToArray());
        File.WriteAllText("result.json", js);

        string resultCode = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxGenerator2
{
";
        foreach (var b in ret.Values.ToArray())
        {
            if (b.FourCC == "encv") // skip
                continue;

            string code = BuildCode(b);
            resultCode += code + "\r\n\r\n";
        }
        resultCode += 
@"
}
";
    }

    private static string Sanitize(string name)
    {
        if (name == "namespace")
            return "ns";
        return name;
    }

    private static string BuildCode(PseudoClass? b)
    {
        string cls = @$"public class {b.BoxName}";
        if(b.Extended != null)
            cls += $" : {b.Extended.BoxName}";

        cls += "\r\n{\r\n";
        cls += $"\tpublic override string FourCC {{ get {{ return \"{b.FourCC}\"; }} }}";

        var fields = FlattenFields(b.Fields);

        foreach (var field in fields)
        {
            cls += "\r\n" + BuildProperty(field);
        }

        cls += $"\r\n\r\n\tpublic {b.BoxName}()\r\n\t{{ }}\r\n";
        cls += "\r\n\tpublic async override Task ReadAsync(Stream stream)\r\n\t{\r\n\t\tawait base.ReadAsync(stream);";
        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, true);
        }
        cls += "\r\n\t}\r\n";


        cls += "\r\n\tpublic async override Task<ulong> WriteAsync(Stream stream)\r\n\t{\r\n\t\tulong size = 0;\r\n\t\tsize += await base.WriteAsync(stream);";
        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, false);
        }
        cls += "\r\n\t\treturn size;\r\n\t}\r\n";

        cls += "}\r\n";

        return cls;
    }

    private static List<PseudoField> FlattenFields(IEnumerable<PseudoCode> fields)
    {
        Dictionary<string, PseudoField> ret = new Dictionary<string, PseudoField>();
        foreach(var code in fields)
        {
            if (code is PseudoField field)
            {
                if (IsWorkaround(field.Type))
                    continue;

                string name = GetFieldName(field);
                if (!ret.TryAdd(name, field))
                {
                    if (CompareTypeSize(ret[name].Type, field.Type) < 0)
                    {
                        ret[name] = field;
                    }
                }
            }
            else if (code is PseudoBlock block)
            {
                var blockFields = FlattenFields(block.Content);
                foreach (var blockField in blockFields)
                {
                    string name = GetFieldName(blockField);
                    if (!ret.TryAdd(name, blockField))
                    {
                        if (CompareTypeSize(ret[name].Type, blockField.Type) < 0)
                        {
                            ret[name] = blockField;
                        }
                    }
                }
            }
        }
        return ret.Values.ToList();
    }

    private static int CompareTypeSize(string type1, string type2)
    {
        if (type1 == type2)
            return 0;

        List<string> map = new List<string>()
        {
            "bit(8 ceil(size / 8) – size)",
            "unsigned int(64)",
            "signed int(64)",
            "int(64)",
            "unsigned int(32)[3]",
            "const unsigned int(32)[2]",
            "const unsigned int(32)",
            "int(32)",
            "int",
            "unsigned int(32)",
            "signed int(32)",
            "signed   int(32)",
            "const unsigned int(16)[3]",
            "const unsigned int(16)",
            "unsigned int(16)",
            "const bit(16)",
            "int(16)",
            "unsigned int(15)",
            "unsigned int(12)",
            "unsigned int(8)",
            "bit(8)",
            "bit(7)",
            "unsigned int(7)",
            "unsigned int(6)",
            "bit(6)",
            "unsigned int(4)",
            "bit(4)",
            "bit(3)",
            "bit(2)",
            "unsigned int(1)",
            "bit(1)",
            "SingleItemTypeReferenceBoxLarge",
            "SingleItemTypeReferenceBox",
        };

        if (!map.Contains(type1))
            throw new Exception(type1);
        if (!map.Contains(type2))
            throw new Exception(type2);

        return map.IndexOf(type2) - map.IndexOf(type1); 
    }

    private static string BuildProperty(PseudoCode field)
    {
        var block = field as PseudoBlock;
        if (block != null)
        {
            string ret = "";
            foreach(var f in block.Content)
            {
                ret += "\r\n" + BuildProperty(f);
            }
            return ret;
        }

        string comment = (field as PseudoField)?.Comment;
        if (!string.IsNullOrEmpty(comment)) 
        {
            comment = " // " + comment;
        }
        else
        {
            comment = "";
        }
        string value = (field as PseudoField)?.Value;
        string tp = (field as PseudoField)?.Type;

        if (string.IsNullOrEmpty(tp) && !string.IsNullOrEmpty((field as PseudoField)?.Name))
            tp = (field as PseudoField)?.Name?.Replace("[]", "").Replace("()", "");

        if (!string.IsNullOrEmpty(tp))
        {
            if (IsWorkaround((field as PseudoField)?.Type))
                return "";
            else
            {
                // TODO: incorrectly parsed type
                string typedef = "";
                if (!string.IsNullOrEmpty(value) && value.StartsWith('['))
                {
                    Debug.WriteLine($"Wrong: {value}");
                    typedef = "[]";
                    value = "";
                }
                if (!string.IsNullOrEmpty(value) && value.StartsWith('('))
                {
                    typedef = "";
                    value = "";
                }
                if (value == "= {if track_is_audio 0x0100 else 0}") // TODO
                {
                    value = "= 0";
                    comment = "// = { default samplerate of media}<<16;" + comment;
                }
                else if (value == "= { default samplerate of media}<<16")
                {
                    value = "= 0";
                    comment = "// = {if track_is_audio 0x0100 else 0};" + comment;
                }
                else if (GetType((field as PseudoField)?.Type) == "bool" && !string.IsNullOrEmpty(value))
                {
                    if (value == "= 0")
                        value = "= false";
                    else if (value == "= 1")
                        value = "= true";
                    else
                        Debug.WriteLine($"Unsupported bool value: {value}");
                }

                if (!string.IsNullOrEmpty(value))
                {
                    value = " " + value.Replace("'", "\"") + ";";
                }
                
                string name = GetFieldName(field);
                return $"\tpublic {GetType((field as PseudoField)?.Type)}{typedef} {name} {{ get; set; }}{value}{comment}";
            }
        }
        else
            return "";
    }

    private static string GetFieldName(PseudoCode field)
    {
        string name = Sanitize((field as PseudoField)?.Name);
        if (string.IsNullOrEmpty(name))
        {
            name = GetType((field as PseudoField)?.Type)?.Replace("[]", "");
        }

        return name;
    }

    private static string BuildMethodCode(PseudoCode field, int level, bool isRead)
    {
        string spacing = GetSpacing(level);
        var block = field as PseudoBlock;
        if(block != null)
        {
            return BuildBlock(block, level, isRead);
        }

        var method = field as PseudoMethod;
        if (method != null)
        {
            return BuildMethod(method, level, isRead);
        }

        var comment = field as PseudoComment;
        if (comment != null)
        {
            return BuildComment(comment, level, isRead);
        }

        string tt = (field as PseudoField)?.Type;

        if (string.IsNullOrEmpty(tt) && !string.IsNullOrEmpty((field as PseudoField)?.Name))
            tt = (field as PseudoField)?.Name?.Replace("[]", "").Replace("()", "");

        if (string.IsNullOrEmpty(tt))
            return "";

        if (IsWorkaround(tt))
        {
            if (tt == "int i, j" || tt == "int i,j" || tt == "int i") // this one must be ignored
                return "";
            else if (tt == "j=1")
                return "int j = 1;";
            else if (tt == "subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8")
                return "ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);";
            else
                return $"{tt};";
        }

        string name = GetFieldName(field);
        string m = isRead ? GetReadMethod(tt) : GetWriteMethod(tt);
        string typedef = "";
        string value = (field as PseudoField)?.Value;
        if (!string.IsNullOrEmpty(value) && value.StartsWith("["))
        {
            typedef = value;
        }
        if(isRead)
            return $"{spacing}this.{name}{typedef} = {m};";
        else
            return $"{spacing}size += {m}, ({GetType(tt)})this.{name}{typedef});";
    }

    private static string BuildComment(PseudoComment comment, int level, bool isRead)
    {
        string spacing = GetSpacing(level);
        string text = comment.Comment;
        return $"{spacing}/* {text} */";
    }

    private static string BuildMethod(PseudoMethod method, int level, bool isRead)
    {
        string comment = "";
        if (!string.IsNullOrEmpty(method.Comment))
        {
            comment = " //" + method.Comment;
        }
        //Debug.WriteLine($"Method: {method.Name}{method.Value}{comment};\r\n");
        return $"// TODO: This should likely be a FullBox: {method.Name}{method.Value};{comment}\r\n";
    }

    private static string BuildBlock(PseudoBlock block, int level, bool isRead)
    {
        string spacing = GetSpacing(level);
        string ret;

        string condition = block.Condition?.Replace("'", "\"").Replace("floor(", "(");
        string blockType = block.Type;
        if (blockType == "for")
        {
            condition = "(int " + condition.TrimStart('(');
        }
        else if(blockType == "do")
        {
            blockType = "while";
            condition = "(true)";
        }

        ret = $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

        foreach (var field in block.Content)
        {
            ret += "\r\n" + BuildMethodCode(field, level + 1, isRead);
        }

        ret += $"\r\n{spacing}}}";

        return ret;
    }

    public static string GetSpacing(int level)
    {
        string ret = "";
        for (int i = 0; i < level; i++)
        {
            ret += "\t";
        }
        return ret;
    }

    private static string GetReadMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)", "IsoReaderWriter.ReadUInt64(stream)" },
            { "unsigned int(48)", "IsoReaderWriter.ReadUInt48(stream)" },
            { "template int(32)[9]", "IsoReaderWriter.ReadUInt32Array(stream, 9)" },
            { "unsigned int(32)[3]", "IsoReaderWriter.ReadUInt32Array(stream, 3)" },
            { "unsigned int(32)", "IsoReaderWriter.ReadUInt32(stream)" },
            { "unsigned_int(32)", "IsoReaderWriter.ReadUInt32(stream)" },
            { "unsigned int(24)", "IsoReaderWriter.ReadUInt24(stream)" },
            { "unsigned int(29)", "IsoReaderWriter.ReadBits(stream, 29)" },
            { "unsigned int(26)", "IsoReaderWriter.ReadBits(stream, 26)" },
            { "unsigned int(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "unsigned_int(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "unsigned int(15)", "IsoReaderWriter.ReadBits(stream, 15)" },
            { "unsigned int(12)", "IsoReaderWriter.ReadBits(stream, 12)" },
            { "unsigned int(10)", "IsoReaderWriter.ReadBits(stream, 10)" },
            { "unsigned int(8)[length]", "IsoReaderWriter.ReadBytes(stream, length)" },
            { "unsigned int(8)[32]", "IsoReaderWriter.ReadBytes(stream, 32)" },
            { "unsigned int(8)[16]", "IsoReaderWriter.ReadBytes(stream, 16)" },
            { "unsigned int(9)", "IsoReaderWriter.ReadBits(stream, 9)" },
            { "unsigned int(8)", "IsoReaderWriter.ReadUInt8(stream)" },
            { "unsigned int(7)", "IsoReaderWriter.ReadBits(stream, 7)" },
            { "unsigned int(6)", "IsoReaderWriter.ReadBits(stream, 6)" },
            { "unsigned int(5)[3]", "IsoReaderWriter.ReadBitsArray(stream, 5, 3)" },
            { "unsigned int(5)", "IsoReaderWriter.ReadBits(stream, 5)" },
            { "unsigned int(4)", "IsoReaderWriter.ReadBits(stream, 4)" },
            { "unsigned int(3)", "IsoReaderWriter.ReadBits(stream, 3)" },
            { "unsigned int(2)", "IsoReaderWriter.ReadBits(stream, 2)" },
            { "unsigned int(1)", "IsoReaderWriter.ReadBit(stream)" },
            { "unsigned int (32)", "IsoReaderWriter.ReadUInt32(stream)" },
            { "unsigned int(f(pattern_size_code))", "IsoReaderWriter.ReadBytes(stream, f(pattern_size_code))" },
            { "unsigned int(f(index_size_code))", "IsoReaderWriter.ReadBytes(stream, f(index_size_code))" },
            { "unsigned int(f(count_size_code))", "IsoReaderWriter.ReadBytes(stream, f(count_size_code))" },
            { "unsigned int(base_offset_size*8)", "IsoReaderWriter.ReadBytes(stream, base_offset_size)" },
            { "unsigned int(offset_size*8)", "IsoReaderWriter.ReadBytes(stream, offset_size)" },
            { "unsigned int(length_size*8)", "IsoReaderWriter.ReadBytes(stream, length_size)" },
            { "unsigned int(index_size*8)", "IsoReaderWriter.ReadBytes(stream, index_size)" },
            { "unsigned int(field_size)", "IsoReaderWriter.ReadBytes(stream, field_size)" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_traf_num+1))" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_trun_num+1))" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_sample_num+1))" },
            { "unsigned int(8*size-64)", "IsoReaderWriter.ReadBytes(stream, size-64)" },
            { "unsigned int(subgroupIdLen)", "IsoReaderWriter.ReadBytes(stream, subgroupIdLen)" },
            { "const unsigned int(32)[2]", "IsoReaderWriter.ReadUInt32Array(stream, 2)" },
            { "const unsigned int(32)[3]", "IsoReaderWriter.ReadUInt32Array(stream, 3)" },
            { "const unsigned int(32)", "IsoReaderWriter.ReadUInt32(stream)" },
            { "const unsigned int(16)[3]", "IsoReaderWriter.ReadUInt16Array(stream, 3)" },
            { "const unsigned int(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "const unsigned int(26)", "IsoReaderWriter.ReadBits(stream, 26)" },
            { "template int(32)", "IsoReaderWriter.ReadInt32(stream)" },
            { "template int(16)", "IsoReaderWriter.ReadInt16(stream)" },
            { "template unsigned int(30)", "IsoReaderWriter.ReadBits(stream, 30)" },
            { "template unsigned int(32)", "IsoReaderWriter.ReadUInt32(stream)" },
            { "template unsigned int(16)[3]", "IsoReaderWriter.ReadUInt16Array(stream, 3)" },
            { "template unsigned int(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "template unsigned int(8)", "IsoReaderWriter.ReadUInt8(stream)" },
            { "int(64)", "IsoReaderWriter.ReadInt64(stream)" },
            { "int(32)", "IsoReaderWriter.ReadInt32(stream)" },
            { "int(16)", "IsoReaderWriter.ReadInt16(stream)" },
            { "int(8)", "IsoReaderWriter.ReadInt8(stream)" },
            { "int(4)", "IsoReaderWriter.ReadBits(stream, 4)" },
            { "int", "IsoReaderWriter.ReadInt32(stream)" },
            { "const bit(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "const bit(1)", "IsoReaderWriter.ReadBit(stream)" },
            { "bit(1)", "IsoReaderWriter.ReadBit(stream)" },
            { "bit(2)", "IsoReaderWriter.ReadBits(stream, 2)" },
            { "bit(3)", "IsoReaderWriter.ReadBits(stream, 3)" },
            { "bit(4)", "IsoReaderWriter.ReadBits(stream, 4)" },
            { "bit(5)", "IsoReaderWriter.ReadBits(stream, 5)" },
            { "bit(6)", "IsoReaderWriter.ReadBits(stream, 6)" },
            { "bit(7)", "IsoReaderWriter.ReadBits(stream, 7)" },
            { "bit(8)", "IsoReaderWriter.ReadUInt8(stream)" },
            { "bit(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "bit(24)", "IsoReaderWriter.ReadBits(stream, 24)" },
            { "bit(31)", "IsoReaderWriter.ReadBits(stream, 31)" },
            { "bit(8 ceil(size / 8) \u2013 size)", "IsoReaderWriter.ReadBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size))" },
            { "bit(8* ps_nalu_length)", "IsoReaderWriter.ReadBytes(stream, ps_nalu_length)" },
            { "utf8string", "IsoReaderWriter.ReadString(stream)" },
            { "utfstring", "IsoReaderWriter.ReadString(stream)" },
            { "utf8list", "IsoReaderWriter.ReadString(stream)" },
            { "boxstring", "IsoReaderWriter.ReadString(stream)" },
            { "string", "IsoReaderWriter.ReadString(stream)" },
            { "bit(32)[6]", "IsoReaderWriter.ReadUInt32Array(stream, 6)" },
            { "uint(32)", "IsoReaderWriter.ReadUInt32(stream)" },
            { "uint(16)", "IsoReaderWriter.ReadUInt16(stream)" },
            { "uint(64)", "IsoReaderWriter.ReadUInt64(stream)" },
            { "uint(8)[32]", "IsoReaderWriter.ReadBytes(stream, 32)" },
            { "uint(8)", "IsoReaderWriter.ReadUInt8(stream)" },
            { "signed int(32)", "IsoReaderWriter.ReadInt32(stream)" },
            { "signed int (16)", "IsoReaderWriter.ReadInt16(stream)" },
            { "signed int(16)", "IsoReaderWriter.ReadInt16(stream)" },
            { "signed int (8)", "IsoReaderWriter.ReadInt8(stream)" },
            { "signed int(64)", "IsoReaderWriter.ReadInt64(stream)" },
            { "signed   int(32)", "IsoReaderWriter.ReadInt32(stream)" },
            { "signed   int(8)", "IsoReaderWriter.ReadInt8(stream)" },
            { "Box()[]", "IsoReaderWriter.ReadBoxes(stream)" },
            { "Box[]", "IsoReaderWriter.ReadBoxes(stream)" },
            { "Box", "IsoReaderWriter.ReadBox(stream)" },
            { "SchemeTypeBox", "(SchemeTypeBox)IsoReaderWriter.ReadBox(stream)" },
            { "SchemeInformationBox", "(SchemeInformationBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemPropertyContainerBox", "(ItemPropertyContainerBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemPropertyAssociationBox", "(ItemPropertyAssociationBox)IsoReaderWriter.ReadBox(stream)" },
            { "char", "IsoReaderWriter.ReadInt8(stream)" },
            { "loudness", "IsoReaderWriter.ReadInt32(stream)" },
            { "ICC_profile", "(ICC_profile)IsoReaderWriter.ReadClass(stream)" },
            { "OriginalFormatBox(fmt)", "(OriginalFormatBox)IsoReaderWriter.ReadBox(stream)" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "(DataEntryBaseBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemInfoEntry[ entry_count ]", "(ItemInfoEntry)IsoReaderWriter.ReadClass(stream)" },
            { "TypeCombinationBox", "(TypeCombinationBox)IsoReaderWriter.ReadBox(stream)" },
            { "FilePartitionBox", "(FilePartitionBox)IsoReaderWriter.ReadBox(stream)" },
            { "FECReservoirBox", "(FECReservoirBox)IsoReaderWriter.ReadBox(stream)" },
            { "FileReservoirBox", "(FileReservoirBox)IsoReaderWriter.ReadBox(stream)" },
            { "PartitionEntry", "(PartitionEntry)IsoReaderWriter.ReadClass(stream)" },
            { "FDSessionGroupBox", "(FDSessionGroupBox)IsoReaderWriter.ReadBox(stream)" },
            { "GroupIdToNameBox", "(GroupIdToNameBox)IsoReaderWriter.ReadBox(stream)" },
            { "base64string", "IsoReaderWriter.ReadString(stream)" },
            { "ProtectionSchemeInfoBox", "(ProtectionSchemeInfoBox)IsoReaderWriter.ReadBox(stream)" },
            { "SingleItemTypeReferenceBox", "(SingleItemTypeReferenceBox)IsoReaderWriter.ReadBox(stream)" },
            { "SingleItemTypeReferenceBoxLarge", "(SingleItemTypeReferenceBoxLarge)IsoReaderWriter.ReadBox(stream)" },
            { "HandlerBox(handler_type)", "(HandlerBox)IsoReaderWriter.ReadBox(stream)" },
            { "PrimaryItemBox", "(PrimaryItemBox)IsoReaderWriter.ReadBox(stream)" },
            { "DataInformationBox", "(DataInformationBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemLocationBox", "(ItemLocationBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemProtectionBox", "(ItemProtectionBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemInfoBox", "(ItemInfoBox)IsoReaderWriter.ReadBox(stream)" },
            { "IPMPControlBox", "(IPMPControlBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemReferenceBox", "(ItemReferenceBox)IsoReaderWriter.ReadBox(stream)" },
            { "ItemDataBox", "(ItemDataBox)IsoReaderWriter.ReadBox(stream)" },
            { "TrackReferenceTypeBox []", "(TrackReferenceTypeBox[])IsoReaderWriter.ReadBoxes(stream)" },
            { "MetadataKeyBox[]", "(MetadataKeyBox[])IsoReaderWriter.ReadBoxes(stream)" },
            { "TierInfoBox", "(TierInfoBox)IsoReaderWriter.ReadBox(stream)" },
            { "MultiviewRelationAttributeBox", "(MultiviewRelationAttributeBox)IsoReaderWriter.ReadBox(stream)" },
            { "TierBitRateBox", "(TierBitRateBox)IsoReaderWriter.ReadBox(stream)" },
            { "BufferingBox", "(BufferingBox)IsoReaderWriter.ReadBox(stream)" },
            { "MultiviewSceneInfoBox", "(MultiviewSceneInfoBox)IsoReaderWriter.ReadBox(stream)" },
            { "MVDDecoderConfigurationRecord", "(MVDDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "MVDDepthResolutionBox", "(MVDDepthResolutionBox)IsoReaderWriter.ReadBox(stream)" },
            { "MVCDecoderConfigurationRecord()", "(MVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "AVCDecoderConfigurationRecord()", "(AVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "HEVCDecoderConfigurationRecord()", "(HEVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "LHEVCDecoderConfigurationRecord()", "(LHEVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "SVCDecoderConfigurationRecord()", "(SVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "HEVCTileTierLevelConfigurationRecord()", "(HEVCTileTierLevelConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "EVCDecoderConfigurationRecord()", "(EVCDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "VvcDecoderConfigurationRecord()", "(VvcDecoderConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "EVCSliceComponentTrackConfigurationRecord()", "(EVCSliceComponentTrackConfigurationRecord)IsoReaderWriter.ReadClass(stream)" },
            { "SampleGroupDescriptionEntry (grouping_type)", "(SampleGroupDescriptionEntry)IsoReaderWriter.ReadClass(stream)" },
            { "Descriptor", "(Descriptor)IsoReaderWriter.ReadClass(stream)" },
            { "WebVTTConfigurationBox", "(WebVTTConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "WebVTTSourceLabelBox", "(WebVTTSourceLabelBox)IsoReaderWriter.ReadBox(stream)" },
            { "OperatingPointsRecord", "(OperatingPointsRecord)IsoReaderWriter.ReadClass(stream)" },
            { "VvcSubpicIDEntry", "(VvcSubpicIDEntry)IsoReaderWriter.ReadClass(stream)" },
            { "VvcSubpicOrderEntry", "(VvcSubpicOrderEntry)IsoReaderWriter.ReadClass(stream)" },
            { "URIInitBox", "(URIInitBox)IsoReaderWriter.ReadBox(stream)" },
            { "URIBox", "(URIBox)IsoReaderWriter.ReadBox(stream)" },
            { "URIbox", "(URIBox)IsoReaderWriter.ReadBox(stream)" },
            { "CleanApertureBox", "(CleanApertureBox)IsoReaderWriter.ReadBox(stream)" },
            { "PixelAspectRatioBox", "(PixelAspectRatioBox)IsoReaderWriter.ReadBox(stream)" },
            { "DownMixInstructions()", "(DownMixInstructions)IsoReaderWriter.ReadClass(stream)" },
            { "DRCCoefficientsBasic() []", "(DRCCoefficientsBasic[])IsoReaderWriter.ReadClasses(stream)" },
            { "DRCInstructionsBasic() []", "(DRCInstructionsBasic[])IsoReaderWriter.ReadClasses(stream)" },
            { "DRCCoefficientsUniDRC() []", "(DRCCoefficientsUniDRC[])IsoReaderWriter.ReadClasses(stream)" },
            { "DRCInstructionsUniDRC() []", "(DRCInstructionsUniDRC[])IsoReaderWriter.ReadClasses(stream)" },
            { "HEVCConfigurationBox", "(HEVCConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "LHEVCConfigurationBox", "(LHEVCConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "AVCConfigurationBox", "(AVCConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "SVCConfigurationBox", "(SVCConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "ScalabilityInformationSEIBox", "(ScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream)" },
            { "SVCPriorityAssignmentBox", "(SVCPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream)" },
            { "ViewScalabilityInformationSEIBox", "(ViewScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream)" },
            { "ViewIdentifierBox", "(ViewIdentifierBox)IsoReaderWriter.ReadBox(stream)" },
            { "MVCConfigurationBox", "(MVCConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "MVCViewPriorityAssignmentBox", "(MVCViewPriorityAssignmentBox)IsoReaderWriter.ReadBox(stream)" },
            { "IntrinsicCameraParametersBox", "(IntrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream)" },
            { "ExtrinsicCameraParametersBox", "(ExtrinsicCameraParametersBox)IsoReaderWriter.ReadBox(stream)" },
            { "MVCDConfigurationBox", "(MVCDConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "MVDScalabilityInformationSEIBox", "(MVDScalabilityInformationSEIBox)IsoReaderWriter.ReadBox(stream)" },
            { "A3DConfigurationBox", "(A3DConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "VvcOperatingPointsRecord", "(VvcOperatingPointsRecord)IsoReaderWriter.ReadClass(stream)" },
            { "VVCSubpicIDRewritingInfomationStruct()", "(VVCSubpicIDRewritingInfomationStruct)IsoReaderWriter.ReadClass(stream)" },
            { "MPEG4ExtensionDescriptorsBox ()", "(MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream)" },
            { "MPEG4ExtensionDescriptorsBox()", "(MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream)" },
            { "MPEG4ExtensionDescriptorsBox", "(MPEG4ExtensionDescriptorsBox)IsoReaderWriter.ReadBox(stream)" },
            { "bit(8*dci_nal_unit_length)", "IsoReaderWriter.ReadBytes(stream, dci_nal_unit_length)" },
            { "DependencyInfo", "(DependencyInfo)IsoReaderWriter.ReadClass(stream)" },
            { "VvcPTLRecord(0)", "(VvcPTLRecord)IsoReaderWriter.ReadClass(stream)" },
            { "EVCSliceComponentTrackConfigurationBox", "(EVCSliceComponentTrackConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "SVCMetadataSampleConfigBox", "(SVCMetadataSampleConfigBox)IsoReaderWriter.ReadBox(stream)" },
            { "SVCPriorityLayerInfoBox", "(SVCPriorityLayerInfoBox)IsoReaderWriter.ReadBox(stream)" },
            { "EVCConfigurationBox", "(EVCConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "VvcNALUConfigBox", "(VvcNALUConfigBox)IsoReaderWriter.ReadBox(stream)" },
            { "VvcConfigurationBox", "(VvcConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "HEVCTileConfigurationBox", "(HEVCTileConfigurationBox)IsoReaderWriter.ReadBox(stream)" },
            { "MetadataKeyTableBox()", "(MetadataKeyTableBox)IsoReaderWriter.ReadBox(stream)" },
            { "BitRateBox ()", "(BitRateBox)IsoReaderWriter.ReadBox(stream)" },
        };
        return map[type];
    }

    private static string GetWriteMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)", "IsoReaderWriter.WriteUInt64(stream" },
            { "unsigned int(48)", "IsoReaderWriter.WriteUInt48(stream" },
            { "template int(32)[9]", "IsoReaderWriter.WriteUInt32Array(stream, 9" },
            { "unsigned int(32)[3]", "IsoReaderWriter.WriteUInt32Array(stream, 3" },
            { "unsigned int(32)", "IsoReaderWriter.WriteUInt32(stream" },
            { "unsigned_int(32)", "IsoReaderWriter.WriteUInt32(stream" },
            { "unsigned int(24)", "IsoReaderWriter.WriteUInt24(stream" },
            { "unsigned int(29)", "IsoReaderWriter.WriteBits(stream, 29" },
            { "unsigned int(26)", "IsoReaderWriter.WriteBits(stream, 26" },
            { "unsigned int(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "unsigned_int(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "unsigned int(15)", "IsoReaderWriter.WriteBits(stream, 15" },
            { "unsigned int(12)", "IsoReaderWriter.WriteBits(stream, 12" },
            { "unsigned int(10)", "IsoReaderWriter.WriteBits(stream, 10" },
            { "unsigned int(8)[length]", "IsoReaderWriter.WriteBytes(stream, length" },
            { "unsigned int(8)[32]", "IsoReaderWriter.WriteBytes(stream, 32" },
            { "unsigned int(8)[16]", "IsoReaderWriter.WriteBytes(stream, 16" },
            { "unsigned int(9)", "IsoReaderWriter.WriteBits(stream, 9" },
            { "unsigned int(8)", "IsoReaderWriter.WriteUInt8(stream" },
            { "unsigned int(7)", "IsoReaderWriter.WriteBits(stream, 7" },
            { "unsigned int(6)", "IsoReaderWriter.WriteBits(stream, 6" },
            { "unsigned int(5)[3]", "IsoReaderWriter.WriteBitsArray(stream, 5, 3" },
            { "unsigned int(5)", "IsoReaderWriter.WriteBits(stream, 5" },
            { "unsigned int(4)", "IsoReaderWriter.WriteBits(stream, 4" },
            { "unsigned int(3)", "IsoReaderWriter.WriteBits(stream, 3" },
            { "unsigned int(2)", "IsoReaderWriter.WriteBits(stream, 2" },
            { "unsigned int(1)", "IsoReaderWriter.WriteBit(stream" },
            { "unsigned int (32)", "IsoReaderWriter.WriteUInt32(stream" },
            { "unsigned int(f(pattern_size_code))", "IsoReaderWriter.WriteBytes(stream, f(pattern_size_code)" },
            { "unsigned int(f(index_size_code))", "IsoReaderWriter.WriteBytes(stream, f(index_size_code)" },
            { "unsigned int(f(count_size_code))", "IsoReaderWriter.WriteBytes(stream, f(count_size_code)" },
            { "unsigned int(base_offset_size*8)", "IsoReaderWriter.WriteBytes(stream, base_offset_size" },
            { "unsigned int(offset_size*8)", "IsoReaderWriter.WriteBytes(stream, offset_size" },
            { "unsigned int(length_size*8)", "IsoReaderWriter.WriteBytes(stream, length_size" },
            { "unsigned int(index_size*8)", "IsoReaderWriter.WriteBytes(stream, index_size" },
            { "unsigned int(field_size)", "IsoReaderWriter.WriteBytes(stream, field_size" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_traf_num+1)" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_trun_num+1)" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_sample_num+1)" },
            { "unsigned int(8*size-64)", "IsoReaderWriter.WriteBytes(stream, size-64" },
            { "unsigned int(subgroupIdLen)", "IsoReaderWriter.WriteBytes(stream, subgroupIdLen" },
            { "const unsigned int(32)[2]", "IsoReaderWriter.WriteUInt32Array(stream, 2" },
            { "const unsigned int(32)[3]", "IsoReaderWriter.WriteUInt32Array(stream, 3" },
            { "const unsigned int(32)", "IsoReaderWriter.WriteUInt32(stream" },
            { "const unsigned int(16)[3]", "IsoReaderWriter.WriteUInt16Array(stream, 3" },
            { "const unsigned int(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "const unsigned int(26)", "IsoReaderWriter.WriteBits(stream, 26" },
            { "template int(32)", "IsoReaderWriter.WriteInt32(stream" },
            { "template int(16)", "IsoReaderWriter.WriteInt16(stream" },
            { "template unsigned int(30)", "IsoReaderWriter.WriteBits(stream, 30" },
            { "template unsigned int(32)", "IsoReaderWriter.WriteUInt32(stream" },
            { "template unsigned int(16)[3]", "IsoReaderWriter.WriteUInt16Array(stream, 3" },
            { "template unsigned int(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "template unsigned int(8)", "IsoReaderWriter.WriteUInt8(stream" },
            { "int(64)", "IsoReaderWriter.WriteInt64(stream" },
            { "int(32)", "IsoReaderWriter.WriteInt32(stream" },
            { "int(16)", "IsoReaderWriter.WriteInt16(stream" },
            { "int(8)", "IsoReaderWriter.WriteInt8(stream" },
            { "int(4)", "IsoReaderWriter.WriteBits(stream, 4" },
            { "int", "IsoReaderWriter.WriteInt32(stream" },
            { "const bit(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "const bit(1)", "IsoReaderWriter.WriteBit(stream" },
            { "bit(1)", "IsoReaderWriter.WriteBit(stream" },
            { "bit(2)", "IsoReaderWriter.WriteBits(stream, 2" },
            { "bit(3)", "IsoReaderWriter.WriteBits(stream, 3" },
            { "bit(4)", "IsoReaderWriter.WriteBits(stream, 4" },
            { "bit(5)", "IsoReaderWriter.WriteBits(stream, 5" },
            { "bit(6)", "IsoReaderWriter.WriteBits(stream, 6" },
            { "bit(7)", "IsoReaderWriter.WriteBits(stream, 7" },
            { "bit(8)", "IsoReaderWriter.WriteUInt8(stream" },
            { "bit(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "bit(24)", "IsoReaderWriter.WriteBits(stream, 24" },
            { "bit(31)", "IsoReaderWriter.WriteBits(stream, 31" },
            { "bit(8 ceil(size / 8) \u2013 size)", "IsoReaderWriter.WriteBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size)" },
            { "bit(8* ps_nalu_length)", "IsoReaderWriter.WriteBytes(stream, ps_nalu_length" },
            { "utf8string", "IsoReaderWriter.WriteString(stream" },
            { "utfstring", "IsoReaderWriter.WriteString(stream" },
            { "utf8list", "IsoReaderWriter.WriteString(stream" },
            { "boxstring", "IsoReaderWriter.WriteString(stream" },
            { "string", "IsoReaderWriter.WriteString(stream" },
            { "bit(32)[6]", "IsoReaderWriter.WriteUInt32Array(stream, 6" },
            { "uint(32)", "IsoReaderWriter.WriteUInt32(stream" },
            { "uint(16)", "IsoReaderWriter.WriteUInt16(stream" },
            { "uint(64)", "IsoReaderWriter.WriteUInt64(stream" },
            { "uint(8)[32]", "IsoReaderWriter.WriteBytes(stream, 32" },
            { "uint(8)", "IsoReaderWriter.WriteUInt8(stream" },
            { "signed int(32)", "IsoReaderWriter.WriteInt32(stream" },
            { "signed int (16)", "IsoReaderWriter.WriteInt16(stream" },
            { "signed int(16)", "IsoReaderWriter.WriteInt16(stream" },
            { "signed int (8)", "IsoReaderWriter.WriteInt8(stream" },
            { "signed int(64)", "IsoReaderWriter.WriteInt64(stream" },
            { "signed   int(32)", "IsoReaderWriter.WriteInt32(stream" },
            { "signed   int(8)", "IsoReaderWriter.WriteInt8(stream" },
            { "Box()[]", "IsoReaderWriter.WriteBoxes(stream" },
            { "Box[]", "IsoReaderWriter.WriteBoxes(stream" },
            { "Box", "IsoReaderWriter.WriteBox(stream" },
            { "SchemeTypeBox", "IsoReaderWriter.WriteBox(stream" },
            { "SchemeInformationBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemPropertyContainerBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemPropertyAssociationBox", "IsoReaderWriter.WriteBox(stream" },
            { "char", "IsoReaderWriter.WriteInt8(stream" },
            { "loudness", "IsoReaderWriter.WriteInt32(stream" },
            { "ICC_profile", "IsoReaderWriter.WriteClass(stream" },
            { "OriginalFormatBox(fmt)", "IsoReaderWriter.WriteBox(stream" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "IsoReaderWriter.WriteBox(stream" },
            { "ItemInfoEntry[ entry_count ]", "IsoReaderWriter.WriteClass(stream" },
            { "TypeCombinationBox", "IsoReaderWriter.WriteBox(stream" },
            { "FilePartitionBox", "IsoReaderWriter.WriteBox(stream" },
            { "FECReservoirBox", "IsoReaderWriter.WriteBox(stream" },
            { "FileReservoirBox", "IsoReaderWriter.WriteBox(stream" },
            { "PartitionEntry", "IsoReaderWriter.WriteClass(stream" },
            { "FDSessionGroupBox", "IsoReaderWriter.WriteBox(stream" },
            { "GroupIdToNameBox", "IsoReaderWriter.WriteBox(stream" },
            { "base64string", "IsoReaderWriter.WriteString(stream" },
            { "ProtectionSchemeInfoBox", "IsoReaderWriter.WriteBox(stream" },
            { "SingleItemTypeReferenceBox", "IsoReaderWriter.WriteBox(stream" },
            { "SingleItemTypeReferenceBoxLarge", "IsoReaderWriter.WriteBox(stream" },
            { "HandlerBox(handler_type)", "IsoReaderWriter.WriteBox(stream" },
            { "PrimaryItemBox", "IsoReaderWriter.WriteBox(stream" },
            { "DataInformationBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemLocationBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemProtectionBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemInfoBox", "IsoReaderWriter.WriteBox(stream" },
            { "IPMPControlBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemReferenceBox", "IsoReaderWriter.WriteBox(stream" },
            { "ItemDataBox", "IsoReaderWriter.WriteBox(stream" },
            { "TrackReferenceTypeBox []", "IsoReaderWriter.WriteBoxes(stream" },
            { "MetadataKeyBox[]", "IsoReaderWriter.WriteBoxes(stream" },
            { "TierInfoBox", "IsoReaderWriter.WriteBox(stream" },
            { "MultiviewRelationAttributeBox", "IsoReaderWriter.WriteBox(stream" },
            { "TierBitRateBox", "IsoReaderWriter.WriteBox(stream" },
            { "BufferingBox", "IsoReaderWriter.WriteBox(stream" },
            { "MultiviewSceneInfoBox", "IsoReaderWriter.WriteBox(stream" },
            { "MVDDecoderConfigurationRecord", "IsoReaderWriter.WriteClass(stream" },
            { "MVDDepthResolutionBox", "IsoReaderWriter.WriteBox(stream" },
            { "MVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "AVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "HEVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "LHEVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "SVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "HEVCTileTierLevelConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "EVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "VvcDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "EVCSliceComponentTrackConfigurationRecord()", "IsoReaderWriter.WriteClass(stream" },
            { "SampleGroupDescriptionEntry (grouping_type)", "IsoReaderWriter.WriteClass(stream" },
            { "Descriptor", "IsoReaderWriter.WriteClass(stream" },
            { "WebVTTConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "WebVTTSourceLabelBox", "IsoReaderWriter.WriteBox(stream" },
            { "OperatingPointsRecord", "IsoReaderWriter.WriteClass(stream" },
            { "VvcSubpicIDEntry", "IsoReaderWriter.WriteClass(stream" },
            { "VvcSubpicOrderEntry", "IsoReaderWriter.WriteClass(stream" },
            { "URIInitBox", "IsoReaderWriter.WriteBox(stream" },
            { "URIBox", "IsoReaderWriter.WriteBox(stream" },
            { "URIbox", "IsoReaderWriter.WriteBox(stream" },
            { "CleanApertureBox", "IsoReaderWriter.WriteBox(stream" },
            { "PixelAspectRatioBox", "IsoReaderWriter.WriteBox(stream" },
            { "DownMixInstructions()", "IsoReaderWriter.WriteClass(stream" },
            { "DRCCoefficientsBasic() []", "IsoReaderWriter.WriteClasses(stream" },
            { "DRCInstructionsBasic() []", "IsoReaderWriter.WriteClasses(stream" },
            { "DRCCoefficientsUniDRC() []", "IsoReaderWriter.WriteClasses(stream" },
            { "DRCInstructionsUniDRC() []", "IsoReaderWriter.WriteClasses(stream" },
            { "HEVCConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "LHEVCConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "AVCConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "SVCConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "ScalabilityInformationSEIBox", "IsoReaderWriter.WriteBox(stream" },
            { "SVCPriorityAssignmentBox", "IsoReaderWriter.WriteBox(stream" },
            { "ViewScalabilityInformationSEIBox", "IsoReaderWriter.WriteBox(stream" },
            { "ViewIdentifierBox", "IsoReaderWriter.WriteBox(stream" },
            { "MVCConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "MVCViewPriorityAssignmentBox", "IsoReaderWriter.WriteBox(stream" },
            { "IntrinsicCameraParametersBox", "IsoReaderWriter.WriteBox(stream" },
            { "ExtrinsicCameraParametersBox", "IsoReaderWriter.WriteBox(stream" },
            { "MVCDConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "MVDScalabilityInformationSEIBox", "IsoReaderWriter.WriteBox(stream" },
            { "A3DConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "VvcOperatingPointsRecord", "IsoReaderWriter.WriteClass(stream" },
            { "VVCSubpicIDRewritingInfomationStruct()", "IsoReaderWriter.WriteClass(stream" },
            { "MPEG4ExtensionDescriptorsBox ()", "IsoReaderWriter.WriteBox(stream" },
            { "MPEG4ExtensionDescriptorsBox()", "IsoReaderWriter.WriteBox(stream" },
            { "MPEG4ExtensionDescriptorsBox", "IsoReaderWriter.WriteBox(stream" },
            { "bit(8*dci_nal_unit_length)", "IsoReaderWriter.WriteBytes(stream, dci_nal_unit_length" },
            { "DependencyInfo", "IsoReaderWriter.WriteClass(stream" },
            { "VvcPTLRecord(0)", "IsoReaderWriter.WriteClass(stream" },
            { "EVCSliceComponentTrackConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "SVCMetadataSampleConfigBox", "IsoReaderWriter.WriteBox(stream" },
            { "SVCPriorityLayerInfoBox", "IsoReaderWriter.WriteBox(stream" },
            { "EVCConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "VvcNALUConfigBox", "IsoReaderWriter.WriteBox(stream" },
            { "VvcConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "HEVCTileConfigurationBox", "IsoReaderWriter.WriteBox(stream" },
            { "MetadataKeyTableBox()", "IsoReaderWriter.WriteBox(stream" },
            { "BitRateBox ()", "IsoReaderWriter.WriteBox(stream" },
        };
        return map[type];
    }

    private static bool IsWorkaround(string type)
    {
        HashSet<string> map = new HashSet<string>
        {
            "int i, j",
            "int i,j",
            "int i",
            "size += 5",
            "j=1",
            "j++",
            "subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8",
            "totalPatternLength = 0"
        };
        return map.Contains(type);
    }

    private static string GetType(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string> {
            { "unsigned int(64)", "ulong" },
            { "unsigned int(48)", "ulong" },
            { "template int(32)[9]", "uint[]" },
            { "unsigned int(32)[3]", "uint[]" },
            { "unsigned int(32)", "uint" },
            { "unsigned_int(32)", "uint" },
            { "unsigned int(24)", "uint" },
            { "unsigned int(29)", "uint" },
            { "unsigned int(26)", "uint" },
            { "unsigned int(16)", "ushort" },
            { "unsigned_int(16)", "ushort" },
            { "unsigned int(15)", "ushort" },
            { "unsigned int(12)", "ushort" },
            { "unsigned int(10)", "ushort" },
            { "unsigned int(8)[length]", "byte[]" },
            { "unsigned int(8)[32]", "byte[]" },
            { "unsigned int(8)[16]", "byte[]" },
            { "unsigned int(9)", "ushort" },
            { "unsigned int(8)", "byte" },
            { "unsigned int(7)", "byte" },
            { "unsigned int(6)", "byte" },
            { "unsigned int(5)[3]", "byte[]" },
            { "unsigned int(5)", "byte" },
            { "unsigned int(4)", "byte" },
            { "unsigned int(3)", "byte" },
            { "unsigned int(2)", "byte" },
            { "unsigned int(1)", "bool" },
            { "unsigned int (32)", "uint" },
            { "unsigned int(f(pattern_size_code))", "byte[]" },
            { "unsigned int(f(index_size_code))", "byte[]" },
            { "unsigned int(f(count_size_code))", "byte[]" },
            { "unsigned int(base_offset_size*8)", "byte[]" },
            { "unsigned int(offset_size*8)", "byte[]" },
            { "unsigned int(length_size*8)", "byte[]" },
            { "unsigned int(index_size*8)", "byte[]" },
            { "unsigned int(field_size)", "byte[]" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "byte[]" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "byte[]" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "byte[]" },
            { "unsigned int(8*size-64)", "byte[]" },
            { "unsigned int(subgroupIdLen)", "uint[]" },
            { "const unsigned int(32)[2]", "uint[]" },
            { "const unsigned int(32)[3]", "uint[]" },
            { "const unsigned int(32)", "uint" },
            { "const unsigned int(16)[3]", "ushort[]" },
            { "const unsigned int(16)", "ushort" },
            { "const unsigned int(26)", "uint" },
            { "template int(32)", "int" },
            { "template int(16)", "short" },
            { "template unsigned int(30)", "uint" },
            { "template unsigned int(32)", "uint" },
            { "template unsigned int(16)[3]", "ushort[]" },
            { "template unsigned int(16)", "ushort" },
            { "template unsigned int(8)", "byte" },
            { "int(64)", "long" },
            { "int(32)", "int" },
            { "int(16)", "short" },
            { "int(8)", "sbyte" },
            { "int(4)", "byte" },
            { "int", "int" },
            { "const bit(16)", "ushort" },
            { "const bit(1)", "bool" },
            { "bit(1)", "bool" },
            { "bit(2)", "byte" },
            { "bit(3)", "byte" },
            { "bit(4)", "byte" },
            { "bit(5)", "byte" },
            { "bit(6)", "byte" },
            { "bit(7)", "byte" },
            { "bit(8)", "byte" },
            { "bit(16)", "ushort" },
            { "bit(24)", "uint" },
            { "bit(31)", "uint" },
            { "bit(8 ceil(size / 8) \u2013 size)", "byte[]" },
            { "bit(8* ps_nalu_length)", "byte[]" },
            { "utf8string", "string" },
            { "utfstring", "string" },
            { "utf8list", "string" },
            { "boxstring", "string" },
            { "string", "string" },
            { "bit(32)[6]", "uint[]" },
            { "uint(32)", "uint" },
            { "uint(16)", "ushort" },
            { "uint(64)", "ulong" },
            { "uint(8)[32]", "byte[]" },
            { "uint(8)", "byte" },
            { "signed int(32)", "int" },
            { "signed int (16)", "short" },
            { "signed int(16)", "short" },
            { "signed int (8)", "sbyte" },
            { "signed int(64)", "long" },
            { "signed   int(32)", "int" },
            { "signed   int(8)", "sbyte" },
            { "Box()[]", "Box[]" },
            { "Box[]", "Box[]" },
            { "Box", "Box" },
            { "SchemeTypeBox", "SchemeTypeBox" },
            { "SchemeInformationBox", "SchemeInformationBox" },
            { "ItemPropertyContainerBox", "ItemPropertyContainerBox" },
            { "ItemPropertyAssociationBox", "ItemPropertyAssociationBox" },
            { "char", "byte" },
            { "loudness", "int" },
            { "ICC_profile", "ICC_profile" },
            { "OriginalFormatBox(fmt)", "OriginalFormatBox" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "DataEntryBaseBox" },
            { "ItemInfoEntry[ entry_count ]", "ItemInfoEntry[]" },
            { "TypeCombinationBox", "TypeCombinationBox" },
            { "FilePartitionBox", "FilePartitionBox" },
            { "FECReservoirBox", "FECReservoirBox" },
            { "FileReservoirBox", "FileReservoirBox" },
            { "PartitionEntry", "PartitionEntry" },
            { "FDSessionGroupBox", "FDSessionGroupBox" },
            { "GroupIdToNameBox", "GroupIdToNameBox" },
            { "base64string", "string" },
            { "ProtectionSchemeInfoBox", "ProtectionSchemeInfoBox" },
            { "SingleItemTypeReferenceBox", "SingleItemTypeReferenceBox" },
            { "SingleItemTypeReferenceBoxLarge", "SingleItemTypeReferenceBoxLarge" },
            { "HandlerBox(handler_type)", "HandlerBox" },
            { "PrimaryItemBox", "PrimaryItemBox" },
            { "DataInformationBox", "DataInformationBox" },
            { "ItemLocationBox", "ItemLocationBox" },
            { "ItemProtectionBox", "ItemProtectionBox" },
            { "ItemInfoBox", "ItemInfoBox" },
            { "IPMPControlBox", "IPMPControlBox" },
            { "ItemReferenceBox", "ItemReferenceBox" },
            { "ItemDataBox", "ItemDataBox" },
            { "TrackReferenceTypeBox []", "TrackReferenceTypeBox[]" },
            { "MetadataKeyBox[]", "MetadataKeyBox[]" },
            { "TierInfoBox", "TierInfoBox" },
            { "MultiviewRelationAttributeBox", "MultiviewRelationAttributeBox" },
            { "TierBitRateBox", "TierBitRateBox" },
            { "BufferingBox", "BufferingBox" },
            { "MultiviewSceneInfoBox", "MultiviewSceneInfoBox" },
            { "MVDDecoderConfigurationRecord", "MVDDecoderConfigurationRecord" },
            { "MVDDepthResolutionBox", "MVDDepthResolutionBox" },
            { "MVCDecoderConfigurationRecord()", "MVCDecoderConfigurationRecord" },
            { "AVCDecoderConfigurationRecord()", "AVCDecoderConfigurationRecord" },
            { "HEVCDecoderConfigurationRecord()", "HEVCDecoderConfigurationRecord" },
            { "LHEVCDecoderConfigurationRecord()", "LHEVCDecoderConfigurationRecord" },
            { "SVCDecoderConfigurationRecord()", "SVCDecoderConfigurationRecord" },
            { "HEVCTileTierLevelConfigurationRecord()", "HEVCTileTierLevelConfigurationRecord" },
            { "EVCDecoderConfigurationRecord()", "EVCDecoderConfigurationRecord" },
            { "VvcDecoderConfigurationRecord()", "VvcDecoderConfigurationRecord" },
            { "EVCSliceComponentTrackConfigurationRecord()", "EVCSliceComponentTrackConfigurationRecord" },
            { "SampleGroupDescriptionEntry (grouping_type)", "SampleGroupDescriptionEntry" },
            { "Descriptor", "Descriptor" },
            { "WebVTTConfigurationBox", "WebVTTConfigurationBox" },
            { "WebVTTSourceLabelBox", "WebVTTSourceLabelBox" },
            { "OperatingPointsRecord", "OperatingPointsRecord" },
            { "VvcSubpicIDEntry", "VvcSubpicIDEntry" },
            { "VvcSubpicOrderEntry", "VvcSubpicOrderEntry" },
            { "URIInitBox", "URIInitBox" },
            { "URIBox", "URIBox" },
            { "URIbox", "URIBox" },
            { "CleanApertureBox", "CleanApertureBox" },
            { "PixelAspectRatioBox", "PixelAspectRatioBox" },
            { "DownMixInstructions()", "DownMixInstructions" },
            { "DRCCoefficientsBasic() []", "DRCCoefficientsBasic[]" },
            { "DRCInstructionsBasic() []", "DRCInstructionsBasic[]" },
            { "DRCCoefficientsUniDRC() []", "DRCCoefficientsUniDRC[]" },
            { "DRCInstructionsUniDRC() []", "DRCInstructionsUniDRC[]" },
            { "HEVCConfigurationBox", "HEVCConfigurationBox" },
            { "LHEVCConfigurationBox", "LHEVCConfigurationBox" },
            { "AVCConfigurationBox", "AVCConfigurationBox" },
            { "SVCConfigurationBox", "SVCConfigurationBox" },
            { "ScalabilityInformationSEIBox", "ScalabilityInformationSEIBox" },
            { "SVCPriorityAssignmentBox", "SVCPriorityAssignmentBox" },
            { "ViewScalabilityInformationSEIBox", "ViewScalabilityInformationSEIBox" },
            { "ViewIdentifierBox", "ViewIdentifierBox" },
            { "MVCConfigurationBox", "MVCConfigurationBox" },
            { "MVCViewPriorityAssignmentBox", "MVCViewPriorityAssignmentBox" },
            { "IntrinsicCameraParametersBox", "IntrinsicCameraParametersBox" },
            { "ExtrinsicCameraParametersBox", "ExtrinsicCameraParametersBox" },
            { "MVCDConfigurationBox", "MVCDConfigurationBox" },
            { "MVDScalabilityInformationSEIBox", "MVDScalabilityInformationSEIBox" },
            { "A3DConfigurationBox", "A3DConfigurationBox" },
            { "VvcOperatingPointsRecord", "VvcOperatingPointsRecord" },
            { "VVCSubpicIDRewritingInfomationStruct()", "VVCSubpicIDRewritingInfomationStruct" },
            { "MPEG4ExtensionDescriptorsBox ()", "MPEG4ExtensionDescriptorsBox" },
            { "MPEG4ExtensionDescriptorsBox()", "MPEG4ExtensionDescriptorsBox" },
            { "MPEG4ExtensionDescriptorsBox", "MPEG4ExtensionDescriptorsBox" },
            { "bit(8*dci_nal_unit_length)", "byte[]" },
            { "DependencyInfo", "DependencyInfo" },
            { "VvcPTLRecord(0)", "VvcPTLRecord" },
            { "EVCSliceComponentTrackConfigurationBox", "EVCSliceComponentTrackConfigurationBox" },
            { "SVCMetadataSampleConfigBox", "SVCMetadataSampleConfigBox" },
            { "SVCPriorityLayerInfoBox", "SVCPriorityLayerInfoBox" },
            { "EVCConfigurationBox", "EVCConfigurationBox" },
            { "VvcNALUConfigBox", "VvcNALUConfigBox" },
            { "VvcConfigurationBox", "VvcConfigurationBox" },
            { "HEVCTileConfigurationBox", "HEVCTileConfigurationBox" },
            { "MetadataKeyTableBox()", "MetadataKeyTableBox" },
            { "BitRateBox ()", "BitRateBox" },
        };
        return map[type];
    }

    static partial void HelloFrom(string name);
}
