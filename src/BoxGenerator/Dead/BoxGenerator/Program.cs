using Newtonsoft.Json;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BoxGenerator;

public enum MethodType
{
    Read,
    Write,
    Size
}

partial class Program
{
    static Dictionary<string, PseudoClass> parsedClasses = null;

    static void Main(string[] args)
    {
        string[] jsonFiles = Directory.GetFiles("Definitions", "*.json");

        int success = 0;
        int duplicated = 0;
        int fail = 0;

        parsedClasses = new Dictionary<string, PseudoClass>();
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
                        if (con.StartsWith("{"))
                        {
                            var des = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(con);
                            foreach (var dc in des.Values)
                            {
                                foreach (var ddd in dc)
                                    containers.Add(ddd);
                            }
                        }
                        else
                        {
                            containers.Add(con);
                        }
                    }

                    if (string.IsNullOrEmpty(sample))
                    {
                        continue;
                    }

                    try
                    {
                        var result = Parser.Boxes.ParseOrThrow(sample);
                        long offset = 0;
                        result = DeduplicateBoxes(result);

                        foreach (var item in result)
                        {
                            item.Syntax = GetSampleCode(sample, offset, item.CurrentOffset);
                            offset = item.CurrentOffset;

                            if (parsedClasses.TryAdd(item.BoxName, item))
                            {
                                success++;
                            }
                            else
                            {
                                duplicated++;
                                duplicates.Add(item);
                                Debug.WriteLine($"Duplicated: {item.BoxName}");

                                if (item.Extended != null && item.Extended.BoxType != parsedClasses[item.BoxName].Extended.BoxType)
                                {
                                    if (!parsedClasses.TryAdd($"{item.BoxName}_{item.Extended.BoxType}", item))
                                    {
                                        LogConsole($"Duplicated and failed to add: {item.BoxName}");
                                    }
                                    // override the name
                                    item.BoxName = $"{item.BoxName}_{item.Extended.BoxType}";
                                }
                            }
                        }

                        Debug.WriteLine($"Succeeded parsing: {fourCC}");

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        LogConsole($"Failed to parse: {fourCC}");
                        fail++;
                    }
                }
            }
        }

        // post-processing
        containers = containers.Distinct().ToList();

        foreach (var item in parsedClasses)
        {
            // determine the box type
            var ancestors = GetClassAncestors(item.Value.BoxName);

            // TODO: fix duplicated code
            if (ancestors.LastOrDefault().Extended.BoxName != null && ancestors.LastOrDefault().Extended.BoxName.EndsWith("Box"))
                item.Value.ParsedBoxType = ParsedBoxType.Box;
            else if (ancestors.LastOrDefault().Extended.BoxName != null && ancestors.LastOrDefault().Extended.BoxName.EndsWith("Descriptor") || (ancestors.FirstOrDefault().Extended != null && !string.IsNullOrEmpty(ancestors.FirstOrDefault().Extended.DescriptorTag)))
                item.Value.ParsedBoxType = ParsedBoxType.Descriptor;
            else if (ancestors.LastOrDefault().BoxName == "SampleGroupDescriptionEntry")
                item.Value.ParsedBoxType = ParsedBoxType.Entry;
            else
                item.Value.ParsedBoxType = ParsedBoxType.Class;

            // flatten fields
            var fields = FlattenFields(item.Value.Fields);

            // fill-in missing fields
            // TODO: ctor content is still C#
            if (item.Value.BoxName == "GASpecificConfig" || item.Value.BoxName == "SLSSpecificConfig")
            {
                fields.Add(new PseudoField() { Name = "audioObjectType", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                fields.Add(new PseudoField() { Name = "channelConfiguration", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
                fields.Add(new PseudoField() { Name = "samplingFrequencyIndex", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
                item.Value.CtorContent = "\t\tthis.audioObjectType = audioObjectType;\r\n\t\tthis.channelConfiguration = channelConfiguration;\r\n\t\tthis.samplingFrequencyIndex = samplingFrequencyIndex;\r\n";
            }
            else if (item.Value.BoxName == "CelpSpecificConfig" || item.Value.BoxName == "ER_SC_CelpHeader" || item.Value.BoxName == "ErrorResilientCelpSpecificConfig")
            {
                fields.Add(new PseudoField() { Name = "samplingFrequencyIndex", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
                item.Value.CtorContent = "\t\tthis.samplingFrequencyIndex = samplingFrequencyIndex;\r\n";
            }
            else if (item.Value.BoxName == "SSCSpecificConfig" || item.Value.BoxName == "ld_sbr_header" || item.Value.BoxName == "ELDSpecificConfig")
            {
                fields.Add(new PseudoField() { Name = "channelConfiguration", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("signed"), "int", new Maybe<string>("(32)")) });
                item.Value.CtorContent = "\t\tthis.channelConfiguration = channelConfiguration;\r\n";
            }
            else if (item.Value.BoxName == "VvcPTLRecord")
            {
                fields.Add(new PseudoField() { Name = "num_sublayers", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                item.Value.CtorContent = "\t\tthis.num_sublayers = num_sublayers;\r\n";
            }
            else if (item.Value.BoxName == "ChannelMappingTable")
            {
                fields.Add(new PseudoField() { Name = "OutputChannelCount", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                item.Value.CtorContent = "\t\tthis.OutputChannelCount = OutputChannelCount;\r\n";
            }
            else if (item.Value.BoxName == "SampleEncryptionSample")
            {
                fields.Add(new PseudoField() { Name = "version", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                fields.Add(new PseudoField() { Name = "flags", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(24)")) });
                fields.Add(new PseudoField() { Name = "Per_Sample_IV_Size", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                item.Value.CtorContent = "\t\tthis.version = version;\r\n\t\tthis.flags = flags;\r\n\t\tthis.Per_Sample_IV_Size = Per_Sample_IV_Size;\r\n";
            }
            else if (item.Value.BoxName == "SampleEncryptionSubsample")
            {
                fields.Add(new PseudoField() { Name = "version", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                item.Value.CtorContent = "\t\tthis.version = version;\r\n";
            }
            else if (item.Value.BoxName == "TrunEntry")
            {
                fields.Add(new PseudoField() { Name = "version", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")) });
                fields.Add(new PseudoField() { Name = "flags", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(24)")) });
                item.Value.CtorContent = "\t\tthis.version = version;\r\n\t\t this.flags = flags;";
            }
            else if (item.Value.BoxName == "BoxHeader")
            {
                item.Value.CtorContent = "\t\tthis.type = boxtype;\r\n\t\tthis.usertype = extended_type;\r\n";
            }
            else if (item.Value.BoxName == "FullBox")
            {
                item.Value.CtorContent = "\t\tthis.version = v;\r\n\t\t this.flags = f;";
            }
            else if (item.Value.BoxName == "SampleEncryptionBox")
            {
                fields.Add(new PseudoField() { Name = "Per_Sample_IV_Size", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>("unsigned"), "int", new Maybe<string>("(8)")), Value = " = 16; // TODO: get from the 'tenc' box" });
            }
            else if (item.Value.BoxName == "SampleGroupDescriptionEntry")
            {
                fields.Add(new PseudoField() { Name = "children", Type = new PseudoType(new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), new Maybe<string>(), "Box", new Maybe<string>()), FieldArray = "[]" });
            }

            item.Value.FlattenedFields = fields;

            // determine container
            item.Value.IsContainer =
                string.IsNullOrEmpty(item.Value.Abstract) &&
                item.Value.BoxName != "MetaDataAccessUnit" &&
                item.Value.BoxName != "ItemReferenceBox" &&
                item.Value.BoxName != "SampleGroupDescriptionBox" &&
                item.Value.BoxName != "SampleGroupDescriptionEntry" &&
                (
                    item.Value.Syntax.Contains("other boxes from derived specifications") ||
                    (item.Value.Extended != null && containers.Contains(item.Value.Extended.BoxType)) || containers.Contains(item.Value.BoxName) ||
                    item.Value.FlattenedFields.FirstOrDefault(x =>
                        x.Type.Type == "Box" || GetClassAncestors(x.Type.Type).LastOrDefault(c => c != null && c.BoxName.EndsWith("Box")) != null) != null
                ) ||
                item.Value.BoxName == "DefaultHevcExtractorConstructorBox"; // DefaultHevcExtractorConstructorBox is a container, but the *constructor boxes have currently unknown syntax
        }

        // collect all boxes, entries and descriptors
        SortedDictionary<string, List<PseudoClass>> boxes = new SortedDictionary<string, List<PseudoClass>>();
        SortedDictionary<string, List<PseudoClass>> entries = new SortedDictionary<string, List<PseudoClass>>();
        foreach (var item in parsedClasses)
        {
            if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.BoxType))
            {
                if (item.Value.ParsedBoxType == ParsedBoxType.Box)
                {
                    if (!boxes.ContainsKey(item.Value.Extended.BoxType))
                        boxes.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                    else
                        boxes[item.Value.Extended.BoxType].Add(item.Value);
                }
                else if(item.Value.ParsedBoxType == ParsedBoxType.Entry)
                { 
                    if (!entries.ContainsKey(item.Value.Extended.BoxType))
                        entries.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                    else
                        entries[item.Value.Extended.BoxType].Add(item.Value);
                }
            }
        }

        SortedDictionary<string, List<PseudoClass>> descriptors = new SortedDictionary<string, List<PseudoClass>>();
        foreach (var item in parsedClasses)
        {
            if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.DescriptorTag) && string.IsNullOrEmpty(item.Value.Abstract))
            {
                if (!descriptors.ContainsKey(item.Value.Extended.DescriptorTag))
                    descriptors.Add(item.Value.Extended.DescriptorTag, new List<PseudoClass>() { item.Value });
                else
                    descriptors[item.Value.Extended.DescriptorTag].Add(item.Value);
            }
        }

        // add additional boxes
        string[] audioSampleEntryTypes = new string[]
        {
            "samr","sawb","mp4a","drms","alac","owma","ac-3","ec-3","mlpa","dtsl","dtsh","dtse","Opus","enca","resa","sevc","sqcp","ssmv","lpcm","dtsc","sowt",
        };
        string[] visualSampleEntryTypes = new string[]
        {
            "mp4v","s263","drmi","encv","resv","icpv","hvc1","hvc2","hvc3","lhv1","lhe1","hev1","hev2","hev3","avcp","mvc1","mvc2","mvc3","mvc4","mvd1","mvd2",
            "mvd3","mvd4","a3d1","a3d2","a3d3","a3d4","svc1","svc2","hvt1","lht1","hvt3","hvt2","vvc1","vvi1","vvs1","vvcN","evc1","evs1","evs2","av01","avc1",
            "avc2","avc3","avc4","vp08","vp09","vp10","apcn","dvhe","dvav",
        };

        foreach (var type in audioSampleEntryTypes)
        {
            if (!boxes.ContainsKey(type))
                boxes.Add(type, new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "AudioSampleEntry").Value });
            else
                boxes[type] = new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "AudioSampleEntry").Value };
        }

        foreach (var type in visualSampleEntryTypes)
        {
            if (!boxes.ContainsKey(type))
                boxes.Add(type, new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "VisualSampleEntry").Value });
            else
                boxes[type] = new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "VisualSampleEntry").Value };
        }

        if (!boxes.ContainsKey("mp4s"))
            boxes.Add("mp4s", new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "MpegSampleEntry").Value });
        else
            boxes["mp4s"] = new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "MpegSampleEntry").Value };

        if (!boxes.ContainsKey("dimg"))
            boxes.Add("dimg", new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "SingleItemTypeReferenceBox").Value });

        if (!boxes.ContainsKey("cdsc"))
            boxes.Add("cdsc", new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "TrackReferenceTypeBox").Value });

        LogConsole($"Successful: {success}, Failed: {fail}, Duplicated: {duplicated}, Total: {success + fail + duplicated}");

        string code = GenerateParser(containers, boxes, entries, descriptors);
    }

    private static string GenerateParser(List<string> containers, SortedDictionary<string, List<PseudoClass>> boxes, SortedDictionary<string, List<PseudoClass>> entries, SortedDictionary<string, List<PseudoClass>> descriptors)
    {
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
        foreach (var item in boxes)
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
                        optParams = $"IsoStream.FromFourCC(\"{item.Key}\")";

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
                if (fourCC[0] == '\0') return new IlstKey(IsoStream.FromFourCC(fourCC));
            }
            else if(uuid != null)
            {
                Log.Debug($""Unknown 'uuid' box: '{fourCC}'"");
                return new UserBox(uuid);
            }

            //throw new NotImplementedException(fourCC);
            Log.Debug($""Unknown box: '{fourCC}'"");
            return new UnknownBox(IsoStream.FromFourCC(fourCC));
        }";


        factory += @"
        public static SampleGroupDescriptionEntry CreateEntry(string fourCC)
        {
            switch(fourCC)
            {
               ";
        foreach (var item in entries)
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
                    optParams = $"IsoStream.FromFourCC(\"{item.Key}\")";
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
            return new UnknownEntry(IsoStream.FromFourCC(fourCC));
        }
";

        factory += @"
        public static Descriptor CreateDescriptor(byte tag)
        {
            switch (tag)
            {
";
        foreach (var item in descriptors)
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

        foreach (var b in parsedClasses.Values.ToArray())
        {
            string code = BuildCode(b, containers);
            resultCode += code + "\r\n\r\n";
        }

        resultCode +=
@"
}
";
        return resultCode;
    }

    private static void LogConsole(string v)
    {
        Debug.WriteLine(v);
        Console.WriteLine(v);
    }

    private static PseudoClass[] GetClassAncestors(string item)
    {
        // find all ancestors of the box/entry/class/descriptor - this allows us to determine the type of the class
        List<PseudoClass> extended = new List<PseudoClass>();

        // right now this algorithm is terribly inefficient, but it works
        PseudoClass it = parsedClasses.Values.SingleOrDefault(x => x.BoxName == item);
        if(it != null)
            extended.Add(it);
        
        while (it != null)
        {
            it = parsedClasses.Values.SingleOrDefault(x => x.BoxName == it.Extended.BoxName);

            if (it == null)
                break;

            extended.Add(it);
        }

        return extended.ToArray();
    }

    private static IEnumerable<PseudoClass> DeduplicateBoxes(IEnumerable<PseudoClass> result)
    {
        // some boxes have FourCC such as "'avc1' or 'avc3'" - make it two boxes
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
                    var copy = JsonConvert.DeserializeObject<PseudoClass>(
                        JsonConvert.SerializeObject(box, 
                            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }), 
                            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }); // deep copy
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
        // parse the syntax of this particular class as there can be multiple boxes/classes in a single JSON record
        return sample.Substring((int)startOffset, (int)(endOffset - startOffset));
    }

    private static string Sanitize(string name)
    {
        // in C#, we cannot use "namespace" as variable name
        if (name == "namespace")
            return "ns";
        return name;
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
                name = GetType(field as PseudoField)?.Replace("[]", "");
            }
        }

        return name;
    }

    private static List<PseudoField> FlattenFields(IEnumerable<PseudoCode> fields, PseudoBlock parent = null)
    {
        Dictionary<string, PseudoField> ret = new Dictionary<string, PseudoField>();
        foreach (var code in fields)
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
            }
            else if (GetFieldType(field) == GetFieldType(ret[name]) && GetNestedInLoop(field) == GetNestedInLoop(ret[name]))
            {
                //Debug.WriteLine($"-Resolved: fields are the same");
            }
            else
            {
                // try to resolve the conflict using the type size
                var type1 = GetTypeInfo(field);
                var type2 = GetTypeInfo(ret[name]);
                
                if (type1.ElementSizeInBits > type2.ElementSizeInBits)
                    ret[name] = field;
                if (type1.ElementSizeInBits != type2.ElementSizeInBits)
                    return;

                if (GetFieldType(field) == "unsigned int(64)[ entry_count ]" && GetFieldType(ret[name]) == "unsigned int(32)[ entry_count ]")
                {
                    ret[name] = field;
                    return;
                }
                else if (GetFieldType(field) == "unsigned int(16)[3]" && GetFieldType(ret[name]) == "unsigned int(32)[3]")
                {
                    return;
                }
                else if (GetFieldType(field) == "aligned bit(1)" && GetFieldType(ret[name]) == "bit")
                {
                    return;
                }

                throw new Exception($"Duplicated fileds: {name} of types: {field.Type}, {ret[name].Type}");
            }
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

    public static string GetSpacing(int level)
    {
        string ret = "";
        for (int i = 0; i < level; i++)
        {
            ret += "\t";
        }
        return ret;
    }

    private static bool IsWorkaround(string type)
    {
        return Parser.Workarounds.Contains(type);
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

        string ctorContent = b.CtorContent;
        var fields = b.FlattenedFields;    
               

        bool hasDescriptors = fields.Select(x => GetReadMethod(x).Contains("ReadDescriptor(")).FirstOrDefault(x => x == true) != false && b.BoxName != "ESDBox" && b.BoxName != "MpegSampleEntry" && b.BoxName != "MPEG4ExtensionDescriptorsBox" && b.BoxName != "AppleInitialObjectDescriptorBox" && b.BoxName != "IPMPControlBox" && b.BoxName != "IPMPInfoBox";

        foreach (var field in fields)
        {
            cls += "\r\n" + BuildField(b, field);
        }

        if(b.BoxName == "MetaBox")
        {
            // This is very badly documented, but in case the Apple "ilst" box appears in "meta" inside "trak" or "udta", then the 
            //  serialization is incompatible with ISOBMFF as it uses the "Quicktime atom format".
            //  see: https://www.academia.edu/66625880/Forensic_Analysis_of_Video_Files_Using_Metadata
            //  see: https://web.archive.org/web/20220126080109/https://leo-van-stee.github.io/
            // TODO: maybe instead of lookahead, we just have to check the parents
            cls += "\r\npublic bool IsQuickTime { get { return (Parent != null && (((Box)Parent).FourCC == IsoStream.FromFourCC(\"udta\") || ((Box)Parent).FourCC == IsoStream.FromFourCC(\"trak\"))); } }";
        }
        else if(b.BoxName == "AVCDecoderConfigurationRecord")
        {
            cls += "\r\npublic bool HasExtensions { get; set; } = false;";
        }
        else if(b.BoxName == "FullBox")
        {
            cls += "\r\npublic FullBox(uint boxtype, byte[] uuid) : base(boxtype, uuid) { }\r\n";
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
                base4cc = $"IsoStream.FromFourCC(\"{b.Extended.BoxType}\")";
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

        if (b.IsContainer)
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

        if (b.IsContainer)
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

        if (b.IsContainer)
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

    private static string GetCtorParams(string classType, IList<(string Name, string Value)> parameters)
    {
        if (!string.IsNullOrEmpty(classType) && classType != "()")
        {
            Dictionary<string, string> map = new Dictionary<string, string>() {
            { "(unsigned int(32) format)",          "uint format" },
            { "(bit(24) flags)",                    "uint flags = 0" },
            { "(fmt)",                              "uint fmt = 0" },
            { "(codingname)",                       "uint codingname = 0" },
            { "(handler_type)",                     "uint handler_type = 0" },
            { "(referenceType)",                    "uint referenceType" },
            { "(unsigned int(32) reference_type)",  "uint reference_type" },
            { "(grouping_type, version, flags)",    "uint grouping_type, byte version, uint flags" }, 
            { "(boxtype = 'msrc')",                 "uint boxtype = 1836282467" }, // msrc
            { "(name)",                             "uint name" },
            { "(uuid)",                             "byte[] uuid" },
            { "(property_type)",                    "uint property_type" },
            { "(channelConfiguration)",             "int channelConfiguration" },
            { "(num_sublayers)",                    "byte num_sublayers" },
            { "(code)",                             "uint code" },
            { "(property_type, version, flags)",    "uint property_type, byte version, uint flags" },
            { "(samplingFrequencyIndex, channelConfiguration, audioObjectType)", "int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType" },
            { "(samplingFrequencyIndex,\r\n  channelConfiguration,\r\n  audioObjectType)", "int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType" },
            { "(unsigned int(32) extension_type)",  "uint extension_type" },
            { "('vvcb', version, flags)",           "byte version = 0, uint flags = 0" },
            { "(\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type)", "uint boxtype = 0, byte[] extended_type = null" },
            { "(unsigned int(32) grouping_type)",   "uint grouping_type" },
            { "(unsigned int(32) boxtype, unsigned int(8) v, bit(24) f)", "uint boxtype, byte v = 0, uint f = 0" },
            { "(unsigned int(8) OutputChannelCount)", "byte OutputChannelCount" },
            { "(entry_type, bit(24) flags)",        "uint entry_type, uint flags" },
            { "(samplingFrequencyIndex)",           "int samplingFrequencyIndex" },
            { "(version, flags, Per_Sample_IV_Size)",  "byte version, uint flags, byte Per_Sample_IV_Size" },
            { "(version, flags)",                   "byte version, uint flags" },
            { "(version)",                          "byte version" },
            };
            return map[classType];
        }
        else if(parameters != null)
        {
            string joinedParams = string.Join(", ", parameters.Select(x => x.Name + (string.IsNullOrEmpty(x.Value) ? "" : " = " + x.Value)));
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "loudnessType",           "uint loudnessType" },
                { "local_key_id",           "uint local_key_id" },
                { "protocol",               "uint protocol" },
                { "0, 0",                   "" },
                { "size",                   "ulong size = 0" },
                { "type",                   "uint type" },
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

    private static string FixMissingMethodCode(PseudoClass box, string cls, MethodType methodType)
    {
        if(box.BoxName == "SampleDependencyBox")
        {
            cls += "\r\n\t\tint sample_count = 0; // TODO: taken from the stsz sample_count\r\n";
        }
        else if (box.BoxName == "SampleDependencyTypeBox")
        {
            if(methodType == MethodType.Read)
                cls += "\r\n\t\tint sample_count = (int)((readSize - boxSize) >> 3); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 1 byte per sample\r\n";
            else
                cls += "\r\n\t\tint sample_count = is_leading.Length;\r\n";
        }
        else if (box.BoxName == "DegradationPriorityBox")
        {
            if (methodType == MethodType.Read)
                cls += "\r\n\t\tint sample_count = (int)((readSize - boxSize) >> 4); // should be taken from the stsz sample_count, but we can calculate it from the readSize - 2 bytes per sample\r\n";
            else
                cls += "\r\n\t\tint sample_count = priority.Length;\r\n";
        }
        else if (box.BoxName == "DownMixInstructions")
        {
            cls += "\r\n\t\tint baseChannelCount = 0; // TODO: get somewhere";
        }
        else if (box.BoxName == "CompactSampleToGroupBox")
        {
            cls += "\r\n\t\tbool grouping_type_parameter_present = (flags & (1 << 6)) == (1 << 6);\r\n";
            cls += "\t\tuint count_size_code = (flags >> 2) & 0x3;\r\n";
            cls += "\t\tuint pattern_size_code = (flags >> 4) & 0x3;\r\n";
            cls += "\t\tuint index_size_code = flags & 0x3;\r\n";
        }
        else if (box.BoxName == "VvcDecoderConfigurationRecord")
        {
            cls += "\r\n\t\tconst int OPI_NUT = 12;\r\n";
            cls += "\t\tconst int DCI_NUT = 13;\r\n";
        }
        else if (box.BoxName == "ld_sbr_header")
        {
            cls += "\r\n\t\tint numSbrHeader = 0;\r\n";
        }
        else if (box.BoxName == "ELDSpecificConfig")
        {
            cls += "\r\n\t\tint len = 0;\r\n";
            cls += "\r\n\t\tconst byte ELDEXT_TERM = 0;\r\n";
        }
        else if (box.BoxName == "CelpHeader" || box.BoxName == "ER_SC_CelpHeader")
        {
            cls += "\r\n\t\tconst bool RPE = true;\r\n";
            cls += "\r\n\t\tconst bool MPE = false;\r\n";
        }
        
        return cls;
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

                string tt = GetType(field as PseudoField);
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

                string readMethod = GetReadMethod(field as PseudoField);
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
            throw new NotSupportedException();
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
        string m = methodType == MethodType.Read ? GetReadMethod(field as PseudoField) : (methodType == MethodType.Write ? GetWriteMethod(field as PseudoField) : GetCalculateSizeMethod(field as PseudoField));
        string typedef = "";
        typedef = GetFieldTypeDef(field);

        string fieldComment = "";
        if (!string.IsNullOrEmpty((field as PseudoField)?.Comment))
        {
            fieldComment = "//" + (field as PseudoField).Comment;
        }

        string boxSize = "boxSize += ";

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
                m = FixNestedInLoopVariables(field, m, "", " ");
            }
            else
            {
                m = FixNestedInLoopVariables(field, m, "", " ");
            }
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

    private static string GetFieldTypeDef(PseudoCode field)
    {
        //string[] wrong = ["[count]", "[ entry_count ]", "[numReferences]", "[0 .. 255]", "[0..1]", "[0 .. 1]", "[0..255]", "[ sample_count ]", "[sample_count]", "[subsample_count]",
        //    "[method_count]","[URLlength]","[sizeOfInstance-4]","[sizeOfInstance-3]","[size-10]","[tagLength]","[length-6]","[3]","[16]","[14]","[4]","[6]","[32]","[36]","[256]","[512]",
        //    "[chunk_length]","[contentIDLength]","[contentTypeLength]","[rightsIssuerLength]","[textualHeadersLength]","[numIndSub + 1]"];
        string[] correct = ["[ c ]", "[i]", "[j][k]", "[j]", "[grid_pos_view_id[i]]", "[i][j]", "[c]", "[f]"];

        string value = (field as PseudoField)?.Value;
        if (!string.IsNullOrEmpty(value) && value.StartsWith("[") && value != "[]" && correct.Contains(value))
        {
            // TODO: maybe check instead whether we are in a loop?
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
                condition = condition.Replace("codingname", "FourCC");
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
                            bool hasBoxes = GetReadMethod(req).Contains("ReadBox(") && b.BoxName != "MetaDataAccessUnit" && b.BoxName != "SampleGroupDescriptionBox";
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

                            string variableType = GetType(req);
                            int indexesTypeDef = GetFieldTypeDef(req).Count(x => x == '[');
                            int indexesType = variableType.Count(x => x == '[');
                            string variableName = GetFieldName(req) + suffix;
                            if (variableType.Contains("[]"))
                            {
                                int diff = (indexesType - indexesTypeDef);
                                variableType = variableType.Replace("[]", "");
                                variableType = $"{variableType}[IsoStream.GetInt({variable})]";
                                for (int i = 0; i < diff; i++)
                                {
                                    variableType += "[]";
                                }
                            }
                            else
                            {
                                variableType = variableType + $"[IsoStream.GetInt({variable})]";
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

        parts[1] = parts[1].Replace("pattern_length[j]", "IsoStream.GetInt(pattern_length[j])");
        
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

    private static string GetReadMethod(PseudoField field)
    {
        var info = GetTypeInfo(field);

        string p = "";
        if(!string.IsNullOrEmpty(field.Type.Param))
            p = field.Type.Param.Substring(1, field.Type.Param.Length - 2)
                .Replace("audioObjectType", "audioObjectType.AudioObjectType")
                .Replace("ptl_max_temporal_id[i]+1", "(byte)(ptl_max_temporal_id[i]+1)"); // TODO: fix this workaround

        string csharpResult = "";
        if (info.IsClass || info.IsEntry)
        {
            string factory;
            if (info.Type == "SampleGroupDescriptionEntry") // info.IsEntry? 
            {
                factory = $"() => BoxFactory.CreateEntry(IsoStream.ToFourCC(grouping_type))";
            }
            else
            {
                factory = $"() => new {info.Type}({p})";
            }

            if (info.IsArray)
            {
                string arrayLength = info.ArrayLengthVariable.TrimStart('[').TrimEnd(']');

                string[] correct = ["[ c ]", "[i]", "[j][k]", "[j]", "[grid_pos_view_id[i]]", "[i][j]", "[c]", "[f]"];
                if (correct.Contains(info.ArrayLengthVariable))
                {
                    csharpResult = $"stream.ReadClass(boxSize, readSize, this, {factory}, ";
                }
                else
                {
                    if (string.IsNullOrEmpty(arrayLength))
                        arrayLength = "uint.MaxValue";
                    csharpResult = $"stream.ReadClass(boxSize, readSize, this, (uint)({arrayLength}), {factory}, ";
                }
            }
            else
            {
                csharpResult = $"stream.ReadClass(boxSize, readSize, this, {factory}, ";
            }
        }
        else if (info.IsBox)
        {
            csharpResult = $"stream.ReadBox(boxSize, readSize, this, ";
        }
        else if (info.IsDescriptor)
        {
            csharpResult = $"stream.ReadDescriptor(boxSize, readSize, this, ";
        }
        else if (info.IsByteAlignment)
        {
            csharpResult = "stream.ReadByteAlignment(";
        }
        else if (info.IsString)
        {
            string arraySuffix = "";
            string arrayParam = "";

            if (info.IsArray)
            {
                string arrayLength = info.ArrayLengthVariable.TrimStart('[').TrimEnd(']');

                if (int.TryParse(arrayLength, out int arrayLen))
                {
                    arraySuffix = "Array";
                    arrayParam = $"{arrayLength}, ";
                }
                else if (string.IsNullOrEmpty(arrayLength))
                {
                    arraySuffix = "Array";
                    arrayParam = "";
                }
                else
                {
                    arraySuffix = "Array";
                    arrayParam = $"(uint)({arrayLength}), ";
                }

                csharpResult = $"stream.ReadString{arraySuffix}({arrayParam}";
            }

            if (info.Type == "MultiLanguageString")
            {
                // TODO array
                csharpResult = "stream.ReadStringSizeLangPrefixed(boxSize, readSize, ";
            }
            else
            {
                csharpResult = $"stream.ReadStringZeroTerminated{arraySuffix}(boxSize, readSize, {arrayParam}";
            }
        }
        else if (info.IsNumber)
        {
            string arraySuffix = "";
            string arrayParam = "";

            if (info.IsArray)
            {
                string[] correct = ["[ c ]", "[i]", "[j][k]", "[j]", "[grid_pos_view_id[i]]", "[i][j]", "[c]", "[f]"];
                if (correct.Contains(info.ArrayLengthVariable))
                {
                    // nothing
                }
                else
                {
                    string arrayLength = info.ArrayLengthVariable.TrimStart('[').TrimEnd(']');

                    if (int.TryParse(arrayLength, out int arrayLen))
                    {
                        arraySuffix = "Array";
                        arrayParam = $"boxSize, readSize, {arrayLength}, ";
                    }
                    else if (string.IsNullOrEmpty(arrayLength))
                    {
                        arraySuffix = "ArrayTillEnd";
                        arrayParam = "boxSize, readSize, ";
                    }
                    else
                    {
                        arraySuffix = "Array";
                        arrayParam = $"boxSize, readSize, (uint)({arrayLength}), ";
                    }
                }
            }

            if (!info.IsFloatingPoint)
            {
                if (info.ElementSizeInBits == 1)
                {
                    csharpResult = "stream.ReadBit(";
                }
                else if (info.ElementSizeInBits == -1)
                {
                    string elSizeVar = info.ElementSizeVariable
                        .Replace("8 ceil(size / 8) – size", "(Math.Ceiling(size / 8d) - size) * 8")
                        .Replace("f(pattern_size_code)", "pattern_size_code")
                        .Replace("f(count_size_code)", "count_size_code")
                        .Replace("f(index_size_code)", "index_size_code")
                        ;
                    csharpResult = $"stream.ReadBits{arraySuffix}((uint)({elSizeVar} ), ";
                }
                else if (info.ElementSizeInBits > 1 && info.ElementSizeInBits % 8 > 0)
                {
                    csharpResult = $"stream.ReadBits({info.ElementSizeInBits}, ";
                }
                else if (info.ElementSizeInBits % 8 == 0)
                {
                    if (info.IsSigned)
                        csharpResult = $"stream.ReadInt{info.ElementSizeInBits}{arraySuffix}({arrayParam}";
                    else
                        csharpResult = $"stream.ReadUInt{info.ElementSizeInBits}{arraySuffix}({arrayParam}";
                }
            }
            else
            {
                if (info.ElementSizeInBits == 32)
                {
                    csharpResult = "stream.ReadDouble32(";
                }
                else
                {
                    throw new NotSupportedException($"{info.Type} is unknown");
                }
            }
        }

        if (info.Type == "string" && !info.IsSigned && info.ArrayDimensions == 0 && info.ElementSizeInBits == 15 && info.ArrayLengthVariable == "")
        {
            csharpResult = $"stream.ReadIso639(";
        }

        // TODO: fix this workaround
        csharpResult = csharpResult.Replace("constant_IV_size", "IsoStream.GetInt(constant_IV_size)");

        return csharpResult;
    }

    private static string GetWriteMethod(PseudoField field)
    {
        var info = GetTypeInfo(field);      

        string csharpResult = "";
        if (info.IsClass || info.IsEntry)
            csharpResult = "stream.WriteClass(";
        else if (info.IsBox)
            csharpResult = "stream.WriteBox(";
        else if (info.IsDescriptor)
            csharpResult = "stream.WriteDescriptor(";
        else if (info.IsByteAlignment)
            csharpResult = "stream.WriteByteAlignment(";
        else if (info.IsString)
        {
            string arraySuffix = "";
            string arrayParam = "";

            if (info.IsArray)
            {
                string arrayLength = info.ArrayLengthVariable.TrimStart('[').TrimEnd(']');

                if (int.TryParse(arrayLength, out int arrayLen))
                {
                    arraySuffix = "Array";
                    arrayParam = $"{arrayLength}, ";
                }
                else if (string.IsNullOrEmpty(arrayLength))
                {
                    arraySuffix = "Array";
                    arrayParam = "";
                }
                else
                {
                    arraySuffix = "Array";
                    arrayParam = $"(uint)({arrayLength}), ";
                }

                csharpResult = $"stream.WriteString{arraySuffix}({arrayParam}";
            }

            if (info.Type == "MultiLanguageString")
            {
                // TODO array
                csharpResult = "stream.WriteStringSizeLangPrefixed(";
            }
            else
            {
                csharpResult = $"stream.WriteStringZeroTerminated{arraySuffix}({arrayParam}";
            }
        }
        else if (info.IsNumber)
        {
            string arraySuffix = "";
            string arrayParam = "";

            if (info.IsArray)
            {
                string[] correct = ["[ c ]", "[i]", "[j][k]", "[j]", "[grid_pos_view_id[i]]", "[i][j]", "[c]", "[f]"];
                if (correct.Contains(info.ArrayLengthVariable))
                {
                    // nothing
                }
                else
                {
                    string arrayLength = info.ArrayLengthVariable.TrimStart('[').TrimEnd(']');

                    if (int.TryParse(arrayLength, out int arrayLen))
                    {
                        arraySuffix = "Array";
                        arrayParam = $"{arrayLength}, ";
                    }
                    else if (string.IsNullOrEmpty(arrayLength))
                    {
                        arraySuffix = "ArrayTillEnd";
                        arrayParam = "";
                    }
                    else
                    {
                        arraySuffix = "Array";
                        arrayParam = $"(uint)({arrayLength}), ";
                    }
                }
            }

            if (!info.IsFloatingPoint)
            {
                if (info.ElementSizeInBits == 1)
                {
                    csharpResult = "stream.WriteBit(";
                }
                else if (info.ElementSizeInBits == -1)
                {
                    string elSizeVar = info.ElementSizeVariable
                        .Replace("8 ceil(size / 8) – size", "(Math.Ceiling(size / 8d) - size) * 8")
                        .Replace("f(pattern_size_code)", "pattern_size_code")
                        .Replace("f(count_size_code)", "count_size_code")
                        .Replace("f(index_size_code)", "index_size_code")
                        ;
                    csharpResult = $"stream.WriteBits{arraySuffix}((uint)({elSizeVar} ), ";
                }
                else if (info.ElementSizeInBits > 1 && info.ElementSizeInBits % 8 > 0)
                {
                    csharpResult = $"stream.WriteBits({info.ElementSizeInBits}, ";
                }
                else if (info.ElementSizeInBits % 8 == 0)
                {
                    if (info.IsSigned)
                        csharpResult = $"stream.WriteInt{info.ElementSizeInBits}{arraySuffix}({arrayParam}";
                    else
                        csharpResult = $"stream.WriteUInt{info.ElementSizeInBits}{arraySuffix}({arrayParam}";
                }
            }
            else
            {
                if (info.ElementSizeInBits == 32)
                {
                    csharpResult = "stream.WriteDouble32(";
                }
                else
                {
                    throw new NotSupportedException($"{info.Type} is unknown");
                }
            }
        }
        
        if (info.Type == "string" && !info.IsSigned && info.ArrayDimensions == 0 && info.ElementSizeInBits == 15 && info.ArrayLengthVariable == "")
        {
            csharpResult = $"stream.WriteIso639(";
        }

        // TODO: fix this workaround
        csharpResult = csharpResult.Replace("constant_IV_size", "IsoStream.GetInt(constant_IV_size)");

        return csharpResult;
    }

    private static string GetCalculateSizeMethod(PseudoField field)
    {
        var info = GetTypeInfo(field);

        string csharpResult = "";
        if(info.IsClass || info.IsEntry)
            csharpResult = "IsoStream.CalculateClassSize(value)";
        else if(info.IsBox)
            csharpResult = "IsoStream.CalculateBoxSize(value)";
        else if (info.IsDescriptor)
            csharpResult = "IsoStream.CalculateDescriptorSize(value)";
        else if (info.IsByteAlignment)
            csharpResult = "IsoStream.CalculateByteAlignmentSize(boxSize, value)";
        else if(info.IsString)
        {
            if (info.Type == "MultiLanguageString")
            {
                csharpResult = "IsoStream.CalculateStringSizeLangPrefixed(value)";
            }
            else
            {
                csharpResult = "IsoStream.CalculateStringSize(value)";
            }
        }
        else if(info.IsNumber)
        {
            if (info.ElementSizeInBits > 0)
            {
                csharpResult = $"{info.ElementSizeInBits}";
            }
            else
            {
                // workaround - TODO
                string elSizeVar = info.ElementSizeVariable
                    .Replace("8 ceil(size / 8) – size", "(Math.Ceiling(size / 8d) - size) * 8")
                    .Replace("f(pattern_size_code)", "pattern_size_code")
                    .Replace("f(count_size_code)", "count_size_code")
                    .Replace("f(index_size_code)", "index_size_code")
                    ;
                csharpResult = $"(ulong)({elSizeVar} )";
            }
            
            if(info.IsArray)
            {
                string[] correct = ["[ c ]", "[i]", "[j][k]", "[j]", "[grid_pos_view_id[i]]", "[i][j]", "[c]", "[f]"];
                if (correct.Contains(info.ArrayLengthVariable))
                {
                    // ignore - in these cases, we're counting the size inside a loop
                }
                else
                {
                    string arrayLength = info.ArrayLengthVariable.TrimStart('[').TrimEnd(']');

                    if (int.TryParse(arrayLength, out int arrayLen))
                    {
                        csharpResult = $"{arrayLength} * {csharpResult}";
                    }
                    else if (string.IsNullOrWhiteSpace(arrayLength))
                    {
                        csharpResult = $"((ulong)value.Length * {csharpResult})";
                    }
                    else
                    {
                        csharpResult = $"((ulong)({arrayLength}) * {csharpResult})";
                    }
                }
            }
        }
        else if(info.Type == "string" && info.IsString == false)
        {
            csharpResult = $"{info.ElementSizeInBits}";
        }

        // TODO: fix this workaround
        csharpResult = csharpResult.Replace("constant_IV_size", "IsoStream.GetInt(constant_IV_size)");

        return csharpResult;
    }

    private static string GetType(PseudoField field)
    {
        var fieldType = field.Type;

        bool isNumber, isFloatningPoint, isString, isSigned;
        int arrayDimensions, fieldSize;
        var info = GetTypeInfo(field);

        string csharpType = GetCSharpType(info);
        return csharpType;
    }

    public class FieldTypeInfo
    {
        public bool IsNumber { get; internal set; }
        public bool IsFloatingPoint { get; internal set; }
        public bool IsSigned { get; internal set; }
        public bool IsString { get; internal set; }
        public bool IsBox { get; internal set; }
        public bool IsDescriptor { get; internal set; }
        public bool IsEntry { get; internal set; }
        public bool IsClass { get; internal set; }
        public bool IsByteAlignment { get; internal set; }

        public bool IsArray { get { return ArrayDimensions > 0; } }

        public string Type { get; internal set; }
        public int ArrayDimensions { get; internal set; }
        public int ElementSizeInBits { get; internal set; }
        public string ElementSizeVariable { get; internal set; }
        public string ArrayLengthVariable { get; internal set; }
        
        public PseudoClass[] Ancestors { get; internal set; }
    }

    private static FieldTypeInfo GetTypeInfo(PseudoField field)
    {
        PseudoType fieldType = field.Type;
        FieldTypeInfo info = new FieldTypeInfo();

        info.Type = fieldType.Type;
        info.ArrayLengthVariable = field.FieldArray;

        switch (fieldType.Type)
        {
            case "int":
                info.ElementSizeInBits = 32; // assume standard int size
                info.IsNumber = true;
                info.IsSigned = true;
                break;

            case "uint":
            case "uimsbf":
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "bit":
                info.ElementSizeInBits = 1;
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "char":
            case "byte":
            case "bslbf":
            case "vluimsbf8":
                info.ElementSizeInBits = 8;
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "byte_alignment":
                info.IsByteAlignment = true;
                info.ElementSizeInBits = 8;
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "double":
            case "fixedpoint1616":
                info.ElementSizeInBits = 32;
                info.IsFloatingPoint = true;
                info.IsNumber = true;
                info.IsSigned = true;
                break;

            case "MultiLanguageString":
            case "base64string":
            case "boxstring":
            case "utf8string":
            case "utfstring":
            case "utf8list":
            case "string":
                info.IsString = true;
                break;

            default:
                //Debug.WriteLine($"GetType - number: {fieldType.Type}");
                break;
        }

        if (info.IsNumber)
        {
            switch (fieldType.Sign)
            {
                case "unsigned":
                    info.IsSigned = false;
                    break;

                case "signed":
                    info.IsSigned = true;
                    break;
            }
        }

        if (!string.IsNullOrEmpty(field.FieldArray))
        {
            int level = 0;
            for (int i = 0; i < field.FieldArray.Length; i++)
            {
                if (field.FieldArray[i] == '[')
                {
                    if (level == 0)
                        info.ArrayDimensions++;

                    level++;
                }
                else if (field.FieldArray[i] == ']')
                {
                    level--;
                }
            }
        }

        // size
        if (info.IsNumber && !string.IsNullOrEmpty(fieldType.Param))
        {
            string innerParam = fieldType.Param.Substring(1, fieldType.Param.Length - 2);
            if (innerParam.Length > 0)
            {
                int fieldSize;
                if (!int.TryParse(innerParam, out fieldSize))
                {
                    // not a number
                    //Debug.WriteLine($"GetType - number: {fieldType.Type} {innerParam}");
                    fieldSize = -1; // we cannot determine the size, we'll use byte[]
                    info.ElementSizeVariable = innerParam;
                }
                info.ElementSizeInBits = fieldSize;
            }
        }

        if (info.IsNumber && info.ElementSizeInBits == 0)
            throw new NotSupportedException($"{fieldType.Type} is unknown");

        if (!info.IsNumber && !info.IsString && !info.IsFloatingPoint && !info.IsByteAlignment)
        {
            info.Ancestors = GetClassAncestors(fieldType.Type);

            if (info.Ancestors.Any())
            {
                if (info.Ancestors.LastOrDefault().Extended.BoxName != null && info.Ancestors.LastOrDefault().Extended.BoxName.EndsWith("Box"))
                    info.IsBox = true;
                else if (info.Ancestors.LastOrDefault().Extended.BoxName != null && info.Ancestors.LastOrDefault().Extended.BoxName.EndsWith("Descriptor") || (info.Ancestors.FirstOrDefault().Extended != null && !string.IsNullOrEmpty(info.Ancestors.FirstOrDefault().Extended.DescriptorTag)))
                    info.IsDescriptor = true;
                else if (info.Ancestors.LastOrDefault().BoxName == "SampleGroupDescriptionEntry")
                    info.IsEntry = true;
                else
                    info.IsClass = true;
            }
            else
            {
                if (info.Type.EndsWith("Box") ||
                    info.Type == "DRCCoefficientsBasic" ||
                    info.Type == "DRCInstructionsBasic" || 
                    info.Type == "DRCCoefficientsUniDRC" || 
                    info.Type == "DRCInstructionsUniDRC" || 
                    info.Type == "UniDrcConfigExtension" ||
                    info.Type == "SampleConstructor" ||
                    info.Type == "InlineConstructor" ||
                    info.Type == "SampleConstructorFromTrackGroup" ||
                    info.Type == "NALUStartInlineConstructor" 
                    )
                    info.IsBox = true;
                else if (info.Type.EndsWith("Descriptor"))
                    info.IsDescriptor = true;
                else
                    info.IsClass = true;
            }
        }

        // workaround: Iso639
        if (info.Type == "int" && !info.IsSigned && info.ArrayDimensions == 1 && info.ElementSizeInBits == 5 && info.ArrayLengthVariable == "[3]")
        {
            info.IsString = false;
            info.IsNumber = false;
            info.IsClass = false;
            info.ArrayDimensions = 0;
            info.ArrayLengthVariable = "";
            info.Type = "string";
            info.ElementSizeInBits = 15;
        }

        return info;
    }

    private static string GetCSharpType(FieldTypeInfo info)
    {
        string t = "";
        int arrayDimensions = info.ArrayDimensions;
        if (info.IsNumber)
        {
            if (!info.IsFloatingPoint)
            {
                if (info.ElementSizeInBits == 1)
                {
                    t = "bool";
                }
                else if (info.ElementSizeInBits > 1 && info.ElementSizeInBits <= 8)
                {
                    if (info.IsSigned)
                    {
                        t = "sbyte";
                    }
                    else
                    {
                        t = "byte";
                    }
                }
                else if (info.ElementSizeInBits > 8 && info.ElementSizeInBits <= 16)
                {
                    if (info.IsSigned)
                    {
                        t = "short";
                    }
                    else
                    {
                        t = "ushort";
                    }
                }
                else if (info.ElementSizeInBits > 16 && info.ElementSizeInBits <= 32)
                {
                    if (info.IsSigned)
                    {
                        t = "int";
                    }
                    else
                    {
                        t = "uint";
                    }
                }
                else if (info.ElementSizeInBits > 32 && info.ElementSizeInBits <= 64)
                {
                    if (info.IsSigned)
                    {
                        t = "long";
                    }
                    else
                    {
                        t = "ulong";
                    }
                }
                else if (info.ElementSizeInBits == -1)
                {
                    t = "byte";
                    arrayDimensions++;
                }
            }
            else
            {
                if(info.ElementSizeInBits == 32)
                {
                    t = "double";
                }
                else
                {
                    throw new NotSupportedException($"{info.Type} is unknown");
                }
            }
        }
        else if(info.IsString)
        {
            if (info.Type == "MultiLanguageString")
                t = "MultiLanguageString";
            else
                t = "BinaryUTF8String";
        }
        else
        {
            t = info.Type;
        }

        for (int i = 0; i < arrayDimensions; i++)
        {
            t += "[]";
        }

        return t;
    }

    static partial void HelloFrom(string name);
}


