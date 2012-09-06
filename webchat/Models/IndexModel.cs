using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using webchat.Validators;
using ServiceStack.Redis;

namespace webchat.Models {
    public class IndexModel {
        [Required(ErrorMessageResourceName = "RequiredNickError",
            ErrorMessageResourceType=typeof(Resources.Strings))]
        [StringLength(30, MinimumLength = 3, ErrorMessageResourceName = "LengthNickError",
            ErrorMessageResourceType = typeof(Resources.Strings))]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceName = "CharNickError",
            ErrorMessageResourceType = typeof(Resources.Strings))]
        [UniqueNickValidation]
        public string Nick { get; set; }

        [RoomsValidation]
        public Rooms Rooms { get; set; }

        public void Store() {
            using(var redis = new RedisClient().As<string>()){
                var global_user_list = redis.Sets[Resources.Strings.GlobalUserListKey];
                redis.AddItemToSet(global_user_list, Nick);
            }
            
            Rooms.AddUser(Nick);
        }
    }
}