using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public struct PerlinSettings
    {
        public int Octaves;
        public float Amplitude, Persistance, FrequencyX, FrequencyY, FrequencyMultiplierX, FrequencyMultiplierY;

        public PerlinSettings(int octaves, float amplitude, float persistance,float frequencyX, float frequencyY = 0, float freqMultiplierX = 1, float freqMultiplierY = 1)
        {
            Octaves = octaves;
            Amplitude = amplitude;
            Persistance = persistance;
            FrequencyX = frequencyX;
            FrequencyY = frequencyY;
            FrequencyMultiplierX = freqMultiplierX;
            FrequencyMultiplierY = freqMultiplierY;
        }
    }
}
