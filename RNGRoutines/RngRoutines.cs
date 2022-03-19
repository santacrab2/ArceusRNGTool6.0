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
    public class RngRoutines 
    {

        public (bool shiny, ulong EC, ulong PID, int[] IVs, ulong ability, ulong gender, ulong nature, ulong fixedseed) GenerateFromSeed(ulong seed, int rolls, int guranteedivs)
        {
            
            bool shiny = false;
            ulong EC;
            ulong pid = 0;
            int[] ivs;
            ulong ability;
            ulong gender;
            ulong nature;
            ulong sseed = 0;
            Random rand = new Random();
            var rng = new Xoroshiro128Plus(seed);

            EC = rng.NextInt(0xFFFFFFFF);
            var sidtid = rng.NextInt(0xFFFFFFFF);
            for (int i = 0; i < rolls; i++)
            {
                pid = rng.NextInt(0xFFFFFFFF);
                shiny = ((pid >> 16) ^ (sidtid >> 16) ^ (pid & 0xFFFF) ^ (sidtid & 0xFFFF)) < 0x10;

                if (shiny)
                {
                    sseed = rng.GetState().s0;
                    break;
                }
            }

            ivs = new int[] { -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < guranteedivs; i++)
            {
                var index = rng.NextInt(6);
                while ((int)index >= 6)
                    index = rng.NextInt(6);


                while (ivs[index] != -1)
                {
                    index = rng.NextInt(6);
                    while ((int)index >= 6)
                        index = rng.NextInt(6);

                }

                ivs[index] = 31;
            }
            for (int i = 0; i < 6; i++)
            {
                if (ivs[i] == -1)
                    ivs[i] = (int)rng.NextInt(32);
            }
            ability = rng.NextInt(2);
            gender = rng.NextInt(252) + 1;
            nature = rng.NextInt(25);
            while ((int)nature >= 25)
                nature = rng.NextInt(25);
            return (shiny, EC, pid, ivs, ability, gender, nature, sseed);
        }

        public ulong GenerateNextStandardMatch(ulong seed)
        {
            ulong seed1 = 0x82A2B175229D6A5B;
            bool alphamatch = false;
           
            ulong sseed = 0;
            var mainrng = new Xoroshiro128Plus(seed);
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            var encslotsum = 101;
            var encmin = 0;
            var encmax = 100;
                var groupname = spawnmap[$"{Program.main.standardgroupid.SelectedItem}"]["name"];
               

                var possiblespawns = encmap[$"{groupname}"][$"{((Enums.Time)Program.main.Timeofdayselection.SelectedItem == Enums.Time.Any ? "Any Time" : $"{Program.main.Timeofdayselection.SelectedItem}")}" + "/" + $"{((Enums.Weather)Program.main.weatherselection.SelectedItem == Enums.Weather.All ? "All Weather" : $"{Program.main.weatherselection.SelectedItem}")}"];


                foreach (var key in possiblespawns)
                {
                    encslotsum += (int)key.Value;
                }

                

                foreach (var key in possiblespawns)
                {

                    var keyspecies = $"{key}".Remove($"{key}".IndexOf(":")).Replace("\"", "");

                    if ((string)Program.main.pokemonselection.SelectedItem == keyspecies)
                    {
                        encmax = encmin + (int)key.Value;
                        break;
                    }
                    else
                        encmin += (int)key.Value;
                }
            
            bool shiny = false;
            ulong encryption_constant = new ulong();
            ulong pid = new ulong();
            int[] ivs = new int[6];
            ulong ability = new ulong();
            ulong gender = new ulong();
            ulong nature = new ulong();
            ulong shinyseed = new ulong();
            int advances = 0;
            var maxadv = Convert.ToUInt32(Program.main.MaxAdvances.Text);


            for (int i = 0; i <= maxadv; i++)
            {
               
               
                var generator_seed = mainrng.Next();
                mainrng.Next();
                var rng = new Xoroshiro128Plus(generator_seed);
                var EncounterSlotRand = (float)(encslotsum * (float)((float)rng.Next() * 5.421e-20f)) + 0.0;
                (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = GenerateFromSeed(rng.Next(), Convert.ToInt32(Program.main.ShinyRollstext.Text), Convert.ToInt32(Program.main.guaranteedivs.Text));
                if (encmin >= (float)EncounterSlotRand && (float)EncounterSlotRand > encmax)
                {
                    continue;
                }
                

                if (shiny)
                {
                    
                        sseed = generator_seed;
                        break;
                }

                mainrng = new Xoroshiro128Plus(mainrng.Next());
                sseed = generator_seed;
                advances = i;

            }
            if (advances >= Convert.ToInt32(Program.main.MaxAdvances.Text))
            {

                Program.main.StandardSpawnsDisplay.AppendText("No Match Found with in your Max Advances\n");
                Program.main.StandardSpawnsDisplay.AppendText($"Advances: {advances}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", sseed)}\n");
                Program.main.SeedToInject.Text = string.Format("0x{0:X}", sseed);
                return sseed;
            }
           Program.main.StandardSpawnsDisplay.AppendText($"Advances: {advances}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", sseed)}\n");
            Program.main.SeedToInject.Text = string.Format("0x{0:X}", sseed);


            return sseed;
        }
        

        public void StandardCheckNear()
        {

            ulong seed1 = 0x82A2B175229D6A5B;
            bool alphamatch = false;

            ulong sseed = 0;
            
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            var encslotsum = 101;
            var encmin = 0;
            var encmax = 0;
            var species = "";
            for (int f = 0; f < spawnmap.Count; f++)
            {
                var groupid = f;
                var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + groupid * 0x440 + 0x20 };
                var SpawnerOff = Main.routes.PointerAll(SpawnerOffpoint).Result;
                var GeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                var group_seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                var mainrng = new Xoroshiro128Plus(group_seed);
                var groupname = spawnmap[$"{f}"]["name"];
                dynamic possiblespawns;
                possiblespawns = encmap[$"{groupname}"][$"{((Enums.Time)Program.main.Timeofdayselection.SelectedItem == Enums.Time.Any ? "Any Time" : $"{Program.main.Timeofdayselection.SelectedItem}")}" + "/" + $"{((Enums.Weather)Program.main.weatherselection.SelectedItem == Enums.Weather.All ? "All Weather" : $"{Program.main.weatherselection.SelectedItem}")}"];
                if(possiblespawns == null || possiblespawns.Count == 0)
                {
                    possiblespawns = encmap[$"{groupname}"][$"Any Time/All Weather"];
                }
                if (possiblespawns == null || possiblespawns.Count == 0)
                    possiblespawns = encmap[$"{groupname}"][$"Any Time/{Program.main.weatherselection.SelectedItem}"];
                if (possiblespawns == null || possiblespawns.Count == 0)
                    possiblespawns = encmap[$"{groupname}"][$"{Program.main.Timeofdayselection.SelectedItem}/All Weather"];
                foreach (var key in possiblespawns)
                {
                    encslotsum += (int)key.Value;
                }

                bool shiny = false;
                ulong encryption_constant = new ulong();
                ulong pid = new ulong();
                int[] ivs = new int[6];
                ulong ability = new ulong();
                ulong gender = new ulong();
                ulong nature = new ulong();
                ulong shinyseed = new ulong();
                int advances = 0;
                var maxadv = Convert.ToUInt32(Program.main.MaxAdvances.Text);


                for (int i = 0; i <= maxadv; i++)
                {


                    var generator_seed = mainrng.Next();
                    mainrng.Next();
                    var rng = new Xoroshiro128Plus(generator_seed);
                    var EncounterSlotRand = (float)(encslotsum * (float)((float)rng.Next() * 5.421e-20f)) + 0.0;
                    
                    foreach (var key in possiblespawns)
                    {
                        encmax += (int)key.Value;
                        if (EncounterSlotRand > encmax)
                        {
                            
                            continue;
                        }
                        species = $"{key}".Remove($"{key}".IndexOf(":")).Replace("\"", "");
                        break;
                    }
                    (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = GenerateFromSeed(rng.Next(), Convert.ToInt32(Program.main.ShinyRollstext.Text), Convert.ToInt32(Program.main.guaranteedivs.Text));

                    if (shiny)
                    {

                        sseed = generator_seed;
                        break;
                    }

                    mainrng = new Xoroshiro128Plus(mainrng.Next());
                    sseed = generator_seed;
                    advances = i;

                }
                if (advances >= Convert.ToInt32(Program.main.MaxAdvances.Text))
                {
                    continue;
                }
                Program.main.StandardSpawnsDisplay.AppendText($"Group ID: {f}\nAdvances: {advances}\nSpecies{species}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", sseed)}\n\n");
                Program.main.SeedToInject.Text = string.Format("0x{0:X}", sseed);
                Program.main.Teleporterdisplay.AppendText($"Group ID: {f}\nCoords: {spawnmap[f.ToString()]["coords"]}\nSpecies{species}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\n\n");
                Program.main.CoordX.Text = spawnmap[f.ToString()]["coords"][0].ToString();
                Program.main.CoordY.Text = spawnmap[f.ToString()]["coords"][1].ToString();
                Program.main.CoordZ.Text = spawnmap[f.ToString()]["coords"][2].ToString();

            }
            return;
        }
     
    }
}

