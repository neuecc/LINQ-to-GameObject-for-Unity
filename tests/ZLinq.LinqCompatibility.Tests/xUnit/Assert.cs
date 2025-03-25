using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ZLinq;
using ZLinq.Linq;

namespace ZLinq.Tests;

/// <summary>
/// This class is used as xUnit's Assert.* methods proxy.
/// </summary>
/// <remarks>
/// Target test code must be placed at `ZLinq.Tests` namespace.
/// Otherwise, it cause conflicts with xUnit Assert methods.
/// </remarks>
public static partial class Assert
{
}
