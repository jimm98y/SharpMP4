using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ItuGenerator
{
    public enum MethodType
    {
        Read,
        Write,
        Size
    }

    public class CSharpGenerator
    {
        public string GenerateParser(ItuClass ituClass)
        {
            string resultCode =
            @"using System;
using System.Linq;
using System.Collections.Generic;

namespace SharpMP4
{
";
            resultCode += GenerateClass(ituClass);
            resultCode +=
                @"
}
";
            return resultCode;
        }

        private string GenerateClass(ItuClass ituClass)
        {
            string resultCode = @$"
    public class {ituClass.ClassName.ToPropertyCase()} : IItuSerializable
    {{
";
            resultCode += GenerateFields(ituClass);

            resultCode += GenerateCtor(ituClass);

            resultCode += $@"
         public ulong Read(ItuStream stream)
         {{
             ulong size = 0;
";
            foreach (var field in ituClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(ituClass, null, field, 3, MethodType.Read);
            }
            resultCode += $@"

             return size;
         }}
";

            resultCode += $@"
    }}
";
            return resultCode;
        }







        private string BuildMethod(ItuClass b, ItuBlock parent, ItuCode field, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            var block = field as ItuBlock;
            if (block != null)
            {
                return BuildBlock(b, parent, block, level, methodType);
            }

            var comment = field as ItuComment;
            if (comment != null)
            {
                return BuildComment(b, comment, level, methodType);
            }

            if ((field as ItuField).Type == null)
            {
                // statement
                return BuildStatement(b, field as ItuField, level, methodType);
            }

            string fieldType = (field as ItuField).Type.ToString();
            
            string name = GetFieldName(field as ItuField);
            string m = methodType == MethodType.Read ? GetReadMethod(field as ItuField) : (methodType == MethodType.Write ? GetWriteMethod(field as ItuField) : GetCalculateSizeMethod(field as ItuField));
            string typedef = "";

            string fieldComment = "";
            if (!string.IsNullOrEmpty((field as ItuField)?.Comment))
            {
                fieldComment = "//" + (field as ItuField).Comment;
            }

            string boxSize = "size += ";

            if (methodType == MethodType.Size)
            {
                m = m.Replace("value", name);
            }

            // if (parserDocument.GetLoopNestingLevel(field) > 0)
            
            if (methodType == MethodType.Read)
                return $"{spacing}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
            else if (methodType == MethodType.Write)
                return $"{spacing}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
            else
                return $"{spacing}{boxSize}{m}; // {name}";
        }

        private string BuildStatement(ItuClass b, ItuField field, int level, MethodType methodType)
        {
            return $"{field.Name} {field.Value};";
        }

        private string GetCalculateSizeMethod(ItuField ituField)
        {
            throw new NotImplementedException();
        }

        private string GetWriteMethod(ItuField ituField)
        {
            throw new NotImplementedException();
        }

        private string GetReadMethod(ItuField ituField)
        {
            switch(ituField.Type)
            {
                case "f(1)":
                    return "stream.ReadFixed(size, 1, ";
                case "f(8)":
                    return "stream.ReadFixed(size, 8, ";
                case "u(1)":
                    return "stream.ReadUnsignedInt(size, 1, ";               
                case "u(2)":
                    return "stream.ReadUnsignedInt(size, 2, ";
                case "u(5)":
                    return "stream.ReadUnsignedInt(size, 5, ";
                case "b(8)":
                    return "stream.ReadBits(size, 8, ";
                default:
                    throw new NotImplementedException();
            }
        }

        private string BuildComment(ItuClass b, ItuComment comment, int level, MethodType methodType)
        {
            throw new NotImplementedException();
        }

        private string BuildBlock(ItuClass b, ItuBlock parent, ItuBlock block, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            string ret = "";

            string condition = block.Condition;
            string blockType = block.Type;

            ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

            foreach (var field in block.Content)
            {
                ret += "\r\n" + BuildMethod(b, block, field, level + 1, methodType);
            }

            ret += $"\r\n{spacing}}}";

            return ret;
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

        private string GenerateCtor(ItuClass ituClass)
        {
            string resultCode = "";

            resultCode = $@"
         public {ituClass.ClassName.ToPropertyCase()}{ituClass.ClassParameter}
         {{ }}
";

            return resultCode;
        }

        private string GenerateFields(ItuClass ituClass)
        {
            string resultCode = "";
            ituClass.FlattenedFields = FlattenFields(ituClass.Fields);

            foreach(var field in ituClass.FlattenedFields)
            {
                resultCode += BuildField(field);
            }

            return resultCode;
        }

        private string BuildField(ItuField field)
        {
            string type = GetFieldType(field);
            string propertyName = GetFieldName(field).ToPropertyCase();
            return $"\t\tprivate {type} {field.Name};\r\n\t\tpublic {type} {propertyName} {{ get {{ return {field.Name}; }} set {{ {field.Name} = value; }} }}\r\n";
        }

        private string GetFieldName(ItuField field)
        {
            return field.Name;
        }

        private string GetFieldType(ItuField field)
        {
            if (field.Type == null)
                return field.Name.ToPropertyCase(); // type is a class

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "f(1)",                       "bool" },
                { "f(8)",                       "byte" },
                { "u(1)",                       "bool" },
                { "u(2)",                       "uint" }, // unsigned integer
                { "u(5)",                       "byte" },
                { "b(8)",                       "byte" }, // byte
            };

            Debug.WriteLine($"Field type: {field.Type}, opt: array: {field.ArrayParameter}");
            return map[field.Type];
        }

        private List<ItuField> FlattenFields(IEnumerable<ItuCode> fields, ItuCode parent = null)
        {
            Dictionary<string, ItuField> ret = new Dictionary<string, ItuField>();
            foreach (var code in fields)
            {
                if (code is ItuField field)
                {
                    field.Parent = parent; // keep track of parent blocks

                    if (!string.IsNullOrEmpty(field.Value))
                        continue;

                    string value = field.Value;
                    AddAndResolveDuplicates(ret, field);
                }
                else if (code is ItuBlock block)
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

        private void AddAndResolveDuplicates(Dictionary<string, ItuField> ret, ItuField field)
        {
            string name = field.Name;
            int index = 0;
            if (!ret.TryAdd(name, field))
            {
                while (!ret.TryAdd($"{name}{index}", field))
                {
                    index++;
                }

                field.Name = $"{name}{index}";
            }
        }
    }
}
