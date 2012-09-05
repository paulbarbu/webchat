using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webchat.Models {
    [ModelBinder(typeof(RoomsModelBinder))]
    public class Rooms : List<string>{
        public Rooms(string[] rooms) {
            this.AddRange(rooms);
        }

        public void Validate(ModelState modelState){
            // TODO: implement this in order to decouple the validation from the RoomsModelBinder 
            // TODO: think about implementing an interface here: IModelValidatable
        }
    }
}