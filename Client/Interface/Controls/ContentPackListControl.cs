using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class ContentPackListControl : Control
    {
        public ImageBox Image;
        public Label lblName;
        public Label lblInfo;
        public ContentPack Pack;
        public StatusBar sb;
        public Color color, oldColor;


        public ContentPackListControl(Manager manager, ContentPack pack, Color color)
            : base(manager)
        {
            ToolTip.Text = pack.Description;
            Pack = pack;

            sb = new StatusBar(manager);
            sb.Init();
            sb.Alpha = .8f;
            color = new Color(((float)color.R / 255f) * sb.Alpha, ((float)color.G / 255f) * sb.Alpha, ((float)color.B / 255f) * sb.Alpha, sb.Alpha);
            sb.Color = color;
            Add(sb);
            Image = new ImageBox(manager);
            Image.Init();
            Image.Height = 64;
            Image.Width = 64;
            Image.Left = 4;
            Image.Top = 4;
            Image.Passive = true;
            Image.SizeMode = SizeMode.Fit;
            Image.Image = pack.Icon;
            Add(Image);

            lblName = new Label(manager);
            lblName.Init();
            lblName.Height = Tile.Height;
            lblName.Text = pack.Name;
            lblName.Left = Image.Left + Image.Width + 4;
            lblName.Top = 4;
            lblName.Anchor = Anchors.Top;
            lblName.Alignment = Alignment.TopLeft;
            lblName.Text = pack.Name;
            lblName.Height = 20;
            lblName.Font = FontSize.Default14;
            Add(lblName);

            lblInfo = new Label(manager);
            lblInfo.Init();
            lblInfo.Height = Tile.Height;
            lblInfo.Text = pack.Name;
            lblInfo.Left = Image.Left + Image.Width + 6;
            lblInfo.Top = lblName.Top + lblName.Height + 4;
            lblInfo.Anchor = Anchors.Top;
            lblInfo.Alignment = Alignment.TopLeft;
            lblInfo.Text = pack.Description + "\nVersion: " + pack.Version + "\nBy: " + pack.Author;
            lblInfo.TextColor = Color.Gray;
            Add(lblInfo);

            Height = 72;

            ItemListControl_Resize(this, new ResizeEventArgs());
            Resize += ItemListControl_Resize;
            this.EnabledChanged += ItemListControl_EnabledChanged;
        }

        void ItemListControl_EnabledChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (Enabled)
            {
                sb.Alpha = .8f;
                color = new Color(((float)color.R / 255f) * sb.Alpha, ((float)color.G / 255f) * sb.Alpha, ((float)color.B / 255f) * sb.Alpha, sb.Alpha);
                sb.Color = color;
            }
            else
            {
                sb.Alpha = .5f;
                color = new Color(((float)color.R / 255f) * sb.Alpha, ((float)color.G / 255f) * sb.Alpha, ((float)color.B / 255f) * sb.Alpha, sb.Alpha);
                sb.Color = color;
            }
        }

        void ItemListControl_Resize(object sender, ResizeEventArgs e)
        {
            lblName.Width = e.Width - (lblName.Left + lblName.Width);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            // base.DrawControl(renderer, rect, gameTime);
        }

    }
}
