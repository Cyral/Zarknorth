using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TomShane.Neoforce.Controls;
using ZarknorthClient.Interface;
using Cyral.Extensions;

namespace ZarknorthClient.Entities
{
    public abstract class Character : BoxCollisionEntity
    {
        #region Properties
        public string Name { get; set; }
        /// <summary>
        /// Is the character jumping
        /// </summary>
        public bool IsJumping
        {
            get { return isJumping; }
            private set { isJumping = value; }
        }
        protected bool isJumping;
        protected bool wasJumping;
        protected float jumpTime;

        /// <summary>
        /// Gets if the player is alive
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        /// <summary>
        /// Is the character climbing
        /// </summary>
        public bool IsClimbing
        {
            get { return isClimbing; }
        }
        protected bool isClimbing;
        protected bool wasClimbing;

        public SpriteEffects Flip
        {
            get { return flip; }
        }
        protected SpriteEffects flip;

        public MovementDirection Direction
        {
            get
            {
                if (Velocity.X > 0)
                    return MovementDirection.Right;
                else if (Velocity.X < 0)
                    return MovementDirection.Left;
                else
                    return MovementDirection.Still;
            }
        }

        /// <summary>
        /// Gets whether or not the player's feet are in water
        /// </summary>
        public bool InWater
        {
            get { return inWater; }
        }
        protected bool inWater;
        protected bool lastInWater;

        /// <summary>
        /// Gets whether or not the player's head is in water
        /// </summary>
        public bool HeadInWater
        {
            get { return headInWater; }
        }
        protected bool headInWater;
        protected bool lastHeadInWater;

        /// <summary>
        /// Gets whether or not the player's feet are in water
        /// </summary>
        public bool InLava
        {
            get { return inLava; }
        }
        protected bool inLava;
        protected bool lastInLava;

        /// <summary>
        /// Characters health
        /// </summary>
        public float Health
        {
            get { return health; }
            private set { health = MathHelper.Clamp(value, 0, 100); }
        }
        protected float health;
        protected float previousHealth;

        /// <summary>
        /// Characters Energy
        /// </summary>
        public float Energy
        {
            get { return energy; }
        }
        protected float energy;

        //How far off the character can be from a ladder to get on it
        protected const int LadderAlignment = 12;

        protected const int AutoJumpTime = 100;
        public const float HealthRegenPerSecond = 2.5f;
        public bool IsStill { get { return LastPosition == Position; } }

        //Varaiables for controlling vertical movement
        protected float MaxJumpTime;
        protected float JumpLaunchVelocity;
        protected float JumpControlPower;

        protected BreathStatusBox BreathStatus;
        protected FireStatusBox FireStatus;
        public DrownStatusBox DrownStatus;
        public LavaStatusBox LavaStatus;
        
        protected int fallStart;
        /// <summary>
        /// Keeps track of the current input and which direction was pressed
        /// </summary>
        protected Vector2 movement;

        public bool OnFire;
        private bool lastOnFire;

        public double achievementPoints;

        /// <summary>
        /// How often the player can get hurt from spikes
        /// </summary>
        public const float SpikeTime = 1.5f;
        /// <summary>
        /// Last spike damage
        /// </summary>
        public float lastSpike;
        #endregion

        #region Constructor
        public Character(Level level, Vector2 spawnPosition) : base(level, spawnPosition)
        {
            this.level = level;
            LoadContent();
            Reset(spawnPosition);
        }
        #endregion

        #region Methods
        public virtual void Heal()
        {
            health = previousHealth = energy = 100;
        }
        public virtual void Respawn()
        {
            Position = lastPosition = level.SpawnPoint;
        }
        public virtual void Reset(Vector2 position)
        {
            //Set position
            Position = lastPosition = position;
            lastOnFire = false;
            OnFire = false;
            Velocity = Vector2.Zero;
            //Alive!
            isAlive = true;
            //Set health and energy
            Heal();
            //Not on ground + fall reset
            isOnGround = lastIsOnGround = true;
            fallStart = (int)position.Y / Tile.Height;

            //Set bg position to character
            if (this is PlayerCharacter)
            level.backgroundPosition.X = position.X;
        }
        protected virtual void LoadContent()
        {

        }
        public virtual void Update(GameTime gameTime, KeyboardState currentKeyState, KeyboardState lastKeyState, bool resetMovement = true)
        {
            position.Y = MathHelper.Clamp(position.Y, Tile.Width + Tile.Width + localBounds.Height, (level.Height * Tile.Width) - Tile.Width - Tile.Width);
            UpdateDirection();
            ApplyPhysics(gameTime);
            position.Y = MathHelper.Clamp(position.Y, Tile.Width + Tile.Width + localBounds.Height, (level.Height * Tile.Width) - Tile.Width - Tile.Width);
            RegenHealth(gameTime);
            //Reset our variables every frame
            if (resetMovement)
                movement = Vector2.Zero;
            wasClimbing = isClimbing;
            isClimbing = false;
            // Clear input.
            isJumping = false;
            health = MathHelper.Clamp(Health, 0, 100);
            energy = MathHelper.Clamp(Energy, 0, 100);
            if (this is PlayerCharacter)
            {
                if (lastPosition.X != Position.X && Level.Gamemode != GameMode.Sandbox)
                    level.backgroundPosition.X -= lastPosition.X - Position.X;
                position = level.tiles.SetPositionRepeat(position, true);
            }
            else
                position = level.tiles.SetPositionRepeat(position, false);

            if (Level.tiles[(int)(position.X / Tile.Width), (int)(position.Y / Tile.Width) - 1].Foreground.ID == Item.PressurePlate.ID)
            {
                ElectronicTile t = (Level.tiles[(int)(position.X / Tile.Width), (int)(position.Y / Tile.Width) - 1] as ElectronicTile);
                if (t.State == 0)
                {
                    t.State = 1;
                    UpdatePressurePlateState(t);
                }
            }
        }

        private void UpdatePressurePlateState(ElectronicTile t)
        {
            level.Actions.Enqueue(new QueueItem(
            () =>
            {
                if (t.Foreground.ID == Item.PressurePlate.ID && Level.tiles[(int)(position.X / Tile.Width), (int)(position.Y / Tile.Width) - 1] != t)
                {
                    if ((t is ElectronicTile) && t.State == 1)
                    {
                        t.State = 0;
                        foreach (Wire w in t.Outputs[0])
                            w.Powered = false;
                    }
                }
                else
                    UpdatePressurePlateState(t);
            }, 500, false)
            );
        }

        private void RegenHealth(GameTime gameTime)
        {
            if (IsStill)
                Health += (float)gameTime.ElapsedGameTime.TotalSeconds * HealthRegenPerSecond;
            else
                Health += (float)gameTime.ElapsedGameTime.TotalSeconds * (HealthRegenPerSecond / 2f);
        }

        double lastAutoJump;
        bool AutoJump;
        public override void ApplyPhysics(GameTime gameTime, bool DefaultVelocity = true, bool CancelVelocity = true, bool Round = true)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPosition = Position;
            lastVelocity = Velocity;
            //Reset
            lastInWater = inWater;
            lastHeadInWater = headInWater;
            inWater = headInWater = false;
            lastInLava = inLava;
            inLava = false;


            UpdateLiquid();
            UpdateFire();

            //Set water/land physics
            if (inWater || inLava)
            {
                #region Physics Constants

                // Constants for controling horizontal movement
                MoveAcceleration = 10000.0f;
                MaxMoveSpeed = 625.0f * (Level.Gravity);
                GroundDragFactor = 0.38f;
                AirDragFactor = 0.35f;

                // Constants for controlling vertical movement
                MaxJumpTime = 10000f;
                JumpLaunchVelocity = -3200.0f;
                GravityAcceleration = 3400.0f;
                MaxFallSpeed = 145.0f * Level.Gravity;
                JumpControlPower = 0.1f;

                if (inLava)
                {
                    MaxFallSpeed = 115.0f * Level.Gravity;
                    MaxMoveSpeed = 550.0f * (Level.Gravity);
                }
                #endregion
            }
            else
            {
                #region Physics Constants

                // Constants for controling horizontal movement
                MoveAcceleration = 13000.0f;
                MaxMoveSpeed = 1550.0f * (Level.Gravity);
                GroundDragFactor = 0.54f;
                AirDragFactor = 0.50f;

                // Constants for controlling vertical movement
                MaxJumpTime = 0.55f;
                JumpLaunchVelocity = -3500.0f;
                GravityAcceleration = 3400.0f;
                MaxFallSpeed = 450.0f * Level.Gravity;
                JumpControlPower = 0.14f; 
                #endregion
            }
            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement.X * MoveAcceleration * .0166f;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * .0166f, -MaxFallSpeed, MaxFallSpeed);
            UpdateAutoJump(gameTime);
            velocity.Y = DoJump(velocity.Y, gameTime);
  

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;


            if (!isClimbing)
            {
                if (!wasClimbing)
                    velocity.Y = MathHelper.Clamp(
                        velocity.Y + GravityAcceleration * .0166f,
                        -MaxFallSpeed,
                        MaxFallSpeed);
            }
            else
            {
                velocity.Y = MathHelper.Clamp(
    movement.Y * MoveAcceleration * .0166f,
    -MaxFallSpeed,
    MaxFallSpeed);
            }

            base.ApplyPhysics(gameTime, false);
            if (Position.Y == lastPosition.Y)
            {
                jumpTime = 0.0f;
            }
            if (Position != lastPosition)
            {
                //TODO: If position has changed
                //Play footstep
                // if (level.random.Next(0, 40) == 0 && isOnGround)
                     //level.soundContent["Footstep"].Play();
            }
            //Falling physics
            UpdateFallPhysics();

            //If character takes damage, play sound
            if (previousHealth > Health) { level.soundContent["PlayerHurt"].Play(); }
            //Reset vars
            previousHealth = Health;
            base.ResetPhysics();
            insideSand = false;
        }

        protected void UpdateAutoJump(GameTime gameTime)
        {
            if (lastAutoJump == 0)
            {
                lastAutoJump = gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            if (gameTime.TotalGameTime.TotalMilliseconds <= lastAutoJump + AutoJumpTime && !IsOnGround)
            {
                isJumping = true;
            }
            if (movement.X != 0 && (int)(Position.Y / Tile.Height) > 3 && gameTime.TotalGameTime.TotalMilliseconds > lastAutoJump + AutoJumpTime && IsOnGround)
            {
                int dir = Flip == SpriteEffects.FlipHorizontally ? 1 : -1;
                AutoJump = ((level.tiles[(int)(Position.X / Tile.Width) + dir, (int)(Position.Y / Tile.Height) - 1].Foreground.Collision == BlockCollision.Impassable || level.tiles[(int)(Position.X / Tile.Width) + dir, (int)(Position.Y / Tile.Height) - 1].Foreground.Collision == BlockCollision.Falling) && level.tiles[(int)(Position.X / Tile.Width) + dir, (int)(Position.Y / Tile.Height) - 2].Foreground.Collision == BlockCollision.Passable && level.tiles[(int)(Position.X / Tile.Width) + dir, (int)(Position.Y / Tile.Height) - 3].Foreground.Collision == BlockCollision.Passable);
                if (AutoJump)
                {
                    isJumping = true;
                    lastAutoJump = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }

        /// <summary>
        /// Same as Damage() but with a random deviation factor, ex dmg 40 with deviation 10 could be anywhere from 30 to 50
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="deviation"></param>
        /// <param name="damageType"></param>
        private void DamageDeviation(int damage, int deviation, DamageType damageType)
        {
            Damage(level.random.Deviation(damage - deviation, damage + deviation, damage), damageType);
        }

        private void UpdateFire()
        {
            lastOnFire = OnFire;
            //OnFire = false;
            if (level.InLevelBounds(position)) //Check if player is in fire
            {
                if (level.GetTileOnPlayer(position).ForegroundFireMeta > 0 || level.GetTileOnPlayer(position).BackgroundFireMeta > 0)
                {
                    if (FireStatus != null)
                    {
                        FireStatus.Progress.Value = 100;
                        FireStatus.AbsValue = 100;
                    } 
                    OnFire = true;
                }
            }
            if (!lastOnFire && OnFire)
            {
                if (FireStatus != null)
                {
                    FireStatus.CloseFinal();
                }

                    FireStatus = new FireStatusBox(level.game.Manager);
                    FireStatus.Init();
                    level.game.Manager.Add(FireStatus);
                
            }
        }

        private void UpdateLiquid()
        {
            if (level.InLevelBounds(position)) //Check if player is in water
            {
                //If there is more than 1/4th water
                if (level.GetLiquidOnPlayer(position) > 64)
                    inWater = true;
                if (level.GetLiquidOnPlayerHead(this) > 0)
                    headInWater = true;
                if (inWater && !lastInWater)
                    level.DefaultParticleEngine.SpawnParticles((int)position.X, (int)position.Y - (int)((level.GetLiquidOnPlayer(position) / 255f) * Tile.Height) + Tile.Height, ParticleType.WaterSplash, 15);
                //Extinguish Fire
                if (inWater && OnFire)
                {
                    OnFire = false;
                    if (FireStatus != null)
                        FireStatus.Close();
                }
            }
            if (!lastHeadInWater && HeadInWater)
            {
                if (BreathStatus != null)
                    BreathStatus.CloseFinal();
                BreathStatus = new BreathStatusBox(level.game.Manager);
                BreathStatus.Init();
                BreathStatus.Progress.Color = Color.SkyBlue;
                level.game.Manager.Add(BreathStatus);
            }
            if (lastHeadInWater && !HeadInWater)
            {
                if (BreathStatus != null)
                {
                    BreathStatus.Close();
                }
                if (DrownStatus != null)
                {
                    DrownStatus.Close();
                }
            }

            //Lava
            if (level.InLevelBounds(position)) //Check if player is in water
            {
                //If there is more than 1/4th water
                if (level.GetLavaOnPlayer(position) > 64)
                    inLava = true;
                if (inLava && !lastInLava)
                {
                    OnFire = true;
                    level.DefaultParticleEngine.SpawnParticles((int)position.X, (int)position.Y - (int)((level.GetLavaOnPlayer(position) / 255f) * Tile.Height) + Tile.Height, ParticleType.LavaSplash, 25);
                    if (LavaStatus != null)
                        LavaStatus.CloseFinal();
                    LavaStatus = new LavaStatusBox(level.game.Manager);
                    LavaStatus.Init();
                    LavaStatus.Progress.Color = Color.OrangeRed;
                    level.game.Manager.Add(LavaStatus);
                }
                if (!inLava && lastInLava)
                {
                    LavaStatus.Close();
                    OnFire = false;
                }
            }
        }

        protected void UpdateFallPhysics()
        {
            //If we start to fall
            if (lastIsOnGround && !isOnGround && (!isClimbing || wasClimbing))
                fallStart = (int)position.Y / Tile.Height;
            //If we have fallen and water is less than 1/4th block
            if (!lastIsOnGround && isOnGround && level.GetLiquidOnPlayer(Position) <= 64)
            {
                int fallen = (int)(position.Y / Tile.Height) - fallStart;
                //If player falls more than 13 blocks, he will recive 4x the fall damage in height.
                int damage = 4 * (fallen - 15);
                Damage(damage, DamageType.Fall);
            }
        }
        /// <summary>
        /// Damages the character and adds a damage indicator and death if needed
        /// </summary>
        /// <param name="HealthLoss"></param>
        public virtual void Damage(float HealthLoss, DamageType damageType = DamageType.None)
        {
            if (IsAlive && HealthLoss > 0)
            {
                // TODO: Add defence multiplier once armor is implemented
                HealthLoss = MathHelper.Clamp(HealthLoss, 0, health);
                health -= HealthLoss;

                switch (damageType)
                {
                    default:
                        break;
                    case DamageType.Fall:
                        level.DefaultParticleEngine.SpawnBlood((int)Position.X, (int)Position.Y,25);
                        break;
                }

                level.AddFloatingLabel("-" + Math.Round(HealthLoss), Position - new Vector2(0, BoundingRectangle.Height), Color.Orange);
                
                if (health <= 0)
                    Kill(damageType);
            }
        }
        /// <summary>
        /// Used for other methods to immediatly kill a player, MAY NOT BE THE SAME AS ONKILLED!
        /// </summary>
        public virtual void Kill(DamageType damageType = DamageType.None)
        {
            if (IsAlive)
            {
                isAlive = false;
                health = 0;
                OnKilled(damageType);
            }
        }

        public virtual void OnKilled(DamageType damageType = DamageType.None)
        {
            string reason = Game.UserName + " Died!";
            switch (damageType)
            {
                default:
                    break;
                case DamageType.Sand:
                    {
                        int r = level.random.Next(0, 2);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] was crushed.";
                        else if (r == 1)
                            reason = "[color:Red]" + Game.UserName + "[/color] suffocated.";
                    }
                    break;
                case DamageType.Spike:
                    {
                        int r = level.random.Next(0, 3);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] was stabbed.";
                        else if (r == 1)
                            reason = "[color:Red]" + Game.UserName + "[/color] got sliced.";
                        else if (r == 2)
                            reason = "[color:Red]" + Game.UserName + "[/color] was impaled by a spike.";
                    }
                    break;
                case DamageType.Fall:
                    {
                        int r = level.random.Next(0,3);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] went splat!";
                        else if (r == 1)
                            reason = "[color:Red]" + Game.UserName + "[/color] took a nosedive.";
                        else if (r == 2)
                            reason = "[color:Red]" + Game.UserName + "[/color] jumped a bit far...";
                    }
                    break;
                case DamageType.Fire:
                    {
                        int r = level.random.Next(0, 3);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] was burnt to a crisp.";
                        else if (r == 1)
                            reason = "[color:Red]" + Game.UserName + "[/color] was playing with fire.";
                        else if (r == 2)
                            reason = "[color:Red]" + Game.UserName + "[/color] is a pyromaniac.";
                    }
                    break;
                case DamageType.Drown:
                    {
                        int r = level.random.Next(0, 3);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] drowned.";
                        else if (r == 1)
                            reason = "[color:Red]" + Game.UserName + "[/color] couldn't find air.";
                        else if (r == 2)
                            reason = "[color:Red]" + Game.UserName + "[/color] thought they were a fish.";
                    }
                    break;
                case DamageType.Self:
                    {
                        int r = level.random.Next(0, 4);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] took their life.";
                        else if (r == 1 || r == 2)
                            reason = "[color:Red]" + Game.UserName + "[/color] suicided.";
                        else if (r == 3)
                            reason = "[color:Red]" + Game.UserName + "[/color] has hit restart.";
                    }
                    break;
                case DamageType.Lava:
                    {
                        int r = level.random.Next(0, 4);
                        if (r == 0)
                            reason = "[color:Red]" + Game.UserName + "[/color] swam in lava.";
                        else if (r == 1 || r == 2)
                            reason = "[color:Red]" + Game.UserName + "[/color] was incinerated.";
                        else if (r == 3)
                            reason = "[color:Red]" + Game.UserName + "[/color] couldn't stand the heat.";
                    }
                    break;
            }
            Interface.MainWindow.Console.MessageBuffer.Add(new TomShane.Neoforce.Controls.ConsoleMessage(reason,0));
         
            TaskDeath tmp = new TaskDeath(level.game.Manager, reason);
            tmp.Closing += new WindowClosingEventHandler(((MainWindow)Game.MainWindow).WindowClosing);
            tmp.Closed += new WindowClosedEventHandler(((MainWindow)Game.MainWindow).WindowClosed);
            tmp.Init();
            level.game.Manager.Add(tmp);
            tmp.Show();

        }
        public virtual void OnKilled(Character killedBy,DamageType damageType = DamageType.None)
        {
          
        }
        /// <summary>
        /// Updates the direction the character is facing based on the velocity of it
        /// </summary>
        protected virtual void UpdateDirection()
        {
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawCharacter(gameTime, spriteBatch);
            base.DebugDraw(gameTime, spriteBatch);
        }

        public virtual void DrawCharacter(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }
        /// <summary>
        /// Checks if the character is aligned to a block with a certain collision
        /// </summary>
        public virtual bool IsAligned(BlockCollision col)
        {
            int playerOffset = ((int)position.X % Tile.Width) - Tile.Center;
            if (Math.Abs(playerOffset) <= LadderAlignment && level.GetTileOnPlayer(position).Foreground.Collision == col)
            {
                // Align the player with the middle of the tile
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        protected float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (isJumping)
            {
                // Begin or continue a jump
                if ((!wasJumping && ((IsOnGround || level.GetTileOnPlayer(position).Foreground.Collision == BlockCollision.Stair) || InWater)) || jumpTime > 0.0f)
                {
                    //if (jumpTime == 0.0f)
                    //jumpSound.Play();
                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // If we are in the ascent of the jump
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                    jumpTime = 0.0f;   // Reached the apex of the jump
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }
        protected override bool HandleCollisions(GameTime gameTime, CollisionDirection direction, BlockCollision collision, Rectangle tileBounds, Vector2 depth, bool intersects, int x, int y)
        {
            //If ran into a spike
            if (collision == BlockCollision.Spike && IsAlive)
            {
                //Hurt the player every X seconds
                if (gameTime.TotalGameTime.TotalSeconds > SpikeTime + lastSpike)
                {
                    lastSpike = (float)gameTime.TotalGameTime.TotalSeconds;
                    //25 +- 10 damage
                    DamageDeviation(20, 10, DamageType.Spike);
                    level.DefaultParticleEngine.SpawnBlood(tileBounds.X + (Tile.Width / 2), tileBounds.Y + (Tile.Height / 2), 20);
                }
            }
            if (collision != BlockCollision.Passable && intersects)
            {
                if ((collision == BlockCollision.Platform || collision == BlockCollision.Stair) && movement.Y > 0)
                    return true;
                if (collision == BlockCollision.Door)
                {
                    if (direction == CollisionDirection.Horizontal)
                    {
                        if (level.tiles[x, y].Flip)
                        {
                            if (x == level.tiles[x, y].X + 1 && level.tiles[x, y].ForegroundVariation == 0)
                                position.X += depth.X;
                        }
                        else
                        {
                            if (x == level.tiles[x, y].X && level.tiles[x, y].ForegroundVariation == 0)
                            {
                                if (direction == CollisionDirection.Horizontal)
                                {
                                    //Fix for buggy door collision
                                    position.X += depth.X;
                                    return true;
                                }
                            }
                        }
                    }
                    else
                        return true;
                }
                else if (collision == BlockCollision.Stair && direction == CollisionDirection.Vertical)
                {
                    bool ret = false;
                    if (level.tiles[x, y].Flip)
                    {
                        if (direction == CollisionDirection.Vertical)
                        {
                            if (previousBounds.Right <= tileBounds.Left + (tileBounds.Width / 2))
                            {
                                if (previousBounds.Bottom <= tileBounds.Top + (Tile.Height / 2))
                                {
                                    if (depth.Y <= -12)
                                    {
                                        isOnGround = true;
                                        position.Y += depth.Y + 12;
                                        isOnGround = true;
                                    }
                                    ret = true;
                                }
                                else
                                {
                                    ret = true;
                                }
                            }
                            else
                            {
                                if (previousBounds.Bottom <= tileBounds.Top)
                                {
                                    isOnGround = true;
                                    position.Y += depth.Y;
                                    ret = true;
                                }

                            }
                        }

                    }
                    else
                    {
                        if (direction == CollisionDirection.Vertical)
                        {
                            if (previousBounds.Left >= tileBounds.Left + (tileBounds.Width / 2))
                            {
                                if (previousBounds.Bottom <= tileBounds.Top + (Tile.Height / 2))
                                {
                                    if (depth.Y <= -12)
                                    {
                                        isOnGround = true;
                                        position.Y += depth.Y + 12;
                                        isOnGround = true;
                                    }
                                    ret = true;
                                }
                                else
                                {
                                    ret = true;
                                }
                            }
                            else
                            {
                                if (previousBounds.Bottom <= tileBounds.Top)
                                {
                                    isOnGround = true;
                                    position.Y += depth.Y;
                                    ret = true;
                                }

                            }
                        }

                    }


                    if (ret)
                        return false;
                }
                if (previousBounds.Bottom <= tileBounds.Top && direction == CollisionDirection.Vertical)
                {
                    if (collision == BlockCollision.Ladder && movement.Y >= 0)
                    {
                        if (!isClimbing && !isJumping)
                        {
                            // When walking over a ladder
                            isOnGround = true;
                        }
                    }
                    else if (collision == BlockCollision.Platform || collision == BlockCollision.Falling)
                    {
                        isOnGround = true;
                        isClimbing = false;
                        isJumping = false;
                    }
                }
                if (collision == BlockCollision.Falling)
                {
                    if (IsAlive && level.tiles[x, y].NoCollide || level.tiles[x, y] is FallingTile)
                    {
                        //Hurt the player every X seconds
                        if (gameTime.TotalGameTime.TotalSeconds > SpikeTime + lastSpike && IsAlive)
                        {
                            lastSpike = (float)gameTime.TotalGameTime.TotalSeconds;
                            //25 +- 10 damage
                            DamageDeviation(15, 7, DamageType.Sand);
                        }
                        level.tiles[x, y].NoCollide = true;
                        return true;
                    }
                    else if (!level.tiles[x, y].NoCollide)
                        isOnGround = true;
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
            //base.HandleCollisions(gameTime, collision, tileBounds, depth, intersects);
            return true;
        }
        public bool IsOverlappingPlayer(int gridX, int gridY)
        {
            return IsOverlappingPlayer(gridX, gridY, new Point(1, 1));
        }
        public bool IsOverlappingPlayer(int gridX, int gridY, Point size)
        {
            Rectangle pointRect = new Rectangle(gridX * Tile.Width, gridY * Tile.Height, size.X * Tile.Width, size.Y * Tile.Height);
            return BoundingRectangle.Intersects(pointRect);
        }
        private void Dispose()
        {
            FireStatus.Dispose();
            BreathStatus.Dispose();
        }
        #endregion

        public bool insideSand { get; set; }
    }
    public enum CollisionDirection
    {
        Horizontal,
        Vertical,
    }
    public enum MovementDirection
    {
        Left, Right, Still
    }
}
