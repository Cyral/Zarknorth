#region Using
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
#endregion

namespace ZarknorthClient
{
    /// <summary>
    /// Represents a tool bar that is part of a menu system made of tool bar buttons.
    /// </summary>
    public class GradientPanel : Control
    { 
        #region Constructors
        /// <summary>
        /// Creates a new GradientPanel control.
        /// </summary>
        /// <param name="manager">GUI manager for the tool bar.</param>
        public GradientPanel(Manager manager)
            : base(manager)
        {
            Left = 0;
            Top = 0;
            Width = 64;
            Height = 24;
            //CanFocus = false;
        }
        #endregion

        #region Init
        /// <summary>
        /// Initializes the tool bar control.
        /// </summary>
        public override void Init()
        {
            base.Init();
        }
        #endregion

        #region Init Skin
        /// <summary>
        /// Initializes the skin of the tool bar control.
        /// </summary>
        protected override void InitSkin()
        {
            Skin = Manager.Skin.Controls["ToolBar"];
        }
        #endregion

        #region Draw Control
        /// <summary>
        /// Draws the tool bar control.
        /// </summary>
        /// <param name="renderer">Render management object.</param>
        /// <param name="rect">Destination region where the tool bar will be drawn.</param>
        /// <param name="gameTime">Snapshot of the application's timing values.</param>
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            //base.DrawControl(renderer, rect, gameTime);
            base.DrawControl(renderer, rect, gameTime);
        }
        #endregion
    }
}
