using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Client.Classes;
using Object = Client.Classes.Object;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для WDashbordCreate.xaml
    /// </summary>
    public partial class WDashbordCreate : Window
    {
        Object obj;
        Dictionary<Property, List<UIElement>> results = new Dictionary<Property, List<UIElement>>();
        public WDashbordCreate(Object obj)
        {
            InitializeComponent();
            this.obj = obj;
            label.Content = obj.Name;

            foreach (var p in obj.Properties)
            {
                var list = new List<UIElement>();
                var child = new StackPanel() { Orientation = Orientation.Horizontal };
                var l = new Label() { Content = p.Name, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Width = 150 };
                l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                child.Children.Add(l);
                //l = new Label() { Content = (string)Application.Current.Resources["Editid1"]+"?", HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12 };
                //l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                //child1.Children.Add(l);
                var tmp = new CheckBox() { IsChecked=true};
                list.Add(tmp);
                child.Children.Add(tmp);
                //l = new Label() { Content = (string)Application.Current.Resources["WDCid1"], HorizontalContentAlignment = HorizontalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 12 };
                //l.SetResourceReference(Control.ForegroundProperty, "OnLightFontColor");
                //child1.Children.Add(l);
                tmp = new CheckBox() { Margin = new Thickness(55, 0, 0, 0) };
                tmp.SetBinding(Control.IsEnabledProperty, new Binding("IsChecked") { Source = (list[0] as CheckBox), Mode = BindingMode.OneWay });
                list.Add(tmp);
                child.Children.Add(tmp);
                switch ((TypeCode)p.Type)
                {
                    case TypeCode.Boolean:
                    case TypeCode.String:
                        break;
                    case TypeCode.Int32:
                    case TypeCode.Int16:
                    case TypeCode.Int64:
                    case TypeCode.UInt32:
                    case TypeCode.UInt16:
                    case TypeCode.UInt64:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.Single:
                        var textbox = new TextBox() { Text="0",Width = 50,Margin=new Thickness(80,0,0,0) };
                        textbox.SetBinding(Control.IsEnabledProperty, new Binding("IsChecked") { Source = (list[0] as CheckBox), Mode = BindingMode.OneWay });
                        list.Add(textbox);
                        child.Children.Add(textbox);
                        textbox = new TextBox() { Text = "100", Width = 50, Margin = new Thickness(30, 0, 0, 0) };
                        textbox.SetBinding(Control.IsEnabledProperty, new Binding("IsChecked") { Source = (list[0] as CheckBox), Mode = BindingMode.OneWay });
                        list.Add(textbox);
                        child.Children.Add(textbox);
                        break;
                }
                results.Add(p, list);
                sw.Children.Add(child);
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            //Добавить круд для дажбородов
            var d = new Dashboard(obj);
            foreach (var pair in results)
                if ((bool)(pair.Value[0] as CheckBox).IsChecked)
                {
                    if (pair.Key.Type < 7 || pair.Key.Type > 15)
                        d.View.Add(new Dashboard.PropertyMap(int.MaxValue - d.View.Count, pair.Key as Property,(long)d.Id, (bool)(pair.Value[1] as CheckBox).IsChecked,null,null));
                    else
                    {
                        double min, max;
                        double.TryParse((pair.Value[2] as TextBox).Text,out min);
                        double.TryParse((pair.Value[3] as TextBox).Text, out max);
                        if (min >= max) { Message.Show((string)Application.Current.Resources["Errid3"], (string)Application.Current.Resources["Dialogid5"]); return; }
                        d.View.Add(new Dashboard.PropertyMap(int.MaxValue - d.View.Count, pair.Key as Property, (long)d.Id, (bool)(pair.Value[1] as CheckBox).IsChecked, min, max));
                    }
                }
            Classes.Client.Current.Dashboards.Add(d);
            Close();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Opacity = 0.5;
                DragMove();
                Opacity = 1;
            }
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                WindowState = WindowState != WindowState.Minimized ? WindowState.Minimized : WindowState.Normal;
            }
        }
    }
}
