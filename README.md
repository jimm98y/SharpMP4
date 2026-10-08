# SharpMP4
Simple lightweight mp4/fmp4/mov/m4v reader/writer. Supports H261/H262/H263/MPEG4/H264/H265/H266/VP9/AV1/AV2/ProRes for video and AAC/Opus/MP3/FLAC/ALAC/AC-3/E-AC-3/PCM for audio. No platform dependencies, easily portable cross-platform. It was designed to be a stream-in and stream-out solution for recording streams from IP cameras into MP4 and fragmented MP4.

[![NuGet version](https://img.shields.io/nuget/v/SharpMP4.svg?style=flat-square)](https://www.nuget.org/packages/SharpMP4)

## Supported boxes
The list of all supported boxes, entries and descriptors is [here](Boxes.md).

## Read MP4
To parse an existing mp4/mov/m4v file, first you have to get the stream:
```cs
using (Stream inputFileStream = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    ...
}
```
Create a new `Container` and call `Read` to get the in-memory representation of all the boxes.
```cs 
var mp4 = new Container();
mp4.Read(new IsoStream(inputFileStream));    ...

```
To process this in-memory representation and read the audio/video samples, create a new `VideoReader`:
```cs
VideoReader videoReader = new VideoReader();
videoReader.Parse(mp4);
```
Now it is possible to get all the tracks from the video:
```cs
IEnumerable<ITrack> tracks = videoReader.GetTracks();
```
To read the first track samples, call:
```cs
uint trackID = tracks.First().TrackID;
MediaSample sample = videoReader.ReadSample(trackID);
```
Where `trackID` is the ID of the track you want to read.

## Build MP4
To write `MP4` into a file, you first have to create the file:
```cs
using (Stream output = new BufferedStream(new FileStream("bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
{
    ...
}
```
Next, create the builder depending upon the output format. Currently, there are two builders - `FragmentedMp4Builder` and `Mp4Builder`.
```cs
IMp4Builder outputBuilder = new Mp4Builder(new SingleStreamOutput(output));
```
For fragmented MP4 output, use `FragmentedMp4Builder` with the fragment duration in miliseconds:
```cs
// use fragment duration 2 seconds
IMp4Builder outputBuilder = new FragmentedMp4Builder(new SingleStreamOutput(output), 2000);
```
To write another format of the MP4 family - `.m4v`, `.m4a`, `.m4b`, `.3gp`, `.3g2` or `.mov` - set `FileFormat` before adding the tracks. The `ftyp` then says what the file is, and the builder refuses a track the format cannot hold, such as video in an `.m4a` or a second audio track in a 3GP file of the Basic profile:
```cs
outputBuilder.FileFormat = Mp4FileFormat.M4A;
```

Add the H264 video track to the builder instance:
```cs
var videoTrack = new H264Track();
outputBuilder.AddTrack(videoTrack);
```
Add the AAC audio track to the builder instance:
```cs
var audioTrack = new AACTrack(2, 44100, 16);
outputBuilder.AddTrack(audioTrack);
```
Pass the track samples to the builder as follows:
```cs
byte[] nalu = ...;
outputBuilder.ProcessTrackSample(videoTrack.TrackID, nalu);
...
byte[] aac = ...;
outputBuilder.ProcessTrackSample(audioTrack.TrackID, aac);
```
`ProcessTrackSample` takes H.264/H.265/H.266 NAL units without their start codes. To pass the Annex B byte stream as an encoder hands it out - one or more NAL units, each behind a start code - use `ProcessAnnexBTrackSample`, which strips the start codes:
```cs
byte[] annexB = ...; // 00 00 00 01 67 ... 00 00 00 01 68 ... 00 00 01 65 ...
outputBuilder.ProcessAnnexBTrackSample(videoTrack.TrackID, annexB);
```
The other audio tracks are made of their codec's configuration, as an encoder hands it out or a file has it, and take a frame a sample:
```cs
var flac = new FlacTrack(FlacTrack.ParseStreamHeader(header)); // 'fLaC' and the metadata blocks, STREAMINFO first
var alac = new AlacTrack(alacSpecificConfig);                   // the 24 byte magic cookie
var mp3 = new Mp3Track(2, 44100);                              // MPEG-1 or MPEG-2 audio, by the rate
var ac3 = AC3Track.TryParseSyncFrame(frame, 0, out var track) ? track : null; // of a sync frame's bit stream information
var eac3 = new EAC3Track(dataRate, substreams);                 // 'dec3''s data rate and independent substreams
```
An MP3 encoder hands out one frame or several at a time: `Mp3Track.ParseFrames` splits them, a sample each.

PCM is made of its format and written as QuickTime's `lpcm`, as Apple's devices write it. It takes a block of frames at a time and writes a sample of each, as QuickTime has it. Reading it, QuickTime's `lpcm`, `sowt`, `twos`, `raw `, `in24`, `in32`, `fl32` and `fl64` and ISO's `ipcm` and `fpcm` are read, and `ReadSamples` reads a run of frames at once rather than a read of each:
```cs
var pcm = new PcmTrack(48000, 2, 16);                           // rate, channels, bits; isFloat and isLittleEndian optional
outputBuilder.ProcessTrackSample(pcm.TrackID, block, block.Length / pcm.BytesPerFrame);
var run = reader.ReadSamples(trackID, 4800);                    // up to 4800 frames, their duration together
```

When done, call `FinalizeMedia` to create the video file:
```cs
outputBuilder.FinalizeMedia();
```
## Extensibility
### Logging
There is an `IMp4Logger` interface exposed on the `Mp4Builder`, `FragmentedMp4Builder`, `VideoReader` and `ImageReader` instances where you can supply your own logger implementation, or use an existing one:
```cs
mp4Builder.Logger = new ConsoleMp4Logger();
```
You can also enable/disable different trace levels like:
```cs
mp4Builder.Logger.IsWarnEnabled = false;
```

## Conformance bitstreams
The published conformance suites for H.264, H.265, H.266 and AV1 can be downloaded for testing the parsers against:
```powershell
.\DownloadConformance.ps1                            # the core sets for all four codecs, about 4.8 GB
.\DownloadConformance.ps1 -Codec H265, H266          # only some codecs
.\DownloadConformance.ps1 -IncludeSvc -IncludeArgon  # also H.264 SVC (12.9 GB) and AV1 Argon Streams (7 GB)
```
They go into the `conformance` folder, which git ignores. Running the script again skips what is already there. The reference decoder's output - decoded pictures, traces and logs, which make the H.264 suites several times their download size - is left out unless `-KeepReference` is given. It works in Windows PowerShell 5.1 and PowerShell 7.

## Credits
Huge inspiration for this project was the `mp4parser` https://github.com/sannies/mp4parser, thank you very much!
