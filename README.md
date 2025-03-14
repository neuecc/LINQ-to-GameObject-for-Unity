ZLinq
===
Zero allocation LINQ with Span and LINQ to SIMD, LINQ to Tree(FileSystem, GameObject, etc...) for all .NET platforms and Unity.

> [!IMPORTANT]
> このライブラリーは現在プレビューです。ほとんどのメソッドが実装されていますが、一部はまだNotImplementedExceptionをthrowします。

* .NET 10のLINQ to Objectsとの99%の互換性(Shuffle, RightJoin, LeftJoinを含む)
* `IValueEnumerable`によるstructベースのEnumerableにより基本的なチェーンはアロケーションフリー
* Source Generatorにより型推論を補完するハイブリッドデザイン
* .NET 9/C# 13の`allows ref struct`によるSpanのLINQ化のフル対応
* ツリー構造のオブジェクトを拡張するLINQ to Tree(ビルトインでFileSystem, JSON(for System.Text.json), GameObject(for Unity))
* SIMD化可能な箇所の自動適用と任意の処理を記述できるLINQ to SIMD
* 過去の私のLINQ実装(linq.js, [LINQ to GameObject](http://u3d.as/content/neuecc/linq-to-game-object), SimdLinq, UniRx, R3)とゼロアロケーションシリーズ(ZString, ZLogger)の融合

実験的なライブラリではなく、実用的なライブラリを目指しました。また、ゲームのような高負荷な要求にも耐えられるよう設計しています。

NuGetからインストール可能です。Unityでの利用はUnityセクションを参照してください。

> dotnet add ZLinq

ZLinqのチェーンは基本的に以下のインターフェイスを内部的に用いています。

```csharp
// struct version of IEnumerable<T> and IEnumerator<T>
public interface IValueEnumerable<T> : IDisposable
{
    bool TryGetNext(out T current); // as MoveNext + Current

    // Optimize options
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetSpan(out ReadOnlySpan<T> span);
    bool TryCopyTo(Span<T> destination);
}
```

strcutベースに変更したこと以外に、MoveNextとCurrentを一体化したことによりイテレーターの呼び出し回数を削減しています。また、structであることにより内部の状態を自動的にコピーするため、EnumerableとEnumeratorを一体化することにより、型の複雑化を減少しています。

```csharp
public static Where<TEnumerable, TSource> Where<TEnumerable, TSource>(this TEnumerable source, Func<TSource, Boolean> predicate)
    where TEnumerable : struct, IValueEnumerable<TSource>, allows ref struct
````

オペレーターはこのようなメソッドシグネチャになりますが、C#はジェネリクスの制約から型を推論する(この場合はTSourceの型をTEnumerableから決定する)ことが出来ません([dotnet/csharplang#6930](https://github.com/dotnet/csharplang/discussions/6930))。そのため、従来のStruct LINQのアプローチは、インスタンスメソッドに全てのオペレーターの組み合わせを実装する、結果として100000近くのメソッドや巨大なアセンブリサイズを引き起こしていました。

ZLinqではSource Generatorにより、部分的に利用箇所に応じてTEnumerableを具象型に変換した拡張メソッドを生成するというハイブリッドなアプローチを採用することにより、必要最小限のアセンブリサイズに留めることに成功しました。

<details><summary>Generated Inference Helper Code</summary>

![](Images/typeinference.jpg)

</details>

構造体ベースのLINQは、ジェネリクスが連鎖するため、型名が非常に読みにくくなります、例えばLinqAFの場合はこのような型となります。

<details><summary>LinqAf IntelliSense</summary>

![](Images/linqaf_intellisense.jpg)

</details>

ZLinqでは生成される型にも気を使って、限りなくリーダブルなものになるように設計しています。

<details><summary>ZLinq IntelliSense</summary>

![](Images/ZLinqIntellisense.jpg)

</details>

また、`TryGetNonEnumeratedCount(out int count)`, `TryGetSpan(out ReadOnlySpan<T> span)`, `TryCopyTo(Span<T> destination)` がインターフェイス自体に定義されることにより、柔軟な最適化を可能にしています。例えばTake+Skipは全てSpanのSliceで表現できるため、元のソースがSpanに変換できるものであれば、TryGetSpanの連鎖でSpanのSliceが送られます。ToArrayでは、シーケンスの値が算出可能な場合は、事前に固定長の配列を用意し、さらにTryCopyToで最終配列へ直接書き込み可能なオペレーターであれば、直接書き込むような最適化が入ります。一部のメソッドではSpanが取得可能な場合はSIMDを使った高速化も自動で行います。

Gettting Started
---
`using ZLinq;`し、イテレート可能な型から`AsValueEnumerable()`を呼ぶと、ZLinqによるゼロアロケーションのLINQが利用できます。また、`Range`, `Repeat`, `Empty`は`ValueEnumerbale`に定義されています。

```csharp
using ZLinq;

var source = new int[] { 1, 2, 3, 4, 5 };

// AsValueEnumerableを呼び出すことでZLinqが適用される
var seq1 = source.AsValueEnumerable().Where(x => x % 2 == 0);

// Spanにも問題なく適用可能(allows ref structが利用可能な.NET 9/C# 13環境のみ)
Span<int> span = stackalloc int[5] { 1, 2, 3, 4, 5 };
var seq2 = span.AsValueEnumerable().Select(x => x * x);
```

> Source Generatorが生成完了するまで一時的に入力補完が止まることがあります。最近のVisual StudioはSource Generatorの実行タイミングが保存時となっているため、明示的に保存したり、動作が停止している際はコンパイルする必要があります。コードエディタに支障を感じる場合は、通常のLINQで記述した後に、AsValueEnumerable()を加えることでスムーズに書くことも可能です。メソッドのシグネチャはほぼ完全な互換があります。

> Source Generatorの制限として、コード解析起動のトリガーの都合上、メソッドチェーンを一時変数に置いて継続することはできません。foreachを除き、全てのオペレーターはメソッドチェーン上に書く必要があります。

LINQ to Tree
---
LINQ to XMLによって、軸を中心にクエリするという概念がC#にもたらされました。XMLを使うことがなくても、同様のAPIはRoslynにも搭載され、SyntaxTreeの探索に有効活用されています。ZLinqではこの概念を拡張可能にし、あらゆるTreeとみなせるものに`Ancestors`, `Children`, `Descendants`, `BeforeSelf`, `AfterSelf`が適用可能になります。

![](Images/axis.jpg)

具体的には以下のインターフェイスを実装したstructを定義することで、イテレート可能になります。

```csharp
public interface ITraversable<TTraversable, T> : IDisposable
    where TTraversable : struct, ITraversable<TTraversable, T> // self
{
    T Origin { get; }
    TTraversable ConvertToTraversable(T next); // for Descendants
    bool TryGetHasChild(out bool hasChild); // optional: optimize use for Descendants
    bool TryGetChildCount(out int count);   // optional: optimize use for Children
    bool TryGetParent(out T parent); // for Ancestors
    bool TryGetNextChild(out T child); // for Children | Descendants
    bool TryGetNextSibling(out T next); // for AfterSelf
    bool TryGetPreviousSibling(out T previous); // BeforeSelf
}
```

標準でパッケージとしてFileSystemInfoとJsonNodeに適用したものを用意してあります。また、Unity向けにはGameObjectとTransformに適用可能です。

### FileSystem

> dotnet add ZLinq.FileSystem

```csharp
using ZLinq;

var root = new DirectoryInfo("C:\\Program Files (x86)\\Steam");

// FileSystemInfo(FileInfo/DirectoryInfo) can call `Ancestors`, `Children`, `Descendants`, `BeforeSelf`, `AfterSelf`
var groupByName = root.Descendants()
    .OfType(default(FileInfo))
    .Where(x => x.Extension == ".dll")
    .GroupBy(x => x.Name)
    .Select(x => (FileName: x.Key, Count: x.Count()))
    .OrderByDescending(x => x.Count);

foreach (var item in groupByName)
{
    Console.WriteLine(item);
}
```

### JSON(System.Text.Json)

> dotnet add ZLinq.Json

```csharp
using ZLinq;

// System.Text.Json's JsonNode is the target of LINQ to JSON(not JsonDocument/JsonElement).
var json = JsonNode.Parse("""
{
    "nesting": {
      "level1": {
        "description": "First level of nesting",
        "value": 100,
        "level2": {
          "description": "Second level of nesting",
          "flags": [true, false, true],
          "level3": {
            "description": "Third level of nesting",
            "coordinates": {
              "x": 10.5,
              "y": 20.75,
              "z": -5.0
            },
            "level4": {
              "description": "Fourth level of nesting",
              "metadata": {
                "created": "2025-02-15T14:30:00Z",
                "modified": null,
                "version": 2.1
              },
              "level5": {
                "description": "Fifth level of nesting",
                "settings": {
                  "enabled": true,
                  "threshold": 0.85,
                  "options": ["fast", "accurate", "balanced"],
                  "config": {
                    "timeout": 30000,
                    "retries": 3,
                    "deepSetting": {
                      "algorithm": "advanced",
                      "parameters": [1, 1, 2, 3, 5, 8, 13]
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
}
""");

// JsonNode
var origin = json!["nesting"]!["level1"]!["level2"]!;

// JsonNode axis, Children, Descendants, Anestors, BeforeSelf, AfterSelf and ***Self.
foreach (var item in origin.Descendants().Select(x => x.Node).OfType(default(JsonArray)))
{
    // [truem false, true], ["fast", "accurate", "balanced"], [1, 1, 2, 3, 5, 8, 13]
    Console.WriteLine(item!.ToJsonString(JsonSerializerOptions.Web));
}
```

### GameObject/Transfrom(Unity)

see: [unity](#unity) section.

LINQ to SIMD
---
WIP

Unity
---
The minimum supported Unity version will be `2022.3.12f1`, as it is necessary to support C# Incremental Source Generator(Compiler Version, 4.3.0).

There are two installation steps required to use it in Unity.

1. Install `ZLinq` from NuGet using [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)
Open Window from NuGet -> Manage NuGet Packages, Search "ZLinq" and Press Install. 

2. Install the `ZLinq.Unity` package by referencing the git URL  
> https://github.com/Cysharp/ZLinq.git?path=src/ZLinq.Unity/Assets/ZLinq.Unity

Unityパッケージの補助により、通常のZLinqの他に、GameObject/Transformに対して探索が行えるLINQ to GameObject機能が有効になります。

![](Images/axis.jpg)

```csharp
using ZLinq;

public class SampleScript : MonoBehaviour
{
    public Transform Origin;

    void Start()
    {
        Debug.Log("Ancestors--------------");  // Container, Root
        foreach (var item in Origin.Ancestors()) Debug.Log(item.name);

        Debug.Log("Children--------------"); // Sphere_A, Sphere_B, Group, Sphere_A, Sphere_B
        foreach (var item in Origin.Children()) Debug.Log(item.name);

        Debug.Log("Descendants--------------"); // Sphere_A, Sphere_B, Group, P1, Group, Sphere_B, P2, Sphere_A, Sphere_B
        foreach (var item in Origin.Descendants()) Debug.Log(item.name);

        Debug.Log("BeforeSelf--------------"); // C1, C2
        foreach (var item in Origin.BeforeSelf()) Debug.Log(item.name);

        Debug.Log("AfterSelf--------------");  // C3, C4
        foreach (var item in Origin.AfterSelf()) Debug.Log(item.name);
    }
}
```

You can chain query(LINQ to Objects). また、`OfComponent<T>`ヘルパーによりコンポーネントでフィルタリングできます。

```csharp
// all filtered(tag == "foobar") objects
var foobars = root.Descendants().Where(x => x.tag == "foobar");

// get FooScript under self childer objects and self
var fooScripts = root.ChildrenAndSelf().OfComponent<FooScript>(); 
```

License
---
This library is under MIT License.
