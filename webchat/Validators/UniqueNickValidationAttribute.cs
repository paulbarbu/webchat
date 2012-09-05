using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace webchat.Validators {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class UniqueNickValidationAttribute : ValidationAttribute {
        public UniqueNickValidationAttribute()
            : base("This nick is already in use!") {
        }

        public override bool IsValid(object value) {
            if(null == value) {
                return true;
            }

            string nick = (string)value;

            using(var redisClient = new RedisClient("localhost")) {
                if(redisClient.SetContainsItem("user_list", nick)) {
                    return false;
                }
            }

            return true;
        }
    }
}