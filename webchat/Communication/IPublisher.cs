using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webchat.Communication {
    /// <summary>
    /// Defines behaviour of publisher classes
    /// </summary>
    /// <typeparam name="T">The type of the Clients property 
    /// that the class implementing this interface will hold</typeparam>
    public interface IPublisher<T> {
        /// <summary>
        /// A collection that holds the clients to whom messages will be published
        /// </summary>
        T Clients { get; }

        /// <summary>
        /// Define how the message is actually sent to the clients
        /// </summary>
        /// <param name="channel">The channel to publish messages on, this may be used for categorizing messages</param>
        /// <param name="message">The message to be sent to every client</param>
        void Publish(string channel, string message);
    }
}
