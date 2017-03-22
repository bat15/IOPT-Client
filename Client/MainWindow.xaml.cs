﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        public MainWindow()
        {
            Application.Current.Exit += (s, e) => { Settings.Get.Save(); };
            Closed += (s, e) => { Controller.Close(); };
            InitializeComponent();
            Settings.Load();
            if (!string.IsNullOrEmpty(Settings.Get.Login) && !string.IsNullOrEmpty(Settings.Get.Password) && !string.IsNullOrEmpty(Settings.Get.Server) && Settings.Get.Server != null)
                if (Network.Connect(Settings.Get.Server, Settings.Get.Login, Settings.Get.Password))
                {
                    Main.GetMainWindow().label1.Content = Settings.Get.Login + '@' + Settings.Get.Server;
                    Main.GetMainWindow().Show();
                    Hide();
                }
            instance = this;
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

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Controller.Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            WSettings.GetSettingsWindow().Show();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            SplashScreen splashScreen = new SplashScreen("IOPT_Splash.png");
            splashScreen.Show(false);
            Hide();
            try
            {
                if (true)//Network.Connect(textBox.Text, TBLogin.Text, TBPass.Text))
                {
                    Main.GetMainWindow().label1.Content = TBLogin.Text + '@' + textBox.Text;
                    Main.GetMainWindow().DGProp.ItemsSource = View.ModelToView();
                    Main.GetMainWindow().Show();
                    
                    Settings.Get.Login = TBLogin.Text;
                    Settings.Get.Password = TBPass.Text;
                    Settings.Get.Server = textBox.Text;
                    Settings.Get.Save();
                    splashScreen.Close(new TimeSpan(0));
                }
                else
                {
                    splashScreen.Close(new TimeSpan(0));
                    Show();
                    Message.Show((string)Application.Current.Resources["Errid1"], (string)Application.Current.Resources["Dialogid5"]);
                }
            }
            catch (Exception ex) { Show(); splashScreen.Close(new TimeSpan(0)); Message.Show((string)Application.Current.Resources["Errid1"] + ex.Message, (string)Application.Current.Resources["Dialogid5"]); }
        }

        private void BGetCert_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
