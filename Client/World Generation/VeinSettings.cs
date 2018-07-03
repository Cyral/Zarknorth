using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public class VeinSettings
    {
        public event VeinCreateEventHandler terrainCreate;
        public Tile Tile;
        public PerlinSettings Settings;
        public float Filter;
        public float MaxFilter;
        public PerlinNoise noise;

        public virtual void OnCreate(WorldGen wg, int x, int y, BiomeType biomeType)
        {
            if (terrainCreate != null) terrainCreate(this, wg, x, y, biomeType);
        }

        public VeinSettings(Tile t, PerlinSettings p, float f, VeinCreateEventHandler tc)
        {
            terrainCreate = tc;
            Filter = f;
            Settings = p;
            Tile = t;
            noise = new PerlinNoise(Game.level.Data.Seed + Game.level.worldGen.VeinList.Count() + 1);
        }
        public delegate void VeinCreateEventHandler(object o, WorldGen wg, int x, int y, BiomeType bt);
    }
}
