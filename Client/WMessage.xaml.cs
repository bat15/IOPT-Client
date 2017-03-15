using System.Windows;
using System.Windows.Input;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для WMessage.xaml
    /// </summary>
    public partial class WMessage : Window
    {
        internal WMessage()
        {
            InitializeComponent();
            textBox.IsReadOnly = true;
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

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public static class Message
    {
        public static bool? Show(string message, string caption, bool isYesNo)
        {
            if (!isYesNo) { Show(message, caption); return false; }
            var a = new WMessage
            {
                Yes = {Visibility = Visibility.Visible},
                No = {Visibility = Visibility.Visible},
                Ok = {Visibility = Visibility.Hidden},
                textBox = {Text = message},
                label = {Content = caption}
            };
            return a.ShowDialog();
        }

        public static void Show(string message, string caption)
        {
            var a = new WMessage
            {
                Yes = {Visibility = Visibility.Hidden},
                No = {Visibility = Visibility.Hidden},
                Ok = {Visibility = Visibility.Visible},
                textBox = {Text = message},
                label = {Content = caption}
            };
            a.ShowDialog();
        }
    }
}
