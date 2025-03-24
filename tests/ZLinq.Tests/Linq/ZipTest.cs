namespace ZLinq.Tests.Linq;

public class ZipTest
{
    [Fact]
    public void EmptySequences()
    {
        var xs = Array.Empty<int>();
        var ys = Array.Empty<string>();
        var zs = Array.Empty<bool>();

        // Two sequence zip
        {
            var actual = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable()).ToArray();
            actual.ShouldBeEmpty();
        }

        // Three sequence zip
        {
            var actual = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), zs.AsValueEnumerable()).ToArray();
            actual.ShouldBeEmpty();
        }

        // Two sequence zip with result selector
        {
            var actual = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), (x, y) => $"{x}:{y}").ToArray();
            actual.ShouldBeEmpty();
        }
    }

    [Fact]
    public void NonEmptySequencesEqualLength()
    {
        var xs = new[] { 1, 2, 3 };
        var ys = new[] { "a", "b", "c" };
        var zs = new[] { true, false, true };

        // Two sequence zip
        {
            var expected = new[] { (1, "a"), (2, "b"), (3, "c") };
            var actual = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable()).ToArray();

            actual.Length.ShouldBe(3);
            for (int i = 0; i < expected.Length; i++)
            {
                actual[i].ShouldBe(expected[i]);
            }
        }

        // Three sequence zip
        {
            var expected = new[] { (1, "a", true), (2, "b", false), (3, "c", true) };
            var actual = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), zs.AsValueEnumerable()).ToArray();

            actual.Length.ShouldBe(3);
            for (int i = 0; i < expected.Length; i++)
            {
                actual[i].ShouldBe(expected[i]);
            }
        }

        // Two sequence zip with result selector
        {
            var expected = new[] { "1:a", "2:b", "3:c" };
            var actual = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), (x, y) => $"{x}:{y}").ToArray();

            actual.ShouldBe(expected);
        }
    }

    [Fact]
    public void SequencesWithDifferentLengths()
    {
        // First sequence shorter
        {
            var xs = new[] { 1, 2 };
            var ys = new[] { "a", "b", "c", "d" };
            var zs = new[] { true, false, true, false };

            // Two sequence zip
            var actual1 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable()).ToArray();
            actual1.Length.ShouldBe(2); // Should be the length of the shorter sequence
            actual1[0].ShouldBe((1, "a"));
            actual1[1].ShouldBe((2, "b"));

            // Three sequence zip
            var actual2 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), zs.AsValueEnumerable()).ToArray();
            actual2.Length.ShouldBe(2); // Should be the length of the shortest sequence
            actual2[0].ShouldBe((1, "a", true));
            actual2[1].ShouldBe((2, "b", false));
        }

        // Second sequence shorter
        {
            var xs = new[] { 1, 2, 3, 4 };
            var ys = new[] { "a", "b" };
            var zs = new[] { true, false, true, false };

            // Two sequence zip
            var actual1 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable()).ToArray();
            actual1.Length.ShouldBe(2); // Should be the length of the shorter sequence
            actual1[0].ShouldBe((1, "a"));
            actual1[1].ShouldBe((2, "b"));

            // Three sequence zip with third being shortest
            var zShort = new[] { true };
            var actual2 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), zShort.AsValueEnumerable()).ToArray();
            actual2.Length.ShouldBe(1); // Should be the length of the shortest sequence
            actual2[0].ShouldBe((1, "a", true));
        }
    }

    [Fact]
    public void SequencesWithNullValues()
    {
        var xs = new int?[] { 1, null, 3 };
        var ys = new string?[] { "a", null, "c" };

        // Two sequence zip
        var actual1 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable()).ToArray();
        actual1.Length.ShouldBe(3);
        actual1[0].ShouldBe((1, "a"));
        actual1[1].ShouldBe((null, null));
        actual1[2].ShouldBe((3, "c"));

        // Two sequence zip with result selector handling nulls
        var actual2 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), (x, y) => $"{x ?? 0}:{y ?? "null"}").ToArray();
        actual2.Length.ShouldBe(3);
        actual2[0].ShouldBe("1:a");
        actual2[1].ShouldBe("0:null");
        actual2[2].ShouldBe("3:c");
    }

    [Fact]
    public void EnumeratorTryGetNonEnumeratedCount()
    {
        // Both sources have count
        {
            var xs = new[] { 1, 2, 3 };
            var ys = new[] { "a", "b", "c" };
            var zs = new[] { true, false, true };

            var zipEnumerable = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable());
            zipEnumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(3);

            var zipEnumerable2 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), zs.AsValueEnumerable());
            zipEnumerable2.TryGetNonEnumeratedCount(out count).ShouldBeTrue();
            count.ShouldBe(3);

            var zipEnumerable3 = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable(), (x, y) => x + y);
            zipEnumerable3.TryGetNonEnumeratedCount(out count).ShouldBeTrue();
            count.ShouldBe(3);
        }

        // Different lengths
        {
            var xs = new[] { 1, 2, 3, 4 };
            var ys = new[] { "a", "b" };

            var zipEnumerable = xs.AsValueEnumerable().Zip(ys.AsValueEnumerable());
            zipEnumerable.TryGetNonEnumeratedCount(out var count).ShouldBeTrue();
            count.ShouldBe(2); // Should be the minimum count
        }

        // One source doesn't support non-enumerated count
        {
            var xs = new[] { 1, 2, 3 };
            var ysIterator = TestUtil.Empty<string>().Concat(new[] { "a", "b", "c" });

            var zipEnumerable = xs.AsValueEnumerable().Zip(ysIterator.ToValueEnumerable());
            zipEnumerable.TryGetNonEnumeratedCount(out var count).ShouldBeFalse();
        }
    }

    [Fact]
    public void ZipEnumeratorDisposalTest()
    {
        var disposed1 = false;
        var disposed2 = false;
        var disposed3 = false;

        // Create test enumerables that track disposal
        var enum1 = new TrackingDisposableEnumerable<int>(new[] { 1, 2, 3 }, () => disposed1 = true);
        var enum2 = new TrackingDisposableEnumerable<string>(new[] { "a", "b", "c" }, () => disposed2 = true);
        var enum3 = new TrackingDisposableEnumerable<bool>(new[] { true, false, true }, () => disposed3 = true);

        // Two sequence zip
        {
            disposed1 = false;
            disposed2 = false;
            var zipEnumerable = enum1.AsValueEnumerable().Zip(enum2.AsValueEnumerable());

            // Just getting the enumerator doesn't dispose anything
            var enumerator = zipEnumerable.Enumerator;
            disposed1.ShouldBeFalse();
            disposed2.ShouldBeFalse();

            enumerator.TryGetNext(out _); // run once

            // Disposal of the zip enumerator should dispose both source enumerators

            enumerator.Dispose();
            disposed1.ShouldBeTrue();
            disposed2.ShouldBeTrue();
        }

        // Three sequence zip
        {
            disposed1 = false;
            disposed2 = false;
            disposed3 = false;
            var zipEnumerable = enum1.AsValueEnumerable().Zip(enum2.AsValueEnumerable(), enum3.AsValueEnumerable());

            // Just getting the enumerator doesn't dispose anything
            var enumerator = zipEnumerable.Enumerator;
            disposed1.ShouldBeFalse();
            disposed2.ShouldBeFalse();
            disposed3.ShouldBeFalse();

            enumerator.TryGetNext(out _); // run once

            // Disposal of the zip enumerator should dispose all three source enumerators
            enumerator.Dispose();
            disposed1.ShouldBeTrue();
            disposed2.ShouldBeTrue();
            disposed3.ShouldBeTrue();
        }

        // Two sequence zip with result selector
        {
            disposed1 = false;
            disposed2 = false;
            var zipEnumerable = enum1.AsValueEnumerable().Zip(enum2.AsValueEnumerable(), (x, y) => $"{x}:{y}");

            // Just getting the enumerator doesn't dispose anything
            var enumerator = zipEnumerable.Enumerator;
            disposed1.ShouldBeFalse();
            disposed2.ShouldBeFalse();

            enumerator.TryGetNext(out _); // run once

            // Disposal of the zip enumerator should dispose both source enumerators
            enumerator.Dispose();
            disposed1.ShouldBeTrue();
            disposed2.ShouldBeTrue();
        }
    }

    private class TrackingDisposableEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;
        private readonly Action _onDispose;

        public TrackingDisposableEnumerable(IEnumerable<T> inner, Action onDispose)
        {
            _inner = inner;
            _onDispose = onDispose;
        }

        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                foreach (var item in _inner)
                {
                    yield return item;
                }
            }
            finally
            {
                _onDispose();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
