using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZarknorthClient
{
    public class LevelData
    {
        public string Name = "World"; //Name of level
        public string Description = "No Description"; //Description Of level
        public string Author = Game.UserName;
        public string DateSaved; //Date level was last saved
        public string Version; //Version saved with
        public int Seed; //Unique level random seed
    }
}
