using System;
using Ookii.Dialogs;

namespace Main
{
    class ErrorDialog : TaskDialog
    {
        public string LogFile { get; set; }
        protected override void OnHyperlinkClicked(HyperlinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(LogFile);
            }
            catch (Exception)
            {
                // eat the error
                return;
                // throw;
            }
        }
    }
}