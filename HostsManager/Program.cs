using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Windows.Forms;
using System.Net;
using Microsoft.Win32;
using System.ComponentModel;

namespace HostsManager
{
    /// <summary>
    /// The main tray app
    /// </summary>
    public class HostsTrayApp : Form
    {

        #region Private members
        private string hostsPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\drivers\etc\hosts";
        private NotifyIcon trayIcon = new NotifyIcon();
        private ContextMenu trayMenu = new ContextMenu();
        private string currentHosts;
        private FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(".", "*" + extension);
        private FileSystemWatcher hostswatcher = new FileSystemWatcher(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\drivers\etc" ,"hosts");
        #endregion

        public static string extension = ".hosts";

        /// <summary>
        /// The application's entry point
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            bool createdNew;
            var mutex = new System.Threading.Mutex(true, "HostsManager", out createdNew);

            if (!createdNew)
            {
                return;
            }
            Application.Run(new HostsTrayApp());
            GC.KeepAlive(mutex);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public HostsTrayApp()
        {

        
            string defaultHosts = "Default" + extension;
            if (!File.Exists(defaultHosts))
            {
                File.WriteAllText(defaultHosts, File.ReadAllText(hostsPath));
                DropPermissions(defaultHosts);
            }

            var lastHosts = Registry.LocalMachine.CreateSubKey("SOFTWARE\\HostsManager");
            currentHosts = (string)lastHosts.GetValue("last", "Default");
            lastHosts.Close();

            BuildMenu();

            trayIcon.Text = "Hosts Manager";
            trayIcon.Icon = Properties.Resources.Icon;
            trayIcon.ContextMenu = trayMenu; //Add menu to tray icon
            trayIcon.Visible = true;

           
            hostswatcher.EnableRaisingEvents = false;
            hostswatcher.SynchronizingObject = this;
            hostswatcher.Changed += (o, e) =>
            {
                
                var chg = new Form1();


                if (chg.ShowDialog() == DialogResult.OK)
                {

                    if (trayMenu.MenuItems[0].Checked == true)
                    {
              
                            hostswatcher.Dispose();

                    }
                    ChangeHost(currentHosts);
                    if (trayMenu.MenuItems[0].Checked == true)
                    {

                        hostswatcher.BeginInit();

                    }

                }
                else
                {
          
                };
            };




            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.SynchronizingObject = this;
            fileSystemWatcher.Changed += (o, e) =>
            {


                string changed = e.Name;
                //If hosts file being used was changed, reflect those changes in the OS's hosts file
                if (changed.Remove(changed.LastIndexOf('.')) == currentHosts && e.ChangeType == WatcherChangeTypes.Changed)
                {

                    if (trayMenu.MenuItems[0].Checked == true)
                    {

                        hostswatcher.Dispose();

                    }
                    
                    ChangeHost(hostsPath);

                    if (trayMenu.MenuItems[0].Checked == true)
                    {

                        hostswatcher.BeginInit();

                    }
                }
            };
            fileSystemWatcher.Created += (o, e) =>
            {
                BuildMenu();
            };
            fileSystemWatcher.Deleted += (o, e) =>
            {
                BuildMenu();
            };
            fileSystemWatcher.Renamed += (o, e) =>
            {
                if (e.OldName.Remove(e.OldName.LastIndexOf('.')) == currentHosts)
                {
                    currentHosts = e.Name.Remove(e.Name.LastIndexOf('.'));
                    BuildMenu();
                }
            };
        }

        #region Helper methods
        /// <summary>
        /// Make file accessible to non-elevated programs and users
        /// </summary>
        /// <param name="fileName">The file to drop administrator-only access to</param>
        private void DropPermissions(string fileName)
        {
            var sec = File.GetAccessControl(fileName);
            sec.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)),
                FileSystemRights.FullControl, AccessControlType.Allow));
            File.SetAccessControl(fileName, sec);
        }

        /// <summary>
        /// Refreshes the list of hosts files in the tray menu
        /// </summary>
        private void BuildMenu()
        {
            trayMenu.MenuItems.Clear();
            ///  trayMenu.MenuItems.Add(hostsPath);
            trayMenu.MenuItems.Add("Guard", (o, e) =>

            {

                if (trayMenu.MenuItems[0].Checked == true)
                {
                    trayMenu.MenuItems[0].Checked = false;
                    hostswatcher.EnableRaisingEvents=false;

                }
else
                {
                    trayMenu.MenuItems[0].Checked = true;
                    hostswatcher.EnableRaisingEvents = true;


                }






            });
            trayMenu.MenuItems.Add("-");
            foreach (var file in new DirectoryInfo(".").GetFiles())
            {
                if (file.Extension == extension)
                {
                    trayMenu.MenuItems.Add(file.Name.Remove(file.Name.LastIndexOf('.')), MenuChanged);
                    trayMenu.MenuItems[trayMenu.MenuItems.Count - 1].Checked = trayMenu.MenuItems[trayMenu.MenuItems.Count - 1].Text == currentHosts;
                }
            }
            trayMenu.MenuItems.Add("-");

            trayMenu.MenuItems.Add("Download Malwaredomain Liste", (o, e) =>
            {
                if (File.Exists("Malware Domains.hosts"))
                { File.Delete("Malware Domains.hosts"); }

                    WebClient Client = new WebClient();
                Client.DownloadFile("http://www.malwaredomainlist.com/hostslist/hosts.txt", @"Malware Domains.hosts");
            });

            trayMenu.MenuItems.Add("Neu", (o, e) =>
            {
                var newFrm = new FrmNew();
                if (newFrm.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(newFrm.HostsName))
                    {
                        File.WriteAllText(newFrm.HostsName, File.Exists("Default" + extension) ? File.ReadAllText("Default" + extension) : String.Empty);
                        DropPermissions(newFrm.HostsName);
                    }
                    EditFile(newFrm.HostsName);
                }
            });

    
            trayMenu.MenuItems.Add("Bearbeiten/Löschen", (o, e) =>
            {
                var editFrm = new FrmEdit(currentHosts);
                if (editFrm.ShowDialog() == DialogResult.OK)
                {
                    if (editFrm.Action == FileAction.Edit)
                    {
                        EditFile(editFrm.HostsName);
                    }
                    else
                    {
                        File.Delete(editFrm.HostsName);
                        if (editFrm.HostsName.Remove(editFrm.HostsName.LastIndexOf('.')) == currentHosts)
                        {
                            currentHosts = "\t"; //It should be impossible for a hosts file to be named this, so this is a dandy null value
                        }
                    }
                }
            });
            trayMenu.MenuItems[2].Enabled = trayMenu.MenuItems.Count != 3; //Disable Edit/Delete if there are no hosts files
            trayMenu.MenuItems.Add("Exit", (o, e) =>
            {
                RegistryKey lastHosts = Registry.LocalMachine.CreateSubKey("SOFTWARE\\HostsManager");
                lastHosts.SetValue("last", currentHosts);
                lastHosts.Close();
                Application.Exit();
            });
        }

        /// <summary>
        /// Opens a file for editing
        /// </summary>
        /// <param name="name">The name of the file to edit</param>
        private void EditFile(string name)
        {
            var process = new Process();
            process.StartInfo.FileName = name;

            try
            {
                process.Start();
            }
            catch (Win32Exception)
            {
                //don't need to do this for Vista and up...
                process.StartInfo.FileName = "rundll32.exe";
                process.StartInfo.Arguments = "shell32.dll,OpenAs_RunDLL " + name;
                process.Start();
            }
        }

        /// <summary>
        /// Changes the host file in use by the OS and flushes DNS
        /// </summary>
        /// <param name="name">The hosts file to switch to</param>
        private void ChangeHost(string name)
        {
            if (trayMenu.MenuItems[0].Checked == true)
            {
                
                hostswatcher.Dispose();

            }
            
            


            File.WriteAllText(hostsPath, File.ReadAllText(name));
            var process = new Process();
            process.StartInfo.FileName = "ipconfig";
            process.StartInfo.Arguments = "/flushdns";
            process.Start();
       

             if (trayMenu.MenuItems[0].Checked == true)
            {

                hostswatcher.BeginInit();

            }

        }


        #endregion

        /// <summary>
        /// Event handler for hosts file change through the ContextMenu
        /// </summary>
        /// <param name="sender">The MenuItem that raised the event</param>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        private void MenuChanged(object sender, EventArgs e)
        {
            string hosts = (sender as MenuItem).Text;
            foreach (MenuItem item in trayMenu.MenuItems)
            {
                item.Checked = false;
            }
            ChangeHost(hosts + extension);
            currentHosts = hosts;
            (sender as MenuItem).Checked = true;
        }

        #region Overridden methods
        /// <summary>
        /// Raises the Load event for the Hosts Manager
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        protected override void OnLoad(EventArgs e)
        {
            Visible = false; //Hide form window.
            ShowInTaskbar = false; //Remove from taskbar.
            base.OnLoad(e);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the Hosts Manager
        /// </summary>
        /// <param name="isDisposing">true to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                trayIcon.Dispose(); //Release the icon resource.
            }
            base.Dispose(isDisposing);
        }
        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // HostsTrayApp
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "HostsTrayApp";
            this.Load += new System.EventHandler(this.HostsTrayApp_Load);
            this.ResumeLayout(false);

        }

        private void HostsTrayApp_Load(object sender, EventArgs e)
        {

        }
    }
}