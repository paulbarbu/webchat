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
        private static readonly ConcurrentDictionary<string, HashSet<string>> RoomUsersList = 
            new ConcurrentDictionary<string, HashSet<string>>();

        public static void AddUser(IEnumerable<string> rooms, string nick) {

            //TODO: lock 
            foreach(var room in rooms) {
                RoomUsersList.AddOrUpdate(room, new HashSet<string> { nick }, (key, val) => {
                    val.Add(nick);

                    return val;
                });
            }

            AddUserToGlobalList(nick);
        }

        private static void AddUserToGlobalList(string nick) {
            //TODO: lock
            Users.Add(nick);
        }
        
        public static void DelUser(IEnumerable<string> rooms, string nick) {
            HashSet<string> user_list;
            //TODO: lock
            foreach(var room in rooms) {
                bool room_exists = RoomUsersList.TryGetValue(room, out user_list);

                if(room_exists) {
                    user_list.Remove(nick);

                    if(0 == user_list.Count) {
                        //TODO: check retval
                        RoomUsersList.TryRemove(room, out user_list);
                    }
                    else {
                        RoomUsersList.AddOrUpdate(room, user_list, (key, val) => user_list);
                    }
                }
            }

            DelUserFromGlobalList(nick);
        }

        private static void DelUserFromGlobalList(string nick) {
            //TODO: lock
            Users.Remove(nick);
        }

        public static Dictionary<string, HashSet<string>> GetUsers() {
            return RoomUsersList.ToDictionary(k => k.Key, k => k.Value);
        }

        public static List<string> GetRooms() {
            return RoomUsersList.Keys.ToList();
        }

        public static List<string> GetRooms(string nick) {
            var users = from n in RoomUsersList where RoomUsersList[n.Key].Contains(nick) select n.Key;

            return users.ToList();
        }
    }
}