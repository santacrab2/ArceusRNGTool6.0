using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PermuteMMO.Lib;

namespace PLARNGGui
{
    public class OutbreakRoutines
    {
        public static int spawncount;
        public void ReadOutbreakJubilife(Span<byte> data)
        {
            Program.main.outbreakmap.DataSource = null;
            Program.main.outbreakmap.Items.Clear();
            var block = new MassOutbreakSet8a(data);
            for(int i = 0; i < 5; i++)
            {
                var spawner = block[i];
                var areaName = AreaUtil.AreaTable[spawner.AreaHash];
                Program.main.outbreakmap.Items.Add($"{areaName}");
                if (!spawner.HasOutbreak)
                {
                    Program.main.OutbreakDisplay.AppendText($"No outbreak in {areaName}.\n");
                    continue;
                }
                spawncount = spawner.BaseCount;
                Program.main.outbreakspawncount.Text = $"{spawner.BaseCount}";
                var mainrng = new Xoroshiro128Plus(spawner.SpawnSeed);
                Program.main.OutbreakDisplay.AppendText($"Outbreak in {areaName} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nSpawn Count: {spawner.BaseCount}\n");
                Program.main.Teleporterdisplay.AppendText($"Outbreak in {areaName} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nCoords:\nX: {spawner.X}\nY: {spawner.Y}\nZ: {spawner.Z}\n");
                GenerateCurrentMassOutbreak(mainrng);
            }
        }
        public void ReadOutbreakID()
        {
            ulong seed1 = 0x82A2B175229D6A5B;
            bool alphamatch = false;
            ulong sseed = 0;
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.outbreakmap.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.outbreakmap.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            var minimum = spawnmap.Count() - 16;
            var group_id = minimum + 30;
            ulong groupseed = 0;
            var coordsptr = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x20 };
            var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
            ulong SpawnerOff;
            MassOutbreakSpawner8a spawner = new();
            if (!Main.USB)
            {
                var coordsoff = Main.routes.PointerAll(coordsptr).Result;
                var theblock = new MassOutbreakSet8a(Main.routes.ReadBytesAbsoluteAsync(coordsoff, 0x190).Result);
                
                for(int i = 0; i < 5; i++)
                {
                    spawner = theblock[i];
                    if (spawner.HasOutbreak)
                        break;
                }
                SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                while (groupseed == 0 && group_id != minimum)
                {
                    group_id -= 1;
                    SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
                    SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                    groupseed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result, 0);
                }
                if (group_id == minimum)
                {
                    Program.main.OutbreakDisplay.AppendText("No Outbreaks Found.");
                    return;
                }
            }
            else
            {
                var coordsoff = Main.usbroutes.PointerAll(coordsptr).Result;
                var theblock = new MassOutbreakSet8a(Main.usbroutes.ReadBytesAbsoluteAsync(coordsoff, 0x190).Result);

                for (int i = 0; i < 5; i++)
                {
                    spawner = theblock[i];
                    if (spawner.HasOutbreak)
                        break;
                }
                SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                while (groupseed == 0 && group_id != minimum)
                {
                    group_id -= 1;
                    SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
                    SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                    groupseed = BitConverter.ToUInt64(Main.usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result, 0);
                }
                if (group_id == minimum)
                {
                    Program.main.OutbreakDisplay.AppendText("No Outbreaks Found.");
                    return;
                }
            }
            Program.main.outbreakgroupid.Text = $"{group_id}";

            byte[] GeneratorSeed;
            SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x20 };
            if (!Main.USB)
            {
                SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                GeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                Program.main.OutbreakDisplay.AppendText($"Species: {SpeciesName.GetSpeciesName(spawner.DisplaySpecies,2)}\nGenerator Seed: {BitConverter.ToString(GeneratorSeed).Replace("-", "")}\n");
                var group_seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                Program.main.OutbreakDisplay.AppendText($"Group Seed: {string.Format("0x{0:X}", group_seed)}\n");
                Program.main.Teleporterdisplay.AppendText($"Outbreak in {AreaUtil.AreaTable[spawner.AreaHash]} shows {SpeciesName.GetSpeciesName(spawner.DisplaySpecies, 2)}\nCoords:\nX: {spawner.X}\nY: {spawner.Y}\nZ: {spawner.Z}");
                var spawns = 0;
                for (int i = 0; i < 4; i++)
                {
                    SpawnerOffpoint = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x60 + i * 0x50 };
                    SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                    spawns = BitConverter.ToInt16(Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 2).Result, 0);
                    if (10 <= spawns && spawns <= 15)
                    {
                        Program.main.outbreakspawncount.Text = $"{spawns}";
                        break;
                    }
                }
                var main_rng = new Xoroshiro128Plus(group_seed);
                GenerateCurrentMassOutbreak(main_rng);
            }
            else
            {
                SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                GeneratorSeed = Main.usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                Program.main.OutbreakDisplay.AppendText($"Generator Seed: {BitConverter.ToString(GeneratorSeed).Replace("-", "")}\n");
                var group_seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                Program.main.OutbreakDisplay.AppendText($"Group Seed: {string.Format("0x{0:X}", group_seed)}\n");
                var spawns = 0;
                for (int i = 0; i < 4; i++)
                {
                    SpawnerOffpoint = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x60 + i * 0x50 };
                    SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                    spawns = BitConverter.ToInt16(Main.usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 2).Result, 0);
                    if (10 <= spawns && spawns <= 15)
                    {
                        Program.main.outbreakspawncount.Text = $"{spawns}";
                        break;
                    }
                }
                var main_rng = new Xoroshiro128Plus(group_seed);
                GenerateCurrentMassOutbreak(main_rng);
            }
        }
        public void GenerateCurrentMassOutbreak(Xoroshiro128Plus mainrng)
        {
            bool shiny = false;
            ulong encryption_constant = new ulong();
            ulong pid = new ulong();
            int[] ivs = new int[6];
            ulong ability = new ulong();
            ulong gender = new ulong();
            ulong nature = new ulong();
            ulong shinyseed = new ulong();
            ulong GeneratorSeed;
            Xoroshiro128Plus fixedrng;
            double EncounterSlotRand;
            bool alpha = false;
            ulong fixedseed;
            for (int i = 1; i < 5; i++)
            {
                GeneratorSeed = mainrng.Next();
                mainrng.Next();
                fixedrng = new Xoroshiro128Plus(GeneratorSeed);
                EncounterSlotRand = (float)(101 * (float)((float)fixedrng.Next() * 5.421e-20f)) + 0.0;
                if (EncounterSlotRand >= 100)
                    alpha = true;
                fixedseed = fixedrng.Next();
                (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, Convert.ToInt32(Program.main.outbreakShinyrolls.Text), alpha ? 3 : 0);
                Program.main.OutbreakDisplay.AppendText($"Initial Spawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                alpha = false;
            }
            ulong groupseed = mainrng.Next();
            mainrng = new Xoroshiro128Plus(groupseed);
            var respawnrng = new Xoroshiro128Plus(groupseed);
            int spawns;
            if (Program.main.inmapbox.Checked)
                spawns = Convert.ToInt32(Program.main.outbreakspawncount.Text);
            else
                spawns = spawncount;
            for (int i = 1; i < spawns - 3; i++)
            {
                GeneratorSeed = respawnrng.Next();
                respawnrng.Next();
                respawnrng = new Xoroshiro128Plus(respawnrng.Next());
                fixedrng = new Xoroshiro128Plus(GeneratorSeed);
                EncounterSlotRand = (float)(101 * (float)((float)fixedrng.Next() * 5.421e-20f)) + 0.0;
                if (EncounterSlotRand >= 100)
                    alpha = true;
                fixedseed = fixedrng.Next();
                (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, Convert.ToInt32(Program.main.outbreakShinyrolls.Text), alpha ? 3 : 0);
                Program.main.OutbreakDisplay.AppendText($"Respawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nSeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                alpha = false;
            }
            return;
        }
        public void GenerateNextOutbreakMatch()
        {
            if(!Program.main.shinysearch.Checked && !Program.main.AlphaSearch.Checked)
            {
                Program.main.OutbreakDisplay.AppendText("Please set at least one search setting!\n");
                return;
            }
            Xoroshiro128Plus mainrng = new();
            if (Program.main.inmapbox.Checked)
            {
                byte[] startGeneratorSeed;
                var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + Convert.ToInt32(Program.main.outbreakgroupid.Text) * 0x440 + 0x20 };
                if (!Main.USB)
                {
                    var SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                    startGeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                }
                else
                {
                    var SpawnerOff = Main.usbroutes.PointerAll(SpawnerOffpoint).Result;
                    startGeneratorSeed = Main.usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                }
                Program.main.OutbreakDisplay.AppendText($"Generator Seed: {BitConverter.ToString(startGeneratorSeed).Replace("-", "")}\n");
                var group_seed = (BitConverter.ToUInt64(startGeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                Program.main.OutbreakDisplay.AppendText($"Group Seed: {string.Format("0x{0:X}", group_seed)}\n");
                mainrng = new Xoroshiro128Plus(group_seed);
            }
            else
            {
                var outbreakptr = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x20 + (0x50 * Program.main.outbreakmap.SelectedIndex) + 0x38 };
                if (!Main.USB)
                {
                    var outbreakoff = Main.routes.PointerAll(outbreakptr).Result;
                    var seed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(outbreakoff, 8).Result);
                    var groupseed = (seed - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                    mainrng = new Xoroshiro128Plus(groupseed);
                }
                else
                {
                    var outbreakoff = Main.usbroutes.PointerAll(outbreakptr).Result;
                    var seed = BitConverter.ToUInt64(Main.usbroutes.ReadBytesAbsoluteAsync(outbreakoff, 8).Result);
                    mainrng = new Xoroshiro128Plus(seed);
                }
            }
            bool shiny = false;
            ulong encryption_constant = new ulong();
            ulong pid = new ulong();
            int[] ivs = new int[6];
            ulong ability = new ulong();
            ulong gender = new ulong();
            ulong nature = new ulong();
            ulong shinyseed = new ulong();
            ulong GeneratorSeed;
            Xoroshiro128Plus fixedrng;
            double EncounterSlotRand;
            bool alpha = false;
            ulong fixedseed;
            int advances = 0;
            while (advances < Convert.ToInt32(Program.main.outbreakmaxadv.Text))
            {
                for (int i = 1; i < 5; i++)
                {
                    GeneratorSeed = mainrng.Next();
                    mainrng.Next();
                    fixedrng = new Xoroshiro128Plus(GeneratorSeed);
                    EncounterSlotRand = (float)(101 * (float)((float)fixedrng.Next() * 5.421e-20f)) + 0.0;
                    if (EncounterSlotRand >= 100)
                        alpha = true;
                    fixedseed = fixedrng.Next();
                    (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, Convert.ToInt32(Program.main.outbreakShinyrolls.Text), alpha ? 3 : 0);

                    if ((Program.main.shinysearch.Checked && shiny) && Program.main.AlphaSearch.Checked && alpha)
                    {
                        Program.main.OutbreakDisplay.AppendText($"Initial Spawn: {i}\nAdvances: {advances}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                       
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    if ((Program.main.shinysearch.Checked && shiny) && (!Program.main.AlphaSearch.Checked&&!alpha))
                    {
                        Program.main.OutbreakDisplay.AppendText($"Initial Spawn: {i}\nAdvances: {advances}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                 
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    if((!Program.main.shinysearch.Checked && !shiny) && (Program.main.AlphaSearch.Checked && alpha))
                    {
                        Program.main.OutbreakDisplay.AppendText($"Initial Spawn: {i}\nAdvances: {advances}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                 
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    
                    alpha = false;
                }
                ulong groupseed = mainrng.Next();
                mainrng = new Xoroshiro128Plus(groupseed);
                var respawnrng = new Xoroshiro128Plus(groupseed);
                int spawns = Convert.ToInt32(Program.main.outbreakspawncount.Text);
                for (int i = 1; i < spawns - 3; i++)
                {
                    GeneratorSeed = respawnrng.Next();
                    respawnrng.Next();
                    respawnrng = new Xoroshiro128Plus(respawnrng.Next());
                    fixedrng = new Xoroshiro128Plus(GeneratorSeed);
                    EncounterSlotRand = (float)(101 * (float)((float)fixedrng.Next() * 5.421e-20f)) + 0.0;
                    if (EncounterSlotRand >= 100)
                        alpha = true;
                    fixedseed = fixedrng.Next();
                    (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, Convert.ToInt32(Program.main.outbreakShinyrolls.Text), alpha ? 3 : 0);

                    if ((Program.main.shinysearch.Checked && shiny) && Program.main.AlphaSearch.Checked && alpha)
                    {
                        Program.main.OutbreakDisplay.AppendText($"ReSpawn: {i}\nAdvances: {advances}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                     
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    if ((Program.main.shinysearch.Checked && shiny) && (!Program.main.AlphaSearch.Checked && !alpha))
                    {
                        Program.main.OutbreakDisplay.AppendText($"ReSpawn: {i}\nAdvances: {advances}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                      
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    if ((!Program.main.shinysearch.Checked && !shiny) && (Program.main.AlphaSearch.Checked && alpha))
                    {
                        Program.main.OutbreakDisplay.AppendText($"ReSpawn: {i}\nAdvances: {advances}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                      
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    alpha = false;
                }
                advances++;
            }
            if (advances >= Convert.ToInt32(Program.main.outbreakmaxadv.Text))
            {
                Program.main.OutbreakDisplay.AppendText("No Matches found within your max advances\n");
            }

        }
    }
}
