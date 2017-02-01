using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
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

        private void AddTab()
        {
            // make a new Tab for the browser
            TabItem item = new TabItem() { Header = "ERP" };

            // the webbrowser to host the page
            ChromiumWebBrowser wbrowser = new ChromiumWebBrowser();
            wbrowser.TitleChanged += (o, f) => { item.Header = wbrowser.Title; };
            wbrowser.Address = HomeAddress;
            item.Content = wbrowser;

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
                ChromiumWebBrowser wbrowser = ((TabItem)tabControl.SelectedItem).Content as ChromiumWebBrowser;
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
                ((ChromiumWebBrowser)t.Content).Dispose();
            }
        }

        private void nav_refresh_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedItem == null) return;


            ChromiumWebBrowser wbrowser = ((ChromiumWebBrowser)(tabControl.SelectedItem as TabItem).Content);
            wbrowser.Load(wbrowser.Address);
        }
    }



}
