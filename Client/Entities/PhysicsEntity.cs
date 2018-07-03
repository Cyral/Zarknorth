using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZarknorthClient.Entities
{
    public class PhysicsEntity : BoxCollisionEntity
    {
        public PhysicsItem Item { get; set; }

        public override Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - (Item.Diameter / 2)) + localBounds.X;
                int top = (int)Math.Round(Position.Y - (Item.Diameter / 2)) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        private bool CollideX, CollideY;
        private float rotation;

        public PhysicsEntity(Level level, PhysicsItem item, Vector2 position) : base(level, position)
        {
            Item = item;
            localBounds = new Rectangle(0, 0, (int)Item.Diameter, (int)Item.Diameter);
        }

        public override void Update(GameTime gameTime, KeyboardState currentKeyState, KeyboardState lastKeyState)
        {
            CollideX = CollideY = false;
            lastPosition = position;
            float elapsed = (float)(gameTime.ElapsedGameTime.TotalSeconds);

            velocity += Item.Gravity;
            velocity.Y /= Item.GravityMultiplier.Y;
            velocity.X /= Item.GravityMultiplier.X;

            {
                Vector2 change = velocity.X * Vector2.UnitX * elapsed;
                change.X = MathHelper.Clamp(change.X, -(Tile.Height), Tile.Height);
                position += change;
                Vector2 prev = position;
                HandleCollisions(CollisionDirection.Horizontal, gameTime);
                if (position.X != prev.X)
                    CollideX = true;
            }

            {
                Vector2 change = velocity.Y * Vector2.UnitY * elapsed;
                change.Y = MathHelper.Clamp(change.Y, -(Tile.Height), Tile.Height);
                position += change;
                Vector2 prev = position;
                HandleCollisions(CollisionDirection.Vertical, gameTime);
                if (position.Y != prev.Y)
                    CollideY = true;
            }

            if (CollideX)
            {
                velocity.X *= -1;
                velocity.Y /= Item.CollisionMultiplier.X;
                position = new Vector2((float)Math.Round(position.X), position.Y);
            }
            if (CollideY)
            {
                if (velocity.Y > 135)
                {
                    velocity.Y *= -1;
                    velocity.Y /= Item.CollisionMultiplier.Y;
                    position = new Vector2(position.X, (float)Math.Round(position.Y));
                }
                else
                    velocity.Y = 0;
            }

            float distanceMoved = lastPosition.X - position.X;
            float circumfrenceOfCircle = Item.RotationDiameter == 0 ? Item.Diameter : Item.RotationDiameter * (float)Math.PI;
            float amountToRotateInDegrees = distanceMoved / circumfrenceOfCircle * 360;
            rotation -= MathHelper.ToRadians(amountToRotateInDegrees);
            position = new Vector2(position.X, (float)Math.Round(position.Y));

            if (Math.Round(Position.X / Tile.Width) != Math.Round(lastPosition.X / Tile.Width) ||Math.Round(Position.Y / Tile.Height) != Math.Round(lastPosition.Y / Tile.Height))
                Level.ComputeLighting = true;
        }
        public override void Draw(GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ContentPack.Textures["items\\" + Item.Name], Position, null, Color.White, rotation, new Vector2(Item.Size.X / 2, Item.Size.Y / 2), 1f, SpriteEffects.None, 0);
            base.Draw(gameTime, spriteBatch);
        }
    }
}
