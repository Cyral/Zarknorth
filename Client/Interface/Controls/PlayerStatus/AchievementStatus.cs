using System;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using ZarknorthClient.Interface;

namespace ZarknorthClient
{
    public class AchievementStatusBox : StatusBox
    {
        #region Properties
        double TotalTime = 6; //Seconds to show
        double Time;
        #endregion

        #region Controls
        Button btnView;
        #endregion

        public AchievementStatusBox(Manager manager, Achievement achievement)
            : base(manager)
        {
            
            Passive = false;
            Caption.Text = achievement.Name;
            Description.Text = achievement.Description;
            Progress.Hide();
            Description.Height = 12;
            Width += 100;
            Height += 2;

            btnView = new Button(manager);
            btnView.Init();
            btnView.Width = 100;
            btnView.Height = 24;
            btnView.Top = Description.Top + Description.Height + 4;
            btnView.TextChanged +=btnView_TextChanged;
            btnView.Text = "View Achievements";
            btnView.Click += btnView_Click;
            Add(btnView);
        }

        void btnView_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            TaskAchievementLog AchievementLog = new TaskAchievementLog(Game.level.game.Manager);
            AchievementLog.Init();
            Game.level.game.Manager.Add(AchievementLog);
        }

        void btnView_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            btnView.Width = (int)Manager.Skin.Fonts[0].Resource.MeasureRichString(btnView.Text, Manager).X + 16;
            btnView.Left = (ClientWidth / 2) - (btnView.Width / 2);
        }
        protected override void Update(GameTime gameTime)
        {
            Time += gameTime.ElapsedGameTime.TotalSeconds;
            if (Time >= TotalTime)
                Close();
            base.Update(gameTime);
        }
        public override void Depleted()
        {
            Close();
            base.Depleted();
        }
    }
}
