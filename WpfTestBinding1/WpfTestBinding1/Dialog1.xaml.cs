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

namespace WpfTestBinding1
{
    /// <summary>
    /// Interaction logic for Dialog1.xaml
    /// </summary>
    public partial class Dialog1 : Window
    {
        public Dialog1()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            checkBoxSelector.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
