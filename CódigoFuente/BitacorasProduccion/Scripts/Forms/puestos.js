$(document).ready(function () {    

    $("#plantaClave").change(function () {

        $.ajax({
            type: 'POST',
            url: '/Puestos/obtieneAreas',
            data: { clavePlanta: $(this).val() },
            success: function (data) {
                populateDropdown($("#areaClave"), data);
                setTimeout(function () {
                    console.log("I am the third log after 5 seconds");
                }, 5000);
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