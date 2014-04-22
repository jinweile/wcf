using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.Reflection;

using Eltc.Base.FrameWork.Helper.Wcf;
using Eltc.Base.FrameWork.Helper.Wcf.LoadBalance;
using Eltc.Base.FrameWork.Helper.Wcf.SerializeBehavior;
using Eltc.Base.FrameWork.Helper.Wcf.Monitor;

namespace Eltc.Base.FrameWork.Helper
{
    #region delegate

    /// <summary>
    /// WCF错误
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WcfFaulted(object sender, EventArgs e);

    /// <summary>
    /// WCF关闭
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void WcfClosed(object sender, EventArgs e);

    /// <summary>
    /// 请求前处理
    /// </summary>
    /// <param name="operationName">请求的方法名</param>
    /// <param name="inputs">输入参数</param>
    /// <param name="AbsolutePath">请求的address</param>
    public delegate void WcfBeforeCall(string operationName, object[] inputs, string AbsolutePath, object correlationState);

    /// <summary>
    /// 请求结束后处理
    /// </summary>
    /// <param name="operationName">请求的方法名</param>
    /// <param name="outputs">输出参数</param>
    /// <param name="returnValue">返回值</param>
    /// <param name="correlationState">状态</param>
    /// <param name="AbsolutePath">请求的address</param>
    public delegate void WcfAfterCall(string operationName, object[] outputs, object returnValue, object correlationState, string AbsolutePath);

    #endregion

    public sealed class WcfServer
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static volatile WcfServer instance = null;
        private static volatile object lockhelper = new object();

        /// <summary>
        /// 获取单例
        /// </summary>
        /// <returns></returns>
        public static WcfServer GetInstance()
        {
            if (instance == null)
            {
                lock (lockhelper)
                {
                    if (instance == null)
                    {
                        instance = new WcfServer();
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// 私有构造
        /// </summary>
        private WcfServer()
        {
            this.services = new List<ServiceHost>();

            //加载配置文件
            WcfServiceSetting config = new WcfServiceSetting();
            this.wcfSetting = config.WcfSetting;
            this.constantSetting = config.ConstantSetting;
        }

        private List<ServiceHost> services = null;
        private WcfSettingConfig wcfSetting = null;
        private WcfConstantSettingConfig constantSetting = null;
        /// <summary>
        /// WCF错误事件
        /// </summary>
        public event WcfFaulted WcfFaultedEvent;
        /// <summary>
        /// WCF关闭事件
        /// </summary>
        public event WcfClosed WcfClosedEvent;
        /// <summary>
        /// Wcf请求之前处理事件
        /// </summary>
        public event WcfBeforeCall WcfBeforeCallEvent;
        /// <summary>
        /// Wcf请求结束后处理事件
        /// </summary>
        public event WcfAfterCall WcfAfterCallEvent;

        private bool isStop = true;

        /// <summary>
        /// 开始服务
        /// </summary>
        public void Start()
        {
            if (constantSetting == null)
            {
                throw new Exception("常量配置错误...");
            }
            if (wcfSetting == null)
            {
                throw new Exception("服务配置错误...");
            }

            isStop = false;

            #region 指定bing模式
            NetTcpBinding bing = new NetTcpBinding();
            bing.CloseTimeout = constantSetting.CloseTimeout;
            bing.OpenTimeout = constantSetting.OpenTimeout;
            bing.ReceiveTimeout = constantSetting.ReceiveTimeout;
            bing.SendTimeout = constantSetting.SendTimeout;
            bing.TransactionFlow = constantSetting.TransactionFlow;
            bing.TransferMode = constantSetting.TransferMode;
            bing.TransactionProtocol = constantSetting.TransactionProtocol;
            bing.HostNameComparisonMode = constantSetting.HostNameComparisonMode;
            bing.ListenBacklog = constantSetting.ListenBacklog;
            bing.MaxBufferPoolSize = constantSetting.MaxBufferPoolSize;
            bing.MaxBufferSize = constantSetting.MaxBufferSize;
            bing.MaxConnections = constantSetting.MaxConnections;
            bing.MaxReceivedMessageSize = constantSetting.MaxReceivedMessageSize;
            bing.Security.Mode = constantSetting.Securitymode;

            bing.PortSharingEnabled = constantSetting.PortSharingEnabled;
            bing.ReaderQuotas.MaxStringContentLength = constantSetting.MaxStringContentLength;
            bing.ReaderQuotas.MaxDepth = constantSetting.MaxDepth;
            bing.ReaderQuotas.MaxArrayLength = constantSetting.MaxArrayLength;
            bing.ReaderQuotas.MaxBytesPerRead = constantSetting.MaxBytesPerRead;
            bing.ReaderQuotas.MaxNameTableCharCount = constantSetting.MaxNameTableCharCount;
            bing.Security.Message.ClientCredentialType = constantSetting.ClientCredentialType;
            bing.ReliableSession.Enabled = constantSetting.ReliableSessionEnabled;
            bing.ReliableSession.Ordered = constantSetting.ReliableSessionOrdered;
            bing.ReliableSession.InactivityTimeout = constantSetting.ReliableSessionInactivityTimeout;
            #endregion

            #region behavior模式
            ServiceDebugBehavior debugbehavior = new ServiceDebugBehavior();
            debugbehavior.IncludeExceptionDetailInFaults = constantSetting.IncludeExceptionDetailInFaults;
            ServiceThrottlingBehavior throtbehavior = new ServiceThrottlingBehavior();
            throtbehavior.MaxConcurrentCalls = constantSetting.MaxConcurrentCalls;
            throtbehavior.MaxConcurrentInstances = constantSetting.MaxConcurrentInstances;
            throtbehavior.MaxConcurrentSessions = constantSetting.MaxConcurrentSessions;
            #endregion

            //加入心跳服务
            ServicePoint heatpoint = new ServicePoint();
            heatpoint.Address = "Eltc.Base/FrameWork/Helper/Wcf/LoadBalance/IHeatBeat";
            heatpoint.Contract = typeof(IHeatBeat);
            heatpoint.MaxItemsInObjectGraph = null;
            heatpoint.Name = typeof(HeatBeat);
            wcfSetting.List.Add(heatpoint);

            //加入监控服务
            ServicePoint monitorpoint = new ServicePoint();
            monitorpoint.Address = "Eltc.Base/FrameWork/Helper/Wcf/Monitor/IMonitorControl";
            monitorpoint.Contract = typeof(IMonitorControl);
            monitorpoint.MaxItemsInObjectGraph = null;
            monitorpoint.Name = typeof(MonitorControl);
            wcfSetting.List.Add(monitorpoint);

            //开始寄宿
            foreach (ServicePoint point in wcfSetting.List)
            {
                OpenHost(point, debugbehavior, throtbehavior, bing, constantSetting.enableBinaryFormatterBehavior);
            }
        }

        /// <summary>
        /// host
        /// </summary>
        /// <param name="point"></param>
        /// <param name="debugbehavior"></param>
        /// <param name="throtbehavior"></param>
        /// <param name="bing"></param>
        private void OpenHost(ServicePoint point, ServiceDebugBehavior debugbehavior, ServiceThrottlingBehavior throtbehavior, NetTcpBinding bing, bool EnableBinaryFormatterBehavior)
        {
            ServiceHost host = new ServiceHost(point.Name, new Uri("net.tcp://" + constantSetting.BaseAddress));

            #region behavior
            if (host.Description.Behaviors.Find<ServiceDebugBehavior>() != null)
                host.Description.Behaviors.Remove<ServiceDebugBehavior>();
            if (host.Description.Behaviors.Find<ServiceThrottlingBehavior>() != null)
                host.Description.Behaviors.Remove<ServiceThrottlingBehavior>();

            host.Description.Behaviors.Add(debugbehavior);
            host.Description.Behaviors.Add(throtbehavior);

            //大数据量传输时必须设定此参数
            if (point.MaxItemsInObjectGraph != null)
            {
                if (host.Description.Behaviors.Find<DataContractSerializerOperationBehavior>() != null)
                    host.Description.Behaviors.Remove<DataContractSerializerOperationBehavior>();
                //通过反射指定MaxItemsInObjectGraph属性(传输大数据时使用)
                object obj = typeof(ServiceHost).Assembly.CreateInstance(
                        "System.ServiceModel.Dispatcher.DataContractSerializerServiceBehavior"
                        , true, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic
                        , null, new object[] { false, (int)point.MaxItemsInObjectGraph }, null, null);
                IServiceBehavior datacontractbehavior = obj as IServiceBehavior;
                host.Description.Behaviors.Add(datacontractbehavior);
            }
            #endregion

            host.AddServiceEndpoint(point.Contract, bing, (point.Address.StartsWith("/") ? point.Address.TrimStart('/') : point.Address));

            //自定义二进制序列化器
            if (EnableBinaryFormatterBehavior)
            {
                System.ServiceModel.Description.ServiceEndpoint spoint = host.Description.Endpoints.Count == 1 ? host.Description.Endpoints[0] : null;
                if (spoint != null && spoint.Behaviors.Find<BinaryFormatterBehavior>() == null)
                {
                    BinaryFormatterBehavior serializeBehavior = new BinaryFormatterBehavior();
                    spoint.Behaviors.Add(serializeBehavior);
                }
            }

            #region 增加拦截器处理
            if (point.Address != "Eltc.Base/FrameWork/Helper/Wcf/LoadBalance/IHeatBeat" && point.Address != "Eltc.Base/FrameWork/Helper/Wcf/Monitor/IMonitorControl")
            {
                int endpointscount = host.Description.Endpoints.Count;
                WcfParameterInspector wcfpi = new WcfParameterInspector();
                wcfpi.WcfAfterCallEvent += new Wcf.WcfAfterCall((operationName, outputs, returnValue, correlationState, AbsolutePath) =>
                {
                    if (WcfAfterCallEvent != null)
                    {
                        WcfAfterCallEvent(operationName, outputs, returnValue, correlationState, AbsolutePath);
                    }
                });
                wcfpi.WcfBeforeCallEvent += new Wcf.WcfBeforeCall((operationName, inputs, AbsolutePath, correlationState) =>
                {
                    if (WcfBeforeCallEvent != null)
                    {
                        WcfBeforeCallEvent(operationName, inputs, AbsolutePath, correlationState);
                    }
                });
                for (int i = 0; i < endpointscount; i++)
                {
                    if (host.Description.Endpoints[i].Contract.Name != "IMetadataExchange")
                    {
                        int Operationscount = host.Description.Endpoints[i].Contract.Operations.Count;
                        for (int j = 0; j < Operationscount; j++)
                        {
                            host.Description.Endpoints[i].Contract.Operations[j].Behaviors.Add(wcfpi);
                        }
                    }
                }
            }
            #endregion

            #region 注册事件
            //错误状态处理
            host.Faulted += new EventHandler((sender, e) =>
            {
                if (WcfFaultedEvent != null)
                {
                    WcfFaultedEvent(sender, e);
                }
            });
            //关闭状态处理
            host.Closed += new EventHandler((sender, e) =>
            {
                if (WcfClosedEvent != null)
                {
                    WcfClosedEvent(sender, e);
                }

                //如果意外关闭，再次打开监听
                if (isStop)
                    return;

                services.Remove(host);
                OpenHost(point, debugbehavior, throtbehavior, bing, EnableBinaryFormatterBehavior);
            });
            #endregion

            host.Open();
            services.Add(host);
        }

        //释放单例
        private void ReleaseInstance()
        {
            if (instance != null)
            {
                lock (lockhelper)
                {
                    if (instance != null)
                    {
                        instance = null;
                    }
                }
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            isStop = true;
            foreach (ServiceHost service in services)
            {
                if (service.State != CommunicationState.Closed)
                {
                    service.Close();
                }
            }
            services = null;
            wcfSetting = null;
            constantSetting = null;
            WcfFaultedEvent = null;
            ReleaseInstance();
        }

    }
}
