function receiptPrint(that) {
    that.addClass("printable");
    window.print();
}

function popitup(url) {
    newwindow = window.open(url, 'name', 'height=800,width=600');
    if (window.focus) { newwindow.focus()}
    return false;
}

