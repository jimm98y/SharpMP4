using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AomGenerator.CSharp
{
    /// <summary>
    /// Turns an expression of the AV2 specification into C#. The specification's expressions are C's:
    /// an integer is true when it is not 0, and a comparison is an integer, 0 or 1. C# keeps booleans
    /// apart, so each expression is parsed, and written back with (x != 0) where a condition wants a
    /// boolean and (b ? 1 : 0) where a value wants a number. Calls of syntax structures are named as
    /// their methods are.
    /// </summary>
    public sealed class AomExpressions
    {
        private readonly ISet<string> _syntaxStructures;

        public AomExpressions(ISet<string> syntaxStructures)
        {
            _syntaxStructures = syntaxStructures;
        }

        /// <summary>An expression where a number is wanted: a value, an index, an argument.</summary>
        public string Int(string expression) => EmitInt(Parse(expression));

        /// <summary>An expression where a condition is wanted.</summary>
        public string Bool(string expression) => EmitBool(Parse(expression));

        /// <summary>The name a syntax structure's method has.</summary>
        public string MethodName(string name) => _syntaxStructures.Contains(name) ? name.ToPropertyCase() : name;

        #region Parsing

        private abstract class Node { }
        private sealed class Literal : Node { public string Text; }
        private sealed class Unary : Node { public string Op; public Node Operand; }
        private sealed class Binary : Node { public string Op; public Node Left, Right; }
        private sealed class Ternary : Node { public Node Condition, Then, Else; }
        private sealed class Call : Node { public string Name; public List<Node> Arguments; }
        private sealed class Index : Node { public Node Target, Position; }

        private static readonly string[] Operators =
        {
            "<<", ">>", "<=", ">=", "==", "!=", "&&", "||",
            "+", "-", "*", "/", "%", "<", ">", "&", "|", "^", "!", "~", "?", ":", "(", ")", "[", "]", ",",
        };

        private static readonly string[][] BinaryLevels =
        {
            new[] { "||" },
            new[] { "&&" },
            new[] { "|" },
            new[] { "^" },
            new[] { "&" },
            new[] { "==", "!=" },
            new[] { "<", "<=", ">", ">=" },
            new[] { "<<", ">>" },
            new[] { "+", "-" },
            new[] { "*", "/", "%" },
        };

        private static readonly HashSet<string> Comparisons = new HashSet<string> { "==", "!=", "<", "<=", ">", ">=" };
        private static readonly HashSet<string> Logical = new HashSet<string> { "&&", "||" };

        private List<string> _tokens;
        private int _position;
        private string _source;

        private Node Parse(string expression)
        {
            _source = expression;
            _tokens = Tokenize(expression);
            _position = 0;
            Node node = ParseTernary();
            if (_position != _tokens.Count)
                throw new FormatException($"Unexpected '{_tokens[_position]}' in: {expression}");
            return node;
        }

        private static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            int i = 0;
            while (i < expression.Length)
            {
                char c = expression[i];
                if (char.IsWhiteSpace(c))
                {
                    i++;
                }
                else if (char.IsLetterOrDigit(c) || c == '_')
                {
                    int start = i;
                    while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;
                    tokens.Add(expression.Substring(start, i - start));
                }
                else
                {
                    string op = Operators.FirstOrDefault(o => string.CompareOrdinal(expression, i, o, 0, o.Length) == 0);
                    if (op == null)
                        throw new FormatException($"Unexpected '{c}' in: {expression}");
                    tokens.Add(op);
                    i += op.Length;
                }
            }
            return tokens;
        }

        private string Peek => _position < _tokens.Count ? _tokens[_position] : null;

        private string Next() => _tokens[_position++];

        private void Expect(string token)
        {
            if (Peek != token)
                throw new FormatException($"Expected '{token}', found '{Peek}' in: {_source}");
            _position++;
        }

        private Node ParseTernary()
        {
            Node condition = ParseBinary(0);
            if (Peek != "?")
                return condition;
            Next();
            Node then = ParseTernary();
            Expect(":");
            Node otherwise = ParseTernary();
            return new Ternary { Condition = condition, Then = then, Else = otherwise };
        }

        private Node ParseBinary(int level)
        {
            if (level == BinaryLevels.Length)
                return ParseUnary();

            Node left = ParseBinary(level + 1);
            while (Peek != null && BinaryLevels[level].Contains(Peek))
            {
                string op = Next();
                Node right = ParseBinary(level + 1);
                left = new Binary { Op = op, Left = left, Right = right };
            }
            return left;
        }

        private Node ParseUnary()
        {
            if (Peek == "!" || Peek == "-" || Peek == "~" || Peek == "+")
            {
                string op = Next();
                return new Unary { Op = op, Operand = ParseUnary() };
            }
            return ParsePostfix();
        }

        private Node ParsePostfix()
        {
            Node node = ParsePrimary();
            while (Peek == "[")
            {
                Next();
                Node position = ParseTernary();
                Expect("]");
                node = new Index { Target = node, Position = position };
            }
            return node;
        }

        private Node ParsePrimary()
        {
            string token = Peek ?? throw new FormatException($"Unexpected end of: {_source}");
            if (token == "(")
            {
                Next();
                Node inner = ParseTernary();
                Expect(")");
                return inner;
            }

            if (!(char.IsLetterOrDigit(token[0]) || token[0] == '_'))
                throw new FormatException($"Unexpected '{token}' in: {_source}");

            Next();
            if (Peek == "(" && !char.IsDigit(token[0]))
            {
                Next();
                var arguments = new List<Node>();
                if (Peek != ")")
                {
                    arguments.Add(ParseTernary());
                    while (Peek == ",")
                    {
                        Next();
                        arguments.Add(ParseTernary());
                    }
                }
                Expect(")");
                return new Call { Name = token, Arguments = arguments };
            }

            return new Literal { Text = token };
        }

        #endregion // Parsing

        #region Emitting

        private static bool IsBoolean(Node node) =>
            node is Binary b && (Comparisons.Contains(b.Op) || Logical.Contains(b.Op)) ||
            node is Unary u && u.Op == "!";

        private string EmitInt(Node node)
        {
            if (IsBoolean(node))
                return $"({EmitBool(node)} ? 1 : 0)";

            switch (node)
            {
                case Literal literal:
                    return literal.Text;
                case Unary unary:
                    return $"{unary.Op}{EmitInt(unary.Operand)}";
                case Binary binary:
                    return $"({EmitInt(binary.Left)} {binary.Op} {EmitInt(binary.Right)})";
                case Ternary ternary:
                    return $"({EmitBool(ternary.Condition)} ? {EmitInt(ternary.Then)} : {EmitInt(ternary.Else)})";
                case Call call:
                    return $"{MethodName(call.Name)}({string.Join(", ", call.Arguments.Select(EmitInt))})";
                case Index index:
                    return $"{EmitInt(index.Target)}[{EmitInt(index.Position)}]";
                default:
                    throw new NotSupportedException(node.GetType().Name);
            }
        }

        private string EmitBool(Node node)
        {
            switch (node)
            {
                case Binary binary when Comparisons.Contains(binary.Op):
                    return $"({EmitInt(binary.Left)} {binary.Op} {EmitInt(binary.Right)})";
                case Binary binary when Logical.Contains(binary.Op):
                    return $"({EmitBool(binary.Left)} {binary.Op} {EmitBool(binary.Right)})";
                case Unary unary when unary.Op == "!":
                    return $"!{EmitBool(unary.Operand)}";
                default:
                    return $"({EmitInt(node)} != 0)";
            }
        }

        #endregion // Emitting
    }
}
