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

        //TODO: join just a specific set of rooms? maybe I need this for the chat page
        public void AddUser(string nick) {

            List<string> user_list = new List<string>();
            //TODO: this now works but it's SO messy
            using(var redis = new RedisClient("localhost")) {

                Thread.Sleep(5000); //TODO: remove me
                
                foreach (var room in this) {
                    string current_users = redis.GetValueFromHash("users", room);

                    if(null == current_users) { //TODO: properly inspect and check this
                        user_list = new List<string> { nick };
                    }
                    else {
                        user_list = JsonConvert.DeserializeObject<List<string>>(current_users);
                        user_list.Add(nick);

                        user_list = user_list.Distinct().ToList();
                    }

                    redis.SetEntryInHash("users", room, JsonConvert.SerializeObject(user_list));
	            }
            }
        }
    }
}