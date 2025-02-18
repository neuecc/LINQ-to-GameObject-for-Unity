#pragma warning disable

using System.Collections.Generic;

namespace UnityEngine
{
    public class GameObject
    {
        public string name;
        public Transform transform;
        public T GetComponent<T>() => default(T);

        public GameObject(string name)
        {
            this.name = name;
            transform = new Transform();
            transform.gameObject = this;
        }

        public GameObject(string name, params GameObject[] childs)
            : this(name)
        {
            foreach (var child in childs)
            {
                child.transform.parent = transform;
                child.transform._siblingIndex = transform.childCount;
                transform._childList.Add(child.transform);
            }
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class Transform
    {
        internal int _siblingIndex;
        internal List<Transform> _childList = new List<Transform>();

        public GameObject gameObject;
        public Transform parent;
        public int childCount => _childList.Count;
        public int GetSiblingIndex() => _siblingIndex;
        public T GetComponent<T>() => default(T);
        public Transform GetChild(int index) => _childList[index];

        public override string ToString()
        {
            return gameObject.name;
        }
    }

    public class Component
    {

    }
}
