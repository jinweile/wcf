using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Eltc.Base.FrameWork.Helper.Wcf.LoadBalance;
using System.Reflection;
namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfClentConstantSettingConfig
    {
        private IDictionary<string, WcfClentBinding> mlist;

        /// <summary>
        /// 客户端常量设置配值类
        /// </summary>
        /// <param name="doc"></param>
        public WcfClentConstantSettingConfig(XmlDocument doc)
        {
            mlist = new Dictionary<string, WcfClentBinding>();
            try
            {
                foreach (XmlElement elem in XmlHelper.Children(doc.DocumentElement, "Servers"))
                {
                    foreach (XmlElement xe in XmlHelper.Children(elem, "Server"))
                    {
                        var name = xe.GetAttribute("name");

                        XmlElement xe1 = XmlHelper.Child(xe, "Binding");
                        Binding bindType = new NetTcpBinding();
                        WcfClentBinding bing = new WcfClentBinding()
                                     {
                                         BindingType = bindType,
                                         MaxBufferPoolSize = Convert.ToInt32(xe1.Attributes["MaxBufferPoolSize"].Value),
                                         MaxBufferSize = Convert.ToInt32(xe1.Attributes["MaxBufferSize"].Value),
                                         MaxReceivedMessageSize = Convert.ToInt32(xe1.Attributes["MaxReceivedMessageSize"].Value),
                                         MaxConnections = Convert.ToInt32(xe1.Attributes["MaxConnections"].Value),
                                         ListenBacklog = Convert.ToInt32(xe1.Attributes["ListenBacklog"].Value),
                                         SendTimeout = TimeSpan.Parse(xe1.Attributes["SendTimeout"].Value),
                                         OpenTimeout = TimeSpan.Parse(xe1.Attributes["OpenTimeout"].Value),
                                         ReceiveTimeout = TimeSpan.Parse(xe1.Attributes["ReceiveTimeout"].Value),
                                         TransferMode = (TransferMode)Enum.Parse(typeof(TransferMode), xe1.Attributes["TransferMode"].Value),
                                         SecurityMode = (SecurityMode)Enum.Parse(typeof(SecurityMode), xe1.Attributes["SecurityMode"].Value),
                                         Uri = xe1.Attributes["Address"].Value,
                                         ReaderQuotasMaxDepth = Convert.ToInt32(xe1.Attributes["ReaderQuotasMaxDepth"].Value),
                                         ReaderQuotasMaxStringContentLength = Convert.ToInt32(xe1.Attributes["ReaderQuotasMaxStringContentLength"].Value),
                                         ReaderQuotasMaxArrayLength = Convert.ToInt32(xe1.Attributes["ReaderQuotasMaxArrayLength"].Value),
                                         ReaderQuotasMaxBytesPerRead = Convert.ToInt32(xe1.Attributes["ReaderQuotasMaxBytesPerRead"].Value),
                                         ReaderQuotasMaxNameTableCharCount = Convert.ToInt32(xe1.Attributes["ReaderQuotasMaxNameTableCharCount"].Value),
                                         ReliableSessionOrdered = bool.Parse(xe1.Attributes["ReliableSessionOrdered"].Value),
                                         ReliableSessionEnabled = bool.Parse(xe1.Attributes["ReliableSessionEnabled"].Value),
                                         ReliableSessionInactivityTimeout = TimeSpan.Parse(xe1.Attributes["ReliableSessionInactivityTimeout"].Value),
                                         //EnableBinaryFormatterBehavior = bool.Parse(xe1.Attributes["EnableBinaryFormatterBehavior"].Value),
                                         IsUseWcfPool = bool.Parse(xe1.Attributes["IsUseWcfPool"].Value),
                                         WcfMaxPoolSize = Convert.ToInt32(xe1.Attributes["WcfMaxPoolSize"].Value),
                                         WcfOutTime = Convert.ToInt64(xe1.Attributes["WcfOutTime"].Value),
                                         WcfFailureTime = Convert.ToInt64(xe1.Attributes["WcfFailureTime"].Value),
                                         WcfPoolMonitorReapTime = Convert.ToInt32(xe1.Attributes["WcfPoolMonitorReapTime"].Value)
                                     };

                        //获取负载均衡信息
                        XmlElement xe2 = XmlHelper.Child(xe, "LoadBalance");
                        LoadBalanceConfig config = new LoadBalanceConfig();
                        if (xe2 == null) { config.IsUsed = false; }
                        else
                        {
                            config.IsUsed = bool.Parse(xe2.Attributes["IsUsed"].Value);
                            config.IsUseWcfPool = bool.Parse(xe2.Attributes["IsUseWcfPool"].Value);
                            config.WcfOutTime = long.Parse(xe2.Attributes["WcfOutTime"].Value);
                            config.WcfFailureTime = long.Parse(xe2.Attributes["WcfFailureTime"].Value);
                            config.WcfPoolMonitorReapTime = int.Parse(xe2.Attributes["WcfPoolMonitorReapTime"].Value);
                            IDictionary<string, Address> addresslist = new Dictionary<string, Address>();
                            foreach (XmlElement xe3 in XmlHelper.Children(xe2, "Address"))
                            {
                                Address address = new Address();
                                address.WcfMaxPoolSize = int.Parse(xe3.Attributes["WcfMaxPoolSize"].Value);
                                address.HeatBeatTime = TimeSpan.Parse(xe3.Attributes["HeatBeatTime"].Value);
                                address.Uri = xe3.InnerText.Trim();

                                addresslist.Add(address.Uri, address);
                            }
                            config.WcfAdress = addresslist;
                            config.BalanceAlgorithm = (BalanceAlgorithmBase)this.GetType().Assembly.CreateInstance(
                                                        "Eltc.Base.FrameWork.Helper.Wcf.LoadBalance."
                                                        + xe2.Attributes["BalanceAlgorithm"].Value);
                        }

                        bing.LoadBalance = config;

                        mlist[name] = bing;
                    }
                }
            }
            catch (Exception oe)
            {
                throw new ArgumentException(oe.Message);
            }
        }

        /// <summary>
        /// 读取列表配置
        /// </summary>
        public IDictionary<string, WcfClentBinding> WcfClentConstantList
        {
            get
            {
                return mlist;
            }
        }

    }

    /// <summary>
    /// 负载均衡信息
    /// </summary>
    internal class LoadBalanceConfig
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// 负载均衡算法
        /// </summary>
        public BalanceAlgorithmBase BalanceAlgorithm { get; set; }

        /// <summary>
        /// 是否启用wcf连接池
        /// </summary>
        public bool IsUseWcfPool { get; set; }

        /// <summary>
        /// Wcf获取连接过期时间
        /// </summary>
        public long WcfOutTime
        {
            get;
            set;
        }

        /// <summary>
        /// Wcf连接失效时间
        /// </summary>
        public long WcfFailureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Wcf连接池监控线程扫描间隔时间（秒为单位）
        /// </summary>
        public int WcfPoolMonitorReapTime
        {
            get;
            set;
        }

        /// <summary>
        /// 负载均衡地址信息
        /// </summary>
        public IDictionary<string, Address> WcfAdress { get; set; }
    }

    /// <summary>
    /// 负载均衡地址信息
    /// </summary>
    internal class Address
    {
        /// <summary>
        /// 连接池最大值
        /// </summary>
        public int WcfMaxPoolSize { get; set; }

        /// <summary>
        /// 心跳扫描时间间隔
        /// </summary>
        public TimeSpan HeatBeatTime { get; set; }

        /// <summary>
        /// wcf服务地址
        /// </summary>
        public string Uri { get; set; }
    }

    /// <summary>
    /// 客户端常量配值实体
    /// </summary>
    internal class WcfClentBinding
    {
        public Binding BindingType
        {
            get;
            set;
        }

        public int MaxBufferPoolSize
        {
            get;
            set;
        }

        public int MaxBufferSize
        {
            get;
            set;
        }

        public int MaxReceivedMessageSize
        {
            get;
            set;
        }
        /// <summary>
        /// 客户端池子最大缓存数目
        /// </summary>
        public int MaxConnections
        {
            get;
            set;
        }

        public int ListenBacklog
        {
            get;
            set;
        }
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan OpenTimeout
        {
            get;
            set;
        }
        public TimeSpan ReceiveTimeout
        {
            get;
            set;
        }
        public TransferMode TransferMode
        {
            get;
            set;
        }
        public SecurityMode SecurityMode
        {
            get;
            set;
        }

        public string Uri
        {
            get;
            set;
        }

        public int ReaderQuotasMaxDepth { get; set; }

        public int ReaderQuotasMaxStringContentLength { get; set; }

        public int ReaderQuotasMaxArrayLength { get; set; }

        public int ReaderQuotasMaxBytesPerRead { get; set; }

        public int ReaderQuotasMaxNameTableCharCount { get; set; }

        public bool ReliableSessionOrdered { get; set; }

        public bool ReliableSessionEnabled { get; set; }

        public TimeSpan ReliableSessionInactivityTimeout { get; set; }

        /// <summary>
        /// 是否启用自定义序列化
        /// </summary>
        public bool EnableBinaryFormatterBehavior { get; set; }

        /// <summary>
        /// 是否启用Wcf连接池
        /// </summary>
        public bool IsUseWcfPool
        {
            get;
            set;
        }

        /// <summary>
        /// Wcf连接池最大值
        /// </summary>
        public int WcfMaxPoolSize
        {
            get;
            set;
        }

        /// <summary>
        /// Wcf获取连接过期时间
        /// </summary>
        public long WcfOutTime
        {
            get;
            set;
        }

        /// <summary>
        /// Wcf连接失效时间
        /// </summary>
        public long WcfFailureTime
        {
            get;
            set;
        }

        /// <summary>
        /// Wcf连接池监控线程扫描间隔时间（秒为单位）
        /// </summary>
        public int WcfPoolMonitorReapTime
        {
            get;
            set;
        }

        /// <summary>
        /// 负载均衡信息
        /// </summary>
        public LoadBalanceConfig LoadBalance { get; set; }
    }
}
