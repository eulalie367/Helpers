﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace System
{
    public static class WebExtensions
    {
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
                    if (req.Method == "GET")//don't change the method if it has been set manually.
                        req.Method = "POST";

                    using (System.IO.Stream reqStream = req.GetRequestStream())
                    {
                        ASCIIEncoding encoding = new ASCIIEncoding();
                        byte[] data = encoding.GetBytes(pData);
                        reqStream.Write(data, 0, data.Length);
                    }
                }

                if (!string.IsNullOrEmpty(ConsumerKey) && !string.IsNullOrEmpty(ConsumerSecret))
                {
                    req.AddOAuth(pData, ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret);
                }

                try
                {
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
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse r = ex.Response as HttpWebResponse;
                        if (r != null)
                        {
                            throw new ProtocolException(r.StatusCode, ex.Message);
                        }
                    }
                    throw ex;
                }
            }
            return "";
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
            if (!string.IsNullOrEmpty(AccessToken))
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

            string at = string.IsNullOrEmpty(AccessToken) ? "" : string.Format("oauth_token=\"{0}\",", AccessToken);
            string oauth = "OAuth realm=\"{0}\",oauth_consumer_key=\"{1}\",oauth_nonce=\"{2}\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"{3}\",{4}oauth_version=\"1.0\",oauth_signature=\"{5}\"";
            oauth = string.Format(oauth, "Spiral16", ConsumerKey, nonce, timeStamp, at, signature);

            req.Headers.Add("Authorization", oauth);
            req.ContentType = "application/x-www-form-urlencoded";

            return req;
        }

    }

    public class ProtocolException : WebException
    {
        public HttpStatusCode StatusCode { get; set; }

        public ProtocolException()
            : base()
        { }
        public ProtocolException(string message)
            : base(message)
        { }
        public ProtocolException(HttpStatusCode statusCode)
            : base()
        {
            this.StatusCode = statusCode;
        }
        public ProtocolException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
