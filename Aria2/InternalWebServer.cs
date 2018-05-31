using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.NetworkInformation;

namespace Aria2
{
    public class InternalWebServer : IDisposable
    {
        private int port = 0;
        private string webRoot = "www";
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
        public InternalWebServer(string webRoot)
        {
            this.webRoot = webRoot;
        }
        /// <summary>
        /// 启动本地网页服务器
        /// </summary>
        /// <returns></returns>
        public int Start()
        {
            try
            {
                //监听端口
                httpListener = new HttpListener();
                port = getPort();
                var host = "http://+:" + port.ToString() + "/";
                httpListener.Prefixes.Add(host);
                httpListener.Start();
                Console.WriteLine("Server Start On: " + host);
                httpListener.BeginGetContext(new AsyncCallback(onWebResponse), httpListener);  //开始异步接收request请求
            }
            catch (Exception ex)
            {
            }
            return port;
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
                    file = Path.Combine(webRoot, "index.html");
                }
                else
                {
                    if (path.StartsWith("/"))
                        path = path.Substring(1);

                    var paths = path.Split('/');

                    path = Path.Combine(paths);

                    if (paths.Last().IndexOf('.') == -1)
                        file = Path.Combine(webRoot, path, "index.html");
                    else
                        file = Path.Combine(webRoot, path);
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
        private int getPort()
        {
            //获取本地计算机的网络连接和通信统计数据的信息            
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            //返回本地计算机上的所有Tcp监听程序            
            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
            //返回本地计算机上的所有UDP监听程序            
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
            //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。   
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            var usedPorts = ipsTCP.Select(en => en.Port).Concat(tcpConnInfoArray.Select(en => en.LocalEndPoint.Port));

            var port = new Random().Next(1024, 65535);
            while (usedPorts.Contains(port))
            {
                port = new Random().Next(1024, 65535);
            }
            return port;
        }
    }
}
