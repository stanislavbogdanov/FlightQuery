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

namespace FlightQuery
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public MainWindow MainWindowOwner
        {
            get { return Owner as MainWindow; }
            set { Owner = value; }
        }
        public SettingsWindow(MainWindow owner)
        {
            InitializeComponent();
            MainWindowOwner = owner;
            //checkBoxAutoDownload.IsChecked = owner.AutoDownload;
        }

        private void btnSettingsOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            //MainWindow owner = (MainWindow)Owner;
            //owner.AutoDownload = checkBoxAutoDownload.IsChecked.GetValueOrDefault();
        }
    }
}
