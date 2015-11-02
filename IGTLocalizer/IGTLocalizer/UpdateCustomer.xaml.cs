using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for UpdateCustomer.xaml
    /// </summary>
    public partial class UpdateCustomer : UserControl//, INotifyPropertyChanged
    {
        //public string _itemSelected;
        //public string ItemSelected { get { return _itemSelected; } set { _itemSelected = value; RaisedPropertyChanged("_itemSelected"); } }
        MainWindow parent;

        public UpdateCustomer(MainWindow p)
        {
            InitializeComponent();
            parent = p;
        }

        private void UpdateCustBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(UpdateCustBox.SelectedValue != null)
            parent.ClientChanged(UpdateCustBox.SelectedValue.ToString());
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void RaisedPropertyChanged (string propertyName)
        //{
        //    if(PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
