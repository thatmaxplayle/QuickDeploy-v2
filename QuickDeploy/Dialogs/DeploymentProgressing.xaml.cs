using System;
using System.Windows;
using System.Windows.Markup.Localizer;

using QuickDeploy.Backend;
using QuickDeploy.Backend.Enums;
using QuickDeploy.Backend.MVVM.ViewModels;

namespace QuickDeploy.Dialogs
{
    /// <summary>
    /// Interaction logic for DeploymentProgressing.xaml
    /// </summary>
    internal partial class DeploymentProgressing : Window
    {
        private RunningDeploymentVM ViewModel;

        public DeploymentProgressing(Deployment deployment)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            InitializeComponent();

            this.DataContext = ViewModel = new RunningDeploymentVM();

            this.Title = this.Title.Replace("{DEPL_TITLE}", deployment.Name);

            deployment.OnDeploymentUpdated += this.OnDeploymentUpdated;
        }

        private void OnDeploymentUpdated(Deployment sender, DeploymentStatus status, string? info = null)
        {
            Console.WriteLine($"Status update: {status}, info: {info}");

            switch (status) 
            {
                case DeploymentStatus.COMPLETE:
                    ViewModel.WindowTitle = "Deployment Complete!";
                    ViewModel.CloseButtonAvailable = true;

                    string files = info.Split('|')[0], folders = info.Split('|')[1]; 

                    ViewModel.CurrentStatus = $"This deployment has finished running. {files} files, and {folders} folders were copied.";
                    break;

                case DeploymentStatus.CopyingFiles:
                    ViewModel.WindowTitle = "Copying files...";
                    ViewModel.CurrentStatus = "Copying: " + info;
                    break;

                case DeploymentStatus.CopyingFolders:
                    ViewModel.WindowTitle = "Copying folders...";
                    ViewModel.CurrentStatus = "Copying: " + info;
                    break;

                case DeploymentStatus.FAILED:
                    ViewModel.WindowTitle = "Deployment Failed";
                    ViewModel.CurrentStatus = info;
                    ViewModel.CloseButtonAvailable = true;
                    break;

                default:
                    ViewModel.CloseButtonAvailable = false;
                    ViewModel.CurrentStatus = status + " " + info;
                    break;
            }
        }

        private void btnDismiss_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
