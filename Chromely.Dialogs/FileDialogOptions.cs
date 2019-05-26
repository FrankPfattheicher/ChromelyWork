using System.Collections.Generic;

namespace Chromely.Dialogs
{
    public class FileDialogOptions : DialogOptions
    {
        public string InitialDirectory { get; set; }
        public List<FileFilter> Filters { get; set; }
    }
}