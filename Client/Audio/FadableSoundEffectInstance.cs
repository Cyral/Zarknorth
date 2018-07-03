using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ZarknorthClient
{
    public sealed class FadableSoundEffectInstance
    {
        public float FadeSpeed;
        public AudioFadeState FadeState = AudioFadeState.Normal;
        public SoundEffectInstance BaseInstance;
        public FadableSoundEffectInstance(SoundEffect soundEffect, float fadeSpeed = .1f)
        {
            BaseInstance = soundEffect.CreateInstance();
            BaseInstance.Volume = 0;
            FadeSpeed = fadeSpeed;
        }
        public void FadeOut()
        {
            FadeState = AudioFadeState.Out;
        }
        public void FadeIn()
        {
            FadeState = AudioFadeState.In;
            if (BaseInstance.State != SoundState.Playing)
                BaseInstance.Play();
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (FadeState == AudioFadeState.In)
            {
                BaseInstance.Volume = Math.Min(1, BaseInstance.Volume + (elapsed * FadeSpeed));
                if (BaseInstance.Volume == 1)
                    FadeState = AudioFadeState.Normal;
            }
            else if (FadeState == AudioFadeState.Out)
            {
                BaseInstance.Volume = Math.Max(0, BaseInstance.Volume - (elapsed * FadeSpeed));
                if (BaseInstance.Volume == 0)
                {
                    FadeState = AudioFadeState.Normal;
                    if (BaseInstance.State != SoundState.Stopped)
                        BaseInstance.Stop();
                }
            }
        }

    }
    public enum AudioFadeState
    {
        Out,
        In,
        Normal,
    }
}
