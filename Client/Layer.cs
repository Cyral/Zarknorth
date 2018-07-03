using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
    
namespace ZarknorthClient
{
    public class LayerManager
    {
        public Layer[] layers = new Layer[9];
        public bool Set;
        public LayerManager(ContentManager content, string basePath, Vector2 scrollRate1, Vector2 scrollRate2, Vector2 scrollRate3,bool set = false)
        {
            Set = set;
            if (!set)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (i == 0)
                        layers[i] = new Layer(content, basePath + i, scrollRate1, set);
                    if (i == 1)
                        layers[i] = new Layer(content, basePath + i, scrollRate2, set);
                    if (i == 2)
                        layers[i] = new Layer(content, basePath + i, scrollRate3, set);
                }
            }
            else
            {
                layers[0] = new Layer(content, basePath, scrollRate3, set);
            }
        }
    }
    /// <summary>
    /// Class for Parallax scrolling layers.
    /// </summary>
    public class Layer 
    {
        public Texture2D[] Textures { get; private set; }
        public Vector2 ScrollRate { get; private set; }
        public Layer(ContentManager content, string basePath, Vector2 scrollRate,bool set = false)
        {
            // Assumes each layer only has 3 segments.
            if (set == false)
            {
                Textures = new Texture2D[3];
                for (int i = 0; i < 3; ++i)
                    Textures[i] = ContentPack.Textures["backgrounds\\" + basePath +  "_" + i];
            }
            else
            {
                Textures = new Texture2D[1];
                Textures[0] = ContentPack.Textures["backgrounds\\" + basePath];
            }
            ScrollRate = scrollRate;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 Position,Color color)
        {
            // Assume each segment is the same width.
            int segmentWidth = Textures[0].Width;
            int segmentHeight = Textures[0].Height;

            // Calculate which segments to draw and how much to offset them.
            float x = Math.Abs(Position.X * ScrollRate.X);

            int leftSegment = (int)Math.Floor(x / segmentWidth);
            int rightSegment = leftSegment + 1;
            int rightSegment2 = rightSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth ;
            spriteBatch.Draw(Textures[leftSegment % Textures.Length], new Vector2(x, 0), color);
            spriteBatch.Draw(Textures[rightSegment % Textures.Length], new Vector2(x + segmentWidth, 0), color);
            spriteBatch.Draw(Textures[rightSegment2 % Textures.Length], new Vector2(x + segmentWidth + segmentWidth, 0), color);
           
        }
    }
}
