using System;
using System.Diagnostics;
using Chromely.CefGlue;
using Chromely.CefGlue.Loader;
using Chromely.Core;
using Chromely.Core.Helpers;
using Chromely.Core.Host;
using Xilium.CefGlue;

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
                .WithFramelessHost()
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
    }
}
