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

/// <summary>Walks a parsed file looking for boxes, so the tests can assert on one table at a time.</summary>
internal static class Boxes
{
    public static T Find<T>(Container container) where T : Box =>
        FindOrNull<T>(container) ?? throw new InvalidOperationException($"{typeof(T).Name} not found");

    public static T? FindOrNull<T>(Container container) where T : Box =>
        FindOrNull<T>(container.Children);

    public static List<T> FindAll<T>(Container container) where T : Box
    {
        var found = new List<T>();
        Collect(container.Children, found);
        return found;
    }

    private static T? FindOrNull<T>(List<Box>? children) where T : Box
    {
        if (children == null)
            return null;

        foreach (var child in children)
        {
            if (child is T match)
                return match;
            if (child is IHasBoxChildren parent)
            {
                var found = FindOrNull<T>(parent.Children);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private static void Collect<T>(List<Box>? children, List<T> found) where T : Box
    {
        if (children == null)
            return;

        foreach (var child in children)
        {
            if (child is T match)
                found.Add(match);
            if (child is IHasBoxChildren parent)
                Collect(parent.Children, found);
        }
    }
}
