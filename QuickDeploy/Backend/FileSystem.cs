using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Windows.Themes;

using Newtonsoft.Json;

using QuickDeploy.Backend.Enums;
using QuickDeploy.Backend.Util;

namespace QuickDeploy.Backend
{
    internal class FileSystem
    {

        internal static Deployment? DeserailizeDeployment(string deployment_fileName)
        {
            try
            {
                string fName = $"deployments\\{deployment_fileName}";

                if (!File.Exists(fName))
                {
                    Console.WriteLine("[ERROR] Deployment file does not exist or is not accessbile.");
                    return null;
                }

                // Load the file 
                string file = File.ReadAllText(fName);

                // Deserialize the file
                Deployment? depl = JsonConvert.DeserializeObject<Deployment>(file);

                return depl;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to deserialize deployment: {0}", e.ToString());
                return null;
            }
        }

        internal static bool SerializeDeployment(Deployment depl)
        {
            try
            {
                if (!Directory.Exists("deployments"))
                    Directory.CreateDirectory("deployments");

                string fName = $"deployments\\{depl.Name}.qdf".GetSafeFileName();
                string file = JsonConvert.SerializeObject(depl, Formatting.Indented);
                File.WriteAllText(fName, file);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to serialize deployment: {0}", e.ToString());
                return false;
            }
        }

        internal static string SerializeDeploymentRetStr(Deployment depl)
        {
            try
            {
                string fName = $"\\deployments\\{depl.Name}";
                string file = JsonConvert.SerializeObject(depl, Formatting.Indented);
                return file;
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to serialize deployment: {0}", e.ToString());
                return null;
            }
        }

        internal static bool DirectoryCopy(Deployment.DeploymentUpdated? evt, Deployment depl, string sourceDirectory, string destinationDirectory, bool recursive, out string? failReason, out string? failDirectory, out int filesCopied, out int directoriesCopied)
        {
            filesCopied = 0;
            directoriesCopied = 0;

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirectory);

            if (!dir.Exists)
            {
                failReason = "Source Directory does not exist.";
                failDirectory = "root";
                return false;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            Console.WriteLine("Creating the destination directory; as it does not exist.");
            Directory.CreateDirectory(destinationDirectory);

            // Get the files in the directory, and copy them to a new location.
            FileInfo[] files = dir.GetFiles();

            foreach (var file in files)
            {
                string tempPath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(tempPath, true);

                evt?.Invoke(depl, DeploymentStatus.CopyingFiles, file.Name);

                filesCopied++;

#if DEBUG
                // Artificial wait on DEBUG builds to ensure
                // the UI is working correctly.
                Thread.Sleep(millisecondsTimeout: 120);
#endif
            }

            if (recursive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destinationDirectory, subdir.Name);
                    if (!DirectoryCopy(subdir.FullName, tempPath, recursive, out failReason, out failDirectory, out int _filesCopied, out int _dirsCopied))
                    {
                        return false;
                    }

                    evt?.Invoke(depl, DeploymentStatus.CopyingFolders, subdir.Name);

                    directoriesCopied += _dirsCopied;
                    filesCopied += _filesCopied;

#if DEBUG
                    // Artificial wait on DEBUG builds to
                    // ensure UI is working correctly.
                    Thread.Sleep(120);
#endif
                }
            }

            Directory.SetLastWriteTime(destinationDirectory, DateTime.Now);

            // Success!
            failReason = null;
            failDirectory = null;
            return true;
        }

        internal static bool DirectoryCopy(string sourceDirectory, string destinationDirectory, bool recursive, out string? failReason, out string? failDirectory, out int filesCopied, out int directoriesCopied)
        {
            filesCopied = 0;
            directoriesCopied = 0;

            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirectory);
        
            if (!dir.Exists)
            {
                failReason = "Source Directory does not exist.";
                failDirectory = "root";
                return false;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            Console.WriteLine("Creating the destination directory; as it does not exist.");
            Directory.CreateDirectory(destinationDirectory);

            // Get the files in the directory, and copy them to a new location.
            FileInfo[] files = dir.GetFiles();
            
            foreach (var file in files)
            {
                string tempPath = Path.Combine(destinationDirectory, file.Name);
                file.CopyTo(tempPath, true);
                filesCopied++;
            }

            if (recursive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destinationDirectory, subdir.Name);
                    if (!DirectoryCopy(subdir.FullName, tempPath, recursive, out failReason, out failDirectory, out int _filesCopied, out int _dirsCopied)) 
                    {
                        return false;
                    }

                    directoriesCopied += _dirsCopied;
                    filesCopied += _filesCopied;
                }
            }

            Directory.SetLastWriteTime(destinationDirectory, DateTime.Now);

            // Success!
            failReason = null;
            failDirectory = null;
            return true;
        }

    }
}
