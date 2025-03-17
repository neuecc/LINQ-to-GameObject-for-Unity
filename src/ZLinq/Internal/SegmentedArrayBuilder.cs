using System.Buffers;

namespace ZLinq.Internal;

[StructLayout(LayoutKind.Auto)]
internal ref struct SegmentedArrayBuilder<T> : IDisposable
{
    // Array.MaxLength = 2147483591
    T[]? array0;  // 16              total:16
    T[]? array1;  // 32              total:48
    T[]? array2;  // 64              total:112
    T[]? array3;  // 128             total:240
    T[]? array4;  // 256             total:496
    T[]? array5;  // 512             total:1008
    T[]? array6;  // 1024            total:2032
    T[]? array7;  // 2048            total:4080
    T[]? array8;  // 4096            total:8176
    T[]? array9;  // 8192            total:16368
    T[]? array10; // 16384           total:32752
    T[]? array11; // 32768           total:65520
    T[]? array12; // 65536           total:131056
    T[]? array13; // 131072          total:262128
    T[]? array14; // 262144          total:524272
    T[]? array15; // 524288          total:1048560
    T[]? array16; // 1048576         total:2097136
    T[]? array17; // 2097152         total:4194288
    T[]? array18; // 4194304         total:8388592
    T[]? array19; // 8388608         total:16777200
    T[]? array20; // 16777216        total:33554416
    T[]? array21; // 33554432        total:67108848
    T[]? array22; // 67108864        total:134217712
    T[]? array23; // 134217728       total:268435440
    T[]? array24; // 268435456       total:536870896
    T[]? array25; // 536870912       total:1073741808
    T[]? array26; // 1073741824      total:2147483632 (over)

    T[]? currentSegment;
    int indexInSegment;
    int segmentIndex;
    int count;

    public int Count => count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T value)
    {
        ref var segment = ref currentSegment;

        if (segment == null)
        {
            Init(value);
            return;
        }
        else if (segment.Length == indexInSegment)
        {
            SlowAdd(value);
            return;
        }

        segment[indexInSegment++] = value;
        count++;
    }

    void Init(T value)
    {
        ref var segment = ref array0;
        segment = ArrayPool<T>.Shared.Rent(16);
        currentSegment = segment;

        segment[indexInSegment++] = value;
        count++;
    }

    void SlowAdd(T value)
    {
        ref var segment = ref currentSegment;
        segmentIndex++;

        switch (segmentIndex)
        {
            case 0: { segment = ref array0; break; }
            case 1: { segment = ref array1; break; }
            case 2: { segment = ref array2; break; }
            case 3: { segment = ref array3; break; }
            case 4: { segment = ref array4; break; }
            case 5: { segment = ref array5; break; }
            case 6: { segment = ref array6; break; }
            case 7: { segment = ref array7; break; }
            case 8: { segment = ref array8; break; }
            case 9: { segment = ref array9; break; }
            case 10: { segment = ref array10; break; }
            case 11: { segment = ref array11; break; }
            case 12: { segment = ref array12; break; }
            case 13: { segment = ref array13; break; }
            case 14: { segment = ref array14; break; }
            case 15: { segment = ref array15; break; }
            case 16: { segment = ref array16; break; }
            case 17: { segment = ref array17; break; }
            case 18: { segment = ref array18; break; }
            case 19: { segment = ref array19; break; }
            case 20: { segment = ref array20; break; }
            case 21: { segment = ref array21; break; }
            case 22: { segment = ref array22; break; }
            case 23: { segment = ref array23; break; }
            case 24: { segment = ref array24; break; }
            case 25: { segment = ref array25; break; }
            case 26: { segment = ref array26; break; }
            default: break;
        }

        segment = ArrayPool<T>.Shared.Rent(indexInSegment * 2);
        indexInSegment = 0;
        currentSegment = segment;

        segment[indexInSegment++] = value;
        count++;
    }

    public void CopyTo(Span<T> dest)
    {
        if (count == 0) return;

        for (int i = 0; i <= segmentIndex; i++)
        {
            T[] segment = default!;
            switch (i)
            {
                case 0: { segment = array0!; break; }
                case 1: { segment = array1!; break; }
                case 2: { segment = array2!; break; }
                case 3: { segment = array3!; break; }
                case 4: { segment = array4!; break; }
                case 5: { segment = array5!; break; }
                case 6: { segment = array6!; break; }
                case 7: { segment = array7!; break; }
                case 8: { segment = array8!; break; }
                case 9: { segment = array9!; break; }
                case 10: { segment = array10!; break; }
                case 11: { segment = array11!; break; }
                case 12: { segment = array12!; break; }
                case 13: { segment = array13!; break; }
                case 14: { segment = array14!; break; }
                case 15: { segment = array15!; break; }
                case 16: { segment = array16!; break; }
                case 17: { segment = array17!; break; }
                case 18: { segment = array18!; break; }
                case 19: { segment = array19!; break; }
                case 20: { segment = array20!; break; }
                case 21: { segment = array21!; break; }
                case 22: { segment = array22!; break; }
                case 23: { segment = array23!; break; }
                case 24: { segment = array24!; break; }
                case 25: { segment = array25!; break; }
                case 26: { segment = array26!; break; }
                default: break;
            }

            if (segmentIndex != i)
            {
                // copy full
                segment.AsSpan().CopyTo(dest);
                dest = dest.Slice(segment.Length);
            }
            else
            {
                // last
                segment.AsSpan(0, indexInSegment).CopyTo(dest);
            }
        }
    }

    public T[] ToArray()
    {
        if (count == 0) return Array.Empty<T>();

        var array = GC.AllocateUninitializedArray<T>(count);
        CopyTo(array);
        return array;
    }

    public void Dispose()
    {
        if (currentSegment == null) return;

        for (int i = 0; i <= segmentIndex; i++)
        {
            ref T[]? segment = ref currentSegment;
            switch (segmentIndex)
            {
                case 0: { segment = ref array0; break; }
                case 1: { segment = ref array1; break; }
                case 2: { segment = ref array2; break; }
                case 3: { segment = ref array3; break; }
                case 4: { segment = ref array4; break; }
                case 5: { segment = ref array5; break; }
                case 6: { segment = ref array6; break; }
                case 7: { segment = ref array7; break; }
                case 8: { segment = ref array8; break; }
                case 9: { segment = ref array9; break; }
                case 10: { segment = ref array10; break; }
                case 11: { segment = ref array11; break; }
                case 12: { segment = ref array12; break; }
                case 13: { segment = ref array13; break; }
                case 14: { segment = ref array14; break; }
                case 15: { segment = ref array15; break; }
                case 16: { segment = ref array16; break; }
                case 17: { segment = ref array17; break; }
                case 18: { segment = ref array18; break; }
                case 19: { segment = ref array19; break; }
                case 20: { segment = ref array20; break; }
                case 21: { segment = ref array21; break; }
                case 22: { segment = ref array22; break; }
                case 23: { segment = ref array23; break; }
                case 24: { segment = ref array24; break; }
                case 25: { segment = ref array25; break; }
                case 26: { segment = ref array26; break; }
                default: break;
            }

            if (segment != null)
            {
                ArrayPool<T>.Shared.Return(segment!, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
                segment = null;
            }
        }

        currentSegment = null;
        indexInSegment = segmentIndex = count = 0;
    }
}