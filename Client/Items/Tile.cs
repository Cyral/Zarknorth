#region Usings
using System;
using Microsoft.Xna.Framework;
using Cyral.Extensions;
using System.Collections.Generic;
#endregion

namespace ZarknorthClient
{
    /// <summary>
    /// A single tile in a grid that contains foregrounds, backgrounds, and other properties
    /// </summary>
    public class Tile
    {
        public static void SetBit(ref byte aByte, int pos, bool value)
        {
            if (value)
            {
                //left-shift 1, then bitwise OR
                aByte = (byte)(aByte | (1 << pos));
            }
            else
            {
                //left-shift 1, then take complement, then bitwise AND
                aByte = (byte)(aByte & ~(1 << pos));
            }
        }

        public static bool GetBit(byte aByte, int pos)
        {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((aByte & (1 << pos)) != 0);
        }

        #region Global Variables
        /// <summary>
        /// The width of a standard tile.
        /// </summary>
        public static int Width { get { return 24; } }
        /// <summary>
        /// The height of a standard tile.
        /// </summary>
        public static int Height { get { return 24; } }
        /// <summary>
        /// The center point (same for X and Y axis) of the origin of a standard tile.
        /// </summary>
        public static int Center { get { return 12; } }
        /// <summary>
        /// Random instance used for choosing random variations and other such data.
        /// </summary>
        protected static Random random;
        #endregion

        #region Properties
        public Rectangle ForegroundSheet
        {
            get { return TileSetType.Rectangles[foregroundSheetID]; }
            set
        {
            for (int i = 0; i < TileSetType.Rectangles.Length; i++)
                if (TileSetType.Rectangles[i].X == value.X && TileSetType.Rectangles[i].Y == value.Y && TileSetType.Rectangles[i].Width == value.Width) 
                    foregroundSheetID = (byte)i;
        }}
        private byte foregroundSheetID;
        public Rectangle BackgroundSheet
        {
            get { return TileSetType.Rectangles[backgroundSheetID]; }
            set
            {
                for (int i = 0; i < TileSetType.Rectangles.Length; i++)
                    if (TileSetType.Rectangles[i].X == value.X && TileSetType.Rectangles[i].Y == value.Y) 
                        backgroundSheetID = (byte)i;
            }
        }
        private byte backgroundSheetID;
        /// <summary>
        /// The reference to a <c>TileWrapper</c> class, used for interacting with tiles around this one.
        /// </summary>
        public static TileWrapper TileWrapper;

        /// <summary>
        /// Defines if the tile is a "large" tile, meaning that the tile occupies more than 1 standard grid area,
        /// therefor has references to it and may need additional logic.
        /// </summary>
        public bool IsLarge { get { return GetBit(Data, 0); } private set { SetBit(ref Data, 0, value); } }
        /// <summary>
        /// The foreground <c>[Foreground]BlockItem</c> on the tile.
        /// </summary>
        public virtual BlockItem Foreground
        {
            get { return foreground; }
            set
            {
                CacheIsLarge();
                //Notify the level or tile wrapper that we modified the fg
                if (TileWrapper != null)
                    TileWrapper.OnSetTile(X, Y);
                //If this (or reference) is a "large" block, and we placed a new block on it, remove old references to this block
                if (value != foreground && IsLarge)
                    Reference.RemoveReferences();
                //Set the foreground to the new value
                foreground = value;
                if (foreground.AutoFlipVariation)
                    Flip = random.NextBoolean();
                //If it has variations, select one
                VariateForeground();
                //Lastly, reset the paint color
                ForegroundPaintColor = 0;
                //If this is a larger tile we need to set the other tiles to point to it
                SetReferences(X, Y);
                ForegroundSheet = Rectangle.Empty;

                if (Game.level != null && (Game.level.ready || Game.level.generating) && x > 1 && y > 1 && x < TileWrapper.level.Width - 1 && y < TileWrapper.level.Height - 1)
                {
                    TileWrapper.level.tiles[x + 1, y, true].WaterIdle = false;
                    TileWrapper.level.tiles[x - 1, y, true].WaterIdle = false;
                    TileWrapper.level.tiles[x, y + 1, true].WaterIdle = false;
                    TileWrapper.level.tiles[x, y - 1, true].WaterIdle = false;

                    TileWrapper.level.tiles[x + 1, y, true].LavaIdle = false;
                    TileWrapper.level.tiles[x - 1, y, true].LavaIdle = false;
                    TileWrapper.level.tiles[x, y + 1, true].LavaIdle = false;
                    TileWrapper.level.tiles[x, y - 1, true].LavaIdle = false;
                }
            }
        }
        protected void CacheIsLarge()
        {
            IsLarge = (foreground != null && foreground.Size != Item.One) || (Reference != this && Reference != null);
        }
        /// <summary>
        /// The background <c>[Background]BlockItem</c> on the tile.
        /// </summary>
        public BlockItem Background
        {
            get { return background; }
            set
            {
                //Notify the level or tile wrapper that we modified the bg
                if (TileWrapper != null)
                    TileWrapper.OnSetTile(X, Y);
                //Set the foreground to the new value
                background = value;
                //If it has variations, select one
                VariateBackground();
                //Lastly, reset the paint color
                BackgroundPaintColor = 0;
                BackgroundSheet = Rectangle.Empty;
            }
        }
        /// <summary>
        /// Identifies if this block is a 'full background', meaning that it is below the surface and will not let light in
        /// (It is the dark '3rd' BG layer, and is calculated in set_X)
        /// </summary>
        public bool FullBackground { get { return GetBit(Data, 1); } private set { SetBit(ref Data, 1, value); } }
        /// <summary>
        /// The reference tile, used for 'large tiles" to point/redirect to the main tile
        /// </summary>
        /// <remarks>
        /// Imagine a 2x2 block, the 1,1 coord is the actual block, while the 1,2 2,1 and 2,2 blocks store a reference
        /// to the 1,1 coordinate, but the other blocks can have their own BG's and properties while still containing the main tile
        /// </remarks>
        public Tile Reference { get { return reference; } set { reference = value; CacheIsLarge(); } }
        /// <summary>
        /// The X position of this tile
        /// </summary>
        public int X
        {
            get { return x; }
            set
            {
                x = (short)value;
                if (x > 0 && TileWrapper != null && TileWrapper.level != null && TileWrapper.level.worldGen != null && TileWrapper.level.worldGen.HeightMap != null && Y > TileWrapper.level.worldGen.HeightMap[value] + WorldGen.FullBackgoundLevel)
                    FullBackground = true;
            }
        }
        /// <summary>
        /// The Y position of this tile
        /// </summary>
        public int Y { get { return y; } set { y = (short)value; } }
        /// <summary>
        /// The index of the color of the foreground
        /// </summary>
        public byte ForegroundPaintColor { get; set; }
        /// <summary>
        /// The index of the color of the background
        /// </summary>
        public byte BackgroundPaintColor { get; set; }
        /// <summary>
        /// Determines if the tile is flipped horizontally
        /// </summary>
        public bool Flip { get { return GetBit(Data, 2); } set { SetBit(ref Data, 2, value); } }
        /// <summary>
        /// The variation of the foreground block
        /// </summary>
        public byte ForegroundVariation { get; set; }
        /// <summary>
        /// The variation of the background block
        /// </summary>
        public byte BackgroundVariation { get; set; }
        /// <summary>
        /// The amount of water (0 - 255) contained in the tile
        /// </summary>
        public int WaterMass
        {
            get { return waterMass; }
            set
            {
                    if (value == 0 && waterMass > 0)
                        TileWrapper.level.LiquidManager.ActiveWaterTiles.Remove(new Point(x, y));
                    if (value > 0 && waterMass == 0)
                        TileWrapper.level.LiquidManager.ActiveWaterTiles.Add(new Point(x, y));
                    waterMass = (byte)value;
                    if ((Game.level.ready || Game.level.generating) && y > 1 && y < TileWrapper.level.Height - 1)
                    {
                        TileWrapper.level.tiles[x + 1, y, true].WaterIdle = false;
                        TileWrapper.level.tiles[x - 1, y, true].WaterIdle = false;
                        TileWrapper.level.tiles[x, y + 1, true].WaterIdle = false;
                        TileWrapper.level.tiles[x, y - 1, true].WaterIdle = false;
                    }
            }
        }
        public int LavaMass
        {
            get { return lavaMass; }
            set
            {
                if (value == 0 && lavaMass > 0)
                    TileWrapper.level.LiquidManager.ActiveLavaTiles.Remove(new Point(x, y));
                else if (value > 0 &&lavaMass == 0)
                    TileWrapper.level.LiquidManager.ActiveLavaTiles.Add(new Point(x, y));
                lavaMass = (byte)value;
                if ((Game.level.ready || Game.level.generating) && y > 1 && y < TileWrapper.level.Height - 1)
                {
                    TileWrapper.level.tiles[x + 1, y, true].LavaIdle = false;
                    TileWrapper.level.tiles[x - 1, y, true].LavaIdle = false;
                    TileWrapper.level.tiles[x, y + 1, true].LavaIdle = false;
                    TileWrapper.level.tiles[x, y - 1, true].LavaIdle = false;
                }
            }
        }
        private byte waterMass;
        private byte lavaMass;
        /// <summary>
        /// The fire state of the foreground
        /// </summary>
        /// <remarks>
        /// Fire has states 0 - 10
        /// 0 = No Fire
        /// 1 - 9 = On fire, different stages mean burning for longer
        /// 10 = Last fire stage, will be extinguished and spread to next block if applicable
        /// </remarks>
        public byte ForegroundFireMeta
        {
            get { return foregroundFireMeta; }
            set
            {
                if (TileWrapper.level != null && TileWrapper.level.FireManager != null)
                {
                    //Set on fire
                    if (foregroundFireMeta == 0 && value > 0 && !TileWrapper.level.FireManager.ActiveFires.Contains(new Point(X, Y)))
                        TileWrapper.level.FireManager.ActiveFires.Add(new Point(X, Y));
                    //Extinguish Fire
                    else if (foregroundFireMeta > 0 && value == 0 && TileWrapper.level.FireManager.ActiveFires.Contains(new Point(X, Y)))
                        TileWrapper.level.FireManager.ActiveFires.Remove(new Point(X, Y));
                    //Set FG fire value
                    foregroundFireMeta = value;
                    ForegroundSheet = Rectangle.Empty;
                }
            }
        }
        /// <summary>
        /// The fire state of the background
        /// </summary>
        public byte BackgroundFireMeta
        {
            get { return backgroundFireMeta; }
            set
            {
                if (TileWrapper.level != null && TileWrapper.level.FireManager != null)
                {
                    //Set on fire
                    if (backgroundFireMeta == 0 && value > 0 && !TileWrapper.level.FireManager.ActiveFires.Contains(new Point(X, Y)))
                        TileWrapper.level.FireManager.ActiveFires.Add(new Point(X, Y));
                    //Extinguish Fire
                    else if (backgroundFireMeta > 0 && value == 0 && TileWrapper.level.FireManager.ActiveFires.Contains(new Point(X, Y)))
                        TileWrapper.level.FireManager.ActiveFires.Remove(new Point(X, Y));
                    //Set FBGfire value
                    backgroundFireMeta = value;
                    BackgroundSheet = Rectangle.Empty;
                }
            }
        }
        /// <summary>
        /// Indicates if the tile should be draw (Used for dark spots in lighting, where tiles don't need to be drawn)
        /// </summary>
        public bool NoDraw { get { return GetBit(Data, 3); } set { SetBit(ref Data, 3, value); } }
        /// <summary>
        /// Indicates if the tile should ignore collision (Used for getting stuck in sand)
        /// </summary>
        public bool NoCollide { get { return GetBit(Data, 4); } set { SetBit(ref Data, 4, value); } }
        /// <summary>
        /// Indicates if the water is stagmant
        /// </summary>
        public bool WaterIdle { get { return GetBit(Data, 5); } set { SetBit(ref Data, 5, value); } }
        /// <summary>
        /// Indicates if the lava is stagmant
        /// </summary>
        public bool LavaIdle { get { return GetBit(Data, 6); } set { SetBit(ref Data, 6, value); } }
        #endregion

        #region Fields
        protected Tile reference;
        /// <summary>
        /// The foreground <c>[Foreground]BlockItem</c> on the tile.
        /// </summary>
        protected BlockItem foreground;
        /// <summary>
        /// The background <c>[Background]BlockItem</c> on the tile.
        /// </summary>
        protected BlockItem background;
        /// <summary>
        /// The X and Y positions of this tile
        /// </summary>
        protected short x, y;
        /// <summary>
        /// The fire state of the foreground
        /// </summary>
        protected byte foregroundFireMeta;
        /// <summary>
        /// The fire state of the background
        /// </summary>
        protected byte backgroundFireMeta;
        /// <summary>
        /// The light of this tile, used for rendering it. Based on the amount of light it emits, or the shadow on it (See Lighting.cs)
        /// </summary>
        public Color Light;
        public byte Data;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new tile instance
        /// </summary>
        /// <param name="item">The foreground item to have</param>
        /// <param name="X">The X position</param>
        /// <param name="Y">The Y position</param>
        /// <param name="flip">Should the tile be flipped</param>
        public Tile(BlockItem item, int X = -1, int Y = -1, bool flip = false)
        {
            Flip = flip;
            Init(item, X, Y);
        }
        static Tile()
        {
            random = new Random();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialized a new tile
        /// </summary>
        public virtual void Init(BlockItem item, int X, int Y)
        {
            //Check to make sure tile positions are specified
            if (item.Size != Item.One && X == -1 && Y == -1)
                throw new ArgumentNullException("Tiles larger than a single tile (1x1) must have their X and Y position specified");
            //Set up some defaults
            Foreground = item;
            Light = Foreground.Light;
            Background = Item.Blank;

            this.Y = Y;
            this.X = X;

            if (this.Reference == null)
            this.Reference = this;

            SetReferences(X, Y);
        }
        /// <summary>
        /// Set the references to nearby tiles if this tile is a 'large' tile
        /// </summary>
        protected void SetReferences(int X, int Y)
        {
            if (Foreground.Size != Item.One && Game.level != null)
            {
                for (int x = 0; x <= Foreground.Size.X - 1; x++)
                {
                    for (int y = 0; y <= Foreground.Size.Y - 1; y++)
                    {
                        if (Foreground.BlockMap[Flip ? Foreground.Size.X - 1 - x : x, y] && !(x == 0 && y == 0))
                        {
                            if (TileWrapper[X + x, Y + y, true] == null)
                                TileWrapper[X + x, Y + y, true] = new Tile(Item.Blank);
                            //InstaBreak Particls
                            if (Game.level != null && Game.level.ready)
                            {
                                for (int i = 0; i < 10; i++)
                                    Game.level.DefaultParticleEngine.SpawnItemParticle(ParticleType.ItemFall, ((x + X) * Tile.Width) + (Tile.Width / 2), ((Y + y) * Tile.Height) + (Tile.Height / 2), TileWrapper[X + x, Y + y, true].Foreground, Color.White);
                            }
                            TileWrapper[X + x, Y + y, true].Foreground = Item.Blank;
                            TileWrapper[X + x, Y + y, true].Reference = this;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Remove the references to nearby tiles if this tile is a 'large' tile
        /// </summary>
        public void RemoveReferences()
        {

            if (Foreground.Size != Item.One && Game.level != null)
                for (int x = 0; x <= Foreground.Size.X - 1; x++)
                    for (int y = 0; y <= Foreground.Size.Y - 1; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;
                        if (Foreground.BlockMap[Flip ? Foreground.Size.X - 1 - x : x, y])
                        {
                            Game.level.tiles[X + x, Y + y, true].Reference = null;
                        }
                    }

        }
        /// <summary>
        /// Creates a variation of the foreground, provided it has more than 1 variation
        /// </summary>
        protected void VariateForeground()
        {
            if (foreground.Variations > 1)
                ForegroundVariation = (byte)random.Next(0, foreground.Variations);
        }
        /// <summary>
        /// Creates a variation of the background, provided it has more than 1 variation
        /// </summary>
        protected void VariateBackground()
        {
            if (background.Variations > 1)
                BackgroundVariation = (byte)random.Next(0, background.Variations);
        }
        /// <summary>
        /// Clones a tile
        /// </summary>
        public object Clone()
        {
            return base.MemberwiseClone();
        }
        /// <summary>
        /// Compares two tiles to check if they are (mostly) the same, used for loading/saving
        /// </summary>
        public bool IsTheSame(Tile compareTo)
        {
            return (Foreground.ID == compareTo.Foreground.ID && Background.ID == compareTo.Background.ID && Flip == compareTo.Flip && BackgroundPaintColor == compareTo.BackgroundPaintColor && ForegroundPaintColor == compareTo.ForegroundPaintColor && FullBackground == compareTo.FullBackground && ForegroundFireMeta == compareTo.ForegroundFireMeta && BackgroundFireMeta == compareTo.BackgroundFireMeta);
        }
        #endregion
    }
    public class FallingTile : Tile
    {
        /// <summary>
        /// Y Position of the tile (current amount of falling)
        /// </summary>
        public float Position
        {
            get
            {
                return position;
            }
            set
            {
                position = MathHelper.Clamp(value, 0, Tile.Height);
            }
        }
        private float position;

        /// <summary>
        /// Pixels to fall per second
        /// </summary>
        public const float FallSpeed = 260f;

        public FallingTile(BlockItem item_, int X_ = 0, int Y_ = 0, bool flip = false)
            : base(item_, X_, Y_, flip)
        {
            // No Initilization needed, Call base ctor
        }

        public void Update(GameTime gameTime)
        {
            Position += FallSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
    /// <summary>
    /// Class for containing a tile with storage slots, such as a chest
    /// </summary>
    public class StorageTile : Tile
    {
        /// <summary>
        /// The slots that hold items, simlar to the players inventory
        /// </summary>
        public Slot[] Slots;

        public StorageTile(BlockItem item_, int X_ = 0, int Y_ = 0, bool flip = false)
            : base(item_, X_, Y_, flip)
        {
            //Initialize the slots with Blank items
            Slots = new Slot[item_.StorageSlots.X * item_.StorageSlots.Y];
            for (int i = 0; i < Slots.Length; i++)
                Slots[i] = new Slot(Item.Blank);
        }
    }
    /// <summary>
    /// Class for containing and animated tile with frames
    /// </summary>
    public class AnimatedTile : Tile
    {
        /// <summary>
        /// The current frame being played
        /// </summary>
        public byte FrameIndex;
        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        public float time;

        public AnimatedTile(BlockItem item_, int X_ = 0, int Y_ = 0, bool flip = false)
            : base(item_, X_, Y_, flip)
        {

        }
    }
    /// <summary>
    /// Class for containing and animated tile with frames
    /// </summary>
    public class ElectronicTile : AnimatedTile
    {
        /// <summary>
        /// The current frame being played
        /// </summary>
        public byte State { get { return FrameIndex; } set {
            if (value != FrameIndex && Foreground.ElectronicLight)
                Game.level.ComputeLighting = true;
            FrameIndex = value;
        } }
        
        public List<Wire>[] Inputs { get; set; }
        public List<Wire>[] Outputs { get; set; }

        /// <summary>
        /// The foreground <c>[Foreground]BlockItem</c> on the tile.
        /// </summary>
        public override BlockItem Foreground
        {
            get { return foreground; }
            set
            {
                CacheIsLarge();
                //Notify the level or tile wrapper that we modified the fg
                if (TileWrapper != null)
                    TileWrapper.OnSetTile(X, Y);
                //If this (or reference) is a "large" block, and we placed a new block on it, remove old references to this block
                if (value != foreground && IsLarge)
                    Reference.RemoveReferences();
                //Set the foreground to the new value
                foreground = value;
                if (foreground.AutoFlipVariation)
                    Flip = random.NextBoolean();
                //If it has variations, select one
                VariateForeground();
                //Lastly, reset the paint color
                ForegroundPaintColor = 0;
                //If this is a larger tile we need to set the other tiles to point to it
                SetReferences(X, Y);
                ForegroundSheet = Rectangle.Empty;

                if (foreground.Inputs != null)
                {
                    Inputs = new List<Wire>[foreground.Inputs.Length];
                    for (int i = 0; i < Inputs.Length; i++ )
                        Inputs[i] = new List<Wire>();
                }
                if (foreground.Outputs != null)
                {
                    Outputs = new List<Wire>[foreground.Outputs.Length];
                    for (int i = 0; i < Outputs.Length; i++)
                        Outputs[i] = new List<Wire>();
                }
            }
        }

        public ElectronicTile(BlockItem item_, int X_ = 0, int Y_ = 0, bool flip = false)
            : base(item_, X_, Y_, flip)
        {

        }

        public bool PluggedIn
        {
            get
            {
                bool pluggedin = false;
                if (Outputs != null)
                foreach (List<Wire> p in Outputs)
                    if (p.Count > 0)
                        pluggedin = true;
                if (Inputs != null)
                foreach (List<Wire> p in Inputs)
                    if (p.Count > 0)
                        pluggedin = true;
                return pluggedin;
            }
        }
    }
    /// <summary>
    /// Class for containing plant tiles that have growth stages and ticks
    /// </summary>
    public class PlantTile : Tile
    {
        public double LastTick;
        public int GrowthStage;

        public PlantTile(BlockItem item_, int X_ = 0, int Y_ = 0, bool flip = false)
            : base(item_, X_, Y_, flip)
        {

        }
    }
    public class PortalTile : Tile
    {
        public float Rotation;
        public Point Target;

        public PortalTile(BlockItem item_, int X_ = 0, int Y_ = 0, bool flip = false)
            : base(item_, X_, Y_, flip)
        {

        }
    }
    /// <summary>
    /// Class for containg text tiles such as signs and banners
    /// </summary>
    public class TextTile : Tile
    {
        public string Text = "";

        public TextTile(BlockItem item, int X = 0, int Y = 0, bool flip = false)
            : base(item, X, Y, flip)
        {

        }
    }
    public class TimerTile : ElectronicTile
    {
        public float Time;
        public float Length;
        public bool Waiting;
        public float WaitTime;

        public TimerTile(BlockItem item, int X = 0, int Y = 0, bool flip = false)
            : base(item, X, Y, flip)
        {
            Time = 1;
            Length = 1;
            Waiting = true;
            WaitTime = 0;
        }
        public override void Init(BlockItem item, int X, int Y)
        {
            base.Init(item, X, Y);
        }
    }

    public class FurnaceTile : Tile
    {
        public float Power;
        public Slot Slot;

        public FurnaceTile(BlockItem item, int X = 0, int Y = 0, bool flip = false)
            : base(item, X, Y, flip)
        {
            Slot = new Slot(Item.Blank, 0);
        }
    }
}
