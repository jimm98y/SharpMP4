using SharpISOBMFF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// TTML and its IMSC profiles (ISO/IEC 14496-30, 6): an 'stpp' sample entry with the documents' namespaces, and each
    /// sample a TTML document - its images, where it has any, after it.
    /// </summary>
    public class TtmlTrack : SubtitleTrackBase
    {
        public override string HandlerName => HandlerNames.Subtitle;

        /// <summary>The documents' namespaces, space separated.</summary>
        public string Namespace { get; set; } = "http://www.w3.org/ns/ttml";

        /// <summary>Where the schemas of the namespaces are, space separated; null for none.</summary>
        public string SchemaLocation { get; set; }

        /// <summary>The media types of the resources after each document - images, fonts - space separated; null for none.</summary>
        public string AuxiliaryMimeTypes { get; set; }

        public TtmlTrack(uint timescale = 1000)
        {
            Timescale = timescale;
        }

        public TtmlTrack(XMLSubtitleSampleEntry entry, uint timescale, string handlerType) : this(timescale)
        {
            SampleEntry = entry;
            _handlerType = handlerType;
            Namespace = entry.Ns.Length > 0 ? entry.Ns.Text : Namespace;
            SchemaLocation = entry.SchemaLocationPresent ? entry.SchemaLocation.Text : null;
            AuxiliaryMimeTypes = entry.AuxiliaryMimeTypesPresent ? entry.AuxiliaryMimeTypes.Text : null;
        }

        // CMAF's IMSC tracks are of the 'text' handler, 14496-30's of 'subt'
        private readonly string _handlerType;
        public override string HandlerType => _handlerType ?? HandlerTypes.Subtitle;

        public override IReadOnlyList<SubtitleCue> ParseCues(byte[] buffer, int offset, int length, long start, long duration)
        {
            // the document, without the images after it: it ends with its root element's end tag, </tt> or </prefix:tt>
            string text = Encoding.UTF8.GetString(buffer, offset, length);
            var end = RootEnd.Match(text);
            return Cue(end.Success ? text.Substring(0, end.Index + end.Length) : text.TrimEnd('\0'), start, duration);
        }

        private static readonly Regex RootEnd = new Regex(@"</([A-Za-z_][\w.-]*:)?tt\s*>", RegexOptions.CultureInvariant);

        /// <summary>
        /// A TTML sample is one document: of one cue, its text the document; of none, a document with nothing in it, for
        /// a sample is a document even where nothing is shown.
        /// </summary>
        public override byte[] CreateSample(IReadOnlyList<SubtitleCue> cues)
        {
            if (cues == null || cues.Count == 0)
                return Encoding.UTF8.GetBytes($"<?xml version=\"1.0\" encoding=\"UTF-8\"?><tt xmlns=\"{Namespace.Split(' ')[0]}\"><body/></tt>");
            if (cues.Count != 1)
                throw new ArgumentException("A TTML sample is one document: one cue, its text the document.", nameof(cues));
            return Encoding.UTF8.GetBytes(cues[0].Text ?? "");
        }

        public override Box CreateSampleEntryBox()
        {
            if (SampleEntry != null)
                return SampleEntry;

            return new XMLSubtitleSampleEntry
            {
                DataReferenceIndex = 1,
                ReservedSampleEntry = new byte[6],
                Ns = Terminated(Namespace),
                SchemaLocation = Terminated(SchemaLocation ?? ""),
                SchemaLocationPresent = SchemaLocation != null || AuxiliaryMimeTypes != null,
                AuxiliaryMimeTypes = Terminated(AuxiliaryMimeTypes ?? ""),
                AuxiliaryMimeTypesPresent = AuxiliaryMimeTypes != null,
                Children = new List<Box>(),
            };
        }

        public override ITrack Clone() => new TtmlTrack(Timescale) { Namespace = Namespace, SchemaLocation = SchemaLocation, AuxiliaryMimeTypes = AuxiliaryMimeTypes, Language = Language };
    }
}
