using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using ZarknorthClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZarknorthClient
{
    public class FloatLabel : Label
    {
        public double lifetime;
        public Vector2 Velocity;
        public Vector2 Position;
       // public Color Color;
        public float scale = 1f;
        public float rotation = 0f;
        protected float rotationRandom;
        /// <summary>
		/// Creates a new label_TextChanged control.
		/// </summary>
		/// <param name="manager">GUI management object for the label_TextChanged control.</param>
		public FloatLabel(Manager manager, string text,Vector2 position,Color color, Vector2 velocity, int Rotation) : base(manager)
		{
            Color = color;
            Text = text;
            Position = position;
            Velocity = velocity;
            rotationRandom = MathHelper.ToRadians(Game.level.random.Next(-Rotation, Rotation));
            SetSize(Game.MainWindow.ClientWidth, Game.MainWindow.ClientHeight);
		}
        protected override void Update(GameTime gameTime)
        {
            if (Game.level != null)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position += (Velocity * new Vector2(elapsed, elapsed));

                scale -= elapsed / 4;
                scale = Math.Max(scale, 0);
                Velocity.Y += elapsed * 5;

                rotation = MathHelper.Lerp(rotation, rotationRandom, 0.015f);
                if (scale == 0)
                {
                    Game.MainWindow.Remove(this);
                    if (this is SlotFloatLabel)
                        Game.level.Points.Remove((SlotFloatLabel)this);
                }
                base.Update(gameTime);
            }
            else
                Parent.Remove(this);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
           // base.DrawControl(renderer, rect, gameTime);
            if (Game.level != null)
            {
                renderer.SpriteBatch.DrawString(Manager.Skin.Fonts["Default14"].Resource, Text, Position - Game.level.MainCamera.Position + Vector2.One, Color.Lerp(Color, Color.Black, .5f), rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
                renderer.SpriteBatch.DrawString(Manager.Skin.Fonts["Default14"].Resource, Text, Position - Game.level.MainCamera.Position, Color, rotation, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
    }
}
