namespace ZLinq.Internal;

internal static class ListMarshal
{
    internal static List<T> AsList<T>(T[] array)
    {
        var list = new List<T>(0); // initialize with [] to avoid allocate array inside
        if (array.Length == 0) return list;

        var view = Unsafe.As<ListView<T>>(list);

        view._items = array;
        view._size = array.Length;

        return list;
    }

#pragma warning disable CS8618
    internal class ListView<T>
    {
        public T[] _items;
        public int _size;
        public int _version;
    }
#pragma warning restore CS8618
}