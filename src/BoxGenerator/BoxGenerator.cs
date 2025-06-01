using BoxGenerator.CSharp;
using Newtonsoft.Json;
using Pidgin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;

namespace BoxGenerator;

public interface ICodeGenerator
{
    string GenerateParser();
}

public static class BoxGenerator
{
    public static string Generate(string[] jsonFiles, string[] jsonContents)
    {
        int success = 0;
        int duplicated = 0;
        int fail = 0;

        var parsedClasses = new Dictionary<string, PseudoClass>();
        List<PseudoClass> duplicates = new List<PseudoClass>();
        List<string> containers = new List<string>();

        for(int i = 0; i < jsonFiles.Length; i++) 
        {
            string file = jsonFiles[i];
            string json = jsonContents[i];

            using (JsonDocument document = JsonDocument.Parse(json, new JsonDocumentOptions()))
            {
                foreach (JsonElement element in document.RootElement.GetProperty("entries").EnumerateArray())
                {
                    string fourCC = element.TryGetProperty("fourcc", out _) ? element.GetProperty("fourcc").GetString() : null;
                    string sample = element.GetProperty("syntax").GetString()!;

                    string[] cont = element.GetProperty("containers").EnumerateArray().Select(x => x.ToString()).ToArray();
                    foreach (var con in cont)
                    {
                        if (con.StartsWith("{"))
                        {
                            var des = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(con);
                            foreach (var dc in des.Values)
                            {
                                foreach (var ddd in dc)
                                    containers.Add(ddd);
                            }
                        }
                        else
                        {
                            containers.Add(con);
                        }
                    }

                    if (string.IsNullOrEmpty(sample))
                    {
                        continue;
                    }

                    try
                    {
                        var result = Parser.Boxes.ParseOrThrow(sample);
                        long offset = 0;
                        result = DeduplicateBoxes(result);

                        foreach (var item in result)
                        {
                            item.Syntax = GetSampleCode(sample, offset, item.CurrentOffset);
                            offset = item.CurrentOffset;

                            if (parsedClasses.TryAdd(item.BoxName, item))
                            {
                                success++;
                            }
                            else
                            {
                                duplicated++;
                                duplicates.Add(item);
                                Debug.WriteLine($"Duplicated: {item.BoxName}");

                                if (item.Extended != null && item.Extended.BoxType != parsedClasses[item.BoxName].Extended.BoxType)
                                {
                                    if (!parsedClasses.TryAdd($"{item.BoxName}_{item.Extended.BoxType}", item))
                                    {
                                        LogConsole($"Duplicated and failed to add: {item.BoxName}");
                                    }
                                    // override the name
                                    item.BoxName = $"{item.BoxName}{item.Extended.BoxType}Dup";
                                }
                            }
                        }

                        Debug.WriteLine($"Succeeded parsing: {fourCC}");

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        LogConsole($"Failed to parse: {fourCC}");
                        fail++;
                    }
                }
            }
        }

        LogConsole($"Successful: {success}, Failed: {fail}, Duplicated: {duplicated}, Total: {success + fail + duplicated}");

        ParserDocument parserDocument = new ParserDocument(parsedClasses, containers);
        ICodeGenerator generator = new CSharpGenerator(parserDocument);
        string code = generator.GenerateParser();
        return code;
    }

    private static IEnumerable<PseudoClass> DeduplicateBoxes(IEnumerable<PseudoClass> result)
    {
        // some boxes have FourCC such as "'avc1' or 'avc3'" - make it two boxes
        List<PseudoClass> boxes = new List<PseudoClass>();
        foreach (var box in result)
        {
            if (box.Extended != null && box.Extended.BoxType != null && box.Extended.BoxType.Contains("\' or \'"))
            {
                string[] parts = box.Extended.BoxType.Split(new string[] { "\' or \'" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < parts.Length; i++)
                {
                    string part = parts[i];
                    string key = part.TrimStart('\'').TrimEnd('\'');
                    var copy = JsonConvert.DeserializeObject<PseudoClass>(
                        JsonConvert.SerializeObject(box,
                            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }),
                            new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All }); // deep copy
                    copy.Extended.BoxType = key;
                    boxes.Add(copy);
                }
            }
            else
            {
                boxes.Add(box);
            }
        }
        return boxes;
    }

    private static string GetSampleCode(string sample, long startOffset, long endOffset)
    {
        // parse the syntax of this particular class as there can be multiple boxes/classes in a single JSON record
        return sample.Substring((int)startOffset, (int)(endOffset - startOffset));
    }

    private static void LogConsole(string v)
    {
        Debug.WriteLine(v);
        Console.WriteLine(v);
    }
}
