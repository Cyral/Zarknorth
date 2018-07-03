
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Configuration;
using System.Diagnostics;
#endregion

namespace ZarknorthClient.Interface
{
    public class TaskContentPacks : Dialog
    {
        #region Fields
        private ControlList<ContentPackListControl> List;
        private Button btnSet, btnOpen;
        private int curPack;
        #endregion

        #region Constructors 
        public TaskContentPacks(Manager manager)
            : base(manager)
        {
            Height = 400;
            Width = 500;
            MinimumHeight = 100;
            MinimumWidth = 100;
            Resizable = false;
            Text = "Content Packs";
            Center();
            TopPanel.Visible = true;
            BottomPanel.Visible = true;

            //TopPanel.Visible = true;
            Caption.Text = "Content Packs";
            Description.Text = "Customize the graphics of Zarknorth with content packs!\nCurrent Pack: " + Game.ContentPackName;
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);

            AddList(manager);
            ResetItems(manager);

            btnOpen = new Button(manager);
            btnOpen.Width = 200;
            btnOpen.Top = 8;
            btnOpen.Left = (ClientWidth / 2) - btnOpen.Width - 8;
            btnOpen.Text = "Open Content Packs Folder";
            btnOpen.Click += btnOpen_Click;
            BottomPanel.Add(btnOpen);

            btnSet = new Button(manager);
            btnSet.Width = 200;
            btnSet.Top = 8;
            btnSet.Left = (ClientWidth / 2) + 8;
            btnSet.Text = "Set as Current Content Pack";
            btnSet.Click += btnSet_Click;
            btnSet.Enabled = false;
            BottomPanel.Add(btnSet);

            for (int i = 0; i < List.Items.Count; i++)
            {
                if ((List.Items[i] as ContentPackListControl).Pack.Name == Game.ContentPackName)
                {
                    List.ItemIndex = i;
                    curPack = i;
                }
            }
            List.ItemIndexChanged += List_ItemIndexChanged;
        }

        void btnSet_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //Set Game's content pack data
            foreach (ContentPack pack in IO.ContentPacks)
                if (pack.Name == (List.Items[List.ItemIndex] as ContentPackListControl).Pack.Name)
                {
                    Game.ContentPackData = pack;
                    Game.ContentPackName = pack.Name;
                }

            //Save in config
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
            config.AppSettings.Settings["ContentPack"].Value = Game.ContentPackName;
            config.Save(ConfigurationSaveMode.Modified);

            //Prompt User
            MessageBox m = new MessageBox(Manager, MessageBoxType.YesNo, "Your content pack has been set. Please restart the game to apply changes!\nRestart now?", "Apply Changes");
            m.Init();
            m.Closed += m_Closed;
            m.ShowModal();
            Manager.Add(m);
            Description.Text = "Customize the graphics of Zarknorth with content packs! [color:Gold]Restart to apply pack![/color]\nCurrent Pack: " + Game.ContentPackName;
            //Close window
            Close();
        }

        void m_Closed(object sender, WindowClosedEventArgs e)
        {
            // Check dialog resule and see if we need to shut down.
            if ((sender as Dialog).ModalResult == ModalResult.Yes)
            {
                //A bit hacky... But restarts the program using a timed ping...
                ProcessStartInfo Info = new ProcessStartInfo();
                Info.Arguments = "/C ping 127.0.0.1 -n 1 && \"" + System.Windows.Forms.Application.ExecutablePath + "\"";
                Info.WindowStyle = ProcessWindowStyle.Hidden;
                Info.CreateNoWindow = true;
                Info.FileName = "cmd.exe";
                Process.Start(Info);
                ((sender as Control).Manager.Game as ZarknorthClient.Game).ExitConfirmation = false;
                System.Windows.Forms.Application.Exit(); 
            }
        }

        void List_ItemIndexChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (List.ItemIndex != curPack)
                btnSet.Enabled = true;
            else
                btnSet.Enabled = false;
        }

        void btnOpen_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            System.Diagnostics.Process.Start(IO.Directories["Content Packs"]);
        }

        private void AddList(Manager manager)
        {
            List = new ControlList<ContentPackListControl>(manager);
            List.Init();
            List.Top = TopPanel.Height + 4;
            List.Left = 4;
            List.Width = ClientWidth - (List.Left * 2);
            List.Height = ClientHeight - List.Top - 12 - 32;
            Add(List);
        }

        public void ResetItems(Manager manager)
        {
            Color greenColor = new Color(70, 255, 70, 255);
            Color grayColor = new Color(128, 128, 128, 255);

            Remove(List);
            AddList(manager);

            foreach (ContentPack pack in IO.ContentPacks)
            {
                ContentPackListControl l = new ContentPackListControl(manager, pack, pack.Name == Game.ContentPackName ? greenColor : grayColor);
                l.Init();
                List.Items.Add(l);
            }
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
