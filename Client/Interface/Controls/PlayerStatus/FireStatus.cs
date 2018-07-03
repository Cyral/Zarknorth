using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class FireStatusBox : StatusBox
    {
        #region Properties
        float dmg;
        #endregion

        #region Controls
        #endregion

        public FireStatusBox(Manager manager)
            : base(manager)
        {
            Passive = false;
            Caption.Text = "On Fire!";
            Description.Text = "Hot! Hot! Hot!";
            Progress.ValueChanged += Progress_ValueChanged;
            DepletionPerSecond = 10;
            Progress.Value = (int)Game.level.Players[0].Health;
            Progress.Color = Color.Orange;
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
                Game.level.Players[0].Damage(DepletionPerSecond / 2, Entities.DamageType.Fire);
                dmg = 0;
            }
            base.Deflate(gameTime);
        }
        protected override void Update(GameTime gameTime)
        {
            if (!Hiding)
            {
                if (Progress.Value < 70)
                {
                    Description.Text = "Ahhhhh!";
                }
                if (Progress.Value < 55)
                {
                    Description.Text = "*Stop* *Drop* *Roll*";
                }
                if (Progress.Value < 45)
                {
                    Description.Text = "It burns!";
                }
                if (Progress.Value < 20)
                {
                    Description.Text = "*Ahh* Almost out!";
                }
                if (Progress.Value < 10)
                {
                    Description.Text = "*Shake* Phew!";
                }
                if (Progress.Value < 10)
                {
                }
            }
            base.Update(gameTime);
        }
        public override void Depleted()
        {
            Game.level.Players[0].OnFire = false;
            base.Depleted();
        }
    }
}
