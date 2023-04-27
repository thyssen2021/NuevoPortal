$(document).ready(function () {

    var Toast = Swal.mixin({
        toast: true,
        position: 'top-right',
        iconColor: 'white',
        customClass: {
            popup: 'colored-toast'
        },
        icon: 'success',
        title: 'General Title',
        animation: false,
        position: 'top-right',
        showConfirmButton: false,
        timer: 5000,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.addEventListener('mouseenter', Swal.stopTimer)
            toast.addEventListener('mouseleave', Swal.resumeTimer)
        }
    });

    mostrarAlerta();

    //muestra una alerta solo si existen los elementos de mensaje y tipoMensaje
    function mostrarAlerta() {

        var elementTipo = document.getElementById("mensajeAlertTipo");
        var elementMensaje = document.getElementById("mensajeAlert");

        if (elementTipo != null && elementMensaje!=null) {
            var tipo = elementTipo.value;
            var mensaje = elementMensaje.value;

            Toast.fire({
                icon: tipo,
                title: mensaje
            })
        }
    

    }

    
});