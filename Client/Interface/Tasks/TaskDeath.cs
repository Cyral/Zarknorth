#region Using
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
#endregion

namespace ZarknorthClient
{
    public class TaskDeath : Dialog
    {

        #region Fields

        private ImageBox deathImage;
        private Label deathMessage, deathMessageBG;
        private Button Respawn, Quit;

        private GradientPanel tb;
        private Manager manager;
        #endregion

        #region Constructors
 
        public TaskDeath(Manager manager, string DeathMessage)
            : base(manager)
        {

            tb = new GradientPanel(manager);
            tb.Init();
            tb.Width = Game.MainWindow.ClientWidth;
            tb.Height = Game.MainWindow.ClientHeight;
            tb.Left = Game.MainWindow.ClientLeft;
            tb.Top =Game.MainWindow.ClientTop;
            tb.Color = Color.Red;
            tb.Alpha = 0;
            manager.Add(tb);
            this.manager = manager;

            Height = 400;
            Width = 400;
            MinimumHeight = 100;
            MinimumWidth = 100;
            Text = "You died!";
            CaptionVisible = false;
            Caption.Text = Description.Text = string.Empty;
            Center();
            Movable = false;
            TopPanel.Visible = true;

            deathImage = new ImageBox(manager);
            deathImage.Init();
            deathImage.Left = 8;
            deathImage.Top = 8;
            deathImage.Image = ContentPack.Textures["gui\\death"];
            deathImage.Width = deathImage.Image.Width;
            deathImage.Height = deathImage.Image.Height;
            ClientWidth = (deathImage.Left * 2) + deathImage.Width;
            deathMessage = new Label(manager);
            deathMessage.Init();
            deathMessage.Left = 8;
            deathMessage.Top = TopPanel.Height + 24;
            deathMessage.Text = DeathMessage;
            deathMessage.Width = (int)manager.Skin.Fonts["Default14"].Resource.MeasureRichString(deathMessage.Text,manager,true).X;
            deathMessage.Font = FontSize.Default14;
            deathMessage.Left = ((ClientWidth / 2) - (deathMessage.Width / 2));

            deathMessageBG = new Label(manager);
            deathMessageBG.Init();
            deathMessageBG.Left = 8;
            deathMessageBG.Top = TopPanel.Height + 23;
            deathMessageBG.Text = DeathMessage;
            deathMessage.Text = deathMessage.Text.Replace(":Red]", ":DarkRed]");
            deathMessage.TextColor = Color.DimGray;
            deathMessageBG.Width = (int)manager.Skin.Fonts["Default14"].Resource.MeasureRichString(deathMessage.Text, manager, true).X;
            deathMessageBG.Font = FontSize.Default14;
            deathMessageBG.Left = ((ClientWidth / 2) - (deathMessage.Width / 2)) -1;
            deathMessageBG.Height = deathMessage.Height = 24;

            Respawn = new Button(manager);
            Respawn.Init();
            Respawn.Text = "Respawn";
            Respawn.Left = ((ClientWidth / 2) - (Respawn.Width)) - 4;
            Respawn.Top = 8;
            Respawn.Click += Respawn_Click;
            BottomPanel.Add(Respawn);

            Quit = new Button(manager);
            Quit.Init();
            Quit.Text = "Quit";
            Quit.Left = (ClientWidth / 2) + 4;
            Quit.Top = 8;
            Quit.Click += Quit_Click;
            BottomPanel.Add(Quit);
   
         
            TopPanel.Add(deathImage);
            Add(deathMessage);
            Add(deathMessageBG);
            TopPanel.Height = (deathImage.Top * 2) + deathImage.Height;
            Height = TopPanel.Height + BottomPanel.Height + deathMessage.Height + 24;

            StayOnTop = true;
            FocusLost += TaskDeath_FocusLost;
            CanFocus = true;
            Focused = true;

            Closed +=TaskDeath_Closed;
        }

        void TaskDeath_Closed(object sender, WindowClosedEventArgs e)
        {
            manager.Remove(tb);
        }

        void TaskDeath_FocusLost(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Focused = true;
        }
        public override void Close()
        {
            if (!closing)
            {
                //When closing (because of quit), respawn anyways so it is saved
                Respawn_Click(this, null);
                base.Close();
            }
        }

        void Quit_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Game.MainWindow.ConfirmLevelQuit();
        }

        void Respawn_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            closing = true;
            this.Hide();
            if (Game.level.BedSpawnPoint != Vector2.Zero)
            {   if (Game.level.tiles[(int)Game.level.BedSpawnPoint.X, (int)Game.level.BedSpawnPoint.Y].Foreground != Item.WoodenBed)
                {
                    Interface.MainWindow.Console.MessageBuffer.Add(new ConsoleMessage("Could not respawn, bed missing.", 2));
                    Game.level.Players[0].Reset(Game.level.SpawnPoint);
                }
                else if (Game.level.tiles[(int)Game.level.BedSpawnPoint.X, (int)Game.level.BedSpawnPoint.Y - 1].Foreground.Collision != BlockCollision.Passable || Game.level.tiles[(int)Game.level.BedSpawnPoint.X, (int)Game.level.BedSpawnPoint.Y - 2].Foreground.Collision != BlockCollision.Passable)
                {
                    Interface.MainWindow.Console.MessageBuffer.Add(new ConsoleMessage("Could not respawn, bed obstructed.", 2));
                    Game.level.Players[0].Reset(Game.level.SpawnPoint);
                }
                else
                {
                     Game.level.Players[0].Reset(new Vector2(Game.level.BedSpawnPoint.X * Tile.Width, Game.level.BedSpawnPoint.Y * Tile.Height));
                }
            }
            else
            Game.level.Players[0].Reset(Game.level.SpawnPoint);
        }

        float darkness = 1f;
        float speed = .2f;
        bool rising;
        bool closing;

        protected override void Update(GameTime gameTime)
        {
            if (!closing && tb.Alpha < 100)
            {
                tb.Alpha += (float)gameTime.ElapsedGameTime.TotalSeconds * 50;
            }
            if (closing && tb.Alpha > 0)
            {
                tb.Alpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 70; 
            }
            else if (closing && tb.Alpha <= 0)
            {
                this.Close();
                Manager.Remove(tb);
            }
            if (darkness > .7f && !rising)
            {
                darkness -= (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
                deathImage.Color = new Color(darkness, darkness, darkness);
                deathImage.Invalidate();
                if (darkness <= .7f)
                    rising = true;
            }
            else if (darkness < 1f && rising)
            {
                darkness += (float)gameTime.ElapsedGameTime.TotalSeconds * speed;
                deathImage.Color = new Color(darkness, darkness, darkness);
                deathImage.Invalidate();
                if (darkness >= 1)
                    rising = false;
            }
            base.Update(gameTime);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
         
            base.DrawControl(renderer, rect, gameTime);
        }
      
       
        #endregion

        #region Methods
        public override void Init()
        {
            base.Init();
        }
        #endregion

    }
}
