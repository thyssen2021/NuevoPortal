$(document).ready(function () {

    //Agrega detector de eventos
    $("input[name=tipoUsuario]").change(function () {
        activaCampos();
    });

    // Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

    //agrega evento OnChange
    $("#IdEmpleado").change(function () {

        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneEmail',
            data: { numEmpleado: $(this).val() },
            success: function (data) {
                $("#Email").val(data[0].email);
                $('#Email').prop('readonly', true);
            },
            async: false
        });
    });


    valorInicial();

    desactivaCampos();

});

function valorInicial() {
    //selecciona valor inicial
    var tipoU = document.getElementById("tipoU");
    var e_numEmpleado = document.getElementById("e_numEmpleado");
    var e_nombre = document.getElementById("e_nombre");


    if (tipoU != null) {


        switch (tipoU.value) {
            case 'empleado':
                //estable el valor del primer combo
                $("#radioEmpleado").prop("checked", true);


                //si existe el numero de empleado
                if (e_numEmpleado != null && $("#IdEmpleado option[value='" + e_numEmpleado.value + "']").length > 0) {
                    console.log(e_numEmpleado.value)
                    $("#IdEmpleado").val(e_numEmpleado.value);

                    $("#Nombre").prop('disabled', true);
                    $("#divNombre").hide();
                    $("#divEmpleado").show();
                    $("#IdEmpleado").prop('required', true);
                    $("#Nombre").prop('required', false);
                    $("#Nombre").val('');
                    verificaEmail();

                } else {
                    $("#IdEmpleado").val("");
                }
                break;
            //en caso de que se haya seleccionado otro
            case 'otro':

                $("#radioOtro").prop("checked", true);


                $("#IdEmpleado").val("");

                $("#Nombre").prop('disabled', false);
                $("#divNombre").show();
                $("#divEmpleado").hide();
                $("#Nombre").prop('required', true);
                $("#IdEmpleado").prop('required', false);
                $("#IdEmpleado").val(0);

                //activa campo email                            
                $('#Email').prop('readonly', false);


                // activaCampos();
                break;
        }


    } else {
        activaCampos();
    }
}

function desactivaCampos() {
    //verifica si es edit
    var edit = document.getElementById("e_edit");

    if (edit != null) {
        $('#Email').prop('readonly', true);
        $('#IdEmpleado').prop('readonly', true);

    }
}

function activaCampos() {
    var seleccion = $('input:radio[name=tipoUsuario]:checked').val();

    if (seleccion == 'empleado') {
        //desactiva y oculta el input nombre
        $("#Nombre").prop('disabled', true);
        $("#divNombre").hide();
        $("#divEmpleado").show();
        $("#IdEmpleado").prop('required', true);
        $("#Nombre").prop('required', false);
        $("#Nombre").val('');
        $("#IdEmpleado").val("");
        //carga email
        //llamada a verifica email
        verificaEmail();

    } //otro
    else {
        $("#IdEmpleado").val("");

        $("#Nombre").prop('disabled', false);
        $("#divNombre").show();
        $("#divEmpleado").hide();
        $("#Nombre").prop('required', true);
        $("#IdEmpleado").prop('required', false);
        $("#IdEmpleado").val(0);

        //activa campo email
        $("#Email").val("");
        $('#Email').prop('readonly', false);
    }

}

function verificaEmail() {

    $("#IdEmpleado").change();
}