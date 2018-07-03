using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public class Fire
    {
	    public Level Level { get { return level; } }
        public List<Point> ActiveFires { get; set; }

        private Level level;
        private double lastUpdate;
        private const int updateFrequency = 300;

        /// <summary>
        /// Manages burning and fire spread (e.g. fire can burn trees and wood)
        /// </summary>
        public Fire(Level level)
        {
            this.level = level;
            ActiveFires = new List<Point>();
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds > lastUpdate + updateFrequency)
            {
                lastUpdate = gameTime.TotalGameTime.TotalMilliseconds;
                List<Point> List = ActiveFires.ToList();
                for (int i = 0; i < List.Count; i++)
                {
                    Point p = List[i];
                    int x = p.X;
                    int y = p.Y;
                    Tile currentTile = level.tiles[x, y, true];

                    if (currentTile.ForegroundFireMeta == 10)
                        currentTile.ForegroundFireMeta = 0;
                    if (currentTile.BackgroundFireMeta == 10)
                        currentTile.BackgroundFireMeta = 0;

                    if (currentTile.ForegroundFireMeta == 0 && currentTile.BackgroundFireMeta == 0)
                        continue;
                    //Check if the fire is burning, if so, move it to the next stage
                    if (currentTile.ForegroundFireMeta > 0 && currentTile.ForegroundFireMeta < 10)
                        currentTile.ForegroundFireMeta++;
                    //If it is at stage 10 and ready to burn out
                    if (currentTile.Foreground.Burnable || currentTile.ForegroundFireMeta == 5)
                    {

                        //Spread to nearby tiles IF it is at the last stage (10) or a 1/2 chance it if is in the middle stage
                        if (currentTile.ForegroundFireMeta == 10 || (currentTile.ForegroundFireMeta == 5 && level.random.Next(0, 2) == 0))
                        {
                            Point direction = GetDirection(x, y, false);
                            if (direction != Point.Zero)
                            {
                                level.tiles[x + direction.X, y + direction.Y, true].ForegroundFireMeta = 1;
                                if (level.tiles[x + direction.X, y + direction.Y, true].BackgroundFireMeta == 0 && level.tiles[x + direction.X, y + direction.Y, true].Background.Burnable)
                                    level.tiles[x + direction.X, y + direction.Y, true].BackgroundFireMeta = 1;
                            }
                        }
                        if (currentTile.ForegroundFireMeta == 10)
                        {
                            level.tiles[x, y, true].Foreground = Item.Blank;
                            level.tiles[x, y, true].ForegroundFireMeta = 0;
                        }
                    }

                    if (currentTile.BackgroundFireMeta > 0 && currentTile.BackgroundFireMeta < 10)
                        currentTile.BackgroundFireMeta++;
                    //If it is at stage 10 and ready to burn out
                    if (currentTile.Background.Burnable || currentTile.BackgroundFireMeta == 5)
                    {
                        //Do the same for backgrounds
                        if (currentTile.BackgroundFireMeta == 10 || (currentTile.BackgroundFireMeta == 5 && level.random.Next(0, 2) == 0))
                        {
                            Point direction = GetDirection(x, y, true);
                            if (direction != Point.Zero)
                            {
                                level.tiles[x + direction.X, y + direction.Y, true].BackgroundFireMeta = 1;
                                if (level.tiles[x + direction.X, y + direction.Y, true].ForegroundFireMeta == 0 && level.tiles[x + direction.X, y + direction.Y, true].Foreground.Burnable)
                                    level.tiles[x + direction.X, y + direction.Y, true].ForegroundFireMeta = 1;
                            }
                        }
                        if (currentTile.BackgroundFireMeta == 10)
                        {
                            level.tiles[x, y, true].Background = Item.Blank;
                            level.tiles[x, y, true].BackgroundFireMeta = 0;
                        }
                        continue;
                    }
                }
            }
        }
		/// <summary>
		/// Gets a random direction for the fire to spread to, and checks if it can spread to that block
		/// </summary>
		/// <param name="x">Current X position</param>
		/// <param name="y">Current Y position</param>
		/// <param name="background">If the current tile is a background</param>
		/// <returns>A point defining which direction fire should spread to, returns Point.Zero if the fire can't spread/should die</returns>
        private Point GetDirection(int x, int y, bool background)
        {
            Point direction = Point.Zero;
			//Give it 6 tries to find a direction
            for (int i = 0; i < 6; i++)
            {
                direction = TryDirection(x, y, background, direction);
                if (direction == Point.Zero)
                    direction = TryDirection(x, y, background, direction);
                else
                    break;
            }
            return direction;
        }

        private Point TryDirection(int x, int y, bool background, Point direction)
        {
            int directionCanidate = level.random.Next(0, 4);
            switch (directionCanidate)
            {
                case 0:
                    if (background ? level.tiles[x + 1, y, true].Background.Burnable : level.tiles[x + 1, y, true].Foreground.Burnable)
                        direction.X = 1;
                    break;
                case 1:
                    if (background ? level.tiles[x - 1, y, true].Background.Burnable : level.tiles[x - 1, y, true].Foreground.Burnable)
                        direction.X = -1;
                    break;
                case 2:
                    if (background ? level.tiles[x, y + 1, true].Background.Burnable : level.tiles[x, y + 1, true].Foreground.Burnable)
                        direction.Y = +1;
                    break;
                case 3:
                    if (background ? level.tiles[x, y - 1, true].Background.Burnable : level.tiles[x, y - 1, true].Foreground.Burnable)
                        direction.Y = -1;
                    break;
            }
            return direction;
        }
    }
}
