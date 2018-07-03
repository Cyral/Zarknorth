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
using Microsoft.Xna.Framework.Input;
using ZarknorthClient.Music;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace ZarknorthClient.Interface
{
    public class TaskSign : Dialog
    {
        private Button Save;
        private Button Cancel;
        private TextBox Input;

        /// <summary>
        /// Create a new window for editng Text tiles, eg signs
        /// </summary>
        /// <param name="manager">UI Manager Object</param>
        /// <param name="interact">Interaction Parameters, includes level and position of sign</param>
        public TaskSign(Manager manager, InteractBlockEventArgs interact) : base(manager)
        {
            //Set up the window
            Text = "Sign";
            Resizable = false;
            Init();
            ClientWidth = 320;
            ClientHeight = 172;
            TopPanel.Visible = false;
            BottomPanel.Visible = true;
            Center();

            //Create the input box for text editing
            Input = new TextBox(manager);
            Input.Init();
            Input.Left = 8;
            Input.Top = 8;
            Input.Width = ClientArea.Width - 16;
            Input.Height = ClientArea.Height - 54;
            Input.Anchor = Anchors.All;
            Input.Mode = TextBoxMode.Multiline;
            Input.Text = (interact.level.tiles[interact.x, interact.y] as TextTile).Text;
            Input.SelectAll();
            Input.Focused = true;
            Input.ScrollBars = ScrollBars.Both;
            Add(Input);

            //Saves the text, Durrr
            Save = new Button(manager);
            Save.Init();
            Save.Text = "Save";
            Save.Width = 64;
            Save.Height = 24;
            Save.Top = 8;
            Save.Left = (ClientWidth / 2)- Save.Width - 8;
            Save.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
            {
                //If the tile is still a text tile (Incase it got destroyed) then set it's text
                if (interact.level.tiles[interact.x, interact.y] is TextTile)
                    (interact.level.tiles[interact.x, interact.y] as TextTile).Text = Input.Text;
                //Close up here
                Close();
            });

            //What could this do?
            Cancel = new Button(manager);
            Cancel.Init();
            Cancel.Text = "Cancel";
            Cancel.Width = 64;
            Cancel.Height = 24;
            Cancel.Top = 8;
            Cancel.Left = (ClientWidth / 2) + 8;
            Cancel.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
            {
                Close(); //Self-Explanitory
            });

            //Add the new controls
            BottomPanel.Add(Save);
            BottomPanel.Add(Cancel);

        }
    }
}
