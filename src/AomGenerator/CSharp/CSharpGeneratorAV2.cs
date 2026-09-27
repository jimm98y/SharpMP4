using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AomGenerator.CSharp
{
    /// <summary>
    /// AV2 (AV2 Bitstream &amp; Decoding Process Specification, AOMedia). Its expressions are translated
    /// by <see cref="AomExpressions"/>; its arrays grow to what the stream needs (AomArray), their
    /// dimensions found by how the specification indexes them; its constants, tables and functions keep
    /// the specification's names, in the hand-written part of AV2Context.
    /// </summary>
    public class CSharpGeneratorAV2 : ICustomGenerator
    {
        // C#'s keywords that the specification uses as names.
        private static readonly string[] Keywords = { "ref", "out", "in", "base", "params", "operator", "event", "lock", "fixed", "checked", "default", "object", "string", "is", "as", "new", "class", "char", "byte", "long", "short", "float", "double", "decimal" };

        private readonly Dictionary<string, int> _dimensions = new Dictionary<string, int>();

        public AomExpressions Expressions { get; private set; }

        /// <summary>
        /// What save_sequence_header, save_grain_params and save_grain_model keep: "all the syntax
        /// elements read in" these (6.4.1, 7.23, 6.13); and, as "*", the whole context, which
        /// save_context keeps for an extended layer (7.6).
        /// </summary>
        public IEnumerable<string> FieldSets => new[] { "sequence_header_obu", "film_grain_model", "film_grain_config", "*" };

        public string PreprocessDefinitionsFile(string definitions)
        {
            foreach (string keyword in Keywords)
                definitions = Regex.Replace(definitions, $@"\b{keyword}\b", keyword + "c");

            var structures = new HashSet<string>(
                // Only where a syntax structure is defined, name( ... ) {, not where one is called.
                Regex.Matches(definitions, @"^([A-Za-z_]\w*)\s*\([^)\n]*\)\s*\{", RegexOptions.Multiline).Cast<Match>().Select(m => m.Groups[1].Value)
                    .Where(name => name != "if" && name != "for" && name != "while"));
            Expressions = new AomExpressions(structures);

            FindDimensions(definitions);

            // Tables whose rows the syntax takes whole: their dimensions are all_tables.h's, not the syntax's.
            foreach (var table in new Dictionary<string, int> { { "Pc_Wiener_Sub_Classify2", 3 } })
                _dimensions[table.Key] = table.Value;
            return definitions;
        }

        /// <summary>How many indices each name is used with, at most: a[ i ][ j ] is two.</summary>
        private void FindDimensions(string text)
        {
            foreach (Match m in Regex.Matches(text, @"[A-Za-z_]\w*"))
            {
                int i = m.Index + m.Length;
                int dimensions = 0;
                while (true)
                {
                    int j = i;
                    while (j < text.Length && char.IsWhiteSpace(text[j]))
                        j++;
                    if (j >= text.Length || text[j] != '[')
                        break;

                    int depth = 0;
                    for (; j < text.Length; j++)
                    {
                        if (text[j] == '[') depth++;
                        else if (text[j] == ']' && --depth == 0) break;
                    }
                    dimensions++;
                    i = j + 1;
                }

                if (dimensions > 0 && (!_dimensions.TryGetValue(m.Value, out int known) || known < dimensions))
                    _dimensions[m.Value] = dimensions;
            }
        }

        #region Arrays through calls

        // What each structure returns: the dimensions of each value, one for a single value.
        private readonly Dictionary<string, List<int>> _returns = new Dictionary<string, List<int>>();
        private readonly Dictionary<string, AomMethod> _methods = new Dictionary<string, AomMethod>();

        /// <summary>
        /// The dimensions the indices do not show: a parameter has those of what is passed to it, a variable
        /// those of the value assigned to it - a function's, an array's, a table's row. Repeated until they
        /// settle, as one found can give another.
        /// </summary>
        public void Prepare(IEnumerable<AomMethod> methods)
        {
            foreach (var m in methods)
                _methods[m.MethodName] = m;

            bool changed = true;
            for (int round = 0; changed && round < 50; round++)
            {
                changed = false;
                foreach (var m in methods)
                {
                    var returned = Returns(m.Fields).Select(r => SplitArguments(r).Select(Dimensions).ToList())
                        .OrderByDescending(r => r.Count).ThenByDescending(r => r.Sum()).FirstOrDefault();
                    if (returned != null && (!_returns.TryGetValue(m.MethodName, out var known) || known.Count != returned.Count || known.Zip(returned, (a, b) => b > a).Any(x => x)))
                    {
                        _returns[m.MethodName] = known == null || known.Count != returned.Count ? returned : known.Zip(returned, Math.Max).ToList();
                        changed = true;
                    }
                    changed |= Propagate(m.Fields);
                }
            }
        }

        private static IEnumerable<string> Returns(IEnumerable<AomCode> code)
        {
            foreach (var c in Flatten(code))
            {
                if (c is AomReturn r && !string.IsNullOrWhiteSpace(r.Parameter))
                    yield return r.Parameter.Trim().StartsWith("(") && SplitArguments(r.Parameter).Count > 1 ? r.Parameter : "(" + r.Parameter + ")";
            }
        }

        private static IEnumerable<AomCode> Flatten(IEnumerable<AomCode> code)
        {
            foreach (var c in code)
            {
                yield return c;
                IEnumerable<AomCode> inner = null;
                if (c is AomBlock block)
                    inner = block.Content;
                else if (c is AomBlockIfThenElse ifThenElse)
                {
                    inner = ((AomBlock)ifThenElse.BlockIf).Content.Concat(ifThenElse.BlockElseIf.SelectMany(e => ((AomBlock)e).Content));
                    if (ifThenElse.BlockElse != null)
                        inner = inner.Concat(((AomBlock)ifThenElse.BlockElse).Content);
                }
                if (inner != null)
                    foreach (var i in Flatten(inner))
                        yield return i;
            }
        }

        private bool Raise(string name, int dimensions)
        {
            if (dimensions <= 0 || string.IsNullOrEmpty(name))
                return false;
            if (_dimensions.TryGetValue(name, out int known) && known >= dimensions)
                return false;
            _dimensions[name] = dimensions;
            return true;
        }

        private bool Propagate(IEnumerable<AomCode> code)
        {
            bool changed = false;
            foreach (var c in Flatten(code))
            {
                if (c is AomTuple tuple)
                {
                    var call = Call(tuple.Value);
                    if (call != null && _returns.TryGetValue(call.Value.Name, out var returned))
                    {
                        for (int i = 0; i < tuple.Targets.Count && i < returned.Count; i++)
                        {
                            string target = tuple.Targets[i];
                            int bracket = target.IndexOf('[');
                            string name = bracket < 0 ? target : target.Substring(0, bracket).Trim();
                            int indices = bracket < 0 ? 0 : TopLevelIndices(target.Substring(bracket));
                            changed |= Raise(name, returned[i] + indices);
                        }
                    }
                    changed |= PropagateCalls(tuple.Value);
                }
                else if (c is AomField f)
                {
                    if (f.Type == null && f.Parameter != null && string.IsNullOrWhiteSpace(f.Value))
                        changed |= PropagateCalls(f.ClassType + f.Parameter);
                    if (!string.IsNullOrWhiteSpace(f.Value))
                    {
                        string value = f.Value.Trim();
                        if (value.StartsWith("=") && !value.StartsWith("=="))
                            changed |= Raise(f.Name, Dimensions(value.Substring(1)) + TopLevelIndices(f.FieldArray));
                        changed |= PropagateCalls(value.TrimStart('=', '+', '-', '|', '&', '^', '*', '/', '<', '>'));
                    }
                }
                else if (c is AomReturn r && !string.IsNullOrWhiteSpace(r.Parameter))
                {
                    changed |= PropagateCalls(r.Parameter);
                }
            }
            return changed;
        }

        /// <summary>Each call of a structure in an expression gives its parameters the dimensions of its arguments.</summary>
        private bool PropagateCalls(string expression)
        {
            bool changed = false;
            foreach (Match m in Regex.Matches(expression, @"\b([A-Za-z_]\w*)\s*\("))
            {
                if (!_methods.TryGetValue(m.Groups[1].Value, out var method))
                    continue;
                string arguments = Parenthesized(expression, m.Index + m.Length - 1);
                var passed = SplitArguments(arguments);
                var parameters = Parameters(method);
                for (int i = 0; i < parameters.Count && i < passed.Count; i++)
                    changed |= Raise(parameters[i], Dimensions(passed[i]));
            }
            return changed;
        }

        private static List<string> Parameters(AomMethod method) =>
            string.IsNullOrEmpty(method.MethodParameter) ? new List<string>() : SplitArguments(method.MethodParameter).Where(p => p.Length > 0).ToList();

        /// <summary>From the '(' at start, the parenthesized text, parentheses and all.</summary>
        private static string Parenthesized(string text, int start)
        {
            int depth = 0;
            for (int i = start; i < text.Length; i++)
            {
                if (text[i] == '(') depth++;
                else if (text[i] == ')' && --depth == 0)
                    return text.Substring(start, i - start + 1);
            }
            return text.Substring(start);
        }

        private static int TopLevelIndices(string array)
        {
            if (string.IsNullOrEmpty(array))
                return 0;
            int depth = 0, count = 0;
            foreach (char ch in array)
            {
                if (ch == '[' && depth++ == 0) count++;
                else if (ch == ']') depth--;
            }
            return count;
        }

        private (string Name, string Arguments)? Call(string expression)
        {
            var m = Regex.Match(expression.Trim(), @"^([A-Za-z_]\w*)\s*(\(.*\))$", RegexOptions.Singleline);
            if (!m.Success || Parenthesized(m.Groups[2].Value, 0).Length != m.Groups[2].Value.Length)
                return null;
            return (m.Groups[1].Value, m.Groups[2].Value);
        }

        /// <summary>The dimensions of an expression that is an array, a row of one, or a structure's value; 0 for any other.</summary>
        private int Dimensions(string expression)
        {
            string e = expression.Trim();
            var call = Call(e);
            if (call != null)
                return _returns.TryGetValue(call.Value.Name, out var returned) && returned.Count == 1 ? returned[0] : 0;

            var m = Regex.Match(e, @"^([A-Za-z_]\w*)\s*((?:\[.*\])?)$", RegexOptions.Singleline);
            if (!m.Success || !_dimensions.TryGetValue(m.Groups[1].Value, out int dimensions))
                return 0;
            return Math.Max(0, dimensions - TopLevelIndices(m.Groups[2].Value));
        }

        private static string ArrayType(int dimensions)
        {
            string type = "int";
            for (int i = 0; i < dimensions; i++)
                type = $"AomArray<{type}>";
            return type;
        }

        public string GetParameterType(AomMethod method, string parameter)
        {
            if (parameter == "nbBits")
                return "long";
            return ArrayType(_dimensions.TryGetValue(parameter, out int dimensions) ? dimensions : 0);
        }

        public string GetReturnType(AomMethod method)
        {
            if (!_returns.TryGetValue(method.MethodName, out var returned))
                return null;
            if (returned.Count > 1)
                return $"({string.Join(", ", returned.Select(ArrayType))})";
            return ArrayType(returned[0]);
        }

        #endregion

        public string GetFieldType(AomField field)
        {
            string element = "int";
            string width = Argument(field.Type, "f") ?? "";
            if (field.Type == "su(64)")
                element = "long";
            else if (int.TryParse(width, out int bits) && bits > 32 || field.Type != null && field.Type.StartsWith("le(") && int.TryParse(Argument(field.Type, "le"), out int bytes) && bytes > 4)
                element = "byte[]";

            string name = field.ClassType ?? field.Name;
            if (!_dimensions.TryGetValue(name, out int dimensions))
                return element;

            string type = element;
            for (int i = 0; i < dimensions; i++)
                type = $"AomArray<{type}>";
            return type;
        }

        public string GetReadMethod(AomField field)
        {
            string type = field.Type;
            if (type == null)
            {
                // A syntax structure: its method, with its arguments translated.
                string arguments = string.IsNullOrEmpty(field.Parameter) ? "()" : $"({string.Join(", ", SplitArguments(field.Parameter).Select(Expressions.Int))})";
                return $"{Expressions.MethodName(field.ClassType)}{arguments}";
            }

            // Arithmetic coded elements are the tile data's, which SharpAV2 does not decode: a syntax
            // structure the headers share with it (read_wienerns_filter) has them on its other path.
            if (type.StartsWith("L(") || type.StartsWith("NS(") || type == "S()") return "ReadArithmetic(";

            if (type == "uvlc()") return "stream.ReadUvlc(";
            if (type == "svlc()") return "stream.ReadSvlc(";
            if (type == "leb128()") return "stream.ReadLeb128(";

            string argument;
            if ((argument = Argument(type, "f")) != null)
            {
                if (int.TryParse(argument, out int bits))
                    return bits > 32 ? $"stream.ReadBytes({bits}," : $"stream.ReadFixed({bits},";
                return $"stream.ReadVariable({Expressions.Int(argument)},";
            }
            if ((argument = Argument(type, "le")) != null)
            {
                if (int.TryParse(argument, out int bytes) && bytes > 4)
                    return $"stream.ReadBytes({bytes * 8},";
                return $"stream.ReadLe({Expressions.Int(argument)},";
            }
            if ((argument = Argument(type, "su")) != null) return $"stream.ReadSignedIntVar({Expressions.Int(argument)},";
            if ((argument = Argument(type, "ns")) != null) return $"stream.Read_ns({Expressions.Int(argument)},";
            if ((argument = Argument(type, "tu")) != null) return $"stream.ReadTu({Expressions.Int(argument)},";
            if ((argument = Argument(type, "rg")) != null) return $"stream.ReadRg({Expressions.Int(argument)},";

            throw new System.NotSupportedException($"Descriptor {type} of {field.Name}");
        }

        /// <summary>"f(n)" and "f" to "n"; null if the descriptor is another.</summary>
        private static string Argument(string descriptor, string name)
        {
            if (descriptor == null || !descriptor.StartsWith(name + "(") || !descriptor.EndsWith(")"))
                return null;
            return descriptor.Substring(name.Length + 1, descriptor.Length - name.Length - 2).Trim();
        }

        /// <summary>"( a, f( b, c ) )" to "a" and "f( b, c )".</summary>
        public static List<string> SplitArguments(string parenthesized)
        {
            string inner = parenthesized.Trim();
            if (inner.StartsWith("(") && inner.EndsWith(")"))
                inner = inner.Substring(1, inner.Length - 2);

            var arguments = new List<string>();
            int depth = 0, start = 0;
            for (int i = 0; i < inner.Length; i++)
            {
                char c = inner[i];
                if (c == '(' || c == '[') depth++;
                else if (c == ')' || c == ']') depth--;
                else if (c == ',' && depth == 0)
                {
                    arguments.Add(inner.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }
            string last = inner.Substring(start).Trim();
            if (last.Length > 0 || arguments.Count > 0)
                arguments.Add(last);
            return arguments;
        }

        public string AppendMethod(AomCode field, string spacing, string retm) => retm;

        public string FixCondition(string value) => value;

        public string FixStatement(string value) => value;

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            // The parameters of AV2's syntax structures are integers, but for these.
            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                { "nbBits", "su(64)" },
            };

            return map.TryGetValue(parameter, out string type) ? type : "su(32)";
        }

        public string GetFieldDefaultValue(AomField field) => "";
    }
}
