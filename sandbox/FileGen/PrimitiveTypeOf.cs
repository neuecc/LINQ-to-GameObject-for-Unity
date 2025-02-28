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
}
