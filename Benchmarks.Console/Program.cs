using BenchmarkDotNet.Running;
using Benchmarks.Console.Npgsql;
using Benchmarks.Console.Sqlite;

BenchmarkRunner.Run<SqliteEFBenchmark>();
BenchmarkRunner.Run<NpgsqlEFBenchmark>();
