#if NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Text;

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
