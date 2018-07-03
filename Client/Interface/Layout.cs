#region Using
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Timers;
using Cyral.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
#endregion

namespace ZarknorthClient.Interface
{
    /// <summary>
    /// Handles initiation and layout of the UI elements
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Fields

        private Texture2D bg;
        public static TomShane.Neoforce.Controls.Console Console;
        public static SideBar sidebar;
        public static TabControl MainTabs;
        public static List<Label> playerLabels;
        public static List<Label> enemyLabels;
        public Label Copyright;
        public static ListBox UserList;
        public SideBarPanel minimapPnl;
        public SideBarPanel playerPnl;

        public static TabControl tabControl;
        public static Banner InfoBanner;
        public Button[] btnTasks;
        public Label lblObjects;
        public Label lblAvgFps;
        public Label lblFps;
        public Label lblTime;
        public Label lblItem;
        public TabControl tabMinimap;
        public ImageBox imgMinimap;
        public static GradientPanel InventoryPanel;
        public Button toolsBtn;
        public Button blocksBtn;
        public Button itemsBtn;
        public Button weaponsBtn;
        public Button[,] slotBtns;
        public Button[] EditBtns;
        public GradientPanel tlbrPrimary;
        public GradientPanel tlbrSecondary;
        public GradientPanel MapPanel;
        public ImageBox imgHealth;
        public ImageBox imgEnergy, imgMoney, imgInvetory, imgOptions, imgHome, imgAchievement, imgCrafting, imgGamemode, imgScreenshot;
        public Texture2D txHealth;
        public Texture2D txEnergy, txMoney, txGamemode, txOptions, txHome, txAchievement, txCrafting, txScreenshot, txMenuBackground;
        public Button btnGamemode, btnOptions, btnHome, btnAchievement, btnCrafting, btnScreenshot;
        public ProgressBar prgHealth;
        public ProgressBar prgEnergy;
        public static ImageBox Logo;
        public static Button[] btnsInventory;
        public static Label[] lblsInventory;
        public static ImageBox[] imgsInventory;

        public static Button[] btnsCrafting;
        public static Label[] lblsCrafting;
        public static ImageBox[] imgsCrafting;
        public Button btnExpand;
        public Texture2D LogoTex;
        //For holding items/inventory
        public static ImageBox mouseImage; //Image of item
        public static Label mouseLabel;
        public static Slot mouseSlot = new Slot(Item.Blank, 0);
        public static Button[] btnsTech;
        public static List<Tuple<Point, Point>> techPoints = new List<Tuple<Point, Point>>();
        public Texture2D[] MenuTextures = new Texture2D[10];
        public static int InventoryClosedHeight, InventoryOpenHeight;
        public static float InventoryHeight;
        public Label lblMoney;
        public static SlotContainer inventory;

        public static TaskItemBox ItemBoxWindow;
        public static TaskCrafting CraftingWindow;
        public static TaskPaint PaintWindow;
        public static List<Control> MenuItems = new List<Control>();
        public static ClearTip BlockControl;
        public static TabControl universeTabs;
        public static ImageBox menuBackground;
        public static Label AutoSaveLvl;
        public static Button SandboxInventory;
        public static bool sandBoxFadingIn, sandBoxFadingOut;
        #endregion
#region Constructors
        public MainWindow(Manager manager)
            : base(manager)
        {
            (Manager.Game as Game).ApplyResolution();
            Transparent = true;
            Click += MainWindow_Click;
            //Autostart
            //Game.CurrentGameState = Game.GameState.HomeLoggedOn;
            // InitMainScreen();      
        }

        void MainWindow_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (MainTabs != null)
                tbc_FocusLost(MainTabs, null);
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region Methods
        public void LoadContent()
        {

            LogoTex = ContentPack.Textures["gui\\logo"];
            bg = ContentPack.Textures["gui\\background"];
            txEnergy = ContentPack.Textures["gui\\icons\\energy"];
            txHealth = ContentPack.Textures["gui\\icons\\health"];
            txMoney = ContentPack.Textures["gui\\icons\\money"];
            txOptions = ContentPack.Textures["gui\\icons\\options"];
            txHome = ContentPack.Textures["gui\\icons\\home"];
            txCrafting = ContentPack.Textures["gui\\icons\\crafting"];
            txAchievement = ContentPack.Textures["gui\\icons\\achievements"];
            txGamemode = ContentPack.Textures["gui\\icons\\gamemode"];
            txScreenshot = ContentPack.Textures["gui\\icons\\screenshot"];
            txMenuBackground = ContentPack.Textures["gui\\menuBackground"];
            (Manager.Game as Application).BackgroundImage = bg;
            (Manager.Game as Application).Window.Title = "Zarknorth";

            for (int i = 0; i < MenuTextures.Length; i++)
            {
                MenuTextures[i] = ContentPack.Textures["gui\\menu" + i];
            }
            InitControls();
#if DEBUG
            InitUniverseScreen();
            InitGenerateScreen();
            DeInitUniverseScreen();
            Game.UniverseViewer.Land(new PlanetaryObject("Debug", 2, null, PlanetaryType.Planet, PlanetType.Temperate) { Radius = 14, Gravity = 1 });
#endif
            //InitMainScreen(); 
        }
        /// <summary>
        /// Handles the Dialog Closed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ////////////////////////////////////////////////////////////////////////////
        private void InitControls()
        {



            sidebar = new SideBar(Manager);
            sidebar.Init();
            sidebar.StayOnBack = true;
            sidebar.Passive = true;
            sidebar.Width = ClientWidth;
            sidebar.Height = 200;
            sidebar.Anchor = Anchors.Horizontal;
            sidebar.Left = ClientLeft;
            sidebar.Top = ClientTop;
            sidebar.Draw += sidebar_Draw;

            //TaskCredits tmp2 = new TaskCredits(Manager);
            //tmp2.Closing += new WindowClosingEventHandler(WindowClosing);
            //tmp2.Closed += new WindowClosedEventHandler(WindowClosed);
            //tmp2.Init();
            //Manager.Add(tmp2);
            mouseImage = new ImageBox(Manager);
            mouseImage.Init();
            mouseImage.Width = 24;
            mouseImage.Height = 24;
            mouseImage.SizeMode = SizeMode.Fit;
            mouseImage.Passive = true;
            mouseLabel = new Label(Manager);
            mouseLabel.Init();
            mouseLabel.Width = 28;
            mouseLabel.Height = 10;
            mouseLabel.Text = "";
            mouseLabel.Passive = true;
            mouseLabel.StayOnTop = true;
            mouseLabel.Alignment = Alignment.MiddleCenter;
            mouseLabel.BringToFront();
            Manager.Add(mouseLabel);
            Manager.Add(mouseImage);
            Manager.Add(sidebar);


            menuBackground = new ImageBox(Manager);
            menuBackground.Init();
            Add(menuBackground);
            menuBackground.SendToBack();

            InitLogo();
            InitCopyright();

#if !DEBUG
            TaskLauncher tmp = new TaskLauncher(Manager);
            tmp.Closing += new WindowClosingEventHandler(WindowClosing);
            tmp.Closed += new WindowClosedEventHandler(WindowClosed);
            tmp.Init();
            tmp.Top += 40;
            tmp.RealTop = tmp.Top;
            CreateInfoBanner(tmp);
#endif
        }

        private void CreateInfoBanner(TaskLauncher tmp)
        {
            if (InfoBanner != null)
                Manager.Remove(InfoBanner);
            InfoBanner = new Banner(Manager);
            InfoBanner.Init();
            InfoBanner.Top = sidebar.Height + sidebar.Top + 22;
            InfoBanner.Alpha = 0;
            InfoBanner.Visible = false;
            Manager.Add(InfoBanner);
            Manager.Add(tmp);
            //Load the announcement
            // 2018 - Dont try to query the site for the announcement, instead show a placeholder and explain about the current state of the game.
            {
                InfoBanner.Visible = true;
                InfoBanner.Title = "Notice:";
                InfoBanner.Text = "This is an unfinished version of Zarknorth released on GitHub. The below launcher does not function since the website and API are now down, so click the start button to bypass it.";
                InfoBanner.Color = new Color(255,182,0);
                InfoBanner.Width = (int)Manager.Skin.Fonts["Default8"].Resource.MeasureRichString(InfoBanner.Text, Manager, true).X + 16;
                InfoBanner.Height = (int)Manager.Skin.Fonts["Default8"].Resource.MeasureRichString(InfoBanner.Text, Manager, true).Y + 22;
                InfoBanner.Left = (ClientWidth / 2) - (InfoBanner.Width / 2);
            }
            return;

            // Old stuff
            Thread LoadItem = new Thread(delegate()
            {
                InfoBanner.Title = "Loading...";
                InfoBanner.Text = string.Empty;
                string[] args = null;
                try
                {
                    //Create the POST data
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    //Make a request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.zarknorth.com/game/announcement.php");
                    request.Proxy = null;
                    request.Method = "GET";
                    //Get response
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    //Make a new steam, We will read from this
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string responseString = sr.ReadToEnd();
                        //Split response. If authenticated, pass, username, and sid
                        if (responseString != string.Empty)
                            args = responseString.Split('~');
                    }
                    if (args != null)
                    {
                        InfoBanner.Title = args[0];
                        InfoBanner.Text = args[1];
                        InfoBanner.Color = TomShane.Neoforce.Controls.Colors.FromName(args[2]);
                        if (args[3] != "null")
                        {
                            InfoBanner.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
                            {
                                System.Diagnostics.Process.Start(args[3]);
                            });
                        }
                        else
                            InfoBanner.Passive = true;
                        InfoBanner.Visible = true;
                    }
                }
                catch (Exception e)
                {
                    InfoBanner.Visible = true;
                    InfoBanner.Title = "Error:";
                    InfoBanner.Text = "An error was encountered while loading the announcement.";
                }
                InfoBanner.Width = (int)Manager.Skin.Fonts["Default8"].Resource.MeasureRichString(InfoBanner.Text, Manager, true).X + 16;
                InfoBanner.Height = (int)Manager.Skin.Fonts["Default8"].Resource.MeasureRichString(InfoBanner.Text, Manager, true).Y + 22;
                InfoBanner.Left = (ClientWidth / 2) - (InfoBanner.Width / 2);
            });
            LoadItem.Start();
        }

        void sidebar_Draw(object sender, DrawEventArgs e)
        {
            if (Game.CurrentGameState == Game.GameState.UniverseViewer)
            {
                Game.UniverseViewer.DrawSidebar((SideBar)sender, e);
            }
        }

        public void InitGameScreen()
        {
            DeInitCopyright();
            DeInitLogo();
            DeInitMainScreen();
            InitInventory();
            InitPrimaryGradientPanel();
            InitSecondaryGradientPanel();
            InitMapGradientPanel();
            InitConsole();
            InitMinimap();
            InitCraftingWindow();
            InitPaintWindow();
            BlockControl = new ClearTip(Manager);
            BlockControl.Init();
            Add(BlockControl);
            sidebar.Alpha = 255;
            sidebar.Width = ClientWidth;
            sidebar.Anchor = Anchors.Horizontal;

            AutoSaveLvl = new Label(Manager);
            AutoSaveLvl.Init();
            AutoSaveLvl.Top = 38;
            AutoSaveLvl.Left = ClientWidth - 250;
            AutoSaveLvl.Width = 140;
            AutoSaveLvl.Text = "";
            AutoSaveLvl.TextChanged += AutoSaveLvl_TextChanged;
            Add(AutoSaveLvl);
        }
        void AutoSaveLvl_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            AutoSaveLvl.Left = ClientWidth - 250 - ((int)Manager.Skin.Fonts["Default8"].Resource.MeasureString(AutoSaveLvl.Text).X / 2);
        }
        public void DeInitGameScreen()
        {
            Remove(InventoryPanel);
            Remove(tlbrSecondary);
            Remove(tlbrPrimary);
            Remove(MapPanel);
            Manager.Remove(MainTabs);
            Remove(BlockControl);
            Manager.Remove(CraftingWindow);
            Manager.Remove(PaintWindow);
            Remove(imgMinimap);
            Remove(AutoSaveLvl);
        }

        private void InitCraftingWindow()
        {
            CraftingWindow = new TaskCrafting(Manager);
            CraftingWindow.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
            CraftingWindow.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
            CraftingWindow.Init();
            Manager.Add(CraftingWindow);
            CraftingWindow.Hide();
        }

        private void InitPaintWindow()
        {
            PaintWindow = new TaskPaint(Manager);
            PaintWindow.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
            PaintWindow.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
            PaintWindow.Init();
            Manager.Add(PaintWindow);
            PaintWindow.Hide();
        }


        public void InitLogo()
        {
            menuBackground.Image = MenuTextures[0];
            menuBackground.Width = MenuTextures[0].Width;
            menuBackground.Height = MenuTextures[0].Height;
            menuBackground.Left = ClientWidth - menuBackground.Width;
            menuBackground.Top = 0;
            menuBackground.Passive = true;
            if (Logo == null)
            {
                Logo = new ImageBox(Manager);
                Logo.Init();
            }
            Logo.Image = LogoTex;
            Logo.Left = (ClientWidth / 2) - (LogoTex.Width / 2);
            Logo.Top = 24;
            Logo.Width = Logo.Image.Width;
            Logo.Height = Logo.Image.Height;
            if (!sidebar.Contains(Logo, true))
            sidebar.Add(Logo);
            Logo.Visible = true;
            sidebar.Height = Logo.Image.Height + Logo.Top + 24;
        }

        private void InitCopyright()
        {
            Copyright = new Label(Manager);
            Copyright.Init();
            WarningText[] warningText = (WarningText[])System.Reflection.Assembly.GetExecutingAssembly()
                           .GetCustomAttributes(typeof(WarningText), false)
                           .Cast<WarningText>();
            Copyright.TextChanged += Copyright_TextChanged;
            string VersionText = Game.GetVersionString();
            Copyright.Text = VersionText + " - " + warningText[0].Text;
            Copyright.Top = ClientHeight - 24;
            Copyright.Font = FontSize.Default14;
            Copyright.TextChanged += Copyright_TextChanged;
            Add(Copyright);
        }

        public void DeInitLogo()
        {
            sidebar.Remove(Logo);
        }
        public void DeInitCopyright()
        {
            Remove(Copyright);
        }
        void Copyright_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Copyright.Width = (int)Manager.Skin.Fonts["Default14"].Resource.MeasureString(Copyright.Text).X;
            Copyright.Left = (ClientWidth / 2) - (Copyright.Width / 2);
        }
        GradientPanel tb;
        PlanetControl startPlanet, campaignPlanet, multiplayerPlanet, freebuildPlanet, backPlanet;
        public void InitMainScreen()
        {
            InitLogo();
            menuBackground.Visible = true;
            WarningText[] warningText = (WarningText[])System.Reflection.Assembly.GetExecutingAssembly()
                         .GetCustomAttributes(typeof(WarningText), false)
                         .Cast<WarningText>();

            VersionText[] versionText = (VersionText[])System.Reflection.Assembly.GetExecutingAssembly()
                           .GetCustomAttributes(typeof(VersionText), false)
                           .Cast<VersionText>();
            string VersionText = Game.UserName + " - " + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + " " + versionText[0].Text + " " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            Copyright.Text = VersionText + " - " + warningText[0].Text;

            PlanetControl optionsPlanet = new PlanetControl(Manager);
            optionsPlanet.Init();
            optionsPlanet.Image = MenuTextures[3];
            optionsPlanet.Text = "Options";
            optionsPlanet.Left = (ClientWidth / 2) + 140;
            optionsPlanet.Top = (ClientHeight / 2) - 170 + 16 + 16;
            optionsPlanet.Click += optionsPlanet_Click;
            Add(optionsPlanet);
            //Add(p2);
            PlanetControl creditsPlanet = new PlanetControl(Manager);
            creditsPlanet.Init();
            creditsPlanet.Image = MenuTextures[4];
            creditsPlanet.Text = "Credits";
            creditsPlanet.Left = (ClientWidth / 2) + 210;
            creditsPlanet.Top = (ClientHeight / 2) - 90 + 16;
            creditsPlanet.Click += creditsPlanet_Click;
            Add(creditsPlanet);

            PlanetControl logoutPlanet = new PlanetControl(Manager);
            logoutPlanet.Init();
            logoutPlanet.Image = MenuTextures[5];
            logoutPlanet.Text = "Sign Out";
            logoutPlanet.Left = (ClientWidth / 2) + 90;
            logoutPlanet.Top = (ClientHeight / 2) + 90 - 16 - 8;
            logoutPlanet.Click += logoutPlanet_Click;
            Add(logoutPlanet);
            // Add(p1);
            PlanetControl profilePlanet = new PlanetControl(Manager);
            profilePlanet.Init();
            profilePlanet.Image = MenuTextures[2];
            profilePlanet.Text = "Profile";
            profilePlanet.Left = (ClientWidth / 2) - 370;
            profilePlanet.Top = (ClientHeight / 2) - 125;
            profilePlanet.Click += profilePlanet_Click;
            Add(profilePlanet);

            startPlanet = new PlanetControl(Manager);
            startPlanet.Init();
            startPlanet.Image = MenuTextures[1];
            startPlanet.Text = "Start";
            startPlanet.Left = (ClientWidth / 2) - 285;
            startPlanet.Top = (ClientHeight / 2) - 130;
            startPlanet.Click += startPlanet_Click;
            startPlanet.Enabled = IO.ProfileExists();
            Add(startPlanet);

            menuBackground.Image = txMenuBackground;
            menuBackground.Width = txMenuBackground.Width;
            menuBackground.Height = txMenuBackground.Height;
            menuBackground.Left = (ClientWidth / 2) - 280;
            menuBackground.Top = (ClientHeight / 2) - 180 + (Game.Fullscreen ? 24 : 0);

            MenuItems.Add(startPlanet);
            MenuItems.Add(profilePlanet);
            MenuItems.Add(logoutPlanet);
            MenuItems.Add(optionsPlanet);
            MenuItems.Add(creditsPlanet);

            if (!IO.ProfileExists())
            {
                tb = new GradientPanel(Manager);
                tb.Init();
                tb.Width = Game.MainWindow.ClientWidth;
                tb.Height = Game.MainWindow.ClientHeight;
                tb.Left = Game.MainWindow.ClientLeft;
                tb.Top = Game.MainWindow.ClientTop;
                tb.Color = Color.Black;
                tb.Alpha = 190f;
                Manager.Add(tb);
                MenuItems.Add(tb);
                profilePlanet_Click(null, null);
            }
            //Fullscreen reset
            Copyright.Top = ClientHeight - 24;
            this.VerticalScrollBar.Value = 100;
            sidebar.Width = ClientArea.Width;
        }

        void profilePlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TaskProfile Task = new TaskProfile(Manager);
            Task.Closing += new WindowClosingEventHandler(WindowClosing);
            Task.Closed += new WindowClosedEventHandler(WindowClosed);
            Task.Init();
            Manager.Add(Task);
            return;
        }

        void creditsPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TaskCredits Task = new TaskCredits(Manager);
            Task.Closing += new WindowClosingEventHandler(WindowClosing);
            Task.Closed += new WindowClosedEventHandler(WindowClosed);
            Task.Init();
            Manager.Add(Task);
            return;
        }

        void logoutPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            MessageBox m = new MessageBox(Manager, MessageBoxType.YesNo, "Are you sure you would like to logout?", "Logout");
            m.Init();
            m.Closed += m_Closed;
            m.ShowModal();
            Manager.Add(m);
            return;
        }

        void optionsPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

            TaskOptions Task = new TaskOptions(Manager);
            Task.Closing += new WindowClosingEventHandler(WindowClosing);
            Task.Closed += new WindowClosedEventHandler(WindowClosed);
            Task.Init();
            Manager.Add(Task);
            return;
        }

        void startPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            menuBackground.Visible = false;
            for (int i = 0; i < MenuItems.Count; i++)
            {
                Remove(MenuItems[i]);
            }
            campaignPlanet = new PlanetControl(Manager);
            campaignPlanet.Init();
            campaignPlanet.Image = MenuTextures[6];
            campaignPlanet.Text = "Play Zarknorth";
            campaignPlanet.Description = "Play alone with normal settings.\nTry this first!";
            campaignPlanet.Left = (ClientWidth / 2) - (250 / 2);
            campaignPlanet.Top = (ClientHeight / 2) - 75;
            campaignPlanet.Click += campaignPlanet_Click;
            Add(campaignPlanet);

            freebuildPlanet = new PlanetControl(Manager);
            freebuildPlanet.Init();
            freebuildPlanet.Image = MenuTextures[7];
            freebuildPlanet.Text = "Sandbox Mode";
            freebuildPlanet.Description = "Play community made maps &\nbuild with unlimited materials!";
            freebuildPlanet.Left = (ClientWidth / 2) - 250 - (200 / 2);
            freebuildPlanet.Top = (ClientHeight / 2) - 50;
            freebuildPlanet.Click += freebuildPlanet_Click;
            Add(freebuildPlanet);

            multiplayerPlanet = new PlanetControl(Manager);
            multiplayerPlanet.Init();
            multiplayerPlanet.Image = MenuTextures[8];
            multiplayerPlanet.Text = "Multiplayer Mode";
            multiplayerPlanet.Description = "Coming soon!";
            multiplayerPlanet.Left = (ClientWidth / 2) + 250 - (200 / 2);
            multiplayerPlanet.Top = (ClientHeight / 2) - 50;
            Add(multiplayerPlanet);

            backPlanet = new PlanetControl(Manager);
            backPlanet.Init();
            backPlanet.Image = MenuTextures[9];
            backPlanet.Text = "Back";
            backPlanet.Left = ClientWidth - 96 - 64;
            backPlanet.Top = ClientHeight - 96 - 64;
            backPlanet.Click += backPlanet_Click;
            backPlanet.Height = 150;
            Add(backPlanet);

            MenuItems.Add(campaignPlanet);
            MenuItems.Add(freebuildPlanet);
            MenuItems.Add(multiplayerPlanet);
            MenuItems.Add(backPlanet);
        }

        void backPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            for (int i = 0; i < MenuItems.Count; i++)
            {
                Remove(MenuItems[i]);
            }
            InitMainScreen();
        }
        void freebuildPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (tb != null)
                Manager.Remove(tb);
            InitSandboxScreen();
        }

        void campaignPlanet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (tb != null)
                Manager.Remove(tb);
            InitUniverseScreen();
        }
        public Label PlanetName;
        public Label PlanetDesc;
        public Button btnLeft, btnRight, btnUp, btnDown;
        public ImageBox imgZoom, imgEdit;
        public TrackBar tbrZoom;
        public static Button[] btnUniverse;
        public static Button btnLand;
        public Label lblLand;
        public ImageBox imgLand;
        public static ListBox bookmarkList, logList;
        public Button bookmarkAdd, bookmarkDel;
        public static Button universeExit, debugEnter;
        public void DeInitUniverseScreen()
        {
            sidebar.Remove(PlanetName);
            sidebar.Remove(PlanetDesc);
            sidebar.Remove(btnLeft);
            sidebar.Remove(btnRight);
            sidebar.Remove(btnDown);
            sidebar.Remove(btnUp);
            sidebar.Remove(tbrZoom);
            sidebar.Remove(btnLand);
            sidebar.Remove(lblLand);
            sidebar.Remove(imgLand);
            sidebar.Remove(universeTabs);
            Remove(universeExit);
#if DEBUG
            Remove(debugEnter);
#endif
            if (btnUniverse != null)
            foreach (Control c in btnUniverse)
                sidebar.Remove(c);
        }
        public void InitSandboxScreen()
        {
            DeInitCopyright();
            DeInitMainScreen();

            TaskSandbox task = new TaskSandbox(Manager, this);
            task.Closed += AchievementLog_Closed;
            task.Init();
            Add(task);
        }
        public void InitUniverseScreen()
        {
            Manager.Remove(sidebar);
            sidebar.Dispose();
            sidebar = new SideBar(Manager);
            sidebar.Init();
            sidebar.StayOnBack = true;
            sidebar.Passive = false;
            sidebar.Width = ClientWidth;
            sidebar.Height = 200;
            sidebar.Anchor = Anchors.Horizontal;
            sidebar.Left = ClientLeft;
            sidebar.Top = ClientTop;
            sidebar.Draw += sidebar_Draw;
            Manager.Add(sidebar);
            btnUniverse = new Button[6];

            DeInitCopyright();
            DeInitLogo();
            DeInitMainScreen();
            sidebar.Passive = false;
            sidebar.Width = 322;
            sidebar.Height = ClientHeight;
            sidebar.Anchor = Anchors.Vertical | Anchors.Left;
            sidebar.BringToFront();


            PlanetName = new Label(Manager);
            PlanetName.Init();
            PlanetName.Left = 8;
            PlanetName.Top = sidebar.Width + 8;
            PlanetName.Text = "Name";
            PlanetName.Font = FontSize.Default14;
            PlanetName.Width = 300;
            PlanetName.Height = 20;
            PlanetName.Alignment = Alignment.MiddleCenter;
            PlanetName.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                imgEdit.Left = (sidebar.ClientWidth / 2) + (int)(Manager.Skin.Fonts["Default14"].Resource.MeasureString(PlanetName.Text).X / 2) + 8;
            });
            sidebar.Add(PlanetName);

            imgEdit = new ImageBox(Manager);
            imgEdit.Init();
            imgEdit.Image = ContentPack.Textures["gui\\icons\\edit"];
            imgEdit.Top = PlanetName.Top;
            imgEdit.Width = imgEdit.Height = 16;
            imgEdit.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                TaskRenamePlanet tmp = new TaskRenamePlanet(Manager, Game.UniverseViewer.SelectedPlanet);
                tmp.Init();
                Manager.Add(tmp);
                tmp.Show();
            });
            sidebar.Add(imgEdit);

            PlanetDesc = new Label(Manager);
            PlanetDesc.Init();
            PlanetDesc.Left = PlanetName.Left;
            PlanetDesc.Top = PlanetName.Top + PlanetName.Height;
            PlanetDesc.Text = "Description";
            PlanetDesc.Font = FontSize.Default8;
            PlanetDesc.Width = 300;
            PlanetDesc.Height = 12;
            PlanetDesc.Alignment = Alignment.MiddleCenter;
            sidebar.Add(PlanetDesc);

            btnLeft = new Button(Manager);
            btnLeft.Init();
            btnLeft.Left = 8 + 2;
            btnLeft.Top = PlanetDesc.Top + PlanetDesc.Height + 8 + 2 + 20;
            btnLeft.Width = 20;
            btnLeft.Height = 20;
            btnLeft.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\left"]);
            btnLeft.ToolTip.Text = "Navigate left (" + Game.Controls["Left"] + ")";
            btnLeft.MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.CameraPointTo.X -= UniverseViewer.CameraMoveSpeed / Game.UniverseViewer.Camera.Zoom;
            });
            sidebar.Add(btnLeft);

            btnUp = new Button(Manager);
            btnUp.Init();
            btnUp.Left = btnLeft.Left + btnLeft.Width + 2;
            btnUp.Top = PlanetDesc.Top + PlanetDesc.Height + 8;
            btnUp.Width = 20;
            btnUp.Height = 20;
            btnUp.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\up"]);
            btnUp.ToolTip.Text = "Navigate up (" + Game.Controls["Up"] + ")";
            btnUp.MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.CameraPointTo.Y -= UniverseViewer.CameraMoveSpeed / Game.UniverseViewer.Camera.Zoom;
            });
            sidebar.Add(btnUp);

            btnRight = new Button(Manager);
            btnRight.Init();
            btnRight.Left = btnUp.Left + btnUp.Width + 2;
            btnRight.Top = btnLeft.Top;
            btnRight.Width = 20;
            btnRight.Height = 20;
            btnRight.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\right"]);
            btnRight.ToolTip.Text = "Navigate right (" + Game.Controls["Right"] + ")";
            btnRight.MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.CameraPointTo.X += UniverseViewer.CameraMoveSpeed / Game.UniverseViewer.Camera.Zoom;
            });
            sidebar.Add(btnRight);

            btnDown = new Button(Manager);
            btnDown.Init();
            btnDown.Left = btnUp.Left;
            btnDown.Top = btnRight.Top + btnRight.Height + 2;
            btnDown.Width = 20;
            btnDown.Height = 20;
            btnDown.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\down"]);
            btnDown.ToolTip.Text = "Navigate down (" + Game.Controls["Down"] + ")";
            btnDown.MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.CameraPointTo.Y += UniverseViewer.CameraMoveSpeed / Game.UniverseViewer.Camera.Zoom;
            });
            sidebar.Add(btnDown);

            tbrZoom = new TrackBar(Manager);
            tbrZoom.Init();
            tbrZoom.Left = btnRight.Left + btnRight.Width + 4;
            tbrZoom.Top = btnRight.Top - 4;
            tbrZoom.Range = 100;
            tbrZoom.Value = 100;
            tbrZoom.Height = 28;
            tbrZoom.Width = 100;
            tbrZoom.ToolTip.Text = "Zoom (" + Game.Controls["Zoom In"] + " or " + Game.Controls["Zoom Out"] + ")";
            tbrZoom.Color = new Color(100, 167, 225);
            tbrZoom.ValueChanged += tbrZoom_ValueChanged;
            sidebar.Add(tbrZoom);

            int btnsWidth = 3;
            int btnsHeight = 2;
            for (int x = 0; x < btnsWidth; x++)
            {
                for (int y = 0; y < btnsHeight; y++)
                {
                    Button b = new Button(Manager);
                    b.Init();
                    b.Width = 24;
                    b.Height = 24;
                    b.Left = tbrZoom.Left + tbrZoom.Width + 4 + ((b.Width + 2) * x);
                    if (x != 0 && x != btnsWidth)
                        b.Left += 2;
                    b.Top = btnRight.Top - 12 + ((b.Height + 2) * y);
                    b.Text = "";
                    btnUniverse[y * btnsWidth + x] = b;
                    sidebar.Add(btnUniverse[y * btnsWidth + x]);
                }
            }
            btnUniverse[0].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\next"]);
            btnUniverse[0].ToolTip.Text = "Next";
            btnUniverse[0].MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.Next();
            });

            btnUniverse[btnsWidth].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\back"]);
            btnUniverse[btnsWidth].MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.Back();
            });
            btnUniverse[btnsWidth].ToolTip.Text = "Previous";

            btnUniverse[1].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\home_go"]);
            btnUniverse[1].ToolTip.Text = "Go to Home";
            btnUniverse[1].Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                MessageBox confirmGoHome = new MessageBox(Manager, MessageBoxType.YesNo, "Are you sure you would like to go to your home planet?", "Go to Home");
                confirmGoHome.Init();
                confirmGoHome.Closed += new WindowClosedEventHandler(delegate(object sender, WindowClosedEventArgs ev)
                {
                    if ((sender as Dialog).ModalResult == ModalResult.Yes)
                    {
                        Game.UniverseViewer.GoHome();
                    }
                });
                confirmGoHome.ShowModal();
                Manager.Add(confirmGoHome);
            });

            btnUniverse[btnsWidth + 1].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\home_set"]);
            btnUniverse[btnsWidth + 1].ToolTip.Text = "Set as Home";
            btnUniverse[btnsWidth + 1].Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                MessageBox confirmHomeSet = new MessageBox(Manager, MessageBoxType.YesNo, "Are you sure you would like to set this planet as your home?", "Set as Home");
                confirmHomeSet.Init();
                confirmHomeSet.Closed += new WindowClosedEventHandler(delegate(object sender, WindowClosedEventArgs ev)
                {
                    if ((sender as Dialog).ModalResult == ModalResult.Yes)
                    {
                        Game.UniverseViewer.SetHome();
                    }
                });
                confirmHomeSet.ShowModal();
                Manager.Add(confirmHomeSet);
            });
            btnUniverse[btnsWidth + 1].EnabledChanged += btn_EnabledChanged;

            btnUniverse[2].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\bookmarks_add"]);
            btnUniverse[2].ToolTip.Text = "Add to Bookmarks";
            btnUniverse[2].MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.Bookmarks.AddUnique(Game.UniverseViewer.SelectedPlanet);
                bookmarkList.Items.AddUnique(Game.UniverseViewer.SelectedPlanet.Name);
                RecalcBookmarks();
            });

            btnUniverse[btnsWidth + 2].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\bookmarks_delete"]);
            btnUniverse[btnsWidth + 2].ToolTip.Text = "Remove from Bookmarks";
            btnUniverse[btnsWidth + 2].MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                Game.UniverseViewer.Bookmarks.Remove(Game.UniverseViewer.SelectedPlanet);
                bookmarkList.Items.Remove(Game.UniverseViewer.SelectedPlanet.Name);
                RecalcBookmarks();
            });

            btnLand = new Button(Manager);
            btnLand.Init();
            btnLand.Height = btnLand.Width = 50;
            btnLand.Top = btnUniverse[2].Top;
            btnLand.Left = btnUniverse[2].Left + 28;
            btnLand.Text = "";
            btnLand.Color = Color.Red;
            btnLand.MousePress += new MouseEventHandler(delegate(object o, MouseEventArgs e)
            {
                InitGenerateScreen();
                DeInitUniverseScreen();
                Game.UniverseViewer.Land(Game.UniverseViewer.SelectedPlanet);
            });
            sidebar.Add(btnLand);

            lblLand = new Label(Manager) { Passive = true };
            lblLand.Init();
            lblLand.Text = "Land";
            lblLand.Top = 8;
            lblLand.Left = 11;
            btnLand.Add(lblLand);

            imgLand = new ImageBox(Manager) { Passive = true };
            imgLand.Init();
            imgLand.Image = ContentPack.Textures["gui\\icons\\land"];
            imgLand.Top = 24;
            imgLand.Left = 17;
            imgLand.Width = imgLand.Height = 16;
            imgLand.EnabledChanged += img_EnabledChanged;
            btnLand.Add(imgLand);

            universeTabs = new TabControl(Manager);
            universeTabs.Init();
            universeTabs.Left = 8;
            universeTabs.Top = btnDown.Top + btnDown.Height + 6;
            universeTabs.Width = sidebar.ClientWidth - (universeTabs.Left * 2);
            universeTabs.Height = sidebar.ClientHeight - 8 - universeTabs.Top;
            sidebar.Add(universeTabs);

            universeTabs.AddPage("Info");
            universeTabs.AddPage("Bookmarks");
            universeTabs.AddPage("Log");

            universeStatLabels = new Label[12];
            for (int i = 0; i < 12; i++)
            {
                universeStatLabels[i] = new Label(Manager);
                universeStatLabels[i].Init();
                universeStatLabels[i].Passive = false;
                universeStatLabels[i].Left = 8;
                universeStatLabels[i].Top = 8 + (i * (universeStatLabels[i].Height + 4));
                universeStatLabels[i].Width = universeTabs.Width;
                universeTabs.TabPages[0].Add(universeStatLabels[i]);
            }

            bookmarkList = new ListBox(Manager);
            bookmarkList.Init();
            bookmarkList.Left = bookmarkList.Top = 8;
            bookmarkList.Width = universeTabs.ClientWidth - 16;
            bookmarkList.Height = universeTabs.ClientHeight - 16 - 24;
            bookmarkList.ItemIndexChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (bookmarkList.Items.Count > 0)
                {
                    foreach (Galaxy galaxy in Game.UniverseViewer.Systems)
                        foreach (SolarSystem solarsystem in galaxy.Children)
                            foreach (PlanetaryObject planet in solarsystem.Children)
                                if (planet.Name == bookmarkList.Items[bookmarkList.ItemIndex].ToString())
                                {
                                    Game.UniverseViewer.SelectedPlanet = planet;
                                    Game.UniverseViewer.CenterCamera();
                                }
                }
            });
            universeTabs.TabPages[1].Add(bookmarkList);

            bookmarkAdd = new Button(Manager);
            bookmarkAdd.Init();
            bookmarkAdd.Width = 128;
            bookmarkAdd.Text = "Add To Bookmarks";
            bookmarkAdd.Left = (universeTabs.ClientWidth / 2) - bookmarkAdd.Width - 4;
            bookmarkAdd.Top = bookmarkList.Top + bookmarkList.ClientHeight + 6;
            bookmarkAdd.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                Game.UniverseViewer.Bookmarks.AddUnique(Game.UniverseViewer.SelectedPlanet);
                bookmarkList.Items.AddUnique(Game.UniverseViewer.SelectedPlanet.Name);
                RecalcBookmarks();
            });
            universeTabs.TabPages[1].Add(bookmarkAdd);

            bookmarkDel = new Button(Manager);
            bookmarkDel.Init();
            bookmarkDel.Width = 128;
            bookmarkDel.Text = "Remove Bookmark";
            bookmarkDel.Left = (universeTabs.ClientWidth / 2) + 4;
            bookmarkDel.Top = bookmarkList.Top + bookmarkList.ClientHeight + 6;
            bookmarkDel.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (bookmarkList.Items.Count > 0 && bookmarkList.ItemIndex >= 0)
                {
                    foreach (Galaxy galaxy in Game.UniverseViewer.Systems)
                        foreach (SolarSystem solarsystem in galaxy.Children)
                            foreach (PlanetaryObject planet in solarsystem.Children)
                                if (planet.Name == bookmarkList.Items[bookmarkList.ItemIndex].ToString())
                                {
                                    Game.UniverseViewer.Bookmarks.Remove(planet);
                                    bookmarkList.Items.Remove(planet.Name);
                                    bookmarkList.ItemIndex = 0;
                                    return;
                                }
                }
                RecalcBookmarks();
            });
            universeTabs.TabPages[1].Add(bookmarkDel);

            logList = new ListBox(Manager);
            logList.Init();
            logList.Left = logList.Top = 8;
            logList.Width = universeTabs.ClientWidth - 16;
            logList.Height = universeTabs.ClientHeight - 16;
            logList.ItemIndexChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                if (bookmarkList.Items.Count > 0)
                {
                    foreach (Galaxy galaxy in Game.UniverseViewer.Systems)
                        foreach (SolarSystem solarsystem in galaxy.Children)
                            foreach (PlanetaryObject planet in solarsystem.Children)
                                if (planet.Name == bookmarkList.Items[bookmarkList.ItemIndex].ToString().Split('-')[0].Trim())
                                {
                                    Game.UniverseViewer.SelectedPlanet = planet;
                                    Game.UniverseViewer.CenterCamera();
                                }
                }
            });
            universeTabs.TabPages[2].Add(logList);

            universeExit = new Button(Manager);
            universeExit.Init();
            universeExit.Width = 140;
            universeExit.Text = "[color:Gold]Exit[/color] to main menu";
            universeExit.Top = ClientHeight - universeExit.Height - 8;
            universeExit.Left = ClientWidth - universeExit.Width - 8;
            universeExit.CanFocus = false;
            universeExit.Click += universeExit_Click;
            universeExit.Visible = false;
            universeExit.Color = Color.Red;
            Add(universeExit);

#if DEBUG
            debugEnter = new Button(Manager);
            debugEnter.Init();
            debugEnter.Text = "Debug World >";
            debugEnter.Width = 100;
            debugEnter.Top = ClientHeight - debugEnter.Height - 8;
            debugEnter.Left = ClientWidth - debugEnter.Width - 16 - universeExit.Width;
            debugEnter.CanFocus = false;
            debugEnter.Visible = false;
            debugEnter.Click += debugEnter_Click;
            debugEnter.Color = Color.Orange;
            Add(debugEnter);
#endif


            (Manager.Game as Game).OpenUniverseViewer();
        }

        void debugEnter_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            InitGenerateScreen();
            DeInitUniverseScreen();
            Game.UniverseViewer.Land(new PlanetaryObject("Debug", 2, null, PlanetaryType.Planet, PlanetType.Temperate) { Radius = 12, Gravity = 1 });
        }

        private void universeExit_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            IO.SaveUniverse(Game.UniverseViewer, IO.UniverseFile);
            ExitUniverse(false);
        }

        public void ExitUniverse(bool isMap)
        {
            Game.UniverseViewer.Dispose();
            Game.UniverseViewer = null;
            Game.CurrentGameState = Game.GameState.HomeLoggedOn;
            DeInitUniverseScreen();
            sidebar.Dispose();
            Manager.Remove(sidebar);
            sidebar = new SideBar(Manager);
            sidebar.Init();
            sidebar.StayOnBack = true;
            sidebar.Passive = true;
            sidebar.Width = ClientWidth;
            sidebar.Height = 200;
            sidebar.Anchor = Anchors.Horizontal;
            sidebar.Left = ClientLeft;
            sidebar.Top = ClientTop;
            sidebar.Draw += sidebar_Draw;
            Manager.Add(sidebar);
            InitLogo();
            InitCopyright();
            if (!isMap)
            InitMainScreen();
            (Manager.Game as Application).BackgroundImage = bg;
        }

        private static void RecalcBookmarks()
        {
            btnUniverse[5].Enabled = Game.UniverseViewer.Bookmarks.Contains(Game.UniverseViewer.SelectedPlanet);
            btnUniverse[5].Glyph.Color = MainWindow.btnUniverse[5].Enabled ? Color.White : Color.Gray;
            btnUniverse[2].Enabled = !Game.UniverseViewer.Bookmarks.Contains(Game.UniverseViewer.SelectedPlanet);
            btnUniverse[2].Glyph.Color = MainWindow.btnUniverse[2].Enabled ? Color.White : Color.Gray;
        }
        public static Label[] universeStatLabels;
        void btn_EnabledChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if ((sender as Button).Glyph != null)
                (sender as Button).Glyph.Color = (sender as Button).Enabled ? Color.White : Color.Gray;
        }
        void img_EnabledChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            (sender as Control).Color = (sender as Control).Enabled ? Color.White : Color.Gray;
        }
        public static ProgressBar prgLoading;
        public static Label lblLoading;
        public static Label lblLoadingDesc;
        private static int taskBarBlinkState = 1;
        public static void UpdateLoading(string text, float percent)
        {
            if (Game.level != null && Game.level.AutoSaving)
            {
                AutoSaveLvl.Text = text;
            }
            else
            {
                lblLoadingDesc.Text = text + "...";
                prgLoading.Value = percent * 100;

                if (Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.IsPlatformSupported)
                {
                    var prog = Microsoft.WindowsAPICodePack.Taskbar.TaskbarManager.Instance;
                    if ((int)percent == 0)
                        prog.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Normal);
                    prog.SetProgressValue((int)Math.Round(percent * 100), 100);
                    if ((int)percent == 1)
                    {
                        System.Timers.Timer t = new System.Timers.Timer(1000);
                        t.Elapsed += new ElapsedEventHandler(delegate(object o, ElapsedEventArgs e)
                        {
                            if (taskBarBlinkState % 2 != 0)
                            {
                                prog.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.Error);
                                prog.SetProgressValue(100, 100);
                            }
                            else
                            {
                                prog.SetProgressState(Microsoft.WindowsAPICodePack.Taskbar.TaskbarProgressBarState.NoProgress);
                            }
                            if (taskBarBlinkState >= 4)
                            {
                                taskBarBlinkState = 1;
                                (o as System.Timers.Timer).Stop();
                            }
                            taskBarBlinkState++;
                        });
                        t.Start();
                    }
                }
            }
        }
        public void InitGenerateScreen()
        {
            if (prgLoading != null)
            {
                Remove(prgLoading);
                Remove(lblLoading);
                Remove(lblLoadingDesc);
            }
                //Setup
                prgLoading = new ProgressBar(Manager);
                prgLoading.Init();
                prgLoading.Width = ClientWidth / 2;
                prgLoading.Height = 48;
                prgLoading.Top = (ClientHeight / 2) + 48;
                prgLoading.Left = (ClientWidth / 2) - (prgLoading.Width / 2);
                prgLoading.Color = Color.LawnGreen;
                Add(prgLoading);

                lblLoading = new Label(Manager);
                lblLoading.Init();
                lblLoading.Font = FontSize.Default32;
                lblLoading.TextChanged += label_TextChanged;
                lblLoading.Text = "";
                lblLoading.Top = prgLoading.Top - 92;
                Add(lblLoading);

                lblLoadingDesc = new Label(Manager);
                lblLoadingDesc.Init();
                lblLoadingDesc.Font = FontSize.Default14;
                lblLoadingDesc.TextChanged += label_TextChanged;
                lblLoadingDesc.Text = "0%";
                lblLoadingDesc.Top = prgLoading.Top - 36;
                Add(lblLoadingDesc);
        }

        void label_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Label l = sender as Label;
            l.Height = (int)Manager.Skin.Fonts[l.Font.ToString()].Resource.MeasureRichString(l.Text, Manager).Y;
            l.Width = (int)Manager.Skin.Fonts[l.Font.ToString()].Resource.MeasureRichString(l.Text, Manager).X;
            l.Left = (ClientWidth / 2) - (l.Width / 2);
        }

        void tbrZoom_ValueChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (Game.UniverseViewer != null)
            {
                tbrZoom.Value = (int)MathHelper.Clamp(tbrZoom.Value, UniverseViewer.MinZoom * 100, UniverseViewer.MaxZoom * 100);
                Game.UniverseViewer.newZoom = (float)tbrZoom.Value / 100;
            }
        }

        public void DeInitMainScreen()
        {
            closingtb = false;
            menuBackground.Visible = false;
            Copyright.Text = "";
            for (int i = 0; i < MenuItems.Count; i++)
            {
                Remove(MenuItems[i]);
                Remove(MenuItems[i]);
            }
        }

        void m_Closed(object sender, WindowClosedEventArgs e)
        {
            // Check dialog resule and see if we need to shut down.
            if ((sender as Dialog).ModalResult == ModalResult.Yes)
            {
                DeInitMainScreen();
                DeInitLogo();
                InitLogo();
                TaskLauncher tmp = new TaskLauncher(Manager);
                tmp.Closing += new WindowClosingEventHandler(WindowClosing);
                tmp.Closed += new WindowClosedEventHandler(WindowClosed);
                tmp.Init();
                tmp.Top += 40;
                tmp.RealTop = tmp.Top;
                CreateInfoBanner(tmp);
                Manager.Add(tmp);
                DeInitCopyright();
                InitCopyright();
                menuBackground.Visible = true;
            }

            // Unhook event handlers and dispose of the dialog.
            else
            {
                (sender as Dialog).Closed -= m_Closed;
                (sender as Dialog).Dispose();
                this.Focused = true;
            }
        }

        public static bool BoundingCircle(int x1, int y1, int radius1, int x2, int y2)
        {
            Vector2 V1 = new Vector2(x1, y1); // reference point 1
            Vector2 V2 = new Vector2(x2, y2); // reference point 2
            if (Vector2.Distance(V1, V2) < radius1) // if the distance is less than the diameter
                return true;

            return false;
        }
        public Button PencilTool, EraserTool, SquareTool, FilledSquareTool, CircleTool, FilledCircleTool, LineTool, DefaultTool;
        public TrackBar SizeTool;
        private ImageBox Divider1, Divider2;
        private void InitMapGradientPanel()
        {
            MapPanel = new GradientPanel(Manager);
            MapPanel.Init();
            MapPanel.Width = 0;
            MapPanel.Left = InventoryPanel.Left - MapPanel.Width - 8;
            MapPanel.Height = 64;
            MapPanel.Top = 16;
            MapPanel.Alpha = 0;
            absMapPanelWidth = MapPanel.Width;
            Add(MapPanel);

            PencilTool = new Button(Manager) { Top = 8, Left = 8, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\edit"]), Text = "", };
            PencilTool.Init();
            PencilTool.ToolTip.Text = "Place Tool";
            PencilTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditType = EditType.Place; SelectTool(PencilTool); });
            MapPanel.Add(PencilTool);

            EraserTool = new Button(Manager) { Top = 8, Left = PencilTool.Left + PencilTool.Width + 8, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\erase"]), Text = "", };
            EraserTool.Init();
            EraserTool.ToolTip.Text = "Erase Tool";
            EraserTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditType = EditType.Erase; SelectTool(EraserTool); });
            MapPanel.Add(EraserTool);

            SizeTool = new TrackBar(Manager) { Top = 8 + 24 + 4, Left = 8, Width = EraserTool.Left + EraserTool.Width - PencilTool.Left, Height = 24, Range = 4, Value = 0, Color = Color.SkyBlue };
            SizeTool.Init();
            SizeTool.ToolTip.Text = "Brush Size";
            MapPanel.Add(SizeTool);

            Divider1 = new ImageBox(Manager) { Top = 8, Left = SizeTool.Left + SizeTool.Width + 3, Width = 3, Height = 54, Image = ContentPack.Textures["gui\\divider"], };
            Divider1.Init();
            MapPanel.Add(Divider1);

            DefaultTool = new Button(Manager) { Top = 8, Left = EraserTool.Left + EraserTool.Width + 9, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\defaulttool"]), Text = "", };
            DefaultTool.Init();
            DefaultTool.ToolTip.Text = "Default Tool";
            DefaultTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditTool = EditTool.Default; SelectType(DefaultTool); });
            MapPanel.Add(DefaultTool);

            LineTool = new Button(Manager) { Top = 8 + 24 + 4, Left = EraserTool.Left + EraserTool.Width + 9, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\line"]), Text = "", };
            LineTool.Init();
            LineTool.ToolTip.Text = "Line Tool";
            LineTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditTool = EditTool.Line; SelectType(LineTool); });
            MapPanel.Add(LineTool);

            SquareTool = new Button(Manager) { Top = 8, Left = DefaultTool.Left + DefaultTool.Width + 8, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\square"]), Text = "", };
            SquareTool.Init();
            SquareTool.ToolTip.Text = "Bordered Rectangle Tool";
            SquareTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditTool = EditTool.Square; SelectType(SquareTool); });
            MapPanel.Add(SquareTool);

            FilledSquareTool = new Button(Manager) { Top = 8, Left = SquareTool.Left + SquareTool.Width + 8, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\filledsquare"]), Text = "", };
            FilledSquareTool.Init();
            FilledSquareTool.ToolTip.Text = "Solid Rectangle Tool";
            FilledSquareTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditTool = EditTool.FilledSquare; SelectType(FilledSquareTool); });
            MapPanel.Add(FilledSquareTool);

            CircleTool = new Button(Manager) { Top = 8 + 24 + 4, Left = DefaultTool.Left + DefaultTool.Width + 8, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\circle"]), Text = "", };
            CircleTool.Init();
            CircleTool.ToolTip.Text = "Bordered Circle Tool";
            CircleTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditTool = EditTool.Circle; SelectType(CircleTool); });
            MapPanel.Add(CircleTool);

            FilledCircleTool = new Button(Manager) { Top = 8 + 24 + 4, Left = CircleTool.Left + CircleTool.Width + 8, Width = 24, Height = 24, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\filledcircle"]), Text = "", };
            FilledCircleTool.Init();
            FilledCircleTool.ToolTip.Text = "Solid Circle Tool";
            FilledCircleTool.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            { Game.level.EditTool = EditTool.FilledCircle; SelectType(FilledCircleTool); });
            MapPanel.Add(FilledCircleTool);

            Divider2 = new ImageBox(Manager) { Top = 8, Left = FilledCircleTool.Left + CircleTool.Width + 3, Width = 3, Height = 54, Image = ContentPack.Textures["gui\\divider"], };
            Divider2.Init();
            MapPanel.Add(Divider2);

            SandboxInventory = new Button(Manager) { Top = 8, Left = FilledCircleTool.Left + CircleTool.Width + 9, Width = 24, Height = 24 + 24 + 4, Glyph = new Glyph(ContentPack.Textures["gui\\icons\\blocks"]) { SizeMode = SizeMode.Centered }, Text = "", };
            SandboxInventory.Init();
            SandboxInventory.ToolTip.Text = "Item List";
            SandboxInventory.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                ItemBoxWindow.Show();
                SandboxInventory.Enabled = false;
            });
            MapPanel.Add(SandboxInventory);

            SelectTool(PencilTool);
            SelectType(DefaultTool);

            ItemBoxWindow = new TaskItemBox(Manager);
            ItemBoxWindow.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
            ItemBoxWindow.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
            ItemBoxWindow.Init();
            Manager.Add(ItemBoxWindow);
            ItemBoxWindow.Hide();
            
        }
        private void SelectType(Button Tool)
        {
            SquareTool.Color = FilledSquareTool.Color = CircleTool.Color = FilledCircleTool.Color = LineTool.Color = DefaultTool.Color = Color.White;
            Tool.Color = Color.Black;
        }
        private void SelectTool(Button Tool)
        {
            EraserTool.Color = PencilTool.Color = Color.White;
            Tool.Color = Color.Black;
        }
        ////////////////////////////////////////////////////////////////////////////   
        private void InitSecondaryGradientPanel()
        {
            tlbrSecondary = new GradientPanel(Manager);
            tlbrSecondary.Init();
            tlbrSecondary.Width = tlbrPrimary.Width;
            tlbrSecondary.Left = ClientWidth - 16 - tlbrSecondary.ClientWidth;
            tlbrSecondary.Height = 64;
            tlbrSecondary.Top = 16;
            Add(tlbrSecondary);

            btnScreenshot = new Button(Manager);
            btnScreenshot.Init();
            btnScreenshot.Width = 24;
            btnScreenshot.Height = 24;
            btnScreenshot.Top = 6;
            btnScreenshot.Text = "";
            btnScreenshot.ToolTip.Text = "Take a screenshot";
            btnScreenshot.Click += btnScreenshot_Click;
            btnScreenshot.Left = tlbrSecondary.ClientWidth - 24 - 6;
            tlbrSecondary.Add(btnScreenshot);
            imgScreenshot = new ImageBox(Manager);
            imgScreenshot.Init();
            imgScreenshot.Passive = true;
            imgScreenshot.SizeMode = SizeMode.Auto;
            imgScreenshot.Image = txScreenshot;
            imgScreenshot.Width = 16;
            imgScreenshot.Height = 16;
            imgScreenshot.Top = 4;
            imgScreenshot.Left = 4;
            btnScreenshot.CanFocus = false;
            btnScreenshot.Add(imgScreenshot);

            btnOptions = new Button(Manager);
            btnOptions.Init();
            btnOptions.Width = 24;
            btnOptions.Height = 24;
            btnOptions.Top = 6;
            btnOptions.Text = "";
            btnOptions.Left = tlbrSecondary.ClientWidth - (24 * 2) - (6 * 2);
            btnOptions.Click += btnOptions_Click;
            btnOptions.ToolTip.Text = "Edit Settings";
            tlbrSecondary.Add(btnOptions);
            imgOptions = new ImageBox(Manager);
            imgOptions.Init();
            imgOptions.SizeMode = SizeMode.Auto;
            imgOptions.Image = txOptions;
            imgOptions.Width = 16;
            imgOptions.Height = 16;
            imgOptions.Top = 4;
            imgOptions.Left = 4;
            imgOptions.Passive = true;
            btnOptions.CanFocus = false;
            btnOptions.Add(imgOptions);


            btnHome = new Button(Manager);
            btnHome.Init();
            btnHome.Width = 24;
            btnHome.Height = 24;
            btnHome.Top = 6;
            btnHome.Text = "";
            btnHome.ToolTip.Text = "Save and quit to home screen.";
            btnHome.Left = tlbrSecondary.ClientWidth - (24 * 3) - (6 * 3);
            btnHome.CanFocus = false;
            btnHome.Click += btnHome_Click;
            tlbrSecondary.Add(btnHome);
            imgHome = new ImageBox(Manager);
            imgHome.Init();
            imgHome.Passive = true;
            imgHome.SizeMode = SizeMode.Auto;
            imgHome.Image = txHome;
            imgHome.Width = 16;
            imgHome.Height = 16;
            imgHome.Top = 4;
            imgHome.Left = 4;
            btnHome.Add(imgHome);

            btnCrafting = new Button(Manager);
            btnCrafting.Init();
            btnCrafting.Width = 24;
            btnCrafting.Height = 24;
            btnCrafting.Top = 34;
            btnCrafting.Text = "";
            btnCrafting.ToolTip.Text = "Open crafting interface.";
            btnCrafting.Left = tlbrSecondary.ClientWidth - 24 - 6;
            btnCrafting.Click += btnCrafting_Click;
            btnCrafting.CanFocus = false;
            tlbrSecondary.Add(btnCrafting);
            imgCrafting = new ImageBox(Manager);
            imgCrafting.Init();
            imgCrafting.SizeMode = SizeMode.Auto;
            imgCrafting.Image = txCrafting;
            imgCrafting.Width = 16;
            imgCrafting.Height = 16;
            imgCrafting.Top = 4;
            imgCrafting.Left = 4;
            imgCrafting.Passive = true;
            btnCrafting.Add(imgCrafting);

            btnGamemode = new Button(Manager);
            btnGamemode.Init();
            btnGamemode.Width = 24;
            btnGamemode.Height = 24;
            btnGamemode.Top = 34;
            btnGamemode.Text = "";
            btnGamemode.ToolTip.Text = "Switch Gamemode (Sandbox Maps Only)";
            btnGamemode.Click += btnGamemode_Click;
            btnGamemode.Left = tlbrSecondary.ClientWidth - (24 * 2) - (6 * 2);
            btnGamemode.CanFocus = false;
            btnGamemode.Enabled = Game.level.IsMap;
            tlbrSecondary.Add(btnGamemode);
            imgGamemode = new ImageBox(Manager);
            imgGamemode.Init();
            imgGamemode.Passive = true;
            imgGamemode.SizeMode = SizeMode.Auto;
            imgGamemode.Image = txGamemode;
            imgGamemode.Width = 16;
            imgGamemode.Height = 16;
            imgGamemode.Top = 4;
            imgGamemode.Left = 4;
            btnGamemode.Add(imgGamemode);

            btnAchievement = new Button(Manager);
            btnAchievement.Init();
            btnAchievement.Width = 24;
            btnAchievement.Height = 24;
            btnAchievement.Top = 34;
            btnAchievement.Text = "";
            btnAchievement.ToolTip.Text = "Open achievement log.";
            btnAchievement.Left = tlbrSecondary.ClientWidth - (24 * 3) - (6 * 3);
            btnAchievement.Click += btnAchievement_Click;
            btnAchievement.CanFocus = false;
            tlbrSecondary.Add(btnAchievement);
            imgAchievement = new ImageBox(Manager);
            imgAchievement.Init();
            imgAchievement.Passive = true;
            imgAchievement.SizeMode = SizeMode.Auto;
            imgAchievement.Image = txAchievement;
            imgAchievement.Width = 16;
            imgAchievement.Height = 16;
            imgAchievement.Top = 4;
            imgAchievement.Left = 4;
            btnAchievement.Add(imgAchievement);

        }

        void btnHome_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            ConfirmLevelQuit();
        }

        public void ConfirmLevelQuit()
        {
            MessageBox m = new MessageBox(Manager, MessageBoxType.YesNo, "Are you sure you would like to quit?", "Save and Quit");
            m.Init();
            m.buttons[0].Text = "Yes, Save and Quit";
            m.buttons[0].Width = 130;
            m.buttons[0].Left = (m.ClientWidth / 2) - m.buttons[0].Width - 4;
            m.buttons[1].Text = "No, Continue Playing";
            m.buttons[1].Width = 130;
            m.StayOnTop = true;
            m.Closed += new WindowClosedEventHandler(delegate(object o, WindowClosedEventArgs ev)
            {
                // Check dialog resule and see if we need to shut down.
                if ((o as Dialog).ModalResult == ModalResult.Yes)
                {
                    //Remove any death windows
                    List<Control> DeathWindows = Game.level.game.Manager.Controls.Where(x => x is TaskDeath).ToList();
                    foreach (Control c in DeathWindows)
                    {
                        (c as TaskDeath).Close();
                        Game.level.game.Manager.Remove(c);
                    }
                    DeInitGameScreen();
                    InitUniverseScreen();
                    Game.CurrentGameState = Game.GameState.UniverseViewer;
                    (Manager.Game as Game).OpenUniverseViewer();
                    Game.UniverseViewer.Save(Game.level);
                }
            });
            m.ShowModal();
            Manager.Add(m);
            m.BringToFront();
        }

        void btnCrafting_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CraftingWindow.Show(Item.Blank);
        }

        void btnAchievement_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TaskAchievementLog AchievementLog = new TaskAchievementLog(Game.level.game.Manager);
            AchievementLog.Closed += AchievementLog_Closed;
            AchievementLog.Init();
            Game.level.game.Manager.Add(AchievementLog);
            btnAchievement.Enabled = false;
        }

        void AchievementLog_Closed(object sender, WindowClosedEventArgs e)
        {
            if (btnAchievement != null)
            btnAchievement.Enabled = true;
        }

        void btnScreenshot_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            IO.Screenie((Manager.Game as ZarknorthClient.Game));
        }

        void btnGamemode_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
#if !DEBUG
            if (Game.level.IsMap)
            {
#endif
            if (Game.level.Gamemode == GameMode.Survival)
            {
                Game.level.Gamemode = GameMode.Sandbox;
                sandBoxFadingIn = true;
                sandBoxFadingOut= false;
            }
            else if (Game.level.Gamemode == GameMode.Sandbox)
            {
                Game.level.Gamemode = GameMode.Survival;
                sandBoxFadingOut = true;
                sandBoxFadingIn = false;
            }
            #if !DEBUG
            }
#endif
        }

        void btnOptions_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TaskOptions Task = new TaskOptions(Manager);
            Task.Closing += new WindowClosingEventHandler(WindowClosing);
            Task.Closed += new WindowClosedEventHandler(WindowClosed);
            Task.Closed += Task_Closed;
            Task.Init();
            Manager.Add(Task);
            btnOptions.Enabled = false;
        }

        void Task_Closed(object sender, WindowClosedEventArgs e)
        {
            btnOptions.Enabled = true;
        }
        public void InitPrimaryGradientPanel()
        {
            tlbrPrimary = new GradientPanel(Manager);
            tlbrPrimary.Init();

            imgHealth = new ImageBox(Manager);
            imgHealth.Init();
            imgHealth.Top = 4;
            imgHealth.Left = 6;
            imgHealth.Width = 16;
            imgHealth.Height = 16;
            imgHealth.Image = txHealth;
            tlbrPrimary.Add(imgHealth);

            imgEnergy = new ImageBox(Manager);
            imgEnergy.Init();
            imgEnergy.Top = imgHealth.Top + imgHealth.Height + 4;
            imgEnergy.Left = imgHealth.Left;
            imgEnergy.Width = 16;
            imgEnergy.Height = 16;
            imgEnergy.Image = txEnergy;
            tlbrPrimary.Add(imgEnergy);

            prgHealth = new ProgressBar(Manager);
            prgHealth.Init();
            prgHealth.Top = imgHealth.Top;
            prgHealth.Left = imgHealth.Width + imgHealth.Left + 4;
            prgHealth.Width = 66;
            prgHealth.Height = 16;
            tlbrPrimary.Add(prgHealth);

            prgEnergy = new ProgressBar(Manager);
            prgEnergy.Init();
            prgEnergy.Top = prgHealth.Top + prgHealth.Height + 4;
            prgEnergy.Left = imgHealth.Width + imgHealth.Left + 4;
            prgEnergy.Width = 66;
            prgEnergy.Height = 16;



            tlbrPrimary.Add(prgEnergy);

            imgMoney = new ImageBox(Manager);
            imgMoney.Init();
            imgMoney.Top = imgEnergy.Top + imgEnergy.Height + 4;
            imgMoney.Left = 6;
            imgMoney.Width = 16;
            imgMoney.Height = 16;
            imgMoney.Image = txMoney;
            tlbrPrimary.Add(imgMoney);
            lblMoney = new Label(Manager);
            lblMoney.Init();
            lblMoney.Top = prgEnergy.Top + prgEnergy.Height + 6;
            lblMoney.Left = imgMoney.Left + imgMoney.Width + 6;
            lblMoney.Width = 100;
            lblMoney.Height = 12;
            lblMoney.Text = "0";
            tlbrPrimary.Add(lblMoney);

            tlbrPrimary.Width = 96;
            tlbrPrimary.Left = 16;
            tlbrPrimary.Height = 64;
            tlbrPrimary.Top = 16;
            Add(tlbrPrimary);

        }

        ////////////////////////////////////////////////////////////////////////////
        public void InitMinimap()
        {
            imgMinimap = new ImageBox(Manager);
            imgMinimap.Init();
            imgMinimap.Parent = minimapPnl;
            imgMinimap.Top = 0;
            imgMinimap.Left = 8;
            Add(imgMinimap);
        }


        ////////////////////////////////////////////////////////////////////////////   
        public void InitInventory()
        {
            InventoryPanel = new GradientPanel(Manager);
            InventoryPanel.Init();
            InventoryPanel.Passive = false;
            InventoryPanel.Height = 8 + 8 + 48;
            InventoryPanel.Width = 8 + ((48 + 8) * 10);
            InventoryPanel.Top = 16;
            InventoryPanel.Left = (ClientWidth / 2) - (InventoryPanel.Width / 2);
            InventoryPanel.StayOnTop = true;
            btnsInventory = new Button[60];
            imgsInventory = new ImageBox[60];
            lblsInventory = new Label[60];

            #region Old Stuff, WILL BE NEEDED SOMEDAY for tech tree / skill progression
            #region TechTree
            //Variables for tech tree
            int AmountOfItems = 0;
            const int TechButtonWidth = 96;
            const int TechButtonHeight = 32;
            const int TechButtonSpacing = 16;
            const int TechButtonOffsetLeft = 8;
            const int TechButtonOffsetTop = 8;

            foreach (Item item in Item.ItemList) //Count how many items we have matching a certain criteria
                if (item is MeleeWepItem)
                    AmountOfItems++;

            btnsTech = new Button[AmountOfItems]; //Create the technology button array

            int j = 0; //Keep track of our position within the tech tree array - btnsTech
            foreach (Item item in Item.ItemList) //For all items we have matching a certain criteria
            {
                if (item is MeleeWepItem)
                {
                    //Create the new button
                    btnsTech[j] = new Button(Manager);
                    btnsTech[j].Init();
                    btnsTech[j].Top = TechButtonOffsetTop + ((item.TechTreePosition.Y * TechButtonHeight) + (item.TechTreePosition.Y * TechButtonSpacing)) - (TechButtonHeight / 2);
                    btnsTech[j].Left = TechButtonOffsetLeft + ((item.TechTreePosition.X * TechButtonWidth) + (item.TechTreePosition.X * TechButtonSpacing)) - (TechButtonWidth / 2);
                    btnsTech[j].Width = TechButtonWidth;
                    btnsTech[j].Height = TechButtonHeight;
                    btnsTech[j].Text = item.Name;
                    btnsTech[j].Glyph = new Glyph(ContentPack.Textures["items\\" + item.Name], new Rectangle(0, 0, 24, 24));
                    //Add it to the crafting/tech tree page
                    //  tabInventory.TabPages[1].Add(btnsTech[j]); 

                    j++; //Increment to keep track of where we are in the array - btnsTech
                }
            }
            foreach (Item item in Item.ItemList) //For all items we have matching a certain criteria
            {
                if (item is MeleeWepItem)
                {
                    if (item.Child != null && item.Child.childeren.Length > 0) //If this item has children items
                    {
                        foreach (int child in item.Child.childeren)
                        {

                            int parent = 0;
                            int child1 = 0;
                            for (int i = 0; i < btnsTech.Length; i++) //Search the array of buttons for the correct one
                            {
                                if (btnsTech[i].Text == item.Name)
                                    parent = i;
                                if (btnsTech[i].Text == Item.FindItem(child).Name)
                                    child1 = i;
                            }
                            //Add the line in between the 2 buttons
                            //First line, "squared look"
                            if (btnsTech[parent].Top == TechButtonOffsetTop + ((1 * TechButtonHeight) + (1 * TechButtonSpacing)) - (TechButtonHeight / 2))
                            {
                                techPoints.Add(new Tuple<Point, Point>(new Point(btnsTech[parent].Left + (TechButtonWidth / 2), btnsTech[parent].Top + TechButtonHeight), new Point(btnsTech[child1].Left + (TechButtonWidth / 2), btnsTech[parent].Top + TechButtonHeight)));
                                techPoints.Add(new Tuple<Point, Point>(new Point(btnsTech[child1].Left + (TechButtonWidth / 2), btnsTech[parent].Top + TechButtonHeight), new Point(btnsTech[child1].Left + (TechButtonWidth / 2), btnsTech[child1].Top + TechButtonHeight)));
                            }
                            else //Normal diagnol/straight
                                techPoints.Add(new Tuple<Point, Point>(new Point(btnsTech[parent].Left + (TechButtonWidth / 2), btnsTech[parent].Top + TechButtonHeight), new Point(btnsTech[child1].Left + (TechButtonWidth / 2), btnsTech[child1].Top + TechButtonHeight)));
                        }
                    }
                }
            }
            #endregion
            ////Start TechTree
            //int AmountOfItems = 1;
            //int tiers = 3;

            //    foreach (Item item in Item.Items) //Check all items and count
            //    {
            //        if (item.Type == ItemType.WeaponMelee)
            //        {
            //            if (item.Child != null)
            //            foreach (Item child in item.Child.childeren)
            //            {

            //                AmountOfItems++;
            //            }

            //        }

            //    }

            // btnsTech =  new Button[AmountOfItems];
            //int j = 0;
            //int k = 0;
            // for (int teir = 1; teir <= tiers; teir++) //For every tier
            // {
            //     int AmountInTeir = 0;
            //     foreach (Item item in Item.Items) //Check all items and count
            //     {
            //         if (item.Type == ItemType.WeaponMelee && item.Teir == teir)
            //         {
            //         AmountInTeir++;
            //         }
            //     }
            //     foreach (Item item in Item.Items) //Check all items and count
            //     {
            //         if (item.Type == ItemType.WeaponMelee && item.Teir == teir)
            //         {
            //             btnsTech[j] = new Button(Manager);
            //             btnsTech[j].Init();
            //             btnsTech[j].Top = (32 + 8) * teir;
            //             btnsTech[j].Width = 96;
            //             btnsTech[j].Height = 32;
            //             btnsTech[j].btnLeft = (tabInventory.Width / (AmountInTeir + 1)) *  (k + 1)-( btnsTech[j].Width / 2); //horizontal positioning
            //             btnsTech[j].Text = item.Name;
            //             tabInventory.TabPages[1].Add(btnsTech[j]);
            //             k++;
            //                 j++;
            //         }
            //     }
            //     k= 0;
            // } 
            //foreach (Item item in Item.Items) //Check all items and count
            // {
            //     if (item.Type == ItemType.WeaponMelee)
            //     {
            //         if (item.Child != null && item.Child.childeren.Length > 0)
            //         {
            //             int l = 0;
            //             foreach (Item child in item.Child.childeren)
            //             {

            //                 int parent = 0;
            //                 int child1 = 0;
            //                 for (int i = 0; i < btnsTech.Length; i++)
            //                 {
            //                     if (btnsTech[i].Text == item.Name)
            //                         parent = i;
            //                     if (btnsTech[i].Text == child.Name)
            //                         child1 = i;
            //                 }
            //                 l++;
            //                 techPoints.Add(new Vector2(btnsTech[parent].btnLeft + 48, btnsTech[parent].Top+32+ l), new Vector2(btnsTech[child1].btnLeft+48, btnsTech[child1].Top + 32));
            //             }
            //         }
            //     }
            // }

            InventoryPanel.Draw += new DrawEventHandler(delegate(object o, DrawEventArgs a)
            {
                //if (tabInventory.SelectedIndex == 1)
                //{
                //    foreach (DoublePoint p in techPoints)
                //        Manager.Renderer.SpriteBatch.DrawLineArrow(Manager.Skin.Images["Shared.ArrowDown"].Resource,p.Point1.ToVector2(), p.Point2.ToVector2(), Color.LightGray, 1);
                //}

            });
            //End TechTree
            //for (int i = 0; i < 50; i++)
            //{
            //    int level = (i / 10) - 1;
            //   btnsInventory[i] = new Button(Manager);
            //   btnsInventory[i].Init();
            //   btnsInventory[i].Width = 48;
            //   btnsInventory[i].Height = 48;
            //   btnsInventory[i].Top = 8+ ((48 + 8) * (level + 1));
            //   btnsInventory[i].btnLeft = 8 + (i - (10 * (level + 1))) * (btnsInventory[i].Width + 8);
            //   btnsInventory[i].Text = "";
            //   btnsInventory[i].ToolTip.Text = "";
            //   btnsInventory[i].Click += new TomShane.Neoforce.Controls.EventHandler(inventory_Click);
            //   btnsInventory[i].MouseUp += new MouseEventHandler(inventory_MousePress);

            //   lblsInventory[i] = new lblName(Manager);
            //   lblsInventory[i].Init();
            //   lblsInventory[i].Width = btnsInventory[0].Width;
            //   lblsInventory[i].Height = 10;
            //   lblsInventory[i].Top = btnsInventory[i].Top + 37;
            //   lblsInventory[i].btnLeft = btnsInventory[i].btnLeft + 8;
            //   lblsInventory[i].Text = "";
            //   lblsInventory[i].Passive = true;
            //   lblsInventory[i].StayOnTop = true;

            //   imgsInventory[i] = new ImageBox(Manager);
            //   imgsInventory[i].Init();
            //   imgsInventory[i].Width = 24;
            //   imgsInventory[i].Height = 24;
            //   imgsInventory[i].Top = btnsInventory[i].Top +8;
            //   imgsInventory[i].btnLeft = btnsInventory[i].btnLeft + 8;
            //   imgsInventory[i].Text = "";
            //   imgsInventory[i].Passive = true;

            //   imgsInventory[i].SizeMode = SizeMode.Stretched;

            //   tabInventory.Add(btnsInventory[i]);
            //   tabInventory.Add(imgsInventory[i]);
            //   tabInventory.Add(lblsInventory[i]);

            //}
            //for (int i = 50; i < 60; i++)
            //{
            //    i -= 50;
            //    int level = (i  / 3) - 1;
            //    btnsInventory[i + 50] = new Button(Manager);
            //    btnsInventory[i + 50].Init();
            //    btnsInventory[i + 50].Width = 48;
            //    btnsInventory[i + 50].Height = 48;
            //   if (i ==9)
            //    btnsInventory[i + 50].Top = 350 + ((48 + 8) * (1));
            //   else
            //       btnsInventory[i + 50].Top = 350 + ((48 + 8) * (level + 1));
            //   if (i == 9)
            //       btnsInventory[i + 50].btnLeft = 16 + 48 + (3 * (btnsInventory[i + 50].Width + 8));
            //   else

            //    btnsInventory[i + 50].btnLeft = 16 + (i - (3 * (level + 1))) * (btnsInventory[i + 50].Width + 8);
            //    btnsInventory[i + 50].Text = "";
            //    btnsInventory[i + 50].ToolTip.Text = "";
            //    btnsInventory[i + 50].Click += new TomShane.Neoforce.Controls.EventHandler(inventory_Click);
            //    btnsInventory[i + 50].MouseUp += new MouseEventHandler(inventory_MousePress);

            //    imgsInventory[i + 50] = new ImageBox(Manager);
            //    imgsInventory[i + 50].Init();
            //    imgsInventory[i + 50].Width = 24;
            //    imgsInventory[i + 50].Height = 24;
            //    imgsInventory[i + 50].Top = btnsInventory[i + 50].Top + 8;
            //    imgsInventory[i + 50].btnLeft = btnsInventory[i + 50].btnLeft + 8;
            //    imgsInventory[i + 50].Text = "";
            //    imgsInventory[i + 50].Passive = true;
            //    imgsInventory[i + 50].Click += new TomShane.Neoforce.Controls.EventHandler(inventory_Click);
            //    imgsInventory[i + 50].MouseUp += new MouseEventHandler(inventory_MousePress);
            //    imgsInventory[i + 50].SizeMode = SizeMode.Stretched;

            //    lblsInventory[i + 50] = new lblName(Manager);
            //    lblsInventory[i + 50].Init();
            //    lblsInventory[i + 50].Width = btnsInventory[0 ].Width;
            //    lblsInventory[i + 50].Height = 10;
            //    lblsInventory[i + 50].Top = btnsInventory[i + 50].Top + 37;
            //    lblsInventory[i + 50].btnLeft = btnsInventory[i + 50].btnLeft + 8;
            //    lblsInventory[i + 50].Text = "";
            //    lblsInventory[i + 50].Passive = true;
            //    lblsInventory[i + 50].StayOnTop = true;

            //    tabInventory.Add(btnsInventory[i + 50]);
            //    tabInventory.Add(imgsInventory[i + 50]);
            //    tabInventory.Add(lblsInventory[i + 50]);
            //    i += 50;
            //}
            #endregion

            inventory = new SlotContainer(Manager, 10, 5);
            inventory.Init();
            inventory.ItemSlots = Game.level.Players[0].Inventory;
            inventory.Left = 8;
            inventory.Top = 8;
            inventory.MoveItems += inventory_MoveItem;
            inventory.SelectItem += inventory_SelectItem;
            inventory.DeSelectItem += inventory_DeSelectItem;
            inventory.ShiftClickItems += inventory_ShiftClickItems;
            InventoryPanel.Add(inventory);
            Add(InventoryPanel);
            sidebar.Height = InventoryPanel.Height + InventoryPanel.Top + 16;
            sidebar.SendToBack();
            Interface.MainWindow.InventoryHeight = InventoryClosedHeight = InventoryPanel.Height;
            inventory.Height = InventoryClosedHeight - 16;
            InventoryPanel.Passive = false;
            Interface.MainWindow.inventory.button_MouseDown(inventory.Slots[0, 0].button, null);
            InventoryOpenHeight = 8 + ((48 + 8) * 5);

        }

        void inventory_SelectItem(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Interface.MainWindow.inventory.ItemSlots[Interface.MainWindow.inventory.Selected].Item.OnSelect(new SelectItemEventArgs(Game.level, Interface.MainWindow.inventory.Selected));
        }
        void inventory_DeSelectItem(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Interface.MainWindow.inventory.ItemSlots[Interface.MainWindow.inventory.Selected].Item.OnDeSelect(new SelectItemEventArgs(Game.level, Interface.MainWindow.inventory.Selected));
        }

        void inventory_ShiftClickItems(Slot slot)
        {
            foreach (Control c in Manager.Controls)
            {
                if (c is TaskStorage)
                {
                    for (int i = 0; i < (c as TaskStorage).slotContainer.ItemSlots.Count(); i++)
                    {
                        if ((c as TaskStorage).slotContainer.ItemSlots[i].Equals(Slot.Empty))
                        {
                            Slot s = (c as TaskStorage).slotContainer.ItemSlots[i];
                            (c as TaskStorage).slotContainer.ItemSlots[i] = (Slot)slot.Clone();
                            inventory.ItemSlots[inventory.Selected] = (Slot)s.Clone();
                            (c as TaskStorage).slotContainer.Refresh();
                            return;
                        }
                    }
                }
            }
        }

        public static void inventory_MoveItem(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (Interface.MainWindow.CraftingWindow != null && Interface.MainWindow.CraftingWindow.Visible)
            {
                Interface.MainWindow.CraftingWindow.UpdateItemPanel(sender);
                Interface.MainWindow.CraftingWindow.UpdateItemList(sender);
            }

        }
        public static List<DebugLabel> DebugList;
        public static List<Label> StatList;
        public static Label DebugLabel;
        public void InitConsole()
        {
            MainTabs = new TabControl(Manager);
            MainTabs.FocusLost += tbc_FocusLost;
            MainTabs.FocusGained += tbc_FocusGained;
            MainTabs.Click += tbc_Click;
            Console = new TomShane.Neoforce.Controls.Console(Manager);
            Console.FocusLost += tbc_FocusLost;
            Console.FocusGained += tbc_FocusGained;
            Console.Click += tbc_Click;
            Console.txtMain.Click += tbc_Click;

            UserList = new ListBox(Manager);

            MainTabs.Init();
            MainTabs.AddPage("#Chat");
            MainTabs.AddPage("#Stats");

            MainTabs.Alpha = 128;
            MainTabs.Left = 20;
            MainTabs.Height = 185;
            MainTabs.Width = 530;
            MainTabs.Top = Height - MainTabs.Height - 20;
            MainTabs.Movable = true;
            MainTabs.Resizable = true;
            MainTabs.Focused = false;
            MainTabs.MinimumHeight = 96;
            MainTabs.MinimumWidth = 200;

            MainTabs.TabPages[0].Add(Console);
            MainTabs.TabPages[0].Add(UserList);


            Console.Init();
            Console.Width = MainTabs.Width - 100;
            Console.Height = MainTabs.ClientHeight;

            UserList.Init();
            UserList.Left = Console.Width + Console.Left + 1;
            UserList.Top = Console.Top;
            UserList.Width = 100;
            UserList.Height = Console.Height;

            MainTabs.ClientWidth = UserList.Width + Console.Width;

            UserList.Anchor = Anchors.Right | Anchors.Top | Anchors.Bottom | Anchors.Vertical;
            Console.Channels.Add(new ConsoleChannel(0, "#Global", Color.White));
            Console.Channels.Add(new ConsoleChannel(1, "#Private", Color.Gray));
            Console.Channels.Add(new ConsoleChannel(2, "#System", Color.Gold));
            Console.ChannelsVisible = false;

            Console.ChannelFilter.Add(0);
            Console.ChannelFilter.Add(2);

            // Select default channels for each tab
            Console.SelectedChannel = 0;

            // Do we want to add timestamp or channel name at the start of every message?
            Console.MessageFormat = ConsoleMessageFormats.ChannelName;

            // Handler for altering incoming message
            Console.MessageSent += new ConsoleMessageEventHandler(con1_MessageSent);

            MainWindow.Console.MessageBuffer.Add(new ConsoleMessage("Welcome to [color:Gold]Zarknorth![/color] [color:MediumPurple]Destroy ~ Defend ~ Discover[/color]", 2, Color.White));

            Manager.Add(MainTabs);

            DebugLabel = new Label(Manager);
            DebugLabel.Init();
            DebugLabel.Top = 8;
            DebugLabel.Left = 8;
            DebugLabel.Width = MainTabs.ClientWidth - 16;
            MainTabs.TabPages[1].Add(DebugLabel);

            DebugList = new List<DebugLabel>();

            for (int i = 0; i < 16; i++)
            {
                DebugLabel lbl = new DebugLabel(Manager);
                lbl.Init();
                lbl.Left = 8;
                lbl.Top = 24 + (i * lbl.Height);
                if (i >= 8)
                {
                    lbl.Left += 150;
                    lbl.Top = 24 + ((i - 8) * lbl.Height);
                }
                lbl.Width = 200;
                DebugList.Add(lbl);
                MainTabs.TabPages[1].Add(DebugList[i]);
            }

            DebugList[0].Name = "Update";
            DebugList[1].Name = "  -Level";
            DebugList[2].Name = "Draw";
            DebugList[3].Name = "  -Level";
            DebugList[4].Name = "Render Tiles";
            DebugList[5].Name = "Lighting";
            DebugList[6].Name = "Liquid";
            DebugList[7].Name = "Animation";
            DebugList[8].Name = "Fire";
            DebugList[9].Name = "Falling Tiles";
            DebugList[10].Name = "Update Particles";
            DebugList[11].Name = "Draw Particles";
            DebugList[12].Name = "CalcNoDraw";
            DebugList[13].Name = "---";

            StatList = new List<Label>();

            for (int i = 0; i < 10; i++)
            {
                Label lbl = new Label(Manager);
                lbl.Init();
                lbl.Left = 300;
                lbl.Top = 24 + (i * lbl.Height);
                lbl.Width = 200;
                lbl.Text = string.Empty;
                StatList.Add(lbl);
                MainTabs.TabPages[1].Add(StatList[i]);
            }
        }

        void tbc_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            MainTabs.Focused = true;
        }

        private void tbc_FocusLost(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Console.Focused = false;
            Console.txtMain.Focused = false;
            MainTabs.Focused = false;
            this.Focused = true;
            Manager.FocusedControl = this;
            MainTabs.Alpha = 100;
            Console.txtMain.Enabled = false;
            Console.txtMain.Enabled = true;
        }

        void tbc_FocusGained(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Console.txtMain.Focused = true;
            MainTabs.Alpha = 220;
        }
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        void con1_MessageSent(object sender, ConsoleMessageEventArgs e)
        {
            if (e.Message.Text.StartsWith("/"))
            {
                ConsoleMessage c = Command.ProcessCommand(e.Message.Text.Substring(1, e.Message.Text.Length - 1));
                string[] texts = c.Text.Split(new string[1] { TomShane.Neoforce.Controls.Manager.StringNewline }, StringSplitOptions.None);
                ConsoleMessage[] msgs = new ConsoleMessage[texts.Length];

                for (int i = 0; i < texts.Length; i++)
                {
                    string str = texts[i];
                    msgs[i] = new ConsoleMessage(str, c.Channel, c.Color);
                    msgs[i].NoShow = i > 0;
                    if (!string.IsNullOrWhiteSpace(msgs[i].Text))
                        Console.MessageBuffer.Add(msgs[i]);
                }
            }
            else
            {
                ConsoleMessage c = new ConsoleMessage(Game.UserName + ": " + e.Message.Text, 0);
                string[] texts = c.Text.Split(new string[1] { TomShane.Neoforce.Controls.Manager.StringNewline }, StringSplitOptions.None);
                ConsoleMessage[] msgs = new ConsoleMessage[texts.Length];

                for (int i = 0; i < texts.Length; i++)
                {
                    string str = texts[i];
                    msgs[i] = new ConsoleMessage(str, c.Channel, c.Color);
                    msgs[i].NoShow = i > 0;
                    if (!string.IsNullOrWhiteSpace(msgs[i].Text))
                        Console.MessageBuffer.Add(msgs[i]);
                }
            }
            tbc_FocusLost(MainTabs, null);
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        public void ResetScrollbar()
        {
            this.VerticalScrollBar.Value = 100;
        }
    }
}

