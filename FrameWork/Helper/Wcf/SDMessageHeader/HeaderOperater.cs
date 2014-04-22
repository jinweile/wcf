using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader
{
    /// <summary>
    /// 消息头操作对象
    /// </summary>
    public class HeaderOperater
    {
        #region 自定义消息头 internal

        /// <summary>
        /// 获取客户端消息头
        /// </summary>
        /// <returns></returns>
        internal static MessageHeader GetClientWcfHeader()
        {
            MessageHeader msgheader = (MessageHeader)CallContext.LogicalGetData("msgheader");
            return msgheader;
        }

        /// <summary>
        /// 删除消息头
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="index"></param>
        internal static void RemoveHeader(MessageHeaders headers)
        {
            if (headers.FindHeader("HeaderContext", "session") >= 0)
                headers.RemoveAt(headers.FindHeader("HeaderContext", "session"));
        }

        internal static void AddHeader(MessageHeaders headers, MessageHeader hContext)
        {
            headers.Add(hContext);
        }

        #endregion

        #region 自定义消息头 public

        /// <summary>
        /// 获取服务器端消息头
        /// </summary>
        /// <returns></returns>
        public static HeaderContext GetServiceWcfHeader(OperationContext context)
        {
            if (context == null)
                return null;

            if (context.IncomingMessageHeaders.FindHeader("HeaderContext", "session") >= 0)
                return context.IncomingMessageHeaders.GetHeader<HeaderContext>("HeaderContext", "session");
            else
                return null;
        }

        /// <summary>
        /// 创建客户端消息头
        /// </summary>
        public static void SetClientWcfHeader(HeaderContext context)
        {
            if (context == null)
            {
                CallContext.LogicalSetData("msgheader", null);
                return;
            }
            MessageHeader msgheader = System.ServiceModel.Channels.MessageHeader.CreateHeader("HeaderContext", "session", context);
            //MessageHeader msgheader = MessageHeader.CreateHeader("HeaderContext", "session", context);
            CallContext.LogicalSetData("msgheader", msgheader);
        }

        /// <summary>
        /// 删除客户端消息头信息
        /// </summary>
        public static void ClearClientWcfHeader()
        {
            CallContext.LogicalSetData("msgheader", null);
        }

        #endregion

        #region wcf应用程序名

        /// <summary>
        /// 获取客户端消息头
        /// </summary>
        /// <returns></returns>
        internal static MessageHeader GetClientWcfAppNameHeader()
        {
            MessageHeader msgheader = (MessageHeader)CallContext.LogicalGetData("appnamemsgheader");
            return msgheader;
        }

        /// <summary>
        /// 删除消息头
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="index"></param>
        internal static void RemoveAppNameHeader(MessageHeaders headers)
        {
            if (headers.FindHeader("AppNameHeaderContext", "appnamesession") >= 0)
                headers.RemoveAt(headers.FindHeader("AppNameHeaderContext", "appnamesession"));
        }

        internal static void AddAppNameHeader(MessageHeaders headers, MessageHeader hContext)
        {
            headers.Add(hContext);
        }

        /// <summary>
        /// 获取服务器端消息头
        /// </summary>
        /// <returns></returns>
        internal static string GetServiceWcfAppNameHeader(OperationContext context)
        {
            if (context == null)
                return null;

            if (context.IncomingMessageHeaders.FindHeader("AppNameHeaderContext", "appnamesession") >= 0)
                return context.IncomingMessageHeaders.GetHeader<string>("AppNameHeaderContext", "appnamesession");
            else
                return null;
        }

        /// <summary>
        /// 创建客户端消息头
        /// </summary>
        internal static void SetClientWcfAppNameHeader(string context)
        {
            if (context == null)
            {
                CallContext.LogicalSetData("appnamemsgheader", null);
                return;
            }
            MessageHeader msgheader = System.ServiceModel.Channels.MessageHeader.CreateHeader("AppNameHeaderContext", "appnamesession", context);
            //MessageHeader msgheader = MessageHeader.CreateHeader("AppNameHeaderContext", "appnamesession", context);
            CallContext.LogicalSetData("appnamemsgheader", msgheader);
        }

        /// <summary>
        /// 删除客户端消息头信息
        /// </summary>
        internal static void ClearClientWcfAppNameHeader()
        {
            CallContext.LogicalSetData("appnamemsgheader", null);
        }

        #endregion

    }
}
