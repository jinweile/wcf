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

public partial class _Default : System.Web.UI.Page
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
        Dictionary<string, string> ipdic = new Dictionary<string, string>();
        int all_connnums = 0;
        StringBuilder sb = new StringBuilder();
        foreach (string key in modellist.Keys)
        {
            LinkModel model = modellist[key];
            int ip_connnums = 0;

            //获取ip和应用名称
            string[] ipinfo = model.ClientIP.Split('_');
            string appip = ipinfo[0];
            string appname = ipinfo.Length == 1 ? "" : ipinfo[1];

            if (ipdic.ContainsKey(appip))
                continue;
            ipdic.Add(appip, null);

            string apptr = "";
            foreach (string keyall in modellist.Keys)
            {
                LinkModel modelall = modellist[keyall];
                string[] ipinfoall = modelall.ClientIP.Split('_');
                string appipall = ipinfoall[0];
                string appnameall = ipinfoall.Length == 1 ? "" : ipinfoall[1];
                if (appipall != appip)
                    continue;

                Dictionary<string, UrlInfo> infolist = modelall.UrlInfoList;
                int app_connnums = 0;
                string tr = "";
                foreach (string url in infolist.Keys)
                {
                    UrlInfo info = infolist[url];
                    all_connnums += info.ConnNums;
                    ip_connnums += info.ConnNums;
                    app_connnums += info.ConnNums;
                    tr += "<tr><td class='title'>" + url + "</td><td style='background:#bbffd3;'>" + info.ConnNums + "</td><td colspan='2'><table  class=\"tab_style\" cellpadding=\"0\" cellspacing=\"1\" border=\"0\">";

                    foreach (string opereateName in info.OperateNums.Keys)
                    {
                        tr += "<tr><td class='title' style='width:300px;'>" + opereateName + "</td><td class='title'>" + info.OperateNums[opereateName] + "</td></tr>";
                    }

                    tr += "</table></td></tr>";
                }

                apptr += apptr_template.Replace("{{appname}}", appnameall)
                                                        .Replace("{{conn_nums}}", app_connnums.ToString())
                                                        .Replace("{{tr}}", tr);
            }

            string apptable = "<table  class=\"tab_style\" cellpadding=\"0\" cellspacing=\"1\" border=\"0\"><tr>" +
                                        "<th style='width:120px;background:yellow;'>应用名</th>" +
                                        "<th style='width:60px;background:yellow;'>链接数</th>" +
                                        "<th class='title' style='background:yellow;'>契约</th>" +
                                        "</tr>" + apptr + "</table>";

            sb.Append("<tr><td>" + appip + "</td><td style='background:#bbffd3;'>" + ip_connnums + "</td><td>" + apptable + "</td></tr>");
        }

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

    private string apptr_template = "<tr><td>{{appname}}</td><td style='background:#bbffd3;'>{{conn_nums}}</td><td>" +
                                                        "<table  class=\"tab_style\" cellpadding=\"0\" cellspacing=\"1\" border=\"0\"><tr>" +
                                                        "<th style='width:450px;background:#c2fdff;'>契约地址</th>" +
                                                        "<th style='width:60px;background:#c2fdff;'>链接数</th>" +
                                                        "<th class='title' style='width:300px;background:#c2fdff;'>操作名称</th>" +
                                                        "<th class='title' style='background:#c2fdff;'>调用次数</th>" +
                                                        "</tr>{{tr}}</table></td></tr>";

}