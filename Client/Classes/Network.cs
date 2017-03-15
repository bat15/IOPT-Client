using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    static class Network
    {
        private static CookieContainer cookies;

        public static bool Connect(string adress, string login, string password)
        {
            if (string.IsNullOrWhiteSpace(adress) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password)) return false;
            try
            {
                string url = "http://" + adress + "/login";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "POST";
                request.Timeout = 5000;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(new { login = login, password = password }));
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
                cookies = new CookieContainer();
                cookies.Add(httpResponse.Cookies);
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

        public static async void SendDataToServer()
        {
            try
            {
                string url = "http://" + Settings.Get.Server + "/snapshot";//"?user= " + Settings.Get.Login;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "PUT";//"POST"; 
                request.CookieContainer = cookies;
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(Snapshot.current));
                }
                var httpResponse = (HttpWebResponse)request.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(responseText, ""); }));
                }
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
                string url = "http://" + Settings.Get.Server + "/snapshot";//"?user="+Settings.Get.Login;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = cookies;
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
                while (true)
                {
                    Thread.Sleep((int)Settings.Get.AutoUpdateInterval * 1000);
                    var props = Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties);
                    if (props.Any())
                    {
                        foreach (var p in props)
                        {
                            try
                            {
                                var newp = IoTFactory.GetProperty(p);
                                if (newp != null)
                                {
                                    p.value=newp.value;
                                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(p.value, ""); }));
                                }
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Main.GetMainWindow().stackscroll.Content = View.GetDashboard(Main.GetMainWindow().Lobjects.SelectedItem as Object); }));
                        
                    }
                }
            });
        }

        //"http://"+snapshot

        public class IoTFactory
        {
            #region Get
            public static Property GetProperty(Property prop)
            {
                if (string.IsNullOrWhiteSpace(prop.pathUnit)) return null;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == prop.objectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return null;
                return (Property)Get(modPath + "/" + obj.pathUnit + "/" + prop.pathUnit);
            }

            private static IoT Get(string path)
            {
                try
                {
                    string url = "http://" + Settings.Get.Server + "/snapshot/" + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "GET";
                    request.CookieContainer = cookies;
                    var httpResponse = (HttpWebResponse)request.GetResponse();
                    if (httpResponse.StatusCode != HttpStatusCode.OK) return null;
                    string responseText;
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        responseText = streamReader.ReadToEnd();
                        responseText = WebUtility.HtmlDecode(responseText);
                    }
                    //Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(responseText,""); }));
                    return JsonConvert.DeserializeObject<Property>(responseText);
                }
                catch { return null; }
            }
            #endregion

            #region Create
            public static Model CreateModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.pathUnit)) return null;
                newModel.id = null;
                return (Model)Register(newModel, "");
            }

            public static Object CreateObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.pathUnit)) return null;
                newObject.id = null;
                var modPath = (from m in Snapshot.current.models where m.id == newObject.modelId select m.pathUnit).First();
                if (modPath == null) return null;
                return (Object)Register(newObject, modPath);
            }
            public static Property CreateProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.pathUnit)) return null;
                newProperty.id = null;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == newProperty.objectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return null;
                return (Property)Register(newProperty, modPath + "/" + obj.pathUnit);
            }
            public static Script CreateScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.pathUnit)) return null;
                newScript.id = null;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties) where p.id == newScript.id select p).First();
                if (prop == null) return null;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == prop.objectId select o).First();
                if (obj == null) return null;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return null;
                return (Script)Register(newScript, modPath + "/" + obj.pathUnit + "/" + prop.pathUnit);
            }
            public static Dashboard CreateDashboard(Dashboard newDashboard)
            {
                newDashboard.id = null;
                return null;
            }
            public static Dashboard.PropertyMap CreatePropertyMap(Dashboard.PropertyMap newPropertyMap)
            {
                newPropertyMap.id = null;
                return null;
            }

            private static IoT Register(IoT obj, string path)
            {
                try
                {
                    string url = "http://" + Settings.Get.Server + "/snapshot/" + path;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";
                    request.CookieContainer = cookies;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(obj));
                    }
                    request.GetResponse();

                    url = url + obj.pathUnit;

                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.CookieContainer = cookies;
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
                if (string.IsNullOrWhiteSpace(newModel.pathUnit)) return false;
                return Change(newModel, newModel.pathUnit);
            }

            public static bool UpdateObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.pathUnit)) return false;
                var modPath = (from m in Snapshot.current.models where m.id == newObject.modelId select m.pathUnit).First();
                if (modPath == null) return false;
                return Change(newObject, modPath + "/" + newObject.pathUnit);
            }
            public static bool UpdateProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.pathUnit)) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == newProperty.objectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return false;
                return Change(newProperty, modPath + "/" + obj.pathUnit + "/" + newProperty.pathUnit);
            }
            public static bool UpdateScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.pathUnit)) return false;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties) where p.id == newScript.id select p).First();
                if (prop == null) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == prop.objectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return false;
                return Change(newScript, modPath + "/" + obj.pathUnit + "/" + prop.pathUnit + "/" + newScript.pathUnit);
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
                    string url = "http://" + Settings.Get.Server + "/snapshot/" + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "POST";//"PATCH"
                    request.CookieContainer = cookies;
                    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(obj));
                    }
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

            #region Delete
            public static bool DeleteModel(Model newModel)
            {
                if (string.IsNullOrWhiteSpace(newModel.pathUnit)) return false;
                return Delete(newModel.pathUnit);
            }

            public static bool DeleteObject(Object newObject)
            {
                if (string.IsNullOrWhiteSpace(newObject.pathUnit)) return false;
                var modPath = (from m in Snapshot.current.models where m.id == newObject.modelId select m.pathUnit).First();
                if (modPath == null) return false;
                return Delete(modPath + "/" + newObject.pathUnit);
            }
            public static bool DeleteProperty(Property newProperty)
            {
                if (string.IsNullOrWhiteSpace(newProperty.pathUnit)) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == newProperty.objectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return false;
                return Delete(modPath + "/" + obj.pathUnit + "/" + newProperty.pathUnit);
            }
            public static bool DeleteScript(Script newScript)
            {
                if (string.IsNullOrWhiteSpace(newScript.pathUnit)) return false;
                var prop = (from p in Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties) where p.id == newScript.id select p).First();
                if (prop == null) return false;
                var obj = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == prop.objectId select o).First();
                if (obj == null) return false;
                var modPath = (from m in Snapshot.current.models where m.id == obj.modelId select m.pathUnit).First();
                if (modPath == null) return false;
                return Delete(modPath + "/" + obj.pathUnit + "/" + prop.pathUnit + "/" + newScript.pathUnit);
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
                    string url = "http://" + Settings.Get.Server + "/snapshot/" + path;
                    //await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate () { Message.Show(JsonConvert.SerializeObject(obj), url); }));
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.ContentType = "application/json";
                    request.Method = "DELETE";
                    request.CookieContainer = cookies;
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
