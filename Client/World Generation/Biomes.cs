using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace ZarknorthClient
{

    public class BiomeType
    {
         public static List<BiomeType> BiomeTypes;

        #region Properties
        /// <summary>
        /// Name of the BiomeType
        /// </summary>
        public string Name;

        /// <summary>
        /// Save ID
        /// </summary>
        public int ID;

        public PerlinSettings TerrainGenerator;
     
        /// <summary>
        /// What BiomeTypes CANNOT appear next to this BiomeType
        /// </summary>
        public BiomeType[] BlackList;
        /// <summary>
        /// 
        /// </summary>
        public float ChanceOfGenerating;
        /// <summary>
        /// Color the grass and foliage will be tinted
        /// </summary>
        public Color GrassColor;
        public Color WaterColor = new Color(60,230,255);
        public Dictionary<BlockItem, float> EnvironmentAddons = new Dictionary<BlockItem, float>();
        public float WaterPercent= 5f;
        public float LavaPercent = 0f;
        public int SecondaryLayer, PrimaryLayer;
        public Tile Primary, Secondary, Top;
        public int ParallaxLayers = 3, ParallaxVariation = 0;
        public event TerrainCreateEventHandler TerrainCreate;
        public virtual void OnTerrainCreate(WorldGen wg, int x, int y)
        {
            if (TerrainCreate != null) TerrainCreate(this, wg,x,y);
        }
        public bool IceOnWater;
        public WeatherChance Weather;
        public int MinWaterSearch = 15;
        public int MaxWaterSearch = 50;
        #endregion

        //Special, Undefined biome type used for the background of the level below a certain hight
        public static BiomeType Background;
        //Biomes... Lots of em.
        public static BiomeType Forest;
        public static BiomeType Desert;
        public static BiomeType Canyon;
        public static BiomeType Hell;
        public static BiomeType LushDesert;
        public static BiomeType Chaparral;
        public static BiomeType Plains;
        public static BiomeType Swamp;
        public static BiomeType FloatingIsland;
        public static BiomeType Jungle;
        public static BiomeType Marsh;
        public static BiomeType Mangrove;
        public static BiomeType Fen;
        public static BiomeType Grove;
        public static BiomeType Taiga, Savanna;

        public BiomeType(int id)
        {
            ID = id;
            TerrainCreate += CreateTerrain;

        }
     
        static BiomeType()
        {
           BiomeTypes = new List<BiomeType>();

           Background = new BiomeType(0); //Dont add the background biome to the list

           Forest = new BiomeType(1) { Name = "Forest" } ;
           Forest.TerrainGenerator = new PerlinSettings(10, .33f, .45f,.008f);
           Forest.GrassColor = Color.LightGreen;
           Forest.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
           Forest.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Forest.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Forest.WaterPercent = 1f;
           Forest.PrimaryLayer = 1;
           Forest.SecondaryLayer = 40;
           Forest.Weather = new WeatherChance(.85, .10,.05, 0);
           Forest.EnvironmentAddons.Add(Item.GrassPlant, 55);
           Forest.EnvironmentAddons.Add(Item.Mushroom, 3);
           Forest.EnvironmentAddons.Add(Item.BellFlower, 5);
           Forest.EnvironmentAddons.Add(Item.Crocosmia, 4);
           BiomeTypes.Add(Forest);

           Desert = new BiomeType(2) { Name = "Desert" };
           Desert.TerrainGenerator = new PerlinSettings(10, .24f, .36f, .006f);
           Desert.GrassColor = Color.Lerp(Color.Brown,Color.LightGreen,.8f);
           Desert.Top = new Tile(Item.Sand) { Background = Item.Blank};
           Desert.Primary = new Tile(Item.SandStone) { Background = Item.SandStoneBG };
           Desert.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Desert.PrimaryLayer = 7;
           Desert.SecondaryLayer = 30;
           Desert.WaterPercent = .1f;
           Desert.EnvironmentAddons.Add(Item.DeadBush, 10);
           Desert.Weather = new WeatherChance(.95, .04, .01, 0);
           BiomeTypes.Add(Desert);

           Marsh = new BiomeType(3) { Name = "Marsh" };
           Marsh.TerrainGenerator = new PerlinSettings(10, .008f, 1.05f, .005f);
           Marsh.GrassColor = Color.Lerp(Color.DarkSeaGreen,Color.LightGreen,.6f);
           Marsh.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
           Marsh.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Marsh.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Marsh.WaterPercent = 100f;
           Marsh.PrimaryLayer = 1;
           Marsh.SecondaryLayer = 40;
           Marsh.WaterColor = new Color(82, 204, 200);
           Marsh.Weather = new WeatherChance(.60, .25, .15, 0);
           Marsh.EnvironmentAddons.Add(Item.TallGrassPlant, 15);
           Marsh.EnvironmentAddons.Add(Item.GrassPlant, 50);
           Marsh.MinWaterSearch = 30;
           Marsh.MaxWaterSearch = 80;
           BiomeTypes.Add(Marsh);


           Mangrove = new BiomeType(4) { Name = "Mangrove" };
           Mangrove.TerrainGenerator = new PerlinSettings(10, .009f, 1f, .0051f);
           Mangrove.GrassColor = Color.Lerp(Color.DarkSeaGreen, Color.LightGreen, .6f);
           Mangrove.Top = new Tile(Item.Sand) { Background = Item.DirtBG };
           Mangrove.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Mangrove.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Mangrove.WaterPercent = 100f;
           Mangrove.PrimaryLayer = 4;
           Mangrove.SecondaryLayer = 40;
           Mangrove.WaterColor = new Color(82, 204, 200);
           Mangrove.Weather = new WeatherChance(.75, .20, .05, 0);
           Mangrove.EnvironmentAddons.Add(Item.TallGrassPlant, 14);
           Mangrove.EnvironmentAddons.Add(Item.GrassPlant, 40);
           BiomeTypes.Add(Mangrove);

           Fen = new BiomeType(5) { Name = "Fen" };
           Fen.TerrainGenerator = new PerlinSettings(10, .13f, .5f, .009f);
           Fen.GrassColor = new Color(175, 217, 110);
           Fen.WaterColor = new Color(0, 132, 207);
           Fen.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
           Fen.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Fen.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Fen.WaterPercent = 30f;
           Fen.PrimaryLayer = 1;
           Fen.SecondaryLayer = 40;
           Fen.Weather = new WeatherChance(.75, .2, .05, 0);
           Fen.EnvironmentAddons.Add(Item.GrassPlant, 55);
           Fen.EnvironmentAddons.Add(Item.Toadstool, 8);
           Fen.EnvironmentAddons.Add(Item.Mushroom, 5);
           Fen.EnvironmentAddons.Add(Item.BellFlower, 3);
           BiomeTypes.Add(Fen);

           Chaparral = new BiomeType(6) { Name = "Chaparral" };
           Chaparral.TerrainGenerator = new PerlinSettings(10, .16f, .46f, .0085f);
           Chaparral.GrassColor = new Color(145,240,115);
           Chaparral.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
           Chaparral.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Chaparral.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Chaparral.WaterPercent = 1f;
           Chaparral.PrimaryLayer = 1;
           Chaparral.SecondaryLayer = 40;
           Chaparral.Weather = new WeatherChance(.75, .15, .10, 0);
           Chaparral.EnvironmentAddons.Add(Item.GrassPlant, 53);
           Chaparral.EnvironmentAddons.Add(Item.TallGrassPlant, 9);
           Chaparral.EnvironmentAddons.Add(Item.BellFlower, 2);
           Chaparral.EnvironmentAddons.Add(Item.Crocosmia, 2);
           BiomeTypes.Add(Chaparral);

           Grove = new BiomeType(7) { Name = "Grove" };
           Grove.TerrainGenerator = new PerlinSettings(10, .22f, .45f, .0082f);
           Grove.GrassColor = new Color(100, 195, 92);
           Grove.WaterColor = new Color(55, 155, 175);
           Grove.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
           Grove.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Grove.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Grove.WaterPercent = 3f;
           Grove.PrimaryLayer = 1;
           Grove.SecondaryLayer = 40;
           Grove.Weather = new WeatherChance(.85, .10, .05, 0);
           Grove.EnvironmentAddons.Add(Item.GrassPlant, 60);
           Grove.EnvironmentAddons.Add(Item.Clover, 20);
           Grove.EnvironmentAddons.Add(Item.Toadstool, 1);
           Grove.EnvironmentAddons.Add(Item.Mushroom, 1);
           Grove.EnvironmentAddons.Add(Item.BellFlower, 1);
           Grove.EnvironmentAddons.Add(Item.Crocosmia, 1);
           BiomeTypes.Add(Grove);

           Taiga = new BiomeType(8) { Name = "Taiga" };
           Taiga.TerrainGenerator = new PerlinSettings(10, .33f, .45f, .008f);
           Taiga.GrassColor = new Color(100, 195, 92);
           Taiga.WaterColor = new Color(55, 155, 175);
           Taiga.Top = new Tile(Item.Snow) { Background = Item.SnowBG };
           Taiga.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Taiga.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Taiga.WaterPercent = 10f;
           Taiga.PrimaryLayer = 20;
           Taiga.SecondaryLayer = 40;
           Taiga.Weather = new WeatherChance(.5, .05, 0, .45);
           Taiga.EnvironmentAddons.Add(Item.GrassPlant, 10);
           Taiga.EnvironmentAddons.Add(Item.Toadstool, .5f);
           Taiga.EnvironmentAddons.Add(Item.Mushroom, .5f);
           Taiga.IceOnWater = true;
           BiomeTypes.Add(Taiga);

           Savanna = new BiomeType(9) { Name = "Savanna" };
           Savanna.TerrainGenerator = new PerlinSettings(10, .16f, .43f, .008f);
           Savanna.GrassColor = new Color(210, 215, 26);
           Savanna.WaterColor = new Color(50, 150, 230);
           Savanna.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
           Savanna.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
           Savanna.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
           Savanna.WaterPercent = 1f;
           Savanna.PrimaryLayer = 1;
           Savanna.SecondaryLayer = 40;
           Savanna.Weather = new WeatherChance(.75, .15, .10, 0);
           Savanna.EnvironmentAddons.Add(Item.GrassPlant, 10);
           Savanna.EnvironmentAddons.Add(Item.TallGrassPlant, 50);
           Savanna.EnvironmentAddons.Add(Item.Wildflower, 15);
           Savanna.EnvironmentAddons.Add(Item.Clover, 10);
           Savanna.EnvironmentAddons.Add(Item.BellFlower, 2);
           Savanna.EnvironmentAddons.Add(Item.Crocosmia, 2);
           BiomeTypes.Add(Savanna);
            //LushDesert = new BiomeType(3) { Name = "Lush Desert" };
            //LushDesert.TerrainGenerator = new PerlinSettings(10, .55f, .4f, .004f);
            //LushDesert.GrassColor = Color.Lerp(Color.Brown, Color.LightGreen, .2f);
            //LushDesert.Top = new Tile(Item.Sand) { Background = Item.SandBG };
            //LushDesert.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
            //LushDesert.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
            //LushDesert.WaterPercent = .8f;
            //LushDesert.PrimaryLayer = 5;
            //LushDesert.SecondaryLayer = 25;
            //LushDesert.Weather = new WeatherChance(.85, .05, .1, 0);

            //BiomeTypes.Add(LushDesert);
            //Jungle = new BiomeType(4) { Name = "Jungle" };
            //Jungle.TerrainGenerator = new PerlinSettings(10, .25f, .41f, .0065f);
            //Jungle.GrassColor = Color.LawnGreen; // new Color(15, 207, 0);
            //Jungle.Top = new Tile(Item.Grass) { Background = Item.DirtBG };
            //Jungle.Primary = new Tile(Item.Dirt) { Background = Item.DirtBG };
            //Jungle.Secondary = new Tile(Item.Stone) { Background = Item.StoneBG };
            //Jungle.WaterPercent = 1f;
            //Jungle.PrimaryLayer = 1;
            //Jungle.SecondaryLayer = 30;
            //Jungle.Weather = new WeatherChance(.65, .25, .10, 0);
            //Jungle.EnvironmentAddons.Add(Item.GrassPlant, 45);
            //Jungle.EnvironmentAddons.Add(Item.BellFlower, 5);
            //Jungle.EnvironmentAddons.Add(Item.Crocosmia, 6);
            //BiomeTypes.Add(Jungle);


            //BiomeTypes = new List<BiomeType>() 
            //{ 
            //     (Forest= new BiomeType(1)
            //     {
            //         Name = "Forest",
            //         Octaves = 10,
            //         Amplitude = .3f,
            //         Persistance = .4f,
            //         Frequency = .004f,
            //         GrassColor= Color.LightGreen,

            //     }),
            //     (Desert = new BiomeType(2)
            //     {
            //         Name = "Desert",
            //         Octaves = 10,
            //         Amplitude = .2f,
            //         Persistance = .34f,
            //         Frequency = .0056f,
            //         WaterPercent = 0f,
            //     }),
            //     (LushDesert = new BiomeType(3)
            //     {
            //         Name = "Lush Desert",
            //         Octaves = 10,
            //         Amplitude = .25f,
            //         Persistance = .37f,
            //         Frequency = .006f,
            //          WaterPercent = 3f,
            //     }),
            //     (Canyon = new BiomeType(4)
            //     {
            //         Name = "Canyon",
            //         Octaves = 10,
            //         Amplitude = .3f,
            //         Persistance = .6f,
            //         Frequency = .006f,
            //          WaterPercent = 1f,
            //     }),
            //     (Hell = new BiomeType(5)
            //     {
            //         Name = "Hell",
            //          Octaves = 8,
            //         Amplitude = .2f,
            //         Persistance = .5f,
            //         Frequency = .02f,
            //          WaterPercent = 0f,
            //          LavaPercent = 13f,
            //     }),
            //      (Chaparral = new BiomeType(6)
            //     {
            //         Name = "Chaparral",
            //         Octaves = 10,
            //         Amplitude = .32f,
            //         Persistance = .45f,
            //         Frequency = .0045f,
            //          GrassColor= new Color(95,180,80),
            //     }),
            //       (Plains = new BiomeType(7)
            //     {
            //         Name = "Plains",
            //         Octaves = 10,
            //         Amplitude = .23f,
            //         Persistance = .45f,
            //         Frequency = .0025f,
            //          GrassColor= new Color(80,200,80),
            //     }),
            //     (Swamp = new BiomeType(8)
            //     {
            //         Name = "Swamp",
            //         Octaves = 10,
            //         Amplitude = .1f,
            //         Persistance = .5f,
            //         Frequency = .005f,
            //           GrassColor= new Color(20,140,20),
            //          WaterPercent = 50f,
            //     }),
            // };
        }

        void CreateTerrain(object o, WorldGen wg, int x, int y)
        {
            BiomeType b = (BiomeType)o;
            if (y < wg.HeightMap[x] + PrimaryLayer && y >= wg.HeightMap[x])
                wg.Level.tiles[x, y] = new Tile(b.Top.Foreground) { Background = b.Top.Background };
            else if (y >= wg.HeightMap[x] + b.PrimaryLayer && y - b.SecondaryLayer <= wg.HeightMap[x] + b.SecondaryLayer)
                wg.Level.tiles[x, y] = new Tile(b.Primary.Foreground) { Background = b.Primary.Background };
            else if (y - b.SecondaryLayer > wg.HeightMap[x] + b.SecondaryLayer)
                wg.Level.tiles[x, y] = new Tile(b.Secondary.Foreground) { Background = b.Secondary.Background };
            else
                wg.Level.tiles[x, y] = new Tile(Item.Blank);
        }
            
        
        

    }
    public delegate void TerrainCreateEventHandler(object o, WorldGen wg, int x, int y);
}
