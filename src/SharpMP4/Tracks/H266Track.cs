using SharpISOBMFF;
using System;
using System.Linq;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H266 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.266/en</remarks>
    public class H266Track : TrackBase, IH26XTrack
    {
        public override string HandlerName => HandlerNames.Video;
        public override string HandlerType => HandlerTypes.Video;
        public override string Language { get; set; } = "und";
        public int NalLengthSize { get; set; } = 4;


        /// <summary>
        /// Ctor.
        /// </summary>
        public H266Track()
        {
            // TODO    
        }

        public H266Track(Box sampleEntry, uint timescale, int sampleDuration) : this()
        {
            Timescale = timescale;
            DefaultSampleDuration = sampleDuration;

            VisualSampleEntry visualSampleEntry = (VisualSampleEntry)sampleEntry;
            VvcConfigurationBox vvcC = visualSampleEntry.Children.OfType<VvcConfigurationBox>().Single();

            NalLengthSize = vvcC._VvcConfig._LengthSizeMinusOne + 1; // usually 4 bytes

            foreach (var nalus in vvcC._VvcConfig.NalUnit)
            {
                foreach (var nalu in nalus)
                {
                    ProcessSample(nalu, out _, out _);
                }
            }

            // TODO SampleDuration
        }

        public override Box CreateSampleEntryBox()
        {
            throw new NotImplementedException();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            throw new NotImplementedException();
        }

        public byte[][] GetVideoNALUs()
        {
            throw new NotImplementedException();
        }
    }
}
