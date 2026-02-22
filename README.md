# SharpMP4
Simple lightweight mp4/fmp4/mov/m4v reader/writer. Supports H264/H265/H266/AV1 for video and AAC/Opus for audio. No platform dependencies, easily portable cross-platform. It was designed to be a stream-in and stream-out solution for recording streams from IP cameras into MP4 and fragmented MP4.

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

## Credits
Huge inspiration for this project was the `mp4parser` https://github.com/sannies/mp4parser, thank you very much!