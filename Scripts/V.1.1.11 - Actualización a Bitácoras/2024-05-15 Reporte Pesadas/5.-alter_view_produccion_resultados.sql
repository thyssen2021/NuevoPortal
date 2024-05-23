--USE [Portal_2_0]
GO

/****** Object:  View [dbo].[view_produccion_resultados]    Script Date: 23/05/2024 12:18:20 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE OR ALTER   VIEW [dbo].[view_produccion_resultados] AS

SELECT 
ISNULL(ROW_NUMBER() OVER (ORDER BY w.Column40), 0) as id
		,w.*
		,w.[Peso Bruto Kgs] + w.[Peso Bruto Kgs_platina2] as [Peso Bruto Kgs_general]  
		,ISNULL(w.[Peso Real Pieza Bruto],0) + ISNULL(w.[Peso Real Pieza Bruto_platina2],0) as [Peso Real Pieza Bruto_general]
		,ISNULL(w.[Peso Real Pieza Neto],0) + ISNULL(w.[Peso Real Pieza Neto_platina2],0) as [Peso Real Pieza Neto_general]		
		,ISNULL(w.[Scrap Natural],0) + ISNULL(w.[Scrap Natural_platina2],0) as [Scrap Natural_general]
		,ISNULL(w.[Peso neto SAP],0) + ISNULL(w.[Peso neto SAP_platina2],0) as [Peso neto SAP_general]
		,ISNULL(w.[Peso Bruto SAP],0) + ISNULL(w.[Peso Bruto SAP_platina2],0) as [Peso Bruto SAP_general]
		,(ISNULL(w.[Peso despunte kgs#],0)+ ISNULL(w.[Peso cola Kgs#],0) +ISNULL(w.[Peso Bruto Kgs],0) + ISNULL(w.[Peso Bruto Kgs_platina2],0))/ NULLIF( w.[Peso Etiqueta (Kg)],0) as [Balance de Scrap_general] 	
		,ISNULL(w.[Peso de rollo usado real _Kg],0) + ISNULL(w.[Peso de rollo usado real _Kg_platina2],0) as [Peso de rollo usado real _Kg_general]	
		,ISNULL(w.[Peso bruto Total piezas_Kg],0) + ISNULL(w.[Peso bruto Total piezas_Kg_platina2],0) as [Peso bruto Total piezas_Kg_general]		
		,ISNULL(w.[Peso NetoTotal piezas_Kg],0) + ISNULL(w.[Peso NetoTotal piezas_Kg_platina2],0) as [Peso NetoTotal piezas_Kg_general]
		,ISNULL(w.[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg],0) + ISNULL([Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg_platina2],0) as [Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg_general]	
		,ISNULL(w.[Peso Neto_total piezas de ajuste_Kgs],0) + ISNULL([Peso Neto_total piezas de ajuste_Kgs_platina2],0) as [Peso Neto_total piezas de ajuste_Kgs_general]
		,ISNULL(w.[Peso puntas y colas reales_Kg],0) + ISNULL(w.[Peso puntas y colas reales_Kg_platina2],0) as [Peso puntas y colas reales_Kg_general]			
		,(ISNULL(w.[Peso puntas y colas reales_Kg],0)+ISNULL(w.[Peso puntas y colas reales_Kg_platina2],0)+ ISNULL(w.[Peso Bruto Kgs],0) + ISNULL(w.[Peso Bruto Kgs_platina2],0))/NULLIF((ISNULL(w.[Peso de rollo usado real _Kg],0) + ISNULL(w.[Peso de rollo usado real _Kg_platina2],0 )),0) as [Balance_de_Scrap_Real_general]
	FROM(
SELECT	
	v.* FROM (
SELECT 		
		s.Operador
		,s.Supervisor
		--sap platina 1
		,s.[Orden SAP]
		,s.[SAP Platina]		
		,s.[Tipo de Material]
		,s.[Número de Parte de Cliente]
		,s.[Material]				
		--sap platina 2
		,s.[Orden en SAP 2]
		,s.[SAP Platina 2]
		,s.[Tipo de Material_platina2]
		,s.[Número de Parte de Cliente_platina2]
		,s.[Material_platina2]	
		--
		,s.[SAP Rollo]		
		,s.Fecha
		,s.descripcion AS [Turno]
		,s.Hora
		,s.[Pieza por Golpe]		
		,s.[Ordenes por pieza]
		,s.[N° de Rollo]
		,s.[Lote de rollo]
		,s.[Peso Etiqueta (Kg)]
		,s.[Peso de regreso de rollo Real]
		,s.[Peso de rollo usado ]
		,s.[Peso Báscula Kgs]
		--Lotes obsoletos
		,s.[N° Lote izquierdo]
		,s.[N° Lote derecho]
		,s.#
		,s.[Piezas por paquete]
		---
		,s.[Total de piezas_platina1] 
		,s.[Total de piezas_platina2] 
		,s.[sum_total_piezas] AS [Total de piezas]
		,s.[Numero de golpes]
		,s.[Kg restante de rollo]
		,s.[Peso despunte kgs#]
		,s.[Peso cola Kgs#]
		,s.[Porcentaje de puntas y colas]
		-- piezas de ajuste
		,s.[Total de piezas de Ajustes_platina1]
		,s.[Total de piezas de Ajustes_platina2]
		,s.[Total de piezas de Ajustes]

		-- datos de sap platina 1
		,s.[Peso Bruto Kgs]	
		,s.[Peso Real Pieza Bruto]
		,s.[Peso Real Pieza Neto]		
		,s.[Scrap Natural]
		,s.[Peso neto SAP]
		,s.[Peso Bruto SAP]
		,s.[Balance de Scrap]	
		,(s.[Peso de rollo usado real _Kg] * s.porcentaje_platina_1) AS [Peso de rollo usado real _Kg]	
		,s.[Peso bruto Total piezas_Kg]		
		,s.[Peso NetoTotal piezas_Kg]
		,s.[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]	
		,s.[Peso Neto_total piezas de ajuste_Kgs]
		,s.[Peso puntas y colas reales_Kg]			
		,(ISNULL((ISNULL(s.[Peso Bruto Kgs], 0) + ISNULL(s.[Peso puntas y colas reales_Kg], 0))/NULLIF(CAST(s.[Peso de rollo usado real _Kg]* s.porcentaje_platina_1  AS float),0),0)) AS [Balance_de_Scrap_Real]
	    -- datos platina 2
		,s.[Peso Bruto Kgs_platina2]	
		,s.[Peso Real Pieza Bruto_platina2]
		,s.[Peso Real Pieza Neto_platina2]		
		,s.[Scrap Natural_platina2]
		,s.[Peso neto SAP_platina2]
		,s.[Peso Bruto SAP_platina2]
		,s.[Balance de Scrap_platina2]
		,(s.[Peso de rollo usado real _Kg] * s.porcentaje_platina_2) AS [Peso de rollo usado real _Kg_platina2]	
		,s.[Peso bruto Total piezas_Kg_platina2]	
		,s.[Peso NetoTotal piezas_Kg_platina2]
		,s.[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg_platina2]	
		,s.[Peso Neto_total piezas de ajuste_Kgs_platina2]		
		,s.[Peso puntas y colas reales_Kg_platina2]			
		,(ISNULL((ISNULL(s.[Peso Bruto Kgs_platina2], 0) + ISNULL(s.[Peso puntas y colas reales_Kg_platina2], 0))/NULLIF(CAST(s.[Peso de rollo usado real _Kg]* s.porcentaje_platina_2  AS float),0),0)) AS [Balance_de_Scrap_Real_platina2]
		-- datos del registro
		,s.Column37
		,s.Column38
		,s.Column39
		,s.Column40		
FROM(
SELECT 
	r.*
	,(ISNULL(r.[Peso despunte kgs#]* r.porcentaje_platina_1, 0) + ISNULL(r.[Peso cola Kgs#]* r.porcentaje_platina_1, 0) + ISNULL(r.[Peso Bruto Kgs], 0))/NULLIF(CAST(r.[Peso Etiqueta (Kg)] * r.porcentaje_platina_1 AS float),0) AS [Balance de Scrap]
	,((ISNULL(r.[Peso bruto Total piezas_Kg], 0) - ISNULL(r.[Peso NetoTotal piezas_Kg], 0))+(ISNULL(r.[Total de piezas de Ajustes], 0) * ISNULL(r.[Scrap Natural], 0)))  AS [Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
	,(SELECT (
	CASE
	WHEN  ((ISNULL(r.[Peso de rollo usado real _Kg], 0)* r.porcentaje_platina_1) -(ISNULL(r.[Peso bruto Total piezas_Kg], 0)+ISNULL(r.[Peso Bruto Kgs], 0)) )<0 
		THEN 0
		ELSE 
		((ISNULL(r.[Peso de rollo usado real _Kg], 0)* r.porcentaje_platina_1) -(ISNULL(r.[Peso bruto Total piezas_Kg], 0)+ISNULL(r.[Peso Bruto Kgs], 0)) )
		END
	)) AS [Peso puntas y colas reales_Kg]	
	--datos platina 2
	,(ISNULL(r.[Peso despunte kgs#]* r.porcentaje_platina_1, 0) + ISNULL(r.[Peso cola Kgs#]* r.porcentaje_platina_1, 0) + ISNULL(r.[Peso Bruto Kgs], 0))/NULLIF(CAST(r.[Peso Etiqueta (Kg)] * r.porcentaje_platina_2 AS float),0) AS [Balance de Scrap_platina2]
	,((ISNULL(r.[Peso bruto Total piezas_Kg_platina2], 0) - ISNULL(r.[Peso NetoTotal piezas_Kg_platina2], 0))+(ISNULL(r.[Total de piezas de Ajustes_platina2], 0) * ISNULL(r.[Scrap Natural_platina2], 0)))  AS [Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg_platina2]
	,(SELECT (
	CASE
	WHEN  ((ISNULL(r.[Peso de rollo usado real _Kg], 0)* r.porcentaje_platina_2) -(ISNULL(r.[Peso bruto Total piezas_Kg_platina2], 0)+ISNULL(r.[Peso Bruto Kgs_platina2], 0)) )<0 
		THEN 0
		ELSE 
		((ISNULL(r.[Peso de rollo usado real _Kg], 0)* r.porcentaje_platina_2) -(ISNULL(r.[Peso bruto Total piezas_Kg_platina2], 0)+ISNULL(r.[Peso Bruto Kgs_platina2], 0)) )
		END
	)) AS [Peso puntas y colas reales_Kg_platina2]	
FROM 

(SELECT 
	q.*
	,(ISNULL(q.[Total de piezas de Ajustes_platina1], 0) * ISNULL(q.[Peso Real Pieza Bruto], 0)) AS [Peso Bruto Kgs]
	,(ISNULL(q.[Peso Real Pieza Bruto], 0) - ISNULL(q.[Peso Real Pieza Neto], 0)) AS [Scrap Natural]
	,(ISNULL(q.[Total de piezas_platina1], 0) * ISNULL(q.[Peso Real Pieza Bruto], 0)* ISNULL(q.[Ordenes por pieza], 0)) AS [Peso bruto Total piezas_Kg]
	,(ISNULL(q.[Total de piezas de Ajustes_platina2], 0) * ISNULL(q.[Peso Real Pieza Bruto_platina2], 0)) AS [Peso Bruto Kgs_platina2]
	,(ISNULL(q.[Peso Real Pieza Bruto_platina2], 0) - ISNULL(q.[Peso Real Pieza Neto_platina2], 0)) AS [Scrap Natural_platina2]
	,(ISNULL(q.[Total de piezas_platina2], 0) * ISNULL(q.[Peso Real Pieza Bruto_platina2], 0)* ISNULL(q.[Ordenes por pieza], 0)) AS [Peso bruto Total piezas_Kg_platina2]
		--porcentajes rollo utilizado
	,ISNULL(q.[Peso Real Pieza Bruto] / NULLIF((ISNULL(q.[Peso Real Pieza Bruto],0)+ISNULL(q.[Peso Real Pieza Bruto_platina2],0)),0),0) AS porcentaje_platina_1
	,ISNULL(q.[Peso Real Pieza Bruto_platina2] / NULLIF((ISNULL(q.[Peso Real Pieza Bruto],0)+ISNULL(q.[Peso Real Pieza Bruto_platina2],0)),0),0) AS porcentaje_platina_2
FROM 
(SELECT 
	p.*
	,(p.[Peso Bruto SAP]/NULLIF(CAST(p.[Peso neto SAP] AS float),0))*p.[Peso Real Pieza Neto] AS [Peso Real Pieza Bruto] 
	,p.[Total de piezas_platina1] * p.[Peso Real Pieza Neto] * p.[Ordenes por pieza] AS [Peso NetoTotal piezas_Kg] 
	,(p.[Peso Bruto SAP_platina2]/NULLIF(CAST(p.[Peso neto SAP_platina2] AS float),0))*p.[Peso Real Pieza Neto_platina2] AS [Peso Real Pieza Bruto_platina2] 
	,p.[Total de piezas_platina2] * p.[Peso Real Pieza Neto_platina2] * p.[Ordenes por pieza] AS [Peso NetoTotal piezas_Kg_platina2] 
FROM 
(SELECT  
	t.*
	,(ISNULL(t.[Peso Báscula Kgs], 0) - ISNULL(t.[Peso de regreso de rollo Real], 0)) AS [Peso de rollo usado real _Kg]
	,t.[sum_total_piezas]/ NULLIF(CAST(t.[Pieza por Golpe] AS float),0)  AS [Numero de golpes]					
FROM 
(
SELECT 				
	(em.nombre+' '+em.apellido1+' '+em.apellido2)as Operador 
	,(ems.nombre+' '+ems.apellido1+' '+ems.apellido2)as Supervisor
	,pr.sap_platina AS [SAP Platina]
	,pr.sap_platina_2 AS [SAP Platina 2]
	,(SELECT TOP (1) [Type of Material] FROM mm_v3 WHERE Material=pr.sap_platina) AS [Tipo de Material]
	,(SELECT TOP (1) [Customer part number] FROM class_v3 WHERE Object=pr.sap_platina) AS [Número de Parte de Cliente]
	,pr.sap_rollo AS [SAP Rollo]
	,(SELECT TOP (1) [Old material no#] FROM mm_v3 WHERE Material=pr.sap_platina) AS [Material]
	--material 2
	,(SELECT TOP (1) [Type of Material] FROM mm_v3 WHERE Material=pr.sap_platina_2) AS [Tipo de Material_platina2]
	,(SELECT TOP (1) [Customer part number] FROM class_v3 WHERE Object=pr.sap_platina_2) AS [Número de Parte de Cliente_platina2]
	,(SELECT TOP (1) [Old material no#] FROM mm_v3 WHERE Material=pr.sap_platina_2) AS [Material_platina2]
	,pr.fecha AS [Fecha]
	,pr.id
	,pt.descripcion
	,pr.fecha AS [Hora]
	,pd.orden_sap AS [Orden SAP]
	,pd.orden_sap_2 AS [Orden en SAP 2]
	,pd.piezas_por_golpe AS [Pieza por Golpe]
	,pd.numero_rollo AS [N° de Rollo]
	,pd.lote_rollo AS [Lote de rollo]
	,pd.peso_etiqueta AS [Peso Etiqueta (Kg)]
	,pd.peso_regreso_rollo_real AS [Peso de regreso de rollo Real]
	,(ISNULL(pd.peso_etiqueta, 0) - ISNULL(pd.peso_regreso_rollo_real, 0)) AS [Peso de rollo usado ]
	,pd.peso_bascula_kgs AS [Peso Báscula Kgs]
	,NULL AS [N° Lote izquierdo] --no representa información relevante
	,NULL AS [N° Lote derecho] --no representa información relevante
	,NULL AS [#] --no representa información relevante
	,NULL AS [Piezas por paquete] --no representa información relevante
	,ISNULL((SELECT SUM(piezas_paquete) FROM produccion_lotes as l WHERE id_produccion_registro=pr.id AND (l.sap_platina = pr.sap_platina OR l.sap_platina IS NULL )),0) AS [Total de piezas_platina1]
	,ISNULL((SELECT SUM(piezas_paquete) FROM produccion_lotes as l WHERE id_produccion_registro=pr.id AND (l.sap_platina = pr.sap_platina_2)),0) AS [Total de piezas_platina2]
	,ISNULL((SELECT SUM(piezas_paquete) FROM produccion_lotes as l WHERE id_produccion_registro=pr.id),0) AS [sum_total_piezas]
	,NULL AS [Kg restante de rollo] --no disponible en las nuevas bitácoras,
	,pd.peso_despunte_kgs AS [Peso despunte kgs#]
	,pd.peso_cola_kgs AS [Peso cola Kgs#]
	--piezas de ajuste
	,ISNULL(total_piezas_ajuste,0) AS [Total de piezas de Ajustes_platina1]
	,ISNULL(total_piezas_ajuste_platina_2,0) AS [Total de piezas de Ajustes_platina2]
	,ISNULL((SELECT pd.total_piezas_ajuste + ISNULL(pd.total_piezas_ajuste_platina_2,0)),0)AS [Total de piezas de Ajustes]
	-------
	,(SELECT (ISNULL(pd.peso_despunte_kgs,0)+ISNULL(pd.peso_cola_kgs,0))/ NULLIF(CAST(pd.peso_bascula_kgs AS float) ,0) ) AS [Porcentaje de puntas y colas]	
	--pesos platina 1
	,pd.peso_real_pieza_neto AS [Peso Real Pieza Neto]
	 ,(CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) as [Peso neto SAP]	
	,(CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) AS [Peso Bruto SAP]	
	--pesos platina 2
	,pd.peso_real_pieza_neto_platina_2 AS [Peso Real Pieza Neto_platina2]
	,(CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina_2)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 net_weight from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END) AS [Peso neto SAP_platina2]
	,(CASE
        WHEN (SELECT top 1 count (*) from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos WHERE plant = plt.codigoSap AND material=pr.sap_platina_2)
        WHEN (SELECT top 1 count (*) from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND) = 1 
			THEN (SELECT top 1 gross_weight from bom_pesos_history WHERE plant = plt.codigoSap AND material=pr.sap_platina_2 and DATEADD(HOUR,+6,pr.fecha) >= SYSSTART AND DATEADD(HOUR,+6,pr.fecha)<SYSEND)        
		ELSE NULL
    END)  AS [Peso Bruto SAP_platina2]
	--peso neto total piezas de ajuste
	,(ISNULL(pd.total_piezas_ajuste, 0) * ISNULL(pd.peso_real_pieza_neto, 0)) AS [Peso Neto_total piezas de ajuste_Kgs]
	,(ISNULL(pd.total_piezas_ajuste_platina_2, 0) * ISNULL(pd.peso_real_pieza_neto_platina_2, 0)) AS [Peso Neto_total piezas de ajuste_Kgs_platina2]
	,pl.linea AS [Column37]
	,plt.descripcion AS [Column38]
	,YEAR(pr.fecha) AS [Column39]
	,pr.id AS [Column40]
	,pd.ordenes_por_pieza AS [Ordenes por pieza]
FROM
	[Portal_2_0].[dbo].[produccion_registros] as pr
	JOIN [Portal_2_0].[dbo].[produccion_operadores] as po ON pr.id_operador = po.id
	JOIN [Portal_2_0].[dbo].[produccion_supervisores] as ps ON pr.id_supervisor = ps.id
	JOIN [Portal_2_0].[dbo].[produccion_lineas] AS pl ON pr.id_linea = pl.id
	JOIN [Portal_2_0].[dbo].[plantas] AS plt ON pr.clave_planta = plt.clave
	JOIN [Portal_2_0].[dbo].[empleados] as em ON po.id_empleado = em.id
	JOIN [Portal_2_0].[dbo].[empleados] as ems ON ps.id_empleado = ems.id
	JOIN [Portal_2_0].[dbo].produccion_turnos as pt ON pr.id_turno = pt.id
	JOIN [Portal_2_0].[dbo].produccion_datos_entrada as pd ON pr.id = pd.id_produccion_registro	
	where pr.activo <>0
	)t)p)q)r)s)v)w
GO

