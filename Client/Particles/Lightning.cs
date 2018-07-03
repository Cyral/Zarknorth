using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
namespace ZarknorthClient
{
    class LightningBolt
    {
        public List<Line> Segments = new List<Line>();
        public Vector2 Start { get { return Segments[0].A; } }
        public Vector2 End { get { return Segments.Last().B; } }
        public bool IsComplete { get { return Alpha <= 0; } }

        public float Alpha { get; set; }
        public float AlphaMultiplier { get; set; }
        public float FadeOutRate { get; set; }
        public Color Tint { get; set; }
        private bool FirstFade = false;
        static Random rand = new Random();

        public LightningBolt(Vector2 source, Vector2 dest) : this(source, dest, new Color(0.9f, 0.8f, 1f)) { }

        public LightningBolt(Vector2 source, Vector2 dest, Color color)
        {
            Segments = CreateBolt(source, dest, 2);

            Tint = color;
            Alpha = 1f;
            AlphaMultiplier = 0.6f;
            FadeOutRate = 3f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Alpha <= 0)
                return;

            foreach (var segment in Segments)
                segment.Draw(spriteBatch, Tint * (Alpha * AlphaMultiplier));
        }

        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (!FirstFade)
            Alpha -= (FadeOutRate * 5 )* elapsed;
            else
            Alpha -= FadeOutRate * elapsed;
            if (Alpha <= 0 && FirstFade == false)
            {
                Alpha = 1f;
                FirstFade = true;
            }
        }

        protected static List<Line> CreateBolt(Vector2 source, Vector2 dest, float thickness)
        {
            var results = new List<Line>();
            Vector2 tangent = dest - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 4; i++)
                positions.Add(Rand(0, 1));

            positions.Sort();

            const float Sway = 90;
            const float Jaggedness = 1 / Sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Rand(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(new Line(prevPoint, point, thickness));
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(new Line(prevPoint, dest, thickness));

            return results;
        }

        // Returns the point where the bolt is at a given fraction of the way through the bolt. Passing
        // zero will return the start of the bolt, and passing 1 will return the end.
        public Vector2 GetPoint(float position)
        {
            var start = Start;
            float length = Vector2.Distance(start, End);
            Vector2 dir = (End - start) / length;
            position *= length;

            var line = Segments.Find(x => Vector2.Dot(x.B - start, dir) >= position);
            float lineStartPos = Vector2.Dot(line.A - start, dir);
            float lineEndPos = Vector2.Dot(line.B - start, dir);
            float linePos = (position - lineStartPos) / (lineEndPos - lineStartPos);

            return Vector2.Lerp(line.A, line.B, linePos);
        }

        private static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }

        private static float Square(float x)
        {
            return x * x;
        }

        public class Line
        {
            public Vector2 A;
            public Vector2 B;
            public float Thickness;

            public Line() { }
            public Line(Vector2 a, Vector2 b, float thickness = 1)
            {
                A = a;
                B = b;
                Thickness = thickness;
            }

            public void Draw(SpriteBatch spriteBatch, Color tint)
            {
                Vector2 tangent = B - A;
                float theta = (float)Math.Atan2(tangent.Y, tangent.X);

                const float ImageThickness = 8;
                float thicknessScale = Thickness / ImageThickness;

                Vector2 capOrigin = new Vector2(ContentPack.Textures["environment\\LightningHalf"].Width, ContentPack.Textures["environment\\LightningHalf"].Height / 2f);
                Vector2 middleOrigin = new Vector2(0, ContentPack.Textures["environment\\LightningSegment"].Height / 2f);
                Vector2 middleScale = new Vector2(tangent.Length(), thicknessScale);

                spriteBatch.Draw(ContentPack.Textures["environment\\LightningSegment"], A, null, tint, theta, middleOrigin, middleScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(ContentPack.Textures["environment\\LightningHalf"], A, null, tint, theta, capOrigin, thicknessScale, SpriteEffects.None, 0f);
                spriteBatch.Draw(ContentPack.Textures["environment\\LightningHalf"], B, null, tint, theta + MathHelper.Pi, capOrigin, thicknessScale, SpriteEffects.None, 0f);
            }
        }
    }
    class BranchLightning
    {
        List<LightningBolt> bolts = new List<LightningBolt>();

        public bool IsComplete { get { return bolts.Count == 0 && Sound.State == SoundState.Stopped; } }
        public Vector2 End { get; private set; }
        private Vector2 direction;
        private SoundEffectInstance Sound;
        static Random rand = new Random();

        public BranchLightning(Vector2 start, Vector2 end, float pan)
        {
            End = end;
            direction = Vector2.Normalize(end - start);
            Create(start, end);
            Sound = Game.level.soundContent["Thunder" + rand.Next(0, 10)].CreateInstance();
            Sound.Play();
        }

        public void Update(GameTime gameTime)
        {
            bolts = bolts.Where(x => !x.IsComplete).ToList();
            foreach (var bolt in bolts)
                bolt.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var bolt in bolts)
                bolt.Draw(spriteBatch);
        }

        private void Create(Vector2 start, Vector2 end)
        {
            var mainBolt = new LightningBolt(start, end);
            bolts.Add(mainBolt);

            int numBranches = rand.Next(3, 6);
            Vector2 diff = end - start;

            float[] branchPoints = Enumerable.Range(0, numBranches)
                .Select(x => Rand(0, 1f))
                .OrderBy(x => x).ToArray();

            for (int i = 0; i < branchPoints.Length; i++)
            {
                Vector2 boltStart = mainBolt.GetPoint(branchPoints[i]);
                Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(30 * ((i & 1) == 0 ? 1 : -1)));
                Vector2 boltEnd = Vector2.Transform(diff * (1 - branchPoints[i]), rot) + boltStart;
                bolts.Add(new LightningBolt(boltStart, boltEnd));
            }
        }

        static float Rand(float min, float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
    }
}
