using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class BreathStatusBox : StatusBox
    {
        #region Controls
        #endregion

        public BreathStatusBox(Manager manager)
            : base(manager)
        {
            Passive = false;
            Caption.Text = "Breath";
            Description.Text = "Ah, Fresh water!";
            DepletionPerSecond = 4;
        }
        protected override void Update(GameTime gameTime)
        {
            if (!Hiding)
            {
                if (Progress.Value < 75)
                {
                    Description.Text = "Just keep swimming!";
                }
                if (Progress.Value < 25)
                {
                    Description.Text = "Almost there...";
                }
            }
            base.Update(gameTime);
        }
        public override void Depleted()
        {
            if (Game.level != null)
            {
                Game.level.Players[0].DrownStatus = new DrownStatusBox(Manager);
                Game.level.Players[0].DrownStatus.Init();
                Game.level.Players[0].DrownStatus.Progress.Color = Color.Red;
                Manager.Add(Game.level.Players[0].DrownStatus);
                //Game.level.Players[0].Kill(Entities.DamageType.Drown);
                base.Depleted();
            }
        }
    }
}
