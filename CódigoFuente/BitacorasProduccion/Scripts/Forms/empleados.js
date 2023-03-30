$(document).ready(function () {

    $('input').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green',
        increaseArea: '20%' // optional
    });

    var demo1 = $('[name=subordinados]').bootstrapDualListbox({
        nonSelectedListLabel: 'Todos los Empleados',
        selectedListLabel: 'Empleados Seleccionados',
        filterTextClear: 'Mostrar todo',
        filterPlaceHolder: 'Nombre',
        moveSelectedLabel: 'Mover Seleccionados',
        moveAllLabel: 'Mover Todo',
        removeSelectedLabel: 'Quitar Seleccionados',
        removeAllLabel: 'Quitar Todos',
        moveOnSelect: false,
    });
    //elimina botones del DualList
    CustomizeDuallistbox('subordinados');

    //maneja el envió el del formulario
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
            html: "Se guardarán los cambios.",
            showCancelButton: true,
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            if (result.value) {
                //espera a que se cierre el modal para enviar el formulario
                setTimeout(function () {
                    $form.submit();
                }, 900);
            }
        });


    });

    //inicializa en input file
    bsCustomFileInput.init()

    //aumenta en uso cada vez que se hace un cambio en el archivo de soporte
    $("#ArchivoImagen").on("change", function () {
        document.getElementById('cambio_documento_soporte').value = ++documento_soporte_cambios;

        filePreview(this);
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


    //carga los valores cuando se cambia la planta
    $("#planta_clave").change(function () {
        let checkedValue = $('#shared_services:checked').val();

        let planta = $(this).val();

        if (checkedValue == 'true')
            planta = 99;

        //obtiene las areas
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAreas',
            data: { clavePlanta: planta },
            success: function (data) {
                populateDropdown($("#id_area"), data);
            },
            async: true
        });

        //obtiene los puestos
        $.ajax({
            type: 'POST',
            url: '/Combos/obtienePuestos',
            data: { claveArea: 0 }, //para que regrese una lista vacia
            success: function (data) {
                populateDropdown($("#puesto"), data);
            },
            async: true
        });

        verificaEstadoCombos();
    });

    //carga los valores cuando se cambia el área
    $("#id_area").change(function () {

        //obtiene los puestos
        $.ajax({
            type: 'POST',
            url: '/Combos/obtienePuestos',
            data: { claveArea: $(this).val() },
            success: function (data) {
                populateDropdown($("#puesto"), data);
            },
            async: true
        });

        verificaEstadoCombos();
    });

    $('input').on('ifChecked', function (event) {
        verificaChecks();
    });

    $('input').on('ifUnchecked', function (event) {
        verificaChecks();
    });

    //seleccionaValoresDefault();
    //$("#planta_clave").change();

});

function verificaChecks(event) {

    var checkedValue = $('#shared_services:checked').val();

    if (checkedValue) {
        let element = document.getElementById('planta_clave');
        element.value = 1;
        $('#planta_clave').attr('readonly', 'readonly');

        $('.select2bs4').select2({
            theme: 'bootstrap4'
        })

    } else {
        $('#planta_clave').removeAttr('readonly');
    }
    $("#planta_clave").change();

}

//elimina los botones del multiselect
function CustomizeDuallistbox(listboxID) {
    var customSettings = $('#' + listboxID).bootstrapDualListbox('getContainer');
    customSettings.find('.btn-group.buttons').remove();
}

window.onload = function () {
    clicMenu(1);
}

function filePreview(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#div-img-container').html('');
            $('#div-img-container').hide().html('<img src="' + e.target.result + '" width="350" height="350"/>').show(150);
        }
        reader.readAsDataURL(input.files[0]);
    } else {
        $('#div-img-container').hide(150).html('');
    }
}


//variable para saber si ha cambiado el formulario y asi no se detenga por doble submit
var documento_soporte_cambios = 1;

//muestra el formulario de carga de archivo
function muestraFileInput() {
    $("#div_document_support").fadeOut(500, function () {
        $("#ArchivoImagen").val('');
    });
    $("#div_document_support_2").fadeIn(500);
    //oculta la imagen original
    $('#div-img-container').html('');

    document.getElementById('cambio_documento_soporte').value = ++documento_soporte_cambios;
    document.getElementById('elimina_documento').value = 'true';
}

//oculta el formulario de carga de archivo
function ocultaFileInput() {
    $("#div_document_support").fadeIn(700, function () {
        $("#ArchivoImagen").val('');
    });
    $("#div_document_support_2").fadeOut(700);

    document.getElementById('cambio_documento_soporte').value = ++documento_soporte_cambios;
    document.getElementById('elimina_documento').value = 'false';

    //muestra la imagen original
    let src = $("#imagen-previa").val();
    $('#div-img-container').hide().html('<img src="' + src + '" width="350" height="350"/>').show(150);
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

    var planta = $("#planta_clave option:selected").val();
    var area = $("#id_area option:selected").val();


    if (planta == "") {
        //si no hay planta desactiva area y puesto
        $("#id_area").prop("disabled", true);
        $("#puesto").prop("disabled", true);
    }
    else {
        //si hay planta activa área
        $("#id_area").prop("disabled", false);

        //verifica el estado del area
        if (area == "") {
            //si no hay área, desactiva puesto
            $("#puesto").prop("disabled", true);
        }
        else {
            //si hay área activa puesto
            $("#puesto").prop("disabled", false);
        }
    }

}

//selecciona los valores que carga por defecto
function seleccionaValoresDefault() {

    var c_planta = document.getElementById("c_planta");
    var c_area = document.getElementById("c_area");
    var c_puesto = document.getElementById("c_puesto");

    //PRIMER COMBO
    if (c_planta != null) {

        //estable el valor del primer combo (no es neesario ajax)
        if ($("#planta_clave option[value='" + c_planta.value + "']").length > 0) {
            $("#planta_clave").val(c_planta.value);
        } else {
            $("#planta_clave").val("");
        }

        //realiza una llamada sincorna del contenido del segundo combo
        //obtiene las areas
        $.ajax({
            type: 'POST',
            url: '/Combos/obtieneAreas',
            data: { clavePlanta: c_planta.value },
            success: function (data) {
                populateDropdown($("#id_area"), data);
            },
            async: false
        });

        //SEGUNDO COMBO
        //verifica si hay valores para el segundo combo
        if (c_area != null) {

            //estable el valor del segundo combo
            if ($("#id_area option[value='" + c_area.value + "']").length > 0) {
                $("#id_area").val(c_area.value);
            } else {
                $("#id_area").val("");
            }

            //realiza una llamada sincorna del contenido del tercer combo

            $.ajax({
                type: 'POST',
                url: '/Combos/obtienePuestos',
                data: { claveArea: c_area.value },
                success: function (data) {
                    populateDropdown($("#puesto"), data);
                },
                async: false
            });

            //TERCER COMBO
            //verifica si hay valores para el tercer combo
            if (c_puesto != null) {

                //estable el valor del segundo combo
                if ($("#puesto option[value='" + c_puesto.value + "']").length > 0) {
                    $("#puesto").val(c_puesto.value);
                } else {
                    $("#puesto").val("");
                }
            } else {
                verificaEstadoCombos();
            }
        } else {
            verificaEstadoCombos();
        }


    } else { //en caso de que no haya valores previos (CREATE)
        verificaEstadoCombos();
    }

    //debe ir despues de inicializar los combos
    // Initialize Select2 Elements
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
}