// Scripts/Views/CTZ_Projects/EditClientPartInfo/page.main.js

(function () {
    'use strict';

    // 1. "Importamos" las variables globales que necesitamos
    const config = window.pageConfig;

    const debouncedUpdateTheoreticalLine = debounce(updateTheoreticalLine, 600);
    const debouncedFetchAndApplyValidationRanges = debounce(fetchAndApplyValidationRanges, 600);

    // 2. Aquí va tu $(document).ready
    $(document).ready(function () {
        // ***************** INICIALIZACIONES *****************

        // Configuración de Toastr
        toastr.options = {
            "closeButton": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "timeOut": "3000",
            "extendedTimeOut": "1000"
        };

        // Desactiva los eventos globales de AJAX para esta página.
        // Esto previene que la pantalla de "Cargando..." aparezca en cada llamada.
        setTimeout(function () {
            $(document).off('ajaxStart ajaxStop');
            console.log("Global AJAX events have been disabled for this page.");
        }, 0);



        // Inicializar Select2 en los selects
        $(".select2").select2({ width: '100%' });

        $(function () {
            $('[data-toggle="tooltip"]').tooltip();
        });


        updateReturnableFieldsVisibility();
        handleReturnableRackChange();
        // Llamar a la función al cargar la página para establecer el estado inicial
        updateDeliveryTransportationTypeState();
        // Llama a los handlers al cargar la página para establecer el estado inicial
        handleInterplantScrapReconciliationChange();
        handleInterplantHeadTailReconciliationChange();
        updateInterplantReturnableFieldsVisibility();
        // Llama a la función una vez al cargar la página para establecer el estado inicial
        toggleTkmmPackagingFields();
        // Activa la lógica para los textareas condicionales
        setupConditionalTextareas();


        if (isDetailsMode) {
            // --- MODO DETALLES ACTIVADO ---

            // 1. Asegurarse de que los Select2 se vean correctamente deshabilitados
            $(".select2").select2({
                disabled: true
            });

            // 2. Desactivar los eventos de click para editar o copiar, como doble seguro
            $(document).off("click", ".edit-row");
            $(document).off("click", ".copy-row");
            $(document).off("click", ".remove-row");

            // 3. Notificar al usuario que está en modo de solo lectura
            toastr.info('Displaying in read-only details mode.', 'Details Mode');
        }


        // Ocultar inicialmente los selects de Vehicle 2, 3 y 4
        $("#Vehicle_2").closest(".col-md-6").hide();
        $("#Vehicle_3").closest(".col-md-6").hide();
        $("#Vehicle_4").closest(".col-md-6").hide();

        // Llamar a la función al cargar la página
        updateVehicleVisibility();


        // Inicializa el select2 para Quality con tags y sugerencias
        var qualityData = config.lists.qualityList;

        //llama al evento de update shape
        shapeVisibility();

        //llama al metodo para mostrar las graficas, OnlyBDMaterials = true
        UpdateCapacityGraphs(true)
        UpdateCapacityHansontable(true)

        $(".select2-quality").select2({
            tags: true,
            data: qualityData,
            width: '100%',
            placeholder: "Select or enter quality"
        }).val(null).trigger("change");  //vacio por defecto


        //obtiene las lineas de sliter disponibles
        $.getJSON(config.urls.getSlitterLines, { plantId: config.project.plantId })
            .done(function (data) {
                availableSlitterLines = data;
            });

        //solo inicializa datepicker si puede editar
        if (canEditSales) {

            // Inicializa datepicker para Real_SOP
            $("#Real_SOP").datepicker({
                format: "yyyy-mm",       // Formato año-mes (en minúsculas, ya que bootstrap-datepicker usa "mm" para mes)
                startView: "months",     // Inicia mostrando meses
                minViewMode: "months",   // Permite seleccionar solo meses
                autoclose: true
            });

            // Inicializa datepicker para Real_EOP
            $("#Real_EOP").datepicker({
                format: "yyyy-mm",
                startView: "months",
                minViewMode: "months",
                autoclose: true
            });

        }

        // Declaramos una variable para el timeout
        let updateTimeout;

        //inicia eventos para validacion en tiempo real
        IniciaValidacionTiempoReal();

        // Inicializar select2 y evento solo si es automotriz
        if (vehicleType == 1) {
            $("#Vehicle, #Vehicle_2, #Vehicle_3, #Vehicle_4").select2({ width: '100%' });

            $("#Vehicle, #Vehicle_2, #Vehicle_3, #Vehicle_4").on("change", function () {
                recalculateVehicleData();
            });
        }

        // Dispara el evento 'change' en el combo box
        $('#ID_Route').trigger('change');

        // Llamar a la gráfica al cargar la página con el parámetro en true
        updateSlitterCapacityChart(true, config.project.id);

        if (config.project.successMessage) {
            toastr.success(config.project.successMessage);
        }


        // ***************** MANEJO DE EVENTOS *****************
        // Cuando el usuario cambia el país, se recarga la lista de vehículos de forma asíncrona.
        $(document).on('change', '.ihs-country-selector', function () {

            // Si el elemento tiene la bandera 'data-in-sync' en true, ignoramos este evento
            // porque la función syncSingleVehicleAndCountry ya se está encargando de la carga.
            if ($(this).data('in-sync') === true) {
                return;

            }
            const country = $(this).val();
            const targetVehicleSelector = $(this).data('target-vehicle'); // Obtenemos el selector del vehículo asociado

            if (country && targetVehicleSelector) {
                // Llamamos a la nueva función que carga vehículos para un dropdown específico.
                loadVehiclesForDropdown(country, targetVehicleSelector);
            }
        });

        $('#PackagingStandard').on('change', function () {
            toggleTkmmPackagingFields(); // Mantenemos la llamada a la otra función que ya existía

        });

        $('#IsWeldedBlank').on('change', handleWeldedBlankChange);
        $('#numberOfPlates').on('input', function () {
            // Limpia el temporizador anterior cada vez que se presiona una tecla.
            clearTimeout(numberOfPlatesTimeout);

            // Establece un nuevo temporizador.
            numberOfPlatesTimeout = setTimeout(function () {
                // Esta función solo se ejecutará 500ms después de que el usuario deje de escribir.
                generateThicknessInputs();
                validateNumberOfPlates();
            }, 500); // 500 milisegundos de espera (puedes ajustar este valor)
        });

        $('#rack-types-container').on('change', '.rack-type-checkbox', function () {
            updateReturnableFieldsVisibility();
            validateReturnableUses();
        });


        // 2. Modificamos el evento existente para el checkbox "Returnable Rack".
        $('#IsReturnableRack').on('change', function () {
            handleReturnableRackChange();
            validateReturnableUses(); // Es importante re-validar aquí también.
        });

        // Listener para los checkboxes de Interplant Rack Type
        $('#interplant-rack-types-container').on('change', '.interplant-rack-type-checkbox', function () {
            // Llama a la nueva función que acabamos de crear
            updateInterplantReturnableFieldsVisibility();
            // También revalidamos "Uses" por si acaso
            validateMaterial("InterplantReturnableUses");
        });

        // 1. Asocia el evento 'input' a los campos que disparan el cálculo.
        $("#Annual_Volume, #Volume_Per_year")
            .on("input", function () {
                updateWeightCalculations(); // <-- CAMBIO DE NOMBRE DE LA FUNCIÓN
            });

        // Listener para Interplant Returnable Rack
        $('#IsInterplantReturnableRack').on('change', function () {
            handleInterplantReturnableRackChange();
            validateMaterial("IsInterplantReturnableRack");
        });

          // Asociar el evento al dropdown
        $("#ID_Arrival_Protective_Material").on("change", handleArrivalProtectiveMaterialChange);
        // Asociar el evento al checkbox "Is Stackable"
        $("#Is_Stackable").on("change", handleStackableChange);
        // Asociar el evento al dropdown "Arrival Transport Type"
        $("#ID_Arrival_Transport_Type").on("change", handleArrivalTransportTypeChange);

        // Evento para campos de texto que SIEMPRE disparan el cálculo de línea teórica
        $("#Tensile_Strenght, " +
            "#Thickness, " +
            "#Width, " +
            "#Pitch")
            .on("change keyup", function () {
                clearTimeout(updateTimeout);
                updateTimeout = setTimeout(debouncedUpdateTheoreticalLine, 800);
            });

        // Evento para los DropDowns y campos que deben disparar la lógica INMEDIATAMENTE
        $("#ID_Route, " +
            "#ID_Material_type, " +
            "#ID_Real_Blanking_Line, " +
            "#ID_Slitting_Line, " +
            "#Multipliers")
            .on("change", function () {
                const fieldId = $(this).attr('id');

                // Ejecuta la validación básica del campo que cambió
                if (fieldId === 'ID_Material_type' ||
                    fieldId === 'Multipliers') {
                    validateMaterial(fieldId);
                }

                // Si el cambio fue en la RUTA, actualizamos la UI de las líneas de producción
                if (fieldId === 'ID_Route') {
                    const selectedId = parseInt($(this).val(), 10);
                    const $slittingLineSelect = $("#ID_Slitting_Line");

                    if (slittingRouteIds.includes(selectedId)) {
                        $slittingLineSelect.empty().append('<option value="">Select slitter</option>');
                        $.each(availableSlitterLines, function (i, item) {
                            $slittingLineSelect.append($('<option>', { value: item.Value, text: item.Text }));
                        });
                        if (availableSlitterLines.length === 1) {
                            $slittingLineSelect.val(availableSlitterLines[0].Value).trigger('change.select2');
                        }
                    }
                }

                // Si el cambio fue en la Ruta o en la Línea de Slitter, actualizamos el dropdown de Material Type.
                if (fieldId === 'ID_Route' || fieldId === 'ID_Slitting_Line') {
                    updateMaterialTypeDropdown();
                }

                // La acción principal es siempre llamar a updateTheoreticalLine.
                // Esta función ahora contiene la lógica para decidir si debe continuar o no.
                debouncedUpdateTheoreticalLine();
            });

        //agrega el evento para el cambio de archivo
        $("#changeFileButton").on("click", function () {
            let fileUploadContainer = $("#cat_file_container");
            let fileActionsContainer = $("#fileActions_container");
            let archivoInput = $("#archivo");
            let cancelButton = $("#cancelFileButton");

            fileUploadContainer.show();
            fileActionsContainer.hide();
            cancelButton.show();

            archivoInput.prop("disabled", false);
        });

        $("#cancelFileButton").on("click", function () {
            let fileUploadContainer = $("#cat_file_container");
            let fileActionsContainer = $("#fileActions_container");
            let archivoInput = $("#archivo");
            let cancelButton = $("#cancelFileButton");

            fileUploadContainer.hide();
            fileActionsContainer.show();
            cancelButton.hide();

            archivoInput.prop("disabled", true);
        });

        // Evento para el botón CHANGE del archivo de empaque
        $("#changePackagingFileButton").on("click", function () {
            // Seleccionamos los contenedores específicos del packaging
            let fileUploadContainer = $("#packaging_file_container");
            let fileActionsContainer = $("#packagingFileActions_container");
            let archivoInput = $("#packaging_archivo");
            let cancelButton = $("#cancelPackagingFileButton");

            // Mostramos el input para subir archivo y ocultamos los botones de acción
            fileUploadContainer.show();
            fileActionsContainer.hide();
            cancelButton.show(); // Mostramos el botón de cancelar cambio

            // Habilitamos el input de archivo si el usuario puede editar
            if (canEditSales) {
                archivoInput.prop("disabled", false);
            }
        });

        // Evento para el botón CANCEL del archivo de empaque
        $("#cancelPackagingFileButton").on("click", function () {
            let fileUploadContainer = $("#packaging_file_container");
            let fileActionsContainer = $("#packagingFileActions_container");
            let archivoInput = $("#packaging_archivo");
            let cancelButton = $(this);

            // Si ya había un archivo guardado, volvemos a mostrar el enlace de descarga
            if ($("#ID_File_Packaging").val()) {
                fileUploadContainer.hide();
                fileActionsContainer.show();
            }

            // Limpiamos el input por si el usuario seleccionó un archivo nuevo y luego canceló
            archivoInput.val('');
            cancelButton.hide();
        });

        $("#changeFileButtonTechnicalSheetFile").on("click", function () {
            let fileUploadContainer = $("#file_container_technicalSheetFile");
            let fileActionsContainer = $("#fileActions_containerTechnicalSheetFile");
            let archivoInput = $("#technicalSheetFile");
            let cancelButton = $("#technicalSheetFileCancelButton");

            fileUploadContainer.show();
            fileActionsContainer.hide();
            cancelButton.show();

            archivoInput.prop("disabled", false);
        });

        $("#technicalSheetFileCancelButton").on("click", function () {
            let fileUploadContainer = $("#file_container_technicalSheetFile");
            let fileActionsContainer = $("#fileActions_containerTechnicalSheetFile");
            let archivoInput = $("#technicalSheetFile");
            let cancelButton = $("#technicalSheetFileCancelButton");

            fileUploadContainer.hide();
            fileActionsContainer.show();
            cancelButton.hide();

            archivoInput.prop("disabled", true);
        });

        $("#changeFileAdditional").on("click", function () {
            let fileUploadContainer = $("#file_container_AdditionalFile");
            let fileActionsContainer = $("#fileActions_containerAdditionalFile");
            let archivoInput = $("#AdditionalFile");
            let cancelButton = $("#AdditionalFileCancelButton");

            fileUploadContainer.show();
            fileActionsContainer.hide();
            cancelButton.show();

            archivoInput.prop("disabled", false);
        });

        $("#AdditionalFileCancelButton").on("click", function () {
            let fileUploadContainer = $("#file_container_AdditionalFile");
            let fileActionsContainer = $("#fileActions_containerAdditionalFile");
            let archivoInput = $("#AdditionalFile");
            let cancelButton = $("#AdditionalFileCancelButton");

            fileUploadContainer.hide();
            fileActionsContainer.show();
            cancelButton.hide();
            archivoInput.prop("disabled", true);
        });

        $("#changeFileArrivalAdditionalFile").on("click", function () {
            let fileUploadContainer = $("#file_container_arrivalAdditionalFile");
            let fileActionsContainer = $("#fileActions_containerArrivalAdditionalFile");
            let archivoInput = $("#arrivalAdditionalFile");
            let cancelButton = $("#arrivalAdditionalFileCancelButton");

            fileUploadContainer.show();
            fileActionsContainer.hide();
            cancelButton.show();
            archivoInput.prop("disabled", false);
        });

        $("#arrivalAdditionalFileCancelButton").on("click", function () {
            let fileUploadContainer = $("#file_container_arrivalAdditionalFile");
            let fileActionsContainer = $("#fileActions_containerArrivalAdditionalFile");
            let archivoInput = $("#arrivalAdditionalFile");
            let cancelButton = $("#arrivalAdditionalFileCancelButton");

            fileUploadContainer.hide();
            fileActionsContainer.show();
            cancelButton.hide();
            archivoInput.prop("disabled", true);
        });

        // Coil Data Additional File
        $("#changeFileCoilDataAdditionalFile").on("click", function () {
            $("#file_container_coilDataAdditionalFile").show();
            $("#fileActions_containerCoilDataAdditionalFile").hide();
            $("#coilDataAdditionalFileCancelButton").show();
            $("#coilDataAdditionalFile").prop("disabled", false);
        });
        $("#coilDataAdditionalFileCancelButton").on("click", function () {
            $("#file_container_coilDataAdditionalFile").hide();
            $("#fileActions_containerCoilDataAdditionalFile").show();
            $(this).hide();
            $("#coilDataAdditionalFile").prop("disabled", true).val('');
        });

        // Slitter Data Additional File
        $("#changeFileSlitterDataAdditionalFile").on("click", function () {
            $("#file_container_slitterDataAdditionalFile").show();
            $("#fileActions_containerSlitterDataAdditionalFile").hide();
            $("#slitterDataAdditionalFileCancelButton").show();
            $("#slitterDataAdditionalFile").prop("disabled", false);
        });
        $("#slitterDataAdditionalFileCancelButton").on("click", function () {
            $("#file_container_slitterDataAdditionalFile").hide();
            $("#fileActions_containerSlitterDataAdditionalFile").show();
            $(this).hide();
            $("#slitterDataAdditionalFile").prop("disabled", true).val('');
        });

        // Volume Additional File
        $("#changeFileVolumeAdditionalFile").on("click", function () {
            $("#file_container_volumeAdditionalFile").show();
            $("#fileActions_containerVolumeAdditionalFile").hide();
            $("#volumeAdditionalFileCancelButton").show();
            $("#volumeAdditionalFile").prop("disabled", false);
        });
        $("#volumeAdditionalFileCancelButton").on("click", function () {
            $("#file_container_volumeAdditionalFile").hide();
            $("#fileActions_containerVolumeAdditionalFile").show();
            $(this).hide();
            $("#volumeAdditionalFile").prop("disabled", true).val('');
        });

        // Outbound Freight Additional File
        $("#changeFileOutboundFreightAdditionalFile").on("click", function () {
            $("#file_container_outboundFreightAdditionalFile").show();
            $("#fileActions_containerOutboundFreightAdditionalFile").hide();
            $("#outboundFreightAdditionalFileCancelButton").show();
            $("#outboundFreightAdditionalFile").prop("disabled", false);
        });
        $("#outboundFreightAdditionalFileCancelButton").on("click", function () {
            $("#file_container_outboundFreightAdditionalFile").hide();
            $("#fileActions_containerOutboundFreightAdditionalFile").show();
            $(this).hide();
            $("#outboundFreightAdditionalFile").prop("disabled", true).val('');
        });

        // Delivery Packaging Additional File
        $("#changeFileDeliveryPackagingAdditionalFile").on("click", function () {
            $("#file_container_deliveryPackagingAdditionalFile").show();
            $("#fileActions_containerDeliveryPackagingAdditionalFile").hide();
            $("#deliveryPackagingAdditionalFileCancelButton").show();
            $("#deliveryPackagingAdditionalFile").prop("disabled", false);
        });
        $("#deliveryPackagingAdditionalFileCancelButton").on("click", function () {
            $("#file_container_deliveryPackagingAdditionalFile").hide();
            $("#fileActions_containerDeliveryPackagingAdditionalFile").show();
            $(this).hide();
            $("#deliveryPackagingAdditionalFile").prop("disabled", true).val('');
        });

        // Listeners para Interplant Packaging File
        $("#changeFileInterplantPackagingFile").on("click", function () {
            $("#file_container_interplant_packaging_archivo").show();
            $("#fileActions_containerInterplantPackagingFile").hide();
            $("#interplant_packaging_archivoCancelButton").show();
            $("#interplant_packaging_archivo").prop("disabled", false);
        });
        $("#interplant_packaging_archivoCancelButton").on("click", function () {
            // Revisa si hay un ID de archivo guardado
            if ($("#ID_File_InterplantPackaging").val()) {
                $("#file_container_interplant_packaging_archivo").hide();
                $("#fileActions_containerInterplantPackagingFile").show();
            } else {
                $("#file_container_interplant_packaging_archivo").show();
                $("#fileActions_containerInterplantPackagingFile").hide();
            }
            $(this).hide();
            $("#interplant_packaging_archivo").prop("disabled", true).val('');
        });

        // Listeners para Interplant Outbound Freight File
        $("#changeFileInterplantOutboundFreightFile").on("click", function () {
            $("#file_container_interplantOutboundFreightAdditionalFile").show();
            $("#fileActions_containerInterplantOutboundFreightFile").hide();
            $("#interplantOutboundFreightAdditionalFileCancelButton").show();
            $("#interplantOutboundFreightAdditionalFile").prop("disabled", false);
        });
        $("#interplantOutboundFreightAdditionalFileCancelButton").on("click", function () {
            // Revisa si hay un ID de archivo guardado
            if ($("#ID_File_InterplantOutboundFreight").val()) {
                $("#file_container_interplantOutboundFreightAdditionalFile").hide();
                $("#fileActions_containerInterplantOutboundFreightFile").show();
            } else {
                $("#file_container_interplantOutboundFreightAdditionalFile").show();
                $("#fileActions_containerInterplantOutboundFreightFile").hide();
            }
            $(this).hide();
            $("#interplantOutboundFreightAdditionalFile").prop("disabled", true).val('');
        });

        // Asociar el evento change a cada select para actualizar la visibilidad
        $("#Vehicle").on("change", updateVehicleVisibility);
        $("#Vehicle_2").on("change", updateVehicleVisibility);
        $("#Vehicle_3").on("change", updateVehicleVisibility);

        //previene enter en formulario
        $(window).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });

        $("#ID_Route").on("change", function () {
            let selectedValue = $(this).val();
            let selectedId = parseInt(selectedValue, 10);
            const $realLineSelect = $("#ID_Real_Blanking_Line");
            const $slittingLineSelect = $("#ID_Slitting_Line");
            // --- LÓGICA CENTRALIZADA Y COMPLETA ---

            // 1. Limpiar los rangos de validación y la visualización INMEDIATAMENTE.
            // Esto asegura que al cambiar a una ruta sin límites, los antiguos desaparezcan.
            window.engineeringRanges = null;
            updateLimitsDisplay(); // Usamos la nueva función unificada

            // 2. Lógica de visibilidad de campos (oculta todo y luego muestra lo relevante)
            allFields.forEach(field => {
                $(`#${field}`).prop("disabled", true);
                $(`#${field}_container`).fadeOut(0);
            });

            if (routeFieldMap[selectedValue]) {
                routeFieldMap[selectedValue].forEach(field => {
                    $(`#${field}_container`).fadeIn(0);

                    if (canEditSales) {
                        $(`#${field}`).prop("disabled", false);
                    }
                });
            }

            // 3. Visibilidad de secciones especiales como "Shape & Angles"
            //if (routeFieldMap[selectedValue] && routeFieldMap[selectedValue].some(field => blankDataFields.includes(field))) {
            //    $("#blankDataDiv").fadeIn(0);
            //} else {
            //    $("#blankDataDiv").fadeOut(0);
            //}
            shapeVisibility();

            // Esta lógica ahora se ejecuta DESPUÉS de que la ruta ha mostrado/ocultado sus campos.
            if (requiresInterplant) {
                // Si el proyecto requiere proceso interplanta...
                if (selectedValue /*&& interplantAllowedRouteIds.includes(selectedId) -> para indicar en que ruta aparecera*/) {
                    handleInterplantPackagingStandardChange();
                    updateInterplantReturnableFieldsVisibility();
                    handleInterplantReturnableRackChange();
                    handleInterplantDeliveryTransportTypeChange();
                    handleInterplantScrapReconciliationChange();
                    handleInterplantHeadTailReconciliationChange();
                    // Disparar los triggers de "Other" para Interplant
                    $('#interplant-additional-6').trigger('change');
                    $('#interplant-label-3').trigger('change');
                    $('#ID_Interplant_Plant_container').slideDown();
                    if (canEditSales) {
                        $('#ID_Interplant_Plant').prop('disabled', false);
                    }
                } else {
                    // ...pero no hay una ruta seleccionada, lo ocultamos.
                    $('#ID_Interplant_Plant_container').slideUp();
                    $('#ID_Interplant_Plant').val("").trigger('change.select2');
                }
            } else {
                // Si el proyecto NO requiere proceso interplanta, el campo SIEMPRE debe estar oculto.
                $('#ID_Interplant_Plant_container').slideUp();
            }

            // 4. Comportamiento específico para las líneas de producción

            // A. Lógica para el campo "Slitting Line"
            if (slittingRouteIds.includes(selectedId)) {
                // Poblamos el dropdown con las líneas de slitter que cargamos al inicio
                $slittingLineSelect.empty().append('<option value="">Select slitter</option>');
                $.each(availableSlitterLines, function (i, item) {
                    $slittingLineSelect.append($('<option>', { value: item.Value, text: item.Text }));
                });

                // Si solo hay una línea de slitter en la planta, la seleccionamos por defecto
                if (availableSlitterLines.length === 1) {
                    $slittingLineSelect.val(availableSlitterLines[0].Value);
                }
            }


            // B. Lógica para los campos de "Blanking Line"
            if (blankingRouteIds.includes(selectedId)) {
                // Si la ruta incluye BLANKING, habilitamos la línea real y disparamos el cálculo de la teórica.
                $realLineSelect.prop("disabled", !canEditEngineering);
                debouncedUpdateTheoreticalLine();
            } else {
                // Si la ruta NO incluye BLANKING, deshabilitamos y limpiamos todo lo relacionado.
                $realLineSelect.prop("disabled", true).val("");
                $("#theoretical_blk_line").val("N/A for this route");
                $("#ID_Theoretical_Blanking_Line, #Theoretical_Strokes, #Real_Strokes").val("");
            }


            handleArrivalProtectiveMaterialChange();
            handleStackableChange()
            handleArrivalTransportTypeChange();

            // 5. Disparamos la actualización de los rangos de validación y de la UI de los selects
            debouncedFetchAndApplyValidationRanges();
            $(".select2").trigger('change.select2');

            // 6. Lógica de mensajes para el usuario (Toastr)
            const selectedText = $(this).find('option:selected').text();
            let messageParts = [];


            if (slittingRouteIds.includes(selectedId) && blankingRouteIds.includes(selectedId)) {
                messageParts.push("Combined process detected: Slitting followed by Blanking.");
            } else if (slittingRouteIds.includes(selectedId)) {
                messageParts.push("Slitting process detected; validation will be against slitting line dimensions.");
                // (Tu código de Swal.fire para mostrar las dimensiones del slitter se mantiene si lo deseas)
            } else if (blankingRouteIds.includes(selectedId)) {
                messageParts.push("Validation will be against the chosen theoretical or real blanking line.");
            } else {
                messageParts.push("Theoretical/Slitting line is not required for this route.");
            }

            if (messageParts.length > 0) {
                const finalMessage = `Route selected: <strong>${selectedText}</strong>. <br>` + messageParts.join('<br>');
                //toastr.info(finalMessage, "Route Information", { timeOut: 7000 });
            }

            handleScrapReconciliationChange();
            handleHeadTailReconciliationChange();
            updateReturnableFieldsVisibility();
            handleDeliveryTransportTypeChange();
            updateDeliveryTransportationTypeState();

            // --- LÓGICA DE VISIBILIDAD DE GRÁFICAS ---
            console.log(`--- Lógica de Visibilidad de Gráficas ---`);
            console.log(`Ruta seleccionada ID: ${selectedId}`);

            // Gráfica de Blanking
            if (blankingRouteIds.includes(selectedId)) {
                console.log("✅ La ruta incluye BLANKING. Mostrando gráficas de capacidad de línea.");
                $("#chartsContainer").slideDown(); // Muestra el contenedor de gráficas de BLK
            } else {
                console.log("❌ La ruta NO incluye BLANKING. Ocultando gráficas de capacidad de línea.");
                $("#chartsContainer").slideUp(); // Oculta el contenedor de gráficas de BLK
            }

            // Gráfica de Slitter
            if (slittingRouteIds.includes(selectedId)) {
                console.log("✅ La ruta incluye SLITTER. Llamando a updateSlitterCapacityChart().");
                updateSlitterCapacityChart(); // Llama a la función para cargar y mostrar la gráfica
            } else {
                console.log("❌ La ruta NO incluye SLITTER. Ocultando gráfica de capacidad de Slitter.");
                $("#slitterChartContainer").slideUp(); // Oculta la gráfica si la ruta no es de slitter
            }
            console.log(`--- Fin de Lógica de Visibilidad ---`);
            // --- FIN DE LÓGICA DE VISIBILIDAD ---

        });

        // Configuración de Toastr
        toastr.options = {
            "closeButton": true,
            "progressBar": true,
            "positionClass": "toast-top-right",
            "timeOut": "3000",
            "extendedTimeOut": "1000"
        };

        /*******************************************************************/
        //Al dar clic en  edit
        $(document).on("click", ".edit-row", function () {
            const row = $(this).closest("tr"); // Obtenemos la fila inmediatamente

            // 1. Mostrar una alerta de confirmación con SweetAlert2
            Swal.fire({
                title: 'Edit Material?',
                text: "This will discard any unsaved changes in the form and load data from the selected row. Are you sure you want to continue?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#009ff5',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, continue!'
            }).then((result) => {
                // 2. Si el usuario confirma, procedemos a cargar los datos
                if (result.isConfirmed) {

                    // --- INICIO DE TU LÓGICA DE EDICIÓN ---

                    //hace visible el boton para ingenieria.
                    if (canEditEngineering) {
                        $("#id-btn-save-engineering").show();
                    }

                    // Poblar el formulario con los datos de la fila
                    loadRowDataIntoForm(row);

                    // Cambiar el texto del botón principal
                    $('.btn-add-material').html('<i class="fa-solid fa-pen-to-square"></i> Save Changes');

                    // --- FIN DE TU LÓGICA DE EDICIÓN ---

                    // 3. Mostrar el toast de éxito DESPUÉS de que todo ha cargado
                    toastr.success('Material data has been loaded into the form.');
                }
            });
        });

        $(document).on("click", ".details-row", function () {
            // Obtenemos la fila de la tabla
            const row = $(this).closest("tr");

            // Llamamos directamente a la función que carga los datos, SIN confirmación
            loadRowDataIntoForm(row);

            // Notificamos al usuario que los datos se han cargado
            toastr.success('Material details have been loaded into the form for viewing.');
        });

        // Evento para el botón COPIAR
        $(document).on("click", ".copy-row", function () {
            let row = $(this).closest("tr");

            Swal.fire({
                title: 'Copy Material?',
                text: "This will load the selected material's data into the form.",
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#009ff5',
                confirmButtonText: 'Yes, copy it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    // 1. Poblar el formulario con los datos de la fila
                    loadRowDataIntoForm(row);

                    // 2. Realizar acciones específicas de la copia
                    $("#materialIndex").val("");
                    $("#materialId").val("0");
                    // a. Limpiar los valores de los archivos CAD y de Empaque
                    $("#ID_File_CAD_Drawing").val("");
                    $("#CADFileName").val("");
                    $("#ID_File_Packaging").val("");
                    $("#PackagingFileName").val("");
                    $("#ID_File_TechnicalSheet").val("");
                    $("#TechnicalSheetFileName").val("");
                    $("#ID_File_Additional").val("");
                    $("#AdditionalFileName").val("");
                    $("#ID_File_ArrivalAdditional").val("");
                    $("#ArrivalAdditionalFileName").val("");
                    $("#ID_File_CoilDataAdditional").val(""); $("#CoilDataAdditionalFileName").val("");
                    $("#ID_File_SlitterDataAdditional").val(""); $("#SlitterDataAdditionalFileName").val("");
                    $("#ID_File_VolumeAdditional").val(""); $("#VolumeAdditionalFileName").val("");
                    $("#ID_File_OutboundFreightAdditional").val(""); $("#OutboundFreightAdditionalFileName").val("");
                    $("#ID_File_DeliveryPackagingAdditional").val(""); $("#DeliveryPackagingAdditionalFileName").val("");

                    // b. Llamar a las funciones que actualizan la UI de los archivos.
                    //    Estas funciones verán que los IDs están vacíos y mostrarán los selectores de archivo.
                    updateFileUI();
                    updatePackagingFileUI();
                    updateFileUIGeneric('ID_File_TechnicalSheet', 'TechnicalSheetFileName', 'file_container_technicalSheetFile', 'fileActions_containerTechnicalSheetFile', 'downloadFileTechnicalSheetFile');
                    updateFileUIGeneric('ID_File_Additional', 'AdditionalFileName', 'file_container_AdditionalFile', 'fileActions_containerAdditionalFile', 'downloadFileAdditional');
                    updateFileUIGeneric('ID_File_CoilDataAdditional', 'CoilDataAdditionalFileName', 'file_container_coilDataAdditionalFile', 'fileActions_containerCoilDataAdditionalFile', 'downloadCoilDataAdditionalFile');
                    updateFileUIGeneric('ID_File_SlitterDataAdditional', 'SlitterDataAdditionalFileName', 'file_container_slitterDataAdditionalFile', 'fileActions_containerSlitterDataAdditionalFile', 'downloadSlitterDataAdditionalFile');
                    updateFileUIGeneric('ID_File_VolumeAdditional', 'VolumeAdditionalFileName', 'file_container_volumeAdditionalFile', 'fileActions_containerVolumeAdditionalFile', 'downloadVolumeAdditionalFile');
                    updateFileUIGeneric('ID_File_OutboundFreightAdditional', 'OutboundFreightAdditionalFileName', 'file_container_outboundFreightAdditionalFile', 'fileActions_containerOutboundFreightAdditionalFile', 'downloadOutboundFreightAdditionalFile');
                    updateFileUIGeneric('ID_File_DeliveryPackagingAdditional', 'DeliveryPackagingAdditionalFileName', 'file_container_deliveryPackagingAdditionalFile', 'fileActions_containerDeliveryPackagingAdditionalFile', 'downloadDeliveryPackagingAdditionalFile');

                    // Limpiar los campos Part Name y Part Number
                    $('#partName').val('');
                    $('#partNumber').val('');

                    $('.btn-add-material').html('<i class="fa-solid fa-plus"></i> Add as New Copy');
                    toastr.success("Material data copied.");
                }
            });
        });

        // Botón para agregar/actualizar material
        $(document).on("click", ".btn-add-material", function () {

            // 1. ACTIVAR BANDERA DE BLOQUEO
            // Esto evitará que UpdateCapacityGraphs se ejecute por los efectos secundarios de la validación.
            window.isBatchValidating = true;

            // 2. Realizar la validación
            let isValid = validateMaterial();

            // 3. GESTIÓN DE LA BANDERA (IMPORTANTE)
            // No la desactivamos inmediatamente (false), porque los 'debounce' de los inputs
            // (ej. 1000ms en Real_SOP) pueden estar en cola y dispararse en un segundo.
            // Mantenemos el bloqueo durante 1.2 segundos para asegurar que absorba esos disparos.
            setTimeout(function () {
                window.isBatchValidating = false;
            }, 5000);

            if (!isValid) {
                toastr.warning("Please check the material data. Some fields are missing or contain invalid information.");

                // --- LÓGICA DE SCROLL AUTOMÁTICO AL PRIMER ERROR ---

                // 1. Buscar el primer mensaje de error visible en el formulario
                // (Los inputs inválidos muestran su span .error-message correspondiente)
                var $firstError = $(".material-form .error-message:visible").first();

                if ($firstError.length > 0) {
                    // 2. Encontrar el input asociado (generalmente es hermano anterior o está en el mismo contenedor)
                    // Buscamos en el padre (.col-md-...) cualquier input, select o textarea
                    var $inputField = $firstError.parent().find("input, select, textarea").first();

                    // Si es un select2, debemos resaltar el contenedor de select2, no el select oculto
                    var $targetElement = $inputField;
                    if ($inputField.hasClass("select2-hidden-accessible")) {
                        $targetElement = $inputField.next(".select2-container").find(".select2-selection");
                    }

                    // 3. Hacer Scroll suave hacia el elemento (ajustando un poco el offset para que no quede pegado arriba)
                    $('html, body').animate({
                        scrollTop: $targetElement.offset().top - 150
                    }, 500);

                    // 4. Aplicar foco (si es input normal) y animación visual
                    $inputField.focus();

                    // Agregar clase de animación
                    $targetElement.addClass("input-error-focus");

                    // Quitar la clase después de que termine la animación para poder repetirla si valida de nuevo
                    setTimeout(function () {
                        $targetElement.removeClass("input-error-focus");
                    }, 1000);
                }

                return; // Detener el guardado
            }

            let index = $("#materialIndex").val().trim();
            let materialData = {};

            // Recoge valores de los inputs
            for (let col of columnDefs) {

                // Si el tipo es checkboxGroup, lo ignoramos en este bucle,
                // porque se procesa manualmente más abajo.
                if (col.type === "checkboxGroup") {
                    continue; // Salta a la siguiente iteración
                }

                let $input = $(col.selector);
                let val;

                // Checkbox
                if (col.type === "check") {
                    // .is(':checked') devuelve true/false
                    val = $input.is(":checked").toString();
                }
                // Hidden siempre lo tomamos
                else if ($input.attr("type") === "hidden") {
                    val = $input.val();
                }
                // Visible normal
                else if ($input.is(":visible")) {
                    val = $input.val();
                }
                // Oculto → vacío
                else {
                    val = "";
                }

                // Comprobamos si la clave es uno de los campos de Coil Position
                if (col.key === 'ID_Coil_Position' ||
                    col.key === 'ID_Delivery_Coil_Position' ||
                    col.key === 'ID_InterplantDelivery_Coil_Position' ||
                    col.key === 'ID_Arrival_Packaging_Type' || // <-- AÑADIDO
                    col.key === 'ID_Arrival_Protective_Material') { // <-- AÑADIDO

                    // Si el valor seleccionado es "0" (N/A), guarda un string vacío
                    // para que el servidor lo interprete como 'null'.
                    materialData[col.key] = (val === "0") ? "" : val;
                } else {
                    // Asignación normal para todos los demás campos
                    materialData[col.key] = val;
                }
            }

            // Recoger los IDs de los Rack Types seleccionados
            materialData.SelectedRackTypeIds = [];
            $('#rack-types-container .rack-type-checkbox:checked').each(function () {
                materialData.SelectedRackTypeIds.push($(this).val());
            });

            materialData.SelectedAdditionalIds = [];
            $('#additionals-container .additional-checkbox:checked').each(function () { materialData.SelectedAdditionalIds.push($(this).val()); });

            materialData.SelectedLabelIds = [];
            $('#labels-container .label-checkbox:checked').each(function () { materialData.SelectedLabelIds.push($(this).val()); });

            materialData.SelectedStrapTypeIds = [];
            $('#straps-container .strap-checkbox:checked').each(function () { materialData.SelectedStrapTypeIds.push($(this).val()); });

            materialData.SelectedInterplantRackTypeIds = [];
            $('#interplant-rack-types-container .interplant-rack-type-checkbox:checked').each(function () {
                materialData.SelectedInterplantRackTypeIds.push($(this).val());
            });

            materialData.SelectedInterplantLabelIds = [];
            $('#interplant-labels-container .form-check-input:checked').each(function () {
                materialData.SelectedInterplantLabelIds.push($(this).val());
            });

            materialData.SelectedInterplantAdditionalIds = [];
            $('#interplant-additionals-container .form-check-input:checked').each(function () {
                materialData.SelectedInterplantAdditionalIds.push($(this).val());
            });

            materialData.SelectedInterplantStrapTypeIds = [];
            $('#interplant-straps-container .form-check-input:checked').each(function () {
                materialData.SelectedInterplantStrapTypeIds.push($(this).val());
            });

            // Aquí: si el input de archivo (#archivo) tiene algún valor, marcamos este material.
            // Consideramos que solo el material que se agregue cuando se tenga un archivo tendrá la bandera.
            materialData["IsFile"] = $("#archivo").val() ? "true" : "false";
            materialData["IsPackagingFile"] = $("#packaging_archivo").val() ? "true" : "false";
            materialData["IsTechnicalSheetFile"] = $("#technicalSheetFile").val() ? "true" : "false";
            materialData["IsAdditionalFile"] = $("#AdditionalFile").val() ? "true" : "false";
            materialData["IsArrivalAdditionalFile"] = $("#arrivalAdditionalFile").val() ? "true" : "false";
            materialData["IsCoilDataAdditionalFile"] = $("#coilDataAdditionalFile").val() ? "true" : "false";
            materialData["IsSlitterDataAdditionalFile"] = $("#slitterDataAdditionalFile").val() ? "true" : "false";
            materialData["IsVolumeAdditionalFile"] = $("#volumeAdditionalFile").val() ? "true" : "false";
            materialData["IsOutboundFreightAdditionalFile"] = $("#outboundFreightAdditionalFile").val() ? "true" : "false";
            materialData["IsDeliveryPackagingAdditionalFile"] = $("#deliveryPackagingAdditionalFile").val() ? "true" : "false";
            materialData["IsInterplantPackagingFile"] = $("#interplant_packaging_archivo").val() ? "true" : "false";
            materialData["IsInterplantOutboundFreightFile"] = $("#interplantOutboundFreightAdditionalFile").val() ? "true" : "false";

            console.log("%c--- DEPURANDO LA CREACIÓN DEL JSON ---", "color: orange; font-weight: bold;");

            let platesData = [];
            const isChecked = $('#IsWeldedBlank').is(':checked');
            // La visibilidad del contenedor de 'numberOfPlates' es un mejor indicador
            const isContainerVisible = $('#numberOfPlates_container').is(':visible');

            console.log("Checkbox 'IsWeldedBlank' está marcado?:", isChecked);
            console.log("Contenedor de 'numberOfPlates' es visible?:", isContainerVisible);

            // Cambiamos la condición para que dependa del contenedor de 'numberOfPlates' que es más fiable
            if (isChecked && isContainerVisible) {
                const numPlatesValue = $('#numberOfPlates').val();
                console.log("Valor leído de '#numberOfPlates':", `"${numPlatesValue}"`); // Se muestra entre comillas para ver si está vacío

                const numPlates = parseInt(numPlatesValue, 10);
                console.log("Valor después de parseInt:", numPlates);

                if (!isNaN(numPlates) && numPlates > 0) {
                    console.log(`El bucle 'for' se ejecutará ${numPlates} veces.`);
                    for (let i = 1; i <= numPlates; i++) {
                        const selector = `#thicknessPlate${i}`;
                        const thicknessInput = $(selector);
                        const thicknessValue = thicknessInput.val();

                        console.log(`Iteración ${i}: Selector: '${selector}', ¿Input encontrado?: ${thicknessInput.length > 0}, Valor leído: '${thicknessValue}'`);

                        platesData.push({
                            PlateNumber: i,
                            Thickness: parseFloat(thicknessValue) || 0
                        });
                    }
                } else {
                    console.error("ERROR: numPlates no es un número válido. El bucle no se ejecutará.");
                }
            } else {
                console.log("Condición principal (if) no cumplida. No se recopilarán datos de platinas.");
            }

            materialData["WeldedPlatesJson"] = JSON.stringify(platesData);
            $("#weldedPlatesJson").val(materialData["WeldedPlatesJson"]);

            console.log("Array final 'platesData':", platesData);
            console.log("JSON String final que se usará en la fila:", materialData["WeldedPlatesJson"]);
            console.log("%c--- FIN DE LA DEPURACIÓN ---", "color: orange; font-weight: bold;");


            if (index === "") {
                // AGREGAR NUEVO
                let rowCount = $("#materialsTable tbody tr").length;
                let rowHtml = buildRowHtml(materialData, rowCount);
                $("#materialsTable tbody").append(rowHtml);
                //    toastr.success("Material added successfully.");
            } else {
                // EDITAR MATERIAL EXISTENTE
                let row = $("#materialsTable tbody").find(`tr[data-index='${index}']`);
                let indice = 1; // El incice empieza en 1 porque se moite 0 de la columna de acciones
                let colCells = row.find("td");

                // Actualizar hidden inputs
                let hiddenInputs = row.find("input[name^='materials']");

                hiddenInputs.each(function () {
                    let nameAttr = $(this).attr("name");
                    for (let col of columnDefs) {
                        if (nameAttr.endsWith(`.${col.key}`)) {
                            let newVal;
                            if (col.type === "check") {
                                newVal = materialData[col.key];
                            }
                            else if ($(this).attr("type") === "hidden") {
                                newVal = materialData[col.key];
                            }
                            else {
                                newVal = $(col.selector).is(":visible") ? materialData[col.key] : "";
                            }
                            $(this).val(newVal);
                        }
                    }
                    if (nameAttr.indexOf("IsFile") !== -1) {
                        $(this).val(materialData["IsFile"]);
                    }
                    if (nameAttr.indexOf("IsPackagingFile") !== -1) {
                        $(this).val(materialData["IsPackagingFile"]);
                    }
                    if (nameAttr.indexOf("IsTechnicalSheetFile") !== -1) {
                        $(this).val(materialData["IsTechnicalSheetFile"]);
                    }
                    if (nameAttr.indexOf("IsAdditionalFile") !== -1) {
                        $(this).val(materialData["IsAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsArrivalAdditionalFile") !== -1) {
                        $(this).val(materialData["IsArrivalAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsCoilDataAdditionalFile") !== -1) {
                        $(this).val(materialData["IsCoilDataAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsSlitterDataAdditionalFile") !== -1) {
                        $(this).val(materialData["IsSlitterDataAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsVolumeAdditionalFile") !== -1) {
                        $(this).val(materialData["IsVolumeAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsOutboundFreightAdditionalFile") !== -1) {
                        $(this).val(materialData["IsOutboundFreightAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsDeliveryPackagingAdditionalFile") !== -1) {
                        $(this).val(materialData["IsDeliveryPackagingAdditionalFile"]);
                    }
                    if (nameAttr.indexOf("IsInterplantPackagingFile") !== -1) {
                        $(this).val(materialData["IsInterplantPackagingFile"]);
                    }
                    if (nameAttr.indexOf("IsInterplantOutboundFreightFile") !== -1) {
                        $(this).val(materialData["IsInterplantOutboundFreightFile"]);
                    }
                });

                let actionsCell = row.find('td:first');

                // 2. ELIMINAR LOS INPUTS OCULTOS ANTERIORES PARA LOS RACK TYPES DE ESTA FILA
                row.find("input[name$='.SelectedRackTypeIds']").remove();
                row.find("input[name$='.SelectedAdditionalIds']").remove();
                row.find("input[name$='.SelectedLabelIds']").remove();
                row.find("input[name$='.SelectedStrapTypeIds']").remove();
                row.find("input[name$='.SelectedInterplantRackTypeIds']").remove();
                row.find("input[name$='.SelectedInterplantLabelIds']").remove();
                row.find("input[name$='.SelectedInterplantAdditionalIds']").remove();
                row.find("input[name$='.SelectedInterplantStrapTypeIds']").remove();
                row.find("input[name$='.WeldedPlatesJson']").val(materialData["WeldedPlatesJson"]);


                // 3. AGREGAR NUEVOS INPUTS OCULTOS BASADOS EN LA SELECCIÓN ACTUAL
                if (materialData.SelectedRackTypeIds && materialData.SelectedRackTypeIds.length > 0) {
                    materialData.SelectedRackTypeIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedRackTypeIds" value="${id}" />`);
                    });
                }

                // 4c. RECREAR los inputs para Additionals
                if (materialData.SelectedAdditionalIds && materialData.SelectedAdditionalIds.length > 0) {
                    materialData.SelectedAdditionalIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedAdditionalIds" value="${id}" />`);
                    });
                }

                // 4d. RECREAR los inputs para Labels
                if (materialData.SelectedLabelIds && materialData.SelectedLabelIds.length > 0) {
                    materialData.SelectedLabelIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedLabelIds" value="${id}" />`);
                    });
                }

                // 4e. RECREAR los inputs para Strap Types
                if (materialData.SelectedStrapTypeIds && materialData.SelectedStrapTypeIds.length > 0) {
                    materialData.SelectedStrapTypeIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedStrapTypeIds" value="${id}" />`);
                    });
                }
                // 4f. RECREAR los inputs para Interplant Rack Types
                if (materialData.SelectedInterplantRackTypeIds && materialData.SelectedInterplantRackTypeIds.length > 0) {
                    materialData.SelectedInterplantRackTypeIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedInterplantRackTypeIds" value="${id}" />`);
                    });
                }
                // 4g. RECREAR los inputs para Interplant Label Types
                if (materialData.SelectedInterplantLabelIds && materialData.SelectedInterplantLabelIds.length > 0) {
                    materialData.SelectedInterplantLabelIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedInterplantLabelIds" value="${id}" />`);
                    });
                }
                if (materialData.SelectedInterplantAdditionalIds && materialData.SelectedInterplantAdditionalIds.length > 0) {
                    materialData.SelectedInterplantAdditionalIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedInterplantAdditionalIds" value="${id}" />`);
                    });
                }
                if (materialData.SelectedInterplantStrapTypeIds && materialData.SelectedInterplantStrapTypeIds.length > 0) {
                    materialData.SelectedInterplantStrapTypeIds.forEach(function (id) {
                        actionsCell.append(`<input type="hidden" name="materials[${index}].SelectedInterplantStrapTypeIds" value="${id}" />`);
                    });
                }
            }


            //envia el formulario
            $("#materialForm").submit();

            //clearMaterialForm(columnDefs);
        });

        // Botón Remove
        $(document).on("click", ".remove-row", function () {
            var row = $(this).closest("tr");
            Swal.fire({
                title: 'Are you sure?',
                text: "Do you really want to remove this material?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#009ff5',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, remove it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    row.remove();
                    renumberRows();
                    //envia el formulario
                    $("#materialForm").submit();
                }
            });
        });


        //Al enviar el formularrio muestra un mensaje
        $("#materialForm").on("submit", function () {
            $("#loadingOverlay").show();
        });

        $(".btn-clear-form").click(function () {
            Swal.fire({
                title: 'Clear Form?',
                text: "This will erase all data currently entered in the form fields.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#6c757d',
                confirmButtonText: 'Yes, clear it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    clearMaterialForm(columnDefs);
                    toastr.success('Form cleared.');
                }
            });
        });

    }); // <-- Fin de $(document).ready

    function IniciaValidacionTiempoReal() {
        // Validación en tiempo real para el input de Vehicle
        $("#Vehicle").on("input", function () {
            validateMaterial("Vehicle");
        });

        // Validación en tiempo real para el input de Vehicle_version
        $("#vehicleVersion").on('input', function () {
            validateMaterial("vehicleVersion");
        });

        $("#TonsPerShift").on("input", function () {
            validateMaterial("TonsPerShift");
        });

        // Validación en tiempo real para el input de Vehicle_version
        $("#partName").on('input', function () {
            validateMaterial("partName");
        });

        // Validación en tiempo real para el input de Vehicle_version
        $("#partNumber").on('input', function () {
            validateMaterial("partNumber");
        });

        // Validación en tiempo real para el input de Vehicle_version
        $("#quality").on('input', function () {
            validateMaterial("quality");
        });

        $("#Real_SOP, #Real_EOP").on('input changeDate', function () {
            // 1. Validar el campo específico que cambió
            validateMaterial($(this).attr('name'));

            // 2. Recalcular la producción máxima (Max_Production_SP)
            recalculateVehicleData();

            // 3. Recalcular la gráfica de slitter (esta llamada ya existía)
            debouncedUpdateSlitterChart();
        });

        // Validación en tiempo real para el input de Ship_To
        $("#Ship_To").on('input', function () {
            validateMaterial("Ship_To");
        });

        // Validación en tiempo real para el input de ID Route Dropdown
        $("#ID_Route").on("input", function () {
            validateMaterial("ID_Route");
        });


        // Redondea a tres decimales después de perder el foco
        $("#Tensile_Strenght," +
            "#Thickness," +
            "#Width," +
            "#Pitch," +
            "#Gross_Weight"
        ).on("blur", function () {
            redondearValor(this, 3);
        });


        //packaging standard
        $("#PackagingStandard").on("change", function () {
            validateMaterial("PackagingStandard");
        });

        $("#Tensile_Strenght").on("input", debounce(function () {
            validateMaterial("Tensile_Strenght");
        }, 600));

        $("#Thickness").on("input", debounce(function () {
            validateMaterial("Thickness");
        }, 600));

        $("#Width").on("input", debounce(function () {
            validateMaterial("Width");
        }, 600));

        $("#Pitch").on("input", debounce(function () {
            validateMaterial("Pitch");
        }, 600));

        //validacion en tiempo real
        $("#Gross_Weight").on("input", function () {
            validateMaterial("Gross_Weight");
            updatePackageWeight();
            updateInterplantPackageWeight();
            // Si la forma es Rectangular o Trapecio, copia el valor al peso neto.
            const shapeId = $("#ID_Shape").val();
            if (shapeId === "2" || shapeId === "3") {
                $("#ClientNetWeight").val($(this).val());
                // Re-validamos el campo neto después de cambiarlo.
                validateClientNetWeight();
            }
        });

        $("#Annual_Volume").on("input", function () {
            validateMaterial("Annual_Volume");
        });
        $("#Volume_Per_year").on("input", function () {
            validateMaterial("Volume_Per_year");
        });

        $("#ID_Shape").on("change", function () {
            validateMaterial("ID_Shape");
            shapeVisibility();
            updateWeightFieldsBasedOnShape();
        });

        $("#Angle_A").on("input", function () {
            validateMaterial("Angle_A");
        });
        $("#Angle_B").on("input", function () {
            validateMaterial("Angle_B");
        });
        $("#Blanks_Per_Stroke").on("input", function () {
            validateMaterial("Blanks_Per_Stroke");
        });
        $("#Parts_Per_Vehicle").on("input", function () {
            validateMaterial("Parts_Per_Vehicle");
        });
        $("#Ideal_Cycle_Time_Per_Tool").on("input", function () {
            validateMaterial("Ideal_Cycle_Time_Per_Tool");
        });
        $("#OEE").on("input", debounce(function () {
            validateMaterial("OEE");
            // Recalcular effective strokes al cambiar OEE (con retardo)
            updateEffectiveStrokes();
        }, 400));
        $("#ThicknessToleranceNegative").on("input", function () {
            validateMaterial("ThicknessToleranceNegative");
        });
        $("#ThicknessTolerancePositive").on("input", function () {
            validateMaterial("ThicknessTolerancePositive");
        });
        $("#WidthToleranceNegative").on("input", function () {
            validateMaterial("WidthToleranceNegative");
        });
        $("#WidthTolerancePositive").on("input", function () {
            validateMaterial("WidthTolerancePositive");
        });
        $("#PitchToleranceNegative").on("input", function () {
            validateMaterial("PitchToleranceNegative");
        });
        $("#PitchTolerancePositive").on("input", function () {
            validateMaterial("PitchTolerancePositive");
        });
        $("#AngleAToleranceNegative").on("input", function () {
            validateMaterial("AngleAToleranceNegative");
        });
        $("#AngleATolerancePositive").on("input", function () {
            validateMaterial("AngleATolerancePositive");
        });
        $("#AngleBToleranceNegative").on("input", function () {
            validateMaterial("AngleBToleranceNegative");
        });
        $("#AngleBTolerancePositive").on("input", function () {
            validateMaterial("AngleBTolerancePositive");
        });

        $("#WeightOfFinalMults").on("input", function () {
            validateMaterial("WeightOfFinalMults");
        });
        $("#Multipliers").on("input", debounce(function () {
            validateMaterial("Multipliers");
            validateMaterial("Width_Mults");
            validateMaterial("WeightOfFinalMults_Max"); 
        }, 500));
        $("#MajorBase").on("input", function () {
            validateMaterial("MajorBase");
        });
        $("#MajorBaseToleranceNegative").on("input", function () {
            validateMaterial("MajorBaseToleranceNegative");
        });
        $("#MajorBaseTolerancePositive").on("input", function () {
            validateMaterial("MajorBaseTolerancePositive");
        });
        $("#MinorBase").on("input", function () {
            validateMaterial("MinorBase");
        });
        $("#MinorBaseToleranceNegative").on("input", function () {
            validateMaterial("MinorBaseToleranceNegative");
        });
        $("#MinorBaseTolerancePositive").on("input", function () {
            validateMaterial("MinorBaseTolerancePositive");
        });
        $("#Flatness").on("input", function () {
            validateMaterial("Flatness");
        });
        $("#FlatnessToleranceNegative").on("input", function () {
            validateMaterial("FlatnessToleranceNegative");
        });
        $("#FlatnessTolerancePositive").on("input", debounce(function () {
            validateMaterial("FlatnessTolerancePositive");
        }, 400));

        $("#MasterCoilWeight").on("input", debounce(function () {
            validateMaterial("MasterCoilWeight");
            validateMaterial("WeightOfFinalMults_Max"); 
        }, 400));

        $("#InnerCoilDiameterArrival").on("input", debounce(function () {
            validateMaterial("InnerCoilDiameterArrival");
        }, 400));

        $("#OuterCoilDiameterArrival").on("input", debounce(function () {
            validateMaterial("OuterCoilDiameterArrival");
        }, 400));

        $("#InnerCoilDiameterDelivery").on("input", debounce(function () {
            validateMaterial("InnerCoilDiameterDelivery");
        }, 400));

        $("#OuterCoilDiameterDelivery").on("input", debounce(function () {
            validateMaterial("OuterCoilDiameterDelivery");
        }, 400));
        // Validación en tiempo real para el input de SpecialRequirement
        $("#SpecialRequirement").on('keyup input', function () {
            validateMaterial("SpecialRequirement");
        });
        $("#SpecialPackaging").on('keyup input', function () {
            validateMaterial("SpecialPackaging");
        });

        $("#TurnOverSide").on("change", function () {
            validateMaterial("TurnOverSide");
        });

        $("#Width_Mults").on("input", function () {
            validateMaterial("Width_Mults");
        });
        $("#Width_Mults_Tol_Neg").on("input", function () {
            validateMaterial("Width_Mults_Tol_Neg");
        });
        $("#Width_Mults_Tol_Pos").on("input", function () {
            validateMaterial("Width_Mults_Tol_Pos");
        });
        $("#Width_Plates").on("input", function () {
            validateMaterial("Width_Plates");
        });
        $("#Width_Plates_Tol_Neg").on("input", function () {
            validateMaterial("Width_Plates_Tol_Neg");
        });
        $("#Width_Plates_Tol_Pos").on("input", function () {
            validateMaterial("Width_Plates_Tol_Pos");
        });

        // Validación en tiempo real para el input de Coil Position Dropdown
        $("#ID_Coil_Position").on("change", function () {
            validateMaterial("ID_Coil_Position");
        });

        $("#ID_Arrival_Transport_Type").on("change", function () {
            validateMaterial("ID_Arrival_Transport_Type");
        });

        $("#Arrival_Transport_Type_Other").on("input", function () {
            validateMaterial("Arrival_Transport_Type_Other");
        });

        $("#ID_Arrival_Packaging_Type").on("change", function () {
            validateMaterial("ID_Arrival_Packaging_Type");
        });

        $("#PassesThroughSouthWarehouse").on("change", function () {
            validateMaterial("PassesThroughSouthWarehouse");
        });

        $("#ID_Arrival_Protective_Material").on("change", function () {
            validateMaterial("ID_Arrival_Protective_Material");
        });
        $("#Arrival_Protective_Material_Other").on("input", function () {
            validateMaterial("Arrival_Protective_Material_Other");
        });

        $("#Stackable_Levels").on("input", function () {
            validateMaterial("Stackable_Levels");
        });
        $("#Arrival_Comments").on("input", function () {
            validateMaterial("Arrival_Comments");
        });

        $("#packaging_archivo").on("change", function () {
            validateMaterial("packaging_archivo");
        });

        $("#StrapTypeObservations").on('keyup input', debounce(function () {
            validateMaterial("StrapTypeObservations");
        }, 400)); // 400ms delay

        $("#AdditionalsOtherDescription").on('keyup input', debounce(function () {
            validateMaterial("AdditionalsOtherDescription");
        }, 400)); // 400ms delay

        $("#LabelOtherDescription").on('keyup input', debounce(function () {
            validateMaterial("LabelOtherDescription");
        }, 400)); // 400ms delay


        $("#isRunningChange").on("change", function () {
            toggleRunningChangeWarning();
            validateMaterial("isRunningChange");
            recalculateVehicleData();
        });

        $("#IsCarryOver").on("change", function () {
            validateMaterial("IsCarryOver");
        });

        /* Interplant Fields*/
        $("#ID_InterplantDelivery_Coil_Position").on("change", function () {
            validateMaterial("ID_InterplantDelivery_Coil_Position");
        });
        $("#ID_InterplantDelivery_Transport_Type").on("change", function () {
            handleInterplantDeliveryTransportTypeChange(); // Lógica condicional
            validateMaterial("ID_InterplantDelivery_Transport_Type");
        });
        $("#InterplantDelivery_Transport_Type_Other").on("input", function () {
            validateMaterial("InterplantDelivery_Transport_Type_Other");
        });
        $("#InterplantPackagingStandard").on("change", function () {
            handleInterplantPackagingStandardChange(); // Lógica condicional
            validateMaterial("InterplantPackagingStandard");
        });
        $("#InterplantRequiresRackManufacturing").on("change", function () {
            validateMaterial("InterplantRequiresRackManufacturing");
        });
        $("#InterplantRequiresDieManufacturing").on("change", function () {
            validateMaterial("InterplantRequiresDieManufacturing");
        });

        $("#ID_Interplant_Plant").on("change", function () {
            validateMaterial('ID_Interplant_Plant');
        });

        // Listener para Interplant Additionals "Other" (ID 6)
        $('#interplant-additionals-container').on('change', '#interplant-additional-6', function () {
            const container = $('#InterplantAdditionalsOtherDescription_container');
            if ($(this).is(':checked')) {
                container.slideDown();
            } else {
                container.slideUp();
                container.find('textarea').val('');
                validateMaterial("InterplantAdditionalsOtherDescription"); // Limpiar error
            }
        });

        $('#interplant-labels-container').on('change', '#interplant-label-3', function () {
            const container = $('#InterplantLabelOtherDescription_container');
            if ($(this).is(':checked')) {
                container.slideDown();
            } else {
                container.slideUp();
                container.find('textarea').val(''); // Limpia al ocultar
                validateMaterial("InterplantLabelOtherDescription"); // Limpiar error si lo había
            }
        });
     

        toggleInterplantFacilityField();

        const calculationTriggers = [
            '#ID_Route',
            '#Annual_Volume',
            '#Volume_Per_year',
            '#Theoretical_Gross_Weight',
            '#Gross_Weight',
            '#ClientNetWeight',
            '#Parts_Per_Vehicle',
            '#ID_Material_type' // <--- ASEGÚRATE DE QUE ESTA LÍNEA EXISTA
        ].join(', ');

        // Evento unificado para todos los campos que afectan el cálculo
        $(document).on('input change', calculationTriggers, function () {
            updateCalculatedWeightFields();
        });


        // --- AGREGA ESTE BLOQUE PARA LOS THICKNESS ---
        // Este evento se "adhiere" al documento y escucha por cualquier
        // 'input' que ocurra en un elemento que TENGA la clase '.welded-thickness-input',
        // sin importar si ese elemento existía al cargar la página.
        $(document).on('input', '.welded-thickness-input', function () {
            validateWeldedThicknesses();
        });

        $("#PiecesPerPackage").on("input", function () {
            validateMaterial("PiecesPerPackage");
            updatePackageWeight(); // Recalcular PackageWeight
        });

        $("#StacksPerPackage").on("input", function () {
            validateMaterial("StacksPerPackage");
            updatePackageWeight(); // Recalcular PackageWeight
        });

        $("#ClientNetWeight").on("input", function () {
            toggleRunningChangeWarning();
            validateMaterial("ClientNetWeight");
            updatePackageWeight();
            updateInterplantPackageWeight();
        });

        $("#DeliveryConditions").on("input", function () {
            validateMaterial("DeliveryConditions");
        });

        // Manejador para el cambio en Freight Type
        $('#ID_FreightType').on('change', function () {
            updateDeliveryTransportationTypeState(); // Esta función ya existía
            validateMaterial("ID_FreightType"); // Validar al cambiar
        });

        // Manejador para el checkbox 'Scrap Reconciliation'
        $('#ScrapReconciliation').on('change', function () {
            handleScrapReconciliationChange();
            validateMaterial("ScrapReconciliation"); // Validar al cambiar
        });

        $("#ScrapReconciliationPercent_Min").on("input", function () { validateMaterial("ScrapReconciliationPercent_Min"); });
        $("#ScrapReconciliationPercent_Max").on("input", function () { validateMaterial("ScrapReconciliationPercent_Max"); });

        // Manejador para el checkbox 'Head/Tail Reconciliation'
        $('#HeadTailReconciliation').on('change', function () {
            handleHeadTailReconciliationChange();
            validateMaterial("HeadTailReconciliation"); // Validar al cambiar
        });

        $("#HeadTailReconciliationPercent_Min").on("input", function () { validateMaterial("HeadTailReconciliationPercent_Min"); });
        $("#HeadTailReconciliationPercent_Max").on("input", function () { validateMaterial("HeadTailReconciliationPercent_Max"); });
        $("#ClientScrapReconciliationPercent").on("input", function () { validateMaterial("ClientScrapReconciliationPercent"); });
        $("#ClientHeadTailReconciliationPercent").on("input", function () { validateMaterial("ClientHeadTailReconciliationPercent"); });

        $("#WeightOfFinalMults_Min").on("input", function () { validateMaterial("WeightOfFinalMults_Min"); });
        $("#WeightOfFinalMults_Max").on("input", debounce(function () {
            validateMaterial("WeightOfFinalMults_Max");
            // Nota: validateMaterial("WeightOfFinalMults_Max") llamará internamente a validateWeightMultsCombination
        }, 500));
        $('#Shearing_Width').on('input', function () {
            validateMaterial('Shearing_Width');
            validateMaterial('Shearing_Width_Tol_Pos');
            validateMaterial('Shearing_Width_Tol_Neg');
        });
        $("#Shearing_Width_Tol_Pos").on("input", function () { validateMaterial('Shearing_Width_Tol_Pos'); });
        $("#Shearing_Width_Tol_Neg").on("input", function () { validateMaterial('Shearing_Width_Tol_Neg'); });

        $('#Shearing_Pitch').on('input', function () {
            validateMaterial('Shearing_Pitch');
            validateMaterial('Shearing_Pitch_Tol_Pos');
            validateMaterial('Shearing_Pitch_Tol_Neg');
        });
        $("#Shearing_Pitch_Tol_Pos").on("input", function () { validateMaterial('Shearing_Pitch_Tol_Pos'); });
        $("#Shearing_Pitch_Tol_Neg").on("input", function () { validateMaterial('Shearing_Pitch_Tol_Neg'); });
        $('#Shearing_Weight').on('input', function () {
            validateMaterial('Shearing_Weight');
            validateMaterial('Shearing_Weight_Tol_Pos');
            validateMaterial('Shearing_Weight_Tol_Neg');
        });

        $("#Shearing_Weight_Tol_Pos").on("input", function () { validateMaterial('Shearing_Weight_Tol_Pos'); });
        $("#Shearing_Weight_Tol_Neg").on("input", function () { validateMaterial('Shearing_Weight_Tol_Neg'); });
        $("#Shearing_Pieces_Per_Stroke").on("input", function () { validateMaterial('Shearing_Pieces_Per_Stroke'); });
        $("#Shearing_Pieces_Per_Car").on("input", function () { validateMaterial('Shearing_Pieces_Per_Car'); });

        $("#InterplantPiecesPerPackage").on("input", function () {
            validateMaterial("#InterplantPiecesPerPackage"); // Usa la key/nombre
            updateInterplantPackageWeight();
        });
        $("#InterplantStacksPerPackage").on("input", function () {
            validateMaterial("#InterplantStacksPerPackage"); // Usa la key/nombre
            updateInterplantPackageWeight();
        });

        $("#InterplantLabelOtherDescription").on('keyup input', debounce(function () {
            validateMaterial("InterplantLabelOtherDescription");
        }, 400));

        $("#InterplantAdditionalsOtherDescription").on('keyup input', debounce(function () {
            validateMaterial("InterplantAdditionalsOtherDescription");
        }, 400)); // 400ms delay

        $("#InterplantStrapTypeObservations").on('keyup input', debounce(function () {
            validateMaterial("InterplantStrapTypeObservations");
        }, 400)); // 400ms delay

        $("#ReturnableUses").on("input", function () {
            validateMaterial("ReturnableUses");
        });
        $("#ScrapReconciliationPercent").on("input", function () {
            validateMaterial("ScrapReconciliationPercent");
        });
        $("#HeadTailReconciliationPercent").on("input", function () {
            validateMaterial("HeadTailReconciliationPercent");
        });

        $("#InterplantSpecialRequirement").on('keyup input', debounce(function () { validateMaterial("InterplantSpecialRequirement"); }, 400));
        $("#InterplantSpecialPackaging").on('keyup input', debounce(function () { validateMaterial("InterplantSpecialPackaging"); }, 400));
        $("#interplant_packaging_archivo").on("change", function () { validateMaterial("interplant_packaging_archivo"); });
        $("#IsInterplantReturnableRack").on("change", function () { validateMaterial("IsInterplantReturnableRack"); });
        $("#InterplantReturnableUses").on("input", function () { validateMaterial("InterplantReturnableUses"); });
        $("#ID_Interplant_FreightType").on("change", function () { validateMaterial("ID_Interplant_FreightType"); });
        $("#InterplantDeliveryConditions").on('keyup input', debounce(function () { validateMaterial("InterplantDeliveryConditions"); }, 400));
        $("#InterplantScrapReconciliation").on("change", function () { validateMaterial("InterplantScrapReconciliation"); });
        $("#InterplantScrapReconciliationPercent_Min").on("input", function () { validateMaterial("InterplantScrapReconciliationPercent_Min"); });
        $("#InterplantScrapReconciliationPercent").on("input", function () { validateMaterial("InterplantScrapReconciliationPercent"); });
        $("#InterplantScrapReconciliationPercent_Max").on("input", function () { validateMaterial("InterplantScrapReconciliationPercent_Max"); });
        $("#InterplantClientScrapReconciliationPercent").on("input", function () { validateMaterial("InterplantClientScrapReconciliationPercent"); });
        $("#InterplantHeadTailReconciliation").on("change", function () { validateMaterial("InterplantHeadTailReconciliation"); });
        $("#InterplantHeadTailReconciliationPercent_Min").on("input", function () { validateMaterial("InterplantHeadTailReconciliationPercent_Min"); });
        $("#InterplantHeadTailReconciliationPercent").on("input", function () { validateMaterial("InterplantHeadTailReconciliationPercent"); });
        $("#InterplantHeadTailReconciliationPercent_Max").on("input", function () { validateMaterial("InterplantHeadTailReconciliationPercent_Max"); });
        $("#InterplantClientHeadTailReconciliationPercent").on("input", function () { validateMaterial("InterplantClientHeadTailReconciliationPercent"); });
        $("#interplantOutboundFreightAdditionalFile").on("change", function () { validateMaterial("interplantOutboundFreightAdditionalFile"); });


        $("#ID_Delivery_Coil_Position").on("change", function () {
            validateMaterial("ID_Delivery_Coil_Position");
        });
        $("#ID_Delivery_Transport_Type").on("change", function () {
            handleDeliveryTransportTypeChange(); // <--- Llama a la nueva función
            validateMaterial("ID_Delivery_Transport_Type");
        });
        $("#Delivery_Transport_Type_Other").on("input", function () {
            validateMaterial("Delivery_Transport_Type_Other");
        });

        $("#ID_Arrival_Warehouse").on("change", function () {
            validateMaterial("ID_Arrival_Warehouse");
            // NUEVO: Ejecutar lógica de bloqueo de checkbox
            handleArrivalWarehouseChange();
        });

        $("#RequiresRackManufacturing").on("change", function () { validateMaterial("RequiresRackManufacturing"); });
        $("#RequiresDieManufacturing").on("change", function () { validateMaterial("RequiresDieManufacturing"); });

        // Validación en tiempo real para el input de ID_Real_Blanking_Line Dropdown
        // --- NUEVO MANEJADOR ÚNICO PARA EL CAMBIO DE LÍNEA REAL ---

        // Variable para guardar el valor anterior antes de un cambio
        let previousRealLineValue = null;

        // 1. Antes de que el usuario abra el dropdown, guardamos el valor actual.
        $("#ID_Real_Blanking_Line").on("select2:open", function () {
            previousRealLineValue = $(this).val();
        });

        // 2. Creamos el manejador de evento 'change' que hará todo en orden.
        $("#ID_Real_Blanking_Line").on("change", function () {
            const selectedLineId = $(this).val();
            const selectedMaterialTypeId = $("#ID_Material_type").val();
            // Invocamos la nueva función para calcular el OEE
            fetchAndApplyOee(selectedLineId);

            // Si el usuario no seleccionó una línea (ej. eligió "Select Line"), no hacemos nada.
            if (!selectedLineId || selectedLineId === "0" || selectedLineId === "") {
                // Simplemente disparamos las actualizaciones con la línea teórica.
                debouncedUpdateTheoreticalLine();
                return;
            }

            // Si no hay un tipo de material seleccionado, tampoco podemos validar.
            if (!selectedMaterialTypeId) {
                // En este caso, el cambio de línea es válido, así que procedemos con las demás acciones.
                validateMaterial("ID_Real_Blanking_Line");
                debouncedUpdateCapacityGraphs();
                debouncedUpdateCapacityHansontable();
                return;
            }

            // 3. Hacemos la llamada AJAX para validar la compatibilidad.
            $.getJSON(config.urls.validateLineForMaterial, { lineId: selectedLineId, materialTypeId: selectedMaterialTypeId })
                .done(function (response) {
                    if (response.isValid) {
                        // CASO A: ¡Es compatible! Procedemos con las acciones originales.
                        validateMaterial("ID_Real_Blanking_Line");
                        debouncedUpdateCapacityGraphs();
                        debouncedUpdateCapacityHansontable();
                    } else {
                        // CASO B: ¡No es compatible! Mostramos alerta y revertimos el cambio.
                        const lineName = $("#ID_Real_Blanking_Line option:selected").text();
                        const materialTypeName = $("#ID_Material_type option:selected").text();

                        Swal.fire({
                            icon: 'error',
                            title: 'Incompatible Line',
                            text: `The selected line "${lineName}" cannot process the material type "${materialTypeName}". The selection will be reverted.`
                        });

                        // Revertimos el dropdown a su valor ANTERIOR.
                        $("#ID_Real_Blanking_Line").val(previousRealLineValue).trigger('change.select2');

                        fetchAndApplyOee(previousRealLineValue);
                    }
                })
                .fail(function () {
                    // Si la llamada AJAX falla, mostramos un error y revertimos por seguridad.
                    toastr.error("Could not validate the production line. Reverting selection.");
                    $("#ID_Real_Blanking_Line").val(previousRealLineValue).trigger('change.select2');
                });
        });

        $("#TurnOver").on("change", shapeVisibility);

        //validacion de cadFile
        $("#archivo").on("change", function () {
            validateMaterial("archivo");
            if (this.files.length > 0) {
                const fileName = this.files[0].name;
                $(this).attr("title", fileName); // Aparece como tooltip
            }
        });
        //*** otros metodos y eventos ***

        //Calcula el valor de Theorical gross
        const debouncedUpdateGross = debounce(updateTheoreticalGrossWeight, 300);


        $("#Thickness, #Pitch, #Width, #ID_Material_type, #Blanks_Per_Stroke")
            .on("change keyup", debouncedUpdateGross);

        //solo se ejecuta cuando hay un cambio en linea teorica
        $("#ID_Theoretical_Blanking_Line").on("change", function () {
            var newValue = $(this).val();
            if (newValue !== previousTheoreticalLine) {

                var realProductionLineId = $("#ID_Real_Blanking_Line option:selected").val();

                //si cambio el valor de la linea teorica y no hay Linea real
                if (realProductionLineId == "") {
                    //actualiza la capacidad
                    debouncedUpdateCapacityGraphs();
                    debouncedUpdateCapacityHansontable();
                }
                // Actualizamos la variable previousValue
                previousTheoreticalLine = newValue;
            }
        });
        //llama al metododo wrapper de graficas
        // Attach: para los select2 (cambia de valor directamente)
        $("#Vehicle").on("change", function () {
            debouncedUpdateCapacityGraphs();
            debouncedUpdateCapacityHansontable();
        });

        // Para los inputs en los que el usuario debe dejar de teclear (se usa debounce de 1 segundo)
        // Para los inputs normales, mantenemos el evento "input"
        $("#Parts_Per_Vehicle, #Ideal_Cycle_Time_Per_Tool, #Blanks_Per_Stroke, #OEE, #Annual_Volume")
            .on("input", debounce(function () {
                debouncedUpdateCapacityGraphs();
                debouncedUpdateCapacityHansontable();
            }, 1000));

        // Para los campos de fecha, escuchamos TANTO "input" (teclear) COMO "changeDate" (seleccionar con el picker)
        $("#Real_SOP, #Real_EOP").on("input changeDate", debounce(function () {
            debouncedUpdateCapacityGraphs();
            debouncedUpdateCapacityHansontable();
        }, 1000));
        // Para los campos readonly que se actualizan mediante código, si se dispara manualmente el 'change'
        $("#Real_Strokes, #Theoretical_Strokes").on("change", function () {
            debouncedUpdateCapacityGraphs();
            debouncedUpdateCapacityHansontable();
        });

        $("#InterplantScrapReconciliation").on("change", function () {
            handleInterplantScrapReconciliationChange();
            // Opcional: si necesitas validar los inputs de % cuando se activan
            // validateMaterial("ID_DEL_INPUT_DE_PORCENTAJE");
        });
        $("#InterplantHeadTailReconciliation").on("change", function () {
            handleInterplantHeadTailReconciliationChange();
            // Opcional: si necesitas validar los inputs de % cuando se activan
            // validateMaterial("ID_DEL_INPUT_DE_PORCENTAJE");
        });
    }


    window.IniciaValidacionTiempoReal = IniciaValidacionTiempoReal;
    window.debouncedUpdateTheoreticalLine = debouncedUpdateTheoreticalLine;
    window.debouncedFetchAndApplyValidationRanges = debouncedFetchAndApplyValidationRanges;
})(); // <-- Fin de la IIFE