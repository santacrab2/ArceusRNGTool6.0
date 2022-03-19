using PKHeX.Core;
namespace PermuteMMO.Lib;

/// <summary>
/// Spawned Pokémon Data that can be encountered.
/// </summary>
public sealed class EntityResult
{
    public ulong Seed { get; set; }
    public string Name { get; set; } = string.Empty;
    public uint EC { get; set; }
    public uint FakeTID { get; set; }

    public uint PID { get; set; }
    public int RollCount { get; set; }
    public uint ShinyXor { get; set; }
    public int Level { get; set; }
    public int PermittedRolls { get; set; }
    public int Ability { get; set; }
    public int Gender { get; set; }
    public int Nature { get; set; }

    public bool IsShiny { get; set; }
    public bool IsAlpha { get; set; }
    public byte Height { get; set; }
    public byte Weight { get; set; }
    public int[] ivs { get; set; }

    public string GetSummary()
    {
        return $"{(IsAlpha? "Alpha-" : "")}{Name}\nShiny: {IsShiny}\nAlpha: {IsAlpha}\nPID: {PID}\nEC: {EC}\nGender: {Gender}\nLevel: {Level}\nIVs: {ivs[0]}\\{ivs[1]}\\{ivs[2]}\\{ivs[3]}\\{ivs[4]}\\{ivs[5]}\nAbility: {Ability}\nNature: {(Nature)Nature}\nHeight: {Height}\nWeight: {Weight}\n\n";
    }
}
