using ZLinq.Tests;

namespace Xunit;

internal class ConditionalFactAttribute : FactAttribute
{
    public ConditionalFactAttribute(Type type, string key)
    {
        bool isEnabled = ConditionalUtils.IsEnable(type, key);
        if (isEnabled)
            return;

        Skip = $"Skipped by reason: {type.Name}: {key}";
    }
}
