#if !NET8_0_OR_GREATER

using System.Diagnostics.CodeAnalysis;

namespace ZLinq
{
    internal static class ArgumentNullException
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
            {
                Throw(paramName);
            }
        }

        [DoesNotReturn]
        static void Throw(string? paramName) => throw new global::System.ArgumentNullException(paramName);
    }
}

#endif
