using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceProcess;
using System.Threading.Tasks;
using BestHTTP.JSON;
using Microsoft.Win32;

namespace launch;

internal class Program
{
    private static void Main(string[] args)
    {
        //IL_00fd: Unknown result type (might be due to invalid IL or missing references)
        //IL_0104: Expected O, but got Unknown
        List<string> argsList = ((args != null) ? new List<string>(args) : new List<string>());
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\EasyAntiCheat_EOS", "ProductsInstalled", "null");
        if (!((Dictionary<string, object>)Json.Decode(File.ReadAllText(baseDirectory + "\\EasyAntiCheat\\Settings.json"))).TryGetValue("productid", out var value) || !(value is string text2))
        {
            throw new Exception("ProductID parse error");
        }
        if (!ServiceController.GetServices().Any((ServiceController x) => x.ServiceName == "EasyAntiCheat_EOS") || text == null || !text.Contains(text2))
        {
            Process process = new Process();
            process.StartInfo.FileName = baseDirectory + "\\EasyAntiCheat\\EasyAntiCheat_EOS_Setup.exe";
            process.StartInfo.WorkingDirectory = baseDirectory;
            process.StartInfo.Arguments = "install " + text2;
            process.Start();
            process.WaitForExit();
        }
        HttpClient val = new HttpClient();
        ((HttpHeaders)val.DefaultRequestHeaders).Add("Authorization", "Api-Key client-bD50OiwphEUxwPTBQbUUIwhSTTDfCGdB");
        IntPtr? intPtr = null;
        bool flag = false;
        bool flag2 = false;
        bool flag3 = false;
        foreach (string item in argsList)
        {
            if (!item.StartsWith("--affinity="))
            {
                continue;
            }
            string[] array = item.Split('=');
            if (array.Length == 2)
            {
                string text3 = array[1].Trim('"').Trim();
                flag3 = true;
                if (text3.StartsWith("0x"))
                {
                    text3 = text3.Substring(2);
                }
                if (ulong.TryParse(text3, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result))
                {
                    intPtr = (IntPtr)(long)result;
                    break;
                }
            }
        }
        string amplitudeUserId = GetAmplitudeUserId();
        if (!flag3)
        {
            object obj = FetchAmplitudeExperimentValue(val, amplitudeUserId, "ct-processor-affinity", 2000);
            if (obj != null)
            {
                try
                {
                    flag = (string)((Dictionary<string, object>)obj)["key"] != "control";
                    flag2 = (string)((Dictionary<string, object>)obj)["key"] == "second-ccx-affinity";
                }
                catch (Exception)
                {
                }
            }
            object obj2 = Registry.LocalMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0\\")?.GetValue("ProcessorNameString");
            if (obj2 != null && flag)
            {
                if (obj2.ToString().Contains("X3D"))
                {
                    flag2 = false;
                }
                if (obj2.ToString().Contains("AMD Ryzen 5 1600 ") || obj2.ToString().Contains("AMD Ryzen 5 1600X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 1920X ") || obj2.ToString().Contains("AMD Ryzen 5 1600 (AF) ") || obj2.ToString().Contains("AMD Ryzen 5 2600E ") || obj2.ToString().Contains("AMD Ryzen 5 2600 ") || obj2.ToString().Contains("AMD Ryzen 5 2600X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 2920X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 2970WX ") || obj2.ToString().Contains("AMD Ryzen 5 3500 ") || obj2.ToString().Contains("AMD Ryzen 5 3500X ") || obj2.ToString().Contains("AMD Ryzen 5 3600 ") || obj2.ToString().Contains("AMD Ryzen 5 3600X ") || obj2.ToString().Contains("AMD Ryzen 5 3600XT ") || obj2.ToString().Contains("AMD Ryzen 9 3900 ") || obj2.ToString().Contains("AMD Ryzen 9 3900X ") || obj2.ToString().Contains("AMD Ryzen 9 3900XT ") || obj2.ToString().Contains("AMD Ryzen Threadripper 3960X ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 3945WX ") || obj2.ToString().Contains("AMD Ryzen 5 4600GE ") || obj2.ToString().Contains("AMD Ryzen 5 4600G ") || obj2.ToString().Contains("AMD Ryzen 5 4500U ") || obj2.ToString().Contains("AMD Ryzen 5 4600U ") || obj2.ToString().Contains("AMD Ryzen 5 4680U ") || obj2.ToString().Contains("AMD Ryzen 5 4600HS ") || obj2.ToString().Contains("AMD Ryzen 5 4600H ") || obj2.ToString().Contains("AMD Ryzen 5 5500U "))
                {
                    intPtr = new IntPtr(63);
                    if (flag2)
                    {
                        intPtr = new IntPtr(intPtr.Value.ToInt64() << 6);
                    }
                }
                else if (obj2.ToString().Contains("AMD Ryzen 7 1700 ") || obj2.ToString().Contains("AMD Ryzen 7 1700X ") || obj2.ToString().Contains("AMD Ryzen 7 1800X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 1900X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 1950X ") || obj2.ToString().Contains("AMD Ryzen 7 2700E ") || obj2.ToString().Contains("AMD Ryzen 7 2700 ") || obj2.ToString().Contains("AMD Ryzen 7 2700X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 2950X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 2990WX ") || obj2.ToString().Contains("AMD Ryzen 7 3700X ") || obj2.ToString().Contains("AMD Ryzen 7 3800X ") || obj2.ToString().Contains("AMD Ryzen 7 3800XT ") || obj2.ToString().Contains("AMD Ryzen 9 3950X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 3970X ") || obj2.ToString().Contains("AMD Ryzen Threadripper 3990X ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 3955WX ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 3975WX ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 3995WX ") || obj2.ToString().Contains("AMD Ryzen 7 4700GE ") || obj2.ToString().Contains("AMD Ryzen 7 4700G ") || obj2.ToString().Contains("AMD Ryzen 7 4700U ") || obj2.ToString().Contains("AMD Ryzen 7 4800U ") || obj2.ToString().Contains("AMD Ryzen 7 4800H ") || obj2.ToString().Contains("AMD Ryzen 7 4800HS ") || obj2.ToString().Contains("AMD Ryzen 7 4980U ") || obj2.ToString().Contains("AMD Ryzen 9 4900HS ") || obj2.ToString().Contains("AMD Ryzen 9 4900H ") || obj2.ToString().Contains("AMD Ryzen 7 5700U ") || obj2.ToString().Contains("AMD Ryzen 3 7330U ") || obj2.ToString().Contains("AMD Ryzen 3 7335U "))
                {
                    intPtr = new IntPtr(255);
                    if (flag2)
                    {
                        intPtr = new IntPtr(intPtr.Value.ToInt64() << 8);
                    }
                }
                else if (obj2.ToString().Contains("AMD Ryzen 9 5900 ") || obj2.ToString().Contains("AMD Ryzen 9 5900X ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 5945WX ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 5965WX ") || obj2.ToString().Contains("AMD Ryzen 9 7900 ") || obj2.ToString().Contains("AMD Ryzen 9 7900X ") || obj2.ToString().Contains("AMD Ryzen 9 7900X3D ") || obj2.ToString().Contains("AMD Ryzen 9 7845HX "))
                {
                    intPtr = new IntPtr(4095);
                    if (flag2)
                    {
                        intPtr = new IntPtr(intPtr.Value.ToInt64() << 12);
                    }
                }
                else if (obj2.ToString().Contains("AMD Ryzen 9 5950X ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 5995WX ") || obj2.ToString().Contains("AMD Ryzen 9 7950X ") || obj2.ToString().Contains("AMD Ryzen 9 7950X3D ") || obj2.ToString().Contains("AMD Ryzen 9 7950HX ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 5955WX ") || obj2.ToString().Contains("AMD Ryzen Threadripper PRO 5975WX "))
                {
                    intPtr = new IntPtr(65535);
                    if (flag2)
                    {
                        intPtr = new IntPtr(intPtr.Value.ToInt64() << 16);
                    }
                }
            }
        }
        if (flag3)
        {
            argsList.Add("--user-affinity");
        }
        else if (intPtr.HasValue)
        {
            argsList.Add("--affinity=" + intPtr.Value.ToString("X"));
        }
        PriorityArg? priorityArg = FindPriorityArgument("--process-priority=");
        PriorityArg? priorityArg2 = FindPriorityArgument("--main-thread-priority=");
        ProcessPriorityClass? processPriorityClass = null;
        if (priorityArg.HasValue)
        {
            argsList.Add("--user-process-priority");
            processPriorityClass = priorityArg.Value switch
            {
                PriorityArg.Idle => ProcessPriorityClass.Idle,
                PriorityArg.BelowNormal => ProcessPriorityClass.BelowNormal,
                PriorityArg.Normal => ProcessPriorityClass.Normal,
                PriorityArg.AboveNormal => ProcessPriorityClass.AboveNormal,
                PriorityArg.High => ProcessPriorityClass.High,
                _ => null,
            };
        }
        if (priorityArg2.HasValue)
        {
            argsList.Add("--user-main-thread-priority");
        }
        if (!priorityArg.HasValue && !priorityArg2.HasValue)
        {
            object obj3 = FetchAmplitudeExperimentValue(val, amplitudeUserId, "ct-process-priority", 2000);
            processPriorityClass = ProcessPriorityClass.Normal;
            if (obj3 != null)
            {
                try
                {
                    string text4 = (string)((Dictionary<string, object>)obj3)["key"];
                    if (!(text4 == "above_normal_process"))
                    {
                        if (text4 == "above_normal_mainthread")
                        {
                            argsList.Add($"--main-thread-priority={1}");
                        }
                    }
                    else
                    {
                        argsList.Add($"--process-priority={1}");
                        processPriorityClass = ProcessPriorityClass.AboveNormal;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        ((HttpMessageInvoker)val).Dispose();
        long timestamp = Stopwatch.GetTimestamp();
        argsList.Add("--startup-begin-ts=" + timestamp);
        string arguments = string.Join(" ", argsList);
        Process process2 = new Process();
        process2.StartInfo.FileName = "start_protected_game.exe";
        process2.StartInfo.WorkingDirectory = baseDirectory;
        process2.StartInfo.Arguments = arguments;
        process2.Start();
        try
        {
            if (intPtr.HasValue)
            {
                process2.ProcessorAffinity = intPtr.Value;
            }
            if (processPriorityClass.HasValue)
            {
                process2.PriorityClass = processPriorityClass.Value;
            }
        }
        catch (Win32Exception)
        {
        }
        PriorityArg? FindPriorityArgument(string argumentPrefix)
        {
            if (!string.IsNullOrEmpty(argumentPrefix))
            {
                for (int num = argsList.Count - 1; num >= 0; num--)
                {
                    string text5 = argsList[num];
                    if (text5.StartsWith(argumentPrefix))
                    {
                        string[] array2 = text5.Split('=');
                        if (array2.Length == 2 && int.TryParse(array2[1].Trim('"').Trim(), out var result2) && Enum.IsDefined(typeof(PriorityArg), result2))
                        {
                            return (PriorityArg)result2;
                        }
                        argsList.RemoveAt(num);
                    }
                }
            }
            return null;
        }
    }

    private static string GetAmplitudeUserId()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "VRChat", "VRChat");
        string result = null;
        try
        {
            if (File.Exists(Path.Combine(path, "settings_com.amplitude")))
            {
                result = (string)((Dictionary<string, object>)Json.Decode(File.ReadAllText(Path.Combine(path, "settings_com.amplitude"))))["com.amplitude_userId"];
            }
        }
        catch (Exception)
        {
        }
        return result;
    }

    private static object FetchAmplitudeExperimentValue(HttpClient httpClient, string amplitudeUserId, string flagKey, int millisecondsTimeout)
    {
        if (amplitudeUserId == null)
        {
            return null;
        }
        HttpResponseMessage val = null;
        try
        {
            Task<HttpResponseMessage> async = httpClient.GetAsync("https://api.lab.amplitude.com/v1/vardata?&user_id=" + amplitudeUserId + "&flag_key=" + flagKey);
            if (!async.Wait(millisecondsTimeout))
            {
                return null;
            }
            val = async.Result;
        }
        catch (Exception)
        {
            return null;
        }
        try
        {
            if (val == null)
            {
                return null;
            }
            if (!val.IsSuccessStatusCode)
            {
                return null;
            }
            HttpContent content = val.Content;
            if (content == null)
            {
                return null;
            }
            object obj = Json.Decode(content.ReadAsStringAsync().Result);
            if (obj == null)
            {
                return null;
            }
            if (!(obj is Dictionary<string, object> dictionary))
            {
                return null;
            }
            if (!dictionary.TryGetValue(flagKey, out var value))
            {
                return null;
            }
            return value;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
