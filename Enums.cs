using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLARNGGui
{
    public class Enums
    {
        public enum Weather
        {
            All,
            None,
            Sunny,
            Cloudy,
            Rain,
            Snow,
            Drought,
            Fog,
            Rainstorm,
            Snowstorm

        }
        public enum Time
        {
            Any,
            Dawn,
            Day,
            Dusk,
            Night
        }
        public enum Maps
        {
            
            ObsidianFieldlands,
            CrimsonMirelands,
            CobaltCoastlands,
            CoronetHighlands,
            AlabasterIcelands
        }
        public enum MMOMapCount
        {
            
            one,
            two,
            three,
            four,
            five,
        }
        public enum PathSearchSettings
        {
            All,
            ShinyandAlpha,
            ShinyOnly,
            AlphaOnly
             
        }

    }
    
}
