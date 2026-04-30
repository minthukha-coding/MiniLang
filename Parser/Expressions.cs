using MiniLang.Lexer;

namespace MiniLang.Parser;

public abstract class Expr
{
    public abstract T Accept<T>(IExprVisitor<T> visitor);
}

public interface IExprVisitor<T>
{
    T VisitBinaryExpr(Binary expr);
    T VisitGroupingExpr(Grouping expr);
    T VisitLiteralExpr(Literal expr);
    T VisitUnaryExpr(Unary expr);
    T VisitVariableExpr(Variable expr);
    T VisitAssignExpr(Assign expr);
}

public sealed class Binary : Expr
{
    public Expr Left { get; }
    public Token Operator { get; }
    public Expr Right { get; }

    public Binary(Expr left, Token op, Expr right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitBinaryExpr(this);
}

public sealed class Grouping : Expr
{
    public Expr Expression { get; }

    public Grouping(Expr expression)
    {
        Expression = expression;
    }

    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitGroupingExpr(this);
}

public sealed class Literal : Expr
{
    public object? Value { get; }

    public Literal(object? value)
    {
        Value = value;
    }

    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitLiteralExpr(this);
}

public sealed class Unary : Expr
{
    public Token Operator { get; }
    public Expr Right { get; }

    public Unary(Token op, Expr right)
    {
        Operator = op;
        Right = right;
    }

    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitUnaryExpr(this);
}

public sealed class Variable : Expr
{
    public Token Name { get; }

    public Variable(Token name)
    {
        Name = name;
    }

    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitVariableExpr(this);
}

public sealed class Assign : Expr
{
    public Token Name { get; }
    public Expr Value { get; }

    public Assign(Token name, Expr value)
    {
        Name = name;
        Value = value;
    }

    public override T Accept<T>(IExprVisitor<T> visitor) => visitor.VisitAssignExpr(this);
}
