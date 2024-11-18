using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using FileService.Properties;

namespace WindowsService1
{
    public partial class Main : ServiceBase
    {
        private System.Timers.Timer timer;
        private string sourceFolder = Resources.SourceFolder;
        private string destinationFolder = Resources.DestinationFolder;


        public Main()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 10000; // 1 minute
            timer.Elapsed += TimerElapsed;
            timer.Start();


            // Log the service start to Event Viewer
            EventLog.WriteEntry("File Transfer Service Started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            timer.Stop();

            // Log the service stop to Event Viewer
            EventLog.WriteEntry("File Transfer Service Stopped", EventLogEntryType.Information);
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            EventLog.WriteEntry("TimerElapsed function is started.", EventLogEntryType.Information);
            try
            {
                // Get all files from the source folder Folder 1
                string[] files = Directory.GetFiles(sourceFolder);

                foreach (string file in files)
                {
                    try
                    {
                        string destFile = Path.Combine(destinationFolder, Path.GetFileName(file));

                        // Check if file already exists in destination Folder 2, if not, move it
                        if (!File.Exists(destFile))
                        {
                            File.Move(file, destFile);
                            EventLog.WriteEntry($"File transferred: {file}", EventLogEntryType.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry($"Error transferring file {file}: {ex.Message}", EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry($"Error during file transfer: {ex.Message}", EventLogEntryType.Error);
            }
        }
    }
}
