

#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Controls = TomShane.Neoforce.Controls;
using System.IO;
using ZarknorthClient;
using System.Linq;
using System.IO.Compression;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace ZarknorthClient.Interface
{
    public class TaskSinglePlayer : Dialog
    {

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////       
        private GroupPanel OpenPanel = null;
        private GroupPanel CreatePanel = null;
        public Label NameLbl;
        public Label descriptionl;
        public Label sizel;
        public Label seedl;
        public TextBox seed;
        public TextBox namel;
        public TextBox NameInput;
        public TextBox description;
        public ComboBox size;
        public Button Open;
        public Button Delete;
        public Button Rename;
        public TextBox reName;
        public Button Create;
        public static Label generating;
        public static Label opening;
        public static Label generatingLbl;
        public static Label openingLbl;
        public Label descrition;
        public Label name;
        public ListBox Worlds;

        public List<string> Info = new List<string>();
        ////////////////////////////////////////////////////////////////////////////


        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////   
        public TaskSinglePlayer(Manager manager)
            : base(manager)
        {
            Width = 600;
           
           
            Resizable = false;
            Center();
            Text = "Singleplayer Mode";

            TopPanel.Visible = true;
            Remove(BottomPanel);
            Caption.Text = "Information";
            Description.Text = "Create or open a new single player world.";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);

            OpenPanel = new GroupPanel(Manager);
            OpenPanel.Init();
            OpenPanel.Parent = this;
            OpenPanel.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            OpenPanel.Width = ClientWidth - 16;
            OpenPanel.Height = 232;
            OpenPanel.Left = 8;
            OpenPanel.Top = TopPanel.Height + 8;
            OpenPanel.Text = "Open a world!";
            Height = OpenPanel.Height * 2 + 64 + TopPanel.Height;
            Add(OpenPanel);
            CreatePanel = new GroupPanel(Manager);
            CreatePanel.Init();
            CreatePanel.Parent = this;
            CreatePanel.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            CreatePanel.Width = ClientWidth - 16;
            CreatePanel.Height = 200;
            CreatePanel.Left = 8;
            CreatePanel.Top = OpenPanel.Height + OpenPanel.Top + 8;
            CreatePanel.Text = "Create a world!";
            Add(CreatePanel);
            name = new Label(manager) { Font = FontSize.Default14 };
            name.Init();
            name.Width = 96;
            name.Left = (OpenPanel.Width / 3 * 2) + 8;
            name.Top = 8 ;
            name.Height = 16;

            OpenPanel.Add(name);
            descrition = new Label(manager);
            descrition.Init();
            descrition.Height = 200;
            descrition.Width =  OpenPanel.Width / 2;
            descrition.Left = (OpenPanel.Width / 3 * 2) + 8;
            descrition.Top = 8  + name.Height;
            OpenPanel.Add(descrition);
            
            Worlds = new ListBox(manager);
            Worlds.Init();
            Worlds.Top = 8 ;
            Worlds.Left = 8;
                Worlds.Width =( OpenPanel.Width / 3 * 2) - 16 ;
            Worlds.Height = OpenPanel.Height - 40 - 32;
          
            Worlds.ItemIndexChanged +=new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
                   {
                       string[] s = Info[Worlds.ItemIndex].Split('~');
                       name.Text = s[0];
                       descrition.Text = s[1] + "\n\nLast Saved: " + s[3] + "\nVersion Saved: " + s[2] + "\nDimensions: " + s[4] + "x" + s[5];
                   });
            OpenPanel.Add(Worlds);
            Open = new Button(manager);
            Open.Init();
            Open.Top = Worlds.Top + Worlds.Height + 8;
            Open.Left = 8;
            Open.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {
        
            });
            Open.Text = "Open";
            OpenPanel.Add(Open);
            Delete = new Button(manager);
            Delete.Init();
            Delete.Top = Worlds.Top + Worlds.Height + 8;
            Delete.Left = Open.Left + Open.Width + 8;
            Delete.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {
                File.Delete(IO.Directories["Planets"] + Worlds.Items[Worlds.ItemIndex] + ".z");
               
                AddFilesToList();
            });
            Delete.Text = "Delete";
            OpenPanel.Add(Delete);
            Rename = new Button(manager);
            Rename.Init();
            Rename.Top = Worlds.Top + Worlds.Height + 8;
            Rename.Left = Delete.Left + Delete.Width + 8;
            Rename.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {
                File.Move(IO.Directories["Planets"] + Worlds.Items[Worlds.ItemIndex] + IO.WorldSuffix, IO.Directories["Planets"] + reName.Text + IO.WorldSuffix);
                AddFilesToList();
            });
           Rename.Text = "Rename";
            reName = new TextBox(manager);
            reName.Init();
            reName.Top = Worlds.Top + Worlds.Height + 9;
            reName.Left = Rename.Left + Rename.Width + 8;
            reName.Text = "New Name";
            reName.Width = (OpenPanel.Width / 3 * 2) - reName.Left - 8;
            OpenPanel.Add(Rename);
            OpenPanel.Add(reName);

            //Add worlds
            AddFilesToList();
  

            NameLbl = new Label(manager);
            NameLbl.Init();
            NameLbl.Top = 8;
            NameLbl.Left = 8;
            NameLbl.Text = "Name:";
            NameLbl.Width = CreatePanel.Width / 2 - 16;
            CreatePanel.Add(NameLbl);

            NameInput = new TextBox(manager);
            NameInput.Init();
            NameInput.Top = NameLbl.Top + NameLbl.Height +2;
            NameInput.Left = 8;
            NameInput.Width = NameLbl.Width;
            
            CreatePanel.Add(NameInput);
           descriptionl = new Label(manager);
           descriptionl.Init();
           descriptionl.Width = NameLbl.Width;
           descriptionl.Top = NameInput.Top + NameInput.Height + 2;
           descriptionl.Left = 8;
           descriptionl.Text = "Description:";
           CreatePanel.Add(descriptionl);
          description = new TextBox(manager);
          description.Init();
          
          description.Mode = TextBoxMode.Multiline;
          description.Top = descriptionl.Top + descriptionl.Height + 2;
          description.Left = 8;
          description.Width = NameInput.Width;
          description.Height = 64;
          description.ScrollBars = ScrollBars.Vertical;
          description.Text = "A short description\nof the level";
          sizel = new Label(manager);
          sizel.Init();
          sizel.Top = 8;
          sizel.Left = 16 + NameInput.Width;
          sizel.Text = "Size:";
          CreatePanel.Add(sizel);
           CreatePanel.Add(description);


           size = new ComboBox(manager);
           size.Init();

           size.Left = sizel.Left;
           size.Top = sizel.Top + sizel.Height + 2;
           size.Width = NameInput.Width;
           size.Height = 20;
           size.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
           size.Items.Add("Little 1000x300");
           size.Items.Add("Small 2000x600");
           size.Items.Add("Mild 3000x900");
           size.Items.Add("Regular 4000x1200");
           size.Items.Add("Moderate 5000x1500");
           size.Items.Add("Big 6000x1800");
           size.Items.Add("Immense 7000x2100");
           

           size.ItemIndex = 0;
           CreatePanel.Add(size);

           seedl = new Label(manager);
           seedl.Init();
           seedl.Width = NameLbl.Width;
           seedl.Top = size.Top + size.Height + 2;
           seedl.Left = sizel.Left;
           seedl.Text = "Seed:";
           seedl.ToolTip.Text = "seeds are values made up of characters \nand numbers that are used as the \nbasis for generating every world";
           CreatePanel.Add(seedl);

           seed = new TextBox(manager);
           seed.Init();
           seed.Top = seedl.Top + seedl.Height + 2;
           seed.Left = sizel.Left;
           seed.Width = NameInput.Width;
           CreatePanel.Add(seed);
            Create = new Button(manager);
            Create.Init();
            Create.Top = description.Top + description.Height + 8;
            Create.Left = 8;
            Create.Click += new Controls.EventHandler(Create_Click);
            Create.Text = "Create";
            CreatePanel.Add(Create);
            generatingLbl = new Label(manager);
            generatingLbl.Init();
            generatingLbl.Left = Create.Left + Create.Width + 8;
            generatingLbl.Top = Create.Top + 4;
            generatingLbl.Width = CreatePanel.Width;
            generatingLbl.TextColor = Color.Gold;
            generatingLbl.Text = string.Empty;
            CreatePanel.Add(generatingLbl);
            openingLbl = new Label(manager);
            openingLbl.Init();
            openingLbl.Left = reName.Left + reName.Width + 8;
            openingLbl.Top = reName.Top + 4;
            openingLbl.Width = CreatePanel.Width;
            openingLbl.TextColor = Color.Gold;
            openingLbl.Text = string.Empty;
            OpenPanel.Add(openingLbl);
          
        }

        private void AddFilesToList()
        {
            Info.Clear();
            Worlds.Items.Clear();
            if (Directory.Exists(IO.Directories["Planets"]))
                foreach (string file in Directory.GetFiles(IO.Directories["Planets"]))
                {



                    using (FileStream fileStream = new FileStream(file, FileMode.Open))
                    {
                        using (BinaryReader binaryWriter1 = new BinaryReader(fileStream))
                        {
                            using (GZipStream zipStream = new GZipStream(binaryWriter1.BaseStream,

                 CompressionMode.Decompress, true))

                            using (BinaryReader binaryWriter = new BinaryReader(zipStream))
                            {
                                //Read game details
                                string name1 = file.Replace(IO.Directories["Planets"], "");
                                name1 = name1.Remove(name1.Length - 2);
                                string desc = binaryWriter.ReadString();
                                string date = binaryWriter.ReadString();
                                string version = binaryWriter.ReadString();
                                int width = binaryWriter.ReadInt32();
                                int height = binaryWriter.ReadInt32();
                                Info.Add(name1 + "~" + desc + "~" + date + "~" + version + "~" + width + "~" + height);
                                Worlds.Items.Add(name1);

                            }
                        }

                    }
                }
                    if (Info.Count > 0)
                    {
                        Worlds.ItemIndex = 0;
                        string[] st = Info[0].Split('~');
                        name.Text = st[0];
                        descrition.Text = st[1] + "\n\nLast Saved: " + st[3] + "\nVersion Saved: " + st[2] + "\nDimensions: " + st[4] + "x" + st[5];
                    }
                
        }

   
        ////////////////////////////////////////////////////////////////////////////    

        #endregion

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////    
        public override void Init()
        {
            base.Init();
        }
        ////////////////////////////////////////////////////////////////////////////   



        ////////////////////////////////////////////////////////////////////////////  
        void Create_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            
               
            //    Manager.Cursor = Manager.Skin.Cursors["Busy"].Resource;
            //    Game.CurrentGameState = Game.GameState.InGame;
            //    Game.genNextUpdate = true;
            //    Game.whatTogenNextUpdate = NameInput.Text;
            //    Game.descTogenNextUpdate = description.Text;
            //   string Str = seed.Text.Trim();
            //    int Num;

            //    bool isNum = int.TryParse(Str, out Num);
            //    if (isNum)
            //        Game.seedTogenNextUpdate = Num;
            //    else
            //        Game.seedTogenNextUpdate = Str != "" ? Str.GetHashCode() : Game.random.Next(0,10000000);
            //string[] st = size.Items[size.ItemIndex].ToString().Split(' ');
            //string[] st2 = st[1].Split('x');
            //Game.sizeTogenNextUpdate = new Vector2(float.Parse(st2[0]), float.Parse(st2[1]));
                
            


        }


        ////////////////////////////////////////////////////////////////////////////  
        protected override void Update(GameTime gameTime)
        {
            if (generatingLbl.Text == "Done")
            {
                Close();
            }
            if (openingLbl.Text == "Done")
            {
                Close();
            }
            if (Worlds.ItemIndex >= 0 && !Open.Enabled)
                Open.Enabled = true;
            if (Worlds.ItemIndex< 0 && Open.Enabled)
                Open.Enabled = false;
            if (NameInput.Text != string.Empty && !Create.Enabled && !Worlds.Items.Contains(NameInput.Text))
            Create.Enabled = true;
            if ((NameInput.Text == string.Empty || Worlds.Items.Contains(NameInput.Text)) && Create.Enabled)
            Create.Enabled = false;
            if (reName.Text != string.Empty && !Rename.Enabled && !Worlds.Items.Contains(reName.Text))
                Rename.Enabled = true;
            if ((reName.Text == string.Empty || Worlds.Items.Contains(reName.Text)) && Rename.Enabled)
                Rename.Enabled = false;
            base.Update(gameTime);
        }
        ////////////////////////////////////////////////////////////////////////////                  

        #endregion

    }
}
