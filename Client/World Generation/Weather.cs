using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cyral.Extensions;

namespace ZarknorthClient
{
    public enum WeatherType
    {
         Normal,
         Rain,
         Storm,
         Snow,
    }
    public class WeatherChance
    {
        //Dictionary of all the chances
        private readonly Dictionary<WeatherType, double> chances = new Dictionary<WeatherType, double>();
        //The largest chance of all of them
        private double maxChance;
        //Dictionary of the chances in order
        private IOrderedEnumerable<KeyValuePair<WeatherType, double>> orderedChances;
        private static Random random = new Random();

        public WeatherChance(double normalChance, double rainChance, double stormChance, double snowChance)
        {
            if (normalChance + rainChance + stormChance + snowChance != 1)
                throw new ArgumentOutOfRangeException("Values of all chances must add up to 100% on a 0 to 1 scale");

            chances.Add(WeatherType.Storm, stormChance);
            chances.Add(WeatherType.Snow, snowChance);
            chances.Add(WeatherType.Normal, normalChance);
            chances.Add(WeatherType.Rain, rainChance);
            //Order the chances from greatest to least using LINQ
            orderedChances = from pair in chances
                        orderby pair.Value ascending
                        select pair;

            foreach (KeyValuePair<WeatherType, double> pair in orderedChances)
            {
                maxChance = pair.Value;
            }
        }


        public WeatherType GetWeather()
        {
            //Get a random value
            double r = random.NextDouble(0,maxChance);
            //Loop results
            foreach (KeyValuePair<WeatherType, double> pair in orderedChances)
            {
                if (r <= pair.Value)
                    return pair.Key;
            }
            return WeatherType.Normal;
        }
    }
}
