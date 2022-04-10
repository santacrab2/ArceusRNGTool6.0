using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using SysBot.Base;
using PermuteMMO.Lib;
using EtumrepMMO.Lib;
using PKHeX.Core;


namespace PLARNGGui
{
    public partial class Main : Form
    {
        public static BotBaseRoutines routes = new BotBaseRoutines();
        public static USBBaseRoutines usbroutes = new();
        public static RngRoutines rngroutes = new RngRoutines();
        public static OutbreakRoutines outbreakroutes = new OutbreakRoutines();
        public static MMORoutines mmoroutes = new MMORoutines();
        public static DistortionRoutines disroutes = new DistortionRoutines();
        public static bool USB = false;
        public Main()
        {
            InitializeComponent();
            MapSelection.DataSource = Enum.GetValues(typeof(Enums.Maps));
            weatherselection.DataSource = Enum.GetValues(typeof(Enums.Weather));
            Timeofdayselection.DataSource = Enum.GetValues(typeof(Enums.Time));
            outbreakmap.DataSource = Enum.GetValues(typeof(Enums.Maps));
            aggrpathsearchsettings.DataSource = Enum.GetValues(typeof(Enums.PathSearchSettings));
            distortionmap.DataSource = Enum.GetValues(typeof(Enums.Maps));
            outbreakpathsettings.DataSource = Enum.GetValues(typeof(Enums.PathSearchSettings));
            speciespath.DataSource = Enum.GetValues(typeof(Species));
            nocfwpathsearchsettings.DataSource = Enum.GetValues(typeof(Enums.PathSearchSettings));
            connecttype.DataSource = Enum.GetValues(typeof(Enums.ConnectionType));
          
            


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void MapSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            standardgroupid.Items.Clear();
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            foreach(var key in spawnmap)
            {
                Program.main.standardgroupid.Items.Add(key.Key);
            }
            standardgroupid.SelectedIndex = 0;
            
        }

        private void weatherselection_SelectedIndexChanged(object sender, EventArgs e)
        {
            pokemonselection.Items.Clear();
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            var groupname = spawnmap[$"{Program.main.standardgroupid.SelectedItem}"]["name"];
            var possiblespawns = encmap[$"{groupname}"][$"{((Enums.Time)Program.main.Timeofdayselection.SelectedItem == Enums.Time.Any ? "Any Time" : Program.main.Timeofdayselection.SelectedItem)}" + "/" + $"{((Enums.Weather)Program.main.weatherselection.SelectedItem == Enums.Weather.All ? "All Weather" : Program.main.weatherselection.SelectedItem)}"];
            try
            {
                foreach (var key in possiblespawns)
                {
                    pokemonselection.Items.Add($"{key}".Remove($"{key}".IndexOf(":")).Replace("\"", ""));
                }
            }
            catch { pokemonselection.Items.Add("(None)"); }
        }

        private void Timeofdayselection_SelectedIndexChanged(object sender, EventArgs e)
        {
            pokemonselection.Items.Clear();
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            var groupname = spawnmap[$"{Program.main.standardgroupid.SelectedItem}"]["name"];
            var possiblespawns = encmap[$"{groupname}"][$"{((Enums.Time)Program.main.Timeofdayselection.SelectedItem == Enums.Time.Any ? "Any Time" : Program.main.Timeofdayselection.SelectedItem)}" + "/" + $"{((Enums.Weather)Program.main.weatherselection.SelectedItem == Enums.Weather.All ? "All Weather" : Program.main.weatherselection.SelectedItem)}"];
            try
            {
                foreach (var key in possiblespawns)
                {
                    pokemonselection.Items.Add($"{key}".Remove($"{key}".IndexOf(":")).Replace("\"", ""));
                }
            }
            catch { pokemonselection.Items.Add("(None)"); }
        }

        private void pokemonselection_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupid_TextChanged(object sender, EventArgs e)
        {
            try
            {
                pokemonselection.Items.Clear();
                var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
                var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
                var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
                var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
                var groupname = spawnmap[$"{Program.main.standardgroupid.SelectedItem}"]["name"];
                var possiblespawns = encmap[$"{groupname}"][$"{((Enums.Time)Program.main.Timeofdayselection.SelectedItem == Enums.Time.Any ? "Any Time" : Program.main.Timeofdayselection.SelectedItem)}" + "/" + $"{((Enums.Weather)Program.main.weatherselection.SelectedItem == Enums.Weather.All ? "All Weather" : Program.main.weatherselection.SelectedItem)}"];
                try
                {
                    foreach (var key in possiblespawns)
                    {
                        pokemonselection.Items.Add($"{key}".Remove($"{key}".IndexOf(":")).Replace("\"", ""));
                    }
                }
                catch { pokemonselection.Items.Add("(None)"); }
            }
            catch { }
        }
       public static Socket Connection = new Socket(SocketType.Stream, ProtocolType.Tcp);
        private async void connect_Click(object sender, EventArgs e)
        {
            if ((Enums.ConnectionType)Program.main.connecttype.SelectedItem == Enums.ConnectionType.WiFi)
            {
                Connection.Connect(Program.main.IP.Text, 6000);
                if (Connection.Connected)
                {
                    var softbanptr = new long[] { 0x42BA6B0, 0x268, 0x70 };
                    var softbanoff = routes.PointerAll(softbanptr).Result;
                    var softbanval = routes.ReadBytesAbsoluteAsync(softbanoff, 4).Result;
                    if (BitConverter.ToUInt32(softbanval) != 0)
                    {
                        Program.main.StandardSpawnsDisplay.AppendText("Soft ban detected, unbanning.");
                        Program.main.OutbreakDisplay.AppendText("Soft ban detected, unbanning.");
                        Program.main.MassiveDisplay.AppendText("Soft ban detected, unbanning.");
                        Program.main.Distortiondisplay.AppendText("Soft ban detected, unbanning.");
                        Program.main.Teleporterdisplay.AppendText("Soft ban detected, unbanning.");
                        // Write the value to 0.
                        var data = BitConverter.GetBytes(0);
                        await routes.PointerPoke(data, softbanptr);
                    }
                    Program.main.StandardSpawnsDisplay.AppendText("connected to switch\n");
                    Program.main.OutbreakDisplay.AppendText("connected to switch\n");
                    Program.main.MassiveDisplay.AppendText("connected to switch\n");
                    Program.main.Distortiondisplay.AppendText("connected to switch\n");
                    Program.main.Teleporterdisplay.AppendText("connected to switch\n");
                }
                USB = false;
            }
            else
            {
                usbroutes.Connect(Program.main.IP.Text);
                var softbanptr = new long[] { 0x42BA6B0, 0x268, 0x70 };
                var softbanoff = usbroutes.PointerAll(softbanptr).Result;
                var softbanval = usbroutes.ReadBytesAbsoluteAsync(softbanoff, 4).Result;
                if (BitConverter.ToUInt32(softbanval) != 0)
                {
                    Program.main.StandardSpawnsDisplay.AppendText("Soft ban detected, unbanning.");
                    Program.main.OutbreakDisplay.AppendText("Soft ban detected, unbanning.");
                    Program.main.MassiveDisplay.AppendText("Soft ban detected, unbanning.");
                    Program.main.Distortiondisplay.AppendText("Soft ban detected, unbanning.");
                    Program.main.Teleporterdisplay.AppendText("Soft ban detected, unbanning.");
                    // Write the value to 0.
                    var data = BitConverter.GetBytes(0);
                    await usbroutes.PointerPoke(data, softbanptr);
                }
                Program.main.StandardSpawnsDisplay.AppendText("connected to switch\n");
                Program.main.OutbreakDisplay.AppendText("connected to switch\n");
                Program.main.MassiveDisplay.AppendText("connected to switch\n");
                Program.main.Distortiondisplay.AppendText("connected to switch\n");
                Program.main.Teleporterdisplay.AppendText("connected to switch\n");
                USB = true;

            }
           
        }

        private void calculatebutton_Click(object sender, EventArgs e)
        {
            ulong SpawnerOff;
            byte[] GeneratorSeed;
            var groupid = Convert.ToUInt32(Program.main.standardgroupid.SelectedItem);
            var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + groupid * 0x440 + 0x20 };
            if (!USB)
            {
                SpawnerOff = routes.PointerAll(SpawnerOffpoint).Result;
                GeneratorSeed = routes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
            }
            else
            {
                SpawnerOff = usbroutes.PointerAll(SpawnerOffpoint).Result;
                GeneratorSeed = usbroutes.ReadBytesAbsoluteAsync(SpawnerOff, 8).Result;
            }
            Program.main.StandardSpawnsDisplay.AppendText($"Generator Seed: {BitConverter.ToString(GeneratorSeed).Replace("-", "")}\n");
            var group_seed = (BitConverter.ToUInt64(GeneratorSeed, 0) - 0x82A2B175229D6A5B) & 0xFFFFFFFFFFFFFFFF;
            Program.main.StandardSpawnsDisplay.AppendText($"Group Seed: {string.Format("0x{0:X}", group_seed)}\n");
            
            var injectionseed = rngroutes.GenerateNextStandardMatch(group_seed);
            
        }

        private async void Inject_Click(object sender, EventArgs e)
        {
            var groupid = Convert.ToUInt32(Program.main.standardgroupid.SelectedItem);
            var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + groupid * 0x440 + 0x20 };
            if (!USB)
            {
                var SpawnerOff = routes.PointerAll(SpawnerOffpoint).Result;
                var seedlong = Convert.ToUInt64(Program.main.SeedToInject.Text, 16);
                await routes.WriteBytesAbsoluteAsync(BitConverter.GetBytes(Convert.ToUInt64(Program.main.SeedToInject.Text, 16)), SpawnerOff);
            }
            else
            {

                var SpawnerOff = usbroutes.PointerAll(SpawnerOffpoint).Result;
                var seedlong = Convert.ToUInt64(Program.main.SeedToInject.Text, 16);
                await usbroutes.WriteBytesAbsoluteAsync(BitConverter.GetBytes(Convert.ToUInt64(Program.main.SeedToInject.Text, 16)), SpawnerOff);
            }
            Program.main.StandardSpawnsDisplay.AppendText("Injecting: " + string.Format("{0:X}", Program.main.SeedToInject.Text)+"\n");
        }

        private async void Disconnect_Click(object sender, EventArgs e)
        {
            if (!USB)
            {
                await routes.DetachController();
                Connection.Close();
                Connection = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            else
                usbroutes.Disconnect();
            Program.main.StandardSpawnsDisplay.AppendText("Disconnected from switch\n");
            Program.main.OutbreakDisplay.AppendText("Disconnected from switch\n");
            Program.main.MassiveDisplay.AppendText("Disconnected from switch\n");
            Program.main.Distortiondisplay.AppendText("Disconnected from switch\n");
            Program.main.Teleporterdisplay.AppendText("Disconnected from switch\n");
            USB = false;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void outbreakread_Click(object sender, EventArgs e)
        {
            if(!inmapbox.Checked)
                Program.main.Teleporterdisplay.Clear();
            Program.main.OutbreakDisplay.Clear();
            if(inmapbox.Checked)
                outbreakroutes.ReadOutbreakID();
            else
            {
                var outbreakptr = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x20 };
                if (!USB)
                {
                    var outbreakoff = routes.PointerAll(outbreakptr).Result;
                    var outbreakblock = routes.ReadBytesAbsoluteAsync(outbreakoff, 0x190).Result;
                    outbreakroutes.ReadOutbreakJubilife(outbreakblock);
                }
                else
                {
                    var outbreakoff = usbroutes.PointerAll(outbreakptr).Result;
                    var outbreakblock = usbroutes.ReadBytesAbsoluteAsync(outbreakoff, 0x190).Result;
                    outbreakroutes.ReadOutbreakJubilife(outbreakblock);
                }
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void outbreakcalculate_Click(object sender, EventArgs e)
        {
            outbreakroutes.GenerateNextOutbreakMatch();
        }

        private async void outbreakinject_Click(object sender, EventArgs e)
        {
            var groupid = Convert.ToUInt32(Program.main.outbreakgroupid.Text);
            var SpawnerOffpoint = new long[] { 0x42a6ee0, 0x330, 0x70 + groupid * 0x440 + 0x20 };
            if (!USB)
            {
                var SpawnerOff = routes.PointerAll(SpawnerOffpoint).Result;
                var seedlong = Convert.ToUInt64(Program.main.SeedToInject.Text, 16);
                await routes.WriteBytesAbsoluteAsync(BitConverter.GetBytes(Convert.ToUInt64(Program.main.outbreakseedtoinject.Text, 16)), SpawnerOff);
            }
            else
            {
                var SpawnerOff = usbroutes.PointerAll(SpawnerOffpoint).Result;
                var seedlong = Convert.ToUInt64(Program.main.SeedToInject.Text, 16);
                await usbroutes.WriteBytesAbsoluteAsync(BitConverter.GetBytes(Convert.ToUInt64(Program.main.outbreakseedtoinject.Text, 16)), SpawnerOff);
            }
            Program.main.OutbreakDisplay.AppendText("Injecting: " + string.Format("{0:X}", Program.main.outbreakseedtoinject.Text) + "\n");
        }

        private void MassiveRead_Click(object sender, EventArgs e)
        {
            Program.main.MassiveDisplay.Clear();
            Program.main.Teleporterdisplay.Clear();
            mmoroutes.ReadMassOutbreak();
        }

        private void label18_Click(object sender, EventArgs e)
        {

        }


        private void Main_Load(object sender, EventArgs e)
        {

        }

        private async void teleportbutton_Click(object sender, EventArgs e)
        {
            var playerlocationptr = new long[] { 0x42D4720, 0x18, 0x48, 0x1F0, 0x18, 0x370, 0x90 };
            if (!USB)
            {
                var playerlocationoff = routes.PointerAll(playerlocationptr).Result;
                var doubleCoordX = float.Parse(Program.main.CoordX.Text);

                byte[] X = BitConverter.GetBytes(doubleCoordX);
                var doubleCoordY = float.Parse(Program.main.CoordY.Text);

                byte[] Y = BitConverter.GetBytes(doubleCoordY);
                var doubleCoordZ = float.Parse(Program.main.CoordZ.Text);

                byte[] Z = BitConverter.GetBytes(doubleCoordZ);
                var XYZ = X.Concat(Y).ToArray();
                XYZ = XYZ.Concat(Z).ToArray();
                await routes.WriteBytesAbsoluteAsync(XYZ, playerlocationoff);
            }
            else
            {
                var playerlocationoff = usbroutes.PointerAll(playerlocationptr).Result;
                var doubleCoordX = float.Parse(Program.main.CoordX.Text);

                byte[] X = BitConverter.GetBytes(doubleCoordX);
                var doubleCoordY = float.Parse(Program.main.CoordY.Text);

                byte[] Y = BitConverter.GetBytes(doubleCoordY);
                var doubleCoordZ = float.Parse(Program.main.CoordZ.Text);

                byte[] Z = BitConverter.GetBytes(doubleCoordZ);
                var XYZ = X.Concat(Y).ToArray();
                XYZ = XYZ.Concat(Z).ToArray();
                await usbroutes.WriteBytesAbsoluteAsync(XYZ, playerlocationoff);
            }
        }

        private async void campreadbutton_Click(object sender, EventArgs e)
        {
            Single coordx;
            Single coordy;
            Single coordz;
            var playerlocptr = new long[] { 0x42D4720, 0x18, 0x48, 0x1F0, 0x18, 0x370, 0x90 };
            if (!USB)
            {
                var playerlocoff = routes.PointerAll(playerlocptr).Result;
                var playercoordarray = await routes.ReadBytesAbsoluteAsync(playerlocoff, 12);
                coordx = BitConverter.ToSingle(playercoordarray, 0);
                coordy = BitConverter.ToSingle(playercoordarray, 4);
                coordz = BitConverter.ToSingle(playercoordarray, 8);
            }
            else
            {
                var playerlocoff = usbroutes.PointerAll(playerlocptr).Result;
                var playercoordarray = await usbroutes.ReadBytesAbsoluteAsync(playerlocoff, 12);
                coordx = BitConverter.ToSingle(playercoordarray, 0);
                coordy = BitConverter.ToSingle(playercoordarray, 4);
                coordz = BitConverter.ToSingle(playercoordarray, 8);
            }
            Program.main.Campx.Text = $"{coordx}";
            Program.main.campy.Text = $"{coordy}";
            Program.main.campz.Text = $"{coordz}";
        }

        private async void campteleportbutton_Click(object sender, EventArgs e)
        {
            var playerlocationptr = new long[] { 0x42D4720, 0x18, 0x48, 0x1F0, 0x18, 0x370, 0x90 };
            if (!USB)
            {
                var playerlocationoff = routes.PointerAll(playerlocationptr).Result;
                var CoordX = float.Parse(Program.main.Campx.Text);
                byte[] X = BitConverter.GetBytes(CoordX);
                var CoordY = float.Parse(Program.main.campy.Text);
                byte[] Y = BitConverter.GetBytes(CoordY);
                var CoordZ = float.Parse(Program.main.campz.Text);
                byte[] Z = BitConverter.GetBytes(CoordZ);
                var XYZ = X.Concat(Y).ToArray();
                XYZ = XYZ.Concat(Z).ToArray();
                await routes.WriteBytesAbsoluteAsync(XYZ, playerlocationoff);
            }
            else
            {
                var playerlocationoff = usbroutes.PointerAll(playerlocationptr).Result;
                var CoordX = float.Parse(Program.main.Campx.Text);
                byte[] X = BitConverter.GetBytes(CoordX);
                var CoordY = float.Parse(Program.main.campy.Text);
                byte[] Y = BitConverter.GetBytes(CoordY);
                var CoordZ = float.Parse(Program.main.campz.Text);
                byte[] Z = BitConverter.GetBytes(CoordZ);
                var XYZ = X.Concat(Y).ToArray();
                XYZ = XYZ.Concat(Z).ToArray();
                await usbroutes.WriteBytesAbsoluteAsync(XYZ, playerlocationoff);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void standardgroupid_SelectedIndexChanged(object sender, EventArgs e)
        {
            pokemonselection.Items.Clear();
            var Spawnersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/Spawners/{Program.main.MapSelection.SelectedItem}.json");
            var Encountersjson = new WebClient().DownloadString($"https://raw.githubusercontent.com/santacrab2/SysBot.NET/RNGstuff/encounters/{Program.main.MapSelection.SelectedItem}.json");
            var encmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Encountersjson);
            var spawnmap = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(Spawnersjson);
            var groupname = spawnmap[$"{Program.main.standardgroupid.SelectedItem}"]["name"];
            var possiblespawns = encmap[$"{groupname}"][$"{((Enums.Time)Program.main.Timeofdayselection.SelectedItem == Enums.Time.Any ? "Any Time" : Program.main.Timeofdayselection.SelectedItem)}" + "/" + $"{((Enums.Weather)Program.main.weatherselection.SelectedItem == Enums.Weather.All ? "All Weather" : Program.main.weatherselection.SelectedItem)}"];
            try
            {
                foreach (var key in possiblespawns)
                {
                    pokemonselection.Items.Add($"{key}".Remove($"{key}".IndexOf(":")).Replace("\"", ""));
                }
            }
            catch { pokemonselection.Items.Add("(None)"); }
        }

        private void checknear_Click(object sender, EventArgs e)
        {
            Program.main.StandardSpawnsDisplay.Clear();
            Program.main.Teleporterdisplay.Clear();
            Program.main.StandardSpawnsDisplay.AppendText("Searching All Group ID's for Shinies!\n");
            rngroutes.StandardCheckNear();
            
        }

        private void readalldisbutton_Click(object sender, EventArgs e)
        {
            Program.main.Distortiondisplay.Clear();
            Program.main.Distortiondisplay.AppendText("Searching All possible Distortions for Shinies...\n");
            disroutes.DistortionReader();
        }

        private void createdis_Click(object sender, EventArgs e)
        {
            Program.main.Distortiondisplay.AppendText("Creating a Distortion!");
            disroutes.DistortionMaker();
        }

        private async void pastureteleport_Click(object sender, EventArgs e)
        {
            var playerlocationptr = new long[] { 0x42D4720, 0x18, 0x48, 0x1F0, 0x18, 0x370, 0x90 };
            if (!USB)
            {
                var playerlocationoff = routes.PointerAll(playerlocationptr).Result;
                var CoordX = float.Parse("535.9406");
                byte[] X = BitConverter.GetBytes(CoordX);
                var CoordY = float.Parse("60.40232");
                byte[] Y = BitConverter.GetBytes(CoordY);
                var CoordZ = float.Parse("490.6487");
                byte[] Z = BitConverter.GetBytes(CoordZ);
                var XYZ = X.Concat(Y).ToArray();
                XYZ = XYZ.Concat(Z).ToArray();
                await routes.WriteBytesAbsoluteAsync(XYZ, playerlocationoff);
            }
            else
            {
                var playerlocationoff = usbroutes.PointerAll(playerlocationptr).Result;
                var CoordX = float.Parse("535.9406");
                byte[] X = BitConverter.GetBytes(CoordX);
                var CoordY = float.Parse("60.40232");
                byte[] Y = BitConverter.GetBytes(CoordY);
                var CoordZ = float.Parse("490.6487");
                byte[] Z = BitConverter.GetBytes(CoordZ);
                var XYZ = X.Concat(Y).ToArray();
                XYZ = XYZ.Concat(Z).ToArray();
                await usbroutes.WriteBytesAbsoluteAsync(XYZ, playerlocationoff);
            }
            
        }

        private void aggrpathbutton_Click(object sender, EventArgs e)
        {
            ulong mmooff;
            byte[] mmoblock;
            Program.main.Teleporterdisplay.Clear();
            Program.main.MassiveDisplay.Clear();
            var mmoptr = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x1b0 };
            if (!USB)
            {
                mmooff = routes.PointerAll(mmoptr).Result;
                mmoblock = routes.ReadBytesAbsoluteAsync(mmooff, 0x3980).Result;
            }
            else
            {
                mmooff = usbroutes.PointerAll(mmoptr).Result;
                mmoblock = usbroutes.ReadBytesAbsoluteAsync(mmooff, 0x3980).Result;
            }
            PermuteMeta.SatisfyCriteria = (results, advances) => (results.IsAlpha || results.IsShiny);
            if ((Enums.PathSearchSettings)Program.main.aggrpathsearchsettings.SelectedItem == Enums.PathSearchSettings.ShinyandAlpha)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsAlpha && results.IsShiny;
            if ((Enums.PathSearchSettings)Program.main.aggrpathsearchsettings.SelectedItem == Enums.PathSearchSettings.ShinyOnly)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsShiny;
            if ((Enums.PathSearchSettings)Program.main.aggrpathsearchsettings.SelectedItem == Enums.PathSearchSettings.AlphaOnly)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsAlpha; 
            ConsolePermuter.PermuteMassiveMassOutbreak(mmoblock);
        }

        private void outbreakpaths_Click(object sender, EventArgs e)
        {
            ulong outbreakoff;
            byte[] outbreakblock;
            Program.main.OutbreakDisplay.Clear();
            var outbreakptr = new long[] { 0x42BA6B0, 0x2B0, 0x58, 0x18, 0x20 };
            if (!USB)
            {
                outbreakoff = routes.PointerAll(outbreakptr).Result;
                outbreakblock = routes.ReadBytesAbsoluteAsync(outbreakoff, 0x190).Result;
            }
            else
            {
                outbreakoff = usbroutes.PointerAll(outbreakptr).Result;
                outbreakblock = usbroutes.ReadBytesAbsoluteAsync(outbreakoff, 0x190).Result;
            }
            PermuteMeta.SatisfyCriteria = (results, advances) => (results.IsAlpha || results.IsShiny);
            if ((Enums.PathSearchSettings)Program.main.outbreakpathsettings.SelectedItem == Enums.PathSearchSettings.ShinyandAlpha)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsAlpha && results.IsShiny;
            if ((Enums.PathSearchSettings)Program.main.outbreakpathsettings.SelectedItem == Enums.PathSearchSettings.ShinyOnly)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsShiny;
            if ((Enums.PathSearchSettings)Program.main.outbreakpathsettings.SelectedItem == Enums.PathSearchSettings.AlphaOnly)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsAlpha;
            ConsolePermuter.PermuteBlockMassOutbreak(outbreakblock);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("explorer.exe","https://www.github.com/kwsch/EtumrepMMO/wiki");
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Program.main.etumrepdisplay.Clear();
            const string entityFolderName = "mons";
            var inputs = GroupSeedFinder.GetInputs(entityFolderName);
            if (inputs.Count < 2)
            {
               Program.main.etumrepdisplay.AppendText("Insufficient inputs found in folder. Needs to have two (2) or more dumped files.");
            }
            else if (inputs.Count > 4)
            {
                Program.main.etumrepdisplay.AppendText("Too many inputs found in folder. Needs to have only the first four (4) Pokémon.");
            }
            else
            {
                var result = GroupSeedFinder.FindSeed(inputs);
                if (result is default(ulong))
                {
                    Program.main.etumrepdisplay.AppendText($"No group seeds found with the input data. Double check your inputs (valid inputs: {inputs.Count}).");
                }
                else
                {
                    Program.main.etumrepdisplay.AppendText("Found seed!");
                    Program.main.etumrepdisplay.AppendText(string.Format("0x{0:X}",result));
                }
            }
        }

        private void nocfwpathfind_Click(object sender, EventArgs e)
        {
            Program.main.nocfwpathdisplay.Clear();

            var userinfo = new UserEnteredSpawnInfo() { Seed = Program.main.seedui.Text,Species = (ushort)(Species)Program.main.speciespath.SelectedItem,BaseCount = int.Parse(Program.main.basecountui.Text),BaseTable = Program.main.basetableui.Text,BonusCount = int.Parse(Program.main.bonuscountui.Text),BonusTable = Program.main.bonustableui.Text};
            var spawner = userinfo.GetSpawn();
            SpawnGenerator.MaxShinyRolls = spawner.Type is SpawnType.MMO ? 19 : 32;
            PermuteMeta.SatisfyCriteria = (results, advances) => (results.IsAlpha || results.IsShiny);
            if ((Enums.PathSearchSettings)Program.main.nocfwpathsearchsettings.SelectedItem == Enums.PathSearchSettings.ShinyandAlpha)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsAlpha && results.IsShiny;
            if ((Enums.PathSearchSettings)Program.main.nocfwpathsearchsettings.SelectedItem == Enums.PathSearchSettings.ShinyOnly)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsShiny;
            if ((Enums.PathSearchSettings)Program.main.nocfwpathsearchsettings.SelectedItem == Enums.PathSearchSettings.AlphaOnly)
                PermuteMeta.SatisfyCriteria = (results, advances) => results.IsAlpha;
            ConsolePermuter.PermuteSingle(spawner, userinfo.GetSeed(), userinfo.Species);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.LinkVisited = true;
            System.Diagnostics.Process.Start("explorer.exe", "https://gist.github.com/Lusamine/eaf012b35bfde9c15905c811d5d8fb5a");


        }
    }
}
