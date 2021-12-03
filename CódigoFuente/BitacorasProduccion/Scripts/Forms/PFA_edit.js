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

            $("#div_document_support_2").fadeOut(700, function () {
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

            $("#div_document_support_2").fadeOut(700, function () {
                $("#PostedFile").val('');
            });
        }

    });

    calculaCostToRecover();
    seleccionaValoresDefault();


    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
});

//agranda el tamaño de la barra
window.onload = function () {
    document.getElementById('menu_toggle').click();
}

//muestra el formulario de carga de archivo
function muestraFileInput() {
    $("#div_document_support").fadeOut(700, function () {
        $("#PostedFile").val('');
    });
    $("#div_document_support_2").fadeIn(700);
}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    let cDepartment = document.getElementById("cDepartment");
    let cVolume = document.getElementById("cVolume");
    let cBorder = document.getElementById("cBorder");
    let cDestinationPlant = document.getElementById("cDestinationPlant");
    let cReason = document.getElementById("cReason");
    let cTypeShipment = document.getElementById("cTypeShipment");
    let cResponsible = document.getElementById("cResponsible");
    let cRecovered = document.getElementById("cRecovered");
    let cAuthorizer = document.getElementById("cAuthorizer");

    //Si hay valores previos (EDIT)
    if (cDepartment != null && cDepartment.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_Department option[value='" + cDepartment.value + "']").length > 0) {
            $("#id_PFA_Department").val(cDepartment.value);
        } else {
            $("#id_PFA_Department").val("");
        }
    }

    if (cVolume != null && cVolume.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_volume option[value='" + cVolume.value + "']").length > 0) {
            $("#id_PFA_volume").val(cVolume.value);
        } else {
            $("#id_PFA_volume").val("");
        }
    }

    if (cBorder != null && cBorder.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_border_port option[value='" + cBorder.value + "']").length > 0) {
            $("#id_PFA_border_port").val(cBorder.value);
        } else {
            $("#id_PFA_border_port").val("");
        }
    }

    if (cDestinationPlant != null && cDestinationPlant.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_destination_plant option[value='" + cDestinationPlant.value + "']").length > 0) {
            $("#id_PFA_destination_plant").val(cDestinationPlant.value);
        } else {
            $("#id_PFA_destination_plant").val("");
        }
    }

    if (cReason != null && cReason.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_reason option[value='" + cReason.value + "']").length > 0) {
            $("#id_PFA_reason").val(cReason.value);
        } else {
            $("#id_PFA_reason").val("");
        }
    }

    if (cTypeShipment != null && cTypeShipment.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_type_shipment option[value='" + cTypeShipment.value + "']").length > 0) {
            $("#id_PFA_type_shipment").val(cTypeShipment.value);
        } else {
            $("#id_PFA_type_shipment").val("");
        }
    }

    if (cResponsible != null && cResponsible.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_responsible_cost option[value='" + cResponsible.value + "']").length > 0) {
            $("#id_PFA_responsible_cost").val(cResponsible.value);
        } else {
            $("#id_PFA_responsible_cost").val("");
        }
    }

    if (cRecovered != null && cRecovered.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_recovered_cost option[value='" + cRecovered.value + "']").length > 0) {
            $("#id_PFA_recovered_cost").val(cRecovered.value);
        } else {
            $("#id_PFA_recovered_cost").val("");
        }
    }

    if (cAuthorizer != null && cAuthorizer.value != 0) {
        //estable el valor del primer combo
        if ($("#id_PFA_autorizador option[value='" + cAuthorizer.value + "']").length > 0) {
            $("#id_PFA_autorizador").val(cAuthorizer.value);
        } else {
            $("#id_PFA_autorizador").val("");
        }
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