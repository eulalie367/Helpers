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

        private static string _scrollTime;
        [Newtonsoft.Json.JsonIgnore]
        public static string ScrollTime
        {
            set
            {
                _scrollTime = value;
            }
            get
            {
                if (string.IsNullOrEmpty(_scrollTime))
                    _scrollTime = ConfigurationManager.AppSettings["ScrollTime"];
                if (string.IsNullOrEmpty(_scrollTime))
                    _scrollTime = "10m";

                return _scrollTime;
            }
        }

        public string _index { get; set; }
        public string _type { get; set; }
        public Guid _id { get; set; }
        public string _version { get; set; }
        public bool found { get; set; }
        public bool created { get; set; }
        public object _source { get; set; }
        [Newtonsoft.Json.JsonProperty("highlight")]
        public Highlight highlight { get; set; }



        public static t FillEntity<t>(string id, string index, string type) where t : class, new()
        {
            var connectionPool = new Elasticsearch.Net.ConnectionPool.SniffingConnectionPool(new[] { ElasticHelper.ElasticURL });

            Nest.ConnectionSettings settings = new Nest.ConnectionSettings(connectionPool);

            var client = new Nest.ElasticClient(settings);


            return client.Get<t>(g => g
                .Index(index)
                .Type(type)
                .Id(id)
            ).Source;

        }


        public static Nest.IIndexResponse Save(iElasticSearchObject result, string index, string type)
        {
            var connectionPool = new Elasticsearch.Net.ConnectionPool.SniffingConnectionPool(new[] { ElasticHelper.ElasticURL });

            Nest.ConnectionSettings settings = new Nest.ConnectionSettings(connectionPool);

            var client = new Nest.ElasticClient(settings);

            Nest.BulkDescriptor descriptor = new Nest.BulkDescriptor();


            return client.Index(result, i => i
                .Id(result._id)
                .Index(index)
                .Type(type)
            );
        }

        public static Nest.IBulkResponse Save<t>(IEnumerable<t> results, string index, string type) where t : class, iElasticSearchObject
        {
            var connectionPool = new Elasticsearch.Net.ConnectionPool.SniffingConnectionPool(new[] { ElasticHelper.ElasticURL });

            Nest.ConnectionSettings settings = new Nest.ConnectionSettings(connectionPool);

            var client = new Nest.ElasticClient(settings);

            Nest.BulkDescriptor descriptor = new Nest.BulkDescriptor();

            foreach (t i in results)
            {
                descriptor.Index<t>(op => op
                    .Document(i)
                    .Id(i._id)
                    .Index(index)
                    .Type(type)
                );
            }

            return client.Bulk(descriptor);
        }


        public static Nest.IDeleteResponse Delete(string id, string index, string type)
        {
            var connectionPool = new Elasticsearch.Net.ConnectionPool.SniffingConnectionPool(new[] { ElasticHelper.ElasticURL });

            Nest.ConnectionSettings settings = new Nest.ConnectionSettings(connectionPool);

            var client = new Nest.ElasticClient(settings);


            return client.Delete<string>(d => d
                .Index(index)
                .Type(type)
                .Id(id)
            );

        }



        public static Nest.ISearchResponse<t> Search<t>(Nest.SearchDescriptor<t> req, string indexQuery, string[] types) where t : class, new()
        {
            var connectionPool = new Elasticsearch.Net.ConnectionPool.SniffingConnectionPool(new[] { ElasticHelper.ElasticURL });

            Nest.ConnectionSettings settings = new Nest.ConnectionSettings(connectionPool);

            var client = new Nest.ElasticClient(settings);

            return client.Search<t>
            (
                req
                .Types(types)
                .Indices(indexQuery)
                .Scroll(ElasticHelper.ScrollTime)
            );
        }

        public static Nest.ISearchResponse<t> Scroll<t>(string scrollID) where t : class, new()
        {
            var connectionPool = new Elasticsearch.Net.ConnectionPool.SniffingConnectionPool(new[] { ElasticHelper.ElasticURL });

            Nest.ConnectionSettings settings = new Nest.ConnectionSettings(connectionPool);

            var client = new Nest.ElasticClient(settings);

            return client.Scroll<t>(sc => sc.ScrollId(scrollID).Scroll(ElasticHelper.ScrollTime));
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
            long FeedID { get; }
        }
        public class Highlight
        {
            public List<string> Content { get; set; }
        }
    }
}
