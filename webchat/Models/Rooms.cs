using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using webchat.Database;

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

            //TODO: lock 
            foreach (var room in this) {
                Db.RoomUsersList.AddOrUpdate(room, new List<string> { nick }, (key, val) => {
                    val.Add(nick);

                    return val.Distinct().ToList();
                });
	        }

            AddUserToGlobalList(nick);
        }

        private void AddUserToGlobalList(string nick) {
            //TODO: lock
            Db.Users.Add(nick);
        }

        //TODO: docs
        //Because this is a List<string> I already have the list of
        //rooms where to add the user to or where to delete him from

        public void DelUser(string nick) {
            List<string> user_list;
            //TODO: lock
            foreach(var room in this) {
                bool room_exists = Db.RoomUsersList.TryGetValue(room, out user_list);

                if(room_exists) {
                    user_list.Remove(nick);

                    if(0 == user_list.Count) {
                        //TODO: check retval
                        Db.RoomUsersList.TryRemove(room, out user_list);
                    }
                    else {
                        Db.RoomUsersList.AddOrUpdate(room, user_list, (key, val) => user_list);
                    }
                }
            }

            DelUserFromGlobalList(nick);
        }

        private void DelUserFromGlobalList(string nick) {
            //TODO: lock
            Db.Users.Remove(nick);
        }

        public Dictionary<string, List<string>> GetUsers() {
            return Db.RoomUsersList.ToDictionary(k => k.Key, k => k.Value);
        }

        public void Update(string nick = null) {
            this.Clear();
                                
            if(null == nick){
                this.AddRange(Db.RoomUsersList.Keys);
            }
            else{
                var users = from n in Db.RoomUsersList where Db.RoomUsersList[n.Key].Contains(nick) select n.Key;

                this.AddRange(users.ToList());
            }
        }
    }
}