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
    let barcodeType = parseInt(document.getElementById("barcodeType").selectedOptions[0].value);
    let barcodeText = document.getElementById("barcodeText").value;
    let barcodeWidth = parseInt(document.getElementById("barcodeWidth").value);
    let barcodeHeight = parseInt(document.getElementById("barcodeHeight").value);
    let barcodeRotate = parseInt(document.getElementById("barcodeRotate").value);

    if (barcodeType == "99999") {
        barcode = {
            url: "hex.png?",
            data: {
                color: barcodeText,
                width: barcodeWidth,
                height: barcodeHeight,
            }
        };
    } else {
        barcode = {
            url: "image.png?",
            data: {
                text: barcodeText,
                type: barcodeType,
                width: barcodeWidth,
                height: barcodeHeight,
                rotate: barcodeRotate
            }
        };
    }

}

function updateImage() {
    let barcodeImg = document.getElementById("barcodeImg");
    var imgUrl = barcode.url + toQueryString(barcode.data);

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
