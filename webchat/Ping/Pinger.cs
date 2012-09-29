using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using webchat.Database;
using webchat.Helpers;

namespace webchat.Ping {
    /// <summary>
    /// Class used for sending PING to the users of the application
    /// </summary>
    public class Pinger {
        /// <summary>
        /// Sets the interval of the PING and does the asynchronous job
        /// </summary>
        private Timer timer;

        /// <summary>
        /// Pinger's constructor
        /// </summary>
        /// <param name="i">The time interval between two PINGs</param>
        public Pinger(int i = 60000) {
            timer = new Timer(i);
            timer.Elapsed += (object source, ElapsedEventArgs e) => { Ping(); };
            timer.Start();
        }

        /// <summary>
        /// Send the PING by using the <see cref="MvcApplication.Pub"/> and 
        /// logging the action using <see cref="MvcApplication.Logger"/>
        /// 
        /// The backup is done by <see cref="MvcApplication.Db"/>
        /// </summary>
        public void Ping() {
            if(!MvcApplication.Db.IsPopulated()) {
                return;
            }

            MvcApplication.Db.Backup();

            MvcApplication.Pub.Publish(Resources.Internals.PingEventChannel, "ping");
            MvcApplication.Logger.Log("Ping!", "INFO");
        }
    }
}