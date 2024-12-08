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


    //maneja el envió el del formulario
    $('#btn-pass').click(function (e) {
        
        var pass = generatePasswordRand();

        $('#Password').val(pass);
        $('#ConfirmPassword').val(pass);
        $('#passwordText').html(pass);

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

        var seleccion = $('input:radio[name=tipoUsuario]:checked').val();

        if (seleccion == 'empleado') {
            $('#Email').prop('readonly', true);
        }
        
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

//genera un password aleatoreo
function generatePasswordRand() {

    var length = 10;

    var mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var minusculas = "abcdefghijklmnopqrstuvwxyz";
    var numeros = "0123456789";
    var simbolos = "!#$%&=*";
    var characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&=*"; //1

    var pass = "";
    let tieneNumero = false;
    let tieneMayuscula = false;
    let tieneMinuscula = false;
    let tieneSimbolo = false;

    do {

        pass = "";
        tieneNumero = false;
        tieneMayuscula = false;
        tieneMinuscula = false;
        tieneSimbolo = false;

        for (i = 0; i < length; i++) {
            pass += characters.charAt(Math.floor(Math.random() * characters.length));
        }
        
        for (i = 0; i < numeros.length; i++) {
            if (pass.includes(numeros.charAt(i))) {
                tieneNumero = true;
                break;
            }
        }
        
        for (i = 0; i < mayusculas.length; i++) {
            if (pass.includes(mayusculas.charAt(i))) {
                tieneMayuscula = true;
                break;
            }
        }
        
        for (i = 0; i < minusculas.length; i++) {
            if (pass.includes(minusculas.charAt(i))) {
                tieneMinuscula = true;
                break;
            }
        }
        
        for (i = 0; i < simbolos.length; i++) {
            if (pass.includes(simbolos.charAt(i))) {
                tieneSimbolo = true;
                break;
            }
        }
    } while (!tieneNumero || !tieneMayuscula || !tieneMinuscula || !tieneSimbolo);

    return pass;
}