
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Input.Touch;
//using System;
//using System.Collections.Generic;

//namespace ZarknorthClient
//{
//    /// <summary>
//    /// Players (Multiplayer and self) in the game world
//    /// </summary>
//    public class Player
//    {

//#region Properties
//        /// <summary>
//        /// The inventory of the player
//        /// Slots 1 - 50 are for the inventory
//        /// Slots 51-60 are for crafting
//        /// Slot 61 is for crafting output
//        /// </summary>

//        public Slot[] inventory;
//        public Vector2 previousPosition;
//        //Animations
//        private Animation idleAnimation;
//        private Animation shootAnimation;
//        private Animation runAnimation;
//        private Animation jumpAnimation;
//        private Animation celebrateAnimation;
//        private Animation dieAnimation;
//        private Animation ladderUpAnimation;
//        private Animation ladderDownAnimation;
//        private Animation attackAnimation;

//        // Collision
//        private Shape playerShape;
//        private Shape leftTriangle;
//        private Shape rightTriangle;
        
//        public float IsSwinging;
//        public bool creativeMode;
//        public bool isAttacking;

//        public float AttackTime;
//        public SpriteEffects flip = SpriteEffects.None;
//        private AnimationPlayer sprite;
//        public int[,] playerMinimapBlur;
//        // Sounds
//        private SoundEffect hurtSound;
//        private SoundEffect jumpSound;
//        private SoundEffect fallSound;
//        /// <summary>
//        /// Player Name Ex: Cyral
//        /// </summary>
//        public string Name;
//        /// <summary>
//        /// Unique Player ID Ex: e7ref7y4rt4t
//        /// </summary>
//        public long ID;
//        /// <summary>
//        /// Order of player in player list, Ex: 2
//        /// </summary>
//        public int SimpleID;

//        public Level Level
//        {
//            get { return level; }
//        }
//        Level level;

//        public bool IsAlive
//        {
//            get { return isAlive; }
//        }
//        bool isAlive;

//        // Physics state
//        public Vector2 Position
//        {
//            get { return position; }
//            set { position = value; }
//        }
//        public Vector2 position;

//        public float previousBottom;

//        public Vector2 Velocity
//        {
//            get { return velocity; }
//            set { velocity = value; }
//        }
//        Vector2 velocity;

//        // variables for controling horizontal movement
//        private  float MoveAcceleration;
//        private  float MaxMoveSpeed ;
//        private  float GroundDragFactor;
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

//        public bool previousIsOnGround;

//        int fallStart;
//        /// <summary>
//        /// Gets whether or not the player's feet are in watewr
//        /// </summary>
//        public bool InWater
//        {
//            get { return inWater; }
//        }
//        bool inWater;

//        /// <summary>
//        /// Money of player
//        /// </summary>
//        public float Money;
//        /// <summary>
//        /// Energy of player
//        /// </summary>
//        public float Energy = 100;
     
//        /// <summary>
//        /// Health of player
//        /// </summary>
//        public float Health;
        
//        float previousHealth;
//        public int fallHeight;
//        /// <summary>
//        /// Gets wethe the player is you (false), or someone online (true)
//        /// </summary>
//        public bool IsMultiplayer
//        {
//            get { return isMultiplayer; }
//        }
//        bool isMultiplayer;

//        //A gameobject for the tool/weapon you are using
//        public GameObject CurrentObject;
//        public Slot LastObject = new Slot(Item.Blank);
//        public GameObject[] bullets;
      
//        //private TimeSpan lastShot;

//        private const int LadderAlignment = 16;
//        private bool isClimbing;
//        public bool IsClimbing
//        {
//            get { return isClimbing; }
//        }
//        private bool wasClimbing;

//        public Vector2 movement;

//        // Jumping state
//        public bool isJumping;
//        private bool wasJumping;
//        private float jumpTime;
       
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

//#endregion

//        /// <summary>
//        /// Creates a new player
//        /// </summary>
//        /// <param name="level">Level reference</param>
//        /// <param name="position">Position to spawn</param>
//        /// <param name="Multiplayer">If the player is anouther player connected to the server</param>
//        public Player(Level level, Vector2 position,bool Multiplayer)
//        {
//            //Set the level and multiplayer status
//            this.level = level;
//            this.isMultiplayer = Multiplayer;
//            //Setup the basic inventory
//            SetupInventory();
//            LoadContent();
//            sprite.PlayAnimation(idleAnimation);
       
//            //Reset Position and stats
//            Reset(position);
//        }

//        private void SetupInventory()
//        {
//            //Setup Inventory
//            inventory = new Slot[60];
//            for (int i = 0; i < 60; i++)
//            {
//                inventory[i] = new Slot(Item.Blank) { Stack = 250 };
//            }

//            inventory[0].Item = Item.Torch;
//            inventory[1].Item = Item.SteelPlating;
//            inventory[2].Item = Item.Asphalt;
//            inventory[3].Item = Item.Ruby;
//            inventory[4].Item = Item.Diamond;
//            inventory[5].Item = Item.DiamondOre;
//            inventory[6].Item = Item.RubyOre;
//            inventory[7].Item = Item.SilverOre;
//            inventory[8].Item = Item.CopperOre;
//            inventory[9].Item = Item.GoldOre;
    
//            Money = int.Parse(Game.random.Next(1, 10).ToString() + "0000");

//            for (int i = 0; i < 60; i++)
//            {
//                if (inventory[i].Item == Item.Blank)
//                    inventory[i].Stack = 0;
//            }
//        }

//        /// <summary>
//        /// Loads the player sprite sheet and sounds.
//        /// </summary>
//        public void LoadContent()
//        {
//            // Load animated textures.

//            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f,false,Tile.Width * 2,Tile.Height * 3);
//            shootAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Shoot"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);
//            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.045f, true, Tile.Width * 2, Tile.Height * 3);
//            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.2f, false, Tile.Width * 2, Tile.Height * 3);
//            celebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);
//            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);
//            attackAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Attack"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);

//            ladderUpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);
//            ladderDownAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false, Tile.Width * 2, Tile.Height * 3);
           
//            // Calculate bounds within texture size.     (This is what determins the colision, add or subtract to change the bounds for arms and such)       
//            int width = (int)(idleAnimation.FrameWidth * 0.4);
//            int left = (idleAnimation.FrameWidth - width) / 2;
//            int height = (int)(idleAnimation.FrameHeight);
//            int top = idleAnimation.FrameHeight - height;
//            localBounds = new Rectangle(left, top, width, height);
//            if (!level.Server)
//            {
//                // Load sounds.            
//                hurtSound = Level.Content.Load<SoundEffect>("Sounds/PlayerHurt");
//                jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
//                fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
//            }
//            // Ramp Collision
//            playerShape = new Shape(new Vector2[]
//            {
//                new Vector2(-width / 2, -height),
//                new Vector2(width / 2, -height),
//                new Vector2(width / 2, 0),
//                new Vector2(-width / 2, 0),
//            });

//            playerShape.CanBePushed = true;

//            leftTriangle = Shape.CreateLeftTriangle(Tile.Width + 2, Tile.Height + 2);
//            leftTriangle.CanBePushed = false;

//            rightTriangle = Shape.CreateRightTriangle(Tile.Width + 2, Tile.Height + 2);
//            rightTriangle.CanBePushed = false;

       
//        }
//        public bool IsOverlappingPlayer(int gridX, int gridY)
//        {
//            return IsOverlappingPlayer(gridX, gridY, new Point(1,1));
//        }
//        public bool IsOverlappingPlayer(int gridX, int gridY, Point size)
//        {
//            Rectangle pointRect = new Rectangle(gridX * Tile.Width, gridY * Tile.Height, size.X * Tile.Width, size.Y * Tile.Height);
//            return BoundingRectangle.Intersects(pointRect);
//        }
//        /// <summary>
//        /// Resets the player to life.
//        /// </summary>
//        /// <param name="position">The position to come to life at.</param>
//        public void Reset(Vector2 position)
//        {
//            //Set position
//            Position = previousPosition = position;
//            Velocity = Vector2.Zero;
//            //Alive!
//            isAlive = true;
//            //Set health and energy
//            Health = previousHealth = 100;
//            Energy = 100;
//            //Not on ground + fall reset
//            isOnGround = previousIsOnGround = true;
//            fallStart = (int)position.Y / Tile.Height;
//            //Play the idle animation
//            if (!level.Server)
//            sprite.PlayAnimation(idleAnimation);
//        }

//        /// <summary>
//        /// Handles input, performs physics, and animates the player sprite.
//        /// </summary>
//        /// <remarks>
//        /// We pass in all of the input states so that our game is only polling the hardware
//        /// once per frame. We also pass the game's orientation because when using the accelerometer,
//        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
//        /// </remarks>
//        public void Update(GameTime gameTime,  KeyboardState keyboardState,KeyboardState previousKeyboardstate)
//        {
//            //If the user switches items
//            if (!level.Server && inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item != LastObject.Item)
//            {
//                CurrentObject = new GameObject(level.tileContent[inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item.Name]);
//            }

//            if (!isMultiplayer) //If it is the main player, get the input
//            GetInput(keyboardState,gameTime);
//            DoAttack(gameTime);
//            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
//            if (isAttacking)
//                AttackTime += elapsed;
            

//            //The all inportant physics!
//            if (inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item is RangedWepItem || inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item is MeleeWepItem || inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item is ToolItem)
//            {
//                CurrentObject.center = new Vector2(CurrentObject.sprite.Width / 1.5f, CurrentObject.sprite.Height / 2);
//                if (flip == SpriteEffects.None)
//                    CurrentObject.position = new Vector2(position.X - 10, position.Y - 32);
//                else
//                    CurrentObject.position = new Vector2(position.X + 10, position.Y - 32);
//               if (inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item is MeleeWepItem)
//                    CurrentObject.center = new Vector2(0, CurrentObject.sprite.Height);
//            }
//            ApplyPhysics(gameTime);
//            if (!level.Server)
//                UpdateMelee(gameTime);
//            //Update bullets
//          //  if (!level.Server)
//              //  UpdateBullets();
            
//            if (IsAlive)
//            {
              
                
//                //This if statement deals with running/idling
//                if (isOnGround &&  (!level.Server))
//                {
//                    if (isAttacking)
//                    {
//                        sprite.PlayAnimation(attackAnimation);
//                    }
//                    else
//                    {
//                        //If Velocity.X is > 0 in any direction, play runAnimation
//                        if (Math.Abs(Velocity.X) - 0.02f > 0)
//                            sprite.PlayAnimation(runAnimation);

//                        //Otherwise, sit still (idleAnimation)
//                        else 
//                            sprite.PlayAnimation(idleAnimation);
//                    }
//                }
//                //This if statement deals with ladder climbing
//                else if (isClimbing &&  (!level.Server))
//                {
//                    //If he's moving down play ladderDownAnimation
//                    if (Velocity.Y - 0.02f > 0)
//                        sprite.PlayAnimation(ladderDownAnimation);
//                    //If he's moving up play ladderUpAnimation
//                    else if (Velocity.Y - 0.02f < 0)
//                        sprite.PlayAnimation(ladderUpAnimation);
//                    //Otherwise, just stand on the ladder (idleAnimation)
//                    else if (sprite.Animation != shootAnimation)
//                        sprite.PlayAnimation(idleAnimation);
//                }
//            }

//            //Reset our variables every frame
//            movement = Vector2.Zero;
//            wasClimbing = isClimbing;
//            isClimbing = false;
//            // Clear input.
//            isJumping = false;
//            Health = MathHelper.Clamp(Health, 0, 100);
//            MathHelper.Clamp(Energy, 0, 100);
//            LastObject = inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected];
//        }
//        private void SwingSword()
//        {
//            isAttacking = true;
//            CurrentObject.rotation = 0;
//        }
//        private void FireBullet()
//        {
//            foreach (GameObject bullet in bullets)
//            {
//                //Find a bullet that isn't alive
//                if (!bullet.alive)
//                {
//                    level.soundContent["PistolShoot"].Play();
//                    //And set it to alive.
//                    bullet.alive = true;
//                    bullet.rotation = CurrentObject.rotation;
                 
                     
//                        float armCos = (float)Math.Cos(CurrentObject.rotation -1.5 + MathHelper.PiOver2);
//                        float armSin = (float)Math.Sin(CurrentObject.rotation  - 1.5+ MathHelper.PiOver2);

//                        //Set the initial position of our bullet at the end of our gun arm
//                        //42 is obtained be taking the width of the Arm_Gun texture / 2
//                        //and subtracting the width of the Bullet texture / 2. ((96/2)-(12/2))
//                        bullet.position = bullet.start = new Vector2(
//                            CurrentObject.position.X - 20 * armCos,
//                            CurrentObject.position.Y - 20 * armSin);

//                        //And give it a velocity of the direction we're aiming.
//                        bullet.velocity = new Vector2(
//                         -armCos,
//                         -armSin) * 10.0f;
                    
                   

//                    return;
//                }
//            }
//        }
//        private void UpdateMelee(GameTime gameTime)
//        {
//            //if (inventory[Interface.MainWindow.inventory.Selected].Item is MeleeWepItem && isAttacking)
//            //{
//            //    if (flip != SpriteEffects.None)
//            //        CurrentObject.rotation = MathHelper.ToRadians((180 * (float)(TimeSpan.FromSeconds(AttackTime).TotalMilliseconds / inventory[Interface.MainWindow.inventory.Selected].Item.useTime.TotalMilliseconds))-45);
//            //    else
//            //        CurrentObject.rotation = MathHelper.ToRadians((180 * (float)(TimeSpan.FromSeconds(AttackTime).TotalMilliseconds / inventory[Interface.MainWindow.inventory.Selected].Item.useTime.TotalMilliseconds))  +180 -45);
//            //    foreach (Npc npc in level.NCPs)
//            //    {
//            //        if (CurrentObject.rectangle.Intersects(npc.BoundingRectangle) && !npc.HitThisSwing)
//            //        {
//            //            //Item i = inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item;
//            //            //npc.OnHurt(this, (int)(level.random.Next(i.Damage - i.DamageOffset, i.Damage + i.DamageOffset)));
//            //            //npc.HitThisSwing = true;
                       
//            //        }
//            //    }
//            //}
//            //if ((TimeSpan.FromSeconds(AttackTime).TotalMilliseconds >= inventory[Interface.MainWindow.inventory.Selected].Item.useTime.TotalMilliseconds))
//            //{
//            //    isAttacking = false;
//            //    foreach (Npc npc in level.NCPs)
//            //    {
//            //        npc.HitThisSwing = false;
//            //    }
//            //}
//        }
//        private void UpdateBullets()
//        {
//            //Check all of our bullets
//            foreach (GameObject bullet in bullets)
//            {
//                //Only update them if they're alive
//                if (bullet.alive)
//                {
//                    //Move our bullet based on it's velocity
//                    bullet.position += bullet.velocity;

//                    //Collision rectangle for each bullet -Will also be
//                    //used for collisions with enemies.
//                    Rectangle bulletRect = new Rectangle(
//                        (int)bullet.position.X,
//                        (int)bullet.position.Y,
//                        bullet.sprite.Width,
//                        bullet.sprite.Height);
//                    //Check for collisions with the enemies
//                    foreach (Npc npc in level.NCPs)
//                    {
//                        if (bulletRect.Intersects(npc.BoundingRectangle))
//                        {
//                            //Item i = inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item;
//                            //float Distance = Vector2.Distance(bullet.start, bullet.position);
//                            //float loss = Distance * i.DistanceLoss;
//                            //npc.OnHurt(this, (int)(level.random.Next(i.Damage - i.DamageOffset, i.Damage + i.DamageOffset) - loss));
//                            //bullet.alive = false;
//                        }
//                    }
                  
//                    //Everything below here can be deleted if you want
//                    //your bullets to shoot through all tiles.

//                    //Look for adjacent tiles to the bullet
//                    Rectangle bounds = new Rectangle(
//                        bulletRect.Center.X ,
//                        bulletRect.Center.Y ,
//                        bulletRect.Width ,
//                        bulletRect.Height );
//                    int leftTile = (int)Math.Floor((float)bounds.btnLeft / Tile.Width);
//                    int rightTile = (int)Math.Ceiling(((float)bounds.btnRight / Tile.Width)) - 1;
//                    int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
//                    int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

//                    // For each potentially colliding tile
//                    for (int y = topTile; y <= bottomTile; ++y)
//                    {
//                        for (int x = leftTile; x <= rightTile; ++x)
//                        {
//                            BlockCollision collision = Level.GetCollision(x, y);

//                            //If we collide with an Impassable or Platform tile
//                            //then delete our bullet.
//                            if (collision == BlockCollision.Impassable ||
//                                collision == BlockCollision.Platform)
//                            {
//                                if (bulletRect.Intersects(bounds))
//                                    bullet.alive = false;
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        private void GetInput(KeyboardState keyboardState,GameTime gameTime)
//        {
//            //Get mouse pos and angle for weps/tools
//            MouseState mousePos = Mouse.GetState();
//            var direction = new Vector2(mousePos.X, mousePos.Y) - CurrentObject.position + level.MainCamera.Position;
//            direction.Normalize();
//            if (inventory[Interface.MainWindow.inventory.Selected].Item == Item.PaintBrush)
//            CurrentObject.rotation = (float)Math.Atan2(-direction.Y, -direction.X) + MathHelper.ToRadians(45);
//            else
//            CurrentObject.rotation = (float)Math.Atan2(-direction.Y, -direction.X) + MathHelper.ToRadians(45);

//            if (!Game.IsMouseOnControl)
//            {
//                //Shoot = RightTrigger
//                //if (level.currentMouseState.LeftButton == ButtonState.Pressed && inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item.Type == ItemType.WeaponRanged && lastShot + inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item.useTime < gameTime.TotalGameTime)
//                //{
//                //    if (!level.Server)
//                //    sprite.PlayAnimation(shootAnimation);
//                //    lastShot = gameTime.TotalGameTime;
//                //    FireBullet();
//                //}
//                //if (level.currentMouseState.LeftButton == ButtonState.Pressed && inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item.Type == ItemType.WeaponMelee && lastShot + inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item.useTime < gameTime.TotalGameTime)
//                //{
//                //    if (!level.Server)
//                //        sprite.PlayAnimation(attackAnimation);
//                //    lastShot = gameTime.TotalGameTime;
//                //    AttackTime = 0;
//                //    SwingSword();
//                //}
//                if (Math.Abs(movement.X) < 0.5f)
//                    movement.X = 0.0f;
//                if (Math.Abs(movement.Y) < 0.5f)
//                    movement.Y = 0.0f;
//                // If any digital horizontal movement input is found, override the analog movement.
//                if (
//                    keyboardState.IsKeyDown(Keys.btnLeft) ||
//                    keyboardState.IsKeyDown(Keys.A))
//                {
//                    //movement = -1.0f;
//                    movement.X = -1.0f;
//                }
//                else if (
//                        keyboardState.IsKeyDown(Keys.btnRight) ||
//                        keyboardState.IsKeyDown(Keys.D))
//                {
                    
//                    movement.X = 1.0f;
//                }
//                if (
//                           keyboardState.IsKeyDown(Keys.btnDown) ||
//                           keyboardState.IsKeyDown(Keys.S))
//                {

//                    movement.Y = 1.0f;
//                }
//                if (creativeMode)
//                {
//                    if (
//                   keyboardState.IsKeyDown(Keys.btnUp) ||
//                   keyboardState.IsKeyDown(Keys.W))
//                    {
                      
//                        movement.Y = -1.0f;
//                    }
//                    else if (
//                            keyboardState.IsKeyDown(Keys.btnDown) ||
//                            keyboardState.IsKeyDown(Keys.S))
//                    {
                   
//                        movement.Y = 1.0f;
//                    }
//                }


//                //Ladder stuff
//                if (keyboardState.IsKeyDown(Keys.btnUp) || keyboardState.IsKeyDown(Keys.W))
//                {
//                    isClimbing = false;
//                    if (IsAligned(BlockCollision.Ladder))
//                    {
//                        //We need to check the tile behind the player,
//                        //not what he is standing on
//                        if (level.GetTileBelowPlayer(position).Foreground.Collision == BlockCollision.Ladder)
//                        {
//                            isClimbing = true;
//                            isJumping = false;
//                            isOnGround = false;
//                            movement.Y = -2.0f;
//                        }
//                    }
//                }
//                else if (
//                        keyboardState.IsKeyDown(Keys.btnDown) ||
//                        keyboardState.IsKeyDown(Keys.S))
//                {
//                    isClimbing = false;
//                    if (IsAligned(BlockCollision.Ladder))
//                    {
//                        // Check the tile the player is standing on
//                        if (level.GetTileBelowPlayer(Position).Foreground.Collision == BlockCollision.Ladder)
//                        {
//                            isClimbing = true;
//                            isJumping = false;
//                            isOnGround = false;
//                            movement.Y = 2.0f;
//                        }
//                    }
//                }
//                // Check if the player wants to jump.
//                isJumping =
//                    keyboardState.IsKeyDown(Keys.Space) ||
//                    keyboardState.IsKeyDown(Keys.btnUp) ||
//                    keyboardState.IsKeyDown(Keys.W);
//            }
           
              
//        }

//        //Check if player is alligned (for ladders)
//        private bool IsAligned(BlockCollision col)
//        {
//            int playerOffset = ((int)position.X % Tile.Width) - Tile.Center;
//            if (Math.Abs(playerOffset) <= LadderAlignment &&
//                level.GetTileOnPlayer(new Vector2(
//                    position.X,
//                   position.Y + 1)).Foreground.Collision == col ||
//                level.GetTileOnPlayer(new Vector2(
//                    position.X,
//                    position.Y - 1)).Foreground.Collision == col)
//            {
//                // Align the player with the middle of the tile
//                position.X -= playerOffset;
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }


//        /// <summary>
//        /// Updates the player's velocity and position based on input, gravity, etc.
//        /// </summary>
//        public void ApplyPhysics(GameTime gameTime)
//        {
//            if (creativeMode)
//            {

//                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

//                velocity.X = movement.X * (MoveAcceleration * 10) * elapsed;
//                velocity.Y = movement.Y * (MoveAcceleration * 10) * elapsed;


//                if (velocity.X != 0f)
//                {
//                    Vector2 change = velocity.X * Vector2.UnitX * elapsed;
//                    change.X = MathHelper.Clamp(change.X, -(Tile.Height), Tile.Height);
//                    Position += change;
//                    HandleCollisions(CollisionDirection.Horizontal);
//                }
//                if (velocity.Y != 0f)
//                {
//                    Vector2 change = velocity.Y * Vector2.UnitY * elapsed;
//                    change.Y = MathHelper.Clamp(change.Y, -(Tile.Height), Tile.Height);
//                    Position += change;
//                    HandleCollisions(CollisionDirection.Vertical);
//                }
//                position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));


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
               
//            }
//            else
//            {
//                inWater = false;
               
//                if (level.InLevelBounds(position)) //Check if player is in water
//                {
//                    if (level.GetLiquidOnPlayer(position) > 0)
//                        inWater = true;
//                }

//                if (level.InLevelBounds(position)) //Check if player is in portal
//                {
//                    if (level.GetTileOnPlayer(position).Foreground == Item.Portal)
//                    {
//                        PortalTile t = (PortalTile)level.GetTileOnPlayer(position);
//                        Position = new Vector2(t.Target.X * Tile.Width, t.Target.Y * Tile.Height);
                
//                    }
//                }

//                if (inWater)
//                {
//                    #region Physics Constants
//                    MoveAcceleration = 2500.0f;
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
//                    #region Physics Constants
//                    // Constants for controling horizontal movement
//                    MoveAcceleration = 2700.0f;
//                    MaxMoveSpeed = 250.0f;
//                    GroundDragFactor = .80f;
//                    AirDragFactor = 1f;

//                    // Constants for controlling vertical movement
//                    MaxJumpTime = .65f;
//                    JumpLaunchVelocity = -3500.0f;
//                    GravityAcceleration = 3000.0f;
//                    MaxFallSpeed = 400.0f;
//                    JumpControlPower = 0.05f;
//                    #endregion
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

//                if (!isClimbing)
//                {
//                    if (wasClimbing)
//                        velocity.Y = 0;
//                    else
//                    {
//                        velocity.Y += movement.Y * MoveAcceleration * elapsed;
//                        velocity.Y += GravityAcceleration * elapsed;
//                        velocity.Y = MathHelper.Clamp(
//                            velocity.Y,
//                            -MaxFallSpeed,
//                            MaxFallSpeed);
//                    }
//                }
//                else
//                {
//                    velocity.Y = movement.Y * MoveAcceleration * elapsed;
//                }
//                // Prevent the player from running faster than his top speed.            
//                velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);


//                {
//                    Vector2 change = velocity.X * Vector2.UnitX * elapsed;
//                    change.X = MathHelper.Clamp(change.X, -(Tile.Height + 1), Tile.Height - 1);
//                    Position += change;
//                    HandleCollisions(CollisionDirection.Horizontal);
//                }
              
//                {
//                    Vector2 change = velocity.Y * Vector2.UnitY * elapsed;
//                    change.Y = MathHelper.Clamp(change.Y, -(Tile.Height) +1, Tile.Height - 1);
//                    Position += change;
//                    HandleCollisions(CollisionDirection.Vertical);
//                }
//                position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y)); //Round position
//                if (level.random.Next(0, 40) == 0 && isOnGround && position.X != previousPosition.X)
//                    level.soundContent["Footstep"].Play();
                
//                // If the collision stopped us from moving, reset the velocity to zero.
//                if (Position.X == previousPosition.X)
//                    velocity.X = 0;
//                if (Position.Y == previousPosition.Y)
//                {
//                    velocity.Y = 0;
//                    jumpTime = 0.0f;
//                }
//                if (Position != previousPosition)
//                {
                 
//                }
//                if (previousIsOnGround && !isOnGround)
//                    fallStart = (int)position.Y / Tile.Height;
//                if (!previousIsOnGround && isOnGround)
//                {
//                    int fallen = (int)(position.Y / Tile.Height) - fallStart;
//                    //If player falls more than 15 blocks, he will recive twice the fall damage.
//                    int damage = fallen > 15 ? damage = fallen * 2 : damage = 0;
//                    Health = Health - damage;
//                }

//                if (previousHealth > Health) { hurtSound.Play(); }
//                previousIsOnGround = IsOnGround;
//                previousHealth = Health;

//            }
//        }
//        /// <summary>
//        /// Detects and resolves the collision between two shapes
//        /// </summary>
//        private void HandleRampCollision(Shape ramp)
//        {
//            playerShape.Position = position;

//            // Our equivilant of the broad phase...
//            if (playerShape.Rectangle.Intersects(ramp.Rectangle) == false)
//            {
//                return;
//            }

//            Vector2[] axis = new Vector2[playerShape.Axes.Length + ramp.Axes.Length];

//            playerShape.Axes.CopyTo(axis, 0);
//            ramp.Axes.CopyTo(axis, playerShape.Axes.Length);

//            float intervalWidth = 0;
//            float minimumDistance = float.MaxValue;

//            Vector2 resolutionAxis = Vector2.Zero;

//            for (int i = 0; i < axis.Length; i++)
//            {
//                // Test to see if this axis seperates shapes A and B.
//                if (AxisSeperatesShapes(axis[i], ramp, playerShape, out intervalWidth))
//                {
//                    // We found a seperating axis! No need to carry on.
//                    return;
//                }
//                else
//                {
//                    intervalWidth *= 1.01f;

//                    // Find the shortest distance needed to move to resolve collision.
//                    if (intervalWidth * intervalWidth < minimumDistance * minimumDistance)
//                    {
//                        minimumDistance = intervalWidth;
//                        resolutionAxis = axis[i] * minimumDistance;
//                    }
//                }
//            }

//            // Make sure we are trying to push shape B 
//            // away and not pull it into shape A.
//            if (resolutionAxis != Vector2.Zero)
//            {
//                Vector2 direction = new Vector2(ramp.Center.X,
//                    ramp.Center.Y) - playerShape.Center;

//                if (Vector2.Dot(direction, resolutionAxis) < 0.0f)
//                {
//                    resolutionAxis = -resolutionAxis;
//                }

//                if (velocity.Y > 0 && resolutionAxis.Y > 0)
//                {
//                    isOnGround = true;
//                    velocity.Y = 0;
//                }

//                if (playerShape.CanBePushed && ramp.CanBePushed == false)
//                {
//                    playerShape.Position -= resolutionAxis;
//                }
//                else if (playerShape.CanBePushed == false && ramp.CanBePushed == true)
//                {
//                    ramp.Position += resolutionAxis;
//                }
//                else
//                {
//                    resolutionAxis /= 2.0f;

//                    playerShape.Position -= resolutionAxis;
//                    ramp.Position += resolutionAxis;
//                }

//                Position = playerShape.Position;
//            }
//        }

//        /// <summary>
//        /// Tests if an axis seperates two shapes.
//        /// </summary>
//        private bool AxisSeperatesShapes(Vector2 axis, Shape A, Shape B, out float intervalWidth)
//        {
//            float minA = 0, maxA = 0;
//            float minB = 0, maxB = 0;

//            A.ProjectOntoAxis(axis, out minA, out maxA);
//            B.ProjectOntoAxis(axis, out minB, out maxB);

//            if (minB > maxA || minA > maxB)
//            {
//                intervalWidth = 0;
//                return true;
//            }

//            float d0 = maxA - minB;
//            float d1 = maxB - minA;

//            intervalWidth = MathHelper.Min(d0, d1);
//            return false;
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
//                    //if (jumpTime == 0.0f)
//                        //jumpSound.Play();

//                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
//                    if (!level.Server)
//                    sprite.PlayAnimation(jumpAnimation);
//                }

//                // If we are in the ascent of the jump
//                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
//                {
//                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
//                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
//                }
//                else
//                    jumpTime = 0.0f;   // Reached the apex of the jump
//            }
//            else
//            {
//                // Continues not jumping or cancels a jump in progress
//                jumpTime = 0.0f;
//            }
//            wasJumping = isJumping;

//            return velocityY;
//        }
//        /// <summary>
//        /// Makes a player attack, simlar to Do Jump
//        /// </summary>
//        /// <param name="gameTime"></param>
//        private void DoAttack(GameTime gameTime)
//        {
//            //// If the player wants to attack
//            //if (isAttacking)
//            //{
//            //    // Begin or continue an attack
//            //    if (AttackTime > 0.0f)
//            //    {
//            //        AttackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
//            //    }
//            //    else
//            //    {
//            //        isAttacking = false;
//            //    }
//            //}
//            //else
//            //{
//            //    //Continues not attack or cancels an attack in progress
//            //    AttackTime = 0.0f;
//            //}
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
//                   Rectangle tileBounds = Level.GetBounds(x, y);
//                    // If this tile is collidable,
//                    BlockCollision collision = Level.GetCollision(x, y);
//                    Vector2 depth;
//                    if (collision != BlockCollision.Passable && TileIntersectsPlayer(BoundingRectangle, tileBounds, direction, out depth))
//                    {
//                        if (collision != BlockCollision.Slope)
//                        {
//                            if ((collision == BlockCollision.Platform && movement.Y > 0))
//                                continue;
                     
//                            // If we crossed the top of a tile, we are on the ground.
//                            if (previousBottom <= tileBounds.Top)
//                            {
//                                if (collision == BlockCollision.Ladder)
//                                {
//                                    if (!isClimbing && !isJumping)
//                                    {
//                                        // When walking over a ladder
//                                        isOnGround = true;
//                                    }
//                                }
//                                else if (collision == BlockCollision.Platform)
//                                {
//                                    isOnGround = true;
//                                    isClimbing = false;
//                                    isJumping = false;
//                                }
//                            }
//                            if (collision == BlockCollision.Impassable || isOnGround)
//                            {
//                                if (direction == CollisionDirection.Horizontal)
//                                {
//                                    position.X += depth.X;
//                                }
//                                if (direction == CollisionDirection.Vertical)
//                                {
                                  
//                                    isOnGround = true;
//                                    position.Y += depth.Y;
//                                }
//                            }
//                        }
//                        // Handle Ramp Collision.
//                        else
//                        {
//                            if (collision == BlockCollision.Slope && !level.tiles[x, y].Flip)
//                            {
//                                leftTriangle.Position = new Vector2(x * Tile.Width - 1,
//                                                                    y * Tile.Height - 1);

//                                HandleRampCollision(leftTriangle);
//                            }
//                            else if (collision == BlockCollision.Slope && level.tiles[x,y].Flip)
//                            {
//                                rightTriangle.Position = new Vector2(x * Tile.Width + 1,
//                                                                     y * Tile.Height - 1);

//                                HandleRampCollision(rightTriangle);
//                            }
                           
//                            bounds = BoundingRectangle;
//                        }
//                    }
//                }
//            }
//            // Save the new bounds bottom.
//            previousBottom = bounds.Bottom;
   
//        }
//        public static bool TileIntersectsPlayer(Rectangle player, Rectangle block, CollisionDirection direction, out Vector2 depth)
//        {
//            depth = direction == CollisionDirection.Vertical ? new Vector2(0, player.GetVerticalIntersectionDepth(block)) : new Vector2(player.GetHorizontalIntersectionDepth(block), 0);
//            return depth.Y != 0 || depth.X != 0;
//        }

//        /// <summary>
//        /// Called when the player has been killed.
//        /// </summary>
//        /// <param name="killedBy">
//        /// The enemy who killed the player. This parameter is null if the player was
//        /// not killed by an enemy (fell into a hole).
//        /// </param>
//        public void OnKilled(Npc killedBy)
//        {
//            //isAlive = false;

//            //if (killedBy != null)
//                //killedSound.Play();
//            //else
//                //fallSound.Play();
//            if (!level.Server)
//            sprite.PlayAnimation(dieAnimation);
//        }
//        public int FindSlot(Item item)
//        {
//            for (int x= 0; x <= inventory.Length -1; x++)
//            {
//                if (inventory[x].Item == item && inventory[x].Stack <= inventory[x].Item.MaxStack)
//                    return x;

//            }
//            for (int x = 0; x <= inventory.Length - 1; x++)
//            {
//                if (inventory[x].Item == Item.Blank)
//                {
//                    inventory[x] = new Slot(item, 0);
//                    return x;
//                }
//            }
//            return -1;
//        }
//        public void AddToInventory(Item item)
//        {
//            int i = FindSlot(item);
//            if (i != -1)
//            {
//                inventory[i].Add();
//            }
            
//        }
//        /// <summary>
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
//            int tileY = (int)Math.Floor( Position.Y / Tile.Height);

//              sprite.Draw(gameTime, spriteBatch, Position, flip, Color.White);
              
//        }
//    }
//    public enum CollisionDirection
//    {
//        Horizontal,
//        Vertical,
//    }

    
//}
        