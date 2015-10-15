using IGTLocalizer.Model;
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
using System.Linq;

namespace IGTLocalizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JToken fileContentToken;
        private JObject fileContentObject;
        private TranslationCaller translator;
        public MainWindow()
        {
            InitializeComponent();
            translator = new TranslationCaller();
        }

        private void OpenFile_Button(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json|Text Files (*.txt)|*.txt)";
            if (openfileDialog.ShowDialog() == true)
            {
                String content = File.ReadAllText(openfileDialog.FileName);            
                fileContentToken = JToken.Parse(content);
                fileContentObject = JObject.Parse(content);
                fileViewer.Text = content;
            }
        }

        string startingLangCode = "en";
        string translatedLangCode = "es";
        private void TranslateFile_Button(Object sender, RoutedEventArgs e)
        {
            
            JObject outer = fileContentToken.Value<JObject>();
            JObject inner = fileContentToken["default"].Value<JObject>();

            List<string> clients = outer.Properties().Select(p => p.Name).ToList();
            List<string> properties = inner.Properties().Select(p => p.Name).ToList();


            foreach(string client in clients)
            {
                foreach(string prop in properties)
                {
                    fileContentObject[client][prop] = 
                        translator.TranslateLine(fileContentObject[client][prop].ToString(), startingLangCode, translatedLangCode);
                }
            }

            fileEditor.Text = fileContentObject.ToString();
        }
    }
}
