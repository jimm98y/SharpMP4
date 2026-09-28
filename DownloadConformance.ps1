<#
.SYNOPSIS
Downloads the conformance bitstreams and files the parsers are tested against.

.DESCRIPTION
Fetches the published conformance suites for H.264, H.265, H.266 and AV1 into the
conformance folder next to this script, which git ignores. Each suite is laid out as

    conformance\<codec>\<set>\<stream or archive name>\...

Archives are unpacked into a folder of their own and then deleted, unless
-KeepArchives is given. Anything already downloaded is skipped, so the script can
be run again to finish an interrupted download or to add a suite.

What the reference decoder produced is left out unless -KeepReference is given: the
decoded pictures, its traces and its logs. They are what makes the H.264 suites
several times their download size - 30 GB of them for 1.5 GB of bitstreams - and
nothing that parses headers needs them. The bitstreams, their MD5 checksums and the
descriptions are kept.

Sources:
    H.264  ITU-T JVT     https://www.itu.int/wftp3/av-arch/jvt-site/draft_conformance/
    H.265  ITU-T JCT-VC  https://www.itu.int/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/
    H.266  ITU-T JVET    https://www.itu.int/wftp3/av-arch/jvet-site/bitstream_exchange/VVC/FDIS_r1/
    AV1    libaom test vectors  https://storage.googleapis.com/aom-test-data/ (av1-1-*)
    AV1    Argon Streams        https://aomedia.org/av1-video-decoder-verification-tool/
    ISOBMFF  MPEG file format conformance (ISO/IEC 14496-32)
                                https://github.com/MPEGGroup/FileFormatConformance
    FATE     FFmpeg's samples that are ISOBMFF or QuickTime files
                                https://fate-suite.ffmpeg.org/
    Metadata The MP4 and QuickTime test files of TagLib, mutagen and ExifTool, and ExifTool
             itself to compare the tags read with
                                https://github.com/taglib/taglib, https://github.com/quodlibet/mutagen,
                                https://github.com/exiftool/exiftool
    Chromium The MP4 files Chromium's media stack is tested with: fragmented, encrypted (CENC,
             cbcs), HDR, Dolby Vision, AC-4, and broken on purpose
                                https://github.com/chromium/chromium/tree/main/media/test/data
    Firefox  The MP4 files Firefox's media tests use: DASH segments, encrypted, AV1, HEVC, and the
             files of bug reports and crash tests
                                https://github.com/mozilla-firefox/firefox/tree/main/dom/media/test
    Libavif  The AVIF files libavif is tested with: still images and sequences, alpha, grids, gain
             maps, sample transforms, and files broken on purpose
                                https://github.com/AOMediaCodec/libavif/tree/main/tests/data
    Avif     The AVIF specification's test files, from Apple, Link-U, Microsoft, Netflix and Xiph:
             still images, grids, alpha, HDR, image sequences
                                https://github.com/AOMediaCodec/av1-avif/tree/main/testFiles
    Exiv2    The ISOBMFF files Exiv2 is tested with: HEIF, AVIF, CR3, JPEG XL and video with Exif
             and XMP, and the files of security reports
                                https://github.com/Exiv2/exiv2/tree/main/test/data
    Mp4parse The MP4 and AVIF files Firefox's parser, mp4parse, is tested with: each made for a case,
             corrupt ones among them
                                https://github.com/mozilla/mp4parse-rust
    Shaka    The MP4 files Shaka Player is tested with: DASH and HLS segments, CMAF text, encrypted
             (CENC, cbcs), AC-3 and E-AC-3, LCEVC, and live streams
                                https://github.com/shaka-project/shaka-player/tree/main/test/test/assets

Only files directly in each set's folder are fetched: the subfolders the ITU keeps
beside them hold superseded versions of the same streams.

Works in Windows PowerShell 5.1 and PowerShell 7.

.PARAMETER Destination
Where the suites go. The conformance folder next to this script by default.

.PARAMETER Codec
Which suites to fetch: any of H264, H265, H266, AV1, IsoBmff (the file format
conformance files, each with GPAC's dump of its boxes), Fate (FFmpeg's samples
that are ISOBMFF or QuickTime files, about 140 MB) and Metadata (the test files of the
metadata libraries, under 1 MB, and ExifTool, 9 MB), Chromium (the MP4 files of Chromium's
media tests, 43 MB), Firefox (those of Firefox's, 18 MB), Libavif (libavif's AVIF files, 2 MB),
Avif (the AVIF specification's test files, 50 MB), Exiv2 (Exiv2's ISOBMFF files, 6 MB), Mp4parse
(those of Firefox's parser, 12 MB) and Shaka (Shaka Player's MP4 files, 84 MB). All of them by default.

.PARAMETER IncludeSvc
Also fetches the H.264 scalable video coding set, 12.9 GB.

.PARAMETER IncludeArgon
Also fetches Argon Streams AV1, the AOMedia coverage suite, 7 GB.

.PARAMETER KeepArchives
Keeps each archive after unpacking it.

.PARAMETER KeepReference
Unpacks the reference decoder's output too: decoded pictures, traces and logs.

.EXAMPLE
.\DownloadConformance.ps1
The core sets for all four codecs, about 4.8 GB.

.EXAMPLE
.\DownloadConformance.ps1 -Codec H265, H266
Only H.265 and H.266.

.EXAMPLE
.\DownloadConformance.ps1 -IncludeSvc -IncludeArgon
Everything, about 25 GB.
#>
[CmdletBinding()]
param(
    [string]$Destination,

    [ValidateSet('H264', 'H265', 'H266', 'AV1', 'IsoBmff', 'Fate', 'Metadata', 'Chromium', 'Firefox', 'Libavif', 'Avif', 'Exiv2', 'Mp4parse', 'Shaka')]
    [string[]]$Codec = @('H264', 'H265', 'H266', 'AV1', 'IsoBmff', 'Fate', 'Metadata', 'Chromium', 'Firefox', 'Libavif', 'Avif', 'Exiv2', 'Mp4parse', 'Shaka'),

    [switch]$IncludeSvc,
    [switch]$IncludeArgon,
    [switch]$KeepArchives,
    [switch]$KeepReference
)

$ErrorActionPreference = 'Stop'

# Windows PowerShell 5.1 has no $PSScriptRoot yet where parameter defaults are worked out.
if (-not $Destination) { $Destination = Join-Path $PSScriptRoot 'conformance' }

# Windows PowerShell 5.1 may still offer only TLS 1.0 and 1.1, which the servers refuse.
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12

# The progress bar slows Invoke-WebRequest down by an order of magnitude in 5.1.
$ProgressPreference = 'SilentlyContinue'

Add-Type -AssemblyName System.IO.Compression.FileSystem

$itu = 'https://www.itu.int'

# The sets to fetch per codec, as ITU directories, and the name each is kept under.
$ituSets = @{
    'H264' = @(
        @{ Name = 'AVCv1'; Url = '/wftp3/av-arch/jvt-site/draft_conformance/AVCv1/' },
        @{ Name = 'FRExt'; Url = '/wftp3/av-arch/jvt-site/draft_conformance/FRExt/' },
        @{ Name = 'MVC'; Url = '/wftp3/av-arch/jvt-site/draft_conformance/MVC/' },
        @{ Name = 'Professional_profiles'; Url = '/wftp3/av-arch/jvt-site/draft_conformance/Professional_profiles/' }
    )
    'H265' = @(
        @{ Name = 'HEVC_v1'; Url = '/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/HEVC_v1/' },
        @{ Name = 'RExt'; Url = '/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/RExt/' },
        @{ Name = 'MV-HEVC'; Url = '/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/MV-HEVC/' },
        @{ Name = 'SCC'; Url = '/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/SCC/' },
        @{ Name = 'SHVC'; Url = '/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/SHVC/' },
        @{ Name = '3D-HEVC'; Url = '/wftp3/av-arch/jctvc-site/bitstream_exchange/draft_conformance/3D-HEVC/' }
    )
    'H266' = @(
        @{ Name = 'FDIS_r1'; Url = '/wftp3/av-arch/jvet-site/bitstream_exchange/VVC/FDIS_r1/' }
    )
}

if ($IncludeSvc) {
    $ituSets['H264'] += @{ Name = 'SVC'; Url = '/wftp3/av-arch/jvt-site/draft_conformance/SVC/' }
}

$argonUrl = 'https://aom-cwg-av1-argon-streams-public.s3.us-east-1.amazonaws.com/argon_coveragetool_av1_base_and_extended_profiles_v2.1.1.zip'

$script:fetched = 0
$script:skipped = 0
$script:failed = New-Object System.Collections.Generic.List[string]

function Get-Text([string]$Url) {
    # A server that labels text as binary gets bytes back from Windows PowerShell 5.1.
    $content = (Invoke-WebRequest -Uri $Url -UseBasicParsing).Content
    if ($content -is [byte[]]) { return [Text.Encoding]::UTF8.GetString($content) }
    return $content
}

# Downloads to a .part file and renames it once complete, so an interrupted download is
# never taken for a finished one. A file already there with the expected size is kept.
function Save-File([string]$Url, [string]$Path, [long]$ExpectedSize = -1) {
    if ((Test-Path -LiteralPath $Path) -and ($ExpectedSize -lt 0 -or (Get-Item -LiteralPath $Path).Length -eq $ExpectedSize)) {
        $script:skipped++
        return $true
    }

    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $Path) | Out-Null
    $part = "$Path.part"

    for ($attempt = 1; $attempt -le 3; $attempt++) {
        try {
            $client = New-Object System.Net.WebClient
            try { $client.DownloadFile($Url, $part) } finally { $client.Dispose() }

            if ($ExpectedSize -ge 0 -and (Get-Item -LiteralPath $part).Length -ne $ExpectedSize) {
                throw "expected $ExpectedSize bytes, got $((Get-Item -LiteralPath $part).Length)"
            }

            Move-Item -LiteralPath $part -Destination $Path -Force
            $script:fetched++
            return $true
        }
        catch {
            Write-Warning "  attempt $attempt of 3 failed for $Url : $($_.Exception.Message)"
            Remove-Item -LiteralPath $part -Force -ErrorAction SilentlyContinue
            Start-Sleep -Seconds (5 * $attempt)
        }
    }

    $script:failed.Add($Url)
    return $false
}

# Unpacks an archive into a folder named after it. A marker is written last, so a folder
# left half unpacked is unpacked again rather than taken as done.
function Expand-Stream([string]$Archive) {
    $folder = [IO.Path]::Combine((Split-Path -Parent $Archive), [IO.Path]::GetFileNameWithoutExtension($Archive))
    $marker = Join-Path $folder '.unpacked'

    if (-not (Test-Path -LiteralPath $marker)) {
        if (Test-Path -LiteralPath $folder) { Remove-Item -LiteralPath $folder -Recurse -Force }
        New-Item -ItemType Directory -Force -Path $folder | Out-Null
        $root = [IO.Path]::GetFullPath($folder).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar

        $zip = [IO.Compression.ZipFile]::OpenRead($Archive)
        try {
            foreach ($entry in $zip.Entries) {
                # A folder entry, or what the reference decoder produced.
                if ($entry.Name -eq '') { continue }
                if (-not $KeepReference -and (Test-Reference $entry.Name $entry.Length)) { continue }

                # An entry that would land outside its folder is left out.
                $target = [IO.Path]::GetFullPath([IO.Path]::Combine($folder, $entry.FullName))
                if (-not $target.StartsWith($root, [StringComparison]::OrdinalIgnoreCase)) { continue }

                New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
                [IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $target, $true)
            }
        }
        finally {
            $zip.Dispose()
        }

        Set-Content -LiteralPath $marker -Value (Split-Path -Leaf $Archive)
    }

    if (-not $KeepArchives) { Remove-Item -LiteralPath $Archive -Force }
}

# What the reference decoder produced, rather than what it was given: decoded pictures, its
# traces and logs, and text too large to be a description - the traces are text as well.
function Test-Reference([string]$Name, [long]$Length) {
    $extension = [IO.Path]::GetExtension($Name).ToLowerInvariant()
    if ($extension -in '.yuv', '.qcif', '.cif', '.rgb', '.rec', '.trc', '.log') { return $true }
    return ($extension -in '.txt', '.opl', '.csv', '.dat') -and $Length -gt 1MB
}

# Takes the reference decoder's output out of what an earlier run, or -KeepReference, unpacked.
function Remove-Reference {
    $freed = 0L
    Get-ChildItem -LiteralPath $Destination -Recurse -File | Where-Object { Test-Reference $_.Name $_.Length } | ForEach-Object {
        $freed += $_.Length
        Remove-Item -LiteralPath $_.FullName -Force
    }

    if ($freed -gt 0) { Write-Host ("Removed {0:N1} GB of decoded pictures, traces and logs." -f ($freed / 1GB)) }
}

function Test-Unpacked([string]$Archive) {
    $folder = [IO.Path]::Combine((Split-Path -Parent $Archive), [IO.Path]::GetFileNameWithoutExtension($Archive))
    return Test-Path -LiteralPath (Join-Path $folder '.unpacked')
}

# The files directly in an ITU directory listing, with their sizes. The listings are
# IIS pages: a date, a size or <dir>, then the link.
function Get-ItuListing([string]$Path) {
    $html = Get-Text ($itu + $Path)
    $entry = '(\d+/\d+/\d+\s+\d+:\d+\s+[AP]M)\s+(&lt;dir&gt;|\d+)\s+<A HREF="([^"]+)">([^<]+)</A>'

    foreach ($m in [regex]::Matches($html, $entry, 'IgnoreCase')) {
        if ($m.Groups[2].Value -notmatch 'dir') {
            [pscustomobject]@{
                Name = [Net.WebUtility]::HtmlDecode($m.Groups[4].Value)
                Url  = $itu + $m.Groups[3].Value
                Size = [long]$m.Groups[2].Value
            }
        }
    }
}

function Get-ItuSet([string]$CodecFolder, $Set) {
    $target = Join-Path (Join-Path $Destination $CodecFolder) $Set.Name
    $files = @(Get-ItuListing $Set.Url | Where-Object { $_.Name -notmatch '\.(html?|txt|doc|docx|xls|xlsx|pdf)$' })
    $megabytes = [math]::Round((($files | Measure-Object Size -Sum).Sum) / 1MB)
    Write-Host "$CodecFolder/$($Set.Name): $($files.Count) files, $megabytes MB"

    $i = 0
    foreach ($file in $files) {
        $i++
        $path = Join-Path $target $file.Name
        $isArchive = $file.Name -match '\.zip$'

        if ($isArchive -and (Test-Unpacked $path)) {
            $script:skipped++
            continue
        }

        Write-Host ("  [{0}/{1}] {2}" -f $i, $files.Count, $file.Name)
        if ((Save-File $file.Url $path $file.Size) -and $isArchive) {
            try { Expand-Stream $path }
            catch {
                Write-Warning "  could not unpack $($file.Name): $($_.Exception.Message)"
                $script:failed.Add($file.Url)
            }
        }
    }
}

# libaom's test vectors, listed through the storage bucket's JSON API, each checked against
# the MD5 the bucket keeps for it.
function Get-LibaomVectors {
    $target = Join-Path (Join-Path $Destination 'av1') 'libaom'
    $objects = New-Object System.Collections.Generic.List[object]
    $token = $null

    do {
        $url = 'https://storage.googleapis.com/storage/v1/b/aom-test-data/o?prefix=av1-1-&maxResults=1000&fields=items(name,size,md5Hash),nextPageToken'
        if ($token) { $url += '&pageToken=' + [Uri]::EscapeDataString($token) }
        $page = Invoke-RestMethod -Uri $url
        # The .orig files are backups of a few vectors, and not public.
        if ($page.items) { $objects.AddRange([object[]]@($page.items | Where-Object { $_.name -notmatch '\.orig$' })) }
        $token = $page.nextPageToken
    } while ($token)

    $bytes = 0L
    foreach ($object in $objects) { $bytes += [long]$object.size }
    $megabytes = [math]::Round($bytes / 1MB)
    Write-Host "av1/libaom: $($objects.Count) files, $megabytes MB"

    foreach ($object in $objects) {
        $path = Join-Path $target $object.name
        $url = 'https://storage.googleapis.com/aom-test-data/' + $object.name
        $known = Test-Path -LiteralPath $path

        if ((Save-File $url $path ([long]$object.size)) -and -not $known) {
            $expected = ([BitConverter]::ToString([Convert]::FromBase64String($object.md5Hash)) -replace '-', '').ToLowerInvariant()
            $actual = (Get-FileHash -LiteralPath $path -Algorithm MD5).Hash.ToLowerInvariant()
            if ($actual -ne $expected) {
                Write-Warning "  $($object.name) does not match its MD5; removed"
                Remove-Item -LiteralPath $path -Force
                $script:failed.Add($url)
            }
        }
    }
}

# Argon Streams: one archive, checked against the MD5 published beside it.
function Get-Argon {
    $target = Join-Path (Join-Path $Destination 'av1') 'argon'
    $archive = Join-Path $target (Split-Path -Leaf ([Uri]$argonUrl).AbsolutePath)

    if (Test-Unpacked $archive) {
        Write-Host 'av1/argon: already unpacked'
        $script:skipped++
        return
    }

    Write-Host 'av1/argon: 1 archive, about 7 GB'
    if (-not (Save-File $argonUrl $archive)) { return }

    # Published BSD style, "MD5 (name) = hash", so take the hash wherever it stands.
    $md5sum = Get-Text "$argonUrl.md5sum"
    if ($md5sum -notmatch '\b([0-9a-fA-F]{32})\b') {
        Write-Warning "  no MD5 in $argonUrl.md5sum"
        $script:failed.Add($argonUrl)
        return
    }
    $expected = $Matches[1].ToLowerInvariant()
    $actual = (Get-FileHash -LiteralPath $archive -Algorithm MD5).Hash.ToLowerInvariant()
    if ($actual -ne $expected) {
        Write-Warning '  the Argon archive does not match its MD5; removed'
        Remove-Item -LiteralPath $archive -Force
        $script:failed.Add($argonUrl)
        return
    }

    Expand-Stream $archive
}

# MPEG's file format conformance files: every published file with its description and GPAC's
# dump of its boxes (*_gpac.json). Listed through GitHub's tree API; the files themselves are in
# Git LFS, served without git-lfs from media.githubusercontent.com, and checked against the MD5
# their description gives.
function Get-FileFormatConformance {
    $repository = 'MPEGGroup/FileFormatConformance'
    $prefix = 'data/file_features/published/'
    $target = Join-Path $Destination 'isobmff'

    $tree = Invoke-RestMethod -Uri "https://api.github.com/repos/$repository/git/trees/main?recursive=1" -Headers @{ 'User-Agent' = 'DownloadConformance' }
    $blobs = @($tree.tree | Where-Object { $_.type -eq 'blob' -and $_.path.StartsWith($prefix) -and $_.path -notmatch '/(README\.md|\.cfignore)$' })
    Write-Host "isobmff: $($blobs.Count) files"

    foreach ($blob in $blobs) {
        $relative = $blob.path.Substring($prefix.Length)
        $path = Join-Path $target $relative.Replace('/', [IO.Path]::DirectorySeparatorChar)
        if ($relative.EndsWith('.json')) {
            [void](Save-File "https://raw.githubusercontent.com/$repository/main/$($blob.path)" $path)
            continue
        }

        $known = Test-Path -LiteralPath $path
        if ((Save-File "https://media.githubusercontent.com/media/$repository/main/$($blob.path)" $path) -and -not $known) {
            # The description beside a file is named after it without its extension.
            $description = [IO.Path]::Combine((Split-Path -Parent $path), [IO.Path]::GetFileNameWithoutExtension($path) + '.json')
            if ($relative.EndsWith('.zip')) {
                $description = [IO.Path]::Combine((Split-Path -Parent $path), [IO.Path]::GetFileNameWithoutExtension([IO.Path]::GetFileNameWithoutExtension($path)) + '.json')
            }
            $expected = $null
            if (Test-Path -LiteralPath $description) {
                $expected = (Get-Content -LiteralPath $description -Raw | ConvertFrom-Json).md5
            }
            if ($expected -and -not $relative.EndsWith('.zip')) {
                $actual = (Get-FileHash -LiteralPath $path -Algorithm MD5).Hash.ToLowerInvariant()
                if ($actual -ne $expected.ToLowerInvariant()) {
                    Write-Warning "  $relative does not match its MD5; removed"
                    Remove-Item -LiteralPath $path -Force
                    $script:failed.Add($blob.path)
                    continue
                }
            }
            if ($relative.EndsWith('.zip')) { Expand-Stream $path }
        }
    }
}

# FFmpeg's FATE samples that are ISOBMFF or QuickTime files: real files from cameras, phones and
# editors, with the metadata and vendor boxes no conformance suite has. The Apache listings are
# crawled for the extensions such files go by, and for files with none; a file is kept only if
# its first box is one a file can start with. FATE publishes no checksums to check them against.
$fateUrl = 'https://fate-suite.ffmpeg.org/'
$fateExtensions = '\.(mov|qt|mp4|m4a|m4v|m4b|3gp|3g2|heic|heif|hif|avif|mj2|f4v|ism|ismv|isma|cmfv|cmfa|mqv|psp|dvr)$'
$fateFirstBoxes = 'ftyp', 'styp', 'moov', 'mdat', 'free', 'skip', 'wide', 'pnot', 'uuid', 'sidx', 'moof', 'junk', 'PICT'

function Get-FateListing([string]$Path) {
    $html = Get-Text ($fateUrl + $Path)
    foreach ($m in [regex]::Matches($html, '<a href="([^"?/][^"?]*)">')) {
        $href = [Net.WebUtility]::HtmlDecode($m.Groups[1].Value)
        if ($href.EndsWith('/')) {
            Get-FateListing ($Path + $href)
        }
        elseif ($href -match $fateExtensions -or ($href -notmatch '\.' -and $href -ne 'md5sum')) {
            $Path + $href
        }
    }
}

function Test-IsoBmffStart([string]$Path) {
    $bytes = New-Object byte[] 8
    $stream = [IO.File]::OpenRead($Path)
    try { $read = $stream.Read($bytes, 0, 8) } finally { $stream.Dispose() }
    return $read -eq 8 -and [Text.Encoding]::ASCII.GetString($bytes, 4, 4) -cin $fateFirstBoxes
}

function Get-FateSuite {
    $target = Join-Path $Destination 'fate'
    $files = @(Get-FateListing '')
    Write-Host "fate: $($files.Count) candidates"

    # What turned out not to be such a file is listed here, so that the next run does not fetch it again.
    $rejectedList = Join-Path $target 'not-isobmff.txt'
    $rejected = @{}
    if (Test-Path -LiteralPath $rejectedList) {
        Get-Content -LiteralPath $rejectedList | ForEach-Object { $rejected[$_] = $true }
    }

    foreach ($relative in $files) {
        if ($rejected.ContainsKey($relative)) { $script:skipped++; continue }

        $path = Join-Path $target $relative.Replace('/', [IO.Path]::DirectorySeparatorChar)
        $known = Test-Path -LiteralPath $path
        $url = $fateUrl + (($relative.Split('/') | ForEach-Object { [Uri]::EscapeDataString($_) }) -join '/')
        if ((Save-File $url $path) -and -not $known -and -not (Test-IsoBmffStart $path)) {
            Remove-Item -LiteralPath $path -Force
            Add-Content -LiteralPath $rejectedList -Value $relative
        }
    }
}

# The MP4 and QuickTime files the metadata libraries test themselves against - the tags in iTunes,
# QuickTime, 3GPP and keyed metadata, and the odd ways files hold them - each at the commit given,
# so that the set does not change under the tests. They are fetched here, not kept in the
# repository: TagLib's are LGPL, mutagen's GPL, ExifTool's GPL or Artistic.
$metadataSources = @(
    @{ Name = 'taglib'; Repo = 'taglib/taglib'; Commit = 'f9efbc7ba8bc596332322073f81d9dc5d2066558'; Folder = 'tests/data' },
    @{ Name = 'mutagen'; Repo = 'quodlibet/mutagen'; Commit = 'ada28b2cc92c515f3f26640a6feef6516d195872'; Folder = 'tests/data' },
    @{ Name = 'exiftool'; Repo = 'exiftool/exiftool'; Commit = '2200871d9cef988051d2a99d67df3bda6cbb30a8'; Folder = 't/images' }
)
$metadataExtensions = '\.(mov|qt|mp4|m4a|m4v|m4b|3gp|3g2|heic|heif|avif|cr3|mqv|f4v)$'

# ExifTool, which reads every kind of these tags, to compare SharpMP4's reading with. Run as a program
# of its own, by the Perl Git for Windows comes with; a test tool, not a part of SharpMP4.
$exifToolCommit = '2200871d9cef988051d2a99d67df3bda6cbb30a8'

function Get-MetadataSet {
    foreach ($source in $metadataSources) {
        $api = "https://api.github.com/repos/$($source.Repo)/contents/$($source.Folder)?ref=$($source.Commit)"
        $listing = Get-Text $api | ConvertFrom-Json
        $files = @($listing | Where-Object { $_.type -eq 'file' -and $_.name -match $metadataExtensions })
        Write-Host "metadata/$($source.Name): $($files.Count) files"
        foreach ($file in $files) {
            $url = "https://raw.githubusercontent.com/$($source.Repo)/$($source.Commit)/$($source.Folder)/$($file.name)"
            Save-File $url (Join-Path (Join-Path (Join-Path $Destination 'metadata') $source.Name) $file.name) $file.size | Out-Null
        }
    }

    $tools = Join-Path $Destination 'tools'
    $archive = Join-Path $tools "exiftool-$exifToolCommit.zip"
    $folder = Join-Path $tools "exiftool-$exifToolCommit"
    if (-not (Test-Path -LiteralPath (Join-Path $folder 'exiftool'))) {
        if (Save-File "https://codeload.github.com/exiftool/exiftool/zip/$exifToolCommit" $archive) {
            [IO.Compression.ZipFile]::ExtractToDirectory($archive, $tools)
            if (-not $KeepArchives) { Remove-Item -LiteralPath $archive -Force }
        }
    }
    else {
        $script:skipped++
    }
}

$browserExtensions = '\.(mp4|m4a|m4s|m4v|mov|3gp|heic|heics|heif|avif|avifs|mj2)$'

# The MP4 files of Chromium's media tests (BSD-3-Clause), at the commit given: fragmented and
# segmented files, encryption (CENC, cbcs, key rotation), HEVC HDR, Dolby Vision, AC-4, AV1, and files
# broken on purpose.
$chromiumSet = @{ Name = 'chromium'; Repo = 'chromium/chromium'; Commit = '30c2a44f19b32e0dc50175f3fa392ec41bd741e6'; Folder = 'media/test/data' }

# The MP4 files of Firefox's media tests (dom/media/test, Mozilla's: MPL-2.0 unless a file says otherwise),
# at the commit given: DASH init and media segments, encrypted (CENC, key rotation) and clear-key files,
# AV1, HEVC, and the files of bug reports and crash tests - broken on purpose, or by what wrote them.
$firefoxSet = @{ Name = 'firefox'; Repo = 'mozilla-firefox/firefox'; Commit = 'b478a70dbe9b20189bb57f12c05bc0d6a8f323bd'; Folder = 'dom/media/test' }

# The AVIF files of libavif's tests (tests/data, BSD-2-Clause), at the commit given: items rather than
# tracks - still images, grids, alpha, gain maps, sample transforms - image sequences, and files broken on
# purpose. Its README says what each file is.
$libavifSet = @{ Name = 'libavif'; Repo = 'AOMediaCodec/libavif'; Commit = 'ef43c7be4167464eab9947267fb6e9a88584910f'; Folder = 'tests/data' }

# The test files of the AVIF specification (testFiles of AOMediaCodec/av1-avif), at the commit given, a folder
# for each company that gave them, under the licence it gives: Microsoft's and Xiph's as the repository,
# BSD-2-Clause; Apple's and Link-U's CC BY-SA 4.0; Netflix's CC BY-NC-ND 4.0 - read here, never changed or
# passed on. Other writers than libavif's and libheif's: HDR, 10 and 12 bit, grids, alpha, image sequences.
$avifSet = @{ Name = 'avif'; Repo = 'AOMediaCodec/av1-avif'; Commit = 'bf4c18d1f3971069b75e87d6ee469790589f4f09'; Folder = 'testFiles' }

# The ISOBMFF files of Exiv2's tests (test/data, Exiv2's: GPL-2.0-or-later), at the commit given: HEIF from
# Apple's, Canon's and Sony's cameras, AVIF and CR3 with their Exif and XMP, JPEG XL's box container, video,
# and the files of security reports - broken on purpose. Read here, never passed on. Camera extensions too,
# so a set of its own filter.
$exiv2Set = @{ Name = 'exiv2'; Repo = 'Exiv2/exiv2'; Commit = '7710fc07c30a9f968a80dcff448c82adee84720d'; Folder = 'test/data'
    Extensions = '\.(mp4|m4a|m4v|mov|3gp|heic|heif|hif|avif|avifs|cr3|crm|jxl)$' }

# The test files of mp4parse, Firefox's MP4 and AVIF parser (mozilla/mp4parse-rust, MPL-2.0), at the commit
# given: of the parser and of its C API, each in a folder of its own - some are in both - made for the case
# each tests, corrupt ones among them - not their zip, which holds three of them again - and a ClusterFuzz
# test case.
$mp4parseSets = @(
    @{ Name = 'mp4parse'; Into = 'mp4parse'; Repo = 'mozilla/mp4parse-rust'; Commit = '95d98b9a6840f5bfc9a2323b3fa3f56d7f3c6077'; Folder = 'mp4parse/tests'
        Extensions = '\.(mp4|avif|avifs|3gp)$|clusterfuzz-testcase' },
    @{ Name = 'mp4parse'; Into = 'mp4parse_capi'; Repo = 'mozilla/mp4parse-rust'; Commit = '95d98b9a6840f5bfc9a2323b3fa3f56d7f3c6077'; Folder = 'mp4parse_capi/tests'
        Extensions = '\.(mp4|avif|avifs|3gp)$' }
)

# The MP4 files of Shaka Player's tests (test/test/assets, Apache-2.0), at the commit given: DASH and HLS init
# and media segments - some named .dash - CMAF text (.cmft), encryption (CENC, cbcs), AC-3 and E-AC-3, LCEVC's
# enhancement tracks, and live streams. Not dash-aes-128's fragments nor hls-aes-256's files: segments encrypted whole.
$shakaSet = @{ Name = 'shaka'; Repo = 'shaka-project/shaka-player'; Commit = '91ca4dbd95a82ff3b352110a173756d414e5d8cb'; Folder = 'test/test/assets'
    Extensions = '^(?!hls-aes-256/).*\.(mp4|m4a|m4s|m4v|mov|dash|cmft)$' }

# The files of a folder of a GitHub repository, at a commit, and in its subfolders, into a folder of their
# own - or of the set's Into in it. Listed by the folder's git tree: the contents listing stops at 1000
# entries. The files are those of the browsers' extensions, unless the set gives its own.
function Get-GitHubFolderSet($Set) {
    $extensions = if ($Set.Extensions) { $Set.Extensions } else { $browserExtensions }
    # a folder at the top of the repository is listed with the repository's contents
    $slash = $Set.Folder.LastIndexOf('/')
    $parent = if ($slash -ge 0) { $Set.Folder.Substring(0, $slash) } else { '' }
    $name = $Set.Folder.Substring($slash + 1)
    $listing = Get-Text "https://api.github.com/repos/$($Set.Repo)/contents/$($parent)?ref=$($Set.Commit)" | ConvertFrom-Json
    $tree = ($listing | Where-Object { $_.name -eq $name }).sha
    $entries = (Get-Text "https://api.github.com/repos/$($Set.Repo)/git/trees/$($tree)?recursive=1" | ConvertFrom-Json).tree
    $files = @($entries | Where-Object { $_.type -eq 'blob' -and $_.path -match $extensions })
    Write-Host "$($Set.Name): $($files.Count) files of $($Set.Folder)"
    $into = Join-Path $Destination $Set.Name
    if ($Set.Into) { $into = Join-Path $into $Set.Into }
    foreach ($file in $files) {
        $url = "https://raw.githubusercontent.com/$($Set.Repo)/$($Set.Commit)/$($Set.Folder)/$($file.path)"
        $path = Join-Path $into $file.path.Replace('/', [IO.Path]::DirectorySeparatorChar)
        Save-File $url $path $file.size | Out-Null
    }
}

New-Item -ItemType Directory -Force -Path $Destination | Out-Null
Write-Host "Downloading into $Destination"

$folders = @{ 'H264' = 'h264'; 'H265' = 'h265'; 'H266' = 'h266' }
foreach ($c in $Codec) {
    if ($c -eq 'AV1') {
        Get-LibaomVectors
        if ($IncludeArgon) { Get-Argon }
    }
    elseif ($c -eq 'IsoBmff') {
        Get-FileFormatConformance
    }
    elseif ($c -eq 'Fate') {
        Get-FateSuite
    }
    elseif ($c -eq 'Metadata') {
        Get-MetadataSet
    }
    elseif ($c -eq 'Chromium') {
        Get-GitHubFolderSet $chromiumSet
    }
    elseif ($c -eq 'Firefox') {
        Get-GitHubFolderSet $firefoxSet
    }
    elseif ($c -eq 'Libavif') {
        Get-GitHubFolderSet $libavifSet
    }
    elseif ($c -eq 'Avif') {
        Get-GitHubFolderSet $avifSet
    }
    elseif ($c -eq 'Exiv2') {
        Get-GitHubFolderSet $exiv2Set
    }
    elseif ($c -eq 'Mp4parse') {
        foreach ($set in $mp4parseSets) { Get-GitHubFolderSet $set }
    }
    elseif ($c -eq 'Shaka') {
        Get-GitHubFolderSet $shakaSet
    }
    else {
        foreach ($set in $ituSets[$c]) { Get-ItuSet $folders[$c] $set }
    }
}

if (-not $KeepReference) { Remove-Reference }

Write-Host ''
Write-Host "Done: $($script:fetched) downloaded, $($script:skipped) already there, $($script:failed.Count) failed."
if ($script:failed.Count -gt 0) {
    $script:failed | ForEach-Object { Write-Host "  failed: $_" }
    Write-Host 'Run the script again to retry them.'
    exit 1
}
