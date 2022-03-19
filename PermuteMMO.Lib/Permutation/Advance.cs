﻿namespace PermuteMMO.Lib;

/// <summary>
/// Advancement step labels.
/// </summary>
public enum Advance : byte
{
    SB,

    A1,
    A2,
    A3,
    A4,

    G1,
    G2,
    G3,
}

public static class AdvanceExtensions
{
    /// <summary>
    /// Option to just emit the <see cref="Advance.ToString()"/> result instead of a humanized string.
    /// </summary>
    public static bool Raw { get; set; } = true;

    /// <summary>
    /// Returns a string for indicating the value of the <see cref="advance"/> step.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">If undefined</exception>
    public static string GetName(this Advance advance) => Raw ? advance.ToString() : advance switch
    {
        Advance.SB => "Start Bonus",
        Advance.A1 => "1",
        Advance.A2 => "2",
        Advance.A3 => "3",
        Advance.A4 => "4",
        Advance.G1 => "~1[3]",
        Advance.G2 => "~2[2]",
        Advance.G3 => "~3[1]",
        _ => throw new ArgumentOutOfRangeException(nameof(advance), advance, null)
    };

    /// <summary>
    /// Gets the count of advances required.
    /// </summary>
    public static int AdvanceCount(this Advance advance) => advance switch
    {
        Advance.A1 => 1,
        Advance.A2 => 2,
        Advance.A3 => 3,
        Advance.A4 => 4,

        Advance.G1 => 1,
        Advance.G2 => 2,
        Advance.G3 => 3,
        _ => 0,
    };

    /// <summary>
    /// Indicates if a multi-battle is required for this advancement.
    /// </summary>
    public static bool IsMulti(this Advance advance) => advance.AdvanceCount() > 1;

    /// <summary>
    /// Indicates if any advance requires a multi-battle for advancement.
    /// </summary>
    public static bool IsAnyMulti(this IEnumerable<Advance> advances) => advances.Any(z => z.IsMulti());
}
