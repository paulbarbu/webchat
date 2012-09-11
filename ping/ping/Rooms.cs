using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ping {
    public class Rooms : List<string> {
        public Rooms(string nick) {
            Update(nick);
        }

        //TODO: docs
        //Because this is a List<string> I already have the list of
        //rooms where to add the user to or where to delete him from

        public void DelUser(string nick) {
            List<string> user_list;

            using (var r = new RedisClient().As<List<string>>()) {
                var room_user_list = r.GetHash<string>("room_user_list");

                foreach (var room in this) {
                    bool room_exists = room_user_list.TryGetValue(room, out user_list);

                    if (room_exists) {
                        user_list.Remove(nick);

                        if (0 == user_list.Count) {
                            r.RemoveEntryFromHash(room_user_list, room);
                        }
                        else {
                            r.SetEntryInHash(room_user_list, room, user_list);
                        }
                    }
                }
            }

            DelUserFromGlobalList(nick);
        }

        private void DelUserFromGlobalList(string nick) {
            using (var redis = new RedisClient().As<string>()) {
                var global_user_list = redis.Sets["global_user_list"];
                redis.RemoveItemFromSet(global_user_list, nick);
            }
        }

        public void Update(string nick = null) {
            using (var r = new RedisClient().As<List<string>>()) {
                var room_user_list = r.GetHash<string>("room_user_list");

                this.Clear();

                if (null == nick) {
                    this.AddRange(r.GetHashKeys<string>(room_user_list));
                }
                else {
                    var users = from n in room_user_list where room_user_list[n.Key].Contains(nick) select n.Key;

                    this.AddRange(users.ToList());
                }
            }
        }
    }
}