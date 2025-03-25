using System.Runtime.CompilerServices;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

public static partial class Assert
{
    internal static void NotEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
    {
        var results1 = expected.ToArray();
        var results2 = actual.ToArray();
        Xunit.Assert.Equal(results1, results2);
    }
}
