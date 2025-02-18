using System.Collections.Generic;

namespace Cysharp.Linq
{
	partial class StructEnumerableExtensions
	{
		public static EnumerableStructEnumerable<T> AsStructEnumerable<T>(this IEnumerable<T> source)
		{
			return new(source);
		}

		public static ArrayStructEnumerable<T> AsStructEnumerable<T>(this T[] source)
		{
			return new(source);
		}

		public static ListStructEnumerable<T> AsStructEnumerable<T>(this List<T> source)
		{
			return new(source);
		}
	}

	public readonly struct EnumerableStructEnumerable<T> : IStructEnumerable<T, EnumerableStructEnumerable<T>.Enumerator>
	{
		readonly IEnumerable<T> source;

		public EnumerableStructEnumerable(IEnumerable<T> source)
		{
			this.source = source;
		}

		public Enumerator GetEnumerator() => new(source);

		public bool TryGetNonEnumeratedCount(out int count)
		{
			if (source is ICollection<T> c)
			{
				count = c.Count;
				return true;
			}
			else if (source is IReadOnlyCollection<T> rc)
			{
				count = rc.Count;
				return true;
			}
			count = 0;
			return false;
		}

		public struct Enumerator : IStructEnumerator<T>
		{
			readonly IEnumerable<T> source;
			IEnumerator<T>? enumerator;

			public T Current { get; private set; }

			public Enumerator(IEnumerable<T> source)
			{
				this.source = source;
				this.enumerator = null;
				this.Current = default!;
			}

			public bool MoveNext()
			{
				if (enumerator == null)
				{
					enumerator = source.GetEnumerator();
				}

				if (enumerator.MoveNext())
				{
					Current = enumerator.Current;
					return true;
				}
				return false;
			}

			public void Dispose()
			{
				if (enumerator != null)
				{
					enumerator.Dispose();
				}
			}
		}
	}

	public readonly struct ArrayStructEnumerable<T> : IStructEnumerable<T, ArrayStructEnumerable<T>.Enumerator>
	{
		readonly T[] source;

		public ArrayStructEnumerable(T[] source)
		{
			this.source = source;
		}

		public Enumerator GetEnumerator() => new(source);

		public bool TryGetNonEnumeratedCount(out int count)
		{
			count = source.Length;
			return false;
		}

		public struct Enumerator : IStructEnumerator<T>
		{
			readonly T[] source;
			int index;

			public T Current { get; private set; }

			public Enumerator(T[] source)
			{
				this.source = source;
				this.index = 0;
				this.Current = default!;
			}

			public bool MoveNext()
			{
				if (index < source.Length)
				{
					Current = source[index++];
					return true;
				}
				return false;
			}

			public void Dispose()
			{
			}
		}
	}

	public readonly struct ListStructEnumerable<T> : IStructEnumerable<T, ListStructEnumerable<T>.Enumerator>
	{
		readonly List<T> source;

		public ListStructEnumerable(List<T> source)
		{
			this.source = source;
		}

		public Enumerator GetEnumerator() => new(source);

		public bool TryGetNonEnumeratedCount(out int count)
		{
			count = source.Count;
			return true;
		}

		public struct Enumerator : IStructEnumerator<T>
		{
			readonly List<T> source;
			List<T>.Enumerator enumerator;
			bool isInit;

			public T Current { get; private set; }

			public Enumerator(List<T> source)
			{
				this.source = source;
				this.enumerator = default;
				this.isInit = false;
				this.Current = default!;
			}

			public bool MoveNext()
			{
				if (!isInit)
				{
					isInit = true;
					enumerator = source.GetEnumerator();
				}

				if (enumerator.MoveNext())
				{
					Current = enumerator.Current;
					return true;
				}
				return false;
			}

			public void Dispose()
			{
				if (isInit)
				{
					enumerator.Dispose();
				}
			}
		}
	}
}
