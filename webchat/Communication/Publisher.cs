using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace webchat.Communication {
    /// <summary>
    /// Concrete implementation of IPublisher
    /// </summary>
    public class Publisher : IPublisher<ConcurrentQueue<StreamWriter>> {
        /// <summary>
        /// The pattern used for publishing messages
        /// </summary>
        private const string eventPattern = "event: {0}\ndata: {1}\n\n";

        /// <summary>
        /// All the subscribers are stored here
        /// </summary>
        private readonly ConcurrentQueue<StreamWriter> clients = new ConcurrentQueue<StreamWriter>();

        /// <summary>
        /// Publish a certain message on a certain channel to all clients stored in <see cref="clients"/>
        /// </summary>
        /// <param name="channel">The channel to publish messages on, this may be used for categorizing messages</param>
        /// <param name="message">The message to be sent to every client</param>
        public void Publish(string channel, string message) {
            //no need of locking because I use a ConcurrentQueue that I don't modify
            foreach(var subscriber in clients) {
                subscriber.Write(eventPattern, channel, message);
                subscriber.Flush();
            }
        }

        /// <summary>
        /// The access to the private <see cref="clients"/> attribute is made through this getter
        /// </summary>
        public ConcurrentQueue<StreamWriter> Clients {
            get { return clients; }
        }
    }
}