using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;

namespace FlightQuery
{
    public class FlightQueryData : IDisposable
    {
        public FlightQueryData()
        {
            PrepareConfig();
            ReadOptionsFromConfig();
        }

        //public delegate void LogOutputHandler(object sender, LogEventArgs le);
        //public event LogOutputHandler LogOutput;

        //public delegate void SetPanelVisibilityHandler(object sender, PanelVisibilityEventArgs pve);
        //public event SetPanelVisibilityHandler SetPanelVisibility;

        public bool Opt_IsAutoDownload
        { get; set; }
        public bool Opt_IsAutoPreprocess
        { get; set; }
        public bool Opt_IsAutoReadSSIM
        { get; set; }
        public bool Opt_IsAutoPostprocess
        { get; set; }
        public string Opt_UpdateURL
        { get; set; }
        public string Opt_SaveUpdateAs
        { get; set; }
        public string Opt_AutoPreprocessCommand
        { get; set; }
        public string Opt_SSIMFileName
        { get; set; }
        public bool Opt_IsKeepUpdate
        { get; set; }
        public string Opt_AutoPostprocessCommand
        { get; set; }

        private void PrepareConfig()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection appSettings = config.AppSettings;
                if (appSettings.Settings["IsAutoDownload"] == null) appSettings.Settings.Add("IsAutoDownload", Boolean.FalseString);
                if (appSettings.Settings["IsAutoPreprocess"] == null) appSettings.Settings.Add("IsAutoPreprocess", Boolean.FalseString);
                if (appSettings.Settings["IsAutoReadSSIM"] == null) appSettings.Settings.Add("IsAutoReadSSIM", Boolean.FalseString);
                if (appSettings.Settings["IsAutoPostprocess"] == null) appSettings.Settings.Add("IsAutoPostprocess", Boolean.FalseString);
                if (appSettings.Settings["UpdateURL"] == null)
                    appSettings.Settings.Add("UpdateURL", "");
                else
                {
                    UriBuilder ub = new UriBuilder(appSettings.Settings["UpdateURL"].Value);
                    appSettings.Settings["UpdateURL"].Value = ub.ToString();
                }
                if (appSettings.Settings["SaveUpdateAs"] == null)
                {
                    if (appSettings.Settings["UpdateURL"].Value == "")
                        appSettings.Settings.Add("SaveUpdateAs", "");
                    else
                    {
                        string fn = "";
                        try
                        {
                            UriBuilder ub = new UriBuilder(appSettings.Settings["UpdateURL"].Value);
                            fn = System.IO.Path.GetFileName(ub.Path);
                        }
                        catch (UriFormatException)
                        {
                            fn = "";
                        }
                        appSettings.Settings.Add("SaveUpdateAs", (fn == "") ? "file.dat" : fn);
                    }
                }
                if (appSettings.Settings["AutoPreprocessCommand"] == null) appSettings.Settings.Add("AutoPreprocessCommand", "");
                if (appSettings.Settings["SSIMFileName"] == null) appSettings.Settings.Add("SSIMFileName", "");
                if (appSettings.Settings["IsKeepUpdate"] == null) appSettings.Settings.Add("IsKeepUpdate", Boolean.TrueString);
                if (appSettings.Settings["AutoPostprocessCommand"] == null) appSettings.Settings.Add("AutoPostprocessCommand", "");
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception e)
            {
                ConfigErrorMessage(e.Message);
                App.Current.Shutdown();
            }
        }

        public void ReadOptionsFromConfig()
        {
            try
            {
                AppSettingsReader ast = new AppSettingsReader();
                Opt_IsAutoDownload = (bool)ast.GetValue("IsAutoDownload", typeof(bool));
                Opt_IsAutoPreprocess = (bool)ast.GetValue("IsAutoPreprocess", typeof(bool));
                Opt_IsAutoReadSSIM = (bool)ast.GetValue("IsAutoReadSSIM", typeof(bool));
                Opt_IsAutoPostprocess = (bool)ast.GetValue("IsAutoPostprocess", typeof(bool));
                Opt_UpdateURL = (string)ast.GetValue("UpdateURL", typeof(string));
                Opt_SaveUpdateAs = (string)ast.GetValue("SaveUpdateAs", typeof(string));
                Opt_AutoPreprocessCommand = (string)ast.GetValue("AutoPreprocessCommand", typeof(string));
                Opt_SSIMFileName = (string)ast.GetValue("SSIMFileName", typeof(string));
                Opt_IsKeepUpdate = (bool)ast.GetValue("IsKeepUpdate", typeof(bool));
                Opt_AutoPostprocessCommand = (string)ast.GetValue("AutoPostprocessCommand", typeof(string));

            }
            catch (Exception e)
            {
                ConfigErrorMessage(e.Message);
                App.Current.Shutdown();
            }
        }

        public void SaveOptionsToConfig()
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                AppSettingsSection appSettings = config.AppSettings;

                appSettings.Settings["IsAutoDownload"].Value = Opt_IsAutoDownload.ToString();
                appSettings.Settings["IsAutoPreprocess"].Value = Opt_IsAutoPreprocess.ToString();
                appSettings.Settings["IsAutoReadSSIM"].Value = Opt_IsAutoReadSSIM.ToString();
                appSettings.Settings["IsAutoPostprocess"].Value = Opt_IsAutoPostprocess.ToString();
                appSettings.Settings["UpdateURL"].Value = Opt_UpdateURL;
                appSettings.Settings["SaveUpdateAs"].Value = Opt_SaveUpdateAs;
                appSettings.Settings["AutoPreprocessCommand"].Value = Opt_AutoPreprocessCommand;
                appSettings.Settings["SSIMFileName"].Value = Opt_SSIMFileName;
                appSettings.Settings["IsKeepUpdate"].Value = Opt_IsKeepUpdate.ToString();
                appSettings.Settings["AutoPostprocessCommand"].Value = Opt_AutoPostprocessCommand;

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

            }
            catch (Exception e)
            {
                ConfigErrorMessage(e.Message);
            }
        }

        private void ConfigErrorMessage(string msgtext)
        {
            MessageBox.Show(App.Current.Windows[0],
                    msgtext,
                    "Ошибка конфигурационного файла",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
        }

        //private string errMsg = string.Empty;

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
            if (Opt_IsAutoDownload)
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
            if (result && Opt_IsAutoPreprocess)
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
            if (!(result && Opt_IsAutoReadSSIM))
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
            if (Opt_IsAutoPostprocess)
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    SaveOptionsToConfig();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FlightQueryData() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
