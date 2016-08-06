using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    // API Frontend

    public static partial class GameObjectExtensions
    {
        // Traverse Game Objects, based on Axis(Parent, Child, Children, Ancestors/Descendants, BeforeSelf/ObjectsBeforeAfter)

        /// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        /// <summary>Gets the first child GameObject with the specified name. If there is no GameObject with the speficided name, returns null.</summary>
        public static GameObject Child(this GameObject origin, string name)
        {
            if (origin == null) return null;

            var child = origin.transform.FindChild(name); // transform.find can get inactive object
            if (child == null) return null;
            return child.gameObject;
        }

        /// <summary>Returns a collection of the child GameObjects.</summary>
        public static ChildrenEnumerable Children(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the child GameObjects.</summary>
        public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the ancestor GameObjects of this GameObject.</summary>
        public static AncestorsEnumerable Ancestors(this GameObject origin)
        {
            return new AncestorsEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this element, and the ancestors of this GameObject.</summary>
        public static AncestorsEnumerable AncestorsAndSelf(this GameObject origin)
        {
            return new AncestorsEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the descendant GameObjects.</summary>
        public static DescendantsEnumerable Descendants(this GameObject origin)
        {
            return new DescendantsEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject.</summary>
        public static DescendantsEnumerable DescendantsAndSelf(this GameObject origin)
        {
            return new DescendantsEnumerable(origin, true);
        }

        public static IEnumerable<GameObject> DescendantsCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform item in origin.transform)
            {
                foreach (var child in DescendantsCore(item.gameObject, nameFilter, withSelf: true))
                {
                    if (nameFilter == null || child.name == nameFilter)
                    {
                        yield return child.gameObject;
                    }
                }
            }
        }

        /// <summary>Returns a collection of the sibling GameObjects before this GameObject.</summary>
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the sibling GameObjects before this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, nameFilter: name, withSelf: false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject.</summary>
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> BeforeSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;

            var parent = origin.transform.parent;
            if (parent == null) goto RETURN_SELF;

            var parentTransform = parent.transform;
            var childCount = parentTransform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var item = parentTransform.GetChild(i);

                var go = item.gameObject;
                if (go == origin)
                {
                    goto RETURN_SELF;
                }

                if (nameFilter == null || item.name == nameFilter)
                {
                    yield return go;
                }
            }

            RETURN_SELF:
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }
        }

        /// <summary>Returns a collection of the sibling GameObjects after this GameObject.</summary>
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, nameFilter: null, withSelf: false);
        }

        /// <summary>Returns a filtered collection of the sibling GameObjects after this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, nameFilter: name, withSelf: false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject.</summary>
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, nameFilter: null, withSelf: true);
        }

        /// <summary>Returns a filtered collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject. Only GameObjects that have a matching name are included in the collection.</summary>       
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> AfterSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parent = origin.transform.parent;
            if (parent == null) yield break;

            var index = origin.transform.GetSiblingIndex() + 1;
            var parentTransform = parent.transform;
            var count = parentTransform.childCount;

            while (index < count)
            {
                var target = parentTransform.GetChild(index).gameObject;
                if (nameFilter == null || target.name == nameFilter)
                {
                    yield return target;
                }

                index++;
            }
        }
    }

    // Implements hand struct enumerator.

    public struct ChildrenEnumerable : IEnumerable<GameObject>
    {
        readonly GameObject origin;
        readonly bool withSelf;

        public ChildrenEnumerable(GameObject origin, bool withSelf)
        {
            this.origin = origin;
            this.withSelf = withSelf;
        }

        public OfComponentEnumerable<T> OfComponent<T>()
            where T : Component
        {
            return new OfComponentEnumerable<T>(this);
        }

        public Enumerator GetEnumerator()
        {
            // check GameObject is destroyed only on GetEnumerator timing
            return (origin == null)
                ? new Enumerator(null, withSelf, false)
                : new Enumerator(origin.transform, withSelf, true);
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region LINQ

        public int ToArrayNonAlloc(ref GameObject[] array)
        {
            var index = 0;

            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public GameObject[] ToArray()
        {
            var array = new GameObject[4];
            var len = ToArrayNonAlloc(ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public GameObject[] ToArray(Func<GameObject, bool> filter)
        {
            var array = new GameObject[4];
            var len = ToArrayNonAlloc(filter, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<T>(Func<GameObject, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc<T>(selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc(filter, selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc(let, filter, selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public GameObject First()
        {
            var e = this.GetEnumerator();
            if (e.MoveNext())
            {
                return e.Current;
            }
            else
            {
                throw new InvalidOperationException("sequence is empty.");
            }
        }

        public GameObject FirstOrDefault()
        {
            var e = this.GetEnumerator();
            return (e.MoveNext())
                ? e.Current
                : null;
        }

        #endregion

        public struct Enumerator : IEnumerator<GameObject>
        {
            readonly int childCount; // childCount is fixed when GetEnumerator is called.

            readonly Transform originTransform;
            readonly bool canRun;

            bool withSelf;
            int currentIndex;
            GameObject current;

            public Enumerator(Transform originTransform, bool withSelf, bool canRun)
            {
                this.originTransform = originTransform;
                this.withSelf = withSelf;
                this.childCount = canRun ? originTransform.childCount : 0;
                this.currentIndex = -1;
                this.canRun = canRun;
                this.current = null;
            }

            public bool MoveNext()
            {
                if (!canRun) return false;

                if (withSelf)
                {
                    current = originTransform.gameObject;
                    withSelf = false;
                    return true;
                }

                currentIndex++;
                if (currentIndex < childCount)
                {
                    var child = originTransform.GetChild(currentIndex);
                    current = child.gameObject;
                    return true;
                }

                return false;
            }

            public GameObject Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }

        public struct OfComponentEnumerable<T> : IEnumerable<T>
            where T : Component
        {
            readonly ChildrenEnumerable parent;

            public OfComponentEnumerable(ChildrenEnumerable parent)
            {
                this.parent = parent;
            }

            public OfComponentEnumerator<T> GetEnumerator()
            {
                return new OfComponentEnumerator<T>(parent);
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public T First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public T FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            public T[] ToArray()
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public int ToArrayNonAlloc(ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = e.Current;
                }

                return index;
            }

            #endregion
        }

        public struct OfComponentEnumerator<T> : IEnumerator<T>
            where T : Component
        {
            readonly ChildrenEnumerable parent;
            Enumerator enumerator;
            T current;
            bool running;

            public OfComponentEnumerator(ChildrenEnumerable parent)
            {
                this.parent = parent;
                this.enumerator = default(Enumerator);
                this.current = default(T);
                this.running = false;
            }

            public bool MoveNext()
            {
                if (!running)
                {
                    enumerator = parent.GetEnumerator();
                    running = true;
                }

                while (enumerator.MoveNext())
                {
                    var component = enumerator.Current.GetComponent<T>();
                    if (component != null)
                    {
                        current = component;
                        return true;
                    }
                }

                return false;
            }

            public T Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }
    }

    public struct AncestorsEnumerable : IEnumerable<GameObject>
    {
        readonly GameObject origin;
        readonly bool withSelf;

        public AncestorsEnumerable(GameObject origin, bool withSelf)
        {
            this.origin = origin;
            this.withSelf = withSelf;
        }

        public OfComponentEnumerable<T> OfComponent<T>()
            where T : Component
        {
            return new OfComponentEnumerable<T>(this);
        }

        public Enumerator GetEnumerator()
        {
            // check GameObject is destroyed only on GetEnumerator timing
            return (origin == null)
                ? new Enumerator(null, null, withSelf, false)
                : new Enumerator(origin, origin.transform, withSelf, true);
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region LINQ

        public int ToArrayNonAlloc(ref GameObject[] array)
        {
            var index = 0;

            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public GameObject[] ToArray()
        {
            var array = new GameObject[4];
            var len = ToArrayNonAlloc(ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public GameObject[] ToArray(Func<GameObject, bool> filter)
        {
            var array = new GameObject[4];
            var len = ToArrayNonAlloc(filter, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<T>(Func<GameObject, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc<T>(selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc(filter, selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc(let, filter, selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public GameObject First()
        {
            var e = this.GetEnumerator();
            if (e.MoveNext())
            {
                return e.Current;
            }
            else
            {
                throw new InvalidOperationException("sequence is empty.");
            }
        }

        public GameObject FirstOrDefault()
        {
            var e = this.GetEnumerator();
            return (e.MoveNext())
                ? e.Current
                : null;
        }

        #endregion

        public struct Enumerator : IEnumerator<GameObject>
        {
            readonly bool canRun;

            GameObject current;
            Transform currentTransform;
            bool withSelf;

            public Enumerator(GameObject origin, Transform originTransform, bool withSelf, bool canRun)
            {
                this.current = origin;
                this.currentTransform = originTransform;
                this.withSelf = withSelf;
                this.canRun = canRun;
                this.current = null;
            }

            public bool MoveNext()
            {
                if (!canRun) return false;

                if (withSelf)
                {
                    withSelf = false;
                    return true;
                }

                var parentTransform = currentTransform.parent;
                if (parentTransform != null)
                {
                    current = parentTransform.gameObject;
                    currentTransform = parentTransform;
                    return true;
                }

                return false;
            }

            public GameObject Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }

        public struct OfComponentEnumerable<T> : IEnumerable<T>
            where T : Component
        {
            readonly AncestorsEnumerable parent;

            public OfComponentEnumerable(AncestorsEnumerable parent)
            {
                this.parent = parent;
            }

            public OfComponentEnumerator<T> GetEnumerator()
            {
                return new OfComponentEnumerator<T>(parent);
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public T First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public T FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            public T[] ToArray()
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public int ToArrayNonAlloc(ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = e.Current;
                }

                return index;
            }

            #endregion
        }

        public struct OfComponentEnumerator<T> : IEnumerator<T>
            where T : Component
        {
            readonly AncestorsEnumerable parent;
            Enumerator enumerator;
            T current;
            bool running;

            public OfComponentEnumerator(AncestorsEnumerable parent)
            {
                this.parent = parent;
                this.enumerator = default(Enumerator);
                this.current = default(T);
                this.running = false;
            }

            public bool MoveNext()
            {
                if (!running)
                {
                    enumerator = parent.GetEnumerator();
                    running = true;
                }

                while (enumerator.MoveNext())
                {
                    var component = enumerator.Current.GetComponent<T>();
                    if (component != null)
                    {
                        current = component;
                        return true;
                    }
                }

                return false;
            }

            public T Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }
    }

    public struct DescendantsEnumerable : IEnumerable<GameObject>
    {
        readonly GameObject origin;
        readonly bool withSelf;

        public DescendantsEnumerable(GameObject origin, bool withSelf)
        {
            this.origin = origin;
            this.withSelf = withSelf;
        }

        public OfComponentEnumerable<T> OfComponent<T>()
            where T : Component
        {
            return new OfComponentEnumerable<T>(this);
        }

        public Enumerator GetEnumerator()
        {
            // check GameObject is destroyed only on GetEnumerator timing
            return (origin == null)
                ? new Enumerator(null, withSelf, false, null)
                : new Enumerator(origin.transform, withSelf, true, new Stack<Enumerator>());
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region LINQ

        public int ToArrayNonAlloc(ref GameObject[] array)
        {
            var index = 0;

            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
        {
            var index = 0;
            var e = this.GetEnumerator(); // does not need to call Dispose.
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

        public GameObject[] ToArray()
        {
            var array = new GameObject[4];
            var len = ToArrayNonAlloc(ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public GameObject[] ToArray(Func<GameObject, bool> filter)
        {
            var array = new GameObject[4];
            var len = ToArrayNonAlloc(filter, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<T>(Func<GameObject, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc<T>(selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc(filter, selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
        {
            var array = new T[4];
            var len = ToArrayNonAlloc(let, filter, selector, ref array);
            if (array.Length != len)
            {
                Array.Resize(ref array, len);
            }
            return array;
        }

        public GameObject First()
        {
            var e = this.GetEnumerator();
            if (e.MoveNext())
            {
                return e.Current;
            }
            else
            {
                throw new InvalidOperationException("sequence is empty.");
            }
        }

        public GameObject FirstOrDefault()
        {
            var e = this.GetEnumerator();
            return (e.MoveNext())
                ? e.Current
                : null;
        }

        #endregion

        public struct Enumerator : IEnumerator<GameObject>
        {
            readonly int childCount; // childCount is fixed when GetEnumerator is called.

            readonly Transform originTransform;
            readonly bool canRun;

            bool withSelf;
            int currentIndex;
            GameObject current;
            object sharedStack; // Stack<Enumerator>

            public Enumerator(Transform originTransform, bool withSelf, bool canRun, Stack<Enumerator> sharedStack)
            {
                this.originTransform = originTransform;
                this.withSelf = withSelf;
                this.childCount = canRun ? originTransform.childCount : 0;
                this.currentIndex = -1;
                this.canRun = canRun;
                this.current = null;
                this.sharedStack = sharedStack;
            }

            public bool MoveNext()
            {
                RECURSIVE:
                if (!canRun) return false;

                if (withSelf)
                {
                    current = originTransform.gameObject;
                    withSelf = false;
                    return true;
                }

                var stack = (Stack<Enumerator>)sharedStack;
                if (stack.Count != 0)
                {
                    var e = stack.Pop();
                    if (e.MoveNext())
                    {
                        current = e.Current;
                        stack.Push(e);
                        return true;
                    }
                }

                currentIndex++;
                if (currentIndex < childCount)
                {
                    var item = originTransform.GetChild(currentIndex);
                    var childEnumerator = new Enumerator(item, true, true, stack);
                    stack.Push(childEnumerator);
                    goto RECURSIVE;
                }

                return false;
            }

            public GameObject Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }

        struct Enumerator2
        {

        }

        public struct OfComponentEnumerable<T> : IEnumerable<T>
            where T : Component
        {
            readonly DescendantsEnumerable parent;

            public OfComponentEnumerable(DescendantsEnumerable parent)
            {
                this.parent = parent;
            }

            public OfComponentEnumerator<T> GetEnumerator()
            {
                return new OfComponentEnumerator<T>(parent);
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public T First()
            {
                var e = this.GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public T FirstOrDefault()
            {
                var e = this.GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            public T[] ToArray()
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public int ToArrayNonAlloc(ref T[] array)
            {
                var index = 0;
                var e = this.GetEnumerator();
                while (e.MoveNext())
                {
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = e.Current;
                }

                return index;
            }

            #endregion
        }

        public struct OfComponentEnumerator<T> : IEnumerator<T>
            where T : Component
        {
            readonly DescendantsEnumerable parent;
            Enumerator enumerator;
            T current;
            bool running;

            public OfComponentEnumerator(DescendantsEnumerable parent)
            {
                this.parent = parent;
                this.enumerator = default(Enumerator);
                this.current = default(T);
                this.running = false;
            }

            public bool MoveNext()
            {
                if (!running)
                {
                    enumerator = parent.GetEnumerator();
                    running = true;
                }

                while (enumerator.MoveNext())
                {
                    var component = enumerator.Current.GetComponent<T>();
                    if (component != null)
                    {
                        current = component;
                        return true;
                    }
                }

                return false;
            }

            public T Current { get { return current; } }
            object IEnumerator.Current { get { return current; } }
            public void Dispose() { }
            public void Reset() { throw new NotSupportedException(); }
        }
    }
}