using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    /// <summary>
    /// Class for creating rooms in houses, castles etc
    /// </summary>
    public class RoomMaker
    {
        public Level Level
        {
            get { return level; }
        }
        Level level;
        private WorldGen generator
        {
            get { return level.worldGen; }
            set { level.worldGen = value; }
        }
        Random random;
        public RoomMaker(Level level_,Random random_)
        {
            level = level_;
            random = random_;
        }
        public void CastleRooms(int x, int y, int width, int height)
        {
            //List<Rectangle> castleRooms = new List<Rectangle>();
            //Rectangle mainRect = new Rectangle(x, y, width, height);
            //Room.Init();
            //for (int i = 0; i <= 3; i++)
            //{
            //    for (int j = 0; j <= 3; j++)
            //    {
            //        if (j<3 && i < 3)
            //        castleRooms.Add(new Rectangle(x + (i * (width / 4)), y + (j * (height / 4)), (width / 4) * random.Next(1, 4), (height / 4) * random.Next(1, 2)));
            //        else
            //        castleRooms.Add(new Rectangle(x + (i * (width / 4)), y + (j * (height / 4)), (width / 4), (height / 4)));
              
            //    }
            //}
            
            //for (int a = 0; a < castleRooms.Count; a++)
            //{
            //    Rectangle r =castleRooms[a];
            //    Tile[,] t = Room.GetRoom(random.Next(1, 4));
            //    for (int b = r.X; b <= r.Width + r.X; b++)
            //    {
            //        for (int c = r.Y; c <= r.Height + r.Y; c++)
            //        {
            //                if (level.tiles[b, c].Foreground == Item.GrayBrickBG || b == x + width - 1)
            //                {
            //                    continue;
            //                }
            //                if ((b == r.Width + r.X || c == r.Y || c == r.Height + r.Y))
            //                {
            //                    level.tiles[b, c] = new Tile(Item.GrayBrick);

            //                }
            //                else if (b != r.X)
            //                {
            //                    level.tiles[b, c] = new Tile(Item.GrayBrickBG);
            //                }

            //                //level.tiles[b, c] = t[b - r.X, c - r.Y];
            //        }
            //    }
            //}
        }

      
    }
    public class Room
    {
        public static Tile[,] Room1 = new Tile[15, 15];
        public static Tile[,] Room2 = new Tile[15, 15];
        public static Tile[,] Room3 = new Tile[15, 15];
        public static Tile[,] Room4 = new Tile[15, 15];
        public static void Init()
        {
            for (int x = 0; x <= 10; x++) //Now add the items
            {
                for (int y = 0; y <= 10; y++)
                {
                    Room2[x, y] = new Tile(Item.Torch);
                    Room3[x, y] = new Tile(Item.Sand);
                    Room4[x, y] = new Tile(Item.Snow);
                }
            }
        }
        public static Tile[,] GetRoom(int i)
        {
            if (i == 1)
                return Room1;
            else if (i == 2)
                return Room2;
            else if (i == 3)
                return Room3;
            else
                return Room4;
        }
    }
    

}
                       