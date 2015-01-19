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
        #region Add

        /// <summary>
        /// <para>Adds the GameObject as children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static GameObject Add(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childOriginal == null) throw new ArgumentNullException("childOriginal");

            var child = UnityEngine.Object.Instantiate(childOriginal) as GameObject;
            // Unity 4.6, we can use SetParent but before that can't therefore use set localPosition/Rotation/Scale.
            child.transform.parent = parent.transform;
            child.layer = parent.layer;

            switch (cloneType)
            {
                case TransformCloneType.FollowParent:
                    child.transform.localPosition = parent.transform.localPosition;
                    child.transform.localScale = parent.transform.localScale;
                    child.transform.localRotation = parent.transform.localRotation;
                    break;
                case TransformCloneType.Origin:
                    child.transform.localPosition = Vector3.zero;
                    child.transform.localScale = Vector3.one;
                    child.transform.localRotation = Quaternion.identity;
                    break;
                case TransformCloneType.KeepOriginal:
                    child.transform.localPosition = childOriginal.transform.localPosition;
                    child.transform.localScale = childOriginal.transform.localScale;
                    child.transform.localRotation = childOriginal.transform.localRotation;
                    break;
                case TransformCloneType.DoNothing:
                default:
                    break;
            }

            if (setActive != null)
            {
                child.SetActive(setActive.Value);
            }
            if (specifiedName != null)
            {
                child.name = specifiedName;
            }

            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject as children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> Add(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (childOriginals == null) throw new ArgumentNullException("childOriginals");

            var list = new List<GameObject>();
            foreach (var childOriginal in childOriginals)
            {
                var child = Add(parent, childOriginal, cloneType, setActive, specifiedName);
                list.Add(child);
            }

            return list;
        }

        /// <summary>
        /// <para>Adds the GameObject as the first children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>      
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static GameObject AddFirst(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            var child = Add(parent, childOriginal, cloneType, setActive, specifiedName);
            child.transform.SetAsFirstSibling();
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject as the first children of this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>       
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> AddFirst(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            var child = Add(parent, childOriginals, cloneType, setActive, specifiedName);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                child[i].transform.SetAsFirstSibling();
            }
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject before this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static GameObject AddBeforeSelf(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex();

            var child = Add(root, childOriginal, cloneType, setActive, specifiedName);
            child.transform.SetSiblingIndex(sibilingIndex);
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject before this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> AddBeforeSelf(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex();
            var child = Add(root, childOriginals, cloneType, setActive, specifiedName);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                child[i].transform.SetSiblingIndex(sibilingIndex);
            }

            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject after this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginal">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static GameObject AddAfterSelf(this GameObject parent, GameObject childOriginal, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex() + 1;
            var child = Add(root, childOriginal, cloneType, setActive, specifiedName);
            child.transform.SetSiblingIndex(sibilingIndex);
            return child;
        }

        /// <summary>
        /// <para>Adds the GameObject after this GameObject. Target is cloned.</para>
        /// </summary>
        /// <param name="parent">Parent GameObject.</param>
        /// <param name="childOriginals">Clone Target.</param>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject. If null, doesn't set specified value.</param>
        /// <param name="specifiedName">Set name of child GameObject. If null, doesn't set specified value.</param>
        public static List<GameObject> AddAfterSelf(this GameObject parent, IEnumerable<GameObject> childOriginals, TransformCloneType cloneType = TransformCloneType.KeepOriginal, bool? setActive = null, string specifiedName = null)
        {
            var root = parent.Parent();
            if (root == null) throw new InvalidOperationException("The parent root is null");

            var sibilingIndex = parent.transform.GetSiblingIndex() + 1;
            var child = Add(root, childOriginals, cloneType, setActive, specifiedName);
            for (int i = child.Count - 1; i >= 0; i--)
            {
                child[i].transform.SetSiblingIndex(sibilingIndex);
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

            // Unity 4.6, we can use SetParent but before that can't therefore use set localPosition/Rotation/Scale.
            child.transform.parent = parent.transform;
            child.layer = parent.layer;

            switch (moveType)
            {
                case TransformMoveType.FollowParent:
                    child.transform.localPosition = parent.transform.localPosition;
                    child.transform.localScale = parent.transform.localScale;
                    child.transform.localRotation = parent.transform.localRotation;
                    break;
                case TransformMoveType.Origin:
                    child.transform.localPosition = Vector3.zero;
                    child.transform.localScale = Vector3.one;
                    child.transform.localRotation = Quaternion.identity;
                    break;
                case TransformMoveType.DoNothing:
                default:
                    break;
            }

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

        /// <summary>Destroy this GameObject safety(check null, deactive/detouch before destroy).</summary>
        /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
        public static void Destroy(this GameObject self, bool useDestroyImmediate = false)
        {
            if (self == null) return;

            self.SetActive(false); // deactive before destroy
            self.transform.parent = null; // detouch hierarchy before destroy

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