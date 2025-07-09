using System;
using System.Collections.Generic;
using System.Text;

namespace AomGenerator.CSharp
{
    public class CSharpGeneratorAV1 : ICustomGenerator
    {
        public string AppendMethod(AomCode field, MethodType methodType, string spacing, string retm)
        {
            throw new NotImplementedException();
        }

        public string FixAllocations(string spacing, string appendType, string variableType, string variableName)
        {
            throw new NotImplementedException();
        }

        public string FixAppendType(string appendType, string variableName)
        {
            throw new NotImplementedException();
        }

        public void FixClassParameters(AomClass ituClass)
        {
            throw new NotImplementedException();
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            throw new NotImplementedException();
        }

        public string FixFieldValue(string fieldValue)
        {
            throw new NotImplementedException();
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            throw new NotImplementedException();
        }

        public string FixMissingParameters(AomClass b, string parameter, string classType)
        {
            throw new NotImplementedException();
        }

        public void FixNestedIndexes(List<string> ret, AomField field)
        {
            throw new NotImplementedException();
        }

        public string FixStatement(string fieldValue)
        {
            throw new NotImplementedException();
        }

        public string FixVariableType(string variableType)
        {
            throw new NotImplementedException();
        }

        public string GetCtorParameterType(string parameter)
        {
            throw new NotImplementedException();
        }

        public string GetDerivedVariables(string name)
        {
            throw new NotImplementedException();
        }

        public string GetFieldDefaultValue(AomField field)
        {
            throw new NotImplementedException();
        }

        public string GetParameterType(string parameter)
        {
            throw new NotImplementedException();
        }

        public string GetVariableSize(string parameter)
        {
            throw new NotImplementedException();
        }

        public string PreprocessDefinitionsFile(string definitions)
        {
            return definitions;
        }

        public string ReplaceParameter(string parameter)
        {
            throw new NotImplementedException();
        }
    }
}
