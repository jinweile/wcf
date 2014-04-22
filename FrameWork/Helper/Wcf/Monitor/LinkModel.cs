using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Eltc.Base.FrameWork.Helper.Wcf.Monitor
{
    /// <summary>
    /// wcf链接监控信息
    /// </summary>
    [DataContract]
    [Serializable]
    public class LinkModel
    {
        /// <summary>
        /// 链接的客户端IP
        /// </summary>
        [DataMember]
        public string ClientIP { get; set; }

        /// <summary>
        /// 请求的地址列表
        /// </summary>
        [DataMember]
        public Dictionary<string, UrlInfo> UrlInfoList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    [Serializable]
    public class UrlInfo
    {
        /// <summary>
        /// 当前链接数
        /// </summary>
        [DataMember]
        public int ConnNums { get; set; }

        /// <summary>
        /// 调用次数列表
        /// key:操作名
        /// value:操作次数
        /// </summary>
        [DataMember]
        public Dictionary<string,long> OperateNums { get; set; }
    }
}
