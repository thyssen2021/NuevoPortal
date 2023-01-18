$(document).ready(function () {


    //inicializa en input file
    bsCustomFileInput.init()

    //aumenta en uso cada vez que se hace un cambio en el archivo de soporte
    $("#ArchivoImagen").on("change", function () {
        document.getElementById('cambio_documento_soporte').value = ++documento_soporte_cambios;
    });

       //carga los valores cuando se cambia la planta
    $("#planta_clave").change(function () {
        //obtiene las areas
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAreas',
            data: { clavePlanta: $(this).val() },
            success: function (data) {
                populateDropdown($("#id_area"), data);
            },
            async: true
        });

        //obtiene los puestos
        $.ajax({
            type: 'POST',
            url: '/Combos/obtienePuestos',
            data: { claveArea: 0 }, //para que regrese una lista vacia
            success: function (data) {
                populateDropdown($("#puesto"), data);
            },
            async: true
        });

        verificaEstadoCombos();
    });

    //carga los valores cuando se cambia el área
    $("#id_area").change(function () {

        //obtiene los puestos
        $.ajax({
            type: 'POST',
            url: '/Combos/obtienePuestos',
            data: { claveArea: $(this).val() },
            success: function (data) {
                populateDropdown($("#puesto"), data);
            },
            async: true
        });

        verificaEstadoCombos();
    });

    seleccionaValoresDefault();

});


//variable para saber si ha cambiado el formulario y asi no se detenga por doble submit
var documento_soporte_cambios = 1;

//muestra el formulario de carga de archivo
function muestraFileInput() {
    $("#div_document_support").fadeOut(500, function () {
        $("#ArchivoImagen").val('');
    });
    $("#div_document_support_2").fadeIn(500);

    document.getElementById('cambio_documento_soporte').value = ++documento_soporte_cambios;
    document.getElementById('elimina_documento').value = 'true';
}

//oculta el formulario de carga de archivo
function ocultaFileInput() {
    $("#div_document_support").fadeIn(700, function () {
        $("#ArchivoImagen").val('');
    });
    $("#div_document_support_2").fadeOut(700);

    document.getElementById('cambio_documento_soporte').value = ++documento_soporte_cambios;
    document.getElementById('elimina_documento').value = 'false';
}


//completa el selec con los datos recibidos
function populateDropdown(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        select.append($('<option></option>').val(option.value).html(option.name));
    });
}

//activa y desactiva los combos
function verificaEstadoCombos() {

    var planta = $("#planta_clave option:selected").val();
    var area = $("#id_area option:selected").val();


    if (planta == "") {
        //si no hay planta desactiva area y puesto
        $("#id_area").prop("disabled", true);
        $("#puesto").prop("disabled", true);
    }
    else {
        //si hay planta activa área
        $("#id_area").prop("disabled", false);

        //verifica el estado del area
        if (area == "") {
            //si no hay área, desactiva puesto
            $("#puesto").prop("disabled", true);
        }
        else {
            //si hay área activa puesto
            $("#puesto").prop("disabled", false);
        }
    }

}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    var c_planta = document.getElementById("c_planta");
    var c_area = document.getElementById("c_area");
    var c_puesto = document.getElementById("c_puesto");

    //PRIMER COMBO
    if (c_planta != null) {

        //estable el valor del primer combo (no es neesario ajax)
        if ($("#planta_clave option[value='" + c_planta.value + "']").length > 0) {
            $("#planta_clave").val(c_planta.value);
        } else {
            $("#planta_clave").val("");
        }

        //realiza una llamada sincorna del contenido del segundo combo
        //obtiene las areas
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAreas',
            data: { clavePlanta: c_planta.value },
            success: function (data) {
                populateDropdown($("#id_area"), data);
            },
            async: false
        });

        //SEGUNDO COMBO
        //verifica si hay valores para el segundo combo
        if (c_area != null) {

            //estable el valor del segundo combo
            if ($("#id_area option[value='" + c_area.value + "']").length > 0) {
                $("#id_area").val(c_area.value);
            } else {
                $("#id_area").val("");
            }

            //realiza una llamada sincorna del contenido del tercer combo

            $.ajax({
                type: 'POST',
                url: '/Combos/obtienePuestos',
                data: { claveArea: c_area.value },
                success: function (data) {
                    populateDropdown($("#puesto"), data);
                },
                async: false
            });

            //TERCER COMBO
            //verifica si hay valores para el tercer combo
            if (c_puesto != null) {

                //estable el valor del segundo combo
                if ($("#puesto option[value='" + c_puesto.value + "']").length > 0) {
                    $("#puesto").val(c_puesto.value);
                } else {
                    $("#puesto").val("");
                }
            } else {
                verificaEstadoCombos();
            }
        } else {
            verificaEstadoCombos();
        }


    } else { //en caso de que no haya valores previos (CREATE)
        verificaEstadoCombos();
    }

    //debe ir despues de inicializar los combos
    // Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
}