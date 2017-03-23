﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Client.Classes
{
    internal class View
    {
        public static IEnumerable<ModelView> ModelToView()
        {
            string lastM = null;
            Func<string, string> ch1 = a => { if (a == lastM) return null;
                lastM = a; return a;
            };
            string lastO = null;
            Func<string, string> ch2 = a => { if (a == lastO) return null;
                lastO = a; return a;
            };
            return
                Snapshot.current.models.SelectMany(m => m.Objects, (m, o) => new {m, o})
                    .SelectMany(@t1 => @t1.o.Properties, (@t1, p) => new ModelView
                    {
                        ModelName = ch1(@t1.m.Name),
                        ObjectName = ch2(@t1.o.Name),
                        PropertyName = p.Name,
                        Value = p.Value,
                        Type = ((TypeCode) p.Type).ToString(),
                        Listeners = string.Join(", ", (from t in p.Scripts select t.Name).ToArray())
                    });
        }

        public static string GetPropertyDisplayName(object descriptor)
        {
            var pd = descriptor as PropertyDescriptor;

            if (pd != null)
            {
                var displayName = pd.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

                if (displayName != null && !Equals(displayName, DisplayNameAttribute.Default))
                {
                    return GetDisplayNameFromRes(displayName.DisplayName);
                }

            }
            else
            {
                var pi = descriptor as PropertyInfo;

                if (pi == null) return null;
                var attributes = pi.GetCustomAttributes(typeof(DisplayNameAttribute), true);
                for (var i = 0; i < attributes.Length; ++i)
                {
                    var displayName = attributes[i] as DisplayNameAttribute;
                    if (displayName != null && !Equals(displayName, DisplayNameAttribute.Default))
                    {
                        return GetDisplayNameFromRes(displayName.DisplayName);
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
            catch
            {
                // ignored
            }
            return res;
        }


        public static UIElement GetDashboard(Object obj)
        {
            var result = new StackPanel { Orientation = Orientation.Vertical };
            result.MouseDown += Main.GetMainWindow().Drag;
            if (obj == null) return result;
            var dashboards = (from t in Snapshot.current.dashboards where t.ObjectId == obj.Id select t).ToList();
            if (!dashboards.Any()) return result;
            foreach (var dash in dashboards)
            {
                var dashboard = new StackPanel { Orientation = Orientation.Horizontal, SnapsToDevicePixels = true };
                dashboard.SetResourceReference(Control.BackgroundProperty,"AlternativeBackgroundColor");
                var border = new Border { Child = dashboard, BorderThickness = new Thickness(1), SnapsToDevicePixels = true };
                var temp = new Grid { Margin = new Thickness(6, 16, 6, 6),SnapsToDevicePixels=true};
                temp.Children.Add(border);

                #region fullWindowbutton
                var path = new Path { Stretch = Stretch.Uniform };
                path.SetResourceReference(Path.DataProperty, "DashFull");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var fullWindow = new Button
                {
                    Style = Application.Current.FindResource("TranspButton") as Style,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(5),
                    Width = 25,
                    Height = 25,
                    Content = path
                };
                Panel.SetZIndex(fullWindow, 20);
                fullWindow.Click += (s, ee) => { Message.Show("Comming soon", ""); };
                temp.Children.Add(fullWindow);
                #endregion

                border.SetResourceReference(Control.BorderBrushProperty, "MainColor");
                var e = new StackPanel { Orientation = Orientation.Vertical,UseLayoutRounding=true };
                var n = new StackPanel { Orientation = Orientation.Vertical, UseLayoutRounding = true };
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
                        n.Children.Add(GetPropertyView(t.Property,t));
                    }
                }
                result.Children.Add(temp);
            }
            return result;
        }

        private static UIElement GetPropertyView(Property prop, Dashboard.PropertyMap plink)
        {
            var child = new StackPanel { Orientation = Orientation.Horizontal };
            var l = new Label { Content = prop.Name, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 200 };
            l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
            child.Children.Add(l);
            switch (prop.Type)
            {
                case 3:
                    var checkBox = new CheckBox
                    {
                        Style = Application.Current.FindResource("StaticCheckBox") as Style,
                        Width = 20,
                        Height = 20,
                        Margin = new Thickness(8)
                    };
                    checkBox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                        new Binding {Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay});
                    child.Children.Add(checkBox);
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
                    var pb = new ProgressBar
                    {
                        Width = 150,
                        Minimum = plink.Min ?? 0,
                        Maximum = plink.Max ?? 0,
                        Margin = new Thickness(4),
                        IsIndeterminate = false
                    };
                    pb.SetBinding(System.Windows.Controls.Primitives.RangeBase.ValueProperty,
                        new Binding {Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay});
                    pb.SetResourceReference(Control.ForegroundProperty, "MainColor");
                    l = new Label
                    {
                        Content = double.Parse(prop.Value),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 20,
                        FontFamily = new FontFamily("Consolas")
                    };
                    l.SetResourceReference(Control.ForegroundProperty, "MainColor");
                    l.SetBinding(ContentControl.ContentProperty,
                        new Binding {Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay});
                    child.Children.Add(pb);
                    child.Children.Add(l);
                    break;
                case 18:
                    l = new Label
                    {
                        Content = "\"" + prop.Value + "\"",
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 12,
                        FontFamily = new FontFamily("Consolas")
                    };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    l.SetBinding(ContentControl.ContentProperty,
                        new Binding {Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay});
                    child.Children.Add(l);
                    break;
            }
            return child;
        }

        private static UIElement GetManagePropertyView(Property prop, Dashboard.PropertyMap plink)
        {           
            var child = new StackPanel { Orientation = Orientation.Horizontal };
            var l = new Label { Content = prop.Name, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 130 };
            l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
            child.Children.Add(l);
            switch ((TypeCode)prop.Type)
            {
                case TypeCode.Boolean:
                    var tmp = new CheckBox { Margin=new Thickness(5)};
                    tmp.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding { Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    tmp.Checked += async (s,e) =>
                    {
                        await Task.Run(() =>
                            { Network.IoTFactory.ModifyProperty(prop); });
                    };
                    tmp.Unchecked += async (s, e) => {
                    await Task.Run(() =>
                    { Network.IoTFactory.ModifyProperty(prop);});
                    };
                    child.Children.Add(tmp);
                    break;
                case TypeCode.Int32:
                case TypeCode.Int16:
                case TypeCode.Int64:
                case TypeCode.UInt32:
                case TypeCode.UInt16:
                case TypeCode.UInt64:
                    l = new Label { Content = plink.Min, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10, Margin = new Thickness(0, 0, -15, 0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l); 
                     var sl = new Slider { Minimum = plink.Min ?? 0, Maximum = plink.Max ?? 0, Width = 150, TickFrequency = 1, IsSnapToTickEnabled = true };
                    sl.SetBinding(FrameworkElement.ToolTipProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    sl.SetBinding(System.Windows.Controls.Primitives.RangeBase.ValueProperty, new Binding { Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    //sl.ValueChanged+=(s,e)=> { Network.IoTFactory.ModifyProperty(prop); };
                    child.Children.Add(sl);
                    l = new Label { Content = plink.Max, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10,Margin=new Thickness(-15,0,0,0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    l = new Label {  HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12};
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    l.SetBinding(ContentControl.ContentProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    child.Children.Add(l);   
                    break;
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                    if (plink.Min == null || plink.Max == null) break;
                    l = new Label { Content = plink.Min, HorizontalContentAlignment = HorizontalAlignment.Right, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10 };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                     sl = new Slider { Minimum = plink.Min ?? 0, Maximum = plink.Max ?? 0, Width=150,TickFrequency= (plink.Max-plink.Min)/ 100.0 ?? 0, IsSnapToTickEnabled =true};
                    sl.SetBinding(FrameworkElement.ToolTipProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    sl.SetBinding(System.Windows.Controls.Primitives.RangeBase.ValueProperty, new Binding { Source = prop, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                    //sl.ValueChanged += (s, e) => { Network.IoTFactory.ModifyProperty(prop); };
                    child.Children.Add(sl); 
                     l = new Label { Content = plink.Max, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 10, Margin = new Thickness(-15, 0, 0, 0) };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    child.Children.Add(l);
                    l = new Label { HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12 };
                    l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    l.SetBinding(ContentControl.ContentProperty, new Binding("Value") { Source = sl, Mode = BindingMode.OneWay });
                    child.Children.Add(l); 
                    break;
                case TypeCode.String:
                    var tmp1 = new TextBox { Width = 100, HorizontalContentAlignment = HorizontalAlignment.Left, Margin = new Thickness(4), VerticalContentAlignment = VerticalAlignment.Center };
                    tmp1.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                    tmp1.SetBinding(TextBox.TextProperty, new Binding { Source = prop, UpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });

                    /*
                     * TODO 
                     * Придумать что-нибудь с уменьшением 
                     * трафика мб на моазу лив и ентер повесить,
                     * завернуть все вызовы в другие потоки                   
                   */
                    //tmp1.TextChanged += (s, e) => { Network.IoTFactory.ModifyProperty(prop); };
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
