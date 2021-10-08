$(document).ready(function () {

    //agranda el tamaño de la barra
    window.onload = function () {
        document.getElementById('menu_toggle').click();
        document.getElementById('menu_toggle').click();
    }

    $("#clave_planta").change(function () {

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneEmpleados',
            data: { clavePlanta: $(this).val() },
            success: function (data) {
                populateDropdown($("#id_empleado"), data);
            },
            async: true
        });

        verificaEstadoCombos();
    });

    seleccionaValoresDefault();

});

//completa el selec con los datos recibidos
function populateDropdown(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        select.append($('<option></option>').val(option.value).html(option.name));
    });
}

//activa y desactiva los combos
function verificaEstadoCombos() {

    var planta = $("#clave_planta option:selected").val();


    if (planta == "") {
        $("#id_empleado").prop("disabled", true);
    }
    else {
        $("#id_empleado").prop("disabled", false);
    }

}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    var cPlanta = document.getElementById("cPlanta");
    var cEmpleado = document.getElementById("iEmpleado");

    //Si hay valores previos (EDIT)
    if (cPlanta != null && cPlanta.value != 0) {
        //estable el valor del primer combo
        if ($("#plantaClave option[value='" + cPlanta.value + "']").length > 0) {
            $("#plantaClave").val(cPlanta.value);
        } else {
            $("#plantaClave").val("");
        }

        //realiza una llamada asincorna del contenido del combo

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneEmpleados',
            data: { clavePlanta: cPlanta.value },
            success: function (data) {
                populateDropdown($("#id_empleado"), data);
            },
            async: false
        });

        if (cEmpleado != null && cEmpleado.value != 0) {
            //establece el valor del segundo select una vez termina la llamada ajax
            if ($("#id_empleado option[value='" + cEmpleado.value + "']").length > 0) {
                $("#id_empleado").val(cEmpleado.value);
            } else {
                $("#id_empleado").val("");
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