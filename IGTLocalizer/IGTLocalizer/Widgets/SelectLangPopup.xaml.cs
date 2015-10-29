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
using System.Windows.Shapes;

namespace IGTLocalizer.Widgets
{
    /// <summary>
    /// Interaction logic for SelectLangPopup.xaml
    /// </summary>
    public partial class SelectLangPopup : Window
    {
        private LanguageSelector langBox;
        public  string selectedLang { get; private set; }
        public SelectLangPopup()
        {
            InitializeComponent();
            langBox = new LanguageSelector();
            langCombo.Children.Add(langBox);
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            selectedLang = langBox.selectLang;
            this.DialogResult = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
