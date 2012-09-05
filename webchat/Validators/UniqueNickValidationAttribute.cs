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

            using(var redis = new RedisClient().As<string>()) {
                var global_user_list = redis.Sets["global_user_list"];

                if(redis.SetContainsItem(global_user_list, nick)) {
                    return false;
                }
            }

            return true;
        }
    }
}