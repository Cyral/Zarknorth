//using System;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using System.Collections.Generic;

//namespace ZarknorthClient
//{
   
//    public class NpcType
//    {
//        Variables
//        public string name;
//        public string description;
//        public int health; //NPC Starting Health
//        public int spawnRate; //1 in X Chance of spawning per tick
//        public bool isFriendly; //Enemy or Pet/Passive
//        public int tiredLevel; //how tired it gets
//        public RelationPlayer relationPlayer; //check relationship with player
//        public bool Single;
//        public float Framerate = .1f;
//        public int tired;
//        Ncps
//        public static NpcType Bunny;
//        public static NpcType Skeleton;
//        public static NpcType Dog;
//        public static NpcType Cat;
//        public static NpcType Ghost;
//        public static NpcType Bird;
//        public static NpcType Bat;
//        public static NpcType Jellyfish;
//        public static NpcType Spider;
//        public NpcType()
//        {
//        }
//        static NpcType()
//        {
//            Bunny = new NpcType()
//            {
//                name = "Bunny",
//                description = "Its fluffy!",
//                isFriendly = true,
//                tiredLevel = 10,
//                relationPlayer = RelationPlayer.Avoid
//            };
//            Skeleton = new NpcType()
//            {
//                name = "Skeleton",
//                description = "Its fluffy!",
//                isFriendly = true,
//                tiredLevel = 10,
//                relationPlayer = RelationPlayer.Avoid,
//                health = 100,
//            };
//            Spider = new NpcType()
//            {
//                name = "Spider",
//                description = "Its fluffy!",
//                isFriendly = true,
//                tiredLevel = 10,
//                relationPlayer = RelationPlayer.Avoid,
//                health = 100,
//            };
//            Jellyfish = new NpcType()
//            {
//                name = "Jellyfish",
//                description = "Stung...",
//                isFriendly = true,
//                tiredLevel = 10,
//                relationPlayer = RelationPlayer.Avoid,
//                health = 100,
//                Single = true,
//                Framerate = .2f,
//            };
//            Cat = new NpcType()
//            {
//                name = "Cat",
//                description = "Jak's favorite!",
//                isFriendly = true,
//                tiredLevel = 100,
//                relationPlayer = RelationPlayer.Follow
//            };
//            Ghost = new NpcType()
//            {
//                name = "Ghost",
//                description = "Boo!",
//                tiredLevel = 100,
//                relationPlayer = RelationPlayer.Follow
//            };
//            Bird = new NpcType()
//            {
//                name = "Bird",
//                description = "Tweet Tweet",
//                tiredLevel = 100,z
//                Single = true,
//                relationPlayer = RelationPlayer.Pass
//            };
//            Bat = new NpcType()
//            {
//                name = "Bat",
//                description = "",
//                tiredLevel = 100,
//                Single = true,
//                relationPlayer = RelationPlayer.Pass
//            };

//        }
//    }
//}
