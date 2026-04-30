using MiniLang.Lexer;

namespace MiniLang.Parser;

public sealed class Parser
{
    private readonly List<Token> _tokens;
    private int _current;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();

        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt Declaration()
    {
        if (Match(TokenType.Let)) return LetDeclaration();
        return Statement();
    }

    private Stmt LetDeclaration()
    {
        Token name = Consume(TokenType.Identifier, "Expected variable name.");
        Consume(TokenType.Equal, "Expected '=' after variable name.");
        Expr initializer = Expression();
        Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
        return new LetStmt(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(TokenType.Print)) return PrintStatement();
        if (Match(TokenType.If)) return IfStatement();
        if (Match(TokenType.While)) return WhileStatement();
        if (Match(TokenType.LeftBrace)) return new BlockStmt(Block());

        return ExpressionStatement();
    }

    private Stmt PrintStatement()
    {
        Consume(TokenType.LeftParen, "Expected '(' after print.");
        Expr value = Expression();
        Consume(TokenType.RightParen, "Expected ')' after value.");
        Consume(TokenType.Semicolon, "Expected ';' after print statement.");
        return new PrintStmt(value);
    }

    private Stmt IfStatement()
    {
        Consume(TokenType.LeftParen, "Expected '(' after if.");
        Expr condition = Expression();
        Consume(TokenType.RightParen, "Expected ')' after if condition.");

        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;

        if (Match(TokenType.Else))
        {
            elseBranch = Statement();
        }

        return new IfStmt(condition, thenBranch, elseBranch);
    }

    private Stmt WhileStatement()
    {
        Consume(TokenType.LeftParen, "Expected '(' after while.");
        Expr condition = Expression();
        Consume(TokenType.RightParen, "Expected ')' after condition.");

        Stmt body = Statement();
        return new WhileStmt(condition, body);
    }

    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(TokenType.RightBrace, "Expected '}' after block.");
        return statements;
    }

    private Stmt ExpressionStatement()
    {
        Expr expr = Expression();
        Consume(TokenType.Semicolon, "Expected ';' after expression.");
        return new ExpressionStmt(expr);
    }

    private Expr Expression()
    {
        return Assignment();
    }

    private Expr Assignment()
    {
        Expr expr = Equality();

        if (Match(TokenType.Equal))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Variable variable)
            {
                return new Assign(variable.Name, value);
            }

            throw Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            Token op = Previous();
            Expr right = Comparison();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        Expr expr = Term();

        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(TokenType.Minus, TokenType.Plus))
        {
            Token op = Previous();
            Expr right = Factor();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(TokenType.Slash, TokenType.Star))
        {
            Token op = Previous();
            Expr right = Unary();
            expr = new Binary(expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            Token op = Previous();
            Expr right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.False)) return new Literal(false);
        if (Match(TokenType.True)) return new Literal(true);

        if (Match(TokenType.Number, TokenType.String))
        {
            return new Literal(Previous().Literal);
        }

        if (Match(TokenType.Identifier))
        {
            return new Variable(Previous());
        }

        if (Match(TokenType.LeftParen))
        {
            Expr expr = Expression();
            Consume(TokenType.RightParen, "Expected ')' after expression.");
            return new Grouping(expr);
        }

        throw Error(Peek(), "Expected expression.");
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == TokenType.EOF;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    private static Exception Error(Token token, string message)
    {
        return new Exception($"[line {token.Line}] Error at '{token.Lexeme}': {message}");
    }
}
