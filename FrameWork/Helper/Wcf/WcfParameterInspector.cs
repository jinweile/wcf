using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections;
using System.Threading;

using Eltc.Base.FrameWork.Helper.Wcf.Monitor;
using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;

namespace Eltc.Base.FrameWork.Helper.Wcf
{
    /// <summary>
    /// 请求前处理
    /// </summary>
    /// <param name="operationName"></param>
    /// <param name="inputs"></param>
    /// <param name="context"></param>
    internal delegate void WcfBeforeCall(string operationName, object[] inputs, string AbsolutePath, object correlationState);
    /// <summary>
    /// 请求结束后处理
    /// </summary>
    /// <param name="operationName"></param>
    /// <param name="outputs"></param>
    /// <param name="returnValue"></param>
    /// <param name="correlationState"></param>
    /// <param name="context"></param>
    internal delegate void WcfAfterCall(string operationName, object[] outputs, object returnValue, object correlationState, string AbsolutePath);

    internal class WcfParameterInspector : IOperationBehavior, IParameterInspector
    {
        #region IOperationBehavior Members
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="operationDescription"></param>   
        /// <param name="bindingParameters"></param>   
        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {

        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="operationDescription"></param>   
        /// <param name="clientOperation"></param>   
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {

        }
        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="operationDescription"></param>   
        /// <param name="dispatchOperation"></param>   
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(this);
        }

        /// <summary>   
        ///    
        /// </summary>   
        /// <param name="operationDescription"></param>   
        public void Validate(OperationDescription operationDescription)
        {

        }

        #endregion

        internal event WcfBeforeCall WcfBeforeCallEvent;
        internal event WcfAfterCall WcfAfterCallEvent;

        /// <summary>   
        /// 调用方法后 输出结果值   
        /// </summary>   
        /// <param name="operationName"></param>   
        /// <param name="outputs"></param>   
        /// <param name="returnValue"></param>   
        /// <param name="correlationState"></param>   
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            try
            {
                if (WcfAfterCallEvent != null)
                {
                    OperationContext context = OperationContext.Current;
                    string AbsolutePath = "";
                    if (context != null)
                    {
                        //获取客户端请求的路径
                        AbsolutePath = context.EndpointDispatcher.EndpointAddress.Uri.AbsolutePath;
                    }
                    WcfAfterCallEvent(operationName, outputs, returnValue, correlationState, AbsolutePath);

                    #region 测试使用
                    //Console.WriteLine("返回操作结束：" + AbsolutePath + "/" + operationName);
                    //Console.WriteLine("*************返回操作编号：" + correlationState.ToString() + "**************");
                    //for (int i = 0; i < outputs.Length; i++)
                    //{

                    //    Type T = outputs[i].GetType();
                    //    Console.WriteLine("返回操作参数" + i.ToString() + "  类型为：" + T.ToString());
                    //    Console.WriteLine("返回操作参数" + i.ToString() + "  ToString为：" + outputs[i].ToString());
                    //    Console.WriteLine("返回操作参数" + i.ToString() + "  属性：");
                    //    PropertyInfo[] PIs = T.GetProperties();
                    //    foreach (PropertyInfo PI in PIs)
                    //    {
                    //        Console.Write(PI.Name + ":");
                    //        Console.WriteLine(PI.GetValue(outputs[i], null));
                    //    }


                    //}

                    //Type Treturn = returnValue.GetType();
                    //Console.WriteLine("操作返回值" + "  类型为：" + Treturn.ToString());
                    //Console.WriteLine("操作返回值" + "  ToString为：" + Treturn.ToString());
                    //Console.WriteLine("操作返回值" + "  属性：");

                    //if (Treturn.ToString() != "System.String")
                    //{
                    //    PropertyInfo[] PIreturns = Treturn.GetProperties();
                    //    foreach (PropertyInfo PI in PIreturns)
                    //    {
                    //        Console.Write(PI.Name + ":");
                    //        Console.WriteLine(PI.GetValue(returnValue, null));
                    //    }
                    //}
                    #endregion
                }
            }
            catch { }

        }
        /// <summary>   
        /// 调用方法前 输出参数值   
        /// </summary>   
        /// <param name="operationName"></param>   
        /// <param name="inputs"></param>   
        /// <returns></returns>   
        public object BeforeCall(string operationName, object[] inputs)
        {
            String guid = Guid.NewGuid().ToString();

            try
            {
                if (WcfBeforeCallEvent != null)
                {
                    OperationContext context = OperationContext.Current;
                    string AbsolutePath = "";
                    if (context != null)
                    {
                        //获取传递的自定义消息头
                        HeaderContext headercontext = HeaderOperater.GetServiceWcfHeader(context);
                        string wcfappname = HeaderOperater.GetServiceWcfAppNameHeader(context);
                        wcfappname = wcfappname == null ? "" : wcfappname;
                        if (headercontext != null)
                            guid = headercontext.CorrelationState;

                        //获取客户端请求的路径
                        AbsolutePath = context.EndpointDispatcher.EndpointAddress.Uri.AbsolutePath;

                        //获取客户端ip和端口
                        MessageProperties properties = context.IncomingMessageProperties;
                        RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                        string client_ip = endpoint.Address;
                        //int client_port = endpoint.Port;

                        if (!AbsolutePath.Contains("Eltc.Base/FrameWork/Helper/Wcf"))
                        {
                            Hashtable ht = new Hashtable();
                            ht.Add("ip", client_ip + "_" + wcfappname);
                            ht.Add("url", AbsolutePath);
                            ht.Add("operatename", operationName);
                            //MonitorData.Instance.UpdateOperateNums(client_ip, AbsolutePath, operationName);
                            Thread th = new Thread(new ParameterizedThreadStart(Run));
                            th.Start(ht);
                        }
                    }
                    WcfBeforeCallEvent(operationName, inputs, AbsolutePath, guid);

                    #region
                    //Console.WriteLine("返回操作开始：" + AbsolutePath + "/" + operationName);
                    //Console.WriteLine("*************调用操作编号：" + guid.ToString() + "**************");
                    //for (int i = 0; i < inputs.Length; i++)
                    //{

                    //    Type T = inputs[i].GetType();
                    //    Console.WriteLine("操作参数" + i.ToString() + "  类型为：" + T.ToString());
                    //    Console.WriteLine("操作参数" + i.ToString() + "  ToString为：" + inputs[i].ToString());
                    //    Console.WriteLine("操作参数" + i.ToString() + "  属性：");
                    //    PropertyInfo[] PIs = T.GetProperties();
                    //    foreach (PropertyInfo PI in PIs)
                    //    {
                    //        Console.Write(PI.Name + ":");
                    //        Console.WriteLine(PI.GetValue(inputs[i], null));
                    //    }

                    //}
                    #endregion
                }
            }
            catch { }

            return guid;
        }

        private void Run(object operateht)
        {
            Hashtable ht = (Hashtable)operateht;

            MonitorData.Instance.UpdateOperateNums((string)ht["ip"], (string)ht["url"], (string)ht["operatename"]);

            Thread.CurrentThread.Abort();
        }

    }
}