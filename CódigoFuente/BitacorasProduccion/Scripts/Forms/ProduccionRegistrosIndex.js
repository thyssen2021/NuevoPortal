$(document).ready(function () {
    //agranda el tamaño de la barra
    window.onload = function () {
        document.getElementById('menu_toggle').click();
    }
    //inicializa los data table
    $('#datatable_1').DataTable({
        "paging": false,
        "ordering": false,
        "searching": false,
        "scrollX": true,
        "info": false
    });

    //cuando hay cambio de planta
    $("#planta").change(function () {

        ////llamada ajax para obtener las lineas por planta
        //$.ajax({
        //    type: 'POST',
        //    url: '/Combos/obtieneLineasPlantas',
        //    data: { clavePlanta: $(this).val() },
        //    success: function (data) {
        //        populateDropdown($("#linea"), data);
                verificaEstadoCombos();
        //    },
        //    async: false
        //});

        ////oculta la tabla y la paginación
        //$('#body_tabla').hide();
        //$('#nav_paginacion').hide();
    });

    $("#linea").change(function () {
        verificaEstadoCombos();
    });
    $("#posteado").change(function () {
        verificaEstadoCombos();
    });

   seleccionaValoresDefault();
    //verificaEstadoCombos();

    // Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })


});

//activa y desactiva los combos
function verificaEstadoCombos() {

    $("#buscarForm").submit();

         $.blockUI({
            css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: .3,
                color: '#fff'
            },
            message: '<h3>Cargando...</h3>'

        });

    //var planta = $("#planta option:selected").val();

    ////si planta esta vacia
    //if (planta == "") {
    //    $("#linea").prop("disabled", true);
    //  //  $("#crearFormButton").prop("disabled", true);
    //}
    //else {
    //    $("#linea").prop("disabled", false);
    //}

    //var linea = $("#linea option:selected").val();

    ////si linea esta vacia
    //if (linea == "") {
    //   // $("#crearFormButton").prop("disabled", true);
    //    $("#crearFormButton").fadeOut(1000);
    //    $('#body_tabla').hide();
    //    $('#nav_paginacion').hide();
    //}
    //else {
    //    //$("#crearFormButton").prop("disabled", false);           
    //    $("#buscarForm").submit();
    //    $.blockUI({
    //        css: {
    //            border: 'none',
    //            padding: '15px',
    //            backgroundColor: '#000',
    //            '-webkit-border-radius': '10px',
    //            '-moz-border-radius': '10px',
    //            opacity: .3,
    //            color: '#fff'
    //        },
    //        message: '<h3>Cargando...</h3>'

    //    });
    //}
}


//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    var cPlanta = document.getElementById("cPlanta");
    var cLinea = document.getElementById("iLinea");

    //Si hay valores previos (EDIT)
    if (cPlanta != null && cPlanta.value != 0) {
        //estable el valor del primer combo
        if ($("#planta option[value='" + cPlanta.value + "']").length > 0) {
            $("#planta").val(cPlanta.value);
        } else {
            $("#planta").val("");
        }

        ////realiza llamada para lineas
        //$.ajax({
        //    type: 'POST',
        //    url: '/Combos/obtieneLineasPlantas',
        //    data: { clavePlanta: cPlanta.value },
        //    success: function (data) {
        //        populateDropdown($("#linea"), data);
        //    },
        //    async: false
        //});


        if (cLinea != null && cLinea.value != 0) {
            //establece el valor del segundo select una vez termina la llamada ajax
            if ($("#linea option[value='" + cLinea.value + "']").length > 0) {
                $("#linea").val(cLinea.value);
               // $("#crearFormButton").prop("disabled", false);
                $("#crearFormButton").fadeIn(1000);
            } else {
                $("#linea").val("");
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

//borra los datos del formulario para restablecerlo
var form = document.getElementById("buscarForm");
//borra los campos antes de enviarlos
$("#borrarForm").click(function () {
    $('#planta').val("");
    $('#linea').val("");
    form.submit();
});

//valida los datos de búsqueda para poder insertar un nuevo registro
var formV = document.getElementById("createForm");
$("#crearFormButton").click(function () {

    var planta = $("#planta option:selected").val();
    var linea = $("#linea option:selected").val();

    if (planta != "" && linea != "") {
        //copia los valores seleccionados a los input ocultos del formulario crear
        $('#hidden_linea').val(linea);
        $('#hidden_planta').val(planta);
        formV.submit();
    } else {
        alert("complete los valores del formulario");
    }


});