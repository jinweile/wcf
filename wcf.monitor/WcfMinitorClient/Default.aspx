<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>WCF监控中心</title>
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
    <h1>
        WCF监控中心
    </h1>
    <div class="area">
        <div style="padding: 10px 0 25px 0;">
            <div style="float: left; padding-left: 20px;">
                <a href="wcfserverlist.aspx">WCF监控服务器列表</a>&nbsp;&nbsp;
            </div>
        </div>
        <div class="search" style="display: inline-block;">
            wcf服务器地址：
            <asp:DropDownList ID="IP" runat="server">
            </asp:DropDownList>
            &nbsp;
            <asp:Button ID="Button1" runat="server" CssClass="btn_style" Text=" 检 索 " OnClick="Button1_Click" />
        </div>
        <!--列表start-->
        <%=table%>
        <!--列表end-->
    </div>
    </form>
</body>
</html>
