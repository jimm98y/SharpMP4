using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace SharpMP4.Readers
{
    /// <summary>A step of the path to an XMP value: a property or a field of a structure, or an item of an array.</summary>
    public sealed class XmpStep
    {
        /// <summary>The namespace URI of the property or field; null for an array item.</summary>
        public string Namespace { get; set; }

        /// <summary>The prefix the packet gives the namespace.</summary>
        public string Prefix { get; set; }

        /// <summary>The local name of the property or field; null for an array item.</summary>
        public string Name { get; set; }

        /// <summary>The index of an array item, from 1; 0 for a property or field.</summary>
        public int Index { get; set; }

        public override string ToString() => Index > 0 ? $"[{Index}]" : $"{Prefix}:{Name}";
    }

    /// <summary>A value of an XMP packet: the path to it from its top-level property, and its language.</summary>
    public sealed class XmpProperty
    {
        /// <summary>The path to the value, first the top-level property: xmpMM:History, [2], stEvt:action.</summary>
        public List<XmpStep> Path { get; set; } = new List<XmpStep>();

        /// <summary>The namespace URI of the top-level property.</summary>
        public string Namespace => Path[0].Namespace;

        /// <summary>The local name of the top-level property.</summary>
        public string Name => Path[0].Name;

        /// <summary>The value: the text of a simple property, or the URI of an rdf:resource.</summary>
        public string Value { get; set; }

        /// <summary>The xml:lang in effect for the value (an item of a language alternative); null where there is none.</summary>
        public string Language { get; set; }

        /// <summary>The path as XMP writes it: xmpMM:History[2]/stEvt:action.</summary>
        public override string ToString() =>
            string.Concat(Path.Select((s, i) => s.Index > 0 ? s.ToString() : (i > 0 ? "/" : "") + s)) + " = " + Value + (Language != null ? $" ({Language})" : "");
    }

    /// <summary>
    /// Reads the values of an XMP packet (ISO 16684-1, 7: the RDF serialization): the properties of each
    /// rdf:Description, whether written as attributes or as elements; structures, whether written as an
    /// rdf:Description, with rdf:parseType="Resource" or as attributes; arrays (rdf:Seq, rdf:Bag, rdf:Alt)
    /// and their items; and rdf:resource values. Qualifiers of a value (rdf:value) give the value.
    /// </summary>
    public static class XmpReader
    {
        private const string Rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
        private static readonly XNamespace RdfNs = Rdf;
        private static readonly XName Lang = XNamespace.Xml + "lang";

        public static List<XmpProperty> Read(string packet)
        {
            var properties = new List<XmpProperty>();
            if (string.IsNullOrEmpty(packet))
                return properties;

            XDocument document;
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null };
            using (var reader = XmlReader.Create(new StringReader(packet.TrimEnd('\0')), settings))
                document = XDocument.Load(reader);

            // x:xmpmeta names the toolkit that wrote the packet (7.3)
            XNamespace meta = "adobe:ns:meta/";
            foreach (var xmpmeta in document.Descendants(meta + "xmpmeta"))
            {
                var toolkit = xmpmeta.Attribute(meta + "xmptk");
                if (toolkit != null)
                    properties.Add(new XmpProperty { Path = new List<XmpStep> { Step(xmpmeta, toolkit.Name) }, Value = toolkit.Value });
            }

            foreach (var rdf in document.Descendants(RdfNs + "RDF"))
            {
                // what the packet is about (7.4): one URI, or none, for all its descriptions
                foreach (var about in rdf.Elements(RdfNs + "Description").Select(d => d.Attribute(RdfNs + "about")).Where(a => !string.IsNullOrEmpty(a?.Value)).GroupBy(a => a.Value).Select(g => g.First()))
                    properties.Add(new XmpProperty { Path = new List<XmpStep> { Step(about.Parent, about.Name) }, Value = about.Value });

                foreach (var description in rdf.Elements(RdfNs + "Description"))
                    ReadFields(description, new List<XmpStep>(), (string)description.Attribute(Lang), properties);
            }
            return properties;
        }

        // The fields of a node - an rdf:Description or a structure: its attributes, then its elements
        private static void ReadFields(XElement node, List<XmpStep> path, string language, List<XmpProperty> properties)
        {
            foreach (var attribute in node.Attributes())
            {
                if (attribute.IsNamespaceDeclaration || attribute.Name.Namespace == RdfNs || attribute.Name.Namespace == XNamespace.Xml || attribute.Name.Namespace == XNamespace.None)
                    continue;
                properties.Add(new XmpProperty { Path = With(path, Step(node, attribute.Name)), Value = attribute.Value, Language = language });
            }
            foreach (var element in node.Elements())
                ReadValue(element, With(path, Step(element, element.Name)), (string)element.Attribute(Lang) ?? language, properties);
        }

        // A property element's value (ISO 16684-1, 7.9): a resource, a structure, an array or text
        private static void ReadValue(XElement element, List<XmpStep> path, string language, List<XmpProperty> properties)
        {
            var resource = element.Attribute(RdfNs + "resource");
            if (resource != null)
            {
                properties.Add(new XmpProperty { Path = path, Value = resource.Value, Language = language });
                return;
            }

            // a qualified value: its rdf:value is the value
            var qualified = element.Element(RdfNs + "value") ?? element.Element(RdfNs + "Description")?.Element(RdfNs + "value");
            if (qualified != null)
            {
                ReadValue(qualified, path, (string)qualified.Attribute(Lang) ?? language, properties);
                return;
            }

            if ((string)element.Attribute(RdfNs + "parseType") == "Resource")
            {
                ReadFields(element, path, language, properties);
                return;
            }

            var inner = element.Elements().FirstOrDefault();
            if (inner != null && inner.Name.Namespace == RdfNs && (inner.Name.LocalName == "Seq" || inner.Name.LocalName == "Bag" || inner.Name.LocalName == "Alt"))
            {
                int index = 0;
                foreach (var item in inner.Elements(RdfNs + "li"))
                    ReadValue(item, With(path, new XmpStep { Index = ++index }), (string)item.Attribute(Lang) ?? language, properties);
                return;
            }
            if (inner != null && inner.Name == RdfNs + "Description")
            {
                ReadFields(inner, path, (string)inner.Attribute(Lang) ?? language, properties);
                return;
            }
            if (inner != null)
            {
                // a structure of fields, written as elements without rdf:parseType
                ReadFields(element, path, language, properties);
                return;
            }

            // a structure written as attributes only (7.9.2.5), or text
            if (element.Attributes().Any(a => !a.IsNamespaceDeclaration && a.Name.Namespace != RdfNs && a.Name.Namespace != XNamespace.Xml && a.Name.Namespace != XNamespace.None))
            {
                ReadFields(element, path, language, properties);
                return;
            }
            properties.Add(new XmpProperty { Path = path, Value = element.Value, Language = language });
        }

        private static XmpStep Step(XElement context, XName name) =>
            new XmpStep { Namespace = name.NamespaceName, Name = name.LocalName, Prefix = context.GetPrefixOfNamespace(name.Namespace) };

        private static List<XmpStep> With(List<XmpStep> path, XmpStep step) => new List<XmpStep>(path) { step };
    }
}
