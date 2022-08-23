
using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;

using QuickDeploy.Backend.Enums;
using QuickDeploy.Backend.MVVM.ViewModels;
using QuickDeploy.Dialogs;

namespace QuickDeploy.Backend
{
    /// <summary>
    /// A model representing a Deployment Object from either a configuration file or Database Entry.
    /// </summary>
    internal class Deployment
    {

        /// <summary>
        /// The name of the deployment; often the name of the project it deploys.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// A basic description of the deployment; which is shown on the UI underneath the title.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// This object controls whether the deployment can be run right now. This should check all ensure files and similar before changing state. <br/>
        /// </summary>
        /// <remarks>
        /// The <see cref="DeploymentVM"/> object associated with this <see cref="Deployment"/> will also take this into account, disabling the <c>RUN</c> button on the <see cref="Controls.VisualDeployment"/> card if it is set to <see langword="false"/>.
        /// </remarks>
        internal bool CanDeployNow { get; set; }

        /// <summary>
        /// The directory from which files are copied when this deployment is run.
        /// </summary>
        public string? SourceDirectory { get; set; }

        /// <summary>
        /// The directory to which files are copied when this deployment is run.
        /// </summary>
        public string? DestinationDirectory { get; set; }

        /// <summary>
        /// A list of relative file paths, relative to the <see cref="SourceDirectory"/>, which are to be copied when this deploymnet is run.
        /// </summary>
        public string[]? Files { get; set; }

        /// <summary>
        /// A list of relative directory paths, relative to the <see cref="SourceDirectory"/>, which are to be copied when this deployment is run.
        /// </summary>
        /// <remarks>
        /// Directories are only copied recursively if <see cref="RecursivelyCopyDirectories"/> is <see langword="true"/>
        /// </remarks>
        public string[]? Directories { get; set; }

        /// <summary>
        /// Whether directories within <see cref="Directories"/> are copied recursively or not, meaning whether subdirectories of those directories are copied too.
        /// </summary>
        public bool RecursivelyCopyDirectories { get; set; }

        /// <summary>
        /// A list of file names, relative to <see cref="DestinationDirectory"/>, all of which are checked for presence on the File System before a deployment is run. 
        /// </summary>
        public string[]? EnsureFiles { get; set; }

        /// <summary>
        /// Whether the <see cref="Deployment"/> should check for <see cref="EnsureFiles"/> before running.
        /// </summary>
        public bool CheckEnsureFiles { get; set; }

        /// <summary>
        /// The ID of the build counter associated with this deployment.
        /// </summary>
        public string? BuildCounterId { get; set; }

        /// <summary>
        /// Return this <see cref="Deployment"/> as a <see cref="DeploymentVM"/>, for use within a <see cref="Controls.VisualDeployment"/>'s <c>DataContext</c>.
        /// </summary>
        /// <returns>The converted <see cref="DeploymentVM"/> object.</returns>
        public DeploymentVM GetAsViewModel()
        {
            return new DeploymentVM()
            {
                Name = this.Name,
                Description = this.Description,
                CanDeployNow = this.CanDeployNow,
                DeploymentSummary = $"{this.Files?.Length ?? 0} files | {this.Directories?.Length ?? 0} folders",
            };
        }

        public delegate void DeploymentUpdated(Deployment sender, DeploymentStatus status, string? info = null);
        public event DeploymentUpdated OnDeploymentUpdated;

        public void Run(bool silent)
        {
            BackgroundWorker worker = new BackgroundWorker();

            bool result = false;

            worker.DoWork += (sender, e) =>
            {
                result = RunInternal(silent, out var failReason);
            };

            DeploymentProgressing progressingDialog = new DeploymentProgressing(this)
            {
                Owner = Application.Current.MainWindow
            };
            progressingDialog.Show();

            worker.RunWorkerAsync();
        }

        private bool RunInternal(bool silent, out string? failReason)
        {
            bool IsValidDeployment(out string? reason)
            {
                if (string.IsNullOrEmpty(this.SourceDirectory))
                {
                    reason = "Source Directory does not exist.";
                    return false;
                }

                if ((!this.Files?.Any() ?? true) && (!this.Directories?.Any() ?? true))
                {
                    reason = "Neither any files or folders to deploy.";
                    return false;
                }

                reason = null;
                return true;
            }

            bool CheckEnsureFiles(out string? failReason)
            {
                failReason = null;

                // Always successfully return this check if we're not 
                if (!this.CheckEnsureFiles || (!this.EnsureFiles?.Any() ?? true))
                    return true;

                if (this.EnsureFiles == null)
                    return true;

                // If the destination directory doesn't exist, ensure files cannot be played.
                if (!Directory.Exists(this.DestinationDirectory))
                {
                    failReason = "Destination Directory does not exist; therefore Ensure Files (which should be inside the Destination Directory) cannot possibly exist.";
                    return false;
                }

                foreach (var ensureFile in this.EnsureFiles)
                {
                    if (!File.Exists(ensureFile))
                    {
                        failReason = $"Ensure file {ensureFile} does not exist!";
                        return false;
                    }
                }

                return true;
            }

            int filesCopied = 0, directoriesCopied = 0;

            bool CopyFiles(out string? failReason, out string? failedFile)
            {
                if (this.Files == null)
                {
                    failReason = "Deployment->Files is null.";
                    failedFile = "none";
                    return false;
                }

                foreach (var file in this.Files)
                {
                    if (!silent)
                        OnDeploymentUpdated?.Invoke(this, DeploymentStatus.CopyingFiles, file);
#if DEBUG
                    // For source-code viewers. This artificial wait
                    // allows me to ensure that the deployment progression UI is working correctly.
                    // ** It will not apply to RELEASE builds **
                    Thread.Sleep(500);
#endif

                    try
                    {
                        string sourceFile = Path.Combine(this.SourceDirectory ?? throw new NullReferenceException("Source Directory is not specified."), file);
                        string destinationFile = Path.Combine(this.DestinationDirectory ?? throw new NullReferenceException("Destination Directory is not specified."), file);
                        File.Copy(sourceFile, destinationFile, true);
                        filesCopied++;
                    }
                    catch (Exception e)
                    {
                        failReason = e.ToString();
                        failedFile = file;
                        return false;
                    }
                }

                failedFile = null;
                failReason = null;
                return true;
            }

            if (!silent)
                OnDeploymentUpdated?.Invoke(this, DeploymentStatus.ValidatingDeployment);

            // Check whether the deployment is valid or not.
            if (!IsValidDeployment(out failReason))
            {
                if (!silent)
                    OnDeploymentUpdated?.Invoke(this, DeploymentStatus.FAILED, failReason);
                Console.WriteLine("Deployment Failed: " + failReason);
                return false;
            }

            if (!silent)
                OnDeploymentUpdated?.Invoke(this, DeploymentStatus.ValidatingEnsureFiles);

            // Check the ensure files for the deployment.
            if (!CheckEnsureFiles(out failReason))
            {
                if (!silent)
                    OnDeploymentUpdated?.Invoke(this, DeploymentStatus.FAILED, failReason);

                Console.WriteLine("Deployment Failed: " + failReason);
                return false;
            }

            if (!silent)
                OnDeploymentUpdated?.Invoke(this, DeploymentStatus.CopyingFiles);

            // Try copy files
            if (!CopyFiles(out failReason, out string? failedFile))
            {
                if (!silent)
                    OnDeploymentUpdated?.Invoke(this, DeploymentStatus.FAILED, failReason);

                Console.WriteLine("Deployment Failed whilst copying file \"{0}\": {1}", failedFile, failReason);
                return false;
            }

            if (this.SourceDirectory == null || this.DestinationDirectory == null)
            {
                if (!silent)
                    OnDeploymentUpdated?.Invoke(this, DeploymentStatus.FAILED, "Non-accessible source/destination directory.");
                throw new InvalidOperationException("Non-accessible source directory or destination directory.");
            }

            if (!silent)
                OnDeploymentUpdated?.Invoke(this, DeploymentStatus.CopyingFolders);

            // Try copy directories
            foreach (var dir in this.Directories)
            {
                if (!FileSystem.DirectoryCopy(this.OnDeploymentUpdated, this, Path.Combine(this.SourceDirectory, dir), Path.Combine(this.DestinationDirectory, dir), this.RecursivelyCopyDirectories, out failReason, out failedFile, out var _filesCopied, out var _dirsCopied))
                {
                    if (!silent)
                        OnDeploymentUpdated?.Invoke(this, DeploymentStatus.FAILED, failReason);

                    Console.WriteLine("FAILED: DirectoryCopy failed for {0}", failedFile);
                    return false;
                }
                else
                {
                    filesCopied += _filesCopied;
                    directoriesCopied += _dirsCopied;
                }
            }

            if (!silent)
                OnDeploymentUpdated?.Invoke(this, DeploymentStatus.COMPLETE, $"{filesCopied}|{directoriesCopied}");

            return true;
        }
        
        internal bool EvaluateEnsureFiles()
        {
            if (!this.CheckEnsureFiles)
                return true;

            if ((!this.EnsureFiles?.Any()) ?? true)
                return true;

            foreach (var ef in this.EnsureFiles)
            {
                if (!File.Exists(ef))
                    return false;
            }

            return true;
        }

        internal ValidationResult Validate(out string? failReason)
        {
            //
            // Source Directory
            //

            if (string.IsNullOrEmpty(this.SourceDirectory))
            {
                failReason = $"{nameof(SourceDirectory)} is not specified.";
                return ValidationResult.Invalid;
            }

            if (!Directory.Exists(this.SourceDirectory))
            {
                failReason = $"{nameof(SourceDirectory)} is not a valid Directory Path.";
                return ValidationResult.Invalid;
            }

            //
            // Destination Directory
            //

            if (string.IsNullOrEmpty(this.DestinationDirectory))
            {
                failReason = $"{nameof(DestinationDirectory)} is not specified.";
                return ValidationResult.Invalid;
            }

            if (!Directory.Exists(this.DestinationDirectory))
            {
                failReason = $"{nameof(DestinationDirectory)} is not a valid Directory Path.";
                return ValidationResult.Invalid;
            }

            //
            // Deployment Name
            //

            if (string.IsNullOrEmpty(this.Name))
            {
                failReason = $"No {nameof(Name)} was specified for the deployment.";
                return ValidationResult.Invalid;
            }

            //
            // Deployment Description
            //

            if (string.IsNullOrEmpty(this.Description))
            {
                failReason = $"No {nameof(Description)} was specified for the deployment. As of QuickDeployEngine-v4, all deployments must have a description.";
                return ValidationResult.Invalid;
            }

            // 
            // File/Folder Targets
            //

            if (this.Files == null && this.Directories == null)
            {
                failReason = $"No file/folder targets have been created. Both {nameof(Files)} and {nameof(Directories)} are empty collections.";
                return ValidationResult.Invalid;
            }

            if ((!this.Files?.Any() ?? true) && (!this.Directories?.Any() ?? true))
            {
                failReason = $"No file/folder targets have been created. Both {nameof(Files)} and {nameof(Directories)} are empty collections.";
                return ValidationResult.Invalid;
            }


            failReason = null;
            return ValidationResult.Valid;
        }

        //
        // STATIC STUFF
        //

        public static Deployment? Load(string deploymentName)
        {
            return FileSystem.DeserailizeDeployment(deploymentName);
        }

        public static bool TryLoad(string deploymentName, out Deployment? deployment) 
        {
            deployment = FileSystem.DeserailizeDeployment(deploymentName);
            return deployment != null;
        }
        
    }
}
