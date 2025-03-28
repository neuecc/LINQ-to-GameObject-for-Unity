#if NET48

namespace System.Runtime.CompilerServices
{
    public static class RuntimeHelpers
    {
        public static bool IsReferenceOrContainsReferences<T>()
        {
            // only primitive
            if (typeof(T).IsPrimitive)
            {
                return false;
            }
            return true;
        }
    }
}

#endif
