using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using WcfStatistics.Model;
using WcfStatistics.Dao;
using WcfStatistics.Dao.Int;

public partial class serveredit : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            string id = Request.QueryString["ID"];
            if (!string.IsNullOrEmpty(id))
            {
                int ID = int.Parse(id);
                WcfServer obj = null;
                IWcfServerDao dao = CastleContext.Instance.GetService<IWcfServerDao>();
                obj = dao.Find(ID);
                if (obj != null)
                {
                    this.IP.Text = obj.IP;
                    this.Point.Text = obj.Point.ToString();
                    this.Remark.Text = obj.Remark;
                }
            }
        }
    }

    //编辑
    protected void add_Click(object sender, EventArgs e)
    {
        string id = Request.QueryString["ID"];
        WcfServer obj = new WcfServer();
        obj.IP = this.IP.Text.Trim();
        obj.Point = int.Parse(this.Point.Text.Trim());
        obj.Remark = this.Remark.Text.Trim();
        IWcfServerDao dao = CastleContext.Instance.GetService<IWcfServerDao>();
        if (!string.IsNullOrEmpty(id))
        {
            obj.ID = int.Parse(id);
            dao.Update(obj);

            ClientScript.RegisterClientScriptBlock(this.GetType(), "sourceedit", "<script>window.parent.location = window.parent.location;</script>");
        }
        else
        {
            obj.CreateTime = DateTime.Now;
            dao.Insert(obj);

            ClientScript.RegisterClientScriptBlock(this.GetType(), "sourceadd", "<script>window.parent.location = window.parent.location;</script>");
        }
    }
}