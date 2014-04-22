using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eltc.Base.FrameWork.Helper.Wcf;
using System.ServiceModel;
using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;

namespace Eltc.Base.FrameWork.Helper
{
    /// <summary>
    /// 连接池客户端封装
    /// </summary>
    public sealed class WcfClient
    {
        /// <summary>
        /// 缓存的代理拦截器
        /// </summary>
        private static volatile IDictionary<string, object> proxyDic = new Dictionary<string, object>();
        private static volatile object lockproxyDic = new object();

        /// <summary>
        /// 获取客户端代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetProxy<T>()
        {
            return SetProxy<T>();
        }

        /// <summary>
        /// 获取客户端代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T SetProxy<T>()
        {
            string contract = typeof(T).FullName;

            T proxy = default(T);

            //首先判断是否启用负载均衡
            bool isloadbalance = WcfCacheData.IsLoadBalance(contract);

            if (isloadbalance)
            {
                #region 启用负载均衡

                if (proxyDic.ContainsKey(contract))
                {
                    proxy = (T)proxyDic[contract];
                }
                else
                {
                    lock (lockproxyDic)
                    {
                        if (!proxyDic.ContainsKey(contract))
                        {
                            proxy = (T)new WcfLoadBalanceStandardProxy<T>().GetTransparentProxy();

                            proxyDic.Add(contract, proxy);
                        }
                        else
                        {
                            proxy = (T)proxyDic[contract];
                        }
                    }
                }

                #endregion
            }
            else
            {
                #region 不启用负载均衡

                if (proxyDic.ContainsKey(contract))
                {
                    proxy = (T)proxyDic[contract];
                }
                else
                {
                    lock (lockproxyDic)
                    {
                        if (!proxyDic.ContainsKey(contract))
                        {
                            NetTcpBinding binging;
                            EndpointAddress address;
                            WcfClent client;
                            WcfClentBinding wcfcbing;
                            string server_name;

                            WcfCacheData.GetCachedData(contract, out binging, out address, out client, out wcfcbing, out server_name);
                            proxy = (T)new WcfStandardProxy<T>(binging, address, wcfcbing, client, server_name, wcfcbing.IsUseWcfPool).GetTransparentProxy();

                            proxyDic.Add(contract, proxy);
                        }
                        else
                        {
                            proxy = (T)proxyDic[contract];
                        }
                    }
                }

                #endregion
            }

            return proxy;
        }

    }
}
