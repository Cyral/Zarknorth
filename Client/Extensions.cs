using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace ZarknorthClient
{
    /// <summary>
    /// A set of helpful methods
    /// </summary>
    public static class Extensions
    {
        #region Audio
        public static FadableSoundEffectInstance CreateFadableInstance(this SoundEffect soundEffect)
        {
            return new FadableSoundEffectInstance(soundEffect);
        }
        #endregion

        #region Check if window is focused/activated
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);
        /// <summary>
        /// Checks if the application is focused/activated. Apparently there is a glitch with XNA I found, IsActive will not work always
        /// when the application size it set to the screen size (fullscreen but not in fullscreen mode)
        /// </summary>
        /// <returns>Returns true if the current application has focus, false otherwise</returns>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }
        #endregion

        #region Color Extensions
        /// <summary>
        /// Convert HSV to RGB
        /// h is from 0-360
        /// s,v values are 0-1
        /// r,g,b values are 0-255
        /// Based upon http://ilab.usc.edu/wiki/index.php/HSV_And_H2SV_Color_Space#HSV_Transformation_C_.2F_C.2B.2B_Code_2
        /// </summary>
        public static Color ColorFromHSV(double h, double S, double V)
        {
            int r, g, b;

            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }
            r = Clamp((int)(R * 255.0));
            g = Clamp((int)(G * 255.0));
            b = Clamp((int)(B * 255.0));
            return new Color(r,g,b);
        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
        public static Color GetBlendedColor(int percentage)
        {
            float multiplier = (float)percentage / 100f;
            return new Color((byte)(255 - (255 * multiplier)), (byte)(255 * multiplier), 0);
        }
        public static Color GetBlendedColorAlt(int percentage)
        {
            float multiplier = (float)percentage / 100f;
            return new Color(0, (byte)(255 * multiplier), (byte)(255 - (255 * multiplier)));
        }
        #endregion

        #region Vector2 Extensions
        public static Vector2 ToVector2(this Point p)
        {
            return new Vector2(p.X, p.Y);
        }
        #endregion

        #region SpriteBatch Drawline
        /// <summary>
        /// Draws a single line. 
        /// Require SpriteBatch.Begin() and SpriteBatch.End()
        /// </summary>
        /// <param name="begin">Begin point.</param>
        /// <param name="end">End point.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 1, Texture2D texture = null)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(texture == null ? TexGen.White : texture, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
        public static void DrawLineArrow(this SpriteBatch spriteBatch, Texture2D arrow, Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(TexGen.White, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
            spriteBatch.Draw(arrow, new Rectangle((int)((begin.X + end.X) / 2), (int)((begin.Y + end.Y) / 2), arrow.Width, arrow.Height), null, new Color(color.R + 10, color.G + 10, color.B + 10), angle - MathHelper.ToRadians(90), new Vector2(arrow.Width / 2f, arrow.Height / 2f), SpriteEffects.None, 0);
        }
        /// <summary>
        /// Draws a single line. 
        /// Doesn't require SpriteBatch.Begin() or SpriteBatch.End()
        /// </summary>
        /// <param name="begin">Begin point.</param>
        /// <param name="end">End point.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        public static void DrawSingleLine(this SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 1, Texture2D texture = null)
        {
            spriteBatch.Begin();
            spriteBatch.DrawLine(begin, end, color, width, texture);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws a poly line.
        /// Doesn't require SpriteBatch.Begin() or SpriteBatch.End()
        /// <param name="points">The points.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="closed">Whether the shape should be closed.</param>
        public static void DrawPolyLine(this SpriteBatch spriteBatch, Vector2[] points, Color color, int width = 1, bool closed = false)
        {
            for (int i = 0; i < points.Length - 1; i++)
                spriteBatch.DrawLine(points[i], points[i + 1], color, width);
            if (closed)
                spriteBatch.DrawLine(points[points.Length - 1], points[0], color, width);
        }

        /// <summary>
        /// The graphics device, set this before drawing lines
        /// </summary>
        public static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Generates a 1 pixel white texture used to draw lines.
        /// </summary>
        static class TexGen
        {
            static Texture2D white = null;
            /// <summary>
            /// Returns a single pixel white texture, if it doesn't exist, it creates one
            /// </summary>
            /// <exception cref="System.Exception">Please set the SpriteBatchEx.GraphicsDevice to your graphicsdevice before drawing lines.</exception>
            public static Texture2D White
            {
                get
                {
                    if (white == null)
                    {
                        if (Extensions.GraphicsDevice == null)
                            throw new Exception("Please set the SpriteBatchEx.GraphicsDevice to your GraphicsDevice before drawing lines.");
                        white = new Texture2D(Extensions.GraphicsDevice, 1, 1);
                        Color[] color = new Color[1];
                        color[0] = Color.White;
                        white.SetData<Color>(color);
                    }
                    return white;
                }
            }
        }
        #endregion

        #region Texture2D Extensions
        public static Color[,] TextureTo2DArray(this Texture2D texture)
        {
            Color[] colorsOne = new Color[texture.Width * texture.Height]; //The hard to read,1D array
            texture.GetData(colorsOne); //Get the colors and add them to the array

            Color[,] colorsTwo = new Color[texture.Width, texture.Height]; //The new, easy to read 2D array
            for (int x = 0; x < texture.Width; x++) //Convert!
                for (int y = 0; y < texture.Height; y++)
                    colorsTwo[x, y] = colorsOne[x + y * texture.Width];

            return colorsTwo; //Done!
        }
        public static Texture2D TextureTo2DArray(Texture2D texture, Color[,] data)
        {
            int Width = data.GetUpperBound(0);
            int Height = data.GetUpperBound(1);
            Color[] colorsOne = new Color[Width * Height]; //The new, hard to read,1D array
            Color[,] colorsTwo = new Color[Width, Height]; //The easy to read 2D array
            for (int x = 0; x < Width; x++) //Convert!
                for (int y = 0; y < Height; y++)
                    colorsOne[x + y * Width] = colorsTwo[x, y];
            texture.SetData<Color>(colorsOne);
            return texture;
        }
        #endregion

        #region Collision Extensions
        /// <summary>
        /// Calculates the signed depth of intersection between two rectangles.
        /// </summary>
        /// <returns>
        /// The amount of overlap between two intersecting rectangles. These
        /// depth values can be negative depending on which wides the rectangles
        /// intersect. This allows callers to determine the correct direction
        /// to push objects in order to resolve collisions.
        /// If the rectangles are not intersecting, Vector2.Zero is returned.
        /// </returns>
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Calculate and return intersection depths.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Gets the position of the center of the bottom edge of the rectangle.
        /// </summary>
        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
        public static float GetHorizontalIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;

            // Calculate centers.
            float centerA = rectA.Left + halfWidthA;
            float centerB = rectB.Left + halfWidthB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceX = centerA - centerB;
            float minDistanceX = halfWidthA + halfWidthB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX)
                return 0f;

            // Calculate and return intersection depths.
            return distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        }

        public static float GetVerticalIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Calculate half sizes.
            float halfHeightA = rectA.Height / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Calculate centers.
            float centerA = rectA.Top + halfHeightA;
            float centerB = rectB.Top + halfHeightB;

            // Calculate current and minimum-non-intersecting distances between centers.
            float distanceY = centerA - centerB;
            float minDistanceY = halfHeightA + halfHeightB;

            // If we are not intersecting at all, return (0, 0).
            if (Math.Abs(distanceY) >= minDistanceY)
                return 0f;

            // Calculate and return intersection depths.
            return distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        }
        #endregion

    }
}
