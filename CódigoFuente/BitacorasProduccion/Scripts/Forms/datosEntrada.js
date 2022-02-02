
$(document).ready(function () {

    //inicializa los data table
    $('#datatable_1').DataTable({
        "paging": false,
        "ordering": false,
        "searching": false,
        "scrollX": true,
        "info": false
    });

    //inicializa los data table
    $('#datatable_2').DataTable({
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
function mostrarModalPass() {
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
}

//muestra el modal de captura de peso
function muestraModalSocket() {
    $('#myModalTipoPieza').modal('show')
}

//verifica la contraseña ingresada
function verificarContraseña(idSupervisor) {
    let passSupervisor = $('#password').val();
    let tiempo = $('#tiempo').val();
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
}

//verifica si está dentro del tiempo autorizado
function VerificaTiempo() {

    let valor = false;
    //verifica el si la contraseña es correcta
    $.ajax({
        type: 'POST',
        url: '/ProduccionRegistros/VerificaTiempoPermitido',
        data: {  },
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

    return valor;

}

//muestra el modal de captura de peso
function ocultarModal(denominador) {
    let peso_etiqueta = $('#peso_manual').val();
    $('#myModal').modal('hide');

    //verifica si es un numero
    if (!isNaN(peso_etiqueta)) {
        let peso = TryParseFloat(peso_etiqueta, 0);
        $('#produccion_datos_entrada_peso_real_pieza_neto').val(peso_etiqueta / denominador);
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
                    $('#produccion_datos_entrada_peso_real_pieza_neto').val(peso / denominador);
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

    //Variables
    let peso_etiqueta = TryParseFloat($('#produccion_datos_entrada_peso_etiqueta').val(), 0);
    let peso_despunte = TryParseFloat($('#produccion_datos_entrada_peso_despunte_kgs').val(), 0);
    let peso_cola = TryParseFloat($('#produccion_datos_entrada_peso_cola_kgs').val(), 0);
    let peso_real_pieza_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto').val(), 0);  //valor de la báscula
    let peso_regreso_rollo = TryParseFloat($('#produccion_datos_entrada_peso_regreso_rollo_real').val(), 0);
    let ordenes_por_pieza = TryParseFloat($('#produccion_datos_entrada_ordenes_por_pieza').val(), 0);
    let peso_bascula = TryParseFloat($('#produccion_datos_entrada_peso_bascula_kgs').val(), 0);
    let total_piezas_ajuste = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste').val(), 0);
    let piezas_por_golpe = TryParseFloat($('#produccion_datos_entrada_piezas_por_golpe').val(), 0);

    //variable calculadas
    let peso_rollo_usado = peso_etiqueta - peso_regreso_rollo;
    let peso_rollo_usado_real = peso_bascula - peso_regreso_rollo;
    let total_piezas = obtieneTotalPiezaLote();
    let numero_golpes = DivisionPorCero(total_piezas, piezas_por_golpe, 0)
    let peso_real_pieza_bruto = obtienePesoRealPiezaBruto();
    let peso_bruto_kgs = obtienePesoBrutoKgs();
    let peso_bruto_total_piezas = (total_piezas * peso_real_pieza_bruto * ordenes_por_pieza);
    let peso_neto_total_piezas = (total_piezas * peso_real_pieza_neto * ordenes_por_pieza);
    let scrap_natural = obtieneScrapNatural();
    let scrap_ingenieria_buenas_ajuste = ((peso_bruto_total_piezas - peso_neto_total_piezas) + (total_piezas_ajuste * scrap_natural));
    let peso_neto_total_piezas_ajuste_kgs = total_piezas_ajuste * peso_real_pieza_neto;
    let peso_punta_y_cola_reales = peso_rollo_usado_real - (peso_bruto_total_piezas + peso_bruto_kgs);
    if (peso_punta_y_cola_reales < 0)
        peso_punta_y_cola_reales = 0;

    let balance_scrap_real = (DivisionPorCero(peso_bruto_kgs + peso_punta_y_cola_reales, peso_rollo_usado_real, 0)) * 100;

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
    $('#total_piezas').val(total_piezas);
    //agrega el valor de la bácula a la tabla
    document.getElementById("td_peso_real_pieza_neto").innerHTML = peso_real_pieza_neto.toFixed(3);
    //asigna el peso real pieza bruto
    let prpb = obtienePesoRealPiezaBruto().toFixed(3);
    document.getElementById("td_peso_real_pieza_bruto").innerHTML = prpb;
    $('#peso_real_pieza_bruto_input').val(prpb);
    //crear y llamar método para obtener el scrap natural td_scrap_natural
    document.getElementById("td_scrap_natural").innerHTML = obtieneScrapNatural().toFixed(3);
    //asiga el valor del peso bruto
    $('#peso_bruto_kgs').val(obtienePesoBrutoKgs().toFixed(3));
    //asiga el numero de golpes (VERIFICAR LA FORMULA)
    $('#numero_golpes').val(numero_golpes.toFixed(0));

    //completa el balance de scrap
    document.getElementById("td_balance_scrap").innerHTML = obtieneBalanceScrap().toFixed(2) + '%';
    //cambia el color del fondo de la celda
    cambiaColorCelda('fila_balance_scrap', obtieneBalanceScrap());

    //asigna el valor de las ordenes por pieza
    document.getElementById("td_ordenes_pieza").innerHTML = ordenes_por_pieza;
    //asiga el valor de peso rollo real usado
    document.getElementById("td_peso_rollo_usado_real").innerHTML = peso_rollo_usado_real.toFixed(3);
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
}



//---- el valor de Peso real pieza bruto ---
function obtienePesoRealPiezaBruto() {
    var peso_neto_sap = TryParseFloat(document.getElementById("td_peso_neto_sap").innerHTML, 0);
    var peso_bruto_sap = TryParseFloat(document.getElementById("td_peso_bruto_sap").innerHTML, 0);
    var peso_real_pieza_neto = TryParseFloat($('#produccion_datos_entrada_peso_real_pieza_neto').val(), 0);


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

//---- el valor de Peso Bruto Kgs  ---
function obtienePesoBrutoKgs() {
    let peso_bruto = obtienePesoRealPiezaBruto();
    let total_piezas = TryParseFloat($('#produccion_datos_entrada_total_piezas_ajuste').val(), 0);

    return (peso_bruto * total_piezas);

}

//---- obtiene el balance de scrap  ---
function obtieneBalanceScrap() {
    let peso_despunte = TryParseFloat($('#produccion_datos_entrada_peso_despunte_kgs').val(), 0);
    let peso_cola = TryParseFloat($('#produccion_datos_entrada_peso_cola_kgs').val(), 0);
    let peso_bruto = obtienePesoBrutoKgs();
    let peso_etiqueta = TryParseFloat($('#produccion_datos_entrada_peso_etiqueta').val(), 0);

    return balance_scrap = DivisionPorCero((peso_despunte + peso_cola + peso_bruto), peso_etiqueta, 0) * 100;

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
    if (str !== null) {
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


    $("#div_lotes").append(

        `
                                                        <div class="form-group row" id="div_lotes_`+ num + `">
                                                                        <div class="col-md-1">
                                                                            <input style="text-align:center; background-color:antiquewhite" type="text" class="form-control col-md-12 input-contador-lotes" value="" readonly="readonly">
                                                                        </div>
                                                                        <input type="hidden" name="produccion_lotes.Index" id="produccion_lotes.Index" value="`+ num + `" />
                                                                        <label class="control-label col-md-1" for="produccion_lotes[`+ num + `].numero_lote_izquierdo">
                                                                            <span class="float-right">Lote Izquierdo</span>
                                                                        </label>
                                                                        <div class="col-md-2">
                                                                            <input style="text-align:right" type="number" min="0" step="1" max="50000" name="produccion_lotes[`+ num + `].numero_lote_izquierdo" id="produccion_lotes[` + num + `].lote_izquierdo" class="form-control col-md-12" value="" autocomplete="off">
                                                                            <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[` + num + `].numero_lote_izquierdo" data-valmsg-replace="true"></span>
                                                                        </div>
                                                                        <label class="control-label col-md-1" for="produccion_lotes[`+ num + `].numero_lote_derecho">
                                                                            <span class="float-right">Lote Derecho</span>
                                                                        </label>
                                                                        <div class="col-md-2">
                                                                            <input type="number" min="0" step="1" max="50000" name="produccion_lotes[`+ num + `].numero_lote_derecho" id="produccion_lotes[` + num + `].lote_derecho" class="form-control col-md-12" value="" autocomplete="off">
                                                                            <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[` + num + `].numero_lote_derecho" data-valmsg-replace="true"></span>
                                                                        </div>
                                                                        <label class="control-label col-md-2" for="produccion_lotes[`+ num + `].piezas_paquete">
                                                                             <span class="float-right">Piezas por paquete</span>
                                                                        </label>
                                                                        <div class="col-md-2">
                                                                            <input type="number" min="0" step="1" max="5000" name="produccion_lotes[`+ num + `].piezas_paquete" id="produccion_lotes[` + num + `].piezas_paquete" class="form-control col-md-12 total_piezas" value="" maxlength="10" required>
                                                                               <span class="field-validation-valid text-danger" data-valmsg-for="produccion_lotes[` + num + `].piezas_paquete" data-valmsg-replace="true"></span>
                                                                            </div>
                                                                        <div class="col-md-1">
                                                                            <input type="button" value="Borrar" class="btn btn-danger" onclick="borrarLote(` + num + `); return false;">
                                                                        </div>
                                                                    </div>
                                                                `
    );
    $("#div_lotes_" + num).hide().fadeIn(500);
    num++;


    //vuelve a asigar el evento on a cada input de los lotes
    $('.total_piezas').each(function () {
        $(this).on('input', function (e) {
            calculaDatos();
        });
    });


    AsignaNumeroLote();
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