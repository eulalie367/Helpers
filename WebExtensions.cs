using System;
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
                            throw new ProtocolException((ApiStatusCode)r.StatusCode, ex.Message);
                        }
                    }
                    throw;
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
        public static HttpWebRequest SignRequest(this HttpWebRequest req, string pData, string singingKey)
        {
            string nonce = new Random().Next(Int16.MaxValue, Int32.MaxValue).ToString("X", System.Globalization.CultureInfo.InvariantCulture);
            string timeStamp = DateTime.UtcNow.ToUnixTimeStamp().ToString();

            // Create the base string. This is the string that will be hashed for the signature.
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("oauth_nonce", nonce);
            param.Add("oauth_signature_method", "HMAC-SHA512");
            param.Add("oauth_timestamp", timeStamp);

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



            // Generate the hash
            System.Security.Cryptography.HMACSHA512 hmacsha512 = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(singingKey));
            byte[] signatureBytes = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(signatureBaseString));

            string signature = Convert.ToBase64String(signatureBytes).PercentEncode();


            req.Headers.Add("oauth_nonce", nonce);
            req.Headers.Add("oauth_signature_method", "HMAC-SHA512");
            req.Headers.Add("oauth_timestamp", timeStamp);
            req.Headers.Add("oauth_signature", signature);
            req.ContentType = "application/x-www-form-urlencoded";

            return req;
        }

        //public static bool CheckRequestSignature(this System.Web.HttpRequest req, string singingKey)
        //{
        //    int? nonce = req.Headers["oauth_nonce"].ToInt();
        //    var signature_method = req.Headers["oauth_signature_method"];
        //    var timestamp = req.Headers["oauth_timestamp"];
        //    string signature = req.Headers["oauth_signature"];
        //    signature = signature = signature.PercentDecode();

        //    if (nonce.HasValue && StateManagement.Get<int?>(nonce.Value.ToString()).HasValue)
        //        return false;

        //    StateManagement.Add(nonce.Value.ToString(), nonce, 525600);

        //    return true;
        //}

    }
    public enum ApiStatusCode : long
    {
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Gone = 410,
        TooManyRequest = 429,
        UnprocessableEntity = 422,
        InternalServerError = 500,
        ServiceUnavailable = 503
    }
    public class ProtocolException : WebException
    {
        public ApiStatusCode StatusCode { get; set; }

        public ProtocolException()
            : base()
        { }
        public ProtocolException(string message)
            : base(message)
        { }
        public ProtocolException(ApiStatusCode statusCode)
            : base()
        {
            this.StatusCode = statusCode;
        }
        public ProtocolException(ApiStatusCode statusCode, string message)
            : base(message)
        {
            this.StatusCode = statusCode;
        }
    }
}
