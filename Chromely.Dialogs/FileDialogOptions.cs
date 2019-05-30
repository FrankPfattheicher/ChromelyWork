using System.Collections.Generic;
using System.Threading;

namespace Chromely.Dialogs
{
    public class FileDialogOptions : DialogOptions
    {
        public bool MustExist { get; set; }
        public string InitialDirectory { get; set; }
        public List<FileFilter> Filters { get; set; }

        public FileDialogOptions()
        {
            // ReSharper disable StringLiteralTypo
            var allFiles = Thread.CurrentThread.CurrentUICulture.Name.StartsWith("de")
                ? "Alle Dateien (*.*)"
                : "All files (*.*)";
            // ReSharper restore StringLiteralTypo
            Filters = new List<FileFilter> {new FileFilter {Name = allFiles, Extension = "*"}};
        }
    }
}