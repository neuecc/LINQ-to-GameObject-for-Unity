using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Linq;
using UnityEngine;

namespace Assets.Editor
{
    public class Menu
    {
        [UnityEditor.MenuItem("LINQ/Destroy")]
        public static void DestroyClone()
        {
            var go = UnityEngine.GameObject.Find("Root");
            if (go == null)
            {
                UnityEngine.Debug.Log("null!");
            }
            else
            {
                go.Descendants().Where(x => x.name.EndsWith("(Clone)")).Destroy(useDestroyImmediate: true);
            }
        }

        [UnityEditor.MenuItem("LINQ/Add")]
        public static void Add()
        {
            var go = UnityEngine.GameObject.Find("Origin");
            if (go == null)
            {
                UnityEngine.Debug.Log("null!");
            }
            else
            {
                go.Add(new UnityEngine.GameObject { name = "lastChild" });
                go.AddFirst(new UnityEngine.GameObject { name = "firstChild" });
                go.AddBeforeSelf(new UnityEngine.GameObject { name = "beforeSelf" });
                go.AddAfterSelf(new UnityEngine.GameObject { name = "afterSelf" });
            }
        }

        [UnityEditor.MenuItem("LINQ/Adds")]
        public static void Adds()
        {
            var go = UnityEngine.GameObject.Find("Origin");
            if (go == null)
            {
                UnityEngine.Debug.Log("null!");
            }
            else
            {
                go.AddRange(new[] { new GameObject("lastChild1"), new GameObject("lastChild2"), new GameObject("lastChild3") });
                go.AddFirstRange(new[] { new GameObject("firstChild1"), new GameObject("firstChild2"), new GameObject("firstChild3") });
                go.AddBeforeSelfRange(new[] { new GameObject("beforeSelf1"), new GameObject("beforeSelf2"), new GameObject("beforeSelf3") });
                go.AddAfterSelfRange(new[] { new GameObject("afterSelf1"), new GameObject("afterSelf2"), new GameObject("afterSelf3") });
            }
        }
    }
}
