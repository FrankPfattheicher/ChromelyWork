using System;
using System.Diagnostics;
using Chromely.CefGlue;
using Chromely.CefGlue.Loader;
using Chromely.CefGlue.Winapi.BrowserWindow;
using Chromely.Core;
using Chromely.Core.Host;
using Xilium.CefGlue;

namespace BasicCrossPlatform
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var platform = CefRuntime.Platform;
                var version = CefRuntime.ChromeVersion;
                Console.WriteLine($"Running {platform} chromium {version}");

                try
                {
                    CefRuntime.Load();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    CefLoader.Load();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(0);
            }


            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"App directory is {appDirectory}");
            var startUrl = $"file:///{appDirectory}/index.html";
            //var startUrl = $"https://google.de";

            var config = ChromelyConfiguration
                .Create()
                .WithAppArgs(args)
                .WithHostSize(1000, 600)
                .WithStartUrl(startUrl);

            // Set multi-threaded_message_loop false
            // only supported on windows
            //.WithCustomSetting(CefSettingKeys.MultiThreadedMessageLoop, false)
            //.WithCustomSetting(CefSettingKeys.SingleProcess, true)
            //.WithCustomSetting(CefSettingKeys.NoSandbox, true)

            //.WithCommandLineArg("disable-extensions", "1")
            //.WithCommandLineArg("disable-gpu", "1")
            //.WithCommandLineArg("disable-gpu-compositing", "1")
            //.WithCommandLineArg("disable-smooth-scrolling", "1")
            //.WithCommandLineArg("no-sandbox", "1");

            try
            {
                using (var window = BrowserWindow.Create(config))
                    //using (var window = new CefGlueBrowserWindow(config))
                {
                    var result = ((IChromelyWindow) window).Run(args);
                    Console.WriteLine("Run returns " + result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            Console.WriteLine("Done.");
            return 0;
        }
    }
}
