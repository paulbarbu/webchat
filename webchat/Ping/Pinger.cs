using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using webchat.Database;
using webchat.Helpers;

namespace webchat.Ping {
    public class Pinger {
        private Timer timer;

        public Pinger(int i = 60000) {
            timer = new Timer(i);
            timer.Elapsed += (object source, ElapsedEventArgs e) => { Ping(); };
            timer.Start();
        }

        public void Ping() {
            if(0 == Db.Users.Count) {
                return;
            }

            Db.Backup();

            Publisher.Publish(Resources.Strings.PingEventChannel, "ping");            
            Logger.Log(string.Format("Ping sent at {0}", DateTime.Now), "INFO");
        }
    }
}