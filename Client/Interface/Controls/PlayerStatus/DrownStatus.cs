using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class DrownStatusBox : StatusBox
    {
        #region Properties
        float dmg;
        #endregion

        #region Controls
        #endregion

        public DrownStatusBox(Manager manager)
            : base(manager)
        {
            Passive = false;
            Caption.Text = "Drowning!";
            Description.Text = "Must... Find... Air...";
            Progress.ValueChanged += Progress_ValueChanged;
            DepletionPerSecond = 10;
            Progress.Value = (int)Game.level.Players[0].Health;
            AbsValue = Game.level.Players[0].Health;
        }

        void Progress_ValueChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
         
        }
        public override void Deflate(GameTime gameTime)
        {
            dmg += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dmg >= 1f)
            {
                Game.level.Players[0].Damage(DepletionPerSecond, Entities.DamageType.Drown);
                Progress.Value = (int)Game.level.Players[0].Health;
                AbsValue = Game.level.Players[0].Health;
                dmg = 0;
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (!Hiding)
            {
                if (Progress.Value < 70)
                {
                    Description.Text = "...Need... Air...";
                }
                if (Progress.Value < 45)
                {
                    Description.Text = "ARRHHHHH";
                }
                if (Progress.Value < 35)
                {
                    Description.Text = "*Gasp*";
                }
                if (Progress.Value < 20)
                {
                    Description.Text = "*Gulp*";
                }
                if (Progress.Value < 10)
                {
                    Description.Text = "...";
                }
            }
            base.Update(gameTime);
        }
        public override void Depleted()
        {
            Game.level.Players[0].Kill(Entities.DamageType.Drown);
            base.Depleted();
        }
    }
}
