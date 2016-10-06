using System;
using System.Windows.Forms;
using Ookii.Dialogs;

namespace Main
{
    public static class DialogHelper
    {
        public static void ShowErrorDialog(Exception ex, string path)
        {
            var message = $"{ex.Message}";
            if (ex.InnerException != null && string.IsNullOrWhiteSpace(ex.InnerException.Message))
            {
                message += $"{Environment.NewLine}{ex.InnerException.Message}";
            }

            if (TaskDialog.OSSupportsTaskDialogs)
            {
                var taskDialog = new ErrorDialog()
                {
                    EnableHyperlinks = true,
                    Content = message,
                    WindowTitle = @"Error",
                    Footer = @"Open <a href=""http://www.ookii.org"">" +
                             @"log details</a>.",
                    Buttons =
                    {
                        new TaskDialogButton()
                        {
                            Default = true,
                            Text = "OK",
                            ButtonType = ButtonType.Ok
                        }
                    },
                    ExpandedControlText = "Stack Track",
                    CollapsedControlText = "Stack Track",
                    ExpandedInformation = ex.StackTrace,
                    LogFile = path,
                    CenterParent = true,
                    MainInstruction = @"There has been an error processing your request",
                    MainIcon = TaskDialogIcon.Error,
                };

                taskDialog.ShowDialog();
            }
            else
            {
                message += $"{Environment.NewLine}{ex.StackTrace}";
                MessageBox.Show(message, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
