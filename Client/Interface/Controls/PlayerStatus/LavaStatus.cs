using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class LavaStatusBox : StatusBox
    {
        #region Properties
        float dmg;
        #endregion

        #region Controls
        #endregion

        public LavaStatusBox(Manager manager)
            : base(manager)
        {
            Passive = false;
            Caption.Text = "Melting!";
            Description.Text = "Ahh! Lavaaaaa";
            Progress.ValueChanged += Progress_ValueChanged;
            Progress.Value = (int)Game.level.Players[0].Health;
            AbsValue = Game.level.Players[0].Health;
        }

        void Progress_ValueChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
         
        }
        public override void Deflate(GameTime gameTime)
        {
            dmg += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dmg >= .25f)
            {
                Game.level.Players[0].Damage(Game.random.Next(2,4), Entities.DamageType.Lava);
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
                    Description.Text = "Ouch, HOT!";
                }
                if (Progress.Value < 55)
                {
                    Description.Text = "Quick Quick!";
                }
                if (Progress.Value < 45)
                {
                    Description.Text = "ARRHHHHH";
                }
                if (Progress.Value < 35)
                {
                    Description.Text = "The pain...";
                }
                if (Progress.Value < 20)
                {
                    Description.Text = "AHRHH";
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
            Game.level.Players[0].Kill(Entities.DamageType.Lava);
            base.Depleted();
        }
    }
}
