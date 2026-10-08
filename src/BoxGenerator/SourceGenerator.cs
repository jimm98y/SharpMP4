using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace BoxGenerator
{
    /// <summary>
    /// Integration with Microsoft Roslyn Source Generators.
    /// </summary>
    [Generator]
    public class SourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            // find all additional files that end with .json
            IncrementalValuesProvider<AdditionalText> textFiles = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith(".json"));

            // read their contents and save their name
            var compilationAndFiles = initContext.CompilationProvider.Combine(textFiles.Collect());

            // generate a class that contains their values as const strings
            initContext.RegisterSourceOutput(compilationAndFiles, (productionContext, sourceContext) =>
            {
                Dictionary<string, string> code = BoxGenerator.Generate(sourceContext.Right.Select(x => x.GetText().ToString()).ToArray(), sourceContext.Right.Select(x => x.GetText().ToString()).ToArray());

                foreach (var file in code)
                {
                    productionContext.AddSource($"{file.Key}.g.cs", WithCrlf(file.Value));
                }
            });
        }

        /// <summary>
        /// The code with Windows line endings, whatever the platform it is generated on: the generator's own "\r\n"s and
        /// the "\n"s of AppendLine and of verbatim strings off a Unix checkout alike, so the files are the same everywhere.
        /// </summary>
        private static string WithCrlf(string code) => code.Replace("\r\n", "\n").Replace("\n", "\r\n");
    }
}
