using Pidgin;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace AomGenerator
{
    public class Parser
    {
        // TODO
        public static Parser<char, IEnumerable<AomClass>> AomClasses => throw new NotImplementedException();
    }

    [SuppressMessage("naming", "CA1724:The type name conflicts with the namespace name", Justification = "Example code")]
    public interface AomCode
    { }

    public class AomComment : AomCode
    {
        public AomComment()
        { }

        public AomComment(string comment)
        {
            Comment = comment;
        }

        public string Comment { get; }
    }

    public class AomField : AomCode
    {
        public AomField()
        { }

        public AomField(string name, Maybe<string> parameter, Maybe<string> array, Maybe<string> increment, Maybe<string> value, Maybe<string> comment, Maybe<string> category, Maybe<string> type)
        {
            Name = ClassType = name;
            Increment = increment.GetValueOrDefault();
            Parameter = parameter.GetValueOrDefault();
            FieldArray = array.GetValueOrDefault();
            Value = value.GetValueOrDefault();
            Comment = comment.GetValueOrDefault();
            Category = category.GetValueOrDefault();
            Type = type.GetValueOrDefault();
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string ClassType { get; set; }
        public string Value { get; set; }
        public string Parameter { get; set; }
        public string Increment { get; set; }
        public string FieldArray { get; set; }
        public string Comment { get; set; }
        public string Category { get; set; }

        public AomBlock Parent { get; internal set; }
        public bool MakeList { get; internal set; }
    }

    public class AomClass : AomCode
    {
        public AomClass(string className, Maybe<string> classParameter, IEnumerable<AomCode> fields, int endOffset)
        {
            ClassName = className;
            ClassParameter = classParameter.GetValueOrDefault();
            Fields = fields;
            EndOffset = endOffset;
        }

        public string ClassName { get; }
        public string ClassParameter { get; set; }
        public IEnumerable<AomCode> Fields { get; }
        public int EndOffset { get; }

        public List<AomField> FlattenedFields { get; internal set; }
        public List<AomField> AddedFields { get; internal set; } = new List<AomField>();
        public List<AomField> RequiresDefinition { get; internal set; } = new List<AomField>();
        public string Syntax { get; set; }
    }

    public class AomBlock : AomCode
    {
        public string Type { get; }
        public string Condition { get; }
        public string Comment { get; }
        public IEnumerable<AomCode> Content { get; }
        public AomBlock Parent { get; internal set; }
        public List<AomField> RequiresAllocation { get; internal set; } = new List<AomField>();

        public AomBlock(string type, Maybe<string> condition, Maybe<string> comment, IEnumerable<AomCode> content)
        {
            this.Type = type;
            this.Condition = condition.GetValueOrDefault();
            this.Comment = comment.GetValueOrDefault();
            this.Content = content;
        }
    }

    public class AomBlockIfThenElse : AomCode
    {
        public AomBlockIfThenElse(AomCode blockIf, Maybe<IEnumerable<AomCode>> blockElseIf, Maybe<AomCode> blockElse)
        {
            BlockIf = blockIf;
            BlockElseIf = blockElseIf.GetValueOrDefault();
            if (BlockElseIf == null)
                BlockElseIf = new List<AomCode>();
            BlockElse = blockElse.GetValueOrDefault();
        }

        public AomCode BlockIf { get; }
        public AomCode BlockElse { get; }
        public IEnumerable<AomCode> BlockElseIf { get; set; }
        public AomBlock Parent { get; internal set; }
    }
}
