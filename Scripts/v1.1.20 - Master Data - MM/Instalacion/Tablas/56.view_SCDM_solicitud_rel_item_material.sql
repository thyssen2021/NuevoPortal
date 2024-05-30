--use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_SCDM_solicitud_rel_item_material;  
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
--select * from view_SCDM_solicitud_rel_item_material
CREATE VIEW [dbo].view_SCDM_solicitud_rel_item_material AS

select t.descripcion As tipo_material, i.*, 
(select top 1 codigoSap from plantas where clave = (select top 1 id_planta from SCDM_rel_solicitud_plantas where id_solicitud=i.id_solicitud)) as planta_sap,
CASE
	WHEN i.id_tipo_material =1 --Rollo
		THEN CAST(i.espesor_mm AS varchar) +'X'+CAST(i.ancho_mm AS varchar) 
	WHEN i.id_tipo_material = 2 --Cinta
		THEN CAST(i.espesor_mm AS varchar) +'X'+CAST(i.ancho_entrega_cinta_mm AS varchar) 
	WHEN i.id_tipo_material = 3 OR i.id_tipo_material = 4  OR i.id_tipo_material = 5 --Platina, Shearing, PlatinaSoldada
		THEN CAST(i.espesor_mm AS varchar) +'X'+CAST(i.ancho_mm AS varchar)+'X'+CAST(i.avance_mm AS varchar) 
	ELSE null
END  as dimensiones,
CASE
	WHEN i.id_tipo_material =1 --Rollo
		THEN CAST(i.espesor_mm AS varchar) + '(+'+CAST( espesor_tolerancia_positiva_mm as varchar)+'/'+
			CASE 
				WHEN espesor_tolerancia_negativa_mm = 0
				THEN '-'
				else ''
			END
		+CAST( espesor_tolerancia_negativa_mm as varchar)+') X ' +
		CAST(i.ancho_mm AS varchar) + '(+'+CAST( ancho_tolerancia_positiva_mm as varchar)+'/'+
			CASE 
				WHEN ancho_tolerancia_negativa_mm = 0
				THEN '-'
				else ''
			END
		+CAST( ancho_tolerancia_negativa_mm as varchar)+')'
	ELSE null
END  as dimensiones_tolerancia,
CASE
	WHEN clase_aprovisionamiento = 'Acopio Externo'
		THEN 'F'
	ELSE 'E'
END  as procurement_type,
(select 
top 1 material_sap from SCDM_cat_material_referencia where clave = 
'R-'+
			CASE
				WHEN clase_aprovisionamiento = 'Acopio Externo'
					THEN 'E'
				ELSE 'I'
			END +'-'+ SUBSTRING(i.tipo_recubrimiento,3,2)+'-'+i.unidad_medida_inventario+'-'+
			CASE
				WHEN id_tipo_material = 4 OR id_tipo_material = 5
					THEN 'Platina'
				ELSE t.descripcion
			END
 ) as material_referencia,
 d.*,
 CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN '1.005'
	ELSE CAST(i.peso_bruto AS varchar)
END  as peso_bruto_class,
 CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN '1.000'
	ELSE CAST(i.peso_neto AS varchar)
END  as peso_neto_class,
CASE
	WHEN SUBSTRING(i.tipo_recubrimiento,3,2) ='CM'
		THEN NULL
	ELSE SUBSTRING(i.tipo_recubrimiento,3,2)
END as commodity_class,
CAST(i.espesor_mm + i.espesor_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.espesor_mm + i.espesor_tolerancia_positiva_mm AS varchar) as espesor_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
	ELSE CAST(i.ancho_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
END AS ancho_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN i.ancho_entrega_cinta_mm
	ELSE i.ancho_mm
END AS ancho_budget,
CAST(i.avance_mm + i.avance_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.avance_mm + i.avance_tolerancia_positiva_mm AS varchar) as avance_tolerancias,
CASE
	WHEN i.planicidad_mm is NOT null
		THEN i.planicidad_mm
	--espesor >=0 && espesor <.7
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0 AND i.espesor_mm< .7 AND i.ancho_mm >=1 and i.ancho_mm<1200
		THEN 4	
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0 AND i.espesor_mm< .7 AND i.ancho_mm >=1200 and i.ancho_mm<1500
		THEN 4.5
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0 AND i.espesor_mm< .7 AND i.ancho_mm >=1500
		THEN 4.5
		--espesor >=.7 && espesor <1.2
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0.7 AND i.espesor_mm< 1.2 AND i.ancho_mm >=1 and i.ancho_mm<1200
		THEN 3
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0.7 AND i.espesor_mm< 1.2 AND i.ancho_mm >=1200 and i.ancho_mm<1500
		THEN 4
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 0.7 AND i.espesor_mm< 1.2 AND i.ancho_mm >=1500
		THEN 5
		--espesor >=1.2 
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 1.2 AND i.ancho_mm >=1 and i.ancho_mm<1200
		THEN 2.5
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 1.2 AND i.ancho_mm >=1200 and i.ancho_mm<1500
		THEN 3
	WHEN i.planicidad_mm IS NULL AND i.espesor_mm >= 1.2 AND i.ancho_mm >=1500
		THEN 4.5
	ELSE null
END as planicidad_class,
CASE
	WHEN i.parte_interior_exterior = 'Exterior'
		THEN 1
	ELSE 3
END as superficie_class,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where i.grado_calidad = grado_calidad ) >=1
		THEN cast((select top 1 clave from SCDM_cat_grado_calidad where i.grado_calidad = grado_calidad) AS varchar)	
	ELSE i.grado_calidad
END AS clave_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_grado_calidad where i.grado_calidad = grado_calidad) >=1
		THEN 1
	ELSE 0
END AS existe_grado_calidad,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where i.peso_recubrimiento = descripcion) >=1
		THEN (select top 1 clave from SCDM_cat_peso_recubrimiento where i.peso_recubrimiento = descripcion) 
	ELSE i.peso_recubrimiento
END AS clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_peso_recubrimiento where i.peso_recubrimiento = descripcion) >=1
		THEN 1
	ELSE 0
END AS existe_clave_peso_recubrimiento,
CASE
	WHEN (select count(*) from SCDM_cat_molino where i.molino = descripcion) >=1
		THEN (select top 1 clave from SCDM_cat_molino where i.molino = descripcion) 
	ELSE i.molino
END AS clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_molino where i.molino = descripcion) >=1
		THEN 1
	ELSE 0
END AS existe_clave_molino,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where i.forma = descripcion) >=1
		THEN (select top 1 clave from SCDM_cat_forma_material where i.forma = descripcion) 
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN '1'
	ELSE CAST (i.forma as varchar)
END AS clave_forma,
CASE
	WHEN (select count(*) from SCDM_cat_forma_material where i.forma = descripcion) >=1 OR i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta
		THEN 1
	ELSE 0
END AS existe_clave_forma,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = i.cliente ) >=1
		THEN cast((select top 1 claveSAP from clientes where '('+claveSAP+') - '+descripcion = i.cliente) AS varchar)	
	ELSE i.cliente
END AS clave_cliente,
CASE
	WHEN (select count(*) from clientes where '('+claveSAP+') - '+descripcion = i.cliente ) >=1
		THEN 1
	ELSE 0
END AS existe_clave_cliente,
CASE
	WHEN i.id_tipo_material = 1 --Rollo 
		THEN i.diametro_interior
	WHEN i.id_tipo_material = 2 --Cinta
		THEN i.diametro_interior_salida
	ELSE NULL
END  as diametro_interior_class,
CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta 
		THEN i.peso_max_kg
	WHEN i.id_tipo_material in (3,4,5) --platina, platina soldada, shearing
		THEN i.peso_neto
	ELSE NULL
END  as peso_maximo_budget

from SCDM_solicitud_rel_item_material As i 
join SCDM_cat_tipo_materiales_solicitud As t On t.id = i.id_tipo_material
left join SCDM_solicitud_item_material_datos_sap as d on d.id_scdm_solicitud_rel_item_material = i.id

