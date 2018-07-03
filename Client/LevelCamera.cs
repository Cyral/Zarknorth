using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace ZarknorthClient
{
    public class LevelCamera
    {
        /// <summary>
        /// Current position of the Camera
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 position = Vector2.One;

        /// <summary>
        /// Current position of the Camera
        /// </summary>
        public Vector2 PreviousPosition
        {
            get { return previousPosition; }
        }
        private Vector2 previousPosition;

        private Level level;

        public int Left, Top, Bottom, Right;

        public LevelCamera(Level level, Vector2 startPosition)
        {
            position = startPosition;
            this.level = level;
        }
        /// <summary>
        /// Calculate where the Camera should be positioned
        /// </summary>
        /// <param name="viewport">Viewport of the game/Camera</param>
        /// <param name="FollowPosition">Position the Camera should follow (ie, player.Position)</param>
        /// <param name="MouseScroll">Should the Camera move when it hits the screen edges?</param>
        public void ScrollCamera(float delta, Vector2 FollowPosition, bool MouseScroll)
        {

            previousPosition = position;
            //Add viewport dimensions to MainCamera.Position
            position.X = position.X + (Game.MainWindow.Width / 2);
            position.Y = position.Y + (Game.MainWindow.Height / 2);

            //Smoothly scroll the Camera between current cam position, and the players position
            float speed = 4f;
            position.X = MathHelper.Lerp(position.X, FollowPosition.X, speed * delta);
            position.Y = MathHelper.Lerp(position.Y, FollowPosition.Y, speed * delta);

            //Subtract viewport dimensions to MainCamera.Position
            position.X = position.X - (Game.MainWindow.Width / 2);
            position.Y = position.Y - (Game.MainWindow.Height / 2);

            //Round the MainCamera.Position, So the gfx card dosent try and antialize it (which will make it blurry, being a fraction of a pixel)
            position.Y = (float)Math.Round(position.Y);
            position.X = (float)Math.Round(position.X);

            float offSet = 4f;
            if (MouseScroll)
            {
                if (Game.level.currentMouseState.X == 0)
                    position.X = MathHelper.Lerp(position.X, position.X - offSet, speed);
                if (Game.level.currentMouseState.X == 0 + Game.MainWindow.Width - 1)
                    position.X = MathHelper.Lerp(position.X, position.X + offSet, speed);
                if (Game.level.currentMouseState.Y == 0)
                    position.Y = MathHelper.Lerp(position.Y, position.Y - offSet, speed);
                if (Game.level.currentMouseState.Y == 0 + Game.MainWindow.Height - 1)
                    position.Y = MathHelper.Lerp(position.Y, position.Y + offSet, speed);
            }

            //Clamp it so it cant go offscreen
            //position.X = MathHelper.Clamp(position.X, 2 * Tile.Width, ((level.Width - 2) * Tile.Width) - Game.MainWindow.Width);
            position.Y = MathHelper.Clamp(position.Y, 2 * Tile.Height, ((level.Height - 2) * Tile.Height) - Game.MainWindow.Height);

            //Calculate the bounds for the edges of the Camera
            Left = (int)Math.Floor(position.X / Tile.Width);
            Top = (int)Math.Floor(position.Y / Tile.Width);
            Right = Left + (int)((Game.MainWindow.Width) / (int)(Tile.Width)) + 2;
            Bottom = Top + (int)((Game.MainWindow.Height) / (int)(Tile.Height)) + 1;
        }
    }
}
