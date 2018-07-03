#region Using
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Cyral.Extensions;
using Cyral.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using ZarknorthClient.Entities;
using ZarknorthClient.Interface;
#endregion

namespace ZarknorthClient
{
    public partial class Level : IDisposable
    {
        #region Properties
        public GameMode Gamemode = GameMode.Survival;
        public Point LastPoint;
        public bool HeldMode;
        public bool generating;
        public float Gravity { get; set; }
        public Generator Generator;
        public TileWrapper tiles; //Array of tiles
        public bool ComputeLighting = false;
        public static bool FullBright;
        public bool ready; //Is the level ready for action?
        public Thread GenerateThread, LoadThread; //Thread to generate the level terrain and stuff
        public Dictionary<string, SoundEffect> soundContent; //Content of everything in the sound folder
        public ParticleEngine DefaultParticleEngine;
        public ParticleEngine AdditiveParticleEngine;
        public List<GameObject> clouds; //List of clouds in the background
        public List<PlayerCharacter> Players; // Entities in the level.
        public List<Texture2D> cloudTextures; //Clouds
        public List<Npc> Npcs = new List<Npc>(); 
        public List<CollectableItem> Collectables = new List<CollectableItem>(); //List of pick-up-able items (dropped items)
        public List<PhysicsEntity> PhysicsEntities = new List<PhysicsEntity>();
        public List<Wire> Wires = new List<Wire>();
        public LevelData Data;
        public Liquid LiquidManager; //Liquid Physics
        public WorldGen worldGen; //World Generator
        public Texture2D MapTexture;
        public Texture2D skyTexture; //Sky texture
        public Texture2D skyboxTexture; //The "Skybox" texture for colors of the sky
        public Texture2D sunTexture; //Sun texture
        public Texture2D moonTexture; //Moon texture
        public Color skyColor; //Color of the sky (current Time)
        public Color[] skyColors; //Colors of the sky for different times
        public float elapsedTime; //Elapsed time in milliseconds
        public bool isDay = true; //If its day of not
        public const int MaxTime = 24 * 60 * 60; //Hours in a day
        public float MinutesPerDay { get { return minutesPerDay * Gravity; } } //How long a real life zarknorth day lasts
        private float minutesPerDay = 12f;
        private LayerManager[] layers; //layer managers
        private BiomeType LastBiome; //Biome we were last in (PER BIOME)
        private BiomeType PreviousBiome; //Last biome we were in, useful for blending layers (PER FRAME)
        private int layerAlpha = 255; //Again, for blending layers
        public EditType EditType = EditType.Place; //Current tool for editing
        public EditTool EditTool = EditTool.Default;
        public MouseState currentMouseState; //Current mouse state
        public MouseState lastMouseState; //Mouse state behind one frame  
        public Vector2 SpawnPoint; //Spawn position
        public Vector2 BedSpawnPoint;
        public float wind; //Wind
        public Random random; //Random used for all things in the level, set seed in contructor
        public Vector2 startPos; //For making lines, start of line
        public Vector2 endPos; //End of line
        public bool isMakingWires; //CURRENTLY making a wire?
        public Wire currentWire;
        public int lastPoint; //Last wire point
        public bool isNextBegin;
        public bool Server; //Is the level currently running as a server?
        public static Texture2D lightMap;
        public Game game;
        public LevelCamera MainCamera;
        public List<FloatLabel> Points;
        public List<GameObject> Stars;
        public int paintColor;
        public Interface.MainWindow MainWindow;
        public Texture2D Atlas;
        public Tile blankTile;
        public int WeatherChangesPerDay = 4;
        public double LastWeatherChange;
        public WeatherType CurrentWeather = WeatherType.Normal;
        public WeatherType LastWeather = WeatherType.Normal;
        private float starColorMultiplyer;
        private double lastSnow;
        public static ContentManager Content //Content manager
        {
            get { return content; }
        }
        static ContentManager content;
        private double nextLightningBolt;
        private double lastLightningBolt;
        private const int MaxClouds = 5;
        private const int MaxStars = 50;
        Color[] LightingBufferData;
        BlendState Multiply = new BlendState()
        {
            AlphaSourceBlend = Blend.DestinationAlpha,
            AlphaDestinationBlend = Blend.Zero,
            AlphaBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.DestinationColor,
            ColorDestinationBlend = Blend.Zero,
            ColorBlendFunction = BlendFunction.Add,
        };
        List<BranchLightning> lightningBolts = new List<BranchLightning>();
        public FadableSoundEffectInstance RainSound;
        public float StormMultiplyer = 1f;
        public bool FadingStormOut;
        public Fire FireManager;
        public FallingBlocks FallingBlocksManager;
        private double lastFireParticleSpawn;
        private const int fireParticleSpawnFrequency = 100;
        public Vector2 AbsolutePosition;
        public Vector2 LastAbsolutePosition;
        public Item selectedItem;
        public Item lastSelectedItem;
        public TaskFactory TaskFactory { get; private set; }
        private List<Task> tasks = new List<Task>();
        public List<Npc> NCPs = new List<Npc>(); // Just ingore this one       
        public Vector2 backgroundPosition;
        public ActionQueue Actions;
        public bool IsMap = false;
        private double lastMinimapUpdate;
        public PlayerCharacter Player
        {
            get { return Players[0]; }
            set { Players[0] = value; }
        }
        public Npc Ncp
        {
            get { return Npcs[0]; }
            set { Npcs[0] = value; }
        }
        public int SelectedPaintColor = 0;
        private float lastLiquidUpdate, liquidUpdateRate = 32;
        public System.Timers.Timer AutoSaver;
        public bool AutoSaving;
        public bool saveOnLoad;
        #endregion

        #region Constructors
        /// <summary> 
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="fileStream">A stream containing the tile data. </param>
        ///<param name="levelIndex"></param>
        ///<param name="generate">Should we generate a level? or load it from the stream?</param>
        public Level(IServiceProvider serviceProvider, Game game)
        {
            Data = new LevelData();
            game.Manager.Cursor = game.Manager.Skin.Cursors["Busy"].Resource;
            tiles = new TileWrapper(0,0);
            Init(serviceProvider, game, 0, 0, 0);

            LoadThread = new Thread(delegate()
            {
                LoadThreaded(game);
                worldGen = new WorldGen(this, Data.Seed);
                Tile.TileWrapper = tiles;
                tiles.level = this;
                tiles = IO.LoadLevel(this, IsMap);
                tiles.SetTile += new TileSetEventHandler(delegate(object o, int x, int y)
                {
                    ComputeLighting = true;
                    ForceRecalcLighting = true;
                });
                Tile.TileWrapper = tiles;
                tiles.level = this;
                this.Width = tiles.Width;
                this.Height = tiles.Height;
                foreach (Wire w in Wires)
                {
                    w.Connection1 = tiles[w.X1, w.Y1];
                    w.Connection2 = tiles[w.X2, w.Y2];
                    if (w.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
                        (w.Connection1 as ElectronicTile).Inputs[w.ConnectionID1].Add(w);
                    else if (w.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Output)
                        (w.Connection1 as ElectronicTile).Outputs[w.ConnectionID1].Add(w);

                    if (w.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
                        (w.Connection2 as ElectronicTile).Inputs[w.ConnectionID2].Add(w);
                    else if (w.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Output)
                        (w.Connection2 as ElectronicTile).Outputs[w.ConnectionID2].Add(w);
                }
               // Npcs[0].Position = new Vector2((Width / 2) * Tile.Width, (worldGen.HeightMap[Width / 2] - 2) * Tile.Width);
                MainCamera = new LevelCamera(this, Players[0].Position - new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2));
                Players[0].Reset(Players[0].Position);
            
                ready = true;
                Game.UniverseViewer.State = UniverseViewer.TransitionState.ClosingToLevel;
                Game.CurrentGameState = Game.GameState.InGame;
                ComputeLighting = true;
                ForceRecalcLighting = true;
                CalculateSkyLight((float)elapsedTime / (MaxTime));
            });
            LoadThread.Start();
        }
        /// <summary> 
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="fileStream">A stream containing the tile data. </param>
        ///<param name="levelIndex"></param>
        ///<param name="generate">Should we generate a level? or load it from the stream?</param>
        public Level(IServiceProvider serviceProvider, Game game, int width, int height, int seed)
        {
            Data = new LevelData();
            Width = width;
            Height = height;

            Data.Seed = seed;

            tiles = new TileWrapper(width, height);
            tiles.SetTile += new TileSetEventHandler(delegate(object o, int x, int y)
            {
                ComputeLighting = true;
                ForceRecalcLighting = true;
            });
            tiles.level = this;
            Tile.TileWrapper = tiles;
            Init(serviceProvider, game, width, height, seed);

            GenerateThread = new Thread(delegate()
            {
                LoadThreaded(game);
                generating = true;
                Generate();
                generating = false;
            });
            GenerateThread.Start();

            elapsedTime = 7000; //Set it to mid morning
        }

        private void Init(IServiceProvider serviceProvider, Game game, int width, int height, int seed)
        {
            this.game = game;
            Game.ready = false;
            Actions = new ActionQueue();
            content = new ContentManager(serviceProvider, "Content");
            LiquidManager = new Liquid(this); //new liquid manager
            //Create a new list of players in the world
            Players = new List<PlayerCharacter>();
            Npcs = new List<Npc>();
            Points = new List<FloatLabel>();
            TaskFactory = new TaskFactory();
            BedSpawnPoint = Vector2.Zero;
            Stars = new List<GameObject>();
            soundContent = IO.LoadListContent<SoundEffect>(content, "Sounds");
            BlurEffect = Content.Load<Effect>("Effects\\Blur");
            RainSound = soundContent["Rain"].CreateFadableInstance();
            RainSound.BaseInstance.IsLooped = true;
            Players.Add(new PlayerCharacter(this, Vector2.Zero) { Name = Game.UserName });
            Player = IO.LoadPlayer(Player);
 
            //Autosave every 10 mins
            AutoSaver = new System.Timers.Timer(10000 * 60);
            AutoSaver.Elapsed += AutoSave;
            AutoSaver.Enabled = true;
            //Npcs.Add(new Person(this, Vector2.Zero));
            ComputeLighting = true;

            //Create a new list for particle emmitters/engines
            DefaultParticleEngine = new ParticleEngine(Content, this);
            AdditiveParticleEngine = new ParticleEngine(Content, this);

            clouds = new List<GameObject>();
            FireManager = new Fire(this);
            FallingBlocksManager = new FallingBlocks(this);
            random = new Random(seed); //new randomizer
            LastBiome = PreviousBiome = BiomeType.Forest;

            //if (generate)
            //{
            //}
            //else
            //{
            //    SpawnPoint = Extensions.GetBottomCenter(GetBounds((int)(pos.X / Tile.Width), (int)(pos.Y / Tile.Width)));
            //    Players[0].Position = pos;
            //    Players[0].Reset(Players[0].Position);
            //    Npcs[0].Position = pos;
            //    MainCamera = new LevelCamera(this, Players[0].Position - new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2));

            //}
        }

        public void AutoSave(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Game.Autosave && ready && !AutoSaving)
            new Thread((ThreadStart)delegate {
                    MainWindow.btnHome.Enabled = false;
                    MainWindow.AutoSaveLvl.Visible = true;
                    AutoSaving = true;
                    IO.SaveLevel(this, false);
                    IO.SavePlayer(Player);
                    AutoSaving = false;
                    MainWindow.AutoSaveLvl.Visible = false;
                    MainWindow.btnHome.Enabled = true;
            }).Start();
        }

        private void LoadThreaded(Game game)
        {
            SetItems(game.GraphicsDevice); //Set minimap colors and ID's
            // Load background layer textures. We get how many biomes, and make a new LayerManager for each
            layers = new LayerManager[BiomeType.BiomeTypes.Count + 1];
            for (int i = 1; i < BiomeType.BiomeTypes.Count + 1; i++) //For every biome
            {
                string name = BiomeType.BiomeTypes[i - 1].Name; //Get biome name

                //if (name == "Snow" || name == "Lush Desert" || name == "Chaparral" || name == "Plains") //Use forest for snow backgrounds
                layers[i] = new LayerManager(Content, "ForestLayer", new Vector2(0.2f, 0f), new Vector2(0.5f, 0f), new Vector2(0.8f, 0f));
                //  else //Load image
                // layers[i] = new LayerManager(Content, "Backgrounds/" + name + "Layer", new Vector2(0.2f, 0f), new Vector2(0.5f, 0f), new Vector2(0.8f, 0f));

            }
            //Load sky textures
            sunTexture = ContentPack.Textures["environment\\sun"];
            moonTexture = ContentPack.Textures["environment\\moon"];
            skyboxTexture = ContentPack.Textures["environment\\skybox"]; //Sky lighting
            cloudTextures = new List<Texture2D>(); //Load clouds
            for (int i = 0; i <= 4; i++)
                cloudTextures.Add(ContentPack.Textures["environment\\cloud" + i]);
            for (int i = 0; i < MaxClouds; i++)
                SpawnCloud(true);
            for (int i = 0; i < MaxStars; i++)
                SpawnStar(true);
        }
        private int GetObjectSize(object TestObject)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            byte[] Array;
            bf.Serialize(ms, TestObject);
            Array = ms.ToArray();
            return Array.Length;
        }
        /// <summary>
        /// Loads or generates the level terrain
        /// </summary>
        private void Generate()
        {
            saveOnLoad = true;
            //Generate world
            worldGen = new WorldGen(this, Data.Seed);
            worldGen.Generate(Generator);
            SpawnPoint = Extensions.GetBottomCenter(GetBounds(Width / 2, worldGen.HeightMap[Width / 2] - 2));
            Players[0].Position = new Vector2((Width / 2) * Tile.Width, (worldGen.HeightMap[Width / 2] - 2) * Tile.Width);
            // Npcs[0].Position = new Vector2((Width / 2) * Tile.Width, (worldGen.HeightMap[Width / 2] - 2) * Tile.Width);
            MainCamera = new LevelCamera(this, Players[0].Position - new Vector2(game.GraphicsDevice.Viewport.Width / 2, game.GraphicsDevice.Viewport.Height / 2));
            Players[0].Reset(Players[0].Position);

            ready = true;
            Game.UniverseViewer.State = UniverseViewer.TransitionState.ClosingToLevel;
            Game.CurrentGameState = Game.GameState.InGame;
        }

        public void SpawnCollectable(int x, int y, Slot slot)
        {
            SpawnCollectable(x, y, slot, new Vector2(random.NextFloat(-200, 200), random.NextFloat(-200, -500)));
        }
        /// <summary>
        /// Instantiates a pickup-able item and puts it in the level.
        /// </summary>
        public void SpawnCollectable(int x, int y, Slot slot, Vector2 velocity)
        {
            Point position = GetBounds(x, y).Location;
            SpawnCollectableAbsolutePos(slot, velocity, position);
        }

        public void SpawnCollectableAbsolutePos(Slot slot, Vector2 velocity, Point position)
        {
            if (slot.Item != Item.Blank)
            {
                //For items close to gether and with stacks more than a 1/4th of their maxPoints stack, join them together to save space
                for (int i = 0; i < Collectables.Count; i++)
                    if (Collectables[i].Slot.Item == slot.Item)
                        if (Collectables[i].Slot.Stack + slot.Stack <= slot.Item.MaxStack && Collectables[i].Slot.Stack + slot.Stack > slot.Item.MaxStack / 4 && Vector2.Distance(Collectables[i].Position, new Vector2(position.X, position.Y)) <= Tile.Width * 2)
                        {
                            Collectables[i].Slot.Stack += slot.Stack;
                            return;
                        }
                //Spawn item
                Collectables.Add(new CollectableItem(this, new Vector2(position.X, position.Y), velocity, slot));
            }
        }
        /// <summary>
        /// Unloads the level content.
        /// </summary>
        public void Dispose()
        {
            if (MapTexture != null)
                MapTexture.Dispose();
            Content.Unload();
            LightingTarget.Dispose();
            tiles = null;
        }
        #endregion

        #region Bounds and collision
        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the MainCamera.btnLeft or MainCamera.btnRight edges, but allowing things
        /// to jump beyond the MainCamera.Top of the level and fall off the MainCamera.bottom.
        /// </summary>
        public BlockCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (y < 1 || y >= Height - 1)
                return BlockCollision.Impassable;
            BlockCollision col = tiles[x, y].Foreground.Collision;
            if (col == BlockCollision.Platform && tiles[x, y, true].Reference.Y != y)
                col = tiles[x, y, true].Foreground.Collision;
            return col;
        }
        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }
        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width;
        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height;
        #endregion

        #region Update

        /// <summary>
        /// Deletes a block
        /// </summary>
        /// <param name="x">X position (in tiles) to delete</param>
        /// <param name="y">Y position (in tiles) to delete</param>
        /// <param name="pickup">Spawn a pickup item?</param>
        public void DeleteBlock(int x, int y)
        {
            tiles[x, y] = new Tile(Item.Blank);
        }

        /// <summary>
        /// Places a block
        /// </summary>
        /// <param name="x">X position (in tiles) to place</param>
        /// <param name="y">Y position (in tiles) to place</param>
        /// <param name="block"></param>
        /// <returns>If the block was placed</returns>
        public bool PlaceBlock(int x, int y, BlockItem block, bool flip, GameTime currentTime = null, bool CanPlace = false)
        {
            bool BGKey = KeyboardState.IsKeyDown(Game.Controls["Place on Background"]);
            //Check to see if the tile is on the foreground
            bool IsForeground = block is ForegroundBlockItem || (block.ID == Item.Blank.ID && !BGKey);
            //Check to see if it is a large tile (> 1x1)
            bool IsLarge = block.Size != Item.One;

            //Get the position for the args if it is a large tile
            int a = IsLarge ? x : -1;
            int b = IsLarge ? y : -1;

            //Check if its flipable or not and flip it if it needs to be
            flip = block.Flipable ? flip : false;

            //Check for each blocktype and create the appopropriate class
            //If it is an empty foreground block
            if (IsForeground && (!BGKey || block.BackgroundEquivelent == null) && (CanPlaceBlock(x,y, block) || CanPlace))
            {
                //Create a backup of the tile so we can apply the background and other prop to the new tile
                Tile tempTile = (Tile)tiles[x, y].Clone();
                if (tempTile.Foreground.InstaBreak && block.Size == Item.One && tempTile.Foreground != block)
                {
                    if (ready)
                    {
                        for (int i = 0; i < 10; i++)
                            Game.level.DefaultParticleEngine.SpawnItemParticle(ParticleType.ItemFall, (x * Tile.Width) + (Tile.Width / 2), (y * Tile.Height) + (Tile.Height / 2), tempTile.Foreground, Color.White);
                    }
                }
                PlaceForegroundBlock(x, y, block, flip, currentTime, a, b, tempTile);
                return true;
            }
            //If it is a blank background block
            else if ((!IsForeground || BGKey || (block.ID == Item.Blank.ID && BGKey)) && (CanPlaceBlock(x, y, block) || CanPlace))
            {
                if (BGKey && IsForeground && block.BackgroundEquivelent != null)
                tiles[x, y, true].Background = block.BackgroundEquivelent;
                else
                tiles[x, y, true].Background = block;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Places a block
        /// </summary>
        /// <param name="x">X position (in tiles) to place</param>
        /// <param name="y">Y position (in tiles) to place</param>
        /// <param name="block"></param>
        /// <returns>If the block was placed</returns>
        public bool ForcePlaceBlock(int x, int y, BlockItem block, bool flip, GameTime currentTime = null)
        {
            //Check to see if the tile is on the foreground
            bool IsForeground = !(block is BackgroundBlockItem);
            //Check to see if it is a large tile (> 1x1)
            bool IsLarge = block.Size != Item.One;

            //Get the position for the args if it is a large tile
            int a = IsLarge ? x : -1;
            int b = IsLarge ? y : -1;

            //Check if its flipable or not and flip it if it needs to be
            flip = block.Flipable ? flip : false;

            if (!IsOverlappingTile(x, y, block))
            {
                //Check for each blocktype and create the appopropriate class
                //If it is an empty foreground block
                if (IsForeground)
                {
                    //Create a backup of the tile so we can apply the background and other prop to the new tile
                    Tile tempTile = null;
                    if ((Tile)Tile.TileWrapper[x, y] != null)
                        tempTile = (Tile)Tile.TileWrapper[x, y].Clone();
                    PlaceForegroundBlock(x, y, block, flip, currentTime, a, b, tempTile);
                    return true;
                }
                //If it is a blank background block
                else if (!IsForeground)
                {
                    if (Tile.TileWrapper[x, y, true] != null)
                    Tile.TileWrapper[x, y, true].Background = block;
                    return true;
                }
            }
            return false;
        }

        private void PlaceForegroundBlock(int x, int y, BlockItem block, bool flip, GameTime currentTime, int a, int b, Tile tempTile)
        {
            switch (block.SubType)
            {
                default:
                    Tile.TileWrapper[x, y, true] = new Tile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Animated:
                    Tile.TileWrapper[x, y, true] = new AnimatedTile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Electronic:
                    Tile.TileWrapper[x, y, true] = new ElectronicTile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Plant:
                    Tile.TileWrapper[x, y, true] = new PlantTile(block, a, b, flip) { LastTick = (currentTime != null ? currentTime.TotalGameTime.TotalMilliseconds : 0), Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Text:
                    Tile.TileWrapper[x, y, true] = new TextTile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Timer:
                    Tile.TileWrapper[x, y, true] = new TimerTile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Storage:
                    Tile.TileWrapper[x, y, true] = new StorageTile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
                case BlockSubType.Furnace:
                    Tile.TileWrapper[x, y, true] = new FurnaceTile(block, a, b, flip) { Flip = flip, Background = (tempTile != null ? tempTile.Background : Item.Blank), BackgroundPaintColor = (tempTile != null ? tempTile.BackgroundPaintColor : (byte)0) };
                    break;
            }
        }
        /// <summary>
        /// Checks to see if this tile overlaps any other tiles (used for big tiles)
        /// </summary>
        private bool IsOverlappingTile(int x, int y, BlockItem item)
        {
            if (item is ForegroundBlockItem) //If this tile is on the foreground
            {
                if (item.Size != Item.One) //If it is not a single tile
                    for (int a = 0; a < item.Size.X; a++) //Loop through the other tiles it intersects
                        for (int b = 0; b < item.Size.Y; b++)
                            if (item.BlockMap == null)
                            {
                                if (!(Tile.TileWrapper[x + a, y + b].Foreground.ID == BlockItem.Blank.ID || Tile.TileWrapper[x + a, y + b].Foreground.InstaBreak))
                                    return true;
                                else
                                    continue;
                            }
                            else
                            {
                                if (item.BlockMap[a, b] && Tile.TileWrapper[x + a, y + b] != null && !(Tile.TileWrapper[x + a, y + b].Foreground.ID == BlockItem.Blank.ID || Tile.TileWrapper[x + a, y + b].Foreground.InstaBreak))
                                    return true;
                                else
                                    continue;
                            }
                else //If it is a single 1x1 tile, just check if it is blank or not
                    return Tile.TileWrapper[x, y] != null && (!(Tile.TileWrapper[x, y].Foreground.ID == Item.Blank.ID || (Tile.TileWrapper[x, y].Foreground.InstaBreak && Tile.TileWrapper[x, y].Foreground != item)));
            }
            else if (item is BackgroundBlockItem) //If it is background
                return Tile.TileWrapper[x, y, true] != null && Tile.TileWrapper[x, y, true].Background.ID != Item.Blank.ID;
            return false;
        }
        /// <summary>
        /// Checks if a block can be placed in this position
        /// </summary>
        public bool CanPlaceBlock(int gridX, int gridY, BlockItem currentItem)
        {
            if (EditType == EditType.Erase)
                return true;
            bool BGKey = Keyboard.GetState().IsKeyDown(Game.Controls["Place on Background"]);
            if (currentItem is ForegroundBlockItem && BGKey)
            {
                return !IsOverlappingTile(gridX, gridY, currentItem.BackgroundEquivelent) && CanRadius(gridX, gridY, 6) && CorrectPlaceMode(gridX, gridY, currentItem, BGKey);
            }
            else if (currentItem is BackgroundBlockItem && !BGKey)
                return !IsOverlappingTile(gridX, gridY, currentItem) && CanRadius(gridX, gridY, 6) && CorrectPlaceMode(gridX, gridY, currentItem, BGKey);
            else
                return ((!(IsOverlappingTile(gridX, gridY, currentItem) || (currentItem.Collision == BlockCollision.Passable ? false : Players[0].IsOverlappingPlayer(gridX, gridY, currentItem.Size))) && CanRadius(gridX, gridY, 6))) && CorrectPlaceMode(gridX, gridY, currentItem, BGKey);
        }

        private bool CorrectPlaceMode(int gridX, int gridY, BlockItem currentItem, bool BGKey)
        {
            if (Gamemode == GameMode.Sandbox)
                return true;
            bool canPlace = false;
            if (currentItem is ForegroundBlockItem && !BGKey)
            {
                if (currentItem.PlaceMode == BlockPlaceMode.Edge)
                {
                    canPlace = true;
                    for (int i = 0; i < currentItem.Size.X; i++)
                        for (int j = 0; j < currentItem.Size.Y; j++)
                        if (!(tiles[gridX + i, gridY + j].Foreground.Collision != BlockCollision.Passable))
                            canPlace = false;
                    if (tiles[gridX, gridY, true].Background != Item.Blank && tiles[gridX, gridY, true].Background.ForegroundEquivelent != null)
                        canPlace = true;
                    else if ((tiles[gridX - 1, gridY].Foreground.Collision != BlockCollision.Passable && tiles[gridX - 1, gridY].Foreground.Collision != BlockCollision.Platform) || tiles[gridX - 1, gridY].Foreground == currentItem)
                        canPlace = true;
                    else if ((tiles[gridX + 1, gridY].Foreground.Collision != BlockCollision.Passable && tiles[gridX + 1, gridY].Foreground.Collision != BlockCollision.Platform) || tiles[gridX + 1, gridY].Foreground == currentItem)
                        canPlace = true;
                    else if ((tiles[gridX, gridY - 1].Foreground.Collision != BlockCollision.Passable && tiles[gridX, gridY - 1].Foreground.Collision != BlockCollision.Platform) || tiles[gridX, gridY - 1].Foreground == currentItem)
                        canPlace = true;
                    else if (tiles[gridX, gridY + 1].Foreground.Collision != BlockCollision.Passable || tiles[gridX, gridY + 1].Foreground == currentItem)
                        canPlace = true;
                }
                else if (currentItem.PlaceMode == BlockPlaceMode.Bottom)
                {
                    canPlace = true;
                    for (int i = 0; i < currentItem.Size.X; i++)
                        if (!(tiles[gridX + i, gridY + currentItem.Size.Y].Foreground.Collision != BlockCollision.Passable || tiles[gridX + i, gridY + currentItem.Size.Y].Foreground.Collision == BlockCollision.Platform || tiles[gridX + i, gridY + currentItem.Size.Y].Foreground.Collision == BlockCollision.Falling))
                            canPlace = false;
                }
                else if (currentItem.PlaceMode == BlockPlaceMode.AllButTop)
                {
                    if (tiles[gridX, gridY, true].Background != Item.Blank && (tiles[gridX, gridY - 1].Foreground == Item.Blank || tiles[gridX, gridY - 1].Foreground.Collision != BlockCollision.Impassable))
                        canPlace = true;
                    else if (tiles[gridX - 1, gridY].Foreground.Collision != BlockCollision.Passable && tiles[gridX - 1, gridY].Foreground.Collision != BlockCollision.Platform)
                        canPlace = true;
                    else if (tiles[gridX + 1, gridY].Foreground.Collision != BlockCollision.Passable && tiles[gridX + 1, gridY].Foreground.Collision != BlockCollision.Platform)
                        canPlace = true;
                    else if (tiles[gridX, gridY + 1].Foreground.Collision != BlockCollision.Passable)
                        canPlace = true;
                }
                else if (currentItem.PlaceMode == BlockPlaceMode.Hanging)
                {
                    canPlace = true;
                    for (int i = 0; i < currentItem.Size.X; i++)
                        if (!(tiles[gridX + i, gridY - 1].Foreground.Collision != BlockCollision.Passable || tiles[gridX + i, gridY - 1].Foreground.Collision == BlockCollision.Platform))
                            canPlace = false;
                }
            }
            else
            {
                if (tiles[gridX, gridY].Foreground.Collision != BlockCollision.Passable)
                    canPlace = true;
                if ((!tiles[gridX + 1, gridY, true].Background.Clear && tiles[gridX + 1, gridY, true].Background != Item.Blank) || tiles[gridX + 1, gridY, true].Background == currentItem)
                    canPlace = true;
                if ((!tiles[gridX - 1, gridY, true].Background.Clear && tiles[gridX - 1, gridY, true].Background != Item.Blank) || tiles[gridX - 1, gridY, true].Background == currentItem)
                    canPlace = true;
                if ((!tiles[gridX, gridY + 1, true].Background.Clear && tiles[gridX, gridY + 1, true].Background != Item.Blank) || tiles[gridX, gridY + 1, true].Background == currentItem)
                    canPlace = true;
                if ((!tiles[gridX, gridY - 1, true].Background.Clear && tiles[gridX, gridY - 1, true].Background != Item.Blank) || tiles[gridX, gridY - 1, true].Background == currentItem)
                    canPlace = true;
            }
            return canPlace;
        }
        /// <summary>
        /// Determines if the position is within the diameter, for block placing and mining
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="diameter"></param>
        /// <returns></returns>
        public bool CanRadius(int gridX, int gridY, int radius)
        {
            if (Gamemode == GameMode.Sandbox)
                return true;
#if !DEBUG
            return (Vector2.Distance(new Vector2(gridX * Tile.Width, gridY * Tile.Height), new Vector2(Players[0].Position.X, Players[0].Position.Y - (Players[0].BoundingRectangle.Height / 2))) <= radius * Tile.Width);
#else
            return (Vector2.Distance(new Vector2(gridX * Tile.Width, gridY * Tile.Height), new Vector2(Players[0].Position.X, Players[0].Position.Y - (Players[0].BoundingRectangle.Height / 2))) <= radius * Tile.Width * 1.6f);
#endif
        }
        /// <summary>
        /// Checks if the user can mine a block in the position
        /// </summary>
        public bool CanMine(int gridX, int gridY, MineItem currentItem)
        {
            return CanRadius(gridX, gridY, currentItem.Radius);
        }
        /// <summary>
        /// Interacts with a block, ie opening a chest, changing a sign, etc
        /// </summary>
        public void InteractBlock(int x, int y, GameTime gameTime)
        {
            BlockItem block = tiles[x, y].Foreground;
            //Note that some items like pianos might just have there own event handler, instead of the default InteractBlock() here
            switch (block.SubType)
            {
                default:
                    //Default does nothing
                    break;
                case BlockSubType.Animated:
                    //Animated does nothing
                    break;
                case BlockSubType.Electronic:
                    //Animated does nothing
                    break;
                case BlockSubType.Plant:
                    //Plant does nothing
                    break;
                case BlockSubType.Text:
                    {
                        //Text opens a new sign window
                        TaskSign tmp = new TaskSign(game.Manager, new InteractBlockEventArgs(this, x, y));
                        tmp.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
                        tmp.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
                        tmp.Init();
                        game.Manager.Add(tmp);
                        tmp.Show();
                        Achievement.Show(Achievement.ReadSign);
                    }
                    break;
                case BlockSubType.Storage:
                    {
                        //Opens a new chest window
                        TaskStorage tmp = new TaskStorage(game.Manager, ((StorageTile)tiles[x, y]).Slots, block, block.StorageSlots.X, block.StorageSlots.Y);
                        tmp.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
                        tmp.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
                        tmp.Init();
                        game.Manager.Add(tmp);
                        tmp.Show();
                    }
                    break;
            }
        }
        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        /// 
        float LastElapsed;
        KeyboardState KeyboardState;
        public void Update(GameTime gameTime, KeyboardState keyboardState, KeyboardState lastKeyboardState)
        {
            KeyboardState = keyboardState;
            //Reset frame stuff
            lastSelectedItem = selectedItem;
            selectedItem = Players[0].CurrentSlot.Item;
            if (selectedItem != lastSelectedItem) //If item has changed, make hand light up..
                ComputeLighting = true;
            //Get stuff
            BiomeType b = worldGen.CheckBiome((int)(Players[0].Position.X / Tile.Width));
            lastMouseState = currentMouseState;
            LastElapsed = elapsedTime;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentMouseState = Mouse.GetState();

            //Update small tasks (No thread needed)
            UpdateTime(gameTime);
            UpdateWeather(gameTime, b);
            UpdateLightning(gameTime);
            UpdateClouds(gameTime);
            UpdateStars(gameTime);
            UpdatePhysicsEntities(gameTime, ref keyboardState, ref lastKeyboardState);
            UpdatePlayers(gameTime, ref keyboardState, ref lastKeyboardState);
            UpdateNpcs(gameTime, ref keyboardState, ref lastKeyboardState);
            UpdateCollectables(gameTime);
            UpdateWiring(gameTime);
            HandleInput(gameTime);
           // MainWindow.DebugList[8].Value = LiquidManager.Array.tiles.Length;
            //Update CPU intensive processes
            if (gameTime.TotalGameTime.TotalMilliseconds > lastLiquidUpdate + liquidUpdateRate)
            {
                tasks.Add(TaskFactory.StartNew(() =>
                {
                    //Update Lava
                    if (!LiquidManager.IsSettling())
                        LiquidManager.UpdateLava();
                }));
                tasks.Add(TaskFactory.StartNew(() =>
                {
                    //Time update
                    Stopwatch liquidWatch = new Stopwatch();
                    liquidWatch.Start();
                    //Update Water
                    if (!LiquidManager.IsSettling())
                        LiquidManager.UpdateWater();
                    liquidWatch.Stop();
                    if (MainWindow.DebugList != null)
                        MainWindow.DebugList[6].Value = (int)liquidWatch.ElapsedMilliseconds;
                }));
                lastLiquidUpdate = (float)gameTime.TotalGameTime.TotalMilliseconds;
            }
            tasks.Add(TaskFactory.StartNew(() =>
            {
                Stopwatch liquidWatch = new Stopwatch();
                liquidWatch.Start();
                FireManager.Update(gameTime);
                liquidWatch.Stop();
              //  if (MainWindow.DebugList != null)
                  //  MainWindow.DebugList[8].Value = (int)liquidWatch.ElapsedMilliseconds;
            }));
            tasks.Add(TaskFactory.StartNew(() =>
            {
                Stopwatch liquidWatch = new Stopwatch();
                liquidWatch.Start();
                FallingBlocksManager.Update(gameTime);
                liquidWatch.Stop();
                if (MainWindow.DebugList != null)
                    MainWindow.DebugList[9].Value = (int)liquidWatch.ElapsedMilliseconds;
            }));
            tasks.Add(TaskFactory.StartNew(() =>
            {
                Stopwatch liquidWatch = new Stopwatch();
                liquidWatch.Start();
                UpdateAnimation(gameTime);
                liquidWatch.Stop();
                if (MainWindow.DebugList != null)
                    MainWindow.DebugList[7].Value = (int)liquidWatch.ElapsedMilliseconds;
            }));
            tasks.Add(TaskFactory.StartNew(() =>
            {
                UpdateFireParticles(gameTime);
            }));
            //Update Particle engines
           // tasks.Add(TaskFactory.StartNew(() =>
           // {
              //  Stopwatch liquidWatch = new Stopwatch();
                //liquidWatch.Start();
                DefaultParticleEngine.Update(gameTime);
                AdditiveParticleEngine.Update(gameTime);
             //   liquidWatch.Stop();
              //  if (MainWindow.DebugList != null)
                //    MainWindow.DebugList[10].Value = (int)liquidWatch.ElapsedMilliseconds;
          //  }));
            //Run all tasks parallel
            #if DEBUG
            Task.WaitAll(tasks.ToArray());
            tasks.Clear();
            #else
            //In RELEASE, throw away exceptions
            try
            {
                Task.WaitAll(tasks.ToArray());
                tasks.Clear();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error: [Task.WaitAll, Update] " + e.Message);
            }
            #endif
            LastWeather = CurrentWeather;
        }

        private void UpdatePhysicsEntities(GameTime gameTime, ref KeyboardState keyboardState, ref KeyboardState lastKeyboardState)
        {
            foreach (PhysicsEntity phys in PhysicsEntities)
                phys.Update(gameTime, keyboardState, lastKeyboardState);
        }

        private void UpdatePlayers(GameTime gameTime, ref KeyboardState keyboardState, ref KeyboardState lastKeyboardState)
        {
            foreach (PlayerCharacter Player in Players)
            {
                // Pause while the player is dead
                if (!Player.IsAlive)
                    // Still want to perform physics on the player.
                    Player.ApplyPhysics(gameTime);
                else
                    Player.Update(gameTime, keyboardState, lastKeyboardState);
            }
        }

        private void UpdateNpcs(GameTime gameTime, ref KeyboardState keyboardState, ref KeyboardState lastKeyboardState)
        {
            foreach (Npc npc in Npcs)
            {
                // Pause while the player is dead
                if (!npc.IsAlive)
                    // Still want to perform physics on the npc
                    npc.ApplyPhysics(gameTime);
                else
                    npc.Update(gameTime, keyboardState, lastKeyboardState);
            }
        }

        private void UpdateLightning(GameTime gameTime)
        {
            lightningBolts = lightningBolts.Where(x => !x.IsComplete).ToList();
            foreach (var bolt in lightningBolts)
                bolt.Update(gameTime);
        }

        private void UpdateTime(GameTime gameTime)
        {
            //Each day is 20 minutes long
            if (isDay)
                elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds / (MinutesPerDay / .75f);
            else
            elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds / (MinutesPerDay);
            if (elapsedTime >= MaxTime / 2) //If half the day is over, it is now night 
            {
                if (isDay != false)
                    isDay = false;
            }
            if (elapsedTime >= MaxTime) //If day is over, it is day again
            {
                elapsedTime = 0;
                isDay = true;
            }
           
        }

        private void UpdateWeather(GameTime gameTime, BiomeType b)
        {

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (gameTime.TotalGameTime.TotalMilliseconds - LastWeatherChange > (MaxTime) / WeatherChangesPerDay)
            {
                LastWeatherChange = gameTime.TotalGameTime.TotalMilliseconds;
                if (HighEnoughForRain())
                    CurrentWeather = b.Weather.GetWeather();
                else
                    CurrentWeather = WeatherType.Normal;
            }
            if (LastWeather != CurrentWeather)
                OnWeatherChange();
            if (CurrentWeather == WeatherType.Rain)
            {
                if (HighEnoughForRain())
                {
                    for (int i = 0; i < gameTime.ElapsedGameTime.Milliseconds / 2f; i++)
                        DefaultParticleEngine.SpawnParticle(ParticleType.Rain, random.Next(0, Game.ViewportWidth) + (int)MainCamera.Position.X, (int)MainCamera.Position.Y - (Tile.Height * 2));
                }
            }
            if (CurrentWeather == WeatherType.Snow)
            {
                if (gameTime.TotalGameTime.TotalSeconds - lastSnow > 0.025 && HighEnoughForRain())
                {
                     DefaultParticleEngine.SpawnParticle(ParticleType.Snow, random.Next(-400, Game.ViewportWidth + 400) + (int)MainCamera.Position.X, (int)MainCamera.Position.Y - (Tile.Height * 2));
                     lastSnow = gameTime.TotalGameTime.TotalSeconds;
                }
            }
            if (CurrentWeather == WeatherType.Storm)
            {
                if (HighEnoughForRain())
                {
                    if (lastLightningBolt == 0)
                    {
                        lastLightningBolt = gameTime.TotalGameTime.TotalMilliseconds;
                        nextLightningBolt = random.Next(25, 55) * 1000;
                    }
                    if (lastLightningBolt + nextLightningBolt < gameTime.TotalGameTime.TotalMilliseconds)
                    {
                        SpawnBolt();
                        lastLightningBolt = gameTime.TotalGameTime.TotalMilliseconds;
                        nextLightningBolt = random.Next(25, 55) * 1000;
                    }

                    for (int i = 0; i < gameTime.ElapsedGameTime.Milliseconds / 1.5f; i++)
                        DefaultParticleEngine.SpawnParticle(ParticleType.Rain, random.Next(0, Game.ViewportWidth) + (int)MainCamera.Position.X, (int)MainCamera.Position.Y - (Tile.Height * 2));
                }
            }
            if (CurrentWeather == WeatherType.Rain || CurrentWeather == WeatherType.Storm)
            {
                if (StormMultiplyer >= .8f)
                {
                    StormMultiplyer -= elapsed / 10;
                    StormMultiplyer = MathHelper.Clamp(StormMultiplyer, .8f, 1f);

                }
            }
            if (FadingStormOut)
            {
                if (StormMultiplyer <=1)
                {
                    StormMultiplyer += elapsed / 10;
                    StormMultiplyer = MathHelper.Clamp(StormMultiplyer, .8f, 1f);
                }
            }
            RainSound.Update(gameTime);
        }

        private bool HighEnoughForRain()
        {
            return (Player.Position.Y / Tile.Height) < worldGen.HeightMap[(int)(tiles.PerformWorldRepeatLogic(Player.Position.X) / Tile.Width)] + WorldGen.FullBackgoundLevel + (MainWindow.Manager.ScreenHeight / Tile.Height);
        }

        public void SpawnBolt()
        {
            float rand = random.NextFloat(0, 1);
            lightningBolts.Add(new BranchLightning(new Vector2(Game.ViewportWidth * rand, 0), new Vector2((Game.ViewportWidth * rand) + random.Next(-200, 200), (Game.ViewportHeight / 2) + random.Next(-300, 100)), -.5f + rand));
        }

        private void OnWeatherChange()
        {
            if (LastWeather == WeatherType.Normal && (CurrentWeather == WeatherType.Rain || CurrentWeather == WeatherType.Storm))
            {
                RainSound.FadeIn();
                FadingStormOut = false;
            }
            if (CurrentWeather == WeatherType.Normal && (LastWeather == WeatherType.Rain || LastWeather == WeatherType.Storm))
            {
                RainSound.FadeOut();
                FadingStormOut = true;
            }
        }
        private void   HandleInput(GameTime gameTime)
        {
            //Get the current mouse position
            LastAbsolutePosition = AbsolutePosition;
            AbsolutePosition = new Vector2((float)Math.Floor(((double)currentMouseState.X + (double)MainCamera.Position.X)), (float)Math.Floor(((double)currentMouseState.Y + (double)MainCamera.Position.Y)));
            Point GridPosition = new Point((int)(AbsolutePosition.X / Tile.Width), (int)(AbsolutePosition.Y / Tile.Height));
            //Current inventory slot selected
            Slot s = Players[0].Inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected];
                        bool BGKey = KeyboardState.IsKeyDown(Game.Controls["Place on Background"]);
            SetSignTooltip(GridPosition);

            if (AbsolutePosition.X <= 0)
            {
                AbsolutePosition.X -= Tile.Width;
            }
            if (IsMap)
            {
                if (currentMouseState.LeftButton == ButtonState.Pressed && !Game.IsMouseOnControl)
                {
                    if (EditTool != EditTool.Default && !HeldMode)
                    {
                        HeldMode = true;
                        LastPoint = AbsolutePosition.ToPoint();
                    }
                }
                if (currentMouseState.LeftButton == ButtonState.Pressed && !Game.IsMouseOnControl)
                {
                    if (EditTool == EditTool.Default && EditType == EditType.Place)
                    {
                        if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0 && (s.Item is BlockItem))
                        {
                            BlockItem item = (s.Item as BlockItem);
                            if (item.Size == Item.One)
                            {
                                if (MainWindow.SizeTool.Value == 0)
                                    goto Normal;
                                for (int x = -(MainWindow.SizeTool.Value); x <= MainWindow.SizeTool.Value; x++)
                                {
                                    for (int y = -(MainWindow.SizeTool.Value); y <= MainWindow.SizeTool.Value; y++)
                                    {
                                        if (InLevelBounds(GridPosition.X + x, GridPosition.Y + y) && Vector2.Distance(new Vector2(GridPosition.X + x, GridPosition.Y + y), new Vector2(GridPosition.X, GridPosition.Y)) <= MainWindow.SizeTool.Value)
                                        {
                                            worldGen.PlaceTool(GridPosition.X + x, GridPosition.Y + y, item, BGKey);
                                        }
                                    }
                                }
                                return;
                            }
                        }
                    }
                    else if (EditTool == EditTool.Default && EditType == EditType.Erase)
                    {
                        if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0)
                        {
                                for (int x = -(MainWindow.SizeTool.Value); x <= MainWindow.SizeTool.Value; x++)
                                {
                                    for (int y = -(MainWindow.SizeTool.Value); y <= MainWindow.SizeTool.Value; y++)
                                    {
                                        if (InLevelBounds(GridPosition.X + x, GridPosition.Y + y) && Vector2.Distance(new Vector2(GridPosition.X + x, GridPosition.Y + y), new Vector2(GridPosition.X, GridPosition.Y)) <= MainWindow.SizeTool.Value)
                                        {
                                            worldGen.PlaceTool(GridPosition.X + x, GridPosition.Y + y, Item.Blank, BGKey);
                                        }
                                    }
                                }
                                return;
                        }
                    }
                }
                if (EditType == EditType.Place || EditType == EditType.Erase)
                {
                    BlockItem item = (s.Item as BlockItem);
                    if (EditType == EditType.Erase)
                        item =Item.Blank;
                    if (((s.Item is BlockItem && item.Size == Item.One && EditType == EditType.Place) || EditType == EditType.Erase) && HeldMode && currentMouseState.LeftButton == ButtonState.Released && !Game.IsMouseOnControl && (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0))
                    {

                        HeldMode = false;
                        if (EditTool == EditTool.Line)
                        {
                            worldGen.LineTool(LastPoint, AbsolutePosition.ToPoint(), item, BGKey);
                            return;
                        }
                        else if (EditTool == EditTool.Square || EditTool == EditTool.FilledSquare)
                        {
                            worldGen.RectangleTool(LastPoint, AbsolutePosition.ToPoint(), item, BGKey, EditTool == EditTool.FilledSquare);
                            return;
                        }
                        else if (EditTool == EditTool.Circle || EditTool == EditTool.FilledCircle)
                        {
                            worldGen.CircleTool(LastPoint, AbsolutePosition.ToPoint(), item, BGKey, EditTool == EditTool.FilledCircle);
                            return;
                        }
                        else
                        {

                        }
                    }
                    if (EditTool != EditTool.Default)
                        return;
                }
                else if (EditType == EditType.Erase && currentMouseState.LeftButton == ButtonState.Pressed && !Game.IsMouseOnControl)
                    if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0)
                    {
                        for (int x = -(MainWindow.SizeTool.Value); x <= MainWindow.SizeTool.Value; x++)
                        {
                            for (int y = -(MainWindow.SizeTool.Value); y <= MainWindow.SizeTool.Value; y++)
                            {
                                if (InLevelBounds(GridPosition.X + x, GridPosition.Y + y) && Vector2.Distance(new Vector2(GridPosition.X + x, GridPosition.Y + y), new Vector2(GridPosition.X, GridPosition.Y)) <= MainWindow.SizeTool.Value)
                                {
                                    Item.Blank.OnLeftHold(new MouseItemEventArgs(this, (int)AbsolutePosition.X + (x * Tile.Width), (int)AbsolutePosition.Y + (y * Tile.Width), (int)LastAbsolutePosition.X + (x * Tile.Width), (int)LastAbsolutePosition.Y + (y * Tile.Width), new Slot(Item.Blank, 1), gameTime));
                                }
                            }
                        }
                        return;
                    }
            }
          
            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
            {
                foreach (PhysicsEntity p in PhysicsEntities)
                {
                    if (Vector2.Distance(p.Position, Player.Position) <= Tile.Width * 6)
                    {
                        if (new Rectangle((int)p.BoundingRectangle.X, (int)p.BoundingRectangle.Y, (int)p.Item.Size.X, (int)p.Item.Size.Y).Contains(new Point((int)AbsolutePosition.X, (int)AbsolutePosition.Y)))
                        {
                            p.Item.OnClickWorld(new MouseItemWorldEventArgs(this, (int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)LastAbsolutePosition.X, (int)LastAbsolutePosition.Y, s, gameTime, p));
                            return;
                        }
                    }
                }
            }
            if (currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released)
            {
                foreach (PhysicsEntity p in PhysicsEntities)
                {
                    if (Vector2.Distance(p.Position, Player.Position) <= Tile.Width * 6)
                    {
                        if (new Rectangle((int)p.BoundingRectangle.X, (int)p.BoundingRectangle.Y, (int)p.Item.Size.X, (int)p.Item.Size.Y).Contains(new Point((int)AbsolutePosition.X, (int)AbsolutePosition.Y)))
                        {
                            SpawnCollectable(GridPosition.X, GridPosition.Y, new Slot(p.Item, 1));
                            PhysicsEntities.Remove(p);
                            return;
                        }
                    }
                }
            }
            Normal:
            //If HELD down left button
            if (currentMouseState.LeftButton == ButtonState.Pressed && !Game.IsMouseOnControl)
                if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0)
                    s.Item.OnLeftHold(new MouseItemEventArgs(this, (int)AbsolutePosition.X, (int)AbsolutePosition.Y,(int)LastAbsolutePosition.X, (int)LastAbsolutePosition.Y, s, gameTime));
            
            //If HELD down right button
            if (currentMouseState.RightButton == ButtonState.Pressed && !Game.IsMouseOnControl)
                if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0)
                    s.Item.OnRightHold(new MouseItemEventArgs(this, (int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)LastAbsolutePosition.X, (int)LastAbsolutePosition.Y, s, gameTime));


            //If CLICKED right button
            if (currentMouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released && !Game.IsMouseOnControl)
                if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0)
                    s.Item.OnRightClick(new MouseItemEventArgs(this, (int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)LastAbsolutePosition.X, (int)LastAbsolutePosition.Y, s, gameTime));
 
            //If CLICKED left button
            if (currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released && !Game.IsMouseOnControl)
                if (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0)
                    s.Item.OnLeftClick(new MouseItemEventArgs(this, (int)AbsolutePosition.X, (int)AbsolutePosition.Y, (int)LastAbsolutePosition.X, (int)LastAbsolutePosition.Y, s, gameTime));
        }
        /// <summary>
        /// Shows text in a tooltip if the cursor is hovering over a sign
        /// </summary>
        private void SetSignTooltip(Point GridPosition)
        {
            if (InLevelBounds(GridPosition.X, GridPosition.Y) && tiles[GridPosition.X, GridPosition.Y] is TextTile && LastAbsolutePosition != Vector2.Zero)
            {
                //Set control to mouse position and show sign text
                Interface.MainWindow.BlockControl.Left = (int)(AbsolutePosition.X - MainCamera.Position.X) - 8 - 8;
                Interface.MainWindow.BlockControl.Top = (int)(AbsolutePosition.Y - MainCamera.Position.Y) - 26 - 8;
                Interface.MainWindow.BlockControl.ToolTip.Text = (tiles[GridPosition.X, GridPosition.Y] as TextTile).Text;
                Interface.MainWindow.BlockControl.Visible = true;
                //If position changed, resize tooltip
                if (tiles[(int)(LastAbsolutePosition.X / Tile.Width), (int)(LastAbsolutePosition.Y / Tile.Height)].X != tiles[GridPosition.X, GridPosition.Y].X || tiles[(int)(LastAbsolutePosition.X / Tile.Width), (int)(LastAbsolutePosition.Y / Tile.Height)].Y != tiles[GridPosition.X, GridPosition.Y].Y)
                {
                    Interface.MainWindow.BlockControl.ToolTip.ResetSize();
                }
            }
            else
                Interface.MainWindow.BlockControl.Visible = false;
        }

        public void AddSlotFloatingLabel(Slot s, Vector2 Position, Color color)
        {
            foreach (FloatLabel f in Points)
            {
                SlotFloatLabel f2 = null;
                if (f is SlotFloatLabel)
                {
                    f2 = (SlotFloatLabel)f;
                    if (f2.Slot.Item.ID == s.Item.ID && f2.scale > .45f && Math.Abs(f2.Position.X - Position.X) < 200)
                    {
                        f2.Slot.Stack += s.Stack;
                        f2.scale = 2.5f;
                        return;
                    }
                }
            }
            SlotFloatLabel label = new SlotFloatLabel(game.Manager, s, Position, color, new Vector2(random.Next(-5, 5), -9 - random.Next(-5, 5)), 5);
            label.Init();
            label.Text = s.Item.Name;
            if (Points.Count() > 0)
            {
                if (Math.Abs(Points[Points.Count() - 1].Position.Y - label.Position.Y) < 128)
                label.Position.Y = Points[Points.Count() - 1].Position.Y - 32;
            }
            Points.Add(label);
            Game.MainWindow.Add(label);
        }
        public void AddFloatingLabel(string Text, Vector2 Position, Color color)
        {
            FloatLabel label = new FloatLabel(game.Manager, Text, Position, color, new Vector2(random.Next(-5, 5), -9 - random.Next(-5, 5)), 5);
            label.Init();
            //label_TextChanged.Text = Text
            if (Points.Count() > 0)
            {
                if (Math.Abs(Points[Points.Count() - 1].Position.Y - label.Position.Y) < 128)
                label.Position.Y = Points[Points.Count() - 1].Position.Y - 32;
            }
            Points.Add(label);
            Game.MainWindow.Add(label);
        }
        private void UpdateFireParticles(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds > lastFireParticleSpawn + fireParticleSpawnFrequency)
            {
                lastFireParticleSpawn = gameTime.TotalGameTime.TotalMilliseconds;
                for (int y = MainCamera.Top - 12; y < MainCamera.Bottom + 12; ++y) //MainCamera.btnLeft to MainCamera.btnRight
                {
                    for (int x = MainCamera.Left - 12; x < MainCamera.Right + 12; ++x)
                    {
                        if (x > Width - 2 || x < 1 || y > Height - 1 || y < 1)
                            continue;
                        if (tiles[x, y,true].ForegroundFireMeta > 0 || tiles[x, y,true].BackgroundFireMeta > 0)
                        {
                            AdditiveParticleEngine.SpawnParticle(ParticleType.Fire, (x * Tile.Width) + (Tile.Width / 2), (y * Tile.Height) + (Tile.Height / 2));
                        }
                        if (tiles[x,y,true].LavaMass > 128 && random.Next(0,15) == 0)
                        {
                            if (!GetSpriteSheetCompare(x, y-1, x, y, SpriteSheetCompareProperties.Lava))
                            {
                                AdditiveParticleEngine.SpawnParticle(ParticleType.LavaAmbient, (x * Tile.Width) + (Tile.Width / 2) + random.Next(-12, 12), (y * Tile.Height) + (Tile.Height / 2) + random.Next(-12, 12));
                            }
                        }
                        if (tiles[x, y, true].Foreground.ID == Item.Torch.ID)
                        {
                            Rectangle source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Foreground);
                            int Xoffset = 0;
                            if (source == TileSetType.West || source == TileSetType.WestStub || source == TileSetType.NorthWest || source == TileSetType.SouthWest || source == TileSetType.Vertical)
                                Xoffset -= 6;
                            else if (source == TileSetType.East || source == TileSetType.EastStub || source == TileSetType.NorthEast || source == TileSetType.SouthEast)
                                Xoffset += 6;
                            AdditiveParticleEngine.SpawnParticle(ParticleType.Torch, (x * Tile.Width) + (Tile.Width / 2) + Xoffset, (y * Tile.Height) + (Tile.Height / 2) - 3);
                        }
                    }
                }
                //Players on fire emit fire
                for (int i = 0; i < 2; i++)
                Players.Where(p => p.OnFire).ToList().ForEach(p => AdditiveParticleEngine.SpawnParticle(ParticleType.Fire,(int)p.Position.X,(int)p.Position.Y));
            }
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            for (int x = MainCamera.Left; x < MainCamera.Right; ++x)
            {
                for (int y = MainCamera.Bottom - 1; y > MainCamera.Top; --y)
                {

                    if (tiles[x,y].Foreground.SubType == BlockSubType.Electronic)
                    {
                        if (tiles[x, y].Foreground.PluginCheck)
                            if (!(tiles[x, y] as ElectronicTile).PluggedIn)
                                continue;
                        (tiles[x, y] as ElectronicTile).State = (byte)tiles[x, y].Foreground.OnGetState(new GetElectronicStateEventArgs(this, x, y));
                    }
                    else if (tiles[x,y].Foreground.SubType == BlockSubType.Timer)
                    {
                        if (tiles[x, y].Foreground.PluginCheck)
                            if (!(tiles[x, y] as ElectronicTile).PluggedIn)
                                continue;
                       
                        TimerTile t = (tiles[x, y] as TimerTile);
                        if (t.Waiting)
                        {
                            t.WaitTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (t.WaitTime > t.Time)
                            {
                                if (t.Inputs[0].Any(i => i.Powered))
                                    t.State = 1;
                                t.WaitTime = 0;
                                t.Waiting = false;
                            }
                        }
                        else
                        {
                           // if (t.Inputs[0].Count > 0)
                               // if (!t.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(this, t.Inputs[0][0], 0)))
                                 //   t.State = 0;
                            t.WaitTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (t.WaitTime > t.Length)
                            {
                                t.State = 0;
                                t.WaitTime = 0;
                                t.Waiting = true;
                                if (t.Outputs[0].Count > 0)
                                    t.Outputs[0].All(xi => xi.Powered = false);
                            }
                        }

                    }

                    if (tiles[x, y, true].Foreground.FrameCount > 1 && tiles[x, y, true].Foreground.AutoPlay)
                    {
                        AnimatedTile tile = (AnimatedTile)tiles[x, y, true];
                        //Process passing time.
                        tile.time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (tile.time > tile.Foreground.FrameTime)
                        {
                            tile.time -= tile.Foreground.FrameTime;

                            tile.FrameIndex = (byte)(((int)tile.FrameIndex + 1) % tile.Foreground.FrameCount);
                        }
                    }
                    //Grow
                    if (tiles[x, y].Foreground.GrowStages > 0)
                    {
                        if (gameTime.TotalGameTime.TotalMilliseconds - ((PlantTile)tiles[x, y]).LastTick > tiles[x, y].Foreground.GrowTime && ((PlantTile)tiles[x, y]).GrowthStage < tiles[x, y].Foreground.GrowStages)
                        {
                            ((PlantTile)tiles[x, y]).GrowthStage += (int)((gameTime.TotalGameTime.TotalMilliseconds - ((PlantTile)tiles[x, y]).LastTick) / tiles[x, y].Foreground.GrowTime);
                            ((PlantTile)tiles[x, y]).GrowthStage = (int)MathHelper.Clamp(((PlantTile)tiles[x, y]).GrowthStage, 0, tiles[x, y].Foreground.GrowStages - 1);
                            ((PlantTile)tiles[x, y]).LastTick = gameTime.TotalGameTime.TotalMilliseconds;

                        }
                    }
                }
            }
        }
        private void UpdateWiring(GameTime gameTime)
        {
            //Reset all wires
            foreach (Wire wire in Wires)
            {
                if (wire.ConnectionPoint1 != null && wire.ConnectionPoint2 != null && wire.Powered && wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input && wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
                    wire.Powered = false;
                if (wire.ReCalc)
                {
                    wire.Powered = false;
                    wire.ReCalc = false;
                }
            }
            for (int i = 0; i < Wires.Count; i++)
            {
                Wire wire = Wires[i];

                if (wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Output)
                {
                    if ((wire.Connection1 as ElectronicTile).Outputs[wire.ConnectionID1].Any(x => x.Powered))
                        wire.Powered = true;
                    else
                        wire.Powered = wire.Connection1.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(this, wire, 1));
                }
                else if (wire.ConnectionPoint2 != null && wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Output)
                {
                    if ((wire.Connection2 as ElectronicTile).Outputs[wire.ConnectionID2].Any(x => x.Powered))
                        wire.Powered = true;
                    else
                        wire.Powered = wire.Connection2.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(this, wire, 2));
                }
                else if (wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
                {
                    if ((wire.Connection1 as ElectronicTile).Inputs[wire.ConnectionID1].Any(x => x.Powered))
                        wire.Powered = true;
                }
                else if (wire.ConnectionPoint2 != null && wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
                {
                    if ((wire.Connection2 as ElectronicTile).Inputs[wire.ConnectionID2].Any(x => x.Powered))
                        wire.Powered = true;
                }
            }
            for (int i = Wires.Count - 1; i > 0; i--)
            {
                Wire wire = Wires[i];

                if (wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Output)
                {
                    if ((wire.Connection1 as ElectronicTile).Outputs[wire.ConnectionID1].Any(x => x.Powered))
                        wire.Powered = true;
                    else
                        wire.Powered = wire.Connection1.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(this, wire, 1));
                }
                else if (wire.ConnectionPoint2 != null && wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Output)
                {
                    if ((wire.Connection2 as ElectronicTile).Outputs[wire.ConnectionID2].Any(x => x.Powered))
                        wire.Powered = true;
                    else
                        wire.Powered = wire.Connection2.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(this, wire, 2));
                }
                else if (wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
                {
                    if ((wire.Connection1 as ElectronicTile).Inputs[wire.ConnectionID1].Any(x => x.Powered))
                        wire.Powered = true;
                }
                else if (wire.ConnectionPoint2 != null && wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
                {
                    if ((wire.Connection2 as ElectronicTile).Inputs[wire.ConnectionID2].Any(x => x.Powered))
                        wire.Powered = true;
                }
            }
        }
        public void Line(Vector2 start, Vector2 end, BlockItem item, BlockItem bg)
        {
            Vector2 deltaVector = end - start;
            float distance = deltaVector.Length();
            Vector2 direction = deltaVector / new Vector2(distance, distance);
            for (float z = 0; z <= distance; z++)
            {

                Vector2 newPoint = start + direction * (distance * (z / distance));
                if (item != Item.Blank)
                {

                    tiles[(int)newPoint.X, (int)newPoint.Y] = new Tile(item);
                }
                if (bg != Item.Blank)
                {
                    tiles[(int)newPoint.X, (int)newPoint.Y].Background = bg;
                }
            }
        }
        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerKilled(Character killedBy)
        {
            Players[0].OnKilled(killedBy);
        }
        private void OnNpcKilled(Character enemy, Character killedBy)
        {
            enemy.OnKilled(killedBy);
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        public void StartNewLife()
        {
            Players[0].Reset(SpawnPoint);
        }

        #endregion

        #region Draw
        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
          
            BiomeType b;
            //Get the current biome
            b = worldGen.CheckBiome(tiles.PerformTileRepeatLogic((int)Players[0].Position.X / Tile.Width));
            //Get the elapsed time since the last frame
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Scroll the Camera one step closer to the target
            MainCamera.ScrollCamera(elapsed, Players[0].Position,KeyboardState.IsKeyDown(Keys.LeftControl));

            if (MainCamera.Position.X.RoundTo(Tile.Width) != MainCamera.PreviousPosition.X.RoundTo(Tile.Width) || MainCamera.Position.Y.RoundTo(Tile.Height) != MainCamera.PreviousPosition.Y.RoundTo(Tile.Height))
            {
                ComputeLighting = true;
                if (Interface.MainWindow.CraftingWindow != null && Interface.MainWindow.CraftingWindow.Visible)
                    {
                        Interface.MainWindow.CraftingWindow.UpdateItemList(MainWindow, true);
                        Interface.MainWindow.CraftingWindow.UpdateItemPanel(MainWindow);
                    }
            }
            Stopwatch drawWatch = new Stopwatch();
            drawWatch.Start();
            //Calculate the lighting of the tiles on screen
            CalculateLighting(gameTime);

            drawWatch.Stop();
            if (MainWindow.DebugList != null)
                MainWindow.DebugList[5].Value = (int)drawWatch.ElapsedMilliseconds;
           // MainWindow.dbgLabel.Value = (int)drawWatch.ElapsedMilliseconds;
            //Render the lighting to a rendertarget that will be stretched/blurred later
            RenderLightingToRT2D(spriteBatch);
            float Time = (float)elapsedTime / (MaxTime); //Get current time (0 to 1)

            if (!Server)
            {
                CalculateSkyLight(Time);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                //Draw the sky color based on 
                spriteBatch.Draw(skyboxTexture, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), new Rectangle((int)((float)(Time) * skyboxTexture.Width), 0, 1, skyboxTexture.Height), Color.White);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

                //Draw lightning
                foreach (var bolt in lightningBolts)
                    bolt.Draw(spriteBatch);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                DrawStars(spriteBatch);
                Vector2 SunPos = DrawSun(spriteBatch, Time);
                DrawClouds(spriteBatch);

                if (PreviousBiome != b) //If the biomes are different, start fading
                {
                    layerAlpha = 0;
                    LastBiome = PreviousBiome;
                }
                if (b != BiomeType.Background)
                {
                    for (int j = 0; j <= 2; ++j) //Draw the current biomes layer
                        layers[(int)b.ID].layers[j].Draw(spriteBatch, backgroundPosition, Item.Blank.Light);
                }
                else
                    layers[0].layers[0].Draw(spriteBatch, backgroundPosition, Item.Blank.Light);

                if (layerAlpha < 255)
                {
                    layerAlpha += 7; //Add, so its fades
                    if (LastBiome != BiomeType.Background)
                        for (int j = 0; j <= 2; ++j) //Draw the last biomes layer
                            layers[(int)LastBiome.ID].layers[j].Draw(spriteBatch, backgroundPosition, new Color(255 - layerAlpha, 255 - layerAlpha, 255 - layerAlpha, 255 - layerAlpha));
                    else
                        layers[0].layers[0].Draw(spriteBatch, backgroundPosition, new Color(255 - layerAlpha, 255 - layerAlpha, 255 - layerAlpha, 255 - layerAlpha));
                }
                PreviousBiome = b;
                spriteBatch.End();
                Vector2 Origin = new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2.0f, spriteBatch.GraphicsDevice.Viewport.Height / 2.0f);
                Matrix cameraTransform = Matrix.CreateTranslation(new Vector3(-MainCamera.Position, 0f));
           
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, cameraTransform);

                drawWatch = new Stopwatch();
                drawWatch.Start();
                //Calculate the lighting of the tiles on screen
                DrawTiles(spriteBatch, gameTime);

                drawWatch.Stop();
                if (MainWindow.DebugList != null)
                    MainWindow.DebugList[4].Value = (int)drawWatch.ElapsedMilliseconds;

                foreach (CollectableItem p in Collectables)
                {
                    //Check if its on screen
                    if (p.Position.X > MainCamera.Position.X && p.Position.Y > MainCamera.Position.Y &&
                        p.Position.X < MainCamera.Position.X + spriteBatch.GraphicsDevice.Viewport.Width && p.Position.Y < MainCamera.Position.Y + spriteBatch.GraphicsDevice.Viewport.Height)
                        p.Draw(gameTime, spriteBatch);
                }
                if (Game.PlayerView)
                {
                    foreach (PlayerCharacter player in Players)
                        player.Draw(gameTime, spriteBatch);

                    foreach (Npc Ncp in Npcs)
                        Ncp.Draw(gameTime, spriteBatch);
                }

                foreach (PhysicsEntity phEnt in PhysicsEntities)
                    phEnt.Draw(gameTime, spriteBatch);

                Stopwatch particleWatch = new Stopwatch();
                particleWatch.Start();
                DefaultParticleEngine.Draw(gameTime, spriteBatch);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, cameraTransform);
                AdditiveParticleEngine.Draw(gameTime, spriteBatch);
                particleWatch.Stop();
                if (MainWindow.DebugList != null)
                    MainWindow.DebugList[11].Value = (int)particleWatch.ElapsedMilliseconds;
                spriteBatch.End();

                if (!FullBright)
                DrawLightingMask(spriteBatch);
                //CalcMinimap(spriteBatch, gameTime);
            }   
        }

        private void DrawLightingMask(SpriteBatch spriteBatch)
        {
            //Draw Lighting
            EffectPass pass = BlurEffect.CurrentTechnique.Passes[0];
            spriteBatch.Begin(SpriteSortMode.Immediate, Multiply, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
            //Apply the blur (Even though it will be blurred when scaled up) if the lighting settings are on high
            if (Game.LightingQuality == 2)
            {
                float edgeX = (float)Tile.Width / (float)(spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth);
                BlurEffect.Parameters["width"].SetValue(1f);
                float edgeY = (float)Tile.Height / (float)(spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight);
                BlurEffect.Parameters["height"].SetValue(1f);
                BlurEffect.Parameters["edgeX"].SetValue(edgeX);
                BlurEffect.Parameters["edgeY"].SetValue(edgeY);
                pass.Apply();
            }
            spriteBatch.Draw(LightingTarget, new Rectangle((Tile.Width - (int)tiles.PerformWorldRepeatLogic(MainCamera.Position.X) % Tile.Width) - (Tile.Width), (Tile.Height - (int)MainCamera.Position.Y % Tile.Height) - (Tile.Height), LightingTarget.Width * Tile.Width, LightingTarget.Height * Tile.Height), Color.White);
            spriteBatch.End();       
        }

        private Vector2 DrawSun(SpriteBatch spriteBatch, float Time)
        {
            Vector2 SunPos;
            float offset = spriteBatch.GraphicsDevice.Viewport.Width * .2f;
            if (isDay)
            {

                SunPos = new Vector2(offset +(Time * 2) * (spriteBatch.GraphicsDevice.Viewport.Width - (offset * 2)+ sunTexture.Width * 2) - sunTexture.Width, spriteBatch.GraphicsDevice.Viewport.Height - (spriteBatch.GraphicsDevice.Viewport.Height * (float)Math.Sin((Time * 2) * MathHelper.Pi) / 2) - (spriteBatch.GraphicsDevice.Viewport.Height / 2));
                spriteBatch.Draw(sunTexture, SunPos, null, Color.White, elapsedTime / 10000, new Vector2(sunTexture.Width / 2, sunTexture.Height / 2), 1, SpriteEffects.None, 0);
            }
            else
            {
                float t = (Time * 2) - 1;
                SunPos = new Vector2(offset + t * (spriteBatch.GraphicsDevice.Viewport.Width - (offset * 2) + sunTexture.Width * 2) - sunTexture.Width, spriteBatch.GraphicsDevice.Viewport.Height - (spriteBatch.GraphicsDevice.Viewport.Height * (float)Math.Sin(t * MathHelper.Pi) / 2) - (spriteBatch.GraphicsDevice.Viewport.Height / 2));
                spriteBatch.Draw(moonTexture, SunPos, null, Color.White, elapsedTime / 10000, new Vector2(sunTexture.Width / 2, sunTexture.Height / 2), 1, SpriteEffects.None, 0);

            }
            return SunPos;
        }

        private void DrawStars(SpriteBatch spriteBatch)
        {
            foreach (GameObject c in Stars)
            {
                Color color = c.color;
                color.R = (byte)((float)c.color.R * starColorMultiplyer);
                color.G = (byte)((float)c.color.G * starColorMultiplyer);
                color.B = (byte)((float)c.color.B * starColorMultiplyer);
                color.A = (byte)((float)c.color.A * starColorMultiplyer);
                spriteBatch.Draw(c.sprite, c.position, null, color, c.rotation, Vector2.Zero, c.scale, SpriteEffects.None, 0);
            }
        }
        private void DrawClouds(SpriteBatch spriteBatch)
        {
            foreach (GameObject c in clouds)
            {
                spriteBatch.Draw(c.sprite, c.position, null, Item.Blank.Light, 0, Vector2.Zero, 1, c.effects, 0);
            }
        }
        private void UpdateClouds(GameTime gameTime)
        {
            foreach (GameObject c in clouds)
            {
                c.position.X += c.velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (c.position.X > Game.ViewportWidth)
                {
                    clouds.Remove(c);
                    break;
                }
            }
            if (clouds.Count < MaxClouds)
            {
                SpawnCloud();
            }
        }
        private void UpdateStars(GameTime gameTime)
        {
            foreach (GameObject c in Stars)
            {
                c.position += (c.velocity)*(float)gameTime.ElapsedGameTime.TotalSeconds;
                if (c.position.X > Game.ViewportWidth)
                {
                    Stars.Remove(c);
                    break;
                }
            }
            if (Stars.Count < MaxStars)
                SpawnStar();
        }

        private void SpawnStar(bool init = false)
        {
            GameObject c = new GameObject(ContentPack.Textures["environment\\star" + random.Next(0, 11)]);
            c.position.Y = random.Next(0, (int)(game.GraphicsDevice.Viewport.Height / 2.5));
            if (!init)
            c.position.X = -c.sprite.Width;
            else
                c.position.X = random.Next(0, Game.ViewportWidth);
            c.velocity.X = random.NextFloat(.6f, 2f);
            c.rotation = random.NextFloat(.4f, 1f);
            c.scale = 1;
            Color color = Color.White; //This will be the final color of the star
            int RGB = random.Next(0, 10); //We need to choose the color of the star
            switch (RGB)
            {
                default: color = new Color(255, 255, 255); break; //White Star
                case 1: color = new Color(255, 200,200); break; //Red Star
                case 2: color = new Color(255, 200,255); break; //Purple Star
                case 3: color = new Color(200,255,200); break; //Blue Star
                case 4: color = new Color(255,200,255); break; //Yellow Star
            }

            c.color = color;
            Stars.Add(c);
        }

        /// <summary>
        /// Spawns a new cloud
        /// </summary>
        /// <param name="init">If this is a cloud that is first loaded, and needs a random position, if false, just make it start at the left of the screen</param>
        private void SpawnCloud(bool init = false)
        {
            GameObject c = new GameObject(cloudTextures[random.Next(0, 5)]);
            c.position.Y = random.Next(-20, 250);
            if (init)
                c.position.X = random.Next(0, Game.ViewportWidth);
            else
               c.position.X -= c.sprite.Width;
            c.velocity.X = random.NextFloat(.6f,2f);
            clouds.Add(c);
        }
        private void DrawLighting(SpriteBatch spriteBatch)
        {
            //Loop Through every tile on the screen, and draw it along with backgrounds, liquids, etc
            for (int y = MainCamera.Top; y < MainCamera.Bottom; ++y)
            {
                for (int x = MainCamera.Left; x < MainCamera.Right; ++x)
                {
                    Tile tile = tiles[x, y]; //Get the tile
                    spriteBatch.Draw(ContentPack.Textures["items\\BlackFill"], new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height), new Color(tiles[x, y].Light.R, tiles[x, y].Light.G, tiles[x, y].Light.B, 255 - (tiles[x, y].Light.R + tiles[x, y].Light.B + tiles[x, y].Light.B / 3)));
                }
            }
        }
        public bool InLevelBounds(Vector2 position)
        {
            if (((position.Y / (float)Tile.Height) >=1 && (position.Y / Tile.Height) < Height - 1))
                return true;
            else
                return false;

        }
        public bool InLevelBounds(int x, int y)
        {
            if ((y >=1 && y <= Height -1))
                return true;
            else
                return false;

        }
        /// <summary>
        /// Animates each gem and checks to allows the player to collect them.
        /// </summary>
        private void UpdateCollectables(GameTime gameTime)
        {
            //Update each item
            Collectables.ForEach(c => c.Update(gameTime));
        }
        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Stopwatch drawWatch1 = new Stopwatch();
            drawWatch1.Start();
            //Loop Through every tile on the screen, and draw it along with backgrounds, liquids, etc
            RenderTiles(spriteBatch, gameTime);
            //CalcMinimap(spriteBatch);
            drawWatch1.Stop();
            if (MainWindow.DebugList != null)
                MainWindow.DebugList[4].Value = (int)drawWatch1.ElapsedMilliseconds;
        }
        private void DrawFullBackgroundTile(SpriteBatch spriteBatch, Tile tile, Tile tileTrue, Rectangle dest, Rectangle source, Color color, Texture2D texture, int x, int y, float elapsed)
        {
            if (tileTrue.FullBackground && !tileTrue.NoDraw && tileTrue.Background.Clear && (tileTrue.Foreground.Clear || (tileTrue.Foreground.CanFall && tileTrue is FallingTile)))
            {
                source = new Rectangle(0, 0, Tile.Width, Tile.Height * 2);
                dest.Y = (y * Tile.Height) - Tile.Width / 2;
                dest.X = (x * Tile.Width);
                dest.Width = Tile.Width;
                dest.Height = Tile.Height * 2;

                int offset = ((int)MainCamera.Position.X / 6) % Tile.Width;
                source.X += offset;
                spriteBatch.Draw(ContentPack.Textures["backgrounds\\FullBackground"], dest, source, color);
                //source.X -= Tile.Width;
                //spriteBatch.Draw(tileContent["FullBackground"], dest, source, color);
            }
        }
        private void DrawBackgroundTile(SpriteBatch spriteBatch, Tile tile, Tile tileTrue, Rectangle dest, Rectangle source, Color color, Texture2D texture, int x, int y, float elapsed)
        {
            if (tileTrue.Background.ID == Item.Blank.ID && tileTrue.BackgroundSheet == Rectangle.Empty)
            {
                RecalcSurrounding(x, y, SpriteSheetCompareProperties.Background, false, false);
                tileTrue.BackgroundSheet = TileSetType.Center; //Just assign it to something...
            }
            //Only if the background is not blank, and we can see the background through the forground
            if (tileTrue.Background.ID != Item.Blank.ID && !tileTrue.NoDraw && (tileTrue.Foreground.Clear || (tileTrue.Foreground.CanFall && tileTrue is FallingTile)))
            {
                DrawTile(spriteBatch, tile, tileTrue, dest, source, color, texture, x, y, elapsed, true);
                dest.X = dest.Y = source.X = source.Y = 0;
                dest.Height = dest.Width = source.Height = source.Width = 24;
            }
        }
        private void DrawForegroundTile(SpriteBatch spriteBatch, Tile tile, Tile tileTrue, Rectangle dest, Rectangle source, Color color, Texture2D texture, int x, int y, float elapsed)
        {
            if (tileTrue.Foreground.ID == Item.Blank.ID && tileTrue.ForegroundSheet == Rectangle.Empty)
            {
                RecalcSurrounding(x, y, SpriteSheetCompareProperties.Foreground, false, false);
                tileTrue.ForegroundSheet = TileSetType.Center; //Just assign it to something...
            }
            //Only if the background is not blank, and we can see the background through the forground
            if (tile.Foreground.ID != Item.Blank.ID && !tileTrue.NoDraw)
            {
                DrawTile(spriteBatch, tile, tileTrue, dest, source, color, texture, x, y, elapsed, false);
                dest.X = dest.Y = source.X = source.Y = 0;
                dest.Height = dest.Width = source.Height = source.Width = 24;
            }
        }
        private void DrawExtraTile(SpriteBatch spriteBatch, Tile tile, Tile tileTrue, Rectangle dest, Rectangle source, Color color, Texture2D texture, int x, int y, float elapsed, BiomeType b)
        {
            if (!tileTrue.NoDraw)
            {
                if (tileTrue.WaterMass > 0)
                {
                    texture = ContentPack.Textures["spritesheets\\Water"];
                    source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Water);
                    float WaterMass = tileTrue.WaterMass / 255f;
                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height) + (int)((1f - WaterMass) * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = (int)(WaterMass * Tile.Width) + 1;
                    dest.Height = (int)MathHelper.Clamp(dest.Height, 0, Tile.Height);
                    spriteBatch.Draw(texture, dest, source, b.WaterColor * WaterMass);
                }
                if (tileTrue.LavaMass > 0)
                {
                    texture = ContentPack.Textures["spritesheets\\Lava"];
                    source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Lava);
                    
                    float LavaMass = tileTrue.LavaMass / 255f;
                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height) + (int)((1f - LavaMass) * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = (int)(LavaMass * Tile.Width) + 1;
                    dest.Height = (int)MathHelper.Clamp(dest.Height, 0, Tile.Height);
                    spriteBatch.Draw(texture, dest, source, Color.White * LavaMass);
                }
                if (tileTrue.Foreground == Item.Grass)
                {
                    texture = tile.Foreground.Textures[tile.ForegroundVariation + 1];
                    //Get rectangle
                    source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Foreground);
                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = Tile.Height;

                    spriteBatch.Draw(texture, dest, source, b.GrassColor);
                }
                if (tileTrue.Foreground == Item.GrassPlant || tileTrue.Foreground == Item.TallGrassPlant || tileTrue.Foreground == Item.Vine)
                {
                    texture = tileTrue.Foreground.Textures[tileTrue.ForegroundVariation + 1];

                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = Tile.Height;
                    if (tileTrue.Foreground.RenderMode == BlockRenderMode.Single)
                        spriteBatch.Draw(texture, dest, b.GrassColor);
                    else
                    {
                        source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Foreground);
                        texture = tileTrue.Foreground.Textures[1];
                        spriteBatch.Draw(texture, dest, source, b.GrassColor);
                    }
                }
            }
        }
        private void DrawTile(SpriteBatch spriteBatch, Tile tile, Tile tileTrue, Rectangle dest, Rectangle source, Color color, Texture2D texture, int x, int y, float elapsed, bool Background)
        {
            BlockItem itemTrue = (!Background ? tileTrue.Foreground : tileTrue.Background);
            BlockItem item = (!Background ? tileTrue.IsLarge ? tile.Foreground : tileTrue.Foreground : tileTrue.Background);
            Rectangle edges = Rectangle.Empty;
            BlockRenderMode RenderMode = !Background ? tile.Foreground.RenderMode : tileTrue.Background.RenderMode;
            SpriteSheetCompareProperties compareMode = Background ? SpriteSheetCompareProperties.Background : SpriteSheetCompareProperties.Foreground;
            switch (RenderMode)
            {
                case BlockRenderMode.SpriteSheet:
                    if (!Background && tile.Foreground.ID == Item.Vine.ID)
                        return;
                 
                    if (!Background && tile.Foreground.CanFall)
                    {
                        FallingTile fallingTile = tile as FallingTile;
                        if (fallingTile != null)
                        {
                            source = TileSetType.Single;
                            dest.Y = (y * Tile.Height) + (int)fallingTile.Position;
                        }
                        else
                        {
                            source = GetSpriteSheetPositions(x, y, compareMode);
                            dest.Y = (y * Tile.Height);
                        }
                    }
                    else
                    {
                        source = GetSpriteSheetPositions(x, y, compareMode);
                        dest.Y = (y * Tile.Height);
                    }
                    texture = GetTextureForSpriteSheet(tile, tileTrue, texture, x, y, Background, item, ref source);
                    dest.X = (x * Tile.Width);
                    dest.Width = Tile.Width;
                    dest.Height = Tile.Height;
                   
                    color = GetPaintColor(tile, tileTrue, color, Background);
                    DrawTileTexture(spriteBatch, tile, ref dest, ref source, ref color, texture, Background);
                    //Draw fancy blend borders
                    if (tile.Foreground.SmoothBlend != null || tile.Background.SmoothBlend != null)
                    {
                        texture = GetTextureForSpriteSheetBlend(tile, tileTrue, texture, x, y, Background, item, ref source);
                        if (texture != null)
                        {
                            DrawTileTexture(spriteBatch, tile, ref dest, ref source, ref color, texture, Background);
                            //Draw Border
                            texture = GetTextureForSpriteSheetBorder(tile, tileTrue, texture, x, y, Background, item, ref source);
                            if (source != TileSetType.Center)
                            {
                                DrawTileTexture(spriteBatch, tile, ref dest, ref source, ref color, texture, Background);
                            }
                        }
                    }
                    break;

                case BlockRenderMode.Single:
                    if (!Background && tile.Foreground == Item.GrassPlant || tile.Foreground == Item.TallGrassPlant || tile.Foreground == Item.Vine)
                        return;
                    else if (!Background)
                    {
                        if (tile.Foreground.PaintColors != null && tileTrue.ForegroundPaintColor >= 1)
                            texture = item.GrayTextures[tile.Foreground.Variations > 1 ? tile.ForegroundVariation + 1 : 1];
                        else
                            texture = item.Textures[tile.Foreground.Variations > 1 ? tile.ForegroundVariation + 1 : 1];
                    }
                    else if (Background)
                    {
                        if (tile.Background.PaintColors != null && tileTrue.BackgroundPaintColor >= 1)
                            texture = item.GrayTextures[tile.Background.Variations > 1 ? tile.BackgroundVariation + 1 : 1];
                        else
                            texture = item.Textures[tile.Background.Variations > 1 ? tile.BackgroundVariation + 1 : 1];
                    }

                    if (!Background && tileTrue.IsLarge)
                    {
                        if (tile.Flip)
                        {
                            int differenceX = tiles.PerformTileRepeatLogic(tileTrue.X) - tiles.PerformTileRepeatLogic(tile.X);
                            source.X = ((tile.Foreground.Size.X - 1 - differenceX)) * Tile.Width;
                            source.Y = (tiles.PerformTileRepeatLogic(y) - tileTrue.Reference.Y) * Tile.Height;
                        }
                        else
                        {
                            source.X = (tiles.PerformTileRepeatLogic(x) - tiles.PerformTileRepeatLogic(tileTrue.Reference.X)) * Tile.Width;
                            source.Y = (y - tileTrue.Reference.Y) * Tile.Height;
                        }
                    }
                    else
                    {
                        source.X = source.Y = 0;
                    }
                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = Tile.Height;
                    color = GetPaintColor(tile,tileTrue, color, Background);
                    spriteBatch.Draw(texture, dest, source, color,0,Vector2.Zero,tile.Flip ? SpriteEffects.FlipHorizontally: SpriteEffects.None,0);
                    break;
                case BlockRenderMode.Animation | BlockRenderMode.Single:
                case BlockRenderMode.Animation:

                    AnimatedTile ATile = (AnimatedTile)tiles[x, y];
                    texture = item.Textures[tile.ForegroundVariation + 1];

                    if (RenderMode.HasFlag(BlockRenderMode.Single))
                    {
                        if (tileTrue.IsLarge)
                        {
                            source.X = (x - tileTrue.Reference.X) * Tile.Width;
                            source.Y = (y - tileTrue.Reference.Y) * Tile.Height;
                            source.X += ATile.FrameIndex * (item.Size.X * Tile.Width);
                        }
                        else
                            source.X += ATile.FrameIndex * (itemTrue.Size.X * Tile.Width);
                    }
                    else
                    {
                        //Get rectangle
                        source = GetSpriteSheetPositions(x, y, compareMode);
                        source.X += ATile.FrameIndex * (Tile.Width * 4);
                    }

                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = Tile.Height;
                    color = GetPaintColor(tile, tileTrue, color, Background);
                    spriteBatch.Draw(texture, dest, source, color);
                    break;
                case BlockRenderMode.Grown | BlockRenderMode.Single:
                case BlockRenderMode.Grown:

                    PlantTile PTile = (PlantTile)tiles[x, y];
                    texture = item.Textures[tile.ForegroundVariation + 1];

                    if (RenderMode.HasFlag(BlockRenderMode.Single))
                    {
                        if (tileTrue.IsLarge)
                        {
                            source.X = (x - tileTrue.Reference.X) * Tile.Width;
                            source.Y = (y - tileTrue.Reference.Y) * Tile.Height;
                            source.X += PTile.GrowthStage * (itemTrue.Size.X * Tile.Width);
                        }
                        else
                            source.X += PTile.GrowthStage * (itemTrue.Size.X * Tile.Width);
                    }
                    else
                    {
                        //Get rectangle
                        source = GetSpriteSheetPositions(x, y, compareMode);
                        source.X += PTile.GrowthStage * (Tile.Width * 4);
                    }

                    dest.X = (x * Tile.Width);
                    dest.Y = (y * Tile.Height);
                    dest.Width = Tile.Width;
                    dest.Height = Tile.Height;

                    spriteBatch.Draw(texture, dest, source, color);
                    break;
                default:
                    break;
            }
        }

        public static Color GetPaintColor(Tile tile, Tile tileTrue, Color color, bool Background)
        {
            if (!Background)
            {
                if (tile.Foreground.PaintColors != null)
                {
                    if (tileTrue.ForegroundPaintColor == 0)
                        return Color.White;
                    else if (tileTrue.ForegroundPaintColor == 1)
                        return Color.White;
                    else
                        color = tile.Foreground.PaintColors[tileTrue.ForegroundPaintColor - 2];
                }
            }
            else
            {
                if (tile.Background.PaintColors != null)
                {
                    if (tileTrue.BackgroundPaintColor == 0)
                        return Color.White;
                    else if (tileTrue.BackgroundPaintColor == 1)
                        return Color.White;
                    else
                        color = tile.Background.PaintColors[tileTrue.BackgroundPaintColor - 2];
                }
            }
            if (color == Color.Transparent)
                color = Color.White;
            return color;
        }

        private static void DrawTileTexture(SpriteBatch spriteBatch, Tile tile, ref Rectangle dest, ref Rectangle source, ref Color color, Texture2D texture, bool Background)
        {
            if (tile.Flip && !Background)
                spriteBatch.Draw(texture, dest, source, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(texture, dest, source, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }
        private static void DrawTileTexture(SpriteBatch spriteBatch, Tile tile, ref Vector2 dest, ref Rectangle source, Color color, Texture2D texture, bool Background)
        {
            if (tile.Flip && !Background)
                spriteBatch.Draw(texture, new Rectangle((int)dest.X,(int)dest.Y,source.Width,source.Height), source, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            else
                spriteBatch.Draw(texture, new Rectangle((int)dest.X, (int)dest.Y, source.Width, source.Height), source, color, 0, Vector2.Zero, SpriteEffects.None, 0);
        }

        private Texture2D GetTextureForSpriteSheet(Tile tile, Tile tileTrue, Texture2D texture, int x, int y, bool Background, BlockItem item, ref Rectangle source)
        {
            if (tile.Foreground.ID == Item.Grass.ID)
                texture = Item.Dirt.Textures[1];
            else if (tile.Foreground.ID == Item.Pipe.ID)
                texture = ContentPack.Textures["items\\PipeBackground"];
            else if (!Background)
            {
                    if (tile.Foreground.PaintColors != null && tile.ForegroundPaintColor >= 1)
                        texture = item.GrayTextures[tileTrue.Foreground.Variations > 1 ? tile.ForegroundVariation + 1 : 1];
                    else
                    texture = item.Textures[tileTrue.Foreground.Variations > 1 ? tile.ForegroundVariation + 1 : 1];
            }
            else
            {
                if (tile.Background.PaintColors != null && tile.BackgroundPaintColor >= 1)
                    texture = item.GrayTextures[tileTrue.Background.Variations > 1 ? tile.BackgroundVariation + 1 : 1];
                else
                    texture = item.Textures[tileTrue.Background.Variations > 1 ? tile.BackgroundVariation + 1 : 1];
            }
            return texture;
        }
        private Texture2D GetTextureForSpriteSheetBlend(Tile tile, Tile tileTrue, Texture2D texture, int x, int y, bool Background, BlockItem item, ref Rectangle source)
        {
            if (!Background)
            {
                if (tile.Foreground.SmoothBlend != null)
                {
                    if (GetSmoothBlend(x, y, tile.Foreground.SmoothBlend))
                        texture = ContentPack.Textures["spritesheets\\" + tile.Foreground.SmoothBlend.Name + "Blend"];
                    else texture = null;
                }
                else
                    texture = null;
            }
            else
            {
                if (tile.Background.SmoothBlend != null)
                {
                    if (GetSmoothBlend(x, y, tile.Background.SmoothBlend))
                        texture = ContentPack.Textures["spritesheets\\" + tile.Background.SmoothBlend.Name + "Blend"];
                    else texture = null;
                }
                else
                    texture = null;
            }
            return texture;
        }
        private Texture2D GetTextureForSpriteSheetBorder(Tile tile, Tile tileTrue, Texture2D texture, int x, int y, bool Background, BlockItem item, ref Rectangle source)
        {
            if (tile.Foreground.SmoothBlend != null && !Background)
            {
                if (GetSmoothBlend(x, y, tile.Foreground.SmoothBlend))
                {
                    source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Foreground, true, tile.Foreground.SmoothBlend, true, false, true, true);
                    if (source != TileSetType.Center)
                        texture = ContentPack.Textures["spritesheets\\" + tile.Foreground.SmoothBlend.Name + "BlendBorder"];
                }
            }
            else if (tile.Background.SmoothBlend != null && Background)
            {
                if (GetSmoothBlend(x, y, tile.Background.SmoothBlend))
                {
                    source = GetSpriteSheetPositions(x, y, SpriteSheetCompareProperties.Background, true, tile.Background.SmoothBlend, true, false, true, true);
                    if (source != TileSetType.Center)
                        texture = ContentPack.Textures["spritesheets\\" + tile.Background.SmoothBlend.Name + "BlendBorder"];
                }
            }
            return texture;
        }
       
        private void RenderTiles(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Matrix cameraTransform = Matrix.CreateTranslation(new Vector3(-MainCamera.Position, 0f));
            //Init holders
            Tile tile;
            Tile tileTrue;
            Rectangle dest = new Rectangle(0, 0, 24, 24);
            Rectangle source = new Rectangle(0, 0, 24, 24);
            Color tileColor = Color.White;
            Texture2D texture = null;
            for (int x = MainCamera.Left; x < MainCamera.Right; ++x)
            {
                for (int y = MainCamera.Bottom - 1; y > MainCamera.Top - 1; --y)
                {

                    //Reset holder
                    dest.X = dest.Y = source.X = source.Y = 0;
                    dest.Height = dest.Width = source.Height = source.Width = 24;

                    tile = tiles[x, y]; //Get the tile
                    tileTrue = tiles[x, y, true]; //Get the absolute tile

                    DrawFullBackgroundTile(spriteBatch, tile, tileTrue, dest, source, tileColor, texture, x, y, elapsed);
                }
            }
            for (int x = MainCamera.Right - 1; x >= MainCamera.Left; x--)
            {
                BiomeType b = worldGen.CheckBiome(x);
                for (int y = MainCamera.Bottom - 1; y > MainCamera.Top - 1; y--)
                {
                    //Reset holder
                    dest.X = dest.Y = source.X = source.Y = 0;
                    dest.Height = dest.Width = source.Height = source.Width = 24;

                    tile = tiles[x, y]; //Get the tile
                    tileTrue = tiles[x, y, true]; //Get the absolute tile


                    if (!(tile.Foreground.ID == Item.Blank.ID && tileTrue.Background.ID == Item.Blank.ID && tileTrue.WaterMass == 0 && tileTrue.LavaMass == 0))
                    {
                        DrawBackgroundTile(spriteBatch, tile, tileTrue, dest, source, tileColor, texture, x, y, elapsed);
                        DrawForegroundTile(spriteBatch, tile, tileTrue, dest, source, tileColor, texture, x, y, elapsed);
                        DrawExtraTile(spriteBatch, tile, tileTrue, dest, source, tileColor, texture, x, y, elapsed, b);
                    }
                }
            }
            if (Player.CurrentSlot.Item == Item.Wirer)
            {
                DrawWiring(spriteBatch, gameTime);
                for (int x = MainCamera.Left; x < MainCamera.Right; ++x)
                {
                    for (int y = MainCamera.Bottom - 1; y > MainCamera.Top - 1; --y)
                    {
                        tile = tiles[x, y, true]; //Get the tile
                        if (tile.Foreground.SubType == BlockSubType.Electronic || tile.Foreground.SubType == BlockSubType.Timer)
                        {
                            if (tile.Foreground.Inputs != null)
                                foreach (ConnectionPoint cp in tile.Foreground.Inputs)
                                    cp.Draw(spriteBatch, gameTime, tile, x, y);
                            if (tile.Foreground.Outputs != null)
                                foreach (ConnectionPoint cp in tile.Foreground.Outputs)
                                    cp.Draw(spriteBatch, gameTime, tile, x, y);
                        }
                    }
                }
            }
        }

        private void DrawWiring(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Wire wire in Wires)
            {
                wire.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Sets the Item ID, Texture, etc
        /// </summary>
        public void SetItems(GraphicsDevice graphics)
        {
            System.Diagnostics.Debug.WriteLine("Info: Setting up Items...");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            //For all items
            foreach (Item i in Item.ItemList)
            {
                if (i is BlockItem)
                {
                    BlockItem item = i as BlockItem;
                    if (item.Size != Item.One)
                    {
                        if (item.PlaceMode == BlockPlaceMode.Edge)
                        item.PlaceMode = BlockPlaceMode.Bottom;
                        item.BreakNoSupportBottom = true;
                    }
                    if (item.InstaBreak)
                        item.BreakTime = 0;
                    if (i is ForegroundBlockItem)
                    {
                        //Set the BG equivelent
                        IEnumerable<Item> bg = Item.ItemList.Where(x => x is BackgroundBlockItem && x.Name == item.Name + " BG");
                        if (bg.Count() == 1)
                        {
                            item.BackgroundEquivelent = bg.ElementAt(0) as BlockItem;
                            (bg.ElementAt(0) as BlockItem).ForegroundEquivelent = item;
                            BackgroundBlockItem background = (bg.ElementAt(0) as BackgroundBlockItem);
                            item.BackgroundEquivelent = background;
                            if (background.Undefined)
                            {
                                background.BreakTime = item.BreakTime;
                                background.MaxStack = item.MaxStack;
                                background.Description = item.Description;
                                background.Flipable = item.Flipable;
                                background.PaintColors = item.PaintColors;
                                background.RenderMode = item.RenderMode;
                                background.Burnable = item.Burnable;
                                background.Variations = item.Variations;
                            }
                        }
                    }
                }
            }
            foreach (Item i in Item.ItemList)
            {
                if (string.IsNullOrWhiteSpace(i.Name))
                    System.Diagnostics.Debug.WriteLine("Warn: Item \"" + i.ID + "\" has no name defined.");
                if (string.IsNullOrWhiteSpace(i.Description))
                    System.Diagnostics.Debug.WriteLine("Warn: Item \"" + i.Name + "\" (" + i.ID + ") has no description defined.");

                if (i.ID == Item.Blank.ID)
                {
                    //Setup the default blank item
                    Item.Blank.Textures[0] = ContentPack.Textures["items\\BlackFill"];
                    Item.Blank.Textures[1] = ContentPack.Textures["items\\BlackFill"];
                    Item.Blank.MinimapColor = new Color(0, 0, 0, 0);
                    Item.Blank.Clear = true;
                }
                else if (i is BlockItem)
                {
                    //Setup all block items
                    BlockItem item = i as BlockItem;

                    if (item is ForegroundBlockItem || (item is BackgroundBlockItem && item.ForegroundEquivelent == null))
                    {
                        if (((BlockItem)i).Variations > 1) //If the block has variations...
                        {
                            i.Textures = new Texture2D[((BlockItem)i).Variations + 1];
                            for (int j = 0; j < ((BlockItem)i).Variations; j++) //...Loop through each one
                            {
                                string file = "spritesheets\\" + i.Name + j;
                                if (ContentPack.Textures.ContainsKey(file)) //If the texture exists
                                    i.Textures[j + 1] = ContentPack.Textures[file]; //Get Texture
                                else //If it dosent, print error and show default texture
                                {
                                    i.Textures[j + 1] = ContentPack.Textures["spritesheets\\Placeholder"];
                                    Debug.Assert(false, "Missing spritesheet texture for " + item.Name + " (variation " + j + ")");
                                }
                            }
                        }
                        else //If no variations
                        {
                            string file = "spritesheets\\" + i.Name;
                            if (ContentPack.Textures.ContainsKey(file)) //If the texture exists
                                i.Textures[1] = ContentPack.Textures[file]; //Get Texture
                            else //If it dosent, print error and show default texture
                            {
                                i.Textures[1] = ContentPack.Textures["spritesheets\\Placeholder"];
                                Debug.Assert(false, "Missing spritesheet texture for " + item.Name);
                            }
                        }
                    }
                    else if (!ContentPack.BackgroundsConverted && item is BackgroundBlockItem)
                    {
                        //Convert blend edges to background
                        if ((i as BackgroundBlockItem).ForegroundEquivelent.BlendEdge != null && (i as BackgroundBlockItem).ForegroundEquivelent.SmoothBlend == null)
                        {
                            string file = "spritesheets\\" + (i as BackgroundBlockItem).ForegroundEquivelent.Name + "Blend";
                            ContentPack.Textures.Add("spritesheets\\" + i.Name + "Blend", MakeBackground(graphics, item, file));
                            file = "spritesheets\\" + (i as BackgroundBlockItem).ForegroundEquivelent.Name + "BlendBorder";
                            ContentPack.Textures.Add("spritesheets\\" + i.Name + "BlendBorder", MakeBackground(graphics, item, file));
                        }
                        if (((BlockItem)i).Variations > 1) //If the block has variations...
                        {
                            i.Textures = new Texture2D[((BlockItem)i).Variations + 1];
                            for (int j = 0; j < ((BlockItem)i).Variations; j++) //...Loop through each one
                            {
                                string file = "spritesheets\\" + (i as BackgroundBlockItem).ForegroundEquivelent.Name + j;
                                if (ContentPack.Textures.ContainsKey(file)) //If the texture exists
                                {
                                    i.Textures[j + 1] = MakeBackground(graphics, item, file);
                                }
                                else //If it dosent, print error and show default texture
                                {
                                    i.Textures[j + 1] = ContentPack.Textures["spritesheets\\Placeholder"];
                                    Debug.Assert(false, "Missing spritesheet texture for " + item.Name + " (variation " + j + ")");
                                }
                            }
                        }
                        else //If no variations
                        {
                            string file = "spritesheets\\" + (i as BackgroundBlockItem).ForegroundEquivelent.Name;
                            if (ContentPack.Textures.ContainsKey(file)) //If the texture exists
                            {
                                i.Textures[1] = MakeBackground(graphics, item, file);
                            }
                            else //If it dosent, print error and show default texture
                            {
                                i.Textures[1] = ContentPack.Textures["spritesheets\\Placeholder"];
                                Debug.Assert(false, "Missing spritesheet texture for " + item.Name);
                            }
                        }
                    }

                    //Throw error if it dosent contain icon
                    if (!ContentPack.Textures.ContainsKey("items\\" + i.Name) && !(item is BackgroundBlockItem && item.ForegroundEquivelent != null))
                    {
                        ContentPack.Textures.Add("items\\" + i.Name, ContentPack.Textures["items\\Placeholder"]);
                        Debug.Assert(false, "Missing icon texture for " + item.Name);
                    }
                    else if (!ContentPack.BackgroundsConverted && item is BackgroundBlockItem && item.ForegroundEquivelent != null)
                    {
                         ContentPack.Textures.Add("items\\" + i.Name, ContentPack.Textures["items\\Placeholder"]);
                        i.Textures[0] = ContentPack.Textures["items\\" + i.Name];
                    }
                    else
                    {
                        i.Textures[0] = ContentPack.Textures["items\\" + i.Name];
                    }
                    if ((i as BlockItem).PaintColors != null && i is BlockItem)
                    {
                        i.GrayTextures = new Texture2D[i.Textures.Length];
                        for (int e = 0; e < i.Textures.Length; e++)
                        {
                                i.GrayTextures[e] = new Texture2D(graphics, i.Textures[e].Width, i.Textures[e].Height);
                                Color[] bits = new Color[i.Textures[e].Width * i.Textures[e].Height];
                                i.Textures[e].GetData(bits);

                                for (int x = 0; x < bits.Length; x++)
                                {
                                    Color c = bits[x];
                                    int grayScale = (int)((c.R * .3) + (c.G * .59) + (c.B * .11));
                                    c = new Color(grayScale, grayScale, grayScale, c.A);
                                    int C = (int)Math.Pow(((100.0 + 35) / 100.0), 2);

                                    int B = (int)(((((c.B / 255.0) - 0.5) * C) + 0.5) * 255.0);

                                    int G = (int)(((((c.G / 255.0) - 0.5) * C) + 0.5) * 255.0);

                                    int R = (int)(((((c.R / 255.0) - 0.5) * C) + 0.5) * 255.0);
                                    bits[x] = new Color(R, G, B, c.A);
                                }
                                i.GrayTextures[e].SetData<Color>(bits);
                        }
                    }
                    if (!(i is BackgroundBlockItem && (i as BackgroundBlockItem).ForegroundEquivelent != null))
                    {
                        Texture2D t = ContentPack.Textures["items\\" + i.Name]; //Get Icon Texture
                        Color[] data = new Color[t.Width * t.Height];
                        int r = 0;
                        int g = 0;
                        int b = 0;
                        int a = 0;
                        int amount = 0;
                        int amount2 = 0;
                        t.GetData<Color>(data); //Get data 

                        foreach (Color c in data) //Foreach colored pixel; Get RGB values
                        {
                            if (c.A > 1)
                            {
                                r += c.R;
                                g += c.G;
                                b += c.B;
                                a += c.A;
                                amount++;
                            }
                            amount2++;
                        }
                        //Set the minimap color and clear vales
                        if (a < amount2 * 255)
                            item.Clear = true;
                        item.MinimapColor = new Color(r / amount, g / amount, b / amount, a / amount); //Calculate average
                    }
                    else
                        item.MinimapColor = Color.Lerp(item.ForegroundEquivelent.MinimapColor, Color.Black, .5f);

                    //Set the break fall value, if it should break sand/gravel fall
                    if (item is ForegroundBlockItem)
                    {
                        Texture2D tex = item.Textures[1];
                        Color[,] colors = tex.TextureTo2DArray();
                        bool hasSurface = false;
                        int width = item.RenderMode.HasFlag(BlockRenderMode.Single) ? item.Size.X * Tile.Width : Tile.Width * 3;
                        //Loop across to see if the top pixels have alpha on them (therefor it is not a platform)
                        for (int k = 0; k < width; k++)
                        {
                            if (colors[k, 0].A < 255)
                                hasSurface = true;
                        }
                        item.BreakFall = hasSurface;
                    }
                }
                else
                {
                    //Get icon
                    if (!ContentPack.Textures.ContainsKey("items\\" + i.Name))
                    {
                        ContentPack.Textures.Add("items\\" + i.Name, ContentPack.Textures["items\\Placeholder"]);
                        Debug.Assert(false, "Missing item icon texture for " + i.Name);
                    }
                    else
                    {
                        i.Textures[0] = ContentPack.Textures["items\\" + i.Name];
                    }
                }
            }

            //TODO: Add random initilization logic for items
            Item.SilverOre.MinimapColor = Color.Silver;
            Item.QuartzOre.MinimapColor = Color.White;
            Item.CopperOre.MinimapColor = Color.RosyBrown;
            Item.IronOre.MinimapColor = Color.SaddleBrown;
            Item.RubyOre.MinimapColor = Color.Red;
            Item.GoldOre.MinimapColor = Color.Gold;
            Item.DiamondOre.MinimapColor = Color.Blue;

            //Make grass support sand
            Item.Grass.BreakFall = false;

            Item.Dirt.Clear = Item.Stone.Clear = Item.Clay.Clear = false;
            //Set the default empty slot
            Slot.Empty = new Slot(Item.Blank,0);
            ContentPack.BackgroundsConverted = true;
            timer.Stop();
            System.Diagnostics.Debug.WriteLine("Info: Item setup complete! (" + timer.ElapsedMilliseconds + " ms)");

            #region Converter (Converts old sprite layouts to new)
#if false
            foreach (Item i in Item.ItemList)
            {
                if (i is BlockItem)
                {
                    BlockItem item = i as BlockItem;
                    if (item.RenderMode == BlockRenderMode.SpriteSheet && item != Item.Blank && !(item.RenderMode.HasFlag(BlockRenderMode.Animation)))
                    {
                        if (i is BackgroundBlockItem && (i as BackgroundBlockItem).ForegroundEquivelent != null)
                            continue;
                        for (int e = 1; e < i.Textures.Length; e++)
                        {
                            if (i.Textures[e] == null)
                                continue;
                            using (FileStream fileStream = new FileStream(@"C:\Users\Heath_TEMP\AppData\Roaming\.Zarknorth\Content Packs\Kreative\textures\spritesheets\PipeWaterSheet.png", FileMode.Open))
                            {
                                i.Textures[e] = Texture2D.FromStream(graphics, fileStream);
                            }
                            Texture2D Texture = i.Textures[e];
                            Texture2D NewTexture = new Texture2D(graphics, i.Textures[e].Width, i.Textures[e].Height);
                            Color[] bits = new Color[i.Textures[e].Width * i.Textures[e].Height];
                           // Texture.GetData(bits);
                            //NewTexture.SetData<Color>(bits);
                            Color[,] Colors = Extensions.TextureTo2DArray(Texture);
                            Color[,] NewColors = new Color[96, 96];

                            for (int a = 0; a < 96; a++)
                                for (int b = 0; b < 96; b++)
                                    NewColors[a, b] = Color.Pink;

                                for (int x = 0; x < 24; x++)
                                    for (int y = 0; y < 24; y++)
                                        NewColors[x + 24, y + 24] = Colors[x, y];

                                for (int x = 24; x < 48; x++)
                                    for (int y = 0; y < 24; y++)
                                        NewColors[x + 24, y + 24] = Colors[x, y];

                                for (int x = 48; x < 72; x++)
                                    for (int y = 0; y < 24; y++)
                                        NewColors[x - 24, y] = Colors[x, y];
                                for (int x = 72; x < 96; x++)
                                    for (int y = 0; y < 24; y++)
                                        NewColors[x, y] = Colors[x, y];

                                for (int x = 0; x < 24; x++)
                                    for (int y = 24; y < 48; y++)
                                        NewColors[x + 48, y + 48] = Colors[x, y];

                                for (int x = 24; x < 48; x++)
                                    for (int y = 24; y < 48; y++)
                                        NewColors[x, y + 48] = Colors[x, y];
                                for (int x = 48; x < 72; x++)
                                    for (int y = 24; y < 48; y++)
                                        NewColors[x, y - 24] = Colors[x, y];
                                for (int x = 72; x < 96; x++)
                                    for (int y = 24; y < 48; y++)
                                        NewColors[x - 72, y - 24] = Colors[x, y];


                                for (int x = 0; x < 24; x++)
                                    for (int y = 48; y < 72; y++)
                                        NewColors[x + 72, y + 24] = Colors[x, y];
                                for (int x = 24; x < 48; x++)
                                    for (int y = 48; y < 72; y++)
                                        NewColors[x + 24, y] = Colors[x, y];
                                for (int x = 48; x < 72; x++)
                                    for (int y = 48; y < 72; y++)
                                        NewColors[x + 24, y] = Colors[x, y];
                                for (int x = 72; x < 96; x++)
                                    for (int y = 48; y < 72; y++)
                                        NewColors[x - 72, y - 24] = Colors[x, y];


                                for (int x = 0; x < 24; x++)
                                    for (int y = 72; y < 96; y++)
                                        NewColors[x + 24, y - 24] = Colors[x, y];
                                for (int x = 24; x < 48; x++)
                                    for (int y = 72; y < 96; y++)
                                        NewColors[x - 24, y - 24] = Colors[x, y];
                                for (int x = 48; x < 72; x++)
                                    for (int y = 72; y < 96; y++)
                                        NewColors[x + 24, y - 48] = Colors[x, y];
                                for (int x = 72; x < 96; x++)
                                    for (int y = 72; y < 96; y++)
                                        NewColors[x - 72, y] = Colors[x, y];


                            Texture2D texture = ColorArray2DToTexture2D(NewColors, graphics);
                            int width = 96;
                            int height = 96;
                            //Custom saving routine to prevent XNA memory leak
                            //More info at http://stackoverflow.com/a/14310276/1218281
                            using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, PixelFormat.Format32bppArgb))
                            {
                                byte blue;
                                IntPtr safePtr;
                                BitmapData bitmapData;
                                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
                                byte[] textureData = new byte[4 * width * height];

                                texture.GetData<byte>(textureData);
                                for (int id = 0; id < textureData.Length; id += 4)
                                {
                                    blue = textureData[id];
                                    textureData[id] = textureData[id + 2];
                                    textureData[id + 2] = blue;
                                }
                                bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                                safePtr = bitmapData.Scan0;
                                System.Runtime.InteropServices.Marshal.Copy(textureData, 0, safePtr, textureData.Length);
                                bitmap.UnlockBits(bitmapData);
                                if (item.Variations > 1)
                                bitmap.Save(IO.Directories["Content Packs"] + item.Name + (e-1).ToString() + ".png");
                                else
                                bitmap.Save(IO.Directories["Content Packs"] + item.Name + ".png");
                            }
                        }
                    }
                }
            }
#endif
#endregion
        }
        public static Texture2D ColorArray2DToTexture2D(Microsoft.Xna.Framework.Color[,] Colors2D, GraphicsDevice Graphics)
        {
            Texture2D Texture;
            // Figure out the width and height of the new texture,
            // by looking at the dimensions of the array.
            int TextureWidth = Colors2D.GetUpperBound(0) + 1;
            int TextureHeight = Colors2D.GetUpperBound(1) + 1;
            Microsoft.Xna.Framework.Color[] Colors1D = new Microsoft.Xna.Framework.Color[TextureWidth * TextureHeight];

            for (int x = 0; x < TextureWidth; x++)
            {
                for (int y = 0; y < TextureHeight; y++)
                {

                    Colors1D[x + (y * TextureWidth)] = Colors2D[x, y];

                }
            }

            Texture = new Texture2D(Graphics, TextureWidth, TextureHeight, false, SurfaceFormat.Color);
            Texture.SetData(Colors1D);

            return (Texture);
        }
        private static Texture2D MakeBackground(GraphicsDevice graphics, BlockItem item, string file)
        {
            Texture2D tx = ContentPack.Textures[file];
            Texture2D n = new Texture2D(graphics, tx.Width, tx.Height);
            Color[] bits = new Color[n.Width * n.Height];
            tx.GetData(bits);

            for (int x = 0; x < bits.Length; x++)
            {
                Color c = bits[x];
                float C = (float)Math.Pow(((100.0 + 15) / 100.0), 2);

                float B = (float)(((((c.B / 255.0) - 0.5) * C) + 0.5) * 255.0);

                float G = (float)(((((c.G / 255.0) - 0.5) * C) + 0.5) * 255.0);

                float R = (float)(((((c.R / 255.0) - 0.5) * C) + 0.5) * 255.0);
                bits[x] = new Color((R / 2.5f) / 255f, (G / 2.5f) / 255f, (B / 2.5f) / 255f, c.A / 255f);
            }
            n.SetData<Color>(bits);
            return n;
        }

        private Color[,] TwoArray;
        private Color[] OneArray;
        /// <summary>
        /// Calculates the minimap color each frame
        /// </summary>
        [Obsolete("Old minimap function causes memory leaks and is not needed")]
        private void CalcMinimap(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > lastMinimapUpdate + 1)
            {
                if (TwoArray == null)
                {
                    //a 2D color array used to store the primary texture and allow precise data retrieval 
                    TwoArray = new Color[(Math.Min(Width, 4000)), Math.Min(Height, 4000)];
                    //the 1D array that will be used to create the final texture 
                    OneArray = new Color[(int)(((Math.Min(Width, 4000))) * (Math.Min(Height, 4000)))];
                }

                for (int y = 1; y < Math.Min(Height, 4000); ++y)
                {
                    for (int x = 1; x < Math.Min(Width, 4000); ++x)
                    {
                        Tile tile = tiles[x, y];
                        if (tile.Foreground != Item.Blank)
                            TwoArray[x, y] = tile.Foreground.MinimapColor; //Set the color to the items minimap color
                        else if (tiles[x, y].WaterMass > 10)
                            TwoArray[x, y] = Item.Water.MinimapColor; //Water
                        else if (tiles[x, y].LavaMass > 10)
                            TwoArray[x, y] = Item.Lava.MinimapColor; //Lava
                        else if (tile.Foreground.ID == Item.Blank.ID && tile.Background.ID != Item.Blank.ID)
                            TwoArray[x, y] = tile.Background.MinimapColor;
                        else
                            TwoArray[x, y] = skyColor; //Blank

                        //Convert the 2D array to 1D
                        OneArray[x + y * (int)((Math.Min(Width, 4000)))] = TwoArray[x % Math.Min(Width, 4000), y % (Math.Min(Height, 4000))];
                    }
                }

                //create a new, blank, square texture 
                if (MapTexture == null)
                    MapTexture = new Texture2D(spriteBatch.GraphicsDevice, Math.Min(Width, 4000), Math.Min(Height, 4000));

                //assign the newly filled 1D array into the texture 
                MapTexture.SetData<Color>(OneArray);
                
                
             //MapTexture.SaveAsPng(new FileStream(@"C:\Users\Cyral\AppData\Roaming\.Zarknorth\Minimaps\minimap.png", FileMode.Create), MapTexture.Width, MapTexture.Height);
                lastMinimapUpdate = gameTime.TotalGameTime.TotalSeconds;
            }
        }
   
        public int GetLiquidOnPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y - 1) / Tile.Height;

            return tiles[x, y, true].WaterMass;
        }
        public int GetLavaOnPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y - 1) / Tile.Height;

            return tiles[x, y, true].LavaMass;
        }
        public int GetLiquidOnPlayerHead(Character p)
        {
            int x = (int)p.Position.X / Tile.Width;
            int y = (int)(p.Position.Y - p.BoundingRectangle.Height) / Tile.Height;

            return tiles[x, y, true].WaterMass;
        }
        public Tile GetTileOnPlayerHead(Character p)
        {
            int x = (int)(p.Position.X / Tile.Width);
            int y = (int)(p.Position.Y - p.BoundingRectangle.Height) / Tile.Height;

            return tiles[x, y];
        }
        public Tile GetTileAbovePlayer(Character p)
        {
            int x = (int)(p.Position.X / Tile.Width);
            int y = (int)(p.Position.Y - p.BoundingRectangle.Height) / Tile.Height;

            return tiles[x, y - 1];
        }
        public Tile GetTileOnPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y - 1) / Tile.Height;

            return tiles[x, y];
        }
        public Tile GetTileBelowPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y) / Tile.Height;

            return tiles[x, y];
        }
    }
    public enum GameMode
    {
        Survival,
        Sandbox,
    }
    #endregion
}