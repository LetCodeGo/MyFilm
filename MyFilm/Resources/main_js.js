function SearchContentCheck() {
    var content = document.getElementById("search").value;
    if (content == "" ||
        content == undefined ||
        content == null ||
        (content.length > 0 && content.trim().length == 0)) {
        alert("��������������");
        return false;
    }
    else return true;
}