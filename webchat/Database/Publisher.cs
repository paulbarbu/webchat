using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace webchat.Database {
    public static class Publisher {
        private delegate void Writer(string data);
        private static string eventPattern = "event: {0}\ndata: {1}\n\n";

        public static readonly ConcurrentQueue<StreamWriter> clients = new ConcurrentQueue<StreamWriter>();

        public static void Publish(string channel, string message) {
            foreach(var subscriber in clients) {
                subscriber.Write(eventPattern, channel, message);
                subscriber.Flush();
            }
        }
    }
}