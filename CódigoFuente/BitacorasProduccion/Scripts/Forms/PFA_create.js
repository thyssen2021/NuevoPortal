$(document).ready(function () {

    //Agrega el evento a todos los input
    $("#total_cost").each(function () {
        $(this).on('input', function (e) {
            calculaCostToRecover()
        });
    });
    $("#total_pf_cost").each(function () {
        $(this).on('input', function (e) {
            calculaCostToRecover()
        });
    });



    //agrega el evento para mostrar o ocultar estado de Aceptacion
    $("#id_PFA_responsible_cost").change(function () {
        let seleccion = $(this).val();
      
        if (seleccion != "" && seleccion != "1") {
            $("#div_separador_soporte").fadeIn(700);
            $("#div_is_accepted").fadeIn(700);           
        } else {
            $("#cost_is_accepted").val('');
            
            $('#cost_is_accepted').select2({
                theme: 'bootstrap4'
            })

            $("#div_separador_soporte").fadeOut(700);
            $("#div_is_accepted").fadeOut(700);

            //$("#div_email_support").fadeOut(700, function () {
            //    $("#email_support").val('');
            //});
            $("#div_document_support").fadeOut(700, function () {
                $("#PostedFile").val('');
            });
        }

    });


    //agrega el evento para mostrar o ocultar el soporte en caso de true
    $("#cost_is_accepted").change(function () {
        let seleccion = $(this).val();

        if (seleccion == "true") {
           /* $("#div_email_support").fadeIn(700);*/
            $("#div_document_support").fadeIn(700);
        } else {
            //$("#div_email_support").fadeOut(700, function () {
            //    $("#email_support").val('');
            //});
            $("#div_document_support").fadeOut(700, function () {
                $("#PostedFile").val('');
            });
        }

    });


    calculaCostToRecover();

    //selecciona autorizador por defecto
    seleccionaAutorizador();

    

    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
});

//agranda el tamaño de la barra
window.onload = function () {
    document.getElementById('menu_toggle').click();
}

//Selecciona autorizador por defecto
function seleccionaAutorizador() {
    var idAutorizador = document.getElementById("idAutorizador");

    if (idAutorizador == null) { //no hay autorizador

        //recorre el select
        $("#id_PFA_autorizador option").each(function () {
            let nombre = $(this).text();

            if (nombre.toUpperCase().includes('JESUS OLVERA')) {

                $("#id_PFA_autorizador").val($(this).val());
            }
        });

    }
}


//calcula cost to recover
function calculaCostToRecover(str, defaultValue) {
    let total_cost = TryParseFloat($('#total_cost').val(), 0);
    let total_pf_cost = TryParseFloat($('#total_pf_cost').val(), 0);
    $('#total_coast_to_recover').val((total_pf_cost - total_cost).toFixed(2));
}

//convierte texto a float
function TryParseFloat(str, defaultValue) {
    var retValue = defaultValue;
    if (str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseFloat(str);
            }
        }
    }
    return retValue;
}