using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Classes;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool? _editState;
        public static MainWindow Instance;
        public MainWindow()
        {
            Application.Current.Exit += (s, e) => { Settings.Save(); };
            Closed += (s, e) => { Controller.Close(); };
            InitializeComponent();
            GNew.MouseDown += Drag;
            GList.MouseDown += Drag;        
            BExit.Click += (s, e) => { Controller.Close(); };
            Settings.Load();
            Accounts.ItemsSource = Settings.AccountsSettings;
            Instance = this;
            label4.Content = typeof(Model).Assembly.GetName().Version;
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

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text) || string.IsNullOrWhiteSpace(TBLogin.Text) || string.IsNullOrWhiteSpace(TBPass.Password))
                return;
            try
            {
                if (_editState == false)
                {
                    var settings = new Settings
                    {
                        Login = TBLogin.Text,
                        Password = TBPass.Password,
                        Server = textBox.Text
                    };

                    Settings.AccountsSettings.Add(settings);
                }
                if (_editState == true)
                {
                    var settings = (sender as Button)?.Tag as Settings;
                    if (settings == null) return;
                    settings.Login = TBLogin.Text;
                    settings.Password = TBPass.Password;
                    settings.Server = textBox.Text;
                }
                _editState = null;
                ChangeScene(false);
            }
            catch
            {
                // ignored
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (e.ChangedButton != MouseButton.Left) return;
            //Cursor = Cursors.Wait;
            SplashScreen splashScreen = new SplashScreen("IOPT_Splash.png");
            splashScreen.Show(false);
            Hide();
            try
            {
                var o = sender as Label;
                if (o != null) Settings.Current = o.Tag as Settings;
                if (true)//Network.Connect(textBox.Text, TBLogin.Text, TBPass.Text))
                {
                    Cursor = Cursors.Arrow;
                    splashScreen.Close(new TimeSpan(0));
                    Main.GetMainWindow().DGProp.ItemsSource = View.ModelToView();
                    Main.GetMainWindow().Show();
                }
                else
                {
                    Show();
                    splashScreen.Close(new TimeSpan(0));
                    Message.Show((string)Application.Current.Resources["Errid1"], (string)Application.Current.Resources["Dialogid5"]);
                }
            }
            catch (Exception ex) { Show(); splashScreen.Close(new TimeSpan(0)); Message.Show((string)Application.Current.Resources["Errid1"] + ex.Message, (string)Application.Current.Resources["Dialogid5"]); }
        }

        private void EditEvent(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && !(button.Tag is Settings)) return;
            _editState = true;
            Connect.Tag = button?.Tag;
            ChangeScene(true);
            TBLogin.Text = ((Settings)button?.Tag)?.Login;
            TBPass.Password = ((Settings)button?.Tag)?.Password;
            textBox.Text = ((Settings)button?.Tag)?.Server;

        }

        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && !(button.Tag is Settings)) return;
            Settings.AccountsSettings.Remove((Settings)button?.Tag);
            Settings.Save();
        }

        private void BAdd_OnClick(object sender, RoutedEventArgs e)
        {
            _editState = false;
            ChangeScene(true);
        }

        private void BBack_OnClick(object sender, RoutedEventArgs e)
        {
            ChangeScene(false);
        }

        private void ChangeScene(bool isNew)
        {
            if (isNew)
            {
                if (_editState == null) return;
                Connect.Content = (bool)_editState ? (string)Application.Current.Resources["Sid7"] : (string)Application.Current.Resources["Sid6"];
                GList.Visibility = Visibility.Hidden;
                GNew.Visibility = Visibility.Visible;
                //TBLogin.Text = TBPass.Password = TBServer.Text = TBPort.Text = TBDB.Text = "";
            }
            else
            {
                GList.Visibility = Visibility.Visible;
                GNew.Visibility = Visibility.Hidden;
            }
        }
    }
}
