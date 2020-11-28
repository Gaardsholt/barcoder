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

$("#barcodeForm select").change(onInputChange);
$("#barcodeForm input").on('input', onInputChange);


function onInputChange(event) {
    updateBarcodeData();
    updateImage();
}

function updateBarcodeData() {
    barcode = {
        text: $("#barcodeText").val(),
        type: parseInt($("#barcodeType option:selected").attr("value")),
        width: parseInt($("#barcodeWidth").val()),
        height: parseInt($("#barcodeHeight").val()),
        rotate: parseInt($("#barcodeRotate").val())
    };
}

function updateImage() {
    let barcodeImg = document.getElementById("barcodeImg");
    var imgUrl = "image.png?" + toQueryString(barcode);
    
    $.ajax({
        url: imgUrl,
        context: document.body
    }).done(function () {
        $("#error_message").hide();
        
        barcodeImg.src = imgUrl;
        $(barcodeImg).show();
    }).fail(function (xhr, ajaxOptions, thrownError) {
        $("#error_message").text(xhr.responseText).show();
        $(barcodeImg).hide();
    });
}


function toQueryString(obj) {
    var str = [];
    for (var p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }
    return str.join("&");
}