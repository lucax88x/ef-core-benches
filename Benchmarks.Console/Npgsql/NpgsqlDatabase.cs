using Dapper;
using Npgsql;

namespace Benchmarks.Console.Npgsql;

public static class NpgsqlDatabase
{
    private static readonly Random Rng = new(1234);

    public static void Make(string databaseName)
    {
        try
        {
            using var creationConnnection = CreateConnection();

            using var createCommand = new NpgsqlCommand($"CREATE DATABASE {databaseName}", creationConnnection);
            createCommand.ExecuteNonQuery();

            using var connection = CreateConnection(databaseName);

            connection.Execute(@"CREATE TABLE public.Things(
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
                INSERT INTO public.Things(Id, Value_a, Value_b) 
                VALUES (@Id, @Value_A, @Value_B)
            ", param: vals, transaction: tx);

            tx.Commit();
        }
        catch (Exception e)
        {
            System.Console.WriteLine("Db already there, skipping");
            System.Console.WriteLine(e.Message);
        }
    }

    public static void Destroy(string databaseName)
    {
        using var dropConnection = CreateConnection();

        using var dropCommand = new NpgsqlCommand(
            $"DROP DATABASE IF EXISTS {databaseName} WITH (FORCE)",
            dropConnection
        );

        dropCommand.ExecuteNonQuery();
    }

    public static string CreateConnectionString(string database = "postgres")
    {
        return $"Server=localhost;Port=5432;Database={database};User Id=demo;Password=demo;Timeout=15;";
    }

    private static NpgsqlConnection CreateConnection(string database = "postgres")
    {
        var connection =
            new NpgsqlConnection(CreateConnectionString(database));

        connection.Open();

        return connection;
    }
}