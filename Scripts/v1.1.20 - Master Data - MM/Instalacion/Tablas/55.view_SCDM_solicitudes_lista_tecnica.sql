--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_solicitudes_lista_tecnica;  
/*****************************************************************************
*  Tipo de objeto: View
*  Funcion: Vista para mostrar el año fiscal 
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 25/02/2022
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
		
GO
CREATE VIEW [dbo].view_SCDM_solicitudes_lista_tecnica AS

select lt.id_solicitud, lt.resultado, imr.tipo_venta, tmr.descripcion as [tipo_material_resultado], imr.peso_bruto, imr.peso_neto, imr.unidad_medida_inventario,
lt.sobrante, lt.componente, tmc.descripcion as [tipo_material_componente], lt.cantidad_platinas, cantidad_cintas, lt.fecha_validez_reaplicacion
 from SCDM_solicitud_rel_lista_tecnica as lt
left join SCDM_solicitud_rel_item_material as imr on imr.id = (select top 1 id from SCDM_solicitud_rel_item_material where numero_material = lt.resultado AND id_solicitud=lt.id_solicitud)
left join SCDM_solicitud_rel_item_material as imc on imc.id = (select top 1 id from SCDM_solicitud_rel_item_material where numero_material = lt.componente AND id_solicitud=lt.id_solicitud)
left join SCDM_cat_tipo_materiales_solicitud as tmr on tmr.id = imr.id_tipo_material
left join SCDM_cat_tipo_materiales_solicitud as tmc on tmc.id = imc.id_tipo_material
