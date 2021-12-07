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