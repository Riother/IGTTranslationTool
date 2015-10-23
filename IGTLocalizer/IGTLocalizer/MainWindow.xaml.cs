using IGTLocalizer.Model;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private JToken fileContentToken;
        private JObject fileContentObject;
        private TranslationCaller translator;
        private List<string> clients;
        private List<string> properties;
        private AddLanguage toTranslateLang;
        private ObservableCollection<string> custIDs;
        UpdateCustomer updateCust;
        public event EventHandler ChangeCustomerContent;
        public MainWindow()
        {
            InitializeComponent();
            translator = new TranslationCaller();
            custIDs = new ObservableCollection<string>();
        }

        private void OpenFile_Button(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openfileDialog.ShowDialog() == true)
            {
                custIDs.Clear();
                String content = File.ReadAllText(openfileDialog.FileName);            
                fileContentToken = JToken.Parse(content);
                fileContentObject = JObject.Parse(content);

                JObject outer = fileContentToken.Value<JObject>();

                for(int i = 0; i < outer.Count; i++)
                {
                   custIDs.Add(outer.Children().ElementAt(i).Path);
                }
                JObject inner = fileContentToken["default"].Value<JObject>();

                clients = outer.Properties().Select(p => p.Name).ToList();
                properties = inner.Properties().Select(p => p.Name).ToList();

                StkJSONProperties.Children.Clear();
                foreach(string p in properties)
                {
                    JSONProperty prop = new JSONProperty();
                    prop.property.Content = p;
                    StkJSONProperties.Children.Add(prop);
                }
                populateLeftSide("default");

                //fileViewer.Text = content;
            }
        }

        string startingLangCode = "en";
        //string translatedLangCode = "es";


        private void TranslateFile_Button(Object sender, RoutedEventArgs e)
        {
            foreach(string p in properties)
            {
                fileContentObject["default"][p] =
                    translator.TranslateLine(fileContentObject["default"][p].ToString(), startingLangCode, toTranslateLang.ls.selectLang);

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

        private void AddNewUser(Object sender, EventArgs e)
        {
            AddCustomer addCustID = new AddCustomer();
            AddUserControlStep3(addCustID);
        }

        private void UpdateUser(Object sender, EventArgs e)
        {
            updateCust = new UpdateCustomer(this);
            updateCust.UpdateCustBox.ItemsSource = custIDs;
            updateCust.UpdateCustBox.SelectedIndex =  0;
            
            AddUserControlStep3(updateCust);
        }

        private void ComboBox_ChangeCustomerContent(object sender, RoutedEventArgs e)
        {
            var handler = ChangeCustomerContent;
            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        //private void UpdateCust_DropDownClosed(object sender, EventArgs e)
        //{
        //    if(updateCust  != null)
        //    {
        //        //string id = updateCust.ItemSelected;
        //        populateLeftSide(id);
        //    }
        //}

        private void AddNewLanguage(Object sender, EventArgs e)
        {
            toTranslateLang = new AddLanguage();
            AddUserControlStep3(toTranslateLang);
        }

        private void AddUserControlStep3(UserControl uc)
        {
            if (Step3.Children.Count > 0)
            {
                if (Step3.Children[0].GetType() != uc.GetType())
                {
                    Step3.Children.Clear();
                    Step3.Children.Add(uc);
                }
            }
            else
                Step3.Children.Add(uc);
        }

        private void populateLeftSide(string clientId)
        {
            StkOriginalValues.Children.Clear();
            foreach (string p in properties)
            {
                JSONValue oValue = new JSONValue(true);
                oValue.myValue.Text = fileContentObject[clientId][p].ToString();
                StkOriginalValues.Children.Add(oValue);
            }
        }

        private void populateRightSide(string clientId)
        {
            StkEditableValues.Children.Clear();
            foreach (string p in properties)
            {
                JSONValue oValue = new JSONValue(false);
                oValue.myValue.Text = fileContentObject[clientId][p].ToString();
                StkEditableValues.Children.Add(oValue);
            }
        }
        public void clientChanged(string clientId)
        {
            populateLeftSide(clientId);
            populateRightSide(clientId);
        }
    }
}
