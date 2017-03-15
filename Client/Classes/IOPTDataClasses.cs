using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public abstract class IoT
    {
        protected long _id;
        protected string _pathUnit;
        protected string _name;
        //генерирует ид по имени
        public string GenerateId(string val)
        {
            string tmpid = Transliteration.Front(val);
            if (CheckName(tmpid)) return tmpid;
            int i = 1;
            while (!CheckName(tmpid + i.ToString())) i++;
            return tmpid + i.ToString();
        }

        public long id { get { return _id; } set { _id = value; } }
        public string pathUnit { get { return _pathUnit; } set { _pathUnit = value; } }
        public string name { get { return _name; } set { _name = value; pathUnit = GenerateId(value); } }
        public override string ToString() { return name; }
        //Проверяет есть ли ид в текущем снапшоте
        protected abstract bool CheckName(string name);
    }

    public class Snapshot
    {
        long _id;
        static Snapshot _instance;

        public long id { get { return _id; } set { _id = value; } }
        public static Snapshot current { get { return _instance ?? (_instance = new Snapshot()); } set { _instance = value; } }

        Snapshot() { }

        public ObservableCollection<Model> models { get; } = new ObservableCollection<Model>();

        public ObservableCollection<Dashboard> dashboards { get; } = new ObservableCollection<Dashboard>();

        public DateTimeOffset lastUpdate { get; set; }
    }

    public class Model : IoT
    {
        ObservableCollection<Object> _objects;

        public virtual ObservableCollection<Object> objects
        {
            get { return _objects ?? (_objects = new ObservableCollection<Object>()); }
            set { _objects = value; }
        }

        [JsonConstructor]
        public Model(long id, string pu, string name, IEnumerable<Object> objects)
        {
            base.id = id;
            base._pathUnit = pu;
            base._name = name;
            if (objects != null)
                foreach (var o in objects)
                    this.objects.Add(o);
        }
        public Model() { }
        public Model(Model m)
        {
            name = m.name;
            foreach (var i in m.objects)
            {
                objects.Add(new Object(i));
            }
        }
        public Model(string name)
        {
            base.name = name;
            //id = null;
        }

        protected override bool CheckName(string name)
        {
            if (Snapshot.current.models.Count == 0) return true;
            bool res = true;
            foreach (var m in Snapshot.current.models) if (m.pathUnit.Equals(name)) res = false;
            return res;
        }
    }

    public sealed class Object : IoT
    {
        long _modelId;
        ObservableCollection<Property> _properties;
        public long modelId { get { return _modelId; } set { _modelId = value; } }
        public ObservableCollection<Property> properties
        {
            get { return _properties ?? (_properties = new ObservableCollection<Property>()); }
            set { _properties = value; }
        }
        public Object() { }
        public Object(Object o)
        {
            name = o.name;
            //modelId = null;
            foreach (var i in o.properties)
                properties.Add(new Property(i));
        }


        [JsonConstructor]
        public Object(long id, string pu, string name, long modelid, IEnumerable<Property> properties)
        {
            base.id = id;
            base._pathUnit = pu;
            base._name = name;
            modelId = modelid;
            if (properties != null)
                foreach (var o in properties)
                    this.properties.Add(o);
        }

        public Object(string name, long modelid)
        {
            base.name = name;
            modelId = modelid;
        }

        protected override bool CheckName(string val)
        {
            if (Snapshot.current.models.Count == 0) return true;
            bool res = true;
            var objects= (from m in Snapshot.current.models where m.id == modelId select m).FirstOrDefault()?.objects;
            if(objects!=null)
            foreach (var o in objects)
                if (o.pathUnit.Equals(val)) res = false;
            return res;
        }
    }

    public sealed class Property : IoT
    {
        int _type;
        long _objectId;
        ObservableCollection<Script> _scripts;
        string _value;
        public string value
        {
            get { return _value; }
            set
            {
                if (_type == 3) value = value.ToLower();
                if (_type >= 7 && _type <= 12) { try { value = ((int)double.Parse(value.Replace('.', ','))).ToString(); } catch { } }
                if (_type >= 13 && _type <= 15) value = value.Replace('.', ',');
                if (CheckType(value))
                    this._value = value;
            }
        }
        public int type { get { return _type; } set { _type = value; } }
        public long objectId { get { return _objectId; } set { _objectId = value; } }

        public ObservableCollection<Script> scripts
        {
            get { return _scripts ?? (_scripts = new ObservableCollection<Script>()); }
            set { _scripts = value; }
        }
        public Property() { }
        [JsonConstructor]
        public Property(long id, string pu, string name, int type, long objectid, IEnumerable<Script> scripts, string value)
        {
            base.id = id;
            base._pathUnit = pu;
            base._name = name;
            this.type = type;
            objectId = objectid;
            if (scripts != null)
                foreach (var o in scripts)
                    this.scripts.Add(o);
            this.value = value;
        }

        public Property(Property b)
        {
            name = b.name;
            type = b.type;
            value = b.value;
            //objectId = null;
            foreach (var i in b.scripts)
                scripts.Add(new Script(i));
        }

        public Property(string name, long objectid, int type, string value)
        {
            base.name = name;
            objectId = objectid;
            this.type = type;
            this.value = value;
        }

        private bool CheckType(string value)
        {
            try
            {
                Convert.ChangeType(value, (TypeCode)type);
                return true;
            }
            catch {/* Message.Show(value,((TypeCode)type).ToString());*/ return false; }
        }

        protected override bool CheckName(string val)
        {
            if (Snapshot.current.models.Count == 0) return true;
            bool res = true;
            var props = (from o in Snapshot.current.models.SelectMany(x => x.objects) where o.id == objectId select o).FirstOrDefault()?.properties;
            if(props!=null)
            foreach (var p in props)
                if (p.pathUnit.Equals(val)) res = false;
            return res;
        }
    }

    public class Script : IoT
    {
        string _value;
        long _propertyId;
        public long propertyId { get { return _propertyId; } set { _propertyId = value; } }
        public string value { get { return _value; } set { _value = value; } }
        public Script() { }
        [JsonConstructor]
        public Script(long id, string pu, string name, string script, long propertyId)
        {
            base.id = id;
            base._pathUnit = pu;
            base._name = name;
            value = script;
            this.propertyId = propertyId;
        }

        public Script(Script s)
        {
            name = s.name;
            value = s.value;
            //propertyId = null;
        }

        public Script(string name, long propertyid, string value)
        {
            base.name = name;
            _propertyId = propertyid;
            this.value = value;
        }

        protected override bool CheckName(string val)
        {
            if (Snapshot.current.models.Count == 0) return true;
            bool res = true;
            var scripts = (from o in Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties) where o.id == _propertyId select o).FirstOrDefault()?.scripts;
            if(scripts!=null)
            foreach (var s in scripts)
                if (s.pathUnit.Equals(val)) res = false;
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
        long _id;
        long _objectId;
        public long id { get { return _id; } set { _id = value; } }

        public long objectId { get { return _objectId; } set { _objectId = value; } }

        public List<PropertyMap> view { get; } = new List<PropertyMap>();

        [JsonConstructor]
        public Dashboard(long id, long parentid, IEnumerable<PropertyMap> pm)
        {
            this.id = id;
            objectId = parentid;
            if (pm != null) foreach (var p in pm) view.Add(p);
        }
        public Dashboard(Object parent)
        {
            _id = Snapshot.current.dashboards.Count == 0 ? 0 : Snapshot.current.dashboards.MaxBy(x => x.id).id + 1;
            objectId = parent.id;
        }

        public class PropertyMap
        {
            public long id { get; set; }
            public Property property { get; set; }
            public long dashboardId { get; set; }

            public bool isControl { get; set; }

            public double? min { get; set; }
            public double? max { get; set; }

            [JsonConstructor]
            public PropertyMap(long id, Property parent, long dashboardId, bool isControl, double? min, double? max)
            {
                this.id = id;
                property = parent;
                this.dashboardId = dashboardId;
                this.isControl = isControl;
                this.min = min;
                this.max = max;
            }
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
