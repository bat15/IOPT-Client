using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class WSettings : Window
    {
        static WSettings instance;

        public static WSettings GetSettingsWindow()
        {
            return instance ?? (instance = new WSettings());
        }

        private WSettings()
        {
            InitializeComponent();
           
            styleBox.SelectionChanged += ThemeChange;
            styleBox.ItemsSource = Enum.GetValues(typeof(Settings.Themes)).Cast<Settings.Themes>();
            styleBox.SelectedItem = Settings.Get.Theme;

            langBox.SelectionChanged += LanguageChange;
            langBox.ItemsSource = Enum.GetValues(typeof(Settings.Languages)).Cast<Settings.Languages>();
            langBox.SelectedItem = Settings.Get.Language;


            uintBox.ItemsSource = new string[] {"1","5","30","60","600","1800","3600"};
            uintBox.SelectedItem = Settings.Get.AutoUpdateInterval.ToString() ?? "1";
            uintBox.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, new Binding() { Source = Settings.Get, Path = new PropertyPath("AutoUpdateInterval"), Mode = BindingMode.TwoWay });
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Hide();
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

        private void ThemeChange(object sender, SelectionChangedEventArgs e)
        {
            Settings.Get.Theme = (Settings.Themes)styleBox.SelectedItem;
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            Settings.Get.Language = (Settings.Languages)langBox.SelectedItem;
        }
    }
}
