// See https://aka.ms/new-console-template for more information
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

var enumerableMethods = typeof(Enumerable).GetMethods()
    .Where(x => IsExtensionMethod(x))
    .ToArray();


var returnEnumerables = enumerableMethods
    .Where(x => x.ReturnType.Name == "IEnumerable`1")
    .GroupBy(x => x.Name)
    .OrderBy(x => x.Key)
    .ToArray();

// OrderBy/ThenBy is here but ok(fix manually...)
var others = enumerableMethods
    .Where(x => x.ReturnType.Name != "IEnumerable`1")
    .GroupBy(x => x.Name)
    .OrderBy(x => x.Key)
    .ToArray();


foreach (var item in returnEnumerables)
{
    EmitEnumerableTemplate(item);
    //Console.WriteLine(item.Key + ":" + item.Count());
}



static void EmitEnumerableTemplate(IGrouping<string, MethodInfo> methods)
{
    if (methods.Key != "Select") return; // TODO

    var className = methods.Key;
    var fileName = $"{className}.cs";

    var sb1 = new StringBuilder(); // ZLinq
    sb1.AppendLine("namespace ZLinq");
    sb1.AppendLine("{");
    sb1.AppendLine("    partial class ValueEnumerableExtensions");
    sb1.AppendLine("    {");

    var sb2 = new StringBuilder(); // ZLinq.Linq
    sb2.AppendLine("namespace ZLinq.Linq");
    sb2.AppendLine("{");

    var test = new StringBuilder(); // TODO:...

    var i = 0;
    foreach (var methodInfo in methods)
    {
        var suffix = (++i == 1) ? "" : i.ToString();
        var t = methodInfo.GetParameters()[0].ParameterType.GetGenericArguments()[0]; // this IEnumerable<T> source's T
        var enumerableType = $"{className}ValueEnumerable{suffix}";
        var baseGenericArguments = string.Join(", ", methodInfo.GetGenericArguments().Select(x => x.Name));
        var genericArguments = "TEnumerable" + (baseGenericArguments == "" ? "" : $", {baseGenericArguments}");
        var baseParameters = string.Join(", ", methodInfo.GetParameters().Skip(1).Select(x => $"{FormatType(x.ParameterType)} {x.Name}")); // skip TSource source
        var parameters = "TEnumerable source" + (baseParameters == "" ? "" : $", {baseParameters}");
        var baseParameterNames = string.Join(", ", methodInfo.GetParameters().Skip(1).Select(x => $"{x.Name}"));
        var parameterNames = "source" + (baseParameterNames == "" ? "" : $", {baseParameterNames}");
        var enumerableElementType = methodInfo.ReturnType.GetGenericArguments()[0].Name;

        // sb1
        sb1.AppendLine($"        public static {enumerableType}<{genericArguments}> Select<{genericArguments}>(this {parameters})");
        sb1.AppendLine($"            where TEnumerable : struct, IValueEnumerable<{t}>");
        sb1.AppendLine($"#if NET9_0_OR_GREATER");
        sb1.AppendLine($"            , allows ref struct");
        sb1.AppendLine($"#endif");
        sb1.AppendLine($"            => new({parameterNames});");
        sb1.AppendLine();

        sb1.AppendLine($"        public static ValueEnumerator<{enumerableType}<{genericArguments}>, {enumerableElementType}> GetEnumerator<{genericArguments}>(this {enumerableType}<{genericArguments}> source)");
        sb1.AppendLine($"            where TEnumerable : struct, IValueEnumerable<{t}>");
        sb1.AppendLine($"#if NET9_0_OR_GREATER");
        sb1.AppendLine($"            , allows ref struct");
        sb1.AppendLine($"#endif");
        sb1.AppendLine($"            => new(source);");
        sb1.AppendLine();

        // sb2
        sb2.AppendLine("""
    [StructLayout(LayoutKind.Auto)]
    [EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
    public ref
#else
    public
#endif
""");

        sb2.AppendLine($"    struct {enumerableType}<{genericArguments}>({parameters})");
        sb2.AppendLine($"        : IValueEnumerable<{enumerableElementType}>");
        sb2.AppendLine($"        where TEnumerable : struct, IValueEnumerable<{t}>");
        sb2.AppendLine("""
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
""");
        sb2.AppendLine("    {");
        sb2.AppendLine("        TEnumerable source = source;");
        sb2.AppendLine();
        // TryGetNonEnumeratedCount
        sb2.AppendLine("        public bool TryGetNonEnumeratedCount(out int count) => throw new NotImplementedException();");
        sb2.AppendLine();

        // TryGetSpan
        sb2.AppendLine($"        public bool TryGetSpan(out ReadOnlySpan<{enumerableElementType}> span)");
        sb2.AppendLine("        {");
        sb2.AppendLine($"            throw new NotImplementedException();");
        sb2.AppendLine($"            // span = default;");
        sb2.AppendLine($"            // return false;");
        sb2.AppendLine("        }");
        sb2.AppendLine();

        // TryGetNext
        sb2.AppendLine($"        public bool TryGetNext(out {enumerableElementType} current)");
        sb2.AppendLine("        {");
        sb2.AppendLine($"            throw new NotImplementedException();");
        sb2.AppendLine($"            // Unsafe.SkipInit(out current);");
        sb2.AppendLine($"            // return false;");
        sb2.AppendLine("        }");
        sb2.AppendLine();

        // Dispose
        sb2.AppendLine("        public void Dispose()");
        sb2.AppendLine("        {");
        sb2.AppendLine("            source.Dispose();");
        sb2.AppendLine("        }");

        sb2.AppendLine("    }");
        sb2.AppendLine();
    }

    sb1.Append("    }");
    sb1.Append("}");

    sb2.Append("}");

    var file = sb1.ToString() + Environment.NewLine + Environment.NewLine + sb2.ToString();

    Console.WriteLine(file);
}

static bool IsExtensionMethod(MethodInfo methodInfo)
{
    if (methodInfo == null) return false;
    return methodInfo.IsStatic && methodInfo.IsDefined(typeof(ExtensionAttribute), false);
}

static string FormatType(Type type)
{
    if (!type.IsGenericType) return type.Name;

    var genericName = type.GetGenericTypeDefinition().Name.Split('`')[0];
    var typeArguments = type.GetGenericArguments();
    var arguments = string.Join(", ", typeArguments.Select(t => FormatType(t)));

    return $"{genericName}<{arguments}>";
}