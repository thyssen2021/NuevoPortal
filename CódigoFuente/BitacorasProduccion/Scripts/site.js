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

});

//hace clic el nunmero de veces indicado en el menu (utilizado para desplegar el menu)
function clicMenu(num) {
   
    for (var i = 1; i <= num; i++) {
        document.getElementById('menu_toggle').click();
    }
}