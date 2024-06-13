namespace CadDev.Utils.Messages
{
    public class IO
    {
        /// <summary>
        /// prompt user with information
        /// </summary>
        public static void ShowInfo(string content, string title = "Info")
        {
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// prompt user with warning
        /// </summary>
        public static void ShowWarning(string content, string title = "Warning")
        {
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// prompt a yes/no question to ask for user decision
        /// </summary>
        public static DialogResult ShowQuestion(string content, string title = "Question")
        {
            return MessageBox.Show(content, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        /// <summary>
        /// prompt user with an exception detail
        /// </summary>
        public static void ShowException(Exception ex, string title = "Exception")
        {
            string content = ex.Message + "\n" + ex.StackTrace.ToString();
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
