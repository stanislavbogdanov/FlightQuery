using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlightQuery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog openSSIMFileDialog = new OpenFileDialog();
        public MainWindow()
        {
            InitializeComponent();
            openSSIMFileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            openSSIMFileDialog.Filter = "SSIM files (*.*)|*.*";
            openSSIMFileDialog.RestoreDirectory = true;
        }

        private void calDateRange_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime d1 = calDateRange.SelectedDates.Min();
            DateTime d2 = calDateRange.SelectedDates.Max().Add(new TimeSpan(23, 59, 59));
            labelDateRange.Content = "From " + d1.ToString() +
                                     " to " + d2.ToString();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            FlightQueryData fqd = (FlightQueryData)App.Current.Resources["FQDataInstance"];
            bool succession = await fqd.AutoRunAsync(this);

            // Stage 5 - Select Date Range
            panelDateRange.Visibility = Visibility.Visible;
        }

        private void btnSSIMOpen_Click(object sender, RoutedEventArgs e)
        {
            openSSIMFileDialog.ShowDialog();
        }

        public async Task<string> GetUserSSIMFileNameAsync()
        {
            string SSIMfname = string.Empty;
            this.Dispatcher.Invoke(() => {
                panelTimetableUpdate.Visibility = Visibility.Visible;
            });            
            try
            {
                SSIMfname = await Task.Run(() =>
                {
                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
                    openSSIMFileDialog.FileOk += delegate { tcs.SetResult(openSSIMFileDialog.FileName); };
                    btnSSIMSkip.Click += delegate { tcs.SetCanceled(); };
                    return tcs.Task;
                });
            }
            catch
            {
                SSIMfname = string.Empty;
            }
            this.Dispatcher.Invoke(() => {
                panelTimetableUpdate.Visibility = Visibility.Collapsed;
            });
            return SSIMfname;
        }

        public void WriteLineToLog(string msg, bool clear)
        {
            this.Dispatcher.Invoke(() => {
                if (clear) log.Inlines.Clear();
                log.Inlines.Add(msg + "\n");
            });

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setwin = new SettingsWindow();
            setwin.Owner = this;
            setwin.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            setwin.ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            FlightQueryData fqd = (FlightQueryData)App.Current.Resources["FQDataInstance"];
            //fqd.Dispose();
        }
    }
}
