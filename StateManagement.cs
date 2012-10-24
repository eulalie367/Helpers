using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace System
{
    public class StateManagement
    {
        public static void Add(string key, object value, int minutes)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                //add for current request, inproc
                context.Items.Remove(key);
                context.Items.Add(key, value);

                //add for session
                if (context.Session != null)
                {
                    context.Session.Remove(key);
                    context.Session.Add(key, value);
                }

                //add globally
                if (context.Cache != null)
                {
                    context.Cache.Remove(key);
                    context.Cache.Add(key, value, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, minutes, 0), CacheItemPriority.Normal, null);
                }
            }
        }
       
        public static void Add(string key, object value, int minutes, System.Data.SqlClient.SqlCommand command)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                //add for current request, inproc
                context.Items.Remove(key);
                context.Items.Add(key, value);

                //add for session
                if (context.Session != null)
                {
                    context.Session.Remove(key);
                    context.Session.Add(key, value);
                }

                //add globally
                if (context.Cache != null)
                {
                    context.Cache.Remove(key);
                    context.Cache.Add(key, value, new SqlCacheDependency(command), Cache.NoAbsoluteExpiration, new TimeSpan(0, minutes, 0), CacheItemPriority.Normal, null);
                }
            }
        }

        public static t Get<t>(string key)
        {
            t retval = default(t);

            HttpContext context = HttpContext.Current;
           
            if (context != null)
            {
                object o = null;

                if (!context.Request["NoCache"].ToBool().HasValue)
                {
                    o = context.Items[key];

                    if (o == null && context.Session != null)
                        o = context.Session[key];

                    if (o == null && context.Cache != null)
                        o = context.Cache[key];

                    if (o != null)
                        try
                        {
                            retval = (t)o;
                        }
                        catch//really don't care if it doesn't cast right, we'll just return empty
                        { }
                }
            }

            return retval;
        }
    }
}
