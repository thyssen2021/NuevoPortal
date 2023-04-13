
$(document).ready(function () {

    //inicializa los data table
    $('.table').DataTable({
        "paging": false,
        "ordering": false,
        "searching": false,
        "scrollX": true,
        "info": false
    });


    //Agrega el evento a todos los input
    $(":input").each(function () {
        $(this).on('input', function (e) {
            calculaDatos()
        });
    });

    //agrega el evento para comprobar si es un numero
    $('#peso_manual').on('input', function (e) {

        let peso_manual = $('#peso_manual').val();
        //verifica si es un numero
        if (isNaN(peso_manual)) {
            $('#error_peso').show();
            $('#aceptar_modal_doble').prop("disabled", true)
            $('#aceptar_modal_sencilla').prop("disabled", true)
        } else {
            $('#error_peso').hide();
            $('#aceptar_modal_doble').prop("disabled", false)
            $('#aceptar_modal_sencilla').prop("disabled", false)
        }
    });

    //agrega el evento para comprobar si es un numero valido el tiempo permitido
    $('#tiempo').on('input', function (e) {

        let tiempo = $('#tiempo').val();

        //verifica si es un numero
        if (isNaN(tiempo)) {
            $('#error_tiempo').show();
            $('#aceptar_pass').prop("disabled", true)
        } else if (tiempo >= 1 && tiempo <= 300) {
            $('#error_tiempo').hide();
            $('#aceptar_pass').prop("disabled", false)
        } else {
            $('#error_tiempo').show();
            $('#aceptar_pass').prop("disabled", true)
        }
    });


    //carga input mask
    $('.entero').inputmask({ 'alias': 'integer', 'autoGroup': true, 'autoUnmask': true, 'removeMaskOnSubmit': true });

    //calcula los datos la primera vez que carga la página
    calculaDatos();

    //llamada al método para asignar el número de lote
    AsignaNumeroLote();

});

//agranda el tamaño de la barra
window.onload = function () {
    document.getElementById('menu_toggle').click();
}

//variable global para el id
var num = 0;
//variable global para saber si se presionó el segundo valor
var peso_bascula_2 = false;

var Toast = Swal.mixin({
    toast: true,
    icon: 'success',
    title: 'General Title',
    animation: false,
    position: 'top-right',
    showConfirmButton: true,
    timer: 5000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
});

//muestra el modal de captura de peso
function mostrarModalPass(platina) {

    if (platina == 2)
        peso_bascula_2 = true;
    else
        peso_bascula_2 = false;


    let autorizado = VerificaTiempo();
    if (autorizado) {
        mostrarModal();
    }
    else {
        $('#modalPass').modal('show');
        $('#password').val('');
    }

}


//muestra el modal de captura de peso
function mostrarModal() {

    try {

        document.getElementById("peso_manual").focus();

        $('#myModal').modal('show')
        $('#peso_manual').val('');
        let peso_manual = $('#peso_manual').val();
        //verifica si es un numero
        if (isNaN(peso_manual)) {
            $('#error_peso').show();
            $('#aceptar_modal_doble').prop("disabled", true)
            $('#aceptar_modal_sencilla').prop("disabled", true)
        } else {
            $('#error_peso').hide();
            $('#aceptar_modal_doble').prop("disabled", false)
            $('#aceptar_modal_sencilla').prop("disabled", false)
        }
    } catch (error) {
        //en caso de que no ser muestre info de ningun tipo muestra error
        Swal.fire({
            icon: 'error',
            title: 'Ocurrió un error',
            text: 'Intente nuevamente o capture el peso de forma manual.'
        })
    }



}

//muestra el modal de captura de peso
function muestraModalSocket(platina) {

    if (platina == 2)
        peso_bascula_2 = true;
    else
        peso_bascula_2 = false;


    $('#myModalTipoPieza').modal('show')
}

//verifica la contraseña ingresada
function verificarContraseña(idSupervisor) {
    let passSupervisor = $('#password').val();
    let tiempo = $('#tiempo').val();
    try {
        //verifica el si la contraseña es correcta
        $.ajax({
            type: 'POST',
            url: '/ProduccionRegistros/VerificaPassword',
            data: { idSupervisor: idSupervisor, tiempo: tiempo, password: passSupervisor },
            success: function (data) {
                console.log(data);
                if (data[0].Status == "OK") {
                    $('#modalPass').modal('hide');
                    mostrarModal();
                } else {
                    $('#modalPass').modal('hide')
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Contraseña incorrecta.'
                    })
                }
            },
            async: true
        });

        let pass = $('#password').val('');

    } catch (error) {
        console.error(error);
        $('#modalPass').modal('hide')
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Ha ocurrido un error al obtener la contraseña.'
        })
    }



}

//verifica si está dentro del tiempo autorizado
function VerificaTiempo() {


    let valor = false;

    try {
        //verifica el si la contraseña es correcta
        $.ajax({
            type: 'POST',
            url: '/ProduccionRegistros/VerificaTiempoPermitido',
            data: {},
            success: function (data) {
                console.log(data);
                if (data[0].Status == "OK") {
                    valor = true;
                } else {
                    valor = false;
                }
            },
            async: false
        });

    } catch (error) {
        console.error('Hubo un error al verificar el tiempo autorizado');
        return false;
    }

    return valor;

}

//muestra el modal de captura de peso
function ocultarModal(denominador) {
    let peso_etiqueta = $('#peso_manual').val();
    $('#myModal').modal('hide');

    //verifica si es un numero
    if (!isNaN(peso_etiqueta)) {
        let peso = TryParseFloat(peso_etiqueta, 0);

        let resultado = peso / denominador;
        resultado = resultado.toFixed(3);
        if (peso_bascula_2)
            $('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(resultado);
        else
            $('#produccion_datos_entrada_peso_real_pieza_neto').val(resultado);

        //en caso de que sea doble se aplica a ambos pesos de platina
        if (denominador == 2) {
            $('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(resultado);
            $('#produccion_datos_entrada_peso_real_pieza_neto').val(resultado);
        }

        Toast.fire({
            icon: 'success',
            title: 'Se capturó el peso correctamente.'
        })
        calculaDatos();
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Ocurrió un error',
            text: 'Intente nuevamente o capture el peso de forma manual.'
        })
    }
}


//realizar llamada ajax a la báscula
function ObtenerPesoBascula(ip, denominador) {

    //obtiene las el peso
    $.ajax({
        type: 'POST',
        url: '/ProduccionRegistros/obtienePesoBascula',
        data: { ip: ip },
        success: function (data) {
            try {
                console.log(data);
                if (data[0].Message == "OK") {
                    let peso = TryParseFloat(data[0].Peso, 0);
                    let resultado = peso / denominador;
                    resultado = resultado.toFixed(3);
                    if (peso_bascula_2)
                        $('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(resultado);
                    else
                        $('#produccion_datos_entrada_peso_real_pieza_neto').val(resultado);

                    //en caso de que sea doble se aplica a ambos pesos de platina
                    if (denominador == 2) {
                        $('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(resultado);
                        $('#produccion_datos_entrada_peso_real_pieza_neto').val(resultado);
                    }


                    Toast.fire({
                        icon: 'success',
                        title: 'Se capturó el peso correctamente.'
                    })
                    calculaDatos();
                } else { //en caso de que el mensaje no sea exitoso muestra mensaje
                    Swal.fire({
                        icon: 'error',
                        title: 'Ocurrió un error',
                        text: 'Intente nuevamente o capture el peso de forma manual.'
                    })
                }
            } catch (error) {
                //en caso de que no ser muestre info de ningun tipo muestra error
                Swal.fire({
                    icon: 'error',
                    title: 'Ocurrió un error',
                    text: 'Intente nuevamente o capture el peso de forma manual.'
                })
            }
        },
        error: function (textStatus, errorThrown) {
            //en caso de error en la llamada ajax
            Swal.fire({
                icon: 'error',
                title: 'Ocurrió un error',
                text: 'Intente nuevamente o capture el peso de forma manual.'
            })
        },
        async: true
    });
    $('#myModalTipoPieza').modal('hide')

}



//funcion que calcula los datos de lo campos readonly
function calculaDatos() {

    //obtine el valor de sap platina
    let sap_platina_2 = $("#sap_platina_2").val();
    let tiene_platina2 = sap_platina_2 !== "";

    //limita a tres decimales el peso real pieza bruto de la báscula
    let pb2 = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(),0);
    $('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(pb2.toFixed(3));
    let pb1 = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto').val(),0);
    $('#produccion_datos_entrada_peso_real_pieza_neto').val(pb1.toFixed(3));

    //Variables
    let peso_etiqueta = TryParseFloat($('#produccion_datos_entrada_peso_etiqueta').val(), 0);
    let peso_despunte = TryParseFloat($('#produccion_datos_entrada_peso_despunte_kgs').val(), 0);
    let peso_cola = TryParseFloat($('#produccion_datos_entrada_peso_cola_kgs').val(), 0);
    let peso_real_pieza_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto').val(), 0);  //valor de la báscula
    if (tiene_platina2) {
        var peso_real_pieza_neto_2 = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(), 0);  //valor de la báscula
    }
    let peso_regreso_rollo = TryParseFloat($('#produccion_datos_entrada_peso_regreso_rollo_real').val(), 0);
    let ordenes_por_pieza = TryParseFloat($('#produccion_datos_entrada_ordenes_por_pieza').val(), 0);
    let peso_bascula = TryParseFloat($('#produccion_datos_entrada_peso_bascula_kgs').val(), 0);

    //piezas de ajuste
    let piezas_ajuste_1 = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste').val(), 0);
    let piezas_ajuste_2 = 0;
    if (tiene_platina2) {
        piezas_ajuste_2 = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste_platina_2').val(), 0);
    }

    let total_piezas_ajuste = piezas_ajuste_1 + piezas_ajuste_2;  //suma ambas cantidades de piezas de ajuste
    //asigna el valor de las piezas de ajuste
    $('#suma_piezas_de_ajuste').val(total_piezas_ajuste.toFixed(0));

    //variable para calcular la suma del peso real 
    //por defecto incluye el valor correspondiente al de sap platina 1
    let suma_peso_real_pieza_bruto = obtienePesoRealPiezaBruto();

    //si tiene platina 2 agrega el valor a la sumatoria
    if (tiene_platina2) {
        suma_peso_real_pieza_bruto += obtienePesoRealPiezaBruto2();
    }

    //porcentaje utilizado del rollo en proporcion al peso real pieza bruto
    let porcentaje_platina_1 = (obtienePesoRealPiezaBruto() / suma_peso_real_pieza_bruto);
    let porcentaje_platina_2 = 0;

    if (tiene_platina2) {
        porcentaje_platina_2 = (obtienePesoRealPiezaBruto2() / suma_peso_real_pieza_bruto);
    }


    let piezas_por_golpe = TryParseFloat($('#produccion_datos_entrada_piezas_por_golpe').val(), 0);

    //variable calculadas
    let peso_rollo_usado = peso_etiqueta - peso_regreso_rollo;
    let peso_rollo_usado_real_1 = (peso_bascula - peso_regreso_rollo) * porcentaje_platina_1;
    let peso_rollo_usado_real_2 = (peso_bascula - peso_regreso_rollo) * porcentaje_platina_2;
    let total_piezas = obtieneTotalPiezaLote();
    let total_piezas_platina_1 = obtieneTotalPiezaLotePlatina1();
    if (tiene_platina2) {
        var total_piezas_platina_2 = obtieneTotalPiezaLotePlatina2();
    }
    let numero_golpes = DivisionPorCero(total_piezas, piezas_por_golpe, 0)
    let peso_real_pieza_bruto = obtienePesoRealPiezaBruto();
    let peso_bruto_kgs = obtienePesoBrutoKgs();
    if (tiene_platina2) {
        var peso_bruto_kgs_2 = obtienePesoBrutoKgs2();
    }   

    let peso_bruto_total_piezas = (total_piezas_platina_1 * peso_real_pieza_bruto * ordenes_por_pieza);

    if (tiene_platina2) {
        var peso_bruto_total_piezas_2 = (total_piezas_platina_2 * obtienePesoRealPiezaBruto2() * ordenes_por_pieza);
    }
    let peso_neto_total_piezas = (total_piezas_platina_1 * peso_real_pieza_neto * ordenes_por_pieza);

    if (tiene_platina2) {
        var peso_neto_total_piezas_2 = (total_piezas_platina_2 * peso_real_pieza_neto_2 * ordenes_por_pieza);
    }
    let scrap_natural = obtieneScrapNatural();

    //si no tiene sap platina toma el total de piezas de ajuste
    let scrap_ingenieria_buenas_ajuste = 0;
    if (tiene_platina2) {
        var scrap_ingenieria_buenas_ajuste_2 = ((peso_bruto_total_piezas_2 - peso_neto_total_piezas_2) + (piezas_ajuste_2 * obtieneScrapNatural_2()));
        scrap_ingenieria_buenas_ajuste = ((peso_bruto_total_piezas - peso_neto_total_piezas) + (piezas_ajuste_1 * scrap_natural));
    } else {
        scrap_ingenieria_buenas_ajuste = ((peso_bruto_total_piezas - peso_neto_total_piezas) + (total_piezas_ajuste * scrap_natural));
    }

    //si no tiene sap platina sap platina 2, toma en cuenta el total de las piezas    
    let peso_neto_total_piezas_ajuste_kgs = 0;
    if (tiene_platina2) {
        var peso_neto_total_piezas_ajuste_kgs_2 = piezas_ajuste_2 * peso_real_pieza_neto_2;
        peso_neto_total_piezas_ajuste_kgs = piezas_ajuste_1 * peso_real_pieza_neto;
    } else {
        peso_neto_total_piezas_ajuste_kgs = total_piezas_ajuste * peso_real_pieza_neto;
    }


    let peso_punta_y_cola_reales = peso_rollo_usado_real_1 - (peso_bruto_total_piezas + peso_bruto_kgs);
    if (peso_punta_y_cola_reales < 0)
        peso_punta_y_cola_reales = 0;

    if (tiene_platina2) {
        var peso_punta_y_cola_reales_2 = peso_rollo_usado_real_2 - (peso_bruto_total_piezas_2 + peso_bruto_kgs_2);
        if (peso_punta_y_cola_reales_2 < 0)
            peso_punta_y_cola_reales_2 = 0;
    }

    let balance_scrap_real = (DivisionPorCero(peso_bruto_kgs + peso_punta_y_cola_reales, peso_rollo_usado_real_1, 0)) * 100;

    if (tiene_platina2) {
        var balance_scrap_real_2 = (DivisionPorCero(peso_bruto_kgs_2 + peso_punta_y_cola_reales_2, peso_rollo_usado_real_2, 0)) * 100;
    }
    //asigna el peso de rollo usado
    $('#peso_rollo_usado').val(peso_rollo_usado);
    //calcula y asiga punta y colas
    if (peso_bascula == 0) {
        $('#porcentaje_punta_cola').val('--'); //en caso de que denominador sea cero
    } else {
        let porcentaje_punta_colas = ((peso_despunte + peso_cola) / peso_bascula) * 100;
        $('#porcentaje_punta_cola').val(porcentaje_punta_colas.toFixed(2) + " %");
    }
    //asiga total pieza lotes
    $('#total_piezas_platina_1').val(total_piezas_platina_1);

    if (tiene_platina2)
        $('#total_piezas_platina_2').val(total_piezas_platina_2);

    $('#total_piezas').val(total_piezas);


    //agrega el valor de piezas de ajusta a cada tabla

    if (tiene_platina2) {
        document.getElementById("td_piezas_de_ajuste").innerHTML = piezas_ajuste_1.toFixed(0);
        document.getElementById("td_piezas_de_ajuste_2").innerHTML = piezas_ajuste_2.toFixed(0);
    } else { //considera el total de piezas de ajuste
        document.getElementById("td_piezas_de_ajuste").innerHTML = total_piezas_ajuste.toFixed(0);
    }


    //agrega el valor de peso real pieza neto
    document.getElementById("td_peso_real_pieza_neto").innerHTML = peso_real_pieza_neto.toFixed(3);
    if (tiene_platina2)
        document.getElementById("td_peso_real_pieza_neto_2").innerHTML = peso_real_pieza_neto_2.toFixed(3);


    //agrega el valor de peso bruto kgs
    document.getElementById("td_peso_bruto_kgs").innerHTML = peso_bruto_kgs.toFixed(3);
    if (tiene_platina2)
        document.getElementById("td_peso_bruto_kgs_2").innerHTML = peso_bruto_kgs_2.toFixed(3);

    //asigna el peso real pieza bruto PLATINA 1
    let prpb = obtienePesoRealPiezaBruto().toFixed(3);
    document.getElementById("td_peso_real_pieza_bruto").innerHTML = prpb;
    $('#peso_real_pieza_bruto_input').val(prpb);

    if (tiene_platina2) {
        //asigna el peso real pieza bruto PLATINA 2
        var prpb2 = obtienePesoRealPiezaBruto2().toFixed(3);
        document.getElementById("td_peso_real_pieza_bruto_2").innerHTML = prpb2;
        $('#peso_real_pieza_bruto_input_2').val(prpb2);
    }

    //crear y llamar método para obtener el scrap natural td_scrap_natural
    document.getElementById("td_scrap_natural").innerHTML = obtieneScrapNatural().toFixed(3);
    //crear y llamar método para obtener el scrap natural td_scrap_natural platina 2
    if (tiene_platina2)
        document.getElementById("td_scrap_natural_2").innerHTML = obtieneScrapNatural_2().toFixed(3);

    //asiga el valor del peso bruto
    // $('#peso_bruto_kgs').val(obtienePesoBrutoKgs().toFixed(3));

    //asiga el numero de golpes (VERIFICAR LA FORMULA)
    $('#numero_golpes').val(numero_golpes.toFixed(0));

    //completa el balance de scrap
    document.getElementById("td_balance_scrap").innerHTML = obtieneBalanceScrap(porcentaje_platina_1).toFixed(2) + '%';
    //completa el balance de scrap platina2
    if (tiene_platina2)
        document.getElementById("td_balance_scrap_2").innerHTML = obtieneBalanceScrap2(porcentaje_platina_2).toFixed(2) + '%';

    //cambia el color del fondo de la celda
    cambiaColorCelda('fila_balance_scrap', obtieneBalanceScrap(porcentaje_platina_1));
    //cambia el color del fondo de la celda Platina 2
    if (tiene_platina2)
        cambiaColorCelda('fila_balance_scrap_2', obtieneBalanceScrap2(porcentaje_platina_2));

    //asigna el valor de las ordenes por pieza
    document.getElementById("td_ordenes_pieza").innerHTML = ordenes_por_pieza;
    //asigna el valor de las ordenes por pieza platina 2
    if (tiene_platina2)
        document.getElementById("td_ordenes_pieza_2").innerHTML = ordenes_por_pieza;

    //asiga el valor de peso rollo real usado
    document.getElementById("td_peso_rollo_usado_real").innerHTML = peso_rollo_usado_real_1.toFixed(3);
    //asigna el valor de peso bruto total pieza kg
    document.getElementById("td_peso_bruto_total_piezas").innerHTML = peso_bruto_total_piezas.toFixed(3);
    //asigna el valor de peso neto total pieza kg
    document.getElementById("td_peso_neto_total_piezas").innerHTML = peso_neto_total_piezas.toFixed(3);
    //asigna scrap ingenieria buenas + ajuste
    document.getElementById("td_scrap_ingenieria").innerHTML = scrap_ingenieria_buenas_ajuste.toFixed(3);
    //asigna el valor peso neto  total piezas de ajuste
    document.getElementById("td_peso_neto_total_piezas_ajuste").innerHTML = peso_neto_total_piezas_ajuste_kgs.toFixed(3);
    //asiga el valor de peso punta y cola reales
    document.getElementById("td_peso_punta_colas_reales").innerHTML = peso_punta_y_cola_reales.toFixed(3);
    //asiga el balance de scrap real
    document.getElementById("td_balance_scrap_reales").innerHTML = balance_scrap_real.toFixed(2) + ' %';
    cambiaColorCelda("fila_balance_scrap_real", balance_scrap_real);

    if (tiene_platina2) {
        //asiga el valor de peso rollo real usado
        document.getElementById("td_peso_rollo_usado_real_2").innerHTML = peso_rollo_usado_real_2.toFixed(3);
        //asigna el valor de peso bruto total pieza kg
        document.getElementById("td_peso_bruto_total_piezas_2").innerHTML = peso_bruto_total_piezas_2.toFixed(3);
        //asigna el valor de peso neto total pieza kg
        document.getElementById("td_peso_neto_total_piezas_2").innerHTML = peso_neto_total_piezas_2.toFixed(3);
        //asigna scrap ingenieria buenas + ajuste
        document.getElementById("td_scrap_ingenieria_2").innerHTML = scrap_ingenieria_buenas_ajuste_2.toFixed(3);
        //asigna el valor peso neto  total piezas de ajuste
        document.getElementById("td_peso_neto_total_piezas_ajuste_2").innerHTML = peso_neto_total_piezas_ajuste_kgs_2.toFixed(3);
        //asiga el valor de peso punta y cola reales
        document.getElementById("td_peso_punta_colas_reales_2").innerHTML = peso_punta_y_cola_reales_2.toFixed(3);
        //asiga el balance de scrap real
        document.getElementById("td_balance_scrap_reales_2").innerHTML = balance_scrap_real_2.toFixed(2) + ' %';
        cambiaColorCelda("fila_balance_scrap_real_2", balance_scrap_real_2);
    }
}



//---- el valor de Peso real pieza bruto ---
function obtienePesoRealPiezaBruto() {
    let peso_neto_sap = TryParseFloat(document.getElementById("td_peso_neto_sap").innerHTML, 0);
    let peso_bruto_sap = TryParseFloat(document.getElementById("td_peso_bruto_sap").innerHTML, 0);
    let peso_real_pieza_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto').val(), 0);
    return peso_real_pieza_bruto = DivisionPorCero(peso_bruto_sap, peso_neto_sap, 0) * peso_real_pieza_neto;

}

//---- el valor de Peso real pieza bruto ---
function obtienePesoRealPiezaBruto2() {
    let peso_neto_sap = TryParseFloat(document.getElementById("td_peso_neto_sap_2").innerHTML, 0);
    let peso_bruto_sap = TryParseFloat(document.getElementById("td_peso_bruto_sap_2").innerHTML, 0);
    let peso_real_pieza_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(), 0);

    return peso_real_pieza_bruto = DivisionPorCero(peso_bruto_sap, peso_neto_sap, 0) * peso_real_pieza_neto;

}

//----division por cero----
function DivisionPorCero(numerador, denominador, defecto = 0) {
    let result = defecto;

    if (denominador != 0) {
        result = numerador / denominador;

    }
    return result;
}
//----cambia el color de la celda----
function cambiaColorCelda(celda, valor) {
    var celda = $('#' + celda);
    valor = Math.abs(valor);


    celda.removeClass("fondo_verde");
    celda.removeClass("fondo_amarillo");
    celda.removeClass("fondo_rojo");

    //agrega fondo segun el valor
    if (valor >= 0 && valor < 1) {
        celda.addClass("fondo_verde");
    } else if (valor >= 1 && valor < 3) {
        celda.addClass("fondo_amarillo");
    } else if (valor >= 3) {
        celda.addClass("fondo_rojo");
    }

}
//---- el valor de Scrap Natural ---
function obtieneScrapNatural() {
    let peso_bruto = obtienePesoRealPiezaBruto();
    let peso_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto').val(), 0);

    return (peso_bruto - peso_neto);

}
//---- el valor de Scrap Natural ---
function obtieneScrapNatural_2() {
    let peso_bruto = obtienePesoRealPiezaBruto2();
    let peso_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto_platina_2').val(), 0);

    return (peso_bruto - peso_neto);

}

//---- el valor de Peso Bruto Kgs  --- Las piezas de ajuste se dividen entre dos momentaneamente
function obtienePesoBrutoKgs() {
    let peso_bruto = obtienePesoRealPiezaBruto();

    let sap_platina_2 = $("#sap_platina_2").val();
    let tiene_platina2 = sap_platina_2 !== "";

    //piezas de ajuste se dividen entre 2
    let piezas_ajuste_1 = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste').val(), 0);
    let piezas_ajuste_2 = 0;
    if (tiene_platina2) {
        piezas_ajuste_2 = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste_platina_2').val(), 0);
    }

    let total_piezas_ajuste = piezas_ajuste_1 + piezas_ajuste_2;  //suma ambas cantidades de piezas de ajuste

    if (tiene_platina2) {
        return (peso_bruto * piezas_ajuste_1);
    } else {
        return (peso_bruto * total_piezas_ajuste);
    }

}
//---- el valor de Peso Bruto Kgs  --- Las piezas de ajuste se dividen entre dos momentaneamente
function obtienePesoBrutoKgs2() {
    let peso_bruto = obtienePesoRealPiezaBruto2();
    let piezas_ajuste_2 = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste_platina_2').val(), 0);

    let sap_platina_2 = $("#sap_platina_2").val();
    let tiene_platina2 = sap_platina_2 !== "";

    return (peso_bruto * piezas_ajuste_2);
}

//---- obtiene el balance de scrap  ---
function obtieneBalanceScrap(porcentaje) {

    let peso_despunte = TryParseFloat($('#produccion_datos_entrada_peso_despunte_kgs').val(), 0);
    let peso_cola = TryParseFloat($('#produccion_datos_entrada_peso_cola_kgs').val(), 0);
    let peso_bruto = obtienePesoBrutoKgs();
    let peso_etiqueta = TryParseFloat($('#produccion_datos_entrada_peso_etiqueta').val(), 0);

    return balance_scrap = DivisionPorCero((((peso_despunte + peso_cola) * porcentaje) + peso_bruto), peso_etiqueta * porcentaje, 0) * 100;

}
//---- obtiene el balance de scrap  ---
function obtieneBalanceScrap2(porcentaje) {
    let peso_despunte = TryParseFloat($('#produccion_datos_entrada_peso_despunte_kgs').val(), 0);
    let peso_cola = TryParseFloat($('#produccion_datos_entrada_peso_cola_kgs').val(), 0);
    let peso_bruto = obtienePesoBrutoKgs2();
    let peso_etiqueta = TryParseFloat($('#produccion_datos_entrada_peso_etiqueta').val(), 0);

    return balance_scrap = DivisionPorCero((((peso_despunte + peso_cola) * porcentaje) + peso_bruto), peso_etiqueta * porcentaje, 0) * 100;

}

function obtieneTotalPiezaLotePlatina1() {
    var suma = 0;

    //obtine los valores de lotes
    let sap_platina_1 = $("#sap_platina").val();
    //obtine el valor de sap platina
    let tiene_platina2 = $("#sap_platina_2").val() !== "";

    $('.total_piezas').each(function () {
        let row = $(this).attr("data-row");

        let select = $('select[name="produccion_lotes[' + row + '].sap_platina"] option:selected').text();

        if (sap_platina_1 !== "" && sap_platina_1 == select || !tiene_platina2) {
            suma = suma + TryParseFloat($(this).val(), 0)
        }
    });

    return suma;
}

function obtieneTotalPiezaLotePlatina2() {
    var suma = 0;

    //obtine los valores de lotes
    let sap_platina_2 = $("#sap_platina_2").val();

    $('.total_piezas').each(function () {
        let row = $(this).attr("data-row");

        let select = $('select[name="produccion_lotes[' + row + '].sap_platina"] option:selected').text();

        if (sap_platina_2 !== "" && sap_platina_2 == select) {
            suma = suma + TryParseFloat($(this).val(), 0)
        }
    });

    return suma;
}


function obtieneTotalPiezaLote() {
    var suma = 0;

    $('.total_piezas').each(function () {
        suma = suma + TryParseFloat($(this).val(), 0)
    });

    return suma;
}


//convierte texto a float
function TryParseFloat(str, defaultValue) {

    var retValue = defaultValue;
    if (str !== undefined && str !== null) {
        if (str.length > 0) {
            if (!isNaN(str)) {
                retValue = parseFloat(str);
            }
        }
    }
    return retValue;
}

//agreaga una fila para lotes
function AgregarConcepto() {
    //obtine los valores de lotes
    let sap_platina_1 = $("#sap_platina").val();
    let sap_platina_2 = $("#sap_platina_2").val();

    let cadena = `
                  <div class="form-group row" id="div_lotes_`+ num + `">
                        <div class="col-md-1">
                            <input style="text-align:center; background-color:antiquewhite" type="text" class="form-control col-md-12 input-contador-lotes" value="" readonly="readonly">
                        </div>
                    
                        <input type="hidden" name="produccion_lotes.Index" id="produccion_lotes.Index" value="`+ num + `" />
                        <label class="control-label col-md-1" for="produccion_lotes[`+ num + `].sap_platina">
                            <span class="float-right">Platina</span>
                        </label>
                        <div class="col-md-2">
                            <select name="produccion_lotes[`+ num + `].sap_platina" name="produccion_lotes[` + num + `].sap_platina"  class="combo-material form-control select2bs4" data-row="` + num + `" id="combo_material_` + num + `" style="width: 100%" required>
                               <option value="" selected>-- Sin Definir --</option>
                              <option value="`+ sap_platina_1 + `" >` + sap_platina_1 + `</option>
                        `;

    if (sap_platina_2 !== "") {
        cadena += `<option value="` + sap_platina_2 + `">` + sap_platina_2 + `</option>`;
    }

    cadena += `
                              
                            </select>
                            <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[`+ num + `].sap_platina" data-valmsg-replace="true"></span>
                        </div>                      
                        <label class="control-label col-md-1" for="produccion_lotes[`+ num + `].numero_lote_izquierdo">
                            <span class="float-right">Lote Izquierdo</span>
                        </label>
                        <div class="col-md-1">
                            <input style="text-align:right" type="text" min="0" step="1" max="50000" name="produccion_lotes[`+ num + `].numero_lote_izquierdo" id="produccion_lotes[` + num + `].lote_izquierdo" class="form-control col-md-12 entero" value="" autocomplete="off">
                            <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[` + num + `].numero_lote_izquierdo" data-valmsg-replace="true"></span>
                        </div>
                        <label class="control-label col-md-1" for="produccion_lotes[`+ num + `].numero_lote_derecho">
                            <span class="float-right">Lote Derecho</span>
                        </label>
                        <div class="col-md-1">
                            <input type="text" min="0" step="1" max="50000" name="produccion_lotes[`+ num + `].numero_lote_derecho" id="produccion_lotes[` + num + `].lote_derecho" class="form-control col-md-12 entero" value="" autocomplete="off">
                            <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[` + num + `].numero_lote_derecho" data-valmsg-replace="true"></span>
                        </div>
                        <label class="control-label col-md-1" for="produccion_lotes[`+ num + `].piezas_paquete">
                                <span class="float-right">Piezas por paquete</span>
                        </label>
                        <div class="col-md-1">
                            <input type="text" min="0" step="1" max="5000" name="produccion_lotes[`+ num + `].piezas_paquete" id="produccion_lotes[` + num + `].piezas_paquete" class="form-control col-md-12 total_piezas entero" data-row="` + num + `" value="" maxlength="10" autocomplete="off" required>
                                <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[` + num + `].piezas_paquete" data-valmsg-replace="true"></span>
                            </div>
                        <div class="col-md-1">
                            <input type="button" value="Borrar" class="btn btn-danger" onclick="borrarLote(` + num + `); return false;">
                        </div>
        </div>
        `;

    $("#div_lotes").append(cadena);

    $("#div_lotes_" + num).hide().fadeIn(500);


    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })

    //aplica el evento a los combos de material
    $("#combo_material_" + num).change(function () {
        calculaDatos();
    });

    //carga input mask
    $('.entero').inputmask({ 'alias': 'integer', 'autoGroup': true, 'autoUnmask': true, 'removeMaskOnSubmit': true });

    //vuelve a asigar el evento on a cada input de los lotes
    $('.total_piezas').each(function () {
        $(this).on('input', function (e) {
            calculaDatos();
        });
    });

    AsignaNumeroLote();

    num++;
}





//borra un lote
function borrarLote(id) {

    $("#div_lotes_" + id).fadeOut(0, function () {
        $(this).remove();
    });
    calculaDatos();
    AsignaNumeroLote();
}

//numera los lotes
function AsignaNumeroLote() {
    let indice = 1;

    $('.input-contador-lotes').each(function (index) {
        $(this).val(indice);
        indice++;
    });
}