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
    public class MMORoutines
    {
        public void ReadMassOutbreak()
        {
            string map = "";
            var shinyrolls = Convert.ToInt32(Program.main.MMOSRs.Text);
            for (int l = 0; l < 5; l++)
            {
                for (int i = 0; i < 15; i++)
                {
                    var outbreakpointer = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x1d4 + (i * 0x90) + (0xB80 * l) };
                    ulong Outbreakoff;
                    if(!Main.USB)
                        Outbreakoff = Main.routes.PointerAll(outbreakpointer).Result;
                    else
                        Outbreakoff = Main.usbroutes.PointerAll(outbreakpointer).Result;

                    if (i == 0)
                    {
                        byte[] location;
                        if(!Main.USB)
                            location = Main.routes.ReadBytesAbsoluteAsync(Outbreakoff - 0x24, 2).Result;
                        else
                            location = Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff - 0x24, 2).Result;
                        map = BitConverter.ToString(location);
                        switch (map)
                        {
                            case "B7-56": map = "in the Cobalt Coastlands"; break;
                            case "04-55": map = "in the Crimson Mirelands"; break;
                            case "51-53": map = "in the Alabaster Icelands"; break;
                            case "9E-51": map = "in the Coronet Highlands"; break;
                            case "1D-5A": map = "in the Obsidian Fieldlands"; break;
                            case "45-26": map = "none"; break;
                        }

                        Program.main.MassiveDisplay.AppendText($"Searching {map}\n");
                    }
                    int species;
                    if (!Main.USB)
                        species = BitConverter.ToUInt16(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff, 2).Result, 0);
                    else
                        species = BitConverter.ToUInt16(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff, 2).Result, 0);
                    Task.Delay(1000);

                    if (species != 0)
                    {
                        if (species == 201)
                            shinyrolls = 19;
                        Program.main.MassiveDisplay.AppendText($"GroupID: {i} {(Species)species}\n");
                        bool shiny = false;
                        ulong encryption_constant = new ulong();
                        ulong pid = new ulong();
                        int[] ivs = new int[6];
                        ulong ability = new ulong();
                        ulong gender = new ulong();
                        ulong nature = new ulong();
                        ulong shinyseed = new ulong();
                        float spawncoordx;
                        float spawncoordy;
                        float spawncoordz;
                        ulong groupseed;
                        int maxspawns;
                        int currspawns;
                        ushort bonusround;
                        int bonuscount;
                        bool bonus;
                        if (!Main.USB)
                        {
                            spawncoordx = BitConverter.ToSingle(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff - 0x14, 4).Result, 0);
                            spawncoordy = BitConverter.ToSingle(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff - 0x10, 4).Result, 0);
                            spawncoordz = BitConverter.ToSingle(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff - 0x0C, 4).Result, 0);
                            Program.main.MassiveDisplay.AppendText($"Coordinates: X: {spawncoordx} Y: {spawncoordy} Z: {spawncoordz}\n");
                            groupseed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x44, 8).Result, 0);
                            maxspawns = BitConverter.ToInt32(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x4c, 4).Result, 0);

                            Program.main.MassiveDisplay.AppendText($"Max spawns: {maxspawns}\n");
                            currspawns = BitConverter.ToInt32(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x50, 4).Result, 0);
                            Program.main.MassiveDisplay.AppendText($"Current spawns: {currspawns}\n\n");
                            bonusround = BitConverter.ToUInt16(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x18, 2).Result, 0);
                            var bonustable = GetEncountersum(i, true, l);
                            bonus = false;
                            if (bonustable.Item1 != null)
                                bonus = true;
                            bonuscount = BitConverter.ToInt16(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x60, 2).Result, 0);
                        }
                        else
                        {
                            spawncoordx = BitConverter.ToSingle(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff - 0x14, 4).Result, 0);
                            spawncoordy = BitConverter.ToSingle(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff - 0x10, 4).Result, 0);
                            spawncoordz = BitConverter.ToSingle(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff - 0x0C, 4).Result, 0);
                            Program.main.MassiveDisplay.AppendText($"Coordinates: X: {spawncoordx} Y: {spawncoordy} Z: {spawncoordz}\n");
                            groupseed = BitConverter.ToUInt64(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x44, 8).Result, 0);
                            maxspawns = BitConverter.ToInt32(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x4c, 4).Result, 0);

                            Program.main.MassiveDisplay.AppendText($"Max spawns: {maxspawns}\n");
                            currspawns = BitConverter.ToInt32(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x50, 4).Result, 0);
                            Program.main.MassiveDisplay.AppendText($"Current spawns: {currspawns}\n\n");
                            bonusround = BitConverter.ToUInt16(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x18, 2).Result, 0);
                            var bonustable = GetEncountersum(i, true, l);
                            bonus = false;
                            if (bonustable.Item1 != null)
                                bonus = true;
                            bonuscount = BitConverter.ToInt16(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x60, 2).Result, 0);
                        }
                       
                        var mainrng = new Xoroshiro128Plus(groupseed);
                        for (int h = 0; h < 4; h++)
                        {
                            var generatorseed = mainrng.Next();
                            mainrng.Next();
                            var fixedrng = new Xoroshiro128Plus(generatorseed);
                            fixedrng.Next();
                            var fixedseed = fixedrng.Next();
                            (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, shinyrolls, 0);
                            if (shiny)
                            {
                                Program.main.MassiveDisplay.AppendText($"Initial Spawn {h}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generatorseed)}\n\n");
                                Program.main.Teleporterdisplay.AppendText($"Map: {map.Replace("in the ", "")}\nInitial Spawn {h}\nSpecies: {(Species)species}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generatorseed)}\n\n");
                                Program.main.Teleporterdisplay.AppendText($"Coords:\nX: {spawncoordx}\nY: {spawncoordy}\nZ: {spawncoordz}\n ");
                                Program.main.CoordX.Text = $"{spawncoordx}"; Program.main.CoordY.Text = $"{spawncoordy}"; Program.main.CoordZ.Text = $"{spawncoordz}";
                            }
                        }
                        groupseed = mainrng.Next();
                        mainrng = new Xoroshiro128Plus(groupseed);
                        var respawnrng = new Xoroshiro128Plus(groupseed);
                        for (int p = 1; p < maxspawns - 3; p++)
                        {
                            var generator_seed = respawnrng.Next();
                            respawnrng.Next();
                            respawnrng = new Xoroshiro128Plus(respawnrng.Next());
                            var fixed_rng = new Xoroshiro128Plus(generator_seed);
                            fixed_rng.Next();
                            var fixed_seed = fixed_rng.Next();

                            (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixed_seed, 13, 0);
                            if (shiny)
                            {
                                Program.main.MassiveDisplay.AppendText($"Respawn {p}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generator_seed)}\n\n");
                                Program.main.Teleporterdisplay.AppendText($"Map: {map.Replace("in the ", "")}\nRespawn {p}\nSpecies: {(Species)species}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generator_seed)}\n\n");
                                Program.main.Teleporterdisplay.AppendText($"Coords:\nX: {spawncoordx}\nY: {spawncoordy}\nZ: {spawncoordz}\n ");
                                Program.main.CoordX.Text = $"{spawncoordx}"; Program.main.CoordY.Text = $"{spawncoordy}"; Program.main.CoordZ.Text = $"{spawncoordz}";
                            }
                        }
                        if (bonus)
                        {
                            Program.main.MassiveDisplay.AppendText($"group {i} has a bonus round.\n\n");
                            var bonusseed = respawnrng.Next() - 0x82A2B175229D6A5B & 0xFFFFFFFFFFFFFFFF;
                            mainrng = new Xoroshiro128Plus(bonusseed);
                            for (int h = 0; h < 4; h++)
                            {
                                var generatorseed = mainrng.Next();
                                mainrng.Next();
                                var fixedrng = new Xoroshiro128Plus(generatorseed);
                                fixedrng.Next();
                                var fixedseed = fixedrng.Next();
                                mainrng.Next();
                                (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, 13, 3);
                                if (shiny)
                                {
                                    Program.main.MassiveDisplay.AppendText($"Initial Bonus Spawn {h}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generatorseed)}\n\n");
                                    Program.main.Teleporterdisplay.AppendText($"Map: {map.Replace("in the ", "")}\nInitial Bonus Spawn {h}\nSpecies: {(Species)species}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generatorseed)}\n\n");
                                    Program.main.Teleporterdisplay.AppendText($"Coords:\nX: {spawncoordx}\nY: {spawncoordy}\nZ: {spawncoordz}\n ");
                                    Program.main.CoordX.Text = $"{spawncoordx}"; Program.main.CoordY.Text = $"{spawncoordy}"; Program.main.CoordZ.Text = $"{spawncoordz}";
                                }
                            }
                            bonusseed = mainrng.Next();
                            mainrng = new Xoroshiro128Plus(bonusseed);
                            var bonusrng = new Xoroshiro128Plus(bonusseed);
                            for (int p = 1; p < bonuscount - 3; p++)
                            {
                                var generator_seed = bonusrng.Next();
                                bonusrng.Next();
                                bonusrng = new Xoroshiro128Plus(respawnrng.Next());
                                var fixed_rng = new Xoroshiro128Plus(generator_seed);
                                fixed_rng.Next();
                                var fixed_seed = fixed_rng.Next();

                                (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixed_seed, 13, 0);
                                if (shiny)
                                {
                                    Program.main.MassiveDisplay.AppendText($"Bonus Respawn {p}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generator_seed)}\n\n");
                                    Program.main.Teleporterdisplay.AppendText($"Map: {map.Replace("in the ", "")}\nBonus Respawn {p}\nSpecies: {(Species)species}\nShiny:{shiny}\nEC:{string.Format("{0:X}", encryption_constant)}\nPID:{string.Format("{0:X}", pid)}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility:{ability}\nGender:{gender}\nNature{((Nature)nature)}\nShinySeed{string.Format("0x{0:X}", generator_seed)}\n\n");
                                    Program.main.Teleporterdisplay.AppendText($"Coords:\nX: {spawncoordx}\nY: {spawncoordy}\nZ: {spawncoordz}\n ");
                                    Program.main.CoordX.Text = $"{spawncoordx}"; Program.main.CoordY.Text = $"{spawncoordy}"; Program.main.CoordZ.Text = $"{spawncoordz}";
                                }
                            }
                           
                        }

                    }

                }
            }
        }

       
      
      
       

        public (Species,bool) getspecies(SpawnerMMO[]encounters,double encounter_slot)
        {
            
            Species slot;
            int encsum = 0;
            bool alpha = false;
            foreach(var species in encounters)
            {
                encsum += species.Slot;
                if(encounter_slot < encsum)
                {
                    alpha = species.Alpha;
                    slot = species.Name;
                    return (slot,alpha);
                }
            }
            return (Species.None, false);
        }
        public (SpawnerMMO[], int) GetEncountersum(int groupid, bool bonus, int map)
        {
            var pointer = new long[5];
            if (bonus)
            {
                pointer = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x1d4 + (groupid * 0x90) + (0xb80 * map) + 0x2c };
            }
            else
                pointer = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x1d4 + (groupid * 0x90) + (0xb80 * map) + 0x24 };
            ulong pointeroff;
            string? enclong;
            if (!Main.USB)
            {
                pointeroff = Main.routes.PointerAll(pointer).Result;
                enclong = "0x" + LittleEndian(BitConverter.ToString(Main.routes.ReadBytesAbsoluteAsync(pointeroff, 8).Result, 0).ToString().ToUpper().Replace("-", ""));
            }
            else
            {
                pointeroff = Main.usbroutes.PointerAll(pointer).Result;
                enclong = "0x" + LittleEndian(BitConverter.ToString(Main.usbroutes.ReadBytesAbsoluteAsync(pointeroff, 8).Result, 0).ToString().ToUpper().Replace("-", ""));
            }
            
            var MMOSpawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/zyro670/JS-Finder/notabranch/Resources/pla_spawners/jsons/massivemassoutbreaks.json");
            mmoslots = JsonConvert.DeserializeObject<Dictionary<string, SpawnerMMO[]>>(MMOSpawnersjson);
            int encmax = 0;
            try
            {
                var encounters = mmoslots[enclong];
                foreach (var keyValuePair in mmoslots)
                {
                    if (keyValuePair.Key == enclong)
                    {

                        foreach (var keyValue in keyValuePair.Value)
                        {
                            encmax += keyValue.Slot;

                        }
                    }
                }
                return (encounters, encmax);
            }
            catch { return (null, 0); }
        }
        public ulong getbonusseed(int groupid,int rolls, int map,int[] paths)
        {
            var outbreakpointer = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x1d4 + (groupid * 0x90) + (0xB80 * map) };
            ulong Outbreakoff;
            int species;
            if (!Main.USB)
            {
                Outbreakoff = Main.routes.PointerAll(outbreakpointer).Result;
                species = BitConverter.ToUInt16(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff, 2).Result, 0);
            }
            else
            {
                Outbreakoff = Main.usbroutes.PointerAll(outbreakpointer).Result;
                species = BitConverter.ToUInt16(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff, 2).Result, 0);
            }
            if (species != 0)
            {
                if (species == 201)
                    rolls = 19;
                ulong groupseed;
                int maxspawns;
                if (!Main.USB)
                {
                    groupseed = BitConverter.ToUInt64(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x44, 8).Result, 0);
                    maxspawns = BitConverter.ToInt32(Main.routes.ReadBytesAbsoluteAsync(Outbreakoff + 0x4c, 4).Result, 0);
                }
                else
                {
                    groupseed = BitConverter.ToUInt64(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x44, 8).Result, 0);
                    maxspawns = BitConverter.ToInt32(Main.usbroutes.ReadBytesAbsoluteAsync(Outbreakoff + 0x4c, 4).Result, 0);
                }
                var mainrng = new Xoroshiro128Plus(groupseed);
                for(int b = 0; b < 4; b++)
                {
                    var generatorseed = mainrng.Next();
                    mainrng.Next();
                    var fixedrng = new Xoroshiro128Plus(generatorseed);
                    fixedrng.Next();
                    var fixedseed = fixedrng.Next();
                }
                groupseed = mainrng.Next();
                mainrng = new Xoroshiro128Plus(groupseed);
                var respawnrng = new Xoroshiro128Plus(groupseed);
                ulong generatorseeds;
                foreach(var path in paths)
                {
                    for(int pokemon = 0; pokemon < path; pokemon++)
                    {
                        generatorseeds = respawnrng.Next();
                        var tempseed = respawnrng.Next();
                        var fixerng = new Xoroshiro128Plus(generatorseeds);
                        fixerng.Next();
                        var fixerseed = fixerng.Next();
                        fixerseed = fixerng.Next();
                    }
                    respawnrng.Next();
                }
                var bonusseed = (respawnrng.Next() - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                return bonusseed;

            }
            return 0x0;
        }
        public class SpawnerMMO
        {
            public int Slot { get; set; }
            public Species Name { get; set; }

            public int Form { get; set; }
            public bool Alpha { get; set; }
            public int[] Levels { get; set; } = Array.Empty<int>();
            public int IVs { get; set; }
        }
        public Dictionary<string, SpawnerMMO[]> mmoslots { get; set; } = new Dictionary<string, SpawnerMMO[]>();

        static string LittleEndian(string num)
        {
            ulong number = Convert.ToUInt64(num,16);
            byte[] bytes = BitConverter.GetBytes(number);
            string retval = "";
            foreach (byte b in bytes)
                retval += b.ToString("X2");
            return retval;
        }

    }
}
