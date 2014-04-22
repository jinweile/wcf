using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eltc.Base.FrameWork.Helper.Wcf.Monitor
{
    /// <summary>
    /// wcf监控操作类(服务器端加入监控信息)
    /// </summary>
    internal class MonitorData
    {
        /// <summary>
        /// 监控信息
        /// </summary>
        private Dictionary<string, LinkModel> monitorInfo = null;

        /// <summary>
        /// 单例
        /// </summary>
        private static volatile MonitorData _instance = null;
        private static volatile object lockhelper = new object();

        /// <summary>
        /// 似有构造函数
        /// </summary>
        private MonitorData()
        {
            this.monitorInfo = new Dictionary<string, LinkModel>();
        }

        /// <summary>
        /// 实体
        /// </summary>
        public static MonitorData Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockhelper)
                    {
                        if (_instance == null)
                        {
                            _instance = new MonitorData();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// 获取监控信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, LinkModel> getMonitorInfo()
        {
            return this.monitorInfo;
        }


        private static volatile object lockupdateconnnums = new object();
        /// <summary>
        /// 更新某地址的链接数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <param name="Url">请求的url绝对地址</param>
        /// <param name="isadd">是否增加，否则就是减</param>
        public void UpdateUrlConnNums(string ip, string Url, bool isadd)
        {
            if (!this.monitorInfo.ContainsKey(ip))
            {
                lock (lockupdateconnnums)
                {
                    if (!this.monitorInfo.ContainsKey(ip))
                    {
                        LinkModel model = new LinkModel();
                        model.ClientIP = ip;
                        model.UrlInfoList = new Dictionary<string, UrlInfo>();
                        UrlInfo url = new UrlInfo();
                        url.ConnNums = 0;
                        url.OperateNums = new Dictionary<string, long>();
                        model.UrlInfoList[Url] = url;

                        this.monitorInfo[ip] = model;
                    }
                }
            }

            if (this.monitorInfo.ContainsKey(ip))
            {
                lock (lockupdateconnnums)
                {
                    if (!this.monitorInfo[ip].UrlInfoList.ContainsKey(Url))
                    {
                        UrlInfo url = new UrlInfo();
                        url.ConnNums = 0;
                        url.OperateNums = new Dictionary<string, long>();
                        this.monitorInfo[ip].UrlInfoList[Url] = url;
                    }

                    if (isadd)
                        this.monitorInfo[ip].UrlInfoList[Url].ConnNums += 1;
                    else
                        this.monitorInfo[ip].UrlInfoList[Url].ConnNums -= 1;

                    //if (this.monitorInfo[ip].UrlInfoList[Url].ConnNums < 0)
                    //    this.monitorInfo[ip].UrlInfoList[Url].ConnNums = 0;
                }
            }

        }

        private static volatile object lockupdateoperatenums = new object();
        /// <summary>
        /// 更新操作次数
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="Url"></param>
        /// <param name="operateName"></param>
        public void UpdateOperateNums(string ip, string Url, string operateName)
        {
            if (!this.monitorInfo.ContainsKey(ip))
            {
                lock (lockupdateconnnums)
                {
                    if (!this.monitorInfo.ContainsKey(ip))
                    {
                        LinkModel model = new LinkModel();
                        model.ClientIP = ip;
                        model.UrlInfoList = new Dictionary<string, UrlInfo>();
                        UrlInfo url = new UrlInfo();
                        url.ConnNums = 0;
                        url.OperateNums = new Dictionary<string, long>();
                        model.UrlInfoList[Url] = url;

                        this.monitorInfo[ip] = model;
                    }
                }
            }

            lock (lockupdateoperatenums)
            {
                if (!this.monitorInfo[ip].UrlInfoList.ContainsKey(Url))
                {
                    UrlInfo url = new UrlInfo();
                    url.ConnNums = 0;
                    url.OperateNums = new Dictionary<string, long>();
                    this.monitorInfo[ip].UrlInfoList[Url] = url;
                }

                this.monitorInfo[ip].UrlInfoList[Url].OperateNums[operateName] = 
                    this.monitorInfo[ip].UrlInfoList[Url].OperateNums.ContainsKey(operateName) ?
                    (this.monitorInfo[ip].UrlInfoList[Url].OperateNums[operateName] == long.MaxValue ? 1 : this.monitorInfo[ip].UrlInfoList[Url].OperateNums[operateName] + 1) : 1;
            }

        }

    }
}
