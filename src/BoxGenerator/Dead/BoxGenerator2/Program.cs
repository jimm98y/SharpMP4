// See https://aka.ms/new-console-template for more information
using PegExamples;

Console.WriteLine("Hello, World!");

var parser = new ExpressionParser();
var result = parser.Parse("5.1+2*3");
Console.WriteLine(result); // Outputs "11.1".