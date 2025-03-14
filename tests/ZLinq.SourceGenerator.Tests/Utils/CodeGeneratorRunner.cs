using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using ZLinq.SourceGenerator;

public static class CSharpGeneratorRunner
{
    static Compilation baseCompilation = default!;

    [ModuleInitializer]
    public static void InitializeCompilation()
    {
        var globalUsings = """
// global using System;
// global using System.Threading;
// global using System.Threading.Tasks;
// global using System.ComponentModel.DataAnnotations;
global using ZLinq;
""";

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Concat([
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),                                                 // System.Console.dll
                // MetadataReference.CreateFromFile(typeof(Func<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(List<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IServiceProvider).Assembly.Location),                                        // System.ComponentModel.dll
                MetadataReference.CreateFromFile(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute).Assembly.Location), // System.ComponentModel.DataAnnotations
                MetadataReference.CreateFromFile(typeof(System.Text.Json.JsonDocument).Assembly.Location),                           // System.Text.Json.dll
                MetadataReference.CreateFromFile(typeof(ZLinq.IValueEnumerable<>).Assembly.Location),                                // ZLinq.dll
            ]);

        var compilation = CSharpCompilation.Create("generatortest",
            references: references,
            syntaxTrees: [CSharpSyntaxTree.ParseText(globalUsings, path: "GlobalUsings.cs")],
            options: new CSharpCompilationOptions(OutputKind.ConsoleApplication, allowUnsafe: true)); // .exe

        baseCompilation = compilation;
    }

    public static (Compilation, ImmutableArray<Diagnostic>) RunGenerator([StringSyntax("C#-test")] string source, string[]? preprocessorSymbols = null, AnalyzerConfigOptionsProvider? options = null)
    {
        if (preprocessorSymbols == null)
        {
            preprocessorSymbols = new[] { "NET9_0_OR_GREATER" };
        }
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp13, preprocessorSymbols: preprocessorSymbols); // 13

        var driver = CSharpGeneratorDriver.Create(new ZLinqSourceGenerator()).WithUpdatedParseOptions(parseOptions);
        if (options != null)
        {
            driver = (Microsoft.CodeAnalysis.CSharp.CSharpGeneratorDriver)driver.WithUpdatedAnalyzerConfigOptions(options);
        }

        var compilation = baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, parseOptions));

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);
        return (newCompilation, diagnostics);
    }

    public static (Compilation, ImmutableArray<Diagnostic>, string) CompileAndExecute(string source, string[] args, string[]? preprocessorSymbols = null, AnalyzerConfigOptionsProvider? options = null)
    {
        var (compilation, diagnostics) = RunGenerator(source, preprocessorSymbols, options);

        using var ms = new MemoryStream();
        var emitResult = compilation.Emit(ms);
        if (!emitResult.Success)
        {
            throw new InvalidOperationException("Emit Failed\r\n" + string.Join("\r\n", emitResult.Diagnostics.Select(x => x.ToString())));
        }

        ms.Position = 0;

        // capture stdout log
        // modify global stdout so can't run in parallel unit-test
        var originalOut = Console.Out;
        try
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // load and invoke Main(args)
            var loadContext = new AssemblyLoadContext("source-generator", isCollectible: true); // isCollectible to support Unload
            var assembly = loadContext.LoadFromStream(ms);
            assembly.EntryPoint!.Invoke(null, new object[] { args });
            loadContext.Unload();

            return (compilation, diagnostics, stringWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    public static (string Key, string Reasons)[][] GetIncrementalGeneratorTrackedStepsReasons(string keyPrefixFilter, params string[] sources)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp13); // 13
        var driver = CSharpGeneratorDriver.Create(
            [new ZLinqSourceGenerator().AsSourceGenerator()],
            driverOptions: new GeneratorDriverOptions(IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true))
            .WithUpdatedParseOptions(parseOptions);

        var generatorResults = sources
            .Select(source =>
            {
                var compilation = baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, parseOptions));
                driver = driver.RunGenerators(compilation);
                return driver.GetRunResult().Results[0];
            })
            .ToArray();

        var reasons = generatorResults
            .Select(x => x.TrackedSteps
                .Where(x => x.Key.StartsWith(keyPrefixFilter) || x.Key == "SourceOutput")
                .Select(x =>
                {
                    if (x.Key == "SourceOutput")
                    {
                        var values = x.Value.Where(x => x.Inputs[0].Source.Name?.StartsWith(keyPrefixFilter) ?? false);
                        return (
                            x.Key,
                            Reasons: string.Join(", ", values.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray())
                        );
                    }
                    else
                    {
                        return (
                            Key: x.Key.Substring(keyPrefixFilter.Length),
                            Reasons: string.Join(", ", x.Value.SelectMany(x => x.Outputs).Select(x => x.Reason).ToArray())
                        );
                    }
                })
                .OrderBy(x => x.Key)
                .ToArray())
            .ToArray();

        return reasons;
    }
}

public class VerifyHelper(ITestOutputHelper output, string idPrefix)
{
    // Diagnostics Verify

    public void Ok([StringSyntax("C#-test")] string code, [CallerArgumentExpression("code")] string? codeExpr = null)
    {
        output.WriteLine(codeExpr!);

        var (compilation, diagnostics) = CSharpGeneratorRunner.RunGenerator(code);
        foreach (var item in diagnostics)
        {
            output.WriteLine(item.ToString());
        }
        foreach (var item in compilation.GetDiagnostics())
        {
            output.WriteLine(item.ToString());
        }

        OutputGeneratedCode(compilation);

        diagnostics.Length.ShouldBe(0);
        compilation.GetDiagnostics().Length.ShouldBe(0);
    }

    public void Verify(int id, [StringSyntax("C#-test")] string code, string diagnosticsCodeSpan, [CallerArgumentExpression("code")] string? codeExpr = null)
    {
        output.WriteLine(codeExpr!);

        var (compilation, diagnostics) = CSharpGeneratorRunner.RunGenerator(code);
        foreach (var item in diagnostics)
        {
            output.WriteLine(item.ToString());
        }
        OutputGeneratedCode(compilation);

        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(idPrefix + id.ToString("000"));

        var text = GetLocationText(diagnostics[0], compilation.SyntaxTrees);
        text.ShouldBe(diagnosticsCodeSpan);
    }

    public (string, string)[] Verify([StringSyntax("C#-test")] string code, [CallerArgumentExpression("code")] string? codeExpr = null)
    {
        output.WriteLine(codeExpr!);

        var (compilation, diagnostics) = CSharpGeneratorRunner.RunGenerator(code);
        OutputGeneratedCode(compilation);
        return diagnostics.Select(x => (x.Id, GetLocationText(x, compilation.SyntaxTrees))).ToArray();
    }

    // Execute and check stdout result

    public void Execute([StringSyntax("C#-test")] string code, string args, string expected, [CallerArgumentExpression("code")] string? codeExpr = null)
    {
        output.WriteLine(codeExpr!);

        var (compilation, diagnostics, stdout) = CSharpGeneratorRunner.CompileAndExecute(code, args == "" ? [] : args.Split(' '));
        foreach (var item in diagnostics)
        {
            output.WriteLine(item.ToString());
        }
        OutputGeneratedCode(compilation);

        stdout.ShouldBe(expected, StringCompareShould.IgnoreLineEndings);
    }

    public string Error([StringSyntax("C#-test")] string code, string args, [CallerArgumentExpression("code")] string? codeExpr = null)
    {
        output.WriteLine(codeExpr!);

        var (compilation, diagnostics, stdout) = CSharpGeneratorRunner.CompileAndExecute(code, args == "" ? [] : args.Split(' '));
        foreach (var item in diagnostics)
        {
            output.WriteLine(item.ToString());
        }
        OutputGeneratedCode(compilation);

        return stdout;
    }

    string GetLocationText(Diagnostic diagnostic, IEnumerable<SyntaxTree> syntaxTrees)
    {
        var location = diagnostic.Location;

        var textSpan = location.SourceSpan;
        var sourceTree = location.SourceTree;
        if (sourceTree == null)
        {
            var lineSpan = location.GetLineSpan();
            if (lineSpan.Path == null) return "";

            sourceTree = syntaxTrees.FirstOrDefault(x => x.FilePath == lineSpan.Path);
            if (sourceTree == null) return "";
        }

        var text = sourceTree.GetText().GetSubText(textSpan).ToString();
        return text;
    }

    void OutputGeneratedCode(Compilation compilation)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            // only shows ConsoleApp.Run/Builder generated code
            if (!syntaxTree.FilePath.Contains("g.cs")) continue;
            output.WriteLine(syntaxTree.ToString());
        }
    }
}
