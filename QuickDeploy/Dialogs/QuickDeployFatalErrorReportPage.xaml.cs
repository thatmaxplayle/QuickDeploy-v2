using System;
using System.Diagnostics;
using System.Windows;

using QuickDeploy.Backend.MVVM.ViewModels;

namespace QuickDeploy.Dialogs
{
    /// <summary>
    /// Interaction logic for QuickDeployFatalError.xaml
    /// </summary>
    public partial class QuickDeployFatalErrorReportPage : Window
    {
        public Exception Exception { get; private set; }

        public QuickDeployFatalErrorReportPage(Exception e)
        {
            // Create a data context for the current exception.
            this.DataContext = new CrashVM(e);
            
            // Set the exception so we can get data from it for reporting purposes, for example.
            this.Exception = e;

            // Do WPF setup stuff. 
            InitializeComponent();
        }

        private void btnOpenGithubIssue_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName    = "https://github.com/thatmaxplayle/QuickDeploy-v2/issues/new",
                UseShellExecute = true
            });
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
