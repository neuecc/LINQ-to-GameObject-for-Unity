using System.Text.Json.Nodes;

namespace ZLinq;

// LINQ to Json(JsonNode)

public static class JsonNodeExtensions
{
    public static JsonNodeTraversable AsTraversable(this JsonNode origin) => new(new JsonNodeProperty(origin.Parent != null ? origin.GetPropertyName() : "", origin));

    // shortcut
    public static ChildrenEnumerable<JsonNodeTraversable, JsonNodeProperty> Children(this JsonNode origin) => origin.AsTraversable().Children<JsonNodeTraversable, JsonNodeProperty>();
    public static ChildrenEnumerable<JsonNodeTraversable, JsonNodeProperty> ChildrenAndSelf(this JsonNode origin) => origin.AsTraversable().ChildrenAndSelf<JsonNodeTraversable, JsonNodeProperty>();
    public static DescendantsEnumerable<JsonNodeTraversable, JsonNodeProperty> Descendants(this JsonNode origin) => origin.AsTraversable().Descendants<JsonNodeTraversable, JsonNodeProperty>();
    public static DescendantsEnumerable<JsonNodeTraversable, JsonNodeProperty> DescendantsAndSelf(this JsonNode origin) => origin.AsTraversable().DescendantsAndSelf<JsonNodeTraversable, JsonNodeProperty>();
    public static AncestorsEnumerable<JsonNodeTraversable, JsonNodeProperty> Ancestors(this JsonNode origin) => origin.AsTraversable().Ancestors<JsonNodeTraversable, JsonNodeProperty>();
    public static AncestorsEnumerable<JsonNodeTraversable, JsonNodeProperty> AncestorsAndSelf(this JsonNode origin) => origin.AsTraversable().AncestorsAndSelf<JsonNodeTraversable, JsonNodeProperty>();
    public static BeforeSelfEnumerable<JsonNodeTraversable, JsonNodeProperty> BeforeSelf(this JsonNode origin) => origin.AsTraversable().BeforeSelf<JsonNodeTraversable, JsonNodeProperty>();
    public static BeforeSelfEnumerable<JsonNodeTraversable, JsonNodeProperty> BeforeSelfAndSelf(this JsonNode origin) => origin.AsTraversable().BeforeSelfAndSelf<JsonNodeTraversable, JsonNodeProperty>();
    public static AfterSelfEnumerable<JsonNodeTraversable, JsonNodeProperty> AfterSelf(this JsonNode origin) => origin.AsTraversable().AfterSelf<JsonNodeTraversable, JsonNodeProperty>();
    public static AfterSelfEnumerable<JsonNodeTraversable, JsonNodeProperty> AfterSelfAndSelf(this JsonNode origin) => origin.AsTraversable().AfterSelfAndSelf<JsonNodeTraversable, JsonNodeProperty>();

    // type inference helper
    public static StructEnumerator<ChildrenEnumerable<JsonNodeTraversable, JsonNodeProperty>, JsonNodeProperty> GetEnumerator(this ChildrenEnumerable<JsonNodeTraversable, JsonNodeProperty> source) => new(source);
    public static StructEnumerator<DescendantsEnumerable<JsonNodeTraversable, JsonNodeProperty>, JsonNodeProperty> GetEnumerator(this DescendantsEnumerable<JsonNodeTraversable, JsonNodeProperty> source) => new(source);
    public static StructEnumerator<AncestorsEnumerable<JsonNodeTraversable, JsonNodeProperty>, JsonNodeProperty> GetEnumerator(this AncestorsEnumerable<JsonNodeTraversable, JsonNodeProperty> source) => new(source);
    public static StructEnumerator<BeforeSelfEnumerable<JsonNodeTraversable, JsonNodeProperty>, JsonNodeProperty> GetEnumerator(this BeforeSelfEnumerable<JsonNodeTraversable, JsonNodeProperty> source) => new(source);
    public static StructEnumerator<AfterSelfEnumerable<JsonNodeTraversable, JsonNodeProperty>, JsonNodeProperty> GetEnumerator(this AfterSelfEnumerable<JsonNodeTraversable, JsonNodeProperty> source) => new(source);
}

// Can't use ITraversable<JsonNode> because JsonNull is not exists and we must represents null on iterate.
// System.Text.Json has no JsonNull https://github.com/dotnet/runtime/issues/68128

public record struct JsonNodeProperty(string Name, JsonNode? Node);

[StructLayout(LayoutKind.Auto)]
public struct JsonNodeTraversable(JsonNodeProperty origin) : ITraversable<JsonNodeTraversable, JsonNodeProperty>
{
    IEnumerator<KeyValuePair<string, JsonNode?>>? jsonObjectEnumerator; // state for TryGet...

    public JsonNodeProperty Origin => origin;

    public JsonNodeTraversable ConvertToTraversable(JsonNodeProperty next) => new(next);

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

        parent = new JsonNodeProperty(p.Parent != null ? p.GetPropertyName(): "", p);
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