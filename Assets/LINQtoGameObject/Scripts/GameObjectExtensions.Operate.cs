using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    /// <summary>
    /// Set type of cloned child GameObject's localPosition/Scale/Rotation.
    /// </summary>
    public enum TransformCloneType
    {
        /// <summary>Set to same as Original. This is default of Add methods.</summary>
        KeepOriginal,
        /// <summary>Set to same as Parent.</summary>
        FollowParent,
        /// <summary>Set to Position = zero, Scale = one, Rotation = identity.</summary>
        Origin,
        /// <summary>Position/Scale/Rotation as is.</summary>
        DoNothing
    }

    /// <summary>
    /// Set type of moved child GameObject's localPosition/Scale/Rotation.
    /// </summary>
    public enum TransformMoveType
    {
        /// <summary>Set to same as Parent.</summary>
        FollowParent,
        /// <summary>Set to Position = zero, Scale = one, Rotation = identity.</summary>
        Origin,
        /// <summary>Position/Scale/Rotation as is.</summary>
        DoNothing
    }

    public static partial class GameObjectExtensions
    {
        static UnityEngine.GameObject GetGameObject<T>(T obj)
            where T : UnityEngine.Object
        {
            var gameObject = obj as GameObject;
            if (gameObject == null)
            {
                var component = obj as Component;
                if (component == null)
                {
                    return null;
                }

                gameObject = component.gameObject;
            }

            return gameObject;
        }

        #region Add

        /// <summary>
        /// <para>Adds the GameObject/Component as children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static T Add<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childOriginal == null) throw new ArgumentNullException("childOriginal");

            var child = UnityEngine.Object.Instantiate(childOriginal);

            var childGameObject = GetGameObject(child);

            // for uGUI, should use SetParent(parent, false)
            var childTransform = childGameObject.transform;
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
            var rectTransform = childTransform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(parent.transform, worldPositionStays: false);
            }
            else
            {
#endif
                var parentTransform = parent.transform;
                childTransform.parent = parentTransform;
                switch (cloneType)
                {
                    case TransformCloneType.FollowParent:
                        childTransform.localPosition = parentTransform.localPosition;
                        childTransform.localScale = parentTransform.localScale;
                        childTransform.localRotation = parentTransform.localRotation;
                        break;
                    case TransformCloneType.Origin:
                        childTransform.localPosition = Vector3.zero;
                        childTransform.localScale = Vector3.one;
                        childTransform.localRotation = Quaternion.identity;
                        break;
                    case TransformCloneType.KeepOriginal:
                        var co = GetGameObject(childOriginal);
                        var childOriginalTransform = co.transform;
                        childTransform.localPosition = childOriginalTransform.localPosition;
                        childTransform.localScale = childOriginalTransform.localScale;
                        childTransform.localRotation = childOriginalTransform.localRotation;
                        break;
                    case TransformCloneType.DoNothing:
                    default:
                        break;
                }
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
            }
#endif
            if (setLayer)
            {
                childGameObject.layer = parent.layer;
            }

            if (setActive != null)
            {
                childGameObject.SetActive(setActive.Value);
            }
            if (specifiedName != null)
            {
                child.name = specifiedName;
            }

            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component as children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static List<T> AddRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childOriginals == null) throw new ArgumentNullException("childOriginals");

            var list = new List<T>();
            foreach (var childOriginal in childOriginals)
            {
                var child = Add(parent, childOriginal, cloneType, setActive, specifiedName, setLayer);
                list.Add(child);
            }

            return list;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component as the first children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>      
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static T AddFirst<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            var child = Add(parent, childOriginal, cloneType, setActive, specifiedName, setLayer);
            var go = GetGameObject(child);
            if (go == null) return child;

            go.transform.SetAsFirstSibling();
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component as the first children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>       
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static List<T> AddFirstRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            var child = AddRange(parent, childOriginals, cloneType, setActive, specifiedName, setLayer);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                var go = GetGameObject(child[i]);
                if (go == null) continue;
                go.transform.SetAsFirstSibling();
            }
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component before this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static T AddBeforeSelf<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex();

            var child = Add(root, childOriginal, cloneType, setActive, specifiedName, setLayer);

            var go = GetGameObject(child);
            if (go == null) return child;

            go.transform.SetSiblingIndex(sibilingIndex);
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component before this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static List<T> AddBeforeSelfRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex();
            var child = AddRange(root, childOriginals, cloneType, setActive, specifiedName, setLayer);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                var go = GetGameObject(child[i]);
                if (go == null) continue;
                go.transform.SetSiblingIndex(sibilingIndex);
            }

            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component after this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static T AddAfterSelf<T>(this GameObject parent, T childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex() + 1;
            var child = Add(root, childOriginal, cloneType, setActive, specifiedName, setLayer);
            var go = GetGameObject(child);
            if (go == null) return child;

            go.transform.SetSiblingIndex(sibilingIndex);
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject/Component after this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        /// <param name="setLayer">Set layer of child GameObject same with parent.</param>
        public static List<T> AddAfterSelfRange<T>(this GameObject parent, IEnumerable<T> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null, bool setLayer = false)
            where T : UnityEngine.Object
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex() + 1;
            var child = AddRange(root, childOriginals, cloneType, setActive, specifiedName, setLayer);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                var go = GetGameObject(child[i]);
                if (go == null) continue;
                go.transform.SetSiblingIndex(sibilingIndex);
            }

            return child;
        }

        #endregion

        #region Move

        /// <summary>
        /// <para>Move the GameObject as children of this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="child">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>      
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static GameObject MoveToLast(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (child == null) throw new ArgumentNullException("child");

            // for uGUI, should use SetParent(parent, false)
            var childTransform = child.transform;
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
            var rectTransform = childTransform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.SetParent(parent.transform, worldPositionStays: false);
            }
            else
            {
#endif
                var parentTransform = parent.transform;
                childTransform.parent = parentTransform;
                switch (moveType)
                {
                    case TransformMoveType.FollowParent:
                        childTransform.localPosition = parentTransform.localPosition;
                        childTransform.localScale = parentTransform.localScale;
                        childTransform.localRotation = parentTransform.localRotation;
                        break;
                    case TransformMoveType.Origin:
                        childTransform.localPosition = Vector3.zero;
                        childTransform.localScale = Vector3.one;
                        childTransform.localRotation = Quaternion.identity;
                        break;
                    case TransformMoveType.DoNothing:
                    default:
                        break;
                }
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
            }
#endif
            child.layer = parent.layer;

            if (setActive != null)
            {
                child.SetActive(setActive.Value);
            }

            return child;
        }

        /// <summary>
        /// <para>Move the GameObject as children of this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childs">Target.</param>
        /// <param name="moveType">Choose set type of moved child GameObject's localPosition/Scale/Rotation.</param>
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> MoveToLast(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childs == null) throw new ArgumentNullException("childs");

            var list = new List<GameObject>();
            foreach (var childOriginal in childs)
            {
                var child = MoveToLast(parent, childOriginal, moveType);
                list.Add(child);
            }

            return list;
        }

        /// <summary>
        /// <para>Move the GameObject as the first children of this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="child">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>      
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static GameObject MoveToFirst(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            MoveToLast(parent, child, moveType, setActive);
            child.transform.SetAsFirstSibling();
            return child;
        }

        /// <summary>
        /// <para>Move the GameObject as the first children of this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childs">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>       
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> MoveToFirst(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            var child = MoveToLast(parent, childs, moveType, setActive);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                child[i].transform.SetAsFirstSibling();
            }
            return child;
        }

        /// <summary>
        /// <para>Move the GameObject before this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="child">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>      
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static GameObject MoveToBeforeSelf(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex();

            MoveToLast(root, child, moveType, setActive);
            child.transform.SetSiblingIndex(sibilingIndex);
            return child;
        }

        /// <summary>
        /// <para>Move the GameObject before GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childs">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>       
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> MoveToBeforeSelf(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex();
            var child = MoveToLast(root, childs, moveType, setActive);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                child[i].transform.SetSiblingIndex(sibilingIndex);
            }

            return child;
        }

        /// <summary>
        /// <para>Move the GameObject after this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="child">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>      
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static GameObject MoveToAfterSelf(this GameObject parent, GameObject child, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex() + 1;
            MoveToLast(root, child, moveType, setActive);
            child.transform.SetSiblingIndex(sibilingIndex);
            return child;
        }

        /// <summary>
        /// <para>Move the GameObject after this GameObject.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childs">Target.</param>
        /// <param name="moveType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>       
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> MoveToAfterSelf(this GameObject parent, IEnumerable<GameObject> childs, TransformMoveType moveType = TransformMoveType.DoNothing, bool? setActive = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex() + 1;
            var child = MoveToLast(root, childs, moveType, setActive);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                child[i].transform.SetSiblingIndex(sibilingIndex);
            }

            return child;
        }

        #endregion

        /// <summary>Destroy this GameObject safety(check null).</summary>
        /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
        /// <param name="detouchParent">set to parent = null.</param>
        public static void Destroy(this GameObject self, bool useDestroyImmediate = false, bool detouchParent = false)
        {
            if (self == null) return;

            if (detouchParent)
            {
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
                self.transform.SetParent(null);
#else
                self.transform.parent = null;
#endif
            }

            if (useDestroyImmediate)
            {
                GameObject.DestroyImmediate(self);
            }
            else
            {
                GameObject.Destroy(self);
            }
        }
    }
}