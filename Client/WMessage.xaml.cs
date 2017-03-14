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
        public static bool? Show(string Message, string Caption, bool isYesNo)
        {
            if (!isYesNo) { Show(Message, Caption); return false; }
            var a = new WMessage();
            a.Yes.Visibility = Visibility.Visible;
            a.No.Visibility = Visibility.Visible;
            a.Ok.Visibility = Visibility.Hidden;
            a.textBox.Text = Message;
            a.label.Content = Caption;
            return a.ShowDialog();
        }

        public static void Show(string Message, string Caption)
        {
            var a = new WMessage();
            a.Yes.Visibility = Visibility.Hidden;
            a.No.Visibility = Visibility.Hidden;
            a.Ok.Visibility = Visibility.Visible;
            a.textBox.Text = Message;
            a.label.Content = Caption;
            a.ShowDialog();
        }
    }
}
