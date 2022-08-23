using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using QuickDeploy.Backend;
using QuickDeploy.Controls;
using QuickDeploy.Dialogs;

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
            //Test();

            LoadDeployments();
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

        private void LoadDeployments()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (sender, args) =>
            {
                Console.WriteLine("[MainWindow] Loading deployments...");

                // Get all QDF files.
                var files = new DirectoryInfo("deployments").GetFiles("*.qdf");
                Console.WriteLine("Found {0} potential deployments.", files.Length);

                foreach (var file in files)
                {
                    Console.WriteLine("Attempting to load deployment file {0}...", file.Name);

                    var canLoad = Deployment.TryLoad(file.Name, out Deployment? depl);
                    
                    if (canLoad && depl != null)
                    {
                        depl.CanDeployNow = depl.EvaluateEnsureFiles();

                        Console.WriteLine("[INFO] Successfully loaded deployment {0} from file {1}!", depl.Name, file.Name);
                        this.Dispatcher.Invoke(() =>
                        {
                            spDeployments.Children.Add(new VisualDeployment(depl));
                        });
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Failed to load deployment {0}!", file.Name);
                    }

                }

                this.Dispatcher.Invoke(() =>
                {
                    if (spDeployments.Children.Count <= 0)
                    {
                        spDeployments.Children.Add(new TextBlock
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
                            TextWrapping = TextWrapping.WrapWithOverflow,
                            Text = "There are no deployments. Why not create a new one using the New Deployment button above?!",
                        });
                    }
                });
            };

            Task.Run(worker.RunWorkerAsync);
        }

        private void btnNewDeployment_Click(object sender, RoutedEventArgs e)
        {
            NewDeployment newDeployment = new NewDeployment()
            {
                Owner = this
            };
            newDeployment.ShowDialog();
        }
    }
}
