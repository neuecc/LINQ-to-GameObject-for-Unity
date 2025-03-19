using Microsoft.CodeAnalysis;

namespace ZLinq;

[Generator(LanguageNames.CSharp)]
public class DropInGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // TODO: ZLinqDropInAttribute not found warning

        var generatorOptions = context.CompilationProvider.Select((compilation, token) =>
        {
            foreach (var attr in compilation.Assembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name == "ZLinqDropInAttribute")
                {
                    var args = attr.NamedArguments;
                    var disableNamingConversion = args.FirstOrDefault(x => x.Key == "DisableNamingConversion").Value.Value as bool? ?? false;
                    //return new ConsoleAppFrameworkGeneratorOptions(disableNamingConversion);
                    return "";
                }
            }

            return "";
            //return new ConsoleAppFrameworkGeneratorOptions(DisableNamingConversion: false);
        });

        context.RegisterSourceOutput(generatorOptions, (_, _) => { });


        context.RegisterPostInitializationOutput(EmitPostInitializationCode);
    }

    public void EmitPostInitializationCode(IncrementalGeneratorPostInitializationContext context)
    {
        var thisAsm = typeof(DropInGenerator).Assembly;
        var resourceNames = thisAsm.GetManifestResourceNames();
        foreach (var resourceName in resourceNames)
        {

            using var stream = thisAsm.GetManifestResourceStream(resourceName);

            using var sr = new StreamReader(stream!);
            var code = sr.ReadToEnd();

            //context.AddSource(hintName, code);
        }
    }
}
