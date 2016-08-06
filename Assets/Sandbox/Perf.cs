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
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var _= root.DescendantsAndSelf().ToArray();
            sw.Stop();
            Debug.Log(_.Length + ":" + sw.Elapsed.TotalMilliseconds + "ms");
        });

        native.onClick.AddListener(() =>
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            //var _= root.GetComponentsInChildren<Text>();

            var _ = root.DescendantsCore(null, true).ToArray();
            sw.Stop();
            Debug.Log(_.Length + ":" + sw.Elapsed.TotalMilliseconds + "ms");
        });
    }
}
