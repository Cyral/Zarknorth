using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    public partial class WorldGen
    {
        void Castle(int percent, int height, int start)
        {
            //for (int x = 1; x < level.Width - 40; x++)
            //{
            //    if (x == start)
            //    {
            //        int Height = 60; //height; //How tall to make it (In blocks)
            //        int Length = 10;
            //        int lengthSoFar = 0; //How long it is so far(In Blocks)
            //        int towerWidth = 10;
            //        //Create outer towers
            //        CastleTower(x - 25, x, Height - 1, Length, lengthSoFar);
            //        CastleTower(x + 25, x, Height - 1, Length, lengthSoFar);
            //        //Connect them
            //        Line(new Vector2(x - 25 , terrainContour[x]), new Vector2(x + 25 + towerWidth, terrainContour[x]), Item.GrayBrick);
            //        //room.CastleRooms(x - 15, terrainContour[x] - (int)(40), 40, (int)(40));
            //        //fill in stuff under house
            //        for (int a = 0; a <= lengthSoFar; a++) //Fill foundation, Loop through each floor tiles, and go down 
            //        {
            //            if (terrainContour[x] < terrainContour[x + a])
            //            {
            //                for (int b = 0; b <= 20; b++) //Go down to terrain countour 
            //                {
            //                    //Stop if we hit a the terrain line
            //                    if (terrainContour[x] + b == terrainContour[x + a])
            //                        break;
            //                    else
            //                        level.tiles[(int)x + a, terrainContour[x] + b] = new Tile(Item.GrayBrick);
            //                }
            //            }


            //        }
            //    }
            //}
        }

        private void CastleTower(int x, int baseX, int Height, int Length, int lengthSoFar)
        {
        //    //Get base
        //    int y = terrainContour[baseX];

        //    for (int i = 0; i <= Height + 1; i++) //For Height... Go btnUp and make walls/Add stuff
        //    {
        //        //Add first wall
        //        level.tiles[x + (lengthSoFar), y - i] = new Tile(Item.GrayBrick);
        //        //Fill with Background, and add floor 
        //        for (int j = 1; j <= Length; j++)
        //            level.tiles[x + j, y - i] = new Tile(Item.Blank) { background = Item.GrayBrickBG };//BG
        //    }
        //    for (int i = 0; i <= Height + 1; i++) //For Height... Go btnUp and make walls/Add stuff
        //    {
        //        for (int j = 1; j <= Length; j++)
        //        {

        //            if (IsDivisble(i, 10) && IsDivisble(i, 20) == false)
        //            {
        //                if (j < Length - 4)
        //                    level.tiles[x + j, y - i] = new Tile(Item.GrayBrick);//Floor
        //                else
        //                    level.tiles[x + j, y - i] = new Tile(Item.WoodPlatform) { background = Item.GrayBrickBG };//Floor
        //                if (i < Height)
        //                    Line(new Vector2(x + 1, y - (i + 10) + 1), new Vector2(x + Length, y - i ), Item.WoodPlatform, true);
        //            }
        //            else if (IsDivisble(i, 20))
        //            {
        //                if (j > Length - 6)
        //                    level.tiles[x + j, y - i] = new Tile(Item.GrayBrick);//Floor
        //                else
        //                    level.tiles[x + j, y - i] = new Tile(Item.WoodPlatform) { background = Item.GrayBrickBG };//Floor
        //                if (i < Height)
        //                    Line(new Vector2(x + Length + 1, y - (i + 10) ), new Vector2(x+1 , y - i ), Item.WoodPlatform, true);
        //            }
        //        }
        //        if (IsDivisble(i, 10))
        //        {
        //            level.tiles[x + 1, y - i + 3] = new Tile(Item.Torch) { background = Item.GrayBrickBG };
        //            level.tiles[x + 9, y - i + 3] = new Tile(Item.Torch) { background = Item.GrayBrickBG };
                   
        //        }
        //    }
        //    //Add Doors
        //    for (int i = 0; i <= Height ; i++) 
        //    {
        //        if (IsDivisble(i, 10))
        //        {

        //            level.tiles[x, y - i - 3] = new Tile(Item.Door, x, y - i - 3) { background = Item.GrayBrickBG };
        //        }
        //    }
        //    //Add door
        //    level.tiles[x + (lengthSoFar), y - 3] = new Tile(Item.Door, x + (lengthSoFar), y - 3) { background = Item.GrayBrickBG };

        //    for (int i = 0; i <= Height; i++) //Add last wall
        //    {
        //        //Add end wall
        //        level.tiles[x + Length, y - i] = new Tile(Item.GrayBrick);
               

        //    }
        }
    }
}