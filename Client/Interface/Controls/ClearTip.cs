using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class ClearTip : Control
    {
        #region Properties
        #endregion

        #region Controls
        #endregion

        /// <summary>
        /// Invisible control used for tooltips on signs
        /// </summary>
        /// <param name="manager"></param>
        public ClearTip(Manager manager)
            : base(manager)
        {
            Passive = true;
            Width = Height = 24;
            DrawFormattedText = false;
            //TODO: Add Initialization logic and controls
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            //base.DrawControl(renderer,rect,gameTime);
        }
    }
}
