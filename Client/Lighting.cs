using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ZarknorthClient.Entities;

namespace ZarknorthClient
{
    public partial class Level
    {
        public Color SkyColor = Color.Lerp(Color.White, Color.Red, 0f);
        public Color Sky { get; private set; }
        public Color LastSky { get; private set; }

        private int CurLightR;
        private int CurLightG;
        private int CurLightB;

        public RenderTarget2D LightingTarget;
        private Effect BlurEffect;

        /// <summary>
        /// Offset around the window to calculate lighting
        /// </summary>
        private const int OffSet = 15;
        /// <summary>
        /// The minimum time needed (in seconds) before re-calulating lighting
        /// </summary>
        private double MinWait { get { return (Game.LightingRefresh == 0 ? 10 : Game.LightingRefresh == 1 ? 7 : 5) / 100f; } }
        private double LastCalc;
        /// <summary>
        /// True if the lighting must be recalculated the next frame.
        /// </summary>
        private bool ForceRecalcLighting = false;


        public void CalculateLighting(GameTime gameTime, bool EmergencyForce = false)
        {
            if (ComputeLighting)
            {
                if (gameTime.TotalGameTime.TotalSeconds > LastCalc + MinWait || ForceRecalcLighting || EmergencyForce)
                {
                    if (!FullBright)
                    {
                        //Reset tiles to their default lighting values
                        ResetLights();
                        CalcLights();
                        //If lighting quality is medium or above, do dual pass lighting
                        if (Game.LightingQuality >= 1)
                            CalcLights();
                    }
                    CalculateNoDrawSpots();
                    LastCalc = gameTime.TotalGameTime.TotalSeconds;
                    ForceRecalcLighting = false;
                    ComputeLighting = false;
                }
            }
        }


        /// <summary>
        /// Calculates the color of the ambient blocks, such as blank blocks, and the light they should emit during day/night
        /// </summary>
        /// <param name="Time">Time ranging from 0 to 1</param>
        private void CalculateSkyLight(float Time)
        {
            LastSky = Item.Blank.Light;
            if (isDay) //Set the light of tiles based on time
            {
                if (Time < .25f) //If before half of day go up
                {
                    float light = (float)(Time * 8 + .2f);

                    light = MathHelper.Clamp(light, .3f, 1f);
                    Sky = new Color((light * StormMultiplyer) * (SkyColor.R / 255f),
                        (light * StormMultiplyer) * (SkyColor.G / 255f),
                        (light * StormMultiplyer) * (SkyColor.B / 255f));
                    starColorMultiplyer = MathHelper.Clamp(1f - ((Time) / .07f), 0, 1);
                }
                if (Time > .25f) //If after, Start going down
                {
                    float light = 4f - (Time * 8) + .2f;
                    light = MathHelper.Clamp(light, .3f, 1f);
                    Sky = new Color((light * StormMultiplyer) * (SkyColor.R / 255f),
                        (light * StormMultiplyer) * (SkyColor.G / 255f),
                        (light * StormMultiplyer) * (SkyColor.B / 255f));
                    starColorMultiplyer = MathHelper.Clamp(((Time) - .43f) / .07f, 0, 1);
                }
            }
            else //If Night
            {
                Sky = new Color(.3f * StormMultiplyer, .3f * StormMultiplyer, .3f * StormMultiplyer);
            }
            if (Math.Abs(Sky.R - LastSky.R) > 5 || ForceRecalcLighting)
                ComputeLighting = true;
        }

        /// <summary>
        /// Reset all tiles to their default values for the next frame
        /// </summary>
        private void ResetLights()
        {
            int Left = MainCamera.Left - OffSet;
            int Right = MainCamera.Right + OffSet;
            int Top = (int)MathHelper.Max(MainCamera.Top - OffSet, 1);
            int Bottom = (int)MathHelper.Min(MainCamera.Bottom + OffSet, Height - 1);

            for (int x = Left; x < Right; ++x)
            {
                for (int y = Top; y < Bottom; ++y)
                {
                    Tile t = tiles[x, y];
                    Tile tt = tiles[x, y,true];
                    if (tt.Background == Item.Blank && tt.Foreground == Item.Blank && !tt.FullBackground)
                    {
                        tt.Light = Sky;
                    }
                    else if (tt.Background.SkyLight && t.Foreground.SkyLight && tt.FullBackground)
                    {
                        tt.Light = Color.Black;
                    }
                    else if (tt.Background.SkyLight && t.Foreground.ID == Item.Blank.ID)
                    {
                        tt.Light = Sky;
                    }
                    else if (t.Foreground.SkyLight && t.Background.ID == Item.Blank.ID)
                    {
                        tt.Light = Sky;
                    }
                    else if (t.Foreground.ElectronicLight)
                    {
                        if ((t as ElectronicTile).Inputs[0].Any(x1 => x1.Powered))
                            tt.Light = t.Foreground.Light;
                        else
                            tt.Light = tt.Background.ID == Item.Blank.ID ? Sky : Color.Black;
                    }
                    else if (tt.Background.ID != Item.Blank.ID && t.Foreground.ID == Item.Blank.ID)
                    {
                        tt.Light = Color.Black;
                    }
                    else if (!t.Foreground.ElectronicLight)
                    {
                        tt.Light = t.Foreground.Light;
                    }

                    //Liquid colors
                    if (tiles[x, y, true].LavaMass > 32)
                    {
                        tt.Light = (tiles[x, y, true].LavaMass > 0) ? Item.Lava.Light : t.Light;
                    }
                }
            }
            if (Interface.MainWindow.inventory != null && (Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item is BlockItem) && (Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item as BlockItem).LightHand)
            {
                tiles[(int)(Players[0].Position.X / Tile.Width), (int)(Players[0].Position.Y / Tile.Height) - 1, true].Light = (Players[0].Inventory[Interface.MainWindow.inventory.Selected].Item as BlockItem).Light;
            }
            else if (Gamemode == GameMode.Sandbox)
                tiles[(int)(Players[0].Position.X / Tile.Width), (int)(Players[0].Position.Y / Tile.Height) - 1, true].Light = Color.White;
            if (Players[0].OnFire)
                tiles[(int)(Players[0].Position.X / Tile.Width), (int)(Players[0].Position.Y / Tile.Height) - 1, true].Light = Color.Orange;
            Particle p;
            for (int i = 0; i < AdditiveParticleEngine.Particles.Count; i++)
            {
                p = AdditiveParticleEngine.Particles[i];
                if (p.EmitColor && p.Position.X / Tile.Width > MainCamera.Left - 12 && p.Position.X / Tile.Width < MainCamera.Right + 12 && p.Position.Y / Tile.Height > MainCamera.Top - 12 && p.Position.Y / Tile.Height < MainCamera.Bottom + 12)
                {
                    tiles[(int)(p.Position.X / Tile.Width), (int)(p.Position.Y / Tile.Height), true].Light = p.BaseColor;
                }
            }
            for (int i = 0; i < DefaultParticleEngine.Particles.Count; i++)
            {
                p = DefaultParticleEngine.Particles[i];
                if (p.EmitColor && p.Position.X / Tile.Width > MainCamera.Left - 12 && p.Position.X / Tile.Width < MainCamera.Right + 12 && p.Position.Y / Tile.Height > MainCamera.Top - 12 && p.Position.Y / Tile.Height < MainCamera.Bottom + 12)
                {
                    tiles[(int)(p.Position.X / Tile.Width), (int)(p.Position.Y / Tile.Height), true].Light = p.BaseColor;
                }
            }
            foreach (PhysicsEntity pe in PhysicsEntities)
            {
                if (pe.Item.EmitColor && pe.Position.X / Tile.Width > MainCamera.Left - 12 && pe.Position.X / Tile.Width < MainCamera.Right + 12 && pe.Position.Y / Tile.Height > MainCamera.Top - 12 && pe.Position.Y / Tile.Height < MainCamera.Bottom + 12)
                {
                    tiles[(int)(pe.Position.X / Tile.Width), (int)(pe.Position.Y / Tile.Height), true].Light = pe.Item.Color;
                }
            }
        }
        Tile getTile;
        /// <summary>
        /// Will calculate the light value of a tile
        /// </summary>
        /// <param name="x">X tile</param>
        /// <param name="y">Y tile</param>
        private void LightTile(int x, int y)
        {
            getTile = tiles[x, y, true];

            if (getTile.Light.R > CurLightR) //If this tile is brighter than the last
                CurLightR = getTile.Light.R;
            else
                getTile.Light.R = (byte)CurLightR;
            CurLightR -= getTile.Foreground.Absorb; //Start dimming

            if (getTile.Light.B > CurLightB) //If this tile is bighter than the last
                CurLightB = getTile.Light.B;
            else
                getTile.Light.B = (byte)CurLightB;
            CurLightB -= getTile.Foreground.Absorb; //Start dimming


            if (getTile.Light.G > CurLightG) //If this tile is brighter than the last
                CurLightG = getTile.Light.G;
            else
                getTile.Light.G = (byte)CurLightG;
            CurLightG -= getTile.Foreground.Absorb; //Start dimming
        }
        /// <summary>
        /// Method to pass lights 4 times 
        /// </summary>
        private void CalcLights()
        {
            int Left = MainCamera.Left - OffSet;
            int Right = MainCamera.Right + OffSet;
            int Top = (int)MathHelper.Max(MainCamera.Top - OffSet, 2);
            int Bottom = (int)MathHelper.Min(MainCamera.Bottom + OffSet, Height - 2);

            int LeftAbs = MainCamera.Left;
            int RightAbs = MainCamera.Right;
            int TopAbs = (int)MathHelper.Max(MainCamera.Top, Tile.Height * 2);
            int BottomAbs = (int)MathHelper.Min(MainCamera.Bottom, Height - (Tile.Height * 2));

            for (int x = Left; x < Right; ++x) //MainCamera.Top to MainCamera.Bottom |
            {                                  //                                    V
                CurLightB = CurLightG = CurLightR = -1;
                for (int y = Top; y < Bottom; ++y)
                    LightTile(x, y);
                CurLightB = CurLightG = CurLightR = -1;
                for (int y = Bottom; y > Top; --y)
                    LightTile(x, y);
            }
            for (int y = Top; y < Bottom; ++y) //MainCamera.btnLeft to MainCamera.btnRight -->
            {
                CurLightB = CurLightG = CurLightR = -1;
                for (int x = Left; x < Right; ++x)
                    LightTile(x, y);
                CurLightB = CurLightG = CurLightR = -1;
                for (int x = Right; x > Left; --x)
                    LightTile(x, y);
            }
        }
        /// <summary>
        /// Takes the lighting values for each onscreen tile and draws them to a render target 1/24th (Tile Size) the Size of the scree, to be scaled
        /// and draw later
        /// </summary>
        private void RenderLightingToRT2D(SpriteBatch spriteBatch)
        {
            Stopwatch drawWatch = new Stopwatch();
            drawWatch.Start();

            //If the rendertarget is null, create it at the width and height of the viewport in tiles
            if (LightingTarget == null)
            {
                LightingTarget = new RenderTarget2D(game.GraphicsDevice, MainCamera.Right - MainCamera.Left, MainCamera.Bottom - MainCamera.Top, false, game.GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24);
                spriteBatch.GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                LightingBufferData = new Color[(MainCamera.Right - MainCamera.Left) * (MainCamera.Bottom - MainCamera.Top)];
            }

            //Foreach tile on screen
            for (int x = MainCamera.Left; x < MainCamera.Right; ++x)
            {
                for (int y = MainCamera.Top; y < MainCamera.Bottom; ++y)
                {
                    Tile tile = tiles[x, y, true]; //Get the tile

                    LightingBufferData[(x - MainCamera.Left) + (y - MainCamera.Top) * (MainCamera.Right - MainCamera.Left)] = tile.Light;
                }
            }

            LightingTarget.SetData<Color>(LightingBufferData);
            drawWatch.Stop();
            //if (Interface.MainWindow.DebugList != null)
            //    Interface.MainWindow.DebugList.Items[20] = "   -RenderLigtingToRT2D() " + drawWatch.ElapsedMilliseconds;

        }
        private void CalculateNoDrawSpots()
        {
            int Left = MainCamera.Left - OffSet;
            int Right = MainCamera.Right + OffSet;
            int Top = (int)MathHelper.Max(MainCamera.Top - OffSet, 2);
            int Bottom = (int)MathHelper.Min(MainCamera.Bottom + OffSet, Height - 2);
            for (int x = Left + 1; x < Right; ++x)
            {
                for (int y = Top + 1; y < Bottom; ++y)
                {
                    if (!FullBright)
                    {
                        bool light = false;
                        if (!tiles[x + 2, y, true].Light.Equals(Color.Black))
                            light = true;
                        else if (!tiles[x - 2, y, true].Light.Equals(Color.Black))
                            light = true;
                        else if (!tiles[x, y + 2, true].Light.Equals(Color.Black))
                            light = true;
                        else if (!tiles[x, y - 2, true].Light.Equals(Color.Black))
                            light = true;
                        tiles[x, y].NoDraw = !light;
                    }
                    else
                        tiles[x, y].NoDraw = false;
                }
            }
        }
    }
}
