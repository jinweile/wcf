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

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    internal class WcfPoolCache
    {
        /// <summary>
        /// Wcf连接池
        /// </summary>
        volatile static IDictionary<string, WcfPool> poolDic = new Dictionary<string, WcfPool>();
        volatile static object lockpool = new object();
        /// <summary>
        /// 监控线程
        /// </summary>
        volatile static IDictionary<string, Thread> thDic = new Dictionary<string, Thread>();
        volatile static object lockth = new object();

        /// <summary>
        /// 初始化连接池
        /// </summary>
        /// <param name="isUseWcfPool">是否使用连接池</param>
        /// <param name="wcfMaxPoolSize">池子最大值</param>
        /// <param name="wcfOutTime">获取连接超时时间</param>
        /// <param name="WcfFailureTime">连接池回收时间</param>
        /// <param name="server_name">服务器名</param>
        public static void Init(bool isUseWcfPool, int wcfMaxPoolSize, long wcfOutTime, long WcfFailureTime, string server_name,int WcfPoolMonitorReapTime)
        {
            //装在连接池
            if (isUseWcfPool && !poolDic.ContainsKey(server_name))
            {
                lock (lockpool)
                {
                    if (isUseWcfPool && !poolDic.ContainsKey(server_name))
                    {
                        WcfPool pool = new WcfPool(wcfMaxPoolSize, wcfOutTime, WcfFailureTime, WcfPoolMonitorReapTime);
                        poolDic.Add(server_name, pool);
                    }
                }
            }
            //开启监控线程
            if (isUseWcfPool && !thDic.ContainsKey(server_name))
            {
                lock (lockth)
                {
                    if (!thDic.ContainsKey(server_name))
                    {
                        Thread poolMonitorTh = new Thread(poolDic[server_name].MonitorExec);
                        poolMonitorTh.Start();
                        thDic.Add(server_name, poolMonitorTh);
                    }
                }
            }
        }

        /// <summary>
        /// 获取连接池
        /// </summary>
        /// <param name="server_name"></param>
        /// <returns></returns>
        public static WcfPool GetWcfPool(string server_name)
        {
            return poolDic[server_name];
        }

    }
}
