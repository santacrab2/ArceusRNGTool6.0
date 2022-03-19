using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PKHeX.Core;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PLARNGGui
{
    public class OutbreakRoutines
    {
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
            var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x408 };
            var SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
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
            Program.main.outbreakgroupid.Text = $"{group_id}";


            SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + group_id * 0x440 + 0x20 };
            SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
            var GeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
            Program.main.OutbreakDisplay.AppendText($"Generator Seed: {BitConverter.ToString(GeneratorSeed).Replace("-", "")}\n");
            var group_seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
            Program.main.OutbreakDisplay.AppendText($"Group Seed: {string.Format("0x{0:X}", group_seed)}\n");
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
                Program.main.OutbreakDisplay.AppendText($"Respawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                alpha = false;
            }
            return;
        }
        public void GenerateNextOutbreakMatch()
        {
            var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + Convert.ToInt32(Program.main.outbreakgroupid.Text) * 0x440 + 0x20 };
            var SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
            var startGeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
            Program.main.OutbreakDisplay.AppendText($"Generator Seed: {BitConverter.ToString(startGeneratorSeed).Replace("-", "")}\n");
            var group_seed = (BitConverter.ToUInt64(startGeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
            Program.main.OutbreakDisplay.AppendText($"Group Seed: {string.Format("0x{0:X}", group_seed)}\n");
            var mainrng = new Xoroshiro128Plus(group_seed);
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

                    if (shiny && Program.main.AlphaSearch.Checked && alpha)
                    {
                        Program.main.OutbreakDisplay.AppendText($"Initial Spawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    if (shiny && !Program.main.AlphaSearch.Checked)
                    {
                        Program.main.OutbreakDisplay.AppendText($"Initial Spawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
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

                    if (shiny && Program.main.AlphaSearch.Checked && alpha)
                    {
                        Program.main.OutbreakDisplay.AppendText($"Respawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
                        Program.main.outbreakseedtoinject.Text = string.Format("0x{0:X}", GeneratorSeed);
                        return;
                    }
                    if (shiny && !Program.main.AlphaSearch.Checked)
                    {
                        Program.main.OutbreakDisplay.AppendText($"Respawn: {i}\nAlpha: {alpha}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", GeneratorSeed)}\n\n");
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
