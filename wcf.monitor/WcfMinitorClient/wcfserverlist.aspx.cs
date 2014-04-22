using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WcfStatistics.Model;
using WcfStatistics.Dao;
using WcfStatistics.Dao.Int;

public partial class wcfserverlist : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            IList<WcfServer> list = dao.FindAll();

            this.Repeater1.DataSource = list;
            this.Repeater1.DataBind();

            this.myPager.Visible = false;
        }
    }

    IWcfServerDao dao = CastleContext.Instance.GetService<IWcfServerDao>();

    //命令事件
    protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
        {
            //删除操作
            if (e.CommandName == "del")
            {
                //获取删除的ID
                int id = int.Parse(e.CommandArgument.ToString());
                WcfServer obj = new WcfServer() { ID = id };
                dao.Delete(obj);

                ClientScript.RegisterClientScriptBlock(this.GetType(), "del", "<script>window.location.href = window.location.href;</script>");
            }
        }
    }
}