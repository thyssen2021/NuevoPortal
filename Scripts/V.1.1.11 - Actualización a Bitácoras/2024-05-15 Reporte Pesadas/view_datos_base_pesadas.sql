/****** Object:  View [dbo].[view_datos_base_reporte_pesadas]    Script Date: 17/05/2024 09:59:17 a.m. ******/
--DROP VIEW [dbo].[view_datos_base_reporte_pesadas]
--GO

CREATE VIEW view_datos_base_reporte_pesadas  
AS  
select  t.*, (t.gross_weight/ t.net_weight)* t.peso_real_pieza_neto AS peso_real_pieza_bruto from (
SELECT  
	pr.id,  
	pr.fecha, 
	pr.clave_planta, 
	pr.sap_platina as [sap_platina], --Platina 1
	pr.sap_rollo,
	(SELECT SUM(pl.piezas_paquete) from produccion_lotes as pl where pl.id_produccion_registro = pr.id and pl.sap_platina = pr.sap_platina) AS [total_piezas],
	 (CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as gross_weight,
	/*obtiene net weight*/
    (CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as net_weight,
	pde.peso_real_pieza_neto as [peso_real_pieza_neto],
	pde.peso_etiqueta,
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
	 (CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSSTART) from bom_pesos  WHERE plant = pl.codigoSap AND material=pr.sap_platina)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSSTART) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as fecha_inicio_validez_peso_bom,
	 (CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSEND) from bom_pesos  WHERE plant = pl.codigoSap AND material=pr.sap_platina)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSEND) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as fecha_fin_validez_peso_bom
	
FROM dbo.produccion_datos_entrada as pde
join produccion_registros pr on pde.id_produccion_registro = pr.id
join plantas as pl on pl.clave = pr.clave_planta 
join class_v3 as clss on clss.Object = pr.sap_platina 
join mm_v3 as mm on mm.Material = pr.sap_platina and mm.Plnt = pl.codigoSap
join clientes as clt on clt.claveSAP = clss.Customer
--where pr.id = 44146
) t
UNION ALL
-- une con platina 2
select  t.*, (t.gross_weight/ t.net_weight)* t.peso_real_pieza_neto AS peso_real_pieza_bruto from (
SELECT
	pr.id,  
	pr.fecha, 
	pr.clave_planta, 
	pr.sap_platina_2, 
	pr.sap_rollo,
	(SELECT SUM(pl.piezas_paquete) from produccion_lotes as pl where pl.id_produccion_registro = pr.id and pl.sap_platina = pr.sap_platina_2) AS [total_piezas],
	 (CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as gross_weight,
	/*obtiene net weight*/
    (CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as net_weight,
	pde.peso_real_pieza_neto_platina_2 as [peso_real_pieza_neto],
	pde.peso_etiqueta,
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
	(CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSSTART) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSSTART) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as fecha_inicio_validez_peso_bom,
	(CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSEND) from bom_pesos WHERE plant = pl.codigoSap AND material=pr.sap_platina_2)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 DATEADD(HOUR, -6, SYSEND) from bom_pesos_history WHERE plant = pl.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as fecha_fin_validez_peso_bom	
FROM dbo.produccion_datos_entrada as pde
join produccion_registros pr on pde.id_produccion_registro = pr.id
join plantas as pl on pl.clave = pr.clave_planta 
join class_v3 as clss on clss.Object = pr.sap_platina 
join mm_v3 as mm on mm.Material = pr.sap_platina and mm.Plnt = pl.codigoSap
join clientes as clt on clt.claveSAP = clss.Customer
AND pr.sap_platina_2 IS NOT NULL 
--where pr.id = 44146
)t

GO
select * from view_datos_base_reporte_pesadas where id =56428

