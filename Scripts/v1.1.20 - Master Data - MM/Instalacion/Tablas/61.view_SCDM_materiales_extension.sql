--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_materiales_extension;  
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
--select * from view_SCDM_materiales_extension
CREATE VIEW [dbo].view_SCDM_materiales_extension AS

SELECT
  im.id_solicitud,
  im.numero_material,
  im.planta_sap,
  sl.clave,
  (SELECT TOP 1
    extension_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_item_material = im.id
  AND sl.id = id_cat_storage_location)
  AS [extension_ejecucion_correcta],
  (SELECT TOP 1
    extension_mensaje_sap
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_item_material = im.id
  AND sl.id = id_cat_storage_location)
  AS [mensaje_sap]
FROM SCDM_cat_storage_location AS sl
JOIN plantas AS p
  ON p.clave = sl.id_planta
CROSS JOIN view_SCDM_solicitud_rel_item_material AS im
WHERE im.planta_sap = p.codigoSap

UNION

SELECT
  cr.id_solicitud,
  cr.nuevo_material,
  p.codigoSap,
  sl.clave,
  (SELECT TOP 1
    extension_ejecucion_correcta
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_creacion_referencia = cr.id
  AND sl.id = id_cat_storage_location)
  AS [extension_ejecucion_correcta],
  (SELECT TOP 1
    extension_mensaje_sap
  FROM SCDM_solicitud_rel_extension
  WHERE id_solicitud_rel_creacion_referencia = cr.id
  AND sl.id = id_cat_storage_location)
  AS [mensaje_sap]
FROM SCDM_cat_storage_location AS sl
JOIN plantas AS p
  ON p.clave = sl.id_planta
CROSS JOIN SCDM_solicitud_rel_creacion_referencia AS cr
WHERE p.clave = cr.id_planta