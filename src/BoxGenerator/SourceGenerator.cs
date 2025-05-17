using Microsoft.CodeAnalysis;
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
                string code = BoxGenerator.Generate(sourceContext.Right.Select(x => x.GetText().ToString()).ToArray(), sourceContext.Right.Select(x => x.GetText().ToString()).ToArray());
                productionContext.AddSource($"Boxes.g.cs", code);
            });
        }
    }
}
