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

using System;
using System.IO;
using System.Linq;

string? filePath = args.Length > 0 ? args[0] : null;

if (string.IsNullOrEmpty(filePath))
{
    throw new ArgumentNullException("filePath");
}

string fileContent = File.ReadAllText(filePath);

// AV2.js and the AV2.*.js beside it are generated as one, as the source generator does
string name = Path.GetFileNameWithoutExtension(filePath);
foreach (string sibling in Directory.GetFiles(Path.GetDirectoryName(Path.GetFullPath(filePath))!, name + ".*.js").OrderBy(f => f))
{
    fileContent += "\n\n" + File.ReadAllText(sibling);
}

string code = AomGenerator.AomGenerator.Generate(filePath, fileContent);

Console.WriteLine(code);