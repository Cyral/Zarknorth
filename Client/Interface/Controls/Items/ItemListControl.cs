using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class ItemListControl : Control
    {  
        public ImageBox Image;
        public Label Label;
        public Item Item;
        public StatusBar sb;
        public Color color, oldColor;


        public ItemListControl(Manager manager, Item item, Color color) : base(manager)
        {
            this.ToolTip.Text = item.Description;
            sb = new StatusBar(manager);
            sb.Init();
            sb.Alpha = .8f;
            color = new Color(((float)color.R / 255f) * sb.Alpha, ((float)color.G / 255f) * sb.Alpha, ((float)color.B / 255f) * sb.Alpha, sb.Alpha);
            sb.Color = color;
            Add(sb);
            Image = new ImageBox(manager);
            Image.Init();
            Image.Height = Tile.Height;
            Image.Width = Tile.Width;
            Image.Left = 4;
            Image.Top = 4;
            Image.Passive = true;
            Image.SizeMode = SizeMode.Fit; 
            Image.Image = ContentPack.Textures["items\\" + item.Name];
            Add(Image);

            Label = new Label(manager);
            Label.Init();
            Label.Height = Tile.Height;
            Label.Text = item.Name;
            Label.Left = Image.Left + Image.Width + 4;
            Label.Top = 0;
            Label.Text = item.Name;
            Add(Label);

            this.Item = item;
            this.Height = Tile.Height + 8;
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
            Label.Width = e.Width - (Label.Left + Label.Width);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
           // base.DrawControl(renderer, rect, gameTime);
        }

    }
}
