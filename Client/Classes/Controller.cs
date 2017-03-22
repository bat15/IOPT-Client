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
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace Client
{
    static class Controller
    {
        private const string Key = "Q1e3@3e344";
        private static readonly byte[] Salt = { 0x15, 0xdc, 0xf5, 0x40, 0xad, 0x5d, 0x7a, 0x0e, 0xc5, 0xae, 0x89, 0xaf, 0x4d, 0xaa, 0xc2, 0x3c };

        public static void Close()
        {
            Settings.Save();
            Application.Current.Shutdown();
        }

        public static void SaveToFile(string data)
        {
            if (Key.Length == 0) return;
            using (var myAes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, Salt);
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
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, Salt);
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
            var scripts = new ObservableCollection<Script>()
            {
                new Script(-1,"Name","Name","Some code",-1),
                new Script(-2,"Name1","Name1","Some other code",-1)
            };
            var props = new ObservableCollection<Property>()
            {
                new Property(-1,"LED","LED",(int)TypeCode.Boolean,-1,scripts,"false"),
                new Property(-2,"XPosition","XPosition",(int)TypeCode.Int32,-1,null,"0"),
                new Property(-3,"ButtonState","ButtonState",(int)TypeCode.Boolean,-1,null,"false")
            };

            var obj = new ObservableCollection<Object> { new Object(-1, "TestObject", "TestObject", -1, props) };

            var model = new Model(-1, "TestModel", "TestModel", obj);
            Snapshot.current.models.Add(model);
        }


        static byte[] EncryptStringToBytesAes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
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
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException(nameof(IV));
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
