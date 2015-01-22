using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Spiral16.Utilities
{
    public class ElasticHelper
    {
        [Newtonsoft.Json.JsonIgnore]
        public static string Collection
        {
            get
            {
                //this is so we can switch the collection on the fly.  We will most likely use this when we switch to 1 index per client
                return "spiral16";
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public static Uri ElasticURL
        {
            get
            {
                return new Uri("http://s16-elasticsearch.cloudapp.net:6782");
            }
        }



        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public string _version { get; set; }
        public bool found { get; set; }
        public bool created { get; set; }
        public object _source { get; set; }



        public static t FillEntity<t>(string id)
        {
            t tmp = default(t);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}/result/{2}", ElasticURL, Collection, id));
            req.ContentType = "application/json";
            req.Method = "GET";
            string retval = req.GetResponseString();
            if (!string.IsNullOrEmpty(retval))
            {
                ElasticHelper eh = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(retval);
                if (eh != null && eh.found && eh._source != null)
                {
                    tmp = (t)Newtonsoft.Json.JsonConvert.DeserializeObject(eh._source.ToString(), typeof(t));
                }
            }

            return tmp;
        }

        public static ElasticHelper Save(string id, object value)
        {
            ElasticHelper retVal = new ElasticHelper();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}/result/{2}", ElasticURL, Collection, id));
            req.ContentType = "application/json";
            req.Method = "PUT";
            string eh = req.GetResponseString(Newtonsoft.Json.JsonConvert.SerializeObject(value));
            if (eh != null)
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(eh);
            }
            return retVal;
        }

        public static string Save(IEnumerable<iElasticSearchObject> results)
        {
            ElasticHelper retVal = new ElasticHelper();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}/result/_bulk", ElasticURL, Collection));
            req.ContentType = "application/json";
            req.Method = "PUT";

            StringBuilder sb = new StringBuilder();
            foreach (iElasticSearchObject r in results)
            {
                sb.AppendLine(string.Format("{{ \"index\": {{\"_id\": \"{0}\"}}}}", r._id));
                sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(r));
            }


            string eh = req.GetResponseString(sb.ToString());
            //if (eh != null)
            //{
            //    retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(eh);
            //}
            //return retVal;
            return eh;
        }

        
        public static ElasticHelper Delete(string id)
        {
            ElasticHelper retVal = new ElasticHelper();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}/result/{2}", ElasticURL, Collection, id));
            req.ContentType = "application/json";
            req.Method = "DELETE";
            string eh = req.GetResponseString();
            if (eh != null)
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(eh);
            }
            return retVal;
        }

        public static IEnumerable<t> FillEntities<t>(string query)
        {
            IEnumerable<t> tmp = new List<t>();

            SearchResults r = FillEntities(query);

            if (r.hits != null && r.hits.total > 0)
            {
                IEnumerable<ElasticHelper> eh = r.hits.hits.Select(h => Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(h.ToString()));
                if (eh != null && eh.Count() > 0)
                {
                    tmp = eh.Select(h => (t)Newtonsoft.Json.JsonConvert.DeserializeObject(h._source.ToString(), typeof(t)));
                }
            }

            return tmp;
        }

        public static SearchResults FillEntities(string query)
        {
            SearchResults retVal = new SearchResults();

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}/{1}/result/_search", ElasticURL, Collection));
            req.ContentType = "application/json";
            req.Method = "POST";
            string sr = req.GetResponseString(string.Format("{{ \"query\": {{ \"query_string\": {{ \"default_field\" : \"_all\", \"query\": \"{0}\" }} }} }}", query));

            if (sr != null)
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResults>(sr);
            }

            return retVal;
        }



        public class SearchResults
        {
            public int took { get; set; }
            public bool timed_out { get; set; }
            public ShardInfo _shards { get; set; }
            public HitInfo hits { get; set; }

            public class ShardInfo
            {
                public int total { get; set; }
                public int successful { get; set; }
                public int failed { get; set; }
            }

            public class HitInfo
            {
                public int total { get; set; }
                public float max_score { get; set; }
                public List<object> hits { get; set; }
            }
        }

        public interface iElasticSearchObject
        {
            string _id { get; }
        }
    }
}
