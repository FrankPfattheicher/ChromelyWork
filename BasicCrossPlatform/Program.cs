using System;
using System.Diagnostics;
using Chromely.CefGlue.Gtk.BrowserWindow;
using Chromely.Core;
using Chromely.Core.Helpers;
using Xilium.CefGlue;

namespace BasicCrossPlatform
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //var startUrl = $"file:///{appDirectory}index.html";
            var startUrl = $"https://google.de";

            try
            {
                var platform = CefRuntime.Platform;
                var version = CefRuntime.ChromeVersion;
                Console.WriteLine($"Running {platform} chromium {version}");
                Console.WriteLine($"App directory is {appDirectory}");

                try
                {
                    CefRuntime.Load();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    BasicCrossPlatform.CefLoader.Load();
                }
                CefRuntime.Load();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(0);
            }


            
            var config = ChromelyConfiguration
                .Create()
                .WithAppArgs(args)
                .WithHostSize(1000, 600)
                .WithStartUrl(startUrl)
                
                // Set multi-threaded_message_loop false
                // only supported on windows
                .WithCustomSetting(CefSettingKeys.MultiThreadedMessageLoop, false)
                .WithCustomSetting(CefSettingKeys.SingleProcess, true)
                .WithCustomSetting(CefSettingKeys.NoSandbox, true)
            
                .WithCommandLineArg("disable-extensions", "1")
                .WithCommandLineArg("disable-gpu", "1")
                .WithCommandLineArg("disable-gpu-compositing", "1")
                .WithCommandLineArg("disable-smooth-scrolling", "1")
                .WithCommandLineArg("no-sandbox", "1");

            using (var window = new CefGlueBrowserWindow(config))
            {
                return window.Run(args);
            }
        }
    }
}
