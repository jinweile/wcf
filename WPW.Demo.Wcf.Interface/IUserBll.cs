using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;

using WPW.Demo.Wcf.Model;

namespace WPW.Demo.Wcf.Interface
{
    [ServiceContract]
    public interface IUserBll
    {
        [OperationContract]
        IList<User> FindAll();
    }
}
