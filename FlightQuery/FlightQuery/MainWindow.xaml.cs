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
        private bool _autodownload = true;
        public bool IsAutoDownload
        {
            get { return _autodownload; }
            set { _autodownload = value; }
        }
        public bool IsAutoPreprocess
        {
            get { return true; }
            set { }
        }
        public bool IsAutoReadSSIM
        {
            get { return true; }
            set { }
        }
        public bool IsAutoPostprocess
        {
            get { return true; }
            set { }
        }

        private string errMsg = string.Empty;
        private async Task<bool> DownloadAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
            return false;
        }
        private async Task<bool> PreprocessAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
            return true;
        }
        private async Task<bool> ReadSSIMAsync(string fname)
        {
            if (fname == "") return false;
            await Task.Run(() => Thread.Sleep(1000));
            return true;
        }
        private async Task<bool> PostprocessAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
            return true;
        }

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
            bool successStory = true;

            // Stage 1 - Download
            if (IsAutoDownload)
            {
                successStory = await DownloadAsync();
                if (!successStory) {
                    WriteLineToLog("Ошибка автоматической загрузки расписания. Проверьте настройки программы. Рекомендуется обновить расписание вручную.", 
                                   false);
                }
            }

            // Stage 2 - Preprocessing
            if (successStory && IsAutoPreprocess)
            {
                successStory = await PreprocessAsync();
                if (!successStory) {
                    WriteLineToLog("Ошибка предварительной обработки. Проверьте настройки программы. Рекомендуется обновить расписание вручную.", 
                                   false);
                }
            }

            // Stage 3 - Reading SSIM file
            string SSIMfname = string.Empty;
            if (!(successStory && IsAutoReadSSIM))
            {
                panelTimetableUpdate.Visibility = Visibility.Visible;
                try
                {
                    SSIMfname = await GetUserSSIMFileName();
                }
                catch
                {
                    SSIMfname = string.Empty;
                }
                panelTimetableUpdate.Visibility = Visibility.Collapsed;
            }
            if (SSIMfname == "")
                WriteLineToLog("Расписание рейсов не обновлено.", true);
            else
            {
                WriteLineToLog("Выбран " + System.IO.Path.GetFileName(SSIMfname), true);
                successStory = await ReadSSIMAsync(SSIMfname);
                if (!successStory)
                {
                    WriteLineToLog("Ошибка чтения SSIM.", true);
                }
            }

            // Stage 4 - Postprocessing
            if (IsAutoPostprocess)
            {
                if (successStory)
                {
                    successStory = await PostprocessAsync();
                    if (!successStory)
                    {
                        WriteLineToLog("Ошибка постобработки. Проверьте настройки программы.", false);
                    }
                }
                else
                {
                    WriteLineToLog("Постобработка пропущена.", false);
                }
            }

            // Stage 5 - Select Date Range
            panelDateRange.Visibility = Visibility.Visible;
        }

        private void btnSSIMOpen_Click(object sender, RoutedEventArgs e)
        {
            openSSIMFileDialog.ShowDialog();
        }

        private Task<string> GetUserSSIMFileName()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
            openSSIMFileDialog.FileOk += delegate { tcs.SetResult(openSSIMFileDialog.FileName); };
            btnSSIMSkip.Click += delegate { tcs.SetCanceled(); };
            return tcs.Task;
        }

        private void WriteLineToLog(string msg, bool clear)
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow setwin = new SettingsWindow(this);
            setwin.ShowDialog();
        }
    }
}
