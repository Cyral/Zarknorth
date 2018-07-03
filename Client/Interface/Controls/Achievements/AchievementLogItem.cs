using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class AchievementLogItem : Control
    {
        public Label Label;
        public Achievement Achievement;
        public StatusBar sb;
        public Label PrizeLabel;

        public AchievementLogItem(Manager manager, Achievement achievement, Color color)
            : base(manager)
        {
            string completedText = achievement.Achieved ? "Completed" : "Incomplete";
            Achievement = achievement;
            this.ToolTip.Text = achievement.Description;
            sb = new StatusBar(manager);
            sb.Init();
            sb.Alpha = .8f;
            sb.Color = new Color(((float)color.R / 255f) * sb.Alpha, ((float)color.G / 255f) * sb.Alpha, ((float)color.B / 255f) * sb.Alpha, sb.Alpha);
            Add(sb);
         
            Label = new Label(manager);
            Label.Init();
            Label.Height = 12;
            string plural = achievement.Points > 1 ? "s" : "";
            Label.Text = "[color:White]" + achievement.Name + "[/color]" + " - " + achievement.Description + " (" + achievement.Points + " point" + plural + ", " +completedText + ")";
            Label.Left = 4;
            Label.Top = -12;
            Label.ToolTip.Text = achievement.Name;
            Add(Label);

            PrizeLabel = new Label(manager);
            PrizeLabel.Init();
            PrizeLabel.Left = 4;
            PrizeLabel.Top = Label.Top + Label.Height + 8;
            PrizeLabel.TextChanged += PrizeLabel_TextChanged;
            PrizeLabel.Text = Achievement.Prizes.Count == 0 ? achievement.Achieved ? "Rewarded: None" : "Reward: None" : achievement.Achieved ? "Rewarded: " : "Reward" + plural + ":";
            Add(PrizeLabel);

            int i = 0;
            int left = PrizeLabel.Left + PrizeLabel.Width;
            foreach (Slot s in Achievement.Prizes) //(Just ported from crafting class, finds items and prints them)
            {
                ImageBox imgTmp = new ImageBox(manager);
                imgTmp.Init();
                imgTmp.Image = ContentPack.Textures["items\\" + s.Item.Name];
                Label lblTmp = new Label(manager);
                lblTmp.Init();
                lblTmp.Text = "x" + s.Stack.ToString();
                //lblTmp.ToolTip.Text = Slot.HowMany(s.Item, Game.level.Players[0].inventory) + "/" + s.Stack;
                lblTmp.Width = (int)manager.Skin.Fonts[0].Resource.MeasureRichString(lblTmp.Text, manager).X;
                lblTmp.Top = 8;
                imgTmp.Top = 20;
                lblTmp.Left = left + Tile.Width + 2;
                imgTmp.Left = left;
                imgTmp.ToolTip.Text = s.Item.Name;
                left += Tile.Width + 4 + lblTmp.Width;
                imgTmp.SizeMode = SizeMode.Fit;
                imgTmp.Width = Tile.Width;
                imgTmp.Height = Tile.Height;

                Add(imgTmp);
                Add(lblTmp);
                i += 1;
            }
            if (Achievement.Money > 0)
            {
                left += 4;
                ImageBox imgTmp = new ImageBox(manager);
                imgTmp.Init();
                imgTmp.Image = ContentPack.Textures["gui\\icons\\money"];
                Label lblTmp = new Label(manager);
                lblTmp.Init();
                lblTmp.Text = "Cash: " + Achievement.Money;
                //lblTmp.ToolTip.Text = Slot.HowMany(s.Item, Game.level.Players[0].inventory) + "/" + s.Stack;
                lblTmp.Width = (int)manager.Skin.Fonts[0].Resource.MeasureRichString(lblTmp.Text, manager).X;
                lblTmp.Top = 8;
                imgTmp.Top = 24;
                lblTmp.Left = left + Tile.Width + 2;
                imgTmp.Left = left + 4;
                imgTmp.ToolTip.Text = "Money Reward: " + Achievement.Money;
                left += Tile.Width + 4 + lblTmp.Width;
                imgTmp.SizeMode = SizeMode.Auto;
                imgTmp.Width = Tile.Width;
                imgTmp.Height = Tile.Height;

                Add(imgTmp);
                Add(lblTmp);
            }
                
            this.Height = 48;
            ItemListControl_Resize(this, new ResizeEventArgs());
            Resize += ItemListControl_Resize;


        }

        void PrizeLabel_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            PrizeLabel.Width = (int)Manager.Skin.Fonts[0].Resource.MeasureRichString(PrizeLabel.Text, Manager).X;
        }

        void ItemListControl_Resize(object sender, ResizeEventArgs e)
        {
            Label.Width = e.Width - (Label.Left + Label.Width);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            // base.DrawControl(renderer, rect, gameTime);
        }

    }
}
