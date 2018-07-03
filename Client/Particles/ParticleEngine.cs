using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Cyral.Extensions;
using Cyral.Extensions.Xna;

namespace ZarknorthClient
{
    /// <summary>
    /// Types of particles
    /// </summary>
    public enum ParticleType
    {
        LavaAmbient,
        Rain,
        RainSplash,
        Fire,
        ItemBreak,
        ItemFall,
        Blood,
        Torch,
        WaterSplash,
        LavaSplash,
        Snow,
    }
    /// <summary>
    /// ParticleEngine class, Generates, Emits and handles the particles
    /// </summary>
    public class ParticleEngine
    {
        #region Properties
        /// <summary>
        /// List of particles
        /// </summary>
        public List<Particle> Particles { get; set; }
        /// <summary>
        /// Level
        /// </summary>
        public Level Level { get; set; } 
        #endregion

        #region Fields
        private Random random;
        #endregion

        /// <summary>
        /// New particle engine contructor
        /// </summary>
        /// <param name="textures">List of textures to use</param>
        /// <param name="location">Location of the particles</param>
        public ParticleEngine(ContentManager content, Level level)
        {
            Particles = new List<Particle>();
            Level = level;
            random = new Random();
        }
                /// <summary>
        /// Generates particles
        /// </summary>
        /// <returns>texture, position, velocity, angle, angularVelocity, color, size, ttl</returns>
        public void SpawnItemParticle(ParticleType type,int x,int y, BlockItem item, Color color)
        {   
            //Return if particle quality is lower
            if (Game.ParticleQuality == 1)
                if (random.NextDouble() >= .80)
                    return;
            if (Game.ParticleQuality == 0)
                if (random.NextDouble() >= .60)
                    return;

            if (type == ParticleType.ItemBreak)
            {
                //Old
                Texture2D texture = ContentPack.Textures["items\\ " + item.Name]; //Get random texture
                Vector2 position = new Vector2(x,y); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-.4f, .4f), RandomMinMax(-.4f, .4f));


                float size = RandomMinMax(.2f,.9f);
                int ttl = 50 + random.Next(50);
                float speed = size / 2.5f;

                Rectangle rect;
                rect.X = random.Next(0, item.Size.X * Tile.Width - 4);
                rect.Y = random.Next(0, item.Size.Y * Tile.Height - 4);
                rect.Width =  random.Next(4,Math.Min(rect.X + 10, (item.Size.X * Tile.Width)));
                rect.Height = random.Next(4,Math.Min(rect.Y + 10, (item.Size.Y * Tile.Height)));
                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 20, Color.White * .8f, size, ttl, speed, new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f),false, true) { AlphaFade = true, SourceRectangle = rect }); //return particle
            }
            else if (type == ParticleType.ItemFall)
            {
                Texture2D texture = ContentPack.Textures["particles\\item"]; //Get random texture
                Vector2 position = new Vector2(x + random.Next(-10, 11), y + random.Next(-10,11)); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-.6f, .6f), -RandomMinMax(.4f,.7f));

                float size = (float)random.NextDouble();
                int ttl = 40 + random.Next(30);
                float speed = size;
                if (color == Color.White)
                    color = item.MinimapColor;
                else
                    color = Color.Lerp(color, item.MinimapColor, .5f);

                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 3, color, size, ttl, speed, new Vector2(0.0f, -0.1f), new Vector2(0.0f, 0.0f), false, true)); //return particle
            }
        }
        /// <summary>
        /// Generates particles
        /// </summary>
        /// <returns>texture, position, velocity, angle, angularVelocity, color, size, ttl</returns>
        public void SpawnParticle(ParticleType type,int x,int y)
        {
            //Return if particle quality is lower
            if (Game.ParticleQuality == 1)
                if (random.NextDouble() >= .75)
                    return;
            if (Game.ParticleQuality == 0)
                if (random.NextDouble() >= .50)
                    return;
            if (type == ParticleType.LavaAmbient)
            {
                Texture2D texture = ContentPack.Textures["particles\\lavaambient"]; //Get random texture
                Vector2 position = new Vector2(x,y); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-.5f,.5f), -RandomMinMax(1.5f,3.2f));


                float size = (float)random.NextDouble() / 1.5f;
                int ttl = 75 + random.Next(50);
                float speed = size;


                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 3, Color.Orange, size, ttl, speed, new Vector2(0.0f, -0.04f), new Vector2(0.0f, 0.0f), false, true)); //return particle
            }

            else if (type == ParticleType.Fire)
            {
                Texture2D texture = ContentPack.Textures["particles\\fire"]; //Get random texture
                Vector2 position = new Vector2(x, y); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-0.2f, 0.2f), -RandomMinMax(.4f, .8f) * 2);


                float size = (float)random.NextDouble();
                int ttl = 50 + random.Next(50);
                float speed = size;


                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), .01f, Color.Orange, size, ttl, speed, new Vector2(0.0f, 0.005f), new Vector2(0.0f, 0.0f), false, true) { AlphaFade = true, EmitColor = true }); //return particle
            }
            else if (type == ParticleType.Torch)
            {
                Texture2D texture = ContentPack.Textures["particles\\fire"]; //Get random texture
                Vector2 position = new Vector2(x, y); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-0.2f, 0.2f), -RandomMinMax(.4f, .8f) * 2);


                float size = (float)random.NextDouble() / 2.6f;
                int ttl = 25 + random.Next(20);
                float speed = size;


                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), .01f, Color.Orange * .8f, size, ttl, speed, new Vector2(0.0f, 0.006f), new Vector2(0.0f, 0.0f), false, true) { AlphaFade = true, EmitColor = false }); //return particle
            }
            else if (type == ParticleType.RainSplash)
            {
                Texture2D texture = ContentPack.Textures["particles\\raindrop"]; //Get random texture
                Vector2 position = new Vector2(x, y - 3); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-.6f, .6f), -RandomMinMax(1f, 2f));

                float size = (float)random.NextDouble();
                int ttl = 25 + random.Next(30);
                float speed = size;

                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 3, Color.White, size, ttl, speed, new Vector2(0.0f, -0.1f), new Vector2(0.0f, 0.0f), false, true)); //return particle
            }
            else if (type == ParticleType.WaterSplash)
            {
                Texture2D texture = ContentPack.Textures["particles\\watersplash"]; //Get random texture
                Vector2 position = new Vector2(x, y - 3); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-.6f, .6f), -RandomMinMax(1f, 2f));

                float size = (float)random.NextDouble();
                int ttl = 25 + random.Next(30);
                float speed = size;

                Particles.Add(new Particle(Level, texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 3, Level.worldGen.CheckBiome(x / Tile.Width).WaterColor, size, ttl, speed, new Vector2(0.0f, -0.1f), new Vector2(0.0f, 0.0f), false, true)); //return particle
            }
            else if (type == ParticleType.LavaSplash)
            {
                Texture2D texture = ContentPack.Textures["particles\\lavasplash"]; //Get random texture
                Vector2 position = new Vector2(x, y - 3); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-.6f, .6f), -RandomMinMax(.5f, 1f));

                float size = (float)random.NextDouble(.5f,1f);
                int ttl = 55 + random.Next(30);
                float speed = size;

                Particles.Add(new Particle(Level, texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 3, Color.White * .7f, size, ttl, speed, new Vector2(0.0f, -0.1f), new Vector2(0.0f, 0.0f), false, true)); //return particle
            }
            else if (type == ParticleType.Rain)
            {
                Texture2D texture = ContentPack.Textures["particles\\rain"]; //Get random texture
                Vector2 position = new Vector2(x, y); //Set EmitterLocation

                // return the new velocity based on the angle
                float angle = MathHelper.ToRadians(random.NextFloat(-1,1));

                Vector2 velocity = Vector2.Zero;

                float size = (float)random.NextDouble() * 1.8f;

                float speed = RandomMinMax(2.3f, 2.8f);
                int ttl = 1200;

                Particles.Add(new Particle(Level, texture, type, position, velocity, angle, 0, Color.White, size, ttl, speed, new Vector2(0.0f, -random.Next(15, 25) / 10f), new Vector2(0f, 0.0f), true, false)); //return particle
            }
            else if (type == ParticleType.Snow)
            {
                Texture2D texture = ContentPack.Textures["particles\\snow"]; //Get random texture
                Vector2 position = new Vector2(x, y); //Set EmitterLocation

                Vector2 velocity = Vector2.Zero;

                float size = (float)random.NextFloat(.3f,.7f);

                float speed = RandomMinMax(.3f,.4f);
                int ttl = (int)(Game.MainWindow.ClientHeight * 3);

                Particles.Add(new Particle(Level, texture, type, position, velocity,0,0, Color.White, size, ttl, speed, new Vector2(0f, -random.Next(15, 25) / 100f), new Vector2(random.Next(-8, 8), 0.0f), true, false)); //return particle
            }
            else if (type == ParticleType.Blood)
            {
                Texture2D texture = ContentPack.Textures["particles\\blood"]; //Get random texture
                Vector2 position = new Vector2(x, y); //Set EmitterLocation

                // return the new velocity based on the angle
                Vector2 velocity = new Vector2(RandomMinMax(-1.3f, 1.3f), -RandomMinMax(.7f, 2.5f));

                float size = (float)random.NextDouble();
                int ttl = 50 + random.Next(35);
                float speed = size;

                Particles.Add(new Particle(Level,texture, type, position, velocity, MathHelper.ToRadians(random.Next(0, 360)), velocity.X / 3, Color.Lerp(Color.Red, new Color(200, 0, 0, 255), random.NextFloat(0, 1)), size, ttl, speed, new Vector2(0.0f, -random.NextFloat(0.14f, 0.16f)), new Vector2(0.0f, 0.0f), false, true)); //return particle
            }
         }
        /// <summary>
        /// Spawn blood particles X (interations) times.
        /// </summary>
        /// <param name="x">X Pos</param>
        /// <param name="y">Y Pos</param>
        /// <param name="iterations">Iterations (Amount)</param>
        public void SpawnBlood(int x, int y, int iterations)
        {
            SpawnParticles(x, y, ParticleType.Blood, iterations);
        }
        /// <summary>
        /// Spawn particles X (interations) times.
        /// </summary>
        public void SpawnParticles(int x, int y, ParticleType p,int iterations)
        {
            for (int i = 0; i < iterations; i++)
                SpawnParticle(p, x, y);
        }
        /// <summary>
        /// Get a random float between min and max
        /// </summary>
        /// <param name="min">the minimum random value</param>
        /// <param name="max">the maximum random value</param>
        /// <returns>float</returns>
        private float RandomMinMax(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        /// <summary>
        /// Update particles
        /// </summary>
        public void Update(GameTime gameTime)
        {
            Rectangle rect = new Rectangle((int)Level.MainCamera.Position.X - 400, (int)Level.MainCamera.Position.Y - 400, Game.MainWindow.Manager.GraphicsDevice.PresentationParameters.BackBufferWidth + 400, Game.MainWindow.Manager.GraphicsDevice.PresentationParameters.BackBufferHeight + 400);
            for (int particle = 0; particle < Particles.Count; particle++)
            {
                Particle p = Particles[particle];
                if (p == null)
                {
                    Particles.RemoveAt(particle);
                    particle--;
                    continue;
                }
                if (rect.Contains(p.Position.ToPoint()))
                {
                    p.Update(gameTime, Game.keyboardState, Game.oldKeyBoardState);  //Update each particle
                }
                else
                {
                    Particles.RemoveAt(particle);
                    particle--;
                    continue;
                }
                if (Particles[particle].TTL <= 0) //Remove old particles
                {
                    Particles.RemoveAt(particle);
                    particle--;
                }
            }
        }


        /// <summary>
        /// Tells each particle to draw itself
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw particles</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int index = 0; index < Particles.Count; index++)
            {
               Particle p = Particles[index];
               Rectangle rect = new Rectangle((int)Level.MainCamera.Position.X, (int)Level.MainCamera.Position.Y, spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth, spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight);
               if (rect.Contains(p.Position.ToPoint()))
                   p.Draw(gameTime, spriteBatch); //Draw each particle
            }
        }
    }
}
