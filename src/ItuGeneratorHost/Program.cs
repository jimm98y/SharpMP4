
using System;
using System.IO;

string filePath = args.Length > 0 ? args[0] : null;

if (string.IsNullOrEmpty(filePath))
{
    throw new ArgumentNullException("filePath");
}

string fileContent = File.ReadAllText(filePath);
string code = ItuGenerator.ItuGenerator.Generate(filePath, fileContent);

Console.WriteLine(code);