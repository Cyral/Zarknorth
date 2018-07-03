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
////////////////////////////////////////////////////////////////////////////

#endregion


namespace ZarknorthClient.Interface
{
    #region //// Fields ////////////
    public class TaskCreatePortal : Dialog
    {


        private GroupPanel grpportal;
        private SpinBox spnID;
        private SpinBox spnTarget;
        private Label ID;
        private Label Target;
        private Button btn = null;
        public static int count;
        public static int id;
        public static int target;



    #endregion

        #region //// Constructors //////

        ////////////////////////////////////////////////////////////////////////////   
        public TaskCreatePortal(Manager manager)
            : base(manager)
        {
            Height = 256;
            Width = 200;

            Text = "Create new Portal";
            Center();

            TopPanel.Visible = true;
            Caption.Text = "Creating Portals";
            Description.Text = "Set a portal ID, and a TargetID (A portal goes to its Target)";
            Caption.TextColor = Description.TextColor = new Color(96, 96, 96);

            grpportal = new GroupPanel(Manager);
            grpportal.Init();
            grpportal.Parent = this;
            grpportal.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            grpportal.Width = ClientWidth - 16;
            grpportal.Height = 96;
            grpportal.Left = 8;
            grpportal.Top = TopPanel.Height + 8;
            grpportal.Text = "";


            ID = new Label(Manager);
            ID.Init();
            ID.Width = 48;
            ID.Top = 8;
            ID.Left = 8;
            ID.Height = 20;
            ID.Text = "ID:";
            grpportal.Add(ID);

            Target = new Label(Manager);
            Target.Init();
            Target.Width = 48;
            Target.Top = ID.Top + ID.Height + 8;
            Target.Left = 8;
            Target.Height = 20;
            Target.Text = "Target:";
            grpportal.Add(Target);



            spnID = new SpinBox(manager, SpinBoxMode.List);
            spnID.Init();
            spnID.Parent = grpportal;
            spnID.Left = ID.Left + ID.Width + 8;
            spnID.Top = 8;
            spnID.Width = 100;
            spnID.Height = 20;
            spnID.Text = "ID";
            spnID.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            for (int i = 0; i < 1000; i++)
                spnID.Items.Add(i);
            spnID.ItemIndex = 1;
            spnID.Mode = SpinBoxMode.List;

            grpportal.Add(spnID);
            spnTarget = new SpinBox(manager, SpinBoxMode.List);
            spnTarget.Init();
            spnTarget.Parent = grpportal;
            spnTarget.Left = ID.Left + ID.Width + 8;
            spnTarget.Top = spnID.Top + spnID.Height + 8;
            spnTarget.Width = 100;
            spnTarget.Height = 20;
            spnTarget.Text = "Target";
            spnTarget.Anchor = Anchors.Left | Anchors.Top | Anchors.Right;
            for (int i = 0; i < 1000; i++)
                spnTarget.Items.Add(i);
            spnTarget.ItemIndex = 1;
            spnTarget.Mode = SpinBoxMode.List;

            grpportal.Add(spnTarget);

       

            btn = new Button(manager);
            btn.Init();
            btn.Parent = BottomPanel;
            btn.Left = 8;
            btn.Top = 8;
            btn.Width = 96;
            btn.Text = "Create Portal";
            btn.Click += new TomShane.Neoforce.Controls.EventHandler(btn_Click);
            btn.TextColor = Color.White;


        }
        ////////////////////////////////////////////////////////////////////////////

        #endregion

        #region //// Methods ///////////

        ////////////////////////////////////////////////////////////////////////////    
        public override void Init()
        {
            base.Init();
        }
        ////////////////////////////////////////////////////////////////////////////   
        void btn_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
        {

            Manager.Cursor = Manager.Skin.Cursors["Busy"].Resource;
            if (spnID.Text == "")
            {
                spnID.Color = Color.Red;
            }
            else if (spnTarget.Text == "")
            {
                spnTarget.Color = Color.Red;
            }
            else if (spnID.Text != "" || spnTarget.Text != "")
            {
                id = Convert.ToInt32(spnID.Text);
                target = Convert.ToInt32(spnTarget.Text);

                Manager.Cursor = Manager.Skin.Cursors["Default"].Resource;
                this.Hide();
            }


        }
        #endregion


    }
}
