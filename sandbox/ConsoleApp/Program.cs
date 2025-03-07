using ZLinq;


//Span<int> xs = stackalloc int[255];

// caseof bool, char, decimal, nint...

// var xs = new[] { 1, 2, 3, 4, 5 };


var xs = new int[] { 5, 2, 8, 1, 9 };

// Custom comparer that orders by the remainder when divided by 3
var customComparer = Comparer<int>.Create((x, y) =>
    (x % 3).CompareTo(y % 3));

var ordered = xs.AsValueEnumerable().OrderBy(x => x, customComparer);
ordered.ToArray();

foreach (var item in ordered)
{
    Console.WriteLine(item);
}


class Person
{
    public string FirstName { get; }
    public string LastName { get; }
    public int Age { get; }

    public Person(string firstName, string lastName, int age)
    {
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}

//Console.WriteLine(hoge.Length);


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
//      }
//    }
//}
//""");


//var origin = json!["nesting"]!["level1"]!["level2"]!;

//foreach (var item in origin.Descendants().Select(x => x.Node).OfType(default(JsonArray)))
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
        //public static SelectValueEnumerable<FromArray<TSource>, TSource, TResult> Select<TSource, TResult>(this TSource[] source, Func<TSource, TResult> selector)
        //{
        //    return source.AsValueEnumerable().Select(selector);
        //}

        //public static ConcatValueEnumerable2<RangeValueEnumerable, int, ArrayValueEnumerable<int>> Concat2(this RangeValueEnumerable source, ArrayValueEnumerable<int> second)
        //{
        //    return ValueEnumerableExtensions.Concat2<RangeValueEnumerable, int, ArrayValueEnumerable<int>>(source, second);
        //}
    }
}