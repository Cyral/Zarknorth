
#region //// Using /////////////

////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
////////////////////////////////////////////////////////////////////////////

#endregion

namespace ZarknorthClient.Interface
{
    public class TaskProfile : Dialog
    {
        #region Fields
        //Character Peices
        public Texture2D Arm, Body, Head, Leg;
        public Texture2D[] Hair, PantsLeg, ShirtSleeve, Shoe, ShirtBody, PantsTop, Expression;
        public int PantsLegVariation, HairVariation, ShirtSleeveVariation, ShoeVariation, ShirtBodyVariation, PantsTopVariation, ExpressionVariation;
        public static int HairTotal = 1, PantsLegTotal = 2, ShirtSleeveTotal = 2, ShoeTotal = 3, ShirtBodyTotal = 1, PantsTopTotal = 1, ExpressionTotal = 4;
        float LeftLegRotation, RightLegRotation, LeftArmRotation, RightArmRotation, HeadRotation;
        Vector2 HeadPosition;
        //Controls
        SpinBox EmotionSpn, ShirtSpn, PantsSpn, HairSpn, ShoesSpn;
        Label Emotionlbl, Shirtlbl, Pantslbl, Hairlbl, Shoeslbl;
        public ProfileColorPicker SkinColor, ShirtColor, PantsColor, HairColor, ShoeColor;
        Button Savebtn, Cancelbtn;
        #endregion
        #region Constructors
        public TaskProfile(Manager manager)
            : base(manager)
        {
            LoadContent();
            CreateColorPickers(manager);
            IO.LoadPlayerSkin(this);
            Resizable = false;
            BottomPanel.Top -= 16;
            Height = 256;
            Width = 550;
            Text = "Profile";
            Center();

            TopPanel.Visible = true;
            Caption.Text = "Profile - " + Game.UserName;
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);


            //Cant close form unless character is made
            bool CanClose = IO.ProfileExists();
            Savebtn = new Button(manager);
            Savebtn.Init();
            Savebtn.Top = 8;
            Savebtn.Left = CanClose ? (Width / 2) - Savebtn.Width - 4 : (Width / 2) - (Savebtn.Width / 2);
            Savebtn.Text = CanClose ? "Save" : "Create";
            Savebtn.Click += Savebtn_Click;
            BottomPanel.Add(Savebtn);
            if (CanClose)
            {
                Cancelbtn = new Button(manager);
                Cancelbtn.Init();
                Cancelbtn.Top = 8;
                Cancelbtn.Left = (Width / 2) + 4;
                Cancelbtn.Text = "Cancel";
                Cancelbtn.Click += Cancelbtn_Click;
                BottomPanel.Add(Cancelbtn);
            }
            else
            {
                CaptionVisible = false;   
                Movable = false;
                StayOnTop = true;
            }
            CloseButton.Enabled = CanClose;
            if (CanClose)
                Description.Text = "Customize your character's personality!";
            else
                Description.Text = "[color:Gold]Create your character to continue![/color]";

            Emotionlbl = new Label(manager);
            Emotionlbl.Init();
            Emotionlbl.Left = 16 + 8;
            Emotionlbl.Top = TopPanel.Height + ClientArea.Top - 24;
            Emotionlbl.Text = "Expression:";
            Emotionlbl.Width = 120;
            Emotionlbl.Alignment = Alignment.MiddleRight;
            Add(Emotionlbl);

            EmotionSpn = new SpinBox(manager, SpinBoxMode.List);
            EmotionSpn.Init();
            EmotionSpn.Top = TopPanel.Height + ClientArea.Top - 24;
            EmotionSpn.Left = 16 + 8 + 124;
            EmotionSpn.Items.Add("Neutral");
            EmotionSpn.Items.Add("Happy");
            EmotionSpn.Items.Add("Sad");
            EmotionSpn.Items.Add("Smile");
            EmotionSpn.Width = 100;
            EmotionSpn.ItemIndex = ExpressionVariation;
            EmotionSpn.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                ExpressionVariation = EmotionSpn.ItemIndex;
            });
            Add(EmotionSpn);

            Shirtlbl = new Label(manager);
            Shirtlbl.Init();
            Shirtlbl.Left = 16 + 8;
            Shirtlbl.Top = TopPanel.Height + ClientArea.Top - 2;
            Shirtlbl.Text = "Shirt Style:";
            Shirtlbl.Width = 120;
            Shirtlbl.Alignment = Alignment.MiddleRight;
            Add(Shirtlbl);

            ShirtSpn = new SpinBox(manager, SpinBoxMode.List);
            ShirtSpn.Init();
            ShirtSpn.Top = TopPanel.Height + ClientArea.Top - 2;
            ShirtSpn.Left = 16 + 8 + 124;
            ShirtSpn.Items.Add("Long");
            ShirtSpn.Items.Add("Short");
            ShirtSpn.Width = 100;
            ShirtSpn.ItemIndex = ShirtSleeveVariation;
            ShirtSpn.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                ShirtSleeveVariation = ShirtSpn.ItemIndex;
            });
            Add(ShirtSpn);

            Pantslbl = new Label(manager);
            Pantslbl.Init();
            Pantslbl.Left = 16 + 8;
            Pantslbl.Top = TopPanel.Height + ClientArea.Top + 20;
            Pantslbl.Text = "Pants Style:";
            Pantslbl.Width = 120;
            Pantslbl.Alignment = Alignment.MiddleRight;
            Add(Pantslbl);

            PantsSpn = new SpinBox(manager, SpinBoxMode.List);
            PantsSpn.Init();
            PantsSpn.Top = TopPanel.Height + ClientArea.Top + 20;
            PantsSpn.Left = 16 + 8 + 124;
            PantsSpn.Items.Add("Long");
            PantsSpn.Items.Add("Short");
            PantsSpn.Width = 100;
            PantsSpn.ItemIndex = PantsLegVariation;
            PantsSpn.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                PantsLegVariation = PantsSpn.ItemIndex;
            });
            Add(PantsSpn);

            Hairlbl = new Label(manager);
            Hairlbl.Init();
            Hairlbl.Left = 16 + 8;
            Hairlbl.Top = TopPanel.Height + ClientArea.Top + 42;
            Hairlbl.Text = "Hair Style:";
            Hairlbl.Width = 120;
            Hairlbl.Alignment = Alignment.MiddleRight;
            Add(Hairlbl);

            HairSpn = new SpinBox(manager, SpinBoxMode.List);
            HairSpn.Init();
            HairSpn.Top = TopPanel.Height + ClientArea.Top + 42;
            HairSpn.Left = 16 + 8 + 124;
            HairSpn.Items.Add("Undefined");
            HairSpn.Width = 100;
            HairSpn.ItemIndex = HairVariation;
            HairSpn.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                HairVariation = HairSpn.ItemIndex;
            });
            Add(HairSpn);

            Shoeslbl = new Label(manager);
            Shoeslbl.Init();
            Shoeslbl.Left = 16 + 8;
            Shoeslbl.Top = TopPanel.Height + ClientArea.Top + 64;
            Shoeslbl.Text = "Shoe Style:";
            Shoeslbl.Width = 120;
            Shoeslbl.Alignment = Alignment.MiddleRight;
            Add(Shoeslbl);

            ShoesSpn = new SpinBox(manager, SpinBoxMode.List);
            ShoesSpn.Init();
            ShoesSpn.Top = TopPanel.Height + ClientArea.Top + 64;
            ShoesSpn.Left = 16 + 8 + 124;
            ShoesSpn.Items.Add("Normal");
            ShoesSpn.Items.Add("Boots");
            ShoesSpn.Items.Add("Flat");
            ShoesSpn.Width = 100;
            ShoesSpn.ItemIndex = ShoeVariation;
            ShoesSpn.TextChanged += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                ShoeVariation = ShoesSpn.ItemIndex;
            });
            Add(ShoesSpn);

            //Have to ajust
            if (!CaptionVisible)
            {
                Emotionlbl.Top += 20;
                EmotionSpn.Top += 20;
                Shirtlbl.Top += 20;
                ShirtSpn.Top += 20;
                Pantslbl.Top += 20;
                PantsSpn.Top += 20;
                Shoeslbl.Top += 20;
                ShoesSpn.Top += 20;
                Hairlbl.Top += 20;
                HairSpn.Top += 20;
                Height -= 20;
            }
        }

        private void CreateColorPickers(Manager manager)
        {
            SkinColor = new ProfileColorPicker(manager, "Skin Color:",
                new Color(244, 184, 128),
                new Color(248, 197, 142),
                new Color(242, 205, 165),
                new Color(228, 165, 106),
                new Color(236, 178, 95),
                new Color(228, 180, 110),
                new Color(170, 134, 82),
                new Color(168, 125, 91),
                new Color(110, 79, 56),
                new Color(77, 57, 42)
                );
            SkinColor.Init();
            SkinColor.Left = 264;
            SkinColor.Top = TopPanel.Height + ClientArea.Top - 24;
            Add(SkinColor);

            ShirtColor = new ProfileColorPicker(manager, "Shirt Color:",
                new Color(205, 50, 50),
                new Color(79, 99, 124),
                new Color(54, 155, 64),
                new Color(50, 171, 205),
                new Color(255, 162, 54),
                new Color(131, 73, 164),
                new Color(160, 174, 89),
                new Color(168, 125, 91),
                new Color(255, 255, 255),
                new Color(89, 89, 89)
                );
            ShirtColor.Init();
            ShirtColor.Left = 264;
            ShirtColor.Top = TopPanel.Height + ClientArea.Top - 2;
            Add(ShirtColor);

            PantsColor = new ProfileColorPicker(manager, "Pants Color:",
                new Color(69, 135, 200),
                new Color(93, 123, 162),
                new Color(72, 89, 113),
                new Color(89, 130, 139),
                new Color(81, 126, 76),
                new Color(216, 193, 170),
                new Color(145, 120, 94),
                new Color(189, 120, 49),
                new Color(190, 190, 190),
                new Color(41, 41, 41)
                );
            PantsColor.Init();
            PantsColor.Left = 264;
            PantsColor.Top = TopPanel.Height + ClientArea.Top + 20;
            Add(PantsColor);

            HairColor = new ProfileColorPicker(manager, "Hair Color:",
                new Color(217, 176, 122),
                new Color(81, 76, 76),
                new Color(143, 113, 80),
                new Color(118, 85, 60),
                new Color(163, 141, 89),
                new Color(228, 180, 110),
                new Color(91, 56, 34),
                new Color(246, 226, 129),
                new Color(242, 235, 163),
                new Color(234, 234, 234)
                );
            HairColor.Init();
            HairColor.Left = 264;
            HairColor.Top = TopPanel.Height + ClientArea.Top + 42;
            Add(HairColor);

            ShoeColor = new ProfileColorPicker(manager, "Shoe Color:",
                 new Color(110, 70, 31),
                 new Color(222, 193, 162),
                 new Color(145, 120, 94),
                 new Color(189, 138, 85),
                 new Color(230, 118, 118),
                 new Color(118, 189, 230),
                 new Color(140, 230, 118),
                 new Color(220, 220, 220),
                 new Color(122, 122, 122),
                 new Color(41, 41, 41)
                 );
            ShoeColor.Init();
            ShoeColor.Left = 264;
            ShoeColor.Top = TopPanel.Height + ClientArea.Top + 64;
            Add(ShoeColor);
        }

        void Cancelbtn_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Close();
        }

        void Savebtn_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            IO.SavePlayerSkin(this);
            Close();
        }

        private void LoadContent()
        {
            Arm = ContentPack.Textures["entities\\player\\armSheet"];
            Body = ContentPack.Textures["entities\\player\\bodySheet"];
            Head = ContentPack.Textures["entities\\player\\headSheet"];
            Leg = ContentPack.Textures["entities\\player\\legSheet"];
            //JacketBody = ContentPack.Textures["entities\\player\\jacketBodySheet"];
            //JacketSleeve = ContentPack.Textures["entities\\player\\jacketSleeveSheet"];
            PantsLeg = new Texture2D[PantsLegTotal];
            for (int i = 1; i <= PantsLegTotal; i++)
                PantsLeg[i - 1] = ContentPack.Textures["entities\\player\\pantsLeg" + i];
            PantsTop = new Texture2D[PantsTopTotal];
            for (int i = 1; i <= PantsTopTotal; i++)
                PantsTop[i - 1] = ContentPack.Textures["entities\\player\\pantsTop" + i];
            ShirtBody = new Texture2D[ShirtBodyTotal];
            for (int i = 1; i <= ShirtBodyTotal; i++)
                ShirtBody[i - 1] = ContentPack.Textures["entities\\player\\shirtBody" + i];
            ShirtSleeve = new Texture2D[ShirtSleeveTotal];
            for (int i = 1; i <= ShirtSleeveTotal; i++)
                ShirtSleeve[i - 1] = ContentPack.Textures["entities\\player\\shirtSleeve" + i];
            Expression = new Texture2D[ExpressionTotal];
            for (int i = 1; i <= ExpressionTotal; i++)
                Expression[i - 1] = ContentPack.Textures["entities\\player\\expression" + i];
            Shoe = new Texture2D[ShoeTotal];
            for (int i = 1; i <= ShoeTotal; i++)
                Shoe[i - 1] = ContentPack.Textures["entities\\player\\shoe" + i];
        }
        #endregion
        #region Methods
        public override void Init()
        {
            base.Init();
        }
        public override void DrawControl(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
            base.DrawControl(renderer, rect, gameTime);
            DrawCharacter(renderer, rect, gameTime);
        }

        private void DrawCharacter(Renderer renderer, Rectangle rect, GameTime gameTime)
        {
      
            //Find positions and origins
            Vector2 Position = new Vector2(32, TopPanel.Height + ClientArea.Top + /* Character Height */ (Tile.Height * 3) + 18);
            Vector2 LegOrigin = new Vector2(Leg.Width / 2, Leg.Width / 2);
            Vector2 ArmOrigin = new Vector2(Arm.Width / 2, Arm.Width / 2);
            Vector2 HeadOrigin = new Vector2(Head.Width / 2, Head.Width / 2);
            HeadPosition = Position + new Vector2(-2, -66) + (HeadOrigin);

            //Draw BackArm
            renderer.SpriteBatch.Draw(Arm, Position + new Vector2(2, -43), null, SkinColor.SelectedColor, RightArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
            //Draw BackShirt
            renderer.SpriteBatch.Draw(ShirtSleeve[ShirtSleeveVariation], Position + new Vector2(2, -44), null, ShirtColor.SelectedColor, RightArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
            //Draw Backleg
            renderer.SpriteBatch.Draw(Leg, Position + new Vector2(5, -24), null, SkinColor.SelectedColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            //Draw BackPantsLeg
            renderer.SpriteBatch.Draw(PantsLeg[PantsLegVariation], Position + new Vector2(5, -24), null, PantsColor.SelectedColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            //Draw BackShoes
            renderer.SpriteBatch.Draw(Shoe[ShoeVariation], Position + new Vector2(4, -24), null, ShoeColor.SelectedColor, LeftLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            //Draw FrontLeg
            renderer.SpriteBatch.Draw(Leg, Position + new Vector2(9, -24), null, SkinColor.SelectedColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            //Draw FrontPantsLeg
            renderer.SpriteBatch.Draw(PantsLeg[PantsLegVariation], Position + new Vector2(9, -24), null, PantsColor.SelectedColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            //Draw FrontShoes
            renderer.SpriteBatch.Draw(Shoe[ShoeVariation], Position + new Vector2(8, -24), null, ShoeColor.SelectedColor, RightLegRotation, LegOrigin, 1f, SpriteEffects.None, 0);
            //Draw Body
            renderer.SpriteBatch.Draw(Body, Position + new Vector2(1, -53), null, SkinColor.SelectedColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //Draw Shirt
             renderer.SpriteBatch.Draw(ShirtBody[ShirtBodyVariation], Position + new Vector2(0, -48), null,  ShirtColor.SelectedColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //Draw PantsTop
            renderer.SpriteBatch.Draw(PantsTop[PantsTopVariation], Position + new Vector2(0, -26), null, PantsColor.SelectedColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //Draw FrontArm
            renderer.SpriteBatch.Draw(Arm, Position + new Vector2(11, -43), null, SkinColor.SelectedColor, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
            //Draw FrontShirt
            renderer.SpriteBatch.Draw(ShirtSleeve[ShirtSleeveVariation], Position + new Vector2(11, -44), null, ShirtColor.SelectedColor, LeftArmRotation, ArmOrigin, 1f, SpriteEffects.None, 0);
            //Draw Head and Face
            if (HeadRotation > MathHelper.ToRadians(90) && HeadRotation < MathHelper.ToRadians(270))
            {
                renderer.SpriteBatch.Draw(Head, HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor.SelectedColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
                renderer.SpriteBatch.Draw(Expression[ExpressionVariation], HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), Color.White, HeadRotation, HeadOrigin, 1f, SpriteEffects.FlipVertically, 0);
            }
            else
            {
                renderer.SpriteBatch.Draw(Head, HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), SkinColor.SelectedColor, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);
                renderer.SpriteBatch.Draw(Expression[ExpressionVariation], HeadPosition, new Rectangle(0, 0, Head.Width, Head.Height), Color.White, HeadRotation, HeadOrigin, 1f, SpriteEffects.None, 0);
            }
        }
        #endregion
    }
}
