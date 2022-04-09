using System.Diagnostics;
using PKHeX.Core;
using PLARNGGui;
using System.Net;
using Newtonsoft.Json;

namespace PermuteMMO.Lib;

/// <summary>
/// Logic to permute spawner data.
/// </summary>
public static class ConsolePermuter
{
    static ConsolePermuter() => Console.OutputEncoding = System.Text.Encoding.Unicode;

    /// <summary>
    /// Permutes all the areas to print out all possible spawns.
    /// </summary>
    public static void PermuteMassiveMassOutbreak(Span<byte> data)
    {
        var block = new MassiveOutbreakSet8a(data);
        for (int i = 0; i < MassiveOutbreakSet8a.AreaCount; i++)
        {
            var area = block[i];
            var areaName = AreaUtil.AreaTable[area.AreaHash];
            if (!area.IsActive)
            {
                Program.main.MassiveDisplay.AppendText($"No outbreak in {areaName}.\n");
                continue;
            }
            Debug.Assert(area.IsValid);

            bool hasPrintedAreaMMO = false;
            for (int j = 0; j < MassiveOutbreakArea8a.SpawnerCount; j++)
            {
                var spawner = area[j];
                if (spawner.Status is MassiveOutbreakSpawnerStatus.None)
                    continue;

                Debug.Assert(spawner.HasBase);
                var seed = spawner.SpawnSeed;
                if (Program.main.mmoinmap.Checked)
                {
                    var inmapseedptr = new long[] { 0x42EEEE8, 0x78, 0xD48 + (j * 0x08), 0x58, 0x38, 0x478, 0x20 };
                    var inmapseedoff = Main.routes.PointerAll(inmapseedptr).Result;
                    seed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(inmapseedoff, 8).Result);
                    var rnger = new Xoroshiro128Plus(seed);
                    seed = rnger.Next();
                }
                var spawn = new SpawnInfo
                {
                    BaseCount = spawner.BaseCount,
                    BaseTable = spawner.BaseTable,

                    BonusCount = spawner.HasBonus ? spawner.BonusCount : 0,
                    BonusTable = spawner.HasBonus ? spawner.BonusTable : 0,
                };

                var result = Permuter.Permute(spawn, seed);
                if (!result.HasResults)
                    continue;

                if (!hasPrintedAreaMMO)
                {
                    Program.main.MassiveDisplay.AppendText($"Found paths for Massive Mass Outbreaks in {areaName}.\n");
                    hasPrintedAreaMMO = true;
                }

                Program.main.MassiveDisplay.AppendText($"Spawner {j+1} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\n");
                Program.main.Teleporterdisplay.AppendText($"{areaName}\nSpawner {j + 1} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nCoords:\nX: {spawner.X}\nY: {spawner.Y}\nZ: {spawner.Z}\n\n");
                Program.main.MassiveDisplay.AppendText($"First Round Spawns: {spawn.BaseCount} Bonus Round Spawns: {spawn.BonusCount}\n");
                bool hasSkittish = SpawnGenerator.IsSkittish(spawn.BaseTable);
                result.PrintResults(hasSkittish,true);
                
            }

            if (!hasPrintedAreaMMO)
            {
                Program.main.MassiveDisplay.AppendText($"Found no results for any Massive Mass Outbreak in {areaName}.\n");
            }
            else
            {
                Program.main.MassiveDisplay.AppendText("Done permuting area.\n\n");
               
            }
        }
    }

    /// <summary>
    /// Permutes all the Mass Outbreaks to print out all possible spawns.
    /// </summary>
    public static void PermuteBlockMassOutbreak(Span<byte> data)
    {
        Program.main.OutbreakDisplay.AppendText("Permuting Mass Outbreaks.\n");
        var block = new MassOutbreakSet8a(data);
        for (int i = 0; i < MassOutbreakSet8a.AreaCount; i++)
        {
            var spawner = block[i];
            var areaName = AreaUtil.AreaTable[spawner.AreaHash];
            if (!spawner.HasOutbreak)
            {
                Program.main.OutbreakDisplay.AppendText($"No outbreak in {areaName}.\n");
                continue;
            }
            Debug.Assert(spawner.IsValid);

            var seed = spawner.SpawnSeed;
            if (Program.main.inmapbox.Checked)
            {
                var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.outbreakmap.SelectedItem}.json");
                var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
                var minimum = spawnmap.Count() - 16;
                var group_id = minimum + 30;
                ulong groupseed = 0;
                var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
                var SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                while (groupseed == 0 && group_id != minimum)
                {
                    group_id -= 1;
                    SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
                    SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                    groupseed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result, 0);
                }
                SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x20 };
                SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                var GeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                Program.main.outbreakgroupid.Text = group_id.ToString();
            }
            var spawn = new SpawnInfo
            {
                BaseCount = spawner.BaseCount,
                BaseTable = spawner.DisplaySpecies,
                Type = SpawnType.Outbreak,
            };

            var result = Permuter.Permute(spawn, seed);
            if (!result.HasResults)
            {
                Program.main.OutbreakDisplay.AppendText($"Found no paths for {(Species)spawner.DisplaySpecies} Mass Outbreak in {areaName}.\n");
                Program.main.Teleporterdisplay.AppendText($"Outbreak shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nCoords:\nX: {spawner.X}\nY: {spawner.Y}\nZ: {spawner.Z}\n");
                continue;
            }

            Program.main.OutbreakDisplay.AppendText($"Found paths for {(Species)spawner.DisplaySpecies} Mass Outbreak in {areaName}:\n");
            Program.main.OutbreakDisplay.AppendText($"Outbreak shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\n");
            Program.main.Teleporterdisplay.AppendText($"Outbreak shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nCoords:\nX: {spawner.X}\nY: {spawner.Y}\nZ: {spawner.Z}\n");
            Program.main.OutbreakDisplay.AppendText($"Spawn Count: {spawn.BaseCount}\n");
            bool hasSkittish = SpawnGenerator.IsSkittish(spawner.DisplaySpecies);
            result.PrintResults(hasSkittish,false);
            
        }
        Program.main.OutbreakDisplay.AppendText("Done permuting Mass Outbreaks.\n\n");
     
    }

    /// <summary>
    /// Permutes a single spawn with simple info.
    /// </summary>
    public static void PermuteSingle(SpawnInfo spawn, ulong seed, ushort species)
    {
        Console.WriteLine($"Permuting all possible paths for {seed:X16}.");
        Console.WriteLine($"Base Species: {SpeciesName.GetSpeciesName(species, 2)}");
        Console.WriteLine($"Parameters: {spawn}");
        Console.WriteLine();

        var result = Permuter.Permute(spawn, seed);
        if (!result.HasResults)
        {
            Console.WriteLine("No results found. Try another outbreak! :(");
        }
        else
        {
            bool hasSkittish = SpawnGenerator.IsSkittish(spawn.BaseTable);
            result.PrintResults(hasSkittish,false);
        }

        Console.WriteLine();
        Console.WriteLine("Done.");
    }
}
