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
        private String[] TranslatedValues;
        private ObservableCollection<string> custIDs;
        private AddCustomer addCustID;
        UpdateCustomer updateCust;
        public event EventHandler ChangeCustomerContent;

        public MainWindow()
        {
            InitializeComponent();
            translator = new TranslationCaller();
            custIDs = new ObservableCollection<string>();
        }

        string currDir = "";
        string fileName = "";//without extension
        private void OpenFile_Button(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openfileDialog.ShowDialog() == true)
            {
                custIDs.Clear();
                string fullPath = openfileDialog.FileName;
                currDir = System.IO.Path.GetDirectoryName(fullPath);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                String content = File.ReadAllText(fullPath);            
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
<<<<<<< HEAD
=======
        //string translatedLangCode = "es";
        int radioSelection; //0 = update, 1 = add customer, 2 = add lang


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
        //key and a value ( value can be an array ) 
            //key is surrounded by quotes
        //object is surrounded by {}


        String toSave = "{0,{\"1\":{\"2\":{\"3\":{\"4\":[5,{\"6\":7}]}}}}}";
        private void SaveFile_Button(Object sender, RoutedEventArgs e) {

            string path = currDir + "\\" + fileName + "1.json";
            string quote = "\"";

            string json = "{";
            string newCustName = null;
            foreach (string c in clients)
            {
                json += "\n\t" + quote + c + quote + ":{";
                foreach (string p in properties)
                {
                    json += "\n\t\t" + quote + p + quote + ": " + quote + ((JSONValue)StkEditableValues.Children[properties.IndexOf(p)]).myValue.Text + quote + ",";
                }
                json += "\n\t}\n";

                if (radioSelection == 1 && clients.IndexOf(c) == (clients.Count - 1) ) {
                    newCustName = addCustID.CustomerID.Value.ToString();
                    
                    json += "\n\t" + quote + newCustName + quote + ":{";
                    foreach (string p in properties)
                    {
                        json += "\n\t\t" + quote + p + quote + ": " + quote + ((JSONValue)StkEditableValues.Children[properties.IndexOf(p)]).myValue.Text + quote + ",";
                    }
                    json += "\n\t}\n";
                }
            }
            if (newCustName != null) { 
                clients.Add(newCustName);
            }
            json += "}";
            System.IO.File.WriteAllText(path, json);
        }
<<<<<<< HEAD
>>>>>>> saving files
=======
>>>>>>> ba6bf6bc3063d871396da97caf5db1eb69dfd5f8

        private void CanTranslateFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TranslateFile(object sender, ExecutedRoutedEventArgs e)
        {
<<<<<<< HEAD
<<<<<<< HEAD
            TranslatedValues = new String[properties.Count];
            for(int i = 0; i < TranslatedValues.Length; i++)
            {
                TranslatedValues[i] = fileContentObject["default"][properties[i]].ToString();
            }

            TranslatedValues = translator.TranslateMultiLines(TranslatedValues, startingLangCode, toTranslateLang.ls.selectLang);

            StkEditableValues.Children.Clear();
            foreach (string p in TranslatedValues)
            {
=======

            string[] TranslatedValues = new String[properties.Count];
            for (int i = 0; i < TranslatedValues.Length; i++)
            {
                TranslatedValues[i] = fileContentObject["default"][properties[i]].ToString();
            }
            TranslatedValues = translator.TranslateMultiLines(TranslatedValues, startingLangCode, toTranslateLang.ls.selectLang);
            
            StkEditableValues.Children.Clear();
            foreach (string p in TranslatedValues)
            {
>>>>>>> saving files
=======

            string[] TranslatedValues = new String[properties.Count];
            for (int i = 0; i < TranslatedValues.Length; i++)
            {
                TranslatedValues[i] = fileContentObject["default"][properties[i]].ToString();
            }
            TranslatedValues = translator.TranslateMultiLines(TranslatedValues, startingLangCode, toTranslateLang.ls.selectLang);
            
            StkEditableValues.Children.Clear();
            foreach (string p in TranslatedValues)
            {
>>>>>>> ba6bf6bc3063d871396da97caf5db1eb69dfd5f8
                JSONValue eValue = new JSONValue(false);
                eValue.myValue.Text = p;
                StkEditableValues.Children.Add(eValue);
            }
        }

        private void AddNewUser(Object sender, EventArgs e)
        {
            radioSelection = 1;
            addCustID = new AddCustomer();
            AddUserControlStep3(addCustID);
        }

        private void UpdateUser(Object sender, EventArgs e)
        {
            radioSelection = 0;
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
            radioSelection = 2;
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
