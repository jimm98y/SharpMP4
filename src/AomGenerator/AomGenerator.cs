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
            else if (path.Contains("VP9"))
            {
                customGenerator = new CSharpGeneratorVP9();
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
            return sample.Substring((int)startOffset, (int)(endOffset - startOffset)).Trim();
        }
    }
}
