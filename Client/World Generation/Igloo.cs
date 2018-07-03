//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;

//namespace ZarknorthClient
//{
//    public partial class WorldGen
//    {
//        /// <summary>
//        /// Creates Igloos in the world
//        /// </summary>
//        /// <param name="percent">Percent to be dedicated to igloos</param>
//        /// <param name="maxheight">Maximum Height</param>
//        /// <param name="minheight">Minimum Height</param>
//        void Igloo(float percent, int maxheight, int minheight)
//        {
//            for (int x = 1; x < level.Width - 40; x++)
//            {
//                if ((randomizer.Next(100) <= percent) && (level.tiles[x, HeightMap[x]].Foreground == Item.Snow))
//                {
//                    int Height = randomizer.Next(minheight, maxheight); //How tall to make it (In blocks)
//                    int Length = randomizer.Next((int)(minheight * 1.5), (int)(maxheight * 1.5)); //Length should be longer than height
//                    Direction dir = (Direction)Enum.ToObject(typeof(Direction), randomizer.Next(0, 2)); //Pick a random direction to face
//                    //Make first wall
//                    level.tiles[(int)x + (Length * (int)dir), HeightMap[x] - 4] = new Tile(Item.Ice);
//                    level.tiles[(int)x + (Length * (int)dir), HeightMap[x] - 3] = new Tile(Item.Ice);
//                    level.tiles[(int)x + (Length * (int)dir), HeightMap[x] - 2] = new Tile(Item.Ice);
//                    level.tiles[(int)x + (Length * (int)dir), HeightMap[x] - 1] = new Tile(Item.Ice);
//                    for (int j = 0; j < Length; j++) //Add roof arc
//                    {

//                        float a = (float)j / (float)Length;

//                        //Use Math.Sin to make a wave for roof
//                        Vector2 Pos = new Vector2((float)a * (Length), Height - (Height * (float)Math.Sin(a * MathHelper.Pi) / 2) - (Height / 2) - 3);
//                        try
//                        {
//                            level.tiles[(int)x + j, HeightMap[x]] = new Tile(Item.Ice);//Make floor
//                            level.tiles[(int)Pos.X + x, HeightMap[x] + (int)Pos.Y - 5] = new Tile(Item.Ice);//Make Curved shape
//                        }
//                        catch { }

//                    }
//                    //Calculate opening to igloo
//                    int exLength = 0;
//                    int exStart = 0;
//                    if (dir == Direction.btnLeft)
//                    {
//                        for (int i = 0; i <= Length / 4; i++) //Make opening (1/4 of the length)
//                        {
//                            level.tiles[(int)x + Length + i, HeightMap[x]] = new Tile(Item.Ice);
//                            level.tiles[(int)x + Length + i, HeightMap[x] - 4] = new Tile(Item.Ice);
//                        }
//                        exLength += Length / 4;
//                    }
//                    else if (dir == Direction.btnRight)
//                    {
//                        for (int i = -(Length / 4); i <= 0; i++) //Make opening (1/4 of the length)
//                        {
//                            level.tiles[(int)x + i, HeightMap[x]] = new Tile(Item.Ice);
//                            level.tiles[(int)x + i, HeightMap[x] - 4] = new Tile(Item.Ice);
//                        }
//                        exStart -= Length / 4;
//                    }
//                    for (int a = exStart; a <= Length + exLength; a++) //Fill with BG, Loop through each floor tiles, and go up and add until we hit the roof
//                    {
//                        for (int b = 1; b <= Height; b++) //Go up to top 
//                        {
//                            //Stop if we hit a block (or if we go to high)
//                            if (level.tiles[(int)x + a, HeightMap[x] - b].Foreground == Item.Ice || b > Height)
//                                break;
//                            else
//                                level.tiles[(int)x + a, HeightMap[x] - b] = new Tile(Item.Blank) { Background = Item.IceBG};
//                        }
//                    }
//                }
//            }
//        }
//    }
//}
