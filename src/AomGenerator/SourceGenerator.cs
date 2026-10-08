using Microsoft.CodeAnalysis;
using System.IO;
using System.Linq;

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

            // AV2.js and the AV2.*.js beside it are one syntax, generated as one: AV2.js first, then the rest by name
            var families = namesAndContents.Collect().SelectMany((files, cancellationToken) => files
                .GroupBy(f => AomGenerator.Family(f.name))
                .Select(g => (name: g.Key, content: string.Join("\n\n", g.OrderBy(f => f.name == g.Key ? "" : f.name).Select(f => f.content))))
                .ToList());

            // generate a class that contains their values as const strings
            initContext.RegisterSourceOutput(families, (spc, nameAndContent) =>
            {
                string code = AomGenerator.Generate(nameAndContent.name, nameAndContent.content);
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
