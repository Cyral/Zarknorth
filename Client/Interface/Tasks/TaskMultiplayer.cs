

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

////////////////////////////////////////////////////////////////////////////

#endregion

namespace ZarknorthClient.Interface
{
    public class TaskMultiPlayer : Dialog
    {

        #region //// Fields ////////////

        ////////////////////////////////////////////////////////////////////////////       
        private GroupPanel OpenPanel = null;
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
        public Button Edit;
        public Button AddServer;
        public TextBox reName;
        public Button Create;
        public Button Direct;
        public static Label generating;
        public static Label opening;
        public static Label generatingLbl;
        public Label descrition;
        public Label name;
        public ListBox Worlds;

        public List<string> Info = new List<string>();
        ////////////////////////////////////////////////////////////////////////////


        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////   
        public TaskMultiPlayer(Manager manager)
            : base(manager)
        {
            Width = 600;


            Resizable = false;
            Center();
            Text = "Multiplayer Mode";

            TopPanel.Visible = true;
            Remove(BottomPanel);
            Caption.Text = "Information";
            Description.Text = "Play in a multiplayer server!";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);
            
            OpenPanel = new GroupPanel(Manager);
            OpenPanel.Init();
            OpenPanel.Parent = this;
            OpenPanel.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            OpenPanel.Width = ClientWidth - 16;
            OpenPanel.Height = 232;
            OpenPanel.Left = 8;
            OpenPanel.Top = TopPanel.Height + 8;
            OpenPanel.Text = "Join a server!";
            Height = OpenPanel.Height * 2 + 64 + TopPanel.Height;
            Add(OpenPanel);

            name = new Label(manager) { Font = FontSize.Default14 };
            name.Init();
            name.Width = 96;
            name.Left = (OpenPanel.Width / 3 * 2) + 8;
            name.Top = 8;
            name.Height = 16;

            OpenPanel.Add(name);
            descrition = new Label(manager);
            descrition.Init();
            descrition.Height = 200;
            descrition.Width = OpenPanel.Width / 2;
            descrition.Left = (OpenPanel.Width / 3 * 2) + 8;
            descrition.Top = 8 + name.Height;
            OpenPanel.Add(descrition);

            Worlds = new ListBox(manager);
            Worlds.Init();
            Worlds.Top = 8;
            Worlds.Left = 8;
            Worlds.Width = (OpenPanel.Width / 3 * 2) - 16;
            Worlds.Height = OpenPanel.Height - 40 - 32;

            Worlds.ItemIndexChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
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
            Open.Text = "Join";
            OpenPanel.Add(Open);
            
            Delete = new Button(manager);
            Delete.Init();
            Delete.Top = Worlds.Top + Worlds.Height + 8;
            Delete.Left = Open.Left + Open.Width + 8;
            Delete.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {
                File.Delete(IO.Directories["World"] + Worlds.Items[Worlds.ItemIndex] + IO.WorldSuffix);

                AddFilesToList();
            });
            Delete.Text = "Delete";
            OpenPanel.Add(Delete);
            Edit = new Button(manager);
            Edit.Init();
            Edit.Top = Worlds.Top + Worlds.Height + 8;
            Edit.Left = Delete.Width + Delete.Left + 8;
            Edit.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {

            });
            Edit.Text = "Edit";
            OpenPanel.Add(Edit);
           
            AddServer = new Button(manager);
            AddServer.Init();
            AddServer.Top = Worlds.Top + Worlds.Height + 8;
            AddServer.Left = Edit.Width + Edit.Left + 8;
            AddServer.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {
              
            });
            AddServer.Text = "Add";
            OpenPanel.Add(AddServer);


            Direct = new Button(manager);
            Direct.Init();
            Direct.Top = Worlds.Top + Worlds.Height + 8;
            Direct.Left = AddServer.Width + AddServer.Left + 8;
            Direct.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
            {
                DirectConnect d = new DirectConnect(manager);
                d.Init();
                manager.Add(d);
            });
            Direct.Width = 100;
            Direct.Text = "Direct Connect";
            OpenPanel.Add(Direct);
            //Add worlds
            AddFilesToList();



        }

        private void AddFilesToList()
        {
            //Info.Clear();
            //Worlds.Items.Clear();
            //if (Directory.Exists(Game.WorldDir))
            //    foreach (string file in Directory.GetFiles(Game.WorldDir))
            //    {


            //        using (FileStream fileStream = new FileStream(file, FileMode.Open))
            //        {
            //            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            //            {

            //                //Read game details
            //                string name1 = file.Replace(Game.WorldDir, "");
            //                name1 = name1.Remove(name1.Length - 2);
            //                string desc = binaryReader.ReadString();
            //                string date = binaryReader.ReadString();
            //                string version = binaryReader.ReadString();
            //                int width = binaryReader.ReadInt32();
            //                int height = binaryReader.ReadInt32();
            //                Info.Add(name1 + "~" + desc + "~" + date + "~" + version + "~" + width + "~" + height);
            //                Worlds.Items.Add(name1);

            //            }
            //        }

            //    }
            //if (Info.Count > 0)
            //{
            //    Worlds.ItemIndex = 0;
            //    string[] st = Info[0].Split('~');
            //    name.Text = st[0];
            //    descrition.Text = st[1] + "\n\nLast Saved: " + st[3] + "\nVersion Saved: " + st[2] + "\nDimensions: " + st[4] + "x" + st[5];
            //}
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

        }


        ////////////////////////////////////////////////////////////////////////////  
        protected override void Update(GameTime gameTime)
        {
            //if (generatingLbl.Text == "Done")
            //{
            //    Close();
            //}
            if (Worlds.ItemIndex >= 0 && !Open.Enabled)
                Open.Enabled = true;
            if (Worlds.ItemIndex < 0 && Open.Enabled)
                Open.Enabled = false;
         
          
            base.Update(gameTime);
        }
        ////////////////////////////////////////////////////////////////////////////                  

        #endregion

    }
}
