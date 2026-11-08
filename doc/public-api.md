# Public API

What the libraries of SharpMP4 expose is written down, and a removal from it does not build until it is written down too, an addition warns until it is. The libraries are SharpMP4, SharpISOBMFF, SharpMP4Common, SharpH26X, SharpH261,
SharpH262, SharpH263, SharpH264, SharpH265, SharpH266, SharpMPEG4, SharpProRes, SharpAVX, SharpAV1,
SharpAV2 and SharpVP9, and two checks run on every `dotnet build` of each:

- **The API files.** `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt` next to each project list
  its public types and members, the generated ones (the boxes, the syntax of each codec) among them.
  They are the same on all three target frameworks. Shipped is the API as released; Unshipped is what
  changed since. A public symbol missing from them is a warning, RS0016, until it is
  listed - an addition breaks no one - and a listed one that is gone fails the build with RS0017.
- **Package validation.** Each package being built is checked for its target frameworks agreeing
  with one another: what net481 offers, net8.0 and net10.0 have to offer too. A library with a release
  is compared with it as well (`PackageValidationBaselineVersion` in its project, 0.3.2 now), and every
  binary or source breaking change fails the build with a CP error. SharpAV2, SharpAVX, SharpVP9,
  SharpH261, SharpH262, SharpH263, SharpMPEG4 and SharpProRes have no release yet.

## Adding to the API

List what the RS0016 warnings name: accept their fix in the IDE ("Add to public API"), or from the command line:

```
dotnet format analyzers src/SharpMP4.sln --diagnostics RS0016 --severity warn
```

The new lines land in each project's `PublicAPI.Unshipped.txt`. A change to a generator - a box, a
syntax element - changes the API of every library it generates code for, and is listed the same way.

What only some frameworks have, inside an `#if` - a polyfill of what net481 lacks, say - is kept
internal, so that the API is the same on all three.

## Removing or changing the API

RS0017 names what disappeared. Moving its line from Shipped to Unshipped with a `*REMOVED*` prefix
records the removal; a member whose signature changed is a removal of the old line and an addition
of the new one.

If package validation then reports a CP error, the change breaks code built against another
framework or the last release. When that is intended, record it:

```
dotnet build src/SharpISOBMFF/SharpISOBMFF.csproj -c Release -p:ApiCompatGenerateSuppressionFile=true
```

That writes `CompatibilitySuppressions.xml` next to the project. Say in the pull request what broke.

Validation runs as part of the build, because the build packs. `dotnet pack` on its own does not
rebuild and validates whatever binaries are already in `bin`.

## Releasing

1. Move every line of each library's `PublicAPI.Unshipped.txt` into the `PublicAPI.Shipped.txt`
   beside it, dropping the `*REMOVED*` lines together with the shipped lines they remove.
2. Once the release is on nuget.org, set `PackageValidationBaselineVersion` in each library's
   project to it (a library's first release adds the property), and delete the
   `CompatibilitySuppressions.xml` files: what they excused is in the new baseline.

## What is recorded now

Against 0.3.2, the `CompatibilitySuppressions.xml` files record what 0.4.0 changes, about a thousand
breaks, each once for each target framework:

- **SharpISOBMFF**: the boxes as generated now - fields renamed, retyped or gone, boxes of other base
  types, a few boxes removed - and `IMp4Serializable` with `Syntax` and `Trace`. On net481,
  `StreamExtensions`, the polyfill of .NET 7's `Stream.ReadExactly`, is internal now.
- **SharpAV1**: the types of AOM's syntax moved to SharpAVX (`AomStream`, `IAomContext`,
  `IAomSerializable`, ...), and the context's members as generated now.
- **SharpH264, SharpH265, SharpH266, SharpH26X**: members of the contexts and the syntax as generated
  now, and `IItuSerializable.Syntax`.
- **SharpMP4**: the builders' and tracks' interfaces with new members, `TrackBase.ProcessSample`, and
  `ParseSample` gone.
- **SharpMP4Common**: `Bitstream` and `RbspBitstream` with their reads no longer virtual and their
  fields no longer protected.
