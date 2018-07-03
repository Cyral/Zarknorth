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
    public class DirectConnect : Dialog
    {
        Button Connect;
        //Button Cancel;
        TextBox IPBox;
        Label IP;
        public DirectConnect(Manager manager): base(manager)
    {                                         
      Height = 150;
      Width = 300; 
      MinimumHeight = 100;  
      MinimumWidth = 100;
      Text = "Direct Connect";
      Center();

      TopPanel.Visible = true;
      Caption.Text = "Information";
      Description.Text = "Connect to a server without adding to list";
      Caption.TextColor = Description.TextColor = new Color(96, 96, 96);
            IP = new Label(manager);
            IP.Init();
            IP.Top = 8 + TopPanel.Height;
            IP.Left = 8;
            IP.Text = "IP Address:";
            IP.Width = 100;
            Add(IP);
            IPBox= new TextBox(manager);
            IPBox.Init();
            IPBox.Top = 8 + TopPanel.Height;
            IPBox.Left = 96;
            
            Add(IPBox);
            Connect = new Button(manager);
            Connect.Init();
                Connect.Left = 8;
            Connect.Top = 8;
            Connect.Text = "Join";
            Connect.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                    //Game.config.EnableMessageType(Lidgren.Network.NetIncomingMessageType.DiscoveryResponse);

                    //Game.client = new Lidgren.Network.NetClient(Game.config);
                    //Game.client.Start();
                    //Lidgren.Network.NetOutgoingMessage outmsg = Game.client.CreateMessage();
                    //outmsg.Write((byte)PacketTypes.Login);
                    //outmsg.Write(Game.UserName);
                    //Game.client.Connect(IPBox.Text, 14242,outmsg);
                   

                    //Game.currentGameState = Game.GameState.GameOn;
                
                Close();
            });

            BottomPanel.Add(Connect);
            
    }
    }
}