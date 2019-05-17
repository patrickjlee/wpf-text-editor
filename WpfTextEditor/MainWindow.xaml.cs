using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace WpfTextEditor
{
    public partial class MainWindow : Window
    {
        private List<string> _lineEndings = new List<string> { ".", "!", "?" };
        private string _space = " ";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            string folderToUse = ""; // we don't know which initial folder to use, so just leave blank
            string inputFile;
            if (!GetOpenFileFullPath("Text files (*.txt)|*.txt", "Please select the text file containing the course video transcript",
                folderToUse, out inputFile))
                return;

            var stream = File.OpenText(inputFile);
            var lines = new List<string>();
            var line = stream.ReadLine();
            while (line != null)
            {
                lines.Add(line.TrimEnd());
                line = stream.ReadLine();
            }
            stream.Close();

            var processedlines = new List<string>();
            var currentLine = "";
            var needToRead = true;
            var lineNumber = 0;
            while (lineNumber < lines.Count)
            {
                if (needToRead)
                    currentLine = lines[lineNumber];
                var lastCharacter = currentLine.Substring(currentLine.Length - 1, 1);
                if (_lineEndings.Contains(lastCharacter))
                {
                    processedlines.Add(currentLine);
                    needToRead = true;
                }
                else
                {
                    try
                    {
                        var linenext = lines[lineNumber + 1];
                        currentLine += _space + linenext;
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        // we don't normally need to do anything here
                    }
                    catch
                    {
                        // something unexpected has gone wrong, so we rethrow the exception
                        throw;
                    }
                    needToRead = false;
                }
                lineNumber++;
            }

            var sb = new StringBuilder();
            foreach (var line1 in processedlines)
                sb.AppendLine(line1);
            txtOutput.Text = sb.ToString();

        } // buttonRun_Click

        /// <summary>
        /// Allows user to select a file to open, returns true/false on success/failure and the file full path is in parameter selectedFileFullPath
        /// </summary>
        /// <param name="filter">filter to restrict the type of file shown (e.g. "Access database files (*.mdb)|*.mdb")</param>
        /// <param name="title">The title to display in the dialog (e.g. "Please select the Access database containing the data etc.")</param>
        /// <param name="initialDirectory">Initial directory (set to "" if you wish the dialog to start in the last used directory)(will work either with or without the final /)</param>
        /// <param name="selectedFileFullPath">Output parameter: the full path including filename of the selected file</param>
        /// <returns></returns>
        private bool GetOpenFileFullPath(string filter, string title, string initialDirectory, out string selectedFileFullPath)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = filter;
            dialog.Title = title;
            dialog.InitialDirectory = initialDirectory;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedFileFullPath = dialog.FileName;// contrary to "FileName" this actually gives the full path!
                return true;
            }
            else
            {
                selectedFileFullPath = "";
                return false;
            }
        } 
    }
}
