using System;
using UnityEngine;
using System.Collections.Generic;

namespace Unity.Linq
{
    // Peformance Optimization helper

    public static partial class GameObjectExtensions
    {
        // IEnumerable<T>

        public static int ToArrayNonAlloc<T>(this IEnumerable<T> source, ref T[] array)
        {
            var index = 0;
            foreach (var item in source)
            {
                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = item;
            }

            return index;
        }

        public static void ToListNonAlloc<T>(this IEnumerable<T> source, ref List<T> list)
        {
            list.Clear();
            foreach (var item in source)
            {
                list.Add(item);
            }
        }

        // ChildrenEnumerable

        public static int ToArrayNonAlloc(this ChildrenEnumerable source, ref GameObject[] array)
        {
            var index = 0;

            var e = source.GetEnumerator(); // does not need to call Dispose.
            while (e.MoveNext())
            {
                var item = e.Current;
                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = item;
            }

            return index;
        }

        public static int ToArrayNonAlloc(this ChildrenEnumerable source, Func<GameObject, bool> filter, ref GameObject[] array)
        {
            var index = 0;
            var e = source.GetEnumerator(); // does not need to call Dispose.
            while (e.MoveNext())
            {
                var item = e.Current;
                if (!filter(item)) continue;

                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = item;
            }

            return index;
        }

        public static int ToArrayNonAlloc<T>(this ChildrenEnumerable source, Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = source.GetEnumerator(); // does not need to call Dispose.
            while (e.MoveNext())
            {
                var item = e.Current;
                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = selector(item);
            }

            return index;
        }

        public static int ToArrayNonAlloc<T>(this ChildrenEnumerable source, Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = source.GetEnumerator(); // does not need to call Dispose.
            while (e.MoveNext())
            {
                var item = e.Current;
                if (!filter(item)) continue;

                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = selector(item);
            }

            return index;
        }

        public static int ToArrayNonAlloc<TState, T>(this ChildrenEnumerable source, Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
        {
            var index = 0;
            var e = source.GetEnumerator(); // does not need to call Dispose.
            while (e.MoveNext())
            {
                var item = e.Current;
                var state = let(item);

                if (!filter(state)) continue;

                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
                array[index++] = selector(state);
            }

            return index;
        }

        // TODO:ToList...

        // TODO:Other XxxEnumerable...
    }
}
