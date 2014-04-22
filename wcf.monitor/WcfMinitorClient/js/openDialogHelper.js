//模拟模态窗口工具(定义命名空间OpenDialogHelper)absolute模式
jQuery.OpenDialogHelper = {
    //打开模态窗口
    openModalDlg: function (sPath, iWid, iHeig) {
        //模拟窗口浮动层
        divHtml = ""
	                + "<div class='area' id='_DialogDiv' style='background-position: #fff; display:none; border-style: double; border-color: #A0CDE4; background: #fff; position:absolute; top:20px; left:500px; padding:0px 0 0 0px; color:#000; float:left; z-index:1000;' >"
                    + "    <div style='background-color:#A0CDE4; text-align:right; padding-top:2px; padding-right:2px; height:15px;'>"
                    + "        <a href='javascript:void(0);' onclick='jQuery.OpenDialogHelper.CloseDialog()'><img alt='关闭' src='images/close.jpg' /></a></div>"
                    + "    <div style='padding:0 0 0 0'><iframe id='_dialogWindow' scrolling='auto' frameborder='0'></iframe></div>"
                    + "</div>";
        $("body").append(divHtml);
        iWid1 = iWid == 0 ? $(window).width() - 50 : iWid;
        iHeig1 = iHeig == 0 ? parseInt($(window).height()) - 50 : iHeig;
        sPath = sPath.indexOf("?") > 0 ? (sPath + "&zip=" + Math.random()) : (sPath + "?zip=" + Math.random());
        $("#_dialogWindow").attr("src", sPath).css({ width: "" + (iWid1 - 1) + "px", height: "" + (iHeig1 - 20) + "px" });
        $("#_DialogDiv").css({ width: "" + iWid1 + "px", height: "" + iHeig1 + "px" });
        //计算并设定浮动层位置
        var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;
        var divHeight = scrollTop + parseInt($(window).height()) - (parseInt($(window).height()) - parseInt($("#_DialogDiv").height())) / 2 - parseInt($("#_DialogDiv").height());
        var divWidth = (parseInt($(window).width()) - parseInt($("#_DialogDiv").width())) / 2 + scrollLeft;
        $("#_DialogDiv").css({ top: (divHeight <= 0 ? "10" : divHeight) + "px", left: divWidth + "px" });
        this.CreatMaskDiv();
        $("#_DialogDiv").show();

        $(window).bind("resize", function () {
            //改变蒙版的大小
            if ($('#_Dialog_MaskID')[0]) {
                var wnd = $(window), doc = $(document);
                if (wnd.height() > doc.height()) { //当高度少于一屏
                    wHeight = wnd.height();
                } else {//当高度大于一屏
                    wHeight = doc.height();
                }
                $("body").find("#_Dialog_MaskID").width(doc.width()).height(wHeight);
            }

            //改变对话框的位置
            if ($("#_DialogDiv")[0]) {
                iWid2 = iWid == 0 ? $(window).width() - 50 : iWid;
                iHeig2 = iHeig == 0 ? parseInt($(window).height()) - 50 : iHeig;
                $("#_dialogWindow").attr("src", sPath).css({ width: "" + (iWid2 - 1) + "px", height: "" + (iHeig2 - 20) + "px" });
                $("#_DialogDiv").css({ width: "" + iWid2 + "px", height: "" + iHeig2 + "px" });
                var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
                var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;
                var divHeight = scrollTop + parseInt($(window).height()) - (parseInt($(window).height()) - parseInt($("#_DialogDiv").height())) / 2 - parseInt($("#_DialogDiv").height());
                var divWidth = (parseInt($(window).width()) - parseInt($("#_DialogDiv").width())) / 2 + scrollLeft;
                $("#_DialogDiv").css({ top: (divHeight <= 0 ? "10" : divHeight) + "px", left: divWidth + "px" });
            }
        });
    },
    //关闭模态窗口
    CloseDialog: function () {
        $("#_DialogDiv").empty();
        $("#_DialogDiv").remove();
        this.DelMaskDiv();
        $(window).unbind("resize");
    },
    //创建遮罩层
    CreatMaskDiv: function () {
        var wnd = $(window), doc = $(document);
        if (wnd.height() > doc.height()) { //当高度少于一屏
            wHeight = wnd.height();
        } else {//当高度大于一屏
            wHeight = doc.height();
        }
        //创建遮罩背景
        $("body").append("<div ID=_Dialog_MaskID></div>");
        $("body").find("#_Dialog_MaskID").width(doc.width()).height(wHeight).css({ position: "absolute", top: "0px", left: "0px", background: "#000", filter: "Alpha(opacity=30);", opacity: "0.3", zIndex: "100" });
    },
    //关闭遮罩层
    DelMaskDiv: function () {
        $("#_Dialog_MaskID").empty();
        $("#_Dialog_MaskID").remove();
    }
};