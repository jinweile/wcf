using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.ServiceModel;

namespace Eltc.Base.FrameWork.Helper.Wcf.LoadBalance
{
    /// <summary>
    /// 负载均衡算法基类
    /// </summary>
    internal abstract class BalanceAlgorithmBase
    {
        private IDictionary<string, Address> addresslist;
        /// <summary>
        /// 全部负载的服务器地址
        /// </summary>
        internal IDictionary<string, Address> Addresslist
        {
            get { return addresslist; }
            set { addresslist = value; }
        }

        private ICollection<string> urilist;
        private object lockurilist = new object();
        /// <summary>
        /// 正在负载的服务器列表
        /// </summary>
        public ICollection<string> Urilist
        {
            get { return urilist; }
        }
        /// <summary>
        /// 心跳监听线程，维护负载均衡服务器列表
        /// </summary>
        private IList<Thread> thlist;
        private object thlock = new object();
        private IList<ThreadObj> thobjlist;

        private bool isNewInstance = false;
        /// <summary>
        /// 是否初始化完成
        /// </summary>
        public bool IsNewInstance
        {
            get { return isNewInstance; }
        }

        /// <summary>
        /// 初始化服务器端长连接，激活心跳
        /// </summary>
        internal void NewInstance(IDictionary<string, Address> address, NetTcpBinding binding)
        {
            if (isNewInstance) return;

            if (urilist == null)
            {
                lock (lockurilist)
                {
                    if (urilist == null)
                    {
                        this.addresslist = address;
                        IList<string> urilist_temp = new List<string>();
                        thlist = new List<Thread>();
                        thobjlist = new List<ThreadObj>();
                        //创建心跳连接并初始化正在负载的服务器列表
                        ChannelFactory<IHeatBeat> channelFactory = new ChannelFactory<IHeatBeat>(binding);

                        foreach (string uri in this.addresslist.Keys)
                        {
                            #region for
                            EndpointAddress point = new EndpointAddress("net.tcp://" + uri + "/Eltc.Base/FrameWork/Helper/Wcf/LoadBalance/IHeatBeat");

                            IHeatBeat proxy = null;
                            try
                            {
                                proxy = channelFactory.CreateChannel(point);
                                (proxy as ICommunicationObject).Open();
                                //增加到服务器列表
                                urilist_temp.Add(uri);
                            }
                            catch { }

                            Thread th = new Thread(new ParameterizedThreadStart(ThreadRun));
                            ThreadObj thobj = new ThreadObj();
                            thobj.channelFactory = channelFactory;
                            thobj.point = point;
                            thobj.proxy = proxy;
                            thobj.uri = uri;
                            thobj.heatBeatTime = address[uri].HeatBeatTime;
                            this.thobjlist.Add(thobj);
                            this.thlist.Add(th);
                            #endregion
                        }

                        Init(urilist_temp);
                        this.urilist = urilist_temp;
                    }
                }
            }

            for (int i = 0; i < thlist.Count; i++)
            {
                if (thlist[i].ThreadState == ThreadState.Unstarted)
                {
                    lock (thlock)
                    {
                        if (thlist[i].ThreadState == ThreadState.Unstarted)
                            thlist[i].Start(thobjlist[i]);
                    }
                }
            }
            isNewInstance = true;
        }

        /// <summary>
        /// 删除一个挂掉的服务器
        /// </summary>
        /// <param name="uri"></param>
        internal void Kill(string uri)
        {
            #region 删除出服务器列表
            if (this.urilist.Contains(uri))
            {
                lock (this.lockurilist)
                {
                    if (this.urilist.Contains(uri))
                    {
                        this.urilist.Remove(uri);
                        //初始化一下负载算法
                        Init(this.urilist);
                    }
                }
            }
            #endregion
        }

        private void ThreadRun(object parms)
        {
            ThreadObj obj = (ThreadObj)parms;
            IHeatBeat proxy = obj.proxy;
            string uri = obj.uri;
            TimeSpan heatBeatTime = obj.heatBeatTime;
            EndpointAddress point = obj.point;
            ChannelFactory<IHeatBeat> channelFactory = obj.channelFactory;
            #region 线程处理
            while (true)
            {
                if ((proxy as ICommunicationObject).State != CommunicationState.Opened)
                {
                    //删除失效的节点
                    Kill(uri);
                    try
                    {
                        (proxy as ICommunicationObject).Close();
                    }
                    catch
                    {
                        (proxy as ICommunicationObject).Abort();
                    }

                    #region 尝试3次连接，如果连接不上认为服务器挂掉
                    int i = 1;
                    while (i < 4)
                    {
                        try
                        {
                            proxy = channelFactory.CreateChannel(point);
                            (proxy as ICommunicationObject).Open();
                            #region 增加到服务器列表
                            if (!this.urilist.Contains(uri))
                            {
                                lock (this.lockurilist)
                                {
                                    if (!this.urilist.Contains(uri))
                                    {
                                        this.urilist.Add(uri);
                                        //初始化一下负载算法
                                        Init(this.urilist);
                                    }
                                }
                            }
                            #endregion
                            break;
                        }
                        catch
                        {
                            try
                            {
                                (proxy as ICommunicationObject).Close();
                            }
                            catch
                            {
                                (proxy as ICommunicationObject).Abort();
                            }
                        }
                        i++;
                    }
                    #endregion
                    #region 删除出服务器列表
                    if (i > 3)
                    {
                        Kill(uri);
                        //30秒后再重试
                        Thread.Sleep(TimeSpan.Parse("00:00:30"));
                        continue;
                    }
                    #endregion
                }

                #region 维持心跳
                if ((proxy as ICommunicationObject).State == CommunicationState.Opened)
                {
                    //维持心跳
                    while (true)
                    {
                        try
                        {
                            proxy.CallBack(true);
                            //心跳扫描时间间隔
                            Thread.Sleep(heatBeatTime);
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 初始化服务器列表
        /// </summary>
        /// <param name="nodes">负载节点列表</param>
        protected abstract void Init(ICollection<string> _urilist);

        /// <summary>
        /// 获取服务器key
        /// </summary>
        /// <returns>key</returns>
        internal abstract string GetServerKey();
    }

    internal class ThreadObj
    {
        public string uri;
        public TimeSpan heatBeatTime;
        public IHeatBeat proxy;
        public EndpointAddress point;
        public ChannelFactory<IHeatBeat> channelFactory;
    }
}
