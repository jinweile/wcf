using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WcfStatistics.Model
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class Client
    {
        /// <summary>
        /// 客户端ip
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 连接数
        /// </summary>
        public int LinkNums { get; set; }
        /// <summary>
        /// 客户端应用列表
        /// </summary>
        public Dictionary<string, App> AppList { get; set; }
    }

    /// <summary>
    /// 应用
    /// </summary>
    public class App
    {
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 连接数
        /// </summary>
        public int LinkNums { get; set; }
        /// <summary>
        /// 契约列表
        /// </summary>
        public Dictionary<string,Adress> AdressList { get; set; }
    }

    /// <summary>
    /// 契约
    /// </summary>
    public class Adress
    {
        /// <summary>
        /// 契约地址
        /// </summary>
        public string AdressUrl { get; set; }
        /// <summary>
        /// 连接数
        /// </summary>
        public int LinkNums { get; set; }
        /// <summary>
        /// 操作信息列表
        /// </summary>
        public List<Operate> OperateList { get; set; }
    }

    /// <summary>
    /// 操作信息
    /// </summary>
    public class Operate
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string OperateName { get; set; }
        /// <summary>
        /// 操作次数
        /// </summary>
        public int OperateNums { get; set; }
    }
}
