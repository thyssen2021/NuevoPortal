USE [Portal_2_0_scdm]
GO

/****** Object:  View [dbo].[view_SCDM_solicitud_rel_item_budget]    Script Date: 23/08/2024 10:41:15 a.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


--select * from view_SCDM_solicitud_rel_item_budget
CREATE OR ALTER   VIEW [dbo].[view_SCDM_solicitud_rel_item_budget] AS
--Creación de Materiales
select t.descripcion As tipo_material, i.id, i.id_solicitud, i.id_tipo_material, i.metal, i.tipo_venta, i.posicion_rollo, i.ihs_1, i.ihs_2, i.ihs_3, i.ihs_4, i.ihs_5,
i.modelo_negocio, i.numero_material, CAST (i.espesor_mm AS varchar) AS espesor_mm, CAST (i.espesor_tolerancia_negativa_mm as varchar) AS  espesor_tolerancia_negativa_mm, 
CAST (i.espesor_tolerancia_positiva_mm AS varchar) AS espesor_tolerancia_positiva_mm, CAST (i.ancho_mm AS varchar) AS ancho_mm,
CAST (i.ancho_tolerancia_negativa_mm AS varchar) AS ancho_tolerancia_negativa_mm, CAST (i.ancho_tolerancia_positiva_mm AS varchar) AS ancho_tolerancia_positiva_mm,
CAST (i.peso_min_kg AS VARCHAR) as peso_min_kg, CAST (i.requiere_consiliacion_puntas_colar AS VARCHAR) AS requiere_consiliacion_puntas_colar, CAST (i.scrap_permitido_puntas_colas as VARCHAR) AS scrap_permitido_puntas_colas, 
CAST (i.avance_mm AS varchar) AS avance_mm, CAST (i.avance_tolerancia_negativa_mm AS varchar) AS avance_tolerancia_negativa_mm, cast (i.avance_tolerancia_positiva_mm as varchar) as avance_tolerancia_positiva_mm,
CAST (i.piezas_por_golpe AS varchar) as piezas_por_golpe, CAST (i.piezas_por_paquete AS varchar) as piezas_por_paquete, '\' as [peso_real_bruto], '\' as [peso_real_neto], CAST (i.piezas_por_auto AS varchar) AS piezas_por_auto, 
CAST (i.peso_inicial AS varchar) as peso_inicial, CAST (i.porcentaje_scrap_puntas_colas as varchar) as porcentaje_scrap_puntas_colas,
CAST (i.conciliacion_scrap_ingenieria AS varchar) as conciliacion_scrap_ingenieria, CAST (i.angulo_a as varchar) as angulo_a, cast(i.angulo_b as varchar) as angulo_b
,CASE
	WHEN i.tipo_venta = 'Reaplicación'
	THEN 1
	ELSE 0
END AS reaplicacion,
 d.budget_ejecucion_correcta, d.budget_mensaje_sap,

CAST(i.espesor_mm + i.espesor_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.espesor_mm + i.espesor_tolerancia_positiva_mm AS varchar) as espesor_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_entrega_cinta_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
	ELSE CAST(i.ancho_mm + i.ancho_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.ancho_mm + i.ancho_tolerancia_positiva_mm AS varchar) 
END AS ancho_tolerancias,
CASE
	WHEN i.id_tipo_material = 2 --cinta
		THEN cast (i.ancho_entrega_cinta_mm as varchar) 
	ELSE cast (i.ancho_mm as varchar)
END AS ancho_budget,
CAST(i.avance_mm + i.avance_tolerancia_negativa_mm AS varchar)+' - '+CAST(i.avance_mm + i.avance_tolerancia_positiva_mm AS varchar) as avance_tolerancias,
CASE
	WHEN i.id_tipo_material = 1 OR i.id_tipo_material = 2 --Rollo, Cinta 
		THEN CAST (i.peso_max_kg As varchar)
	WHEN i.id_tipo_material in (3,4,5) --platina, platina soldada, shearing
		THEN CAST (i.peso_neto AS varchar) 
	ELSE NULL
END  as peso_maximo_budget
, NULL peso_maximo_tolerancia_negativa, null peso_maximo_tolerancia_positiva, null as peso_maximo_tolerancias
, NULL peso_minimo_tolerancia_negativa, null peso_minimo_tolerancia_positiva, null as peso_minimo_tolerancias
, null AS pieza_doble
from SCDM_solicitud_rel_item_material As i 
join SCDM_cat_tipo_materiales_solicitud As t On t.id = i.id_tipo_material
left join SCDM_solicitud_item_material_datos_sap as d on d.id_scdm_solicitud_rel_item_material = i.id

UNION
-- cambios de budget
SELECT rc.tipo_material, rc.id, rc.id_solicitud, 1 as id_tipo_material,rc.tipo_metal as metal, rc.tipo_venta, 
rc.posicion_rollo, rc.IHS_num_1 as ihs_1, rc.IHS_num_2 as ihs_2, rc.IHS_num_3 as ihs_3, rc.IHS_num_4 as ihs_4, rc.IHS_num_5 as ihs_5,
rc.modelo_negocio, rc.material_existente as numero_material, '\' as espesor_mm, '\' as espesor_tolerancia_negativa_mm, '\' as espesor_tolerancia_positiva_mm,
'\' as ancho_mm, '\' as ancho_tolerancia_negativa_mm, '\' as ancho_tolerancia_positiva_mm, CAST (rc.peso_minimo as varchar) as peso_min_kg,
CAST(rc.conciliacion_puntas_colas as VARCHAR) AS requiere_consiliacion_puntas_colar, CAST (rc.scrap_permitido_puntas_colas AS varchar) as scrap_permitido_puntas_colas,
'\' as avance_mm, '\' as avance_tolerancia_negativa_mm, '\' as avance_tolerancia_positiva_mm, CAST (rc.piezas_por_golpe AS varchar) AS piezas_por_golpe, CAST (rc.piezas_por_paquete AS VARCHAR) AS piezas_por_paquete,
cast (rc.peso_bruto_real_bascula as varchar) as peso_real_bruto, cast (rc.peso_neto_real_bascula as varchar) as peso_real_neto, CAST (rc.piezas_por_auto AS VARCHAR) AS piezas_por_auto, CAST(rc.peso_inicial AS varchar) AS peso_inicial,
CAST (rc.scrap_permitido_puntas_colas AS varchar) as porcentaje_scrap_puntas_colas, CAST (rc.conciliacion_scrap_ingenieria AS VARCHAR) as conciliacion_scrap_ingenieria, 
CAST (rc.angulo_a AS VARCHAR) AS angulo_a, CAST (rc.angulo_b AS varchar) AS angulo_b, CAST(rc.reaplicacion AS VARCHAR) AS reaplicacion, rc.ejecucion_correcta as budget_ejecucion_correcta, rc.resultado as budget_mensaje_sap, 
'\' as espesor_tolerancias, '\' as ancho_tolerancias, '\' as ancho_budget, '\' as avance_tolerancias, 
CAST (peso_maximo AS varchar) as peso_maximo_budget, CAST(rc.peso_maximo_tolerancia_negativa AS varchar) AS peso_maximo_tolerancia_negativa, CAST (rc.peso_maximo_tolerancia_positiva AS varchar) AS peso_maximo_tolerancia_positiva,
CASE
	WHEN rc.peso_maximo is not null AND NOT ((rc.peso_maximo_tolerancia_negativa = 0 OR rc.peso_maximo_tolerancia_negativa is null) AND (rc.peso_maximo_tolerancia_positiva = 0 OR rc.peso_maximo_tolerancia_positiva is null))
		THEN CAST(rc.peso_maximo + isnull(rc.peso_maximo_tolerancia_negativa,0) AS varchar)+' - '+CAST(rc.peso_maximo + isnull(rc.peso_maximo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN rc.peso_maximo is not null AND ((rc.peso_maximo_tolerancia_negativa = 0 OR rc.peso_maximo_tolerancia_negativa is null) AND (rc.peso_maximo_tolerancia_positiva = 0 OR rc.peso_maximo_tolerancia_positiva is null))
		THEN CAST(rc.peso_maximo AS varchar)
	ELSE NULL
END AS peso_maximo_tolerancias,
CAST(rc.peso_minimo_tolerancia_negativa AS varchar) AS peso_minimo_tolerancia_negativa, CAST (rc.peso_minimo_tolerancia_positiva AS varchar) AS peso_minimo_tolerancia_positiva,
CASE
	WHEN rc.peso_minimo is not null AND NOT ((rc.peso_minimo_tolerancia_negativa = 0 OR rc.peso_minimo_tolerancia_negativa is null) AND (rc.peso_minimo_tolerancia_positiva = 0 OR rc.peso_minimo_tolerancia_positiva is null))
		THEN CAST(rc.peso_minimo + isnull(rc.peso_minimo_tolerancia_negativa,0) AS varchar)+' - '+CAST(rc.peso_minimo + isnull(rc.peso_minimo_tolerancia_positiva,0) AS VARCHAR) 
	WHEN rc.peso_minimo is not null AND ((rc.peso_minimo_tolerancia_negativa = 0 OR rc.peso_minimo_tolerancia_negativa is null) AND (rc.peso_minimo_tolerancia_positiva = 0 OR rc.peso_minimo_tolerancia_positiva is null))
		THEN CAST(rc.peso_minimo AS varchar)
	ELSE NULL
END AS peso_minimo_tolerancias,
CAST(rc.pieza_doble AS varchar) AS pieza_doble
from SCDM_solicitud_rel_cambio_budget as rc

UNION
--Creación con referencia
SELECT cr.tipo_material_text as tipo_material, cr.id, cr.id_solicitud, 1 As id_tipo_material,
tipo_metal as metal, tv.descripcion as tipo_venta, '\' posicion_rollo, '\' ihs_1, '\' ihs_2, '\' ihs_3, '\' ihs_4, '\' ihs_5,
'\' modelo_negocio, nuevo_material as numero_material, CAST (cr.espesor_mm AS varchar) AS espesor_mm, CAST (cr.espesor_tolerancia_negativa_mm as varchar) AS  espesor_tolerancia_negativa_mm, 
CAST (cr.espesor_tolerancia_positiva_mm AS varchar) AS espesor_tolerancia_positiva_mm, CAST (cr.ancho_mm AS varchar) AS ancho_mm,
CAST (cr.ancho_tolerancia_negativa_mm AS varchar) AS ancho_tolerancia_negativa_mm, CAST (cr.ancho_tolerancia_positiva_mm AS varchar) AS ancho_tolerancia_positiva_mm,
'\' as peso_min_kg, '\' requiera_consiliacion_puntas_colar, '\' scrap_permitido_puntas_colas, 
CAST (cr.avance_mm AS varchar) AS avance_mm, CAST (cr.avance_tolerancia_negativa_mm AS varchar) AS avance_tolerancia_negativa_mm, cast (cr.avance_tolerancia_positiva_mm as varchar) as avance_tolerancia_positiva_mm,
'\' piezas_por_golpe, '\' piezas_por_paquete, '\' peso_real_bruto, '\' peso_real_neto, '\' piezas_por_auto, '\' peso_inicial, '\' porcentaje_scrap_puntas_colas,
'\' coinciliacion_scrap_ingenieria, '\' angulo_a, '\' angulo_b, 
CASE
	WHEN tv.id = 3 --Reaplicación
	THEN '1'
	ELSE '0'
END AS reaplicacion,
cr.ejecucion_correcta_budget as budget_ejecucion_correcta, cr.resultado_budget as budget_mensaje_sap,
CASE
	WHEN cr.espesor_mm is not null AND NOT ((cr.espesor_tolerancia_negativa_mm = 0 OR cr.espesor_tolerancia_negativa_mm is null) AND (cr.espesor_tolerancia_positiva_mm = 0 OR cr.espesor_tolerancia_positiva_mm is null))
		THEN CAST(cr.espesor_mm + isnull(cr.espesor_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(cr.espesor_mm  + isnull(cr.espesor_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN cr.espesor_mm is not null AND ((cr.espesor_tolerancia_negativa_mm = 0 OR cr.espesor_tolerancia_negativa_mm is null) AND (cr.espesor_tolerancia_positiva_mm = 0 OR cr.espesor_tolerancia_positiva_mm is null))
		THEN CAST(cr.espesor_mm AS varchar)
	ELSE NULL
END AS espesor_tolerancias,
CASE
	WHEN cr.ancho_mm is not null AND NOT ((cr.ancho_tolerancia_negativa_mm = 0 OR cr.ancho_tolerancia_negativa_mm is null) AND (cr.ancho_tolerancia_positiva_mm = 0 OR cr.ancho_tolerancia_positiva_mm is null))
		THEN CAST(cr.ancho_mm + isnull(cr.ancho_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(cr.ancho_mm  + isnull(cr.ancho_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN cr.ancho_mm is not null AND ((cr.ancho_tolerancia_negativa_mm = 0 OR cr.ancho_tolerancia_negativa_mm is null) AND (cr.ancho_tolerancia_positiva_mm = 0 OR cr.ancho_tolerancia_positiva_mm is null))
		THEN CAST(cr.ancho_mm AS varchar)
	ELSE NULL
END AS ancho_tolerancias,
CAST(cr.ancho_mm AS varchar) as ancho_budget,
CASE
	WHEN cr.avance_mm is not null AND NOT ((cr.avance_tolerancia_negativa_mm = 0 OR cr.avance_tolerancia_negativa_mm is null) AND (cr.avance_tolerancia_positiva_mm = 0 OR cr.avance_tolerancia_positiva_mm is null))
		THEN CAST(cr.avance_mm + isnull(cr.avance_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(cr.avance_mm  + isnull(cr.avance_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN cr.avance_mm is not null AND ((cr.avance_tolerancia_negativa_mm = 0 OR cr.avance_tolerancia_negativa_mm is null) AND (cr.avance_tolerancia_positiva_mm = 0 OR cr.avance_tolerancia_positiva_mm is null))
		THEN CAST(cr.avance_mm AS varchar)
	ELSE NULL
END AS avance_tolerancias,
'\' peso_maximo_budget, '\' peso_maximo_tolerancia_negativa, '\' peso_maximo_tolerancia_positiva, '\' peso_maximo_tolerancias, '\' peso_minimo_tolerancia_negativa,
'\' peso_minimo_tolerancia_positiva, '\' peso_minimo_tolerancias, '\' pieza_doble
FROM SCDM_solicitud_rel_creacion_referencia as cr
join SCDM_cat_tipo_venta as tv on tv.id = cr.id_tipo_venta

UNION
--Cambio de Ingenieria
SELECT ci.tipo_material_text as tipo_material, ci.id, ci.id_solicitud, 1 as id_tipo_material, ci.tipo_metal as metal,
ci.tipo_venta, '\' posicion_rollo,'\' ihs_1, '\' ihs_2, '\' ihs_3, '\' ihs_4, '\' ihs_5,
'\' modelo_negocio, material_existente as numero_material,
CAST (ci.espesor_mm AS varchar) AS espesor_mm, CAST (ci.espesor_tolerancia_negativa_mm as varchar) AS  espesor_tolerancia_negativa_mm, 
CAST (ci.espesor_tolerancia_positiva_mm AS varchar) AS espesor_tolerancia_positiva_mm, CAST (ci.ancho_mm AS varchar) AS ancho_mm,
CAST (ci.ancho_tolerancia_negativa_mm AS varchar) AS ancho_tolerancia_negativa_mm, CAST (ci.ancho_tolerancia_positiva_mm AS varchar) AS ancho_tolerancia_positiva_mm,
'\' as peso_min_kg, '\' requiera_consiliacion_puntas_colar, '\' scrap_permitido_puntas_colas, 
CAST (ci.avance_mm AS varchar) AS avance_mm, CAST (ci.avance_tolerancia_negativa_mm AS varchar) AS avance_tolerancia_negativa_mm, cast (ci.avance_tolerancia_positiva_mm as varchar) as avance_tolerancia_positiva_mm,
'\' piezas_por_golpe, '\' piezas_por_paquete, '\' peso_real_bruto, '\' peso_real_neto, '\' piezas_por_auto, '\' peso_inicial, '\' porcentaje_scrap_puntas_colas,
'\' coinciliacion_scrap_ingenieria, '\' angulo_a, '\' angulo_b,
CASE
	WHEN ci.tipo_venta = 'Reaplicación' --Reaplicación
	THEN '1'
	ELSE '0'
END AS reaplicacion,
ci.ejecucion_correcta_budget as budget_ejecucion_correcta, ci.resultado_budget as budget_mensaje_sap,
CASE
	WHEN ci.espesor_mm is not null AND NOT ((ci.espesor_tolerancia_negativa_mm = 0 OR ci.espesor_tolerancia_negativa_mm is null) AND (ci.espesor_tolerancia_positiva_mm = 0 OR ci.espesor_tolerancia_positiva_mm is null))
		THEN CAST(ci.espesor_mm + isnull(ci.espesor_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(ci.espesor_mm  + isnull(ci.espesor_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN ci.espesor_mm is not null AND ((ci.espesor_tolerancia_negativa_mm = 0 OR ci.espesor_tolerancia_negativa_mm is null) AND (ci.espesor_tolerancia_positiva_mm = 0 OR ci.espesor_tolerancia_positiva_mm is null))
		THEN CAST(ci.espesor_mm AS varchar)
	ELSE NULL
END AS espesor_tolerancias,
CASE
	WHEN ci.ancho_mm is not null AND NOT ((ci.ancho_tolerancia_negativa_mm = 0 OR ci.ancho_tolerancia_negativa_mm is null) AND (ci.ancho_tolerancia_positiva_mm = 0 OR ci.ancho_tolerancia_positiva_mm is null))
		THEN CAST(ci.ancho_mm + isnull(ci.ancho_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(ci.ancho_mm  + isnull(ci.ancho_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN ci.ancho_mm is not null AND ((ci.ancho_tolerancia_negativa_mm = 0 OR ci.ancho_tolerancia_negativa_mm is null) AND (ci.ancho_tolerancia_positiva_mm = 0 OR ci.ancho_tolerancia_positiva_mm is null))
		THEN CAST(ci.ancho_mm AS varchar)
	ELSE NULL
END AS ancho_tolerancias,
CAST(ci.ancho_mm AS varchar) as ancho_budget,
CASE
	WHEN ci.avance_mm is not null AND NOT ((ci.avance_tolerancia_negativa_mm = 0 OR ci.avance_tolerancia_negativa_mm is null) AND (ci.avance_tolerancia_positiva_mm = 0 OR ci.avance_tolerancia_positiva_mm is null))
		THEN CAST(ci.avance_mm + isnull(ci.avance_tolerancia_negativa_mm,0) AS varchar)+' - '+CAST(ci.avance_mm  + isnull(ci.avance_tolerancia_positiva_mm,0) AS VARCHAR) 
	WHEN ci.avance_mm is not null AND ((ci.avance_tolerancia_negativa_mm = 0 OR ci.avance_tolerancia_negativa_mm is null) AND (ci.avance_tolerancia_positiva_mm = 0 OR ci.avance_tolerancia_positiva_mm is null))
		THEN CAST(ci.avance_mm AS varchar)
	ELSE NULL
END AS avance_tolerancias,
'\' peso_maximo_budget, '\' peso_maximo_tolerancia_negativa, '\' peso_maximo_tolerancia_positiva, '\' peso_maximo_tolerancias, '\' peso_minimo_tolerancia_negativa,
'\' peso_minimo_tolerancia_positiva, '\' peso_minimo_tolerancias, '\' pieza_doble
from SCDM_solicitud_rel_cambio_ingenieria as ci

GO
