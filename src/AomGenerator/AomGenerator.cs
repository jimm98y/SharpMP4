
using AomGenerator.CSharp;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AomGenerator
{
    public class AomGenerator
    {
        /// <summary>The syntax a file belongs to: AV2.Functions.js is AV2's, as AV2.js is.</summary>
        public static string Family(string name) => name.Split('.')[0];

        public static string Generate(string path, string content)
        {
            ICustomGenerator customGenerator = null;
            if (path.Contains("AV1"))
            {
                customGenerator = new CSharpGeneratorAV1();
            }
            else if (path.Contains("AV2"))
            {
                customGenerator = new CSharpGeneratorAV2();
            }
            else
            {
                throw new NotSupportedException();
            }

            content = customGenerator.PreprocessDefinitionsFile(content);

            IEnumerable<AomMethod> parsed = Parser.AomClasses.ParseOrThrow(content);

            long startOffset = 0;
            foreach (var c in parsed)
            {
                c.Syntax = GetSampleCode(content, startOffset, c.EndOffset);
                startOffset = c.EndOffset;
            }

            CSharpGenerator generator = new CSharpGenerator(customGenerator);
            string code = generator.GenerateParser(Path.GetFileNameWithoutExtension(path), parsed);
            Debug.WriteLine(code);
            return code;
        }

        private static string GetSampleCode(string sample, long startOffset, long endOffset)
        {
            return sample.Substring((int)startOffset, (int)(endOffset - startOffset)).TrimStart();
        }
    }
}
