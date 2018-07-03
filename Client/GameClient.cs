#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cyral.Extensions;
using Cyral.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using ZarknorthClient.Interface;
#endregion

namespace ZarknorthClient
{
    /// <summary>
    /// The main application class your application will be built upon.
    /// </summary>
    public class Game : Application
    {
        #region Variables
        //Current state of the game
        public static GameState CurrentGameState = GameState.HomeLoggedOff;
        public static GameState lastGameState;
        public static Random random;
        public enum GameState
        {
            HomeLoggedOff,HomeLoggedOn,UniverseViewer,InGame
        }
        public static string UserName = "Guest";
        public static string SessionID;
        public static Level level;
        public static UniverseViewer UniverseViewer;
        public static bool ready;
        public static KeyboardState keyboardState;
        public static KeyboardState oldKeyBoardState;
        public static MouseState mouseState;
        public static MouseState oldMouseState;
        public SpriteBatch spriteBatch;
        public static List<string> s = new List<string>();
        public static int LightingQuality;
        public static string ContentPackName;
        public static int ContentPackIndex;
        public static ContentPack ContentPackData;
        public static bool DropShadows;
        public static bool TileEdges;
        public static bool Antialiasing;
        public static bool Autosave;
        public static int ParticleQuality;
        public static bool Fullscreen;
        public static bool IsMouseOnControl;
        public static int Skin;
        public static int LightingRefresh;
        public static Point Screen;
        public static MainWindow MainWindow;
        public static TextureLoader TextureLoader;
        public static Dictionary<string, Keys> Controls;
        private float RadiusAlpha;
        Texture2D GridTexture;
        public static bool DebugView = false;
        public static bool PlayerView = true;
        public static bool GuiView = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an application using the Default skin file. 
        /// </summary>
        public Game() : base("Red", true)
        {
            //Set up the window and UI defaults
            Manager.Content.RootDirectory = "Content";
            Manager.SkinDirectory = "Content/Skins";

            SystemBorder = false;
            FullScreenBorder = false;
            ExitConfirmation = true;
            ClearBackground = true;
            Manager.TargetFrames = 60;
            Manager.UseGuide = false;
            //Create a new random instance to be used for non level/seeded operations
            random = new Random();

            //Set the window size to the screen size
            Manager.Graphics.PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Width;
            Manager.Graphics.PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Height;
            Manager.Graphics.ApplyChanges();
            ((System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle)).Location = new System.Drawing.Point(0, 0);
           
        }

      
        /// <summary>
        /// Creates a new instance of the main UI window
        /// </summary>
        protected override Window CreateMainWindow()
        {
            IO.CheckFiles();
            IO.LoadSettings(this);
            MainWindow = new ZarknorthClient.Interface.MainWindow(Manager);
            return MainWindow;
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes the application.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            spriteBatch = Manager.Renderer.SpriteBatch as SpriteBatch;
            Extensions.GraphicsDevice = GraphicsDevice;
            TextureLoader = new TextureLoader(Manager.GraphicsDevice, Content);
            IO.LoadContentPacks(this);
            MainWindow.LoadContent();
            GridTexture = ContentPack.Textures["gui\\grid"];
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }
        #endregion

        #region Load Content
        /// <summary>
        /// Loads content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }
        #endregion

        #region Unload Content
        /// <summary>
        /// Unloads content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here.
            base.UnloadContent();
        }
        #endregion
        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            Stopwatch updateWatch = new Stopwatch();
            updateWatch.Start();
            //If window is focused, update
            MainWindow wnd = MainWindow as MainWindow;
            if (level != null && ready == false)
            {
                //Done generating, set UI up and save
                if (level.ready)
                {
                    ready = true;
                    Manager.Cursor = Manager.Skin.Cursors["Default"].Resource;
                    wnd.InitGameScreen();
                    //Force Save
                    if (level.saveOnLoad)
                        level.AutoSave(null, null);
                }
            }
            if (Extensions.ApplicationIsActivated())
            {
                HandleInput(gameTime); // Handle polling for our input and handling high-level input
                oldMouseState = mouseState;
                lastGameState = CurrentGameState;
                oldKeyBoardState = keyboardState;
                mouseState = Mouse.GetState();
                //Check if house is hovering over any controls
                IsMouseOnControl = !CheckPosition(new Point(mouseState.X, mouseState.Y));
                if (Game.CurrentGameState == Game.GameState.InGame)
                {
                    if (level != null && level.ready == true && ready) //Check if level exists
                    {
                        MainWindow.UpdateStats(gameTime, (float)gameTime.ElapsedGameTime.TotalSeconds, mouseState);
                        //Update our level, passing down the GameTime along with all of our input states
                        Stopwatch drawWatch = new Stopwatch();
                        drawWatch.Start();
                        level.Update(gameTime, keyboardState, oldKeyBoardState);
                        drawWatch.Stop();
                        if (MainWindow.DebugList != null)
                            MainWindow.DebugList[1].Value = (int)drawWatch.ElapsedMilliseconds;
                    }
                }
                if (UniverseViewer != null && (CurrentGameState == GameState.UniverseViewer || (CurrentGameState == GameState.InGame && UniverseViewer.State == ZarknorthClient.UniverseViewer.TransitionState.ClosingToLevel)))
                {
                    UniverseViewer.Update(gameTime);
                }
            }
            base.Update(gameTime);
            updateWatch.Stop();
            if (MainWindow.DebugList != null)
                MainWindow.DebugList[0].Value = (int)updateWatch.ElapsedMilliseconds;
        }

        private void HandleNetworking()
        {
            // read messages
            //NetIncomingMessage msg;
            //while ((msg = client.ReadMessage()) != null)
            //{
            //    switch (msg.MessageType)
            //    {
            //        case NetIncomingMessageType.DiscoveryResponse:
            //             just connect to first server discovered
            //            client.Connect(msg.SenderEndPoint);
            //            break;
            //        case NetIncomingMessageType.Data:
            //            byte b = msg.ReadByte();
            //            if (b == (byte)PacketTypes.WorldData)
            //            {
            //                LoadFromServer(msg);
            //            }
            //            else if (b == (byte)PacketTypes.Block)
            //            {
            //                int x = msg.ReadInt32();
            //                int y = msg.ReadInt32();
            //                int id = msg.ReadByte();
            //                level.tiles[x, y].item = Item.Items[id];
            //            }
            //            else if (b == (byte)PacketTypes.Background)
            //            {
            //                int x = msg.ReadInt32();
            //                int y = msg.ReadInt32();
            //                int id = msg.ReadByte();
            //                level.tiles[x, y].background = Item.Items[id];
            //            }
            //            else if (b == (byte)PacketTypes.Movement)
            //            {
            //                int ID = msg.ReadInt32();
            //                float moveX = msg.ReadFloat();
            //                float moveY = msg.ReadFloat();
            //                if (ID != level.Players[0].SimpleID)
            //                {
            //                    Player p = FindPlayer(ID);
            //                    p.position.X = moveX;
            //                    p.position.Y = moveY;
            //                }
            //            }
            //            else if (b == (byte)PacketTypes.PlayerJoin)
            //            {

            //                string name = msg.ReadString();
            //                if (name != UserName)
            //                {
            //                    int ID = msg.ReadInt32();
            //                    float posX = msg.ReadFloat();
            //                    float posY = msg.ReadFloat();
            //                    level.Players.Add(new Player(level, new Vector2(posX, posY), true) { Name = name, SimpleID = ID });

            //                }

            //            }
            //            else if (b == (byte)PacketTypes.PlayerLeave)
            //            {
            //                int ID = msg.ReadInt32();
            //                Interface.MainWindow.userList.Items.Remove(GetPlayerNameSimple(ID));
            //                level.Players.Remove(FindPlayer(ID));

            //            }
            //            else if (b == (byte)PacketTypes.Chat)
            //            {

            //                int ID = msg.ReadInt32();
            //                string Text = msg.ReadString();
            //                string name = GetPlayerNameSimple(ID);
            //                Interface.MainWindow.con1.MessageBuffer.Add(new ConsoleMessage(name + ": " + Text, 0));

            //            }

            //            break;
            //    }
            //}
          
        }
        public void OpenWindow()
        {
            MainWindow wnd = MainWindow as MainWindow;
            TaskCreatePortal tmp = new TaskCreatePortal(Manager);
            tmp.Closing += new WindowClosingEventHandler(wnd.WindowClosing);
            tmp.Closed += new WindowClosedEventHandler(wnd.WindowClosed);
            tmp.Init();
            Manager.Add(tmp);
            tmp.Show();
        }
        #endregion

        #region Level Tools
        private void HandleInput(GameTime gameTime)
        {
            //Get all of our input states
            keyboardState = Keyboard.GetState();
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Game.IsKeyToggled(Game.Controls["Debug Mode"]))
                DebugView = !DebugView;
            if (Game.IsKeyToggled(Game.Controls["Show GUI"]))
                GuiView = !GuiView;
            if (Game.IsKeyToggled(Game.Controls["Show Player"]))
                PlayerView = !PlayerView;
            if (Game.IsKeyToggled(Game.Controls["Toggle Fullscreen"]))
            {
                Fullscreen = !Fullscreen;
                ApplyResolution();
            }

            MainWindow.CloseButton.Enabled = !(CurrentGameState == GameState.InGame || CurrentGameState == GameState.UniverseViewer);
            if (level != null)
            {
                if (level.ready)
                {
                    //If not typing in a textbox...
                    if (!(level.MainWindow.Manager.FocusedControl is TextBox) && !IsMouseOnControl)
                    {

                        if (Game.IsKeyToggled(Game.Controls["Take Screenshot"]))
                            IO.Screenie(this);

                        if (Game.IsKeyToggled(Game.Controls["Open Achievements"]))
                        {
                            TaskAchievementLog AchievementLog = new TaskAchievementLog(Game.level.game.Manager);
                            AchievementLog.Init();
                            Game.level.game.Manager.Add(AchievementLog);
                        }
                        if (Game.IsKeyToggled(Game.Controls["Toggle Chat"]))
                        {
                            MainWindow.MainTabs.Focused = !MainWindow.MainTabs.Focused;
                        }
                        if (Game.IsKeyToggled(Game.Controls["Open Crafting"]))
                        {
                            MainWindow.CraftingWindow.Show(Item.Blank);
                        }

                        if (keyboardState.IsKeyDown(Keys.F10) && !oldKeyBoardState.IsKeyDown(Keys.F10))
                        {
                            if (level.Gamemode == GameMode.Survival)
                            {
                                level.Gamemode = GameMode.Sandbox;
                                MainWindow.sandBoxFadingIn = true;
                                MainWindow.sandBoxFadingOut = false;
                            }
                            else if (level.Gamemode == GameMode.Sandbox)
                            {
                                level.Gamemode = GameMode.Survival;
                                MainWindow.sandBoxFadingOut = true;
                                MainWindow.sandBoxFadingIn = false;
                            }
                        }

                        int LastSelected = Interface.MainWindow.inventory.Selected;
                        #region 1-0 Keys to select inventory
                        //Select an inventory item if 1-0 are presed
                        int key = LastSelected;

                        if (IsKeyToggled(Keys.D1))
                           key = 0;
                        else if (IsKeyToggled(Keys.D2))
                           key = 1;
                        else if (IsKeyToggled(Keys.D3))
                           key = 2;
                        else if (IsKeyToggled(Keys.D4))
                           key = 3;
                        else if (IsKeyToggled(Keys.D5))
                           key = 4;
                        else if (IsKeyToggled(Keys.D6))
                           key = 5;
                        else if (IsKeyToggled(Keys.D7))
                           key = 6;
                        else if (IsKeyToggled(Keys.D8))
                           key = 7;
                        else if (IsKeyToggled(Keys.D9))
                           key = 8;
                        else if (IsKeyToggled(Keys.D0))
                           key = 9;

                        Interface.MainWindow.inventory.Selected = key;
                        if (LastSelected != Interface.MainWindow.inventory.Selected && LastSelected < 10)
                        {
                            Interface.MainWindow.inventory.Slots[LastSelected, 0].button.Focused = false;
                            Interface.MainWindow.inventory.Slots[key, 0].button.Focused = true;
                        }
                        #endregion

                        if (LastSelected != Interface.MainWindow.inventory.Selected)
                        {
                            Interface.MainWindow.inventory.ItemSlots[LastSelected].Item.OnDeSelect(new SelectItemEventArgs(level, LastSelected));
                            foreach (SlotControl s in Interface.MainWindow.inventory.Slots)
                                if (s.ID == Interface.MainWindow.inventory.Selected)
                                    Interface.MainWindow.inventory.button_MouseDown(s.button, null);
                            Interface.MainWindow.inventory.ItemSlots[Interface.MainWindow.inventory.Selected].Item.OnSelect(new SelectItemEventArgs(level,Interface.MainWindow.inventory.Selected));
                        }

                        if (IsKeyToggled(Game.Controls["Toggle Inventory"]))
                        {
                            ToggleInventory();
                        }
                    }
                }
            }
        }

        public static void ToggleInventory()
        {
            if (Interface.MainWindow.expanded == true && Interface.MainWindow.mouseSlot.Equals(Slot.Empty))
                Interface.MainWindow.hiding = true;
            else if (Interface.MainWindow.expanded == false)
                Interface.MainWindow.expanding = true;
        }
        /// <summary>
        /// Draws a single frame and allows Draw to be accessed from another class (Useful for screenshots)
        /// </summary>
        public void InvokeDraw()
        {
            Draw(new GameTime());
        }

        public void GenerateLevel(PlanetaryObject planet)
        {
            level = new Level(Services, this, GetPlanetWidth(planet), GetPlanetHeight(planet), planet.Seed) { MainWindow = MainWindow as MainWindow, Gravity = planet.Gravity, IsMap = false };
            level.Data.Name = planet.Name;
            level.Data.Description = planet.Description;
            level.Data.Seed = planet.Seed;
        }
        public void GenerateLevel(LevelData data, int Width, int Height, Generator generator)
        {
            level = new Level(Services, this, Width,Height, data.Seed) { MainWindow = MainWindow as MainWindow, Gravity = 1, Generator = generator, IsMap = true};
            level.IsMap = true;
            level.Data = data;
        }

        public static int GetPlanetHeight(PlanetaryObject planet)
        {
            return (int)MathHelper.Clamp((int)((planet.Diameter / 256f) * 1500), 500, 2000);
        }
        public static int GetPlanetWidth(PlanetaryObject planet)
        {
            return (int)((planet.Diameter / 256f) * 8000);
        }
        public void LoadLevel(PlanetaryObject planet)
        {
            level = new Level(Services, this) { MainWindow = MainWindow as MainWindow, Gravity = planet.Gravity, IsMap = false };
            level.Data.Name = planet.Name;
            level.Data.Description = planet.Description;
            level.Data.Seed = planet.Seed;
        }
        public void LoadLevel(string map)
        {
            level = new Level(Services, this) { MainWindow = MainWindow as MainWindow, Gravity = 1 };
            level.Data.Name = map;
            level.IsMap = true;
            string[] data = IO.GetLevelData(map);
            level.Data.Description = data[1];
            level.Data.Seed = int.Parse(data[4]);
        }

        public MainWindow GetWnd()
        {
            MainWindow wnd = MainWindow as MainWindow;
            return wnd;
        }
        
        public static bool IsKeyToggled(Keys key)
        {
            if (!oldKeyBoardState.IsKeyDown(key) && keyboardState.IsKeyDown(key))
                return true;
            else
                return false;
        }
     
        #endregion

        #region Draw
        protected override void Draw(GameTime gameTime)
        {
            Stopwatch drawWatch = new Stopwatch();
            drawWatch.Start();
            if (!GuiView)
            {
                DrawScene(gameTime);
            }
            else
            base.Draw(gameTime);
            //Incriment Fps
            MainWindow.Frames++;
            drawWatch.Stop();
            if (MainWindow.DebugList != null)
                MainWindow.DebugList[2].Value = (int)drawWatch.ElapsedMilliseconds;
        }
        /// <summary>
        /// Draws the actual level and non UI objects
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void DrawScene(GameTime gameTime)
        {
            //If the level is loaded
            if (CurrentGameState == GameState.InGame && level != null && level.ready == true && ready)
            {
                Stopwatch drawWatch = new Stopwatch();
                drawWatch.Start();
                level.Draw(gameTime, spriteBatch);
                drawWatch.Stop();
                if (MainWindow.DebugList != null)
                    MainWindow.DebugList[3].Value = (int)drawWatch.ElapsedMilliseconds;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                //Position Data (abs is absolute mouse position and grid is the mouse position on the tile grid)
                double absX = ((float)(Math.Floor((double)(mouseState.X / 24f) + (level.MainCamera.Position.X / 24f)) * 24)).RoundTo(24) - level.MainCamera.Position.X;
                double absY = ((float)(Math.Floor((double)(mouseState.Y / 24f) + (level.MainCamera.Position.Y / 24f)) * 24)).RoundTo(24) - level.MainCamera.Position.Y;
                if (absX <= 0)
                    absX -= Tile.Width;
                
                Vector2 Position = new Vector2(((int)absX), ((int)absY));
                int gridX = (int)(int)((mouseState.X + (int)level.MainCamera.Position.X) / Tile.Width);
                int gridY = (int)MathHelper.Clamp((int)((mouseState.Y + (int)level.MainCamera.Position.Y) / Tile.Height),0, level.Height -1);
                if (gridX <= 0)
                    gridX -= 1;
                //Color Data
                Color redColor = new Color(255, 50, 50);
                Color greenColor = new Color(70, 255, 70);

                //If the player is holding an item
                if (!IsMouseOnControl && Interface.MainWindow.inventory != null && level.Players[0].CurrentSlot.Item != Item.Blank || level.EditType == EditType.Erase)
                {
                    //If we are holding a block
                    if (level.Player.CurrentSlot.Item is BlockItem || level.EditType == EditType.Erase)
                    {
                        Point size = Item.One;
                        BlockItem currentItem = Item.Blank;
                        if (level.Player.CurrentSlot.Item is BlockItem)
                        {
                            //Get Size of the block we are holding
                            size = ((BlockItem)level.Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item).Size;
                            currentItem = ((BlockItem)level.Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item);
                        }
                        if (level.IsMap)
                        {
                            if (level.EditType == EditType.Place || level.EditType == EditType.Erase)
                            {
                                BlockItem item = (currentItem as BlockItem);
                                if (level.EditType == EditType.Erase)
                                    item = Item.Blank;
                                if (((currentItem is BlockItem && item.Size == Item.One && level.EditType == EditType.Place) || level.EditType == EditType.Erase) && level.HeldMode && !Game.IsMouseOnControl && (ZarknorthClient.Interface.MainWindow.inventory.Selected >= 0))
                                {
                                    Color color = greenColor;
                                    Point startPoint = level.LastPoint;
                                    Point endPoint = level.AbsolutePosition.ToPoint();
                                    if (level.EditTool == EditTool.Line)
                                    {
                                        ToolConvertUnits(ref startPoint, ref endPoint, false);
                                        Point realStart = startPoint;
                                        int dx = Math.Abs(endPoint.X - startPoint.X);
                                        int dy = Math.Abs(endPoint.Y - startPoint.Y);

                                        int sx, sy;

                                        if (startPoint.X < endPoint.X) sx = 1; else sx = -1;
                                        if (startPoint.Y < endPoint.Y) sy = 1; else sy = -1;

                                        int err = dx - dy;

                                        while (true)
                                        {
                                            DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - startPoint.X) * Tile.Width, (int)(( realStart.Y - startPoint.Y)) * Tile.Height) - level.MainCamera.Position, size, ref color);

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
                                    else if (level.EditTool == EditTool.Square || level.EditTool == EditTool.FilledSquare)
                                    {
                                        Point realStart = new Point(startPoint.X / Tile.Width, startPoint.Y / Tile.Height);
        
                                        ToolConvertUnits(ref startPoint, ref endPoint, true);
                                        for (int x = startPoint.X; x <= endPoint.X; x++)
                                            for (int y = startPoint.Y; y <= endPoint.Y; y++)
                                            {
                                                if (level.EditTool == EditTool.FilledSquare)
                                                    DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - x) * Tile.Width, (int)((realStart.Y - y)) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                else if (x == startPoint.X || x == endPoint.X || y == startPoint.Y || y == endPoint.Y)
                                                    DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - x) * Tile.Width, (int)((realStart.Y - y)) * Tile.Height) - level.MainCamera.Position, size, ref color);

                                            }
                                    }
                                    else if (level.EditTool == EditTool.Circle || level.EditTool == EditTool.FilledCircle)
                                    {
                                        Point realStart = new Point(startPoint.X / Tile.Width, startPoint.Y / Tile.Height);
        
                                        ToolConvertUnits(ref startPoint, ref endPoint, true);
                                        float radiusVert = Math.Abs(startPoint.Y - endPoint.Y) / 2;
                                        float radiusHor = Math.Abs(startPoint.X - endPoint.X) / 2;
                                        int radius = (int)Math.Round(Math.Max(radiusHor, radiusVert));
                                        int x0 = startPoint.X + (int)Math.Round(radiusHor);
                                        int y0 = startPoint.Y + (int)Math.Round(radiusVert);
                                        int x = radius;
                                        int y = 0;
                                        int xChange = 1 - (radius << 1);
                                        int yChange = 0;
                                        int radiusError = 0;

                                        while (x >= y)
                                        {
                                            if (level.EditTool != EditTool.FilledCircle)
                                            {
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (x + x0)) * Tile.Width, (int)((realStart.Y - (y + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (y + x0)) * Tile.Width, (int)((realStart.Y - (x + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (-x + x0)) * Tile.Width, (int)((realStart.Y - (y + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (-y + x0)) * Tile.Width, (int)((realStart.Y - (x + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (-x + x0)) * Tile.Width, (int)((realStart.Y - (-y + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (-y + x0)) * Tile.Width, (int)((realStart.Y - (-x + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (x + x0)) * Tile.Width, (int)((realStart.Y - (-y + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (y + x0)) * Tile.Width, (int)((realStart.Y - (-x + y0))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                              
                                            }
                                            else
                                            {
                                                for (int i = x0 - x; i <= x0 + x; i++)
                                                {
                                                    DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (i)) * Tile.Width, (int)((realStart.Y - (y0 + y))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                    DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (i)) * Tile.Width, (int)((realStart.Y - (y0 - y))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                }
                                                for (int i = x0 - y; i <= x0 + y; i++)
                                                {
                                                    DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (i)) * Tile.Width, (int)((realStart.Y - (y0 + x))) * Tile.Height) - level.MainCamera.Position, size, ref color);
                                                    DrawTileBox(new Vector2(gridX * Tile.Width, gridY * Tile.Height) + new Vector2((int)(realStart.X - (i)) * Tile.Width, (int)((realStart.Y - (y0 - x))) * Tile.Height) - level.MainCamera.Position, size, ref color);
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
                                   
                                }
                            }
                        }

                        if (level.IsMap && level.EditTool == EditTool.Default && ((level.EditType == EditType.Erase) || (level.EditType == EditType.Place && size == Item.One)))
                        {
                            for (int x = -(MainWindow.SizeTool.Value); x <= MainWindow.SizeTool.Value; x++)
                            {
                                for (int y = -(MainWindow.SizeTool.Value); y <= MainWindow.SizeTool.Value; y++)
                                {
                                    if (level.InLevelBounds(gridX + x, gridY + y) && Vector2.Distance(new Vector2(gridX + x, gridY + y), new Vector2(gridX, gridY)) <= MainWindow.SizeTool.Value)
                                    {
                                        Tile tileItem = level.tiles[gridX+ x, gridY + y];
                                        Color color = Color.White;
                                        color = level.CanPlaceBlock(gridX + x, gridY + y, currentItem) ? greenColor : redColor;

                                        //Draw the different sides of the texture
                                        DrawTileBox(Position + new Vector2(x * Tile.Width, y * Tile.Height), size, ref color);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Tile tileItem = level.tiles[gridX, gridY];
                            Color color = Color.White;
                            color = level.CanPlaceBlock(gridX, gridY, currentItem) ? greenColor : redColor;

                            //Draw the different sides of the texture
                            DrawTileBox(Position, size, ref color);
                        }
                    }
                    else if (level.Player.CurrentSlot.Item == Item.PaintBrush)
                    {
                        Tile tileItem = null;
                        Color color = Color.White;
                        if (keyboardState.IsKeyDown(Game.Controls["Place on Background"]) && level.tiles[gridX, gridY, true].Background.PaintColors != null)
                        {
                            tileItem = level.tiles[gridX, gridY, true];
                            color = Level.GetPaintColor(level.tiles[gridX, gridY], tileItem, Color.White, true);
                            Interface.MainWindow.PaintWindow.UpdateColors(tileItem.Background.PaintColors);
                        }
                        else if (level.tiles[gridX, gridY].Foreground.PaintColors != null)
                        {
                            tileItem = level.tiles[gridX, gridY];
                            color = Level.GetPaintColor(level.tiles[gridX, gridY], tileItem, Color.White, false);
                            Interface.MainWindow.PaintWindow.UpdateColors(tileItem.Foreground.PaintColors);
                        }
                        //Draw the different sides of the texture
                        DrawTileBox(Position, new Point(1, 1), ref color);
                    }
                    //If we are holding a pickaxe
                    else if (level.Player.CurrentSlot.Item is MineItem)
                    {
                        if (mouseState.LeftButton == ButtonState.Pressed)
                        {
                            if (RadiusAlpha < 1)
                            RadiusAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                        }
                        else if (RadiusAlpha > 0)
                            RadiusAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                        if (RadiusAlpha > 0)
                        {
                            bool canMine = level.CanMine(gridX, gridY, level.Players[0].CurrentSlot.Item as MineItem);
                            spriteBatch.DrawCircle(new Vector2(level.Players[0].Position.X, level.Players[0].Position.Y - (level.Players[0].BoundingRectangle.Height / 2)) - level.MainCamera.Position, (level.Players[0].CurrentSlot.Item as MineItem).Radius * Tile.Width, (level.Players[0].CurrentSlot.Item as MineItem).Radius * 10, canMine ? greenColor * RadiusAlpha * .5f : redColor * RadiusAlpha * .5f);
                        }
                    }
                }
                spriteBatch.End();
            }
            if (UniverseViewer != null && (CurrentGameState == GameState.UniverseViewer || (CurrentGameState == GameState.InGame && UniverseViewer.State == ZarknorthClient.UniverseViewer.TransitionState.ClosingToLevel)))
            {
                UniverseViewer.Draw(gameTime, spriteBatch);
            }
            base.DrawScene(gameTime);
        }
        private static void ToolConvertUnits(ref Point start, ref Point end, bool sort = true)
        {
            if (sort)
            {
                int startX = start.X / Tile.Width;
                int startY = start.Y / Tile.Height;
                int endX = end.X / Tile.Width;
                int endY = end.Y / Tile.Height;
                start.X = Math.Min(startX, endX);
                end.X = Math.Max(startX, endX);
                start.Y = Math.Min(startY, endY);
                end.Y = Math.Max(startY, endY);
            }
            else
            {
                start.X = start.X / Tile.Width;
                start.Y = start.Y / Tile.Height;
                end.X = end.X / Tile.Width;
                end.Y = end.Y / Tile.Height;
            }
        }
        private void DrawTileBox(Vector2 Position, Point size, ref Color color)
        {
            if (level.MainCamera.Position.X + Position.X > level.MainCamera.Position.X
                 && level.MainCamera.Position.Y + Position.Y > level.MainCamera.Position.Y
                     && level.MainCamera.Position.X + Position.X < level.MainCamera.Position.X + MainWindow.Width
                     && level.MainCamera.Position.Y + Position.Y < level.MainCamera.Position.Y + MainWindow.Height)
            {
                if (level.EditType == EditType.Erase)
                    size = Item.One;
                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X - Tile.Width, (int)Position.Y - Tile.Height, Tile.Width, Tile.Height), new Rectangle(0, 0, Tile.Width, Tile.Height), color);
                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X, (int)Position.Y - Tile.Height, size.X * Tile.Width, Tile.Height), new Rectangle(Tile.Width, 0, Tile.Width, Tile.Height), color);
                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X + ((size.X) * Tile.Width), (int)Position.Y - Tile.Height, Tile.Width, Tile.Height), new Rectangle(Tile.Width * 2, 0, Tile.Width, Tile.Height), color);

                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X - Tile.Width, (int)Position.Y, Tile.Width, Tile.Height * size.Y), new Rectangle(0, Tile.Height, Tile.Width, Tile.Height), color);
                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X + ((size.X) * Tile.Width), (int)Position.Y, Tile.Width, Tile.Height * size.Y), new Rectangle(Tile.Width * 2, Tile.Height, Tile.Width, Tile.Height), color);

                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X - Tile.Width, (int)Position.Y + (size.Y * Tile.Height), Tile.Width, Tile.Height), new Rectangle(0, Tile.Height * 2, Tile.Width, Tile.Height), color);
                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X, (int)Position.Y + (size.Y * Tile.Height), size.X * Tile.Width, Tile.Height), new Rectangle(Tile.Width, Tile.Height * 2, Tile.Width, Tile.Height), color);
                spriteBatch.Draw(GridTexture, new Rectangle((int)Position.X + ((size.X) * Tile.Width), (int)Position.Y + (size.Y * Tile.Height), Tile.Width, Tile.Height), new Rectangle(Tile.Width * 2, Tile.Height * 2, Tile.Width, Tile.Height), color);
            }
        }
        #endregion
        
        #region UI Extras
        /// <summary>
        /// Checks to see if the mouse is over a control (Mostly used for seeing if you can place a block there)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="cl"></param>
        /// <returns></returns>
        public bool CheckPosition(Point pos)
        {
            if (CurrentGameState == GameState.InGame && MainWindow.InventoryPanel != null && keyboardState.IsKeyDown(Controls["Toggle Inventory"]) && !CheckControlPos(MainWindow.InventoryPanel, pos))
                return false;
            // Is the mouse cursor within the application window?
            if (pos.X >= 0 && pos.X <= Manager.TargetWidth && pos.Y >= 24 && pos.Y <= Manager.TargetHeight)
            {
                if (MainWindow.InventoryPanel != null && !CheckControlPos(MainWindow.InventoryPanel, pos))
                    return false;
                foreach (Control c in Manager.Controls)
                {
                    if (!CheckControlPos(c, pos))
                        return false;
                }
                // Mouse is not over any controls, but is within the application window.
                return true;
            }
            else
                return false;
        }
        public bool CheckControlPos(Control c, Point pos)
        {
        
            // Is this a visible control other than the MainWindow?
            // Is the mouse cursor within this control's boundaries?
            if (c.Visible && !c.Passive && c != MainWindow && c != Interface.MainWindow.mouseImage && c != Interface.MainWindow.mouseLabel &&
                pos.X >= c.AbsoluteRect.Left && pos.X <= c.AbsoluteRect.Right &&
                pos.Y >= c.AbsoluteRect.Top && pos.Y <= c.AbsoluteRect.Bottom)
            {
                // Yes, mouse cursor is over this control.
                return false;
            }
            else
                return true;
        }
        #endregion

        public void OpenUniverseViewer(bool full = true)
        {
            CurrentGameState = GameState.UniverseViewer;
            if (UniverseViewer != null)
            {
            }
            else
            {
                UniverseViewer = new UniverseViewer(Services, this);
                if (full)
                UniverseViewer.Open();
                UniverseViewer.Full = full;
                BackgroundImageColor = Color.Transparent;
            }
        }
        public static string GetVersionString()
        {
            VersionText[] versionText = (VersionText[])System.Reflection.Assembly.GetExecutingAssembly()
                           .GetCustomAttributes(typeof(VersionText), false)
                           .Cast<VersionText>();
            string VersionText = System.Reflection.Assembly.GetEntryAssembly().GetName().Name + " " + versionText[0].Text + " " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            return VersionText;
        }
        /// <summary>
        /// Applies new display settings to the game
        /// </summary>
        public void ApplyResolution()
        {
            //Get vars
            System.Windows.Forms.Form Form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            System.Windows.Forms.Screen Screen = (System.Windows.Forms.Screen)System.Windows.Forms.Screen.FromHandle(Window.Handle);
            //Apply settings
            if (Fullscreen)
            {
                Manager.Graphics.PreferMultiSampling = Antialiasing;
                Manager.Graphics.PreferredBackBufferWidth = Manager.ScreenWidth = Screen.Bounds.Width;
                Manager.Graphics.PreferredBackBufferHeight = Manager.ScreenHeight = Screen.Bounds.Height;
                Manager.Graphics.IsFullScreen = Fullscreen;
                Manager.Graphics.ApplyChanges();
                Manager.Window.Left = 0;
                Manager.Window.Top = 0;
                System.Windows.Forms.Control.FromHandle(Manager.Game.Window.Handle).Location = new System.Drawing.Point(0,0);
            }
            else
            {
                Manager.Graphics.IsFullScreen = Fullscreen;
                Manager.Graphics.PreferMultiSampling = Antialiasing;
                Manager.Graphics.PreferredBackBufferWidth = Manager.ScreenWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Width;
                Manager.Graphics.PreferredBackBufferHeight = Manager.ScreenHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size.Height;
                Manager.Graphics.ApplyChanges();
                Manager.Window.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left;
                Manager.Window.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top;
                System.Windows.Forms.Control.FromHandle(Manager.Game.Window.Handle).Location = new System.Drawing.Point(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top);
            }
            //Create new sized lighting target
            if (Game.level != null)
                Game.level.LightingTarget = null;

            //Fix sidebar
            MainWindow window = GetWnd();
            if (CurrentGameState == GameState.InGame)
            {
                MainWindow.sidebar.Top = window.ClientTop;
                MainWindow.sidebar.Left = window.ClientLeft;
                MainWindow.sidebar.Width = window.ClientWidth;
            }
            else if (CurrentGameState == GameState.HomeLoggedOn)
            {
                MainWindow.sidebar.Top = window.ClientTop;
                MainWindow.sidebar.Left = window.ClientLeft;
                MainWindow.sidebar.Width = window.ClientWidth;
                MainWindow.DeInitMainScreen();
                MainWindow.InitMainScreen();
            }
            else if (CurrentGameState == GameState.HomeLoggedOff && MainWindow.sidebar != null)
            {
                MainWindow.sidebar.Top = window.ClientTop;
                MainWindow.sidebar.Left = window.ClientLeft;
                MainWindow.sidebar.Width = window.ClientWidth;
                MainWindow.Copyright.Top = MainWindow.ClientHeight - 24;
                MainWindow.menuBackground.Left = (Fullscreen ? MainWindow.Width : MainWindow.ClientWidth) - MainWindow.menuBackground.Width;
                MainWindow.ResetScrollbar();
            
            }
            else if (CurrentGameState == GameState.UniverseViewer)
            {
                MainWindow.sidebar.Left = MainWindow.ClientArea.Left;
                MainWindow.sidebar.Top = MainWindow.ClientArea.Top;
                MainWindow.sidebar.Width = 322;
                MainWindow.sidebar.Height = MainWindow.ClientHeight;
                MainWindow.sidebar.Anchor = Anchors.Vertical | Anchors.Left;
                MainWindow.universeExit.Top = MainWindow.ClientHeight - MainWindow.universeExit.Height - 8;
                MainWindow.universeExit.Left = MainWindow.ClientWidth - MainWindow.universeExit.Width - 8;
#if DEBUG
                MainWindow.debugEnter.Top = MainWindow.ClientHeight - MainWindow.debugEnter.Height - 8;
                MainWindow.debugEnter.Left = MainWindow.ClientWidth - MainWindow.debugEnter.Width - 16 - MainWindow.universeExit.Width;
#endif
                MainWindow.ResetScrollbar();
                MainWindow.sidebar.Height = MainWindow.ClientHeight;
                MainWindow.universeTabs.Height = MainWindow.sidebar.ClientHeight - 8 - MainWindow.universeTabs.Top;
            }
        }
    }
}
