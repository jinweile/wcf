using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader
{
    /// <summary>
    /// 自定义消息头实体
    /// </summary>
    public class HeaderContext
    {
        private string correlationState;
        /// <summary>
        /// 会话唯一性标识
        /// </summary>
        public string CorrelationState
        {
            get { return correlationState; }
            set { correlationState = value; }
        }

        private string rootID;
        /// <summary>
        /// 根id
        /// </summary>
        public string RootID
        {
            get { return rootID; }
            set { rootID = value; }
        }

        private string parentID;
        /// <summary>
        /// 父id
        /// </summary>
        public string ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }

        private string ip;
        /// <summary>
        /// 客户端ip
        /// </summary>
        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        private string appName;
        /// <summary>
        /// 消费机应用名称
        /// </summary>
        public string AppName
        {
            get { return appName; }
            set { appName = value; }
        }
    }
}
