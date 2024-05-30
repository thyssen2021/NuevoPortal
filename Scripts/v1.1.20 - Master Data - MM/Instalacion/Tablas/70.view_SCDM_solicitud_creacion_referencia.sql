--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_solicitud_creacion_referencia;  
/*****************************************************************************
*  Tipo de objeto: View
*  Funcion: Vista para motrar los datos de los materiales de creacion con referencia
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 25/02/2022
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
		
GO
CREATE VIEW [dbo].view_SCDM_solicitud_creacion_referencia AS

select cr.*, tm.descripcion as tipo_material, tv.descripcion as tipo_venta, p.codigoSap,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad ) >=1
		THEN cast((select top 1 clave from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad) AS varchar)	
	ELSE cr.grado_calidad
END AS clave_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where cr.grado_calidad = grado_calidad) >=1
		THEN 1
	ELSE 0
END AS existe_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity ) >=1
		THEN cast((select top 1 clave from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity) AS varchar)	
	ELSE cr.commodity
END AS clave_commodity,
CASE
	WHEN (select count(*) from SCDM_cat_commodity where clave+' - '+descripcion = cr.commodity) >=1
		THEN 1
	ELSE 0
END AS existe_commodity,
CASE
	WHEN (select count(*) from SCDM_cat_superficie where descripcion = cr.superficie ) >=1
		THEN cast((select top 1 clave from SCDM_cat_superficie where descripcion = cr.superficie) AS varchar)	
	ELSE cr.superficie
END AS clave_superficie,
CASE
	WHEN (select count(*) from SCDM_cat_superficie where descripcion = cr.superficie) >=1
		THEN 1
	ELSE 0
END AS existe_superficie,
CASE
	WHEN (select count(*) from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial ) >=1
		THEN cast((select top 1 clave from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial) AS varchar)	
	ELSE cr.tratamiento_superficial
END AS clave_tratamiento_superficial,
CASE
	WHEN (select count(*) from SCDM_cat_tratamiento_superficial where descripcion = cr.tratamiento_superficial) >=1
		THEN 1
	ELSE 0
END AS existe_tratamiento_superficial,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento ) >=1
		THEN cast((select top 1 clave from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento) AS varchar)	
	ELSE cr.peso_recubrimiento
END AS clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where descripcion = cr.peso_recubrimiento) >=1
		THEN 1
	ELSE 0
END AS existe_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_molino where descripcion = cr.molino ) >=1
		THEN cast((select top 1 clave from SCDM_cat_molino where descripcion = cr.molino) AS varchar)	
	ELSE cr.molino
END AS clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_molino where descripcion = cr.molino) >=1
		THEN 1
	ELSE 0
END AS existe_molino,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where descripcion = cr.forma ) >=1
		THEN cast((select top 1 clave from SCDM_cat_forma_material where descripcion = cr.forma) AS varchar)	
	ELSE cr.forma
END AS clave_forma,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where descripcion = cr.forma) >=1
		THEN 1
	ELSE 0
END AS existe_forma,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = cr.cliente ) >=1
		THEN cast((select top 1 claveSAP from clientes where '('+claveSAP+') - '+descripcion = cr.cliente) AS varchar)	
	ELSE cr.cliente
END AS clave_cliente,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = cr.cliente) >=1
		THEN 1
	ELSE 0
END AS existe_cliente
from SCDM_solicitud_rel_creacion_referencia as cr
join SCDM_cat_tipo_materiales_solicitud as tm on tm.id = cr.id_tipo_material
join SCDM_cat_tipo_venta as tv on tv.id = cr.id_tipo_venta
join plantas as p on p.clave = cr.id_planta

GO
