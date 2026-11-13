// SharpMP4
// Copyright (C) 2026 Lukas Volf
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
// SOFTWARE.

using SharpISOBMFF;

namespace SharpMP4.Tests;

/// <summary>
/// Tests against <see cref="BinaryUTF8String.Text"/>, which reads a handler name in whichever form a
/// writer left it: zero terminated as ISOBMFF has it, or counted as QuickTime has it.
/// </summary>
[TestClass]
public class BinaryUTF8StringTests
{
    [TestMethod]
    public void ReadsAZeroTerminatedName()
    {
        Assert.AreEqual("VideoHandler", Text("VideoHandler\0"));
    }

    [TestMethod]
    public void ReadsACountedName()
    {
        Assert.AreEqual("Apple Alias Data Handler", Text("\x18" + "Apple Alias Data Handler"));
    }

    /// <summary>As mp4-with-mov-in24-ver.mp4 has it: a counted name, then zeros.</summary>
    [TestMethod]
    public void ReadsACountedNameWithZerosAfterIt()
    {
        Assert.AreEqual("Apple Alias Data Handler", Text("\x18" + "Apple Alias Data Handler\0\0\0"));
    }

    /// <summary>As Apple's afconvert writes it: nothing, and a zero more than it needs.</summary>
    [TestMethod]
    public void ReadsAnEmptyNameWithAZeroAfterIt()
    {
        Assert.AreEqual("", Text("\0\0"));
    }

    /// <summary>
    /// A zero terminated name whose first letter, read as a count, would fit: 'A' is 65, and the name is
    /// longer than that. It has no zero where a count of 65 would end it, so it is not counted.
    /// </summary>
    [TestMethod]
    public void DoesNotTakeALongNameForACountedOne()
    {
        string name = "A" + new string('x', 80);
        Assert.AreEqual(name, Text(name + "\0"));
    }

    [TestMethod]
    public void KeepsTheBytesAsTheyWere()
    {
        byte[] bytes = [0x00, 0xB0];
        Assert.AreSame(bytes, new BinaryUTF8String(bytes).Bytes);
    }

    private static string Text(string raw)
    {
        return new BinaryUTF8String(raw.Select(c => (byte)c).ToArray()).Text;
    }
}
