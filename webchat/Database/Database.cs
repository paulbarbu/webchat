using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using webchat.Communication;
using webchat.Logging;

namespace webchat.Database {
    /// <summary>
    /// A concrete implementation of <see cref="IDatabase"/>
    /// </summary>
    public class Db : IDatabase {
        /// <summary>
        /// The global user list, it;s global because it doesn't group users by room
        /// </summary>
        private readonly HashSet<string> Users = new HashSet<string>();

        /// <summary>
        /// This is, in fact, the database where data is stored
        /// </summary>
        private readonly ConcurrentDictionary<string, HashSet<string>> RoomUsersList = 
            new ConcurrentDictionary<string, HashSet<string>>();

        /// <summary>
        /// The backup used during PINGing the users
        /// </summary>
        private readonly ConcurrentDictionary<string, List<string>> BackupRoomUsersList =
            new ConcurrentDictionary<string, List<string>>();

        /// <summary>
        /// An IPublisher object used to notify clients of modifications
        /// </summary>
        private IPublisher<ConcurrentQueue<StreamWriter>> Pub;
        
        /// <summary>
        /// An ILogger used to log errors
        /// </summary>
        private ILogger Logger;

        /// <summary>
        /// Lock object used for the thread safety of the database
        /// </summary>
        private readonly object roomUserListLock = new object();

        /// <summary>
        /// Lock object used for the thread safety of the global user list
        /// </summary>
        private readonly object usersLock = new object();

        /// <summary>
        /// Lock object used for thread safety of the backup
        /// </summary>
        private readonly object backupLock = new object();

        /// <summary>
        /// The Database's constructor
        /// </summary>
        /// <param name="p">A concrete implementation of an <see cref="Communication.IPublisher<T>"/></param>
        /// <param name="l">A concrete implementation of an <see cref="Logging.ILogger"/></param>
        public Db(IPublisher<ConcurrentQueue<StreamWriter>> p, ILogger l) {
            Pub = p;
            Logger = l;
        }

        /// <summary>
        /// Add a user to the database
        /// </summary>
        /// <param name="rooms">Which rooms the user joined</param>
        /// <param name="nick">The user's nickname</param>
        public void AddUser(IEnumerable<string> rooms, string nick) {
            lock(roomUserListLock) {
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
            }

            AddUserToGlobalList(nick);

            Pub.Publish(Resources.Internals.UsersEventChannel,
                JsonConvert.SerializeObject(GetUsers()));
        }

        /// <summary>
        /// Add a user tyo <see cref="Users"/>
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        private void AddUserToGlobalList(string nick) {
            lock(usersLock) Users.Add(nick);
        }
        

        /// <summary>
        /// Delete a user from the database in case he leaves
        /// </summary>
        /// <param name="rooms">The rooms the user left</param>
        /// <param name="nick">The user's nickname</param>
        public void DelUser(IEnumerable<string> rooms, string nick) {
            HashSet<string> user_list;

            lock(roomUserListLock) {
                foreach(var room in rooms) {
                    bool room_exists = RoomUsersList.TryGetValue(room, out user_list);

                    if(room_exists) {
                        user_list.Remove(nick);

                        if(0 == user_list.Count) {
                            if(!RoomUsersList.TryRemove(room, out user_list)) {
                                Logger.Log(
                                    string.Format("Deleting key {0} from Db.RoomUsersList failed, giving up!", room),
                                    "ERROR"
                                );
                            }
                        }
                        else {
                            RoomUsersList.AddOrUpdate(room, user_list, (key, val) => user_list);
                        }
                    }
                }
            }

            Pub.Publish(Resources.Internals.UsersEventChannel,
                JsonConvert.SerializeObject(GetUsers()));
        }

        /// <summary>
        /// Delete a user form the global list
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        public void DelUserFromGlobalList(string nick) {
            lock(usersLock) Users.Remove(nick);
        }

        /// <summary>
        /// Get the users currently connected
        /// </summary>
        /// <returns>Returns a Dictionary&lt;string, HashSet&lt;string&gt;&gt;
        /// of rooms as keys and users as values for the HastSet&lt;string&gt;</returns>
        public Dictionary<string, HashSet<string>> GetUsers() {
            lock(roomUserListLock) return RoomUsersList.ToDictionary(k => k.Key, k => k.Value);
        }

        /// <summary>
        /// Get all the rooms currently populated
        /// </summary>
        /// <returns>Returns a List&lt;string&gt; of rooms</returns>
        public List<string> GetRooms() {
            lock(roomUserListLock) return RoomUsersList.Keys.ToList();
        }

        /// <summary>
        /// Get all the rooms currently joined by a user
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        /// <returns>Returns a List&lt;string&gt; of rooms</returns> 
        public List<string> GetRooms(string nick) {
            lock(roomUserListLock) {
                var users = from n in RoomUsersList where RoomUsersList[n.Key].Contains(nick) select n.Key;

                return users.ToList();
            }
        }

        /// <summary>
        /// Get the rooms the user was connected to before the PING
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        /// <returns>Returns a List&lt;string&gt; of rooms</returns>
        public List<string> GetBackupRooms(string nick) {
            lock(backupLock) return BackupRoomUsersList[nick];
        }
        
        /// <summary>
        /// Do a backup of the database before sending a PING
        /// </summary>
        public void Backup() {
            lock(backupLock){
                foreach(var user in Users.ToList()) {
                    List<string> rooms = GetRooms(user);

                    if(BackupRoomUsersList.ContainsKey(user)){
                        List<string> t;
                        if(!BackupRoomUsersList.TryRemove(user, out t)) {
                            Logger.Log(string.Format("backup cleanup for {0} failed, giving up!", user),
                                "ERROR");
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
        }

        /// <summary>
        /// Check if there are users connected
        /// </summary>
        /// <returns>Returns true if there is at least one user, else returns false</returns>
        public bool IsPopulated() {
            lock(usersLock) return Users.Count > 0;
        }

        /// <summary>
        /// Check if a user is connected on at least a room
        /// </summary>
        /// <param name="nick">The user's nickname</param>
        /// <returns>Return true if the user is connected, else false</returns>
        public bool IsUser(string nick) {
            lock(usersLock) return Users.Contains(nick);
        }
    }
}