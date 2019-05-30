using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Chromely.Dialogs.Windows
{
    public class WindowsDialogs : IChromelyDialogs
    {
        public DialogResponse MessageBox(string message, DialogOptions options)
        {
            var type = WindowsInterop.MB_SYSTEMMODAL | WindowsInterop.MB_SETFOREGROUND;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (options.Icon)
            {
                case DialogIcon.Question:
                    type |= WindowsInterop.MB_ICONQUESTION;
                    break;
                case DialogIcon.Information:
                    type |= WindowsInterop.MB_ICONINFORMATION;
                    break;
                case DialogIcon.Warning:
                    type |= WindowsInterop.MB_ICONWARNING;
                    break;
                case DialogIcon.Error:
                    type |= WindowsInterop.MB_ICONERROR;
                    break;
            }
            var ret = WindowsInterop.MessageBox(IntPtr.Zero, message, options.Title, type);
            return new DialogResponse { IsCanceled = ret == WindowsInterop.IDCANCEL };
        }

        public DialogResponse InputBox(string message, DialogOptions options)
        {
            throw new System.NotImplementedException();
        }


        private static string _initialPath;
        private int OnBrowseEvent(IntPtr hWnd, int msg, IntPtr lp, IntPtr lpData)
        {
            switch(msg)
            {
                case WindowsInterop.BFFM_INITIALIZED: // Required to set initialPath
                {
                    //Win32.SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONA, 1, lpData);
                    // Use BFFM_SETSELECTIONW if passing a Unicode string, i.e. native CLR Strings.
                    if (!string.IsNullOrEmpty(_initialPath))
                    {
                        WindowsInterop.SendMessage(new HandleRef(null, hWnd), WindowsInterop.BFFM_SETSELECTIONW, 1, _initialPath);
                    }
                    break;
                }
                case WindowsInterop.BFFM_SELCHANGED:
                {
                    IntPtr pathPtr = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));
                    if (WindowsInterop.SHGetPathFromIDList(lp, pathPtr))
                        WindowsInterop.SendMessage(new HandleRef(null, hWnd), WindowsInterop.BFFM_SETSTATUSTEXTW, 0, pathPtr);
                    Marshal.FreeHGlobal(pathPtr);
                    break;
                }
            }

            return 0;
        }

        public DialogResponse SelectFolder(string message, FileDialogOptions options)
        {
            var result = new DialogResponse { IsCanceled = true }; 
            var th = new Thread(new ThreadStart(() =>
            {
                WindowsInterop.OleInitialize(IntPtr.Zero);
                result = SelectFolderInternal(message, options);
            }))
            {
#pragma warning disable 618
                ApartmentState = ApartmentState.STA
#pragma warning restore 618
            };
            th.Start();
            th.Join();
            return result;
        }

        private DialogResponse SelectFolderInternal(string message, FileDialogOptions options)
        {
            if (Directory.Exists(options.InitialDirectory))
            {
                _initialPath = options.InitialDirectory;
            }
            var pidl = IntPtr.Zero;
            var sb = new StringBuilder(256);
            var bufferAddress = Marshal.AllocHGlobal(256);
            var bi = new WindowsInterop.BROWSEINFO
            {
                hwndOwner = IntPtr.Zero,
                pidlRoot = IntPtr.Zero,
                pszDisplayName = message,
                lpszTitle = options.Title,
                ulFlags = WindowsInterop.BIF_NEWDIALOGSTYLE | WindowsInterop.BIF_SHAREABLE,
                lpfn = OnBrowseEvent,
                lParam = IntPtr.Zero,
                iImage = 0
            };
            
            try
            {
                
                pidl = WindowsInterop.SHBrowseForFolder(ref bi);
                if (pidl == IntPtr.Zero)
                {
                    return new DialogResponse { IsCanceled = true };
                }
                if (true != WindowsInterop.SHGetPathFromIDList(pidl, bufferAddress))
                {
                    return new DialogResponse { Value = string.Empty };
                }
                sb.Append(Marshal.PtrToStringAuto(bufferAddress));
            }
            finally
            {
                // Caller is responsible for freeing this memory.
                Marshal.FreeCoTaskMem(pidl);
            }

            return new DialogResponse { Value = sb.ToString() };
        }

        public DialogResponse FileOpen(string message, FileDialogOptions options)
        {
            var ofn = new WindowsInterop.OPENFILENAME();
            ofn.lstructSize = Marshal.SizeOf(ofn);
            
            if (Directory.Exists(options.InitialDirectory))
            {
                ofn.lpstrInitialDir = options.InitialDirectory;
            }

            ofn.lpstrFilter = options.Filters
                .Select(f => $"{f.Name}\0*.{f.Extension}\0")
                .Aggregate("", (s1, s2) => s1 + s2)
                + "\0";

            ofn.lpstrFile = new string(' ', 4096);
            ofn.lMaxFile = ofn.lpstrFile.Length;
            
            ofn.lpstrTitle = options.Title;
            ofn.lMaxFileTitle = ofn.lpstrTitle.Length;

            ofn.lFlags = WindowsInterop.OFN_EXPLORER;
            if (options.MustExist)
            {
                ofn.lFlags |= WindowsInterop.OFN_PATHMUSTEXIST | WindowsInterop.OFN_FILEMUSTEXIST;
            }
            
            var ok = WindowsInterop.GetOpenFileName(ofn);
            return new DialogResponse { IsCanceled = !ok, Value = ofn.lpstrFile };
        }

        public DialogResponse FileSave(string message, string fileName, FileDialogOptions options)
        {
            var ofn = new WindowsInterop.OPENFILENAME();
            ofn.lstructSize = Marshal.SizeOf(ofn);
            
            if (Directory.Exists(options.InitialDirectory))
            {
                ofn.lpstrInitialDir = options.InitialDirectory;
            }

            ofn.lpstrFilter = options.Filters
                                  .Select(f => $"{f.Name}\0*.{f.Extension}\0")
                                  .Aggregate("", (s1, s2) => s1 + s2)
                              + "\0";

            ofn.lpstrFile = fileName;
            ofn.lMaxFile = 4096;
            
            ofn.lpstrTitle = options.Title;
            ofn.lMaxFileTitle = ofn.lpstrTitle.Length;

            ofn.lFlags = WindowsInterop.OFN_EXPLORER;
            if (options.MustExist)
            {
                ofn.lFlags |= WindowsInterop.OFN_PATHMUSTEXIST | WindowsInterop.OFN_FILEMUSTEXIST;
            }
            
            var ok = WindowsInterop.GetSaveFileName(ofn);
            return new DialogResponse { IsCanceled = !ok, Value = ofn.lpstrFile };
        }
        
    }
}