
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