#region Usings
using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace ZarknorthClient
{
    /// <summary>
    /// Error report dialog which gives the user options on sending in the report.
    /// A crashlog with relevant information is automaticly generated.
    /// </summary>
    /// <author>Cyral</author>
    public partial class ErrorForm : Form
    {
        #region Constants
        /// <summary>
        /// Link to create a new bug report topic via the forums
        /// </summary>
        public const string ReportForumLink = "http://www.zarknorth.com/forum/posting.php?mode=post&f=15";
        /// <summary>
        /// Mailto link to create a new email report, contains autopopulation text
        /// </summary>
        public const string ReportEmailLink = "mailto:contact@zarknorth.com?subject=Error%20Report&body=Before%20submitting%20this%20error%20report%2C%20please%20make%20sure%20you%20read%20and%20follow%20our%20error%20reporting%20guide%2C%20which%20contains%20a%20template%20for%20this%20report.%20You%20MUST%20follow%20this%20template%20in%20order%20for%20your%20issue%20to%20be%20reviewed!%0A%0AYou%20may%20view%20this%20at%20the%20following%20link%3A%20http%3A%2F%2Fzarknorth.com%2Fforum%2Fviewtopic.php%3Ff%3D15%26t%3D184%23p945";
        /// <summary>
        /// Link to full instructions
        /// </summary>
        public const string InstructionsLink = "http://www.zarknorth.com/forum/viewtopic.php?f=15&p=945#p945";
        #endregion

        /// <summary>
        /// Create a new error report dialog from a runtime exception
        /// </summary>
        /// <param name="e">The exception to generate a crashlog from, using the message and stacktrace</param>
        public ErrorForm(Exception exception)
        {
            Random r = new Random();
            InitializeComponent();
            errorBox.Text = "Collecting system information...";
            string[] reasons = new string[] { "Blame Cyral", "Sorry :(", "Darn Bugs...", "Report this!", "Ooops!", "Our bad...", "Please wait!", "We broke it.", "It crashed :|" };
            errorBox.Text += Environment.NewLine + reasons[r.Next(0, reasons.Length)];
            Thread CollectInfo = new Thread(delegate()
            {
                CollectSystemInfo(exception);
            });
            CollectInfo.Start();
        }
        private void CollectSystemInfo(Exception exception)
        {

            //Use a stringbuilder for more efficient string concatenation.
            StringBuilder sb = new StringBuilder();

            //Print basic infomation
            sb.AppendLine("---Automaticly generated crashlog, please attach to post when reporting---");
            sb.AppendLine("Date: " + DateTime.Now.ToString("F"));
            sb.AppendLine("Version: " + Game.GetVersionString());
            sb.AppendLine("");

            //Print system info, catching any random exceptions that might happen
            try
            {
                sb.AppendLine("OS: " + GetOS());
                sb.AppendLine("Graphics: " + GetGraphics());
                sb.AppendLine("Processor: " + GetProcessor());
                sb.AppendLine("Installed RAM: " + Math.Round(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024f / 1024f / 1024f, 2) + "GB");
            }
            catch
            {
                sb.AppendLine("Error while fetching system information");
            }

            //Append the entire exception (Info, Message, Stacktrace)
            sb.AppendLine("");
            sb.AppendLine(exception.ToString());

            errorBox.Invoke(new Action(() => errorBox.Text = sb.ToString()));
            //errorBox.Text = sb.ToString();
        }

        #region Get System Info
        private string GetProcessor()
        {
            string Processor = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (managementObject["Name"] != null)
                    Processor = managementObject["Name"].ToString();

                Processor += " " + System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + " (";
                if (managementObject["NumberOfCores"] != null)
                    Processor += managementObject["NumberOfCores"].ToString() + " Core(s), ";
            }
            foreach (ManagementObject managementObject in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                if (managementObject["NumberOfProcessors"] != null)
                    Processor += managementObject["NumberOfProcessors"].ToString() + " Physical, ";
                if (managementObject["NumberOfLogicalProcessors"] != null)
                    Processor += managementObject["NumberOfLogicalProcessors"].ToString() + " Logical";
            }
            Processor += ")";
            return Processor;
        }
        private string GetGraphics()
        {

            string GraphicsCard = string.Empty;
            string Frequency = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (managementObject["Description"] != null)
                    GraphicsCard = managementObject["Description"].ToString();
                if (managementObject["DisplayFrequency"] != null)
                    Frequency = " (" + managementObject["DisplayFrequency"].ToString() + "Hz)";
            }
            return GraphicsCard + Frequency;
        }
        private string GetOS()
        {
            string OS = string.Empty;
            string Architecture = string.Empty;
            string Version = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (managementObject["Caption"] != null)
                    OS = managementObject["Caption"].ToString();   //Display operating system caption
                if (managementObject["OSArchitecture"] != null)
                    Architecture = managementObject["OSArchitecture"].ToString();   //Display operating system architecture.
                if (managementObject["CSDVersion"] != null)
                    Version =   managementObject["CSDVersion"].ToString(); //Display operating system version.
            }
            return OS.Trim() + " " + Architecture + ", " + Version;
        }
        #endregion

        #region Events
        private void btnForums_Click(object sender, EventArgs e)
        {
            Process.Start(ReportForumLink);
        }
        private void btnInfo_Click(object sender, EventArgs e)
        {
            Process.Start(InstructionsLink);
        }
        private void btnEmail_Click(object sender, EventArgs e)
        {
            Process.Start(ReportEmailLink);
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(errorBox.Text);
            //Select all text just to give feeling and responsiveness of "copying"
            errorBox.SelectAll();
        }
        #endregion
    }
}
