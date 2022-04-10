using System.Diagnostics;
using PKHeX.Core;
using PLARNGGui;
using Newtonsoft.Json;
using System.Net;

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
                Console.WriteLine($"No outbreak in {areaName}.");
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
                    ulong inmapseedoff;
                    var inmapseedptr = new long[] { 0x42EEEE8, 0x78, 0xD48 + (j * 0x08), 0x58, 0x38, 0x478, 0x20 };
                    if (!Main.USB)
                    {
                        inmapseedoff = Main.routes.PointerAll(inmapseedptr).Result;
                        seed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(inmapseedoff, 8).Result);
                    }
                    else
                    {
                        inmapseedoff = Main.usbroutes.PointerAll(inmapseedptr).Result;
                        seed = BitConverter.ToUInt64(Main.usbroutes.ReadBytesAbsoluteAsync(inmapseedoff, 8).Result);
                    }
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

                Program.main.MassiveDisplay.AppendText($"Spawner {j + 1} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\n");
                Program.main.Teleporterdisplay.AppendText($"{areaName}\nSpawner {j + 1} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nCoords:\nX: {spawner.X}\nY: {spawner.Y}\nZ: {spawner.Z}\n\n");
                Program.main.MassiveDisplay.AppendText($"First Round Spawns: {spawn.BaseCount} Bonus Round Spawns: {spawn.BonusCount}\n");
                bool skittishBase = SpawnGenerator.IsSkittish(spawn.BaseTable);
                bool skittishBonus = SpawnGenerator.IsSkittish(spawn.BonusTable);
                var lines = result.GetLines(skittishBase, skittishBonus);
                foreach (var line in lines)
                    Program.main.MassiveDisplay.AppendText(line+"\n\n");
             
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
                if (!Main.USB)
                {
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
                }
                else
                {
                    var SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                    while (groupseed == 0 && group_id != minimum)
                    {
                        group_id -= 1;
                        SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
                        SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                        groupseed = BitConverter.ToUInt64(Main.usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result, 0);
                    }
                    SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x20 };
                    SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                    var GeneratorSeed = Main.usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                    seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                }
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
            bool skittishBase = SpawnGenerator.IsSkittish(spawner.DisplaySpecies);
            var lines = result.GetLines(skittishBase);
            foreach (var line in lines)
                Program.main.OutbreakDisplay.AppendText(line+"\n");
            Console.WriteLine();
        }
        Program.main.OutbreakDisplay.AppendText("Done permuting Mass Outbreaks.\n\n");
    }

    /// <summary>
    /// Permutes a single spawn with simple info.
    /// </summary>
    public static void PermuteSingle(SpawnInfo spawn, ulong seed, ushort species)
    {
        Program.main.nocfwpathdisplay.AppendText($"Permuting all possible paths for {seed:X16}.");
        Program.main.nocfwpathdisplay.AppendText($"Base Species: {SpeciesName.GetSpeciesName(species, 2)}");
    

        var result = Permuter.Permute(spawn, seed);
        if (!result.HasResults)
        {
            Program.main.nocfwpathdisplay.AppendText("No results found. Try another outbreak! :(");
        }
        else
        {
            bool skittishBase = SpawnGenerator.IsSkittish(spawn.BaseTable);
            bool skittishBonus = SpawnGenerator.IsSkittish(spawn.BonusTable);
            var lines = result.GetLines(skittishBase, skittishBonus);
            foreach (var line in lines)
                Program.main.nocfwpathdisplay.AppendText(line+"\n");
        }

        Program.main.nocfwpathdisplay.AppendText("\n");
        Program.main.nocfwpathdisplay.AppendText("Done.");
    }
}
