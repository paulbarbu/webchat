using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
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

        public void NotifyJoin(){
            using(var redis = new RedisClient()) {
                var r = redis.As<List<string>>();
                var room_user_list = r.GetHash<string>(Resources.Strings.RoomUserListKey);

                redis.PublishMessage(Resources.Strings.UsersEventChannel,
                    JsonConvert.SerializeObject(room_user_list));
            }
        }
    }
}