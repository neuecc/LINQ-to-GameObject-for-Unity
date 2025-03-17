using System.Text.Json.Nodes;
using ZLinq.Traversables;

namespace ZLinq;

// LINQ to Json(JsonNode)

public static class JsonNodeExtensions
{
    public static JsonNodeTraverser AsTraverser(this JsonNode origin) => new(new JsonNodeProperty(origin.Parent != null ? origin.GetPropertyName() : "", origin));

    // type inference helper

    public static ValueEnumerable<Children<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> Children(this JsonNodeTraverser traverser) => traverser.Children<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<Children<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> ChildrenAndSelf(this JsonNodeTraverser traverser) => traverser.ChildrenAndSelf<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<Descendants<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> Descendants(this JsonNodeTraverser traverser) => traverser.Descendants<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<Descendants<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> DescendantsAndSelf(this JsonNodeTraverser traverser) => traverser.DescendantsAndSelf<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<Ancestors<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> Ancestors(this JsonNodeTraverser traverser) => traverser.Ancestors<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<Ancestors<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> AncestorsAndSelf(this JsonNodeTraverser traverser) => traverser.AncestorsAndSelf<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<BeforeSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> BeforeSelf(this JsonNodeTraverser traverser) => traverser.BeforeSelf<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<BeforeSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> BeforeSelfAndSelf(this JsonNodeTraverser traverser) => traverser.BeforeSelfAndSelf<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<AfterSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> AfterSelf(this JsonNodeTraverser traverser) => traverser.AfterSelf<JsonNodeTraverser, JsonNodeProperty>();
    public static ValueEnumerable<AfterSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> AfterSelfAndSelf(this JsonNodeTraverser traverser) => traverser.AfterSelfAndSelf<JsonNodeTraverser, JsonNodeProperty>();

    // direct shortcut

    public static ValueEnumerable<Children<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> Children(this JsonNode origin) => origin.AsTraverser().Children();
    public static ValueEnumerable<Children<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> ChildrenAndSelf(this JsonNode origin) => origin.AsTraverser().ChildrenAndSelf();
    public static ValueEnumerable<Descendants<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> Descendants(this JsonNode origin) => origin.AsTraverser().Descendants();
    public static ValueEnumerable<Descendants<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> DescendantsAndSelf(this JsonNode origin) => origin.AsTraverser().DescendantsAndSelf();
    public static ValueEnumerable<Ancestors<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> Ancestors(this JsonNode origin) => origin.AsTraverser().Ancestors();
    public static ValueEnumerable<Ancestors<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> AncestorsAndSelf(this JsonNode origin) => origin.AsTraverser().AncestorsAndSelf();
    public static ValueEnumerable<BeforeSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> BeforeSelf(this JsonNode origin) => origin.AsTraverser().BeforeSelf();
    public static ValueEnumerable<BeforeSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> BeforeSelfAndSelf(this JsonNode origin) => origin.AsTraverser().BeforeSelfAndSelf();
    public static ValueEnumerable<AfterSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> AfterSelf(this JsonNode origin) => origin.AsTraverser().AfterSelf();
    public static ValueEnumerable<AfterSelf<JsonNodeTraverser, JsonNodeProperty>, JsonNodeProperty> AfterSelfAndSelf(this JsonNode origin) => origin.AsTraverser().AfterSelfAndSelf();
}

// Can't use ITraversable<JsonNode> because JsonNull is not exists and we must represents null on iterate.
// System.Text.Json has no JsonNull https://github.com/dotnet/runtime/issues/68128

public record struct JsonNodeProperty(string Name, JsonNode? Node);

[StructLayout(LayoutKind.Auto)]
public struct JsonNodeTraverser(JsonNodeProperty origin) : ITraverser<JsonNodeTraverser, JsonNodeProperty>
{
    IEnumerator<KeyValuePair<string, JsonNode?>>? jsonObjectEnumerator; // state for TryGet...

    public JsonNodeProperty Origin => origin;

    public JsonNodeTraverser ConvertToTraverser(JsonNodeProperty next) => new(next);

    public bool TryGetChildCount(out int count)
    {
        if (origin.Node is JsonObject obj)
        {
            count = obj.Count;
            return true;
        }
        count = 0;
        return false;
    }

    public bool TryGetHasChild(out bool hasChild)
    {
        if (origin.Node is JsonObject obj)
        {
            hasChild = obj.Count != 0;
            return true;
        }
        hasChild = false;
        return false;
    }

    public bool TryGetParent(out JsonNodeProperty parent)
    {
        var p = origin.Node?.Parent!;
        if (p == null)
        {
            parent = default;
            return false;
        }

        parent = new JsonNodeProperty(p.Parent != null ? p.GetPropertyName() : "", p);
        return true;
    }

    public bool TryGetNextChild(out JsonNodeProperty child)
    {
        if (origin.Node is not JsonObject obj)
        {
            child = default!;
            return false;
        }

        jsonObjectEnumerator ??= obj.GetEnumerator();

        if (jsonObjectEnumerator.MoveNext())
        {
            var kvp = jsonObjectEnumerator.Current;
            child = new(kvp.Key, kvp.Value);
            return true;
        }

        child = default!;
        return false;
    }

    public bool TryGetNextSibling(out JsonNodeProperty next)
    {
    BEGIN:
        if (jsonObjectEnumerator != null)
        {
            if (jsonObjectEnumerator.MoveNext())
            {
                var kvp = jsonObjectEnumerator.Current;
                next = new(kvp.Key, kvp.Value);
                return true;
            }
        }
        else if (TryGetParent(out var parent) && parent.Node is JsonObject obj)
        {
            var enumerator = jsonObjectEnumerator = obj.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Key == origin.Name)
                {
                    // ok to skip self
                    goto BEGIN;
                }
            }
        }

        next = default!;
        return false;
    }

    public bool TryGetPreviousSibling(out JsonNodeProperty previous)
    {
    BEGIN:
        if (jsonObjectEnumerator != null)
        {
            if (jsonObjectEnumerator.MoveNext())
            {
                var obj = jsonObjectEnumerator.Current;
                if (obj.Key != origin.Name)
                {
                    previous = new(obj.Key, obj.Value);
                    return true;
                }
                // else: find self, stop. 
            }
        }
        else if (TryGetParent(out var parent) && parent.Node is JsonObject obj)
        {
            jsonObjectEnumerator = obj.GetEnumerator();
            goto BEGIN;
        }

        previous = default!;
        return false;
    }

    public void Dispose()
    {
        if (jsonObjectEnumerator != null)
        {
            jsonObjectEnumerator.Dispose();
            jsonObjectEnumerator = null;
        }
    }
}