using IGTLocalizer.Model;
using IGTLocalizer.Widgets;
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
        private List<string> clients;
        private List<string> properties;
        LanguageSelector ls;
        public MainWindow()
        {
            InitializeComponent();
            translator = new TranslationCaller();
            ls = new LanguageSelector();
            testGrid.Children.Add(ls);
        }

        private void OpenFile_Button(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openfileDialog.ShowDialog() == true)
            {
                String content = File.ReadAllText(openfileDialog.FileName);            
                fileContentToken = JToken.Parse(content);
                fileContentObject = JObject.Parse(content);

                JObject outer = fileContentToken.Value<JObject>();
                JObject inner = fileContentToken["default"].Value<JObject>();

                clients = outer.Properties().Select(p => p.Name).ToList();
                properties = inner.Properties().Select(p => p.Name).ToList();

                StkJSONProperties.Children.Clear();
                StkOriginalValues.Children.Clear();
                foreach(string p in properties)
                {
                    JSONProperty prop = new JSONProperty();
                    prop.property.Content = p;
                    StkJSONProperties.Children.Add(prop);

                    JSONValue oValue = new JSONValue(true);
                    oValue.myValue.Text = fileContentObject["default"][p].ToString();
                    StkOriginalValues.Children.Add(oValue);
                }

                //fileViewer.Text = content;
            }
        }

        string startingLangCode = "en";
        string translatedLangCode = "es";
        private void TranslateFile_Button(Object sender, RoutedEventArgs e)
        {
            foreach(string p in properties)
            {
                fileContentObject["default"][p] =
                    translator.TranslateLine(fileContentObject["default"][p].ToString(), startingLangCode, ls.selectLang);

                JSONValue eValue = new JSONValue(false);
                eValue.myValue.Text = fileContentObject["default"][p].ToString();
                StkEditableValues.Children.Add(eValue);
            }
            //foreach(string client in clients)
            //{
            //    foreach(string prop in properties)
            //    {
            //        fileContentObject[client][prop] = 
            //            translator.TranslateLine(fileContentObject[client][prop].ToString(), startingLangCode, translatedLangCode);
            //    }
            //}

            //fileEditor.Text = fileContentObject.ToString();
        }
    }
}
