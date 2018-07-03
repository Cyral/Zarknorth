//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using System.Collections.Generic;

//namespace ZarknorthClient
//{
//    /// <summary>
//    /// Non Player Characters (THIS FILE IS FOR THE AI ONLY)
//    /// </summary>
//    public partial class Npc
//    {
//        public void Update(GameTime gameTime)
//        {
//            #region UpdateStuff
//            //Elapsed time

//            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;


//            if (!IsAlive)
//                deathTime -= elapsed;
//            //Current X and Y positions (In blocks) of an ncp
//            tileX = (int)Math.Floor(position.X / Tile.Width);
//            tileY = (int)Math.Floor(position.Y / Tile.Height);
//            #endregion
        
//            #region Skeleton
//            if (ncpType == NpcType.Jellyfish)
//            {
//                if (IsAlive)
//                {
//                    // 1. If he finds a ledge, Turn around
//                    if (IsBigLedge())
//                        TurnAround();
//                    if (IsBigWall())
//                        TurnAround();
//                    if (IsWall())
//                        Jump();

                   
//                }

//                //3.  Apply physics and reset variables
//                ApplyPhysics(gameTime);
//                movement = Vector2.Zero;
//                isJumping = false;
//            }
//            if (ncpType == NpcType.Skeleton)
//            {
//                if (IsAlive)
//                {
//                    // 1. If he finds a ledge, Turn around
//                    if (IsBigLedge())
//                        TurnAround();
//                    if (IsBigWall())
//                        TurnAround();
//                    if (IsWall())
//                        Jump();

//                    // 2. Set the movement
//                    movement.X = (int)direction;
//                }

//                //3.  Apply physics and reset variables
//                ApplyPhysics(gameTime);
//                movement = Vector2.Zero;
//                isJumping = false;
               
//            }
//            if (ncpType == NpcType.Spider)
//            {
//                if (IsAlive)
//                {
//                    // 1. If he finds a ledge, Turn around
//                    if (IsBigLedge())
//                        TurnAround();
//                    if (IsBigWall())
//                        TurnAround();
//                    if (IsWall())
//                        Jump();

//                    // 2. Set the movement
//                    movement.X = (int)direction;
//                }

//                //3.  Apply physics and reset variables
//                ApplyPhysics(gameTime);
//                movement = Vector2.Zero;
//                isJumping = false;

//            }
//            #endregion

//        }
//    }
//}
