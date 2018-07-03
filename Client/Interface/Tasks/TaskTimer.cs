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
    public class TaskTimer : Dialog
    {
        private Label timeLabel, lengthLabel;
        private TrackBar timeBar, lengthBar;
        private Button Save, Cancel;
        private float time;
        private float length;
        private InteractBlockEventArgs interact;
        private float orgLength, orgTime;

        /// <summary>
        /// Create a new window for editng Text tiles, eg signs
        /// </summary>
        /// <param name="manager">UI Manager Object</param>
        /// <param name="interact">Interaction Parameters, includes level and position of sign</param>
        public TaskTimer(Manager manager, InteractBlockEventArgs interact) : base(manager)
        {
            this.interact = interact;

            if (interact.level.tiles[interact.x, interact.y] is TimerTile)
            {
                orgTime = (interact.level.tiles[interact.x, interact.y] as TimerTile).Time;
                orgLength = (interact.level.tiles[interact.x, interact.y] as TimerTile).Length;
            }
            //Set up the window
            Text = "Edit Timer";
            Resizable = false;
            Init();
            Width = 300 + 16;
            ClientHeight = 142;
            TopPanel.Visible = false;
            TopPanel.Height = 0;
            BottomPanel.Visible = true;
            DefaultControl = timeBar;
            Center();

            timeLabel = new Label(manager);
            timeLabel.Init();
            timeLabel.Top = TopPanel.Height + 6;
            timeLabel.Left = 8;
            timeLabel.Width = ClientWidth - 32; 
            Add(timeLabel);

            timeBar = new TrackBar(manager);
            timeBar.Init();
            timeBar.Top = timeLabel.Top + timeLabel.Height + 4;
            timeBar.Range = (30 * 10) - 5;
            timeBar.Value = 0;
            timeBar.Width = ClientWidth - 12;
            timeBar.Height = 24;
            timeBar.Left = (ClientWidth / 2) - (timeBar.Width / 2);
            timeBar.ValueChanged += timeBar_ValueChanged;
            Add(timeBar);

            lengthLabel = new Label(manager);
            lengthLabel.Init();
            lengthLabel.Top = timeBar.Top + timeBar.Height + 4;
            lengthLabel.Left = 8;
            lengthLabel.Width = ClientWidth - 32;
            Add(lengthLabel);

            lengthBar = new TrackBar(manager);
            lengthBar.Init();
            lengthBar.Top = lengthLabel.Top + lengthLabel.Height + 4;
            lengthBar.Range = (30 * 10) - 5;
            lengthBar.Value = 0;
            lengthBar.Width = ClientWidth - 12;
            lengthBar.Height = 24;
            lengthBar.Left = (ClientWidth / 2) - (timeBar.Width / 2);
            lengthBar.ValueChanged += lengthBar_ValueChanged;
            Add(lengthBar);

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
                if (interact.level.tiles[interact.x, interact.y] is TimerTile)
                {
                    (interact.level.tiles[interact.x, interact.y] as TimerTile).Time = time;
                    (interact.level.tiles[interact.x, interact.y] as TimerTile).Length = length;
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
                if (interact.level.tiles[interact.x, interact.y] is TimerTile)
                {
                    (interact.level.tiles[interact.x, interact.y] as TimerTile).Time = orgTime;
                    (interact.level.tiles[interact.x, interact.y] as TimerTile).Length = orgLength;
                }
                Close(); //Self-Explanitory
            });

            //Add the new controls
            BottomPanel.Add(Save);
            BottomPanel.Add(Cancel);

            if (interact.level.tiles[interact.x, interact.y] is TimerTile)
            {
                timeBar.Value = (int)(((interact.level.tiles[interact.x, interact.y] as TimerTile).Time * 10) - 5);
                lengthBar.Value = (int)(((interact.level.tiles[interact.x, interact.y] as TimerTile).Length * 10) - 5);
                timeBar_ValueChanged(null, null);
                lengthBar_ValueChanged(null, null);
            }
        }

        void timeBar_ValueChanged(object sender, Controls.EventArgs e)
        {
            time = (timeBar.Value + 5) / 10f;
            time = MathHelper.Clamp(time, .5f, 30);
            timeLabel.Text = "Output a current every " + (time == 1 ? "second" : time + " seconds");
            timeBar.Color = Extensions.GetBlendedColor(100 - (int)((timeBar.Value / (float)timeBar.Range) * 100));
            if (interact.level.tiles[interact.x, interact.y] is TimerTile)
            {
                (interact.level.tiles[interact.x, interact.y] as TimerTile).Time = time;
            }
        }
        void lengthBar_ValueChanged(object sender, Controls.EventArgs e)
        {
            length = (lengthBar.Value + 5) / 10f;
            length = MathHelper.Clamp(length, .5f, 30);
            lengthLabel.Text = "Lasting for " + (length == 1 ? "1 second" : length + " seconds");
            lengthBar.Color = Extensions.GetBlendedColor(100 - (int)((lengthBar.Value / (float)lengthBar.Range) * 100));
            if (interact.level.tiles[interact.x, interact.y] is TimerTile)
            {
                (interact.level.tiles[interact.x, interact.y] as TimerTile).Length = length;
            }
        }
    }
}
