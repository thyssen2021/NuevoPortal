--USE [Portal_2_0]
GO

/****** Object:  View [dbo].[view_datos_base_reporte_pesadas]    Script Date: 12/11/2024 05:17:37 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








CREATE OR ALTER               VIEW [dbo].[view_datos_base_reporte_pesadas]  
AS  
select  t.*, (t.gross_weight/ t.net_weight)* t.peso_real_pieza_neto AS peso_real_pieza_bruto from (
SELECT  
	pr.id,  
	ln.linea,
	pr.fecha, 
	pr.clave_planta, 
	pr.sap_platina as [sap_platina], --Platina 1
	pr.sap_rollo,
	(SELECT SUM(pl.piezas_paquete) from produccion_lotes as pl where pl.id_produccion_registro = pr.id and pl.sap_platina = pr.sap_platina) AS [total_piezas],
	(SELECT gross_weight FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina, pl.codigoSap) as [peso_bruto]) as gross_weight,
	/*obtiene net weight*/
    (SELECT net_weight FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina, pl.codigoSap) as [peso_neto]) as net_weight,
	pde.peso_real_pieza_neto as [peso_real_pieza_neto],
	(CASE
		WHEN (pr.sap_platina_2 IS NOT NULL)
			THEN (pde.peso_etiqueta/2)
		ELSE pde.peso_etiqueta
	 END)as peso_etiqueta,
	--pde.peso_etiqueta,
	clt.descripcion as [invoiced_to],
	mm.[Type of Metal] as [tipo_metal],
	mm.Thickness,
	mm.Width,
	mm.Advance,
	 (CASE
        WHEN  mm.[Type of Metal] like '%ACERO%'
			THEN mm.Thickness * mm.Width * mm.Advance * 7.85 / 1000000
        WHEN mm.[Type of Metal] like '%ALUMI%'
			THEN mm.Thickness * mm.Width * mm.Advance * 2.7 / 1000000        
		ELSE NULL
    END) as peso_teorico,
	-- FECHAS DE VALIDEZ PARA PESO BOM
	(SELECT DATEADD(HOUR, -6, SYSSTART) FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina, pl.codigoSap)) as fecha_inicio_validez_peso_bom,
	(SELECT DATEADD(HOUR, -6, SYSEND) FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina, pl.codigoSap)) as fecha_fin_validez_peso_bom
	
FROM dbo.produccion_datos_entrada as pde
join produccion_registros pr on pde.id_produccion_registro = pr.id
join plantas as pl on pl.clave = pr.clave_planta 
join class_v3 as clss on clss.Object = pr.sap_platina 
join mm_v3 as mm on mm.Material = pr.sap_platina and mm.Plnt = pl.codigoSap
join clientes as clt on clt.claveSAP = clss.Customer
join produccion_lineas as ln on ln.id = pr.id_linea
AND pr.activo = 1
--where pr.id = 44146
) t
UNION ALL
-- une con platina 2
select  t.*, (t.gross_weight/ t.net_weight)* t.peso_real_pieza_neto AS peso_real_pieza_bruto from (
SELECT
	pr.id,  
	ln.linea,
	pr.fecha, 
	pr.clave_planta, 
	pr.sap_platina_2, 
	pr.sap_rollo,
	(SELECT SUM(pl.piezas_paquete) from produccion_lotes as pl where pl.id_produccion_registro = pr.id and pl.sap_platina = pr.sap_platina_2) AS [total_piezas],
	 (SELECT gross_weight FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina_2, pl.codigoSap) as [peso_neto]) as gross_weight,
	/*obtiene net weight*/
    (SELECT net_weight FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina_2,pl.codigoSap) as [peso_neto]) as net_weight,
	pde.peso_real_pieza_neto_platina_2 as [peso_real_pieza_neto],
	(pde.peso_etiqueta/2) as peso_etiqueta,
	clt.descripcion as [invoiced_to],
	mm.[Type of Metal] as [tipo_metal],
	mm.Thickness,
	mm.Width,
	mm.Advance,
	 (CASE
        WHEN  mm.[Type of Metal] like '%ACERO%'
			THEN mm.Thickness * mm.Width * mm.Advance * 7.85 / 1000000
        WHEN mm.[Type of Metal] like '%ALUMI%'
			THEN mm.Thickness * mm.Width * mm.Advance * 2.7 / 1000000        
		ELSE NULL
    END) as peso_teorico,
	-- FECHAS DDE VALIDEZ PESOS BOM
	(SELECT DATEADD(HOUR, -6, SYSSTART) FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina_2, pl.codigoSap)) as fecha_inicio_validez_peso_bom,
	(SELECT DATEADD(HOUR, -6, SYSEND) FROM GetBOM(DATEADD(HH, 6, pr.fecha), pr.sap_platina_2, pl.codigoSap)) as fecha_fin_validez_peso_bom

FROM dbo.produccion_datos_entrada as pde
join produccion_registros pr on pde.id_produccion_registro = pr.id
join plantas as pl on pl.clave = pr.clave_planta 
join class_v3 as clss on clss.Object = pr.sap_platina_2 
join mm_v3 as mm on mm.Material = pr.sap_platina_2 and mm.Plnt = pl.codigoSap
join clientes as clt on clt.claveSAP = clss.Customer
join produccion_lineas as ln on ln.id = pr.id_linea
AND pr.sap_platina_2 IS NOT NULL 
AND pr.activo = 1
--where pr.id = 44146
)t
GO


