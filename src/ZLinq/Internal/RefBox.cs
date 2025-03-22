namespace ZLinq.Internal;

internal sealed class RefBox<T> : IDisposable
    where T : struct, IDisposable
{
    T value;

    public RefBox(ref T value)
    {
        this.value = value;
    }

    public ref T GetValueRef() => ref value;

    public void Dispose()
    {
        value.Dispose();
    }
}
