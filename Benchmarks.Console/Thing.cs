using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Benchmarks.Console;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class Thing
{
    [Column("id")]
    public string Id { get; set; } = null!;

    [Column("value_a")]
    public string Value_A { get; set; } = null!;

    [Column("value_b")]
    public int Value_B { get; set; }
}