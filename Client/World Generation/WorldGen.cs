using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cyral.Extensions;
using ZarknorthClient.Interface;

namespace ZarknorthClient
{
    /// <summary>
    /// Creates and generates planet terrain
    /// </summary>
    public partial class WorldGen
    {
        #region Config
        /// <summary>
        /// Level below heightmap at which the very back, background appears
        /// </summary>
        public static int FullBackgoundLevel = 15;
        //Percents of world
        public static double AirPercent = .1;
        public static double WaterEndPercent = .7;
        public static double LavaStartPercent = .8;
        public static double LavaEndPercent = 1;
        #endregion
        #region Properties
        /// <summary>
        /// The levels heightmap of terrain, the distance between the top of the level and the ground
        /// </summary>
        public int[] HeightMap { get; set; }
        /// <summary>
        /// An array of all the biomes in a level
        /// </summary>
        public Biome[] Biomes { get; set; }
        /// <summary>
        /// Reference to the level
        /// </summary>
        public Level Level { get { return level; } private set { level = value; } }
        /// <summary>
        /// List of ore/rock viens
        /// </summary>
        public List<VeinSettings> VeinList = new List<VeinSettings>();
        #endregion
        #region Fields
        /// <summary>
        /// Main perlin noise generator
        /// </summary>
        private PerlinNoise noise;
        /// <summary>
        /// Random instanced aligned with the level seed
        /// </summary>
        private Random random;
        private Level level;
        #endregion

        /// <summary>
        /// Creates a new world generator
        /// </summary>
        /// <param name="level">level to be created upon</param>
        /// <param name="seed">Seed for randomness</param>
        public WorldGen(Level level, int seed)
        {
            this.level = level;
            noise = new PerlinNoise(seed);
            random = new Random(seed);
        }

        /// <summary>
        /// Start Generating
        /// </summary>
        public void Generate(Generator generator)
        {
            #if DEBUG
            //generator = Generator.Flat;
            #endif
            if (generator == Generator.Planet)
            {
                SetBiomes();
                Terrain();
                AddVeins();
                CreateVeins();
                CreateCaves();
                Trees(10);
                EnvironmentAddons();
                Water();
                Lava();
                Vines();
                Stalagtites();
                level.LiquidManager.SettleLiquids();
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Loading", 1);
                //Old Stuff, Keep for reference
                // Igloo(1, 13, 7);
                //int s =( level.Width / 2);
                //Dungeon(15,25,25,new Vector2(s,terrainContour[s] ),level.Height - randomizer.Next(30,100));
                //Volcano(RareRandom(30, 45, 60), RareRandom(40, 55, 75),.01f);
                //Castle(1, 41, level.Width / 2);
                //generating = "Building Structures...";
                //loatingIsland(level.Width / 2, level.Height / 2 - 100);
                //LoadAndDraw(Structure.Temple, level.Width / 2 + 50, terrainContour[level.Width / 2],true,out width );

            }
            else if (generator == Generator.Flat)
            {
                GenerateFlatWorld();
            }
        }
        private void CreateCaves()
        {
            for (int i = 0; i < level.Width; i++)
            {
                if (i % 100 == 0 && i != 0)
                    CreateCave(i, random.Next(0, 2) == 0 ? HeightMap[i] : random.Next(HeightMap[i], HeightMap[i] + 250), 10000, true);
            }
        }
        private void CreateCave(int startX, int startY, int iterations, bool branch)
        {
            float direction = 0;
            int randomDirection = random.Next(0, 4);
            if (randomDirection == 0)
                direction = 0;
            else if (randomDirection == 1)
                direction = 90;
            else if (randomDirection == 2)
                direction = 180;
            else if (randomDirection == 3)
                direction = 270;
            int x = startX;
            int y = startY;
            int inc = 0;
            int invisimode = 0;
            bool invisi = false;
            int nextTotal = GetNextTotal();
            int nextInvisi = random.Next(600, 1300);
            float lastDirection;
            while (y <= level.Height - 1)
            {
                lastDirection = direction;
                if (1 == random.Next(1, 1500) && branch)
                    CreateCave(x, y, random.Next(500, 1200), false);
                invisimode++;
                if (!invisi && invisimode > nextInvisi)
                {
                    invisi = true;
                    nextInvisi = random.Next(100, 250);
                    invisimode = 0;
                }
                if (invisi && invisimode > nextInvisi)
                {
                    invisi = false;
                    nextInvisi = random.Next(600, 1300);
                    invisimode = 0;
                }
                int dirX = 0, dirY = 0;
                if (direction > 300)
                    dirX = -1;
                else if (direction > 180)
                {
                    if (random.Next(0, 8) == 0)
                        dirY = 1;
                }
                else if (direction > 120)
                    dirX = 1;
                else if (direction > 0)
                {
                    if (random.Next(0, 8) == 0)
                        dirY = 1;
                }
                x += dirX;
                y += dirY;
                if (y > level.Width - 3)
                    return;
                direction = MathHelper.Lerp(lastDirection, direction += ((float)random.NextDouble() - .5f) * random.Next(80, 200), .45f);
                if (Math.Abs(lastDirection - direction) > 40)
                    y += 4;
                inc++;
                if (direction > 360)
                    direction -= 360;
                if (direction < 0)
                    direction = 360;
                if (inc > nextTotal)
                {
                    nextTotal = GetNextTotal();
                }
                if (!invisi)
                    if (!CaveTile(x, y))
                        return;
            }
        }
        private int GetNextTotal()
        {
            return random.Next(70, 130);
        }
        private bool CaveTile(int x, int y)
        {
            bool placeable = false;
            if (x > 0 && y > 0 && x < level.Width && y < level.Height)
            {
                placeable = true;
                int radius = random.Next(0, 3);
                if (random.Next(0, 8) == 0)
                    radius += random.Next(1, 3);
                for (int x2 = -radius; x2 < radius; x2++)
                {
                    for (int y2 = -radius; y2 < radius; y2++)
                    {
                        if (Vector2.Distance(new Vector2(x, y), new Vector2(x + x2, y + y2)) <= radius)
                        {
                            if (x + x2 > 0 && y + y2 > 0 && x + x2 < level.Width && y + y2 < level.Height)
                                level.ForcePlaceBlock(x + x2, y + y2, Item.Blank, false);
                        }
                    }
                }
            }
            return placeable;
        }
        /// <summary>
        /// Looks for areas and puts vines in the world
        /// </summary>
        private void Vines()
        {
            //One in X chance of generating in this position, provided that it is hanging on a dirt block
            int chance = 3;
            //How long a vine should be
            int minLength = 3;
            int maxLength = 8;

            for (int x = 1; x < level.Width - 2; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Growing vines", (((float)x / (level.Width - 2)) * .05f) + .9f);
                //Where a vine starts
                int start = 0;
                //Where a vine ends
                int end = 0;

                BiomeType curBiome = CheckBiome(x);

                //Loop from top of terrain (grass) to the bottom of the world
                for (int y = HeightMap[x]; y < level.Height - 2; y++)
                {
                    //If the Y position is between the start of the vine and end, and the current block is blank, then make a vine
                    if (y > start && y <= end && level.tiles[x, y].Foreground.ID == Item.Blank.ID)
                        level.tiles[x, y].Foreground = Item.Vine;
                    //If we are no longer in the vine, were never in it, or hit an obstacle, reset the count
                    else
                        start = 0;

                    //If we are not already making a vine, and the chance is met, then test to see if there is an area to hang on and if so, set the start/end points
                    if (start == 0 && random.Next(0, y > HeightMap[x] + curBiome.SecondaryLayer ? chance * 2 : chance) == 0 &&
                    (level.tiles[x, y].Foreground.ID == Item.Dirt.ID || level.tiles[x, y].Foreground.ID == Item.Grass.ID) &&
                    level.tiles[x, y + 1].Foreground.ID == Item.Blank.ID)
                    {
                        start = y;
                        end = y + random.Next(y > HeightMap[x] + curBiome.SecondaryLayer ? minLength / 2 : minLength, y > HeightMap[x] + curBiome.SecondaryLayer ? maxLength / 2 : maxLength);
                    }
                }
            }
        }
        /// <summary>
        /// Looks for areas and puts stalagtites in the world
        /// </summary>
        private void Stalagtites()
        {
            //One in X chance of generating in this position, provided that it is hanging on a stone block
            int chance = 6;

            for (int x = 1; x < level.Width - 1; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Forming stalagtites", (((float)x / (level.Width - 2)) * .02f) + .95f);
                //Loop from 2nd layer to the bottom of the world
                for (int y = CheckBiome(x).SecondaryLayer + HeightMap[x]; y < level.Height - 2; y++)
                {
                    if (random.Next(0, chance) == 0 &&
                    level.tiles[x, y].Foreground.ID == Item.Stone.ID &&
                    level.tiles[x, y + 1].Foreground.ID == Item.Blank.ID)
                    {
                        level.tiles[x, y + 1].Foreground = random.Next(0,5) != 0 ? Item.SmallStalagtite : Item.LargeStalagtite;
                    }
                }
            }
        }
        private void GenerateFlatWorld()
        {
            Biomes = new Biome[1];
            Biomes[0] = new Biome(BiomeType.Chaparral, 0, level.Width);
            HeightMap = new int[level.Width];
   
            for (int x = 0; x < level.Width; x++)
            {
                HeightMap[x] = -100 + (level.Height / 2);
            }
            //Set layers
            for (int x = 0; x < level.Width; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Creating world", (((float)x / (level.Width)) * .75f) + 0f);
                for (int y = 0; y < level.Height; y++)
                {
                    //Check for current biome, but a little bit off, to make a fading effect during transitions
                    BiomeType CurBiome = CheckBiome(x);
                    CurBiome.OnTerrainCreate(this, x, y);
                    if (x == 0 || x == level.Width - 1 || y == 0 || y == level.Height - 1 && y > HeightMap[x])
                        level.tiles[x, y] = new Tile(Item.Glass);
                }
            }
            Trees(10);
            EnvironmentAddons();
            level.tiles[level.Width / 2 -1, HeightMap[level.Width / 2] - 1] = new AnimatedTile(Item.Torch);
            level.tiles[level.Width / 2 - 4, HeightMap[level.Width / 2] - 2] = new TextTile(Item.Sign, level.Width / 2 - 4, HeightMap[level.Width / 2] - 2) { Text = "Welcome to the Admin flatworld!\nCyral says HI!\no3o, a plain world surrounded by a glass box\nNothing to explore, more to test!\nNow get back to work. :P ~Cy\n\nP.S. I used my L337 programming skills to create\na supply of all the items ingame\nwhich will always work with any number of items!\neven when you add new ones!" };
            List<Item> Items = Item.ItemList.Where(x => !(x is BackgroundBlockItem && (x as BlockItem).ForegroundEquivelent != null)).ToList();
            for (int a = 0; a < Math.Ceiling((float)Items.Count() / (float)(Item.WoodenChest.StorageSlots.X * Item.WoodenChest.StorageSlots.Y)); a++)
            {
                int per = (Item.WoodenChest.StorageSlots.X * Item.WoodenChest.StorageSlots.Y);
                level.tiles[level.Width / 2 + (a * 3), HeightMap[level.Width / 2] - 2] = new StorageTile(Item.WoodenChest, level.Width / 2 + (a * 3), HeightMap[level.Width / 2] - 2);
                ((StorageTile)level.tiles[level.Width / 2 + (a * 3), HeightMap[level.Width / 2] - 2]).Slots = new Slot[Item.WoodenChest.StorageSlots.X * Item.WoodenChest.StorageSlots.Y];

                for (int i = 0; i < Item.WoodenChest.StorageSlots.X * Item.WoodenChest.StorageSlots.Y; i++)
                {
                    if (i + (a * per) < Items.Count)
                        ((StorageTile)level.tiles[level.Width / 2 + (a * 3), HeightMap[level.Width / 2] - 2]).Slots[i] = new Slot(Items[i + (a * per)], Items[i + (a * per)].MaxStack);
                    else
                        ((StorageTile)level.tiles[level.Width / 2 + (a * 3), HeightMap[level.Width / 2] - 2]).Slots[i] = new Slot(Item.Blank,0);
                }
            }
        }

        public void Mite(int x, int y, int size, int maxHeight, float velocityX, BlockItem type)
        {
            int height = 0;
            float originalVelocityX = velocityX;
            float cursize = size;
 
            for (height = 0; height < maxHeight; height++)
            {
                for (int i = 0; i < cursize; i++)
                {
                    level.tiles[x+i + (size - (int)cursize/2) + (int)velocityX, y + height].Foreground = type;
                    
                }
                cursize -= (float)size / (float)maxHeight;
                velocityX += originalVelocityX;
                if (random.Next(0, 10) == 0 && height < maxHeight / 2)
                {
                    Mite(x + (size - (int)cursize / 2) + (int)velocityX, y + height, random.Next(1,4),random.Next(3,8),originalVelocityX * -1f, Item.ClayBG);
                }

            }
        }
       

        public void FloatingIsland(int i, int j)
        {
            int r = random.Next(20,30); // diameter
            int os = random.Next(1, 1000) * 10;
            BiomeType b = CheckBiome(i);
            for (int x = -r; x < r; x++)
            {
                int height = (int)Math.Sqrt(r * r - x * x);
                float result = (OctaveGenerator(noise, x + os, b.TerrainGenerator.Octaves, b.TerrainGenerator.Amplitude, b.TerrainGenerator.Persistance, b.TerrainGenerator.FrequencyX) );
                int point = -((int)(200 * ((result + 1) / 2)) - 100);
                for (int y = point; y < height; y++)
                {
                    if (y == point)
                        level.tiles[x + i, y + j] = new Tile(b.Top.Foreground) { Background = b.Top.Background };
                    else if (y - random.Next(8,10) < point)
                        level.tiles[x + i, y + j] = new Tile(b.Primary.Foreground) { Background = b.Primary.Background };
                    else
                        level.tiles[x + i, y + j] = new Tile(b.Secondary.Foreground) { Background = b.Secondary.Background };
                    if (y == height - 1&& random.Next(0,9)==0 && x > -r + 13&& x < r-13)
                    {
                        Mite(x + i, y + j-5, random.Next(5, 10), random.Next(13, 28), random.Next(-50,50) / 50, Item.Stone);
                    }
                }
            }
        }
            
            
      
        public void SetBiomes()
        {
            Biomes = new Biome[random.Next(16,20)];
           
            for (int i = 0; i < Biomes.Length; i++)
            {
                //Select random biome (This gets the amount of biome types, generates a random number)
                BiomeType biome =  BiomeType.BiomeTypes[random.Next(0, BiomeType.BiomeTypes.Count)];
                //biome = BiomeType.Grove;
                if (i == 0) //If First biome
                {
                    //Create biome starting at 0, and going as far as Width / Biomes +- a small offset
                    Biomes[i] = new Biome(biome, 0, level.Width / Biomes.Length + random.Next(-20,20));
                }
                else if (i > 0 && i < Biomes.Length - 1) //If a middle biome
                {
                    //Create biome starting at the last biomes end, and going as far as Width / Biomes +- a small offset
                    Biomes[i] = new Biome(biome, Biomes[i - 1].End + 1, Biomes[i - 1].End + 1 + (level.Width / Biomes.Length) + random.Next(-20, 20));
                }
                else if (i == Biomes.Length - 1) //If Last biome
                {
                    //Create biome starting at the last biomes end, and going to the end of the world
                    Biomes[i] = new Biome(biome, Biomes[i - 1].End + 1, level.Width);
                }
            }

        }
        /// <summary>
        /// Checks if a coordinate is in a biome
        /// </summary>
        /// <returns>A BiomeType</returns>
        public BiomeType CheckBiome(int x)
        {
            x = level.tiles.PerformTileRepeatLogic(x);
            foreach (Biome b in Biomes)
            {
                if (x >= b.Start && x <= b.End)
                    return b.Type;
            }
            return BiomeType.Forest;
        }
        /// <summary>
        /// Generates basic terrain (hills)
        /// </summary>
        /// <param name="offset">Offset from top of level</param>
        /// <param name="peakheight">Peak of hills</param>
        /// <param name="flatness">Percent of flatness</param>
        private void Terrain()
        {
            //Variables
            HeightMap = new int[level.Width];
            //Generate
            ReGen(noise);
            for (int x = 0; x < level.Width; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Creating land", (((float)x / level.Width) * .05f) + .0f);
                BiomeType b = CheckBiome(x);
                var result = 1f;
                result = OctaveGenerator(noise, x, b.TerrainGenerator);
                HeightMap[x] = (int)(200 * ((result + 1) / 2)) + ((int)Math.Max(128,AirPercent * Level.Width) - 100);
            }
            //Smooth biome transitions
            for (int i = 0; i < Biomes.Count(); i++)
            {
                bool IsFirstBiome = i == 0;
                bool IsLastBiome = i == Biomes.Count() - 1;

                //If this is not the first or last biome in the world
                if (!IsFirstBiome)
                {
                    Biome CurrentBiome = Biomes[i];
                    Biome LeftBiome = Biomes[i - 1];

                    if (CurrentBiome.Type != LeftBiome.Type)
                    {
                        //Get the midpoint between the 2 terrain types
                        int Midpoint = (HeightMap[LeftBiome.End - 1] + HeightMap[CurrentBiome.Start]) / 2;
                        int Length = Math.Abs(HeightMap[LeftBiome.End - 1] - HeightMap[CurrentBiome.Start]);
                        //Lerp backwards for the last biome
                        for (int x = 0; x < Length; x++)
                        {
                            HeightMap[CurrentBiome.Start - x] = (int)MathHelper.Lerp(HeightMap[CurrentBiome.Start - x], Midpoint, 1 - ((float)x / (float)Length));
                        }

                        //Lerp backwards for the last biome
                        for (int x = 1; x < Length; x++)
                        {
                            HeightMap[CurrentBiome.Start + x] = (int)MathHelper.Lerp(HeightMap[CurrentBiome.Start + x], Midpoint, 1 - ((float)x / (float)Length));
                        }
                    }
                }
                else if (IsFirstBiome)
                {
                    Biome CurrentBiome = Biomes[i];
                    Biome LeftBiome = Biomes[Biomes.Count() - 1];

                    //Get the midpoint between the 2 terrain types
                    int Midpoint = (HeightMap[LeftBiome.End - 1] + HeightMap[CurrentBiome.Start]) / 2;
                    int Length = Math.Abs(HeightMap[LeftBiome.End - 1] - HeightMap[CurrentBiome.Start]);
                    //Lerp backwards for the last biome
                    for (int x = 0; x < Length; x++)
                    {
                        HeightMap[level.tiles.PerformTileRepeatLogic(CurrentBiome.Start - x)] = (int)MathHelper.Lerp(HeightMap[level.tiles.PerformTileRepeatLogic(CurrentBiome.Start - x)], Midpoint, 1 - ((float)x / (float)Length));
                    }

                    //Lerp backwards for the last biome
                    for (int x = 1; x < Length; x++)
                    {
                        HeightMap[level.tiles.PerformTileRepeatLogic(CurrentBiome.Start + x)] = (int)MathHelper.Lerp(HeightMap[level.tiles.PerformTileRepeatLogic(CurrentBiome.Start + x)], Midpoint, 1 - ((float)x / (float)Length));
                    }
                }
            }
            //Set layers
            for (int x = 0; x < level.Width; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Filling world", (((float)x / level.Width) * .05f) + .05f);
                for (int y = 0; y < level.Height; y++)
                {
                    //Check for current biome, but a little bit off, to make a fading effect during transitions
                    BiomeType CurBiome = CheckBiome(x + random.Next(-5, 5));
                    CurBiome.OnTerrainCreate(this, x, y);
                }
            }
        }
        
        void Lava()
        {
            for (int x = 1; x < level.Width - 1; x++)
            {
                for (int y = level.Height - 10; y < level.Height; y++)
                {
                    if (level.tiles[x,y].Foreground == Item.Blank)
                    {
                        level.tiles[x, y].LavaMass = 255;
                    }
                }
            }
            for (int x = 50; x < level.Width - 50; x++)
            {
                if (random.NextDouble() <= CheckBiome(x).LavaPercent / 100) //Percent
                    FillLavaHoles(x, HeightMap[x]);
            }
            for (int x = 1; x < level.Width - 1; x++)
            {
                for (int y = (int)(level.Height * LavaStartPercent); y < level.Height * LavaEndPercent; y++)
                {
                    if (level.tiles[x,y].Foreground.Collision.CanFlowThrough() && random.Next(0, 60) == 0)
                        AddLava(x, y);
                }
            }
        }

        public void FillLavaHoles(int x, int y)
        {
            //int a = randomizer.Next(10, 50);
            //if (CheckBiome(x) == BiomeType.Swamp)
            //    a = randomizer.Next(10, 100);
            //int right = -1;
            //int left = -1;
            //if (x + a < level.Width - 3)
            //{
            //    for (int i = 0; i <= a; i++) //Go across and check for holes
            //    {

            //        if (level.tiles[x + i, y - 1].Foreground.Collision != BlockCollision.Passable)//If its not long enough, exit
            //        {
            //            left = i;
            //            i = a + 1;
            //        }
            //    }
            //    for (int i = 0; i <= a; i++) //Go across and check for holes
            //    {
            //        if (level.tiles[x - i, y - 1].Foreground.Collision != BlockCollision.Passable)//If its not long enough, exit
            //        {
            //            right = i;
            //            i = a + 1;
            //        }
            //    }
            //    if (right != -1 && left != -1)
            //    {

            //        for (int j = 0; j <= left; j++)
            //        {
            //            for (int k = 0; level.tiles[x + j, y - 1 + k].Foreground == Item.Blank && level.liquid.LavaMass[x + j, y - 1 + k] < 1; k++)
            //            {

            //                AddLava(x + j, y - 1 + k, 1);
            //                //level.tiles[x + j, y - 1 + k].item = Item.Torch;
            //            }
            //        }
            //        for (int j = 0; j <= right; j++)
            //        {
            //            for (int k = 0; level.tiles[x - j, y - 1 + k].Foreground == Item.Blank && level.liquid.LavaMass[x - j, y - 1 + k] < 1; k++)
            //            {
            //                //level.tiles[x - j, y - 1 + k].item = Item.Torch;
            //                AddLava(x - j, y - 1 + k, 1);
            //            }
            //        }

            //    }
            //}
        }
        /// <summary>
        /// Add water to the world
        /// </summary>
        void Water()
        {
            for (int x = 50; x < level.Width - 50; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Watering", (((float)x / level.Width) * .05f) + .85f);
                if (random.NextDouble() <= CheckBiome(x).WaterPercent / 100) //Percent
                    x += FillWaterHoles(x,HeightMap[x]);
            }
            for (int x = 1; x < level.Width - 1; x++)
            {
                for (int y = (int)(HeightMap[x] + 10); y < level.Height * WaterEndPercent; y++)
                {
                    if (level.tiles[x,y].Foreground.Collision.CanFlowThrough() && random.Next(0, 60) == 0)
                        AddWater(x, y);
                }
            }
        }

        public int FillWaterHoles(int x, int y)
        {
            BiomeType b = CheckBiome(x);
            int a = random.Next(b.MinWaterSearch,b.MaxWaterSearch);
            int right = -1;
            int left = -1;
            if (x + a < level.Width - 3)
            {
                for (int i = 0; i <= a; i++) //Go across and check for holes
                {

                    if (level.tiles[x + i, y - 1].Foreground.Collision != BlockCollision.Passable)//If its not long enough, exit
                    {
                        left = i;
                        i = a + 1;
                    }
                }
                for (int i = 0; i <= a; i++) //Go across and check for holes
                {
                    if (level.tiles[x - i, y - 1].Foreground.Collision != BlockCollision.Passable)//If its not long enough, exit
                    {
                        right = i;
                        i = a + 1;
                    }
                }
                if (right != -1 && left != -1)
                {
                    
                        for (int j = 0; j <= left; j++)
                        {
                            for (int k = 0; (level.tiles[x + j, y - 1 + k].Foreground.Collision == BlockCollision.Passable || level.tiles[x + j, y - 1 + k].Foreground == Item.SmoothIce) && level.tiles[x + j, y - 1 + k].WaterMass < 1; k++)
                            {
                                if (CheckBiome(x - j).IceOnWater && k == 0)
                                {
                                    BlockItem old = level.tiles[x + j, y - 1, true].Background;
                                    level.tiles[x + j, y - 1, true] = new Tile(Item.SmoothIce) { Background = old };
                                }
                                else
                                AddWater(x + j, y - 1 + k);
                            }
                        }
                        for (int j = 0; j <= right; j++)
                        {
                            for (int k = 0; (level.tiles[x - j, y - 1 + k].Foreground.Collision == BlockCollision.Passable|| level.tiles[x - j, y - 1 + k].Foreground == Item.SmoothIce) && level.tiles[x - j, y - 1 + k].WaterMass < 1; k++)
                            {
                                if (CheckBiome(x-j).IceOnWater && k == 0)
                                {
                                    BlockItem old1 = level.tiles[x - j, y - 1, true].Background;
                                    level.tiles[x - j, y - 1, true] = new Tile(Item.SmoothIce) { Background = old1 };
                                }
                                else
                                AddWater(x - j, y - 1 + k);
                            }
                        }
                    
                }
            }
            return left == -1 ? 2 : left;
        }

        public void EnvironmentAddons()
        {
            for (int x = 1; x < level.Width - 1; x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Landscaping", (((float)x / level.Width) * .05f) + .8f);
                BiomeType b = CheckBiome(x);
                var e = from pair in b.EnvironmentAddons orderby pair.Value descending select pair;
                foreach (KeyValuePair<BlockItem, float> kv in e)
                {
                    if ((random.Next(100) <= kv.Value) && (level.tiles[x, HeightMap[x]].Foreground == CheckBiome(x).Top.Foreground))
                    {
                        level.ForcePlaceBlock(x, HeightMap[x] - 1 - (kv.Key.Size.Y - 1), kv.Key,false,new GameTime());
                    }
                }
            }
        }
       /// <summary>
       /// Makes trees
       /// </summary>
       /// <param name="percent">Amount of trees</param>
       /// <param name="maxheight">Max height of trees</param>
       /// /// <param name="minheight">Min height of trees</param>
        public void Trees(int percent)
        {
            for(int x =20;x < level.Width-20;x++)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Growing trees", (((float)x / level.Width) * .05f) + .75f);
                if ((random.Next(100) <= percent) && (level.tiles[x, HeightMap[x]].Foreground == CheckBiome(x).Top.Foreground))
                {
                    int ranHeight = 0;
                    BiomeType CurBiome = CheckBiome(x);
                    if (CurBiome == BiomeType.Forest)
                    {
                       ranHeight = random.Deviation(5,13,9);
                       OakTree(x, ranHeight);
                       x+=7;
                    }
                    else if (CurBiome == BiomeType.Fen)
                    {
                        int r = random.Next(0, 3);
                        if (r == 0)
                        {
                            ranHeight = random.Next(5, 8);
                            SmallAlderTree(x, ranHeight);
                            x += 7;
                        }
                        else if (r == 1)
                        {
                            ranHeight = random.Next(6, 12);
                            TallAlderTree(x, ranHeight);
                            x += 5;
                        }
                        else if (r == 2)
                        {
                            ranHeight = random.Next(7, 12);
                            MediumAlderTree(x, ranHeight);
                            x += 6;
                        }
                    }
                    else if (CurBiome == BiomeType.Grove)
                    {
                        ranHeight = random.Next(17, 24);
                        PoplarTree(x, ranHeight);
                        x += 8;
                    }
                    else if (CurBiome == BiomeType.Savanna)
                    {
                        ranHeight = random.Next(4, 7);
                        SavannaTree(x, ranHeight);
                        x += 8;
                    }
                    else if (CurBiome == BiomeType.Mangrove)
                    {
                        ranHeight = random.Next(4, 8);
                        MangroveTree(x, ranHeight);
                        x += 7;
                    }
                    else if (CurBiome == BiomeType.Jungle)
                    {
                        ranHeight = random.Deviation(5, 13, 9);
                        JungleTree(x, (int)(ranHeight * 1.5f));
                        x += 7;
                    }
                    else if (CurBiome == BiomeType.Desert)
                    {
                        ranHeight = random.Deviation(5, 13, 9);
                        Cactus(x, ranHeight + 3); //If in the desert, Make a cactus
                        x += 4;
                    }
                    else if (CurBiome == BiomeType.Chaparral)
                    {
                        ranHeight = random.Deviation(5, 12, 7);
                        if (random.NextDouble() > .1) //Most plants are bushes
                        {
                            ranHeight = random.Next(1, 4);
                            MiniBush(x, ranHeight); //Make a small bushy thing
                        }
                        else
                            OakTree(x, ranHeight);
                        x += 7;
                    }
                    else if (CurBiome == BiomeType.Swamp)
                    {
                        ranHeight = random.Deviation(5, 13, 9);
                        SwampTree(x, ranHeight / 2); //If in the desert, Make a cactus
                        x += 7;
                    }
                    else if (CurBiome == BiomeType.Taiga)
                    {
                        ranHeight = random.Deviation(5, 13, 9);
                        SpruceTree(x, ranHeight, random.NextBoolean()); //If in the desert, Make a cactus
                        x += 7;
                    }
                }
                
                 
                 
            }
        }
        public void SpruceTree(int x, int height, bool variation)
        {
            if (!variation)
            {
                int leafStart = (int)(height / 2f);
                //Go up the tree and make the base/stem
                for (int i = 1; i <= height + leafStart - 4; i++)
                    level.tiles[x, HeightMap[x] - i].Background = Item.SpruceTree;
                for (int i = 0; i <= height - 2; i++) //Add leaves on top
                {
                        int l = (int)((height - i) / 2f);
                        level.tiles[x, HeightMap[x] - i - (leafStart + 1)].Foreground = Item.SpruceLeaf;
                        for (int j = -l; j <= l; j++)
                        {

                            level.tiles[x + j, HeightMap[x] - i - leafStart].Foreground = Item.SpruceLeaf;
                        }
                }
                 
            }
            else
            {
                int leafStart = (int)(height / 2f);
                //Go up the tree and make the base/stem
                for (int i = 1; i <= height + leafStart - 4; i++)
                    level.tiles[x, HeightMap[x] - i].Background = Item.SpruceTree;
                for (int i = 0; i <= height - 2; i++) //Add leaves on top
                {
                    if (i % 2 != 0)
                    {
                        int l = (int)((height - i) / 2f);
                        level.tiles[x, HeightMap[x] - i - (leafStart + 1)].Foreground = Item.SpruceLeaf;
                        for (int j = -l; j <= l; j++)
                        {
                            if (j > (int)(-l / random.NextFloat(1.3f, 1.8f)) && j < (int)(l / random.NextFloat(1.3f, 1.8f)))
                                level.tiles[x + j, HeightMap[x] - i - (leafStart + 1)].Foreground = Item.SpruceLeaf;
                            level.tiles[x + j, HeightMap[x] - i - leafStart].Foreground = Item.SpruceLeaf;

                            level.tiles[x + j, HeightMap[x] - i - leafStart].Foreground = Item.SpruceLeaf;

                        }
                    }
                }
            }
        }
        public void TallAlderTree(int x, int height)
        {
            for (int i = 1; i <= height; i++)
                level.tiles[x, HeightMap[x] - i].Background = Item.AlderTree;
            for (int i = ((height / 3)).RoundTo(2); i <= height.RoundTo(2) + 1; i++)
            {
                level.tiles[x, HeightMap[x] - i].Foreground = Item.AlderLeaf;
                //If even
                if (i % 2 == 0)
                {
                    level.tiles[x + 1, HeightMap[x] - i].Foreground = Item.AlderLeaf;
                    level.tiles[x - 1, HeightMap[x] - i].Foreground = Item.AlderLeaf;
                }
            }
        }
        public void MediumAlderTree(int x, int height)
        {
            for (int i = 1; i <= height; i++)
                level.tiles[x, HeightMap[x] - i].Background = Item.AlderTree;

            //75% chance to have top
            if (random.NextBoolean() || random.NextBoolean())
                    level.tiles[x , (HeightMap[x] - height) - 1].Foreground = Item.AlderLeaf;
            level.tiles[x, (HeightMap[x] - height)].Foreground = Item.AlderLeaf;
            level.tiles[x, (HeightMap[x] - height) + 1].Foreground = Item.AlderLeaf;
            for (int i = -random.Next(1, 3); i <= random.Next(1, 3); i++)
                level.tiles[x + 1, (HeightMap[x] - height)].Foreground = Item.AlderLeaf;
            for (int i = -random.Next(1, 3); i <= random.Next(1, 3); i++)
                level.tiles[x -1, (HeightMap[x] - height)].Foreground = Item.AlderLeaf;

            for (int i = -random.Next(1, 3); i <= random.Next(1, 3); i++)
                level.tiles[x + 1, (HeightMap[x] - height) + 1].Foreground = Item.AlderLeaf;
            for (int i = -random.Next(1, 3); i <= random.Next(1, 3); i++)
                level.tiles[x - 1, (HeightMap[x] - height) + 1].Foreground = Item.AlderLeaf;

            if (random.Next(0, 3) > 0)
                level.tiles[x + 1, (HeightMap[x] - height) + 2].Foreground = Item.AlderLeaf;
            if (random.Next(0, 3) > 0)
                level.tiles[x - 1, (HeightMap[x] - height) + 2].Foreground = Item.AlderLeaf;
        }
        public void SmallAlderTree(int x, int height)
        {
            for (int i = 1; i <= height; i++)
                level.tiles[x, HeightMap[x] - i].Background = Item.AlderTree;

            if (random.NextBoolean())
            for (int i = -random.Next((int)(height / 2f) - 1, (int)(height / 2f) + 1) + 2; i <= random.Next((int)(height / 2f) - 1, (int)(height / 2f) + 1) - 2; i++)
                level.tiles[x + i, (HeightMap[x] - height) + 2].Foreground = Item.AlderLeaf;

            for (int i = -random.Next((int)(height / 1.8f) - 1, (int)(height / 1.8f) + 1) + 1; i <= random.Next((int)(height / 1.8f) - 1, (int)(height / 1.8f) + 1) - 1; i++)
                level.tiles[x + i, (HeightMap[x] - height) + 1].Foreground = Item.AlderLeaf;

            for (int i = -random.Next((int)(height / 1.8f) - 1, (int)(height / 1.8f) + 1); i <= random.Next((int)(height / 1.8f) - 1, (int)(height / 1.8f) + 1); i++)
                level.tiles[x + i, (HeightMap[x] - height)].Foreground = Item.AlderLeaf;
            for (int i = -random.Next((int)(height / 1.8f) - 1, (int)(height / 1.8f) + 1) + 2; i <= random.Next((int)(height / 1.8f) - 1, (int)(height / 1.8f) + 1) - 2; i++)
                level.tiles[x + i, (HeightMap[x] - height) - 1].Foreground = Item.AlderLeaf;

            if (random.Next(0,3) > 0)
                level.tiles[x + 1, (HeightMap[x] - height) + 2].Foreground = Item.AlderLeaf;
            if (random.Next(0, 3) > 0)
                level.tiles[x - 1, (HeightMap[x] - height) + 2].Foreground = Item.AlderLeaf;
        }
        public void MangroveTree(int x, int height)
        {
            for (int i = 0; i <= height; i++)
                level.tiles[x, HeightMap[x] - i].Background = Item.MangroveTree;
            for (int a = -5; a <= random.Next(1,3); a++)
                if (level.tiles[x +1, HeightMap[x] - a - 1].Foreground.Collision == BlockCollision.Passable)
                level.tiles[x +1, HeightMap[x] - a].Background = Item.MangroveTree;
            for (int a = -5; a <= random.Next(1, 3); a++)
                if (level.tiles[x - 1, HeightMap[x] - a - 1].Foreground.Collision == BlockCollision.Passable)
                level.tiles[x - 1, HeightMap[x] - a].Background = Item.MangroveTree;

            for (int i = -random.Next((int)(height / 1.5f) - 1, (int)(height / 1.5f) + 1) + 1; i <= random.Next((int)(height / 1.5f) - 1, (int)(height / 1.5f) + 1) - 1; i++)
                level.tiles[x + i, (HeightMap[x] - height) + 1].Foreground = Item.MangroveLeaf;

            for (int i = -random.Next((int)(height / 1.5f) - 1, (int)(height / 1.5f) + 1); i <= random.Next((int)(height / 1.5f) - 1, (int)(height / 1.5f) + 1); i++)
                level.tiles[x + i, (HeightMap[x] - height)].Foreground = Item.MangroveLeaf;

            for (int i = -random.Next((int)(height / 1.5f) - 1, (int)(height / 1.5f) + 1) + 2; i <= random.Next((int)(height / 1.5f) - 1, (int)(height / 1.5f) + 1) - 2; i++)
                level.tiles[x + i, (HeightMap[x] - height) -1].Foreground = Item.MangroveLeaf;

            if (random.NextBoolean())
                level.tiles[x + 1, (HeightMap[x] - height) + 2].Foreground = Item.MangroveLeaf;
            if (random.NextBoolean())
                level.tiles[x - 1, (HeightMap[x] - height) + 2].Foreground = Item.MangroveLeaf;
        }
        public void OakTree(int x, int height)
        {
            //Go up the tree and make the base/stem
            for (int i = 1; i <= height; i++)
                level.tiles[x, HeightMap[x] - i].Background = Item.OakTree;
            BushyThingy(x, HeightMap[x] - height + 1, 25,Item.OakLeaf);
        }
        public void PoplarTree(int x, int height)
        {
            int start = height / 3;
            List<Tuple<int,int>> sizes = new List<Tuple<int,int>>();

            sizes.Add(new Tuple<int,int>(1,3));
            sizes.Add(new Tuple<int,int>(2,5));
            sizes.Add(new Tuple<int,int>(4,7));
            sizes.Add(new Tuple<int,int>(3,5));
            sizes.Add(new Tuple<int,int>(3,3));
            sizes.Add(new Tuple<int,int>(3,1));
            //Go up the tree and make the base/stem
            for (int j = 1; j <= start + 10; j++)
                level.tiles[x, HeightMap[x] - j].Background = Item.SpruceTree;
            int i = 0;
            foreach (Tuple<int, int> t in sizes)
            {
                int h = t.Item1 + (random.NextBoolean() ? random.Next(-1, 2) : 0);
                for (int a = 0; a < h; a++)
                {
                    for (int b = -((t.Item2 - 1) / 2); b <= ((t.Item2 - 1) / 2); b++)
                    {
                        level.tiles[x + b, HeightMap[x] - start - i].Foreground = Item.SpruceLeaf;
                    }
                    i++;
                }
            }
        }
        public void SavannaTree(int x, int height)
        {
            int start = height;
            if (random.Next(0, 5) == 0)
                start = 1;
            List<Tuple<int, int>> sizes = new List<Tuple<int, int>>();

            sizes.Add(new Tuple<int, int>(1, start == 1 ? 6 : 8));
            sizes.Add(new Tuple<int, int>(1, start == 1 ? random.Next(1, 4) : random.Next(3, 6)));
            //Go up the tree and make the base/stem
            for (int j = 1; j <= start; j++)
            {
                level.tiles[x, HeightMap[x] - j].Background = Item.AcadiaTree;
            }
            int i = 0;
            foreach (Tuple<int, int> t in sizes)
            {
                int h = t.Item1;
                for (int a = 0; a < h; a++)
                {
                    for (int b = -((t.Item2 - 1) / 2); b <= ((t.Item2 - 1) / 2); b++)
                    {
                        level.tiles[x + b, HeightMap[x] - start - i].Foreground = Item.AcadiaLeaf;
                    }
                    i++;
                }
            }
        }
        public void JungleTree(int x, int height)
        {
            for (int j = 0; j < 9; j += 3)
            {
                //Go up the tree and make the base/stem
                for (int i = 1; i <= height; i++)
                {
                    level.tiles[x, HeightMap[x] + j - i].Background = Item.JungleTree;
                }
                int r = random.Next(6, 10);
                Line(new Vector2(x - r, HeightMap[x] + j -height), new Vector2(x + r, HeightMap[x] + j - height), Item.JungleLeaf);
                Line(new Vector2(x - r + random.Next(2, 5), HeightMap[x] - height + j -1), new Vector2(x + r - random.Next(2, 5), HeightMap[x] + j - height - 1), Item.JungleLeaf);
                if (random.Next(0, 2) == 0)
                {
                    Line(new Vector2(x - r + 4, HeightMap[x] + j - height - 1), new Vector2(x, HeightMap[x] + j -height + 4), Item.JungleTree);
                    Line(new Vector2(x - r + 5, HeightMap[x] + j - height - 1), new Vector2(x, HeightMap[x] + j -height + 5), Item.JungleTree);
                }
                else
                {
                    Line(new Vector2(x + r - 4, HeightMap[x] + j - height - 1), new Vector2(x, HeightMap[x] + j -height + 4), Item.JungleTree);
                    Line(new Vector2(x + r + -5, HeightMap[x] + j - height - 1), new Vector2(x, HeightMap[x] + j -height + 5), Item.JungleTree);
                }
            }
                // BushyThingy(x, HeightMap[x] - height + 1, 25, Item.JungleLeaf);
        }

        /// <summary>
        /// Creates a line between 2 points
        /// </summary>
        /// <param name="start">Start point</param>
        /// <param name="end">End point</param>
        /// <param name="Item">Item to draw with</param>
        public void Line(Vector2 start, Vector2 end, BlockItem Item)
        {
            Vector2 deltaVector = end - start;
            float distance = deltaVector.Length();
            Vector2 direction = deltaVector / new Vector2(distance, distance);
            Vector2 oldPoint = new Vector2(0, 0);
            for (float z = 1; z <= distance; z++)
            {
                Vector2 newPoint = start + direction * (distance * (z / distance));
                if (Item is BackgroundBlockItem)
                    level.tiles[(int)newPoint.X, (int)newPoint.Y].Background = Item;
                else if (Item is ForegroundBlockItem)
                level.tiles[(int)newPoint.X, (int)newPoint.Y].Foreground = Item;// //background = Backgrounds.Dirt };
                oldPoint = newPoint;
            }
        }
        public void PlaceTool(int x, int y, BlockItem item, bool BGKey)
        {
            if (!BGKey)
                level.tiles[x, y].Foreground.OnDrop(new DropBlockEventArgs(level, x, y, item, true));
            else
                level.tiles[x, y].Background.OnDrop(new DropBlockEventArgs(level, x, y, item, true));
            level.PlaceBlock(x,y, item, false, null, true);
            level.tiles[x, y].ForegroundSheet = Rectangle.Empty;
            level.tiles[x, y].BackgroundSheet = Rectangle.Empty;

            level.tiles[x+ 1, y].ForegroundSheet = Rectangle.Empty;
            level.tiles[x+ 1, y].BackgroundSheet = Rectangle.Empty;
            level.tiles[x - 1, y].ForegroundSheet = Rectangle.Empty;
            level.tiles[x - 1, y].BackgroundSheet = Rectangle.Empty;
            level.tiles[x, y + 1].ForegroundSheet = Rectangle.Empty;
            level.tiles[x, y + 1].BackgroundSheet = Rectangle.Empty;
            level.tiles[x, y - 1].ForegroundSheet = Rectangle.Empty;
            level.tiles[x, y - 1].BackgroundSheet = Rectangle.Empty;
        }
        public void CircleTool(Point start, Point end, BlockItem item, bool BGKey, bool filled)
        { 
            ToolConvertUnits(ref start, ref end);
            float radiusVert = Math.Abs(start.Y - end.Y) / 2;
            float radiusHor = Math.Abs(start.X - end.X) / 2;
            int radius =(int)Math.Round( Math.Max(radiusHor, radiusVert));
            int x0 = start.X + (int)Math.Round(radiusHor); 
            int y0 = start.Y + (int)Math.Round(radiusVert);
            int x = radius;
            int y = 0;
            int xChange = 1 - (radius << 1);
            int yChange = 0;
            int radiusError = 0;

            while (x >= y)
            {
                if (!filled)
                {
                    PlaceTool(x + x0, y + y0, item, BGKey);
                    PlaceTool(y + x0, x + y0, item, BGKey);
                    PlaceTool(-x + x0, y + y0, item,BGKey);
                    PlaceTool(-y + x0, x + y0, item,BGKey);
                    PlaceTool(-x + x0, -y + y0, item, BGKey);
                    PlaceTool(-y + x0, -x + y0, item, BGKey);
                    PlaceTool(x + x0, -y + y0, item, BGKey);
                    PlaceTool(y + x0, -x + y0, item,BGKey);
                }
                else
                {
                    for (int i = x0 - x; i <= x0 + x; i++)
                    {
                        PlaceTool(i, y0 + y, item, BGKey);
                        PlaceTool(i, y0 - y, item, BGKey);
                    }
                    for (int i = x0 - y; i <= x0 + y; i++)
                    {
                        PlaceTool(i, y0 + x, item, BGKey);
                        PlaceTool(i, y0 - x, item, BGKey);
                    }
                }

                y++;
                radiusError += yChange;
                yChange += 2;
                if (((radiusError << 1) + xChange) > 0)
                {
                    x--;
                    radiusError += xChange;
                    xChange += 2;
                }
            }
        }
        public void RectangleTool(Point start, Point end, BlockItem item, bool BGKey, bool filled)
        {
            ToolConvertUnits(ref start, ref end);
            for (int x = start.X; x <= end.X; x++)
                for(int y = start.Y; y <= end.Y; y++)
                {
                    if (filled)
                        PlaceTool(x, y, item, BGKey);
                    else if (x == start.X || x == end.X || y == start.Y || y == end.Y)
                        PlaceTool(x, y, item, BGKey);
                }
        }
        public void LineTool(Point startPoint, Point endPoint, BlockItem item, bool BGKey)
        {
            ToolConvertUnits(ref startPoint, ref endPoint, false);
            Point realStart = startPoint;
            if (level.EditTool == EditTool.Line)
            {
                int dx = Math.Abs(endPoint.X - startPoint.X);
                int dy = Math.Abs(endPoint.Y - startPoint.Y);

                int sx, sy;

                if (startPoint.X < endPoint.X) sx = 1; else sx = -1;
                if (startPoint.Y < endPoint.Y) sy = 1; else sy = -1;

                int err = dx - dy;

                while (true)
                {
                    PlaceTool(startPoint.X, startPoint.Y, item, BGKey);
                    if (startPoint.X == endPoint.X && startPoint.Y == endPoint.Y)
                        break;

                    int e2 = 2 * err;

                    if (e2 > -dy)
                    {
                        err = err - dy;
                        startPoint.X = startPoint.X + sx;
                    }

                    if (e2 < dx)
                    {
                        err = err + dx;
                        startPoint.Y = startPoint.Y + sy;
                    }
                }
                //return;
            }
        }

        public static void ToolConvertUnits(ref Point start, ref Point end, bool sort = true)
        {
            int startX = start.X / Tile.Width;
            int startY = start.Y / Tile.Height;
            int endX = end.X / Tile.Width;
            int endY = end.Y / Tile.Height;
            if (sort)
            {
                start.X = Math.Min(startX, endX);
                end.X = Math.Max(startX, endX);
                start.Y = Math.Min(startY, endY);
                end.Y = Math.Max(startY, endY);
            }
            else
            {
                start.X = startX = start.X / Tile.Width;
                start.Y = start.Y / Tile.Height;
                end.X = end.X / Tile.Width;
                end.Y = end.Y / Tile.Height;
            }
        }
        private void BushyThingy(int x, int y, int iterations, BlockItem item, int length = 7)
        {
            for (int g = 0; g < iterations; g++)
            {
                int a = x; //Temporary X holder
                int b = y; //Y holder
                int k = random.Next(1, length); //No, It generates 6 at random length :o k.
                for (int j = 0; j <= k; j++) //Add random length too them
                {

                    //Slide to the left... Slide to the right... EVERYBODY CLAP YOUR HANDS! Doop doop doop dooop
                    int c = random.Next(-1, 2); //Choose a random X direction to go 
                    int d = 1; //Go one up

                    level.tiles[a - c, b - d].Foreground = item; //Set it as leaf

                    //If this is the top of the "random wavy thing" then add a "top" to it
                    if (j == k)
                    {   //   []
                        //[] [] [] <- dats what it does, YUP
                        level.tiles[a - c - 1, b - d].Foreground = item;
                        level.tiles[a - c, b - d].Foreground = item;
                        level.tiles[a - c + 1, b - d].Foreground = item;
                        level.tiles[a - c, b - d - 1].Foreground = item;
                    }
                    //Subtract the random directions
                    a -= c;
                    b -= d;

                }
            }
        }
        public void SwampTree(int x, int ranHeight)
        {
            for (int i = 1; i <= ranHeight; i++) //Make base
            {
                level.tiles[x, HeightMap[x] - i] = new Tile(Item.OakTree);
                if (i < ranHeight / 3 || i > (ranHeight / 1.33f))
                {
                    level.tiles[x + 1, HeightMap[x] - i] = new Tile(Item.OakTree);
                    level.tiles[x - 1, HeightMap[x] - i] = new Tile(Item.OakTree);
                }
                if (i == ranHeight)
                {
                    BushTop(x - 3, HeightMap[x] - ranHeight, 2);
                    BushTop(x, HeightMap[x] - ranHeight, 2);
                    BushTop(x + 3, HeightMap[x] - ranHeight, 2);
                    for (int v = -7; v <=7; v++)
                        if (random.Next(0,2) == 0 && level.tiles[x + v, HeightMap[x] - ranHeight ].Foreground == Item.OakLeaf)
                        {
                           // level.tiles[x +v, terrainContour[x] - i + 1].plant = Plants.Vine;
                          //  level.tiles[x + v, terrainContour[x] - i + 1].plantRandom = (sbyte)randomizer.Next(0, 5);
                        }
                
                }
                
            }
           
         
        }
        private void BushTop(int x,int y,int length = 4)
        {
            for (int i = 1; i <= 4; i++)
            {


                int a = x;
                int b = y;

                int k = random.Next(1, length);
                for (int j = 0; j <= k; j++)
                {
                    int c = random.Next(-1, 2);
                    int d = random.Next(2);
                    level.tiles[a - c, b - d] = new Tile(Item.OakLeaf);

                    if (j == k)
                    {
                        level.tiles[a - c - 1, b - d] = new Tile(Item.OakLeaf);
                        level.tiles[a - c, b - d] = new Tile(Item.OakLeaf);
                        level.tiles[a - c + 1, b - d] = new Tile(Item.OakLeaf);
                        level.tiles[a - c, b - d - 1] = new Tile(Item.OakLeaf);
                    }
                    a -= c;
                    b -= d;
                }


            }
        }
        private void MiniBush(int x, int height)
        {
            //Go up the tree and make the base/stem
            for (int i = 1; i <= height; i++)
                level.tiles[x, HeightMap[x] - i].Background = Item.OakTree;
            if (random.NextDouble() < .3f)
            {
                level.tiles[x, HeightMap[x] - height].Foreground = Item.OakLeaf;
            }
            else
            {
                level.tiles[x, HeightMap[x] - height].Foreground = Item.OakLeaf;
                level.tiles[x, HeightMap[x] - height - 1].Foreground = Item.OakLeaf;
                level.tiles[x - 1, HeightMap[x] - height].Foreground = Item.OakLeaf;
                level.tiles[x + 1, HeightMap[x] - height].Foreground = Item.OakLeaf;
            }
        }
        private void Cactus(int x, int ranHeight)
        {
            for (int i = 1; i <= ranHeight / 2; i++)
            {
                level.tiles[x, HeightMap[x] - i] = new Tile(Item.Cactus);
            }
            for (int i = 2; i <= (ranHeight / 2) - 3; )
            {
                bool incremented = false;
                if ((ranHeight / 2) > 4)
                {
                    if (random.Next(1, 3) == 1 && i >= 2 && i <= (ranHeight / 2) - 3)
                    {
                        level.tiles[x - 1, HeightMap[x] - i] = new Tile(Item.Cactus);
                        level.tiles[x - 1, HeightMap[x] - i - 1] = new Tile(Item.Cactus);
                        incremented = true;
                    }
                    if (random.Next(1, 3) == 1&& i >= 2 && i <= (ranHeight / 2) - 3)
                    {
                        level.tiles[x + 1, HeightMap[x] - i] = new Tile(Item.Cactus);
                        level.tiles[x + 1, HeightMap[x] - i - 1] = new Tile(Item.Cactus);
                        incremented = true;
                    }
                    if (incremented)
                        i += 3;
                }

                if (random.Next(1, 3) == 1 && i >= 2 && i <= (ranHeight / 2) - 3)
                {
                    level.tiles[x - 1, HeightMap[x] - i] = new Tile(Item.Cactus);
                    i += 1;
                    incremented = true;
                }
                if (random.Next(1, 3) == 1 && i >= 2 && i <= (ranHeight / 2) - 3)
                {
                    level.tiles[x + 1, HeightMap[x] - i] = new Tile(Item.Cactus);
                    i += 1;
                    incremented = true;
                }
                if (!incremented)
                    i++;

            }
        }
        public bool IsDivisble(int x, int n)
        {
            return (x % n) == 0;
        }
        void Volcano(int width, int height,float percent)
        {
         
          for (int x = 1; x < level.Width - 60; x++) //Go across world
          {
              //If we are in a lava biome, and it picks a random, generate a volcano
              if (random.NextDouble() < percent && CheckBiome(x) == BiomeType.Hell)
              {
                  
                  int[] VolcContour = new int[width];
                  for (int a = 1; a < width; a++)
                  {
                      VolcContour[a] = (int)(Math.Sin((float)Math.PI * ((float)a / (float)width)) * (float)width);
                  }
                  //Start by drawing the triangle shape
                
                  
                  ReGen(noise); //Make a new random perlin noise

                  //Fill the volcano with ash and obsidian
                  for (int a = 1; a < width; a++) 
                  {
                      for (int b = 1; b < VolcContour[a]; b++) //Go up the volcano, until we hit the top
                      {
                              //Add ash
                          level.tiles[(int)x + a, HeightMap[x + a] - b] = new Tile(Item.Ash) { Background = Item.AshBG };
                              //Add obsidian in clumps
                              if (OctaveGenerator(noise,x + a, HeightMap[x + a] - b, 4,2f,.9f,0.04f) > 0.7)
                                  level.tiles[x + a, HeightMap[x + a] - b] = new Tile(Item.Obsidian) { Background = Item.AshBG };

                              //Add lava in clumps
                              if (a > 5 && a < width - 5 && OctaveGenerator(noise,x + a, HeightMap[x + a] - b, 5,1f,1f,0.05f) > 0.8)
                              {
                                  level.tiles[x + a, HeightMap[x + a] - b] = new Tile(Item.Blank) { Background = Item.AshBG };
                                  AddLava(x + a, HeightMap[x + a] - b, 1);
                                 
                              }
                          
                      }
                  }
                  
                  ReGen(noise); //generate new perlin noise
                  //Loop through center of volcano, and add a main hole down
                  for (int a = (width / 2) - 2; a < (width / 2) + 2; a++) 
                  {
                      for (int b = 1; b < VolcContour[a]; b++) 
                      {
                          //Add main hole
                          level.tiles[x + (width / 2), HeightMap[x + (width / 2)] - b] = new Tile(Item.Blank) { Background = Item.AshBG };
                          AddLava(x + (width / 2), HeightMap[x + (width / 2)] - b, 1);
                          if (b == 1) 
                          level.tiles[x + a, HeightMap[x + a] - b] = new Tile(Item.Ash);
                          else if (OctaveGenerator(noise,x + a, HeightMap[x + a] - b, 10, 3f, .9f, 0.2f) > 0.8)
                          {
                              level.tiles[x + a, HeightMap[x + a] - b] = new Tile(Item.Blank) { Background = Item.AshBG };
                              AddLava(x + a, HeightMap[x + a] - b, 3);
                                 
                          }
                      }
                  }
                  //Create "roots" extending out
                  //for (int i = 0; i <= randomizer.Next(3,8);i++)
                  //Line(new Vector2(x +(width / 2), terrainContour[x + (width / 2)] - RareRandom(0,15,2)), new Vector2(x +(width / 2) + randomizer.Next(-(int)(width / 1.5),(int)(width / 1.5)), terrainContour[x + (width / 2)] + randomizer.Next(-(int)(height / 1.5),(int)(height / 1.5))), Item.Blank, false, true,-2,2,2,1);
                  x += width;
              }
           }
        }
      
        void House(int percent, int maxheight, int minheight, int minwave, int maxwave)
        {
            for (int x = 1; x < level.Width - 40; x++)
            {
                if ((random.Next(100) <= percent) && CheckBiome(x) == BiomeType.Desert)
                {
                    // int width;
                    //  LoadAndDraw(Structure.DesertMiniShack, x, terrainContour[x],true,out width);
                    //x += 9;
                }
            }
            for (int x = 1; x < level.Width - 40; x++)
            {
                if ((random.Next(100) <= percent) && (level.tiles[x, HeightMap[x]].Foreground == Item.Grass))
                {
                    int Height = random.Next(minheight, maxheight); //How tall to make it (In blocks)
                    int Wave = random.Next(minwave, maxwave); //How many waves to add (Times 4 for length in blocks)
                    int lengthSoFar = 0; //How long it is so far(In Blocks)
                    int wavesSoFar = 0; //How long it is so far (In Waves)

                        for (int i = 0; i <= Height + 1; i++) //For Height... Go btnUp and make walls/Add stuff
                        {
                        //Add first wall
                        level.tiles[x + (lengthSoFar), HeightMap[x] - i] = new Tile(Item.Brick);
                        level.tiles[x + (lengthSoFar), HeightMap[x]] = new Tile(Item.Stone);

                        //Fill with Background, and add floor (Wave must be * 4 + 4 to convert to blocks)
                        for (int j = 1; j <= Wave * 4 + 4; j++)
                        {
                            //level.tiles[x + j, terrainContour[x] - i] = new Tile(Item.Blank) { background = Item.GrayBrickBG };//BG
                            level.tiles[x + j, HeightMap[x]] = new Tile(Item.Stone);//Floor
                        }
                    }
                    //Add door
                        level.tiles[x + (lengthSoFar), HeightMap[x] - 3] = new Tile(Item.WoodenDoor, x + (lengthSoFar), HeightMap[x] - 2);

                    //Now Go up and make a roof/wave
                    //Go from 0 waves, to Wave // Add 4 (Wave length) to length, and increment wave by 1
                    for (; wavesSoFar <= Wave; lengthSoFar += 4, wavesSoFar++)
                    {
                        //Add blocks for wave pattern (at x + length + how far into the wave, and at y -+ the current waves height
                        //Make Wave
                        level.tiles[x + 1 + (lengthSoFar), HeightMap[x] - Height - 2] = new Tile(Item.Brick);
                        level.tiles[x + 2 + (lengthSoFar), HeightMap[x] - Height - 3] = new Tile(Item.Brick);
                    }
                    for (int i = 0; i <= Height; i++) //Add last wall
                    {
                        //Add end wall
                        level.tiles[x + (lengthSoFar), HeightMap[x] - i] = new Tile(Item.Brick);
                        level.tiles[x + (lengthSoFar), HeightMap[x]] = new Tile(Item.Stone);
                    }
                    //fill in stuff under house
                    for (int a = 0; a <= lengthSoFar; a++) //Fill foundation, Loop through each floor tiles, and go down 
                    {
                        if (HeightMap[x] < HeightMap[x + a])
                        {
                            for (int b = 0; b <= 20;b++) //Go down to terrain countour 
                            {
                                //Stop if we hit a the terrain line
                                if (HeightMap[x] + b == HeightMap[x + a])
                                    break;
                                else
                                    level.tiles[(int)x + a, HeightMap[x] + b] = new Tile(Item.Stone);
                            }
                        }
                    }
                }
            }
        }
        public void AddWater(int x, int y, int amount = 255)
        {
            level.tiles[(int)x, (int)y].WaterMass += amount;
        }
        public void AddLava(int x, int y, byte amount = 255)
        {
            level.tiles[(int)x, (int)y].LavaMass += amount;
        }
        public static void ReGen(PerlinNoise noise)
        {
            noise.generator.NextBytes(noise.source);
            for (int i = 0; i < noise.RANDOM_SIZE; i++)
                noise.values[i + noise.RANDOM_SIZE] = noise.values[i] = noise.source[i];
        }
        public static float OctaveGenerator(PerlinNoise noise,int worldX, PerlinSettings p)
        {
            return OctaveGenerator(noise, worldX, p.Octaves, p.Amplitude, p.Persistance, p.FrequencyX);
        }
        public static float OctaveGenerator(PerlinNoise noise, int worldX, int octaves, float amplitude, float persistance, float frequency)
        {
            float result = 0f;


            for (int i = 0; i < octaves; i++)
            {
                result += noise.Noise(worldX * frequency) * amplitude;
                amplitude *= persistance;
                frequency *= 2.0f;
            }
            if (result < -1) result = -1f;
            if (result > 1) result = 1.0f;
            return result;
        }
        public static float OctaveGenerator(PerlinNoise noise, int worldX, int worldY, PerlinSettings p)
        {
            return OctaveGenerator(noise, worldX, worldY, p.Octaves, p.Amplitude, p.Persistance, p.FrequencyX,p.FrequencyY == 0 ? p.FrequencyX :  p.FrequencyY, p.FrequencyMultiplierX, p.FrequencyMultiplierY); 
        }
        public static float OctaveGenerator(PerlinNoise noise, int worldX, int worldY, int octaves, float amplitude, float persistance, float frequencyX, float frequencyY = 0, float frequencyMultiplierX = 1, float frequencyMultiplierY = 1)
        {
            float result = 0f;


            for (int i = 0; i < octaves; i++)
            {
                result += noise.Noise(worldX * (frequencyX * frequencyMultiplierX), worldY * (frequencyY * frequencyMultiplierY)) * amplitude;
                amplitude *= persistance;
                frequencyX *= 2.0f;
                frequencyY *= 2.0f;
            }

            if (result < -1) result = -1f;
            if (result > 1) result = 1.0f;
            return result;
        }
    }
    public enum Generator
    {
        Planet,
        Flat
    }
    public class Biome
    {
        public BiomeType Type;
        public int Start;
        public int End;

        public Biome(BiomeType type, int start, int end)
        {
            Type = type;
            Start = start;
            End = end;
        }
    }
}

         
