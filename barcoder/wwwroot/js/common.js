HTMLElement.prototype.show = function () {
    this.style.display = "inline-block";
}
HTMLElement.prototype.hide = function () {
    this.style.display = "none";
}


function toQueryString(obj) {
    var str = [];
    for (var p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }
    return str.join("&");
}

