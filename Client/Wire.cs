using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace ZarknorthClient
{
    public class Wire
    {
        public bool Powered
        {
            get
            {
                return powered;
            }
            set
            {
                bool old = powered;
                powered = value;
                if (old != value)
                PowerChanged(powered, value);
            }
        }
        private bool powered;
        public bool ReCalc;

        public short X1, X2, Y1, Y2;
        public bool Input1, Input2;

        public Tile Connection1;
        public Tile Connection2;

        public int ConnectionID1;
        public int ConnectionID2;

        public ConnectionPoint ConnectionPoint1;
        public ConnectionPoint ConnectionPoint2;

        public void PowerChanged(bool OldPower, bool NewPower)
        {
            ConnectionPoint SelectedPoint = null;
            ElectronicTile Connection = null;
            int ID = 0;
            int inputID = 0;
            int outputID = 0;
            if (ConnectionPoint1 != null && ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Input)
            {
                SelectedPoint = ConnectionPoint1;
                Connection = Connection1 as ElectronicTile;
                ID = 1;
                inputID = ConnectionID1;
                Connection.Inputs[inputID].ForEach(x => x.Powered = NewPower);
            }
            else if (ConnectionPoint2 != null && ConnectionPoint2.Type == ConnectionPoint.ConnectionType.Input)
            {
                SelectedPoint = ConnectionPoint2;
                Connection = Connection2 as ElectronicTile;
                ID = 2;
                inputID = ConnectionID2;
                Connection.Inputs[inputID].ForEach(x => x.Powered = NewPower);
            }
            else
                return;
            if (Connection.Outputs == null)
                return;
            foreach (List<Wire> points in Connection.Outputs)
            {
                Wire point = null;

                bool voltage = false;
                if (points.Count > 0)
                    if (ID == 1)
                        voltage = Connection.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(Game.level, this, 1));
                    else if (ID == 2)
                        voltage = Connection.Foreground.OnRequestOutput(new RequestElectronicOutputEventArgs(Game.level, this, 2));
                foreach (Wire w in points)
                {
                    if (w != this)
                    w.Powered = voltage;
                }
            }
            Connection.Inputs[inputID].ForEach(x => x.Powered = NewPower);
       
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            Vector2 point2 = Vector2.Zero;
            Vector2 point1 = ConnectionPoint.GetDrawPosition(ConnectionPoint1, Connection1, Connection1.X, Connection1.Y);
            if (Connection2 != null)
                point2 = ConnectionPoint.GetDrawPosition(ConnectionPoint2, Connection2, Connection2.X, Connection2.Y);
            else
                point2 = Game.level.MainCamera.Position + new Vector2(ms.X, ms.Y);
            DrawLine(spriteBatch, point1 + new Vector2(ConnectionPoint.PointWidth / 2, ConnectionPoint.PointHeight / 2), point2 + new Vector2(ConnectionPoint.PointWidth / 2, ConnectionPoint.PointHeight / 2), Powered ? Color.White : Color.Gray);
        }
        private static void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 5)
        {
             Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length()+width, width);
             Vector2 v = Vector2.Normalize(begin - end);
             float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
             if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
             spriteBatch.Draw(ContentPack.Textures["gui\\icons\\wire"], r, null, color, angle, new Vector2(0, ContentPack.Textures["gui\\icons\\wire"].Height / 2), SpriteEffects.None, 0);
        }
        public static bool GetInputState(int input, GetElectronicStateEventArgs e)
        {
            return (e.level.tiles[e.x, e.y] as ElectronicTile).Inputs[input].Any(x => x.Powered);
        }
        public static bool GetInputPower(int input, RequestElectronicOutputEventArgs e)
        {
            return e.connectionNum == 1 ? (e.wire.Connection1 as ElectronicTile).Inputs[input].Any(x => x.Powered) : (e.wire.Connection2 as ElectronicTile).Inputs[input].Any(x => x.Powered);
        }
        //public void Update()
        //{
        //    if (ConnectionPoint1.Type == ConnectionPoint.ConnectionType.Output)
        //}
    }
    public class ConnectionPoint
    {
        public const int PointWidth = 11, PointHeight = 11;
        /// <summary>
        /// Each connection is either an output (+) or an input (-)
        /// </summary>
        public enum ConnectionType
        { Input, Output }

        /// <summary>
        /// Common positions around the block that may be used for connections
        /// </summary>
        public enum ConnectionPreset
        { TopCenter, TopRight, TopLeft, BottomCenter, BottomRight, BottomLeft, MiddleCenter, MiddleRight, MiddleLeft, Custom }

        /// <summary>
        /// The type, output or input of this connection
        /// </summary>
        public ConnectionType Type { get; set; }
        /// <summary>
        /// The position relative to the center of the block, if no ConnectionPreset has been specified
        /// </summary>
        public Point Position { get; set; }
        public ConnectionPreset ConnectionPosition { get; set; }

        public ConnectionPoint(Point position, ConnectionType connectionType)
        {
            Position = position;
            Type = connectionType;
            ConnectionPosition = ConnectionPreset.Custom;
        }
        public ConnectionPoint(ConnectionPreset position, ConnectionType connectionType)
        {
            ConnectionPosition = position;
            Type = connectionType;
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Tile tile, int X, int Y)
        {
            Vector2 drawPosition = GetDrawPosition(this, tile, X, Y);
            Texture2D texture = Type == ConnectionType.Input ? ContentPack.Textures["gui\\icons\\WireInput"] : ContentPack.Textures["gui\\icons\\WireOutput"];
            spriteBatch.Draw(texture, drawPosition, Color.White);
        }
        public static Vector2 GetDrawPosition(ConnectionPoint point, Tile tile, int X, int Y)
        {
            Vector2 drawPosition = Vector2.Zero;
            if (point.ConnectionPosition == ConnectionPreset.Custom)
            {
                drawPosition = new Vector2((X * Tile.Width) + ((tile.Foreground.Size.X * Tile.Width) / 2) + point.Position.X,
                     (Y * Tile.Height) + ((tile.Foreground.Size.Y * Tile.Height) / 2) + point.Position.Y);
            }
            else
            {
                switch (point.ConnectionPosition)
                {
                    case ConnectionPreset.TopCenter:
                        drawPosition = new Vector2((X * Tile.Width) + ((tile.Foreground.Size.X * Tile.Width) / 2),
     (Y * Tile.Height));
                        break;
                    case ConnectionPreset.BottomCenter:
                        drawPosition = new Vector2((X * Tile.Width) + ((tile.Foreground.Size.X * Tile.Width) / 2),
     (Y * Tile.Height) + (tile.Foreground.Size.Y * Tile.Height));
                        break;
                    case ConnectionPreset.MiddleCenter:
                        drawPosition = new Vector2((X * Tile.Width) + ((tile.Foreground.Size.X * Tile.Width) / 2),
     (Y * Tile.Height) + ((tile.Foreground.Size.Y * Tile.Height) / 2));
                        break;
                    case ConnectionPreset.TopRight:
                        drawPosition = new Vector2((X * Tile.Width) + (tile.Foreground.Size.X * Tile.Width),
     (Y * Tile.Height));
                        break;
                    case ConnectionPreset.BottomRight:
                        drawPosition = new Vector2((X * Tile.Width) + (tile.Foreground.Size.X * Tile.Width),
     (Y * Tile.Height) + (tile.Foreground.Size.Y * Tile.Height));
                        break;
                    case ConnectionPreset.MiddleRight:
                        drawPosition = new Vector2((X * Tile.Width) + (tile.Foreground.Size.X * Tile.Width),
     (Y * Tile.Height) + ((tile.Foreground.Size.Y * Tile.Height) / 2));
                        break;
                    case ConnectionPreset.TopLeft:
                        drawPosition = new Vector2((X * Tile.Width),
     (Y * Tile.Height));
                        break;
                    case ConnectionPreset.BottomLeft:
                        drawPosition = new Vector2((X * Tile.Width),
     (Y * Tile.Height) + (tile.Foreground.Size.Y * Tile.Height));
                        break;
                    case ConnectionPreset.MiddleLeft:
                        drawPosition = new Vector2((X * Tile.Width),
     (Y * Tile.Height) + ((tile.Foreground.Size.Y * Tile.Height) / 2));
                        break;
                }
            }
            return drawPosition -= new Vector2(PointWidth / 2, PointHeight / 2);
        }
    }
}
