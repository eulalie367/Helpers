﻿using System;
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
        public static Boolean? ToBool(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                switch (str.ToLower())
                {
                    case "on":
                        return true;
                        break;
                    case "off":
                        return false;
                        break;
                    case "yes":
                        return true;
                        break;
                    case "no":
                        return false;
                        break;
                    case "true":
                        return true;
                        break;
                    case "false":
                        return false;
                        break;
                    case "1":
                        return true;
                        break;
                    case "0":
                        return false;
                        break;
                    default:
                        return null;
                        break;
                }
            }
            else
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

        public static string GetResponseString(this HttpWebRequest req)
        {
            return req.GetResponseString(null, null, null, null, null);
        }
        public static string GetResponseString(this HttpWebRequest req, string pData)
        {
            return req.GetResponseString(pData, null, null, null, null);
        }
        public static string GetResponseString(this HttpWebRequest req, string ConsumerKey, string ConsumerSecret, string AccessToken, string AccessTokenSecret)
        {
            return req.GetResponseString(null, ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
        }
        public static string GetResponseString(this HttpWebRequest req, string pData, string ConsumerKey, string ConsumerSecret, string AccessToken, string AccessTokenSecret)
        {
            if (req != null)
            {
                //req.Method = "GET";//Get is the default anyway.
                if (!string.IsNullOrEmpty(pData))
                {
                    req.Method = "POST";
                    using (System.IO.Stream reqStream = req.GetRequestStream())
                    {
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] data = encoding.GetBytes(pData);
                        reqStream.Write(data, 0, data.Length);
                    }
                }

                if (!string.IsNullOrEmpty(ConsumerKey) && !string.IsNullOrEmpty(ConsumerSecret) && !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(AccessTokenSecret))
                {
                    req.AddOAuth(pData, ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
                }

                using (HttpWebResponse resp = req.GetResponse() as HttpWebResponse)
                {
                    using (System.IO.Stream st = resp.GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(st))
                        {
                            return sr.ReadToEnd();
                        }
                    }

                }
            }
            return "";
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

        public static string TruncateWholeString(this string s, int length, string trailingString)
        {
            if (s.Length >= length)
            {
                s = s.Substring(0, length);
                s = s.Substring(0, s.LastIndexOf(' '));
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
            if (!string.IsNullOrEmpty(u.Query))
            {
                string baseQstring = u.Query.Replace('?', '&');
                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("&" + key + "=[^&]+");
                baseQstring = r.Replace(baseQstring, "");
                if (!string.IsNullOrEmpty(baseQstring))
                    return "?" + baseQstring.Remove(0, 1);
            }

            return "?";
        }
        public static string AppendQuerystringParam(this Uri u, string key, string value)
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

        public static string GetRequestHeader(this Uri address)
        {
            return GetWebRequest(address, "HEAD");
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

        public static Guid GetHashCode128(this string s)
        {
            System.Security.Cryptography.MD5 c = System.Security.Cryptography.MD5.Create();
            s = s.NormalizeURL();
            byte[] b = c.ComputeHash(Encoding.UTF8.GetBytes(s));
            int z = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(b, 0));
            short y = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt16(b, 4));
            short x = System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt16(b, 6));
            Guid g = new Guid(z, y, x, b.Skip(8).ToArray());
            return g;
        }

        public static string NormalizeURL(this string s)
        {
            s = s.ToLower();
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
                .Replace("'", "%27");

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

        public static HttpWebRequest AddOAuth(this HttpWebRequest req, string ConsumerKey, string ConsumerSecret, string AccessToken, string AccessTokenSecret)
        {
            return req.AddOAuth("", ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
        }
        public static HttpWebRequest AddOAuth(this HttpWebRequest req, string pData, string ConsumerKey, string ConsumerSecret, string AccessToken, string AccessTokenSecret)
        {
            string nonce = new Random().Next(Int16.MaxValue, Int32.MaxValue).ToString("X", System.Globalization.CultureInfo.InvariantCulture);
            string timeStamp = DateTime.UtcNow.ToUnixTimeStamp().ToString();

            // Create the base string. This is the string that will be hashed for the signature.
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("oauth_consumer_key", ConsumerKey);
            param.Add("oauth_nonce", nonce);
            param.Add("oauth_signature_method", "HMAC-SHA1");
            param.Add("oauth_timestamp", timeStamp);
            param.Add("oauth_token", AccessToken);
            param.Add("oauth_version", "1.0");

            pData += req.RequestUri.Query;

            foreach (string kv in pData.Replace("?", "&").Split('&'))
            {
                string[] akv = kv.Split('=');
                if (akv.Length == 2)
                {
                    param.Add(akv[0], akv[1].PercentDecode());
                }
            }

            StringBuilder sParam = new StringBuilder(); ;
            foreach (KeyValuePair<string, string> p in param.OrderBy(k => k.Key))
            {
                if (sParam.Length > 0)
                    sParam.Append("&");

                sParam.AppendFormat("{0}={1}", p.Key.PercentEncode(), p.Value.PercentEncode());
            }

            string url = req.RequestUri.AbsoluteUri;
            if (!string.IsNullOrEmpty(req.RequestUri.Query))
            {
                url = url.Replace(req.RequestUri.Query, "");
            }

            string signatureBaseString
                = string.Format("{0}&{1}&{2}",
                req.Method.ToUpper(),
                url.PercentEncode(),
                sParam.ToString().PercentEncode()
            );

            
            // Create our hash key (you might say this is a password)
            string signatureKey = string.Format("{0}&{1}", ConsumerSecret.PercentEncode(), AccessTokenSecret.PercentEncode());

            
            // Generate the hash
            System.Security.Cryptography.HMACSHA1 hmacsha1 = new System.Security.Cryptography.HMACSHA1(Encoding.UTF8.GetBytes(signatureKey));
            byte[] signatureBytes = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString));

            string signature = Convert.ToBase64String(signatureBytes).PercentEncode();

            string oauth = "OAuth realm=\"{0}\",oauth_consumer_key=\"{1}\",oauth_nonce=\"{2}\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"{3}\",oauth_token=\"{4}\",oauth_version=\"1.0\",oauth_signature=\"{5}\"";
            oauth = string.Format(oauth, "Spiral16", ConsumerKey, nonce, timeStamp, AccessToken, signature);

            req.Headers.Add("Authorization", oauth);

            return req;
        }

    }
}
