using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Collections;

using Eltc.Base.FrameWork.Helper.Wcf.Monitor;
using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;

namespace Eltc.Base.FrameWork
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                            ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class CommonWcfBll
    {
        /// <summary>
        /// 获取客户端信息，备用记录日志
        /// </summary>
        public CommonWcfBll()
        {
            #region
            OperationContext context = OperationContext.Current;
            if (context != null)
            {
                //获取客户端请求的路径
                string AbsolutePath = context.EndpointDispatcher.EndpointAddress.Uri.AbsolutePath;
                if (!AbsolutePath.Contains("Eltc.Base/FrameWork/Helper/Wcf"))
                {
                    //获取客户端ip和端口
                    MessageProperties properties = context.IncomingMessageProperties;
                    RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    string client_ip = endpoint.Address;
                    //int client_port = endpoint.Port;

                    //获取客户端请求的契约信息
                    //string contract_name = context.EndpointDispatcher.ContractName;
                    //获取客户端请求的路径
                    //Uri request_uri = context.EndpointDispatcher.EndpointAddress.Uri;
                    string sessionid = context.SessionId;
                    string wcfappname = HeaderOperater.GetServiceWcfAppNameHeader(context);
                    wcfappname = wcfappname == null ? "" : wcfappname;
                    context.Channel.Closed += (object sender, EventArgs e) =>
                    {
                        //Console.WriteLine(sessionid + "请求结束:" + client_ip + ":" + client_port + "->" + request_uri.AbsolutePath);
                        MonitorData.Instance.UpdateUrlConnNums(client_ip + "_" + wcfappname, AbsolutePath, false);
                    };

                    //Console.WriteLine(sessionid + "请求开始:" + client_ip + ":" + client_port + "->" + request_uri.AbsolutePath);
                    Hashtable ht = new Hashtable();
                    ht.Add("ip", client_ip + "_" + wcfappname);
                    ht.Add("url", AbsolutePath);
                    ht.Add("isadd", true);
                    Thread th = new Thread(new ParameterizedThreadStart(Run));
                    th.Start(ht);
                }
            }
            #endregion
        }

        private void Run(object operateht)
        {
            Hashtable ht = (Hashtable)operateht;
            MonitorData.Instance.UpdateUrlConnNums((string)ht["ip"], (string)ht["url"], (bool)ht["isadd"]);

            Thread.CurrentThread.Abort();
        }

    }
}
