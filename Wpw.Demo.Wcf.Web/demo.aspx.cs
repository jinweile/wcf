using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Eltc.Base.FrameWork.Helper;

using WPW.Demo.Wcf.Interface;
using WPW.Demo.Wcf.Model;
using Eltc.Base.FrameWork.Helper.Wcf.SDMessageHeader;
using Com.Dianping.Cat;
using Com.Dianping.Cat.Message;

namespace Wpw.Demo.Wcf.Web
{
    public partial class demo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string filename = Server.MapPath("~/config/catclient.xml");
                Cat.Initialize(filename);
                ITransaction t = Cat.GetProducer().NewTransaction("URL", "demo.aspx");
                IMessageTree tree = Cat.GetManager().ThreadLocalMessageTree;

                //创建传递的上下文信息
                HeaderContext context = new HeaderContext();
                context.AppName = "";
                context.CorrelationState = tree.MessageId;
                context.RootID = tree.RootMessageId == null ? tree.MessageId : tree.RootMessageId;
                context.ParentID = Cat.GetManager().CreateMessageId();
                context.Ip = "";
                HeaderOperater.SetClientWcfHeader(context);
                Cat.GetProducer().LogEvent("URL", "Call", "0", "Call Start...");
                Cat.GetProducer().LogEvent("RemoteCall", "PigeonRequest", "0", context.ParentID);

                IUserBll bll = WcfClient.GetProxy<IUserBll>();
                IList<User> list = bll.FindAll();

                Cat.GetProducer().LogEvent("URL", "Call", "0", "Call End...");

                //注：上下文信息必须在创建后清除
                HeaderOperater.ClearClientWcfHeader();
                t.Status = "A";
                t.Complete();

                this.GridView1.DataSource = list;
                this.GridView1.DataBind();
            }
        }
    }
}