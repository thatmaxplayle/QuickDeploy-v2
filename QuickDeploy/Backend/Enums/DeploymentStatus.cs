using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDeploy.Backend.Enums
{
    internal enum DeploymentStatus
    {

        ValidatingDeployment,
        ValidatingEnsureFiles,
        CopyingFiles,
        CopyingFolders,

        COMPLETE,
        FAILED,

    }
}
