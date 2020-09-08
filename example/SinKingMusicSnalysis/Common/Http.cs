using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;

namespace SinKingMusicSnalysis.Common
{
    public class Http
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
        public string SendNormal(string url, string post = null, string referer = null, string cookie = null, string[] header = null, string ua = null)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = this.AllowAutoRedirect;
                SetHeaderValue(request.Headers, "Accept-Encoding", "gzip,deflate,sdch");
                SetHeaderValue(request.Headers, "Accept-Language", "zh-CN,zh;q=0.8");
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
                    byte[] data = Encoding.UTF8.GetBytes(post);
                    request.ContentLength = data.Length;
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
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
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
                }
                else
                {
                    request.UserAgent = ua;
                }
                if (cookie != null)
                {
                    request.Headers.Add("Cookie", cookie);
                }
                WebResponse myResponse;
                try
                {
                    myResponse = request.GetResponse();
                }
                catch (WebException ex)
                {
                    myResponse = ex.Response;
                }
                StreamReader reader;
                try
                {
                    reader = new StreamReader(this.gzip ? new System.IO.Compression.GZipStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress) : myResponse.GetResponseStream(), Encoding.UTF8);
                }
                catch
                {
                    reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                }
                string content = reader.ReadToEnd();
                reader.Close();
                this.Headers = myResponse.Headers;
                myResponse.Close();
                if (string.IsNullOrEmpty(content)) content = "";
                return content;
            }
            catch (WebException ex)
            {
                StreamReader reader;
                try
                {
                    reader = new StreamReader(this.gzip ? new System.IO.Compression.GZipStream(ex.Response.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress) : ex.Response.GetResponseStream(), Encoding.UTF8);
                }
                catch
                {
                    reader = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
                }
                string content = reader.ReadToEnd();
                reader.Close();
                this.Headers = ex.Response.Headers;
                ex.Response.Close();
                if (string.IsNullOrEmpty(content)) content = "";
                return content;
            }
        }
        public string Location(string URL)
        {
            HttpClientHandler hander = new HttpClientHandler();
            hander.AllowAutoRedirect = false;
            HttpClient client = new HttpClient(hander);
            return client.GetAsync(URL).Result.Headers.Location.ToString();
        }
        public string Send(string url, string post = null, string referer = null, string cookie = null, string[] header = null, string ua = null)
        {
            System.Threading.Tasks.Task<string> res = SendAsync(url, post, referer, cookie, header, ua);
            return res.Result;
        }
        private async System.Threading.Tasks.Task<string> SendAsync(string url, string post = null, string referer = null, string cookie = null, string[] header = null, string ua = null)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback = ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = gzip ? DecompressionMethods.GZip : DecompressionMethods.All,
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    ServerCertificateCustomValidationCallback = delegate { return true; }
                };
                HttpClient client = new HttpClient(handler);
                var postdata = new Dictionary<string, string>();
                if (post != null)
                {

                    foreach (var item in post.Split("&"))
                    {
                        var arr = item.Split("=");
                        postdata.Add(arr[0], arr[1]);
                    }
                }
                var content = new FormUrlEncodedContent(postdata);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate,sdch");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.8");
                if (header != null)
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        try
                        {
                            client.DefaultRequestHeaders.Add(header[i].Split(':')[0], header[i].Split(':')[1]);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                if (referer != null)
                {
                    client.DefaultRequestHeaders.Add("referer", referer);
                }
                else
                {
                    client.DefaultRequestHeaders.Add("referer", url);
                }

                if (ua == null)
                {
                    client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Linux; U; Android 4.4.1; zh-cn) AppleWebKit/533.1 (KHTML, like Gecko)Version/4.0 MQQBrowser/5.5 Mobile Safari/533.1");
                }
                else
                {
                    client.DefaultRequestHeaders.Add("UserAgent", ua);
                }
                if (cookie != null)
                {
                    client.DefaultRequestHeaders.Add("Cookie", cookie);
                }
                if (post != null)
                {
                    var response = await client.PostAsync(url, content);
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
                else
                {
                    var response = await client.GetAsync(url);
                    var responseString = await response.Content.ReadAsStringAsync();
                    return responseString;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
