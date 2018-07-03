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
    public class TaskRenamePlanet : Dialog
    {
        public const int MaxNameLength = 22;
        private Button Save;
        private Button Cancel;
        private TextBox Input;

        /// <summary>
        /// Create a new window for editng Text tiles, eg signs
        /// </summary>
        /// <param name="manager">UI Manager Object</param>
        /// <param name="interact">Interaction Parameters, includes level and position of sign</param>
        public TaskRenamePlanet(Manager manager, PlanetaryObject planet)
            : base(manager)
        {
            //Set up the window
            Text = "Rename Planet";
            Resizable = false;
            Init();
            ClientWidth = 175;
            ClientHeight = 74;
            TopPanel.Visible = false;
            BottomPanel.Visible = true;
            DefaultControl = Input;
            Center();

            //Create the input box for text editing
            Input = new TextBox(manager);
            Input.Init();
            Input.Left = 8;
            Input.Top = 8;
            Input.Width = ClientArea.Width - 16;
            Input.Anchor = Anchors.All;
            Input.Text = planet.Name;
            Input.SelectAll();
            Input.Focused = true;
            Input.TextChanged += Input_TextChanged;
            Add(Input);

            //Saves the text, Durrr
            Save = new Button(manager);
            Save.Init();
            Save.Text = "Save";
            Save.Width = 64;
            Save.Height = 24;
            Save.Top = 8;
            Save.Left = (ClientWidth / 2) - Save.Width - 8;
            Save.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
            {
                try
                {
                    planet.Name = Input.Text.Trim();
                    Game.UniverseViewer.CenterCamera();
                }
                catch
                {
                    System.Diagnostics.Debug.Assert(false, "Error");
                }
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
        void Input_TextChanged(object sender, Controls.EventArgs e)
        {
            Save.Enabled = true;
            if (Input.Text.Length > MaxNameLength)
                Input.Text = Input.Text.Substring(0, Math.Min(Input.Text.Length, MaxNameLength));
            foreach (Galaxy galaxy in Game.UniverseViewer.Systems)
                foreach (SolarSystem solarsystem in galaxy.Children)
                    foreach (PlanetaryObject planet in solarsystem.Children)
                        if (planet.Name == Input.Text)
                            Save.Enabled = false;
            if (!Cyral.Extensions.StringExtensions.IsFileNameSafe(Input.Text))
                Save.Enabled = false;
        }
    }
    public class TaskRenameMap : Dialog
    {
        public const int MaxNameLength = 22;
        private Button Save;
        private Button Cancel;
        private TextBox Input;
        private TaskSandbox task;
        /// <summary>
        /// Create a new window for editng Text tiles, eg signs
        /// </summary>
        /// <param name="manager">UI Manager Object</param>
        /// <param name="interact">Interaction Parameters, includes level and position of sign</param>
        public TaskRenameMap(Manager manager, TaskSandbox task, string Map)
            : base(manager)
        {
            this.task = task;
            //Set up the window
            Text = "Rename Map";
            Resizable = false;
            Init();
            ClientWidth = 175;
            ClientHeight = 74;
            TopPanel.Visible = false;
            BottomPanel.Visible = true;
            DefaultControl = Input;
            Center();

            //Create the input box for text editing
            Input = new TextBox(manager);
            Input.Init();
            Input.Left = 8;
            Input.Top = 8;
            Input.Width = ClientArea.Width - 16;
            Input.Anchor = Anchors.All;
            Input.Text = Map;
            Input.SelectAll();
            Input.Focused = true;
            Input.TextChanged += Input_TextChanged;
            Add(Input);

            //Saves the text, Durrr
            Save = new Button(manager);
            Save.Init();
            Save.Text = "Save";
            Save.Width = 64;
            Save.Height = 24;
            Save.Top = 8;
            Save.Left = (ClientWidth / 2) - Save.Width - 8;
            Save.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object sender, TomShane.Neoforce.Controls.EventArgs e)
            {
                try
                {
                    IO.RenameMap(Map, Input.Text);
                }
                catch
                {
                    System.Diagnostics.Debug.Assert(false, "Error");
                }
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
        void Input_TextChanged(object sender, Controls.EventArgs e)
        {
            Save.Enabled = true;
            if (Input.Text.Length > MaxNameLength)
                Input.Text = Input.Text.Substring(0, Math.Min(Input.Text.Length, MaxNameLength));
            foreach (MapListItem i in task.MapList.Items)
                if (i.MapName.Text == Input.Text)
                    Save.Enabled = false;
            if (!Cyral.Extensions.StringExtensions.IsFileNameSafe(Input.Text))
                Save.Enabled = false;
        }
    }
}
