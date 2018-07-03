using System;

namespace ZarknorthClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Setup forms stuff, to possibly be used if an error occurs and a dialog needs to be opened
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            #if DEBUG
            using (Game game = new Game())
            {
                game.Run(); //In a land... far... far away... There was a game...
            }
            #else
            try
            {
                using (Game game = new Game())
                {
                    game.Run(); //In a land... far... far away... There was a game...
                }
            }
            catch (Exception e)
            {
                //Open all exceptions in an error dialog
                System.Windows.Forms.Application.Run(new ErrorForm(e));
            }
            #endif
        }
    }
}

