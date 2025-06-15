using SharpISOBMFF;
using System;

namespace SharpMP4.Tracks
{
    /// <summary>
    /// H266 track.
    /// </summary>
    /// <remarks>https://www.itu.int/rec/T-REC-H.266/en</remarks>
    public class H266Track : TrackBase
    {
        public override string HandlerName => HandlerNames.Video;

        public override string HandlerType => HandlerTypes.Video;

        public override string Language { get; set; } = "und";
        
        /// <summary>
        /// Ctor.
        /// </summary>
        public H266Track()
        {
            // TODO    
        }

        public override Box CreateSampleEntryBox()
        {
            throw new NotImplementedException();
        }

        public override void FillTkhdBox(TrackHeaderBox tkhd)
        {
            throw new NotImplementedException();
        }
    }
}
