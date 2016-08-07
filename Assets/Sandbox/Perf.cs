using UnityEngine;
using System.Linq;
using Unity.Linq;
using System.Collections;
using UnityEngine.UI;

public class Perf : MonoBehaviour
{
    public Button linq;
    public Button native;

    public GameObject root;

    void Start()
    {
        linq.onClick.AddListener(() =>
        {


            {
                var count = 0;
                var sw = System.Diagnostics.Stopwatch.StartNew();
                Profiler.BeginSample("AfterSelf New");
                var e = root.AfterSelf().OfComponent<Transform>().GetEnumerator();
                while (e.MoveNext())
                {
                    count++;
                    UnityEngine.Debug.Log(e.Current.name);
                }
                Profiler.EndSample();
                sw.Stop();
                Debug.Log("New:" + count + ":" + sw.Elapsed.TotalMilliseconds + "ms");

            }

            //{
            //    var count = 0;
            //    var sw = System.Diagnostics.Stopwatch.StartNew();
            //    Profiler.BeginSample("AncestorsAndSelf Native");
            //    var e = root.GetComponentsInParent<Transform>(true);
            //    count = e.Length;
            //    Profiler.EndSample();

            //    sw.Stop();
            //    Debug.Log("Native:" + count + ":" + sw.Elapsed.TotalMilliseconds + "ms");

            //    foreach (var item in e)
            //    {
            //        UnityEngine.Debug.Log(item.name);
            //    }
            //}


            //var _= root.DescendantsAndSelf().ToArray();

            //Debug.Log(_.Length + ":" + sw.Elapsed.TotalMilliseconds + "ms");
        });

        native.onClick.AddListener(() =>
        {
            //var sw = System.Diagnostics.Stopwatch.StartNew();
            //var _ = root.GetComponentsInChildren<Text>();

            // var _ = root.DescendantsCore(null, true).ToArray();
            //sw.Stop();
            //Debug.Log(_.Length + ":" + sw.Elapsed.TotalMilliseconds + "ms");
        });
    }
}
