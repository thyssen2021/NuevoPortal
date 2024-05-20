/****** Object:  StoredProcedure [dbo].[sp_reporte_pesadas_puebla]    Script Date: 16/05/2024 04:33:24 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_reporte_pesadas]
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
	@fecha_fin datetime
 )  
AS   
    BEGIN  
    -- Insert statements for procedure here  
   
       SELECT 
			ISNULL(ROW_NUMBER() OVER (ORDER BY dbo.view_datos_base_reporte_pesadas.[sap_platina]), 0) as id
			 ,[sap_platina]
			 ,sap_rollo
			 ,[invoiced_to]
			 ,[tipo_metal]
			 ,ROUND(AVG([net_weight]),3) AS 'peso_neto_sap'
			 ,ROUND(AVG([peso_real_pieza_neto]),3) AS 'peso_neto_mean'
			 ,ROUND(STDEV([peso_real_pieza_neto]),3) AS 'peso_neto_stdev'
			 ,COUNT([sap_platina]) as 'count'
			 --,AVG([Net weight]) - AVG([Peso Real Pieza Neto]) AS 'Diferencia Peso Neto'
			 ,[Thickness]
			 ,[Width]
			 ,[Advance]
			 ,ROUND([peso_teorico],3) AS [peso_teorico]
			 ,ROUND(AVG([gross_weight]),3) AS 'peso_bruto_sap'
			 ,ROUND(AVG([peso_real_pieza_bruto]),3) AS 'peso_bruto_mean'
			 ,ROUND(AVG([gross_weight]) - AVG([peso_real_pieza_bruto]),3) AS 'diferencia_peso_bruto_mean_peso_real_bruto_mean'
			 ,ROUND(AVG([gross_weight]) - [peso_teorico], 3) AS 'diferencia_peso_bruto_mean_peso_teorico'
			 ,SUM([total_piezas]) as [total_piezas]
			 ,SUM([peso_etiqueta]) as [peso_etiqueta]
			 , fecha_inicio_validez_peso_bom
			 , fecha_fin_validez_peso_bom
		  FROM [dbo].[view_datos_base_reporte_pesadas]
		  where [peso_real_pieza_neto]>0 AND [peso_real_pieza_bruto]>0 
		  --busqueda
		  AND [invoiced_to] LIKE '%'+@cliente+'%'
		  AND fecha between @fecha_inicio AND @fecha_fin
		  AND clave_planta = @id_planta
		  AND sap_platina LIKE '%'+@material+'%'
		 group by [sap_platina],[sap_rollo],[invoiced_to], [tipo_metal], [Thickness], [Width], [Advance], [peso_teorico], [fecha_inicio_validez_peso_bom], [fecha_fin_validez_peso_bom]
		 order by [sap_platina] ASC
    END  
GO


------------------------------------------------------------
---------------------------------------------------------------------------------------------
 --------------------------------------------------------------------------------------------

DECLARE @id_planta int = 1;  
DECLARE @cliente Varchar(200) = 'VOLKSWAGEN DE MEXICO'; 
DECLARE @material Varchar(20) = 'EG07579'; 
DECLARE @fecha_inicio datetime = '20240101 16:08';  
DECLARE @fecha_fin datetime = '20241231 16:08';  

SELECT 
			ISNULL(ROW_NUMBER() OVER (ORDER BY dbo.view_datos_base_reporte_pesadas.[sap_platina]), 0) as id
			 ,[sap_platina]
			 ,sap_rollo
			 ,[invoiced_to]
			 ,[tipo_metal]
			 ,ROUND(AVG([net_weight]),3) AS 'peso_neto_sap'
			 ,ROUND(AVG([peso_real_pieza_neto]),3) AS 'peso_neto_mean'
			 ,ROUND(STDEV([peso_real_pieza_neto]),3) AS 'peso_neto_stdev'
			 ,COUNT([sap_platina]) as 'count'
			 --,AVG([Net weight]) - AVG([Peso Real Pieza Neto]) AS 'Diferencia Peso Neto'
			 ,[Thickness]
			 ,[Width]
			 ,[Advance]
			 ,ROUND([peso_teorico],3) AS [peso_teorico]
			 ,ROUND(AVG([gross_weight]),3) AS 'peso_bruto_sap'
			 ,ROUND(AVG([peso_real_pieza_bruto]),3) AS 'peso_bruto_mean'
			 ,ROUND(AVG([gross_weight]) - AVG([peso_real_pieza_bruto]),3) AS 'diferencia_peso_bruto_mean_peso_real_bruto_mean'
			 ,ROUND(AVG([gross_weight]) - [peso_teorico], 3) AS 'diferencia_peso_bruto_mean_peso_teorico'
			 ,SUM([total_piezas]) as [total_piezas]
			 ,SUM([peso_etiqueta]) as [peso_etiqueta]
			 , fecha_inicio_validez_peso_bom
			 , fecha_fin_validez_peso_bom
		  FROM [dbo].[view_datos_base_reporte_pesadas]
		  where [peso_real_pieza_neto]>0 AND [peso_real_pieza_bruto]>0 
		  --busqueda
		  AND [invoiced_to] LIKE '%'+@cliente+'%'
		  AND fecha between @fecha_inicio AND @fecha_fin
		  AND clave_planta = @id_planta
		  AND sap_platina LIKE '%'+@material+'%'
		 group by [sap_platina],[sap_rollo],[invoiced_to], [tipo_metal], [Thickness], [Width], [Advance], [peso_teorico], [fecha_inicio_validez_peso_bom], [fecha_fin_validez_peso_bom]
		 order by [sap_platina] ASC