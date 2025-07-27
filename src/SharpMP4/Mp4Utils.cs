using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMP4
{
    public static class Mp4Utils
    {
        public static int TrackIdToTrackIndex(uint trackID)
        {
            return (int)trackID - 1;
        }

        public static uint TrackIndexToTrackId(int trackIndex)
        {
            return (uint)(trackIndex + 1);
        }
    }
}
