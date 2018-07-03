
#region Using

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
#endregion

namespace ZarknorthClient.Interface
{
  /// <summary>
  /// Logical class for UI, Handles button clicks, window closes, etc
  /// </summary>
  public partial class MainWindow
  {
    #region Variables
    //State of the inventory window
    public static bool expanding;
    public static bool hiding;
    public static bool expanded;
    public static bool isDragging;
    public static bool closingtb;
    //Stats
    private int afps = 0;
    private int fps = 0;
    private double et = 0;
    public static long Frames = 0;
    public float absMapPanelWidth;
    #endregion

    #region Methods
    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        MouseState m = Mouse.GetState();
        if (Game.CurrentGameState == Game.GameState.InGame)
        {
            if (sandBoxFadingIn)
            {
                if (MapPanel.Width < 202)
                    absMapPanelWidth = MathHelper.Lerp(absMapPanelWidth, 202, elapsed * 10);
                MapPanel.Width = (int)absMapPanelWidth;
                MapPanel.Left = InventoryPanel.Left - MapPanel.Width - 8;
            }
            if (sandBoxFadingOut)
            {
                if (MapPanel.Width > 0)
                    absMapPanelWidth = MathHelper.Lerp(absMapPanelWidth, 0, elapsed * 10);
                MapPanel.Width = (int)absMapPanelWidth;
                MapPanel.Left = InventoryPanel.Left - MapPanel.Width - 8;
            }
            if (expanded)
            {
                //Drop items
                if ((Game.mouseState.LeftButton == ButtonState.Pressed || Game.mouseState.RightButton == ButtonState.Pressed) && !inventory.AbsoluteRect.Contains(new Point(Game.mouseState.X, Game.mouseState.Y)))
                {
                    if (Interface.MainWindow.mouseSlot.Item.ID != Item.Blank.ID && Game.level.InLevelBounds(new Vector2((int)(Game.level.MainCamera.Position.X + Game.mouseState.X), (int)(Game.level.MainCamera.Position.Y + Game.mouseState.Y))))
                    {
                        if (Game.level.tiles[(int)(Game.level.MainCamera.Position.X + Game.mouseState.X) / Tile.Width, (int)(Game.level.MainCamera.Position.Y + Game.mouseState.Y) / Tile.Height].Foreground.Collision == BlockCollision.Passable)
                        {
                            if (Game.mouseState.LeftButton == ButtonState.Pressed)
                            {
                                Game.level.SpawnCollectableAbsolutePos(Interface.MainWindow.mouseSlot, new Vector2(0, 0), new Point((int)(Game.level.MainCamera.Position.X + Game.mouseState.X), (int)(Game.level.MainCamera.Position.Y + Game.mouseState.Y)));
                                Interface.MainWindow.mouseSlot = new Slot(Item.Blank);
                            }
                            else if (Game.mouseState.RightButton == ButtonState.Pressed && Game.oldMouseState.RightButton == ButtonState.Released)
                            {
                                Game.level.SpawnCollectableAbsolutePos(new Slot(Interface.MainWindow.mouseSlot.Item, 1), new Vector2(0, 0), new Point((int)(Game.level.MainCamera.Position.X + Game.mouseState.X), (int)(Game.level.MainCamera.Position.Y + Game.mouseState.Y)));
                                Interface.MainWindow.mouseSlot.Sub();
                            }
                        }
                    }
                }
            }
        }
        else if (Game.CurrentGameState == Game.GameState.HomeLoggedOff)
        {
            if (InfoBanner != null && InfoBanner.Alpha < 255 && InfoBanner.Visible)
            {
                InfoBanner.Alpha += (float)gameTime.ElapsedGameTime.TotalSeconds * 255;
            }
        }
        else if (Game.CurrentGameState == Game.GameState.HomeLoggedOn)
        {
            if (tb != null && closingtb && tb.Alpha > 0)
            {
                tb.Alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 150;
                tb.Passive = true;
                if (tb.Alpha < 1f)
                    Manager.Remove(tb);
            }
        }
    }

    public void UpdateStats(GameTime gameTime, float elapsed, MouseState mouseState)
    {
        if (((Interface.MainWindow.expanded && !Interface.MainWindow.hiding) || (ZarknorthClient.Interface.MainWindow.CraftingWindow != null && ZarknorthClient.Interface.MainWindow.CraftingWindow.Visible)))
        {
            mouseImage.Left  = mouseState.X;
            mouseImage.Top  = mouseState.Y;
            mouseLabel.Top = mouseImage.Top + 26;
            mouseLabel.Left = mouseImage.Left;
            mouseImage.BringToFront();
            mouseLabel.BringToFront();
        }
        else if (InventoryPanel != null && !expanded)
        {
            //Off screen
            mouseImage.Left = mouseLabel.Left = -100;
            mouseImage.Top = mouseLabel.Top = -100;
        }
        if (Game.level != null && Game.level.ready == true && prgHealth != null)
        {
            //Set health/money, etc
            prgHealth.Value = (int)Math.Floor(Game.level.Players[0].Health);
            prgEnergy.Value = (int)Math.Floor(Game.level.Players[0].Energy);
            prgHealth.Color = Extensions.GetBlendedColor((int)prgHealth.Value);
            prgEnergy.Color = Extensions.GetBlendedColorAlt((int)prgEnergy.Value);
            prgHealth.ToolTip.Text = "Health: " + prgHealth.Value + " / 100";
            prgEnergy.ToolTip.Text = "Energy: " + prgEnergy.Value + " / 100";
            lblMoney.Text = string.Format("${0:n0}", Game.level.Players[0].Money);
            lblMoney.ToolTip.Text = imgMoney.ToolTip.Text = "You have " + lblMoney.Text;

            //Set minimap texture
            if (Game.level.MapTexture != null)
            {
                imgMinimap.Image = Game.level.MapTexture;
                imgMinimap.Width = Game.level.MapTexture.Width;
                imgMinimap.Height = Game.level.MapTexture.Height;
            }
            if (expanding == true)
            {
                if (Math.Round(InventoryHeight) >= InventoryOpenHeight)
                {
                    expanded = true;
                    expanding = false;
                    InventoryPanel.Height = InventoryOpenHeight;
                    inventory.Height = InventoryOpenHeight - 16;
                }
                else
                {
                    float amount = MathHelper.Clamp(elapsed * 10, 0, 1);
                    InventoryHeight = MathHelper.Lerp((float)InventoryHeight, InventoryOpenHeight, amount);
                    InventoryPanel.Height = (int)InventoryHeight;
                    inventory.Height = (int)InventoryHeight - 16;
                    foreach (SlotControl s in inventory.Slots)
                        if (s.AbsoluteTop < InventoryPanel.Top + InventoryPanel.Height)
                            s.Visible = true;
                }
            }
            if (hiding == true)
            {
                if (Math.Round(InventoryHeight) <= InventoryClosedHeight)
                {
                    hiding = false;
                    expanded = false;
                    inventory.Height = (int)InventoryClosedHeight - 16;
                    InventoryPanel.Height = (int)InventoryClosedHeight;
                }
                else
                {
                    float amount = MathHelper.Clamp(elapsed * 10, 0, 1);
                    InventoryHeight = MathHelper.Lerp((float)InventoryHeight, InventoryClosedHeight, amount);
                    inventory.Height = (int)InventoryHeight - 16;
                    InventoryPanel.Height = (int)InventoryHeight;
                    if (mouseSlot != null && mouseSlot.Item != Item.Blank)
                    {
                        mouseSlot = new Slot(Item.Blank);
                        isDragging = false;
                    }
                    foreach (SlotControl s in inventory.Slots)
                        if (s.AbsoluteTop > InventoryPanel.Top + InventoryPanel.Height)
                            s.Visible = false;

                }
            }
            if (mouseSlot.Stack > 0)
                mouseLabel.Text = mouseSlot.Stack.ToString();
            else
                mouseLabel.Text = "";

            if (mouseSlot.Item == Item.Blank)
                mouseImage.Visible = false;
            else
            {
                mouseImage.Image = ContentPack.Textures["items\\" + mouseSlot.Item.Name];
                mouseImage.Visible = true;
            }
            int x = (int)(Math.Floor((float)mouseState.X + (int)Game.level.MainCamera.Position.X) / Tile.Width);
            int y = (int)((mouseState.Y + (int)Game.level.MainCamera.Position.Y) / Tile.Height);

            if (Console != null && MainTabs.SelectedIndex == 1)
            {
                //string active = "X";
                //if (Game.level.liquid.ActiveTiles.Contains(new Point(x, y)))
                //    active = "A";
                if (Game.level.InLevelBounds(x, y))
                {
                    StatList[0].Text = "Item: (" + x + "," + y + ") " + Game.level.tiles[x, y].Foreground.Name;
                    StatList[1].Text = "FG FireMeta: " + Game.level.tiles[x, y].ForegroundFireMeta;
                    StatList[2].Text = "BG FireMeta: " + Game.level.tiles[x, y].BackgroundFireMeta;
                    //DebugList.Items[1] = Game.level.tiles[x, y].Foreground.Name;
                    //DebugList.Items[2] = "Draw? : " + !Game.level.tiles[x, y].NoDraw;
                    //DebugList.Items[3] = "   -Background: " + Game.level.tiles[x, y].Background.Name;
                    //DebugList.Items[4] = "Player: Abs(" + Game.level.Players[0].Position.X + "," + Game.level.Players[0].Position.Y + ") " + "Grid(" + (int)(Game.level.Players[0].Position.X / Tile.Width) + "," + (int)(Game.level.Players[0].Position.Y / Tile.Height) + ") ";
                    //DebugList.Items[5] = "   -Admin Mode: " + Game.level.Players[0].IsAdminMode;
                    //DebugList.Items[6] = "   -Health/Energy: " + Game.level.Players[0].Health + " / " + Game.level.Players[0].Energy;
                    //DebugList.Items[9] = "Entities: -Plr: " + Game.level.Players.Count + " -Ncp: " + Game.level.NCPs.Count;
                    //DebugList.Items[10] = "Particles: " + Game.level.DefaultParticleEngine.Particles.Count;
                    //DebugList.Items[11] = "Liquids: " + Game.level.liquid.ActiveTiles.Count;
                    StatList[4].Text = "NeoForce Controls #:  " + Disposable.Count.ToString();
                    StatList[5].Text = "GC: " + GC.GetTotalMemory(false);
                    StatList[6].Text = "WaterIdle: " + Game.level.tiles[x, y].WaterIdle;
                }
            }
        }
        if (et >= 400 || et == 0)
        {
            if (Console != null)
            {
                DebugLabel.Text = string.Format("Fps: {0} (Average: {1}) - {2}", fps, afps, Game.GetVersionString());
            }
            if (gameTime.TotalGameTime.TotalSeconds != 0)
            {
                afps = (int)(Frames / gameTime.TotalGameTime.TotalSeconds);
            }
            if (gameTime.ElapsedGameTime.TotalMilliseconds != 0)
            {
                fps = (int)(1000 / gameTime.ElapsedGameTime.TotalMilliseconds);
            }
            et = 1;
        }
        et += gameTime.ElapsedGameTime.TotalMilliseconds;
    }

 
  
    /// <summary>
    /// Called when a window is being closed, you can continue to close it, or cancel the close and do something else
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void WindowClosing(object sender, WindowClosingEventArgs e)
    {
        if (sender is TaskLauncher)
        {
            InitMainScreen();
        }
        if (sender is TaskProfile && Game.CurrentGameState == Game.GameState.HomeLoggedOn)
        {
            startPlanet.Enabled = true;
            closingtb = true;
        }
    }
 
    /// <summary>
    /// Called when a window has been closed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void WindowClosed(object sender, WindowClosedEventArgs e)
    {
        (sender as Control).Focused = false;
    }

    #endregion


    public int mouseLabelOffset { get; set; }
  }
}
