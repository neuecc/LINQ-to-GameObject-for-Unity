using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using ZLinq;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject Origin;

    void Start()
    {
        Debug.Log("Ancestors--------------");  // Container, Root
        foreach (var item in Origin.Ancestors()) Debug.Log(item.name);

        Debug.Log("Children--------------"); // Sphere_A, Sphere_B, Group, Sphere_A, Sphere_B
        foreach (var item in Origin.Children()) Debug.Log(item.name);

        Debug.Log("Descendants--------------"); // Sphere_A, Sphere_B, Group, P1, Group, Sphere_B, P2, Sphere_A, Sphere_B
        foreach (var item in Origin.Descendants()) Debug.Log(item.name);

        Debug.Log("BeforeSelf--------------"); // C1, C2
        foreach (var item in Origin.BeforeSelf()) Debug.Log(item.name);

        Debug.Log("AfterSelf--------------");  // C3, C4
        foreach (var item in Origin.AfterSelf()) Debug.Log(item.name);

        // Origin.Ancestors().OfComponent<UnityEngine.TrailRenderer>();

        Test();
    }

    public static void Test()
    {
        var tako = ValueEnumerable.Range(1, 10).Select(x => x.ToString());
        var str = string.Join(',', tako.AsEnumerable());
        Debug.Log(str);
    }

    public static void Test2()
    {
     // NativeSlice   
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
