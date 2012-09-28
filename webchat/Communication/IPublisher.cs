using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webchat.Communication {
    public interface IPublisher<T> {
        T Clients { get; }
        void Publish(string channel, string message);
    }
}
