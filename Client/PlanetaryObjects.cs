using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Cyral.Extensions;

namespace ZarknorthClient
{
    public class PlanetaryBase
    {
        public static int Total;
        #region Properties
        public string Name { get; set; }
        protected Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float OuterRadius { get; set; }

        public virtual float Radius { get { return radius; } set { radius = value; diameter = value * 2; } }
        protected float radius;

        public virtual float Diameter { get { return diameter; } set { diameter = value; radius = value / 2; } }
        protected float diameter;

        public int ID { get; set; }

        public PlanetaryBase()
        {
            Name = string.Empty;
            ID = Total;
            Total++;
        }
        #endregion
    }
    public class Galaxy : PlanetaryBase
    {
        #region Properties
        public List<SolarSystem> Children { get; set; }
        #endregion
        #region Constructors
        public Galaxy()
            : base()
        {
            Children = new List<SolarSystem>();
        }
        #endregion
    }
    public class SolarSystem : PlanetaryBase
    {
        #region Properties
        public List<PlanetaryObject> Children { get; set; }
        public Galaxy Parent { get; set; }
        public PlanetNamer.NameStyle NameStyle { get; set; }
        #endregion
        #region Fields
        private static Random random;
        #endregion
        #region Constructors
        public SolarSystem()
            : base()
        {
            Children = new List<PlanetaryObject>();
            //Pick a random name style
            double f = random.NextDouble();
            if (f < .33f)
                NameStyle = PlanetNamer.NameStyle.Roman;
            else if (f < .55f)
                NameStyle = PlanetNamer.NameStyle.Greek;
            else if (f < .84f)
                NameStyle = PlanetNamer.NameStyle.Alphabetical;
            else
                NameStyle = PlanetNamer.NameStyle.Numerical;
        }
        static SolarSystem()
        {
            random = new Random();
        }
        #endregion
    }
    public class PlanetaryObject : PlanetaryBase
    {
        #region Properties
        public SolarSystem Parent { get; set; }
        public string Description { get; set; }
        public Color Color { get; set; }
        public PlanetaryType Type { get; set; }
        public PlanetarySubType SubType { get; set; }
        public int Seed { get; set; }
        public float Gravity { get; set; }
        public override float Radius { get { return radius; } set { radius = value; diameter = value * 2; CalcGravity(); } }
        public override float Diameter { get { return diameter; } set { diameter = value; radius = value / 2; CalcGravity(); } }

        private float angle;
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = value;
                if (Type == PlanetaryType.Planet)
                    position = Rotate(MathHelper.ToRadians(angle), OuterRadius, Parent.Position + new Vector2(Parent.Children[0].Radius, Parent.Children[0].Radius)) - new Vector2(Radius,Radius);

            }
        }
        public static Vector2 Rotate(float angle, float distance, Vector2 centre)
        {
            return new Vector2((float)(distance * Math.Cos(angle)), (float)(distance * Math.Sin(angle))) + centre;
        }
        #endregion
        #region Fields

        #endregion
        #region Constructors
        public PlanetaryObject(string name, int i, SolarSystem parent, PlanetaryType type, PlanetarySubType subType) : base()
        {
            Parent = parent;
            Name = name;
            Color = Color.White;
            Seed = Guid.NewGuid().GetHashCode();

            Type = type;
            SubType = subType;

            if (Type == PlanetaryType.Planet && Parent != null)
            {              
                Name = PlanetNamer.AddSuffix(Parent.NameStyle, Name, i);
                Description = i.AddOrdinal() + " Planet, " + subType.Name;
            }
            else if (Type == PlanetaryType.Sun)
            {
                Description = SubType.Name + " - Planetary System";
            }
        }
        /// <summary>
        /// Computes gravity relative to 1 for a planet
        /// </summary>
        private void CalcGravity()
        {
            if (Type == PlanetaryType.Planet)
            {
                if (Diameter > 200)
                    Gravity = 1 + (.2f * (Math.Abs(200 - Diameter) / (256 - 200)));
                else
                    Gravity = 1 - (.2f * ((200 - Diameter) / (200 - 156)));
            }
        }
        #endregion
    }
    public enum PlanetaryType
    {
        Planet,
        Sun,
        Moon,
    }
    public class PlanetarySubType
    {
        public int MinDiameter { get; protected set; }
        public int MaxDiameter { get; protected set; }
        public int AvgDiameter { get; protected  set; }
        public string Name { get; protected set; }
        public int ID { get; protected set; }
    }
    public class SunType : PlanetarySubType
    {
        public static List<SunType> SunTypes;

        public static SunType OMainSequence, BMainSequence, AMainSequence, FMainSequence, GMainSequence, KMainSequence, MMainSequence, RedGiant, WhiteDwarf, SuperGiant;

        public Color[] Colors { get; protected set; }
        public float Abundance { get; protected set; }

        private static List<SunType> Chances;

        public SunType(string name)
        {
            Name = name;
            SunTypes.Add(this);
            ID = SunTypes.Count();
        }

        static SunType()
        {
            SunTypes = new List<SunType>();
            OMainSequence = new SunType("Main Sequence Star (O)")
            {
                Abundance = .02f,
                Colors = new Color[] { new Color(0,220,255) },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };
            BMainSequence = new SunType("Main Sequence Star (B)")
            {
                Abundance = .05f,
                Colors = new Color[] { new Color(45, 180, 255) },
                MinDiameter = 550,
                MaxDiameter = 680,
                AvgDiameter = 630,
            };
            AMainSequence = new SunType("Main Sequence Star (A)")
            {
                Abundance = .05f,
                Colors = new Color[] { new Color(140, 200, 255) },
                MinDiameter = 530,
                MaxDiameter = 660,
                AvgDiameter = 590,
            };
            FMainSequence = new SunType("Main Sequence Star (F)")
            {
                Abundance = .06f,
                Colors = new Color[] { new Color(255, 235, 165) },
                MinDiameter = 500,
                MaxDiameter = 650,
                AvgDiameter = 570,
            };
            GMainSequence = new SunType("Main Sequence Star (G)")
            {
                Abundance = .08f,
                Colors = new Color[] { new Color(255, 255, 135), new Color(255, 230, 135) },
                MinDiameter = 490,
                MaxDiameter = 640,
                AvgDiameter = 580,
            };
            KMainSequence = new SunType("Main Sequence Star (K)")
            {
                Abundance = .14f,
                Colors = new Color[] { new Color(255, 182, 63), new Color(255, 192, 68) },
                MinDiameter = 470,
                MaxDiameter = 630,
                AvgDiameter = 550,
            };
            MMainSequence = new SunType("Main Sequence Star (M)")
            {
                Abundance = .42f,
                Colors = new Color[] { new Color(255, 145, 0), new Color(255, 120, 5), new Color(255, 165, 0), new Color(255, 155, 20), },
                MinDiameter = 460,
                MaxDiameter = 610,
                AvgDiameter = 520,
            };
            RedGiant = new SunType("Red Giant")
            {
                Abundance = .12f,
                Colors = new Color[] { new Color(255, 52, 0), new Color(255, 100, 20) },
                MinDiameter = 630,
                MaxDiameter = 760,
                AvgDiameter = 710,
            };
            SuperGiant = new SunType("Super Giant")
            {
                Abundance = .07f,
                Colors = new Color[] { new Color(255, 70, 30), new Color(255, 150, 60) },
                MinDiameter = 680,
                MaxDiameter = 768,
                AvgDiameter = 750,
            };
            WhiteDwarf = new SunType("White Dwarf")
            {
                Abundance = .06f,
                Colors = new Color[] { new Color(250,255,255), new Color(245, 253, 255) },
                MinDiameter = 90,
                MaxDiameter = 200,
                AvgDiameter = 140
            };
            SunTypes = SunTypes.OrderBy(x => x.Abundance).ToList();
            Chances = new List<SunType>();
            foreach (SunType st in SunTypes)
            {
                for (int i = 0; i < st.Abundance * 100; i++)
                {
                    Chances.Add(st);
                }
            }
        }
        public static SunType GetRandom()
        {
            return Chances[Game.random.Next(0,Chances.Count-1)];
        }
    }
    public class PlanetType : PlanetarySubType
    {
        public static List<PlanetType> PlanetTypes;

        public static PlanetType Temperate, Tropical, Desert, Barren, Tundra, Artic, Inferno;

        public Color[] Colors { get; protected set; }
        public Tuple<float, float> Range { get; protected set; }

        public PlanetType(string name)
        {
            Name = name;
            PlanetTypes.Add(this);
            ID = PlanetTypes.Count();
        }

        static PlanetType()
        {
            PlanetTypes = new List<PlanetType>();

            Inferno = new PlanetType("Inferno")
            {
                Range = new Tuple<float, float>(0, 0.15f),
                Colors = new Color[] { new Color(190, 100, 15), new Color(190, 40, 15), new Color(220, 80, 0), new Color(220, 160,0), new Color(220, 120, 60), new Color(230, 40, 0), new Color(250, 180, 0), new Color(230, 30, 0) },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };

            Barren = new PlanetType("Barren")
            {
                Range = new Tuple<float, float>(Inferno.Range.Item2, 0.17f),
                Colors = new Color[] { new Color(200, 170, 140), new Color(160, 140, 130), new Color(190, 160, 130) },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };

            Desert = new PlanetType("Desert")
            {
                Range = new Tuple<float, float>(Barren.Range.Item2, .334f),
                Colors = new Color[] { new Color(230, 220, 135), new Color(230, 170, 70), new Color(230, 200, 120), },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };

            Tropical = new PlanetType("Tropical")
            {
                Range = new Tuple<float, float>(.3f, .53f),
                Colors = new Color[] { new Color(0, 180, 10), new Color(0, 138, 10), new Color(120, 200, 30), new Color(140, 175, 0), new Color(70, 180, 0), new Color(190, 230, 0), new Color(100, 220, 50), new Color(70, 150, 0), new Color(130, 200, 0), },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };

            Temperate = new PlanetType("Temperate")
            {
                Range = new Tuple<float, float>(.5f, .82f),
                Colors = new Color[] { new Color(0, 180, 10), new Color(0, 210, 10), new Color(0, 230, 10), new Color(0, 225, 10), new Color(0, 240, 10), new Color(50, 235, 75), new Color(25, 205, 50), new Color(80, 210, 30), new Color(10, 130, 40), new Color(35, 130, 60), new Color(35, 130, 110), new Color(35, 130, 92), new Color(10, 180, 110), new Color(120, 180, 50), new Color(100, 180, 50), },
                MinDiameter = 900,
                MaxDiameter = 1000,
                AvgDiameter = 950,
            };

            Tundra = new PlanetType("Tundra")
            {
                Range = new Tuple<float, float>(.76f, 0.86f),
                Colors = new Color[] { new Color(186, 225, 184), new Color(200, 225, 222), new Color(130, 200, 200), new Color(125, 200, 195), new Color(125, 180, 160), new Color(15, 100, 70) },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };
            Artic = new PlanetType("Artic")
            {
                Range = new Tuple<float, float>(0.86f, 1f),
                Colors = new Color[] { new Color(200, 225, 240), new Color(140, 150, 240), new Color(30, 125, 140), new Color(125, 200, 195), new Color(100, 220, 230), new Color(90, 160, 230), new Color(230, 250, 252) },
                MinDiameter = 590,
                MaxDiameter = 730,
                AvgDiameter = 660,
            };
        }
        public static PlanetType GetRandom(float index)
        {
            PlanetTypes.Shuffle();
            foreach (PlanetType p in PlanetTypes)
            {
                if (index.Between(p.Range.Item1, p.Range.Item2, true))
                    return p;
            }
            throw new Exception("Could not find PlanetType containing the value of " + index + " in its range.");
        }
    }
}
