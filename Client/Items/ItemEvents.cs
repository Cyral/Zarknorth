using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ZarknorthClient
{
    /// <summary>
    /// Called when the item is selected or deselected in the inventory
    /// </summary>
    public delegate void SelectItemEventHandler(object o, SelectItemEventArgs e);
    public class SelectItemEventArgs : EventArgs
    {
        public readonly Level Level;
        public readonly int Selected;

        public SelectItemEventArgs(Level level, int selected)
        {
            this.Selected = selected;
            this.Level = level;
        }
    }

    /// <summary>
    /// Called when you are holding an item and click the level with it
    /// </summary>
    public delegate void MouseItemEventHandler(object o, MouseItemEventArgs e);
    public class MouseItemEventArgs : EventArgs
    {
        public readonly Level Level;
        public readonly int X, Y;
        public readonly int lastX, lastY;
        public readonly Slot CurrentSlot;
        public readonly GameTime GameTime;

        public MouseItemEventArgs(Level level, int absoluteX, int absoluteY, int lastX, int lastY, Slot item)
        {
            this.X = absoluteX;
            this.Y = absoluteY;
            this.lastX = lastX;
            this.lastY = lastY;
            this.Level = level;
            this.CurrentSlot = item;
            this.GameTime = new GameTime();
        }

        public MouseItemEventArgs(Level level, int absoluteX, int absoluteY, int lastX, int lastY, Slot item, GameTime gameTime)
        {
            this.X = absoluteX;
            this.Y = absoluteY;
            this.lastX = lastX;
            this.lastY = lastY;
            this.Level = level;
            this.CurrentSlot = item;
            this.GameTime = gameTime;
        }
    }
    public delegate void MouseItemWorldEventHandler(object o, MouseItemWorldEventArgs e);
    public class MouseItemWorldEventArgs : MouseItemEventArgs
    {
        public readonly Entities.PhysicsEntity Entity;
        public MouseItemWorldEventArgs(Level level, int absoluteX, int absoluteY, int lastX, int lastY, Slot item, GameTime gameTime, Entities.PhysicsEntity entity) : base(level, absoluteX, absoluteY, lastX, lastY, item, gameTime)
        {
            Entity = entity;
        }
    }
    /// <summary>
    /// Handler for printing item stats used in crafting UI
    /// </summary>
    public delegate string[] PrintItemDataEventHandler(object o, PrintItemDataEventArgs e);
    public class PrintItemDataEventArgs : EventArgs
    {
        //public readonly Item Item;

        public PrintItemDataEventArgs()
        {
            //Item = item;
        }
    }
}

