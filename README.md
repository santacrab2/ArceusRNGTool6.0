# ArceusRNGTool6.0

## Credits:
  A huge Thank you for all the hours upon hours of hardwork that Lincoln-LM, Capitalism(Cappy), Kurt and Many others who are constantly researching these games and
  developing tools for developers to develop tools for the users. This code is heavily based upon the previous mentioned users contributions to the community. I simply
  took their code, reworked it very very minorly and put it into a GUI. This is essentially just a collection of the current RNG scripts for Pokemon Legends Arceus. 
  ## More Credits:
  Kurt - For creating PKHeX, the Permutation script and many other projects<br>
  Anubis - ^ <br>
  Lincoln-LM For creating the original RNG scripts<br>
  Capitalism(Cappy) - For creating the Bonus round pathing script and general support understanding this stuff<br>
  Berichan - For their work on the Teleporting mechanisms.<br>
  Olizor - For their work on Sysbot.base and all the others who have helped develop it.<br>
  Koi - For their work on the USB-sybot-base and the code on how to connect to it<br>
  Zyro - For their work on the Distortion scripts and general support through out the entire coding process<br>
  The Villain a.k.a Kyle™ - for stress testing initial builds!<br>
  Any one else who has contributed in anyway to the RNG community.<br>
  
  # Usage:
## Standard Spawns:
   1. Set your map that you are currently located in, these scans have to be done from the map.
   2. Select your group id, and current weather/time that you have in your game.
   Check Near:
   1. Set your max advances 5-10 is usually good, any more can take a long time to advance.
   2. Click Check near and it will let you know if there are any shinies within the max advances you set at every single spawner
   at the map, this can take a few seconds.
   Calculate:
   1. Set all of the setings, including the species you are looking for and your shiny rolls
   2. Press Calculate and it will tell you how many advances required for a Shiny of the pokemon you are looking for.
   Inject:
   1. Set your group id and seed to inject which will be automatically filled after using calculate
   2. press inject, catch your shiny. 
![](https://raw.githubusercontent.com/santacrab2/ArceusRNGTool6.0/master/rngtoolscreens/standardchecknear.png)
## Outbreaks:
  Currently this can only be done while you are in the map. I do plan to adjust this is the near future.
  1. Go to the map of choice.
  2. Select the map you are in
  3. Press Read and it will read the current outbreak and all of the spawns, it will also fill in the group seed box, and the spawn count box.
  4. Set Max advances and hit calculate and it will tell you how many advances to find a shiny, as well as the seed for that shiny to appear in the initial spawns.
      - Click the Alpha Search if you would like an Alpha-Shiny to appear
  5. Either figure out a way to advance or click inject to write the seed it calculated.
![](https://raw.githubusercontent.com/santacrab2/ArceusRNGTool6.0/master/rngtoolscreens/outbreakreadcalc.png)
# Massive Outbreaks
  There are two modes with this, a read which just lets you know whats in the current spawners without any changes and an aggressive path finding tool.
  For Now, <i><b>All MMO Reads must be done from Jubilife</i></b>, this will change soon.
  1. To use the read, enter your shiny rolls, press read. 
  2. To use the Aggressive Path Finder
  3. Ensure you have your Save file from your game(your Main) in the same folder as the app
  so the app can determine your shiny rolls more accurately this way. Eventually Reads will include this.
  4. Set your search settings
     - All: includes both shinies and non-shiny alphas
     - ShinyandAlpha - only lists pokemon that are both alpha and shiny
     - Shiny Only
     - Alpha Only
  5. Press Aggr. Path Find button<br>
Aggressive Path Key:<br>
  A1-4 : Advance the number listed, so 1 is despawn 1, 2 is despawn in a 1v2, etc.<br>
  G1-3 : Despawn the number listed and then go to your camp and back to the Specific location.<br>
  SB : Start the Bonus Round, basically finish the first round off completely.<br>
 <b>The Read</b>
![](https://raw.githubusercontent.com/santacrab2/ArceusRNGTool6.0/master/rngtoolscreens/mmoread.png)
<b>The Path Find Search</b>
![](https://raw.githubusercontent.com/santacrab2/ArceusRNGTool6.0/master/rngtoolscreens/mmopathfind.png)

# Distortions
  This is very limited and currently just reads the Distortions that are loaded into your game, this only changes if you close
  and open your game. Read from the Map, it looks for specific high profile pokemon. 
  Create Distortion button will instantly create a distortion, don't spam this button as it may crash your game.
  
  ![](https://raw.githubusercontent.com/santacrab2/ArceusRNGTool6.0/master/rngtoolscreens/distortionread.png)
 
# Teleporter
  Shiny's that are found will be posted to this page along with its coordinates for easy copy and paste. Copy and Paste the x,y,z coordinates you want and press teleport. When you first load up a Map Press Camp Read to load the coordinates for the camp. You can also use this to find any coordinates anywhere in the game. Pasture Teleport only works when you are in Jubilife to make up for Heinous crime that it is not a quick travel spot. 
  
  ![](https://raw.githubusercontent.com/santacrab2/ArceusRNGTool6.0/master/rngtoolscreens/teleport.png)
  
  
  Any questions on this Repo after completely reading the Guide can be answered at Https://www.piplup.net 
