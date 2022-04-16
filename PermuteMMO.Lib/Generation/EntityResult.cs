using PKHeX.Core;

namespace PermuteMMO.Lib;

/// <summary>
/// Spawned Pokémon Data that can be encountered.
/// </summary>
public sealed class EntityResult
{
    public string Name { get; init; } = string.Empty;
    public readonly byte[] IVs = { byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue };

    public ulong Seed { get; init; }
    public int Level { get; init; }

    public uint EC { get; set; }
    public uint FakeTID { get; set; }
    public uint PID { get; set; }

    public uint ShinyXor { get; set; }
    public int RollCountUsed { get; set; }
    public int RollCountAllowed { get; set; }
    public ushort Species { get; init; }
    public ushort Form { get; init; }

    public bool IsShiny { get; set; }
    public bool IsAlpha { get; init; }
    public byte Ability { get; set; }
    public byte Gender { get; set; }
    public byte Nature { get; set; }
    public byte Height { get; set; }
    public byte Weight { get; set; }

    public bool IsOblivious => BehaviorUtil.Oblivious.Contains(Species);
    public bool IsSkittish => BehaviorUtil.Skittish.Contains(Species);
    public bool IsAggressive => IsAlpha || !(IsSkittish || IsOblivious);

    public string GetSummary()
    {
        var pid = string.Format("{0:X}", PID);
        var ec = string.Format("{0:X}", EC);
        var shiny = IsShiny ? $"Shiny: {(ShinyXor == 0 ? "square" : "star")}\n" : "False\n";
        var level = Level.ToString();
        var ivs = $" {IVs[0]:00}/{IVs[1]:00}/{IVs[2]:00}/{IVs[3]:00}/{IVs[4]:00}/{IVs[5]:00}\n";
        var nature = $"Nature: {GameInfo.GetStrings(1).Natures[Nature]}";
        var alpha = IsAlpha ? "Alpha-" : "";
        var notAlpha = !IsAlpha ? " -- NOT ALPHA" : "";
        var gender = Gender switch
        {
            2 => "",
            1 => " (F)",
            _ => " (M)",
        };
        return $"{alpha}{Name}{gender}\nPID:{pid}\nEC:{ec}\n{shiny}Level: {level}\nIVs:{ivs}{nature,-8}\n";
    }
}

