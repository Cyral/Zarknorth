using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace ZarknorthClient
{
    public class PlanetControl : Control
    {
        #region Properties
        public Texture2D Image { get { return image; } set { image = value; ClientWidth = image.Width; ClientHeight = image.Height + (Game.Fullscreen ? 56 : 24); } }
        public override string Text { get; set; }
        public string Description { get; set; }
        public int Font = 14;
        public int Radius
        {
            get { return (int)Math.Round(((Image.Width / 2) * .825)); }
        }
        float scale = 0f;
        #endregion
        #region Fields
        private float bounceHeight = 0.02f;
        private float bounceRate = 2.0f;
        private float curBounce = 0f;
        private float fade;
        private Texture2D image;
        private FadeState state;
        private enum FadeState
        { In, Out } 
        #endregion

        #region Constructor
        public PlanetControl(Manager manager)
            : base(manager)
        {
            MouseOut += PlanetControl_MouseOut;
            MouseOver += PlanetControl_MouseOver;
            state = FadeState.Out;
        } 
        #endregion
        #region Events
        void PlanetControl_MouseOver(object sender, MouseEventArgs e)
        {
            state = FadeState.In;
        }
        void PlanetControl_MouseOut(object sender, MouseEventArgs e)
        {
            state = FadeState.Out;
        }
        public override bool CheckPositionMouse(Point pos)
        {
            //Checks for intersection
            return (Interface.MainWindow.BoundingCircle(Left + 4 + (Width / 2), Top + 54 + (Height / 2), Radius, pos.X, pos.Y));
        }
        protected override void OnClick(TomShane.Neoforce.Controls.EventArgs e)
        {
            base.OnClick(e);
        }
        #endregion
        protected override void Update(GameTime gameTime)
        {
            //Bouncing of text
            double t = gameTime.TotalGameTime.TotalSeconds * bounceRate;
            curBounce = (float)System.Math.Sin(t) * bounceHeight * Image.Height;

            //Fading
            if (state == FadeState.In && fade < .6f)
                fade = MathHelper.Lerp(fade, .6f, (float)gameTime.ElapsedGameTime.TotalSeconds * 3);
            else if (state == FadeState.Out && fade > 0f)
                fade = MathHelper.Lerp(fade, 0, (float)gameTime.ElapsedGameTime.TotalSeconds * 3);

            fade = MathHelper.Clamp(fade, 0, .6f);

            scale = MathHelper.Lerp(scale, 1, (float)gameTime.ElapsedGameTime.TotalSeconds * 10);

            base.Update(gameTime);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            bool HasDescription = !string.IsNullOrWhiteSpace(Description);
            Color DrawColor = Color.Lerp(Color.White, Color.Black, fade * scale);
            if (!Enabled)
                DrawColor = Color.Gray;
            renderer.SpriteBatch.Draw(Image, new Vector2(Left + 4, Top + 64) + new Vector2((Image.Width / 2) * (1f - scale), (Image.Height / 2) * (1f - scale)), null, DrawColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (HasDescription)
            {
                renderer.SpriteBatch.DrawString(Manager.Skin.Fonts[(Font < 14 ? "Bold" : "Default") + Font].Resource, Text, new Vector2(Left + (Image.Width / 2) - (((int)Manager.Skin.Fonts[(Font < 14 ? "Bold" : "Default") + Font].Resource.MeasureString(Text).X / 2) * 1f), Top + 32 + curBounce), DrawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                renderer.SpriteBatch.DrawString(Manager.Skin.Fonts["Default8"].Resource, Description, new Vector2(Left + (Image.Width / 2) - (((int)Manager.Skin.Fonts["Default8"].Resource.MeasureString(Description).X / 2) * 1f), Top + 54 + curBounce), DrawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            else
            renderer.SpriteBatch.DrawString(Manager.Skin.Fonts[(Font < 14 ? "Bold" : "Default") + Font].Resource, Text, new Vector2(Left + (Image.Width / 2) - (((int)Manager.Skin.Fonts[(Font < 14 ? "Bold" : "Default") + Font].Resource.MeasureString(Text).X / 2) * 1f), Top + 48 + curBounce), DrawColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        }
    }
}
