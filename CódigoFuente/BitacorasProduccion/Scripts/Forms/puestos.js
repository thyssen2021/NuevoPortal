$(document).ready(function () {    

    $('input').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });

    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
    //evita que se abra el select
    $('select[data-select2-id]').on('select2:opening', function (e) {
        if ($(this).attr('readonly') == 'readonly') { // Check if select tag is readonly.
            console.log('readonly, cant be open!');
            e.preventDefault();
            $(this).select2('close');
            return false;
        } else {
            console.log('expandable/selectable');
        }
    });

    $('input').on('ifChecked', function (event) {
        verificaChecks();
    });

    $('input').on('ifUnchecked', function (event) {
        verificaChecks();
    });

    $("#areaClave").change(function () {
        let area = $(this).val();
        actualizaPuestos(area);
    });

    $("#plantaClave").change(function () {

        actualizaAreas($(this).val());
    });

    actualizaPuestos($("#areaClave option:selected").val());

    //verificaChecks();
    //$("#areaClave").change();
    //seleccionaValoresDefault();
});

function actualizaAreas(id) {
    $.ajax({
        type: 'POST',
        url: '/Puestos/obtieneAreas',
        data: { clavePlanta: id },
        success: function (data) {
            populateDropdown($("#areaClave"), data);

            actualizaPuestos(0);
        },
        async: true
    });

    verificaEstadoCombos();

}

function verificaChecks(event) {

    var checkedValue = $('#shared_services:checked').val();

    if (checkedValue) {
        let element = document.getElementById('plantaClave');
        element.value = 1;
        $('#plantaClave').attr('readonly', 'readonly');

        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })

        actualizaAreas(99); //valor para shared sevices
    } else {
        $('#plantaClave').removeAttr('readonly');
        actualizaAreas($("#plantaClave option:selected").val()); //por defecto el de puebla
    }

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

    var planta = $("#plantaClave option:selected").val();


    if (planta == "") {
        $("#areaClave").prop("disabled", true);
    }
    else {
        $("#areaClave").prop("disabled", false);
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
            url: '/Puestos/obtieneAreas',
            data: { clavePlanta: cPlanta.value },
            success: function (data) {
                populateDropdown($("#areaClave"), data);
            },
            async: false
        });

        //establece el valor del segundo select una vez termina la llamada ajax
        if ($("#areaClave option[value='" + cArea.value + "']").length > 0) {
           
            $("#areaClave").val(cArea.value);
        } else {
            $("#areaClave").val("");
        }

    } else { //en caso de que no haya valores previos (CREATE)
        verificaEstadoCombos();
    }  

    // Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

}

function actualizaPuestos(id) {
    let departamento = $("#areaClave option:selected").text();
    if (id == 0)
        $('#title-departamento').html('');
    else
        $('#title-puesto').html('Departamento ' + departamento);

    $.ajax({
        type: 'POST',
        url: '/Combos/obtienePuestos',
        data: { claveArea: id },
        success: function (data) {
            populateList($("#puestosActuales"), data);
        },
        async: true
    });

}

//completa el selec con los datos recibidos
function populateList(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        if (id != 0)
            select.append($('<li></li>').html(option.name));
    });
}

//agranda el tamaño de la barra
window.onload = function () {
    document.getElementById('menu_toggle').click();
}