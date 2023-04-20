$(document).ready(function () {

    $(document).on({


        //muestra una pantalla cada vez que se realiza una solicitud ajax
        ajaxStart: function () {
            $.blockUI({
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .3,
                    color: '#fff'
                },
                message: '<h3>Cargando...</h3>'

            });
        },
        ajaxStop: function () { $.unblockUI(); }



    });

    //agrega el evento a los formularios
    //$('form:not(.js-allow-double-submission)').preventDoubleSubmission();

    var submittedFormContent = null;
    $('form:not(.js-allow-double-submission)').submit(function (e) {
        var newFormContent = $(this).serialize();

        if (submittedFormContent === newFormContent) {
            console.log('ya se envio el formulario')
            e.preventDefault(true);
        }
        else {
            console.log('Enviando el formulario')
            submittedFormContent = newFormContent;
        }
    });


});

//hace clic el nunmero de veces indicado en el menu (utilizado para desplegar el menu)
function clicMenu(num) {

    for (var i = 1; i <= num; i++) {
        document.getElementById('menu_toggle').click();
    }
}

function volverAtras() {
    history.go(-1);
}

//convierte texto a float
function ConvierteAFloat(str, defaultValue) {
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

//bloquea la pantalla
function BloqueaPantalla() {
    $.blockUI({
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .3,
            color: '#fff'
        },
        message: '<h3>Cargando...</h3>'
    });
}


//Convierte jqueryval a espalo

if (typeof this.jQuery.validator !== 'undefined' && this.jQuery.validator.messages !== 'undefined') {

    var jQueryValEnglish = $('#jquery_val_english').val();

    if (typeof jQueryValEnglish === 'undefined') {
        jQuery.extend(jQuery.validator.messages, {
            required: "Este campo es obligatorio.",
            remote: "Por favor, rellena este campo.",
            email: "Por favor, escribe una dirección de correo válida",
            url: "Por favor, escribe una URL válida.",
            date: "Por favor, escribe una fecha válida.",
            dateISO: "Por favor, escribe una fecha (ISO) válida.",
            number: "Por favor, escribe un número entero válido.",
            digits: "Por favor, escribe sólo dígitos.",
            creditcard: "Por favor, escribe un número de tarjeta válido.",
            equalTo: "Por favor, escribe el mismo valor de nuevo.",
            accept: "Por favor, escribe un valor con una extensión aceptada.",
            maxlength: jQuery.validator.format("Por favor, no escribas más de {0} caracteres."),
            minlength: jQuery.validator.format("Por favor, no escribas menos de {0} caracteres."),
            rangelength: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1} caracteres."),
            range: jQuery.validator.format("Por favor, escribe un valor entre {0} y {1}."),
            max: jQuery.validator.format("Por favor, escribe un valor menor o igual a {0}."),
            min: jQuery.validator.format("Por favor, escribe un valor mayor o igual a {0}.")

        });
    } 
}

//inicia los tooltip
$('[rel=tooltip]').tooltip();

// jQuery plugin to prevent double submission of forms
jQuery.fn.preventDoubleSubmission = function () {
    $(this).on('submit', function (e) {
        var $form = $(this);

        if ($form.data('submitted') === true) {
            // Previously submitted - don't submit again
            console.log('Ya se envio el formulario')
            e.preventDefault();
        } else {
            // Mark it so that the next submit can be ignored
            $form.data('submitted', true);
        }
    });

    // Keep chainability
    return this;
};

const popupCenter = ({ url, title, w, h }) => {
    // Fixes dual-screen position                             Most browsers      Firefox
    const dualScreenLeft = window.screenLeft !== undefined ? window.screenLeft : window.screenX;
    const dualScreenTop = window.screenTop !== undefined ? window.screenTop : window.screenY;

    const width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
    const height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

    const systemZoom = width / window.screen.availWidth;
    const left = (width - w) / 2 / systemZoom + dualScreenLeft
    const top = (height - h) / 2 / systemZoom + dualScreenTop
    const newWindow = window.open(url, title,
        `
      scrollbars=yes,
      width=${w / systemZoom}, 
      height=${h / systemZoom}, 
      top=${top}, 
      left=${left}
      `
    )

    if (window.focus) newWindow.focus();
}