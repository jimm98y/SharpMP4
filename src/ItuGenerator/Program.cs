using Pidgin;
using System;
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
            var parsed = Parser.ItuClasses.ParseOrThrow(definitions);
            Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(parsed, Newtonsoft.Json.Formatting.Indented));

            CSharpGenerator generator = new CSharpGenerator();
            string code = generator.GenerateParser(parsed.First());
            Debug.WriteLine(code);
        }
    }
}