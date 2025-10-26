
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