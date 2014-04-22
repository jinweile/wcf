//全选控制
function CheckAllArea(obj) {
	$("input[@name='delid']").attr("checked", obj.checked);
}

//单选控制
function Checkbox(obj) {
	var len = $("input[@name='delid']").length;
	var chklen = $("input[@name='delid']:checked").length;
	if (len == chklen) {
		$("#check").attr("checked", true);
	} else {
		$("#check").attr("checked", false);
	}
}

//检查是否有删除项
function CheckDel() {
	if ($("input[@name='delid']:checked").length == 0) {
		alert("请选择操作项!");
		return false;
	}
	return confirm("确定操作?");
}