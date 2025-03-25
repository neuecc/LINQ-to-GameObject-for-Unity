using System.Reflection;

namespace System.Diagnostics;

#nullable disable

internal class DebuggerAttributeInfo
{
    public object Instance { get; set; }
    public IEnumerable<PropertyInfo> Properties { get; set; }
}
