using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections;
using System.ServiceModel.Description;
using Eltc.Base.FrameWork.Helper.Wcf.SerializeBehavior;
using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfCacheData
    {
        /// <summary>
        /// 客户端配置
        /// </summary>
        private static volatile IDictionary<string, string> clientsServiceReverseList = null;
        private static volatile IDictionary<string, IDictionary<string, WcfClent>> clientsServiceList = null;
        private static volatile IDictionary<string, WcfClentBinding> wcfClentConstantList = null;
        private static volatile object lockhelper = new object();
        private static volatile bool isInitialization = false;

        /// <summary>
        /// 缓存的绑定参数
        /// </summary>
        private static volatile IDictionary<string, NetTcpBinding> bingDic = null;
        private static volatile object lockbing = new object();

        /// <summary>
        /// 通道工厂缓存
        /// </summary>
        static Hashtable factorycache = new Hashtable();

        /// <summary>
        /// 初始化常量
        /// </summary>
        public static void Init()
        {
            if (isInitialization) return;

            if (!isInitialization)
            {
                lock (lockhelper)
                {
                    if (!isInitialization)
                    {
                        WcfClentSetting config = new WcfClentSetting();
                        //根据契约名获取部署服务器
                        clientsServiceReverseList = config.WcfClent.ClientsServiceReverseList;
                        //根据服务器名获取客户端契约哈希表
                        clientsServiceList = config.WcfClent.ClientsServiceList;
                        //根据服务器名获取客户端常量配置
                        wcfClentConstantList = config.WcfClentConstant.WcfClentConstantList;
                        //标记客户端配置初始化完成
                        isInitialization = true;
                    }
                }
            }
            if (bingDic == null)
            {
                lock (lockbing)
                {
                    if (bingDic == null)
                    {
                        bingDic = new Dictionary<string, NetTcpBinding>();
                    }
                }
            }
            if (endpointDic == null)
            {
                lock (lockpoint)
                {
                    if (endpointDic == null)
                    {
                        endpointDic = new Dictionary<string, EndpointAddress>();
                    }
                }
            }
        }

        /// <summary>
        /// 获取缓存的常量
        /// </summary>
        /// <param name="client"></param>
        /// <param name="wcfcbing"></param>
        /// <param name="server_name"></param>
        public static string GetServerName(string contract)
        {
            if (!clientsServiceReverseList.ContainsKey(contract))
            {
                throw new Exception("没有注册此契约,请检查契约配置文件,契约名称:" + contract);
            }
            //根据契约获取服务器配置名
            return clientsServiceReverseList[contract];
        }

        /// <summary>
        /// 获取缓存的服务常量配置
        /// </summary>
        /// <param name="server_name"></param>
        /// <param name="wcfcbing"></param>
        public static WcfClentBinding GetWcfClientConst(string server_name)
        {
            if (!wcfClentConstantList.ContainsKey(server_name))
            {
                throw new Exception("没有注册此契约对应的服务器配置,请检查常量配置文件,服务器配置名称:" + server_name);
            }
            //根据服务器名获取常量配置
            return wcfClentConstantList[server_name];
        }

        /// <summary>
        /// 获取缓存的契约信息
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="client"></param>
        public static WcfClent GetContract(string contract)
        {
            string server_name = GetServerName(contract);
            if (!clientsServiceList.ContainsKey(server_name))
            {
                throw new Exception("没有注册此契约对应的服务器配置,请检查契约配置文件,服务器配置名称:" + server_name);
            }
            //根据服务名契约名获取客户端契约实体
            return clientsServiceList[server_name][contract];
        }

        /// <summary>
        /// 获取缓存的绑定信息
        /// </summary>
        /// <param name="contract">契约名称</param>
        /// <param name="server_name">服务器名称</param>
        /// <param name="wcfcbing">绑定信息</param>
        private static NetTcpBinding GetBing(string contract)
        {
            string server_name = GetServerName(contract);
            WcfClentBinding wcfcbing = GetWcfClientConst(server_name);

            NetTcpBinding binging = null;
            if (bingDic.ContainsKey(server_name))
            {
                binging = bingDic[server_name];
            }
            else
            {
                binging = new NetTcpBinding();
                binging.MaxBufferPoolSize = wcfcbing.MaxBufferPoolSize;
                binging.MaxBufferSize = wcfcbing.MaxBufferSize;
                binging.MaxReceivedMessageSize = wcfcbing.MaxReceivedMessageSize;
                binging.MaxConnections = wcfcbing.MaxConnections;
                binging.ListenBacklog = wcfcbing.ListenBacklog;
                binging.OpenTimeout = wcfcbing.OpenTimeout;
                binging.ReceiveTimeout = wcfcbing.ReceiveTimeout;
                binging.TransferMode = wcfcbing.TransferMode;
                binging.Security.Mode = wcfcbing.SecurityMode;

                binging.SendTimeout = wcfcbing.SendTimeout;
                binging.ReaderQuotas.MaxDepth = wcfcbing.ReaderQuotasMaxDepth;
                binging.ReaderQuotas.MaxStringContentLength = wcfcbing.ReaderQuotasMaxStringContentLength;
                binging.ReaderQuotas.MaxArrayLength = wcfcbing.ReaderQuotasMaxArrayLength;
                binging.ReaderQuotas.MaxBytesPerRead = wcfcbing.ReaderQuotasMaxBytesPerRead;
                binging.ReaderQuotas.MaxNameTableCharCount = wcfcbing.ReaderQuotasMaxNameTableCharCount;
                binging.ReliableSession.Ordered = wcfcbing.ReliableSessionOrdered;
                binging.ReliableSession.Enabled = wcfcbing.ReliableSessionEnabled;
                binging.ReliableSession.InactivityTimeout = wcfcbing.ReliableSessionInactivityTimeout;

                //缓存绑定参数
                if (!bingDic.ContainsKey(server_name))
                {
                    lock (lockbing)
                    {
                        if (!bingDic.ContainsKey(server_name))
                        {
                            bingDic.Add(server_name, binging);
                        }
                    }
                }
            }

            return binging;
        }

        /// <summary>
        /// 获取缓存的
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static EndpointAddress GetAddress(string contract, string uri)
        {
            string server_name = GetServerName(contract);
            WcfClentBinding wcfcbing = GetWcfClientConst(server_name);
            WcfClent client = GetContract(contract);

            string key = uri == null ? contract : uri + "/" + contract;

            EndpointAddress address = null;
            if (endpointDic.ContainsKey(key))
            {
                address = endpointDic[key];
            }
            else
            {
                string Uri = uri == null ? wcfcbing.Uri : uri;
                string baseaddress = Uri.EndsWith("/") ? Uri.TrimEnd('/') : Uri;
                string extendaddress = client.Address.StartsWith("/") ? client.Address.TrimStart('/') : client.Address;
                address = new EndpointAddress("net.tcp://" + baseaddress + "/" + extendaddress);
                //缓存终结点
                if (!endpointDic.ContainsKey(contract))
                {
                    lock (lockpoint)
                    {
                        if (!endpointDic.ContainsKey(key))
                        {
                            endpointDic.Add(key, address);
                        }
                    }
                }
            }

            return address;
        }

        #region 不启用负载均衡使用

        /// <summary>
        /// 终结点缓存
        /// </summary>
        private static volatile IDictionary<string, EndpointAddress> endpointDic = null;
        private static volatile object lockpoint = new object();

        /// <summary>
        /// 获取缓存的常量配置
        /// </summary>
        /// <param name="contract">契约名</param>
        /// <param name="binging">绑定信息</param>
        /// <param name="address">终结点地址</param>
        /// <param name="client">契约配置</param>
        /// <param name="wcfcbing">客户端常量配置</param>
        /// <param name="server_name">服务器名</param>
        public static void GetCachedData(string contract, out NetTcpBinding binging, out EndpointAddress address, out WcfClent client, out WcfClentBinding wcfcbing, out string server_name)
        {
            //获取常量
            server_name = GetServerName(contract);
            wcfcbing = GetWcfClientConst(server_name);
            client = GetContract(contract);

            //获取绑定
            binging = GetBing(contract);

            //获取终结点，注：没启用负载均衡，缓存终结点的的key是契约，启用负载均衡，缓存终结点的key是address+"/"+contract
            address = GetAddress(contract, null);
        }

        /// <summary>
        /// 获取缓存的通道工厂
        /// </summary>
        /// <typeparam name="T">契约</typeparam>
        /// <param name="binging">绑定信息</param>
        /// <param name="address">终结点地址</param>
        /// <param name="MaxItemsInObjectGraph">序列化大小</param>
        /// <returns>通道工厂</returns>
        public static ChannelFactory<T> GetFactory<T>(NetTcpBinding binging, EndpointAddress address, int? MaxItemsInObjectGraph,bool EnableBinaryFormatterBehavior)
        {
            string contract = typeof(T).FullName;
            ChannelFactory<T> client = null;
            if (factorycache.ContainsKey(contract))
            {
                client = (ChannelFactory<T>)factorycache[contract];
                if (client.State != CommunicationState.Opened)
                {
                    client = null;
                }
            }
            if (client == null)
            {
                client = new ChannelFactory<T>(binging, address);
                //增加头信息行为
                ClientMessageInspector msgBehavior = new ClientMessageInspector();
                client.Endpoint.Behaviors.Add(msgBehavior);
                //如果启用自定义二进制序列化器
                if (EnableBinaryFormatterBehavior)
                {
                    if (client.Endpoint.Behaviors.Find<BinaryFormatterBehavior>() == null)
                    {
                        BinaryFormatterBehavior serializeBehavior = new BinaryFormatterBehavior();
                        client.Endpoint.Behaviors.Add(serializeBehavior);
                    }
                }
                //如果有MaxItemsInObjectGraph配置指定配置此行为
                if (MaxItemsInObjectGraph != null)
                {
                    foreach (OperationDescription op in client.Endpoint.Contract.Operations)
                    {
                        DataContractSerializerOperationBehavior dataContractBehavior =
                                    op.Behaviors.Find<DataContractSerializerOperationBehavior>()
                                    as DataContractSerializerOperationBehavior;
                        if (dataContractBehavior != null)
                        {
                            dataContractBehavior.MaxItemsInObjectGraph = (int)MaxItemsInObjectGraph;
                        }
                    }
                }
                factorycache[contract] = client;
            }
            return client;
        }

        #endregion

        #region 启用负载均衡

        /// <summary>
        /// 是否启用负载均衡
        /// </summary>
        public static bool IsLoadBalance(string contract)
        {
            Init();
            string server_name = GetServerName(contract);
            WcfClentBinding bing = GetWcfClientConst(server_name);
            bool flag = bing.LoadBalance.IsUsed;
            //如果启用负载均衡，则启动监听心跳
            if (flag && !wcfClentConstantList[server_name].LoadBalance.BalanceAlgorithm.IsNewInstance)
            {
                LoadBalanceConfig balance = wcfClentConstantList[server_name].LoadBalance;
                balance.BalanceAlgorithm.NewInstance(balance.WcfAdress, GetBing(contract));
            }
            return flag;
        }

        /// <summary>
        /// 得到负载的通道工厂
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static ChannelFactory<T> GetFactory<T>(Address uri_address, bool EnableBinaryFormatterBehavior)
        {
            string contract = typeof(T).FullName;
            string key = uri_address.Uri + "/" + contract;
            ChannelFactory<T> client = null;
            if (factorycache.ContainsKey(key))
            {
                client = (ChannelFactory<T>)factorycache[key];
                if (client.State != CommunicationState.Opened)
                {
                    client = null;
                }
            }
            if (client == null)
            {
                NetTcpBinding binging = GetBing(contract);
                EndpointAddress address = GetAddress(contract, uri_address.Uri);
                WcfClent client_contract = GetContract(contract);
                client = new ChannelFactory<T>(binging, address);
                //增加头信息行为
                ClientMessageInspector msgBehavior = new ClientMessageInspector();
                client.Endpoint.Behaviors.Add(msgBehavior);
                //如果启用自定义二进制序列化器
                if (EnableBinaryFormatterBehavior)
                {
                    if (client.Endpoint.Behaviors.Find<BinaryFormatterBehavior>() == null)
                    {
                        BinaryFormatterBehavior serializeBehavior = new BinaryFormatterBehavior();
                        client.Endpoint.Behaviors.Add(serializeBehavior);
                    }
                }
                //如果有MaxItemsInObjectGraph配置指定配置此行为
                if (client_contract.MaxItemsInObjectGraph != null)
                {
                    foreach (OperationDescription op in client.Endpoint.Contract.Operations)
                    {
                        DataContractSerializerOperationBehavior dataContractBehavior =
                                    op.Behaviors.Find<DataContractSerializerOperationBehavior>()
                                    as DataContractSerializerOperationBehavior;
                        if (dataContractBehavior != null)
                        {
                            dataContractBehavior.MaxItemsInObjectGraph = (int)client_contract.MaxItemsInObjectGraph;
                        }
                    }
                }
                factorycache[contract] = client;
            }
            return client;
        }

        #endregion
    }
}
