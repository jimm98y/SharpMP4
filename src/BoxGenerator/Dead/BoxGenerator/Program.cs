using Newtonsoft.Json;
using Pidgin;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace ConsoleApp;

[SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
public interface PseudoCode
{
}

public class PseudoClass : PseudoCode
{
    public string FourCC { get; set; }
    public string BoxName { get; set; }
    public string ClassType { get; set; }
    public string Comment { get; set; }
    public string EndComment { get; set; }
    public IList<PseudoCode> Fields { get; set; }
    public string Alignment { get; set; }
    public PseudoExtendedClass Extended { get; set; }
    public string Syntax { get; set; }
    public long CurrentOffset { get; set; }

    public PseudoClass()
    {

    }

    public PseudoClass(
        Maybe<string> comment,
        Maybe<string> alignment,
        string boxName, 
        Maybe<string> classType,
        Maybe<PseudoExtendedClass> extended,
        IEnumerable<PseudoCode> fields,
        Maybe<string> endComment,
        long currentOffset)
    {
        Comment = comment.GetValueOrDefault();
        BoxName = boxName;
        ClassType = classType.GetValueOrDefault();
        Fields = fields.ToList();
        Alignment = alignment.GetValueOrDefault();
        Extended = extended.GetValueOrDefault();
        EndComment = endComment.GetValueOrDefault();
        CurrentOffset = currentOffset;
    }

}

public class PseudoExtendedClass : PseudoCode
{
    public string OldType { get; set; }
    public string BoxType { get; set; }
    public IList<(string Name, string Value)> Parameters { get; set; }
    public string BoxName { get; set; }
    public string DescriptorTag { get; set; }

    public PseudoExtendedClass()
    {

    }

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
    {

    }

    public PseudoField(string type, Maybe<string> name, Maybe<IEnumerable<char>> value, Maybe<string> comment)
    {
        Type = type;
        Name = name.GetValueOrDefault();
        Value = value.HasValue ? string.IsNullOrEmpty(string.Concat(value.GetValueOrDefault())) ? null : string.Concat(value.GetValueOrDefault()) : null;
        Comment = comment.GetValueOrDefault();
    }

    public string Type { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public string Comment { get; set; }
}


public class PseudoComment : PseudoCode
{
    public PseudoComment()
    {

    }

    public PseudoComment(string comment)
    {
        Comment = comment;
    }

    public string Comment { get; }
}

public class PseudoBlock : PseudoCode
{
    public PseudoBlock()
    {

    }

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
    public PseudoRepeatingBlock()
    {

    }

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
            Try(String("unsigned int(8 * OutputChannelCount)")),
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
            Try(String("const unsigned int(8)[6]")),
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
            Try(String("double(32)")),
            Try(String("GetAudioObjectType()")),
            Try(String("bslbf(header_size * 8)")),
            Try(String("bslbf(trailer_size * 8)")),
            Try(String("bslbf(aux_size * 8)")),
            Try(String("bslbf(11)")),
            Try(String("bslbf(5)")),
            Try(String("bslbf(4)")),
            Try(String("bslbf(1)")),
            Try(String("uimsbf(32)")),
            Try(String("uimsbf(24)")),
            Try(String("uimsbf(18)")),
            Try(String("uimsbf(16)")),
            Try(String("uimsbf(14)")),
            Try(String("uimsbf(12)")),
            Try(String("uimsbf(10)")),
            Try(String("uimsbf(8)")),
            Try(String("uimsbf(7)")),
            Try(String("uimsbf(6)")),
            Try(String("uimsbf(5)")),
            Try(String("uimsbf(4)")),
            Try(String("uimsbf(3)")),
            Try(String("uimsbf(2)")),
            Try(String("uimsbf(1)")),
            Try(String("sbrPresentFlag = -1")), // WORKAROUND
            Try(String("psPresentFlag = -1")), // WORKAROUND
            Try(String("extensionAudioObjectType = 0")), // WORKAROUND
            Try(String("extensionAudioObjectType = 5")), // WORKAROUND
            Try(String("sbrPresentFlag = 1")), // WORKAROUND
            Try(String("psPresentFlag = 1")), // WORKAROUND
            Try(String("case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()")),
            Try(String("case 8:\r\n      CelpSpecificConfig()")),
            Try(String("case 9:\r\n      HvxcSpecificConfig()")),
            Try(String("case 12:\r\n      TTSSpecificConfig()")),
            Try(String("case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()")),
            Try(String("case 24:\r\n      ErrorResilientCelpSpecificConfig()")),
            Try(String("case 25:\r\n      ErrorResilientHvxcSpecificConfig()")),
            Try(String("case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()")),
            Try(String("case 28:\r\n      SSCSpecificConfig()")),
            Try(String("case 30:\r\n      uimsbf(1)")),
            Try(String("SpatialSpecificConfig()")),
            Try(String("case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()")),
            Try(String("case 35:\r\n      DSTSpecificConfig()")),
            Try(String("case 36:\r\n      bslbf(5)")),
            Try(String("ALSSpecificConfig()")),
            Try(String("case 37:\r\n    case 38:\r\n      SLSSpecificConfig()")),
            Try(String("case 39:\r\n      ELDSpecificConfig(channelConfiguration)")),
            Try(String("case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()")),
            Try(String("default:\r\n      /* reserved */\r\n      break")),
            Try(String("case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)")),
            Try(String("ErrorProtectionSpecificConfig()")),
            Try(String("break")),
            Try(String("audioObjectType = 32 + audioObjectTypeExt")),
            Try(String("return audioObjectType")),
            Try(String("program_config_element()")),
            Try(String("byte_alignment()")),
            Try(String("CelpHeader(samplingFrequencyIndex)")),
            Try(String("CelpBWSenhHeader()")),
            Try(String("HVXCconfig()")),
            Try(String("TTS_Sequence()")),
            Try(String("ER_SC_CelpHeader(samplingFrequencyIndex)")),
            Try(String("ErHVXCconfig()")),
            Try(String("PARAconfig()")),
            Try(String("HILNenexConfig()")),
            Try(String("HILNconfig()")),
            Try(String("ld_sbr_header(channelConfiguration)")),
            Try(String("len = eldExtLen")),
            Try(String("len += eldExtLenAddAdd")),
            Try(String("len += eldExtLenAdd")),
            Try(String("default:\r\n        int cntt")),
            Try(String("case 1:\r\n    case 2:\r\n      numSbrHeader = 1")),
            Try(String("case 3:\r\n      numSbrHeader = 2")),
            Try(String("case 4:\r\n    case 5:\r\n    case 6:\r\n      numSbrHeader = 3")),
            Try(String("case 7:\r\n      numSbrHeader = 4")),
            Try(String("default:\r\n      numSbrHeader = 0")),
            Try(String("sbr_header()")),
            Try(String("AV1SampleEntry")),
            Try(String("AV1CodecConfigurationBox")),
            Try(String("AV1CodecConfigurationRecord")),
            Try(String("FieldLength = (large_size + 1) * 16")) // WORKAROUND
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
                Try(String("while")),
                Try(String("switch"))
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
            Try(String("(unsigned int(32) format)")),
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
            Try(String("(samplingFrequencyIndex, channelConfiguration, audioObjectType)")),
            Try(String("(samplingFrequencyIndex,\r\n  channelConfiguration,\r\n  audioObjectType)")),
            Try(String("(channelConfiguration)")),
            Try(String("(num_sublayers)")),
            Try(String("(code)")),
            Try(String("(property_type, version, flags)")),
            Try(String("(unsigned int(32) extension_type)")),
            Try(String("('vvcb', version, flags)")),
            Try(String("(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)")),
            Try(String("(unsigned int(32) grouping_type)")),
            Try(String("(unsigned int(32) boxtype, unsigned int(8) v, bit(24) f)")),            
            Try(String("(unsigned int(8) OutputChannelCount)")),         
            Try(String("(samplingFrequencyIndex)"))            
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
        Map((comment, alignment, boxName, classType, extended, fields, endComment, endOffset) => new PseudoClass(comment, alignment, boxName, classType, extended, fields, endComment, endOffset),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
            SkipWhitespaces.Then(Try(String("abstract")).Optional()).Then(SkipWhitespaces).Then(Try(String("aligned(8)")).Optional()).Before(SkipWhitespaces).Before(Try(String("expandable(228-1)")).Optional()),
            SkipWhitespaces.Then(Try(String("abstract")).Optional()).Then(SkipWhitespaces).Then(String("class")).Then(SkipWhitespaces).Then(Identifier),
            SkipWhitespaces.Then(Try(ClassType).Optional()),
            SkipWhitespaces.Then(Try(ExtendedClass).Optional()),
            Char('{').Then(SkipWhitespaces).Then(CodeBlocks).Before(Char('}')),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
            CurrentOffset
        );

    public static Parser<char, IEnumerable<PseudoClass>> Boxes => SkipWhitespaces.Then(Box.SeparatedAndOptionallyTerminated(SkipWhitespaces));


    static void Main(string[] args)
    {
        //var jds = File.ReadAllText("14496-3-added.js");
        //var test = Boxes.ParseOrThrow(jds);

        string[] jsonFiles = {
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
            "14496-1-added.json",
            "14496-3-added.json",
            "Opus.json",
            "AV1.json",
            "AVIF.json",
        };
        int success = 0;
        int duplicated = 0;
        int fail = 0;

        Dictionary<string, PseudoClass> ret = new Dictionary<string, PseudoClass>();
        List<PseudoClass> duplicates = new List<PseudoClass>();
        List<string> containers = new List<string>();

        foreach (var file in jsonFiles)
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
                        long offset = 0;
                        result = DeduplicateBoxes(result);
                        foreach (var item in result)
                        {
                            item.Syntax = GetSampleCode(sample, offset, item.CurrentOffset);
                            offset = item.CurrentOffset;
                            if (item.Extended != null)
                            {
                                item.FourCC = item.Extended.BoxType;
                            }

                            // allowed duplicates
                            if(item.BoxName == "hintPacketsSent"||
                                item.BoxName == "hintBytesSent")
                            {
                                item.BoxName = item.BoxName + item.FourCC.ToCapital();
                                if (ret.TryAdd(item.BoxName, item))
                                    success++;
                                else
                                {
                                    duplicated++;
                                    duplicates.Add(item);
                                    Console.WriteLine($"Duplicated: {item.BoxName}");
                                }
                                continue;
                            }

                            if (ret.TryAdd(item.BoxName, item))
                            {
                                success++;                                
                            }
                            else
                            {
                                duplicated++;
                                duplicates.Add(item);
                                Console.WriteLine($"Duplicated: {item.BoxName}");

                                if (item.Extended != null && item.Extended.BoxType != ret[item.BoxName].Extended.BoxType)
                                {
                                    if (!ret.TryAdd($"{item.BoxName}_{item.Extended.BoxType}", item))
                                    {
                                        Console.WriteLine($"Duplicated and failed to add: {item.BoxName}");
                                    }
                                    // override the name
                                    item.BoxName = $"{item.BoxName}_{item.Extended.BoxType}";
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
using System.Threading.Tasks;

namespace BoxGenerator2
{
";
        // build box factory
        string factory =
@"   public class BoxFactory
    {
        public static Box CreateBox(string fourCC)
        {
            switch(fourCC)
            {
               ";

        SortedDictionary<string, List<PseudoClass>> fourccBoxes = new SortedDictionary<string, List<PseudoClass>>();
        foreach (var item in ret)
        {
            if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.BoxType))
            {
                if (item.Value.Extended.BoxType == "fdel") // item extension, not a box
                    continue;

                if (!fourccBoxes.ContainsKey(item.Value.Extended.BoxType))
                    fourccBoxes.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                else
                    fourccBoxes[item.Value.Extended.BoxType].Add(item.Value);
            }
        }

        foreach (var item in fourccBoxes)
        {
            if (item.Value.Count == 1)
            {
                string comment = "";
                if (item.Value.Single().BoxName.Contains('_'))
                    comment = " // TODO: fix duplicate";
                factory += $"               case \"{item.Key}\": return new {item.Value.Single().BoxName}();{comment}\r\n";
            }
            else
            {
                factory += $"               case \"{item.Key}\": throw new NotSupportedException(\"\'{item.Key}\' is ambiguous in between {string.Join(" and ", item.Value.Select(x => x.BoxName))}\");\r\n";
            }
        }

        factory +=
@"          }

            throw new NotImplementedException(fourCC);
        }
    }

";
        resultCode += factory;

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

    private static IEnumerable<PseudoClass> DeduplicateBoxes(IEnumerable<PseudoClass> result)
    {
        List<PseudoClass> boxes = new List<PseudoClass>();
        foreach (var box in result)
        {
            if(box.Extended != null && box.Extended.BoxType != null && box.Extended.BoxType.Contains("\' or \'"))
            {
                string[] parts = box.Extended.BoxType.Split("\' or \'");
                for(int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];
                    string key = part.TrimStart('\'').TrimEnd('\'');
                    var copy = JsonConvert.DeserializeObject<PseudoClass>(JsonConvert.SerializeObject(box, new JsonSerializerSettings() {  TypeNameHandling = TypeNameHandling.All }), new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }); // deep copy
                    copy.Extended.BoxType = key;
                    boxes.Add(copy);
                }
            }
            else
            {
                boxes.Add(box);
            }
        }
        return boxes;
    }

    private static string GetSampleCode(string sample, long startOffset, long endOffset)
    {
        return sample.Substring((int)startOffset, (int)(endOffset - startOffset));
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

        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrWhiteSpace(b.Extended.BoxType))
        {
            cls += $"\tpublic const string FourCC = \"{b.Extended.BoxType}\";";
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

        string ctorParams = "";
        if (!string.IsNullOrEmpty(b.ClassType) || (b.Extended != null && b.Extended.Parameters != null))
        {
            ctorParams = GetCtorParams(b.ClassType, b.Extended.Parameters);
        }
        string baseCtorParams = "";
        if (b.Extended != null)
        {
            string base4cc = "";
            if (!string.IsNullOrWhiteSpace(b.Extended.BoxType))
                base4cc = $"\"{b.Extended.BoxType}\"";
            string base4ccparams = "";
            if (b.Extended.Parameters != null)
            {
                base4ccparams = string.Join(", ", b.Extended.Parameters.Select(x => string.IsNullOrEmpty(x.Value) ? x.Name : x.Value));
            }
            string base4ccseparator = "";
            if (!string.IsNullOrEmpty(base4cc) && !string.IsNullOrEmpty(base4ccparams))
                base4ccseparator = ", ";
            baseCtorParams = $": base({base4cc}{base4ccseparator}{base4ccparams})";
        }

        cls += $"\r\n\r\n\tpublic {b.BoxName}({ctorParams}){baseCtorParams}\r\n\t{{ }}\r\n";

        cls += "\r\n\tpublic async " + (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "override " : "virtual ") + "Task<ulong> ReadAsync(IsoStream stream)\r\n\t{\r\n\t\tulong boxSize = 0;" +
            (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "\r\n\t\tboxSize += await base.ReadAsync(stream);" : "");

        cls = FixMissingCode(b, cls);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, MethodType.Read);
        }

        if (containers.Contains(b.FourCC) || containers.Contains(b.BoxName))
        {
            cls += "\r\n" + "boxSize += stream.ReadBoxChildren(boxSize, this);";
        }

        //if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrEmpty(b.FourCC))
        //{
        //    cls += "\r\n" + "boxSize += stream.ReadSkip(size, boxSize);";
        //}

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";


        cls += "\r\n\tpublic async " + (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "override " : "virtual ") + "Task<ulong> WriteAsync(IsoStream stream)\r\n\t{\r\n\t\tulong boxSize = 0;" +
            (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "\r\n\t\tboxSize += await base.WriteAsync(stream);" : "");

        cls = FixMissingCode(b, cls);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, MethodType.Write);
        }

        if (containers.Contains(b.FourCC) || containers.Contains(b.BoxName))
        {
            cls += "\r\n" + "boxSize += stream.WriteBoxChildren(this);";
        }

        //if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrEmpty(b.FourCC))
        //{
        //    cls += "\r\n" + "boxSize += stream.Flush();";
        //}

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";

        cls += "\r\n\tpublic " + (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "override " : "virtual ") + "ulong CalculateSize()\r\n\t{\r\n\t\tulong boxSize = 0;" +
            (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) ? "\r\n\t\tboxSize += base.CalculateSize();" : "");

        cls = FixMissingCode(b, cls);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(field, 2, MethodType.Size);
        }

        //if (containers.Contains(b.FourCC) || containers.Contains(b.BoxName))
        //{
        //    cls += "\r\n" + "boxSize += IsoStream.CalculateSize(children);";
        //}

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";

        // end of class
        cls += "}\r\n";

        return cls;
    }

    private static string GetCtorParams(string classType, IList<(string Name, string Value)> parameters)
    {
        if (!string.IsNullOrEmpty(classType) && classType != "()")
        {
            Dictionary<string, string> map = new Dictionary<string, string>() {
            { "(unsigned int(32) format)", "string format" },
            { "(bit(24) flags)", "uint flags = 0" },
            { "(fmt)", "string fmt = \"\"" },
            { "(codingname)", "string codingname = \"\"" },
            { "(handler_type)", "string handler_type = \"\"" },
            { "(referenceType)", "string referenceType" },
            { "(unsigned int(32) reference_type)", "string reference_type" },
            { "(grouping_type, version, flags)", "string grouping_type, byte version, uint flags" },
            { "('snut')", "string boxtype = \"snut\"" },
            { "('resv')", "string boxtype = \"resv\"" },
            { "('msrc')", "string boxtype = \"msrc\"" },
            { "('cstg')", "string boxtype = \"cstg\"" },
            { "('alte')", "string boxtype = \"alte\"" },
            { "(name)", "string name" },
            { "(property_type)", "string property_type" },
            { "(channelConfiguration)", "int channelConfiguration" },
            { "(num_sublayers)", "ulong num_sublayers" },
            { "(code)", "string code" },
            { "(property_type, version, flags)", "string property_type, byte version, uint flags" },
            { "(samplingFrequencyIndex, channelConfiguration, audioObjectType)", "int samplingFrequencyIndex, int channelConfiguration, int audioObjectType" },
            { "(samplingFrequencyIndex,\r\n  channelConfiguration,\r\n  audioObjectType)", "int samplingFrequencyIndex, int channelConfiguration, int audioObjectType" },
            { "(unsigned int(32) extension_type)", "string extension_type" },
            { "('vvcb', version, flags)", "byte version = 0, uint flags = 0" },
            { "(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)", "string boxtype, byte[] extended_type" },
            { "(unsigned int(32) grouping_type)", "string grouping_type" },
            { "(unsigned int(32) boxtype, unsigned int(8) v, bit(24) f)", "string boxtype, byte v = 0, uint f = 0" },
            { "(unsigned int(8) OutputChannelCount)", "byte OutputChannelCount" },
            { "(samplingFrequencyIndex)", "int samplingFrequencyIndex" },
            };
            return map[classType];
        }
        else if(parameters != null)
        {
            Debug.WriteLine($"---Params: {string.Join(", ", parameters.Select(x => x.Name))}");
            string joinedParams = string.Join(", ", parameters.Select(x => x.Name));
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "loudnessType", "string loudnessType" },
                { "local_key_id", "string local_key_id" },
                { "version, flags", "byte version = 0, uint flags = 0" },
                { "version, 0", "byte version = 0" },
                { "0, 0", "" },
                { "0, flags", "uint flags = 0" },
                { "version, tr_flags", "byte version = 0, uint tr_flags = 0" },
                { "0, tf_flags", "uint tf_flags = 0" },
                { "version, 1", "byte version = 0" },
                { "size", "ulong size = 0" },
                { "protocol", "string protocol" },
                { "type", "string type" },
            };
            return map[joinedParams];
        }
        else
        {
            return "";
        }
    }

    private static string FixMissingCode(PseudoClass? box, string cls)
    {
        if (box.BoxName == "DegradationPriorityBox" || box.BoxName == "SampleDependencyTypeBox" || box.BoxName == "SampleDependencyBox")
        {
            cls += "\r\n\t\tint sample_count = 0; // TODO: taken from the stsz sample_count\r\n";
        }
        
        if (box.BoxName == "DownMixInstructions")
        {
            cls += "\r\n\t\tint baseChannelCount = 0; // TODO: get somewhere";
        }
        
        if(box.BoxName == "CompactSampleToGroupBox")
        {
            cls += "\r\n";
            cls += "\t\tbool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);\r\n";
            cls += "\t\tuint count_size_code = (flags >> 2) & 0x3;\r\n";
            cls += "\t\tuint pattern_size_code = (flags >> 4) & 0x3;\r\n";
            cls += "\t\tuint index_size_code = flags & 0x3;\r\n";
        }
        
        if(box.BoxName == "VvcPTLRecord")
        {
            cls += "\r\n";
            cls += "\t\tint num_sublayers = 0; // TODO pass arg\r\n";
        }
        
        if (box.BoxName == "VvcDecoderConfigurationRecord")
        {
            cls += "\r\n";
            cls += "\t\tconst int OPI_NUT = 12;\r\n";
            cls += "\t\tconst int DCI_NUT = 13;\r\n";
        }
        
        if(box.BoxName == "ChannelMappingTable")
        {
            cls += "\r\n\t\tulong OutputChannelCount = 0; // TODO: pass through ctor\r\n";
        }
        
        if(box.BoxName == "ld_sbr_header")
        {
            cls += "\r\n\t\tint numSbrHeader = 0;\r\n";
        }
        
        if(box.BoxName == "ELDSpecificConfig")
        {
            cls += "\r\n\t\tint len = 0;\r\n";
        }
        
        if(box.BoxName == "ld_sbr_header" || box.BoxName == "SSCSpecificConfig")
        {
            cls += "\r\n\t\tint channelConfiguration = 0; // TODO: pass through ctor\r\n";
        }

        if(box.BoxName == "ELDSpecificConfig")
        {
            cls += "\r\n\t\tconst byte ELDEXT_TERM = 0;\r\n";
        }

        if(box.BoxName == "CelpHeader" || box.BoxName == "ER_SC_CelpHeader")
        {
            cls += "\r\n\t\tconst bool RPE = true;\r\n";
            cls += "\r\n\t\tconst bool MPE = false;\r\n";
        }

        if(box.BoxName == "GASpecificConfig")
        {
            cls += "\r\n\t\tGetAudioObjectType audioObjectType = null; // TODO: pass through ctor\r\n";
        }
        
        if(box.BoxName == "GASpecificConfig" || box.BoxName == "SLSSpecificConfig")
        {
            cls += "\r\n\t\tbyte channelConfiguration = 0; // TODO: pass through ctor\r\n";
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
            else if (tt == "audioObjectType = 32 + audioObjectTypeExt")
                return "audioObjectType = (byte)(32 + audioObjectTypeExt);";
            else if (tt == "sbrPresentFlag = -1")
                return "sbrPresentFlag = false;";
            else if (tt == "psPresentFlag = -1")
                return "psPresentFlag = false;";
            else if (tt == "sbrPresentFlag = 1")
                return "sbrPresentFlag = true;";
            else if (tt == "psPresentFlag = 1")
                return "psPresentFlag = true;";
            else if (tt.Contains("extensionAudioObjectType"))
                return tt.Replace("extensionAudioObjectType", "extensionAudioObjectType.AudioObjectType") + ";";
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
            value != "[ sample_count ]" && value != "[method_count]" && value != "[URLlength]" && value != "[sizeOfInstance-4]" && value != "[3]")
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

        string boxSize = "boxSize += ";
        if (m.StartsWith("case")) // workaround for missing case support
            boxSize = "";

        if (methodType == MethodType.Read)
            return $"{spacing}{optional}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
        else if(methodType == MethodType.Write)
            return $"{spacing}{optional}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
        else
            return $"{spacing}{optional}{boxSize}{m.Replace("value", name)}; // {name}";
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
                condition.Contains("rate_escape") ||
                condition.Contains("length_escape") ||
                condition.Contains("crclen_escape") ||
                condition.Contains("class_reordered_output") ||
                condition.Contains("header_protection") ||
                condition.Contains("sbrPresentFlag") ||
                condition.Contains("mono_mixdown_present") ||
                condition.Contains("stereo_mixdown_present") ||
                condition.Contains("matrix_mixdown_idx_present") ||
                condition.Contains("level_is_static_flag"))
            {
                condition = condition
                    .Replace("==0", "== false").Replace("==1", "== true")
                    .Replace("== 0", "== false").Replace("== 1", "== true")
                    .Replace("!= 1", "!= true").Replace("!= 0", "!= false")
                    .Replace("!=1", "!=true").Replace("!=0", "!=false");
            }

            if(condition.Contains("!channelConfiguration"))
            {
                condition = condition.Replace("!channelConfiguration", "channelConfiguration == 0");
            }    

            // other fixes - taken from ISO_IEC_14496-12_2015 comments
            if (condition.Contains("channelStructured"))
                condition = condition.Replace("channelStructured", "1");
            if (condition.Contains("objectStructured"))
                condition = condition.Replace("objectStructured", "2");
        }

        if (!string.IsNullOrEmpty(condition))
        {
            if (condition.Contains("audioObjectType") && !condition.Contains("audioObjectType == 31"))
                condition = condition.Replace("audioObjectType", "audioObjectType.AudioObjectType");
            
            if (condition.Contains("extensionAudioObjectType"))
                condition = condition.Replace("extensionAudioObjectType", "extensionAudioObjectType.AudioObjectType");

            if (condition.Contains("bits_to_decode()"))
                condition = condition.Replace("bits_to_decode()", "IsoStream.BitsToDecode()");
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
                string replaceValue = "IsoStream.FromFourCC(" + inputValue + ")";
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
            { "unsigned int(64)[ entry_count ]", "stream.ReadUInt64Array(entry_count, " },
            { "unsigned int(64)", "stream.ReadUInt64(" },
            { "unsigned int(48)", "stream.ReadUInt48(" },
            { "unsigned int(32)[ entry_count ]", "stream.ReadUInt32Array(entry_count, " },
            { "template int(32)[9]", "stream.ReadUInt32Array(9, " },
            { "unsigned int(32)[3]", "stream.ReadUInt32Array(3, " },
            { "unsigned int(32)", "stream.ReadUInt32(" },
            { "unsigned_int(32)", "stream.ReadUInt32(" },
            { "unsigned int(24)", "stream.ReadUInt24(" },
            { "unsigned int(29)", "stream.ReadBits(29, " },
            { "unsigned int(26)", "stream.ReadBits(26, " },
            { "unsigned int(16)[i]", "stream.ReadUInt16(" },
            { "unsigned int(16)[j]", "stream.ReadUInt16(" },
            { "unsigned int(16)[i][j]", "stream.ReadUInt16(" },
            { "unsigned int(16)", "stream.ReadUInt16(" },
            { "unsigned_int(16)", "stream.ReadUInt16(" },
            { "unsigned int(15)", "stream.ReadBits(15, " },
            { "unsigned int(12)", "stream.ReadBits(12, " },
            { "signed int(12)", "stream.ReadBits(12, " },
            { "unsigned int(10)[i][j]", "stream.ReadBits(10, " },
            { "unsigned int(10)[i]", "stream.ReadBits(10, " },
            { "unsigned int(10)", "stream.ReadBits(10, " },
            { "unsigned int(8)[ sample_count ]", "stream.ReadBytes(sample_count, " },
            { "unsigned int(8)[length]", "stream.ReadBytes(length, " },
            { "unsigned int(8)[32]", "stream.ReadBytes(32, " },
            { "unsigned int(8)[16]", "stream.ReadBytes(16, " },
            { "unsigned int(9)", "stream.ReadBits(9, " },
            { "unsigned int(8)", "stream.ReadUInt8(" },
            { "unsigned int(7)", "stream.ReadBits(7, " },
            { "unsigned int(6)", "stream.ReadBits(6, " },
            { "unsigned int(5)[3]", "stream.ReadBitsArray(5, 3, " },
            { "unsigned int(5)", "stream.ReadBits(5, " },
            { "unsigned int(4)", "stream.ReadBits(4, " },
            { "unsigned int(3)", "stream.ReadBits(3, " },
            { "unsigned int(2)[i][j]", "stream.ReadBits(2, " },
            { "unsigned int(2)", "stream.ReadBits(2, " },
            { "unsigned int(1)[i]", "stream.ReadBit(" },
            { "unsigned int(1)", "stream.ReadBit(" },
            { "unsigned int (32)", "stream.ReadUInt32(" },
            { "unsigned int(f(pattern_size_code))[i]", "stream.ReadBits(pattern_size_code, " },
            { "unsigned int(f(index_size_code))[i]", "stream.ReadBits(index_size_code, " },
            { "unsigned int(f(index_size_code))[j][k]", "stream.ReadBits(index_size_code, " },
            { "unsigned int(f(count_size_code))[i]", "stream.ReadBits(count_size_code, " },
            { "unsigned int(base_offset_size*8)", "stream.ReadBytes(base_offset_size, " },
            { "unsigned int(offset_size*8)", "stream.ReadBytes(offset_size, " },
            { "unsigned int(length_size*8)", "stream.ReadBytes(length_size, " },
            { "unsigned int(index_size*8)", "stream.ReadBytes(index_size, " },
            { "unsigned int(field_size)", "stream.ReadBytes(field_size, " },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "stream.ReadBytes((ulong)(length_size_of_traf_num+1), " },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "stream.ReadBytes((ulong)(length_size_of_trun_num+1), " },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "stream.ReadBytes((ulong)(length_size_of_sample_num+1), " },
            { "unsigned int(8*size-64)", "stream.ReadBytes(size-64, " },
            { "unsigned int(subgroupIdLen)[i]", "stream.ReadUInt32(" },
            { "const unsigned int(8)[6]", "stream.ReadUInt8Array(6, " },
            { "const unsigned int(32)[2]", "stream.ReadUInt32Array(2, " },
            { "const unsigned int(32)[3]", "stream.ReadUInt32Array(3, " },
            { "const unsigned int(32)", "stream.ReadUInt32(" },
            { "const unsigned int(16)[3]", "stream.ReadUInt16Array(3, " },
            { "const unsigned int(16)", "stream.ReadUInt16(" },
            { "const unsigned int(26)", "stream.ReadBits(26, " },
            { "template int(32)", "stream.ReadInt32(" },
            { "template int(16)", "stream.ReadInt16(" },
            { "template unsigned int(30)", "stream.ReadBits(30, " },
            { "template unsigned int(32)", "stream.ReadUInt32(" },
            { "template unsigned int(16)[3]", "stream.ReadUInt16Array(3, " },
            { "template unsigned int(16)", "stream.ReadUInt16(" },
            { "template unsigned int(8)[]", "stream.ReadUInt8Array(" },
            { "template unsigned int(8)", "stream.ReadUInt8(" },
            { "int(64)", "stream.ReadInt64(" },
            { "int(32)", "stream.ReadInt32(" },
            { "int(16)", "stream.ReadInt16(" },
            { "int(8)", "stream.ReadInt8(" },
            { "int(4)", "stream.ReadBits(4, " },
            { "int", "stream.ReadInt32(" },
            { "const bit(16)", "stream.ReadUInt16(" },
            { "const bit(1)", "stream.ReadBit(" },
            { "bit(1)", "stream.ReadBit(" },
            { "bit(2)", "stream.ReadBits(2, " },
            { "bit(3)", "stream.ReadBits(3, " },
            { "bit(4)", "stream.ReadBits(4, " },
            { "bit(5)", "stream.ReadBits(5, " },
            { "bit(6)", "stream.ReadBits(6, " },
            { "bit(7)", "stream.ReadBits(7, " },
            { "bit(8)[]", "stream.ReadUInt8Array(" },
            { "bit(8)", "stream.ReadUInt8(" },
            { "bit(16)[i]", "stream.ReadUInt16(" },
            { "bit(16)", "stream.ReadUInt16(" },
            { "bit(24)", "stream.ReadBits(24, " },
            { "bit(31)", "stream.ReadBits(31, " },
            { "bit(8 ceil(size / 8) \u2013 size)", "stream.ReadBytes((ulong)(Math.Ceiling(size / 8d) - size), " },
            { "bit(8* ps_nalu_length)", "stream.ReadBytes(ps_nalu_length, " },
            { "bit(8*nalUnitLength)", "stream.ReadBytes(nalUnitLength, " },
            { "bit(8*sequenceParameterSetLength)", "stream.ReadBytes(sequenceParameterSetLength, " },
            { "bit(8*pictureParameterSetLength)", "stream.ReadBytes(pictureParameterSetLength, " },
            { "bit(8*sequenceParameterSetExtLength)", "stream.ReadBytes(sequenceParameterSetExtLength, " },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "stream.ReadBytes((ulong)(num_bytes_constraint_info - 2), " },
            { "bit(8*nal_unit_length)", "stream.ReadBytes(nal_unit_length, " },
            { "bit(timeStampLength)", "stream.ReadBytes(timeStampLength, " },
            { "utf8string", "stream.ReadString(" },
            { "utfstring", "stream.ReadString(" },
            { "utf8list", "stream.ReadString(" },
            { "boxstring", "stream.ReadString(" },
            { "string", "stream.ReadString(" },
            { "bit(32)[6]", "stream.ReadUInt32Array(6, " },
            { "bit(32)", "stream.ReadUInt32(" },
            { "uint(32)", "stream.ReadUInt32(" },
            { "uint(16)", "stream.ReadUInt16(" },
            { "uint(64)", "stream.ReadUInt64(" },
            { "uint(8)[32]", "stream.ReadBytes(32, " },
            { "uint(8)", "stream.ReadUInt8(" },
            { "uint(7)", "stream.ReadBits(7, " },
            { "uint(1)", "stream.ReadBits(1, " },
            { "signed   int(64)", "stream.ReadInt64(" },
            { "signed int(32)", "stream.ReadInt32(" },
            { "signed int (16)", "stream.ReadInt16(" },
            { "signed int(16)[grid_pos_view_id[i]]", "stream.ReadInt16(" },
            { "signed int(16)", "stream.ReadInt16(" },
            { "signed int (8)", "stream.ReadInt8(" },
            { "signed int(64)", "stream.ReadInt64(" },
            { "signed   int(32)", "stream.ReadInt32(" },
            { "signed   int(8)", "stream.ReadInt8(" },
            { "Box()[]", "stream.ReadBox(" },
            { "Box[]", "stream.ReadBox(" },
            { "Box", "stream.ReadBox(" },
            { "SchemeTypeBox", "stream.ReadBox(" },
            { "SchemeInformationBox", "stream.ReadBox(" },
            { "ItemPropertyContainerBox", "stream.ReadBox(" },
            { "ItemPropertyAssociationBox", "stream.ReadBox(" },
            { "ItemPropertyAssociationBox[]", "stream.ReadBox(" },
            { "char", "stream.ReadInt8(" },
            { "loudness", "stream.ReadInt32(" },
            { "ICC_profile", "stream.ReadClass(" },
            { "OriginalFormatBox(fmt)", "stream.ReadBox(" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "stream.ReadBox(" },
            { "ItemInfoEntry[ entry_count ]", "stream.ReadBox(" },
            { "TypeCombinationBox[]", "stream.ReadBox(" },
            { "FilePartitionBox", "stream.ReadBox(" },
            { "FECReservoirBox", "stream.ReadBox(" },
            { "FileReservoirBox", "stream.ReadBox(" },
            { "PartitionEntry[ entry_count ]", "stream.ReadBox(entry_count, " },
            { "FDSessionGroupBox", "stream.ReadBox(" },
            { "GroupIdToNameBox", "stream.ReadBox(" },
            { "base64string", "stream.ReadString(" },
            { "ProtectionSchemeInfoBox", "stream.ReadBox(" },
            { "SingleItemTypeReferenceBox", "stream.ReadBox(" },
            { "SingleItemTypeReferenceBox[]", "stream.ReadBox(" },
            { "SingleItemTypeReferenceBoxLarge", "stream.ReadBox(" },
            { "SingleItemTypeReferenceBoxLarge[]", "stream.ReadBox(" },
            { "HandlerBox(handler_type)", "stream.ReadBox(" },
            { "PrimaryItemBox", "stream.ReadBox(" },
            { "DataInformationBox", "stream.ReadBox(" },
            { "ItemLocationBox", "stream.ReadBox(" },
            { "ItemProtectionBox", "stream.ReadBox(" },
            { "ItemInfoBox", "stream.ReadBox(" },
            { "IPMPControlBox", "stream.ReadBox(" },
            { "ItemReferenceBox", "stream.ReadBox(" },
            { "ItemDataBox", "stream.ReadBox(" },
            { "TrackReferenceTypeBox []", "stream.ReadBox(" },
            { "MetaDataKeyBox[]", "stream.ReadBox(" },
            { "TierInfoBox", "stream.ReadBox(" },
            { "MultiviewRelationAttributeBox", "stream.ReadBox(" },
            { "TierBitRateBox", "stream.ReadBox(" },
            { "BufferingBox", "stream.ReadBox(" },
            { "MultiviewSceneInfoBox", "stream.ReadBox(" },
            { "MVDDecoderConfigurationRecord", "stream.ReadClass(" },
            { "MVDDepthResolutionBox", "stream.ReadBox(" },
            { "MVCDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "AVCDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "HEVCDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "LHEVCDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "SVCDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "HEVCTileTierLevelConfigurationRecord()", "stream.ReadClass(" },
            { "EVCDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "VvcDecoderConfigurationRecord()", "stream.ReadClass(" },
            { "EVCSliceComponentTrackConfigurationRecord()", "stream.ReadClass(" },
            { "SampleGroupDescriptionEntry (grouping_type)", "stream.ReadBox(" },
            { "Descriptor[0 .. 255]", "stream.ReadClass(" },
            { "Descriptor", "stream.ReadClass(" },
            { "WebVTTConfigurationBox", "stream.ReadBox(" },
            { "WebVTTSourceLabelBox", "stream.ReadBox(" },
            { "OperatingPointsRecord", "stream.ReadClass(" },
            { "VvcSubpicIDEntry", "stream.ReadBox(" },
            { "VvcSubpicOrderEntry", "stream.ReadBox(" },
            { "URIInitBox", "stream.ReadBox(" },
            { "URIBox", "stream.ReadBox(" },
            { "URIbox", "stream.ReadBox(" },
            { "CleanApertureBox", "stream.ReadBox(" },
            { "PixelAspectRatioBox", "stream.ReadBox(" },
            { "DownMixInstructions() []", "stream.ReadBox(" },
            { "DRCCoefficientsBasic() []", "stream.ReadClass(" },
            { "DRCInstructionsBasic() []", "stream.ReadClass(" },
            { "DRCCoefficientsUniDRC() []", "stream.ReadClass(" },
            { "DRCInstructionsUniDRC() []", "stream.ReadClass(" },
            { "HEVCConfigurationBox", "stream.ReadBox(" },
            { "LHEVCConfigurationBox", "stream.ReadBox(" },
            { "AVCConfigurationBox", "stream.ReadBox(" },
            { "SVCConfigurationBox", "stream.ReadBox(" },
            { "ScalabilityInformationSEIBox", "stream.ReadBox(" },
            { "SVCPriorityAssignmentBox", "stream.ReadBox(" },
            { "ViewScalabilityInformationSEIBox", "stream.ReadBox(" },
            { "ViewIdentifierBox", "stream.ReadBox(" },
            { "MVCConfigurationBox", "stream.ReadBox(" },
            { "MVCViewPriorityAssignmentBox", "stream.ReadBox(" },
            { "IntrinsicCameraParametersBox", "stream.ReadBox(" },
            { "ExtrinsicCameraParametersBox", "stream.ReadBox(" },
            { "MVCDConfigurationBox", "stream.ReadBox(" },
            { "MVDScalabilityInformationSEIBox", "stream.ReadBox(" },
            { "A3DConfigurationBox", "stream.ReadBox(" },
            { "VvcOperatingPointsRecord", "stream.ReadClass(" },
            { "VVCSubpicIDRewritingInfomationStruct()", "stream.ReadClass(" },
            { "MPEG4ExtensionDescriptorsBox ()", "stream.ReadBox(" },
            { "MPEG4ExtensionDescriptorsBox()", "stream.ReadBox(" },
            { "MPEG4ExtensionDescriptorsBox", "stream.ReadBox(" },
            { "bit(8*dci_nal_unit_length)", "stream.ReadBytes(dci_nal_unit_length, " },
            { "DependencyInfo[numReferences]", "stream.ReadClass(numReferences, " },
            { "VvcPTLRecord(0)[i]", "stream.ReadClass(" },
            { "EVCSliceComponentTrackConfigurationBox", "stream.ReadBox(" },
            { "SVCMetadataSampleConfigBox", "stream.ReadBox(" },
            { "SVCPriorityLayerInfoBox", "stream.ReadBox(" },
            { "EVCConfigurationBox", "stream.ReadBox(" },
            { "VvcNALUConfigBox", "stream.ReadBox(" },
            { "VvcConfigurationBox", "stream.ReadBox(" },
            { "HEVCTileConfigurationBox", "stream.ReadBox(" },
            { "MetaDataKeyTableBox()", "stream.ReadBox(" },
            { "BitRateBox ()", "stream.ReadBox(" },
            { "char[count]", "stream.ReadUInt8Array(count, " },
            { "signed int(32)[ c ]", "stream.ReadInt32(" },
            { "unsigned int(8)[]", "stream.ReadUInt8Array(" },
            { "unsigned int(8)[i]", "stream.ReadUInt8(" },
            { "unsigned int(6)[i]", "stream.ReadBits(6, " },
            { "unsigned int(6)[i][j]", "stream.ReadBits(6, " },
            { "unsigned int(1)[i][j]", "stream.ReadBits(1, " },
            { "unsigned int(9)[i]", "stream.ReadBits(9, " },
            { "unsigned int(32)[]", "stream.ReadUInt32Array(" },
            { "unsigned int(32)[i]", "stream.ReadUInt32(" },
            { "unsigned int(32)[j]", "stream.ReadUInt32(" },
            { "unsigned int(8)[j][k]", "stream.ReadUInt8(" },
            { "signed   int(64)[j][k]", "stream.ReadInt64(" },
            { "unsigned int(8)[j]", "stream.ReadUInt8(" },
            { "signed   int(64)[j]", "stream.ReadInt64(" },
            { "char[]", "stream.ReadUInt8Array(" },
            { "string[method_count]", "stream.ReadStringArray(method_count, " },
            { "ItemInfoExtension", "stream.ReadClass(" },
            { "SampleGroupDescriptionEntry", "stream.ReadBox(" },
            { "SampleEntry", "stream.ReadBox(" },
            { "SampleConstructor", "stream.ReadBox(" },
            { "InlineConstructor", "stream.ReadBox(" },
            { "SampleConstructorFromTrackGroup", "stream.ReadBox(" },
            { "NALUStartInlineConstructor", "stream.ReadBox(" },
            { "MPEG4BitRateBox", "stream.ReadBox(" },
            { "ChannelLayout", "stream.ReadBox(" },
            { "UniDrcConfigExtension", "stream.ReadBox(" },
            { "SamplingRateBox", "stream.ReadBox(" },
            { "TextConfigBox", "stream.ReadBox(" },
            { "MetaDataKeyTableBox", "stream.ReadBox(" },
            { "BitRateBox", "stream.ReadBox(" },
            { "CompleteTrackInfoBox", "stream.ReadBox(" },
            { "TierDependencyBox", "stream.ReadBox(" },
            { "InitialParameterSetBox", "stream.ReadBox(" },
            { "PriorityRangeBox", "stream.ReadBox(" },
            { "ViewPriorityBox", "stream.ReadBox(" },
            { "SVCDependencyRangeBox", "stream.ReadBox(" },
            { "RectRegionBox", "stream.ReadBox(" },
            { "IroiInfoBox", "stream.ReadBox(" },
            { "TranscodingInfoBox", "stream.ReadBox(" },
            { "MetaDataKeyDeclarationBox", "stream.ReadBox(" },
            { "MetaDataDatatypeBox", "stream.ReadBox(" },
            { "MetaDataLocaleBox", "stream.ReadBox(" },
            { "MetaDataSetupBox", "stream.ReadBox(" },
            { "MetaDataExtensionsBox", "stream.ReadBox(" },
            { "TrackLoudnessInfo[]", "stream.ReadBox(" },
            { "AlbumLoudnessInfo[]", "stream.ReadBox(" },
            { "VvcPTLRecord(num_sublayers)", "stream.ReadClass(num_sublayers," },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "stream.ReadClass(" },
            { "OpusSpecificBox", "stream.ReadBox(" },
            { "unsigned int(8 * OutputChannelCount)", "stream.ReadBytes(OutputChannelCount, " },
            { "ChannelMappingTable", "stream.ReadClass(" },
            // descriptors
            { "DecoderConfigDescriptor", "stream.ReadClass(" },
            { "SLConfigDescriptor", "stream.ReadClass(" },
            { "IPI_DescrPointer[0 .. 1]", "stream.ReadClass(" },
            { "IP_IdentificationDataSet[0 .. 255]", "stream.ReadClass(" },
            { "IPMP_DescriptorPointer[0 .. 255]", "stream.ReadClass(" },
            { "LanguageDescriptor[0 .. 255]", "stream.ReadClass(" },
            { "QoS_Descriptor[0 .. 1]", "stream.ReadClass(" },
            { "RegistrationDescriptor[0 .. 1]", "stream.ReadClass(" },
            { "ExtensionDescriptor[0 .. 255]", "stream.ReadClass(" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "stream.ReadClass(" },
            { "DecoderSpecificInfo[0 .. 1]", "stream.ReadClass(" },
            { "bit(8)[URLlength]", "stream.ReadBytes(URLlength, " },
            { "bit(8)[sizeOfInstance-4]", "stream.ReadBytes((ulong)(sizeOfInstance - 4), " },
            { "double(32)", "stream.ReadDouble32(" },
            { "QoS_Qualifier[]", "stream.ReadClass(" },
            { "GetAudioObjectType()", "stream.ReadClass(" },
            { "bslbf(header_size * 8)[]", "stream.ReadBslbf(header_size * 8, " },
            { "bslbf(trailer_size * 8)[]", "stream.ReadBslbf(trailer_size * 8, " },
            { "bslbf(aux_size * 8)[]", "stream.ReadBslbf(aux_size * 8, " },
            { "bslbf(11)", "stream.ReadBslbf(11, " },
            { "bslbf(5)", "stream.ReadBslbf(5, " },
            { "bslbf(4)", "stream.ReadBslbf(4, " },
            { "bslbf(1)", "stream.ReadBslbf(" },
            { "uimsbf(32)", "stream.ReadUimsbf(32, " },
            { "uimsbf(24)", "stream.ReadUimsbf(24, " },
            { "uimsbf(18)", "stream.ReadUimsbf(18, " },
            { "uimsbf(16)", "stream.ReadUimsbf(16, " },
            { "uimsbf(14)", "stream.ReadUimsbf(14, " },
            { "uimsbf(12)", "stream.ReadUimsbf(12, " },
            { "uimsbf(10)", "stream.ReadUimsbf(10, " },
            { "uimsbf(8)", "stream.ReadUimsbf(8, " },
            { "uimsbf(7)", "stream.ReadUimsbf(7, " },
            { "uimsbf(6)", "stream.ReadUimsbf(6, " },
            { "uimsbf(5)", "stream.ReadUimsbf(5, " },
            { "uimsbf(4)", "stream.ReadUimsbf(4, " },
            { "uimsbf(3)", "stream.ReadUimsbf(3, " },
            { "uimsbf(2)", "stream.ReadUimsbf(2, " },
            { "uimsbf(1)", "stream.ReadUimsbf(" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      boxSize += stream.ReadClass(" },
            { "case 8:\r\n      CelpSpecificConfig()", "case 8:\r\n      boxSize += stream.ReadClass(" },
            { "case 9:\r\n      HvxcSpecificConfig()", "case 9:\r\n      boxSize += stream.ReadClass(" },
            { "case 12:\r\n      TTSSpecificConfig()", "case 12:\r\n      boxSize += stream.ReadClass(" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      boxSize += stream.ReadClass(" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "case 24:\r\n      boxSize += stream.ReadClass(" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "case 25:\r\n      boxSize += stream.ReadClass(" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "case 26:\r\n    case 27:\r\n      boxSize += stream.ReadClass(" },
            { "case 28:\r\n      SSCSpecificConfig()", "case 28:\r\n      boxSize += stream.ReadClass(" },
            { "case 30:\r\n      uimsbf(1)", "case 30:\r\n      boxSize += stream.ReadUimsbf(" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "case 32:\r\n    case 33:\r\n    case 34:\r\n      boxSize += stream.ReadClass(" },
            { "case 35:\r\n      DSTSpecificConfig()", "case 35:\r\n      boxSize += stream.ReadClass(" },
            { "case 36:\r\n      bslbf(5)", "case 36:\r\n      boxSize += stream.ReadBslbf(5, " },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "case 37:\r\n    case 38:\r\n      boxSize += stream.ReadClass(" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "case 39:\r\n      boxSize += stream.ReadClass(" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "case 40:\r\n    case 41:\r\n      boxSize += stream.ReadClass(" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      boxSize += stream.ReadBslbf(2, " },
            { "SpatialSpecificConfig", "stream.ReadClass(" },
            { "ALSSpecificConfig", "stream.ReadClass(" },
            { "ErrorProtectionSpecificConfig", "stream.ReadClass(" },
            { "program_config_element", "stream.ReadClass(" },
            { "byte_alignment", "stream.ReadByteAlignment(" },
            { "CelpHeader(samplingFrequencyIndex)", "stream.ReadClass(" },
            { "CelpBWSenhHeader", "stream.ReadClass(" },
            { "HVXCconfig", "stream.ReadClass(" },
            { "TTS_Sequence", "stream.ReadClass(" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "stream.ReadClass(" },
            { "ErHVXCconfig", "stream.ReadClass(" },
            { "PARAconfig", "stream.ReadClass(" },
            { "HILNenexConfig", "stream.ReadClass(" },
            { "HILNconfig", "stream.ReadClass(" },
            { "ld_sbr_header", "stream.ReadClass(" },
            { "sbr_header", "stream.ReadClass(" },
            { "uimsbf(1)[i]", "stream.ReadUimsbf(" },
            { "uimsbf(8)[i]", "stream.ReadUimsbf(8, " },
            { "bslbf(1)[i]", "stream.ReadBslbf(1, " },
            { "uimsbf(4)[i]", "stream.ReadUimsbf(4, " },
            { "uimsbf(1)[c]", "stream.ReadUimsbf(" },
            { "uimsbf(32)[f]", "stream.ReadUimsbf(32, " },
            { "CelpHeader", "stream.ReadClass(" },
            { "ER_SC_CelpHeader", "stream.ReadClass(" },
            { "uimsbf(6)[i]", "stream.ReadUimsbf(6, " },
            { "uimsbf(1)[i][j]", "stream.ReadUimsbf(" },
            { "uimsbf(2)[i][j]", "stream.ReadUimsbf(2, " },
            { "uimsbf(4)[i][j]", "stream.ReadUimsbf(4, " },
            { "uimsbf(16)[i][j]", "stream.ReadUimsbf(16, " },
            { "uimsbf(7)[i][j]", "stream.ReadUimsbf(7, " },
            { "uimsbf(5)[i][j]", "stream.ReadUimsbf(5, " },
            { "uimsbf(6)[i][j]", "stream.ReadUimsbf(6, " },
            { "AV1SampleEntry", "stream.ReadBox(" },
            { "AV1CodecConfigurationBox", "stream.ReadBox(" },
            { "AV1CodecConfigurationRecord", "stream.ReadClass(" },
            { "unsigned int(16)[3]", "stream.ReadUInt16Array(3, " },
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
            { "const unsigned int(8)[6]", "6 * 8" },
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
            { "Box()[]", "IsoStream.CalculateSize(value)" },
            { "Box[]", "IsoStream.CalculateSize(value)" },
            { "Box", "IsoStream.CalculateSize(value)" },
            { "SchemeTypeBox", "IsoStream.CalculateSize(value)" },
            { "SchemeInformationBox", "IsoStream.CalculateSize(value)" },
            { "ItemPropertyContainerBox", "IsoStream.CalculateSize(value)" },
            { "ItemPropertyAssociationBox", "IsoStream.CalculateSize(value)" },
            { "ItemPropertyAssociationBox[]", "IsoStream.CalculateSize(value)" },
            { "char", "8" },
            { "ICC_profile", "IsoStream.CalculateClassSize(value)" },
            { "OriginalFormatBox(fmt)", "IsoStream.CalculateSize(value)" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "IsoStream.CalculateSize(value)" },
            { "ItemInfoEntry[ entry_count ]", "IsoStream.CalculateSize(value)" },
            { "TypeCombinationBox[]", "IsoStream.CalculateSize(value)" },
            { "FilePartitionBox", "IsoStream.CalculateSize(value)" },
            { "FECReservoirBox", "IsoStream.CalculateSize(value)" },
            { "FileReservoirBox", "IsoStream.CalculateSize(value)" },
            { "PartitionEntry[ entry_count ]", "IsoStream.CalculateSize(value)" },
            { "FDSessionGroupBox", "IsoStream.CalculateSize(value)" },
            { "GroupIdToNameBox", "IsoStream.CalculateSize(value)" },
            { "base64string", "(ulong)value.Length * 8" },
            { "ProtectionSchemeInfoBox", "IsoStream.CalculateSize(value)" },
            { "SingleItemTypeReferenceBox", "IsoStream.CalculateSize(value)" },
            { "SingleItemTypeReferenceBox[]", "IsoStream.CalculateSize(value)" },
            { "SingleItemTypeReferenceBoxLarge", "IsoStream.CalculateSize(value)" },
            { "SingleItemTypeReferenceBoxLarge[]", "IsoStream.CalculateSize(value)" },
            { "HandlerBox(handler_type)", "IsoStream.CalculateSize(value)" },
            { "PrimaryItemBox", "IsoStream.CalculateSize(value)" },
            { "DataInformationBox", "IsoStream.CalculateSize(value)" },
            { "ItemLocationBox", "IsoStream.CalculateSize(value)" },
            { "ItemProtectionBox", "IsoStream.CalculateSize(value)" },
            { "ItemInfoBox", "IsoStream.CalculateSize(value)" },
            { "IPMPControlBox", "IsoStream.CalculateSize(value)" },
            { "ItemReferenceBox", "IsoStream.CalculateSize(value)" },
            { "ItemDataBox", "IsoStream.CalculateSize(value)" },
            { "TrackReferenceTypeBox []", "IsoStream.CalculateSize(value)" },
            { "MetaDataKeyBox[]", "IsoStream.CalculateSize(value)" },
            { "TierInfoBox", "IsoStream.CalculateSize(value)" },
            { "MultiviewRelationAttributeBox", "IsoStream.CalculateSize(value)" },
            { "TierBitRateBox", "IsoStream.CalculateSize(value)" },
            { "BufferingBox", "IsoStream.CalculateSize(value)" },
            { "MultiviewSceneInfoBox", "IsoStream.CalculateSize(value)" },
            { "MVDDecoderConfigurationRecord", "IsoStream.CalculateClassSize(value)" },
            { "MVDDepthResolutionBox", "IsoStream.CalculateSize(value)" },
            { "MVCDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "AVCDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "HEVCDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "LHEVCDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "SVCDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "HEVCTileTierLevelConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "EVCDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "VvcDecoderConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "EVCSliceComponentTrackConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "SampleGroupDescriptionEntry (grouping_type)", "IsoStream.CalculateSize(value)" },
            { "Descriptor[0 .. 255]", "IsoStream.CalculateClassSize(value)" },
            { "Descriptor", "IsoStream.CalculateClassSize(value)" },
            { "WebVTTConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "WebVTTSourceLabelBox", "IsoStream.CalculateSize(value)" },
            { "OperatingPointsRecord", "IsoStream.CalculateClassSize(value)" },
            { "VvcSubpicIDEntry", "IsoStream.CalculateSize(value)" },
            { "VvcSubpicOrderEntry", "IsoStream.CalculateSize(value)" },
            { "URIInitBox", "IsoStream.CalculateSize(value)" },
            { "URIBox", "IsoStream.CalculateSize(value)" },
            { "URIbox", "IsoStream.CalculateSize(value)" },
            { "CleanApertureBox", "IsoStream.CalculateSize(value)" },
            { "PixelAspectRatioBox", "IsoStream.CalculateSize(value)" },
            { "DownMixInstructions() []", "IsoStream.CalculateSize(value)" },
            { "DRCCoefficientsBasic() []", "IsoStream.CalculateClassSize(value)" },
            { "DRCInstructionsBasic() []", "IsoStream.CalculateClassSize(value)" },
            { "DRCCoefficientsUniDRC() []", "IsoStream.CalculateClassSize(value)" },
            { "DRCInstructionsUniDRC() []", "IsoStream.CalculateClassSize(value)" },
            { "HEVCConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "LHEVCConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "AVCConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "SVCConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "ScalabilityInformationSEIBox", "IsoStream.CalculateSize(value)" },
            { "SVCPriorityAssignmentBox", "IsoStream.CalculateSize(value)" },
            { "ViewScalabilityInformationSEIBox", "IsoStream.CalculateSize(value)" },
            { "ViewIdentifierBox", "IsoStream.CalculateSize(value)" },
            { "MVCConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "MVCViewPriorityAssignmentBox", "IsoStream.CalculateSize(value)" },
            { "IntrinsicCameraParametersBox", "IsoStream.CalculateSize(value)" },
            { "ExtrinsicCameraParametersBox", "IsoStream.CalculateSize(value)" },
            { "MVCDConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "MVDScalabilityInformationSEIBox", "IsoStream.CalculateSize(value)" },
            { "A3DConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "VvcOperatingPointsRecord", "IsoStream.CalculateClassSize(value)" },
            { "VVCSubpicIDRewritingInfomationStruct()", "IsoStream.CalculateClassSize(value)" },
            { "MPEG4ExtensionDescriptorsBox ()", "IsoStream.CalculateSize(value)" },
            { "MPEG4ExtensionDescriptorsBox()", "IsoStream.CalculateSize(value)" },
            { "MPEG4ExtensionDescriptorsBox", "IsoStream.CalculateSize(value)" },
            { "bit(8*dci_nal_unit_length)", "(ulong)dci_nal_unit_length * 8" },
            { "DependencyInfo[numReferences]", "IsoStream.CalculateClassSize(value)" },
            { "VvcPTLRecord(0)[i]", "IsoStream.CalculateClassSize(value)" },
            { "EVCSliceComponentTrackConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "SVCMetadataSampleConfigBox", "IsoStream.CalculateSize(value)" },
            { "SVCPriorityLayerInfoBox", "IsoStream.CalculateSize(value)" },
            { "EVCConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "VvcNALUConfigBox", "IsoStream.CalculateSize(value)" },
            { "VvcConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "HEVCTileConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataKeyTableBox()", "IsoStream.CalculateSize(value)" },
            { "BitRateBox ()", "IsoStream.CalculateSize(value)" },
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
            { "string[method_count]", "IsoStream.CalculateSize(value)" },
            { "ItemInfoExtension", "IsoStream.CalculateClassSize(value)" },
            { "SampleGroupDescriptionEntry", "IsoStream.CalculateSize(value)" },
            { "SampleEntry", "IsoStream.CalculateSize(value)" },
            { "SampleConstructor", "IsoStream.CalculateSize(value)" },
            { "InlineConstructor", "IsoStream.CalculateSize(value)" },
            { "SampleConstructorFromTrackGroup", "IsoStream.CalculateSize(value)" },
            { "NALUStartInlineConstructor", "IsoStream.CalculateSize(value)" },
            { "MPEG4BitRateBox", "IsoStream.CalculateSize(value)" },
            { "ChannelLayout", "IsoStream.CalculateSize(value)" },
            { "UniDrcConfigExtension", "IsoStream.CalculateSize(value)" },
            { "SamplingRateBox", "IsoStream.CalculateSize(value)" },
            { "TextConfigBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataKeyTableBox", "IsoStream.CalculateSize(value)" },
            { "BitRateBox", "IsoStream.CalculateSize(value)" },
            { "CompleteTrackInfoBox", "IsoStream.CalculateSize(value)" },
            { "TierDependencyBox", "IsoStream.CalculateSize(value)" },
            { "InitialParameterSetBox", "IsoStream.CalculateSize(value)" },
            { "PriorityRangeBox", "IsoStream.CalculateSize(value)" },
            { "ViewPriorityBox", "IsoStream.CalculateSize(value)" },
            { "SVCDependencyRangeBox", "IsoStream.CalculateSize(value)" },
            { "RectRegionBox", "IsoStream.CalculateSize(value)" },
            { "IroiInfoBox", "IsoStream.CalculateSize(value)" },
            { "TranscodingInfoBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataKeyDeclarationBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataDatatypeBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataLocaleBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataSetupBox", "IsoStream.CalculateSize(value)" },
            { "MetaDataExtensionsBox", "IsoStream.CalculateSize(value)" },
            { "TrackLoudnessInfo[]", "IsoStream.CalculateSize(value)" },
            { "AlbumLoudnessInfo[]", "IsoStream.CalculateSize(value)" },
            { "VvcPTLRecord(num_sublayers)", "IsoStream.CalculateClassSize(value)" },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "IsoStream.CalculateClassSize(value)" },
            { "OpusSpecificBox", "IsoStream.CalculateSize(value)" },
            { "unsigned int(8 * OutputChannelCount)", "(ulong)(OutputChannelCount * 8)" },
            { "ChannelMappingTable", "IsoStream.CalculateClassSize(value)" },
            // descriptors
            { "DecoderConfigDescriptor", "IsoStream.CalculateClassSize(value)" },
            { "SLConfigDescriptor", "IsoStream.CalculateClassSize(value)" },
            { "IPI_DescrPointer[0 .. 1]", "IsoStream.CalculateClassSize(value)" },
            { "IP_IdentificationDataSet[0 .. 255]", "IsoStream.CalculateClassSize(value)" },
            { "IPMP_DescriptorPointer[0 .. 255]", "IsoStream.CalculateClassSize(value)" },
            { "LanguageDescriptor[0 .. 255]", "IsoStream.CalculateClassSize(value)" },
            { "QoS_Descriptor[0 .. 1]", "IsoStream.CalculateClassSize(value)" },
            { "RegistrationDescriptor[0 .. 1]", "IsoStream.CalculateClassSize(value)" },
            { "ExtensionDescriptor[0 .. 255]", "IsoStream.CalculateClassSize(value)" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "IsoStream.CalculateClassSize(value)" },
            { "DecoderSpecificInfo[0 .. 1]", "IsoStream.CalculateClassSize(value)" },
            { "bit(8)[URLlength]", "(ulong)(URLlength * 8)" },
            { "bit(8)[sizeOfInstance-4]", "(ulong)(sizeOfInstance - 4)" },
            { "double(32)", "32" },
            { "QoS_Qualifier[]", "IsoStream.CalculateClassSize(value)" },

            { "GetAudioObjectType()", "IsoStream.CalculateClassSize(value)" },
            { "bslbf(header_size * 8)[]", "header_size * 8" },
            { "bslbf(trailer_size * 8)[]", "trailer_size * 8" },
            { "bslbf(aux_size * 8)[]", "aux_size * 8" },
            { "bslbf(11)", "11" },
            { "bslbf(5)", "5" },
            { "bslbf(4)", "4" },
            { "bslbf(1)", "1" },
            { "uimsbf(32)", "32" },
            { "uimsbf(24)", "24" },
            { "uimsbf(18)", "18" },
            { "uimsbf(16)", "16" },
            { "uimsbf(14)", "14" },
            { "uimsbf(12)", "12" },
            { "uimsbf(10)", "10" },
            { "uimsbf(8)", "8" },
            { "uimsbf(7)", "7" },
            { "uimsbf(6)", "6" },
            { "uimsbf(5)", "5" },
            { "uimsbf(4)", "4" },
            { "uimsbf(3)", "3" },
            { "uimsbf(2)", "2" },
            { "uimsbf(1)", "1" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n     boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 8:\r\n      CelpSpecificConfig()", "case 8:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 9:\r\n      HvxcSpecificConfig()", "case 9:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 12:\r\n      TTSSpecificConfig()", "case 12:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "case 24:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "case 25:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "case 26:\r\n    case 27:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 28:\r\n      SSCSpecificConfig()", "case 28:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 30:\r\n      uimsbf(1)", "boxSize += 1" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "case 32:\r\n    case 33:\r\n    case 34:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 35:\r\n      DSTSpecificConfig()", "case 35:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 36:\r\n      bslbf(5)", "boxSize += 5" },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "case 37:\r\n    case 38:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "case 39:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "case 40:\r\n    case 41:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n     boxSize +=  2" },
            { "SpatialSpecificConfig", "IsoStream.CalculateClassSize(value)" },
            { "ALSSpecificConfig", "IsoStream.CalculateClassSize(value)" },
            { "ErrorProtectionSpecificConfig", "IsoStream.CalculateClassSize(value)" },
            { "program_config_element", "IsoStream.CalculateClassSize(value)" },
            { "byte_alignment", "IsoStream.CalculateByteAlignmentSize(value)" },
            { "CelpHeader(samplingFrequencyIndex)", "IsoStream.CalculateClassSize(value)" },
            { "CelpBWSenhHeader", "IsoStream.CalculateClassSize(value)" },
            { "HVXCconfig", "IsoStream.CalculateClassSize(value)" },
            { "TTS_Sequence", "IsoStream.CalculateClassSize(value)" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "IsoStream.CalculateClassSize(value)" },
            { "ErHVXCconfig", "IsoStream.CalculateClassSize(value)" },
            { "PARAconfig", "IsoStream.CalculateClassSize(value)" },
            { "HILNenexConfig", "IsoStream.CalculateClassSize(value)" },
            { "HILNconfig", "IsoStream.CalculateClassSize(value)" },
            { "ld_sbr_header", "IsoStream.CalculateClassSize(value)" },
            { "sbr_header", "IsoStream.CalculateClassSize(value)" },
            { "uimsbf(1)[i]", "1" },
            { "uimsbf(8)[i]", "8" },
            { "bslbf(1)[i]", "1" },
            { "uimsbf(4)[i]", "4" },
            { "uimsbf(1)[c]", "1" },
            { "uimsbf(32)[f]", "32" },
            { "CelpHeader", "IsoStream.CalculateClassSize(value)" },
            { "ER_SC_CelpHeader", "IsoStream.CalculateClassSize(value)" },
            { "uimsbf(6)[i]", "6" },
            { "uimsbf(1)[i][j]", "1" },
            { "uimsbf(2)[i][j]", "2" },
            { "uimsbf(4)[i][j]", "4" },
            { "uimsbf(16)[i][j]", "16" },
            { "uimsbf(7)[i][j]", "7" },
            { "uimsbf(5)[i][j]", "5" },
            { "uimsbf(6)[i][j]", "6" },
            { "AV1SampleEntry", "IsoStream.CalculateSize(value)" },
            { "AV1CodecConfigurationBox", "IsoStream.CalculateSize(value)" },
            { "AV1CodecConfigurationRecord","IsoStream.CalculateClassSize(value)" },
            { "unsigned int(16)[3]", "3 * 16" },
       };
        return map[type];
    }

    private static string GetWriteMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)[ entry_count ]", "stream.WriteUInt64Array(entry_count, " },
            { "unsigned int(64)", "stream.WriteUInt64(" },
            { "unsigned int(48)", "stream.WriteUInt48(" },
            { "template int(32)[9]", "stream.WriteUInt32Array(9, " },
            { "unsigned int(32)[ entry_count ]", "stream.WriteUInt32Array(entry_count, " },
            { "unsigned int(32)[3]", "stream.WriteUInt32Array(3, " },
            { "unsigned int(32)", "stream.WriteUInt32(" },
            { "unsigned_int(32)", "stream.WriteUInt32(" },
            { "unsigned int(24)", "stream.WriteUInt24(" },
            { "unsigned int(29)", "stream.WriteBits(29, " },
            { "unsigned int(26)", "stream.WriteBits(26, " },
            { "unsigned int(16)[i]", "stream.WriteUInt16(" },
            { "unsigned int(16)[j]", "stream.WriteUInt16(" },
            { "unsigned int(16)[i][j]", "stream.WriteUInt16(" },
            { "unsigned int(16)", "stream.WriteUInt16(" },
            { "unsigned_int(16)", "stream.WriteUInt16(" },
            { "unsigned int(15)", "stream.WriteBits(15, " },
            { "unsigned int(12)", "stream.WriteBits(12, " },
            { "signed int(12)", "stream.WriteBits(12, " },
            { "unsigned int(10)[i][j]", "stream.WriteBits(10, " },
            { "unsigned int(10)[i]", "stream.WriteBits(10, " },
            { "unsigned int(10)", "stream.WriteBits(10, " },
            { "unsigned int(8)[ sample_count ]", "stream.WriteBytes(sample_count, " },
            { "unsigned int(8)[length]", "stream.WriteBytes(length, " },
            { "unsigned int(8)[32]", "stream.WriteBytes(32, " },
            { "unsigned int(8)[16]", "stream.WriteBytes(16, " },
            { "unsigned int(9)", "stream.WriteBits(9, " },
            { "unsigned int(8)", "stream.WriteUInt8(" },
            { "unsigned int(7)", "stream.WriteBits(7, " },
            { "unsigned int(6)", "stream.WriteBits(6, " },
            { "unsigned int(5)[3]", "stream.WriteBitsArray(5, 3, " },
            { "unsigned int(5)", "stream.WriteBits(5, " },
            { "unsigned int(4)", "stream.WriteBits(4, " },
            { "unsigned int(3)", "stream.WriteBits(3, " },
            { "unsigned int(2)[i][j]", "stream.WriteBits(2, " },
            { "unsigned int(2)", "stream.WriteBits(2, " },
            { "unsigned int(1)[i]", "stream.WriteBit(" },
            { "unsigned int(1)", "stream.WriteBit(" },
            { "unsigned int (32)", "stream.WriteUInt32(" },
            { "unsigned int(f(pattern_size_code))[i]", "stream.WriteBits(pattern_size_code, " },
            { "unsigned int(f(index_size_code))[i]", "stream.WriteBits(index_size_code, " },
            { "unsigned int(f(index_size_code))[j][k]", "stream.WriteBits(index_size_code, " },
            { "unsigned int(f(count_size_code))[i]", "stream.WriteBits(count_size_code, " },
            { "unsigned int(base_offset_size*8)", "stream.WriteBytes(base_offset_size, " },
            { "unsigned int(offset_size*8)", "stream.WriteBytes(offset_size, " },
            { "unsigned int(length_size*8)", "stream.WriteBytes(length_size, " },
            { "unsigned int(index_size*8)", "stream.WriteBytes(index_size, " },
            { "unsigned int(field_size)", "stream.WriteBytes(field_size, " },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "stream.WriteBytes((ulong)(length_size_of_traf_num+1), " },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "stream.WriteBytes((ulong)(length_size_of_trun_num+1), " },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "stream.WriteBytes((ulong)(length_size_of_sample_num+1), " },
            { "unsigned int(8*size-64)", "stream.WriteBytes(size-64, " },
            { "unsigned int(subgroupIdLen)[i]", "stream.WriteUInt32(" },
            { "const unsigned int(8)[6]", "stream.WriteUInt8Array(6, " },
            { "const unsigned int(32)[2]", "stream.WriteUInt32Array(2, " },
            { "const unsigned int(32)[3]", "stream.WriteUInt32Array(3, " },
            { "const unsigned int(32)", "stream.WriteUInt32(" },
            { "const unsigned int(16)[3]", "stream.WriteUInt16Array(3, " },
            { "const unsigned int(16)", "stream.WriteUInt16(" },
            { "const unsigned int(26)", "stream.WriteBits(26, " },
            { "template int(32)", "stream.WriteInt32(" },
            { "template int(16)", "stream.WriteInt16(" },
            { "template unsigned int(30)", "stream.WriteBits(30, " },
            { "template unsigned int(32)", "stream.WriteUInt32(" },
            { "template unsigned int(16)[3]", "stream.WriteUInt16Array(3, " },
            { "template unsigned int(16)", "stream.WriteUInt16(" },
            { "template unsigned int(8)[]", "stream.WriteUInt8Array(" },
            { "template unsigned int(8)", "stream.WriteUInt8(" },
            { "int(64)", "stream.WriteInt64(" },
            { "int(32)", "stream.WriteInt32(" },
            { "int(16)", "stream.WriteInt16(" },
            { "int(8)", "stream.WriteInt8(" },
            { "int(4)", "stream.WriteBits(4, " },
            { "int", "stream.WriteInt32(" },
            { "const bit(16)", "stream.WriteUInt16(" },
            { "const bit(1)", "stream.WriteBit(" },
            { "bit(1)", "stream.WriteBit(" },
            { "bit(2)", "stream.WriteBits(2, " },
            { "bit(3)", "stream.WriteBits(3, " },
            { "bit(4)", "stream.WriteBits(4, " },
            { "bit(5)", "stream.WriteBits(5, " },
            { "bit(6)", "stream.WriteBits(6, " },
            { "bit(7)", "stream.WriteBits(7, " },
            { "bit(8)[]", "stream.WriteUInt8Array(" },
            { "bit(8)", "stream.WriteUInt8(" },
            { "bit(16)[i]", "stream.WriteUInt16(" },
            { "bit(16)", "stream.WriteUInt16(" },
            { "bit(24)", "stream.WriteBits(24, " },
            { "bit(31)", "stream.WriteBits(31, " },
            { "bit(8 ceil(size / 8) \u2013 size)", "stream.WriteBytes((ulong)(Math.Ceiling(size / 8d) - size), " },
            { "bit(8* ps_nalu_length)", "stream.WriteBytes(ps_nalu_length, " },
            { "bit(8*nalUnitLength)", "stream.WriteBytes(nalUnitLength, " },
            { "bit(8*sequenceParameterSetLength)", "stream.WriteBytes(sequenceParameterSetLength, " },
            { "bit(8*pictureParameterSetLength)", "stream.WriteBytes(pictureParameterSetLength, " },
            { "bit(8*sequenceParameterSetExtLength)", "stream.WriteBytes(sequenceParameterSetExtLength, " },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "stream.WriteBytes((ulong)(num_bytes_constraint_info - 2), " },
            { "bit(8*nal_unit_length)", "stream.WriteBytes(nal_unit_length, " },
            { "bit(timeStampLength)", "stream.WriteBytes(timeStampLength, " },
            { "utf8string", "stream.WriteString(" },
            { "utfstring", "stream.WriteString(" },
            { "utf8list", "stream.WriteString(" },
            { "boxstring", "stream.WriteString(" },
            { "string", "stream.WriteString(" },
            { "bit(32)[6]", "stream.WriteUInt32Array(6, " },
            { "bit(32)", "stream.WriteUInt32(" },
            { "uint(32)", "stream.WriteUInt32(" },
            { "uint(16)", "stream.WriteUInt16(" },
            { "uint(64)", "stream.WriteUInt64(" },
            { "uint(8)[32]", "stream.WriteBytes(32, " },
            { "uint(8)", "stream.WriteUInt8(" },
            { "uint(7)", "stream.WriteBits(7, " },
            { "uint(1)", "stream.WriteBits(1, " },
            { "signed   int(64)", "stream.WriteInt64(" },
            { "signed int(32)", "stream.WriteInt32(" },
            { "signed int (16)", "stream.WriteInt16(" },
            { "signed int(16)[grid_pos_view_id[i]]", "stream.WriteInt16(" },
            { "signed int(16)", "stream.WriteInt16(" },
            { "signed int (8)", "stream.WriteInt8(" },
            { "signed int(64)", "stream.WriteInt64(" },
            { "signed   int(32)", "stream.WriteInt32(" },
            { "signed   int(8)", "stream.WriteInt8(" },
            { "Box()[]", "stream.WriteBox(" },
            { "Box[]", "stream.WriteBox(" },
            { "Box", "stream.WriteBox(" },
            { "SchemeTypeBox", "stream.WriteBox(" },
            { "SchemeInformationBox", "stream.WriteBox(" },
            { "ItemPropertyContainerBox", "stream.WriteBox(" },
            { "ItemPropertyAssociationBox", "stream.WriteBox(" },
            { "ItemPropertyAssociationBox[]", "stream.WriteBox(" },
            { "char", "stream.WriteInt8(" },
            { "ICC_profile", "stream.WriteClass(" },
            { "OriginalFormatBox(fmt)", "stream.WriteBox(" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "stream.WriteBox(" },
            { "ItemInfoEntry[ entry_count ]", "stream.WriteBox(entry_count, " },
            { "TypeCombinationBox[]", "stream.WriteBox(" },
            { "FilePartitionBox", "stream.WriteBox(" },
            { "FECReservoirBox", "stream.WriteBox(" },
            { "FileReservoirBox", "stream.WriteBox(" },
            { "PartitionEntry[ entry_count ]", "stream.WriteBox(entry_count, " },
            { "FDSessionGroupBox", "stream.WriteBox(" },
            { "GroupIdToNameBox", "stream.WriteBox(" },
            { "base64string", "stream.WriteString(" },
            { "ProtectionSchemeInfoBox", "stream.WriteBox(" },
            { "SingleItemTypeReferenceBox", "stream.WriteBox(" },
            { "SingleItemTypeReferenceBox[]", "stream.WriteBox(" },
            { "SingleItemTypeReferenceBoxLarge", "stream.WriteBox(" },
            { "SingleItemTypeReferenceBoxLarge[]", "stream.WriteBox(" },
            { "HandlerBox(handler_type)", "stream.WriteBox(" },
            { "PrimaryItemBox", "stream.WriteBox(" },
            { "DataInformationBox", "stream.WriteBox(" },
            { "ItemLocationBox", "stream.WriteBox(" },
            { "ItemProtectionBox", "stream.WriteBox(" },
            { "ItemInfoBox", "stream.WriteBox(" },
            { "IPMPControlBox", "stream.WriteBox(" },
            { "ItemReferenceBox", "stream.WriteBox(" },
            { "ItemDataBox", "stream.WriteBox(" },
            { "TrackReferenceTypeBox []", "stream.WriteBox(" },
            { "MetaDataKeyBox[]", "stream.WriteBox(" },
            { "TierInfoBox", "stream.WriteBox(" },
            { "MultiviewRelationAttributeBox", "stream.WriteBox(" },
            { "TierBitRateBox", "stream.WriteBox(" },
            { "BufferingBox", "stream.WriteBox(" },
            { "MultiviewSceneInfoBox", "stream.WriteBox(" },
            { "MVDDecoderConfigurationRecord", "stream.WriteClass(" },
            { "MVDDepthResolutionBox", "stream.WriteBox(" },
            { "MVCDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "AVCDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "HEVCDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "LHEVCDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "SVCDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "HEVCTileTierLevelConfigurationRecord()", "stream.WriteClass(" },
            { "EVCDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "VvcDecoderConfigurationRecord()", "stream.WriteClass(" },
            { "EVCSliceComponentTrackConfigurationRecord()", "stream.WriteClass(" },
            { "SampleGroupDescriptionEntry (grouping_type)", "stream.WriteBox(" },
            { "Descriptor[0 .. 255]", "stream.WriteClass(" },
            { "Descriptor", "stream.WriteClass(" },
            { "WebVTTConfigurationBox", "stream.WriteBox(" },
            { "WebVTTSourceLabelBox", "stream.WriteBox(" },
            { "OperatingPointsRecord", "stream.WriteClass(" },
            { "VvcSubpicIDEntry", "stream.WriteBox(" },
            { "VvcSubpicOrderEntry", "stream.WriteBox(" },
            { "URIInitBox", "stream.WriteBox(" },
            { "URIBox", "stream.WriteBox(" },
            { "URIbox", "stream.WriteBox(" },
            { "CleanApertureBox", "stream.WriteBox(" },
            { "PixelAspectRatioBox", "stream.WriteBox(" },
            { "DownMixInstructions() []", "stream.WriteBox(" },
            { "DRCCoefficientsBasic() []", "stream.WriteClass(" },
            { "DRCInstructionsBasic() []", "stream.WriteClass(" },
            { "DRCCoefficientsUniDRC() []", "stream.WriteClass(" },
            { "DRCInstructionsUniDRC() []", "stream.WriteClass(" },
            { "HEVCConfigurationBox", "stream.WriteBox(" },
            { "LHEVCConfigurationBox", "stream.WriteBox(" },
            { "AVCConfigurationBox", "stream.WriteBox(" },
            { "SVCConfigurationBox", "stream.WriteBox(" },
            { "ScalabilityInformationSEIBox", "stream.WriteBox(" },
            { "SVCPriorityAssignmentBox", "stream.WriteBox(" },
            { "ViewScalabilityInformationSEIBox", "stream.WriteBox(" },
            { "ViewIdentifierBox", "stream.WriteBox(" },
            { "MVCConfigurationBox", "stream.WriteBox(" },
            { "MVCViewPriorityAssignmentBox", "stream.WriteBox(" },
            { "IntrinsicCameraParametersBox", "stream.WriteBox(" },
            { "ExtrinsicCameraParametersBox", "stream.WriteBox(" },
            { "MVCDConfigurationBox", "stream.WriteBox(" },
            { "MVDScalabilityInformationSEIBox", "stream.WriteBox(" },
            { "A3DConfigurationBox", "stream.WriteBox(" },
            { "VvcOperatingPointsRecord", "stream.WriteClass(" },
            { "VVCSubpicIDRewritingInfomationStruct()", "stream.WriteClass(" },
            { "MPEG4ExtensionDescriptorsBox ()", "stream.WriteBox(" },
            { "MPEG4ExtensionDescriptorsBox()", "stream.WriteBox(" },
            { "MPEG4ExtensionDescriptorsBox", "stream.WriteBox(" },
            { "bit(8*dci_nal_unit_length)", "stream.WriteBytes(dci_nal_unit_length, " },
            { "DependencyInfo[numReferences]", "stream.WriteClass(numReferences, " },
            { "VvcPTLRecord(0)[i]", "stream.WriteClass(" },
            { "EVCSliceComponentTrackConfigurationBox", "stream.WriteBox(" },
            { "SVCMetadataSampleConfigBox", "stream.WriteBox(" },
            { "SVCPriorityLayerInfoBox", "stream.WriteBox(" },
            { "EVCConfigurationBox", "stream.WriteBox(" },
            { "VvcNALUConfigBox", "stream.WriteBox(" },
            { "VvcConfigurationBox", "stream.WriteBox(" },
            { "HEVCTileConfigurationBox", "stream.WriteBox(" },
            { "MetaDataKeyTableBox()", "stream.WriteBox(" },
            { "BitRateBox ()", "stream.WriteBox(" },
            { "char[count]", "stream.WriteUInt8Array(count, " },
            { "signed int(32)[ c ]", "stream.WriteInt32(" },
            { "unsigned int(8)[]", "stream.WriteUInt8Array(" },
            { "unsigned int(8)[i]", "stream.WriteUInt8(" },
            { "unsigned int(6)[i]", "stream.WriteBits(6, " },
            { "unsigned int(6)[i][j]", "stream.WriteBits(6, " },
            { "unsigned int(1)[i][j]", "stream.WriteBits(1, " },
            { "unsigned int(9)[i]", "stream.WriteBits(9, " },
            { "unsigned int(32)[]", "stream.WriteUInt32Array(" },
            { "unsigned int(32)[i]", "stream.WriteUInt32(" },
            { "unsigned int(32)[j]", "stream.WriteUInt32(" },
            { "unsigned int(8)[j][k]", "stream.WriteUInt8(" },
            { "signed   int(64)[j][k]", "stream.WriteInt64(" },
            { "unsigned int(8)[j]", "stream.WriteUInt8(" },
            { "signed   int(64)[j]", "stream.WriteInt64(" },
            { "char[]", "stream.WriteUInt8Array(" },
            { "string[method_count]", "stream.WriteStringArray(method_count, " },
             { "ItemInfoExtension", "stream.WriteClass(" },
            { "SampleGroupDescriptionEntry", "stream.WriteBox(" },
            { "SampleEntry", "stream.WriteBox(" },
            { "SampleConstructor", "stream.WriteBox(" },
            { "InlineConstructor", "stream.WriteBox(" },
            { "SampleConstructorFromTrackGroup", "stream.WriteBox(" },
            { "NALUStartInlineConstructor", "stream.WriteBox(" },
            { "MPEG4BitRateBox", "stream.WriteBox(" },
            { "ChannelLayout", "stream.WriteBox(" },
            { "UniDrcConfigExtension", "stream.WriteBox(" },
            { "SamplingRateBox", "stream.WriteBox(" },
            { "TextConfigBox", "stream.WriteBox(" },
            { "MetaDataKeyTableBox", "stream.WriteBox(" },
            { "BitRateBox", "stream.WriteBox(" },
            { "CompleteTrackInfoBox", "stream.WriteBox(" },
            { "TierDependencyBox", "stream.WriteBox(" },
            { "InitialParameterSetBox", "stream.WriteBox(" },
            { "PriorityRangeBox", "stream.WriteBox(" },
            { "ViewPriorityBox", "stream.WriteBox(" },
            { "SVCDependencyRangeBox", "stream.WriteBox(" },
            { "RectRegionBox", "stream.WriteBox(" },
            { "IroiInfoBox", "stream.WriteBox(" },
            { "TranscodingInfoBox", "stream.WriteBox(" },
            { "MetaDataKeyDeclarationBox", "stream.WriteBox(" },
            { "MetaDataDatatypeBox", "stream.WriteBox(" },
            { "MetaDataLocaleBox", "stream.WriteBox(" },
            { "MetaDataSetupBox", "stream.WriteBox(" },
            { "MetaDataExtensionsBox", "stream.WriteBox(" },
            { "TrackLoudnessInfo[]", "stream.WriteBox(" },
            { "AlbumLoudnessInfo[]", "stream.WriteBox(" },
            { "VvcPTLRecord(num_sublayers)", "stream.WriteClass(num_sublayers, " },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "stream.WriteClass(" },
            { "OpusSpecificBox", "stream.WriteBox(" },
            { "unsigned int(8 * OutputChannelCount)", "stream.WriteBytes(OutputChannelCount, " },
            { "ChannelMappingTable", "stream.WriteClass(" },
            // descriptors
            { "DecoderConfigDescriptor", "stream.WriteClass(" },
            { "SLConfigDescriptor", "stream.WriteClass(" },
            { "IPI_DescrPointer[0 .. 1]", "stream.WriteClass(" },
            { "IP_IdentificationDataSet[0 .. 255]", "stream.WriteClass(" },
            { "IPMP_DescriptorPointer[0 .. 255]", "stream.WriteClass(" },
            { "LanguageDescriptor[0 .. 255]", "stream.WriteClass(" },
            { "QoS_Descriptor[0 .. 1]", "stream.WriteClass(" },
            { "RegistrationDescriptor[0 .. 1]", "stream.WriteClass(" },
            { "ExtensionDescriptor[0 .. 255]", "stream.WriteClass(" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "stream.WriteClass(" },
            { "DecoderSpecificInfo[0 .. 1]", "stream.WriteClass(" },
            { "bit(8)[URLlength]", "stream.WriteBytes(URLlength, " },
            { "bit(8)[sizeOfInstance-4]", "stream.WriteBytes((ulong)(sizeOfInstance - 4), " },
            { "double(32)", "stream.WriteDouble32(" },
            { "QoS_Qualifier[]", "stream.WriteClass(" },
            { "GetAudioObjectType()", "stream.WriteClass(" },
            { "bslbf(header_size * 8)[]", "stream.WriteBslbf(header_size * 8, " },
            { "bslbf(trailer_size * 8)[]", "stream.WriteBslbf(trailer_size * 8, " },
            { "bslbf(aux_size * 8)[]", "stream.WriteBslbf(aux_size * 8, " },
            { "bslbf(11)", "stream.WriteBslbf(11, " },
            { "bslbf(5)", "stream.WriteBslbf(5, " },
            { "bslbf(4)", "stream.WriteBslbf(4, " },
            { "bslbf(1)", "stream.WriteBslbf(" },
            { "uimsbf(32)", "stream.WriteUimsbf(32, " },
            { "uimsbf(24)", "stream.WriteUimsbf(24, " },
            { "uimsbf(18)", "stream.WriteUimsbf(18, " },
            { "uimsbf(16)", "stream.WriteUimsbf(16, " },
            { "uimsbf(14)", "stream.WriteUimsbf(14, " },
            { "uimsbf(12)", "stream.WriteUimsbf(12, " },
            { "uimsbf(10)", "stream.WriteUimsbf(10, " },
            { "uimsbf(8)", "stream.WriteUimsbf(8, " },
            { "uimsbf(7)", "stream.WriteUimsbf(7, " },
            { "uimsbf(6)", "stream.WriteUimsbf(6, " },
            { "uimsbf(5)", "stream.WriteUimsbf(5, " },
            { "uimsbf(4)", "stream.WriteUimsbf(4, " },
            { "uimsbf(3)", "stream.WriteUimsbf(3, " },
            { "uimsbf(2)", "stream.WriteUimsbf(2, " },
            { "uimsbf(1)", "stream.WriteUimsbf(" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      boxSize += stream.WriteClass(" },
            { "case 8:\r\n      CelpSpecificConfig()", "case 8:\r\n      boxSize += stream.WriteClass(" },
            { "case 9:\r\n      HvxcSpecificConfig()", "case 9:\r\n      boxSize += stream.WriteClass(" },
            { "case 12:\r\n      TTSSpecificConfig()", "case 12:\r\n      boxSize += stream.WriteClass(" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      boxSize += stream.WriteClass(" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "case 24:\r\n      boxSize += stream.WriteClass(" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "case 25:\r\n      boxSize += stream.WriteClass(" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "case 26:\r\n    case 27:\r\n      boxSize += stream.WriteClass(" },
            { "case 28:\r\n      SSCSpecificConfig()", "case 28:\r\n      boxSize += stream.WriteClass(" },
            { "case 30:\r\n      uimsbf(1)", "case 30:\r\n      boxSize += stream.WriteUimsbf(" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "case 32:\r\n    case 33:\r\n    case 34:\r\n      boxSize += stream.WriteClass(" },
            { "case 35:\r\n      DSTSpecificConfig()", "case 35:\r\n      boxSize += stream.WriteClass(" },
            { "case 36:\r\n      bslbf(5)", "case 36:\r\n      boxSize += stream.WriteBslbf(5, " },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "case 37:\r\n    case 38:\r\n      boxSize += stream.WriteClass(" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "case 39:\r\n      boxSize += stream.WriteClass(" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "case 40:\r\n    case 41:\r\n      boxSize += stream.WriteClass(" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      boxSize += stream.WriteBslbf(2, " },
            { "SpatialSpecificConfig", "stream.WriteClass(" },
            { "ALSSpecificConfig", "stream.WriteClass(" },
            { "ErrorProtectionSpecificConfig", "stream.WriteClass(" },
            { "program_config_element", "stream.WriteClass(" },
            { "byte_alignment", "stream.WriteByteAlignment(" },
            { "CelpHeader(samplingFrequencyIndex)", "stream.WriteClass(" },
            { "CelpBWSenhHeader", "stream.WriteClass(" },
            { "HVXCconfig", "stream.WriteClass(" },
            { "TTS_Sequence", "stream.WriteClass(" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "stream.WriteClass(" },
            { "ErHVXCconfig", "stream.WriteClass(" },
            { "PARAconfig", "stream.WriteClass(" },
            { "HILNenexConfig", "stream.WriteClass(" },
            { "HILNconfig", "stream.WriteClass(" },
            { "ld_sbr_header", "stream.WriteClass(" },
            { "sbr_header", "stream.WriteClass(" },
            { "uimsbf(1)[i]", "stream.WriteUimsbf(" },
            { "uimsbf(8)[i]", "stream.WriteUimsbf(8, " },
            { "bslbf(1)[i]", "stream.WriteBslbf(1, " },
            { "uimsbf(4)[i]", "stream.WriteUimsbf(4, " },
            { "uimsbf(1)[c]", "stream.WriteUimsbf(" },
            { "uimsbf(32)[f]", "stream.WriteUimsbf(32, " },
            { "CelpHeader", "stream.WriteClass(" },
            { "ER_SC_CelpHeader", "stream.WriteClass(" },
            { "uimsbf(6)[i]", "stream.WriteUimsbf(6, " },
            { "uimsbf(1)[i][j]", "stream.WriteUimsbf(" },
            { "uimsbf(2)[i][j]", "stream.WriteUimsbf(2, " },
            { "uimsbf(4)[i][j]", "stream.WriteUimsbf(4, " },
            { "uimsbf(16)[i][j]", "stream.WriteUimsbf(16, " },
            { "uimsbf(7)[i][j]", "stream.WriteUimsbf(7, " },
            { "uimsbf(5)[i][j]", "stream.WriteUimsbf(5, " },
            { "uimsbf(6)[i][j]", "stream.WriteUimsbf(6, " },
            { "AV1SampleEntry", "stream.WriteBox(" },
            { "AV1CodecConfigurationBox", "stream.WriteBox(" },
            { "AV1CodecConfigurationRecord", "stream.WriteClass(" },
            { "unsigned int(16)[3]", "stream.WriteUInt16Array(3, " },
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
            "int sizeOfInstance = 0",
            "sbrPresentFlag = -1", // WORKAROUND
            "psPresentFlag = -1", // WORKAROUND
            "extensionAudioObjectType = 0", // WORKAROUND
            "extensionAudioObjectType = 5", // WORKAROUND
            "sbrPresentFlag = 1", // WORKAROUND
            "psPresentFlag = 1", // WORKAROUND
            "sbrPresentFlag = -1",
            "psPresentFlag = -1",
            "extensionAudioObjectType = 0",
            "extensionAudioObjectType = 5",
            "sbrPresentFlag = 1",
            "psPresentFlag = 1",
            "audioObjectType = 32 + audioObjectTypeExt",
            "return audioObjectType",
            "break",
            "len = eldExtLen",
            "len += eldExtLenAddAdd",
            "len += eldExtLenAdd",
            "default:\r\n      /* reserved */\r\n      break",
            "default:\r\n        int cntt",
            "case 1:\r\n    case 2:\r\n      numSbrHeader = 1",
            "case 3:\r\n      numSbrHeader = 2",
            "case 4:\r\n    case 5:\r\n    case 6:\r\n      numSbrHeader = 3",
            "case 7:\r\n      numSbrHeader = 4",
            "default:\r\n      numSbrHeader = 0",
            "FieldLength = (large_size + 1) * 16",
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
            { "const unsigned int(8)[6]", "byte[]" },
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
            { "OpusSpecificBox", "OpusSpecificBox" },
            { "unsigned int(8 * OutputChannelCount)", "byte[]" },
            { "ChannelMappingTable", "ChannelMappingTable" },
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
            { "bslbf(header_size * 8)[]", "byte[]" },
            { "bslbf(trailer_size * 8)[]", "byte[]" },
            { "bslbf(aux_size * 8)[]", "byte[]" },
            { "bslbf(11)", "ushort" },
            { "bslbf(5)", "byte" },
            { "bslbf(4)", "byte" },
            { "bslbf(1)", "bool" },
            { "uimsbf(32)", "uint" },
            { "uimsbf(24)", "uint" },
            { "uimsbf(18)", "uint" },
            { "uimsbf(16)", "ushort" },
            { "uimsbf(14)", "ushort" },
            { "uimsbf(12)", "ushort" },
            { "uimsbf(10)", "ushort" },
            { "uimsbf(8)", "byte" },
            { "uimsbf(7)", "byte" },
            { "uimsbf(6)", "byte" },
            { "uimsbf(5)", "byte" },
            { "uimsbf(4)", "byte" },
            { "uimsbf(3)", "byte" },
            { "uimsbf(2)", "byte" },
            { "uimsbf(1)", "bool" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "GASpecificConfig" },
            { "case 8:\r\n      CelpSpecificConfig()", "CelpSpecificConfig" },
            { "case 9:\r\n      HvxcSpecificConfig()", "HvxcSpecificConfig" },
            { "case 12:\r\n      TTSSpecificConfig()", "TTSSpecificConfig" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "StructuredAudioSpecificConfig" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "ErrorResilientCelpSpecificConfig" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "ErrorResilientHvxcSpecificConfig" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "ParametricSpecificConfig" },
            { "case 28:\r\n      SSCSpecificConfig()", "SSCSpecificConfig" },
            { "case 30:\r\n      uimsbf(1)", "bool" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "MPEG_1_2_SpecificConfig" },
            { "case 35:\r\n      DSTSpecificConfig()", "DSTSpecificConfig" },
            { "case 36:\r\n      bslbf(5)", "byte" },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "SLSSpecificConfig" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "ELDSpecificConfig" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "SymbolicMusicSpecificConfig" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "byte" },
            { "GetAudioObjectType()", "GetAudioObjectType" },
            { "SpatialSpecificConfig", "SpatialSpecificConfig" },
            { "ALSSpecificConfig", "ALSSpecificConfig" },
            { "ErrorProtectionSpecificConfig", "ErrorProtectionSpecificConfig" },
            { "program_config_element", "program_config_element" },
            { "byte_alignment", "byte" },
            { "CelpHeader(samplingFrequencyIndex)", "CelpHeader" },
            { "CelpBWSenhHeader", "CelpBWSenhHeader" },
            { "HVXCconfig", "HVXCconfig" },
            { "TTS_Sequence", "TTS_Sequence" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "ER_SC_CelpHeader" },
            { "ErHVXCconfig", "ErHVXCconfig" },
            { "PARAconfig", "PARAconfig" },
            { "HILNenexConfig", "HILNenexConfig" },
            { "HILNconfig", "HILNconfig" },
            { "ld_sbr_header", "ld_sbr_header" },
            { "sbr_header", "sbr_header" },
            { "uimsbf(1)[i]", "bool[]" },
            { "uimsbf(8)[i]", "byte[]" },
            { "bslbf(1)[i]", "byte[]" },
            { "uimsbf(4)[i]", "byte[]" },
            { "uimsbf(1)[c]", "bool[]" },
            { "uimsbf(32)[f]", "uint[]" },
            { "uimsbf(6)[i]", "byte[]" },
            { "uimsbf(1)[i][j]", "bool[][]" },
            { "uimsbf(2)[i][j]", "byte[][]" },
            { "uimsbf(4)[i][j]", "byte[][]" },
            { "uimsbf(16)[i][j]", "ushort[][]" },
            { "uimsbf(7)[i][j]", "byte[][]" },
            { "uimsbf(5)[i][j]", "byte[][]" },
            { "uimsbf(6)[i][j]", "byte[][]" },
            { "CelpHeader", "CelpHeader" },
            { "ER_SC_CelpHeader", "ER_SC_CelpHeader" },
            { "AV1CodecConfigurationBox", "AV1CodecConfigurationBox" },
            { "AV1CodecConfigurationRecord", "AV1CodecConfigurationRecord" },
            { "unsigned int(16)[3]", "ushort[]" },
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
        if (string.IsNullOrEmpty(source) || source.Length < 2)
            return source;
        return string.Format("{0}{1}", char.ToUpper(source[0]), source.Substring(1));
    }
}