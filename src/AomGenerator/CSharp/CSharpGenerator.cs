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

        /// <summary>The descriptor a structure's parameter is taken to have: su(32), or su(64) for a count of bits.</summary>
        string GetCtorParameterType(string parameter);

        /// <summary>Translates the specification's expressions.</summary>
        AomExpressions Expressions { get; }

        /// <summary>The C# type of a field.</summary>
        string GetFieldType(AomField field);

        /// <summary>The start of the call that reads a field.</summary>
        string GetReadMethod(AomField field);

        /// <summary>
        /// Syntax structures whose fields - those they assign, and those of the structures they call -
        /// are listed in the generated SyntaxFields, for the processes that save and load "all the syntax
        /// elements read in" one (save_sequence_header and the like).
        /// </summary>
        IEnumerable<string> FieldSets { get; }

        /// <summary>Fields the field sets leave out: state that is not kept with the rest, as VP9's probability tables.</summary>
        ISet<string> UnsavedFields { get; }

        /// <summary>Called with every syntax structure before any is generated, to learn what it needs of them.</summary>
        void Prepare(IEnumerable<AomMethod> methods);

        /// <summary>The C# type of a structure's parameter; null, the generator's own.</summary>
        string GetParameterType(AomMethod method, string parameter);

        /// <summary>The C# type a structure returns, if it returns a value; null, the generator's own.</summary>
        string GetReturnType(AomMethod method);

        /// <summary>
        /// What an encoder writes for an element, from a state - {state} - where the statements after its
        /// read do not show it: a choice the state makes (found_ref), a table the value was looked up in.
        /// Null, the generator's own.
        /// </summary>
        string GetInverse(AomField element);
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
{string.Concat((specificGenerator as CSharpGeneratorAom)?.StaticUsings.Select(u => $"using static {u};\r\n") ?? Enumerable.Empty<string>())}
namespace Sharp{type}
{{
";
            resultCode += @$"
    public partial class {type}Context : IAomContext
    {{";

            specificGenerator.Prepare(aomClasses);
            foreach (var aomClass in aomClasses)
                FindSyntaxElements(aomClass.Fields);
            FindReadingStructures(aomClasses);
            specificGenerator.Expressions.ReadingStructures = _readingStructures;

            resultCode += $@"
        /// <summary>
        /// Writing, the state the OBU's syntax elements were read into, and the same state as it was
        /// changed: an element whose value the two give alike is written as it was read.
        /// </summary>
        private {type}Context _original;
        private {type}Context _edited;
";

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

        // The structures that read the bitstream, themselves or through a structure they call: each has a Write method.
        private readonly HashSet<string> _readingStructures = new HashSet<string>();

        // True while a Write method is generated.
        private bool _writing;

        private void FindReadingStructures(IEnumerable<AomMethod> aomClasses)
        {
            var names = new HashSet<string>(aomClasses.Select(c => c.MethodName));
            var calls = new Dictionary<string, HashSet<string>>();
            foreach (var aomClass in aomClasses)
            {
                if (HasDescriptor(aomClass.Fields))
                    _readingStructures.Add(aomClass.MethodName);
                // Its calls, wherever they are: statements, conditions, arguments and values
                var called = new HashSet<string>();
                foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(aomClass.Syntax, @"\b([A-Za-z_]\w*)\s*\("))
                {
                    string name = m.Groups[1].Value;
                    if (name != aomClass.MethodName && names.Contains(name))
                        called.Add(name);
                }
                calls[aomClass.MethodName] = called;
            }

            bool changed = true;
            while (changed)
            {
                changed = false;
                foreach (var entry in calls)
                {
                    if (!_readingStructures.Contains(entry.Key) && entry.Value.Any(_readingStructures.Contains))
                        changed |= _readingStructures.Add(entry.Key);
                }
            }
        }

        private static bool HasDescriptor(IEnumerable<AomCode> code)
        {
            foreach (var c in code)
            {
                if (c is AomField f && f.Type != null)
                    return true;
                if (c is AomBlock block && HasDescriptor(block.Content))
                    return true;
                if (c is AomBlockIfThenElse ifThenElse)
                {
                    if (HasDescriptor(((AomBlock)ifThenElse.BlockIf).Content) ||
                        ifThenElse.BlockElseIf.Any(e => HasDescriptor(((AomBlock)e).Content)) ||
                        ifThenElse.BlockElse != null && HasDescriptor(((AomBlock)ifThenElse.BlockElse).Content))
                        return true;
                }
            }
            return false;
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
                fields.ExceptWith(specificGenerator.UnsavedFields);

                string name2 = all ? "Context" : structure.ToPropertyCase();
                string copy(string value, string type) =>
                    type.StartsWith("AomArray<") ? $"{value}?.Clone()" : type.EndsWith("[]") ? ArrayCopy(value, type, 0) : value;

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

                // The whole context may take a state just saved as it is, rather than copy it again: a copy of
                // the context is a state saved and loaded.
                string loadParameters = all ? $"{name2}State state, bool copy = true" : $"{name2}State state";
                result.Append($"\r\n\t\tprivate void Load{name2}({loadParameters})\r\n\t\t{{\r\n");
                foreach (string field in fields)
                {
                    string copied = copy("state." + field, _fieldTypes[field]);
                    result.Append(all && copied != "state." + field
                        ? $"\t\t\tthis.{field} = copy ? {copied} : state.{field};\r\n"
                        : $"\t\t\tthis.{field} = {copied};\r\n");
                }
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

        /// <summary>A copy of an array, and of the arrays in it: int[][] element by element.</summary>
        private static string ArrayCopy(string value, string type, int depth)
        {
            string element = type.Substring(0, type.Length - 2);
            return element.EndsWith("[]")
                ? $"({value} == null ? null : Array.ConvertAll({value}, e{depth} => {ArrayCopy("e" + depth, element, depth + 1)}))"
                : $"(({type}){value}?.Clone())";
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

            // Read, and written as it is read: the same statements, each element written where it is read.
            var addedFields = aomClass.AddedFields.ToList();
            foreach (bool writing in _readingStructures.Contains(aomClass.MethodName) ? new[] { false, true } : new[] { false })
            {
                _writing = writing;
                specificGenerator.Expressions.Writing = writing;
                // The variables a method declares where it first assigns them, declared again in the other
                var declared = aomClass.AddedFields.ToList();
                aomClass.AddedFields.Clear();
                aomClass.AddedFields.AddRange(addedFields);

                string methodName = (writing ? "Write" : "") + aomClass.MethodName.ToPropertyCase();
                resultCode += $@"
        private {retType} {methodName}({ituClassParameters})
        {{";
                resultCode += GenerateMethodBody(aomClass, ctorParameters);
                resultCode += $@"
        }}
";
                foreach (var field in declared.Where(f => !aomClass.AddedFields.Contains(f)))
                    aomClass.AddedFields.Add(field);
            }
            _writing = false;
            specificGenerator.Expressions.Writing = false;

            return resultCode;
        }

        private string GenerateMethodBody(AomMethod aomClass, string[] ctorParameters)
        {
            string resultCode = "";

            // A process's lower case variables are its own (4.8 in AV1's specification, 4.9 in AV2's): its
            // loop counters are locals, or a function called in a loop over i would move the caller's i, and
            // so is every such variable - startBitPos of AV2's tile_group_obu is not frame_header's.
            foreach (var counter in aomClass.RequiresDefinition.Where(v => !ctorParameters.Contains(v.Name)).Select(v => v.Name).Distinct())
                resultCode += $"\r\n\t\t\tint {counter} = 0;";

            var counters = new HashSet<string>(aomClass.RequiresDefinition.Select(v => v.Name));
            foreach (var local in aomClass.FlattenedFields.Where(f => IsLocal(f) && !ctorParameters.Contains(f.Name) && !counters.Contains(f.Name)))
            {
                string type = GetCSharpType(local);
                string value = type.StartsWith("AomArray<") ? NewAomArray(type) : type == "byte[]" ? "null" : "0";
                resultCode += $"\r\n\t\t\t{type} {local.Name} = {value};";
            }
            foreach (var field in aomClass.Fields)
            {
                resultCode += "\r\n" + BuildMethod(aomClass, null, field, 3);
            }
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
        private static List<string> SplitTopLevel(string parenthesized) => CSharpGeneratorAom.SplitArguments(parenthesized);

        /// <summary>"[ a ][ b ]" with each index translated.</summary>
        private string TranslateIndices(string indices)
        {
            if (string.IsNullOrEmpty(indices))
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
        /// A variable of the process it is in: a lower case name with no underscore (4.8 in AV1's
        /// specification, 4.9 in AV2's), assigned rather than read from the bitstream. It is a local, not a
        /// field of the context.
        /// </summary>
        private bool IsLocal(AomField field) =>
            field.Type == null && !_syntaxElements.Contains(field.Name) &&
            System.Text.RegularExpressions.Regex.IsMatch(field.Name, "^[a-z][A-Za-z0-9]*$") &&
            (specificGenerator as CSharpGeneratorAom)?.KeptVariables.Contains(field.Name) != true;

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
                    if (!isCall)
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
                else if (code is AomReturn rt && !string.IsNullOrWhiteSpace(rt.Parameter))
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
                    // a scalar is a local of each process that uses it (GenerateMethodBody), not a field
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

            string initializer = type.StartsWith("AomArray<") ? $" = {NewAomArray(type)}" : "";

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
                string value = specificGenerator.Expressions.Int(tuple.Value);
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

            if ((field as AomField).Type != null && _writing)
            {
                // The value of this occurrence, then written as it would be read
                string target = $"this.{name}{typedef}";
                var siblings = (parent != null ? parent.Content : b.Fields).ToList();
                string Of(string state) => Inverse(siblings, siblings.IndexOf(field), (AomField)field, state) ?? $"{state}.{name}{typedef}";
                string original = $"_original != null ? {Of("_original")} : {target}";
                string edited = $"_edited != null ? {Of("_edited")} : _original != null ? {Of("_original")} : {target}";
                retm = $"{spacing}{target} = stream.Pick(\"{name}\", {original}, {edited});\r\n{spacing}{WriteMethod(m)} {target}, \"{name}\"); {fieldComment}";
            }
            else if ((field as AomField).Type != null)
            {
                retm = $"{spacing}{m} out this.{name}{typedef}, \"{name}\"); {fieldComment}";
            }
            else
            {
                // A structure: written, one that reads is its Write method, as the expressions name it
                retm = $"{spacing}{m}; {fieldComment}";
            }

            return retm;
        }

        private string BuildReturn(AomMethod b, AomBlock parent, AomReturn retrn, int level)
        {
            var expressions = specificGenerator.Expressions;
            if (string.IsNullOrWhiteSpace(retrn.Parameter))
                return $"{GetSpacing(level)}return;";

            var values = SplitTopLevel(retrn.Parameter);
            string returned = retrn.Parameter.Trim().StartsWith("(") && values.Count > 1
                ? $"({string.Join(", ", values.Select(expressions.Int))})"
                : expressions.Int(retrn.Parameter);
            return $"{GetSpacing(level)}return {returned};";
        }

        private string BuildStatement(AomMethod b, AomBlock parent, AomField field, int level)
        {
            // name[ i ] op= value, or name++
            string assigned = $"{field.Name}{TranslateIndices(field.FieldArray)}";
            if (!string.IsNullOrWhiteSpace(field.Increment))
                return $"{GetSpacing(level)}{assigned}{field.Increment};";

            string value = field.Value.Trim();
            int operatorEnd = value.IndexOf('=') + 1;
            return $"{GetSpacing(level)}{assigned} {value.Substring(0, operatorEnd)} {specificGenerator.Expressions.Int(value.Substring(operatorEnd))};";
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

            if (!string.IsNullOrEmpty(condition))
                condition = TranslateCondition(blockType, condition);

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

        private string GetCSharpType(AomField field) => specificGenerator.GetFieldType(field);

        /// <summary>The types of the descriptors the generator gives structure parameters and loop counters.</summary>
        private static Dictionary<string, string> GetCSharpTypeMapping() => new Dictionary<string, string>
        {
            { "su(32)", "int" },
            { "su(64)", "long" },
        };

        /// <summary>
        /// The value an element read at siblings[ index ] had, as the statements after it leave it in a
        /// state - the state the OBU was read into, or that state changed - where they change it, or keep it
        /// elsewhere: what an encoder writes from its state. Null where the element is kept as it is read.
        /// </summary>
        private string Inverse(IList<AomCode> siblings, int index, AomField element, string state)
        {
            var expressions = specificGenerator.Expressions;
            if (index < 0)
                return null;

            string custom = specificGenerator.GetInverse(element);
            if (custom != null)
                return custom.Replace("{state}", state);

            string Normalized(string text) => System.Text.RegularExpressions.Regex.Replace(text ?? "", @"\s+", "");
            string indices = element.FieldArray ?? "";
            string self = Normalized(element.Name + indices);
            string translated = TranslateIndices(indices);
            string kept = $"{state}.{element.Name}{translated}";

            for (int i = index + 1; i < siblings.Count; i++)
            {
                var next = siblings[i];

                // Only the first statement after it that uses it says what became of it: past one that is
                // none of these, what a later one says is of the value as that one left it, or of another
                // case - reduced_still_picture_header sets nine elements, then frame_id_numbers_present_flag.
                if (!Mentions(next, element.Name))
                    continue;

                // x op= E: x as read is x as kept, E undone (E is the writer's, where the reader's was)
                if (next is AomField statement && statement.Type == null && Normalized(statement.Name + statement.FieldArray) == self && statement.Value != null)
                {
                    string value = statement.Value.Trim();
                    if (value.StartsWith("+=") || value.StartsWith("-="))
                    {
                        string e = expressions.Int(value.Substring(2));
                        return value.StartsWith("+=") ? $"({kept} - ({e}))" : $"({kept} + ({e}))";
                    }
                    return null;
                }

                // T = Table[ x ]: x is where T is in the table - a constant table, named as the specification
                // names them (Remap_Lr_Type), not a variable's values (RefFrameType[ x ] is x's frame's type)
                var lookup = next is AomField l && l.Type == null && l.Value != null
                    ? System.Text.RegularExpressions.Regex.Match(Normalized(l.Value), @"^=([A-Z][a-z0-9]*(?:_[A-Za-z0-9]+)+)\[" + System.Text.RegularExpressions.Regex.Escape(self) + @"\]$") : null;
                if (lookup != null && lookup.Success && _fieldTypes.ContainsKey(((AomField)next).Name))
                {
                    var table = (AomField)next;
                    return $"AomArray.IndexOf({lookup.Groups[1].Value}, {state}.{table.Name}{TranslateIndices(table.FieldArray)})";
                }

                // T = x: x is kept as T, where T is kept in the state
                if (next is AomField copy && copy.Type == null && copy.Value != null && Normalized(copy.Value) == "=" + self
                    && copy.Name != element.Name && _fieldTypes.ContainsKey(copy.Name) && !IsLocal(copy))
                    return $"{state}.{copy.Name}{TranslateIndices(copy.FieldArray)}";

                if (next is AomBlockIfThenElse choice)
                {
                    var then = (AomBlock)choice.BlockIf;
                    string condition = Normalized(then.Condition);
                    var thenContent = then.Content.ToList();
                    bool onlyIf = !choice.BlockElseIf.Any();

                    // if ( x == C ) { x += D }: C + D kept is C read
                    var m = System.Text.RegularExpressions.Regex.Match(condition, @"^\(" + System.Text.RegularExpressions.Regex.Escape(self) + @"==(\d+)\)$");
                    if (m.Success && onlyIf && choice.BlockElse == null && thenContent.Count == 1
                        && thenContent[0] is AomField increment && Normalized(increment.Name + increment.FieldArray) == self
                        && increment.Value != null && System.Text.RegularExpressions.Regex.IsMatch(Normalized(increment.Value), @"^\+=\d+$"))
                    {
                        int c = int.Parse(m.Groups[1].Value);
                        int d = int.Parse(Normalized(increment.Value).Substring(2));
                        return $"({kept} == {c + d} ? {c} : {kept})";
                    }

                    // if ( update_x == 1 ) T f(n): x says T is coded, which an encoder does where T changes - where
                    // it is kept other than the writer has it before it is written. Only for the flags the
                    // specification names update_: a flag that says whether T is present at all (hours_flag) does
                    // not say T changed.
                    if (element.Name.StartsWith("update_") && (condition == $"({self})" || condition == $"({self}==1)") && onlyIf && choice.BlockElse == null && thenContent.Count == 1
                        && thenContent[0] is AomField updated && updated.Type != null && updated.Name != element.Name && _fieldTypes.ContainsKey(updated.Name))
                    {
                        string indices2 = TranslateIndices(updated.FieldArray);
                        return $"({state}.{updated.Name}{indices2} != {updated.Name}{indices2} ? 1 : 0)";
                    }

                    // if ( condition ) { x op= E }, the condition not of x: undone where the condition holds
                    if (onlyIf && choice.BlockElse == null && thenContent.Count == 1 && !condition.Contains(self)
                        && thenContent[0] is AomField change && change.Type == null && Normalized(change.Name + change.FieldArray) == self
                        && change.Value != null && (change.Value.Trim().StartsWith("+=") || change.Value.Trim().StartsWith("-=")))
                    {
                        string value = change.Value.Trim();
                        string e = expressions.Int(value.Substring(2));
                        string undone = value.StartsWith("+=") ? $"{kept} - ({e})" : $"{kept} + ({e})";
                        return $"({expressions.Bool(then.Condition.Trim().Substring(1, then.Condition.Trim().Length - 2))} ? {undone} : {kept})";
                    }

                    // if ( x ) { T = K } else { T f(n) }: x is whether T is K
                    if (condition == $"({self})" && onlyIf && choice.BlockElse != null && thenContent.Count == 1
                        && thenContent[0] is AomField preset && preset.Type == null && preset.Value != null
                        && System.Text.RegularExpressions.Regex.IsMatch(Normalized(preset.Value), @"^=-?\w+$")
                        && ((AomBlock)choice.BlockElse).Content.FirstOrDefault() is AomField coded && coded.Type != null
                        && Normalized(coded.Name + coded.FieldArray) == Normalized(preset.Name + preset.FieldArray) && !IsLocal(preset))
                    {
                        string k = expressions.Int(Normalized(preset.Value).Substring(1));
                        return $"({state}.{preset.Name}{TranslateIndices(preset.FieldArray)} == {k} ? 1 : 0)";
                    }
                }

                return null;
            }
            return null;
        }

        /// <summary>Whether a statement, or a block and all in it, names a variable.</summary>
        private static bool Mentions(AomCode code, string name)
        {
            bool In(string text) => text != null && System.Text.RegularExpressions.Regex.IsMatch(text, $@"\b{System.Text.RegularExpressions.Regex.Escape(name)}\b");
            switch (code)
            {
                case AomField f:
                    return f.Name == name || In(f.FieldArray) || In(f.Value) || In(f.Parameter) || In(f.Type);
                case AomTuple t:
                    return t.Targets.Any(In) || In(t.Value);
                case AomReturn r:
                    return In(r.Parameter);
                case AomBlock b:
                    return In(b.Condition) || b.Content.Any(c => Mentions(c, name));
                case AomBlockIfThenElse ite:
                    return Mentions((AomBlock)ite.BlockIf, name) || ite.BlockElseIf.Any(e => Mentions((AomBlock)e, name))
                        || ite.BlockElse != null && Mentions((AomBlock)ite.BlockElse, name);
                default:
                    return false;
            }
        }

        /// <summary>The start of the call that writes what the read method's start reads: stream.ReadFixed(3, as stream.WriteFixed(3,.</summary>
        private static string WriteMethod(string readMethod) =>
            readMethod.Replace("stream.Read", "stream.Write").Replace("ReadArithmetic(", "WriteArithmetic(");

        private string GetReadMethod(AomMethod b, AomField aomField) => specificGenerator.GetReadMethod(aomField);
    }
}
