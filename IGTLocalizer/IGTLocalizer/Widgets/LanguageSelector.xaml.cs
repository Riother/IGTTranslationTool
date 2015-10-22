using System;
using System.Collections.Generic;
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

namespace IGTLocalizer.Widgets
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LanguageSelector : UserControl
    {
        public Dictionary<string, string> Lang { get; set; }
        public string selectLang;
        public LanguageSelector()
        {

            InitializeComponent();
            Lang = new Dictionary<string, string>
            {
            {"Albanian", "sq"},
            {"Arabian", "ar"},
            {"Armenian", "hy"},
            {"Azeri", "az"},
            {"Belarusian", "be"},
            {"Bosnian", "bs"},
            {"Bulgarian", "bg"},
            {"Catalan", "ca"},
            {"Croatian", "hr"},
            {"Czech", "cs"},
            {"Chinese", "zh"},
            {"Danish", "da"},
            {"Dutch", "nl"},
            {"English", "en"},
            {"Estonian", "et"},
            {"Finnish", "fi"},
            {"French", "fr"},
            {"Georgian", "ka"},
            {"German", "de"},
            {"Greek", "el"},
            {"Hebrew", "he"},
            {"Hungarian", "hu"},
            {"Icelandic", "is"},
            {"Indonesian", "id"},
            {"Italian", "it"},
            {"Japanese", "ja"},
            {"Korean", "ko"},
            {"Latvian", "lv"},
            {"Lithuanian", "lt"},
            {"Macedonian", "mk"},
            {"Malay", "ms"},
            {"Maltese", "mt"},
            {"Norwegian", "no"},
            {"Polish", "pl"},
            {"Portuguese", "pt"},
            {"Romanian", "ro"},
            {"Russian", "ru"},
            {"Spanish", "es"},
            {"Serbian", "sr"},
            {"Slovak", "sk"},
            {"Slovenian", "sl"},
            {"Swedish", "sv"},
            {"Thai", "th"},
            {"Turkish", "tr"},
            {"Ukrainian", "uk"},
            {"Vietnamese", "vi"}


            };

            SelectALanguage.ItemsSource = Lang;
            SelectALanguage.SelectedIndex = 13;
            //SelectALanguage.DisplayMemberPath = "Key";
            //SelectALanguage.SelectedValuePath = "Value";
           // temp.DataContext = SelectALanguage.SelectedValue;
        }

        private void language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            KeyValuePair<string, string> selob =
                (KeyValuePair<string, string>)SelectALanguage.SelectedItem;
            //temp.Content = SelectALanguage;
            selectLang = selob.Value;
        }
    }
}
