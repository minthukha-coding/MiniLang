using System;
using System.Collections.Generic;
using System.Data;

var variables = new Dictionary<string, object>();

Console.WriteLine("MiniLang Started");
Console.WriteLine("Commands: let x = 10, print x, exit");

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(line))
        continue;

    if (line == "exit")
        break;

    try
    {
        Execute(line);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}

void Execute(string line)
{
    // let x = 10 + 5
    if (line.StartsWith("let "))
    {
        var code = line.Substring(4);
        var parts = code.Split('=', 2);

        if (parts.Length != 2)
            throw new Exception("Invalid let syntax");

        var name = parts[0].Trim();
        var expression = parts[1].Trim();

        var value = Eval(expression);
        variables[name] = value;

        return;
    }

    // print x
    if (line.StartsWith("print "))
    {
        var expression = line.Substring(6).Trim();
        Console.WriteLine(Eval(expression));
        return;
    }

    throw new Exception("Unknown command");
}

object Eval(string expression)
{
    foreach (var variable in variables)
    {
        expression = expression.Replace(variable.Key, variable.Value.ToString());
    }

    var table = new DataTable();
    return table.Compute(expression, "");
}