using Newtonsoft.Json;
using Pidgin;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
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
    public string Syntax { get; internal set; }

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
    public string DescriptorTag { get; }

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
    public PseudoField(string type, Maybe<string> name, Maybe<IEnumerable<char>> value, Maybe<string> comment)
    {
        Type = type;
        Name = name.GetValueOrDefault();
        Value = value.HasValue ? string.IsNullOrEmpty(string.Concat(value.GetValueOrDefault())) ? null : string.Concat(value.GetValueOrDefault()) : null;
        Comment = comment.GetValueOrDefault();
    }

    public string Type { get; set; }
    public string Name { get; set; }
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
            Try(String("int size = 4")), // WORKAROUND
            Try(String("sizeOfInstance = sizeOfInstance<<7 | sizeByte")), // WORKAROUND
            Try(String("int sizeOfInstance = 0")), // WORKAROUND
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
            Try(String("signed int(12)")),
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
            Try(String("bit(8*nalUnitLength)")),
            Try(String("bit(8*sequenceParameterSetLength)")),
            Try(String("bit(8*pictureParameterSetLength)")),
            Try(String("bit(8*sequenceParameterSetExtLength)")),
            Try(String("unsigned int(8*num_bytes_constraint_info - 2)")),
            Try(String("bit(8*nal_unit_length)")),
            Try(String("bit(timeStampLength)")),
            Try(String("utf8string")),
            Try(String("utfstring")),
            Try(String("utf8list")),
            Try(String("boxstring")),
            Try(String("string")),
            Try(String("bit(32)[6]")),
            Try(String("bit(32)")),
            Try(String("uint(32)")),
            Try(String("uint(16)")),
            Try(String("uint(64)")),
            Try(String("uint(8)[32]")),
            Try(String("uint(8)")),
            Try(String("uint(7)")),
            Try(String("uint(1)")),
            Try(String("signed   int(64)")),
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
            Try(String("MetaDataKeyBox[]")),
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
            Try(String("DownMixInstructions() []")),
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
            Try(String("MetaDataKeyTableBox()")),
            Try(String("BitRateBox ()")),
            Try(String("TrackLoudnessInfo[]")),
            Try(String("AlbumLoudnessInfo[]")),           
            Try(String("VvcPTLRecord(num_sublayers)")),           
            Try(String("VvcPTLRecord(ptl_max_temporal_id[i]+1)")),           
            Try(String("size += 5")), // WORKAROUND
            Try(String("j=1")), // WORKAROUND
            Try(String("j++")), // WORKAROUND
            Try(String("subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8")), // WORKAROUND
            Try(String("totalPatternLength = 0")), // WORKAROUND
            // descriptors
            Try(String("DecoderConfigDescriptor")),
            Try(String("SLConfigDescriptor")),
            Try(String("IPI_DescrPointer")),
            Try(String("IP_IdentificationDataSet")),
            Try(String("IP_IdentificationDataSet")),
            Try(String("IPMP_DescriptorPointer")),
            Try(String("LanguageDescriptor")),
            Try(String("QoS_Descriptor")),
            Try(String("RegistrationDescriptor")),
            Try(String("ExtensionDescriptor")),
            Try(String("ProfileLevelIndicationIndexDescriptor")),
            Try(String("DecoderSpecificInfo")),
            Try(String("QoS_Qualifier")),
            Try(String("double(32)"))
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
        Map((name, value, comment) => new PseudoField(name, new Maybe<string>(name), new Maybe<IEnumerable<char>>(), comment),
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
                Try(String("for")),
                Try(String("while"))
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
            Try(String("(name)")),
            Try(String("(property_type)")),
            Try(String("(code)")),
            Try(String("(property_type, version, flags)")),
            Try(String("(unsigned int(32) extension_type)")),
            Try(String("('vvcb', version, flags)")),
            Try(String("(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)")),
            Try(String("(unsigned int(32) grouping_type)")),
            Try(String("(unsigned int(32) boxtype, unsigned int(8) v, bit(24) f)"))            
            ).Labelled("class type");

    public static Parser<char, string> DescriptorTag =>
       OneOf(
           Try(String("tag=DecSpecificInfoTag")),
           Try(String("tag=ES_DescrTag")),
           Try(String("tag=SLConfigDescrTag")),
           Try(String("tag=DecoderConfigDescrTag")),
           Try(String("ProfileLevelIndicationIndexDescrTag")),
           Try(String("tag=IPI_DescrPointerTag")),
           Try(String("tag=ContentIdentDescrTag..SupplContentIdentDescrTag")),
           Try(String("tag=IPMP_DescrPointerTag")),
           Try(String("tag=LanguageDescrTag")),
           Try(String("tag=QoS_DescrTag")),
           Try(String("tag=0x01..0xff")),
           Try(String("tag=0x01")),
           Try(String("tag=0x02")),
           Try(String("tag=0x03")),
           Try(String("tag=0x04")),
           Try(String("tag=0x41")),
           Try(String("tag=0x42")),
           Try(String("tag=0x43")),
           Try(String("tag=RegistrationDescrTag")),
           Try(String("tag=ExtDescrTagStartRange..ExtDescrTagEndRange")),
           Try(String("tag=OCIDescrTagStartRange..OCIDescrTagEndRange")),
           Try(String("tag=0"))
           ).Labelled("descriptor tag");

    public static Parser<char, PseudoExtendedClass> ExtendedClass => Map((extendedBoxName, oldBoxType, boxType, parameters, descriptorTag) => new PseudoExtendedClass(extendedBoxName, oldBoxType, boxType, parameters, descriptorTag),
            SkipWhitespaces.Then(Try(String("extends")).Optional()).Then(SkipWhitespaces).Then(Try(BoxName).Optional()),
            SkipWhitespaces.Then(Try(Char('(')).Optional()).Then(Try(OldBoxType).Optional()),
            SkipWhitespaces.Then(
                    Try(String("'avc2' or 'avc4'")).Or(
                    Try(String("'svc1' or 'svc2'"))).Or(
                    Try(String("'vvc1' or 'vvi1'"))).Or(
                    Try(String("'evs1' or 'evs2'"))).Or(
                    Try(String("'avc1' or 'avc3'"))).Or(
                    Try(BoxType)).Optional()),
            SkipWhitespaces.Then(Try(Char(',')).Optional()).Then(Try(Parameters).Optional()).Before(Try(Char(')')).Optional()).Before(SkipWhitespaces).Optional(),
            SkipWhitespaces.Then(Try(Char(':').Then(SkipWhitespaces).Then(String("bit(8)")).Then(SkipWhitespaces).Then(DescriptorTag).Before(SkipWhitespaces))).Optional()
        );

    public static Parser<char, PseudoClass> Box =>
        Map((comment, alignment, boxName, classType, extended, fields, endComment) => new PseudoClass(comment, alignment, boxName, classType, extended, fields, endComment),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
            SkipWhitespaces.Then(Try(String("abstract")).Optional()).Then(SkipWhitespaces).Then(Try(String("aligned(8)")).Optional()).Before(SkipWhitespaces).Before(Try(String("expandable(228-1)")).Optional()),
            SkipWhitespaces.Then(Try(String("abstract")).Optional()).Then(SkipWhitespaces).Then(String("class")).Then(SkipWhitespaces).Then(Identifier),
            SkipWhitespaces.Then(Try(ClassType).Optional()),
            SkipWhitespaces.Then(Try(ExtendedClass).Optional()),
            Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}')),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional())
        );

    public static Parser<char, IEnumerable<PseudoClass>> Boxes => SkipWhitespaces.Then(Box.SeparatedAndOptionallyTerminated(SkipWhitespaces));


    static void Main(string[] args)
    {
        string[] files = {
            "14496-1-added.json",
            //"14496-3-added.json",
            "14496-12-added.json",
            "14496-15-added.json",
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
        int duplicated = 0;
        int fail = 0;

        Dictionary<string, PseudoClass> ret = new Dictionary<string, PseudoClass>();
        List<PseudoClass> duplicates = new List<PseudoClass>();
        List<string> containers = new List<string>();

        foreach (var file in files)
        {
            using (var json = File.OpenRead(file))
            using (JsonDocument document = JsonDocument.Parse(json, new JsonDocumentOptions()))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("entries").EnumerateArray())
                {
                    string fourCC = element.TryGetProperty("fourcc", out _) ? element.GetProperty("fourcc").GetString() : null;
                    string sample = element.GetProperty("syntax").GetString()!;
                    string[] cont = element.GetProperty("containers").EnumerateArray().Select(x => x.ToString()).ToArray();
                    foreach (var con in cont)
                    {
                        if(con.StartsWith("{"))
                        {
                            var des = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(con);
                            foreach(var dc in des.Values)
                            {
                                foreach(var ddd in dc)
                                    containers.Add(ddd); 
                            }
                            continue;
                        }
                        containers.Add(con);
                    }

                    if (string.IsNullOrEmpty(sample))
                    {
                        Console.WriteLine($"Succeeded parsing: {fourCC} (empty)");
                        success++;
                        continue;
                    }

                    try
                    {
                        var result = Boxes.ParseOrThrow(sample);
                        foreach (var item in result)
                        {
                            item.FourCC = fourCC;
                            item.Syntax = GetSampleCode(sample, item.BoxName);
                            if (ret.TryAdd(item.BoxName, item))
                            {
                                success++;                                
                            }
                            else
                            {
                                duplicated++;
                                duplicates.Add(item);
                                Console.WriteLine($"Duplicated: {item.BoxName}, 4cc: {item.FourCC}");

                                if (item.FourCC != ret[item.BoxName].FourCC)
                                {
                                    int index = 1;
                                    while (!ret.TryAdd($"{item.BoxName}{index}", item))
                                    {
                                        index++;
                                    }
                                    // override the name
                                    item.BoxName = $"{item.BoxName}{index}";
                                }
                            }
                        }

                        Console.WriteLine($"Succeeded parsing: {fourCC}");

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine($"Failed to parse: {fourCC}");
                        fail++;
                    }
                }
            }
        }

        containers = containers.Distinct().ToList();

        Console.WriteLine($"Successful: {success}, Failed: {fail}, Duplicated: {duplicated}, Total: {success + fail + duplicated}");
        string js = Newtonsoft.Json.JsonConvert.SerializeObject(ret.Values.ToArray());
        File.WriteAllText("result.json", js);

        string resultCode = 
@"using System;
using System.IO;
using System.Threading.Tasks;

namespace BoxGenerator2
{
";
        foreach (var b in ret.Values.ToArray())
        {
            string code = BuildCode(b, containers);
            resultCode += code + "\r\n\r\n";
        }
        resultCode += 
@"
}
";
    }

    private static string GetSampleCode(string sample, string boxName)
    {
        Dictionary<string, string> c = new System.Collections.Generic.Dictionary<string, string>();
        List<string> lines = new List<string>();
        string className = "";
        using (var streamReader = new StringReader(sample))
        {
            String line;
            while ((line = streamReader.ReadLine()) != null)
            {
                if (line.Contains("class "))
                {
                    if(!string.IsNullOrEmpty(className))
                    {
                        c.Add(className, string.Join("\r\n", lines));
                        lines = new List<string>();
                    }

                    // start of class
                    className = line;
                }
                lines.Add(line);
            }
            c.Add(className, string.Join("\r\n", lines));
            lines = new List<string>();
        }
        return c.FirstOrDefault(x => x.Key.Contains(boxName)).Value;
    }

    private static string Sanitize(string name)
    {
        if (name == "namespace")
            return "ns";
        return name;
    }

    private static string BuildCode(PseudoClass? b, List<string> containers)
    {
        string cls = "";

        string optAbstract = "";
        if (b.BoxName == "SampleGroupDescriptionEntry" || b.BoxName == "AudioSampleGroupEntry" || b.BoxName == "VisualSampleGroupEntry" ||
            b.BoxName == "SubtitleSampleGroupEntry" || b.BoxName == "TextSampleGroupEntry" || b.BoxName == "HintSampleGroupEntry" || b.BoxName == "SubtitleSampleEntry")
        {
            optAbstract = "abstract ";
        }

        cls += $"/*\r\n{b.Syntax.Replace("*/", "*//*")}\r\n*/\r\n";

        cls += @$"public {optAbstract}class {b.BoxName}";
        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName))
            cls += $" : {b.Extended.BoxName}";

        cls += "\r\n{\r\n";

        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrWhiteSpace(b.FourCC))
        {
            cls += $"\tpublic override string FourCC {{ get; set; }} = \"{b.FourCC}\";";
        }
        else if(b.Extended != null && !string.IsNullOrEmpty(b.Extended.DescriptorTag))
        {
            if (!b.Extended.DescriptorTag.Contains(".."))
            {
                string tag = b.Extended.DescriptorTag.Replace("tag=", "");
                if(!tag.StartsWith("0"))
                {
                    tag = "DescriptorTags." + tag;
                }
                cls += $"\tpublic byte Tag {{ get; set; }} = {tag};";
            }
            else
            {
                string[] tags = b.Extended.DescriptorTag.Replace("tag=", "").Split("..");
                if (!tags[0].StartsWith("0"))
                {
                    tags[0] = "DescriptorTags." + tags[0];
                }
                if (!tags[1].StartsWith("0"))
                {
                    tags[1] = "DescriptorTags." + tags[1];
                }
                cls += $"\tpublic byte TagMin {{ get; set; }} = {tags[0]};\r\n";
                cls += $"\tpublic byte TagMax {{ get; set; }} = {tags[1]};";
            }
        }

        var fields = FlattenFields(b.Fields);

        foreach (var field in fields)
        {
            cls += "\r\n" + BuildField(field);
        }

        cls += $"\r\n\r\n\tpublic {b.BoxName}()\r\n\t{{ }}\r\n";
        cls += "\r\n\tpublic async " + (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "override " : "virtual ") + "Task<ulong> ReadAsync(Stream stream)\r\n\t{\r\n\t\tulong boxSize = 0;" +
            (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "\r\n\t\tboxSize += await base.ReadAsync(stream);" : "");

        cls = FixMissingCode(b, cls);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, MethodType.Read);
        }

        if (containers.Contains(b.FourCC) || containers.Contains(b.BoxName))
        {
            cls += "\r\n" + "boxSize += IsoReaderWriter.ReadBoxChildren(stream, boxSize, this);";
        }

        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrEmpty(b.FourCC))
        {
            cls += "\r\n" + "boxSize += IsoReaderWriter.ReadSkip(stream, size, boxSize);";
        }

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";


        cls += "\r\n\tpublic async " + (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "override " : "virtual ") + "Task<ulong> WriteAsync(Stream stream)\r\n\t{\r\n\t\tulong boxSize = 0;" +
            (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "\r\n\t\tboxSize += await base.WriteAsync(stream);" : "");

        cls = FixMissingCode(b, cls);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, MethodType.Write);
        }

        if (containers.Contains(b.FourCC) || containers.Contains(b.BoxName))
        {
            cls += "\r\n" + "boxSize += IsoReaderWriter.WriteBoxChildren(stream, this);";
        }

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";

        cls += "\r\n\tpublic " + (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "override " : "virtual ") + "ulong CalculateSize()\r\n\t{\r\n\t\tulong boxSize = 0;" +
            (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "\r\n\t\tboxSize += base.CalculateSize();" : "");

        cls = FixMissingCode(b, cls);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, MethodType.Size);
        }

        if (containers.Contains(b.FourCC) || containers.Contains(b.BoxName))
        {
            cls += "\r\n" + "boxSize += IsoReaderWriter.CalculateSize(children);";
        }

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";

        // end of class
        cls += "}\r\n";

        return cls;
    }

    private static string FixMissingCode(PseudoClass? box, string cls)
    {
        if (box.BoxName == "DegradationPriorityBox" || box.BoxName == "SampleDependencyTypeBox" || box.BoxName == "SampleDependencyBox")
        {
            cls += "\r\n\t\tint sample_count = 0; // TODO: taken from the stsz sample_count\r\n";
        }
        else if (box.BoxName == "DownMixInstructions")
        {
            cls += "\r\n\t\tint baseChannelCount = 0; // TODO: get somewhere";
        }
        else if(box.BoxName == "CompactSampleToGroupBox")
        {
            cls += "\r\n";
            cls += "\t\tbool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);\r\n";
            cls += "\t\tuint count_size_code = (flags >> 2) & 0x3;\r\n";
            cls += "\t\tuint pattern_size_code = (flags >> 4) & 0x3;\r\n";
            cls += "\t\tuint index_size_code = flags & 0x3;\r\n";
        }
        else if(box.BoxName == "VvcPTLRecord")
        {
            cls += "\r\n";
            cls += "\t\tint num_sublayers = 0; // TODO pass arg\r\n";
        }
        else if (box.BoxName == "VvcDecoderConfigurationRecord")
        {
            cls += "\r\n";
            cls += "\t\tconst int OPI_NUT = 12;\r\n";
            cls += "\t\tconst int DCI_NUT = 13;\r\n";
        }

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

                AddAndResolveDuplicates(ret, field);
            }
            else if (code is PseudoBlock block)
            {
                var blockFields = FlattenFields(block.Content);
                foreach (var blockField in blockFields)
                {
                    AddAndResolveDuplicates(ret, blockField);
                }
            }
        }
        return ret.Values.ToList();
    }

    private static void AddAndResolveDuplicates(Dictionary<string, PseudoField> ret, PseudoField field)
    {
        string name = GetFieldName(field);
        if (!ret.TryAdd(name, field))
        {
            // TODO: this only works as long as the name does not appear in a condition/code
            int i = 0;
            string updatedName = $"{name}{i}";
            while (!ret.TryAdd(updatedName, field))
            {
                i++;
                updatedName = $"{name}{i}";
            }
            field.Name = updatedName;
        }
    }

    private static string BuildField(PseudoCode field)
    {
        var block = field as PseudoBlock;
        if (block != null)
        {
            string ret = "";
            foreach(var f in block.Content)
            {
                ret += "\r\n" + BuildField(f);
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
                if (!string.IsNullOrEmpty(value) && value.StartsWith('['))
                {
                    (field as PseudoField).Type = tp + value;
                    tp = (field as PseudoField)?.Type;
                    value = "";
                }

                string tt = GetType((field as PseudoField)?.Type);
                if (!string.IsNullOrEmpty(value) && value.StartsWith('('))
                {
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
                else if(value == "= boxtype" || value == "= extended_type" || value == "= codingname" ||
                    value == "= f" || value == "= v")
                {
                    comment = "// " + value;
                    value = "";
                }
                else if (tt == "bool" && !string.IsNullOrEmpty(value))
                {
                    if (value == "= 0" || value == "=0")
                        value = "= false";
                    else if (value == "= 1" || value == "=1")
                        value = "= true";
                    else
                        Debug.WriteLine($"Unsupported bool value: {value}");
                }
                else if(tt.Contains('[') && value == "= 0")
                {
                    value = "= []";
                }

                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("'b"))
                    {
                        value = value.Replace("'b", "").Replace("'", "0b");

                        if (value == "= 0b0")
                            value = "= false";
                    }

                    value = " " + value.Replace("'", "\"");
                }

                value = FixFourCC(value);

                string name = GetFieldName(field);
                string propertyName = name.ToPropertyCase();

                if (name == propertyName)
                    propertyName = "_" + name;

                return $"\r\n\tprotected {tt} {name}{value}; {comment}\r\n" + // must be "protected", derived classes access base members
                 $"\tpublic {tt} {propertyName} {{ get {{ return this.{name}; }} set {{ this.{name} = value; }} }}";
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

    private static string BuildMethodCode(PseudoCode field, int level, MethodType methodType)
    {
        string spacing = GetSpacing(level);
        var block = field as PseudoBlock;
        if(block != null)
        {
            return BuildBlock(block, level, methodType);
        }

        var comment = field as PseudoComment;
        if (comment != null)
        {
            return BuildComment(comment, level, methodType);
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
            else if (tt == "totalPatternLength = 0")
                return "uint totalPatternLength = 0;";
            else
                return $"{tt};";
        }

        string name = GetFieldName(field);
        string m = methodType == MethodType.Read ? GetReadMethod(tt) : (methodType == MethodType.Write ? GetWriteMethod(tt) : GetCalculateSizeMethod(tt));
        string typedef = "";
        string value = (field as PseudoField)?.Value;
        if (!string.IsNullOrEmpty(value) && value.StartsWith("[") && value != "[]" &&
            value != "[count]" && value != "[ entry_count ]" && value != "[numReferences]" 
            && value != "[0 .. 255]" && value != "[0..1]" && value != "[0 .. 1]" && value != "[0..255]" && 
            value != "[ sample_count ]" && value != "[method_count]" && value != "[URLlength]" && value != "[sizeOfInstance-4]")
        {
            typedef = value;
        }

        string fieldComment = "";
        if (!string.IsNullOrEmpty((field as PseudoField)?.Comment))
        {
            fieldComment = "//" + (field as PseudoField).Comment;
        }

        string optional = "";
        if(fieldComment.Contains("optional") && !fieldComment.Contains("optional boxes follow"))
        {
            if (methodType == MethodType.Write || methodType == MethodType.Size)
            {
                optional = $"if(this.{name} != null) ";
            }
            else if(methodType == MethodType.Read)
            {
                // TODO: check if we can read
                optional = "if(boxSize < size) ";
            }
        }

        if (methodType == MethodType.Read)
            return $"{spacing}{optional}boxSize += {m} out this.{name}{typedef}); {fieldComment}";
        else if(methodType == MethodType.Write)
            return $"{spacing}{optional}boxSize += {m} this.{name}{typedef}); {fieldComment}";
        else
            return $"{spacing}{optional}boxSize += {m.Replace("value", name)}; // {name}";
    }

    private static string BuildComment(PseudoComment comment, int level, MethodType methodType)
    {
        string spacing = GetSpacing(level);
        string text = comment.Comment;
        return $"{spacing}/* {text} */";
    }

    public enum MethodType
    {
        Read,
        Write,
        Size
    }

    private static string BuildBlock(PseudoBlock block, int level, MethodType methodType)
    {
        string spacing = GetSpacing(level);
        string ret;

        string condition = block.Condition?.Replace("'", "\"").Replace("floor(", "(");
        string blockType = block.Type;
        if (blockType == "for")
        {
            condition = "(int " + condition.TrimStart('(');
        }
        else if (blockType == "do")
        {
            blockType = "while";
            condition = "(true)";
        }
        else if (blockType == "if")
        {
            // fix "flags"
            if (condition.Contains('&') && !condition.Contains("&&"))
            {
                string[] conditionParts = condition.TrimStart('(').TrimEnd(')').Split('&');
                if (conditionParts.Length == 2)
                {
                    condition = $"(({conditionParts[0]} & {conditionParts[1]}) == {conditionParts[1]})"; // fix the "flags" syntax
                }
            }

            if(condition.Contains("grouping_type_parameter_present") ||
                condition.Contains("omitted_channels_present") ||
                condition.Contains("in_stream") ||
                condition.Contains("dynamic_rect") ||
                condition.Contains("level_is_static_flag"))
            {
                condition = condition
                    .Replace("==0", "== false").Replace("==1", "== true")
                    .Replace("== 0", "== false").Replace("== 1", "== true");
            }

            // other fixes - taken from ISO_IEC_14496-12_2015 comments
            if (condition.Contains("channelStructured"))
                condition = condition.Replace("channelStructured", "1");
            if (condition.Contains("objectStructured"))
                condition = condition.Replace("objectStructured", "2");
        }

        condition = FixFourCC(condition);

        ret = $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

        foreach (var field in block.Content)
        {
            ret += "\r\n" + BuildMethodCode(field, level + 1, methodType);
        }

        ret += $"\r\n{spacing}}}";

        return ret;
    }

    private static string FixFourCC(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            // fix 4cc
            const string regex = "\\\"[\\w\\s\\!]{4}\\\"";
            var match = Regex.Match(value, regex);
            if (match.Success)
            {
                string inputValue = match.Value;
                string replaceValue = "IsoReaderWriter.FromFourCC(" + inputValue + ")";
                value = value.Replace(inputValue, replaceValue);
            }
        }

        return value;
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
            { "unsigned int(64)[ entry_count ]", "IsoReaderWriter.ReadUInt64Array(stream, entry_count, " },
            { "unsigned int(64)", "IsoReaderWriter.ReadUInt64(stream, " },
            { "unsigned int(48)", "IsoReaderWriter.ReadUInt48(stream, " },
            { "unsigned int(32)[ entry_count ]", "IsoReaderWriter.ReadUInt32Array(stream, entry_count, " },
            { "template int(32)[9]", "IsoReaderWriter.ReadUInt32Array(stream, 9, " },
            { "unsigned int(32)[3]", "IsoReaderWriter.ReadUInt32Array(stream, 3, " },
            { "unsigned int(32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "unsigned_int(32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "unsigned int(24)", "IsoReaderWriter.ReadUInt24(stream, " },
            { "unsigned int(29)", "IsoReaderWriter.ReadBits(stream, 29, " },
            { "unsigned int(26)", "IsoReaderWriter.ReadBits(stream, 26, " },
            { "unsigned int(16)[i]", "IsoReaderWriter.ReadUInt16(stream, " },
            { "unsigned int(16)[j]", "IsoReaderWriter.ReadUInt16(stream, " },
            { "unsigned int(16)[i][j]", "IsoReaderWriter.ReadUInt16(stream, " },
            { "unsigned int(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "unsigned_int(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "unsigned int(15)", "IsoReaderWriter.ReadBits(stream, 15, " },
            { "unsigned int(12)", "IsoReaderWriter.ReadBits(stream, 12, " },
            { "signed int(12)", "IsoReaderWriter.ReadBits(stream, 12, " },
            { "unsigned int(10)[i][j]", "IsoReaderWriter.ReadBits(stream, 10, " },
            { "unsigned int(10)[i]", "IsoReaderWriter.ReadBits(stream, 10, " },
            { "unsigned int(10)", "IsoReaderWriter.ReadBits(stream, 10, " },
            { "unsigned int(8)[ sample_count ]", "IsoReaderWriter.ReadBytes(stream, sample_count, " },
            { "unsigned int(8)[length]", "IsoReaderWriter.ReadBytes(stream, length, " },
            { "unsigned int(8)[32]", "IsoReaderWriter.ReadBytes(stream, 32, " },
            { "unsigned int(8)[16]", "IsoReaderWriter.ReadBytes(stream, 16, " },
            { "unsigned int(9)", "IsoReaderWriter.ReadBits(stream, 9, " },
            { "unsigned int(8)", "IsoReaderWriter.ReadUInt8(stream, " },
            { "unsigned int(7)", "IsoReaderWriter.ReadBits(stream, 7, " },
            { "unsigned int(6)", "IsoReaderWriter.ReadBits(stream, 6, " },
            { "unsigned int(5)[3]", "IsoReaderWriter.ReadBitsArray(stream, 5, 3, " },
            { "unsigned int(5)", "IsoReaderWriter.ReadBits(stream, 5, " },
            { "unsigned int(4)", "IsoReaderWriter.ReadBits(stream, 4, " },
            { "unsigned int(3)", "IsoReaderWriter.ReadBits(stream, 3, " },
            { "unsigned int(2)[i][j]", "IsoReaderWriter.ReadBits(stream, 2, " },
            { "unsigned int(2)", "IsoReaderWriter.ReadBits(stream, 2, " },
            { "unsigned int(1)[i]", "IsoReaderWriter.ReadBit(stream, " },
            { "unsigned int(1)", "IsoReaderWriter.ReadBit(stream, " },
            { "unsigned int (32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "unsigned int(f(pattern_size_code))[i]", "IsoReaderWriter.ReadBits(stream, pattern_size_code, " },
            { "unsigned int(f(index_size_code))[i]", "IsoReaderWriter.ReadBits(stream, index_size_code, " },
            { "unsigned int(f(index_size_code))[j][k]", "IsoReaderWriter.ReadBits(stream, index_size_code, " },
            { "unsigned int(f(count_size_code))[i]", "IsoReaderWriter.ReadBits(stream, count_size_code, " },
            { "unsigned int(base_offset_size*8)", "IsoReaderWriter.ReadBytes(stream, base_offset_size, " },
            { "unsigned int(offset_size*8)", "IsoReaderWriter.ReadBytes(stream, offset_size, " },
            { "unsigned int(length_size*8)", "IsoReaderWriter.ReadBytes(stream, length_size, " },
            { "unsigned int(index_size*8)", "IsoReaderWriter.ReadBytes(stream, index_size, " },
            { "unsigned int(field_size)", "IsoReaderWriter.ReadBytes(stream, field_size, " },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_traf_num+1), " },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_trun_num+1), " },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "IsoReaderWriter.ReadBytes(stream, (ulong)(length_size_of_sample_num+1), " },
            { "unsigned int(8*size-64)", "IsoReaderWriter.ReadBytes(stream, size-64, " },
            { "unsigned int(subgroupIdLen)[i]", "IsoReaderWriter.ReadUInt32(stream, " },
            { "const unsigned int(32)[2]", "IsoReaderWriter.ReadUInt32Array(stream, 2, " },
            { "const unsigned int(32)[3]", "IsoReaderWriter.ReadUInt32Array(stream, 3, " },
            { "const unsigned int(32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "const unsigned int(16)[3]", "IsoReaderWriter.ReadUInt16Array(stream, 3, " },
            { "const unsigned int(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "const unsigned int(26)", "IsoReaderWriter.ReadBits(stream, 26, " },
            { "template int(32)", "IsoReaderWriter.ReadInt32(stream, " },
            { "template int(16)", "IsoReaderWriter.ReadInt16(stream, " },
            { "template unsigned int(30)", "IsoReaderWriter.ReadBits(stream, 30, " },
            { "template unsigned int(32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "template unsigned int(16)[3]", "IsoReaderWriter.ReadUInt16Array(stream, 3, " },
            { "template unsigned int(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "template unsigned int(8)[]", "IsoReaderWriter.ReadUInt8Array(stream, " },
            { "template unsigned int(8)", "IsoReaderWriter.ReadUInt8(stream, " },
            { "int(64)", "IsoReaderWriter.ReadInt64(stream, " },
            { "int(32)", "IsoReaderWriter.ReadInt32(stream, " },
            { "int(16)", "IsoReaderWriter.ReadInt16(stream, " },
            { "int(8)", "IsoReaderWriter.ReadInt8(stream, " },
            { "int(4)", "IsoReaderWriter.ReadBits(stream, 4, " },
            { "int", "IsoReaderWriter.ReadInt32(stream, " },
            { "const bit(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "const bit(1)", "IsoReaderWriter.ReadBit(stream, " },
            { "bit(1)", "IsoReaderWriter.ReadBit(stream, " },
            { "bit(2)", "IsoReaderWriter.ReadBits(stream, 2, " },
            { "bit(3)", "IsoReaderWriter.ReadBits(stream, 3, " },
            { "bit(4)", "IsoReaderWriter.ReadBits(stream, 4, " },
            { "bit(5)", "IsoReaderWriter.ReadBits(stream, 5, " },
            { "bit(6)", "IsoReaderWriter.ReadBits(stream, 6, " },
            { "bit(7)", "IsoReaderWriter.ReadBits(stream, 7, " },
            { "bit(8)[]", "IsoReaderWriter.ReadUInt8Array(stream, " },
            { "bit(8)", "IsoReaderWriter.ReadUInt8(stream, " },
            { "bit(16)[i]", "IsoReaderWriter.ReadUInt16(stream, " },
            { "bit(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "bit(24)", "IsoReaderWriter.ReadBits(stream, 24, " },
            { "bit(31)", "IsoReaderWriter.ReadBits(stream, 31, " },
            { "bit(8 ceil(size / 8) \u2013 size)", "IsoReaderWriter.ReadBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size), " },
            { "bit(8* ps_nalu_length)", "IsoReaderWriter.ReadBytes(stream, ps_nalu_length, " },
            { "bit(8*nalUnitLength)", "IsoReaderWriter.ReadBytes(stream, nalUnitLength, " },
            { "bit(8*sequenceParameterSetLength)", "IsoReaderWriter.ReadBytes(stream, sequenceParameterSetLength, " },
            { "bit(8*pictureParameterSetLength)", "IsoReaderWriter.ReadBytes(stream, pictureParameterSetLength, " },
            { "bit(8*sequenceParameterSetExtLength)", "IsoReaderWriter.ReadBytes(stream, sequenceParameterSetExtLength, " },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "IsoReaderWriter.ReadBytes(stream, (ulong)(num_bytes_constraint_info - 2), " },
            { "bit(8*nal_unit_length)", "IsoReaderWriter.ReadBytes(stream, nal_unit_length, " },
            { "bit(timeStampLength)", "IsoReaderWriter.ReadBytes(stream, timeStampLength, " },
            { "utf8string", "IsoReaderWriter.ReadString(stream, " },
            { "utfstring", "IsoReaderWriter.ReadString(stream, " },
            { "utf8list", "IsoReaderWriter.ReadString(stream, " },
            { "boxstring", "IsoReaderWriter.ReadString(stream, " },
            { "string", "IsoReaderWriter.ReadString(stream, " },
            { "bit(32)[6]", "IsoReaderWriter.ReadUInt32Array(stream, 6, " },
            { "bit(32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "uint(32)", "IsoReaderWriter.ReadUInt32(stream, " },
            { "uint(16)", "IsoReaderWriter.ReadUInt16(stream, " },
            { "uint(64)", "IsoReaderWriter.ReadUInt64(stream, " },
            { "uint(8)[32]", "IsoReaderWriter.ReadBytes(stream, 32, " },
            { "uint(8)", "IsoReaderWriter.ReadUInt8(stream, " },
            { "uint(7)", "IsoReaderWriter.ReadBits(stream, 7, " },
            { "uint(1)", "IsoReaderWriter.ReadBits(stream, 1, " },
            { "signed   int(64)", "IsoReaderWriter.ReadInt64(stream, " },
            { "signed int(32)", "IsoReaderWriter.ReadInt32(stream, " },
            { "signed int (16)", "IsoReaderWriter.ReadInt16(stream, " },
            { "signed int(16)[grid_pos_view_id[i]]", "IsoReaderWriter.ReadInt16(stream, " },
            { "signed int(16)", "IsoReaderWriter.ReadInt16(stream, " },
            { "signed int (8)", "IsoReaderWriter.ReadInt8(stream, " },
            { "signed int(64)", "IsoReaderWriter.ReadInt64(stream, " },
            { "signed   int(32)", "IsoReaderWriter.ReadInt32(stream, " },
            { "signed   int(8)", "IsoReaderWriter.ReadInt8(stream, " },
            { "Box()[]", "IsoReaderWriter.ReadBox(stream, " },
            { "Box[]", "IsoReaderWriter.ReadBox(stream, " },
            { "Box", "IsoReaderWriter.ReadBox(stream, " },
            { "SchemeTypeBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SchemeInformationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemPropertyContainerBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemPropertyAssociationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemPropertyAssociationBox[]", "IsoReaderWriter.ReadBox(stream, " },
            { "char", "IsoReaderWriter.ReadInt8(stream, " },
            { "loudness", "IsoReaderWriter.ReadInt32(stream, " },
            { "ICC_profile", "IsoReaderWriter.ReadClass(stream, " },
            { "OriginalFormatBox(fmt)", "IsoReaderWriter.ReadBox(stream, " },
            { "DataEntryBaseBox(entry_type, entry_flags)", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemInfoEntry[ entry_count ]", "IsoReaderWriter.ReadClass(stream, " },
            { "TypeCombinationBox[]", "IsoReaderWriter.ReadBox(stream, " },
            { "FilePartitionBox", "IsoReaderWriter.ReadBox(stream, " },
            { "FECReservoirBox", "IsoReaderWriter.ReadBox(stream, " },
            { "FileReservoirBox", "IsoReaderWriter.ReadBox(stream, " },
            { "PartitionEntry[ entry_count ]", "IsoReaderWriter.ReadClass(stream, entry_count, " },
            { "FDSessionGroupBox", "IsoReaderWriter.ReadBox(stream, " },
            { "GroupIdToNameBox", "IsoReaderWriter.ReadBox(stream, " },
            { "base64string", "IsoReaderWriter.ReadString(stream, " },
            { "ProtectionSchemeInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SingleItemTypeReferenceBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SingleItemTypeReferenceBox[]", "IsoReaderWriter.ReadBox(stream, " },
            { "SingleItemTypeReferenceBoxLarge", "IsoReaderWriter.ReadBox(stream, " },
            { "SingleItemTypeReferenceBoxLarge[]", "IsoReaderWriter.ReadBox(stream, " },
            { "HandlerBox(handler_type)", "IsoReaderWriter.ReadBox(stream, " },
            { "PrimaryItemBox", "IsoReaderWriter.ReadBox(stream, " },
            { "DataInformationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemLocationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemProtectionBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "IPMPControlBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemReferenceBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ItemDataBox", "IsoReaderWriter.ReadBox(stream, " },
            { "TrackReferenceTypeBox []", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataKeyBox[]", "IsoReaderWriter.ReadBox(stream, " },
            { "TierInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MultiviewRelationAttributeBox", "IsoReaderWriter.ReadBox(stream, " },
            { "TierBitRateBox", "IsoReaderWriter.ReadBox(stream, " },
            { "BufferingBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MultiviewSceneInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MVDDecoderConfigurationRecord", "IsoReaderWriter.ReadClass(stream, " },
            { "MVDDepthResolutionBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MVCDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "AVCDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "HEVCDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "LHEVCDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "SVCDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "HEVCTileTierLevelConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "EVCDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "VvcDecoderConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "EVCSliceComponentTrackConfigurationRecord()", "IsoReaderWriter.ReadClass(stream, " },
            { "SampleGroupDescriptionEntry (grouping_type)", "IsoReaderWriter.ReadClass(stream, " },
            { "Descriptor[0 .. 255]", "IsoReaderWriter.ReadClass(stream, " },
            { "Descriptor", "IsoReaderWriter.ReadClass(stream, " },
            { "WebVTTConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "WebVTTSourceLabelBox", "IsoReaderWriter.ReadBox(stream, " },
            { "OperatingPointsRecord", "IsoReaderWriter.ReadClass(stream, " },
            { "VvcSubpicIDEntry", "IsoReaderWriter.ReadClass(stream, " },
            { "VvcSubpicOrderEntry", "IsoReaderWriter.ReadClass(stream, " },
            { "URIInitBox", "IsoReaderWriter.ReadBox(stream, " },
            { "URIBox", "IsoReaderWriter.ReadBox(stream, " },
            { "URIbox", "IsoReaderWriter.ReadBox(stream, " },
            { "CleanApertureBox", "IsoReaderWriter.ReadBox(stream, " },
            { "PixelAspectRatioBox", "IsoReaderWriter.ReadBox(stream, " },
            { "DownMixInstructions() []", "IsoReaderWriter.ReadClass(stream, " },
            { "DRCCoefficientsBasic() []", "IsoReaderWriter.ReadClass(stream, " },
            { "DRCInstructionsBasic() []", "IsoReaderWriter.ReadClass(stream, " },
            { "DRCCoefficientsUniDRC() []", "IsoReaderWriter.ReadClass(stream, " },
            { "DRCInstructionsUniDRC() []", "IsoReaderWriter.ReadClass(stream, " },
            { "HEVCConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "LHEVCConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "AVCConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SVCConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ScalabilityInformationSEIBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SVCPriorityAssignmentBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ViewScalabilityInformationSEIBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ViewIdentifierBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MVCConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MVCViewPriorityAssignmentBox", "IsoReaderWriter.ReadBox(stream, " },
            { "IntrinsicCameraParametersBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ExtrinsicCameraParametersBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MVCDConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MVDScalabilityInformationSEIBox", "IsoReaderWriter.ReadBox(stream, " },
            { "A3DConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "VvcOperatingPointsRecord", "IsoReaderWriter.ReadClass(stream, " },
            { "VVCSubpicIDRewritingInfomationStruct()", "IsoReaderWriter.ReadClass(stream, " },
            { "MPEG4ExtensionDescriptorsBox ()", "IsoReaderWriter.ReadBox(stream, " },
            { "MPEG4ExtensionDescriptorsBox()", "IsoReaderWriter.ReadBox(stream, " },
            { "MPEG4ExtensionDescriptorsBox", "IsoReaderWriter.ReadBox(stream, " },
            { "bit(8*dci_nal_unit_length)", "IsoReaderWriter.ReadBytes(stream, dci_nal_unit_length, " },
            { "DependencyInfo[numReferences]", "IsoReaderWriter.ReadClass(stream, numReferences, " },
            { "VvcPTLRecord(0)[i]", "IsoReaderWriter.ReadClass(stream, " },
            { "EVCSliceComponentTrackConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SVCMetadataSampleConfigBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SVCPriorityLayerInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "EVCConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "VvcNALUConfigBox", "IsoReaderWriter.ReadBox(stream, " },
            { "VvcConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "HEVCTileConfigurationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataKeyTableBox()", "IsoReaderWriter.ReadBox(stream, " },
            { "BitRateBox ()", "IsoReaderWriter.ReadBox(stream, " },
            { "char[count]", "IsoReaderWriter.ReadUInt8Array(stream, count, " },
            { "signed int(32)[ c ]", "IsoReaderWriter.ReadInt32(stream, " },
            { "unsigned int(8)[]", "IsoReaderWriter.ReadUInt8Array(stream, " },
            { "unsigned int(8)[i]", "IsoReaderWriter.ReadUInt8(stream, " },
            { "unsigned int(6)[i]", "IsoReaderWriter.ReadBits(stream, 6, " },
            { "unsigned int(6)[i][j]", "IsoReaderWriter.ReadBits(stream, 6, " },
            { "unsigned int(1)[i][j]", "IsoReaderWriter.ReadBits(stream, 1, " },
            { "unsigned int(9)[i]", "IsoReaderWriter.ReadBits(stream, 9, " },
            { "unsigned int(32)[]", "IsoReaderWriter.ReadUInt32Array(stream, " },
            { "unsigned int(32)[i]", "IsoReaderWriter.ReadUInt32(stream, " },
            { "unsigned int(32)[j]", "IsoReaderWriter.ReadUInt32(stream, " },
            { "unsigned int(8)[j][k]", "IsoReaderWriter.ReadUInt8(stream, " },
            { "signed   int(64)[j][k]", "IsoReaderWriter.ReadInt64(stream, " },
            { "unsigned int(8)[j]", "IsoReaderWriter.ReadUInt8(stream, " },
            { "signed   int(64)[j]", "IsoReaderWriter.ReadInt64(stream, " },
            { "char[]", "IsoReaderWriter.ReadUInt8Array(stream, " },
            { "string[method_count]", "IsoReaderWriter.ReadStringArray(stream, method_count, " },
            { "ItemInfoExtension", "IsoReaderWriter.ReadBox(stream, " },
            { "SampleGroupDescriptionEntry", "IsoReaderWriter.ReadBox(stream, " },
            { "SampleEntry", "IsoReaderWriter.ReadBox(stream, " },
            { "SampleConstructor", "IsoReaderWriter.ReadBox(stream, " },
            { "InlineConstructor", "IsoReaderWriter.ReadBox(stream, " },
            { "SampleConstructorFromTrackGroup", "IsoReaderWriter.ReadBox(stream, " },
            { "NALUStartInlineConstructor", "IsoReaderWriter.ReadBox(stream, " },
            { "MPEG4BitRateBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ChannelLayout", "IsoReaderWriter.ReadBox(stream, " },
            { "UniDrcConfigExtension", "IsoReaderWriter.ReadBox(stream, " },
            { "SamplingRateBox", "IsoReaderWriter.ReadBox(stream, " },
            { "TextConfigBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataKeyTableBox", "IsoReaderWriter.ReadBox(stream, " },
            { "BitRateBox", "IsoReaderWriter.ReadBox(stream, " },
            { "CompleteTrackInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "TierDependencyBox", "IsoReaderWriter.ReadBox(stream, " },
            { "InitialParameterSetBox", "IsoReaderWriter.ReadBox(stream, " },
            { "PriorityRangeBox", "IsoReaderWriter.ReadBox(stream, " },
            { "ViewPriorityBox", "IsoReaderWriter.ReadBox(stream, " },
            { "SVCDependencyRangeBox", "IsoReaderWriter.ReadBox(stream, " },
            { "RectRegionBox", "IsoReaderWriter.ReadBox(stream, " },
            { "IroiInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "TranscodingInfoBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataKeyDeclarationBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataDatatypeBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataLocaleBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataSetupBox", "IsoReaderWriter.ReadBox(stream, " },
            { "MetaDataExtensionsBox", "IsoReaderWriter.ReadBox(stream, " },
            { "TrackLoudnessInfo[]", "IsoReaderWriter.ReadBox(stream, " },
            { "AlbumLoudnessInfo[]", "IsoReaderWriter.ReadBox(stream, " },
            { "VvcPTLRecord(num_sublayers)", "IsoReaderWriter.ReadClass(stream, num_sublayers," },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "IsoReaderWriter.ReadClass(stream, " },
            // descriptors
            { "DecoderConfigDescriptor", "IsoReaderWriter.ReadClass(stream, " },
            { "SLConfigDescriptor", "IsoReaderWriter.ReadClass(stream, " },
            { "IPI_DescrPointer[0 .. 1]", "IsoReaderWriter.ReadClass(stream, " },
            { "IP_IdentificationDataSet[0 .. 255]", "IsoReaderWriter.ReadClass(stream, " },
            { "IPMP_DescriptorPointer[0 .. 255]", "IsoReaderWriter.ReadClass(stream, " },
            { "LanguageDescriptor[0 .. 255]", "IsoReaderWriter.ReadClass(stream, " },
            { "QoS_Descriptor[0 .. 1]", "IsoReaderWriter.ReadClass(stream, " },
            { "RegistrationDescriptor[0 .. 1]", "IsoReaderWriter.ReadClass(stream, " },
            { "ExtensionDescriptor[0 .. 255]", "IsoReaderWriter.ReadClass(stream, " },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "IsoReaderWriter.ReadClass(stream, " },
            { "DecoderSpecificInfo[0 .. 1]", "IsoReaderWriter.ReadClass(stream, " },
            { "bit(8)[URLlength]", "IsoReaderWriter.ReadBytes(stream, URLlength, " },
            { "bit(8)[sizeOfInstance-4]", "IsoReaderWriter.ReadBytes(stream, (ulong)(sizeOfInstance - 4), " },
            { "double(32)", "IsoReaderWriter.ReadDouble32(stream, " },
            { "QoS_Qualifier[]", "IsoReaderWriter.ReadClass(stream, " },
        };
        return map[type];
    }

    private static string GetCalculateSizeMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)[ entry_count ]", "(ulong)entry_count * 64" },
            { "unsigned int(64)", "64" },
            { "unsigned int(48)", "48" },
            { "unsigned int(32)[ entry_count ]", "(ulong)entry_count * 32" },
            { "template int(32)[9]", "9 * 32" },
            { "unsigned int(32)[3]", "3 * 32" },
            { "unsigned int(32)", "32" },
            { "unsigned_int(32)", "32" },
            { "unsigned int(24)", "24" },
            { "unsigned int(29)", "29" },
            { "unsigned int(26)", "26" },
            { "unsigned int(16)[i]", "16" },
            { "unsigned int(16)[j]", "16" },
            { "unsigned int(16)[i][j]", "16" },
            { "unsigned int(16)", "16" },
            { "unsigned_int(16)", "16" },
            { "unsigned int(15)", "15" },
            { "unsigned int(12)", "12" },
            { "signed int(12)", "12" },
            { "unsigned int(10)[i][j]", "10" },
            { "unsigned int(10)[i]", "10" },
            { "unsigned int(10)", "10" },
            { "unsigned int(8)[ sample_count ]", "(ulong)sample_count * 8" },
            { "unsigned int(8)[length]", "(ulong)length * 8" },
            { "unsigned int(8)[32]", "32 * 8" },
            { "unsigned int(8)[16]", "16 * 8" },
            { "unsigned int(9)", "9" },
            { "unsigned int(8)", "8" },
            { "unsigned int(7)", "7" },
            { "unsigned int(6)", "6" },
            { "unsigned int(5)[3]", "3 * 5" },
            { "unsigned int(5)", "5" },
            { "unsigned int(4)", "4" },
            { "unsigned int(3)", "3" },
            { "unsigned int(2)[i][j]", "2" },
            { "unsigned int(2)", "2" },
            { "unsigned int(1)[i]", "1" },
            { "unsigned int(1)", "1" },
            { "unsigned int (32)", "32" },
            { "unsigned int(f(pattern_size_code))[i]", "(ulong)pattern_size_code" },
            { "unsigned int(f(index_size_code))[i]", "(ulong)index_size_code" },
            { "unsigned int(f(index_size_code))[j][k]", "(ulong)index_size_code" },
            { "unsigned int(f(count_size_code))[i]", "(ulong)count_size_code" },
            { "unsigned int(base_offset_size*8)", "(ulong)base_offset_size * 8" },
            { "unsigned int(offset_size*8)", "(ulong)offset_size * 8" },
            { "unsigned int(length_size*8)", "(ulong)length_size * 8" },
            { "unsigned int(index_size*8)", "(ulong)index_size * 8" },
            { "unsigned int(field_size)", "(ulong)field_size" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "(ulong)(length_size_of_traf_num+1) * 8" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "(ulong)(length_size_of_trun_num+1) * 8" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "(ulong)(length_size_of_sample_num+1) * 8" },
            { "unsigned int(8*size-64)", "(ulong)(size - 64) * 8" },
            { "unsigned int(subgroupIdLen)[i]", "32" },
            { "const unsigned int(32)[2]", "2 * 32" },
            { "const unsigned int(32)[3]", "3 * 32" },
            { "const unsigned int(32)", "32" },
            { "const unsigned int(16)[3]", "3 * 16" },
            { "const unsigned int(16)", "16" },
            { "const unsigned int(26)", "26" },
            { "template int(32)", "32" },
            { "template int(16)", "16" },
            { "template unsigned int(30)", "30" },
            { "template unsigned int(32)", "32" },
            { "template unsigned int(16)[3]", "3 * 16" },
            { "template unsigned int(16)", "16" },
            { "template unsigned int(8)[]", "(ulong)value.Length * 8" },
            { "template unsigned int(8)", "8" },
            { "int(64)", "64" },
            { "int(32)", "32" },
            { "int(16)", "16" },
            { "int(8)", "8" },
            { "int(4)", "4" },
            { "int", "32" },
            { "const bit(16)", "16" },
            { "const bit(1)", "1" },
            { "bit(1)", "1" },
            { "bit(2)", "2" },
            { "bit(3)", "3" },
            { "bit(4)", "4" },
            { "bit(5)", "5" },
            { "bit(6)", "6" },
            { "bit(7)", "7" },
            { "bit(8)[]", "8 * (ulong)value.Length" },
            { "bit(8)", "8" },
            { "bit(16)[i]", "16" },
            { "bit(16)", "16" },
            { "bit(24)", "24" },
            { "bit(31)", "31" },
            { "bit(8 ceil(size / 8) \u2013 size)", "(ulong)(Math.Ceiling(size / 8d) - size) * 8" },
            { "bit(8* ps_nalu_length)", "(ulong)ps_nalu_length * 8" },
            { "bit(8*nalUnitLength)", "(ulong)nalUnitLength * 8" },
            { "bit(8*sequenceParameterSetLength)", "(ulong)sequenceParameterSetLength * 8" },
            { "bit(8*pictureParameterSetLength)", "(ulong)pictureParameterSetLength * 8" },
            { "bit(8*sequenceParameterSetExtLength)", "(ulong)sequenceParameterSetExtLength * 8" },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "(ulong)(num_bytes_constraint_info - 2)" },
            { "bit(8*nal_unit_length)", "(ulong)nal_unit_length * 8" },
            { "bit(timeStampLength)", "(ulong)timeStampLength" },
            { "utf8string", "(ulong)value.Length * 8" },
            { "utfstring", "(ulong)value.Length * 8" },
            { "utf8list", "(ulong)value.Length * 8" },
            { "boxstring", "(ulong)value.Length * 8" },
            { "string", "(ulong)value.Length * 8" },
            { "bit(32)[6]", "6 * 32" },
            { "bit(32)", "32" },
            { "uint(32)", "32" },
            { "uint(16)", "16" },
            { "uint(64)", "64" },
            { "uint(8)[32]", "32 * 8" },
            { "uint(8)", "8" },
            { "uint(7)", "7" },
            { "uint(1)", "1" },
            { "signed   int(64)", "64" },
            { "signed int(32)", "32" },
            { "signed int (16)", "16" },
            { "signed int(16)[grid_pos_view_id[i]]", "16" },
            { "signed int(16)", "16" },
            { "signed int (8)", "8" },
            { "signed int(64)", "64" },
            { "signed   int(32)", "32" },
            { "signed   int(8)", "8" },
            { "Box()[]", "IsoReaderWriter.CalculateSize(value)" },
            { "Box[]", "IsoReaderWriter.CalculateSize(value)" },
            { "Box", "IsoReaderWriter.CalculateSize(value)" },
            { "SchemeTypeBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SchemeInformationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemPropertyContainerBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemPropertyAssociationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemPropertyAssociationBox[]", "IsoReaderWriter.CalculateSize(value)" },
            { "char", "8" },
            { "ICC_profile", "IsoReaderWriter.CalculateClassSize(value)" },
            { "OriginalFormatBox(fmt)", "IsoReaderWriter.CalculateSize(value)" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemInfoEntry[ entry_count ]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "TypeCombinationBox[]", "IsoReaderWriter.CalculateSize(value)" },
            { "FilePartitionBox", "IsoReaderWriter.CalculateSize(value)" },
            { "FECReservoirBox", "IsoReaderWriter.CalculateSize(value)" },
            { "FileReservoirBox", "IsoReaderWriter.CalculateSize(value)" },
            { "PartitionEntry[ entry_count ]", "IsoReaderWriter.CalculateSize(value)" },
            { "FDSessionGroupBox", "IsoReaderWriter.CalculateSize(value)" },
            { "GroupIdToNameBox", "IsoReaderWriter.CalculateSize(value)" },
            { "base64string", "(ulong)value.Length * 8" },
            { "ProtectionSchemeInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SingleItemTypeReferenceBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SingleItemTypeReferenceBox[]", "IsoReaderWriter.CalculateSize(value)" },
            { "SingleItemTypeReferenceBoxLarge", "IsoReaderWriter.CalculateSize(value)" },
            { "SingleItemTypeReferenceBoxLarge[]", "IsoReaderWriter.CalculateSize(value)" },
            { "HandlerBox(handler_type)", "IsoReaderWriter.CalculateSize(value)" },
            { "PrimaryItemBox", "IsoReaderWriter.CalculateSize(value)" },
            { "DataInformationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemLocationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemProtectionBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "IPMPControlBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemReferenceBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemDataBox", "IsoReaderWriter.CalculateSize(value)" },
            { "TrackReferenceTypeBox []", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataKeyBox[]", "IsoReaderWriter.CalculateSize(value)" },
            { "TierInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MultiviewRelationAttributeBox", "IsoReaderWriter.CalculateSize(value)" },
            { "TierBitRateBox", "IsoReaderWriter.CalculateSize(value)" },
            { "BufferingBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MultiviewSceneInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MVDDecoderConfigurationRecord", "IsoReaderWriter.CalculateClassSize(value)" },
            { "MVDDepthResolutionBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MVCDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "AVCDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "HEVCDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "LHEVCDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "SVCDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "HEVCTileTierLevelConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "EVCDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "VvcDecoderConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "EVCSliceComponentTrackConfigurationRecord()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "SampleGroupDescriptionEntry (grouping_type)", "IsoReaderWriter.CalculateClassSize(value)" },
            { "Descriptor[0 .. 255]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "Descriptor", "IsoReaderWriter.CalculateClassSize(value)" },
            { "WebVTTConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "WebVTTSourceLabelBox", "IsoReaderWriter.CalculateSize(value)" },
            { "OperatingPointsRecord", "IsoReaderWriter.CalculateClassSize(value)" },
            { "VvcSubpicIDEntry", "IsoReaderWriter.CalculateClassSize(value)" },
            { "VvcSubpicOrderEntry", "IsoReaderWriter.CalculateClassSize(value)" },
            { "URIInitBox", "IsoReaderWriter.CalculateSize(value)" },
            { "URIBox", "IsoReaderWriter.CalculateSize(value)" },
            { "URIbox", "IsoReaderWriter.CalculateSize(value)" },
            { "CleanApertureBox", "IsoReaderWriter.CalculateSize(value)" },
            { "PixelAspectRatioBox", "IsoReaderWriter.CalculateSize(value)" },
            { "DownMixInstructions() []", "IsoReaderWriter.CalculateClassSize(value)" },
            { "DRCCoefficientsBasic() []", "IsoReaderWriter.CalculateClassSize(value)" },
            { "DRCInstructionsBasic() []", "IsoReaderWriter.CalculateClassSize(value)" },
            { "DRCCoefficientsUniDRC() []", "IsoReaderWriter.CalculateClassSize(value)" },
            { "DRCInstructionsUniDRC() []", "IsoReaderWriter.CalculateClassSize(value)" },
            { "HEVCConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "LHEVCConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "AVCConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SVCConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ScalabilityInformationSEIBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SVCPriorityAssignmentBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ViewScalabilityInformationSEIBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ViewIdentifierBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MVCConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MVCViewPriorityAssignmentBox", "IsoReaderWriter.CalculateSize(value)" },
            { "IntrinsicCameraParametersBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ExtrinsicCameraParametersBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MVCDConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MVDScalabilityInformationSEIBox", "IsoReaderWriter.CalculateSize(value)" },
            { "A3DConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "VvcOperatingPointsRecord", "IsoReaderWriter.CalculateClassSize(value)" },
            { "VVCSubpicIDRewritingInfomationStruct()", "IsoReaderWriter.CalculateClassSize(value)" },
            { "MPEG4ExtensionDescriptorsBox ()", "IsoReaderWriter.CalculateSize(value)" },
            { "MPEG4ExtensionDescriptorsBox()", "IsoReaderWriter.CalculateSize(value)" },
            { "MPEG4ExtensionDescriptorsBox", "IsoReaderWriter.CalculateSize(value)" },
            { "bit(8*dci_nal_unit_length)", "(ulong)dci_nal_unit_length * 8" },
            { "DependencyInfo[numReferences]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "VvcPTLRecord(0)[i]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "EVCSliceComponentTrackConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SVCMetadataSampleConfigBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SVCPriorityLayerInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "EVCConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "VvcNALUConfigBox", "IsoReaderWriter.CalculateSize(value)" },
            { "VvcConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "HEVCTileConfigurationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataKeyTableBox()", "IsoReaderWriter.CalculateSize(value)" },
            { "BitRateBox ()", "IsoReaderWriter.CalculateSize(value)" },
            { "char[count]", "(ulong)count * 8" },
            { "signed int(32)[ c ]", "32" },
            { "unsigned int(8)[]", "(ulong)value.Length * 8" },
            { "unsigned int(8)[i]", "8" },
            { "unsigned int(6)[i]", "6" },
            { "unsigned int(6)[i][j]", "6" },
            { "unsigned int(1)[i][j]", "1" },
            { "unsigned int(9)[i]", "9" },
            { "unsigned int(32)[]", "32" },
            { "unsigned int(32)[i]", "32" },
            { "unsigned int(32)[j]", "32" },
            { "unsigned int(8)[j][k]", "8" },
            { "unsigned int(8)[j]", "8" },
            { "signed   int(64)[j][k]", "64" },
            { "signed   int(64)[j]", "64" },
            { "char[]", "(ulong)value.Length * 8" },
            { "string[method_count]", "IsoReaderWriter.CalculateSize(value)" },
            { "ItemInfoExtension", "IsoReaderWriter.CalculateSize(value)" },
            { "SampleGroupDescriptionEntry", "IsoReaderWriter.CalculateSize(value)" },
            { "SampleEntry", "IsoReaderWriter.CalculateSize(value)" },
            { "SampleConstructor", "IsoReaderWriter.CalculateSize(value)" },
            { "InlineConstructor", "IsoReaderWriter.CalculateSize(value)" },
            { "SampleConstructorFromTrackGroup", "IsoReaderWriter.CalculateSize(value)" },
            { "NALUStartInlineConstructor", "IsoReaderWriter.CalculateSize(value)" },
            { "MPEG4BitRateBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ChannelLayout", "IsoReaderWriter.CalculateSize(value)" },
            { "UniDrcConfigExtension", "IsoReaderWriter.CalculateSize(value)" },
            { "SamplingRateBox", "IsoReaderWriter.CalculateSize(value)" },
            { "TextConfigBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataKeyTableBox", "IsoReaderWriter.CalculateSize(value)" },
            { "BitRateBox", "IsoReaderWriter.CalculateSize(value)" },
            { "CompleteTrackInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "TierDependencyBox", "IsoReaderWriter.CalculateSize(value)" },
            { "InitialParameterSetBox", "IsoReaderWriter.CalculateSize(value)" },
            { "PriorityRangeBox", "IsoReaderWriter.CalculateSize(value)" },
            { "ViewPriorityBox", "IsoReaderWriter.CalculateSize(value)" },
            { "SVCDependencyRangeBox", "IsoReaderWriter.CalculateSize(value)" },
            { "RectRegionBox", "IsoReaderWriter.CalculateSize(value)" },
            { "IroiInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "TranscodingInfoBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataKeyDeclarationBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataDatatypeBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataLocaleBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataSetupBox", "IsoReaderWriter.CalculateSize(value)" },
            { "MetaDataExtensionsBox", "IsoReaderWriter.CalculateSize(value)" },
            { "TrackLoudnessInfo[]", "IsoReaderWriter.CalculateSize(value)" },
            { "AlbumLoudnessInfo[]", "IsoReaderWriter.CalculateSize(value)" },
            { "VvcPTLRecord(num_sublayers)", "IsoReaderWriter.CalculateClassSize(value)" },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "IsoReaderWriter.CalculateClassSize(value)" },
            // descriptors
            { "DecoderConfigDescriptor", "IsoReaderWriter.CalculateClassSize(value)" },
            { "SLConfigDescriptor", "IsoReaderWriter.CalculateClassSize(value)" },
            { "IPI_DescrPointer[0 .. 1]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "IP_IdentificationDataSet[0 .. 255]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "IPMP_DescriptorPointer[0 .. 255]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "LanguageDescriptor[0 .. 255]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "QoS_Descriptor[0 .. 1]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "RegistrationDescriptor[0 .. 1]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "ExtensionDescriptor[0 .. 255]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "DecoderSpecificInfo[0 .. 1]", "IsoReaderWriter.CalculateClassSize(value)" },
            { "bit(8)[URLlength]", "(ulong)(URLlength * 8)" },
            { "bit(8)[sizeOfInstance-4]", "(ulong)(sizeOfInstance - 4)" },
            { "double(32)", "32" },
            { "QoS_Qualifier[]", "IsoReaderWriter.CalculateClassSize(value)" },
       };
        return map[type];
    }

    private static string GetWriteMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)[ entry_count ]", "IsoReaderWriter.WriteUInt64Array(stream, entry_count, " },
            { "unsigned int(64)", "IsoReaderWriter.WriteUInt64(stream, " },
            { "unsigned int(48)", "IsoReaderWriter.WriteUInt48(stream, " },
            { "template int(32)[9]", "IsoReaderWriter.WriteUInt32Array(stream, 9, " },
            { "unsigned int(32)[ entry_count ]", "IsoReaderWriter.WriteUInt32Array(stream, entry_count, " },
            { "unsigned int(32)[3]", "IsoReaderWriter.WriteUInt32Array(stream, 3, " },
            { "unsigned int(32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "unsigned_int(32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "unsigned int(24)", "IsoReaderWriter.WriteUInt24(stream, " },
            { "unsigned int(29)", "IsoReaderWriter.WriteBits(stream, 29, " },
            { "unsigned int(26)", "IsoReaderWriter.WriteBits(stream, 26, " },
            { "unsigned int(16)[i]", "IsoReaderWriter.WriteUInt16(stream, " },
            { "unsigned int(16)[j]", "IsoReaderWriter.WriteUInt16(stream, " },
            { "unsigned int(16)[i][j]", "IsoReaderWriter.WriteUInt16(stream, " },
            { "unsigned int(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "unsigned_int(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "unsigned int(15)", "IsoReaderWriter.WriteBits(stream, 15, " },
            { "unsigned int(12)", "IsoReaderWriter.WriteBits(stream, 12, " },
            { "signed int(12)", "IsoReaderWriter.WriteBits(stream, 12, " },
            { "unsigned int(10)[i][j]", "IsoReaderWriter.WriteBits(stream, 10, " },
            { "unsigned int(10)[i]", "IsoReaderWriter.WriteBits(stream, 10, " },
            { "unsigned int(10)", "IsoReaderWriter.WriteBits(stream, 10, " },
            { "unsigned int(8)[ sample_count ]", "IsoReaderWriter.WriteBytes(stream, sample_count, " },
            { "unsigned int(8)[length]", "IsoReaderWriter.WriteBytes(stream, length, " },
            { "unsigned int(8)[32]", "IsoReaderWriter.WriteBytes(stream, 32, " },
            { "unsigned int(8)[16]", "IsoReaderWriter.WriteBytes(stream, 16, " },
            { "unsigned int(9)", "IsoReaderWriter.WriteBits(stream, 9, " },
            { "unsigned int(8)", "IsoReaderWriter.WriteUInt8(stream, " },
            { "unsigned int(7)", "IsoReaderWriter.WriteBits(stream, 7, " },
            { "unsigned int(6)", "IsoReaderWriter.WriteBits(stream, 6, " },
            { "unsigned int(5)[3]", "IsoReaderWriter.WriteBitsArray(stream, 5, 3, " },
            { "unsigned int(5)", "IsoReaderWriter.WriteBits(stream, 5, " },
            { "unsigned int(4)", "IsoReaderWriter.WriteBits(stream, 4, " },
            { "unsigned int(3)", "IsoReaderWriter.WriteBits(stream, 3, " },
            { "unsigned int(2)[i][j]", "IsoReaderWriter.WriteBits(stream, 2, " },
            { "unsigned int(2)", "IsoReaderWriter.WriteBits(stream, 2, " },
            { "unsigned int(1)[i]", "IsoReaderWriter.WriteBit(stream, " },
            { "unsigned int(1)", "IsoReaderWriter.WriteBit(stream, " },
            { "unsigned int (32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "unsigned int(f(pattern_size_code))[i]", "IsoReaderWriter.WriteBits(stream, pattern_size_code, " },
            { "unsigned int(f(index_size_code))[i]", "IsoReaderWriter.WriteBits(stream, index_size_code, " },
            { "unsigned int(f(index_size_code))[j][k]", "IsoReaderWriter.WriteBits(stream, index_size_code, " },
            { "unsigned int(f(count_size_code))[i]", "IsoReaderWriter.WriteBits(stream, count_size_code, " },
            { "unsigned int(base_offset_size*8)", "IsoReaderWriter.WriteBytes(stream, base_offset_size, " },
            { "unsigned int(offset_size*8)", "IsoReaderWriter.WriteBytes(stream, offset_size, " },
            { "unsigned int(length_size*8)", "IsoReaderWriter.WriteBytes(stream, length_size, " },
            { "unsigned int(index_size*8)", "IsoReaderWriter.WriteBytes(stream, index_size, " },
            { "unsigned int(field_size)", "IsoReaderWriter.WriteBytes(stream, field_size, " },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_traf_num+1), " },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_trun_num+1), " },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "IsoReaderWriter.WriteBytes(stream, (ulong)(length_size_of_sample_num+1), " },
            { "unsigned int(8*size-64)", "IsoReaderWriter.WriteBytes(stream, size-64, " },
            { "unsigned int(subgroupIdLen)[i]", "IsoReaderWriter.WriteUInt32(stream, " },
            { "const unsigned int(32)[2]", "IsoReaderWriter.WriteUInt32Array(stream, 2, " },
            { "const unsigned int(32)[3]", "IsoReaderWriter.WriteUInt32Array(stream, 3, " },
            { "const unsigned int(32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "const unsigned int(16)[3]", "IsoReaderWriter.WriteUInt16Array(stream, 3, " },
            { "const unsigned int(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "const unsigned int(26)", "IsoReaderWriter.WriteBits(stream, 26, " },
            { "template int(32)", "IsoReaderWriter.WriteInt32(stream, " },
            { "template int(16)", "IsoReaderWriter.WriteInt16(stream, " },
            { "template unsigned int(30)", "IsoReaderWriter.WriteBits(stream, 30, " },
            { "template unsigned int(32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "template unsigned int(16)[3]", "IsoReaderWriter.WriteUInt16Array(stream, 3, " },
            { "template unsigned int(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "template unsigned int(8)[]", "IsoReaderWriter.WriteUInt8Array(stream, " },
            { "template unsigned int(8)", "IsoReaderWriter.WriteUInt8(stream, " },
            { "int(64)", "IsoReaderWriter.WriteInt64(stream, " },
            { "int(32)", "IsoReaderWriter.WriteInt32(stream, " },
            { "int(16)", "IsoReaderWriter.WriteInt16(stream, " },
            { "int(8)", "IsoReaderWriter.WriteInt8(stream, " },
            { "int(4)", "IsoReaderWriter.WriteBits(stream, 4, " },
            { "int", "IsoReaderWriter.WriteInt32(stream, " },
            { "const bit(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "const bit(1)", "IsoReaderWriter.WriteBit(stream, " },
            { "bit(1)", "IsoReaderWriter.WriteBit(stream, " },
            { "bit(2)", "IsoReaderWriter.WriteBits(stream, 2, " },
            { "bit(3)", "IsoReaderWriter.WriteBits(stream, 3, " },
            { "bit(4)", "IsoReaderWriter.WriteBits(stream, 4, " },
            { "bit(5)", "IsoReaderWriter.WriteBits(stream, 5, " },
            { "bit(6)", "IsoReaderWriter.WriteBits(stream, 6, " },
            { "bit(7)", "IsoReaderWriter.WriteBits(stream, 7, " },
            { "bit(8)[]", "IsoReaderWriter.WriteUInt8Array(stream, " },
            { "bit(8)", "IsoReaderWriter.WriteUInt8(stream, " },
            { "bit(16)[i]", "IsoReaderWriter.WriteUInt16(stream, " },
            { "bit(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "bit(24)", "IsoReaderWriter.WriteBits(stream, 24, " },
            { "bit(31)", "IsoReaderWriter.WriteBits(stream, 31, " },
            { "bit(8 ceil(size / 8) \u2013 size)", "IsoReaderWriter.WriteBytes(stream, (ulong)(Math.Ceiling(size / 8d) - size), " },
            { "bit(8* ps_nalu_length)", "IsoReaderWriter.WriteBytes(stream, ps_nalu_length, " },
            { "bit(8*nalUnitLength)", "IsoReaderWriter.WriteBytes(stream, nalUnitLength, " },
            { "bit(8*sequenceParameterSetLength)", "IsoReaderWriter.WriteBytes(stream, sequenceParameterSetLength, " },
            { "bit(8*pictureParameterSetLength)", "IsoReaderWriter.WriteBytes(stream, pictureParameterSetLength, " },
            { "bit(8*sequenceParameterSetExtLength)", "IsoReaderWriter.WriteBytes(stream, sequenceParameterSetExtLength, " },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "IsoReaderWriter.WriteBytes(stream, (ulong)(num_bytes_constraint_info - 2), " },
            { "bit(8*nal_unit_length)", "IsoReaderWriter.WriteBytes(stream, nal_unit_length, " },
            { "bit(timeStampLength)", "IsoReaderWriter.WriteBytes(stream, timeStampLength, " },
            { "utf8string", "IsoReaderWriter.WriteString(stream, " },
            { "utfstring", "IsoReaderWriter.WriteString(stream, " },
            { "utf8list", "IsoReaderWriter.WriteString(stream, " },
            { "boxstring", "IsoReaderWriter.WriteString(stream, " },
            { "string", "IsoReaderWriter.WriteString(stream, " },
            { "bit(32)[6]", "IsoReaderWriter.WriteUInt32Array(stream, 6, " },
            { "bit(32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "uint(32)", "IsoReaderWriter.WriteUInt32(stream, " },
            { "uint(16)", "IsoReaderWriter.WriteUInt16(stream, " },
            { "uint(64)", "IsoReaderWriter.WriteUInt64(stream, " },
            { "uint(8)[32]", "IsoReaderWriter.WriteBytes(stream, 32, " },
            { "uint(8)", "IsoReaderWriter.WriteUInt8(stream, " },
            { "uint(7)", "IsoReaderWriter.WriteBits(stream, 7, " },
            { "uint(1)", "IsoReaderWriter.WriteBits(stream, 1, " },
            { "signed   int(64)", "IsoReaderWriter.WriteInt64(stream, " },
            { "signed int(32)", "IsoReaderWriter.WriteInt32(stream, " },
            { "signed int (16)", "IsoReaderWriter.WriteInt16(stream, " },
            { "signed int(16)[grid_pos_view_id[i]]", "IsoReaderWriter.WriteInt16(stream, " },
            { "signed int(16)", "IsoReaderWriter.WriteInt16(stream, " },
            { "signed int (8)", "IsoReaderWriter.WriteInt8(stream, " },
            { "signed int(64)", "IsoReaderWriter.WriteInt64(stream, " },
            { "signed   int(32)", "IsoReaderWriter.WriteInt32(stream, " },
            { "signed   int(8)", "IsoReaderWriter.WriteInt8(stream, " },
            { "Box()[]", "IsoReaderWriter.WriteBox(stream, " },
            { "Box[]", "IsoReaderWriter.WriteBox(stream, " },
            { "Box", "IsoReaderWriter.WriteBox(stream, " },
            { "SchemeTypeBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SchemeInformationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemPropertyContainerBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemPropertyAssociationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemPropertyAssociationBox[]", "IsoReaderWriter.WriteBox(stream, " },
            { "char", "IsoReaderWriter.WriteInt8(stream, " },
            { "ICC_profile", "IsoReaderWriter.WriteClass(stream, " },
            { "OriginalFormatBox(fmt)", "IsoReaderWriter.WriteBox(stream, " },
            { "DataEntryBaseBox(entry_type, entry_flags)", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemInfoEntry[ entry_count ]", "IsoReaderWriter.WriteClass(stream, entry_count, " },
            { "TypeCombinationBox[]", "IsoReaderWriter.WriteBox(stream, " },
            { "FilePartitionBox", "IsoReaderWriter.WriteBox(stream, " },
            { "FECReservoirBox", "IsoReaderWriter.WriteBox(stream, " },
            { "FileReservoirBox", "IsoReaderWriter.WriteBox(stream, " },
            { "PartitionEntry[ entry_count ]", "IsoReaderWriter.WriteClass(stream, entry_count, " },
            { "FDSessionGroupBox", "IsoReaderWriter.WriteBox(stream, " },
            { "GroupIdToNameBox", "IsoReaderWriter.WriteBox(stream, " },
            { "base64string", "IsoReaderWriter.WriteString(stream, " },
            { "ProtectionSchemeInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SingleItemTypeReferenceBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SingleItemTypeReferenceBox[]", "IsoReaderWriter.WriteBox(stream, " },
            { "SingleItemTypeReferenceBoxLarge", "IsoReaderWriter.WriteBox(stream, " },
            { "SingleItemTypeReferenceBoxLarge[]", "IsoReaderWriter.WriteBox(stream, " },
            { "HandlerBox(handler_type)", "IsoReaderWriter.WriteBox(stream, " },
            { "PrimaryItemBox", "IsoReaderWriter.WriteBox(stream, " },
            { "DataInformationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemLocationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemProtectionBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "IPMPControlBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemReferenceBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ItemDataBox", "IsoReaderWriter.WriteBox(stream, " },
            { "TrackReferenceTypeBox []", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataKeyBox[]", "IsoReaderWriter.WriteBox(stream, " },
            { "TierInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MultiviewRelationAttributeBox", "IsoReaderWriter.WriteBox(stream, " },
            { "TierBitRateBox", "IsoReaderWriter.WriteBox(stream, " },
            { "BufferingBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MultiviewSceneInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MVDDecoderConfigurationRecord", "IsoReaderWriter.WriteClass(stream, " },
            { "MVDDepthResolutionBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "AVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "HEVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "LHEVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "SVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "HEVCTileTierLevelConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "EVCDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "VvcDecoderConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "EVCSliceComponentTrackConfigurationRecord()", "IsoReaderWriter.WriteClass(stream, " },
            { "SampleGroupDescriptionEntry (grouping_type)", "IsoReaderWriter.WriteClass(stream, " },
            { "Descriptor[0 .. 255]", "IsoReaderWriter.WriteClass(stream, " },
            { "Descriptor", "IsoReaderWriter.WriteClass(stream, " },
            { "WebVTTConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "WebVTTSourceLabelBox", "IsoReaderWriter.WriteBox(stream, " },
            { "OperatingPointsRecord", "IsoReaderWriter.WriteClass(stream, " },
            { "VvcSubpicIDEntry", "IsoReaderWriter.WriteClass(stream, " },
            { "VvcSubpicOrderEntry", "IsoReaderWriter.WriteClass(stream, " },
            { "URIInitBox", "IsoReaderWriter.WriteBox(stream, " },
            { "URIBox", "IsoReaderWriter.WriteBox(stream, " },
            { "URIbox", "IsoReaderWriter.WriteBox(stream, " },
            { "CleanApertureBox", "IsoReaderWriter.WriteBox(stream, " },
            { "PixelAspectRatioBox", "IsoReaderWriter.WriteBox(stream, " },
            { "DownMixInstructions() []", "IsoReaderWriter.WriteClass(stream, " },
            { "DRCCoefficientsBasic() []", "IsoReaderWriter.WriteClass(stream, " },
            { "DRCInstructionsBasic() []", "IsoReaderWriter.WriteClass(stream, " },
            { "DRCCoefficientsUniDRC() []", "IsoReaderWriter.WriteClass(stream, " },
            { "DRCInstructionsUniDRC() []", "IsoReaderWriter.WriteClass(stream, " },
            { "HEVCConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "LHEVCConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "AVCConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SVCConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ScalabilityInformationSEIBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SVCPriorityAssignmentBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ViewScalabilityInformationSEIBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ViewIdentifierBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MVCConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MVCViewPriorityAssignmentBox", "IsoReaderWriter.WriteBox(stream, " },
            { "IntrinsicCameraParametersBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ExtrinsicCameraParametersBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MVCDConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MVDScalabilityInformationSEIBox", "IsoReaderWriter.WriteBox(stream, " },
            { "A3DConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "VvcOperatingPointsRecord", "IsoReaderWriter.WriteClass(stream, " },
            { "VVCSubpicIDRewritingInfomationStruct()", "IsoReaderWriter.WriteClass(stream, " },
            { "MPEG4ExtensionDescriptorsBox ()", "IsoReaderWriter.WriteBox(stream, " },
            { "MPEG4ExtensionDescriptorsBox()", "IsoReaderWriter.WriteBox(stream, " },
            { "MPEG4ExtensionDescriptorsBox", "IsoReaderWriter.WriteBox(stream, " },
            { "bit(8*dci_nal_unit_length)", "IsoReaderWriter.WriteBytes(stream, dci_nal_unit_length, " },
            { "DependencyInfo[numReferences]", "IsoReaderWriter.WriteClass(stream, numReferences, " },
            { "VvcPTLRecord(0)[i]", "IsoReaderWriter.WriteClass(stream, " },
            { "EVCSliceComponentTrackConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SVCMetadataSampleConfigBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SVCPriorityLayerInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "EVCConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "VvcNALUConfigBox", "IsoReaderWriter.WriteBox(stream, " },
            { "VvcConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "HEVCTileConfigurationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataKeyTableBox()", "IsoReaderWriter.WriteBox(stream, " },
            { "BitRateBox ()", "IsoReaderWriter.WriteBox(stream, " },
            { "char[count]", "IsoReaderWriter.WriteUInt8Array(stream, count, " },
            { "signed int(32)[ c ]", "IsoReaderWriter.WriteInt32(stream, " },
            { "unsigned int(8)[]", "IsoReaderWriter.WriteUInt8Array(stream, " },
            { "unsigned int(8)[i]", "IsoReaderWriter.WriteUInt8(stream, " },
            { "unsigned int(6)[i]", "IsoReaderWriter.WriteBits(stream, 6, " },
            { "unsigned int(6)[i][j]", "IsoReaderWriter.WriteBits(stream, 6, " },
            { "unsigned int(1)[i][j]", "IsoReaderWriter.WriteBits(stream, 1, " },
            { "unsigned int(9)[i]", "IsoReaderWriter.WriteBits(stream, 9, " },
            { "unsigned int(32)[]", "IsoReaderWriter.WriteUInt32Array(stream, " },
            { "unsigned int(32)[i]", "IsoReaderWriter.WriteUInt32(stream, " },
            { "unsigned int(32)[j]", "IsoReaderWriter.WriteUInt32(stream, " },
            { "unsigned int(8)[j][k]", "IsoReaderWriter.WriteUInt8(stream, " },
            { "signed   int(64)[j][k]", "IsoReaderWriter.WriteInt64(stream, " },
            { "unsigned int(8)[j]", "IsoReaderWriter.WriteUInt8(stream, " },
            { "signed   int(64)[j]", "IsoReaderWriter.WriteInt64(stream, " },
            { "char[]", "IsoReaderWriter.WriteUInt8Array(stream, " },
            { "string[method_count]", "IsoReaderWriter.WriteStringArray(stream, method_count, " },
             { "ItemInfoExtension", "IsoReaderWriter.WriteBox(stream, " },
            { "SampleGroupDescriptionEntry", "IsoReaderWriter.WriteBox(stream, " },
            { "SampleEntry", "IsoReaderWriter.WriteBox(stream, " },
            { "SampleConstructor", "IsoReaderWriter.WriteBox(stream, " },
            { "InlineConstructor", "IsoReaderWriter.WriteBox(stream, " },
            { "SampleConstructorFromTrackGroup", "IsoReaderWriter.WriteBox(stream, " },
            { "NALUStartInlineConstructor", "IsoReaderWriter.WriteBox(stream, " },
            { "MPEG4BitRateBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ChannelLayout", "IsoReaderWriter.WriteBox(stream, " },
            { "UniDrcConfigExtension", "IsoReaderWriter.WriteBox(stream, " },
            { "SamplingRateBox", "IsoReaderWriter.WriteBox(stream, " },
            { "TextConfigBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataKeyTableBox", "IsoReaderWriter.WriteBox(stream, " },
            { "BitRateBox", "IsoReaderWriter.WriteBox(stream, " },
            { "CompleteTrackInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "TierDependencyBox", "IsoReaderWriter.WriteBox(stream, " },
            { "InitialParameterSetBox", "IsoReaderWriter.WriteBox(stream, " },
            { "PriorityRangeBox", "IsoReaderWriter.WriteBox(stream, " },
            { "ViewPriorityBox", "IsoReaderWriter.WriteBox(stream, " },
            { "SVCDependencyRangeBox", "IsoReaderWriter.WriteBox(stream, " },
            { "RectRegionBox", "IsoReaderWriter.WriteBox(stream, " },
            { "IroiInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "TranscodingInfoBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataKeyDeclarationBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataDatatypeBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataLocaleBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataSetupBox", "IsoReaderWriter.WriteBox(stream, " },
            { "MetaDataExtensionsBox", "IsoReaderWriter.WriteBox(stream, " },
            { "TrackLoudnessInfo[]", "IsoReaderWriter.WriteBox(stream, " },
            { "AlbumLoudnessInfo[]", "IsoReaderWriter.WriteBox(stream, " },
            { "VvcPTLRecord(num_sublayers)", "IsoReaderWriter.WriteClass(stream, num_sublayers, " },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "IsoReaderWriter.WriteClass(stream, " },
            // descriptors
            { "DecoderConfigDescriptor", "IsoReaderWriter.WriteClass(stream, " },
            { "SLConfigDescriptor", "IsoReaderWriter.WriteClass(stream, " },
            { "IPI_DescrPointer[0 .. 1]", "IsoReaderWriter.WriteClass(stream, " },
            { "IP_IdentificationDataSet[0 .. 255]", "IsoReaderWriter.WriteClass(stream, " },
            { "IPMP_DescriptorPointer[0 .. 255]", "IsoReaderWriter.WriteClass(stream, " },
            { "LanguageDescriptor[0 .. 255]", "IsoReaderWriter.WriteClass(stream, " },
            { "QoS_Descriptor[0 .. 1]", "IsoReaderWriter.WriteClass(stream, " },
            { "RegistrationDescriptor[0 .. 1]", "IsoReaderWriter.WriteClass(stream, " },
            { "ExtensionDescriptor[0 .. 255]", "IsoReaderWriter.WriteClass(stream, " },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "IsoReaderWriter.WriteClass(stream, " },
            { "DecoderSpecificInfo[0 .. 1]", "IsoReaderWriter.WriteClass(stream, " },
            { "bit(8)[URLlength]", "IsoReaderWriter.WriteBytes(stream, URLlength, " },
            { "bit(8)[sizeOfInstance-4]", "IsoReaderWriter.WriteBytes(stream, (ulong)(sizeOfInstance - 4), " },
            { "double(32)", "IsoReaderWriter.WriteDouble32(stream, " },
            { "QoS_Qualifier[]", "IsoReaderWriter.WriteClass(stream, " },
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
            "int size = 4",
            "size += 5",
            "j=1",
            "j++",
            "subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8",
            "totalPatternLength = 0",
            "sizeOfInstance = sizeOfInstance<<7 | sizeByte",
            "int sizeOfInstance = 0"
        };
        return map.Contains(type);
    }

    private static string GetType(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string> {
            { "unsigned int(64)[ entry_count ]", "ulong[]" },
            { "unsigned int(64)", "ulong" },
            { "unsigned int(48)", "ulong" },
            { "template int(32)[9]", "uint[]" },
            { "unsigned int(32)[ entry_count ]", "uint[]" },
            { "unsigned int(32)[3]", "uint[]" },
            { "unsigned int(32)", "uint" },
            { "unsigned_int(32)", "uint" },
            { "unsigned int(24)", "uint" },
            { "unsigned int(29)", "uint" },
            { "unsigned int(26)", "uint" },
            { "unsigned int(16)[i]", "ushort[]" },
            { "unsigned int(16)[j]", "ushort[]" },
            { "unsigned int(16)[i][j]", "ushort[][]" },
            { "unsigned int(16)", "ushort" },
            { "unsigned_int(16)", "ushort" },
            { "unsigned int(15)", "ushort" },
            { "unsigned int(12)", "ushort" },
            { "signed int(12)", "short" },
            { "unsigned int(10)[i][j]", "ushort[][]" },
            { "unsigned int(10)[i]", "ushort[]" },
            { "unsigned int(10)", "ushort" },
            { "unsigned int(8)[ sample_count ]", "byte[]" },
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
            { "unsigned int(2)[i][j]", "byte[][]" },
            { "unsigned int(2)", "byte" },
            { "unsigned int(1)[i]", "bool[]" },
            { "unsigned int(1)", "bool" },
            { "unsigned int (32)", "uint" },
            { "unsigned int(f(pattern_size_code))[i]", "byte[]" },
            { "unsigned int(f(index_size_code))[j][k]", "byte[][]" },
            { "unsigned int(f(count_size_code))[i]", "byte[]" },
            { "unsigned int(base_offset_size*8)", "byte[]" },
            { "unsigned int(offset_size*8)", "byte[]" },
            { "unsigned int(length_size*8)", "byte[]" },
            { "unsigned int(index_size*8)", "byte[]" },
            { "unsigned int(field_size)", "byte[]" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "byte[]" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "byte[]" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "byte[]" },
            { "unsigned int(8*size-64)", "byte[]" },
            { "unsigned int(subgroupIdLen)[i]", "uint[]" },
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
            { "template unsigned int(8)[]", "byte[]" },
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
            { "bit(8)[]", "byte[]" },
            { "bit(8)", "byte" },
            { "bit(16)[i]", "ushort[]" },
            { "bit(16)", "ushort" },
            { "bit(24)", "uint" },
            { "bit(31)", "uint" },
            { "bit(8 ceil(size / 8) \u2013 size)", "byte[]" },
            { "bit(8* ps_nalu_length)", "byte[]" },
            { "bit(8*nalUnitLength)", "byte[]" },
            { "bit(8*sequenceParameterSetLength)", "byte[]" },
            { "bit(8*pictureParameterSetLength)", "byte[]" },
            { "bit(8*sequenceParameterSetExtLength)", "byte[]" },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "byte[]" },
            { "bit(8*nal_unit_length)", "byte[]" },
            { "bit(timeStampLength)", "byte[]" },
            { "utf8string", "string" },
            { "utfstring", "string" },
            { "utf8list", "string" },
            { "boxstring", "string" },
            { "string", "string" },
            { "bit(32)[6]", "uint[]" },
            { "bit(32)", "uint" },
            { "uint(32)", "uint" },
            { "uint(16)", "ushort" },
            { "uint(64)", "ulong" },
            { "uint(8)[32]", "byte[]" },
            { "uint(8)", "byte" },
            { "uint(7)", "byte" },
            { "uint(1)", "byte" },
            { "signed   int(64)", "long" },
            { "signed int(32)", "int" },
            { "signed int (16)", "short" },
            { "signed int(16)[grid_pos_view_id[i]]", "short[]" },
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
            { "TypeCombinationBox[]", "TypeCombinationBox[]" },
            { "FilePartitionBox", "FilePartitionBox" },
            { "FECReservoirBox", "FECReservoirBox" },
            { "FileReservoirBox", "FileReservoirBox" },
            { "PartitionEntry[ entry_count ]", "PartitionEntry[]" },
            { "FDSessionGroupBox", "FDSessionGroupBox" },
            { "GroupIdToNameBox", "GroupIdToNameBox" },
            { "base64string", "string" },
            { "ProtectionSchemeInfoBox", "ProtectionSchemeInfoBox" },
            { "SingleItemTypeReferenceBox", "SingleItemTypeReferenceBox" },
            { "SingleItemTypeReferenceBox[]", "SingleItemTypeReferenceBox[]" },
            { "SingleItemTypeReferenceBoxLarge", "SingleItemTypeReferenceBoxLarge" },
            { "SingleItemTypeReferenceBoxLarge[]", "SingleItemTypeReferenceBoxLarge[]" },
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
            { "MetaDataKeyBox[]", "MetaDataKeyBox[]" },
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
            { "Descriptor[0 .. 255]", "Descriptor[]" },
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
            { "DownMixInstructions() []", "DownMixInstructions[]" },
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
            { "DependencyInfo[numReferences]", "DependencyInfo[]" },
            { "VvcPTLRecord(0)[i]", "VvcPTLRecord[]" },
            { "EVCSliceComponentTrackConfigurationBox", "EVCSliceComponentTrackConfigurationBox" },
            { "SVCMetadataSampleConfigBox", "SVCMetadataSampleConfigBox" },
            { "SVCPriorityLayerInfoBox", "SVCPriorityLayerInfoBox" },
            { "EVCConfigurationBox", "EVCConfigurationBox" },
            { "VvcNALUConfigBox", "VvcNALUConfigBox" },
            { "VvcConfigurationBox", "VvcConfigurationBox" },
            { "HEVCTileConfigurationBox", "HEVCTileConfigurationBox" },
            { "MetaDataKeyTableBox()", "MetaDataKeyTableBox" },
            { "BitRateBox ()", "BitRateBox" },
            { "char[count]", "byte[]" },
            { "signed   int(64)[j][k]", "long[][]" },
            { "signed   int(64)[j]", "long[]" },
            { "unsigned int(8)[j][k]", "byte[][]" },
            { "unsigned int(8)[j]", "byte[]" },
            { "signed int(32)[ c ]", "int[]" },
            { "unsigned int(8)[]", "byte[]" },
            { "unsigned int(8)[i]", "byte[]" },
            { "unsigned int(6)[i]", "byte[]" },
            { "unsigned int(6)[i][j]", "byte[][]" },
            { "unsigned int(1)[i][j]", "byte[][]" },
            { "unsigned int(9)[i]", "ushort[]" },
            { "unsigned int(32)[]", "uint[]" },
            { "unsigned int(32)[i]", "uint[]" },
            { "unsigned int(32)[j]", "uint[]" },
            { "char[]", "byte[]" },
            { "loudness[]", "int[]" },
            { "ItemPropertyAssociationBox[]", "ItemPropertyAssociationBox[]" },
            { "string[method_count]", "string[]" },
            { "ItemInfoExtension", "ItemInfoExtension" },
            { "SampleGroupDescriptionEntry", "SampleGroupDescriptionEntry" },
            { "SampleEntry", "SampleEntry" },
            { "SampleConstructor", "SampleConstructor" },
            { "InlineConstructor", "InlineConstructor" },
            { "SampleConstructorFromTrackGroup", "SampleConstructorFromTrackGroup" },
            { "NALUStartInlineConstructor", "NALUStartInlineConstructor" },
            { "MPEG4BitRateBox", "MPEG4BitRateBox" },
            { "ChannelLayout", "ChannelLayout" },
            { "UniDrcConfigExtension", "UniDrcConfigExtension" },
            { "SamplingRateBox", "SamplingRateBox" },
            { "TextConfigBox", "TextConfigBox" },
            { "MetaDataKeyTableBox", "MetaDataKeyTableBox" },
            { "BitRateBox", "BitRateBox" },
            { "CompleteTrackInfoBox", "CompleteTrackInfoBox" },
            { "TierDependencyBox", "TierDependencyBox" },
            { "InitialParameterSetBox", "InitialParameterSetBox" },
            { "PriorityRangeBox", "PriorityRangeBox" },
            { "ViewPriorityBox", "ViewPriorityBox" },
            { "SVCDependencyRangeBox", "SVCDependencyRangeBox" },
            { "RectRegionBox", "RectRegionBox" },
            { "IroiInfoBox", "IroiInfoBox" },
            { "TranscodingInfoBox", "TranscodingInfoBox" },
            { "MetaDataKeyDeclarationBox", "MetaDataKeyDeclarationBox" },
            { "MetaDataDatatypeBox", "MetaDataDatatypeBox" },
            { "MetaDataLocaleBox", "MetaDataLocaleBox" },
            { "MetaDataSetupBox", "MetaDataSetupBox" },
            { "MetaDataExtensionsBox", "MetaDataExtensionsBox" },
            { "TrackLoudnessInfo[]", "TrackLoudnessInfo[]" },
            { "AlbumLoudnessInfo[]", "AlbumLoudnessInfo[]" },
            { "VvcPTLRecord(num_sublayers)", "VvcPTLRecord[]" },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "VvcPTLRecord[]" },
            // descriptors
            { "DecoderConfigDescriptor", "DecoderConfigDescriptor" },
            { "SLConfigDescriptor", "SLConfigDescriptor" },
            { "IPI_DescrPointer[0 .. 1]", "IPI_DescrPointer" },
            { "IP_IdentificationDataSet[0 .. 255]", "IP_IdentificationDataSet[]" },
            { "IPMP_DescriptorPointer[0 .. 255]", "IPMP_DescriptorPointer[]" },
            { "LanguageDescriptor[0 .. 255]", "LanguageDescriptor" },
            { "QoS_Descriptor[0 .. 1]", "QoS_Descriptor" },
            { "RegistrationDescriptor[0 .. 1]", "RegistrationDescriptor" },
            { "ExtensionDescriptor[0 .. 255]", "ExtensionDescriptor[]" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "ProfileLevelIndicationIndexDescriptor[]" },
            { "DecoderSpecificInfo[0 .. 1]", "DecoderSpecificInfo[]" },
            { "bit(8)[URLlength]", "byte[]" },
            { "bit(8)[sizeOfInstance-4]", "byte[]" },
            { "double(32)", "double" },
            { "QoS_Qualifier[]", "QoS_Qualifier[]" },
        };
        return map[type];
    }

    static partial void HelloFrom(string name);
}

public static class StringExtensions
{
    public static string ToPropertyCase(this string source)
    {
        if (source.Contains('_'))
        {
            var parts = source
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

            return string.Join("", parts.Select(ToCapital));
        }
        else
        {
            return ToCapital(source);
        }
    }

    public static string ToCapital(this string source)
    {
        return string.Format("{0}{1}", char.ToUpper(source[0]), source.Substring(1));
    }
}