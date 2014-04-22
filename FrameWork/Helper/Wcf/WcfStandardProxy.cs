using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Collections;
using System.Threading;
using System.Diagnostics;

using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;
using System.ServiceModel.Channels;
using System.Configuration;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    /// <summary>
    /// wcf代理拦截，自动处理wcf连接池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class WcfStandardProxy<T> : RealProxy 
    {

        /// <summary>
        /// bing模式
        /// </summary>
        NetTcpBinding binging = null;

        /// <summary>
        /// 终结点
        /// </summary>
        EndpointAddress address = null;

        /// <summary>
        /// 契约配置
        /// </summary>
        WcfClent client = null;

        /// <summary>
        /// 服务器节点名称
        /// </summary>
        string server_name = null;

        /// <summary>
        /// Wcf连接池最大值，默认为100
        /// </summary>
        int wcfMaxPoolSize = 100;

        /// <summary>
        /// Wcf获取连接过期时间(默认一分钟)
        /// </summary>
        long wcfOutTime = 60;

        /// <summary>
        /// Wcf连接失效时间(默认一分钟)
        /// </summary>
        long WcfFailureTime = 60;

        /// <summary>
        /// 是否启用连接池，默认不启用
        /// </summary>
        bool isUseWcfPool = false;

        /// <summary>
        /// 是否启用自定义二进制序列化器
        /// </summary>
        bool EnableBinaryFormatterBehavior = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Binging"></param>
        /// <param name="Address"></param>
        public WcfStandardProxy(NetTcpBinding Binging, EndpointAddress Address, WcfClentBinding wcfcbing, WcfClent Client, string Server_Name, bool IsUseWcfPool)
            : base(typeof(T))
        {
            this.binging = Binging;
            this.address = Address;
            this.EnableBinaryFormatterBehavior = wcfcbing.EnableBinaryFormatterBehavior;
            this.wcfMaxPoolSize = wcfcbing.WcfMaxPoolSize;
            this.wcfOutTime = wcfcbing.WcfOutTime;
            this.WcfFailureTime = wcfcbing.WcfFailureTime;
            this.client = Client;
            this.server_name = Server_Name;
            this.isUseWcfPool = IsUseWcfPool;
            //初始话连接池
            WcfPoolCache.Init(this.isUseWcfPool, wcfMaxPoolSize, wcfOutTime, WcfFailureTime, server_name, wcfcbing.WcfPoolMonitorReapTime);
        }

        /// <summary>
        /// 拦截器处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override IMessage Invoke(IMessage msg)
        {
            IMethodReturnMessage methodReturn = null;
            IMethodCallMessage methodCall = (IMethodCallMessage)msg;

            string contract = typeof(T).FullName;

            //此处使用wcf连接池技术，获取当前wcf连接池
            WcfPool pool = isUseWcfPool ? WcfPoolCache.GetWcfPool(this.server_name) : null;

            //获取的池子索引
            int? index = null;
            T channel = default(T);
            OperationContextScope scope = null;
            if (!isUseWcfPool)//不启用连接池
            {
                ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(this.binging, this.address, this.client.MaxItemsInObjectGraph, this.EnableBinaryFormatterBehavior);
                channel = factory.CreateChannel();
                //scope = new OperationContextScope(((IClientChannel)channel));
            }
            else
            {
                #region 传统模式

                //是否超时
                bool isouttime = false;
                //超时计时器
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    bool isReap = true;
                    //先判断池子中是否有此空闲连接
                    if (pool.GetFreePoolNums(contract) > 0)
                    {
                        isReap = false;
                        WcfCommunicationObj commobj = pool.GetChannel<T>();
                        if (commobj != null)
                        {
                            index = commobj.Index;
                            channel = (T)commobj.CommucationObject;
                            //Console.WriteLine(contract + "获取空闲索引:" + index);
                        }
                    }

                    //如果没有空闲连接判断是否池子是否已满，未满，则创建新连接并装入连接池
                    if (channel == null && !pool.IsPoolFull)
                    {
                        //创建新连接
                        ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(this.binging, this.address, this.client.MaxItemsInObjectGraph, this.EnableBinaryFormatterBehavior);

                        //装入连接池
                        bool flag = pool.AddPool<T>(factory, out channel, out index, isReap);
                        //Console.WriteLine(contract + "装入:" + flag + "  索引:" + index);
                    }

                    //如果当前契约无空闲连接，并且队列已满，并且非当前契约有空闲，则踢掉一个非当前契约
                    if (channel == null && pool.IsPoolFull && pool.GetFreePoolNums(contract) == 0 && pool.GetUsedPoolNums(contract) != this.wcfMaxPoolSize)
                    {
                        //创建新连接
                        ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(this.binging, this.address, this.client.MaxItemsInObjectGraph, this.EnableBinaryFormatterBehavior);
                        pool.RemovePoolOneNotAt<T>(factory, out channel, out index);
                    }

                    if (channel != null)
                        break;

                    //如果还未获取连接判断是否超时，如果超时抛异常
                    if (sw.Elapsed >= new TimeSpan(wcfOutTime * 1000 * 10000))
                    {
                        isouttime = true;
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
                sw.Stop();
                sw = null;

                if (isouttime)
                {
                    throw new Exception("获取连接池中的连接超时，请配置WCF客户端常量配置文件中的WcfOutTime属性，Server name=\"" + server_name + "\"");
                }

                #endregion

                #region 反应器

                //AutoResetEvent autoEvents = new AutoResetEvent(false);
                //BusinessException bex = null;

                //ThreadPool.QueueUserWorkItem(delegate(object param)
                //{
                //    //超时计时器
                //    Stopwatch sw = new Stopwatch();
                //    sw.Start();
                //    while (true)
                //    {
                //        #region while
                //        bool isReap = true;
                //        //先判断池子中是否有此空闲连接
                //        if (pool.GetFreePoolNums(contract) > 0)
                //        {
                //            isReap = false;
                //            try
                //            {
                //                WcfCommunicationObj commobj = pool.GetChannel<T>();
                //                if (commobj != null)
                //                {
                //                    index = commobj.Index;
                //                    channel = (T)commobj.CommucationObject;
                //                }
                //            }
                //            catch (Exception ex)
                //            {
                //                bex = new BusinessException(ex.ToString());
                //                autoEvents.Set();
                //                break;
                //            }
                //        }

                //        //如果没有空闲连接判断是否池子是否已满，未满，则创建新连接并装入连接池
                //        if (channel == null && !pool.IsPoolFull)
                //        {
                //            //创建新连接
                //            ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(this.binging, this.address, this.client.MaxItemsInObjectGraph,this.EnableBinaryFormatterBehavior);

                //            //装入连接池
                //            try
                //            {
                //                bool flag = pool.AddPool<T>(factory, out channel, out index, isReap);
                //            }
                //            catch (Exception ex)
                //            {
                //                bex = new BusinessException(ex.ToString());
                //                autoEvents.Set();
                //                break;
                //            }
                //        }

                //        //如果当前契约无空闲连接，并且队列已满，并且非当前契约有空闲，则踢掉一个非当前契约
                //        if (channel == null && pool.IsPoolFull && pool.GetFreePoolNums(contract) == 0 && pool.GetUsedPoolNums(contract) != this.wcfMaxPoolSize)
                //        {
                //            //创建新连接
                //            ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(this.binging, this.address, this.client.MaxItemsInObjectGraph,this.EnableBinaryFormatterBehavior);
                //            try
                //            {
                //                pool.RemovePoolOneNotAt<T>(factory, out channel, out index);
                //            }
                //            catch (Exception ex)
                //            {
                //                bex = new BusinessException(ex.ToString());
                //                autoEvents.Set();
                //                break;
                //            }
                //        }

                //        if (channel != null)
                //        {
                //            autoEvents.Set();
                //            break;
                //        }

                //        //如果还未获取连接判断是否超时，如果超时抛异常
                //        if (sw.Elapsed >= new TimeSpan(wcfOutTime * 1000 * 10000))
                //        {
                //            break;
                //        }
                //        else
                //        {
                //            Thread.Sleep(100);
                //        }
                //        #endregion
                //    }
                //    sw.Stop();
                //    sw = null;

                //    Thread.CurrentThread.Abort();
                //});

                //if (!autoEvents.WaitOne(new TimeSpan(wcfOutTime * 1000 * 10000)))
                //{
                //    throw new NormalException("获取连接池中的连接超时，请配置WCF客户端常量配置文件中的WcfOutTime属性，Server name=\"" + server_name + "\"");
                //}
                //if (bex != null)
                //{
                //    throw bex;
                //}

                #endregion
            }

            #region 传递上下文

            //web.config或app.config中的ApplicationName
            string wcfappname = ConfigurationManager.AppSettings["ApplicationName"];
            if (wcfappname != null)
                HeaderOperater.SetClientWcfAppNameHeader(wcfappname);

            #endregion

            try
            {
                object[] copiedArgs = Array.CreateInstance(typeof(object), methodCall.Args.Length) as object[];
                methodCall.Args.CopyTo(copiedArgs, 0);
                object returnValue = methodCall.MethodBase.Invoke(channel, copiedArgs);
                methodReturn = new ReturnMessage(returnValue,
                                                copiedArgs,
                                                copiedArgs.Length,
                                                methodCall.LogicalCallContext,
                                                methodCall);

                //如果启用连接池，使用完后把连接回归连接池
                if (isUseWcfPool)
                {
                    if (index != null)
                        pool.ReturnPool<T>((int)index);
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
                if (ex.InnerException != null)
                    exception = ex.InnerException;
                methodReturn = new ReturnMessage(exception, methodCall);

                //如果启用连接池，出错则关闭连接，并删除连接池中的连接
                if (isUseWcfPool)
                {
                    if (index != null)
                        pool.RemovePoolAt<T>((int)index);
                }
            }
            finally
            {
                if (!isUseWcfPool)//不启用连接池
                {
                    if (scope != null)
                        scope.Dispose();
                    (channel as IDisposable).Dispose();
                }

                //清除wcf应用程序名上下文
                if (wcfappname != null)
                    HeaderOperater.ClearClientWcfAppNameHeader();
            }
            
            return methodReturn;
        }

    }
}
