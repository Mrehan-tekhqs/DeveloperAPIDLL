using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Net;

namespace EEPM
{
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public class SourceKeyChecker
    {
        private readonly FileInfo lastCheckedFilePath = new FileInfo(Path.Combine(Path.Combine(Environment.SpecialFolder.LocalApplicationData.ToString(), "eBizCharge"), "session"));
        private string gatewayUrl = "https://payments.eBizCharge.com/in/process.php";
        private const string action = "poscheckintegrationserver";
        private const string user = "user_1409";
        private const string software = "NAV";
        private readonly ICheckedTracker tracker = new CheckedTracker();

        public SourceKeyChecker(string gatewayUrl = null, ICheckedTracker tracker = null/* TODO Change to default(_) if this is not a reference type */)
        {
            this.gatewayUrl = gatewayUrl == null ? this.gatewayUrl : gatewayUrl;
            this.tracker = tracker == null ? this.tracker : tracker;
        }

        public bool CheckSourceKey(string sourceKey)
        {
            if (tracker.LastCheckedWithin(TimeSpan.FromDays(1)))
                return true;
            var request = (HttpWebRequest)WebRequest.Create(string.Format("{0}?action={1}&user={2}&software={3}&sourcekey={4}", gatewayUrl, action, user, software, sourceKey));
            request.ContentType = "text/html;charset=\"utf-8\"";
            request.Accept = "text/html";
            request.Method = "GET";
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            lastResponseBackingField = reader.ReadToEnd();
                            if (!lastResponseBackingField.ToLower().Contains("legituser"))
                                return false;
                            tracker.MarkChecked();
                            return true;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                return false;
            }
        }

        private string lastResponseBackingField;
        public string LastResponse
        {
            get
            {
                return lastResponseBackingField;
            }
        }
    }
}
