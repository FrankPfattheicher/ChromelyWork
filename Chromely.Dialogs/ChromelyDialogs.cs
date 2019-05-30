using System;
using Chromely.Core;
using Chromely.Dialogs.Windows;
// ReSharper disable MemberCanBePrivate.Global

namespace Chromely.Dialogs
{
    public static class ChromelyDialogs
    {
        private static IChromelyDialogs _dialogs;

        static ChromelyDialogs()
        {
            switch (ChromelyRuntime.Platform)
            {
                case ChromelyPlatform.Windows:
                    _dialogs = new WindowsDialogs();
                    return;
                case ChromelyPlatform.Linux:
                    break;
                case ChromelyPlatform.MacOSX:
                    break;
            }
            throw new System.NotImplementedException();
        }
        
        public static DialogResponse MessageBox(string message)
        {
            return MessageBox(message, new DialogOptions());
        }
        public static DialogResponse MessageBox(string message, DialogOptions options)
        {
            return _dialogs.MessageBox(message, options);
        }

        public static DialogResponse InputBox(DialogIcon icon, string message, DialogOptions options)
        {
            return _dialogs.InputBox(message, options);
        }

        public static DialogResponse SelectFolder(string message, FileDialogOptions options)
        {
            return _dialogs.SelectFolder(message, options);
        }

        public static DialogResponse FileOpen(string message, FileDialogOptions options)
        {
            return _dialogs.FileOpen(message, options);
        }

        public static DialogResponse FileSave(string message, string fileName, FileDialogOptions options)
        {
            return _dialogs.FileSave(message, fileName, options);
        }
        
    }
}