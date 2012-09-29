using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace webchat.Logging {
    /// <summary>
    /// Concrete implementation of ILogger
    /// </summary>
    public class Logger : ILogger{
        /// <summary>
        /// Overloaded method to log a message to <see cref="Trace"/>
        /// </summary>
        /// <param name="message">The string to be logged</param>
        public void Log(string message) {
            Trace.WriteLine(
                string.Format("{0} - {1}", 
                    DateTime.Now.ToString(Resources.Internals.DateTimeFormat),
                    message)
            );
        }

        /// <summary>
        /// Overloaded method to log a message to <see cref="Trace"/>
        /// </summary>
        /// <param name="message">The string to be logged</param>
        /// <param name="category">The category in which the logged message belongs</param>
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