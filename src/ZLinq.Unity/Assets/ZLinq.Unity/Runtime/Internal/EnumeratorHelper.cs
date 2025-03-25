using System;

namespace ZLinq.Internal
{
    internal static class EnumeratorHelper
    {
        public static bool TryGetSliceRange(int sourceLength, Index offset, int destinationLength, out int start, out int count)
        {
            var sourceOffset = offset.GetOffset(sourceLength);
            if (unchecked((uint)sourceOffset) < sourceLength)
            {
                start = sourceOffset;
                count = Math.Min(sourceLength - sourceOffset, destinationLength);
                return true;
            }

            start = 0;
            count = 0;
            return false;
        }

        public static bool TryGetSlice<T>(ReadOnlySpan<T> source, Index offset, int destinationLength, out ReadOnlySpan<T> slice)
        {
            var sourceOffset = offset.GetOffset(source.Length);
            if (unchecked((uint)sourceOffset) < source.Length) // zero length is not allowed.
            {
                var count = Math.Min(source.Length - sourceOffset, destinationLength);
                slice = source.Slice(sourceOffset, count);
                return true;
            }

            slice = default;
            return false;
        }
    }
}