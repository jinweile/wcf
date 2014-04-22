using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Eltc.Base.FrameWork.Helper.Wcf.Monitor
{
    [ServiceContract]
    public interface IMonitorControl
    {
        /// <summary>
        /// 获取wcf监控信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Dictionary<string, LinkModel> GetMonitorInfo(out PCData pcdata, out double memCount);
    }
}
