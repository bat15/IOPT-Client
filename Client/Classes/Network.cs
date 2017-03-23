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

        public static void SendDataToServer()
        {
            try
            {
                var url = "http://" + Settings.Current.Server + "/snapshot?user=" + Settings.Current.Login;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method ="POST"; // "PUT";
                request.CookieContainer = _cookies;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(Snapshot.current));
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();

                if(httpResponse.StatusCode!=HttpStatusCode.Accepted|| httpResponse.StatusCode != HttpStatusCode.OK)throw new Exception();
            }
            catch
            {
                // ignored
            }
        }

        public static async void GetDataFromServer()
        {
            try
            {
                string url = "http://" + Settings.Current.Server + "/snapshot?user="+Settings.Current.Login;

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
                Snapshot.current = JsonConvert.DeserializeObject<Snapshot>(resp);
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
                while (Settings.Current.AutoUpdate)
                {
                    Thread.Sleep((int)Settings.Current.AutoUpdateInterval * 1000);
                    var props = Snapshot.current.models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).ToList();
                    if (!props.Any()) continue;
                    foreach (var p in props)
                    {
                        try
                        {
                            var newp = IoTFactory.GetProperty(p);
                            if (newp != null)
                            {
                                p.Value=newp;
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
            public static string GetProperty(Property prop)
            {
                if (string.IsNullOrWhiteSpace(prop.PathUnit)) return null;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return Get(modPath + "/Objects/" + obj.PathUnit + "/Properties/" + prop.PathUnit);
            }


            private static string Get(string path)
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
                    var httpResponse =(HttpWebResponse)request.GetResponseAsync().Result;
                    if (httpResponse.StatusCode != HttpStatusCode.OK|| httpResponse.StatusCode != HttpStatusCode.Found) return null;
                    string responseText;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseText = streamReader.ReadToEnd();
                        responseText = WebUtility.HtmlDecode(responseText);
                    }
                    responseText=responseText.Substring(responseText.IndexOf("value", StringComparison.Ordinal) + 6);
                    responseText = responseText.Substring(0, responseText.IndexOf('\"'));
                    Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate { Message.Show(responseText,""); }));
                    return responseText;
                }
                catch { return null; }
            }
            #endregion

            #region Create
            public static Model CreateModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.PathUnit)) return null;
                newModel.Id = null;
                return (Model)CreateIoT(newModel, "platform/Models");
            }

            public static Object CreateObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return null;
                newObject.Id = null;
                var modPath = (from m in Snapshot.current.models where m.Id == newObject.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Object)CreateIoT(newObject, "platform/Models/"+modPath+"Objects");
            }
            public static Property CreateProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return null;
                newProperty.Id = null;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Property)CreateIoT(newProperty, "platform/Models/"+modPath+"/Objects/"+ obj.PathUnit+"/Properties");
            }
            public static Script CreateScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return null;
                newScript.Id = null;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return null;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return null;
                return (Script)CreateIoT(newScript, "platform/Models/" + modPath + "/Objects/" + obj.PathUnit + "/Properties/" + prop.PathUnit+"/Scripts");
            }

            public static Dashboard CreateDashboard(Dashboard newDashboard)
            {
                newDashboard.Id = null;
                try
                {
                    string url = "http://" + Settings.Current.Server + "clent/Dashboards/";
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.CookieContainer = _cookies;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(newDashboard));
                    }
                    request.GetResponse();

                    url = url + newDashboard.PathUnit;

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
                    return JsonConvert.DeserializeObject<Dashboard>(resp);
                }
                catch { return null; }
            }
            public static Dashboard.PropertyMap CreatePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                newPropertyMap.Id = null;
                return null;
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

            #region Update
            public static bool UpdateModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.PathUnit)) return false;
                return Change(newModel, newModel.PathUnit);
            }

            public static bool UpdateObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == newObject.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Change(newObject, modPath + "/" + newObject.PathUnit);
            }
            public static bool UpdateProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Change(newProperty, modPath + "/" + obj.PathUnit + "/" + newProperty.PathUnit);
            }
            public static bool UpdateScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return false;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Change(newScript, modPath + "/" + obj.PathUnit + "/" + prop.PathUnit + "/" + newScript.PathUnit);
            }
            public static bool UpdateDashboard(Dashboard newDashboard)
            {
                return false;
            }
            public static bool UpdatePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                return false;
            }
            private static bool Change(IoT obj, string path)
            {
                try
                {
                    //var tmp = new TmpProperty {name = obj.name,type = ((Property)obj).type,value = ((Property)obj).value };
                    string url = "http://" + Settings.Current.Server + "/models/" + path+"?user="+Settings.Current.Login;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";//"PATCH"
                    request.CookieContainer = _cookies;
                    //request.Timeout = 3000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        //streamWriter.Write("{\"value\":" + ((Property)obj).value + "}");
                        streamWriter.Write("{\"value\":" + ((Property)obj).Value + "}");
                    }
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show("{\"value\":" + ((Property)obj).value + "}", url); }));
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(url, url); }));
                    var httpResponse = (HttpWebResponse)request.GetResponseAsync().Result;
                    if (httpResponse.StatusCode == HttpStatusCode.OK) return true;
                    else return false;
                    //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //{
                    //    var responseText = await streamReader.ReadToEndAsync();
                    //    await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(httpResponse.StatusCode.ToString(), ""); }));
                    //}
                }
                catch { return false; }
            }
            #endregion

            #region Modify

            public static bool ModifyProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Modify(newProperty, modPath + "/" + obj.PathUnit + "/" + newProperty.PathUnit);
            }

            public static bool ModifyScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return false;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Modify(newScript, modPath + "/" + obj.PathUnit + "/" + prop.PathUnit + "/" + newScript.PathUnit);
            }

            private static bool Modify(IoT obj, string path)
            {
                try
                {
                    //var tmp = new TmpProperty { name = obj.name, type = ((Property)obj).type, value = ((Property)obj).value };
                    string url = "http://" + Settings.Current.Server + "/models/" + path + "?user=" + Settings.Current.Login;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";//"PATCH"
                    request.CookieContainer = _cookies;
                    //request.Timeout = 3000;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        //streamWriter.Write("{\"value\":" + ((Property)obj).value + "}");
                        streamWriter.Write("{\"value\":" + ((Property)obj).Value + "}");
                    }
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show("{\"value\":" + ((Property)obj).value + "}", url); }));
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(url, url); }));
                    var httpResponse = (HttpWebResponse)request.GetResponseAsync().Result;
                    if (httpResponse.StatusCode == HttpStatusCode.OK) return true;
                    else return false;
                    //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    //{
                    //    var responseText = await streamReader.ReadToEndAsync();
                    //    await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(httpResponse.StatusCode.ToString(), ""); }));
                    //}
                }
                catch { return false; }
            }

            #endregion

            #region Delete
            public static bool DeleteModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.PathUnit)) return false;
                return Delete(newModel.PathUnit);
            }

            public static bool DeleteObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.PathUnit)) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == newObject.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Delete(modPath + "/" + newObject.PathUnit);
            }
            public static bool DeleteProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.PathUnit)) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == newProperty.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Delete(modPath + "/" + obj.PathUnit + "/" + newProperty.PathUnit);
            }
            public static bool DeleteScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.PathUnit)) return false;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where p.Id == newScript.Id select p).First();
                if (prop == null) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.Objects) where o.Id == prop.ObjectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.Id == obj.ModelId select m.PathUnit).First();
                if (modPath == null) return false;
                return Delete(modPath + "/" + obj.PathUnit + "/" + prop.PathUnit + "/" + newScript.PathUnit);
            }
            public static bool DeleteDashboard(Dashboard newDashboard)
            {
                return false;
            }
            public static bool DeletePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                return false;
            }
            private static bool Delete(string path)
            {
                try
                {
                    string url = "http://" + Settings.Current.Server + "/snapshot/" + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "DELETE";
                    request.CookieContainer = _cookies;
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK) return true;
                    else return false;
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
