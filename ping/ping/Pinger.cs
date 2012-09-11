using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using ServiceStack.Redis;

namespace Ping {
    class Pinger {
        private Timer timer;

        public Pinger(int interval = 30000) {
            timer = new Timer(interval);
            timer.Elapsed += (object source, ElapsedEventArgs e) => { Ping(); };
            timer.Start();
        }

        public void Ping() {
            try {
                List<string> users = GetUsers();
                if (0 == users.Count) {
                    return;
                }

                BackupRooms(users);

                using (var r = new RedisClient()) {
                    r.PublishMessage("webchat.ping", "ping");
                }

                Log(string.Format("Ping sent at {0}", DateTime.Now));
            }
            catch (RedisException) {
                Log("Please start the redis server!");
            }
        }

        private void Log(string msg) {
            //This could be imporved by Aggregation and Inversion of Control (an Interface)
            // but that would be overhead for this small program
            Console.WriteLine(msg);
        }

        private List<string> GetUsers(){
            using(var redis = new RedisClient().As<string>()) {
                var global_user_list = redis.Sets["global_user_list"];

                return global_user_list.ToList();
            }
        }

        private void BackupRooms(List<string> users) {
            using (var r = new RedisClient().As<string>()) {
                /**
                 * For every user on the chat get his current rooms and store them
                 * in a set, the set's key is the user's nick
                 */
                foreach (var user in users) {
                    var user_rooms = r.Sets[user];

                    Rooms rooms = new Rooms(user);

                    foreach (var room in rooms) {
                        r.AddItemToSet(user_rooms, room);
                    }

                    rooms.DelUser(user);
                }
            }
        }
    }
}
