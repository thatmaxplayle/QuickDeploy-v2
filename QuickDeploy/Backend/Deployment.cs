
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
    }
}
