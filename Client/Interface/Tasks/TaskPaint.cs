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
    public class TaskPaint : Dialog
    {
        private Button[] ColorBtns;
        private Button WhiteButton;
        private Button ClearButton;
        private Button HelpButton;

        public TaskPaint(Manager manager)
            : base(manager)
        {
            Text = "Paint";
            Center();
            BottomPanel.Visible = false;
            TopPanel.Visible = false;
            Resizable = false;
            ColorBtns = new Button[Item.StandardColors.Length];

            int BtnsWidth = 15;
            int BtnsHeight = (int)Math.Round((float)ColorBtns.Length / (float)BtnsWidth);
            for (int x = 0; x < BtnsWidth; x++)
            {
                for (int y = 0; y <= BtnsHeight; y++)
                {
                    int i = y * BtnsWidth + x;
                    if (i < ColorBtns.Length)
                    {
                        ColorBtns[i] = new Button(manager) { Enabled = false };
                        ColorBtns[i].Init();
                        ColorBtns[i].Width = ColorBtns[i].Height = 24;
                        ColorBtns[i].Left = 4 + (x * (ColorBtns[i].Width + 2));
                        ColorBtns[i].Top = 30 + (y * (ColorBtns[i].Height + 2));
                        ColorBtns[i].Text = "";


                        ColorBtns[i].Glyph = new Glyph(ContentPack.Textures["gui\\icons\\paint" + Game.level.random.Next(1, 6)]);
                        ColorBtns[i].Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
                        {
                            WhiteButton.Color = Color.White;
                            ClearButton.Color = Color.White;
                            int selected = -1;
                            for (int j = 0; j < ColorBtns.Length; j++)
                            {
                                ColorBtns[j].Color = Color.White;
                                if ((Button)o == ColorBtns[j])
                                    selected = j;
                            }
                            ((Button)o).Color = new Color(50, 50, 50);
                            Game.level.SelectedPaintColor = selected + 2;
                        });
                        Add(ColorBtns[i]);
                    }
                }
            }
            ClientWidth = 4 + (BtnsWidth * (ColorBtns[1].Width + 2)) + 4;

            ClearButton = new Button(Manager);
            ClearButton.Init();
            ClearButton.Width = ClearButton.Height = 24;
            ClearButton.Left = (ClientWidth / 2) - (ClearButton.Width / 2) - 2;
            ClearButton.Top = 4;
            ClearButton.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\cross"]);
            ClearButton.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                WhiteButton.Color = Color.White;
                for (int j = 0; j < ColorBtns.Length; j++)
                {
                    ColorBtns[j].Color = Color.White;
                }
                ((Button)o).Color = new Color(50, 50, 50);
                Game.level.SelectedPaintColor = 0;
            });
            Add(ClearButton);

            HelpButton = new Button(Manager);
            HelpButton.Init();
            HelpButton.Width = HelpButton.Height = 24;
            HelpButton.Left = ClearButton.Left - HelpButton.Width - 2;
            HelpButton.Top = 4;
            HelpButton.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\help"]);
            HelpButton.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                MessageBox m = new MessageBox(Manager, MessageBoxType.Okay,
                @"You can use painting to spice up your creations.
Many blocks can be painted, click on any block with the
paint dialog open to see the colors you can use.
Hold " + Game.Controls["Place on Background"] + @" to switch from foreground to background.
Press the X icon to select the eraser tool.",
                "Paint Help");
                m.Init();
                m.ShowModal();
                m.Height = 170;
                Manager.Add(m);
            });
            Add(HelpButton);

            WhiteButton = new Button(Manager);
            WhiteButton.Init();
            WhiteButton.Width = WhiteButton.Height = 24;
            WhiteButton.Left = ClearButton.Width + ClearButton.Left + 2;
            WhiteButton.Top = ClearButton.Top;
            WhiteButton.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\paint" + Game.level.random.Next(1, 5)]);
            WhiteButton.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                ClearButton.Color = Color.White;
                for (int j = 0; j < ColorBtns.Length; j++)
                {
                    ColorBtns[j].Color = Color.White;
                }
                ((Button)o).Color = new Color(50, 50, 50);
                Game.level.SelectedPaintColor = 1;
            });
            Add(WhiteButton);

            MinimumHeight = ClientHeight = (BtnsHeight * (ColorBtns[1].Height + 2)) + 32;
        }
        public void DisableColors()
        {
            ClearButton.Enabled = false;
            WhiteButton.Enabled = false;
            for (int i = 0; i < ColorBtns.Length; i++)
            {
                ColorBtns[i].Enabled = false;
            }
        }
        public void UpdateColors(Color[] Colors)
        {
            ClearButton.Enabled = true;
            WhiteButton.Enabled = true;
            for (int i = 0; i < ColorBtns.Length; i++)
            {
                if (i < Colors.Length)
                {
                    Color c = Colors[i];
                    ColorBtns[i].Glyph.Color = c;
                    ColorBtns[i].Enabled = true;
                    ColorBtns[i].ToolTip.Text = "R: " + c.R + "\nG: " + c.G + "\nB: " + c.B;
                }
                else
                {
                    ColorBtns[i].Glyph.Color = Color.White;
                    ColorBtns[i].Enabled = false;
                }
            }
        }
        public override void Show()
        {
            base.Show();
        }

        public void Select(int i)
        {
            WhiteButton.Color = Color.White;
            ClearButton.Color = Color.White;
            for (int j = 0; j < ColorBtns.Length; j++)
            {
                ColorBtns[j].Color = Color.White;
            }
            if (i == 0)
                WhiteButton.Color = new Color(50, 50, 50);
            else if (i == 1)
                ClearButton.Color = new Color(50, 50, 50);
            else
            ColorBtns[i].Color = new Color(50, 50, 50);
        }
    }
}
