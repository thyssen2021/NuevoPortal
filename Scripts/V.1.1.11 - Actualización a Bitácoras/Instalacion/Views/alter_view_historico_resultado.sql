use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_historico_resultado;  
/*****************************************************************************
*  Tipo de objeto: View
*  Funcion: Vista para mostrar el historico de resultados más nuevo
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 11/02/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
		
GO
CREATE VIEW [dbo].view_historico_resultado AS

SELECT
	ISNULL(ROW_NUMBER() OVER (ORDER BY v3.fecha), 0) as id,
	v3.*,
	(SELECT comentarios from dbo.produccion_datos_entrada where v3.Column40>0 AND id_produccion_registro =v3.Column40) as comentario
	FROM (
SELECT       
      [operador] AS [Operador]
      ,[supervisor] as [Supervisor]
      ,[sap_platina] as [SAP Platina]
	  ,null as [SAP Platina 2]
      ,[tipo_material] AS [Tipo de Material]
      ,[numero_parte_cliente] AS [Número de Parte  de cliente]
      ,[sap_rollo] AS [SAP Rollo]
      ,[material] AS [Material]
	  , CASE
            WHEN DATEPART(HOUR, fecha) = 0 AND DATEPART(MINUTE, fecha)= 0 AND  DATEPART(SECOND, fecha)= 0
               THEN  DATEADD(HOUR, DATEPART(HOUR, hora),  DATEADD(MINUTE, DATEPART(MINUTE, hora), DATEADD(SECOND, DATEPART(SECOND, hora), fecha))  ) 
               ELSE fecha
       END as [Fecha]
      --,[fecha] AS [Fecha]
      ,[turno] AS [Turno]
      ,[hora] AS [Hora]
      ,[orden_sap] AS [Orden SAP]
      ,[orden_sap_2] AS [Orden en SAP 2]
      ,[pieza_por_golpe] AS [Pieza por Golpe]
      ,[numero_rollo] AS [N° de Rollo]
      ,[lote_rollo] AS [Lote de rollo]
      ,[peso_etiqueta] AS [Peso Etiqueta (Kg)]
      ,[peso_regreso_rollo_real] AS [Peso de regreso de rollo Real]
      ,[peso_rollo_usado] AS [Peso de rollo usado]
      ,[peso_bascula_kgs] AS [Peso Báscula Kgs]
      ,[piezas_por_paquete] AS [Piezas por paquete]
      ,[total_piezas] AS [Total de piezas]
      ,[peso_rollo_consumido] AS [Peso de rollo consumido]
      ,[numero_golpes] AS [Numero de golpes]
	  ,null AS  [Kg restante de rollo]
      ,[peso_despunte_kgs] AS [Peso despunte kgs#]
      ,[peso_cola_kgs] AS [Peso cola Kgs#]
      ,[porcentaje_punta_y_colas] AS [Porcentaje de puntas y colas]
      ,[total_piezas_ajuste] AS [Total de piezas de Ajustes]
      ,[peso_bruto_kgs] AS [Peso Bruto_Kgs]
      ,[peso_real_pieza_bruto] AS [Peso Real Pieza Bruto]
      ,[peso_real_pieza_neto] AS [Peso Real Pieza Neto]
      ,[scrap_natural] AS [Scrap Natural]
      ,[peso_neto_sap] AS [Peso neto SAP]
      ,[peso_bruto_sap] AS [Peso Bruto SAP]
      ,[balance_scrap] AS [Balance_de_Scrap]
	  ,[linea] AS [Linea]
	  ,[planta] AS [Planta]      
	  ,YEAR(fecha) AS [Anio]
	  ,null as [Column40]
	  ,[ordenes_por_pieza] AS [Ordenes por pieza]
      ,[peso_rollo_usado_real_kgs] AS [Peso de rollo usado real _Kg]
      ,[peso_bruto_total_piezas_kgs] AS [Peso bruto Total piezas_Kg]
      ,[peso_neto_total_piezas_kgs] AS [Peso NetoTotal piezas_Kg]
      ,[scrap_ingenieria_buenas_mas_ajuste] AS [Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
      ,[peso_neto_total_piezas_ajuste] AS [Peso Neto_total piezas de ajuste_Kgs]
      ,[peso_punta_y_colas_reales] AS  [Peso puntas y colas reales_Kg]
      ,[balance_scrap_real] AS [Balance_de_Scrap_Real]
  FROM [dbo].[produccion_respaldo]

  UNION ALL

SELECT
	--ISNULL(ROW_NUMBER() OVER (ORDER BY v2.fecha), 0) as id,
	v2.* FROM (SELECT v1.* FROM (
	
	SELECT distinct d2.* FROM(
	SELECT  [Operador ] as [Operador]
      ,[Supervisor ] as [Supervisor]
      ,[SAP Platina ] as [SAP Platina]
	    ,null as [SAP Platina 2]
      ,[Tipo de Material]
      ,[Número de Parte  de cliente]
      ,[SAP Rollo]
      ,[Material]
      , CASE
            WHEN DATEPART(HOUR, fecha) = 0 AND DATEPART(MINUTE, fecha)= 0 AND  DATEPART(SECOND, fecha)= 0
               THEN  DATEADD(HOUR, DATEPART(HOUR, [Hora ]),  DATEADD(MINUTE, DATEPART(MINUTE, [Hora ]), DATEADD(SECOND, DATEPART(SECOND, [Hora ]), fecha))  ) 
               ELSE fecha
       END as [Fecha]
      --,[fecha] AS [Fecha]
      ,[Turno]
      ,[Hora ] AS [Hora]
      ,[Orden SAP]
      ,[Orden en SAP 2]
      ,[Pieza por Golpe]
      ,[N° de Rollo]
      ,CONVERT(varchar(256), CAST([Lote de rollo] AS decimal(10, 0))) AS [Lote de rollo]
      ,[Peso Etiqueta (Kg)]
	  ,CONVERT(float, CASE WHEN ISNUMERIC([Peso de regreso de rollo Real]) = 1 THEN [Peso de regreso de rollo Real] ELSE NULL END) AS [Peso de regreso de rollo Real]
      ,[Peso de rollo usado ] AS [Peso de rollo usado]
      ,[Peso Báscula Kgs]
      --,[N° Lote izquierdo]
      --,[N° Lote derecho]
      --,[#]
      ,[Piezas por paquete]
      ,[Total de piezas]
	  ,null AS [Peso de rollo consumido]
      ,[Numero de golpes]
      ,[Kg restante de rollo ] AS [Kg restante de rollo]
      ,[Peso despunte kgs#]
      ,[Peso cola Kgs#]
      ,[Porcentaje de puntas y colas]
     ,CONVERT(float, CASE WHEN ISNUMERIC([Total de piezas de Ajustes]) = 1 THEN [Total de piezas de Ajustes] ELSE NULL END) AS [Total de piezas de Ajustes]
      ,[Peso Bruto_Kgs]
      ,[Peso Real Pieza Bruto]
      ,[Peso Real Pieza Neto]
      ,[Scrap Natural]
      ,[Peso neto SAP]
      ,[Peso Bruto SAP]
      ,[Balance_de_Scrap]
      --,[Linea]
      ,ISNULL([Linea],SUBSTRING( [Operador ],1,10)) AS [Linea]
      ,ISNULL([Column38],'Puebla') as [Planta]
      ,YEAR(Fecha) AS [Anio]
      ,null as [Column40]
      ,[Ordenes por pieza]
      ,[Peso de rollo usado real _Kg]
      ,[Peso bruto Total piezas_Kg]
      ,[Peso NetoTotal piezas_Kg]
      ,[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
      ,[Peso Neto_total piezas de ajuste_Kgs]
      ,[Peso puntas y colas reales_Kg]
      ,[Balance_de_Scrap_Real]
  FROM [cube_tkmm].[dbo].[view_union_bitacoras_puebla]
  )d2

  UNION ALL
  SELECT distinct d1.* FROM(
	SELECT [Operador]
      ,[Supervisor]
      ,[SAP Platina]
	    ,null as [SAP Platina 2]
      ,[Tipo de Material]
      ,[Número de Parte  de cliente]
      ,[SAP Rollo]
      ,[Material]
       , CASE
            WHEN DATEPART(HOUR, fecha) = 0 AND DATEPART(MINUTE, fecha)= 0 AND  DATEPART(SECOND, fecha)= 0
               THEN  DATEADD(HOUR, DATEPART(HOUR, [Hora]),  DATEADD(MINUTE, DATEPART(MINUTE, [Hora]), DATEADD(SECOND, DATEPART(SECOND, [Hora]), fecha))  ) 
               ELSE fecha
       END as [Fecha]
      --,[fecha] AS [Fecha]
      ,[Turno]
      ,[Hora]
      ,[Orden SAP]
      ,[Orden en SAP 2]
      ,[Pieza por Golpe]
      ,[N° de Rollo]
      ,CONVERT(varchar(256), CAST([Lote de rollo] AS decimal(10, 0))) AS [Lote de rollo]
      ,[Peso Etiqueta (Kg)]
      ,CONVERT(float, CASE WHEN ISNUMERIC([Peso de regreso de rollo Real]) = 1 THEN [Peso de regreso de rollo Real] ELSE NULL END) AS [Peso de regreso de rollo Real]
      ,[Peso de rollo usado]
      ,[Peso Báscula Kgs]
      --,[N° Lote izquierdo]
      --,[N° Lote derecho]
      --,[#]
      ,[Piezas por paquete]
      ,[Total de piezas]
      ,[Peso de rollo consumido]
	  ,null as [Numero de golpes]
      ,null as [Kg restante de rollo]
      ,[Peso despunte kgs#]
      ,[Peso cola Kgs#]
      ,[Porcentaje de puntas y colas]
      ,[Total de piezas de Ajustes]
      ,[Peso Bruto Kgs]
      ,[Peso Real Pieza Bruto]
      ,[Peso Real Pieza Neto]
      ,[Scrap Natural]
      ,[Peso neto SAP]
      ,[Peso Bruto SAP]
      ,[Balance de Scrap]
	 -- ,null as [Linea]
      ,ISNULL([Column37],SUBSTRING( [Operador],1,10)) AS [Linea]
      ,ISNULL([Column38],'Silao') as [Planta]
      ,YEAR(Fecha) AS [Anio]
      ,null as [Column40]
      ,[Ordenes por pieza]
      ,[Peso de rollo usado real _Kg]
      ,[Peso bruto Total piezas_Kg]
      ,[Peso NetoTotal piezas_Kg]
      ,[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
      ,[Peso Neto_total piezas de ajuste_Kgs]
      ,[Peso puntas y colas reales_Kg]
      ,[Balance_de_Scrap_Real]
  FROM [cube_tkmm].[dbo].[view_union_bitacoras_silao]
  )d1

 ) v1

 UNION ALL

SELECT  
		--[id]
      [Operador]
      ,[Supervisor]
      ,[SAP Platina]
	  ,[SAP Platina 2]
      ,[Tipo de Material]
      ,[Número de Parte de Cliente]
      ,[SAP Rollo]
      ,[Material]
      ,[Fecha]
      ,[Turno]
      ,[Hora]
      ,[Orden SAP]
      ,[Orden en SAP 2]
      ,[Pieza por Golpe]
      ,[N° de Rollo]
      ,CONVERT(varchar(256), CAST([Lote de rollo] AS decimal(10, 0))) AS [Lote de rollo]
      ,[Peso Etiqueta (Kg)]
      ,[Peso de regreso de rollo Real]
      ,[Peso de rollo usado ]
      ,[Peso Báscula Kgs]
      --,[N° Lote izquierdo]
      --,[N° Lote derecho]
      --,[#]
      ,[Piezas por paquete]
      ,[Total de piezas]
	  ,null AS [Peso de rollo consumido]
      ,[Numero de golpes]
      ,[Kg restante de rollo]
      ,[Peso despunte kgs#]
      ,[Peso cola Kgs#]
      ,[Porcentaje de puntas y colas]
      ,[Total de piezas de Ajustes]
      ,[Peso Bruto Kgs]
      ,[Peso Real Pieza Bruto]
      ,[Peso Real Pieza Neto]
      ,[Scrap Natural]
      ,[Peso neto SAP]
      ,[Peso Bruto SAP]
      ,[Balance de Scrap]
      ,[Column37] AS [Linea]
      ,[Column38] AS [Planta]
      ,[Column39] AS [Anio]
	  ,CONVERT(float, [Column40]) AS [Column40]
      ,[Ordenes por pieza]
      ,[Peso de rollo usado real _Kg]
      ,[Peso bruto Total piezas_Kg]
      ,[Peso NetoTotal piezas_Kg]
      ,[Scrap de ingeniería (buenas + Ajuste)_Total_Piezas_Kg]
      ,[Peso Neto_total piezas de ajuste_Kgs]
      ,[Peso puntas y colas reales_Kg]
      ,[Balance_de_Scrap_Real]
  FROM [Portal_2_0].[dbo].[view_produccion_resultados])v2
	WHERE Fecha is not null)v3 
