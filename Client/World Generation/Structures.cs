using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
namespace ZarknorthClient
{
    //For storing settings for each strcuture
    public class Structure
    {
         public static List<Structure> Structures;

        #region Properties
        /// <summary>
        /// Name of the item
        /// </summary>
        public string Name;
       

     
        #endregion
        
        public static Structure Temple;
        public static Structure DesertMiniShack, OakTree;
    
        
        public Structure()
        {
            
        }
        static Structure()
        {
           Structures = new List<Structure>() 
           { 
                (Temple = new Structure()
                {
                    Name = "Temple",
                }),
                 (DesertMiniShack = new Structure()
                {
                    Name = "DesertMiniShack",
                }),
                (OakTree = new Structure()
                {
                    Name = "OakTree",
                }),
            };
        }
    }
}