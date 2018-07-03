#region File Description
//-----------------------------------------------------------------------------
// Gem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using ZarknorthClient.Entities;
using Cyral.Extensions.Xna;

namespace ZarknorthClient
{
    /// <summary>
    /// A droped item in the level the player can collect ("pick up")
    /// </summary>
    public class CollectableItem : BoxCollisionEntity
    {
        #region Properties
        /// <summary>
        /// The item and stack the pickup item contains
        /// </summary>
        public Slot Slot { get { return slot; } set { slot = value; Texture = ContentPack.Textures["items\\" + slot.Item.Name]; } }
        /// <summary>
        /// Indicates if the item has been collected yet
        /// </summary>
        public bool Collected { get; private set; }

        /// <summary>
        /// The area this occupies in world space
        /// </summary>
        public override Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Tile.Width * Scale), (int)(Tile.Width * Scale));
            }
        }

        /// <summary>
        /// Rotation of the item
        /// </summary>
        public float Rotation { get; private set; }

        /// <summary>
        /// Scale of the item
        /// </summary>
        public float Scale { get; private set; }

        /// <summary>
        /// Texture of the item
        /// </summary>
        public Texture2D Texture { get; private set; }
        #endregion

        #region Fields
        /// <summary>
        /// The offset to give the item a bouncing effect
        /// </summary>
        private float bounce;
        /// <summary>
        /// Slot contained in the item
        /// </summary>
        private Slot slot;
        /// <summary>
        /// Distance vector used to rotate towards player
        /// </summary>
        private Vector2 Distance;
        /// <summary>
        /// Starting distance used for scaling
        /// </summary>
        private float initialDistance;
        private PlayerCharacter collector;
        private float speed = 1;
        #endregion

        #region Constants
        static readonly float PickupRadius = 5;
        // Bounce control constants
        private const float BounceHeight = 0.08f;
        private const float BounceRate = 3.0f;
        private const float BounceSync = .7f;
        #endregion

        /// <summary>
        /// Constructs a new pickup item
        /// </summary>
        public CollectableItem(Level level, Vector2 position, Vector2 velocity, Slot slot) : base(level,position)
        {
            Level = level;
            Position = position;
            Slot = slot;
            Velocity = velocity;
            Scale = .75f;
        }
        public void Update(GameTime gameTime)
        {
            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring items bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + (Position.X * 3) * BounceSync;
            float elapsed = (float)(gameTime.ElapsedGameTime.TotalSeconds);

            if (Collected)
            Scale = (Vector2.Distance(collector.OriginPosition, position) / initialDistance) * .75f;
            Scale = MathHelper.Clamp(Scale, 0, .75f);

            if (Collected)
            {
                Distance = new Vector2(level.Player.Position.X - position.X, level.Player.OriginPosition.Y - position.Y);
                //Start going towards the player
                position += Distance * speed * elapsed;
                speed += 8f * elapsed;
            }
            else
            {
                //If not collected, apply standard physics
                base.ApplyPhysics(gameTime, true);
                if (Velocity == Vector2.Zero)
                bounce = (float)Math.Sin(t) * BounceHeight * Tile.Width;
            }

            foreach (PlayerCharacter pl in level.Players)
            {
                if (BoundingRectangle.Contains(pl.OriginPosition.ToPoint()) || Scale < .075f) //If we touch it
                {
                    if (pl.FitsInInventory(slot))
                        Level.Collectables.Remove(this);
                    else
                        Collected = false;
                }
                //If we are within XXX blocks of it
                else if (Collected == false && Vector2.Distance(Position, pl.OriginPosition) <= Tile.Width * PickupRadius)
                {
                    if (pl.FitsInInventory(slot))
                    OnCollected(pl);
                }
            }
        }

        /// <summary>
        /// Called when this item has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player that collected the item
        /// </param>
        public void OnCollected(PlayerCharacter collectedBy)
        {
            initialDistance = Vector2.Distance(collectedBy.OriginPosition, position);
            collector = collectedBy;
            //Show achivements
            Achievement.Show(Achievement.PickUp);
            if (Slot.Item == Item.Log)
                Achievement.Show(Achievement.Chop);
            //Set to collected
            Collected = true;
            //Give player the items
            for (int i = 0; i < Slot.Stack; i++)
            {
                bool c = collectedBy.AddToInventory(Slot.Item);
            }
            //Show a floating label_TextChanged in level (like "Dirt (3)")
            level.AddSlotFloatingLabel(slot, collectedBy.Position - new Vector2(0, collectedBy.BoundingRectangle.Height), Color.White);
        }
        /// Draws a pickup item
        /// </summary>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //Get grid position
            int gridX = (int)Math.Floor(position.X / Tile.Width);
            int gridY = (int)Math.Floor(position.Y / Tile.Height);

            //Time since last draw
            float elapsed = (float)(gameTime.ElapsedGameTime.TotalSeconds);

            //If collected, Rotate towards player
            if (Collected)
            {
                Distance = new Vector2(level.Player.Position.X - position.X, level.Player.OriginPosition.Y - position.Y);
                //Rotate towards player and flip 180 degrees if on right of player
                float offset = level.Player.Position.X < Position.X ? MathHelper.ToRadians(180) : 0;
                Rotation = MathHelper.Lerp(Rotation,(float)Math.Atan2(Distance.Y, Distance.X) - offset, .7f * elapsed);
            }
            if (Game.DropShadows)
            {
                //Drop shadow
                spriteBatch.Draw(Texture, position - new Vector2(0.0f, bounce - (int)(Tile.Width * .25)) + Vector2.One, null, Color.Black * .15f, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, 0f);
                //Drop shadow 2
                spriteBatch.Draw(Texture, position - new Vector2(0.0f, bounce - (int)(Tile.Width * .25)) + Vector2.One + Vector2.One, null, Color.Black * .075f, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, 0f);
            }
            //Draw the item with rotation and at 75% of the real size
            spriteBatch.Draw(Texture, position - new Vector2(0.0f, bounce - (int)(Tile.Width * .25)), null, Color.White, Rotation, new Vector2(Texture.Width / 2, Texture.Height / 2), Scale, SpriteEffects.None, 0f);
            DebugDraw(gameTime, spriteBatch);
        }
        public override void DebugDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Game.DebugView)
                spriteBatch.DrawRectangle(BoundingRectangle, Color.Orange);
        }
    }
}
