using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.Text;

using WcfStatistics.Model;
using WcfStatistics.Dao;
using WcfStatistics.Dao.Int;

using Eltc.Base.FrameWork.Helper.Wcf.Monitor;

public partial class _Default1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //加载下拉列表
            IWcfServerDao dao = CastleContext.Instance.GetService<IWcfServerDao>();
            IList<WcfServer> list = dao.FindAll();
            foreach (WcfServer obj in list)
            {
                ListItem li = new ListItem();
                li.Text = obj.Remark + ":" + obj.IP + ":" + obj.Point;
                li.Value = obj.IP + ":" + obj.Point;

                this.IP.Items.Add(li);
            }
        }
    }

    protected string table = "";

    //检索wcf监控数据
    protected void Button1_Click(object sender, EventArgs e)
    {
        string ip = this.IP.SelectedValue;

        NetTcpBinding binging = new NetTcpBinding();
        binging.MaxBufferPoolSize = 524288;
        binging.MaxBufferSize = 2147483647;
        binging.MaxReceivedMessageSize = 2147483647;
        binging.MaxConnections = 1000;
        binging.ListenBacklog = 200;
        binging.OpenTimeout = TimeSpan.Parse("00:10:00");
        binging.ReceiveTimeout = TimeSpan.Parse("00:10:00");
        binging.TransferMode = TransferMode.Buffered;
        binging.Security.Mode = SecurityMode.None;

        binging.SendTimeout = TimeSpan.Parse("00:10:00");
        binging.ReaderQuotas.MaxDepth = 64;
        binging.ReaderQuotas.MaxStringContentLength = 2147483647;
        binging.ReaderQuotas.MaxArrayLength = 16384;
        binging.ReaderQuotas.MaxBytesPerRead = 4096;
        binging.ReaderQuotas.MaxNameTableCharCount = 16384;
        binging.ReliableSession.Ordered = true;
        binging.ReliableSession.Enabled = false;
        binging.ReliableSession.InactivityTimeout = TimeSpan.Parse("00:10:00");

        Dictionary<string, LinkModel> modellist = new Dictionary<string,LinkModel>();
        PCData pcdata;
        double memCount;
        using (MonitorClient client = MonitorClient.Instance)
        {
            IMonitorControl proxy = client.getProxy(ip, binging);
            modellist = proxy.GetMonitorInfo(out pcdata, out memCount);
        }

        StringBuilder sb_all = new StringBuilder();
        sb_all.Append("<table class=\"tab_style\" cellpadding=\"0\" cellspacing=\"1\" border=\"0\">");
        sb_all.Append("<tr><th>客户端IP</th><th>链接数</th><th>详细信息</th></tr>");
        int all_connnums = 0;
        StringBuilder sb = new StringBuilder();
        foreach (string key in modellist.Keys)
        {
            LinkModel model = modellist[key];
            Dictionary<string, UrlInfo> infolist = model.UrlInfoList;
            int ip_connnums = 0;
            string tr = "";
            foreach (string url in infolist.Keys)
            {
                UrlInfo info = infolist[url];
                all_connnums += info.ConnNums;
                ip_connnums += info.ConnNums;
                tr += "<tr><td class='title'>" + url + "</td><td>" + info.ConnNums + "</td><td colspan='2'><table  class=\"tab_style\" cellpadding=\"0\" cellspacing=\"1\" border=\"0\">";

                foreach (string opereateName in info.OperateNums.Keys)
                {
                    tr += "<tr><td class='title' style='width:300px;'>" + opereateName + "</td><td class='title'>" + info.OperateNums[opereateName] + "</td></tr>";
                }

                tr += "</table></td></tr>";
            }

            sb.Append("<tr><td>" + model.ClientIP + "</td><td>" + ip_connnums + "</td><td>"+
                "<table  class=\"tab_style\" cellpadding=\"0\" cellspacing=\"1\" border=\"0\"><tr>" +
                "<th style='width:450px;background:yellow;'>契约地址</th>" +
                "<th style='width:60px;background:yellow;'>链接数</th>" +
                "<th class='title' style='width:300px;background:yellow;'>操作名称</th>"+
                "<th class='title' style='background:yellow;'>调用次数</th>" +
                "</tr>" + tr + "</table></td></tr>");
        }

        //string serverdata = "进程ID：<font color='red'>" + pcdata.ProcessId + "</font>&nbsp;&nbsp;"
        //                            + "cpu：<font color='red'>" + pcdata.Cpu.ToString("F1") + "%" + "</font>&nbsp;&nbsp;"
        //                            + "内存：<font color='red'>" + pcdata.Mem.ToString("F1") + "</font>M&nbsp;&nbsp;"
        //                            + "当前线程数：<font color='red'>" + pcdata.ThreadCount + "</font>";


        string serverdata = @"<table>
            <tr>
                <td rowspan='2' width='60px'>进程ID：</td>
                <td class='title' rowspan='2' width='60px'><font color='red'>" + pcdata.ProcessId + @"</font></td>
                <td rowspan='2' width='60px'>cpu：</td>
                <td class='title' width='160px'><font color='red'>" + pcdata.Cpu.ToString("F1") + @"%</td>
                <td rowspan='2' width='60px'>内存：</td>
                <td class='title' width='160px'><font color='red'>" + pcdata.Mem.ToString("F1") + @"</font>M/<font color='red'>" + memCount.ToString("F1") + @"</font>M</td>
                <td rowspan='2' width='80px'>当前线程数：</td>
                <td class='title' rowspan='2'><font color='red'>" + pcdata.ThreadCount + @"</font></td>
            </tr>
            <tr>
                <td class='title'><div class='optionbar' style='width: 120px;'><div style='width: " + (pcdata.Cpu * 120) + @"px;'></div></div></td>
                <td class='title'><div class='optionbar' style='width: 120px;'><div style='width: " + (pcdata.Mem / memCount * 120).ToString("F1") + @"px;'></div></div></td>
            </tr>
        </table>";

        sb_all.Append("<tr><td><font color='red'>当前链接数</font></td>"+
            "<td><font color='red'>" + all_connnums + "</font></td><td class='title'>" +
            serverdata + "</td></tr>");
        sb_all.Append(sb);
        sb_all.Append("</table>");
        table = sb_all.ToString();
    }

}