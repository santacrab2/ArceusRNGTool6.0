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
    public class DistortionRoutines
    {
        public void DistortionReader()
        {
            int encounter_slot_sum = 0;
            int count = 0;
            long[] disptr = new long[0];
            var mode = (Enums.Maps)Program.main.distortionmap.SelectedItem;
            switch (mode)
            {
                case Enums.Maps.ObsidianFieldlands: count = 16;break;
                case Enums.Maps.CrimsonMirelands: count = 25; break;
                case Enums.Maps.CobaltCoastlands: count = 20; break;
                case Enums.Maps.CoronetHighlands: count = 20; break;
                case Enums.Maps.AlabasterIcelands: count = 24; break;
            }
            for(int i = 0; i < count; i++)
            {
                switch (mode)
                {
                    case Enums.Maps.ObsidianFieldlands: disptr = new long[] { 0x42CC4D8, 0xC0, 0x1C0, 0x990 + i * 0x8, 0x18, 0x430, 0xC0 }; encounter_slot_sum = 112; break;
                    case Enums.Maps.CrimsonMirelands: disptr = new long[] { 0x42CC4D8, 0xC0, 0x1C0, 0xC70 + i * 0x8, 0x18, 0x430, 0xC0 }; encounter_slot_sum = 276; break;
                    case Enums.Maps.CobaltCoastlands: disptr = new long[] { 0x42CC4D8, 0xC0, 0x1C0, 0xCC0 + i * 0x8, 0x18, 0x430, 0xC0 }; encounter_slot_sum = 163; break;
                    case Enums.Maps.CoronetHighlands: disptr = new long[] { 0x42CC4D8, 0xC0, 0x1C0, 0x818 + i * 0x8, 0x18, 0x430, 0xC0 }; encounter_slot_sum = 382; break;
                    case Enums.Maps.AlabasterIcelands: disptr = new long[] { 0x42CC4D8, 0xC0, 0x1C0, 0x948 + i * 0x8, 0x18, 0x430, 0xC0 }; encounter_slot_sum = 259; break;
                }
                var SpawnerOff = Main.routes.PointerAll(disptr).Result;
                var GeneratorSeed = Main.routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
                
                var group_seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
                if(group_seed != 0)
                {
                    if (i >= 13 && i <= 15 && (Enums.Maps)Program.main.distortionmap.SelectedItem == Enums.Maps.CrimsonMirelands)
                    {
                        encounter_slot_sum = 118;
                    }
                    var groupseed = group_seed;
                    var mainrng = new Xoroshiro128Plus(groupseed);
                    var generator_seed = mainrng.Next();
                    var rng = new Xoroshiro128Plus(generator_seed);
                    var encounter_slot = rng.Next() / Math.Pow(2, 64) * encounter_slot_sum;
                    var fixedseed = rng.Next();
                    var (shiny, encryption_constant, pid, ivs, ability, gender, nature, shinyseed) = Main.rngroutes.GenerateFromSeed(fixedseed, Convert.ToInt32(Program.main.DistortionSRs.Text), 0);
                    var (species, alpha) = GetDistortionSpecies(encounter_slot);
                    var speclocation = GetDistortionSpeciesLocation(i);
                    if (i == 0 || i == 4 || i == 8 || i == 12 || i == 16 || i == 20)
                    {

                        continue;
                    }
                    if (i >= 9 && i <= 12 && (Enums.Maps)Program.main.distortionmap.SelectedItem == Enums.Maps.CrimsonMirelands)
                    {

                        continue;
                    }
                    if (shiny)
                    {
                        Program.main.Distortiondisplay.AppendText($"location: {speclocation}\nSpecies: {species}\nShiny: {shiny}\nAlpha: {alpha}\nEC: {encryption_constant}\nPID: {pid}\nIVs:{ivs[0]}/{ivs[1]}/{ivs[2]}/{ivs[3]}/{ivs[4]}/{ivs[5]}\nAbility: {ability}\nGender: {gender}\nNature: {(Nature)nature}\nGenerator Seed: {generator_seed}\n\n");
                    }
                    else
                        Program.main.Distortiondisplay.AppendText($"No Shiny {species} Found at GroupID: {i}\nLocation: {speclocation}\n\n");
                    mainrng.Next();
                    mainrng.Next();
                    _ = new Xoroshiro128Plus(mainrng.Next());
                }
            }
        }
        public async void DistortionMaker()
        {
            uint ActivateDistortion = 0x024A03B8;
            var MainNsoBase = await Main.routes.GetMainNsoBaseAsync().ConfigureAwait(false);
            await Main.routes.WriteBytesAbsoluteAsync(BitConverter.GetBytes(0x5280010A), MainNsoBase + ActivateDistortion).ConfigureAwait(false);
            await Main.routes.WriteBytesAbsoluteAsync(BitConverter.GetBytes(0x7100052A), MainNsoBase + ActivateDistortion).ConfigureAwait(false);
        }
        public (string,bool) GetDistortionSpecies(double encslot)
        {
            var mode = (Enums.Maps)Program.main.distortionmap.SelectedItem;
            string species = "";
            bool alpha = false;
            switch (mode)
            {
                case Enums.Maps.ObsidianFieldlands:
                    {
                        if (encslot < 100) species = Species.Sneasel.ToString();
                        if (encslot > 100 && encslot < 101)
                        {
                            species = Species.Sneasel.ToString();
                            alpha = true;
                        }
                        if (encslot > 101 && encslot < 111) species = Species.Weavile.ToString();
                        if (encslot > 111 && encslot < 112)
                        {
                            species = Species.Weavile.ToString();
                            alpha = true;
                        }
                    }
                    break;
                case Enums.Maps.CrimsonMirelands:
                    {
                        if (encslot < 100) species = Species.Porygon.ToString();
                        if (encslot > 100 && encslot < 101)
                        {
                            species = Species.Porygon.ToString();
                            alpha = true;
                        }
                        if (encslot > 101 && encslot < 111) species = Species.Porygon2.ToString();
                        if (encslot > 111 && encslot < 112)
                        {
                            species = Species.Porygon2.ToString();
                            alpha = true;
                        }
                        if (encslot > 112 && encslot < 117) species = Species.PorygonZ.ToString();
                        if (encslot > 117 && encslot < 118)
                        {
                            species = Species.PorygonZ.ToString();
                            alpha = true;
                        }
                        if (encslot > 118 && encslot < 218) species = Species.Cyndaquil.ToString();
                        if (encslot > 218 && encslot < 219)
                        {
                            species = Species.Cyndaquil.ToString();
                            alpha = true;
                        }
                        if (encslot > 219 && encslot < 269) species = Species.Quilava.ToString();
                        if (encslot > 269 && encslot < 270)
                        {
                            species = Species.Quilava.ToString();
                            alpha = true;
                        }
                        if (encslot > 270 && encslot < 275) species = Species.Typhlosion.ToString();
                        if (encslot >= 275)
                        {
                            species = Species.Typhlosion.ToString();
                            alpha = true;
                        }
                    }
                    break;
                case Enums.Maps.CobaltCoastlands:
                    {
                        if (encslot < 100) species = Species.Magnemite.ToString();
                        if (encslot > 100 && encslot < 101)
                        {
                            species = Species.Magnemite.ToString();
                            alpha = true;
                        }
                        if (encslot > 101 && encslot < 151) species = Species.Magneton.ToString();
                        if (encslot > 151 && encslot < 152)
                        {
                            species = Species.Magneton.ToString();
                            alpha = true;
                        }
                        if (encslot > 152 && encslot < 162) species = Species.Magnezone.ToString();
                        if (encslot >= 162)
                        {
                            species = Species.Magnezone.ToString();
                            alpha = true;
                        }
                    }
                    break;
                case Enums.Maps.CoronetHighlands:
                    {
                        if (encslot < 100) species = Species.Cranidos.ToString();
                        if (encslot > 100 && encslot < 101)
                        {
                            species = Species.Cranidos.ToString();
                            alpha = true;
                        }
                        if (encslot > 101 && encslot < 111) species = Species.Rampardos.ToString();
                        if (encslot > 111 && encslot < 112)
                        {
                            species = Species.Rampardos.ToString();
                            alpha = true;
                        }
                        if (encslot > 112 && encslot < 212) species = Species.Shieldon.ToString();
                        if (encslot > 212 && encslot < 213)
                        {
                            species = Species.Shieldon.ToString();
                            alpha = true;
                        }
                        if (encslot > 213 && encslot < 223) species = Species.Bastiodon.ToString();
                        if (encslot > 223 && encslot < 224)
                        {
                            species = Species.Bastiodon.ToString();
                            alpha = true;
                        }
                        if (encslot > 224 && encslot < 324) species = Species.Rowlet.ToString();
                        if (encslot > 324 && encslot < 325)
                        {
                            species = Species.Rowlet.ToString();
                            alpha = true;
                        }
                        if (encslot > 325 && encslot < 375) species = Species.Dartrix.ToString();
                        if (encslot > 375 && encslot < 376)
                        {
                            species = Species.Dartrix.ToString();
                            alpha = true;
                        }
                        if (encslot > 376 && encslot < 381) species = Species.Decidueye.ToString();
                        if (encslot >= 381)
                        {
                            species = Species.Decidueye.ToString();
                            alpha = true;
                        }
                    }
                    break;
                case Enums.Maps.AlabasterIcelands:
                    {
                        if (encslot < 100) species = Species.Scizor.ToString();
                        if (encslot > 100 && encslot < 101)
                        {
                            species = Species.Scizor.ToString();
                            alpha = true;
                        }
                        if (encslot > 101 && encslot < 201) species = Species.Oshawott.ToString();
                        if (encslot > 201 && encslot < 202)
                        {
                            species = Species.Oshawott.ToString();
                            alpha = true;
                        }
                        if (encslot > 202 && encslot < 252) species = Species.Dewott.ToString();
                        if (encslot > 252 && encslot < 253)
                        {
                            species = Species.Dewott.ToString();
                            alpha = true;
                        }
                        if (encslot > 253 && encslot < 258) species = Species.Samurott.ToString();
                        if (encslot > 258)
                        {
                            species = Species.Samurott.ToString();
                            alpha = true;
                        }
                    }
                    break;
            }
            return (species,alpha);
            
        }
        public string GetDistortionSpeciesLocation(int id)
        {
            string location = string.Empty;
            var mode = (Enums.Maps)Program.main.distortionmap.SelectedItem;
            switch (mode)
            {
                case Enums.Maps.ObsidianFieldlands:
                    {
                        if (id <= 4) location = "Horseshoe Plains";
                        if (id > 4 && id <= 8) location = "Windswept Run";
                        if (id > 8 && id <= 12) location = "Nature's Pantry";
                        if (id > 12 && id <= 16) location = "Sandgem Flats";
                    }
                    break;
                case Enums.Maps.CrimsonMirelands:
                    {
                        if (id <= 4) location = "Droning Meadow";
                        if (id > 4 && id <= 8) location = "Holm of Trials";
                        if (id > 8 && id <= 12) location = "Unknown";
                        if (id > 12 && id <= 16) location = "Ursa's Ring";
                        if (id > 16 && id <= 20) location = "Prairie";
                        if (id > 20 && id <= 24) location = "Gapejaw Bog";
                    }
                    break;
                case Enums.Maps.CobaltCoastlands:
                    {
                        if (id <= 4) location = "Ginko Landing";
                        if (id > 4 && id <= 8) location = "Aipom Hill";
                        if (id > 8 && id <= 12) location = "Deadwood Haunt";
                        if (id > 12 && id <= 16) location = "Spring Path";
                        if (id > 16 && id <= 20) location = "Windbreak Stand";
                    }
                    break;
                case Enums.Maps.CoronetHighlands:
                    {
                        if (id <= 4) location = "Sonorous Path";
                        if (id > 4 && id <= 8) location = "Ancient Quarry";
                        if (id > 8 && id <= 12) location = "Celestica Ruins";
                        if (id > 12 && id <= 16) location = "Primeval Grotto";
                        if (id > 16 && id <= 20) location = "Boulderoll Ravine";
                    }
                    break;
                case Enums.Maps.AlabasterIcelands:
                    {
                        if (id <= 4) location = "Bonechill Wastes North";
                        if (id > 4 && id <= 8) location = "Avalugg's Legacy";
                        if (id > 8 && id <= 12) location = "Bonechill Wastes South";
                        if (id > 12 && id <= 16) location = "Southeast of Arena";
                        if (id > 16 && id <= 20) location = "Heart's Crag";
                        if (id > 20 && id <= 24) location = "Arena's Approach";
                    }
                    break;
            }
            return location;
        }
    }
}
