namespace Xunit;

internal static class ConditionalUtils
{
    public static bool IsEnable(Type type, string key)
    {
        if (type == typeof(TestEnvironment))
        {
            switch (key)
            {
                case "IsStressModeEnabled" when TestEnvironment.IsStressModeEnabled:
                    return true;
                default:
                    return false;
            }
        }

        if (type == typeof(PlatformDetection))
        {
            switch (key)
            {
                case "IsThreadingSupported" when PlatformDetection.IsThreadingSupported:
                case "IsDebuggerTypeProxyAttributeSupported" when PlatformDetection.IsDebuggerTypeProxyAttributeSupported:

#if NET10_0_OR_GREATER
                case "IsLinqSpeedOptimized" when PlatformDetection.IsLinqSpeedOptimized:
#else
                case "IsSpeedOptimized" when PlatformDetection.IsSpeedOptimized:
#endif
                    return true;
                default:
                    return false;
            }
        }

        return false;
    }
}
