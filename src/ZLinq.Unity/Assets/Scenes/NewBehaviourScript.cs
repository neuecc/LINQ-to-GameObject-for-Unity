using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using ZLinq;
using ZLinq.Linq;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject Origin;

    void Start()
    {
        //Debug.Log("Ancestors--------------");  // Container, Root
        //foreach (var item in Origin.Ancestors()) Debug.Log(item.name);

        //Debug.Log("Children--------------"); // Sphere_A, Sphere_B, Group, Sphere_A, Sphere_B
        //foreach (var item in Origin.Children()) Debug.Log(item.name);

        //Debug.Log("Descendants--------------"); // Sphere_A, Sphere_B, Group, P1, Group, Sphere_B, P2, Sphere_A, Sphere_B
        //foreach (var item in Origin.Descendants()) Debug.Log(item.name);

        //Debug.Log("BeforeSelf--------------"); // C1, C2
        //foreach (var item in Origin.BeforeSelf()) Debug.Log(item.name);

        //Debug.Log("AfterSelf--------------");  // C3, C4
        //foreach (var item in Origin.AfterSelf()) Debug.Log(item.name);

        // Origin.Ancestors().OfComponent<UnityEngine.TrailRenderer>();


        Debug.Log("OfComponent GO");
        var i = 0;
        foreach (var item in Origin.Descendants().OfComponent<Transform>())
        {
            Debug.Log("OfComponent");
            Debug.Log(item.name);
            if (i++ == 100)
            {
                Debug.Log("BREAK END");
                break;
            }
        }

        //Test();
        //Test2();
    }

    public static void Test()
    {
        var tako = ValueEnumerable.Range(1, 10).Select(x => x.ToString());
        var str = string.Join(',', tako.AsEnumerable());
        Debug.Log(str);
    }

    public static void Test2()
    {
        var w = ValueEnumerable.Range(1, 10)
            .Where(x => x % 2 == 0)
            .Take(10)
            .Index()
            .Order()
            .Skip(1)
            .Shuffle()
            .Select(x => x.Item)
            .Prepend(9999)
            .Append(10000)
            .Chunk(99)
            .Distinct();

        foreach (var item in w)
        {
            Debug.Log(item);
        }
    }
}

public static class ZLinqExtensions
{
    public static IEnumerable<T> AsEnumerable<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> valueEnumerable)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        using (var e = valueEnumerable.Enumerator)
        {
            while (e.TryGetNext(out var current))
            {
                yield return current;
            }
        }
    }
}
