using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZLinq.Internal;

internal static class RandomShared
{
    public static void Shuffle<T>(T[] array)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(array.AsSpan());
#else
        Shared.Value.Shuffle(array.AsSpan());
#endif
    }

    public static void Shuffle<T>(Span<T> span)
    {
#if NET8_0_OR_GREATER
        Random.Shared.Shuffle(span);
#else
        Shared.Value.Shuffle(span);
#endif
    }

#if !NET8_0_OR_GREATER

    private static ThreadLocal<Random> Shared = new ThreadLocal<Random>(() =>
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var buffer = new byte[sizeof(int)];
            rng.GetBytes(buffer);
            var seed = BitConverter.ToInt32(buffer, 0);
            return new Random(seed);
        }
    });

    static void Shuffle<T>(this Random random, Span<T> values)
    {
        int n = values.Length;

        for (int i = 0; i < n - 1; i++)
        {
            int j = random.Next(i, n);

            if (j != i)
            {
                T temp = values[i];
                values[i] = values[j];
                values[j] = temp;
            }
        }
    }

#endif
}