using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace FlightQuery
{
    public class FlightQueryData
    {
        public FlightQueryData()
        {
        }

        //public delegate void LogOutputHandler(object sender, LogEventArgs le);
        //public event LogOutputHandler LogOutput;

        //public delegate void SetPanelVisibilityHandler(object sender, PanelVisibilityEventArgs pve);
        //public event SetPanelVisibilityHandler SetPanelVisibility;

        private bool _autodownload = true;
        public bool IsAutoDownload
        {
            get { return _autodownload; }
            set { _autodownload = value; }
        }
        private bool _autopreprocess = true;
        public bool IsAutoPreprocess
        {
            get { return _autopreprocess; }
            set { _autopreprocess = value; }
        }
        private bool _autoreadssim = true;
        public bool IsAutoReadSSIM
        {
            get { return _autoreadssim; }
            set { _autoreadssim = value; }
        }
        private bool _autopostprocess = true;
        public bool IsAutoPostprocess
        {
            get { return _autopostprocess; }
            set { _autopostprocess = value; }
        }

        private string errMsg = string.Empty;

        public async Task<bool> DownloadAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
            return false;
        }
        public async Task<bool> PreprocessAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
            return true;
        }
        public async Task<bool> ReadSSIMAsync(string fname)
        {
            if (fname == "") return false;
            await Task.Run(() => Thread.Sleep(1000));
            return true;
        }
        public async Task<bool> PostprocessAsync()
        {
            await Task.Run(() => Thread.Sleep(1000));
            return true;
        }

        public async Task<bool> AutoDownloadAsync(MainWindow mw, bool succession)
        {
            bool result = succession;
            if (IsAutoDownload)
            {
                result = await DownloadAsync();
                if (!result)
                    mw.WriteLineToLog("Ошибка автоматической загрузки расписания. Проверьте настройки программы. Рекомендуется обновить расписание вручную.", false);
            }
            return result;
        }

        public async Task<bool> AutoPreprocessAsync(MainWindow mw, bool succession)
        {
            bool result = succession;
            if (result && IsAutoPreprocess)
            {
                result = await PreprocessAsync();
                if (!result)
                    mw.WriteLineToLog("Ошибка предварительной обработки. Проверьте настройки программы. Рекомендуется обновить расписание вручную.", false);
            }
            return result;
        }

        public async Task<bool> AutoReadSSIMAsync(MainWindow mw, bool succession)
        {
            bool result = succession;
            string SSIMfilename = string.Empty;
            if (!(result && IsAutoReadSSIM))
            {
                SSIMfilename = await mw.GetUserSSIMFileNameAsync();
            }
            if (SSIMfilename == "")
            {
                mw.WriteLineToLog("Расписание рейсов не обновлено.", true);
            }
            else
            {
                mw.WriteLineToLog("Выбран " + System.IO.Path.GetFileName(SSIMfilename), true);
                result = await ReadSSIMAsync(SSIMfilename);
                if (!result)
                {
                    mw.WriteLineToLog("Ошибка чтения SSIM.", true);
                }
            }
            return result;
        }

        public async Task<bool> AutoPostprocessAsync(MainWindow mw, bool succession)
        {
            bool result = succession;
            if (IsAutoPostprocess)
            {
                if (result)
                {
                    result = await PostprocessAsync();
                    if (!result)
                        mw.WriteLineToLog("Ошибка постобработки. Проверьте настройки программы.", false);
                }
                else
                    mw.WriteLineToLog("Постобработка пропущена.", false);
            }
            return result;
        }

        public async Task<bool> AutoRunAsync(MainWindow mainwin)
        {
            bool midresult = true;
            midresult = await AutoDownloadAsync(mainwin, midresult);
            midresult = await AutoPreprocessAsync(mainwin, midresult);
            midresult = await AutoReadSSIMAsync(mainwin, midresult);
            midresult = await AutoPostprocessAsync(mainwin, midresult);
            return midresult;
        }
    }
}
