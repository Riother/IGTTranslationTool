using IGTLocalizer.Model;
using IGTLocalizer.Widgets;
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
        private AddCustomer addCustID;
        UpdateCustomer updateCust;
        public event EventHandler ChangeCustomerContent;
        string defaultClient = "default";

        public MainWindow()
        {
            InitializeComponent();
            RadioButtons.Visibility = Visibility.Hidden;
            translator = new TranslationCaller();
            custIDs = new ObservableCollection<string>();
        }

        string fullPath = "";
        string currDir = "";
        string fileName = "";//without extension
        private void OpenFile_Button(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json";
            if (openfileDialog.ShowDialog() == true)
            {
                RadioButtons.Visibility = Visibility.Visible;
                custIDs.Clear();
                fullPath = openfileDialog.FileName;
                currDir = System.IO.Path.GetDirectoryName(fullPath);
                fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                String content = File.ReadAllText(fullPath);
                fileContentToken = JToken.Parse(content);
                fileContentObject = JObject.Parse(content);

                JObject outer = fileContentToken.Value<JObject>();

                for (int i = 0; i < outer.Count; i++)
                {
                    custIDs.Add(outer.Children().ElementAt(i).Path);
                }
                JObject inner = fileContentToken[defaultClient].Value<JObject>();

                clients = outer.Properties().Select(p => p.Name).ToList();
                properties = inner.Properties().Select(p => p.Name).ToList();

                StkJSONProperties.Children.Clear();
                foreach (string p in properties)
                {
                    JSONProperty prop = new JSONProperty();
                    prop.property.Content = p;
                    StkJSONProperties.Children.Add(prop);
                }
                populateLeftSide(defaultClient);
            }
        }

        string startingLangCode = "en";
        //string translatedLangCode = "es";
        int radioSelection; //0 = update, 1 = add customer, 2 = add lang


        private void TranslateFile_Button(Object sender, RoutedEventArgs e)
        {
            string perferedLang = toTranslateLang.ls.selectLang;
            if (toTranslateLang.ls.selectLang.Equals("??"))
            {
                SelectLangPopup question = new SelectLangPopup();
                perferedLang = question.selectedLang;
            }
            else
                foreach (string p in properties)
                {
                    fileContentObject["default"][p] =
                        translator.TranslateLine(fileContentObject["default"][p].ToString(), startingLangCode, perferedLang);


                    JSONValue eValue = new JSONValue(false);
                    eValue.myValue.Text = fileContentObject[defaultClient][p].ToString();
                    StkEditableValues.Children.Add(eValue);
                }
        }

        private void SaveFile_Button(Object sender, RoutedEventArgs e)
        {

            //add new
            if (radioSelection == 1 && clients.Contains(addCustID.CustomerID.Text))
            {
                ShowDuplicateClientError();
                return;
            }

            Stream myStream;
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "JSON Files (*.json)|*.json";

            if (saveFile.ShowDialog() == true)
            {
                if ((myStream = saveFile.OpenFile()) != null)
                {

                    string currEditedClientName = (updateCust == null) ? "" : updateCust.UpdateCustBox.SelectedValue.ToString();
                    string json = (radioSelection == 2) ? GetTranslatedFileContent() : GetEditedFileContent(currEditedClientName);

                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    myStream.Write(bytes, 0, bytes.Length);
                    myStream.Flush();
                    myStream.Close();

                    if (currEditedClientName.Equals(""))
                    {
                        currEditedClientName = defaultClient;
                    }
                    ReloadCurrentLottery(currEditedClientName);

                    //if they save it to a new file, open that file
                    if (radioSelection == 2)
                    {
                        fullPath = saveFile.FileName;
                        currDir = System.IO.Path.GetDirectoryName(fullPath);
                        fileName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
                        clients.Clear();
                        clients.Add(defaultClient);
                        custIDs.Clear();
                        String content = File.ReadAllText(fullPath);
                        fileContentToken = JToken.Parse(content);
                        fileContentObject = JObject.Parse(content);

                        JObject outer = fileContentToken.Value<JObject>();

                        for (int i = 0; i < outer.Count; i++)
                        {
                            custIDs.Add(outer.Children().ElementAt(i).Path);
                        }
                        updateCust = new UpdateCustomer(this);
                        updateCust.UpdateCustBox.ItemsSource = custIDs;
                        updateCust.UpdateCustBox.SelectedIndex = 0;
                        AddUserControlStep3(updateCust);

                        addCustID = new AddCustomer();
                        AddUserControlStep3(addCustID);
                    }
                    
                }

                foreach (RadioButton child in RadioButtons.Children)
                {
                    child.IsChecked = false;
                    radioSelection = -1;
                }
                Step3.Children.Clear();
            }
        }

        private void ReloadCurrentLottery(string currLotteryName)
        {
            //reload saved values
            String content = File.ReadAllText(fullPath);
            fileContentToken = JToken.Parse(content);
            fileContentObject = JObject.Parse(content);

            JObject outer = fileContentToken.Value<JObject>();
            JObject inner = fileContentToken[defaultClient].Value<JObject>();

            clients = outer.Properties().Select(p => p.Name).ToList();
            properties = inner.Properties().Select(p => p.Name).ToList();

            StkJSONProperties.Children.Clear();
            foreach (string p in properties)
            {
                JSONProperty prop = new JSONProperty();
                prop.property.Content = p;
                StkJSONProperties.Children.Add(prop);
            }
            populateLeftSide(currLotteryName);
            populateRightSide(currLotteryName);
        }

        private string GetTranslatedFileContent()
        {
            string quote = "\"";
            string json = "{" + "\n\t" + quote + defaultClient + quote + ":{";

            foreach (string propName in properties)
            {
                //if saving current user (need to save the edited values)
                string value = ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text;
                json += "\n\t\t"
                    + quote
                        + propName
                    + quote + ": "
                        + quote + value.Replace("\n", "\\n") + quote + ",";
            }
            json += "\n\t},\n}";
            return json;
        }

        private void ReloadCurrentLottery(string currLotteryName) {
            //reload saved values
            String content = File.ReadAllText(fullPath);
            fileContentToken = JToken.Parse(content);
            fileContentObject = JObject.Parse(content);

            JObject outer = fileContentToken.Value<JObject>();
            JObject inner = fileContentToken[defaultClient].Value<JObject>();

            clients = outer.Properties().Select(p => p.Name).ToList();
            properties = inner.Properties().Select(p => p.Name).ToList();

            StkJSONProperties.Children.Clear();
            foreach (string p in properties)
            {
                JSONProperty prop = new JSONProperty();
                prop.property.Content = p;
                StkJSONProperties.Children.Add(prop);
            }
            populateLeftSide(currLotteryName);
            populateRightSide(currLotteryName);
        }

        private string GetTranslatedFileContent() {
            string quote = "\"";
            string json = "{" + "\n\t" + quote + defaultClient + quote + ":{";
            
            foreach (string propName in properties)
            {
                //if saving current user (need to save the edited values)
                string value = ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text;
                json += "\n\t\t"
                    + quote
                        + propName
                    + quote + ": "
                        + quote + value.Replace("\n", "\\n") + quote + ",";
            }
            json += "\n\t},\n}";
            return json;
        }

        private string GetEditedFileContent(string currClientName) {
            string quote = "\"";

            string json = "{";
            foreach (string clientName in clients)
            {
                json += "\n\t" + quote + clientName + quote + ":{";
                foreach (string propName in properties)
                {
                    //if saving current user (need to save the edited values)
                    string value = (clientName.ToLower().Equals(currClientName.ToLower())) ? ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text
                        : fileContentObject[clientName][propName].ToString();
                    json += "\n\t\t"
                        + quote
                            + propName
                        + quote + ": "
                            + quote + value.Replace("\n", "\\n") + quote + ",";
                }
                json += "\n\t},\n";

            }

            //new customer
            if (radioSelection == 1)
            {
                string newCustName = addCustID.CustomerID.Value.ToString();
                clients.Add(newCustName);
                json += "\n\t"
                    + quote
                        + newCustName
                    + quote + ":{";
                foreach (string propName in properties)
                {
                    json += "\n\t\t"
                        + quote
                            + propName
                        + quote + ": "
                            + quote + ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text.Replace("\n", "\\n") + quote + ",";
                }
                json += "\n\t}\n";

            }

            json += "}";
            return json;
        }

        private void ShowDuplicateClientError()
        {
            MessageBox.Show("Sorry, there is a customer already by that id.  please choose another one.");
        }

        private void CanTranslateFile(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void TranslateFile(object sender, ExecutedRoutedEventArgs e)
        {
            string selectedLanguage = toTranslateLang.ls.selectLang;
            if (selectedLanguage.Equals("??"))
            {

                SelectLangPopup question = new SelectLangPopup();
                question.ShowDialog();
                selectedLanguage = question.selectedLang;

            }
            string[] TranslatedValues = new String[properties.Count];
            for (int i = 0; i < TranslatedValues.Length; i++)
            {
                TranslatedValues[i] = fileContentObject[defaultClient][properties[i]].ToString();
            }
            TranslatedValues = translator.TranslateMultiLines(TranslatedValues, startingLangCode, selectedLanguage);

            StkEditableValues.Children.Clear();
            foreach (string p in TranslatedValues)
            {
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
            populateLeftSide(defaultClient);
            populateRightSide(defaultClient);
        }

        private void UpdateUser(Object sender, EventArgs e)
        {
            radioSelection = 0;
            updateCust = new UpdateCustomer(this);
            updateCust.UpdateCustBox.ItemsSource = custIDs;
            updateCust.UpdateCustBox.SelectedIndex = 0;

            AddUserControlStep3(updateCust);
        }

        private void ComboBox_ChangeCustomerContent(object sender, RoutedEventArgs e)
        {
            var handler = ChangeCustomerContent;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void AddNewLanguage(Object sender, EventArgs e)
        {
            radioSelection = 2;
            toTranslateLang = new AddLanguage();
            AddUserControlStep3(toTranslateLang);
            populateLeftSide(defaultClient);
            populateRightSide(defaultClient);

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
