using System;
using System.Net;
using System.Linq;
using System.Net.NetworkInformation;

namespace Aria2
{
    public class PortHelper
    {
        public int[] GetUsedPorts()
        {
            //获取本地计算机的网络连接和通信统计数据的信息            
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            //返回本地计算机上的所有Tcp监听程序            
            IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();
            //返回本地计算机上的所有UDP监听程序            
            IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();
            //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。   
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            return ipsTCP.Select(en => en.Port).Concat(tcpConnInfoArray.Select(en => en.LocalEndPoint.Port)).ToArray();
        }
        public int GetRandPort()
        {
            var usedPorts = GetUsedPorts();
            var port = new Random().Next(1024, 65535);
            while (usedPorts.Contains(port))
            {
                port = new Random().Next(1024, 65535);
            }
            return port;
        }
    }
}
