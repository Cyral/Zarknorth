using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ZarknorthClient.Entities;

namespace ZarknorthClient
{
    public abstract class BoxCollisionEntity : IEntity
    {
        #region IEntity Members
        /// <summary>
        /// Current position in pixels of the character
        /// </summary>
        public virtual Vector2 Position
        {
            get { return position; }
            set
            {
                value.Y = MathHelper.Clamp(value.Y, Tile.Width + localBounds.Height, (level.Height * Tile.Width) - Tile.Width);
                position = value;
            }
        }
        protected Vector2 position;

        /// <summary>
        /// Last position in pixels of the character
        /// </summary>
        public Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }
        protected Vector2 lastPosition;

        /// <summary>
        /// Current velocity of the character
        /// </summary>
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        protected Vector2 velocity;

        /// <summary>
        /// Current velocity of the character
        /// </summary>
        public Vector2 LastVelocity
        {
            get { return lastVelocity; }
            set { lastVelocity = value; }
        }
        protected Vector2 lastVelocity;

        public Level Level
        {
            get { return level; }
            set { level = value; }
        }
        protected Level level;
        #endregion

        public virtual Rectangle BoundingRectangle { get { return localBounds; } }
        protected Rectangle localBounds;
        public Rectangle previousBounds { get; set; }
        /// <summary>
        /// Is the character on a solid surface
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        protected bool isOnGround;
        protected bool lastIsOnGround;

        protected float MoveAcceleration;
        protected float MaxMoveSpeed;
        protected float GroundDragFactor;
        protected float AirDragFactor;

        protected float GravityAcceleration;
        protected float MaxFallSpeed;

        public BoxCollisionEntity(Level level, Vector2 position)
        {
            this.level = level;
            this.position = position;
            //Default speeds
            MaxMoveSpeed = 325.0f;
            AirDragFactor = .90f;
            GravityAcceleration = 3000.0f;
            MaxFallSpeed = 250.0f;
        }
        public virtual void Update(GameTime gameTime, KeyboardState currentKeyState, KeyboardState lastKeyState)
        {
            position.Y = MathHelper.Clamp(position.Y, Tile.Width + localBounds.Height, (level.Height * Tile.Width) - Tile.Width);
            ApplyPhysics(gameTime);
        }
        public virtual void DebugDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Game.DebugView)
                spriteBatch.DrawRectangle(BoundingRectangle, Color.Orange);
        }
        /// <summary>
        /// Apply physics to the item, To fall down and handle collisions
        /// </summary>
        /// <param name="gameTime">Snapshot of timing values</param>
        public virtual void ApplyPhysics(GameTime gameTime, bool DefaultVelocity = true, bool CancelVelocity = true, bool Round = true)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPosition = position;
            if (DefaultVelocity)
                 CalculateVelocity(elapsed);
            Vector2 prev = position;
            {
                Vector2 change = velocity.X * Vector2.UnitX * elapsed;
                change.X = MathHelper.Clamp(change.X, -(Tile.Height), Tile.Height);
                position += change;
                if (Round)
                position = new Vector2((float)Math.Round(position.X), position.Y);
                HandleCollisions(CollisionDirection.Horizontal, gameTime);
            }
           
            {
                Vector2 change = velocity.Y * Vector2.UnitY * elapsed;
                change.Y = MathHelper.Clamp(change.Y, -(Tile.Height), Tile.Height);
                position += change;
                if (Round)
                position = new Vector2(position.X, (float)Math.Round(position.Y));
                HandleCollisions(CollisionDirection.Vertical, gameTime);
            }
            if (CancelVelocity)
            {
                // If the collision stopped us from moving, reset the velocity to zero.
                if (position.X == lastPosition.X)
                    velocity.X = 0;
                if (position.Y == lastPosition.Y)
                    velocity.Y = 0;
            }
        }
        /// <summary>
        /// Default velocity for BoxCollision Entities
        /// </summary>
        private void CalculateVelocity(float elapsed)
        {
            velocity.X *= AirDragFactor;
            velocity.Y += GravityAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(
                velocity.Y,
                -MaxFallSpeed,
                MaxFallSpeed);
            // Prevent the item from moving faster than its top speed.            
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        protected void HandleCollisions(CollisionDirection direction, GameTime gameTime)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    Rectangle tileBounds = Level.GetBounds(x,y);
                    
                    // If this tile is collidable,
                    BlockCollision collision = Level.GetCollision(x, y);
                    Vector2 depth;
      

                    bool intersects = TileIntersectsPlayer(BoundingRectangle, tileBounds, direction, out depth);
                    HandleCollisions(gameTime, direction, collision, tileBounds, depth, intersects,x,y);
                }
            }
            // Save the new bounds bottom.
            previousBounds = bounds;

        }
        /// <summary>
        /// Handles collisions for a given block, don't call base.HandleCollision 
        /// </summary>
        protected virtual bool HandleCollisions(GameTime gameTime, CollisionDirection direction,BlockCollision collision, Rectangle tileBounds, Vector2 depth, bool intersects, int x, int y)
        {
            if (collision != BlockCollision.Passable && intersects)
            {
                // If we crossed the top of a tile, we are on the ground.
                if (previousBounds.Bottom <= tileBounds.Top)
                {
                    if (collision == BlockCollision.Ladder || collision == BlockCollision.Platform || collision == BlockCollision.Falling)
                    {
                        isOnGround = true;
                    }
                }
                if (collision == BlockCollision.Impassable || isOnGround)
                {
                    if (direction == CollisionDirection.Horizontal)
                    {
                        position.X += depth.X;
                    }
                    if (direction == CollisionDirection.Vertical)
                    {
                        isOnGround = true;
                        position.Y += depth.Y;
                    }
                }
            }
            return false;
        }
        public static bool TileIntersectsPlayer(Rectangle player, Rectangle block, CollisionDirection direction, out Vector2 depth)
        {
            depth = direction == CollisionDirection.Vertical ? new Vector2(0, player.GetVerticalIntersectionDepth(block)) : new Vector2(player.GetHorizontalIntersectionDepth(block), 0);
            return depth.Y != 0 || depth.X != 0;
        }

        /// <summary>
        /// Resets physics variables
        /// </summary>
        protected virtual void ResetPhysics()
        {
            lastIsOnGround = IsOnGround;
        }
    }
}
