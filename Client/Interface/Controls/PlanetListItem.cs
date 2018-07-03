using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class MapListItem : Control
    {
        public Label MapName, Author, Description;
        public StatusBar sb;

        public MapListItem(Manager manager, LevelData data)
            : base(manager)
        {
            sb = new StatusBar(manager);
            sb.Init();
            sb.Alpha = .8f;
            Add(sb);

            MapName = new Label(manager) { Left = 4, Top = 4, Text = data.Name, Font = FontSize.Default14, Alignment = TomShane.Neoforce.Controls.Alignment.TopLeft, Width = 200, Height = 20 };
            MapName.Init();
            Add(MapName);

            Author = new Label(manager) { Left = 4, Top = MapName.Top + MapName.Height + 4, Text = "By: " + data.Author, Alignment = TomShane.Neoforce.Controls.Alignment.TopLeft, Width = 200, Height = 12 };
            Author.Init();
            Add(Author);

            Description = new Label(manager) { Left = 4, Top = Author.Top + Author.Height + 4, Text = data.Description.Split('\n')[0], Alignment = TomShane.Neoforce.Controls.Alignment.TopLeft, Width = 200, Height = 12, ClientHeight = 12 };
            Description.Init();
            Add(Description);

            this.Height = 64;
            this.Width = 300;
            ItemListControl_Resize(this, new ResizeEventArgs());
            Resize += ItemListControl_Resize;
        }
        void ItemListControl_Resize(object sender, ResizeEventArgs e)
        {
            MapName.Width = e.Width - (MapName.Left + MapName.Width);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            // base.DrawControl(renderer, rect, gameTime);
        }

    }
}
