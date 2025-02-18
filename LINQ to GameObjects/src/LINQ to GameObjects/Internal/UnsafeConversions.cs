using System.Runtime.CompilerServices;
using UnityEngine;

namespace Cysharp.Linq.Internal
{
    // Once C# 11's static abstract members become available, this kind of code will no longer be necessary.
    internal static class UnsafeConversions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Transform ConvertToTransformFrom<T>(T obj)
        {
            if (typeof(T) == typeof(Transform))
            {
                return Unsafe.As<T, Transform>(ref obj);
            }
            else if (typeof(T) == typeof(GameObject))
            {
                return Unsafe.As<T, GameObject>(ref Unsafe.AsRef(obj)).transform;
            }
            else
            {
                return default!;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ConvertTransformTo<T>(Transform transform)
        {
            if (typeof(T) == typeof(Transform))
            {
                return Unsafe.As<Transform, T>(ref transform);
            }
            else if (typeof(T) == typeof(GameObject))
            {
                return Unsafe.As<GameObject, T>(ref Unsafe.AsRef(transform.gameObject));
            }
            else
            {
                return default!;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TComponent GetComponentFrom<T, TComponent>(T obj)
            where TComponent : Component
        {
            if (typeof(T) == typeof(Transform))
            {
                return Unsafe.As<T, Transform>(ref obj).GetComponent<TComponent>();
            }
            else if (typeof(T) == typeof(GameObject))
            {
                return Unsafe.As<T, GameObject>(ref Unsafe.AsRef(obj)).GetComponent<TComponent>();
            }
            else
            {
                return default!;
            }
        }
    }
}
