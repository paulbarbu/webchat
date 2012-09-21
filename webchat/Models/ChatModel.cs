using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webchat.Models {
    public class ChatModel {
        public Dictionary<string, HashSet<string>> Users { get; set; }
        public List<string> Rooms { get; set; }
    }
}