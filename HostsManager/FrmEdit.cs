using System;
using System.IO;
using System.Windows.Forms;

namespace HostsManager
{
    /// <summary>
    /// A dialog for editing a hosts file
    /// </summary>
    public partial class FrmEdit : Form
    {
        /// <summary>
        /// The action to perform on the file
        /// </summary>
        public FileAction Action { get; set; }

        /// <summary>
        /// The file name of the selected hosts file
        /// </summary>
        public string HostsName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FrmEdit(string hostsFile)
        {
            InitializeComponent();
            this.Icon = Properties.Resources.Icon;
            foreach (var file in new DirectoryInfo(".").GetFiles())
            {
                if (file.Extension == HostsTrayApp.extension)
                {
                    cbxNames.Items.Add(file.Name.Remove(file.Name.LastIndexOf('.')));
                }
            }
            cbxNames.SelectedItem = hostsFile;
        }

        /// <summary>
        /// Event handler for Edit/Delete button click
        /// </summary>
        /// <param name="sender">The button that raised the event</param>
        /// <param name="e">A System.EventArgs that contains the event data</param>
        private void btn_Click(object sender, EventArgs e)
        {
            Action = (FileAction)Enum.Parse(typeof(FileAction), (sender as Button).Text);
            if (Action == FileAction.Edit || MessageBox.Show("Wirklich löschen " + cbxNames.Text + "?", "Bist du sicher??", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                HostsName = cbxNames.Text + HostsTrayApp.extension;
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
    }
}