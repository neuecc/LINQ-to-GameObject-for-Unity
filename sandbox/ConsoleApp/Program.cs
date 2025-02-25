using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;
using ZLinq.Linq;
using ZLinq.AutoInstrument;
using System.Runtime.InteropServices;

var xs = new[] { 1, 2, 3 };

var hoge = xs.Select(x => x).Where(x => x % 2 == 0).ToArray();

var enumerable = xs.AsSpan().AsValueEnumerable()
    .Where(x => x == 2)
    .Select(x => x * x);

var ok = enumerable.Count();



foreach (var item in enumerable)
{

}

//    .Select(x => x * x)
//  .Distinct()
// .Prepend(100);
//.Prepend(1000)
//.Append(99)
//.Distinct();

//var size = Marshal.SizeOf(enumerable);
var size = Unsafe.SizeOf<DistinctValueEnumerable<AppendValueEnumerable<PrependValueEnumerable<WhereValueEnumerable<SelectValueEnumerable<SpanValueEnumerable<int>, int, int>, int>, int>, int>, int>>();
Console.WriteLine(size);

//Console.WriteLine(Unsafe.SizeOf();
// .Select(x => x);
//.Select(x => x * 100)
//.Select(x => x * x);
//.((x, i) => x * 100);

//.ToArray();

var e3 = enumerable;

var array = e3.ToArray();

var zzz = enumerable.Select(takoyakix =>
{
    var note = takoyakix;
    return note * 2;
});

More();

foreach (var item in zzz)
{
    Console.WriteLine(item);
}

static void More()
{
    var xs = new[] { 1, 2, 3 };

    var enumerable = xs.AsValueEnumerable()
        .Select(x => x * x)
        .Where(x => x == 2);
    //.ToArray();

    var e3 = enumerable;

    // var array = e3.ToAS

    var zzz = e3.Select(takoyakix =>
    {
        var note = takoyakix;
        return note * 2;
    });

}


// xs.AsValueEnumerable().Select(x => x);`

//var json = JsonNode.Parse("""
//{
//    "nesting": {
//      "level1": {
//        "description": "First level of nesting",
//        "value": 100,
//        "level2": {
//          "description": "Second level of nesting",
//          "flags": [true, false, true],
//          "level3": {
//            "description": "Third level of nesting",
//            "coordinates": {
//              "x": 10.5,
//              "y": 20.75,
//              "z": -5.0
//            },
//            "level4": {
//              "description": "Fourth level of nesting",
//              "metadata": {
//                "created": "2025-02-15T14:30:00Z",
//                "modified": null,
//                "version": 2.1
//              },
//              "level5": {
//                "description": "Fifth level of nesting",
//                "settings": {
//                  "enabled": true,
//                  "threshold": 0.85,
//                  "options": ["fast", "accurate", "balanced"],
//                  "config": {
//                    "timeout": 30000,
//                    "retries": 3,
//                    "deepSetting": {
//                      "algorithm": "advanced",
//                      "parameters": [1, 1, 2, 3, 5, 8, 13]
//                    }
//                  }
//                }
//              }
//            }
//          }
//        }
//      },
//      "alternativePath": {
//        "branchA": {
//          "leaf1": "End of branch A path"
//        },
//        "branchB": {
//          "section1": {
//            "leaf2": "End of branch B section 1"
//          },
//          "section2": {
//            "leaf3": "End of branch B section 2"
//          }
//        }
//      }
//    }
//}
//""");


//var origin = json!["nesting"]!["level1"]!["level2"]!;

//foreach (var item in origin.Descendants().Select((x, i) => i))
//{
//    Console.WriteLine(item);
//}

//foreach (var item in origin.Descendants().Where(x => x.Name == "hoge"))
//{
//    if (item.Node == null)
//    {
//        Console.WriteLine(item.Name);
//    }
//    else
//    {
//        Console.WriteLine(item.Node.GetPath() + ":" + item.Name);
//    }
//}

// je.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object


namespace ZLinq.AutoInstrument
{
    public static class AutoInstrumentLinq
    {
        public static SelectValueEnumerable<ArrayValueEnumerable<TSource>, TSource, TResult> Select<TSource, TResult>(this TSource[] source, Func<TSource, TResult> selector)
        {
            return source.AsValueEnumerable().Select(selector);
        }
    }
}