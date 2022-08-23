using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDeploy.Backend.MVVM.ViewModels
{
    internal class RunningDeploymentVM : VMBase
    {

        public RunningDeploymentVM()
        {
            _closeBtnAvailable = false;
            CurrentStatus = "Starting...";
            _windowTitle = "Running Deployment...";
        }

        private string? _currentStatus;
        public string? CurrentStatus
        {
            get => _currentStatus;
            set
            {
                if (value != _currentStatus)
                {
                    _currentStatus = value;
                    OnPropertyChanged(nameof(CurrentStatus));
                }
            }
        }

        private bool _closeBtnAvailable;
        public bool CloseButtonAvailable
        {
            get => _closeBtnAvailable;
            set
            {
                if (_closeBtnAvailable != value)
                {
                    _closeBtnAvailable = value;
                    OnPropertyChanged(nameof(CloseButtonAvailable));
                }
            }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get => _windowTitle;
            set
            {
                if (value != _windowTitle)
                {
                    _windowTitle = value;
                    OnPropertyChanged(nameof(WindowTitle));
                }
            }
        }

    }
}
