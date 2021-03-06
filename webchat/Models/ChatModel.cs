﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webchat.Models {
    /// <summary>
    /// Model for the <see cref="Controllers.ChatController"/>
    /// </summary>
    public class ChatModel {
        /// <summary>
        /// A copy of the database used to display the connected users on every room
        /// </summary>
        public Dictionary<string, HashSet<string>> Users { get; set; }

        /// <summary>
        /// The rooms the user connected to
        /// </summary>
        public RoomsModel ConnectedRooms { get; set; }

        /// <summary>
        /// The rooms that any user is connected to
        /// </summary>
        public RoomsModel AllRooms { get; set; }
    }
}