using System;
using System.Windows;

using QuickDeploy.Backend.MVVM;
using QuickDeploy.Dialogs;

namespace QuickDeploy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DeploymentViewManager.Instance.Start();

            AppDomain.CurrentDomain.UnhandledException += this.OnUnhandledException;

            Current.MainWindow = new MainWindow();
            Current.MainWindow.ShowDialog();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var crashDialog = new QuickDeployFatalErrorReportPage((Exception)e.ExceptionObject);
            crashDialog.ShowDialog();
        }
    }
}
