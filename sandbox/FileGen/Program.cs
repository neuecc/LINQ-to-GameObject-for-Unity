using ConsoleAppFramework;
using FileGen;

var app = ConsoleApp.Create();
app.Add<Commands>();
app.Add<DropinGen>();

// linq-template
// type-of-contains
app.Run(args);

