using UnityEngine;
using System.Linq;
using Unity.Linq;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

public class Perf : MonoBehaviour
{
    public Button linq;
    public Button native;
    public Button legacy;

    public GameObject root;

    void Start()
    {
        linq.onClick.AddListener(() =>
        {
            //var l1 = new List<string>();
            //var l2 = new List<string>();

            {
                var count = 0;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                Profiler.BeginSample("Perf:LINQ");

                var e = root.DescendantsAndSelf().GetEnumerator();
                while (e.MoveNext())
                {
                    count++;
                    // this.GetComponent(
                    //var _ = e.Current.GetComponent<Perf>();
                    //l1.Add(e.Current.name);
                }

                Profiler.EndSample();
                sw.Stop();

                Debug.Log("LINQ:" + count + ":" + sw.Elapsed.TotalMilliseconds + "ms");
            }

            {
                var count = 0;
                var sw = System.Diagnostics.Stopwatch.StartNew();

                Profiler.BeginSample("Perf:LINQ2");

                root.DescendantsAndSelf().ForEach(_ => { });

                Profiler.EndSample();
                sw.Stop();

                Debug.Log("LINQ ForEach:" + count + ":" + sw.Elapsed.TotalMilliseconds + "ms");
            }

            {
                var sw = System.Diagnostics.Stopwatch.StartNew();

                Profiler.BeginSample("Perf:Native");
                var e = root.GetComponentsInChildren<Transform>(true);
                Profiler.EndSample();

                sw.Stop();

                Debug.Log("Native:" + e.Length + ":" + sw.Elapsed.TotalMilliseconds + "ms");
            }
            {
                var count = 0;
                var sw = System.Diagnostics.Stopwatch.StartNew();
                Profiler.BeginSample("Perf:Legacy");
                var e = LegacyDescendants(root, true).OfComponent<Text>().GetEnumerator();
                while (e.MoveNext())
                {
                    count++;

                    //l2.Add(e.Current.name);
                }
                Profiler.EndSample();
                sw.Stop();

                Debug.Log("Legacy:" + count + ":" + sw.Elapsed.TotalMilliseconds + "ms");
            }
        });

        native.onClick.AddListener(() =>
        {
        });

        legacy.onClick.AddListener(() =>
        {
            var count = 0;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            Profiler.BeginSample("Perf:Legacy");
            var e = LegacyDescendants(root, true).OfComponent<Text>().GetEnumerator();
            while (e.MoveNext())
            {
                count++;
            }
            Profiler.EndSample();
            sw.Stop();

        });
    }

    static IEnumerable<GameObject> LegacyDescendants(GameObject origin, bool withSelf)
    {
        if (origin == null) yield break;
        if (withSelf)
        {
            yield return origin;
        }

        foreach (Transform item in origin.transform)
        {
            foreach (var child in LegacyDescendants(item.gameObject, withSelf: true))
            {
                yield return child.gameObject;
            }
        }
    }
}
