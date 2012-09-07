using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace webchat.Models {
    [ModelBinder(typeof(RoomsModelBinder))]
    public class Rooms : List<string> {
        public Rooms(string[] rooms) {
            this.AddRange(rooms);
        }
        
        public Rooms(string nick) {
            Update(nick);
        }

        public void AddUser(string nick) {
            List<string> user_list;

            using(var r = new RedisClient().As<List<string>>()) {
                var room_user_list = r.GetHash<string>(Resources.Strings.RoomUserListKey);
                
                foreach (var room in this) {
                    bool room_exists = room_user_list.TryGetValue(room, out user_list); 
                    
                    if(!room_exists) {
                        user_list = new List<string> { nick };
                    }
                    else {
                        user_list.Add(nick);

                        user_list = user_list.Distinct().ToList();
                    }

                    r.SetEntryInHash(room_user_list, room, user_list);
	            }
            }
        }

        public Dictionary<string, List<string>> GetUsers() {
            using(var r = new RedisClient().As<List<string>>()) {
                var room_user_list = r.GetHash<string>(Resources.Strings.RoomUserListKey);

                return r.GetAllEntriesFromHash<string>(room_user_list);
            }
        }

        public void NotifyJoin(){
            using(var redis = new RedisClient()) {
                redis.PublishMessage(Resources.Strings.UsersEventChannel,
                    JsonConvert.SerializeObject(GetUsers()));
            }
        }

        public void Update(string nick = null) {
            using(var r = new RedisClient().As<List<string>>()) {
                var room_user_list = r.GetHash<string>(Resources.Strings.RoomUserListKey);

                this.Clear();
                                
                if(null == nick){
                    this.AddRange(r.GetHashKeys<string>(room_user_list));
                }
                else{
                    var users = from n in room_user_list where room_user_list[n.Key].Contains(nick) select n.Key;

                    this.AddRange(users.ToList());
                }
            }
        }
    }
}