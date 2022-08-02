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

    //inicializa icheck
    $('input').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });

    //captura evento de icheck
    $('#segunda_platina').on('ifToggled', function (event) {
        verificaSegundaPlatina();
    });

   
    verificaSegundaPlatina();

   
    //// Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })


});

//activa y desactiva segunda platina
function verificaSegundaPlatina() {

    if ($('#segunda_platina').prop('checked')) {
        $('#sap_platina_2').prop('disabled', false);
    } else {
        $("#sap_platina_2").val('');
        $('#sap_platina_2').prop('disabled', 'disabled');
        // Initialize Select2 Elements
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    }

}

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


//completa el selec con los datos recibidos
function populateDropdown(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        select.append($('<option></option>').val(option.value).html(option.name));
    });
}