using MiniLang.Lexer;
using MiniLang.Parser;

namespace MiniLang.Runtime;

public sealed class Interpreter : IExprVisitor<object?>, IStmtVisitor
{
    private Environment _environment = new();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Runtime Error: {ex.Message}");
        }
    }

    public void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = _environment;

        try
        {
            _environment = environment;

            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }

    public void VisitExpressionStmt(ExpressionStmt stmt)
    {
        Evaluate(stmt.Expression);
    }

    public void VisitPrintStmt(PrintStmt stmt)
    {
        object? value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));
    }

    public void VisitLetStmt(LetStmt stmt)
    {
        object? value = Evaluate(stmt.Initializer);
        _environment.Define(stmt.Name.Lexeme, value);
    }

    public void VisitBlockStmt(BlockStmt stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
    }

    public void VisitIfStmt(IfStmt stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch is not null)
        {
            Execute(stmt.ElseBranch);
        }
    }

    public void VisitWhileStmt(WhileStmt stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }
    }

    public object? VisitLiteralExpr(Literal expr)
    {
        return expr.Value;
    }

    public object? VisitGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitUnaryExpr(Unary expr)
    {
        object? right = Evaluate(expr.Right);

        return expr.Operator.Type switch
        {
            TokenType.Minus => -ToNumber(right, expr.Operator),
            TokenType.Bang => !IsTruthy(right),
            _ => null
        };
    }

    public object? VisitVariableExpr(Variable expr)
    {
        return _environment.Get(expr.Name);
    }

    public object? VisitAssignExpr(Assign expr)
    {
        object? value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
    }

    public object? VisitBinaryExpr(Binary expr)
    {
        object? left = Evaluate(expr.Left);
        object? right = Evaluate(expr.Right);

        return expr.Operator.Type switch
        {
            TokenType.Plus => Plus(left, right),
            TokenType.Minus => ToNumber(left, expr.Operator) - ToNumber(right, expr.Operator),
            TokenType.Star => ToNumber(left, expr.Operator) * ToNumber(right, expr.Operator),
            TokenType.Slash => ToNumber(left, expr.Operator) / ToNumber(right, expr.Operator),
            TokenType.Greater => ToNumber(left, expr.Operator) > ToNumber(right, expr.Operator),
            TokenType.GreaterEqual => ToNumber(left, expr.Operator) >= ToNumber(right, expr.Operator),
            TokenType.Less => ToNumber(left, expr.Operator) < ToNumber(right, expr.Operator),
            TokenType.LessEqual => ToNumber(left, expr.Operator) <= ToNumber(right, expr.Operator),
            TokenType.EqualEqual => IsEqual(left, right),
            TokenType.BangEqual => !IsEqual(left, right),
            _ => null
        };
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private static object Plus(object? left, object? right)
    {
        if (left is double leftNumber && right is double rightNumber)
            return leftNumber + rightNumber;

        if (left is string || right is string)
            return Stringify(left) + Stringify(right);

        throw new Exception("Operands must be two numbers or at least one string.");
    }

    private static double ToNumber(object? value, Token token)
    {
        if (value is double number)
            return number;

        throw new Exception($"Operand must be a number near '{token.Lexeme}'.");
    }

    private static bool IsTruthy(object? value)
    {
        if (value is null) return false;
        if (value is bool boolean) return boolean;
        return true;
    }

    private static bool IsEqual(object? left, object? right)
    {
        if (left is null && right is null) return true;
        if (left is null) return false;
        return left.Equals(right);
    }

    private static string Stringify(object? value)
    {
        if (value is null) return "nil";

        if (value is double number)
        {
            string text = number.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (text.EndsWith(".0")) text = text[..^2];
            return text;
        }

        return value.ToString() ?? "nil";
    }
}
