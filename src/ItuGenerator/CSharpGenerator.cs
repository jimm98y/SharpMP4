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
        public string GenerateParser(string type, ItuClass ituClass)
        {
            string resultCode =
            $@"using System;
using System.Linq;
using System.Collections.Generic;

namespace Sharp{type}
{{
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
            ituClass.AddedFields.Clear();
            resultCode += $@"

             return size;
         }}
";

            resultCode += $@"
         public ulong Write(ItuStream stream)
         {{
             ulong size = 0;
";
            foreach (var field in ituClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(ituClass, null, field, 3, MethodType.Write);
            }
            ituClass.AddedFields.Clear();
            resultCode += $@"

             return size;
         }}
";

            resultCode += $@"
         public ulong CalculateSize(ItuStream stream)
         {{
             ulong size = 0;
";
            foreach (var field in ituClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(ituClass, null, field, 3, MethodType.Size);
            }
            ituClass.AddedFields.Clear();
            resultCode += $@"

             return size;
         }}
";

            resultCode += $@"
    }}
";
            return resultCode;
        }

        public int GetLoopNestingLevel(ItuCode code)
        {
            int ret = 0;
            var field = code as ItuField;
            var block = code as ItuBlock;
            ItuBlock parent = null;

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

            if ((field as ItuField).Type == null && !string.IsNullOrWhiteSpace((field as ItuField).Value))
            {
                // statement
                return BuildStatement(b, parent, field as ItuField, level, methodType);
            }
           
            string name = GetFieldName(field as ItuField);
            string m = methodType == MethodType.Read ? GetReadMethod(field as ItuField) : (methodType == MethodType.Write ? GetWriteMethod(field as ItuField) : GetCalculateSizeMethod(field as ItuField));
            string typedef = (field as ItuField).ArrayParameter;

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

            if (GetLoopNestingLevel(field) > 0)
            {
                //string suffix;
                //GetNestedInLoopSuffix(field, typedef, out suffix);
                //typedef += suffix;

                //if (methodType != MethodType.Size)
                //{
                //    m = FixNestedInLoopVariables(field, m, "(", ",");
                //    m = FixNestedInLoopVariables(field, m, ")", ","); // when casting
                //    m = FixNestedInLoopVariables(field, m, "", " ");
                //}
                //else
                //{
                //    m = FixNestedInLoopVariables(field, m, "", " ");
                //}
            }
            if (methodType == MethodType.Read)
                return $"{spacing}{boxSize}{m} out this.{name}{typedef}); {fieldComment}";
            else if (methodType == MethodType.Write)
                return $"{spacing}{boxSize}{m} this.{name}{typedef}); {fieldComment}";
            else
                return $"{spacing}{boxSize}{m}; // {name}";
        }

        private string BuildStatement(ItuClass b, ItuBlock parent, ItuField field, int level, MethodType methodType)
        {
            if (b.FlattenedFields.FirstOrDefault(x => x.Name == field.Name) != null || parent != null)
            {
                return $"{GetSpacing(level)}{field.Name} {field.Value};";
            }
            else
            {
                if(b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                    b.AddedFields.Add(new ItuField() { Name = field.Name, Value = field.Value });
                return $"{GetSpacing(level)}var {field.Name} {field.Value};";
            }
        }

        private string GetCalculateSizeMethod(ItuField ituField)
        {
            switch (ituField.Type)
            {
                case "f(1)":
                    return "1";
                case "f(8)":
                    return "8";
                case "u(1)":
                    return "1";
                case "u(2)":
                    return "2";
                case "u(5)":
                    return "5";
                case "b(8)":
                    return "8";
                default:
                    if (ituField.Type == null)
                        return $"stream.CalculateSize<{ituField.Name.ToPropertyCase()}>(value)";
                    throw new NotImplementedException();
            }
        }

        private string GetWriteMethod(ItuField ituField)
        {
            switch (ituField.Type)
            {
                case "f(1)":
                    return "stream.WriteFixed(1, ";
                case "f(8)":
                    return "stream.WriteFixed(8, ";
                case "u(1)":
                    return "stream.WriteUnsignedInt(1, ";
                case "u(2)":
                    return "stream.WriteUnsignedInt(2, ";
                case "u(5)":
                    return "stream.WriteUnsignedInt(5, ";
                case "b(8)":
                    return "stream.WriteBits(8, ";
                default:
                    if (ituField.Type == null)
                        return $"stream.WriteClass<{ituField.Name.ToPropertyCase()}>(";
                    throw new NotImplementedException();
            }
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
                    if (ituField.Type == null)
                        return $"stream.ReadClass<{ituField.Name.ToPropertyCase()}>(size, ";
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

            if (block.Type == "for")
            {
                condition = FixForCycleCondition(condition);
            }

            if (!string.IsNullOrEmpty(condition))
            {
                condition = condition.Replace("next_bits(", "stream.NextBits(");
            }

            if (methodType == MethodType.Read)
            {
                if (block.RequiresAllocation.Count > 0)
                {
                    if (block.Type == "for")
                    {
                        string[] parts = condition.Substring(1, condition.Length - 2).Split(';');
                        string variable = parts[1].Split('<', '=', '>', '!').Last();

                        if (!string.IsNullOrWhiteSpace(variable))
                        {
                            foreach (var req in block.RequiresAllocation)
                            {
                                string suffix;
                                int blockSuffixLevel = GetNestedInLoopSuffix(block, "", out suffix);
                                int fieldSuffixLevel = GetNestedInLoopSuffix(req, "", out _);

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
                                int indexesTypeDef = req.ArrayParameter.Count(x => x == '[');
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

        private string FixForCycleCondition(string condition)
        {
            condition = condition.Substring(1, condition.Length - 2);

            string[] parts = condition.Split(";");
            return $"(int {string.Join(";", parts)})";
        }

        public string FixNestedInLoopVariables(ItuCode code, string condition, string prefix = "", string suffix = "")
        {
            if (string.IsNullOrEmpty(condition))
                return condition;

            List<string> ret = new List<string>();
            ItuBlock parent = null;
            var field = code as ItuField;
            if (field != null)
                parent = field.Parent;
            var block = code as ItuBlock;
            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i ="))
                        ret.Insert(0, "[i]");
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
                    if (f is ItuField ff)
                    {
                        if (string.IsNullOrWhiteSpace(ff.Name) || !string.IsNullOrEmpty(ff.Value))
                            continue;
                        if (condition.Contains(prefix + ff.Name + suffix) && !condition.Contains(prefix + ff.Name + "["))
                        {
                            condition = condition.Replace(prefix + ff.Name + suffix, prefix + ff.Name + str + suffix);
                        }
                    }
                    else if (f is ItuBlock bb)
                    {
                        foreach (var fff in bb.Content)
                        {
                            if (fff is ItuField ffff)
                            {
                                if (string.IsNullOrWhiteSpace(ffff.Name) || !string.IsNullOrEmpty(ffff.Value))
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

        private int GetNestedInLoopSuffix(ItuCode code, string currentSuffix, out string result)
        {
            List<string> ret = new List<string>();
            ItuBlock parent = null;
            var field = code as ItuField;
            if (field != null)
                parent = field.Parent;
            var block = code as ItuBlock;
            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i ="))
                        ret.Insert(0, "[i]");
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
            string[] ctorParameters = GetCtorParameters(ituClass);
            string[] ctorParameterDefs = ctorParameters.Select(x => $"{GetCSharpTypeMapping()[GetCtorParameterType(x)]} {x}").ToArray();
            string ituClassParameters = $"({string.Join(", ", ctorParameterDefs)})";

            string ctorInitializer = string.Join("\r\n", ctorParameters.Select(x => $"\t\t\tthis.{x.ToFirstLower()} = {x};"));

            string resultCode = $@"
         public {ituClass.ClassName.ToPropertyCase()}{ituClassParameters}
         {{ 
{ctorInitializer}
         }}
";
            return resultCode;
        }

        private string[] GetCtorParameters(ItuClass ituClass)
        {
            return ituClass.ClassParameter.Substring(1, ituClass.ClassParameter.Length - 2).Split(',').Select(x => x.Trim()).ToArray();
        }

        private string GetCtorParameterType(string parameter)
        {
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "NumBytesInNALunit", "u(32)" },
            };

            return map[parameter];
        }

        private string GenerateFields(ItuClass ituClass)
        {
            string resultCode = "";
            ituClass.FlattenedFields = FlattenFields(ituClass, ituClass.Fields);

            foreach(var field in ituClass.FlattenedFields)
            {
                resultCode += BuildField(field);
            }

            return resultCode;
        }

        private string BuildField(ItuField field)
        {
            string type = GetCSharpType(field);

            int nestingLevel = GetLoopNestingLevel(field);
            if (nestingLevel > 0)
            {
                AddRequiresAllocation(field);
            }

            string propertyName = GetFieldName(field).ToPropertyCase();
            return $"\t\tprivate {type} {field.Name.ToFirstLower()};\r\n\t\tpublic {type} {propertyName} {{ get {{ return {field.Name}; }} set {{ {field.Name} = value; }} }}\r\n";
        }

        public void AddRequiresAllocation(ItuField field)
        {
            ItuBlock parent = null;
            parent = field.Parent;
            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    if (!string.IsNullOrEmpty(field.ArrayParameter))
                    {
                        // add the allocation above the first for in the hierarchy
                        parent.RequiresAllocation.Add(field);
                    }
                }

                parent = parent.Parent;
            }
        }

        private string GetFieldName(ItuField field)
        {
            return field.Name;
        }

        private string GetCSharpType(ItuField field)
        {
            if (field.Type == null)
                return field.Name.ToPropertyCase(); // type is a class

            Dictionary<string, string> map = GetCSharpTypeMapping();

            Debug.WriteLine($"Field type: {field.Type}, opt: array: {field.ArrayParameter}");
            return map[field.Type] + (!string.IsNullOrWhiteSpace(field.ArrayParameter) ? "[]" : "");
        }

        private static Dictionary<string, string> GetCSharpTypeMapping()
        {
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "f(1)",                       "bool" },
                { "f(8)",                       "byte" },
                { "u(1)",                       "bool" },
                { "u(2)",                       "uint" }, // unsigned integer
                { "u(5)",                       "byte" },
                { "u(32)",                      "uint" },
                { "b(8)",                       "byte" }, // byte
            };
            return map;
        }

        private List<ItuField> FlattenFields(ItuClass cls, IEnumerable<ItuCode> fields, ItuBlock parent = null)
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

                    var blockFields = FlattenFields(cls, block.Content, block);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(ret, blockField);
                    }
                }
            }

            if (parent == null)
            {
                // add also ctor params as fields
                string[] ctorParams = GetCtorParameters(cls);
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    AddAndResolveDuplicates(ret,
                        new ItuField()
                        {
                            Name = ctorParams[i].ToFirstLower(),
                            Type = GetCtorParameterType(ctorParams[i])
                        });
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
                //while (!ret.TryAdd($"{name}{index}", field))
                //{
                //    index++;
                //}

                //field.Name = $"{name}{index}";
            }
        }
    }
}
