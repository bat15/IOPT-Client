using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class Settings
    {
        static Settings instance;

        public static Settings Get()
        {
            if (instance == null) instance = new Settings();
            return instance;
        }
        private Settings() { }

        private volatile string _login;
        private volatile string _password;
        private volatile string _server;
        private volatile string _language;
        private volatile string _theme;
        private volatile bool _autoupdate = false;
        private volatile uint _autoupdateinterval = 1;

        public static List<string> Themes = new List<string> { "Light", "Dark" };

        public static List<string> Languages = new List<string> { "Russian", "English" };

        public string Login { get { return _login; } set { _login = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public string Server { get { return _server; } set { _server = value; } }
        public string Language
        {
            get { return _language; }
            set
            {
                if (value != null)
                {
                    _language = value;
                    var uri = new Uri("Languages\\" + _language + ".xaml", UriKind.Relative);
                    ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                    Application.Current.Resources.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                    Main.GetMainWindow().Notified(null, null);
                    Save();
                }
            }
        }
        public string Theme
        {
            get { return _theme; }
            set
            {
                if (value != null)
                {
                    _theme = value;
                    var uri = new Uri("Themes\\" + _theme + ".xaml", UriKind.Relative);
                    ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
                    Application.Current.Resources.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(resourceDict);
                    Save();
                }
            }
        }

        public bool AutoUpdate { get { return _autoupdate; } set { _autoupdate = value; } }
        public uint AutoUpdateInterval { get { return _autoupdateinterval; } set { _autoupdateinterval = value < 1 ? 1 : value; } }
        public void Save()
        {
            try
            {
                Controller.SaveToFile(JsonConvert.SerializeObject(instance));
            }
            catch { }
        }

        public static void Load()
        {
            instance = JsonConvert.DeserializeObject<Settings>(Controller.LoadFromFile() ?? "");
            //MessageBox.Show(Get().Login + " " + Get().Password + " " + Get().Server + " " + Get().Language + " " + Get().Theme);
        }
    }
}
