using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConsoleERP_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string HomeAddress { get; set; }

        public MainWindow()
        {
            InitializeComponent();


            // get address
            if (File.Exists("info.txt"))
            {
                string[] settings = File.ReadAllLines("info.txt");
                HomeAddress = settings[0];
                AddTab();

            } else
            {
                MessageBox.Show("Could not find info file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Logs to log.txt
        /// </summary>
        /// <param name="lines"></param>
        public static void Logger(String lines)
        {

            // Write the string to a file.append mode is enabled so that the log
            // lines get appended to  test.txt than wiping content and writing the log

            using (System.IO.StreamWriter file = new System.IO.StreamWriter("log.txt", true)) 
                file.WriteLine(lines);            

        }


        private void AddTab()
        {
            // make a new Tab for the browser
            TabItem item = new TabItem() { Header = "ERP" };

            Grid panel = new Grid();            
            ProgressBar pb = new ProgressBar() { };

            // the webbrowser to host the page
            ChromiumWebBrowser wbrowser = new ChromiumWebBrowser();
            wbrowser.TitleChanged += (o, f) => 
            {
                item.Header = wbrowser.Title;
                EvaluateForwardBackward();
            };
            wbrowser.LoadingStateChanged += (o, e) => {                
                Dispatcher.Invoke(async () =>
                {
                    await Task.Delay(200);
                    pb.IsIndeterminate = wbrowser.IsLoading;
                    EvaluateForwardBackward();
                });                
            };
            if (tabControl.Items.Count == 0)
                wbrowser.Address = HomeAddress;
            else
                wbrowser.Address = HomeAddress + "/desk";

            RowDefinition r1 = new RowDefinition();
            RowDefinition r2 = new RowDefinition() { Height = new GridLength(10) };
            panel.RowDefinitions.Add(r1);
            panel.RowDefinitions.Add(r2);

            Grid.SetRow(panel, 0); Grid.SetRow(pb, 1);

            panel.Children.Add(wbrowser);
            panel.Children.Add(pb);

            item.Content = panel;

            tabControl.Items.Add(item);
        }

        /// <summary>
        /// Event fired when addTab button is cliecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void add_tab_Click(object sender, RoutedEventArgs e)
        {
            AddTab();
        }


        /// <summary>
        /// Closes active tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void close_tab_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex != -1)
            {
                ChromiumWebBrowser wbrowser = getActiveTabBrowser();
                wbrowser.Dispose();

                tabControl.Items.Remove(tabControl.SelectedItem);
            }
        }

        /// <summary>
        /// Releases all webbrowsers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (TabItem t in tabControl.Items)
            {
                getActiveTabBrowser(t).Dispose();
            }
        }

        private void nav_refresh_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedItem == null) return;


            ChromiumWebBrowser wbrowser = getActiveTabBrowser();
            wbrowser.Load(wbrowser.Address);
        }

        /// <summary>
        /// Handles forward and backward
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navigate_tofo(object sender, RoutedEventArgs e)
        {
            CefSharp.IBrowser wbrowser = getActiveTabBrowser().GetBrowser();

            if (sender == nav_back && wbrowser.CanGoBack)
                wbrowser.GoBack();
            else if (sender == nav_for && wbrowser.CanGoForward)
                wbrowser.GoForward();
        }

        private void EvaluateForwardBackward()
        {
            ChromiumWebBrowser wbrowser = getActiveTabBrowser();
            if (wbrowser == null)
            {
                nav_back.IsEnabled = false;
                nav_for.IsEnabled = false;
                return;
            }

            nav_back.IsEnabled = wbrowser.CanGoBack;
            nav_for.IsEnabled = wbrowser.CanGoForward;
        }

        /// <summary>
        /// Gets te ChromiumWebBrowser for active tab
        /// </summary>
        /// <param name="t">Specify to get browser from particular tab</param>
        /// <returns></returns>
        private ChromiumWebBrowser getActiveTabBrowser(TabItem t = null)
        {
            if (tabControl.SelectedItem == null)
                return null;
                     
            return ((Grid)(t ?? tabControl.SelectedItem as TabItem).Content).Children[0] as ChromiumWebBrowser;
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EvaluateForwardBackward();
        }
    }



}
