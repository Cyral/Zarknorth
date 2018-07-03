using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class ProfileColorPicker : Control
    {
        #region Properties
        public override string Text
        {
            get
            {
                return label.Text;
            }
            set
            {
                label.Text = value;
            }
        }
        public Color SelectedColor
        {
            get
            {
                return Colors[Selected];
            }
            set
            {
                for (int i =0; i<Colors.Length; i++)
                    if (Colors[i] == value)
                        Selected = i;
                ButtonClick(Buttons[Selected], null);
            }
        }
        public Color[] Colors;
        private Label label;
        public Button[] Buttons;
        public int Selected;
        #endregion

        #region Controls
        #endregion

        public ProfileColorPicker(Manager manager, string name, params Color[] colors)
            : base(manager)
        {
            Passive = false;
            label = new Label(manager);
            label.Init();
            label.Text = name;
            label.Alignment = Alignment.MiddleRight;
            label.Height = 16;
            label.Width = 72;
            Add(label);
            Colors = colors;
            Buttons = new Button[Colors.Length];
            for (int i = 0; i < Colors.Length; i++)
            {
                Button b = new Button(manager);
                b.Init();
                b.Left = label.Left + label.Width + 4 + (18 * i);
                b.Height = 16;
                b.Width = 16;
                b.Glyph = new Glyph(ContentPack.Textures["gui\\icons\\color"]);
                b.Glyph.Color = Colors[i];
                b.Text = string.Empty;
                b.Click += ButtonClick;
                Buttons[i] = b;
                Add(Buttons[i]);
            }
            Width = label.Width + 4 + (colors.Length * 18);
            Height = 16;
        }
        public void ButtonClick(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            foreach (Button btn in Buttons)
                btn.Color = Color.White;
            for (int i = 0; i < Buttons.Length; i++)
                if (Buttons[i] == sender)
                    Selected = i;
            (sender as Button).Color = Color.Black;
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            //base.DrawControl(renderer,rect,gameTime);
        }
    }
}
