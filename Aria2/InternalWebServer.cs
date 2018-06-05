using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

namespace Aria2
{
    public class InternalWebServer : IDisposable
    {
        public int Port { get; private set; } = 0;
        public string WebRoot { get; private set; } = "www";
        private HttpListener httpListener = null;
        private Dictionary<string, string> mimeType = new Dictionary<string, string>()
        { 
            //{ "extension", "content type" }
            { ".htm", "text/html" },
            { ".html", "text/html" },
            { ".xml", "text/xml" },
            { ".txt", "text/plain" },
            { ".css", "text/css" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".jpg", "image/jpg" },
            { ".jpeg", "image/jpeg" },
            { ".zip", "application/zip"},
            { ".manifest", "text/cache-manifest" },
            { ".js", "text/javascript" },
            { ".ttf", "font/ttf" },
            { ".woff", "font/woff" },
            { ".woff2", "font/woff" },
            { ".ico", "image/icon" },
            { ".eot", "font/eot" },
        };
        public InternalWebServer(string webRoot, int port)
        {
            this.WebRoot = webRoot;
            this.Port = port;
        }
        /// <summary>
        /// 启动本地网页服务器
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            try
            {
                //监听端口
                httpListener = new HttpListener();
                var host = "http://+:" + Port.ToString() + "/";
                httpListener.Prefixes.Add(host);
                httpListener.Start();
                //Console.WriteLine("Server Start On: " + host);
                httpListener.BeginGetContext(new AsyncCallback(onWebResponse), httpListener);  //开始异步接收request请求
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 网页服务器相应处理
        /// </summary>
        /// <param name="ar"></param>
        private void onWebResponse(IAsyncResult ar)
        {
            try
            {
                byte[] responseByte = null;    //响应数据

                HttpListener httpListener = ar.AsyncState as HttpListener;
                HttpListenerContext context = httpListener.EndGetContext(ar);  //接收到的请求context（一个环境封装体）            

                httpListener.BeginGetContext(new AsyncCallback(onWebResponse), httpListener);  //开始 第二次 异步接收request请求

                HttpListenerRequest request = context.Request;  //接收的request数据
                HttpListenerResponse response = context.Response;  //用来向客户端发送回复

                var path = request.Url.AbsolutePath;

                var file = "";

                if (path == "" || path == "/")
                {
                    file = Path.Combine(WebRoot, "index.html");
                }
                else
                {
                    if (path.StartsWith("/"))
                        path = path.Substring(1);

                    var paths = path.Split('/');

                    path = Path.Combine(paths);

                    if (paths.Last().IndexOf('.') == -1)
                        file = Path.Combine(WebRoot, path, "index.html");
                    else
                        file = Path.Combine(WebRoot, path);
                }

                //处理请求文件名的后缀
                string fileExt = Path.GetExtension(file);

                if (!File.Exists(file))
                {
                    responseByte = Encoding.UTF8.GetBytes("404 Not Found!");
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    responseByte = File.ReadAllBytes(file);
                    response.StatusCode = (int)HttpStatusCode.OK;
                }

                if (mimeType.ContainsKey(fileExt))
                    response.ContentType = mimeType[fileExt];
                else
                    response.ContentType = mimeType["*"];

                response.Cookies = request.Cookies; //处理Cookies

                response.ContentEncoding = Encoding.UTF8;

                using (Stream output = response.OutputStream)  //发送回复
                {
                    output.Write(responseByte, 0, responseByte.Length);
                }
            }
            catch { }
        }

        public void Stop()
        {
            if (httpListener != null)
            {
                httpListener.Stop();
            }
        }

        public void Dispose(object sender, System.EventArgs args)
        {
            this.Dispose();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
