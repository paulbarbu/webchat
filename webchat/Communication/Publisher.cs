using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace webchat.Communication {
    public class Publisher : IPublisher<ConcurrentQueue<StreamWriter>> {
        private const string eventPattern = "event: {0}\ndata: {1}\n\n";

        private readonly ConcurrentQueue<StreamWriter> clients = new ConcurrentQueue<StreamWriter>();

        public void Publish(string channel, string message) {
            //no need of locking because I use a ConcurrentQueue that I don't modify
            foreach(var subscriber in clients) {
                subscriber.Write(eventPattern, channel, message);
                subscriber.Flush();
            }
        }

        public ConcurrentQueue<StreamWriter> Clients {
            get { return clients; }
        }
    }
}