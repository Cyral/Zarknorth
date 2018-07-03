//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using ZarknorthClient.Entities;

//namespace ZarknorthClient
//{
//    public class Ball
//    {
//        public static Level Level;
//        public BallItem item;
//        public Vector2 Velocity;
//        public Vector2 Position;
//        public float Rotation;
//        public Vector2 previousPosition;
//        public bool isOnGround;
//        public float previousBottom;
//        private Rectangle localBounds;
//        bool colidedX;
//        bool colidedY;

//        public Rectangle BoundingRectangle
//        {
//            get
//            {
//                int left = (int)Math.Round(Position.X - (item.Size.X / 2)) + localBounds.X;
//                int top = (int)Math.Round(Position.Y -(item.Size.Y / 2)) + localBounds.Y;

//                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
//            }
//        }


//        public Ball(BallItem item,Vector2 position)
//        {
//            this.item = item;
//            Position = position;
//            Rotation = 0f;
//            localBounds = new Rectangle(0, 0, (int)item.Size.X,(int) item.Size.Y);
//        }
//        public void Update(GameTime gameTime)
//        {
//            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
//            Velocity.Y += 6f;
//            Velocity.Y /= 1.008f;
//            Velocity.X /= 1.003f;
//            //Position += Velocity;
//            if (item.ID == Item.Basketball.ID)
//                Rotation = MathHelper.ToRadians(360 - (15f * Velocity.X));
//            else if (item.ID == Item.Football.ID)
//                Rotation = (float)Math.Atan2(Velocity.Y, Velocity.X);
     
//            {
//                Vector2 change = Velocity.X * Vector2.UnitX * elapsed;
//                change.X = MathHelper.Clamp(change.X, -(Tile.Height + 1), Tile.Height - 1);
//                Position += change;
//                HandleCollisions(CollisionDirection.Horizontal);
//            }
        
//            {
//                Vector2 change = Velocity.Y * Vector2.UnitY * elapsed;
//                change.Y = MathHelper.Clamp(change.Y, -(Tile.Height) + 1, Tile.Height - 1);
//                Position += change;
//                HandleCollisions(CollisionDirection.Vertical);
//            }
          
//            if (colidedX)
//            {
//                if (item.ID == Item.Basketball.ID)
//                    Velocity.X *= -1;
//                else if (item.ID == Item.Football.ID)
//                {
//                    Velocity.X *= -1;
//                    Velocity.X += Level.random.Next(-10, 10);
//                }
//                colidedX = false;
//            }
//            if( colidedY)
//            {

//                if (item.ID == Item.Basketball.ID)
//                    Velocity.Y*= -1;
//                else if (item.ID == Item.Football.ID)
//                {
//                    Velocity.Y *= -1;
//                    Velocity.Y += Level.random.Next(-10, 10);
//                }
//                colidedY = false;
              
//            }
//            previousPosition = Position;
            

//        }
//        /// <summary>
//        /// Detects and resolves all collisions between the player and his neighboring
//        /// tiles. When a collision is detected, the player is pushed away along one
//        /// axis to prevent overlapping. There is some special logic for the Y axis to
//        /// handle platforms which behave differently depending on direction of movement.
//        /// </summary>
//        private void HandleCollisions(CollisionDirection direction)
//        {
//            //// Get the player's bounding rectangle and find neighboring tiles.
//            //Rectangle bounds = BoundingRectangle;
//            //int leftTile = (int)Math.Floor((float)bounds.btnLeft / Tile.Width);
//            //int rightTile = (int)Math.Ceiling(((float)bounds.btnRight / Tile.Width)) - 1;
//            //int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
//            //int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

//            //// Reset flag to search for ground collision.
//            //isOnGround = false;

//            //// For each potentially colliding tile,
//            //for (int y = topTile; y <= bottomTile; ++y)
//            //{
//            //    for (int x = leftTile; x <= rightTile; ++x)
//            //    {
//            //        Rectangle tileBounds = Level.GetBounds(x, y);
//            //        // If this tile is collidable,
//            //        BlockCollision collision = Level.GetCollision(x, y);
//            //        Vector2 depth;
//            //        if (collision != BlockCollision.Passable && TileIntersectsPlayer(BoundingRectangle, tileBounds, direction, out depth))
//            //        {
//            //                // If we crossed the top of a tile, we are on the ground.
//            //                if (previousBottom <= tileBounds.Top)
//            //                {
                                
//            //                }
//            //                if (collision == BlockCollision.Impassable || isOnGround)
//            //                {
//            //                    if (direction == CollisionDirection.Horizontal)
//            //                    {
//            //                        Position.X += depth.X;
//            //                        colidedX = true;
//            //                    }
//            //                    if (direction == CollisionDirection.Vertical)
//            //                    {

//            //                        isOnGround = true;
//            //                       Position.Y += depth.Y;
//            //                       colidedY = true;
//            //                    }
//            //                }
//            //            }
//            //        }
//            //    }
//            //}
//            //// Save the new bounds bottom.
//            //previousBottom = bounds.Bottom;

//        }
//        public static bool TileIntersectsPlayer(Rectangle player, Rectangle block, CollisionDirection direction, out Vector2 depth)
//        {
//            depth = direction == CollisionDirection.Vertical ? new Vector2(0, player.GetVerticalIntersectionDepth(block)) : new Vector2(player.GetHorizontalIntersectionDepth(block), 0);
//            return depth.Y != 0 || depth.X != 0;
//        }

//    }
//}
