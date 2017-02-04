using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleERP_Client
{
    public class ChromiumEventObj
    {

        public IBrowser browser { get; set; }

        public string Script { get; set; }

        public Func<bool> AddressCheck { get; set; }

        public Action<string> EventMethod { get; set; } 

        public ChromiumEventObj()
        {
            
        }

        public void OnFrameLoaded(object sender, LoadingStateChangedEventArgs e)
        {
            
            if (!e.IsLoading && AddressCheck())
            {
                e.Browser.MainFrame.ExecuteJavaScriptAsync(Script);
            }
        }


        public void Event(string s)
        {
            EventMethod(s);
        }
    }
}
