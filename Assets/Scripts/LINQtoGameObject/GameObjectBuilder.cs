using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    /// <summary>
    /// Functional Construction inspired by LINQ to XML. See: http://msdn.microsoft.com/en-us/library/bb387019.aspx
    /// </summary>
    public class GameObjectBuilder
    {
        readonly GameObject original;
        readonly IEnumerable<GameObjectBuilder> children;

        /// <summary>
        /// <para>Functional Construction inspired by LINQ to XML.</para>
        /// <para>See: http://msdn.microsoft.com/en-us/library/bb387019.aspx</para>
        /// </summary>
        public GameObjectBuilder(GameObject original, params GameObjectBuilder[] children)
            : this(original, (IEnumerable<GameObjectBuilder>)children)
        {

        }

        /// <summary>
        /// <para>Functional Construction inspired by LINQ to XML.</para>
        /// <para>See: http://msdn.microsoft.com/en-us/library/bb387019.aspx</para>
        /// </summary>
        public GameObjectBuilder(GameObject original, IEnumerable<GameObjectBuilder> children)
        {
            this.original = original;
            this.children = children;
        }

        /// <summary>Instantiate tree objects.</summary>
        public GameObject Instantiate()
        {
            return Instantiate(TransformCloneType.KeepOriginal);
        }

        /// <summary>Instantiate tree objects.</summary>
        /// <param name="setActive">Set activates/deactivates child GameObject.</param>
        public GameObject Instantiate(bool setActive)
        {
            return Instantiate(TransformCloneType.KeepOriginal);
        }

        /// <summary>Instantiate tree objects.</summary>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        public GameObject Instantiate(TransformCloneType cloneType)
        {
            var root = UnityEngine.Object.Instantiate(original) as GameObject;
            InstantiateChildren(root, cloneType, null);
            return root;
        }

        /// <summary>Instantiate tree objects.</summary>
        /// <param name="cloneType">Choose set type of cloned child GameObject's localPosition/Scale/Rotation.</param>     
        /// <param name="setActive">Set activates/deactivates child GameObject.</param>
        public GameObject Instantiate(TransformCloneType cloneType, bool setActive)
        {
            var root = UnityEngine.Object.Instantiate(original) as GameObject;
            InstantiateChildren(root, cloneType, setActive);
            return root;
        }

        void InstantiateChildren(GameObject root, TransformCloneType cloneType, bool? setActive)
        {
            foreach (var child in children)
            {
                var childRoot = root.Add(child.original, cloneType, setActive);
                child.InstantiateChildren(childRoot, cloneType, setActive);
            }
        }
    }
}