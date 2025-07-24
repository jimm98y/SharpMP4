using Microsoft.CodeAnalysis;
using System.IO;

namespace AomGenerator
{
    /// <summary>
    /// Integration with Microsoft Roslyn Source Generators.
    /// </summary>
    [Generator]
    public class SourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            // find all additional files that end with .js
            IncrementalValuesProvider<AdditionalText> textFiles = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".js"));

            // read their contents and save their name
            IncrementalValuesProvider<(string name, string content)> namesAndContents = textFiles.Select((text, cancellationToken) => (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)!.ToString()));

            // generate a class that contains their values as const strings
            initContext.RegisterSourceOutput(namesAndContents, (spc, nameAndContent) =>
            {
                string code = AomGenerator.Generate(nameAndContent.name, nameAndContent.content);
                spc.AddSource($"{nameAndContent.name}.g.cs", code);
            });
        }
    }
}
