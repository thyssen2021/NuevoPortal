use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_produccion_resultados;  
/*****************************************************************************
*  Tipo de objeto: View
*  Funcion: Vista para mostrar los resultados de bitácoras
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 10/28/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
		
GO
CREATE VIEW [dbo].[view_produccion_resultados] AS
(SELECT 
		ISNULL(ROW_NUMBER() OVER (ORDER BY s.id), 0) as id
		,s.Operador
		,s.Supervisor
		,s.[SAP Platina]
		,s.[Tipo de Material]
		,s.[Número de Parte de Cliente]
		,s.[SAP Rollo]
		,s.Material
		,s.Fecha
		,s.descripcion AS [Turno]
		,s.Hora
		,s.[Orden SAP]
		,s.[Orden en SAP 2]
		,s.[Pieza por Golpe]
		,s.[N° de Rollo]
		,s.[Lote de rollo]
		,s.[Peso Etiqueta (Kg)]
		,s.[Peso de regreso de rollo Real]
		,s.[Peso de rollo usado ]
		,s.[Peso Báscula Kgs]
		,s.[N° Lote izquierdo]
		,s.[N° Lote derecho]
		,s.#
		,s.[Piezas por paquete]
		,s.[Total de piezas]
		,s.[Numero de golpes]
		,s.[Kg restante de rollo]
		,s.[Peso despunte kgs#]
		,s.[Peso cola Kgs#]
		,s.[Porcentaje de puntas y colas]
		,s.[Total de piezas de Ajustes]
		,s.[Peso Bruto Kgs]
		,s.[Peso Real Pieza Bruto]
		,s.[Peso Real Pieza Neto]
		,s.[Scrap Natural]
		,s.[Peso neto SAP]
		,s.[Peso Bruto SAP]
		,s.[Balance de Scrap]
		,s.Column37
		,s.Column38
		,s.Column39
		,s.Column40
		,s.[Ordenes por pieza]
		,s.[Peso de rollo usado real _Kg]
		,s.[Peso bruto Total piezas_Kg]
		,s.[Peso NetoTotal piezas_Kg]
		,s.[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
		,s.[Peso Neto_total piezas de ajuste_Kgs]
		,s.[Peso puntas y colas reales_Kg]					
		,(ISNULL(s.[Peso Bruto Kgs], 0) + ISNULL(s.[Peso puntas y colas reales_Kg], 0))/NULLIF(CAST(s.[Peso de rollo usado real _Kg] AS float),0) AS [Balance_de_Scrap_Real]
FROM(
SELECT 
	r.*
	,(ISNULL(r.[Peso despunte kgs#], 0) + ISNULL(r.[Peso cola Kgs#], 0) + ISNULL(r.[Peso Bruto Kgs], 0))/NULLIF(CAST(r.[Peso Etiqueta (Kg)] AS float),0) AS [Balance de Scrap]
	,((ISNULL(r.[Peso bruto Total piezas_Kg], 0) - ISNULL(r.[Peso NetoTotal piezas_Kg], 0))+(ISNULL(r.[Total de piezas de Ajustes], 0) * ISNULL(r.[Scrap Natural], 0)))  AS [Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
	,(SELECT (
	CASE
	WHEN  (ISNULL(r.[Peso de rollo usado real _Kg], 0) -(ISNULL(r.[Peso bruto Total piezas_Kg], 0)+ISNULL(r.[Peso Bruto Kgs], 0)) )<0 
		THEN 0
		ELSE 
		(ISNULL(r.[Peso de rollo usado real _Kg], 0) -(ISNULL(r.[Peso bruto Total piezas_Kg], 0)+ISNULL(r.[Peso Bruto Kgs], 0)) )
		END
	)) AS [Peso puntas y colas reales_Kg]
FROM 

(SELECT 
	q.*
	,(ISNULL(q.[Total de piezas de Ajustes], 0) * ISNULL(q.[Peso Real Pieza Bruto], 0)) AS [Peso Bruto Kgs]
	,(ISNULL(q.[Peso Real Pieza Bruto], 0) - ISNULL(q.[Peso Real Pieza Neto], 0)) AS [Scrap Natural]
	,(ISNULL(q.[Total de piezas], 0) * ISNULL(q.[Peso Real Pieza Bruto], 0)* ISNULL(q.[Ordenes por pieza], 0)) AS [Peso bruto Total piezas_Kg]
FROM 
(SELECT 
	p.*
	,(p.[Peso Bruto SAP]/NULLIF(CAST(p.[Peso neto SAP] AS float),0))*p.[Peso Real Pieza Neto] AS [Peso Real Pieza Bruto] 
	,p.[Total de piezas] * p.[Peso Real Pieza Neto] * p.[Ordenes por pieza] AS [Peso NetoTotal piezas_Kg] 
FROM 
(SELECT  
	t.*
	,(ISNULL(t.[Peso Báscula Kgs], 0) - ISNULL(t.[Peso de regreso de rollo Real], 0)) AS [Peso de rollo usado real _Kg]
	,t.[Total de piezas]/ NULLIF(CAST(t.[Pieza por Golpe] AS float),0)  AS [Numero de golpes]
					
FROM 
(
SELECT 				
	(em.nombre+' '+em.apellido1+' '+em.apellido2)as Operador 
	,(ems.nombre+' '+ems.apellido1+' '+ems.apellido2)as Supervisor
	,pr.sap_platina AS [SAP Platina]
	,(SELECT TOP (1) [Type of Material] FROM mm_v3 WHERE Material=pr.sap_platina) AS [Tipo de Material]
	,(SELECT TOP (1) [Customer part number] FROM class_v3 WHERE Object=pr.sap_platina) AS [Número de Parte de Cliente]
	,pr.sap_rollo AS [SAP Rollo]
	,(SELECT TOP (1) [Old material no#] FROM mm_v3 WHERE Material=pr.sap_platina) AS [Material]
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
	,ISNULL((SELECT SUM(piezas_paquete) FROM produccion_lotes WHERE id_produccion_registro=pr.id),0) AS [Total de piezas]
	,NULL AS [Kg restante de rollo] --no disponible en las nuevas bitácoras,
	,pd.peso_despunte_kgs AS [Peso despunte kgs#]
	,pd.peso_cola_kgs AS [Peso cola Kgs#]
	,pd.total_piezas_ajuste AS [Total de piezas de Ajustes]
	,(SELECT (ISNULL(pd.peso_despunte_kgs,0)+ISNULL(pd.peso_cola_kgs,0))/ NULLIF(CAST(pd.peso_bascula_kgs AS float) ,0) ) AS [Porcentaje de puntas y colas]
	,pd.peso_real_pieza_neto AS [Peso Real Pieza Neto]
	,(SELECT TOP (1) [Net weight] FROM mm_v3 WHERE Material=pr.sap_platina) AS [Peso neto SAP]
	,(SELECT TOP (1) [Gross weight] FROM mm_v3 WHERE Material=pr.sap_platina) AS [Peso Bruto SAP]
	,pl.linea AS [Column37]
	,plt.descripcion AS [Column38]
	,YEAR(pr.fecha) AS [Column39]
	,pr.id AS [Column40]
	,pd.ordenes_por_pieza AS [Ordenes por pieza]
	,(ISNULL(pd.total_piezas_ajuste, 0) * ISNULL(pd.peso_real_pieza_neto, 0)) AS [Peso Neto_total piezas de ajuste_Kgs]
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
	)t)p)q)r)s
	)
				