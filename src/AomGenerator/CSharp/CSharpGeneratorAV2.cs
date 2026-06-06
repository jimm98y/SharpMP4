using System.Collections.Generic;

namespace AomGenerator.CSharp
{
    public class CSharpGeneratorAV2 : ICustomGenerator
    {
        public string AppendMethod(AomCode field, string spacing, string retm)
        {
            switch ((field as AomField).Name)
            {
                default:
                    return retm;
            }
        }

        public string FixCondition(string value)
        {
            return FixStatement(value);
        }

        public string FixStatement(string value)
        {
            return value;
        }

        public string GetCtorParameterType(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return "";

            Dictionary<string, string> map = new Dictionary<string, string>()
            {
                
            };

            return map[parameter];
        }

        public string GetFieldDefaultValue(AomField field)
        {
            switch (field.Name)
            {
                default:
                    return "";
            }
        }

        public string PreprocessDefinitionsFile(string definitions)
        {
            // rename ref -> refc to avoid C# conflicts with built-in ref keyword
            return definitions;
        }
    }
}
