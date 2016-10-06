using System.Collections.Generic;
using System.Windows.Forms;

namespace Main.Libs
{
    public class ErrorTracker
    {
        private readonly HashSet<Control> mErrors = new HashSet<Control>();
        private readonly ErrorProvider mProvider;

        public ErrorTracker(ErrorProvider provider)
        {
            mProvider = provider;
        }
        public void SetError(Control ctl, string text)
        {
            if (string.IsNullOrEmpty(text)) mErrors.Remove(ctl);
            else if (!mErrors.Contains(ctl)) mErrors.Add(ctl);
            mProvider.SetError(ctl, text);
        }

        public void Clear()
        {
            mProvider.Clear();
        }
        public int Count { get { return mErrors.Count; } }
    }
}
