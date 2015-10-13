using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Spiral16.Utilities;

namespace Spiral16.Utilities
{
    public class ElasticHelper
    {
        private static string _collection;
        [Newtonsoft.Json.JsonIgnore]
        public static string Collection
        {
            set
            {
                _collection = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_collection))
                {
                    _collection = ConfigurationManager.AppSettings["ElasticCollection"];
                }

                return string.IsNullOrEmpty(_collection) ? "public" : _collection;
            }
        }

        public static string GetCollection(int organizationID)
        {
            string collection = organizationID != 1 ? organizationID.ToString() : "current";
            return collection;
        }

        private static Uri _elasticURL;
        [Newtonsoft.Json.JsonIgnore]
        public static Uri ElasticURL
        {
            set
            {
                _elasticURL = value;
            }
            get
            {
                if (_elasticURL == null)
                    _elasticURL = ConfigurationManager.ConnectionStrings["ElasticURL"].ConnectionString.ToUri();

                return _elasticURL;
            }
        }


        public string _index { get; set; }
        public string _type { get; set; }
        public Guid _id { get; set; }
        public string _version { get; set; }
        public bool found { get; set; }
        public bool created { get; set; }
        public object _source { get; set; }
        public double? _score { get; set; }
        [Newtonsoft.Json.JsonProperty("highlight")]
        public Highlight highlight { get; set; }



        public static t FillEntity<t>(string id)
        {
            return ElasticHelper.FillEntity<t>(id, ElasticHelper.Collection);
        }
        public static t FillEntity<t>(string id, string collection)
        {
            t tmp = default(t);

            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/result/{2}", ElasticHelper.ElasticURL, collection, id));
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
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/result/{2}", ElasticURL, Collection, id));
            req.ContentType = "application/json";
            req.Method = "PUT";
            string ser = Newtonsoft.Json.JsonConvert.SerializeObject(value, Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore });
            string eh = req.GetResponseString(ser);
            if (eh != null)
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(eh);
            }
            return retVal;
        }

        public static string Save(IEnumerable<iElasticSearchObject> results)
        {
            return Save(results, Collection);
        }
        public static string Save(IEnumerable<iElasticSearchObject> results, string collection)
        {
            if (results != null && results.Count() > 0)
            {
                ElasticHelper retVal = new ElasticHelper();
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/result/_bulk", ElasticURL, collection));
                req.ContentType = "application/json";
                req.Method = "PUT";
                req.Timeout = 1000 * 60 * 3;//3minutes

                StringBuilder sb = new StringBuilder();
                foreach (iElasticSearchObject r in results)
                {
                    try
                    {
                        sb.AppendLine(string.Format("{{ \"index\": {{ \"_id\":\"{0}\" }} }}", r._id));
                        sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(r, Newtonsoft.Json.Formatting.None, new Newtonsoft.Json.JsonSerializerSettings { NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore }));
                    }
                    catch
                    { }
                }


                string eh = req.GetResponseString(sb.ToString());
                //if (eh != null)
                //{
                //    retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(eh);
                //}
                //return retVal;
                return eh;
            }
            return "";
        }


        public static ElasticHelper Delete(string id)
        {
            ElasticHelper retVal = new ElasticHelper();
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/result/{2}", ElasticURL, Collection, id));
            req.ContentType = "application/json";
            req.Method = "DELETE";
            string eh = req.GetResponseString();
            if (eh != null)
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(eh);
            }
            return retVal;
        }



        public static IEnumerable<t> Search<t>(string query, string scrollID, out SearchResults r)
        {
            return Search<t>(query, Collection, scrollID, out r);
        }
        public static IEnumerable<t> Search<t>(string query, string index, string scrollID, out SearchResults r)
        {
            return convertElasticHelper<t>(Search(query, index, scrollID, out r));
        }
        //public static IEnumerable<ElasticHelper> Search(string query, string scrollID, out SearchResults r)
        //{
        //    return Search(query, Collection, scrollID, out r);
        //}
        public static IEnumerable<ElasticHelper> Search(string query, string index, string scrollID, out SearchResults r)
        {
            IEnumerable<ElasticHelper> tmp = new List<ElasticHelper>();

            r = Search(query, index, scrollID);

            if (r.hits != null && r.hits.total > 0)
            {
                tmp = r.hits.hits.Select(h => Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(h.ToString()));
            }

            return tmp;
        }
        public static IEnumerable<ElasticHelper> Search(string query, string index, out SearchResults r)
        {
            IEnumerable<ElasticHelper> tmp = new List<ElasticHelper>();

            r = Search(query, index);

            if (r.hits != null && r.hits.total > 0)
            {
                tmp = r.hits.hits.Select(h => Newtonsoft.Json.JsonConvert.DeserializeObject<ElasticHelper>(h.ToString()));
            }

            return tmp;
        }

        //public static SearchResults Search(string query, string scrollID)
        //{
        //    return Search(query, Collection, scrollID);
        //}
        public static SearchResults Search(string query, string index)
        {
            SearchResults retVal = new SearchResults();
            HttpWebRequest req;

            req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/result/_search", ElasticURL, index));
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.Timeout = 300000;
            string sr = req.GetResponseString(query);

            if (sr != null)
            {
                retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResults>(sr);
                retVal.RawJSON = sr;
            }
            return retVal;
        }
        public static SearchResults Search(string query, string index, string scrollID)
        {
            SearchResults retVal = new SearchResults();
            HttpWebRequest req;

            if (string.IsNullOrEmpty(scrollID))
            {
                req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/result/_search?scroll=5m", ElasticURL, index));
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Timeout = 300000;
                string sr = req.GetResponseString(query);

                if (sr != null)
                {
                    retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResults>(sr);
                    retVal.RawJSON = sr;
                }
                scrollID = retVal._scroll_id;
            }
            else
            {
                req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}_search/scroll?scroll=5m", ElasticURL, index));
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";
                req.Timeout = 60000;
                string s = req.GetResponseString(scrollID);

                if (s != null)
                {
                    retVal = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchResults>(s);
                    retVal.RawJSON = s;
                }

            }
            return retVal;
        }

        private static IEnumerable<t> convertElasticHelper<t>(IEnumerable<ElasticHelper> eh)
        {
            IEnumerable<t> tmp = new List<t>();

            if (eh != null && eh.Count() > 0)
            {
                tmp = eh.Select(h => (t)Newtonsoft.Json.JsonConvert.DeserializeObject(h._source.ToString(), typeof(t)));
            }

            return tmp;
        }


        public class SearchResults
        {
            public int took { get; set; }
            public bool timed_out { get; set; }
            public ShardInfo _shards { get; set; }
            public HitInfo hits { get; set; }
            public string _scroll_id { get; set; }
            public string RawJSON { get; set; }

            public class ShardInfo
            {
                public int total { get; set; }
                public int successful { get; set; }
                public int failed { get; set; }
            }

            public class HitInfo
            {
                public int total { get; set; }
                public float? max_score { get; set; }
                public List<object> hits { get; set; }
            }
        }

        public interface iElasticSearchObject
        {
            string _id { get; }
        }
        public class Highlight
        {
            public List<string> Content { get; set; }
        }


        public static void RefreshCollection(string collection)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/_refresh", ElasticURL, collection));
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string s = req.GetResponseString();

        }

        public static void CreateCollection(string collection)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}{1}/", ElasticURL, collection));
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string s = req.GetResponseString();

        }

        public static void RemoveCollectionAlias(string alias, string collection)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}_aliases/", ElasticURL));

            var action = new
            {
                actions = new[] { 
                    new { remove = new { index=collection, alias = alias } }
                }
            };



            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string s = req.GetResponseString(Newtonsoft.Json.JsonConvert.SerializeObject(action));

        }

        public static void AddCollectionAlias(string alias, string collection)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}_aliases/", ElasticURL));

            var action = new
            {
                actions = new[] { 
                    new { add = new { index=collection, alias = alias } }
                }
            };



            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "PUT";
            string s = req.GetResponseString(Newtonsoft.Json.JsonConvert.SerializeObject(action));

        }

        public static void CollectionReAlias(string alias, string collectionOld, string collectionNew)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(string.Format("{0}_aliases/", ElasticURL));

            var action = new
            {
                actions = new object[] { 
                    new { remove = new { index=collectionOld, alias = alias } },
                    new { add = new { index=collectionNew, alias = alias } }
                }
            };



            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            string s = req.GetResponseString(Newtonsoft.Json.JsonConvert.SerializeObject(action));

        }
    }
}
