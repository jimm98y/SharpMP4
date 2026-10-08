using Microsoft.CodeAnalysis;
using System.IO;

namespace ItuGenerator
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
                string code = ItuGenerator.Generate(nameAndContent.name, nameAndContent.content);
                spc.AddSource($"{nameAndContent.name}.g.cs", WithCrlf(code));
            });
        }

        /// <summary>
        /// The code with Windows line endings, whatever the platform it is generated on: the generator's own "\r\n"s and
        /// the "\n"s of AppendLine and of verbatim strings off a Unix checkout alike, so the files are the same everywhere.
        /// </summary>
        private static string WithCrlf(string code) => code.Replace("\r\n", "\n").Replace("\n", "\r\n");
    }
}
