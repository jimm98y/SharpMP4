using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AomGenerator.CSharp
{
    public interface ICustomGenerator
    {
        string PreprocessDefinitionsFile(string definitions);
        string GetFieldDefaultValue(AomField field);
        string FixCondition(string condition);
        string FixStatement(string fieldValue);
        string GetCtorParameterType(string parameter);
        string AppendMethod(AomCode field, string spacing, string retm);

        /// <summary>
        /// Translates the specification's expressions, where the generator writes them itself; null, the
        /// expressions are fixed up as they are (FixStatement, FixCondition).
        /// </summary>
        AomExpressions Expressions { get; }

        /// <summary>The C# type of a field; null, the generator's own mapping.</summary>
        string GetFieldType(AomField field);

        /// <summary>The start of the call that reads a field; null, the generator's own.</summary>
        string GetReadMethod(AomField field);

        /// <summary>
        /// Syntax structures whose fields - those they assign, and those of the structures they call -
        /// are listed in the generated SyntaxFields, for the processes that save and load "all the syntax
        /// elements read in" one (save_sequence_header and the like).
        /// </summary>
        IEnumerable<string> FieldSets { get; }

        /// <summary>Called with every syntax structure before any is generated, to learn what it needs of them.</summary>
        void Prepare(IEnumerable<AomMethod> methods);

        /// <summary>The C# type of a structure's parameter; null, the generator's own.</summary>
        string GetParameterType(AomMethod method, string parameter);

        /// <summary>The C# type a structure returns, if it returns a value; null, the generator's own.</summary>
        string GetReturnType(AomMethod method);
    }

    public class CSharpGenerator
    {
        private ICustomGenerator specificGenerator = null;

        private HashSet<string> _fields = new HashSet<string>();
        private HashSet<string> _properties = new HashSet<string>();

        public CSharpGenerator(ICustomGenerator specificGenerator)
        {
            this.specificGenerator = specificGenerator ?? throw new ArgumentNullException(nameof(specificGenerator));
        }

        public string GenerateParser(string type, IEnumerable<AomMethod> aomClasses)
        {
            string resultCode =
            $@"using System;
using System.Collections.Generic;
using System.Numerics;
using SharpAVX;

namespace Sharp{type}
{{
";
            resultCode += @$"
    public partial class {type}Context : IAomContext
    {{";

            specificGenerator.Prepare(aomClasses);
            foreach (var aomClass in aomClasses)
                FindSyntaxElements(aomClass.Fields);

            foreach (var aomClass in aomClasses)
            {
                resultCode += GenerateMethods(aomClass);
            }

            resultCode += GenerateFieldSets();

            resultCode += @$"
    }}
}}
";
            return resultCode;
        }

        // What each syntax structure assigns, and which it calls.
        private readonly Dictionary<string, (HashSet<string> Fields, HashSet<string> Calls)> _structures = new Dictionary<string, (HashSet<string>, HashSet<string>)>();

        private void RecordStructure(AomMethod aomClass)
        {
            var parameters = new HashSet<string>(aomClass.AddedFields.Select(f => f.Name));
            var fields = new HashSet<string>(aomClass.FlattenedFields.Select(f => f.Name).Where(n => !parameters.Contains(n) && _fields.Contains(n)));
            var calls = new HashSet<string>();
            CollectCalls(aomClass.Fields, calls);
            _structures[aomClass.MethodName] = (fields, calls);
        }

        private static void CollectCalls(IEnumerable<AomCode> code, HashSet<string> calls)
        {
            foreach (var c in code)
            {
                if (c is AomField f && f.Type == null && string.IsNullOrWhiteSpace(f.Value) && string.IsNullOrWhiteSpace(f.Increment) && f.Parameter != null)
                    calls.Add(f.ClassType);
                else if (c is AomTuple t && t.Value.Contains("("))
                    calls.Add(t.Value.Substring(0, t.Value.IndexOf('(')).Trim());
                else if (c is AomBlock block)
                    CollectCalls(block.Content, calls);
                else if (c is AomBlockIfThenElse ifThenElse)
                {
                    CollectCalls(((AomBlock)ifThenElse.BlockIf).Content, calls);
                    foreach (var elseIf in ifThenElse.BlockElseIf)
                        CollectCalls(((AomBlock)elseIf).Content, calls);
                    if (ifThenElse.BlockElse != null)
                        CollectCalls(((AomBlock)ifThenElse.BlockElse).Content, calls);
                }
            }
        }

        /// <summary>
        /// For each structure the custom generator names, a class that holds what it assigns (and what the
        /// structures it calls assign), with Save{Name}() and Load{Name}( state ) to copy those fields out
        /// and back, arrays and all. "*" is every field: the whole context, which the hand-written part
        /// adds its own to through the partial methods SaveContextExtra and LoadContextExtra.
        /// </summary>
        private string GenerateFieldSets()
        {
            var sets = specificGenerator.FieldSets?.ToList();
            if (sets == null || sets.Count == 0)
                return "";

            var result = new StringBuilder();
            foreach (string structure in sets)
            {
                bool all = structure == "*";
                var fields = new SortedSet<string>(StringComparer.Ordinal);
                if (all)
                {
                    fields.UnionWith(_fieldTypes.Keys);
                }
                else
                {
                    var visited = new HashSet<string>();
                    var pending = new Stack<string>(new[] { structure });
                    while (pending.Count > 0)
                    {
                        string name = pending.Pop();
                        if (!visited.Add(name) || !_structures.TryGetValue(name, out var info))
                            continue;
                        fields.UnionWith(info.Fields.Where(_fieldTypes.ContainsKey));
                        foreach (string call in info.Calls)
                            pending.Push(call);
                    }
                }

                string name2 = all ? "Context" : structure.ToPropertyCase();
                string copy(string value, string type) =>
                    type.StartsWith("AomArray<") ? $"{value}?.Clone()" : type == "byte[]" ? $"(byte[]){value}?.Clone()" : value;

                result.Append($"\r\n\t\t/// <summary>What {(all ? "the context holds" : structure + " and the structures it calls assign")}, as Save{name2} keeps it.</summary>\r\n");
                result.Append($"\t\tprivate sealed partial class {name2}State\r\n\t\t{{\r\n");
                foreach (string field in fields)
                    result.Append($"\t\t\tpublic {_fieldTypes[field]} {field};\r\n");
                result.Append("\t\t}\r\n");

                result.Append($"\r\n\t\tprivate {name2}State Save{name2}()\r\n\t\t{{\r\n\t\t\tvar state = new {name2}State();\r\n");
                foreach (string field in fields)
                    result.Append($"\t\t\tstate.{field} = {copy("this." + field, _fieldTypes[field])};\r\n");
                if (all)
                    result.Append("\t\t\tSaveContextExtra(state);\r\n");
                result.Append("\t\t\treturn state;\r\n\t\t}\r\n");

                result.Append($"\r\n\t\tprivate void Load{name2}({name2}State state)\r\n\t\t{{\r\n");
                foreach (string field in fields)
                    result.Append($"\t\t\tthis.{field} = {copy("state." + field, _fieldTypes[field])};\r\n");
                if (all)
                    result.Append("\t\t\tLoadContextExtra(state);\r\n");
                result.Append("\t\t}\r\n");

                if (all)
                {
                    result.Append("\r\n\t\tpartial void SaveContextExtra(ContextState state);\r\n");
                    result.Append("\t\tpartial void LoadContextExtra(ContextState state);\r\n");
                }
            }
            return result.ToString();
        }

        private string GenerateMethods(AomMethod aomClass)
        {
            string resultCode = @$"
    /*
{aomClass.Syntax.Replace("*/", "*//*")}
    */
";
            resultCode += GenerateFields(aomClass);
            RecordStructure(aomClass);


            string[] ctorParameters = GetMethodParameters(aomClass);
            var typeMappings = GetCSharpTypeMapping();
            string[] ctorParameterDefs = ctorParameters.Select(x => $"{specificGenerator.GetParameterType(aomClass, x) ?? (typeMappings.ContainsKey(specificGenerator.GetCtorParameterType(x)) ? typeMappings[specificGenerator.GetCtorParameterType(x)] : "")} {x}").ToArray();
            string ituClassParameters = $"{string.Join(", ", ctorParameterDefs)}";

            resultCode += BuildRequiredVariables(aomClass);

            string retType = "void";
            if(aomClass.HasReturn)
            {
                retType = specificGenerator.GetReturnType(aomClass) ?? GetReturnType(aomClass) ?? "int";
            }

            resultCode += $@"
        private {retType} {aomClass.MethodName.ToPropertyCase()}({ituClassParameters})
        {{";

            // A process's lower case variables are its own (4.9 in AV1's and AV2's specifications): its
            // loop counters are locals, or a function called in a loop over i would move the caller's i.
            // AV1's syntax structures keep theirs as fields, as they always have; its functions do not.
            if (specificGenerator.Expressions != null || aomClass.HasReturn)
            {
                foreach (var counter in aomClass.RequiresDefinition.Where(v => !ctorParameters.Contains(v.Name)).Select(v => v.Name).Distinct())
                    resultCode += $"\r\n\t\t\tint {counter} = 0;";
            }

            // In AV2 every such variable is: startBitPos of tile_group_obu is not frame_header's.
            if (specificGenerator.Expressions != null)
            {
                var counters = new HashSet<string>(aomClass.RequiresDefinition.Select(v => v.Name));
                foreach (var local in aomClass.FlattenedFields.Where(f => IsLocal(f) && !ctorParameters.Contains(f.Name) && !counters.Contains(f.Name)))
                {
                    string type = GetCSharpType(local);
                    string value = type.StartsWith("AomArray<") ? NewAomArray(type) : type == "byte[]" ? "null" : "0";
                    resultCode += $"\r\n\t\t\t{type} {local.Name} = {value};";
                }
            }
            foreach (var field in aomClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(aomClass, null, field, 3);
            }
            resultCode += $@"
        }}
";

            return resultCode;
        }

        /// <summary>
        /// The type of a function that returns several values - return ( a, b ) - as a tuple of theirs;
        /// null for one that returns one.
        /// </summary>
        private string GetReturnType(AomMethod aomClass)
        {
            var returned = FindReturns(aomClass.Fields).FirstOrDefault(r => r.Parameter.Trim().StartsWith("(") && SplitTopLevel(r.Parameter).Count > 1);
            if (returned == null)
                return null;

            var types = SplitTopLevel(returned.Parameter).Select(name => _fieldTypes.TryGetValue(name, out string type) ? type : "int");
            return $"({string.Join(", ", types)})";
        }

        private static IEnumerable<AomReturn> FindReturns(IEnumerable<AomCode> code)
        {
            foreach (var c in code)
            {
                if (c is AomReturn r && !string.IsNullOrWhiteSpace(r.Parameter))
                    yield return r;
                else if (c is AomBlock block)
                    foreach (var inner in FindReturns(block.Content)) yield return inner;
                else if (c is AomBlockIfThenElse ifThenElse)
                {
                    foreach (var inner in FindReturns(((AomBlock)ifThenElse.BlockIf).Content)) yield return inner;
                    foreach (var elseIf in ifThenElse.BlockElseIf)
                        foreach (var inner in FindReturns(((AomBlock)elseIf).Content)) yield return inner;
                    if (ifThenElse.BlockElse != null)
                        foreach (var inner in FindReturns(((AomBlock)ifThenElse.BlockElse).Content)) yield return inner;
                }
            }
        }

        /// <summary>" ( a, f( b ) ) " to "a" and "f( b )".</summary>
        private static List<string> SplitTopLevel(string parenthesized) => CSharpGeneratorAV2.SplitArguments(parenthesized);

        /// <summary>"[ a ][ b ]" with each index translated.</summary>
        private string TranslateIndices(string indices)
        {
            if (string.IsNullOrEmpty(indices) || specificGenerator.Expressions == null)
                return indices;

            var result = new StringBuilder();
            int depth = 0, start = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] == '[')
                {
                    if (depth++ == 0)
                        start = i + 1;
                }
                else if (indices[i] == ']' && --depth == 0)
                {
                    result.Append('[').Append(specificGenerator.Expressions.Int(indices.Substring(start, i - start))).Append(']');
                }
            }
            return result.ToString();
        }

        private readonly Dictionary<string, string> _fieldTypes = new Dictionary<string, string>();

        // The names read from the bitstream by some syntax structure: syntax elements, which persist.
        private readonly HashSet<string> _syntaxElements = new HashSet<string>();

        /// <summary>
        /// A variable of the process it is in, in AV2: a lower case name with no underscore (4.9), assigned
        /// rather than read from the bitstream. It is a local, not a field of the context.
        /// </summary>
        private bool IsLocal(AomField field) =>
            specificGenerator.Expressions != null && field.Type == null && !_syntaxElements.Contains(field.Name) &&
            System.Text.RegularExpressions.Regex.IsMatch(field.Name, "^[a-z][A-Za-z0-9]*$");

        private void FindSyntaxElements(IEnumerable<AomCode> code)
        {
            foreach (var c in code)
            {
                if (c is AomField f && f.Type != null)
                    _syntaxElements.Add(f.Name);
                else if (c is AomBlock block)
                    FindSyntaxElements(block.Content);
                else if (c is AomBlockIfThenElse ifThenElse)
                {
                    FindSyntaxElements(((AomBlock)ifThenElse.BlockIf).Content);
                    foreach (var elseIf in ifThenElse.BlockElseIf)
                        FindSyntaxElements(((AomBlock)elseIf).Content);
                    if (ifThenElse.BlockElse != null)
                        FindSyntaxElements(((AomBlock)ifThenElse.BlockElse).Content);
                }
            }
        }

        /// <summary>new AomArray&lt;AomArray&lt;int&gt;&gt;(() =&gt; new AomArray&lt;int&gt;()): an array whose arrays are made as they are reached.</summary>
        private static string NewAomArray(string type)
        {
            string element = type.Substring("AomArray<".Length, type.Length - "AomArray<".Length - 1);
            return element.StartsWith("AomArray<") ? $"new {type}(() => {NewAomArray(element)})" : $"new {type}()";
        }

        private string[] GetMethodParameters(AomMethod aomClass)
        {
            var parameters = aomClass.MethodParameter.Substring(1, aomClass.MethodParameter.Length - 2).Split(',').Select(x => x.Trim()).ToArray();
            if (parameters.Length == 1 && string.IsNullOrEmpty(parameters[0]))
            {
                return new string[] { };
            }
            return parameters;
        }

        private string GenerateFields(AomMethod aomClass)
        {
            string resultCode = "";
            aomClass.FlattenedFields = FlattenFields(aomClass, aomClass.Fields);

            foreach (var field in aomClass.FlattenedFields)
            {
                resultCode += BuildField(aomClass, field);
            }

            return resultCode;
        }

        private List<AomField> FlattenFields(AomMethod b, IEnumerable<AomCode> fields, AomBlock parent = null)
        {
            Dictionary<string, AomField> ret = new Dictionary<string, AomField>();

            if (parent == null)
            {
                // add also ctor params as fields
                string[] ctorParams = GetMethodParameters(b);
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
                        if (p.Type == "while")
                        {
                            field.MakeList = true;
                            Debug.WriteLine($"Field {field.Name} is a list");
                        }
                        p = p.Parent;
                    }

                    // A call - process( a ) - assigns nothing to be kept; its name is a method's
                    bool isCall = field.Type == null && string.IsNullOrWhiteSpace(field.Value) && string.IsNullOrWhiteSpace(field.Increment) && field.Parameter != null;
                    if (!(isCall && specificGenerator.Expressions != null))
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
                                b.RequiresDefinition.Add(new AomField() { Name = variable, Type = "su(32)" });
                            }
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
                else if(code is AomReturn rt && !string.IsNullOrEmpty(rt.Parameter) && (specificGenerator.Expressions == null || !string.IsNullOrWhiteSpace(rt.Parameter)))
                {
                    b.HasReturn = true;
                }
                else if (code is AomTuple tuple)
                {
                    // ( a, b ) = f( ... ): a and b are assigned as any other variable is
                    tuple.Parent = parent;
                    foreach (string target in tuple.Targets)
                    {
                        // a[ i ] assigns an element of a; _ assigns nothing
                        string name = target.Split('[')[0].Trim();
                        if (name != "_")
                            AddAndResolveDuplicates(b, ret, new AomField() { Name = name, ClassType = name, Parent = parent });
                    }
                }
            }

            return ret.Values.ToList();
        }

        private void AddAndResolveDuplicates(AomMethod b, Dictionary<string, AomField> ret, AomField field)
        {
            string name = field.Name;
            if (!ret.TryAdd(name, field))
            {
                if (string.IsNullOrEmpty(field.Type))
                {
                    if (field.Parameter != ret[name].Parameter) // we have to duplicate these
                    {
                        if (field.Name == "read_global_param")
                            return;

                        AddNewDuplicatedField(ret, field, name);
                    }
                    else
                    {
                        // just log a warning for now
                        Debug.WriteLine($"-------Field {field.Name} already exists in {b.MethodName} class, possible issue with the value being overwritten! Type: {field.Type}, Value: {field.Value}");
                    }
                }
                else
                {
                    // just log a warning for now
                    Debug.WriteLine($"-------Field {field.Name} already exists in {b.MethodName} class, possible issue with the value being overwritten! Type: {field.Type}, Value: {field.Value}");
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

        private string BuildRequiredVariables(AomMethod aomClass)
        {
            string resultCode = "";

            foreach (var v in aomClass.RequiresDefinition)
            {
                if (!_fields.Add(v.Name))
                    continue;

                string type = GetCSharpTypeMapping()[v.Type];
                if (string.IsNullOrEmpty(v.FieldArray))
                {
                    string value = "= 0";
                    if (!string.IsNullOrWhiteSpace(v.Value))
                    {
                        value = v.Value;
                    }

                    resultCode += $"\t\tprivate {type} {v.Name} {value};\r\n";
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
                    resultCode += $"\r\n\t\t\t{type}{array} {v.Name} = null;\r\n"; // TODO: size
                }
            }

            return resultCode;
        }

        private string BuildField(AomMethod ituClass, AomField field)
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
            if (string.IsNullOrEmpty(initializer) && type.StartsWith("AomArray<"))
                initializer = $" = {NewAomArray(type)}";

            if (IsLocal(field))
                return "";

            if (_fields.Add(field.Name))
            {
                _fieldTypes[field.Name] = type;
                string ret = $"\t\tprivate {type} {field.Name}{initializer};\r\n";
                if (field.Name.Length > 1)
                {
                    string propertyName = $"_{field.Name.ToPropertyCase()}";
                    while(_fields.Contains(propertyName) || _properties.Contains(propertyName))
                    {
                        propertyName = $"_{propertyName}";
                    }

                    if(!_properties.Add(propertyName))
                        throw new Exception();

                    ret += $"\t\tpublic {type} {propertyName} {{ get {{ return {field.Name}; }} set {{ {field.Name} = value; }} }}\r\n";
                }
                return ret;
            }
            else
            {
                return "";
            }
        }

        private string GetSpacing(int level)
        {
            var ret = new StringBuilder();
            for (int i = 0; i < level; i++)
            {
                ret.Append("\t");
            }
            return ret.ToString();
        }

        private string BuildMethod(AomMethod b, AomBlock parent, AomCode field, int level)
        {
            string spacing = GetSpacing(level);
            var block = field as AomBlock;
            if (block != null)
            {
                return BuildBlock(b, parent, block, level);
            }

            var blockIf = field as AomBlockIfThenElse;
            if (blockIf != null)
            {
                string ret = BuildBlock(b, parent, (AomBlock)blockIf.BlockIf, level);
                foreach (var elseBlock in blockIf.BlockElseIf)
                {
                    ret += BuildBlock(b, parent, (AomBlock)elseBlock, level);
                }
                if (blockIf.BlockElse != null)
                {
                    ret += BuildBlock(b, parent, (AomBlock)blockIf.BlockElse, level);
                }
                return ret;
            }

            var comment = field as AomComment;
            if (comment != null)
            {
                return BuildComment(b, comment, level);
            }

            var retrn = field as AomReturn;
            if (retrn != null)
            {
                return BuildReturn(b, parent, retrn, level);
            }

            var brk = field as AomBreak;
            if (brk != null)
            {
                return $"{GetSpacing(level)}break;";
            }

            if (field is AomTuple tuple)
            {
                // C#'s deconstruction assigns the values in order, as the specification does
                string value = specificGenerator.Expressions != null ? specificGenerator.Expressions.Int(tuple.Value) : tuple.Value;
                var targets = tuple.Targets.Select(t => t.Contains("[") ? t.Substring(0, t.IndexOf('[')).Trim() + TranslateIndices(t.Substring(t.IndexOf('['))) : t);
                return $"{spacing}({string.Join(", ", targets)}) = {value};";
            }

            if ((field as AomField).Type == null && (!string.IsNullOrWhiteSpace((field as AomField).Value) || !string.IsNullOrWhiteSpace((field as AomField).Increment)))
            {
                return BuildStatement(b, parent, field as AomField, level);
            }

            string name = (field as AomField).Name;
            string m = GetReadMethod(b, field as AomField);

            string typedef = TranslateIndices((field as AomField).FieldArray);

            string fieldComment = "";
            if (!string.IsNullOrEmpty((field as AomField)?.Comment))
            {
                fieldComment = "//" + (field as AomField).Comment;
            }

            string retm = "";

            if ((field as AomField).Type != null)
            {
                retm = $"{spacing}{m} out this.{name}{typedef}, \"{name}\"); {fieldComment}";
            }
            else
            {
                retm = $"{spacing}{m}; {fieldComment}";
            }

            retm = specificGenerator.AppendMethod(field, spacing, retm);

            return retm;
        }

        private string BuildReturn(AomMethod b, AomBlock parent, AomReturn retrn, int level)
        {
            var expressions = specificGenerator.Expressions;
            if (expressions != null)
            {
                if (string.IsNullOrWhiteSpace(retrn.Parameter))
                    return $"{GetSpacing(level)}return;";

                var values = SplitTopLevel(retrn.Parameter);
                string returned = retrn.Parameter.Trim().StartsWith("(") && values.Count > 1
                    ? $"({string.Join(", ", values.Select(expressions.Int))})"
                    : expressions.Int(retrn.Parameter);
                return $"{GetSpacing(level)}return {returned};";
            }

            string p = "";
            if (!string.IsNullOrEmpty(retrn.Parameter))
                p = specificGenerator.FixStatement(retrn.Parameter);

            return $"{GetSpacing(level)}return" + p + ";";
        }

        private string BuildStatement(AomMethod b, AomBlock parent, AomField field, int level)
        {
            var expressions = specificGenerator.Expressions;
            if (expressions != null)
            {
                // name[ i ] op= value, or name++
                string assigned = $"{field.Name}{TranslateIndices(field.FieldArray)}";
                if (!string.IsNullOrWhiteSpace(field.Increment))
                    return $"{GetSpacing(level)}{assigned}{field.Increment};";

                string value = field.Value.Trim();
                int operatorEnd = value.IndexOf('=') + 1;
                return $"{GetSpacing(level)}{assigned} {value.Substring(0, operatorEnd)} {expressions.Int(value.Substring(operatorEnd))};";
            }

            string fieldValue = field.Value;
            string fieldArray = field.FieldArray;

            if (!string.IsNullOrEmpty(fieldArray))
                fieldArray = specificGenerator.FixStatement(fieldArray);

            if (!string.IsNullOrEmpty(fieldValue))
            {
                fieldValue = specificGenerator.FixStatement(fieldValue);

                string trimmed = fieldValue.TrimStart(new char[] { ' ', '=' });
                if (trimmed.StartsWith("!"))
                {
                    fieldValue = $"= {trimmed.Substring(1)} == 0";
                }

                if (fieldValue.Contains("flag") && !fieldValue.Contains(")"))
                    fieldValue = fieldValue.Replace("||", "|").Replace("&&", "&");
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
                    return $"{GetSpacing(level)}int {field.Name}{fieldArray}{fieldValue}".TrimEnd() + ";";
                }
                else
                {
                    return $"{GetSpacing(level)}{field.Name}{fieldArray}{field.Increment}{fieldValue}".TrimEnd() + ";";
                }
            }
        }

        private string BuildComment(AomMethod b, AomComment comment, int level)
        {
            return $"/* {comment.Comment} */\r\n";
        }

        private string BuildBlock(AomMethod b, AomBlock parent, AomBlock block, int level)
        {
            string spacing = GetSpacing(level);
            string ret = "";

            string condition = block.Condition;
            string blockType = block.Type;

            if (specificGenerator.Expressions != null)
            {
                if (!string.IsNullOrEmpty(condition))
                    condition = TranslateCondition(blockType, condition);
            }
            else if (!string.IsNullOrEmpty(condition))
            {
                if (blockType == "if" || blockType == "else if" || blockType == "while")
                {
                    condition = FixCondition(b, condition);
                }
                else if (blockType == "for")
                {
                    condition = specificGenerator.FixCondition(condition);
                }
            }

            if (!string.IsNullOrEmpty(condition) && specificGenerator.Expressions == null)
            {
                condition = condition.Replace("<<", "<< (int)");
            }

            ret += $"\r\n{spacing}{blockType} {condition}\r\n{spacing}{{";

            foreach (var field in block.Content)
            {
                ret += "\r\n" + BuildMethod(b, block, field, level + 1);
            }

            ret += $"\r\n{spacing}}}";

            return ret;
        }

        /// <summary>
        /// "( a && !b )" as a C# condition; a for's "( i = 0; i < n; i++ )" part by part, as a value, a
        /// condition and a step.
        /// </summary>
        private string TranslateCondition(string blockType, string condition)
        {
            var expressions = specificGenerator.Expressions;
            string inner = condition.Trim();
            inner = inner.Substring(1, inner.Length - 2);

            if (blockType != "for")
                return $"({expressions.Bool(inner)})";

            string[] parts = inner.Split(';');
            string Assignments(string part) => string.Join(", ", part.Split(',').Where(p => p.Trim().Length > 0).Select(p =>
            {
                string s = p.Trim();
                if (s.EndsWith("++") || s.EndsWith("--"))
                    return s;
                int op = s.IndexOf('=');
                // i = 0, i += 2
                string target = s.Substring(0, op).TrimEnd('+', '-', '*', '/', '<', '>', '|', '&', '^').Trim();
                string opText = s.Substring(target.Length, op + 1 - target.Length).Trim();
                return $"{target} {opText} {expressions.Int(s.Substring(op + 1))}";
            }));

            return $"({Assignments(parts[0])}; {expressions.Bool(parts[1])}; {Assignments(parts[2])})";
        }

        private string FixCondition(AomMethod b, string condition)
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

            condition = specificGenerator.FixCondition(condition);

            return condition;
        }

        private string GetCSharpType(AomField field)
        {
            string custom = specificGenerator.GetFieldType(field);
            if (custom != null)
                return custom;

            Dictionary<string, string> map = GetCSharpTypeMapping();

            if (string.IsNullOrWhiteSpace(field.Type))
            {
                if (
                    field.ClassType == "operating_point_idc" ||
                    field.ClassType == "decoder_model_present_for_this_op" ||
                    field.ClassType == "initial_display_delay_present_for_this_op" ||
                    field.ClassType == "decoder_model_present_for_this_op" ||
                    field.ClassType == "seq_tier" ||
                    field.ClassType == "cdef_y_pri_strength" ||
                    field.ClassType == "cdef_y_sec_strength" ||
                    field.ClassType == "cdef_uv_pri_strength" ||
                    field.ClassType == "cdef_uv_sec_strength" ||
                    field.ClassType == "cdef_damping_minus_3" ||
                    field.ClassType == "loop_filter_level" ||
                    field.ClassType == "loop_filter_ref_deltas" ||
                    field.ClassType == "GmType" ||
                    field.ClassType == "tile_size_minus_1" ||
                    field.ClassType == "initial_display_delay_minus_1" ||
                    field.ClassType == "RefValid" ||
                    field.ClassType == "RefOrderHint" ||
                    field.ClassType == "OrderHints" ||
                    field.ClassType == "ref_order_hint" ||
                    field.ClassType == "expectedFrameId" ||
                    field.ClassType == "RefFrameSignBias" ||
                    field.ClassType == "LoopRestorationSize" ||
                    field.ClassType == "FrameRestorationType" ||
                    field.ClassType == "MiColStarts" ||
                    field.ClassType == "MiRowStarts" ||
                    field.ClassType == "loop_filter_mode_deltas" ||
                    field.ClassType == "LosslessArray" ||
                    field.ClassType == "SkipModeFrame" ||
                    field.ClassType == "usedFrame" ||
                    field.ClassType == "shiftedOrderHints"
                    )
                {
                    return map["su(32)[]"];
                }
                else if(
                    field.ClassType == "FeatureEnabled" ||
                    field.ClassType == "FeatureData" ||
                    field.ClassType == "gm_params" ||
                    field.ClassType == "SegQMLevel" 
                    )
                {
                    return map["su(32)[][]"];
                }

                return map["su(32)"];
            }

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
                { "f(1)",                       "int" },
                { "f(2)",                       "int" },
                { "f(3)",                       "int" },
                { "f(4)",                       "int" },
                { "f(5)",                       "int" },
                { "f(6)",                       "int" },
                { "f(8)",                       "int" },
                { "f(9)",                       "int" },
                { "f(12)",                      "int" },
                { "f(16)",                      "int" },
                { "f(32)",                      "int" },
                { "f(32)[]",                    "int[]" },
                { "f(b2)",                      "int" },
                { "f(bitsToRead)",              "int" },
                { "f(idLen)",                   "int" },
                { "f(N)",                       "byte[]" },
                { "f(n)",                       "int" },
                { "f(OrderHintBits)",           "int" },
                { "f(SUPERRES_DENOM_BITS)",     "int" },
                { "f(tileBits)",                "int" },
                { "f(TileRowsLog2+TileColsLog2)", "int" },
                { "f(time_offset_length)",      "int" },
                { "L(1)",                       "int" },
                { "L(2)",                       "int" },
                { "L(3)",                       "int" },
                { "L(b2)",                      "int" },
                { "L(BitDepth)",                "int" },
                { "L(cdef_bits)",               "int" },
                { "L(delta_q_rem_bits)",        "int" },
                { "L(n)",                       "int" },
                { "L(paletteBits)",             "int" },
                { "L(SGRPROJ_PARAMS_BITS)",     "int" },
                { "le(TileSizeBytes)",          "int" },
                { "leb128()",                   "int" },
                { "uint(32)",                   "uint" },
                { "ns(maxWidth)",               "uint" },
                { "ns(maxHeight)",              "uint" },
                { "ns(numSyms - mk)",           "uint" },
                { "NS(numSyms - mk)",           "uint" },
                { "NS(PaletteSizeUV)",          "uint" },
                { "S()",                        "uint" },
                { "su(32)",                     "int" },
                { "su(32)[]",                   "int[]" },
                { "su(32)[][]",                 "int[][]" },
                { "su(64)",                     "long" },
                { "su(1+6)",                    "int" },
                { "su(1+bitsToRead)",           "int" },
                { "uvlc()",                     "uint" },
            };
            return map;
        }

        private string GetReadMethod(AomMethod b, AomField aomField)
        {
            string custom = specificGenerator.GetReadMethod(aomField);
            if (custom != null)
                return custom;

            switch (aomField.Type)
            {
                case "f(1)":
                    return "stream.ReadFixed(1,";
                
                case "f(2)":
                    return "stream.ReadFixed(2,";
                
                case "f(3)":
                    return "stream.ReadFixed(3,";

                case "f(4)":
                    return "stream.ReadFixed(4,";

                case "f(5)":
                    return "stream.ReadFixed(5,";

                case "f(6)":
                    return "stream.ReadFixed(6,";

                case "f(8)":
                    return "stream.ReadFixed(8,";

                case "f(9)":
                    return "stream.ReadFixed(9,";

                case "f(12)":
                    return "stream.ReadFixed(12,";

                case "f(16)":
                    return "stream.ReadFixed(16,";

                case "f(32)":
                    return "stream.ReadFixed(32,";

                case "f(b2)":
                    return "stream.ReadVariable(b2,";

                case "f(bitsToRead)":
                    return "stream.ReadVariable(bitsToRead,";

                case "f(idLen)":
                    return "stream.ReadVariable(idLen,";

                case "f(N)":
                    return "stream.ReadBytes(N,";

                case "f(n)":
                    return "stream.ReadVariable(n,";

                case "f(OrderHintBits)":
                    return "stream.ReadVariable(OrderHintBits,";

                case "f(SUPERRES_DENOM_BITS)":
                    return "stream.ReadVariable(AV1Constants.SUPERRES_DENOM_BITS,";

                case "f(tileBits)":
                    return "stream.ReadVariable(tileBits,";

                case "f(TileRowsLog2+TileColsLog2)":
                    return "stream.ReadVariable(TileRowsLog2+TileColsLog2,";

                case "f(time_offset_length)":
                    return "stream.ReadVariable(time_offset_length,";

                case "L(1)":
                    return "stream.ReadL(1,";

                case "L(2)":
                    return "stream.ReadL(2,";

                case "L(3)":
                    return "stream.ReadL(3,";

                case "L(b2)":
                    return "stream.ReadL(b2,";

                case "L(BitDepth)":
                    return "stream.ReadL(BitDepth,";

                case "L(cdef_bits)":
                    return "stream.ReadL(cdef_bits,";

                case "L(delta_q_rem_bits)":
                    return "stream.ReadL(delta_q_rem_bits,";

                case "L(n)":
                    return "stream.ReadL(n,";

                case "L(paletteBits)":
                    return "stream.ReadL(paletteBits,";

                case "L(SGRPROJ_PARAMS_BITS)":
                    return "stream.ReadL(SGRPROJ_PARAMS_BITS,";

                case "le(TileSizeBytes)":
                    return "stream.ReadLeVar(TileSizeBytes,";

                case "leb128()":
                    // Only obu_size's length is kept - it says where the payload starts. Every
                    // leb128 used to set it, so a metadata_type after it made the OBU header out
                    // by the difference in their lengths.
                    return aomField.Name == "obu_size" ? "obu_size_len = (int)stream.ReadLeb128(" : "stream.ReadLeb128(";

                case "uint(32)":
                    return "stream.ReadUnsignedInt32(size,";

                case "ns(maxHeight)":
                    return "stream.Read_ns(maxHeight,";

                case "ns(maxWidth)":
                    return "stream.Read_ns(maxWidth,";

                case "ns(numSyms - mk)":
                    return "stream.Read_ns(numSyms - mk,";

                case "NS(numSyms - mk)":
                    return "stream.Read_NS(numSyms - mk,";

                case "NS(PaletteSizeUV)":
                    return "stream.Read_NS(PaletteSizeUV,";

                case "S()":
                    return "stream.ReadS(size,";

                case "su(32)":
                    return "stream.ReadSignedInt32(size,";

                case "su(32)[]":
                    return "stream.ReadSignedInt32(size,";

                case "su(32)[][]":
                    return "stream.ReadSignedInt32(size,";

                case "su(1+6)":
                    return "stream.ReadSignedIntVar(1+6,";

                case "su(1+bitsToRead)":
                    return "stream.ReadSignedIntVar(1+bitsToRead,";

                case "uvlc()":
                    return "stream.ReadUvlc(";

                default:
                    if (aomField.Type == null)
                    {
                        return $"{aomField.ClassType.ToPropertyCase()}{aomField.Parameter}";
                    }
                    throw new NotImplementedException();
            }
        }
    }
}
