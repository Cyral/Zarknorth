using System;
using Cyral.Extensions.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient.Entities
{
    public class PlayerCharacter : Character
    {
        #region Properties
        /// <summary>
        /// The collision-box of the character
        /// </summary>
        public override Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - (Tile.Width / 2)) + localBounds.X;
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
        public Slot[] Inventory;
        public const int InventorySlots = 50;
        public Slot CurrentSlot { get { return Inventory[Interface.MainWindow.inventory.Selected]; } }
        public Texture2D SandboxTexture, Arm, Body, Head, Leg;
        public Texture2D[] PantsLeg, Hair, ShirtSleeve, Shoe, ShirtBody, PantsTop, Expression;
        public Color SkinColor, ShirtColor, PantsColor, HairColor, ShoeColor;
        public int PantsLegVariation, HairVariation, ShirtSleeveVariation, ShoeVariation, ShirtBodyVariation, PantsTopVariation, ExpressionVariation;
        public static int PantsLegTotal = 2, HairTotal = 0, ShirtSleeveTotal = 2, ShoeTotal = 3, ShirtBodyTotal = 1, PantsTopTotal = 1, ExpressionTotal = 4;
        private float LeftLegRotation, RightLegRotation, LeftArmRotation, RightArmRotation, HeadRotation;
        private Vector2 HeadPosition;
        private float timeHeld;
        private float sandboxShadowRotation;
        #endregion

        #region Contructor
        public PlayerCharacter(Level level, Vector2 spawnPosition)
            : base(level, spawnPosition)
        {
            SetupInventory();
            money = int.Parse(Game.random.Next(1, 10).ToString() + "0000");
        }
        #endregion

        #region Methods
        protected override void LoadContent()
        {
            Arm = ContentPack.Textures["entities\\player\\armSheet"];
            Body = ContentPack.Textures["entities\\player\\bodySheet"];
            Head = ContentPack.Textures["entities\\player\\headSheet"];
            Leg = ContentPack.Textures["entities\\player\\legSheet"];
            SandboxTexture = ContentPack.Textures["entities\\player\\sandbox"];
            //JacketBody = ContentPack.Textures["entities\\player\\jacketBodySheet"];
            //JacketSleeve = ContentPack.Textures["entities\\player\\jacketSleeveSheet"];
            PantsLeg = new Texture2D[PantsLegTotal];
            for (int i = 1; i <= PantsLegTotal; i++)
                PantsLeg[i - 1] = ContentPack.Textures["entities\\player\\pantsLeg" + i];
            PantsTop = new Texture2D[PantsTopTotal];
            for (int i = 1; i <= PantsTopTotal; i++)
                PantsTop[i - 1] = ContentPack.Textures["entities\\player\\pantsTop" + i];
            ShirtBody = new Texture2D[ShirtBodyTotal];
            for (int i = 1; i <= ShirtBodyTotal; i++)
                ShirtBody[i - 1] = ContentPack.Textures["entities\\player\\shirtBody" + i];
            ShirtSleeve = new Texture2D[ShirtSleeveTotal];
            for (int i = 1; i <= ShirtSleeveTotal; i++)
                ShirtSleeve[i - 1] = ContentPack.Textures["entities\\player\\shirtSleeve" + i];
            Expression = new Texture2D[ExpressionTotal];
            for (int i = 1; i <= ExpressionTotal; i++)
                Expression[i - 1] = ContentPack.Textures["entities\\player\\expression" + i];
            Shoe = new Texture2D[HairTotal];
            for (int i = 1; i <= HairTotal; i++)
                Hair[i - 1] = ContentPack.Textures["entities\\player\\hair" + i];
            Shoe = new Texture2D[ShoeTotal];
            for (int i = 1; i <= ShoeTotal; i++)
                Shoe[i - 1] = ContentPack.Textures["entities\\player\\shoe" + i];
            // Calculate bounds within texture size. (This is what determines the colision, add or subtract to change the bounds for arms and such)       
            int width = 14;
            int left = 5;
            int height = 66;
            int top = 0;
            localBounds = new Rectangle(left, top, width, height);
            //Load skin
            IO.LoadPlayerSkin(this);
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
            Inventory[2].Item = Item.Wood;
            Inventory[3].Item = Item.WoodPlank;
            Inventory[4].Item = Item.Dirt;
            Inventory[5].Item = Item.Stone;
            Inventory[6].Item = Item.Torch;
            Inventory[7].Item = Item.CraftingTable;
            Inventory[8].Item = Item.Blank;
            Inventory[9].Item = Item.Blank;

            for (int i = 0; i < InventorySlots; i++)
            {
                if (Inventory[i].Item != Item.Blank)
                    Inventory[i].Stack = Inventory[i].Item.MaxStack;
            }
        }

        public override void ApplyPhysics(GameTime gameTime, bool DefaultVelocity = true, bool CancelVelocity = true, bool Round = true)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Level.Gamemode == GameMode.Sandbox)
                ApplyAdminPhysics(elapsed, gameTime);
            else
                base.ApplyPhysics(gameTime);
        }

        private void ApplyAdminPhysics(float elapsed, GameTime gameTime)
        {
            velocity.X = movement.X * 600 * elapsed;
            velocity.Y = movement.Y * 600 * elapsed;

            position += velocity;
            position = new Vector2((float)Math.Round(position.X), (float)Math.Round(position.Y));

            movement = Vector2.Zero;
        }
        float animationFadeIn = 0;
        public override void Update(GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState currentKeyState, Microsoft.Xna.Framework.Input.KeyboardState lastKeyState, bool resetMovement = true)
        {
            Item SelectedItem = Inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item;
            if (SelectedItem is MineItem)
            {
                if (level.currentMouseState.LeftButton == ButtonState.Pressed)
                    timeHeld += (float)gameTime.ElapsedGameTime.TotalSeconds * (SelectedItem as MineItem).Multiplier * 1.35f;
                if (timeHeld > .5f)
                    timeHeld = 0;
            }
            HandleInput(currentKeyState, lastKeyState);
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            sandboxShadowRotation += movement.X == 0 ? elapsed / 3 : (elapsed * 1.5f) * movement.X;

            if ((Level.Gamemode == GameMode.Sandbox && velocity.X == 0 && !isClimbing) || (Level.Gamemode != GameMode.Sandbox && movement.X == 0))
            {
                float speed = 7;
                LeftLegRotation = MathHelper.Lerp(LeftLegRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                RightLegRotation = MathHelper.Lerp(RightLegRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                LeftArmRotation = MathHelper.Lerp(LeftArmRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                RightArmRotation = MathHelper.Lerp(RightArmRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                animationFadeIn = 0;
            }
            else
            {
                if (isClimbing)
                {
                    //Swingin' Hands
                    LeftLegRotation = MathHelper.Lerp(((float)Math.Cos(((position.Y / 5) * 0.6662F) * 2.0F * .4f * 0.5F) / 4) * (Flip == SpriteEffects.None ? 1 : -1), LeftLegRotation, 1f - animationFadeIn);
                    RightLegRotation = MathHelper.Lerp(((float)Math.Cos(((position.Y / 5) * 0.6662F + Math.PI) * 2.0F * .4f * 0.5F) / 4) * (Flip == SpriteEffects.None ? 1 : -1), RightLegRotation, 1f - animationFadeIn);

                    LeftArmRotation = MathHelper.Lerp(((float)Math.Cos(((position.Y / 7) * 0.6662F) * 2.0F * .4f * 0.5F) + MathHelper.ToRadians(180)) * (Flip == SpriteEffects.None ? 1 : -1), LeftArmRotation, 1f - animationFadeIn);
                    RightArmRotation = MathHelper.Lerp(((float)Math.Cos(((position.Y / 7) * 0.6662F + Math.PI) * 2.0F * .4f * 0.5F) + MathHelper.ToRadians(180)) * (Flip == SpriteEffects.None ? 1 : -1), RightArmRotation, 1f - animationFadeIn);
                    if (animationFadeIn < 1f)
                        if (animationFadeIn < 1f)
                            animationFadeIn += elapsed * 5.5f;
                }
                else
                {
                    //Swingin' Hands
                    LeftLegRotation = MathHelper.Lerp(((float)Math.Cos((position.X / 20) * 0.6662F) * 2.0F * .7f * 0.5F) * (Flip == SpriteEffects.None ? 1 : -1), LeftLegRotation, 1f - animationFadeIn);
                    RightLegRotation = MathHelper.Lerp(((float)Math.Cos((position.X / 20) * 0.6662F + Math.PI) * 2.0F * .7f * 0.5F) * (Flip == SpriteEffects.None ? 1 : -1), RightLegRotation, 1f - animationFadeIn);

                    LeftArmRotation = MathHelper.Lerp(((float)Math.Cos((position.X / 20) * 0.6662F) * 2.0F * .4f * 0.5F) * (Flip == SpriteEffects.None ? 1 : -1), LeftArmRotation, 1f - animationFadeIn);
                    RightArmRotation = MathHelper.Lerp(((float)Math.Cos((position.X / 20) * 0.6662F + Math.PI) * 2.0F * .4f * 0.5F) * (Flip == SpriteEffects.None ? 1 : -1), RightArmRotation, 1f - animationFadeIn);
                    if (animationFadeIn < 1f)
                        if (animationFadeIn < 1f)
                            animationFadeIn += elapsed * 5.5f;
                }
            }
            if (!isOnGround && !isClimbing && velocity.X == 0)
            {

                //Jump/Fall Animation
                if (Flip == SpriteEffects.None)
                {
                    RightArmRotation += 7f * elapsed;
                    LeftArmRotation += -7f * elapsed;
                    RightLegRotation += 7f * elapsed;
                    LeftLegRotation += -7f * elapsed;
                }
                else
                {
                    RightArmRotation -= 10f * elapsed;
                    LeftArmRotation -= -10f * elapsed;
                    RightLegRotation -= 10f * elapsed;
                    LeftLegRotation -= -10f * elapsed;
                }
                animationFadeIn = .08f;
                float speed = 7;
                LeftLegRotation = MathHelper.Lerp(LeftLegRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                RightLegRotation = MathHelper.Lerp(RightLegRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                LeftArmRotation = MathHelper.Lerp(LeftArmRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                RightArmRotation = MathHelper.Lerp(RightArmRotation, 0f, (float)gameTime.ElapsedGameTime.TotalSeconds * speed);
                if (animationFadeIn < 1f)
                    if (animationFadeIn < 1f)
                        animationFadeIn += elapsed * 5.5f;
                animationFadeIn = 0;
            }

            Vector2 direction = (level.MainCamera.Position + new Vector2(Game.mouseState.X, Game.mouseState.Y)) - HeadPosition;
            direction.Normalize();
            HeadRotation = (float)Math.Atan2((double)direction.Y, (double)direction.X) + MathHelper.PiOver2 + MathHelper.ToRadians(90);

            if (HeadRotation < MathHelper.ToRadians(90))
                HeadRotation = MathHelper.Clamp(HeadRotation, MathHelper.ToRadians(0), MathHelper.ToRadians(15));
            else if (HeadRotation > MathHelper.ToRadians(270))
                HeadRotation = MathHelper.Clamp(HeadRotation, MathHelper.ToRadians(345), MathHelper.ToRadians(360));
            else if (HeadRotation > MathHelper.ToRadians(90))
                HeadRotation = MathHelper.Clamp(HeadRotation, MathHelper.ToRadians(165), MathHelper.ToRadians(195));
   
            SelectedItem = Inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item;
            Vector2 ArmDirection = (level.MainCamera.Position + new Vector2(Game.mouseState.X, Game.mouseState.Y)) - HeadPosition;
            ArmDirection.Normalize();
            bool flip = level.currentMouseState.X > level.game.GraphicsDevice.PresentationParameters.BackBufferWidth / 2;

            if (SelectedItem != Item.Blank && (Level.EditType != EditType.Erase && (Level.EditTool == EditTool.Default)))
            {
                if (SelectedItem is BlockItem)
                    LeftArmRotation = (float)Math.Atan2((double)ArmDirection.Y, (double)ArmDirection.X) - MathHelper.PiOver2;
                if (SelectedItem is MineItem && level.currentMouseState.LeftButton == ButtonState.Pressed)
                    LeftArmRotation = (float)Math.Atan2((double)ArmDirection.Y, (double)ArmDirection.X) - MathHelper.PiOver2 - MathHelper.ToRadians((timeHeld * 40) * (flip ? -1 : 1));
            }

            base.Update(gameTime, currentKeyState, lastKeyState);
        }
        public override void DrawCharacter(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 DrawPosition = Position - new Vector2(7, 0);
            if (Level.Gamemode == GameMode.Sandbox)
            spriteBatch.Draw(SandboxTexture, DrawPosition + new Vector2(localBounds.Width / 2, -localBounds.Height / 2), null, Color.White, sandboxShadowRotation, new Vector2(SandboxTexture.Width / 2, SandboxTexture.Height / 2), 1f, SpriteEffects.None, 0);
                
            if (Flip == SpriteEffects.None)
            {
                //Find positions and origins
                Vector2 LegOrigin = new Vector2(Leg.Width / 2, Leg.Width / 2);
                Vector2 ArmOrigin = new Vector2(Arm.Width / 2, Arm.Width / 2);
                Vector2 HeadOrigin = new Vector2(Head.Width / 2, Head.Width / 2);
                HeadPosition = DrawPosition + new Vector2(-2, -66) + (HeadOrigin);

                //Draw BackArm
                spriteBatch.Draw(Arm, DrawPosition + new Vector2(2, -43), null, SkinColor, RightArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
                //Draw BackShirt
                spriteBatch.Draw(ShirtSleeve[ShirtSleeveVariation], DrawPosition + new Vector2(2, -44), null, ShirtColor, RightArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
                //Draw Backleg
                spriteBatch.Draw(Leg, DrawPosition + new Vector2(5, -24), null, SkinColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
                //Draw BackPantsLeg
                spriteBatch.Draw(PantsLeg[PantsLegVariation], DrawPosition + new Vector2(5, -24), null, PantsColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
                //Draw BackShoes
                spriteBatch.Draw(Shoe[ShoeVariation], DrawPosition + new Vector2(4, -24), null, ShoeColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
                //Draw FrontLeg
                spriteBatch.Draw(Leg, DrawPosition + new Vector2(9, -24), null, SkinColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
                //Draw FrontPantsLeg
                spriteBatch.Draw(PantsLeg[PantsLegVariation], DrawPosition + new Vector2(9, -24), null, PantsColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
                //Draw FrontShoes
                spriteBatch.Draw(Shoe[ShoeVariation], DrawPosition + new Vector2(8, -24), null, ShoeColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
                //Draw Body
                spriteBatch.Draw(Body, DrawPosition + new Vector2(1, -53), null, SkinColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                //Draw Shirt
                spriteBatch.Draw(ShirtBody[ShirtBodyVariation], DrawPosition + new Vector2(0, -48), null, ShirtColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                //Draw PantsTop
                spriteBatch.Draw(PantsTop[PantsTopVariation], DrawPosition + new Vector2(0, -26), null, PantsColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                //Draw Head and Face
                if (HeadRotation > MathHelper.ToRadians(90) && HeadRotation < MathHelper.ToRadians(270))
                {
                    spriteBatch.Draw(Head, HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
                    spriteBatch.Draw(Expression[ExpressionVariation], HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
                }
                else
                {
                    spriteBatch.Draw(Head, HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Expression[ExpressionVariation], HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);
                }
                DrawPosition = DrawHandAnimation(spriteBatch, DrawPosition);
                //Draw FrontArm
                spriteBatch.Draw(Arm, DrawPosition + new Vector2(11, -43), null, SkinColor, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
                //Draw FrontShirt
                spriteBatch.Draw(ShirtSleeve[ShirtSleeveVariation], DrawPosition + new Vector2(11, -44), null, ShirtColor, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
               
            }
            else
            {
                //Find positions and origins
                Vector2 LegOrigin = new Vector2(Leg.Width / 2, Leg.Width / 2);
                Vector2 ArmOrigin = new Vector2(Arm.Width / 2, Arm.Width / 2);
                Vector2 HeadOrigin = new Vector2(Head.Width / 2, Head.Width / 2);
                HeadPosition = DrawPosition + new Vector2(-2, -66) + (HeadOrigin);

                //Draw BackArm
                spriteBatch.Draw(Arm, DrawPosition + new Vector2(11, -43), null, SkinColor, RightArmRotation, ArmOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw BackShirt
                spriteBatch.Draw(ShirtSleeve[ShirtSleeveVariation], DrawPosition + new Vector2(11, -44), null, ShirtColor, RightArmRotation, ArmOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw Backleg
                spriteBatch.Draw(Leg, DrawPosition + new Vector2(8, -24), null, SkinColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw BackPantsLeg
                spriteBatch.Draw(PantsLeg[PantsLegVariation], DrawPosition + new Vector2(8, -24), null, PantsColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw BackShoes
                spriteBatch.Draw(Shoe[ShoeVariation], DrawPosition + new Vector2(8, -24), null, ShoeColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw FrontLeg
                spriteBatch.Draw(Leg, DrawPosition + new Vector2(4, -24), null, SkinColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw FrontPantsLeg
                spriteBatch.Draw(PantsLeg[PantsLegVariation], DrawPosition + new Vector2(4, -24), null, PantsColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw FrontShoes
                spriteBatch.Draw(Shoe[ShoeVariation], DrawPosition + new Vector2(4, -24), null, ShoeColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw Body
                spriteBatch.Draw(Body, DrawPosition + new Vector2(2, -53), null, SkinColor, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw Shirt
                spriteBatch.Draw(ShirtBody[ShirtBodyVariation], DrawPosition + new Vector2(1, -48), null, ShirtColor, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw PantsTop
                spriteBatch.Draw(PantsTop[PantsTopVariation], DrawPosition + new Vector2(0, -26), null, PantsColor, 0, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw Head and Face
                if (HeadRotation > MathHelper.ToRadians(270) || HeadRotation < MathHelper.ToRadians(90))
                {
                    spriteBatch.Draw(Head, HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);
                    spriteBatch.Draw(Expression[ExpressionVariation], HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(Head, HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
                    spriteBatch.Draw(Expression[ExpressionVariation], HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
                }
                DrawPosition = DrawHandAnimation(spriteBatch, DrawPosition);

                //Draw FrontArm
                spriteBatch.Draw(Arm, DrawPosition + new Vector2(2, -43), null, SkinColor, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                //Draw FrontShirt
                spriteBatch.Draw(ShirtSleeve[ShirtSleeveVariation], DrawPosition + new Vector2(2, -44), null, ShirtColor, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.FlipHorizontally, 0);
                
            }
        }

        private Vector2 DrawHandAnimation(SpriteBatch spriteBatch, Vector2 DrawPosition)
        {
            Item SelectedItem = Inventory[ZarknorthClient.Interface.MainWindow.inventory.Selected].Item;
            if (Level.EditType == EditType.Erase || (Level.EditTool != EditTool.Default && !(SelectedItem is BlockItem)))
                return DrawPosition;
            Vector2 ArmDirection = (level.MainCamera.Position + new Vector2(Game.mouseState.X, Game.mouseState.Y)) - HeadPosition;
            ArmDirection.Normalize();
            float rot = (float)Math.Atan2((double)ArmDirection.Y, (double)ArmDirection.X);
            bool flip = level.currentMouseState.X > spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth / 2;
            if (SelectedItem != Item.Blank)
            {
                if (SelectedItem is BlockItem)
                {
                    if (Flip == SpriteEffects.None)
                        spriteBatch.Draw(SelectedItem.Textures[0],
                            (DrawPosition + new Vector2(10, -43) + new Vector2(-20, 0)).RotateAboutOrigin((DrawPosition + new Vector2(10, -43)),
                            (float)Math.Atan2((double)ArmDirection.Y, (double)ArmDirection.X) + MathHelper.ToRadians(180)),
                            null,
                            Color.White,
                            rot + MathHelper.ToRadians(135) + (flip ? MathHelper.ToRadians(180) : MathHelper.ToRadians(90)),
                            new Vector2(SelectedItem.Textures[0].Width / 2, SelectedItem.Textures[0].Height / 2), .5f,
                            SpriteEffects.None, 0);
                    else
                        spriteBatch.Draw(SelectedItem.Textures[0],
        (DrawPosition + new Vector2(3, -43) + new Vector2(-20, 0)).RotateAboutOrigin((DrawPosition + new Vector2(3, -43)),
        (float)Math.Atan2((double)ArmDirection.Y, (double)ArmDirection.X) + MathHelper.ToRadians(180)),
        null,
        Color.White,
        rot + MathHelper.ToRadians(135) + (flip ? MathHelper.ToRadians(180) : MathHelper.ToRadians(90)),
        new Vector2(SelectedItem.Textures[0].Width / 2, SelectedItem.Textures[0].Height / 2), .5f,
        SpriteEffects.None, 0);
                }
                else if (SelectedItem is MineItem && level.currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    spriteBatch.Draw(SelectedItem.Textures[0],
                        (DrawPosition + new Vector2(Flip == SpriteEffects.None ? 10 : 3, -50 + (flip ? 15 : 0)) + new Vector2(-17, 0)).RotateAboutOrigin((DrawPosition + new Vector2(Flip == SpriteEffects.None ? 10 : 3, -43)),
                        rot + MathHelper.ToRadians(180) - MathHelper.ToRadians((timeHeld * 40) * (flip ? -1 : 1))),
                        null,
                        Color.White,
                        rot + MathHelper.ToRadians(135) - MathHelper.ToRadians((timeHeld * 40) * (flip ? -1 : 1)) + (flip ? MathHelper.ToRadians(180) : 0),
                        new Vector2(SelectedItem.Textures[0].Width / 2, SelectedItem.Textures[0].Height / 2), 1,
                        SpriteEffects.None, 0);

                }
            }
            return DrawPosition;
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
                if (currentKeyState.IsKeyDown(Game.Controls["Left"]))
                    movement.X = -1;
                //Go right
                if (currentKeyState.IsKeyDown(Game.Controls["Right"]))
                    movement.X = 1;
                if (currentKeyState.IsKeyDown(Game.Controls["Up"]))
                {
                    if (Level.Gamemode == GameMode.Sandbox)
                        movement.Y = -1;
                    isClimbing = false;
                    if (IsAligned(BlockCollision.Ladder))
                    {
                        if (level.GetTileBelowPlayer(Position).Foreground.Collision == BlockCollision.Ladder && level.GetTileAbovePlayer(this).Foreground.Collision != BlockCollision.Impassable)
                        {
                            isClimbing = true;
                            isJumping = false;
                            isOnGround = false;
                            movement.Y = -level.GetTileBelowPlayer(position).Foreground.ClimbUpSpeed;
                        }
                    }
                }

                else if (currentKeyState.IsKeyDown(Game.Controls["Down"]))
                {
                    movement.Y = 1;
                    isClimbing = false;
                    if (IsAligned(BlockCollision.Ladder))
                    {
                        // Check the tile the player is standing on
                        if (level.GetTileBelowPlayer(Position).Foreground.Collision == BlockCollision.Ladder)
                        {
                            isClimbing = true;
                            isJumping = false;
                            isOnGround = false;
                            movement.Y = level.GetTileBelowPlayer(position).Foreground.ClimbDownSpeed;
                        }
                    }
                }


                // Check if the player wants to jump.
                isJumping = currentKeyState.IsKeyDown(Game.Controls["Up"]);
                if (currentKeyState.IsKeyDown(Game.Controls["Up"]))
                    Achievement.Show(Achievement.Jump);

            }
        }

        public int FindSlot(Item item)
        {
            for (int x = 0; x <= Inventory.Length - 1; x++)
            {
                if (Inventory[x].Item == item && Inventory[x].Stack < Inventory[x].Item.MaxStack)
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

        public void Pay(int amount)
        {
            money += amount;
        }
        #endregion

    }
}
