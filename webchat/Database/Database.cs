using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using webchat.Helpers;

namespace webchat.Database {
    public class Db : IDatabase {
        private readonly HashSet<string> Users = new HashSet<string>();

        private readonly ConcurrentDictionary<string, HashSet<string>> RoomUsersList = 
            new ConcurrentDictionary<string, HashSet<string>>();

        private readonly ConcurrentDictionary<string, List<string>> BackupRoomUsersList =
            new ConcurrentDictionary<string, List<string>>();

        private IPublisher<ConcurrentQueue<StreamWriter>> Pub;

        public Db(IPublisher<ConcurrentQueue<StreamWriter>> p) {
            Pub = p;
        }

        public void AddUser(IEnumerable<string> rooms, string nick) {
            foreach(var room in rooms) {
                RoomUsersList.AddOrUpdate(room,
                    (arg) => {
                        return new HashSet<string> { nick };
                    },
                    (key, val) => {
                        val.Add(nick);
                        return val;
                    }
                );
            }

            AddUserToGlobalList(nick);

            Pub.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(GetUsers()));
        }

        private void AddUserToGlobalList(string nick) {
            Users.Add(nick);
        }
        
        public void DelUser(IEnumerable<string> rooms, string nick) {
            HashSet<string> user_list;

            foreach(var room in rooms) {
                bool room_exists = RoomUsersList.TryGetValue(room, out user_list);

                if(room_exists) {
                    user_list.Remove(nick);

                    if(0 == user_list.Count) {
                        if(!RoomUsersList.TryRemove(room, out user_list)) {
                            Logger.Log(string.Format("Deleting key {0} from Db.RoomUsersList failed, giving up!", room),
                                "ERROR");
                        }
                    }
                    else {
                        RoomUsersList.AddOrUpdate(room, user_list, (key, val) => user_list);
                    }
                }
            }

            Pub.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(GetUsers()));
        }

        public void DelUserFromGlobalList(string nick) {
            Users.Remove(nick);
        }

        public Dictionary<string, HashSet<string>> GetUsers() {
            return RoomUsersList.ToDictionary(k => k.Key, k => k.Value);
        }

        public List<string> GetRooms() {
            return RoomUsersList.Keys.ToList();
        }

        public List<string> GetBackupRooms(string nick) {
            return BackupRoomUsersList[nick];
        }

        public List<string> GetRooms(string nick) {
            var users = from n in RoomUsersList where RoomUsersList[n.Key].Contains(nick) select n.Key;

            return users.ToList();
        }

        public void Backup() {
            foreach(var user in Users.ToList()) {
                List<string> rooms = GetRooms(user);

                if(BackupRoomUsersList.ContainsKey(user)){
                    List<string> t;
                    if(!BackupRoomUsersList.TryRemove(user, out t)) {
                        Logger.Log(string.Format("backup cleanup for {0} failed, giving up!", user), "ERROR");
                    }
                }

                if(BackupRoomUsersList.TryAdd(user, rooms)) {
                    DelUser(rooms, user);
                }
                else {
                    Logger.Log(string.Format("Backup for {0} failed, giving up!", user), "ERROR");
                }
            }
        }

        public bool IsPopulated() {
            return Users.Count > 0;
        }

        public bool IsUser(string nick) {
            return Users.Contains(nick);
        }
    }
}