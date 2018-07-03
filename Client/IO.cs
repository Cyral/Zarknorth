#region Usings
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZarknorthClient.Entities;
using Color = Microsoft.Xna.Framework.Color;
using Cyral.Extensions.Xna;
using System.Text;
#endregion

namespace ZarknorthClient
{
    /// <summary>
    /// Class used for disk operations such as loading, saving, and gathering settings
    /// </summary>
    public static class IO
    {
        public static string AssemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static Dictionary<string, string> Directories = new Dictionary<string, string>();
        public static string MainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.Zarknorth";
        public const string WorldSuffix = ".wld";
        public const string UniverseSuffix = ".dat";
        public const string PlayerDataSuffix = ".dat";
        public const string PlayerSkinSuffix = ".profile";
        public static string UniverseFile;
        private static int FileBufferSize = 65536;

        public static List<ContentPack> ContentPacks = new List<ContentPack>();

        /// <summary>
        /// Flags used for saving/loading to save space in bytes
        /// </summary>
        [Flags]
        public enum SaveFlags : byte
        {
            None = 0, //Tile contains no flags
            Background = 1, //Tile contains background
            RLE = 2, //Run length encoding
            Water = 04, //Tile contains water
            Lava = 08, //Tile contains lava
            Fire = 16,
            FrameImportant = 32,
            Paint = 64,
            Flip = 128,
            AnimationFrameImportant = 255,
        }

        static IO()
        {
            //Add the sub-directories to the directory list
            Directories.Add("Planets", MainDirectory + "\\Planets\\");
            Directories.Add("Maps", MainDirectory + "\\Maps\\");
            Directories.Add("Screenshots", MainDirectory + "\\Screenshots\\");
            Directories.Add("Content Packs", MainDirectory + "\\Content Packs\\");
            Directories.Add("Players", MainDirectory + "\\Players\\");

            UniverseFile = MainDirectory + "\\Universe" + UniverseSuffix;
        }

        /// <summary>
        /// Checks to make sure the application files are there, if not it will create them
        /// </summary>
        public static void CheckFiles()
        {
            //Check if the main directory exists. If its dosent, Create the main directory
            if (!Directory.Exists(MainDirectory))
                Directory.CreateDirectory(MainDirectory);
            //Now check for each sub-folder. If they dont exist, then add them
            foreach (KeyValuePair<string, string> kv in Directories)
                if (!Directory.Exists(kv.Value))
                    Directory.CreateDirectory(kv.Value);
        }


        /// <summary>
        /// Opens the settings file and loads the values
        /// </summary>
        public static void LoadSettings(Game game)
        {
            Game.Controls = new Dictionary<string, Microsoft.Xna.Framework.Input.Keys>();
            Game.Controls.Add("Left", Keys.None);
            Game.Controls.Add("Right", Keys.None);
            Game.Controls.Add("Up", Keys.None);
            Game.Controls.Add("Down", Keys.None);
            Game.Controls.Add("Toggle Inventory", Keys.None);
            Game.Controls.Add("Toggle Chat", Keys.None);
            Game.Controls.Add("Open Achievements", Keys.None);
            Game.Controls.Add("Open Crafting", Keys.None);
            Game.Controls.Add("Take Screenshot", Keys.None);
            Game.Controls.Add("Place on Background", Keys.None);
            Game.Controls.Add("Zoom In", Keys.None);
            Game.Controls.Add("Zoom Out", Keys.None);
            Game.Controls.Add("Toggle Fullscreen", Keys.None);
            Game.Controls.Add("Show GUI", Keys.None);
            Game.Controls.Add("Show Player", Keys.None);
            Game.Controls.Add("Debug Mode", Keys.None);

            try
            {
                for (int i = 0; i < Game.Controls.Count(); i++)
                {
                    string s = ConfigurationManager.AppSettings[Game.Controls.ElementAt(i).Key];
                    Game.Controls[Game.Controls.ElementAt(i).Key] = (Keys)Enum.Parse(typeof(Keys), ConfigurationManager.AppSettings[Game.Controls.ElementAt(i).Key], true);
                }
                //Load settings
                Game.Fullscreen = Boolean.Parse(ConfigurationManager.AppSettings["Fullscreen"]);
                Game.Antialiasing = Boolean.Parse(ConfigurationManager.AppSettings["Antialiasing"]);
                Game.TileEdges = Boolean.Parse(ConfigurationManager.AppSettings["TileEdges"]);
                Game.Autosave = Boolean.Parse(ConfigurationManager.AppSettings["Autosave"]);
                Game.DropShadows = Boolean.Parse(ConfigurationManager.AppSettings["DropShadows"]);
                Game.LightingQuality = Int32.Parse(ConfigurationManager.AppSettings["LightingQuality"]);
                Game.LightingRefresh = Int32.Parse(ConfigurationManager.AppSettings["LightingRefresh"]);
                Game.ParticleQuality = Int32.Parse(ConfigurationManager.AppSettings["ParticleQuality"]);
                Game.ContentPackName = ConfigurationManager.AppSettings["ContentPack"];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert(false, ex.Message);
            }
            //Apply settings
            Game.Skin = Int32.Parse(ConfigurationManager.AppSettings["Skin"]);
            if (Game.Skin == 0 && game.Manager.Skin.Name != "Red")
                game.Manager.SetSkin("Red");
            else if (Game.Skin == 1 && game.Manager.Skin.Name != "Blue")
                game.Manager.SetSkin("Blue");
            else if (Game.Skin == 2 && game.Manager.Skin.Name != "Green")
                game.Manager.SetSkin("Green");
        }

        public static void LoadContentPacks(Game game)
        {
            //Set current pack
            Game.ContentPackName = ConfigurationManager.AppSettings["ContentPack"];

            //Load Content Packs
            List<string> dirs = new List<string>();
            //Load embedded pack directories from Content folder
            dirs.Add(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Content\\Content Packs\\Default");
            //Load packs from Zarknorth folder
            dirs.AddRange(Directory.GetDirectories(Directories["Content Packs"]).ToList<string>());

            //Foreach directory
            for (int x = 0; x < dirs.Count(); x++)
            {
                string dir = dirs[x];

                //Load the packs xml data
                XmlDocument doc = new XmlDocument();
                doc.Load(dir + "\\pack.xml");

                //Create the new pack and set it's data
                ContentPack pack = new ContentPack();
                pack.Name = doc.ChildNodes[1].ChildNodes[0].ChildNodes[0].Value;
                pack.Description = doc.ChildNodes[1].ChildNodes[1].ChildNodes[0].Value;
                pack.Version = doc.ChildNodes[1].ChildNodes[2].ChildNodes[0].Value;
                pack.Author = doc.ChildNodes[1].ChildNodes[3].ChildNodes[0].Value;
                pack.Path = dir;
                pack.Embedded = pack.Name == "Default";
                
                //If an embedded pack, load from ContentManager, else FromStream
                if (pack.Embedded)
                    pack.Icon = Game.TextureLoader.Content.Load<Texture2D>("Content\\Content Packs\\" + pack.Name + "\\icon");
                else
                    using (FileStream fileStream = new FileStream(dir + "\\icon.png", FileMode.Open))
                        pack.Icon = Texture2D.FromStream(game.GraphicsDevice, fileStream);

                //Add the pack
                ContentPacks.Add(pack);

                //If this pack is the current pack, set the game's data to it, so it is aware of the pack to use
                if (pack.Name == Game.ContentPackName)
                {
                    Game.ContentPackIndex = x;
                    Game.ContentPackData = pack;
                }
            }
            //Load the current pack
            ContentPack.LoadPack(Game.ContentPackData);
        }
        public static List<LevelData> ListMaps()
        {
            //List files by date
            string[] Files = new DirectoryInfo(Directories["Maps"]).GetFiles("*" + WorldSuffix)
                                                  .OrderByDescending(f => f.LastWriteTime).Select(x => x.Name).ToArray();
            List<LevelData> List = new List<LevelData>();
            for (int i = 0; i < Files.Count(); i++)
            {
                string File = Files[i];
                LevelData Data = new LevelData();
                Data.Name = Path.GetFileNameWithoutExtension(File);
                Data.Description = "Test";
                using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
    new BufferedStream(
    new GZipStream(
        System.IO.File.Open(Path.Combine(Directories["Maps"], Data.Name) + WorldSuffix, FileMode.Open), //Open the file
    CompressionMode.Decompress))))
                {
                    //Read game details
                    Data.Author = binaryReader.ReadString(); //Author
                    Data.Description = binaryReader.ReadString(); //Description
                }
                List.Add(Data);
            }
            return List;
        }
        public static string[] GetLevelData(string Name)
        {
                List<string> list = new List<string>();
                using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                    new BufferedStream(
                    new GZipStream(
                        File.Open(Path.Combine(Directories["Maps"], Name) + WorldSuffix, FileMode.Open), //Open the file
                    CompressionMode.Decompress))))
                {
                    //Read game details
                    list.Add(binaryReader.ReadString()); //Author
                    list.Add(binaryReader.ReadString()); //Description
                    list.Add(binaryReader.ReadString()); //Version
                    DateTime date = DateTime.Parse(binaryReader.ReadString());
                    list.Add(Cyral.Extensions.DateTimeExtensions.GetPrettyDate(date) + " (" + date.ToString("G") + ")"); //Date

                    list.Add(binaryReader.ReadInt32().ToString()); //Seed

                    list.Add(binaryReader.ReadInt32().ToString()); //Width
                    list.Add(binaryReader.ReadInt32().ToString()); //Height
                }
                return list.ToArray();
        }
        public static void DeleteMap(string name)
        {
            File.Delete(Path.Combine(Directories["Maps"], name) + WorldSuffix);
            System.Diagnostics.Debug.WriteLine("Info: Deleted " + name);
        }
        public static void RenameMap(string name, string newname)
        {
            File.Move(Path.Combine(Directories["Maps"], name) + WorldSuffix, Path.Combine(Directories["Maps"], newname) + WorldSuffix);
            System.Diagnostics.Debug.WriteLine("Info: Renamed " + name + " to " + newname);
        }
        /// <summary>
        /// Saves a player skin/profile
        /// </summary>
        public static void SavePlayerSkin(ZarknorthClient.Interface.TaskProfile profile)
        {
            try
            {
                using (BinaryWriter binaryWriter = new BinaryWriter( //Create a new binary writer to write to the file
                    new BufferedStream(
                    new GZipStream(
                        File.Open(Path.Combine(Directories["Players"], Game.UserName) + PlayerSkinSuffix, FileMode.Create), //Open the file
                    CompressionMode.Compress)))) //Get data in 64kb chunks for faster proccessing
                {
                    //Write username
                    binaryWriter.Write(Game.UserName);
                    //Write variations
                    binaryWriter.Write((byte)profile.ExpressionVariation);
                    binaryWriter.Write((byte)profile.HairVariation);
                    binaryWriter.Write((byte)profile.PantsLegVariation);
                    binaryWriter.Write((byte)profile.PantsTopVariation);
                    binaryWriter.Write((byte)profile.ShirtBodyVariation);
                    binaryWriter.Write((byte)profile.ShirtSleeveVariation);
                    binaryWriter.Write((byte)profile.ShoeVariation);
                    //Write colors
                    binaryWriter.Write(profile.SkinColor.SelectedColor);
                    binaryWriter.Write(profile.ShirtColor.SelectedColor);
                    binaryWriter.Write(profile.PantsColor.SelectedColor);
                    binaryWriter.Write(profile.HairColor.SelectedColor);
                    binaryWriter.Write(profile.ShoeColor.SelectedColor);
                }
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Error saving player profile.");
            }
        }
        /// <summary>
        /// Saves a player skin/profile
        /// </summary>
        public static bool LoadPlayerSkin(PlayerCharacter player)
        {
            try
            {
                if (File.Exists(Path.Combine(Directories["Players"], Game.UserName) + PlayerSkinSuffix))
                {
                    using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                        new BufferedStream(
                        new GZipStream(
                            File.Open(Path.Combine(Directories["Players"], Game.UserName) + PlayerSkinSuffix, FileMode.Open), //Open the file
                        CompressionMode.Decompress)))) //Get data in 64kb chunks for faster proccessing
                    {
                        //Sanity check to disallow loading of others profiles
                        if (Game.UserName != binaryReader.ReadString())
                        {
                            System.Windows.Forms.MessageBox.Show("Invalid player profile.");
                            return false;
                        }
                        //Read variations
                        player.ExpressionVariation = (int)binaryReader.ReadByte();
                        player.HairVariation = (int)binaryReader.ReadByte();
                        player.PantsLegVariation = (int)binaryReader.ReadByte();
                        player.PantsTopVariation = (int)binaryReader.ReadByte();
                        player.ShirtBodyVariation = (int)binaryReader.ReadByte();
                        player.ShirtSleeveVariation = (int)binaryReader.ReadByte();
                        player.ShoeVariation = (int)binaryReader.ReadByte();

                        player.SkinColor = binaryReader.ReadColor();
                        player.ShirtColor = binaryReader.ReadColor();
                        player.PantsColor = binaryReader.ReadColor();
                        player.HairColor = binaryReader.ReadColor();
                        player.ShoeColor = binaryReader.ReadColor();
                        return true;
                    }
                }
                else
                    return false;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Malformed or corrupted player profile.");
                return false;
            }
        }
        /// <summary>
        /// Saves a player skin/profile
        /// </summary>
        public static bool LoadPlayerSkin(ZarknorthClient.Interface.TaskProfile profile)
        {
            try
            {
                if (File.Exists(Path.Combine(Directories["Players"], Game.UserName) + PlayerSkinSuffix))
                {
                    using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                        new BufferedStream(
                        new GZipStream(
                            File.Open(Path.Combine(Directories["Players"], Game.UserName) + PlayerSkinSuffix, FileMode.Open), //Open the file
                        CompressionMode.Decompress)))) //Get data in 64kb chunks for faster proccessing
                    {
                        //Sanity check to disallow loading of others profiles
                        if (Game.UserName != binaryReader.ReadString())
                        {
                            System.Windows.Forms.MessageBox.Show("Invalid player profile.");
                            return false;
                        }
                        //Read variations
                        profile.ExpressionVariation = (int)binaryReader.ReadByte();
                        profile.HairVariation = (int)binaryReader.ReadByte();
                        profile.PantsLegVariation = (int)binaryReader.ReadByte();
                        profile.PantsTopVariation = (int)binaryReader.ReadByte();
                        profile.ShirtBodyVariation = (int)binaryReader.ReadByte();
                        profile.ShirtSleeveVariation = (int)binaryReader.ReadByte();
                        profile.ShoeVariation = (int)binaryReader.ReadByte();

                        profile.SkinColor.SelectedColor = binaryReader.ReadColor();
                        profile.ShirtColor.SelectedColor = binaryReader.ReadColor();
                        profile.PantsColor.SelectedColor = binaryReader.ReadColor();
                        profile.HairColor.SelectedColor = binaryReader.ReadColor();
                        profile.ShoeColor.SelectedColor = binaryReader.ReadColor();
                        return true;
                    }
                }
                else
                    return false;
            }
            catch
            {
                System.Windows.Forms.MessageBox.Show("Malformed or corrupted player profile.");
                return false;
            }
        }
        /// <summary>
        /// Saves a level
        /// </summary>
        /// <param name="Level">Level to be saved</param>
        public static void SavePlayer(PlayerCharacter Player)
        {
            IO.CheckFiles();
            using (BinaryWriter binaryWriter = new BinaryWriter( //Create a new binary writer to write to the file
                new BufferedStream(
                new GZipStream(
                    File.Open(Path.Combine(Directories["Players"], Player.Name) + PlayerDataSuffix, FileMode.Create), //Open the file
                CompressionMode.Compress)))) //Get data in 64kb chunks for faster proccessing
            {
                binaryWriter.Write(Player.Name);
                binaryWriter.Write(Player.Inventory.Length);
                foreach (Slot s in (Player.Inventory))
                {
                    binaryWriter.Write((short)s.Stack);
                    binaryWriter.Write((byte)s.Item.ID);
                }
                binaryWriter.Write(Achievement.AchievementList.Count);
                foreach (Achievement a in Achievement.AchievementList)
                {
                    binaryWriter.Write((byte)a.ID);
                    binaryWriter.Write(a.Achieved);
                }
            }
        }
        public static PlayerCharacter LoadPlayer(PlayerCharacter Player)
        {
            if (File.Exists(Path.Combine(Directories["Players"], Player.Name) + PlayerDataSuffix))
            {
                using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                    new BufferedStream(
                    new GZipStream(
                        File.Open(Path.Combine(Directories["Players"], Player.Name) + PlayerDataSuffix, FileMode.Open), //Open the file
                    CompressionMode.Decompress)))) //Get data in 64kb chunks for faster proccessing
                {
                    Player.Name = binaryReader.ReadString();
                    int slots = binaryReader.ReadInt32();
                    Player.Inventory = new Slot[slots];
                    for (int s = 0; s < slots; s++)
                    {
                        int stack = binaryReader.ReadInt16();
                        int id = binaryReader.ReadByte();
                        Item item = Item.ItemList.Find(it => it.ID == id);
                        Player.Inventory[s] = new Slot(item, stack);
                    }
                    int achievements = binaryReader.ReadInt32();
                    for (int a = 0; a < achievements; a++)
                    {
                        int id = binaryReader.ReadByte();
                        Achievement ac = Achievement.AchievementList.Find(x => x.ID == id);
                        ac.Achieved = binaryReader.ReadBoolean();
                    }
                }
            }
            return Player;
        }
        /// <summary>
        /// Saves a level
        /// </summary>
        /// <param name="Level">Level to be saved</param>
        public static void SaveLevel(Level Level, bool DisposeLevel = true)
        {
            IO.CheckFiles();
            using (BinaryWriter binaryWriter = new BinaryWriter( //Create a new binary writer to write to the file
                new BufferedStream(
                new GZipStream(
                    File.Open(Path.Combine(Directories[Level.IsMap ? "Maps" : "Planets"], Level.Data.Name) + WorldSuffix, FileMode.Create), //Open the file
                CompressionMode.Compress), //Create a Gzip compressor
                FileBufferSize))) //Get data in 64kb chunks for faster proccessing
            {
                //Write game details
                binaryWriter.Write(Level.Data.Author);
                binaryWriter.Write(Level.Data.Description ?? " "); //Write description
                binaryWriter.Write(AssemblyVersion); //Write version saved with 
                binaryWriter.Write(DateTime.Now.ToString());//Write date saved
                binaryWriter.Write(Level.Data.Seed);
                binaryWriter.Write((Int32)Level.Width); //Write level width and height
                binaryWriter.Write((Int32)Level.Height);

                binaryWriter.Write((double)Level.SpawnPoint.X);
                binaryWriter.Write((double)Level.SpawnPoint.Y);
                binaryWriter.Write((double)Level.BedSpawnPoint.X);
                binaryWriter.Write((double)Level.BedSpawnPoint.Y);
                binaryWriter.Write((double)Level.Players[0].Position.X); //Write players position
                binaryWriter.Write((double)Level.Players[0].Position.Y);
                binaryWriter.Write(Level.elapsedTime);
                binaryWriter.Write(Level.isDay);

                binaryWriter.Write(Level.worldGen.Biomes.Length); //Write how many biomes
                for (int i = 0; i < Level.worldGen.Biomes.Length; i++) //Write the biome data
                {
                    binaryWriter.Write(Level.worldGen.Biomes[i].Start); //Start, End and ID
                    binaryWriter.Write(Level.worldGen.Biomes[i].End);
                    binaryWriter.Write((int)Level.worldGen.Biomes[i].Type.ID);
                }
                for (int x = 0; x < Level.Width; x++)
                {
                    binaryWriter.Write((short)Level.worldGen.HeightMap[x]);
                }
                //Loop through each block
                for (int y = 0; y < Level.Height; y++)
                {
                    if (Level.AutoSaving)
                        ZarknorthClient.Interface.MainWindow.UpdateLoading("Auto Saving " + Math.Round(((float)y / (Level.Height - 2)) * 100) + "%", (((float)y / (Level.Height - 2)) * 1f) + 0f);
                    else
                        ZarknorthClient.Interface.MainWindow.UpdateLoading("Saving " + Math.Round(((float)y / (Level.Height - 2)) * 100) + "%", (((float)y / (Level.Height - 2)) * 1f) + 0f);
                    for (int x = 0; x < Level.Width; x++)
                    {
                        //Get and write tile ID
                        Tile tile = (Tile)Level.tiles[x, y, true].Clone();
                        binaryWriter.Write((byte)tile.Foreground.ID);
                        if (tile.Foreground.SubType == BlockSubType.Timer)
                        {
                            binaryWriter.Write((tile as TimerTile).Time);
                            binaryWriter.Write((tile as TimerTile).Length);
                        }
                        if (tile.Foreground.SubType == BlockSubType.Text)
                            binaryWriter.Write((tile as TextTile).Text);
                        if (tile.Foreground.SubType == BlockSubType.Storage)
                        {
                            binaryWriter.Write((byte)(tile as StorageTile).Slots.Length);
                            foreach (Slot s in (tile as StorageTile).Slots)
                            {
                                binaryWriter.Write(s.Stack);
                                binaryWriter.Write((byte)s.Item.ID);
                            }
                        }

                        //Calculate Run-Length Encoding
                        int i = 0;
                        while (i + 1 + x < Level.Width && tile.IsTheSame(Level.tiles[x + i + 1, y]) && Level.tiles[x, y].WaterMass == Level.tiles[x + i + 1, y].WaterMass && Level.tiles[x + i + 1, y].Foreground.SubType == BlockSubType.Default && Level.tiles[x, y].Foreground.SubType == BlockSubType.Default && Level.tiles[x, y].Foreground.Size == Item.One)
                            i++; //If next block is the same, record the amount of same blocks in i

                        //Calculate Flags
                        SaveFlags sf = SaveFlags.None;
                        if (i > 0)
                            sf |= SaveFlags.RLE;
                        if (Level.tiles[x, y].WaterMass > 0)
                            sf |= SaveFlags.Water;
                        if (Level.tiles[x, y].LavaMass > 0)
                            sf |= SaveFlags.Lava;
                        if (tile.Background != Item.Blank)
                            sf |= SaveFlags.Background;
                        if (tile.ForegroundVariation > 0 && tile.Foreground.FrameImportant)
                            sf |= SaveFlags.FrameImportant;
                        if ((tile is AnimatedTile) && (tile as AnimatedTile).FrameIndex > 0 && tile.Foreground.AnimationFrameImportant)
                            sf |= SaveFlags.AnimationFrameImportant;
                        if (tile.ForegroundFireMeta > 0 || tile.BackgroundFireMeta > 0)
                            sf |= SaveFlags.Fire;
                        if (tile.ForegroundPaintColor > 0 || tile.BackgroundPaintColor > 0)
                            sf |= SaveFlags.Paint;
                        if (tile.Flip)
                            sf |= SaveFlags.Flip;

                        //Write flag byte
                        binaryWriter.Write((byte)sf);

                        //Write details flags describe
                        if (sf.HasFlag(SaveFlags.Background)) //Background
                            binaryWriter.Write((byte)tile.Background.ID);
                        if (sf.HasFlag(SaveFlags.Water))
                            binaryWriter.Write((byte)tile.WaterMass);
                        if (sf.HasFlag(SaveFlags.Lava))
                            binaryWriter.Write((byte)tile.LavaMass);
                        if (sf.HasFlag(SaveFlags.FrameImportant)) //Varaition should be saved
                            binaryWriter.Write((byte)tile.ForegroundVariation);
                        if (sf.HasFlag(SaveFlags.AnimationFrameImportant)) //Varaition should be saved
                            binaryWriter.Write((byte)(tile as AnimatedTile).FrameIndex);
                        if (sf.HasFlag(SaveFlags.Fire)) //Fire
                        {
                            binaryWriter.Write((byte)tile.ForegroundFireMeta);
                            binaryWriter.Write((byte)tile.BackgroundFireMeta);
                        }
                        if (sf.HasFlag(SaveFlags.Paint)) //Paint
                        {
                            binaryWriter.Write((byte)tile.ForegroundPaintColor);
                            binaryWriter.Write((byte)tile.BackgroundPaintColor);
                        }
                        if (sf.HasFlag(SaveFlags.RLE)) //RLE
                            binaryWriter.Write((short)i);
                        //Add RLE distance to x
                        x += i;
                    }
                }
                binaryWriter.Write(Level.Wires.Count);
                foreach(Wire w in Level.Wires)
                {
                    binaryWriter.Write((short)w.Connection1.X);
                    binaryWriter.Write((short)w.Connection1.Y);
                    binaryWriter.Write((short)w.Connection2.X);
                    binaryWriter.Write((short)w.Connection2.Y);
                    binaryWriter.Write((byte)w.ConnectionID1);
                    binaryWriter.Write((byte)w.ConnectionID2);
                    binaryWriter.Write(w.ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input);
                    binaryWriter.Write(w.ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input);
                }
            }
            if (DisposeLevel)
            {
                Game.level.Dispose();
                Game.level = null;
            }
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced); 
        }

        /// <summary>
        /// Loads a level
        /// </summary>
        /// <returns>The loaded level</returns>
        public static TileWrapper LoadLevel(Level Level, bool FromMap)
        {
            IO.CheckFiles();
            using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                new BufferedStream(
                new GZipStream(
                    File.Open(Path.Combine(Directories[FromMap ? "Maps" : "Planets"], Level.Data.Name) + WorldSuffix, FileMode.Open), //Open the file
                CompressionMode.Decompress), //Create a Gzip compressor
                FileBufferSize))) //Get data in 64kb chunks for faster proccessing
            {
                //Read game details
                Level.Data.Author = binaryReader.ReadString();
                Level.Data.Description = binaryReader.ReadString();
                Level.Data.Version = binaryReader.ReadString();
                Level.Data.DateSaved = Cyral.Extensions.DateTimeExtensions.GetPrettyDate(DateTime.Parse(binaryReader.ReadString()));
                Level.Data.Seed = binaryReader.ReadInt32();
                int LevelWidth = binaryReader.ReadInt32();
                int LevelHeight = binaryReader.ReadInt32();
                Level.Height = LevelHeight;
                Level.Width = LevelWidth;

                Level.SpawnPoint = new Vector2((float)binaryReader.ReadDouble(), (float)binaryReader.ReadDouble());
                Level.BedSpawnPoint = new Vector2((float)binaryReader.ReadDouble(), (float)binaryReader.ReadDouble());
                //Load player position
                Level.Player.Position = new Vector2((float)binaryReader.ReadDouble(), (float)binaryReader.ReadDouble());
                Level.elapsedTime = binaryReader.ReadSingle();
                Level.isDay = binaryReader.ReadBoolean();
                //Read how many biomes
                Biome[] Biomes = new Biome[binaryReader.ReadInt32()];
                //Read the biomes
                for (int i = 0; i < Biomes.Length; i++)
                {
                    int start = binaryReader.ReadInt32();
                    int end = binaryReader.ReadInt32();
                    int id = binaryReader.ReadInt32();
                    BiomeType t = BiomeType.Forest;
                    foreach (BiomeType b in BiomeType.BiomeTypes) //Find the ID
                        if (b.ID == id)
                            t = b;
                    Biomes[i] = new Biome(t, start, end); //Create the new biome
                }
                Level.worldGen.Biomes = Biomes;

                //Create new liquid and tile arrays
                TileWrapper tiles = new TileWrapper(LevelWidth, LevelHeight);
                Tile.TileWrapper = tiles;
                tiles.level = Level;
                Level.worldGen.HeightMap = new int[tiles.Width];
                for (int x = 0; x < tiles.Width; x++)
                {
                    Level.worldGen.HeightMap[x] = binaryReader.ReadInt16();
                }
                //Parse tiles
                for (int y = 0; y < tiles.Height; y++)
                {
                    ZarknorthClient.Interface.MainWindow.UpdateLoading("Loading " + Math.Round(((float)y / (Level.Height - 2)) * 100) + "%", (((float)y / (Level.Height - 2)) * 1f) + 0f);
                    for (int x = 0; x < tiles.Width; x++)
                    {
                        if (Tile.TileWrapper[x, y, true] == null)
                        Tile.TileWrapper[x, y, true] = new Tile(Item.Blank);
                        //Get tile id
                        int id = binaryReader.ReadByte();
                        //convert tile id
                        Item it = Item.ItemList.Find(item => item.ID == id);
                        if (tiles[x, y] == null || tiles[x, y].Foreground == Item.Blank)
                        Level.ForcePlaceBlock(x, y, (BlockItem)it, false);
                        if (tiles[x, y, true].Foreground.SubType == BlockSubType.Timer)
                        {
                            (tiles[x, y, true] as TimerTile).Time = binaryReader.ReadSingle();
                            (tiles[x, y, true] as TimerTile).Length = binaryReader.ReadSingle();
                        }
                        if (tiles[x, y, true].Foreground.SubType == BlockSubType.Text)
                            (tiles[x, y, true] as TextTile).Text = binaryReader.ReadString();
                        if (tiles[x, y, true].Foreground.SubType == BlockSubType.Storage)
                        {
                            (tiles[x, y, true] as StorageTile).Slots = new Slot[binaryReader.ReadByte()];
                            for (int i = 0; i < (tiles[x, y, true] as StorageTile).Slots.Length; i++)
                            {
                                int stack = binaryReader.ReadInt32();
                                int slotID = binaryReader.ReadByte();
                                Item slotItem = Item.ItemList.Find(item => item.ID == slotID);
                                (tiles[x, y, true] as StorageTile).Slots[i] = new Slot(slotItem, stack);
                            }
                        }

                        //Get flags
                        byte sf1 = binaryReader.ReadByte();

                        SaveFlags sf = (SaveFlags)sf1; //flag details
                        Item b = Item.Blank;

                        //Write details flags describe
                        if (sf.HasFlag(SaveFlags.Background)) //Background
                        {
                            id = binaryReader.ReadByte();
                            tiles[x, y, true].Background = (BlockItem)Item.ItemList.Find(item => item.ID == id);
                        }
                        if (sf.HasFlag(SaveFlags.Water))
                        {
                            tiles[x, y, true].WaterMass = (int)binaryReader.ReadByte();
                        }
                        if (sf.HasFlag(SaveFlags.Lava))
                        {
                            tiles[x, y, true].LavaMass = (int)binaryReader.ReadByte();
                        }
                        if (sf.HasFlag(SaveFlags.FrameImportant)) //Varaition should be saved
                            tiles[x, y, true].ForegroundVariation = binaryReader.ReadByte();
                        if (sf.HasFlag(SaveFlags.AnimationFrameImportant)) //Varaition should be saved
                            (tiles[x, y, true] as AnimatedTile).FrameIndex = binaryReader.ReadByte();
                        if (sf.HasFlag(SaveFlags.Fire)) //Fire
                        {
                            tiles[x, y, true].ForegroundFireMeta = binaryReader.ReadByte();
                            tiles[x, y, true].BackgroundFireMeta = binaryReader.ReadByte();
                        }
                        if (sf.HasFlag(SaveFlags.Paint)) //Paint
                        {
                            tiles[x, y, true].ForegroundPaintColor = binaryReader.ReadByte();
                            tiles[x, y, true].BackgroundPaintColor = binaryReader.ReadByte();
                        }
                        if (sf.HasFlag(SaveFlags.Flip))
                        {
                            tiles[x, y, true].Flip = true;
                        }
                        if (sf.HasFlag(SaveFlags.RLE))
                        {
                            //How many tiles in a row are the same, also clamp it so it cant go out of range
                            int same = binaryReader.ReadInt16();
                            for (int i = 1; i <= same; i++)
                            {
                                Tile cur = (Tile)tiles[x, y, true].Clone();
                                tiles[x + i, y, true] = new Tile(cur.Foreground);
                                tiles[x + i, y, true].Background = cur.Background;
                                tiles[x + i, y, true].Flip = cur.Flip;
                                if (cur.Foreground.FrameImportant)
                                    tiles[x + i, y, true].ForegroundVariation = cur.ForegroundVariation;
                                tiles[x + i, y, true].WaterMass = cur.WaterMass;
                                tiles[x + i, y, true].LavaMass = cur.LavaMass;
                                tiles[x + i, y, true].ForegroundPaintColor = cur.ForegroundPaintColor;
                                tiles[x + i, y, true].BackgroundPaintColor = cur.BackgroundPaintColor;
                                tiles[x + i, y, true].ForegroundFireMeta = cur.ForegroundFireMeta;
                                tiles[x + i, y, true].BackgroundFireMeta = cur.BackgroundFireMeta;
                                 
                            }
                            x += same;
                        }
                    }
                }
                Level.Wires = new List<Wire>();
                int wires = binaryReader.ReadInt32();
                for (int i = 0; i < wires; i++)
                {
                    Wire wire = new Wire();
                    wire.X1 = binaryReader.ReadInt16();
                    wire.Y1 = binaryReader.ReadInt16();
                    wire.X2 = binaryReader.ReadInt16();
                    wire.Y2 = binaryReader.ReadInt16();
                    wire.Connection1 = tiles[wire.X1, wire.Y1];
                    wire.Connection2 = tiles[wire.X2, wire.Y2];
                    wire.ConnectionID1 = binaryReader.ReadByte();
                    wire.ConnectionID2 = binaryReader.ReadByte();
                    wire.Input1 = binaryReader.ReadBoolean();
                    wire.Input2 = binaryReader.ReadBoolean();
                    bool input = wire.Input1;
                    if (input)
                        wire.ConnectionPoint1 = (wire.Connection1 as ElectronicTile).Foreground.Inputs[wire.ConnectionID1];
                    else
                        wire.ConnectionPoint1 = (wire.Connection1 as ElectronicTile).Foreground.Outputs[wire.ConnectionID1];
                    input = wire.Input2;
                    if (input)
                        wire.ConnectionPoint2 = (wire.Connection2 as ElectronicTile).Foreground.Inputs[wire.ConnectionID2];
                    else
                        wire.ConnectionPoint2 = (wire.Connection2 as ElectronicTile).Foreground.Outputs[wire.ConnectionID2];
                    Level.Wires.Add(wire);
                }
                return tiles;
            }
            throw new Exception();
        }

        /// <summary>
        /// Takes a screen shot of the current frame
        /// </summary>
        public static void Screenie(Game game)
        {
            int width = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = game.GraphicsDevice.PresentationParameters.BackBufferHeight;

            //Force a frame to be drawn (otherwise back buffer is empty) 
            game.InvokeDraw();

            //Pull the picture from the buffer 
            int[] backBuffer = new int[width * height];
            game.GraphicsDevice.GetBackBufferData(backBuffer);

            //Copy to texture
            using (Texture2D texture = new Texture2D(game.GraphicsDevice, width, height, false, game.GraphicsDevice.PresentationParameters.BackBufferFormat))
            {
                texture.SetData(backBuffer);
                //Custom saving routine to prevent XNA memory leak
                //More info at http://stackoverflow.com/a/14310276/1218281
                using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    byte blue;
                    IntPtr safePtr;
                    BitmapData bitmapData;
                    System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
                    byte[] textureData = new byte[4 * width * height];

                    texture.GetData<byte>(textureData);
                    for (int i = 0; i < textureData.Length; i += 4)
                    {
                        blue = textureData[i];
                        textureData[i] = textureData[i + 2];
                        textureData[i + 2] = blue;
                    }
                    bitmapData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                    safePtr = bitmapData.Scan0;
                    Marshal.Copy(textureData, 0, safePtr, textureData.Length);
                    bitmap.UnlockBits(bitmapData);
                    //Save it in the screenshots folder
                    if (Interface.MainWindow.Console != null)
                        Interface.MainWindow.Console.MessageBuffer.Add(new TomShane.Neoforce.Controls.ConsoleMessage("Screenshot saved as " + Directories["Screenshots"] + DateTime.Now.ToString("MM-dd-yy H.mm.ss") + ".png",2));
                    bitmap.Save(Directories["Screenshots"] + DateTime.Now.ToString("MM-dd-yy H_mm") + ".png");
                }
            }

        }
        public static void LoadUniverse(UniverseViewer viewer, string Path)
        {
            IO.CheckFiles();
            using (BinaryReader binaryReader = new BinaryReader( //Create a new binary writer to write to the file
                new BufferedStream(
                new GZipStream(
                    File.Open(Path, FileMode.Open), //Open the file
                CompressionMode.Decompress), //Create a Gzip compressor
                FileBufferSize))) //Get data in 64kb chunks for faster proccessing
            {
                viewer.TotalSize = binaryReader.ReadInt32();
                int galaxies = binaryReader.ReadInt32();
                int homeID = binaryReader.ReadInt32();
                int selectedID = binaryReader.ReadInt32();

                //viewer.Camera.Zoom = binaryReader.ReadSingle();
                viewer.newZoom = binaryReader.ReadSingle();
                viewer.Camera.Zoom = viewer.newZoom;
                viewer.Camera.Position = new Vector2(binaryReader.ReadSingle(),binaryReader.ReadSingle());
                viewer.CameraPointTo = viewer.Camera.Position;
                Game.MainWindow.tbrZoom.Value = (int)(viewer.newZoom * 100);

                for (int i = 0; i < galaxies; i++)
                {
                    Galaxy g = new Galaxy();
                    g.ID = binaryReader.ReadInt32();
                    g.Name = binaryReader.ReadString();
                    g.Radius = binaryReader.ReadSingle();
                    g.OuterRadius = binaryReader.ReadSingle();
                    g.Position = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    g.Children = new List<SolarSystem>();
                    int systems = binaryReader.ReadInt32();

                    for (int a = 0; a < systems; a++)
                    {
                        SolarSystem s = new SolarSystem();
                        s.ID = binaryReader.ReadInt32();
                        s.Name = binaryReader.ReadString();
                        s.Radius = binaryReader.ReadSingle();
                        s.OuterRadius = binaryReader.ReadSingle();
                        s.Position = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        s.Children = new List<PlanetaryObject>();
                        int planets = binaryReader.ReadInt32();

                        for (int b = 0; b < planets; b++)
                        {
                            PlanetaryType Type = (PlanetaryType)binaryReader.ReadInt32();
                            PlanetarySubType SubType = null;
                            if (Type == PlanetaryType.Planet)
                            {
                                int t = binaryReader.ReadInt32();
                                foreach (PlanetType pt in PlanetType.PlanetTypes)
                                    if (t == pt.ID)
                                        SubType = pt;
                            }
                            else if (Type == PlanetaryType.Sun)
                            {
                                int t = binaryReader.ReadInt32();
                                foreach (SunType st in SunType.SunTypes)
                                    if (t == st.ID)
                                        SubType = st;
                            }
                            string Name = binaryReader.ReadString();
                            PlanetaryObject p = new PlanetaryObject(Name, b, s, Type, SubType);
                            p.Name = Name;
                            p.ID = binaryReader.ReadInt32();
                            if (p.ID == homeID)
                                viewer.HomePlanet = p;
                            if (p.ID == selectedID)
                                viewer.SelectedPlanet = p;
                            p.Radius = binaryReader.ReadSingle();
                            p.OuterRadius = binaryReader.ReadSingle();
                            p.Position = new Vector2(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                            p.Angle = binaryReader.ReadSingle();
                            p.Color = new Color(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte()); if (Type == PlanetaryType.Planet)
                                if (Type == PlanetaryType.Planet)
                                    p.Seed = binaryReader.ReadInt32();
                            s.Children.Add(p);
                        }
                        g.Children.Add(s);
                    }
                    viewer.Systems.Add(g);
                }
                int bookmarks = binaryReader.ReadInt32();
                for (int i = 0; i < bookmarks; i++)
                {
                    int id = binaryReader.ReadInt32();
                    foreach (Galaxy galaxy in viewer.Systems)
                        foreach (SolarSystem solarsystem in galaxy.Children)
                            foreach (PlanetaryObject planet in solarsystem.Children)
                            {
                                if (planet.ID == id)
                                    viewer.Bookmarks.Add(planet);
                            }
                }
                int logs = binaryReader.ReadInt32();
                for (int i = 0; i < logs; i++)
                    viewer.Log.Add(binaryReader.ReadString());
            }
        }
        public static void SaveUniverse(UniverseViewer viewer, string Path)
        {
            if (!viewer.Full)
                return;
            IO.CheckFiles();
            using (BinaryWriter binaryWriter = new BinaryWriter( //Create a new binary writer to write to the file
                new BufferedStream(
                new GZipStream(
                    File.Open(Path, FileMode.Create), //Open the file
                CompressionMode.Compress), //Create a Gzip compressor
                FileBufferSize))) //Get data in 64kb chunks for faster proccessing
            {
                binaryWriter.Write(viewer.TotalSize);
                binaryWriter.Write(viewer.Systems.Count);
                binaryWriter.Write(viewer.HomePlanet.ID);
                binaryWriter.Write(viewer.SelectedPlanet != null ? viewer.SelectedPlanet.ID : viewer.HomePlanet.ID);

                binaryWriter.Write(viewer.newZoom);
                binaryWriter.Write(viewer.Camera.Position.X);
                binaryWriter.Write(viewer.Camera.Position.Y);

                foreach (Galaxy galaxy in viewer.Systems)
                {
                    binaryWriter.Write(galaxy.ID);
                    binaryWriter.Write(galaxy.Name);
                    binaryWriter.Write(galaxy.Radius);
                    binaryWriter.Write(galaxy.OuterRadius);
                    binaryWriter.Write(galaxy.Position.X);
                    binaryWriter.Write(galaxy.Position.Y);
                    binaryWriter.Write(galaxy.Children.Count);

                    foreach (SolarSystem system in galaxy.Children)
                    {
                        binaryWriter.Write(system.ID);
                        binaryWriter.Write(system.Name);
                        binaryWriter.Write(system.Radius);
                        binaryWriter.Write(system.OuterRadius);
                        binaryWriter.Write(system.Position.X);
                        binaryWriter.Write(system.Position.Y);
                        binaryWriter.Write(system.Children.Count);

                        foreach (PlanetaryObject p in system.Children)
                        {
                            binaryWriter.Write((int)p.Type);
                            if (p.SubType == null)
                                binaryWriter.Write(-1);
                            else
                                binaryWriter.Write(p.SubType.ID);

                            binaryWriter.Write(p.Name);
                            binaryWriter.Write(p.ID);
                            binaryWriter.Write(p.Radius);
                            binaryWriter.Write(p.OuterRadius);
                            binaryWriter.Write(p.Position.X);
                            binaryWriter.Write(p.Position.Y);
                            binaryWriter.Write(p.Angle);
                            binaryWriter.Write(p.Color.R);
                            binaryWriter.Write(p.Color.G);
                            binaryWriter.Write(p.Color.B);
                            if (p.Type == PlanetaryType.Planet)
                                binaryWriter.Write(p.Seed);
                        }
                    }
                }
                binaryWriter.Write(viewer.Bookmarks.Count);
                foreach (PlanetaryObject planet in viewer.Bookmarks)
                    binaryWriter.Write(planet.ID);
                binaryWriter.Write(Math.Min(viewer.Log.Count, UniverseViewer.MaxLogs));
                foreach (string s in viewer.Log.GetRange(0,Math.Min(viewer.Log.Count, UniverseViewer.MaxLogs)))
                    binaryWriter.Write(s);
            }
        }
        
        /// <summary>
        /// Method for loading all the files in a given folder
        /// </summary>
        /// <typeparam name="T">Type of content to load exception.g Texture2D</typeparam>
        /// <returns>A dictonary of the names and content files</returns>
        public static Dictionary<string, T> LoadListContent<T>(this ContentManager contentManager, string contentFolder)
        {
            DirectoryInfo dir = new DirectoryInfo(contentManager.RootDirectory + "/" + contentFolder);
            if (!dir.Exists)
                throw new DirectoryNotFoundException();
            Dictionary<String, T> result = new Dictionary<String, T>();

            FileInfo[] files = dir.GetFiles("*.*");

            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                result[key] = contentManager.Load<T>(contentFolder + "/" + key);
            }
            return result;
        }
        public static bool MapExists(string map)
        {
            return File.Exists(Directories["Maps"] + map + WorldSuffix);
        }
        public static bool PlanetExists(PlanetaryObject Planet)
        {
            return File.Exists(Directories["Planets"] + Planet.Name + WorldSuffix);
        }

        public static bool ProfileExists()
        {
            return File.Exists(Path.Combine(Directories["Players"], Game.UserName) + PlayerSkinSuffix);
        }
    }
    /// <summary>
    /// Based on http://jakepoz.com/jake_poznanski__background_load_xna.html 
    /// </summary>
    public class TextureLoader : IDisposable
    {
        static TextureLoader()
        {
            BlendColorBlendState = new BlendState
            {
                ColorDestinationBlend = Blend.Zero,
                ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
                AlphaDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorSourceBlend = Blend.SourceAlpha
            };

            BlendAlphaBlendState = new BlendState
            {
                ColorWriteChannels = ColorWriteChannels.Alpha,
                AlphaDestinationBlend = Blend.Zero,
                ColorDestinationBlend = Blend.Zero,
                AlphaSourceBlend = Blend.One,
                ColorSourceBlend = Blend.One
            };
        }

        public ContentManager Content { get; set; }

        public TextureLoader(GraphicsDevice graphicsDevice, ContentManager content, bool needsBmp = false)
        {
            _graphicsDevice = graphicsDevice;
            _needsBmp = needsBmp;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
            Content = content;
        }

        public Texture2D FromFile(string path, bool preMultiplyAlpha = true)
        {
            using (Stream fileStream = File.OpenRead(path))
                return FromStream(fileStream, preMultiplyAlpha);
        }


        public Texture2D FromStream(Stream stream, bool preMultiplyAlpha = true)
        {
            Texture2D texture;

            if (_needsBmp)
            {
                // Load image using GDI because Texture2D.FromStream doesn't support BMP
                using (Image image = Image.FromStream(stream))
                {
                    // Now create a MemoryStream which will be passed to Texture2D after converting to PNG internally
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        texture = Texture2D.FromStream(_graphicsDevice, ms);
                    }
                }
            }
            else
            {
                texture = Texture2D.FromStream(_graphicsDevice, stream);
            }

            if (preMultiplyAlpha)
            {
                // Setup a render target to hold our final texture which will have premulitplied alpha values
                using (RenderTarget2D renderTarget = new RenderTarget2D(_graphicsDevice, texture.Width, texture.Height))
                {
                    Viewport viewportBackup = _graphicsDevice.Viewport;
                    _graphicsDevice.SetRenderTarget(renderTarget);
                    _graphicsDevice.Clear(Color.Black);

                    // Multiply each color by the source alpha, and write in just the color values into the final texture
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendColorBlendState);
                    _spriteBatch.Draw(texture, texture.Bounds, Color.White);
                    _spriteBatch.End();

                    // Now copy over the alpha values from the source texture to the final one, without multiplying them
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendAlphaBlendState);
                    _spriteBatch.Draw(texture, texture.Bounds, Color.White);
                    _spriteBatch.End();

                    // Release the GPU back to drawing to the screen
                    _graphicsDevice.SetRenderTarget(null);
                    _graphicsDevice.Viewport = viewportBackup;

                    // Store data from render target because the RenderTarget2D is volatile
                    Color[] data = new Color[texture.Width * texture.Height];
                    renderTarget.GetData(data);

                    // Unset texture from graphic device and set modified data back to it
                    _graphicsDevice.Textures[0] = null;
                    texture.SetData(data);
                }

            }

            return texture;
        }

        private static readonly BlendState BlendColorBlendState;
        private static readonly BlendState BlendAlphaBlendState;

        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly bool _needsBmp;

        #region IDisposable Members

        public void Dispose()
        {
            _spriteBatch.Dispose();
        }

        #endregion
    }
}
       