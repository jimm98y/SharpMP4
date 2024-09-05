# SharpMP4
Simple lightweight fragmented mp4 (fmp4) reader/writer. Supports H264/H265 for video and AAC/Opus for audio. No platform dependencies, easily portable cross-platform. It was designed to be a stream-in and stream-out solution for recording streams from IP cameras into fragmented MP4.

## Read fragmented MP4
To parse an existing fmp4 file, first you have to get the stream:
```cs
using (Stream fs = new BufferedStream(new FileStream("frag_bunny.mp4", FileMode.Open, FileAccess.Read, FileShare.Read)))
{
    ...
}
```
Pass the stream to the `FragmentedMp4.ParseAsync` and you will get an in-memory representation of the fmp4:
```cs 
using (var fmp4 = await FragmentedMp4.ParseAsync(fs))
{
    ...
}
```
You can examine all the boxes, VPS/SPS/PPS, or parse the MDAT to get NALs and/or audio samples. You should also be able to use it on parts of the fragmented MP4, e.g. to only read the media initialization (MOOV) or media fragments (MOOF + MDAT).

## Write fragmented MP4
To write `FragmentedMP4` into a file, you first have to create the file and then call:
```cs
using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
{
    await FragmentedMp4.BuildAsync(fmp4, output);
}
```
This allows you to modify any boxes, add/remove tracks, modify VPS/SPS/PPS and then save the modified file.

## Build fragmented MP4
While you could use just the `FragmentedMp4` and build all the boxes manually, there is a helper class `FragmentedMp4Builder` to assist you with this task. First create the output stream, then wrap it inside one of the `IMp4Output` outputs (more on that later, here we just use the simplest `SingleStreamOutput`) and pass it to `FragmentedMp4Builder`:
```cs
using (Stream output = new BufferedStream(new FileStream("frag_bunny_out.mp4", FileMode.Create, FileAccess.Write, FileShare.Read)))
{
    using (FragmentedMp4Builder builder = new FragmentedMp4Builder(new SingleStreamOutput(output)))
    {
        ...
    }
}
```
Next you have to specify which tracks your fmp4 is going to have. Currently supported tracks include H264, H265, AAC and Opus. 
Add H264 track:
```cs
var videoTrack = new H264Track();
builder.AddTrack(videoTrack);
```
Add AAC track for AAC-LC 2 channels (stereo), 22050Hz and 16-bit samples:
```cs
var audioTrack = new AACTrack(2, 22050, 16);
builder.AddTrack(audioTrack);
```
Start adding individual NALUs to H264/H265 tracks:
```cs
await videoTrack.ProcessSampleAsync(sample);
```
Start adding AAC frames to AAC track:
```cs
await audioTrack.ProcessSampleAsync(frame);
```
The default setting of the `FragmentedMp4Builder` is to produce samples of 0.5 seconds and fragments with 8 samples per fragment (4 seconds of play time). Once there is enough samples in both tracks, fragments will be written to the file and you can simultaneously start playing it.

## Video analysis
You can use the `H264SpsNalUnit`, `H264PpsNalUnit`, `H265VpsNalUnit`, `H265SpsNalUnit` and `H265PpsNalUnit`classes standalone to just parse and modify VPS/SPS/PPS from your H264/H265 video:
```cs
byte[] sps = ...
var parsedSPS = H264SpsNalUnit.Parse(sps);
```

## Extensibility
### Logging
There is a `Log` class where you can supply your own delegates for all the actions like:
```cs
Log.SinkWarn = (message, exception) => 
{
    ...
};
```
You can also enable/disable different trace levels like:
```cs
Log.WarnEnabled = false;
```
### Temporary Storage
By default the parser is using in-memory temporary storage, which means all parsed data are loaded in RAM. You can easily change this behavior and forward them to a temporary file:
```cs
TemporaryStorage.Factory = new TemporaryFileStorageFactory();
```
Or you can implement `ITemporaryStorageFactory` interface and create your own temporary storage.
### Output
There are multiple ways to output the encoded video:
- Single file where the output will have MOOV + MOOF + MDAT
```cs
new SingleStreamOutput(outputStream);
```
- Multiple files, split into the initialization segment (MOOV) and media fragments (MOOF + MDAT)
```cs
new MultiStreamFileOutput("C:\\Temp", "output", "mp4");
```
- In-memory BLOB with an event notification when a new BLOB is available
```cs
var blobOutput = new FragmentedBlobOutput()
blobOutput.OnFragmentReady += (blobEventArgs) => {
    ...
};
```
- A custom output by implementing the `IMp4Output` interface

### Custom boxes
It is possible to extend the currently supported boxes by extending `Mp4Box` class and providing your own implementation of the parser:
```cs
public class CustomBox : Mp4Box
{
    public const string TYPE = "cust";
    public CustomBox(uint size, string type, Mp4Box parent) : base(size, type, parent)
    { }

    public static async Task<Mp4Box> ParseAsync(uint size, ulong largeSize, string type, Mp4Box parent, Stream stream)
    {
        return new CustomBox(size, type, parent);
    }

    public static async Task<uint> BuildAsync(Mp4Box box, Stream stream)
    {
        CustomBox b = (CustomBox)box;
        return 0;
    }

    public override uint CalculateSize()
    {
        return (uint)(base.CalculateSize() + 0);
    }
}
```
Then you can register the custom box by calling:
```cs
SharpMp4.Mp4Parser.RegisterBox("cust", CustomBox.ParseAsync, CustomBox.BuildAsync);
```
### Custom descriptors
Similar as boxes, you can write custom descriptors by extending the `DescriptorBase` class. Then you can register custom descriptors by calling:
```cs
SharpMp4.Mp4Parser.RegisterDescriptor(objectTypeIndication, type, CustomDescriptor.ParseAsync, CustomDescriptor.BuildAsync);
```

## Future development
- Add support for building MP4
- Add H265 SPS extensions
- Add H265 PPS extensions
- AudioSpecificConfigDescriptor extensions and configuration

## Credits
Huge inspiration for this project was the `mp4parser` https://github.com/sannies/mp4parser, thank you very much!