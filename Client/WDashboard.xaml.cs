using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Client.Classes;
using OxyPlot;
using OxyPlot.Wpf;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для WDashboard.xaml
    /// </summary>
    public partial class WDashboard : Window
    {
        private Dashboard _dashboard;


        public WDashboard(Dashboard dashboard)
        {
            InitializeComponent();
            _dashboard = dashboard;

            Title.Text = dashboard.ObjectId.ToString();
            BExit.Click += (s, e) => { Close(); };
            BMinimize.Click += (s, e) => { WindowState=WindowState.Minimized; };
            var i = 0;
            foreach (var pm in dashboard.View)
            {
                if (pm.Property.Type != 3 && (pm.Property.Type < 7 || pm.Property.Type > 15)) continue;
                var p=CreatePlot(pm);
                p.Width = 900;
                p.Height = 200;
                p.HorizontalAlignment=HorizontalAlignment.Left;
                p.VerticalAlignment = VerticalAlignment.Top;
                p.Margin=new Thickness(20,200*i+50,0,0);
                GMain.Children.Add(p);
                i++;
            }
        }

        private Plot CreatePlot(Dashboard.PropertyMap pm)
        {
            var plot = new Plot
            {
                Margin = new Thickness(0, 20, 0, 20),
                Title = pm.Property.Name
            };
            plot.SetResourceReference(BackgroundProperty, "BackgroundColor");
            plot.SetResourceReference(ForegroundProperty, "MainColor");
            var lineserie = new LineSeries
            {
                ItemsSource = pm.Property.Changes,
                DataFieldY = "Value",
                DataFieldX = "Name",
                StrokeThickness = 1,
                MarkerSize = 1,
                LineStyle = LineStyle.Solid,
                MarkerType = MarkerType.Cross,
                
            };

            var dateAxis = new DateTimeAxis { MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None, IntervalLength = 80 };
            plot.Axes.Add(dateAxis);
            var valueAxis = new LinearAxis { MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None};
            plot.Axes.Add(valueAxis);

            plot.Series.Add(lineserie);
            plot.UpdateLayout();
            return plot;
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
