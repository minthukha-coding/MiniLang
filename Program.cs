using MiniLang.Lexer;
using MiniLang.Parser;
using MiniLang.Runtime;

var source = File.ReadAllText("test.mini");

var lexer = new Lexer(source);
var tokens = lexer.ScanTokens();

var parser = new Parser(tokens);
var statements = parser.Parse();

var interpreter = new Interpreter();
interpreter.Interpret(statements);