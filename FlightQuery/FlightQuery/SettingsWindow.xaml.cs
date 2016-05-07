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
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void btnSettingsOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            checkBoxAutoDownload.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            checkBoxAutoPreprocess.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            checkBoxAutoReadSSIM.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            checkBoxAutoPostprocess.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            txtUpdateURL.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtSaveUpdateAs.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtAutoPreprocessCommand.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            txtSSIMFileName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            checkBoxKeepUpdate.GetBindingExpression(CheckBox.IsCheckedProperty).UpdateSource();
            txtAutoPostprocessCommand.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}
