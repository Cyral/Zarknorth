#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using Controls = TomShane.Neoforce.Controls;
using System.IO;
using ZarknorthClient;
using Microsoft.Xna.Framework.Input;
using ZarknorthClient.Music;
////////////////////////////////////////////////////////////////////////////

#endregion


namespace ZarknorthClient.Interface
{
    #region //// Fields ////////////
    public class TaskSynthesizer : Dialog
    {
        KeyboardState _currentKeyboardState;
        KeyboardState _previousKeyboardState;
        Texture2D blackKey;
        Texture2D whiteKey;
        Texture2D blackKeyPressed;
        Texture2D whiteKeyPressed;
        public ComboBox synths;
        public TrackBar fadeIn;
        public TrackBar fadeOut;
        public Label synthsLbl;
        public Label fadeInLbl;
        public Label fadeOutLbl;
        public static int count;
        public static int id;
        public static int target;
        public Synth _synth;
        public List<ImageBox> buttonList;
        private const int MaxNotes = 15;
        private string[] _letters = new[] { "A", "W", "S", "E", "D", "F", "T", "G", "Y", "H", "U", "J", "K", "O", "L" };
        private OscillatorTypes _oscillatorType = OscillatorTypes.Triangle;

        private enum OscillatorTypes
        {
            Sine = 0,
            Triangle = 1,
            Square = 2,
            Sawtooth = 3,
            Moag = 4,
            Bright_Organ = 5,
            Damped_Organ = 6,
            Warm_Organ = 7,
            Attacked_Sine = 8,
            Pulse = 9,
        }
        public void ApplyOscillator()
        {
            if (_oscillatorType == OscillatorTypes.Sine)
                _synth.Oscillator = Oscillator.Sine;
            else if (_oscillatorType == OscillatorTypes.Triangle)
                _synth.Oscillator = Oscillator.Triangle;
            else if (_oscillatorType == OscillatorTypes.Square)
                _synth.Oscillator = Oscillator.Square;
            else if (_oscillatorType == OscillatorTypes.Moag)
                _synth.Oscillator = Oscillator.Moag;
            else if (_oscillatorType == OscillatorTypes.Bright_Organ)
                _synth.Oscillator = Oscillator.OrganBright;
            else if (_oscillatorType == OscillatorTypes.Damped_Organ)
                _synth.Oscillator = Oscillator.OrganDamped;
            else if (_oscillatorType == OscillatorTypes.Warm_Organ)
                _synth.Oscillator = Oscillator.OrganWarm;
            else if (_oscillatorType == OscillatorTypes.Attacked_Sine)
                _synth.Oscillator = Oscillator.AttackedSine;
            else if (_oscillatorType == OscillatorTypes.Pulse)
                _synth.Oscillator = Oscillator.Pulse;
            else
                _synth.Oscillator = Oscillator.Sawtooth;
        }

        public void NextOscillatorType()
        {
            _oscillatorType = (OscillatorTypes)(((int)_oscillatorType + 1) % 10);
            ApplyOscillator();
        }


    #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////   
        public TaskSynthesizer(Manager manager)
            : base(manager)
        {

            int whiteKeyWidth = 24;
            int blackKeyWidth = 15;
            {
                int m = (15 / 12);
                int n = 15 % 12;


                Width = ((n + m * 7) * whiteKeyWidth + whiteKeyWidth) - 16;
                Height = 136 + 16 + (TopPanel.Height * 2);
            }
            //labelList[0].
            Text = "Music - Synthesizer";
            Center();
            buttonList = new List<ImageBox>();
            TopPanel.Visible = true;
            BottomPanel.Visible = false;
            Resizable = false;
            Caption.Text = "Welcome to the synthesizer!";
            Description.Text = "Use your keyboard to play!";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);
            _synth = new Synth();
            ApplyOscillator();
            whiteKey = ContentPack.Textures["gui\\pianoWhite"];
            blackKey = ContentPack.Textures["gui\\pianoBlack"];
            whiteKeyPressed = ContentPack.Textures["gui\\pianoWhitePressed"];
            blackKeyPressed = ContentPack.Textures["gui\\pianoBlackPressed"];

            synthsLbl = new Label(manager);
            synthsLbl.Init();
            synthsLbl.Top = 8 + TopPanel.Height;
            synthsLbl.Left = 8;
            synthsLbl.Width = 72;
            synthsLbl.Text = "Synthesizer:";
            Add(synthsLbl);

            synths = new ComboBox(manager);
            synths.Init();
            synths.Top = 6 + TopPanel.Height;
            synths.Left = synthsLbl.Left + synthsLbl.Width + 32;
            synths.Width = 110;
            Array values = Enum.GetValues(typeof(OscillatorTypes));
            foreach (OscillatorTypes val in values)
            synths.Items.Add(Enum.GetName(typeof(OscillatorTypes), val).Replace('_',' '));
            synths.ItemIndexChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
                        {
                            _oscillatorType = (OscillatorTypes)synths.ItemIndex;
                            ApplyOscillator();
                        });
            synths.ItemIndex = 0;
            Add(synths);
            fadeInLbl = new Label(manager);
            fadeInLbl.Init();
            fadeInLbl.Top = 8 + TopPanel.Height + synths.Height + 4;
            fadeInLbl.Left = 8;
            fadeInLbl.Width = 150;
            fadeInLbl.Text = "Fade In:";
            Add(fadeInLbl);
            fadeOutLbl = new Label(manager);
            fadeOutLbl.Init();
            fadeOutLbl.Top = 8 + TopPanel.Height + (synths.Height * 2) + 4;
            fadeOutLbl.Left = 8;
            fadeOutLbl.Width = 150;
             fadeOutLbl.Text = "Fade Out:";
            Add(fadeOutLbl);
            fadeIn = new TrackBar(manager);
            fadeIn.Init();
            fadeIn.Left = synths.Left;
            fadeIn.Top = fadeInLbl.Top;
            fadeIn.Range = 50000;
            fadeIn.Value = 0;
            fadeIn.Width = synths.Width;
            fadeIn.Color = Color.OrangeRed;
            Add(fadeIn);
            fadeOut = new TrackBar(manager);
            fadeOut.Init();
            fadeOut.Left = synths.Left;
            fadeOut.Top = fadeOutLbl.Top;
            fadeOut.Range = 50000;
            fadeOut.Value = 0;
            fadeOut.Width = synths.Width;
            fadeOut.Color = Color.OrangeRed;
            Add(fadeOut);

            for (int i = 0; i < MaxNotes; i++)
            {
                int m = (i / 12);
                int n = i % 12;

                // White Note
                if (n == 0 || n == 2 || n == 4 || n == 5 || n == 7 || n == 9 || n == 11)
                {
                    if (n == 2)
                    {
                        n = 1;
                    }
                    else if (n == 4)
                    {
                        n = 2;
                    }
                    else if (n == 5)
                    {
                        n = 3;
                    }
                    else if (n == 7)
                    {
                        n = 4;
                    }
                    else if (n == 9)
                    {
                        n = 5;
                    }
                    else if (n == 11)
                    {
                        n = 6;
                    }
                    buttonList.Add(new ImageBox(manager));
                    buttonList[i].Top = 76 + TopPanel.Height;
                    buttonList[i].Width = whiteKeyWidth;
                    buttonList[i].Height = 96;
                    buttonList[i].Init();
                    buttonList[i].Left = (n + m * 7) * whiteKeyWidth + 8;

                    buttonList[i].Image = whiteKey;
                    Add(buttonList[i]);

                }
                // Black Key
                else
                {
                    if (n == 1)
                    {
                        n = 0;
                    }
                    else if (n == 3)
                    {
                        n = 1;
                    }
                    else if (n == 6)
                    {
                        n = 3;
                    }
                    else if (n == 8)
                    {
                        n = 4;
                    }
                    else if (n == 10)
                    {
                        n = 5;
                    }
                    buttonList.Add(new ImageBox(manager));

                    buttonList[i].Init();
                    buttonList[i].Left = (n + m * 7) * whiteKeyWidth + whiteKeyWidth - blackKeyWidth / 2 + 8;
                    buttonList[i].Top = 76 + TopPanel.Height;
                    buttonList[i].Height = 72;
                    buttonList[i].Width = blackKeyWidth;
                    buttonList[i].Image = blackKey;
                    Add(buttonList[i]);
                }
            }
            for (int i = 0; i < MaxNotes; i++)
            {
                int m = (i / 12);
                int n = i % 12;

                // White Note
                if (n == 0 || n == 2 || n == 4 || n == 5 || n == 7 || n == 9 || n == 11)
                {
                }
                else
                {
                    buttonList[i].BringToFront();
                }
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
        protected override void Update(GameTime gameTime)
        {
            if (Visible)
            {
                _currentKeyboardState = Keyboard.GetState();
              
                if (_currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
                {
                    NextOscillatorType();
                    if (synths.ItemIndex < Enum.GetValues(typeof(OscillatorTypes)).Length - 1)
                        synths.ItemIndex++;
                    else
                        synths.ItemIndex = 0;
                }


                _synth.FadeInDuration = fadeIn.Value;
                _synth.FadeOutDuration = fadeOut.Value;
                fadeOutLbl.Text = "Fade Out: " + ((int)(1000 * _synth.FadeOutDuration / 44100.0f)).ToString() + "ms";
                fadeInLbl.Text = "Fade In: " + ((int)(1000 * _synth.FadeInDuration / 44100.0f)).ToString() + "ms";
                CheckNoteTrigger(Keys.A, 0);
                CheckNoteTrigger(Keys.W, 1);
                CheckNoteTrigger(Keys.S, 2);
                CheckNoteTrigger(Keys.E, 3);
                CheckNoteTrigger(Keys.D, 4);
                CheckNoteTrigger(Keys.F, 5);
                CheckNoteTrigger(Keys.T, 6);
                CheckNoteTrigger(Keys.R, 6);
                CheckNoteTrigger(Keys.G, 7);
                CheckNoteTrigger(Keys.Y, 8);
                CheckNoteTrigger(Keys.H, 9);
                CheckNoteTrigger(Keys.U, 10);
                CheckNoteTrigger(Keys.J, 11);
                CheckNoteTrigger(Keys.K, 12);
                CheckNoteTrigger(Keys.O, 13);
                CheckNoteTrigger(Keys.L, 14);

                _previousKeyboardState = _currentKeyboardState;

                _synth.Update(gameTime);
            }
            base.Update(gameTime);
        }
        private void CheckNoteTrigger(Keys key, int n)
        {
            if (_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key))
            {
                _synth.NoteOn(n);
                if (n == 0 || n == 2 || n == 4 || n == 5 || n == 7 || n == 9 || n == 11 || n == 12 || n == 14)
                {
                    buttonList[n].Image = whiteKeyPressed;
                }
                else
                    buttonList[n].Image = blackKeyPressed;


            }
            if (_currentKeyboardState.IsKeyUp(key) && _previousKeyboardState.IsKeyDown(key))
            {
                _synth.NoteOff(n);

                if (n == 0 || n == 2 || n == 4 || n == 5 || n == 7 || n == 9 || n == 11 || n == 12 || n == 14)
                {
                    buttonList[n].Image = whiteKey;
                }
                else
                    buttonList[n].Image = blackKey;

            }
        }
        ////////////////////////////////////////////////////////////////////////////   
        void btn_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {




        }
        #endregion


    }
}
