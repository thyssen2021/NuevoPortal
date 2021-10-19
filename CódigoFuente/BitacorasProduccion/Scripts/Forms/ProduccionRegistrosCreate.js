$(document).ready(function () {


    //cuando hay cambio de planta
    $("#sap_platina").change(function () {

        //llamada ajax para obtener las lineas por planta
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneRollosBom',
            data: { material: $(this).val() },
            success: function (data) {
                populateDropdown($("#sap_rollo"), data);
                verificaEstadoCombos();
            },
            async: false
        });

    });

    seleccionaValoresDefault();


});

//activa y desactiva los combos
function verificaEstadoCombos() {

    var material = $("#sap_platina option:selected").val();

    //si material esta vacia
    if (material == "") {
        $("#sap_rollo").prop("disabled", true);
    }
    else {
        $("#sap_rollo").prop("disabled", false);
    }

}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {


    var c_platina = document.getElementById("cSapPlatina");

    //Si hay valores previos (EDIT)
    if (c_platina != null && c_platina.value != 0) {

        //realiza llamada para lineas
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneRollosBom',
            data: { material: c_platina.value },
            success: function (data) {
                populateDropdown($("#sap_rollo"), data);
            },
            async: false
        });

        var c_rollo = document.getElementById("cSapRollo");

        if (c_rollo != null && c_rollo.value != 0) {
            //establece el valor del segundo select una vez termina la llamada ajax
            if ($("#sap_rollo option[value='" + c_rollo.value + "']").length > 0) {
                $("#sap_rollo").val(c_rollo.value);
            } else {
                $("#sap_rollo").val("");
            }
        }

    } else { //en caso de que no haya valores previos (CREATE)
        verificaEstadoCombos();
    }

    // Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

}

//completa el selec con los datos recibidos
function populateDropdown(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        select.append($('<option></option>').val(option.value).html(option.name));
    });
}