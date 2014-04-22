<%@ Page Language="C#" AutoEventWireup="true" CodeFile="wcfserverlist.aspx.cs" Inherits="wcfserverlist" %>
<%@ Register Assembly="AspNetPager" Namespace="Wuqi.Webdiyer" TagPrefix="webdiyer" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WCF监控服务器列表</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="css/main_style.css" type="text/css" rel="stylesheet" />
    <link href="css/style.css" type="text/css" rel="stylesheet" />
    <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/checkbox.js" type="text/javascript"></script>
    <script src="js/openDialogHelper.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript"></script>
</head>
<body id="main_content">
    <form id="form1" runat="server">
    <div class="t">
        <ul>
            <li style="background: none;">您所在的位置： </li>
            <li><a href="wcfserverlist.aspx" title="WCF监控服务器列表">WCF监控服务器列表</a> </li>
        </ul>
    </div>
    <h1>
        WCF监控服务器列表
    </h1>
    <div class="area">
        <div style="padding: 10px 0 25px 0;">
            <div style="float: left; padding-left: 20px;">
                <a href="Default.aspx">WCF监控中心</a>&nbsp;&nbsp;
            </div>
            <div style="float: right; padding-right: 20px;">
                <a href="javascript:void(0)" onclick="javascript:$.OpenDialogHelper.openModalDlg('serveredit.aspx',700,350)">
                    新增监控服务器</a>
            </div>
        </div>
        <!--列表start-->
        <table class="tab_style" cellpadding="0" cellspacing="1" border="0">
            <tr>
                <th style="width: 30px;">
                    <input type="checkbox" id="check" onclick="javascript:CheckAllArea(this);" />
                </th>
                <th style="width: 50px">
                    ID
                </th>
                <th style="width: 120px">
                    IP
                </th>
                <th style="width: 50px">
                    端口
                </th>
                <th>
                    说明
                </th>
                <th style="width: 120px">
                    创建时间
                </th>
                <th style="width: 50px">
                    修改
                </th>
                <th style="width: 50px">
                    删除
                </th>
            </tr>
            <asp:Repeater ID="Repeater1" runat="server" 
                onitemcommand="Repeater1_ItemCommand">
            <ItemTemplate>
					<tr
						onmouseover="c=this.style.backgroundColor;this.style.backgroundColor='#ccffaa';"
						onmouseout="this.style.backgroundColor=c;">
						<td>
							<input type="checkbox" name="delid" title=""
								value='<%# Eval("ID") %>'
								onclick="javascript:Checkbox(this);" />
						</td>
						<td>
							<%# Eval("ID") %>
						</td>
						<td>
							<%# Eval("IP")%>
						</td>
						<td>
							<%# Eval("Point")%>
						</td>
						<td class="title">
							<%# Eval("Remark")%>
						</td>
						<td>
							<%# Eval("CreateTime","{0:yyyy-MM-dd HH:mm}")%>
						</td>
						<td>
							<a href='javascript:void(0);'
								onclick="$.OpenDialogHelper.openModalDlg('serveredit.aspx?ID=<%# Eval("ID") %>',700,350)"
								class="btn_style_a"
								style="display: inline-block; width: 40px; margin-top: 2px;">修改</a>
						</td>
						<td>
                            <asp:LinkButton runat="server" CommandName="del" CommandArgument='<%# Eval("ID") %>' CssClass="btn_style_a" 
                               OnClientClick="if(!confirm('是否删除！')){return false;}" style="display: inline-block; width: 40px; margin-top: 2px;">删除</asp:LinkButton>
						</td>
					</tr>
			</ItemTemplate>
            </asp:Repeater>
        </table>
        <!--列表end-->
        <!--分页start-->
        <div style="margin-top: 20px; text-align: center;">
                <webdiyer:AspNetPager ID="myPager" runat="server" AlwaysShow="True" 
                    FirstPageText="首页"  LastPageText="尾页" NextPageText="下一页" PageSize="5" PrevPageText="上一页"  ShowCustomInfoSection="Left" 
                    ShowInputBox="Never" CustomInfoTextAlign="Left" LayoutType="Table" >
                </webdiyer:AspNetPager>
        </div>
        <!--分页end-->
    </div>
    </form>
</body>
</html>
