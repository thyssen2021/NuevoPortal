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
SELECT  id_rel_anio_centro,
        [id_cuenta_sap],
		currency_iso,		
        SUM(CASE WHEN [Mes] = 1 THEN [Cantidad] ELSE NULL END) [Enero],
		SUM(CASE WHEN [Mes] = 2 THEN [Cantidad] ELSE NULL END) [Febrero],
		SUM(CASE WHEN [Mes] = 3 THEN [Cantidad] ELSE NULL END) [Marzo],
		SUM(CASE WHEN [Mes] = 4 THEN [Cantidad] ELSE NULL END) [Abril],
		SUM(CASE WHEN [Mes] = 5 THEN [Cantidad] ELSE NULL END) [Mayo],
		SUM(CASE WHEN [Mes] = 6 THEN [Cantidad] ELSE NULL END) [Junio],
		SUM(CASE WHEN [Mes] = 7 THEN [Cantidad] ELSE NULL END) [Julio],
		SUM(CASE WHEN [Mes] = 8 THEN [Cantidad] ELSE NULL END) [Agosto],
		SUM(CASE WHEN [Mes] = 9 THEN [Cantidad] ELSE NULL END) [Septiembre],
		SUM(CASE WHEN [Mes] = 10 THEN [Cantidad] ELSE NULL END) [Octubre],
		SUM(CASE WHEN [Mes] = 11 THEN [Cantidad] ELSE NULL END) [Noviembre],
		SUM(CASE WHEN [Mes] = 12 THEN [Cantidad] ELSE NULL END) [Diciembre],
		(SELECT top(1) comentario FROM budget_comentarios_rel_anio_cuenta as c WHERE c.id_cuenta_sap=v.id_cuenta_sap AND c.id_rel_anio_centro=v.id_rel_anio_centro) AS [Comentario]
FROM budget_valores as v
GROUP BY id_rel_anio_centro,
        [id_cuenta_sap],
		currency_iso) t1