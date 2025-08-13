using Pidgin;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BoxGenerator
{
    public class ParserDocument
    {
        public Dictionary<string, PseudoClass> Classes { get; }
        public List<string> Containers { get; }

        public SortedDictionary<string, List<PseudoClass>> Boxes { get; }
        public SortedDictionary<string, List<PseudoClass>> Entries { get; }
        public SortedDictionary<string, List<PseudoClass>> Descriptors { get; }

        public ParserDocument(Dictionary<string, PseudoClass> parsedClasses, List<string> containers)
        {
            Classes = parsedClasses;

            // post-processing
            Containers = containers.Distinct().ToList();

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
                        (item.Value.Extended != null && Containers.Contains(item.Value.Extended.BoxType)) || Containers.Contains(item.Value.BoxName) ||
                        item.Value.FlattenedFields.FirstOrDefault(x =>
                            x.Type.Type == "Box" || GetClassAncestors(x.Type.Type).LastOrDefault(c => c != null && c.BoxName.EndsWith("Box")) != null) != null
                    ) ||
                    item.Value.BoxName == "DefaultHevcExtractorConstructorBox"; // DefaultHevcExtractorConstructorBox is a container, but the *constructor boxes have currently unknown syntax
            }

            // collect all boxes, entries and descriptors
            Boxes = new SortedDictionary<string, List<PseudoClass>>();
            Entries = new SortedDictionary<string, List<PseudoClass>>();
            foreach (var item in parsedClasses)
            {
                if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.BoxType))
                {
                    if (item.Value.ParsedBoxType == ParsedBoxType.Box)
                    {
                        if (!Boxes.ContainsKey(item.Value.Extended.BoxType))
                            Boxes.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                        else
                            Boxes[item.Value.Extended.BoxType].Add(item.Value);
                    }
                    else if (item.Value.ParsedBoxType == ParsedBoxType.Entry)
                    {
                        if (!Entries.ContainsKey(item.Value.Extended.BoxType))
                            Entries.Add(item.Value.Extended.BoxType, new List<PseudoClass>() { item.Value });
                        else
                            Entries[item.Value.Extended.BoxType].Add(item.Value);
                    }
                }
            }

            Descriptors = new SortedDictionary<string, List<PseudoClass>>();
            foreach (var item in parsedClasses)
            {
                if (item.Value.Extended != null && !string.IsNullOrWhiteSpace(item.Value.Extended.DescriptorTag) && string.IsNullOrEmpty(item.Value.Abstract))
                {
                    if (!Descriptors.ContainsKey(item.Value.Extended.DescriptorTag))
                        Descriptors.Add(item.Value.Extended.DescriptorTag, new List<PseudoClass>() { item.Value });
                    else
                        Descriptors[item.Value.Extended.DescriptorTag].Add(item.Value);
                }
            }

            // add additional boxes
            string[] audioSampleEntryTypes = new string[]
            {
            "samr","sawb","mp4a","drms","alac","owma","ac-3","ec-3","mlpa","dtsl","dtsh","dtse","Opus","enca","resa","sevc","sqcp","ssmv","lpcm","dtsc","sowt",
            // quicktime https://developer.apple.com/documentation/quicktime-file-format/sound_sample_descriptions
            "\\0\\0\\0\\0","NONE","raw ","twos","sowt","MAC3","MAC6","ima4","fl32","fl64","in24","in32","ulaw","alaw","\\x6D\\x73\\x00\\x02","\\x6D\\x73\\x00\\x11",
            "dvca","QDMC","QDM2","Qclp","\\x6D\\x73\\x00\\x55",".mp3"
            };
            string[] visualSampleEntryTypes = new string[]
            {
            "mp4v","s263","drmi","encv","resv","icpv","hvc1","hvc2","hvc3","lhv1","lhe1","hev1","hev2","hev3","avcp","mvc1","mvc2","mvc3","mvc4","mvd1","mvd2",
            "mvd3","mvd4","a3d1","a3d2","a3d3","a3d4","svc1","svc2","hvt1","lht1","hvt3","hvt2","vvc1","vvi1","vvs1","vvcN","evc1","evs1","evs2","av01","avc1",
            "avc2","avc3","avc4","vp08","vp09","vp10","apcn","dvhe","dvav","mjpg","uncv","j2ki",
            // quicktime https://developer.apple.com/documentation/quicktime-file-format/video_sample_description
            "cvid","jpeg","smc ","rle ","rpza","kpcd","png ","mjpa","mjpb","SVQ1","SVQ3","dvc ","dvcp","gif ","h263","tiff","raw ","2vuY","yuv2","v308","v408",
            "v216","v410","v210"
            };

            foreach (var type in audioSampleEntryTypes)
            {
                if (!Boxes.ContainsKey(type))
                    Boxes.Add(type, new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "AudioSampleEntry").Value });
                else
                    Boxes[type] = new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "AudioSampleEntry").Value };
            }

            foreach (var type in visualSampleEntryTypes)
            {
                if (!Boxes.ContainsKey(type))
                    Boxes.Add(type, new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "VisualSampleEntry").Value });
                else
                    Boxes[type] = new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "VisualSampleEntry").Value };
            }

            if (!Boxes.ContainsKey("mp4s"))
                Boxes.Add("mp4s", new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "MpegSampleEntry").Value });
            else
                Boxes["mp4s"] = new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "MpegSampleEntry").Value };

            if (!Boxes.ContainsKey("dimg"))
                Boxes.Add("dimg", new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "SingleItemTypeReferenceBox").Value });

            if (!Boxes.ContainsKey("cdsc"))
                Boxes.Add("cdsc", new List<PseudoClass>() { parsedClasses.First(x => x.Value.BoxName == "TrackReferenceTypeBox").Value });
        }

        public bool IsWorkaround(string type)
        {
            return Parser.Workarounds.Contains(type);
        }

        public string GetFieldName(PseudoField field)
        {
            string name = field.Name;
            // in C#, we cannot use "namespace" as variable name
            if (name == "namespace")
                return "ns";

            if (string.IsNullOrEmpty(name))
            {
                if (field.Type.Type.StartsWith("byte_"))
                {
                    // byte_alignment would otherwise produce a name "byte"
                    name = "byte_alignment";
                }
                else
                {
                    name = field.Type.Type;
                }
            }

            return name;
        }

        private PseudoClass[] GetClassAncestors(string item)
        {
            // find all ancestors of the box/entry/class/descriptor - this allows us to determine the type of the class
            List<PseudoClass> extended = new List<PseudoClass>();

            // right now this algorithm is terribly inefficient, but it works
            PseudoClass it = Classes.Values.SingleOrDefault(x => x.BoxName == item);
            if (it != null)
                extended.Add(it);

            while (it != null)
            {
                it = Classes.Values.SingleOrDefault(x => x.BoxName == it.Extended.BoxName);

                if (it == null)
                    break;

                extended.Add(it);
            }

            return extended.ToArray();
        }

        public int GetLoopNestingLevel(PseudoCode code)
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

        public void AddRequiresAllocation(PseudoField field)
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

        public int GetNestedInLoopSuffix(PseudoCode code, string currentSuffix, out string result)
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

            foreach (var suffix in ret.ToArray())
            {
                if (!string.IsNullOrEmpty(currentSuffix) && currentSuffix.Replace(" ", "").Contains(suffix))
                    ret.Remove(suffix);
            }

            result = string.Concat(ret);
            return ret.Count;
        }

        public string FixNestedInLoopVariables(PseudoCode code, string condition, string prefix = "", string suffix = "")
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

        public string GetFieldTypeDef(PseudoCode field)
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

        public FieldTypeInfo GetTypeInfo(PseudoField field)
        {
            PseudoType fieldType = field.Type;
            FieldTypeInfo info = new FieldTypeInfo();

            info.Type = fieldType.Type;
            info.ArrayLengthVariable = field.FieldArray;

            switch (fieldType.Type)
            {
                case "int":
                    info.ElementSizeInBits = 32; // assume standard int size
                    info.FieldType = ParsedBoxType.Number;
                    info.IsSigned = true;
                    break;

                case "uint":
                case "uimsbf":
                    info.FieldType = ParsedBoxType.Number;
                    info.IsSigned = false;
                    break;

                case "bit":
                    info.ElementSizeInBits = 1;
                    info.FieldType = ParsedBoxType.Number;
                    info.IsSigned = false;
                    break;

                case "char":
                case "byte":
                case "bslbf":
                case "vluimsbf8":
                    info.ElementSizeInBits = 8;
                    info.FieldType = ParsedBoxType.Number;
                    info.IsSigned = false;
                    break;

                case "byte_alignment":
                    info.FieldType = ParsedBoxType.ByteAlignment;
                    info.ElementSizeInBits = 8;
                    info.IsSigned = false;
                    break;

                case "double":
                case "fixedpoint1616":
                    info.ElementSizeInBits = 32;
                    info.IsFloatingPoint = true;
                    info.FieldType = ParsedBoxType.Number;
                    info.IsSigned = true;
                    break;

                case "MultiLanguageString":
                case "base64string":
                case "boxstring":
                case "utf8string":
                case "utfstring":
                case "utf8list":
                case "string":
                    info.FieldType = ParsedBoxType.String;
                    break;

                default:
                    //Debug.WriteLine($"GetType - number: {fieldType.Type}");
                    break;
            }

            if (info.FieldType == ParsedBoxType.Number)
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
            if (info.FieldType == ParsedBoxType.Number && !string.IsNullOrEmpty(fieldType.Param))
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

            if (info.FieldType == ParsedBoxType.Number && info.ElementSizeInBits == 0)
                throw new NotSupportedException($"{fieldType.Type} is unknown");

            if (info.FieldType != ParsedBoxType.Number && info.FieldType != ParsedBoxType.String && !info.IsFloatingPoint && info.FieldType != ParsedBoxType.ByteAlignment)
            {
                info.Ancestors = GetClassAncestors(fieldType.Type);

                if (info.Ancestors.Any())
                {
                    if (info.Ancestors.LastOrDefault().Extended.BoxName != null && info.Ancestors.LastOrDefault().Extended.BoxName.EndsWith("Box"))
                        info.FieldType = ParsedBoxType.Box;
                    else if (info.Ancestors.LastOrDefault().Extended.BoxName != null && info.Ancestors.LastOrDefault().Extended.BoxName.EndsWith("Descriptor") ||
                        (info.Ancestors.FirstOrDefault().Extended != null && !string.IsNullOrEmpty(info.Ancestors.FirstOrDefault().Extended.DescriptorTag)))
                        info.FieldType = ParsedBoxType.Descriptor;
                    else if (info.Ancestors.LastOrDefault().BoxName == "SampleGroupDescriptionEntry")
                        info.FieldType = ParsedBoxType.Entry;
                    else
                        info.FieldType = ParsedBoxType.Class;
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
                        info.FieldType = ParsedBoxType.Box;
                    else if (info.Type.EndsWith("Descriptor"))
                        info.FieldType = ParsedBoxType.Descriptor;
                    else
                        info.FieldType = ParsedBoxType.Class;
                }
            }

            // workaround: Iso639
            if (info.Type == "int" && !info.IsSigned && info.ArrayDimensions == 1 && info.ElementSizeInBits == 5 && info.ArrayLengthVariable == "[3]")
            {
                info.FieldType = ParsedBoxType.Iso639;
                info.ArrayDimensions = 0;
                info.ArrayLengthVariable = "";
                info.Type = "string";
                info.ElementSizeInBits = 15;
            }

            return info;
        }

        private List<PseudoField> FlattenFields(IEnumerable<PseudoCode> fields, PseudoBlock parent = null)
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

                    // TODO: incorrectly parsed type
                    if (!string.IsNullOrEmpty(value) && value.StartsWith("["))
                    {
                        field.FieldArray = value;
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

        private void AddAndResolveDuplicates(Dictionary<string, PseudoField> ret, PseudoField field)
        {
            string name = GetFieldName(field);
            if (!ret.TryAdd(name, field))
            {
                if (name.StartsWith("reserved") ||
                    name == "pre_defined" ||
                    field.Type.Type == "SingleItemTypeReferenceBoxLarge" ||
                    (field.Type.ToString() == "signed int(32)" && ret[name].Type.ToString() == "unsigned int(32)") ||
                    field.Name == "min_initial_alt_startup_offset" ||
                    field.Name == "target_rate_share") // special case: requires nesting
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
                else if (field.Type.ToString() == ret[name].Type.ToString() && GetLoopNestingLevel(field) == GetLoopNestingLevel(ret[name]))
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

                    if (field.Type.ToString() == "aligned bit(1)" && ret[name].Type.ToString() == "bit")
                    {
                        return;
                    }

                    throw new Exception($"Duplicated fields: {name} of types: {field.Type}, {ret[name].Type}");
                }
            }
        }
    }

    public enum MethodType
    {
        Read,
        Write,
        Size
    }

    public class FieldTypeInfo
    {
        public bool IsFloatingPoint { get; internal set; }
        public bool IsSigned { get; internal set; }
        public ParsedBoxType FieldType { get; internal set; }

        public bool IsArray { get { return ArrayDimensions > 0; } }

        public string Type { get; internal set; }
        public int ArrayDimensions { get; internal set; }
        public int ElementSizeInBits { get; internal set; }
        public string ElementSizeVariable { get; internal set; }
        public string ArrayLengthVariable { get; internal set; }

        public PseudoClass[] Ancestors { get; internal set; }
    }
}
