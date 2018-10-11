using System.Windows.Forms;

namespace ScanAnalyze.Util
{
    class MessageUtil
    {
        public static void ShowError(string msg, string title = "警告")
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowInformation(string msg, string title = "信息")
        {
            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool ShowYesNo(string msg, string title = "警告")
        {
            return MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK;
        }
    }
}
