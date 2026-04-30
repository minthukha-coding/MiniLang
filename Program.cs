using MiniLang.Lexer;
using MiniLang.Parser;
using MiniLang.Runtime;

string source;

if (args.Length > 0)
{
    source = File.ReadAllText(args[0]);
}
else
{
    source = """
    // MiniLang sample program
    let name = "MiniLang";
    let x = 5;

    print("Hello " + name);
    print(x * 10);

    if (x >= 5) {
        print("x is big");
    } else {
        print("x is small");
    }

    while (x > 0) {
        print(x);
        x = x - 1;
    }
    """;
}

try
{
    var lexer = new Lexer(source);
    var tokens = lexer.ScanTokens();

    var parser = new Parser(tokens);
    var statements = parser.Parse();

    var interpreter = new Interpreter();
    interpreter.Interpret(statements);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
