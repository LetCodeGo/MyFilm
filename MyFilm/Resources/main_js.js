var HtmlUtil = {
    /*1.��������ڲ�ת����ʵ��htmlת��*/
    htmlEncode: function (html) {
        //1.���ȶ�̬����һ��������ǩԪ�أ���DIV
        var temp = document.createElement("div");
        //2.Ȼ��Ҫת�����ַ�������Ϊ���Ԫ�ص�innerText(ie֧��)����textContent(�����google֧��)
        (temp.textContent != undefined) ? (temp.textContent = html) : (temp.innerText = html);
        //3.��󷵻����Ԫ�ص�innerHTML�����õ�����HTML����ת�����ַ�����
        var output = temp.innerHTML;
        temp = null;
        return output;
    },
    /*2.��������ڲ�ת����ʵ��html����*/
    htmlDecode: function (text) {
        //1.���ȶ�̬����һ��������ǩԪ�أ���DIV
        var temp = document.createElement("div");
        //2.Ȼ��Ҫת�����ַ�������Ϊ���Ԫ�ص�innerHTML(ie�������google��֧��)
        temp.innerHTML = text;
        //3.��󷵻����Ԫ�ص�innerText(ie֧��)����textContent(�����google֧��)�����õ�����HTML������ַ����ˡ�
        var output = temp.innerText || temp.textContent;
        temp = null;
        return output;
    },
    /*3.��������ʽʵ��htmlת��*/
    htmlEncodeByRegExp: function (str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&/g, "&amp;");
        s = s.replace(/</g, "&lt;");
        s = s.replace(/>/g, "&gt;");
        s = s.replace(/ /g, "&nbsp;");
        s = s.replace(/\'/g, "&#39;");
        s = s.replace(/\"/g, "&quot;");
        return s;
    },
    /*4.��������ʽʵ��html����*/
    htmlDecodeByRegExp: function (str) {
        var s = "";
        if (str.length == 0) return "";
        s = str.replace(/&amp;/g, "&");
        s = s.replace(/&lt;/g, "<");
        s = s.replace(/&gt;/g, ">");
        s = s.replace(/&nbsp;/g, " ");
        s = s.replace(/&#39;/g, "\'");
        s = s.replace(/&quot;/g, "\"");
        return s;
    }
};

function SearchContentCheck() {
    var content = document.getElementById("search").value;
    if (content == "" ||
        content == undefined ||
        content == null ||
        (content.length > 0 && content.trim().length == 0)) {
        alert("��������Ч��������");
        return false;
    }
    else return true;
}

// &ensp; \u2002
function SearchSubmit() {
    if (!SearchContentCheck()) return;

    var content = document.getElementById("search").value.trim();
    var x = document.getElementById("disk_select");
    var href = ("/?search=" + content);
    if (x.selectedIndex != 0)
        href += ("&diskdesc=" + x.options[x.selectedIndex].text.replace(/\u2002/g, " "));

    window.location.href = href;
}

function ToDeleteOrWatch(s) {
    var x = document.getElementById("disk_select");
    var href = s;
    if (x.selectedIndex != 0)
        href += ("?diskdesc=" + x.options[x.selectedIndex].text.replace(/\u2002/g, " "));
    window.location.href = href;
}