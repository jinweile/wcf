using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfConstantSettingConfig
    {
        #region 内部字段
        private int _maxDepth;
        private int _maxStringContentLength;
        private int _maxArrayLength;
        private int _maxBytesPerRead;
        private int _maxNameTableCharCount;

        private bool _reliableSessionEnabled;
        private bool _reliableSessionOrdered;
        private TimeSpan _reliableSessionInactivityTimeout;

        private Uri _addres;
        private Binding _binding;
        private TimeSpan _closeTimeout;
        private TimeSpan _openTimeout;
        private TimeSpan _receiveTimeout;
        private TimeSpan _sendTimeout;
        private bool _transactionFlow;
        private TransferMode _transferMode;
        private string _transactionProtocol;
        private HostNameComparisonMode _hostNameComparisonMode;
        private int _listenBacklog;
        private int _maxBufferPoolSize;
        private int _maxBufferSize;
        private int _maxConnections;
        private int _maxReceivedMessageSize;
        private bool _portSharingEnabled;
        private SecurityMode _securitymode;
        private MessageCredentialType _clientCredentialType;
        private bool _enableBinaryFormatterBehavior;

        private bool _includeExceptionDetailInFaults;

        private int _maxConcurrentCalls;
        private int _maxConcurrentInstances;
        private int _maxConcurrentSessions;

        //private int _maxItemsInObjectGraph;

        #endregion

        /// <summary>
        /// wcf服务端常量设置文件
        /// </summary>
        /// <param name="doc"></param>
        public WcfConstantSettingConfig(XmlDocument doc)
        {
            try
            {
                foreach (XmlNode elem in XmlHelper.Children(doc.DocumentElement, "NetTcpBinding"))
                {
                    XmlElement xe = XmlHelper.Child(elem, "readerQuotas");
                    int.TryParse(xe.GetAttribute("maxDepth"), out _maxDepth);
                    int.TryParse(xe.GetAttribute("maxStringContentLength"), out _maxStringContentLength);
                    int.TryParse(xe.GetAttribute("maxArrayLength"), out _maxArrayLength);
                    int.TryParse(xe.GetAttribute("maxBytesPerRead"), out _maxBytesPerRead);
                    int.TryParse(xe.GetAttribute("maxNameTableCharCount"), out _maxNameTableCharCount);

                    XmlElement xea = XmlHelper.Child(elem, "reliableSession");
                    Boolean.TryParse(xea.GetAttribute("enabled"), out _reliableSessionEnabled);
                    Boolean.TryParse(xea.GetAttribute("ordered"), out _reliableSessionOrdered);
                    TimeSpan.TryParse(xea.GetAttribute("inactivityTimeout"), out _reliableSessionInactivityTimeout);

                    XmlElement xe1 = XmlHelper.Child(elem, "host");
                    _addres = new Uri(xe1.GetAttribute("baseAddress"));
                    _binding = new NetTcpBinding();

                    XmlElement xe2 = XmlHelper.Child(elem, "behaviors");
                    TimeSpan.TryParse(xe2.GetAttribute("closeTimeout"), out _closeTimeout);
                    TimeSpan.TryParse(xe2.GetAttribute("openTimeout"), out _openTimeout);
                    TimeSpan.TryParse(xe2.GetAttribute("receiveTimeout"), out _receiveTimeout);
                    TimeSpan.TryParse(xe2.GetAttribute("sendTimeout"), out _sendTimeout);
                    Boolean.TryParse(xe2.GetAttribute("transactionFlow"), out _transactionFlow);
                    _transferMode = (TransferMode)Enum.Parse(typeof(TransferMode), xe2.GetAttribute("transferMode"));
                    _transactionProtocol = xe2.GetAttribute("transactionProtocol");
                    _hostNameComparisonMode = (HostNameComparisonMode)Enum.Parse(typeof(HostNameComparisonMode), xe2.GetAttribute("hostNameComparisonMode"));
                    int.TryParse(xe2.GetAttribute("listenBacklog"), out  _listenBacklog);
                    int.TryParse(xe2.GetAttribute("maxBufferPoolSize"), out _maxBufferPoolSize);
                    int.TryParse(xe2.GetAttribute("maxBufferSize"), out _maxBufferSize);
                    int.TryParse(xe2.GetAttribute("maxConnections"), out _maxConnections);
                    int.TryParse(xe2.GetAttribute("maxReceivedMessageSize"), out _maxReceivedMessageSize);
                    bool.TryParse(xe2.GetAttribute("portSharingEnabled"), out _portSharingEnabled);
                    _securitymode = (SecurityMode)Enum.Parse(typeof(SecurityMode), xe2.GetAttribute("securitymode"));
                    _clientCredentialType = (MessageCredentialType)Enum.Parse(typeof(MessageCredentialType), xe2.GetAttribute("clientCredentialType"));
                    //_enableBinaryFormatterBehavior = bool.Parse(xe2.GetAttribute("enableBinaryFormatterBehavior"));

                    XmlElement xe3 = XmlHelper.Child(elem, "serviceDebug");
                    Boolean.TryParse(xe3.GetAttribute("includeExceptionDetailInFaults"), out _includeExceptionDetailInFaults);

                    XmlElement xe4 = XmlHelper.Child(elem, "serviceThrottling");
                    int.TryParse(xe4.GetAttribute("maxConcurrentCalls"), out _maxConcurrentCalls);
                    int.TryParse(xe4.GetAttribute("maxConcurrentInstances"), out _maxConcurrentInstances);
                    int.TryParse(xe4.GetAttribute("maxConcurrentSessions"), out _maxConcurrentSessions);

                    //XmlElement xe5 = XmlHelper.Child(elem, "dataContractSerializer");
                    //int.TryParse(xe5.GetAttribute("maxItemsInObjectGraph"), out _maxItemsInObjectGraph);
                }
            }
            catch (Exception oe)
            {
                throw new ArgumentException(oe.Message);
            }
        }

        #region readerQuotas
        public int MaxDepth
        {
            get
            {
                return _maxDepth;
            }
        }

        public int MaxStringContentLength
        {
            get
            {
                return _maxStringContentLength;
            }
        }

        public int MaxArrayLength
        {
            get
            {
                return _maxArrayLength;
            }
        }

        public int MaxBytesPerRead
        {
            get
            {
                return _maxBytesPerRead;
            }
        }

        public int MaxNameTableCharCount
        {
            get
            {
                return _maxNameTableCharCount;
            }
        }

        #endregion

        #region reliableSession

        public bool ReliableSessionEnabled
        {
            get
            {
                return _reliableSessionEnabled;
            }
        }
        public bool ReliableSessionOrdered
        {
            get { return _reliableSessionOrdered; }
        }
        public TimeSpan ReliableSessionInactivityTimeout
        {
            get { return _reliableSessionInactivityTimeout; }
        }

        #endregion

        #region host
        public Uri BaseAddress
        {
            get
            {
                return _addres;
            }
        }

        public Binding Binding
        {
            get
            {
                return _binding;
            }
        }
        #endregion

        #region behaviors
        public TimeSpan CloseTimeout
        {
            get
            {
                return _closeTimeout;
            }
        }
        public TimeSpan OpenTimeout
        {
            get
            {
                return _openTimeout;
            }
        }
        public TimeSpan ReceiveTimeout
        {
            get
            {
                return _receiveTimeout;
            }
        }
        public TimeSpan SendTimeout
        {
            get
            {
                return _sendTimeout;
            }
        }

        public bool TransactionFlow
        {
            get
            {
                return _transactionFlow;
            }
        }
        public TransferMode TransferMode
        {
            get
            {
                return _transferMode;
            }
        }
        public TransactionProtocol TransactionProtocol
        {
            get
            {
                switch (_transactionProtocol.ToLower())
                {
                    case "oletransactions":
                        return TransactionProtocol.OleTransactions;
                    case "wsatomictransaction11":
                        return TransactionProtocol.WSAtomicTransaction11;
                    case "wsatomictransactionoctober2004":
                        return TransactionProtocol.WSAtomicTransactionOctober2004;
                    default:
                        return TransactionProtocol.Default;
                }
            }
        }
        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return _hostNameComparisonMode;
            }
        }
        public int ListenBacklog
        {
            get
            {
                return _listenBacklog;
            }
        }
        public int MaxBufferPoolSize
        {
            get
            {
                return _maxBufferPoolSize;
            }
        }
        public int MaxBufferSize
        {
            get
            {
                return _maxBufferSize;
            }
        }
        public int MaxConnections
        {
            get
            {
                return _maxConnections;
            }
        }
        public int MaxReceivedMessageSize
        {
            get
            {
                return _maxReceivedMessageSize;
            }
        }
        public bool PortSharingEnabled
        {
            get
            {
                return _portSharingEnabled;
            }
        }
        public SecurityMode Securitymode
        {
            get
            {
                return _securitymode;
            }
        }
        public MessageCredentialType ClientCredentialType
        {
            get
            {
                return _clientCredentialType;
            }
        }
        public bool enableBinaryFormatterBehavior
        {
            get
            {
                return _enableBinaryFormatterBehavior;
            }
        }

        #endregion

        #region serviceDebug
        public bool IncludeExceptionDetailInFaults
        {
            get
            {
                return _includeExceptionDetailInFaults;
            }
        }
        #endregion

        #region serviceThrottling
        public int MaxConcurrentCalls
        {
            get
            {
                return _maxConcurrentCalls;
            }
        }
        public int MaxConcurrentInstances
        {
            get
            {
                return _maxConcurrentInstances;
            }
        }
        public int MaxConcurrentSessions
        {
            get
            {
                return _maxConcurrentSessions;
            }
        }
        #endregion

        #region dataContractSerializer
        //public int MaxItemsInObjectGraph
        //{
        //    get
        //    {
        //        return _maxItemsInObjectGraph;
        //    }
        //}
        #endregion
    }
}
