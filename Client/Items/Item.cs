using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using ZarknorthClient;
using ZarknorthClient.Entities;
using ZarknorthClient.Interface;

namespace ZarknorthClient
{
    public class Item
    {
        /// <summary>
        /// List of items in the game
        /// </summary>
        public static List<Item> ItemList;

        /// <summary>
        /// Unique name of the item
        /// </summary>
        public string Name;

        /// <summary>
        /// Description of the item to be shown in tooltip
        /// </summary>
        public string Description;

        /// <summary>
        /// Unique ID of the item
        /// </summary>
        public int ID;

        /// <summary>
        /// Maximum amount of this item that one slot can hold
        /// </summary>
        public int MaxStack;

        public Texture2D[] Textures;
        public Texture2D[] GrayTextures;
        public int Teir;
        public Childeren Child;
        public Point TechTreePosition;
        public static Point One = new Point(1, 1);
        public float Fuel;
        public float UseTime;
        protected double LastUse;
        public float TimeToSmelt;

      
        public static Color[] StandardColors = new Color[0];

        /// <summary>
        /// If this tile item has any alpha
        /// </summary>
        public bool Clear;
        public bool Station;

        //All Hail the great item list
        public static BlockItem Blank, Dirt, DirtBG, Mud, MudBG, Stone, StoneBG, CobbleStone, CobbleStoneBG, Grass, Gravel, Sand, SandStone, GlassBlock, GlassBlockBG, Drywall, DrywallBG, SandStoneBG, Glass, GlassBG, WindowLattice, WoodenWindow, WoodenWindowBG, Obsidian, ObsidianBG, Ash, AshBG, Water, Lava, WoodLadder, MetalLadder, WoodBeam, MetalBeam, MetalPole, Wood, WoodBG, SpruceTree, SpruceLeaf, OakTree, OakLeaf, MangroveLeaf, MangroveTree, BirchTree, BirchLeaf, JungleTree, JungleLeaf, Cactus, WoodPlankPlatform, WoodPlatform, StoneBrick, StoneBrickBG, Brick, BrickBG, Pipe, RoughIce, RoughIceBG, Snow, SnowBG, Slush, MetalDoor, ModernDoor, WoodenDoor, Torch, MetalBed, WoodenBed, WoodenBench, MetalTable, WoodenTable, MetalChair, WoodenChair, Lamp, LavaLamp, WoodenNightstand, Synthesizer, Piano, WaterBanner, EarthBanner, FireBanner, AirBanner, SteelPlating, SteelPlatingBG, Pillar, Clay, ClayBG, Portal, Switch, Sign, CinderBlock, Sidewalk, Wheat, Asphalt, MetalBlock, MetalBlockBG, SciFi, SciFiBG, MetalChest, WoodenChest, WoodenCrate, MetalCrate, BellFlower, Crocosmia, WoodPlank, WoodPlankBG;
        public static Item Paper, Diamond, SilverBar, CopperBar, GoldBar, Ruby, IronBar, Coal, Quartz, Steel, Flint, FlintAndSteel, LogBG, Log, DiamondPickaxe, CopperPickaxe, GoldPickaxe, SilverPickaxe, IronPickaxe, StonePickaxe, OPPickaxe, Stick;
        public static BlockItem PressurePlate, AcadiaLeaf, AcadiaTree, WallLamp, TableLamp,FluorescentLight, Diode, PNPTransistor, NPNTransistor, Timer, Thatch, ThatchBG, MetalBars, MetalBarsBG, SmoothIce, SmoothIceBG, WoodenShingle, WoodenShingleBG, SmallDungeonBrick, SmallDungeonBrickBG, TempleStone, TempleStoneBG,TempleSlab, TempleSlabBG, TempleBrick, TempleBrickBG, PackedDirt, PackedDirtBG, Wildflower, Clover, WoodenDesk, WoodenCloset, WoodenDresser, Anvil, Sawmill, ANDGate, ORGate, NORGate, NOTGate, NANDGate, XORGate, XNORGate, Button, Lever, Indicator, PowerSource, CardboardBox, Cardboard, CardboardBG, SmallStalagtite, LargeStalagtite, WoodenStool,Bookshelf,WoodPedestal, CeramicTile,CeramicTileBG,SmoothWallpaper, TexturedWallpaper, DottedWallpaper,FancyCheckeredWallpaper, TiledWallpaper, SmallCheckeredWallpaper,LargeCheckeredWallpaper, PlainWallpaper, MetalPlatform, MetalStair, StoneStair, WoodPlankStair, WoodStair, Vine, AlderTree, AlderLeaf, Toadstool, TallGrassPlant, DeadBush, WoodPlankSupport, WoodSupport, Spike,CoffeeTable,PicketFence, ChainLinkFence, WoodFence, Furnace, DiamondOre, SilverOre, CopperOre, GoldOre, RubyOre, IronOre, CoalOre, QuartzOre, CallaLily, GrassPlant, IceMushroom, Mushroom, CraftingTable, StonePlatform;
        public static ToolItem Bucket, WaterBucket, LavaBucket, PaintBrush, Light_Orb, Wirer, LightningRod;
        public static PhysicsItem Football, Basketball, Glowstick;

        #region Event handlers
        /// <summary>
        /// Event fired when the item is selected in the inventory
        /// </summary>
        public event SelectItemEventHandler Select;
        public virtual void OnSelect(SelectItemEventArgs e)
        {
            if (Select != null) Select(this, e);
        }

        /// <summary>
        /// Event fired when the item is de-selected in the inventory
        /// </summary>
        public event SelectItemEventHandler DeSelect;
        public virtual void OnDeSelect(SelectItemEventArgs e)
        {
            if (DeSelect != null) DeSelect(this, e);
        }

        /// <summary>
        /// Event fired when the item left clicks a point in the world
        /// </summary>
        public event MouseItemEventHandler LeftClick;
        public virtual void OnLeftClick(MouseItemEventArgs e)
        {
            if (LeftClick != null) LeftClick(this, e);
        }

        /// <summary>
        /// Event fired when the item right clicks a point in the world
        /// </summary>
        public event MouseItemEventHandler RightClick;
        public virtual void OnRightClick(MouseItemEventArgs e)
        {
            if (RightClick != null) RightClick(this, e);
        }

        /// <summary>
        /// Event fired when the item is held down over a point in the world
        /// </summary>
        public event MouseItemEventHandler LeftHold;
        public virtual void OnLeftHold(MouseItemEventArgs e)
        {
            if (LastUse + UseTime <= e.GameTime.TotalGameTime.TotalSeconds)
                if (LeftHold != null) { LastUse = e.GameTime.TotalGameTime.TotalSeconds; LeftHold(this, e); };
        }

        /// <summary>
        /// Event fired when the item is held down over a point in the world
        /// </summary>
        public event MouseItemEventHandler RightHold;
        public virtual void OnRightHold(MouseItemEventArgs e)
        {
            if (RightHold != null) RightHold(this, e);
        }

        /// <summary>
        /// Event fired when stats are loaded for crafting UI
        /// </summary>
        private PrintItemDataEventHandler printStats;
        public event PrintItemDataEventHandler PrintStats
        {
            add { printStats = value; }
            remove { printStats -= value; }
        }
        public virtual string[] OnPrintStats(PrintItemDataEventArgs e)
        {
            if (printStats != null) return printStats(this, e);
            else return new string[1] { "No PrintItemDataEventArgs defined." };
        }
        #endregion

        public Item(string name, string description = null)
        {
            ID = ItemList.Count;
            Name = name;
            Description = description;
            ItemList.Add(this);
            Textures = new Texture2D[1];

            PrintStats += new PrintItemDataEventHandler(delegate(object o, PrintItemDataEventArgs e)
            {
                List<string> str = new List<string>();
                str.Add("Max Stack: " + MaxStack);
                if (Fuel > 0)
                str.Add("- Is Fuel (Can smelt " + Fuel + " blocks)");
                return str.ToArray();
            });
            //Interact with blocks clicked on
            RightClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                e.Level.tiles[e.X / Tile.Width, e.Y / Tile.Height].Foreground.OnInteract(new InteractBlockEventArgs(e.Level, e.X / Tile.Width, e.Y / Tile.Height));
            });

        }

        static Item()
        {
            ItemList = new List<Item>();
            List<Color> SC = new List<Color>();
            for (int i = 0; i < 360; i += 12)
                SC.Add(Extensions.ColorFromHSV(i, .83, 1));
            for (int i = 0; i < 360; i += 12)
                SC.Add(Extensions.ColorFromHSV(i, .48, 1));
            StandardColors = SC.ToArray();
            Init();
        }
        public static bool operator ==(Item item1, Item item2)
        {
            // If left hand side is null...
            if (System.Object.ReferenceEquals(item1, null))
            {
                // ...and right hand side is null...
                if (System.Object.ReferenceEquals(item2, null))
                {
                    //...both are null and are Equal.
                    return true;
                }

                // ...right hand side is not null, therefore not Equal.
                return false;
            }
            if (System.Object.ReferenceEquals(item2, null))
            {
                // ...and right hand side is null...
                if (System.Object.ReferenceEquals(item1, null))
                {
                    //...both are null and are Equal.
                    return true;
                }

                // ...right hand side is not null, therefore not Equal.
                return false;
            }
            // Return true if the fields match:
            return item1.ID == item2.ID;
        }
        public static bool operator !=(Item item1, Item item2)
        {
            return !(item1 == item2);
        }
        public static bool Equals(Item item1, Item item2)
        {
            return item1.ID == item2.ID;
        }
        public static void Init()
        {
            Blank = new BlockItem("Blank", "Nothing Here!")
            {
                MaxStack = (int)StackSize.None,
                Absorb = 16,
                Light = Color.White,
                MinimapColor = Color.White,
                SkyLight = true,
            };
            Dirt = new ForegroundBlockItem("Dirt", "Plain 'ol dirt.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            DirtBG = new BackgroundBlockItem("Dirt BG");

            Stone = new ForegroundBlockItem("Stone", "Basic yet abundant material for sturdy building!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 1200,
            };
            StoneBG = new BackgroundBlockItem("Stone BG");

            CobbleStone = new ForegroundBlockItem("Cobblestone", "Basic brick for contruction.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 1200,
                PaintColors = StandardColors,
            };
            CobbleStoneBG = new BackgroundBlockItem("Cobblestone BG");

            Grass = new ForegroundBlockItem("Grass", "Warm feet and the wonder of nature!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.SpriteSheet,
                Drop = Item.Dirt,
            };

            Gravel = new ForegroundBlockItem("Gravel", "A pile of small stones.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Falling,
                BreakTime = 800,
                CanFall = true,
                UseTime = .17f,
            };
            Sand = new ForegroundBlockItem("Sand", "Some fine sand, could be used to smelt glass.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Falling,
                BreakTime = 800,
                CanFall = true,
                UseTime = .17f,
            };
            SandStone = new ForegroundBlockItem("Sand Stone", "Sturdy enough for a sandcastle,")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };

            SandStoneBG = new BackgroundBlockItem("Sand Stone BG");

            Glass = new ForegroundBlockItem("Glass", "Caution: Sharp Parts")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 14,
                TimeToSmelt = 2000,
                PaintColors = StandardColors,
            };

            GlassBG = new BackgroundBlockItem("Glass BG", "Caution: Sharp Parts")
            {
                MaxStack = (int)StackSize.Max,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 14,
                PaintColors = StandardColors,
            };

            GlassBlock = new ForegroundBlockItem("Glass Block", "Fancy glass offering nice privacy.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 0,
                TimeToSmelt = 2000,
                PaintColors = StandardColors,
            };

            GlassBlockBG = new BackgroundBlockItem("Glass Block BG", "Fancy glass offering nice privacy.")
            {
                MaxStack = (int)StackSize.Max,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 0,
                PaintColors = StandardColors,
            };
            WoodenWindow = new ForegroundBlockItem("Wooden Window", "A nice window to take a look out.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 0,
                TimeToSmelt = 2000,
                PaintColors = StandardColors,
            };
            WoodenWindowBG = new BackgroundBlockItem("Wooden Window BG", "A nice window to take a look out.")
            {
                MaxStack = (int)StackSize.Max,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 0,
                PaintColors = StandardColors,
            };

            WindowLattice = new BackgroundBlockItem("Window Lattice", "A window of crossed wood.")
            {
                MaxStack = (int)StackSize.Max,
                BreakTime = 500,
                SkyLight = true,
                Absorb = 0,
                PaintColors = StandardColors,
            };

            Drywall = new ForegroundBlockItem("Drywall", "Perfect for walls in your home.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                BreakTime = 900,
                Burnable = true,
            };

            DrywallBG = new BackgroundBlockItem("Drywall BG");

            PackedDirt = new ForegroundBlockItem("Packed Dirt", "Some hard and dry dirt.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            PackedDirtBG = new BackgroundBlockItem("Packed Dirt BG");

            Obsidian = new ForegroundBlockItem("Obsidian", "Forged from the core of a volcano.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 10000,
            };
            ObsidianBG = new BackgroundBlockItem("Obsidian BG");

            Ash = new ForegroundBlockItem("Ash", "Don't breathe it in.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };

            AshBG = new BackgroundBlockItem("Ash BG");

            Mud = new ForegroundBlockItem("Mud", "Mucky. Yucky.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            MudBG = new BackgroundBlockItem("Mud BG");

            Water = new BlockItem("Water", "Down in the deep blue sea!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Liquid,
                Absorb = 10,
                RenderMode = BlockRenderMode.Custom,
            };
            Water.LeftHold += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e) {
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width , e.Y / Tile.Height)))
                        e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].WaterMass = 255;
            });

            Lava = new BlockItem("Lava", "Hot stuff!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Liquid,
                Absorb = 10,
                Light = new Color(255, 72, 15),
                RenderMode = BlockRenderMode.Custom,
            };
            Lava.LeftHold += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e) {
                if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                    e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].LavaMass = 255;
            });

            WoodLadder = new ForegroundBlockItem("Wood Ladder", "Allows you to climb up and down.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Ladder,
                Burnable = true,
                ClimbUpSpeed = 1,
                ClimbDownSpeed = 1,
                PaintColors = StandardColors,
            };
            MetalLadder = new ForegroundBlockItem("Metal Ladder", "A faster ladder to climb with.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Ladder,
                Burnable = false,
                ClimbUpSpeed = 1.5f,
                ClimbDownSpeed = 1.5f,
            };
            WoodBeam = new ForegroundBlockItem("Wood Beam", "A sturdy wooden beam for construction.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                Burnable = true,
                Absorb = 16,
                PaintColors = StandardColors,
            };
            MetalPole = new ForegroundBlockItem("Metal Pole", "A very fast pole to slide down.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Ladder,
                ClimbUpSpeed = 1.25f,
                ClimbDownSpeed = 3f,
                Absorb = 16,
            };
            MetalBeam = new ForegroundBlockItem("Metal Beam", "A strong metal beam for supporting structures.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Ladder,
                Absorb = 16,
            };
            #region Tree Related Items (long)
            Log = new ForegroundBlockItem("Log", "Freshly chopped and simple wood.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                Burnable = true,
                PaintColors = StandardColors,
                Fuel = 1,
            };
            LogBG = new BackgroundBlockItem("Log BG");
            SpruceTree = new BackgroundBlockItem("SpruceTree", "Log of a Spruce tree.")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };

            SpruceLeaf = new ForegroundBlockItem("SpruceLeaf", "Leaf of a Spruce tree.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,
            };
            SpruceLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                if (e.Level.random.Next(0, 10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick, 1));
                ((BlockItem)o).HandleDrop(o, e);
            });

            OakTree = new BackgroundBlockItem("OakTree", "Log of a Oak tree.")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };

            OakLeaf = new ForegroundBlockItem("OakLeaf", "Leaf of a Oak tree")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,
            };
            OakLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                 if (e.Level.random.Next(0,10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick,1));
                 ((BlockItem)o).HandleDrop(o, e);
            });
            AcadiaTree = new BackgroundBlockItem("AcadiaTree", "Log of an Acadia tree.")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };
            AcadiaLeaf = new ForegroundBlockItem("AcadiaLeaf", "Leaf of an Acadia tree")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,
            };
            AcadiaLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                if (e.Level.random.Next(0, 10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick, 1));
                ((BlockItem)o).HandleDrop(o, e);
            });

            MangroveTree = new BackgroundBlockItem("MangroveTree", "Log of a Mangrove tree")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };

            MangroveLeaf = new ForegroundBlockItem("MangroveLeaf", "Leaf of a Mangrove tree")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,
            };
            MangroveLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                if (e.Level.random.Next(0, 10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick, 1));
                ((BlockItem)o).HandleDrop(o, e);
            });

            AlderTree = new BackgroundBlockItem("AlderTree", "Log of a Alder tree")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };

            AlderLeaf = new ForegroundBlockItem("AlderLeaf", "Leaf of a Alder tree")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,
            };
            AlderLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                if (e.Level.random.Next(0, 10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick, 1));
                ((BlockItem)o).HandleDrop(o, e);
            });
            
            BirchTree = new BackgroundBlockItem("BirchTree", "Log of a Birch tree")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };
            BirchLeaf = new ForegroundBlockItem("BirchLeaf", "Leaf of a Birch tree")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,

            };
            BirchLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                if (e.Level.random.Next(0, 10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick, 1));
            });
            JungleTree = new BackgroundBlockItem("JungleTree", "Log of a Jungle tree")
            {
                MaxStack = (int)StackSize.Max,
                Absorb = 10,
                Burnable = true,
                Drop = Item.Log,
                TreeCapitate = true,
                BreakTime = 5000,
            };
            JungleLeaf = new ForegroundBlockItem("JungleLeaf", "Leaf of a Jungle tree")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 10,
                Burnable = true,
                TreeCapitate = true,
                Drop = Item.Blank,

            };
            JungleLeaf.Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                if (e.Level.random.Next(0, 10) == 0)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Item.Stick, 1));
            });
            #endregion
            Wood = new ForegroundBlockItem("Wood", "Classic, sturdy and nice looking")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                Burnable = true,
                PaintColors = StandardColors,
            };
            WoodBG = new BackgroundBlockItem("Wood BG");

            WoodPlank = new ForegroundBlockItem("Wood Plank", "Classic, sturdy and nice looking")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                Burnable = true,
                Variations = 6,
                PaintColors = StandardColors,
            };
            WoodPlankBG = new BackgroundBlockItem("Wood Plank BG");

            Cactus = new ForegroundBlockItem("Cactus", "Keep your hands and face away!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Burnable = true,
                TreeCapitate = true,
            };
            WoodPlatform = new ForegroundBlockItem("Wood Platform", "A small ledge that can be used as a 2 way playform.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Platform,
                PaintColors = StandardColors,
                Burnable = true,
            };
            WoodPlankPlatform = new ForegroundBlockItem("Wood Plank Platform", "A small ledge that can be used as a 2 way playform.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Platform,
                PaintColors = StandardColors,
                Burnable = true,
            };
            MetalPlatform = new ForegroundBlockItem("Metal Platform", "A thin metal surface.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Platform,
                PaintColors = StandardColors,
                Burnable = true,
            };
            Brick = new ForegroundBlockItem("Brick", "Perfect for houses, walls, and more!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                Variations = 4,
            };
            BrickBG = new BackgroundBlockItem("Brick BG");

            TempleSlab = new ForegroundBlockItem("Temple Slab", "A sacred stone used by ancient architects.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                Variations = 14,
            };
            TempleSlabBG = new BackgroundBlockItem("Temple Slab BG");

            TempleBrick = new ForegroundBlockItem("Temple Brick", "A sacred brick used by ancient architects.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                Variations = 5,
            };
            TempleBrickBG = new BackgroundBlockItem("Temple Brick BG");

            TempleStone = new ForegroundBlockItem("Temple Stone", "A finely crafted ancient stone.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                Variations = 2,
            };
            TempleStoneBG = new BackgroundBlockItem("Temple Stone BG");

            SmallDungeonBrick = new ForegroundBlockItem("Small Dungeon Brick", "A finely crafted ancient stone.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                Variations = 1,
            };
            SmallDungeonBrickBG = new BackgroundBlockItem("Small Dungeon Brick BG");

            StoneBrick = new ForegroundBlockItem("Stone Brick", "Perfect for houses, walls, and more!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
            };
            StoneBrickBG = new BackgroundBlockItem("Stone Brick BG");

            Pipe = new ForegroundBlockItem("Pipe", "Call the plumber!")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.SpriteSheet,
            };

            RoughIce = new ForegroundBlockItem("Rough Ice", "A chilly hunk of ice with shards of rock embedded within.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            RoughIceBG = new BackgroundBlockItem("Rough Ice BG");

            SmoothIce = new ForegroundBlockItem("Smooth Ice", "Smooth ice, cold to the touch.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            SmoothIceBG = new BackgroundBlockItem("Smooth Ice BG");

            Snow = new ForegroundBlockItem("Snow", "Perfect for building a winter snow fort!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            SnowBG = new BackgroundBlockItem("Snow BG");

            Slush = new ForegroundBlockItem("Slush", "Mushy melted ice.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Falling,
                CanFall = true,
            };

            Torch = new ForegroundBlockItem("Torch", "A simple way... to light the way.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Light = Color.Lerp(Color.Orange, Color.White, .3f),
                RenderMode = BlockRenderMode.Animation | BlockRenderMode.SpriteSheet,
                SubType = BlockSubType.Animated,
                FrameCount = 2,
                FrameTime = .2f,
                EdgeMode = BlockEdgeMode.Stick,
                Burnable = true,
                BreakFall = true,
                LightHand = true,
                PlaceMode = BlockPlaceMode.AllButTop,
                NeedsWallSupport = true,
            };
            Torch.Placed += new PlaceBlockEventHandler(delegate(object o, PlaceBlockEventArgs e)
            {
                bool handled = (o as BlockItem).HandlePlace(e);
                if (handled)
                    Achievement.Show(Achievement.Torch);
                return handled;
            });
            WoodenDoor = new ForegroundBlockItem("Wooden Door", "A wooden door, not very secure.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Door,
                Size = new Point(2, 3),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                Variations = 2,
                AutoVariation = false,
                PaintColors = StandardColors,
                FrameImportant = true,
                PluginCheck = true,
                SubType = BlockSubType.Electronic,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleCenter, ConnectionPoint.ConnectionType.Input) },
            };
            WoodenDoor.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                int state = (Wire.GetInputState(0, e) ? 1 : 0);
                if (state != e.level.tiles[e.x, e.y].ForegroundVariation)
                e.level.tiles[e.x, e.y].ForegroundVariation = (byte)state;
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            ModernDoor = new ForegroundBlockItem("Modern Door", "A stylish door for contemporary homes.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Door,
                Size = new Point(2, 3),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                Variations = 2,
                AutoVariation = false,
                PaintColors = StandardColors,
                FrameImportant = true,
                PluginCheck = true,
                SubType = BlockSubType.Electronic,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleCenter, ConnectionPoint.ConnectionType.Input) },
            };
            ModernDoor.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                int state = (Wire.GetInputState(0, e) ? 1 : 0);
                if (state != e.level.tiles[e.x, e.y].ForegroundVariation)
                    e.level.tiles[e.x, e.y].ForegroundVariation = (byte)state;
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            MetalDoor = new ForegroundBlockItem("Metal Door", "A strong metal barrier.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Door,
                Size = new Point(2, 3),
                RenderMode = BlockRenderMode.Single,
                Flipable = true,
                Variations = 2,
                AutoVariation = false,
                PaintColors = StandardColors,
                FrameImportant = true,
                PluginCheck = true,
                SubType = BlockSubType.Electronic,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleCenter, ConnectionPoint.ConnectionType.Input) },
            };
            MetalDoor.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                int state = (Wire.GetInputState(0, e) ? 1 : 0);
                if (state != e.level.tiles[e.x, e.y].ForegroundVariation)
                    e.level.tiles[e.x, e.y].ForegroundVariation = (byte)state;
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            WoodenBed = new ForegroundBlockItem("Wooden Bed", "A cozy little bed.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(3, 1),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
            };
            WoodenBed.Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                if (e.y >= 3)
                {
                    if (e.level.tiles[e.x, e.y - 1].Foreground.Collision != BlockCollision.Passable || e.level.tiles[e.x, e.y - 2].Foreground.Collision != BlockCollision.Passable)
                    {
                        Interface.MainWindow.Console.MessageBuffer.Add(new ConsoleMessage("Could not set respawn, bed obstructed.", 2));
                    }
                    else
                    {
                        Interface.MainWindow.Console.MessageBuffer.Add(new ConsoleMessage("Respawn set.", 2));
                        e.level.BedSpawnPoint = new Vector2(e.x, e.y);
                    }
                }
                else Interface.MainWindow.Console.MessageBuffer.Add(new ConsoleMessage("Could not set respawn, bed too high.", 2));
            });
            MetalBed = new ForegroundBlockItem("Metal Bed", "A cold, hard bed.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(3, 1),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
            };
            MetalBed.Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                WoodenBed.OnInteract(e);
            });
            Thatch = new ForegroundBlockItem("Thatch", "A flimsy thatch roof.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.SpriteSheet,
                Burnable = true,
                PaintColors = StandardColors,
            };
            ThatchBG = new BackgroundBlockItem("Thatch BG");
            WoodenShingle = new ForegroundBlockItem("Wooden Shingle", "A flimsy wooden roof.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Stair,
                RenderMode = BlockRenderMode.SpriteSheet,
                Burnable = true,
                PaintColors = StandardColors,
                Flipable = true,
            };
            WoodenShingleBG = new BackgroundBlockItem("Wooden Shingle BG");
            WoodenNightstand = new ForegroundBlockItem("Wooden Nightstand", "A bedside table.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Platform,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                PaintColors = StandardColors,
                BreakNoSupportBottom = true,
            };
            WoodenTable = new ForegroundBlockItem("Wooden Table", "A simple wood table.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Platform,
                Size = new Point(3, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                PaintColors = StandardColors,
                BlockMap = new bool[3, 2] 
                {
                    { true, true },
                    { true, false },
                    { true, true }
                },
                BreakNoSupportBottom = true,
            };
            MetalTable = new ForegroundBlockItem("Metal Table", "A sturdy metal table, cold to the touch.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Platform,
                Size = new Point(3, 2),
                RenderMode = BlockRenderMode.Single,
                PaintColors = StandardColors,
                BlockMap = new bool[3, 2] 
                {
                    { true, true },
                    { true, false },
                    { true, true }
                },
                BreakNoSupportBottom = true,
            };
            WoodenChair = new ForegroundBlockItem("Wooden Chair", "A small hand-crafted wooden chair.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(1, 3),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                PaintColors = StandardColors,
                BreakNoSupportBottom = true,
            };
            MetalChair = new ForegroundBlockItem("Metal Chair", "Not exactly made to be comforting.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(1, 3),
                RenderMode = BlockRenderMode.Single,
                Flipable = true,
                PaintColors = StandardColors,
                BreakNoSupportBottom = true,
            };
            CoffeeTable = new ForegroundBlockItem("Coffee Table", "For like... coffee in the morning.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Platform,
                Size = new Point(3,1),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                PaintColors = StandardColors,
                BreakNoSupportBottom = true,
            };
            WoodenBench = new ForegroundBlockItem("Wooden Bench", "A seat for a group.")
            {
                MaxStack = (int)StackSize.Small,
                Collision = BlockCollision.Passable,
                Size = new Point(3, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                PaintColors = StandardColors,
                BreakNoSupportBottom = true,
            };
            Lamp = new ForegroundBlockItem("Lamp", "Perfect for late night reading.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Light = Color.Wheat,
                Burnable = true,
                BreakNoSupportBottom = true,
            };
            LavaLamp = new ForegroundBlockItem("Lava Lamp", "A relaxing lamp, its a wonder how it works.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Animation | BlockRenderMode.Single,
                Light = Color.White,
                SubType = BlockSubType.Animated,
                Size = new Point(1, 2),
                FrameCount = 9,
                FrameTime = 5f,
                Burnable = true,
                BreakNoSupportBottom = true,
            };
            Piano = new ForegroundBlockItem("Piano", "Simple, classic music.")
            {
                MaxStack = (int)StackSize.Few,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Music,
                Size = new Point(3, 2),
                Burnable = true,
                BreakNoSupportBottom = true,
            };
            //Since the Music Category is so small, just add a custom Interact handler
            Piano.Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                //Text opens a new sign window
                TaskMusic tmp = new TaskMusic(e.level.game.Manager);
                tmp.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
                tmp.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
                tmp.Init();
                e.level.game.Manager.Add(tmp);
                tmp.Show();
            });

            Synthesizer = new ForegroundBlockItem("Synthesizer", "Dem fancy synthesizers...")
            {
                MaxStack = (int)StackSize.Few,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Music,
                Size = new Point(3, 2),
                Burnable = true,
                BreakNoSupportBottom = true,
            };

            //Since the Music Category is so small, just add a custom Interact handler
            Synthesizer.Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                //Text opens a new sign window
                TaskSynthesizer tmp = new TaskSynthesizer(e.level.game.Manager);
                tmp.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
                tmp.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
                tmp.Init();
                e.level.game.Manager.Add(tmp);
                tmp.Show();
            });

            WaterBanner = new ForegroundBlockItem("Water Banner", "The element of water.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Size = new Point(1,2),
                AutoFlipVariation = true,
                Flipable = true,
                PlaceMode = BlockPlaceMode.Hanging,
            };
            AirBanner = new ForegroundBlockItem("Air Banner", "The element of air.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Size = new Point(1, 2),
                AutoFlipVariation = true,
                Flipable = true,
                PlaceMode = BlockPlaceMode.Hanging,
            };
            EarthBanner = new ForegroundBlockItem("Earth Banner", "The element of earth.")
            {
                MaxStack = (int)StackSize.Few,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Size = new Point(1, 2),
                AutoFlipVariation = true,
                Flipable = true,
                PlaceMode = BlockPlaceMode.Hanging,
            };
            FireBanner = new ForegroundBlockItem("Fire Banner", "The element of fire.")
            {
                MaxStack = (int)StackSize.Few,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Size = new Point(1, 2),
                AutoFlipVariation = true,
                Flipable = true,
                PlaceMode = BlockPlaceMode.Hanging,
            };
            SteelPlating = new ForegroundBlockItem("Steel Plating", "Strengthen your buildings!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            SteelPlatingBG = new BackgroundBlockItem("Steel Plating BG");

            Cardboard= new ForegroundBlockItem("Cardboard", "Not of much use.")
            {
                MaxStack = (int)StackSize.Max,
                PaintColors = StandardColors,
                Collision = BlockCollision.Impassable,
                Burnable = true,
            };
            CardboardBG = new BackgroundBlockItem("Cardboard BG");

            CardboardBox = new ForegroundBlockItem("Cardboard Box", "")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                Collision = BlockCollision.Passable,
                Variations = 3,
                Flipable = true,
                AutoFlipVariation = true,
                Size = new Point(2, 1),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                BreakNoSupportBottom = true,
            };
            Pillar = new ForegroundBlockItem("Pillar", "An ancient decor.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                PaintColors = StandardColors,
            };

            Clay = new ForegroundBlockItem("Clay", "Some soft clay.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
            };
            ClayBG = new BackgroundBlockItem("Clay BG");

            Sign = new ForegroundBlockItem("Sign", "Leave a message!")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Size = new Point(2, 2),
                SubType = BlockSubType.Text,
                Burnable = true,
                PaintColors = StandardColors,
                BreakNoSupportBottom = true,
            };
            CinderBlock = new ForegroundBlockItem("Cinder Block", "'Bout to put a cinder block through yall's windshield!")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.SpriteSheet,
                Size = new Point(1, 1),
                PaintColors = StandardColors,
                EdgeWidth = 2,
                BreakTime = 1300,
            };
            Sidewalk = new ForegroundBlockItem("Sidewalk", "A simple concrete surface.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.SpriteSheet,
                EdgeWidth = 5,
                PaintColors = StandardColors,
                BreakTime = 1300,
            };
            //LightningRod = new ToolItem("Lightning Rod", "You shall be smitten!")
            //{
            //    MaxStack = (int)StackSize.Few,
            //};
            //LightningRod.LeftClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            //{
            //    e.Level.SpawnBolt();
            //});
            Paper = new Item("Paper", "Draw something!")
            {
                MaxStack = (int)StackSize.Few,
            };
            Bucket = new ToolItem("Bucket", "An empty metal bucket.")
            {
                MaxStack = (int)StackSize.Single,
            };
            Bucket.LeftClick+= new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.X / Tile.Height, e.Y / Tile.Width), new Vector2(e.Level.Player.Position.X / Tile.Width, e.Level.Player.Position.Y / Tile.Height)) <= 8)
                {
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                    {
                        if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].WaterMass > 200)
                        {
                            e.Level.Player.Inventory[MainWindow.inventory.Selected].Item = Item.WaterBucket;
                            e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].WaterMass = 0;
                        }
                        if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].LavaMass > 200)
                        {
                            e.Level.Player.Inventory[MainWindow.inventory.Selected].Item = Item.LavaBucket;
                            e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].LavaMass = 0;
                        }
                    }
                }
            });
            WaterBucket = new ToolItem("Water Bucket", "A bucket full of cool water.")
            {
                MaxStack = (int)StackSize.Single,
            };
            WaterBucket.LeftClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.X / Tile.Height, e.Y / Tile.Width), new Vector2(e.Level.Player.Position.X / Tile.Width, e.Level.Player.Position.Y / Tile.Height)) <= 8)
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                        if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].WaterMass < 255)
                        {
                            e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].WaterMass = 255;
                            e.Level.Player.Inventory[MainWindow.inventory.Selected].Item = Item.Bucket;
                        }
            });
            LavaBucket = new ToolItem("Lava Bucket", "A bucket boiling with fiery magma.")
            {
                MaxStack = (int)StackSize.Single,
            };
            LavaBucket.LeftClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.X / Tile.Height, e.Y / Tile.Width), new Vector2(e.Level.Player.Position.X / Tile.Width, e.Level.Player.Position.Y / Tile.Height)) <= 8)
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                        if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].LavaMass < 255)
                        {
                            e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].LavaMass = 255;
                            e.Level.Player.Inventory[MainWindow.inventory.Selected].Item = Item.Bucket;
                        }
            });
            PaintBrush = new ToolItem("Paint Brush", "Applies paint to many surfaces, get creative!")
            {
                MaxStack = (int)StackSize.Few,
            };
            PaintBrush.Select += new SelectItemEventHandler(delegate(object o, SelectItemEventArgs e)
            {
                if (Interface.MainWindow.PaintWindow != null)
                Interface.MainWindow.PaintWindow.Show();
            });
            PaintBrush.DeSelect += new SelectItemEventHandler(delegate(object o, SelectItemEventArgs e)
            {
                if (Interface.MainWindow.PaintWindow != null)
                Interface.MainWindow.PaintWindow.Hide();
            });
            PaintBrush.LeftHold += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.X / Tile.Height, e.Y / Tile.Width), new Vector2(e.Level.Player.Position.X / Tile.Width, e.Level.Player.Position.Y / Tile.Height)) <= 8)
                {
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                    {
                        if (Keyboard.GetState().IsKeyDown(Game.Controls["Place on Background"]))
                        {
                            if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].Background.PaintColors == null)
                                Interface.MainWindow.PaintWindow.DisableColors();
                            else
                                e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height), true].BackgroundPaintColor = (byte)e.Level.SelectedPaintColor;
                        }
                        else
                        {
                            if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].Foreground.PaintColors == null)
                                Interface.MainWindow.PaintWindow.DisableColors();
                            else
                                e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height), true].ForegroundPaintColor = (byte)e.Level.SelectedPaintColor;
                        }
                    }
                }
            });
            PaintBrush.RightClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.X / Tile.Height, e.Y / Tile.Width), new Vector2(e.Level.Player.Position.X / Tile.Width, e.Level.Player.Position.Y / Tile.Height)) <= 6)
                {
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                    {
                        if (Keyboard.GetState().IsKeyDown(Game.Controls["Place on Background"]))
                        {
                            if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height),true].Background.PaintColors != null)
                            {
                                if (e.Level.SelectedPaintColor < e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height),true].Background.PaintColors.Length - 1)
                                    e.Level.SelectedPaintColor++;
                                else
                                    e.Level.SelectedPaintColor = 0;
                                if (Interface.MainWindow.PaintWindow != null)
                                {
                                    Interface.MainWindow.PaintWindow.Select(e.Level.SelectedPaintColor);
                                }
                            }
                            else
                                Interface.MainWindow.PaintWindow.DisableColors();
                        }
                        else
                        {
                            if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].Foreground.PaintColors != null)
                            {
                                if (e.Level.SelectedPaintColor < e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].Foreground.PaintColors.Length - 1)
                                    e.Level.SelectedPaintColor++;
                                else
                                    e.Level.SelectedPaintColor = 0;
                                if (Interface.MainWindow.PaintWindow != null)
                                {
                                    Interface.MainWindow.PaintWindow.Select(e.Level.SelectedPaintColor);
                                }
                            }
                            else
                                Interface.MainWindow.PaintWindow.DisableColors();
                        }
                      
                    }
                }
            });
            Wheat = new ForegroundBlockItem("Wheat", "Great grains!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Grown | BlockRenderMode.Single,
                Size = new Point(1, 2),
                GrowTime = 3000,
                SubType = BlockSubType.Plant,
                GrowStages = 7,
                Burnable = true,
                BreakNoSupportBottom = true,
                BreakTime = 400,
            };
            Asphalt = new ForegroundBlockItem("Asphalt", "Parking space.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                BreakTime = 1300,
            };
            MetalBlock = new ForegroundBlockItem("Metal Block", "A heavy metal block.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                Variations = 10,
                RenderMode = BlockRenderMode.Single,
                PaintColors = StandardColors,
            };
            MetalBlockBG = new BackgroundBlockItem("Metal Block BG");

            SciFi = new ForegroundBlockItem("SciFi", "It's extra terrestrial, pretty good for building too!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
            };
            SciFiBG = new BackgroundBlockItem("SciFi BG");

            WoodenChest = new ForegroundBlockItem("Wooden Chest", "Hide your loot!")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(8, 4),
                Burnable = true,
                PaintColors = StandardColors,
            };
            MetalChest = new ForegroundBlockItem("Metal Chest", "Hide your loot!")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(8, 4),
                Burnable = true,
                PaintColors = StandardColors,
            };
            WoodenCrate = new ForegroundBlockItem("Wooden Crate", "Store your goods.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Platform,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(6,6),
                Burnable = true,
                PaintColors = StandardColors,
            };
            MetalCrate = new ForegroundBlockItem("Metal Crate", "Store your goods.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Platform,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(8,4),
            };
            Diamond = new Item("Diamond", "A valueable and precious jewel.")
            {
                MaxStack = (int)StackSize.Large,
                TimeToSmelt = 5100,
            };
            Ruby = new Item("Ruby", "A blood-red gemstone.")
            {
                MaxStack = (int)StackSize.Large,
                TimeToSmelt = 4300,
            };
            SilverBar = new Item("Silver Bar", "Some shiny money.")
            {
                MaxStack = (int)StackSize.Large,
                TimeToSmelt = 2900,
            };
            CopperBar = new Item("Copper Bar", "A metal with many applications.")
            {
                MaxStack = (int)StackSize.Large,
                TimeToSmelt = 2000,
            };
            GoldBar = new Item("Gold Bar", "A nugget of gold, now 'yer rich.")
            {
                MaxStack = (int)StackSize.Large,
                TimeToSmelt = 3500,
            };
            IronBar = new Item("Iron Bar", "Low cost and high strength.")
            {
                MaxStack = (int)StackSize.Large,
                TimeToSmelt = 2300,
            };
            Coal = new Item("Coal", "Some coal to fuel the fire.")
            {
                MaxStack = (int)StackSize.Large,
                Fuel = 5,
            };
            Quartz = new Item("Quartz", "A very common crystal.")
            {
                MaxStack = (int)StackSize.Large,
            };
            Steel = new Item("Steel", "A heavy duty metal.")
            {
                MaxStack = (int)StackSize.Large,
            };

            Flint = new Item("Flint", "Sharp and useful rock.")
            {
                MaxStack = (int)StackSize.Large,
            };

            DiamondOre = new ForegroundBlockItem("Diamond Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
            };

            RubyOre = new ForegroundBlockItem("Ruby Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
            };

            GoldOre = new ForegroundBlockItem("Gold Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
            };

            SilverOre = new ForegroundBlockItem("Silver Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
            };
            IronOre = new ForegroundBlockItem("Iron Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
            };
            CopperOre = new ForegroundBlockItem("Copper Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,

            };
            CoalOre = new ForegroundBlockItem("Coal Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
                Drop = Coal,
            };
            QuartzOre = new ForegroundBlockItem("Quartz Ore", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                Variations = 5,
                Drop = Quartz,
            };
            GrassPlant = new ForegroundBlockItem("Grass Plant", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Absorb = 16,
                Variations = 2,
                Drop = Item.Blank,
                Burnable = true,
                BreakNoSupportBottom = true,
                AutoFlipVariation = true,
                InstaBreak = true,
            };
            TallGrassPlant = new ForegroundBlockItem("Tall Grass Plant", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Absorb = 16,
                Drop = Item.Blank,
                Burnable = true,
                BreakNoSupportBottom = true,
                AutoFlipVariation = true,
                InstaBreak = true,
            };
            Vine = new ForegroundBlockItem("Vine", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Ladder,
                RenderMode = BlockRenderMode.SpriteSheet,
                Absorb = 13,
                Drop = Item.Blank,
                Burnable = true,
                BreakNoSupportTop = true,
                BreakTime = 1000,
                ClimbUpSpeed = .5f,
                ClimbDownSpeed = 1,
            };
            Toadstool = new ForegroundBlockItem("Toadstool", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Absorb = 16,
                Variations = 2,
                // Drop = Item.Blank,
                Burnable = true,
                BreakNoSupportBottom = true,
                BreakTime = 100,
                AutoFlipVariation = true,
            };
            DeadBush = new ForegroundBlockItem("Dead Bush", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                Absorb = 16,
                Drop = Item.Blank,
                Burnable = true,
                BreakNoSupportBottom = true,
                BreakTime = 100,
                AutoFlipVariation = true,
                InstaBreak = true,
            };
            BellFlower = new ForegroundBlockItem("Bell Flower", "A giant yellow flower.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Grown | BlockRenderMode.Single,
                Size = new Point(1, 2),
                GrowTime = 3000,
                GrowStages = 4,
                SubType = BlockSubType.Plant,
                Burnable = true,
                Variations = 2,
                BreakNoSupportBottom = true,
                BreakTime = 400,
                Absorb = 16,
            };
            Crocosmia = new ForegroundBlockItem("Crocosmia", "An unusual red flower.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Grown | BlockRenderMode.Single,
                GrowTime = 3000,
                GrowStages = 6,
                SubType = BlockSubType.Plant,
                Burnable = true,
                Variations = 2,
                BreakNoSupportBottom = true,
                BreakTime = 400,
                Absorb = 16,
            };
            Clover = new ForegroundBlockItem("Clover", "Lucky.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Plant,
                Burnable = true,
                Variations = 2,
                BreakNoSupportBottom = true,
                BreakTime = 300,
                Absorb = 16,
            };
            Wildflower = new ForegroundBlockItem("Wildflower", "wow. such wild. very color. flower.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                SubType = BlockSubType.Plant,
                Burnable = true,
                AutoFlipVariation = true,
                Variations = 6,
                BreakNoSupportBottom = true,
                BreakTime = 300,
                Absorb = 16,
            };
            CallaLily = new ForegroundBlockItem("Calla Lily", "")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Grown | BlockRenderMode.Single,
                Size = new Point(1, 2),
                GrowTime = 3000,
                GrowStages = 5,
                SubType = BlockSubType.Plant,
                Burnable = true,
                BreakNoSupportBottom = true,
                BreakTime = 400,
                Absorb = 16,
            };
            Mushroom = new ForegroundBlockItem("Mushroom", "Shrooms!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 16,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                BreakNoSupportBottom = true,
                BreakTime = 400,
                AutoFlipVariation = true,
            };
            IceMushroom = new ForegroundBlockItem("Ice Mushroom", "Don't lick it.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Passable,
                Absorb = 16,
                RenderMode = BlockRenderMode.Single,
                Light = Color.RoyalBlue,
                BreakNoSupportBottom = true,
                BreakTime = 400,
                AutoFlipVariation = true,
            };
            CraftingTable = new ForegroundBlockItem("Crafting Table", "Used to craft basic items.")
            {
                MaxStack = (int)StackSize.Small,
                Collision = BlockCollision.Passable,
                Size = new Point(3, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                Station = true,
            };
            Sawmill = new ForegroundBlockItem("Sawmill", "Used for advanced wood work.")
            {
                MaxStack = (int)StackSize.Small,
                Collision = BlockCollision.Passable,
                Size = new Point(3, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                Station = true,
            };
            Anvil = new ForegroundBlockItem("Anvil", "Used to forge metals into useful items.")
            {
                MaxStack = (int)StackSize.Small,
                Collision = BlockCollision.Passable,
                Size = new Point(2, 1),
                RenderMode = BlockRenderMode.Single,
                Flipable = true,
                Station = true,
            };
            Furnace = new ForegroundBlockItem("Furnace", "Smelt materials into new items.")
            {
                MaxStack = (int)StackSize.Small,
                Collision = BlockCollision.Passable,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = false,
                SubType = BlockSubType.Furnace,
                Station = true,
            };
          
            FlintAndSteel = new Item("Flint and Steel", "Have at it, pyros.")
            {
                MaxStack = (int)StackSize.Large,
            };
            FlintAndSteel.LeftHold += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.X / Tile.Height, e.Y / Tile.Width), new Vector2(e.Level.Player.Position.X / Tile.Width, e.Level.Player.Position.Y / Tile.Height)) <= 6)
                {
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)))
                    {
                        //Ignite!
                        if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].BackgroundFireMeta == 0 && e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].Background.Burnable)
                            e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].BackgroundFireMeta = 1;

                        if (e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].ForegroundFireMeta == 0 && e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].Foreground.Burnable)
                            e.Level.tiles[(int)(e.X / Tile.Width), (int)(e.Y / Tile.Height)].ForegroundFireMeta = 1;
                    }
                }
            });
            StonePlatform = new ForegroundBlockItem("Stone Platform", "A sturdy but thin platform.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Platform,
                Burnable = false,
                PaintColors = StandardColors,
            };
            StonePickaxe = new MineItem("Stone Pickaxe", "A basic stone pickaxe.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 1f,
            };
            CopperPickaxe = new MineItem("Copper Pickaxe", "An upgraded pickaxe for mining and harvesting materials.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 1.25f,
            };
            IronPickaxe = new MineItem("Iron Pickaxe", "A heavy and powerful iron pickaxe.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 1.5f,
            };
            SilverPickaxe = new MineItem("Silver Pickaxe", "A high teir silver pickaxe.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 1.75f,
            };
            GoldPickaxe = new MineItem("Gold Pickaxe", "A valueable gold pickaxe.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 1.9f,
            };
            DiamondPickaxe = new MineItem("Diamond Pickaxe", "A finely cut pickaxe made of diamond.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 2.2f,
            };
            OPPickaxe = new MineItem("OP Pickaxe", "Mine like an OP. Use responsibly.")
            {
                MaxStack = (int)StackSize.Single,
                Multiplier = 900.1f,
                Radius = 69,
            };

            Stick = new Item("Stick", "Useful for many tools.")
            {
                MaxStack = (int)StackSize.Max,
            };

            PicketFence = new BackgroundBlockItem("Picket Fence", "A decorative boundary.")
            {
                MaxStack = (int)StackSize.Medium,
                Absorb = 0,
            };
            WoodFence = new BackgroundBlockItem("Wood Fence", "Standard fence to enclose your yard.")
            {
                MaxStack = (int)StackSize.Medium,
                Absorb = 0,
            };
            ChainLinkFence = new BackgroundBlockItem("Chain Link Fence", "Sturdy fence to enclose your buildings.")
            {
                MaxStack = (int)StackSize.Medium,
                Absorb = 0,
            };
            Spike = new ForegroundBlockItem("Spike", "Ouch!")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Spike,
                EdgeMode = BlockEdgeMode.Stick,
            };
            WoodSupport = new ForegroundBlockItem("Wood Support", "A wooden support beam.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                BlockMap = new bool[2,2] 
                {
                    { true, true },
                    { true, false }
                },
                PaintColors = StandardColors,
            };
            WoodPlankSupport = new ForegroundBlockItem("Wood Plank Support", "A wooden support beam.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Passable,
                Size = new Point(2, 2),
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Flipable = true,
                BlockMap = new bool[2, 2] 
                {
                    { true, true },
                    { true, false }
                },
                PaintColors = StandardColors,
            };
            WoodStair = new ForegroundBlockItem("Wood Stair", "A nice addition to your home.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Stair,
                Burnable = true,
                Flipable = true,
                PaintColors = StandardColors,
            };
            WoodPlankStair = new ForegroundBlockItem("Wood Plank Stair", "A nice addition to your home.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Stair,
                Burnable = true,
                Flipable = true,
                PaintColors = StandardColors,
            };
            StoneStair = new ForegroundBlockItem("Stone Stair", "Sturdy but not so pretty stairs.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Stair,
                Burnable = true,
                Flipable = true,
                PaintColors = StandardColors,
            };
            MetalStair = new ForegroundBlockItem("Metal Stair", "A durable metal stair.")
            {
                MaxStack = (int)StackSize.Medium,
                Collision = BlockCollision.Stair,
                Burnable = true,
                Flipable = true,
                PaintColors = StandardColors,
            };
            PlainWallpaper = new BackgroundBlockItem("Plain Wallpaper", "A basic backdrop for your home's walls. Goes well with painting.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            LargeCheckeredWallpaper = new BackgroundBlockItem("Large Checkered Wallpaper", "Decorate your home with this nice pattern.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            SmallCheckeredWallpaper = new BackgroundBlockItem("Small Checkered Wallpaper", "Decorate your home with this nice pattern.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            FancyCheckeredWallpaper = new BackgroundBlockItem("Fancy Checkered Wallpaper", "Decorate your home with this great pattern")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            DottedWallpaper = new BackgroundBlockItem("Dotted Wallpaper", "Decorate your home with this simple pattern.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            TexturedWallpaper = new BackgroundBlockItem("Textured Wallpaper", "Decorate your home with this rough wallpaper.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            SmoothWallpaper = new BackgroundBlockItem("Smooth Wallpaper", "Decorate your home with this smooth design.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                BreakTime = 500,
            };
            TiledWallpaper = new BackgroundBlockItem("Tiled Wallpaper", "Decorate your home with this simple pattern.")
            {
                MaxStack = (int)StackSize.Max,
                Burnable = true,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                BreakTime = 500,
            };
            CeramicTile = new ForegroundBlockItem("Ceramic Tile", "Perfect on the floors, or the walls.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                PaintColors = StandardColors,
                Variations = 5,
                RenderMode = BlockRenderMode.Single,
            };
            CeramicTileBG = new BackgroundBlockItem("Ceramic Tile BG");

            WoodPedestal = new ForegroundBlockItem("Wood Pedestal", "Great for showcasing your stuff!")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Collision = BlockCollision.Platform,
                Size = new Point(1,2),
            };
            Bookshelf = new ForegroundBlockItem("Bookshelf", "A nice area to store your novels!")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                Collision = BlockCollision.Platform,
                Burnable = true,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(5,3),
                Size = new Point(3,4),
                Variations = 2,
                Flipable = true,
                BreakTime = 2000,
            };
            WoodenDresser = new ForegroundBlockItem("Wooden Dresser", "Storage for your clothes.")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                Collision = BlockCollision.Platform,
                Burnable = true,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(5, 4),
                Size = new Point(3,3),
                Flipable = true,
                BreakTime = 2000,
            };
            WoodenDesk = new ForegroundBlockItem("Wooden Desk", "A storage space and place to work.")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                Collision = BlockCollision.Platform,
                Burnable = true,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(5, 4),
                Size = new Point(4, 2),
                Flipable = true,
                BreakTime = 2000,
            };
            WoodenCloset =  new ForegroundBlockItem("Wooden Closet", "A small closet.")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                Collision = BlockCollision.Platform,
                Burnable = true,
                SubType = BlockSubType.Storage,
                StorageSlots = new Point(5, 4),
                Size = new Point(3, 4),
                Flipable = true,
                BreakTime = 2000,
            };
            WoodenStool = new ForegroundBlockItem("Wooden Stool", "Reach a high place!")
            {
                MaxStack = (int)StackSize.Medium,
                PaintColors = StandardColors,
                RenderMode = BlockRenderMode.Single,
                Burnable = true,
                Collision = BlockCollision.Platform,
            };
            LargeStalagtite = new ForegroundBlockItem("Large Stalagtite", "")
            {
                Drop = Item.Stone.Drop,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                MaxStack = (int)StackSize.Max,
                Size = new Point(1,2),
                Variations = 6,
                AutoFlipVariation = true,
            };
            SmallStalagtite = new ForegroundBlockItem("Small Stalagtite", "")
            {
                Drop = Item.Stone.Drop,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single,
                MaxStack = (int)StackSize.Max,
                Variations = 10,
                AutoFlipVariation = true,
            };
            Basketball = new PhysicsItem("Basketball", "Slam, Dunk!")
            {
                MaxStack = (int)StackSize.Medium,
                Size = new Vector2(24, 24),
                Diameter = 24,
                RotationDiameter = 24,
                LaunchSpeed = 700,
            };
            Basketball.ClickWorld += new MouseItemWorldEventHandler(delegate(object o, MouseItemWorldEventArgs e)
            {
                e.Entity.Velocity = new Vector2(e.Entity.Velocity.X, e.Entity.Velocity.Y - 250);
            });
            Glowstick = new PhysicsItem("Glowstick", "")
            {
                MaxStack = (int)StackSize.Medium,
                Size = new Vector2(24, 24),
                Diameter = 4,
                RotationDiameter = 24,
                Gravity = new Vector2(0, 20),
                GravityMultiplier = new Vector2(1.03f, 1.03f),
                CollisionMultiplier = new Vector2(1, 1.2f),
                Color = new Color(0, 220, 35),
                LaunchSpeed = 700,
                EmitColor = true,
            };
            Wirer = new ToolItem("Wire", "Used for connecting circuts.")
            {
                MaxStack = (int)StackSize.Few,
            };
            Wirer.LeftClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                int gridX = e.X / Tile.Width;
                int gridY = e.Y / Tile.Width;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        Tile tile =  e.Level.tiles[x, y];
                        if (tile.Foreground.Inputs != null)
                            for (int i = 0; i < tile.Foreground.Inputs.Length; i++)
                            {
                                ConnectionPoint cp = tile.Foreground.Inputs[i];
                                Vector2 pos = ConnectionPoint.GetDrawPosition(cp, tile, x, y);
                                if (new Rectangle((int)pos.X, (int)pos.Y, ConnectionPoint.PointWidth, ConnectionPoint.PointHeight).Contains(new Point(e.X, e.Y)))
                                {
                                    if (e.Level.isMakingWires)
                                    {
                                        //if (e.Level.currentWire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Output)
                                        //{
                                            e.Level.currentWire.Connection2 = tile;
                                            e.Level.currentWire.ConnectionPoint2 = cp;
                                            e.Level.currentWire.ConnectionID2 = i;
                                            e.Level.isMakingWires = false;
                                            (tile as ElectronicTile).Inputs[i].Add(e.Level.currentWire);
                                        //}
                                    }
                                    else
                                    {
                                        Wire wire = new Wire() { Connection1 = tile, ConnectionID1 = i, ConnectionPoint1 = cp };
                                        (tile as ElectronicTile).Inputs[i].Add(wire);
                                        e.Level.currentWire = wire;
                                        e.Level.Wires.Add(e.Level.currentWire);
                                        e.Level.isMakingWires = true;
                                    }
                                }
                            }
                        if (tile.Foreground.Outputs != null)
                        for (int i = 0; i < tile.Foreground.Outputs.Length; i++)
                        {
                            ConnectionPoint cp = tile.Foreground.Outputs[i];
                            Vector2 pos = ConnectionPoint.GetDrawPosition(cp, tile, x, y);
                            if (new Rectangle((int)pos.X, (int)pos.Y, ConnectionPoint.PointWidth, ConnectionPoint.PointHeight).Contains(new Point(e.X, e.Y)))
                            {
                                if (e.Level.isMakingWires)
                                {
                                    if (e.Level.currentWire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
                                    {
                                        e.Level.currentWire.Connection2 = tile;
                                        e.Level.currentWire.ConnectionPoint2 = cp;
                                        e.Level.currentWire.ConnectionID2 = i;
                                        e.Level.isMakingWires = false;
                                        (tile as ElectronicTile).Outputs[i].Add(e.Level.currentWire);
                                    }
                                }
                                else
                                {
                                    Wire wire = new Wire() { Connection1 = tile, ConnectionID1 = i, ConnectionPoint1 = cp };
                                    (tile as ElectronicTile).Outputs[i].Add(wire);
                                    e.Level.currentWire = wire;
                                    e.Level.Wires.Add(e.Level.currentWire);
                                    e.Level.isMakingWires = true;
                                }
                            }
                        }
                    }
            });
            Wirer.RightClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                int gridX = e.X / Tile.Width;
                int gridY = e.Y / Tile.Width;
                for (int x = gridX - 1; x <= gridX + 1; x++)
                    for (int y = gridY - 1; y <= gridY + 1; y++)
                    {
                        Tile tile = e.Level.tiles[x, y];
                        if (tile.Foreground.Inputs != null)
                            for (int i = 0; i < tile.Foreground.Inputs.Length; i++)
                            {
                                ConnectionPoint cp = tile.Foreground.Inputs[i];
                                Vector2 pos = ConnectionPoint.GetDrawPosition(cp, tile, x, y);
                                if (new Rectangle((int)pos.X, (int)pos.Y, ConnectionPoint.PointWidth, ConnectionPoint.PointHeight).Contains(new Point(e.X, e.Y)))
                                {
                                    if ((tile as ElectronicTile).Inputs[i].Count > 0)
                                    {
                                        Wire w = (tile as ElectronicTile).Inputs[i][(tile as ElectronicTile).Inputs[i].Count - 1];
                                        w.Powered = false;
                                        w.PowerChanged(w.Powered, false);
                                        (tile as ElectronicTile).Inputs[i].Remove(w);
                                        if (tile == w.Connection1)
                                        {
                                            if (w.Connection2 != null && (w.Connection2 as ElectronicTile).Outputs != null && (w.Connection2 as ElectronicTile).Outputs.Length > w.ConnectionID2)
                                            (w.Connection2 as ElectronicTile).Outputs[w.ConnectionID2].Remove(w);
                                        }
                                        else
                                        {
                                            if (w.Connection1 != null && (w.Connection1 as ElectronicTile).Outputs != null && (w.Connection1 as ElectronicTile).Outputs.Length > w.ConnectionID1)
                                            (w.Connection1 as ElectronicTile).Outputs[w.ConnectionID1].Remove(w);
                                        }
              
                                        e.Level.Wires.Remove(w);
                                    }
                                }
                            }
                        if (tile.Foreground.Outputs != null)
                            for (int i = 0; i < tile.Foreground.Outputs.Length; i++)
                            {
                                ConnectionPoint cp = tile.Foreground.Outputs[i];
                                Vector2 pos = ConnectionPoint.GetDrawPosition(cp, tile, x, y);
                                if (new Rectangle((int)pos.X, (int)pos.Y, ConnectionPoint.PointWidth, ConnectionPoint.PointHeight).Contains(new Point(e.X, e.Y)))
                                {
                                    if ((tile as ElectronicTile).Outputs[i].Count > 0)
                                    {
                                        Wire w = (tile as ElectronicTile).Outputs[i][(tile as ElectronicTile).Outputs[i].Count - 1];
                                        (tile as ElectronicTile).Outputs[i].Remove(w);
                                        if (tile == w.Connection1)
                                        {
                                            (w.Connection2 as ElectronicTile).Inputs[w.ConnectionID2].Remove(w);
                                            if ((w.Connection2 as ElectronicTile).Outputs != null &&(w.Connection2 as ElectronicTile).Outputs.Length > 0)
                                                foreach (List<Wire> wires in (w.Connection2 as ElectronicTile).Outputs)
                                                    foreach (Wire wire in wires)
                                                        wire.ReCalc = true;
                                        }
                                        else
                                        {
                                            (w.Connection1 as ElectronicTile).Inputs[w.ConnectionID1].Remove(w);
                                            if ((w.Connection1 as ElectronicTile).Outputs != null && (w.Connection1 as ElectronicTile).Outputs.Length > 0)
                                                foreach (List<Wire> wires in (w.Connection1 as ElectronicTile).Outputs)
                                                    foreach (Wire wire in wires)
                                                        wire.ReCalc = true;
                                        }
                                        e.Level.Wires.Remove(w);
                                    }
                                }
                            }
                    }
            });
            PowerSource = new ForegroundBlockItem("Power Source", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Output) },
            };
            PowerSource.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return true;
            });
            PowerSource.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return 1;
            });
            Lever = new ForegroundBlockItem("Lever", "Wonder what this does...")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.SpriteSheet | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                PlaceMode = BlockPlaceMode.AllButTop,
                EdgeMode = BlockEdgeMode.Stick,
                AutoPlay = false,
                NeedsWallSupport = true,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input) },
            };
            Lever.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                bool input = Wire.GetInputPower(0, e);
                return input && ((e.connectionNum == 1 ? e.wire.Connection1 : e.wire.Connection2)as ElectronicTile).State == 1;
            });
            Lever.Interact += new InteractBlockEventHandler(delegate (object o, InteractBlockEventArgs e)
                {
                    ElectronicTile t = (e.level.tiles[e.x, e.y] as ElectronicTile);
                    if (t.State == 0)
                    t.State = 1;
                    else
                    t.State = 0;
                    foreach (Wire w in t.Outputs[0])
                        w.Powered = t.State == 1 && t.Inputs[0].Any(x2 => x2.Powered);
                });

            Button = new ForegroundBlockItem("Button", "Wonder what this does...")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.SpriteSheet | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                PlaceMode = BlockPlaceMode.AllButTop,
                EdgeMode = BlockEdgeMode.Stick,
                AutoPlay = false,
                NeedsWallSupport = true,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input) },
            };
            Button.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                bool input = Wire.GetInputPower(0, e);
                return input && ((e.connectionNum == 1 ? e.wire.Connection1 : e.wire.Connection2) as ElectronicTile).State == 1;
            });
            Button.Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                ElectronicTile t = (e.level.tiles[e.x, e.y] as ElectronicTile);
                if (t.State == 0 && t.Inputs[0].Any(x =>x.Powered))
                {
                    t.State = 1;
                    foreach (Wire w in t.Outputs[0])
                        w.Powered = t.State == 1 && t.Inputs[0].Any(x2 => x2.Powered);
                    e.level.Actions.Enqueue(new QueueItem(
                        () =>
                        {
                                t.State = 0;
                                foreach (Wire w in t.Outputs[0])
                                    w.Powered = false;
                        }, 1000, false)
                    );
                }
            });
            Indicator = new ForegroundBlockItem("Indicator", "")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode =  BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input) },
            };
            Indicator.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            ANDGate = new ForegroundBlockItem("AND Gate", "If both inputs are high, output will be high, otherwise, low")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomLeft, ConnectionPoint.ConnectionType.Input) },
            };
            ANDGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return Wire.GetInputPower(0, e) && Wire.GetInputPower(1, e);
            });
            ANDGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return ((Wire.GetInputState(0, e)) && (Wire.GetInputState(1, e))) ? 1 : 0;
            });
            ORGate = new ForegroundBlockItem("OR Gate", "If any inputs are high, output will be high, otherwise low")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomLeft, ConnectionPoint.ConnectionType.Input) },
            };
            ORGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return Wire.GetInputPower(0, e) || Wire.GetInputPower(1, e);
            });
            ORGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return ((Wire.GetInputState(0, e)) || (Wire.GetInputState(1, e))) ? 1 : 0;
            });
            NORGate = new ForegroundBlockItem("NOR Gate", "If both inputs are low, the output is high, otherwise low")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomLeft, ConnectionPoint.ConnectionType.Input) },
            };
            NORGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return !Wire.GetInputPower(0, e) && !Wire.GetInputPower(1, e);
            });
            NORGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return (!(Wire.GetInputState(0, e)) && !(Wire.GetInputState(1, e))) ? 1 : 0;
            });

            NOTGate = new ForegroundBlockItem("NOT Gate", "Outputs the inverted input.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input) },
            };
            NOTGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return !Wire.GetInputPower(0, e);
            });
            NOTGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return (!(Wire.GetInputState(0, e))) ? 1 : 0;
            });

            NANDGate = new ForegroundBlockItem("NAND Gate", "Outputs low if both inputs are high, otherwise low")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomLeft, ConnectionPoint.ConnectionType.Input) },
            };
            NANDGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return !(Wire.GetInputPower(0, e) && Wire.GetInputPower(1, e));
            });
            NANDGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return !((Wire.GetInputState(0, e)) && (Wire.GetInputState(1, e))) ? 1 : 0;
            });
            XORGate = new ForegroundBlockItem("XOR Gate", "Outputs high if one input is high, but not both.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomLeft, ConnectionPoint.ConnectionType.Input) },
            };
            XORGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return ((Wire.GetInputPower(0, e) && !Wire.GetInputPower(1, e)) || (!Wire.GetInputPower(0, e) && Wire.GetInputPower(1, e)));
            });
            XORGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return (((Wire.GetInputState(0, e) && !(Wire.GetInputState(1, e)) || (!Wire.GetInputState(0, e) && (Wire.GetInputState(1, e))) ? 1 : 0)));
            });
            XNORGate = new ForegroundBlockItem("XNOR Gate", "Outputs high if both outputs are high, otherwise, outputs low.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.TopLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomLeft, ConnectionPoint.ConnectionType.Input) },
            };
            XNORGate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return ((Wire.GetInputPower(0, e) && Wire.GetInputPower(1, e)) || (!Wire.GetInputPower(0, e) && !Wire.GetInputPower(1, e)));
            });
            XNORGate.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return (((Wire.GetInputState(0, e) && (Wire.GetInputState(1, e)) || (!Wire.GetInputState(0, e) && (!Wire.GetInputState(1, e))) ? 1 : 0)));
            });

            Diode = new ForegroundBlockItem("Diode", "A one way electrical component.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input) },
            };
            Diode.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return Wire.GetInputPower(0, e);
            });
            Diode.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return (Wire.GetInputState(0, e)) ? 1 : 0;
            });
            NPNTransistor = new ForegroundBlockItem("NPN Transistor", "Will allow flow from it's collector to it's emitter if it's base is powered.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomCenter, ConnectionPoint.ConnectionType.Input) },
            };
            NPNTransistor.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                if (Wire.GetInputPower(1, e))
                    return Wire.GetInputPower(0, e);
                else
                    return false;
            });
            NPNTransistor.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                if (Wire.GetInputState(1, e))
                    return (Wire.GetInputState(0, e)) ? 1 : 0;
                else
                    return 0;
            });
            PNPTransistor = new ForegroundBlockItem("PNP Transistor", "Will allow flow from it's emitter to it's collector if it's base is not powered.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[2] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input), new ConnectionPoint(ConnectionPoint.ConnectionPreset.BottomCenter, ConnectionPoint.ConnectionType.Input) },
            };
            PNPTransistor.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                if (!Wire.GetInputPower(1, e))
                    return Wire.GetInputPower(0, e);
                else
                    return false;
            });
            PNPTransistor.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                if (!Wire.GetInputState(1, e))
                    return (Wire.GetInputState(0, e)) ? 1 : 0;
                else
                    return 0;
            });
            MetalBars = new ForegroundBlockItem("Metal Bars", "Strong metal bars.")
            {
                MaxStack = (int)StackSize.Max,
                Collision = BlockCollision.Impassable,
                SkyLight = true,
            };
            NPNTransistor = new ForegroundBlockItem("NPN Transistor", "Will allow flow from it's collector to it's emitter if it's base is powered.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) }, 
            };
            Timer = new ForegroundBlockItem("Timer", "Wonder what this does...")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Impassable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                SubType = BlockSubType.Timer,
                FrameCount = 2,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input) },
            };
            Timer.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                return ((e.connectionNum == 1 ? e.wire.Connection1 : e.wire.Connection2) as ElectronicTile).State == 1 && Wire.GetInputPower(0,e) ? true : false;
            });
            Timer.Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                //Text opens a new sign window
                TaskTimer tmp = new TaskTimer(e.level.game.Manager, e);
                tmp.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
                tmp.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
                tmp.Init();
                e.level.game.Manager.Add(tmp);
                tmp.Show();
            });
            WallLamp = new ForegroundBlockItem("Wall Lamp", "An outdoor wall lamp.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.SpriteSheet | BlockRenderMode.Animation,
                EdgeMode = BlockEdgeMode.Stick,
                PlaceMode = BlockPlaceMode.AllButTop,
                SubType = BlockSubType.Electronic,
                Light = Color.White,
                Absorb = 10,
                NeedsWallSupport = true,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleCenter, ConnectionPoint.ConnectionType.Input) },
            };
            WallLamp.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            TableLamp = new ForegroundBlockItem("Table Lamp", "A tall table lamp.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single| BlockRenderMode.Animation,
                Size = new Point(1,2),
                SubType = BlockSubType.Electronic,
                Light = Color.White,
                Absorb = 10,
                BreakNoSupportBottom = true,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleCenter, ConnectionPoint.ConnectionType.Input) },
            };
            TableLamp.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            FluorescentLight = new ForegroundBlockItem("Fluorescent Light", "A bright fluorescent light fixture.")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single | BlockRenderMode.Animation,
                Size = new Point(4,1),
                SubType = BlockSubType.Electronic,
                Light = Color.White,
                Absorb = 10,
                BreakNoSupportTop = true,
                PlaceMode = BlockPlaceMode.Hanging,
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleCenter, ConnectionPoint.ConnectionType.Input) },
            };
            FluorescentLight.GetElectronicState += new GetElectronicStateEventHandler(delegate(object o, GetElectronicStateEventArgs e)
            {
                return Wire.GetInputState(0, e) ? 1 : 0;
            });
            PressurePlate = new ForegroundBlockItem("Pressure Plate", "Better watch your step...")
            {
                MaxStack = (int)StackSize.Large,
                Collision = BlockCollision.Passable,
                RenderMode = BlockRenderMode.Single| BlockRenderMode.Animation,
                SubType = BlockSubType.Electronic,
                FrameCount = 2,
                PlaceMode = BlockPlaceMode.Bottom,
                BreakNoSupportBottom = true,
                AutoPlay = false,
                Outputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleRight, ConnectionPoint.ConnectionType.Output) },
                Inputs = new ConnectionPoint[1] { new ConnectionPoint(ConnectionPoint.ConnectionPreset.MiddleLeft, ConnectionPoint.ConnectionType.Input) },
            };
            PressurePlate.OutputRequested += new RequestElectronicOutputEventHandler(delegate(object o, RequestElectronicOutputEventArgs e)
            {
                bool input = Wire.GetInputPower(0, e);
                return input && ((e.connectionNum == 1 ? e.wire.Connection1 : e.wire.Connection2) as ElectronicTile).State == 1;
            });
            MetalBarsBG = new BackgroundBlockItem("Metal Bars BG");



            Dirt.BlendEdge = new List<BlockItem>() { Item.Grass };

            SandStone.SmoothBlend = Item.Sand;
            Stone.SmoothBlend = Item.Dirt;
            Sand.SmoothBlend = Item.Dirt;
            Gravel.SmoothBlend = Item.Dirt;
            Mud.SmoothBlend = Item.Dirt;
            MudBG.SmoothBlend = Item.DirtBG;
            Clay.SmoothBlend = Item.Dirt;
            ClayBG.SmoothBlend = Item.DirtBG;
            StoneBG.SmoothBlend = Item.DirtBG;
            DiamondOre.SmoothBlend = Item.Dirt;
            RubyOre.SmoothBlend = Item.Dirt;
            GoldOre.SmoothBlend = Item.Dirt;
            CopperOre.SmoothBlend = Item.Dirt;
            SilverOre.SmoothBlend = Item.Dirt;
            QuartzOre.SmoothBlend = Item.Dirt;
            CoalOre.SmoothBlend = Item.Dirt;
            IronOre.SmoothBlend = Item.Dirt;


            SandStone.BlendEdge = new List<BlockItem> { Item.Sand };
            StoneBG.BlendEdge = new List<BlockItem>() { Item.DirtBG };
            Clay.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass };
            ClayBG.BlendEdge = new List<BlockItem>() { Item.DirtBG };
            MudBG.BlendEdge = new List<BlockItem>() { Item.DirtBG };
            Mud.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass };
            Sand.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass };
            Gravel.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass };
            Grass.BlendEdge = new List<BlockItem>() { Item.Dirt };
            Stone.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.GoldOre, Item.CopperOre, Item.SilverOre, Item.QuartzOre, Item.CoalOre, Item.IronOre };
            DiamondOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.Stone, Item.RubyOre, Item.GoldOre, Item.CopperOre, Item.SilverOre, Item.QuartzOre, Item.CoalOre, Item.IronOre };
            RubyOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.Stone, Item.GoldOre, Item.CopperOre, Item.SilverOre, Item.QuartzOre, Item.CoalOre, Item.IronOre };
            GoldOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.Stone, Item.CopperOre, Item.SilverOre, Item.QuartzOre, Item.CoalOre, Item.IronOre };
            CopperOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.GoldOre, Item.Stone, Item.SilverOre, Item.QuartzOre, Item.CoalOre, Item.IronOre };
            SilverOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.GoldOre, Item.CopperOre, Item.Stone, Item.QuartzOre, Item.CoalOre, Item.IronOre };
            QuartzOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.GoldOre, Item.CopperOre, Item.SilverOre, Item.Stone, Item.CoalOre, Item.IronOre };
            CoalOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.GoldOre, Item.CopperOre, Item.SilverOre, Item.QuartzOre, Item.Stone, Item.IronOre };
            IronOre.BlendEdge = new List<BlockItem>() { Item.Dirt, Item.Grass, Item.DiamondOre, Item.RubyOre, Item.GoldOre, Item.CopperOre, Item.SilverOre, Item.QuartzOre, Item.CoalOre, Item.Stone };

            System.Diagnostics.Debug.WriteLine("Info: Items Created. (Total: " + ItemList.Count() + ")");
        }


        public static Item FindItem(int ID)
        {
            return ItemList.Find(item => item.ID == ID);
        }
        public static Item FindItem(string name)
        {
            return ItemList.Find(item => item.Name == name);
        }
    }

    public enum StackSize
    {
        /// <summary>
        /// 0
        /// </summary>
        None = 0,
        /// <summary>
        /// 1
        /// </summary>
        Single = 1,
        /// <summary>
        /// 10
        /// </summary>
        Few = 10,
        /// <summary>
        /// 50
        /// </summary>
        Small = 50,
        /// <summary>
        /// 100
        /// </summary>
        Medium = 100,
        /// <summary>
        /// 250
        /// </summary>
        Large = 250,
        /// <summary>
        /// 1000
        /// </summary>
        Max = 1000,
    }
    public class MeleeWepItem : Item
    {
        public Vector2 Size;
        public MeleeWepItem(string name, string description = null)
            : base(name, description)
        {

        }
    }
    public class RangedWepItem : Item
    {
        public Vector2 Size;
        public RangedWepItem(string name, string description = null)
            : base(name, description)
        {

        }
    }
    /// <summary>
    /// A mining item used to mine blocks
    /// </summary>
    public class MineItem : Item
    {
        //Timing
        private static double BreakFrame;
        private static double LastBreak;

        /// <summary>
        /// The multiplier value used to determine the mining speed, 1 is normal speed while 2 is twice the speed.
        /// </summary>
        public float Multiplier { get; set; }

        /// <summary>
        /// How far you can mine with this pickaxe
        /// </summary>
        public int Radius { get; set; }

        public MineItem(string name, string description = null)
            : base(name, description)
        {
            //Default multiplier value
            Multiplier = 1f;
            Radius = 6;

            PrintStats += new PrintItemDataEventHandler(delegate(object o, PrintItemDataEventArgs e)
            {
                List<string> str = new List<string>();
                str.Add("Max Stack: " + MaxStack);
                str.Add("Mining Radius: " + Radius);
                str.Add("Power: " + (int)(((Multiplier - 1) * 10) + ((Multiplier ==1) ? 1 : 0)));
                return str.ToArray();
            });
            //btnLeft hold event for clicking to mine
            base.LeftHold += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                if (e.Level.Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item == Item.Blank || e.Level.Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item is MineItem)
                {
                    //Setup positions
                    BlockItem block = Item.Blank;
                    int gridX = e.X / Tile.Width;
                    int gridY = e.Y / Tile.Height;
                    if (e.Level.CanMine(gridX, gridY, this))
                    {
                        int preGridX = e.lastX / Tile.Width;
                        int preGridY = e.lastY / Tile.Height;

                        //Get the currently mining block
                        if (e.Level.tiles[gridX, gridY].Foreground != Item.Blank && !Keyboard.GetState().IsKeyDown(Game.Controls["Place on Background"]))
                            block = e.Level.tiles[gridX, gridY].Foreground;
                        else if (e.Level.tiles[gridX, gridY, true].Background != Item.Blank)
                            block = e.Level.tiles[gridX, gridY, true].Background;
                        

                        float _BreakTime = block.BreakTime;
                        if (e.Level.Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item != Item.Blank)
                            _BreakTime = block.BreakTime / (e.Level.Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item as MineItem).Multiplier;
                        if (block != Item.Blank)
                        {
                            if (preGridX == gridX && preGridY == gridY)
                            {
                                Color color = Color.White;
                                if (block is ForegroundBlockItem && e.Level.tiles[gridX, gridY].Foreground.PaintColors != null)
                                    color = Level.GetPaintColor(e.Level.tiles[gridX, gridY], e.Level.tiles[gridX, gridY,true], Color.White, false);
                                else if (block is BackgroundBlockItem && e.Level.tiles[gridX, gridY].Background.PaintColors != null)
                                    color = Level.GetPaintColor(e.Level.tiles[gridX, gridY], e.Level.tiles[gridX, gridY, true], Color.White, true);
                                BreakFrame += e.GameTime.ElapsedGameTime.TotalMilliseconds * Multiplier;

                                LastBreak = e.GameTime.TotalGameTime.TotalMilliseconds;
                                e.Level.DefaultParticleEngine.SpawnItemParticle(ParticleType.ItemFall, (gridX * Tile.Width) + (Tile.Width / 2), (gridY * Tile.Height) + (Tile.Height / 2), block, color);

                                if (BreakFrame >= _BreakTime)
                                {
                                    for (int i = 0; i < 10; i++)
                                        e.Level.DefaultParticleEngine.SpawnItemParticle(ParticleType.ItemFall, (gridX * Tile.Width) + (Tile.Width / 2), (gridY * Tile.Height) + (Tile.Height / 2), block, color);

                                    BreakFrame = 0;
                                    if (block is ForegroundBlockItem)
                                        e.Level.tiles[gridX, gridY].Foreground.OnDrop(new DropBlockEventArgs(e.Level, gridX, gridY, block));
                                    else if (block is BackgroundBlockItem)
                                        e.Level.tiles[gridX, gridY].Background.OnDrop(new DropBlockEventArgs(e.Level, gridX, gridY, block));

                                    e.Level.tiles[gridX, gridY].ForegroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX, gridY].BackgroundSheet = Rectangle.Empty;
                                                  
                                    e.Level.tiles[gridX + 1,gridY].ForegroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX + 1,gridY].BackgroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX - 1,gridY].ForegroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX - 1,gridY].BackgroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX, gridY + 1].ForegroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX, gridY + 1].BackgroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX, gridY - 1].ForegroundSheet = Rectangle.Empty;
                                    e.Level.tiles[gridX, gridY - 1].BackgroundSheet = Rectangle.Empty;
                                }
                            }
                            else
                            {
                                LastBreak = e.GameTime.TotalGameTime.TotalMilliseconds;
                                BreakFrame = 0;
                            }
                        }
                    }
                }
            });
        }
    }
    }
    public class PhysicsItem : Item
    {
        public Vector2 Size { get; set; }
        public float Diameter { get; set; }
        public float RotationDiameter { get; set; }
        public Vector2 Gravity { get; set; }
        public Vector2 GravityMultiplier { get; set; }
        public Vector2 CollisionMultiplier { get; set; }
        public Color Color { get; set; }
        public bool EmitColor { get; set; }
        public float LaunchSpeed { get; set; }

        public event MouseItemWorldEventHandler ClickWorld;
        public virtual void OnClickWorld(MouseItemWorldEventArgs e)
        {
            if (ClickWorld != null) ClickWorld(this, e);
        }

        public PhysicsItem(string name, string description = null)
            : base(name, description)
        {
            Diameter = Size.X;
            Gravity = new Vector2(0, 35);
            GravityMultiplier = new Vector2(1.006f, 1.006f);
            CollisionMultiplier = new Vector2(1, 1);
            Color = Color.White;
            EmitColor = false;
            LaunchSpeed = 500;
            RotationDiameter = 0;

            LeftClick += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                Vector2 direction = new Vector2(e.X, e.Y) - e.Level.Player.OriginPosition;
                direction.Normalize();
                float rotation = (float)Math.Atan2((double)direction.Y,
                                             (double)direction.X) + MathHelper.PiOver2; //this will return the angle(in radians) from sprite to mouse.
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(rotation - MathHelper.PiOver2),
                    (float)Math.Sin(rotation - MathHelper.PiOver2)) * LaunchSpeed;
                e.Level.PhysicsEntities.Add(new PhysicsEntity(e.Level, this, e.Level.Player.OriginPosition) { Velocity = velocity });
            });
        }
    }   
    public class ToolItem : Item
    {
        public ToolItem(string name, string description = null)
            : base(name, description)
        {
        }
    }
    public class BlockItem : Item
    {
        public Color[] PaintColors;
        public string[] PaintFiles;
        public BlockSubType SubType = BlockSubType.Default;
        /// <summary>
        /// Used to give blocks borders between others, so torches can stick, blocks can have edges, etc
        /// </summary>
        public BlockEdgeMode EdgeMode = BlockEdgeMode.Edge;
        public BlockPlaceMode PlaceMode = BlockPlaceMode.Edge;
        /// <summary>
        /// A list of items that this block will blend with instead of creating a border when placed next to it
        /// </summary>
        public List<BlockItem> BlendEdge;
        /// The kind of collision the block has, wether is is passable, non passable, etc
        /// </summary>
        public BlockCollision Collision;

        /// <summary>
        /// The amount of light the block "absorbs" when light flows through it
        /// </summary>
        public int Absorb;
        /// <summary>
        /// Defines if the item will light up the player, like a torch
        /// </summary>
        public bool LightHand;
        /// <summary>
        /// Defines if the item breaks when another item is place on it (Ex: Grass breaks instantly)
        /// </summary>
        public bool InstaBreak;
        /// <summary>
        /// The color of light emited from the block (Black = none)
        /// </summary>
        public Color Light = Color.Black;

        public bool ElectronicLight
        {
            get
            {
                return ((SubType == BlockSubType.Electronic || SubType == BlockSubType.Timer) && Light != Color.Black);
            }
        }

        /// <summary>
        /// The size in blocks of the item
        /// </summary>
        public Point Size
        {
            get { return size; }
            set
            {
                size = value;
                BlockMap = new bool[size.X, size.Y];
                for (int x = 0; x < size.X; x++)
                    for (int y = 0; y < size.Y; y++)
                        BlockMap[x, y] = true;
            }
        }
        private Point size = Item.One;

        /// <summary>
        /// How many variations the tile can have
        /// </summary>
        public int Variations = 1;
        /// <summary>
        /// Indicates if the variation will be given automaticly, or if you should manually define it
        /// </summary>
        public bool AutoVariation { get { return autoVariation; } set { autoVariation = value; Flipable = true; } }
        private bool autoVariation;
        public bool FrameImportant { get; set; }
        public bool AnimationFrameImportant { get { return animationFrameImportant || (this.SubType == BlockSubType.Electronic); } set { animationFrameImportant = value; } }
        private bool animationFrameImportant;
        /// <summary>
        /// Indicates if the variation can be varied by automaticly flipping the sprite
        /// </summary>
        public bool AutoFlipVariation;
        /// <summary>
        /// How many animation frames the block has
        /// </summary>
        public int FrameCount = 1;

        /// <summary>
        /// The amount of time each animation frame runs for
        /// </summary>
        public float FrameTime = 0;

        /// <summary>
        /// How often the block will grow to the next stage
        /// </summary>
        public double GrowTime;
        /// <summary>
        /// How many possible stages of growth there are
        /// </summary>
        public int GrowStages;

        /// <summary>
        /// The color displayed on the minimap, automaticly calculated at runtime
        /// </summary>
        public Color MinimapColor;

        /// <summary>
        /// True if the block can be flipped to face the other direction
        /// </summary>
        public bool Flipable = false;

        public bool NeedsWallSupport
        {
            get { return needsWallSupport; }
            set { if (this is ForegroundBlockItem) needsWallSupport = value; else throw new InvalidOperationException(); }
        }
        private bool needsWallSupport;

        /// <summary>
        /// The style in which the block is rendered, as a single sprite, as a spritesheet, etc
        /// </summary>
        public BlockRenderMode RenderMode = BlockRenderMode.SpriteSheet;
        public bool Burnable = false;
        public Point StorageSlots;
        /// <summary>
        /// The item the block drops, can be configured in detail with the event handler
        /// </summary>
        public Item Drop;

        /// <summary>
        /// Indicates if the item can fall (ie sand)
        /// </summary>
        public bool CanFall;
        /// <summary>
        /// Indicates if the item will break fallable items (ie torch breaks sand)
        /// </summary>
        public bool BreakFall;

        public float ClimbUpSpeed;
        public float ClimbDownSpeed;
        public bool AutoPlay = true;
        /// <summary>
        /// Defines if the block should break if nothing is supporting it, this is NOT for trees, but for things like plants
        /// </summary>
        public bool BreakNoSupportBottom;
        public bool BreakNoSupportTop;
        public float BreakTime = 1000;
        /// <summary>
        /// True if this tile should emit the ambient sky color (ie blank blocks, and glass blocks)
        /// </summary>
        public bool SkyLight;
        public BlockItem SmoothBlend = null;

        public bool PluginCheck;
        public int EdgeWidth = 0;
        public bool[,] BlockMap;
        public BlockItem BackgroundEquivelent;
        public BlockItem ForegroundEquivelent;
        public ConnectionPoint[] Inputs;
        public ConnectionPoint[] Outputs;

        #region Electronic State Event
        /// <summary>
        /// Event fired when the tile is dropped
        /// </summary>
        private GetElectronicStateEventHandler getElectronicState;
        public event GetElectronicStateEventHandler GetElectronicState
        {
            add { getElectronicState = value; }
            remove { getElectronicState -= value; }
        }
        public virtual int OnGetState(GetElectronicStateEventArgs e)
        {
            if (getElectronicState != null) return getElectronicState(this, e);
            else return (e.level.tiles[e.x,e.y] as ElectronicTile).State;
        }
        #endregion

        #region Electronic Output Event
        /// <summary>
        /// Event fired when the tile is dropped)
        /// </summary>
        private RequestElectronicOutputEventHandler outputRequested;
        public event RequestElectronicOutputEventHandler OutputRequested
        {
            add { outputRequested = value; }
            remove { outputRequested -= value; }
        }
        public virtual bool OnRequestOutput(RequestElectronicOutputEventArgs e)
        {
            if (outputRequested != null) return outputRequested(this, e);
            else return false;
        }
        #endregion

        #region Place Event
        /// <summary>
        /// Event fired when the tile is created (besides standard tile initialization)
        /// </summary>
        private PlaceBlockEventHandler placed;
        public event PlaceBlockEventHandler Placed
        {
            add { placed = value; }
            remove { placed -= value; }
        }
        public virtual bool OnPlace(PlaceBlockEventArgs e)
        {
            if (placed != null) return (placed(this, e));
            else return false;
        }
        #endregion

        #region Drop Event
        /// <summary>
        /// Event fired when the tile is dropped)
        /// </summary>
        private DropBlockEventHandler droped;
        public event DropBlockEventHandler Droped
        {
            add { droped = value; }
            remove { droped -= value; }
        }
        public virtual void OnDrop(DropBlockEventArgs e)
        {
            if (droped != null) droped(this, e);
        }
        #endregion

        #region Interact Event
        /// <summary>
        /// Event fired when the tile is interactped)
        /// </summary>
        private InteractBlockEventHandler interact;
        public bool TreeCapitate;
        public event InteractBlockEventHandler Interact
        {
            add { interact = value; } //Make sure it can only have 1 handler
            remove { interact -= value; }
        }
        public virtual void OnInteract(InteractBlockEventArgs e)
        {
            if (interact != null) interact(this, e);
        }
        #endregion

        public BlockItem(string name, string description = null)
            : base(name, description)
        {
            Drop = this;
            Textures = new Texture2D[Variations + 1];

            base.LeftHold += new MouseItemEventHandler(delegate(object o, MouseItemEventArgs e)
            {
                Item item = e.CurrentSlot.Item;
                //Check if the item is a block
                if (item is BlockItem)
                {
                    //If its in bounds and has enough, try to add it to the world
                    if (e.Level.InLevelBounds(new Vector2(e.X / Tile.Width, e.Y / Tile.Height)) && e.CurrentSlot.Stack > 0)
                        if (((BlockItem)item).OnPlace(new PlaceBlockEventArgs(e.Level, e.X / Tile.Width,e.Y / Tile.Height, item, e.Level.Players[0].Flip == SpriteEffects.FlipHorizontally, e.GameTime)))
                            e.CurrentSlot.Sub();
                }
            });

            Droped += new DropBlockEventHandler(delegate(object o, DropBlockEventArgs e)
            {
                HandleDrop(o, e);
            });

            Placed += new PlaceBlockEventHandler(delegate(object o, PlaceBlockEventArgs e)
            {
                if (this == Item.Water || this == Item.Lava)
                    return false;
                if (this.Station)
                {
                    if (ZarknorthClient.Interface.MainWindow.CraftingWindow != null && ZarknorthClient.Interface.MainWindow.CraftingWindow.Visible)
                    {
                        ZarknorthClient.Interface.MainWindow.CraftingWindow.UpdateItemList(e.level.MainWindow, true);
                        ZarknorthClient.Interface.MainWindow.CraftingWindow.UpdateCaption((o as BlockItem));
                    }
                    return (o as BlockItem).HandlePlace(e);
                }
                return HandlePlace(e);
            });

            Interact += new InteractBlockEventHandler(delegate(object o, InteractBlockEventArgs e)
            {
                if (Vector2.Distance(new Vector2(e.x, e.y), new Vector2(e.level.Player.Position.X / Tile.Width, e.level.Player.Position.Y / Tile.Height)) <= 6)
                {
                    //Open crafting interface if crafting station
                    if (Station)
                        ZarknorthClient.Interface.MainWindow.CraftingWindow.Show((o as BlockItem));
                    //Open door if door
                    if (Collision == BlockCollision.Door)
                        if (e.level.tiles[e.x, e.y].ForegroundVariation == 0)
                            e.level.tiles[e.x, e.y].ForegroundVariation = 1;
                        else if (e.level.tiles[e.x, e.y].ForegroundVariation == 1)
                            e.level.tiles[e.x, e.y].ForegroundVariation = 0;
                    //Handle Interaction
                    e.level.InteractBlock(e.x, e.y, e.gameTime);
                }
            });

            PrintStats += new PrintItemDataEventHandler(delegate(object o, PrintItemDataEventArgs e)
            {
                List<string> str = new List<string>();
                if (this is ForegroundBlockItem && BackgroundEquivelent == null)
                    str.Add("Foreground Tile");
                else if (this is BackgroundBlockItem && ForegroundEquivelent == null)
                    str.Add("Background Tile");
                else
                    str.Add("Foreground & Background Tile");
                str.Add("Max Stack: " + MaxStack);
                str.Add("Durability: " + BreakTime / 100);
                str.Add("Dimensions: " + Size.X + "x" + Size.Y);
                str.Add("Physics: " + Collision.ToString());
                if (Burnable)
                str.Add("- Burnable");
                if (CanFall)
                str.Add("- Can Fall");
                if (Flipable)
                str.Add("- Flipable");
                if (SubType == BlockSubType.Storage)
                str.Add("- " + StorageSlots.X + "x" + StorageSlots.Y + " Storage Slots");
                return str.ToArray();
            });
        }

        public bool HandlePlace(PlaceBlockEventArgs e)
        {
            return e.level.PlaceBlock(e.x, e.y, this, e.flip, e.gameTime);
        }

        public void HandleDrop(object o, DropBlockEventArgs e)
        {
            if (!e.SuppressDrop)
            {
                if (((BlockItem)o).SubType == BlockSubType.Storage)
                    foreach (Slot s in ((StorageTile)(e.Level.tiles[e.X, e.Y])).Slots)
                    {
                        e.Level.SpawnCollectable(e.X, e.Y, s);
                    }

                if (this is BackgroundBlockItem && ForegroundEquivelent != null)
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(ForegroundEquivelent, 1));
                else
                    e.Level.SpawnCollectable(e.X, e.Y, new Slot(Drop, 1));
            }
            if (this is BackgroundBlockItem && e.Level.tiles[e.X,e.Y].Foreground.NeedsWallSupport &&  e.Level.tiles[e.X,e.Y].ForegroundSheet == TileSetType.Single)
                e.Level.tiles[e.X,e.Y].Foreground.OnDrop(new DropBlockEventArgs(e.Level,e.X,e.Y, e.Level.tiles[e.X,e.Y].Foreground.Drop));

            if (this is ForegroundBlockItem && !NeedsWallSupport)
            {
                if (e.Level.tiles[e.X + 1, e.Y].Foreground.NeedsWallSupport)
                    e.Level.tiles[e.X + 1, e.Y].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X + 1, e.Y, e.Level.tiles[e.X + 1, e.Y].Foreground.Drop));
                if (e.Level.tiles[e.X - 1, e.Y].Foreground.NeedsWallSupport)
                    e.Level.tiles[e.X - 1, e.Y].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X - 1, e.Y, e.Level.tiles[e.X- 1, e.Y].Foreground.Drop));
                if (e.Level.tiles[e.X, e.Y + 1].Foreground.NeedsWallSupport)
                    e.Level.tiles[e.X, e.Y + 1].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X, e.Y + 1, e.Level.tiles[e.X, e.Y + 1].Foreground.Drop));
                if (e.Level.tiles[e.X, e.Y - 1].Foreground.NeedsWallSupport)
                    e.Level.tiles[e.X, e.Y - 1].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X, e.Y - 1, e.Level.tiles[e.X, e.Y - 1].Foreground.Drop));
            }
            if (this is ForegroundBlockItem && (SubType == BlockSubType.Electronic || SubType == BlockSubType.Timer))
            {
                ElectronicTile t = e.Level.tiles[e.X, e.Y] as ElectronicTile;
                if (t.Inputs != null)
                foreach (List<Wire> w in t.Inputs)
                    foreach (Wire wire in w.ToList())
                    {
                        ZarknorthClient.Game.level.Wires.Remove(wire);
                        if (wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
                            (wire.Connection1 as ElectronicTile).Inputs[wire.ConnectionID1].Remove(wire);
                        else
                            (wire.Connection1 as ElectronicTile).Outputs[wire.ConnectionID1].Remove(wire);

                        if (wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
                            (wire.Connection2 as ElectronicTile).Inputs[wire.ConnectionID2].Remove(wire);
                        else
                            (wire.Connection2 as ElectronicTile).Outputs[wire.ConnectionID2].Remove(wire);
                    }
                if (t.Outputs != null)
                foreach (List<Wire> w in t.Outputs)
                    foreach (Wire wire in w.ToList())
                    {
                        ZarknorthClient.Game.level.Wires.Remove(wire);
                        if (wire.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
                            (wire.Connection1 as ElectronicTile).Inputs[wire.ConnectionID1].Remove(wire);
                        else
                            (wire.Connection1 as ElectronicTile).Outputs[wire.ConnectionID2].Remove(wire);

                        if (wire.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
                            (wire.Connection2 as ElectronicTile).Inputs[wire.ConnectionID2].Remove(wire);
                        else
                            (wire.Connection2 as ElectronicTile).Outputs[wire.ConnectionID2].Remove(wire);
                    }
            }
            ((BlockItem)o).Delete(e);

            //If storage, drop contained items
            if (((BlockItem)o).TreeCapitate)
                BlockItem.CapitateTree(e);
            BlockItem.DelSupport(e);
            if (!e.SuppressDrop)
            DropParticles(o, e);

            if (!e.SuppressDrop && Station && ZarknorthClient.Interface.MainWindow.CraftingWindow != null)
            {
                ZarknorthClient.Interface.MainWindow.CraftingWindow.UpdateItemList(e.Level.MainWindow, true);
            }
        }

        private static void DropParticles(object o, DropBlockEventArgs e)
        {
            for (int i = 0; i < 5; i++)
                e.Level.DefaultParticleEngine.SpawnItemParticle(ParticleType.ItemFall, (e.X * Tile.Width) + (Tile.Width / 2), (e.Y * Tile.Height) + (Tile.Height / 2), ((BlockItem)o), Color.White);
        }

        public void Delete(DropBlockEventArgs e)
        {
            if (this is BackgroundBlockItem)
                e.Level.tiles[e.X, e.Y, true].Background = Item.Blank;
            else if (this is ForegroundBlockItem)
                e.Level.tiles[e.X, e.Y].Foreground = Item.Blank;
        }
        /// <summary>
        /// Will make a tree fall down, searches left,right,and top for relevant blocks to break, but not down
        /// </summary>
        /// <param name="e"></param>
        public static void CapitateTree(DropBlockEventArgs e)
        {
            Achievement.Show(Achievement.Chop);
            //FG
            if (e.Level.tiles[e.X + 1, e.Y].Foreground.TreeCapitate) //btnRight
            {
                e.Level.tiles[e.X + 1, e.Y].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X + 1, e.Y, e.Level.tiles[e.X + 1, e.Y].Foreground.Drop));
            }
            if (e.Level.tiles[e.X - 1, e.Y].Foreground.TreeCapitate) //btnLeft
            {
                e.Level.tiles[e.X - 1, e.Y].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X - 1, e.Y, e.Level.tiles[e.X - 1, e.Y].Foreground.Drop));
            }
            if (e.Level.tiles[e.X, e.Y - 1].Foreground.TreeCapitate) //Top
            {
                e.Level.tiles[e.X, e.Y - 1].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X, e.Y - 1, e.Level.tiles[e.X, e.Y - 1].Foreground.Drop));
            }
            //FB
            if (e.Level.tiles[e.X + 1, e.Y].Background.TreeCapitate) //btnRight
            {
                e.Level.tiles[e.X + 1, e.Y].Background.OnDrop(new DropBlockEventArgs(e.Level, e.X + 1, e.Y, e.Level.tiles[e.X + 1, e.Y].Background.Drop));
            }
            if (e.Level.tiles[e.X - 1, e.Y].Background.TreeCapitate) //btnLeft
            {
                e.Level.tiles[e.X - 1, e.Y].Background.OnDrop(new DropBlockEventArgs(e.Level, e.X - 1, e.Y, e.Level.tiles[e.X - 1, e.Y].Background.Drop));
            }
            if (e.Level.tiles[e.X, e.Y - 1].Background.TreeCapitate) //Top
            {
                e.Level.tiles[e.X, e.Y - 1].Background.OnDrop(new DropBlockEventArgs(e.Level, e.X, e.Y - 1, e.Level.tiles[e.X, e.Y - 1].Background.Drop));
            }
        }

        /// <summary>
        /// Will make items with no support fall, ie grass plant on grass
        /// </summary>
        public static void DelSupport(DropBlockEventArgs e)
        {
            //FG
            if (e.Level.tiles[e.X, e.Y].Foreground == Item.Blank) //Bottom
            {
                if (e.Item is ForegroundBlockItem)
                for (int i = 0; i < (e.Item as ForegroundBlockItem).Size.X; i++ )
                    if (e.Level.tiles[e.X + i, e.Y - 1].Foreground.BreakNoSupportBottom)
                    e.Level.tiles[e.X + i, e.Y - 1].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X + i, e.Y - 1, e.Level.tiles[e.X + i, e.Y - 1].Foreground.Drop));
            }
            //BG
            if (e.Level.tiles[e.X, e.Y].Background == Item.Blank && e.Level.tiles[e.X, e.Y - 1].Background.BreakNoSupportBottom) //Bottom
            {
                e.Level.tiles[e.X, e.Y - 1].Background.OnDrop(new DropBlockEventArgs(e.Level, e.X, e.Y - 1, e.Level.tiles[e.X, e.Y - 1].Background.Drop));
            }
            //FG
            if (e.Level.tiles[e.X, e.Y].Foreground == Item.Blank) //Bottom
            {
                for (int i = 0; i < e.Level.tiles[e.X, e.Y].Foreground.Size.X; i++)
                    if (e.Level.tiles[e.X + i, e.Y + 1].Foreground.BreakNoSupportTop)
                        e.Level.tiles[e.X + i, e.Y + 1].Foreground.OnDrop(new DropBlockEventArgs(e.Level, e.X + i, e.Y + 1, e.Level.tiles[e.X + i, e.Y - 1].Foreground.Drop));
            }
            //BG
            if (e.Level.tiles[e.X, e.Y].Background == Item.Blank && e.Level.tiles[e.X, e.Y + 1].Background.BreakNoSupportTop) //Top
            {
                e.Level.tiles[e.X, e.Y + 1].Background.OnDrop(new DropBlockEventArgs(e.Level, e.X, e.Y + 1, e.Level.tiles[e.X, e.Y + 1].Background.Drop));
            }
        }
    }
   
    public class ForegroundBlockItem : BlockItem
    {
        public ForegroundBlockItem(string name, string description = null) : base(name, description)
        {
            Absorb = 50;
        }
    }

    public class BackgroundBlockItem : BlockItem
    {
        public bool Undefined;
        public BackgroundBlockItem(string name, string description = null)
            : base(name, description)
        {
            Absorb = 25;
            if (string.IsNullOrEmpty(description) && MaxStack == 0)
                Undefined = true;
        }
    }
    [Flags]
    public enum BlockRenderMode
    {
        SpriteSheet = 0,
        Single = 1,
        Variation = 2,
        Animation = 4,
        Grown = 8,
        Custom = 16,
    }
    public enum BlockSubType
    {
        Default,
        Animated,
        Plant,
        Text,
        Storage,
        Portal,
        Music,
        Furnace,
        Electronic,
        Timer,
    }
    public enum BlockEdgeMode
    {
        Edge,
        Stick,
    }
    public enum BlockPlaceMode
    {
        Edge,
        Bottom,
        Hanging,
        AllButTop,
    }
  /// <summary>
  /// Old techtree stuff
  /// </summary>
  public class Childeren
  {

      public int[] childeren;
 
      public Childeren(params int[] childeren)
      {
         this.childeren = childeren;
      }
  }

    public enum BlockCollision : byte
    {
        Passable = 0,
        Impassable = 1,
        Platform = 2,
        Ladder = 3,
        Portal = 4,
        Bouncy = 5,
        Door = 6,
        Falling = 7,
        Spike = 8,
        Stair = 9,
        Liquid = 10,
    }
