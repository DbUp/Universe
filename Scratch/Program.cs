using System;
using System.Data;
using System.Linq;
using DbUp;
using DbUp.Engine;
using DbUp.Helpers;

namespace Scratch;

/// <summary>
/// Simple example using idiomatic DbUp syntax with IScript that performs a query.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        
        Console.WriteLine("DbUp Sample - IScript with Query");
        Console.WriteLine();

        // Create a script that queries first, then returns a script

        EnsureDatabase.For.SqlDatabase("Server=localhost;Database=DbUpScratch;Integrated Security=true;TrustServerCertificate=true");
        // Use idiomatic DbUp syntax to execute the script
        var upgrader = DeployChanges.To
            .SqlDatabase("Server=localhost;Database=DbUpScratch;Integrated Security=true;TrustServerCertificate=true")
            .WithScriptsAndCodeEmbeddedInAssembly(typeof(Program).Assembly)
            .JournalTo(new NullJournal())
            .LogToConsole()
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Upgrade failed: {result.Error}");
            Console.ResetColor();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Upgrade successful!");
        Console.ResetColor();
        var scriptCount = result.Scripts.Count();
        Console.WriteLine($"Scripts executed: {scriptCount}");
    }
}

/// <summary>
/// IScript implementation that performs a query and returns a script based on the query result.
/// </summary>
public class QueryBasedScript : IScript
{
    readonly string _name = $"Embedded {Guid.NewGuid()}";


    /// <inheritdoc/>
    public string ProvideScript(Func<IDbCommand> dbCommandFactory)
    {
        // First, perform a simple query to get some data
        int currentCount = 0;
        using (var command = dbCommandFactory())
        {
            command.CommandText = @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'TestTable'";
            
            var result = command.ExecuteScalar();
            Console.WriteLine("Result: " + result);
        }

            // Table exists, return a simple query script
            return @"
                SELECT COUNT(*) 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = 'TestTable'";
    }

    public string Name => _name;
}
