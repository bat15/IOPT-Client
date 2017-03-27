using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
// ReSharper disable AssignNullToNotNullAttribute

namespace Client.Classes
{
    internal static class Network
    {
        private static CookieContainer _cookies;

        public static bool Connect()
        {
            if (string.IsNullOrWhiteSpace(Settings.Current.Server) || string.IsNullOrWhiteSpace(Settings.Current.Login) || string.IsNullOrWhiteSpace(Settings.Current.Password)) return false;
            try
            {
                var url = "http://" + Settings.Current.Server + "/login";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Timeout = 5000;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(new { login = Settings.Current.Login, password = Settings.Current.Password }));
                }

                var httpResponse = (HttpWebResponse)request.GetResponse();

                //var coc = new CookieCollection();
                //for (int i = 0; i < httpResponse.Headers.Count; i++)
                //{
                //    string name = httpResponse.Headers.GetKey(i);
                //    if (name != "Set-Cookie")
                //        continue;
                //    string value = httpResponse.Headers.Get(i);
                //    foreach (var singleCookie in value.Split(','))
                //    {
                //        Match match = Regex.Match(singleCookie, "(.+?)=(.+?);");
                //        if (match.Captures.Count == 0)
                //            continue;
                //        coc.Add(
                //            new Cookie(
                //                match.Groups[1].ToString(),
                //                match.Groups[2].ToString(),
                //                "/",
                //                request.Host.Split(':')[0]));

                //        Message.Show("Name: " + match.Groups[1].ToString() +
                //                "\nValue: " + match.Groups[2].ToString() +
                //                "\nPath: " + "/IOPT-Server-Tester-1-2/service/"+
                //                "\nHost: " + request.Host.Split(':')[0],"");
                //    }
                //}
                _cookies = new CookieContainer();
                _cookies.Add(httpResponse.Cookies);
                //Message.Show(coc.Count.ToString(), "");
                //Message.Show(new StreamReader(request.GetRequestStream()).ReadToEnd(), "address");
                //string s = "";
                //foreach (var a in httpResponse.Headers)
                //{
                //    s += a.ToString()+"\n";
                //}
                //Message.Show(url + "\n\n" + s, "address");
                return true;
            }
            catch { return false; }
        }

        /*
         * TODO Переделать
         */
        public static void SendDataToServer()
        {
            try
            {
                var url = "http://" + Settings.Current.Server + "/snapshot?user=" + Settings.Current.Login;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST"; // "PUT";
                request.CookieContainer = _cookies;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    // streamWriter.Write(JsonConvert.SerializeObject(Snapshot.current));
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();

                if (httpResponse.StatusCode != HttpStatusCode.Accepted || httpResponse.StatusCode != HttpStatusCode.OK) throw new Exception();
            }
            catch
            {
                // ignored
            }
        }

        /*
         * TODO Переделать
         */
        public static async void GetDataFromServer()
        {
            try
            {
                string url = "http://" + Settings.Current.Server + "/snapshot?user=" + Settings.Current.Login;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = _cookies;
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream resStream = response.GetResponseStream();
                string resp = null;
                if (resStream != null)
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        resp = await reader.ReadToEndAsync();
                        resp = WebUtility.HtmlDecode(resp);
                    }
                //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(url, ""); }));
                if (string.IsNullOrWhiteSpace(resp)) throw new Exception("Can't get data from server");
                //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate { Message.Show(resp, response.ResponseUri.AbsolutePath); }));
                //Snapshot.current = JsonConvert.DeserializeObject<Snapshot>(resp);
            }
            catch
            {
                // ignored
            }
        }

        public static async void AutoUpdate()
        {
            await Task.Run(() =>
            {
                // TODO
                while (false)//Settings.Current.AutoUpdate)
                {
                    /*
                     * TODO Уменьшить нагрузку на сеть
                     */
                    Thread.Sleep((int)Settings.Current.AutoUpdateInterval * 1000);
                    var props = Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).ToList();
                    if (!props.Any()) continue;
                    foreach (var p in props)
                    {
                        try
                        {
                            var newp = IoTFactory.GetProperty(p);
                            if (newp != null)
                            {
                                //p.Value = newp;
                                //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(p.value, ""); }));
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate { Main.GetMainWindow().stackscroll.Content = View.GetDashboard(Main.GetMainWindow().Lobjects.SelectedItem as Object); }));
                }
            });
        }

        public class IoTFactory
        {
            #region Get
            public static Model GetModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.PathUnit)) return null;
                return (Model)Get("/platform/models/" + newModel.PathUnit);
            }

            public static Object GetObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return null;
                var modPath = (from m in Platform.Current.Models where m.Id == newObject.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                    return (Object)Get("/platform/models/" + modPath + "/objects/" + newObject.PathUnit);
            }

            public static Property GetProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return null;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Property)Get("/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + newProperty.PathUnit);
            }

            public static Script GetScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return null;
                var prop = (from p in Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return null;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Script)Get( "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + prop.PathUnit + "/scripts/" + newScript.PathUnit);
            }

            // TODO s
            //public static Dashboard GetDashboard(Dashboard newDashboard)
            //{
            //    try
            //    {
            //        string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + newDashboard.PathUnit;
            //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //        request.ContentType = "application/json";
            //        request.Method = "PUT";
            //        request.CookieContainer = _cookies;
            //        request.Timeout = 10000;
            //        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            //        {
            //            streamWriter.Write(JsonConvert.SerializeObject(newDashboard));
            //        }
            //        var httpResponse = (HttpWebResponse)request.GetResponse();
            //        return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
            //    }
            //    catch { return false; }
            //}
            //public static Dashboard.PropertyMap GetPropertyMap(Dashboard.PropertyMap newPropertyMap)
            //{
            //    try
            //    {
            //        var parent =
            //                    (from d in Client.Current.Dashboards where newPropertyMap.DashboardId == d.Id select d)
            //                    .FirstOrDefault();
            //        if (parent == null) return false;

            //        string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + parent.PathUnit + "/visio/" + newPropertyMap.PathUnit;
            //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            //        request.ContentType = "application/json";
            //        request.Method = "PUT";
            //        request.CookieContainer = _cookies;
            //        request.Timeout = 10000;
            //        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            //        {
            //            streamWriter.Write(JsonConvert.SerializeObject(newPropertyMap));
            //        }
            //        var httpResponse = (HttpWebResponse)request.GetResponse();
            //        return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
            //    }
            //    catch { return false; }
            //}

            private static IoT Get(string path)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + "/models/" + path + "?user=" + Settings.Current.Login;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "GET";
                    request.CookieContainer = _cookies;
                    request.Timeout = 1000;
                    var httpResponse = (HttpWebResponse)request.GetResponseAsync().Result;
                    if (httpResponse.StatusCode != HttpStatusCode.OK || httpResponse.StatusCode != HttpStatusCode.Found) return null;
                    string responseText;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseText = streamReader.ReadToEnd();
                        responseText = WebUtility.HtmlDecode(responseText);
                    }
                    if (string.IsNullOrWhiteSpace(responseText)) throw new Exception("Can't get data from server");
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate { Message.Show(responseText, ""); }));
                    return JsonConvert.DeserializeObject<IoT>(responseText);
                }
                catch { return null; }
            }
            #endregion

            // Done
            #region Create
            public static Model CreateModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.PathUnit)) return null;
                newModel.Id = null;
                return (Model)CreateIoT(newModel, "/platform/models");
            }

            public static Object CreateObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return null;
                newObject.Id = null;
                var modPath = (from m in Platform.Current.Models where m.Id == newObject.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Object)CreateIoT(newObject, "/platform/models/" + modPath + "objects");
            }
            public static Property CreateProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return null;
                newProperty.Id = null;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Property)CreateIoT(newProperty, "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties");
            }
            public static Script CreateScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return null;
                newScript.Id = null;
                var prop = (from p in Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return null;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Script)CreateIoT(newScript, "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + prop.PathUnit + "/scripts");
            }

            public static Dashboard CreateDashboard(Dashboard newDashboard)
            {
                newDashboard.Id = null;
                try
                {
                    string url = "http://" + Settings.Current.Server + "/clent/dashboards/";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newDashboard));
                    }
                    request.GetResponse();

                    url = url + newDashboard.PathUnit;

                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string resp;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        resp = reader.ReadToEnd();
                    }
                    resp = WebUtility.HtmlDecode(resp);
                    if (string.IsNullOrWhiteSpace(resp)) throw new Exception("Can't get data from server");
                    return JsonConvert.DeserializeObject<Dashboard>(resp);
                }
                catch { return null; }
            }
            public static Dashboard.PropertyMap CreatePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                newPropertyMap.Id = null;
                try
                {
                    var parent =
                        (from d in Client.Current.Dashboards where newPropertyMap.DashboardId == d.Id select d)
                        .FirstOrDefault();
                    if (parent == null) return null;

                    string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + parent.PathUnit + "/visio/";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newPropertyMap));
                    }
                    request.GetResponse();

                    url = url + newPropertyMap.PathUnit;

                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string resp;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        resp = reader.ReadToEnd();
                    }
                    resp = WebUtility.HtmlDecode(resp);
                    if (string.IsNullOrWhiteSpace(resp)) throw new Exception("Can't get data from server");
                    return JsonConvert.DeserializeObject<Dashboard.PropertyMap>(resp);
                }
                catch { return null; }
            }

            private static IoT CreateIoT(IoT obj, string path)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + path;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(obj));
                    }
                    request.GetResponse();

                    url = url + obj.PathUnit;

                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.CookieContainer = _cookies;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    string resp;
                    using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                    {
                        resp = reader.ReadToEnd();
                    }
                    resp = WebUtility.HtmlDecode(resp);
                    if (string.IsNullOrWhiteSpace(resp)) throw new Exception("Can't get data from server");
                    return JsonConvert.DeserializeObject<IoT>(resp);
                }
                catch { return null; }
            }

            #endregion

            // Done
            #region Update
            public static bool UpdateModel(Model newModel)
            {
                return !string.IsNullOrWhiteSpace(newModel.PathUnit) && Change(newModel, "/platform/models/" + newModel.PathUnit);
            }

            public static bool UpdateObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == newObject.ModelId select m.PathUnit).First();
                return modPath != null && Change(newObject, "/platform/models/" + modPath + "/objects/" + newObject.PathUnit);
            }
            public static bool UpdateProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return false;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Change(newProperty, "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + newProperty.PathUnit);
            }
            public static bool UpdateScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return false;
                var prop = (from p in Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return false;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Change(newScript, "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + prop.PathUnit + "/scripts/" + newScript.PathUnit);
            }

            public static bool UpdateDashboard(Dashboard newDashboard)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + newDashboard.PathUnit;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "PUT";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newDashboard));
                    }
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                }
                catch { return false; }
            }
            public static bool UpdatePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                try
                {
                    var parent =
                                (from d in Client.Current.Dashboards where newPropertyMap.DashboardId == d.Id select d)
                                .FirstOrDefault();
                    if (parent == null) return false;

                    string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + parent.PathUnit+ "/visio/" + newPropertyMap.PathUnit;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "PUT";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newPropertyMap));
                    }
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                }
                catch { return false; }
            }
            private static bool Change(IoT obj, string path)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "PUT";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(obj));
                    }
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show("{\"value\":" + ((Property)obj).value + "}", url); }));
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(url, url); }));
                    var httpResponse = (HttpWebResponse)request.GetResponseAsync().Result;
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                    //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //{
                    //    var responseText = await streamReader.ReadToEndAsync();
                    //    await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(httpResponse.StatusCode.ToString(), ""); }));
                    //}
                }
                catch { return false; }
            }
            #endregion

            // Done
            #region Modify

            public static bool ModifyProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return false;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                return modPath != null && Modify(newProperty, "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + newProperty.PathUnit);
            }

            public static bool ModifyScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return false;
                var prop = (from p in Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return false;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                return modPath != null && Modify(newScript, "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + prop.PathUnit + "/scripts/" + newScript.PathUnit);
            }

            private static bool Modify(IoT iot, string path)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";//"PATCH"
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        if (iot is Property)
                            streamWriter.Write("{\"value\":" + ((Property)iot).Value + "}");
                        if (iot is Script)
                            streamWriter.Write("{\"value\":" + ((Script)iot).Value + "}");
                    }
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show("{\"value\":" + ((Property)obj).value + "}", url); }));
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(url, url); }));
                    var httpResponse = (HttpWebResponse)request.GetResponseAsync().Result;
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                    //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //{
                    //    var responseText = await streamReader.ReadToEndAsync();
                    //    await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(httpResponse.StatusCode.ToString(), ""); }));
                    //}
                }
                catch { return false; }
            }

            #endregion

            //Done
            #region Delete
            public static bool DeleteModel(Model newModel)
            {
                return !string.IsNullOrWhiteSpace(newModel.PathUnit) && Delete("/platform/models/" + newModel.PathUnit);
            }

            public static bool DeleteObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == newObject.ModelId select m.PathUnit).First();
                return modPath != null && Delete( "/platform/models/" + modPath + "/objects/" + newObject.PathUnit);
            }
            public static bool DeleteProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return false;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Delete("/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + newProperty.PathUnit);
            }
            public static bool DeleteScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return false;
                var prop = (from p in Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return false;
                var obj = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Platform.Current.Models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Delete( "/platform/models/" + modPath + "/objects/" + obj.PathUnit + "/properties/" + prop.PathUnit + "/scripts/" + newScript.PathUnit);
            }

            public static bool DeleteDashboard(Dashboard newDashboard)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + newDashboard.PathUnit;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "DELETE";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newDashboard));
                    }
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                }
                catch { return false; }
            }
            public static bool DeletePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                try
                {
                    var parent =
                                (from d in Client.Current.Dashboards where newPropertyMap.DashboardId == d.Id select d)
                                .FirstOrDefault();
                    if (parent == null) return false;

                    string url = "http://" + Settings.Current.Server + "/clent/dashboards/" + parent.PathUnit + "/visio/" + newPropertyMap.PathUnit;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "DELETE";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newPropertyMap));
                    }
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                }
                catch { return false; }
            }

            private static bool Delete(string path)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "DELETE";
                    request.CookieContainer = _cookies;
                    request.Timeout = 10000;
                    var httpResponse = (HttpWebResponse)request.GetResponseAsync().Result;
                    return httpResponse.StatusCode == HttpStatusCode.OK || httpResponse.StatusCode == HttpStatusCode.Accepted;
                    //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //{
                    //    var responseText = await streamReader.ReadToEndAsync();
                    //    await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(httpResponse.StatusCode.ToString(), ""); }));
                    //}
                }
                catch { return false; }
            }
            #endregion
        }

    }
}
