using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader
{
    public class ClientMessageInspector : IClientMessageInspector, IEndpointBehavior
    {

        #region Implementation for IClientMessageInspector
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState) 
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            //先删除消息头
            HeaderOperater.RemoveHeader(request.Headers);
            HeaderOperater.RemoveAppNameHeader(request.Headers);
            //设定传递的上下文信息
            MessageHeader hContext = HeaderOperater.GetClientWcfHeader();
            if (hContext != null)
            {
                HeaderOperater.AddHeader(request.Headers, hContext);
                //Console.WriteLine("取到上下文:"+hContext.ToString());
            }
            MessageHeader appnamehContext = HeaderOperater.GetClientWcfAppNameHeader();
            if (appnamehContext != null)
            {
                HeaderOperater.AddHeader(request.Headers, appnamehContext);
            }
            return null;
        }
        #endregion

        #region Implementation for IEndpointBehavior
        //==================================  
        public void AddBindingParameters(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
            //此处为Extension附加到ClientRuntime。  
            behavior.MessageInspectors.Add(this);
        }

        public void ApplyDispatchBehavior(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            //如果是扩展服务器端的MessageInspector，则要附加到EndpointDispacther上了。  
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);  
        }

        public void Validate(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint) { }
        #endregion
    }
}
