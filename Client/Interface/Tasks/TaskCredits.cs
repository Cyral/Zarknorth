
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
  public class TaskCredits: Dialog
  {
    
    #region //// Fields ////////////
    private ImageBox Image;
    private Dictionary<string, string> staff = new Dictionary<string, string>();
    private Dictionary<string, string> extras = new Dictionary<string, string>();
    private Label lblExtra;
    private List<Label> extraLabels = new List<Label>();
    #endregion

    #region //// Constructors //////

    ////////////////////////////////////////////////////////////////////////////   
    public TaskCredits(Manager manager): base(manager)
    {

      Height = 400;
      Width = 400; 
      MinimumHeight = 100;  
      MinimumWidth = 100;
      Text = "Credits";
      Center();
      BottomPanel.Visible = false;
      Resizable = false;
      TopPanel.Visible = true;
      Caption.Text = "";
     
      Description.Text = "The people and services that made this game possible!";
      Caption.TextColor = Description.TextColor = new Color(96, 96, 96);

      Image = new ImageBox(manager);
      Image.Init();
      Image.Left = (ClientWidth / 2) - (Image.Width / 2);
      Image.Top = 2;
      Image.Image = ContentPack.Textures["gui\\credits"];
      Image.Width = Image.Image.Width;
      Image.Height = Image.Image.Height;
      TopPanel.Add(Image);
    
      Description.Alignment = Alignment.BottomCenter;
      Description.Anchor = Anchors.Left | Anchors.Bottom | Anchors.Right;
      Description.Top = Image.Top + Image.Height;
      TopPanel.Height = (Image.Top * 2) + Image.Height + Description.Height + 8;
     // ClientWidth = (Image.btnLeft * 2) + Image.Width;

      staff.Add("Cyral", "[color:Gold]Lead Developer[/color]");
      staff.Add("Pugmatt", "[color:Gold]Developer[/color]");
      staff.Add("Fer22f", "[color:LightSeaGreen]Contributing Dev[/color]");
      staff.Add("Kentiya", "[color:Cyan]Contributing Artist[/color]");
      int i = 0;
      foreach (KeyValuePair<string, string> kv in staff)
      {
          Label l2 = new Label(manager);
          l2.Init();
          l2.Text = kv.Key;
          l2.Font = FontSize.Default14;
          l2.Width = 128;
          l2.Alignment = Alignment.MiddleCenter;
          l2.Top = TopPanel.Height;
          l2.Height = 24;
          l2.Left = i * l2.Width;
          Label l = new Label(manager);
          l.Init();
          l.Text = kv.Value;
          l.Width = 128;
          l.Alignment = Alignment.MiddleCenter;
          l.Top = l2.Top + l2.Height;
          l.Left = i * l.Width;

      
          Add(l2);
          Add(l);
          i++;
      }

      Width = staff.Count * 128 + 16;
      Image.Left = (ClientWidth / 2) - (Image.Width / 2);
      lblExtra = new Label(manager);
      lblExtra.Init();
      lblExtra.Text = "Other Resources:";
      lblExtra.Width = (int)Manager.Skin.Fonts[0].Resource.MeasureString(lblExtra.Text).X;
     // lblExtra.Alignment = Alignment.MiddleCenter;
     // lblExtra.Anchor = Anchors.btnLeft | Anchors.btnRight;
      lblExtra.Top = TopPanel.Height + 58;
      lblExtra.Left = (ClientWidth / 2) - (lblExtra.Width / 2);
      Add(lblExtra);
      extras.Add("SmittyW - Former Graphics Artist.", "");
      extras.Add("\"Erdie\" - Thunder Audio. (Remixed, under CC by 3.0 license)", "http://freesound.org/people/Erdie/sounds/23222/");
      extras.Add("Everyone who has supported and contributed to Zarknorth through it's development!","");
      i = 0;
      foreach (KeyValuePair<string, string> kv in extras)
      {
          extraLabels.Add(new Label(manager));
          extraLabels[extraLabels.Count- 1].Init();
          extraLabels[extraLabels.Count - 1].Text = kv.Key;
          extraLabels[extraLabels.Count - 1].Width = (int)Manager.Skin.Fonts[0].Resource.MeasureString(extraLabels[extraLabels.Count - 1].Text).X;
         // extraLabels[extraLabels.Count - 1].Alignment = Alignment.BottomCenter;
          //extraLabels[extraLabels.Count - 1].Anchor = Anchors.btnLeft | Anchors.btnRight | Anchors.Bottom;
          extraLabels[extraLabels.Count - 1].Left = (ClientWidth / 2) - (extraLabels[extraLabels.Count - 1].Width / 2);
          extraLabels[extraLabels.Count - 1].Top = lblExtra.Top + (12 * (i + 1));
          if (kv.Value != string.Empty)
          {
              extraLabels[extraLabels.Count - 1].Click += l_Click;
              extraLabels[extraLabels.Count - 1].MouseOver += TaskCredits_MouseOver;
              extraLabels[extraLabels.Count - 1].MouseOut += TaskCredits_MouseOut;
              extraLabels[extraLabels.Count - 1].Passive = false;
              extraLabels[extraLabels.Count - 1].TextColor = Color.SkyBlue;
              extraLabels[extraLabels.Count - 1].ToolTip.Text = kv.Value;
          }
          else
          {
              extraLabels[extraLabels.Count - 1].TextColor = Color.Gray;
          }
          Add(extraLabels[extraLabels.Count - 1]);
  
          i++;
      }
      ClientHeight = extraLabels[extraLabels.Count - 1].Top + 12 + 16;
      Center();
    }

    void TaskCredits_MouseOut(object sender, MouseEventArgs e)
    {
        foreach (KeyValuePair<string, string> kv in extras)
        {
            if (((Label)sender).Text == kv.Key)
            {
                ((Label)sender).TextColor = Color.SkyBlue;
            }
        }    
    }

    void TaskCredits_MouseOver(object sender, MouseEventArgs e)
    {
        foreach (KeyValuePair<string, string> kv in extras)
        {
            if (((Label)sender).Text == kv.Key)
            {
                ((Label)sender).TextColor = Color.DeepSkyBlue;
            }
        }    
    }

    void l_Click(object sender, TomShane.Neoforce.Controls.EventArgs e)
    {
       foreach (KeyValuePair<string, string> kv in extras)
       {
           if (((Label)sender).Text == kv.Key)
           {
               System.Diagnostics.Process.Start(kv.Value);
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
    ////////////////////////////////////////////////////////////////////////////   

    #endregion
    
  }
}
