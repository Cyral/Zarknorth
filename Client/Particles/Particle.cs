using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZarknorthClient.Entities;

namespace ZarknorthClient
{
    /// <summary>
    /// Particle class to handle drawing and variables of particles
    /// </summary>
    public class Particle : BoxCollisionEntity
    {
        #region Properties
        /// <summary>
        /// The type of particle (Ex, Fire, Water, Rain)
        /// </summary>
        public ParticleType Type;
        /// <summary>
        /// The texture to be draw to represent the particle
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// The current angle of rotation of the particle
        /// </summary>
        public float Angle { get; set; }
        /// <summary>
        /// The speed that the angle is changing
        /// </summary>
        public float AngularVelocity { get; set; }
        /// <summary>
        /// The ORIGINAL color of the particle
        /// </summary>
        public Color BaseColor { get; private set; }
        /// <summary>
        /// The size of the particle
        /// </summary>
        public float Size { get; set; }
        /// <summary>
        /// The 'time to live' of the particle
        /// </summary>
        public int TTL { get; set; }
        /// <summary>
        /// The speed of the particle
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// The velocity of the particle should be pushed
        /// </summary>
        public Vector2 Wind { get; set; }
        /// <summary>
        /// The velocity of the particle should be pulled
        /// </summary>
        public Vector2 Gravity { get; set; }
        /// <summary>
        /// Should the particle fade out as it dies?
        /// </summary>
        public bool AlphaFade { get; set; }
        /// <summary>
        /// Should the particle emit light?
        /// </summary>
        public bool EmitColor { get; set; }
        /// <summary>
        /// The current color of the particle
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// The transparent value of the particle
        /// </summary>
        public float Alpha { get; private set; }
        /// <summary>
        /// If the particle should shrink as TTL decreases
        /// </summary>
        public bool Shrink { get; private set; }
        /// <summary>
        /// Indicates if the particle has collision and will interact with the level blocks
        /// </summary>
        public bool Collision { get; set; }
        #endregion

        #region Fields
        private int TTLoriginal;
        private float SizeOriginal; 
        #endregion

        /// <summary>
        /// The area this occupies in world space
        /// </summary>
        public override Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(Texture.Width * Size), (int)(Texture.Height * Size));
            }
        }

        /// <summary>
        /// The source area of the texture to be drawn
        /// </summary>
        public Rectangle SourceRectangle { get; set; }

        /// <summary>
        /// Creates a new Particle, a small texture that has physics
        /// </summary>
        /// <param name="texture">The texture to be draw to represent the particle</param>
        /// <param name="type">The type of particle (Ex, Fire, Water, Rain)</param>
        /// <param name="position">The starting position of the particle</param>
        /// <param name="velocity">The starting velocity of the particle</param>
        /// <param name="angle">The starting angle of rotation of the particle</param>
        /// <param name="angularVelocity">The speed that the angle is changing</param>
        /// <param name="color">The base color of the particle</param>
        /// <param name="size">The scale of the particle</param>
        /// <param name="ttl">The 'time to live' of the particle</param>
        /// <param name="speed">The speed of the particle</param>
        /// <param name="gravity">The velocity of the particle should be pulled</param>
        /// <param name="wind">The velocity of the particle should be pushed</param>
        /// <param name="collision">If the particle has collision and will interact with the level blocks</param>
        /// <param name="shrink">Indicates if the particle should shrink as the TTL gets smaller</param>
        public Particle(Level level,Texture2D texture,ParticleType type, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, int ttl,float speed,Vector2 gravity, Vector2 wind, bool collision,bool shrink) : base(level,position)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            BaseColor = color;
            Size = SizeOriginal =  size;
            TTL = TTLoriginal = ttl;
            Speed = speed;
            Wind = wind;
            Gravity = gravity;
            Collision = collision;
            Shrink = shrink;
            Type = type;
            Color = BaseColor;

            SourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height); //Specify source textles

            Alpha = 1f;
        }
        /// <summary>
        /// Update particles
        /// </summary>
        public override void Update(GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState currentKeyState, Microsoft.Xna.Framework.Input.KeyboardState lastKeyState)
        {
            //Add velocity
            Velocity = Velocity - Gravity + Wind;
             
            if (TTL > 0)
            TTL--; //Decrease time to live as game goes on

            if (Shrink) //Shrink
                Size = SizeOriginal * (TTL>0 ? (float)TTL / TTLoriginal : TTLoriginal / (float)TTLoriginal);
            if (AlphaFade) //Fade out
            {
                Alpha = (float)TTL / (float)TTLoriginal;
                Color = new Color((byte)(BaseColor.R * Alpha), (byte)(BaseColor.G * Alpha),(byte)( BaseColor.R * Alpha), (byte)(BaseColor.A * Alpha));
            }


            if (!Collision)
                Position += (Velocity * Speed) * ((float)gameTime.ElapsedGameTime.TotalSeconds * 60); //Set Position based on Velocity

            Angle += AngularVelocity; //Set Angle based on AngularVelocity

            if (Collision)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                LastPosition = Position;

                Rectangle bounds = BoundingRectangle;
                int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
                int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
                int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
                int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

                velocity.Y += GravityAcceleration * elapsed;
                velocity.Y = MathHelper.Clamp(
                    Velocity.Y,
                    -MaxFallSpeed,
                    MaxFallSpeed);

                Velocity *= Speed;

                if (Velocity.X != 0f)
                {
                    Vector2 change = Velocity.X * Vector2.UnitX * elapsed;
                    change.X = MathHelper.Clamp(change.X, -(Tile.Height - 12), Tile.Height - 12);
                    Position += change;
                    HandleCollisions(CollisionDirection.Horizontal);
                }
                if (Velocity.Y != 0f)
                {
                    Vector2 change = Velocity.Y * Vector2.UnitY * elapsed;
                    change.Y = MathHelper.Clamp(change.Y, -(Tile.Height - 12), Tile.Height - 12);
                    Position += change;
                    if (Type == ParticleType.Snow && AlphaFade)
                        position = new Vector2(position.X, (float)Math.Round(position.Y));
                    HandleCollisions(CollisionDirection.Vertical);
                }

                // If the collision stopped us from moving, reset the Velocity to zero.
                if (Position.X == LastPosition.X)
                {
                    velocity.X = 0;
                }

                if (Position.Y == LastPosition.Y)
                {
                    velocity.Y = 0;
                }
            }
        }

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions(CollisionDirection direction)
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
                    Rectangle tileBounds = level.GetBounds(x, y);
                    // If this tile is collidable,
                    BlockCollision collision = level.GetCollision(x, y);
                    Vector2 depth;
                    if (collision != BlockCollision.Passable && PlayerCharacter.TileIntersectsPlayer(BoundingRectangle, tileBounds, direction, out depth))
                    {
                        if (collision == BlockCollision.Impassable || isOnGround || collision == BlockCollision.Platform || collision == BlockCollision.Falling)
                        {
                            if (direction == CollisionDirection.Horizontal)
                            {
                                position.X += depth.X;
                            }
                            else
                            {
                                if (Type == ParticleType.Rain)
                                {
                                    level.DefaultParticleEngine.SpawnParticle(ParticleType.RainSplash, (int)Position.X, (int)Position.Y + bounds.Height - 3);
                                    //Remove particle
                                    TTL = 0;
                                    //Make rain put out fire
                                    if (level.InLevelBounds(Position) && (level.tiles[(int)(Position.X / Tile.Width), (int)(Position.Y / Tile.Width)].ForegroundFireMeta > 0 || level.tiles[(int)(Position.X / Tile.Width), (int)(Position.Y / Tile.Width)].BackgroundFireMeta > 0))
                                        level.tiles[(int)(Position.X / Tile.Width), (int)(Position.Y / Tile.Width)].ForegroundFireMeta = level.tiles[(int)(Position.X / Tile.Width), (int)(Position.Y / Tile.Width)].BackgroundFireMeta = 0;
                                }
                                else if (Type == ParticleType.Snow)
                                {
                                    if (!AlphaFade)
                                    {
                                        TTL = TTLoriginal = 20;
                                        AlphaFade = true;
                                    }
                                }
                                isOnGround = true;
                                position.Y += depth.Y;
                            }
                        }
                    }
                }
            }
            // Save the new bounds bottom.
            previousBounds = bounds;
        }
        /// <summary>
        /// Draw the particles
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to draw with</param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2); //Specify Sprite origin
            spriteBatch.Draw(Texture, Position, SourceRectangle,Color,Angle ,origin, Size, SpriteEffects.None, 0f); //Draw Particles
            if (Game.DebugView)
                spriteBatch.DrawRectangle(new Rectangle(BoundingRectangle.X - (int)(origin.X * Size), BoundingRectangle.Y - (int)(origin.Y * Size), BoundingRectangle.Width, BoundingRectangle.Height), Color.DarkOrange);
        }
    }
}
