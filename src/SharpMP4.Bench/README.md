# SharpMP4.Bench

Timing and fuzzing harness. It runs against `bunny.mp4`, which is committed to the repo, so it
needs nothing from outside; pass a path to point it at another file.

```
dotnet run --project src/SharpMP4.Bench -c Release -- all
dotnet run --project src/SharpMP4.Bench -c Release -- bits
dotnet run --project src/SharpMP4.Bench -c Release -- fuzz [file.mp4] [iterations]
```

Build it in Release. A Debug build measures the debug code, and the numbers are not comparable.

## What it measures

`parse` times reading the box tree and demuxing every sample, and reports what one NAL unit costs
in allocation.

`bits` times the bit reader every H.26x parser sits on. The reference reader at the bottom - a
64-bit accumulator with none of the behaviour the real one needs - is there as a floor; without
it a number on its own says nothing about whether it can be improved. The `logging` rows use a
logger that discards its messages, so what separates them is the cost of building a message per
syntax element, not the cost of writing one.

## What it fuzzes

`fuzz` corrupts NAL units and whole files and records what comes back. Bit flips explore the
syntax; runs of zeroes are what truncation and zero fill look like in practice, and are also what
makes an Exp-Golomb value enormous, which matters because several of those values are used
directly as array lengths.

What it watches for is not whether the parse fails - a corrupt file should fail - but how much it
costs to fail: a parser that allocates hundreds of megabytes out of a few kilobytes of input is a
denial of service waiting to happen on anything that opens untrusted files. The input behind the
worst allocation is saved so it can be looked at rather than guessed at, and the seeds are fixed,
so a saved case reproduces.

Two findings came out of it, both now bounded and covered by tests in `SharpMP4.Tests`: a slice
header declaring 125,898,939 negative reference pictures allocated 16 GB from a 36 KB NAL unit,
and four bytes of an ftyp header declaring a 256 MB box allocated 256 MB out of a 1 MB file.
