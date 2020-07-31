using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace SinKingMusicSnalysis.Common
{
    class Http
    {
        private void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
        public bool gzip = false;
        public WebHeaderCollection Headers = new WebHeaderCollection();
        public bool AllowAutoRedirect = false;
        public string Send(string url, string post = null, string referer = null, string cookie = null, string[] header = null, string ua = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (header != null)
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        this.SetHeaderValue(request.Headers, header[i].Split(':')[0], header[i].Split(':')[1]);
                    }
                }
                if (post != null)
                {
                    request.Method = "POST";
                }
                else
                {
                    request.Method = "GET";
                    request.Proxy = null;
                    request.KeepAlive = false;
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                }

                if (referer != null)
                {
                    request.Referer = referer;
                }
                else
                {
                    request.Referer = url;
                }
                if (ua == null)
                {
                    request.UserAgent = "Mozilla/5.0 (Linux; U; Android 4.4.1; zh-cn) AppleWebKit/533.1 (KHTML, like Gecko)Version/4.0 MQQBrowser/5.5 Mobile Safari/533.1";
                }
                else
                {
                    request.UserAgent = ua;
                }
                if (cookie != null)
                {
                    request.Headers.Add("Cookie", cookie);
                }
                if (post != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(post);
                    request.ContentLength = data.Length;
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                request.AllowAutoRedirect = this.AllowAutoRedirect;
                SetHeaderValue(request.Headers, "Accept-Encoding", "gzip,deflate,sdch");
                SetHeaderValue(request.Headers, "Accept-Language", "zh-CN,zh;q=0.8");
                SetHeaderValue(request.Headers, "Connection", "close");
                WebResponse myResponse;
                try
                {
                    myResponse = request.GetResponse();
                }
                catch (WebException ex)
                {
                    myResponse = ex.Response;
                }
                using StreamReader reader = new StreamReader((this.gzip? new System.IO.Compression.GZipStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress) : myResponse.GetResponseStream()), Encoding.UTF8);
                string content = reader.ReadToEnd();
                this.Headers = myResponse.Headers;
                myResponse.Close();
                return content;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
