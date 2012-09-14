using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webchat.Database {
    public static class Locker {
        public static readonly Object locker = new Object();
    }
}