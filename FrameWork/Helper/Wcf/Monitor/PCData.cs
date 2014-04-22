using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Eltc.Base.FrameWork.Helper.Wcf.Monitor
{
    /// <summary>
    /// 服务器性能
    /// </summary>
    [DataContract]
    [Serializable]
    public class PCData
    {
        /// <summary>
        /// 进程ID
        /// </summary>
        [DataMember]
        public int ProcessId { get; set; }

        /// <summary>
        /// cpu
        /// </summary>
        [DataMember]
        public double Cpu { get; set; }

        /// <summary>
        /// 内存
        /// </summary>
        [DataMember]
        public double Mem { get; set; }

        /// <summary>
        /// 当前工作线程数
        /// </summary>
        [DataMember]
        public int ThreadCount { get; set; }
    }
}
