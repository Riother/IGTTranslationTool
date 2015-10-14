using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace IGTLocalizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JObject fileContent;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Button(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt)";
            if (openfileDialog.ShowDialog() == true)
            {
                String content = File.ReadAllText(openfileDialog.FileName);            
                fileContent = JObject.Parse(content);
                fileViewer.Text = content;
            }
        }

        private void TranslateFile_Button(Object sender, RoutedEventArgs e)
        {
            fileEditor.Text = fileContent.ToString();
        }
    }
}
