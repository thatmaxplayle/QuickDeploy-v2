using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

using QuickDeploy.Backend;

using MsgBox = System.Windows.Forms.MessageBox;

namespace QuickDeploy.Dialogs
{
    /// <summary>
    /// Interaction logic for NewDeployment.xaml
    /// </summary>
    public partial class NewDeployment : Window
    {
        private Deployment Constructing;

        public NewDeployment()
        {    
            Constructing = new();
            
            InitializeComponent();

            btnRemoveFile.IsEnabled = false;
            btnRemoveFolder.IsEnabled = false;
            btnRemoveEnsureFile.IsEnabled = false;
        }

        private void btnSelectSourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            // Get selected file name
            string? dirName = GetFolderName("Select Source Directory", this.Constructing.SourceDirectory ?? Environment.CurrentDirectory);

            Console.WriteLine("Selected source directory: {0}", dirName ?? "NULL");

            if (!Directory.Exists(dirName))
                return;

            // Set source directory on constructed object.
            this.Constructing.SourceDirectory = dirName;
            UpdateJsonPreview();
        }

        private void btnSelectDestinationDirectory_Click(object sender, RoutedEventArgs e)
        {
            // Get selected folder name
            string? dirName = GetFolderName("Select Destination Directory", this.Constructing.DestinationDirectory ?? Environment.CurrentDirectory);

            Console.WriteLine("Selecting destination directory: {0}", dirName ?? "NULLNAME");

            if (!Directory.Exists(dirName))
                return;
            
            this.Constructing.DestinationDirectory = dirName;
            UpdateJsonPreview();
        }

        private void UpdateJsonPreview()
        {
            string json = FileSystem.SerializeDeploymentRetStr(this.Constructing);

            txtJsonPreview.Text = json;
        }

        private string? GetFileName(string ttl, string filter, string initial)
        {
            var sfd = new OpenFileDialog();

            sfd.Filter = filter;
            sfd.Title = ttl;
            sfd.InitialDirectory = initial;

            return sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK ? sfd.FileName : null;
        }

        private string? GetFolderName(string ttl, string initial)
        {
            var ofd = new FolderBrowserDialog();

            ofd.InitialDirectory = initial;
            ofd.UseDescriptionForTitle = true;
            ofd.Description = "QuickDeploy - Select Folder";

            return ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK ? ofd.SelectedPath : null;
        }

        private void txtDeploymentName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.Constructing.Name = txtDeploymentName.Text;
            UpdateJsonPreview();
        }

        private void txtDeploymentDescription_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.Constructing.Description = txtDeploymentDescription.Text;
            UpdateJsonPreview();
        }

        private void SaveFileNames()
        {
            var items = lbDeploymentFiles.Items;

            System.Collections.Generic.List<string> fileNames = new();

            foreach (var i in items)
            {
                try
                {
                    var item = i as ListBoxItem;

                    if (item == null)
                        continue;

                    Console.WriteLine("item content: {0}", item.Content.ToString());

                    fileNames.Add(item.Content.ToString());
                }
                catch { }
            }

            this.Constructing.Files = fileNames.ToArray();
            UpdateJsonPreview();
        }

        private void SaveFolderNames()
        {
            var items = lbDeploymentFolders.Items;

            System.Collections.Generic.List<string> folderNames = new();

            foreach (var i in items)
            {
                try
                {
                    var item = i as ListBoxItem;

                    if (item == null)
                        continue;

                    Console.WriteLine("folder item content: {0}", item.Content.ToString());

                    folderNames.Add(item.Content.ToString());
                }
                catch { }
            }

            this.Constructing.Directories = folderNames.ToArray();
            
            UpdateJsonPreview();
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            // If source directory not set, prompt user to set it first.
            if (string.IsNullOrEmpty(this.Constructing.SourceDirectory))
            {
                MsgBox.Show("Please first select a source directory.", "Source Directory not set!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get file
            string? fileName = GetFileName("QuickDeploy - Create new file target", "All Files|*.*", this.Constructing.SourceDirectory);

            // Check file exists
            if (!File.Exists(fileName))
            {
                Console.WriteLine("[ERROR] Selected file target is non-existant.");
                return;
            }

            // Check if selected file is outside of source directory
            if (!new DirectoryInfo(this.Constructing.SourceDirectory).GetFiles().Any(x => x.FullName == fileName))
            {
                MsgBox.Show("You cannot select a file target that is not within the Source Directory.", "Invalid File Target", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lbDeploymentFiles.Items.Add(new ListBoxItem()
            {
                Content = new DirectoryInfo(this.Constructing.SourceDirectory).GetFiles().FirstOrDefault(x => x.FullName == fileName)?.Name ?? throw new InvalidOperationException("Cannot append null file name to folders list.")
            });

            SaveFileNames();
        }

        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            int selectedindex = lbDeploymentFiles.SelectedIndex;

            if (selectedindex < 0)
            {
                return;
            }

            lbDeploymentFiles.Items.RemoveAt(selectedindex);

            SaveFileNames();
        }

        private void lbDeploymentFiles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            btnRemoveFile.IsEnabled = lbDeploymentFiles.SelectedItems.Count > 0;
        }

        private void lbDeploymentFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemoveFolder.IsEnabled = lbDeploymentFolders.SelectedItems.Count > 0;
        }

        private void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            // If source directory not set, prompt user to set it first.
            if (string.IsNullOrEmpty(this.Constructing.SourceDirectory))
            {
                MsgBox.Show("Please first select a source directory.", "Source Directory not set!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get folder name
            string? folderName = GetFolderName("QuickDeploy - Create new folder target", this.Constructing.SourceDirectory);

            if (string.IsNullOrEmpty(folderName))
                return;

            if (!Directory.Exists(folderName))
            {
                MsgBox.Show("The folder you have selected does not exist.", "Invalid Folder Target", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!new DirectoryInfo(this.Constructing.SourceDirectory).GetDirectories().Any(x => x.FullName == folderName))
            {
                MsgBox.Show("You cannot select a folder target that is not within the Source Directory.", "Invalid Folder Target", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lbDeploymentFolders.Items.Add(new ListBoxItem
            {
                Content = new DirectoryInfo(this.Constructing.SourceDirectory).GetDirectories().FirstOrDefault(x => x.FullName == folderName)?.Name ?? throw new InvalidOperationException("Cannot append null folder name to folders list.")
            });

            SaveFolderNames();
        }

        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (lbDeploymentFolders.SelectedIndex < 0)
                return;

            int index = lbDeploymentFolders.SelectedIndex;
            lbDeploymentFolders.Items.RemoveAt(index);

            SaveFolderNames();
        }

        private void SaveEnsureFiles()
        {
            var itms = lbEnsureFiles.Items;

            List<string> fileNames = new();

            foreach (var i in itms)
            {
                if (i is not ListBoxItem item)
                    continue;

                if (string.IsNullOrEmpty(item.Content.ToString()))
                    continue;

                fileNames.Add(item.Content.ToString());
            }

            this.Constructing.EnsureFiles = fileNames.ToArray();

            UpdateJsonPreview();
        }

        private void btnAddEnsureFile_Click(object sender, RoutedEventArgs e)
        {
            var fileName = GetFileName("Add Ensure File", "All Items|*.*", this.Constructing.DestinationDirectory ?? Environment.CurrentDirectory);

            if (string.IsNullOrEmpty(fileName))
                return;

            if (!File.Exists(fileName))
            {
                MsgBox.Show("Ensure File targets must be present when the deployment is created, for validity checking purposes.", "Invalid Ensure File Target", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lbEnsureFiles.Items.Add(new ListBoxItem
            {
                Content = fileName
            });

            SaveEnsureFiles();
        }

        private void btnRemoveEnsureFile_Click(object sender, RoutedEventArgs e)
        {
            int si = lbEnsureFiles.SelectedIndex;

            // si will be -1 if there is no selected item in the listbox. As such, we want to fail.
            if (si < 0)
                return;

            lbEnsureFiles.Items.RemoveAt(si);
            
            SaveEnsureFiles();
        }

        private void lbEnsureFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRemoveEnsureFile.IsEnabled = lbEnsureFiles.SelectedIndex > 0;
        }

        private void chkRecursiveDirectoryCopy_Checked(object sender, RoutedEventArgs e)
        {
            this.Constructing.RecursivelyCopyDirectories = true;
            UpdateJsonPreview();
        }

        private void chkRecursiveDirectoryCopy_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Constructing.RecursivelyCopyDirectories = false;
            UpdateJsonPreview();
        }

        private void chkRecursiveDirectoryCopy_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MsgBox.Show("This option configures whether deployment folders are copied recursively.\n\nWhen this option is enabled, any folders will also have their subdirectories copied when they are copied.\n\nIf this option is disabled, only the top-level folder included in the deployment will be copied.", "Recursive Documentary Copying", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkUseEnsureFiles_Checked(object sender, RoutedEventArgs e)
        {
            this.Constructing.CheckEnsureFiles = true;
            UpdateJsonPreview();
        }

        private void chkUseEnsureFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Constructing.CheckEnsureFiles = false;
            UpdateJsonPreview();
        }

        private void chkUseEnsureFiles_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MsgBox.Show("This option configures whether the Ensure Files are checked for validity before this deployment can run.\n\nIf this option is enabled, the Deployment Runtime will check all of the Ensure Files in this deployment exist before running the deployment. If one or more files do not exist, the deployment will throw a fatal erorr, and will not copy anything. All Ensure File checks are performed before the application copies any data over to the target folder.\n\nIf this option is disabled, no ensure file checks will be made, even if Ensure Files are configured on the deployment.\n\nIf there are no ensure files configured on the deployment, no checks will be made, even if this option is enabled.", "Use Ensure Files", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtBuildCounterId_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.Constructing.BuildCounterId = this.txtBuildCounterId.Text;
            UpdateJsonPreview();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            var validationResult = this.Constructing.Validate(out string? failReason);
        
            if (string.IsNullOrEmpty(failReason))
            {
                // Create deployment
                var dr = MsgBox.Show($"Are you sure you would like to create the deployment {this.Constructing.Name}, copying {this.Constructing.Files?.Length ?? 0} files and {this.Constructing.Directories?.Length ?? 0} folders?\n\nSource:\n{this.Constructing.SourceDirectory}\n\nTarget:\n{this.Constructing.DestinationDirectory}\n\n", "QuickDeploy - Create new deployment?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
                // Non-positive
                if (dr != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }

                FileSystem.SerializeDeployment(this.Constructing);
            }
            else
            {
                MsgBox.Show("Failed to create deployment: " + failReason, "QuickDeploy - Deployment could not be created.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
