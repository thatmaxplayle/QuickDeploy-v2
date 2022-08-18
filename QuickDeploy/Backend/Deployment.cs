
using System.IO;
using System.Linq;

using QuickDeploy.Backend.MVVM.ViewModels;

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
        internal string? Name { get; set; }

        /// <summary>
        /// A basic description of the deployment; which is shown on the UI underneath the title.
        /// </summary>
        internal string? Description { get; set; }

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
        internal string[]? Files { get; set; }

        /// <summary>
        /// A list of relative directory paths, relative to the <see cref="SourceDirectory"/>, which are to be copied when this deployment is run.
        /// </summary>
        /// <remarks>
        /// Directories are only copied recursively if <see cref="RecursivelyCopyDirectories"/> is <see langword="true"/>
        /// </remarks>
        internal string[]? Directories { get; set; }

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
        
        public bool Run(bool silent = false)
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

            bool CheckEnsureFiles(out string failReason)
            {
                failReason = "";

                // Always successfully return this check if we're not 
                if (!this.CheckEnsureFiles || (!this.EnsureFiles?.Any() ?? true))
                    return true;

                if (this.EnsureFiles == null)
                    return true;

                if (!Directory.Exists(this.DestinationDirectory))
                {
                    failReason = "Destination Directory does not exist; therefore Ensure Files (which should be inside the Destination Directory) cannot possibly exist.";
                    return false;
                }
               
                DirectoryInfo directory = new DirectoryInfo(this.DestinationDirectory);

                foreach (var ensureFile in this.EnsureFiles)
                {
                    if (!directory.GetFiles().Any(x => x.Name == ensureFile))
                    {
                        failReason = $"Ensure file {ensureFile} does not exist in the destination directory: {this.DestinationDirectory}";
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
