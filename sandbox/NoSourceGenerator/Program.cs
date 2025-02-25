




using ZLinq;

var src = new int[5];

var dest = new int[3]; // dest is smaller

// ArgumentException: Destination is too short.
src.AsSpan().CopyTo(dest);


