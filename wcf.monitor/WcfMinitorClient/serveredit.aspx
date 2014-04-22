<%@ Page Language="C#" AutoEventWireup="true" CodeFile="serveredit.aspx.cs" Inherits="serveredit" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>来源编辑</title>
    <link href="css/main_style.css"type="text/css" rel="stylesheet" />
	<link href="css/style.css"type="text/css" rel="stylesheet" />
	<script src="js/jquery.js" type="text/javascript"></script>
</head>
<body id="main_content">
    <form id="form1" runat="server">
    <div class="t">
	    <ul>
		    <li style="background:none;">您所在的位置：</li>
		    <li class="liclass">WCF监控中心</li>
            <li>监控服务器编辑</li>
	    </ul>
    </div>
    
    <div class="area">
    <h1>监控服务器编辑</h1>
        <ul class="label">
        <li>
            <span class="title">IP:</span>
            <span class="input_area">
            <asp:TextBox ID="IP" runat="server" Width="454px"></asp:TextBox></span>
            <span></span>
                
        </li>
        <li>
            <span class="title">端口:</span>
            <span class="input_area">
            <asp:TextBox ID="Point" runat="server" Width="454px"></asp:TextBox></span>
            <span></span>
        </li>
        <li>
            <span class="title">说明:</span>
            <span class="input_area">
            <asp:TextBox ID="Remark" runat="server" Width="454px"></asp:TextBox></span>
            <span></span>
        </li>
        <li>
        <span class="title"></span>
        <span class="input_area">
            <asp:Button ID="add" runat="server" Text="确定"
                CssClass="btn_style" Height="20px" onclick="add_Click"/>&nbsp;&nbsp;
            <input type="reset" value="重写" class="btn_style" style="height:20px;" />
        </span>
        </li>
        </ul>
    </div>
    </form>
</body>
</html>
