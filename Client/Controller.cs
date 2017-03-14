using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Collections;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;

namespace Client
{
    static class Controller
    {
        private static readonly string key = "Q1e3@3e344";
        private static readonly byte[] salt = new byte[] { 0x15, 0xdc, 0xf5, 0x40, 0xad, 0x5d, 0x7a, 0x0e, 0xc5, 0xae, 0x89, 0xaf, 0x4d, 0xaa, 0xc2, 0x3c };
        private static IPEndPoint server;

        public static bool Connect(string adress, string login, string password)
        {
            if (string.IsNullOrWhiteSpace(adress) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password)) return false;
            IPHostEntry hostEntry;

            hostEntry = Dns.GetHostEntry(adress);

            if (hostEntry.AddressList.Length <= 0) return false;
            try
            {
                GetDataFromServer(hostEntry.AddressList[0]);
                Snapshot.Current.LastUpdate = DateTimeOffset.Now;
                return true;
            }
            catch { return false; }
        }

        public static void SendDataToServer()
        {

        }

        public static async void GetDataFromServer(IPAddress ip)
        {
            try
            {
                string url = "http://" + ip.ToString() + ":8080/IOPT-Server/models/get_test";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                string resp;
                using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                {
                    resp = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrWhiteSpace(resp)) throw new Exception("Can't get data from server");
                Deserialize(resp);
            }
            catch {}
            //Message.Show(resp, "");
        }

        public static string Serialize()
        {
            return JsonConvert.SerializeObject(Snapshot.Current);
        }

        public static void Deserialize(string model)
        {
            //Message.Show((a as Snapshot).Models.Count.ToString(), "");
           Snapshot.Current = JsonConvert.DeserializeObject<Snapshot>(model);          
        }

        public static void Close()
        {
            Settings.Get().Save();
            Application.Current.Shutdown();
        }
        public static void SaveToFile(string data)
        {
            if (key.Length == 0) return;
            using (var myAes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
                myAes.Key = pdb.GetBytes(32);
                myAes.IV = pdb.GetBytes(16);
                using (StreamWriter sw = new StreamWriter("user.data", false, Encoding.Default))
                {
                    sw.Write(Encoding.Default.GetString(EncryptStringToBytesAes(data, myAes.Key, myAes.IV)));
                }
            }
        }

        public static string LoadFromFile()
        {
            if (!File.Exists("user.data")) return null;
            using (var myAes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(key, salt);
                myAes.Key = pdb.GetBytes(32);
                myAes.IV = pdb.GetBytes(16);
                byte[] data;
                using (StreamReader sr = new StreamReader("user.data", Encoding.Default))
                {
                    data = Encoding.Default.GetBytes(sr.ReadToEnd());
                }
                return DecryptStringFromBytesAes(data, myAes.Key, myAes.IV);
            }
        }

        public static void GenerateTestData()
        {
            var model = new Model("Тепличное хозяйство");
            var obj = new List<Object>() { new Object("Теплица с огурцами") { }
            ,new Object("Теплица с картошкой")
            ,new Object("Теплица с трупами") };

            var props = new List<Property>()
            {
                new Property("Температура",TypeCode.Double) { Value="25"},
                new Property("Влажность",TypeCode.Int32){ Value="30"},
                new Property("Строка",TypeCode.String){ Value="Some string"},
                new Property("Состояние окна",TypeCode.Boolean){ Value="true"},
                new Property("Состояние насоса",TypeCode.Boolean){ Value="false"},
                new Property("Свет лампы",TypeCode.Double){ Value="0"}
            };

            var scripts = new List<Script>() {
                new Script("Script1","JS")
            };

            foreach (var a in props)
            {
                foreach (var b in scripts)
                    a.Scripts.Add(new Script(b));
            }
            foreach (var a in obj)
            {
                foreach (var b in props)
                    a.Properties.Add(new Property(b));
            }
            foreach (var b in obj)
                model.Objects.Add(new Object(b));

            model = new Model("Умный дом");
            obj = new List<Object>() { new Object("Комната с овоощами") { }
            ,new Object("Ванная") };

            props = new List<Property>()
            {
                new Property("Температура",TypeCode.Double) { Value="20"},
                new Property("Влажность",TypeCode.Double){ Value="10"},
                new Property("Освещенность",TypeCode.Double){ Value="0"},
                new Property("Состояние окна",TypeCode.Boolean){ Value="false"},
                new Property("Состояние насоса",TypeCode.Boolean){ Value="false"},
                new Property("Свет лампы",TypeCode.Double){ Value="100"}
            };

            scripts = new List<Script>() {
                new Script("Script2","JS")
            };

            foreach (var a in props)
            {
                foreach (var b in scripts)
                    a.Scripts.Add(new Script(b));
            }
            foreach (var a in obj)
            {
                foreach (var b in props)
                    a.Properties.Add(new Property(b));
            }
            foreach (var b in obj)
                model.Objects.Add(new Object(b));
        }


        static byte[] EncryptStringToBytesAes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Создаем объект класса AES
            // с определенным ключом и IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Создаем объект, который определяет основные операции преобразований.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Создаем поток для шифрования.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Записываем в поток все данные.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            //Возвращаем зашифрованные байты из потока памяти.
            return encrypted;

        }

        static string DecryptStringFromBytesAes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            string plaintext;
            // Создаем объект класса AES,
            // Ключ и IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Создаем объект, который определяет основные операции преобразований.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Создаем поток для расшифрования.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

    }
    public class Settings
    {
        static Settings instance;

        public static Settings Get()
        {
            if (instance == null) instance = new Settings();
            return instance;
        }
        private Settings() { }

        private string _login { get; set; }
        private string _password { get; set; }
        private string _server { get; set; }
        private string _language { get; set; }
        private string _theme { get; set; }

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
                if(value!=null)
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

        public void Save()
        {
            Controller.SaveToFile(JsonConvert.SerializeObject(instance));
        }

        public static void Load()
        {
            instance=JsonConvert.DeserializeObject<Settings>(Controller.LoadFromFile()??"");
            //MessageBox.Show(Get().Login + " " + Get().Password + " " + Get().Server + " " + Get().Language + " " + Get().Theme);
        }
    }

    public enum TransliterationType
    {
        Gost,
        ISO
    }
    public static class Transliteration
    {
        private static Dictionary<string, string> gost = new Dictionary<string, string>(); //ГОСТ 16876-71
        private static Dictionary<string, string> iso = new Dictionary<string, string>(); //ISO 9-95
        public static string Front(string text)
        {
            return Front(text, TransliterationType.ISO);
        }
        public static string Front(string text, TransliterationType type)
        {
            string output = text;

            output = Regex.Replace(output, @"\s|\.|\(", " ");
            output = Regex.Replace(output, @"\s+", " ");
            output = Regex.Replace(output, @"[^\s\w\d-]", "");
            output = output.Trim();

            Dictionary<string, string> tdict = GetDictionaryByType(type);

            foreach (KeyValuePair<string, string> key in tdict)
            {
                output = output.Replace(key.Key, key.Value);
            }
            return output;
        }
        public static string Back(string text)
        {
            return Back(text, TransliterationType.ISO);
        }
        public static string Back(string text, TransliterationType type)
        {
            string output = text;
            Dictionary<string, string> tdict = GetDictionaryByType(type);

            foreach (KeyValuePair<string, string> key in tdict)
            {
                output = output.Replace(key.Value, key.Key);
            }
            return output;
        }

        private static Dictionary<string, string> GetDictionaryByType(TransliterationType type)
        {
            Dictionary<string, string> tdict = iso;
            if (type == TransliterationType.Gost) tdict = gost;
            return tdict;
        }

        static Transliteration()
        {
            gost.Add("Є", "EH");
            gost.Add("І", "I");
            gost.Add("і", "i");
            gost.Add("№", "#");
            gost.Add("є", "eh");
            gost.Add("А", "A");
            gost.Add("Б", "B");
            gost.Add("В", "V");
            gost.Add("Г", "G");
            gost.Add("Д", "D");
            gost.Add("Е", "E");
            gost.Add("Ё", "JO");
            gost.Add("Ж", "ZH");
            gost.Add("З", "Z");
            gost.Add("И", "I");
            gost.Add("Й", "JJ");
            gost.Add("К", "K");
            gost.Add("Л", "L");
            gost.Add("М", "M");
            gost.Add("Н", "N");
            gost.Add("О", "O");
            gost.Add("П", "P");
            gost.Add("Р", "R");
            gost.Add("С", "S");
            gost.Add("Т", "T");
            gost.Add("У", "U");
            gost.Add("Ф", "F");
            gost.Add("Х", "KH");
            gost.Add("Ц", "C");
            gost.Add("Ч", "CH");
            gost.Add("Ш", "SH");
            gost.Add("Щ", "SHH");
            gost.Add("Ъ", "'");
            gost.Add("Ы", "Y");
            gost.Add("Ь", "");
            gost.Add("Э", "EH");
            gost.Add("Ю", "YU");
            gost.Add("Я", "YA");
            gost.Add("а", "a");
            gost.Add("б", "b");
            gost.Add("в", "v");
            gost.Add("г", "g");
            gost.Add("д", "d");
            gost.Add("е", "e");
            gost.Add("ё", "jo");
            gost.Add("ж", "zh");
            gost.Add("з", "z");
            gost.Add("и", "i");
            gost.Add("й", "jj");
            gost.Add("к", "k");
            gost.Add("л", "l");
            gost.Add("м", "m");
            gost.Add("н", "n");
            gost.Add("о", "o");
            gost.Add("п", "p");
            gost.Add("р", "r");
            gost.Add("с", "s");
            gost.Add("т", "t");
            gost.Add("у", "u");

            gost.Add("ф", "f");
            gost.Add("х", "kh");
            gost.Add("ц", "c");
            gost.Add("ч", "ch");
            gost.Add("ш", "sh");
            gost.Add("щ", "shh");
            gost.Add("ъ", "");
            gost.Add("ы", "y");
            gost.Add("ь", "");
            gost.Add("э", "eh");
            gost.Add("ю", "yu");
            gost.Add("я", "ya");
            gost.Add("«", "");
            gost.Add("»", "");
            gost.Add("—", "-");
            gost.Add(" ", "-");

            iso.Add("Є", "YE");
            iso.Add("І", "I");
            iso.Add("Ѓ", "G");
            iso.Add("і", "i");
            iso.Add("№", "#");
            iso.Add("є", "ye");
            iso.Add("ѓ", "g");
            iso.Add("А", "A");
            iso.Add("Б", "B");
            iso.Add("В", "V");
            iso.Add("Г", "G");
            iso.Add("Д", "D");
            iso.Add("Е", "E");
            iso.Add("Ё", "YO");
            iso.Add("Ж", "ZH");
            iso.Add("З", "Z");
            iso.Add("И", "I");
            iso.Add("Й", "J");
            iso.Add("К", "K");
            iso.Add("Л", "L");
            iso.Add("М", "M");
            iso.Add("Н", "N");
            iso.Add("О", "O");
            iso.Add("П", "P");
            iso.Add("Р", "R");
            iso.Add("С", "S");
            iso.Add("Т", "T");
            iso.Add("У", "U");
            iso.Add("Ф", "F");
            iso.Add("Х", "X");
            iso.Add("Ц", "C");
            iso.Add("Ч", "CH");
            iso.Add("Ш", "SH");
            iso.Add("Щ", "SHH");
            iso.Add("Ъ", "'");
            iso.Add("Ы", "Y");
            iso.Add("Ь", "");
            iso.Add("Э", "E");
            iso.Add("Ю", "YU");
            iso.Add("Я", "YA");
            iso.Add("а", "a");
            iso.Add("б", "b");
            iso.Add("в", "v");
            iso.Add("г", "g");
            iso.Add("д", "d");
            iso.Add("е", "e");
            iso.Add("ё", "yo");
            iso.Add("ж", "zh");
            iso.Add("з", "z");
            iso.Add("и", "i");
            iso.Add("й", "j");
            iso.Add("к", "k");
            iso.Add("л", "l");
            iso.Add("м", "m");
            iso.Add("н", "n");
            iso.Add("о", "o");
            iso.Add("п", "p");
            iso.Add("р", "r");
            iso.Add("с", "s");
            iso.Add("т", "t");
            iso.Add("у", "u");
            iso.Add("ф", "f");
            iso.Add("х", "x");
            iso.Add("ц", "c");
            iso.Add("ч", "ch");
            iso.Add("ш", "sh");
            iso.Add("щ", "shh");
            iso.Add("ъ", "");
            iso.Add("ы", "y");
            iso.Add("ь", "");
            iso.Add("э", "e");
            iso.Add("ю", "yu");
            iso.Add("я", "ya");
            iso.Add("«", "");
            iso.Add("»", "");
            iso.Add("—", "-");
            iso.Add(" ", "-");
        }
    }
}
