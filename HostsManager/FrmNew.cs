using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HostsManager
{
    /// <summary>
    /// A dialog for creating a new hosts file
    /// </summary>
    public partial class FrmNew : Form
    {
        /// <summary>
        /// The file name of the selected hosts file
        /// </summary>
        public string HostsName { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public FrmNew()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
            //max length for full path must be < 260, so reserve space for "/.hosts" + application path
            txtName.MaxLength = 252 - Application.StartupPath.Length;
        }

        /// <summary>
        /// Event handler for OK button click
        /// </summary>
        /// <param name="sender">The button that raised the event</param>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            HostsName = txtName.Text.Trim();
            if (HostsName.Length == 0 || HostsName.ToCharArray().Any(i => Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).Contains(i)))
            {
                MessageBox.Show("Please enter a valid name for this hosts file.", "Invalid name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();
                return;
            }

            //rather not have names with weird spacing
            while (HostsName.Contains("  "))
            {
                HostsName = HostsName.Replace("  ", " ");
            }

            //no .hosts.hosts files please
            if (!HostsName.EndsWith(HostsTrayApp.extension))
            {
                HostsName += HostsTrayApp.extension;
            }

            if (File.Exists(HostsName))
            {
                if (MessageBox.Show(HostsName + " already exists! Would you rather edit the existing file?", "Edit existing hosts file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    txtName.Focus();
                }
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }

        /// <summary>
        /// Event handler for Cancel button click
        /// </summary>
        /// <param name="sender">The button that raised the event</param>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Event handler for when a key is pressed in txtName
        /// </summary>
        /// <param name="sender">The TextBox that raised the event</param>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        private void txtName_KeyPress(object sender, KeyPressEventArgs e)
        {
            //We want Ctrl-A to select all
            if (e.KeyChar == '\x1')
            {
                (sender as TextBox).SelectAll();
                e.Handled = true;
            }
        }
    }
}