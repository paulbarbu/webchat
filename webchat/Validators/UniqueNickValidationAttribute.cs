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
            : base(Resources.Strings.UniqueNickError) {
        }

        public override bool IsValid(object value) {
            if(null == value) {
                return true;
            }

            string nick = (string)value;            

            try {
                using(var redis = new RedisClient().As<string>()) {
                    var global_user_list = redis.Sets[Resources.Strings.GlobalUserListKey];

                    if(redis.SetContainsItem(global_user_list, nick)) {
                        return false;
                    }
                }
            }
            catch(RedisException){
                /**
                 * At this point this is risky, but if other redis exceptions 
                 * are properly catched then the Controller would stop processing.
                 * Also even if the nick is a duplicate nothing would happen
                 * since redis is not functioning, so overall this isn't riky at all.
                 */
                return true;
            }

            return true;
        }
    }
}