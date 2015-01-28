using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Reflection;

namespace System.Data
{
    public class SqlHelper
    {
        private static string connString;
        public static string ConnString
        {
            set
            {
                connString = value;
            }
            get
            {
                if (string.IsNullOrEmpty(connString))
                    connString = ConfigurationManager.ConnectionStrings["DBString"].ConnectionString;
                return connString;
            }
        }
        private static int _timeOut;
        public static int TimeOut
        {
            set
            {
                _timeOut = value;
            }
            get
            {
                if(_timeOut == 0)
                    _timeOut = ConfigurationManager.AppSettings["DBTimeOut"].ToInt() ?? 30;
    
                return _timeOut;
            }
        }
        private static readonly object lockExecuteScalar = new object();
        public static object ExecuteScalar(string sql)
        {
            return ExecuteScalar(sql, null, CommandType.Text);
        }
        public static object ExecuteScalar(string sql, SqlParameter[] Params, CommandType commandType)
        {
            return SqlHelper.ExecuteScalar(sql, Params, commandType, SqlHelper.ConnString);
        }
        public static object ExecuteScalar(string sql, SqlParameter[] Params, CommandType commandType, string connString)
        {
            return ExecuteScalar(sql, Params, commandType, connString, 0);
        }
        private static object ExecuteScalar(string sql, SqlParameter[] Params, CommandType commandType, string connString, int tries)
        {
            object retVal = null;
            try
            {
                lock (lockExecuteScalar)
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand com = new SqlCommand(sql, conn))
                        {
                            com.CommandType = commandType;

                            if (Params != null)
                                com.Parameters.AddRange(Params);

                            conn.Open();

                            retVal = com.ExecuteScalar();

                            conn.Close();
                        }
                    }
                }
                if (tries > 0)
                    Logger.Log("Successfull updated {0} after {1} trie(s)", sql, tries + 1);
            }
            catch (SqlException e)
            {
                if (tries < 1000 && e.Message.ToLower().Contains("deadlock"))
                {
                    System.Threading.Thread.SpinWait(1000 * tries * new Random().Next(1, 11));

                    Logger.Log("Deadlock in proc {0}.  Trying Again, Tries:{1}", sql, tries + 1);

                    SqlParameter[] ps = Params.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToArray();//deepcopy

                    return ExecuteScalar(sql, ps, commandType, connString, tries + 1);
                }
                else
                {
                    StringBuilder sb = new StringBuilder(sql);
                    if (Params != null && Params.Length > 0)
                    {
                        foreach (var p in Params)
                        {
                            sb.AppendFormat("\n{0} = {1},", p.ParameterName, p.Value);
                        }
                    }
                    Logger.Warn(e, sb.ToString());

                    throw;
                }
            }

            return retVal;
        }


        private static readonly object lockExecuteNonQuery = new object();
        public static int ExecuteNonQuery(string sql)
        {
            return ExecuteNonQuery(sql, null, CommandType.Text);
        }
        public static int ExecuteNonQuery(string sql, SqlParameter[] Params, CommandType commandType)
        {
            return ExecuteNonQuery(sql, Params, commandType, SqlHelper.ConnString);
        }
        public static int ExecuteNonQuery(string sql, SqlParameter[] Params, CommandType commandType, string connString)
        {
            return ExecuteNonQuery(sql, Params, commandType, connString, 0);
        }
        private static int ExecuteNonQuery(string sql, SqlParameter[] Params, CommandType commandType, string connString, int tries)
        {
            int retVal = 0;
            try
            {
                lock (lockExecuteNonQuery)
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand com = new SqlCommand(sql, conn))
                        {
                            com.CommandTimeout = int.MaxValue;
                            com.CommandType = commandType;
                            if (Params != null && Params.Length > 0)
                                com.Parameters.AddRange(Params);

                            conn.Open();

                            retVal = com.ExecuteNonQuery();

                            conn.Close();
                        }
                    }
                }
                if (tries > 0)
                    Logger.Log("Successfull updated {0} after {1} trie(s)", sql, tries + 1);
            }
            catch (SqlException e)
            {
                StringBuilder sb = new StringBuilder(sql);
                if (Params != null && Params.Length > 0)
                {
                    foreach (var p in Params)
                    {
                        sb.AppendFormat("\n{0} = {1},", p.ParameterName, p.Value);
                    }
                }
                Logger.Warn(e, sb.ToString());
                if (tries < 1000 && e.Message.ToLower().Contains("deadlock"))
                {
                    System.Threading.Thread.SpinWait(1000 * tries * new Random().Next(1, 11));

                    Logger.Log("Deadlock in proc {0} \n{2}.  Trying Again, Tries:{1}", sql, tries + 1, sb.ToString());

                    SqlParameter[] ps = Params.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToArray();//deepcopy

                    return ExecuteNonQuery(sql, ps, commandType, connString, tries + 1);
                }
                else
                {
                    Logger.Warn(e, sb.ToString());

                    throw;
                }
            }

            return retVal;
        }

        private static readonly object lockFillEntities = new object();
        public static List<t> FillEntities<t>(string sql) where t : new()
        {
            return FillEntities<t>(sql, null, CommandType.Text);
        }
        public static List<t> FillEntities<t>(string sql, SqlParameter[] Params, CommandType commandType) where t : new()
        {
            return FillEntities<t>(sql, Params, commandType, SqlHelper.ConnString);
        }
        public static List<t> FillEntities<t>(string sql, SqlParameter[] Params, CommandType commandType, string connString) where t : new()
        {
            return FillEntities<t>(sql, Params, commandType, connString, 0);
        }
        private static List<t> FillEntities<t>(string sql, SqlParameter[] Params, CommandType commandType, string connString, int tries) where t : new()
        {
            List<t> retVal = new List<t>();
            t tmp = default(t);
            try
            {
                lock (lockFillEntities)
                {

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand com = new SqlCommand(sql, conn))
                        {
                            com.CommandType = commandType;
                            if (Params != null && Params.Length > 0)
                                com.Parameters.AddRange(Params);

                            conn.Open();


                            using (SqlDataReader reader = com.ExecuteReader())
                            {
                                string colName = "";
                                object value = null;
                                Type type = typeof(t);
                                PropertyInfo prop = null;
                                while (reader.Read())
                                {
                                    tmp = new t();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        colName = reader.GetName(i);
                                        colName = colName.Replace(" ", ""); //spaces don't work in names, SqlMetal will just remove them... So will we.
                                        value = reader[i];

                                        if (string.IsNullOrEmpty(colName))
                                            prop = type.GetProperties()[i];
                                        else
                                            prop = type.GetProperty(colName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);


                                        SetPropetyValue(tmp, value, prop);
                                    }
                                    retVal.Add(tmp);
                                }
                                reader.Close();
                            }
                            conn.Close();
                        }
                    }
                }
                if (tries > 0)
                    Logger.Log("Successfull updated {0} after {1} trie(s)", sql, tries + 1);
            }
            catch (SqlException e)
            {
                if (tries < 1000 && e.Message.ToLower().Contains("deadlock"))
                {
                    System.Threading.Thread.SpinWait(1000 * tries * new Random().Next(1, 11));

                    Logger.Log("Deadlock in proc {0}.  Trying Again, Tries:{1}", sql, tries + 1);

                    SqlParameter[] ps = Params.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToArray();//deepcopy

                    return FillEntities<t>(sql, ps, commandType, connString, tries + 1);
                }
                else
                {
                    StringBuilder sb = new StringBuilder(sql);
                    if (Params != null && Params.Length > 0)
                    {
                        foreach (var p in Params)
                        {
                            sb.AppendFormat("\n{0} = {1},", p.ParameterName, p.Value);
                        }
                    }
                    Logger.Warn(e, sb.ToString());

                    throw;
                }
            }


            return retVal;
        }

        private static readonly object lockFillEntity = new object();
        public static t FillEntity<t>(string sql) where t : new()
        {
            return FillEntity<t>(sql, null, CommandType.Text, ConnString);
        }
        public static t FillEntity<t>(string sql, SqlParameter[] Params, CommandType commandType) where t : new()
        {
            return FillEntity<t>(sql, Params, commandType, ConnString);
        }
        public static t FillEntity<t>(string sql, SqlParameter[] Params, CommandType commandType, string connString) where t : new()
        {
            return FillEntity<t>(sql, Params, commandType, connString, 0);
        }
        private static t FillEntity<t>(string sql, SqlParameter[] Params, CommandType commandType, string connString, int tries) where t : new()
        {
            t tmp = default(t);
            try
            {
                lock (lockFillEntity)
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand com = new SqlCommand(sql, conn))
                        {
                            com.CommandType = commandType;
                            if (Params != null && Params.Length > 0)
                                com.Parameters.AddRange(Params);

                            conn.Open();

                            using (SqlDataReader reader = com.ExecuteReader())
                            {
                                string colName = "";
                                object value = null;
                                Type type = typeof(t);
                                PropertyInfo prop = null;

                                if (reader.Read())
                                {
                                    tmp = new t();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        colName = reader.GetName(i);
                                        value = reader[i];

                                        prop = type.GetProperty(colName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                                        SetPropetyValue(tmp, value, prop);
                                    }
                                }
                                reader.Close();
                            }
                            conn.Close();
                        }
                    }
                    if (tries > 0)
                        Logger.Log("Successfull updated {0} after {1} trie(s)", sql, tries + 1);

                }
            }
            catch (SqlException e)
            {
                if (tries < 1000 && e.Message.ToLower().Contains("deadlock"))
                {
                    System.Threading.Thread.SpinWait(1000 * tries * new Random().Next(1, 11));


                    Logger.Log("Deadlock in proc {0}.  Trying Again, Tries:{1}", sql, tries + 1);

                    SqlParameter[] ps = Params.Select(p => new SqlParameter(p.ParameterName, p.Value)).ToArray();//deepcopy

                    return FillEntity<t>(sql, ps, commandType, connString, tries + 1);
                }
                else
                {
                    StringBuilder sb = new StringBuilder(sql);
                    if (Params != null && Params.Length > 0)
                    {
                        foreach (var p in Params)
                        {
                            sb.AppendFormat("\n{0} = {1},", p.ParameterName, p.Value);
                        }
                    }
                    Logger.Warn(e, sb.ToString());

                    throw;
                }
            }

            return tmp;
        }

        public static void BulkInsert(DataTable dt, string tableName)
        {
            BulkInsert(dt, tableName, SqlHelper.ConnString);
        }
        public static void BulkInsert(DataTable dt, string tableName, string connString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (System.Data.SqlClient.SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = tableName;

                        foreach (DataColumn dc in dt.Columns)
                        {
                            string cn = dc.ColumnName.Substring(0, 1).ToUpper() + dc.ColumnName.Substring(1, dc.ColumnName.Length - 1);
                            bulkCopy.ColumnMappings.Add(cn, dc.ColumnName);
                        }
                        bulkCopy.BulkCopyTimeout = 300000;
                        conn.Open();
                        bulkCopy.WriteToServer(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                throw;
            }
        }

        private static void SetPropetyValue(object tmp, object value, PropertyInfo prop)
        {
            //add logic here for any DB type that doesn't parse corretly, such as an XElement.

            if (!(value is DBNull) && prop != null)
            {
                if (prop.PropertyType == typeof(System.Xml.Linq.XElement))
                {
                    if (value != null)
                    {
                        string tmpVal = value.ToString();
                        if (!string.IsNullOrEmpty(tmpVal))
                        {
                            try
                            {
                                System.Xml.Linq.XElement elem = System.Xml.Linq.XElement.Parse(tmpVal);
                                prop.SetValue(tmp, elem, null);
                            }
                            catch
                            {
                                try//make sure there is a root node
                                {
                                    System.Xml.Linq.XElement elem = System.Xml.Linq.XElement.Parse(string.Format("<rootasdf>{0}</rootasdf>", tmpVal));
                                    prop.SetValue(tmp, elem, null);
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
                else if (prop.PropertyType == typeof(Uri))
                {
                    prop.SetValue(tmp, new Uri(value.ToString()), null);
                }
                else if (IsNullableType(prop.PropertyType))
                {

                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType);
                    value = Convert.ChangeType(value, targetType); 
                    prop.SetValue(tmp, value, null);
                }
                else
                {
                    prop.SetValue(tmp, value, null);
                }
            }
        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }    
    }
}
