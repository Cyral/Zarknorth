using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    public class Liquid
    {
        /// <summary>
        /// Reference to level
        /// </summary>
        private Level level;

        /// <summary>
        /// List of active water tile positions, only tiles on level.tiles list are updated
        /// </summary>
        public List<Point> ActiveWaterTiles = new List<Point>();
        /// <summary>
        /// List of active lava tile positions, only tiles on level.tiles list are updated
        /// </summary>
        public List<Point> ActiveLavaTiles = new List<Point>();
        
        /// <summary>
        /// Area around the viewport in which water will no longer be updated
        /// </summary>
        private const int offset = 60;
        private bool settling;

        public Liquid(Level level)
        {
            this.level = level;
        }

        public int getLiquid(int x, int y)
        {
            return level.tiles[x, y, true].WaterMass;
        }

        public void setLiquid(int x, int y, int value)
        {
            level.tiles[x, y, true].WaterMass = value;
        }
        public void SettleLiquids(int iterations = 1000)
        {
            settling = true;
            for (int times = 0; times < iterations; times++)
            {
                if (times == 0)
                {
                    WaterSettleDown();
                    LavaSettleDown();
                }
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Settling Liquids... " + times + "/" + iterations, (((float)times / iterations) * .03f) + .97f);
                //UpdateWater();
                //UpdateLava();
            }
            settling = false;
        }

        private void LavaSettleDown()
        {
            ActiveLavaTiles.Sort((a, b) =>
            {
                int result = a.Y.CompareTo(b.Y);
                if (result == 0) result = a.X.CompareTo(b.X);
                return result;
            });
            int ac = ActiveLavaTiles.Count;
            //Loop through each water tile
            for (int i = ActiveLavaTiles.Count - 1; i >= 0; i--)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Settling Liquids... ", 0 + .97f);
                //Get the X and Y components from the point
                int x = ActiveLavaTiles[i].X;
                int y = ActiveLavaTiles[i].Y;
                if (y >= level.Height - 2)
                {
                    ActiveLavaTiles.RemoveAt(i);
                    continue;
                }
                int originy = y;
                while (y < level.Width - 1 && level.tiles[x, y + 1, true].Foreground.Collision.CanFlowThrough() && level.tiles[x, y + 1, true].LavaMass == 0)
                {
                    y++;
                }
                if (y < level.Width - 1)
                {
                    y--;
                    //Hold a few values just so we don't have to keep looking them up
                    int current = level.tiles[x, originy, true].LavaMass;
                    //Down Logic
                    int total = (current + level.tiles[x, y + 1, true].LavaMass); //Total liquid from current tile and below tile
                    level.tiles[x, y + 1, true].LavaMass = (int)MathHelper.Clamp(total, 0, 255); //Make sure bottom tile can only hold up to 255
                    if (total > 255)
                        level.tiles[x, originy, true].LavaMass = (total - 255); // If the calculation gived number > 255
                    else
                        level.tiles[x, originy, true].LavaMass = 0;
                }
            }
        }

        private void WaterSettleDown()
        {
            ActiveWaterTiles.Sort((a, b) =>
            {
                int result = a.Y.CompareTo(b.Y);
                if (result == 0) result = a.X.CompareTo(b.X);
                return result;
            });
            int ac = ActiveWaterTiles.Count;
            //Loop through each water tile
            for (int i = ActiveWaterTiles.Count - 1; i >= 0; i--)
            {
                ZarknorthClient.Interface.MainWindow.UpdateLoading("Settling Liquids... ",0 + .97f);
                //Get the X and Y components from the point
                int x = ActiveWaterTiles[i].X;
                int y = ActiveWaterTiles[i].Y;
                if (y >= level.Height - 2)
                {
                    ActiveWaterTiles.RemoveAt(i);
                    continue;
                }
                int originy = y;
                while (y < level.Width - 1 && level.tiles[x, y + 1, true].Foreground.Collision.CanFlowThrough() && level.tiles[x, y + 1, true].WaterMass == 0)
                {
                    y++;
                }
                if (y < level.Width - 1)
                {
                    y--;
                    //Hold a few values just so we don't have to keep looking them up
                    int current = level.tiles[x, originy, true].WaterMass;
                    //Down Logic
                    int total = (current + level.tiles[x, y + 1, true].WaterMass); //Total liquid from current tile and below tile
                    level.tiles[x, y + 1, true].WaterMass = (int)MathHelper.Clamp(total, 0, 255); //Make sure bottom tile can only hold up to 255
                    if (total > 255)
                        level.tiles[x, originy, true].WaterMass = (total - 255); // If the calculation gived number > 255
                    else
                        level.tiles[x, originy, true].WaterMass = 0;
                }
            }
        }
        public bool IsSettling()
        {
            return settling;
        }
        #region Water
        private void WaterLogic()
        {
            //Sort list so it goes from BOTTOM to TOP
            if (!settling)
            ActiveWaterTiles.Sort((a, b) =>
            {
                int result = a.Y.CompareTo(b.Y);
                if (result == 0) result = a.X.CompareTo(b.X);
                return result;
            });

                    //Loop through each water tile
                    for (int i = ActiveWaterTiles.Count - 1; i >= 0; i--)
                    {
                        //Get the X and Y components from the point
                        int x = ActiveWaterTiles[i].X;
                        int y = ActiveWaterTiles[i].Y;

                        if (y >= level.Height - 2)
                        {
                            ActiveWaterTiles.RemoveAt(i);
                            continue;
                        }
                        if (level.tiles[x, y, true].WaterIdle)
                            continue;
                        //If out of viewport + offset, do not update
                        if (!settling)
                            if (x <= 0 || y <= 0 || x < level.MainCamera.Left - offset || x > level.MainCamera.Right + offset || y < level.MainCamera.Top - offset || y > level.MainCamera.Bottom + offset)
                                continue;


                        //Hold a few values just so we don't have to keep looking them up
                        int right = level.tiles[x + 1, y, true].WaterMass;
                        int left = level.tiles[x - 1, y, true].WaterMass;
                        int current = level.tiles[x, y, true].WaterMass;
                        int original = current;
                        if (current == 0)
                        {
                            ActiveWaterTiles.RemoveAt(i);
                            continue;
                        }
                        //if (current == 255 && right == 255 && left == 255 && level.tiles[x, y + 1, true].WaterMass == 255 && level.tiles[x, y - 1, true].WaterMass == 255)
                        //    continue;

                        if (level.tiles[x, y + 1, true].Foreground.Collision.CanFlowThrough() && level.tiles[x, y + 1, true].WaterMass < 255)
                        {
                            //Down Logic
                            int total = (current + level.tiles[x, y + 1, true].WaterMass); //Total liquid from current tile and below tile
                            level.tiles[x, y + 1, true].WaterMass = (int)MathHelper.Clamp(total, 0, 255); //Make sure bottom tile can only hold up to 255
                            if (total > 255)
                                level.tiles[x, y, true].WaterMass = (total - 255); // If the calculation gived number > 255
                            else
                            {
                                //Move all of it out
                                level.tiles[x, y, true].WaterMass = 0;
                                continue; //Continue since we have nothing left
                            }
                        }

                        //Reset the current tiles amount
                        current = level.tiles[x, y, true].WaterMass;

                        //Self explanitory
                        bool rightPassable = level.tiles[x + 1, y, true].Foreground.Collision.CanFlowThrough() && right < 255;
                        bool leftPassable = level.tiles[x - 1, y, true].Foreground.Collision.CanFlowThrough() && left < 255;
                        bool twoSidesAreSame = (right == current) && (left == current);
                        bool sidesAreZero = (right == 0) || (left == 0);

                        //The 3 way situation :)
                        //If we can pass right and left and also our two sides aren't the same or our sides are zero
                        if (rightPassable && leftPassable && (!twoSidesAreSame || sidesAreZero))
                        {
                            // Supposed to be 42 but can be furthely processed into the sum of 3 tiles
                            // (Which may or may not equal 42) Nerds FTW! -Fer22f
                            int total = (current + right + left);

                            //Split the sum of all the tiles through each one, so they will all be equal. Add the remainder, if any, to the middle tile
                            int div = (total / 3);
                            level.tiles[x, y, true].WaterMass = div + (total % 3);
                            level.tiles[x + 1, y, true].WaterMass = div;
                            level.tiles[x - 1, y, true].WaterMass = div;
                        }
                        else if (rightPassable && !leftPassable) //If we can go right, but not left
                        {
                            int total = (right + current);

                            int div = (total / 2);
                            level.tiles[x + 1, y, true].WaterMass = div + (total & 1);
                            level.tiles[x, y, true].WaterMass = div;

                        }
                        else if (!rightPassable && leftPassable) // If we can go left, but not right
                        {
                            int total = (left + current);

                            int div = (total / 2);
                            level.tiles[x - 1, y, true].WaterMass = div + (total & 1);
                            level.tiles[x, y, true].WaterMass = div;
                        }
                        if (original == level.tiles[x, y, true].WaterMass)
                            level.tiles[x, y, true].WaterIdle = true;
                    }
        }

        #endregion
        #region Lava
        public void UpdateLava()
        {
            if (settling)
                lock (ActiveLavaTiles)
                    LavaLogic();
            else
                LavaLogic();
        }
        private void LavaLogic()
        {
            //Sort list so it goes from BOTTOM to TOP
            if (!settling)
            ActiveLavaTiles.Sort((a, b) =>
            {
                int result = a.Y.CompareTo(b.Y);
                if (result == 0) result = a.X.CompareTo(b.X);
                return result;
            });

            //Loop through each water tile
            for (int i = ActiveLavaTiles.Count - 1; i >= 0; i--)
            {
                //Get the X and Y components from the point
                int x = ActiveLavaTiles[i].X;
                int y = ActiveLavaTiles[i].Y;
                if (level.tiles[x, y, true].LavaIdle)
                    continue;
                if (y >= level.Height - 2)
                {
                    ActiveLavaTiles.RemoveAt(i);
                    continue;
                }
                //If out of viewport + offset, do not update
                if (!settling)
                    if (x <= 0 || y <= 0 || x < level.MainCamera.Left - offset || x > level.MainCamera.Right + offset || y < level.MainCamera.Top - offset || y > level.MainCamera.Bottom + offset)
                        continue;

                if (level.tiles[x, y, true].Foreground.Collision == BlockCollision.Impassable)
                {
                    level.tiles[x, y, true].LavaMass = 0;
                    continue;
                }

                //Hold a few values just so we don't have to keep looking them up
                int right = level.tiles[x + 1, y, true].LavaMass;
                int left = level.tiles[x - 1, y, true].LavaMass;
                int current = level.tiles[x, y, true].LavaMass;
                int original = current;
                //Obsidian
                if (current > 10 && level.tiles[x, y, true].WaterMass > 10)
                {
                    BlockItem bg = level.tiles[x, y, true].Background;
                    level.tiles[x, y] = new Tile(Item.Obsidian) { Background = bg };
                    level.tiles[x, y, true].WaterMass = 0;
                    level.tiles[x, y, true].LavaMass = 0;
                    continue;
                }

                //if (current == 255 && right == 255 && left == 255 && level.tiles[x, y + 1, true].LavaMass == 255 && level.tiles[x, y - 1, true].LavaMass == 255)
                //    continue;

                if (level.tiles[x, y + 1, true].Foreground.Collision.CanFlowThrough() && level.tiles[x, y + 1, true].LavaMass < 255)
                {
                    //Down Logic
                    int total = (current + level.tiles[x, y + 1, true].LavaMass); //Total liquid from current tile and below tile
                    level.tiles[x, y + 1, true].LavaMass = (int)MathHelper.Clamp(total, 0, 255); //Make sure bottom tile can only hold up to 255
                    if (total > 255)
                        level.tiles[x, y, true].LavaMass = (total - 255); // If the calculation gived number > 255
                    else
                    {
                        //Move all of it out
                        level.tiles[x, y, true].LavaMass = 0;
                        level.ComputeLighting = true;
                        continue; //Continue since we have nothing left
                    }
                }

                //Reset the current tiles amount
                current = level.tiles[x, y, true].LavaMass;

                //Self explanitory
                bool rightPassable = level.tiles[x + 1, y, true].Foreground.Collision.CanFlowThrough() && right < 255;
                bool leftPassable = level.tiles[x - 1, y, true].Foreground.Collision.CanFlowThrough() && left < 255;
                bool twoSidesAreSame = (right == current) && (left == current);
                bool sidesAreZero = (right == 0) || (left == 0);

                //The 3 way situation :)
                //If we can pass right and left and also our two sides aren't the same or our sides are zero
                if (rightPassable && leftPassable && (!twoSidesAreSame || sidesAreZero))
                {
                    // Supposed to be 42 but can be furthely processed into the sum of 3 tiles
                    // (Which may or may not equal 42) Nerds FTW! -Fer22f
                    int total = (current + right + left);

                    //Split the sum of all the tiles through each one, so they will all be equal. Add the remainder, if any, to the middle tile
                    int div = (total / 3);
                    level.tiles[x, y, true].LavaMass = div + (total % 3);
                    level.tiles[x + 1, y, true].LavaMass = div;
                    level.tiles[x - 1, y, true].LavaMass = div;
                }
                else if (rightPassable && !leftPassable) //If we can go right, but not left
                {
                    int total = (right + current);

                    int div = (total / 2);
                    level.tiles[x + 1, y, true].LavaMass = div + (total & 1);
                    level.tiles[x, y, true].LavaMass = div;

                }
                else if (!rightPassable && leftPassable) // If we can go left, but not right
                {
                    int total = (left + current);

                    int div = (total / 2);
                    level.tiles[x - 1, y, true].LavaMass = div + (total & 1);
                    level.tiles[x, y, true].LavaMass = div;
                }
                if (original == level.tiles[x, y, true].LavaMass)
                    level.tiles[x, y, true].LavaIdle = true;
            }
        }
        public void UpdateWater()
        {
            if (settling)
                lock (ActiveWaterTiles)
                    WaterLogic();
            else
                WaterLogic();
        }
        #endregion
    }
    public static class LiquidExtensions
    {
        public static bool CanFlowThrough(this BlockCollision collision)
        {
            return (collision != BlockCollision.Impassable && collision != BlockCollision.Falling);
        }
    }
}