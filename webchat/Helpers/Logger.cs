using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace webchat.Helpers {
    public class Logger : ILogger{
        public void Log(string message) {
            Trace.WriteLine(
                string.Format("{0} - {1}", 
                    DateTime.Now.ToString(Resources.Internals.DateTimeFormat),
                    message)
            );
        }

        public void Log(string message, string category) {
            Trace.WriteLine(
                string.Format("{0} - {1}",
                    DateTime.Now.ToString(Resources.Internals.DateTimeFormat),
                    message),
                category
            );
        }
    }
}