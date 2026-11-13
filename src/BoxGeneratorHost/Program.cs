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
using System.Collections.Generic;
using System.IO;
using System.Linq;

string directoryPath = args.Length > 0 ? args[0] : null;

if (string.IsNullOrEmpty(directoryPath))
{
    throw new ArgumentNullException("directoryPath");
}

string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");
string[] jsonContent = jsonFiles.Select(File.ReadAllText).ToArray();

//string code = BoxGenerator.BoxGenerator.GenerateDocumentation(jsonFiles, jsonContent);
Dictionary<string, string> code = BoxGenerator.BoxGenerator.Generate(jsonFiles, jsonContent);

foreach (var file in code)
{
    Console.WriteLine(file.Value);
}