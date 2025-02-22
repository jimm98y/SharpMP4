using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace BoxGenerator
{
    public class CSharpGenerator : ICodeGenerator
    {
        private ParserDocument parserDocument;

        public CSharpGenerator(ParserDocument parserDocument)
        {
            this.parserDocument = parserDocument;
        }

        private string GetCtorParams(string classType, IList<(string Name, string Value)> parameters)
        {
            string par;

            if (!string.IsNullOrEmpty(classType) && classType != "()")
            {
                par = classType.Substring(1, classType.Length - 2);
            }
            else if (parameters != null)
            {
                par = string.Join(", ", parameters.Select(x => x.Name + (string.IsNullOrEmpty(x.Value) ? "" : " = " + x.Value)));
            }
            else
            {
                return "";
            }

            Dictionary<string, string> map = new Dictionary<string, string>() {
                { "unsigned int(32) format",          "uint format" },
                { "bit(24) flags",                    "uint flags = 0" },
                { "fmt",                              "uint fmt = 0" },
                { "codingname",                       "uint codingname = 0" },
                { "handler_type",                     "uint handler_type = 0" },
                { "referenceType",                    "uint referenceType" },
                { "unsigned int(32) reference_type",  "uint reference_type" },
                { "grouping_type, version, flags",    "uint grouping_type, byte version, uint flags" },
                { "boxtype = 'msrc'",                 "uint boxtype = 1836282467" }, // msrc
                { "name",                             "uint name" },
                { "uuid",                             "byte[] uuid" },
                { "property_type",                    "uint property_type" },
                { "channelConfiguration",             "int channelConfiguration" },
                { "num_sublayers",                    "byte num_sublayers" },
                { "code",                             "uint code" },
                { "property_type, version, flags",    "uint property_type, byte version, uint flags" },
                { "samplingFrequencyIndex, channelConfiguration, audioObjectType", "int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType" },
                { "samplingFrequencyIndex,\r\n  channelConfiguration,\r\n  audioObjectType", "int samplingFrequencyIndex, int channelConfiguration, byte audioObjectType" },
                { "unsigned int(32) extension_type",  "uint extension_type" },
                { "'vvcb', version, flags",           "byte version = 0, uint flags = 0" },
                { "\n\t\tunsigned int(32) boxtype,\n\t\toptional unsigned int(8)[16] extended_type", "uint boxtype = 0, byte[] extended_type = null" },
                { "unsigned int(32) grouping_type",   "uint grouping_type" },
                { "unsigned int(32) boxtype, unsigned int(8) v, bit(24) f", "uint boxtype, byte v = 0, uint f = 0" },
                { "unsigned int(8) OutputChannelCount", "byte OutputChannelCount" },
                { "entry_type, bit(24) flags",        "uint entry_type, uint flags" },
                { "samplingFrequencyIndex",           "int samplingFrequencyIndex" },
                { "version, flags, Per_Sample_IV_Size",  "byte version, uint flags, byte Per_Sample_IV_Size" },
                { "version, flags",                   "byte version = 0, uint flags = 0" },
                { "loudnessType",                     "uint loudnessType" },
                { "local_key_id",                     "uint local_key_id" },
                { "protocol",                         "uint protocol" },
                { "0, 0",                             "" },
                { "size",                             "ulong size = 0" },
                { "type",                             "uint type" },
                { "version = 0, flags",               "uint flags = 0" },
                { "version = 0, 0",                   "" },
                { "version = 0, 1",                   "" },
                { "version, 0",                       "byte version = 0" },
                { "version, flags = 0",               "byte version = 0" },
                { "version",                          "byte version" },
                { "version = 0, flags = 0",           "" },
                { "0, tf_flags",                      "uint tf_flags = 0" },
                { "0, flags",                         "uint flags = 0" },
                { "version, tr_flags",                "byte version = 0, uint tr_flags = 0" },
            };

            return map[par];            
        }

        public string GenerateParser()
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
            foreach (var item in parserDocument.Boxes)
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
                        if (item.Value.Single().BoxName == "AudioSampleEntry" || 
                            item.Value.Single().BoxName == "VisualSampleEntry" ||
                            item.Value.Single().BoxName == "SingleItemTypeReferenceBox" ||
                            item.Value.Single().BoxName == "SingleItemTypeReferenceBoxLarge" ||
                            item.Value.Single().BoxName == "TrackReferenceTypeBox")
                            optParams = $"IsoStream.FromFourCC(\"{item.Key}\")";

                        // for instance "mp4a" box can be also under the "wave" box where it has a different syntax
                        if (item.Value.Single().BoxName == "AudioSampleEntry" || 
                            item.Value.Single().BoxName == "VisualSampleEntry" || 
                            item.Value.Single().BoxName == "MpegSampleEntry")
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
            foreach (var item in parserDocument.Entries)
            {
                if (item.Value.Count == 1)
                {
                    string comment = "";
                    if (item.Value.Single().BoxName.Contains('_'))
                        comment = " // TODO: fix duplicate";
                    string optParams = "";
                    if (item.Value.Single().BoxName == "AudioSampleEntry" ||
                        item.Value.Single().BoxName == "VisualSampleEntry" ||
                        item.Value.Single().BoxName == "SingleItemTypeReferenceBox" ||
                        item.Value.Single().BoxName == "SingleItemTypeReferenceBoxLarge" ||
                        item.Value.Single().BoxName == "TrackReferenceTypeBox")
                    {
                        optParams = $"IsoStream.FromFourCC(\"{item.Key}\")";
                    }

                    factory += $"               case \"{item.Key}\": return new {item.Value.Single().BoxName}({optParams});{comment}\r\n";
                }
                else
                {
                    if (item.Value.First().BoxName == "MovieBox" ||
                        item.Value.First().BoxName == "MovieFragmentBox" ||
                        item.Value.First().BoxName == "SegmentIndexBox" ||
                        item.Value.First().BoxName == "trackhintinformation" ||
                        item.Value.First().BoxName == "ViewPriorityBox" ||
                        item.Value.First().BoxName == "rtpmoviehintinformation")
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
            foreach (var item in parserDocument.Descriptors)
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

            foreach (var b in parserDocument.Classes.Values.ToArray())
            {
                string code = BuildCode(b, parserDocument.Containers);
                resultCode += code + "\r\n\r\n";
            }

            resultCode +=
    @"
}
";
            return resultCode;
        }

        private string BuildCode(PseudoClass b, List<string> containers)
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
                    if (b.BoxName == "SampleGroupDescriptionEntry")
                    {
                        inType = "IHasBoxChildren";
                    }
                    cls += $" : {inType}\r\n{{\r\n\t\tpublic StreamMarker Padding {{ get; set; }}\r\n\t\tprotected IMp4Serializable parent = null;\r\n  public IMp4Serializable GetParent() {{ return parent; }}\r\n        public void SetParent(IMp4Serializable parent) {{ this.parent = parent; }}\r\n";
                }
            }

            if (b.Extended != null && !string.IsNullOrWhiteSpace(b.Extended.BoxName) && !string.IsNullOrWhiteSpace(b.Extended.BoxType))
            {
                // UUID in case the box is a UserBox
                if (b.Extended.BoxType.Length > 4)
                {
                    string[] parts = b.Extended.BoxType.Split(' ');
                    b.Extended.BoxType = parts[0];
                    b.Extended.UserType = parts[1];
                }

                cls += $"\tpublic const string TYPE = \"{b.Extended.BoxType}\";\r\n";
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

            if (b.BoxName == "MetaBox")
            {
                // This is very badly documented, but in case the Apple "ilst" box appears in "meta" inside "trak" or "udta", then the 
                //  serialization is incompatible with ISOBMFF as it uses the "Quicktime atom format".
                //  see: https://www.academia.edu/66625880/Forensic_Analysis_of_Video_Files_Using_Metadata
                //  see: https://web.archive.org/web/20220126080109/https://leo-van-stee.github.io/
                // TODO: maybe instead of lookahead, we just have to check the parents
                cls += "\r\npublic bool IsQuickTime { get { return (GetParent() != null && (((Box)GetParent()).FourCC == IsoStream.FromFourCC(\"udta\") || ((Box)GetParent()).FourCC == IsoStream.FromFourCC(\"trak\"))); } }";
            }
            else if (b.BoxName == "AVCDecoderConfigurationRecord")
            {
                cls += "\r\npublic bool HasExtensions { get; set; } = false;";
            }
            else if (b.BoxName == "FullBox")
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
                else if (b.BoxName == "UserBox")
                {
                    base4ccparams = "uuid";
                }
                else if (!string.IsNullOrEmpty(b.Extended.UserType))
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
                        else if (b.Extended.BoxName != "DecoderSpecificInfo")
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
                cls += "\r\n" + BuildMethod(b, null, field, 2, MethodType.Read);
            }

            if (b.IsContainer)
            {
                cls += "\r\n" + "boxSize += stream.ReadBoxArrayTillEnd(boxSize, readSize, this);";
            }
            else if (hasDescriptors)
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
                cls += "\r\n" + BuildMethod(b, null, field, 2, MethodType.Write);
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
                cls += "\r\n" + BuildMethod(b, null, field, 2, MethodType.Size);
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

        private string FixMissingMethodCode(PseudoClass box, string cls, MethodType methodType)
        {
            if (box.BoxName == "SampleDependencyBox")
            {
                cls += "\r\n\t\tint sample_count = 0; // TODO: taken from the stsz sample_count\r\n";
            }
            else if (box.BoxName == "SampleDependencyTypeBox")
            {
                if (methodType == MethodType.Read)
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

        private string BuildField(PseudoClass b, PseudoCode field)
        {
            var block = field as PseudoBlock;
            if (block != null)
            {
                string ret = "";
                foreach (var f in block.Content)
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

            if (parserDocument.IsWorkaround((field as PseudoField).Type.Type))
                return "";
            else
            {
                // TODO: incorrectly parsed type
                if (!string.IsNullOrEmpty(value) && value.StartsWith('['))
                {
                    value = "";
                }

                string fieldType = GetCSharpType(field as PseudoField);
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
                else if (value == "= boxtype" || value == "= extended_type" || value == "= codingname" ||
                    value == "= f" || value == "= v")
                {
                    comment = "// " + value;
                    value = "";
                }
                else if (fieldType == "bool" && !string.IsNullOrEmpty(value))
                {
                    if (value == "= 0" || value == "=0")
                        value = "= false";
                    else if (value == "= 1" || value == "=1")
                        value = "= true";
                    else
                        Debug.WriteLine($"Unsupported bool value: {value}");
                }
                else if (fieldType.Contains('[') && value == "= 0")
                {
                    value = "= []";
                }

                if (fieldType == "byte[]" && (b.BoxName == "MediaDataBox" || b.BoxName == "FreeSpaceBox_skip" || b.BoxName == "FreeSpaceBox" || b.BoxName == "ZeroBox"))
                {
                    fieldType = "StreamMarker";
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

                value = EscapeFourCC(value);

                string name = parserDocument.GetFieldName(field as PseudoField);
                string propertyName = name.ToPropertyCase();

                if (name == propertyName)
                    propertyName = "_" + name;

                int nestingLevel = parserDocument.GetLoopNestingLevel(field);
                if (nestingLevel > 0)
                {
                    string typedef = parserDocument.GetFieldTypeDef(field);
                    nestingLevel = parserDocument.GetNestedInLoopSuffix(field, typedef, out _);

                    parserDocument.AddRequiresAllocation((PseudoField)field);

                    if (nestingLevel > 0)
                    {
                        // change the type
                        for (int i = 0; i < nestingLevel; i++)
                        {
                            fieldType += "[]";
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
                    string suffix = fieldType.Contains("[]") ? "" : ".FirstOrDefault()";
                    string ttttt = fieldType.Replace("[]", "");
                    string ttt = fieldType.Contains("[]") ? ("IEnumerable<" + fieldType.Replace("[]", "") + ">") : fieldType;
                    return $"\tpublic {ttt} {propertyName} {{ get {{ return this.children.OfType<{ttttt}>(){suffix}; }} }}";
                }
                else
                {
                    if (b.BoxName == "SampleGroupDescriptionEntry")
                    {
                        if (fieldType == "Box[]")
                        {
                            fieldType = "List<Box>";
                            value = "= new List<Box>()";
                        }
                    }

                    return $"\r\n\tprotected {fieldType} {name}{value}; {comment}\r\n" + // must be "protected", derived classes access base members
                        $"\tpublic {fieldType} {propertyName} {{ get {{ return this.{name}; }} set {{ this.{name} = value; }} }}";
                }
            }
        }

        private string BuildMethod(PseudoClass b, PseudoBlock parent, PseudoCode field, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            var block = field as PseudoBlock;
            if (block != null)
            {
                return BuildBlock(b, parent, block, level, methodType);
            }

            var repeatingBlock = field as PseudoRepeatingBlock;
            if (repeatingBlock != null)
            {
                throw new NotSupportedException();
            }

            var comment = field as PseudoComment;
            if (comment != null)
            {
                return BuildComment(b, comment, level, methodType);
            }

            var swcase = field as PseudoCase;
            if (swcase != null)
            {
                return BuildSwitchCase(b, swcase, level, methodType);
            }

            string fieldType = (field as PseudoField).Type.ToString();

            if (string.IsNullOrEmpty(fieldType) && !string.IsNullOrEmpty((field as PseudoField)?.Name))
                fieldType = (field as PseudoField)?.Name?.Replace("[]", "").Replace("()", "");

            if (string.IsNullOrEmpty(fieldType))
                return "";

            if (parserDocument.IsWorkaround(fieldType))
            {
                Dictionary<string, string> map = new Dictionary<string, string>()
                {
                    { "int i, j",                                                "" },
                    { "int i,j",                                                 "" },
                    { "int i",                                                   "" },
                    { "j=1",                                                     "int j = 1;" },
                    { "subgroupIdLen = (num_subgroup_ids >= (1 << 8)) ? 16 : 8", "ulong subgroupIdLen = (ulong)((num_subgroup_ids >= (1 << 8)) ? 16 : 8);" },
                    { "totalPatternLength = 0",                                  "uint totalPatternLength = 0;" },
                    { "audioObjectType = 32 + audioObjectTypeExt",               "audioObjectType = (byte)(32 + audioObjectTypeExt);" },
                    { "sbrPresentFlag = -1",                                     "sbrPresentFlag = false;" },
                    { "psPresentFlag = -1",                                      "psPresentFlag = false;" },
                    { "sbrPresentFlag = 1",                                      "sbrPresentFlag = true;" },
                    { "psPresentFlag = 1",                                       "psPresentFlag = true;" },
                    { "int downmix_instructions_count = 1",                      "downmix_instructions_count = 1;" },
                    { "return audioObjectType;",                                 "// return audioObjectType;" },
                    { "extensionAudioObjectType = 0",                            "extensionAudioObjectType.AudioObjectType = 0;"},
                    { "extensionAudioObjectType = 5",                            "extensionAudioObjectType.AudioObjectType = 5;"},
                    { "samplerate = samplerate >> 16",                           "// samplerate = samplerate >> 16;"},
                };

                if (map.ContainsKey(fieldType))
                    return map[fieldType];
                else
                    return $"{fieldType};";
            }

            string name = parserDocument.GetFieldName(field as PseudoField);
            string m = methodType == MethodType.Read ? GetReadMethod(field as PseudoField) : (methodType == MethodType.Write ? GetWriteMethod(field as PseudoField) : GetCalculateSizeMethod(field as PseudoField));
            string typedef = "";
            typedef = parserDocument.GetFieldTypeDef(field);

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

            if (fieldComment != null && fieldComment.Contains("optional"))
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

            if (parserDocument.GetLoopNestingLevel(field) > 0)
            {
                string suffix;
                parserDocument.GetNestedInLoopSuffix(field, typedef, out suffix);
                typedef += suffix;

                if (methodType != MethodType.Size)
                {
                    m = parserDocument.FixNestedInLoopVariables(field, m, "(", ",");
                    m = parserDocument.FixNestedInLoopVariables(field, m, ")", ","); // when casting
                    m = parserDocument.FixNestedInLoopVariables(field, m, "", " ");
                }
                else
                {
                    m = parserDocument.FixNestedInLoopVariables(field, m, "", " ");
                }
            }

            if (methodType == MethodType.Read)
                return $"{spacing}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
            else if (methodType == MethodType.Write)
                return $"{spacing}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
            else
                return $"{spacing}{boxSize}{m}; // {name}";
        }

        private string BuildBlock(PseudoClass b, PseudoBlock parent, PseudoBlock block, int level, MethodType methodType)
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

                // fix single field flags (in C# 0 is not the same as false and non-zero is not the same as true)
                if (condition.Contains("grouping_type_parameter_present") ||
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

                if (condition.Contains("!channelConfiguration"))
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

                if (condition.Contains("AVCProfileIndication  ==  100  ||  AVCProfileIndication  ==  110"))
                {
                    // this condition is necessary, otherwise AVCDecoderConfigurationRecord can exceed its box size
                    if (methodType == MethodType.Read)
                    {
                        ret += $"\r\n{spacing}if (boxSize >= readSize || (readSize - boxSize) < 4) return boxSize; else HasExtensions = true;";
                    }
                    else
                    {
                        ret += $"\r\n{spacing}if (!HasExtensions) return boxSize;";
                    }
                }
            }

            condition = EscapeFourCC(condition);

            int nestedLevel = parserDocument.GetLoopNestingLevel(block);
            if (nestedLevel > 0)
            {
                // patch condition
                condition = parserDocument.FixNestedInLoopVariables(block, condition);
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
                        else if (variable.Contains("_minus1"))
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
                                int blockSuffixLevel = parserDocument.GetNestedInLoopSuffix(block, "", out suffix);
                                int fieldSuffixLevel = parserDocument.GetNestedInLoopSuffix(req, "", out _);

                                string appendType = "";
                                if (fieldSuffixLevel - blockSuffixLevel > 1)
                                {
                                    int count = fieldSuffixLevel - blockSuffixLevel - 1;

                                    for (int i = 0; i < count; i++)
                                    {
                                        appendType += "[]";
                                    }
                                }

                                string variableType = GetCSharpType(req);
                                int indexesTypeDef = parserDocument.GetFieldTypeDef(req).Count(x => x == '[');
                                int indexesType = variableType.Count(x => x == '[');
                                string variableName = parserDocument.GetFieldName(req) + suffix;
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
                ret += "\r\n" + BuildMethod(b, block, field, level + 1, methodType);
            }

            ret += $"\r\n{spacing}}}";

            return ret;
        }

        private string BuildSwitchCase(PseudoClass b, PseudoCase swcase, int level, MethodType methodType)
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

        private string BuildComment(PseudoClass b, PseudoComment comment, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            string text = comment.Comment;
            return $"{spacing}/* {text} */";
        }

        private string FixForCycleCondition(string condition)
        {
            condition = condition.Substring(1, condition.Length - 2);

            string[] parts = condition.Split(";");

            // indexes in C# begin at 0 instead of 1, shift the loop condition to this new range
            if (parts[0].Contains("1") && parts[1].Contains("="))
            {
                parts[0] = parts[0].Replace("1", "0");
                parts[1] = parts[1].Replace("=", "");
            }

            parts[1] = parts[1].Replace("pattern_length[j]", "IsoStream.GetInt(pattern_length[j])");

            return $"(int {string.Join(";", parts)})";
        }

        private string EscapeFourCC(string value)
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

        private string GetReadMethod(PseudoField field)
        {
            var info = parserDocument.GetTypeInfo(field);

            string parameter = "";
            if (!string.IsNullOrEmpty(field.Type.Param))
                parameter = field.Type.Param.Substring(1, field.Type.Param.Length - 2)
                    .Replace("audioObjectType", "audioObjectType.AudioObjectType")
                    .Replace("ptl_max_temporal_id[i]+1", "(byte)(ptl_max_temporal_id[i]+1)"); // TODO: fix this workaround

            string csharpResult = "";
            if (info.FieldType == ParsedBoxType.Class || info.FieldType == ParsedBoxType.Entry)
            {
                string factory;
                if (info.Type == "SampleGroupDescriptionEntry") // info.IsEntry? 
                {
                    factory = $"() => BoxFactory.CreateEntry(IsoStream.ToFourCC({parameter}))";
                }
                else
                {
                    factory = $"() => new {info.Type}({parameter})";
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
            else if (info.FieldType == ParsedBoxType.Box)
            {
                csharpResult = $"stream.ReadBox(boxSize, readSize, this, ";
            }
            else if (info.FieldType == ParsedBoxType.Descriptor)
            {
                csharpResult = $"stream.ReadDescriptor(boxSize, readSize, this, ";
            }
            else if (info.FieldType == ParsedBoxType.ByteAlignment)
            {
                csharpResult = "stream.ReadByteAlignment(boxSize, readSize, ";
            }
            else if (info.FieldType == ParsedBoxType.String)
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
            else if (info.FieldType == ParsedBoxType.Number)
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
                        csharpResult = "stream.ReadBit(boxSize, readSize, ";
                    }
                    else if (info.ElementSizeInBits == -1)
                    {
                        string elSizeVar = info.ElementSizeVariable
                            .Replace("8 ceil(size / 8) – size", "(Math.Ceiling(size / 8d) - size) * 8")
                            .Replace("f(pattern_size_code)", "pattern_size_code")
                            .Replace("f(count_size_code)", "count_size_code")
                            .Replace("f(index_size_code)", "index_size_code")
                            ;
                        csharpResult = $"stream.ReadBits{arraySuffix}(boxSize, readSize, (uint)({elSizeVar} ), ";
                    }
                    else if (info.ElementSizeInBits > 1 && info.ElementSizeInBits % 8 > 0)
                    {
                        csharpResult = $"stream.ReadBits(boxSize, readSize, {info.ElementSizeInBits}, ";
                    }
                    else if (info.ElementSizeInBits % 8 == 0)
                    {
                        if (info.IsSigned)
                            csharpResult = $"stream.ReadInt{info.ElementSizeInBits}{arraySuffix}(boxSize, readSize, {arrayParam}";
                        else
                            csharpResult = $"stream.ReadUInt{info.ElementSizeInBits}{arraySuffix}(boxSize, readSize, {arrayParam}";
                    }
                }
                else
                {
                    if (info.ElementSizeInBits == 32)
                    {
                        csharpResult = "stream.ReadDouble32(boxSize, readSize, ";
                    }
                    else
                    {
                        throw new NotSupportedException($"{info.Type} is unknown");
                    }
                }
            }

            if (info.Type == "string" && !info.IsSigned && info.ArrayDimensions == 0 && info.ElementSizeInBits == 15 && info.ArrayLengthVariable == "")
            {
                csharpResult = $"stream.ReadIso639(boxSize, readSize, ";
            }

            // TODO: fix this workaround
            csharpResult = csharpResult.Replace("constant_IV_size", "IsoStream.GetInt(constant_IV_size)");

            return csharpResult;
        }

        private string GetWriteMethod(PseudoField field)
        {
            var info = parserDocument.GetTypeInfo(field);

            string csharpResult = "";
            if (info.FieldType == ParsedBoxType.Class || info.FieldType == ParsedBoxType.Entry)
                csharpResult = "stream.WriteClass(";
            else if (info.FieldType == ParsedBoxType.Box)
                csharpResult = "stream.WriteBox(";
            else if (info.FieldType == ParsedBoxType.Descriptor)
                csharpResult = "stream.WriteDescriptor(";
            else if (info.FieldType == ParsedBoxType.ByteAlignment)
                csharpResult = "stream.WriteByteAlignment(";
            else if (info.FieldType == ParsedBoxType.String)
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
            else if (info.FieldType == ParsedBoxType.Number)
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

        private string GetCalculateSizeMethod(PseudoField field)
        {
            var info = parserDocument.GetTypeInfo(field);

            string csharpResult = "";
            if (info.FieldType == ParsedBoxType.Class || info.FieldType == ParsedBoxType.Entry)
                csharpResult = "IsoStream.CalculateClassSize(value)";
            else if (info.FieldType == ParsedBoxType.Box)
                csharpResult = "IsoStream.CalculateBoxSize(value)";
            else if (info.FieldType == ParsedBoxType.Descriptor)
                csharpResult = "IsoStream.CalculateDescriptorSize(value)";
            else if (info.FieldType == ParsedBoxType.ByteAlignment)
                csharpResult = "IsoStream.CalculateByteAlignmentSize(boxSize, value)";
            else if (info.FieldType == ParsedBoxType.String)
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
            else if (info.FieldType == ParsedBoxType.Number)
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

                if (info.IsArray)
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
            else if (info.FieldType == ParsedBoxType.Iso639)
            {
                csharpResult = $"{info.ElementSizeInBits}";
            }

            // TODO: fix this workaround
            csharpResult = csharpResult.Replace("constant_IV_size", "IsoStream.GetInt(constant_IV_size)");

            return csharpResult;
        }

        private string GetCSharpType(PseudoField field)
        {
            FieldTypeInfo info = parserDocument.GetTypeInfo(field);
            string t = "";
            int arrayDimensions = info.ArrayDimensions;
            if (info.FieldType == ParsedBoxType.Number)
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
                    if (info.ElementSizeInBits == 32)
                    {
                        t = "double";
                    }
                    else
                    {
                        throw new NotSupportedException($"{info.Type} is unknown");
                    }
                }
            }
            else if (info.FieldType == ParsedBoxType.String)
            {
                if (info.Type == "MultiLanguageString")
                    t = "MultiLanguageString";
                else
                    t = "BinaryUTF8String";
            }
            else if (info.FieldType == ParsedBoxType.Iso639)
            {
                t = "string";
            }
            else if (info.FieldType == ParsedBoxType.ByteAlignment)
            {
                t = "byte";
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

        private string GetSpacing(int level)
        {
            string ret = "";
            for (int i = 0; i < level; i++)
            {
                ret += "\t";
            }
            return ret;
        }
    }
}
