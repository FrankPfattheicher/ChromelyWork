﻿using System;
using System.Collections.Generic;
using System.IO;
using Chromely.CefGlue;
using Chromely.CefGlue.Browser.EventParams;
using Chromely.Core;
using Chromely.Core.Helpers;
using Chromely.Dialogs;

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
                //.WithFramelessHost()
                //.WithGtkHostApi()
                .RegisterEventHandler<ConsoleMessageEventArgs>(CefEventKey.ConsoleMessage, OnWebBrowserConsoleMessage)
                .WithAppArgs(args)
                .WithHostBounds(1000, 600)
                .WithStartUrl(startUrl);

#if true
            var folderResponse =
                ChromelyDialogs.SelectFolder("where to save ?", new FileDialogOptions {Title = "Select Temp"});
            
            DialogResponse response;

            if (folderResponse.IsCanceled)
            {
                response = ChromelyDialogs.MessageBox("<canceled>", new DialogOptions{ Icon = DialogIcon.Warning, Title = "Selected Path"});
            }
            else
            {
                response = ChromelyDialogs.MessageBox(folderResponse.Value.ToString(), new DialogOptions{ Icon = DialogIcon.Information, Title = "Selected Path"});
            }
            
            var filters = new List<FileFilter>
            {
                new FileFilter { Name = "All files (*.*)", Extension = "*" },
                new FileFilter { Name = "Text files (*.txt)", Extension = "txt" }
            };
            var fileResponse = ChromelyDialogs.FileOpen("Select cfg",
                new FileDialogOptions { Title = "Config", MustExist = true, Filters = filters, Directory = folderResponse.Value.ToString() });
            
            response = ChromelyDialogs.MessageBox(fileResponse.Value.ToString(), new DialogOptions { Icon = DialogIcon.Error, Title = "Sample"});

            var fileName = Path.GetFileName(fileResponse.Value.ToString());
            fileName = Path.ChangeExtension(fileName, "bak");
            
            fileResponse = ChromelyDialogs.FileSave("Save cfg", fileName,
                new FileDialogOptions { Title = "Config", Filters = filters, Directory = folderResponse.Value.ToString(), ConfirmOverwrite = true });

            response = ChromelyDialogs.MessageBox(fileResponse.Value.ToString(), new DialogOptions { Icon = DialogIcon.Error, Title = "Sample"});
#endif
            
            try
            {
                using (var window = ChromelyWindow.Create(config))
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
//            Task.Run(async () =>
//            {
//                await Task.Delay(TimeSpan.FromSeconds(1));
//                Environment.Exit(0);
//            });
        }
    }

}

