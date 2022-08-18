using System.Windows;

using QuickDeploy.Backend;
using QuickDeploy.Controls;

namespace QuickDeploy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Test();
        }

        void Test()
        {
            var deploymentDisabled = new Deployment
            {
                CanDeployNow = false,
                Name = "Disabled Deployment",
                Description = "This is a disabled deployment."
            };

            var visualDeploymentDisabled = new VisualDeployment(deploymentDisabled);

            spDeployments.Children.Add(visualDeploymentDisabled);

            var enabledDeployment = new Deployment
            {
                CanDeployNow = true,
                Name = "Enabled Deployment",
                Description = "This is a generic deployment, which is enabled."
            };

            var visualDeploymentEnabled = new VisualDeployment(enabledDeployment);

            spDeployments.Children.Add(visualDeploymentEnabled);

        }
    }
}
