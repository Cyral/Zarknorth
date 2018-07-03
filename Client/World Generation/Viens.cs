using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    public partial class WorldGen
    {
        #region Main Methods
        private void CreateVeins()
        {
            //VeinList = VeinList.Reverse<VeinSettings>().ToList<VeinSettings>();
            for (int x = 1; x < level.Width; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Adding caves, rock and ores " + ((int)Math.Round(((float)x / level.Width) * 100)).ToString() + "%", (((float)x / level.Width) * .65f) + .1f);
                BiomeType currentBiome = CheckBiome(x);
                for (int y = 0; y < level.Height; y++)
			    {
			        for (int v = 0; v < VeinList.Count(); v++)
                        VeinList[v].OnCreate(this, x, y, currentBiome); 
                }
            }
        }
        private void AddVeins()
        {
            //Stone in dirt
            VeinList.Add(new VeinSettings(new Tile(Item.Stone) { Background = Item.StoneBG }, new PerlinSettings(5, .9f, .54f, 0.045f), .6f, StoneInDirt));
            //Ice in snow
            VeinList.Add(new VeinSettings(new Tile(Item.RoughIce) { Background = Item.RoughIceBG }, new PerlinSettings(2, 2.2f, .6f, .03f), .99f, IceInSnow));
            //Slush in snow
            VeinList.Add(new VeinSettings(new Tile(Item.Slush) { Background = Item.SnowBG }, new PerlinSettings(4, 1f, .6f, 0.045f), .6f, SlushInSnow));
            //Stone on Chaparal
            VeinList.Add(new VeinSettings(new Tile(Item.Stone) { Background = Item.StoneBG }, new PerlinSettings(5, 1.7f, .65f, 0.053f), .6f, StoneOnChaparal));
            //Mud in dirt
            VeinList.Add(new VeinSettings(new Tile(Item.Mud) { Background = Item.MudBG }, new PerlinSettings(5, 1.7f, .65f, 0.053f), .6f, MudInDirt));
            //Gravel
            VeinList.Add(new VeinSettings(new Tile(Item.Gravel) { Background = Item.Blank }, new PerlinSettings(4, .76f, .54f, 0.0445f), .6f, SandInWorld));
            //Sand
            VeinList.Add(new VeinSettings(new Tile(Item.Sand) { Background = Item.Blank }, new PerlinSettings(4, .7f, .52f, 0.043f), .6f, SandInWorld));
            //Mud
            VeinList.Add(new VeinSettings(new Tile(Item.Mud) { Background = Item.MudBG }, new PerlinSettings(4, .78f, .52f, 0.043f), .6f, SandInWorld));
            //Clay
            VeinList.Add(new VeinSettings(new Tile(Item.Clay) { Background = Item.ClayBG }, new PerlinSettings(4, .78f, .52f, 0.043f), .6f, SandInWorld));
         
            //Clay in desert
            VeinList.Add(new VeinSettings(new Tile(Item.Clay) { Background = Item.ClayBG }, new PerlinSettings(7, 1.5f, .7f, 0.05f), .6f, ClayInLushDesert));
            //Sandstone in desert
            VeinList.Add(new VeinSettings(new Tile(Item.SandStone) { Background = Item.SandStoneBG }, new PerlinSettings(7, 1.6f, .6f, 0.045f), .6f, SandStoneInDesert));
            //Dirt in stone
            VeinList.Add(new VeinSettings(new Tile(Item.Dirt) { Background = Item.DirtBG }, new PerlinSettings(7, 1.5f, .65f, 0.05f), .6f, DirtInStone));
            //Large Caves 1
            VeinList.Add(new VeinSettings(new Tile(Item.Blank), new PerlinSettings(3, 2.4f, .6f, .033f, 0, 1f,2.2f), .99f, LargeCaves));
            
            //Large Caves 3
           // VeinList.Add(new VeinSettings(new Tile(Item.Blank), new PerlinSettings(3, 2.4f, .6f, .033f, 0, 1f, 2.2f), .99f, LargeCaves));
            //Mini Caves
            VeinList.Add(new VeinSettings(new Tile(Item.Blank), new PerlinSettings(5, 2.1f, .5f, .03f, 0, 1f, 2f), .99f, MiniCaves));
            VeinList.Add(new VeinSettings(new Tile(Item.Blank), new PerlinSettings(4, 1f, .5f, .02f, 0, 1f, 2f), .99f, MiniCaves));
            //Copper Ore
            VeinList.Add(new VeinSettings(new Tile(Item.CopperOre) { Background = Item.StoneBG }, new PerlinSettings(2, .95f, .5f, .105f), .85f, CopperOre));
            //Copper Ore 2
            VeinList.Add(new VeinSettings(new Tile(Item.CopperOre) { Background = Item.StoneBG }, new PerlinSettings(2, .9f, .5f, .1f), .80f, CopperOre));

            //Iron Ore
            VeinList.Add(new VeinSettings(new Tile(Item.IronOre) { Background = Item.StoneBG }, new PerlinSettings(2, .90f, .5f, .105f), .85f, IronOre) { MaxFilter = .90f });
            //Iron Ore
            VeinList.Add(new VeinSettings(new Tile(Item.IronOre) { Background = Item.StoneBG }, new PerlinSettings(2, .90f, .5f, .105f), .80f, IronOre) { MaxFilter = .90f });
            //Coal Ore
            VeinList.Add(new VeinSettings(new Tile(Item.CoalOre) { Background = Item.StoneBG }, new PerlinSettings(2, 1f, .5f, .115f), .80f, CoalOre) { MaxFilter = .82f });
            //Quartz Ore
            VeinList.Add(new VeinSettings(new Tile(Item.QuartzOre) { Background = Item.StoneBG }, new PerlinSettings(2, 1f, .5f, .115f), .80f, CoalOre) { MaxFilter = .86f });
            //Silver Ore
            VeinList.Add(new VeinSettings(new Tile(Item.SilverOre) { Background = Item.StoneBG }, new PerlinSettings(2, .94f, .5f, .11f), .85f, IronOre) { MaxFilter = .95f });
            //Silver Ore
            VeinList.Add(new VeinSettings(new Tile(Item.SilverOre) { Background = Item.StoneBG }, new PerlinSettings(2, .94f, .5f, .11f), .85f, IronOre) { MaxFilter = .95f });
            //Gold Ore
            VeinList.Add(new VeinSettings(new Tile(Item.GoldOre) { Background = Item.StoneBG }, new PerlinSettings(2, .90f, .5f, .1f), .85f, IronOre) { MaxFilter = .95f });
            //Gold Ore
            VeinList.Add(new VeinSettings(new Tile(Item.GoldOre) { Background = Item.StoneBG }, new PerlinSettings(2, .90f, .5f, .1f), .85f, IronOre) { MaxFilter = .95f });

            //Ruby Ore
            VeinList.Add(new VeinSettings(new Tile(Item.RubyOre) { Background = Item.StoneBG }, new PerlinSettings(2, .87f, .51f, .11f), .80f, RareOre) { MaxFilter = .90f });
            //Diamond Ore
            VeinList.Add(new VeinSettings(new Tile(Item.DiamondOre) { Background = Item.StoneBG }, new PerlinSettings(2, .85f, .51f, .11f), .82f, RareOre) { MaxFilter = .92f });

            //Bottom Caves
            VeinList.Add(new VeinSettings(new Tile(Item.Blank), new PerlinSettings(4, 15f, .4f, .06f), .01f, BottomCaves));
            VeinList.Add(new VeinSettings(new Tile(Item.Blank), new PerlinSettings(4, 15f, .4f, .06f), .01f, BottomCaves));
        }
        void BottomCaves(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= level.Height - HeightMap[x] + 85 && y < wg.level.Height - 3)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = wg.Level.tiles[x, y].Background };
        }
        private void StoneOnChaparal(object o, WorldGen wg, int x, int y, BiomeType bt)
        {
            VeinSettings v = (VeinSettings)o;
            if (bt == BiomeType.Chaparral && y > wg.HeightMap[x] && y < wg.HeightMap[x] + bt.SecondaryLayer - 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        private void IceInSnow(object o, WorldGen wg, int x, int y, BiomeType bt)
        {
            VeinSettings v = (VeinSettings)o;
            if (bt == BiomeType.Taiga && y >= wg.HeightMap[x] && y < wg.HeightMap[x] + bt.SecondaryLayer - 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        private void SandInWorld(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            BlockItem prev = wg.Level.tiles[x, y].Background;
            VeinSettings v = (VeinSettings)o;
            if (y > wg.HeightMap[x] + 4 && y < level.Height)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background != Item.Blank ? v.Tile.Background : prev };
        }
        private void SlushInSnow(object o, WorldGen wg, int x, int y, BiomeType bt)
        {
            VeinSettings v = (VeinSettings)o;
            if (bt == BiomeType.Taiga && y >= wg.HeightMap[x] && y < wg.HeightMap[x] + bt.SecondaryLayer - 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        #endregion

        #region Helper Methods
        void RareOre(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] + b.SecondaryLayer && y < wg.Level.Height)
            {
                float value = ((float)(y - wg.HeightMap[x]) / ((float)wg.Level.Height - (wg.HeightMap[x])));
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > MathHelper.Lerp(v.MaxFilter, v.Filter, value))
                {
                    if (wg.Level.tiles[x, y].Foreground == Item.Stone || wg.Level.tiles[x, y].Foreground == Item.Dirt)
                        wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
                }
            }
        }
        void CoalOre(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] && y < wg.Level.Height)
            {
                float value = ((float)(y - wg.HeightMap[x]) / ((float)wg.Level.Height - (wg.HeightMap[x])));
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > MathHelper.Lerp(v.MaxFilter, v.Filter, value))
                {
                    if ( wg.Level.tiles[x, y].Foreground == b.Secondary.Foreground|| wg.Level.tiles[x, y].Foreground == b.Primary.Foreground)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
                }
            }
        }
        void IronOre(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] && y < wg.Level.Height)
            {
                float value  =  ((float)(y - wg.HeightMap[x]) / ((float)wg.Level.Height - (wg.HeightMap[x])));
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > MathHelper.Lerp(v.MaxFilter, v.Filter, value))
                {
                    if (wg.Level.tiles[x, y].Foreground == b.Secondary.Foreground || wg.Level.tiles[x, y].Foreground == b.Primary.Foreground)
                        wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
                }
            }
        }
        void CopperOre(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] && y < wg.Level.Height)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                {
                    if (wg.Level.tiles[x, y].Foreground == b.Secondary.Foreground || wg.Level.tiles[x, y].Foreground == b.Primary.Foreground)
                        wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
                }
        }
        void MiniCaves(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                if (b != BiomeType.Desert && WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = wg.Level.tiles[x, y].Background };
        }
        void LargeCaves(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] + b.SecondaryLayer - 10 && y < wg.level.Height)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = wg.Level.tiles[x, y].Background } ;
        }
        void DirtInStone(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (y >= wg.HeightMap[x] + b.SecondaryLayer - 10 && y < wg.level.Height)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }

        void StoneInDirt(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            if (b != BiomeType.Taiga)
            {
                VeinSettings v = (VeinSettings)o;
                if (b == BiomeType.LushDesert || b == BiomeType.Desert || b == BiomeType.Marsh ? y >= wg.HeightMap[x] + wg.random.Next(7, 10) : y >= wg.HeightMap[x] + 1 && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                    if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                        wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
            }
        }


        void SandInDirt(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (b == BiomeType.Desert && y > wg.HeightMap[x] && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        void MudInDirt(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if ((b == BiomeType.Marsh || b == BiomeType.Fen) && y > wg.HeightMap[x] && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        void ClayInLushDesert(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (b == BiomeType.LushDesert && y > wg.HeightMap[x] && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        void SandStoneInDesert(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (b == BiomeType.LushDesert || b == BiomeType.Desert && y >= wg.HeightMap[x] && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        void LushDesert(object o, WorldGen wg, int x, int y, BiomeType b)
        {
            VeinSettings v = (VeinSettings)o;
            if (b == BiomeType.LushDesert || b == BiomeType.Desert && y > wg.HeightMap[x] && y < wg.HeightMap[x] + b.SecondaryLayer + 10)
                if (WorldGen.OctaveGenerator(v.noise, x, y, v.Settings) > v.Filter)
                    wg.Level.tiles[x, y] = new Tile(v.Tile.Foreground) { Background = v.Tile.Background };
        }
        #endregion
    }
}
