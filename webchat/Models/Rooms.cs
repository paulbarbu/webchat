using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace webchat.Models {
    [ModelBinder(typeof(RoomsModelBinder))]
    public class Rooms : List<string> {
        public Rooms(string[] rooms) {
            this.AddRange(rooms);
        }

        //TODO: think about abstracting the connection part into a base class maybe
        public void Join() {
            using(var redis = new RedisClient("localhost")){
                //TODO: code me
            }
        }
    }
}