// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Original code: https://github.com/dotnet/runtime/blob/v9.0.3/src/libraries/Common/tests/TestUtilities/System/TestEnvironment.cs

namespace System;

public static class TestEnvironment
{
    public static bool IsStressModeEnabled
    {
        get
        {
            string? value = Environment.GetEnvironmentVariable("DOTNET_TEST_STRESS");
            return value != null && (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase));
        }
    }
}
