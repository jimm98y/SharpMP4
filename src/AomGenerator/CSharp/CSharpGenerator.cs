using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AomGenerator.CSharp
{
    public enum MethodType
    {
        Read,
        Write
    }

    public interface ICustomGenerator
    {
        string FixVariableType(string variableType);
        string FixAppendType(string appendType, string variableName);
        string GetParameterType(string parameter);
        string FixAllocations(string spacing, string appendType, string variableType, string variableName);
        string AppendMethod(AomCode field, MethodType methodType, string spacing, string retm);
        string PreprocessDefinitionsFile(string definitions);
        string GetFieldDefaultValue(AomField field);
        void FixClassParameters(AomClass ituClass);
        string ReplaceParameter(string parameter);
        string GetVariableSize(string parameter);
        string FixMissingParameters(AomClass b, string parameter, string classType);
        string FixCondition(string condition, MethodType methodType);
        string FixStatement(string fieldValue);
        string GetCtorParameterType(string parameter);
        string FixFieldValue(string fieldValue);
        void FixMethodAllocation(string name, ref string method, ref string typedef);
        string GetDerivedVariables(string name);
        void FixNestedIndexes(List<string> ret, AomField field);
    }

    public class CSharpGenerator
    {
        private ICustomGenerator specificGenerator = null;

        public CSharpGenerator(ICustomGenerator specificGenerator)
        {
            this.specificGenerator = specificGenerator ?? throw new ArgumentNullException(nameof(specificGenerator));
        }

        public string GenerateParser(string type, IEnumerable<AomClass> aomClasses)
        {
            string resultCode =
            $@"using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sharp{type}
{{
";
            resultCode += GenerateContext(type, aomClasses);

            foreach (var ituClass in aomClasses)
            {
                resultCode += GenerateClass(ituClass);
            }

            resultCode +=
                @"
}
";
            return resultCode;
        }

        private string GenerateContext(string type, IEnumerable<AomClass> aomClasses)
        {
            string ret = @$"
    public partial class {type}Context : IAomContext
    {{
        public ObuHeader ObuHeader {{ get; set; }}
";
            var obu = aomClasses.Where(x => x.ClassName.EndsWith("_obu")).ToArray();
            foreach (var cls in obu)
            {
                ret += $"\t\tpublic {cls.ClassName.ToPropertyCase()} {cls.ClassName.ToPropertyCase()} {{ get; set; }}\r\n";
            }

            ret += @$"
    }}
";
            return ret;
        }

        private string GenerateClass(AomClass aomClass)
        {
            specificGenerator.FixClassParameters(aomClass);

            string resultCode = @$"
    /*
{aomClass.Syntax.Replace("*/", "*//*")}
    */
    public class {aomClass.ClassName.ToPropertyCase()} : IAomSerializable
    {{
";
            resultCode += GenerateFields(aomClass);

            resultCode += GenerateCtor(aomClass);

            resultCode += $@"
         public ulong Read(IAomContext context, AomStream stream)
         {{
            ulong size = 0;
";
            resultCode += BuildRequiredVariables(aomClass);

            foreach (var field in aomClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(aomClass, null, field, 3, MethodType.Read);
            }
            aomClass.AddedFields.Clear();
            resultCode += $@"

            return size;
         }}
";

            resultCode += $@"
         public ulong Write(IAomContext context, AomStream stream)
         {{
            ulong size = 0;
";
            resultCode += BuildRequiredVariables(aomClass);

            foreach (var field in aomClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(aomClass, null, field, 3, MethodType.Write);
            }
            aomClass.AddedFields.Clear();
            resultCode += $@"

            return size;
         }}
";

            resultCode += $@"
    }}
";
            return resultCode;
        }

        private string GenerateCtor(AomClass aomClass)
        {
            string[] ctorParameters = GetCtorParameters(aomClass);
            var typeMappings = GetCSharpTypeMapping();
            string[] ctorParameterDefs = ctorParameters.Select(x => $"{(typeMappings.ContainsKey(specificGenerator.GetCtorParameterType(x)) ? typeMappings[specificGenerator.GetCtorParameterType(x)] : "")} {x}").ToArray();
            string ituClassParameters = $"({string.Join(", ", ctorParameterDefs)})";

            string ctorInitializer = string.Join("\r\n", ctorParameters.Select(x => $"\t\t\tthis.{x.ToFirstLower()} = {x};"));

            string resultCode = $@"
         public {aomClass.ClassName.ToPropertyCase()}{ituClassParameters}
         {{ 
{ctorInitializer}
         }}
";
            return resultCode;
        }

        private string[] GetCtorParameters(AomClass aomClass)
        {
            var parameters = aomClass.ClassParameter.Substring(1, aomClass.ClassParameter.Length - 2).Split(',').Select(x => x.Trim()).ToArray();
            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0]))
            {
                return new string[] { };
            }
            return parameters;
        }

        private string GenerateFields(AomClass aomClass)
        {
            string resultCode = "";
            aomClass.FlattenedFields = FlattenFields(aomClass, aomClass.Fields);

            foreach (var field in aomClass.FlattenedFields)
            {
                resultCode += BuildField(aomClass, field);
            }

            return resultCode;
        }

        private List<AomField> FlattenFields(AomClass b, IEnumerable<AomCode> fields, AomBlock parent = null)
        {
            Dictionary<string, AomField> ret = new Dictionary<string, AomField>();

            if (parent == null)
            {
                // add also ctor params as fields
                string[] ctorParams = GetCtorParameters(b);
                for (int i = 0; i < ctorParams.Length; i++)
                {
                    var f = new AomField()
                    {
                        Name = ctorParams[i].ToFirstLower(),
                        Type = specificGenerator.GetCtorParameterType(ctorParams[i])
                    };
                    b.AddedFields.Add(f);
                    AddAndResolveDuplicates(b, ret, f);
                }
            }

            foreach (var code in fields)
            {
                if (code is AomField field)
                {
                    field.Parent = parent; // keep track of parent blocks

                    var p = parent;
                    while (p != null)
                    {
                        if (p.Type == "do" || p.Type == "while")
                        {
                            field.MakeList = true;
                            Debug.WriteLine($"Field {field.Name} is a list");
                        }
                        p = p.Parent;
                    }

                    AddAndResolveDuplicates(b, ret, field);
                }
                else if (code is AomBlock block)
                {
                    block.Parent = parent; // keep track of parent blocks

                    // make sure we define for cycle variables
                    if (block.Type == "for")
                    {
                        string[] partsFor = block.Condition.Substring(1, block.Condition.Length - 2).Split(';');
                        string[] parts = partsFor[0].Split(',');
                        var conditionChars = new char[] { '=' };
                        foreach (var part in parts)
                        {
                            int variableIndex = part.IndexOfAny(conditionChars);
                            if (variableIndex == -1)
                                continue;
                            string variable = part.Substring(0, variableIndex).TrimStart(conditionChars).Trim();

                            if (b.RequiresDefinition.FirstOrDefault(x => x.Name == variable) == null && b.AddedFields.FirstOrDefault(x => x.Name == variable) == null)
                            {
                                b.RequiresDefinition.Add(new AomField() { Name = variable, Type = "ns(32)" });
                            }
                        }
                    }

                    if (block.Type == "do" || block.Type == "while")
                    {
                        if (b.RequiresDefinition.FirstOrDefault(x => x.Name == "whileIndex") == null)
                        {
                            var f = new AomField() { Name = "whileIndex", Type = "su(32)", Value = "= -1" };
                            b.RequiresDefinition.Add(f);
                        }
                    }

                    var blockFields = FlattenFields(b, block.Content, block);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }
                }
                else if (code is AomBlockIfThenElse blockifThenElse)
                {
                    blockifThenElse.Parent = parent; // keep track of parent blocks
                    ((AomBlock)blockifThenElse.BlockIf).Parent = parent;
                    if ((AomBlock)blockifThenElse.BlockElse != null) ((AomBlock)blockifThenElse.BlockElse).Parent = parent;

                    var blockFields = FlattenFields(b, ((AomBlock)blockifThenElse.BlockIf).Content, (AomBlock)blockifThenElse.BlockIf);
                    foreach (var blockField in blockFields)
                    {
                        AddAndResolveDuplicates(b, ret, blockField);
                    }

                    foreach (var blockelseif in blockifThenElse.BlockElseIf)
                    {
                        ((AomBlock)blockelseif).Parent = parent;
                        var blockElseIfFields = FlattenFields(b, ((AomBlock)blockelseif).Content, (AomBlock)blockelseif);
                        foreach (var blockField in blockElseIfFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockField);
                        }
                    }

                    if (blockifThenElse.BlockElse != null)
                    {
                        var blockElseFields = FlattenFields(b, ((AomBlock)blockifThenElse.BlockElse).Content, (AomBlock)blockifThenElse.BlockElse);
                        foreach (var blockElseField in blockElseFields)
                        {
                            AddAndResolveDuplicates(b, ret, blockElseField);
                        }
                    }
                }
            }

            return ret.Values.ToList();
        }

        private void AddAndResolveDuplicates(AomClass b, Dictionary<string, AomField> ret, AomField field)
        {
            if (string.IsNullOrEmpty(field.Type))
            {
                if (!string.IsNullOrEmpty(field.Increment))
                {
                    return;
                }
                else if (!string.IsNullOrEmpty(field.Value) && b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null)
                {
                    if (b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null)
                    {
                        string requiredType = specificGenerator.GetParameterType(field.Name);
                        if (!string.IsNullOrEmpty(requiredType))
                        {
                            b.RequiresDefinition.Add(new AomField() { Name = field.Name, Type = requiredType, FieldArray = field.FieldArray });
                        }
                    }

                    return;
                }
            }

            string name = field.Name;
            if (!ret.TryAdd(name, field))
            {
                if (string.IsNullOrEmpty(field.Type))
                {
                    if (field.Parameter != ret[name].Parameter) // we have to duplicate these
                    {
                        AddNewDuplicatedField(ret, field, name);
                    }
                    else
                    {
                        // just log a warning for now
                        Debug.WriteLine($"-------Field {field.Name} already exists in {b.ClassName} class, possible issue with the value being overwritten! Type: {field.Type}, Value: {field.Value}");
                    }
                }
                else
                {
                    // just log a warning for now
                    Debug.WriteLine($"-------Field {field.Name} already exists in {b.ClassName} class, possible issue with the value being overwritten! Type: {field.Type}, Value: {field.Value}");
                }
            }
        }

        private static void AddNewDuplicatedField(Dictionary<string, AomField> ret, AomField field, string name)
        {
            int index = 0;
            while (!ret.TryAdd($"{name}{index}", field))
            {
                index++;
            }

            field.ClassType = field.Name;
            field.Name = $"{name}{index}";
        }

        private static string BuildRequiredVariables(AomClass aomClass)
        {
            string resultCode = "";

            foreach (var v in aomClass.RequiresDefinition)
            {
                string type = GetCSharpTypeMapping()[v.Type];
                if (string.IsNullOrEmpty(v.FieldArray))
                {
                    string value = "= 0";
                    if (!string.IsNullOrWhiteSpace(v.Value))
                    {
                        value = v.Value;
                    }

                    resultCode += $"\r\n\t\t\t{type} {v.Name} {value};";
                }
                else
                {
                    int arrayDimensions = 0;
                    int indicesArrayDimensions = 0;
                    int level = 0;
                    for (int i = 0; i < v.FieldArray.Length; i++)
                    {
                        if (v.FieldArray[i] == '[')
                        {
                            if (level == 0)
                                arrayDimensions++;

                            level++;
                        }
                        else if (v.FieldArray[i] == ']')
                        {
                            level--;
                        }
                        else if (v.FieldArray[i] == ',')
                        {
                            indicesArrayDimensions++;
                        }
                    }

                    string array = "";
                    if (arrayDimensions == 1 && indicesArrayDimensions > 0)
                    {
                        array += "[";
                        for (int i = 0; i < indicesArrayDimensions; i++)
                        {
                            array += ",";
                        }
                        array += "]";
                    }
                    else
                    {
                        for (int i = 0; i < arrayDimensions; i++)
                        {
                            array += "[]";
                        }
                    }
                    resultCode += $"\r\n\t\t\t{type}{array} {v.Name} = null;"; // TODO: size
                }
            }

            return resultCode;
        }

        private string BuildField(AomClass ituClass, AomField field)
        {
            string type = GetCSharpType(field);
            if (ituClass.AddedFields != null && ituClass.AddedFields.FirstOrDefault(x => x.Name == field.Name) != null)
            {
                // NumOutputLayerSets - adding a calculated field as a property
                if (string.IsNullOrEmpty(field.Type))
                    type = GetCSharpTypeMapping()[ituClass.AddedFields.FirstOrDefault(x => x.Name == field.Name).Type];
            }

            string defaultInitializer = specificGenerator.GetFieldDefaultValue(field);
            string initializer = string.IsNullOrEmpty(defaultInitializer) ? "" : $"= {defaultInitializer}";

            if (field.MakeList)
            {
                type = $"Dictionary<int, {type}>";
                initializer = $" = new {type}()";
            }
            else
            {

                int nestingLevel = GetLoopNestingLevel(field);
                if (nestingLevel > 0)
                {
                    nestingLevel = GetNestedInLoopSuffix(field, field.FieldArray, out _);

                    AddRequiresAllocation(field);

                    if (nestingLevel > 0)
                    {
                        // change the type
                        for (int i = 0; i < nestingLevel; i++)
                        {
                            type += "[]";
                            initializer = "";
                        }
                    }
                }
            }

            string propertyName = field.Name.ToPropertyCase();
            if (propertyName == ituClass.ClassName.ToPropertyCase())
            {
                propertyName = $"_{propertyName}";
            }

            return $"\t\tprivate {type} {field.Name.ToFirstLower()}{initializer};\r\n\t\tpublic {type} {propertyName} {{ get {{ return {field.Name}; }} set {{ {field.Name} = value; }} }}\r\n";
        }

        public void AddRequiresAllocation(AomField field)
        {
            AomBlock parent = null;
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

        public int GetLoopNestingLevel(AomCode code)
        {
            int ret = 0;
            var field = code as AomField;
            var block = code as AomBlock;
            AomBlock parent = null;

            if (field != null)
                parent = field.Parent;

            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for" || parent.Type == "do" || parent.Type == "while")
                    ret++;
                parent = parent.Parent;
            }

            return ret;
        }

        private int GetNestedInLoopSuffix(AomCode code, string currentSuffix, out string result)
        {
            List<string> ret = new List<string>();
            AomBlock parent = null;
            var field = code as AomField;
            if (field != null)
                parent = field.Parent;
            var block = code as AomBlock;
            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    ret.Insert(0, $"[{parent.Condition.Substring(1, parent.Condition.Length - 2).Split(';').First().Split('=').First()}]");
                }
                else if (parent.Type == "do" || parent.Type == "while")
                {
                    ret.Insert(0, $"[whileIndex]");
                }

                parent = parent.Parent;
            }

            specificGenerator.FixNestedIndexes(ret, field);

            foreach (var suffix in ret.ToArray())
            {
                if (!string.IsNullOrEmpty(currentSuffix) && currentSuffix.Replace(" ", "").Replace("-2", "").Contains(suffix.Replace(" ", "")))
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

        private string BuildMethod(AomClass b, AomBlock parent, AomCode field, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            var block = field as AomBlock;
            if (block != null)
            {
                return BuildBlock(b, parent, block, level, methodType);
            }

            var blockIf = field as AomBlockIfThenElse;
            if (blockIf != null)
            {
                string ret = BuildBlock(b, parent, (AomBlock)blockIf.BlockIf, level, methodType);
                foreach (var elseBlock in blockIf.BlockElseIf)
                {
                    ret += BuildBlock(b, parent, (AomBlock)elseBlock, level, methodType);
                }
                if (blockIf.BlockElse != null)
                {
                    ret += BuildBlock(b, parent, (AomBlock)blockIf.BlockElse, level, methodType);
                }
                return ret;
            }

            var comment = field as AomComment;
            if (comment != null)
            {
                return BuildComment(b, comment, level, methodType);
            }

            var retrn = field as AomReturn;
            if (retrn != null)
            {
                return BuildReturn(b, parent, retrn, level, methodType);
            }

            var brk = field as AomBreak;
            if (brk != null)
            {
                return "break;";
            }

            if ((field as AomField).Type == null && (!string.IsNullOrWhiteSpace((field as AomField).Value) || !string.IsNullOrWhiteSpace((field as AomField).Increment)))
            {
                return BuildStatement(b, parent, field as AomField, level, methodType);
            }

            string name = (field as AomField).Name;
            string m = methodType == MethodType.Read ? GetReadMethod(b, field as AomField) : GetWriteMethod(field as AomField);

            string typedef = (field as AomField).FieldArray;

            string fieldComment = "";
            if (!string.IsNullOrEmpty((field as AomField)?.Comment))
            {
                fieldComment = "//" + (field as AomField).Comment;
            }

            string boxSize = "size += ";

            if (GetLoopNestingLevel(field) > 0)
            {
                if (!(field as AomField).MakeList)
                {
                    string suffix;
                    GetNestedInLoopSuffix(field, typedef, out suffix);
                    typedef += suffix;

                    m = FixNestedInLoopVariables(field, m, "(", ",");
                    m = FixNestedInLoopVariables(field, m, ")", ","); // when casting
                    m = FixNestedInLoopVariables(field, m, "", " ");
                }
            }

            string retm = "";

            if (m.Contains("###value###") && methodType == MethodType.Read)
            {
                specificGenerator.FixMethodAllocation(name, ref m, ref typedef);

                if ((field as AomField).MakeList)
                {
                    // special case, create class first, then read it
                    m = m.Replace("###value###", $"{spacing}this.{name}{typedef}.Add(whileIndex, ");
                    m = m.Replace("###size###", $");\r\n{spacing}{boxSize}");
                    retm = $"{m} this.{name}{typedef}[whileIndex], \"{name}\"); {fieldComment}";
                }
                else
                {
                    // special case, create class first, then read it
                    m = m.Replace("###value###", $"{spacing}this.{name}{typedef} = ");
                    m = m.Replace("###size###", $";\r\n{spacing}{boxSize}");
                    retm = $"{m} this.{name}{typedef}, \"{name}\"); {fieldComment}";
                }
            }
            else
            {
                if ((field as AomField).MakeList)
                {
                    if (methodType == MethodType.Read)
                        retm = $"{spacing}{boxSize}{m} whileIndex, this.{name}{typedef}, \"{name}\"); {fieldComment}";
                    else if (methodType == MethodType.Write)
                        retm = $"{spacing}{boxSize}{m} whileIndex, this.{name}{typedef}, \"{name}\"); {fieldComment}";
                    else
                        throw new NotSupportedException();
                }
                else
                {
                    if (methodType == MethodType.Read)
                        retm = $"{spacing}{boxSize}{m} out this.{name}{typedef}, \"{name}\"); {fieldComment}";
                    else if (methodType == MethodType.Write)
                        retm = $"{spacing}{boxSize}{m} this.{name}{typedef}, \"{name}\"); {fieldComment}";
                    else
                        throw new NotSupportedException();
                }
            }

            retm = specificGenerator.AppendMethod(field, methodType, spacing, retm);

            string hookDerivedVariables = specificGenerator.GetDerivedVariables(name);
            if (!string.IsNullOrEmpty(hookDerivedVariables))
            {
                retm += $"\r\n{spacing}{hookDerivedVariables}";
            }

            return retm;
        }

        private string BuildReturn(AomClass b, AomBlock parent, AomReturn retrn, int level, MethodType methodType)
        {
            string p = "";
            if (!string.IsNullOrEmpty(retrn.Parameter))
                p = retrn.Parameter;

            return "return" + p + ";";
        }

        private string BuildStatement(AomClass b, AomBlock parent, AomField field, int level, MethodType methodType)
        {
            string fieldValue = field.Value;
            string fieldArray = field.FieldArray;

            if (!string.IsNullOrEmpty(fieldArray))
                fieldArray = specificGenerator.FixStatement(fieldArray);

            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValue = specificGenerator.FixStatement(fieldValue);
                fieldValue = specificGenerator.FixFieldValue(fieldValue);

                string trimmed = fieldValue.TrimStart(new char[] { ' ', '=' });
                if (trimmed.StartsWith("!"))
                {
                    fieldValue = $"= {trimmed.Substring(1)} == 0";
                }

                if (fieldValue.Contains("flag") && !fieldValue.Contains(")"))
                    fieldValue = fieldValue.Replace("||", "|").Replace("&&", "&");

                if ((fieldValue.Contains("==") || fieldValue.Contains(">") || fieldValue.Contains("<")) && !fieldValue.Contains("?") && !fieldValue.Contains("<<") && !fieldValue.Contains(">>"))
                    fieldValue += " ? (uint)1 : (uint)0";

                fieldValue = FixMissingFields(b, fieldValue);
                fieldValue = FixNestedInLoopVariables(field, fieldValue);
            }

            if (b.FlattenedFields.FirstOrDefault(x => x.Name == field.Name) != null || parent != null)
            {
                return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue}".TrimEnd() + ";";
            }
            else
            {
                if (b.AddedFields.FirstOrDefault(x => x.Name == field.Name) == null && b.RequiresDefinition.FirstOrDefault(x => x.Name == field.Name) == null)
                {
                    b.AddedFields.Add(new AomField() { Name = field.Name, Value = fieldValue });
                    return $"{GetSpacing(level)}uint {field.Name}{fieldArray}{fieldValue}".TrimEnd() + ";";
                }
                else
                {
                    return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue}".TrimEnd() + ";";
                }
            }
        }

        private string FixMissingFields(AomClass b, string condition)
        {
            Regex r = new Regex("\\b[a-zA-Z_][\\w_]+");
            var matches = r.Matches(condition).OfType<Match>().Select(x => x.Value).Distinct().ToArray();
            foreach (var match in matches)
            {
                if (b.FlattenedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.AddedFields.FirstOrDefault(x => x.Name == match) == null &&
                    b.RequiresDefinition.FirstOrDefault(x => x.Name == match) == null
                    )
                {
                    condition = condition.Replace(match, specificGenerator.ReplaceParameter(match));
                }
            }

            return condition;
        }

        public string FixNestedInLoopVariables(AomCode code, string condition, string prefix = "", string suffix = "")
        {
            if (string.IsNullOrEmpty(condition))
                return condition;

            List<string> ret = new List<string>();
            AomBlock parent = null;
            var field = code as AomField;
            if (field != null)
                parent = field.Parent;
            var block = code as AomBlock;
            if (block != null)
                parent = block.Parent;

            while (parent != null)
            {
                if (parent.Type == "for")
                {
                    if (parent.Condition.Contains("i =") || parent.Condition.Contains("i=") || parent.Condition.Contains("i =") || parent.Condition.Contains("i++"))
                        ret.Insert(0, "[i]");
                    else if (parent.Condition.Contains("j =") || parent.Condition.Contains("j=") || parent.Condition.Contains("j ="))
                        ret.Insert(0, "[j]");
                    else if (parent.Condition.Contains("opNum =") || parent.Condition.Contains("opNum=") || parent.Condition.Contains("opNum ="))
                        ret.Insert(0, "[opNum]");
                    else if (parent.Condition.Contains("segmentId =") || parent.Condition.Contains("segmentId=") || parent.Condition.Contains("segmentId ="))
                        ret.Insert(0, "[segmentId]");
                    else if (parent.Condition.Contains("startSb =") || parent.Condition.Contains("startSb=") || parent.Condition.Contains("startSb ="))
                        ret.Insert(0, "[startSb]");
                    else if (parent.Condition.Contains("ref =") || parent.Condition.Contains("ref=") || parent.Condition.Contains("ref ="))
                        ret.Insert(0, "[ref]");
                    else if (parent.Condition.Contains("TileNum =") || parent.Condition.Contains("TileNum=") || parent.Condition.Contains("TileNum ="))
                        ret.Insert(0, "[TileNum]");
                    else if (parent.Condition.Contains("plane =") || parent.Condition.Contains("plane=") || parent.Condition.Contains("plane ="))
                        ret.Insert(0, "[plane]");
                    else if (parent.Condition.Contains("pass =") || parent.Condition.Contains("pass=") || parent.Condition.Contains("pass ="))
                        ret.Insert(0, "[pass]");
                    else if (parent.Condition.Contains("r =") || parent.Condition.Contains("r=") || parent.Condition.Contains("r ="))
                        ret.Insert(0, "[r]");
                    else if (parent.Condition.Contains("c =") || parent.Condition.Contains("c=") || parent.Condition.Contains("c ="))
                        ret.Insert(0, "[c]");
                    else if (parent.Condition.Contains("y =") || parent.Condition.Contains("y=") || parent.Condition.Contains("y ="))
                        ret.Insert(0, "[y]");
                    else if (parent.Condition.Contains("x =") || parent.Condition.Contains("x=") || parent.Condition.Contains("x ="))
                        ret.Insert(0, "[x]");
                    else if (parent.Condition.Contains("refList =") || parent.Condition.Contains("refList=") || parent.Condition.Contains("refList ="))
                        ret.Insert(0, "[refList]");
                    else if (parent.Condition.Contains("row =") || parent.Condition.Contains("row=") || parent.Condition.Contains("row ="))
                        ret.Insert(0, "[row]");
                    else if (parent.Condition.Contains("col =") || parent.Condition.Contains("col=") || parent.Condition.Contains("col ="))
                        ret.Insert(0, "[col]");
                    else if (parent.Condition.Contains("chunkY =") || parent.Condition.Contains("chunkY=") || parent.Condition.Contains("chunkY ="))
                        ret.Insert(0, "[chunkY]");
                    else if (parent.Condition.Contains("chunkX =") || parent.Condition.Contains("chunkX=") || parent.Condition.Contains("chunkX ="))
                        ret.Insert(0, "[chunkX]");
                    else if (parent.Condition.Contains("txSz =") || parent.Condition.Contains("txSz=") || parent.Condition.Contains("txSz ="))
                        ret.Insert(0, "[txSz]");
                    else if (parent.Condition.Contains("k =") || parent.Condition.Contains("k=") || parent.Condition.Contains("k ="))
                        ret.Insert(0, "[k]");
                    else if (parent.Condition.Contains("unitRow =") || parent.Condition.Contains("unitRow=") || parent.Condition.Contains("unitRow ="))
                        ret.Insert(0, "[unitRow]");
                    else if (parent.Condition.Contains("unitCol =") || parent.Condition.Contains("unitCol=") || parent.Condition.Contains("unitCol ="))
                        ret.Insert(0, "[unitCol]");
                    else if (parent.Condition.Contains("tile =") || parent.Condition.Contains("tile=") || parent.Condition.Contains("tile ="))
                        ret.Insert(0, "[tile]");
                    else
                        throw new Exception();
                }
                else if (parent.Type == "do" || parent.Type == "while")
                {
                    ret.Insert(0, "[whileIndex]");
                }

                parent = parent.Parent;
            }

            if (block != null && (block.Type == "do" || block.Type == "while"))
            {
                ret.Insert(0, "[whileIndex]");
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
                else if (parent.Type == "do" || parent.Type == "while")
                {
                    level++;
                }

                foreach (var f in parent.Content)
                {
                    if (f is AomField ff)
                    {
                        if (string.IsNullOrWhiteSpace(ff.Name) || !string.IsNullOrEmpty(ff.Value))
                            continue;
                        if (condition.Contains(prefix + ff.Name + suffix) && !condition.Contains(prefix + ff.Name + "["))
                        {
                            condition = condition.Replace(prefix + ff.Name + suffix, prefix + ff.Name + str + suffix);
                        }
                    }
                    else if (f is AomBlock bb)
                    {
                        foreach (var fff in bb.Content)
                        {
                            if (fff is AomField ffff)
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

        private string BuildComment(AomClass b, AomComment comment, int level, MethodType methodType)
        {
            return $"/* {comment.Comment} */\r\n";
        }

        private string BuildBlock(AomClass b, AomBlock parent, AomBlock block, int level, MethodType methodType)
        {
            string spacing = GetSpacing(level);
            string ret = "";

            string condition = block.Condition;
            string blockType = block.Type;

            if (!string.IsNullOrEmpty(condition))
            {
                if (blockType == "if" || blockType == "else if" || blockType == "while" || blockType == "do")
                {
                    condition = FixCondition(b, condition, methodType);
                }
                else if (blockType == "for")
                {
                    condition = specificGenerator.FixCondition(condition, methodType);
                }
            }

            if (!string.IsNullOrEmpty(condition))
            {
                condition = condition.Replace("<<", "<< (int)");

                condition = FixMissingFields(b, condition);

                int nestedLevel = GetLoopNestingLevel(block);
                condition = FixNestedInLoopVariables(block, condition, "", ";");
                condition = FixNestedInLoopVariables(block, condition, "", "=");
                condition = FixNestedInLoopVariables(block, condition, "", " ");
                condition = FixNestedInLoopVariables(block, condition, "", ")");
                condition = FixNestedInLoopVariables(block, condition, "", ",");
            }

            if (methodType == MethodType.Read)
            {
                if (block.RequiresAllocation.Count > 0)
                {
                    if (block.Type == "for")
                    {
                        string[] parts = condition.Substring(1, condition.Length - 2).Split(';');

                        var conditionChars = new char[] { '<', '=', '>', '!' };
                        int variableIndex = parts[1].IndexOfAny(conditionChars);
                        if (variableIndex != -1)
                        {
                            string variable = parts[1].Substring(variableIndex).TrimStart(conditionChars);

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
                                    int indexesTypeDef = req.FieldArray.Count(x => x == '[');
                                    int indexesType = variableType.Count(x => x == '[');
                                    string variableName = req.Name + suffix;

                                    if (variableType.Contains("[]"))
                                    {
                                        int diff = indexesType - indexesTypeDef;
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

                                    appendType = specificGenerator.FixAppendType(appendType, variableName);

                                    variableType = specificGenerator.FixVariableType(variableType);

                                    if (methodType == MethodType.Read)
                                    {
                                        string fixedAllocations = specificGenerator.FixAllocations(spacing, appendType, variableType, variableName);
                                        if (!string.IsNullOrEmpty(fixedAllocations))
                                        {
                                            ret += fixedAllocations;
                                        }

                                        if (fixedAllocations == "") // when null, don't append anything
                                        {
                                            ret += $"\r\n{spacing}this.{variableName} = new {variableType}{appendType};";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("allocation for block other than for");
                    }
                }
            }

            if (block.Type == "do")
                ret += $"\r\n{spacing}{blockType}\r\n{spacing}{{";
            else
                ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

            if (blockType == "do" || blockType == "while")
                ret += $"\r\n{spacing}\twhileIndex++;\r\n";

            foreach (var field in block.Content)
            {
                ret += "\r\n" + BuildMethod(b, block, field, level + 1, methodType);
            }

            ret += $"\r\n{spacing}}}";

            if (block.Type == "do")
                ret += $" while {condition};";

            return ret;
        }

        private string FixCondition(AomClass b, string condition, MethodType methodType)
        {
            string[] parts = condition.Substring(1, condition.Length - 2).Split(new string[] { "||", "&&" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].Trim();

                if (parts[i].StartsWith("!"))
                {
                    string trimmed = parts[i].Trim(new char[] { ' ', '(', ')' });
                    if (!trimmed.Contains("(") && !parts[i].Contains("()")) 
                    {
                        // we don't have bool anymore, so in this case it's easy fix
                        condition = condition.Replace(parts[i].TrimEnd(')'), parts[i].TrimEnd(')').Substring(1, parts[i].TrimEnd(')').Length - 1) + "== 0");
                    }
                }
                else if (!parts[i].Contains('=') && !parts[i].Contains('>') && !parts[i].Contains('<'))
                {
                    string trimmed = parts[i].Trim(new char[] { ' ', '(', ')' });
                    if (!trimmed.Contains("(") && !parts[i].Contains("()")) 
                    {
                        if (trimmed.StartsWith("!"))
                        {
                            if (condition.Contains($"{trimmed} != 0"))
                                condition = condition.Replace($"{trimmed} != 0", $"{trimmed.Substring(1)} == 0");
                            else
                                condition = condition.Replace(trimmed, trimmed.Substring(1) + " == 0");
                        }
                        else
                            condition = condition.Replace(trimmed, trimmed + " != 0");
                    }
                }
            }

            condition = specificGenerator.FixCondition(condition, methodType);

            return condition;
        }

        private string GetCSharpType(AomField field)
        {
            if (string.IsNullOrWhiteSpace(field.Type))
            {
                if (!string.IsNullOrEmpty(field.ClassType))
                    return field.ClassType.ToPropertyCase();
                else
                    return field.Name.ToPropertyCase();
            }

            Dictionary<string, string> map = GetCSharpTypeMapping();

            int arrayDimensions = 0;
            if (!string.IsNullOrEmpty(field.FieldArray))
            {
                int level = 0;
                for (int i = 0; i < field.FieldArray.Length; i++)
                {
                    if (field.FieldArray[i] == '[')
                    {
                        if (level == 0)
                            arrayDimensions++;

                        level++;
                    }
                    else if (field.FieldArray[i] == ']')
                    {
                        level--;
                    }
                }
            }


            string arraySuffix = "";
            for (int i = 0; i < arrayDimensions; i++)
            {
                arraySuffix += "[]";
            }

            return map[field.Type] + arraySuffix;
        }

        private static Dictionary<string, string> GetCSharpTypeMapping()
        {
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "f(1)",                       "uint" },
                { "f(2)",                       "uint" },
                { "f(3)",                       "uint" },
                { "f(4)",                       "uint" },
                { "f(5)",                       "uint" },
                { "f(6)",                       "uint" },
                { "f(8)",                       "uint" },
                { "f(9)",                       "uint" },
                { "f(16)",                      "uint" },
                { "f(32)",                      "uint" },
                { "f(32)[]",                    "uint[]" },
                { "f(b2)",                      "uint" },
                { "f(bitsToRead)",              "uint" },
                { "f(idLen)",                   "uint" },
                { "f(N)",                       "uint" },
                { "f(n)",                       "uint" },
                { "f(OrderHintBits)",           "uint" },
                { "f(SUPERRES_DENOM_BITS)",     "uint" },
                { "f(tileBits)",                "uint" },
                { "f(TileRowsLog2+TileColsLog2)", "uint" },
                { "f(time_offset_length)",      "uint" },
                { "L(1)",                       "uint" },
                { "L(2)",                       "uint" },
                { "L(3)",                       "uint" },
                { "L(b2)",                      "uint" },
                { "L(BitDepth)",                "uint" },
                { "L(cdef_bits)",               "uint" },
                { "L(delta_q_rem_bits)",        "uint" },
                { "L(n)",                       "uint" },
                { "L(paletteBits)",             "uint" },
                { "L(SGRPROJ_PARAMS_BITS)",     "uint" },
                { "le(TileSizeBytes)",          "uint" },
                { "leb128()",                   "uint" },
                { "ns(32)",                     "uint" },
                { "ns(maxWidth)",               "uint" },
                { "ns(maxHeight)",              "uint" },
                { "ns(numSyms - mk)",           "uint" },
                { "NS(numSyms - mk)",           "uint" },
                { "NS(PaletteSizeUV)",          "uint" },
                { "S()",                        "uint" },
                { "su(32)",                     "int" },
                { "su(1+6)",                    "uint" },
                { "uvlc()",                     "uint" },
            };
            return map;
        }

        private string GetReadMethod(AomClass b, AomField aomField)
        {
            switch (aomField.Type)
            {
                case "f(1)":
                    return "stream.ReadFixed(size, 1,";
                
                case "f(2)":
                    return "stream.ReadFixed(size, 2,";
                
                case "f(3)":
                    return "stream.ReadFixed(size, 3,";

                case "f(4)":
                    return "stream.ReadFixed(size, 4,";

                case "f(5)":
                    return "stream.ReadFixed(size, 5,";

                case "f(6)":
                    return "stream.ReadFixed(size, 6,";

                case "f(8)":
                    return "stream.ReadFixed(size, 8,";

                case "f(9)":
                    return "stream.ReadFixed(size, 9,";

                case "f(12)":
                    return "stream.ReadFixed(size, 12,";

                case "f(16)":
                    return "stream.ReadFixed(size, 16,";

                case "f(32)":
                    return "stream.ReadFixed(size, 32,";

                case "f(b2)":
                    return "stream.ReadVariable(size, b2,";

                case "f(bitsToRead)":
                    return "stream.ReadVariable(size, bitsToRead,";

                case "f(idLen)":
                    return "stream.ReadVariable(size, idLen,";

                case "f(N)":
                    return "stream.ReadVariable(size, N,";

                case "f(n)":
                    return "stream.ReadVariable(size, n,";

                case "f(OrderHintBits)":
                    return "stream.ReadVariable(size, OrderHintBits,";

                case "f(SUPERRES_DENOM_BITS)":
                    return "stream.ReadVariable(size, SUPERRES_DENOM_BITS,";

                case "f(tileBits)":
                    return "stream.ReadVariable(size, tileBits,";

                case "f(TileRowsLog2+TileColsLog2)":
                    return "stream.ReadVariable(size, TileRowsLog2+TileColsLog2,";

                case "f(time_offset_length)":
                    return "stream.ReadVariable(size, time_offset_length,";

                case "L(1)":
                    return "stream.ReadL(size, 1,";

                case "L(2)":
                    return "stream.ReadL(size, 2,";

                case "L(3)":
                    return "stream.ReadL(size, 3,";

                case "L(b2)":
                    return "stream.ReadL(size, b2,";

                case "L(BitDepth)":
                    return "stream.ReadL(size, BitDepth,";

                case "L(cdef_bits)":
                    return "stream.ReadL(size, cdef_bits,";

                case "L(delta_q_rem_bits)":
                    return "stream.ReadL(size, delta_q_rem_bits,";

                case "L(n)":
                    return "stream.ReadL(size, n,";

                case "L(paletteBits)":
                    return "stream.ReadL(size, paletteBits,";

                case "L(SGRPROJ_PARAMS_BITS)":
                    return "stream.ReadL(size, SGRPROJ_PARAMS_BITS,";

                case "le(TileSizeBytes)":
                    return "stream.ReadLeVar(size, TileSizeBytes,";

                case "leb128()":
                    return "stream.ReadLeb128(size, ";

                case "ns(32)":
                    return "stream.ReadUnsignedInt32(size,";

                case "ns(maxHeight)":
                    return "stream.ReadUnsignedInt(size, maxHeight,";

                case "ns(maxWidth)":
                    return "stream.ReadUnsignedInt(size, maxWidth,";

                case "NS(numSyms - mk)":
                case "ns(numSyms - mk)":
                    return "stream.ReadUnsignedInt(size, numSyms - mk,";

                case "NS(PaletteSizeUV)":
                    return "stream.ReadNS(size, PaletteSizeUV,";

                case "S()":
                    return "stream.ReadS(size,";

                case "su(32)":
                    return "stream.ReadSignedInt32(size,";

                case "su(1+6)":
                    return "stream.ReadSignedIntVar(size, 1+6,";

                case "su(1+bitsToRead)":
                    return "stream.ReadSignedIntVar(size, 1+bitsToRead,";

                case "uvlc()":
                    return "stream.ReadUvlc(size, ";

                default:
                    if (aomField.Type == null)
                    {

                        string par = specificGenerator.FixMissingParameters(b, aomField.Parameter, aomField.ClassType);
                        string[] parameters = par.TrimStart('(').TrimEnd(')').Split(new char[] { ',' });
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (!parameters[i].Contains(' ') && !parameters[i].Contains('\"'))
                                parameters[i] = specificGenerator.ReplaceParameter(parameters[i].Trim());
                            else
                            {
                                string[] parts = parameters[i].Split(new char[] { ' ' });
                                for (int j = 0; j < parts.Length; j++)
                                {
                                    parts[j] = specificGenerator.ReplaceParameter(parts[j].Trim());
                                }
                                parameters[i] = string.Join(" ", parts);
                            }
                        }
                        par = $"({string.Join(", ", parameters)})";
                        return $"###value### new {aomField.ClassType.ToPropertyCase()}{par} ###size### stream.ReadClass<{aomField.ClassType.ToPropertyCase()}>(size, context,";
                    }
                    throw new NotImplementedException();
            }
        }

        private string GetWriteMethod(AomField aomField)
        {
            switch (aomField.Type)
            {
                case "f(1)":
                    return "stream.WriteFixed(1,";
                
                case "f(2)":
                    return "stream.WriteFixed(2,";
                
                case "f(3)":
                    return "stream.WriteFixed(3,";

                case "f(4)":
                    return "stream.WriteFixed(4,";

                case "f(5)":
                    return "stream.WriteFixed(5,";

                case "f(6)":
                    return "stream.WriteFixed(6,";

                case "f(8)":
                    return "stream.WriteFixed(8,";

                case "f(9)":
                    return "stream.WriteFixed(9,";

                case "f(12)":
                    return "stream.WriteFixed(12,";

                case "f(16)":
                    return "stream.WriteFixed(16,";

                case "f(32)":
                    return "stream.WriteFixed(32,";

                case "f(b2)":
                    return "stream.WriteVariable(b2,";

                case "f(bitsToRead)":
                    return "stream.WriteVariable(bitsToRead,";

                case "f(idLen)":
                    return "stream.WriteVariable(idLen,";

                case "f(N)":
                    return "stream.WriteVariable(N,";

                case "f(n)":
                    return "stream.WriteVariable(n,";

                case "f(paletteBits)":
                    return "stream.WriteVariable(paletteBits,";

                case "f(OrderHintBits)":
                    return "stream.WriteVariable(OrderHintBits,";

                case "f(SUPERRES_DENOM_BITS)":
                    return "stream.WriteVariable(SUPERRES_DENOM_BITS,";

                case "f(tileBits)":
                    return "stream.WriteVariable(tileBits,";

                case "f(TileRowsLog2+TileColsLog2)":
                    return "stream.WriteVariable(TileRowsLog2+TileColsLog2,";

                case "f(time_offset_length)":
                    return "stream.WriteVariable(time_offset_length,";

                case "L(1)":
                    return "stream.WriteL(1, ";

                case "L(2)":
                    return "stream.WriteL(2, ";

                case "L(3)":
                    return "stream.WriteL(3, ";

                case "L(b2)":
                    return "stream.WriteL(b2, ";

                case "L(BitDepth)":
                    return "stream.WriteL(BitDepth, ";

                case "L(cdef_bits)":
                    return "stream.WriteL(cdef_bits, ";

                case "L(delta_q_rem_bits)":
                    return "stream.WriteL(delta_q_rem_bits, ";

                case "L(n)":
                    return "stream.WriteL(n, ";

                case "L(paletteBits)":
                    return "stream.WriteL(paletteBits, ";

                case "L(SGRPROJ_PARAMS_BITS)":
                    return "stream.WriteL(SGRPROJ_PARAMS_BITS, ";

                case "le(TileSizeBytes)":
                    return "stream.WriteLeVar(TileSizeBytes, ";

                case "leb128()":
                    return "stream.WriteLeb128(";

                case "ns(32)":
                    return "stream.WriteUnsignedInt32(";

                case "ns(maxHeight)":
                    return "stream.WriteUnsignedInt(maxHeight,";

                case "ns(maxWidth)":
                    return "stream.WriteUnsignedInt(maxWidth,";

                case "NS(numSyms - mk)":
                case "ns(numSyms - mk)":
                    return "stream.WriteUnsignedInt(numSyms - mk,";

                case "NS(PaletteSizeUV)":
                    return "stream.WriteNS(PaletteSizeUV,";

                case "S()":
                    return "stream.WriteS(";

                case "su(32)":
                    return "stream.WriteSignedInt32(";

                case "su(1+6)":
                    return "stream.WriteSignedIntVar(1+6,";

                case "su(1+bitsToRead)":
                    return "stream.WriteSignedIntVar(1+bitsToRead,";

                case "uvlc()":
                    return "stream.WriteUvlc(";

                default:
                    if (aomField.Type == null)
                    {
                        return $"stream.WriteClass<{aomField.ClassType.ToPropertyCase()}>(context,";
                    }
                    throw new NotImplementedException();
            }
        }
    }
}
