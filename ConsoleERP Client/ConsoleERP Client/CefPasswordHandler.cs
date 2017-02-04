using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ConsoleERP_Client
{
    /// <summary>
    /// A class that will handle password saving and loading
    /// </summary>
    public class CefFrappeLoginHandler
    {
        public ChromiumWebBrowser Browser { get; set; }
        
        public Dispatcher Dispatcher { get; set; }


        private string[] loginDetails = null;

        public CefFrappeLoginHandler(ChromiumWebBrowser browser, Dispatcher dispatcher)
        {
            this.Browser = browser;
            Dispatcher = dispatcher;
            Browser.RegisterJsObject("passHandler", this);

            Browser.FrameLoadEnd += Browser_FrameLoadEnd;

            if (File.Exists("login.txt"))
            {
                loginDetails = File.ReadAllLines("login.txt");
            }
        }

        private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            if (e == null || !e.Frame.IsMain)
            {                
                return;
            }

            

            if (!e.Url.Contains("login"))
                return;

            if (loginDetails != null)
            {
                // inject saved credentials
                e.Frame.ExecuteJavaScriptAsync(string.Format("$('#login_email').val('{0}'); $('#login_password').val('{1}');", loginDetails));
            }

            e.Frame.ExecuteJavaScriptAsync(@"$('.form-login').submit(function(){ 
                passHandler.formSubmit($('#login_email').val(), $('#login_password').val());
            });");       
        }


        public void formSubmit(string username, string password)
        {
            File.WriteAllLines("login.txt", new string[] { username, password });
        }
    }
}
