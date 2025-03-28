#if NET48
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLinq.Tests.Linq;

namespace ZLinq;

public static class Array
{
    public static void Clear(System.Array array)
    {
        System.Array.Clear(array, 0, array.Length);
    }

    public static T[] Empty<T>() => System.Array.Empty<T>();

    public static void Copy<T>(T[] sourceArray, T[] destinationArray, int length)
    {
        System.Array.Copy(sourceArray, destinationArray, length);
    }

    public static void Fill<T>(T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
    }

    public static int FindIndex<T>(T[] array, Predicate<T> match)
    {
        return System.Array.FindIndex(array, match);
    }

    public static int FindIndex<T>(T[] array,int startIndex, Predicate<T> match)
    {
        return System.Array.FindIndex(array, startIndex, match);
    }

    public static void Sort<T>(T[] array)
    {
        System.Array.Sort(array);
    }   
}

#endif
