barcode = {};

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

$("#barcodeForm input[type=text]").keyup(onInputChange);
$("#barcodeForm input").change(onInputChange);
$("#barcodeForm select").change(onInputChange);

function onInputChange(event) {
    console.log(event.target.id + " input was changed!");

    validateForm();
}

function validateForm() {
    barcode = {
        text: $("#barcodeText").val(),
        type: parseInt($("#barcodeType option:selected").attr("value")),
        width: parseInt($("#barcodeWidth").val()),
        height: parseInt($("#barcodeHeight").val()),
        rotate: parseInt($("#barcodeRotate").val())
    };

    var formIsValid = true;
    for (var key in barcode) {
        if (barcode.hasOwnProperty(key)) {
            if (typeof (barcode[key]) === "number" && isNaN(barcode[key]) || barcode[key] === "") {
                formIsValid = false;
            }
        }
    }

    $("#barcodeSubmit").prop('disabled', !formIsValid);
}


$("#barcodeSubmit").click(function () {
    var imgUrl = "image.png?" + toQueryString(barcode);

    barcodeImg = document.getElementById("barcodeImg");
    barcodeImg.src = imgUrl;

    $(barcodeImg).show();
});


function toQueryString(obj) {
    var str = [];
    for (var p in obj)
        if (obj.hasOwnProperty(p)) {
            str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
        }
    return str.join("&");
}