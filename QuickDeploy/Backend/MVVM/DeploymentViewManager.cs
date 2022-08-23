using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

using QuickDeploy.Controls;

namespace QuickDeploy.Backend.MVVM
{
    internal class DeploymentViewManager
    {

        static DeploymentViewManager i;
        public static DeploymentViewManager Instance => i ??= new();

        record DeploymentAssociation(Deployment deployment, VisualDeployment visual);

        private Thread thread;
        private List<DeploymentAssociation> deployments;
        
        private DeploymentViewManager()
        {
            deployments = new();
            thread = new Thread(() => this.Tick());
        }

        public void SubscribeToRefreshDataContext(VisualDeployment vDeployment, Deployment deployment)
        {
            Console.WriteLine("New deployment subscribed to {0}: {1}", nameof(DeploymentViewManager), deployment.Name);
            deployments.Add(new(deployment, vDeployment));
        }

        private void Tick()
        {
            while (true)
            {
                // Sleep
                Thread.Sleep(5000);
                Console.WriteLine("Running " + nameof(DeploymentViewManager));

                try
                {

                    Application.Current?.Dispatcher?.Invoke(() =>
                    {
                        try
                        {
                            // If main window is of invalid type, don't bother.
                            if (Application.Current.MainWindow is not MainWindow mw)
                                throw new InvalidOperationException("Cannot update deployment information for a MainWindow which is not of the correct type.");

                            Console.WriteLine("slDeployment Children: {0} | MVVM Registrations: {1}", mw.spDeployments.Children.Count, this.deployments.Count);

                            if (!this.deployments.Any())
                            {
                                Console.WriteLine("Resetting call to Tick() because there are no deployments registered with {0}. Deployments must be registered with {0} to be updated.", nameof(DeploymentViewManager));
                                return;
                            }

                            // Iterate through the children of spDeployments, and update their data context.
                            foreach (var item in deployments.ToArray())
                            {
                                foreach (var v in mw.spDeployments.Children)
                                {
                                    if (v is not VisualDeployment vd)
                                    {
                                        Console.WriteLine("Child is not {0}. Skipping.", nameof(VisualDeployment));
                                        continue;
                                    }

                                    // Check if we've registered this deployment in this manager.
                                    var fod = deployments.FirstOrDefault(x => x.deployment == vd.Deployment);

                                    // Deployment isn't registered, continue to the next one.
                                    if (fod == null)
                                    {
                                        Console.WriteLine("Deployment is not registered within {1}, skipping...", nameof(DeploymentViewManager));
                                        continue;
                                    }

                                    fod.visual.DataContext = fod.deployment.GetAsViewModel();
                                    Console.WriteLine("Updated data context for deployment: {0}", fod.deployment.Name);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("[ERROR] {0} failed to run. Could not dispatch: {1}", nameof(DeploymentViewManager), e.ToString());
                        }
                    });

                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] Failed to run {0}: Dispatcher was unavailable or failed to enquue.", nameof(DeploymentViewManager));
                }
            }
        }

        internal void Start()
        {
            // Start deployment manager thread
            this.thread.Start();
        }
    }
}
