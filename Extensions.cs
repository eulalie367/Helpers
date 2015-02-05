using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Linq;
using System.Web.UI;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class Extensions
    {
        ///// <summary>
        ///// Parses a request string and returns it's nullable int value
        ///// </summary>
        //public static int? ToInt(this HttpRequest req, string key)
        //{
        //    int? retVal = null;
        //    try
        //    {
        //        int tmp = -1;
        //        if (req[key] != null && !string.IsNullOrEmpty(req[key]))
        //            if (int.TryParse(req[key], out tmp))
        //                retVal = tmp;
        //    }
        //    catch
        //    { }
        //    return retVal;
        //}
        /// <summary>
        /// Parses a string and returns it's nullable int value
        /// </summary>
        public static int? ToInt(this object obj)
        {
            int? retVal = null;
            try
            {
                if (obj != null)
                {
                    string str = obj.ToString();
                    int tmp = -1;
                    if (!string.IsNullOrEmpty(str))
                        if (int.TryParse(str, out tmp))
                            retVal = tmp;
                }
            }
            catch
            { }
            return retVal;
        }
        public static long? ToLong(this object obj)
        {
            long? retVal = null;
            try
            {
                if (obj != null)
                {
                    string str = obj.ToString();
                    long tmp = -1;
                    if (!string.IsNullOrEmpty(str))
                        if (long.TryParse(str, out tmp))
                            retVal = tmp;
                }
            }
            catch
            { }
            return retVal;
        }
        public static Guid? ToGuid(this string str)
        {
            Guid retVal;
            if (Guid.TryParse(str, out retVal))
                return retVal;
            return null;
        }
        public static double? ToDouble(this string str)
        {
            double? retVal = null;
            try
            {
                double tmp = -1;
                if (!string.IsNullOrEmpty(str))
                    if (double.TryParse(str, out tmp))
                        retVal = tmp;
            }
            catch
            { }
            return retVal;
        }
        public static Uri ToUri(this string str)
        {
            Uri retVal = null;
            try
            {
                retVal = new Uri(str);
            }
            catch
            { }
            return retVal;
        }
        public static int RoundUp(this double d)
        {
            int retval = (int)d;
            if (retval < d)
                retval++;
            return retval;
        }
        /// <summary>
        /// Parses a string and returns it's nullable datetime value
        /// </summary>
        public static DateTime? ToDateTime(this object obj)
        {
            DateTime? retVal = null;
            try
            {
                if (obj != null)
                {
                    string str = obj.ToString();
                    DateTime tmp;
                    if (!string.IsNullOrEmpty(str))
                        if (DateTime.TryParse(str, out tmp))
                            retVal = tmp;
                }
            }
            catch
            { }
            return retVal;
        }
        /// <summary>
        /// Parses a string and returns it's nullable bool value
        /// </summary>
        public static Boolean? ToBool(this object obj)
        {
            if (obj != null)
            {
                string str = obj.ToString();
                if (!string.IsNullOrEmpty(str))
                {
                    switch (str.ToLower())
                    {
                        case "on":
                            return true;
                        case "off":
                            return false;
                        case "yes":
                            return true;
                        case "no":
                            return false;
                        case "true":
                            return true;
                        case "false":
                            return false;
                        case "1":
                            return true;
                        case "0":
                            return false;
                        default:
                            return null;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// This is a helper stub to fill a select in 1 line...
        /// </summary>
        /// <typeparam name="source">IEnumerable object to fill the select with</typeparam>
        /// <param name="nameCol">The parameterName to use for the DataTextField when filling the select</param>
        /// <param name="valCol">The parameterName to use for the DataValueField when filling the select</param>
        /// <param name="index0">The text to display as the first option in the select</param>
        public static void FillSelect<t>(this DropDownList select, t source, string nameCol, string valCol, string index0, string index0Value) where t : IEnumerable
        {
            if (!string.IsNullOrEmpty(index0))
            {
                select.Items.Add(new ListItem(index0, index0Value));
                select.AppendDataBoundItems = true;
            }
            select.DataSource = source;
            select.DataTextField = nameCol;
            select.DataValueField = valCol;
            select.DataBind();
        }
        /// <summary>
        /// This is a helper stub to fill a select in 1 line...
        /// </summary>
        /// <typeparam name="source">IEnumerable object to fill the select with</typeparam>
        /// <param name="nameCol">The parameterName to use for the DataTextField when filling the select</param>
        /// <param name="valCol">The parameterName to use for the DataValueField when filling the select</param>
        public static void FillSelect<t>(this HtmlSelect select, t source, string nameCol, string valCol) where t : IEnumerable
        {
            select.DataSource = source;
            select.DataTextField = nameCol;
            select.DataValueField = valCol;
            select.DataBind();
        }

        public static string GetItemValue(this XmlAttributeCollection attribs, string name)
        {
            XmlNode n = attribs.GetNamedItem(name);
            if (n != null)
                return n.Value;
            else
                return "";
        }

        public static string GetAttributeValue(this XElement elem, string name)
        {
            XAttribute attr = elem.Attribute(name);
            if (attr != null && !string.IsNullOrEmpty(attr.Value))
                return attr.Value;
            else
                return "";
        }

        public static string GetElementValue(this XmlDocument xDoc, string xpath)
        {
            XmlNode node = xDoc.SelectSingleNode(xpath);
            if (node != null)
            {
                return node.Value;
            }
            return "";
        }
        public static string GetElementValue(this XmlNode xNode, string xpath)
        {
            XmlNode node = xNode.SelectSingleNode(xpath);
            if (node != null)
            {
                return node.InnerText;
            }
            return "";
        }

        public static void AppendAttribute(this System.Web.UI.HtmlControls.HtmlContainerControl cont, string name, string value)
        {
            string a = cont.Attributes[name];
            if (string.IsNullOrEmpty(a))
                cont.Attributes.Add(name, value);
            else
            {
                cont.Attributes[name] += " " + value;
            }
        }

        public static void AddSafely(this AttributeCollection attribs, string key, string value)
        {
            if (attribs != null)
            {
                if (!string.IsNullOrEmpty(attribs[key]))
                {
                    attribs[key] += " " + value;
                }
                else
                {
                    attribs.Add(key, value);
                }
            }
        }
        public static string ToRssDateString(this DateTime dt)
        {
            dt = dt.ToUniversalTime();
            return dt.ToString("ddd, dd MMM yyyy HH:mm:ss G\\MT");
        }
        public static long ToUnixTimeStamp(this DateTime dt)
        {
            return (long)(dt - new DateTime(1970, 1, 1)).TotalSeconds;
        }
        public static DateTime FromUnixTimeStamp(this long seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }

        public static string TruncateWholeString(this string s, int length, string trailingString)
        {
            if (!string.IsNullOrEmpty(s) && s.Length >= length)
            {
                s = s.Substring(0, length);
                if (s.IndexOf(' ') > 0)
                {
                    s = s.Substring(0, s.LastIndexOf(' '));
                }
                if (!string.IsNullOrEmpty(trailingString))
                    s += trailingString;
            }
            return s;
        }
        public static string TruncateWholeString(this string s, int length)
        {
            return TruncateWholeString(s, length, "");
        }

        public static string RemoveQuerystringParam(this Uri u, string key)
        {
            if (u != null && !string.IsNullOrEmpty(u.Query))
            {
                return RemoveQuerystringParam(u.Query, key);
            }
            return "";
        }
        public static string RemoveQuerystringParam(this string u, string key)
        {
            if (!string.IsNullOrEmpty(u))
            {
                string baseQstring = u.Replace('?', '&');
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("&" + key + "=[^&]+");
                baseQstring = r.Replace(baseQstring, "");
                if (!string.IsNullOrEmpty(baseQstring))
                    return "?" + baseQstring.Remove(0, 1);
            }

            return "?";
        }
        public static string AppendQuerystringParam(this Uri u, string key, string value)
        {
            if (u != null)
            {
                return AppendQuerystringParam(u.Query, key, value);
            }
            return "";
        }
        public static string AppendQuerystringParam(this string u, string key, string value)
        {
            string baseQuery = u.RemoveQuerystringParam(key);
            return string.Format("{0}{1}{2}={3}", baseQuery, (baseQuery == "?" ? "" : "&"), key, value);
        }

        public static string InnerHTML(this string html, string tagName)
        {
            string retVal = "";

            if (!string.IsNullOrEmpty(html))
            {
                System.Text.RegularExpressions.Regex r = new Text.RegularExpressions.Regex(string.Format("<{0}.*>([^<]+)", tagName));
                var matches = r.Match(html);
                if (matches != null && matches.Groups.Count > 0 && matches.Groups[1] != null)
                    retVal = matches.Groups[1].Value;
            }

            return retVal;
        }

        public static System.Data.DataRow[] GetRows(this System.Data.DataSet ds, int table)
        {
            List<System.Data.DataRow> retVal = new List<System.Data.DataRow>();

            if (ds != null && ds.Tables != null && ds.Tables[table] != null)
                foreach (System.Data.DataRow dr in ds.Tables[table].Rows)
                    retVal.Add(dr);

            return retVal.ToArray();
        }
        public static System.Data.DataRow[] GetDefaultRows(this System.Data.DataSet ds)
        {
            return ds.GetRows(0);
        }

        public static System.Drawing.Bitmap CropWhiteSpace(this System.Drawing.Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new System.Drawing.Bitmap(croppedWidth, croppedHeight);
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new System.Drawing.RectangleF(0, 0, croppedWidth, croppedHeight),
                      new System.Drawing.RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      System.Drawing.GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }

        public static System.Drawing.Bitmap Crop(this System.Drawing.Bitmap bmp, int width, int? height)
        {
            height = height ?? (int)(bmp.Height * ((double)width / (double)bmp.Width));

            System.Drawing.Bitmap retVal = new Drawing.Bitmap(width, height.Value);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(retVal);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(bmp, 0, 0, width, height.Value);

            return retVal;
        }

        public static string ToBase64(this string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }
        public static string FromBase64(this string s)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }

        public static Dictionary<string, string> GetRequestHeader(this Uri address)
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            System.Net.WebRequest request = System.Net.WebRequest.Create(address);

            request.Method = "HEAD";

            using (WebResponse response = request.GetResponse())
            {
                foreach (string key in response.Headers.AllKeys)
                {
                    var h = response.Headers[key];
                    retVal.Add(key, h);
                }
            }


            return retVal;
        }
        public static string GetWebRequest(this Uri address, string method)
        {
            string retVal = "";
            System.Net.WebRequest request = System.Net.WebRequest.Create(address);

            if (!string.IsNullOrEmpty(method))
                request.Method = method;

            using (WebResponse response = request.GetResponse())
            {
                using (System.IO.Stream s = response.GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new IO.StreamReader(s))
                    {
                        retVal = sr.ReadToEnd();
                    }
                }
            }


            return retVal;
        }
        public static Uri GetFinalURL(this Uri address)
        {
            System.Net.WebRequest request = System.Net.WebRequest.Create(address);
            request.Method = "HEAD";

            Uri retVal = null;
            using (WebResponse response = request.GetResponse())
            {
                using (System.IO.Stream s = response.GetResponseStream())
                {
                    retVal = response.ResponseUri;
                }
            }
            return retVal;
        }

        public static Guid GetHashCode128(this string s)
        {
            System.Security.Cryptography.MD5 c = System.Security.Cryptography.MD5.Create();
            //s = s.NormalizeURL();
            s = s.ToLower();
            s = Regex.Replace(s, "\\s", "");
            byte[] b = c.ComputeHash(Encoding.UTF8.GetBytes(s));
            int z = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(b, 0));
            short y = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt16(b, 4));
            short x = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt16(b, 6));
            Guid g = new Guid(z, y, x, b.Skip(8).ToArray());
            return g;
        }

        public static string NormalizeURL(this string s)
        {
            //tolower breaks youtube
            //            s = s.ToLower();
            System.Text.RegularExpressions.Regex r = new Text.RegularExpressions.Regex("\\s");
            s = r.Replace(s, "");
            return s;
        }
        public static string PercentEncode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = Uri.EscapeDataString(value);

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = System.Text.RegularExpressions.Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27")
                .Replace("%22", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }
        public static string PercentDecode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = System.Text.RegularExpressions.Regex.Replace(value, "(%[0-9A-F][0-9A-F])", c => c.Value.ToLower());

            value = Uri.UnescapeDataString(value);


            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("%28", "(")
                .Replace("%29", ")")
                .Replace("%24", "$")
                .Replace("%21", "!")
                .Replace("%2A", "*")
                .Replace("%27", "'");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            //value = value.Replace("%7E", "~");

            return value;
        }


        public static string ToJSONString(this string value)
        {
            #region illegalChars

            char[] illegalChars = new char[] 
            { 
                (char)1, 
                (char)2, 
                (char)3, 
                (char)4, 
                (char)5, 
                (char)6, 
                (char)7, 
                (char)11, 
                (char)14, 
                (char)15, 
                (char)16, 
                (char)17, 
                (char)18,
                (char)19,
                (char)20,
                (char)21,
                (char)22,
                (char)23,
                (char)24,
                (char)25,
                (char)26,
                (char)27,
                (char)28,
                (char)29,
                (char)30,
                (char)31,
                (char)8232,
                (char)8233
            };

            #endregion

            foreach (char c in illegalChars)
            {
                value = value.Replace(c, (char)0);
            }
            return value;
        }


        public static string URLEncode(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            value = Uri.EscapeDataString(value);

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = System.Text.RegularExpressions.Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value
                .Replace("(", "%28")
                .Replace(")", "%29")
                .Replace("$", "%24")
                .Replace("!", "%21")
                .Replace("*", "%2A")
                .Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }

        public static string URLDecode(this string value)
        {
            return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(value));
        }

        public static string RenderControl(this System.Web.UI.UserControl c)
        {
            StringBuilder sb = new StringBuilder();
            using (System.IO.StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter w = new HtmlTextWriter(sw))
                {
                    c.RenderControl(w);
                }
            }
            return sb.ToString();
        }

        public static List<string> GetStandardOutPut(this System.Diagnostics.Process process, string cmd, string arguments)
        {
            List<string> retVal = new List<string>();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = cmd,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            process.Start();
            process.WaitForExit();


            using (var std = process.StandardOutput)
            {
                while (!std.EndOfStream)
                {
                    string s = std.ReadLine();

                    if (!string.IsNullOrEmpty(s))
                    {
                        retVal.Add(s);
                    }
                }
            }
            return retVal;
        }

        public static void RunProcess(this System.Diagnostics.Process process, string cmd, string arguments)
        {
            List<string> retVal = new List<string>();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = cmd,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = true
            };

            process.Start();
            process.WaitForExit();
        }

        public static string JSONStringify(this string s)
        {
            try
            {
                var json = Newtonsoft.Json.Linq.JObject.Parse(s);
                return json.ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch
            {
                try
                {
                    var json = Newtonsoft.Json.Linq.JArray.Parse(s);
                    return json.ToString(Newtonsoft.Json.Formatting.Indented);
                }
                catch
                {
                    return "";
                }
            }
        }

        public static string XmlEscape(this string unescaped)
        {
            string retVal = unescaped.Replace((char)0x00, ' ').Replace((char)0x01, ' ').Replace((char)0x02, ' ').Replace((char)0x03, ' ').Replace((char)0x04, ' ').Replace((char)0x05, ' ').Replace((char)0x06, ' ').Replace((char)0x07, ' ').Replace((char)0x08, ' ').Replace((char)0x0B, ' ').Replace((char)0x0C, ' ').Replace((char)0x0E, ' ').Replace((char)0x0F, ' ').Replace((char)0x10, ' ').Replace((char)0x11, ' ').Replace((char)0x12, ' ').Replace((char)0x13, ' ').Replace((char)0x14, ' ').Replace((char)0x15, ' ').Replace((char)0x16, ' ').Replace((char)0x17, ' ').Replace((char)0x18, ' ').Replace((char)0x19, ' ').Replace((char)0x1A, ' ').Replace((char)0x1B, ' ').Replace((char)0x1C, ' ').Replace((char)0x1D, ' ').Replace((char)0x1E, ' ').Replace((char)0x1F, ' ').Replace((char)0x7F, ' ');
            retVal = retVal.Replace("<", "&#60;").Replace(">", "&#62;").Replace("\"", "&#34;").Replace("&", "&#38;").Replace("'", "&#39;");
            return retVal.Trim();

            //            return XmlConvert.EncodeName(unescaped);
        }

        public static string XmlUnescape(this string escaped)
        {
            XmlDocument doc = new XmlDocument();
            var node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return node.InnerText;
        }
        public static string FormatWith(this string format, object source)
        {
            return FormatWith(format, null, source);
        }

        public static string FormatWith(this string format, IFormatProvider provider, object source)
        {
            if (!string.IsNullOrEmpty(format))
            {
                Regex r = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
                  RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                List<object> values = new List<object>();
                string rewrittenFormat = r.Replace(format, delegate(Match m)
                {
                    Group startGroup = m.Groups["start"];
                    Group propertyGroup = m.Groups["property"];
                    Group formatGroup = m.Groups["format"];
                    Group endGroup = m.Groups["end"];

                    if (propertyGroup.Value.ToInt().HasValue)
                    {
                        return startGroup.Value + propertyGroup.Value + endGroup.Value;
                    }
                    else
                    {
                        return DataBinder.Eval(source, propertyGroup.Value).ToString();
                    }
                });

                return rewrittenFormat;
            }
            return format;
        }

        public static async System.Threading.Tasks.Task ForEachAsync<T>(this IEnumerable<T> source, Func<T, System.Threading.Tasks.Task> body)
        {
            foreach (var item in source) await body(item);
        }
        public static void ForEachAsync<T>(this IEnumerable<T> source, int maxThreads, Func<T, System.Threading.Tasks.Task> body)
        {
            source.ForEachAsync<T>(maxThreads, body, true);
        }
        public static void ForEachAsync<T>(this IEnumerable<T> source, int maxThreads, Func<T, System.Threading.Tasks.Task> body, bool wait)
        {
            int sourceCount = source.Count();
            if (sourceCount > 0)
            {
                var partitions = System.Collections.Concurrent.Partitioner.Create(source).GetPartitions(sourceCount < maxThreads ? sourceCount : maxThreads);

                IEnumerable<System.Threading.Tasks.Task> tasks = partitions.Select
                    (p =>
                        System.Threading.Tasks.Task.Run
                        (
                            async delegate
                            {
                                using (p)
                                {
                                    while (p.MoveNext())
                                    {
                                        await body(p.Current);
                                    }
                                }
                            }
                        )
                    );

                if (wait)
                {
                    try
                    {
                        System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }
        }
        public static void ForEachAsync<T>(this IEnumerable<T> source, int maxThreads, Func<T, bool> body, bool wait)
        {
            int itterations = source.Count();

            int pages = itterations > maxThreads ? itterations / maxThreads + 1 : 1;

            T[] sa = source.ToArray();

            for (int p = 1; p <= pages; p++)
            {


                int it = p == pages ? itterations == maxThreads ? itterations : pages * maxThreads - itterations : maxThreads;

                Task[] tasks = new Task[it];
                for (int i = 0; i < it; i++)
                {
                    int ia = i;
                    tasks[i] = Task.Factory.StartNew
                    (
                        async () =>
                        {
                            int c = ia + (p - 1) * maxThreads;
                            body(sa[c]);
                        }
                    );
                }

                if (wait)
                {
                    try
                    {
                        System.Threading.Tasks.Task.WaitAll(tasks);
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }
        }

        public static void ForEachAsync(this int itterations, Func<int, System.Threading.Tasks.Task> body)
        {
            itterations.ForEachAsync(32, body, true);
        }
        public static void ForEachAsync(this int itterations, int maxThreads, Func<int, System.Threading.Tasks.Task> body)
        {
            itterations.ForEachAsync(maxThreads, body, true);
        }
        public static void ForEachAsync(this int itterations, int maxThreads, Func<int, System.Threading.Tasks.Task> body, bool wait)
        {
            int[] it = new int[itterations];
            for (int i = 0; i < itterations; i++)
            {
                it[i] = i;
            }
            var partitions = System.Collections.Concurrent.Partitioner.Create(it).GetPartitions(itterations < maxThreads ? itterations : maxThreads);

            IEnumerable<System.Threading.Tasks.Task> tasks = partitions.Select
                (p =>
                    System.Threading.Tasks.Task.Run
                    (
                        async delegate
                        {
                            using (p)
                            {
                                while (p.MoveNext())
                                {
                                    await body(p.Current);
                                }
                            }
                        }
                    )
                );

            if (wait && tasks != null && tasks.Count() > 0)
            {
                System.Threading.Tasks.Task.WaitAll(tasks.ToArray());
            }
        }
        public static void ForEachAsync(this int itterations, int maxThreads, Func<int, bool> body, bool wait)
        {
            int pages = itterations > maxThreads ? itterations / maxThreads + 1 : 1;

            for (int p = 1; p <= pages; p++)
            {


                int it = p == pages ? itterations <= maxThreads ? itterations : pages * maxThreads - itterations : maxThreads;

                Task[] tasks = new Task[it];
                for (int i = 0; i < it; i++)
                {
                    tasks[i] = Task.Factory.StartNew
                    (
                        async () =>
                        {
                            body(i);
                        }
                    );
                }

                if (wait)
                {
                    try
                    {
                        System.Threading.Tasks.Task.WaitAll(tasks);
                    }
                    catch (Exception ex)
                    {
                        throw ex.InnerException;
                    }
                }
            }

        }
    }
}