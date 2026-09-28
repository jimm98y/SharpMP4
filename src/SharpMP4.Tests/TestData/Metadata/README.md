# Metadata test files

Made for SharpMP4's tests: a short clip from FFmpeg's test sources, and tags written into it by ExifTool.
Nothing in them is anyone else's. MetadataReaderTests reads them; with ExifTool present
(DownloadConformance.ps1 -Codec Metadata), MetadataReadsAsExifToolReadsIt compares them with what ExifTool
reads.

```sh
ffmpeg -f lavfi -i color=c=gray:s=64x64:d=0.2 -f lavfi -i sine=d=0.2 -c:v libx264 -c:a aac -shortest base.mp4

# itunes-items.mp4: iTunes items SharpISOBMFF had as bytes (GUID, VERS, rate, rldt, @pti, ©arg...) or not at all (ownr)
exiftool -ItemList:Grouping=Group1 -ItemList:ReleaseDate=2020:01:02 -ItemList:RatingPercent=80 \
  -ItemList:GUID=guid-1 -ItemList:ProductVersion=v1 -ItemList:ProductID=pid-1 -ItemList:Arranger=Arr \
  -ItemList:Director=Dir -ItemList:Producer=Prod -ItemList:ParentTitle=Parent -ItemList:ShortTitle=Short \
  -ItemList:Owner=Me -ItemList:Subtitle=sub itunes-items.mp4

# user-data-strings.mp4: cameras' strings in 'udta', and the 3GPP collection name
exiftool -UserData:CameraAngle=front -UserData:ReelName=reel1 -UserData:Scene=sc1 -UserData:ShotName=shot1 \
  -UserData:ClipID=clip1 -UserData:SerialNumber=SN1 -UserData:CollectionName=coll1 \
  -UserData:FirmwareVersion=fw1 -UserData:ClipFileName=cf1 user-data-strings.mp4

# xmp.mp4: XMP in the XMP Specification's 'uuid' box - language alternatives, arrays, structures, rationals
exiftool '-XMP-dc:Title=Default title' '-XMP-dc:Title-fr=Titre' '-XMP-dc:Title-de-DE=Titel' \
  -XMP-dc:Subject=one -XMP-dc:Subject=two -XMP-dc:Subject=three -XMP-dc:Creator=Alice -XMP-dc:Creator=Bob \
  '-XMP-dc:Description=A description' '-XMP-dc:Rights=(c) nobody' \
  -XMP-iptcCore:CreatorWorkEmail=a@b.c -XMP-iptcCore:CreatorCity=Prague \
  -XMP-exif:FNumber=2.8 -XMP-exif:ExposureTime=1/125 -XMP-exif:FocalLength=35 \
  -XMP-xmp:Rating=4 -XMP-xmp:Label=Red '-XMP-xmp:CreateDate=2021:05:06 07:08:09+02:00' \
  -XMP-photoshop:City=Brno -XMP-xmpRights:Marked=True \
  -XMP-mwg-rs:RegionAppliedToDimensionsW=100 -XMP-mwg-rs:RegionAppliedToDimensionsH=50 \
  -XMP-mwg-rs:RegionName=face -XMP-mwg-rs:RegionType=Face -XMP-mwg-rs:RegionAreaX=0.5 xmp.mp4
```

Each exiftool command ran on a copy of base.mp4 (ExifTool 13.59, FFmpeg's libavformat 62.1.103).
