using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace webchat.Models {
    [ModelBinder(typeof(RoomsModelBiner))]
    public class Rooms : List<string>{
        public Rooms(string[] rooms) {
            this.AddRange(rooms);
        }
    }
}