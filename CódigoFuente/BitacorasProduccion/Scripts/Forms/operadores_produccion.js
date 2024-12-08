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
                verificaEstadoCombos();
            },
            async: true
        });

        //llamada ajax para obtener las lineas por planta
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneLineasPlantas',
            data: { clavePlanta: $(this).val() },
            success: function (data) {
                populateDropdown($("#id_linea"), data);
                verificaEstadoCombos();
            },
            async: true
        });

    });


    $("#id_linea").change(function () {
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

    //si planta esta vacia
    if (planta == "") {
        $("#id_linea").prop("disabled", true);
        $("#id_empleado").prop("disabled", true);
    }
    else {
        $("#id_linea").prop("disabled", false);
        $("#id_empleado").prop("disabled", false);
    }

    var linea = $("#id_linea option:selected").val();
    //si linea está vacía
    if (linea == "") {
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
    var cLinea = document.getElementById("iLinea");

    //Si hay valores previos (EDIT)
    if (cPlanta != null && cPlanta.value != 0) {
        //estable el valor del primer combo
        if ($("#clave_planta option[value='" + cPlanta.value + "']").length > 0) {
            $("#clave_planta").val(cPlanta.value);
        } else {
            $("#clave_planta").val("");
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

        //realiza llamada para lineas
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneLineasPlantas',
            data: { clavePlanta: cPlanta.value },
            success: function (data) {
                populateDropdown($("#id_linea"), data);
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

        if (cLinea != null && cLinea.value != 0) {
            //establece el valor del segundo select una vez termina la llamada ajax
            if ($("#id_linea option[value='" + cLinea.value + "']").length > 0) {
                $("#id_linea").val(cLinea.value);
            } else {
                $("#id_linea").val("");
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