namespace ZarknorthClient.Music
{
    using System;

    public delegate float OscillatorDelegate(float frequency, float time);
    /// <summary>
    /// Class for handling the way diffent synths sound
    /// </summary>
    public static class Oscillator
    {
        public static float Sine(float frequency, float time)
        {
            return (float)Math.Sin(frequency * time * 2 * Math.PI);
        }
        public static float Square(float frequency, float time)
        {
            return Sine(frequency, time) >= 0 ? 1.0f : -1.0f;
        }
        public static float Sawtooth(float frequency, float time)
        {
            return (float)(2 * (time * frequency - Math.Floor(time * frequency + 0.5)));
        }
        public static float Triangle(float frequency, float time)
        {
            return Math.Abs(Sawtooth(frequency, time)) * 2.0f - 1.0f;
        }
        public static float Moag(float frequency, float time)
        {
            return Microsoft.Xna.Framework.MathHelper.SmoothStep(Sine(frequency, time), Triangle(frequency, time), time);
        }
     
        public static float OrganBright(float frequency, float time)
        {
            return (Sine(frequency * 16, time) +
            Sine(frequency * 8, time) +
            Sine(frequency * 4, time) +
            Sine(frequency * 2, time) +
            Sine(frequency, time)) / 5;
        }

        public static float OrganDamped(float frequency, float time)
        {
            return (Sine(frequency, time) +
            Sine(frequency * 2, time) +
            Sine(frequency * 3, time) +
            Sine(frequency * 4, time) +
            Sine(frequency * 5, time)) / 5;
        }

        // and my favourite:
        public static float OrganWarm(float frequency, float time)
        {
            return (Sine(frequency, time) +
            Sine(frequency / 2, time) +
            Sine(frequency * 2, time) +
            Sine(frequency / 4, time) +
            Sine(frequency * 4, time)) / 5;
        }

        private static Random random = new Random();
        // Combine this with others…
        public static float AttackedSine(float frequency, float time)
        {
            return (float)(((random.NextDouble() - 0.5f) / time * 0.003f) + Sine(frequency, time));
        }
        public static float Pulse(float frequency, float time)
        {
         
            double period = 1.0 / frequency;
            double timeModulusPeriod = time - Math.Floor(time / period) * period;
            double phase = timeModulusPeriod / period;
            if (phase <= .1f)
                return 1;
            else
                return -1;
        }
    }
}
