using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace QuickDeploy.Backend.MVVM.ViewModels
{
    internal class CreateDeploymentVM : VMBase
    {

        public CreateDeploymentVM()
        {
            _destinationDirectorySelectForeground = new SolidColorBrush(Color.FromArgb(255,255,0,0));
            _sourceDriectorySelectForeground = new SolidColorBrush(Color.FromArgb(255,255,0,0));
        }

        private SolidColorBrush _destinationDirectorySelectForeground;
        public SolidColorBrush DestinationDirectorySelectForeground
        {
            get => _destinationDirectorySelectForeground;
            set
            {
                if (value != _destinationDirectorySelectForeground)
                {
                    _destinationDirectorySelectForeground = value;
                    OnPropertyChanged(nameof(DestinationDirectorySelectForeground));
                }
            }
        }

        private SolidColorBrush _sourceDriectorySelectForeground;
        public SolidColorBrush SourceDirectorySelectForeground
        {
            get => _sourceDriectorySelectForeground;
            set
            {
                if (value != _sourceDriectorySelectForeground)
                {
                    _sourceDriectorySelectForeground = value;
                    OnPropertyChanged(nameof(SourceDirectorySelectForeground));
                }
            }
        }

        

    }
}
