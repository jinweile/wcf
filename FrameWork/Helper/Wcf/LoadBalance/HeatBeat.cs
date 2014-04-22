using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Eltc.Base.FrameWork.Helper.Wcf.LoadBalance
{
    /// <summary>
    /// 维持心跳
    /// </summary>
    public class HeatBeat : CommonWcfBll, IHeatBeat
    {
        public void CallBack(bool flag)
        {
            //Console.WriteLine("接收到心跳数据:" + flag);
        }
    }
}
