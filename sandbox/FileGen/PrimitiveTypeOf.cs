using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileGen;

public partial class Commands
{
    readonly static string[] PrimitiveTypes = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "bool", "char", "decimal", "nint", "nuint"];
    readonly static string[] PrimitiveTypesPlusString = [.. PrimitiveTypes, "string"];
    readonly static string[] PrimitiveNumbers = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "float", "double", "decimal", "nint", "nuint"];
    readonly static string[] PrimitiveNumbersWithoutFloat = ["byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "double", "decimal", "nint", "nuint"];

    public void TypeOfContains()
    {
        var sb = new StringBuilder();
        foreach (var type in PrimitiveTypesPlusString)
        {
            var code = $$"""
            else if (typeof(T) == typeof({{type}}))
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<T, {{type}}> (ref MemoryMarshal.GetReference(source)), source.Length);
                return MemoryExtensions.Contains(span, Unsafe.As<T, {{type}}>(ref value));
            }
""";
            sb.AppendLine(code);
        }

        Console.WriteLine(sb.ToString());
    }

    public void Sum()
    {
        var sb = new StringBuilder();
        sb.AppendLine("#region generate from FileGen.Commands.Sum");
        foreach (var type in PrimitiveNumbersWithoutFloat)
        {
            var code = $$"""
        else if (typeof(TSource) == typeof({{type}}))
        {
            using (source)
            {
                {{type}} sum = default;
                while (source.TryGetNext(out var item))
                {
                    checked { sum += Unsafe.As<TSource, {{type}}>(ref item); }
                }
                return Unsafe.As<{{type}}, TSource>(ref sum);
            }
        }
""";
            sb.AppendLine(code);
        }
        sb.AppendLine("#endregion");

        Console.WriteLine(sb.ToString());

    }
}
