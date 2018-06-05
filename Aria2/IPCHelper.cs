using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading.Tasks;

namespace Aria2
{
    public class IPCHelper
    {
        private class IPCObject : MarshalByRefObject
        {
            public void Send(string data)
            {
                IPCHandler.Receive(data);
            }
        }
        private class IPCHandler
        {
            private IPCHandler() { }
            public event EventHandler<string> DataReceived;
            private static IPCHandler handler = null;
            public static IPCHandler GetHandler()
            {
                if (handler == null)
                    handler = new IPCHandler();
                return handler;
            }
            public static void Receive(string data)
            {
                handler.DataReceived(handler, data);
            }
        }
        private IChannel channel = null;
        private IPCObject myObj = null;
        public event EventHandler<string> DataReceived
        {
            add { IPCHandler.GetHandler().DataReceived += value; }
            remove { IPCHandler.GetHandler().DataReceived -= value; }
        }
        public IPCHelper Server(string portName, string uri)
        {
            Task.Run(() =>
            {
                channel = new IpcChannel(portName);
                ChannelServices.RegisterChannel(channel, false);
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(IPCObject), uri, WellKnownObjectMode.Singleton);
            });
            return this;
        }
        public IPCHelper Client(string url)
        {
            channel = new IpcChannel();
            ChannelServices.RegisterChannel(channel, false);
            myObj = (IPCObject)Activator.GetObject(typeof(IPCObject), url);
            return this;
        }
        public void Send(string data)
        {
            if (myObj != null)
                myObj.Send(data);
        }
    }
}
