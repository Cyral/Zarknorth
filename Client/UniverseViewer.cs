using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Cyral.Extensions;
using Cyral.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using ZarknorthClient.Interface;

namespace ZarknorthClient
{
    public class UniverseViewer : IDisposable
    {
        #region Enums
        public enum TransitionState
        {
            Normal,
            Generating,
            Loading,
            Saving,
            ClosingToLevel,
            ClosingToViewer,
        }
        #endregion

        #region Properties
        public List<Galaxy> Systems { get; private set; }
        public Camera Camera { get; set; }
        public Vector2 CameraPointTo;
        public PlanetaryObject HomePlanet { get; set; }
        public PlanetaryObject SelectedPlanet { get; set; }
        public TransitionState State { get; set; }
        public List<string> Log { get; set; }
        public List<PlanetaryObject> Bookmarks { get; set; }
        #endregion

        #region Fields
        public bool Full;
        public int TotalSize;
        public const int MaxLogs = 35;
        public const float MaxZoom = 1f;
        public const float MinZoom = .016f;
        public const float CameraMoveSpeed = 60f;
        public const float CameraLerpSpeed = .2f;
        private Game game;
        private ContentManager content;
        private MouseState mouseState;
        private MouseState lastMouseState;
        public Texture2D SunTexture, PlanetTexture, Shadow;
        private float nameAlpha = 1f;
        private float descAlpha = 1f;
        private const float nameShowThresh = .2f;
        private const float descShowThresh = .75f;
        private bool nameFadingIn, nameFadingOut, descFadingIn, descFadingOut;
        public float newZoom
        {
            get
            {
                return newzoom;
            }
            set
            {
                newzoom = value;
            }
        }
            private float newzoom;
        private float backAlpha;
        private Vector2 scroll;
        private float maxDistance;
        private float BackgroundSpeed = .024f;
        int total = 20;
        int apart = 12000;
        int deviation = 6500;
        #endregion

        #region Constructors
        public UniverseViewer(IServiceProvider serviceProvider, Game game)
        {
            newZoom = 1;
            this.game = game;
            content = new ContentManager(serviceProvider, "Content");
            Systems = new List<Galaxy>();
            Camera = new Camera(game.Manager.Renderer.SpriteBatch.GraphicsDevice.Viewport);
            CameraPointTo = Camera.Position;
            Camera.Zoom = 1;
            State = TransitionState.Normal;
            PlanetTexture = ContentPack.Textures["environment\\planet"];
            SunTexture = ContentPack.Textures["environment\\sunplanet"];
            Shadow = ContentPack.Textures["gui\\shadow"];

            Bookmarks = new List<PlanetaryObject>();
            Log = new List<string>();
        }
        #endregion

        #region Methods
        public void Next()
        {
            foreach (Galaxy galaxy in Systems)
            {
                if (galaxy.ID == SelectedPlanet.ID + 1)
                {
                    SelectedPlanet = galaxy.Children[0].Children[0];
                    break;
                }
                foreach (SolarSystem solarsystem in galaxy.Children)
                {
                    if (solarsystem.ID == SelectedPlanet.ID + 1)
                    {
                        SelectedPlanet = solarsystem.Children[0];
                        break;
                    }
                    foreach (PlanetaryObject planet in solarsystem.Children)
                    {
                        if (planet.ID == SelectedPlanet.ID + 1)
                        {
                            SelectedPlanet = planet;
                            break;
                        }
                    }
                }
            }
            ClickPlanet(SelectedPlanet);
            CameraPointTo = SelectedPlanet.Position - new Vector2((Game.MainWindow.ClientWidth) / 2, (Game.MainWindow.ClientHeight) / 2);
        }
        public void Back()
        {
            foreach (Galaxy galaxy in Systems)
            {
                if (galaxy.ID == SelectedPlanet.ID - 1)
                {
                    SelectedPlanet = galaxy.Children[0].Children[0];
                    break;
                }
                foreach (SolarSystem solarsystem in galaxy.Children)
                {
                    if (solarsystem.ID == SelectedPlanet.ID - 1)
                    {
                        SelectedPlanet = solarsystem.Children[0];
                        break;
                    }
                    foreach (PlanetaryObject planet in solarsystem.Children)
                    {
                        if (planet.ID == SelectedPlanet.ID - 1)
                        {
                            SelectedPlanet = planet;
                            break;
                        }
                    }
                }
            }
            ClickPlanet(SelectedPlanet);
            CameraPointTo = SelectedPlanet.Position - new Vector2((Game.MainWindow.ClientWidth) / 2, (Game.MainWindow.ClientHeight) / 2);
        }

        public void CenterCamera()
        {
            ClickPlanet(SelectedPlanet);
            CameraPointTo = SelectedPlanet.Position - new Vector2(((Game.MainWindow.ClientWidth) / 2) - SelectedPlanet.Radius, ((Game.MainWindow.ClientHeight) / 2) - SelectedPlanet.Radius);
        }
        public void Save(Level level)
        {
            State = UniverseViewer.TransitionState.Saving;
            MainWindow.lblLoading.Text = "Saving World...";
            MainWindow.prgLoading.Color = Color.Orange;
            Thread SaveThread = new Thread(delegate()
            {
                bool map = level.IsMap;
                IO.SaveLevel(level);
                IO.SavePlayer(level.Player);
                backAlpha = 1f;
                if (!map)
                State = TransitionState.ClosingToViewer;
                else
                {
                    Game.MainWindow.ExitUniverse(true);
                    //Game.MainWindow.DeInitMainScreen();
                    Game.MainWindow.InitSandboxScreen();
                    Game.MainWindow.InitLogo();
                }
            });
            SaveThread.Start();
        }
        public void OpenMap(string Map)
        {
            if (IO.MapExists(Map))
            {
                //TODO: Loading
                backAlpha = 1;
                State = TransitionState.Loading;
                MainWindow.lblLoading.Text = "Loading Map...";
                game.LoadLevel(Map);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Map does not exist anymore.", "Error");
            }
        }
        public void GenerateMap(LevelData Data, int Width, int Height, Generator generator)
        {
            backAlpha = 1;
            State = TransitionState.Generating;
            MainWindow.lblLoading.Text = "Generating Map...";
            game.GenerateLevel(Data, Width, Height, generator);
        }
        public void Land(PlanetaryObject planet)
        {
            Log.Add(planet.Name + " - " + DateTime.Now.ToString());
            if (IO.PlanetExists(planet))
            {
                //TODO: Loading
                backAlpha = 1;
                State = TransitionState.Loading;
                MainWindow.lblLoading.Text = "Loading Planet...";
                game.LoadLevel(planet);
            }
            else
            {
                backAlpha = 1;
                State = TransitionState.Generating;
                MainWindow.lblLoading.Text = "Generating Planet...";
                game.GenerateLevel(planet);
            }
        }
        public void GoHome()
        {
            SelectedPlanet = HomePlanet;
            CenterCamera();
        }
        public void SetHome()
        {
            HomePlanet = SelectedPlanet;
            ClickPlanet(SelectedPlanet);
        }
        public Vector2 FindSystemPosition(Galaxy g, int i)
        {
              Vector2 Position = PlanetaryObject.Rotate(MathHelper.ToRadians(Game.random.Next(0, 360)), g.Children[i - 1].Children[g.Children[i - 1].Children.Count - 1].OuterRadius * 3, g.Children[i - 1].Position);
              foreach (SolarSystem s in g.Children)
                  if (Vector2.Distance(s.Position, Position) < s.Radius)
                      FindSystemPosition(g, i);
                  else
                      return Position;
              return Vector2.Zero;
        }
        public void Open()
        {
            //Keep track of time to open
            Stopwatch timer = new Stopwatch();
            timer.Start();
            string method = string.Empty;

            if (File.Exists(IO.UniverseFile))
            {
                //Load
                method = "loaded";
                IO.LoadUniverse(this, IO.UniverseFile);
     
                foreach (PlanetaryObject p in Bookmarks)
                    MainWindow.bookmarkList.Items.Add(p.Name);
                foreach (string s in Log)
                    MainWindow.logList.Items.Add(s);
            }
            else
            {
                //Generate and save
                method = "generated";
                Generate();
                IO.SaveUniverse(this, IO.UniverseFile);
                SelectDefaultPlanet(Systems[0]);
                CenterCamera();
            }
            ClickPlanet(SelectedPlanet);
            maxDistance = Math.Abs(Vector2.Distance(Vector2.Zero, Systems[0].Children[Systems[0].Children.Count - 1].Position));

            //Diagnostics
            timer.Stop();
            int numPlanets = 0;
            int numGalaxies = 0;
            int numSystems = 0;
            foreach (Galaxy galaxy in Systems)
            {
                numGalaxies++;
                foreach (SolarSystem solarsystem in galaxy.Children)
                {
                    numSystems++;
                    foreach (PlanetaryObject planet in solarsystem.Children)
                        numPlanets++;
                }
            }

            Debug.WriteLine("Info: UniverseViewer " + method + " in " + timer.ElapsedMilliseconds + "ms.\n    " + "Total galaxies: " + numGalaxies + ", systems: " + numSystems + ", planets: " + numPlanets);
        }
        public void Generate()
        {
            Debug.Assert(Systems.Count <= 0, "Universe already created");

            //Create a galaxy
            Galaxy g = new Galaxy() { Position = Vector2.Zero };
           
            //Add solar systems
            for (int i = 0; i < total * total; i++)
            {
                int posX = ((i % total) * apart) + Game.random.Next(-deviation, deviation);
                int posY = ((i / total) * apart) + Game.random.Next(-deviation, deviation);
                    //Add system
                    if (i == 0)
                        g.Children.Add(new SolarSystem()
                        {
                            Name = PlanetNamer.NamePlanet(),
                            Position = Vector2.Zero,
                        });
                    else
                    {
                        Vector2 Position = new Vector2(posX, posY);
                        g.Children.Add(new SolarSystem()
                        {
                            Name = PlanetNamer.NamePlanet(),
                            Position = Position,
                        });
                    }


                    //Planets per system
                    int totalPlanets = Game.random.Deviation(3, 11, 7);
                    //Loop through planets to be made
                    for (int planet = 0; planet < totalPlanets; planet++)
                    {
                        //First object is always a sun
                        if (planet == 0)
                        {
                            //Choose a type of sun
                            PlanetarySubType SubType = SunType.GetRandom();

                            //Add and set it up
                            g.Children[i].Children.Add(new PlanetaryObject(g.Children[i].Name, planet, g.Children[i], PlanetaryType.Sun, SubType)
                            {
                                Diameter = Game.random.Deviation((SubType as SunType).MinDiameter, (SubType as SunType).MaxDiameter, (SubType as SunType).AvgDiameter),
                                Color = ((SunType)SubType).Colors[Game.random.Next(0, ((SunType)SubType).Colors.Length)],
                                OuterRadius = 0,
                                Position = g.Children[i].Position,
                                Angle = 0,
                            });
                        }
                        //Others are planets
                        else
                        {
                            //Choose type of planet
                            PlanetarySubType SubType = PlanetType.GetRandom((float)planet / (float)totalPlanets);

                            //Choose diameter
                            int Diameter = (int)MathHelper.Clamp(((float)(planet - 1) / (float)totalPlanets <= .5f ? (int)(((float)(planet - 1) / (float)totalPlanets) * (100f * 2)) + 156 : (int)((1f - ((float)(planet) / ((float)totalPlanets - 1))) * (100f * 2)) + 156) + Game.random.Next(-30, 30), 156, 256);

                            //Choose 'outer radius', the ring
                            float OuterRadius = 0;
                            if (planet == 1) //First planet in system
                                OuterRadius = (g.Children[i].Children[0].Diameter) + Diameter + Game.random.Next(120, 220);
                            else
                                OuterRadius = g.Children[i].Children[planet - 1].OuterRadius + (g.Children[i].Children[planet - 1].Diameter) + Game.random.Next(0, 65);

                            //Add and set it up
                            g.Children[i].Children.Add(new PlanetaryObject(g.Children[i].Name, planet, g.Children[i], PlanetaryType.Planet, SubType)
                            {
                                Diameter = Diameter,
                                Color = Color.Lerp(((PlanetType)SubType).Colors[Game.random.Next(0, ((PlanetType)SubType).Colors.Length)],
                                ((PlanetType)SubType).Colors[Game.random.Next(0, ((PlanetType)SubType).Colors.Length)], (float)Game.random.NextDouble()),
                                OuterRadius = OuterRadius,
                                Angle = Game.random.Next(0, 360),
                            });

                            g.Children[i].Radius = OuterRadius;
                        }

                    }
                
            }
            Systems.Add(g);
            HomePlanet = g.Children[(g.Children.Count / 2) +(total / 2)].Children[1];
            TotalSize = total;
        }

        private void SelectDefaultPlanet(Galaxy g)
        {
            SelectedPlanet = HomePlanet;
            ClickPlanet(SelectedPlanet);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            switch (State)
            {
                case TransitionState.Normal:
                    {
                        if (!MainWindow.universeExit.Visible)
                            MainWindow.universeExit.Visible = true;
                        if (Full)
                        {
                            DrawBackground(spriteBatch);
                            DrawRings(gameTime, spriteBatch);
                            DrawPlanets(spriteBatch);
                            DrawStars(spriteBatch);
                            DrawDebugInfo(spriteBatch);
                        }
                        break;
                    }
                case TransitionState.ClosingToLevel:
                    {
                        if (backAlpha < 1f)
                        {
                            backAlpha += (float)(gameTime.ElapsedGameTime.TotalSeconds) * 1.5f;
                            MainWindow.sidebar.Alpha = 255;
                            MainWindow.lblLoadingDesc.TextColor = Color.White * (1f - backAlpha);
                            MainWindow.lblLoading.TextColor = Color.White * (1f - backAlpha);
                            MainWindow.prgLoading.Color = Color.LawnGreen * (1f - backAlpha);
                        }
                        else if (backAlpha >= 1f)
                        {
                            backAlpha = 1f;
                            if (Full)
                            IO.SaveUniverse(this, IO.UniverseFile);
                            Game.UniverseViewer = null;
                            return;
                        }
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
                        spriteBatch.Draw(game.BackgroundImage, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), new Rectangle(-(int)scroll.X, -(int)scroll.Y, game.BackgroundImage.Width, game.BackgroundImage.Height), Color.White * (1f - backAlpha));
                        spriteBatch.Draw(Shadow, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), Color.White * (1f - backAlpha));
                        spriteBatch.End();
                        break;
                    }
                case TransitionState.ClosingToViewer:
                    {
                        if (backAlpha > 0f)
                        {
                            backAlpha -= (float)(gameTime.ElapsedGameTime.TotalSeconds) * 1.5f;
                            MainWindow.sidebar.Alpha = 255 * (1f - backAlpha);
                            MainWindow.lblLoadingDesc.TextColor = Color.White * backAlpha;
                            MainWindow.lblLoading.TextColor = Color.White * backAlpha;
                            MainWindow.prgLoading.Color = Color.Orange * backAlpha;

                            if (Full)
                            {
                                DrawBackground(spriteBatch);
                                DrawRings(gameTime, spriteBatch);
                                DrawPlanets(spriteBatch);
                                DrawStars(spriteBatch);
                                DrawDebugInfo(spriteBatch);
                            }
                            MainWindow.universeExit.Visible = true;
                            MainWindow.universeExit.Color = Color.Red * (1f - backAlpha);
                            MainWindow.universeExit.TextColor = Color.White * (1f - backAlpha);
#if DEBUG
                            MainWindow.debugEnter.Visible = true;
                            MainWindow.debugEnter.Color = Color.Orange * (1f - backAlpha);
                            MainWindow.debugEnter.TextColor = Color.White * (1f - backAlpha);
#endif
                        }
                        else if (backAlpha <= 0f)
                        {
                            backAlpha = 0;
                            State = TransitionState.Normal;
                            if (Full)
                            {
                                DrawBackground(spriteBatch);
                                DrawRings(gameTime, spriteBatch);
                                DrawPlanets(spriteBatch);
                                DrawStars(spriteBatch);
                                DrawDebugInfo(spriteBatch);
                            }
                        }
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
                        spriteBatch.Draw(game.BackgroundImage, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), new Rectangle(-(int)scroll.X, -(int)scroll.Y, game.BackgroundImage.Width, game.BackgroundImage.Height), Color.White * backAlpha);
                        spriteBatch.Draw(Shadow, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), Color.White * backAlpha);
                        spriteBatch.End();
                        break;
                    }
                case TransitionState.Saving:
                    {
                        if (backAlpha < 1f)
                        {
                            backAlpha += (float)(gameTime.ElapsedGameTime.TotalSeconds) * 1.5f;
                            MainWindow.sidebar.Alpha = 0;
                            MainWindow.lblLoadingDesc.TextColor = Color.White * backAlpha;
                            MainWindow.lblLoading.TextColor = Color.White * backAlpha;
                            MainWindow.prgLoading.Color = Color.Orange * backAlpha;
                        }
                        scroll.X -= (float)(gameTime.ElapsedGameTime.TotalSeconds) * 18;
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
                        spriteBatch.Draw(game.BackgroundImage, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), new Rectangle(-(int)scroll.X, -(int)scroll.Y, game.BackgroundImage.Width, game.BackgroundImage.Height), Color.White * backAlpha);
                        spriteBatch.Draw(Shadow, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), Color.White * backAlpha);
                        spriteBatch.End();
                        break;
                    }
                case TransitionState.Loading:
                case TransitionState.Generating:
                    {
                        if (backAlpha > 0f)
                        {
                            #if !DEBUG
                            if (Full)
                            {
                                DrawBackground(spriteBatch);
                                DrawRings(gameTime, spriteBatch);
                                DrawPlanets(spriteBatch);
                                DrawStars(spriteBatch);
                                DrawDebugInfo(spriteBatch);
                            }
                            MainWindow.sidebar.Alpha = backAlpha * 255;
#else
                            MainWindow.sidebar.Alpha = 0;
                            #endif
                            backAlpha -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
                        }
                        else if (backAlpha <= 0f)
                        {
                            backAlpha = 0f;
                            MainWindow.sidebar.Alpha = backAlpha * 255;
                            game.GetWnd().DeInitUniverseScreen();
                        }
                        scroll.X -= (float)(gameTime.ElapsedGameTime.TotalSeconds) * 18;
                        spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
                        spriteBatch.Draw(game.BackgroundImage, new Rectangle(0,0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), new Rectangle(-(int)scroll.X, -(int)scroll.Y, game.BackgroundImage.Width, game.BackgroundImage.Height), Color.White * (1f - backAlpha));
                        spriteBatch.Draw(Shadow, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), Color.White * (1f - backAlpha));
                        spriteBatch.End();
                        break;
                    }
            }
        }

        #region Draw
        private void DrawDebugInfo(SpriteBatch spriteBatch)
        {
            //Debug Draw
            if (Game.DebugView)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, null);
                foreach (Galaxy galaxy in Systems)
                {
                    foreach (SolarSystem solarsystem in galaxy.Children)
                    {
                        foreach (PlanetaryObject planet in solarsystem.Children)
                        {
                            spriteBatch.DrawRectangle(GetPlanetBox(planet), Color.Green);
                            Circle c = GetPlanetCircle(planet);
                            spriteBatch.DrawCircle(c.Center, c.Radius, (int)c.Radius, Color.Yellow);
                        }
                    }
                }
                spriteBatch.End();
            }
        }

        private void DrawPlanets(SpriteBatch spriteBatch)
        {
            //Draw planets
            foreach (Galaxy galaxy in Systems)
                foreach (SolarSystem solarsystem in galaxy.Children)
                    foreach (PlanetaryObject planet in solarsystem.Children)
                        DrawPlanet(spriteBatch, planet);
            spriteBatch.End();
        }

        private void DrawRings(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Camera.GetViewMatrix(Vector2.One));
            if (Camera.Zoom > .03f)
            {
                //Draw Rings
                foreach (Galaxy galaxy in Systems)
                {
                    //spriteBatch.DrawCircle(galaxy.Position, (int)galaxy.Diameter, (int)(galaxy.Diameter / 1.2f), new Color(60,60,60,60), 10);
                    foreach (SolarSystem solarsystem in galaxy.Children)
                    {
                        //spriteBatch.DrawCircle(solarsystem.Position, (int)solarsystem.Diameter, (int)(solarsystem.Diameter / 1.2f), new Color(100,100,100,100), 10);
                        foreach (PlanetaryObject planet in solarsystem.Children)
                        {
                            DrawPlanetRing(spriteBatch, planet);
                        }
                    }
                }
            }
        }

        private void DrawStars(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, Camera.GetViewMatrix(Vector2.One));
            //Draw planets
            foreach (Galaxy galaxy in Systems)
                foreach (SolarSystem solarsystem in galaxy.Children)
                    foreach (PlanetaryObject planet in solarsystem.Children)
                        if (planet.Type == PlanetaryType.Sun)
                            DrawPlanet(spriteBatch, planet);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, null, null, null, Camera.GetViewMatrix(Vector2.One));
            //Draw planets
            foreach (Galaxy galaxy in Systems)
                foreach (SolarSystem solarsystem in galaxy.Children)
                    foreach (PlanetaryObject planet in solarsystem.Children)
                        if (planet.Type == PlanetaryType.Sun)
                            DrawPlanet(spriteBatch, planet);
            spriteBatch.End();
        }

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            spriteBatch.Draw(game.BackgroundImage, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), new Rectangle((int)((Camera.Position.X * BackgroundSpeed) % 1920), (int)((Camera.Position.Y * BackgroundSpeed) % 1080), game.BackgroundImage.Width, game.BackgroundImage.Height), Color.White);
            //spriteBatch.Draw(Shadow, new Rectangle(0, 0, game.Graphics.PreferredBackBufferWidth, game.Graphics.PreferredBackBufferHeight), Color.White * backAlpha);
            spriteBatch.End();
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (!Full)
                return;
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Galaxy galaxy in Systems)
                foreach (SolarSystem solarsystem in galaxy.Children)
                    foreach (PlanetaryObject planet in solarsystem.Children)
                        planet.Angle += (float)gameTime.ElapsedGameTime.TotalSeconds / 3;

            Camera.Position = Vector2.Lerp(Camera.Position, CameraPointTo, CameraLerpSpeed * (elapsed * 60));

            FadeText(elapsed);
            if (!Game.IsMouseOnControl)
            {
                Zoom();
                if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                    foreach (Galaxy galaxy in Systems)
                        foreach (SolarSystem solarsystem in galaxy.Children)
                            foreach (PlanetaryObject planet in solarsystem.Children)
                                if (GetPlanetCircle(planet).Contains(mouseState.GetPositionPoint()))
                                    ClickPlanet(planet);
            }

            HandleInput(gameTime);

            newZoom = MathHelper.Clamp(newZoom, MinZoom, MaxZoom);
            Camera.Zoom = MathHelper.Lerp(Camera.Zoom, newZoom, 0.1f *(elapsed * 60));
            Game.MainWindow.tbrZoom.Value = (int)(newZoom * 100);
            //Clamp Camera
            if (CameraPointTo.X + Camera.Origin.X > TotalSize * apart || CameraPointTo.X + Camera.Origin.X < 0 || CameraPointTo.Y + Camera.Origin.Y > TotalSize * apart || CameraPointTo.Y + Camera.Origin.Y < 0)
            {
                CameraPointTo = new Vector2(MathHelper.Clamp(CameraPointTo.X, 0, TotalSize * apart), MathHelper.Clamp(CameraPointTo.Y, 0, TotalSize * apart));
            }
        }

        private void HandleInput(GameTime gameTime)
        {
            if (!Game.IsMouseOnControl)
            {
                KeyboardState ks = Keyboard.GetState();
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (ks.IsKeyDown(Game.Controls["Left"]))
                    CameraPointTo.X -= ((CameraMoveSpeed * 8) * elapsed) / Camera.Zoom;
                else if (ks.IsKeyDown(Game.Controls["Right"]))
                    CameraPointTo.X += ((CameraMoveSpeed * 8) * elapsed) / Camera.Zoom;

                if (ks.IsKeyDown(Game.Controls["Up"]))
                    CameraPointTo.Y -= ((CameraMoveSpeed * 8) * elapsed) / Camera.Zoom;
                else if (ks.IsKeyDown(Game.Controls["Down"]))
                    CameraPointTo.Y += ((CameraMoveSpeed * 8) * elapsed) / Camera.Zoom;

                if (ks.IsKeyDown(Game.Controls["Zoom Out"]))
                    newZoom -= elapsed;
                else if (ks.IsKeyDown(Game.Controls["Zoom In"]))
                    newZoom += elapsed;
            }
        }

        private void ClickPlanet(PlanetaryObject planet)
        {
            MainWindow wnd = game.GetWnd();
            wnd.PlanetName.Text = planet.Name;
            SelectedPlanet = planet;

            //Zoom to sun if clicked
            if (planet.Type == PlanetaryType.Sun && Camera.Zoom < .2f)
            {
                newZoom = .25f;
                CameraPointTo = SelectedPlanet.Position - new Vector2(((Game.MainWindow.ClientWidth) / 2) - SelectedPlanet.Radius * Camera.Zoom, ((Game.MainWindow.ClientHeight) / 2) - SelectedPlanet.Radius * Camera.Zoom);
            }
            if (planet.Type == PlanetaryType.Planet && Camera.Zoom < .28f)
            {
                newZoom = .6f;
                CameraPointTo = SelectedPlanet.Position - new Vector2(((Game.MainWindow.ClientWidth) / 2) - SelectedPlanet.Radius * Camera.Zoom, ((Game.MainWindow.ClientHeight) / 2) - SelectedPlanet.Radius * Camera.Zoom);
            }

            MainWindow.btnUniverse[5].Enabled = Bookmarks.Contains(SelectedPlanet);
            MainWindow.btnUniverse[5].Glyph.Color = MainWindow.btnUniverse[5].Enabled ? Color.White : Color.Gray;
            MainWindow.btnUniverse[2].Enabled = !Bookmarks.Contains(SelectedPlanet);
            MainWindow.btnUniverse[2].Glyph.Color = MainWindow.btnUniverse[2].Enabled ? Color.White : Color.Gray;

            for (int i = 0; i < MainWindow.universeStatLabels.Count(); i++)
            {
                MainWindow.universeStatLabels[i].Text = string.Empty;
                MainWindow.universeStatLabels[i].TextColor = Color.White;
            }

            if (planet.Type == PlanetaryType.Sun)
            {
                MainWindow.btnLand.Enabled = MainWindow.btnUniverse[4].Enabled = false;
                wnd.PlanetName.Text = planet.Name;
                wnd.PlanetDesc.Text = planet.Description;

                MainWindow.universeStatLabels[0].Text = "Coordinates: " + Math.Round(planet.Position.X) + " " + Math.Round(planet.Position.Y);
                MainWindow.universeStatLabels[1].Text = "Objects:";
                for (int i = 1; i < MathHelper.Clamp(planet.Parent.Children.Count(), 0, MainWindow.universeStatLabels.Count() - 1); i++)
                {
                    MainWindow.universeStatLabels[i + 1].Text = "  -" + planet.Parent.Children[i].Name;
                    MainWindow.universeStatLabels[i + 1].TextColor = planet.Parent.Children[i].Color;
                    PlanetaryObject p = planet.Parent.Children[i];
                }
            }
            else if (planet.Type == PlanetaryType.Planet)
            {
                MainWindow.btnLand.Enabled = MainWindow.btnUniverse[4].Enabled = true;
                wnd.PlanetName.Text = planet.Name;
                wnd.PlanetDesc.Text = planet.Description;

                MainWindow.universeStatLabels[0].Text = "Size: " + Game.GetPlanetWidth(planet) + " feet wide by " + Game.GetPlanetHeight(planet) + " feet thick";
                MainWindow.universeStatLabels[1].Text = "Coordinates: " + Math.Round(planet.Position.X) + " " + Math.Round(planet.Position.Y);
                MainWindow.universeStatLabels[2].Text = "Seed: " + planet.Seed;
                MainWindow.universeStatLabels[3].Text = "Gravity: " + Math.Round(planet.Gravity,1);
            }
            if (planet == HomePlanet)
                wnd.PlanetDesc.Text += " - Home";
        }

        private void Zoom()
        {
            if (!Game.IsMouseOnControl)
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    CameraPointTo -= (new Vector2(mouseState.X, mouseState.Y) - new Vector2(lastMouseState.X, lastMouseState.Y)) * (1f / Camera.Zoom);

                }
            }
            if (mouseState.ScrollWheelValue != lastMouseState.ScrollWheelValue)
            {
                newZoom += ((mouseState.ScrollWheelValue - lastMouseState.ScrollWheelValue)) / (2200f);
            }
        }

        private void FadeText(float elapsed)
        {
            //Fade names
            if (newZoom <= nameShowThresh)
            {
                nameFadingOut = true;
                nameFadingIn = false;
            }
            if (newZoom > nameShowThresh)
            {
                nameFadingIn = true;
                nameFadingOut = false;
            }
            if (nameFadingIn)
            {
                nameAlpha = MathHelper.Lerp(nameAlpha, 1.1f, 2f * elapsed);
                if (nameAlpha >= 1)
                {
                    nameFadingIn = false;
                    nameAlpha = 1;
                }
            }
            if (nameFadingOut)
            {
                nameAlpha = MathHelper.Lerp(nameAlpha, -.1f, 2f * elapsed);
                if (nameAlpha <= 0)
                {
                    nameFadingOut = false;
                    nameAlpha = 0;
                }
            }
            //Fade Descriptions
            if (newZoom <= descShowThresh)
            {
                descFadingOut = true;
                descFadingIn = false;
            }
            if (newZoom > descShowThresh)
            {
                descFadingIn = true;
                descFadingOut = false;
            }
            if (descFadingIn)
            {
                descAlpha = MathHelper.Lerp(descAlpha, 1.1f, 2f * elapsed);
                if (descAlpha >= 1)
                {
                    descFadingIn = false;
                    descAlpha = 1;
                }
            }
            if (descFadingOut)
            {
                descAlpha = MathHelper.Lerp(descAlpha, -.1f, 2f * elapsed);
                if (descAlpha <= 0)
                {
                    descFadingOut = false;
                    descAlpha = 0;
                }
            }
        }

        public void DrawPlanet(SpriteBatch spriteBatch, PlanetaryObject planet)
        {
            Rectangle box = GetPlanetBox(planet);
            //Add room for text
            box.Y -= 72;
            box.Height += 72;
            if (game.Window.ClientBounds.Intersects(box))
            {
                if (planet.Type == PlanetaryType.Planet)
                {
                    Vector2 Origin = GetOrigin(planet); 
                    Vector2 SunOrigin = GetOrigin(planet.Parent.Children[0]);

                    //Draw planet
                    spriteBatch.Draw(PlanetTexture, planet.Position + new Vector2(planet.Radius, planet.Radius), null, planet.Color, (float)Math.Atan2(planet.Parent.Children[0].Position.Y - (planet.Position.Y), planet.Parent.Children[0].Position.X - (planet.Position.X)) + MathHelper.PiOver2, new Vector2(planet.Radius / (planet.Diameter / (float)PlanetTexture.Width), planet.Radius / (planet.Diameter / (float)PlanetTexture.Width)), planet.Diameter / (float)PlanetTexture.Width, SpriteEffects.None, 0);

                    if (nameAlpha > 0)
                    {
                        Vector2 TextPosition = new Vector2((((planet.Position.X) + (PlanetTexture.Width * (planet.Diameter / (float)PlanetTexture.Width)) / 2)) - (game.Manager.Skin.Fonts["Default32"].Resource.MeasureString(planet.Name).X / 2), planet.Position.Y - 72);
                        spriteBatch.DrawString(game.Manager.Skin.Fonts["Default32"].Resource, planet.Name, TextPosition, (Color.White * nameAlpha));
                    }
                    if (descAlpha > 0)
                    {
                        Vector2 TextPosition = new Vector2((((planet.Position.X) + (PlanetTexture.Width * (planet.Diameter / (float)PlanetTexture.Width)) / 2)) - (game.Manager.Skin.Fonts["Default12"].Resource.MeasureString(planet.Description).X / 2), planet.Position.Y - 26);
                        spriteBatch.DrawString(game.Manager.Skin.Fonts["Default12"].Resource, planet.Description, TextPosition, (Color.LightGray * descAlpha));
                    }
                }
                else if (planet.Type == PlanetaryType.Sun)
                {
                    spriteBatch.Draw(SunTexture, planet.Position, null, planet.Color, 0, Vector2.Zero, planet.Diameter / (float)SunTexture.Width, SpriteEffects.None, 0);
                }
            }
        }
        public void DrawPlanetRing(SpriteBatch spriteBatch, PlanetaryObject planet)
        {
            if (planet.Type == PlanetaryType.Planet)
            {
                Vector2 Origin = GetOrigin(planet);
                Vector2 SunOrigin = GetOrigin(planet.Parent.Children[0]);
                if (game.Window.ClientBounds.Intersects(GetPlanetRingBox(planet, Origin, SunOrigin)))
                    spriteBatch.DrawCircle(SunOrigin, Vector2.Distance(Origin, SunOrigin), (int)Vector2.Distance(Origin, SunOrigin) / (Camera.Zoom > .5f ? 11 : Camera.Zoom > .25f ? 17 : Camera.Zoom > .075f ? 21 : 55), new Color(20, 20, 20, 20), 5);
            }
            else if (planet.Type == PlanetaryType.Sun)
            {
            }
        }
        private Rectangle GetPlanetRingBox(PlanetaryObject planet, Vector2 Origin, Vector2 SunOrigin)
        {
            Vector2 Position = Camera.WorldToScreen(SunOrigin);
            int Radius = (int)Vector2.Distance(Origin, SunOrigin);

            return new Rectangle((int)Position.X - Radius, (int)Position.Y - Radius, Radius * 2, Radius * 2);
        }
        private Circle GetPlanetCircle(PlanetaryObject planet)
        {
            Vector2 Position = Camera.WorldToScreen(planet.Position);
            switch (planet.Type)
            {
                case PlanetaryType.Planet:
                    {
                        float Radius = ((PlanetTexture.Width * (planet.Diameter / (float)PlanetTexture.Width)) * Camera.Zoom) / 2f;
                        return new Circle(Position + new Vector2(Radius, Radius), Radius);
                    }
                case PlanetaryType.Sun:
                    {
                        float Radius = ((SunTexture.Width * (planet.Diameter / (float)SunTexture.Width)) * Camera.Zoom) / 2f;
                        return new Circle(Position + new Vector2(Radius, Radius), Radius);
                    }
            }
            return Circle.Empty;
        }
        private Rectangle GetPlanetBox(PlanetaryObject planet)
        {
            Vector2 Position = Camera.WorldToScreen(planet.Position);
            switch (planet.Type)
            {
                case PlanetaryType.Planet:
                    {
                        return new Rectangle((int)Position.X,(int)Position.Y,(int)(((PlanetTexture.Width * (planet.Diameter / (float)PlanetTexture.Width))) * Camera.Zoom),(int)(((PlanetTexture.Height * (planet.Diameter / (float)PlanetTexture.Height)) * Camera.Zoom)));
                    }
                case PlanetaryType.Sun:
                    {
                        return new Rectangle((int)Position.X, (int)Position.Y, (int)(((SunTexture.Width * (planet.Diameter / (float)SunTexture.Width))) * Camera.Zoom), (int)(((SunTexture.Height * (planet.Diameter / (float)SunTexture.Height)) * Camera.Zoom)));
                    }
            }
            return Rectangle.Empty;
        }
        public static Vector2 GetOrigin(PlanetaryObject planet)
        {
            return planet.Position + new Vector2(planet.Radius, planet.Radius);
        }
        public void DrawSidebar(SideBar sideBar, DrawEventArgs e)
        {
            if (SelectedPlanet != null)
            {
                if (SelectedPlanet.Type == PlanetaryType.Planet)
                {
                    //Calculate Position
                    int Width = (int)((sideBar.Width - 8) * (float)(SelectedPlanet.Diameter / PlanetTexture.Width));
                    int Height = (int)((sideBar.Width - 8) * (float)(SelectedPlanet.Diameter / PlanetTexture.Height));
                    //Draw Planet on sidebar
                    e.Renderer.Draw(PlanetTexture, new Rectangle(e.Rectangle.Left + (sideBar.Width / 2) - (Width / 2),
                        e.Rectangle.Top + (sideBar.Width / 2) - (Height / 2),
                        Width,
                        Height),
                        SelectedPlanet.Color
                    );
                }
                else if (SelectedPlanet.Type == PlanetaryType.Sun)
                {
                    int Width = (int)((sideBar.Width - 8) * (float)(SelectedPlanet.Diameter / SunTexture.Width));
                    int Height = (int)((sideBar.Width - 8) * (float)(SelectedPlanet.Diameter / SunTexture.Height));
                    //First Alpha-Blend pass
                    e.Renderer.Draw(SunTexture, new Rectangle(e.Rectangle.Left + (sideBar.Width / 2) - (Width / 2),
                        e.Rectangle.Top + (sideBar.Width / 2) - (Height / 2),
                        Width,
                        Height),
                        SelectedPlanet.Color);
                    e.Renderer.SpriteBatch.End();
                    //Additive Pass
                    e.Renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                    e.Renderer.Draw(SunTexture, new Rectangle(e.Rectangle.Left + (sideBar.Width / 2) - (Width / 2),
                        e.Rectangle.Top + (sideBar.Width / 2) - (Height / 2),
                        Width,
                        Height),
                        SelectedPlanet.Color);
                    e.Renderer.SpriteBatch.End();
                    e.Renderer.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }
            }
        }
        public void Dispose()
        {
            game.BackgroundImageColor = Color.White;
        }
        #endregion

    }
}
