#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;
using ZarknorthClient.Interface;
#endregion

namespace ZarknorthClient
{
    public class Achievement
    {
        /// <summary>
        /// List of all achievements possible
        /// </summary>
        public static List<Achievement> AchievementList;
        /// <summary>
        /// The current achivement status popup
        /// </summary>
        public static AchievementStatusBox StatusBox;

        /// <summary>
        /// Name of the achievement, Ex, "Jump for George" or "Thats my Jam!"
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Short description of the achievement
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Item prizes to give out
        /// </summary>
        public List<Slot> Prizes { get; private set; }

        /// <summary>
        /// Amount of achievment points it is worth
        /// </summary>
        public double Points { get; private set; }

        /// <summary>
        /// Money payout to give
        /// </summary>
        public int Money { get; private set; }

        /// <summary>
        /// If the achievment has been earned yet
        /// </summary>
        public bool Achieved { get; set; }

        /// <summary>
        /// ID used for loading and saving
        /// </summary>
        public byte ID { get; private set; }

        public static Achievement Chop, Torch, PickUp, ReadSign, Jump, Music;
       
        /// <summary>
        /// Creates a new instance of an achievement
        /// </summary>
        /// <param name="name">Name of the achievement</param>
        /// <param name="description">Short description</param>
        public Achievement(string name, string description)
        {
            Name = name;
            Description = description;
            Prizes = new List<Slot>();
            Achieved = false;
            ID = (byte)AchievementList.Count();

            AchievementList.Add(this);
        }
        /// <summary>
        /// Creates a new achivement lists and adds the achivements
        /// </summary>
        static Achievement()
        {
            AchievementList = new List<Achievement>();
            Init();
        }
        /// <summary>
        /// Add all of the achivement configs here
        /// </summary>
        public static void Init()
        { 
            //
            // NOTICE: Add ALL new achievements to the end of the file, DO NOT remove achivements in production code (after release) it will mess up ID's
            //
            Jump = new Achievement("Jump for George!", "Take your first jump!")
            {
                //Prize = Item.Diamond,
                Points = 1,
            };
            Chop = new Achievement("Choppin' time!", "Chop down your first tree")
            {
                Prizes = new List<Slot> { new Slot(Item.WoodPlank,10) },
                Points = 1,
            };
            PickUp = new Achievement("Pick em' Up!", "Pick up your first item drop!")
            {
                Points = 1,
            };
            Torch = new Achievement("Light the way!", "Place a torch for light!")
            {
                Prizes = new List<Slot> { new Slot(Item.Stick, 5), new Slot(Item.Coal,5) },
                Points = 1,
                Money = 10,
            };
            ReadSign = new Achievement("Good reading", "Read a sign")
            {
               Points = 1,
            };
            Music = new Achievement("That's my jam!", "Play a note on a musical instrument")
            {
                Points = 5.0,
            };
            //ReadSign = new Achievement("Get Building!", "Place your first block")
            //{
            //    Prize = Item.Diamond,
            //};
        }
        /// <summary>
        /// Checks to see if the achievement has been earned before, if not mark it off and open a popup
        /// </summary>
        /// <param name="achievement">The achievement earned</param>
        public static void Show(Achievement achievement)
        {
            //If we have not gotten this achivement before
            if (!achievement.Achieved)
            {
                //..Now we have
                achievement.Achieved = true;

                //Create and add the popup
                StatusBox = new AchievementStatusBox(Game.level.game.Manager, achievement);
                StatusBox.Init();
                Game.level.game.Manager.Add(StatusBox);

                //Give the user the prizes, points and payout
                foreach (Slot i in achievement.Prizes) //Foreach prize
                    for (int j = 0; j < i.Stack; j++) //Stack size
                        Game.level.Players[0].AddToInventory(i.Item); //Give item
                Game.level.Players[0].achievementPoints += achievement.Points; //Add points
                Game.level.Players[0].Pay(achievement.Money); //Pay player

                //Search controls to find any open achivement logs, if found, update them.
                IEnumerable<Control> controlList = Game.level.game.Manager.Controls.Where(x => x is TaskAchievementLog);
                foreach (Control c in controlList)
                    (c as TaskAchievementLog).ResetItems(Game.level.game.Manager);

                //If there are more achivements on screen, move this box up a bit so they don't overlap
                controlList = Game.level.game.Manager.Controls.Where(x => x is AchievementStatusBox);
                StatusBox.FinalTop -= (controlList.Count() - 1) * (StatusBox.Height + 12);
            }
        }
    }
}
