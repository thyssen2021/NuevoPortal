$(document).ready(function () {

    var form = document.getElementById("buscarForm");
    //borra los campos antes de enviarlos
    $("#borrarForm").click(function () {
        $('#cliente').val("");
        $('#fecha_inicial').val("");
        $('#fecha_final').val("");
        form.submit();
    });

    // Initialize Select2 Elements (debe ir después de asignar el valor)
    $('.select2bs4').select2({
        theme: 'bootstrap4'
    })
   
    //inicializa la tabla
    $("#excel").DataTable({
        dom: "Bfrtip",
        buttons: [
            {
                extend: "copy",
                text: "Copiar",
                className: "btn btn-success"
            },
            {
                extend: "excel",
                className: "btn btn-success",
                excelStyles: [                     // Add an excelStyles definition
                  
                    {
                        cells: "sH",  // Smart select Columns A and B
                        style: {
                            //fill: {
                            //    pattern: {
                            //        color: 'BCD6EE', // Light red color                                  
                            //    }
                            //},
                            font: {                     // Style the font                            
                                b: true,               // Remove bolding from header row
                            },
                        }
                    },
                    {
                        cells: "sO",  // Smart select Columns A and B
                        style: {
                            //fill: {
                            //    pattern: {
                            //        color: 'BCD6EE', // Light red color
                            //    }
                            //},
                            font: {                     // Style the font                            
                                b: true,               // Remove bolding from header row
                            },
                        }
                    },
                    {
                        cells: "1:2",  // Smart select Columns A and B
                        style: {                        // The style block
                            font: {                     // Style the font
                                name: "Calibri",          // Font name
                                size: "11",             // Font size
                                color: "FFFFFF",        // Font Color
                                b: true,               // Remove bolding from header row
                            },
                            fill: {                     // Style the cell fill (background)
                                pattern: {              // Type of fill (pattern or gradient)
                                    color: "0094ff",    // Fill color
                                }
                            }
                        }
                    },
                    {
                        template: [
                            "outline_blue_gray",
                            "title_medium"]
                        ,   // Apply the "green_medium" template
                    },
                ],
            },

        ],
        responsive: false
    });

    //agranda el tamaño de la barra
    window.onload = function () {
        document.getElementById('menu_toggle').click();
    }
});
