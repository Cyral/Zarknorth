using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class DebugLabel : Label
    {
        #region Properties
        public int Value { get { return value; } set { this.value = value; total += value; samples += 1; Invalidate(); } }
        private int value;
        public override string Name { get; set; }
        private int average = 0;
        private int normal = 0;
        private double et = 0;
        private long total;
        private long samples;
        public int UpdateRate { get; set; }
        #endregion

        #region Controls
        #endregion

        public DebugLabel(Manager manager)
            : base(manager)
        {
            UpdateRate = 300;
            //TODO: Add Initialization logic and controls
        }
        protected override void Update(GameTime gameTime)
        {
            if (et >= UpdateRate || et == 0)
            {
                if (gameTime.TotalGameTime.TotalSeconds != 0 && samples != 0)
                {
                    average = (int)(total / samples);
                }
                if (gameTime.ElapsedGameTime.TotalMilliseconds != 0)
                {
                    normal = Value;
                }
                et = 1;
            }
            et += gameTime.ElapsedGameTime.TotalMilliseconds;
            base.Update(gameTime);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            Text = Name + ": " + normal + "/" + average + "ms";
            TextColor = Color.Black * .5f;
            base.DrawControl(renderer, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width, rect.Height), gameTime);
            TextColor = Extensions.GetBlendedColor(100 - (int)((MathHelper.Clamp(Value, 0, 12) / 12) * 100));
            base.DrawControl(renderer,rect,gameTime);
        }
    }
}
