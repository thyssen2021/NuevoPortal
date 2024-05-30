--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_materiales_extension_almacenes;  
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
--select * from view_SCDM_materiales_extension_almacenes
CREATE VIEW [dbo].view_SCDM_materiales_extension_almacenes AS

SELECT
  im.id_solicitud,
  im.numero_material,
  im.planta_sap,
  al.warehouse,
  al.storage_type,
  --  ubicacion
  (SELECT TOP 1 ubicacion
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_material = im.id
  AND al.id = id_cat_almacenes
  )as ubicacion,
  --ejecucion_correcta
   (SELECT TOP 1 almacen_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_material = im.id
  AND al.id = id_cat_almacenes
  )as almacen_ejecucion_correcta,
  --mensaje_sap
   (SELECT TOP 1 almacen_mensaje_sap
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_material = im.id
  AND al.id = id_cat_almacenes
  )as almacen_mensaje_sap,
  'CreacionMateriales' AS tipo_fuente
FROM SCDM_cat_almacenes AS al
JOIN plantas AS p
  ON p.clave = al.id_planta
CROSS JOIN view_SCDM_solicitud_rel_item_material AS im
WHERE im.planta_sap = p.codigoSap

UNION

SELECT
  cr.id_solicitud,
  cr.nuevo_material,
  cr.codigoSap,
  al.warehouse,
  al.storage_type,
  --  ubicacion
  (SELECT TOP 1 ubicacion
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  AND al.id = id_cat_almacenes
  )as ubicacion,
  --ejecucion_correcta
   (SELECT TOP 1 almacen_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  AND al.id = id_cat_almacenes
  )as almacen_ejecucion_correcta,
  --mensaje_sap
   (SELECT TOP 1 almacen_mensaje_sap
  FROM SCDM_solicitud_rel_extension_almacenes
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  AND al.id = id_cat_almacenes
  )as almacen_mensaje_sap,
'CreacionReferencia' AS tipo_fuente
FROM SCDM_cat_almacenes AS al
JOIN plantas AS p
  ON p.clave = al.id_planta
CROSS JOIN view_SCDM_solicitud_creacion_referencia AS cr
WHERE cr.codigoSap = p.codigoSap 

