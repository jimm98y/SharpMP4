using SharpISOBMFF;
using SharpMP4.Common;
using System;

namespace SharpMP4.Builders
{
    /// <summary>
    /// How a file's samples are laid out, which the builder that writes it is chosen by. Of either, the format - MP4,
    /// QuickTime, 3GP and the rest - is <see cref="VideoBuilderOptions.FileFormat"/>.
    /// </summary>
    public enum Mp4Layout
    {
        /// <summary>
        /// One 'moov' and one 'mdat', written at <see cref="IMp4Builder.FinalizeMedia"/>, the 'moov' first: <see cref="Mp4Builder"/>.
        /// </summary>
        Progressive,

        /// <summary>
        /// An initialization segment, then a 'moof' and an 'mdat' a fragment, written as the samples come:
        /// <see cref="FragmentedMp4Builder"/>.
        /// </summary>
        Fragmented,
    }

    /// <summary>
    /// What <see cref="VideoBuilder.Create"/> makes a builder with. Those of one layout alone are null unless set, and set,
    /// make a builder of the other layout throw.
    /// </summary>
    public class VideoBuilderOptions
    {
        /// <summary>The format of the file: MP4 unless set. <see cref="IMp4Builder.FileFormat"/>.</summary>
        public Mp4FileFormat FileFormat { get; set; } = Mp4FileFormat.Mp4;

        /// <summary>The movie's timescale; the builder's own, 1000, unless set.</summary>
        public uint? MovieTimescale { get; set; }

        /// <summary>The logger; the builder's own, <see cref="DefaultMp4Logger"/>, unless set.</summary>
        public IMp4Logger Logger { get; set; }

        /// <summary>Where the samples wait until they are written; temporary files unless set. <see cref="Mp4BuilderBase.TemporaryStorageFactory"/>.</summary>
        public ITemporaryStorageFactory TemporaryStorageFactory { get; set; }

        /// <summary>The time the headers say. <see cref="Mp4BuilderBase.CreationTime"/>.</summary>
        public DateTime? CreationTime { get; set; }

        /// <summary>Of <see cref="Mp4Layout.Progressive"/>: <see cref="Mp4Builder.NegativeCompositionOffsets"/>.</summary>
        public NegativeCompositionOffsets? NegativeCompositionOffsets { get; set; }

        /// <summary>Of <see cref="Mp4Layout.Progressive"/>: <see cref="Mp4Builder.ChunkDurationInMs"/>.</summary>
        public uint? ChunkDurationInMs { get; set; }

        /// <summary>Of <see cref="Mp4Layout.Progressive"/>: <see cref="Mp4Builder.Storage"/>.</summary>
        public IStorage Storage { get; set; }

        /// <summary>
        /// Of <see cref="Mp4Layout.Fragmented"/>: how long a fragment runs at least, in milliseconds; 2000 unless set.
        /// <see cref="FragmentedMp4Builder(IMp4Output, ulong, ulong, bool)"/>.
        /// </summary>
        public ulong? FragmentLengthInMs { get; set; }

        /// <summary>Of <see cref="Mp4Layout.Fragmented"/>: the movie's duration, where it is known; 0, of a live recording, unless set.</summary>
        public ulong? DurationInMs { get; set; }

        /// <summary>Of <see cref="Mp4Layout.Fragmented"/>: whether the 'mfra' is written at the end; it is unless set.</summary>
        public bool? AppendMovieFragmentRandomAccessBox { get; set; }

        /// <summary>Of <see cref="Mp4Layout.Fragmented"/>: <see cref="FragmentedMp4Builder.MaxFragmentDelayInMs"/>.</summary>
        public ulong? MaxFragmentDelayInMs { get; set; }
    }

    /// <summary>
    /// Makes the builder of a layout, as <see cref="Readers.VideoReader"/> reads a file of either: one place to start
    /// writing, whichever the file is to be.
    /// </summary>
    public static class VideoBuilder
    {
        /// <summary>The fragment length of a fragmented file whose options say none.</summary>
        public const ulong DefaultFragmentLengthInMs = 2000;

        /// <summary>
        /// A builder of the layout, of the options given.
        /// </summary>
        /// <param name="output">Where the file is written. <see cref="IMp4Output"/>.</param>
        /// <param name="layout">How the samples are laid out: the builder made.</param>
        /// <param name="options">The file's format, and the builder's settings; of none, an MP4 of the builder's defaults.</param>
        /// <exception cref="ArgumentException">An option is set that is of the other layout.</exception>
        public static IMp4Builder Create(IMp4Output output, Mp4Layout layout, VideoBuilderOptions options = null)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            options ??= new VideoBuilderOptions();

            Mp4BuilderBase builder;
            switch (layout)
            {
                case Mp4Layout.Progressive:
                    ThrowIfSet(layout, nameof(options.FragmentLengthInMs), options.FragmentLengthInMs.HasValue);
                    ThrowIfSet(layout, nameof(options.DurationInMs), options.DurationInMs.HasValue);
                    ThrowIfSet(layout, nameof(options.AppendMovieFragmentRandomAccessBox), options.AppendMovieFragmentRandomAccessBox.HasValue);
                    ThrowIfSet(layout, nameof(options.MaxFragmentDelayInMs), options.MaxFragmentDelayInMs.HasValue);

                    var progressive = new Mp4Builder(output);
                    if (options.NegativeCompositionOffsets.HasValue)
                        progressive.NegativeCompositionOffsets = options.NegativeCompositionOffsets.Value;
                    if (options.ChunkDurationInMs.HasValue)
                        progressive.ChunkDurationInMs = options.ChunkDurationInMs.Value;
                    if (options.Storage != null)
                        progressive.Storage = options.Storage;
                    builder = progressive;
                    break;

                case Mp4Layout.Fragmented:
                    ThrowIfSet(layout, nameof(options.NegativeCompositionOffsets), options.NegativeCompositionOffsets.HasValue);
                    ThrowIfSet(layout, nameof(options.ChunkDurationInMs), options.ChunkDurationInMs.HasValue);
                    ThrowIfSet(layout, nameof(options.Storage), options.Storage != null);

                    var fragmented = new FragmentedMp4Builder(output, options.FragmentLengthInMs ?? DefaultFragmentLengthInMs,
                        options.DurationInMs ?? 0, options.AppendMovieFragmentRandomAccessBox ?? true);
                    if (options.MaxFragmentDelayInMs.HasValue)
                        fragmented.MaxFragmentDelayInMs = options.MaxFragmentDelayInMs.Value;
                    builder = fragmented;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, "A layout is Progressive or Fragmented.");
            }

            builder.FileFormat = options.FileFormat;
            if (options.MovieTimescale.HasValue)
                builder.MovieTimescale = options.MovieTimescale.Value;
            if (options.Logger != null)
                builder.Logger = options.Logger;
            if (options.TemporaryStorageFactory != null)
                builder.TemporaryStorageFactory = options.TemporaryStorageFactory;
            builder.CreationTime = options.CreationTime;
            return builder;
        }

        private static void ThrowIfSet(Mp4Layout layout, string option, bool isSet)
        {
            if (isSet)
                throw new ArgumentException($"{option} is not of the {layout} layout: a builder of it has nothing to set it on.", "options");
        }
    }
}
