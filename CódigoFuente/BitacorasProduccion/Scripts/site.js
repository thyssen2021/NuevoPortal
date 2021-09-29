$(document).ready(function () {   
    
    $(document).on({
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