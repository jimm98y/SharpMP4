using Pidgin;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ItuGenerator;

partial class Program
{
    static void Main(string[] args)
    {
        string[] jsonFiles = Directory.GetFiles("Definitions", "*.js");

        foreach (var file in jsonFiles)
        {
            string definitions = File.ReadAllText(file);
            definitions = definitions.Replace(" scalingList", " scalingLst"); // TODO remove this temporary fix

            var parsed = Parser.ItuClasses.ParseOrThrow(definitions);
            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parsed, Newtonsoft.Json.Formatting.Indented));

            long startOffset = 0;
            foreach (var c in parsed)
            {
                c.Syntax = GetSampleCode(definitions, startOffset, c.EndOffset);
                startOffset = c.EndOffset;
            }

            CSharpGenerator generator = new CSharpGenerator();
            string code = generator.GenerateParser(Path.GetFileNameWithoutExtension(file), parsed);
            Debug.WriteLine(code);
            break;
        }
    }

    private static string GetSampleCode(string sample, long startOffset, long endOffset)
    {
        // parse the syntax of this particular class as there can be multiple boxes/classes in a single JSON record
        return sample.Substring((int)startOffset, (int)(endOffset - startOffset));
    }
}