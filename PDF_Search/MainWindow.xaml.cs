using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Parser.Parts;
using Page = UglyToad.PdfPig.Content.Page;
using System.Diagnostics;
using System.Threading;

namespace PDF_Search
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async Task<List<string>> FindWordInFolder(string directoryPath, string wordToFind)
        {   
            string[] pathArray = [];
            return await Task.Run(() =>
            {
                try { 
                    pathArray = Directory.GetFiles(directoryPath, "*.pdf");
                }catch (Exception e) {
                    MessageBox.Show("Irregular Filepath", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                List<string> filesWithWord = new List<string>();

                foreach (string path in pathArray)
                {
                    if (FindWord(path, wordToFind) > 0)
                    {
                        filesWithWord.Add(path);
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Progressbar.Maximum = pathArray.Length;
                        Progressbar.Value += 1;
                        if (Progressbar.Value == pathArray.Length){
                            Progressbar.Value = 0;
                        }
                    });
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LBPaths.ItemsSource = filesWithWord;
                });

                return filesWithWord;
            });
        }
        public int FindWord(string Filepath, string wordToFind)
        {
            PdfDocument document = PdfDocument.Open(Filepath);
            int foundWordCounter = 0;
            foreach (Page page in document.GetPages())
            {
                string PageText = "";
                foreach (Word word in page.GetWords())
                {
                    PageText = PageText + word.ToString();
                }
                if (PageText.Contains(wordToFind))
                {
                    foundWordCounter++;
                }
            }
            return foundWordCounter;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           FindWordInFolder(TBPath.Text, TBWord.Text);
        }

        private void LBPaths_DoubleClick(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = LBPaths.SelectedValue.ToString(),
                UseShellExecute = true 
            };

            Process.Start(psi);
        }

        private void TBPath_DoubleClick(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFolderDialog dialog = new();

            dialog.Multiselect = false;
            dialog.Title = "Select a folder";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {   string fullPathToFolder = dialog.FolderName;
                string folderNameOnly = dialog.SafeFolderName;

                TBPath.Text = fullPathToFolder;
            }
        }

    }
}
