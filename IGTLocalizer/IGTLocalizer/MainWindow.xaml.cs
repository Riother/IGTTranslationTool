using IGTLocalizer.Model;
using IGTLocalizer.Widgets;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace IGTLocalizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULT_CLIENT = "default";

        private TranslationCaller translator;

        private JToken fileContentToken;
        private JObject fileContentObject;
        private List<string> clients, properties;

        private AddLanguage toTranslateLang;
        private UpdateCustomer updateCust;
        private AddCustomer addCustID;
        private ObservableCollection<string> custIDs;

        public event EventHandler ChangeCustomerContent;

        private string startingLangCode;
        private string fullPath;
        private string currDir;
        private string fileName;//without extension
        private enum RadioSelection {
            Nothing,
            Update,
            Add,
            Translate
        };
        private RadioSelection radioSelection;

        public MainWindow()
        {
            InitializeComponent();
            RadioButtons.Visibility = Visibility.Hidden; //possible to just put in xml
            translator = new TranslationCaller();
            custIDs = new ObservableCollection<string>();
        }

        private void OpenFile_Button(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfileDialog = new OpenFileDialog();
            openfileDialog.Filter = "JSON Files (*.json)|*.json";

            if (openfileDialog.ShowDialog() == true)
            {
                RadioButtons.Visibility = Visibility.Visible;
                custIDs.Clear();

                fullPath = openfileDialog.FileName;
                currDir = Path.GetDirectoryName(fullPath);
                fileName = Path.GetFileNameWithoutExtension(fullPath);

                ReadJsonFromFile(fullPath);

                JObject outer = PopulateClientIDs();
                //properties
                JObject inner = fileContentToken[DEFAULT_CLIENT].Value<JObject>();

                clients = outer.Properties().Select(p => p.Name).ToList();
                properties = inner.Properties().Select(p => p.Name).ToList();

                StkJSONProperties.Children.Clear();
                foreach (string p in properties)
                {
                    JSONProperty prop = new JSONProperty();
                    prop.property.Content = p;
                    StkJSONProperties.Children.Add(prop);
                }
                PopulateLeftSide(DEFAULT_CLIENT);
                startingLangCode = translator.Detect(fileContentObject[DEFAULT_CLIENT].ToString());
            }
        }

        private JObject PopulateClientIDs()
        {
            JObject outer = fileContentToken.Value<JObject>();
            for (int i = 0; i < outer.Count; i++)
            {
                custIDs.Add(outer.Children().ElementAt(i).Path);
            }

            return outer;
        }

        private void ReadJsonFromFile(string path)
        {
            string content = File.ReadAllText(path);
            fileContentToken = JToken.Parse(content);
            fileContentObject = JObject.Parse(content);
        }

        private void SaveFile_Button(object sender, RoutedEventArgs e)
        {
            //add new
            if ( radioSelection == RadioSelection.Add )
            {
                if (clients.Contains(addCustID.CustomerID.Text)) {
                    ShowDuplicateClientError();
                    return;
                }

                int val;
                if (!Int32.TryParse(addCustID.CustomerID.Text, out val))
                {
                    MessageBox.Show("Sorry, the lottery id must be a number");
                    return;
                }
            }

            Stream myStream;
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "JSON Files (*.json)|*.json";

            if (saveFile.ShowDialog() == true)
            {
                if ((myStream = saveFile.OpenFile()) != null)
                {
                    string currEditedClientName = (updateCust == null) ? "" : updateCust.UpdateCustBox.SelectedValue.ToString();
                    string json = (radioSelection == RadioSelection.Translate) ? GetTranslatedFileContent() : GetEditedFileContent(currEditedClientName);

                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
                    myStream.Write(bytes, 0, bytes.Length);
                    myStream.Flush();
                    myStream.Close();

                    //if they save it to a new file, open that file
                    if (radioSelection != RadioSelection.Update)
                    {
                        fullPath = saveFile.FileName;
                        currDir = Path.GetDirectoryName(fullPath);
                        fileName = Path.GetFileNameWithoutExtension(fullPath);
                        clients.Clear();
                        clients.Add(DEFAULT_CLIENT);
                        custIDs.Clear();
                        ReadJsonFromFile(fullPath);

                        PopulateClientIDs();

                        updateCust = new UpdateCustomer(this);
                        updateCust.UpdateCustBox.ItemsSource = custIDs;
                        updateCust.UpdateCustBox.SelectedIndex = 0;
                        AddUserControlStep3(updateCust);

                        addCustID = new AddCustomer();
                        AddUserControlStep3(addCustID);
                    }

                    if (currEditedClientName.Equals(""))
                    {
                        currEditedClientName = DEFAULT_CLIENT;
                    }

                    ReloadCurrentLottery(currEditedClientName);
                    StkEditableValues.Children.Clear();
                }

                //Clear Radio Buttons and user controls
                foreach (RadioButton child in RadioButtons.Children)
                {
                    child.IsChecked = false;
                    radioSelection = RadioSelection.Nothing;
                }
                Step3.Children.Clear();
            }
        }

        private void ReloadCurrentLottery(string currLotteryName)
        {
            //reload saved values
            ReadJsonFromFile(fullPath);

            JObject outer = fileContentToken.Value<JObject>();
            //JObject inner = fileContentToken[DEFAULT_CLIENT].Value<JObject>();

            clients = outer.Properties().Select(p => p.Name).ToList();
            //properties = inner.Properties().Select(p => p.Name).ToList();

            StkJSONProperties.Children.Clear();
            foreach (string p in properties)
            {
                JSONProperty prop = new JSONProperty();
                prop.property.Content = p;
                StkJSONProperties.Children.Add(prop);
            }
            ClientChanged(currLotteryName);
            string languageTest = "";
            foreach (string p in properties) {
                languageTest += fileContentObject[DEFAULT_CLIENT][p] + "\n";
            }
            startingLangCode = translator.Detect(languageTest);
        }

        private string GetTranslatedFileContent()
        {
            string quote = "\"";
            char singleQuote = '"';
            string json = "{" + "\n\t" + quote + DEFAULT_CLIENT + quote + ":{";

            foreach (string propName in properties)
            {
                //if saving current user (need to save the edited values)
                string value = ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text;
                json += "\n\t\t"
                    + quote
                        + propName
                    + quote + ": "
                        + quote + value.Replace("\n", "\\n").Replace("\"",singleQuote.ToString()) + quote + ",";
            }
            json += "\n\t},\n}";
            return json;
        }

        private string GetEditedFileContent(string currClientName)
        {
            string quote = "\"";
            char singleQuote = '"';
            char slash = '\\';
            string json = "{";
            foreach (string clientName in clients)
            {
                json += "\n\t" + quote + clientName + quote + ":{";
                foreach (string propName in properties)
                {
                    //if saving current user (need to save the edited values)
                    string value = (clientName.ToLower().Equals(currClientName.ToLower())) ? ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text
                        : fileContentObject[clientName][propName].ToString();

                    if (radioSelection == RadioSelection.Add 
                        || radioSelection == RadioSelection.Update)
                    {
                        json += "\n\t\t"
                            + quote
                                + propName
                            + quote + ": "
                                + quote + value.Replace("\n", "\\n").Replace("\"",slash + singleQuote.ToString()) + quote + ",";
                    }
                    else
                    {
                        json += "\n\t\t"
                            + quote
                                + propName
                            + quote + ": "
                                + quote + value.Replace("\n", "\\n").Replace("\"",singleQuote.ToString()) + quote + ",";
                    }
                }
                json += "\n\t},\n";

            }

            //new customer
            if (radioSelection == RadioSelection.Add)
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
                            + quote + ((JSONValue)StkEditableValues.Children[properties.IndexOf(propName)]).myValue.Text.Replace("\n", "\\n").Replace("\"", slash+singleQuote.ToString()) + quote + ",";
                }
                json += "\n\t},\n";

            }

            json += "}";
            return json;
        }

        private void ShowDuplicateClientError()
        {
            MessageBox.Show("Sorry, there is a Lottery already by that id. Please choose another one.");
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

                if (selectedLanguage != null && !selectedLanguage.Equals("??"))
                {
                    DoTranslation(selectedLanguage);
                }
            }
            else if (!selectedLanguage.Equals("??"))
            {
                DoTranslation(selectedLanguage);
            }
        }

        private void DoTranslation(string selectedLanguage)
        {
            string[] TranslatedValues = new String[properties.Count];
            for (int i = 0; i < TranslatedValues.Length; i++)
            {
                TranslatedValues[i] = fileContentObject[DEFAULT_CLIENT][properties[i]].ToString();
            }
            TranslatedValues = translator.TranslateMultiLines(TranslatedValues, startingLangCode, selectedLanguage);

            StkEditableValues.Children.Clear();
            foreach (string p in TranslatedValues)
            {
                JSONValue eValue = new JSONValue(false);
                eValue.myValue.Text = p.Replace("\\n", "\n");
                StkEditableValues.Children.Add(eValue);
            }
        }

        private void AddNewUser(Object sender, EventArgs e)
        {
            radioSelection = RadioSelection.Add;
            addCustID = new AddCustomer();
            AddUserControlStep3(addCustID);
            ClientChanged(DEFAULT_CLIENT);
        }

        private void UpdateUser(Object sender, EventArgs e)
        {
            radioSelection = RadioSelection.Update;
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
            radioSelection = RadioSelection.Translate;
            toTranslateLang = new AddLanguage();
            AddUserControlStep3(toTranslateLang);
            ClientChanged(DEFAULT_CLIENT);
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

        private void PopulateLeftSide(string clientId)
        {
            StkOriginalValues.Children.Clear();
            foreach (string p in properties)
            {
                JSONValue oValue = new JSONValue(true);
                oValue.myValue.Text = fileContentObject[clientId][p].ToString();
                StkOriginalValues.Children.Add(oValue);
            }
        }

        private void PopulateRightSide(string clientId)
        {
            StkEditableValues.Children.Clear();
            foreach (string p in properties)
            {
                JSONValue oValue = new JSONValue(false);
                oValue.myValue.Text = fileContentObject[clientId][p].ToString();
                StkEditableValues.Children.Add(oValue);
            }
        }

        public void ClientChanged(string clientId)
        {
            PopulateLeftSide(clientId);
            PopulateRightSide(clientId);
        }
    }
}
