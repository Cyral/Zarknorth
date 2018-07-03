
#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace ZarknorthClient.Interface
{
    public class TaskSandbox: Dialog
    {
        #region Fields
        string link = "http://www.zarknorth.com/maps";
        MainWindow MainWindow;
        public ControlList<MapListItem> MapList;
        Label Map, Author, Description, Version, LastSaved, Size, Seed, NameLbl, SizeLbl, SeedLbl, DescriptionLbl, GeneratorLbl;
        Button Delete, Rename, Open, Generate;
        ComboBox GeneratorCmb, SizeCmb;
        TextBox NameInput, DescriptionInput, SeedInput;
        Button OpenFolder, VisitSite, Exit;
        #endregion

        #region Constructors
        public TaskSandbox(Manager manager, MainWindow window) : base(manager)
        {
            MainWindow = window;
            Resizable = false;
            Movable = false;
            CaptionVisible = false;
            TopPanel.Visible = false;
            BottomPanel.Visible = false;
            if (MainWindow.prgLoading != null)
            MainWindow.prgLoading.Visible = MainWindow.lblLoading.Visible = MainWindow.lblLoadingDesc.Visible = false;

            Top = MainWindow.sidebar.Height + 16;
            Width = 868;
            Left = (MainWindow.ClientWidth / 2) - (Width / 2);
            Height = MainWindow.ClientHeight - Top - 16;
            GroupPanel MapPanel = new GroupPanel(manager) { Text = "Map List", Left = 8, Top = 8, Width = 316, Height = ClientHeight - 16, };
            MapPanel.Init();
            Add(MapPanel);
            GroupPanel JoinPanel = new GroupPanel(manager) { Text = "Open Map", Left = MapPanel.Left + MapPanel.Width + 8, Top = 8, Width = 512, Height = (ClientHeight / 2) - 16, };
            JoinPanel.Init();
            Add(JoinPanel);
            GroupPanel CreatePanel = new GroupPanel(manager) { Text = "Create Map", Left = JoinPanel.Left, Top = (ClientHeight / 2) + 8, Width = 512, Height = (ClientHeight / 2) - 16, };
            CreatePanel.Init();
            Add(CreatePanel);

            MapList = new ControlList<MapListItem>(Manager);
            MapList.Init();
            MapList.Left = 8;
            MapList.Top = 8;
            MapList.Width = 300;
            MapList.Height = MapPanel.ClientHeight - 16;
            MapList.ItemIndexChanged += MapList_ItemIndexChanged;
            MapPanel.Add(MapList);
            List();

            Map = new Label(manager) { Text = "No Maps Found", Left = 8, Top = 8, Font = FontSize.Default14, Width = JoinPanel.Width - 32, Height = 20 };
            Map.Init();
            JoinPanel.Add(Map);
            Author = new Label(manager) { Text = "", Left = 8, Top = Map.Top + Map.Height + 4, Width = JoinPanel.Width - 32, Height = 12 };
            Author.Init();
            JoinPanel.Add(Author);
            Description = new Label(manager) { Text = "", Alignment = TomShane.Neoforce.Controls.Alignment.TopLeft, Left = 8, Top = Author.Top + Author.Height + 4, Width = JoinPanel.Width - 32, Height = 12 * 4 };
            Description.Init();
            JoinPanel.Add(Description);
            LastSaved = new Label(manager) { Text = "", Left = 8, Top = Description.Top + Description.Height + 4, Width = JoinPanel.Width - 32, Height = 12 };
            LastSaved.Init();
            JoinPanel.Add(LastSaved);
            Version = new Label(manager) { Text = "", Left = 8, Top = LastSaved.Top + LastSaved.Height + 4, Width = JoinPanel.Width - 32, Height = 12 };
            Version.Init();
            JoinPanel.Add(Version);
            Size = new Label(manager) { Text = "", Left = 8, Top = Version.Top + Version.Height + 4, Width = JoinPanel.Width - 32, Height = 12 };
            Size.Init();
            JoinPanel.Add(Size);

            Seed = new Label(manager) { Text = "", Left = 8, Top = Size.Top + Size.Height + 4, Width = JoinPanel.Width - 32, Height = 12 };
            Seed.Init();
            JoinPanel.Add(Seed);

            Rename = new Button(manager);
            Rename.Left = ((JoinPanel.Width) / 2) - (Rename.Width / 2);
            Rename.Top = JoinPanel.ClientHeight - Rename.Height - 8;
            Rename.Text = "Rename";
            Rename.Click += Rename_Click;
            JoinPanel.Add(Rename);

            Delete = new Button(manager);
            Delete.Left =((JoinPanel.Width) / 2) - (Delete.Width / 2) - (Rename.Width) - 8;
            Delete.Top = JoinPanel.ClientHeight - Delete.Height - 8;
            Delete.Text = "Delete";
            Delete.Click += Delete_Click;
            JoinPanel.Add(Delete);

            Open = new Button(manager);
            Open.Left = ((JoinPanel.Width) / 2) + (Rename.Width / 2) + 8;
            Open.Top = JoinPanel.ClientHeight - Rename.Height - 8;
            Open.Text = "Join";
            Open.Color = Color.LightGreen;
            Open.Click += Open_Click;
            JoinPanel.Add(Open);

            Delete.Enabled = Rename.Enabled = Open.Enabled = false;
            //---------------------------------------------

            NameLbl = new Label(manager);
            NameLbl.Init();
            NameLbl.Top = 8;
            NameLbl.Left = 8;
            NameLbl.Text = "Name:";
            NameLbl.Width = CreatePanel.Width / 2 - 16;
            CreatePanel.Add(NameLbl);

            NameInput = new TextBox(manager);
            NameInput.Init();
            NameInput.Top = NameLbl.Top + NameLbl.Height + 2;
            NameInput.Left = 8;
            NameInput.TextChanged += NameInput_TextChanged;
            NameInput.Width = NameLbl.Width;

            CreatePanel.Add(NameInput);
            DescriptionLbl = new Label(manager);
            DescriptionLbl.Init();
            DescriptionLbl.Width = NameLbl.Width;
            DescriptionLbl.Top = NameInput.Top + NameInput.Height + 2;
            DescriptionLbl.Left = 8;
            DescriptionLbl.Text = "Description:";
            CreatePanel.Add(DescriptionLbl);
            DescriptionInput = new TextBox(manager);
            DescriptionInput.Init();

            DescriptionInput.Mode = TextBoxMode.Multiline;
            DescriptionInput.Top = DescriptionLbl.Top + DescriptionLbl.Height + 2;
            DescriptionInput.Left = 8;
            DescriptionInput.Width = NameInput.Width;
            DescriptionInput.Height = 64;
            DescriptionInput.ScrollBars = ScrollBars.Vertical;
            DescriptionInput.Text = "A short description\nof the level";
            SizeLbl = new Label(manager);
            SizeLbl.Init();
            SizeLbl.Top = 8;
            SizeLbl.Left = 16 + NameInput.Width;
            SizeLbl.Text = "Size:";
            CreatePanel.Add(SizeLbl);
            CreatePanel.Add(DescriptionInput);


            SizeCmb = new ComboBox(manager);
            SizeCmb.Init();

            SizeCmb.Left = SizeLbl.Left;
            SizeCmb.Top = SizeLbl.Top + SizeLbl.Height + 2;
            SizeCmb.Width = NameInput.Width;
            SizeCmb.Height = 20;
            SizeCmb.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            SizeCmb.Items.Add("Mini 1000x300");
            SizeCmb.Items.Add("Small 2000x600");
            SizeCmb.Items.Add("Mild 3000x900");
            SizeCmb.Items.Add("Regular 4000x1200");
            SizeCmb.Items.Add("Moderate 5000x1500");
            SizeCmb.Items.Add("Big 6000x1800");
            SizeCmb.Items.Add("Immense 7000x2100");
            SizeCmb.ReadOnly = true;
            SizeCmb.OutlineMoving = SizeCmb.OutlineResizing = false;
            SizeCmb.ItemIndex = 0;
            SizeCmb.Detached = false;
            CreatePanel.Add(SizeCmb);

            SeedLbl = new Label(manager);
            SeedLbl.Init();
            SeedLbl.Width = NameLbl.Width;
            SeedLbl.Top = SizeCmb.Top + SizeCmb.Height + 2;
            SeedLbl.Left = SizeLbl.Left;
            SeedLbl.Text = "Seed:";
            SeedLbl.ToolTip.Text = "Seeds are an id used to generate a unique world, using the same seed will generate the same world as anothe seed that is the same.";
            CreatePanel.Add(SeedLbl);

            SeedInput = new TextBox(manager);
            SeedInput.Init();
            SeedInput.Top = SeedLbl.Top + SeedLbl.Height + 2;
            SeedInput.Left = SizeLbl.Left;
            SeedInput.Width = NameInput.Width;
            SeedInput.TextChanged += SeedInput_TextChanged;
            SeedInput.Text = Guid.NewGuid().GetHashCode().ToString();


            GeneratorLbl = new Label(manager);
            GeneratorLbl.Init();
            GeneratorLbl.Top = SeedInput.Top + SeedInput.Height + 4;
            GeneratorLbl.Left = SizeLbl.Left;
            GeneratorLbl.Text = "Generator:";
            GeneratorLbl.Width = SeedLbl.Width;
            CreatePanel.Add(GeneratorLbl);


            GeneratorCmb = new ComboBox(manager);
            GeneratorCmb.Init();
            GeneratorCmb.Detached = false;
            GeneratorCmb.Left = SizeLbl.Left;
            GeneratorCmb.Top = GeneratorLbl.Top + GeneratorLbl.Height + 4;
            GeneratorCmb.Width = NameInput.Width;
            GeneratorCmb.Height = 20;
            GeneratorCmb.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            GeneratorCmb.Items.Add("Normal Planet");
            GeneratorCmb.Items.Add("Flat World");
            GeneratorCmb.ItemIndex = 0;
            GeneratorCmb.ReadOnly = true;
            GeneratorCmb.OutlineMoving = GeneratorCmb.OutlineResizing = false;
            CreatePanel.Add(GeneratorCmb);

            CreatePanel.Add(SeedInput);
            Generate = new Button(manager);
            Generate.Init();
            Generate.Top = DescriptionInput.Top + DescriptionInput.Height + 8;
            Generate.Left = (CreatePanel.Width / 2) - (Generate.Width / 2);
            Generate.Click += Generate_Click;
            Generate.Text = "Create";
            Generate.Color = Color.LightGreen;
            Generate.Enabled = false;
            CreatePanel.Add(Generate);

            //-----------------------------------------
            OpenFolder = new Button(manager);
            OpenFolder.Init();
            OpenFolder.Width = 140;
            OpenFolder.Left = ((MainWindow.ClientWidth) / 2) - (OpenFolder.Width / 2);
            OpenFolder.Top = MainWindow.sidebar.Height + MainWindow.sidebar.Top - 58;
            OpenFolder.Text = "[color:Gold]Open[/color] map folder";
            OpenFolder.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                System.Diagnostics.Process.Start(IO.Directories["Maps"]);
            });
            MainWindow.sidebar.Add(OpenFolder);

            VisitSite = new Button(manager);
            VisitSite.Init();
            VisitSite.Width = 140;
            VisitSite.Left = ((MainWindow.ClientWidth) / 2) - (OpenFolder.Width / 2) - (VisitSite.Width) - 8;
            VisitSite.Top = MainWindow.sidebar.Height + MainWindow.sidebar.Top - 58;
            VisitSite.Text = "[color:Gold]Get[/color] maps from site";
            VisitSite.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                System.Diagnostics.Process.Start(link);
            });
            VisitSite.Enabled = false; // 2018
            MainWindow.sidebar.Add(VisitSite);

            Exit = new Button(manager);
            Exit.Init();
            Exit.Width = 140;
            Exit.Left = ((MainWindow.ClientWidth) / 2) + (VisitSite.Width / 2) + 8;
            Exit.Top = MainWindow.sidebar.Height + MainWindow.sidebar.Top - 58;
            Exit.Text = "[color:Gold]Exit[/color] to main menu";
            Exit.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                Close();
                MainWindow.InitMainScreen();
            });
            MainWindow.sidebar.Add(Exit);

            Exit.BringToFront();
            VisitSite.BringToFront();
            OpenFolder.BringToFront();

            if (MapList.Items.Count > 0)
            {
                MapList.ItemIndex = 0;
                (MapList.Items[MapList.ItemIndex] as MapListItem).sb.Color = Color.LightGreen;
            }
            Closed += TaskSandbox_Closed;
            CheckGenerate();
        }

        void NameInput_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CheckGenerate();
        }

        void SeedInput_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CheckGenerate();
        }

        private void CheckGenerate()
        {
            if (Generate != null)
            {
                Generate.Enabled = true;
                int n;
                bool isNumeric = int.TryParse(SeedInput.Text, out n);
                if (!isNumeric || string.IsNullOrWhiteSpace(NameInput.Text) || !MapList.Items.All(x => (x as MapListItem).MapName.Text != NameInput.Text))
                    Generate.Enabled = false;
            }
        }

        void TaskSandbox_Closed(object sender, WindowClosedEventArgs e)
        {
            MainWindow.sidebar.Remove(OpenFolder);
            MainWindow.sidebar.Remove(VisitSite);
            MainWindow.sidebar.Remove(Exit);
        }
        public Point GetSize(int index)
        {
            Point[] sizes = new Point[7] {
                new Point(1000,300),
                new Point(2000,600),
                new Point(3000,900),
                new Point(4000,1200),
                new Point(5000,1500),
                new Point(6000,1800),
                new Point(7000,2100),
            };
            return sizes[index];
        }
        public Generator GetGenerator(int index)
        {
            Generator[] sizes = new Generator[2] {
                 Generator.Planet,
                 Generator.Flat,
            };
            return sizes[index];
        }
        void Generate_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            CheckGenerate();
            if (Generate.Enabled)
            {
                MainWindow.DeInitGameScreen();
                Game.CurrentGameState = Game.GameState.UniverseViewer;
                (Manager.Game as Game).OpenUniverseViewer(false);
                MainWindow.InitUniverseScreen();
                MainWindow.DeInitUniverseScreen();
                MainWindow.InitGenerateScreen();

                Point size = GetSize(SizeCmb.ItemIndex);
                LevelData data = new LevelData();
                data.Name = NameInput.Text;
                data.Description = DescriptionInput.Text;
                data.Seed = int.Parse(SeedInput.Text);
                data.Author = Game.UserName;
                Game.UniverseViewer.GenerateMap(data, size.X, size.Y, GetGenerator(GeneratorCmb.ItemIndex));
                Close();

                if (MapList.Items.Count == 0)
                {
                    Map.Text = "No Maps Found";
                    Author.Text = Description.Text = Seed.Text = Size.Text = Version.Text = LastSaved.Text = string.Empty;
                    Rename.Enabled = Open.Enabled = Delete.Enabled = false;
                }
            }
        }

        void Open_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (MapList.Items.Count == 0)
                return;
            string name =(MapList.Items[MapList.ItemIndex] as MapListItem).MapName.Text;
            List();
            if (!MapList.Items.Any(x => (x as MapListItem).MapName.Text == name))
                return;
            MainWindow.DeInitGameScreen();
            Game.CurrentGameState = Game.GameState.UniverseViewer;
            (Manager.Game as Game).OpenUniverseViewer(false);
            MainWindow.InitUniverseScreen();
            MainWindow.InitGenerateScreen();
            MainWindow.DeInitUniverseScreen();
            Game.UniverseViewer.OpenMap((MapList.Items[MapList.ItemIndex] as MapListItem).MapName.Text);
            Close();
        }

        private void List()
        {
            if (Map != null)
            Rename.Enabled = Open.Enabled = Delete.Enabled = true;
            MapList.Items.Clear();
            List<LevelData> Data = IO.ListMaps();
            foreach (LevelData ld in Data)
            {
                MapListItem Item = new MapListItem(Manager, ld);
                Item.Init();
                MapList.Items.Add(Item);
            }
            if (Map != null && MapList.Items.Count == 0)
            {
                Map.Text = "No Maps Found";
                Author.Text = Description.Text = Seed.Text = Size.Text = Version.Text = LastSaved.Text = string.Empty;
                Rename.Enabled = Open.Enabled = Delete.Enabled = false;
            }
        }

        void Rename_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (MapList.Items.Count == 0)
                return;
            TaskRenameMap tmp = new TaskRenameMap(Manager, this, (MapList.Items[MapList.ItemIndex] as MapListItem).MapName.Text);
            tmp.Init();
            tmp.Closed += tmp_Closed;
            Manager.Add(tmp);
        }

        void tmp_Closed(object sender, WindowClosedEventArgs e)
        {
            List();
            if (MapList.Items.Count > 0)
            {
                MapList.ItemIndex = 0;
                (MapList.Items[MapList.ItemIndex] as MapListItem).sb.Color = Color.LightGreen;
            }
        }

        void Delete_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (MapList.Items.Count == 0)
                return;
            MapListItem item = (MapList.Items[MapList.ItemIndex] as MapListItem);
            MapList.Enabled = false;
            MessageBox confirmDelete = new MessageBox(Manager, MessageBoxType.YesNo, "Are you sure you would like to delete " + item.MapName.Text + " ?\nIt will be lost forever.", "Confirm Deletion");
            confirmDelete.Init();
            confirmDelete.buttons[0].Text = "Delete";
            confirmDelete.buttons[1].Text = "Cancel";
            confirmDelete.buttons[0].Color = Color.Red;
            confirmDelete.Closed += new WindowClosedEventHandler(delegate(object s, WindowClosedEventArgs ev)
            {
                if ((s as Dialog).ModalResult == ModalResult.Yes)
                {
                    IO.DeleteMap(item.MapName.Text);
                    MapList.Items.Remove(item);
                    if (MapList.Items.Count > 0)
                    {
                        MapList.ItemIndex = 0;
                        (MapList.Items[MapList.ItemIndex] as MapListItem).sb.Color = Color.LightGreen;
                    }
                }
                MapList.Enabled = true;
            });
            confirmDelete.ShowModal();
            Manager.Add(confirmDelete);
        }

        void MapList_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            List();
            if (MapList.Items.Count > 0)
            {
                foreach (MapListItem C in MapList.Items)
                    C.sb.Color = Color.White;
                (MapList.Items[MapList.ItemIndex] as MapListItem).sb.Color = Color.LightGreen;
                MapListItem item = (MapList.Items[MapList.ItemIndex] as MapListItem);
                try
                {
                    string[] Data = IO.GetLevelData(item.MapName.Text);
                    Map.Text = item.MapName.Text;
                    Author.Text = "By: " + Data[0];
                    Description.Text = "Description: " + Data[1];
                    Version.Text = "Game Version: " + Data[2];
                    LastSaved.Text = "Last Played: " + Data[3];
                    Size.Text = "Size: " + Data[5] + "x" + Data[6];
                    Seed.Text = "Seed: " + Data[4];
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.ToString());
                }
            }
        }  
        #endregion
    }
}
