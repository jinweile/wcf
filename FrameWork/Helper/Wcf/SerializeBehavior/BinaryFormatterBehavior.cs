using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.ServiceModel.Configuration;

namespace Eltc.Base.FrameWork.Helper.Wcf.SerializeBehavior
{
    internal class BinaryFormatterBehavior : IEndpointBehavior
    {
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                DecorateFormatterBehavior(operation, clientRuntime);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (var operation in endpoint.Contract.Operations)
            {
                DecorateFormatterBehavior(operation, endpointDispatcher.DispatchRuntime);
            }
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void Validate(ServiceEndpoint endpoint) { }

        private static void DecorateFormatterBehavior(OperationDescription operation, object runtime)
        {
            //这个行为附加一次。 
            var dfBehavior = operation.Behaviors.Find<BinaryFormatterOperationBehavior>();
            if (dfBehavior == null)
            {
                //装饰新的操作行为 
                //这个行为是操作的行为，但是我们期望只为当前终结点做操作的序列化，所以传入 runtime 进行区分。
                dfBehavior = new BinaryFormatterOperationBehavior(runtime);
                operation.Behaviors.Add(dfBehavior);
            }
        }
    }

    /// <summary> 
    /// 在原始 Formatter 的基础上装饰 BinaryFormatterAdapter 
    /// <remarks> 
    /// BinaryFormatterOperationBehavior 为什么要实现为操作的行为： 
    /// 因为只有当操作的 DataContractSerializerBehavior 行为应用功能后，才能拿到 DataContractSerializerFormatter 并包装到 BinaryFormatterAdapter 中。 
    ///  
    /// 由于一个操作的操作契约在系统中只有一份。而我们期望序列化的行为只影响指定的终结点，所以这个行为在应用时，会检查是否传入的运行时，即是添加时的运行时。 
    /// </remarks> 
    /// </summary> 
    internal class BinaryFormatterOperationBehavior : IOperationBehavior
    {
        private object _runtime;

        internal BinaryFormatterOperationBehavior(object runtime)
        {
            _runtime = runtime;
        }

        /// <summary> 
        /// 本行为只为这个运行时起作用。 
        /// </summary> 
        public object ParentRuntime
        {
            get { return _runtime; }
        }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation runtime)
        {
            if (_runtime == runtime.Parent)
            {
                //在之前的创建的 Formatter 的基础上，装饰新的 Formatter
                runtime.Formatter = new BinaryFormatterAdapter(description.Name, runtime.SyncMethod.GetParameters(), runtime.Formatter, runtime.Action);
            }
        }

        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation runtime)
        {
            if (_runtime == runtime.Parent)
            {
                runtime.Formatter = new BinaryFormatterAdapter(description.Name, description.SyncMethod.GetParameters(), runtime.Formatter);
            }
        }

        public void AddBindingParameters(OperationDescription description, BindingParameterCollection parameters) { }

        public void Validate(OperationDescription description) { }
    }


    /// <summary> 
    /// 在内部序列化器的基础上添加 Remoting 二进制序列化的功能。 
    /// </summary> 
    internal class BinaryFormatterAdapter : IClientMessageFormatter, IDispatchMessageFormatter
    {
        private IClientMessageFormatter _innerClientFormatter;
        private IDispatchMessageFormatter _innerDispatchFormatter;
        private ParameterInfo[] _parameterInfos;
        private string _operationName;
        private string _action;

        /// <summary> 
        /// for client 
        /// </summary> 
        /// <param name="operationName"></param> 
        /// <param name="parameterInfos"></param> 
        /// <param name="innerClientFormatter"></param> 
        /// <param name="action"></param> 
        public BinaryFormatterAdapter(
            string operationName,
           ParameterInfo[] parameterInfos,
             IClientMessageFormatter innerClientFormatter,
             string action
            )
        {
            if (operationName == null) throw new ArgumentNullException("methodName");
            if (parameterInfos == null) throw new ArgumentNullException("parameterInfos");
            if (innerClientFormatter == null) throw new ArgumentNullException("innerClientFormatter");
            if (action == null) throw new ArgumentNullException("action");

            this._innerClientFormatter = innerClientFormatter;
            this._parameterInfos = parameterInfos;
            this._operationName = operationName;
            this._action = action;
        }

        /// <summary> 
        /// for server 
        /// </summary> 
        /// <param name="operationName"></param> 
        /// <param name="parameterInfos"></param> 
        /// <param name="innerDispatchFormatter"></param> 
        public BinaryFormatterAdapter(
            string operationName,
           ParameterInfo[] parameterInfos,
           IDispatchMessageFormatter innerDispatchFormatter
           )
        {
            if (operationName == null) throw new ArgumentNullException("operationName");
            if (parameterInfos == null) throw new ArgumentNullException("parameterInfos");
            if (innerDispatchFormatter == null) throw new ArgumentNullException("innerDispatchFormatter");

            this._innerDispatchFormatter = innerDispatchFormatter;
            this._operationName = operationName;
            this._parameterInfos = parameterInfos;
        }

        Message IClientMessageFormatter.SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            var result = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++) { result[i] = Serializer.SerializeBytes(parameters[i]); }

            return _innerClientFormatter.SerializeRequest(messageVersion, result);
        }

        object IClientMessageFormatter.DeserializeReply(Message message, object[] parameters)
        {
            var result = _innerClientFormatter.DeserializeReply(message, parameters);

            for (int i = 0; i < parameters.Length; i++) { parameters[i] = Serializer.DeserializeBytes(parameters[i] as byte[]); }
            result = Serializer.DeserializeBytes(result as byte[]);

            return result;
        }

        void IDispatchMessageFormatter.DeserializeRequest(Message message, object[] parameters)
        {
            _innerDispatchFormatter.DeserializeRequest(message, parameters);

            for (int i = 0; i < parameters.Length; i++) { parameters[i] = Serializer.DeserializeBytes(parameters[i] as byte[]); }
        }

        Message IDispatchMessageFormatter.SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            var seralizedParameters = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++) { seralizedParameters[i] = Serializer.SerializeBytes(parameters[i]); }
            var serialzedResult = Serializer.SerializeBytes(result);

            return _innerDispatchFormatter.SerializeReply(messageVersion, seralizedParameters, serialzedResult);
        }
    }


    /// <summary> 
    /// 序列化门户 API 
    /// </summary> 
    public static class Serializer
    {
        /// <summary> 
        /// 使用二进制序列化对象。 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static byte[] SerializeBytes(object value)
        {
            if (value == null) return null;

            var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, value);

            //var dto = Encoding.UTF8.GetString(stream.GetBuffer()); 
            var bytes = stream.ToArray();
            return bytes;
        }

        /// <summary> 
        /// 使用二进制反序列化对象。 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static object DeserializeBytes(byte[] bytes)
        {
            if (bytes == null) return null;

            //var bytes = Encoding.UTF8.GetBytes(dto as string); 
            var stream = new MemoryStream(bytes);

            var result = new BinaryFormatter().Deserialize(stream);

            return result;
        }
    }

    //用例
    // <system.serviceModel> 
    //     <behaviors> 
    //         <endpointBehaviors> 
    //             <behavior name="enableRemotingBinarySerialization"> 
    //                <remotingBinarySerialization/> 
    //             </behavior> 
    //       </endpointBehaviors> 
    //    </behaviors> 
    //    <extensions> 
    //         <behaviorExtensions> 
    //            <add name="remotingBinarySerialization" type="OEA.WCF.EnableBinaryFormatterBehaviorElement, OEA"/> 
    //        </behaviorExtensions> 
    //    </extensions> 
    //</system.serviceModel>
    /// <summary> 
    /// 启用旧的 BinaryFormatter 来对数据进行序列化。
    /// 可配置endpointBehaviors节点
    /// </summary> 
    public class EnableBinaryFormatterBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(BinaryFormatterBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new BinaryFormatterBehavior();
        }
    }

}

