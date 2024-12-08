$(document).ready(function () {

    $("#id_mapping_bridge").change(function () {

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneMapping',
            data: { id_mapping_bridge: $(this).val() },
            success: function (data) {
                populateDropdown($("#id_mapping"), data);

            },
            async: true
        });

        verificaEstadoCombos();
    });

    seleccionaValoresDefault();

    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

});

//activa y desactiva los combos
function verificaEstadoCombos() {

    var planta = $("#id_mapping_bridge option:selected").val();


    if (planta == "") {
        $("#id_mapping").prop("disabled", true);
    }
    else {
        $("#id_mapping").prop("disabled", false);
    }

}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    var cMappingBridge = document.getElementById("cMappingBridge");
    var cMapping = document.getElementById("cMapping");

    //Si hay valores previos (EDIT)
    if (cMappingBridge != null && cMapping != null) {

        //estable el valor del primer combo
        if ($("#id_mapping_bridge option[value='" + cMappingBridge.value + "']").length > 0) {
            $("#id_mapping_bridge").val(cMappingBridge.value);
        } else {
            $("#id_mapping_bridge").val("");
        }

        //realiza una llamada asincorna del contenido del combo

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneMapping',
            data: { id_mapping_bridge: cMappingBridge.value},
            success: function (data) {
                populateDropdown($("#id_mapping"), data);
            },
            async: false
        });

        //establece el valor del segundo select una vez termina la llamada ajax
        if ($("#id_mapping option[value='" + cMapping.value + "']").length > 0) {
            $("#id_mapping").val(cMapping.value);
        } else {
            $("#id_mapping").val("");
        }

    } else { //en caso de que no haya valores previos (CREATE)
        verificaEstadoCombos();
    }
}

//completa el select con los datos recibidos
function populateDropdown(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        select.append($('<option></option>').val(option.value).html(option.name));
    });
}

window.onload = function () {
    clicMenu(2);
}