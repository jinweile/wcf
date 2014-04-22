/*
* My97 日期控件 My97 DatePicker Ver 2.1 Build:20070122
* Blog: http://blog.csdn.net/my97/
* Mail: smallcarrot@163.com
*/
/* 设置 */
var dpcfg = {};
/* 默认风格 如果你喜欢whyGreen这个样式,你可以改成whyGreen 另外你还可以自定义自己的样式 */
dpcfg.skin = "default";
/* 日期格式 %Y %M %D %h %m %s 表示年月日时分秒(注意大小写) */
dpcfg.dateFmt = "%Y-%M-%D";
/* 是否显示时间 */
dpcfg.showTime = false;
/* 是否高亮显示 周六 周日 */
dpcfg.highLineWeekDay = true;
/* 日期范围 */
dpcfg.minDate = "1900-1-1";
dpcfg.maxDate = "2099-12-30";
/* 纠错模式设置 可设置3中模式 0 - 提示 1 - 自动纠错 2 - 标记 */
dpcfg.errDealMode = 0;
/* 纠错提示信息,仅当提示提示模式为0时有效 */
dpcfg.errAlertMsg = "不合法的日期格式或者日期超出限定范围,需要撤销吗?";
/* 语言设置 */
dpcfg.aWeekStr = ["日", "一", "二", "三", "四", "五", "六"];
dpcfg.aMonStr = ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一", "十二"];
dpcfg.todayStr = "今天";
dpcfg.okStr = "确定";
dpcfg.timeStr = "时间";
dpcfg.monthStr = "月份";
dpcfg.yearStr = "年份";

var $d = null;
function __sb() {
    this.s = new Array();
    this.i = 0;
    this.a = function (t) {
        this.s[this.i++] = t;
    };
    this.j = function () {
        return this.s.join('');
    };
}
function WdatePicker(el, dateFmt, showTime, skin) {
    this.win = window;
    this.top = window;
    while (this.top.parent.document != this.top.document && this.top.parent.document.getElementsByTagName("frameset").length == 0) {
        this.top = this.top.parent;
    }
    this.q = new Date();
    this.aa = this.q.getFullYear();
    this.p = this.q.getMonth() + 1;
    this.z = this.q.getDate();
    this.be = this.q.getHours();
    this.as = this.q.getMinutes();
    this.bj = this.q.getSeconds();
    this.eCont = (typeof el == 'string') ? document.getElementById(el) : el;
    this.dateFmt = (dateFmt == null) ? dpcfg.dateFmt : dateFmt;
    this.showTime = (showTime != dpcfg.showTime) ? showTime : dpcfg.showTime;
    this.skin = (skin == null) ? dpcfg.skin : skin;
    var c = this.eCont.getAttribute("MINDATE");
    if (c == null) {
        c = dpcfg.minDate;
    }
    if (c.substring(0, 1) == '$') {
        c = document.getElementById(c.substring(1));
        if (c) {
            if (c.getAttribute("REALVALUE") == null || c.getAttribute("REALVALUE") == "") {
                c = c.value;
            }
            else {
                c = c.getAttribute("REALVALUE");
            }
        }
        else {
            c = null;
        }
        if (c == null || c == '') {
            c = dpcfg.minDate;
        }
    }
    c = c.replace(/#Today/, this.aa + '/' + this.p + '/' + this.z);
    this.minDate = this.aw(c);
    c = this.eCont.getAttribute("MAXDATE");
    if (c == undefined) {
        c = dpcfg.maxDate;
    }
    if (c.substring(0, 1) == '$') {
        c = document.getElementById(c.substring(1));
        if (c) {
            if (c.getAttribute("REALVALUE") == null || c.getAttribute("REALVALUE") == "") {
                c = c.value;
            } else {
                c = c.getAttribute("REALVALUE");
            }
        }
        else {
            c = null;
        };
        if (c == null || c == '') {
            c = dpcfg.maxDate;
        }
    }
    c = c.replace(/#Today/, this.aa + '/' + this.p + '/' + this.z);
    this.maxDate = this.aw(c);
    if (this.top.document.dateDiv && this.top.document.dateDiv.obj.eCont == this.eCont) {
        $d = this.top.document.dateDiv;
    }
    else {
        this.top.document.dateDiv = null;
    }
    this.cssPath = 'default/datepicker.css';
    this.highLineWeekDay = dpcfg.highLineWeekDay;
    this.aWeekStr = dpcfg.aWeekStr;
    this.aMonStr = dpcfg.aMonStr;
    this.todayStr = dpcfg.todayStr;
    this.okStr = dpcfg.okStr;
    this.timeStr = dpcfg.timeStr;
    this.bo();
    this.bc(this.eCont.value, this.dateFmt);
    if (this.eCont.value != '' && this.eCont.getAttribute("REALVALUE") == null && this.l(this.eCont.value)) {
        this.am();
    }
    this.bt = this.year;
    this.bq = this.month;
    this.bs = this.date;
    if (this.top.document.dateDiv == undefined) {
        this.dd = this.top.document.createElement("DIV");
        this.dd.style.cssText = 'position:absolute;z-index:197;width:180px;';
        this.dd.obj = this; this.dd.className = "WdateDiv";
        this.dd.innerHTML = this.aq();
        var bv = this.top.document.createElement('iframe');
        this.dd.ifr = bv;
        var w = this.dd.getElementsByTagName('input');
        this.dd.mInput = w[0];
        this.dd.yInput = w[1];
        var ap = this.dd.getElementsByTagName('div');
        this.dd.mDiv = ap[2];
        this.dd.yDiv = ap[4];
        this.dd.dDiv = ap[5];
        this.dd.tDiv = ap[6].firstChild;
        this.dd.dDiv.innerHTML = this.u();
        this.o = function () {
            var evt = $d.obj.top.event;
            var k = (evt.which == undefined) ? evt.keyCode : evt.which;
            if (!((k >= 48 && k <= 57) || (k >= 96 && k <= 105) || k == 8 || k == 46 || k == 37 || k == 39)) {
                evt.returnValue = false;
            }
        };
        this.dd.mInput.attachEvent('onkeydown', this.o);
        this.dd.yInput.attachEvent('onkeydown', this.o);
        this.dd.yInput.onblur = function () {
            if (parseInt(this.value) != $d.obj.year) {
                $d.obj.redraw();
            }
            this.className = 'yminput';
        };
        this.dd.mInput.onblur = function () {
            if (this.value > 12) {
                this.value = '12';
            }
            else if (this.value < 1) {
                this.value = '1';
            } else if (parseInt(this.value) != $d.obj.month) {
                $d.obj.redraw();
            }
            this.className = 'yminput';
        };
        this.dd.mInput.onfocus = function () {
            this.className = 'yminputfocus';
            this.select();
            $d.obj._fillmonth();
            $d.mDiv.style.display = 'block';
        };
        this.dd.yInput.onfocus = function () {
            this.className = 'yminputfocus';
            this.select();
            $d.obj._fillyear();
            $d.yDiv.style.display = 'block';
        };
        this.dd.hhInput = w[2];
        this.dd.mmInput = w[4];
        this.dd.ssInput = w[6];
        this.dd.okInput = w[7];
        this.dd.hhInput.onfocus = this.dd.mmInput.onfocus = this.dd.ssInput.onfocus = function () {
            this.select();
            $d.obj.currFocus = this;
        };
        this.dd.hhInput.onblur = function () {
            if (parseInt(this.value) > 23) {
                this.value = '23';
            }
            else if (parseInt(this.value) < 0) {
                this.value = '0';
            }
        };
        this.dd.mmInput.onblur = this.dd.ssInput.onblur = function () {
            if (parseInt(this.value) > 59) {
                this.value = '59';
            } else if (parseInt(this.value) < 0) {
                this.value = '0';
            }
        };
        this.dd.hhInput.attachEvent('onkeydown', this.o);
        this.dd.mmInput.attachEvent('onkeydown', this.o);
        this.dd.ssInput.attachEvent('onkeydown', this.o);
        var bn = this.dd.getElementsByTagName('button');
        this.dd.upButton = bn[0];
        this.dd.downButton = bn[1];
        this.dd.upButton.onclick = function () {
            if ($d.obj.currFocus == undefined) {
                $d.obj.currFocus = $d.mmInput;
            }
            if (($d.obj.currFocus == $d.hhInput && parseInt($d.obj.currFocus.value) < 23) || ($d.obj.currFocus != $d.hhInput && parseInt($d.obj.currFocus.value) < 59)) {
                $d.obj.currFocus.value = parseInt($d.obj.currFocus.value) + 1;
            }
            $d.obj.currFocus.focus();
        };
        this.dd.downButton.onclick = function () {
            if ($d.obj.currFocus == undefined) {
                $d.obj.currFocus = $d.mmInput;
            } if (parseInt($d.obj.currFocus.value) > 0) {
                $d.obj.currFocus.value = parseInt($d.obj.currFocus.value) - 1;
            }
            $d.obj.currFocus.focus();
        };
        this.top.document.body.insertAdjacentElement('afterBegin', this.dd);
        this.top.document.body.insertAdjacentElement('beforeEnd', this.dd.ifr);
        this.top.document.dateDiv = this.dd; this.eCont.getValue = function () {
            if (this.value == '') {
                return '';
            }
            else {
                return this.getAttribute("REALVALUE");
            }
        };
        this.eCont.attachEvent('onkeydown', function () {
            if ($d.style.display != 'none') {
                var evt = $d.obj.top.event;
                var k = (evt.which == undefined) ? evt.keyCode : evt.which; if (k == 9) {
                    evt.returnValue = false;
                }
            }
        });
    }
    else {
        this.dd = this.top.document.dateDiv;
        this.dd.obj.win = this.win;
        this.dd.obj.top = this.top;
        this.dd.obj.eCont = this.eCont;
        this.dd.obj.showTime = this.showTime;
        this.dd.obj.dateFmt = this.dateFmt;
        this.dd.style.display = '';
        this.dd.mInput.value = this.month;
        this.dd.yInput.value = this.year;
        this.dd.dDiv.innerHTML = this.u();
        if (this.showTime) {
            this.dd.tDiv.style.display = 'block';
            this.dd.hhInput.value = this.hour;
            this.dd.mmInput.value = this.minute;
            this.dd.ssInput.value = this.sec;
        }
        else {
            this.dd.tDiv.style.display = 'none';
        }
    }
    this.bm();
    this._setOkInput();
    var bp = this.eCont.getBoundingClientRect();
    var mm = $getAbsM(this.top);
    var ar = $getClientWidthHeight(this.top);
    var bd = mm.topM + bp.bottom;
    var br = mm.leftM + bp.left;
    if ((bd + parseInt(this.dd.offsetHeight) < (ar.height)) || (bd - this.eCont.offsetHeight < this.dd.offsetWidth * 0.8)) {
        this.dd.style.top = (this.top.document.body.scrollTop + bd + 1) + 'px';
    }
    else {
        this.dd.style.top = (this.top.document.body.scrollTop + bd - parseInt(this.dd.offsetHeight) - this.eCont.offsetHeight - 3) + 'px';
    }
    this.dd.style.left = -1 + this.top.document.body.scrollLeft + Math.min(br, ar.width - parseInt(this.dd.offsetWidth) - 5) + 'px';
    this.dd.ifr.style.cssText = 'top:' + this.dd.style.top + ';left:' + (this.dd.style.left) + ';width:' + Math.min(180, (this.dd.offsetWidth - 1)) + 'px;height:' + (this.dd.offsetHeight - 1) + 'px;position:absolute;z-index:196;overflow:hidden;border:0px;filter:alpha(opacity=0);';
}
WdatePicker.prototype.ba = function () {
    var path = '';
    var i;
    var scripts = document.getElementsByTagName("script");
    for (i = 0; i < scripts.length; i++) { if (scripts[i].src.substring(scripts[i].src.length - 14).toLowerCase() == 'wdatepicker.js') { path = scripts[i].src.substring(0, scripts[i].src.length - 14); break; } } if (path.indexOf('://') == -1) { var a = this.top.location.href.toLowerCase(); var b = location.href.toLowerCase(); var al = '', bl = '', bls = ''; var j, s = ''; for (i = 0; i < Math.max(a.length, b.length); i++) { if (a.charAt(i) != b.charAt(i)) { j = i; while (a.charAt(j) != '/') { if (j == 0) { break; } j -= 1; } al = a.substring(j + 1, a.length); al = al.substring(0, al.indexOf('/')); bl = b.substring(j + 1, b.length); bl = bl.substring(0, bl.indexOf('/')); break; } } if (al != '') { for (i = 0; i < al.split('/').length; i++) { s += "../"; } } if (bl != '') { bls = bl.split('/'); for (i = 0; i < bls.length; i++) { s += bls[i] + '/'; } } path = s + path; } this.cssPath = path + this.skin.toLowerCase() + '/datepicker.css';
};
WdatePicker.prototype.bo = function () {
    if (!$d || ($d && $d.obj.skin != this.skin)) {
        if ($d) {
            $d.obj.skin = this.skin;
        }
        this.ba();
        var ay = true;
        for (var i = this.top.document.styleSheets.length - 1; i >= 0; i--) {
            tempStyle = this.top.document.styleSheets[i];
            if (tempStyle.href.substring(tempStyle.href.lastIndexOf('/') + 1).toLowerCase() == 'datepicker.css') {
                if (tempStyle.href.toLowerCase() == this.cssPath.toLowerCase()) {
                    ay = false; tempStyle.disabled = false; continue;
                } else {
                    tempStyle.disabled = true;
                }
            }
        }
        if (ay) {
            this.top.document.createStyleSheet(this.cssPath);
        }
    }
};
WdatePicker.prototype.bc = function (str, fmt) {
    this.year = this.month = this.date = this.hour = this.minute = this.sec = -1;
    var v = str.split(/\W+/);
    var f = fmt.match(/%./g);
    for (var i = 0; i < f.length; i++) {
        if (v[i]) {
            if (f[i].toLowerCase() == '%y') {
                this.year = parseInt(v[i], 10);
                if (isNaN(this.year)) {
                    this.year = this.aa;
                }
            } else if (f[i] == '%M') {
                this.month = parseInt(v[i], 10);
                if (isNaN(this.month)) {
                    this.month = this.p;
                }
            } else if (f[i].toLowerCase() == '%d') {
                this.date = parseInt(v[i], 10);
                if (isNaN(this.date)) {
                    this.date = this.z;
                }
            } else if (f[i].toLowerCase() == '%h') {
                this.hour = parseInt(v[i], 10);
                if (isNaN(this.hour)) {
                    this.hour = this.be;
                }
            } else if (f[i] == '%m') {
                this.minute = parseInt(v[i], 10);
                if (isNaN(this.minute)) {
                    this.minute = this.as;
                }
            } else if (f[i].toLowerCase() == '%s') {
                this.sec = parseInt(v[i], 10);
                if (isNaN(this.sec)) {
                    this.sec = this.bj;
                }
            }
        }
    } if (!this.az(this.year + '-' + this.month + '-' + this.date)) {
        this.year = this.aa;
        this.month = this.p;
        this.date = this.z;
    }
    if ((this.hour < 0) || (this.hour > 23)) {
        this.hour = this.be;
    }
    if ((this.minute < 0) || (this.minute > 59)) {
        this.minute = this.as;
    }
    if ((this.sec < 0) || (this.sec > 59)) {
        this.sec = this.bj;
    }
};
WdatePicker.prototype._fillmonth = function () { var s = new __sb(); s.a("<table cellspacing=0 cellpadding=2 border=0>"); var i, n = 0, v = parseInt(this.dd.mInput.value); var at = new Array(12); var t = new Array(12); for (i = 0; i < 11; i++) { if (i + 1 == v) { n = 1; } at[i] = this.aMonStr[n + i]; t[i] = n + i + 1; } this.year = parseInt(this.dd.yInput.value); var af = this.year == this.minDate.year; var ae = this.year == this.maxDate.year; var ax = (this.year > this.minDate.year && this.year < this.maxDate.year); var ac; for (i = 0; i < 6; i++) { s.a("<tr><td "); ac = (ax) || (af && t[i] >= this.minDate.month) || (ae && t[i] <= this.maxDate.month); s.a((ac) ? "class='Wym' onmouseover=\"this.className='WdayOn'\" onmouseout=\"this.className='Wym'\" onmousedown=\"$d.mInput.value='" + t[i] + "';$d.mDiv.style.display='none';$d.mInput.blur();\"" : "class='Winvalidym'"); s.a(">" + at[i] + "</td>"); if (i == 5) { break; }; s.a("<td "); ac = (ax) || (af && t[i + 6] >= this.minDate.month) || (ae && t[i + 6] <= this.maxDate.month); s.a((ac) ? "class='Wym' onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown=\"$d.mInput.value='" + t[i + 6] + "';$d.mDiv.style.display='none';$d.mInput.blur();\"" : "class='Winvalidym'"); s.a(">" + at[i + 6] + "</td></tr>"); } s.a("<td align=center onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown=\"$d.mDiv.style.display='none';\">×</td</tr>"); s.a("</table>"); this.dd.mDiv.innerHTML = s.j(); }; WdatePicker.prototype._fillyear = function (ao, au) { if (ao == null || au == null) { var v = parseInt(this.dd.yInput.value); ao = v - 5; au = v + 4; } var i; var a = new Array(au - ao); for (i = ao; i <= au; i++) { a[i - ao] = i; } var n = (a.length / 2); var s = new __sb(); var ah; s.a("<table cellspacing=0 cellpadding=2 border=0>"); for (i = 0; i < n; i++) { ah = (a[i] >= this.minDate.year && a[i] <= this.maxDate.year); s.a("<tr><td "); s.a((ah) ? "class='Wym' onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown=\"$d.yInput.value='" + a[i] + "';$d.yDiv.style.display='none';$d.yInput.blur();\"" : "class='Winvalidym'"); s.a(">" + a[i] + "</td><td "); ah = (a[i + n] >= this.minDate.year && a[i + n] <= this.maxDate.year); s.a((ah) ? "class='Wym' onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown=\"$d.yInput.value='" + a[i + n] + "';$d.yDiv.style.display='none';$d.yInput.blur();\"" : "class='Winvalidym'"); s.a(">" + a[i + n] + "</td></tr>"); } s.a("</table>"); s.a("<table cellspacing=0 cellpadding=3 border=0><tr><td "); s.a((this.minDate.year < ao) ? "class='Wym' onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown='$d.obj._fillyear(" + (ao - 10) + "," + (au - 10) + ")'" : "class='Winvalidym'"); s.a(">←</td><td class='Wym' onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown=\"$d.yDiv.style.display='none';$d.yInput.blur();\">×</td><td "); s.a((this.maxDate.year > au) ? "class='Wym' onmouseover=\"this.className='WymOn'\" onmouseout=\"this.className='Wym'\" onmousedown='$d.obj._fillyear(" + (ao + 10) + "," + (au + 10) + ")'" : "class='Winvalidym'"); s.a(">→</td></tr></table>"); this.dd.yDiv.innerHTML = s.j(); }; WdatePicker.prototype._returnDateStr = function (Y, M, D, h, m, s, fmt) { if (Y == null) { Y = this.year; } if (M == null) { M = this.month; } if (D == null) { D = this.date; } if (h == null) { h = this.hour; } if (m == null) { m = this.minute; } if (s == null) { s = this.sec; } if (fmt == null) { fmt = this.dateFmt; } var ab = fmt.replace(/%[Yy]/, this.ad(Y, 4)).replace(/%[M]/, this.ad(M, 2)).replace(/%[Dd]/, this.ad(D, 2)); if (this.showTime) { ab = ab.replace(/%[Hh]/, this.ad(h, 2)).replace(/%[m]/, this.ad(m, 2)).replace(/%[Ss]/, this.ad(s, 2)); } return ab; }; WdatePicker.prototype.ad = function (s, len) { s = s + ''; for (var i = s.length; i < len; i++) { s = '0' + s; } return s; }; WdatePicker.prototype.aw = function (ab) { if (ab.indexOf(' ') != -1) { ab = ab.substring(0, ab.indexOf(' ')); } var s = ab.split(/\W+/); if (s.length >= 3 && this.az(s[0] + '/' + s[1] + '/' + s[2])) { return { 'year': parseInt(s[0], 10), 'month': parseInt(s[1], 10), 'date': parseInt(s[2], 10) }; } else { alert("日期范围格式错误\nInvalid MINDATE or MAXDATE\nFormat:YYYY-MM-DD or YYYY/MM/DD"); return; } }; WdatePicker.prototype.am = function (Y, M, D, h, m, s) { if (this.showTime) { this.eCont.setAttribute("REALVALUE", this._returnDateStr(Y, M, D, h, m, s, '%Y-%M-%D %h:%m:%s')); } else { this.eCont.setAttribute("REALVALUE", this._returnDateStr(Y, M, D, null, null, null, '%Y-%M-%D')); } }; WdatePicker.prototype.az = function (ab) { return ab.match(/^((\d{2}(([02468][048])|([13579][26]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|([1-2][0-9])))))|(\d{2}(([02468][1235679])|([13579][01345789]))[\-\/\s]?((((0?[13578])|(1[02]))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(3[01])))|(((0?[469])|(11))[\-\/\s]?((0?[1-9])|([1-2][0-9])|(30)))|(0?2[\-\/\s]?((0?[1-9])|(1[0-9])|(2[0-8]))))))(\s(((0?[0-9])|([1-2][0-3]))\:([0-5]?[0-9])((\s)|(\:([0-5]?[0-9])))))?$/); }; WdatePicker.prototype.l = function (sDateTime) { var Y, M, D, h, m, s; var v = sDateTime.split(/\W+/); var f = this.dateFmt.match(/%./g); for (var i = 0; i < f.length; i++) { if (f[i].toLowerCase() == '%y') { Y = Number(v[i]); if (isNaN(Y)) { return false; } } else if (f[i] == '%M') { M = Number(v[i]); if (isNaN(M)) { return false; } } else if (f[i].toLowerCase() == '%d') { D = Number(v[i]); if (isNaN(D)) { return false; } } else if (f[i].toLowerCase() == '%h') { h = Number(v[i]); if (isNaN(h)) { return false; } } else if (f[i] == '%m') { m = Number(v[i]); if (isNaN(m)) { return false; } } else if (f[i].toLowerCase() == '%s') { s = Number(v[i]); if (isNaN(s)) { return false; } } } Y = (Y == undefined) ? '2000' : Y; M = (M == undefined) ? '1' : M; D = (D == undefined) ? '1' : D; if (this.az(Y + '-' + M + '-' + D) && (h == undefined || (h >= 0) && (h <= 23)) && (s == undefined || (m >= 0) && (h <= 59)) && (s == undefined || (s >= 0) && (s <= 59))) { this.eCont.value = this._returnDateStr(Y, M, D, h, m, s); if (((Y * 10000 + M * 100 + D * 1) >= (this.minDate.year * 10000 + this.minDate.month * 100 + this.minDate.date * 1)) && ((Y * 10000 + M * 100 + D * 1) <= (this.maxDate.year * 10000 + this.maxDate.month * 100 + this.maxDate.date * 1))) { return true; } } return false; }; WdatePicker.prototype.aq = function () { var s = new __sb(); s.a("<div id=dpTitle>"); s.a("<div style='float:left;margin:2px'><div class='ymsel'></div>" + dpcfg.monthStr + "<input class='yminput' style='width:40px;' maxlength=2 value=" + this.month + "></div>"); s.a("<div style='float:right;margin:2px'><div class='ymsel'></div>" + dpcfg.yearStr + "<input class='yminput' style='width:50px;' maxlength=4 value=" + this.year + "></div></div>"); s.a("<div></div>"); s.a("<div>"); s.a(this.av()); s.a("</div>"); return s.j(); }; WdatePicker.prototype.av = function () { var s = new __sb(); s.a("<div id=dpTime style='" + ((this.showTime) ? '' : 'display:none;') + "float:left;margin-top:3px'><table cellspacing=0 cellpadding=0 border=0><tr><td rowspan=2><span id=dpTimeStr>" + this.timeStr + "</span>"); s.a(" <input class=tB maxlength=2 value=" + this.hour + "><input value=':' class=tm readonly>"); s.a("<input class=tE maxlength=2 value=" + this.minute + "><input value=':' class=tm readonly>"); s.a("<input class=tE maxlength=2 value=" + this.sec + "></td><td>"); s.a("<button id=dpTimeUp></button></td></tr><tr><td><button id=dpTimeDown></button></td></tr></table></div>"); s.a("<div style='float:right;margin-top:3px;text-align:right'>"); s.a("<input id=dpOkInput type=button style='height:20px;width:90%'></input>"); s.a("</div>"); return s.j(); }; WdatePicker.prototype._setOkInput = function () { var d = $d.obj; if (d.eCont.value == "") { if (((this.aa * 10000 + this.p * 100 + this.z * 1) >= (this.minDate.year * 10000 + this.minDate.month * 100 + this.minDate.date * 1)) && ((this.aa * 10000 + this.p * 100 + this.z * 1) <= (this.maxDate.year * 10000 + this.maxDate.month * 100 + this.maxDate.date * 1))) { $d.okInput.onclick = function () { var d = $d.obj; d.pickDate(d.aa, d.p, d.z, d.be, d.as, d.bj); }; } else { $d.okInput.disabled = "disabled"; } $d.okInput.value = d.todayStr + ((d.showTime) ? "" : (" " + d.ad(d.aa, 4) + '-' + d.ad(d.p, 2) + '-' + d.ad(d.z, 2))); } else { $d.okInput.onclick = function () { $d.obj.pickDate(); }; $d.okInput.value = d.okStr; } }; WdatePicker.prototype.u = function () { var e, tempMonth; if ((this.year * 100 + this.month * 1) < (this.minDate.year * 100 + this.minDate.month * 1)) { this.dd.yInput.value = e = this.minDate.year; this.dd.mInput.value = tempMonth = this.minDate.month; } else if ((this.year * 100 + this.month * 1) > (this.maxDate.year * 100 + this.maxDate.month * 1)) { this.dd.yInput.value = e = this.maxDate.year; this.dd.mInput.value = tempMonth = this.maxDate.month; } else { e = this.year; tempMonth = this.month; } var bg, bb, lastDay, lastDate; var s = new __sb(); var i, j, k; bg = new Date(e, tempMonth - 1, 1).getDay(); bb = 1 - bg; lastDay = new Date(e, tempMonth, 0).getDay(); lastDate = new Date(e, tempMonth, 0).getDate(); s.a("<table id=dpDayTable width=100% border=0 cellspacing=0 cellpadding=0>"); s.a("<tr id=dpWeekTitle align=center>"); var ss = new Array(); for (i = 0; i < 7; i++) { s.a("<td>" + this.aWeekStr[i] + "</td>"); } var ag = ''; var bu = ''; var bi = ((e == this.aa) && (tempMonth == this.p)); var bk = ((e == this.bt) && (tempMonth == this.bq)); var aj = ((e * 100 + tempMonth * 1) == (this.minDate.year * 100 + this.minDate.month)); var ai = ((e * 100 + tempMonth * 1) == (this.maxDate.year * 100 + this.maxDate.month)); var bh = !aj && !ai; for (i = 1, j = bb; i < 7; i++) { s.a("<tr>"); for (k = 0; k < 7; k++) { if (j >= 1 && j <= lastDate) { if (bk && (j == this.bs)) { ag = 'Wselday'; } else if (bi && (j == this.z)) { ag = 'Wtoday'; } else { ag = ((this.highLineWeekDay && (k == 0 || k == 6)) ? 'Wwday' : 'Wday'); } classOnStr = ((this.highLineWeekDay && (k == 0 || k == 6)) ? 'WwdayOn' : 'WdayOn'); s.a("<td align=center "); if (bh || ((aj && j >= this.minDate.date) || (ai && j <= this.maxDate.date))) { s.a("onclick=\"$d.obj.pickDate(null,null," + j + ");\" "); s.a("onmouseover=\"this.className='" + classOnStr + "'\" "); s.a("onmouseout=\"this.className='" + ag + "'\" "); } else { ag = 'WinvalidDay'; } s.a("class=" + ag); s.a("><span>" + j + "</span>"); } else { s.a("<td><span></span>"); } j++; s.a("</td>"); } s.a("</tr>"); } s.a("</table>"); return s.j(); }; WdatePicker.prototype.bm = function () { this.top.$d = this.top.document.dateDiv; $d = this.top.$d; }; WdatePicker.prototype.redraw = function () { this.year = this.dd.yInput.value; this.month = this.dd.mInput.value; this.dd.dDiv.innerHTML = this.u(); }; WdatePicker.prototype.pickDate = function (Y, M, D, h, m, s) { if (Y == null) { Y = this.dd.yInput.value; } if (M == null) { M = this.dd.mInput.value; } if (D == null) { D = this.date; } this.year = Y; this.month = M; this.date = D; if (this.showTime) { if (h == null) { h = this.dd.hhInput.value; } if (m == null) { m = this.dd.mmInput.value; } if (s == null) { s = this.dd.ssInput.value; } this.hour = h; this.minute = m; this.sec = s; this.eCont.value = this._returnDateStr(Y, M, D, h, m, s); } else { this.eCont.value = this._returnDateStr(Y, M, D); } this.am(Y, M, D, h, m, s); $d.obj._markValue(true); this.dd.style.display = 'none'; this.dd.ifr.style.display = 'none'; }; WdatePicker.prototype._markValue = function (bValue) { if (bValue) { this.eCont.className = this.eCont.className.replace(/ WdateFmtErr/, ''); } else { var bf = dpcfg.errDealMode; while (true) { switch (bf) { case 0: if (!confirm(dpcfg.errAlertMsg)) { bf = 2; continue; } case 1: if (this.eCont.getAttribute("REALVALUE")) { this.l(this.eCont.getAttribute("REALVALUE")) } else { this.eCont.value = ""; } this.eCont.className = this.eCont.className.replace(/ WdateFmtErr/, ''); break; case 2: this.eCont.className = this.eCont.className.replace(/ WdateFmtErr/, ''); this.eCont.className = this.eCont.className.replace(/Wdate/, 'Wdate WdateFmtErr'); break; } break; } } }; if (navigator.product == 'Gecko') { Window.prototype.__defineGetter__("screenLeft", function () { return this.screenX + (this.outerWidth - this.innerWidth); }); Window.prototype.__defineGetter__("screenTop", function () { return screenY + (window.outerHeight - window.innerHeight); }); Document.prototype.attachEvent = function (sType, fHandler) { var ak = sType.replace(/on/, ""); fHandler._ieEmuEventHandler = function (e) { window.event = e; return fHandler(); }; this.addEventListener(ak, fHandler._ieEmuEventHandler, false); }; Document.prototype.createStyleSheet = function (cssPath) { var head = document.getElementsByTagName('HEAD').item(0); var style = document.createElement('link'); style.href = cssPath; style.rel = 'stylesheet'; style.type = 'text/css'; head.appendChild(style); }; Event.prototype.__defineSetter__("returnValue", function (value) { if (!value) { this.preventDefault(); } return value; }); Event.prototype.__defineGetter__("srcElement", function () { var node = this.target; while (node.nodeType != 1) { node = node.parentNode; } return node; }); Node.prototype.replaceNode = function (Node) { this.parentNode.replaceChild(Node, this); }; Node.prototype.removeNode = function (removeChildren) { if (removeChildren) { return this.parentNode.removeChild(this); } else { var range = document.createRange(); range.selectNodeContents(this); return this.parentNode.replaceChild(range.extractContents(), this); } }; HTMLElement.prototype.__defineSetter__("outerHTML", function (sHTML) { var r = this.ownerDocument.createRange(); r.setStartBefore(this); var df = r.createContextualFragment(sHTML); this.parentNode.replaceChild(df, this); return sHTML; }); HTMLElement.prototype.__defineGetter__("outerHTML", function () { var attr; var attrs = this.attributes; var str = "<" + this.tagName; for (var i = 0; i < attrs.length; i++) { attr = attrs[i]; if (attr.specified) { str += " " + attr.name + '="' + attr.value + '"'; } } if (!this.canHaveChildren) { return str + ">"; } return str + ">" + this.innerHTML + "</" + this.tagName + ">"; }); HTMLElement.prototype.__defineGetter__("parentElement", function () { if (this.parentNode == this.ownerDocument) { return null; } return this.parentNode; }); HTMLElement.prototype.attachEvent = function (sType, fHandler) { var ak = sType.replace(/on/, ""); fHandler._ieEmuEventHandler = function (e) { window.event = e; return fHandler(); }; this.addEventListener(ak, fHandler._ieEmuEventHandler, false); }; HTMLElement.prototype.insertAdjacentElement = function (where, parsedNode) { switch (where) { case "beforeBegin": this.parentNode.insertBefore(parsedNode, this); break; case "afterBegin": this.insertBefore(parsedNode, this.firstChild); break; case "beforeEnd": this.appendChild(parsedNode); break; case "afterEnd": if (this.nextSibling) this.parentNode.insertBefore(parsedNode, this.nextSibling); else this.parentNode.appendChild(parsedNode); break; } }; HTMLElement.prototype.getBoundingClientRect = function () { var obj = this; var top = obj.offsetTop; var left = obj.offsetLeft; var right = obj.offsetWidth; var bottom = obj.offsetHeight; while (obj = obj.offsetParent) { if (obj.style.position == 'absolute' || obj.style.position == 'relative' || (obj.style.overflow != 'visible' && obj.style.overflow != '')) { break; } top += obj.offsetTop; left += obj.offsetLeft; } left -= document.body.scrollLeft; top -= document.body.scrollTop; right += left; bottom += top; return { 'left': left, 'top': top, 'right': right, 'bottom': bottom }; }; HTMLIFrameElement.prototype.__defineGetter__("Document", function () { return this.contentDocument }); } function $getClientWidthHeight(win) { var cw = win.document.body.clientWidth; var ch = win.document.body.clientHeight; return { 'width': cw, 'height': ch }; } function $getAbsM(topWin) { if (topWin == null) { topWin = top; } var leftM = 0; var topM = 0; var an = window; while (an != topWin) { var ifs = an.parent.document.getElementsByTagName('iframe'); for (var i = 0; i < ifs.length; i++) { try { if (ifs[i].Document == an.document) { var rc = ifs[i].getBoundingClientRect(); leftM += rc.left; topM += rc.top; break; } } catch (e) { continue; } } an = an.parent; } return { 'leftM': leftM, 'topM': topM }; } function disposeDatePicker() { if ($d != undefined && $d.obj != undefined && $d.obj.eCont != window.event.srcElement && $d.style.display != 'none') { var x = event.clientX; var y = event.clientY; var rc = $d.getBoundingClientRect(); if (x < rc.left || x > rc.right || y < rc.top || y > rc.bottom) { if ($d.obj.eCont.value == '' || $d.obj.l($d.obj.eCont.value)) { $d.obj._markValue(true); if ($d.obj.eCont.value != '') { $d.obj.bc($d.obj.eCont.value, $d.obj.dateFmt); $d.obj.am(); } else { $d.obj.eCont.setAttribute("REALVALUE", ""); } } else { $d.obj._markValue(false); } $d.yDiv.style.display = $d.mDiv.style.display = 'none'; $d.ifr.style.display = 'none'; $d.style.display = 'none'; } else { $d.yDiv.style.display = $d.mDiv.style.display = 'none'; } } } document.attachEvent('onmousedown', disposeDatePicker);