//variables globales
var num = 0;
const Toast = Swal.mixin({
    toast: true,
    position: 'top-right',
    iconColor: 'white',
    customClass: {
        popup: 'colored-toast'
    },
    showConfirmButton: false,
    timer: 4500,
    timerProgressBar: true
})

$(document).ready(function () {
    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
    $('.check').iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    });

    //agrega evento OnChange para saber si el empleado tiene usuari
    $("#id_planta").change(function () {
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAlmacenes',
            data: { id_planta: $(this).val() },
            success: function (data) {
                try {
                    console.log(data);
                    populateDropdown($("#almacenClave"), data);
                }
                catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Ocurrió un error obteniendo la información: ' + error,
                        confirmButtonText: 'Aceptar',
                    })
                }
            },
            error: function (textStatus, errorThrown) {
                //en caso de error en la llamada ajax
                Swal.fire({
                    icon: 'error',
                    title: 'Ocurrió un error',
                    text: 'Intente nuevamente.'
                })
            },
            async: true
        });
    });

    //agrega evento OnChange para saber el detalle del cliente
    $("#clienteClave").change(function () {
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneClienteDetalles',
            data: { id_cliente: $(this).val() },
            success: function (data) {
                try {
                    console.log(data);
                    $('#clienteOtro').val(data[0].nombre);
                    $('#clienteOtroDireccion').val(data[0].direccion);
                }
                catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Ocurrió un error obteniendo la información: ' + error,
                        confirmButtonText: 'Aceptar',
                    })
                }
            },
            error: function (textStatus, errorThrown) {
                //en caso de error en la llamada ajax
                Swal.fire({
                    icon: 'error',
                    title: 'Ocurrió un error',
                    text: 'Intente nuevamente.'
                })
            },
            async: true
        });
    });
    //agrega evento OnChange para saber el detalle del cliente (enviado a)
    $("#enviadoAClave").change(function () {
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneClienteDetalles',
            data: { id_cliente: $(this).val() },
            success: function (data) {
                try {
                    console.log(data);
                    $('#enviadoAOtro').val(data[0].nombre);
                    $('#enviadoAOtroDireccion').val(data[0].direccion);
                }
                catch (error) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Ocurrió un error obteniendo la información: ' + error,
                        confirmButtonText: 'Aceptar',
                    })
                }
            },
            error: function (textStatus, errorThrown) {
                //en caso de error en la llamada ajax
                Swal.fire({
                    icon: 'error',
                    title: 'Ocurrió un error',
                    text: 'Intente nuevamente.'
                })
            },
            async: true
        });
    });

    $("#transporteProveedorClave").change(function () {
        let transporte = $("#transporteProveedorClave option:selected").text();
        let valor = $("#transporteProveedorClave option:selected").val();
        if (valor == 0)
            $('#transporteOtro').val('');
        else
            $('#transporteOtro').val(transporte);
    });

    $('input').on('ifChecked', function (event) {
        verificaChecks(event);
    });

    $('input').on('ifUnchecked', function (event) {
        verificaChecks(event);
    });

    //evento para leer el archivo xml
    $("#cargar-archivo-btn").click(function () {
        leeXLSX();
    });

    //controla envio del formulario
    $('form #btn-ok').click(function (e) {
        let $form = $(this).closest('form');

        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-success',
                cancelButton: 'btn btn-danger'
            },
            buttonsStyling: false,
        })


        swalWithBootstrapButtons.fire({
            title: '¿Desea Continuar?',
            html: "Se guardarán los cambios realizados.",
            showCancelButton: true,
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.value) {
                //espera a que se cierre el modal para enviar el formulario
                setTimeout(function () {
                    $form.submit();
                }, 800);
            }
        });

    });
    //eventos que se cargan por defecto
    asignaEventosInput();
    AsignaNumeroConcepto();
    calculaDatos();
    //inicializa en input file
    bsCustomFileInput.init();
    //inicializa input mask
    $('.sap').inputmask({ 'alias': 'integer', 'mask': '99999999', 'autoGroup': true, 'autoUnmask': true, 'removeMaskOnSubmit': true });

});

function imprimir(id) {
    //window.open('/@ViewBag.ControllerName/print/' + id, 'popup', 'width=600,height=600');
    popupCenter({ url: '/RM_cabecera/print/' + id, title: 'Imprimir Remisión', w: screen.width * .70, h: screen.height * .70 });
}

function leeXLSX() {
    //var f = $(this);
    var formData = new FormData(document.getElementById("formulario"));
    formData.append("numConcepto", num);
    $.ajax({
        url: "/RM_cabecera/LeeArchivoXLSX",
        type: "post",
        dataType: "html",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (data) {
            try {
                //convierte la respuesta en archivo json
                let datos = JSON.parse(data)
                let estatus = datos[0].status;

                //verifica si se leyó correctamente el archivo xlsx
                if (estatus == "WARNING") {
                    Toast.fire({
                        icon: 'warning',
                        title: 'Mensaje',
                        text: 'Advertencia: ' + datos[0].message,
                    })

                } else if (estatus == "ERROR") {
                    Toast.fire({
                        icon: 'error',
                        title: 'Mensaje',
                        text: 'Ocurrió un error: ' + datos[0].message,
                    })

                } else if (datos[0].status == "SUCCESS") {

                    //inserte el contenido html
                    $("#body_conceptos").append(datos[0].html);
                    asignaEventosInput();
                    AsignaNumeroConcepto();
                    calculaDatos();

                    Toast.fire({
                        icon: 'success',
                        title: 'Mensaje',
                        text: 'Se cargó correctamente. ' + datos[0].message,
                    })
                    $("#archivo_xlsx").val('');
                    $('#archivo_xlsx').next('label').html('Seleccione un archivo...');
                    num = num + datos[0].num_conceptos;
                }

            } catch (error) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Ocurrió un error obteniendo la información: ' + error,
                    confirmButtonText: 'Aceptar',
                })


            }

        },
        error: function (textStatus, errorThrown) {
            //en caso de error en la llamada ajax
            Swal.fire({
                icon: 'error',
                title: 'Ocurrió un error: ' + errorThrown,
                text: 'Intente nuevamente.'
            })
            $("#archivo_xlsx").val('');
            $('#archivo_xlsx').next('label').html('Seleccione un archivo...');
        },
        async: true

    });


}


function verificaChecks(event) {
    if (event.target.id == 'aplicaClienteOtro') {
        checkInputCliente(event);
    } else if (event.target.id == 'aplicaEnviadoOtro') {
        checkInputEnviado(event);
    } else if (event.target.id == 'aplicaTransporteOtro') {
        checkInputTransporte(event);
    }
}

function checkInputCliente(event) {
    //si cliente otro esta seleccionado
    if (event.target.checked) {
        $("#clienteClave").val("");
        $('#clienteClave').prop('disabled', true);
        $('#clienteOtro').prop('readonly', false);
        $('#clienteOtroDireccion').prop('readonly', false);
        $('#clienteOtro').val("");
        $('#clienteOtroDireccion').val("");
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    } else { //si cliente otro no está seleccionado
        $('#clienteClave').prop('disabled', false);
        $('#clienteOtro').prop('readonly', true);
        $('#clienteOtroDireccion').prop('readonly', true);
        $('#clienteOtro').val("");
        $('#clienteOtroDireccion').val("");
    }
}
function checkInputEnviado(event) {
    //si cliente otro esta seleccionado
    if (event.target.checked) {
        $("#enviadoAClave").val("");
        $('#enviadoAClave').prop('disabled', true);
        $('#enviadoAOtro').prop('readonly', false);
        $('#enviadoAOtroDireccion').prop('readonly', false);
        $('#enviadoAOtro').val("");
        $('#enviadoAOtroDireccion').val("");
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    } else { //si cliente otro no está seleccionado
        $('#enviadoAClave').prop('disabled', false);
        $('#enviadoAOtro').prop('readonly', true);
        $('#enviadoAOtroDireccion').prop('readonly', true);
        $('#enviadoAOtro').val("");
        $('#enviadoAOtroDireccion').val("");
    }
}
function checkInputTransporte(event) {
    //si cliente otro esta seleccionado
    if (event.target.checked) {
        $("#transporteProveedorClave").val("");
        $('#transporteProveedorClave').prop('disabled', true);
        $('#transporteOtro').prop('readonly', false);
        $('#transporteOtro').val("");
        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })
    } else { //si cliente otro no está seleccionado
        $('#transporteProveedorClave').prop('disabled', false);
        $('#transporteOtro').prop('readonly', true);
        $('#transporteOtro').val("");
    }
}


//completa el select con los datos recibidos
function populateDropdown(select, data) {
    select.html('');
    $.each(data, function (id, option) {
        select.append($('<option></option>').val(option.value).html(option.name));
    });
}

//agrega una fila para los conceptos
function AgregarConcepto() {


    $("#body_conceptos").append(
        `
                                                                    <tr  id="div_concepto_`+ num + `">
                                                                        <input type="hidden" name="RM_elemento.Index" id="RM_elemento.Index" value="`+ num + `" />
                                                                        <input type="hidden" name="RM_elemento.[`+ num + `].activo" value="true" />
                                                                            <td class="input-contador-conceptos"></td>
                                                                            <td>
                                                                                <input style=" font-size: 12px;" type="text" name="RM_elemento[`+ num + `].numeroParteCliente" id="RM_elemento[` + num + `].numeroParteCliente" class="form-control col-md-12" value="" autocomplete="off" maxlength="50" required>
                                                                                <span class="field-validation-valid text-danger" data-valmsg-for="RM_elemento[` + num + `].numeroParteCliente" data-valmsg-replace="true"></span>
                                                                            </td>
                                                                            <td>
                                                                                <input style=" font-size: 12px;" type="text" name="RM_elemento[`+ num + `].numeroMaterial" id="RM_elemento[` + num + `].numeroMaterial" class="form-control col-md-12" value="" autocomplete="off" maxlength="50" required>
                                                                                <span class="field-validation-valid text-danger" data-valmsg-for="RM_elemento[` + num + `].numeroMaterial" data-valmsg-replace="true"></span>
                                                                            </td>
                                                                             <td>
                                                                                <input style=" font-size: 12px;" type="text" name="RM_elemento[`+ num + `].numeroLote" id="RM_elemento[` + num + `].numeroLote" class="form-control col-md-12" value="" autocomplete="off" maxlength="50" required>
                                                                                <span class="field-validation-valid text-danger" data-valmsg-for="RM_elemento[` + num + `].numeroLote" data-valmsg-replace="true"></span>
                                                                            </td>
                                                                           <td>
                                                                                <input style=" font-size: 12px;" type="text" name="RM_elemento[`+ num + `].numeroRollo" id="RM_elemento[` + num + `].numeroRollo" class="form-control col-md-12" value="" autocomplete="off" maxlength="50" required>
                                                                                <span class="field-validation-valid text-danger" data-valmsg-for="RM_elemento[` + num + `].numeroRollo" data-valmsg-replace="true"></span>
                                                                            </td>
                                                                           <td>
                                                                                <input style=" font-size: 12px;" type="text" name="RM_elemento[`+ num + `].cantidad" id="RM_elemento[` + num + `].cantidad" class="form-control col-md-12 entero" value="" autocomplete="off">
                                                                                <span class="field-validation-valid text-danger" data-valmsg-for="RM_elemento[` + num + `].cantidad" data-valmsg-replace="true"></span>
                                                                            </td>
                                                                            <td>
                                                                                <input style=" font-size: 12px;" type="text" name="RM_elemento[`+ num + `].peso" id="RM_elemento[` + num + `].peso" class="form-control col-md-12 decimal" value="" autocomplete="off">
                                                                                <span class="field-validation-valid text-danger" data-valmsg-for="RM_elemento[` + num + `].peso" data-valmsg-replace="true"></span>
                                                                            </td>

                                                                           <td>
                                                                                 <input type="button" value="Borrar" class="btn btn-danger" onclick="borrarConcepto(` + num + `); return false;">
                                                                            </td>
                                                                     </tr>

                                                                       `
    );
    $("#div_concepto_" + num).hide().fadeIn(700);

    num++;

    asignaEventosInput();

    AsignaNumeroConcepto()
}

//borra un concepto
function borrarConcepto(id) {

    $("#div_concepto_" + id).fadeOut(0, function () {
        $(this).remove();
    });

    AsignaNumeroConcepto()
    calculaDatos()
}

//numera los concepto
function AsignaNumeroConcepto() {
    let indice = 1;

    $('.input-contador-conceptos').each(function (index) {
        $(this).html(indice);
        indice++;
    });
}

//funcion para agregar los eventos de cantidades a los input
function asignaEventosInput() {

    $('.entero').inputmask({ 'alias': 'integer', 'placeholder': '0', 'allowMinus': false, 'min': 0, 'max': 99999, 'autoGroup': true, 'autoUnmask': true, 'removeMaskOnSubmit': true });
    $('.decimal').inputmask({ 'alias': 'decimal', 'groupSeparator': ',', 'autoGroup': true, 'min': 0, 'max': 99999, 'digits': 3, 'digitsOptional': false, 'allowMinus': false, 'placeholder': '0', 'autoUnmask': true, 'removeMaskOnSubmit': true });
    //Agrega el evento a todos los input
    $(".entero").each(function () {
        $(this).on('input keyup', function (e) {
            calculaDatos()
        });
    });

    $(".decimal").each(function () {
        $(this).on('input keyup', function (e) {
            calculaDatos()
        });
    });
}

//cálcula datos
function calculaDatos() {

    //realiza la suma de conceptos Debe
    let total_cantidad = ObtieneTotalCantidad();
    let total_peso = ObtieneTotalPeso();

    //redondea a dos decimales
    total_cantidad = total_cantidad.toFixed(0);
    total_peso = total_peso.toFixed(3);

    //asigna el valor de total_cantidad
    if (total_cantidad != null) {
        $("#total_cantidad").html(total_cantidad);
    } else {
        $("#total_cantidad").html("0");
    }

    //asigna el valor de total haber
    if (total_peso != null) {
        $("#total_peso").html(total_peso);
    } else {
        $("#total_peso").html(" 0.000");
    }
}

window.onload = function () {
    clicMenu(1);
}

function ObtieneTotalCantidad() {
    var suma = 0;
    $('.entero').each(function () {
        suma = suma + ConvierteAFloat($(this).val(), 0)
    });

    return suma;
}

function ObtieneTotalPeso() {
    var suma = 0;
    $('.decimal').each(function () {
        suma = suma + ConvierteAFloat($(this).val(), 0)
    });

    return suma;
}
