using Newtonsoft.Json;
using Pidgin;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.RegularExpressions;
using static Pidgin.Parser<char>;
using static Pidgin.Parser;

namespace ConsoleApp;

#region Parser

public enum ParsedBoxType
{
    Box,
    Entry,
    Descriptor
}

[SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
public interface PseudoCode
{
}


public class PseudoType : PseudoCode
{
    public string Aligned { get; set; }
    public string Template { get; set; }
    public string Cons { get; set; }
    public string Sign { get; set; }
    public string Type { get; set; }
    public string Param { get; set; }

    public PseudoType()
    {

    }

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
    public string FourCC { get; set; }
    public string BoxName { get; set; }
    public string ClassType { get; set; }
    public string Comment { get; set; }
    public string EndComment { get; set; }
    public IList<PseudoCode> Fields { get; set; }
    public string Abstract { get; set; }
    public PseudoExtendedClass Extended { get; set; }
    public string Syntax { get; set; }
    public long CurrentOffset { get; set; }

    public PseudoClass()
    {

    }

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
        BoxName = boxName;
        ClassType = classType.GetValueOrDefault();
        Fields = fields.ToList();
        Abstract = abstrct.GetValueOrDefault();
        Extended = extended.GetValueOrDefault();
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
    {

    }

    public PseudoComment(string comment)
    {
        Comment = comment;
    }

    public string Comment { get; }
}

public class PseudoCase : PseudoCode
{
    public PseudoCase()
    {
        
    }

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
    public PseudoBlock Parent { get; set; }
    public List<PseudoField> RequiresAllocation { get; set; } = new List<PseudoField>();
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

#endregion // Parser

partial class Program
{
    private static Parser<char, string> Identifier => LetterOrDigit.Then(Token(c => char.IsLetterOrDigit(c) || c == '_').ManyString(), (first, rest) => first + rest);
    private static Parser<char, string> IdentifierWithSpace => Token(c => char.IsLetterOrDigit(c) || c == '_' || c == ' ' || c == '©' || c == '-').ManyString();
    private static Parser<char, char> LetterOrDigitOrUnderscore => Token(c => char.IsLetterOrDigit(c) || c == '_');
    private static Parser<char, char> LetterOrDigitOrUnderscoreOrDot => Token(c => char.IsLetterOrDigit(c) || c == '_' || c == '.');
    private static Parser<char, string> Parentheses => Char('(').Then(Rec(() => Expr).Until(Char(')'))).Select(x => $"({string.Concat(x)})");
    private static Parser<char, string> Expr => OneOf(Rec(() => Parentheses), AnyCharE);
    private static Parser<char, string> AnyCharE => AnyCharExcept('(', ')').AtLeastOnceString();

    private static Parser<char, string> BoxName => Identifier;
    private static Parser<char, string> FieldName => Identifier;

    private static Parser<char, string> BoxType => Char('\'').Then(IdentifierWithSpace).Before(Char('\''));
    private static Parser<char, string> OldBoxType => Char('\'').Then(Char('!')).Then(IdentifierWithSpace).Before(Char('\'')).Before(Char(',')).Before(SkipWhitespaces);
    private static Parser<char, string> ClassType => Parentheses;

    private static Parser<char, string> Parameter => SkipWhitespaces.Then(LetterOrDigitOrUnderscore.ManyString());
    private static Parser<char, string> ParameterValue => Char('=').Then(SkipWhitespaces).Then(LetterOrDigit.ManyString());
    private static Parser<char, (string Name, Maybe<string> Value)> ParameterFull => Map((name, value) => (name, value), Parameter.Before(SkipWhitespaces), Try(ParameterValue).Optional());
    private static Parser<char, IEnumerable<(string Name, Maybe<string> Value)>> Parameters => ParameterFull.SeparatedAndOptionallyTerminated(Char(','));

    private static Parser<char, string> TagValue => LetterOrDigitOrUnderscoreOrDot.ManyString();
    private static Parser<char, string> DescriptorTag => SkipWhitespaces.Then(Try(String("ProfileLevelIndicationIndexDescrTag")).Or(String("tag=").Then(TagValue)));

    private static Parser<char, string> FieldArray => Char('[').Then(Any.Until(Char(']'))).Select(x => $"[{string.Concat(x)}]");
    private static Parser<char, IEnumerable<PseudoCode>> SingleBlock => Field.Repeat(1);
    private static Parser<char, PseudoCode> CodeBlock => Try(SwitchBlock).Or(Try(Block).Or(Try(RepeatingBlock).Or(Try(Field).Or(Comment))));
    private static Parser<char, IEnumerable<PseudoCode>> CodeBlocks => SkipWhitespaces.Then(CodeBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));

    private static Parser<char, PseudoType> FieldType =>
        Map((aligned, template, cons, sign, type, param) => new PseudoType(aligned, template, cons, sign, type, param),
            Try(String("aligned").Before(SkipWhitespaces)).Optional(),
            Try(String("template").Before(SkipWhitespaces)).Optional(),
            Try(String("const").Before(SkipWhitespaces)).Optional(),
            Try(OneOf(Try(String("signed")), Try(String("unsigned"))).Before(Try(Char('_')).Optional()).Before(SkipWhitespaces)).Optional(),
            Identifier.Before(SkipWhitespaces),
            Try(Parentheses).Optional()
        );

    private static Parser<char, PseudoCode> Field =>
        Map((type, array, name, value, comment) => new PseudoField(type, array, name, value, comment),
            Try(FieldTypeWorkaround.Select(x => new PseudoType() { Type = x })).Or(FieldType).Before(SkipWhitespaces),
            Try(FieldArray.Before(SkipWhitespaces)).Optional(),
            Try(FieldName.Before(SkipWhitespaces)).Optional(),
            Try(Any.Until(Char(';')).Before(SkipWhitespaces)).Or(Try(Any.Until(Char('\n')).Before(SkipWhitespaces))).Optional(),
            Try(LineComment(String("//"))).Optional()
        ).Select(x => (PseudoCode)x);

    private static Parser<char, PseudoCode> Block =>
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


    private static Parser<char, PseudoCode> CaseBlock => Map((cs, op) => new PseudoCase(cs, op), String("case").Before(SkipWhitespaces), AnyCharExcept(':').AtLeastOnceString().Before(SkipWhitespaces).Before(Char(':')).Before(SkipWhitespaces)).Select(x => (PseudoCode)x);
    private static Parser<char, PseudoCode> DefaultBlock => Map((cs) => new PseudoCase(cs), String("default").Before(SkipWhitespaces).Before(Char(':')).Before(SkipWhitespaces).Select(x => $"{x}:")).Select(x => (PseudoCode)x);
    private static Parser<char, PseudoCode> BreakBlock => Map((cs) => new PseudoCase(cs), String("break").Before(SkipWhitespaces).Before(Char(';')).Before(SkipWhitespaces).Select(x => $"{x}:")).Select(x => (PseudoCode)x);

    private static Parser<char, PseudoCode> SwitchBlock =>
        Map((type, condition, comment, content) => new PseudoBlock(type, condition, comment, content),
            String("switch"),
            SkipWhitespaces.Then(Try(Parentheses).Optional()),
            SkipWhitespaces.Then(Try(LineComment(String("//"))).Or(Try(BlockComment(String("/*"), String("*/")))).Optional()),
            SkipWhitespaces.Then(Try(Rec(() => SwitchCaseBlocks).Between(Char('{'), Char('}'))))
        ).Select(x => (PseudoCode)x);

    private static Parser<char, PseudoCode> SwitchCaseBlock => Try(CaseBlock).Or(Try(DefaultBlock).Or(Try(BreakBlock).Or(Try(Block).Or(Try(Field).Or(Comment)))));

    private static Parser<char, IEnumerable<PseudoCode>> SwitchCaseBlocks => SkipWhitespaces.Then(SwitchCaseBlock.SeparatedAndOptionallyTerminated(SkipWhitespaces));

    private static Parser<char, PseudoCode> RepeatingBlock =>
        Map((content, array) => new PseudoRepeatingBlock(content, array),
            SkipWhitespaces.Then(Try(Rec(() => CodeBlocks).Between(Char('{'), Char('}'))).Or(Try(Rec(() => SingleBlock)))),
            SkipWhitespaces.Then(Char('[')).Then(Any.AtLeastOnceUntil(Char(']')))
        ).Select(x => (PseudoCode)x);

    private static Parser<char, string> MultiBoxType => 
        Map((box, otherBox) => $"'{box}' or '{otherBox}'",
            BoxType.Before(SkipWhitespaces),
            String("or").Then(SkipWhitespaces).Then(BoxType)
        );

    private static Parser<char, PseudoExtendedClass> ExtendedClass =>
        Map((extendedBoxName, oldBoxType, boxType, parameters, descriptorTag) => new PseudoExtendedClass(extendedBoxName, oldBoxType, boxType, parameters, descriptorTag),
            SkipWhitespaces.Then(Try(String("extends")).Optional()).Then(SkipWhitespaces).Then(Try(BoxName).Optional()),
            SkipWhitespaces.Then(Try(Char('(')).Optional()).Then(Try(OldBoxType).Optional()),
            SkipWhitespaces.Then(Try(MultiBoxType).Or(Try(BoxType)).Optional()),
            SkipWhitespaces.Then(Try(Char(',')).Optional()).Then(Try(Parameters).Optional()).Before(Try(Char(')')).Optional()).Before(SkipWhitespaces).Optional(),
            SkipWhitespaces.Then(Try(Char(':').Then(SkipWhitespaces).Then(String("bit(8)")).Then(SkipWhitespaces).Then(DescriptorTag).Before(SkipWhitespaces))).Optional()
        );

    private static Parser<char, PseudoClass> Box =>
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
    private static Parser<char, IEnumerable<PseudoClass>> Boxes => SkipWhitespaces.Then(Box.SeparatedAndOptionallyTerminated(SkipWhitespaces));

    private static Parser<char, PseudoCode> Comment =>
        Map((comment) => new PseudoComment(comment),
            Try(LineComment(String("//"))).Or(BlockComment(String("/*"), String("*/")))
        ).Select(x => (PseudoCode)x);

    private static Parser<char, string> LineComment<T>(Parser<char, T> lineCommentStart)
    {
        if (lineCommentStart == null)
            throw new ArgumentNullException(nameof(lineCommentStart));
        Parser<char, Unit> eol = Try(EndOfLine).IgnoreResult();
        return lineCommentStart.Then(Any.Until(End.Or(eol)), (first, rest) => string.Concat(rest));
    }

    private static Parser<char, string> BlockComment<T, U>(Parser<char, T> blockCommentStart, Parser<char, U> blockCommentEnd)
    {
        if (blockCommentStart == null)
            throw new ArgumentNullException(nameof(blockCommentStart));
        if (blockCommentEnd == null)
            throw new ArgumentNullException(nameof(blockCommentEnd));
        return blockCommentStart.Then(Any.Until(blockCommentEnd), (first, rest) => string.Concat(rest));
    }

    private static Parser<char, string> FieldTypeWorkaround =>
        OneOf(
            Try(String("int downmix_instructions_count = 1")), // WORKAROUND
            Try(String("int i, j")), // WORKAROUND
            Try(String("int i,j")), // WORKAROUND
            Try(String("int i")), // WORKAROUND
            Try(String("int size = 4")), // WORKAROUND
            Try(String("sizeOfInstance = sizeOfInstance<<7 | sizeByte")), // WORKAROUND
            Try(String("int sizeOfInstance = 0")), // WORKAROUND
            Try(String("size += 5")), // WORKAROUND
            Try(String("j=1")), // WORKAROUND
            Try(String("j++")), // WORKAROUND
            Try(String("subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8")), // WORKAROUND
            Try(String("totalPatternLength = 0")), // WORKAROUND
            Try(String("samplerate = samplerate >> 16")), // WORKAROUND
            Try(String("sbrPresentFlag = -1")), // WORKAROUND
            Try(String("psPresentFlag = -1")), // WORKAROUND
            Try(String("extensionAudioObjectType = 0")), // WORKAROUND
            Try(String("extensionAudioObjectType = 5")), // WORKAROUND
            Try(String("sbrPresentFlag = 1")), // WORKAROUND
            Try(String("psPresentFlag = 1")), // WORKAROUND
            Try(String("break")),
            Try(String("audioObjectType = 32 + audioObjectTypeExt")),
            Try(String("return audioObjectType")),
            Try(String("len = eldExtLen")),
            Try(String("len += eldExtLenAddAdd")),
            Try(String("len += eldExtLenAdd")),
            Try(String("FieldLength = (large_size + 1) * 16")), // WORKAROUND
            Try(String("numSbrHeader = 1")), // WORKAROUND
            Try(String("numSbrHeader = 2")), // WORKAROUND
            Try(String("numSbrHeader = 3")), // WORKAROUND
            Try(String("numSbrHeader = 4")), // WORKAROUND
            Try(String("numSbrHeader = 0")) // WORKAROUND
            );

    static void Main(string[] args)
    {
        //var jds = File.ReadAllText("14496-3-added.js");
        //var test = Boxes.ParseOrThrow(jds);

        string[] jsonFiles = {
            "14496-23-boxes-added.json",
            "14496-12-boxes-added.json",
            "14496-15-boxes-added.json",
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
            "14496-1-boxes-added.json",
            "14496-3-boxes-added.json",
            "Opus-boxes.json",
            "AV1-boxes.json",
            "AVIF-boxes.json",
            "Added-boxes.json",
        };

        int success = 0;
        int duplicated = 0;
        int fail = 0;

        Dictionary<string, PseudoClass> ret = new Dictionary<string, PseudoClass>();
        List<PseudoClass> duplicates = new List<PseudoClass>();
        List<string> containers = new List<string>();

        foreach (var file in jsonFiles)
        {
            ParsedBoxType parsedBoxType = ParsedBoxType.Entry;
            if (file.Contains("-boxes") || file.Contains("-properties"))
                parsedBoxType = ParsedBoxType.Box;

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
                            string[] ignoredBoxes = [
                                    "IncompleteAVCSampleEntry",
                                    "HEVCSampleEntry",
                                    "LHEVCSampleEntry",
                                    "AVCParameterSampleEntry",
                                    "AVCSampleEntry",
                                    "AVC2SampleEntry",
                                    "MVCSampleEntry",
                                    "MVCDSampleEntry",
                                    "A3DSampleEntry",
                                    "SVCSampleEntry",
                                    "HEVCTileSampleEntry",
                                    "LHEVCTileSampleEntry",
                                    "HEVCTileSSHInfoSampleEntry",
                                    "HEVCSliceSegmentDataSampleEntry",
                                    "VvcSampleEntry",
                                    "VvcSubpicSampleEntry",
                                    "VvcNonVCLSampleEntry",
                                    "EVCSampleEntry",
                                    "EVCSliceComponentTrackSampleEntry",
                                    "AV1SampleEntry",
                                    "AVC2MVCSampleEntry",
                                    "AVC2SVCSampleEntry",
                                    "AVCMVCSampleEntry",
                                    "AVCSVCSampleEntry",
                                    "HEVCLHVCSampleEntry",
                                    "OpusSampleEntry",
                                ];

                            if (ignoredBoxes.Contains(item.BoxName))
                                continue;

                            if (item.BoxName == "AutoExposureBracketingEntry" ||
                                item.BoxName == "FlashExposureBracketingEntry" ||
                                item.BoxName == "AV1ForwardKeyFrameSampleGroupEntry" ||
                                item.BoxName == "AV1MetadataSampleGroupEntry" ||
                                item.BoxName == "AV1SwitchFrameSampleGroupEntry" ||
                                item.BoxName == "AVCSubSequenceEntry" ||
                                item.BoxName == "DepthOfFieldBracketingEntry" ||
                                item.BoxName == "FDItemInfoExtension" ||
                                item.BoxName == "FocusBracketingEntry" ||
                                item.BoxName == "PanoramaEntry" ||
                                item.BoxName == "WhiteBalanceBracketingEntry")
                            {
                                item.ParsedBoxType = ParsedBoxType.Entry;
                            }
                            else if(
                                item.BoxName == "SubpicCommonGroupBox" ||
                                item.BoxName == "TrackGroupTypeBox_alte" ||
                                item.BoxName == "SubpicMultipleGroupsBox" ||
                                item.BoxName == "TrackGroupTypeBox_cstg" ||
                                item.BoxName == "TrackGroupTypeBox" ||
                                item.BoxName == "OperatingPointGroupBox" ||
                                item.BoxName == "TrackGroupTypeBox_snut" ||
                                item.BoxName == "StereoVideoGroupBox" ||
                                item.BoxName == "SwitchableTracks" ||
                                item.BoxName == "EntityToGroupBox_vvcb"
                                )
                            {
                                item.ParsedBoxType = ParsedBoxType.Box;
                            }
                            else
                            {
                                item.ParsedBoxType = parsedBoxType;
                            }

                            if(item.Extended != null && !string.IsNullOrEmpty(item.Extended.DescriptorTag))
                            {
                                item.ParsedBoxType = ParsedBoxType.Descriptor;
                            }

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
        string js = JsonConvert.SerializeObject(ret.Values.ToArray());
        File.WriteAllText("result.json", js);

        string resultCode = 
@"using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpMP4
{
";
        // build box factory
        string factory =
@"   public class BoxFactory
    {
        public static Box CreateBox(string fourCC, string parent, byte[] uuid = null)
        {
            if (uuid != null) fourCC = $""{fourCC} {Convert.ToHexString(uuid).ToLowerInvariant()}"";

            switch(fourCC)
            {
               ";

        SortedDictionary<string, List<PseudoClass>> fourccBoxes = new SortedDictionary<string, List<PseudoClass>>();
        SortedDictionary<string, List<PseudoClass>> fourccEntries = new SortedDictionary<string, List<PseudoClass>>();
        foreach (var item in ret)
        {
            if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.BoxType))
            {
                if (item.Value.ParsedBoxType == ParsedBoxType.Box) 
                {
                    if (!fourccBoxes.ContainsKey(item.Value.Extended.BoxType))
                        fourccBoxes.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                    else
                        fourccBoxes[item.Value.Extended.BoxType].Add(item.Value);
                }
                else
                {
                    if (!fourccEntries.ContainsKey(item.Value.Extended.BoxType))
                        fourccEntries.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                    else
                        fourccEntries[item.Value.Extended.BoxType].Add(item.Value);
                }
            }
        }

        // add additional boxes
        string[] audioSampleEntryTypes = new string[]
        { 
            "samr","sawb","mp4a","drms","alac","owma","ac-3","ec-3","mlpa","dtsl","dtsh","dtse","Opus","enca","resa","sevc","sqcp","ssmv","lpcm","dtsc","sowt",
        };
        foreach(var type in audioSampleEntryTypes)
        {
            if (!fourccBoxes.ContainsKey(type))
                fourccBoxes.Add(type, new List<PseudoClass>() { ret.First(x => x.Value.BoxName == "AudioSampleEntry").Value });
        }
        string[] visualSampleEntryTypes = new string[]
        {
            "mp4v","s263","drmi","encv","resv","icpv","hvc1","hvc2","hvc3","lhv1","lhe1","hev1","hev2","hev3","avcp","mvc1","mvc2","mvc3","mvc4","mvd1","mvd2",
            "mvd3","mvd4","a3d1","a3d2","a3d3","a3d4","svc1","svc2","hvt1","lht1","hvt3","hvt2","vvc1","vvi1","vvs1","vvcN","evc1","evs1","evs2","av01","avc1",
            "avc2","avc3","avc4","vp08","vp09","vp10","apcn","dvhe","dvav",
        };
        foreach (var type in visualSampleEntryTypes)
        {
            if (!fourccBoxes.ContainsKey(type))
                fourccBoxes.Add(type, new List<PseudoClass>() { ret.First(x => x.Value.BoxName == "VisualSampleEntry").Value });
        }
        if (!fourccBoxes.ContainsKey("mp4s"))
            fourccBoxes.Add("mp4s", new List<PseudoClass>() { ret.First(x => x.Value.BoxName == "MpegSampleEntry").Value });
        if (!fourccBoxes.ContainsKey("dimg"))
            fourccBoxes.Add("dimg", new List<PseudoClass>() { ret.First(x => x.Value.BoxName == "SingleItemTypeReferenceBox").Value });
        if (!fourccBoxes.ContainsKey("cdsc"))
            fourccBoxes.Add("cdsc", new List<PseudoClass>() { ret.First(x => x.Value.BoxName == "TrackReferenceTypeBox").Value });

        foreach (var item in fourccBoxes)
        {
            string optCondition = "";

            if (item.Value.Count == 1)
            {
                if (item.Key == "uuid")
                {
                    factory += $"               case \"{item.Key}\": return new UserBox(uuid);\r\n";
                }
                else
                {
                    string comment = "";
                    if (item.Value.Single().BoxName.Contains('_'))
                        comment = " // TODO: fix duplicate";
                    string optParams = "";
                    if (item.Value.Single().BoxName == "AudioSampleEntry" || item.Value.Single().BoxName == "VisualSampleEntry" ||
                        item.Value.Single().BoxName == "SingleItemTypeReferenceBox" || item.Value.Single().BoxName == "SingleItemTypeReferenceBoxLarge" ||
                        item.Value.Single().BoxName == "TrackReferenceTypeBox")
                        optParams = $"\"{item.Key}\"";

                    // for instance "mp4a" box can be also under the "wave" box where it has a different syntax
                    if (item.Value.Single().BoxName == "AudioSampleEntry" || item.Value.Single().BoxName == "VisualSampleEntry" || item.Value.Single().BoxName == "MpegSampleEntry")
                        optCondition = "if(parent == \"stsd\") ";

                    factory += $"               case \"{item.Key}\": {optCondition} return new {item.Value.Single().BoxName}({optParams});{(optCondition != "" ? "break;" : "")}{comment}\r\n";
                }
            }
            else
            {
                if (
                    item.Value.First().BoxName == "MovieBox" ||
                    item.Value.First().BoxName == "MovieFragmentBox" ||
                    item.Value.First().BoxName == "SegmentIndexBox" ||
                    item.Value.First().BoxName == "trackhintinformation" ||
                    item.Value.First().BoxName == "ViewPriorityBox" ||
                    item.Value.First().BoxName == "rtpmoviehintinformation" ||
                    item.Value.First().BoxName == "AppleName2Box"
                    )
                {
                    string comment = $" // TODO: box is ambiguous in between {string.Join(" and ", item.Value.Select(x => x.BoxName))}";
                    factory += $"               case \"{item.Key}\": return new {item.Value.First().BoxName}();{comment}\r\n";
                }
                else if (item.Key == "cprt")
                {
                    factory += $"               case \"{item.Key}\": if(parent == \"ilst\") return new AppleCopyrightBox(); else return new CopyrightBox();\r\n";
                }
                else if (item.Value.First().BoxName == "TextMediaBox")
                {
                    factory += $"               case \"{item.Key}\": if(parent == \"gmhd\") return new TextGmhdMediaBox(); else if(parent == \"stsd\") return new TextMediaBox(); break;\r\n";
                }
                else
                {
                    factory += $"               case \"{item.Key}\": throw new NotSupportedException($\"\'{item.Key}\' under \'{{parent}}\' is ambiguous in between {string.Join(" and ", item.Value.Select(x => x.BoxName))}\");\r\n";
                }
            }
        }

        factory +=
@"          }

            if(parent == ""ilst"")
            {
                if (fourCC[0] == '\0') return new IlstKey(fourCC);
            }
            else if(uuid != null)
            {
                Log.Debug($""Unknown 'uuid' box: '{fourCC}'"");
                return new UserBox(uuid);
            }

            //throw new NotImplementedException(fourCC);
            Log.Debug($""Unknown box: '{fourCC}'"");
            return new UnknownBox(fourCC);
        }";


        factory += @"
        public static IMp4Serializable CreateEntry(string fourCC)
        {
            switch(fourCC)
            {
               ";
        foreach (var item in fourccEntries)
        {
            if (item.Value.Count == 1)
            {
                string comment = "";
                if (item.Value.Single().BoxName.Contains('_'))
                    comment = " // TODO: fix duplicate";
                string optParams = "";
                if (item.Value.Single().BoxName == "AudioSampleEntry" || item.Value.Single().BoxName == "VisualSampleEntry" ||
                    item.Value.Single().BoxName == "SingleItemTypeReferenceBox" || item.Value.Single().BoxName == "SingleItemTypeReferenceBoxLarge" ||
                    item.Value.Single().BoxName == "TrackReferenceTypeBox")
                    optParams = $"\"{item.Key}\"";
                factory += $"               case \"{item.Key}\": return new {item.Value.Single().BoxName}({optParams});{comment}\r\n";
            }
            else
            {
                if (
                    item.Value.First().BoxName == "MovieBox" ||
                    item.Value.First().BoxName == "MovieFragmentBox" ||
                    item.Value.First().BoxName == "SegmentIndexBox" ||
                    item.Value.First().BoxName == "trackhintinformation" ||
                    item.Value.First().BoxName == "ViewPriorityBox" ||
                    item.Value.First().BoxName == "rtpmoviehintinformation" 
                    )
                {
                    string comment = $" // TODO: box is ambiguous in between {string.Join(" and ", item.Value.Select(x => x.BoxName))}";
                    factory += $"               case \"{item.Key}\": return new {item.Value.First().BoxName}();{comment}\r\n";
                }
                else
                {
                    factory += $"               case \"{item.Key}\": throw new NotSupportedException(\"\'{item.Key}\' is ambiguous in between {string.Join(" and ", item.Value.Select(x => x.BoxName))}\");\r\n";
                }
            }
        }

        factory +=
@"          }

            //throw new NotImplementedException(fourCC);
            Log.Debug($""Unknown entry: '{fourCC}'"");
            return new UnknownEntry(fourCC);
        }
";

        factory += @"
        public static Descriptor CreateDescriptor(byte tag)
        {
            switch (tag)
            {
";
        SortedDictionary<string, List<PseudoClass>> descriptors = new SortedDictionary<string, List<PseudoClass>>();
        foreach (var item in ret)
        {
            if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.DescriptorTag) && string.IsNullOrEmpty(item.Value.Abstract))
            {
                if (!descriptors.ContainsKey(item.Value.Extended.DescriptorTag))
                    descriptors.Add(item.Value.Extended.DescriptorTag, new List<PseudoClass>() { item.Value });
                else
                    descriptors[item.Value.Extended.DescriptorTag].Add(item.Value);
            }
        }

        foreach(var item in descriptors)
        {
            if (!item.Key.StartsWith("0x"))
            {
                string key = "DescriptorTags." + item.Key;
                if (item.Key == "DecSpecificInfoTag")
                {
                    factory += $"               case {key}: return new GenericDecoderSpecificInfo(); // TODO: choose the specific descriptor\r\n";
                }
                else
                {
                    factory += $"               case {key}: return new {item.Value.Single().BoxName}();\r\n";
                }
            }
        }

        factory +=
@"          }

            //throw new NotImplementedException($""Unknown descriptor: 'tag'"");
            Log.Debug($""Unknown descriptor: '{tag}'"");
            return new UnknownDescriptor(tag);";

        factory += @"
        }
";

        factory += 
@"
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
        if (!string.IsNullOrEmpty(b.Abstract))
        {
            optAbstract = "abstract ";
        }

        cls += $"/*\r\n{b.Syntax.Replace("*/", "*//*")}\r\n*/\r\n";

        cls += @$"public {optAbstract}class {b.BoxName}";
        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName))
        {
            cls += $" : {b.Extended.BoxName}\r\n{{\r\n";
        }
        else
        {
            if (b.BoxName == "BaseDescriptor" || b.BoxName == "QoS_Qualifier")
                cls += $" : Descriptor\r\n{{\r\n";
            else
            {
                string inType = "IMp4Serializable";
                if(b.BoxName == "SampleGroupDescriptionEntry")
                {
                    inType = "IHasBoxChildren";
                }
                cls += $" : {inType}\r\n{{\r\n\t\tpublic StreamMarker Padding {{ get; set; }}\r\n\t\tpublic IMp4Serializable Parent {{ get; set; }}\r\n";
            }
        }

        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrWhiteSpace(b.Extended.BoxType))
        {
            // UUID in case the box is a UserBox
            if(b.Extended.BoxType.Length > 4)
            {
                string[] parts = b.Extended.BoxType.Split(' ');
                b.Extended.BoxType = parts[0];
                b.Extended.UserType = parts[1];
            }

            cls += $"\tpublic const string TYPE = \"{b.Extended.BoxType}\";\r\n";
        }
        else if(b.Extended != null && !string.IsNullOrEmpty(b.Extended.DescriptorTag))
        {
            if (!b.Extended.DescriptorTag.Contains(".."))
            {
                string tag = b.Extended.DescriptorTag;
                if(!tag.StartsWith("0"))
                {
                    tag = "DescriptorTags." + tag;
                }
                cls += $"\tpublic const byte TYPE = {tag};\r\n";
            }
            else
            {
                string[] tags = b.Extended.DescriptorTag.Split("..");
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

        string ov = ((b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName)) || (b.BoxName == "BaseDescriptor" || b.BoxName == "QoS_Qualifier")) ? "override " : "virtual ";
        cls += $"\tpublic {ov}string DisplayName {{ get {{ return \"{b.BoxName}\"; }} }}";

        string ctorContent = "";
        var fields = FlattenFields(b.Fields);

        if (b.BoxName == "GASpecificConfig" || b.BoxName == "SLSSpecificConfig")
        {
            fields.Add(new PseudoField() { Name = "audioObjectType", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
            fields.Add(new PseudoField() { Name = "channelConfiguration", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
            fields.Add(new PseudoField() { Name = "samplingFrequencyIndex", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
            ctorContent = "\t\tthis.audioObjectType = audioObjectType;\r\n\t\tthis.channelConfiguration = channelConfiguration;\r\n\t\tthis.samplingFrequencyIndex = samplingFrequencyIndex;\r\n";
        }
        else if (b.BoxName == "CelpSpecificConfig" || b.BoxName == "ER_SC_CelpHeader" || b.BoxName == "ErrorResilientCelpSpecificConfig")
        {
            fields.Add(new PseudoField() { Name = "samplingFrequencyIndex", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
            ctorContent = "\t\tthis.samplingFrequencyIndex = samplingFrequencyIndex;\r\n";
        }
        else if(b.BoxName == "SSCSpecificConfig" || b.BoxName == "ld_sbr_header" || b.BoxName == "ELDSpecificConfig")
        {
            fields.Add(new PseudoField() { Name = "channelConfiguration", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
            ctorContent = "\t\tthis.channelConfiguration = channelConfiguration;\r\n";
        }
        else if (b.BoxName == "VvcPTLRecord")
        {
            fields.Add(new PseudoField() { Name = "num_sublayers", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
            ctorContent = "\t\tthis.num_sublayers = num_sublayers;\r\n";
        }
        else if (b.BoxName == "ChannelMappingTable")
        {
            fields.Add(new PseudoField() { Name = "OutputChannelCount", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
            ctorContent = "\t\tthis.OutputChannelCount = OutputChannelCount;\r\n";
        }
        else if(b.BoxName == "BoxHeader")
        {
            ctorContent = "\t\tthis.type = IsoStream.FromFourCC(boxtype);\r\n\t\tthis.usertype = extended_type;\r\n";
        }
        else if(b.BoxName == "SampleEncryptionSample")
        {
            fields.Add(new PseudoField() { Name = "version", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
            fields.Add(new PseudoField() { Name = "flags", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(32)")) });
            fields.Add(new PseudoField() { Name = "Per_Sample_IV_Size", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
            ctorContent = "\t\tthis.version = version;\r\n\t\tthis.flags = flags;\r\n\t\tthis.Per_Sample_IV_Size = Per_Sample_IV_Size;\r\n";
        }
        else if(b.BoxName == "SampleEncryptionSubsample")
        {
            fields.Add(new PseudoField() { Name = "version", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
            ctorContent = "\t\tthis.version = version;\r\n";
        }
        else if(b.BoxName == "SampleEncryptionBox")
        {
            fields.Add(new PseudoField() { Name = "Per_Sample_IV_Size", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")), Value = " = 16; // TODO: get from the 'tenc' box" });
        }
        else if(b.BoxName == "SampleGroupDescriptionEntry")
        {
            fields.Add(new PseudoField() { Name = "children", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), "Box()", new Maybe<string>()), FieldArray = "[]" });
        }
        else if(b.BoxName == "FullBox")
        {
            ctorContent = "\t\tthis.version = v;\r\n\t\t this.flags = f;";
        }

        bool hasBoxes = (fields.Select(x => GetReadMethod(GetFieldType(x)).Contains("ReadBox(")).FirstOrDefault(x => x == true) != false && b.BoxName != "MetaDataAccessUnit" && b.BoxName != "ItemReferenceBox") || b.Syntax.Contains("other boxes from derived specifications");
        bool hasDescriptors = fields.Select(x => GetReadMethod(GetFieldType(x)).Contains("ReadDescriptor(")).FirstOrDefault(x => x == true) != false && b.BoxName != "ESDBox" && b.BoxName != "MpegSampleEntry" && b.BoxName != "MPEG4ExtensionDescriptorsBox" && b.BoxName != "AppleInitialObjectDescriptorBox" && b.BoxName != "IPMPControlBox" && b.BoxName != "IPMPInfoBox";

        foreach (var field in fields)
        {
            cls += "\r\n" + BuildField(b, field);
        }

        if(b.BoxName == "TrackRunBox")
        {
            cls += "\r\n" + "protected List<TrunEntry> entries;  \r\n        public List<TrunEntry> Entries { get { return this.entries; } set { this.entries = value; } }";
        }
        else if(b.BoxName == "MetaBox")
        {
            // This is very badly documented, but in case the Apple "ilst" box appears in "meta" inside "trak" or "udta", then the 
            //  serialization is incompatible with ISOBMFF as it uses the "Quicktime atom format".
            //  see: https://www.academia.edu/66625880/Forensic_Analysis_of_Video_Files_Using_Metadata
            //  see: https://web.archive.org/web/20220126080109/https://leo-van-stee.github.io/
            // TODO: maybe instead of lookahead, we just have to check the parents
            cls += "\r\npublic bool IsQuickTime { get { return (Parent != null && (((Box)Parent).FourCC == \"udta\" || ((Box)Parent).FourCC == \"trak\")); } }";
        }
        else if(b.BoxName == "AVCDecoderConfigurationRecord")
        {
            cls += "\r\npublic bool HasExtensions { get; set; } = false;";
        }
        else if(b.BoxName == "FullBox")
        {
            cls += "\r\npublic FullBox(string boxtype, byte[] uuid) : base(boxtype, uuid) { }\r\n";
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
            if (b.Extended.BoxName != null && b.Extended.Parameters != null)
            {
                base4ccparams = string.Join(", ", b.Extended.Parameters.Select(x => string.IsNullOrEmpty(x.Value) ? x.Name : x.Value));
            }
            else if(b.BoxName == "UserBox")
            {
                base4ccparams = "uuid";
            }
            else if(!string.IsNullOrEmpty(b.Extended.UserType))
            {
                base4ccparams = $"Convert.FromHexString(\"{b.Extended.UserType}\")";
            }
            else if (b.Extended != null && !string.IsNullOrEmpty(b.Extended.DescriptorTag))
            {
                if (!b.Extended.DescriptorTag.Contains(".."))
                {
                    string tag = b.Extended.DescriptorTag;
                    if (!tag.StartsWith("0"))
                    {
                        tag = "DescriptorTags." + tag;
                    }

                    if (b.BoxName == "BaseDescriptor")
                    {
                        ctorParams = "byte tag";
                        base4ccparams = "tag";
                    }
                    else if(b.Extended.BoxName != "DecoderSpecificInfo")
                    {
                        base4ccparams = tag;
                    }
                }
                else
                {
                    base4ccparams = "tag";
                    ctorParams = "byte tag";
                }
            }

            string base4ccseparator = "";
            if (!string.IsNullOrEmpty(base4cc) && !string.IsNullOrEmpty(base4ccparams))
                base4ccseparator = ", ";
            baseCtorParams = $": base({base4cc}{base4ccseparator}{base4ccparams})";
        }

        cls += $"\r\n\r\n\tpublic {b.BoxName}({ctorParams}){baseCtorParams}\r\n\t{{\r\n";
        cls += ctorContent;
        cls += $"\t}}\r\n";

        bool shouldOverride = (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName)) || b.BoxName == "BaseDescriptor" || b.BoxName == "QoS_Qualifier";
        cls += "\r\n\tpublic " + (shouldOverride ? "override " : "virtual ") + "ulong Read(IsoStream stream, ulong readSize)\r\n\t{\r\n\t\tulong boxSize = 0;";


        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName))
        {
            string baseRead = "\r\n\t\tboxSize += base.Read(stream, readSize);";
            if (b.BoxName == "MetaBox")
            {
                baseRead = "\r\n\t\tif(IsQuickTime) boxSize += base.Read(stream, readSize);";
            }
            cls += baseRead;
        }

        cls = FixMissingMethodCode(b, cls, MethodType.Read);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(b, null, field, 2, MethodType.Read);
        }

        if ((string.IsNullOrWhiteSpace(b.Abstract) && (containers.Contains(b.FourCC) || containers.Contains(b.BoxName) || hasBoxes)) && 
            b.BoxName != "SampleGroupDescriptionBox" && b.BoxName != "SampleGroupDescriptionEntry")
        {
            cls += "\r\n" + "boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);";
        }
        else if(hasDescriptors)
        {
            string objectTypeIndication = "";
            if (b.BoxName == "DecoderConfigDescriptor")
                objectTypeIndication = ", objectTypeIndication";
            cls += "\r\n" + $"boxSize += stream.ReadDescriptorsTillEnd(boxSize, readSize, this{objectTypeIndication});";
        }

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";


        cls += "\r\n\tpublic " + (shouldOverride ? "override " : "virtual ") + "ulong Write(IsoStream stream)\r\n\t{\r\n\t\tulong boxSize = 0;";

        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName))
        {
            string baseWrite = "\r\n\t\tboxSize += base.Write(stream);";
            if (b.BoxName == "MetaBox")
            {
                baseWrite = "\r\n\t\tif(IsQuickTime) boxSize += base.Write(stream);";
            }
            cls += baseWrite;
        }

        cls = FixMissingMethodCode(b, cls, MethodType.Write);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(b, null, field, 2, MethodType.Write);
        }

        if ((string.IsNullOrWhiteSpace(b.Abstract) && (containers.Contains(b.FourCC) || containers.Contains(b.BoxName) || hasBoxes)) &&
            b.BoxName != "SampleGroupDescriptionBox" && b.BoxName != "SampleGroupDescriptionEntry")
        {
            cls += "\r\n" + "boxSize += stream.WriteBoxArrayTillEnd(this);";
        }
        else if (hasDescriptors)
        {
            string objectTypeIndication = "";
            if (b.BoxName == "DecoderConfigDescriptor")
                objectTypeIndication = ", objectTypeIndication";
            cls += "\r\n" + $"boxSize += stream.WriteDescriptorsTillEnd(this{objectTypeIndication});";
        }

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";

        cls += "\r\n\tpublic " + (shouldOverride ? "override " : "virtual ") + "ulong CalculateSize()\r\n\t{\r\n\t\tulong boxSize = 0;";

        if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName))
        {
            string baseSize = "\r\n\t\tboxSize += base.CalculateSize();";
            if (b.BoxName == "MetaBox")
            {
                baseSize = "\r\n\t\tif(IsQuickTime) boxSize += base.CalculateSize();";
            }
            cls += baseSize;
        }

        cls = FixMissingMethodCode(b, cls, MethodType.Size);

        foreach (var field in b.Fields)
        {
            cls += "\r\n" + BuildMethodCode(b, null, field, 2, MethodType.Size);
        }

        if ((string.IsNullOrWhiteSpace(b.Abstract) && (containers.Contains(b.FourCC) || containers.Contains(b.BoxName) || hasBoxes)) &&
            b.BoxName != "SampleGroupDescriptionBox" && b.BoxName != "SampleGroupDescriptionEntry")
        {
            cls += "\r\n" + "boxSize += IsoStream.CalculateBoxArray(this);";
        }
        else if (hasDescriptors)
        {
            string objectTypeIndication = "";
            if (b.BoxName == "DecoderConfigDescriptor")
                objectTypeIndication = ", objectTypeIndication";
            cls += "\r\n" + $"boxSize += IsoStream.CalculateDescriptors(this{objectTypeIndication});";
        }

        cls += "\r\n\t\treturn boxSize;\r\n\t}\r\n";

        // end of class
        cls += "}\r\n";

        return cls;
    }

    private static string GetFieldType(PseudoField x)
    {
        if (x == null)
            return null;

        if (string.IsNullOrEmpty(x.FieldArray))
            return x.Type.ToString();
        else
            return $"{x.Type}{x.FieldArray}";
    }

    private static string GetCtorParams(string classType, IList<(string Name, string Value)> parameters)
    {
        if (!string.IsNullOrEmpty(classType) && classType != "()")
        {
            Dictionary<string, string> map = new Dictionary<string, string>() {
            { "(unsigned int(32) format)",          "string format" },
            { "(bit(24) flags)",                    "uint flags = 0" },
            { "(fmt)",                              "string fmt = \"\"" },
            { "(codingname)",                       "string codingname = \"\"" },
            { "(handler_type)",                     "string handler_type = \"\"" },
            { "(referenceType)",                    "string referenceType" },
            { "(unsigned int(32) reference_type)",  "string reference_type" },
            { "(grouping_type, version, flags)",    "string grouping_type, byte version, uint flags" },
            { "('snut')",                           "string boxtype = \"snut\"" },
            { "('msrc')",                           "string boxtype = \"msrc\"" },
            { "('cstg')",                           "string boxtype = \"cstg\"" },
            { "('alte')",                           "string boxtype = \"alte\"" },
            { "(name)",                             "string name" },
            { "(uuid)",                             "byte[] uuid" },
            { "(property_type)",                    "string property_type" },
            { "(channelConfiguration)",             "int channelConfiguration" },
            { "(num_sublayers)",                    "byte num_sublayers" },
            { "(code)",                             "string code" },
            { "(property_type, version, flags)",    "string property_type, byte version, uint flags" },
            { "(samplingFrequencyIndex, channelConfiguration, audioObjectType)", "int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType" },
            { "(samplingFrequencyIndex,\r\n  channelConfiguration,\r\n  audioObjectType)", "int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType" },
            { "(unsigned int(32) extension_type)",  "string extension_type" },
            { "('vvcb', version, flags)",           "byte version = 0, uint flags = 0" },
            { "(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)", "string boxtype = \"\", byte[] extended_type = null" },
            { "(unsigned int(32) grouping_type)",   "string grouping_type" },
            { "(unsigned int(32) boxtype, unsigned int(8) v, bit(24) f)", "string boxtype, byte v = 0, uint f = 0" },
            { "(unsigned int(8) OutputChannelCount)", "byte OutputChannelCount" },
            { "(entry_type, bit(24) flags)",        "string entry_type, uint flags" },
            { "(samplingFrequencyIndex)",           "int samplingFrequencyIndex" },
            { "(version, flags, Per_Sample_IV_Size)",  "byte version, uint flags, byte Per_Sample_IV_Size" },
            { "(version)",                          "byte version" },
            };
            return map[classType];
        }
        else if(parameters != null)
        {
            string joinedParams = string.Join(", ", parameters.Select(x => x.Name + (string.IsNullOrEmpty(x.Value) ? "" : " = " + x.Value)));
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "loudnessType",           "string loudnessType" },
                { "local_key_id",           "string local_key_id" },
                { "protocol",               "string protocol" },
                { "0, 0",                   "" },
                { "size",                   "ulong size = 0" },
                { "type",                   "string type" },
                { "version = 0, flags",     "uint flags = 0" },
                { "version = 0, 0",         "" },
                { "version = 0, 1",         "" },
                { "version, 0",             "byte version = 0" },
                { "version, flags = 0",     "byte version = 0" },
                { "version",                "byte version" },
                { "version = 0, flags = 0", "" },
                { "version, flags",         "byte version = 0, uint flags = 0" },
                { "0, tf_flags",            "uint tf_flags = 0" },
                { "0, flags",               "uint flags = 0" },
                { "version, tr_flags",      "byte version = 0, uint tr_flags = 0" },
            };
            return map[joinedParams];
        }
        else
        {
            return "";
        }
    }

    private static string FixMissingMethodCode(PseudoClass? box, string cls, MethodType methodType)
    {
        if(box.BoxName == "SampleDependencyBox")
        {
            cls += "\r\n\t\tint sample_count = 0; // TODO: taken from the stsz sample_count\r\n";
        }
        if (box.BoxName == "SampleDependencyTypeBox")
        {
            if(methodType == MethodType.Read)
                cls += "\r\n\t\tint sample_count = (int)((readSize - boxSize) >> 3); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 1 byte per sample\r\n";
            else
                cls += "\r\n\t\tint sample_count = is_leading.Length;\r\n";
        }
        if (box.BoxName == "DegradationPriorityBox")
        {
            if (methodType == MethodType.Read)
                cls += "\r\n\t\tint sample_count = (int)((readSize - boxSize) >> 4); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 2 bytes per sample\r\n";
            else
                cls += "\r\n\t\tint sample_count = priority.Length;\r\n";
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
                
        if (box.BoxName == "VvcDecoderConfigurationRecord")
        {
            cls += "\r\n";
            cls += "\t\tconst int OPI_NUT = 12;\r\n";
            cls += "\t\tconst int DCI_NUT = 13;\r\n";
        }
                
        if(box.BoxName == "ld_sbr_header")
        {
            cls += "\r\n\t\tint numSbrHeader = 0;\r\n";
        }
        
        if(box.BoxName == "ELDSpecificConfig")
        {
            cls += "\r\n\t\tint len = 0;\r\n";
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
        
        return cls;
    }

    private static List<PseudoField> FlattenFields(IEnumerable<PseudoCode> fields, PseudoBlock parent = null)
    {
        Dictionary<string, PseudoField> ret = new Dictionary<string, PseudoField>();
        foreach(var code in fields)
        {
            if (code is PseudoField field)
            {
                field.Parent = parent; // keep track of parent blocks

                if (IsWorkaround(field.Type.Type))
                    continue;

                string value = field.Value;
                string tp = GetFieldType(field);

                if (string.IsNullOrEmpty(tp) && !string.IsNullOrEmpty(field.Name))
                    tp = field.Name?.Replace("[]", "").Replace("()", "");

                if (!string.IsNullOrEmpty(tp))
                {
                    // TODO: incorrectly parsed type
                    if (!string.IsNullOrEmpty(value) && value.StartsWith('['))
                    {
                        field.FieldArray = value;
                    }
                }

                AddAndResolveDuplicates(ret, field);
            }
            else if (code is PseudoBlock block)
            {
                block.Parent = parent; // keep track of parent blocks

                var blockFields = FlattenFields(block.Content, block);
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
            if (name.StartsWith("reserved") || name == "pre_defined" || GetFieldType(field).StartsWith("SingleItemTypeReferenceBox") ||
                (GetFieldType(field) == "signed int(32)" && GetFieldType(ret[name]) == "unsigned int(32)") ||
                field.Name == "min_initial_alt_startup_offset" || field.Name == "target_rate_share") // special case: requires nesting
            {
                int i = 0;
                string updatedName = $"{name}{i}";
                while (!ret.TryAdd(updatedName, field))
                {
                    i++;
                    updatedName = $"{name}{i}";
                }
                field.Name = updatedName;
                //Debug.WriteLine($"-Resolved: {name} => {updatedName}");
            }
            else if(GetFieldType(field) == GetFieldType(ret[name]) && GetNestedInLoop(field) == GetNestedInLoop(ret[name]))
            {
                //Debug.WriteLine($"-Resolved: fields are the same");
            }
            else
            {
                // try to resolve the conflict using the type size
                string type1 = GetCalculateSizeMethod(GetFieldType(field));
                string type2 = GetCalculateSizeMethod(GetFieldType(ret[name]));
                int type1Size;
                if (int.TryParse(type1, out type1Size))
                {
                    int type2Size;
                    if(int.TryParse(type2, out type2Size))
                    {
                        if (type1Size > type2Size)
                            ret[name] = field;
                        if (type1Size != type2Size)
                            return;
                    }
                }

                if(GetFieldType(field) == "unsigned int(64)[ entry_count ]" && GetFieldType(ret[name]) == "unsigned int(32)[ entry_count ]")
                {
                    ret[name] = field;
                    return;
                }
                else if(GetFieldType(field) == "unsigned int(16)[3]" && GetFieldType(ret[name]) == "unsigned int(32)[3]")
                {
                    return;
                }
                else if (GetFieldType(field) == "aligned bit(1)" && GetFieldType(ret[name]) == "bit")
                {
                    return;
                }

                throw new Exception($"---Duplicated fileds: {name} of types: {field.Type}, {ret[name].Type}");                
            }
        }
    }

    private static string BuildField(PseudoClass b, PseudoCode field)
    {
        var block = field as PseudoBlock;
        if (block != null)
        {
            string ret = "";
            foreach(var f in block.Content)
            {
                ret += "\r\n" + BuildField(b, f);
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
        string tp = GetFieldType(field as PseudoField);

        if (string.IsNullOrEmpty(tp) && !string.IsNullOrEmpty((field as PseudoField)?.Name))
            tp = (field as PseudoField)?.Name?.Replace("[]", "").Replace("()", "");

        if (!string.IsNullOrEmpty(tp))
        {
            if (IsWorkaround(GetFieldType(field as PseudoField)))
                return "";
            else
            {
                // TODO: incorrectly parsed type
                if (!string.IsNullOrEmpty(value) && value.StartsWith('['))
                {
                    value = "";
                }

                string tt = GetType(GetFieldType(field as PseudoField));
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

                if(tt == "byte[]" && (b.BoxName == "MediaDataBox" || b.BoxName == "FreeSpaceBox_skip" || b.BoxName == "FreeSpaceBox" || b.BoxName == "ZeroBox"))
                {
                    tt = "StreamMarker";
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

                int nestingLevel = GetNestedInLoop(field);
                if (nestingLevel > 0)
                {
                    string typedef = GetFieldTypeDef(field);
                    nestingLevel = GetNestedInLoopSuffix(field, typedef, out _);

                    AddRequiresAllocation((PseudoField)field);

                    if (nestingLevel > 0)
                    {
                        // change the type
                        for (int i = 0; i < nestingLevel; i++)
                        {
                            tt += "[]";
                        }
                    }

                    if (!string.IsNullOrEmpty(value))
                    {
                        Debug.WriteLine($"Removing default value: {value}");
                        value = ""; // default value can no longer be used
                    }
                }

                string readMethod = GetReadMethod(GetFieldType(field as PseudoField));
                if (((readMethod.Contains("ReadBox(") && b.BoxName != "MetaDataAccessUnit") || (readMethod.Contains("ReadDescriptor(") && b.BoxName != "ESDBox" && b.BoxName != "MpegSampleEntry")) && b.BoxName != "SampleGroupDescriptionBox" && b.BoxName != "SampleGroupDescriptionEntry"
                     && b.BoxName != "ItemReferenceBox" && b.BoxName != "MPEG4ExtensionDescriptorsBox" && b.BoxName != "AppleInitialObjectDescriptorBox" && b.BoxName != "IPMPControlBox" && b.BoxName != "IPMPInfoBox")
                {
                    string suffix = tt.Contains("[]") ? "" : ".FirstOrDefault()";
                    string ttttt = tt.Replace("[]", "");
                    string ttt = tt.Contains("[]") ? ("IEnumerable<" + tt.Replace("[]", "") + ">") : tt;
                    return $"\tpublic {ttt} {propertyName} {{ get {{ return this.children.OfType<{ttttt}>(){suffix}; }} }}";
                }
                else
                {
                    if(b.BoxName == "SampleGroupDescriptionEntry")
                    {
                        if (tt == "Box[]")
                        {
                            tt = "List<Box>";
                            value = "= new List<Box>()";
                        }
                    }

                    return $"\r\n\tprotected {tt} {name}{value}; {comment}\r\n" + // must be "protected", derived classes access base members
                     $"\tpublic {tt} {propertyName} {{ get {{ return this.{name}; }} set {{ this.{name} = value; }} }}";
                }
            }
        }
        else
            return "";
    }

    private static void AddRequiresAllocation(PseudoField field)
    {
        PseudoBlock parent = null;
        parent = field.Parent;
        while (parent != null)
        {
            if (parent.Type == "for")
            {
                // add the allocation above the first for in the hierarchy
                parent.RequiresAllocation.Add(field);
            }

            parent = parent.Parent;
        }
    }

    private static int GetNestedInLoop(PseudoCode code)
    {
        int ret = 0;
        var field = code as PseudoField;
        var block = code as PseudoBlock;
        PseudoBlock parent = null;

        if (field != null)
            parent = field.Parent;

        if (block != null)
            parent = block.Parent;

        while (parent != null)
        {
            if (parent.Type == "for")
                ret++;
            parent = parent.Parent;
        }

        return ret;
    }

    private static int GetNestedInLoopSuffix(PseudoCode code, string currentSuffix, out string result)
    {
        List<string> ret = new List<string>();
        PseudoBlock parent = null;
        var field = code as PseudoField;
        if (field != null)
            parent = field.Parent;
        var block = code as PseudoBlock;
        if (block != null)
            parent = block.Parent;

        while (parent != null)
        {
            if (parent.Type == "for")
            {
                if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i ="))
                    ret.Insert(0, "[i]");
                else if (parent.Condition.Contains("j =") || parent.Condition.Contains("j="))
                    ret.Insert(0, "[j]");
                else if (parent.Condition.Contains("f =") || parent.Condition.Contains("f="))
                    ret.Insert(0, "[f]");
                else if (parent.Condition.Contains("c =") || parent.Condition.Contains("c="))
                    ret.Insert(0, "[c]");
                else if (parent.Condition.Contains("a =") || parent.Condition.Contains("a="))
                    ret.Insert(0, "[a]");
                else if (parent.Condition.Contains("k =") || parent.Condition.Contains("k="))
                    ret.Insert(0, "[k]");
                else if (parent.Condition.Contains("cnt =") || parent.Condition.Contains("cnt="))
                    ret.Insert(0, "[cnt]");
                else if (parent.Condition.Contains("el =") || parent.Condition.Contains("el="))
                    ret.Insert(0, "[el]");
                else
                    throw new Exception();
            }
            parent = parent.Parent;
        }

        foreach(var suffix in ret.ToArray())
        {
            if (!string.IsNullOrEmpty(currentSuffix) && currentSuffix.Replace(" ", "").Contains(suffix))
                ret.Remove(suffix);
        }

        result = string.Concat(ret);
        return ret.Count;
    }

    private static string FixNestedInLoopVariables(PseudoCode code, string condition, string prefix = "", string suffix = "")
    {
        if (string.IsNullOrEmpty(condition))
            return condition;

        List<string> ret = new List<string>();
        PseudoBlock parent = null;
        var field = code as PseudoField;
        if (field != null)
            parent = field.Parent;
        var block = code as PseudoBlock;
        if (block != null)
            parent = block.Parent;

        while (parent != null)
        {
            if (parent.Type == "for")
            {
                if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i ="))
                    ret.Insert(0, "[i]");
                else if (parent.Condition.Contains("j =") || parent.Condition.Contains("j="))
                    ret.Insert(0, "[j]");
                else if (parent.Condition.Contains("f =") || parent.Condition.Contains("f="))
                    ret.Insert(0, "[f]");
                else if (parent.Condition.Contains("c =") || parent.Condition.Contains("c="))
                    ret.Insert(0, "[c]");
                else if (parent.Condition.Contains("a =") || parent.Condition.Contains("a="))
                    ret.Insert(0, "[a]");
                else if (parent.Condition.Contains("k =") || parent.Condition.Contains("k="))
                    ret.Insert(0, "[k]");
                else if (parent.Condition.Contains("cnt =") || parent.Condition.Contains("cnt="))
                    ret.Insert(0, "[cnt]");
                else if (parent.Condition.Contains("el =") || parent.Condition.Contains("el="))
                    ret.Insert(0, "[el]");
                else
                    throw new Exception();
            }

            parent = parent.Parent;
        }

        if (field != null)
            parent = field.Parent;
        if (block != null)
            parent = block.Parent;

        int level = 0;
        while (parent != null)
        {
            string str = string.Concat(ret.Skip(level));

            if (parent.Type == "for")
            {
                level++;
            }

            foreach (var f in parent.Content)
            {
                if (f is PseudoField ff)
                {
                    if (string.IsNullOrWhiteSpace(ff.Name) || IsWorkaround(ff.Name))
                        continue;
                    if (condition.Contains(prefix + ff.Name + suffix) && !condition.Contains(prefix + ff.Name + "["))
                    {
                        condition = condition.Replace(prefix + ff.Name + suffix, prefix + ff.Name + str + suffix);
                    }
                }
                else if (f is PseudoBlock bb)
                {
                    foreach (var fff in bb.Content)
                    {
                        if (fff is PseudoField ffff)
                        {
                            if (string.IsNullOrWhiteSpace(ffff.Name) || IsWorkaround(ffff.Name))
                                continue;
                            if (condition.Contains(prefix + ffff.Name + suffix) && !condition.Contains(prefix + ffff.Name + "["))
                            {
                                condition = condition.Replace(prefix + ffff.Name + suffix, prefix + ffff.Name + str + suffix);
                            }
                        }
                    }
                }
            }

            parent = parent.Parent;
        }

        return condition;
    }

    private static string GetFieldName(PseudoCode field)
    {
        string name = Sanitize((field as PseudoField)?.Name);
        if (string.IsNullOrEmpty(name))
        {
            if ((field as PseudoField).Type.Type.StartsWith("byte_"))
            {
                // byte_alignment would otherwise produce a name "byte"
                name = "byte_alignment";
            }
            else
            {
                name = GetType(GetFieldType(field as PseudoField))?.Replace("[]", "");
            }
        }

        return name;
    }

    private static string BuildMethodCode(PseudoClass b, PseudoBlock parent, PseudoCode field, int level, MethodType methodType)
    {
        string spacing = GetSpacing(level);
        var block = field as PseudoBlock;
        if (block != null)
        {
            return BuildBlock(b, parent, block, level, methodType);
        }

        var repeatingBlock = field as PseudoRepeatingBlock;
        if(repeatingBlock != null)
        {
            return BuildRepeatingBlock(b, parent, block, level, methodType);
        }

        var comment = field as PseudoComment;
        if (comment != null)
        {
            return BuildComment(b, comment, level, methodType);
        }

        var swcase = field as PseudoCase;
        if(swcase != null)
        {
            return BuildSwitchCase(b, swcase, level, methodType);
        }

        string tt = GetFieldType(field as PseudoField);

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
            else if (tt == "int downmix_instructions_count = 1")
                return "downmix_instructions_count = 1;";
            else if (tt == "return audioObjectType;")
                return "// return audioObjectType;";
            else if (tt == "samplerate = samplerate >> 16")
                return "// samplerate = samplerate >> 16";
            else
                return $"{tt};";
        }

        string name = GetFieldName(field);
        string m = methodType == MethodType.Read ? GetReadMethod(tt) : (methodType == MethodType.Write ? GetWriteMethod(tt) : GetCalculateSizeMethod(tt));
        string typedef = "";
        typedef = GetFieldTypeDef(field);

        string fieldComment = "";
        if (!string.IsNullOrEmpty((field as PseudoField)?.Comment))
        {
            fieldComment = "//" + (field as PseudoField).Comment;
        }

        string boxSize = "boxSize += ";
        if (m.StartsWith("case")) // workaround for missing case support
            boxSize = "";

        // comment out all ReadBox/ReadDescriptor, WriteBox/WriteDescriptor and Calculate* methods
        if ((m.Contains("Box") && b.BoxName != "MetaDataAccessUnit" && b.BoxName != "SampleGroupDescriptionBox" && b.BoxName != "ItemReferenceBox" && b.BoxName != "IPMPControlBox" && b.BoxName != "IPMPInfoBox") || 
            (m.Contains("Descriptor") && b.BoxName != "ESDBox" && b.BoxName != "MpegSampleEntry" && b.BoxName != "AppleInitialObjectDescriptorBox" && b.BoxName != "MPEG4ExtensionDescriptorsBox"))
        {
            spacing += "// ";
        }

        if(fieldComment != null && fieldComment.Contains("optional"))
        {
            if (methodType == MethodType.Read)
            {
                spacing += "if (stream.HasMoreData(boxSize, readSize)) ";
            }
            else if (methodType == MethodType.Write)
            {
                spacing += ""; // TODO
            }
            else if (methodType == MethodType.Size)
            {
                spacing += ""; // TODO
            }
        }

        if (methodType == MethodType.Size)
            m = m.Replace("value", name);

        if (GetNestedInLoop(field) > 0)
        {
            string suffix;
            GetNestedInLoopSuffix(field, typedef, out suffix);
            typedef += suffix;

            if (methodType != MethodType.Size)
            {
                m = FixNestedInLoopVariables(field, m, "(", ",");
                m = FixNestedInLoopVariables(field, m, ")", ","); // when casting
            }
            else
                m = FixNestedInLoopVariables(field, m, "", " ");
        }

        if (methodType == MethodType.Read)
            return $"{spacing}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
        else if (methodType == MethodType.Write)
            return $"{spacing}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
        else
            return $"{spacing}{boxSize}{m}; // {name}";
    }

    private static string BuildSwitchCase(PseudoClass b, PseudoCase swcase, int level, MethodType methodType)
    {
        string spacing = GetSpacing(level);
        if (string.IsNullOrEmpty(swcase.Op))
        {
            if (swcase.Cs == "default:")
                return $"{spacing}default:\r\n";
            else
                return $"{spacing}break;\r\n";
        }
        else
        {
            return $"{spacing}{swcase.Cs} {swcase.Op}:";
        }
    }

    private static string BuildRepeatingBlock(PseudoClass b, PseudoBlock parent, PseudoBlock? block, int level, MethodType methodType)
    {
        if (b.BoxName == "TrackRunBox")
        {
            if (methodType == MethodType.Read)
            {
                return "entries = new List<TrunEntry>();\r\n            for (int i = 0; i < sample_count; i++)\r\n            {\r\n                TrunEntry entry = new TrunEntry();\r\n                if ((flags & 0x100) == 0x100)\r\n                {\r\n                    boxSize += stream.ReadUInt32(out entry.SampleDuration);\r\n                }\r\n\r\n                if ((flags & 0x200) == 0x200)\r\n                {\r\n                    boxSize += stream.ReadUInt32(out entry.SampleSize);\r\n                }\r\n\r\n                if ((flags & 0x400) == 0x400)\r\n                {\r\n                    boxSize = stream.ReadUInt32(out entry.SampleFlags);\r\n                }\r\n\r\n                if ((flags & 0x800) == 0x800)\r\n                {\r\n                    if (version == 0)\r\n                    {\r\n                        boxSize += stream.ReadUInt32(out entry.SampleCompositionTimeOffset0);\r\n                    }\r\n                    else\r\n                    {\r\n                        boxSize = stream.ReadInt32(out entry.SampleCompositionTimeOffset);\r\n                    }\r\n                }\r\n\r\n                entries.Add(entry);\r\n            }";
            }
            else if (methodType == MethodType.Write)
            {
                return "for (int i = 0; i < sample_count; i++)\r\n            {\r\n                if ((flags & 0x100) == 0x100)\r\n                {\r\n                    boxSize += stream.WriteUInt32(entries[i].SampleDuration);\r\n                }\r\n\r\n                if ((flags & 0x200) == 0x200)\r\n                {\r\n                    boxSize += stream.WriteUInt32(entries[i].SampleSize);\r\n                }\r\n\r\n                if ((flags & 0x400) == 0x400)\r\n                {\r\n                    boxSize = stream.WriteUInt32(entries[i].SampleFlags);\r\n                }\r\n\r\n                if ((flags & 0x800) == 0x800)\r\n                {\r\n                    if (version == 0)\r\n                    {\r\n                        boxSize += stream.WriteUInt32(entries[i].SampleCompositionTimeOffset0);\r\n                    }\r\n                    else\r\n                    {\r\n                        boxSize = stream.WriteInt32(entries[i].SampleCompositionTimeOffset);\r\n                    }\r\n                }\r\n            }";
            }
            else if (methodType == MethodType.Size)
            {
                return "for (int i = 0; i < sample_count; i++)\r\n            {\r\n                if ((flags & 0x100) == 0x100)\r\n                {\r\n                    boxSize += 32;\r\n                }\r\n\r\n                if ((flags & 0x200) == 0x200)\r\n                {\r\n                    boxSize += 32;\r\n                }\r\n\r\n                if ((flags & 0x400) == 0x400)\r\n                {\r\n                    boxSize = 32;\r\n                }\r\n\r\n                if ((flags & 0x800) == 0x800)\r\n                {\r\n                    if (version == 0)\r\n                    {\r\n                        boxSize += 32;\r\n                    }\r\n                    else\r\n                    {\r\n                        boxSize = 32;\r\n                    }\r\n                }\r\n            }";
            }
        }
        
        throw new NotImplementedException();
    }

    private static string GetFieldTypeDef(PseudoCode field)
    {
        string value = (field as PseudoField)?.Value;
        if (!string.IsNullOrEmpty(value) && value.StartsWith("[") && value != "[]" &&
            value != "[count]" && value != "[ entry_count ]" && value != "[numReferences]"
            && value != "[0 .. 255]" && value != "[0..1]" && value != "[0 .. 1]" && value != "[0..255]" &&
            value != "[ sample_count ]" && value != "[sample_count]" && value != "[subsample_count]" && value != "[method_count]" && value != "[URLlength]" && value != "[sizeOfInstance-4]" && value != "[sizeOfInstance-3]" && value != "[size-10]" && value != "[tagLength]" && value != "[length-6]" && value != "[3]" && value != "[16]" && value != "[14]" && value != "[4]" && value != "[6]" && value != "[32]" && value != "[36]" && value != "[256]" && value != "[512]" && value != "[chunk_length]" &&
            value != "[contentIDLength]" && value != "[contentTypeLength]" && value != "[rightsIssuerLength]" && value != "[textualHeadersLength]" && value != "[numIndSub + 1]")
        {
            return value;
        }

        return "";
    }

    private static string BuildComment(PseudoClass b, PseudoComment comment, int level, MethodType methodType)
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

    private static string BuildBlock(PseudoClass b, PseudoBlock parent, PseudoBlock block, int level, MethodType methodType)
    {
        string spacing = GetSpacing(level);
        string ret = "";

        string condition = block.Condition?.Replace("'", "\"").Replace("floor(", "(");
        string blockType = block.Type;
        if (blockType == "for")
        {
            condition = FixForCycleCondition(condition);
        }
        else if (blockType == "do ")
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

            // fix for Apple extensions to AudioSampleEntry
            if (condition.Contains("codingname"))
                condition = condition.Replace("codingname", "IsoStream.FromFourCC(FourCC)");
        }

        if (!string.IsNullOrEmpty(condition))
        {
            if (b.BoxName != "GASpecificConfig")
            {
                if (condition.Contains("audioObjectType") && !condition.Contains("audioObjectType == 31"))
                    condition = condition.Replace("audioObjectType", "audioObjectType.AudioObjectType");
            }

            if (condition.Contains("extensionAudioObjectType"))
                condition = condition.Replace("extensionAudioObjectType", "extensionAudioObjectType.AudioObjectType");

            if (condition.Contains("bits_to_decode()"))
            {
                if (methodType == MethodType.Read)
                {
                    condition = condition.Replace("bits_to_decode()", "IsoStream.BitsToDecode(boxSize, readSize)");
                }
                else
                {
                    condition = condition.Replace("bits_to_decode()", "IsoStream.BitsToDecode()");
                }
            }

            if(condition.Contains("AVCProfileIndication  ==  100  ||  AVCProfileIndication  ==  110"))
            {
                // this condition is necessary, otherwise AVCDecoderConfigurationRecord can exceed its box size
                if(methodType == MethodType.Read)
                {
                    ret += $"\r\n{spacing}if (boxSize >= readSize || (readSize - boxSize) < 4) return boxSize; else HasExtensions = true;";
                }
                else
                {
                    ret += $"\r\n{spacing}if (!HasExtensions) return boxSize;";
                }
            }
        }

        condition = FixFourCC(condition);

        int nestedLevel = GetNestedInLoop(block);
        if (nestedLevel > 0)
        {
            // patch condition
            condition = FixNestedInLoopVariables(block, condition);
        }

        if (methodType == MethodType.Read)
        {
            if (block.RequiresAllocation.Count > 0)
            {
                if (block.Type == "for")
                {
                    string[] parts = condition.Substring(1, condition.Length - 2).Split(';');
                    string variable = parts[1].Split('<', '=', '>', '!').Last();

                    if (parts[1] == " j<=8 && num_sublayers > 1")
                    {
                        variable = "9";
                    }
                    else if (variable.Contains("0"))
                    {
                        if (parts[0].Contains("num_sublayers-2") || parts[0].Contains("num_sublayers - 2"))
                        {
                            variable = "num_sublayers - 1";
                        }
                        else if (parts[1] != " i< numOfSequenceParameterSets && numOfSequenceParameterSets <= 64 && numOfSequenceParameterSets >= 0")
                        {
                            throw new Exception();
                        }
                    }
                    else if(variable.Contains("_minus1"))
                    {
                        variable = variable + " + 1";
                    }

                    if (!string.IsNullOrWhiteSpace(variable))
                    {
                        foreach (var req in block.RequiresAllocation)
                        {
                            bool hasBoxes = GetReadMethod(GetFieldType(req)).Contains("ReadBox(") && b.BoxName != "MetaDataAccessUnit" && b.BoxName != "SampleGroupDescriptionBox";
                            if (hasBoxes)
                                continue;

                            string suffix;
                            int blockSuffixLevel = GetNestedInLoopSuffix(block, "", out suffix);
                            int fieldSuffixLevel = GetNestedInLoopSuffix(req, "", out _);

                            string appendType = "";
                            if(fieldSuffixLevel - blockSuffixLevel > 1)
                            {
                                int count = fieldSuffixLevel - blockSuffixLevel - 1;

                                for (int i = 0; i < count; i++)
                                {
                                    appendType += "[]";
                                }
                            }

                            string variableType = GetType(GetFieldType(req));
                            int indexesTypeDef = GetFieldTypeDef(req).Count(x => x == '[');
                            int indexesType = variableType.Count(x => x == '[');
                            string variableName = GetFieldName(req) + suffix;
                            if (variableType.Contains("[]"))
                            {
                                int diff = (indexesType - indexesTypeDef);
                                variableType = variableType.Replace("[]", "");
                                variableType = $"{variableType}[{variable}]";
                                for (int i = 0; i < diff; i++)
                                {
                                    variableType += "[]";
                                }
                            }
                            else
                            {
                                variableType = variableType + $"[{variable}]";
                            }
                            ret += $"\r\n{spacing}this.{variableName} = new {variableType}{appendType};";
                        }
                    }
                    else
                    {
                        Debug.WriteLine("ERROR invalid variable");
                    }
                }
                else
                {
                    throw new Exception("allocation for block other than for");
                }
            }
        }

        ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

        foreach (var field in block.Content)
        {
            ret += "\r\n" + BuildMethodCode(b, block, field, level + 1, methodType);
        }

        ret += $"\r\n{spacing}}}";

        return ret;
    }

    private static string FixForCycleCondition(string condition)
    {
        condition = condition.Substring(1, condition.Length - 2);

        string[] parts = condition.Split(";");

        if (parts[0].Contains("1") && parts[1].Contains("="))
        {
            parts[0] = parts[0].Replace("1", "0");
            parts[1] = parts[1].Replace("=", "");
        }
        else
        {
            if (!((parts[0].Contains("0") && !parts[1].Contains("=")) || (parts[0].Contains("0") && parts[1].Contains("_minus1") && parts[1].Contains("="))) &&
                parts[1] != " i >= 0" &&
                parts[1] != " j<=8 && num_sublayers > 1" && 
                parts[1] != " i< numOfSequenceParameterSets && numOfSequenceParameterSets <= 64 && numOfSequenceParameterSets >= 0")
                throw new Exception();
        }
        
        return $"(int {string.Join(";", parts)})";
    }

    private static string FixFourCC(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            // fix 4cc
            const string regex = "\\\"[\\w\\s\\!]{4}\\\"";
            var match = Regex.Match(value, regex);
            while (match.Success)
            {
                string inputValue = match.Value;
                string replaceValue = "IsoStream.FromFourCC(" + inputValue + ")";
                value = value.Replace(inputValue, replaceValue);
                match = match.NextMatch();
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
            { "unsigned int(64)[ entry_count ]",        "stream.ReadUInt64Array(entry_count, " },
            { "unsigned int(64)",                       "stream.ReadUInt64(" },
            { "unsigned int(48)",                       "stream.ReadUInt48(" },
            { "unsigned int(32)[ entry_count ]",        "stream.ReadUInt32Array(entry_count, " },
            { "template int(32)[9]",                    "stream.ReadUInt32Array(9, " },
            { "unsigned int(32)[3]",                    "stream.ReadUInt32Array(3, " },
            { "unsigned int(32)",                       "stream.ReadUInt32(" },
            { "unsigned int(31)",                       "stream.ReadBits(31, " },
            { "unsigned_int(32)",                       "stream.ReadUInt32(" },
            { "unsigned int(24)",                       "stream.ReadUInt24(" },
            { "unsigned int(29)",                       "stream.ReadBits(29, " },
            { "unsigned int(28)",                       "stream.ReadBits(28, " },
            { "unsigned int(26)",                       "stream.ReadBits(26, " },
            { "unsigned int(16)[i]",                    "stream.ReadUInt16(" },
            { "unsigned int(16)[j]",                    "stream.ReadUInt16(" },
            { "unsigned int(16)[i][j]",                 "stream.ReadUInt16(" },
            { "unsigned int(16)",                       "stream.ReadUInt16(" },
            { "unsigned_int(16)",                       "stream.ReadUInt16(" },
            { "unsigned int(15)",                       "stream.ReadBits(15, " },
            { "unsigned int(12)",                       "stream.ReadBits(12, " },
            { "signed int(12)",                         "stream.ReadBits(12, " },
            { "unsigned int(10)[i][j]",                 "stream.ReadBits(10, " },
            { "unsigned int(10)[i]",                    "stream.ReadBits(10, " },
            { "unsigned int(10)",                       "stream.ReadBits(10, " },
            { "unsigned int(8)[ sample_count ]",        "stream.ReadUInt8Array((uint)sample_count, " },
            { "unsigned int(8)[length]",                "stream.ReadUInt8Array((uint)length, " },
            { "unsigned int(8)[32]",                    "stream.ReadUInt8Array(32, " },
            { "unsigned int(8)[36]",                    "stream.ReadUInt8Array(36, " },
            { "unsigned int(8)[20]",                    "stream.ReadUInt8Array(20, " },
            { "unsigned int(8)[16]",                    "stream.ReadUInt8Array(16, " },
            { "unsigned int(9)",                        "stream.ReadBits(9, " },
            { "unsigned int(8)",                        "stream.ReadUInt8(" },
            { "unsigned int(7)",                        "stream.ReadBits(7, " },
            { "unsigned int(6)",                        "stream.ReadBits(6, " },
            { "unsigned int(5)[3]",                     "stream.ReadIso639(" },
            { "unsigned int(5)",                        "stream.ReadBits(5, " },
            { "unsigned int(4)",                        "stream.ReadBits(4, " },
            { "unsigned int(3)",                        "stream.ReadBits(3, " },
            { "unsigned int(2)[i][j]",                  "stream.ReadBits(2, " },
            { "unsigned int(2)",                        "stream.ReadBits(2, " },
            { "unsigned int(1)[i]",                     "stream.ReadBit(" },
            { "unsigned int(1)",                        "stream.ReadBit(" },
            { "unsigned int (32)",                      "stream.ReadUInt32(" },
            { "unsigned int(f(pattern_size_code))[i]",  "stream.ReadBits(pattern_size_code, " },
            { "unsigned int(f(index_size_code))[i]",    "stream.ReadBits(index_size_code, " },
            { "unsigned int(f(index_size_code))[j][k]", "stream.ReadBits(index_size_code, " },
            { "unsigned int(f(count_size_code))[i]",    "stream.ReadBits(count_size_code, " },
            { "unsigned int(base_offset_size*8)",       "stream.ReadUInt8Array((uint)base_offset_size, " },
            { "unsigned int(offset_size*8)",            "stream.ReadUInt8Array((uint)offset_size, " },
            { "unsigned int(length_size*8)",            "stream.ReadUInt8Array((uint)length_size, " },
            { "unsigned int(index_size*8)",             "stream.ReadUInt8Array((uint)index_size, " },
            { "unsigned int(field_size)",               "stream.ReadUInt8Array((uint)field_size, " },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "stream.ReadUInt8Array((uint)(length_size_of_traf_num+1), " },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "stream.ReadUInt8Array((uint)(length_size_of_trun_num+1), " },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "stream.ReadUInt8Array((uint)(length_size_of_sample_num+1), " },
            { "unsigned int(8*size-64)",                "stream.ReadUInt8Array((uint)(size-8), " },
            { "bit(8)[chunk_length]",                   "stream.ReadUInt8Array((uint)chunk_length, " },
            { "unsigned int(subgroupIdLen)[i]",         "stream.ReadUInt32(" },
            { "const unsigned int(8)[6]",               "stream.ReadUInt8Array(6, " },
            { "const unsigned int(32)[2]",              "stream.ReadUInt32Array(2, " },
            { "const unsigned int(32)[3]",              "stream.ReadUInt32Array(3, " },
            { "const unsigned int(32)",                 "stream.ReadUInt32(" },
            { "const unsigned int(16)[3]",              "stream.ReadUInt16Array(3, " },
            { "const unsigned int(16)",                 "stream.ReadUInt16(" },
            { "const unsigned int(26)",                 "stream.ReadBits(26, " },
            { "template int(32)",                       "stream.ReadInt32(" },
            { "template int(16)",                       "stream.ReadInt16(" },
            { "template unsigned int(30)",              "stream.ReadBits(30, " },
            { "template unsigned int(32)",              "stream.ReadUInt32(" },
            { "template unsigned int(16)[3]",           "stream.ReadUInt16Array(3, " },
            { "template unsigned int(16)",              "stream.ReadUInt16(" },
            { "template unsigned int(8)[]",             "stream.ReadUInt8ArrayTillEnd(boxSize, readSize, " },
            { "template unsigned int(8)",               "stream.ReadUInt8(" },
            { "int(64)",                                "stream.ReadInt64(" },
            { "int(32)",                                "stream.ReadInt32(" },
            { "int(16)",                                "stream.ReadInt16(" },
            { "int(8)",                                 "stream.ReadInt8(" },
            { "int(4)",                                 "stream.ReadBits(4, " },
            { "int",                                    "stream.ReadInt32(" },
            { "const bit(16)",                          "stream.ReadUInt16(" },
            { "const bit(1)",                           "stream.ReadBit(" },
            { "bit(1)",                                 "stream.ReadBit(" },
            { "bit(2)",                                 "stream.ReadBits(2, " },
            { "bit(3)",                                 "stream.ReadBits(3, " },
            { "bit(length-3)",                          "stream.ReadBits((uint)(length-3), " },
            { "bit(length)",                            "stream.ReadBits(length, " },
            { "bit(4)",                                 "stream.ReadBits(4, " },
            { "bit(5)",                                 "stream.ReadBits(5, " },
            { "bit(6)",                                 "stream.ReadBits(6, " },
            { "bit(7)",                                 "stream.ReadBits(7, " },
            { "bit(8)[]",                               "stream.ReadUInt8ArrayTillEnd(boxSize, readSize, " },
            { "bit(8)",                                 "stream.ReadUInt8(" },
            { "bit(9)",                                 "stream.ReadBits(9, " },
            { "bit(13)",                                "stream.ReadBits(13, " },
            { "bit(14)",                                "stream.ReadBits(14, " },
            { "bit(15)",                                "stream.ReadBits(15, " },
            { "bit(16)[i]",                             "stream.ReadUInt16(" },
            { "bit(16)",                                "stream.ReadUInt16(" },
            { "bit(24)",                                "stream.ReadBits(24, " },
            { "bit(31)",                                "stream.ReadBits(31, " },
            { "bit(8 ceil(size / 8) \u2013 size)",      "stream.ReadUInt8Array((uint)(Math.Ceiling(size / 8d) - size), " },
            { "bit(8* ps_nalu_length)",                 "stream.ReadUInt8Array((uint)ps_nalu_length, " },
            { "bit(8*nalUnitLength)",                   "stream.ReadUInt8Array((uint)nalUnitLength, " },
            { "bit(8*sequenceParameterSetLength)",      "stream.ReadUInt8Array((uint)sequenceParameterSetLength, " },
            { "bit(8*pictureParameterSetLength)",       "stream.ReadUInt8Array((uint)pictureParameterSetLength, " },
            { "bit(8*sequenceParameterSetExtLength)",   "stream.ReadUInt8Array((uint)sequenceParameterSetExtLength, " },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "stream.ReadBits((uint)(8*num_bytes_constraint_info - 2), " },
            { "bit(8*nal_unit_length)",                 "stream.ReadUInt8Array((uint)nal_unit_length, " },
            { "bit(timeStampLength)",                   "stream.ReadUInt8Array((uint)timeStampLength, " },
            { "utf8string",                             "stream.ReadStringZeroTerminated(boxSize, readSize, " },
            { "utfstring",                              "stream.ReadStringZeroTerminated(boxSize, readSize, " },
            { "utf8list",                               "stream.ReadStringZeroTerminated(boxSize, readSize, " },
            { "boxstring",                              "stream.ReadStringZeroTerminated(boxSize, readSize, " },
            { "string",                                 "stream.ReadStringZeroTerminated(boxSize, readSize, " },
            { "bit(32)[6]",                             "stream.ReadUInt32Array(6, " },
            { "bit(32)",                                "stream.ReadUInt32(" },
            { "uint(32)",                               "stream.ReadUInt32(" },
            { "uint(16)",                               "stream.ReadUInt16(" },
            { "uint(64)",                               "stream.ReadUInt64(" },
            { "uint(8)[32]",                            "stream.ReadUInt8Array(32, " }, // compressor_name
            { "uint(8)",                                "stream.ReadUInt8(" },
            { "uint(7)",                                "stream.ReadBits(7, " },
            { "uint(1)",                                "stream.ReadBits(1, " },
            { "signed int(32)",                         "stream.ReadInt32(" },
            { "signed int (16)",                        "stream.ReadInt16(" },
            { "signed int(16)[grid_pos_view_id[i]]",    "stream.ReadInt16(" },
            { "signed int(16)",                         "stream.ReadInt16(" },
            { "signed int (8)",                         "stream.ReadInt8(" },
            { "signed int(64)",                         "stream.ReadInt64(" },
            { "signed int(8)",                        "stream.ReadInt8(" },
            { "Box()[]",                                "stream.ReadBox(boxSize, readSize, this, " },
            { "Box[]",                                  "stream.ReadBox(boxSize, readSize, this, " },
            { "Box()",                                    "stream.ReadBox(boxSize, readSize, this, " },
            { "Box",                                    "stream.ReadBox(boxSize, readSize, this, " },
            { "SchemeTypeBox",                          "stream.ReadBox(boxSize, readSize, this, " },
            { "SchemeInformationBox",                   "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemPropertyContainerBox",               "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemPropertyAssociationBox",             "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemPropertyAssociationBox[]",           "stream.ReadBox(boxSize, readSize, this, " },
            { "char",                                   "stream.ReadInt8(" },
            { "loudness",                               "stream.ReadInt32(" },
            { "ICC_profile",                            "stream.ReadClass(boxSize, readSize, this, new ICC_profile(), " },
            { "OriginalFormatBox(fmt)",                 "stream.ReadBox(boxSize, readSize, this, " },
            { "DataEntryBaseBox(entry_type, entry_flags)", "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemInfoEntry[ entry_count ]",           "stream.ReadBox(boxSize, readSize, this, " },
            { "TypeCombinationBox[]",                   "stream.ReadBox(boxSize, readSize, this, " },
            { "FilePartitionBox",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "FECReservoirBox",                        "stream.ReadBox(boxSize, readSize, this, " },
            { "FileReservoirBox",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "PartitionEntry[ entry_count ]",          "stream.ReadBox(boxSize, readSize, this, entry_count, " },
            { "FDSessionGroupBox",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "GroupIdToNameBox",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "base64string",                           "stream.ReadStringZeroTerminated(boxSize, readSize, " },
            { "ProtectionSchemeInfoBox",                "stream.ReadBox(boxSize, readSize, this, " },
            { "SingleItemTypeReferenceBox",             "stream.ReadBox<SingleItemTypeReferenceBox>(boxSize, readSize, (header) => new SingleItemTypeReferenceBox(IsoStream.ToFourCC(header.Type)), this, " },
            { "SingleItemTypeReferenceBox[]",           "stream.ReadBox<SingleItemTypeReferenceBox>(boxSize, readSize, (header) => new SingleItemTypeReferenceBox(IsoStream.ToFourCC(header.Type)), this, " },
            { "SingleItemTypeReferenceBoxLarge",        "stream.ReadBox<SingleItemTypeReferenceBoxLarge>(boxSize, readSize, (header) => new SingleItemTypeReferenceBoxLarge(IsoStream.ToFourCC(header.Type)), this, " },
            { "SingleItemTypeReferenceBoxLarge[]",      "stream.ReadBox<SingleItemTypeReferenceBoxLarge>(boxSize, readSize, (header) => new SingleItemTypeReferenceBoxLarge(IsoStream.ToFourCC(header.Type)), this, " },
            { "HandlerBox(handler_type)",               "stream.ReadBox(boxSize, readSize, this, " },
            { "PrimaryItemBox",                         "stream.ReadBox(boxSize, readSize, this, " },
            { "DataInformationBox",                     "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemLocationBox",                        "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemProtectionBox",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemInfoBox",                            "stream.ReadBox(boxSize, readSize, this, " },
            { "IPMPControlBox",                         "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemReferenceBox",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "ItemDataBox",                            "stream.ReadBox(boxSize, readSize, this, " },
            { "TrackReferenceTypeBox[]",                "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataKeyBox[]",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "TierInfoBox()",                            "stream.ReadBox(boxSize, readSize, this, " },
            { "TierInfoBox",                            "stream.ReadBox(boxSize, readSize, this, " },
            { "MultiviewRelationAttributeBox",          "stream.ReadBox(boxSize, readSize, this, " },
            { "TierBitRateBox()",                         "stream.ReadBox(boxSize, readSize, this, " },
            { "TierBitRateBox",                         "stream.ReadBox(boxSize, readSize, this, " },
            { "BufferingBox()",                           "stream.ReadBox(boxSize, readSize, this, " },
            { "BufferingBox",                           "stream.ReadBox(boxSize, readSize, this, " },
            { "MultiviewSceneInfoBox",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "MVDDecoderConfigurationRecord",          "stream.ReadClass(boxSize, readSize, this, new MVDDecoderConfigurationRecord(), " },
            { "MVDDepthResolutionBox",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "MVCDecoderConfigurationRecord()",        "stream.ReadClass(boxSize, readSize, this, new MVCDecoderConfigurationRecord(), " },
            { "AVCDecoderConfigurationRecord()",        "stream.ReadClass(boxSize, readSize, this, new AVCDecoderConfigurationRecord(), " },
            { "HEVCDecoderConfigurationRecord()",       "stream.ReadClass(boxSize, readSize, this, new HEVCDecoderConfigurationRecord(), " },
            { "LHEVCDecoderConfigurationRecord()",      "stream.ReadClass(boxSize, readSize, this, new LHEVCDecoderConfigurationRecord(), " },
            { "SVCDecoderConfigurationRecord()",        "stream.ReadClass(boxSize, readSize, this, new SVCDecoderConfigurationRecord(), " },
            { "HEVCTileTierLevelConfigurationRecord()", "stream.ReadClass(boxSize, readSize, this, new HEVCTileTierLevelConfigurationRecord(), " },
            { "EVCDecoderConfigurationRecord()",        "stream.ReadClass(boxSize, readSize, this, new EVCDecoderConfigurationRecord(), " },
            { "VvcDecoderConfigurationRecord()",        "stream.ReadClass(boxSize, readSize, this, new VvcDecoderConfigurationRecord(), " },
            { "EVCSliceComponentTrackConfigurationRecord()", "stream.ReadClass(boxSize, readSize, this, new EVCSliceComponentTrackConfigurationRecord(), " },
            { "Descriptor[0 .. 255]",                   "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "Descriptor[count]",                      "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "Descriptor",                             "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "WebVTTConfigurationBox",                 "stream.ReadBox(boxSize, readSize, this, " },
            { "WebVTTSourceLabelBox",                   "stream.ReadBox(boxSize, readSize, this, " },
            { "OperatingPointsRecord",                  "stream.ReadClass(boxSize, readSize, this, new OperatingPointsRecord(), " },
            { "VvcSubpicIDEntry",                       "stream.ReadEntry(boxSize, readSize, this, TYPE, " },
            { "VvcSubpicOrderEntry",                    "stream.ReadEntry(boxSize, readSize, this, TYPE, " },
            { "URIInitBox",                             "stream.ReadBox(boxSize, readSize, this, " },
            { "URIBox",                                 "stream.ReadBox(boxSize, readSize, this, " },
            { "CleanApertureBox",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "PixelAspectRatioBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "DownMixInstructions()[]",               "stream.ReadBox(boxSize, readSize, this, " },
            { "DRCCoefficientsBasic()[]",              "stream.ReadBox(boxSize, readSize, this, " },
            { "DRCInstructionsBasic()[]",              "stream.ReadBox(boxSize, readSize, this, " },
            { "DRCCoefficientsUniDRC()[]",             "stream.ReadBox(boxSize, readSize, this, " },
            { "DRCInstructionsUniDRC()[]",             "stream.ReadBox(boxSize, readSize, this, " },
            { "HEVCConfigurationBox",                   "stream.ReadBox(boxSize, readSize, this, " },
            { "LHEVCConfigurationBox",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "AVCConfigurationBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "SVCConfigurationBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "ScalabilityInformationSEIBox",           "stream.ReadBox(boxSize, readSize, this, " },
            { "SVCPriorityAssignmentBox",               "stream.ReadBox(boxSize, readSize, this, " },
            { "ViewScalabilityInformationSEIBox",       "stream.ReadBox(boxSize, readSize, this, " },
            { "ViewIdentifierBox()",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "MVCConfigurationBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "MVCViewPriorityAssignmentBox",           "stream.ReadBox(boxSize, readSize, this, " },
            { "IntrinsicCameraParametersBox",           "stream.ReadBox(boxSize, readSize, this, " },
            { "ExtrinsicCameraParametersBox",           "stream.ReadBox(boxSize, readSize, this, " },
            { "MVCDConfigurationBox",                   "stream.ReadBox(boxSize, readSize, this, " },
            { "MVDScalabilityInformationSEIBox",        "stream.ReadBox(boxSize, readSize, this, " },
            { "A3DConfigurationBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "VvcOperatingPointsRecord",               "stream.ReadClass(boxSize, readSize, this, new VvcOperatingPointsRecord(), " },
            { "VVCSubpicIDRewritingInfomationStruct()", "stream.ReadClass(boxSize, readSize, this, new VVCSubpicIDRewritingInfomationStruct(), " },
            { "MPEG4ExtensionDescriptorsBox ()",        "stream.ReadBox(boxSize, readSize, this, " },
            { "MPEG4ExtensionDescriptorsBox()",         "stream.ReadBox(boxSize, readSize, this, " },
            { "MPEG4ExtensionDescriptorsBox",           "stream.ReadBox(boxSize, readSize, this, " },
            { "bit(8*dci_nal_unit_length)",             "stream.ReadUInt8Array((uint)dci_nal_unit_length, " },
            { "DependencyInfo[numReferences]",          "stream.ReadClass(boxSize, readSize, this, " },
            { "VvcPTLRecord(0)[i]",                     "stream.ReadClass(boxSize, readSize, this, new VvcPTLRecord(0), " },
            { "EVCSliceComponentTrackConfigurationBox", "stream.ReadBox(boxSize, readSize, this, " },
            { "SVCMetadataSampleConfigBox",             "stream.ReadBox(boxSize, readSize, this, " },
            { "SVCPriorityLayerInfoBox",                "stream.ReadBox(boxSize, readSize, this, " },
            { "EVCConfigurationBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "VvcNALUConfigBox",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "VvcConfigurationBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "HEVCTileConfigurationBox",               "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataKeyTableBox()",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "BitRateBox()",                          "stream.ReadBox(boxSize, readSize, this, " },
            { "char[count]",                            "stream.ReadUInt8Array((uint)count, " },
            { "signed int(32)[ c ]",                    "stream.ReadInt32(" },
            { "unsigned int(8)[]",                      "stream.ReadUInt8ArrayTillEnd(boxSize, readSize, " },
            { "unsigned int(8)[i]",                     "stream.ReadUInt8(" },
            { "unsigned int(6)[i]",                     "stream.ReadBits(6, " },
            { "unsigned int(6)[i][j]",                  "stream.ReadBits(6, " },
            { "unsigned int(1)[i][j]",                  "stream.ReadBits(1, " },
            { "unsigned int(9)[i]",                     "stream.ReadBits(9, " },
            { "unsigned int(32)[]",                     "stream.ReadUInt32ArrayTillEnd(boxSize, readSize, " },
            { "unsigned int(32)[i]",                    "stream.ReadUInt32(" },
            { "unsigned int(32)[j]",                    "stream.ReadUInt32(" },
            { "unsigned int(8)[j][k]",                  "stream.ReadUInt8(" },
            { "signed int(64)[j][k]",                 "stream.ReadInt64(" },
            { "unsigned int(8)[j]",                     "stream.ReadUInt8(" },
            { "signed int(64)[j]",                    "stream.ReadInt64(" },
            { "char[]",                                 "stream.ReadUInt8ArrayTillEnd(boxSize, readSize, " },
            { "string[method_count]",                   "stream.ReadStringArray(method_count, " },
            { "ItemInfoExtension(extension_type)",                      "stream.ReadClass(boxSize, readSize, this, new ItemInfoExtension(IsoStream.ToFourCC(extension_type)), " },
            { "SampleGroupDescriptionEntry(grouping_type)",            "stream.ReadEntry(boxSize, readSize, this, IsoStream.ToFourCC(grouping_type), " },
            { "SampleEntry()",                            "stream.ReadBox(boxSize, readSize, this, " },
            { "SampleConstructor()",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "InlineConstructor()",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "SampleConstructorFromTrackGroup()",        "stream.ReadBox(boxSize, readSize, this, " },
            { "NALUStartInlineConstructor()",             "stream.ReadBox(boxSize, readSize, this, " },
            { "MPEG4BitRateBox",                        "stream.ReadBox(boxSize, readSize, this, " },
            { "ChannelLayout()",                          "stream.ReadBox(boxSize, readSize, this, " },
            { "UniDrcConfigExtension()",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "SamplingRateBox()",                        "stream.ReadBox(boxSize, readSize, this, " },
            { "TextConfigBox()",                          "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataKeyTableBox",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "BitRateBox",                             "stream.ReadBox(boxSize, readSize, this, " },
            { "CompleteTrackInfoBox",                   "stream.ReadBox(boxSize, readSize, this, " },
            { "TierDependencyBox()",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "InitialParameterSetBox",                 "stream.ReadBox(boxSize, readSize, this, " },
            { "PriorityRangeBox()",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "ViewPriorityBox",                        "stream.ReadBox(boxSize, readSize, this, " },
            { "SVCDependencyRangeBox",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "RectRegionBox",                          "stream.ReadBox(boxSize, readSize, this, " },
            { "IroiInfoBox",                            "stream.ReadBox(boxSize, readSize, this, " },
            { "TranscodingInfoBox",                     "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataKeyDeclarationBox()",              "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataDatatypeBox()",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataLocaleBox()",                      "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataSetupBox()",                       "stream.ReadBox(boxSize, readSize, this, " },
            { "MetaDataExtensionsBox()",                  "stream.ReadBox(boxSize, readSize, this, " },
            { "TrackLoudnessInfo[]",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "AlbumLoudnessInfo[]",                    "stream.ReadBox(boxSize, readSize, this, " },
            { "VvcPTLRecord(num_sublayers)",            "stream.ReadClass(boxSize, readSize, this, new VvcPTLRecord(num_sublayers)," },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "stream.ReadClass(boxSize, readSize, this, new VvcPTLRecord((byte)(ptl_max_temporal_id[i]+1)), " },
            { "OpusSpecificBox",                        "stream.ReadBox(boxSize, readSize, this, " },
            { "unsigned int(8 * OutputChannelCount)",   "stream.ReadUInt8Array((uint)OutputChannelCount, " },
            { "ChannelMappingTable(OutputChannelCount)",                    "stream.ReadClass(boxSize, readSize, this, new ChannelMappingTable(_OutputChannelCount), " },
            // descriptors
            { "DecoderConfigDescriptor",                "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "SLConfigDescriptor",                     "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "IPI_DescrPointer[0 .. 1]",               "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "IP_IdentificationDataSet[0 .. 255]",     "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "IPMP_DescriptorPointer[0 .. 255]",       "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "LanguageDescriptor[0 .. 255]",           "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "QoS_Descriptor[0 .. 1]",                 "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "ES_Descriptor",                          "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "RegistrationDescriptor[0 .. 1]",         "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "ExtensionDescriptor[0 .. 255]",          "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "DecoderSpecificInfo[0 .. 1]",            "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "bit(8)[URLlength]",                      "stream.ReadUInt8Array((uint)URLlength, " },
            { "bit(8)[sizeOfInstance-4]",               "stream.ReadUInt8Array((uint)(sizeOfInstance - 4), " },
            { "bit(8)[sizeOfInstance-3]",               "stream.ReadUInt8Array((uint)(sizeOfInstance - 3), " },
            { "bit(8)[size-10]",                        "stream.ReadUInt8Array((uint)(size - 10), " },
            { "double(32)",                             "stream.ReadDouble32(" },
            { "fixedpoint1616",                         "stream.ReadFixedPoint1616(" },
            { "QoS_Qualifier[]",                        "stream.ReadDescriptor(boxSize, readSize, this, " },
            { "GetAudioObjectType()",                   "stream.ReadClass(boxSize, readSize, this, new GetAudioObjectType(), " },
            { "bslbf(header_size * 8)[]",               "stream.ReadBslbf(header_size * 8, " },
            { "bslbf(trailer_size * 8)[]",              "stream.ReadBslbf(trailer_size * 8, " },
            { "bslbf(aux_size * 8)[]",                  "stream.ReadBslbf(aux_size * 8, " },
            { "bslbf(11)",                              "stream.ReadBslbf(11, " },
            { "bslbf(5)",                               "stream.ReadBslbf(5, " },
            { "bslbf(4)",                               "stream.ReadBslbf(4, " },
            { "bslbf(2)",                               "stream.ReadBslbf(2, " },
            { "bslbf(1)",                               "stream.ReadBslbf(" },
            { "uimsbf(32)",                             "stream.ReadUimsbf(32, " },
            { "uimsbf(24)",                             "stream.ReadUimsbf(24, " },
            { "uimsbf(18)",                             "stream.ReadUimsbf(18, " },
            { "uimsbf(16)",                             "stream.ReadUimsbf(16, " },
            { "uimsbf(14)",                             "stream.ReadUimsbf(14, " },
            { "uimsbf(12)",                             "stream.ReadUimsbf(12, " },
            { "uimsbf(10)",                             "stream.ReadUimsbf(10, " },
            { "uimsbf(8)",                              "stream.ReadUimsbf(8, " },
            { "uimsbf(7)",                              "stream.ReadUimsbf(7, " },
            { "uimsbf(6)",                              "stream.ReadUimsbf(6, " },
            { "uimsbf(5)",                              "stream.ReadUimsbf(5, " },
            { "uimsbf(4)",                              "stream.ReadUimsbf(4, " },
            { "uimsbf(3)",                              "stream.ReadUimsbf(3, " },
            { "uimsbf(2)",                              "stream.ReadUimsbf(2, " },
            { "uimsbf(1)",                              "stream.ReadUimsbf(" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new GASpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType.AudioObjectType), " },
            { "case 8:\r\n      CelpSpecificConfig()",  "case 8:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new CelpSpecificConfig(samplingFrequencyIndex), " },
            { "case 9:\r\n      HvxcSpecificConfig()",  "case 9:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new HvxcSpecificConfig(), " },
            { "case 12:\r\n      TTSSpecificConfig()",  "case 12:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new TTSSpecificConfig(), " },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new StructuredAudioSpecificConfig(), " },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "case 24:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new ErrorResilientCelpSpecificConfig(samplingFrequencyIndex), " },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "case 25:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new ErrorResilientHvxcSpecificConfig(), " },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "case 26:\r\n    case 27:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new ParametricSpecificConfig(), " },
            { "case 28:\r\n      SSCSpecificConfig()",  "case 28:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new SSCSpecificConfig(channelConfiguration), " },
            { "case 30:\r\n      uimsbf(1)", "case 30:\r\n      boxSize += stream.ReadUimsbf(" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "case 32:\r\n    case 33:\r\n    case 34:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new MPEG_1_2_SpecificConfig(), " },
            { "case 35:\r\n      DSTSpecificConfig()",  "case 35:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new DSTSpecificConfig(channelConfiguration), " },
            { "case 36:\r\n      bslbf(5)", "case 36:\r\n      boxSize += stream.ReadBslbf(5, " },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "case 37:\r\n    case 38:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new SLSSpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType.AudioObjectType), " },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "case 39:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new ELDSpecificConfig(channelConfiguration), " },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "case 40:\r\n    case 41:\r\n      boxSize += stream.ReadClass(boxSize, readSize, this, new SymbolicMusicSpecificConfig(), " },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      boxSize += stream.ReadBslbf(2, " },
            { "SpatialSpecificConfig",                  "stream.ReadClass(boxSize, readSize, this, new SpatialSpecificConfig(), " },
            { "ALSSpecificConfig",                      "stream.ReadClass(boxSize, readSize, this, new ALSSpecificConfig(), " },
            { "ErrorProtectionSpecificConfig",          "stream.ReadClass(boxSize, readSize, this, new ErrorProtectionSpecificConfig(), " },
            { "program_config_element",                 "stream.ReadClass(boxSize, readSize, this, new program_config_element(), " },
            { "byte_alignment",                         "stream.ReadByteAlignment(" },
            { "CelpHeader(samplingFrequencyIndex)",     "stream.ReadClass(boxSize, readSize, this, new CelpHeader(samplingFrequencyIndex), " },
            { "CelpBWSenhHeader",                       "stream.ReadClass(boxSize, readSize, this, new CelpBWSenhHeader(), " },
            { "HVXCconfig",                             "stream.ReadClass(boxSize, readSize, this, new HVXCconfig(), " },
            { "TTS_Sequence",                           "stream.ReadClass(boxSize, readSize, this, new TTS_Sequence(), " },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "stream.ReadClass(boxSize, readSize, this, new ER_SC_CelpHeader(samplingFrequencyIndex), " },
            { "ErHVXCconfig",                           "stream.ReadClass(boxSize, readSize, this, new ErHVXCconfig(), " },
            { "PARAconfig",                             "stream.ReadClass(boxSize, readSize, this, new PARAconfig(), " },
            { "HILNenexConfig",                         "stream.ReadClass(boxSize, readSize, this, new HILNenexConfig(), " },
            { "HILNconfig",                             "stream.ReadClass(boxSize, readSize, this, new HILNconfig(), " },
            { "ld_sbr_header(channelConfiguration)",                          "stream.ReadClass(boxSize, readSize, this, new ld_sbr_header(channelConfiguration), " },
            { "sbr_header",                             "stream.ReadClass(boxSize, readSize, this, new sbr_header(), " },
            { "uimsbf(1)[i]",                           "stream.ReadUimsbf(" },
            { "uimsbf(8)[i]",                           "stream.ReadUimsbf(8, " },
            { "bslbf(1)[i]",                            "stream.ReadBslbf(1, " },
            { "uimsbf(4)[i]",                           "stream.ReadUimsbf(4, " },
            { "uimsbf(1)[c]",                           "stream.ReadUimsbf(" },
            { "uimsbf(32)[f]",                          "stream.ReadUimsbf(32, " },
            { "CelpHeader",                             "stream.ReadClass(boxSize, readSize, this, new CelpHeader(samplingFrequencyIndex), " },
            { "ER_SC_CelpHeader",                       "stream.ReadClass(boxSize, readSize, this, new ER_SC_CelpHeader(samplingFrequencyIndex), " },
            { "uimsbf(6)[i]",                           "stream.ReadUimsbf(6, " },
            { "uimsbf(1)[i][j]",                        "stream.ReadUimsbf(" },
            { "uimsbf(2)[i][j]",                        "stream.ReadUimsbf(2, " },
            { "uimsbf(4)[i][j]",                        "stream.ReadUimsbf(4, " },
            { "uimsbf(16)[i][j]",                       "stream.ReadUimsbf(16, " },
            { "uimsbf(7)[i][j]",                        "stream.ReadUimsbf(7, " },
            { "uimsbf(5)[i][j]",                        "stream.ReadUimsbf(5, " },
            { "uimsbf(6)[i][j]",                        "stream.ReadUimsbf(6, " },
            { "AV1SampleEntry",                         "stream.ReadBox(boxSize, readSize, this, " },
            { "AV1CodecConfigurationBox",               "stream.ReadBox(boxSize, readSize, this, " },
            { "AV1CodecConfigurationRecord",            "stream.ReadClass(boxSize, readSize, this, new AV1CodecConfigurationRecord(), " },
            { "vluimsbf8",                              "stream.ReadUimsbf(8, " },
            { "byte(urlMIDIStream_length)",             "stream.ReadUInt8Array((uint)urlMIDIStream_length, " },
            { "aligned bit(3)",                         "stream.ReadAlignedBits(3, " },
            { "aligned bit(1)",                         "stream.ReadAlignedBits(1, " },
            { "bit",                                    "stream.ReadBit(" },
            { "case 0b000:\r\n                mainscore_file", "case 0b000:\r\n                boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b001:\r\n                bit(8) partID; // ID of the part at which the following info refers \r\n                part_file", "case 0b001:\r\n               boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b010:\r\n                // this segment is always in binary as stated in Section 9 \r\n                synch_file", "case 0b010:\r\n                // this segment is always in binary as stated in Section 9 \r\n                boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b011:\r\n                format_file", "case 0b011:\r\n                boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b100:\r\n                bit(8) partID;\r\n                bit(8) lyricID;\r\n                lyrics_file", "case 0b100:\r\n                boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b101:\r\n                // this segment is always in binary as stated in Section 11.4 \r\n                font_file", "case 0b101:\r\n                // this segment is always in binary as stated in Section 11.4 \r\n                boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b110: ",                           "case 0b110: boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "case 0b111: ",                           "case 0b111: boxSize += stream.ReadBits((uint)(8 * chunk_length), " },
            { "unsigned int(16)[3]",                    "stream.ReadUInt16Array(3, " },
            { "MultiLanguageString[]",                  "stream.ReadStringSizeLangPrefixed(boxSize, readSize, " },
            { "AdobeChapterRecord[]",                   "stream.ReadClass(boxSize, readSize, this, " },
            { "ThreeGPPKeyword[]",                      "stream.ReadClass(boxSize, readSize, this, " },
            { "IodsSample[]",                           "stream.ReadClass(boxSize, readSize, this, " },
            { "XtraTag[]",                              "stream.ReadClass(boxSize, readSize, this, " },
            { "XtraValue[count]",                       "stream.ReadClass(boxSize, readSize, this, " },
            { "ViprEntry[]",                            "stream.ReadClass(boxSize, readSize, this, " },
            { "TrickPlayEntry[]",                       "stream.ReadClass(boxSize, readSize, this, " },
            { "MtdtEntry[ entry_count ]",               "stream.ReadClass(boxSize, readSize, this, " },
            { "RectRecord",                             "stream.ReadClass(boxSize, readSize, this, new RectRecord(), " },
            { "StyleRecord",                            "stream.ReadClass(boxSize, readSize, this, new StyleRecord(), " },
            { "ProtectionSystemSpecificKeyID[count]",   "stream.ReadClass(boxSize, readSize, this, (uint)count, " },
            { "unsigned int(8)[contentIDLength]",       "stream.ReadUInt8Array((uint)contentIDLength, " },
            { "unsigned int(8)[contentTypeLength]",     "stream.ReadUInt8Array((uint)contentTypeLength, " },
            { "unsigned int(8)[rightsIssuerLength]",    "stream.ReadUInt8Array((uint)rightsIssuerLength, " },
            { "unsigned int(8)[textualHeadersLength]",  "stream.ReadUInt8Array((uint)textualHeadersLength, " },
            { "unsigned int(8)[count]",                 "stream.ReadUInt8Array((uint)count, " },
            { "unsigned int(8)[4]",                     "stream.ReadUInt8Array((uint)4, " },
            { "unsigned int(8)[14]",                    "stream.ReadUInt8Array((uint)14, " },
            { "unsigned int(8)[6]",                     "stream.ReadUInt8Array((uint)6, " },
            { "unsigned int(8)[256]",                   "stream.ReadUInt8Array((uint)256, " },
            { "unsigned int(8)[512]",                   "stream.ReadUInt8Array((uint)512, " },
            { "char[tagLength]",                        "stream.ReadUInt8Array((uint)tagLength, " },
            { "unsigned int(8)[constant_IV_size]",      "stream.ReadUInt8Array((uint)constant_IV_size, " },
            { "unsigned int(8)[length-6]",              "stream.ReadUInt8Array((uint)(length-6), " },
            { "unsigned int(Per_Sample_IV_Size*8)",     "stream.ReadUInt8Array((uint)Per_Sample_IV_Size, " },
            { "EC3SpecificEntry[numIndSub + 1]",        "stream.ReadClass(boxSize, readSize, this, (uint)numIndSub + 1, " },
            { "SampleEncryptionSample(version, flags, Per_Sample_IV_Size)[sample_count]", "stream.ReadClass(boxSize, readSize, this, (uint)sample_count, () => new SampleEncryptionSample(version, flags, Per_Sample_IV_Size), " },
            { "SampleEncryptionSubsample(version)[subsample_count]", "stream.ReadClass(boxSize, readSize, this, (uint)subsample_count, () => new SampleEncryptionSubsample(version), " },
            { "CelpSpecificConfig()",                   "stream.ReadClass(boxSize, readSize, this, new CelpSpecificConfig(samplingFrequencyIndex), " },
            { "HvxcSpecificConfig()",                   "stream.ReadClass(boxSize, readSize, this, new HvxcSpecificConfig(), " },
            { "TTSSpecificConfig()",                    "stream.ReadClass(boxSize, readSize, this, new TTSSpecificConfig(), " },
            { "ErrorResilientCelpSpecificConfig()",     "stream.ReadClass(boxSize, readSize, this, new ErrorResilientCelpSpecificConfig(samplingFrequencyIndex), " },
            { "ErrorResilientHvxcSpecificConfig()",     "stream.ReadClass(boxSize, readSize, this, new ErrorResilientHvxcSpecificConfig(), " },
            { "SSCSpecificConfig()",                    "stream.ReadClass(boxSize, readSize, this, new SSCSpecificConfig(channelConfiguration), " },
            { "DSTSpecificConfig()",                    "stream.ReadClass(boxSize, readSize, this, new DSTSpecificConfig(channelConfiguration), " },
            { "ELDSpecificConfig(channelConfiguration)","stream.ReadClass(boxSize, readSize, this, new ELDSpecificConfig(channelConfiguration), " },
            { "GASpecificConfig()",                     "stream.ReadClass(boxSize, readSize, this, new GASpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType.AudioObjectType), " },
            { "StructuredAudioSpecificConfig()",        "stream.ReadClass(boxSize, readSize, this, new StructuredAudioSpecificConfig(), " },
            { "ParametricSpecificConfig()",             "stream.ReadClass(boxSize, readSize, this, new ParametricSpecificConfig(), " },
            { "MPEG_1_2_SpecificConfig()",              "stream.ReadClass(boxSize, readSize, this, new MPEG_1_2_SpecificConfig(), " },
            { "SLSSpecificConfig()",                    "stream.ReadClass(boxSize, readSize, this, new SLSSpecificConfig(samplingFrequencyIndex, channelConfiguration, audioObjectType.AudioObjectType), " },
            { "SymbolicMusicSpecificConfig()",          "stream.ReadClass(boxSize, readSize, this, new SymbolicMusicSpecificConfig(), " },
        };

        if(map.ContainsKey(type))
            return map[type];
        else if(map.ContainsKey(type.Replace("()","")))
            return map[type.Replace("()", "")];
        else
            throw new NotSupportedException(type);
    }

    private static string GetCalculateSizeMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)[ entry_count ]",        "(ulong)entry_count * 64" },
            { "unsigned int(64)",                       "64" },
            { "unsigned int(48)",                       "48" },
            { "unsigned int(32)[ entry_count ]",        "IsoStream.CalculateSize((ulong)entry_count, value, 32)" },
            { "template int(32)[9]",                    "9 * 32" },
            { "unsigned int(32)[3]",                    "3 * 32" },
            { "unsigned int(32)",                       "32" },
            { "unsigned int(31)",                       "31" },
            { "unsigned_int(32)",                       "32" },
            { "unsigned int(24)",                       "24" },
            { "unsigned int(29)",                       "29" },
            { "unsigned int(28)",                       "28" },
            { "unsigned int(26)",                       "26" },
            { "unsigned int(16)[i]",                    "16" },
            { "unsigned int(16)[j]",                    "16" },
            { "unsigned int(16)[i][j]",                 "16" },
            { "unsigned int(16)",                       "16" },
            { "unsigned_int(16)",                       "16" },
            { "unsigned int(15)",                       "15" },
            { "unsigned int(12)",                       "12" },
            { "signed int(12)",                         "12" },
            { "unsigned int(10)[i][j]",                 "10" },
            { "unsigned int(10)[i]",                    "10" },
            { "unsigned int(10)",                       "10" },
            { "unsigned int(8)[ sample_count ]",        "(ulong)sample_count * 8" },
            { "unsigned int(8)[length]",                "(ulong)length * 8" },
            { "unsigned int(8)[32]",                    "32 * 8" },
            { "unsigned int(8)[36]",                    "36 * 8" },
            { "unsigned int(8)[20]",                    "20 * 8" },
            { "unsigned int(8)[16]",                    "16 * 8" },
            { "unsigned int(9)",                        "9" },
            { "unsigned int(8)",                        "8" },
            { "unsigned int(7)",                        "7" },
            { "unsigned int(6)",                        "6" },
            { "unsigned int(5)[3]",                     "15" }, // Iso639
            { "unsigned int(5)",                        "5" },
            { "unsigned int(4)",                        "4" },
            { "unsigned int(3)",                        "3" },
            { "unsigned int(2)[i][j]",                  "2" },
            { "unsigned int(2)",                        "2" },
            { "unsigned int(1)[i]",                     "1" },
            { "unsigned int(1)",                        "1" },
            { "unsigned int (32)",                      "32" },
            { "unsigned int(f(pattern_size_code))[i]",  "(ulong)pattern_size_code" },
            { "unsigned int(f(index_size_code))[i]",    "(ulong)index_size_code" },
            { "unsigned int(f(index_size_code))[j][k]", "(ulong)index_size_code" },
            { "unsigned int(f(count_size_code))[i]",    "(ulong)count_size_code" },
            { "unsigned int(base_offset_size*8)",       "(ulong)base_offset_size * 8" },
            { "unsigned int(offset_size*8)",            "(ulong)offset_size * 8" },
            { "unsigned int(length_size*8)",            "(ulong)length_size * 8" },
            { "unsigned int(index_size*8)",             "(ulong)index_size * 8" },
            { "unsigned int(field_size)",               "(ulong)field_size" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "(ulong)(length_size_of_traf_num+1) * 8" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "(ulong)(length_size_of_trun_num+1) * 8" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "(ulong)(length_size_of_sample_num+1) * 8" },
            { "unsigned int(8*size-64)",                "(ulong)(size - 8) * 8" },
            { "bit(8)[chunk_length]",                "(ulong)(chunk_length * 8)" },
            { "unsigned int(subgroupIdLen)[i]",         "32" },
            { "const unsigned int(8)[6]",               "6 * 8" },
            { "const unsigned int(32)[2]",              "2 * 32" },
            { "const unsigned int(32)[3]",              "3 * 32" },
            { "const unsigned int(32)",                 "32" },
            { "const unsigned int(16)[3]",              "3 * 16" },
            { "const unsigned int(16)",                 "16" },
            { "const unsigned int(26)",                 "26" },
            { "template int(32)",                       "32" },
            { "template int(16)",                       "16" },
            { "template unsigned int(30)",              "30" },
            { "template unsigned int(32)",              "32" },
            { "template unsigned int(16)[3]",           "3 * 16" },
            { "template unsigned int(16)",              "16" },
            { "template unsigned int(8)[]",             "(ulong)value.Length * 8" },
            { "template unsigned int(8)",               "8" },
            { "int(64)",                                "64" },
            { "int(32)",                                "32" },
            { "int(16)",                                "16" },
            { "int(8)",                                 "8" },
            { "int(4)",                                 "4" },
            { "int",                                    "32" },
            { "const bit(16)",                          "16" },
            { "const bit(1)",                           "1" },
            { "bit(1)",                                 "1" },
            { "bit(2)",                                 "2" },
            { "bit(3)",                                 "3" },
            { "bit(length-3)",                          "(ulong)(length-3)" },
            { "bit(length)",                            "(ulong)length" },
            { "bit(4)",                                 "4" },
            { "bit(5)",                                 "5" },
            { "bit(6)",                                 "6" },
            { "bit(7)",                                 "7" },
            { "bit(8)[]",                               "8 * (ulong)value.Length" },
            { "bit(8)",                                 "8" },
            { "bit(9)",                                 "9" },
            { "bit(13)",                                "13" },
            { "bit(14)",                                "14" },
            { "bit(15)",                                "15" },
            { "bit(16)[i]",                             "16" },
            { "bit(16)",                                "16" },
            { "bit(24)",                                "24" },
            { "bit(31)",                                "31" },
            { "bit(8 ceil(size / 8) \u2013 size)",      "(ulong)(Math.Ceiling(size / 8d) - size) * 8" },
            { "bit(8* ps_nalu_length)",                 "(ulong)ps_nalu_length * 8" },
            { "bit(8*nalUnitLength)",                   "(ulong)nalUnitLength * 8" },
            { "bit(8*sequenceParameterSetLength)",      "(ulong)sequenceParameterSetLength * 8" },
            { "bit(8*pictureParameterSetLength)",       "(ulong)pictureParameterSetLength * 8" },
            { "bit(8*sequenceParameterSetExtLength)",   "(ulong)sequenceParameterSetExtLength * 8" },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "(uint)(8*num_bytes_constraint_info - 2)" },
            { "bit(8*nal_unit_length)",                 "(ulong)nal_unit_length * 8" },
            { "bit(timeStampLength)",                   "(ulong)timeStampLength" },
            { "utf8string",                             "IsoStream.CalculateStringSize(value)" },
            { "utfstring",                              "IsoStream.CalculateStringSize(value)" },
            { "utf8list",                               "IsoStream.CalculateStringSize(value)" },
            { "boxstring",                              "IsoStream.CalculateStringSize(value)" },
            { "string",                                 "IsoStream.CalculateStringSize(value)" },
            { "bit(32)[6]",                             "6 * 32" },
            { "bit(32)",                                "32" },
            { "uint(32)",                               "32" },
            { "uint(16)",                               "16" },
            { "uint(64)",                               "64" },
            { "uint(8)[32]",                            "32 * 8" },  // compressor_name
            { "uint(8)",                                "8" },
            { "uint(7)",                                "7" },
            { "uint(1)",                                "1" },
            { "signed int(32)",                         "32" },
            { "signed int (16)",                        "16" },
            { "signed int(16)[grid_pos_view_id[i]]",    "16" },
            { "signed int(16)",                         "16" },
            { "signed int (8)",                         "8" },
            { "signed int(64)",                         "64" },
            { "signed int(8)",                        "8" },
            { "Box()[]",                                "IsoStream.CalculateBoxSize(value)" },
            { "Box[]",                                  "IsoStream.CalculateBoxSize(value)" },
            { "Box()",                                    "IsoStream.CalculateBoxSize(value)" },
            { "Box",                                    "IsoStream.CalculateBoxSize(value)" },
            { "SchemeTypeBox",                          "IsoStream.CalculateBoxSize(value)" },
            { "SchemeInformationBox",                   "IsoStream.CalculateBoxSize(value)" },
            { "ItemPropertyContainerBox",               "IsoStream.CalculateBoxSize(value)" },
            { "ItemPropertyAssociationBox",             "IsoStream.CalculateBoxSize(value)" },
            { "ItemPropertyAssociationBox[]",           "IsoStream.CalculateBoxSize(value)" },
            { "char",                                   "8" },
            { "ICC_profile",                            "IsoStream.CalculateClassSize(value)" },
            { "OriginalFormatBox(fmt)",                 "IsoStream.CalculateBoxSize(value)" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "IsoStream.CalculateBoxSize(value)" },
            { "ItemInfoEntry[ entry_count ]",           "IsoStream.CalculateBoxSize(value)" },
            { "TypeCombinationBox[]",                   "IsoStream.CalculateBoxSize(value)" },
            { "FilePartitionBox",                       "IsoStream.CalculateBoxSize(value)" },
            { "FECReservoirBox",                        "IsoStream.CalculateBoxSize(value)" },
            { "FileReservoirBox",                       "IsoStream.CalculateBoxSize(value)" },
            { "PartitionEntry[ entry_count ]",          "IsoStream.CalculateBoxSize(value)" },
            { "FDSessionGroupBox",                      "IsoStream.CalculateBoxSize(value)" },
            { "GroupIdToNameBox",                       "IsoStream.CalculateBoxSize(value)" },
            { "base64string",                           "IsoStream.CalculateStringSize(value)" },
            { "ProtectionSchemeInfoBox",                "IsoStream.CalculateBoxSize(value)" },
            { "SingleItemTypeReferenceBox",             "IsoStream.CalculateBoxSize(value)" },
            { "SingleItemTypeReferenceBox[]",           "IsoStream.CalculateBoxSize(value)" },
            { "SingleItemTypeReferenceBoxLarge",        "IsoStream.CalculateBoxSize(value)" },
            { "SingleItemTypeReferenceBoxLarge[]",      "IsoStream.CalculateBoxSize(value)" },
            { "HandlerBox(handler_type)",               "IsoStream.CalculateBoxSize(value)" },
            { "PrimaryItemBox",                         "IsoStream.CalculateBoxSize(value)" },
            { "DataInformationBox",                     "IsoStream.CalculateBoxSize(value)" },
            { "ItemLocationBox",                        "IsoStream.CalculateBoxSize(value)" },
            { "ItemProtectionBox",                      "IsoStream.CalculateBoxSize(value)" },
            { "ItemInfoBox",                            "IsoStream.CalculateBoxSize(value)" },
            { "IPMPControlBox",                         "IsoStream.CalculateBoxSize(value)" },
            { "ItemReferenceBox",                       "IsoStream.CalculateBoxSize(value)" },
            { "ItemDataBox",                            "IsoStream.CalculateBoxSize(value)" },
            { "TrackReferenceTypeBox[]",               "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataKeyBox[]",                       "IsoStream.CalculateBoxSize(value)" },
            { "TierInfoBox()",                            "IsoStream.CalculateBoxSize(value)" },
            { "TierInfoBox",                            "IsoStream.CalculateBoxSize(value)" },
            { "MultiviewRelationAttributeBox",          "IsoStream.CalculateBoxSize(value)" },
            { "TierBitRateBox()",                         "IsoStream.CalculateBoxSize(value)" },
            { "TierBitRateBox",                         "IsoStream.CalculateBoxSize(value)" },
            { "BufferingBox()",                           "IsoStream.CalculateBoxSize(value)" },
            { "BufferingBox",                           "IsoStream.CalculateBoxSize(value)" },
            { "MultiviewSceneInfoBox",                  "IsoStream.CalculateBoxSize(value)" },
            { "MVDDecoderConfigurationRecord",          "IsoStream.CalculateClassSize(value)" },
            { "MVDDepthResolutionBox",                  "IsoStream.CalculateBoxSize(value)" },
            { "MVCDecoderConfigurationRecord()",        "IsoStream.CalculateClassSize(value)" },
            { "AVCDecoderConfigurationRecord()",        "IsoStream.CalculateClassSize(value)" },
            { "HEVCDecoderConfigurationRecord()",       "IsoStream.CalculateClassSize(value)" },
            { "LHEVCDecoderConfigurationRecord()",      "IsoStream.CalculateClassSize(value)" },
            { "SVCDecoderConfigurationRecord()",        "IsoStream.CalculateClassSize(value)" },
            { "HEVCTileTierLevelConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "EVCDecoderConfigurationRecord()",        "IsoStream.CalculateClassSize(value)" },
            { "VvcDecoderConfigurationRecord()",        "IsoStream.CalculateClassSize(value)" },
            { "EVCSliceComponentTrackConfigurationRecord()", "IsoStream.CalculateClassSize(value)" },
            { "Descriptor[0 .. 255]",                   "IsoStream.CalculateDescriptorSize(value)" },
            { "Descriptor[count]",                      "IsoStream.CalculateDescriptorSize(value)" },
            { "Descriptor",                             "IsoStream.CalculateDescriptorSize(value)" },
            { "WebVTTConfigurationBox",                 "IsoStream.CalculateBoxSize(value)" },
            { "WebVTTSourceLabelBox",                   "IsoStream.CalculateBoxSize(value)" },
            { "OperatingPointsRecord",                  "IsoStream.CalculateClassSize(value)" },
            { "VvcSubpicIDEntry",                       "IsoStream.CalculateEntrySize(value)" },
            { "VvcSubpicOrderEntry",                    "IsoStream.CalculateEntrySize(value)" },
            { "URIInitBox",                             "IsoStream.CalculateBoxSize(value)" },
            { "URIBox",                                 "IsoStream.CalculateBoxSize(value)" },
            { "CleanApertureBox",                       "IsoStream.CalculateBoxSize(value)" },
            { "PixelAspectRatioBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "DownMixInstructions()[]",               "IsoStream.CalculateBoxSize(value)" },
            { "DRCCoefficientsBasic()[]",              "IsoStream.CalculateBoxSize(value)" },
            { "DRCInstructionsBasic()[]",              "IsoStream.CalculateBoxSize(value)" },
            { "DRCCoefficientsUniDRC()[]",             "IsoStream.CalculateBoxSize(value)" },
            { "DRCInstructionsUniDRC()[]",             "IsoStream.CalculateBoxSize(value)" },
            { "HEVCConfigurationBox",                   "IsoStream.CalculateBoxSize(value)" },
            { "LHEVCConfigurationBox",                  "IsoStream.CalculateBoxSize(value)" },
            { "AVCConfigurationBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "SVCConfigurationBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "ScalabilityInformationSEIBox",           "IsoStream.CalculateBoxSize(value)" },
            { "SVCPriorityAssignmentBox",               "IsoStream.CalculateBoxSize(value)" },
            { "ViewScalabilityInformationSEIBox",       "IsoStream.CalculateBoxSize(value)" },
            { "ViewIdentifierBox()",                      "IsoStream.CalculateBoxSize(value)" },
            { "MVCConfigurationBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "MVCViewPriorityAssignmentBox",           "IsoStream.CalculateBoxSize(value)" },
            { "IntrinsicCameraParametersBox",           "IsoStream.CalculateBoxSize(value)" },
            { "ExtrinsicCameraParametersBox",           "IsoStream.CalculateBoxSize(value)" },
            { "MVCDConfigurationBox",                   "IsoStream.CalculateBoxSize(value)" },
            { "MVDScalabilityInformationSEIBox",        "IsoStream.CalculateBoxSize(value)" },
            { "A3DConfigurationBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "VvcOperatingPointsRecord",               "IsoStream.CalculateClassSize(value)" },
            { "VVCSubpicIDRewritingInfomationStruct()", "IsoStream.CalculateClassSize(value)" },
            { "MPEG4ExtensionDescriptorsBox ()",        "IsoStream.CalculateBoxSize(value)" },
            { "MPEG4ExtensionDescriptorsBox()",         "IsoStream.CalculateBoxSize(value)" },
            { "MPEG4ExtensionDescriptorsBox",           "IsoStream.CalculateBoxSize(value)" },
            { "bit(8*dci_nal_unit_length)",             "(ulong)dci_nal_unit_length * 8" },
            { "DependencyInfo[numReferences]",          "IsoStream.CalculateClassSize(value)" },
            { "VvcPTLRecord(0)[i]",                     "IsoStream.CalculateClassSize(value)" },
            { "EVCSliceComponentTrackConfigurationBox", "IsoStream.CalculateBoxSize(value)" },
            { "SVCMetadataSampleConfigBox",             "IsoStream.CalculateBoxSize(value)" },
            { "SVCPriorityLayerInfoBox",                "IsoStream.CalculateBoxSize(value)" },
            { "EVCConfigurationBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "VvcNALUConfigBox",                       "IsoStream.CalculateBoxSize(value)" },
            { "VvcConfigurationBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "HEVCTileConfigurationBox",               "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataKeyTableBox()",                  "IsoStream.CalculateBoxSize(value)" },
            { "BitRateBox()",                          "IsoStream.CalculateBoxSize(value)" },
            { "char[count]",                            "(ulong)count * 8" },
            { "signed int(32)[ c ]",                    "32" },
            { "unsigned int(8)[]",                      "(ulong)value.Length * 8" },
            { "unsigned int(8)[i]",                     "8" },
            { "unsigned int(6)[i]",                     "6" },
            { "unsigned int(6)[i][j]",                  "6" },
            { "unsigned int(1)[i][j]",                  "1" },
            { "unsigned int(9)[i]",                     "9" },
            { "unsigned int(32)[]",                     "(ulong)value.Length * 32" },
            { "unsigned int(32)[i]",                    "32" },
            { "unsigned int(32)[j]",                    "32" },
            { "unsigned int(8)[j][k]",                  "8" },
            { "unsigned int(8)[j]",                     "8" },
            { "signed int(64)[j][k]",                 "64" },
            { "signed int(64)[j]",                    "64" },
            { "char[]",                                 "(ulong)value.Length * 8" },
            { "string[method_count]",                   "IsoStream.CalculateStringSize(value)" },
            { "ItemInfoExtension(extension_type)",                      "IsoStream.CalculateClassSize(value)" },
            { "SampleGroupDescriptionEntry(grouping_type)",            "IsoStream.CalculateEntrySize(value)" },
            { "SampleEntry()",                            "IsoStream.CalculateBoxSize(value)" },
            { "SampleConstructor()",                      "IsoStream.CalculateBoxSize(value)" },
            { "InlineConstructor()",                      "IsoStream.CalculateBoxSize(value)" },
            { "SampleConstructorFromTrackGroup()",        "IsoStream.CalculateBoxSize(value)" },
            { "NALUStartInlineConstructor()",             "IsoStream.CalculateBoxSize(value)" },
            { "MPEG4BitRateBox",                        "IsoStream.CalculateBoxSize(value)" },
            { "ChannelLayout()",                          "IsoStream.CalculateBoxSize(value)" },
            { "UniDrcConfigExtension()",                  "IsoStream.CalculateBoxSize(value)" },
            { "SamplingRateBox()",                        "IsoStream.CalculateBoxSize(value)" },
            { "TextConfigBox()",                          "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataKeyTableBox",                    "IsoStream.CalculateBoxSize(value)" },
            { "BitRateBox",                             "IsoStream.CalculateBoxSize(value)" },
            { "CompleteTrackInfoBox",                   "IsoStream.CalculateBoxSize(value)" },
            { "TierDependencyBox()",                      "IsoStream.CalculateBoxSize(value)" },
            { "InitialParameterSetBox",                 "IsoStream.CalculateBoxSize(value)" },
            { "PriorityRangeBox()",                       "IsoStream.CalculateBoxSize(value)" },
            { "ViewPriorityBox",                        "IsoStream.CalculateBoxSize(value)" },
            { "SVCDependencyRangeBox",                  "IsoStream.CalculateBoxSize(value)" },
            { "RectRegionBox",                          "IsoStream.CalculateBoxSize(value)" },
            { "IroiInfoBox",                            "IsoStream.CalculateBoxSize(value)" },
            { "TranscodingInfoBox",                     "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataKeyDeclarationBox()",              "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataDatatypeBox()",                    "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataLocaleBox()",                      "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataSetupBox()",                       "IsoStream.CalculateBoxSize(value)" },
            { "MetaDataExtensionsBox()",                  "IsoStream.CalculateBoxSize(value)" },
            { "TrackLoudnessInfo[]",                    "IsoStream.CalculateBoxSize(value)" },
            { "AlbumLoudnessInfo[]",                    "IsoStream.CalculateBoxSize(value)" },
            { "VvcPTLRecord(num_sublayers)",            "IsoStream.CalculateClassSize(value)" },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "IsoStream.CalculateClassSize(value)" },
            { "OpusSpecificBox",                        "IsoStream.CalculateBoxSize(value)" },
            { "unsigned int(8 * OutputChannelCount)",   "(ulong)(OutputChannelCount * 8)" },
            { "ChannelMappingTable(OutputChannelCount)",                    "IsoStream.CalculateClassSize(value)" },
            // descriptors
            { "DecoderConfigDescriptor",                "IsoStream.CalculateDescriptorSize(value)" },
            { "SLConfigDescriptor",                     "IsoStream.CalculateDescriptorSize(value)" },
            { "IPI_DescrPointer[0 .. 1]",               "IsoStream.CalculateDescriptorSize(value)" },
            { "IP_IdentificationDataSet[0 .. 255]",     "IsoStream.CalculateDescriptorSize(value)" },
            { "IPMP_DescriptorPointer[0 .. 255]",       "IsoStream.CalculateDescriptorSize(value)" },
            { "LanguageDescriptor[0 .. 255]",           "IsoStream.CalculateDescriptorSize(value)" },
            { "QoS_Descriptor[0 .. 1]",                 "IsoStream.CalculateDescriptorSize(value)" },
            { "ES_Descriptor",                          "IsoStream.CalculateDescriptorSize(value)" },
            { "RegistrationDescriptor[0 .. 1]",         "IsoStream.CalculateDescriptorSize(value)" },
            { "ExtensionDescriptor[0 .. 255]",          "IsoStream.CalculateDescriptorSize(value)" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "IsoStream.CalculateDescriptorSize(value)" },
            { "DecoderSpecificInfo[0 .. 1]",            "IsoStream.CalculateDescriptorSize(value)" },
            { "bit(8)[URLlength]",                      "(ulong)(URLlength * 8)" },
            { "bit(8)[sizeOfInstance-4]",               "(ulong)(sizeOfInstance - 4) * 8" },
            { "bit(8)[sizeOfInstance-3]",               "(ulong)(sizeOfInstance - 3) * 8" },
            { "bit(8)[size-10]",                        "(ulong)(size - 10) * 8" },
            { "double(32)",                             "32" },
            { "fixedpoint1616",                         "32" },
            { "QoS_Qualifier[]",                        "IsoStream.CalculateDescriptorSize(value)" },
            { "GetAudioObjectType()",                   "IsoStream.CalculateClassSize(value)" },
            { "bslbf(header_size * 8)[]",               "header_size * 8" },
            { "bslbf(trailer_size * 8)[]",              "trailer_size * 8" },
            { "bslbf(aux_size * 8)[]",                  "aux_size * 8" },
            { "bslbf(11)",                              "11" },
            { "bslbf(5)",                               "5" },
            { "bslbf(4)",                               "4" },
            { "bslbf(2)",                               "2" },
            { "bslbf(1)",                               "1" },
            { "uimsbf(32)",                             "32" },
            { "uimsbf(24)",                             "24" },
            { "uimsbf(18)",                             "18" },
            { "uimsbf(16)",                             "16" },
            { "uimsbf(14)",                             "14" },
            { "uimsbf(12)",                             "12" },
            { "uimsbf(10)",                             "10" },
            { "uimsbf(8)",                              "8" },
            { "uimsbf(7)",                              "7" },
            { "uimsbf(6)",                              "6" },
            { "uimsbf(5)",                              "5" },
            { "uimsbf(4)",                              "4" },
            { "uimsbf(3)",                              "3" },
            { "uimsbf(2)",                              "2" },
            { "uimsbf(1)",                              "1" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n     boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 8:\r\n      CelpSpecificConfig()", "case 8:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 9:\r\n      HvxcSpecificConfig()", "case 9:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 12:\r\n      TTSSpecificConfig()", "case 12:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "case 24:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "case 25:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "case 26:\r\n    case 27:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 28:\r\n      SSCSpecificConfig()", "case 28:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 30:\r\n      uimsbf(1)",            "boxSize += 1" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "case 32:\r\n    case 33:\r\n    case 34:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 35:\r\n      DSTSpecificConfig()", "case 35:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 36:\r\n      bslbf(5)",             "boxSize += 5" },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "case 37:\r\n    case 38:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "case 39:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "case 40:\r\n    case 41:\r\n      boxSize += IsoStream.CalculateClassSize(value)" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n     boxSize +=  2" },
            { "SpatialSpecificConfig",                  "IsoStream.CalculateClassSize(value)" },
            { "ALSSpecificConfig",                      "IsoStream.CalculateClassSize(value)" },
            { "ErrorProtectionSpecificConfig",          "IsoStream.CalculateClassSize(value)" },
            { "program_config_element",                 "IsoStream.CalculateClassSize(value)" },
            { "byte_alignment",                         "IsoStream.CalculateByteAlignmentSize(boxSize, value)" },
            { "CelpHeader(samplingFrequencyIndex)",     "IsoStream.CalculateClassSize(value)" },
            { "CelpBWSenhHeader",                       "IsoStream.CalculateClassSize(value)" },
            { "HVXCconfig",                             "IsoStream.CalculateClassSize(value)" },
            { "TTS_Sequence",                           "IsoStream.CalculateClassSize(value)" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "IsoStream.CalculateClassSize(value)" },
            { "ErHVXCconfig",                           "IsoStream.CalculateClassSize(value)" },
            { "PARAconfig",                             "IsoStream.CalculateClassSize(value)" },
            { "HILNenexConfig",                         "IsoStream.CalculateClassSize(value)" },
            { "HILNconfig",                             "IsoStream.CalculateClassSize(value)" },
            { "ld_sbr_header(channelConfiguration)",                          "IsoStream.CalculateClassSize(value)" },
            { "sbr_header",                             "IsoStream.CalculateClassSize(value)" },
            { "uimsbf(1)[i]",                           "1" },
            { "uimsbf(8)[i]",                           "8" },
            { "bslbf(1)[i]",                            "1" },
            { "uimsbf(4)[i]",                           "4" },
            { "uimsbf(1)[c]",                           "1" },
            { "uimsbf(32)[f]",                          "32" },
            { "CelpHeader",                             "IsoStream.CalculateClassSize(value)" },
            { "ER_SC_CelpHeader",                       "IsoStream.CalculateClassSize(value)" },
            { "uimsbf(6)[i]",                           "6" },
            { "uimsbf(1)[i][j]",                        "1" },
            { "uimsbf(2)[i][j]",                        "2" },
            { "uimsbf(4)[i][j]",                        "4" },
            { "uimsbf(16)[i][j]",                       "16" },
            { "uimsbf(7)[i][j]",                        "7" },
            { "uimsbf(5)[i][j]",                        "5" },
            { "uimsbf(6)[i][j]",                        "6" },
            { "AV1SampleEntry",                         "IsoStream.CalculateBoxSize(value)" },
            { "AV1CodecConfigurationBox",               "IsoStream.CalculateBoxSize(value)" },
            { "AV1CodecConfigurationRecord",            "IsoStream.CalculateClassSize(value)" },
            { "vluimsbf8",                              "8" },
            { "byte(urlMIDIStream_length)",             "(ulong)(urlMIDIStream_length * 8)" },
            { "aligned bit(3)",                         "(ulong)3" }, // TODO: calculate alignment
            { "aligned bit(1)",                         "(ulong)1" }, // TODO: calculate alignment
            { "bit",                                    "1" },
            { "case 0b000:\r\n                mainscore_file", "case 0b000:\r\n                boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b001:\r\n                bit(8) partID; // ID of the part at which the following info refers \r\n                part_file", "case 0b001:\r\n                boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b010:\r\n                // this segment is always in binary as stated in Section 9 \r\n                synch_file", "case 0b010:\r\n                boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b011:\r\n                format_file", "case 0b011:\r\n                boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b100:\r\n                bit(8) partID;\r\n                bit(8) lyricID;\r\n                lyrics_file", "case 0b100:\r\n                boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b101:\r\n                // this segment is always in binary as stated in Section 11.4 \r\n                font_file", "case 0b101:\r\n                boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b110: ",                           "case 0b110: boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "case 0b111: ",                           "case 0b111: boxSize += (ulong)((uint)(8 * chunk_length))" },
            { "unsigned int(16)[3]",                    "3 * 16" },
            { "MultiLanguageString[]",                  "IsoStream.CalculateStringSizeLangPrefixed(value)" },
            { "AdobeChapterRecord[]",                   "IsoStream.CalculateClassSize(value)" },
            { "ThreeGPPKeyword[]",                      "IsoStream.CalculateClassSize(value)" },
            { "IodsSample[]",                           "IsoStream.CalculateClassSize(value)" },
            { "XtraTag[]",                              "IsoStream.CalculateClassSize(value)" },
            { "XtraValue[count]",                       "IsoStream.CalculateClassSize(value)" },
            { "ViprEntry[]",                            "IsoStream.CalculateClassSize(value)" },
            { "TrickPlayEntry[]",                       "IsoStream.CalculateClassSize(value)" },
            { "MtdtEntry[ entry_count ]",               "IsoStream.CalculateClassSize(value)" },
            { "RectRecord",                             "IsoStream.CalculateClassSize(value)" },
            { "StyleRecord",                            "IsoStream.CalculateClassSize(value)" },
            { "ProtectionSystemSpecificKeyID[count]",   "IsoStream.CalculateClassSize(value)" },
            { "unsigned int(8)[contentIDLength]",       "(uint)contentIDLength * 8" },
            { "unsigned int(8)[contentTypeLength]",     "(uint)contentTypeLength * 8" },
            { "unsigned int(8)[rightsIssuerLength]",    "(uint)rightsIssuerLength * 8" },
            { "unsigned int(8)[textualHeadersLength]",  "(uint)textualHeadersLength * 8" },
            { "unsigned int(8)[count]",                 "(uint)count * 8" },
            { "unsigned int(8)[4]",                     "(uint)32" },
            { "unsigned int(8)[14]",                    "(uint)14 * 8" },
            { "unsigned int(8)[6]",                     "(uint)48" },
            { "unsigned int(8)[256]",                   "(uint)256 * 8" },
            { "unsigned int(8)[512]",                   "(uint)512 * 8" },
            { "char[tagLength]",                        "(uint)tagLength * 8" },
            { "unsigned int(8)[constant_IV_size]",      "(uint)constant_IV_size * 8" },
            { "unsigned int(8)[length-6]",              "(uint)(length-6) * 8" },
            { "EC3SpecificEntry[numIndSub + 1]",        "IsoStream.CalculateClassSize(value)" },
            { "SampleEncryptionSample(version, flags, Per_Sample_IV_Size)[sample_count]", "IsoStream.CalculateClassSize(value)" },
            { "SampleEncryptionSubsample(version)[subsample_count]", "IsoStream.CalculateClassSize(value)" },
            { "unsigned int(Per_Sample_IV_Size*8)",     "(uint)Per_Sample_IV_Size * 8" },
            { "CelpSpecificConfig()",                   "IsoStream.CalculateClassSize(value)" },
            { "HvxcSpecificConfig()",                   "IsoStream.CalculateClassSize(value)" },
            { "TTSSpecificConfig()",                    "IsoStream.CalculateClassSize(value)" },
            { "ErrorResilientCelpSpecificConfig()",     "IsoStream.CalculateClassSize(value)" },
            { "ErrorResilientHvxcSpecificConfig()",     "IsoStream.CalculateClassSize(value)" },
            { "SSCSpecificConfig()",                    "IsoStream.CalculateClassSize(value)" },
            { "DSTSpecificConfig()",                    "IsoStream.CalculateClassSize(value)" },
            { "ELDSpecificConfig(channelConfiguration)","IsoStream.CalculateClassSize(value)" },
            { "GASpecificConfig()",                     "IsoStream.CalculateClassSize(value)" },
            { "StructuredAudioSpecificConfig()",        "IsoStream.CalculateClassSize(value)" },
            { "ParametricSpecificConfig()",             "IsoStream.CalculateClassSize(value)" },
            { "MPEG_1_2_SpecificConfig()",              "IsoStream.CalculateClassSize(value)" },
            { "SLSSpecificConfig()",                    "IsoStream.CalculateClassSize(value)" },
            { "SymbolicMusicSpecificConfig()",          "IsoStream.CalculateClassSize(value)" },
       };
        if (map.ContainsKey(type))
            return map[type];
        else if (map.ContainsKey(type.Replace("()", "")))
            return map[type.Replace("()", "")];
        else
            throw new NotSupportedException(type);
    }

    private static string GetWriteMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)[ entry_count ]",        "stream.WriteUInt64Array(entry_count, " },
            { "unsigned int(64)",                       "stream.WriteUInt64(" },
            { "unsigned int(48)",                       "stream.WriteUInt48(" },
            { "template int(32)[9]",                    "stream.WriteUInt32Array(9, " },
            { "unsigned int(32)[ entry_count ]",        "stream.WriteUInt32Array(entry_count, " },
            { "unsigned int(32)[3]",                    "stream.WriteUInt32Array(3, " },
            { "unsigned int(32)",                       "stream.WriteUInt32(" },
            { "unsigned int(31)",                       "stream.WriteBits(31, " },
            { "unsigned_int(32)",                       "stream.WriteUInt32(" },
            { "unsigned int(24)",                       "stream.WriteUInt24(" },
            { "unsigned int(29)",                       "stream.WriteBits(29, " },
            { "unsigned int(28)",                       "stream.WriteBits(28, " },
            { "unsigned int(26)",                       "stream.WriteBits(26, " },
            { "unsigned int(16)[i]",                    "stream.WriteUInt16(" },
            { "unsigned int(16)[j]",                    "stream.WriteUInt16(" },
            { "unsigned int(16)[i][j]",                 "stream.WriteUInt16(" },
            { "unsigned int(16)",                       "stream.WriteUInt16(" },
            { "unsigned_int(16)",                       "stream.WriteUInt16(" },
            { "unsigned int(15)",                       "stream.WriteBits(15, " },
            { "unsigned int(12)",                       "stream.WriteBits(12, " },
            { "signed int(12)",                         "stream.WriteBits(12, " },
            { "unsigned int(10)[i][j]",                 "stream.WriteBits(10, " },
            { "unsigned int(10)[i]",                    "stream.WriteBits(10, " },
            { "unsigned int(10)",                       "stream.WriteBits(10, " },
            { "unsigned int(8)[ sample_count ]",        "stream.WriteUInt8Array(sample_count, " },
            { "unsigned int(8)[length]",                "stream.WriteUInt8Array(length, " },
            { "unsigned int(8)[32]",                    "stream.WriteUInt8Array(32, " },
            { "unsigned int(8)[36]",                    "stream.WriteUInt8Array(36, " },
            { "unsigned int(8)[20]",                    "stream.WriteUInt8Array(20, " },
            { "unsigned int(8)[16]",                    "stream.WriteUInt8Array(16, " },
            { "unsigned int(9)",                        "stream.WriteBits(9, " },
            { "unsigned int(8)",                        "stream.WriteUInt8(" },
            { "unsigned int(7)",                        "stream.WriteBits(7, " },
            { "unsigned int(6)",                        "stream.WriteBits(6, " },
            { "unsigned int(5)[3]",                     "stream.WriteIso639(" },
            { "unsigned int(5)",                        "stream.WriteBits(5, " },
            { "unsigned int(4)",                        "stream.WriteBits(4, " },
            { "unsigned int(3)",                        "stream.WriteBits(3, " },
            { "unsigned int(2)[i][j]",                  "stream.WriteBits(2, " },
            { "unsigned int(2)",                        "stream.WriteBits(2, " },
            { "unsigned int(1)[i]",                     "stream.WriteBit(" },
            { "unsigned int(1)",                        "stream.WriteBit(" },
            { "unsigned int (32)",                      "stream.WriteUInt32(" },
            { "unsigned int(f(pattern_size_code))[i]",  "stream.WriteBits(pattern_size_code, " },
            { "unsigned int(f(index_size_code))[i]",    "stream.WriteBits(index_size_code, " },
            { "unsigned int(f(index_size_code))[j][k]", "stream.WriteBits(index_size_code, " },
            { "unsigned int(f(count_size_code))[i]",    "stream.WriteBits(count_size_code, " },
            { "unsigned int(base_offset_size*8)",       "stream.WriteUInt8Array((uint)base_offset_size, " },
            { "unsigned int(offset_size*8)",            "stream.WriteUInt8Array((uint)offset_size, " },
            { "unsigned int(length_size*8)",            "stream.WriteUInt8Array((uint)length_size, " },
            { "unsigned int(index_size*8)",             "stream.WriteUInt8Array((uint)index_size, " },
            { "unsigned int(field_size)",               "stream.WriteUInt8Array((uint)field_size, " },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "stream.WriteUInt8Array((uint)(length_size_of_traf_num+1), " },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "stream.WriteUInt8Array((uint)(length_size_of_trun_num+1), " },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "stream.WriteUInt8Array((uint)(length_size_of_sample_num+1), " },
            { "unsigned int(8*size-64)",                "stream.WriteUInt8Array((uint)(size-8), " },
            { "bit(8)[chunk_length]",                "stream.WriteUInt8Array((uint)chunk_length, " },
            { "unsigned int(subgroupIdLen)[i]",         "stream.WriteUInt32(" },
            { "const unsigned int(8)[6]",               "stream.WriteUInt8Array(6, " },
            { "const unsigned int(32)[2]",              "stream.WriteUInt32Array(2, " },
            { "const unsigned int(32)[3]",              "stream.WriteUInt32Array(3, " },
            { "const unsigned int(32)",                 "stream.WriteUInt32(" },
            { "const unsigned int(16)[3]",              "stream.WriteUInt16Array(3, " },
            { "const unsigned int(16)",                 "stream.WriteUInt16(" },
            { "const unsigned int(26)",                 "stream.WriteBits(26, " },
            { "template int(32)",                       "stream.WriteInt32(" },
            { "template int(16)",                       "stream.WriteInt16(" },
            { "template unsigned int(30)",              "stream.WriteBits(30, " },
            { "template unsigned int(32)",              "stream.WriteUInt32(" },
            { "template unsigned int(16)[3]",           "stream.WriteUInt16Array(3, " },
            { "template unsigned int(16)",              "stream.WriteUInt16(" },
            { "template unsigned int(8)[]",             "stream.WriteUInt8ArrayTillEnd(" },
            { "template unsigned int(8)",               "stream.WriteUInt8(" },
            { "int(64)",                                "stream.WriteInt64(" },
            { "int(32)",                                "stream.WriteInt32(" },
            { "int(16)",                                "stream.WriteInt16(" },
            { "int(8)",                                 "stream.WriteInt8(" },
            { "int(4)",                                 "stream.WriteBits(4, " },
            { "int",                                    "stream.WriteInt32(" },
            { "const bit(16)",                          "stream.WriteUInt16(" },
            { "const bit(1)",                           "stream.WriteBit(" },
            { "bit(1)",                                 "stream.WriteBit(" },
            { "bit(2)",                                 "stream.WriteBits(2, " },
            { "bit(3)",                                 "stream.WriteBits(3, " },
            { "bit(length-3)",                          "stream.WriteBits((uint)(length-3), " },
            { "bit(length)",                            "stream.WriteBits(length, " },
            { "bit(4)",                                 "stream.WriteBits(4, " },
            { "bit(5)",                                 "stream.WriteBits(5, " },
            { "bit(6)",                                 "stream.WriteBits(6, " },
            { "bit(7)",                                 "stream.WriteBits(7, " },
            { "bit(8)[]",                               "stream.WriteUInt8ArrayTillEnd(" },
            { "bit(8)",                                 "stream.WriteUInt8(" },
            { "bit(9)",                                 "stream.WriteBits(9, " },
            { "bit(13)",                                "stream.WriteBits(13, " },
            { "bit(14)",                                "stream.WriteBits(14, " },
            { "bit(15)",                                "stream.WriteBits(15, " },
            { "bit(16)[i]",                             "stream.WriteUInt16(" },
            { "bit(16)",                                "stream.WriteUInt16(" },
            { "bit(24)",                                "stream.WriteBits(24, " },
            { "bit(31)",                                "stream.WriteBits(31, " },
            { "bit(8 ceil(size / 8) \u2013 size)",      "stream.WriteUInt8Array((uint)(Math.Ceiling(size / 8d) - size), " },
            { "bit(8* ps_nalu_length)",                 "stream.WriteUInt8Array((uint)ps_nalu_length, " },
            { "bit(8*nalUnitLength)",                   "stream.WriteUInt8Array((uint)nalUnitLength, " },
            { "bit(8*sequenceParameterSetLength)",      "stream.WriteUInt8Array((uint)sequenceParameterSetLength, " },
            { "bit(8*pictureParameterSetLength)",       "stream.WriteUInt8Array((uint)pictureParameterSetLength, " },
            { "bit(8*sequenceParameterSetExtLength)",   "stream.WriteUInt8Array((uint)sequenceParameterSetExtLength, " },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "stream.WriteBits((uint)(8*num_bytes_constraint_info - 2), " },
            { "bit(8*nal_unit_length)",                 "stream.WriteUInt8Array((uint)nal_unit_length, " },
            { "bit(timeStampLength)",                   "stream.WriteUInt8Array((uint)timeStampLength, " },
            { "utf8string",                             "stream.WriteStringZeroTerminated(" },
            { "utfstring",                              "stream.WriteStringZeroTerminated(" },
            { "utf8list",                               "stream.WriteStringZeroTerminated(" },
            { "boxstring",                              "stream.WriteStringZeroTerminated(" },
            { "string",                                 "stream.WriteStringZeroTerminated(" },
            { "bit(32)[6]",                             "stream.WriteUInt32Array(6, " },
            { "bit(32)",                                "stream.WriteUInt32(" },
            { "uint(32)",                               "stream.WriteUInt32(" },
            { "uint(16)",                               "stream.WriteUInt16(" },
            { "uint(64)",                               "stream.WriteUInt64(" },
            { "uint(8)[32]",                            "stream.WriteUInt8Array(32, " }, // compressor_name
            { "uint(8)",                                "stream.WriteUInt8(" },
            { "uint(7)",                                "stream.WriteBits(7, " },
            { "uint(1)",                                "stream.WriteBits(1, " },
            { "signed int(64)",                       "stream.WriteInt64(" },
            { "signed int(32)",                         "stream.WriteInt32(" },
            { "signed int (16)",                        "stream.WriteInt16(" },
            { "signed int(16)[grid_pos_view_id[i]]",    "stream.WriteInt16(" },
            { "signed int(16)",                         "stream.WriteInt16(" },
            { "signed int (8)",                         "stream.WriteInt8(" },
            { "signed int(8)",                        "stream.WriteInt8(" },
            { "Box()[]",                                "stream.WriteBox(" },
            { "Box[]",                                  "stream.WriteBox(" },
            { "Box()",                                    "stream.WriteBox(" },
            { "Box",                                    "stream.WriteBox(" },
            { "SchemeTypeBox",                          "stream.WriteBox(" },
            { "SchemeInformationBox",                   "stream.WriteBox(" },
            { "ItemPropertyContainerBox",               "stream.WriteBox(" },
            { "ItemPropertyAssociationBox",             "stream.WriteBox(" },
            { "ItemPropertyAssociationBox[]",           "stream.WriteBox(" },
            { "char",                                   "stream.WriteInt8(" },
            { "ICC_profile",                            "stream.WriteClass(" },
            { "OriginalFormatBox(fmt)",                 "stream.WriteBox(" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "stream.WriteBox(" },
            { "ItemInfoEntry[ entry_count ]",           "stream.WriteBox(entry_count, " },
            { "TypeCombinationBox[]",                   "stream.WriteBox(" },
            { "FilePartitionBox",                       "stream.WriteBox(" },
            { "FECReservoirBox",                        "stream.WriteBox(" },
            { "FileReservoirBox",                       "stream.WriteBox(" },
            { "PartitionEntry[ entry_count ]",          "stream.WriteBox(entry_count, " },
            { "FDSessionGroupBox",                      "stream.WriteBox(" },
            { "GroupIdToNameBox",                       "stream.WriteBox(" },
            { "base64string",                           "stream.WriteStringZeroTerminated(" },
            { "ProtectionSchemeInfoBox",                "stream.WriteBox(" },
            { "SingleItemTypeReferenceBox",             "stream.WriteBox(" },
            { "SingleItemTypeReferenceBox[]",           "stream.WriteBox(" },
            { "SingleItemTypeReferenceBoxLarge",        "stream.WriteBox(" },
            { "SingleItemTypeReferenceBoxLarge[]",      "stream.WriteBox(" },
            { "HandlerBox(handler_type)",               "stream.WriteBox(" },
            { "PrimaryItemBox",                         "stream.WriteBox(" },
            { "DataInformationBox",                     "stream.WriteBox(" },
            { "ItemLocationBox",                        "stream.WriteBox(" },
            { "ItemProtectionBox",                      "stream.WriteBox(" },
            { "ItemInfoBox",                            "stream.WriteBox(" },
            { "IPMPControlBox",                         "stream.WriteBox(" },
            { "ItemReferenceBox",                       "stream.WriteBox(" },
            { "ItemDataBox",                            "stream.WriteBox(" },
            { "TrackReferenceTypeBox[]",               "stream.WriteBox(" },
            { "MetaDataKeyBox[]",                       "stream.WriteBox(" },
            { "TierInfoBox()",                            "stream.WriteBox(" },
            { "TierInfoBox",                            "stream.WriteBox(" },
            { "MultiviewRelationAttributeBox",          "stream.WriteBox(" },
            { "TierBitRateBox()",                         "stream.WriteBox(" },
            { "TierBitRateBox",                         "stream.WriteBox(" },
            { "BufferingBox()",                           "stream.WriteBox(" },
            { "BufferingBox",                           "stream.WriteBox(" },
            { "MultiviewSceneInfoBox",                  "stream.WriteBox(" },
            { "MVDDecoderConfigurationRecord",          "stream.WriteClass(" },
            { "MVDDepthResolutionBox",                  "stream.WriteBox(" },
            { "MVCDecoderConfigurationRecord()",        "stream.WriteClass(" },
            { "AVCDecoderConfigurationRecord()",        "stream.WriteClass(" },
            { "HEVCDecoderConfigurationRecord()",       "stream.WriteClass(" },
            { "LHEVCDecoderConfigurationRecord()",      "stream.WriteClass(" },
            { "SVCDecoderConfigurationRecord()",        "stream.WriteClass(" },
            { "HEVCTileTierLevelConfigurationRecord()", "stream.WriteClass(" },
            { "EVCDecoderConfigurationRecord()",        "stream.WriteClass(" },
            { "VvcDecoderConfigurationRecord()",        "stream.WriteClass(" },
            { "EVCSliceComponentTrackConfigurationRecord()", "stream.WriteClass(" },
            { "Descriptor[0 .. 255]",                   "stream.WriteDescriptor(" },
            { "Descriptor[count]",                      "stream.WriteDescriptor(" },
            { "Descriptor",                             "stream.WriteDescriptor(" },
            { "WebVTTConfigurationBox",                 "stream.WriteBox(" },
            { "WebVTTSourceLabelBox",                   "stream.WriteBox(" },
            { "OperatingPointsRecord",                  "stream.WriteClass(" },
            { "VvcSubpicIDEntry",                       "stream.WriteEntry(" },
            { "VvcSubpicOrderEntry",                    "stream.WriteEntry(" },
            { "URIInitBox",                             "stream.WriteBox(" },
            { "URIBox",                                 "stream.WriteBox(" },
            { "CleanApertureBox",                       "stream.WriteBox(" },
            { "PixelAspectRatioBox",                    "stream.WriteBox(" },
            { "DownMixInstructions()[]",               "stream.WriteBox(" },
            { "DRCCoefficientsBasic()[]",              "stream.WriteBox(" },
            { "DRCInstructionsBasic()[]",              "stream.WriteBox(" },
            { "DRCCoefficientsUniDRC()[]",             "stream.WriteBox(" },
            { "DRCInstructionsUniDRC()[]",             "stream.WriteBox(" },
            { "HEVCConfigurationBox",                   "stream.WriteBox(" },
            { "LHEVCConfigurationBox",                  "stream.WriteBox(" },
            { "AVCConfigurationBox",                    "stream.WriteBox(" },
            { "SVCConfigurationBox",                    "stream.WriteBox(" },
            { "ScalabilityInformationSEIBox",           "stream.WriteBox(" },
            { "SVCPriorityAssignmentBox",               "stream.WriteBox(" },
            { "ViewScalabilityInformationSEIBox",       "stream.WriteBox(" },
            { "ViewIdentifierBox()",                      "stream.WriteBox(" },
            { "MVCConfigurationBox",                    "stream.WriteBox(" },
            { "MVCViewPriorityAssignmentBox",           "stream.WriteBox(" },
            { "IntrinsicCameraParametersBox",           "stream.WriteBox(" },
            { "ExtrinsicCameraParametersBox",           "stream.WriteBox(" },
            { "MVCDConfigurationBox",                   "stream.WriteBox(" },
            { "MVDScalabilityInformationSEIBox",        "stream.WriteBox(" },
            { "A3DConfigurationBox",                    "stream.WriteBox(" },
            { "VvcOperatingPointsRecord",               "stream.WriteClass(" },
            { "VVCSubpicIDRewritingInfomationStruct()", "stream.WriteClass(" },
            { "MPEG4ExtensionDescriptorsBox ()",        "stream.WriteBox(" },
            { "MPEG4ExtensionDescriptorsBox()",         "stream.WriteBox(" },
            { "MPEG4ExtensionDescriptorsBox",           "stream.WriteBox(" },
            { "bit(8*dci_nal_unit_length)",             "stream.WriteUInt8Array((uint)dci_nal_unit_length, " },
            { "DependencyInfo[numReferences]",          "stream.WriteClass(" },
            { "VvcPTLRecord(0)[i]",                     "stream.WriteClass(" },
            { "EVCSliceComponentTrackConfigurationBox", "stream.WriteBox(" },
            { "SVCMetadataSampleConfigBox",             "stream.WriteBox(" },
            { "SVCPriorityLayerInfoBox",                "stream.WriteBox(" },
            { "EVCConfigurationBox",                    "stream.WriteBox(" },
            { "VvcNALUConfigBox",                       "stream.WriteBox(" },
            { "VvcConfigurationBox",                    "stream.WriteBox(" },
            { "HEVCTileConfigurationBox",               "stream.WriteBox(" },
            { "MetaDataKeyTableBox()",                  "stream.WriteBox(" },
            { "BitRateBox()",                          "stream.WriteBox(" },
            { "char[count]",                            "stream.WriteUInt8Array((uint)count, " },
            { "signed int(32)[ c ]",                    "stream.WriteInt32(" },
            { "unsigned int(8)[]",                      "stream.WriteUInt8ArrayTillEnd(" },
            { "unsigned int(8)[i]",                     "stream.WriteUInt8(" },
            { "unsigned int(6)[i]",                     "stream.WriteBits(6, " },
            { "unsigned int(6)[i][j]",                  "stream.WriteBits(6, " },
            { "unsigned int(1)[i][j]",                  "stream.WriteBits(1, " },
            { "unsigned int(9)[i]",                     "stream.WriteBits(9, " },
            { "unsigned int(32)[]",                     "stream.WriteUInt32ArrayTillEnd(" },
            { "unsigned int(32)[i]",                    "stream.WriteUInt32(" },
            { "unsigned int(32)[j]",                    "stream.WriteUInt32(" },
            { "unsigned int(8)[j][k]",                  "stream.WriteUInt8(" },
            { "signed int(64)[j][k]",                 "stream.WriteInt64(" },
            { "unsigned int(8)[j]",                     "stream.WriteUInt8(" },
            { "signed int(64)[j]",                    "stream.WriteInt64(" },
            { "char[]",                                 "stream.WriteUInt8ArrayTillEnd(" },
            { "string[method_count]",                   "stream.WriteStringArray(method_count, " },
             { "ItemInfoExtension(extension_type)",                     "stream.WriteClass(" },
            { "SampleGroupDescriptionEntry(grouping_type)",            "stream.WriteEntry(" },
            { "SampleEntry()",                            "stream.WriteBox(" },
            { "SampleConstructor()",                      "stream.WriteBox(" },
            { "InlineConstructor()",                      "stream.WriteBox(" },
            { "SampleConstructorFromTrackGroup()",        "stream.WriteBox(" },
            { "NALUStartInlineConstructor()",             "stream.WriteBox(" },
            { "MPEG4BitRateBox",                        "stream.WriteBox(" },
            { "ChannelLayout()",                          "stream.WriteBox(" },
            { "UniDrcConfigExtension()",                  "stream.WriteBox(" },
            { "SamplingRateBox()",                        "stream.WriteBox(" },
            { "TextConfigBox()",                          "stream.WriteBox(" },
            { "MetaDataKeyTableBox",                    "stream.WriteBox(" },
            { "BitRateBox",                             "stream.WriteBox(" },
            { "CompleteTrackInfoBox",                   "stream.WriteBox(" },
            { "TierDependencyBox()",                      "stream.WriteBox(" },
            { "InitialParameterSetBox",                 "stream.WriteBox(" },
            { "PriorityRangeBox()",                       "stream.WriteBox(" },
            { "ViewPriorityBox",                        "stream.WriteBox(" },
            { "SVCDependencyRangeBox",                  "stream.WriteBox(" },
            { "RectRegionBox",                          "stream.WriteBox(" },
            { "IroiInfoBox",                            "stream.WriteBox(" },
            { "TranscodingInfoBox",                     "stream.WriteBox(" },
            { "MetaDataKeyDeclarationBox()",              "stream.WriteBox(" },
            { "MetaDataDatatypeBox()",                    "stream.WriteBox(" },
            { "MetaDataLocaleBox()",                      "stream.WriteBox(" },
            { "MetaDataSetupBox()",                       "stream.WriteBox(" },
            { "MetaDataExtensionsBox()",                  "stream.WriteBox(" },
            { "TrackLoudnessInfo[]",                    "stream.WriteBox(" },
            { "AlbumLoudnessInfo[]",                    "stream.WriteBox(" },
            { "VvcPTLRecord(num_sublayers)",            "stream.WriteClass(" },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "stream.WriteClass(" },
            { "OpusSpecificBox",                        "stream.WriteBox(" },
            { "unsigned int(8 * OutputChannelCount)",   "stream.WriteUInt8Array((uint)OutputChannelCount, " },
            { "ChannelMappingTable(OutputChannelCount)",                    "stream.WriteClass(" },
            // descriptors
            { "DecoderConfigDescriptor",                "stream.WriteDescriptor(" },
            { "SLConfigDescriptor",                     "stream.WriteDescriptor(" },
            { "IPI_DescrPointer[0 .. 1]",               "stream.WriteDescriptor(" },
            { "IP_IdentificationDataSet[0 .. 255]",     "stream.WriteDescriptor(" },
            { "IPMP_DescriptorPointer[0 .. 255]",       "stream.WriteDescriptor(" },
            { "LanguageDescriptor[0 .. 255]",           "stream.WriteDescriptor(" },
            { "QoS_Descriptor[0 .. 1]",                 "stream.WriteDescriptor(" },
            { "ES_Descriptor",                          "stream.WriteDescriptor(" },
            { "RegistrationDescriptor[0 .. 1]",         "stream.WriteDescriptor(" },
            { "ExtensionDescriptor[0 .. 255]",          "stream.WriteDescriptor(" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "stream.WriteDescriptor(" },
            { "DecoderSpecificInfo[0 .. 1]",            "stream.WriteDescriptor(" },
            { "bit(8)[URLlength]",                      "stream.WriteUInt8Array((uint)URLlength, " },
            { "bit(8)[sizeOfInstance-4]",               "stream.WriteUInt8Array((uint)(sizeOfInstance - 4), " },
            { "bit(8)[sizeOfInstance-3]",               "stream.WriteUInt8Array((uint)(sizeOfInstance - 3), " },
            { "bit(8)[size-10]",                        "stream.WriteUInt8Array((uint)(size - 10), " },
            { "double(32)",                             "stream.WriteDouble32(" },
            { "fixedpoint1616",                         "stream.WriteFixedPoint1616(" },
            { "QoS_Qualifier[]",                        "stream.WriteDescriptor(" },
            { "GetAudioObjectType()",                   "stream.WriteClass(" },
            { "bslbf(header_size * 8)[]",               "stream.WriteBslbf(header_size * 8, " },
            { "bslbf(trailer_size * 8)[]",              "stream.WriteBslbf(trailer_size * 8, " },
            { "bslbf(aux_size * 8)[]",                  "stream.WriteBslbf(aux_size * 8, " },
            { "bslbf(11)",                              "stream.WriteBslbf(11, " },
            { "bslbf(5)",                               "stream.WriteBslbf(5, " },
            { "bslbf(4)",                               "stream.WriteBslbf(4, " },
            { "bslbf(2)",                               "stream.WriteBslbf(2, " },
            { "bslbf(1)",                               "stream.WriteBslbf(" },
            { "uimsbf(32)",                             "stream.WriteUimsbf(32, " },
            { "uimsbf(24)",                             "stream.WriteUimsbf(24, " },
            { "uimsbf(18)",                             "stream.WriteUimsbf(18, " },
            { "uimsbf(16)",                             "stream.WriteUimsbf(16, " },
            { "uimsbf(14)",                             "stream.WriteUimsbf(14, " },
            { "uimsbf(12)",                             "stream.WriteUimsbf(12, " },
            { "uimsbf(10)",                             "stream.WriteUimsbf(10, " },
            { "uimsbf(8)",                              "stream.WriteUimsbf(8, " },
            { "uimsbf(7)",                              "stream.WriteUimsbf(7, " },
            { "uimsbf(6)",                              "stream.WriteUimsbf(6, " },
            { "uimsbf(5)",                              "stream.WriteUimsbf(5, " },
            { "uimsbf(4)",                              "stream.WriteUimsbf(4, " },
            { "uimsbf(3)",                              "stream.WriteUimsbf(3, " },
            { "uimsbf(2)",                              "stream.WriteUimsbf(2, " },
            { "uimsbf(1)",                              "stream.WriteUimsbf(" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      boxSize += stream.WriteClass(" },
            { "case 8:\r\n      CelpSpecificConfig()",  "case 8:\r\n      boxSize += stream.WriteClass(" },
            { "case 9:\r\n      HvxcSpecificConfig()",  "case 9:\r\n      boxSize += stream.WriteClass(" },
            { "case 12:\r\n      TTSSpecificConfig()",  "case 12:\r\n      boxSize += stream.WriteClass(" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      boxSize += stream.WriteClass(" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "case 24:\r\n      boxSize += stream.WriteClass(" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "case 25:\r\n      boxSize += stream.WriteClass(" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "case 26:\r\n    case 27:\r\n      boxSize += stream.WriteClass(" },
            { "case 28:\r\n      SSCSpecificConfig()",  "case 28:\r\n      boxSize += stream.WriteClass(" },
            { "case 30:\r\n      uimsbf(1)", "case 30:\r\n      boxSize += stream.WriteUimsbf(" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "case 32:\r\n    case 33:\r\n    case 34:\r\n      boxSize += stream.WriteClass(" },
            { "case 35:\r\n      DSTSpecificConfig()",  "case 35:\r\n      boxSize += stream.WriteClass(" },
            { "case 36:\r\n      bslbf(5)", "case 36:\r\n      boxSize += stream.WriteBslbf(5, " },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "case 37:\r\n    case 38:\r\n      boxSize += stream.WriteClass(" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "case 39:\r\n      boxSize += stream.WriteClass(" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "case 40:\r\n    case 41:\r\n      boxSize += stream.WriteClass(" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      boxSize += stream.WriteBslbf(2, " },
            { "SpatialSpecificConfig", "stream.WriteClass(" },
            { "ALSSpecificConfig",                      "stream.WriteClass(" },
            { "ErrorProtectionSpecificConfig",          "stream.WriteClass(" },
            { "program_config_element",                 "stream.WriteClass(" },
            { "byte_alignment",                         "stream.WriteByteAlignment(" },
            { "CelpHeader(samplingFrequencyIndex)",     "stream.WriteClass(" },
            { "CelpBWSenhHeader",                       "stream.WriteClass(" },
            { "HVXCconfig",                             "stream.WriteClass(" },
            { "TTS_Sequence",                           "stream.WriteClass(" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "stream.WriteClass(" },
            { "ErHVXCconfig",                           "stream.WriteClass(" },
            { "PARAconfig",                             "stream.WriteClass(" },
            { "HILNenexConfig",                         "stream.WriteClass(" },
            { "HILNconfig",                             "stream.WriteClass(" },
            { "ld_sbr_header(channelConfiguration)",                          "stream.WriteClass(" },
            { "sbr_header",                             "stream.WriteClass(" },
            { "uimsbf(1)[i]",                           "stream.WriteUimsbf(" },
            { "uimsbf(8)[i]",                           "stream.WriteUimsbf(8, " },
            { "bslbf(1)[i]",                            "stream.WriteBslbf(1, " },
            { "uimsbf(4)[i]",                           "stream.WriteUimsbf(4, " },
            { "uimsbf(1)[c]",                           "stream.WriteUimsbf(" },
            { "uimsbf(32)[f]",                          "stream.WriteUimsbf(32, " },
            { "CelpHeader",                             "stream.WriteClass(" },
            { "ER_SC_CelpHeader",                       "stream.WriteClass(" },
            { "uimsbf(6)[i]",                           "stream.WriteUimsbf(6, " },
            { "uimsbf(1)[i][j]",                        "stream.WriteUimsbf(" },
            { "uimsbf(2)[i][j]",                        "stream.WriteUimsbf(2, " },
            { "uimsbf(4)[i][j]",                        "stream.WriteUimsbf(4, " },
            { "uimsbf(16)[i][j]",                       "stream.WriteUimsbf(16, " },
            { "uimsbf(7)[i][j]",                        "stream.WriteUimsbf(7, " },
            { "uimsbf(5)[i][j]",                        "stream.WriteUimsbf(5, " },
            { "uimsbf(6)[i][j]",                        "stream.WriteUimsbf(6, " },
            { "AV1SampleEntry",                         "stream.WriteBox(" },
            { "AV1CodecConfigurationBox",               "stream.WriteBox(" },
            { "AV1CodecConfigurationRecord",            "stream.WriteClass(" },
            { "vluimsbf8",                              "stream.WriteUimsbf(8, " },
            { "byte(urlMIDIStream_length)",             "stream.WriteUInt8Array((uint)urlMIDIStream_length, " },
            { "aligned bit(3)",                         "stream.WriteAlignedBits(3, " },
            { "aligned bit(1)",                         "stream.WriteAlignedBits(1, " },
            { "bit",                                    "stream.WriteBit(" },
            { "case 0b000:\r\n                mainscore_file", "case 0b000:\r\n                boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b001:\r\n                bit(8) partID; // ID of the part at which the following info refers \r\n                part_file", "case 0b001:\r\n                boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b010:\r\n                // this segment is always in binary as stated in Section 9 \r\n                synch_file", "case 0b010:\r\n                boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b011:\r\n                format_file", "case 0b011:\r\n                boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b100:\r\n                bit(8) partID;\r\n                bit(8) lyricID;\r\n                lyrics_file", "case 0b100:\r\n                boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b101:\r\n                // this segment is always in binary as stated in Section 11.4 \r\n                font_file", "case 0b101:\r\n                boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b110: ",                           "case 0b110: boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "case 0b111: ",                           "case 0b111: boxSize += stream.WriteBits((uint)(8 * chunk_length), " },
            { "unsigned int(16)[3]",                    "stream.WriteUInt16Array(3, " },
            { "MultiLanguageString[]",                  "stream.WriteStringSizeLangPrefixed(" },
            { "AdobeChapterRecord[]",                   "stream.WriteClass(" },
            { "ThreeGPPKeyword[]",                      "stream.WriteClass(" },
            { "IodsSample[]",                           "stream.WriteClass(" },
            { "XtraTag[]",                              "stream.WriteClass(" },
            { "XtraValue[count]",                       "stream.WriteClass(" },
            { "ViprEntry[]",                            "stream.WriteClass(" },
            { "TrickPlayEntry[]",                       "stream.WriteClass(" },
            { "MtdtEntry[ entry_count ]",               "stream.WriteClass(" },
            { "RectRecord",                             "stream.WriteClass(" },
            { "StyleRecord",                            "stream.WriteClass(" },
            { "ProtectionSystemSpecificKeyID[count]",   "stream.WriteClass(" },
            { "unsigned int(8)[contentIDLength]",       "stream.WriteUInt8Array((uint)contentIDLength, " },
            { "unsigned int(8)[contentTypeLength]",     "stream.WriteUInt8Array((uint)contentTypeLength, " },
            { "unsigned int(8)[rightsIssuerLength]",    "stream.WriteUInt8Array((uint)rightsIssuerLength, " },
            { "unsigned int(8)[textualHeadersLength]",  "stream.WriteUInt8Array((uint)textualHeadersLength, " },
            { "unsigned int(8)[count]",                 "stream.WriteUInt8Array((uint)count, " },
            { "unsigned int(8)[4]",                     "stream.WriteUInt8Array((uint)4, " },
            { "unsigned int(8)[14]",                    "stream.WriteUInt8Array((uint)14, " },
            { "unsigned int(8)[6]",                     "stream.WriteUInt8Array((uint)6, " },
            { "unsigned int(8)[256]",                   "stream.WriteUInt8Array((uint)256, " },
            { "unsigned int(8)[512]",                   "stream.WriteUInt8Array((uint)512, " },
            { "char[tagLength]",                        "stream.WriteUInt8Array((uint)tagLength, " },
            { "unsigned int(8)[constant_IV_size]",      "stream.WriteUInt8Array((uint)constant_IV_size, " },
            { "unsigned int(8)[length-6]",              "stream.WriteUInt8Array((uint)(length-6), " },
            { "EC3SpecificEntry[numIndSub + 1]",        "stream.WriteClass(" },
            { "SampleEncryptionSample(version, flags, Per_Sample_IV_Size)[sample_count]",        "stream.WriteClass(" },
            { "unsigned int(Per_Sample_IV_Size*8)",     "stream.WriteUInt8Array((uint)Per_Sample_IV_Size, " },
            { "SampleEncryptionSubsample(version)[subsample_count]", "stream.WriteClass(" },
            { "CelpSpecificConfig()",                   "stream.WriteClass(" },
            { "HvxcSpecificConfig()",                   "stream.WriteClass(" },
            { "TTSSpecificConfig()",                    "stream.WriteClass(" },
            { "ErrorResilientCelpSpecificConfig()",     "stream.WriteClass(" },
            { "ErrorResilientHvxcSpecificConfig()",     "stream.WriteClass(" },
            { "SSCSpecificConfig()",                    "stream.WriteClass(" },
            { "DSTSpecificConfig()",                    "stream.WriteClass(" },
            { "ELDSpecificConfig(channelConfiguration)","stream.WriteClass(" },
            { "GASpecificConfig()",                     "stream.WriteClass(" },
            { "StructuredAudioSpecificConfig()",        "stream.WriteClass(" },
            { "ParametricSpecificConfig()",             "stream.WriteClass(" },
            { "MPEG_1_2_SpecificConfig()",              "stream.WriteClass(" },
            { "SLSSpecificConfig()",                    "stream.WriteClass(" },
            { "SymbolicMusicSpecificConfig()",          "stream.WriteClass(" },
        };
        if (map.ContainsKey(type))
            return map[type];
        else if (map.ContainsKey(type.Replace("()", "")))
            return map[type.Replace("()", "")];
        else
            throw new NotSupportedException(type);
    }

    private static bool IsWorkaround(string type)
    {
        HashSet<string> map = new HashSet<string>
        {
            "samplerate = samplerate >> 16",
            "int downmix_instructions_count = 1",
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
            "numSbrHeader = 1",
            "numSbrHeader = 2",
            "numSbrHeader = 3",
            "numSbrHeader = 4",
            "numSbrHeader = 0",
        };
        return map.Contains(type);
    }

    private static string GetType(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string> {
            { "unsigned int(64)[ entry_count ]",        "ulong[]" },
            { "unsigned int(64)",                       "ulong" },
            { "unsigned int(48)",                       "ulong" },
            { "template int(32)[9]",                    "uint[]" },
            { "unsigned int(32)[ entry_count ]",        "uint[]" },
            { "unsigned int(32)[3]",                    "uint[]" },
            { "unsigned int(32)",                       "uint" },
            { "unsigned int(31)",                       "uint" },
            { "unsigned_int(32)",                       "uint" },
            { "unsigned int(24)",                       "uint" },
            { "unsigned int(29)",                       "uint" },
            { "unsigned int(28)",                       "uint" },
            { "unsigned int(26)",                       "uint" },
            { "unsigned int(16)[i]",                    "ushort[]" },
            { "unsigned int(16)[j]",                    "ushort[]" },
            { "unsigned int(16)[i][j]",                 "ushort[][]" },
            { "unsigned int(16)",                       "ushort" },
            { "unsigned_int(16)",                       "ushort" },
            { "unsigned int(15)",                       "ushort" },
            { "unsigned int(12)",                       "ushort" },
            { "signed int(12)",                         "short" },
            { "unsigned int(10)[i][j]",                 "ushort[][]" },
            { "unsigned int(10)[i]",                    "ushort[]" },
            { "unsigned int(10)",                       "ushort" },
            { "unsigned int(8)[ sample_count ]",        "byte[]" },
            { "unsigned int(8)[length]",                "byte[]" },
            { "unsigned int(8)[32]",                    "byte[]" },
            { "unsigned int(8)[36]",                    "byte[]" },
            { "unsigned int(8)[20]",                    "byte[]" },
            { "unsigned int(8)[16]",                    "byte[]" },
            { "unsigned int(9)",                        "ushort" },
            { "unsigned int(8)",                        "byte" },
            { "unsigned int(7)",                        "byte" },
            { "unsigned int(6)",                        "byte" },
            { "unsigned int(5)[3]",                     "string" },
            { "unsigned int(5)",                        "byte" },
            { "unsigned int(4)",                        "byte" },
            { "unsigned int(3)",                        "byte" },
            { "unsigned int(2)[i][j]",                  "byte[][]" },
            { "unsigned int(2)",                        "byte" },
            { "unsigned int(1)[i]",                     "bool[]" },
            { "unsigned int(1)",                        "bool" },
            { "unsigned int (32)",                      "uint" },
            { "unsigned int(f(pattern_size_code))[i]",  "byte[]" },
            { "unsigned int(f(index_size_code))[j][k]", "byte[][]" },
            { "unsigned int(f(count_size_code))[i]",    "byte[]" },
            { "unsigned int(base_offset_size*8)",       "byte[]" },
            { "unsigned int(offset_size*8)",            "byte[]" },
            { "unsigned int(length_size*8)",            "byte[]" },
            { "unsigned int(index_size*8)",             "byte[]" },
            { "unsigned int(field_size)",               "byte[]" },
            { "unsigned int((length_size_of_traf_num+1) * 8)", "byte[]" },
            { "unsigned int((length_size_of_trun_num+1) * 8)", "byte[]" },
            { "unsigned int((length_size_of_sample_num+1) * 8)", "byte[]" },
            { "unsigned int(8*size-64)",                "byte[]" },
            { "bit(8)[chunk_length]",                   "byte[]" },
            { "unsigned int(subgroupIdLen)[i]",         "uint[]" },
            { "const unsigned int(8)[6]",               "byte[]" },
            { "const unsigned int(32)[2]",              "uint[]" },
            { "const unsigned int(32)[3]",              "uint[]" },
            { "const unsigned int(32)",                 "uint" },
            { "const unsigned int(16)[3]",              "ushort[]" },
            { "const unsigned int(16)",                 "ushort" },
            { "const unsigned int(26)",                 "uint" },
            { "template int(32)",                       "int" },
            { "template int(16)",                       "short" },
            { "template unsigned int(30)",              "uint" },
            { "template unsigned int(32)",              "uint" },
            { "template unsigned int(16)[3]",           "ushort[]" },
            { "template unsigned int(16)",              "ushort" },
            { "template unsigned int(8)[]",             "byte[]" },
            { "template unsigned int(8)",               "byte" },
            { "int(64)",                                "long" },
            { "int(32)",                                "int" },
            { "int(16)",                                "short" },
            { "int(8)",                                 "sbyte" },
            { "int(4)",                                 "byte" },
            { "int",                                    "int" },
            { "const bit(16)",                          "ushort" },
            { "const bit(1)",                           "bool" },
            { "bit(1)",                                 "bool" },
            { "bit(2)",                                 "byte" },
            { "bit(3)",                                 "byte" },
            { "bit(length-3)",                          "byte[]" },
            { "bit(length)",                            "byte[]" },
            { "bit(4)",                                 "byte" },
            { "bit(5)",                                 "byte" },
            { "bit(6)",                                 "byte" },
            { "bit(7)",                                 "byte" },
            { "bit(8)[]",                               "byte[]" },
            { "bit(8)",                                 "byte" },
            { "bit(9)",                                 "ushort" },
            { "bit(13)",                                "ushort" },
            { "bit(14)",                                "ushort" },
            { "bit(15)",                                "ushort" },
            { "bit(16)[i]",                             "ushort[]" },
            { "bit(16)",                                "ushort" },
            { "bit(24)",                                "uint" },
            { "bit(31)",                                "uint" },
            { "bit(8 ceil(size / 8) \u2013 size)",      "byte[]" },
            { "bit(8* ps_nalu_length)",                 "byte[]" },
            { "bit(8*nalUnitLength)",                   "byte[]" },
            { "bit(8*sequenceParameterSetLength)",      "byte[]" },
            { "bit(8*pictureParameterSetLength)",       "byte[]" },
            { "bit(8*sequenceParameterSetExtLength)",   "byte[]" },
            { "unsigned int(8*num_bytes_constraint_info - 2)", "byte[]" },
            { "bit(8*nal_unit_length)",                 "byte[]" },
            { "bit(timeStampLength)",                   "byte[]" },
            { "utf8string",                             "BinaryUTF8String" },
            { "utfstring",                              "BinaryUTF8String" },
            { "utf8list",                               "BinaryUTF8String" },
            { "boxstring",                              "BinaryUTF8String" },
            { "string",                                 "BinaryUTF8String" },
            { "bit(32)[6]",                             "uint[]" },
            { "bit(32)",                                "uint" },
            { "uint(32)",                               "uint" },
            { "uint(16)",                               "ushort" },
            { "uint(64)",                               "ulong" },
            { "uint(8)[32]",                            "byte[]" }, // compressor_name
            { "uint(8)",                                "byte" },
            { "uint(7)",                                "byte" },
            { "uint(1)",                                "byte" },
            { "signed int(64)",                       "long" },
            { "signed int(32)",                         "int" },
            { "signed int (16)",                        "short" },
            { "signed int(16)[grid_pos_view_id[i]]",    "short[]" },
            { "signed int(16)",                         "short" },
            { "signed int (8)",                         "sbyte" },
            { "signed int(8)",                        "sbyte" },
            { "Box()[]",                                "Box[]" },
            { "Box[]",                                  "Box[]" },
            { "Box()",                                    "Box" },
            { "Box",                                    "Box" },
            { "SchemeTypeBox",                          "SchemeTypeBox" },
            { "SchemeInformationBox",                   "SchemeInformationBox" },
            { "ItemPropertyContainerBox",               "ItemPropertyContainerBox" },
            { "ItemPropertyAssociationBox",             "ItemPropertyAssociationBox" },
            { "char",                                   "byte" },
            { "loudness",                               "int" },
            { "ICC_profile",                            "ICC_profile" },
            { "OriginalFormatBox(fmt)",                 "OriginalFormatBox" },
            { "DataEntryBaseBox(entry_type, entry_flags)", "DataEntryBaseBox" },
            { "ItemInfoEntry[ entry_count ]",           "ItemInfoEntry[]" },
            { "TypeCombinationBox[]",                   "TypeCombinationBox[]" },
            { "FilePartitionBox",                       "FilePartitionBox" },
            { "FECReservoirBox",                        "FECReservoirBox" },
            { "FileReservoirBox",                       "FileReservoirBox" },
            { "PartitionEntry[ entry_count ]",          "PartitionEntry[]" },
            { "FDSessionGroupBox",                      "FDSessionGroupBox" },
            { "GroupIdToNameBox",                       "GroupIdToNameBox" },
            { "base64string",                           "BinaryUTF8String" },
            { "ProtectionSchemeInfoBox",                "ProtectionSchemeInfoBox" },
            { "SingleItemTypeReferenceBox",             "SingleItemTypeReferenceBox" },
            { "SingleItemTypeReferenceBox[]",           "SingleItemTypeReferenceBox[]" },
            { "SingleItemTypeReferenceBoxLarge",        "SingleItemTypeReferenceBoxLarge" },
            { "SingleItemTypeReferenceBoxLarge[]",      "SingleItemTypeReferenceBoxLarge[]" },
            { "HandlerBox(handler_type)",               "HandlerBox" },
            { "PrimaryItemBox",                         "PrimaryItemBox" },
            { "DataInformationBox",                     "DataInformationBox" },
            { "ItemLocationBox",                        "ItemLocationBox" },
            { "ItemProtectionBox",                      "ItemProtectionBox" },
            { "ItemInfoBox",                            "ItemInfoBox" },
            { "IPMPControlBox",                         "IPMPControlBox" },
            { "ItemReferenceBox",                       "ItemReferenceBox" },
            { "ItemDataBox",                            "ItemDataBox" },
            { "TrackReferenceTypeBox[]",               "TrackReferenceTypeBox[]" },
            { "MetaDataKeyBox[]",                       "MetaDataKeyBox[]" },
            { "TierInfoBox()",                            "TierInfoBox" },
            { "TierInfoBox",                            "TierInfoBox" },
            { "MultiviewRelationAttributeBox",          "MultiviewRelationAttributeBox" },
            { "TierBitRateBox()",                         "TierBitRateBox" },
            { "TierBitRateBox",                         "TierBitRateBox" },
            { "BufferingBox()",                           "BufferingBox" },
            { "BufferingBox",                           "BufferingBox" },
            { "MultiviewSceneInfoBox",                  "MultiviewSceneInfoBox" },
            { "MVDDecoderConfigurationRecord",          "MVDDecoderConfigurationRecord" },
            { "MVDDepthResolutionBox",                  "MVDDepthResolutionBox" },
            { "MVCDecoderConfigurationRecord()",        "MVCDecoderConfigurationRecord" },
            { "AVCDecoderConfigurationRecord()",        "AVCDecoderConfigurationRecord" },
            { "HEVCDecoderConfigurationRecord()",       "HEVCDecoderConfigurationRecord" },
            { "LHEVCDecoderConfigurationRecord()",      "LHEVCDecoderConfigurationRecord" },
            { "SVCDecoderConfigurationRecord()",        "SVCDecoderConfigurationRecord" },
            { "HEVCTileTierLevelConfigurationRecord()", "HEVCTileTierLevelConfigurationRecord" },
            { "EVCDecoderConfigurationRecord()",        "EVCDecoderConfigurationRecord" },
            { "VvcDecoderConfigurationRecord()",        "VvcDecoderConfigurationRecord" },
            { "EVCSliceComponentTrackConfigurationRecord()", "EVCSliceComponentTrackConfigurationRecord" },
            { "Descriptor[0 .. 255]",                   "Descriptor[]" },
            { "Descriptor[count]",                      "Descriptor[]" },
            { "Descriptor",                             "Descriptor" },
            { "WebVTTConfigurationBox",                 "WebVTTConfigurationBox" },
            { "WebVTTSourceLabelBox",                   "WebVTTSourceLabelBox" },
            { "OperatingPointsRecord",                  "OperatingPointsRecord" },
            { "VvcSubpicIDEntry",                       "VvcSubpicIDEntry" },
            { "VvcSubpicOrderEntry",                    "VvcSubpicOrderEntry" },
            { "URIInitBox",                             "URIInitBox" },
            { "URIBox",                                 "URIBox" },
            { "CleanApertureBox",                       "CleanApertureBox" },
            { "PixelAspectRatioBox",                    "PixelAspectRatioBox" },
            { "DownMixInstructions()[]",               "DownMixInstructions[]" },
            { "DRCCoefficientsBasic()[]",              "DRCCoefficientsBasic[]" },
            { "DRCInstructionsBasic()[]",              "DRCInstructionsBasic[]" },
            { "DRCCoefficientsUniDRC()[]",             "DRCCoefficientsUniDRC[]" },
            { "DRCInstructionsUniDRC()[]",             "DRCInstructionsUniDRC[]" },
            { "HEVCConfigurationBox",                   "HEVCConfigurationBox" },
            { "LHEVCConfigurationBox",                  "LHEVCConfigurationBox" },
            { "AVCConfigurationBox",                    "AVCConfigurationBox" },
            { "SVCConfigurationBox",                    "SVCConfigurationBox" },
            { "ScalabilityInformationSEIBox",           "ScalabilityInformationSEIBox" },
            { "SVCPriorityAssignmentBox",               "SVCPriorityAssignmentBox" },
            { "ViewScalabilityInformationSEIBox",       "ViewScalabilityInformationSEIBox" },
            { "ViewIdentifierBox()",                      "ViewIdentifierBox" },
            { "MVCConfigurationBox",                    "MVCConfigurationBox" },
            { "MVCViewPriorityAssignmentBox",           "MVCViewPriorityAssignmentBox" },
            { "IntrinsicCameraParametersBox",           "IntrinsicCameraParametersBox" },
            { "ExtrinsicCameraParametersBox",           "ExtrinsicCameraParametersBox" },
            { "MVCDConfigurationBox",                   "MVCDConfigurationBox" },
            { "MVDScalabilityInformationSEIBox",        "MVDScalabilityInformationSEIBox" },
            { "A3DConfigurationBox",                    "A3DConfigurationBox" },
            { "VvcOperatingPointsRecord",               "VvcOperatingPointsRecord" },
            { "VVCSubpicIDRewritingInfomationStruct()", "VVCSubpicIDRewritingInfomationStruct" },
            { "MPEG4ExtensionDescriptorsBox ()",        "MPEG4ExtensionDescriptorsBox" },
            { "MPEG4ExtensionDescriptorsBox()",         "MPEG4ExtensionDescriptorsBox" },
            { "MPEG4ExtensionDescriptorsBox",           "MPEG4ExtensionDescriptorsBox" },
            { "bit(8*dci_nal_unit_length)",             "byte[]" },
            { "DependencyInfo[numReferences]",          "DependencyInfo[]" },
            { "VvcPTLRecord(0)[i]",                     "VvcPTLRecord[]" },
            { "EVCSliceComponentTrackConfigurationBox", "EVCSliceComponentTrackConfigurationBox" },
            { "SVCMetadataSampleConfigBox",             "SVCMetadataSampleConfigBox" },
            { "SVCPriorityLayerInfoBox",                "SVCPriorityLayerInfoBox" },
            { "EVCConfigurationBox",                    "EVCConfigurationBox" },
            { "VvcNALUConfigBox",                       "VvcNALUConfigBox" },
            { "VvcConfigurationBox",                    "VvcConfigurationBox" },
            { "HEVCTileConfigurationBox",               "HEVCTileConfigurationBox" },
            { "MetaDataKeyTableBox()",                  "MetaDataKeyTableBox" },
            { "BitRateBox()",                          "BitRateBox" },
            { "char[count]",                            "byte[]" },
            { "signed int(64)[j][k]",                 "long[][]" },
            { "signed int(64)[j]",                    "long[]" },
            { "unsigned int(8)[j][k]",                  "byte[][]" },
            { "unsigned int(8)[j]",                     "byte[]" },
            { "signed int(32)[ c ]",                    "int[]" },
            { "unsigned int(8)[]",                      "byte[]" },
            { "unsigned int(8)[i]",                     "byte[]" },
            { "unsigned int(6)[i]",                     "byte[]" },
            { "unsigned int(6)[i][j]",                  "byte[][]" },
            { "unsigned int(1)[i][j]",                  "byte[][]" },
            { "unsigned int(9)[i]",                     "ushort[]" },
            { "unsigned int(32)[]",                     "uint[]" },
            { "unsigned int(32)[i]",                    "uint[]" },
            { "unsigned int(32)[j]",                    "uint[]" },
            { "char[]",                                 "byte[]" },
            { "loudness[]",                             "int[]" },
            { "ItemPropertyAssociationBox[]",           "ItemPropertyAssociationBox[]" },
            { "string[method_count]",                   "BinaryUTF8String[]" },
            { "ItemInfoExtension(extension_type)",                      "ItemInfoExtension" },
            { "SampleGroupDescriptionEntry(grouping_type)",            "SampleGroupDescriptionEntry" },
            { "SampleEntry()",                            "SampleEntry" },
            { "SampleConstructor()",                      "SampleConstructor" },
            { "InlineConstructor()",                      "InlineConstructor" },
            { "SampleConstructorFromTrackGroup()",        "SampleConstructorFromTrackGroup" },
            { "NALUStartInlineConstructor()",             "NALUStartInlineConstructor" },
            { "MPEG4BitRateBox",                        "MPEG4BitRateBox" },
            { "ChannelLayout()",                          "ChannelLayout" },
            { "UniDrcConfigExtension()",                  "UniDrcConfigExtension" },
            { "SamplingRateBox()",                        "SamplingRateBox" },
            { "TextConfigBox()",                          "TextConfigBox" },
            { "MetaDataKeyTableBox",                    "MetaDataKeyTableBox" },
            { "BitRateBox",                             "BitRateBox" },
            { "CompleteTrackInfoBox",                   "CompleteTrackInfoBox" },
            { "TierDependencyBox()",                      "TierDependencyBox" },
            { "InitialParameterSetBox",                 "InitialParameterSetBox" },
            { "PriorityRangeBox()",                       "PriorityRangeBox" },
            { "ViewPriorityBox",                        "ViewPriorityBox" },
            { "SVCDependencyRangeBox",                  "SVCDependencyRangeBox" },
            { "RectRegionBox",                          "RectRegionBox" },
            { "IroiInfoBox",                            "IroiInfoBox" },
            { "TranscodingInfoBox",                     "TranscodingInfoBox" },
            { "MetaDataKeyDeclarationBox()",              "MetaDataKeyDeclarationBox" },
            { "MetaDataDatatypeBox()",                    "MetaDataDatatypeBox" },
            { "MetaDataLocaleBox()",                      "MetaDataLocaleBox" },
            { "MetaDataSetupBox()",                       "MetaDataSetupBox" },
            { "MetaDataExtensionsBox()",                  "MetaDataExtensionsBox" },
            { "TrackLoudnessInfo[]",                    "TrackLoudnessInfo[]" },
            { "AlbumLoudnessInfo[]",                    "AlbumLoudnessInfo[]" },
            { "VvcPTLRecord(num_sublayers)",            "VvcPTLRecord" },
            { "VvcPTLRecord(ptl_max_temporal_id[i]+1)[i]", "VvcPTLRecord[]" },
            { "OpusSpecificBox",                        "OpusSpecificBox" },
            { "unsigned int(8 * OutputChannelCount)",   "byte[]" },
            { "ChannelMappingTable(OutputChannelCount)",                    "ChannelMappingTable" },
            // descriptors
            { "DecoderConfigDescriptor",                "DecoderConfigDescriptor" },
            { "SLConfigDescriptor",                     "SLConfigDescriptor" },
            { "IPI_DescrPointer[0 .. 1]",               "IPI_DescrPointer" },
            { "IP_IdentificationDataSet[0 .. 255]",     "IP_IdentificationDataSet[]" },
            { "IPMP_DescriptorPointer[0 .. 255]",       "IPMP_DescriptorPointer[]" },
            { "LanguageDescriptor[0 .. 255]",           "LanguageDescriptor" },
            { "QoS_Descriptor[0 .. 1]",                 "QoS_Descriptor" },
            { "ES_Descriptor",                          "ES_Descriptor" },
            { "RegistrationDescriptor[0 .. 1]",         "RegistrationDescriptor" },
            { "ExtensionDescriptor[0 .. 255]",          "ExtensionDescriptor[]" },
            { "ProfileLevelIndicationIndexDescriptor[0..255]", "ProfileLevelIndicationIndexDescriptor[]" },
            { "DecoderSpecificInfo[0 .. 1]",            "DecoderSpecificInfo[]" },
            { "bit(8)[URLlength]",                      "byte[]" },
            { "bit(8)[sizeOfInstance-4]",               "byte[]" },
            { "bit(8)[sizeOfInstance-3]",               "byte[]" },
            { "bit(8)[size-10]",                        "byte[]" },
            { "double(32)",                             "double" },
            { "fixedpoint1616",                         "double" },
            { "QoS_Qualifier[]",                        "QoS_Qualifier[]" },
            { "bslbf(header_size * 8)[]",               "byte[]" },
            { "bslbf(trailer_size * 8)[]",              "byte[]" },
            { "bslbf(aux_size * 8)[]",                  "byte[]" },
            { "bslbf(11)",                              "ushort" },
            { "bslbf(5)",                               "byte" },
            { "bslbf(4)",                               "byte" },
            { "bslbf(2)",                               "byte" },
            { "bslbf(1)",                               "bool" },
            { "uimsbf(32)",                             "uint" },
            { "uimsbf(24)",                             "uint" },
            { "uimsbf(18)",                             "uint" },
            { "uimsbf(16)",                             "ushort" },
            { "uimsbf(14)",                             "ushort" },
            { "uimsbf(12)",                             "ushort" },
            { "uimsbf(10)",                             "ushort" },
            { "uimsbf(8)",                              "byte" },
            { "uimsbf(7)",                              "byte" },
            { "uimsbf(6)",                              "byte" },
            { "uimsbf(5)",                              "byte" },
            { "uimsbf(4)",                              "byte" },
            { "uimsbf(3)",                              "byte" },
            { "uimsbf(2)",                              "byte" },
            { "uimsbf(1)",                              "bool" },
            { "case 1:\r\n    case 2:\r\n    case 3:\r\n    case 4:\r\n    case 6:\r\n    case 7:\r\n    case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n      GASpecificConfig()", "GASpecificConfig" },
            { "case 8:\r\n      CelpSpecificConfig()",  "CelpSpecificConfig" },
            { "case 9:\r\n      HvxcSpecificConfig()",  "HvxcSpecificConfig" },
            { "case 12:\r\n      TTSSpecificConfig()",  "TTSSpecificConfig" },
            { "case 13:\r\n    case 14:\r\n    case 15:\r\n    case 16:\r\n      StructuredAudioSpecificConfig()", "StructuredAudioSpecificConfig" },
            { "case 24:\r\n      ErrorResilientCelpSpecificConfig()", "ErrorResilientCelpSpecificConfig" },
            { "case 25:\r\n      ErrorResilientHvxcSpecificConfig()", "ErrorResilientHvxcSpecificConfig" },
            { "case 26:\r\n    case 27:\r\n      ParametricSpecificConfig()", "ParametricSpecificConfig" },
            { "case 28:\r\n      SSCSpecificConfig()", "SSCSpecificConfig" },
            { "case 30:\r\n      uimsbf(1)",            "bool" },
            { "case 32:\r\n    case 33:\r\n    case 34:\r\n      MPEG_1_2_SpecificConfig()", "MPEG_1_2_SpecificConfig" },
            { "case 35:\r\n      DSTSpecificConfig()", "DSTSpecificConfig" },
            { "case 36:\r\n      bslbf(5)",             "byte" },
            { "case 37:\r\n    case 38:\r\n      SLSSpecificConfig()", "SLSSpecificConfig" },
            { "case 39:\r\n      ELDSpecificConfig(channelConfiguration)", "ELDSpecificConfig" },
            { "case 40:\r\n    case 41:\r\n      SymbolicMusicSpecificConfig()", "SymbolicMusicSpecificConfig" },
            { "case 17:\r\n    case 19:\r\n    case 20:\r\n    case 21:\r\n    case 22:\r\n    case 23:\r\n    case 24:\r\n    case 25:\r\n    case 26:\r\n    case 27:\r\n    case 39:\r\n      bslbf(2)", "byte" },
            { "GetAudioObjectType()",                   "GetAudioObjectType" },
            { "SpatialSpecificConfig",                  "SpatialSpecificConfig" },
            { "ALSSpecificConfig",                      "ALSSpecificConfig" },
            { "ErrorProtectionSpecificConfig",          "ErrorProtectionSpecificConfig" },
            { "program_config_element",                 "program_config_element" },
            { "byte_alignment",                         "byte" },
            { "CelpHeader(samplingFrequencyIndex)",     "CelpHeader" },
            { "CelpBWSenhHeader",                       "CelpBWSenhHeader" },
            { "HVXCconfig",                             "HVXCconfig" },
            { "TTS_Sequence",                           "TTS_Sequence" },
            { "ER_SC_CelpHeader(samplingFrequencyIndex)", "ER_SC_CelpHeader" },
            { "ErHVXCconfig",                           "ErHVXCconfig" },
            { "PARAconfig",                             "PARAconfig" },
            { "HILNenexConfig",                         "HILNenexConfig" },
            { "HILNconfig",                             "HILNconfig" },
            { "ld_sbr_header(channelConfiguration)",                          "ld_sbr_header" },
            { "sbr_header",                             "sbr_header" },
            { "uimsbf(1)[i]",                           "bool[]" },
            { "uimsbf(8)[i]",                           "byte[]" },
            { "bslbf(1)[i]",                            "byte[]" },
            { "uimsbf(4)[i]",                           "byte[]" },
            { "uimsbf(1)[c]",                           "bool[]" },
            { "uimsbf(32)[f]",                          "uint[]" },
            { "uimsbf(6)[i]",                           "byte[]" },
            { "uimsbf(1)[i][j]",                        "bool[][]" },
            { "uimsbf(2)[i][j]",                        "byte[][]" },
            { "uimsbf(4)[i][j]",                        "byte[][]" },
            { "uimsbf(16)[i][j]",                       "ushort[][]" },
            { "uimsbf(7)[i][j]",                        "byte[][]" },
            { "uimsbf(5)[i][j]",                        "byte[][]" },
            { "uimsbf(6)[i][j]",                        "byte[][]" },
            { "CelpHeader",                             "CelpHeader" },
            { "ER_SC_CelpHeader",                       "ER_SC_CelpHeader" },
            { "AV1CodecConfigurationBox",               "AV1CodecConfigurationBox" },
            { "AV1CodecConfigurationRecord",            "AV1CodecConfigurationRecord" },
            { "vluimsbf8",                              "byte" },
            { "byte(urlMIDIStream_length)",             "byte[]" },
            { "aligned bit(3)",                         "byte" },
            { "aligned bit(1)",                         "bool" },
            { "bit",                                    "bool" },
            { "case 0b000:\r\n                mainscore_file", "byte[]" },
            { "case 0b001:\r\n                bit(8) partID; // ID of the part at which the following info refers \r\n                part_file", "byte[]" },
            { "case 0b010:\r\n                // this segment is always in binary as stated in Section 9 \r\n                synch_file", "byte[]" },
            { "case 0b011:\r\n                format_file", "byte[]" },
            { "case 0b100:\r\n                bit(8) partID;\r\n                bit(8) lyricID;\r\n                lyrics_file", "byte[]" },
            { "case 0b101:\r\n                // this segment is always in binary as stated in Section 11.4 \r\n                font_file", "byte[]" },
            { "case 0b110: ",                           "byte[]" },
            { "case 0b111: ",                           "byte[]" },
            { "unsigned int(16)[3]",                    "ushort[]" },
            { "MultiLanguageString[]",                  "MultiLanguageString[]" },
            { "AdobeChapterRecord[]",                   "AdobeChapterRecord[]" },
            { "ThreeGPPKeyword[]",                      "ThreeGPPKeyword[]" },
            { "IodsSample[]",                           "IodsSample[]" },
            { "XtraTag[]",                              "XtraTag[]" },
            { "XtraValue[count]",                       "XtraValue[]" },
            { "ViprEntry[]",                            "ViprEntry[]" },
            { "TrickPlayEntry[]",                       "TrickPlayEntry[]" },
            { "MtdtEntry[ entry_count ]",               "MtdtEntry[]" },
            { "RectRecord",                             "RectRecord" },
            { "StyleRecord",                            "StyleRecord" },
            { "ProtectionSystemSpecificKeyID[count]",   "ProtectionSystemSpecificKeyID[]" },
            { "unsigned int(8)[contentIDLength]",       "byte[]" },
            { "unsigned int(8)[contentTypeLength]",     "byte[]" },
            { "unsigned int(8)[rightsIssuerLength]",    "byte[]" },
            { "unsigned int(8)[textualHeadersLength]",  "byte[]" },
            { "unsigned int(8)[count]",                 "byte[]" },
            { "unsigned int(8)[4]",                     "byte[]" },
            { "unsigned int(8)[14]",                    "byte[]" },
            { "unsigned int(8)[6]",                     "byte[]" },
            { "unsigned int(8)[256]",                   "byte[]" },
            { "unsigned int(8)[512]",                   "byte[]" },
            { "char[tagLength]",                        "byte[]" },
            { "unsigned int(8)[constant_IV_size]",      "byte[]" },
            { "unsigned int(8)[length-6]",              "byte[]" },
            { "unsigned int(Per_Sample_IV_Size*8)",     "byte[]" },
            { "EC3SpecificEntry[numIndSub + 1]",        "EC3SpecificEntry[]" },
            { "SampleEncryptionSubsample(version)[subsample_count]", "SampleEncryptionSubsample[]" },
            { "SampleEncryptionSample(version, flags, Per_Sample_IV_Size)[sample_count]", "SampleEncryptionSample[]" },
            { "CelpSpecificConfig()",                   "CelpSpecificConfig" },
            { "HvxcSpecificConfig()",                   "HvxcSpecificConfig" },
            { "TTSSpecificConfig()",                    "TTSSpecificConfig" },
            { "ErrorResilientCelpSpecificConfig()",     "ErrorResilientCelpSpecificConfig" },
            { "ErrorResilientHvxcSpecificConfig()",     "ErrorResilientHvxcSpecificConfig" },
            { "SSCSpecificConfig()",                    "SSCSpecificConfig" },
            { "DSTSpecificConfig()",                    "DSTSpecificConfig" },
            { "ELDSpecificConfig(channelConfiguration)","ELDSpecificConfig" },
            { "GASpecificConfig()",                     "GASpecificConfig" },
            { "StructuredAudioSpecificConfig()",        "StructuredAudioSpecificConfig" },
            { "ParametricSpecificConfig()",             "ParametricSpecificConfig" },
            { "MPEG_1_2_SpecificConfig()",              "MPEG_1_2_SpecificConfig" },
            { "SLSSpecificConfig()",                    "SLSSpecificConfig" },
            { "SymbolicMusicSpecificConfig()",          "SymbolicMusicSpecificConfig" },
        };
        if (map.ContainsKey(type))
            return map[type];
        else if (map.ContainsKey(type.Replace("()", "")))
            return map[type.Replace("()", "")];
        else
            throw new NotSupportedException(type);
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