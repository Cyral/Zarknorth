using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    /// <summary>
    /// Event handler for tile place events
    /// </summary>
    public delegate void TileSetEventHandler(object o, int x, int y);
    /// <summary>
    /// Wrapper class to set and get tiles, Using this method can let us see the X and Y coords and validate them
    /// </summary>
    public class TileWrapper
    {
        /// <summary>
        /// Width of the array
        /// </summary>
        public int Width;
        /// <summary>
        /// Height of the array
        /// </summary>
        public int Height;

        /// <summary>
        /// (Optional) level used for tiles and their logic
        /// </summary>
        public Level level { get; set; }

        private Tile[] tiles; //BackingStore

        /// <summary>
        /// Event handler for tile place events
        /// </summary>
        public event TileSetEventHandler SetTile;
        public void OnSetTile(int x, int y)
        {
            if (SetTile != null) SetTile(this,x,y);
        }

        public TileWrapper()
        {
            Width = Height = 0;
            tiles = new Tile[Width * Height];
        }

        public TileWrapper(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = new Tile[Width * Height];
            SetTile = null;
        }
        /// <summary>
        /// Accessor for tiles
        /// </summary>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <param name="overide">Bool to "override" the get, if true, it wont get the reference tile</param>
        public Tile this[int x, int y, bool overide = false]
        {
            get //When we want to GET a tile
            {
                //Convert position for looping worlds
                x = PerformTileRepeatLogic(x);
                int index = y * Width + x;
                // if (Game.level.Chunks[x / Chunk.Size, y / Chunk.Size].tiles[x % Chunk.Size, y % Chunk.Size] == null)
                if (!overide && tiles[index] != null && tiles[index].IsLarge) //If we dont want to overide, redirect the tile to the correct tile
                    return tiles[index].Reference; //redirect tile, for example, if we want to access the MainCamera.bottom of a chair tile, it will "redirect" us to the actual position it was placed
                else  //If not redirecting
                    return tiles[index];
            }
            set //When we want to SET a tile
            {
                //Convert position for looping worlds
                x = PerformTileRepeatLogic(x);
                int index = y * Width + x;
                //Send on set event
                OnSetTile(x, y);

                if (tiles[index] != null && !overide && tiles[index].IsLarge) //If not overide, redrect the tile to the actual tile and set that
                {
                    tiles[index].Reference = value;
                }
                else  //Set absolute tile
                {
                    value.Y = y;
                    value.X = x;
                    tiles[index] = value;
                }
            }
        }
        /// <summary>
        /// Sets the position of an object for repeating worlds, for example, the player.
        /// </summary>
        public Vector2 SetPositionRepeat(Vector2 position, bool setCamera)
        {
            if (setCamera && level != null)
            {
                if (position.X < 0)
                    level.MainCamera.position.X += level.Width * Tile.Width;
                else if (position.X > Width * Tile.Width)
                    level.MainCamera.position.X -= level.Width * Tile.Width;
            }
            if (position.X < 0)
                position.X = Width * Tile.Width;
            else if (position.X > Width * Tile.Width)
                position.X = 0;
            return position;
        }
        /// <summary>
        /// Compute the position based on the level width, used for looping worlds (Ex: -5 loops to 995 if Width is 1000, and 5 stays the same)
        /// </summary>
        public Vector2 PerformWorldRepeatLogic(Vector2 p)
        {
            p.X = PerformWorldRepeatLogic(p.X);
            return p;
        }
        /// <summary>
        /// Compute the X position based on the level width, used for looping worlds (Ex: -5 loops to 995 if Width is 1000, and 5 stays the same)
        /// </summary>
        public float PerformWorldRepeatLogic(float x)
        {
            if (x < 0)
            {
                x = x % (Width * Tile.Width);
                x = (Width * Tile.Width) + x;
            }
            else
                x = x % (Width * Tile.Width);
            return x;
        }
        /// <summary>
        /// Compute the X position based on the level width, used for looping worlds (Ex: -5 loops to 995 if Width is 1000, and 5 stays the same)
        /// </summary>
        public int PerformTileRepeatLogic(int x)
        {
            return x % Width + (x < 0 ? Width : 0);
        }
    }
}
