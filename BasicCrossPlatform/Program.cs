using System;
using System.Threading.Tasks;
using Chromely.CefGlue;
using Chromely.CefGlue.Browser.EventParams;
using Chromely.Core;
using Chromely.Core.Helpers;

namespace BasicCrossPlatform
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"App directory is {appDirectory}");
            var startUrl = $"file:///{appDirectory}/index.html";

            var config = ChromelyConfiguration
                .Create()
                .WithLoadingCefBinariesIfNotFound(true)
                .RegisterEventHandler<ConsoleMessageEventArgs>(CefEventKey.ConsoleMessage, OnWebBrowserConsoleMessage)
                .WithAppArgs(args)
                .WithHostSize(1000, 600)
                .WithStartUrl(startUrl);

            try
            {
                using (var window = ChromelyBrowserWindow.Create(config))
                {
                    var result = window.Run(args);
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

        private static void OnWebBrowserConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            Console.WriteLine("Browser console: " + e.Message);
            Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                Environment.Exit(0);
            });
        }
    }

}

