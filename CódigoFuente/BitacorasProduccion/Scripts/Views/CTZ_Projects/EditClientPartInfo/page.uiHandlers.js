// Scripts/Views/CTZ_Projects/EditClientPartInfo/page.uiHandlers.js

(function () {
    'use strict';

    // 1. "Importamos" las variables globales que necesitamos
    const config = window.pageConfig;
    const requiresInterplant = config.project.requiresInterplant;
    const idProject = config.project.id;
    const canEditSales = config.permissions.canEditSales;
    const blankingRouteIds = window.blankingRouteIds;
    const slittingRouteIds = window.slittingRouteIds;
    // Indica si estamos en un proceso de validación masiva (Save)
    window.isBatchValidating = false;

    // 2. Definimos los manejadores de eventos UI
    function handleHeadTailReconciliationChange() {
        const percentContainer = $(config.fieldSelectors.HeadTailReconciliationPercent + '_container');
        if ($(config.fieldSelectors.HeadTailReconciliation).is(':checked')) {
            percentContainer.slideDown();
        } else {
            percentContainer.slideUp();
            // No es necesario limpiar los valores aquí
        }
    }

    function handleScrapReconciliationChange() {
        const percentContainer = $(config.fieldSelectors.ScrapReconciliationPercent + '_container');
        if ($(config.fieldSelectors.ScrapReconciliation).is(':checked')) {
            percentContainer.slideDown();
        } else {
            percentContainer.slideUp();
            // No es necesario limpiar los valores aquí, el usuario podría querer re-activarlo
        }
    }

    function handleWeldedBlankChange() {
        const isChecked = $('#IsWeldedBlank').is(':checked');
        const numberOfPlatesContainer = $('#numberOfPlates_container');
        const thicknessContainer = $('#weldedPlatesThickness_container');

        if (isChecked) {
            // Si se activa, simplemente mostramos los contenedores.
            // Los valores que ya estaban ahí, se conservarán.
            numberOfPlatesContainer.slideDown();
            thicknessContainer.slideDown();
        } else {
            // Si se desactiva, solo ocultamos. NO borramos los valores de los inputs.
            // El usuario podría haber cometido un error y querer reactivarlo.
            numberOfPlatesContainer.slideUp();
            thicknessContainer.slideUp();
        }
    }

    function updateInterplantPackageWeight() {
        const piecesInput = $(config.fieldSelectors.InterplantPiecesPerPackage);
        const stacksInput = $(config.fieldSelectors.InterplantStacksPerPackage);
        const targetInput = $(config.fieldSelectors.InterplantPackageWeight); // Input donde se mostrará el resultado

        // Obtener valores numéricos, usando 0 como default si están vacíos o no son números
        const pieces = parseFloat(piecesInput.val()) || 0;
        const stacks = parseFloat(stacksInput.val()) || 0;

        // Lógica de fallback para el peso a usar (igual que en updatePackageWeight)
        const clientNetWeight = parseFloat($(config.fieldSelectors.ClientNetWeight).val()) || 0;
        const grossWeight = parseFloat($(config.fieldSelectors.Gross_Weight).val()) || 0;
        const weightToUse = (clientNetWeight > 0) ? clientNetWeight : grossWeight;

        // Calcular el peso solo si todos los componentes son válidos y positivos
        if (pieces > 0 && stacks > 0 && weightToUse > 0) {
            const weight = (pieces * stacks * weightToUse);
            targetInput.val(weight.toFixed(3)); // Mostrar con 3 decimales
        } else {
            targetInput.val(""); // Limpiar si falta algún dato
        }
    }
    function toggleInterplantFacilityField() {
        if (requiresInterplant) {
            $('#ID_Interplant_Plant_container').show();
        } else {
            $('#ID_Interplant_Plant_container').hide();
        }
    }

    function shapeVisibility() {
        let shapeInput = $("#ID_Shape");
        let fileName = $("#CADFileName").val();
        let idFile = $("#ID_File_CAD_Drawing").val();
        let shapeVal = shapeInput.val();

        // Contenedores y control de TurnOver
        let turnoverContainer = $("#TurnOver_container");
        let turnoverInput = $("#TurnOver");
        let turnoversideContainer = $("#TurnOverSide_container");
        let turnoversideInput = $("#TurnOverSide");

        // Campos relacionados con los ángulos
        let camposAngulos = [
            "Angle_A",
            "AngleAToleranceNegative",
            "AngleATolerancePositive",
            "Angle_B",
            "AngleBToleranceNegative",
            "AngleBTolerancePositive",
            "MinorBase",
            "MinorBaseToleranceNegative",
            "MinorBaseTolerancePositive",
            "MajorBase",
            "MajorBaseToleranceNegative",
            "MajorBaseTolerancePositive"
        ];

        // 1) Si Shape no está visible, ocultamos todo y deshabilitamos TurnOver
        if (!shapeInput.is(":visible")) {
            camposAngulos.forEach(campo => {
                $("#" + campo).prop("disabled", true).val("");
                $("#" + campo + "_container").hide();
            });
            $("#archivo").prop("disabled", true).val("");
            $("#cat_file_container, #fileActions_container").hide();

            // TurnOver también se oculta y deshabilita
            turnoverInput.prop("disabled", true).prop("checked", false);
            turnoverContainer.hide();
            turnoversideInput.prop("disabled", true);
            turnoversideInput.val(null).trigger('change.select2');
            turnoversideContainer.hide();

            return;
        }

        // 2) Ángulos sólo si Shape = "3"
        camposAngulos.forEach(campo => {
            let input = $("#" + campo);
            let container = $("#" + campo + "_container");

            if (shapeVal === "3") {
                input.prop("disabled", false);
                container.show();
            } else {
                input.prop("disabled", true).val("");
                container.hide();
            }
        });

        // 3) Lógica de archivo sólo si Shape = "18"


        if (shapeVal === "18") {
            updateFileUI();
        } else {
            $("#cat_file_container").hide();
            $("#fileActions_container").hide();
            $("#archivo").prop("disabled", true).val("");
        }

        // 4) Lógica para TurnOver y TurnOverSide, dependiente de Shape = "18"
        if (shapeVal === "18") {
            // El checkbox "TurnOver" siempre es visible y se puede editar si el shape es 18
            turnoverContainer.show();
            if (canEditSales) {
                turnoverInput.prop("disabled", false);
            }

            // AHORA, la visibilidad de "TurnOverSide" depende del checkbox "TurnOver"
            if (turnoverInput.is(":checked")) {
                turnoversideContainer.show();
                if (canEditSales) {
                    turnoversideInput.prop("disabled", false);
                }
            } else {
                // Si "TurnOver" NO está chequeado, ocultamos, deshabilitamos y reseteamos "TurnOverSide"
                turnoversideContainer.hide();
                turnoversideInput.prop("disabled", true);
                turnoversideInput.val(null).trigger('change.select2');
            }

        } else {
            // Si el Shape no es "18", ocultamos y reseteamos ambos campos
            turnoverContainer.hide();
            turnoverInput.prop("disabled", true).prop("checked", false);

            turnoversideContainer.hide();
            turnoversideInput.prop("disabled", true);
            turnoversideInput.val(null).trigger('change.select2');
        }
    }

    function updateTheoreticalStrokes() {

        var productionLineId = $("#ID_Theoretical_Blanking_Line").val();

        // Suponiendo que tienes los valores en los campos:
        var pitch = parseFloat($("#Pitch").val());
        ////var rotation = parseFloat($("#Max_Production_SP").val()); // valor de giro
        var rotation = 0; // 0 de momento


        if (isNaN(pitch) || isNaN(rotation) || productionLineId == "") {
            // Manejar error: se requieren ambos valores.
            $("#Theoretical_Strokes").val("");
            return;
        }

        $.ajax({
            url: config.urls.getTheoreticalStrokes,
            type: 'GET',
            data: { productionLineId: productionLineId, pitch: pitch, rotation: rotation },
            success: function (response) {
                if (response.success) {
                    // Actualiza el campo de Theoretical_Strokes con el valor obtenido
                    var strokes = parseFloat(response.theoreticalStrokes);
                    var formattedStrokes = (strokes % 1 === 0) ? strokes : strokes.toFixed(2);
                    $("#Theoretical_Strokes").val(formattedStrokes);
                    updateEffectiveStrokes();
                } else {
                    $("#Theoretical_Strokes").val("");
                    updateEffectiveStrokes();
                    toastr.warning("Warning: " + response.message);
                }
            },
            error: function (xhr, status, error) {
                toastr.error("Error en Ajax: " + response.message);
                $("#Theoretical_Strokes").val("");
            },
            async: false
        });
    }

    function updateTheoreticalGrossWeight() {

        // 1. Obtener los valores de los inputs
        let thicknessStr = $("#Thickness").val().trim();
        let widthStr = $("#Width").val().trim();
        let pitchStr = $("#Pitch").val().trim();
        let blanksStr = $("#Blanks_Per_Stroke").val().trim();
        let materialTypeStr = $("#ID_Material_type option:selected").text().trim().toUpperCase();

        // Obtener el ID de la ruta seleccionada
        const selectedRouteId = parseInt($("#ID_Route").val(), 10);

        // Si la ruta contiene Blanking (usa el array que ya tienes en tu vista)
        if (blankingRouteIds.includes(selectedRouteId)) {

            // Validar que los campos para el cálculo de BLK no estén vacíos
            if (!thicknessStr || !widthStr || !pitchStr || !blanksStr || !materialTypeStr || materialTypeStr === "SELECT MATERIAL TYPE") {
                $("#Theoretical_Gross_Weight").val("");
                return;
            }

            let thickness = parseFloat(thicknessStr);
            let width = parseFloat(widthStr);
            let pitch = parseFloat(pitchStr);
            let blanksPerStroke = parseFloat(blanksStr);

            // 2. Determinar la densidad según el tipo de material
            let density = 0;
            if (materialTypeStr.includes("STEEL")) {
                density = 7.85;
            } else if (materialTypeStr.includes("ALU")) {
                density = 2.7;
            } else {
                $("#Theoretical_Gross_Weight").val("");
                return; // Salir si el tipo de material no es válido
            }

            // 3. Realizar el cálculo original para Blanking
            // Asegurarse de que blanksPerStroke no sea cero para evitar división por cero
            if (blanksPerStroke > 0) {
                let weight = (thickness * width * pitch * density) / 1000000 / blanksPerStroke;
                // 4. Actualizar el campo
                $("#Theoretical_Gross_Weight").val(weight.toFixed(3));
            } else {
                $("#Theoretical_Gross_Weight").val("");
            }

        } else {
            // Para cualquier otra ruta que no sea ni SLT puro ni contenga BLK
            $("#Theoretical_Gross_Weight").val("");
        }
    }

    //Función que envuelve la llamada a UpdateCapacityGraphs con debounce
    const debouncedUpdateCapacityGraphs = debounce(function () {
        UpdateCapacityGraphs();
        //llama al metodo para actualizar el ht de capacidad.
        //UpdateCapacityHansontable();
    }, 500); // 300 ms de espera

    const debouncedUpdateCapacityHansontable = debounce(function () {
        UpdateCapacityHansontable();
    }, 500); // 300 ms de espera (puedes ajustar este valor si lo deseas)

    function UpdateCapacityGraphs(OnlyBDMaterials = false) {
        // Si estamos validando el formulario masivamente, no calculamos gráficas.
        if (window.isBatchValidating) {
            console.log("UpdateCapacityGraphs: Bloqueado por validación masiva.");
            return;
        }

        // ---  LÓGICA DE VISIBILIDAD ---
        // Si OnlyBDMaterials es true (carga inicial), confiamos en el servidor o escaneamos la tabla.
        // Pero para actualizaciones en tiempo real, usamos nuestra nueva función.
        if (!OnlyBDMaterials) {
            const activeRoutes = getEffectiveProjectRoutes();

            // Verificamos si AL MENOS UNA de las rutas activas es de Blanking
            // Usamos la constante global 'blankingRouteIds' definida en page.constants.js
            const hasBlankingRoute = activeRoutes.some(r => blankingRouteIds.includes(r));

            if (!hasBlankingRoute) {
                console.log("UpdateCapacityGraphs: No se detectaron rutas de Blanking activas. Ocultando gráficas.");
                $("#chartsContainer").slideUp();
                // También ocultamos la leyenda de turnos
                $("#chartsNote, #shiftLegend").hide();
                return; // <--- DETENER EJECUCIÓN AQUÍ
            } else {
                // Si hay rutas, mostramos el contenedor (el contenido se cargará vía AJAX)
                $("#chartsContainer").slideDown();
            }
        }

        console.log('entra UpdateCapacityGraphs');

        // Obtiene el valor de la línea teórica
        let theoricalBLKID = $("#ID_Theoretical_Blanking_Line").val();

        let materialId = $("#materialId").val();

        // Obtiene el ID de la línea real (del select correspondiente)
        var productionLineId = $("#ID_Real_Blanking_Line option:selected").val();

        // Si la línea real es vacía o 0, se utiliza la línea teórica
        if (productionLineId === "" || productionLineId === "0") {
            productionLineId = theoricalBLKID;
            console.log('Usando linea teorica para calculos');
        }

        // Obtener los valores de los campos necesarios
        var vehicle = $("#Vehicle").val();
        var partsPerVehicle = parseFloat($("#Parts_Per_Vehicle").val()) || null;
        var idealCycleTimePerTool = parseFloat($("#Ideal_Cycle_Time_Per_Tool").val()) || null;
        var blanksPerStroke = parseFloat($("#Blanks_Per_Stroke").val()) || null;
        var oee = parseFloat($("#OEE").val()) || null;
        var rawSOP = $("#Real_SOP").val().trim();
        var rawEOP = $("#Real_EOP").val().trim();
        // Regex simple para YYYY-MM
        var dateRegex = /^\d{4}-\d{2}$/;

        var realSOP = dateRegex.test(rawSOP) ? rawSOP : null;
        var realEOP = dateRegex.test(rawEOP) ? rawEOP : null;
        var annualVol = parseInt($("#Annual_Volume").val()) || null;

        // Campos de fallback para idealCycleTimePerTool
        var realStrokes = parseFloat($("#Real_Strokes").val()) || null;
        var theoreticalStrokes = parseFloat($("#Theoretical_Strokes").val()) || null;

        // Limpiar el contenedor de gráficos
        $("#chartsContainer").html("<p style='color:gray;'>Loading <i class='fa-solid fa-spinner fa-spin-pulse'></i></p>");

        // Validar que existan los campos obligatorios
        var missingFields = [];
        if (!vehicle || vehicle.trim() === "") {
            missingFields.push("Vehicle");
        }
        if (partsPerVehicle === null) {
            missingFields.push("Parts per Vehicle");
        }
        if (blanksPerStroke === null) {
            missingFields.push("Blanks per Stroke");
        }
        if (!realSOP || realSOP.trim() === "") {
            missingFields.push("Real SOP");
        }
        if (!realEOP || realEOP.trim() === "") {
            missingFields.push("Real EOP");
        }
        if (annualVol === null) {
            missingFields.push("Annual Volume");
        }

        // Fallback para idealCycleTimePerTool
        if (idealCycleTimePerTool === null) {
            if (realStrokes !== null) {
                idealCycleTimePerTool = realStrokes;
            } else if (theoreticalStrokes !== null) {
                idealCycleTimePerTool = theoreticalStrokes;
            } else {
                missingFields.push("Ideal Cycle Time (or Real/Theoretical Strokes)");
            }
        }

        // Si faltan campos, no se ejecuta la llamada AJAX; se limpia el contenedor y se muestra un mensaje
        if (missingFields.length > 0 && !OnlyBDMaterials) {
            $("#chartsContainer").html(
                "<p style='color:red;'>Missing required fields: " + missingFields.join(", ") + "</p>"
            );
            return;
        }

        // Realizar la llamada AJAX
        $.ajax({
            url: config.urls.getMaterialCapacityScenariosGraphs,
            type: 'GET',
            data: {
                projectId: idProject,
                materialId: materialId,  // Puede ser null
                blkID: productionLineId,
                vehicle: vehicle,
                partsPerVehicle: partsPerVehicle,
                idealCycleTimePerTool: idealCycleTimePerTool,
                blanksPerStroke: blanksPerStroke,
                oee: oee,
                realSOP: realSOP,
                realEOP: realEOP,
                annualVol: annualVol,
                partNumber: $("#partNumber").val() || null,
                OnlyBDMaterials: OnlyBDMaterials
            },
            success: function (response) {
                var container = document.getElementById("chartsContainer");
                container.innerHTML = ""; // Limpiar contenedor
                if (response.success) {
                    generateCharts(response.data, response.dateRanges || {});

                    // 2) Relleno los inputs dm_status
                    //    Asumo que cada fila/material tiene un <input name="dm_status" data-material-id="…">
                    console.log(response.dmStatuses)

                    // 2) Rellenas los campos ocultos DM_status
                    $.each(response.dmStatuses, function (matId, status) {
                        // busca el input cuyo data-material-id coincida y cuyo name termine en '.DM_status'
                        $('input[data-material-id="' + matId + '"][name$=".DM_status"]')
                            .val(status);
                    });

                    // 3) Rellenas los campos ocultos DM_status_comment
                    $.each(response.dmStatusComments, function (matId, comment) {
                        $('input[data-material-id="' + matId + '"][name$=".DM_status_comment"]')
                            .val(comment);
                    });

                    // 3) Relleno el campo de texto "Status DM" para el material seleccionado
                    // 3) Relleno el campo de texto "Status DM" para el material seleccionado
                    var selMatId = $("#materialId").val();

                    // <<<<< INICIO DE MODIFICACIÓN >>>>>
                    // Si selMatId es "" (un material nuevo), el controlador usa el ID 0 como clave.
                    var keyToSearch = (selMatId === "") ? "0" : selMatId;

                    var dm = response.dmStatuses[keyToSearch];
                    // <<<<< FIN DE MODIFICACIÓN >>>>>

                    if (dm !== undefined) {
                        // este es tu input de texto
                        $("#DM_status").val(dm);
                    }

                    // <<<<< INICIO DE MODIFICACIÓN >>>>>
                    var dmc = response.dmStatusComments[keyToSearch];
                    // <<<<< FIN DE MODIFICACIÓN >>>>>

                    if (dmc !== undefined) {
                        // este es tu input de texto
                        $("#DM_status_comment").html(dmc);
                    }

                    // 4) **Actualiza el <td> visible** para cada material
                    $.each(response.dmStatuses, function (matId, status) {
                        $('td.dm-status-cell[data-material-id="' + matId + '"]')
                            .text(status);
                    });
                    $.each(response.dmStatusComments, function (matId, comment) {
                        $('td.dm-status-comment-cell[data-material-id="' + matId + '"]')
                            .text(comment);
                    });

                } else {
                    $("#DM_status").val('');
                    $("#DM_status_comment").html('');

                    $("#chartsContainer").html(
                        "<p style='color:red;'>Warning: " + response.message + "</p>"
                    );
                    //toastr.error("Error: " + response.message);
                }
            },
            error: function (xhr, status, error) {
                $("#DM_status").val('');
                $("#DM_status_comment").html('');

                $("#chartsContainer").html(
                    "<p style='color:red;'>Error: " + error + "</p>"
                );
                // toastr.error("An error occurred: " + error);
            },
            async: true
        });
    }

    function generateCharts(data, fyRanges = {}, containerSelector = "#chartsContainer") {
        // Limpiar el contenedor principal de gráficos
        const container = $(containerSelector);
        container.empty(); // Esto ahora limpiará el contenedor correcto (sea el de BLK o el de SLT)

        // Si no hay datos, muestra un mensaje y termina.
        if (!data || data.length === 0) {
            container.html("<p>No capacity data available to display.</p>");
            return;
        }

        var currentRow = null;
        var charts = [];      // <-- aquí acumularemos las instancias

        // Iterar sobre cada línea de producción (cada gráfica)
        data.forEach(function (lineData, index) {
            // Comenzar una nueva fila cada 2 gráficos
            if (index % 2 === 0) {
                currentRow = $('<div class="row" style="margin-bottom:20px;"></div>');
                container.append(currentRow);
            }

            // Crear un contenedor de columna (col-md-6)
            var colDiv = $('<div class="col-md-6"></div>');

            // --- INICIO DE LA MODIFICACIÓN: FILTRAR DATOS POR RANGO DE FY ---
            var fyData = lineData.DataByFY;
            var range = fyRanges[lineData.LineId];
            var filteredFYData = fyData; // Por defecto, usamos todos los datos.

            // Solo filtramos si tenemos un rango válido para esta línea.
            if (range && range.MinFY && range.MaxFY) {
                let startIndex = fyData.findIndex(fy => fy.FiscalYear === range.MinFY);
                let endIndex = fyData.findIndex(fy => fy.FiscalYear === range.MaxFY);

                // Si encontramos el FY de inicio y no es el primer elemento, retrocedemos uno.
                //if (startIndex > 0) {
                //    startIndex--;
                //}

                // Si encontramos el FY de fin y no es el último elemento, avanzamos uno.
                if (endIndex !== -1 && endIndex < fyData.length - 1) {
                    endIndex++;
                }

                // Aseguramos que los índices sean válidos antes de cortar el array.
                if (startIndex !== -1 && endIndex !== -1 && startIndex <= endIndex) {
                    filteredFYData = fyData.slice(startIndex, endIndex + 1);
                }
            }

            var labels = filteredFYData.map(fy => fy.FiscalYear);

            // --- Paso 2: Armar datasets agrupados por estatus ---
            var statusMap = {};  // Key: StatusId, Value: { label, data: [], backgroundColor }
            filteredFYData.forEach(function (fyItem) {
                if (fyItem.Breakdown) {
                    fyItem.Breakdown.forEach(function (b) {
                        if (!statusMap[b.StatusId]) {
                            statusMap[b.StatusId] = {
                                statusId: b.StatusId,
                                label: statusLabels[b.StatusId] || ("Status " + b.StatusId),
                                data: [],
                                backgroundColor: statusColors[b.StatusId] || "#999999"
                            };
                        }
                    });
                }
            });
            Object.keys(statusMap).forEach(function (statusId) {
                filteredFYData.forEach(function (fyItem) {
                    var entry = (fyItem.Breakdown || []).find(b => b.StatusId == statusId);
                    statusMap[statusId].data.push(entry ? entry.Percentage : 0);
                });
            });
            // Ordenar los datasets según el orden deseado
            var desiredOrder = ["Spare Parts", "POH", "Casi Casi", "Carry Over", "Quotes"];
            var datasets = Object.values(statusMap).sort(function (a, b) {
                var indexA = desiredOrder.indexOf(a.label);
                var indexB = desiredOrder.indexOf(b.label);
                indexA = indexA === -1 ? desiredOrder.length : indexA;
                indexB = indexB === -1 ? desiredOrder.length : indexB;
                return indexA - indexB;
            });

            // --- Paso 3: Calcular el máximo acumulado para configurar el eje Y ---
            var maxAcc = 0;
            for (let i = 0; i < filteredFYData.length; i++) {
                let sum = 0;
                datasets.forEach(ds => {
                    sum += ds.data[i] || 0;
                });
                if (sum > maxAcc) {
                    maxAcc = sum;
                }
            }
            var maxPercentage = Math.ceil(Math.max(100, maxAcc) * 1.1);

            // --- Paso 4: Crear el canvas para la gráfica y agregarlo a la columna ---
            colDiv.append('<h4>' + lineData.LineName + '</h4>');
            var canvasId = "chartLine_" + lineData.LineId;
            colDiv.append('<canvas id="' + canvasId + '"></canvas>');
            currentRow.append(colDiv);

            // --- Paso 5: Generar la gráfica con Chart.js ---
            var ctx = document.getElementById(canvasId).getContext("2d");
            var chart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: datasets.map(function (ds) {

                        // --- MODIFICACIÓN 2: ESTILO TRANSPARENTE PARA SPARE PARTS ---
                        const isSpareParts = ds.label === "Spare Parts";

                        return {
                            label: ds.label,
                            data: ds.data,
                            // Si es Spare Parts, fondo transparente. Si no, su color normal.
                            backgroundColor: isSpareParts ? 'transparent' : ds.backgroundColor,
                            // El borde mantiene el color original para que se vea la línea "flotante"
                            borderColor: ds.backgroundColor,
                            // Hacemos la línea un poco más gruesa si es Spare Parts para que destaque
                            borderWidth: isSpareParts ? 2 : 1,
                            fill: true, // Mantenemos fill:true para que ocupe espacio en el stack
                            stack: 'CapacityStack',
                            tension: 0,
                            pointRadius: 0
                        };
                    })
                },
                options: {
                    responsive: true,
                    interaction: {
                        mode: 'index',
                        intersect: false
                    },
                    scales: {
                        x: {
                            stacked: true,
                            title: {
                                display: true,
                                text: "Fiscal Year"
                            }
                        },
                        y: {
                            stacked: true,
                            beginAtZero: true,
                            max: maxPercentage,
                            title: {
                                display: true,
                                text: "Capacity (%)"
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    var label = context.dataset.label || "";
                                    var value = context.parsed.y;
                                    return label + ": " + value + "%";
                                },
                                footer: function (tooltipItems) {
                                    var total = tooltipItems.reduce((acc, item) => acc + item.parsed.y, 0);
                                    return "Total: " + total.toFixed(1) + "%";
                                },
                                title: function (tooltipItems) {
                                    return "Fiscal Year: " + tooltipItems[0].label;
                                }
                            }
                        },
                        annotation: {
                            annotations: {
                                line100: {
                                    type: 'line',
                                    yMin: 100,
                                    yMax: 100,
                                    borderColor: 'red',
                                    borderDash: [8, 8],
                                    borderWidth: 3,
                                    label: {
                                        enabled: false,
                                        content: '100% capacity',
                                        position: 'end',
                                        backgroundColor: 'rgba(255,0,0,0.7)',
                                        color: '#fff'
                                    }
                                },
                                line85: {
                                    type: 'line',
                                    yMin: 85,
                                    yMax: 85,
                                    borderColor: 'red',
                                    borderDash: [8, 8],
                                    borderWidth: 3,
                                    label: {
                                        enabled: false,
                                        content: '85% capacity',
                                        position: 'end',
                                        backgroundColor: 'rgba(255,165,0,0.7)',
                                        color: '#fff'
                                    }
                                },
                                line25: {
                                    type: 'line',
                                    yMin: 25,
                                    yMax: 25,
                                    borderColor: '#555555',
                                    borderWidth: 1,
                                    borderDash: [8, 3, 3, 3],
                                    clip: false,            // permite dibujar fuera del área “chartArea”
                                    label: {
                                        enabled: false,
                                        content: '1st shift',
                                        position: 'start',      // ancla al extremo derecho de la línea
                                        xAdjust: 10,          // lo mueve 10px hacia la derecha, fuera de la gráfica
                                        yAdjust: -5,          // opcional: lo mueve 5px hacia arriba
                                        backgroundColor: 'transparent',
                                        color: '#555555'
                                    }
                                },
                                line53: {
                                    type: 'line',
                                    yMin: 53,
                                    yMax: 53,
                                    borderColor: '#555555',
                                    borderWidth: 1,
                                    borderDash: [3, 3],
                                    clip: false,            // permite dibujar fuera del área “chartArea”
                                    label: {
                                        enabled: false,
                                        content: '2nd shift',
                                        position: 'start',      // ancla al extremo derecho de la línea
                                        xAdjust: 10,          // lo mueve 10px hacia la derecha, fuera de la gráfica
                                        yAdjust: -5,          // opcional: lo mueve 5px hacia arriba
                                        backgroundColor: 'transparent',
                                        color: '#555555'
                                    }
                                },
                                line80: {
                                    type: 'line',
                                    yMin: 80,
                                    yMax: 80,
                                    borderColor: '#555555',
                                    borderWidth: 1,
                                    borderDash: [8, 4],        // guion-espacio
                                    clip: false,            // permite dibujar fuera del área “chartArea”
                                    label: {
                                        enabled: false,
                                        content: '3rd shift',
                                        position: 'start',      // ancla al extremo derecho de la línea
                                        xAdjust: 10,          // lo mueve 10px hacia la derecha, fuera de la gráfica
                                        yAdjust: -5,          // opcional: lo mueve 5px hacia arriba
                                        backgroundColor: 'transparent',
                                        color: '#555555'
                                    }
                                },
                                line100_2: {
                                    type: 'line',
                                    yMin: 100,
                                    yMax: 100,
                                    borderColor: '#555555',
                                    borderWidth: 1,
                                    borderDash: [5, 10],
                                    clip: false,            // permite dibujar fuera del área “chartArea”
                                    label: {
                                        enabled: false,
                                        content: '4th shift',
                                        position: 'start',      // ancla al extremo derecho de la línea
                                        xAdjust: 10,          // lo mueve 10px hacia la derecha, fuera de la gráfica
                                        yAdjust: -5,          // opcional: lo mueve 5px hacia arriba
                                        backgroundColor: 'transparent',
                                        color: '#555555'
                                    }
                                },
                            }
                        },
                        // Opcional: configuración para verticalLinePlugin
                        verticalLinePlugin: {
                            lineWidth: 1,
                            lineColor: '#e91e63'
                        },
                        totalLabelPlugin: {
                            font: "bold 14px sans-serif",
                            textColor: "#555555"
                        }
                    }
                },
                plugins: [totalLabelPlugin, verticalLinePlugin]
            });

            // --- NUEVO BLOQUE: si existe fyRanges para esta línea, anotar inicio/fin de FY ---
            var range = fyRanges[lineData.LineId];
            if (range && range.MinFY && range.MaxFY) {
                chart.options.plugins.annotation.annotations['fyStart_' + lineData.LineId] = {
                    type: 'line',
                    mode: 'vertical',
                    scaleID: 'x',
                    value: range.MinFY,
                    borderColor: '#888888',      // azul corporativo
                    borderWidth: 2,
                    label: { enabled: true, content: 'FY Start', position: 'top', backgroundColor: 'rgba(128,128,128,0.5)', color: '#fff' }
                };
                chart.options.plugins.annotation.annotations['fyEnd_' + lineData.LineId] = {
                    type: 'line',
                    mode: 'vertical',
                    scaleID: 'x',
                    value: range.MaxFY,
                    borderColor: '#888888',      // naranja complementario
                    borderWidth: 2,
                    label: { enabled: true, content: 'FY End', position: 'top', backgroundColor: 'rgba(128,128,128,0.5)', color: '#fff' }
                };
                chart.update();
            }

            charts.push(chart);   // <-- lo guardamos

        });

        if (charts.length > 0) {
            // Usamos la primera gráfica para extraer las anotaciones
            buildShiftLegend(charts[0].options.plugins.annotation.annotations);
        }

        // Mostrar el mensaje solo si hay alguna gráfica generada
        if ($("#chartsContainer").children().length > 0) {
            $("#chartsNote").show();
            $("#shiftLegend").show();
        } else {
            $("#chartsNote").hide();
            $("#shiftLegend").hide();

        }
    }

    function handleDeliveryTransportTypeChange() {
        const otherContainer = $("#Delivery_Transport_Type_Other_container");
        const otherInput = $("#Delivery_Transport_Type_Other");
        const selectedValue = $("#ID_Delivery_Transport_Type").val();

        // ID 5 es "Other" en la tabla CTZ_Transport_Types
        if (selectedValue == "5") {
            otherContainer.slideDown();
        } else {
            otherContainer.slideUp();
            otherInput.val(''); // Limpiar el valor al ocultar
            validateMaterial("Delivery_Transport_Type_Other"); // Quitar error si lo había
        }
    }

    function handleInterplantDeliveryTransportTypeChange() {
        const otherContainer = $("#InterplantDelivery_Transport_Type_Other_container");
        const otherInput = $("#InterplantDelivery_Transport_Type_Other");
        const selectedValue = $("#ID_InterplantDelivery_Transport_Type").val();

        // 5 es el ID para "Other"
        if (selectedValue == "5") {
            otherContainer.slideDown();
        } else {
            otherContainer.slideUp();
            otherInput.val('');
            validateInterplantDeliveryTransportTypeOther(); // Re-validar para limpiar error
        }
    }

    function handleInterplantPackagingStandardChange() {
        const standard = $('#InterplantPackagingStandard').val();
        const rackContainer = $('#InterplantRequiresRackManufacturing_container');
        const dieContainer = $('#InterplantRequiresDieManufacturing_container');
        const rackInput = rackContainer.find('input[type="checkbox"]');
        const dieInput = dieContainer.find('input[type="checkbox"]');

        if (standard === 'OWN') {
            // Usamos slideDown() y aseguramos que 'd-flex' esté presente
            rackContainer.addClass('d-flex').slideDown();
            dieContainer.addClass('d-flex').slideDown();
            // Habilitamos los inputs solo si el usuario tiene permiso
            rackInput.prop('disabled', !canEditSales);
            dieInput.prop('disabled', !canEditSales);
        } else {
            // Ocultamos y limpiamos los checkboxes
            rackContainer.slideUp(function () { $(this).removeClass('d-flex'); });
            dieContainer.slideUp(function () { $(this).removeClass('d-flex'); });
            // Deshabilitamos y desmarcamos
            rackInput.prop('checked', false).prop('disabled', true);
            dieInput.prop('checked', false).prop('disabled', true);
        }
    }

    function buildShiftLegend(annotations) {
        var legend = document.getElementById('shiftLegend');
        legend.innerHTML = '';

        // Contador de turnos
        Object.values(annotations).forEach(function (a) {
            // Solo procesamos las anotaciones con borderDash definido
            if (Array.isArray(a.borderDash)) {
                // Construimos el atributo dash-array si existe
                var dashArray = a.borderDash.length
                    ? ' stroke-dasharray="' + a.borderDash.join(',') + '"'
                    : '';

                // Creamos un SVG con una línea horizontal
                var svg =
                    '<svg width="30" height="6" ' +
                    'style="vertical-align:middle;">' +
                    '<line x1="0" y1="3" x2="30" y2="3" ' +
                    'stroke="' + a.borderColor + '" ' +
                    'stroke-width="2"' +
                    dashArray +
                    '/>' +
                    '</svg>';

                // Obtenemos el texto de la etiqueta (si lo tenías guardado en label.content)
                var text = (a.label && a.label.content)
                    ? a.label.content
                    : '';

                // Añadimos al contenedor
                var item = document.createElement('span');
                item.className = 'legend-item';
                item.innerHTML = svg + ' ' + text;
                legend.appendChild(item);
            }
        });
    }

    // Función para mostrar/ocultar el campo "Other"
    function handleArrivalProtectiveMaterialChange() {
        console.log('entra handleArrivalProtectiveMaterialChange')
        const otherContainer = $("#Arrival_Protective_Material_Other_container");
        const otherInput = $("#Arrival_Protective_Material_Other");

        // El ID para "Other" en la tabla CTZ_Packaging_Additionals es 6
        if ($("#ID_Arrival_Protective_Material").val() == "6") {
            otherContainer.slideDown();
        } else {
            otherContainer.slideUp();
            otherInput.val(''); // Limpiar el valor al ocultar
        }
    }

    // Función para mostrar/ocultar "Stackable Levels"
    function handleStackableChange() {
        const levelsContainer = $("#Stackable_Levels_container");
        const levelsInput = $("#Stackable_Levels");

        if ($("#Is_Stackable").is(":checked")) {
            levelsContainer.slideDown();
        } else {
            levelsContainer.slideUp();
            levelsInput.val(''); // Limpiar el valor al ocultar
            validateMaterial("Stackable_Levels"); // Revalidar para quitar el error si lo había
        }
    }
    // Función para mostrar/ocultar el campo "Other" para Arrival Transport Type
    function handleArrivalTransportTypeChange() {
        const otherContainer = $("#Arrival_Transport_Type_Other_container");
        const otherInput = $("#Arrival_Transport_Type_Other");

        // 5 es el ID para "Other" en la tabla CTZ_Transport_Types
        if ($("#ID_Arrival_Transport_Type").val() == "5") {
            otherContainer.slideDown();
        } else {
            otherContainer.slideUp();
            otherInput.val(''); // Limpiar el valor al ocultar
        }
    }

    /**
   * Calcula el Tonelaje Anual (Annual Tonnage) basado en el Volumen Anual y el Peso Bruto.
   * Formula: (Annual_Volume * Gross_Weight) / 1000
   */
    function updateAnnualTonnage() {
        const targetInput = $("#AnnualTonnage");

        targetInput.val("Pending");
        //const annualVolume = parseFloat($("#Annual_Volume").val());
        //const grossWeight = parseFloat($("#Gross_Weight").val());

        //if (!isNaN(annualVolume) && !isNaN(grossWeight)) {
        //    // (Units/Year * KG/Unit) / (1000 KG/Ton) = Tons/Year
        //    const tonnage = (annualVolume * grossWeight) / 1000.0;
        //    targetInput.val(tonnage.toFixed(3));
        //} else {
        //    targetInput.val("");
        //}
    }

    function updateReturnableFieldsVisibility() {
        // Mensaje 1: Indica que la función ha comenzado a ejecutarse.
        console.log("%c--- updateReturnableFieldsVisibility [START] ---", "color: #28a745; font-weight: bold;");

        const $returnableContainer = $("#IsReturnableRack_container");
        const $returnableCheckbox = $("#IsReturnableRack");

        // 1. Obtenemos los IDs de los checkboxes de Rack Type que están marcados.
        const selectedRackIds = $('.rack-type-checkbox:checked').map(function () {
            return parseInt($(this).val(), 10);
        }).get();

        // Mensaje 2: Muestra qué IDs de racks están actualmente seleccionados.
        console.log("Rack IDs seleccionados:", selectedRackIds);
        console.log("Racks que permiten 'Returnable':", showReturnableOptionFor);

        // 2. Comprobamos si alguno de los checkboxes seleccionados está en nuestra lista de permitidos.
        const shouldShowReturnableOption = selectedRackIds.some(id => showReturnableOptionFor.includes(id));

        // Mensaje 3: Muestra el resultado booleano de la comprobación.
        console.log("¿Debería mostrarse la opción 'Returnable'?", shouldShowReturnableOption);

        // 3. Mostramos u ocultamos el campo "Returnable Rack" según corresponda.
        if (shouldShowReturnableOption) {
            // Mensaje 4a: Se ejecuta si la condición para mostrar es verdadera.
            console.log("Resultado: MOSTRANDO el contenedor 'Returnable Rack'.");
            $returnableContainer.slideDown();
        } else {
            // Mensaje 4b: Se ejecuta si la condición para mostrar es falsa.
            console.log("Resultado: OCULTANDO el contenedor 'Returnable Rack' y reseteando sus campos.");
            $returnableContainer.slideUp();
            $returnableCheckbox.prop('checked', false).trigger('change'); // Disparar 'change' para ocultar el campo "Usos"
        }

        console.log("%c--- updateReturnableFieldsVisibility [END] ---", "color: #dc3545;");
    }

    /**
     * Calcula el Peso del Paquete (Package Weight) basado en Piezas, Pilas y Peso Bruto.
     * Formula: (PiecesPerPackage * StacksPerPackage * Gross_Weight)
     */
    function updatePackageWeight() {
        const pieces = parseFloat($("#PiecesPerPackage").val());
        const stacks = parseFloat($("#StacksPerPackage").val());

        // --- INICIO LÓGICA DE FALLBACK ---
        const clientNetWeight = parseFloat($("#ClientNetWeight").val());
        const grossWeight = parseFloat($("#Gross_Weight").val());

        // Usar ClientNetWeight si es un número válido y mayor que 0, si no, usar Gross_Weight
        const weightToUse = (!isNaN(clientNetWeight) && clientNetWeight > 0) ? clientNetWeight : grossWeight;
        // --- FIN LÓGICA DE FALLBACK ---

        const targetInput = $("#PackageWeight");

        if (!isNaN(pieces) && !isNaN(stacks) && !isNaN(weightToUse) && pieces > 0 && stacks > 0 && weightToUse > 0) {
            // (Pieces/Stack * Stacks/Package * KG/Piece) = KG/Package
            const weight = (pieces * stacks * weightToUse); // <-- Usa la variable weightToUse
            targetInput.val(weight.toFixed(3));
        } else {
            targetInput.val("");
        }
    }

    function updateCalculatedWeightFields() {
        console.log("Recalculando campos de peso y tonelaje...");

        // 1. Obtener valores de los inputs
        const routeId = parseInt($('#ID_Route').val());
        const annualVolume = parseFloat($('#Annual_Volume').val()) || 0;
        const volumePerYear = parseFloat($('#Volume_Per_year').val()) || 0;
        const theoreticalGrossWeight = parseFloat($('#Theoretical_Gross_Weight').val()) || 0;
        const grossWeight = parseFloat($('#Gross_Weight').val()) || 0;
        const clientNetWeight = parseFloat($('#ClientNetWeight').val()) || 0;
        const partsPerVehicle = parseFloat($('#Parts_Per_Vehicle').val()) || 0;

        // Obtenemos el TEXTO del material seleccionado, lo limpiamos y lo pasamos a mayúsculas.
        const materialTypeText = $('#ID_Material_type option:selected').text().trim().toUpperCase();
        let scrapPercent = 0.015; // Por defecto es 1.5% (No Expuesto)

        // Si el texto empieza con "E " (E seguido de un espacio), es Expuesto.
        if (materialTypeText.startsWith("E ")) {
            scrapPercent = 0.025; // 2.5%
            console.log("Material Expuesto detectado. Usando scrap del 2.5%");
        } else {
            console.log("Material No Expuesto (o no definido). Usando scrap del 1.5%");
        }

        // 2. Definir variables para los resultados
        let weightPerPart = 0;
        let initialWeight = 0;
        let initialWeightPerPart = 0;
        let annualTonnage = 0;
        let shippingTons = 0;

        // 3. Realizar los cálculos
        if (annualVolume > 0 && volumePerYear > 0) {
            weightPerPart = (volumePerYear / annualVolume) * 1000;
            initialWeight = weightPerPart * (1 + scrapPercent);
        }

        // CORRECCIÓN: Se invierte el orden para dar prioridad a 'blanking'.
        if (routeCalculationConfig.blanking.includes(routeId)) {
            // --- CÁLCULO PARA BLANKING Y SUS VARIANTES ---
            // Este bloque se ejecutará para cualquier ruta que incluya BLK.
            console.log("Ruta de Blanking detectada. Aplicando cálculo complejo.");

            if (theoreticalGrossWeight > 0) {
                initialWeightPerPart = (initialWeight / theoreticalGrossWeight) * grossWeight;
            }
            if (initialWeightPerPart > 0) {
                annualTonnage = (initialWeightPerPart * annualVolume) / 1000 * partsPerVehicle;
                // Usamos ClientNetWeight si existe, si no, GrossWeight como fallback
                const netWeight = clientNetWeight > 0 ? clientNetWeight : grossWeight;
                shippingTons = netWeight * annualVolume * partsPerVehicle / 1000;
            }
        }
        else if (routeCalculationConfig.coil.includes(routeId)) {
            // --- CÁLCULO PARA COIL / SLITTER / WAREHOUSE ---
            // Este bloque solo se ejecutará si la ruta NO es de blanking.
            console.log("Ruta de Coil/Slitter detectada. Aplicando cálculo simple.");

            if (initialWeight > 0) {
                annualTonnage = (initialWeight * annualVolume) / 1000;
                shippingTons = initialWeight * annualVolume / 1000; // La fórmula era Peso Inicial * Autos
            }
        }

        // 4. Actualizar los campos del formulario
        $('#WeightPerPart').val(weightPerPart > 0 ? weightPerPart.toFixed(3) : "");
        $('#Initial_Weight').val(initialWeight > 0 ? initialWeight.toFixed(3) : "");
        $('#InitialWeightPerPart').val(initialWeightPerPart > 0 ? initialWeightPerPart.toFixed(3) : "");
        $('#AnnualTonnage').val(annualTonnage > 0 ? annualTonnage.toFixed(3) : "");
        $('#ShippingTons').val(shippingTons > 0 ? shippingTons.toFixed(3) : "");
    }

    /**
     * Busca y aplica el OEE para una línea de producción específica,
     * solo si el campo OEE está vacío.
     * param {any} lineId El ID de la línea de producción.
     */
    function fetchAndApplyOee(lineId) {
        const oeeInput = $("#OEE");
        const oeeNoteSpan = $("#oeeCalculationNote");

        // Ya no comprobamos si el input tiene valor aquí. La llamada AJAX se hará siempre que haya una línea.

        if (!lineId || lineId === "0" || lineId === "") {
            oeeNoteSpan.hide().text('');
            // No limpiamos el input aquí para no borrar la entrada del usuario si cambia de línea y no hay cálculo.
            return;
        }

        $.getJSON(config.urls.getOeeForLine, { lineId: lineId })
            .done(function (response) {

                if (response.success && response.isCalculated) {

                    if (response.foundRecords) {
                        // CASO 1: Se encontraron registros.
                        // Siempre mostramos la nota con el valor calculado para informar al usuario.
                        oeeNoteSpan.text(`* Calculated average is ${response.oeeValue.toFixed(2)}% from the last 6 months.`);
                        oeeNoteSpan.show();

                        // SOLO si el campo de texto está vacío, lo rellenamos y lo resaltamos.
                        if (!oeeInput.val() || oeeInput.val().trim() === "") {
                            oeeInput.val(response.oeeValue.toFixed(2));
                            oeeInput.css('background-color', '#e9f5ff');
                        }
                    } else {
                        // CASO 2: No se encontraron registros.
                        // Siempre mostramos la nota de aviso.
                        oeeNoteSpan.text('* No OEE records found for the last 6 months.');
                        oeeNoteSpan.show();
                        // Nos aseguramos de que no esté resaltado si no se pudo calcular un valor.
                        oeeInput.css('background-color', '');
                    }
                } else {
                    // Si la respuesta no fue exitosa o no fue un cálculo, limpiamos la nota.
                    oeeNoteSpan.hide().text('');
                    oeeInput.css('background-color', '');
                }
            })
            .fail(function () {
                toastr.error("Could not retrieve OEE value for the selected line.");
            });
    }

    /**
      * Esta función controla la visibilidad del combo "Delivery Transport Type".
      * Solo se debe mostrar si el campo "Freight Type" está configurado como 'DDP'.
      */
    function updateDeliveryTransportationTypeState() {

        // 1. Obtener el valor del combo "Freight Type"
        const freightTypeSelect = $('#ID_FreightType');
        const freightType = freightTypeSelect.val();

        // 2. Obtener los elementos de "Delivery Transport Type"
        const deliveryTransportContainer = $('#ID_Delivery_Transport_Type_container');
        const deliveryTransportSelect = $('#ID_Delivery_Transport_Type');

        // 3. ID de "DDP" (Según el script SQL, DDP es el ID 2)
        const ddpFreightTypeId = "2";

        // Verifica que los selectores de JQuery hayan encontrado los elementos
        if (deliveryTransportContainer.length === 0) {
            console.error("ERROR: No se encontró el CONTENEDOR '#ID_Delivery_Transport_Type_container'");
        } else {
            console.log("Contenedor a mostrar/ocultar:", deliveryTransportContainer.attr('id'));
        }
        if (deliveryTransportSelect.length === 0) {
            console.error("ERROR: No se encontró el SELECT '#ID_Delivery_Transport_Type'");
        }
        // --- FIN DEBUG ---

        // 4. Lógica de visibilidad
        if (freightType === ddpFreightTypeId) {

            deliveryTransportContainer.slideDown(); // O .show()

            if (config.permissions.canEditSales) {
                deliveryTransportSelect.prop('disabled', false);
            }
        } else {
            deliveryTransportContainer.slideUp();
            deliveryTransportSelect.val('').trigger('change');
            deliveryTransportSelect.prop('disabled', true);
        }
    }

    function handleReturnableRackChange() {
        const usesContainer = $('#ReturnableUses_container');
        const usesInput = $('#ReturnableUses');

        if ($('#IsReturnableRack').is(':checked')) {
            usesContainer.slideDown();
        } else {
            usesContainer.slideUp();
            usesInput.val(''); // Limpia el valor al ocultar
            validateReturnableUses(); // Vuelve a validar para limpiar cualquier mensaje de error
        }
    }

    function updateFileUI() {
        // 1. Lee los datos del archivo CAD desde los inputs ocultos del formulario.
        let fileId = $("#ID_File_CAD_Drawing").val();
        let fileName = $("#CADFileName").val();

        // 2. Comprueba si existe un archivo asociado a este material.
        if (fileId && fileName && fileId.trim() !== "" && fileName.trim() !== "") {
            // --- CASO A: SÍ hay un archivo ---

            $("#cat_file_container").hide();
            $("#fileActions_container").show();

            // Configura el enlace de descarga
            $("#downloadFile")
                .attr("href", "/CTZ_Projects/DownloadFile?fileId=" + fileId)
                .attr("title", fileName)
                .html('<i class="fa-solid fa-download"></i> ' + fileName);

            $("#archivo").prop("disabled", true);

        } else {

            $("#cat_file_container").show();
            $("#fileActions_container").hide();

            if (canEditSales) {
                $("#archivo").prop("disabled", false);
            } else {
                $("#archivo").prop("disabled", true);
            }
        }
    }

    function updatePackagingFileUI() {
        let fileId = $("#ID_File_Packaging").val();
        let fileName = $("#PackagingFileName").val();


        if (fileId && fileName) {
            $("#packaging_file_container").hide();
            $("#packagingFileActions_container").show();
            $("#downloadPackagingFile")
                .attr("href", "/CTZ_Projects/DownloadFile?fileId=" + fileId)
                .attr("title", fileName) // <-- AÑADE ESTA LÍNEA
                .html('<i class="fa-solid fa-download"></i> ' + fileName);
        } else {
            $("#packaging_file_container").show();
            $("#packagingFileActions_container").hide();
        }
    }

    function updateFileUIGeneric(fileIdText, fileNameText, fileContainerText, fileActionContainer, downloadContainer) {

        // --- INICIO DE DEPURACIÓN ---
        console.groupCollapsed(`--- DEPURANDO: ${fileIdText} ---`);
        console.log(`Parámetros recibidos:
        ID Field (Hidden): ${fileIdText}
        Name Field (Hidden): ${fileNameText}
        File Container (Input): ${fileContainerText}
        Action Container (Download): ${fileActionContainer}`);
        // --- FIN DE DEPURACIÓN ---

        let fileId = $("#" + fileIdText).val();
        let fileName = $("#" + fileNameText).val();

        // --- INICIO DE DEPURACIÓN ---
        console.log(`Valor de ID_File (${fileIdText}): "${fileId}"`);
        console.log(`Valor de FileName (${fileNameText}): "${fileName}"`);

        const hasFile = fileId && fileName && fileId.trim() !== "" && fileName.trim() !== "";
        console.log(`Condición de existencia (fileId && fileName): ${hasFile}`);
        // --- FIN DE DEPURACIÓN ---

        if (hasFile) {
            // --- CASO 1: Archivo existe en BD (se muestra el enlace) ---
            $("#" + fileContainerText).hide();
            $("#" + fileActionContainer).show();

            // Configuramos el enlace de descarga
            $("#" + downloadContainer)
                .attr("href", "/CTZ_Projects/DownloadFile?fileId=" + fileId)
                .attr("title", fileName)
                .html('<i class="fa-solid fa-download"></i> ' + fileName);

            // --- INICIO DE DEPURACIÓN ---
            console.log(`✅ Resultado: Mostrando enlace de descarga en #${fileActionContainer}.`);
            // --- FIN DE DEPURACIÓN ---

        } else {
            // --- CASO 2: Archivo NO existe (se muestra el input de carga) ---
            $("#" + fileContainerText).show();
            $("#" + fileActionContainer).hide();

            // --- INICIO DE DEPURACIÓN ---
            console.log(`❌ Resultado: Mostrando input de archivo en #${fileContainerText}.`);
            // --- FIN DE DEPURACIÓN ---
        }

        console.groupEnd(); // Cierra el grupo de depuración
    }

    function updateRealStrokes() {

        var productionLineId = $("#ID_Real_Blanking_Line option:selected").val();

        // Suponiendo que tienes los valores en los campos:
        var pitch = parseFloat($("#Pitch").val());
        var rotation = 0; // 0 de momento


        if (isNaN(pitch) || isNaN(rotation) || productionLineId == "") {
            // Manejar error: se requieren ambos valores.
            $("#Real_Strokes").val("");
            $("#Real_Strokes").trigger("change");

            return;
        }

        $.ajax({
            url: config.urls.getTheoreticalStrokes,
            type: 'GET',
            data: {
                productionLineId: productionLineId,
                pitch: pitch,
                rotation: rotation
            },
            success: function (response) {
                if (response.success) {
                    var strokes = parseFloat(response.theoreticalStrokes);
                    var formattedStrokes = (strokes % 1 === 0) ? strokes : strokes.toFixed(2);
                    $("#Real_Strokes").val(formattedStrokes);
                    updateEffectiveStrokes();
                } else {
                    $("#Real_Strokes").val("");
                    updateEffectiveStrokes();
                    toastr.error("Error: " + response);
                }
            },
            error: function (xhr, status, error) {
                toastr.error("Error en Ajax: " + error);
                $("#Real_Strokes").val("");
            },
            async: false
        }).always(function () {
            $("#Real_Strokes").trigger("change");
        });


    }

    function updateStrokesPerAuto() {
        var partsVal = parseFloat($("#Parts_Per_Vehicle").val());
        var blanksVal = parseFloat($("#Blanks_Per_Stroke").val());

        // Si alguno es inválido o la división no es posible, limpia el campo
        if (isNaN(partsVal) || isNaN(blanksVal) || blanksVal === 0) {
            $("#strokes_auto").val("");
        } else {
            var result = partsVal / blanksVal;
            $("#strokes_auto").val(result.toFixed(2));
        }
    }

    function updateMinMaxReales() {
        var blanksPerYear = parseFloat($("#blanks_per_year").val());
        var idealCycleTime = parseFloat($("#Ideal_Cycle_Time_Per_Tool").val());
        var blanksPerStroke = parseFloat($("#Blanks_Per_Stroke").val());

        // Validamos que todos los valores sean números y que idealCycleTime y blanksPerStroke no sean 0
        if (isNaN(blanksPerYear) || isNaN(idealCycleTime) || isNaN(blanksPerStroke) || idealCycleTime === 0 || blanksPerStroke === 0) {
            $("#min_max_reales").val("");
        } else {
            var result = blanksPerYear / idealCycleTime / blanksPerStroke;
            $("#min_max_reales").val(result.toFixed(2));
        }

        //tambien actualiza la version de OEE
        updateMinMaxRealesOEE();
    }

    function updateBlanksPerYear() {
        var partsVal = parseFloat($("#Parts_Per_Vehicle").val());
        var annualVolume = parseFloat($("#Annual_Volume").val());

        // Verificar si alguno es inválido o nulo
        if (isNaN(partsVal) || isNaN(annualVolume)) {
            $("#blanks_per_year").val("");
        } else {
            var result = partsVal * annualVolume;
            $("#blanks_per_year").val(result.toFixed(2));
        }

        //valida los campos que pudieran depender de este input
        updateMinMaxReales();
        updateStrokesShift();
    }

    function updateMinMaxRealesOEE() {
        var minMaxReales = parseFloat($("#min_max_reales").val());
        var oee = parseFloat($("#OEE").val());

        if (isNaN(minMaxReales) || isNaN(oee) || oee === 0) {
            $("#min_max_reales_oee").val("");
        } else {
            // Normalizamos el OEE de porcentaje a valor decimal
            var oeeDecimal = oee / 100;
            var result = minMaxReales / oeeDecimal;
            $("#min_max_reales_oee").val(result.toFixed(2));
        }

        // Actualiza los turnos reales
        updateActualShifts();
    }


    function updateActualShifts() {
        var minMaxRealesOEE = parseFloat($("#min_max_reales_oee").val());

        // Verifica si el valor es válido (7.5 y 60 son constantes, por lo que no se espera división por 0)
        if (isNaN(minMaxRealesOEE)) {
            $("#actual_shifts").val("");
        } else {
            var result = minMaxRealesOEE / 7.5 / 60;
            $("#actual_shifts").val(result.toFixed(2));
        }

        updateStrokesShift();
    }

    function updateStrokesShift() {
        var blanksPerYear = parseFloat($("#blanks_per_year").val());
        var blanksPerStroke = parseFloat($("#Blanks_Per_Stroke").val());
        var actualShifts = parseFloat($("#actual_shifts").val());

        if (isNaN(blanksPerYear) || isNaN(blanksPerStroke) || isNaN(actualShifts) || blanksPerStroke === 0 || actualShifts === 0) {
            $("#strokes_shift").val("");
        } else {
            var result = blanksPerYear / blanksPerStroke / actualShifts;
            $("#strokes_shift").val(result.toFixed(0));
        }
    }

    /**
* Actualiza el texto de ayuda (<small>) debajo de los inputs de ingeniería
* y el límite de mults, usando las variables globales.
*/
    function updateLimitsDisplay() {
        console.log("--- updateLimitsDisplay ---");

        const rangesToUse = window.engineeringRanges;

        // Mapeo de los campos que queremos actualizar en la UI.
        const limits = [
            { displayId: "#tensil-limit-display", criterioId: 14 },     // Tensile Strength
            { displayId: "#thickness-limit-display", criterioId: 7 },    // Thickness
            { displayId: "#width-limit-display", criterioId: 8 },        // Width
            { displayId: "#pitch-limit-display", criterioId: 9 }         // Pitch
        ];

        // 1. Limpiamos todos los displays antes de empezar.
        limits.forEach(({ displayId }) => {
            $(displayId).text("");
        });
        // Limpiamos también el display de mults
        $("#mults-limit-display").text("");

        // 2. Si tenemos un array de rangos para mostrar, los procesamos.
        if (rangesToUse && Array.isArray(rangesToUse)) {
            limits.forEach(({ displayId, criterioId }) => {
                const range = rangesToUse.find(r => r.ID_Criteria === criterioId);
                const limitText = range ? describirLimites(range) : "No limits defined for this selection.";
                $(displayId).text(limitText);
            });
        } else {
            console.log("No hay rangos de dimensiones para mostrar.");
        }

        // 3. Actualizamos el display para el máximo de mults permitido
        if (typeof window.maxMultsAllowed === 'number') {
            $("#mults-limit-display").text(`Max. allowed: ${window.maxMultsAllowed} strips`);
        }
    }

    function updateVehicleVisibility() {
        // CAMBIO: Mensaje de depuración inicial
        console.log("--- Ejecutando updateVehicleVisibility ---");

        // Si Vehicle 1 no tiene valor, ocultar las filas de los demás vehículos.
        if (!$("#Vehicle").val()) {
            // CAMBIO: Se usan los IDs de las filas (.row) y se incluye la fila 4.
            $(".vehicle-row-2, .vehicle-row-3").hide();
            console.log("Valor de Vehículo 1: VACÍO. Ocultando filas 2, 3 y 4.");
        } else {
            // Si Vehicle 1 tiene valor, mostrar la fila de Vehicle 2.
            $(".vehicle-row-2").show();
            console.log("Valor de Vehículo 1: PRESENTE. Mostrando Fila 2.");

            if (!$("#Vehicle_2").val()) {
                // Si Vehicle 2 no tiene valor, ocultar las filas 3 y 4.
                $(".vehicle-row-3").hide();
                console.log(" -> Valor de Vehículo 2: VACÍO. Ocultando filas 3 y 4.");
            } else {
                // Si Vehicle 2 tiene valor, mostrar la fila de Vehicle 3.
                $(".vehicle-row-3").show();
                console.log(" -> Valor de Vehículo 2: PRESENTE. Mostrando Fila 3.");

            }
        }
        console.log("--- Fin de updateVehicleVisibility ---");
    }

    // Función que actualiza el fondo del input Annual_Volume según el porcentaje de diferencia
    function updateAnnualVolumeStyle() {
        // Obtenemos los valores
        let annualVolumeStr = $("#Annual_Volume").val().trim();
        let maxProductionStr = $("#Max_Production_SP").val().trim();

        // Si alguno no tiene valor, salimos
        if (!annualVolumeStr || !maxProductionStr) {
            // Podrías resetear el fondo a blanco
            $("#Annual_Volume").css("background-color", "");
            return;
        }

        // Convertir a número
        let annualVolume = parseFloat(annualVolumeStr);
        let maxProduction = parseFloat(maxProductionStr);

        // Evitar división por cero
        if (maxProduction === 0) {
            $("#Annual_Volume").css("background-color", "");
            return;
        }

        // Calcular el ratio
        let ratio = annualVolume / maxProduction;
        // Calcular diferencia porcentual respecto a maxProduction
        let diffPercentage = ((annualVolume - maxProduction) / maxProduction) * 100;

        // Actualizar el fondo según el rango:
        // Verde: ratio entre 0.85 y 1.15
        // Amarillo: ratio entre 0.70 y 0.85 o entre 1.15 y 1.30
        // Rojo: cualquier otro caso
        let newBgColor = "";
        if (ratio >= 0.85 && ratio <= 1.15) {
            newBgColor = "lightgreen";
        } else if ((ratio >= 0.70 && ratio < 0.85) || (ratio > 1.15 && ratio <= 1.30)) {
            newBgColor = "lightyellow";
        } else {
            newBgColor = "lightcoral";
        }

        $("#Annual_Volume").css("background-color", newBgColor);

        // Devuelve la diferencia porcentual para usar en el toast
        return diffPercentage;
    }

    // Función para mostrar el toast con la diferencia porcentual
    function showVolumeDifferenceToast(diffPercentage) {
        // Redondear a dos decimales
        let diffRounded = diffPercentage.toFixed(2);
        // Generar el mensaje: puede ser "x% above" o "x% below"
        let message = "";
        if (diffPercentage > 0) {
            message = `${diffRounded}% above Max Production`;
        } else if (diffPercentage < 0) {
            message = `${Math.abs(diffRounded)}% below Max Production`;
        } else {
            message = "No difference with Max Production";
        }

        // Mostrar el toast
        toastr.info(message);
    }

    function toggleRunningChangeWarning() {
        const isChecked = $("#isRunningChange").is(":checked");
        const warningSpan = $("#runningChangeWarning");

        // Nuevo: Leemos el valor del campo Client Net Weight
        const netWeightValue = $("#ClientNetWeight").val();

        // La advertencia ahora solo se muestra si el checkbox está marcado Y el campo de peso está vacío.
        if (isChecked && (!netWeightValue || netWeightValue.trim() === "")) {
            warningSpan.slideDown();
        } else {
            warningSpan.slideUp();
        }
    }

    function updateWeightFieldsBasedOnShape() {
        const shapeId = $("#ID_Shape").val();
        const grossWeightInput = $("#Gross_Weight");
        const netWeightInput = $("#ClientNetWeight");

        // IDs para Rectangular (2) y Trapecio (3)
        if (shapeId === "2" || shapeId === "3") {
            // Para Rectangular y Trapecio, el Neto es igual al Bruto.
            netWeightInput.prop('readonly', true); // Hacemos el campo de solo lectura
            netWeightInput.val(grossWeightInput.val()); // Copiamos el valor inmediatamente
        } else {
            // Para cualquier otra forma (incluyendo "Configurado"), ambos son editables.
            // Solo quitamos 'readonly' si el usuario tiene permiso para editar.
            if (canEditSales) {
                netWeightInput.prop('readonly', false);
            }
        }

        // Re-validamos ambos campos para asegurar consistencia
        validateGrossWeight();
        validateClientNetWeight();
    }

    function UpdateCapacityHansontable(OnlyBDMaterials = false) {
        if (window.isBatchValidating) {
            console.log("UpdateCapacityHansontable: Bloqueado por validación masiva.");
            return;
        }

        // --- LÓGICA DE VISIBILIDAD ---
        if (!OnlyBDMaterials) {
            const activeRoutes = getEffectiveProjectRoutes();
            // La tabla de capacidad se usa principalmente para Blanking
            const hasBlankingRoute = activeRoutes.some(r => blankingRouteIds.includes(r));

            if (!hasBlankingRoute) {
                console.log("UpdateCapacityHansontable: Sin rutas Blanking. Limpiando tabla.");
                $("#capacityTableContainer").html(""); // Limpiar contenido
                // Opcional: Ocultar contenedor padre si deseas
                return;
            }
        }

        console.log('entra UpdateCapacityHansontable');

        let materialId = $("#materialId").val();

        // obtiene el valor de la linea teorica
        let theoricalBLKID = $("#ID_Theoretical_Blanking_Line").val();

        // Obtiene los ids que se enviaran
        var productionLineId = $("#ID_Real_Blanking_Line option:selected").val();

        //si la linea real es igual a vacio 0, se utiliza la linea teorica
        if (productionLineId == "" || productionLineId == 0) {
            productionLineId = theoricalBLKID;
            console.log('Usando linea teorica para calculos HT')
        }

        var vehicle = $("#Vehicle").val();
        var partsPerVehicle = parseFloat($("#Parts_Per_Vehicle").val()) || null;
        var idealCycleTimePerTool = parseFloat($("#Ideal_Cycle_Time_Per_Tool").val()) || null;
        var blanksPerStroke = parseFloat($("#Blanks_Per_Stroke").val()) || null;
        var oee = parseFloat($("#OEE").val()) || null;
        var realSOP = $("#Real_SOP").val();
        var realEOP = $("#Real_EOP").val();
        var annualVol = parseInt($("#Annual_Volume").val()) || null;

        // Campos de fallback para idealCycleTimePerTool
        var realStrokes = parseFloat($("#Real_Strokes").val()) || null;
        var theoreticalStrokes = parseFloat($("#Theoretical_Strokes").val()) || null;

        // Limpiar el contenedor de gráficos
        $("#capacityTableContainer").html("<p style='color:gray;'>Loading <i class='fa-solid fa-spinner fa-spin-pulse'></i></p>");

        // Validar que existan los campos obligatorios
        var missingFields = [];
        if (!vehicle || vehicle.trim() === "") {
            missingFields.push("Vehicle");
        }
        if (partsPerVehicle === null) {
            missingFields.push("Parts per Vehicle");
        }
        if (blanksPerStroke === null) {
            missingFields.push("Blanks per Stroke");
        }
        if (!realSOP || realSOP.trim() === "") {
            missingFields.push("Real SOP");
        }
        if (!realEOP || realEOP.trim() === "") {
            missingFields.push("Real EOP");
        }
        if (annualVol === null) {
            missingFields.push("Annual Volume");
        }

        // Fallback para idealCycleTimePerTool
        if (idealCycleTimePerTool === null) {
            if (realStrokes !== null) {
                idealCycleTimePerTool = realStrokes;
            } else if (theoreticalStrokes !== null) {
                idealCycleTimePerTool = theoreticalStrokes;
            } else {
                missingFields.push("Ideal Cycle Time (or Real/Theoretical Strokes)");
            }
        }

        // Si faltan campos, no se ejecuta la llamada AJAX; se limpia el contenedor y se muestra un mensaje
        if (missingFields.length > 0 && !OnlyBDMaterials) {
            $("#capacityTableContainer").html(
                "<p style='color:red;'>Missing required fields: " + missingFields.join(", ") + "</p>"
            );
            return;
        }

        // Llamada AJAX para obtener el "what‑if" de capacidad para el material seleccionado.
        // Se envían además projectId, materialId y blkID (que es la línea real consultada)
        $.ajax({
            url: config.urls.GetMaterialCapacityScenarios,
            type: 'GET',
            data: {
                projectId: config.project.id,
                materialId: materialId, // Puede ser null
                blkID: productionLineId,
                vehicle: vehicle,
                partsPerVehicle: partsPerVehicle,
                idealCycleTimePerTool: idealCycleTimePerTool,
                blanksPerStroke: blanksPerStroke,
                oee: oee,
                realSOP: realSOP,
                realEOP: realEOP,
                annualVol: annualVol,
                partNumber: $("#partNumber").val() || null, 
                OnlyBDMaterials: OnlyBDMaterials
            },
            success: function (response) {
                var container = document.getElementById("capacityTableContainer");
                container.innerHTML = ""; // Limpiar contenedor
                if (response.success) {


                    // 1. Actualizamos el input status_dm con el valor calculado en el servidor
                    if (OnlyBDMaterials == false) {
                        $("#status_dm").val(response.status_dm);              
                    }

                    //$("#DM_status_comment").html(response.status_dm_comment);
                    // 2. Convertir la data (array de objetos) a una matriz para Handsontable
                    var arrayData = convertToHandsontableMatrix(response.data);
                    // arrayData.rows es una matriz donde:
                    //   columna 0: LineId (oculta)
                    //   columna 1: Line (nombre)
                    //   columnas siguientes: valores de los FY

                    // 3. Inicializar Handsontable
                    new Handsontable(container, {
                        data: arrayData.rows,
                        readOnly: true,
                        colHeaders: arrayData.headers,
                        rowHeaders: true,
                        // Ocultamos la primera columna (LineId)
                        hiddenColumns: {
                            columns: [0],
                        },
                        //colWidths: [0, 150, 250], // Agregamos un ancho para la nueva columna PartNumbers (ej. 250px)
                        // Hook para modificar los encabezados de columna.
                        // Si el encabezado (por ejemplo, "FY 22/23") se encuentra en response.highlightedFY, se le aplica fondo dorado.
                        afterGetColHeader: function (col, TH) {
                            // Las columnas FY comienzan en el índice 2 (0: LineId, 1: Line)
                            if (col >= 3) {
                                var headerText = this.getColHeader(col);
                                if (response.highlightedFY.indexOf(headerText) !== -1) {
                                    TH.style.backgroundColor = "#FFD700"; // Dorado
                                } else {
                                    TH.style.backgroundColor = "#009ff5"; // Valor por defecto
                                    TH.style.color = "#ffffff";           // Texto en blanco
                                }
                            } else {
                                TH.style.backgroundColor = "#009ff5"; // Valor por defecto
                                TH.style.color = "#ffffff";           // Texto en blanco
                            }
                        },
                        // Configuración de las celdas
                        cells: function (row, col, prop) {
                            var cellProperties = {};
                            cellProperties.readOnly = true;

                            cellProperties.renderer = function (instance, td, row, col, prop, value, cellProperties) {
                                // Render base de texto
                                Handsontable.renderers.TextRenderer.apply(this, arguments);

                                // Obtenemos el nombre de la columna (header) para saber qué estamos renderizando
                                var colHeader = instance.getColHeader(col);

                                // Columna 1: Nombre de la línea
                                if (colHeader === "Line") {
                                    // Estilos base si deseas
                                }
                                // Columna 2: PartNumbers
                                else if (colHeader === "PartNumbers") {
                                    td.style.whiteSpace = 'normal';
                                }
                                // NUEVO: Columna de ESTATUS
                                else if (colHeader === "Status") {
                                    td.style.fontWeight = 'bold';
                                    td.style.textAlign = 'center';

                                    if (value === "REJECTED") {
                                        td.style.color = "#dc3545"; // Rojo
                                        td.style.backgroundColor = "#ffe6e6"; // Fondo rojo claro opcional
                                    } else if (value === "ON REVIEWED") {
                                        td.style.color = "#ffc107"; // Amarillo oscuro / Naranja
                                        // td.style.backgroundColor = "#fff3cd"; 
                                    } else if (value === "APPROVED") {
                                        td.style.color = "#28a745"; // Verde
                                        // td.style.backgroundColor = "#d4edda";
                                    }
                                }
                                // Columnas de Años Fiscales (Contienen números)
                                else if (typeof value === "number") {
                                    var percentage = value * 100;
                                    td.innerHTML = percentage.toFixed(2) + "%";

                                    if (percentage < 95) {
                                        // td.style.backgroundColor = "#c1fcac"; // Verde claro (opcional)
                                    } else if (percentage < 98) {
                                        td.style.backgroundColor = "#ffd53b"; // Entre 95% y 98%
                                    } else {
                                        td.style.backgroundColor = "#ff7c4f"; // 98% o mayor
                                    }
                                }

                                return td;
                            };

                            return cellProperties;
                        },
                        licenseKey: 'non-commercial-and-evaluation'
                    });
                } else {
                    $(container).html("<p style='color:red;'>" + response.message + "</p>");
                }
            },
            error: function () {
                $("#capacityTableContainer").html("<p style='color:red;'>Error al cargar datos de capacidad.</p>");
            }
        });
    }

    function recalculateVehicleData() {
        // Supongamos que tienes una función similar a getVehicleData que, para cada combo,
        // obtiene un objeto incluyendo la propiedad productionData, la cual se extrae del atributo data-productionjson.
        function getVehicleData(selector) {
            var selectedOption = $(selector).find("option:selected");
            return {
                sop: selectedOption.data("sop"),
                eop: selectedOption.data("eop"),
                program: selectedOption.data("program"),
                maxProduction: selectedOption.data("maxproduction"),
                productionJson: selectedOption.data("productionjson")
            };
        }

        var data1 = getVehicleData("#Vehicle");
        var data2 = getVehicleData("#Vehicle_2");
        var data3 = getVehicleData("#Vehicle_3");
        var data4 = getVehicleData("#Vehicle_4");

        // Solo considerar aquellos vehículos que tienen un valor seleccionado y producción definida
        var selectedVehicles = [];
        [data1, data2, data3, data4].forEach(function (d) {
            if (d && d.sop && d.productionJson) {
                try {
                    d.productionData = JSON.parse(d.productionJson);
                } catch (e) {
                    d.productionData = [];
                }
                selectedVehicles.push(d);
            }
        });

        // Calcular el SOP (mínimo) y EOP (máximo) usando data de los vehículos seleccionados (si es necesario)
        var minSOP = null, maxEOP = null;
        selectedVehicles.forEach(function (d) {
            var sopDate = new Date(d.sop);
            var eopDate = new Date(d.eop);
            console.log("SOP: " + sopDate + " EOP: " + eopDate)
            if (!minSOP || sopDate < minSOP) minSOP = sopDate;
            if (!maxEOP || eopDate > maxEOP) maxEOP = eopDate;
        });
        if (minSOP) $("#SOP_SP").val(moment.utc(minSOP).format("YYYY-MM"));
        if (maxEOP) $("#EOP_SP").val(moment.utc(maxEOP).format("YYYY-MM"));


        // Para Program_SP: Concatenar los posibles "program" de los vehículos (data1, data2, data3, data4)
        // separando por " / " (con espacio antes y después) y evitando duplicados
        var programs = [];
        [data1, data2, data3, data4].forEach(function (d) {
            if (d && d.program && programs.indexOf(d.program) === -1) {
                programs.push(d.program);
            }
        });
        var concatenatedProgram = programs.join(" / ");
        $("#programSP").val(concatenatedProgram);

        // Calcular la suma de producción por año
        var productionByYear = {};
        selectedVehicles.forEach(function (d) {
            console.log("Vehículo:", d);
            if (d.productionJson && Array.isArray(d.productionJson)) {
                d.productionJson.forEach(function (item) {
                    console.log("Item de producción:", item);
                    var year = item.Production_Year;
                    var sum = parseFloat(item.Production_Amount) || 0;
                    if (year) {
                        if (!productionByYear[year]) {
                            productionByYear[year] = 0;
                        }
                        productionByYear[year] += sum;
                    }
                });
            }
        });

        console.log("Suma de producción por año para validación:", productionByYear);


        // Determinar el año con mayor producción, aplicando la lógica de "Carry Over"
        var maxProduction = 0, maxProductionYear = null;
        const isCarryOver = $("#isRunningChange").is(":checked");

        if (isCarryOver) {
            // --- NUEVA LÓGICA PARA CARRY OVER ---
            console.log("Calculando Max Production para Running Change.");
            const sopStr = $("#Real_SOP").val();
            const eopStr = $("#Real_EOP").val();

            // Solo filtramos si ambas fechas reales existen
            if (sopStr && eopStr) {
                const sopYear = parseInt(sopStr.substring(0, 4), 10);
                const eopYear = parseInt(eopStr.substring(0, 4), 10);
                console.log(`Filtrando producción entre los años ${sopYear} y ${eopYear}.`);

                for (var year in productionByYear) {
                    const currentYear = parseInt(year, 10);
                    // Incluir solo los años dentro del rango de Real SOP y Real EOP
                    if (currentYear >= sopYear && currentYear <= eopYear) {
                        if (productionByYear[year] > maxProduction) {
                            maxProduction = productionByYear[year];
                            maxProductionYear = year;
                        }
                    }
                }
            } else {
                console.log("Real SOP o Real EOP están vacíos. No se puede calcular Max Production para Carry Over. Se usará el máximo de toda la vida del proyecto.");
                // Si faltan fechas, se recurre a la lógica original como fallback.
                for (var year in productionByYear) {
                    if (productionByYear[year] > maxProduction) {
                        maxProduction = productionByYear[year];
                        maxProductionYear = year;
                    }
                }
            }
        } else {
            // --- LÓGICA ORIGINAL (NO RUNNING CHANGE) ---
            console.log("Calculando Max Production para la vida completa del proyecto.");
            for (var year in productionByYear) {
                if (productionByYear[year] > maxProduction) {
                    maxProduction = productionByYear[year];
                    maxProductionYear = year;
                }
            }
        }

        $("#Max_Production_SP").val(maxProduction);

        //llamamos a grafica de slitter
        debouncedUpdateSlitterChart();
    }

    /**
  * Actualiza el dropdown de "Material Type" basado en la ruta seleccionada.
  * Si la ruta incluye Slitting, filtra los materiales; si no, muestra todos los de la planta.
  */
    function updateMaterialTypeDropdown() {
        const selectedRouteId = parseInt($("#ID_Route").val(), 10);
        const slitterLineId = $("#ID_Slitting_Line").val();
        const materialTypeSelect = $("#ID_Material_type");
        const plantId = config.project.plantId;

        // --- INICIO DE LA MODIFICACIÓN ---
        // PASO 1: Guardar la selección actual ANTES de hacer cualquier cambio.
        const previouslySelectedMaterialType = materialTypeSelect.val();
        // --- FIN DE LA MODIFICACIÓN ---

        if (slittingRouteIds.includes(selectedRouteId) && slitterLineId) {

            materialTypeSelect.html('<option>Loading compatible materials...</option>').prop('disabled', true).trigger('change.select2');

            $.getJSON(config.urls.getMaterialTypesForSlitter, { plantId: plantId, slitterLineId: slitterLineId })
                .done(function (response) {
                    if (response.success) {
                        materialTypeSelect.empty().append('<option value="">Select Material Type</option>');
                        response.data.forEach(function (item) {
                            materialTypeSelect.append(new Option(item.Text, item.Value));
                        });

                        // --- INICIO DE LA MODIFICACIÓN ---
                        // PASO 2: Después de repoblar, intentar restaurar la selección previa.
                        if (previouslySelectedMaterialType && materialTypeSelect.find(`option[value="${previouslySelectedMaterialType}"]`).length > 0) {
                            materialTypeSelect.val(previouslySelectedMaterialType);
                        }
                        // --- FIN DE LA MODIFICACIÓN ---

                        toastr.info("Material Type list has been filtered for the selected Slitting Line.");
                    } else {
                        toastr.error(response.message || "Could not load compatible materials.");
                        loadAllMaterialTypesForPlant(); // Llama a la versión actualizada de esta función
                    }
                })
                .fail(function () {
                    toastr.error("Error fetching compatible materials.");
                    loadAllMaterialTypesForPlant(); // Llama a la versión actualizada de esta función
                })
                .always(function () {
                    materialTypeSelect.prop('disabled', !canEditSales).trigger('change.select2');
                });

        } else {
            // Si la ruta NO incluye Slitting, carga todos los materiales para la planta.
            loadAllMaterialTypesForPlant();
        }
    }

    /**
     * Función auxiliar para cargar TODOS los tipos de material para la planta actual,
     * preservando la selección si es posible.
     */
    function loadAllMaterialTypesForPlant() {
        const materialTypeSelect = $("#ID_Material_type");

        // --- INICIO DE LA MODIFICACIÓN ---
        // PASO 1: Guardar la selección actual.
        const previouslySelectedMaterialType = materialTypeSelect.val();
        // --- FIN DE LA MODIFICACIÓN ---

        materialTypeSelect.empty().append('<option value="">Select Material Type</option>');

        if (config.lists && config.lists.materialTypes) {

            config.lists.materialTypes.forEach(function (item) {
                // 'item' es ahora un objeto JS { Text: "...", Value: "..." }
                materialTypeSelect.append(new Option(item.Text, item.Value));
            });
        }

        // --- INICIO DE LA MODIFICACIÓN ---
        // PASO 2: Intentar restaurar la selección previa.
        if (previouslySelectedMaterialType && materialTypeSelect.find(`option[value="${previouslySelectedMaterialType}"]`).length > 0) {
            materialTypeSelect.val(previouslySelectedMaterialType);
        }
        // --- FIN DE LA MODIFICACIÓN ---

        materialTypeSelect.trigger('change.select2');
    }

    function updateTheoreticalLine() {
        console.log("--- updateTheoreticalLine ---");
        const selectedRouteId = parseInt($("#ID_Route").val(), 10);

        // Guard: If the route is not a blanking route, do nothing here.
        if (!blankingRouteIds.includes(selectedRouteId)) {
            $("#theoretical_blk_line").val("N/A for this route");
            $("#ID_Theoretical_Blanking_Line, #Theoretical_Strokes").val("");
            debouncedFetchAndApplyValidationRanges();
            return;
        }

        const previousTheoreticalLineId = $("#ID_Theoretical_Blanking_Line").val();
        let materialTypeId = $("#ID_Material_type").val();

        if (!materialTypeId) {
            $("#theoretical_blk_line, #ID_Theoretical_Blanking_Line, #Theoretical_Strokes").val("");
            debouncedFetchAndApplyValidationRanges();
            return;
        }

        $("#theoretical_blk_line").val("Calculating...");

        let dataToSend = {
            plantId: config.project.plantId,
            materialTypeId: materialTypeId,
            thickness: parseFloat($("#Thickness").val()) || null,
            width: parseFloat($("#Width").val()) || null,
            pitch: parseFloat($("#Pitch").val()) || null,
            tensile: parseFloat($("#Tensile_Strenght").val()) || null
        };

        const $realLineSelect = $("#ID_Real_Blanking_Line");
        const realLineId = $realLineSelect.val();

        $.getJSON(config.urls.getTheoreticalLine, dataToSend)
            .done(function (response) {
                let newLineId = "";
                const realLineName = (realLineId && realLineId !== "0" && realLineId !== "") ? $realLineSelect.find('option:selected').text() : null;

                if (response.success) {
                    // We found a theoretical line
                    $("#theoretical_blk_line").val(response.lineName);
                    $("#ID_Theoretical_Blanking_Line").val(response.lineId);
                    newLineId = response.lineId.toString();

                    // Si NO hay una línea real seleccionada, la línea teórica es la que manda.
                    // Calculamos el OEE basado en ella.
                    if (!realLineId || realLineId === "0" || realLineId === "") {
                        fetchAndApplyOee(response.lineId);
                    }

                    if (newLineId !== previousTheoreticalLineId) {
                        if (realLineName) {
                            // Case 1: Success + Real Line exists. The Real Line has priority.
                            toastr.info(`Theoretical line is ${response.lineName}, but validations will use the selected Real Line: ${realLineName}.`);
                        } else {
                            // Case 2: Success + NO Real Line. The new Theoretical Line is in charge.
                            toastr.success(`Theoretical line updated to: ${response.lineName}`);
                        }
                    }
                } else {
                    // No theoretical line was found
                    $("#theoretical_blk_line").val("No Match");
                    $("#ID_Theoretical_Blanking_Line").val("");
                    newLineId = "";

                    // Si no hay línea teórica Y TAMPOCO hay línea real, limpiamos el campo OEE.
                    if (!realLineId || realLineId === "0" || realLineId === "") {
                        fetchAndApplyOee(null); // Llamar con null limpiará el campo.
                    }

                    if (previousTheoreticalLineId !== "") { // Only show a message if a value was cleared
                        if (realLineName) {
                            // Case 3: No Match + Real Line exists. The Real Line has priority.
                            toastr.warning(`No match for theoretical line. Validations will use the selected Real Line: ${realLineName}.`);
                        } else {
                            // Case 4: No Match + NO Real Line. Inform the user.
                            toastr.warning("No match found for any theoretical line. Selection has been cleared.");
                        }
                    }
                }
            })
            .fail(function () {
                $("#theoretical_blk_line").val("Error");
                $("#ID_Theoretical_Blanking_Line").val("");
            })
            .always(function () {
                // CRUCIAL: After calculation, ALWAYS call the function to get the final validation ranges.
                debouncedFetchAndApplyValidationRanges();
            });
    }

    // 2. Nueva función para calcular y mostrar el "Initial Weight".
    function updateWeightCalculations() {
        const annualVolume = parseFloat($("#Annual_Volume").val());
        const volumePerYear = parseFloat($("#Volume_Per_year").val());

        const weightPerPartInput = $("#WeightPerPart");
        const initialWeightInput = $("#Initial_Weight");

        let weightPerPart = 0;

        // 1. Calcular "Weight per Part" (la fórmula original de Initial Weight)
        if (!isNaN(annualVolume) && !isNaN(volumePerYear) && annualVolume !== 0) {
            weightPerPart = (volumePerYear / annualVolume) * 1000;
            weightPerPartInput.val(weightPerPart.toFixed(3));
        } else {
            weightPerPartInput.val("");
        }

        // 2. Calcular "Initial Weight" como "Weight per Part" + 1.5%
        if (weightPerPart > 0) {
            const initialWeight = weightPerPart * 1.015; // Suma del 1.5%
            initialWeightInput.val(initialWeight.toFixed(3));
        } else {
            initialWeightInput.val("");
        }

        // 3. Disparar actualización de la gráfica (ya estaba)
        debouncedUpdateSlitterChart();
    }

    function fetchAndApplyValidationRanges() {
        // 1. Guardar estado previo
        const previousRangesJSON = JSON.stringify(window.engineeringRanges);
        const previousMaxMults = window.maxMultsAllowed;

        const selectedRouteId = parseInt($("#ID_Route").val(), 10);
        const materialTypeId = $("#ID_Material_type").val();
        const realLineId = $("#ID_Real_Blanking_Line").val();
        const theoreticalLineId = $("#ID_Theoretical_Blanking_Line").val();
        const slitterLineId = $("#ID_Slitting_Line").val();

        const thicknessValue = parseFloat($("#Thickness").val()) || null;
        const tensileValue = parseFloat($("#Tensile_Strenght").val()) || null;

        const errorDiv = $('#slittingRuleError');

        // Aseguramos acceso a la variable global o usamos fallback
        const slittingRouteIdsLocal = (typeof slittingRouteIds !== 'undefined') ? slittingRouteIds : [8, 9, 10];
        const isSlittingRoute = slittingRouteIdsLocal.includes(selectedRouteId);

        // 2. Si falta el tipo de material, no podemos consultar reglas al servidor.
        if (!materialTypeId) {
            window.engineeringRanges = null;
            window.maxMultsAllowed = null;
            errorDiv.slideUp();
            updateLimitsDisplay();
            return;
        }

        let ajaxData = {
            materialTypeId: materialTypeId,
            thickness: thicknessValue,
            tensile: tensileValue
        };

        let isCallNeeded = false;

        // Configurar IDs para Blanking si aplica
        if (typeof blankingRouteIds !== 'undefined' && blankingRouteIds.includes(selectedRouteId)) {
            ajaxData.primaryLineId = realLineId || theoreticalLineId;
            isCallNeeded = !!ajaxData.primaryLineId;
        }

        // Configurar IDs para Slitting
        if (isSlittingRoute) {
            ajaxData.slitterLineId = slitterLineId;
            if (!ajaxData.primaryLineId) {
                ajaxData.primaryLineId = slitterLineId;
            }
            // Para Slitter SIEMPRE intentamos validar para obtener la tabla de reglas,
            // incluso si faltan dimensiones.
            isCallNeeded = true;
        }

        if (!isCallNeeded) {
            window.engineeringRanges = null;
            window.maxMultsAllowed = null;
            errorDiv.slideUp();
            updateLimitsDisplay();
            return;
        }

        $.getJSON(config.urls.getEngineeringDimensions, ajaxData)
            .done(function (response) {
                if (response.success) {
                    const newRangesJSON = JSON.stringify(response.validationRanges);
                    const newMaxMults = response.maxMultsAllowed;

                    window.engineeringRanges = response.validationRanges;
                    window.maxMultsAllowed = newMaxMults;

                    if (newRangesJSON !== previousRangesJSON) {
                        toastr.info("Validation limits updated.");
                    }

                    // --- LÓGICA DE VISIBILIDAD CON DEPURACIÓN ---
                    if (isSlittingRoute) {
                        console.log(`[Slitter Validation] MaxMults recibido: ${newMaxMults}`);

                        // Si el servidor nos devuelve un Máximo de Mults válido (> 0),
                        // significa que la combinación (Thickness + Tensile) es CORRECTA.
                        if (newMaxMults !== null && newMaxMults > 0) {
                            console.log("[Slitter Validation] Regla encontrada. OCULTANDO tabla de error.");
                            errorDiv.slideUp();
                        }
                        // Si nos devuelve 0 o null (porque faltan datos O porque están fuera de rango),
                        // MOSTRAMOS la tabla para guiar al usuario.
                        else {
                            console.log("[Slitter Validation] Regla NO encontrada (0 o null). MOSTRANDO tabla de error.");
                            errorDiv.slideDown();
                        }
                    } else {
                        // No es ruta de Slitter
                        errorDiv.slideUp();
                    }
                    // ---------------------------------------

                } else {
                    window.engineeringRanges = null;
                    window.maxMultsAllowed = null;
                    toastr.error(response.message || "Could not load validation limits.");
                }
            })
            .fail(function () {
                window.engineeringRanges = null;
            })
            .always(function () {
                updateLimitsDisplay();
                validateMultipliers();
                // Revalidar campos dependientes
                validateTensileStrength();
                validateThickness();
                validateWidth();
                validatePitch();
            });
    }

    function setupConditionalTextareas() {
        // Escuchar cambios en los checkboxes de "Additionals"
        $('#additionals-container').on('change', '.additional-checkbox', function () {
            // La opción "Other" para Additionals tiene el ID 6
            const isOtherChecked = $('#additional-6').is(':checked');
            const otherContainer = $('#AdditionalsOtherDescription_container');

            if (isOtherChecked) {
                otherContainer.slideDown();
            } else {
                otherContainer.slideUp();
                otherContainer.find('textarea').val(''); // Limpia el valor al ocultar
            }
        });

        // Escuchar cambios en los checkboxes de "Labels"
        $('#labels-container').on('change', '.label-checkbox', function () {
            // La opción "Other" para Labels tiene el ID 3
            const isOtherChecked = $('#label-3').is(':checked');
            const otherContainer = $('#LabelOtherDescription_container');

            if (isOtherChecked) {
                otherContainer.slideDown();
            } else {
                otherContainer.slideUp();
                otherContainer.find('textarea').val(''); // Limpia el valor al ocultar
            }
        });
    }
    // Muestra u oculta los campos de empaque de tkMM según la selección
    function toggleTkmmPackagingFields() {
        const standard = $('#PackagingStandard').val();

        // Contenedores a controlar
        const rackContainer = $('#RequiresRackManufacturing_container');
        const dieContainer = $('#RequiresDieManufacturing_container');
        const additionalGroup = $('#additionals-group_container');
        const strapGroup = $('#straps-group_container');

        if (standard === 'OWN') {
            // --- LÓGICA PARA MOSTRAR ---
            // Primero nos aseguramos de que los contenedores tengan la clase d-flex ANTES de animar.
            rackContainer.addClass('d-flex');
            dieContainer.addClass('d-flex');

            // Ahora ejecutamos la animación para todos.
            additionalGroup.slideDown();
            strapGroup.slideDown();
            rackContainer.slideDown();
            dieContainer.slideDown();

        } else {
            // --- LÓGICA PARA OCULTAR (LA CORRECCIÓN) ---
            // Animamos hacia arriba y, en el callback (que se ejecuta al terminar),
            // quitamos la clase 'd-flex' que causa el conflicto.
            rackContainer.slideUp(function () {
                $(this).removeClass('d-flex');
            });
            dieContainer.slideUp(function () {
                $(this).removeClass('d-flex');
            });

            // Los otros dos contenedores también usan flexbox a través de la clase 'checkbox-group-container',
            // por lo que aplicamos la misma lógica para ser consistentes.
            additionalGroup.slideUp();
            strapGroup.slideUp();

            // Limpiamos los valores (esta parte no cambia)
            rackContainer.find('input[type="checkbox"]').prop('checked', false);
            dieContainer.find('input[type="checkbox"]').prop('checked', false);
            additionalGroup.find('input[type="checkbox"]').prop('checked', false).trigger('change');
            strapGroup.find('input[type="checkbox"]').prop('checked', false).trigger('change');
            strapGroup.find('textarea').val('');
        }
    }

    //***************** funciones globales ************************

    // Función debounce
    function debounce(func, wait) {
        var timeout;
        return function () {
            var context = this, args = arguments;
            clearTimeout(timeout);
            timeout = setTimeout(function () {
                func.apply(context, args);
            }, wait);
        };
    }

    function redondearValor(elemento, decimales) {
        let value = parseFloat($(elemento).val());
        if (!isNaN(value)) {
            let factor = Math.pow(10, decimales);
            let rounded = Math.round(value * factor) / factor;
            $(elemento).val(rounded);
        }
    }

    function generateThicknessInputs() {
        const container = $('#weldedPlatesThickness_container');
        const numberOfPlatesInput = $('#numberOfPlates');
        const numberOfPlates = parseInt(numberOfPlatesInput.val(), 10);

        // PASO 1: Guardar los valores existentes ANTES de borrar nada.
        let existingValues = {};
        container.find('.welded-thickness-input').each(function () {
            // Guardamos el valor usando el ID del input como llave (ej: "thicknessPlate1")
            existingValues[$(this).attr('id')] = $(this).val();
        });

        // PASO 2: Ahora sí, limpiar el contenedor.
        container.empty().hide();

        if (isNaN(numberOfPlates) || numberOfPlates < 2 || numberOfPlates > 5) {
            if (numberOfPlatesInput.val()) { // Solo mostrar error si hay algo escrito
                toastr.warning('Number of plates must be between 2 and 5.', 'Invalid Input');
            }
            return;
        }

        // Si canEditSales es true, es un string vacío ""; si es false, es "readonly".
        const readOnlyAttribute = canEditSales ? "" : "readonly";

        let inputsHtml = '<div class="row">';
        const colWidth = Math.max(Math.floor(12 / numberOfPlates), 2);

        for (let i = 1; i <= numberOfPlates; i++) {
            const inputId = `thicknessPlate${i}`;
            // PASO 3: Al crear el input, buscar si tenía un valor guardado
            const previousValue = existingValues[inputId] || ''; // Si no hay valor, queda vacío

            inputsHtml += `
                    <div class="col-md-${colWidth} form-group">
                        <label for="${inputId}">Thickness Plate ${i}</label>
                        <input type="number" id="${inputId}" class="form-control welded-thickness-input"
                               placeholder="mm" min="0" step="any"
                               value="${previousValue}" ${readOnlyAttribute} /> <span id="${inputId}Error" class="error-message" style="color: red; display: none;"></span>
                    </div>
                `;
        }
        inputsHtml += '</div>';

        container.html(inputsHtml).slideDown();
    }

    // Función para convertir la respuesta en un formato tabular para Handsontable.
    function convertToHandsontableMatrix(dataArray) {
        // 1) Recopilar todas las claves únicas de todos los objetos
        let allKeys = new Set();
        dataArray.forEach(obj => {
            Object.keys(obj).forEach(k => allKeys.add(k));
        });
        let keysArray = Array.from(allKeys);

        // 2) Definir el orden deseado:
        //    a) "LineId" (oculta)
        //    b) "Line" (nombre)
        //    c) "PartNumbers"
        //    d) Columnas "FY" (años fiscales)
        //    e) "Status" (Al final)

        const lineIdKey = keysArray.includes("LineId") ? ["LineId"] : [];
        const lineKey = keysArray.includes("Line") ? ["Line"] : [];
        const partNumbersKey = keysArray.includes("PartNumbers") ? ["PartNumbers"] : [];

        const fyKeys = keysArray.filter(k => k.startsWith("FY"));
        fyKeys.sort(); // Ordena las claves de FY alfabéticamente.

        // Extraemos explícitamente "Status"
        const statusKey = keysArray.includes("Status") ? ["Status"] : [];

        // Filtramos las claves restantes, excluyendo las que ya procesamos
        const remainingKeys = keysArray.filter(k =>
            k !== "LineId" &&
            k !== "Line" &&
            k !== "PartNumbers" &&
            k !== "Status" &&
            !k.startsWith("FY")
        );
        remainingKeys.sort();

        // 3) Construir el arreglo final de claves en el orden exacto
        let sortedKeys = [].concat(lineIdKey, lineKey, partNumbersKey, fyKeys, remainingKeys, statusKey);

        // 4) Construir la cabecera (headers) y las filas (rows)
        let headers = sortedKeys;
        let rows = dataArray.map(obj => {
            return sortedKeys.map(k => obj[k] !== undefined ? obj[k] : "");
        });

        return { headers, rows };
    }

    function parseYearMonth(dateStr) {
        if (!dateStr) return null;
        var parts = dateStr.split("-");
        if (parts.length < 2) return null;
        return new Date(parts[0], parts[1] - 1, 1);
    }

    // Re-numerar filas tras eliminación
    function renumberRows() {
        $("#materialsTable tbody tr").each(function (i, row) {
            $(row).attr("data-index", i);
            let hiddenInputs = $(row).find("input[name^='materials']");
            hiddenInputs.each(function () {
                let nameAttr = $(this).attr("name"); // p.ej. materials[3].Part_Number
                let fieldName = nameAttr.substring(nameAttr.indexOf('.') + 1);
                // fieldName => Part_Number
                $(this).attr("name", `materials[${i}].${fieldName}`);
            });
        });
    }

    function validarContraCriterio(valor, criterio) {
        // Destructuramos la nueva propiedad 'Tolerance' que ahora viene del objeto
        const { NumericValue, MinValue, MaxValue, Tolerance } = criterio;

        if (NumericValue != null) {
            return valor === NumericValue;
        }

        // La holgura es el valor que viene de la BD. Si es nulo o inválido, será 0.
        const toleranceAmount = (typeof Tolerance === 'number') ? Tolerance : 0;

        if (MinValue != null) {
            const lowerLimit = MinValue - toleranceAmount;
            if (valor < lowerLimit) {
                return false;
            }
        }

        if (MaxValue != null) {
            const upperLimit = MaxValue + toleranceAmount;
            if (valor > upperLimit) {
                return false;
            }
        }

        return true; // El valor es válido
    }

    function describirLimites(criterio) {
        const { NumericValue, MinValue, MaxValue, Tolerance } = criterio;
        const toleranceAmount = (typeof Tolerance === 'number') ? Tolerance : 0;

        console.log('critero: ')
        console.log(criterio)

        let text = "No limits defined";

        if (NumericValue != null) {
            text = `Must be exactly ${NumericValue}`;
        }
        else if (MinValue != null && MaxValue != null) {
            text = `Allowed range: ${MinValue} - ${MaxValue}`;
            if (toleranceAmount > 0) {
                text += ` (+/- ${toleranceAmount})`;
            }
        }
        else if (MinValue != null) {
            text = `Minimum allowed: ${MinValue}`;
            if (toleranceAmount > 0) {
                text += ` (tolerance: -${toleranceAmount})`;
            }
        }
        else if (MaxValue != null) {
            text = `Maximum allowed: ${MaxValue}`;
            if (toleranceAmount > 0) {
                text += ` (tolerance: +${toleranceAmount})`;
            }
        }

        return text;
    }

    const debouncedUpdateSlitterChart = debounce(function () {
        toastr.info("Recalculating slitter graph...");
        updateSlitterCapacityChart(false, config.project.id);
    }, 800);

    function syncAllVehicleSections(row) {
        // Obtenemos los códigos de los 4 vehículos desde los inputs ocultos de la fila.
        const vehicleCode1 = row.find("input[name$='.Vehicle']").val();
        const vehicleCode2 = row.find("input[name$='.Vehicle_2']").val();
        const vehicleCode3 = row.find("input[name$='.Vehicle_3']").val();
        //const vehicleCode4 = row.find("input[name$='.Vehicle_4']").val();

        // Sincronizamos cada par. La función interna se encarga de todo.
        syncSingleVehicleAndCountry(vehicleCode1, '#ihsCountry1', '#Vehicle');
        syncSingleVehicleAndCountry(vehicleCode2, '#ihsCountry2', '#Vehicle_2');
        syncSingleVehicleAndCountry(vehicleCode3, '#ihsCountry3', '#Vehicle_3');
        //syncSingleVehicleAndCountry(vehicleCode4, '#ihsCountry4', '#Vehicle_4');
    }

    /**
     * Orquesta la sincronización del país y el vehículo usando llamadas SÍNCRONAS.
     */
    function syncSingleVehicleAndCountry(vehicleCode, countrySelector, vehicleSelector) {
        const $countryDropdown = $(countrySelector);
        const $vehicleDropdown = $(vehicleSelector);

        // Si no hay código de vehículo, reseteamos los dropdowns y terminamos.
        if (!vehicleCode) {
            $countryDropdown.val("MEX").trigger('change.select2'); // Opcional: poner un país por defecto
            loadVehiclesForDropdown("MEX", vehicleSelector);
            return;
        }

        const mnemonic = vehicleCode.split('_')[0];
        let country = "MEX"; // País por defecto

        // 1. PRIMERA LLAMADA SÍNCRONA: Obtener el país para este vehículo.
        // Es síncrona (async: false) para asegurar que tenemos el país antes de continuar.
        $.ajax({
            url: config.urls.GetCountryForIHS,
            data: { ihsCode: vehicleCode },
            async: false,
            success: function (response) {
                if (response.success) {
                    country = response.country;
                }
            }
        });

        // 2. SEGUNDA LLAMADA SÍNCRONA: Obtener la lista de vehículos para ese país.
        // También es síncrona para asegurar que la lista esté cargada antes de intentar seleccionar un valor.
        $.ajax({
            url: config.urls.getIHSByCountry,
            data: { country: country },
            async: false,
            success: function (vehicleData) {
                // 3. POBLAR Y SELECCIONAR: Ahora que todo está cargado, podemos asignar valores.
                $countryDropdown.val(country).trigger('change.select2');

                let newOptions = '<option value="">Select a Vehicle</option>';
                $.each(vehicleData, (i, item) => {
                    newOptions += `<option value="${item.Value}" data-sop="${item.SOP}" data-eop="${item.EOP}"
                                       data-program="${item.Program}" data-maxproduction="${item.MaxProduction}"
                                       data-productionjson='${item.ProductionDataJson}'>${item.Text}</option>`;
                });
                $vehicleDropdown.html(newOptions);

                // Buscamos el código exacto en la nueva lista y lo seleccionamos.
                const matchingVehicle = vehicleData.find(v => v.Value.startsWith(mnemonic + '_'));
                const finalVehicleCode = matchingVehicle ? matchingVehicle.Value : vehicleCode;
                $vehicleDropdown.val(finalVehicleCode).trigger('change.select2');


                $vehicleDropdown.prop('disabled', !canEditSales).trigger('change.select2');

            }
        });
    }

    /**
     * Carga la lista de vehículos de forma asíncrona (usada solo si el usuario cambia el país manualmente).
     */
    function loadVehiclesForDropdown(country, targetVehicleSelector) {
        const $vehicleDropdown = $(targetVehicleSelector);

        // 1. Deshabilitamos y mostramos el mensaje de "cargando"
        $vehicleDropdown.prop('disabled', true).html('<option>Loading vehicles...</option>').trigger('change.select2');

        // 2. Ejecutamos la llamada AJAX y encadenamos los handlers de promesa.
        $.ajax({
            url: config.urls.getIHSByCountry,
            type: 'GET',
            data: { country: country },
            dataType: 'json',
            success: function (data) {
                // Éxito: poblamos el dropdown
                let newOptions = '<option value="">Select a Vehicle</option>';
                $.each(data, (i, item) => {
                    newOptions += `<option value="${item.Value}" data-sop="${item.SOP}" data-eop="${item.EOP}"
                                       data-program="${item.Program}" data-maxproduction="${item.MaxProduction}"
                                       data-productionjson='${item.ProductionDataJson}'>${item.Text}</option>`;
                });
                $vehicleDropdown.html(newOptions);
            },
            error: function (xhr, status, error) {
                // Error: Notificamos y reseteamos el dropdown
                console.error("Error en loadVehiclesForDropdown:", status, error);
                toastr.error("Error loading vehicles for the selected country.", "AJAX Error");
                $vehicleDropdown.html('<option value="">Error loading data</option>');
            }
        })
        // Usamos .always() encadenado para garantizar que la re-habilitación se ejecute SIEMPRE,
        // sin importar si fue success o error.
        .always(function () {
            // 3. Siempre: Re-habilitamos el dropdown (respetando el permiso)
            console.log('CanEditSales: ' + canEditSales)
            $vehicleDropdown.prop('disabled', !canEditSales).trigger('change.select2');
        });

    }

    // Calcular Golpes Efectivos ---
    // --- FUNCIÓN MODIFICADA: Calcular Golpes Efectivos (Enteros) ---
    function updateEffectiveStrokes() {
        // Obtenemos valores
        const oeeVal = parseFloat($("#OEE").val()) || 0;
        const theoStrokes = parseFloat($("#Theoretical_Strokes").val()) || 0;
        const realStrokes = parseFloat($("#Real_Strokes").val()) || 0;

        // Factor OEE (ej: 85% -> 0.85). Si es 0, el resultado será 0.
        const oeeFactor = oeeVal / 100;

        // Cálculo Theoretical Effective
        if (theoStrokes > 0 && oeeFactor > 0) {
            // CAMBIO: Usamos Math.round() para redondear al entero más cercano
            let result = theoStrokes * oeeFactor;
            $("#Theoretical_Effective_Strokes").val(Math.round(result));
        } else {
            $("#Theoretical_Effective_Strokes").val("");
        }

        // Cálculo Real Effective
        if (realStrokes > 0 && oeeFactor > 0) {
            // CAMBIO: Usamos Math.round() para redondear al entero más cercano
            let result = realStrokes * oeeFactor;
            $("#Real_Effective_Strokes").val(Math.round(result));
        } else {
            $("#Real_Effective_Strokes").val("");
        }
    }

    // --- FUNCIONES AUXILIARES PARA IHS ---
    function loadRowDataIntoForm(row) {

        // 1. Obtener datos y sincronizar IHS (sin cambios)
        let hiddenInputs = row.find("input[name^='materials']");
        let vehicleCode = row.find("input[name$='.Vehicle']").val();
        syncAllVehicleSections(row);

        // Limpiar todos
        $('.rack-type-checkbox, .additional-checkbox, .label-checkbox, .strap-checkbox').prop('checked', false);
        $('#interplant-rack-types-container .interplant-rack-type-checkbox').prop('checked', false);
        $('#interplant-labels-container .form-check-input').prop('checked', false); // <-- LÍNEA AÑADIDA
        $('#interplant-additionals-container .form-check-input').prop('checked', false);
        $('#interplant-straps-container .form-check-input').prop('checked', false);

        // 2. Poblar los campos del formulario
        hiddenInputs.each(function () {
            let nameAttr = $(this).attr("name");
            let val = $(this).val();

            // Si es un ID de RackType, márcalo y continúa
            if (nameAttr.endsWith('.SelectedRackTypeIds')) {
                if (val) {
                    $(`#rack-type-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedAdditionalIds')) {
                if (val) {
                    $(`#additional-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedLabelIds')) {
                if (val) {
                    $(`#label-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedStrapTypeIds')) {
                if (val) {
                    $(`#strap-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedInterplantRackTypeIds')) {
                if (val) {
                    // Busca el checkbox con ese valor y lo marca
                    $(`#interplant-rack-type-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedInterplantLabelIds')) {
                if (val) {
                    // Busca el checkbox con ese valor y lo marca
                    $(`#interplant-label-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedInterplantAdditionalIds')) {
                if (val) {
                    $(`#interplant-additional-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }
            if (nameAttr.endsWith('.SelectedInterplantStrapTypeIds')) {
                if (val) {
                    $(`#interplant-strap-${val}`).prop('checked', true);
                }
                return; // Continúa con el siguiente input oculto
            }

            for (let col of columnDefs) {
                if (nameAttr.endsWith(`.${col.key}`)) {

                    if (col.type === "check") {
                        $(col.selector).prop("checked", val === "true");
                    }
                    else if (col.key === 'Quality') {
                        const $qualitySelect = $(col.selector);

                        // Paso A: Asegurarse de que la opción exista en el <select> (para valores nuevos)
                        if (val && $qualitySelect.find(`option[value="${val}"]`).length === 0) {
                            let newOption = new Option(val, val, true, true);
                            $qualitySelect.append(newOption);
                        }

                        // Paso B: Asignar el valor y disparar el evento 'change' INMEDIATAMENTE
                        // en este elemento específico para forzar la actualización visual de Select2.
                        $qualitySelect.val(val).trigger('change');
                    }
                    else {
                        // --- INICIO DE LA MODIFICACIÓN (Manejar N/A al cargar) ---
                        // Comprobamos si la clave es uno de los campos de Coil Position
                        if (col.key === 'ID_Coil_Position' ||
                            col.key === 'ID_Delivery_Coil_Position' ||
                            col.key === 'ID_InterplantDelivery_Coil_Position' ||
                            col.key === 'ID_Arrival_Packaging_Type' || // <-- AÑADIDO
                            col.key === 'ID_Arrival_Protective_Material') { // <-- AÑADIDO

                            // Si el valor es nulo o vacío (vino como null de la BD),
                            // lo forzamos a "0" para que seleccione "N/A".
                            // Si tiene un valor (como 1, 2, etc.), usa ese valor.
                            $(col.selector).val(val || "0");
                        } else {
                            // Manejo normal para todos los demás campos
                            $(col.selector).val(val);
                        }
                        // --- FIN DE LA MODIFICACIÓN ---
                    }
                }
            }

        });

        // 3. Forzamos la actualización del resto de los combos Select2 (excluyendo Quality que ya se actualizó)
        $('.select2:not(#quality)').trigger('change.select2');

        // --- INICIO DE LA LÓGICA CORREGIDA PARA WELDER BLANKS ---
        // --- INICIO DEPURACIÓN DE PLATINAS SOLDADAS ---
        const isWelded = row.find("input[name$='.IsWeldedBlank']").val() === "true";
        $('#IsWeldedBlank').prop('checked', isWelded);

        if (isWelded) {
            // 1. Mostrar el contenedor del número de platinas
            $('#numberOfPlates_container').show(); // <-- ¡LA LÍNEA CLAVE QUE FALTABA!

            // 2. Establecer el número de platinas
            const numPlates = row.find("input[name$='.NumberOfPlates']").val();
            $('#numberOfPlates').val(numPlates);

            // 3. Generar y poblar los inputs de espesor (esto ya estaba bien)
            generateThicknessInputs();

            const weldedPlatesJson = row.find("input[name$='.WeldedPlatesJson']").val();
            if (weldedPlatesJson && weldedPlatesJson !== '[]') {
                $("#weldedPlatesJson").val(weldedPlatesJson);
                try {
                    const plates = JSON.parse(weldedPlatesJson);
                    plates.forEach(plate => {
                        $(`#thicknessPlate${plate.PlateNumber}`).val(plate.Thickness);
                    });
                } catch (e) {
                    console.error("Error al parsear el JSON de las platinas:", e);
                }
            }
        } else {
            // Lógica para ocultar si NO es Welded Blank
            $('#numberOfPlates_container').hide();
            $('#weldedPlatesThickness_container').empty().hide();
        }
        // --- FIN DE LA LÓGICA CORREGIDA ---


        // 4. Asignar el texto de la línea teórica desde la celda de la tabla
        $('#theoretical_blk_line').val(row.find('td').eq(28).text());

        // 4. Se actualiza el resto de la UI
        $("#ID_Route").trigger("change");

        let slittingLineId = row.find("input[name$='.ID_Slitting_Line']").val();
        if (slittingLineId === "8") {
            $("#slitting_line_name").val("SLITTER");
        } else {
            $("#slitting_line_name").val("");
        }

        updateVehicleVisibility();
        shapeVisibility();
        updatePackagingFileUI();
        updateFileUIGeneric('ID_File_TechnicalSheet', 'TechnicalSheetFileName', 'file_container_technicalSheetFile', 'fileActions_containerTechnicalSheetFile', 'downloadFileTechnicalSheetFile');
        updateFileUIGeneric('ID_File_Additional', 'AdditionalFileName', 'file_container_AdditionalFile', 'fileActions_containerAdditionalFile', 'downloadFileAdditional');
        updateFileUIGeneric('ID_File_ArrivalAdditional', 'ArrivalAdditionalFileName', 'file_container_arrivalAdditionalFile', 'fileActions_containerArrivalAdditionalFile', 'downloadArrivalAdditionalFile');
        updateFileUIGeneric('ID_File_CoilDataAdditional', 'CoilDataAdditionalFileName', 'file_container_coilDataAdditionalFile', 'fileActions_containerCoilDataAdditionalFile', 'downloadCoilDataAdditionalFile');
        updateFileUIGeneric('ID_File_SlitterDataAdditional', 'SlitterDataAdditionalFileName', 'file_container_slitterDataAdditionalFile', 'fileActions_containerSlitterDataAdditionalFile', 'downloadSlitterDataAdditionalFile');
        updateFileUIGeneric('ID_File_VolumeAdditional', 'VolumeAdditionalFileName', 'file_container_volumeAdditionalFile', 'fileActions_containerVolumeAdditionalFile', 'downloadVolumeAdditionalFile');
        updateFileUIGeneric('ID_File_OutboundFreightAdditional', 'OutboundFreightAdditionalFileName', 'file_container_outboundFreightAdditionalFile', 'fileActions_containerOutboundFreightAdditionalFile', 'downloadOutboundFreightAdditionalFile');
        updateFileUIGeneric('ID_File_DeliveryPackagingAdditional', 'DeliveryPackagingAdditionalFileName', 'file_container_deliveryPackagingAdditionalFile', 'fileActions_containerDeliveryPackagingAdditionalFile', 'downloadDeliveryPackagingAdditionalFile');
        updateFileUIGeneric('ID_File_InterplantPackaging', 'InterplantPackagingFileName', 'file_container_interplant_packaging_archivo', 'fileActions_containerInterplantPackagingFile', 'downloadInterplantPackagingFile');
        updateFileUIGeneric('ID_File_InterplantOutboundFreight', 'InterplantOutboundFreightFileName', 'file_container_interplantOutboundFreightAdditionalFile', 'fileActions_containerInterplantOutboundFreightFile', 'downloadInterplantOutboundFreightFile');

        toggleTkmmPackagingFields(); // <-- Añadir esta llamada
        handleArrivalProtectiveMaterialChange();
        handleStackableChange();
        handleArrivalTransportTypeChange();
        toggleRunningChangeWarning();

        $("#materialIndex").val(row.data("index"));
        $("#materialId").val(row.data("material-id"));


        // -- INICIO Calculo oee --
        console.log("Datos cargados en el formulario. Disparando cálculo de OEE...");
        // 1. Obtenemos los IDs de las líneas que acabamos de cargar en los campos del formulario.
        const realLineId = $("#ID_Real_Blanking_Line").val();
        const theoreticalLineId = $("#ID_Theoretical_Blanking_Line").val();

        // 2. Determinamos qué línea usar: la Real tiene prioridad.
        const lineIdToUseForOee = (realLineId && realLineId !== "0" && realLineId !== "")
            ? realLineId
            : theoreticalLineId;
        // 3. Llamamos a nuestra función para calcular el OEE con la línea correspondiente.
        fetchAndApplyOee(lineIdToUseForOee);
        // -- FIN DEL Calculo oee --

        // 5. Llamamos a la función que actualiza las gráficas
        debouncedUpdateCapacityGraphs();

        // dispara el evento 'change' en los checkboxes "Other"
        $('#additional-6').trigger('change');
        $('#label-3').trigger('change');
        updateWeightCalculations();
        updatePackageWeight();
        updateInterplantPackageWeight();
        updateEffectiveStrokes();
        handleArrivalWarehouseChange();

        // Disparar los handlers para ajustar la UI de los campos dependientes
        $('#IsReturnableRack').trigger('change');
        $('#ScrapReconciliation').trigger('change');
        $('#HeadTailReconciliation').trigger('change');
        $('#ID_Delivery_Transport_Type').trigger('change');
        $('#ID_FreightType').trigger('change');

        $('#ID_InterplantDelivery_Transport_Type').trigger('change');
        $('#InterplantPackagingStandard').trigger('change');

        $('#InterplantScrapReconciliation').trigger('change');
        $('#InterplantHeadTailReconciliation').trigger('change');

        // Dispara los eventos 'change' de los checkboxes "Other" (Final Delivery e Interplant)
        // para asegurar que sus textareas se muestren u oculten correctamente al cargar.
        $('#additional-6').trigger('change');
        $('#label-3').trigger('change');
        $('#interplant-additional-6').trigger('change');
        $('#interplant-label-3').trigger('change');

        updateInterplantReturnableFieldsVisibility();

        // Disparar handlers para los nuevos campos de Interplant
        $('#IsInterplantReturnableRack').trigger('change');
        $('#InterplantScrapReconciliation').trigger('change');
        $('#InterplantHeadTailReconciliation').trigger('change');

     
        // Usamos 'false' porque estamos cargando datos al formulario para una posible edición (escenario what-if).
        debouncedUpdateSlitterChart();



    }


    // Construye fila para la tabla
    function buildRowHtml(materialData, rowIndex) {
        let tdVehicle = `<td nowrap>${materialData["Vehicle"]}</td>`;
        let tdVehicleVersion = `<td nowrap>${materialData["Vehicle_version"]}</td>`;
        let tdSOP = `<td nowrap>${materialData["SOP_SP"]}</td>`;
        let tdEOP = `<td nowrap>${materialData["EOP_SP"]}</td>`;
        let tdRealSOP = `<td nowrap>${materialData["Real_SOP"]}</td>`;
        let tdRealEOP = `<td nowrap>${materialData["Real_EOP"]}</td>`;
        let tdShipTo = `<td nowrap>${materialData["Ship_To"]}</td>`;
        let tdPartName = `<td nowrap>${materialData["Part_Name"]}</td>`;
        let tdPartNumber = `<td nowrap>${materialData["Part_Number"]}</td>`;
        let tdQuality = `<td nowrap>${materialData["Quality"]}</td>`;
        let tdProgramSP = `<td nowrap>${materialData["Program_SP"]}</td>`;
        let tdMaxProduction = `<td nowrap>${materialData["Max_Production_SP"]}</td>`;
        let tdAnnualVolume = `<td nowrap>${materialData["Annual_Volume"]}</td>`;
        let tdVolumePerYear = `<td nowrap>${materialData["Volume_Per_year"]}</td>`;

        // Para ID_Route, buscamos el texto en el combo:
        let routeId = materialData["ID_Route"];
        let routeText = $("#ID_Route option[value='" + routeId + "']").text() || routeId;  // Si no se encuentra, muestra el id
        let tdIDRoute = `<td nowrap>${routeText}</td>`;

        //tensile
        let tdTensileStrenght = `<td nowrap>${materialData["Tensile_Strenght"]}</td>`;

        // Para ID_Material_type
        let materialTypeId = materialData["ID_Material_type"];
        let materialTypeText = $("#ID_Material_type option[value='" + materialTypeId + "']").text() || materialTypeId;  // Si no se encuentra, muestra el id
        let tdMaterialType = `<td nowrap>${materialTypeText}</td>`;

        //thickness, width, pitch
        let tdThickness = `<td nowrap>${materialData["Thickness"]}</td>`;
        let tdWidth = `<td nowrap>${materialData["Width"]}</td>`;
        let tdPitch = `<td nowrap>${materialData["Pitch"]}</td>`;
        let tdTheoricalGrossWeight = `<td nowrap>${materialData["Theoretical_Gross_Weight"]}</td>`;
        let tdGrossWeight = `<td nowrap>${materialData["Gross_Weight"]}</td>`;

        // Para ID_Shape
        let shapeId = materialData["ID_Shape"];
        let shapeText = $("#ID_Shape option[value='" + shapeId + "']").text() || shapeId;  // Si no se encuentra, muestra el id
        let tdShape = `<td nowrap>${shapeText}</td>`;

        let tdAngleA = `<td nowrap>${materialData["Angle_A"]}</td>`;
        let tdAngleB = `<td nowrap>${materialData["Angle_A"]}</td>`;
        let tdBlanksPerStroke = `<td nowrap>${materialData["Blanks_Per_Stroke"]}</td>`;
        let tdPartsPerVehicle = `<td nowrap>${materialData["Parts_Per_Vehicle"]}</td>`;

        // Para ID_Real_Blanking_Line
        let realLineId = materialData["ID_Real_Blanking_Line"];
        let realLineText = $("#ID_Real_Blanking_Line option[value='" + realLineId + "']").text() || realLineId;  // Si no se encuentra, muestra el id
        let tdRealLine = `<td nowrap>${realLineText}</td>`;

        let tdTheoricalStrokes = `<td nowrap>${materialData["Theoretical_Strokes"]}</td>`;
        let tdRealStrokes = `<td nowrap>${materialData["Real_Strokes"]}</td>`;
        let tdIdealCycleTimePerTool = `<td nowrap>${materialData["Ideal_Cycle_Time_Per_Tool"]}</td>`;
        let tdOEE = `<td nowrap>${materialData["OEE"]}</td>`;
        let tdStatusDM = `<td nowrap>${materialData["OEE"]}</td>`;


        let hiddenInputs = "";
        for (let col of columnDefs) {
            // Ignoramos los checkboxGroup en este bucle, porque
            // tienen su propia lógica de bucle 'forEach' más abajo.
            if (col.type === "checkboxGroup") {
                continue; // Salta a la siguiente iteración
            }

            let inputValue;

            if (col.type === "check") {
                // Para checkbox siempre usamos el verdadero "true" o "false"
                inputValue = materialData[col.key] === "true" ? "true" : "false";
            }
            else {
                let $input = $(col.selector);
                if ($input.attr("type") === "hidden") {
                    // Hidden: tomamos siempre materialData
                    inputValue = materialData[col.key];
                } else {
                    // Visible → su valor, Oculto → vacío
                    inputValue = $input.is(":visible") ? materialData[col.key] : "";
                }
            }

            hiddenInputs +=
                `<input type="hidden" name="materials[${rowIndex}].${col.key}" value="${inputValue}" />`;
        }

        // V V V --- AÑADIR ESTE BUCLE --- V V V
        // Generar un input hidden por cada ID de RackType seleccionado
        if (materialData.SelectedRackTypeIds && materialData.SelectedRackTypeIds.length > 0) {
            materialData.SelectedRackTypeIds.forEach(function (id) {
                hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedRackTypeIds" value="${id}" />`;
            });
        }
        if (materialData.SelectedAdditionalIds) { materialData.SelectedAdditionalIds.forEach(function (id) { hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedAdditionalIds" value="${id}" />`; }); }
        if (materialData.SelectedLabelIds) { materialData.SelectedLabelIds.forEach(function (id) { hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedLabelIds" value="${id}" />`; }); }
        if (materialData.SelectedStrapTypeIds) { materialData.SelectedStrapTypeIds.forEach(function (id) { hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedStrapTypeIds" value="${id}" />`; }); }

        // --- INICIO DE LA MODIFICACIÓN (Añadir bucles para Interplant) ---
        if (materialData.SelectedInterplantRackTypeIds && materialData.SelectedInterplantRackTypeIds.length > 0) {
            materialData.SelectedInterplantRackTypeIds.forEach(function (id) {
                hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedInterplantRackTypeIds" value="${id}" />`;
            });
        }
        if (materialData.SelectedInterplantLabelIds && materialData.SelectedInterplantLabelIds.length > 0) {
            materialData.SelectedInterplantLabelIds.forEach(function (id) {
                hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedInterplantLabelIds" value="${id}" />`;
            });
        }
        if (materialData.SelectedInterplantAdditionalIds && materialData.SelectedInterplantAdditionalIds.length > 0) {
            materialData.SelectedInterplantAdditionalIds.forEach(function (id) {
                hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedInterplantAdditionalIds" value="${id}" />`;
            });
        }
        if (materialData.SelectedInterplantStrapTypeIds && materialData.SelectedInterplantStrapTypeIds.length > 0) {
            materialData.SelectedInterplantStrapTypeIds.forEach(function (id) {
                hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].SelectedInterplantStrapTypeIds" value="${id}" />`;
            });
        }

        // Agregar bandera para identificar que este material es el que tendrá el archivo.
        // Si materialData["IsFile"] está definido, úsalo; en caso contrario, asigna "false".
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsFile" value="${materialData["IsFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsPackagingFile" value="${materialData["IsPackagingFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsTechnicalSheetFile" value="${materialData["IsTechnicalSheetFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsAdditionalFile" value="${materialData["IsAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsArrivalAdditionalFile" value="${materialData["IsArrivalAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsCoilDataAdditionalFile" value="${materialData["IsCoilDataAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsSlitterDataAdditionalFile" value="${materialData["IsSlitterDataAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsVolumeAdditionalFile" value="${materialData["IsVolumeAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsOutboundFreightAdditionalFile" value="${materialData["IsOutboundFreightAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsDeliveryPackagingAdditionalFile" value="${materialData["IsDeliveryPackagingAdditionalFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsInterplantPackagingFile" value="${materialData["IsInterplantPackagingFile"] || "false"}" />`;
        hiddenInputs += `<input type="hidden" name="materials[${rowIndex}].IsInterplantOutboundFreightFile" value="${materialData["IsInterplantOutboundFreightFile"] || "false"}" />`;

        let tdActions = `
                            <td nowrap>
                                <button type="button" class="btn btn-warning btn-xs edit-row">Edit</button>
                                <button type="button" class="btn btn-danger btn-xs remove-row">Remove</button>
                                ${hiddenInputs}
                            </td nowrap>`;

        let rowHtml = `
                            <tr data-index="${rowIndex}">
                                ${tdActions}
                                ${tdVehicle}
                                ${tdVehicleVersion}
                                ${tdSOP}
                                ${tdEOP}
                                ${tdRealSOP}
                                ${tdRealEOP}
                                ${tdShipTo}
                                ${tdPartName}
                                ${tdPartNumber}
                                ${tdQuality}
                                ${tdProgramSP}
                                ${tdMaxProduction}
                                ${tdAnnualVolume}
                                ${tdVolumePerYear}
                                ${tdIDRoute}
                                ${tdTensileStrenght}
                                ${tdMaterialType}
                                ${tdThickness}
                                ${tdWidth}
                                ${tdPitch}
                                ${tdBlanksPerStroke}
                                ${tdPartsPerVehicle}
                                ${tdTheoricalGrossWeight}
                                ${tdGrossWeight}
                                ${tdShape}
                                ${tdAngleA}
                                ${tdAngleB}
                                ${tdRealLine}
                                ${tdTheoricalStrokes}
                                ${tdRealStrokes}
                                ${tdIdealCycleTimePerTool}
                                ${tdOEE}
                                ${tdStatusDM}
                            </tr>`;

        return rowHtml;
    }

    // Limpia el formulario
    function clearMaterialForm(columns) {
        $("#materialIndex").val("");
        $("#materialId").val("");

        for (let col of columns) {
            if (col.type && col.type === "dropdown") {
                $(col.selector).val("").trigger("change");
            } else {
                $(col.selector).val("");
                $(col.selector).css("background-color", "");
            }
            $(col.selector).css("border", "");
            $(col.selector + "Error").text("").hide();
        }
        //datos fuera del columns
        // --- LÓGICA DE LIMPIEZA MEJORADA ---

        updateWeightCalculations();
        // 1. Limpiar los campos que no están en columnDefs
        $("#ID_Theoretical_Blanking_Line").val("");
        $("#parts_auto").val("");
        updateStrokesPerAuto();
        updateBlanksPerYear();
        updateMinMaxReales();
        $("#ID_File_Packaging").val("");
        $("#ID_File_CAD_Drawing").val("");
        $("#PackagingFileName").val("");
        $("#CADFileName").val("");
        $("#ID_File_Additional").val("");
        $("#AdditionalFileName").val("");
        $("#ID_File_ArrivalAdditional").val("");
        $("#ArrivalAdditionalFileName").val("");
        $("#ID_File_CoilDataAdditional").val(""); $("#CoilDataAdditionalFileName").val("");
        $("#ID_File_SlitterDataAdditional").val(""); $("#SlitterDataAdditionalFileName").val("");
        $("#ID_File_VolumeAdditional").val(""); $("#VolumeAdditionalFileName").val("");
        $("#ID_File_OutboundFreightAdditional").val(""); $("#OutboundFreightAdditionalFileName").val("");
        $("#ID_File_DeliveryPackagingAdditional").val(""); $("#DeliveryPackagingAdditionalFileName").val("");
        $("#InterplantPackageWeight").val("");
        $('#InterplantSpecialRequirement, #InterplantSpecialPackaging, #InterplantDeliveryConditions').val('');
        $('#interplant_packaging_archivo, #interplantOutboundFreightAdditionalFile').val('');
        $('#IsInterplantReturnableRack, #InterplantScrapReconciliation, #InterplantHeadTailReconciliation').prop('checked', false);
        $('#InterplantReturnableUses').val('');
        $('#ID_Interplant_FreightType').val('').trigger('change');
        $('#InterplantScrapReconciliationPercent_container').find('input').val('');
        $('#InterplantHeadTailReconciliationPercent_container').find('input').val('');
        // Limpiar también los errores asociados si existen
        $("#InterplantPackageWeightError").text("").hide();
        $("#InterplantPiecesPerPackageError").text("").hide(); // Limpia error pieces
        $("#InterplantStacksPerPackageError").text("").hide(); // Limpia error stacks

        updatePackagingFileUI();
        updateFileUIGeneric('ID_File_TechnicalSheet', 'TechnicalSheetFileName', 'file_container_technicalSheetFile', 'fileActions_containerTechnicalSheetFile', 'downloadFileTechnicalSheetFile');
        updateFileUIGeneric('ID_File_Additional', 'AdditionalFileName', 'file_container_AdditionalFile', 'fileActions_containerAdditionalFile', 'downloadFileAdditional');
        updateFileUIGeneric('ID_File_ArrivalAdditional', 'ArrivalAdditionalFileName', 'file_container_arrivalAdditionalFile', 'fileActions_containerArrivalAdditionalFile', 'downloadArrivalAdditionalFile');
        updateFileUIGeneric('ID_File_CoilDataAdditional', 'CoilDataAdditionalFileName', 'file_container_coilDataAdditionalFile', 'fileActions_containerCoilDataAdditionalFile', 'downloadCoilDataAdditionalFile');
        updateFileUIGeneric('ID_File_SlitterDataAdditional', 'SlitterDataAdditionalFileName', 'file_container_slitterDataAdditionalFile', 'fileActions_containerSlitterDataAdditionalFile', 'downloadSlitterDataAdditionalFile');
        updateFileUIGeneric('ID_File_VolumeAdditional', 'VolumeAdditionalFileName', 'file_container_volumeAdditionalFile', 'fileActions_containerVolumeAdditionalFile', 'downloadVolumeAdditionalFile');
        updateFileUIGeneric('ID_File_OutboundFreightAdditional', 'OutboundFreightAdditionalFileName', 'file_container_outboundFreightAdditionalFile', 'fileActions_containerOutboundFreightAdditionalFile', 'downloadOutboundFreightAdditionalFile');
        updateFileUIGeneric('ID_File_DeliveryPackagingAdditional', 'DeliveryPackagingAdditionalFileName', 'file_container_deliveryPackagingAdditionalFile', 'fileActions_containerDeliveryPackagingAdditionalFile', 'downloadDeliveryPackagingAdditionalFile');
        updateFileUIGeneric('ID_File_InterplantPackaging', 'InterplantPackagingFileName', 'file_container_interplant_packaging_archivo', 'fileActions_containerInterplantPackagingFile', 'downloadInterplantPackagingFile');
        updateFileUIGeneric('ID_File_InterplantOutboundFreight', 'InterplantOutboundFreightFileName', 'file_container_interplantOutboundFreightAdditionalFile', 'fileActions_containerInterplantOutboundFreightFile', 'downloadInterplantOutboundFreightFile');

        toggleRunningChangeWarning();

        $('#IsWeldedBlank').prop('checked', false).trigger('change'); // Esto oculta y limpia los campos de platinas
        $('#numberOfPlates').val('');
        $('#weldedPlatesThickness_container').empty().hide();
        $('#weldedPlatesJson').val('');

        $("#ID_Slitting_Line").val("");
        $("#slitting_line_name").val("");

        // 2. Limpiar TODOS los checkboxes con un solo selector
        $('.rack-type-checkbox, .additional-checkbox, .label-checkbox, .strap-checkbox').prop('checked', false);
        $('#interplant-rack-types-container .interplant-rack-type-checkbox').prop('checked', false);
        $('#interplant-labels-container .form-check-input').prop('checked', false);
        $('#interplant-labels-container .form-check-input').prop('checked', false);
        $('#interplant-additionals-container .form-check-input').prop('checked', false);
        $('#interplant-straps-container .form-check-input').prop('checked', false);
        // 3. Ocultar y limpiar los campos de "Other"
        $('#AdditionalsOtherDescription_container, #LabelOtherDescription_container, #InterplantLabelOtherDescription_container').hide();
        $('#AdditionalsOtherDescription, #LabelOtherDescription, #InterplantLabelOtherDescription').val('');
        $('#InterplantAdditionalsOtherDescription_container, #InterplantStrapTypeObservations').closest('.other-description-wrapper').hide();
        $('#InterplantAdditionalsOtherDescription, #InterplantStrapTypeObservations').val('');

        $('#InterplantScrapReconciliation').prop('checked', false);
        $('#InterplantHeadTailReconciliation').prop('checked', false);
        // Limpiamos los inputs de %
        $('#InterplantScrapReconciliationPercent_container').find('input').val('');
        $('#InterplantHeadTailReconciliationPercent_container').find('input').val('');
        // Disparamos los handlers para que oculten los contenedores
        handleInterplantScrapReconciliationChange();
        handleInterplantHeadTailReconciliationChange();

        // Dispara los handlers para ocultar los contenedores
        handleInterplantReturnableRackChange(); // Función nueva

        // Limpia los file inputs genéricos
        clearFileUIGeneric('ID_File_InterplantPackaging', 'interplant_packaging_archivo', 'file_container_interplant_packaging_archivo', 'fileActions_containerInterplantPackagingFile', 'interplant_packaging_archivoCancelButton');
        clearFileUIGeneric('ID_File_InterplantOutboundFreight', 'interplantOutboundFreightAdditionalFile', 'file_container_interplantOutboundFreightAdditionalFile', 'fileActions_containerInterplantOutboundFreightFile', 'interplantOutboundFreightAdditionalFileCancelButton');

        $('#InitialWeightPerPart').val('');
        $('#ShippingTons').val('');
        // También limpiamos los otros campos calculados para consistencia
        $('#WeightPerPart').val('');
        $('#Initial_Weight').val('');
        $('#AnnualTonnage').val('');

        if (canEditSales) {
            $("#PassesThroughSouthWarehouse").prop('disabled', false);
        }

        $("#Theoretical_Effective_Strokes").val("");
        $("#Real_Effective_Strokes").val("");

        // 4. Resetear el botón principal
        $('.btn-add-material').html('<i class="fa-solid fa-plus"></i> Add Material');

        $("#Initial_Weight").val("");
        // Campos de transporte de llegada
        $("#Arrival_Transport_Type_Other_container").hide();
        $("#Arrival_Transport_Type_Other").val('');

    }

    function handleInterplantReturnableRackChange() {
        const usesContainer = $('#InterplantReturnableUses_container');
        if ($('#IsInterplantReturnableRack').is(':checked')) {
            usesContainer.slideDown();
        } else {
            usesContainer.slideUp();
            usesContainer.find('input').val('');
            validateInterplantReturnableUses(); // Limpiar error
        }
    }

    // Función genérica para limpiar un file input
    function clearFileUIGeneric(idField, inputFileId, fileContainerId, actionsContainerId, cancelBtnId) {
        $("#" + idField).val("");
        $("#" + inputFileId).val("").prop("disabled", !canEditSales);
        $("#" + fileContainerId).show();
        $("#" + actionsContainerId).hide();
        $("#" + cancelBtnId).hide();
    }


    // --- INICIO DE LA MODIFICACIÓN ---
    function handleInterplantScrapReconciliationChange() {
        const percentContainer = $('#InterplantScrapReconciliationPercent_container');
        if ($('#InterplantScrapReconciliation').is(':checked')) {
            percentContainer.slideDown();
        } else {
            percentContainer.slideUp();
            // Opcional: Limpiamos los inputs si se desmarca
            // percentContainer.find('input').val('');
        }
    }

    function handleInterplantHeadTailReconciliationChange() {
        const percentContainer = $('#InterplantHeadTailReconciliationPercent_container');
        if ($('#InterplantHeadTailReconciliation').is(':checked')) {
            percentContainer.slideDown();
        } else {
            percentContainer.slideUp();
            // Opcional: Limpiamos los inputs si se desmarca
            // percentContainer.find('input').val('');
        }
    }

    function updateInterplantReturnableFieldsVisibility() {
        console.log("%c--- updateInterplantReturnableFieldsVisibility [START] ---", "color: #009ff5; font-weight: bold;");

        const $returnableContainer = $("#IsInterplantReturnableRack_container");
        const $returnableCheckbox = $("#IsInterplantReturnableRack");

        // 1. Obtenemos los IDs de los checkboxes de Interplant Rack Type que están marcados.
        const selectedRackIds = $('.interplant-rack-type-checkbox:checked').map(function () {
            return parseInt($(this).val(), 10);
        }).get();

        console.log("Interplant Rack IDs seleccionados:", selectedRackIds);
        console.log("Racks que permiten 'Returnable':", window.showReturnableOptionFor); // Usamos la constante global [3, 2, 4]

        // 2. Comprobamos si alguno de los seleccionados está en nuestra lista de permitidos.
        // Usamos la constante global 'showReturnableOptionFor' definida en page.constants.js [cite: 4, 770]
        const shouldShowReturnableOption = selectedRackIds.some(id => window.showReturnableOptionFor.includes(id));

        console.log("¿Debería mostrarse 'Interplant Returnable'?", shouldShowReturnableOption);

        // 3. Mostramos u ocultamos el campo "Returnable Rack"
        if (shouldShowReturnableOption) {
            console.log("Resultado: MOSTRANDO 'Interplant Returnable Rack'.");
            $returnableContainer.slideDown();
            // No habilitamos el check, eso lo hace la lógica de permisos
        } else {
            console.log("Resultado: OCULTANDO 'Interplant Returnable Rack' y reseteando.");
            $returnableContainer.slideUp();
            // Si lo ocultamos, forzamos que se desmarque y disparamos 'change'
            // para que también se oculte el campo "Returnable Uses".
            $returnableCheckbox.prop('checked', false).trigger('change');
        }
        console.log("%c--- updateInterplantReturnableFieldsVisibility [END] ---", "color: #dc3545;");
    }

    function handleArrivalWarehouseChange() {
        const warehouseId = $("#ID_Arrival_Warehouse").val();
        const passSouthCheckbox = $("#PassesThroughSouthWarehouse");

        // Obtenemos el permiso desde la config global, ya que 'canEditSales' es local al IIFE
        const canEdit = window.pageConfig.permissions.canEditSales;

        // ID 2 = Almacén Sur
        if (warehouseId == "2") {
            // Si el almacén de llegada es Sur, desmarcamos y deshabilitamos
            passSouthCheckbox.prop('checked', false);
            passSouthCheckbox.prop('disabled', true);
        } else {
            // Si es otro almacén, habilitamos el check SOLO si el usuario tiene permisos
            if (canEdit) {
                passSouthCheckbox.prop('disabled', false);
            }
        }
    }

    // ---  Obtener rutas efectivas del proyecto ---
    function getEffectiveProjectRoutes() {
        // Usamos groupCollapsed para que no inunde la consola, puedes expandirlo haciendo clic
        console.groupCollapsed("🔍 DEBUG: Calculando Rutas Efectivas");

        let uniqueRoutes = new Set();

        // 1. Obtener datos del formulario actual
        const formRouteId = parseInt($("#ID_Route").val(), 10) || 0;
        // Convertimos a String explícitamente para evitar errores de comparación (null vs "")
        const editingIndex = String($("#materialIndex").val() || "");

        console.log(`1. Estado del Formulario:`);
        console.log(`   - Ruta Seleccionada (Dropdown): ${formRouteId}`);
        console.log(`   - Índice en Edición (Hidden): "${editingIndex}" (vacío = nuevo)`);

        // 2. Escanear la tabla
        console.log(`2. Escaneando tabla (#materialsTable tbody tr)...`);

        $("#materialsTable tbody tr").each(function () {
            const row = $(this);
            const rowIndex = String(row.data("index"));

            // Buscamos el input hidden. Agregamos log si no lo encuentra.
            const inputRoute = row.find("input[name$='.ID_Route']");
            const savedRouteId = parseInt(inputRoute.val(), 10) || 0;

            let statusMsg = "";

            // LÓGICA DE EXCLUSIÓN
            if (editingIndex !== "" && rowIndex === editingIndex) {
                statusMsg = "❌ IGNORADA (En Edición)";
            } else {
                if (savedRouteId > 0) {
                    uniqueRoutes.add(savedRouteId);
                    statusMsg = "✅ AGREGADA";
                } else {
                    statusMsg = "⚠️ IGNORADA (ID 0 o inválido)";
                }
            }

            console.log(`   -> Fila [${rowIndex}]: Ruta Guardada=${savedRouteId} | ${statusMsg}`);
        });

        // 3. Agregar la ruta del formulario actual
        if (formRouteId > 0) {
            uniqueRoutes.add(formRouteId);
            console.log(`3. Agregando Ruta del Formulario: ${formRouteId}`);
        }

        const finalRoutes = Array.from(uniqueRoutes);
        console.log("🏁 RUTAS FINALES CALCULADAS:", finalRoutes);
        console.groupEnd(); // Fin del grupo de logs

        return finalRoutes;
    }


    //publicar las funciones
    window.handleHeadTailReconciliationChange = handleHeadTailReconciliationChange;
    window.handleScrapReconciliationChange = handleScrapReconciliationChange;
    window.handleWeldedBlankChange = handleWeldedBlankChange;
    window.updateInterplantPackageWeight = updateInterplantPackageWeight;
    window.toggleInterplantFacilityField = toggleInterplantFacilityField;
    window.shapeVisibility = shapeVisibility;
    window.updateTheoreticalStrokes = updateTheoreticalStrokes;
    window.updateTheoreticalGrossWeight = updateTheoreticalGrossWeight;
    window.debouncedUpdateCapacityGraphs = debouncedUpdateCapacityGraphs;
    window.UpdateCapacityGraphs = UpdateCapacityGraphs;
    window.handleDeliveryTransportTypeChange = handleDeliveryTransportTypeChange;
    window.generateCharts = generateCharts;
    window.handleInterplantDeliveryTransportTypeChange = handleInterplantDeliveryTransportTypeChange;
    window.handleInterplantPackagingStandardChange = handleInterplantPackagingStandardChange;
    window.buildShiftLegend = buildShiftLegend;
    window.handleArrivalProtectiveMaterialChange = handleArrivalProtectiveMaterialChange;
    window.handleStackableChange = handleStackableChange;
    window.handleArrivalTransportTypeChange = handleArrivalTransportTypeChange;
    window.updateAnnualTonnage = updateAnnualTonnage;
    window.updatePackageWeight = updatePackageWeight;
    window.updateCalculatedWeightFields = updateCalculatedWeightFields;
    window.fetchAndApplyOee = fetchAndApplyOee;
    window.updateDeliveryTransportationTypeState = updateDeliveryTransportationTypeState;
    window.handleReturnableRackChange = handleReturnableRackChange;
    window.updateReturnableFieldsVisibility = updateReturnableFieldsVisibility;
    window.updateFileUI = updateFileUI;
    window.updatePackagingFileUI = updatePackagingFileUI;
    window.updateFileUIGeneric = updateFileUIGeneric;
    window.updateRealStrokes = updateRealStrokes;
    window.updateStrokesPerAuto = updateStrokesPerAuto;
    window.updateMinMaxReales = updateMinMaxReales;
    window.updateBlanksPerYear = updateBlanksPerYear;
    window.updateMinMaxRealesOEE = updateMinMaxRealesOEE;
    window.updateActualShifts = updateActualShifts;
    window.updateStrokesShift = updateStrokesShift;
    window.updateLimitsDisplay = updateLimitsDisplay;
    window.updateVehicleVisibility = updateVehicleVisibility;
    window.updateAnnualVolumeStyle = updateAnnualVolumeStyle;
    window.showVolumeDifferenceToast = showVolumeDifferenceToast;
    window.toggleRunningChangeWarning = toggleRunningChangeWarning;
    window.updateWeightFieldsBasedOnShape = updateWeightFieldsBasedOnShape;
    window.UpdateCapacityHansontable = UpdateCapacityHansontable;
    window.recalculateVehicleData = recalculateVehicleData;
    window.updateMaterialTypeDropdown = updateMaterialTypeDropdown;
    window.loadAllMaterialTypesForPlant = loadAllMaterialTypesForPlant;
    window.updateTheoreticalLine = updateTheoreticalLine;
    window.updateWeightCalculations = updateWeightCalculations;
    window.fetchAndApplyValidationRanges = fetchAndApplyValidationRanges;
    window.setupConditionalTextareas = setupConditionalTextareas;
    window.toggleTkmmPackagingFields = toggleTkmmPackagingFields;
    //funciones
    window.debounce = debounce;
    window.debouncedUpdateSlitterChart = debouncedUpdateSlitterChart;
    window.redondearValor = redondearValor;
    window.generateThicknessInputs = generateThicknessInputs;
    window.convertToHandsontableMatrix = convertToHandsontableMatrix;
    window.parseYearMonth = parseYearMonth;
    window.renumberRows = renumberRows;
    window.validarContraCriterio = validarContraCriterio;
    window.describirLimites = describirLimites;
    window.syncAllVehicleSections = syncAllVehicleSections;
    window.syncSingleVehicleAndCountry = syncSingleVehicleAndCountry;
    window.loadVehiclesForDropdown = loadVehiclesForDropdown;
    window.loadRowDataIntoForm = loadRowDataIntoForm;
    window.buildRowHtml = buildRowHtml;
    window.clearMaterialForm = clearMaterialForm;
    window.requiresInterplant = requiresInterplant;
    window.handleHeadTailReconciliationChange = handleHeadTailReconciliationChange;
    window.handleScrapReconciliationChange = handleScrapReconciliationChange;
    window.handleInterplantScrapReconciliationChange = handleInterplantScrapReconciliationChange;
    window.handleInterplantHeadTailReconciliationChange = handleInterplantHeadTailReconciliationChange;
    window.handleInterplantReturnableRackChange = handleInterplantReturnableRackChange;
    window.clearFileUIGeneric = clearFileUIGeneric;
    window.updateInterplantReturnableFieldsVisibility = updateInterplantReturnableFieldsVisibility;
    window.debouncedUpdateCapacityHansontable = debouncedUpdateCapacityHansontable;
    window.updateEffectiveStrokes = updateEffectiveStrokes; 
    window.handleArrivalWarehouseChange = handleArrivalWarehouseChange;
    window.getEffectiveProjectRoutes = getEffectiveProjectRoutes; 
    //
})();