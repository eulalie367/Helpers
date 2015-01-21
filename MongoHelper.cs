using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    public class MongoHelper
    {
        private static string connectionString;
        public static string ConnectionString
        {
            set
            {
                connectionString = value;
            }
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                    connectionString = ConfigurationManager.ConnectionStrings["DBString"].ConnectionString;
                return connectionString;
            }
        }

        private static MongoClient Client
        {
            get
            {
                return new MongoClient(ConnectionString);
            }
        }

        private static MongoServer Server
        {
            get
            {
                return Client.GetServer();
            }
        }

        public static MongoDatabase DataBase(string dbName)
        {
            return Server.GetDatabase(dbName);
        }

        #region Entities

        public static List<t> FillEntities<t>(string dbName, string collectionName, string field, Enum value)
        {
            var q = Query.EQ(field, value);
            return FillEntities<t>(dbName, collectionName, q);
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName, string field, bool value)
        {
            var q = Query.EQ(field, value);
            return FillEntities<t>(dbName, collectionName, q);
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName, string field, double value)
        {
            var q = Query.EQ(field, value);
            return FillEntities<t>(dbName, collectionName, q);
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName, string field, int value)
        {
            var q = Query.EQ(field, value);
            return FillEntities<t>(dbName, collectionName, q);
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName, string field, string value)
        {
            return FillEntities<t>(dbName, collectionName, field, value, false);
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName, string field, string value, bool exactMatch)
        {
            IMongoQuery q;
            if (exactMatch)
            {
                q = Query.EQ(field, value);
            }
            else
            {
                q = Query.EQ(field, new System.Text.RegularExpressions.Regex(value, Text.RegularExpressions.RegexOptions.IgnoreCase));
            }
            return FillEntities<t>(dbName, collectionName, q);
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName, string query)
        {
            BsonDocument doc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(query);
            QueryDocument q = new QueryDocument(doc); 
            return FillEntities<t>(dbName, collectionName, q);
        }
        private static List<t> FillEntities<t>(string dbName, string collectionName, IMongoQuery query)
        {
            try
            {
                Type type = typeof(t);
                MongoCollection col = DataBase(dbName).GetCollection(collectionName);

                return col.FindAs<t>(query).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<t>();
        }
        public static List<t> FillEntities<t>(string dbName, string collectionName)
        {
            try
            {
                Type type = typeof(t);
                MongoCollection col = DataBase(dbName).GetCollection(collectionName);

                return col.FindAllAs<t>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return new List<t>();
        }

        #endregion

        #region Entity

        public static t FillEntity<t>(string dbName, string collectionName, string field, bool value)
        {
            return FillEntities<t>(dbName, collectionName, field, value).FirstOrDefault();
        }
        public static t FillEntity<t>(string dbName, string collectionName, string field, Enum value)
        {
            return FillEntities<t>(dbName, collectionName, field, value).FirstOrDefault();
        }
        public static t FillEntity<t>(string dbName, string collectionName, string field, double value)
        {
            return FillEntities<t>(dbName, collectionName,field, value).FirstOrDefault();
        }
        public static t FillEntity<t>(string dbName, string collectionName, string field, int value)
        {
            return FillEntities<t>(dbName, collectionName, field, value).FirstOrDefault();
        }
        public static t FillEntity<t>(string dbName, string collectionName, string field, string value)
        {
            return FillEntities<t>(dbName, collectionName, field, value).FirstOrDefault();
        }
        public static t FillEntity<t>(string dbName, string collectionName, string field, string value, bool exactMatch)
        {
            return FillEntities<t>(dbName, collectionName, field, value, exactMatch).FirstOrDefault();
        }
        public static t FillEntity<t>(string dbName, string collectionName, IMongoQuery query)
        {
            return FillEntities<t>(dbName, collectionName, query).FirstOrDefault();
        }

        #endregion

        public static void Save<t>(string dbName, string collectionName, t data)
        {
            var fe = System.Data.MongoHelper.DataBase(dbName).GetCollection<t>(collectionName);
            fe.Save<t>(data);
        }
    }
}
