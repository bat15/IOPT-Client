using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Client
{
    class View
    {
        public static IEnumerable<ModelView> ModelToView()
        {
            string LastM = null;
            Func<string, string> ch1 = (a) => { if (a == LastM) return null; else { LastM = a; return a; } };
            string LastO = null;
            Func<string, string> ch2 = (a) => { if (a == LastO) return null; else { LastO = a; return a; } };
            return from m in Snapshot.current.models from o in m.objects from p in o.properties select new ModelView() { ModelName = ch1(m.name), ObjectName = ch2(o.name), PropertyName = p.name, Value = p.value, Type =  ((TypeCode)p.type).ToString(), Listeners = string.Join(", ", (from t in p.scripts select t.name).ToArray()) };
        }

        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && displayName != DisplayNameAttribute.Default)
                {
                    return GetDisplayNameFromRes(displayName.DisplayName);
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi != null)
                {
                    object[] attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                    for (int i = 0; i < attributes.Length; ++i)
                    {
                        var displayName = attributes[i] as DisplayNameAttribute;
                        if (displayName != null && displayName != DisplayNameAttribute.Default)
                        {
                            return GetDisplayNameFromRes(displayName.DisplayName);
                        }
                    }
                }
            }

            return null;
        }

        public static string GetDisplayNameFromRes(string id)
        {
            string res = null;
            try
            {
                res = (string)Application.Current.Resources[id];
            }
            catch { }
            return res;
        }

        //public static UIElement GetElemetsPanels(Model model)
        //{
        //    var Result = new StackPanel() { Orientation = Orientation.Vertical };
        //    if (model==null) return Result;
        //    foreach (var obj in model.objects)
        //    {
        //        var WP = new StackPanel() { Orientation = Orientation.Horizontal };
        //        var l = new Label() { Content = obj.name, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 16 };
        //        l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
        //        WP.Children.Add(l);
        //        Result.Children.Add(WP);
        //        foreach (var prop in obj.properties)
        //        {
        //            Result.Children.Add(GetPropertyView(prop, Dashboard.PropertyMap plink));
        //        }
        //    }
        //    return Result;
        //}


        public static UIElement GetDashboard(Object obj)
        {
            var Result = new StackPanel() { Orientation = Orientation.Vertical };

            if (obj == null) return Result;
            var tmp = (from t in Snapshot.current.dashboards where t.objectId == obj.id select t);
            if (tmp.Count() == 0) return Result;
            foreach (var dash in tmp)
            {
                DropShadowEffect shadow = new DropShadowEffect() { BlurRadius = 6, ShadowDepth = 6 };
               

                //добавить тень вместто бордера
                var dashboard = new StackPanel() { Orientation = Orientation.Horizontal, SnapsToDevicePixels = true };
                dashboard.SetResourceReference(Control.BackgroundProperty,"BackgroundColor");
                var border = new Border() { Child = dashboard, BorderThickness = new Thickness(1), SnapsToDevicePixels = true };
                var temp = new Grid() { Effect = shadow, Margin = new Thickness(6, 16, 6, 6),SnapsToDevicePixels=true };
                temp.Children.Add(border);
                border.SetResourceReference(Control.BorderBrushProperty, "MainColor");
                var e = new StackPanel() { Orientation = Orientation.Vertical,UseLayoutRounding=true };
                var n = new StackPanel() { Orientation = Orientation.Vertical, UseLayoutRounding = true };
                dashboard.Children.Add(e);
                dashboard.Children.Add(n);
                foreach (var t in dash.view)
                {
                    if (t.isControl)
                    {
                        e.Children.Add(GetManagePropertyView(t.property,t));
                    }
                    else
                    {
                        n.Children.Add(GetPropertyView(t.property,t));
                    }
                }
                Result.Children.Add(temp);
            }
            return Result;
        }

        private static UIElement GetPropertyView(Property prop, Dashboard.PropertyMap plink)
        {
            var child = new StackPanel() { Orientation = Orientation.Horizontal };
            var l = new Label() { Content = prop.name, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 200 };
            l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
            child.Children.Add(l);
            switch (prop.type)
            {
                case 3:
                    if (bool.Parse(prop.value))
                    {
                        var yes = new Path() { Fill = System.Windows.Media.Brushes.LightGreen };
                        yes.SetResourceReference(Path.DataProperty, "Yes");
                        var IYes = new Border() { Child = yes, Padding = new Thickness(0, 10, 0, 10) };
                        child.Children.Add(IYes);
                    }
                    else
                    {
                        var no = new Path() { Fill = System.Windows.Media.Brushes.Red };
                        no.SetResourceReference(Path.DataProperty, "No");
                        var INo = new Border() { Child = no, Padding = new Thickness(0, 10, 0, 10) };
                        child.Children.Add(INo);
                    }
                    break;
                case 9:
                case 7:
                case 11:
                case 10:
                case 8:
                case 12:
                case 14:
                case 15:
                case 13:
                    var pb = new ProgressBar() { Width = 150, Minimum = (long)plink.min, Maximum = (long)plink.max,IsIndeterminate=false };
                    pb.SetBinding(ProgressBar.ValueProperty, new Binding() { Source = prop, Path = new PropertyPath("value"), Mode = BindingMode.TwoWay });
                    pb.SetResourceReference(ProgressBar.ForegroundProperty,"MainColor");
                    l = new Label() { Content = double.Parse(prop.value), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 20, FontFamily = new System.Windows.Media.FontFamily("Consolas") };
                    l.SetResourceReference(Control.ForegroundProperty, "MainColor");
                    l.SetBinding(Label.ContentProperty, new Binding() { Source = prop, Path = new PropertyPath("value"), Mode = BindingMode.OneWay });
                    child.Children.Add(pb);
                    break;
                case 18:
                    l = new Label() { Content = "\""+prop.value+ "\"", HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12,FontFamily=new System.Windows.Media.FontFamily("Consolas") };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    l.SetBinding(Label.ContentProperty, new Binding() { Source = prop, Path = new PropertyPath("value"), Mode = BindingMode.OneWay });
                    child.Children.Add(l);
                    break;
            }
            return child;
        }

        private static UIElement GetManagePropertyView(Property prop, Dashboard.PropertyMap plink)
        {           
            var child = new StackPanel() { Orientation = Orientation.Horizontal };
            var l = new Label() { Content = prop.name, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 130 };
            l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
            child.Children.Add(l);
            switch ((TypeCode)prop.type)
            {
                case TypeCode.Boolean:
                    var tmp = new CheckBox() { Margin=new Thickness(0,5,0,5)};
                    tmp.SetBinding(CheckBox.IsCheckedProperty, new Binding() { Source = prop, Path = new PropertyPath("value"), Mode = BindingMode.TwoWay });
                    //tmp.Checked += (s,e) => { Network.IoTFactory.UpdateProperty(prop); };
                    //tmp.Unchecked += (s, e) => { Network.IoTFactory.UpdateProperty(prop); };
                    child.Children.Add(tmp);
                    break;
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Int64:
                case TypeCode.UInt32:
                case TypeCode.UInt16:
                case TypeCode.UInt64:
                    l = new Label() { Content = plink.min, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10, Margin = new Thickness(0, 0, -15, 0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l); 
                     var sl = new Slider() { Minimum = (long)plink.min, Maximum = (long)plink.max, Width = 150, TickFrequency = 1, IsSnapToTickEnabled = true };
                    sl.SetBinding(Control.ToolTipProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    sl.SetBinding(Slider.ValueProperty, new Binding() { Source = prop, Path = new PropertyPath("value"), Mode = BindingMode.TwoWay });
                    //sl.ValueChanged+=(s,e)=> { Network.IoTFactory.UpdateProperty(prop); };
                    child.Children.Add(sl);
                    l = new Label() { Content = plink.max, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10,Margin=new Thickness(-15,0,0,0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    l = new Label() {  HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12};
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    l.SetBinding(Label.ContentProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    child.Children.Add(l);   
                    break;
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                    l = new Label() { Content = plink.min, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10 };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                     sl = new Slider() { Minimum = (double)plink.min, Maximum = (double)plink.max ,Width=150,TickFrequency= (double)(plink.max-plink.min)/100.0, IsSnapToTickEnabled =true};
                    sl.SetBinding(Control.ToolTipProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    sl.SetBinding(Slider.ValueProperty, new Binding() { Source = prop, Path = new PropertyPath("value"), Mode = BindingMode.TwoWay });
                    //sl.ValueChanged += (s, e) => { Network.IoTFactory.UpdateProperty(prop); };
                    child.Children.Add(sl); 
                     l = new Label() { Content = plink.max, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10, Margin = new Thickness(-15, 0, 0, 0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    l = new Label() { HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12 };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    l.SetBinding(Label.ContentProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    child.Children.Add(l); 
                    break;
                case TypeCode.String:
                    var tmp1 = new TextBox() { Width = 100, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                    tmp1.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    tmp1.SetBinding(TextBox.TextProperty, new Binding() { Source = prop, UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath("value"), Mode = BindingMode.TwoWay });
                    //tmp1.TextChanged += (s, e) => { Network.IoTFactory.UpdateProperty(prop); };
                    child.Children.Add(tmp1); 
                    break;
            }
            return child;
        }
    }

    class ModelView
    {
        [DisplayName("Viewid1")]
        public string ModelName { get; set; }
        [DisplayName("Viewid2")]
        public string ObjectName { get; set; }
        [DisplayName("Viewid3")]
        public string PropertyName { get; set; }

        [DisplayName("Viewid4")]
        public string Value { get; set; }
        [DisplayName("Editid3")]
        public string Type { get; set; }

        [DisplayName("Viewid5")]
        public string Listeners { get; set; }
    }
}
