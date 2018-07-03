using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient.Entities
{
    public class Npc : Character
    {

        public bool canPickUp; //Can the NPC pick up items?
        public bool talking; //Can the NPC talk to players? (By text, box, etc)
        public string Name; //Name of the NPC

        #region Properties
        /// <summary>
        /// The collision-box of the character
        /// </summary>
        public override Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - (localBounds.Width / 2)) + localBounds.X;
                int top = (int)Math.Round(Position.Y - localBounds.Height) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }
        /// <summary>
        /// Gets the center position of the player
        /// </summary>
        public Vector2 OriginPosition
        {
            get
            {
                return Position - new Vector2(0, (int)(localBounds.Height / 2));
            }
        }
        public float Money
        {
            get { return money; }
        }
        protected float money;
        /// <summary>
        /// The slot array for the players inventory
        /// </summary>
        /// 

        //NPC Properties

        //
        int direction = -1;
        public Slot[] Inventory;
        public const int InventorySlots = 50;
        public
        bool AutoWalk; // Set if the NPC walks around the world normal
        //public bool IsAdminMode { get; set; }
        public Slot CurrentSlot { get { return Inventory[Interface.MainWindow.inventory.Selected]; } }
        public Texture2D Leg, Arm, Head, Body;
        #endregion

        #region Contructor
        public Npc(Level level, Vector2 spawnPosition)
            : base(level, spawnPosition)
        {
            if (canPickUp)
                SetupInventory();
            //money = int.Parse(Game.random.Next(1, 10).ToString() + "0000");
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            Arm = ContentPack.Textures["entities\\player\\armSheet"];
            Body = ContentPack.Textures["entities\\player\\bodySheet"];

            Head = ContentPack.Textures["entities\\player\\headSheet"];
            Leg = ContentPack.Textures["entities\\player\\legSheet"];
            // Calculate bounds within texture size.     (This is what determins the colision, add or subtract to change the bounds for arms and such)       
            int width = 14;
            int left = 5;
            int height = Tile.Height * 3;
            int top = 0;
            localBounds = new Rectangle(left, top, width, height);

            base.LoadContent();
        }
        private void SetupInventory()
        {
            //Setup Inventory
            Inventory = new Slot[InventorySlots];
            for (int i = 0; i < InventorySlots; i++)
            {
                Inventory[i] = new Slot(Item.Blank);
            }

            Inventory[0].Item = Item.StonePickaxe;
            Inventory[1].Item = Item.OPPickaxe;
            Inventory[2].Item = Item.WoodenChair;
            Inventory[3].Item = Item.Wood;
            Inventory[4].Item = Item.Stone;
            Inventory[5].Item = Item.Dirt;
            Inventory[6].Item = Item.CraftingTable;
            Inventory[7].Item = Item.Furnace;
            Inventory[8].Item = Item.Coal;
            Inventory[9].Item = Item.Sand;

            for (int i = 0; i < InventorySlots; i++)
            {
                if (Inventory[i].Item != Item.Blank)
                    Inventory[i].Stack = Inventory[i].Item.MaxStack;
            }
        }

        public override void ApplyPhysics(GameTime gameTime, bool DefaultVelocity = true, bool CancelVelocity = true, bool Round = true)
        {
            // lastAutoJump = AutoJump;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            lastPosition = Position;
            lastVelocity = Velocity;
            //Reset
            lastInWater = inWater;
            lastHeadInWater = headInWater;
            inWater = headInWater = false;
            //UpdateWater();
            //UpdateFire();

            //Set water/land physics
            if (inWater)
            {
                #region Physics Constants

                // Constants for controling horizontal movement
                MoveAcceleration = 10000.0f;
                MaxMoveSpeed = 500.0f * (Level.Gravity);
                GroundDragFactor = 0.38f;
                AirDragFactor = 0.35f;

                // Constants for controlling vertical movement
                MaxJumpTime = 10000f;
                JumpLaunchVelocity = -2800.0f;
                GravityAcceleration = 3400.0f;
                MaxFallSpeed = 200.0f * Level.Gravity;
                JumpControlPower = 0.1f;
                #endregion
            }
            else
            {
                #region Physics Constants

                // Constants for controling horizontal movement
                MoveAcceleration = 11000.0f;
                MaxMoveSpeed = 1000.0f * (Level.Gravity);
                GroundDragFactor = 0.1f;
                AirDragFactor = 0.1f;

                // Constants for controlling vertical movement
                MaxJumpTime = 0.35f / Level.Gravity;
                JumpLaunchVelocity = -3500.0f;
                GravityAcceleration = 3400.0f;
                MaxFallSpeed = 610.0f * Level.Gravity;
                JumpControlPower = 0.1f;
                #endregion
            }


            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            velocity.X += movement.X * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
            base.UpdateAutoJump(gameTime);
            velocity.Y = base.DoJump(velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;


            if (!isClimbing)
            {
                if (wasClimbing)
                    velocity.Y = 0;
                else
                    velocity.Y = MathHelper.Clamp(
                        velocity.Y + GravityAcceleration * elapsed,
                        -MaxFallSpeed,
                        MaxFallSpeed);
            }
            else
            {
                velocity.Y = movement.Y * MoveAcceleration * elapsed;
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
            base.UpdateFallPhysics();

            //If character takes damage, play sound
            if (previousHealth > Health) { level.soundContent["PlayerHurt"].Play(); }
            //Reset vars
            previousHealth = Health;
            base.ResetPhysics();
            insideSand = false;
        }


        public float animationFadeIn = 0;
        public override void Update(GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState currentKeyState, Microsoft.Xna.Framework.Input.KeyboardState lastKeyState, bool resetMovement = false)
        {
            HandleInput(currentKeyState, lastKeyState);

            // movement.X = -1;
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (velocity.X == 0)
            {
                float speed = 7;
                LeftLegRotation = MathHelper.Lerp(LeftLegRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                RightLegRotation = MathHelper.Lerp(RightLegRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                LeftArmRotation = MathHelper.Lerp(LeftArmRotation, -.1f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                RightArmRotation = MathHelper.Lerp(RightArmRotation, .1f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                animationFadeIn = 0;
            }
            else
            {
                LeftLegRotation = MathHelper.Lerp((float)Math.Cos((position.X / 20) * 0.6662F) * 2.0F * .7f * 0.5F, LeftLegRotation, 1f - animationFadeIn);
                RightLegRotation = MathHelper.Lerp((float)Math.Cos((position.X / 20) * 0.6662F + Math.PI) * 2.0F * .7f * 0.5F, RightLegRotation, 1f - animationFadeIn);

                LeftArmRotation = MathHelper.Lerp((float)Math.Cos((position.X / 20) * 0.6662F) * 2.0F * .4f * 0.5F, LeftArmRotation, 1f - animationFadeIn);
                RightArmRotation = MathHelper.Lerp((float)Math.Cos((position.X / 20) * 0.6662F + Math.PI) * 2.0F * .4f * 0.5F, RightArmRotation, 1f - animationFadeIn);
                if (animationFadeIn < 1f)
                    if (animationFadeIn < 1f)
                        animationFadeIn += elapsed * 4;
            }
            Vector2 direction = (level.MainCamera.Position + new Vector2(level.Players[0].Position.X, level.Players[0].Position.Y));
            direction.Normalize();
            HeadRotation = (float)Math.Atan2((double)direction.Y, (double)direction.X) + MathHelper.PiOver2 + MathHelper.ToRadians(90);

            if (HeadRotation < MathHelper.ToRadians(90))
                HeadRotation = MathHelper.Clamp(HeadRotation, MathHelper.ToRadians(0), MathHelper.ToRadians(15));
            else if (HeadRotation > MathHelper.ToRadians(270))
                HeadRotation = MathHelper.Clamp(HeadRotation, MathHelper.ToRadians(345), MathHelper.ToRadians(360));
            else if (HeadRotation > MathHelper.ToRadians(90))
                HeadRotation = MathHelper.Clamp(HeadRotation, MathHelper.ToRadians(165), MathHelper.ToRadians(195));
            base.Update(gameTime, currentKeyState, lastKeyState, false);
        }
        public float LeftLegRotation;
        public float RightLegRotation;
        public float LeftArmRotation;
        public float RightArmRotation;
        public float HeadRotation;
        public Vector2 HeadPosition;
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            Vector2 LegOrigin = new Vector2(Leg.Width / 2, Leg.Width / 2);
            spriteBatch.Draw(Leg, Position - new Vector2(0, Leg.Height - LegOrigin.Y), null, Color.White, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(Leg, Position - new Vector2(0, Leg.Height - LegOrigin.Y), null, Color.White, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);

            Vector2 ArmOrigin = new Vector2(Arm.Width / 2, Arm.Width / 2);
            spriteBatch.Draw(Arm, Position - new Vector2(0, (Leg.Height - LegOrigin.Y) + Body.Height), null, Color.White, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(Body, Position - new Vector2(Body.Width / 2, (Leg.Height - LegOrigin.Y) + Body.Height), null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            spriteBatch.Draw(Arm, Position - new Vector2(0, (Leg.Height - LegOrigin.Y) + Body.Height), null, Color.White, RightArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);

            Vector2 HeadOrigin = new Vector2(Head.Width / 2, Head.Width / 2);
            HeadPosition = Position - new Vector2(0, (Leg.Height - LegOrigin.Y) + Body.Height + (Head.Height - HeadOrigin.Y));
            if (HeadRotation > MathHelper.ToRadians(90) && HeadRotation < MathHelper.ToRadians(270))
                spriteBatch.Draw(Head, HeadPosition, null, Color.White, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
            else
                spriteBatch.Draw(Head, HeadPosition, null, Color.White, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);

            for (int i = 1; i <= localBounds.Height / Tile.Height; i++)
                spriteBatch.DrawRectangle(new Rectangle(((int)(Position.X / Tile.Width) + (Flip == SpriteEffects.None ? -1 : 1)) * 24, ((int)(Position.Y / Tile.Height) - i) * 24, 24, 24), Color.Green);
        }
        public bool IsWallInFront()
        {
            bool[] walls = new bool[localBounds.Height / Tile.Height];
            for (int i = 1; i <= localBounds.Height / Tile.Height; i++)
                if (level.tiles[(int)(position.X / Tile.Width) + ((Flip == SpriteEffects.None && Direction == MovementDirection.Left) ? -1 : 1), (int)(position.Y / Tile.Height) - i].Foreground.Collision == BlockCollision.Impassable)
                    walls[i - 1] = true;
            return walls.All(x => x);
        }
        /// <summary>
        /// Handle all input for the character, such as moving and jumping
        /// </summary>
        /// <param name="currentKeyState"></param>
        /// <param name="lastKeyState"></param>
        public virtual void HandleInput(KeyboardState currentKeyState, KeyboardState lastKeyState)
        {
            //If we are not in a textbox field (Like chatting), then move
            if (!(level.MainWindow.Manager.FocusedControl is TextBox) && !Game.IsMouseOnControl)
            {
                //Go left
                if (currentKeyState.IsKeyDown(Keys.H))
                    movement.X = -1;
                ////Go right
                if (currentKeyState.IsKeyDown(Keys.K))
                    movement.X = 1;
                ////Go Down
                //if (currentKeyState.IsKeyDown(Game.Controls["Down"]))
                //{
                //    movement.Y = 2;
                //}
                // if (currentKeyState.IsKeyDown(Game.Controls["Up"]))
                // {
                //     if (IsAdminMode)
                //         movement.Y = -1;
                //     isClimbing = false;
                //     if (IsAligned(BlockCollision.Ladder))
                //     {
                //         //We need to check the tile behind the player,
                //         //not what he is standing on
                //         if (level.GetTileBelowPlayer(position).Foreground.Collision == BlockCollision.Ladder)
                //         {
                //             isClimbing = true;
                //             isJumping = false;
                //             isOnGround = false;
                //             movement.Y = -level.GetTileBelowPlayer(position).Foreground.ClimbUpSpeed;
                //         }
                //     }
                // }
                //
                // else if (currentKeyState.IsKeyDown(Game.Controls["Down"]))
                // {
                //     isClimbing = false;
                //     if (IsAligned(BlockCollision.Ladder))
                //     {
                //         // Check the tile the player is standing on
                //         if (level.GetTileBelowPlayer(Position).Foreground.Collision == BlockCollision.Ladder)
                //         {
                //             isClimbing = true;
                //             isJumping = false;
                //             isOnGround = false;
                //             movement.Y = level.GetTileBelowPlayer(position).Foreground.ClimbDownSpeed;
                //         }
                //     }
                // }


                // Check if the player wants to jump.
                // isJumping = currentKeyState.IsKeyDown(Game.Controls["Up"]);
                // if (currentKeyState.IsKeyDown(Game.Controls["Up"]))
                //     Achievement.Show(Achievement.Jump);

            }
        }

        public int FindSlot(Item item)
        {
            for (int x = 0; x <= Inventory.Length - 1; x++)
            {
                if (Inventory[x].Item == item && Inventory[x].Stack <= Inventory[x].Item.MaxStack)
                    return x;

            }
            for (int x = 0; x <= Inventory.Length - 1; x++)
            {
                if (Inventory[x].Item == Item.Blank)
                {
                    Inventory[x] = new Slot(item, 0);
                    return x;
                }
            }
            return -1;
        }
        public bool FitsInInventory(Slot s)
        {
            int totalSpaces = 0;
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i].Item.ID == s.Item.ID)
                    totalSpaces += Inventory[i].Item.MaxStack - Inventory[i].Stack;
                if (Inventory[i].Item.ID == Item.Blank.ID)
                    return true;
            }
            return totalSpaces >= s.Stack;
        }
        public bool AddToInventory(Item item)
        {
            int i = FindSlot(item);
            if (i != -1)
            {
                Inventory[i].Add();
                Interface.MainWindow.inventory_MoveItem(Interface.MainWindow.inventory, null);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public override void OnKilled(Character killedBy, DamageType damageType = DamageType.None)
        {
            // TODO: Add better die animation

            base.OnKilled(killedBy, damageType);
        }
        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This parameter is null if the player was
        /// not killed by an enemy (fell into a hole).
        /// </param>
        public override void OnKilled(DamageType damageType = DamageType.None)
        {

            base.OnKilled(damageType);
        }
        public void Pay(int amount)
        {
            money += amount;
        }
        #endregion

    }
}
