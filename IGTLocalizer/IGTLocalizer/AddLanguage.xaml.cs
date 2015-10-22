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
using IGTLocalizer.Widgets;

namespace IGTLocalizer
{
    /// <summary>
    /// Interaction logic for AddLanguage.xaml
    /// </summary>
    public partial class AddLanguage : UserControl
    {
        public LanguageSelector ls;
        public AddLanguage()
        {
            InitializeComponent();
            ls = new LanguageSelector();
            myGrid.Children.Add(ls);
        }
    }
}
