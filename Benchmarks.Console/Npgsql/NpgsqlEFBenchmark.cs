using BenchmarkDotNet.Attributes;
using Benchmarks.Console.Npgsql;
using Microsoft.EntityFrameworkCore;

namespace Benchmarks.Console.Npgsql;

[MemoryDiagnoser]
public class NpgsqlEFBenchmark
{
    private string _databaseName;

    private DbContextOptions<SampleDbContext>? _dbContextOptions;

    [IterationSetup]
    public void CreateDatabase()
    {
        _databaseName = $"benchmark_{Guid.NewGuid():N}";
        
        var connectionString = NpgsqlDatabase.CreateConnectionString(_databaseName);

        //TODO: Improve connection string
        _dbContextOptions = new DbContextOptionsBuilder<SampleDbContext>().UseNpgsql(connectionString)
            .Options;

        NpgsqlDatabase.Make(_databaseName);
    }

    [IterationCleanup]
    public void DestroyDatabase()
    {
        NpgsqlDatabase.Destroy(_databaseName);
    }

    [Benchmark]
    public void TrackingQueryWithInsert()
    {
        using var dbContext = new SampleDbContext(_dbContextOptions);

        var things = dbContext.things.ToList();

        foreach (var thing in things)
        {
            thing.Value_A = "changed!";
            thing.Value_B = 2;
        }

        dbContext.SaveChanges();
    }

    [Benchmark]
    public void NoTrackingQueryWithInsert()
    {
        using var dbContext = new SampleDbContext(_dbContextOptions);
    
        var things = dbContext.things.AsNoTracking()
            .ToList();
    
        foreach (var thing in things)
        {
            var newThing = new Thing
            {
                Id = $"new_{thing.Id}",
                Value_A = "new!",
                Value_B = 3
            };
    
            dbContext.things.Add(newThing);
        }
    
        dbContext.SaveChanges();
    }
}
