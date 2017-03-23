using System;
using System.Collections.ObjectModel;
using System.Windows;
using Newtonsoft.Json;

// ReSharper disable EmptyGeneralCatchClause

namespace Client.Classes
{
    public class Settings
    {
        public static ObservableCollection<Settings> AccountsSettings { get; set; } = new ObservableCollection<Settings>();
        static Settings _instance;

        public static Settings Current
        {
            get { return _instance ?? (_instance = new Settings()); }
            set { _instance = value; }
        }

        private volatile string _login;
        private volatile string _password;
        private volatile string _server;
        private volatile Languages _language;
        private volatile Themes _theme;
        private volatile bool _autoupdate = true;
        private volatile uint _autoupdateinterval = 1;

        public enum Themes {  LightGreen, Dark,  Metal, Purple, DarkCyan };
        public enum Languages { Russian, English };

        public string Login { get { return _login; } set { _login = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string Server { get { return _server; } set { _server = value; } }
        public Languages Language
        {
            get { return _language; }
            set
            {
                _language = value;
                var uri = new Uri("Languages\\" + _language.ToString("G") + ".xaml", UriKind.Relative);
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                Main.GetMainWindow().Notified(null, null);
                Save();
            }
        }
        public Themes Theme
        {
            get { return _theme; }
            set
            {
                _theme = value;
                var uri = new Uri("Themes\\" + _theme.ToString("G") + ".xaml", UriKind.Relative);
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Clear();
                Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                Save();
            }
        }

        public bool AutoUpdate
        {
            get { return _autoupdate; }
            set { _autoupdate = value; }
        } 
        public uint AutoUpdateInterval { get { return _autoupdateinterval; } set { _autoupdateinterval = value < 1 ? 1 : value; } }
        public static void Save()
        {
            try
            {
                Controller.SaveToFile(JsonConvert.SerializeObject(AccountsSettings));
            }
            catch { }
        }

        public static void Load()
        {
            try
            {
                AccountsSettings =
                    JsonConvert.DeserializeObject<ObservableCollection<Settings>>(Controller.LoadFromFile() ?? "") ??
                    new ObservableCollection<Settings>();
                //MessageBox.Show(Get().Login + " " + Get().Password + " " + Get().Server + " " + Get().Language + " " + Get().Theme);
            }
            catch
            {
            }
        }
    }
}
