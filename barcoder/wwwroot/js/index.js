var barcode = {};

updateRange("barcodeWidthRange", "barcodeWidth");
updateRange("barcodeHeightRange", "barcodeHeight");
updateRange("barcodeRotateRange", "barcodeRotate");

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
    updateBarcodeData();
    updateImage();
}

function updateBarcodeData() {
    barcode = {
        text: document.getElementById("barcodeText").value,
        type: parseInt(document.getElementById("barcodeType").selectedOptions[0].value),
        width: parseInt(document.getElementById("barcodeWidth").value),
        height: parseInt(document.getElementById("barcodeHeight").value),
        rotate: parseInt(document.getElementById("barcodeRotate").value)
    };
}

function updateImage() {
    let barcodeImg = document.getElementById("barcodeImg");
    var imgUrl = "image.png?" + toQueryString(barcode);

    window.xhttp = new XMLHttpRequest();
    window.xhttp.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (this.status == 200) {
                document.getElementById("error_message").hide();

                barcodeImg.src = imgUrl;
                barcodeImg.show();
            } else {
                document.getElementById("error_message").innerText = this.responseText;
                document.getElementById("error_message").show();
                barcodeImg.hide();

            }
        }

    };
    window.xhttp.onerror = function () {

    };

    window.xhttp.open("GET", imgUrl, true);
    window.xhttp.send();
}


function toQueryString(obj) {
    var str = [];
    for (var p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }
    return str.join("&");
}

