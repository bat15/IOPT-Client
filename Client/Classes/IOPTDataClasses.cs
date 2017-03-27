using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.RightsManagement;
using Newtonsoft.Json;

// ReSharper disable ReturnValueOfPureMethodIsNotUsed
// ReSharper disable InconsistentNaming

namespace Client.Classes
{
    public abstract class IoT
    {
        protected string name;
        //генерирует ид по имени
        public string GenerateId(string val)
        {
            var tmpid = Transliteration.Front(val);
            if (CheckName(tmpid)) return tmpid;
            var i = 1;
            while (!CheckName(tmpid + i)) i++;
            return tmpid + i;
        }

        [JsonProperty(PropertyName = "id")]
        public long? Id { get; set; }

        [JsonProperty(PropertyName = "pathUnit")]
        public string PathUnit { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get { return name; } set { name = value; PathUnit = GenerateId(value); } }

        public override string ToString() { return Name; }
        //Проверяет есть ли ид в текущем снапшоте
        protected abstract bool CheckName(string name);
    }

    public class Platform
    {
        private static Platform current;

        private Platform() { }

        public static Platform Current
        {
            get { return current ?? (current = new Platform()); }
            set { current = value; }
        }

        [JsonProperty(PropertyName = "models")]
        public ObservableCollection<Model> Models { get; set; } = new ObservableCollection<Model>();
    }

    public class Client
    {
        private static Client current;

        private Client() { }

        public static Client Current
        {
            get { return current ?? (current = new Client()); }
            set { current = value; }
        }

        [JsonProperty(PropertyName = "dashboards")]
        public ObservableCollection<Dashboard> Dashboards { get; set; } = new ObservableCollection<Dashboard>();
    }


    public class Model : IoT
    {
        ObservableCollection<Object> objects;

        [JsonProperty(PropertyName = "objects")]
        public ObservableCollection<Object> Objects
        {
            get { return objects ?? (objects = new ObservableCollection<Object>()); }
            set { objects = value; }
        }

        [JsonConstructor]
        public Model(long id, string pu, string name, IEnumerable<Object> objects)
        {
            Id = id;
            PathUnit = pu;
            this.name = name;
            if (objects != null)
                foreach (var o in objects)
                    Objects.Add(o);
        }
        public Model() { }
        public Model(Model m)
        {
            Name = m.Name;
            foreach (var i in m.Objects)
            {
                Objects.Add(new Object(i));
            }
        }
        public Model(string name)
        {
            Name = name;
            //id = null;
        }

        protected override bool CheckName(string cname)
        {
            if (Platform.Current.Models.Count == 0) return true;
            bool res = true;
            foreach (var m in Platform.Current.Models) if (m.PathUnit.Equals(cname)) res = false;
            return res;
        }
    }

    public sealed class Object : IoT
    {
        private ObservableCollection<Property> _properties;

        [JsonProperty(PropertyName = "modelId")]
        public long? ModelId { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public ObservableCollection<Property> Properties
        {
            get { return _properties ?? (_properties = new ObservableCollection<Property>()); }
            set { _properties = value; }
        }

        public Object() { }
        public Object(Object o)
        {
            Name = o.Name;
            //modelId = null;
            foreach (var i in o.Properties)
                Properties.Add(new Property(i));
        }


        [JsonConstructor]
        public Object(long id, string pu, string name, long modelid, IEnumerable<Property> properties)
        {
            Id = id;
            PathUnit = pu;
            this.name = name;
            ModelId = modelid;
            if (properties != null)
                foreach (var o in properties)
                    Properties.Add(o);
        }

        public Object(string name, long modelid)
        {
            Name = name;
            ModelId = modelid;
        }

        protected override bool CheckName(string val)
        {
            if (Platform.Current.Models.Count == 0) return true;
            bool res = true;
            var objects = (from m in Platform.Current.Models where m.Id == ModelId select m).FirstOrDefault()?.Objects;
            if (objects != null)
                foreach (var o in objects)
                    if (o.PathUnit.Equals(val)) res = false;
            return res;
        }
    }

    public sealed class Property : IoT
    {
        private ObservableCollection<Script> _scripts;
        private string _value;

        [JsonProperty(PropertyName = "value")]
        public string Value
        {
            get { return _value; }
            set
            {
                if (Type == 3)
                {
                    value = value.ToLower();
                    try
                    {
                        //Changes.Add(new Series(DateTime.Now, bool.Parse(value) ? 0.0 : 1.0));
                        Changes.Add(new Series(DateTime.Now, bool.Parse(value) ? 1.0 : 0.0));
                    }
                    catch { }
                }
                if (Type >= 7 && Type <= 12)
                {
                    try
                    {
                        value = ((int)double.Parse(value.Replace('.', ','))).ToString();
                        Changes.Add(new Series(DateTime.Now, double.Parse(value)));
                    }
                    catch
                    {
                        // ignored
                    }
                }
                if (Type >= 13 && Type <= 15)
                {
                    value = value.Replace('.', ',');
                    try
                    {
                        Changes.Add(new Series(DateTime.Now, double.Parse(value)));
                    }
                    catch { }
                }
                if (Type == 18)
                {
                    try
                    {
                        Changes.Clear();
                        Changes.Add(new Series(DateTime.Now, double.Parse(value)));
                    }
                    catch { }
                }
                if (CheckType(value))
                    _value = value;
            }
        }

        public class Series
        {
            public DateTime Name { get; set; }
            public double Value { get; set; }

            public Series(DateTime dt, double val)
            {
                Name = dt;
                Value = val;
            }
        }

        [JsonIgnore]
        public ObservableCollection<Series> Changes { get; } = new ObservableCollection<Series>();

        [JsonProperty(PropertyName = "type")]
        public int Type { get; set; }

        [JsonProperty(PropertyName = "objectId")]
        public long? ObjectId { get; set; }

        [JsonProperty(PropertyName = "scripts")]
        public ObservableCollection<Script> Scripts
        {
            get { return _scripts ?? (_scripts = new ObservableCollection<Script>()); }
            set { _scripts = value; }
        }

        public Property() { }
        [JsonConstructor]
        public Property(long id, string pu, string name, int type, long objectid, IEnumerable<Script> scripts, string value)
        {
            Id = id;
            PathUnit = pu;
            this.name = name;
            Type = type;
            ObjectId = objectid;
            if (scripts != null)
                foreach (var o in scripts)
                    Scripts.Add(o);
            Value = value;
        }

        public Property(Property b)
        {
            Name = b.Name;
            Type = b.Type;
            Value = b.Value;
            //objectId = null;
            foreach (var i in b.Scripts)
                Scripts.Add(new Script(i));
        }

        public Property(string name, long objectid, int type, string value)
        {
            Name = name;
            ObjectId = objectid;
            Type = type;
            Value = value;
        }

        private bool CheckType(string value)
        {
            try
            {
                Convert.ChangeType(value, (TypeCode)Type);
                return true;
            }
            catch {/* Message.Show(value,((TypeCode)type).ToString());*/ return false; }
        }

        protected override bool CheckName(string val)
        {
            if (Platform.Current.Models.Count == 0) return true;
            bool res = true;
            var props = (from o in Platform.Current.Models.SelectMany(x => x.Objects) where o.Id == ObjectId select o).FirstOrDefault()?.Properties;
            if (props != null)
                foreach (var p in props)
                    if (p.PathUnit.Equals(val)) res = false;
            return res;
        }
    }

    public class Script : IoT
    {
        [JsonProperty(PropertyName = "propertyId")]
        public long? PropertyId { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        public Script() { }
        [JsonConstructor]
        public Script(long id, string pu, string name, string script, long propertyId)
        {
            Id = id;
            PathUnit = pu;
            this.name = name;
            Value = script;
            PropertyId = propertyId;
        }

        public Script(Script s)
        {
            Name = s.Name;
            Value = s.Value;
            //propertyId = null;
        }

        public Script(string name, long propertyid, string value)
        {
            Name = name;
            PropertyId = propertyid;
            Value = value;
        }

        protected override bool CheckName(string val)
        {
            if (Platform.Current.Models.Count == 0) return true;
            var res = true;
            var scripts = (from o in Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties) where o.Id == PropertyId select o).FirstOrDefault()?.Scripts;
            if (scripts == null) return true;
            foreach (var s in scripts)
                if (s.PathUnit.Equals(val)) res = false;
            ////if (!R(o, val)) res = false;
            return res;
        }

        //private static bool R(IoTObject obj, string val)
        //{
        //    if (obj.Objects.Count == 0) return true;
        //    bool res = true;
        //    foreach (var o in obj.Objects)
        //    {
        //        foreach (var p in o.Properties)
        //            foreach (var s in p.Scripts)
        //                if (s.Id == val) return false;
        //       if (!R(o, val)) res = false;
        //    }
        //    return res;
        //}
    }


    public class Dashboard
    {
        [JsonProperty(PropertyName = "id")]
        public long? Id { get; set; }

        [JsonProperty(PropertyName = "pathUnit")]
        public string PathUnit { get; set; }

        [JsonProperty(PropertyName = "objectId")]
        public long? ObjectId { get; set; }

        [JsonProperty(PropertyName = "view")]
        public List<PropertyMap> View { get; } = new List<PropertyMap>();

        [JsonConstructor]
        public Dashboard(long id, long parentid, IEnumerable<PropertyMap> pm)
        {
            Id = id;
            ObjectId = parentid;
            if (pm == null) return;
            foreach (var p in pm) View.Add(p);
        }
        public Dashboard(Object parent)
        {
            Id = Client.Current.Dashboards.Count == 0 ? 0 : Client.Current.Dashboards.MaxBy(x => x.Id).Id + 1;
            ObjectId = parent.Id;
        }

        public class PropertyMap
        {
            [JsonProperty(PropertyName = "id")]
            public long? Id { get; set; }

            [JsonProperty(PropertyName = "pathUnit")]
            public string PathUnit { get; set; }

            [JsonProperty(PropertyName = "property")]
            public Property Property { get; set; }

            [JsonProperty(PropertyName = "dashboardId")]
            public long? DashboardId { get; set; }

            [JsonProperty(PropertyName = "isControl")]
            public bool IsControl { get; set; }

            [JsonProperty(PropertyName = "min")]
            public double? Min { get; set; }

            [JsonProperty(PropertyName = "max")]
            public double? Max { get; set; }

            [JsonConstructor]
            public PropertyMap(long id, Property parent, long dashboardId, bool isControl, double? min, double? max)
            {
                Id = id;
                Property = parent;
                DashboardId = dashboardId;
                IsControl = isControl;
                Min = min;
                Max = max;
            }
        }

    }

    public static class MoreEnumerable
    {

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, null);
        }
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer = comparer ?? Comparer<TKey>.Default;

            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        public static double GetMedian(this IEnumerable<double> source)
        {
            // Create a copy of the input, and sort the copy
            var temp = source.ToArray();
            Array.Sort(temp);

            var count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            if (count % 2 != 0) return temp[count / 2];

            var a = temp[count / 2 - 1];
            var b = temp[count / 2];
            return (a + b) / 2;

        }
    }
}
