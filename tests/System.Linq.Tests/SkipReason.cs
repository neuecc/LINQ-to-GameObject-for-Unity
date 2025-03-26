namespace System;

public static class SkipReason
{
    public const string FailedByDefault = "Test failed on .NET v9.0.3";

    public const string RefStruct = "There is no compatibility because ZLinq use `ref struct`";

    public const string ZLinq_Issue0070 = "See: https://github.com/Cysharp/ZLinq/issues/70";
}
