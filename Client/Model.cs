using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{

    public class Snapshot
    {
        static Snapshot instance;

        public static Snapshot Current { get { return instance ?? (instance = new Snapshot()); } set { instance = value; } }

        Snapshot() { }
        public ObservableCollection<Model> Models { get; } = new ObservableCollection<Model>();

        public ObservableCollection<Dashboard> Dashboards { get; } = new ObservableCollection<Dashboard>();

        public DateTimeOffset LastUpdate { get; set; }
    }

    public class Model
    {
        string id;
        string name;

        public string Name { get { return name; } set { name = value; GenerateId(value); } }

        public ObservableCollection<Object> Objects { get; } = new ObservableCollection<Object>();

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public Model(string name)
        {
            Name = name;
            Snapshot.Current.Models.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }
        public Model()
        {

        }

        public Model(Model m)
        {
            Name = m.Name;
            foreach (var i in m.Objects)
            {
                Objects.Add(new Object(i));
            }
            Snapshot.Current.Models.Add(this);
        }

        private void GenerateId(string val)
        {
            string tmpid = Transliteration.Front(val);
            if (CheckName(tmpid)) { Id = tmpid; return; }
            int i = 1;
            while (!CheckName(tmpid + i.ToString())) i++;
            Id = tmpid + i.ToString();
        }

        private static bool CheckName(string val)
        {
            if (Snapshot.Current.Models.Count == 0) return true;
            bool res = true;
            foreach (var m in Snapshot.Current.Models) if (m.Id.Equals(val)) res = false;
            return res;
        }
    }

    public class Object
    {
        string id;
        string _name;
        public string Name { get { return _name; } set { _name = value; GenerateId(value); } }
        public ObservableCollection<Property> Properties { get; set; } = new ObservableCollection<Property>();

        public string Id { get { return id; } set { id = value; } }

        public Object()
        {

        }

        public Object(string name)
        {
            Name = name;
        }

        public Object(Object o)
        {
            Name = o.Name;
            foreach (var i in o.Properties)
                Properties.Add(new Property(i));
        }

        public override string ToString()
        {
            return Name;
        }

        private void GenerateId(string val)
        {
            string tmpid = Transliteration.Front(val);
            if (CheckName(tmpid)) { Id = tmpid; return; }
            int i = 1;
            while (!CheckName(tmpid + i.ToString())) i++;
            Id = tmpid + i.ToString();
        }

        private static bool CheckName(string val)
        {
            if (Snapshot.Current.Models.Count == 0) return true;
            bool res = true;
            foreach (var m in Snapshot.Current.Models)
                foreach (var o in m.Objects)
                    if (o.Id.Equals(val)) res = false;
            return res;
        }
    }

    public class Property
    {
        string id;
        int type;
        string name;
        string value;

        public ObservableCollection<Script> Scripts { get; } = new ObservableCollection<Script>();
        public string Id { get { return id; } set { id = value; } }
        public string Name { get { return name; } set { name = value; GenerateId(value); } }
        public string Value { get { return value; } set {
                if (type >= 7 && type <= 12) { try { value = ((int)double.Parse(value.Replace('.', ','))).ToString(); } catch { } }
                if (type >= 13 && type <= 15) value = value.Replace('.', ',');
                if (CheckType(value))
                    this.value = value;
            } }
        public int Type { get { return type; } set { type = value; } }

        [JsonConstructor]
        public Property()
        {      
        }

        public Property(string name, TypeCode type)
        {
            Name = name;
            Type = (int)type;
            switch (type)
            {
                case TypeCode.Boolean: Value = bool.FalseString; break;
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Int64:
                case TypeCode.UInt32:
                case TypeCode.UInt16:
                case TypeCode.UInt64:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                    Value = "0";
                    break;
                case TypeCode.String: Value = ""; break;
            }
        }

        public Property(Property b)
        {
            Name = b.Name;
            Type = b.Type;
            Value = b.Value;
            foreach (var i in b.Scripts)
                Scripts.Add(new Script(i));
        }

        public void AddListener(string name, string script)
        {
            Scripts.Add(new Script(name, script));
        }

        public void DeleteListener(string name)
        {
            try
            {
                Scripts.Remove((from t in Scripts where t.Name == name select t).First());
            }
            catch { }
        }

        public override string ToString()
        {
            return Name;
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

        private void GenerateId(string val)
        {
            string tmpid = Transliteration.Front(val);
            if (CheckName(tmpid)) { Id = tmpid; return; }
            int i = 1;
            while (!CheckName(tmpid + i.ToString())) i++;
            Id = tmpid + i.ToString();
        }

        private static bool CheckName(string val)
        {
            if (Snapshot.Current.Models.Count == 0) return true;
            bool res = true;
            foreach (var m in Snapshot.Current.Models)
                foreach (var o in m.Objects)
                    foreach (var p in o.Properties)
                        if (p.Id.Equals(val)) res = false;
            return res;
        }
    }

    public class Script
    {
        string id;
        string name;
        string script;

        public string Id { get { return id; } set { id = value; } }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                GenerateId(value);
            }
        }
        public string Value { get { return script; } set { script = value; } }
        public Script(string name, string script)
        {
            Name = name;
            Value = script;
        }
        public Script() { }
        public Script(Script s)
        {
            Name = s.Name;
            Value = s.Value;
        }

        public override string ToString()
        {
            return Name;
        }

        private void GenerateId(string val)
        {
            string tmpid = Transliteration.Front(val);
            if (CheckName(tmpid)) { Id = tmpid; return; }
            int i = 1;
            while (!CheckName(tmpid + i.ToString())) i++;
            Id = tmpid + i.ToString();
        }
        private static bool CheckName(string val)
        {
            if (Snapshot.Current.Models.Count == 0) return true;
            bool res = true;
            foreach (var m in Snapshot.Current.Models)
                foreach (var o in m.Objects)
                    foreach (var p in o.Properties)
                        foreach (var s in p.Scripts)
                            if (s.Id.Equals(val)) res = false;
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

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public Object Parent
        {
            get
            {
                return parent;
            }

            set
            {
                parent = value;
            }
        }

        public List<PropertyLink> View { get; } = new List<PropertyLink>();

        int id;
        Object parent;
        public Dashboard() { }
        public Dashboard(Object parent)
        {
            id = Snapshot.Current.Dashboards.Count == 0 ? 0 : Snapshot.Current.Dashboards.MaxBy(x => x.Id).Id + 1;
            Parent = parent;
        }

        public class PropertyLink
        {
            public Property Property { get; set; }

            public bool IsControl { get; set; }

            public double? Min { get; set; }
            public double? Max { get; set; }
        }
    }


    static partial class MoreEnumerable
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
    }
}
