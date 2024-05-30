--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_solicitud_rel_facturacion;  
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
--select * from view_SCDM_solicitud_rel_facturacion
CREATE VIEW [dbo].view_SCDM_solicitud_rel_facturacion AS

SELECT im.id_solicitud, numero_material as material, planta_sap as planta,
(SELECT TOP 1 unidad_medida
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id
  )as unidad_medida,
  (SELECT TOP 1 clave_producto_servicio
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id
  ) clave_producto_servicio,
    (SELECT TOP 1 cliente
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) cliente,
    (SELECT TOP 1 descripcion_en
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) descripcion_en,
  (SELECT TOP 1 uso_CFDI_01
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_01,
	(SELECT TOP 1 uso_CFDI_02
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_02,
(SELECT TOP 1 uso_CFDI_03
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_03,
(SELECT TOP 1 uso_CFDI_04
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_04,
(SELECT TOP 1 uso_CFDI_05
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_05,
(SELECT TOP 1 uso_CFDI_06
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_06,
(SELECT TOP 1 uso_CFDI_07
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_07,
(SELECT TOP 1 uso_CFDI_08
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_08,
(SELECT TOP 1 uso_CFDI_09
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_09,
  (SELECT TOP 1 uso_CFDI_10
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) uso_CFDI_10,
   (SELECT TOP 1 mensaje_sap
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) mensaje_sap,
    (SELECT TOP 1 ejecucion_correcta
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_material = im.id) ejecucion_correcta
from view_SCDM_solicitud_rel_item_material AS im

UNION

SELECT cr.id_solicitud, nuevo_material as material, codigoSap as planta,
(SELECT TOP 1 unidad_medida
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as unidad_medida,
  (SELECT TOP 1 clave_producto_servicio
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as clave_producto_servicio,
  (SELECT TOP 1 cliente
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as cliente,
(SELECT TOP 1 descripcion_en
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as descripcion_en,
  (SELECT TOP 1 uso_CFDI_01
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_01,
  (SELECT TOP 1 uso_CFDI_02
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_02,
(SELECT TOP 1 uso_CFDI_03
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_03,
(SELECT TOP 1 uso_CFDI_04
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_04,
(SELECT TOP 1 uso_CFDI_05
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_05,
(SELECT TOP 1 uso_CFDI_06
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_06,
(SELECT TOP 1 uso_CFDI_07
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_07,
(SELECT TOP 1 uso_CFDI_08
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_08,
(SELECT TOP 1 uso_CFDI_09
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_09,
(SELECT TOP 1 uso_CFDI_10
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as uso_CFDI_10,
(SELECT TOP 1 mensaje_sap
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as mensaje_sap,
(SELECT TOP 1 ejecucion_correcta
  FROM SCDM_solicitud_rel_facturacion
  WHERE id_solicitud_rel_item_creacion_referencia = cr.id
  )as ejecucion_correcta
from view_SCDM_solicitud_creacion_referencia AS cr

select * from  view_SCDM_solicitud_rel_facturacion where id_solicitud=14 
 

 select * from view_SCDM_solicitud_rel_facturacion where id_solicitud= 14 order by material

 --select * from view_SCDM_solicitud_rel_item_material 
  --select * from view_SCDM_solicitud_creacion_referencia 

--  select * from [SCDM_solicitud_rel_facturacion]