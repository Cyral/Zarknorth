////////////////////////////////////////////////////////////////
//                                                            //
//  Neoforce Central                                          //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//         File: TaskAutoScroll.cs                            //
//                                                            //
//      Version: 0.7                                          //
//                                                            //
//         Date: 11/09/2010                                   //
//                                                            //
//       Author: Tom Shane                                    //
//                                                            //
////////////////////////////////////////////////////////////////
//                                                            //
//  Copyright (c) by Tom Shane                                //
//                                                            //
////////////////////////////////////////////////////////////////

#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace ZarknorthClient.Interface
{
    public class TaskAchievementLog : Dialog
    {

        #region Field
        private GroupPanel Panel;
        private ControlList<AchievementLogItem> List;
        #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////   
        public TaskAchievementLog(Manager manager)
            : base(manager)
        {
            Height = 400;
            Width = 500;
            MinimumHeight = 100;
            MinimumWidth = 100;
            Resizable = false;
            Text = "Achievement Log";
            Center();
            TopPanel.Visible = true;
            BottomPanel.Visible = false;

            //TopPanel.Visible = true;
            Caption.Text = "Achievement Log - View your achievements!";
            Description.Text = "Achievements are small quests you can work for to gain a nice reward!\nAchievement Points: " + Game.level.Players[0].achievementPoints;
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);

            AddList(manager);
            ResetItems(manager);
           
        }

        private void AddList(Manager manager)
        {
            List = new ControlList<AchievementLogItem>(manager);
            List.Init();
            List.Top = TopPanel.Height + 4;
            List.Left = 4;
            List.Width = ClientWidth - (List.Left * 2);
            List.Height = ClientHeight - List.Top - 4;
            Add(List);
        }

        public void ResetItems(Manager manager)
        {
            Color redColor = new Color(255, 50, 50, 255);
            Color greenColor = new Color(70, 255, 70, 255);
            Remove(List);
            AddList(manager);
            foreach (Achievement achievement in Achievement.AchievementList)
            {
                AchievementLogItem l = new AchievementLogItem(manager, achievement, achievement.Achieved ? greenColor : redColor);
                l.Init();
                List.Items.Add(l);
            }
        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////    
        public override void Init()
        {
            base.Init();
        }
        ////////////////////////////////////////////////////////////////////////////   

        #endregion

    }
}
