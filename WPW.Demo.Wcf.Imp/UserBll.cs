using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

using Eltc.Base.FrameWork;

using WPW.Demo.Wcf.Interface;
using WPW.Demo.Wcf.Model;
using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;
using Com.Dianping.Cat.Message;
using Com.Dianping.Cat;

namespace WPW.Demo.Wcf.Imp
{
    public class UserBll : CommonWcfBll,  IUserBll
    {
        public IList<User> FindAll()
        {
            HeaderContext headercontext = HeaderOperater.GetServiceWcfHeader(OperationContext.Current);
            ITransaction t = Cat.GetProducer().NewTransaction("Data", "call wcf");
            IMessageTree tree = Cat.GetManager().ThreadLocalMessageTree;
            tree.RootMessageId = headercontext.RootID;
            tree.ParentMessageId = headercontext.CorrelationState;
            tree.MessageId = headercontext.ParentID;

            List<User> list = new List<User>();
            for (int i = 0; i < 30; i++)
            {
                User obj = new User();
                obj.UserID = i + 1;
                obj.UserName = "test" + (i + 1);

                list.Add(obj);
            }

            Cat.GetProducer().LogEvent("Data In.Server", "FindAll", "0", "Success");
            t.Status = "B";
            t.Complete();

            return list;
        }
    }
}
