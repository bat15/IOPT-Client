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
            if (instance == null) instance = new WSettings();
            return instance;
        }

        private WSettings()
        {
            InitializeComponent();
           
            styleBox.SelectionChanged += ThemeChange;
            styleBox.ItemsSource = Settings.Themes;
            styleBox.SelectedItem = Settings.Get().Theme ?? "Light";

            langBox.SelectionChanged += LanguageChange;
            langBox.ItemsSource = Settings.Languages;
            langBox.SelectedItem = Settings.Get().Language ?? "Russian";


            uintBox.ItemsSource = new string[] {"1","5","30","60","600","1800","3600"};
            uintBox.SelectedItem = Settings.Get().AutoUpdateInterval.ToString() ?? "1";
            uintBox.SetBinding(ComboBox.SelectedItemProperty, new Binding() { Source = Settings.Get(), Path = new PropertyPath("AutoUpdateInterval"), Mode = BindingMode.TwoWay });
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
            Settings.Get().Theme = (styleBox.SelectedItem as string);
        }

        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {
            Settings.Get().Language = (langBox.SelectedItem as string);
        }
    }
}
