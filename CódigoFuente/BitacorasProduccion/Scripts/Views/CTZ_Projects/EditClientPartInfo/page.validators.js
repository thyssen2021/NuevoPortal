(function () {

    // Obtenemos las variables globales que necesitamos para las validaciones
    const config = window.pageConfig;
    const statusId = config.project.statusId;
    const projectId = config.project.id;
    const plantId = config.project.plantId;

    function validatePiecesPerPackage() {
        const input = $(config.fieldSelectors.PiecesPerPackage);
        const errorEl = $(config.fieldSelectors.PiecesPerPackage + "Error");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true; // Campo opcional

        let val = parseInt(raw, 10);
        if (isNaN(val) || val < 0 || raw.includes('.')) {
            errorEl.text("Must be a non-negative whole number.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validatePartName() {
        const input = $("#partName");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let partName = $("#partName").val().trim();
        $("#partNameError").text("").hide();
        $("#partName").css("border", "");
        if (!partName) {
            $("#partNameError").text("Part Name is required.").show();
            $("#partName").css("border", "1px solid red");
            return false;
        } else if (partName.length > 50) {
            $("#partNameError").text("Part Name cannot exceed 50 characters.").show();
            $("#partName").css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validatePassesThroughSouthWarehouse() {
        // Un checkbox opcional siempre es válido, ya sea que esté marcado o no.
        // No se necesita lógica de error.
        return true;
    }

    // Coloca esta función junto a tus otras funciones de validación
    function validateInterplant_Plant() {
        const input = $(config.fieldSelectors.ID_Interplant_Plant);
        const errorEl = $(config.fieldSelectors.ID_Interplant_Plant + "Error");

        // Si el campo no es visible o está deshabilitado, la validación pasa.
        if (!input.is(":visible") || input.prop("disabled")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }

        // Limpiar errores previos
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");

        // Si el proceso interplantas está activo, este campo es obligatorio.
        if (config.project.requiresInterplant && !input.val()) {
            errorEl.text("Interplant Facility is required.").show();
            input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateShearing_Width() {
        const input = $(config.fieldSelectors.Shearing_Width);
        const errorEl = $(config.fieldSelectors.Shearing_Width + "Error");
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Width_Tol_Pos() {
        const input = $(config.fieldSelectors.Shearing_Width_Tol_Pos);
        const errorEl = $(config.fieldSelectors.Shearing_Width_Tol_Pos + "Error");
        const mainInput = $(config.fieldSelectors.Shearing_Width);
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        let mainVal = parseFloat(mainInput.val());
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(mainVal) && val > mainVal) {
            errorEl.text("Tolerance cannot be greater than width.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Width_Tol_Neg() {
        const input = $(config.fieldSelectors.Shearing_Width_Tol_Neg);
        const errorEl = $(config.fieldSelectors.Shearing_Width_Tol_Neg + "Error");
        const mainInput = $(config.fieldSelectors.Shearing_Width);
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        let mainVal = parseFloat(mainInput.val());
        if (isNaN(val) || val > 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be <= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(mainVal) && Math.abs(val) > mainVal) {
            errorEl.text("Tolerance magnitude cannot be greater than width.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Pitch() {
        const input = $(config.fieldSelectors.Shearing_Pitch);
        const errorEl = $(config.fieldSelectors.Shearing_Pitch + "Error");
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Pitch_Tol_Pos() {
        const input = $(config.fieldSelectors.Shearing_Pitch_Tol_Pos);
        const errorEl = $(config.fieldSelectors.Shearing_Pitch_Tol_Pos + "Error");
        const mainInput = $(config.fieldSelectors.Shearing_Pitch);
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        let mainVal = parseFloat(mainInput.val());
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(mainVal) && val > mainVal) {
            errorEl.text("Tolerance cannot be greater than pitch.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Pitch_Tol_Neg() {
        const input = $(config.fieldSelectors.Shearing_Pitch_Tol_Neg);
        const errorEl = $(config.fieldSelectors.Shearing_Pitch_Tol_Neg + "Error");
        const mainInput = $(config.fieldSelectors.Shearing_Pitch);
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        let mainVal = parseFloat(mainInput.val());
        if (isNaN(val) || val > 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be <= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(mainVal) && Math.abs(val) > mainVal) {
            errorEl.text("Tolerance magnitude cannot be greater than pitch.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Weight() {
        const input = $(config.fieldSelectors.Shearing_Weight);
        const errorEl = $(config.fieldSelectors.Shearing_Weight + "Error");
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Weight_Tol_Pos() {
        const input = $(config.fieldSelectors.Shearing_Weight_Tol_Pos);
        const errorEl = $(config.fieldSelectors.Shearing_Weight_Tol_Pos + "Error");
        const mainInput = $(config.fieldSelectors.Shearing_Weight);
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        let mainVal = parseFloat(mainInput.val());
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(mainVal) && val > mainVal) {
            errorEl.text("Tolerance cannot be greater than weight.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Weight_Tol_Neg() {
        const input = $(config.fieldSelectors.Shearing_Weight_Tol_Neg);
        const errorEl = $(config.fieldSelectors.Shearing_Weight_Tol_Neg + "Error");
        const mainInput = $(config.fieldSelectors.Shearing_Weight);
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        let mainVal = parseFloat(mainInput.val());
        if (isNaN(val) || val > 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be <= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(mainVal) && Math.abs(val) > mainVal) {
            errorEl.text("Tolerance magnitude cannot be greater than weight.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }


    function validateShearing_Pieces_Per_Stroke() {
        const input = $(config.fieldSelectors.Shearing_Pieces_Per_Stroke);
        const errorEl = $(config.fieldSelectors.Shearing_Pieces_Per_Stroke + "Error");
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShearing_Pieces_Per_Car() {
        const input = $(config.fieldSelectors.Shearing_Pieces_Per_Car);
        const errorEl = $(config.fieldSelectors.Shearing_Pieces_Per_Car + "Error");
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }


    function validateRequiresRackManufacturing() { return true; } // Checkbox opcional, siempre válido
    function validateRequiresDieManufacturing() { return true; } // Checkbox opcional, siempre válido
    function validateIsCarryOver() {
        // Un checkbox opcional no requiere validación de error, siempre es válido.
        return true;
    }
    function validateClientScrapReconciliationPercent() {
        const input = $(config.fieldSelectors.ClientScrapReconciliationPercent);
        const errorEl = $(config.fieldSelectors.ClientScrapReconciliationPercent + "Error");
        if (!input.closest('.row').parent().is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be a number between 0 and 100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateClientHeadTailReconciliationPercent() {
        const input = $(config.fieldSelectors.ClientHeadTailReconciliationPercent);
        const errorEl = $(config.fieldSelectors.ClientHeadTailReconciliationPercent + "Error");
        if (!input.closest('.row').parent().is(":visible")) return true;
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true;
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be a number between 0 and 100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateWeightOfFinalMults_Min() {
        const input = $(config.fieldSelectors.WeightOfFinalMults_Min);
        const errorEl = $(config.fieldSelectors.WeightOfFinalMults_Min + "Error");
        const optimalInput = $(config.fieldSelectors.WeightOfFinalMults);
        const maxInput = $(config.fieldSelectors.WeightOfFinalMults_Max);

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true;

        let val = parseFloat(raw);
        let optimalVal = parseFloat(optimalInput.val());
        let maxVal = parseFloat(maxInput.val());

        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(optimalVal) && val > optimalVal) {
            errorEl.text("Min cannot be greater than Optimal.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(maxVal) && val > maxVal) {
            errorEl.text("Min cannot be greater than Max.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateWeightOfFinalMults_Max() {
        const input = $(config.fieldSelectors.WeightOfFinalMults_Max);
        const errorEl = $(config.fieldSelectors.WeightOfFinalMults_Max + "Error");
        const optimalInput = $(config.fieldSelectors.WeightOfFinalMults);
        const minInput = $(config.fieldSelectors.WeightOfFinalMults_Min);

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) return true;

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true;

        let val = parseFloat(raw);
        let optimalVal = parseFloat(optimalInput.val());
        let minVal = parseFloat(minInput.val());

        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(optimalVal) && val < optimalVal) {
            errorEl.text("Max cannot be less than Optimal.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(minVal) && val < minVal) {
            errorEl.text("Max cannot be less than Min.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateScrapReconciliationPercent_Min() {
        const input = $(config.fieldSelectors.ScrapReconciliationPercent_Min);
        const errorEl = $(config.fieldSelectors.ScrapReconciliationPercent_Min + "Error");
        const optimalInput = $(config.fieldSelectors.ScrapReconciliationPercent);
        const maxInput = $(config.fieldSelectors.ScrapReconciliationPercent_Max);

        if (!input.closest('.row').parent().is(":visible")) return true;

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true;

        let val = parseFloat(raw);
        let optimalVal = parseFloat(optimalInput.val());
        let maxVal = parseFloat(maxInput.val());

        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be between 0-100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(optimalVal) && val > optimalVal) {
            errorEl.text("Min cannot be > Optimal.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(maxVal) && val > maxVal) {
            errorEl.text("Min cannot be > Max.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateScrapReconciliationPercent_Max() {
        const input = $(config.fieldSelectors.ScrapReconciliationPercent_Max);
        const errorEl = $(config.fieldSelectors.ScrapReconciliationPercent_Max + "Error");
        const optimalInput = $(config.fieldSelectors.ScrapReconciliationPercent);
        const minInput = $(config.fieldSelectors.ScrapReconciliationPercent_Min);

        if (!input.closest('.row').parent().is(":visible")) return true;

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true;

        let val = parseFloat(raw);
        let optimalVal = parseFloat(optimalInput.val());
        let minVal = parseFloat(minInput.val());

        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be between 0-100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(optimalVal) && val < optimalVal) {
            errorEl.text("Max cannot be < Optimal.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(minVal) && val < minVal) {
            errorEl.text("Max cannot be < Min.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateHeadTailReconciliationPercent_Min() {
        const input = $(config.fieldSelectors.HeadTailReconciliationPercent_Min);
        const errorEl = $(config.fieldSelectors.HeadTailReconciliationPercent_Min + "Error");
        const optimalInput = $(config.fieldSelectors.HeadTailReconciliationPercent);
        const maxInput = $(config.fieldSelectors.HeadTailReconciliationPercent_Max);

        if (!input.closest('.row').parent().is(":visible")) return true;

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true;

        let val = parseFloat(raw);
        let optimalVal = parseFloat(optimalInput.val());
        let maxVal = parseFloat(maxInput.val());

        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be between 0-100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(optimalVal) && val > optimalVal) {
            errorEl.text("Min cannot be > Optimal.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(maxVal) && val > maxVal) {
            errorEl.text("Min cannot be > Max.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateHeadTailReconciliationPercent_Max() {
        const input = $(config.fieldSelectors.HeadTailReconciliationPercent_Max);
        const errorEl = $(config.fieldSelectors.HeadTailReconciliationPercent_Max + "Error");
        const optimalInput = $(config.fieldSelectors.HeadTailReconciliationPercent);
        const minInput = $(config.fieldSelectors.HeadTailReconciliationPercent_Min);

        if (!input.closest('.row').parent().is(":visible")) return true;

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true;

        let val = parseFloat(raw);
        let optimalVal = parseFloat(optimalInput.val());
        let minVal = parseFloat(minInput.val());

        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be between 0-100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(optimalVal) && val < optimalVal) {
            errorEl.text("Max cannot be < Optimal.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (!isNaN(minVal) && val < minVal) {
            errorEl.text("Max cannot be < Min.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateIsRunningChange() {
        // Un checkbox siempre es válido (true o false)
        // No se necesita lógica de error.
        return true;
    }
    function validateClientNetWeight() {
        const netInput = $(config.fieldSelectors.ClientNetWeight);
        const errorEl = $(config.fieldSelectors.ClientNetWeight + "Error");
        const grossInput = $(config.fieldSelectors.Gross_Weight);

        if (netInput.prop("readonly") || netInput.prop("disabled") || !netInput.is(":visible")) {
            errorEl.text("").hide();
            netInput.css("border", "");
            return true;
        }

        let rawNet = netInput.val() ? netInput.val().trim() : "";
        errorEl.text("").hide();
        netInput.css("border", "");

        if (!rawNet) return true; // El campo es opcional, si está vacío no hay error.

        let netVal = parseFloat(rawNet);
        if (isNaN(netVal) || netVal < 0) {
            errorEl.text(isNaN(netVal) ? "Must be a number." : "Must be >= 0.").show();
            netInput.css("border", "1px solid red");
            return false;
        }

        let rawGross = grossInput.val() ? grossInput.val().trim() : "";
        if (rawGross) {
            let grossVal = parseFloat(rawGross);
            if (!isNaN(grossVal) && netVal > grossVal) {
                errorEl.text("Net weight cannot be greater than Gross weight.").show();
                netInput.css("border", "1px solid red");
                return false;
            }
        }

        return true;
    }

    function validateFreightType() {
        const input = $(config.fieldSelectors.ID_FreightType);
        const errorEl = $(config.fieldSelectors.ID_FreightType + "Error");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }
        // (Validación opcional de obligatoriedad)
        // if (!input.val()) {
        //     errorEl.text("Freight Type is required.").show();
        //     input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
        //     return false;
        // }
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }

    function validateArrivalWarehouse() {
        const input = $(config.fieldSelectors.ID_Arrival_Warehouse);
        const errorEl = $(config.fieldSelectors.ID_Arrival_Warehouse + "Error");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }
        // (Validación opcional de obligatoriedad)
        // if (!input.val()) {
        //     errorEl.text("Arrival Warehouse is required.").show();
        //     input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
        //     return false;
        // }
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }
    function validateStacksPerPackage() {
        const input = $(config.fieldSelectors.StacksPerPackage);
        const errorEl = $(config.fieldSelectors.StacksPerPackage + "Error");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) return true; // Campo opcional

        let val = parseInt(raw, 10);
        if (isNaN(val) || val < 0 || raw.includes('.')) {
            errorEl.text("Must be a non-negative whole number.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateDeliveryConditions() {
        const input = $(config.fieldSelectors.DeliveryConditions);
        const errorEl = $(config.fieldSelectors.DeliveryConditions + "Error");
        const limit = 350;

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        if (val.length > limit) {
            errorEl.text(`Cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateArrivalTransportTypeOther() {
        const input = $(config.fieldSelectors.Arrival_Transport_Type_Other);
        const errorEl = $(config.fieldSelectors.Arrival_Transport_Type_Other + "Error");
        const container = $(config.fieldSelectors.Arrival_Transport_Type_Other + "_container");
        const limit = 50;

        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        const value = input.val().trim();
        errorEl.text("").hide();
        input.css("border", "");

        if (!value) {
            errorEl.text("Please specify the 'Other' transport type.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (value.length > limit) {
            errorEl.text(`Description cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateStackableLevels() {
        const input = $("#Stackable_Levels");
        const errorEl = $("#Stackable_LevelsError");
        const container = $("#Stackable_Levels_container");

        // Si no es visible, no se valida
        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        let val = input.val().trim();
        errorEl.text("").hide();
        input.css("border", "");

        if (!val) {
            errorEl.text("Levels are required when 'Is Stackable' is checked.").show();
            input.css("border", "1px solid red");
            return false;
        }

        let num = parseInt(val, 10);
        if (isNaN(num) || num < 0 || val.includes('.')) {
            errorEl.text("Must be a non-negative whole number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateArrivalComments() {
        const input = $("#Arrival_Comments");
        const errorEl = $("#Arrival_CommentsError");
        const limit = 250;

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        if (val.length > limit) {
            errorEl.text(`Comments cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateArrivalProtectiveMaterial() {
        const input = $("#ID_Arrival_Protective_Material");
        const errorEl = $("#ID_Arrival_Protective_MaterialError");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");

        if (!val || val === "") {
            errorEl.text("Protective Material is required.").show();
            input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
            return false;
        }

        // Disparar la validación del campo "Other" por si acaso
        validateArrivalProtectiveMaterialOther();
        return true;
    }

    function validateArrivalProtectiveMaterialOther() {
        const input = $("#Arrival_Protective_Material_Other");
        const errorEl = $("#Arrival_Protective_Material_OtherError");
        const container = $("#Arrival_Protective_Material_Other_container");

        // Si el contenedor no está visible, la validación no aplica
        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        let val = input.val().trim();
        errorEl.text("").hide();
        input.css("border", "");

        if (!val) {
            errorEl.text("Please specify the 'Other' material.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (val.length > 120) {
            errorEl.text("Cannot exceed 120 characters.").show();
            input.css("border", "1px solid red");
            return false;
        }
        return true;
    }
    function validateArrivalPackagingType() {
        const input = $("#ID_Arrival_Packaging_Type");
        const errorEl = $("#ID_Arrival_Packaging_TypeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH,
        ];

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");

        if (!val || val === "") {
            if (requiredStatus.includes(status)) {
                errorEl.text("Packaging Type is required.").show();
                input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
                return false;
            }
        }
        return true;
    }
    function validateArrivalTransportType() {
        const input = $("#ID_Arrival_Transport_Type");
        const errorEl = $("#ID_Arrival_Transport_TypeError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio (ajusta según tus reglas de negocio)
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH,
        ];

        // Si está deshabilitado o no es visible, es válido
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");

        // Validar selección si es un estatus requerido
        if (!val || val === "") {
            if (requiredStatus.includes(status)) {
                errorEl.text("Transport Type is required.").show();
                input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
                return false;
            }
        }

        return true;
    }

    function validateCoilPosition() {
        const input = $("#ID_Coil_Position");
        const errorEl = $("#ID_Coil_PositionError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH,
        ];

        // Si readonly o disabled, omitir validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        // Validar selección
        if (!val || val === "") {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text("Coil Position is required.")
                    .show();
                input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
                return false;
            }
            return true;
        }

        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }

    function validateWidth_Mults() {
        // 1) Elementos del DOM
        const input = $(config.fieldSelectors.Width_Mults);
        const errorEl = $(config.fieldSelectors.Width_Mults + "Error");

        // 2) Si está deshabilitado o no visible, es válido y se limpian errores.
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        // 3) Limpiar errores previos de esta validación específica.
        // Se hace esto para que la nueva validación de combinación pueda poner su propio error si es necesario.
        if (!errorEl.text().startsWith("Total width of mults")) {
            errorEl.text("").hide();
            input.css("border", "");
        }

        let raw = input.val() ? input.val().trim() : "";

        // 4) Si está vacío, es válido (campo opcional), pero debemos revalidar las otras funciones.
        if (!raw) {
            validateWidth_Mults_Tol_Neg();
            validateWidth_Mults_Tol_Pos();
            return validateWidthMultsCombination(); // Revalida la combinación para limpiar errores.
        }

        // 5) Validar que sea un número y no negativo
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 6) Después de validarse a sí mismo, re-valida sus tolerancias dependientes
        validateWidth_Mults_Tol_Neg();
        validateWidth_Mults_Tol_Pos();

        // 7) Finalmente, llamar a la validación de combinación que compara con el ancho del rollo.
        return validateWidthMultsCombination();
    }

    function validateWidth_Mults_Tol_Neg() {
        const input = $(config.fieldSelectors.Width_Mults_Tol_Neg);
        const errorEl = $(config.fieldSelectors.Width_Mults_Tol_Neg + "Error");
        const widthInput = $(config.fieldSelectors.Width_Mults);

        if (input.prop("readonly") || input.prop("disabled")) return true;

        errorEl.text("").hide();
        input.css("border", "");

        let raw = input.val() ? input.val().trim() : "";
        if (!raw) return true;

        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be <= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Validar que la tolerancia no sea mayor que el ancho
        let widthVal = parseFloat(widthInput.val());
        if (!isNaN(widthVal) && Math.abs(val) > widthVal) {
            errorEl.text(`Absolute value cannot exceed Width (${widthVal}).`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateWidth_Mults_Tol_Pos() {
        const input = $(config.fieldSelectors.Width_Mults_Tol_Pos);
        const errorEl = $(config.fieldSelectors.Width_Mults_Tol_Pos + "Error");
        const widthInput = $(config.fieldSelectors.Width_Mults);

        if (input.prop("readonly") || input.prop("disabled")) return true;

        errorEl.text("").hide();
        input.css("border", "");

        let raw = input.val() ? input.val().trim() : "";
        if (!raw) return true;

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Validar que la tolerancia no sea mayor que el ancho
        let widthVal = parseFloat(widthInput.val());
        if (!isNaN(widthVal) && val > widthVal) {
            errorEl.text(`Value cannot exceed Width (${widthVal}).`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateWidth_Plates() {
        const input = $(config.fieldSelectors.Width_Plates);
        const errorEl = $(config.fieldSelectors.Width_Plates + "Error");

        if (input.prop("readonly") || input.prop("disabled")) return true;

        errorEl.text("").hide();
        input.css("border", "");

        let raw = input.val() ? input.val().trim() : "";
        if (!raw) return true;

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Re-validar tolerancias dependientes
        validateWidth_Plates_Tol_Neg();
        validateWidth_Plates_Tol_Pos();

        return true;
    }

    function validateWidth_Plates_Tol_Neg() {
        const input = $(config.fieldSelectors.Width_Plates_Tol_Neg);
        const errorEl = $(config.fieldSelectors.Width_Plates_Tol_Neg + "Error");
        const widthInput = $(config.fieldSelectors.Width_Plates);

        if (input.prop("readonly") || input.prop("disabled")) return true;

        errorEl.text("").hide();
        input.css("border", "");

        let raw = input.val() ? input.val().trim() : "";
        if (!raw) return true;

        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be <= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        let widthVal = parseFloat(widthInput.val());
        if (!isNaN(widthVal) && Math.abs(val) > widthVal) {
            errorEl.text(`Absolute value cannot exceed Width (${widthVal}).`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateWidth_Plates_Tol_Pos() {
        const input = $(config.fieldSelectors.Width_Plates_Tol_Pos);
        const errorEl = $(config.fieldSelectors.Width_Plates_Tol_Pos + "Error");
        const widthInput = $(config.fieldSelectors.Width_Plates);

        if (input.prop("readonly") || input.prop("disabled")) return true;

        errorEl.text("").hide();
        input.css("border", "");

        let raw = input.val() ? input.val().trim() : "";
        if (!raw) return true;

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        let widthVal = parseFloat(widthInput.val());
        if (!isNaN(widthVal) && val > widthVal) {
            errorEl.text(`Value cannot exceed Width (${widthVal}).`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateTonsPerShift() {
        const input = $("#TonsPerShift");
        const errorEl = $("#TonsPerShiftError");

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            return true; // Es opcional, así que si está vacío, es válido.
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "The value must be a number." : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateStrapTypeObservations() {
        const input = $("#StrapTypeObservations");
        const errorEl = $("#StrapTypeObservationsError");
        const limit = 120;

        // Si el campo no está visible, no se valida
        if (!input.is(":visible")) {
            errorEl.text("").hide();
            return true;
        }

        const value = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        if (value.length > limit) {
            errorEl.text(`Observations cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAdditionalsOtherDescription() {
        const input = $("#AdditionalsOtherDescription");
        const errorEl = $("#AdditionalsOtherDescriptionError");
        const container = $("#AdditionalsOtherDescription_container"); // Apuntamos al contenedor
        const limit = 120;

        // Si el contenedor del campo NO es visible, la validación pasa automáticamente.
        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        // Si el contenedor SÍ es visible, entonces validamos.
        const value = input.val().trim();
        errorEl.text("").hide(); // Limpia errores previos
        input.css("border", "");

        // Regla 1: El campo no puede estar vacío
        if (!value) {
            errorEl.text("Please specify the 'Other' Additional description.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Regla 2: No puede exceder los 120 caracteres
        if (value.length > limit) {
            errorEl.text(`Description cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true; // Pasa todas las validaciones
    }

    function validateLabelOtherDescription() {
        const input = $("#LabelOtherDescription");
        const errorEl = $("#LabelOtherDescriptionError");
        const container = $("#LabelOtherDescription_container"); // Apuntamos al contenedor
        const limit = 120;

        // Si el contenedor del campo NO es visible, la validación pasa automáticamente.
        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        // Si el contenedor SÍ es visible, entonces validamos.
        const value = input.val().trim();
        errorEl.text("").hide(); // Limpia errores previos
        input.css("border", "");

        // Regla 1: El campo no puede estar vacío
        if (!value) {
            errorEl.text("Please specify the 'Other' Label description.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Regla 2: No puede exceder los 120 caracteres
        if (value.length > limit) {
            errorEl.text(`Description cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }

        return true; // Pasa todas las validaciones
    }

    function validateTurnOverSide() {
        // 1. Identificar los elementos con los que vamos a trabajar
        const turnOverSideInput = $(config.fieldSelectors.TurnOverSide);
        const turnOverCheckbox = $(config.fieldSelectors.TurnOver);
        const errorSpan = $(config.fieldSelectors.TurnOverSide + "Error");

        // 2. Si el input está deshabilitado o es de solo lectura, se omite la validación
        if (turnOverSideInput.prop("readonly") || turnOverSideInput.prop("disabled")) {
            return true;
        }

        // 3. Limpiar cualquier mensaje de error y estilo previo
        errorSpan.text("").hide();
        // Para Select2, es mejor aplicar el estilo al contenedor visual
        turnOverSideInput.next(".select2-container").find(".select2-selection").css("border", "");

        // 4. Lógica de validación principal
        // Verificamos si el checkbox 'TurnOver' está seleccionado
        if (turnOverCheckbox.is(":checked")) {

            // Si está seleccionado, TurnOverSide es obligatorio. Obtenemos su valor.
            let turnOverSideValue = (turnOverSideInput.val() || "").trim();

            // Si el valor está vacío, mostramos el error.
            if (!turnOverSideValue) {
                errorSpan.text("Side is required when 'Turn Over' is checked.").show();
                turnOverSideInput.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
                return false;
            }
        }

        // 5. Si el checkbox no está seleccionado, o si está seleccionado y el campo tiene valor, la validación es correcta.
        return true;
    }

    function validateInterplantPiecesPerPackage() {
        const input = $(config.fieldSelectors.InterplantPiecesPerPackage);
        const errorEl = $(config.fieldSelectors.InterplantPiecesPerPackage + "Error");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide(); input.css("border", ""); return true;
        }
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true; // Optional
        let val = parseInt(raw, 10);
        if (isNaN(val) || val < 0 || raw.includes('.')) {
            errorEl.text("Must be a non-negative whole number.").show(); input.css("border", "1px solid red"); return false;
        }
        return true;
    }

    function validateInterplantStacksPerPackage() {
        const input = $(config.fieldSelectors.InterplantStacksPerPackage);
        const errorEl = $(config.fieldSelectors.InterplantStacksPerPackage + "Error");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide(); input.css("border", ""); return true;
        }
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide(); input.css("border", "");
        if (!raw) return true; // Optional
        let val = parseInt(raw, 10);
        if (isNaN(val) || val < 0 || raw.includes('.')) {
            errorEl.text("Must be a non-negative whole number.").show(); input.css("border", "1px solid red"); return false;
        }
        return true;
    }
        
    function validateReturnableUses() {
        const input = $("#ReturnableUses");
        const errorEl = $("#ReturnableUsesError");

        // Si el campo no está visible, siempre es válido.
        if (!input.closest('.col-md-3').is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        // Comprobamos si alguno de los racks de madera está seleccionado.
        const isWoodRackSelected = usesMandatoryFor.some(id => $(`#rack-type-${id}`).is(':checked'));

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // El campo es REQUERIDO solo si un rack de madera está seleccionado.
        if (isWoodRackSelected && !raw) {
            errorEl.text("Number of uses is required for wood racks.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Si el campo tiene un valor (sea obligatorio u opcional), validamos su formato.
        if (raw) {
            let val = parseInt(raw, 10);
            if (isNaN(val) || val <= 0 || raw.includes('.')) {
                errorEl.text("Must be a positive whole number.").show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        return true; // Pasa todas las validaciones
    }

    function validateNumberOfPlates() {
        const input = $("#numberOfPlates");
        const errorEl = $("#numberOfPlatesError"); // Apuntamos al nuevo span
        const isChecked = $('#IsWeldedBlank').is(':checked');

        if (!isChecked || !input.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            errorEl.text("Required when 'Is Welded Blank' is checked.").show();
            input.css("border", "1px solid red");
            return false;
        }

        let val = parseInt(raw, 10);
        // Validamos que sea un número entero (sin decimales) entre 2 y 5
        if (isNaN(val) || val < 2 || val > 5 || raw.includes('.')) {
            errorEl.text("Must be a whole number between 2 and 5.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateWeldedThicknesses() {
        let isValid = true;
        if (!$('#IsWeldedBlank').is(':checked')) {
            return true;
        }

        $('.welded-thickness-input').each(function () {
            const input = $(this);
            const errorEl = input.next('.error-message');
            errorEl.text("").hide();
            input.css("border", "");

            let raw = input.val() ? input.val().trim() : "";
            if (!raw) {
                errorEl.text("Required").show();
                input.css("border", "1px solid red");
                isValid = false;
            } else {
                let val = parseFloat(raw);
                // Se cambia la condición para ser más explícito sobre números negativos
                if (isNaN(val) || val < 0) {
                    errorEl.text("Value cannot be negative.").show(); // Mensaje más claro
                    input.css("border", "1px solid red");
                    isValid = false;
                }
            }
        });
        return isValid;
    }

    function validateWidthMultsCombination() {
        const edgeTrim = 7; // Desorille en mm
        const widthMultsInput = $("#Width_Mults");
        const multipliersInput = $("#Multipliers");
        const widthInput = $("#Width");
        const errorEl = $("#Width_MultsError");

        // Si el campo no está visible o no tiene permiso de edición, la validación no aplica.
        if (!widthMultsInput.is(":visible") || widthMultsInput.prop("disabled")) {
            errorEl.text("").hide();
            widthMultsInput.css("border", "");
            return true;
        }

        // Obtenemos los valores numéricos
        const widthMults = parseFloat(widthMultsInput.val()) || 0;
        const multipliers = parseFloat(multipliersInput.val()) || 0;
        const width = parseFloat(widthInput.val()) || 0;

        // Si alguno de los valores necesarios no está presente, no podemos validar.
        if (widthMults === 0 || multipliers === 0 || width === 0) {
            // Limpiamos un posible error anterior de esta validación específica, pero no retornamos 'false'.
            // La validación de "campo requerido" se maneja en las funciones individuales.
            if (errorEl.text().startsWith("Total width of mults")) {
                errorEl.text("").hide();
                widthMultsInput.css("border", "");
            }
            return true;
        }

        // Realizamos el cálculo
        const totalCalculatedWidth = (widthMults * multipliers) + edgeTrim;

        // Comparamos y mostramos el error si es necesario
        if (totalCalculatedWidth > width) {
            errorEl.text(`Total width of mults (${totalCalculatedWidth.toFixed(2)}mm) cannot exceed coil width (${width.toFixed(2)}mm).`).show();
            widthMultsInput.css("border", "1px solid red");
            return false;
        }

        // Si la validación pasa, limpiamos el error (si era de esta validación).
        if (errorEl.text().startsWith("Total width of mults")) {
            errorEl.text("").hide();
            widthMultsInput.css("border", "");
        }

        return true;
    }

    function validateScrapReconciliationPercent() {
        const input = $("#ScrapReconciliationPercent");
        const errorEl = $("#ScrapReconciliationPercentError");
        const container = $("#ScrapReconciliationPercent_container");

        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        if (!raw) {
            errorEl.text("Percentage is required when 'Scrap reconciliation' is checked.").show();
            input.css("border", "1px solid red");
            return false;
        }
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be a number between 0 and 100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        errorEl.text("").hide();
        input.css("border", "");
        return true;
    }

    function validateHeadTailReconciliationPercent() {
        const input = $("#HeadTailReconciliationPercent");
        const errorEl = $("#HeadTailReconciliationPercentError");
        const container = $("#HeadTailReconciliationPercent_container");

        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }
        let raw = input.val() ? input.val().trim() : "";
        if (!raw) {
            errorEl.text("Percentage is required when 'Head/Tail reconciliation' is checked.").show();
            input.css("border", "1px solid red");
            return false;
        }
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0 || val > 100) {
            errorEl.text("Must be a number between 0 and 100.").show();
            input.css("border", "1px solid red");
            return false;
        }
        errorEl.text("").hide();
        input.css("border", "");
        return true;
    }

    function validateCADFile() {
        const input = $("#archivo");
        const errorSpan = $("#CADFileError");
        const maxSize = 10 * 1024 * 1024; // 10 MB en bytes
        const allowedExtensions = [".dwg", ".dxf", ".dwt", ".pdf", ".zip", ".rar"];

        let requiredStatus = [config.statusIDs.CasiCasi, config.statusIDs.POH];

        // Resetear mensajes y estilos
        errorSpan.text("").hide();
        input.css("border", "");

        // Si el input está oculto, no se valida nada
        if (!input.is(":visible")) {
            return true;
        }

        // Si el estado del proyecto NO está entre los obligatorios, no se exige tener un archivo (es opcional)
        if (requiredStatus.indexOf(statusId) === -1) {
            // Si no se seleccionó archivo, salimos sin error
            if (!input[0].files[0]) {
                return true;
            }
        } else {
            // Si el estado es de archivo obligatorio, se exige que se seleccione uno
            if (!input[0].files[0]) {
                errorSpan.text("CAD file is required for projects with status of 75% or higher.").show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        // Si se seleccionó un archivo, validar tamaño
        const file = input[0].files[0];
        if (file.size > maxSize) {
            errorSpan.text("File size must be 10MB or less.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Validar extensión (usa mayúsculas/minúsculas mediante toLowerCase())
        const fileName = file.name.toLowerCase();
        const isValidExtension = allowedExtensions.some(ext => fileName.endsWith(ext));
        if (!isValidExtension) {
            errorSpan.text("Only DWG, DXF, DWT, PDF, ZIP and RAR files are allowed.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validatePackagingFile() {
        // 1. Apuntar a los elementos HTML correctos
        const input = $("#packaging_archivo");
        const errorSpan = $("#PackagingFileError");

        // 2. Definir las reglas de validación
        const maxSize = 10 * 1024 * 1024; // 10 MB en bytes
        const allowedExtensions = [".dwg", ".dxf", ".dwt", ".pdf", ".zip", ".rar"];

        // 3. Resetear mensajes y estilos de error
        errorSpan.text("").hide();
        input.css("border", "");

        // 4. Si el campo no está visible, no se valida
        if (!input.is(":visible")) {
            return true;
        }

        // 5. Si no se ha seleccionado ningún archivo, es válido (es opcional)
        if (input[0].files.length === 0) {
            // NOTA: Si en el futuro este archivo se vuelve obligatorio bajo
            // ciertas condiciones, la lógica iría aquí, similar a como funciona
            // la validación de `statusId` en validateCADFile.
            return true;
        }

        // 6. Si se seleccionó un archivo, se procede a validar
        const file = input[0].files[0];

        // Validar el tamaño
        if (file.size > maxSize) {
            errorSpan.text("File size must not exceed 10MB.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Validar la extensión del archivo
        const fileName = file.name.toLowerCase();
        const isValidExtension = allowedExtensions.some(ext => fileName.endsWith(ext));
        if (!isValidExtension) {
            errorSpan.text("Only DWG, DXF, DWT, PDF, ZIP and RAR files are allowed.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // Si todas las validaciones pasan
        return true;
    }

    function validateVehicle() {
        const input = $("#Vehicle");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // DESPUÉS (Solución)
        // Se asegura de que vehicle sea siempre una cadena, incluso si .val() es null.
        let vehicle = ($("#Vehicle").val() || "").trim();

        $("#VehicleError").text("").hide();
        $("#Vehicle").css("border", "");

        if (!vehicle) {
            $("#VehicleError").text("Vehicle is required.").show();
            $("#Vehicle").css("border", "1px solid red");
            return false;
        }

        // Si NO es automotriz, validar longitud máxima
        if (vehicleType != 1 && vehicle.length > 120) {
            $("#VehicleError").text("Vehicle cannot exceed 120 characters.").show();
            $("#Vehicle").css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateRoute() {

        const input = $("#ID_Route");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let route = input.val();
        $("#ID_RouteError").text("").hide();
        $("#ID_Route").css("border", "");

        // Se asume que la opción por defecto tiene un valor vacío ("") o "Select a Route"
        if (!route || route === "Select a Route") {
            $("#ID_RouteError").text(config.displayNames.ID_Route + " is required.").show();
            $("#ID_Route").css("border", "1px solid red");
            return false;
        } else {
            return true;
        }
    }

    // Valida ID_InterplantDelivery_Coil_Position
    function validateInterplantDeliveryCoilPosition() {
        const input = $("#ID_InterplantDelivery_Coil_Position");
        const errorEl = $("#ID_InterplantDelivery_Coil_PositionError");

        // Si no está visible o habilitado, es válido
        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }

        // Por ahora es opcional. Si se vuelve obligatorio, agrega la lógica aquí.
        // const status = statusId;
        // let requiredStatus = [...];
        // if (requiredStatus.includes(status) && !input.val()) {
        //     errorEl.text("Interplant Coil Position is required.").show();
        //     input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
        //     return false;
        // }

        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }

    // Valida ID_InterplantDelivery_Transport_Type
    function validateInterplantDeliveryTransportType() {
        const input = $("#ID_InterplantDelivery_Transport_Type");
        const errorEl = $("#ID_InterplantDelivery_Transport_TypeError");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }
        // Opcional, pero se asegura de validar el campo "Other" si es necesario
        validateInterplantDeliveryTransportTypeOther();

        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }

    // Valida InterplantDelivery_Transport_Type_Other
    function validateInterplantDeliveryTransportTypeOther() {
        const input = $("#InterplantDelivery_Transport_Type_Other");
        const errorEl = $("#InterplantDelivery_Transport_Type_OtherError");
        const container = $("#InterplantDelivery_Transport_Type_Other_container");
        const limit = 50;

        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        const value = input.val().trim();
        if (!value) {
            errorEl.text("Please specify the 'Other' transport type.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (value.length > limit) {
            errorEl.text(`Cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }
        errorEl.text("").hide();
        input.css("border", "");
        return true;
    }

    // Valida InterplantPackagingStandard
    function validateInterplantPackagingStandard() {
        const input = $("#InterplantPackagingStandard");
        const errorEl = $("#InterplantPackagingStandardError");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }

        // Lógica opcional de obligatoriedad
        // if (!input.val()) {
        //     errorEl.text("Interplant Packaging Standard is required.").show();
        //     input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
        //     return false;
        // }

        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }

    // Valida InterplantRequiresRackManufacturing
    function validateInterplantRequiresRackManufacturing() {
        // Checkbox opcional, siempre válido
        return true;
    }

    // Valida InterplantRequiresDieManufacturing
    function validateInterplantRequiresDieManufacturing() {
        // Checkbox opcional, siempre válido
        return true;
    }

    function validateMaterialType() {
        const input = $("#ID_Material_type");
        const errorEl = $("#ID_Material_typeError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Si readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        // Validar selección
        if (!val || val === "Select Material Type") {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.ID_Material_type + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        return true;
    }

    function validateShape() {
        const input = $("#ID_Shape");
        const errorEl = $("#ID_ShapeError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Si readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        // Validar selección
        if (!val || val === "Select Shape") {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.ID_Shape + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        return true;
    }

    function validatePackagingStandard() {
        const input = $("#PackagingStandard");
        const errorEl = $("#PackagingStandardError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Si readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let val = input.val();
        errorEl.text("").hide();
        input.css("border", "");

        // Si no seleccionó nada o quedó en el texto por defecto...
        if (!val || val === "Select an option") {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.PackagingStandard + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            // opcional en otros estatus
            return true;
        }

        // Todo OK
        return true;
    }

    function validateRealLine() {
        const input = $("#ID_Real_Blanking_Line");
        const err = $("#ID_Real_Blanking_LineError");

        // 0) Actualiza antes el resto de los campos, por si no alcanzan a actualizarse porque no paso la validación
        debouncedUpdateTheoreticalLine();
        updateRealStrokes();

        // 1) Si está readonly o disabled, no validamos
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 2) Si el usuario PUEDE editar ingeniería, entonces este campo es obligatorio
        if (canEditEngineering) {
            const val = input.val();
            if (!val || val === "0") {
                err.text("Real blanking line is required.").show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        // 3) Si pasó la validación, ocultamos mensaje y borde rojo
        err.text("").hide();
        input.css("border", "");

        // 4) Limpiamos cualquier gráfico previo
        $("#capacityTableContainer").html("");

        return true;
    }

    function validateDeliveryTransportTypeOther() {
        const input = $("#Delivery_Transport_Type_Other");
        const errorEl = $("#Delivery_Transport_Type_OtherError");
        const container = $("#Delivery_Transport_Type_Other_container");
        const limit = 50;

        if (!container.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        const value = input.val().trim();
        if (!value) {
            errorEl.text("Please specify the 'Other' transport type.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (value.length > limit) {
            errorEl.text(`Cannot exceed ${limit} characters.`).show();
            input.css("border", "1px solid red");
            return false;
        }
        errorEl.text("").hide();
        input.css("border", "");
        return true;
    }

    function validateDeliveryCoilPosition() {
        // Esta validación es opcional, puedes hacerla requerida si lo necesitas.
        const input = $("#ID_Delivery_Coil_Position");
        const errorEl = $("#ID_Delivery_Coil_PositionError");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }
        // Ejemplo de validación (opcional):
        // if (!input.val()) {
        //     errorEl.text("Delivery Coil Position is required.").show();
        //     input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
        //     return false;
        // }
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        return true;
    }

    function validateDeliveryTransportType() {
        // Esta validación es opcional
        const input = $("#ID_Delivery_Transport_Type");
        const errorEl = $("#ID_Delivery_Transport_TypeError");

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.next(".select2-container").find(".select2-selection").css("border", "");
            return true;
        }
        // Ejemplo de validación (opcional):
        // if (!input.val()) {
        //     errorEl.text("Delivery Transport Type is required.").show();
        //     input.next(".select2-container").find(".select2-selection").css("border", "1px solid red");
        //     return false;
        // }
        errorEl.text("").hide();
        input.next(".select2-container").find(".select2-selection").css("border", "");
        // Disparar la validación del campo "Other" por si acaso
        validateDeliveryTransportTypeOther();
        return true;
    }

    function validateMaterial(fieldId) {
        console.log("Validando..." + fieldId)

        // Si no se especifica un campo, valida todos
        if (!fieldId) {
            //llama a la funcion de validacion de todos los campos para que valide el formulario completo de una sola vez y muestre todos los errores de formulario de una vez
            let validVehicleVersion = validateVehicleVersion();
            let validPartName = validatePartName();
            let validPartNumber = validatePartNumber();
            let validQuality = validateQuality();
            let validVehicle = validateVehicle();
            let validRealSOP = validateRealSOP();
            let validRealEOP = validateRealEOP();
            let validShipTo = validateShipTo();
            let validRoute = validateRoute();
            let validTensile = validateTensileStrength();
            let validMaterialType = validateMaterialType();
            let validThickness = validateThickness();
            let validWidth = validateWidth();
            let validPitch = validatePitch();
            let validGrossWeight = validateGrossWeight();
            let validAnnualVolume = validateAnnualVolume();
            let validVolumePerYear = validateVolumePerYear();
            let validShape = validateShape();
            let validAngleA = validateAngleA();
            let validAngleB = validateAngleB();
            let validBlanksPerStroke = validateBlanksPerStroke();
            let validPartsPerVehicle = validatePartsPerVehicle();
            let validRealLine = validateRealLine();
            let validIdealCycleTimePerTool = validateIdealCycleTimePerTool();
            let validOEE = validateOEE();
            let validTonsPerShift = validateTonsPerShift(); // <-- Añadir
            let validThicknessTolNegative = validateThicknessToleranceNegative();
            let validThicknessTolPositive = validateThicknessTolerancePositive();
            let validWidthTolNegative = validateWidthToleranceNegative();
            let validWidthTolPositive = validateWidthTolerancePositive();
            let validPitchTolNegative = validatePitchToleranceNegative();
            let validPitchTolPositive = validatePitchTolerancePositive();
            let validAngleATolNegative = validateAngleAToleranceNegative();
            let validAngleATolPositive = validateAngleATolerancePositive();
            let validAngleBTolNegative = validateAngleBToleranceNegative();
            let validAngleBTolPositive = validateAngleBTolerancePositive();
            let validMultipliers = validateMultipliers();
            let validMajorBase = validateMajorBase();
            let validMajorBaseToleranceNegative = validateMajorBaseToleranceNegative();
            let validMajorBaseTolerancePositive = validateMajorBaseTolerancePositive();
            let validMinorBase = validateMinorBase();
            let validMinorBaseToleranceNegative = validateMinorBaseToleranceNegative();
            let validMinorBaseTolerancePositive = validateMinorBaseTolerancePositive();
            let validFlatness = validateFlatness();
            let validFlatnessToleranceNegative = validateFlatnessToleranceNegative();
            let validFlatnessTolerancePositive = validateFlatnessTolerancePositive();
            let validMasterCoilWeight = validateMasterCoilWeight();
            let validInnerCoilDiameterArrival = validateInnerCoilDiameterArrival();
            let validOuterCoilDiameterArrival = validateOuterCoilDiameterArrival();
            let validInnerCoilDiameterDelivery = validateInnerCoilDiameterDelivery();
            let validOuterCoilDiameterDelivery = validateOuterCoilDiameterDelivery();
            let validPackagingStandard = validatePackagingStandard();
            let validSpecialRequirement = validateSpecialRequirement();
            let validSpecialPackaging = validateSpecialPackaging();
            let validTurnOverSide = validateTurnOverSide();
            let validCADFile = validateCADFile();
            let validPackagingFile = validatePackagingFile();
            let validStrapObs = validateStrapTypeObservations();
            let validAdditionalObs = validateAdditionalsOtherDescription();
            let validLabelObs = validateLabelOtherDescription();
            let validWidthMults = validateWidth_Mults();
            let validWidthMultsTolNeg = validateWidth_Mults_Tol_Neg();
            let validWidthMultsTolPos = validateWidth_Mults_Tol_Pos();
            let validWidthPlates = validateWidth_Plates();
            let validWidthPlatesTolNeg = validateWidth_Plates_Tol_Neg();
            let validWidthPlatesTolPos = validateWidth_Plates_Tol_Pos();
            let validCoilPosition = validateCoilPosition();
            let validArrivalTransportType = validateArrivalTransportType();
            let validArrivalPackagingType = validateArrivalPackagingType();
            let validArrivalProtectiveMaterial = validateArrivalProtectiveMaterial();
            let validArrivalProtectiveMaterialOther = validateArrivalProtectiveMaterialOther();
            let validStackableLevels = validateStackableLevels();
            let validArrivalComments = validateArrivalComments();
            let validArrivalTransportTypeOther = validateArrivalTransportTypeOther();
            let validPiecesPerPackage = validatePiecesPerPackage();
            let validStacksPerPackage = validateStacksPerPackage();
            let validDeliveryConditions = validateDeliveryConditions();
            let validReturnableUses = validateReturnableUses();
            let validDeliveryCoilPos = validateDeliveryCoilPosition();
            let validDeliveryTransport = validateDeliveryTransportType();
            let validDeliveryTransportOther = validateDeliveryTransportTypeOther();
            let validFreightType = validateFreightType();
            let validArrivalWarehouse = validateArrivalWarehouse();
            let validClientNetWeight = validateClientNetWeight();
            let validIsRunningChange = validateIsRunningChange();
            let validNumberOfPlates = validateNumberOfPlates();
            let validWeldedThicknesses = validateWeldedThicknesses();
            let validScrapRecPerc = validateScrapReconciliationPercent();
            let validScrapRecPercMin = validateScrapReconciliationPercent_Min();
            let validScrapRecPercMax = validateScrapReconciliationPercent_Max();
            let validHeadTailRecPerc = validateHeadTailReconciliationPercent();
            let validHeadTailRecPercMin = validateHeadTailReconciliationPercent_Min();
            let validHeadTailRecPercMax = validateHeadTailReconciliationPercent_Max();
            let validWeightOfFinalMults = validateWeightOfFinalMults();
            let validWeightOfFinalMultsMin = validateWeightOfFinalMults_Min();
            let validWeightOfFinalMultsMax = validateWeightOfFinalMults_Max();
            let validPassesThroughSouthWarehouse = validatePassesThroughSouthWarehouse();
            let validClientScrap = validateClientScrapReconciliationPercent();
            let validClientHeadTail = validateClientHeadTailReconciliationPercent();
            let validIsCarryOver = validateIsCarryOver();
            let validReqRack = validateRequiresRackManufacturing();
            let validReqDie = validateRequiresDieManufacturing();
            let validShearing_Width = validateShearing_Width();
            let validShearing_Width_Tol_Pos = validateShearing_Width_Tol_Pos();
            let validShearing_Width_Tol_Neg = validateShearing_Width_Tol_Neg();
            let validShearing_Pitch = validateShearing_Pitch();
            let validShearing_Pitch_Tol_Pos = validateShearing_Pitch_Tol_Pos();
            let validShearing_Pitch_Tol_Neg = validateShearing_Pitch_Tol_Neg();
            let validShearing_Weight = validateShearing_Weight();
            let validShearing_Weight_Tol_Pos = validateShearing_Weight_Tol_Pos();
            let validShearing_Weight_Tol_Neg = validateShearing_Weight_Tol_Neg();
            let validShearing_Pieces_Per_Stroke = validateShearing_Pieces_Per_Stroke();
            let validShearing_Pieces_Per_Car = validateShearing_Pieces_Per_Car();
            let validInterplantPlant = validateInterplant_Plant();
            let validInterplantPiecesPerPackage = validateInterplantPiecesPerPackage();
            let validInterplantStacksPerPackage = validateInterplantStacksPerPackage();

            return validVehicleVersion && validPartName && validPartNumber && validQuality && validVehicle
                && validRealSOP && validRealEOP && validShipTo && validRoute && validTensile && validMaterialType
                && validThickness && validWidth && validPitch && validGrossWeight && validAnnualVolume && validVolumePerYear
                && validShape && validAngleA && validAngleB && validBlanksPerStroke && validPartsPerVehicle && validRealLine
                && validIdealCycleTimePerTool && validOEE && validTonsPerShift && validThicknessTolNegative && validThicknessTolPositive
                && validWidthTolNegative && validWidthTolPositive && validPitchTolNegative && validPitchTolPositive
                && validAngleATolNegative && validAngleATolPositive && validAngleBTolNegative && validAngleBTolPositive
                && validMultipliers && validMajorBase && validMajorBaseToleranceNegative
                && validMajorBaseTolerancePositive && validMinorBase && validMinorBaseToleranceNegative && validMinorBaseTolerancePositive
                && validFlatness && validFlatnessToleranceNegative && validFlatnessTolerancePositive
                && validMasterCoilWeight && validInnerCoilDiameterArrival && validOuterCoilDiameterArrival
                && validInnerCoilDiameterDelivery && validOuterCoilDiameterDelivery && validPackagingStandard
                && validSpecialRequirement && validSpecialPackaging && validCADFile && validTurnOverSide && validPackagingFile
                && validSpecialPackaging && validTurnOverSide && validCADFile && validPackagingFile && validStrapObs && validAdditionalObs && validLabelObs
                && validWidthMults && validWidthMultsTolNeg && validWidthMultsTolPos && validWidthPlates && validWidthPlatesTolNeg && validWidthPlatesTolPos
                && validCoilPosition && validArrivalTransportType && validArrivalPackagingType && validArrivalProtectiveMaterial && validArrivalProtectiveMaterialOther
                && validStackableLevels && validArrivalComments && validArrivalTransportTypeOther && validPiecesPerPackage && validStacksPerPackage && validDeliveryConditions
                && validReturnableUses && validDeliveryCoilPos && validDeliveryTransport && validDeliveryTransportOther
                && validFreightType && validArrivalWarehouse && validClientNetWeight && validIsRunningChange && validNumberOfPlates && validWeldedThicknesses
                && validScrapRecPerc && validScrapRecPercMin && validScrapRecPercMax && validHeadTailRecPerc && validHeadTailRecPercMin && validHeadTailRecPercMax
                && validWeightOfFinalMults && validWeightOfFinalMultsMin && validWeightOfFinalMultsMax && validPassesThroughSouthWarehouse
                && validClientScrap && validClientHeadTail && validIsCarryOver && validReqRack && validReqDie && validShearing_Width && validShearing_Width_Tol_Pos
                && validShearing_Width_Tol_Neg && validShearing_Pitch && validShearing_Pitch_Tol_Pos && validShearing_Pitch_Tol_Neg
                && validShearing_Weight && validShearing_Weight_Tol_Pos && validShearing_Weight_Tol_Neg && validShearing_Pieces_Per_Stroke
                && validShearing_Pieces_Per_Car && validInterplantPlant && validateInterplantDeliveryCoilPosition() && validateInterplantDeliveryTransportType()
                && validateInterplantDeliveryTransportTypeOther() && validateInterplantPackagingStandard() && validateInterplantRequiresRackManufacturing()
                && validateInterplantRequiresDieManufacturing() && validInterplantPiecesPerPackage && validInterplantStacksPerPackage
                ;
        }

        // Validar sólo el campo indicado
        switch (fieldId) {
            case "Vehicle":
                return validateVehicle();
            case "vehicleVersion":
                return validateVehicleVersion();
            case "partName":
                return validatePartName();
            case "partNumber":
                return validatePartNumber();
            case "quality":
                return validateQuality();
            case "Real_SOP":
                return validateRealSOP();
            case "Real_EOP":
                return validateRealEOP();
            case "archivo":
                return validateCADFile();
            case "packaging_archivo":
                return validatePackagingFile();
            case "Ship_To":
                return validateShipTo();
            case "ID_Route":
                return validateRoute();
            case "Tensile_Strenght":
                return validateTensileStrength();
            case "ID_Material_type":
                return validateMaterialType();
            case "Thickness":
                return validateThickness();
            case "Width":
                return validateWidth();
            case "Pitch":
                return validatePitch();
            case "Gross_Weight":
                return validateGrossWeight();
            case "Annual_Volume":
                return validateAnnualVolume();
            case "Volume_Per_year":
                return validateVolumePerYear();
            case "ID_Shape":
                return validateShape();
            case "Angle_A":
                return validateAngleA();
            case "Angle_B":
                return validateAngleB();
            case "Blanks_Per_Stroke":
                return validateBlanksPerStroke();
            case "Parts_Per_Vehicle":
                return validatePartsPerVehicle();
            case "ID_Real_Blanking_Line":
                return validateRealLine();
            case "Ideal_Cycle_Time_Per_Tool":
                return validateIdealCycleTimePerTool();
            case "OEE":
                return validateOEE();
            case "ThicknessToleranceNegative":
                return validateThicknessToleranceNegative();
            case "ThicknessTolerancePositive":
                return validateThicknessTolerancePositive();
            case "WidthToleranceNegative":
                return validateWidthToleranceNegative();
            case "WidthTolerancePositive":
                return validateWidthTolerancePositive();
            case "PitchToleranceNegative":
                return validatePitchToleranceNegative();
            case "PitchTolerancePositive":
                return validatePitchTolerancePositive();
            case "AngleAToleranceNegative":
                return validateAngleAToleranceNegative();
            case "AngleATolerancePositive":
                return validateAngleATolerancePositive();
            case "AngleBToleranceNegative":
                return validateAngleBToleranceNegative();
            case "AngleBTolerancePositive":
                return validateAngleBTolerancePositive();
            case "Multipliers":
                return validateMultipliers();
            case "MajorBase":
                return validateMajorBase();
            case "MajorBaseToleranceNegative":
                return validateMajorBaseToleranceNegative();
            case "MajorBaseTolerancePositive":
                return validateMajorBaseTolerancePositive();
            case "MinorBase":
                return validateMinorBase();
            case "MinorBaseToleranceNegative":
                return validateMinorBaseToleranceNegative();
            case "MinorBaseTolerancePositive":
                return validateMinorBaseTolerancePositive();
            case "Flatness":
                return validateFlatness();
            case "FlatnessToleranceNegative":
                return validateFlatnessToleranceNegative();
            case "FlatnessTolerancePositive":
                return validateFlatnessTolerancePositive();
            case "MasterCoilWeight":
                return validateMasterCoilWeight();
            case "InnerCoilDiameterArrival":
                return validateInnerCoilDiameterArrival();
            case "OuterCoilDiameterArrival":
                return validateOuterCoilDiameterArrival();
            case "InnerCoilDiameterDelivery":
                return validateInnerCoilDiameterDelivery();
            case "OuterCoilDiameterDelivery":
                return validateOuterCoilDiameterDelivery();
            case "PackagingStandard":
                return validatePackagingStandard();
            case "SpecialRequirement":
                return validateSpecialRequirement();
            case "SpecialPackaging":
                return validateSpecialPackaging();
            case "TurnOverSide":
                return validateTurnOverSide();
            case "TonsPerShift":
                return validateTonsPerShift();
            case "StrapTypeObservations":
                return validateStrapTypeObservations();
            case "AdditionalsOtherDescription":
                return validateAdditionalsOtherDescription();
            case "LabelOtherDescription":
                return validateLabelOtherDescription();
            case "Width_Mults":
                return validateWidth_Mults();
            case "Width_Mults_Tol_Neg":
                return validateWidth_Mults_Tol_Neg();
            case "Width_Mults_Tol_Pos":
                return validateWidth_Mults_Tol_Pos();
            case "Width_Plates":
                return validateWidth_Plates();
            case "Width_Plates_Tol_Neg":
                return validateWidth_Plates_Tol_Neg();
            case "Width_Plates_Tol_Pos":
                return validateWidth_Plates_Tol_Pos();
            case "ID_Coil_Position":
                return validateCoilPosition();
            case "ID_Arrival_Transport_Type":
                return validateArrivalTransportType();
            case "Arrival_Transport_Type_Other":
                return validateArrivalTransportTypeOther();
            case "ID_Arrival_Packaging_Type":
                return validateArrivalPackagingType();
            case "ID_Arrival_Protective_Material":
                return validateArrivalProtectiveMaterial();
            case "Arrival_Protective_Material_Other":
                return validateArrivalProtectiveMaterialOther();
            case "Stackable_Levels":
                return validateStackableLevels();
            case "Arrival_Comments":
                return validateArrivalComments();
            case "PiecesPerPackage":
                return validatePiecesPerPackage();
            case "StacksPerPackage":
                return validateStacksPerPackage();
            case "DeliveryConditions":
                return validateDeliveryConditions();
            case "IsReturnableRack":
                return validateReturnableUses(); // Validar el hijo cuando el padre cambia
            case "ReturnableUses":
                return validateReturnableUses();
            case "ScrapReconciliation":
                return validateScrapReconciliationPercent(); // Validar el hijo
            case "ScrapReconciliationPercent":
                return validateScrapReconciliationPercent();
            case "ID_Delivery_Coil_Position":
                return validateDeliveryCoilPosition();
            case "ID_Delivery_Transport_Type":
                return validateDeliveryTransportType();
            case "Delivery_Transport_Type_Other":
                return validateDeliveryTransportTypeOther();
            case "ID_FreightType":
                return validateFreightType();
            case "ID_Arrival_Warehouse":
                return validateArrivalWarehouse();
            case "ClientNetWeight":
                return validateClientNetWeight();
            case "isRunningChange": return validateIsRunningChange();
            case "IsWeldedBlank": return validateNumberOfPlates() && validateWeldedThicknesses();
            case "numberOfPlates": return validateNumberOfPlates() && validateWeldedThicknesses();
            case "ScrapReconciliationPercent": return validateScrapReconciliationPercent() && validateScrapReconciliationPercent_Min() && validateScrapReconciliationPercent_Max();
            case "ScrapReconciliationPercent_Min": return validateScrapReconciliationPercent_Min();
            case "ScrapReconciliationPercent_Max": return validateScrapReconciliationPercent_Max();
            case "HeadTailReconciliationPercent": return validateHeadTailReconciliationPercent() && validateHeadTailReconciliationPercent_Min() && validateHeadTailReconciliationPercent_Max();
            case "HeadTailReconciliationPercent_Min": return validateHeadTailReconciliationPercent_Min();
            case "HeadTailReconciliationPercent_Max": return validateHeadTailReconciliationPercent_Max();
            case "WeightOfFinalMults": return validateWeightOfFinalMults() && validateWeightOfFinalMults_Min() && validateWeightOfFinalMults_Max();
            case "WeightOfFinalMults_Min": return validateWeightOfFinalMults_Min();
            case "WeightOfFinalMults_Max": return validateWeightOfFinalMults_Max();
            case "PassesThroughSouthWarehouse": return validatePassesThroughSouthWarehouse();
            case "ClientScrapReconciliationPercent": return validateClientScrapReconciliationPercent();
            case "ClientHeadTailReconciliationPercent": return validateClientHeadTailReconciliationPercent();
            case "IsCarryOver": return validateIsCarryOver();
            case "RequiresRackManufacturing": return validateRequiresRackManufacturing();
            case "RequiresDieManufacturing": return validateRequiresDieManufacturing();
            case "Shearing_Width": return validateShearing_Width();
            case "Shearing_Width_Tol_Pos": return validateShearing_Width_Tol_Pos();
            case "Shearing_Width_Tol_Neg": return validateShearing_Width_Tol_Neg();
            case "Shearing_Pitch": return validateShearing_Pitch();
            case "Shearing_Pitch_Tol_Pos": return validateShearing_Pitch_Tol_Pos();
            case "Shearing_Pitch_Tol_Neg": return validateShearing_Pitch_Tol_Neg();
            case "Shearing_Weight": return validateShearing_Weight();
            case "Shearing_Weight_Tol_Pos": return validateShearing_Weight_Tol_Pos();
            case "Shearing_Weight_Tol_Neg": return validateShearing_Weight_Tol_Neg();
            case "Shearing_Pieces_Per_Stroke": return validateShearing_Pieces_Per_Stroke();
            case "Shearing_Pieces_Per_Car": return validateShearing_Pieces_Per_Car();
            case "ID_Interplant_Plant": return validateInterplant_Plant();
            case "ID_InterplantDelivery_Coil_Position":
                return validateInterplantDeliveryCoilPosition();
            case "ID_InterplantDelivery_Transport_Type":
                return validateInterplantDeliveryTransportType();
            case "InterplantDelivery_Transport_Type_Other":
                return validateInterplantDeliveryTransportTypeOther();
            case "InterplantPackagingStandard":
                return validateInterplantPackagingStandard();
            case "InterplantRequiresRackManufacturing":
                return validateInterplantRequiresRackManufacturing();
            case "InterplantRequiresDieManufacturing":
                return validateInterplantRequiresDieManufacturing();
            default:
                return true;
        }
    }

    function updateSlitterCapacityChart(OnlyBDMaterials = false, projectId) {
        // Limpiamos la consola para tener una salida limpia en cada ejecución de "what-if"
        if (!OnlyBDMaterials) {
            //console.clear();
            console.log("%c--- What-If Slitter Calculation [START] ---", "color: #009ff5; font-weight: bold;");
        }

        const projectIdAjax = config.project.id;

        const container = $("#slitterChartContainer");
        container.html("<p style='color:gray;'>Loading Slitter Capacity <i class='fa-solid fa-spinner fa-spin-pulse'></i></p>").slideDown();
              
        let ajaxData = {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(), // AÑADIDO: Token de seguridad
            projectId: projectIdAjax, // <-- AÑADIDO
            plantId: config.project.plantId,
            applyDateFilter: true,
            OnlyBDMaterials: OnlyBDMaterials
        };



        // Si es un escenario "what-if", recolectamos y enviamos los datos del formulario
        if (!OnlyBDMaterials) {
            let whatIfMaterials = [];
            const materialIdBeingEdited = parseInt($("#materialId").val(), 10) || 0; // 0 si es nuevo

            console.log(`Modo Simulación: Editando Material ID: ${materialIdBeingEdited === 0 ? 'NUEVO' : materialIdBeingEdited}`);

            // 1. Construimos la lista base con los datos guardados de CADA material en la tabla
            // --- CAMBIO INICIA: Lógica mejorada para construir el JSON ---
            $("#materialsTable tbody tr").each(function () {
                const row = $(this);
                const materialIdInRow = parseInt(row.data("material-id"), 10);

                let material = {
                    ID_Material: materialIdInRow,
                    Vehicle: row.find("input[name$='.Vehicle']").val(),
                    Vehicle_2: row.find("input[name$='.Vehicle_2']").val(),
                    Vehicle_3: row.find("input[name$='.Vehicle_3']").val(),
                    Vehicle_4: row.find("input[name$='.Vehicle_4']").val(),
                    ID_Route: parseInt(row.find("input[name$='.ID_Route']").val()) || null,
                    Initial_Weight: 0,
                    IsEdited: false,
                    Real_SOP: row.find("input[name$='.Real_SOP']").val() || null,
                    Real_EOP: row.find("input[name$='.Real_EOP']").val() || null,
                    SOP_SP: row.find("input[name$='.SOP_SP']").val() || null,
                    EOP_SP: row.find("input[name$='.EOP_SP']").val() || null
                };

                // Calcular Initial_Weight basado en los datos de la fila
                // Calcular Initial_Weight para la fila de la tabla usando la NUEVA fórmula
                const annualVol = parseFloat(row.find("input[name$='.Annual_Volume']").val());
                const volPerYear = parseFloat(row.find("input[name$='.Volume_Per_year']").val());

                if (!isNaN(annualVol) && !isNaN(volPerYear) && annualVol !== 0) {
                    // 1. Calcula el peso base (Weight per Part)
                    const weightPerPart = (volPerYear / annualVol) * 1000;
                    // 2. Calcula el Initial Weight final sumando el 1.5%
                    material.Initial_Weight = weightPerPart * 1.015;
                }

                // Si el ID de esta fila coincide con el que está en el formulario,
                // significa que ESTE es el material que se está editando.
                if (materialIdInRow === materialIdBeingEdited && materialIdBeingEdited !== 0) {
                    material.IsEdited = true;
                    // Aquí, podrías sobreescribir los valores de la fila con los del formulario
                    // si quieres que la simulación sea 100% en tiempo real, pero la lógica actual ya lo hace bien
                    // al reemplazar el objeto completo en el siguiente paso.
                }

                whatIfMaterials.push(material);
            });

            const formData = {
                ID_Material: materialIdBeingEdited,
                Vehicle: $("#Vehicle").val(),
                Vehicle_2: $("#Vehicle_2").val(),
                Vehicle_3: $("#Vehicle_3").val(),
                Vehicle_4: $("#Vehicle_4").val(),
                ID_Route: parseInt($("#ID_Route").val()) || null,
                Initial_Weight: parseFloat($("#Initial_Weight").val()) || 0,
                IsEdited: true,
                Real_SOP: $("#Real_SOP").val() || null,
                Real_EOP: $("#Real_EOP").val() || null,
                SOP_SP: $("#SOP_SP").val() || null,
                EOP_SP: $("#EOP_SP").val() || null
            };

            if (materialIdBeingEdited !== 0) {
                // Modo Edición: Reemplazar el material existente en la lista.
                const index = whatIfMaterials.findIndex(m => m.ID_Material === materialIdBeingEdited);
                if (index > -1) {
                    whatIfMaterials[index] = formData;
                }
            } else {
                // Modo Nuevo: Agregar el material del formulario a la lista.
                whatIfMaterials.push(formData);
            }

            // 4. Mensaje de depuración final con el payload que se enviará
            console.log("Payload de materiales para simulación:", JSON.parse(JSON.stringify(whatIfMaterials)));

            ajaxData.whatIfDataJson = JSON.stringify(whatIfMaterials);
        }

        $.ajax({
            url: config.urls.GetSlitterCapacityData,
            type: 'POST',
            data: ajaxData,
            success: function (response) {
                if (!OnlyBDMaterials) console.log("Respuesta del servidor:", response);

                if (response.success && response.data && response.data.length > 0) {
                    generateCharts(response.data, {}, "#slitterChartContainer");
                } else {
                    container.html("<p style='color:red;'>Could not load Slitter capacity data.</p>");
                    if (response.message) toastr.error(response.message);
                }

                if (!OnlyBDMaterials) console.log("%c--- What-If Slitter Calculation [END] ---", "color: #009ff5; font-weight: bold;");
            },
            error: function (xhr, status, error) {
                container.html("<p style='color:red;'>Error fetching Slitter capacity data.</p>");
                if (!OnlyBDMaterials) console.log("%c--- What-If Slitter Calculation [ERROR] ---", "color: red; font-weight: bold;");
            }
        });
    }


    function validateRealSOP() {

        const input = $("#Real_SOP");
        const errorEl = $("#Real_SOPError");


        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // Limpiar cualquier mensaje de error y estilo previo.
        errorEl.text("").hide();
        input.css("border", "");

        let realSOPStr = input.val().trim();

        // Validación 1: Campo requerido.
        if (!realSOPStr) {
            errorEl.text("Real SOP is required.").show();
            input.css("border", "1px solid red");
            return false;
        }

        let realSOPDate = parseYearMonth(realSOPStr);

        // Validar si el formato de fecha es correcto.
        if (!realSOPDate) {
            errorEl.text("Invalid date format. Use yyyy-mm.").show();
            input.css("border", "1px solid red");
            return false;
        }
        // Validación 2: No puede ser de un año anterior al actual.
        const currentYear = new Date().getFullYear();
        if (realSOPDate.getFullYear() < currentYear) {
            // Se muestra un toast de advertencia en lugar de un error.
            toastr.warning(`Warning: Real SOP is in a year before the current year (${currentYear}).`, "Date Warning");

            // IMPORTANTE: Ya no se retorna 'false', por lo que la validación continúa.
        }
        // --- INICIO DE LA NUEVA LÓGICA ---

        // Validación 3: Debe ser anterior al Real EOP, si este existe.
        let realEOPStr = $("#Real_EOP").val().trim();
        if (realEOPStr) {
            let realEOPDate = parseYearMonth(realEOPStr);
            if (realEOPDate && realSOPDate >= realEOPDate) {
                errorEl.text("Real SOP must be before Real EOP.").show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        // --- FIN DE LA NUEVA LÓGICA ---

        // Validación 4 (Advertencia): Comparar con el SOP planeado si está disponible.
        let sopSPStr = $("#SOP_SP").val().trim();
        if (sopSPStr) {
            let sopSPDate = parseYearMonth(sopSPStr);
            if (sopSPDate && realSOPDate < sopSPDate) {
                toastr.warning("Warning: Real SOP is before the planned SOP.");
            }
        }


        return true;
    }

    function validateRealEOP() {
        const input = $("#Real_EOP");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let realEOPStr = $("#Real_EOP").val().trim();
        $("#Real_EOPError").text("").hide();
        $("#Real_EOP").css("border", "");

        if (!realEOPStr) {
            $("#Real_EOPError").text("Real EOP is required.").show();
            $("#Real_EOP").css("border", "1px solid red");
            return false;
        }

        // Obtener el valor de EOP_SP
        let eopSPStr = $("#EOP_SP").val().trim();
        let realEOPDate = parseYearMonth(realEOPStr);
        let eopSPDate = parseYearMonth(eopSPStr);

        if (realEOPDate && eopSPDate && realEOPDate > eopSPDate) {
            toastr.warning("Real EOP is later than the planned EOP.");
        }

        return true;
    }

    function validateTensileStrength() {
        const input = $("#Tensile_Strenght");
        const errorEl = $("#Tensile_StrenghtError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Si readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // Leer y limpiar
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // Si está vacío…
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Tensile_Strenght + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // Validar número ≥ 0
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        // Advertencia de rango (no bloqueante)
        if (window.engineeringRanges) {
            let criterio = window.engineeringRanges.find(r => r.ID_Criteria === 14);
            if (criterio && !validarContraCriterio(val, criterio)) {
                input.css("border", "1px solid red");
                errorEl.text(describirLimites(criterio)).show();
                return false;
            }
        }
        return true;
    }

    /////////////////////////////////
    function validateThickness() {
        // 1) IDs y estatus global
        const input = $("#Thickness");
        const errorEl = $("#ThicknessError");
        const status = statusId;  // tu variable global

        // 2) Estatus en los que Thickness es obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // 3) Si readonly o disabled, salto validación completa
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 4) Lectura y limpieza inicial
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // 5) Si está vacío...
        if (!raw) {
            // 5.a) Si el estatus exige que sea obligatorio → error
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Thickness + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            // 5.b) Si es opcional, permitimos vacío (no aplicamos más reglas)
            return true;
        }

        // 6) Intentamos parsear número
        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 7) Validación de no-negativo
        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 8) Aviso de rango mínimo/máximo (no bloqueante)
        if (window.engineeringRanges) {
            let criterio = window.engineeringRanges.find(r => r.ID_Criteria === 7);  //thickness
            if (criterio && !validarContraCriterio(val, criterio)) {
                input.css("border", "1px solid red");
                errorEl.text(describirLimites(criterio)).show();
                return false;
            }
        }
        // 9) Re-validar las tolerancias, ya que dependen de este valor

        validateThicknessToleranceNegative();
        validateThicknessTolerancePositive();
        // 9) Todo OK
        return true;
    }

    function validateThicknessToleranceNegative() {
        const input = $("#ThicknessToleranceNegative");
        const errorEl = $("#ThicknessToleranceNegativeError");
        const thicknessInput = $("#Thickness");
        const status = statusId;

        // 1) Define los estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // 2) Si está readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 3) Lectura y limpieza inicial
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // 4) Si no escribieron nada...
        if (!raw) {
            // 4.a) Si el estatus exige que sea obligatorio → error
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.ThicknessToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            // 4.b) Si es opcional, permitimos vacío
            return true;
        }

        // 5) Validar que sea número
        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl.text(isNaN(val) ? "The value must be a number." : "The value must be <= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 7) Validar que la tolerancia no sea mayor en valor absoluto que el espesor
        let thicknessVal = parseFloat(thicknessInput.val());
        if (!isNaN(thicknessVal) && Math.abs(val) > thicknessVal) {
            errorEl.text(`Absolute value cannot exceed Thickness (${thicknessVal} mm).`).show();
            input.css("border", "1px solid red");
            return false;
        }

        // 8) Todo OK
        return true;
    }

    function validateThicknessTolerancePositive() {
        const input = $("#ThicknessTolerancePositive");
        const errorEl = $("#ThicknessTolerancePositiveError");
        const thicknessInput = $("#Thickness");
        const status = statusId;

        // 1) Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // 2) Si está readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 3) Lectura y limpieza inicial
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // 4) Si no escribieron nada...
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.ThicknessTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // 5) Validar que sea número
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl.text(isNaN(val) ? "The value must be a number." : "The value must be >= 0.").show();
            input.css("border", "1px solid red");
            return false;
        }
        // 6) Validar que la tolerancia no sea mayor que el espesor
        let thicknessVal = parseFloat(thicknessInput.val());
        if (!isNaN(thicknessVal) && val > thicknessVal) {
            errorEl.text(`Value cannot exceed Thickness (${thicknessVal} mm).`).show();
            input.css("border", "1px solid red");
            return false;
        }

        // 7) Todo OK
        return true;
    }

    function validateWidthToleranceNegative() {
        const input = $("#WidthToleranceNegative");
        const errorEl = $("#WidthToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.WidthToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val > 0) {
            errorEl.text("The value must be lower than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateWidthTolerancePositive() {
        const input = $("#WidthTolerancePositive");
        const errorEl = $("#WidthTolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.WidthTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validatePitchToleranceNegative() {
        const input = $("#PitchToleranceNegative");
        const errorEl = $("#PitchToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.PitchToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val > 0) {
            errorEl.text("The value must be lower than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validatePitchTolerancePositive() {
        const input = $("#PitchTolerancePositive");
        const errorEl = $("#PitchTolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.PitchTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAngleAToleranceNegative() {
        const input = $("#AngleAToleranceNegative");
        const errorEl = $("#AngleAToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.AngleAToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be lower than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAngleATolerancePositive() {
        const input = $("#AngleATolerancePositive");
        const errorEl = $("#AngleATolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.AngleATolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAngleBToleranceNegative() {
        const input = $("#AngleBToleranceNegative");
        const errorEl = $("#AngleBToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.AngleBToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be lower than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAngleBTolerancePositive() {
        const input = $("#AngleBTolerancePositive");
        const errorEl = $("#AngleBTolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.AngleBTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateWeightOfFinalMults() {
        const input = $("#WeightOfFinalMults");
        const errorEl = $("#WeightOfFinalMultsError");
        const status = statusId;

        // 1) Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // 2) Si readonly/disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 3) Leer y limpiar
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // 4) Vacío → si es obligatorio, error; si no, OK
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.WeightOfFinalMults + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // 5) Validar número ≥ 0
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMultipliers() {
        const input = $("#Multipliers");
        const errorEl = $("#MultipliersError");
        const status = statusId;
        const isExternalSlitter = (hasExternalProcessor && externalProcessorId === 1);
        const requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        // Limpiar errores previos que no sean de la validación de combinación
        if (!errorEl.text().startsWith("Total width of mults")) {
            errorEl.text("").hide();
            input.css("border", "");
        }

        let raw = input.val() ? input.val().trim() : "";

        if (!raw) {
            if (isExternalSlitter) {
                return validateWidthMultsCombination(); // Revalida para limpiar errores
            }
            else if (requiredStatus.includes(status)) {
                errorEl.text(config.displayNames.Multipliers + " is required.").show();
                input.css("border", "1px solid red");
                return false;
            }
            return validateWidthMultsCombination(); // Revalida para limpiar errores
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val <= 0) {
            errorEl.text(isNaN(val) ? "Must be a number." : "Must be > 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (!isExternalSlitter) {
            if (window.maxMultsAllowed === null || window.maxMultsAllowed === 0) {
                errorEl.text("A validation rule is missing for the current Thickness and Tensile values.").show();
                input.css("border", "1px solid red");
                return false;
            }

            if (val > window.maxMultsAllowed) {
                errorEl.text(`Value cannot exceed the maximum of ${window.maxMultsAllowed} strips.`).show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        // Si todas las validaciones de este campo pasan, se ejecuta la validación de combinación
        return validateWidthMultsCombination();
    }

    function validateMajorBase() {
        const input = $("#MajorBase");
        const errorEl = $("#MajorBaseError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MajorBase + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMajorBaseToleranceNegative() {
        const input = $("#MajorBaseToleranceNegative");
        const errorEl = $("#MajorBaseToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MajorBaseToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be lower than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMajorBaseTolerancePositive() {
        const input = $("#MajorBaseTolerancePositive");
        const errorEl = $("#MajorBaseTolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MajorBaseTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMinorBase() {
        const input = $("#MinorBase");
        const errorEl = $("#MinorBaseError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Si está readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // Leer y limpiar
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // Vacío → si es obligatorio, error; si no, OK
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MinorBase + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // Validar número ≥ 0
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMinorBaseToleranceNegative() {
        const input = $("#MinorBaseToleranceNegative");
        const errorEl = $("#MinorBaseToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MinorBaseToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val > 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be lower than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMinorBaseTolerancePositive() {
        const input = $("#MinorBaseTolerancePositive");
        const errorEl = $("#MinorBaseTolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MinorBaseTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateFlatness() {
        const input = $("#Flatness");
        const errorEl = $("#FlatnessError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Flatness + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateFlatnessToleranceNegative() {
        const input = $("#FlatnessToleranceNegative");
        const errorEl = $("#FlatnessToleranceNegativeError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.FlatnessToleranceNegative + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val > 0) {
            errorEl.text("The value must be lower than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateFlatnessTolerancePositive() {
        const input = $("#FlatnessTolerancePositive");
        const errorEl = $("#FlatnessTolerancePositiveError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.FlatnessTolerancePositive + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateMasterCoilWeight() {
        const input = $("#MasterCoilWeight");
        const errorEl = $("#MasterCoilWeightError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.MasterCoilWeight + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // Validación de valor numérico y no negativo (sin cambios)
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be a positive number.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        // --- INICIO DE LA MODIFICACIÓN ---

        // 1. Verificamos si los rangos de validación ya se cargaron desde el servidor.
        if (window.engineeringRanges) {

            // 2. Buscamos el criterio específico para MasterCoilWeight (ID_Criteria: 17)
            const criterio = window.engineeringRanges.find(r => r.ID_Criteria === 17);

            // 3. Si encontramos el criterio y el valor no es válido...
            if (criterio && !validarContraCriterio(val, criterio)) {

                // 4. Mostramos el mensaje de error con los límites correctos y marcamos el campo.
                errorEl.text(describirLimites(criterio)).show();
                input.css("border", "1px solid red");
                return false; // La validación falla.
            }
        }

        // --- FIN DE LA MODIFICACIÓN ---

        return true;
    }

    function validateInnerCoilDiameterArrival() {
        const input = $("#InnerCoilDiameterArrival");
        const errorEl = $("#InnerCoilDiameterArrivalError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.InnerCoilDiameterArrival + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be a positive number.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        if (window.engineeringRanges) {
            // Buscamos el criterio para "Inner Coil Diameter" (ID_Criteria: 15)
            const criterio = window.engineeringRanges.find(r => r.ID_Criteria === 15);

            if (criterio) {
                // --- LÓGICA EXISTENTE CUANDO SE ENCUENTRA UN CRITERIO ---
                // Caso 1: El criterio define un único valor exacto.
                if (criterio.NumericValue != null) {
                    if (val !== criterio.NumericValue) {
                        errorEl.text(`Value must be exactly ${criterio.NumericValue}.`).show();
                        input.css("border", "1px solid red");
                        return false;
                    }
                }
                // Caso 2: El criterio define un rango, y solo los extremos son válidos.
                else if (criterio.MinValue != null && criterio.MaxValue != null) {
                    if (val !== criterio.MinValue && val !== criterio.MaxValue) {
                        errorEl.text(`Value must be either ${criterio.MinValue} or ${criterio.MaxValue}.`).show();
                        input.css("border", "1px solid red");
                        return false;
                    }
                }
            } else {
                // Se ejecuta cuando SÍ hay engineeringRanges, pero NINGUNO para el Criterio 15.
                if (val !== 508 && val !== 610) {
                    errorEl.text("Value must be 508 or 610.").show();
                    input.css("border", "1px solid red");
                    return false;
                }
            }
        } else {
            // Se ejecuta cuando NO existe el objeto engineeringRanges en absoluto.
            if (val !== 508 && val !== 610) {
                errorEl.text("Value must be 508 or 610.").show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        return true;
    }

    function validateOuterCoilDiameterArrival() {
        const input = $("#OuterCoilDiameterArrival");
        const errorEl = $("#OuterCoilDiameterArrivalError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.OuterCoilDiameterArrival + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be a positive number.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        if (window.engineeringRanges) {
            // Buscamos el criterio para "Outer Coil Diameter" (ID_Criteria: 16)
            const criterio = window.engineeringRanges.find(r => r.ID_Criteria === 16);

            if (criterio && !validarContraCriterio(val, criterio)) {
                errorEl.text(describirLimites(criterio)).show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        return true;
    }

    function validateInnerCoilDiameterDelivery() {
        const input = $("#InnerCoilDiameterDelivery");
        const errorEl = $("#InnerCoilDiameterDeliveryError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.InnerCoilDiameterDelivery + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be a positive number.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        if (window.engineeringRanges) {
            // Buscamos el criterio para "Inner Coil Diameter" (ID_Criteria: 15)
            const criterio = window.engineeringRanges.find(r => r.ID_Criteria === 15);

            if (criterio) {
                // --- LÓGICA EXISTENTE CUANDO SE ENCUENTRA UN CRITERIO ---
                // Caso 1: El criterio define un único valor exacto.
                if (criterio.NumericValue != null) {
                    if (val !== criterio.NumericValue) {
                        errorEl.text(`Value must be exactly ${criterio.NumericValue}.`).show();
                        input.css("border", "1px solid red");
                        return false;
                    }
                }
                // Caso 2: El criterio define un rango, y solo los extremos son válidos.
                else if (criterio.MinValue != null && criterio.MaxValue != null) {
                    if (val !== criterio.MinValue && val !== criterio.MaxValue) {
                        errorEl.text(`Value must be either ${criterio.MinValue} or ${criterio.MaxValue}.`).show();
                        input.css("border", "1px solid red");
                        return false;
                    }
                }
            } else {
                // Se ejecuta si hay engineeringRanges, pero NINGUNO para el Criterio 15.
                if (val !== 508 && val !== 610) {
                    errorEl.text("Value must be 508 or 610.").show();
                    input.css("border", "1px solid red");
                    return false;
                }
            }
        } else {
            // Se ejecuta si NO existe el objeto engineeringRanges.
            if (val !== 508 && val !== 610) {
                errorEl.text("Value must be 508 or 610.").show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        return true;
    }

    function validateOuterCoilDiameterDelivery() {
        const input = $("#OuterCoilDiameterDelivery");
        const errorEl = $("#OuterCoilDiameterDeliveryError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.OuterCoilDiameterDelivery + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be a positive number.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        if (window.engineeringRanges) {
            // Buscamos el criterio para "Outer Coil Diameter" (ID_Criteria: 16)
            const criterio = window.engineeringRanges.find(r => r.ID_Criteria === 16);

            if (criterio && !validarContraCriterio(val, criterio)) {
                errorEl.text(describirLimites(criterio)).show();
                input.css("border", "1px solid red");
                return false;
            }
        }

        return true;
    }

    function validateWidth() {
        const input = $("#Width");
        const errorEl = $("#WidthError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled") || !input.is(":visible")) {
            errorEl.text("").hide();
            input.css("border", "");
            return true;
        }

        // Limpiar errores que no sean de la validación de combinación
        const widthMultsErrorEl = $("#Width_MultsError");
        if (widthMultsErrorEl.text().startsWith("Total width of mults")) {
            widthMultsErrorEl.text("").hide();
            $("#Width_Mults").css("border", "");
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl.text(config.displayNames.Width + " is required.").show();
                input.css("border", "1px solid red");
                return false;
            }
            return validateWidthMultsCombination(); // Revalida para limpiar errores
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (window.engineeringRanges) {
            let criterio = window.engineeringRanges.find(r => r.ID_Criteria === 8); //width
            if (criterio && !validarContraCriterio(val, criterio)) {
                input.css("border", "1px solid red");
                errorEl.text(describirLimites(criterio)).show();
                return false;
            }
        }

        // Si todas las validaciones de este campo pasan, se ejecuta la validación de combinación
        return validateWidthMultsCombination();
    }

    function validatePitch() {
        const input = $("#Pitch");
        const errorEl = $("#PitchError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Calcula los golpes teóricos
        updateTheoreticalStrokes();
        updateRealStrokes();

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Pitch + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (val < 0) {
            errorEl.text("The value must be greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        if (window.engineeringRanges) {
            let criterio = window.engineeringRanges.find(r => r.ID_Criteria === 9); //pitch
            if (criterio && !validarContraCriterio(val, criterio)) {
                input.css("border", "1px solid red");
                errorEl.text(describirLimites(criterio)).show();
                return false;
            }
        }

        return true;
    }

    function validateGrossWeight() {
        const input = $("#Gross_Weight");
        const errorEl = $("#Gross_WeightError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // Si readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // Leer y limpiar
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // Vacío → obligatorio solo en los estatus seleccionados
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Gross_Weight + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // Validar número ≥ 0
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        validateClientNetWeight();

        return true;
    }

    function validateAnnualVolume() {
        // 1) Actualizaciones previas
        updateBlanksPerYear();
        let diffPercentage = updateAnnualVolumeStyle();
        clearTimeout(volumeTimeout);
        volumeTimeout = setTimeout(function () {
            if (typeof diffPercentage !== "undefined") {
                showVolumeDifferenceToast(diffPercentage);
            }
        }, 900);

        const input = $("#Annual_Volume");
        const errorEl = $("#Annual_VolumeError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Annual_Volume + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateVolumePerYear() {
        const input = $("#Volume_Per_year");
        const errorEl = $("#Volume_Per_yearError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Volume_Per_year + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAngleA() {
        const input = $("#Angle_A");
        const errorEl = $("#Angle_AError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Angle_A + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateAngleB() {
        const input = $("#Angle_B");
        const errorEl = $("#Angle_BError");
        const status = statusId;

        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Angle_B + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        let val = parseFloat(raw);
        if (isNaN(val)) {
            errorEl.text("The value must be a number.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateBlanksPerStroke() {
        // 1) Actualiza dependencias
        updateStrokesPerAuto();
        updateMinMaxReales();
        updateStrokesShift();

        // 2) Selección de elementos y estado
        const input = $("#Blanks_Per_Stroke");
        const errorEl = $("#Blanks_Per_StrokeError");
        const status = statusId;

        // 3) Estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // 4) Omitir validación si readonly o disabled
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 5) Leer y limpiar
        let raw = input.val() ? input.val().trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // 6) Vacío → obligatorio si el estatus lo exige
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Blanks_Per_Stroke + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // 7) Validar número ≥ 0
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        // 8) Todo OK
        return true;
    }

    function validatePartsPerVehicle() {
        // 1) Actualiza Strokes per Auto y Blanks per Year
        updateStrokesPerAuto();
        updateBlanksPerYear();

        const input = $("#Parts_Per_Vehicle");
        const errorEl = $("#Parts_Per_VehicleError");
        const status = statusId;

        // 2) Define los estatus que hacen el campo obligatorio
        let requiredStatus = [
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        // 3) Si está readonly o disabled, omito validación
        if (input.prop("readonly") || input.prop("disabled")) {
            // pero igual asignamos el valor a parts_auto
            $("#parts_auto").val(input.val());
            return true;
        }

        // 4) Asigno a parts_auto sin importar si es obligatorio
        let partsValue = input.val();
        $("#parts_auto").val(partsValue);

        // 5) Leer y limpiar
        let raw = partsValue ? partsValue.trim() : "";
        errorEl.text("").hide();
        input.css("border", "");

        // 6) Si quedó vacío…
        if (!raw) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.Parts_Per_Vehicle + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            return true;
        }

        // 7) Validar número ≥ 0
        let val = parseFloat(raw);
        if (isNaN(val) || val < 0) {
            errorEl
                .text(isNaN(val)
                    ? "The value must be a number."
                    : "The value must be greater than or equal to 0.")
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        // 8) Todo OK
        return true;
    }

    // Funciones de validación individuales
    function validateVehicleVersion() {
        const input = $("#vehicleVersion");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let vehicleVersion = $("#vehicleVersion").val().trim();
        $("#vehicleVersionError").text("").hide();
        $("#vehicleVersion").css("border", "");
        if (!vehicleVersion) {
            $("#vehicleVersionError").text("Vehicle Version is required.").show();
            $("#vehicleVersion").css("border", "1px solid red");
            return false;
        } else if (vehicleVersion.length > 50) {
            $("#vehicleVersionError").text("Vehicle Version cannot exceed 50 characters.").show();
            $("#vehicleVersion").css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateShipTo() {
        const input = $("#Ship_To");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let shipTo = $("#Ship_To").val().trim();
        $("#Ship_ToError").text("").hide();
        $("#Ship_To").css("border", "");
        if (!shipTo) {
            $("#Ship_ToError").text(config.displayNames.Ship_To + " is required.").show();
            $("#Ship_To").css("border", "1px solid red");
            return false;
        } else if (shipTo.length > 150) {
            $("#Ship_ToError").text(config.displayNames.Ship_To + " cannot exceed 150 characters.").show();
            $("#Ship_To").css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateSpecialPackaging() {
        const input = $("#SpecialPackaging");
        const errorEl = $("#SpecialPackagingError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            //config.statusIDs.CasiCasi,
            // config.statusIDs.POH
        ];

        // Si está readonly o disabled, no validar
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        const limit = 350;
        const value = input.val().trim();

        // Limpiar mensajes anteriores
        errorEl.text("").hide();
        input.css("border", "");

        // Si es obligatorio y quedó vacío → error
        if (!value) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.SpecialPackaging + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            // opcional en otros estatus
            return true;
        }

        // ⚠️ Validar longitud si hay contenido
        if (value.length > limit) {
            errorEl
                .text(
                    config.displayNames.SpecialPackaging + " cannot exceed " + limit + " characters."
                )
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateSpecialRequirement() {
        const input = $("#SpecialRequirement");
        const errorEl = $("#SpecialRequirementError");
        const status = statusId;

        // Estatus que hacen el campo obligatorio
        let requiredStatus = [
            //config.statusIDs.CasiCasi,
            //config.statusIDs.POH
        ];

        // Si está readonly o disabled, no validar
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        const limit = 350;
        const value = input.val().trim();

        // Limpiar mensajes anteriores
        errorEl.text("").hide();
        input.css("border", "");

        // Si es obligatorio y quedó vacío → error
        if (!value) {
            if (requiredStatus.includes(status)) {
                errorEl
                    .text(config.displayNames.SpecialRequirement + " is required.")
                    .show();
                input.css("border", "1px solid red");
                return false;
            }
            // opcional en otros estatus
            return true;
        }

        // ⚠️ Validar longitud si hay contenido
        if (value.length > limit) {
            errorEl
                .text(
                    config.displayNames.SpecialRequirement + " cannot exceed " + limit + " characters."
                )
                .show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validatePartNumber() {
        const input = $("#partNumber");

        // Si el input tiene el atributo readonly o disabled, se omite la validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        let partNumber = $("#partNumber").val().trim();
        $("#partNumberError").text("").hide();
        $("#partNumber").css("border", "");
        if (!partNumber) {
            $("#partNumberError").text("Part Number is required.").show();
            $("#partNumber").css("border", "1px solid red");
            return false;
        } else if (partNumber.length > 50) {
            $("#partNumberError").text("Part Number cannot exceed 50 characters.").show();
            $("#partNumber").css("border", "1px solid red");
            return false;
        }
        return true;
    }

    function validateQuality() {
        const input = $("#quality");
        const status = statusId;  // tu variable global

        // 1) Si readonly o disabled, omito todo
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 2) Defino qué estatus obligan a requerir el campo
        let requiredStatus = [
            config.statusIDs.Quotes,
            config.statusIDs.CarryOver,
            config.statusIDs.CasiCasi,
            config.statusIDs.POH
        ];

        let val = input.val() ? input.val().trim() : "";

        // 3) Siempre valido el largo máximo si escribieron algo
        if (val.length > 50) {
            $("#qualityError").text("Quality cannot exceed 50 characters.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 4) Si el estatus exige que sea obligatorio, entonces validamos que NO esté vacío
        if (requiredStatus.includes(status) && !val) {
            $("#qualityError").text("Quality is required.").show();
            input.css("border", "1px solid red");
            return false;
        }
        // 5) Si llegamos aquí, todo está OK
        $("#qualityError").text("").hide();
        input.css("border", "");
        return true;
    }

    function validateIdealCycleTimePerTool() {
        // 1) Actualizar min/max reales antes de validar
        updateMinMaxReales();

        // 2) Seleccionamos input y span de error
        const input = $("#Ideal_Cycle_Time_Per_Tool");
        const errorSpan = $("#Ideal_Cycle_Time_Per_ToolError");

        // 3) Si está readonly o disabled, omitimos validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 4) Obligatorio si puede editar ingeniería
        let valueStr = input.val().trim();
        if (canEditEngineering && valueStr === "") {
            errorSpan.text("Ideal Cycle Time is required.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 5) Reiniciar mensajes y estilos
        errorSpan.text("").hide();
        input.css("border", "");

        // 6) Si vino vacío y no es obligatorio, es válido
        if (valueStr === "") {
            return true;
        }

        // 7) Convertir y validar que sea número >= 0
        let floatVal = parseFloat(valueStr);
        if (isNaN(floatVal) || floatVal < 0) {
            errorSpan.text("The value must be a number and greater than or equal to 0.").show();
            input.css("border", "1px solid red");
            return false;
        }

        return true;
    }

    function validateOEE() {
        // 1) Seleccionamos input y span de error
        const input = $("#OEE");
        const errorSpan = $("#OEEError");

        // 2) Si está readonly o disabled, omitimos validación
        if (input.prop("readonly") || input.prop("disabled")) {
            return true;
        }

        // 3) Obligatorio si puede editar ingeniería
        let value = input.val().trim();
        if (canEditEngineering && value === "") {
            errorSpan.text("OEE is required.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 4) Si está vacío y no es obligatorio, es válido
        if (value === "") {
            return true;
        }

        // 5) Convertir a número y validar rango 0–100
        let num = parseFloat(value);
        if (isNaN(num)) {
            errorSpan.text("Please enter a valid number.").show();
            input.css("border", "1px solid red");
            return false;
        }
        if (num < 0 || num > 100) {
            errorSpan.text("Please enter a value between 0 and 100.").show();
            input.css("border", "1px solid red");
            return false;
        }

        // 6) Limpiar mensajes y estilos
        errorSpan.text("").hide();
        input.css("border", "");

        return true;
    }


    // --- Publicación (no se necesitan más cambio en el resto de los archivos)---        
    window.validatePiecesPerPackage = validatePiecesPerPackage;
    window.validatePartName = validatePartName;
    window.validateShearing_Width = validateShearing_Width;
    window.validateShearing_Width_Tol_Pos = validateShearing_Width_Tol_Pos;
    window.validateShearing_Width_Tol_Neg = validateShearing_Width_Tol_Neg;
    window.validateShearing_Pitch = validateShearing_Pitch;
    window.validateShearing_Pitch_Tol_Pos = validateShearing_Pitch_Tol_Pos;
    window.validateShearing_Pitch_Tol_Neg = validateShearing_Pitch_Tol_Neg;
    window.validateShearing_Weight = validateShearing_Weight;
    window.validateShearing_Weight_Tol_Pos = validateShearing_Weight_Tol_Pos;
    window.validateShearing_Weight_Tol_Neg = validateShearing_Weight_Tol_Neg;
    window.validatePassesThroughSouthWarehouse = validatePassesThroughSouthWarehouse;
    window.validateInterplant_Plant = validateInterplant_Plant;
    window.validateShearing_Pieces_Per_Stroke = validateShearing_Pieces_Per_Stroke;
    window.validateShearing_Pieces_Per_Car = validateShearing_Pieces_Per_Car;
    window.validateRequiresRackManufacturing = validateRequiresRackManufacturing;
    window.validateRequiresDieManufacturing = validateRequiresDieManufacturing;
    window.validateIsCarryOver = validateIsCarryOver;
    window.validateClientScrapReconciliationPercent = validateClientScrapReconciliationPercent;
    window.validateClientHeadTailReconciliationPercent = validateClientHeadTailReconciliationPercent;
    window.validateWeightOfFinalMults_Min = validateWeightOfFinalMults_Min;
    window.validateWeightOfFinalMults_Max = validateWeightOfFinalMults_Max;
    window.validateScrapReconciliationPercent_Min = validateScrapReconciliationPercent_Min;
    window.validateScrapReconciliationPercent_Max = validateScrapReconciliationPercent_Max;
    window.validateHeadTailReconciliationPercent_Min = validateHeadTailReconciliationPercent_Min;
    window.validateHeadTailReconciliationPercent_Max = validateHeadTailReconciliationPercent_Max;
    window.validateIsRunningChange = validateIsRunningChange;
    window.validateClientNetWeight = validateClientNetWeight;
    window.validateFreightType = validateFreightType;
    window.validateArrivalWarehouse = validateArrivalWarehouse;
    window.validateStacksPerPackage = validateStacksPerPackage;
    window.validateDeliveryConditions = validateDeliveryConditions;
    window.validateArrivalTransportTypeOther = validateArrivalTransportTypeOther;
    window.validateStackableLevels = validateStackableLevels;
    window.validateArrivalComments = validateArrivalComments;
    window.validateArrivalProtectiveMaterial = validateArrivalProtectiveMaterial;
    window.validateArrivalProtectiveMaterialOther = validateArrivalProtectiveMaterialOther;
    window.validateArrivalPackagingType = validateArrivalPackagingType;
    window.validateArrivalTransportType = validateArrivalTransportType;
    window.validateCoilPosition = validateCoilPosition;
    window.validateWidth_Mults = validateWidth_Mults;
    window.validateWidth_Mults_Tol_Neg = validateWidth_Mults_Tol_Neg;
    window.validateWidth_Mults_Tol_Pos = validateWidth_Mults_Tol_Pos;
    window.validateWidth_Plates = validateWidth_Plates;
    window.validateWidth_Plates_Tol_Neg = validateWidth_Plates_Tol_Neg;
    window.validateWidth_Plates_Tol_Pos = validateWidth_Plates_Tol_Pos;
    window.validateTonsPerShift = validateTonsPerShift;
    window.validateStrapTypeObservations = validateStrapTypeObservations;
    window.validateAdditionalsOtherDescription = validateAdditionalsOtherDescription;
    window.validateLabelOtherDescription = validateLabelOtherDescription;
    window.validateTurnOverSide = validateTurnOverSide;
    window.validateInterplantPiecesPerPackage = validateInterplantPiecesPerPackage;
    window.validateInterplantStacksPerPackage = validateInterplantStacksPerPackage;
    window.validateMaterial = validateMaterial;
    window.validateReturnableUses = validateReturnableUses;
    window.validateNumberOfPlates = validateNumberOfPlates;
    window.validateWeldedThicknesses = validateWeldedThicknesses;
    window.validateWidthMultsCombination = validateWidthMultsCombination;
    window.validateScrapReconciliationPercent = validateScrapReconciliationPercent;
    window.validateHeadTailReconciliationPercent = validateHeadTailReconciliationPercent;
    window.validateInterplantRequiresDieManufacturing = validateInterplantRequiresDieManufacturing;
    window.validateInterplantRequiresRackManufacturing = validateInterplantRequiresRackManufacturing;
    window.validateInterplantPackagingStandard = validateInterplantPackagingStandard;
    window.validateMaterialType = validateMaterialType;
    window.validateShape = validateShape;
    window.validatePackagingStandard = validatePackagingStandard;
    window.validateRealLine = validateRealLine;
    window.validateInterplantDeliveryTransportTypeOther = validateInterplantDeliveryTransportTypeOther;
    window.validateInterplantDeliveryTransportType = validateInterplantDeliveryTransportType;
    window.validateInterplantDeliveryCoilPosition = validateInterplantDeliveryCoilPosition;
    window.validateRoute = validateRoute;
    window.validateVehicle = validateVehicle;
    window.validatePackagingFile = validatePackagingFile;
    window.validateCADFile = validateCADFile;
    window.validateDeliveryTransportTypeOther = validateDeliveryTransportTypeOther;
    window.validateDeliveryCoilPosition = validateDeliveryCoilPosition;
    window.validateDeliveryTransportType = validateDeliveryTransportType;
    window.updateSlitterCapacityChart = updateSlitterCapacityChart;
    window.validateRealSOP = validateRealSOP;
    window.validateRealEOP = validateRealEOP;
    window.validateTensileStrength = validateTensileStrength;
    window.validateThickness = validateThickness;
    window.validateThicknessToleranceNegative = validateThicknessToleranceNegative;
    window.validateThicknessTolerancePositive = validateThicknessTolerancePositive;
    window.validateWidthToleranceNegative = validateWidthToleranceNegative;
    window.validateWidthTolerancePositive = validateWidthTolerancePositive;
    window.validatePitchToleranceNegative = validatePitchToleranceNegative;
    window.validatePitchTolerancePositive = validatePitchTolerancePositive;
    window.validateAngleAToleranceNegative = validateAngleAToleranceNegative;
    window.validateAngleATolerancePositive = validateAngleATolerancePositive;
    window.validateAngleBToleranceNegative = validateAngleBToleranceNegative;
    window.validateAngleBTolerancePositive = validateAngleBTolerancePositive;
    window.validateWeightOfFinalMults = validateWeightOfFinalMults;
    window.validateMultipliers = validateMultipliers;
    window.validateMajorBase = validateMajorBase;
    window.validateMajorBaseToleranceNegative = validateMajorBaseToleranceNegative;
    window.validateMajorBaseTolerancePositive = validateMajorBaseTolerancePositive;
    window.validateMinorBase = validateMinorBase;
    window.validateMinorBaseToleranceNegative = validateMinorBaseToleranceNegative;
    window.validateMinorBaseTolerancePositive = validateMinorBaseTolerancePositive;
    window.validateFlatness = validateFlatness;
    window.validateFlatnessToleranceNegative = validateFlatnessToleranceNegative;
    window.validateFlatnessTolerancePositive = validateFlatnessTolerancePositive;
    window.validateMasterCoilWeight = validateMasterCoilWeight;
    window.validateInnerCoilDiameterArrival = validateInnerCoilDiameterArrival;
    window.validateOuterCoilDiameterArrival = validateOuterCoilDiameterArrival;
    window.validateInnerCoilDiameterDelivery = validateInnerCoilDiameterDelivery;
    window.validateOuterCoilDiameterDelivery = validateOuterCoilDiameterDelivery;
    window.validateWidth = validateWidth;
    window.validatePitch = validatePitch;
    window.validateGrossWeight = validateGrossWeight;
    window.validateAnnualVolume = validateAnnualVolume;
    window.validateVolumePerYear = validateVolumePerYear;
    window.validateAngleA = validateAngleA;
    window.validateAngleB = validateAngleB;
    window.validateBlanksPerStroke = validateBlanksPerStroke;
    window.validatePartsPerVehicle = validatePartsPerVehicle;
    window.validateVehicleVersion = validateVehicleVersion;
    window.validateShipTo = validateShipTo;
    window.validateSpecialPackaging = validateSpecialPackaging;
    window.validateSpecialRequirement = validateSpecialRequirement;
    window.validatePartNumber = validatePartNumber;
    window.validateQuality = validateQuality;
    window.validateIdealCycleTimePerTool = validateIdealCycleTimePerTool;
    window.validateOEE = validateOEE;



})(); // <-- Fin de la IIFE