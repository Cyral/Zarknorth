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
    public class TaskMusic : Dialog
    {
        KeyboardState _currentKeyboardState;
        KeyboardState _previousKeyboardState;
        Texture2D blackKey;
        Texture2D whiteKey;
        Texture2D blackKeyPressed;
        Texture2D whiteKeyPressed;
        public SpinBox fadeOut;
        public static int count;
        public static int id;
        public static int target;
        public Synth _synth;
        public List<ImageBox> buttonList;
        private const int MaxNotes = 15;
        private string[] _letters = new[] { "A", "W", "S", "E", "D", "F", "T", "G", "Y", "H", "U", "J", "K", "O", "L" };

        private enum OscillatorTypes
        {
            Sine,
            Triangle,
            Square,
            Sawtooth,
            Moag,
            OrganBright,
            OrganDamped,
            OrganWarm,
            AttackedSine
        }
        



    #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////   
        public TaskMusic(Manager manager)
            : base(manager)
        {

            int whiteKeyWidth = 24;
            int blackKeyWidth = 15;
            {
                int m = (15 / 12);
                int n = 15 % 12;


                Width = ((n + m * 7) * whiteKeyWidth + whiteKeyWidth) - 16;
                Height = 72 + 16 + (TopPanel.Height * 2);
            }
            //labelList[0].
            Text = "Music - Piano";
            Center();
            buttonList = new List<ImageBox>();
            TopPanel.Visible = true;
            BottomPanel.Visible = false;
            Resizable = false;
            Caption.Text = "Welcome to the piano!";
            Description.Text = "Use your keyboard to play!";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);
            _synth = new Synth();

            whiteKey = ContentPack.Textures["gui\\pianoWhite"];
            blackKey = ContentPack.Textures["gui\\pianoBlack"];
            whiteKeyPressed = ContentPack.Textures["gui\\pianoWhitePressed"];
            blackKeyPressed = ContentPack.Textures["gui\\pianoBlackPressed"];



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
                    buttonList[i].Top = 8 + TopPanel.Height;
                    buttonList[i].Width = whiteKeyWidth;
                    buttonList[i].Height = 96;
                    buttonList[i].Init();
                    buttonList[i].Left = (n + m * 7 ) * whiteKeyWidth + 8;

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
                    buttonList[i].Top = 8 + TopPanel.Height;
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
                 Achievement.Show(Achievement.Music);
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
