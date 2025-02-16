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

            if (ancestors.Any())
            {
                if (ancestors.LastOrDefault().EndsWith("Box"))
                    item.Value.ParsedBoxType = ParsedBoxType.Box;
                else if (ancestors.LastOrDefault().EndsWith("Descriptor"))
                    item.Value.ParsedBoxType = ParsedBoxType.Descriptor;
                else if (ancestors.LastOrDefault().EndsWith("Entry"))
                    item.Value.ParsedBoxType = ParsedBoxType.Entry;
                else
                    item.Value.ParsedBoxType = ParsedBoxType.Class;
            }

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
                item.Value.CtorContent = "\t\tthis.type = IsoStream.FromFourCC(boxtype);\r\n\t\tthis.usertype = extended_type;\r\n";
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
                        x.Type.Type == "Box" || GetClassAncestors(x.Type.Type).LastOrDefault(c => c.EndsWith("Box")) != null) != null
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
                else
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

    private static string[] GetClassAncestors(string item)
    {
        // find all ancestors of the box/entry/class/descriptor - this allows us to determine the type of the class
        List<string> extended = new List<string>();

        // right now this algorithm is terribly inefficient, but it works
        PseudoClass it = parsedClasses.Values.SingleOrDefault(x => x.BoxName == item);
        while (it != null && !string.IsNullOrEmpty(it.Extended.BoxName))
        {
            extended.Add(it.Extended.BoxName);
            it = parsedClasses.Values.SingleOrDefault(x => x.BoxName == it.Extended.BoxName);
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
                string type1 = GetCalculateSizeMethod(field);
                string type2 = GetCalculateSizeMethod(ret[name]);
                int type1Size;
                if (int.TryParse(type1, out type1Size))
                {
                    int type2Size;
                    if (int.TryParse(type2, out type2Size))
                    {
                        if (type1Size > type2Size)
                            ret[name] = field;
                        if (type1Size != type2Size)
                            return;
                    }
                }

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
               

        bool hasDescriptors = fields.Select(x => GetReadMethod(GetFieldType(x)).Contains("ReadDescriptor(")).FirstOrDefault(x => x == true) != false && b.BoxName != "ESDBox" && b.BoxName != "MpegSampleEntry" && b.BoxName != "MPEG4ExtensionDescriptorsBox" && b.BoxName != "AppleInitialObjectDescriptorBox" && b.BoxName != "IPMPControlBox" && b.BoxName != "IPMPInfoBox";

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
        string m = methodType == MethodType.Read ? GetReadMethod(tt) : (methodType == MethodType.Write ? GetWriteMethod(tt) : GetCalculateSizeMethod(field as PseudoField));
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

    private static string GetReadMethod(string type)
    {
        Dictionary<string, string> map = new Dictionary<string, string>
        {
            { "unsigned int(64)[ entry_count ]",        "stream.ReadUInt64Array(entry_count, " },
            { "unsigned int(64)",                       "stream.ReadUInt64(" },
            { "unsigned int(48)",                       "stream.ReadUInt48(" },
            { "unsigned int(32)[ entry_count ]",        "stream.ReadUInt32Array(entry_count, " },
            { "template int(32)[9]",                    "stream.ReadInt32Array(9, " },
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
            { "unsigned int(subgroupIdLen)[i]",         "stream.ReadBits((uint)subgroupIdLen, " },
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
            { "bslbf(header_size * 8)",               "stream.ReadBslbf(header_size * 8, " },
            { "bslbf(trailer_size * 8)",              "stream.ReadBslbf(trailer_size * 8, " },
            { "bslbf(aux_size * 8)",                  "stream.ReadBslbf(aux_size * 8, " },
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
            { "unsigned int(16)[3]",                    "stream.ReadUInt16Array(3, " },
            { "MultiLanguageString[]",                  "stream.ReadStringSizeLangPrefixed(boxSize, readSize, " },
            { "AdobeChapterRecord[]",                   "stream.ReadClass(boxSize, readSize, this, " },
            { "ThreeGPPKeyword[]",                      "stream.ReadClass(boxSize, readSize, this, " },
            { "IodsSample[]",                           "stream.ReadClass(boxSize, readSize, this, " },
            { "XtraTag[]",                              "stream.ReadClass(boxSize, readSize, this, " },
            { "XtraValue[count]",                       "stream.ReadClass(boxSize, readSize, this, " },
            { "TrunEntry(version, flags)[ sample_count ]",       "stream.ReadClass(boxSize, readSize, this, sample_count, () => new TrunEntry(version, flags), " },
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
            { "ViewIdentifierBox",                      "stream.ReadBox(boxSize, readSize, this, " },
       };

        if(map.ContainsKey(type))
            return map[type];
        else if(map.ContainsKey(type.Replace("()","")))
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
            { "template int(32)[9]",                    "stream.WriteInt32Array(9, " },
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
            { "unsigned int(subgroupIdLen)[i]",         "stream.WriteBits((uint)subgroupIdLen, " },
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
            { "bslbf(header_size * 8)",               "stream.WriteBslbf(header_size * 8, " },
            { "bslbf(trailer_size * 8)",              "stream.WriteBslbf(trailer_size * 8, " },
            { "bslbf(aux_size * 8)",                  "stream.WriteBslbf(aux_size * 8, " },
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
            { "unsigned int(16)[3]",                    "stream.WriteUInt16Array(3, " },
            { "MultiLanguageString[]",                  "stream.WriteStringSizeLangPrefixed(" },
            { "AdobeChapterRecord[]",                   "stream.WriteClass(" },
            { "ThreeGPPKeyword[]",                      "stream.WriteClass(" },
            { "IodsSample[]",                           "stream.WriteClass(" },
            { "XtraTag[]",                              "stream.WriteClass(" },
            { "XtraValue[count]",                       "stream.WriteClass(" },
            { "TrunEntry(version, flags)[ sample_count ]",       "stream.WriteClass(" },
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
            { "ViewIdentifierBox",                      "stream.WriteBox(" },
        };
        if (map.ContainsKey(type))
            return map[type];
        else if (map.ContainsKey(type.Replace("()", "")))
            return map[type.Replace("()", "")];
        else
            throw new NotSupportedException(type);
    }

    private static string GetCalculateSizeMethod(PseudoField field)
    {
        var info = GetTypeInfo(field);
        

        string type = GetFieldType(field);
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
            { "unsigned int(subgroupIdLen)[i]",         "(ulong)subgroupIdLen" },
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
            { "bslbf(header_size * 8)",               "header_size * 8" },
            { "bslbf(trailer_size * 8)",              "trailer_size * 8" },
            { "bslbf(aux_size * 8)",                  "aux_size * 8" },
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
            { "unsigned int(16)[3]",                    "3 * 16" },
            { "MultiLanguageString[]",                  "IsoStream.CalculateStringSizeLangPrefixed(value)" },
            { "AdobeChapterRecord[]",                   "IsoStream.CalculateClassSize(value)" },
            { "ThreeGPPKeyword[]",                      "IsoStream.CalculateClassSize(value)" },
            { "IodsSample[]",                           "IsoStream.CalculateClassSize(value)" },
            { "XtraTag[]",                              "IsoStream.CalculateClassSize(value)" },
            { "XtraValue[count]",                       "IsoStream.CalculateClassSize(value)" },
            { "TrunEntry(version, flags)[ sample_count ]",       "IsoStream.CalculateClassSize(value)" },
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
            { "ViewIdentifierBox",                      "IsoStream.CalculateBoxSize(value)" },
       };
        if (map.ContainsKey(type))
            return map[type];
        else if (map.ContainsKey(type.Replace("()", "")))
            return map[type.Replace("()", "")];
        else
            throw new NotSupportedException(type);
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
        public bool IsString { get; internal set; }
        public bool IsSigned { get; internal set; }
        public int ArrayDimensions { get; internal set; }
        public int FieldSize { get; internal set; }
        public string FieldSizeParam { get; internal set; }
        public string TypeName { get; internal set; }
        public string FieldArray { get; internal set; }
        public string[] Ancestors { get; internal set; }
    }

    private static FieldTypeInfo GetTypeInfo(PseudoField field)
    {
        PseudoType fieldType = field.Type;
        FieldTypeInfo info = new FieldTypeInfo();

        info.TypeName = fieldType.Type;
        info.FieldArray = field.FieldArray;

        switch (fieldType.Type)
        {
            case "int":
                info.FieldSize = 32; // assume standard int size
                info.IsNumber = true;
                info.IsSigned = true;
                break;

            case "uint":
            case "uimsbf":
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "bit":
                info.FieldSize = 1;
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "char":
            case "byte":
            case "bslbf":
            case "vluimsbf8":
            case "byte_alignment":
                info.FieldSize = 8;
                info.IsNumber = true;
                info.IsSigned = false;
                break;

            case "double":
            case "fixedpoint1616":
                info.FieldSize = 32;
                info.IsFloatingPoint = true;
                info.IsNumber = true;
                info.IsSigned = true;
                break;

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
                    info.FieldSizeParam = innerParam;
                }
                info.FieldSize = fieldSize;
            }
        }

        // workaround: Iso639
        if (info.TypeName == "int" && !info.IsSigned && info.ArrayDimensions == 1 && info.FieldSize == 5 && info.FieldArray == "[3]")
        {
            info.IsString = false;
            info.IsNumber = false;
            info.ArrayDimensions = 0;
            info.FieldArray = "";
            info.TypeName = "string";
        }

        if (info.IsNumber && info.FieldSize == 0)
            throw new NotSupportedException($"{fieldType.Type} is unknown");

        if(!info.IsNumber && !info.IsString && !info.IsFloatingPoint)
        {
            info.Ancestors = GetClassAncestors(fieldType.Type);
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
                if (info.FieldSize == 1)
                {
                    t = "bool";
                }
                else if (info.FieldSize > 1 && info.FieldSize <= 8)
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
                else if (info.FieldSize > 8 && info.FieldSize <= 16)
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
                else if (info.FieldSize > 16 && info.FieldSize <= 32)
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
                else if (info.FieldSize > 32 && info.FieldSize <= 64)
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
                else if (info.FieldSize == -1)
                {
                    t = "byte";
                    arrayDimensions++;
                }
            }
            else
            {
                if(info.FieldSize == 32)
                {
                    t = "double";
                }
                else
                {
                    throw new NotSupportedException($"{info.TypeName} is unknown");
                }
            }
        }
        else if(info.IsString)
        {
            t = "BinaryUTF8String";
        }
        else
        {
            t = info.TypeName;
        }

        for (int i = 0; i < arrayDimensions; i++)
        {
            t += "[]";
        }

        return t;
    }

    static partial void HelloFrom(string name);
}


