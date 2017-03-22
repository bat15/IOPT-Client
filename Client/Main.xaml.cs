using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {

        static Main instance;

        public static Main GetMainWindow()
        {
            return instance ?? (instance = new Main());
        }
        private Main()
        {
            InitializeComponent();
            Lobjects.SelectionChanged += (s, e) =>
            {
                stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Object);
            };
            Snapshot.current.models.CollectionChanged += Notified;
            BUpdate_Click(null, null);
            Network.AutoUpdate();
        }

        public void Notified(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DGProp.ItemsSource = null;
            DGProp.ItemsSource = View.ModelToView();
            int tmp = Lmodels.SelectedIndex;
            Lmodels.ItemsSource = null;
            Lmodels.ItemsSource = Snapshot.current.models;
            Lmodels.SelectedIndex = tmp;
            if (Lmodels.SelectedItem != null)
            {
                stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Object);
            }
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Controller.Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            WSettings.GetSettingsWindow().Show();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Editor.GetEditWindow().Show();
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var displayName = View.GetPropertyDisplayName(e.PropertyDescriptor);

            if (!string.IsNullOrEmpty(displayName))
            {
                e.Column.Header = displayName;
            }

        }

        private async void BUpdate_Click(object sender, RoutedEventArgs e)
        {
            Controller.GenerateTestData();
            //Snapshot.Current.LastUpdate = DateTimeOffset.Now;
            //Message.Show(Controller.Serialize(), "");
            //Message.Show(JsonConvert.SerializeObject(Snapshot.Current.Models.First().Objects.First().Properties[3]), "");
            if ((sender as Button) != null)
            {
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Loading");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var but = new Border { Child = path };
                var da = new DoubleAnimation(0, 359, new Duration(TimeSpan.FromMilliseconds(600)));
                var rt = new RotateTransform();
                but.RenderTransform = rt;
                but.RenderTransformOrigin = new Point(0.5, 0.5);
                da.RepeatBehavior = RepeatBehavior.Forever;
                ((Button)sender).Content = but;
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            await Task.Run(async () =>
            {
                try
                {
                    //Network.GetDataFromServer();
                    Snapshot.current.lastUpdate = DateTimeOffset.Now.ToString();
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        DGProp.ItemsSource = View.ModelToView();
                        Lmodels.ItemsSource = Snapshot.current.models;
                        if (Lobjects.SelectedItem != null)
                        {
                            stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Object);
                        }
                    }));
                }
                catch { Message.Show((string)Application.Current.Resources["Dialogid7"], (string)Application.Current.Resources["Dialogid5"]); }
            });
            if ((sender as Button) != null)
            {
                var path = new System.Windows.Shapes.Path() { };
                path.SetResourceReference(System.Windows.Shapes.Path.DataProperty, "Update");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var but = new Border() { Child = path };
                ((Button)sender).Content = but;
            }
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox) == null) return;
            DGProp.ItemsSource = View.ModelToView();
            label.Visibility = !(bool)((CheckBox)sender).IsChecked ? Visibility.Visible : Visibility.Hidden;
            lmgrid.Visibility = !(bool)((CheckBox)sender).IsChecked ? Visibility.Visible : Visibility.Hidden;
            stackscroll.Visibility = !(bool)(sender as CheckBox).IsChecked ? Visibility.Visible : Visibility.Hidden;
            DGProp.Visibility = (bool)(sender as CheckBox).IsChecked ? Visibility.Visible : Visibility.Hidden;
        }

        private void BLogout_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)Message.Show((string)Application.Current.Resources["Dialogid2"], (string)Application.Current.Resources["Dialogid4"], true)) return;
            Settings.Get.Login = null;
            Settings.Get.Password = null;
            Settings.Get.Server = null;
            MainWindow.instance.Show();
            WSettings.GetSettingsWindow().Close();
            Editor.GetEditWindow().Close();
            Close();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Controller.Close();
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is Object)
            {
                //if (Lobjects.SelectedItem == null) { Message.Show((string)Application.Current.Resources["Dialogid6"], (string)Application.Current.Resources["Dialogid5"]); return; }
                Lobjects.SelectedItem = (sender as Button).Tag;
                new WDashbordCreate((sender as Button).Tag as Object).ShowDialog();
                Lobjects.SelectedIndex = -1;
                Lobjects.SelectedItem = (sender as Button).Tag;
            }
        }

        private async void BUpload_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                var path = new System.Windows.Shapes.Path() { };
                path.SetResourceReference(System.Windows.Shapes.Path.DataProperty, "Loading");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var but = new Border() { Child = path };
                var da = new DoubleAnimation(0, 359, new Duration(TimeSpan.FromMilliseconds(600)));
                var rt = new RotateTransform();
                but.RenderTransform = rt;
                but.RenderTransformOrigin = new Point(0.5, 0.5);
                da.RepeatBehavior = RepeatBehavior.Forever;
                (sender as Button).Content = but;
                rt.BeginAnimation(RotateTransform.AngleProperty, da);
            }
            await Task.Run(() =>
            {
                try
                {
                    Network.SendDataToServer();
                }
                catch { Message.Show((string)Application.Current.Resources["Dialogid8"], (string)Application.Current.Resources["Dialogid5"]); }
            });
            if (sender != null)
            {
                var path = new System.Windows.Shapes.Path() { };
                path.SetResourceReference(System.Windows.Shapes.Path.DataProperty, "Upload");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var but = new Border() { Child = path };
                (sender as Button).Content = but;
            }
        }
    }
}
