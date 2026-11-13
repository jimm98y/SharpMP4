// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

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
                    productionContext.AddSource($"{file.Key}.g.cs", WithCrlf(SharpMP4.Generators.LicenseHeader.Prepend(file.Value)));
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
