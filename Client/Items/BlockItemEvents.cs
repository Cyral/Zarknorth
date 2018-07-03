using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    /// <summary>
    /// Called when a block is destroyed, usually results in a pickup item being dropped
    /// </summary>
    public delegate void DropBlockEventHandler(object o, DropBlockEventArgs e);
    public class DropBlockEventArgs : EventArgs
    {
        public readonly Level Level;
        public readonly int X, Y;
        public readonly Item Item;
        public readonly bool SuppressDrop;

        public DropBlockEventArgs(Level level, int x, int y, Item item, bool suppressdrop = false)
        {
            this.X = x;
            this.Y = y;
            this.Level = level;
            this.Item = item;
            this.SuppressDrop = suppressdrop;
        }
    }

    /// <summary>
    /// Called when an item needs to be placed
    /// </summary>
    /// <returns>True if the item was placed (Might not if item is already there)</returns>
    public delegate bool PlaceBlockEventHandler(object o, PlaceBlockEventArgs e);
    public class PlaceBlockEventArgs : EventArgs
    {
        public readonly Level level;
        public readonly int x, y;
        public readonly Item item;
        public readonly GameTime gameTime;
        public readonly bool flip;

        public PlaceBlockEventArgs(Level level, int x, int y, Item item, bool flip, GameTime gameTime)
        {
            this.x = x;
            this.y = y;
            this.level = level;
            this.item = item;
            this.flip = flip;
            this.gameTime = gameTime;
        }
    }

    /// <summary>
    /// Called when an tile is right clicked, and interacted with. It may open a dialog for example
    /// </summary>
    public delegate void InteractBlockEventHandler(object o, InteractBlockEventArgs e);
    public class InteractBlockEventArgs : EventArgs
    {
        public readonly Level level;
        public readonly int x, y;
        public readonly GameTime gameTime;

        public InteractBlockEventArgs(Level level, int x, int y, GameTime gameTime = null)
        {
            this.x = x;
            this.y = y;
            this.level = level;
            this.gameTime = gameTime;
        }
    }
    /// <summary>
    /// Called when the output of an electronic tile is requested
    /// </summary>
    public delegate bool RequestElectronicOutputEventHandler(object o, RequestElectronicOutputEventArgs e);
    public class RequestElectronicOutputEventArgs : EventArgs
    {
        public readonly Level level;
        public readonly Wire wire;
        public readonly int connectionNum;

        public RequestElectronicOutputEventArgs(Level level, Wire wire, int connectionNum)
        {
            this.level = level;
            this.wire = wire;
            this.connectionNum = connectionNum;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public delegate int GetElectronicStateEventHandler(object o, GetElectronicStateEventArgs e);
    public class GetElectronicStateEventArgs : EventArgs
    {
        public readonly Level level;
        public readonly int x, y;

        public GetElectronicStateEventArgs(Level level, int x, int y)
        {
            this.x = x;
            this.y = y;
            this.level = level;
        }
    }
}
