use[Portal_2_0]
DROP VIEW IF EXISTS dbo.view_valores_anio_fiscal;  
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
CREATE VIEW [dbo].view_valores_anio_fiscal AS

SELECT ISNULL(ROW_NUMBER() OVER (ORDER BY t1.id_cuenta_sap), 0) as id,
t1.* FROM (
SELECT  --id_rel_anio_centro,
		r.id_anio_fiscal,		
		r.id_centro_costo,
        [id_cuenta_sap],
		currency_iso,		
        SUM(CASE WHEN [Mes] = 1 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-01-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 1 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-01-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 1 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Enero],
		SUM(CASE WHEN [Mes] =2 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-02-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01'))  
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 2 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-02-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 2 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Febrero],
		SUM(CASE WHEN [Mes] =3 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-03-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 3 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-03-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 3 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Marzo],
			SUM(CASE WHEN [Mes] =4 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-04-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 4 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-04-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 4 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Abril],
			SUM(CASE WHEN [Mes] =5 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-05-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 5 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-05-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 5 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Mayo],
			SUM(CASE WHEN [Mes] =6 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-06-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 6 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-06-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 6 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Junio],
			SUM(CASE WHEN [Mes] =7 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-07-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 7 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-07-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 7 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Julio],
			SUM(CASE WHEN [Mes] =8 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-08-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01'))  
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 8 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-08-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 8 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Agosto],
			SUM(CASE WHEN [Mes] =9 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_fin,'-09-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 9 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-09-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 9 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Septiembre],
			SUM(CASE WHEN [Mes] =10 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_inicio,'-10-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 10 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_inicio,'-10-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 10 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Octubre],
			SUM(CASE WHEN [Mes] =11 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_inicio,'-11-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')> CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01'))  
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 11 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_inicio,'-11-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 11 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Noviembre],
			SUM(CASE WHEN [Mes] =12 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<=  CONVERT(datetime, CONCAT(a.anio_inicio,'-12-01'))
					AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_fin,'-',a.mes_fin,'-01')) 
					AND r.tipo='FORECAST' THEN [cantidad]
				WHEN [Mes] = 12 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')>  CONVERT(datetime, CONCAT(a.anio_inicio,'-12-01')) AND r.tipo='ACTUAL' THEN [cantidad]
				WHEN [Mes] = 12 AND CONCAT(YEAR(GETDATE()),'-',MONTH(GETDATE()),'-01')<  CONVERT(datetime, CONCAT(a.anio_inicio,'-',a.mes_inicio,'-01')) AND r.tipo='BUDGET' THEN [cantidad]
				ELSE NULL END) [Diciembre],
			(SELECT top(1) comentario FROM budget_comentarios_rel_anio_cuenta as c WHERE c.id_cuenta_sap=v.id_cuenta_sap AND c.id_anio_fiscal=r.id_anio_fiscal AND c.id_centro_costo = r.id_centro_costo) AS [Comentario]
FROM budget_valores as v
		join budget_rel_anio_fiscal_centro as r  on r.id=id_rel_anio_centro
		join budget_anio_fiscal as a on a.id=r.id_anio_fiscal
GROUP BY --id_rel_anio_centro,
		r.id_anio_fiscal,
		r.id_centro_costo,		
        [id_cuenta_sap],
		currency_iso
		) t1