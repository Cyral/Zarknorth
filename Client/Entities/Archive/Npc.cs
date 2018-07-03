//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using ZarknorthClient.Entities;

//namespace ZarknorthClient
//{
//    /// <summary>
//    /// Non Player Characters
//    /// </summary>
//    public partial class Npc
//    {
//        #region Properties
//        //Properties
//        public NpcType ncpType;
//        public int Health;
//        public int tired;
//        public bool HitThisSwing;
//        // Animations
//        private Animation idleAnimation;
//        private Animation runAnimation;
//        private AnimationPlayer sprite;
//        int tileX;
//        int tileY;

//        public Level Level
//        {
//            get { return Game.level; }
//        }
//        Level level;

//        public bool IsAlive
//        {
//            get { return isAlive; }
//            set { isAlive = value; }
//        }
//        bool isAlive;
//        private const float deathTimeMax = 1.0f;
//        public float deathTime = deathTimeMax;
//        double speed;
//        // Physics state
//        public Vector2 Position
//        {
//            get { return position; }
//            set { position = value; }
//        }
//        Vector2 position;
//        public Vector2 previousPosition;

//        private float previousBottom;

//        public Vector2 Velocity
//        {
//            get { return velocity; }
//            set { velocity = value; }
//        }
//        Vector2 velocity;
//        SpriteEffects flip;
//        private Vector2 lerpingPosition;
//        // variables for controling horizontal movement
//        private float MoveAcceleration;
//        private float MaxMoveSpeed;
//        private float GroundDragFactor;
//        private float AirDragFactor;
        
//        // varaiables for controlling vertical movement
//        private float MaxJumpTime;
//        private float JumpLaunchVelocity;
//        private float GravityAcceleration;
//        private float MaxFallSpeed;
//        private float JumpControlPower;

//        /// <summary>
//        /// Gets whether or not the player's feet are on the ground.
//        /// </summary>
//        public bool IsOnGround
//        {
//            get { return isOnGround; }
//        }
//        bool isOnGround;
//        /// <summary>
//        /// Gets whether or not the player's feet are in watewr
//        /// </summary>
//        public bool InWater
//        {
//            get { return inWater; }
//        }
//        bool inWater;
        
//        /// <summary>
//        /// Current user movement input.
//        /// </summary>
//        public Vector2 movement;

//        // Jumping state
//        public bool isJumping;
//        private bool wasJumping;
//        private float jumpTime;
//        public float Angle;
//        private Rectangle localBounds;
//        /// <summary>
//        /// Gets a rectangle which bounds this player in world space.
//        /// </summary>
//        public Rectangle BoundingRectangle
//        {
//            get
//            {
//                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
//                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;
//                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
//            }
//        }


//        /// <summary>
//        /// The direction this enemy is facing and moving along the X axis.
//        /// </summary>
//        private Direction direction = Direction.btnLeft;

//        /// <summary>
//        /// How long this enemy has been waiting before turning around.
//        /// </summary>
//        //private float waitTime;

//        /// <summary>
//        /// How long to wait before turning around.
//        /// </summary>
//        private const float MaxWaitTime = 0.5f;
//        #endregion
        
//        /// <summary>
//        /// Constructs a new Ncp.
//        /// </summary>
//        public Npc(Level level, Vector2 position, NpcType ncpType)
//        {

//            this.level = level;
//            this.position = position;
//            this.ncpType = ncpType;
//            IsAlive = true;
//            Health = ncpType.health;
//            LoadContent(ncpType.name,ncpType.Single);
//        }
//        /// <summary>
//        /// Loads a particular ncp sprite sheet and/or sounds.
//        /// </summary>
//        public void LoadContent(string spriteSet,bool single)
//        {
//            // Load animations
            
//            spriteSet = "Sprites/" + spriteSet + "/";
//            if (!single)
//            {
//                if (ncpType == NpcType.Spider)
//                {
//                    runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, false, Tile.Width, Tile.Height);
//                    idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.045f, false, Tile.Width, Tile.Height);
//                }
//                else
//                {
//                    runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);
//                    idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.045f, false, Tile.Width * 2, Tile.Height * 3);
//                }
//            }
//            else
//            {
//                idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), ncpType.Framerate, true);
//            }
//            sprite.PlayAnimation(idleAnimation);

//            // Calculate bounds within texture size.
//            int width = (int)(idleAnimation.FrameWidth * 0.4);
//            int left = (idleAnimation.FrameWidth - width) / 2;
//            int height = (int)(idleAnimation.FrameHeight);
//            int top = idleAnimation.FrameHeight - height;
//            localBounds = new Rectangle(left, top, width, height);
           

//            Initialize();
//        }
//        /// <summary>
//        /// Set up NPCS
//        /// </summary>
//        private void Initialize()
//        {
//            //If its a GHOST, BIRD, OR BAT, set the initial position and speed
//            if (ncpType == NpcType.Ghost || ncpType == NpcType.Bird || ncpType == NpcType.Bat)
//            {
//                lerpingPosition = Position + new Vector2(level.random.Next(-100, 100), level.random.Next(-100, 100));
//                speed = level.random.NextDouble();
//            }
//            direction = Direction.btnLeft;
//        }

       
//        //public void Update(GameTime gameTime)
//        //{

//        //    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

//        //    //Current X and Y positions (In blocks) of an ncp
//        //    int tileX = (int)Math.Floor(position.X / Tile.Width);
//        //    int tileY = (int)Math.Floor(position.Y / Tile.Height);

//        //    //if about to run into a wall, see if we can jump/climb it.  Only want to jump small blocks, not climb walls / edge of screen.
//        //    if (this.ncpType == NpcType.Ghost)
//        //    {

//        //        //Checks if we are near a player
//        //        if (Level.Players[0].Position.X + 500 >= position.X && position.X >= (Level.Players[0].Position.X - 500)
//        //            && Level.Players[0].Position.Y + 500 >= position.X && position.Y >= (Level.Players[0].Position.Y - 500))
//        //        {
//        //            //Go near player
//        //            Position = Vector2.Lerp(Position, lerpingPosition, 0.01f);
//        //            //If we are near player,or done lerping make a new position and go to it
//        //            if (Math.Abs(Position.X - lerpingPosition.X) < 50 || Math.Abs(Position.X - lerpingPosition.X) < 10)
//        //                if (Math.Abs(Position.Y - lerpingPosition.Y) < 50 || Math.Abs(Position.Y - lerpingPosition.Y) < 10)
//        //                {
//        //                    lerpingPosition = new Vector2(level.Players[0].Position.X + level.random.Next(-100, 100), level.Players[0].Position.Y + level.random.Next(-100, 100));
//        //                }
//        //        }
//        //        //If were not near anyone, just move around so it dosent look wierd
//        //        else
//        //        {
//        //            Position = Vector2.Lerp(Position, lerpingPosition, 0.01f);
//        //            //If we are near player,or done lerping make a new position and go to it
//        //            if (Math.Abs(Position.X - lerpingPosition.X) < 50 || Math.Abs(Position.X - lerpingPosition.X) < 10)
//        //                if (Math.Abs(Position.Y - lerpingPosition.Y) < 50 || Math.Abs(Position.Y - lerpingPosition.Y) < 10)
//        //                {
//        //                    lerpingPosition = new Vector2(lerpingPosition.X + level.random.Next(-100, 100), lerpingPosition.Y + level.random.Next(-100, 100));
//        //                }
//        //        }

//        //    }
//        //    else if (this.ncpType == NpcType.Bird || this.ncpType == NpcType.Bat)
//        //    {
             
//        //        Vector2 lastPosition;
//        //        lastPosition = Position;

//        //        Vector2 dir = (lerpingPosition - position); // gives the direction needed to travel from position to target
//        //        dir.Normalize(); // we need it as a unit vector
//        //        if (ncpType == NpcType.Bat)
//        //            position += dir * (float)(1.3 + speed);
//        //        else
//        //            position += dir * (float)(1 + speed);

//        //        //If we are near player,or done lerping make a new position and go to it
//        //        if (Math.Abs(Position.X - lerpingPosition.X) < 50 || Math.Abs(Position.X - lerpingPosition.X) < 10)
//        //            if (Math.Abs(Position.Y - lerpingPosition.Y) < 50 || Math.Abs(Position.Y - lerpingPosition.Y) < 10)
//        //            {
//        //                if (ncpType == NpcType.Bat)
//        //                    lerpingPosition = new Vector2(lerpingPosition.X + level.random.Next(-5000, 500), lerpingPosition.Y + level.random.Next(-100, 100));
//        //                else
//        //                    lerpingPosition = new Vector2(lerpingPosition.X + level.random.Next(-1000, 1000), lerpingPosition.Y + level.random.Next(-40, 40));
//        //                speed = level.random.NextDouble();
//        //            }
//        //        if (lastPosition.X > position.X)
//        //            direction = FaceDirection.btnLeft;
//        //        else if (lastPosition.X < position.X)
//        //            direction = FaceDirection.btnRight;
               
//        //        position.X = (int)Math.Round(position.X);
//        //        position.Y = (int)Math.Round(position.Y);



//        //    }
//        //    else
//        //    {
//        //        if (((Level.GetCollision(tileX + (int)direction, tileY - 1) == ItemCollision.Impassable) &&
//        //        (Level.GetCollision(tileX + (int)direction, tileY - 2) == ItemCollision.Passable))
//        //        || ((Level.GetCollision(tileX + (int)direction, tileY - 1) == ItemCollision.Impassable) &&
//        //        (Level.GetCollision(tileX + (int)direction, tileY - 2) == ItemCollision.Passable) && (Level.GetCollision(tileX + (int)direction + (int)direction, tileY - 3) == ItemCollision.Impassable))
//        //        )
//        //        {
//        //            status = Status.JumpOver;
//        //            isJumping = true;
//        //            status = Status.Neutral;

//        //        }



//        ////Checks if the player position is within 100 blocks
//        //        else if (Level.Players[0].Position.X + 100 >= position.X && position.X >= (Level.Players[0].Position.X - 100)
//        //            && Level.Players[0].Position.Y + 100 >= position.X && position.Y >= (Level.Players[0].Position.Y - 100))
//        //        {
//        //            if (ncpType.relationPlayer == RelationPlayer.Follow)
//        //            {

//        //                if (previousStatus != Status.Turn)
//        //                {
//        //                    status = Status.PlayerContact;
//        //                    if (Level.Players[0].Position.X - position.X < -50) //Select direction
//        //                        direction = FaceDirection.btnLeft;
//        //                    else if (Level.Players[0].Position.X - position.X > 50)
//        //                        direction = FaceDirection.btnRight;
//        //                    status = Status.Neutral;
//        //                    previousStatus = Status.PlayerContact;
//        //                }
//        //            }
//        //            else if (ncpType.relationPlayer == RelationPlayer.Avoid)
//        //            {
//        //                if (previousStatus != Status.Turn)
//        //                {
//        //                    status = Status.PlayerContact;
//        //                    if (Level.Players[0].Position.X - position.X < -50) //Select direction
//        //                        direction = FaceDirection.btnRight;
//        //                    else if (Level.Players[0].Position.X - position.X > 50)
//        //                        direction = FaceDirection.btnLeft;
//        //                    status = Status.Neutral;
//        //                    previousStatus = Status.PlayerContact;

//        //                }
//        //            }

//        //        }

//        //        if (isLedge(tileX, tileY))
//        //        {
//        //            status = Status.Turn;
//        //            direction = (FaceDirection)(-(int)direction);
//        //            status = Status.Neutral;
//        //            previousStatus = Status.Turn;

//        //        }
//        //        if ((Level.GetCollision(tileX + (int)direction, tileY - 2) == ItemCollision.Impassable) &&
//        //        (Level.GetCollision(tileX + (int)direction, tileY - 1) == ItemCollision.Impassable) &&
//        //       (Level.GetCollision(tileX + (int)direction, tileY) == ItemCollision.Impassable) && isOnGround == true)
//        //        {
//        //            status = Status.Turn;
//        //            direction = (FaceDirection)(-(int)direction);
//        //            status = Status.Neutral;
//        //            previousStatus = Status.Turn;
//        //        }

//        //        movement = (int)direction;








//        //        ApplyPhysics(gameTime);

//        //    }
//        //    /*if (IsAlive && IsOnGround)
//        //    {
//        //        if (Math.Abs(Velocity.X) - 0.02f > 0)
//        //        {
//        //            sprite.PlayAnimation(runAnimation);
//        //        }
//        //        else
//        //        {
//        //            sprite.PlayAnimation(idleAnimation);
//        //        }
//        //    }*/


       
//        void Jump()
//        {
//            isJumping = true;
//        }
//        void btnLeft()
//        {
//            direction = Direction.btnLeft;
//        }
//        void Stop()
//        {
//            direction = Direction.None;
//        }
//        void btnRight()
//        {
//            direction = Direction.btnRight;
//        }
//        void TurnAround()
//        {
//            direction = (Direction)(-(int)direction);
//        }
//        bool IsLedge()
//        {
//            if ((Level.GetCollision(tileX + (int)direction, tileY) == BlockCollision.Passable) && (Level.GetCollision(tileX + (int)direction, tileY + 1) == BlockCollision.Impassable) && isOnGround == true)
//            {
//                return true;
//            }
//            return false;
//        }
//        bool IsBigLedge()
//        {
//            if ((Level.GetCollision(tileX + (int)direction, tileY) == BlockCollision.Passable) && (Level.GetCollision(tileX + (int)direction, tileY + 1) == BlockCollision.Passable) && (Level.GetCollision(tileX + (int)direction, tileY + 2) == BlockCollision.Passable) && isOnGround == true)
//            {
//                return true;
//            }
//            return false;
//        }
        
//        bool IsWall()
//        {
//            if ((Level.GetCollision(tileX + (int)direction, tileY - 1) == BlockCollision.Impassable) && (Level.GetCollision(tileX + (int)direction, tileY -2) == BlockCollision.Passable) && isOnGround == true)
//            {
//                return true;
//            }
//            return false;
//        }
//        bool IsBigWall()
//        {
//            if ((Level.GetCollision(tileX + (int)direction, tileY-1) == BlockCollision.Impassable ) && (Level.GetCollision(tileX + (int)direction, tileY - 2) == BlockCollision.Impassable) && (Level.GetCollision(tileX + (int)direction, tileY - 3) == BlockCollision.Impassable) && isOnGround == true)
//            {
//                return true;
//            }
//            return false;
//        }
//        int PlayerNear(int distance)
//        {
//            int pID = -1;
//            float d = 0.0f;
//            for(int i = 0; i < level.Players.Count;i++)
//            {
//                float Distance = Vector2.Distance(level.Players[i].Position, Position);
//                if (pID == -1 && Distance <= distance * Tile.Width)
//                {
//                    pID = i;
//                    d = Distance;
//                }
//                else if (Distance <= distance * Tile.Width)
//                {
//                    if (Distance < d)
//                    {
//                        pID = i;
//                        d = Distance;
//                    }

//                }
//            }
//            return pID;
//        }
//        /// <summary>
//        /// Updates the player's velocity and position based on input, gravity, etc.
//        /// </summary>
//        public void ApplyPhysics(GameTime gameTime)
//        {
            
//                inWater = false;
               
//                if (level.InLevelBounds(position)) //Check if player is in water
//                {
//                    if (level.GetLiquidOnPlayer(position) > 0)
//                        inWater = true;
//                }

                

//                if (inWater)
//                {
//                    #region Physics Constants
//                    MoveAcceleration = 1000.0f;
//                    MaxMoveSpeed = 125.0f;
//                    GroundDragFactor = .80f;
//                    AirDragFactor = .75f;

//                    // Constants for controlling vertical movement
//                    MaxJumpTime = .8f;
//                    JumpLaunchVelocity = -3000.0f;
//                    GravityAcceleration = 3000.0f;
//                    MaxFallSpeed = 200.0f;
//                    JumpControlPower = 0.3f;
//                    #endregion

//                }
//                else
//                {
//                    if (ncpType == NpcType.Spider)
//                    {
//                        MoveAcceleration = 1000.0f;
//                        MaxMoveSpeed = 100.0f;
//                        GroundDragFactor = .85f;
//                        AirDragFactor = .90f;

//                        // Constants for controlling vertical movement
//                        MaxJumpTime = .65f;
//                        JumpLaunchVelocity = -2000.0f;
//                        GravityAcceleration = 3000.0f;
//                        MaxFallSpeed = 400.0f;
//                        JumpControlPower = 0.1f;
//                    }
//                    else
//                    {
//                        #region Physics Constants
//                        // Constants for controling horizontal movement
//                        MoveAcceleration = 1000.0f;
//                        MaxMoveSpeed = 250.0f;
//                        GroundDragFactor = .85f;
//                        AirDragFactor = .90f;

//                        // Constants for controlling vertical movement
//                        MaxJumpTime = .65f;
//                        JumpLaunchVelocity = -3500.0f;
//                        GravityAcceleration = 3000.0f;
//                        MaxFallSpeed = 250.0f;
//                        JumpControlPower = 0.1f;
//                        #endregion
//                    }
//                }


//                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

//                previousPosition = Position;

//                Rectangle bounds = BoundingRectangle;
//                int leftTile = (int)Math.Floor((float)bounds.btnLeft / Tile.Width);
//                int rightTile = (int)Math.Ceiling(((float)bounds.btnRight / Tile.Width)) - 1;
//                int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
//                int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

//                movement.X = MathHelper.Clamp(movement.X, -1, 1);

//                velocity.X += movement.X * MoveAcceleration * elapsed;


//                velocity.Y = DoJump(velocity.Y, gameTime);

//                // Apply pseudo-drag horizontally.
//                if (IsOnGround)
//                    velocity.X *= GroundDragFactor;
//                else
//                    velocity.X *= AirDragFactor;

//                        velocity.Y += movement.Y * MoveAcceleration * elapsed;
//                        velocity.Y += GravityAcceleration * elapsed;
//                        velocity.Y = MathHelper.Clamp(
//                            velocity.Y,
//                            -MaxFallSpeed,
//                            MaxFallSpeed);
                
                
//                // Prevent the player from running faster than his top speed.            
//                velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);





//                if (velocity.X != 0f)
//                {
//                    Vector2 change = velocity.X * Vector2.UnitX * elapsed;
//                    change.X = MathHelper.Clamp(change.X, -(Tile.Height + 1), Tile.Height - 1);
//                    Position += change;
//                    HandleCollisions(CollisionDirection.Horizontal);


//                }
//                if (velocity.Y != 0f)
//                {
//                    Vector2 change = velocity.Y * Vector2.UnitY * elapsed;
//                    change.Y = MathHelper.Clamp(change.Y, -(Tile.Height) + 1, Tile.Height - 1);
//                    Position += change;
//                    HandleCollisions(CollisionDirection.Vertical);

//                }


//                if (Math.Abs(Velocity.X) > 0 && sprite.Animation != runAnimation)
//                {
//                    sprite.PlayAnimation(runAnimation);
//                }




//                // If the collision stopped us from moving, reset the velocity to zero.
//                if (Position.X == previousPosition.X)
//                {
//                    velocity.X = 0;
//                }

//                if (Position.Y == previousPosition.Y)
//                {

//                    velocity.Y = 0;
//                    jumpTime = 0.0f;
//                }
               

               
            
//        }

//        /// <summary>
//        /// Calculates the Y velocity accounting for jumping and
//        /// animates accordingly.
//        /// </summary>
//        /// <remarks>
//        /// During the accent of a jump, the Y velocity is completely
//        /// overridden by a power curve. During the decent, gravity takes
//        /// over. The jump velocity is controlled by the jumpTime field
//        /// which measures time into the accent of the current jump.
//        /// </remarks>
//        /// <param name="velocityY">
//        /// The player's current velocity along the Y axis.
//        /// </param>
//        /// <returns>
//        /// A new Y velocity if beginning or continuing a jump.
//        /// Otherwise, the existing Y velocity.
//        /// </returns>
//        private float DoJump(float velocityY, GameTime gameTime)
//        {
//            // If the player wants to jump
//            if (isJumping)
//            {
//                // Begin or continue a jump
//                if ((!wasJumping && IsOnGround) || jumpTime > 0.0f)
//                {


//                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

//                }

//                // If we are in the ascent of the jump
//                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
//                {
//                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
//                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
//                }
//                else
//                {
//                    // Reached the apex of the jump
//                    jumpTime = 0.0f;
//                }
//            }
//            else
//            {
//                // Continues not jumping or cancels a jump in progress
//                jumpTime = 0.0f;
//            }
//            wasJumping = isJumping;

//            return velocityY;
//        }

//        public static bool TileIntersectsPlayer(Rectangle player, Rectangle block, CollisionDirection direction, out Vector2 depth)
//        {
//            depth = direction == CollisionDirection.Vertical ? new Vector2(0, player.GetVerticalIntersectionDepth(block)) : new Vector2(player.GetHorizontalIntersectionDepth(block), 0);
//            return depth.Y != 0 || depth.X != 0;
//        }
//        /// <summary>
//        /// Detects and resolves all collisions between the player and his neighboring
//        /// tiles. When a collision is detected, the player is pushed away along one
//        /// axis to prevent overlapping. There is some special logic for the Y axis to
//        /// handle platforms which behave differently depending on direction of movement.
//        /// </summary>
//        private void HandleCollisions(CollisionDirection direction)
//        {
//            // Get the player's bounding rectangle and find neighboring tiles.
//            Rectangle bounds = BoundingRectangle;
//            int leftTile = (int)Math.Floor((float)bounds.btnLeft / Tile.Width);
//            int rightTile = (int)Math.Ceiling(((float)bounds.btnRight / Tile.Width)) - 1;
//            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
//            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

//            // Reset flag to search for ground collision.
//            isOnGround = false;

//            // For each potentially colliding tile,
//            for (int y = topTile; y <= bottomTile; ++y)
//            {
//                for (int x = leftTile; x <= rightTile; ++x)
//                {
//                    Rectangle tileBounds = Level.GetBounds(x, y);
//                    // If this tile is collidable,
//                    BlockCollision collision = Level.GetCollision(x, y);
//                    Vector2 depth;
//                    if (collision != BlockCollision.Passable && TileIntersectsPlayer(BoundingRectangle, tileBounds, direction, out depth))
//                    {
//                        if ((collision == BlockCollision.Platform && movement.Y > 0))
//                            continue;

//                        if (collision == BlockCollision.Impassable || isOnGround || (collision == BlockCollision.Platform && velocity.Y >= 0))
//                        {
//                            if (direction == CollisionDirection.Horizontal)
//                            {
//                                position.X += depth.X;
//                            }
//                            else
//                            {

//                                isOnGround = true;
//                                position.Y += depth.Y;
//                            }
//                        }

//                        //// Determine collision depth (with direction) and magnitude.
//                        //Rectangle tileBounds = Level.GetBounds(x, y);

//                        //if (depth != Vector2.Zero)
//                        //{
//                        //    float absDepthX = Math.Abs(depth.X);
//                        //    float absDepthY = Math.Abs(depth.Y);

//                        //  // Resolve the collision along the shallow axis. 
//                        //if (absDepthY < absDepthX || collision == ItemCollision.Platform || velocity.Y > MaxFallSpeed && velocity.X == 0) 
//                        //{ 

//                        //        // If we crossed the top of a tile, we are on the ground.
//                        //        if (previousBottom <= tileBounds.Top)
//                        //        {
//                        //            if (collision == ItemCollision.Ladder)
//                        //            {
//                        //                if (!isClimbing && !isJumping)
//                        //                {
//                        //                    // When walking over a ladder
//                        //                    isOnGround = true;
//                        //                }
//                        //            }
//                        //            else
//                        //            {
//                        //                isOnGround = true;
//                        //                isClimbing = false;
//                        //                isJumping = false;
//                        //            }
//                        //        }

//                        //        // Ignore platforms, unless we are on the ground.
//                        //        if (collision == ItemCollision.Impassable || IsOnGround)
//                        //        {
//                        //            // Resolve the collision along the Y axis.
//                        //            Position = new Vector2(Position.X, Position.Y + depth.Y);

//                        //            // Perform further collisions with the new bounds.
//                        //            bounds = BoundingRectangle;
//                        //        }
//                        //    }

//                        //else if (collision == ItemCollision.Impassable || (level.tiles[x,y].opened == false && level.tiles[x,y].item == Item.Door)) // Ignore platforms.
//                        //{
//                        //    // Resolve the collision along the X axis.
//                        //    Position = new Vector2(Position.X + depth.X, Position.Y);

//                        //    // Perform further collisions with the new bounds.
//                        //    bounds = BoundingRectangle;
//                        //}
//                        //else if (collision == ItemCollision.Ladder && !isClimbing)
//                        //{
//                        //    // When walking in front of a ladder, falling off a ladder
//                        //    // but not climbing
//                        //    // Resolve the collision along the Y axis.
//                        //    Position = new Vector2(Position.X, Position.Y);
//                        //    // Perform further collisions with the new bounds.
//                        //    bounds = BoundingRectangle;
//                        //}
//                    }
//                }
//            }

//            // Save the new bounds bottom.
//            previousBottom = bounds.Bottom;

//        }
//        public void OnKilled(PlayerCharacter killedBy)
//        {
//            IsAlive = false;
//            Health = 0;
//        }
//        public void OnHurt(PlayerCharacter hurtBy,int damage)
//        {
//            if (Health > 0)
//            {
//               // level.Points.Add(new FloatLabel(level.game.Manager,damage.ToString(), position + new Vector2(localBounds.Width / 2, -localBounds.Height), new Color(255, 0, 0), new Vector2(level.random.Next(-10, 10), -75 - level.random.Next(-10, 10)),40));
//                Health -= damage;
//            }
//            if (Health <= 0)
//                OnKilled(hurtBy);
//        }
//       /// <summary>
//        /// Draws the animated player.
//        /// </summary>
//        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
//        {
//            // Flip the sprite to face the way we are moving.
//            if (Velocity.X > 0)
//                flip = SpriteEffects.FlipHorizontally;
//            else if (Velocity.X < 0)
//                flip = SpriteEffects.None;

//            int tileX = (int)Math.Floor(Position.X / Tile.Width);
//            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

//            Color drawColor = level.tiles[tileX, tileY - 2].Light;

          
//            sprite.Draw(gameTime, spriteBatch, Position, flip, drawColor);
            
//        }

//    }



   
//    public enum RelationPlayer
//    {
//        Follow = 1,
//        Avoid = 2,
//        Pass = 3
//    }
//    enum Direction
//    {
//        btnLeft = -1,
//        None = 0,
//        btnRight = 1,
//    }
    

//}