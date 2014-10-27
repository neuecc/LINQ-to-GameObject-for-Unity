using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    public static partial class GameObjectExtensions
    {
        /// <summary>Returns a collection of GameObjects that contains the ancestors of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Ancestors())
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a filtered collection of GameObjects that contains the ancestors of every GameObject in the source collection. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Ancestors(name))
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the ancestors of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.AncestorsAndSelf())
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the ancestors of every GameObject in the source collection. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.AncestorsAndSelf(name))
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains the descendant GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Descendants())
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a filtered collection of GameObjects that contains the descendant GameObjects of every GameObject in the source collection. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Descendants(name))
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the descendent GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.DescendantsAndSelf())
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the descendent GameObjects of every GameObject in the source collection. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.DescendantsAndSelf(name))
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of the child GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Children())
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a filtered collection of the child GameObjects of every GameObject in the source collection. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Children(name))
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a collection of GameObjects that contains every GameObject in the source collection, and the child GameObjects of every GameObject in the source collection.</summary>
        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.ChildrenAndSelf())
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Returns a filtered collection of GameObjects that contains every GameObject in the source collection, and the child GameObjects of every GameObject in the source collection. Only GameObjects that have a matching name are included in the collection.</summary>
        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.ChildrenAndSelf(name))
                {
                    yield return item2;
                }
            }
        }

        /// <summary>Destroy every GameObject in the source collection safety(check null, deactive/detouch before destroy).</summary>
        /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
        public static void Destroy(this IEnumerable<GameObject> source, bool useDestroyImmediate = false)
        {
            foreach (var item in new List<GameObject>(source)) // snapshot, avoid halloween problem http://en.wikipedia.org/wiki/Halloween_Problem
            {
                item.Destroy(useDestroyImmediate);
            }
        }

        /// <summary>Returns a collection of specified component in the source collection.</summary>
        public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source)
            where T : UnityEngine.Component
        {
            foreach (var item in source)
            {
                var component = item.GetComponent<T>();
                if (component != null)
                {
                    yield return component;
                }
            }
        }
    }
}