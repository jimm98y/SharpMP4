using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public string GenerateParser(string type, IEnumerable<AomClass> ituClasses)
        {
            throw new NotImplementedException();
        }
    }
}
