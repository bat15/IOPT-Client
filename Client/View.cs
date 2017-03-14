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
            return from m in Snapshot.Current.Models from o in m.Objects from p in o.Properties select new ModelView() { ModelName = ch1(m.Name), ObjectName = ch2(o.Name), PropertyName = p.Name, Value = p.Value, Type =  ((TypeCode)p.Type).ToString(), Listeners = string.Join(", ", (from t in p.Scripts select t.Name).ToArray()) };
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

        public static UIElement GetElemetsPanels(Model model)
        {
            var Result = new StackPanel() { Orientation = Orientation.Vertical };
            if (model==null) return Result;
            foreach (var obj in model.Objects)
            {
                var WP = new StackPanel() { Orientation = Orientation.Horizontal };
                var l = new Label() { Content = obj.Name, HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 16 };
                l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                WP.Children.Add(l);
                Result.Children.Add(WP);
                foreach (var prop in obj.Properties)
                {
                    Result.Children.Add(GetPropertyView(prop));
                }
            }
            return Result;
        }


        public static UIElement GetDashboard(Object model)
        {
            var Result = new StackPanel() { Orientation = Orientation.Vertical };

            if (model == null) return Result;
            var tmp = (from t in Snapshot.Current.Dashboards where t.Parent == model select t);
            if (tmp.Count() == 0) return Result;
            foreach (var dash in tmp)
            {
                var dashboard = new StackPanel() { Orientation = Orientation.Horizontal };
                var border = new Border() { Child = dashboard, BorderThickness = new Thickness(1), Margin = new Thickness(0, 10, 0, 0), Padding = new Thickness(0,2,0,2) };
                border.SetResourceReference(Control.BorderBrushProperty, "MainColor");
                var e = new StackPanel() { Orientation = Orientation.Vertical };
                var n = new StackPanel() { Orientation = Orientation.Vertical };
                dashboard.Children.Add(e);
                dashboard.Children.Add(n);
                foreach (var t in dash.View)
                {
                    if (t.IsControl)
                    {
                        e.Children.Add(GetManagePropertyView(t.Property,t));
                    }
                    else
                    {
                        n.Children.Add(GetPropertyView(t.Property));
                    }
                }
                Result.Children.Add(border);
            }
            return Result;
        }

        private static UIElement GetPropertyView(Property prop)
        {
            var child = new StackPanel() { Orientation = Orientation.Horizontal };
            var l = new Label() { Content = prop.Name, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 200 };
            l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
            child.Children.Add(l);
            switch (prop.Type)
            {
                case 3:
                    if (bool.Parse(prop.Value))
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

                    l = new Label() { Content = double.Parse(prop.Value), HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 20, FontFamily = new System.Windows.Media.FontFamily("Consolas") };
                    l.SetResourceReference(Control.ForegroundProperty, "MainColor");
                    child.Children.Add(l);
                    break;
                case 18:
                    l = new Label() { Content = "\""+prop.Value+ "\"", HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12,FontFamily=new System.Windows.Media.FontFamily("Consolas") };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    break;
            }
            return child;
        }

        private static UIElement GetManagePropertyView(Property prop, Dashboard.PropertyLink plink)
        {           
            var child = new StackPanel() { Orientation = Orientation.Horizontal };
            var l = new Label() { Content = prop.Name, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 130 };
            l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
            child.Children.Add(l);
            switch ((TypeCode)prop.Type)
            {
                case TypeCode.Boolean:
                    var tmp = new CheckBox() { Margin=new Thickness(0,5,0,5)};
                    tmp.SetBinding(CheckBox.IsCheckedProperty, new Binding() { Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    child.Children.Add(tmp);
                    break;
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Int64:
                case TypeCode.UInt32:
                case TypeCode.UInt16:
                case TypeCode.UInt64:
                    l = new Label() { Content = plink.Min, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10, Margin = new Thickness(0, 0, -15, 0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    var sl = new Slider() { Minimum = (long)plink.Min, Maximum = (long)plink.Max, Width = 150, TickFrequency = 1, IsSnapToTickEnabled = true };
                    sl.SetBinding(Control.ToolTipProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    sl.SetBinding(Slider.ValueProperty, new Binding() { Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    child.Children.Add(sl);
                    l = new Label() { Content = plink.Max, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10,Margin=new Thickness(-15,0,0,0) };
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
                    l = new Label() { Content = plink.Min, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10 };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    sl = new Slider() { Minimum = (double)plink.Min, Maximum = (double)plink.Max ,Width=150,TickFrequency=0.1, IsSnapToTickEnabled =true};
                    sl.SetBinding(Control.ToolTipProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    sl.SetBinding(Slider.ValueProperty, new Binding() { Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    child.Children.Add(sl);
                    l = new Label() { Content = plink.Max, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10, Margin = new Thickness(-15, 0, 0, 0) };
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
                    tmp1.SetBinding(TextBox.TextProperty, new Binding() { Source = prop, UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
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
