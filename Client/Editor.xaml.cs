using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для Editor.xaml
    /// </summary>
    public partial class Editor : Window
    {
        const int SHAnimationTime = 300;
        bool EIsOpen = false;
        int editmode = -1;
        Property iop;
        Script ios;
        Object ioo;
        Model iom;
        Dictionary<string, TypeCode> types = new Dictionary<string, TypeCode>() { { "Boolean", TypeCode.Boolean }, { "String", TypeCode.String }, { "Double", TypeCode.Double }, { "Integer", TypeCode.Int32 } };
        Dictionary<TypeCode, int> tid = new Dictionary<TypeCode, int>() { { TypeCode.Boolean, 0 }, { TypeCode.String, 1 }, { TypeCode.Double, 2 }, { TypeCode.Int32, 3 } };

        static Editor instance;
        public static Editor GetEditWindow()
        {
            if (instance == null) instance = new Editor();
            return instance;
        }
        private Editor()
        {
            InitializeComponent();
            Lmodels.ItemsSource = Snapshot.current.models;
            CBType.ItemsSource = types.Keys;
        }

        private void BBack_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (EIsOpen) OPShowHide();
                Opacity = 0.5;
                DragMove();
                Opacity = 1;
            }
            if (e.ClickCount == 2 && e.ChangedButton == MouseButton.Left)
            {
                WindowState = WindowState != WindowState.Minimized ? WindowState.Minimized : WindowState.Normal;
            }
        }

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
                            model.id = Snapshot.current.models.Count==0?-1:Snapshot.current.models.Min(i => i.id)-1;
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
                        Snapshot.current.models.Add(model);
                        try
                        {
                            int t = Lmodels.SelectedIndex == -1 ? 0 : Lmodels.SelectedIndex;
                            Lmodels.SelectedIndex = -1;
                            Lmodels.ItemsSource = Snapshot.current.models;
                            Lmodels.SelectedItem = Lmodels.Items[t];
                        }
                        catch { }
                        break;
                    case 2:
                        Object obj;
                        try
                        {
                            obj = new Object(name, (long)(Lmodels.SelectedItem as Model).id);
                            obj.id= Snapshot.current.models.SelectMany(x => x.objects).Count() == 0 ? -1 : Snapshot.current.models.SelectMany(x=>x.objects).Min(i => i.id) - 1;
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
                        (Lmodels.SelectedItem as Model)?.objects.Add(obj);
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
                                    prop = new Property(name, (long)(Lobjects.SelectedItem as Object).id, (int)type, TBValue.Text);
                                    prop.id= Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties).Count() == 0 ? -1 : Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y=>y.properties).Min(i => i.id) - 1;
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
                                (Lobjects.SelectedItem as Object)?.properties.Add(prop);

                            }
                        }
                        // MessageBox.Show((string)Application.Current.Resources["Errid2"]);
                        break;
                    case 4:
                        Script script;
                        try
                        {
                            script = new Script(name, (long)(Lproperties.SelectedItem as Property).id, TBScript.Text);
                            script.id= Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties).SelectMany(z => z.scripts).Count() == 0 ? -1 :Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties).SelectMany(z=>z.scripts).Min(i => i.id) - 1;
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
                        (Lproperties.SelectedItem as Property)?.scripts.Add(script);
                        break;
                    case 10:
                        if (iop != null)
                        {                         
                            iop.name = name;
                            iop.value = TBValue.Text;
                            //Network.IoTFactory.UpdateProperty(iop);//добавить проверку на 1/0, а лучше сделать другую модель одновления
                            int t = Lobjects.SelectedIndex;
                            Lobjects.SelectedIndex = -1;
                            Lobjects.SelectedItem = Lobjects.Items[t];
                        }
                        break;
                    case 11:
                        if (ios != null)
                        {
                            ios.name = name;
                            ios.value = TBScript.Text;
                            //Network.IoTFactory.UpdateScript(ios);
                            int t = Lproperties.SelectedIndex;
                            Lproperties.SelectedIndex = -1;
                            Lproperties.SelectedItem = Lproperties.Items[t];
                        }
                        break;
                    case 12:
                        if (ioo != null)
                        {
                            ioo.name = name;
                            //Network.IoTFactory.UpdateObject(ioo);
                            int t = Lmodels.SelectedIndex;
                            Lmodels.SelectedIndex = -1;
                            Lmodels.SelectedItem = Lmodels.Items[t];
                        }
                        break;
                    case 13:
                        if (iom != null)
                        {
                            iom.name = name;
                            try
                            {
                                int t = Lmodels.SelectedIndex==-1?0 : Lmodels.SelectedIndex;
                                Lmodels.SelectedIndex = -1;
                                Lmodels.SelectedItem = Lmodels.Items[t];
                            }
                            catch { }
                            //Network.IoTFactory.UpdateModel(iom);
                        }
                        break;
                }
            TBName.Text = TBScript.Text = TBValue.Text = "";
            Tag = null;
            editmode = -1;
            Main.GetMainWindow().Notified(null, null);
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
                    for (double i = 0; i >= -268; i -= 4)
                    {
                        Thread.Sleep(SHAnimationTime / 67);
                        await Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            editOptions.Margin = new Thickness(i, 0, 0, 0);
                        }));
                    }
                }
                else
                {
                    for (double i = -268; i <= 0; i += 4)
                    {
                        Thread.Sleep(SHAnimationTime / 67);
                        await Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            editOptions.Margin = new Thickness(i, 0, 0, 0);
                        }));
                    }
                }

            });
            EIsOpen = editOptions.Margin.Left == 0;
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
                TBName.Text = ((sender as Button).Tag as Property).name;
                TBValue.Visibility = Visibility.Visible;
                TBValue.Text = ((sender as Button).Tag as Property).value;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.SelectedItem = ((sender as Button).Tag as Property).type;
                CBType.IsEnabled = false;
                Lproperties.SelectedItem = (sender as Button).Tag;
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
                TBName.Text = ((sender as Button).Tag as Script).name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Visible;
                TBScript.Text = ((sender as Button).Tag as Script).value;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                Lscripts.SelectedItem = (sender as Button).Tag;
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
                TBName.Text = ((sender as Button).Tag as Object).name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                Lobjects.SelectedItem = (sender as Button).Tag;
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
                TBName.Text = ((sender as Button).Tag as Model).name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                Lobjects.SelectedItem = (sender as Button).Tag;
                editmode = 12;
                iom = ((sender as Button).Tag as Model);
                OPShowHide();
            }
        }
        private void DeleteEvent(object sender, RoutedEventArgs e)
        {          
            if ((sender as Button).Tag is Model)
            {
                if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                { 
                    //if(Network.IoTFactory.DeleteModel((sender as Button).Tag as Model))
                    Snapshot.current.models.Remove((sender as Button).Tag as Model);
                }
            }
            if ((sender as Button).Tag is Object)
            {
                Lobjects.SelectedItem = (sender as Button).Tag;
                if (Lmodels.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteObject((sender as Button).Tag as Object))
                            (Lmodels.SelectedItem as Model).objects.Remove((sender as Button).Tag as Object);
                    }
            }
            if ((sender as Button).Tag is Property)
            {
                Lproperties.SelectedItem = (sender as Button).Tag;
                if (Lobjects.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteProperty((sender as Button).Tag as Property))
                            (Lobjects.SelectedItem as Object).properties.Remove((sender as Button).Tag as Property);
                    }
            }
            if ((sender as Button).Tag is Script)
            {
                Lscripts.SelectedItem = (sender as Button).Tag;
                if (Lproperties.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteScript((sender as Button).Tag as Script))
                            (Lproperties.SelectedItem as Property).scripts.Remove((sender as Button).Tag as Script);
                    }
            }
            Main.GetMainWindow().Notified(null, null);
        }

        private void CopyEvent(object sender, RoutedEventArgs e)
        { 
            
            if ((sender as Button).Tag is Model)
            {
                Model model;
                try
                {
                    model = new Model((sender as Button).Tag as Model);
                    model.id= Snapshot.current.models.Count == 0 ? -1 : Snapshot.current.models.Min(i => i.id) - 1;
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
                Snapshot.current.models.Add(model);
            }
            if ((sender as Button).Tag is Object)
            {
                if (Lmodels.SelectedItem != null)
                {
                    Object obj;
                    try
                    {
                        obj = new Object((sender as Button).Tag as Object);
                        obj.id = Snapshot.current.models.SelectMany(x => x.objects).Count() == 0 ? -1 : Snapshot.current.models.SelectMany(x=>x.objects).Min(i => i.id) - 1;
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
                        (Lmodels.SelectedItem as Model)?.objects.Add(obj);
                }
            }
            if ((sender as Button).Tag is Property)
            {
                if (Lobjects.SelectedItem != null)
                {
                    Property prop;
                    try
                    {
                        prop = new Property((sender as Button).Tag as Property);
                        prop.id= Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties).Count() == 0 ? -1 : Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y=>y.properties).Min(i => i.id) - 1;
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
                                (Lobjects.SelectedItem as Object).properties.Add(prop);
                }
            }
            if ((sender as Button).Tag is Script)
            {
                if (Lproperties.SelectedItem != null)
                {
                    Script script;
                    try
                    {
                        script = new Script((sender as Button).Tag as Script);
                        script.id = Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties).SelectMany(z => z.scripts).Count() == 0 ? -1 : Snapshot.current.models.SelectMany(x => x.objects).SelectMany(y => y.properties).SelectMany(z=>z.scripts).Min(i => i.id) - 1;
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
                        (Lproperties.SelectedItem as Property)?.scripts.Add(script);
                }
            }
        }
    }
}
