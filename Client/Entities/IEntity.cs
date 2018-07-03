using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ZarknorthClient.Entities
{
    public interface IEntity
    {
        Vector2 Position { get; set; }
        Vector2 LastPosition { get; set; }
        Vector2 Velocity { get; set; }
        Vector2 LastVelocity { get; set; }
        Level Level { get; set; }

        void Update(GameTime gameTime, KeyboardState currentKeyState, KeyboardState lastKeyState);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
