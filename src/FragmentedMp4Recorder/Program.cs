using System;
using FragmentedMp4Recorder;
using SharpISOBMFF;
using SharpMP4.Builders;
using SharpMP4.Readers;
using SharpMP4.Tracks;
using System.Collections.Generic;
using System.IO;

var logger = ConsoleWithoutInfoDebugLogger.Instance;

using (Stream inputFileStream = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    var fmp4 = new Container(logger);
    fmp4.Read(new IsoStream(inputFileStream) { Logger = logger });

    // the reader disposes the tracks it made; the clones written below are ours to dispose
    using VideoReader inputReader = new VideoReader(logger);
    inputReader.Parse(fmp4);
    IEnumerable<ITrack> inputTracks = inputReader.GetTracks();

    using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
    {
        IMp4Builder outputBuilder = new FragmentedMp4Builder(new SingleStreamOutput(output), 2000);
        Dictionary<uint, uint> mapping = new Dictionary<uint, uint>();
        List<ITrack> outputTracks = new List<ITrack>();

        foreach (var inputTrack in inputTracks)
        {
            var outputTrack = inputTrack.Clone();
            outputTracks.Add(outputTrack);
            outputBuilder.AddTrack(outputTrack);
            mapping.Add(inputTrack.TrackID, outputTrack.TrackID);
        }

        // subtitles: 3GPP timed text, in milliseconds - forced, so that players such as VLC show them without being asked
        var subtitleTrack = new TimedTextTrack(1000) { Language = "eng", Forced = true };
        outputTracks.Add(subtitleTrack);
        outputBuilder.AddTrack(subtitleTrack);

        // how long the movie is: its longest track
        double durationInSeconds = 0;

        foreach (var inputTrack in inputTracks)
        {
            long trackDuration = 0;
            if (inputTrack.HandlerType == HandlerTypes.Video)
            {
                var videoUnits = inputTrack.GetContainerSamples();
                foreach (var unit in videoUnits)
                {
                    outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], unit);
                }

                MediaSample sample = null;
                while ((sample = inputReader.ReadSample(inputTrack.TrackID)) != null)
                {
                    trackDuration += sample.Duration;
                    IEnumerable<ArraySegment<byte>> units = inputReader.ParseSample(inputTrack.TrackID, sample.Data);
                    foreach (var unit in units)
                    {
                        outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], unit);
                    }
                }
            }
            else
            {
                MediaSample sample = null;
                while ((sample = inputReader.ReadSample(inputTrack.TrackID)) != null)
                {
                    trackDuration += sample.Duration;
                    outputBuilder.ProcessTrackSample(mapping[inputTrack.TrackID], sample.Data, sample.Duration);
                }
            }

            durationInSeconds = Math.Max(durationInSeconds, (double)trackDuration / inputTrack.Timescale);
        }

        // a subtitle shown for 3 seconds every 6 seconds, over the whole movie: a sample of the cue, then an empty one for
        // the 3 seconds nothing is shown - each sample lasts until the next starts
        const int shownMs = 3000;
        const int periodMs = 6000;
        long durationMs = (long)Math.Ceiling(durationInSeconds * 1000);
        int cueNumber = 1;
        for (long start = 0; start < durationMs; start += periodMs)
        {
            int shown = (int)Math.Min(shownMs, durationMs - start);
            var cue = new SubtitleCue { Text = $"Sample subtitle {cueNumber++}\nat {TimeSpan.FromMilliseconds(start):m\\:ss}" };
            outputBuilder.ProcessTrackSample(subtitleTrack.TrackID, subtitleTrack.CreateSample(new[] { cue }), shown);

            int hidden = (int)Math.Min(periodMs - shownMs, durationMs - start - shown);
            if (hidden > 0)
            {
                outputBuilder.ProcessTrackSample(subtitleTrack.TrackID, subtitleTrack.CreateSample(Array.Empty<SubtitleCue>()), hidden);
            }
        }

        outputBuilder.FinalizeMedia();
        outputBuilder.Dispose();
        foreach (var outputTrack in outputTracks)
            outputTrack.Dispose();
    }
}
