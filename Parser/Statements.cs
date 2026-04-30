using MiniLang.Lexer;

namespace MiniLang.Parser;

public abstract class Stmt
{
    public abstract void Accept(IStmtVisitor visitor);
}

public interface IStmtVisitor
{
    void VisitExpressionStmt(ExpressionStmt stmt);
    void VisitPrintStmt(PrintStmt stmt);
    void VisitLetStmt(LetStmt stmt);
    void VisitBlockStmt(BlockStmt stmt);
    void VisitIfStmt(IfStmt stmt);
    void VisitWhileStmt(WhileStmt stmt);
}

public sealed class ExpressionStmt : Stmt
{
    public Expr Expression { get; }

    public ExpressionStmt(Expr expression)
    {
        Expression = expression;
    }

    public override void Accept(IStmtVisitor visitor) => visitor.VisitExpressionStmt(this);
}

public sealed class PrintStmt : Stmt
{
    public Expr Expression { get; }

    public PrintStmt(Expr expression)
    {
        Expression = expression;
    }

    public override void Accept(IStmtVisitor visitor) => visitor.VisitPrintStmt(this);
}

public sealed class LetStmt : Stmt
{
    public Token Name { get; }
    public Expr Initializer { get; }

    public LetStmt(Token name, Expr initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public override void Accept(IStmtVisitor visitor) => visitor.VisitLetStmt(this);
}

public sealed class BlockStmt : Stmt
{
    public List<Stmt> Statements { get; }

    public BlockStmt(List<Stmt> statements)
    {
        Statements = statements;
    }

    public override void Accept(IStmtVisitor visitor) => visitor.VisitBlockStmt(this);
}

public sealed class IfStmt : Stmt
{
    public Expr Condition { get; }
    public Stmt ThenBranch { get; }
    public Stmt? ElseBranch { get; }

    public IfStmt(Expr condition, Stmt thenBranch, Stmt? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public override void Accept(IStmtVisitor visitor) => visitor.VisitIfStmt(this);
}

public sealed class WhileStmt : Stmt
{
    public Expr Condition { get; }
    public Stmt Body { get; }

    public WhileStmt(Expr condition, Stmt body)
    {
        Condition = condition;
        Body = body;
    }

    public override void Accept(IStmtVisitor visitor) => visitor.VisitWhileStmt(this);
}
