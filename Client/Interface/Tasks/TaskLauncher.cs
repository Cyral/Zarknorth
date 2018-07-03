#region Usings
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Xml;
using Cyral.Extensions;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Controls = TomShane.Neoforce.Controls;
#endregion

namespace ZarknorthClient.Interface
{
    /// <summary>
    /// Launcher dialog for signing in and viewing news feeds
    /// </summary>
    /// <author>Cyral</author>
    public class TaskLauncher : Dialog
    {
        #region Fields
        private TabControl tabFeeds;
        private GroupPanel grpLogin;
        private TextBox txtUser;
        private TextBox txtPass;
        private Label lblUser;
        private Label lblPass;
        private CheckBox chkRememberMe;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnStart; // 2018;
        private Label lblResetPass;
        private Label lblRemindUser;
        public float RealTop;
        #endregion

        #region SubClasses
        private delegate string NewsItemEventHandler(object o, NewsItem n, SyndicationItem s);
        private class NewsItem
        {
            public static List<NewsItem> NewsItems;

            #region Properties
            public string Name { get; set; }
            public string Link { get; set; }
            public string FeedLink { get; set; }
            public string Description { get; set; }
            #endregion
            #region Events
            public event NewsItemEventHandler ToolTip;
            public virtual string GetToolTip(NewsItem n, SyndicationItem s)
            {
                if (ToolTip != null) return ToolTip(this, n, s);
                else
                    return string.Empty;
            }
            #endregion

            public NewsItem(string name, string description, string link, string feedLink)
            {
                Name = name;
                Link = link;
                FeedLink = feedLink;
                Description = description;
            }

            static NewsItem()
            {
                NewsItems = new List<NewsItem>();
            }

            internal void Load(TabPage TabPage)
            {
                Thread LoadItem = new Thread(delegate()
                {
                    Manager manager = TabPage.Manager;

                    //Add description, now saying "prgLoading..."
                    Label DescriptionLabel = new Label(manager) { Left = 8, Top = 8, Width = 400, Height = 12, TextColor = Color.LightGray, Text = "Loading..." };
                    TabPage.Add(DescriptionLabel);
                    DescriptionLabel.BringToFront();

                    //List of labels for the news items
                    List<Label> TextItems = new List<Label>();

                    //Connect to the feed
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(FeedLink);
                    request.Proxy = null;
                    XmlReaderSettings settings = new XmlReaderSettings { CloseInput = true };
                    SyndicationFeed feed = null;

                    // 2018: Since we no longer have these pages, just show a placeholder and don't try to load.
                    DescriptionLabel.Text = "Could not load.";
                    return;

                    try
                    {
                        using (XmlReader reader = XmlReader.Create(request.GetResponse().GetResponseStream(), settings))
                        {
                            feed = SyndicationFeed.Load(reader);
                        }
                    }
                    catch (Exception e)
                    {
                        DescriptionLabel.Text = e.Message;
                    }

                    if (feed != null)
                    {
                        //Setup the descripton link
                        DescriptionLabel.Text = Description;
                        DescriptionLabel.TextColor = Color.LightGray;
                        TomShane.Neoforce.Controls.EventHandler r = new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
                        {
                            System.Diagnostics.Process.Start(Link);
                        });
                        DescriptionLabel.MouseOver += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, Controls.MouseEventArgs e)
                        {
                            ((Label)o).TextColor = Color.SkyBlue;
                        });
                        DescriptionLabel.MouseOut += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, Controls.MouseEventArgs e)
                        {
                            ((Label)o).TextColor = Color.LightGray;
                        });
                        DescriptionLabel.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
                        {
                            System.Diagnostics.Process.Start(Link);
                        });
                        DescriptionLabel.Passive = false;

                        int i = 0;
                        //Foreach item, add a new label_TextChanged and set the text/link
                        foreach (SyndicationItem item in feed.Items)
                        {
                            Label ItemLabel = new Label(manager);
                            ItemLabel.Init();
                            ItemLabel.Top = DescriptionLabel.Top + ((DescriptionLabel.Height + 6) * (i + 1));
                            ItemLabel.Left = DescriptionLabel.Left;
                            ItemLabel.Width = DescriptionLabel.Width;
                            ItemLabel.Height = DescriptionLabel.Height;
                            ItemLabel.Text = item.Title.Text;
                            ItemLabel.Passive = false;
                            ItemLabel.TextColor = Color.White;

                            //Add event handlers
                            ItemLabel.MouseOver += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, Controls.MouseEventArgs e)
                            {
                                ((Label)o).TextColor = Color.DeepSkyBlue;
                            });
                            ItemLabel.MouseOut += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, Controls.MouseEventArgs e)
                            {
                                ((Label)o).TextColor = Color.White;
                            });

                            ItemLabel.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, Controls.EventArgs e)
                            {
                                if (manager.Game.IsActive)
                                    System.Diagnostics.Process.Start(item.Links[0].Uri.ToString());
                            });

                            ItemLabel.ToolTip.Text = GetToolTip(this, item);

                            TextItems.Add((Label)ItemLabel.Clone());
                            i++;
                            //Max amount of items
                            if (i > 5)
                                break;
                        }
                    }
                    //Add each item in the temp list to the tabpage
                    foreach (Label l in TextItems)
                    {
                        TabPage.Add(l);
                        l.BringToFront();
                    }
                });
                LoadItem.Start();
                LoadItem.IsBackground = true;
            }
        }
        #endregion

        #region Constructors
        public TaskLauncher(Manager manager)
            : base(manager)
        {
            Game.CurrentGameState = Game.GameState.HomeLoggedOff;
            //Setup window
            Text = "Zarknorth Launcher";
            CaptionVisible = false;
            TopPanel.Visible = true;
            BottomPanel.Visible = false;
            Caption.Text = "Welcome To Zarknorth!";
            Description.Text = "Sign in, sign up or view the latest news!";
            Description.TextColor = new Color(96, 96, 96);
            Caption.TextColor = Color.LightGray;
            Resizable = false;
            Movable = false;
            DefaultControl = txtUser;
            RealTop = Top;

            //Add feeds tab
            tabFeeds = new TabControl(manager);
            tabFeeds.Init();
            tabFeeds.Left = 8;
            tabFeeds.Top = 8 + TopPanel.Height;
            tabFeeds.Width = 400;
            tabFeeds.Height = 164;
            Add(tabFeeds);

            //Add news item configs
            NewsItem.NewsItems.Clear(); //Just in case we logged in then out again
            NewsItem.NewsItems.Add(new NewsItem("News", "The latest news from the official Zarknorth news forum", "http://www.zarknorth.com/forum/viewforum.php?f=1", "http://www.zarknorth.com/forum/feed.php?mode=news"));
            NewsItem.NewsItems[0].ToolTip += delegate(object o, NewsItem n, SyndicationItem s)
            {
                return "Posted by " + s.Authors[0].Name + " " + s.PublishDate.DateTime.GetPrettyDate() + ".";
            };
            NewsItem.NewsItems.Add(new NewsItem("Reddit", "Hottest links around the /r/Zarknorth Reddit community", "http://www.reddit.com/r/zarknorth", "http://www.reddit.com/r/Zarknorth/.rss"));
            NewsItem.NewsItems[1].ToolTip += delegate(object o, NewsItem n, SyndicationItem s)
            {
                //Get /u/username from html
                int a = s.Summary.Text.IndexOf("<a href=\"http://www.reddit.com/user/");
                int b = s.Summary.Text.IndexOf("\">", a);
                string author = s.Summary.Text.Substring(a + 36, (b) - (a + 36));
                return "Submitted by " + author + " " + s.PublishDate.DateTime.GetPrettyDate() + ".";
            };
            NewsItem.NewsItems.Add(new NewsItem("YouTube", "Newest videos about Zarknorth on the official YouTube", "https://youtube.com/Zarknorth", "http://www.youtube.com/rss/user/Zarknorth/videos.rss"));
            NewsItem.NewsItems[2].ToolTip += delegate(object o, NewsItem n, SyndicationItem s)
            {
                return "Published " + s.PublishDate.DateTime.GetPrettyDate() + ".";
            };
            
            //Load them
            for (int i = 0; i < NewsItem.NewsItems.Count; i++)
            {
                tabFeeds.AddPage(NewsItem.NewsItems[i].Name);
                NewsItem.NewsItems[i].Load(tabFeeds.TabPages[i]);
            }

            #region Init Controls
            //Add user account area
            grpLogin = new GroupPanel(manager);
            grpLogin.Init();
            grpLogin.Text = "Sign In";
            grpLogin.Left = 16 + tabFeeds.Width;
            grpLogin.Top = 8 + TopPanel.Height;
            grpLogin.Width = 200;
            grpLogin.Height = 124;
            grpLogin.Enabled = false; // 2018 - Disable login
            Add(grpLogin);
            //Username label_TextChanged and input
            lblUser = new Label(manager);
            lblUser.Init();
            lblUser.Top = 2;
            lblUser.Left = 8;
            lblUser.Width = grpLogin.Width - 16;
            lblUser.Text = "Username:";
            txtUser = new TextBox(manager);
            txtUser.Init();
            txtUser.Top = lblUser.Top + lblUser.Height + 2;
            txtUser.Left = 8;
            txtUser.Width = grpLogin.Width - 16;
            //Password label_TextChanged and input
            lblPass = new Label(manager);
            lblPass.Init();
            lblPass.Top = txtUser.Top + txtUser.Height + 2;
            lblPass.Left = 8;
            lblPass.Width = 100;
            lblPass.Text = "Password:";
            txtPass = new TextBox(manager);
            txtPass.Init();
            txtPass.Top = lblPass.Top + lblPass.Height + 2;
            txtPass.Left = 8;
            txtPass.Width = grpLogin.Width - 16;
            txtPass.Mode = TextBoxMode.Password;
            txtPass.FocusGained += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                lblResetPass.Visible = true;
            });
            txtPass.FocusLost += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                lblResetPass.Visible = false;
            });

            txtUser.FocusGained += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                lblRemindUser.Visible = true;
            });
            txtUser.FocusLost += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                lblRemindUser.Visible = false;
            });
            //Remember me checkbox
            chkRememberMe = new CheckBox(manager);
            chkRememberMe.Init();
            chkRememberMe.Text = "Remember Me";
            chkRememberMe.Top = txtPass.Top + txtPass.Height + 4;
            chkRememberMe.Left = 8;
            chkRememberMe.Width = 110;
            lblResetPass = new Label(manager);
            lblResetPass.Init();
            lblResetPass.Text = "Forgot Password?";
            lblResetPass.Top = lblPass.Top;
            lblResetPass.Width = 100;
            lblResetPass.Left = grpLogin.Width - lblResetPass.Width - 8;
            lblResetPass.TextColor = Color.SkyBlue;
            lblResetPass.Passive = false;
            lblResetPass.Visible = false;
            lblResetPass.MouseOver += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, TomShane.Neoforce.Controls.MouseEventArgs e)
            {
                lblResetPass.TextColor = Color.DeepSkyBlue;
            });
            lblResetPass.MouseOut += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, TomShane.Neoforce.Controls.MouseEventArgs e)
            {
                lblResetPass.TextColor = Color.SkyBlue;
            });
            lblResetPass.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                System.Diagnostics.Process.Start("http://zarknorth.com/profile?view=reset");
            });

            lblRemindUser = new Label(manager);
            lblRemindUser.Init();
            lblRemindUser.Text = "Forgot Username?";
            lblRemindUser.Top = lblUser.Top;
            lblRemindUser.Width = 104;
            lblRemindUser.Left = grpLogin.Width - lblRemindUser.Width - 8;
            lblRemindUser.TextColor = Color.SkyBlue;
            lblRemindUser.Passive = false;
            lblRemindUser.Visible = false;
            lblRemindUser.MouseOver += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, TomShane.Neoforce.Controls.MouseEventArgs e)
            {
                lblRemindUser.TextColor = Color.DeepSkyBlue;
            });
            lblRemindUser.MouseOut += new TomShane.Neoforce.Controls.MouseEventHandler(delegate(object o, TomShane.Neoforce.Controls.MouseEventArgs e)
            {
                lblRemindUser.TextColor = Color.SkyBlue;
            });
            lblRemindUser.Click += new TomShane.Neoforce.Controls.EventHandler(delegate(object o, TomShane.Neoforce.Controls.EventArgs e)
            {
                System.Diagnostics.Process.Start("http://zarknorth.com/profile?view=remind");
            });
            //Add controls
            grpLogin.Add(txtUser);
            grpLogin.Add(txtPass);
            grpLogin.Add(lblPass);
            grpLogin.Add(lblUser);
            grpLogin.Add(chkRememberMe);
            grpLogin.Add(lblRemindUser);
            grpLogin.Add(lblResetPass);
            //Add Login and Register buttons
            btnLogin = new Button(manager);
            btnLogin.Init();
            btnLogin.Left = 16 + tabFeeds.Width;
            btnLogin.Height = 32;
            btnLogin.Width = 96;
            btnLogin.Top = grpLogin.Height + grpLogin.Top + 6;
            btnLogin.Text = "Sign In";
            btnLogin.Click += new TomShane.Neoforce.Controls.EventHandler(LoginClick);
            Add(btnLogin);
            btnRegister = new Button(manager);
            btnRegister.Init();
            btnRegister.Left = 16 + tabFeeds.Width + 104;
            btnRegister.Height = 32;
            btnRegister.Width = 96;
            btnRegister.Top = grpLogin.Height + grpLogin.Top + 6;
            btnRegister.Text = "Sign Up";
            btnRegister.Click += new TomShane.Neoforce.Controls.EventHandler(RegisterClick);
            Add(btnRegister);

            // 2018
            btnStart = new Button(manager);
            btnStart.Init();
            btnStart.Left = 16;
            btnStart.Height = 64;
            btnStart.Width = 256;
            btnStart.Text = "Start Game";
            btnStart.Color = new Color(30, 255, 0);
            btnStart.Click += new TomShane.Neoforce.Controls.EventHandler(BypassLoginAndStartGame);
            Add(btnStart);

            // 2018
            btnRegister.Enabled = btnLogin.Enabled = false;
            #endregion

            //Change dimensions based on control size
            Width = tabFeeds.Width + grpLogin.Width + 40;
            Height = tabFeeds.Height + tabFeeds.Top + 22 + 64 + 16;

            btnStart.Left = (Width / 2) - (btnStart.Width / 2);
            btnStart.Top = Height - 64 - 24;

            //Center window
            Center();
            //Populate username and password fields if remember me data is present
            GetRememberMeData();
        }
        #endregion

        #region Methods
        protected override void Update(GameTime gameTime)
        {
            if (Game.CurrentGameState == Game.GameState.HomeLoggedOff && MainWindow.InfoBanner.Visible)
            {
                RealTop = MathHelper.Lerp(RealTop, MainWindow.InfoBanner.Top + MainWindow.InfoBanner.Height + 16, .04f * ((float)gameTime.ElapsedGameTime.TotalSeconds * 60f));
                Top = (int)Math.Round(RealTop);
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// Find the remember me info and set the text box's if needed
        /// </summary>
        private void GetRememberMeData()
        {
            if (ConfigurationManager.AppSettings["User"].Length > 0 && ConfigurationManager.AppSettings["Pass"].Length > 0)
            {
                txtUser.Text = Encryption.ToInsecureString(Encryption.DecryptString(ConfigurationManager.AppSettings["User"]));
                txtPass.Text = Encryption.ToInsecureString(Encryption.DecryptString(ConfigurationManager.AppSettings["Pass"]));
                chkRememberMe.Checked = true;
            }
        }
        public void LoginClick(object o, Controls.EventArgs e)
        {
            bool success = false;
            //Set remember me config
            if (chkRememberMe.Checked)
                RememberPass(txtUser.Text, txtPass.Text);
            else
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
                config.AppSettings.Settings["User"].Value = string.Empty;
                config.AppSettings.Settings["Pass"].Value = string.Empty;
                config.Save(ConfigurationSaveMode.Modified);
            }
            //Login
            Manager.Cursor = Manager.Skin.Cursors["Busy"].Resource;
            Login(txtUser.Text, txtPass.Text, grpLogin, out success);
            Manager.Cursor = Manager.Skin.Cursors["Default"].Resource;
            //Close if authenticated
           // if (success)
          //  {
                Close();
                Manager.Remove(MainWindow.InfoBanner);
           // }
        }

        // 2018
        public void BypassLoginAndStartGame(object o, Controls.EventArgs e)
        {
            Game.CurrentGameState = Game.GameState.HomeLoggedOn;
            Game.UserName = "Guest";
            Game.SessionID = "1";
            Close();
            Manager.Remove(MainWindow.InfoBanner);
        }


        /// <summary>
        /// Saves the password and username (encrypted) to the config file
        /// </summary>
        /// <param name="username">The player's username</param>
        /// <param name="password">Password used</param>
        public void RememberPass(string username, string password)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
            config.AppSettings.Settings["Pass"].Value = Encryption.EncryptString(Encryption.ToSecureString(password));
            config.AppSettings.Settings["User"].Value = Encryption.EncryptString(Encryption.ToSecureString(username)); 
            config.Save(ConfigurationSaveMode.Modified);
        }

        /// <summary>
        /// Sends request to zarknorth.com to login
        /// </summary>
        /// <param name="user">Username</param>
        /// <param name="pass">Password</param>
        /// <param name="label_TextChanged">lblName to send details to</param>
        public static void Login(string user, string pass, GroupPanel label, out bool success)
        {
            success = false;
            //If we are logged off
            if (Game.CurrentGameState == Game.GameState.HomeLoggedOff)
            {
                //Get ready...
                label.Text = "Login - Logging in...";
                string error = "";
                string[] args = null;
                bool authenticated = false;
                bool failed = false;
                try
                {
                    if (string.IsNullOrWhiteSpace(user))
                    {
                        error = "Input a username";
                        failed = true;
                    }
                    else if (string.IsNullOrWhiteSpace(pass))
                    {
                        error = "Input a password";
                        failed = true;
                    }
                    else
                    {
                        //Create the POST data
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        string postData = "username=" + user + "&pass=" + pass;
                        byte[] data = encoding.GetBytes(postData);
                        //Make a request
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.zarknorth.com/game/login.php");
                        request.Proxy = null;
                        request.Method = "POST"; //We are posting the info
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = data.Length;
                        //Open a steam write to, then send
                        Stream stream = request.GetRequestStream();
                        stream.Write(data, 0, data.Length);
                        stream.Close();
                        //Get response
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        stream = response.GetResponseStream();
                        //Make a new steam, We will read from this
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            string responseString = sr.ReadToEnd();
                            //Split response. If authenticated, pass, username, and sid
                            args = responseString.Split('-');
                        }
                    }
                }
                catch (Exception e)
                {
                    error = e.Message;
                    failed = true;
                }
                if (!failed)
                {
                    //Did it pass?
                    if (args[0] == "1" && args.Length == 3)
                    {
                        authenticated = true;
                        Game.UserName = args[1];
                        Game.SessionID = args[2];
                    }
                    //Did it fail?
                    else if (args[0] == "0" && args.Length == 2)
                    {
                        failed = true;
                        error = args[1];
                    }
                    else
                    {
                        failed = true;
                        error = "Invalid Response.";
                    }
                    if (authenticated == true)
                    {
                        success = true;
                        Game.CurrentGameState = Game.GameState.HomeLoggedOn;
                    }
                }
                if (failed == true)
                {
                    label.Text = "Login - [color:Gold]" + error + "[/color]";
                }
            }
        }
        public void RegisterClick(object o, Controls.EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.zarknorth.com/component/users/?view=registration");
        }
        #endregion
    }
}
