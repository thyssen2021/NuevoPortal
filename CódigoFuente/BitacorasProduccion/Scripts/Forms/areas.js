$(document).ready(function () {

    //selecciona el valor enviado
    var pClave = document.getElementById("pClave");
    //Si hay valores previos (EDIT)
    if (pClave != null) {
        //estable el valor del primer combo
        if ($("#plantaClave option[value='" + pClave.value + "']").length > 0) {
            $("#plantaClave").val(pClave.value);
        } else {
            $("#plantaClave").val("");
        }
    }

    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

});