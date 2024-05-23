--USE [Portal_2_0]
GO

/****** Object:  StoredProcedure [dbo].[sp_reporte_pesadas]    Script Date: 23/05/2024 12:17:18 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




CREATE OR ALTER           PROCEDURE [dbo].[sp_reporte_pesadas]
/*****************************************************************************
*  Tipo de objeto: Stored Procedure
*  Funcion: Obtiene los registros del reporte de pesadas
*  Autor :  Alfredo Xochitemol Cruz
*  Fecha de Creación: 10/12/2021
*  Log de Mantenimiento: 
*  Date          Modified By             Description            
*  ----------    --------------------    -------------------------------------
*  
******************************************************************************/
(  
	@id_planta int,
	@cliente Varchar(200), 
	@material Varchar(20), 
	@fecha_inicio datetime,  
	@fecha_fin datetime,
	@muestra int
 )  
AS   
    BEGIN  
    -- Insert statements for procedure here  
   
select 
  z.*  
  , ROUND((z.peso_neto_sap - z.peso_neto_mean_muestra)/z.peso_neto_sap, 4) as diferencia_porcentaje_muestra_rollo
  ,  (CASE
		WHEN abs( ROUND((z.peso_neto_sap - z.peso_neto_mean_muestra)/z.peso_neto_sap,4)) > z.porcentaje_tolerancia
				THEN 0					
			ELSE 1
		END) as muestra_dentro_del_rango
from 
  (
SELECT 
			ISNULL(ROW_NUMBER() OVER (ORDER BY pr.[sap_platina]), 0) as id
			 ,pr.[sap_platina]
			 ,sap_rollo
			 ,[invoiced_to]
			 ,[tipo_metal]
			 ,ROUND([net_weight],3) AS 'peso_neto_sap'
			 ,ROUND(AVG([peso_real_pieza_neto]),3) AS 'peso_neto_mean'
			 ,ROUND(STDEV([peso_real_pieza_neto]),3) AS 'peso_neto_stdev'
			 ,COUNT([sap_platina]) as 'count'
			 --,AVG([Net weight]) - AVG([Peso Real Pieza Neto]) AS 'Diferencia Peso Neto'
			 ,[Thickness]
			 ,[Width]
			 ,[Advance]
			 ,ROUND([peso_teorico],3) AS [peso_teorico]
			 ,ROUND([gross_weight],3) AS 'peso_bruto_sap'
			 ,ROUND(AVG([peso_real_pieza_bruto]),3) AS 'peso_bruto_mean'
			 ,ROUND([gross_weight] - AVG([peso_real_pieza_bruto]),3) AS 'diferencia_peso_bruto_mean_peso_real_bruto_mean'
			 ,ROUND([gross_weight] - [peso_teorico], 3) AS 'diferencia_peso_bruto_mean_peso_teorico'
			 ,SUM([total_piezas]) as [total_piezas]
			 ,SUM([peso_etiqueta]) as [peso_etiqueta]
			 , fecha_inicio_validez_peso_bom
			 , fecha_fin_validez_peso_bom
			 , (CASE
					WHEN net_weight >= 0 and  net_weight < 8
						THEN 'Pequeña'
					WHEN net_weight >= 8 and  net_weight < 20
						THEN 'Mediana'
					WHEN net_weight >= 20 and  net_weight < 40
						THEN 'Grande'
					ELSE NULL
				END) as tamano_pieza
				, (CASE
				WHEN net_weight >= 0 and  net_weight < 8
					THEN 0.02
				WHEN net_weight >= 8 and  net_weight < 20
					THEN 0.015
				WHEN net_weight >= 20 and  net_weight < 40
					THEN 0.01
				ELSE NULL
			END) as porcentaje_tolerancia
			, ROUND((net_weight- AVG([peso_real_pieza_neto]))/net_weight,4) as diferencia_porcentaje_total_rollos
			, (select count (r.c) FROM( ((select top (@muestra) x.[peso_real_pieza_neto] as c from view_datos_base_reporte_pesadas x
				where x.[peso_real_pieza_neto]>0 AND x.[peso_real_pieza_bruto]>0 
				AND x.[invoiced_to] LIKE '%'+@cliente+'%'
				AND x.fecha between @fecha_inicio AND @fecha_fin
				AND x.clave_planta = @id_planta
				AND x.sap_platina = pr.[sap_platina]
				AND x.fecha_inicio_validez_peso_bom = pr.fecha_inicio_validez_peso_bom
				AND x.fecha_fin_validez_peso_bom = pr.fecha_fin_validez_peso_bom
				order by x.fecha desc			
			)))r) as tamano_muestra
			, ROUND((select avg (r.c) FROM( ((select top (@muestra) x.[peso_real_pieza_neto] as c from view_datos_base_reporte_pesadas x
				where x.[peso_real_pieza_neto]>0 AND x.[peso_real_pieza_bruto]>0 
				AND x.[invoiced_to] LIKE '%'+@cliente+'%'
				AND x.fecha between @fecha_inicio AND @fecha_fin
				AND x.clave_planta = @id_planta
				AND x.sap_platina = pr.[sap_platina]
				AND x.fecha_inicio_validez_peso_bom = pr.fecha_inicio_validez_peso_bom
				AND x.fecha_fin_validez_peso_bom = pr.fecha_fin_validez_peso_bom
				order by x.fecha desc
			)))r),3) as peso_neto_mean_muestra,
			(select Sum(r.c) FROM( ((select top (@muestra) x.total_piezas as c from view_datos_base_reporte_pesadas x
				where x.[peso_real_pieza_neto]>0 AND x.[peso_real_pieza_bruto]>0 
				AND x.[invoiced_to] LIKE '%'+@cliente+'%'
				AND x.fecha between @fecha_inicio AND @fecha_fin
				AND x.clave_planta = @id_planta
				AND x.sap_platina = pr.[sap_platina]
				AND x.fecha_inicio_validez_peso_bom = pr.fecha_inicio_validez_peso_bom
				AND x.fecha_fin_validez_peso_bom = pr.fecha_fin_validez_peso_bom
				order by x.fecha desc
			)))r) as total_piezas_muestra
		  FROM [dbo].[view_datos_base_reporte_pesadas] pr
		  where [peso_real_pieza_neto]>0 AND [peso_real_pieza_bruto]>0 
		  --busqueda
		  AND [invoiced_to] LIKE '%'+@cliente+'%'
		  AND fecha between @fecha_inicio AND @fecha_fin
		  AND clave_planta = @id_planta
		  AND pr.sap_platina LIKE '%'+@material+'%'
		 group by pr.[sap_platina],[sap_rollo],[invoiced_to], [tipo_metal], [Thickness], [Width], [Advance], [peso_teorico], [fecha_inicio_validez_peso_bom], [fecha_fin_validez_peso_bom], [net_weight], [gross_weight]
		 --order by pr.[sap_platina] ASC
		 )z

    END  
GO

