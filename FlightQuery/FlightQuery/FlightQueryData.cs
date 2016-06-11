using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.IO;
using System.Data.SQLite;

namespace FlightQuery
{
    public class FlightQueryData
    {
        public FlightQueryData()
        {
            PrepareConfig();
            ReadOptionsFromConfig();
            if (!File.Exists(Opt_DBFileName)) PrepareDataBase();
        }

        private void PrepareDataBase()
        {
            string createTableQuery = @"DROP TABLE IF EXISTS [Timetable]; "+
                          "CREATE TABLE [Timetable] (" +
                          "[id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                          "[AirlineDesignator] TEXT NOT NULL, " +
                          "[FlightNumber] INTEGER NOT NULL, "+
                          "[DepartureStation] TEXT NOT NULL, " +
                          "[DepartureDateTime] TEXT NOT NULL, " +
                          "[DepartureLocalTimeVariation] TEXT NOT NULL," +
                          "[ArrivalStation] TEXT NOT NULL, " +
                          "[ArrivalDateTime] TEXT NOT NULL, " +
                          "[ArrivalLocalTimeVariation] TEXT NOT NULL, " +
                          "[AircraftType] TEXT NOT NULL, " +
                          "[AircraftConfiguration] TEXT NOT NULL" +
                          "); " +
                          "CREATE INDEX [FNumIndex] ON [Timetable] ([FlightNumber]); " +
                          "CREATE INDEX [DeptStIndex] ON [Timetable] ([DepartureStation]); " +
                          "CREATE INDEX [DeptDTimeIndex] ON [Timetable] ([DepartureDateTime]); " +
                          "CREATE INDEX [ArvlStIndex] ON [Timetable] ([ArrivalStation]); " +
                          "CREATE INDEX [ArvlDTimeIndex] ON [Timetable] ([ArrivalDateTime]); " +
                          "CREATE TABLE [TDescriptions] (" +
                          "[TableName] TEXT NOT NULL PRIMARY KEY, " +
                          "[Caption] TEXT NOT NULL" +
                          "); " +
                          "CREATE TABLE [QDescriptions] (" +
                          "[QueryName] TEXT NOT NULL PRIMARY KEY, " +
                          "[CommandText] TEXT NOT NULL, " +
                          "[Caption] TEXT NOT NULL" +
                          "); " +
                          "INSERT INTO [TDescriptions] ([TableName], [Caption]) " +
                          "VALUES ('Timetable','Перечень всех известных рейсов'); " +
                          "INSERT INTO [TDescriptions] ([TableName], [Caption]) " +
                          "VALUES ('TDescriptions','Заголовки таблиц'); ";

            SQLiteConnection.CreateFile(Opt_DBFileName);        
            using (SQLiteConnection con = new SQLiteConnection("data source="+Opt_DBFileName))
            {
                using (SQLiteCommand com = new SQLiteCommand(con))
                {
                    con.Open();                             // Open the connection to the database
                    com.CommandText = createTableQuery;     // Set CommandText to our query that will create the table
                    com.ExecuteNonQuery();                  // Execute the query
                    con.Close();                            // Close the connection to the database
                }
            }
        }

        #region Options - Config reading/writing
        private const ulong _ALL_OPTIONS_BITS = 0x7FF;
        private const ulong _OPT_ISAUTODOWNLOAD_MASK = 0x1;
        private const ulong _OPT_ISAUTOPREPROCESS_MASK = 0x2;
        private const ulong _OPT_ISAUTOREADSSIM_MASK = 0x4;
        private const ulong _OPT_ISAUTOPOSTPROCESS_MASK = 0x8;
        private const ulong _OPT_UPDATEURL_MASK = 0x10;
        private const ulong _OPT_SAVEUPDATEAS_MASK = 0x20;
        private const ulong _OPT_AUTOPREPROCESSCOMMAND_MASK = 0x40;
        private const ulong _OPT_SSIMFILENAME_MASK = 0x80;
        private const ulong _OPT_ISKEEPUPDATE_MASK = 0x100;
        private const ulong _OPT_AUTOPOSTPROCESSCOMMAND_MASK = 0x200;
        private const ulong _OPT_DBFILENAME_MASK = 0x400;

        private ulong _modifiedoptionsbits = 0;
        private ulong ModifiedOptionsBits
        {
            get { return _modifiedoptionsbits; }
            set
            {
                _modifiedoptionsbits = value;
                if (_modifiedoptionsbits == _ALL_OPTIONS_BITS)
                {
                    // All options was modified
                    SaveOptionsToConfig();
                    _modifiedoptionsbits = 0;
                }
            }
        }

        private bool _opt_isautodownload = false;
        public bool Opt_IsAutoDownload
        {
            get { return _opt_isautodownload; }
            set
            {
                _opt_isautodownload = value;
                ModifiedOptionsBits |= _OPT_ISAUTODOWNLOAD_MASK; // Set bit "modified" for this option
            }
        }

        private bool _opt_isautopreprocess = false;
        public bool Opt_IsAutoPreprocess
        {
            get { return _opt_isautopreprocess; }
            set
            {
                _opt_isautopreprocess = value;
                ModifiedOptionsBits |= _OPT_ISAUTOPREPROCESS_MASK; // Set bit "modified" for this option
            }
        }

        private bool _opt_isautoreadssim = false;
        public bool Opt_IsAutoReadSSIM
        {
            get { return _opt_isautoreadssim; }
            set
            {
                _opt_isautoreadssim = value;
                ModifiedOptionsBits |= _OPT_ISAUTOREADSSIM_MASK; // Set bit "modified" for this option
            }
        }

        private bool _opt_isautopostprocess = false;
        public bool Opt_IsAutoPostprocess
        {
            get { return _opt_isautopostprocess; }
            set
            {
                _opt_isautopostprocess = value;
                ModifiedOptionsBits |= _OPT_ISAUTOPOSTPROCESS_MASK; // Set bit "modified" for this option
            }
        }

        private string _opt_updateurl = "";
        public string Opt_UpdateURL
        {
            get { return _opt_updateurl; }
            set
            {
                if (value == "")
                    _opt_updateurl = value;
                else
                {
                    UriBuilder ub = new UriBuilder(value);
                    _opt_updateurl = ub.ToString();
                }
                ModifiedOptionsBits |= _OPT_UPDATEURL_MASK; // Set bit "modified" for this option
            }
        }

        private string _opt_saveupdateas = "";
        public string Opt_SaveUpdateAs
        {
            get { return _opt_saveupdateas; }
            set
            {
                _opt_saveupdateas = (value == "") ? GetFileNameFromURL(Opt_UpdateURL) : value;
                ModifiedOptionsBits |= _OPT_SAVEUPDATEAS_MASK; // Set bit "modified" for this option
            }
        }

        private string _opt_autopreprocesscommand = "";
        public string Opt_AutoPreprocessCommand
        {
            get { return _opt_autopreprocesscommand; }
            set
            {
                _opt_autopreprocesscommand = value;
                ModifiedOptionsBits |= _OPT_AUTOPREPROCESSCOMMAND_MASK; // Set bit "modified" for this option
            }
        }

        private string _opt_ssimfilename = "";
        public string Opt_SSIMFileName
        {
            get { return _opt_ssimfilename; }
            set
            {
                _opt_ssimfilename = value;
                ModifiedOptionsBits |= _OPT_SSIMFILENAME_MASK; // Set bit "modified" for this option
            }
        }

        private bool _opt_iskeepupdate = true;
        public bool Opt_IsKeepUpdate
        {
            get { return _opt_iskeepupdate; }
            set
            {
                _opt_iskeepupdate = value;
                ModifiedOptionsBits |= _OPT_ISKEEPUPDATE_MASK; // Set bit "modified" for this option
            }
        }

        private string _opt_autopostprocesscommand = "";
        public string Opt_AutoPostprocessCommand
        {
            get { return _opt_autopostprocesscommand; }
            set
            {
                _opt_autopostprocesscommand = value;
                ModifiedOptionsBits |= _OPT_AUTOPOSTPROCESSCOMMAND_MASK; // Set bit "modified" for this option
            }
        }

        private string _opt_dbfilename = "";
        public string Opt_DBFileName
        {
            get { return _opt_dbfilename; }
            set
            {
                _opt_dbfilename = value;
                ModifiedOptionsBits |= _OPT_DBFILENAME_MASK; // Set bit "modified" for this option
            }
        }

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
                if (appSettings.Settings["UpdateURL"] == null) appSettings.Settings.Add("UpdateURL", "");
              
                if (appSettings.Settings["SaveUpdateAs"] == null)
                    appSettings.Settings.Add("SaveUpdateAs", GetFileNameFromURL(appSettings.Settings["UpdateURL"].Value));

                if (appSettings.Settings["AutoPreprocessCommand"] == null) appSettings.Settings.Add("AutoPreprocessCommand", "");
                if (appSettings.Settings["SSIMFileName"] == null) appSettings.Settings.Add("SSIMFileName", "");
                if (appSettings.Settings["IsKeepUpdate"] == null) appSettings.Settings.Add("IsKeepUpdate", Boolean.TrueString);
                if (appSettings.Settings["AutoPostprocessCommand"] == null) appSettings.Settings.Add("AutoPostprocessCommand", "");
                if (appSettings.Settings["txtDBFileName"] == null) appSettings.Settings.Add("txtDBFileName", "");
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
                if (Opt_SaveUpdateAs == "")
                {
                    Opt_SaveUpdateAs = GetFileNameFromURL(Opt_UpdateURL);
                }

                Opt_AutoPreprocessCommand = (string)ast.GetValue("AutoPreprocessCommand", typeof(string));
                Opt_SSIMFileName = (string)ast.GetValue("SSIMFileName", typeof(string));
                Opt_IsKeepUpdate = (bool)ast.GetValue("IsKeepUpdate", typeof(bool));
                Opt_AutoPostprocessCommand = (string)ast.GetValue("AutoPostprocessCommand", typeof(string));
                Opt_DBFileName = (string)ast.GetValue("txtDBFileName", typeof(string));
                if (Opt_DBFileName == "")
                {
                    Opt_DBFileName = "Flights.sqlite3";
                }
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
                appSettings.Settings["txtDBFileName"].Value = Opt_DBFileName;

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception e)
            {
                ConfigErrorMessage(e.Message);
            }
        }

        private string GetFileNameFromURL (string sURL)
        {
            if (sURL == "") return "";
            string fn = "";
            try
            {
                UriBuilder ub = new UriBuilder(sURL);
                fn = System.IO.Path.GetFileName(ub.Path);
            }
            catch (UriFormatException)
            {
                fn = "";
            }
            if (fn == "") fn = "file.dat";
            return fn;
        }

        private void ConfigErrorMessage(string msgtext)
        {
            MessageBox.Show(App.Current.Windows[0],
                    msgtext,
                    "Ошибка конфигурационного файла",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
        }
        #endregion

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

    }
}
