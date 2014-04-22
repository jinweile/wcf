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
using System.ServiceModel.Channels;
using System.Configuration;

using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    /// <summary>
    /// 负载均衡代理
    /// </summary>
    internal class WcfLoadBalanceStandardProxy<T> : RealProxy
    {
        /// <summary>
        /// 构造
        /// </summary>
        public WcfLoadBalanceStandardProxy()
            : base(typeof(T))
        {

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

            string server_name = WcfCacheData.GetServerName(contract);
            WcfClentBinding clientconst = WcfCacheData.GetWcfClientConst(server_name);
            //web.config或app.config中的ApplicationName
            string wcfappname = ConfigurationManager.AppSettings["ApplicationName"];
            bool EnableBinaryFormatterBehavior = clientconst.EnableBinaryFormatterBehavior;
            LoadBalanceConfig balance = clientconst.LoadBalance; 
            //获取负载均衡设置中是否启用连接池
            bool isUseWcfPool = balance.IsUseWcfPool;

            //获取负载的服务器uri
            string uri = null;
            try
            {
                uri = balance.BalanceAlgorithm.GetServerKey();
            }
            catch (Exception buex)
            {
                throw buex;
            }
            Address uri_address = balance.WcfAdress[uri];

            WcfPool pool = null;

            //获取的池子索引
            int? index = null;
            T channel = default(T);
            OperationContextScope scope = null;
            if (!isUseWcfPool)//不启用连接池
            {
                ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(uri_address, EnableBinaryFormatterBehavior);
                channel = factory.CreateChannel();
                //scope = new OperationContextScope(((IClientChannel)channel));
            }
            else
            {
                string server_key = server_name + "/" + uri;
                //初始化连接池
                WcfPoolCache.Init(isUseWcfPool, uri_address.WcfMaxPoolSize, balance.WcfOutTime, balance.WcfFailureTime, server_key, balance.WcfPoolMonitorReapTime);
                //此处使用wcf连接池技术，获取当前wcf连接池
                pool = WcfPoolCache.GetWcfPool(server_key);

                #region 传统模式

                //是否超时
                bool isouttime = false;
                //超时计时器
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    #region while

                    if (string.IsNullOrEmpty(uri))
                    {
                        throw new Exception("所有的负载服务器都挂掉了");
                    }

                    bool isReap = true;
                    //先判断池子中是否有此空闲连接
                    if (pool.GetFreePoolNums(contract) > 0)
                    {
                        isReap = false;
                        try
                        {
                            WcfCommunicationObj commobj = pool.GetChannel<T>();
                            if (commobj != null && commobj.CommucationObject.State == CommunicationState.Opened)
                            {
                                index = commobj.Index;
                                channel = (T)commobj.CommucationObject;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.ToString());
                        }
                    }

                    //如果没有空闲连接判断是否池子是否已满，未满，则创建新连接并装入连接池
                    if (channel == null && !pool.IsPoolFull)
                    {
                        //创建新连接
                        ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(uri_address, EnableBinaryFormatterBehavior);

                        //装入连接池
                        try
                        {
                            bool flag = pool.AddPool<T>(factory, out channel, out index, isReap);
                        }
                        catch (Exception ex)
                        {
                            #region 重新获取服务器
                            balance.BalanceAlgorithm.Kill(uri);
                            try
                            {
                                uri = balance.BalanceAlgorithm.GetServerKey();
                            }
                            catch (Exception buex)
                            {
                                throw buex;
                            }
                            uri_address = balance.WcfAdress[uri];
                            server_key = server_name + "/" + uri;
                            //初始化连接池
                            WcfPoolCache.Init(isUseWcfPool, uri_address.WcfMaxPoolSize, balance.WcfOutTime, balance.WcfFailureTime, server_key, balance.WcfPoolMonitorReapTime);
                            //此处使用wcf连接池技术，获取当前wcf连接池
                            pool = WcfPoolCache.GetWcfPool(server_key);
                            continue;
                            #endregion
                        }
                    }

                    //如果当前契约无空闲连接，并且队列已满，并且非当前契约有空闲，则踢掉一个非当前契约
                    if (channel == null && pool.IsPoolFull && pool.GetFreePoolNums(contract) == 0 && pool.GetUsedPoolNums(contract) != uri_address.WcfMaxPoolSize)
                    {
                        //创建新连接
                        ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(uri_address, EnableBinaryFormatterBehavior);
                        try
                        {
                            pool.RemovePoolOneNotAt<T>(factory, out channel, out index);
                        }
                        catch (Exception ex)
                        {
                            #region 重新获取服务器
                            balance.BalanceAlgorithm.Kill(uri);
                            try
                            {
                                uri = balance.BalanceAlgorithm.GetServerKey();
                            }
                            catch (Exception buex)
                            {
                                throw buex;
                            }
                            uri_address = balance.WcfAdress[uri];
                            server_key = server_name + "/" + uri;
                            //初始化连接池
                            WcfPoolCache.Init(isUseWcfPool, uri_address.WcfMaxPoolSize, balance.WcfOutTime, balance.WcfFailureTime, server_key, balance.WcfPoolMonitorReapTime);
                            //此处使用wcf连接池技术，获取当前wcf连接池
                            pool = WcfPoolCache.GetWcfPool(server_key);
                            continue;
                            #endregion
                        }
                    }

                    if (channel != null)
                    {
                        break;
                    }

                    //如果还未获取连接判断是否超时，如果超时抛异常
                    if (sw.Elapsed >= new TimeSpan(balance.WcfOutTime * 1000 * 10000))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                    #endregion
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

                //        if (string.IsNullOrEmpty(uri))
                //        {
                //            bex = new BusinessException("所有的负载服务器都挂掉了");
                //            autoEvents.Set();
                //            break;
                //        }

                //        bool isReap = true;
                //        //先判断池子中是否有此空闲连接
                //        if (pool.GetFreePoolNums(contract) > 0)
                //        {
                //            isReap = false;
                //            try
                //            {
                //                WcfCommunicationObj commobj = pool.GetChannel<T>();
                //                if (commobj != null && commobj.CommucationObject.State == CommunicationState.Opened)
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
                //            ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(uri_address, EnableBinaryFormatterBehavior);

                //            //装入连接池
                //            try
                //            {
                //                bool flag = pool.AddPool<T>(factory, out channel, out index, isReap);
                //            }
                //            catch (Exception ex)
                //            {
                //                #region 重新获取服务器
                //                balance.BalanceAlgorithm.Kill(uri);
                //                try
                //                {
                //                    uri = balance.BalanceAlgorithm.GetServerKey();
                //                }
                //                catch (BusinessException buex)
                //                {
                //                    bex = buex;
                //                    autoEvents.Set();
                //                    break;
                //                }
                //                uri_address = balance.WcfAdress[uri];
                //                server_key = server_name + "/" + uri;
                //                //初始化连接池
                //                WcfPoolCache.Init(isUseWcfPool, uri_address.WcfMaxPoolSize, balance.WcfOutTime, balance.WcfFailureTime, server_key, balance.WcfPoolMonitorReapTime);
                //                //此处使用wcf连接池技术，获取当前wcf连接池
                //                pool = WcfPoolCache.GetWcfPool(server_key);
                //                continue;
                //                #endregion
                //            }
                //        }

                //        //如果当前契约无空闲连接，并且队列已满，并且非当前契约有空闲，则踢掉一个非当前契约
                //        if (channel == null && pool.IsPoolFull && pool.GetFreePoolNums(contract) == 0 && pool.GetUsedPoolNums(contract) != uri_address.WcfMaxPoolSize)
                //        {
                //            //创建新连接
                //            ChannelFactory<T> factory = WcfCacheData.GetFactory<T>(uri_address, EnableBinaryFormatterBehavior);
                //            try
                //            {
                //                pool.RemovePoolOneNotAt<T>(factory, out channel, out index);
                //            }
                //            catch (Exception ex)
                //            {
                //                #region 重新获取服务器
                //                balance.BalanceAlgorithm.Kill(uri);
                //                try
                //                {
                //                    uri = balance.BalanceAlgorithm.GetServerKey();
                //                }
                //                catch (BusinessException buex)
                //                {
                //                    bex = buex;
                //                    autoEvents.Set();
                //                    break;
                //                }
                //                uri_address = balance.WcfAdress[uri];
                //                server_key = server_name + "/" + uri;
                //                //初始化连接池
                //                WcfPoolCache.Init(isUseWcfPool, uri_address.WcfMaxPoolSize, balance.WcfOutTime, balance.WcfFailureTime, server_key, balance.WcfPoolMonitorReapTime);
                //                //此处使用wcf连接池技术，获取当前wcf连接池
                //                pool = WcfPoolCache.GetWcfPool(server_key);
                //                continue;
                //                #endregion
                //            }
                //        }

                //        if (channel != null)
                //        {
                //            autoEvents.Set();
                //            break;
                //        }

                //        //如果还未获取连接判断是否超时，如果超时抛异常
                //        if (sw.Elapsed >= new TimeSpan(balance.WcfOutTime * 1000 * 10000))
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

                //if (!autoEvents.WaitOne(new TimeSpan(balance.WcfOutTime * 1000 * 10000)))
                //{
                //    throw new NormalException("获取连接池中的连接超时，请配置WCF客户端常量配置文件中的WcfOutTime属性，Server name=\"" + server_name + "\" Uri:" + uri);
                //}
                //if (bex != null)
                //{
                //    throw bex;
                //}

                #endregion
            }

            #region 传递上下文

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
                    if (index != null && pool != null)
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
                    if (index != null && pool != null)
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
