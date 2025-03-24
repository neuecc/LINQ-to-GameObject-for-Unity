namespace ZLinq.Internal;

// store struct to heap
internal sealed class RefBox<T> : IDisposable
    where T : struct, IDisposable
{
    T value;
    bool isDisposed;

    public RefBox(T value)
    {
        this.value = value;
    }

    public ref T GetValueRef() => ref value;

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            value.Dispose();
        }
    }
}
