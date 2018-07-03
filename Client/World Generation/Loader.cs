using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ZarknorthClient
{
    // For loading Images for structures
    public partial class WorldGen
    {
       
        public bool Rand()
        {
            return random.Next(0, 2) == 1;
        }
        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                    colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
 
        public void LoadAndDraw(string name,int x,int y,bool fillUnder,out int width)
        {
            // TODO: Fix this
            Texture2D texture = null;//level.structuresContent[name];
            // Load the level and ensure all of the lines are the same length.
            width = texture.Width;
            List<string> lines = new List<string>();
            
            //2d array for precise data removal
            Color[,] Colors = TextureTo2DArray(texture);
            
            Dictionary<Color,Tile> Dict = new Dictionary<Color,Tile>();
            if (name.Contains("OakTree"))
            {
                Dict.Add(Color.White, new Tile(Item.OakLeaf));
                Dict.Add(Color.Red, new Tile(Item.OakLeaf) { Background = Item.OakTree });
                Dict.Add(Color.Black, new Tile(Item.Blank) { Background = Item.OakTree });
                Dict.Add(Color.Green, new Tile(Item.Grass) { Background = Item.OakTree });
            }
         
            for (int i = 0; i < Colors.GetLength(0) - 1; ++i)
            {
                for (int j = 0; j < Colors.GetLength(1) ; ++j)
                {
                    foreach (KeyValuePair<Color, Tile> pair in Dict)
                    {
                        if (pair.Key == Colors[i, j])
                        {
                            if (pair.Value.Foreground.Size == Item.One)
                            level.tiles[x  + i, y - texture.Height + j] = new Tile(pair.Value.Foreground) { Background = pair.Value.Background };
                            else
                                level.tiles[x + i, y - texture.Height + j] = new Tile(pair.Value.Foreground, x + i, y - texture.Height + j) { Background = pair.Value.Background };
                        }
                    }
                }
            }

        }
    }
}
