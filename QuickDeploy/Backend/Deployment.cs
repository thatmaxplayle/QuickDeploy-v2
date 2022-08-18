using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QuickDeploy.Backend.MVVM.ViewModels;

namespace QuickDeploy.Backend
{
    internal class Deployment
    {

        internal string Name { get; set; }
        internal string Description { get; set; }
        internal bool CanDeployNow { get; set; }

        internal string[] Files { get; set; }
        internal string[] Directories { get; set; }

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
