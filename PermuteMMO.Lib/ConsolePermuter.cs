using System.Diagnostics;
using PKHeX.Core;
using PLARNGGui;

namespace PermuteMMO.Lib;

/// <summary>
/// Logic to permute spawner data.
/// </summary>
public static class ConsolePermuter
{
    /// <summary>
    /// Permutes all the areas to print out all possible spawns.
    /// </summary>
    public static void PermuteBlock(byte[] data)
    {
        var block = new MassiveOutbreakSet8a(data);
        for (int i = 0; i < MassiveOutbreakSet8a.AreaCount; i++)
        {
            var area = block[i];
            var areaName = AreaUtil.AreaTable[area.AreaHash];
            if (!area.IsActive)
            {
               Program.main.MassiveDisplay.AppendText ($"No outbreak in {areaName}.\n");
                continue;
            }
            Debug.Assert(area.IsValid);

            Program.main.MassiveDisplay.AppendText($"Permuting all possible paths for {areaName}.\n");
           
            for (int j = 0; j < MassiveOutbreakArea8a.SpawnerCount; j++)
            {
                var spawner = area[j];
                if (spawner.Status is MassiveOutbreakSpawnerStatus.None)
                    continue;

                Debug.Assert(spawner.HasBase);
                var seed = spawner.SpawnSeed;
                var spawn = new SpawnInfo
                {
                    BaseCount = spawner.BaseCount,
                    BaseTable = spawner.BaseTable,

                    BonusCount = spawner.BonusCount,
                    BonusTable = spawner.BonusTable,
                };

                Permuter.Permute(spawn, seed);

                var result = Permuter.Permute(spawn, seed);
                if (!result.HasResults)
                    continue;

                Program.main.MassiveDisplay.AppendText ($"Spawner {j+1} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\n");
                Program.main.Teleporterdisplay.AppendText($"Area: {areaName}\nSpawner {j + 1}\nX: {spawner.X:F1}\nY: {spawner.Y:F1}\nZ: {spawner.Z:F1}\n");
                result.PrintResults();
              
            }
            Program.main.MassiveDisplay.AppendText("Done permuting area.\n\n");
      
        }
    }

    /// <summary>
    /// Permutes a single spawn with simple info.
    /// </summary>
    public static void PermuteSingle(SpawnInfo spawn, ulong seed)
    {
        Program.main.MassiveDisplay.AppendText($"Permuting all possible paths for {seed:X16}.\n");
        Program.main.MassiveDisplay.AppendText($"Parameters: {spawn}\n");
      

        var result = Permuter.Permute(spawn, seed);
        result.PrintResults();

      
        Program.main.MassiveDisplay.AppendText("Done.\n");
    }
}
