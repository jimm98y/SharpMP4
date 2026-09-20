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
