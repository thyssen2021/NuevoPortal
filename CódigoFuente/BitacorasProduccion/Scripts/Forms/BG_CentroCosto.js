$(document).ready(function () {

    $("#plantaClave").change(function () {

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAreas',
            data: { clavePlanta: $(this).val() },
            success: function (data) {
                populateDropdown($("#id_area"), data);

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

    var planta = $("#plantaClave option:selected").val();


    if (planta == "") {
        $("#id_area").prop("disabled", true);
    }
    else {
        $("#id_area").prop("disabled", false);
    }
}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    var cPlanta = document.getElementById("cPlanta");
    var cArea = document.getElementById("cArea");

    //Si hay valores previos (EDIT)
    if (cPlanta != null && cArea != null) {

        //estable el valor del primer combo
        if ($("#plantaClave option[value='" + cPlanta.value + "']").length > 0) {
            $("#plantaClave").val(cPlanta.value);
        } else {
            $("#plantaClave").val("");
        }

        //realiza una llamada asincorna del contenido del combo

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAreas',
            data: { clavePlanta: cPlanta.value },
            success: function (data) {
                populateDropdown($("#id_area"), data);
            },
            async: false
        });

        //establece el valor del segundo select una vez termina la llamada ajax
        if ($("#id_area option[value='" + cArea.value + "']").length > 0) {
            $("#id_area").val(cArea.value);
        } else {
            $("#id_area").val("");
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