using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    /// <summary>
    /// File for handling sprite sheet operations in the level
    /// </summary>
    public partial class Level : IDisposable
    {
        private bool[] spriteSheetSides = new bool[4];
        /// <summary>
        /// Gets the position of a tile side within a spritesheet
        /// </summary>
        /// <param name="x">Tile position X</param>
        /// <param name="y">Tile position Y</param>
        /// <param name="sscp">Kind of block (eg FG, BG, etc)</param>
        /// <returns>The rectangle that should be drawn</returns>
        public Rectangle GetSpriteSheetPositions(int x, int y, SpriteSheetCompareProperties sscp, bool ClearOnly = false, Item blendWith = null, bool Border = false, bool recurse = true, bool check = true, bool skip = false)
        {
            if (!Game.TileEdges)
                return TileSetType.Center;

            Tile t = tiles[x, y, true];
            if (check && !skip)
            {
                if (sscp == SpriteSheetCompareProperties.Foreground)
                    if (t.ForegroundSheet != Rectangle.Empty)
                        return t.ForegroundSheet;
                else if (sscp == SpriteSheetCompareProperties.Background)
                    if (t.BackgroundSheet != Rectangle.Empty)
                        return t.BackgroundSheet;
            }

            spriteSheetBooleans = GetSpriteSheetArray(x, y, sscp, ClearOnly, blendWith, Border); //Get the array of bools for different sides

            int boolPosition = BooleansToInt(spriteSheetBooleans);
            Rectangle rect = IntToRectangle(boolPosition);
            if (sscp == SpriteSheetCompareProperties.Foreground && tiles[x, y, true].Foreground.EdgeMode == BlockEdgeMode.Stick && boolPosition == 15)
                rect = TileSetType.Single;
            if (check && !skip)
            {
                if (sscp == SpriteSheetCompareProperties.Foreground)
                {
                    t.ForegroundSheet = rect;
                    if (recurse)
                        RecalcSurrounding(x, y, sscp, ClearOnly, Border);
                }
                else if (sscp == SpriteSheetCompareProperties.Background)
                {
                    t.BackgroundSheet = rect;
                    if (recurse)
                        RecalcSurrounding(x, y, sscp, ClearOnly, Border);
                }
            }

            return rect;
        }

        private void RecalcSurrounding(int x, int y, SpriteSheetCompareProperties sscp, bool ClearOnly, bool Border)
        {
            if (sscp == SpriteSheetCompareProperties.Foreground)
            {
                tiles[x + 1, y, true].ForegroundSheet = GetSpriteSheetPositions(x + 1, y, sscp, ClearOnly, tiles[x + 1, y, true].Foreground.SmoothBlend, Border, false, true, true);
                tiles[x - 1, y, true].ForegroundSheet = GetSpriteSheetPositions(x - 1, y, sscp, ClearOnly, tiles[x - 1, y, true].Foreground.SmoothBlend, Border, false, true, true);
                if (y != Height - 1 && y!= Height - 2)
                tiles[x, y + 1, true].ForegroundSheet = GetSpriteSheetPositions(x, y + 1, sscp, ClearOnly, tiles[x, y + 1, true].Foreground.SmoothBlend, Border, false, true, true);
                tiles[x, y - 1, true].ForegroundSheet = GetSpriteSheetPositions(x, y - 1, sscp, ClearOnly, tiles[x, y - 1, true].Foreground.SmoothBlend, Border, false, true, true);
            }
            else if (sscp == SpriteSheetCompareProperties.Background)
            {
                tiles[x + 1, y, true].BackgroundSheet = GetSpriteSheetPositions(x + 1, y, sscp, ClearOnly, tiles[x + 1, y, true].Background.SmoothBlend, Border, false, true, true);
                tiles[x - 1, y, true].BackgroundSheet = GetSpriteSheetPositions(x - 1, y, sscp, ClearOnly, tiles[x - 1, y, true].Background.SmoothBlend, Border, false, true, true);
                if (y != Height - 1 && y != Height - 2)
                tiles[x, y + 1, true].BackgroundSheet = GetSpriteSheetPositions(x, y + 1, sscp, ClearOnly, tiles[x, y + 1, true].Background.SmoothBlend, Border, false, true, true);
                tiles[x, y - 1, true].BackgroundSheet = GetSpriteSheetPositions(x, y - 1, sscp, ClearOnly, tiles[x, y - 1, true].Background.SmoothBlend, Border, false, true, true);
            }
        }

        private static Rectangle IntToRectangle(int boolPosition)
        {
            switch (boolPosition)
            {
                //Check for the most common block, a center
                case 15: return TileSetType.Center;
                //Check for the side blocks
                case 14: return TileSetType.North;
                case 13: return TileSetType.East;
                case 11: return TileSetType.South;
                case 7: return TileSetType.West;
                //The corner blocks
                case 12: return TileSetType.NorthEast;
                case 6: return TileSetType.NorthWest;
                case 9: return TileSetType.SouthEast;
                case 3: return TileSetType.SouthWest;
                //And the ending "stub" blocks
                case 4: return TileSetType.NorthStub;
                case 8: return TileSetType.EastStub;
                case 1: return TileSetType.SouthStub;
                case 2: return TileSetType.WestStub;
                //And the last few remaining ones
                case 10: return TileSetType.Horizontal;
                case 5: return TileSetType.Vertical;
                case 0: return TileSetType.Single;
                default: return Rectangle.Empty;

                #region Discussion/How it works
                /* TODO: How to add more sides in the future (Note to stupid self) 
                   FER22F COMMENTARY: Much simpler! Just add one more "bit" and do this method:
                    
                So exception[0] is north, it will be true if the tile to the north of the block is the same as the current block
                exception[1] is east, 2 is south, 3 is west.
                    
                So imagine this statement for a center tile
                    
                1  +     2  +     4  +    8    = 15
                if (exception[0] && exception[1] && exception[2] && exception[3])
                    return TileSetType.Center;
                        
                And how about a west tile?
                    
                1   +   2    +   4  +    0   =7
                if (exception[0] && exception[1] && exception[2] && !exception[3])
                     return TileSetType.West;
                         
                So basicly, adding 1,2,4,8 and leaving 0 for ! will give you the binary value you need
                */

                /* TODO: Note to Fer22f:
                Make this WAY more simpler by using more logic (interacting directly with graphics multiply)
                NOTE TO SELF: You probably will lost some weeks trying to make an algorythm for that
                */
                #endregion
            }
        }

        /// <summary>
        /// Checks for each side and returns a 4 bool array indicating if the tile in that direction is the same
        /// </summary>
        private bool[] GetSpriteSheetArray(int x, int y, SpriteSheetCompareProperties sscp, bool ClearOnly = false,Item blendWith = null,bool Border = false)
        {
            //Okay, Now we get the 4 tiles surrounding this and return true if they are the same and we SHOULDNT put a border
            spriteSheetSides[0] = GetSpriteSheetCompare(x, y - 1, x, y, sscp, ClearOnly,blendWith,Border); //North     
            spriteSheetSides[1] = GetSpriteSheetCompare(x + 1, y, x, y, sscp, ClearOnly,blendWith,Border); //East
            spriteSheetSides[2] = GetSpriteSheetCompare(x, y + 1, x, y, sscp, ClearOnly,blendWith,Border); //South
            spriteSheetSides[3] = GetSpriteSheetCompare(x - 1, y, x, y, sscp, ClearOnly,blendWith,Border); //West
            return spriteSheetSides;
        }
        /// <summary>
        /// Actual comparison logic for the spritesheet sides
        /// </summary>
        /// <param name="compareToX">Tile X position to compare to</param>
        /// <param name="compareToY">Tile Y position to compare to</param>
        /// <param name="baseX">Base (Current) X position</param>
        /// <param name="baseY">Base (Current) Y position</param>
        /// <param name="sscp">Type of block (eg; Foreground, BG)</param>
        /// <returns>True if the tile to compare to is the same, false if otherwise</returns>
        private bool GetSpriteSheetCompare(int compareToX, int compareToY, int baseX, int baseY, SpriteSheetCompareProperties sscp, bool ClearOnly = false, Item blendWith = null,bool Border = false)
        {
            if (sscp == SpriteSheetCompareProperties.Foreground)
            {
                //Special case for sticky blocks like torches (They "stick" to solid blocks)
                if (tiles[baseX, baseY].Foreground.EdgeMode == BlockEdgeMode.Stick)
                    return tiles[compareToX, compareToY, true].Foreground.Collision != BlockCollision.Impassable;
                else
                {
                    //Some tiles may have tiles to blend with, like the borders between grass and dirt are not shown
                    if (tiles[compareToX, compareToY].Foreground.BlendEdge != null)
                        foreach (BlockItem block in tiles[compareToX, compareToY].Foreground.BlendEdge)
                            if (tiles[baseX, baseY, true].Foreground.ID == block.ID)
                                return true;
                    if (!Border)
                    {
                        if (!ClearOnly)
                        {

                            if (tiles[baseX, baseY, true].Foreground.EdgeWidth > 0)
                                if (tiles[baseX, baseY, true].Foreground.ID == tiles[compareToX, compareToY, true].Foreground.ID)
                                    if (((baseX + 1 + (baseY % 2)) % tiles[baseX, baseY, true].Foreground.EdgeWidth == 0 && compareToX - baseX == 1) || ((baseX + (baseY % 2)) % tiles[baseX, baseY, true].Foreground.EdgeWidth == 0 && compareToX - baseX == -1))
                                        return false;
                            //For all other tiles, if the tile to compare to is the same, return true
                            return tiles[baseX, baseY, true].Foreground.ID == tiles[compareToX, compareToY, true].Foreground.ID;
                        }
                        else
                        {
                            if (blendWith.ID == Item.Dirt.ID)
                                return (blendWith.ID != tiles[baseX, baseY, true].Foreground.ID || blendWith.ID != Item.Grass.ID) && !tiles[compareToX, compareToY, true].Foreground.Clear;

                            //For all other tiles, if the tile to compare to is the same, return true
                            return blendWith.ID != tiles[baseX, baseY, true].Foreground.ID && !tiles[compareToX, compareToY, true].Foreground.Clear;
                        }
                    }
                    else
                    {
                        if (tiles[compareToX, compareToY, true].Foreground.ID == Item.Grass.ID)
                            return tiles[baseX, baseY, true].Foreground.ID == Item.Dirt.ID || tiles[baseX, baseY, true].Foreground.SmoothBlend.ID == Item.Dirt.ID || (Item.Dirt.SmoothBlend != null && Item.Dirt.SmoothBlend.ID == tiles[compareToX, compareToY, true].Foreground.SmoothBlend.ID);
                        return tiles[baseX, baseY, true].Foreground.ID == tiles[compareToX, compareToY, true].Foreground.ID || (tiles[baseX, baseY, true].Foreground.SmoothBlend != null && tiles[baseX, baseY, true].Foreground.SmoothBlend.ID == tiles[compareToX, compareToY, true].Foreground.ID) || (tiles[compareToX, compareToY, true].Foreground.SmoothBlend != null && tiles[baseX, baseY, true].Foreground.SmoothBlend.ID == tiles[compareToX, compareToY, true].Foreground.SmoothBlend.ID);
                    }
                }
            }
            else if (sscp == SpriteSheetCompareProperties.Background)
            {
               
                {
                    //Some tiles may have tiles to blend with, like the borders between grass and dirt are not shown
                    if (tiles[compareToX, compareToY].Background.BlendEdge != null)
                        foreach (BlockItem block in tiles[compareToX, compareToY].Background.BlendEdge)
                            if (tiles[baseX, baseY, true].Background.ID == block.ID)
                                return true;
                    if (!Border)
                    {
                        if (!ClearOnly)
                        {

                            if (tiles[baseX, baseY, true].Background.EdgeWidth > 0)
                                if (tiles[baseX, baseY, true].Background.ID == tiles[compareToX, compareToY, true].Background.ID)
                                    if (((baseX + 1 + (baseY % 2)) % tiles[baseX, baseY, true].Background.EdgeWidth == 0 && compareToX - baseX == 1) || ((baseX + (baseY % 2)) % tiles[baseX, baseY, true].Background.EdgeWidth == 0 && compareToX - baseX == -1))
                                        return false;
                            //For all other tiles, if the tile to compare to is the same, return true
                            return tiles[baseX, baseY, true].Background.ID == tiles[compareToX, compareToY, true].Background.ID;
                        }
                        else
                        {
                            //For all other tiles, if the tile to compare to is the same, return true
                            return blendWith.ID != tiles[baseX, baseY, true].Background.ID && !tiles[compareToX, compareToY, true].Background.Clear;
                        }
                    }
                    else
                    {
                        return tiles[baseX, baseY, true].Background.ID == tiles[compareToX, compareToY, true].Background.ID || (tiles[baseX, baseY, true].Background.SmoothBlend != null && tiles[baseX, baseY, true].Background.SmoothBlend.ID == tiles[compareToX, compareToY, true].Background.ID) || (tiles[compareToX, compareToY, true].Background.SmoothBlend != null && tiles[baseX, baseY, true].Background.SmoothBlend.ID == tiles[compareToX, compareToY, true].Background.SmoothBlend.ID);
                    }
                }
            }
            else if (sscp == SpriteSheetCompareProperties.Lava)
            {
                return (tiles[baseX, baseY].LavaMass > 0 && tiles[compareToX, compareToY].LavaMass > 128);
            }
            else if (sscp == SpriteSheetCompareProperties.Water)
            {
                return (tiles[baseX, baseY].WaterMass > 0 && tiles[compareToX, compareToY].WaterMass > 200);
            }
            else if (sscp == SpriteSheetCompareProperties.FullBG)
            {
                return tiles[baseX, baseY, true].FullBackground == tiles[compareToX, compareToY, true].FullBackground;
            }
            else
            {
                return false; //This should never happen
            }

        }
        /// <summary>
        /// Checks for each side and returns a 4 bool array indicating if the tile in that direction is the same
        /// </summary>
        private bool GetSmoothBlend(int x, int y, BlockItem blendWith)
        {
            if (!Game.TileEdges)
                return false;
            SpriteSheetCompareProperties sscp = blendWith is ForegroundBlockItem ? SpriteSheetCompareProperties.Foreground : SpriteSheetCompareProperties.Background;
            //Okay, Now we get the 4 tiles surrounding this and return true if they are the same and we SHOULDNT put a border
            spriteSheetSides[0] = GetSmoothSpriteSheetCompare(x, y - 1, x, y, sscp, blendWith); //North     
            spriteSheetSides[1] = GetSmoothSpriteSheetCompare(x + 1, y, x, y, sscp, blendWith); //East
            spriteSheetSides[2] = GetSmoothSpriteSheetCompare(x, y + 1, x, y, sscp, blendWith); //South
            spriteSheetSides[3] = GetSmoothSpriteSheetCompare(x - 1, y, x, y, sscp, blendWith); //West
            return spriteSheetSides.Any(o => o);
        }
        private bool GetSmoothSpriteSheetCompare(int compareToX, int compareToY, int baseX, int baseY, SpriteSheetCompareProperties sscp, Item blendWith)
        {
            if (sscp == SpriteSheetCompareProperties.Foreground)
            {
                if (blendWith.ID == Item.Dirt.ID)
                    return (tiles[compareToX, compareToY, true].Foreground.ID == Item.Grass.ID || blendWith.ID == tiles[compareToX, compareToY, true].Foreground.ID);

                else
                    return blendWith.ID == tiles[compareToX, compareToY, true].Foreground.ID;
            }
            else if (sscp == SpriteSheetCompareProperties.Background)
                return blendWith.ID == tiles[compareToX, compareToY, true].Background.ID;
            else
                return false;
        }
        /// <summary>
        /// Converts a series of bits into a unique int value
        /// </summary>
        private int BooleansToInt(bool[] bits)
        {
            int r = 0;
            for (int i = 0; i < bits.Length; i++)
                if (bits[i])
                    r |= 1 << i;
            return r;
        }
        private bool[] spriteSheetBooleans;
    }
    /// <summary>
    /// Defines what kind of spritesheet is being used, and how it should be compared
    /// </summary>
    public enum SpriteSheetCompareProperties
    {
        Foreground,
        Background,
        Water,
        Lava,
        FullBG,
    }

    /// <summary>
    /// Defins the rectangles for different tile sides
    /// </summary>
    public static class TileSetType
    {
        public static Rectangle[] Rectangles;
        static TileSetType()
        {
            Rectangles = new Rectangle[]
            {
                Rectangle.Empty,
                Center,
                Single,
                East,
                North,
                West,
                South,
                NorthStub,
                EastStub,
                SouthStub,
                WestStub,
                Vertical,
                Horizontal,
                NorthEast,
                NorthWest,
                SouthEast,
                SouthWest,
            };
        }
        private static int h = Tile.Height;
        private static int w = Tile.Width;

        //Basic sides
        public static Rectangle Center = new Rectangle(h, w, w, h);
        public static Rectangle Single = new Rectangle(3 * w, 3 * h, w, h);  

        //Flat directional sides
        public static Rectangle East = new Rectangle(w * 2, h, w, h);
        public static Rectangle North = new Rectangle(w, 0, w, h);
        public static Rectangle West = new Rectangle(0, h, w, h);
        public static Rectangle South = new Rectangle(w, h * 2, w, h);

        //Ending "Stub" sides
        public static Rectangle NorthStub = new Rectangle(w * 3, 0, w, h);
        public static Rectangle EastStub = new Rectangle(2 * w, 3 * h, w, h);
        public static Rectangle SouthStub = new Rectangle(w * 3, h * 2, w, h);
        public static Rectangle WestStub = new Rectangle(0, 3 * h, w, h);

        //Vert/Horz sides
        public static Rectangle Vertical = new Rectangle(w * 3, h, w, h);
        public static Rectangle Horizontal = new Rectangle(w, h * 3, w, h);

        //Corner sides
        public static Rectangle NorthEast = new Rectangle(w * 2, 0, w, h);
        public static Rectangle NorthWest = new Rectangle(0, 0, w, h);
        public static Rectangle SouthEast = new Rectangle(w * 2, h * 2, w, h);
        public static Rectangle SouthWest = new Rectangle(0, h * 2, w, h);
    }
}
