using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webchat.Logging {
    /// <summary>
    /// An interface to define logging actions
    /// </summary>
    public interface ILogger {
        /// <summary>
        /// Overloaded method to log a message
        /// </summary>
        /// <param name="message">The string to be logged</param>
        void Log(string message);

        /// <summary>
        /// Overloaded method to log a message
        /// </summary>
        /// <param name="message">The string to be logged</param>
        /// <param name="category">The category in which the logged message belongs</param>
        void Log(string message, string category);
    }
}
