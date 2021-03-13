var barcode = {};

updateRange("barcodeSizeRange", "barcodeSize");

function updateRange(range, box) {
    var slider = document.getElementById(range);
    var output = document.getElementById(box);
    output.value = slider.value;

    slider.oninput = function () {
        output.value = this.value;
    };
    output.oninput = function () {
        slider.value = this.value;
    };
}

document.querySelectorAll('#barcodeForm select').forEach(item => {
    item.addEventListener('change', onInputChange)
});

document.querySelectorAll('#barcodeForm input').forEach(item => {
    item.addEventListener('input', onInputChange)
});



function onInputChange(event) {
    let shouldUpdateImage = updateBarcodeData();

    if (shouldUpdateImage) {
        document.getElementById("barcodeImg").show();

        updateImage();
    } else {
        document.getElementById("barcodeImg").hide();
    }
        
}

function updateBarcodeData() {
    var nr = document.getElementById("qrNr").value;
    var amount = document.getElementById("qrAmount").value;

    if (nr == "")
        return false

    var text = "https://products.mobilepay.dk/box/box?phone=" + nr + "&amount=" + amount;
    barcode = {
        text: text,
        type: parseInt(document.getElementById("barcodeType").selectedOptions[0].value),
        width: parseInt(document.getElementById("barcodeSize").value),
        height: parseInt(document.getElementById("barcodeSize").value),
        rotate: 0
    };
    return true;
}

function updateImage() {
    let barcodeImg = document.getElementById("barcodeImg");
    var imgUrl = "image.png?" + toQueryString(barcode);

    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                showBarcode(barcodeImg, imgUrl)
            } else {
                showError(barcodeImg)
            }
        }

    };

    xhttp.open("GET", imgUrl, true);
    xhttp.send();
}

function showBarcode(el, imgUrl) {
    document.getElementById("error_message").hide();

    el.src = imgUrl;
    el.show();
}
function showError(el) {
    document.getElementById("error_message").innerText = this.responseText;
    document.getElementById("error_message").show();
    el.hide();
}
