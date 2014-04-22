using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Eltc.Base.FrameWork.Helper.Wcf.Monitor
{
    public class MonitorClient : IDisposable
    {
        private ChannelFactory<IMonitorControl> factory = null;
        private IMonitorControl proxy = null;

        private MonitorClient() { }

        public static MonitorClient Instance
        {
            get
            {
                return new MonitorClient();
            }
        }

        public IMonitorControl getProxy(string uri, NetTcpBinding binding)
        {
            factory = new ChannelFactory<IMonitorControl>(binding);
            EndpointAddress point = new EndpointAddress("net.tcp://" + uri + "/Eltc.Base/FrameWork/Helper/Wcf/Monitor/IMonitorControl");
            proxy = factory.CreateChannel(point);

            return proxy;
        }

        public void Dispose()
        {
            if (proxy != null)
                (proxy as IDisposable).Dispose();
            if (factory != null)
                (factory as IDisposable).Dispose();
        }
    }
}
