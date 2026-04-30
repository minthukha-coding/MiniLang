using MiniLang.Lexer;

namespace MiniLang.Runtime;

public sealed class Environment
{
    private readonly Dictionary<string, object?> _values = new();
    private readonly Environment? _parent;

    public Environment()
    {
    }

    public Environment(Environment parent)
    {
        _parent = parent;
    }

    public void Define(string name, object? value)
    {
        _values[name] = value;
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out object? value))
            return value;

        if (_parent is not null)
            return _parent.Get(name);

        throw new Exception($"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }

        if (_parent is not null)
        {
            _parent.Assign(name, value);
            return;
        }

        throw new Exception($"Undefined variable '{name.Lexeme}'.");
    }
}
