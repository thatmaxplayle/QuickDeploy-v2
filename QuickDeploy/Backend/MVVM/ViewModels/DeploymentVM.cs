using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickDeploy.Backend.MVVM.ViewModels
{
    internal class DeploymentVM : INotifyPropertyChanged
    {

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        private bool _canDeployNow;
        public bool CanDeployNow
        {
            get => _canDeployNow;
            set
            {
                if (value != _canDeployNow)
                {
                    _canDeployNow = value;
                    OnPropertyChanged(nameof(CanDeployNow));
                }
            }
        }

        private string _deploymentSummary;
        public string DeploymentSummary
        {
            get => _deploymentSummary;
            set
            {
                if (value != _deploymentSummary)
                {
                    _deploymentSummary = value;
                    OnPropertyChanged(nameof(DeploymentSummary));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
