using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Client.Classes;
using OxyPlot;
using OxyPlot.Wpf;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using VerticalAlignment = System.Windows.VerticalAlignment;
using System.Windows.Data;
using System.Windows.Shapes;

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
            Title.Text = (from t in Platform.Current.Models.SelectMany(x => x.Objects) where t.Id == dashboard.ObjectId select t.Name).FirstOrDefault();
            BExit.Click += (s, e) => { Close(); };
            BMinimize.Click += (s, e) => { WindowState = WindowState.Minimized; };
            BMaximize.Click += (s, e) => { WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized; };
            foreach (var pm in dashboard.View)
            {
                var border = new Border
                {
                    Margin = new Thickness(0, 5, 0, 5),
                    BorderThickness = new Thickness(2)
                };
                border.SetResourceReference(BorderBrushProperty, "MainColor");

                var grid = new Grid { Height = 220 };
                grid.MouseDown += Drag;
                grid.SetResourceReference(BackgroundProperty, "AlternativeBackgroundColor");
                border.Child = grid;
                ContentPane.Children.Add(border);
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });
                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(3, GridUnitType.Star)
                });
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });


                var label = new Label
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Content = pm.Property.Name
                };
                label.SetResourceReference(ForegroundProperty, "OnLightFontColor");
                label.SetResourceReference(FontFamilyProperty, "FontFamilyHighlight");
                label.SetResourceReference(FontSizeProperty, "FontSizeBig");
                Grid.SetRow(label, 0);
                Grid.SetColumn(label, 2);
                grid.Children.Add(label);


                if (pm.Property.Type == 3)
                {
                    if (pm.IsControl)
                    {

                        var checkBox = new CheckBox
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0, 40, 0, 0)
                        };
                        checkBox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, new Binding { Source = pm.Property, Path = new PropertyPath("Value"), Mode = BindingMode.TwoWay });
                        Grid.SetRow(checkBox, 0);
                        Grid.SetColumn(checkBox, 0);
                        grid.Children.Add(checkBox);
                    }
                    else
                    {
                        var checkBox = new CheckBox
                        {
                            Style = Application.Current.FindResource("StaticCheckBox") as Style,
                            Width = 60,
                            Height = 60,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0, 40, 0, 0)
                        };
                        checkBox.SetBinding(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty,
                            new Binding { Source = pm.Property, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay });
                        Grid.SetRow(checkBox, 0);
                        Grid.SetColumn(checkBox, 0);
                        grid.Children.Add(checkBox);
                    }
                    var plot = CreatePlot(pm);
                    Grid.SetRow(plot, 0);
                    Grid.SetColumn(plot, 2);
                    grid.Children.Add(plot);
                }

                #region Number
                if (pm.Property.Type >= 7 && pm.Property.Type <= 15)
                {
                    if (pm.IsControl)
                    {

                    }
                    else
                    {
                        var pb = new ProgressBar
                        {
                            Minimum = pm.Min ?? 0,
                            Maximum = pm.Max ?? 0,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(10, 0, 40, 0),
                            Height = 80,
                            IsIndeterminate = false
                        };
                        pb.SetBinding(System.Windows.Controls.Primitives.RangeBase.ValueProperty,
                            new Binding { Source = pm.Property, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay });
                        pb.SetResourceReference(Control.ForegroundProperty, "MainColor");
                        var l = new Label
                        {
                            Content = double.Parse(pm.Property.Value),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 20,
                            FontFamily = new FontFamily("Consolas")
                        };
                        l.SetResourceReference(Control.ForegroundProperty, "MainColor");
                        l.SetBinding(ContentControl.ContentProperty,
                            new Binding { Source = pm.Property, Path = new PropertyPath("Value"), Mode = BindingMode.OneWay });
                        Grid.SetRow(pb, 0);
                        Grid.SetColumn(pb, 0);
                        grid.Children.Add(pb);
                        Grid.SetRow(l, 0);
                        Grid.SetColumn(l, 0);
                        grid.Children.Add(l);

                    }
                    var plot = CreatePlot(pm);
                    Grid.SetRow(plot, 0);
                    Grid.SetColumn(plot, 2);
                    grid.Children.Add(plot);
                }
                #endregion
                if (pm.Property.Type == 18)
                {

                }
            }
        }

        private static Plot CreatePlot(Dashboard.PropertyMap pm)
        {
            var plot = new Plot
            {
                Margin = new Thickness(0, 40, 0, 0),
                //Title = pm.Property.Name,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                // Width = SystemParameters.PrimaryScreenWidth * 0.4,
                Style = Application.Current.FindResource("OxyLineStyle") as Style,
            };

            plot.SetResourceReference(BackgroundProperty, "BackgroundColor");
            plot.SetResourceReference(ForegroundProperty, "MainColor");
            var convertFromString = ColorConverter.ConvertFromString("#F62A00");
            if (convertFromString != null)
            {
                var lineserie = new LineSeries
                {
                    ItemsSource = pm.Property.Changes,
                    DataFieldY = "Value",
                    DataFieldX = "Name",
                    StrokeThickness = 1,
                    MarkerSize = 3,
                    LineStyle = LineStyle.Solid,
                    MarkerType = MarkerType.Cross,
                    Color = (Color)convertFromString,
                };

                var dateAxis = new DateTimeAxis { MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None, IntervalLength = 80, AbsoluteMinimum = pm.Property.Changes.Min(x => x.Name).ToOADate() };
                dateAxis.SetResourceReference(Axis.TitleProperty, "Viewid7");
                plot.Axes.Add(dateAxis);
                var valueAxis = new LinearAxis { MajorGridlineStyle = LineStyle.None, MinorGridlineStyle = LineStyle.None };
                valueAxis.SetResourceReference(Axis.TitleProperty, "Viewid6");
                //lags
                //pm.Property.Changes.CollectionChanged += (s, e) =>
                //{
                //    valueAxis.AbsoluteMaximum = pm.Property.Changes.Max(x => x.Value);
                //    valueAxis.AbsoluteMinimum= pm.Property.Changes.Min(x => x.Value);
                //};
                if (pm.Property.Type == 3)
                {
                    valueAxis.AbsoluteMaximum = 1;
                    valueAxis.AbsoluteMinimum = 0;
                }
                plot.Axes.Add(valueAxis);



                plot.Series.Add(lineserie);
            }
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


        #region ResizeWindows

        private bool _resizeInProcess;
        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect == null) return;
            _resizeInProcess = true;
            senderRect.CaptureMouse();
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            var senderRect = sender as Rectangle;
            if (senderRect == null) return;
            _resizeInProcess = false; ;
            senderRect.ReleaseMouseCapture();
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (!_resizeInProcess) return;
            var senderRect = sender as Rectangle;
            var mainWindow = senderRect?.Tag as Window;
            if (mainWindow == null) return;
            var width = e.GetPosition(mainWindow).X;
            var height = e.GetPosition(mainWindow).Y;
            senderRect.CaptureMouse();
            if (senderRect.Name.ToLower().Contains("right"))
            {
                width += 1;
                if (width > 0)
                    mainWindow.Width = width;
            }
            if (senderRect.Name.ToLower().Contains("left"))
            {
                width -= 1;
                mainWindow.Left += width;
                width = mainWindow.Width - width;
                if (width > 0)
                {
                    mainWindow.Width = width;
                }
            }
            if (senderRect.Name.ToLower().Contains("bottom"))
            {
                height += 1;
                if (height > 0)
                    mainWindow.Height = height;
            }
            if (senderRect.Name.ToLower().Contains("top"))
            {
                height -= 1;
                mainWindow.Top += height;
                height = mainWindow.Height - height;
                if (height > 0)
                {
                    mainWindow.Height = height;
                }
            }
        }
        #endregion
    }
}
