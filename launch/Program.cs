using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using BestHTTP.JSON;
using Microsoft.Win32;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace launch
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // Convert command-line arguments to list for easy manipulation
            List<string> argsList = args != null ? new List<string>(args) : new List<string>();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Check if EasyAntiCheat is installed using the Windows Registry and service check
            string installedProducts = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\EasyAntiCheat_EOS", "ProductsInstalled", null);
            string settingsPath = Path.Combine(baseDirectory, "EasyAntiCheat", "Settings.json");
            Dictionary<string, object> settings = Json.Decode(File.ReadAllText(settingsPath)) as Dictionary<string, object>;

            if (!settings.TryGetValue("productid", out var productIdValue) || productIdValue is not string productId)
            {
                throw new Exception("ProductID parse error");
            }

            // Check if the service exists and the product is installed
            if (!ServiceController.GetServices().Any(s => s.ServiceName == "EasyAntiCheat_EOS") || string.IsNullOrEmpty(installedProducts) || !installedProducts.Contains(productId))
            {
                InstallEasyAntiCheat(baseDirectory, productId);
            }

            // Manage process affinity and priority
            IntPtr? affinityMask = GetProcessorAffinity(argsList);
            ProcessPriorityClass? processPriority = GetProcessPriority(argsList);

            // Launch the main application
            string arguments = string.Join(" ", argsList);
            StartProtectedGame(baseDirectory, arguments, affinityMask, processPriority);
        }

        // Method to install EasyAntiCheat if not found
        private static void InstallEasyAntiCheat(string baseDirectory, string productId)
        {
            using (Process installProcess = new())
            {
                installProcess.StartInfo.FileName = Path.Combine(baseDirectory, "EasyAntiCheat", "EasyAntiCheat_EOS_Setup.exe");
                installProcess.StartInfo.Arguments = $"install {productId}";
                installProcess.StartInfo.WorkingDirectory = baseDirectory;
                installProcess.Start();
                installProcess.WaitForExit();
            }
        }

        // Method to determine CPU affinity, either from arguments or based on the processor
        private static IntPtr? GetProcessorAffinity(List<string> argsList)
        {
            IntPtr? affinityMask = null;

            foreach (string arg in argsList)
            {
                if (arg.StartsWith("--affinity="))
                {
                    string[] parts = arg.Split('=');
                    if (parts.Length == 2)
                    {
                        string affinityHex = parts[1].Trim('"').Trim();
                        if (affinityHex.StartsWith("0x"))
                        {
                            affinityHex = affinityHex.Substring(2);
                        }
                        if (ulong.TryParse(affinityHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                        {
                            affinityMask = (IntPtr)(long)result;
                        }
                    }
                }
            }

            // If no affinity is provided, check processor and set defaults
            if (!affinityMask.HasValue)
            {
                string processorName = (string)Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0\\")?.GetValue("ProcessorNameString");
                if (processorName != null && processorName.Contains("AMD"))
                {
                    // Set affinity mask based on CPU type (for AMD Ryzen processors)
                    if (processorName.Contains("Ryzen 5")) affinityMask = new IntPtr(63);
                    else if (processorName.Contains("Ryzen 7")) affinityMask = new IntPtr(255);
                    else if (processorName.Contains("Ryzen 9")) affinityMask = new IntPtr(4095);
                }
            }

            // Add argument if user-defined affinity
            if (affinityMask.HasValue && !argsList.Contains("--user-affinity"))
            {
                argsList.Add("--affinity=" + affinityMask.Value.ToString("X"));
            }

            return affinityMask;
        }

        // Method to determine process priority from arguments or default to normal
        private static ProcessPriorityClass? GetProcessPriority(List<string> argsList)
        {
            ProcessPriorityClass? priorityClass = null;

            foreach (string arg in argsList)
            {
                if (arg.StartsWith("--process-priority="))
                {
                    string[] parts = arg.Split('=');
                    if (parts.Length == 2 && int.TryParse(parts[1], out var priorityValue) && Enum.IsDefined(typeof(PriorityArg), priorityValue))
                    {
                        priorityClass = (PriorityArg)priorityValue switch
                        {
                            PriorityArg.Idle => ProcessPriorityClass.Idle,
                            PriorityArg.BelowNormal => ProcessPriorityClass.BelowNormal,
                            PriorityArg.Normal => ProcessPriorityClass.Normal,
                            PriorityArg.AboveNormal => ProcessPriorityClass.AboveNormal,
                            PriorityArg.High => ProcessPriorityClass.High,
                            _ => ProcessPriorityClass.Normal,
                        };
                    }
                }
            }

            return priorityClass ?? ProcessPriorityClass.Normal;
        }

        // Method to start the main application with the given arguments
        private static void StartProtectedGame(string baseDirectory, string arguments, IntPtr? affinityMask, ProcessPriorityClass? priorityClass)
        {
            using (Process gameProcess = new())
            {
                gameProcess.StartInfo.FileName = Path.Combine(baseDirectory, "start_protected_game.exe");
                gameProcess.StartInfo.Arguments = arguments;
                gameProcess.StartInfo.WorkingDirectory = baseDirectory;

                gameProcess.Start();

                // Set processor affinity and priority if applicable
                try
                {
                    if (affinityMask.HasValue)
                    {
                        gameProcess.ProcessorAffinity = affinityMask.Value;
                    }
                    if (priorityClass.HasValue)
                    {
                        gameProcess.PriorityClass = priorityClass.Value;
                    }
                }
                catch (Win32Exception)
                {
                    // Handle potential permission issues silently
                }
            }
        }
    }
}
