//制保留2位小数，如：2，会在2后面补上00.即2.00
function toDecimal2(x) {
    var f = parseFloat(x);
    if (isNaN(f)) {
        return '?';
    }
    var f = Math.round(x * 100) / 100;
    var s = f.toString();
    var rs = s.indexOf('.');
    if (rs < 0) {
        rs = s.length;
        s += '.';
    }
    while (s.length <= rs + 2) {
        s += '0';
    }
    return s;
}

function arr_count(o) {
    var t = typeof o;
    if (t == 'string') {
        return o.length;
    } else if (t == 'object') {
        var n = 0;
        for (var i in o) { n++; }
        return n;
    }
    return false;
}
//获取daySpan天之前的日期
function GetDate(daySpan) {
    var today = new Date();
    var targetday_milliseconds = today.getTime() - 1000 * 60 * 60 * 24 * daySpan;
    var targetday = new Date();
    targetday.setTime(targetday_milliseconds);

    var strYear = targetday.getFullYear();
    var strDay = targetday.getDate();
    var strMonth = targetday.getMonth() + 1;
    if (strMonth < 10) {
        strMonth = "0" + strMonth;
    }
    var strTargetday = strYear + "/" + strMonth + "/" + strDay;
    return strTargetday;
}

//获取daySpan天之前的日期
function GetDateByMonth(monthSpan) {
    var today = new Date();
    var strYear = today.getFullYear();
    var strMonth = today.getMonth() + 1;
    var yearOffset = parseInt(monthSpan / 12);
    var monthOffset = monthSpan % 12;
    strMonth = strMonth - monthOffset;
    while(strMonth<=0){
        strMonth = strMonth + 12;
        yearOffset = yearOffset + 1;
    }

    if (strMonth < 10) {
        strMonth = "0" + strMonth;
    }
    var strTargetday = (strYear - yearOffset) + "/" + strMonth + "/01";
    return strTargetday;
}
