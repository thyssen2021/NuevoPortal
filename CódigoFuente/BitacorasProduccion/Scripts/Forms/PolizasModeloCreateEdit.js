$(document).ready(function () {


});

//variable global para el id
var num = 0;

//agrega una fila para los conceptos
function AgregarConcepto() {


    $("#body_conceptos").append(
        `
                                                <tr  id="div_concepto_`+ num + `">
                                                    <input type="hidden" name="PM_conceptos_modelo.Index" id="produccion_lotes.Index" value="`+ num + `" />
                                                        <td>
                                                            <input style=" font-size: 12px;" type="text" name="PM_conceptos_modelo[`+ num + `].cuenta" id="PM_conceptos_modelo[` + num + `].cuenta" class="form-control col-md-12" value="" autocomplete="off" maxlength="15" required>
                                                            <span class="field-validation-valid text-danger" data-valmsg-for="PM_conceptos_modelo[` + num + `].cuenta" data-valmsg-replace="true"></span>
                                                        </td>
                                                        <td>
                                                            <input style=" font-size: 12px;" type="text" name="PM_conceptos_modelo[`+ num + `].cc" id="PM_conceptos_modelo[` + num + `].cc" class="form-control col-md-12" value="" maxlength="15" autocomplete="off">
                                                            <span class="field-validation-valid text-danger" data-valmsg-for="PM_conceptos_modelo[` + num + `].cc" data-valmsg-replace="true"></span>
                                                        </td>
                                                        <td>
                                                            <textarea style="font-size: 12px;" type="text" name="PM_conceptos_modelo[`+ num + `].concepto" id="PM_conceptos_modelo[` + num + `].concepto" class="form-control col-md-12" value="" autocomplete="off" maxlength="120" required></textarea>
                                                            <span class="field-validation-valid text-danger" data-valmsg-for="PM_conceptos_modelo[` + num + `].concepto" data-valmsg-replace="true"></span>
                                                        </td>
                                                        <td>
                                                             <input style=" font-size: 12px;" type="text" name="PM_conceptos_modelo[`+ num + `].poliza" id="PM_conceptos_modelo[` + num + `].poliza" class="form-control col-md-12" value="" autocomplete="off"  maxlength="10">
                                                            <span class="field-validation-valid text-danger" data-valmsg-for="PM_conceptos_modelo[` + num + `].poliza" data-valmsg-replace="true"></span>
                                                        </td>
                                                        
                                                        <td>
                                                             <input type="button" value="Borrar" class="btn btn-danger" onclick="borrarConcepto(` + num + `); return false;">
                                                        </td>
                                                 </tr>

                                                   `
    );
    $("#div_concepto_" + num).hide().fadeIn(700);

    num++;


}

//borra un concepto
function borrarConcepto(id) {

    $("#div_concepto_" + id).fadeOut(0, function () {
        $(this).remove();
    });

}

window.onload = function () {
    clicMenu(2);
}