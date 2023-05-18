$(document).ready(function () {
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
            cancelButtonText: 'Cerrar'
        }).then((result) => {
            if (result.value) {
                //espera a que se cierre el modal para enviar el formulario
                setTimeout(function () {
                    $form.submit();
                }, 900);
            }
        });

    });

});