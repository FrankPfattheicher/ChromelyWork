namespace Chromely.Dialogs
{
    public interface IChromelyDialogs
    {
        DialogResponse MessageBox(DialogIcon icon, string message, DialogOptions options);
        DialogResponse InputBox(DialogIcon icon, string message, DialogOptions options);
        
        DialogResponse FileOpen(string message, FileDialogOptions options);
        DialogResponse FileSaveAs(string message, FileDialogOptions options);
        
    }
}