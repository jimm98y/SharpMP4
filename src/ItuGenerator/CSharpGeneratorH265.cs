namespace ItuGenerator
{
    public class CSharpGeneratorH265 : ICustomGenerator
    {
        public void FixClassParameters(ItuClass ituClass)
        {
            throw new System.NotImplementedException();
        }

        public string FixCondition(string condition, MethodType methodType)
        {
            throw new System.NotImplementedException();
        }

        public string FixFieldValue(string fieldValue)
        {
            throw new System.NotImplementedException();
        }

        public void FixMethodAllocation(string name, ref string method, ref string typedef)
        {
            throw new System.NotImplementedException();
        }

        public string FixMissingParameters(ItuClass b, string parameter, string classType)
        {
            throw new System.NotImplementedException();
        }

        public string GetCtorParameterType(string parameter)
        {
            throw new System.NotImplementedException();
        }

        public string GetFieldDefaultValue(ItuField field)
        {
            throw new System.NotImplementedException();
        }

        public string PreprocessDefinitionsFile(string definitions)
        {
            throw new System.NotImplementedException();
        }

        public string ReplaceParameter(string parameter)
        {
            throw new System.NotImplementedException();
        }
    }
}