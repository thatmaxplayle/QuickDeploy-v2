using System.Windows;
using System.Windows.Controls;

using QuickDeploy.Backend;
using QuickDeploy.Backend.MVVM;

namespace QuickDeploy.Controls
{
    /// <summary>
    /// Interaction logic for VisualDeployment.xaml
    /// </summary>
    internal partial class VisualDeployment : UserControl
    {
        /// <summary>
        /// The associated <see cref="Deployment"/> this object represents.
        /// </summary>
        public Deployment Deployment { get; private set; }

        public VisualDeployment(Deployment deployment)
        {
            // Let the DeploymentViewManager know about this VisualDeployment, so any changes to the data within the Deployment are
            // reflected on the UI within 5 seconds.
            DeploymentViewManager.Instance.SubscribeToRefreshDataContext(this, deployment);
            
            // Set the deployment internally
            this.Deployment = deployment;
            // Initially set the view model, so we don't have to wait for DeploymentViewModel to refresh for data to show on the UI.
            this.DataContext = deployment.GetAsViewModel();
            
            // Do all the initial WPF control initialisation stuff. 
            InitializeComponent();
        }

        private void btnRunDeployment_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Running deployment: " + Deployment.Name);
        }
    }
}
