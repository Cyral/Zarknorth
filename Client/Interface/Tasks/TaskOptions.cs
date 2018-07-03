
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading;
using System.Configuration;
using Microsoft.Xna.Framework.Input;
using Cyral.Extensions;
#endregion

namespace ZarknorthClient.Interface
{
    public class TaskOptions : Dialog
    {

        #region Fields
        private GroupPanel GraphicsPnl, ControlsPnl, ContentPackPnl;
        private CheckBox FullscreenChk;
        private CheckBox DropShadowsChk;
        private CheckBox AntialiasingChk;
        private CheckBox TilesidesChk;
        private CheckBox AutosaveChk;
        private Label ParticleLbl;
        private Label SkinLbl;
        private ComboBox ParticleCmb;
        private ComboBox SkinCmb;
        private ComboBox LightingQualityCmb;
        private ComboBox LightingRefreshCmb;
        private Label LightingQualityLbl;
        private Label LightingRefreshLbl;
        private Button btnClose;
        private Button btnApply;
        private Button btnOk;
        private Label[] lblControls;
        private Button[] btnControls;
        private Button btnContentPack;
        #endregion

        #region Constructors

 
        public TaskOptions(Manager manager)
            : base(manager)
        {
            //Set up window UI
            MinimumHeight = 100;
            Height = Manager.ScreenHeight - 48;
            Width = 300;
            Resizable = false;
            Text = "Options";
            Caption.Text = "Information";
            Description.Text = "Customize and edit the game settings for your liking and best performance.\nHover over an option for more details.";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);
            
            //Custom UI Controls
            ContentPackPnl = new GroupPanel(Manager); //Panel for content pack settings
            ContentPackPnl.Init();
            ContentPackPnl.Parent = this;
            ContentPackPnl.Width = ClientWidth - 16;
            ContentPackPnl.ClientHeight = 32;
            ContentPackPnl.Left = 8;
            ContentPackPnl.Top = TopPanel.Height + 8;
            ContentPackPnl.Text = "Content Packs";

            btnContentPack = new Button(Manager);
            btnContentPack.Init();
            btnContentPack.Left = 8;
            btnContentPack.Top = 6;
            btnContentPack.Text =  Game.level != null ? "Choose Content Pack >>" : "Join World To Edit";
            btnContentPack.Width = 164;
            btnContentPack.Height = 18;
            btnContentPack.Click += btnContentPack_Click;
            btnContentPack.Enabled = Game.level != null;
            ContentPackPnl.Add(btnContentPack);

            GraphicsPnl = new GroupPanel(Manager); //Panel for graphics settings
            GraphicsPnl.Init();
            GraphicsPnl.Parent = this;
            GraphicsPnl.Width = ClientWidth - 16;
            GraphicsPnl.Height = 226;
            GraphicsPnl.Left = 8;
            GraphicsPnl.Top = ContentPackPnl.Height + ContentPackPnl.Top + 8;
            GraphicsPnl.Text = "General";

            ControlsPnl = new GroupPanel(Manager); //Panel for controls settings
            ControlsPnl.Init();
            ControlsPnl.Parent = this;
            ControlsPnl.Width = ClientWidth - 16;
            ControlsPnl.Height = 176;
            ControlsPnl.Left = GraphicsPnl.Left + GraphicsPnl.Width + 8;
            ControlsPnl.Top = ContentPackPnl.Top;
            ControlsPnl.Text = "Controls";

            ParticleLbl = new Label(manager);
            ParticleLbl.Init();
            ParticleLbl.Parent = GraphicsPnl;
            ParticleLbl.Left =  8;
            ParticleLbl.Top = 8;
            ParticleLbl.Width = 92;
            ParticleLbl.Anchor = Anchors.Left;
            ParticleLbl.Text = "Particle Quality:";
            ParticleLbl.ToolTip.Text = "Adjust the particle quality (amount)";


            ParticleCmb = new ComboBox(Manager);
            ParticleCmb.Init();
            ParticleCmb.Parent = GraphicsPnl;
            ParticleCmb.Left = ParticleLbl.Left + ParticleLbl.Width + 4 ;
            ParticleCmb.Top = 7;
            ParticleCmb.Width = 82;
            ParticleCmb.Height = 20;
            ParticleCmb.ReadOnly = true;
            ParticleCmb.Items.Add("Low");
            ParticleCmb.Items.Add("Medium");
            ParticleCmb.Items.Add("High");
            ParticleCmb.Movable = ParticleCmb.Resizable = false;
            ParticleCmb.OutlineMoving = ParticleCmb.OutlineResizing = false;
            ParticleCmb.ItemIndex = Game.ParticleQuality;

            SkinLbl = new Label(manager);
            SkinLbl.Init();
            SkinLbl.Parent = GraphicsPnl;
            SkinLbl.Left = 8;
            SkinLbl.Top = ParticleCmb.Top + ParticleCmb.Height + 5;
            SkinLbl.Width = 48;
            SkinLbl.Anchor = Anchors.Left;
            SkinLbl.Text = "UI Skin:";
            SkinLbl.ToolTip.Text = "Adjust the visual appearence of the user interface";
            SkinCmb = new ComboBox(Manager);
            SkinCmb.Init();
            SkinCmb.Parent = GraphicsPnl;
            SkinCmb.Left = SkinLbl.Left + SkinLbl.Width + 4;
            SkinCmb.Top = ParticleCmb.Top + ParticleCmb.Height + 4;
            SkinCmb.Width = 72;
            SkinCmb.Height = 20;
            SkinCmb.ReadOnly = true;
            SkinCmb.Items.Add("Red");
            SkinCmb.Items.Add("Blue");
            SkinCmb.Items.Add("Green");
            SkinCmb.MaxItems = 3;
            SkinCmb.Movable = SkinCmb.Resizable = false;
            SkinCmb.OutlineMoving = SkinCmb.OutlineResizing = false;
            SkinCmb.ItemIndex = Game.Skin;

            LightingQualityLbl = new Label(manager);
            LightingQualityLbl.Init();
            LightingQualityLbl.Parent = GraphicsPnl;
            LightingQualityLbl.Left = 8;
            LightingQualityLbl.Top = SkinCmb.Top + SkinCmb.Height + 6;
            LightingQualityLbl.Width = 96;
            LightingQualityLbl.Anchor = Anchors.Left;
            LightingQualityLbl.Text = "Lighting Quality:";
            LightingQualityLbl.ToolTip.Text = "Adjust the lighting quality\nLow: Simple, Original lighting, twice as fast as medium. However, there may be lighting glitches in complex areas such as caves\nMedium: 'Dual Pass' lighting, which will smooth out any inconsistancies\nHigh: Same as medium, however the lighting gets 'blurred' even more, reducing sharp edges and giving light a smoother look";
            
            LightingQualityCmb = new ComboBox(Manager);
            LightingQualityCmb.Init();
            LightingQualityCmb.Parent = GraphicsPnl;
            LightingQualityCmb.Left = LightingQualityLbl.Left + LightingQualityLbl.Width + 4;
            LightingQualityCmb.Top = SkinCmb.Top + SkinCmb.Height + 4;
            LightingQualityCmb.Width = 72;
            LightingQualityCmb.Height = 20;
            LightingQualityCmb.ReadOnly = true;
            LightingQualityCmb.Items.Add("Low");
            LightingQualityCmb.Items.Add("Medium");
            LightingQualityCmb.Items.Add("High");
            LightingQualityCmb.MaxItems = 3;
            LightingQualityCmb.Movable = LightingQualityCmb.Resizable = false;
            LightingQualityCmb.OutlineMoving = LightingQualityCmb.OutlineResizing = false;
            LightingQualityCmb.ItemIndex = Game.LightingQuality;

            LightingRefreshLbl = new Label(manager);
            LightingRefreshLbl.Init();
            LightingRefreshLbl.Parent = GraphicsPnl;
            LightingRefreshLbl.Left = 8;
            LightingRefreshLbl.Top = LightingQualityLbl.Top + LightingQualityLbl.Height + 6;
            LightingRefreshLbl.Width = 132;
            LightingRefreshLbl.Anchor = Anchors.Left;
            LightingRefreshLbl.Text = "Lighting Refresh Rate:";
            LightingRefreshLbl.ToolTip.Text = "Adjust the lighting refresh rate. Slow will cause less lighting re-calculations, thus less lag.\nHowever, fast will have better quality but cause more lag.";

            LightingRefreshCmb = new ComboBox(Manager);
            LightingRefreshCmb.Init();
            LightingRefreshCmb.Parent = GraphicsPnl;
            LightingRefreshCmb.Left = LightingQualityLbl.Left + LightingRefreshLbl.Width + 4;
            LightingRefreshCmb.Top = LightingQualityLbl.Top + LightingQualityLbl.Height + 4;
            LightingRefreshCmb.Width = 72;
            LightingRefreshCmb.Height = 20;
            LightingRefreshCmb.ReadOnly = true;
            LightingRefreshCmb.Items.Add("Slow");
            LightingRefreshCmb.Items.Add("Default");
            LightingRefreshCmb.Items.Add("Fast");
            LightingRefreshCmb.MaxItems = 3;
            LightingRefreshCmb.Movable = LightingQualityCmb.Resizable = false;
            LightingRefreshCmb.OutlineMoving = LightingQualityCmb.OutlineResizing = false;
            LightingRefreshCmb.ItemIndex = Game.LightingRefresh;


             FullscreenChk = new CheckBox(Manager);
             FullscreenChk.Parent = GraphicsPnl;
             FullscreenChk.Init();
             FullscreenChk.Left = 8;
             FullscreenChk.Width = 150;
             FullscreenChk.Height = ParticleCmb.Height;
             FullscreenChk.Text = "Fullscreen Mode";
             FullscreenChk.Checked = Game.Fullscreen;
             FullscreenChk.Top = LightingRefreshCmb.Top + LightingRefreshCmb.Height + 4;

             DropShadowsChk = new CheckBox(Manager);
             DropShadowsChk.Parent = GraphicsPnl;
             DropShadowsChk.Init();
             DropShadowsChk.Left = 8;
             DropShadowsChk.Width = 150;
             DropShadowsChk.Height = FullscreenChk.Height;
             DropShadowsChk.Text = "Drop Shadows";
             DropShadowsChk.ToolTip.Text = "Gives blocks and collectables small shadows to display a 3D effect (Recommended: On for quality, off for performance)";
             DropShadowsChk.Top = FullscreenChk.Top + FullscreenChk.Height ;
             DropShadowsChk.Checked = Game.DropShadows;

             TilesidesChk = new CheckBox(Manager);
             TilesidesChk.Parent = GraphicsPnl;
             TilesidesChk.Init();
             TilesidesChk.Left = 8;
             TilesidesChk.Width = 150;
             TilesidesChk.Height = FullscreenChk.Height;
             TilesidesChk.Text = "Tile Edges";
             TilesidesChk.Checked = Game.TileEdges;
             TilesidesChk.ToolTip.Text = "Makes edges of tiles differ from normal tiles, (Recommended: On for quality, Decreases peformance)";
             TilesidesChk.Top = DropShadowsChk.Top + DropShadowsChk.Height;

             AntialiasingChk = new CheckBox(Manager);
             AntialiasingChk.Parent = GraphicsPnl;
             AntialiasingChk.Init();
             AntialiasingChk.Left = 8;
             AntialiasingChk.Width = 150;
             AntialiasingChk.Height = FullscreenChk.Height;
             AntialiasingChk.Text = "Anti-aliasing";
             AntialiasingChk.Checked = Game.Antialiasing;
             AntialiasingChk.ToolTip.Text = "Smooths out jagged edges on lines, NOTE: Disable this if text/graphics appear extremly blurred. (Recommended: On for quality, Decreases peformance)";
             AntialiasingChk.Top = TilesidesChk.Top + TilesidesChk.Height;

             AutosaveChk = new CheckBox(Manager);
             AutosaveChk.Parent = GraphicsPnl;
             AutosaveChk.Init();
             AutosaveChk.Left = 8;
             AutosaveChk.Width = 150;
             AutosaveChk.Height = AntialiasingChk.Height;
             AutosaveChk.Text = "Autosave";
             AutosaveChk.Checked = Game.Autosave;
             AutosaveChk.ToolTip.Text = "Autosaves the level every 10 minutes to prevent data loss (Recommended: On)";
             AutosaveChk.Top = AntialiasingChk.Top + AntialiasingChk.Height;

             btnOk = new Button(manager);
             btnOk.Init();
             btnOk.Parent = BottomPanel;
             btnOk.Anchor = Anchors.Top | Anchors.Right;
             btnOk.Top = btnOk.Parent.ClientHeight - btnOk.Height - 8;
             btnOk.Left = btnOk.Parent.ClientWidth - 8 - btnOk.Width * 3 - 8;
             btnOk.Text = "OK";
             btnOk.ModalResult = ModalResult.Ok;
             btnOk.Click += new TomShane.Neoforce.Controls.EventHandler(btnApply_Click);

             btnApply = new Button(manager);
             btnApply.Init();
             btnApply.Parent = BottomPanel;
             btnApply.Anchor = Anchors.Top | Anchors.Right;
             btnApply.Top = btnOk.Parent.ClientHeight - btnOk.Height - 8;
             btnApply.Left = btnOk.Parent.ClientWidth - 4 - btnOk.Width * 2 - 8;
             btnApply.Text = "Apply";
             btnApply.Click += new TomShane.Neoforce.Controls.EventHandler(btnApply_Click);

             btnClose = new Button(manager);
             btnClose.Init();
             btnClose.Parent = BottomPanel;
             btnClose.Anchor = Anchors.Top | Anchors.Right;
             btnClose.Top = btnOk.Parent.ClientHeight - btnClose.Height - 8;
             btnClose.Left = btnOk.Parent.ClientWidth - btnClose.Width - 8;
             btnClose.Text = "Cancel";
             btnClose.ModalResult = ModalResult.Cancel;

             lblControls = new Label[Game.Controls.Count];
             btnControls = new Button[Game.Controls.Count];
             for (int i = 0; i < Game.Controls.Count; i++)
             {
                 Label l = new Label(manager);
                 l.Init();
                 l.Parent = ControlsPnl;
                 l.Left = 8;
                 l.Top = 2 + ((l.Height + 6) * i);
                 l.Width = 128;
                 l.Text = Game.Controls.ElementAt(i).Key;
                 lblControls[i] = l;

                 Button b = new Button(manager);
                 b.Init();
                 b.Parent = ControlsPnl;
                 b.Left = ControlsPnl.ClientWidth - 8 - b.Width;
                 b.Top = 2 + ((l.Height + 6) * i);
                 b.Height = 18;
                 b.Text = Game.Controls.ElementAt(i).Value.ToString().AddSpacesToSentence();
                 b.Click += b_Click;
                 btnControls[i] = b;
             }

             ControlsPnl.ClientHeight = btnControls[btnControls.Length - 1].Height + btnControls[btnControls.Length - 1].Top + 4;
             Height = ControlsPnl.Top + ControlsPnl.Height + BottomPanel.Height + 48;

             TopPanel.Visible = true;
             BottomPanel.Visible = true;
             BottomPanel.Top = ClientHeight - BottomPanel.Height;

             ClientWidth = ControlsPnl.Left + ControlsPnl.Width + 8;
             Center();
        }

        void btnContentPack_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            if (Game.level != null)
            {
                TaskContentPacks wnd = new TaskContentPacks(Game.level.game.Manager);
                wnd.Closed += wnd_Closed;
                wnd.Init();
                wnd.Show();
                Manager.Add(wnd);
                btnContentPack.Enabled = false;
                this.StayOnBack = true;
            }
        }

        void wnd_Closed(object sender, WindowClosedEventArgs e)
        {
            btnContentPack.Enabled = true;
            this.StayOnBack = false;
        }
        private int selected = -1;
        void b_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            //Re-Enabled old key
            if (selected >= 0)
            {
                btnControls[selected].Text = Game.Controls.ElementAt(selected).Value.ToString();
                btnControls[selected].Enabled = true;
            }

            //Find clicked key
            for (int i = 0; i < Game.Controls.Count; i++)
            {
                if (btnControls[i] == sender)
                    selected = i;
            }
            btnControls[selected].Enabled = false;
            btnControls[selected].Text = "Press Key";
        }
        protected override void Update(GameTime gameTime)
        {
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            if (keys.Length > 0 && selected != -1)
            {
                Game.Controls[lblControls[selected].Text] = keys[0];
                btnControls[selected].Text = Game.Controls.ElementAt(selected).Value.ToString().AddSpacesToSentence();
                btnControls[selected].Enabled = true;
                selected = -1;
            }
            base.Update(gameTime);
        }
        #endregion

        #region Methods
        void btnApply_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {
            Game.ParticleQuality = ParticleCmb.ItemIndex;
            Game.Skin = SkinCmb.ItemIndex;
            if (Game.Skin == 0 && Manager.Skin.Name != "Red")
                Manager.SetSkin("Red");
            if (Game.Skin == 1 && Manager.Skin.Name != "Blue")
                Manager.SetSkin("Blue");
            if (Game.Skin == 2 && Manager.Skin.Name != "Green")
                Manager.SetSkin("Green");
            //Let the game know of the changes
            Game.LightingQuality = LightingQualityCmb.ItemIndex;
            Game.TileEdges = TilesidesChk.Checked;
            Game.DropShadows = DropShadowsChk.Checked;
            Game.Antialiasing = AntialiasingChk.Checked;
            Game.Fullscreen = FullscreenChk.Checked;
            Game.LightingRefresh = LightingRefreshCmb.ItemIndex;
            Game.Autosave = AutosaveChk.Checked;

            //Recompute lighting if level is open
            if (Game.level != null && Game.level.ready)
                Game.level.ComputeLighting = true;

            (Manager.Game as Game).ApplyResolution();

            try
            {
                //Set the config file
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
                config.AppSettings.Settings["ParticleQuality"].Value = Game.ParticleQuality.ToString();
                config.AppSettings.Settings["Fullscreen"].Value = Game.Fullscreen.ToString();
                config.AppSettings.Settings["Antialiasing"].Value = Game.Antialiasing.ToString();
                config.AppSettings.Settings["TileEdges"].Value = Game.TileEdges.ToString();
                config.AppSettings.Settings["Autosave"].Value = Game.Autosave.ToString();
                config.AppSettings.Settings["DropShadows"].Value = Game.DropShadows.ToString();
                config.AppSettings.Settings["LightingQuality"].Value = Game.LightingQuality.ToString();
                config.AppSettings.Settings["LightingRefresh"].Value = Game.LightingRefresh.ToString();
                config.AppSettings.Settings["Skin"].Value = Game.Skin.ToString();
                config.AppSettings.Settings["ContentPack"].Value = Game.ContentPackName;

                //Save controls
                for (int i = 0; i < Game.Controls.Count(); i++)
                {
                    config.AppSettings.Settings[Game.Controls.ElementAt(i).Key].Value = Game.Controls.ElementAt(i).Value.ToString().RemoveWhitespace();
                }
                config.Save(ConfigurationSaveMode.Modified);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Assert(false, ex.Message);
            }
        }
        #endregion
    }
}
