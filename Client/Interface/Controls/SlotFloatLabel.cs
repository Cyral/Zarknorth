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
    public class SlotFloatLabel : FloatLabel
    {
        public Slot Slot;
        /// <summary>
        /// Creates a new label_TextChanged control.
        /// </summary>
        /// <param name="manager">GUI management object for the label_TextChanged control.</param>
        public SlotFloatLabel(Manager manager, Slot s, Vector2 position, Color color, Vector2 velocity, int Rotation)
            : base(manager, s.Item.Name, position, color, velocity, Rotation)
        {
            Slot = s;
            scale = 2.5f;
        }
        protected override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Position += (Velocity * new Vector2(elapsed, elapsed));

            scale -= elapsed;
            scale = Math.Max(scale, 0);
            Velocity.Y += elapsed * 2;

            rotation = MathHelper.Lerp(rotation, rotationRandom, 0.015f * elapsed * 16);
            if (scale == 0)
            {
                Game.MainWindow.Remove(this);
                if (this is SlotFloatLabel)
                    Game.level.Points.Remove((SlotFloatLabel)this);
            }
         
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            if (Game.level == null)
            {
                Game.MainWindow.Remove(this);
                return;
            }
            // base.DrawControl(renderer, rect, gameTime);
            string suffix = Slot.Stack > 1 ? " (" + Slot.Stack + ")" : "";
            renderer.SpriteBatch.DrawString(Manager.Skin.Fonts["Default14"].Resource, Slot.Item.Name + suffix, Position - Game.level.MainCamera.Position + Vector2.One, Color.Lerp(Color, Color.Black, .5f), rotation, Vector2.Zero, MathHelper.Clamp(scale, 0, 1), SpriteEffects.None, 0);
            renderer.SpriteBatch.DrawString(Manager.Skin.Fonts["Default14"].Resource, Slot.Item.Name + suffix, Position - Game.level.MainCamera.Position, Color, rotation, Vector2.Zero, MathHelper.Clamp(scale, 0, 1), SpriteEffects.None, 0);
        }
    }
}
