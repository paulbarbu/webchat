using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using webchat.Validators;
using ServiceStack.Redis;

namespace webchat.Models {
    public class IndexModel {
        [Required(ErrorMessage = "Please provide a valid nickname in order to connect!")]
        [StringLength(30, MinimumLength = 3,
            ErrorMessage = "Your nickname must be at least 3 characters long!")]
        [RegularExpression(@"^[\w]+$",
            ErrorMessage = "Your nickname must be composed only of alphanumeric characters and underscores!")]
        [UniqueNickValidation]
        public string Nick { get; set; }

        [RoomsValidation]
        public Rooms Rooms { get; set; }

        public void Store() {
            //TODO: think about the strongly typed client

            using(var redis = new RedisClient()){
                redis.AddItemToSet("user_list", Nick);
            }
            
            Rooms.AddUser(Nick);
        }
    }
}