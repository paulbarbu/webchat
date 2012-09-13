using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webchat.Database {
    public static class Db {
        //TODO: lock for this
        public static readonly HashSet<string> Users = new HashSet<string>();

        //TODO: lock for this
        public static readonly ConcurrentDictionary<string, List<string>> RoomUsersList = 
            new ConcurrentDictionary<string, List<string>>();
    }
}