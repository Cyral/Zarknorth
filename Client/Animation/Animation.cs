
using System;
using Microsoft.Xna.Framework.Graphics;

namespace ZarknorthClient
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    public class Animation
    {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth;
       

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight;
       

        /// <summary>
        /// Constructors a new animation.
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping,int Width = 0,int Height = 0)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
            if (Width != 0)
            {
                FrameHeight = Height;
                FrameWidth = Width;
            }
            else
            {
                FrameHeight = texture.Height;
                FrameWidth = FrameHeight;
            }
        }
    }
}
