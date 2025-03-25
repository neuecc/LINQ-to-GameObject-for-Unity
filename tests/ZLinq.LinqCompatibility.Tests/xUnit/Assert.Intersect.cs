using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void NotSame<T>(
        ValueEnumerable<Select<FromEnumerable<T>, T, T>, T> expected,
        ValueEnumerable<OfType<Select<FromEnumerable<T>, T, T>, T, T>, T> actual)
    {
        throw new NotSupportedException();
    }
}
