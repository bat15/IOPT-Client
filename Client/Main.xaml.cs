using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Client.Classes;
using Model = Client.Classes.Model;
using Object = Client.Classes.Object;
//"$(SolutionDir)\ILMerge\merge_all.bat" "$(SolutionDir)" "$(TargetPath)" $(ConfigurationName)
namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main
    {
        private static Main _instance;

        public static Main GetMainWindow()
        {
            return _instance ?? (_instance = new Main());
        }
        private Main()
        {
            InitializeComponent();
            Lobjects.SelectionChanged += (s, e) =>
            {
                stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Object);
            };
            Platform.Current.Models.CollectionChanged += Notified;
            BUpdate_Click(null, null);
            //Network.AutoUpdate();

            styleBox.ItemsSource = Enum.GetValues(typeof(Settings.Themes)).Cast<Settings.Themes>();
            styleBox.SelectedItem = Settings.Current.Theme;
            styleBox.SelectionChanged += (s, e) => { Settings.Current.Theme = (Settings.Themes)styleBox.SelectedItem; };

            langBox.ItemsSource = Enum.GetValues(typeof(Settings.Languages)).Cast<Settings.Languages>();
            langBox.SelectedItem = Settings.Current.Language;
            langBox.SelectionChanged += (s, e) => { Settings.Current.Language = (Settings.Languages)langBox.SelectedItem; };

            setuintBox.ItemsSource = new[] { "1", "5", "30", "60", "600", "1800", "3600" };
            setuintBox.SelectedItem = Settings.Current.AutoUpdateInterval.ToString();
            setuintBox.SetBinding(System.Windows.Controls.Primitives.Selector.SelectedItemProperty, new Binding { Source = Settings.Current, Path = new PropertyPath("AutoUpdateInterval"), Mode = BindingMode.TwoWay });

            //ELmodels.ItemsSource = Platform.Current.Models;
            CBType.ItemsSource = types.Keys;
            GShade.MouseDown += (s,e) => { OPShowHide();};
            //Data.DataContext = Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[1].Changes;

            //TryReadFromPortAsync();
        }

        public void Notified(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DGProp.ItemsSource = null;
            DGProp.ItemsSource = View.ModelToView();
            int tmp = Lmodels.SelectedIndex;
            Lmodels.ItemsSource = null;
            Lmodels.ItemsSource = Platform.Current.Models;
            Lmodels.SelectedIndex = tmp;
            if (Lmodels.SelectedItem != null)
            {
                stackscroll.Content = View.GetDashboard(Lobjects.SelectedItem as Object);
            }
        }

        public void Drag(object sender, MouseButtonEventArgs e)
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


        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
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
            //Message.Show(JsonConvert.SerializeObject(Platform.Current.Models.First().Objects.First().Properties[3]), "");
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
                    await Dispatcher.BeginInvoke(new Action(delegate
                    {
                        DGProp.ItemsSource = View.ModelToView();
                        Lmodels.ItemsSource = Platform.Current.Models;
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
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Update");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var but = new Border { Child = path };
                ((Button)sender).Content = but;
            }
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Controller.Close();
        }

        private void BAdd_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (!(button?.Tag is Object)) return;
            //if (Lobjects.SelectedItem == null) { Message.Show((string)Application.Current.Resources["Dialogid6"], (string)Application.Current.Resources["Dialogid5"]); return; }
            Lobjects.SelectedItem = button.Tag;
            new WDashbordCreate((Object) button.Tag).ShowDialog();
            Lobjects.SelectedIndex = -1;
            Lobjects.SelectedItem = button.Tag;
        }

        private async void BUpload_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
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
                var button = sender as Button;
                if (button != null) button.Content = but;
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
                var path = new Path();
                path.SetResourceReference(Path.DataProperty, "Upload");
                path.SetResourceReference(Shape.FillProperty, "MainColor");
                var but = new Border { Child = path };
                var button = sender as Button;
                if (button != null) button.Content = but;
            }
        }


        private void MainTabC_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //case
            DGProp.ItemsSource = View.ModelToView();
            
        }

        #region Editor

        const int SHAnimationTime = 300;
        bool EIsOpen;
        int editmode = -1;
        Property iop;
        Script ios;
        Object ioo;
        Model iom;
        readonly Dictionary<string, TypeCode> types = new Dictionary<string, TypeCode> { { "Boolean", TypeCode.Boolean }, { "String", TypeCode.String }, { "Double", TypeCode.Double }, { "Integer", TypeCode.Int32 } };
        Dictionary<TypeCode, int> tid = new Dictionary<TypeCode, int> { { TypeCode.Boolean, 0 }, { TypeCode.String, 1 }, { TypeCode.Double, 2 }, { TypeCode.Int32, 3 } };

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var name = TBName.Text.Trim(' ');
            if (!string.IsNullOrEmpty(name))
                switch (editmode)
                {
                    case 1:
                        Model model;
                        try
                        {
                            model = new Model(name);
                            model.Id = Platform.Current.Models.Count == 0 ? -1 : Platform.Current.Models.Min(i => i.Id) - 1;
                            //model = Network.IoTFactory.CreateModel(new Model(name));
                        }
                        catch
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        if (model == null)
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        Platform.Current.Models.Add(model);
                        break;
                    case 2:
                        Object obj;
                        try
                        {
                            obj = new Object(name, (long)(ELmodels.SelectedItem as Model).Id);
                            obj.Id = Platform.Current.Models.Count == 0 || Platform.Current.Models.SelectMany(x => x.Objects).Count() == 0 ? -1 : Platform.Current.Models.SelectMany(x => x.Objects).Min(i => i.Id) - 1;
                            //obj = Network.IoTFactory.CreateObject(new Object(name, (long)(Lmodels.SelectedItem as Model).id));
                        }
                        catch
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        if (obj == null)
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        (ELmodels.SelectedItem as Model)?.Objects.Add(obj);
                        break;
                    case 3:
                        if (CBType.SelectedItem != null)
                        {
                            TypeCode type = TypeCode.Object;
                            types.TryGetValue((CBType.SelectedItem as string), out type);
                            if (type != TypeCode.Object)
                            {
                                Property prop;
                                try
                                {
                                    prop = new Property(name, (long)(ELobjects.SelectedItem as Object).Id, (int)type, TBValue.Text);
                                    prop.Id = Platform.Current.Models.Count == 0 || Platform.Current.Models.SelectMany(x => x.Objects).Count() == 0 || Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Count() == 0 ? -1 : Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Min(i => i.Id) - 1;
                                    //prop = Network.IoTFactory.CreateProperty(new Property(name, (long)(Lobjects.SelectedItem as Object).id, (int)type, TBValue.Text));
                                }
                                catch
                                {
                                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                                }
                                if (prop == null)
                                {
                                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                                }
                                (ELobjects.SelectedItem as Object)?.Properties.Add(prop);

                            }
                        }
                        // MessageBox.Show((string)Application.Current.Resources["Errid2"]);
                        break;
                    case 4:
                        Script script;
                        try
                        {
                            script = new Script(name, (long)(ELproperties.SelectedItem as Property).Id, TBScript.Text);
                            script.Id = Platform.Current.Models.Count == 0 || Platform.Current.Models.SelectMany(x => x.Objects).Count() == 0 || Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Count() == 0 || Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).SelectMany(z => z.Scripts).Count() == 0 ? -1 : Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).SelectMany(z => z.Scripts).Min(i => i.Id) - 1;
                            //script = Network.IoTFactory.CreateScript(new Script(name, (long)(Lproperties.SelectedItem as Property).id, TBScript.Text));
                        }
                        catch
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        if (script == null)
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        (ELproperties.SelectedItem as Property)?.Scripts.Add(script);
                        break;
                    case 10:
                        if (iop != null)
                        {
                            iop.Name = name;
                            iop.Value = TBValue.Text;
                            //Network.IoTFactory.UpdateProperty(iop);//добавить проверку на 1/0, а лучше сделать другую модель одновления
                            int t = ELobjects.SelectedIndex;
                            ELobjects.SelectedIndex = -1;
                            ELobjects.SelectedItem = ELobjects.Items[t];
                        }
                        break;
                    case 11:
                        if (ios != null)
                        {
                            ios.Name = name;
                            ios.Value = TBScript.Text;
                            //Network.IoTFactory.UpdateScript(ios);
                            int t = ELproperties.SelectedIndex;
                            ELproperties.SelectedIndex = -1;
                            ELproperties.SelectedItem = ELproperties.Items[t];
                        }
                        break;
                    case 12:
                        if (ioo != null)
                        {
                            ioo.Name = name;
                            //Network.IoTFactory.UpdateObject(ioo);
                            int t = ELmodels.SelectedIndex;
                            ELmodels.SelectedIndex = -1;
                            ELmodels.SelectedItem = ELmodels.Items[t];
                        }
                        break;
                    case 13:
                        if (iom != null)
                        {
                            iom.Name = name;
                            try
                            {
                                int t = ELmodels.SelectedIndex == -1 ? 0 : ELmodels.SelectedIndex;
                                ELmodels.SelectedIndex = -1;
                                ELmodels.SelectedItem = ELmodels.Items[t];
                            }
                            catch { }
                            //Network.IoTFactory.UpdateModel(iom);
                        }
                        break;
                }
            TBName.Text = TBScript.Text = TBValue.Text = "";
            Tag = null;
            editmode = -1;
            Notified(null, null);
            OPShowHide();
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Hidden;
            labelValue.Visibility = Visibility.Hidden;
            labelScript.Visibility = Visibility.Hidden;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Hidden;
            TBScript.Visibility = Visibility.Hidden;
            CBType.Visibility = Visibility.Hidden;
            CBType.IsEnabled = false;
            editmode = 1;
            OPShowHide();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Hidden;
            labelValue.Visibility = Visibility.Hidden;
            labelScript.Visibility = Visibility.Hidden;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Hidden;
            TBScript.Visibility = Visibility.Hidden;
            CBType.Visibility = Visibility.Hidden;
            CBType.IsEnabled = false;
            editmode = 2;
            OPShowHide();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Visible;
            labelValue.Visibility = Visibility.Visible;
            labelScript.Visibility = Visibility.Hidden;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Visible;
            TBScript.Visibility = Visibility.Hidden;
            CBType.Visibility = Visibility.Visible;
            CBType.IsEnabled = true;
            editmode = 3;
            OPShowHide();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Hidden;
            labelValue.Visibility = Visibility.Hidden;
            labelScript.Visibility = Visibility.Visible;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Hidden;
            TBScript.Visibility = Visibility.Visible;
            CBType.Visibility = Visibility.Hidden;
            editmode = 4;
            OPShowHide();
        }


        private async void OPShowHide()
        {

            BSave.IsEnabled = false;
            await Task.Run(async () =>
            {
                if (EIsOpen)
                {
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        GShade.Visibility = Visibility.Hidden;
                    }));
                    for (double i = 0; i >= -290; i -= 4)
                    {
                        Thread.Sleep(SHAnimationTime * 4 / 290);
                        await Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            editOptions.Margin = new Thickness(0, 0, i, 0);
                        }));
                    }
                }
                else
                {
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        GShade.Visibility = Visibility.Visible;
                    }));
                    for (double i = -290; i <= 0; i += 4)
                    {
                        Thread.Sleep(SHAnimationTime * 4 / 290);
                        await Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            editOptions.Margin = new Thickness(0, 0, i, 0);
                        }));
                    }
                }

            });
            EIsOpen = editOptions.Margin.Right > -100;
            BSave.IsEnabled = true;
        }

        private void EditEvent(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            if ((sender as Button).Tag is Property)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Visible;
                labelScript.Visibility = Visibility.Hidden;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Property).Name;
                TBValue.Visibility = Visibility.Visible;
                TBValue.Text = ((sender as Button).Tag as Property).Value;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.SelectedItem = ((sender as Button).Tag as Property).Type;
                CBType.IsEnabled = false;
                ELproperties.SelectedItem = (sender as Button).Tag;
                editmode = 10;
                iop = ((sender as Button).Tag as Property);
                OPShowHide();
            }
            if ((sender as Button).Tag is Script)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Hidden;
                labelScript.Visibility = Visibility.Visible;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Script).Name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Visible;
                TBScript.Text = ((sender as Button).Tag as Script).Value;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                ELscripts.SelectedItem = (sender as Button).Tag;
                editmode = 11;
                ios = ((sender as Button).Tag as Script);
                OPShowHide();
            }
            if ((sender as Button).Tag is Object)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Hidden;
                labelScript.Visibility = Visibility.Hidden;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Object).Name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                ELobjects.SelectedItem = (sender as Button).Tag;
                editmode = 12;
                ioo = ((sender as Button).Tag as Object);
                OPShowHide();
            }
            if ((sender as Button).Tag is Model)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Hidden;
                labelScript.Visibility = Visibility.Hidden;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Model).Name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                ELobjects.SelectedItem = (sender as Button).Tag;
                editmode = 12;
                iom = ((sender as Button).Tag as Model);
                OPShowHide();
            }
        }
        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            if (((Button)sender).Tag is Model)
            {
                if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                {
                    //if(Network.IoTFactory.DeleteModel((sender as Button).Tag as Model))
                    Platform.Current.Models.Remove(((Button)sender).Tag as Model);
                }
            }
            if (((Button)sender).Tag is Object)
            {
                ELobjects.SelectedItem = ((Button)sender).Tag;
                if (ELmodels.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteObject((sender as Button).Tag as Object))
                        (ELmodels.SelectedItem as Model).Objects.Remove((sender as Button).Tag as Object);
                    }
            }
            if (((Button)sender).Tag is Property)
            {
                ELproperties.SelectedItem = ((Button)sender).Tag;
                if (ELobjects.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteProperty((sender as Button).Tag as Property))
                        (ELobjects.SelectedItem as Object)?.Properties.Remove(((Button)sender).Tag as Property);
                    }
            }
            if (((Button)sender).Tag is Script)
            {
                ELscripts.SelectedItem = ((Button)sender).Tag;
                if (ELproperties.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteScript((sender as Button).Tag as Script))
                        (ELproperties.SelectedItem as Property).Scripts.Remove((sender as Button).Tag as Script);
                    }
            }
            Main.GetMainWindow().Notified(null, null);
        }

        private void CopyEvent(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == null) return;
            if (((Button)sender).Tag is Model)
            {
                Model model;
                try
                {
                    model = new Model((sender as Button).Tag as Model)
                    {
                        Id = !Platform.Current.Models.Any() ? -1 : Platform.Current.Models.Min(i => i.Id) - 1
                    };
                    //model = Network.IoTFactory.CreateModel(new Model((sender as Button).Tag as Model));
                }
                catch
                {
                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                }
                if (model == null)
                {
                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                }
                Platform.Current.Models.Add(model);
            }
            if (((Button)sender).Tag is Object)
            {
                if (ELmodels.SelectedItem != null)
                {
                    Object obj;
                    try
                    {
                        obj = new Object(((Button)sender).Tag as Object)
                        {
                            Id =
                                !Platform.Current.Models.SelectMany(x => x.Objects).Any()
                                    ? -1
                                    : Platform.Current.Models.SelectMany(x => x.Objects).Min(i => i.Id) - 1
                        };
                        //obj = Network.IoTFactory.CreateObject(new Object((sender as Button).Tag as Object));
                    }
                    catch
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                    if (obj == null)
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                        (ELmodels.SelectedItem as Model)?.Objects.Add(obj);
                }
            }
            if (((Button)sender).Tag is Property)
            {
                if (ELobjects.SelectedItem != null)
                {
                    Property prop;
                    try
                    {
                        prop = new Property(((Button)sender).Tag as Property)
                        {
                            Id =
                                !Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Any()
                                    ? -1
                                    : Platform.Current.Models.SelectMany(x => x.Objects)
                                          .SelectMany(y => y.Properties)
                                          .Min(i => i.Id) - 1
                        };
                        //prop = Network.IoTFactory.CreateProperty(new Property((sender as Button).Tag as Property));
                    }
                    catch
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                    if (prop == null)
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                                (ELobjects.SelectedItem as Object)?.Properties.Add(prop);
                }
            }
            if (((Button)sender).Tag is Script)
            {
                if (ELproperties.SelectedItem != null)
                {
                    Script script;
                    try
                    {
                        script = new Script((sender as Button).Tag as Script)
                        {
                            Id =
                                !Platform.Current.Models.SelectMany(x => x.Objects)
                                    .SelectMany(y => y.Properties)
                                    .SelectMany(z => z.Scripts)
                                    .Any()
                                    ? -1
                                    : Platform.Current.Models.SelectMany(x => x.Objects)
                                          .SelectMany(y => y.Properties)
                                          .SelectMany(z => z.Scripts)
                                          .Min(i => i.Id) - 1
                        };
                        //script = Network.IoTFactory.CreateScript(new Script((sender as Button).Tag as Script));
                    }
                    catch
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]);
                        return;
                    }
                    if (script == null)
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]);
                        return;
                    }
                        (ELproperties.SelectedItem as Property)?.Scripts.Add(script);
                }
            }
        }

        #endregion


        public SerialPort CurrentPort = SerialPort.GetPortNames().FirstOrDefault()!=null?new SerialPort(SerialPort.GetPortNames().FirstOrDefault(), 115200):null;
        public volatile bool Checker = true;
        public async void TryReadFromPortAsync()
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(500);
                Checker = true;
                CurrentPort.DtrEnable = true;
                //currentPort.ReadTimeout = 2500;
                CurrentPort.Open();
                while (Checker)
                {
                    try
                    {
                        string line = CurrentPort.ReadLine();
                        string[] data = line.Split(',');

                        await Dispatcher.BeginInvoke(new Action(delegate()
                        {
                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[1].Value =
                                data[0];
                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[2].Value =
                                data[1];
                        }));
                        Thread.Sleep(500);
                        SendVentState();

                        Thread.Sleep(500);
                    }
                    catch (TimeoutException) { }
                }
                CurrentPort.Close();

            });
        }

        private void SendVentState()
        {
            try {            
            CurrentPort?.WriteLine(bool.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[0].Value) ? "ledon" : "ledoff");
            }
            catch { }
        }

        private void BReconnectClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!(bool)Message.Show((string)Application.Current.Resources["Dialogid2"], (string)Application.Current.Resources["Dialogid4"], true)) return;
            MainWindow.Instance.Show();
            _instance = null;
            Settings.Current = null;
            Close();
        }

        private void BExitClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (!(bool)Message.Show((string)Application.Current.Resources["Dialogid2"], (string)Application.Current.Resources["Dialogid4"], true)) return;
            Controller.Close();
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
