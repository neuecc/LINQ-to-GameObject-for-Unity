#pragma warning disable

namespace UnityEngine
{
    public class GameObject
    {
        public Transform transform;
        public T GetComponent<T>() => default(T);
    }

    public class Transform
    {
        public GameObject gameObject;
        public int childCount;

        public T GetComponent<T>() => default(T);
        public Transform GetChild(int childCount) => default;
    }

    public class Component
    {

    }
}
