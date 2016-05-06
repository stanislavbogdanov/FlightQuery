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

namespace WpfTestBinding1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyData mwd;

        public MainWindow()
        {
            InitializeComponent();
            mwd = (MyData)TryFindResource("MyDataInstance");
            mwd.MySelector = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Dialog1 d = new Dialog1();
            d.Owner = this;
            d.ShowDialog();
        }
    }
}
