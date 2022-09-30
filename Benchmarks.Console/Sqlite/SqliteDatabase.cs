using Dapper;
using Microsoft.Data.Sqlite;

namespace Benchmarks.Console.Sqlite;

public static class SqliteDatabase
{
    private static readonly Random Rng = new(1234);

    public static void Make(string databaseName)
    {
        using var connection = new SqliteConnection($"Data Source = {databaseName}");
        
        connection.Open();

        connection.Execute(@"CREATE TABLE Things(
            id TEXT NOT NULL PRIMARY KEY,
            value_a TEXT NOT NULL,
            value_b INTEGER NOT NULL
        );");

        using var tx = connection.BeginTransaction();

        var vals = new List<Thing>(25_000);

        foreach (var iter in Enumerable.Range(0, 10_000))
        {
            vals.Add(new Thing
            {
                Id = Guid.NewGuid()
                    .ToString("N"),
                Value_A = Guid.NewGuid()
                    .ToString("N"),
                Value_B = Rng.Next(0, 256)
            });
        }

        connection.Execute(@"
            INSERT INTO Things(id, value_a, value_b) 
            VALUES (@Id, @Value_A, @Value_B)
        ", param: vals, transaction: tx);

        tx.Commit();
    }

    public static void Destroy(string databaseName)
    {
        try
        {
            File.Delete(databaseName);
        }
        catch (IOException)
        {
            //Whatever
        }
    }
}
