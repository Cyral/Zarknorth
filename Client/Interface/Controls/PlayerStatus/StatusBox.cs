using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class StatusBox : GradientPanel
    {
        #region Properties
        public int FinalTop;
        public float top;
        public int DepletionPerSecond;
        public float AbsValue;
        #endregion

        #region Controls
        public Label Caption;
        public Label Description;
        public ProgressBar Progress;
        #endregion

        public StatusBox(Manager manager)
            : base(manager)
        {
            Passive = false;
            SendToBack();
            Width = 200;
            Anchor = Anchors.Horizontal | Anchors.Bottom;

            Caption = new Label(Manager);
            Caption.Init();
            Caption.Top = 4;
            Caption.Alignment = Alignment.MiddleCenter;
            Caption.Anchor = Anchors.Top | Anchors.Horizontal;
            Caption.Font = FontSize.Default14;
            Caption.Height = 24;
            Caption.Left = (ClientWidth / 2) - (Caption.Width / 2);
            Caption.TextChanged += Caption_TextChanged;
            Add(Caption);
            
            Description = new Label(Manager);
            Description.Init();
            Description.Top = Caption.Top + 24;
            Description.Alignment = Alignment.MiddleCenter;
            Description.Anchor = Anchors.Top | Anchors.Horizontal;
            Description.Height = 17;
            Description.Left = (ClientWidth / 2) - (Description.Width / 2);
            Description.TextChanged += Description_TextChanged;
            Add(Description);

            Progress = new ProgressBar(Manager);
            Progress.Init();
            Progress.Top = Description.Height + Description.Top;
            Progress.Width = ClientWidth - 16;
            Progress.Height = 16;
            Progress.Left = (ClientWidth / 2) - (Progress.Width / 2);
            Progress.Value = 100;
            AbsValue = Progress.Value;
           // Progress.Anchor = Anchors.Bottom | Anchors.Horizontal;
            Add(Progress);
            Height = Progress.Top + Progress.Height + 8;
            FinalTop = manager.TargetHeight - Height - 16;
            Top = manager.TargetHeight;
            top = Top;
            //TODO: Add Initialization logic and controls
        }
        public override void Init()
        {
            Left = (Manager.TargetWidth / 2) - (Width / 2);
            base.Init();
        }
        void Description_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Description.Width = (int)Manager.Skin.Fonts[0].Resource.MeasureRichString(Description.Text, Manager).X;
            Description.Left = (ClientWidth / 2) - (Description.Width / 2);
        }
        void Caption_TextChanged(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Caption.Width = (int)Manager.Skin.Fonts["Default14"].Resource.MeasureRichString(Caption.Text, Manager).X;
            Caption.Left = (ClientWidth / 2) - (Caption.Width / 2);
        }
        public bool Hiding;
        protected override void Update(GameTime gameTime)
        {
            if (Top != FinalTop && !Hiding)
            {
                top = (float)MathHelper.Lerp(top,FinalTop, 0.07f);
                Top = (int)top;
            }
            else if (Top != FinalTop && Hiding)
            {
                top = (float)MathHelper.Lerp(top, FinalTop, 0.07f);
                Top = (int)top;
            }
            if (Top + 1 == FinalTop && Hiding)
            {
                CloseFinal();
            }
            if (!Hiding)
            {
                Deflate(gameTime);
            }
            if (Progress.Value == 0 && !Hiding)
            {
                Close();
                Depleted();
               
            }
            base.Update(gameTime);
        }

        public virtual void Deflate(GameTime gameTime)
        {
            AbsValue -= (float)gameTime.ElapsedGameTime.TotalSeconds * DepletionPerSecond;
            Progress.Value = (int)AbsValue;
        }
        public virtual void Depleted()
        {
        }
        public void Close()
        {
            Hiding = true;
            FinalTop = Manager.TargetHeight;
        }
        public void CloseFinal()
        {
            Manager.Remove(this);
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            base.DrawControl(renderer,rect,gameTime);
        }
    }
}
