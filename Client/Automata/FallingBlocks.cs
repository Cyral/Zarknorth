using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public class FallingBlocks
    {
        private double lastUpdate;
        private const int updateFrequency = 0;
        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Manages falling blocks like sand and gravel.
        /// </summary>
        public FallingBlocks(Level level)
        {
            this.level = level;
        }

        public void Update(GameTime gameTime)
        {
            //If it is time yet to do a tick
            Tile currentTile;
            FallingTile fallingTile;
            if (gameTime.TotalGameTime.TotalMilliseconds > lastUpdate + updateFrequency)
            {
                lastUpdate = gameTime.TotalGameTime.TotalMilliseconds;
                for (int x = Math.Max(1,level.MainCamera.Left - 10); x < Math.Min(level.Width - 2,level.MainCamera.Right + 10); x++)
                {
                    for (int y = Math.Max(1, level.MainCamera.Top - 10); y < Math.Min(level.Height - 2, level.MainCamera.Bottom + 10); y++)
                    {
                        currentTile = level.tiles[x,y];
                        if (currentTile.Foreground.CanFall)
                        {
                            fallingTile = currentTile as FallingTile;
                            if (Vector2.Distance(new Vector2(x, y), level.Player.Position / Tile.Width) < 5)
                                level.tiles[x, y].NoCollide = level.Player.IsOverlappingPlayer(x, y);
                            //If it's not null, the tile is in the state : FALLING
                            if (fallingTile != null)
                            {
                                fallingTile.Update(gameTime);
                                if (fallingTile.Position == Tile.Height)
                                {
                                    level.tiles[x, y + 1].Foreground = currentTile.Foreground;
                                    BlockItem b = currentTile.Background;
                                    bool nc = currentTile.NoCollide;
                                    level.tiles[x, y] = new Tile(Item.Blank) { Background = b, NoCollide = nc };
                                    Tile next = level.tiles[x, y + 1];

                                    if ((next.Foreground.ID == Item.Blank.ID || level.tiles[x, y + 1] is FallingTile) && !next.Foreground.BreakFall)
                                    {
                                        level.tiles[x, y] = new FallingTile(currentTile.Foreground) {Background = currentTile.Background, ForegroundVariation = currentTile.ForegroundVariation, NoCollide = currentTile.NoCollide };
                                    }
                                    else if (level.tiles[x, y + 2].Foreground.BreakFall)
                                    {
                                        level.tiles[x, y + 1].NoCollide = level.Player.IsOverlappingPlayer(x, y + 1);
                                        currentTile.Foreground.OnDrop(new DropBlockEventArgs(level, x, y + 1, next.Foreground));
                                    }
                                }
                            }

                            //Else it is in the state : STEADY, so we need to check if it can fall
                            else
                            {
                                //If the tile below it is blank, check if it can fall below
                                if (level.tiles[x, y + 1].Foreground.ID == Item.Blank.ID || level.tiles[x, y + 1] is FallingTile)
                                {
                                    level.tiles[x, y] = new FallingTile(currentTile.Foreground) { Background = currentTile.Background, ForegroundVariation = currentTile.ForegroundVariation, BackgroundVariation = currentTile.BackgroundVariation };
                                }
                                else if (level.tiles[x, y + 1].Foreground.BreakFall)
                                {
                                    currentTile.Foreground.OnDrop(new DropBlockEventArgs(level, x, y , level.tiles[x, y].Foreground));
                                }
                            }

                        }
                            
                    }
                }
            }
        }
    }
}
