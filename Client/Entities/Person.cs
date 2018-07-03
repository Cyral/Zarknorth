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
    public class Person : Npc
    {

        public bool testMode = true;
        public enum RelationPlayer
        {
            Follow = 1,
            Avoid = 2,
            Pass = 3
        }
        public Person(Level level, Vector2 spawnPosition)
            : base(level, spawnPosition)
        {
            canPickUp = true;
            talking = true;
        }
        protected override void LoadContent()
        {
            Arm = ContentPack.Textures["entities\\player\\armSheet"];
            Body = ContentPack.Textures["entities\\player\\bodySheet"];

            Head = ContentPack.Textures["entities\\player\\headSheet"];
            Leg = ContentPack.Textures["entities\\player\\legSheet"];

            // Calculate bounds within texture size. (This is what determines the colision, add or subtract to change the bounds for arms and such)       
            int width = 14;
            int left = 5;
            int height = Tile.Height * 3;
            int top = 0;
            localBounds = new Rectangle(left, top, width, height);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, Microsoft.Xna.Framework.Input.KeyboardState currentKeyState, Microsoft.Xna.Framework.Input.KeyboardState lastKeyState, bool resetMovement = false)
        {
            HandleInput(currentKeyState, lastKeyState);
            Movement();
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
        public void Movement()
        {
            UpdateDirection();

            if (IsWallInFront())
            {
                movement.X = -movement.X;
            }
            else
            {
                //movement.X = 1;
            }
        }
    }
}
